Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CGameManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CGameManager))
        Public Sub UpdateGameStatus2H(ByVal strGameId As String)
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(strGameId))

            'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
            Dim sSQL As String = "update Games  set GameStatus='Time' ,SecondHalfTime=DATEADD(hour,5, GAMEDATE) ,IsFirstHalfFinished='N' " & oWhere.SQL
            log.Debug("Get the list of New games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of New games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

        End Sub
        Public Sub UpdateDate()
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameDate>=" & SQLString(GetEasternDate()))

            'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
            Dim sSQL As String = "update Games  set GameDate=DATEADD(hour,1,gameDate) " & oWhere.SQL
            log.Debug("Get the list of New games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of New games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

        End Sub
        Public Function GetAgentAvailableNewGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal psBookmaker As String) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            ' oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            oWhere.AppendANDCondition("GameDate>=" & SQLString(poDate))

            'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
            Dim sSQL As String = "select * from Games inner join GameLines on Games.Gameid=GameLines.GameID " & oWhere.SQL & " order by GameDate,Context "
            log.Debug("Get the list of New games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of New games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAvailableGamesMonitor(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "", Optional ByVal psBookmaker As String = "") As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("GameDate > " & SQLString(poDate))

            'If Not pbQuaterOnly Then
            '    oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
            '                              "(Context = '2H' and gamestatus='Time' and datediff( hour,Gamedate,{0}) < 3  ) )")
            '    '"(Context = '2H' and IsFirstHalfFinished = 'N' and datediff( hour,Gamedate,{0}) < 3  ) )")
            'Else
            '    oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
            '    ''TODO: security bet time for quater

            'End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"

            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of available games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function LockGameTeamTotalPoint(ByVal psGameID As String, ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False
            Dim oDB As CSQLDBUtils = Nothing
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("GameID = " & SQLString(psGameID))
                Dim sField As String = ""
                sField = "IsGameTeamTPointLocked"
                Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                oUpdate.AppendString(sField, SQLString(IIf(pbLocked, "Y", "N")))
                log.Debug("Lock/UnLock Game Team Total point. SQL: " & oUpdate.SQL)
                oDB.executeNonQuery(oUpdate, psChangeBy)
                bResult = True
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock Game Team Total point. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function
        Public Function DeleteGame(ByVal psGameID As String, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False
            Dim oDB As CSQLDBUtils = Nothing
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                'oWhere.AppendANDCondition("GameID = " & SQLString(psGameID))

                oDB.executeNonQuery("delete GameLines where GameID = " & SQLString(psGameID) & " delete Games where GameID = " & SQLString(psGameID))
                log.Debug("delete GameLines where GameID = " & SQLString(psGameID) & " delete Games where GameID = " & SQLString(psGameID))
                bResult = True
            Catch ex As Exception
                log.Error("delete game. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function GetAllAvailableGames(ByVal psGameType As String, ByVal poDate As DateTime) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("ISNULL(IsForProp,'N')<>'Y'")
            oWhere.AppendANDCondition("GameDate > " & SQLString(poDate.AddHours(-4)))

            Dim sSQL As String = "SELECT * FROM Games " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"
            log.Debug("Get the list of available games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAvailableGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "", Optional ByVal psBookmaker As String = "") As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")


            If Not pbQuaterOnly Then
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")
                '"(Context = '2H' and IsFirstHalfFinished = 'N' and datediff( hour,Gamedate,{0}) < 3  ) )")
            Else
                oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater

            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"

            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of available games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function LockAllGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal psContext As String, _
                                     ByVal peLockType As CEnums.ELockType, ByVal pbLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            Select Case psContext
                Case "2H"
                    oWhere.AppendANDCondition("GameStatus='Time' and DATEDIFF( hour,SecondHalfTime,{0}) < 3 ")
                Case Else
                    oWhere.AppendANDCondition("GameDate > " & SQLString(poDate))
            End Select
            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")

            Dim sUpdateField As String = ""
            Select Case peLockType
                Case CEnums.ELockType.Game
                    sUpdateField = ""
                Case CEnums.ELockType.MoneyLine
                    sUpdateField = "ML"
                Case CEnums.ELockType.TotalPoint
                    sUpdateField = "TPoint"
            End Select
            Select Case UCase(psContext)
                Case "CURRENT"
                    If peLockType = CEnums.ELockType.Game Then
                        sUpdateField = "GameBet"
                    Else
                        sUpdateField = "Game" & sUpdateField
                    End If
                Case "1H"
                    sUpdateField = "FirstHalf" & sUpdateField
                Case "2H"
                    sUpdateField = "SecondHalf" & sUpdateField
            End Select
            sUpdateField = String.Format("Is{0}Locked", sUpdateField)

            Dim oUpdate As New CSQLUpdateStringBuilder("Games", String.Format(oWhere.SQL, SQLString(Now.ToUniversalTime())))
            oUpdate.AppendString(sUpdateField, SQLString(IIf(pbLocked, "Y", "N")))

            log.Debug("Set lock all games. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psUpdateBy) > 0
            Catch ex As Exception
                log.Error("Fail to lock all games. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return False
        End Function

        Public Function GetAllGamesForSuper(ByVal psGameType As String, ByVal plstBookmakers As List(Of String), Optional ByVal psContext As String = "") As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition(String.Format("BookMaker IN ('{0}')", Join(plstBookmakers.ToArray(), "','")))
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition(String.Format("GameDate between {0} and {1} ", SQLString(std.GetEasternDate().AddDays(-1).ToString("yyyy/MM/dd")), SQLString(std.GetEasternDate().AddDays(1).ToString("yyyy/MM/dd"))))
            Dim sSQL As String = "SELECT * FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
                    " ORDER BY GameDate,Context, HomeRotationNumber"

            log.Debug("Get the list of all games for super admin. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("et the list of all games for super admin. SQ " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function


        Public Function GetAvailableGamesForSuper(ByVal psGameType As String, ByVal poDate As DateTime, Optional ByVal psBookmaker As String = "") As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("datediff( hour,Gamedate,{0}) < 3 ")

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H ,cast(Convert(varchar(10),gamedate,101) as datetime) as dategroup FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"

            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of available games for super player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAvailableProps(ByVal psGameType As String, ByVal poDate As DateTime) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            'oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsForProp,'N') = 'Y'")
            oWhere.AppendANDCondition("isnull(PropStatus,'') = ''")
            oWhere.AppendANDCondition("PropDescription <> 'Series Price'")
            oWhere.AppendANDCondition("gamedate > " & SQLString(poDate.AddHours(-3)))

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {0}, GameDate) as MinuteBefore FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate, PropRotationNumber"

            sSQL = String.Format(sSQL, SQLString(poDate))

            log.Debug("Get the list of available props. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available props. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetGameTypeAvailableProps(ByVal poDate As DateTime, ByVal plGameType As List(Of String)) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            'oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsForProp,'N') = 'Y'")
            oWhere.AppendANDCondition("isnull(PropStatus,'') = ''")
            oWhere.AppendANDCondition("PropDescription <> 'Series Price'")
            oWhere.AppendANDCondition("gamedate > " & SQLString(poDate.AddHours(-3)))
            oWhere.AppendANDCondition("GameType IN ('" & Join(plGameType.ToArray(), "','") & "')")

            Dim sSQL As String = "SELECT  distinct GameType FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL

            sSQL = String.Format(sSQL, SQLString(poDate))

            log.Debug("Get the list of game Type available props. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of game Type available props. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function


        Public Function GetAvailablePropsByGameDate(ByVal psGameType As String, ByVal poStarDate As DateTime, ByVal poEndDate As DateTime) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(IsForProp,'N') = 'Y'")
            oWhere.AppendANDCondition("isnull(PropStatus,'') = ''")
            oWhere.AppendANDCondition("PropDescription <> 'Series Price'")
            oWhere.AppendANDCondition(getSQLDateRange("GameDate", poStarDate.AddHours(-3), poEndDate.AddHours(-3)))
            Dim sSQL As String = "SELECT * FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"

            'sSQL = String.Format(sSQL, SQLString(poDate))

            log.Debug("Get the list of available props. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available props. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetAvailablePropsPending(ByVal psGameType As String, ByVal psSiteType As String) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(IsForProp,'N') = 'Y'")
            oWhere.AppendANDCondition("isnull(PropStatus,'') = ''")
            oWhere.AppendANDCondition("Games.GameID in (select distinct Gameid from ticketbets as tb " & vbCrLf & _
                                        " inner join Tickets as t on tb.TicketID = t.TicketID " & vbCrLf & _
                                        " inner join Players as p on p.PlayerID = t.PlayerID  where SiteType=" & SQLString(psSiteType) & ")")
            Dim sSQL As String = "SELECT * FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate,PropParticipantName"

            'sSQL = String.Format(sSQL, SQLString(poDate))

            log.Debug("Get the list of available props. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available props. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetAvailableGamesQuaters(ByVal psGameType As String, ByVal poDate As DateTime, _
                                                Optional ByVal poContexts As String() = Nothing, Optional ByVal pbRequireOdd As Boolean = False, Optional ByVal psAgentID As String = "", Optional ByVal psBookmaker As String = "") As DataTable

            Dim oNow As DateTime = GetEasternDate()
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))

            '' Load Games isn't locked.
            If Not String.IsNullOrEmpty(psAgentID) Then
                'oWhere.AppendANDCondition("BookMaker=" & SQLString(psAgentID & " MANUAL"))
                oWhere.AppendANDCondition(String.Format("BookMaker in('{0}','{1}')", psAgentID, psBookmaker))
                'Else
                '    oWhere.AppendANDCondition(String.Format("BookMaker in('{0}')", psBookmaker))
            End If
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("GameDate > " & SQLString(poDate))
            oWhere.AppendANDCondition("CONVERT(VARCHAR, Gamedate , 101) = " & SQLString(poDate.ToString("MM/dd/yyyy")))

            If poContexts Is Nothing Then
                poContexts = New String() {"1Q", "2Q", "3Q", "4Q"}
            End If

            For nIndex As Integer = 0 To poContexts.Length - 1
                poContexts(nIndex) = SQLString(poContexts(nIndex))
            Next

            If pbRequireOdd Then
                oWhere.AppendANDCondition("GamelineID is not null ")
            End If

            'oWhere.AppendANDCondition("CONVERT(VARCHAR, Gamedate , 101) = " & SQLString(oNow.ToShortDateString()))
            '' TODO: security bet time for quater

            oWhere.AppendANDCondition("GameStatus <> 'Final'")

            Dim sSQL As String = "SELECT * FROM Games" & vbCrLf & _
                    " LEFT OUTER JOIN  GameLines ON GameLines.GameID = Games.GameID " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"
            log.Debug("Get the list of available Quaters. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of Available Games Quaters . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAvailableGameTypes(ByVal poDate As DateTime) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()

            '' Load Games isn't locked.
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate.AddHours(-3)) & ") or (Context = '2H' and gamedate > " & _
                                            SQLString(poDate.AddHours(-3)) & ") )")
            oWhere.AppendANDCondition("GameStatus <> 'Final'")
            Dim sSQL As String = "SELECT GameType, Bookmaker, Count(GameLineID) as Num FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " GROUP BY GameType, Bookmaker"
            log.Debug("Get the list of available GameTypes. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetGameTypes() As List(Of String)
            Dim oGameTypes As New List(Of String)

            Dim sSQL As String = "SELECT DISTINCT GameType FROM Games ORDER BY GameType"
            log.Debug("Get the list of game types. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim odtGameTypes As DataTable = odbSQL.getDataTable(sSQL)

                For Each odrGameType As DataRow In odtGameTypes.Rows
                    oGameTypes.Add(SafeString(odrGameType("GameType")))
                Next

            Catch ex As Exception
                log.Error("Cannot get the list of game types. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oGameTypes
        End Function

        Public Function GetGames(ByVal psGameType As String, ByVal poDateFrom As Date, ByVal poDateTo As Date, ByVal pbLocked As Boolean, ByVal pbGamePlayed As Boolean, _
                                Optional ByVal pbFinalGames As Boolean = False, Optional ByVal pbRequireFinal As Boolean = True) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("GameType", SQLString(psGameType), "=")
            oWhere.AppendANDCondition("ISNULL(IsLocked,'N') =" & SQLString(IIf(pbLocked, "Y", "N")))
            oWhere.AppendANDCondition(getSQLDateRange("GameDate", SafeString(poDateFrom), SafeString(poDateTo)))
            If pbGamePlayed Then
                oWhere.AppendANDCondition("Gamedate < " & SQLString(GetEasternDate()))
            End If

            oWhere.AppendANDCondition("(isnull(IsForProp,'N') = 'N' or (isnull(IsForProp,'N') = 'Y' and PropDescription = 'Series Price'))")

            If pbRequireFinal And pbFinalGames Then
                oWhere.AppendANDCondition("ISNULL(GameStatus,'') = 'FINAL'")
            ElseIf pbRequireFinal And Not pbFinalGames Then
                oWhere.AppendANDCondition("ISNULL(GameStatus,'') <> 'FINAL'")
            End If

            Dim sSQL As String = "SELECT * FROM  Games " & oWhere.SQL & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"
            log.Debug("Get the list of Locked/UnLocked Games by GameType. SQL: " & sSQL)

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of Locked/UnLocked Games by GameType. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function LockGames(ByVal poGameIDs As List(Of String), ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                If poGameIDs.Count > 0 Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID IN ('" & Join(poGameIDs.ToArray(), "','") & "')")

                    Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                    oUpdate.AppendString("IsLocked", SQLString(IIf(pbLocked, "Y", "N")))
                    log.Debug("Lock/UnLock Multi Games. SQL: " & oUpdate.SQL)

                    oDB.executeNonQuery(oUpdate, psChangeBy)

                    bResult = True
                End If
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock multi games. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function LockGameBets(ByVal poGameIDs As List(Of String), ByVal psContext As String, ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                If poGameIDs.Count > 0 Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID IN ('" & Join(poGameIDs.ToArray(), "','") & "')")
                    Dim sField As String = ""
                    Select Case UCase(psContext)
                        Case "CURRENT"
                            sField = "IsGameBetLocked"
                        Case "1H"
                            sField = "IsFirstHalfLocked"
                        Case "2H"
                            sField = "IsSecondHalfLocked"
                    End Select

                    Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                    oUpdate.AppendString(sField, SQLString(IIf(pbLocked, "Y", "N")))
                    log.Debug("Lock/UnLock Multi Game Bets. SQL: " & oUpdate.SQL)

                    oDB.executeNonQuery(oUpdate, psChangeBy)

                    bResult = True
                End If
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock multi games. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function LockGameBets(ByVal psGameID As String, ByVal psContext As String, ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try

                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("GameID = " & SQLString(psGameID))
                Dim sField As String = ""
                Select Case UCase(psContext)
                    Case "CURRENT"
                        sField = "IsGameBetLocked"
                    Case "1H"
                        sField = "IsFirstHalfLocked"
                    Case "2H"
                        sField = "IsSecondHalfLocked"
                End Select

                Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                oUpdate.AppendString(sField, SQLString(IIf(pbLocked, "Y", "N")))
                log.Debug("Lock/UnLock Multi Game Bets. SQL: " & oUpdate.SQL)

                oDB.executeNonQuery(oUpdate, psChangeBy)
                bResult = True
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock multi games. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function


        Public Function LockGameMoneyLine(ByVal psGameID As String, ByVal psContext As String, ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try

                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("GameID = " & SQLString(psGameID))
                Dim sField As String = ""
                Select Case UCase(psContext)
                    Case "CURRENT"
                        sField = "IsGameMLLocked"
                    Case "1H"
                        sField = "IsFirstHalfMLLocked"
                    Case "2H"
                        sField = "IsSecondHalfMLLocked"
                End Select

                Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                oUpdate.AppendString(sField, SQLString(IIf(pbLocked, "Y", "N")))
                log.Debug("Lock/UnLock Game Money Line. SQL: " & oUpdate.SQL)

                oDB.executeNonQuery(oUpdate, psChangeBy)
                bResult = True
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock Game Money Line. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function LockGameTotalPoint(ByVal psGameID As String, ByVal psContext As String, ByVal pbLocked As Boolean, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try

                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("GameID = " & SQLString(psGameID))
                Dim sField As String = ""
                Select Case UCase(psContext)
                    Case "CURRENT"
                        sField = "IsGameTPointLocked"
                    Case "1H"
                        sField = "IsFirstHalfTPointLocked"
                    Case "2H"
                        sField = "IsSecondHalfTPointLocked"
                End Select

                Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                oUpdate.AppendString(sField, SQLString(IIf(pbLocked, "Y", "N")))
                log.Debug("Lock/UnLock Game Total point. SQL: " & oUpdate.SQL)

                oDB.executeNonQuery(oUpdate, psChangeBy)
                bResult = True
            Catch ex As Exception
                log.Error("Cannot get Lock/UnLock Game Total point. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function UpdateGameStatus(ByVal psGameID As String, ByVal pbGameFinal As Boolean, ByVal psChangeBy As String, Optional ByVal psGameDate As String = "") As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                If psGameID <> "" Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID =" & SQLString(psGameID))
                    oWhere.AppendANDCondition("FinalCheckCompleted is NULL") ' Recalculate Prop 

                    Dim oUpdate As New CSQLUpdateStringBuilder("Games", oWhere.SQL)
                    If pbGameFinal Then
                        oUpdate.AppendString("GameStatus", "'Final'")
                    End If
                    If Not String.IsNullOrEmpty(psGameDate) Then
                        oUpdate.AppendString("GameDate", SQLString(psGameDate))
                    End If
                    log.Debug("Update Game Status. SQL: " & oUpdate.SQL)
                    oDB.executeNonQuery(oUpdate, psChangeBy)
                    bResult = True
                End If
            Catch ex As Exception
                log.Error("Cannot Update Game Status. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function


#Region "Change Score"

        Public Function UpdateGameScore(ByVal psGameID As String, ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer, ByVal psChangeBy As String, ByVal psGameStatus As String, Optional ByVal pbUpdateFirstHalf As Boolean = False) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                If psGameID <> "" Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    Dim oUpdate As CSQLUpdateStringBuilder

                    ''1: Reset Players balance: BalanceAmount = NewBalanceAmount
                    Dim odtNewBalanceAmountPlayers As DataTable = getNewBalanceAmountPlayers(psGameID, oDB)
                    For Each odrBalanceAmount As DataRow In odtNewBalanceAmountPlayers.Rows
                        oUpdate = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID=" & SQLString(odrBalanceAmount!PlayerID))
                        oUpdate.AppendString("BalanceAmount", SQLDouble(odrBalanceAmount!NewBalanceAmount))

                        oDB.executeNonQuery(oUpdate, psChangeBy, True)
                    Next

                    ''2: TicketBets: TicketBetStatus='Open'
                    oUpdate = New CSQLUpdateStringBuilder("TicketBets", "WHERE GameID=" & SQLString(psGameID))
                    oUpdate.AppendString("TicketBetStatus", "'Open'")

                    oDB.executeNonQuery(oUpdate, psChangeBy, True)

                    ''3: Tickets: TicketStatus='Pending', NetAmount=0 [, TicketCompleteDate=null]
                    Dim sWhere As String = "WHERE TicketID IN (SELECT DISTINCT TicketID FROM TicketBets WHERE GameID=" & SQLString(psGameID) & ")"

                    oUpdate = New CSQLUpdateStringBuilder("Tickets", sWhere)
                    oUpdate.AppendString("TicketStatus", "'Pending'")
                    oUpdate.AppendString("NetAmount", "0")

                    oDB.executeNonQuery(oUpdate, psChangeBy, True)

                    ''4: Change score
                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID =" & SQLString(psGameID))

                    oUpdate = New CSQLUpdateStringBuilder("Games", oWhere.SQL)

                    If pbUpdateFirstHalf Then
                        oUpdate.AppendString("HomeFirstHalfScore", SQLString(pnHomeScore))
                        oUpdate.AppendString("AwayFirstHalfScore", SQLString(pnAwayScore))
                        oUpdate.AppendString("IsFirstHalfFinished", SQLString(IIf(psGameStatus = "Final", "Y", "N")))
                    Else
                        oUpdate.AppendString("HomeScore", SQLString(pnHomeScore))
                        oUpdate.AppendString("AwayScore", SQLString(pnAwayScore))
                        oUpdate.AppendString("GameStatus", SQLString(psGameStatus))
                    End If

                    ''5: Games: Reset all check date for recalculate bettings
                    ResetGameCheck(oUpdate, psGameID)
                    log.Debug("Update Game's score. SQL: " & oUpdate.SQL)
                    oDB.executeNonQuery(oUpdate, psChangeBy, True)

                    ''commit all transactions
                    oDB.commitTransaction()

                    bResult = True
                End If
            Catch ex As Exception
                oDB.rollbackTransaction()
                log.Error("Cannot Update Game's score. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function ChangeScoreForGameFinal(ByVal psGameID As String, ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer, ByVal psChangedBy As String, Optional ByVal pbUpdateFirstHalf As Boolean = False) As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As ISQLEditStringBuilder = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                log.DebugFormat("Change score for game final. GameID({0}) - HomeScore({1}) - AwayScore({2})", psGameID, pnHomeScore, pnAwayScore)

                ''1: Reset Players balance: BalanceAmount=NewBalanceAmount
                Dim odtNewBalanceAmountPlayers As DataTable = getNewBalanceAmountPlayers(psGameID, odbSQL)
                For Each odrBalanceAmount As DataRow In odtNewBalanceAmountPlayers.Rows
                    oUpdate = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID=" & SQLString(odrBalanceAmount!PlayerID))
                    oUpdate.AppendString("BalanceAmount", SQLDouble(odrBalanceAmount!NewBalanceAmount))

                    odbSQL.executeNonQuery(oUpdate, psChangedBy, True)
                Next

                ''2: TicketBets: TicketBetStatus='Open'
                oUpdate = New CSQLUpdateStringBuilder("TicketBets", "WHERE GameID=" & SQLString(psGameID))
                oUpdate.AppendString("TicketBetStatus", "'Open'")

                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)

                ''3: Tickets: TicketStatus='Pending', NetAmount=0 [, TicketCompleteDate=null]
                Dim sWhere As String = "WHERE TicketID IN (SELECT DISTINCT TicketID FROM TicketBets WHERE GameID=" & SQLString(psGameID) & ")"

                oUpdate = New CSQLUpdateStringBuilder("Tickets", sWhere)
                oUpdate.AppendString("TicketStatus", "'Pending'")
                oUpdate.AppendString("NetAmount", "0")

                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)

                ''4: Change score
                ''Games: HomeScore=pnHomeScore, AwayScore=pnAwaySocre, FinalCheckCompleted=null
                oUpdate = New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(psGameID))
                If pbUpdateFirstHalf Then
                    oUpdate.AppendString("HomeFirstHalfScore", SafeString(pnHomeScore))
                    oUpdate.AppendString("AwayFirstHalfScore", SafeString(pnAwayScore))
                    oUpdate.AppendString("IsFirstHalfFinished", "'Y'")
                    'oUpdate.AppendString("FirstHalfProcessedDate", "null")
                Else
                    oUpdate.AppendString("HomeScore", SafeString(pnHomeScore))
                    oUpdate.AppendString("AwayScore", SafeString(pnAwayScore))
                End If

                ''5: Games: Reset all check date for recalculate bettings
                ResetGameCheck(CType(oUpdate, CSQLUpdateStringBuilder), psGameID)

                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)

                ''commit all transactions
                odbSQL.commitTransaction()

            Catch ex As Exception
                bSuccess = False
                odbSQL.rollbackTransaction()
                LogError(log, "Cannot change score game", ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function ChangeScoreForGameQuaters(ByVal psGameID As String, ByVal psChangedBy As String, ByVal pnHomeScore1Q As Integer, ByVal pnHomeScore2Q As Integer, ByVal pnHomeScore3Q As Integer, ByVal pnHomeScore4Q As Integer, _
                                    ByVal pnAwayScore1Q As Integer, ByVal pnAwayScore2Q As Integer, ByVal pnAwayScore3Q As Integer, ByVal pnAwayScore4Q As Integer) As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As ISQLEditStringBuilder = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                log.DebugFormat("Change score for game Quaters.")

                ''1: Reset Players balance
                '' BalanceAmount=NewBalanceAmount
                Dim odtNewBalanceAmountPlayers As DataTable = getNewBalanceAmountPlayers(psGameID, odbSQL)
                For Each odrBalanceAmount As DataRow In odtNewBalanceAmountPlayers.Rows
                    oUpdate = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID=" & SQLString(odrBalanceAmount!PlayerID))
                    oUpdate.AppendString("BalanceAmount", SQLDouble(odrBalanceAmount!NewBalanceAmount))

                    odbSQL.executeNonQuery(oUpdate, psChangedBy, True)
                Next

                ''2: TicketBets: TicketBetStatus='Open'
                oUpdate = New CSQLUpdateStringBuilder("TicketBets", "WHERE GameID=" & SQLString(psGameID))
                oUpdate.AppendString("TicketBetStatus", "'Open'")

                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)

                ''3: Tickets: TicketStatus='Pending', NetAmount=0 [, TicketCompleteDate=null]
                Dim sWhere As String = "WHERE TicketID IN (SELECT DISTINCT TicketID FROM TicketBets WHERE GameID=" & SQLString(psGameID) & ")"

                oUpdate = New CSQLUpdateStringBuilder("Tickets", sWhere)
                oUpdate.AppendString("TicketStatus", "'Pending'")
                oUpdate.AppendString("NetAmount", "0")
                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)


                ''4: Change score : HomeScore=pnHomeScore, AwayScore=pnAwaySocre, FinalCheckCompleted=null
                oUpdate = New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(psGameID))

                oUpdate.AppendString("HomeFirstQScore", SafeString(pnHomeScore1Q))
                oUpdate.AppendString("HomeSecondQScore", SafeString(pnHomeScore2Q))
                oUpdate.AppendString("HomeThirdQScore", SafeString(pnHomeScore3Q))
                oUpdate.AppendString("HomeFourQScore", SafeString(pnHomeScore4Q))

                oUpdate.AppendString("AwayFirstQScore", SafeString(pnAwayScore1Q))
                oUpdate.AppendString("AwaySecondQScore", SafeString(pnAwayScore2Q))
                oUpdate.AppendString("AwayThirdQScore", SafeString(pnAwayScore3Q))
                oUpdate.AppendString("AwayFourQScore", SafeString(pnAwayScore4Q))

                ''5: Games: Reset all check date for recalculate bettings
                ResetGameCheck(CType(oUpdate, CSQLUpdateStringBuilder), psGameID)

                odbSQL.executeNonQuery(oUpdate, psChangedBy, True)

                ''commit all transactions
                odbSQL.commitTransaction()

            Catch ex As Exception
                bSuccess = False
                odbSQL.rollbackTransaction()
                LogError(log, "Cannot change score game", ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Private Sub ResetGameCheck(ByVal poUpdate As CSQLUpdateStringBuilder, ByVal psGameID As String)
            'Dim oUpdate As New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(psGameID))         
            poUpdate.AppendString("FirstHalfProcessedDate", "null")
            poUpdate.AppendString("FirstQuaterProcessedDate", "null")
            poUpdate.AppendString("SecondQuaterProcessedDate", "null")
            poUpdate.AppendString("ThirdQuaterProcessedDate", "null")
            poUpdate.AppendString("FinalCheckCompleted", "null")
            poUpdate.AppendString("GameSuspendProcessedDate", "null")
            'poDB.executeNonQuery(oUpdate, psChangedBy, True)
        End Sub

        Private Function getNewBalanceAmountPlayers(ByVal psGameID As String, ByVal poSQL As CSQLDBUtils) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'') NOT IN ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.TicketID IN (SELECT DISTINCT TicketID FROM TicketBets WHERE GameID=" & SQLString(psGameID) & ")")

            Dim sSQL As String = "select p.PlayerID, (ISNULL(p.BalanceAmount,0) - ISNULL(SUM(t.NetAmount),0)) as NewBalanceAmount " & vbCrLf & _
                    "FROM Players p " & vbCrLf & _
                    "INNER JOIN Tickets t on t.PlayerID=p.PlayerID " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " GROUP BY p.PlayerID, p.BalanceAmount"

            Return poSQL.getDataTable(sSQL)
        End Function

#End Region


        Public Function GetAgentAvailableGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal pslstContext As List(Of String) = Nothing, Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psAgentID), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))

            '' Load Games isn't locked.
            If pbCheckLock Then
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            End If
            If pGameLineOff Then
                oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
            End If
            If Not pbQuaterOnly Then
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")
            Else
                If pslstContext IsNot Nothing Then
                    oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
                End If
                'oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            ' oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            ' oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
            If pslstContext IsNot Nothing Then
                oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
            End If
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
            Dim sSQL As String = "SELECT LiveOpen,Description, *,ags.AgentGameSettingID,ags.AgentID,ISNULL((CASE ISNULL(ags.IsGameCircle, 'N') WHEN            'Y' THEN ags.IsGameCircle ELSE vw_GetGameLines.IsGameCircle END), 'N')  AS IsCircle," & _
            "DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H,cast(Convert(varchar(10),gamedate,101) as datetime) as dategroup" & _
            " FROM vw_GetGameLines " & vbCrLf & _
            " LEFT OUTER JOIN AgentGameSettings as ags" & vbCrLf & _
            "ON (vw_GetGameLines.GameID=ags.GameID and ags.AgentID=" & SQLString(psAgentID) & ")" & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
             "UNION  " & GetSQLAgentAvailableGames(psGameType, poDate, pbCheckLock, pGameLineOff, pbQuaterOnly, pslstContext, psBookmaker, psAgentID, pGameLineOff) & vbCrLf & _
             " and vw_GetGameLines.GameID NOT IN " & GetSQLGamesIDAgentSetting(psGameType, poDate, pbCheckLock, pGameLineOff, pbQuaterOnly, pslstContext, psAgentID) & vbCrLf & _
              SafeString(IIf(psGameType.Equals("NCAA Football", StringComparison.CurrentCultureIgnoreCase), " ORDER BY AwayRotationNumber", " ORDER BY GameDate,AwayRotationNumber"))


            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of available games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetSQLAgentAvailableGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal pslstContext As List(Of String) = Nothing, Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As String
            Dim oWhere As New CSQLWhereStringBuilder()
            If bGetQuarTer Then
                oWhere.AppendANDCondition(String.Format("BookMaker in('{0}','{1}')", psAgentID & " MANUAL", psBookmaker))
            Else
                oWhere.AppendANDCondition(String.Format("BookMaker in('{0}')", psBookmaker))
            End If

            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            '' Load Games isn't locked.
            If pbCheckLock Then
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            End If
            If pGameLineOff Then
                oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
            End If
            If Not pbQuaterOnly Then
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")
            Else
                If pslstContext IsNot Nothing Then
                    oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
                End If
                'oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            'oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            'oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
            If pslstContext IsNot Nothing Then
                oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
            End If
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
            Dim sSQL As String = "SELECT  LiveOpen,Description,*,ags.AgentGameSettingID,ags.AgentID,ISNULL((CASE ISNULL(ags.IsGameCircle, 'N') WHEN            'Y' THEN ags.IsGameCircle ELSE vw_GetGameLines.IsGameCircle END), 'N')  AS IsCircle," & _
            "DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H,cast(Convert(varchar(10),gamedate,101) as datetime) as dategroup" & _
            " FROM vw_GetGameLines " & vbCrLf & _
            "LEFT OUTER JOIN AgentGameSettings as ags ON (vw_GetGameLines.GameID=ags.GameID and ags.AgentID=" & SQLString(psAgentID) & ")" & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) "
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            Return sSQL

        End Function

        Public Function GetSQLGamesIDAgentSetting(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal pslstContext As List(Of String) = Nothing, Optional ByVal psBookmaker As String = "") As String
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))

            If Not pbQuaterOnly Then
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")
            Else
                If pslstContext IsNot Nothing Then
                    oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
                End If
                If pslstContext IsNot Nothing Then
                    oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
                End If
                ' oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            'oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            If pslstContext IsNot Nothing Then
                oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
            End If

            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            Dim sSQL As String = "(SELECT vw_GetGameLines.GameID FROM vw_GetGameLines " & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) )"
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            Return sSQL
        End Function

#Region "get Exists Game"

        Public Function GetExistsAvailableGames(ByVal pdicBookMaker As Dictionary(Of String, String), ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            ' oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psAgentID), "=")
            ' oWhere.AppendANDCondition(String.Format("GameType in ('{0}')", Join(plstGameType.ToArray(), "','")))
            Dim sCondition As String = ""
            For Each sGameType As String In pdicBookMaker.Keys
                If String.IsNullOrEmpty(sCondition) Then
                    sCondition += "(" + String.Format(" (GameType='{0}' AND  BookMaker='{1}')", sGameType, psAgentID)
                Else
                    sCondition += String.Format("OR (GameType='{0}' AND BookMaker='{1}')", sGameType, psAgentID)
                End If

            Next
            sCondition += ")"
            oWhere.AppendANDCondition(sCondition)
            '' Load Games isn't locked.
            If pbCheckLock Then
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            End If
            If pGameLineOff Then
                oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
            End If
            If Not pbQuaterOnly Then
                ' old code
                'oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                '                          " and ((NOT ';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                '                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                '                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")

                ' original code
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                                                          " and ((NOT ';SOCCER;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;SUPER LIGA;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                                                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;SUPER LIGA;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")

            Else

                ' oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            Dim sSQL As String = "SELECT  GameType, SUM(FullGame) AS FullGame, SUM(FirstHalf) AS FirstHalf, SUM(SecondHalf)  AS SecondHalf,SUM(Quarter) as Quarter From(SELECT   GameType, isnull((CASE Context WHEN 'Current' THEN 1 WHEN '1H' THEN 0 WHEN '2H'THEN 0 ELSE 0  END),0) AS FullGame, isnull((CASE Context  WHEN 'Current' THEN 0 WHEN '1H' THEN 1 WHEN '2H'THEN 0 ELSE 0 " & vbCrLf & _
            "END), 0) AS FirstHalf, isnull((CASE Context WHEN 'Current' THEN 0 WHEN '1H' THEN 0 WHEN '2H'THEN 1 ELSE 0  END),0) AS SecondHalf , isnull((CASE Context WHEN 'Current' THEN 0 WHEN '1H' THEN 0 WHEN '2H'THEN 0 WHEN '1Q'THEN 1 WHEN '2Q'THEN 1 WHEN '3Q'THEN 1 WHEN '4Q'THEN 1 ELSE 0  END),0) AS Quarter FROM vw_GetGameLines " & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
             "UNION " & GetSQLExistsAvailableGames(pdicBookMaker, poDate, pbCheckLock, pGameLineOff, pbQuaterOnly, psContext, psAgentID, pGameLineOff) & vbCrLf & _
             " and vw_GetGameLines.GameLineID NOT IN " & GetSQLGamesIDExistsSetting(sCondition, psAgentID, poDate, pbCheckLock, pGameLineOff, pbQuaterOnly, psContext) & vbCrLf & _
              " ) GameExists group by GameType order by GameType"

            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of Exist games. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetSQLExistsAvailableGames(ByVal pdicBookMaker As Dictionary(Of String, String), ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As String
            Dim oWhere As New CSQLWhereStringBuilder()
            Dim sCondition As String = ""
            For Each sGameType As String In pdicBookMaker.Keys
                If String.IsNullOrEmpty(sCondition) Then
                    sCondition += "(" + String.Format(" (GameType='{0}' AND BookMaker in('{1}','{2}'))", sGameType, pdicBookMaker(sGameType), psAgentID & " MANUAL")
                Else
                    sCondition += String.Format(" OR (GameType='{0}' AND BookMaker in('{1}','{2}'))", sGameType, pdicBookMaker(sGameType), psAgentID & " MANUAL")
                End If
            Next
            oWhere.AppendANDCondition(sCondition + ")")
            ' oWhere.AppendANDCondition(String.Format("BookMaker in('{0}','{1}')", psAgentID & " MANUAL", Join(plstBookMaker.ToArray(), "','")))
            ' oWhere.AppendANDCondition(String.Format("GameType in ({0})", Join(plstGameType.ToArray(), "','")))
            '' Load Games isn't locked.
            If pbCheckLock Then
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            End If
            If pGameLineOff Then
                oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
            End If
            If Not pbQuaterOnly Then
                ' old code
                'oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                '                          " and ((NOT ';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                '                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                '                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")

                ' original code
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                                                          " and ((NOT 'CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                                                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;SUPER LIGA;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")
            Else
                oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            If pbCheckLock Then
                oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
                                      "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
                                      "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
                                      "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
                                      "ELSE 'N' " & vbLf & _
                                      "END),'N') <> 'Y'")

            End If

            Dim sSQL As String = "SELECT GameType, isnull((CASE Context WHEN 'Current' THEN 1 WHEN '1H' THEN 0 WHEN '2H'THEN 0 ELSE 0  END),0) AS FullGame, isnull((CASE Context  WHEN 'Current' THEN 0 WHEN '1H' THEN 1 WHEN '2H'THEN 0 ELSE 0 " & vbCrLf & _
            " END) ,0) AS FirstHalf, isnull((CASE Context WHEN 'Current' THEN 0 WHEN '1H' THEN 0 WHEN '2H'THEN 1 ELSE 0  END),0) AS SecondHalf,isnull((CASE Context WHEN 'Current' THEN 0 WHEN '1H' THEN 0 WHEN '2H'THEN 0 WHEN '1Q'THEN 1 WHEN '2Q'THEN 1 WHEN '3Q'THEN 1 WHEN '4Q'THEN 1 ELSE 0  END),0) AS Quarter  FROM vw_GetGameLines " & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) "
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))
            Return sSQL


        End Function

        Public Function GetSQLGamesIDExistsSetting(ByVal psCondition As String, ByVal pAgentID As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "") As String
            Dim oWhere As New CSQLWhereStringBuilder()
           
            'oWhere.AppendOptionalANDCondition("( BookMaker", SQLString(pAgentID), "=")
            ' oWhere.AppendANDCondition(String.Format("GameType in ('{0}'))", Join(lstGameType.ToArray(), "','")))
            'Dim sCondition As String = ""

            oWhere.AppendANDCondition(psCondition)
            If Not pbQuaterOnly Then
                ' old code
                'oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                '                                          " and ((NOT ';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                '                                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;EURO CUPS;SUPER LIGA;EURO;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                '                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")

                ' original code
                oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLDate(poDate) & _
                                                          " and ((NOT ';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN;SUPER LIGA;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%') OR GameDate < " & SQLDate(poDate.AddDays(1)) & _
                                                          ") and (';SOCCER;PREMIER;LA LIGA;SERIE A;BUNDESLIGA;LIGUE 1;NETHERLANDS;SCOTLAND;PORTUGAL;MLS;ARGENTINA;BRAZIL;MEXICAN; LIGA;CHAMPIONS LEAGUE;EUROPA LEAGUE;COPA AMERICA;CARLING CUP;FA CUP;CONCACAF;' LIKE '%' + GameType + '%' or GameDate > " & SQLDate(poDate) & ") ) or " & _
                                                          "(Context = '2H' and gamestatus='Time' and datediff( hour,SecondHalfTime,{2}) < 3  ) )")

            Else
                oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
                ''TODO: security bet time for quater
            End If

            oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
            oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

            '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
            'If pbCheckLock Then
            '    oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
            '                          "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
            '                          "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
            '                          "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
            '                          "ELSE 'N' " & vbLf & _
            '                          "END),'N') <> 'Y'")

            'End If

            Dim sSQL As String = "(SELECT vw_GetGameLines.GameLineID FROM vw_GetGameLines " & vbCrLf & _
             oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) )"
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            Return sSQL
        End Function

#End Region




        Public Function GetAgentAvailableGamesForSuper(ByVal psGameType As String, ByVal poDate As DateTime, Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "") As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psAgentID), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("datediff( hour,Gamedate,{0}) < 3 ")

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
                    "UNION " & GetSQLAgentSuperPlayerAvailableGames(psGameType, poDate, psBookmaker) & vbCrLf & _
                     " and GameLines.GameID not in " & GetSQLGamesIDAgentSuperPlayerSetting(psGameType, poDate, psAgentID) & vbCrLf & _
                    " ORDER BY GameDate, HomeRotationNumber"

            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

            log.Debug("Get the list of available games for super player. SQL:----- " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetSQLAgentSuperPlayerAvailableGames(ByVal psGameType As String, ByVal poDate As DateTime, Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "") As String
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("datediff( hour,Gamedate,{0}) < 3 ")

            Dim sSQL As String = "SELECT *,DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) "
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))
            log.Debug("Get the list of sql available games for super player. SQL: " & sSQL)
            Return sSQL
        End Function

        Public Function GetSQLGamesIDAgentSuperPlayerSetting(ByVal psGameType As String, ByVal poDate As DateTime, Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "") As String
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psBookmaker), "=")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            ' oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
            oWhere.AppendANDCondition("datediff( hour,Gamedate,{0}) < 3 ")

            Dim sSQL As String = "(SELECT GameLines.GameID  FROM GameLines " & vbCrLf & _
                    "INNER JOIN Games ON GameLines.GameID = Games.GameID" & vbCrLf & _
                    oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) )"
            sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))
            log.Debug("Get the list of sql available games for super player. SQL: " & sSQL)
            Return sSQL
        End Function








        'Public Function GetAgentAvailableQuarterGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, ByVal pslstContext As List(Of String), Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As DataTable
        '    Dim oWhere As New CSQLWhereStringBuilder()
        '    oWhere.AppendOptionalANDCondition("BookMaker", SQLString(psAgentID), "=")
        '    oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))

        '    '' Load Games isn't locked.
        '    If pbCheckLock Then
        '        oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
        '    End If
        '    If pGameLineOff Then
        '        oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
        '    End If
        '    oWhere.AppendANDCondition("Context IN ('" & Join(pslstContext.ToArray(), "','") & "')")
        '    oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
        '    oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
        '    oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

        '    '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
        '    If pbCheckLock Then
        '        oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
        '                              "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
        '                              "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
        '                              "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
        '                              "ELSE 'N' " & vbLf & _
        '                              "END),'N') <> 'Y'")

        '    End If

        '    'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
        '    Dim sSQL As String = "SELECT  *,ags.AgentGameSettingID,ags.AgentID,ISNULL((CASE ISNULL(ags.IsGameCircle, 'N') WHEN            'Y' THEN ags.IsGameCircle ELSE vw_GetGameLines.IsGameCircle END), 'N')  AS IsCircle," & _
        '    "DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H" & _
        '    " FROM vw_GetGameLines " & vbCrLf & _
        '    " LEFT OUTER JOIN AgentGameSettings as ags" & vbCrLf & _
        '    "ON (vw_GetGameLines.GameID=ags.GameID and ags.AgentID=" & SQLString(psAgentID) & ")" & vbCrLf & _
        '     oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) " & vbCrLf & _
        '     "UNION  " & GetSQLAgentAvailableQuarterGames(psGameType, poDate, pbCheckLock, pGameLineOff, pslstContext, psBookmaker, psAgentID) & vbCrLf & _
        '     " and vw_GetGameLines.GameID not in " & GetSQLGamesIDAgentSetting(psGameType, poDate, pbCheckLock, pGameLineOff, pslstContext, psAgentID) & vbCrLf & _
        '      " ORDER BY GameDate, HomeRotationNumber"


        '    sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

        '    log.Debug("Get the list of available games. SQL: " & sSQL)

        '    Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

        '    Try
        '        Return odbSQL.getDataTable(sSQL)
        '    Catch ex As Exception
        '        log.Error("Cannot get the list of available games. SQL: " & sSQL, ex)
        '    Finally
        '        odbSQL.closeConnection()
        '    End Try

        '    Return Nothing
        'End Function

        'Public Function GetSQLAgentAvailableQuarterGames(ByVal psGameType As String, ByVal poDate As DateTime, ByVal pbCheckLock As Boolean, ByVal pGameLineOff As Boolean, Optional ByVal pbQuaterOnly As Boolean = False, Optional ByVal psContext As String = "", Optional ByVal psBookmaker As String = "", Optional ByVal psAgentID As String = "", Optional ByVal bGetQuarTer As Boolean = False) As String
        '    Dim oWhere As New CSQLWhereStringBuilder()
        '    If bGetQuarTer Then
        '        oWhere.AppendANDCondition(String.Format("BookMaker in('{0}','{1}')", psAgentID & " MANUAL", psBookmaker))
        '    Else
        '        oWhere.AppendANDCondition(String.Format("BookMaker in('{0}')", psBookmaker))
        '    End If

        '    oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
        '    '' Load Games isn't locked.
        '    If pbCheckLock Then
        '        oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
        '    End If
        '    If pGameLineOff Then
        '        oWhere.AppendANDCondition("ISNULL(GameLineOff,'') <> 'Y'")
        '    End If
        '    If Not pbQuaterOnly Then
        '        oWhere.AppendANDCondition("( (Context <> '2H' and GameDate > " & SQLString(poDate) & " ) or " & _
        '                                  "(Context = '2H' and gamestatus='Time' and datediff( hour,Gamedate,{0}) < 3  ) )")
        '    Else
        '        oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q') ")
        '        ''TODO: security bet time for quater
        '    End If

        '    oWhere.AppendANDCondition("(IsForProp= 'N' or (IsForProp='Y' and  PropDescription = 'Series Price') )")
        '    oWhere.AppendOptionalANDCondition("Context", SQLString(psContext), "=")
        '    oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
        '    oWhere.AppendANDCondition("isnull(IsLockedByPreset,'N') <>  'Y'")

        '    '' display line depend on context and (IsGameBetLocked,IsFirstHalfLocked, IsSecondHalfLocked)
        '    If pbCheckLock Then
        '        oWhere.AppendANDCondition("isnull((CASE Context " & vbLf & _
        '                              "WHEN 'Current' THEN IsGameBetLocked" & vbLf & _
        '                              "WHEN '1H' THEN IsFirstHalfLocked" & vbLf & _
        '                              "WHEN '2H'THEN IsSecondHalfLocked" & vbLf & _
        '                              "ELSE 'N' " & vbLf & _
        '                              "END),'N') <> 'Y'")

        '    End If

        '    'LEFT OUTER JOIN AGENTGAMESETTINGS TABLE
        '    Dim sSQL As String = "SELECT  *,ags.AgentGameSettingID,ags.AgentID,ISNULL((CASE ISNULL(ags.IsGameCircle, 'N') WHEN            'Y' THEN ags.IsGameCircle ELSE vw_GetGameLines.IsGameCircle END), 'N')  AS IsCircle," & _
        '    "DATEDIFF(minute, {1}, GameDate) as MinuteBefore, DATEDIFF( minute,SecondHalfTime,{2}) as MinuteBefore2H" & _
        '    " FROM vw_GetGameLines " & vbCrLf & _
        '    "LEFT OUTER JOIN AgentGameSettings as ags ON (vw_GetGameLines.GameID=ags.GameID and ags.AgentID=" & SQLString(psAgentID) & ")" & vbCrLf & _
        '     oWhere.SQL & "  and ( isnull(AwaySpreadMoney,0) <>0 or isnull(TotalPointsOverMoney,0) <>0 or isnull(AwayMoneyLine,0) <> 0 ) "
        '    sSQL = String.Format(sSQL, SQLString(poDate), SQLString(poDate), SQLString(Now.ToUniversalTime()))

        '    Return sSQL

        'End Function






    End Class

End Namespace

