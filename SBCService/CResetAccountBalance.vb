Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils

Public Class CResetAccountBalance
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())


    Public Sub ResetAccountBalance(ByVal poMonday As Date)

        '' Update Player's OriginalAmount first
        UpdatePlayerOriginalAmount()

        '' Get All Players
        Dim odtPlayer As DataTable = GetAllPlayer()

        For Each odrPlayer As DataRow In odtPlayer.Rows
            '' Reset Balance Amount
            Dim nPendingAmount As Double
            Dim nBalanceAmount As Double
            nPendingAmount = SafeDouble(GetPlayerPendingAmount(SafeString(odrPlayer("PLayerID"))))
            nBalanceAmount = SafeDouble(odrPlayer("OriginalAmount")) - nPendingAmount
            UpdatePlayer(SafeString(odrPlayer("PLayerID")), SafeString(odrPlayer("Name")), SafeString(odrPlayer("Login")), nBalanceAmount)

            '' Add Player's Transaction
            AddTransaction(odrPlayer, poMonday, True)
        Next

        '' Get All Agents
        Dim oAgentManager As New CAgentManager()
        Dim tblAgents As DataTable = oAgentManager.GetAllAgents()

        For Each drAgent As DataRow In tblAgents.Rows
            '' Add Agent's Transaction
            AddTransaction(drAgent, poMonday, False)
        Next
    End Sub

#Region "Reset Player's Amount"

    Public Function GetAllPlayer() As DataTable
        Dim odtPlayer As DataTable = New DataTable()
        Dim oWhere As New CSQLWhereStringBuilder
        oWhere.AppendANDCondition("p.OriginalAmount >0 ")
        Dim sSQL As String = "SELECT p.PLayerID, p.[Name], p.[Login], p.OriginalAmount, p.AgentID, a.ProfitPercentage, a.GrossPercentage " & vbCrLf & _
            "FROM PLayers p INNER JOIN Agents a ON p.AgentID = a.AgentID " & vbCrLf & _
        oWhere.SQL

        log.Debug("Get All Player Pending . SQL: " & sSQL)

        Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Try
            odtPlayer = odbSQL.getDataTable(sSQL)
        Catch ex As Exception
            log.Error("Cannot get All Player Pending. SQL: " & sSQL, ex)
        Finally
            odbSQL.closeConnection()
        End Try

        Return odtPlayer
    End Function

    Public Function GetPlayerPendingAmount(ByVal psPlayerID As String) As Double
        Dim nPlayerPendingAmount As Double
        Dim oWhere As New CSQLWhereStringBuilder
        oWhere.AppendANDCondition("PlayerID =" & SQLString(psPlayerID))
        oWhere.AppendANDCondition("ISNULL(Ticketstatus,'OPEN') in ('Open', 'Pending')")
        Dim sSQL As String = "Select SUM(RiskAmount) as TotalRiskAmount " & vbCrLf & _
            "FROM Tickets  " & vbCrLf & _
        oWhere.SQL

        log.Debug("Get Sum of RiskAmount by PlayerID. SQL: " & sSQL)

        Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Try
            nPlayerPendingAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
        Catch ex As Exception
            log.Error("Cannot get Player's PendingAmount. SQL: " & sSQL, ex)
        Finally
            odbSQL.closeConnection()
        End Try

        Return nPlayerPendingAmount
    End Function

    Public Function UpdatePlayerOriginalAmount() As Boolean

        Dim bSuccess As Boolean = True

        Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE Players.OriginalAmount <> 0")
        Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

        Try
            With oUpdate
                .AppendString("OriginalAmount", "(SELECT CreditMaxAmount FROM PlayerTemplates WHERE PlayerTemplates.PlayerTemplateID = ISNULL(Players.PlayerTemplateID, Players.DefaultPlayerTemplateID))")
            End With
            log.Debug("Update player's Original Amount. SQL: " & oUpdate.SQL)
            odbSQL.executeNonQuery(oUpdate.SQL)

        Catch ex As Exception
            bSuccess = False
            log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
        Finally
            odbSQL.closeConnection()
        End Try

        Return bSuccess
    End Function

    Public Function UpdatePlayer(ByVal psPlayerID As String, ByVal psName As String, ByVal psLogin As String, ByVal pnBalanceAmount As Double) As Boolean

        Dim bSuccess As Boolean = True

        Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
        Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        Try
            With oUpdate
                .AppendString("Name", SQLString(psName))
                .AppendString("Login", SQLString(psLogin))
                .AppendString("BalanceAmount", SafeString(pnBalanceAmount))
            End With
            log.Debug("Update BalanceAmount in player. SQL: " & oUpdate.SQL)
            odbSQL.executeNonQuery(oUpdate, "")
            
        Catch ex As Exception
            bSuccess = False
            log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
        Finally
            odbSQL.closeConnection()
        End Try

        Return bSuccess
    End Function

#End Region

    Private Sub AddTransaction(ByVal pdrInfo As DataRow, ByVal poMonday As Date, ByVal pbPlayer As Boolean)
        Dim oDB As CSQLDBUtils = Nothing
        Dim sSQL As String = ""
        Try
            oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oTicketManager As New CTicketManager
            Dim nAmountOwed As Double = 0
            Dim nPercentage As Double = 0
            Dim nPaidAmount As Double = 0
            Dim nPLAmount As Double = 0

            '' Get previous Monday 5:30AM by Eastern timezone
            Dim oEasternMon As Date = SafeDate(poMonday.AddDays(-7).ToString("MM/dd/yyyy") & " 06:00:00")

            '' Get P/L amount
            If pbPlayer Then
                nAmountOwed = oTicketManager.GetPLAmount(SafeString(pdrInfo("PlayerID")), poMonday.AddDays(-7), poMonday.AddDays(-1), pbPlayer)
                '' Get CreditBack amount
                Dim otblAmount As DataTable = GetTransactionAmount(SafeString(pdrInfo("PlayerID")), oEasternMon, oDB, True)
                If otblAmount IsNot Nothing Then
                    nAmountOwed += SafeDouble(otblAmount.Rows(0)("AmountOwed"))
                End If

                nPaidAmount = nAmountOwed
                '' Get Percentage
                '' Gross Percentage
                nPercentage = SafeDouble(pdrInfo("GrossPercentage"))
                If nPercentage <> 0 AndAlso nPaidAmount < 0 Then
                    nPaidAmount = nPaidAmount * nPercentage / 100
                End If

                '' Calc PLAmount
                nPLAmount = nPaidAmount
                If SafeDouble(pdrInfo("ProfitPercentage")) > 0 Then
                    nPLAmount = nPLAmount * SafeDouble(pdrInfo("ProfitPercentage")) / 100
                End If
            Else
                Dim otblAmount As DataTable = GetTransactionAmount(SafeString(pdrInfo("AgentID")), oEasternMon, oDB)
                If otblAmount IsNot Nothing Then
                    nAmountOwed = SafeDouble(otblAmount.Rows(0)("AmountOwed"))
                    nPaidAmount = SafeDouble(otblAmount.Rows(0)("PaidAmount"))
                    nPLAmount = SafeDouble(otblAmount.Rows(0)("PLAmount"))
                End If
            End If

            '' Create Insert SQL
            Dim oInsert As New CSQLInsertStringBuilder("Transactions")
            With oInsert
                .AppendString("TransactionID", SQLString(newGUID))
                .AppendString("TransactionDate", SQLDate(GetEasternDate))
                .AppendString("TransactionAmount", SQLDouble(nPaidAmount))
                .AppendString("PLAmount", SQLDouble(nPLAmount))
                If nAmountOwed < 0 Then
                    .AppendString("TransactionType", SQLString("Deposit"))
                Else
                    .AppendString("TransactionType", SQLString("Withdraw"))
                End If
                .AppendString("Description", SQLString("Paid"))
                .AppendString("AmountOwed", SQLDouble(nAmountOwed))

                If pbPlayer Then
                    .AppendString("WithdrawID", SQLString(pdrInfo("PlayerID")))
                    .AppendString("PaymentID", SQLString(pdrInfo("AgentID")))

                Else
                    .AppendString("WithdrawID", SQLString(pdrInfo("AgentID")))

                    If SafeString(pdrInfo("ParentID")) <> "" Then
                        .AppendString("PaymentID", SQLString(pdrInfo("ParentID")))
                    Else
                        .AppendString("PaymentID", SQLString(pdrInfo("SuperAdminID")))
                    End If
                End If

            End With

            sSQL = oInsert.SQL
            log.Debug("Add Transaction. SQL: " & sSQL)
            oDB.executeNonQuery(oInsert, "")
        Catch ex As Exception
            log.Error("Error trying to add Transaction. SQL: " & sSQL, ex)
        Finally
            If oDB IsNot Nothing Then
                oDB.closeConnection()
            End If
        End Try
    End Sub

    Private Function GetTransactionAmount(ByVal psUserID As String, ByVal poSDate As Date, ByVal poDB As CSQLDBUtils, Optional ByVal pbPlayer As Boolean = False) As DataTable
        '' Get SubAgents
        Dim oLstAgentIDs As New List(Of String)
        oLstAgentIDs = (New CAgentManager).GetAllSubAgentIDs(psUserID)

        Dim oWhere As New CSQLWhereStringBuilder()
        If pbPlayer Then
            oWhere.AppendANDCondition("WithdrawID = " & SQLString(psUserID))
        Else
            oWhere.AppendANDCondition(String.Format("WithdrawID IN (SELECT PlayerID FROM Players WHERE AgentID IN('{0}'))", Join(oLstAgentIDs.ToArray(), "','")))
        End If

        oWhere.AppendANDCondition("TransactionDate >= " & SQLDate(poSDate))

        Dim sSQL As String = "SELECT SUM(AmountOwed) AS AmountOwed, SUM(TransactionAmount) AS PaidAmount, SUM(PLAmount) AS PLAmount FROM Transactions " & oWhere.SQL
        log.Debug("Get Agent's amount. SQL: " & sSQL)

        Return poDB.getDataTable(sSQL)
    End Function
End Class
