Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils

Public Class CSyncCasino
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Public Sub SyncCasinoAmount(ByVal poDate As Date)
        Dim oAseDB As OleDb.OleDbConnection = Nothing
        Dim oDB As CSQLDBUtils = Nothing

        Try
            Dim nCasinoID As Integer
            ' Create ASE connection
            oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
            oAseDB.Open()

            ' Create Tiger Connection
            oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            '' Get All Players
            Dim odtPlayer As DataTable = GetAllPlayer(oDB)
            For Each odrPlayer As DataRow In odtPlayer.Rows
                ' Get Casino Account ID
                nCasinoID = CasinoID(oAseDB, GetCasinoLogin(SafeString(odrPlayer("Login"))))

                If nCasinoID = 0 Then ' do not exist
                    ' First: get CasinoAmount
                    Dim nCasinoBalance As Double = 0
                    If SafeDouble(odrPlayer("OriginalAmount")) > 0 Then
                        nCasinoBalance = SafeDouble(odrPlayer("CasinoAmount"))
                    End If

                    InsertCasinoAccount(oAseDB, GetCasinoLogin(SafeString(odrPlayer("Login"))), SafeString(odrPlayer("Password")), _
                                        SafeString(odrPlayer("Name")), SafeBoolean(odrPlayer("IsLocked")), nCasinoBalance)
                Else ' exist
                    '' Update lock/unclock casino account
                    Dim bLock As Boolean = SafeBoolean(odrPlayer("IsLocked")) AndAlso SafeDouble(odrPlayer("OriginalAmount")) > 0
                    UpdateCasinoAccount(oAseDB, nCasinoID, SafeString(odrPlayer("Password")), SafeString(odrPlayer("Name")), bLock)

                    '' Sync Casino's amount
                    Dim nCasinoAmount As Double = 0
                    nCasinoAmount = GetCasinoAmount(oAseDB, nCasinoID, poDate)
                    If nCasinoAmount <> 0 Then
                        SyncPlayerAmount(oDB, SafeString(odrPlayer("PlayerID")), SafeString(odrPlayer("AgentID")), _
                                     SafeDouble(odrPlayer("BalanceAmount")), nCasinoAmount, "Casino Played", "")
                    End If
                End If
            Next

        Catch ex As Exception
            log.Error("Fail to sync casino account. Message: " & ex.Message, ex)
        Finally
            If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            If oDB IsNot Nothing Then oDB.closeConnection()
        End Try

    End Sub

    Public Sub InitialCasinoAmount()
        Dim oAseDB As OleDb.OleDbConnection = Nothing
        Dim oDB As CSQLDBUtils = Nothing

        Try
            Dim nCasinoID As Integer
            ' Create ASE connection
            oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
            oAseDB.Open()

            ' Create Tiger Connection
            oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            '' Get All Players
            Dim odtPlayer As DataTable = GetAllPlayer(oDB)
            For Each odrPlayer As DataRow In odtPlayer.Rows
                ' First: get CasinoAmount
                Dim nCasinoBalance As Double = 0
                If SafeDouble(odrPlayer("OriginalAmount")) > 0 Then
                    nCasinoBalance = SafeDouble(odrPlayer("CasinoAmount"))
                End If

                ' Get Casino Account ID
                nCasinoID = CasinoID(oAseDB, GetCasinoLogin(SafeString(odrPlayer("Login"))))

                If nCasinoID = 0 Then ' do not exist
                    InsertCasinoAccount(oAseDB, GetCasinoLogin(SafeString(odrPlayer("Login"))), SafeString(odrPlayer("Password")), _
                                        SafeString(odrPlayer("Name")), SafeBoolean(odrPlayer("IsLocked")), nCasinoBalance)
                Else ' exist
                    '' Update lock/unclock casino account
                    Dim bLock As Boolean = SafeBoolean(odrPlayer("IsLocked")) AndAlso SafeDouble(odrPlayer("OriginalAmount")) > 0
                    UpdateInitialCasinoAmount(oAseDB, nCasinoID, SafeString(odrPlayer("Password")), SafeString(odrPlayer("Name")), bLock, nCasinoBalance)
                End If
            Next

        Catch ex As Exception
            log.Error("Fail to set initial casino account. Message: " & ex.Message, ex)
        Finally
            If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            If oDB IsNot Nothing Then oDB.closeConnection()
        End Try

    End Sub

#Region "Methods"
    Public Function GetCasinoLogin(ByVal psLogin As String) As String
        Return psLogin & CASINO_SUFFIX
    End Function

    Public Function GetAllPlayer(ByVal poDB As CSQLDBUtils) As DataTable
        Dim odtPlayer As DataTable = New DataTable()

        Dim sSQL As String = "SELECT p.PLayerID, p.[Password], p.AgentID, p.[Login], p.BalanceAmount, p.[Login], p.[Name], p.OriginalAmount, " & _
            "p.IsLocked, pt.CasinoMaxAmount AS CasinoAmount " & vbCrLf & _
            "FROM PLayers p INNER JOIN PlayerTemplates pt ON pt.PlayerTemplateID = ISNULL(p.PlayerTemplateID, p.DefaultPlayerTemplateID)"

        log.Debug("Get All Player. SQL: " & sSQL)
        odtPlayer = poDB.getDataTable(sSQL)

        Return odtPlayer
    End Function

    Public Function SyncPlayerAmount(ByVal poDB As CSQLDBUtils, ByVal psPlayerID As String, ByVal psAgentID As String, _
                                     ByVal pnBalance As Double, ByVal pnAddAmount As Double, _
                                     ByVal psDescription As String, ByVal psUpdateBy As String) As Boolean
        Dim oSQLEdit As ISQLEditStringBuilder
        Dim oEstDate As Date = GetEasternDate()

        '' 1st: Get Player's Balance Amount
        Dim nCurrentBalanceAmount As Double = pnBalance

        '' 2nd: Create Games
        Dim sGameID As String = newGUID()
        oSQLEdit = New CSQLInsertStringBuilder("Games")
        oSQLEdit.AppendString("GameID", SQLString(sGameID))
        oSQLEdit.AppendString("GameDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("HomeTeam", SQLString(psDescription))
        oSQLEdit.AppendString("GameStatus", SQLString("Credit Back"))
        oSQLEdit.AppendString("FinalCheckStartedProcessing", SQLDate(oEstDate))
        oSQLEdit.AppendString("FinalCheckCompleted", SQLDate(oEstDate))
        oSQLEdit.AppendString("FirstHalfProcessedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("FirstQuaterProcessedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("SecondQuaterProcessedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("ThirdQuaterProcessedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("GameSuspendProcessedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("CheckCompletedDate", SQLDate(oEstDate))
        poDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

        '' 3rd: Create GameLines
        Dim sGameLineID As String = newGUID()
        oSQLEdit = New CSQLInsertStringBuilder("GameLines")
        oSQLEdit.AppendString("GameLineID", SQLString(sGameLineID))
        oSQLEdit.AppendString("GameID", SQLString(sGameID))
        oSQLEdit.AppendString("LastUpdated", SQLDate(oEstDate))
        oSQLEdit.AppendString("Context", SQLString("Current"))
        oSQLEdit.AppendString("HomeMoneyLine", SQLDouble(100))
        poDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

        '' 4th: Create Tickets
        Dim sTicketID As String = newGUID()
        oSQLEdit = New CSQLInsertStringBuilder("Tickets")
        oSQLEdit.AppendString("TicketID", SQLString(sTicketID))
        'oSQLEdit.AppendString("TicketNumber", SQLDouble(0))
        'oSQLEdit.AppendString("SubTicketNumber", SQLDouble(0))
        oSQLEdit.AppendString("AgentID", SQLString(psAgentID))
        oSQLEdit.AppendString("PlayerID", SQLString(psPlayerID))
        oSQLEdit.AppendString("TransactionDate", SQLDate(oEstDate))
        If psUpdateBy <> "" Then
            oSQLEdit.AppendString("OrderBy", SQLString(psUpdateBy))
        End If
        oSQLEdit.AppendString("TicketType", SQLString("Straight"))

        If pnAddAmount > 0 Then
            oSQLEdit.AppendString("RiskAmount", SQLDouble(0))
            oSQLEdit.AppendString("WinAmount", SQLDouble(pnAddAmount))
            oSQLEdit.AppendString("NetAmount", SQLDouble(pnAddAmount))
            oSQLEdit.AppendString("TicketStatus", SQLString("WIN"))
            oSQLEdit.AppendString("BetAmount", SQLDouble(0))
        Else
            oSQLEdit.AppendString("RiskAmount", SQLDouble(Math.Abs(pnAddAmount)))
            oSQLEdit.AppendString("WinAmount", SQLDouble(Math.Abs(pnAddAmount)))
            oSQLEdit.AppendString("NetAmount", SQLDouble(0))
            oSQLEdit.AppendString("TicketStatus", SQLString("LOSE"))
            oSQLEdit.AppendString("BetAmount", SQLDouble(Math.Abs(pnAddAmount)))
        End If

        oSQLEdit.AppendString("TypeOfBet", SQLString("Internet"))
        oSQLEdit.AppendString("TicketCompletedDate", SQLDate(oEstDate))
        oSQLEdit.AppendString("NumOfBets", SQLDouble(1))
        oSQLEdit.AppendString("CheckCompletedDate", SQLDate(oEstDate))
        poDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

        '' 5th: Insert TicketBets
        oSQLEdit = New CSQLInsertStringBuilder("TicketBets")
        oSQLEdit.AppendString("TicketBetID", "NEWID()")
        oSQLEdit.AppendString("TicketID", SQLString(sTicketID))
        oSQLEdit.AppendString("BetType", SQLString("MoneyLine"))
        oSQLEdit.AppendString("GameID", SQLString(sGameID))
        oSQLEdit.AppendString("HomeMoneyLine", SQLDouble(100))

        If pnAddAmount > 0 Then
            oSQLEdit.AppendString("TicketBetStatus", SQLString("WIN"))
        Else
            oSQLEdit.AppendString("TicketBetStatus", SQLString("LOSE"))
        End If

        oSQLEdit.AppendString("GameLineID", SQLString(sGameLineID))
        oSQLEdit.AppendString("Context", SQLString("Current"))
        poDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

        '' 6th: Update Player's Amount
        oSQLEdit = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID = " & SQLString(psPlayerID))
        oSQLEdit.AppendString("BalanceAmount", SQLDouble(nCurrentBalanceAmount + pnAddAmount))

        poDB.executeNonQuery(oSQLEdit, psUpdateBy, True)
        poDB.commitTransaction()

        Return True
    End Function

    Public Function CasinoID(ByVal poAseDB As OleDb.OleDbConnection, ByVal psCasinoLogin As String) As Integer
        Dim oCommand As OleDb.OleDbCommand
        Dim sSQL As String = "SELECT AID FROM casino_account WHERE EMAIL = " & SQLString(psCasinoLogin)
        log.Debug("Get CasinoID. SQL: " & sSQL)

        oCommand = New OleDb.OleDbCommand(sSQL, poAseDB)
        oCommand.CommandType = CommandType.Text

        Return SafeInteger(oCommand.ExecuteScalar())
    End Function

    Public Function GetCasinoAmount(ByVal poAseDB As OleDb.OleDbConnection, ByVal pnCasinoID As Integer, ByVal poDate As Date) As Double

        Dim oCommand As OleDb.OleDbCommand
        Dim oWhere As New CSQLWhereStringBuilder()
        oWhere.AppendANDCondition("AID =" & SafeString(pnCasinoID))
        oWhere.AppendANDCondition(Replace(getSQLDateRange("[TIME]", poDate.AddDays(-1).ToString("MM-dd-yyyy 01:00:00"), _
                                                  poDate.ToString("MM-dd-yyyy 01:00:00"), True), "/", "-"))

        Dim sSQL As String = "SELECT SUM([WIN] - [BET]) AS Amount FROM casino_betArchive " & oWhere.SQL
        log.Debug("Get Casino's Amount. SQL: " & sSQL)

        oCommand = New OleDb.OleDbCommand(sSQL, poAseDB)
        oCommand.CommandType = CommandType.Text
        oCommand.CommandText = sSQL

        Return SafeDouble(oCommand.ExecuteScalar())
    End Function

    Public Sub UpdateCasinoAccount(ByVal poAseDB As OleDb.OleDbConnection, ByVal pnCasinoID As Integer, ByVal psCasinoPass As String, _
                                   ByVal psName As String, ByVal pbIsLocked As Boolean)
        Dim oCommand As OleDb.OleDbCommand
        Dim sSQL As String = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, FIRSTNAME = {1}, PASSWORD= {2}  WHERE AID = {3}", _
                                           SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), SQLString(psCasinoPass), SafeString(pnCasinoID))
        log.Debug("Update Casino account. SQL: " & sSQL)

        oCommand = New OleDb.OleDbCommand(sSQL, poAseDB)
        oCommand.CommandType = CommandType.Text
        oCommand.ExecuteNonQuery()
    End Sub

    Public Sub UpdateInitialCasinoAmount(ByVal poAseDB As OleDb.OleDbConnection, ByVal pnCasinoID As Integer, ByVal psCasinoPass As String, _
                                   ByVal psName As String, ByVal pbIsLocked As Boolean, ByVal pnInitialAmount As Double)
        Dim oCommand As OleDb.OleDbCommand
        Dim sSQL As String = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, FIRSTNAME = {1}, PASSWORD= {2}, BALANCE= {3} WHERE AID = {4}", _
                                           SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), SQLString(psCasinoPass), SafeString(pnInitialAmount), SafeString(pnCasinoID))
        log.Debug("Update Casino account. SQL: " & sSQL)

        oCommand = New OleDb.OleDbCommand(sSQL, poAseDB)
        oCommand.CommandType = CommandType.Text
        oCommand.ExecuteNonQuery()
    End Sub

    Public Sub InsertCasinoAccount(ByVal poAseDB As OleDb.OleDbConnection, ByVal psCasinoLogin As String, ByVal psCasinoPass As String, _
                                   ByVal psName As String, ByVal pbIsLocked As Boolean, ByVal pnBalance As Double)

        Dim nCasinoID As Integer = 0
        Dim oCommand As New OleDb.OleDbCommand()
        oCommand.Connection = poAseDB
        oCommand.CommandType = CommandType.StoredProcedure

        ' Add User
        oCommand.CommandText = "casino_proc_AddAccount"
        oCommand.Parameters.AddWithValue("@email", psCasinoLogin)
        oCommand.Parameters.AddWithValue("@password", psCasinoPass)
        oCommand.ExecuteNonQuery()

        ' Get UserID
        oCommand.CommandText = "casino_proc_GetUserID"
        oCommand.Parameters.Clear()
        oCommand.Parameters.AddWithValue("@email", psCasinoLogin)
        oCommand.Parameters.AddWithValue("@pass", psCasinoPass)

        nCasinoID = SafeInteger(oCommand.ExecuteScalar())

        If nCasinoID > 0 Then ' Add User successfully
            ' Update User's Info
            Dim sSQL As String = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, FIRSTNAME = {1}, BALANCE= {2} WHERE AID = {3}", _
                                           SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), SafeString(pnBalance), SafeString(nCasinoID))
            log.Debug("Update Casino account. SQL: " & sSQL)

            oCommand.CommandType = CommandType.Text
            oCommand.CommandText = sSQL
            oCommand.Parameters.Clear()

            oCommand.ExecuteNonQuery()
        End If
    End Sub
#End Region
End Class
