Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Imports SBCService.CServiceStd
Imports SBCEngine
Imports SBCEngine.CEngineStd


Public Class CGameResultManager

    '   Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Fields"

    Private SqlDB As CSQLDBUtils = Nothing
    Private RoundingOption As String = "UP"
    Private CATEGORY_CURRENT As String = "_PARLAY_PAYOUT_CURRENT"
    Private CATEGORY_TIGERSB As String = "_PARLAY_PAYOUT_TIGERSB"
    Private ParlayPayouts As List(Of CCheckParlayPayout)

#End Region

#Region "Constructors"

    Public Sub New(ByVal poSQL As CSQLDBUtils)
        Me.SqlDB = poSQL
    End Sub

#End Region

#Region "Calculate Final Result"

    Public Sub CalculateFinalResult()
        Me.RoundingOption = getRoundingOption()
        Me.ParlayPayouts = getParlayPayouts()

        Dim sSQL As String = ""
        Dim odtGames As DataTable = Nothing

        ''Calculate result
        Dim sConditionAND_GameTypes4Quater As String = getConditionAND_GameTypes4Quater()

        ''1: Firt Quater(Basketball, Football)
        sSQL = "SELECT GameID FROM Games WHERE ISNULL(IsForProp,'N')<>'Y' AND FirstQuaterProcessedDate IS NULL" & _
                   " AND (GameStatus IN ('Final') OR CurrentQuater>1)" & sConditionAND_GameTypes4Quater

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute calculate first quater result" & _
        '  ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.FirstQuater)
        Next

        ''2: Second Quater(Basketball, Football)
        sSQL = "SELECT GameID FROM Games WHERE ISNULL(IsForProp,'N')<>'Y' AND SecondQuaterProcessedDate IS NULL" & _
                   " AND (GameStatus='Final' OR CurrentQuater>2 OR (GameStatus='Time' AND CurrentQuater=2))" & sConditionAND_GameTypes4Quater

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute calculate second quater result" & _
        '        ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.SecondQuater)
        Next

        ''3: First Half
        sSQL = "SELECT GameID FROM Games WHERE ISNULL(IsForProp,'N')<>'Y' AND FirstHalfProcessedDate IS NULL" & _
                    " AND (GameStatus IN ('Time', 'Final') OR ISNULL(IsFirstHalfFinished,'N')='Y')"

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute calculate first half result" & _
        '        ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.FirstHalf)
        Next

        ''4: Third Quater(Basketball, Football)
        sSQL = "SELECT GameID FROM Games WHERE ISNULL(IsForProp,'N')<>'Y' AND ThirdQuaterProcessedDate IS NULL" & _
                   " AND (GameStatus='Final' OR CurrentQuater>3 OR (GameStatus='Time' AND CurrentQuater=3))" & sConditionAND_GameTypes4Quater

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute calculate third quater result" & _
        '      ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.ThirdQuater)
        Next

        ''5: Second Half, Four Quater(Basketball, Football), Final
        sSQL = "SELECT GameID FROM Games WHERE ( ISNULL(IsForProp,'N')<>'Y' or (ISNULL(IsForProp,'N')='Y' and PropDescription='Series Prices')  ) AND GameStatus='Final' AND FinalCheckCompleted IS NULL"

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute calculate second half, four quater, final" & _
        '         ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.Final)
        Next

        ''Process Games Proposition, NOTE: Prop games include series and prop, a series game is treated as a regular game
        sSQL = "SELECT GameID FROM Games WHERE ISNULL(IsForProp,'N')='Y' and ISNULL(PropDescription,'') <> 'Series Prices'  AND GameStatus='Final' AND FinalCheckCompleted IS NULL"

        odtGames = Me.SqlDB.getDataTable(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute game proposition." & _
        '     ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.Proposition)
        Next

        ''Process Games Suspend
        odtGames = getGamesSuspend(sSQL)
        'LogDebug(log, "There are " & FormatNumber(odtGames.Rows.Count, 0) & " game(s) to execute game suspend." & _
        '      ".      SQL: " & sSQL)

        For Each odrGame As DataRow In odtGames.Rows
            setOpenBetResult(SafeString(odrGame("GameID")), CheckGameMode.Suspend)
        Next
    End Sub

    Private Function getGamesSuspend(ByRef sSQL As String) As DataTable
        Dim nProcessMinutes As Integer = CEngineStd.MINUTES_GAME_SUSPEND_PROCESSED
        If nProcessMinutes <= 0 Then nProcessMinutes = 5 * 60 + 1

        Dim oWhere As New CSQLWhereStringBuilder()
        oWhere.AppendANDCondition("ISNULL(IsForProp,'N')<>'Y'")
        oWhere.AppendANDCondition("GameStatus in ('Cancelled', 'Poned', 'Susp')")
        oWhere.AppendANDCondition("GameSuspendProcessedDate IS NULL")
        oWhere.AppendANDCondition("DATEDIFF(minute, GameDate, " & SQLDate(CEngineStd.GetCurrentEasternDate()) & ") >= " & nProcessMinutes.ToString())

        sSQL = "SELECT DISTINCT GameID FROM Games " & oWhere.SQL.Trim()

        Return Me.SqlDB.getDataTable(sSQL)
    End Function

    Private Function getConditionAND_GameTypes4Quater() As String
        Dim oGameTypes As New List(Of String)
        oGameTypes.AddRange(CEngineStd.FOOTBALL_GAMES.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries))
        oGameTypes.AddRange(CEngineStd.BASKETBALL_GAMES.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries))

        If oGameTypes.Count > 0 Then
            Return " AND GameType IN (" & Join((From sGameType As String In oGameTypes _
                                                Where sGameType <> "" _
                                                Select SQLString(sGameType)).Distinct.ToArray(), ", ") & ")"
        End If

        Return ""
    End Function

    Private Function getRoundingOption() As String
        Dim sRounding As String = "UP"

        Dim oWhere As New CSQLWhereStringBuilder
        oWhere.AppendANDCondition("Category='Rounding Option'")
        oWhere.AppendANDCondition("[Key]='Rounding'")

        Dim sSQL As String = "SELECT TOP 1 [Value] FROM SysSettings " & oWhere.SQL

        Try
            sRounding = UCase(Me.SqlDB.getScalerValue(sSQL))

        Catch ex As Exception
            '  log.Error("Cannot get rounding option. SQL: " & sSQL, ex)
        End Try

        If Not (sRounding = "NEAREST" OrElse sRounding = "DOWN" OrElse sRounding = "NONE") Then
            sRounding = "UP"
        End If

        Return sRounding
    End Function

#End Region

#Region "Set Open Bet Result"

    Private Sub setOpenBetResult(ByVal psGameID As String, Optional ByVal poCheckGameMode As CheckGameMode = CheckGameMode.Final)
        '  LogDebug(log, "**** ------------------------- ****")
        '    LogDebug(log, "START CHECK OPEN BET RESULT: " & DateTime.Now.ToUniversalTime)

        ''update game with field date
        If poCheckGameMode = CheckGameMode.Final OrElse poCheckGameMode = CheckGameMode.Proposition Then
            updateGameForFieldDate(psGameID, "FinalCheckStartedProcessing")
        End If

        ''get the list of games, tickets, ticketbets
        Dim odtTicketBets As DataTable = getTicketBets(psGameID, poCheckGameMode)

        If odtTicketBets IsNot Nothing AndAlso odtTicketBets.Rows.Count > 0 Then
            Dim oCheckEngine As New CCheckGameEngine()

            ''set: rounding option
            oCheckEngine.RoundingOption = Me.RoundingOption
            '   log.Debug("---- Rounding option for calculate: " & oCheckEngine.RoundingOption)

            ''set: parlay payouts
            If Me.ParlayPayouts IsNot Nothing Then
                oCheckEngine.ParlayPayouts = Me.ParlayPayouts
            End If

            ''set value: game, ticket, ticketbet
            oCheckEngine.SetValues(odtTicketBets)

            ''set value: teaser odds
            If poCheckGameMode <> CheckGameMode.Proposition Then
                Dim odrBetTeasers As DataRow() = odtTicketBets.Select("TeaserRuleID IS NOT NULL")

                If odrBetTeasers.Length > 0 Then
                    Dim odtTeaserOdds As DataTable = getTeaserOdds((From oRow In odrBetTeasers _
                                                                    Select SQLString(oRow("TeaserRuleID"))).Distinct.ToList)
                    If odtTeaserOdds IsNot Nothing Then
                        oCheckEngine.SetTeaserOddValues(odtTeaserOdds)
                    End If
                End If
            End If

            ''exec check game
            oCheckEngine.ExecuteCheck()

            ''update: ticketstatus, ticketbetstatus, netamount, ticketcompleteddate
            updateResultTickets(oCheckEngine.Tickets)

            ''update: balance amount player
            adjustBalanceAmountPlayers(oCheckEngine.AmountPlayers)
        End If

        ''update game with field date
        Select Case poCheckGameMode
            Case CheckGameMode.FirstHalf
                updateGameForFieldDate(psGameID, "FirstHalfProcessedDate")

            Case CheckGameMode.FirstQuater
                updateGameForFieldDate(psGameID, "FirstQuaterProcessedDate")

            Case CheckGameMode.SecondQuater
                updateGameForFieldDate(psGameID, "SecondQuaterProcessedDate")

            Case CheckGameMode.ThirdQuater
                updateGameForFieldDate(psGameID, "ThirdQuaterProcessedDate")

            Case CheckGameMode.Final, CheckGameMode.Proposition
                updateGameForFieldDate(psGameID, "FinalCheckCompleted")

            Case CheckGameMode.Suspend
                updateGameForFieldDate(psGameID, "GameSuspendProcessedDate")
        End Select

        '  LogDebug(log, "END CHECK OPEN BET RESULT: " & DateTime.Now.ToUniversalTime)
        '  LogDebug(log, "            ")
    End Sub

    Private Function getTicketBets(ByVal psGameID As String, Optional ByVal poCheckGameMode As CheckGameMode = CheckGameMode.Final) As DataTable
        Dim oWhere As New CSQLWhereStringBuilder()
        oWhere.AppendANDCondition("t.PlayerID IS NOT NULL" & vbCrLf)
        oWhere.AppendANDCondition("ISNULL(t.IsForProp,'N')=" & SQLString(IIf(poCheckGameMode = CheckGameMode.Proposition, "Y", "N")) & vbCrLf)
        oWhere.AppendANDCondition( _
            "( ISNULL(t.TicketStatus,'OPEN') IN ('OPEN','PENDING')" & vbCrLf & _
            "       OR ( t.TicketStatus='LOSE' AND ISNULL(t.IsLoseStillCheckBets,'N')='Y'" & vbCrLf & _
            "            AND" & vbCrLf & _
            "            (SELECT COUNT(tb1.TicketBetID) FROM TicketBets tb1 WHERE tb1.TicketID=t.TicketID AND tb1.TicketBetStatus='OPEN')>0" & vbCrLf & _
            "          )" & vbCrLf & _
            "       OR t.TicketType LIKE 'If %'" & vbCrLf & _
            "     )" & vbCrLf)

        oWhere.AppendANDCondition("tb.TicketID IN (SELECT DISTINCT TicketID FROM TicketBets WHERE GameID=" & SQLString(psGameID) & _
                                                        getConditionAND_TicketBetContext(poCheckGameMode) & vbCrLf & _
            "                                       UNION ALL SELECT DISTINCT tmpt.RelatedTicketID FROM TicketBets tmptb " & vbCrLf & _
            "                                       INNER JOIN Tickets tmpt ON tmptb.TicketID = tmpt.TicketID WHERE tmptb.GameID=" & SQLString(psGameID) & vbCrLf & _
                                                    getConditionAND_TicketBetContext(poCheckGameMode) & vbCrLf & _
            "                                       UNION ALL SELECT DISTINCT tmpt2.TicketID FROM Tickets tmpt2 WHERE tmpt2.RelatedTicketID IN " & vbCrLf & _
            "                                       (SELECT DISTINCT tmptb2.TicketID FROM TicketBets tmptb2 WHERE tmptb2.GameID=" & SQLString(psGameID) & vbCrLf & _
                                                    getConditionAND_TicketBetContext(poCheckGameMode) & ") " & vbCrLf & _
            ")")

        Dim sSQL As String = _
            "SELECT tb.*, t.*, g.*, g.HomePitcher AS GameHomePitcher, g.AwayPitcher AS GameAwayPitcher, " & vbCrLf & _
            "tb.HomePitcher AS BetHomePitcher, tb.AwayPitcher AS BetAwayPitcher, gl.PropStatus, p.SiteType " & vbCrLf & _
            "FROM TicketBets tb" & vbCrLf & _
            "INNER JOIN Tickets t ON t.TicketID=tb.TicketID" & vbCrLf & _
            "INNER JOIN Games g ON g.GameID=tb.GameID" & vbCrLf & _
            "INNER JOIN Players p ON t.PlayerID = p.PlayerID" & vbCrLf & _
            "LEFT OUTER JOIN GameLines gl ON gl.GameLineID=tb.GameLineID" & vbCrLf & _
            oWhere.SQL.Trim() & vbCrLf & _
            "ORDER BY tb.TicketID, g.GameDate, tb.TicketBetID, t.AgentID, t.PlayerID"

        'log.Debug("Get the list of ticket bets. SQL: " & vbCrLf & sSQL)

        Try
            Return Me.SqlDB.getDataTable(sSQL)

        Catch ex As Exception
            Throw New Exception("Cannot get the list of ticket bets. SQL: " & vbCrLf & sSQL, ex)
        End Try

        Return Nothing
    End Function

    Private Function getConditionAND_TicketBetContext(Optional ByVal poCheckGameMode As CheckGameMode = CheckGameMode.Final) As String
        Dim sContexts As String = ""

        Select Case poCheckGameMode
            Case CheckGameMode.FirstHalf
                sContexts = "'1H'"

            Case CheckGameMode.FirstQuater
                sContexts = "'1Q'"

            Case CheckGameMode.SecondQuater
                sContexts = "'2Q'"

            Case CheckGameMode.ThirdQuater
                sContexts = "'3Q'"

            Case CheckGameMode.Final
                sContexts = "'2H', '4Q', 'Series Prices', 'Current'"

            Case Else
                Return ""
        End Select

        Return " AND Context IN (" & sContexts & ") "
    End Function

    Private Function getTeaserOdds(ByVal poSqlTeaserRuleIDs As List(Of String)) As DataTable
        Dim odtSysSettings As DataTable = Nothing

        Dim oWhere As New CSQLWhereStringBuilder
        oWhere.AppendANDCondition("ss.Category Like '% TEASER ODDS'")
        oWhere.AppendANDCondition("tr.TeaserRuleID IN (" & Join(poSqlTeaserRuleIDs.ToArray, ",") & ")")

        Dim sSQL As String = "SELECT  ss.[Key], ss.[Value], tr.TeaserRuleID, tr.IsTiesLose, tr.MinTeam, tr.MaxTeam " & vbCrLf & _
            "FROM SysSettings ss INNER JOIN TeaserRules tr ON convert(varchar(50),tr.TeaserRuleID)= convert(varchar(50),ss.SubCategory ) " & vbCrLf & _
            oWhere.SQL & vbCrLf & _
            " ORDER BY ss.SubCategory, ss.[Key], ss.[Value]"
        'log.Debug("Get the list of teaser odds. SQL: " & sSQL)

        Try
            odtSysSettings = Me.SqlDB.getDataTable(sSQL)

        Catch ex As Exception
            Throw New Exception("Cannot get the list of teaser odds. SQL: " & sSQL, ex)
        End Try

        Return odtSysSettings
    End Function

    Private Sub updateGameForFieldDate(ByVal psGameID As String, ByVal psFieldDateName As String)
        Dim oUpdate As New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(psGameID))
        oUpdate.AppendString(psFieldDateName, SQLDate(DateTime.Now.ToUniversalTime))

        ' log.Debug("Update " & psFieldDateName & " of game. SQL: " & oUpdate.SQL)

        Try
            Me.SqlDB.executeNonQuery(oUpdate, "")
        Catch ex As Exception
            Throw New Exception("Cannot update " & psFieldDateName & " of game. SQL: " & oUpdate.SQL, ex)
        End Try
    End Sub

    Private Sub updateResultTickets(ByVal poTickets As List(Of CCheckTicket))
        Dim oCustTickets As List(Of CCheckTicket) = poTickets.FindAll(Function(x) x.TicketStatus <> STATUS_OPEN)

        If oCustTickets.Count = 0 Then
            Return
        End If

        Try
            '    log.Debug("---- Start update result tickets: " & FormatNumber(oCustTickets.Count, 0))

            For nIndex As Integer = 0 To oCustTickets.Count - 1
                Dim oTicket As CCheckTicket = oCustTickets(nIndex)

                Dim oUpdate As New CSQLUpdateStringBuilder("Tickets", "WHERE TicketID=" & SQLString(oTicket.TicketID))
                oUpdate.AppendString("IsLoseStillCheckBets", SQLString(IIf(oTicket.IsLoseStillCheckBets, "Y", "N")))

                If Not oTicket.IsStatusChanged Then
                    '  log.DebugFormat("   #{1}: Ticket: {0}: IsLoseStillCheckBets = {2} ; Update ticket bets status" _
                    '                , oTicket.TicketID, nIndex + 1, oTicket.IsLoseStillCheckBets)
                Else
                    '  log.DebugFormat("   #{3}: Ticket: {0}: Status = {1} ; Net Amount = {2} ; IsLoseStillCheckBets = {4}" _
                    '           , oTicket.TicketID, oTicket.TicketStatus, oTicket.NetAmount, nIndex + 1, oTicket.IsLoseStillCheckBets)

                    oUpdate.AppendString("TicketStatus", SQLString(oTicket.TicketStatus))
                    oUpdate.AppendString("PendingStatus", SQLString(oTicket.PendingStatus))
                    If oTicket.TicketStatus <> STATUS_PENDING Then
                        oUpdate.AppendString("NetAmount", SQLDouble(oTicket.NetAmount))
                        oUpdate.AppendString("TicketCompletedDate", SQLDate(oTicket.TicketCompletedDate))
                        oUpdate.AppendString("CheckCompletedDate", SQLDate(CEngineStd.GetCurrentEasternDate()))
                    End If
                End If

                Me.SqlDB.executeNonQuery(oUpdate, "")

                ''ticket bet status
                Dim oCusTicketBets As List(Of CCheckTicketBet) = oTicket.TicketBets.FindAll(Function(x) x.IsStatusChanged)

                For nIndexBet As Integer = 0 To oCusTicketBets.Count - 1
                    Dim oTicketBet As CCheckTicketBet = oCusTicketBets(nIndexBet)
                    ' log.DebugFormat("       -{2}.{3}: Ticket Bet: {0}: Status = {1}", oTicketBet.TicketBetID, oTicketBet.TicketBetStatus _
                    '               , nIndex + 1, nIndexBet + 1)

                    oUpdate = New CSQLUpdateStringBuilder("TicketBets", "WHERE TicketBetID=" & SQLString(oTicketBet.TicketBetID))
                    oUpdate.AppendString("TicketBetStatus", SQLString(oTicketBet.TicketBetStatus))

                    Me.SqlDB.executeNonQuery(oUpdate, "")
                Next

                If nIndex < oCustTickets.Count - 1 Then
                    '   log.Debug("            ")
                End If
            Next

            ' log.Debug("---- End update result tickets")

        Catch ex As Exception
            Throw New Exception("Cannot update result for Tickets and TicketBets", ex)
        End Try
    End Sub

    Private Sub adjustBalanceAmountPlayers(ByVal poAmountPlayers As List(Of CCheckAmountPlayer))
        If poAmountPlayers.Count = 0 Then
            Return
        End If

        Try
            ' log.Debug("            ")
            ' log.Debug("---- Start adjust balance amount players")

            ''get balance amount of players from DB
            Dim sSQL As String = "SELECT DISTINCT PlayerID, BalanceAmount FROM Players WHERE PlayerID IN (" & _
                    Join((From oItem As CCheckAmountPlayer In poAmountPlayers Select SQLString(oItem.PlayerID)).ToArray(), ",") & ")"
            Dim odtBalancePlayers As DataTable = Me.SqlDB.getDataTable(sSQL)

            ''update balance amount player
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            For nIndex As Integer = 0 To poAmountPlayers.Count - 1
                Dim oAmountPlayer As CCheckAmountPlayer = poAmountPlayers(nIndex)

                Dim odrPlayer As DataRow() = odtBalancePlayers.Select("PlayerID=" & SQLString(oAmountPlayer.PlayerID))

                If odrPlayer.Length > 0 Then
                    'log.DebugFormat("   #{3}: Player ({0}): BalanceAmount = Current ({1}) + Total Net Amount({2})" _
                    '               , oAmountPlayer.PlayerID, odrPlayer(0)("BalanceAmount"), oAmountPlayer.TotalNetAmount _
                    '                 , nIndex + 1)

                    oUpdate = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID=" & SQLString(oAmountPlayer.PlayerID))
                    oUpdate.AppendString("BalanceAmount", SQLDouble(SafeDouble(odrPlayer(0)("BalanceAmount")) + oAmountPlayer.TotalNetAmount))

                    Me.SqlDB.executeNonQuery(oUpdate, "")
                End If
            Next

            'log.Debug("---- End adjust balance amount players")
            'log.Debug("            ")

        Catch ex As Exception
            Throw New Exception("Cannot update balance amount player", ex)
        End Try
    End Sub

#End Region

#Region "Get Parlay Payouts"

    Public Function getParlayPayouts() As List(Of CCheckParlayPayout)
        Dim odtCurrent As DataTable = getParlayPayoutFromDB(CATEGORY_CURRENT)
        Dim odtTiggerSB As DataTable = getParlayPayoutFromDB(CATEGORY_TIGERSB)

        If odtCurrent IsNot Nothing AndAlso odtTiggerSB IsNot Nothing Then
            Dim oPayouts As New List(Of CCheckParlayPayout)

            For nTeam As Int32 = 2 To 15
                Dim sWhere As String = "Key='" & nTeam.ToString() & " Teams'"

                Dim oCurrentFinds As DataRow() = odtCurrent.Select(sWhere)
                Dim oTigerSBFinds As DataRow() = odtTiggerSB.Select(sWhere)

                For nIndex As Integer = 0 To oTigerSBFinds.Count - 1
                    oPayouts.Add(New CCheckParlayPayout(oCurrentFinds(nIndex), oTigerSBFinds(nIndex)))
                Next
            Next

            Return oPayouts
        End If

        Return Nothing
    End Function

    Private Function getParlayPayoutFromDB(ByVal psCategory As String) As DataTable
        Dim sSQL As String = "SELECT * FROM SysSettings WHERE Category Like " & SQLString("%" & psCategory) & " ORDER BY Category, itemindex"
        'log.Debug("Get the list of parlay payouts. SQL: " & sSQL)

        Try
            Return Me.SqlDB.getDataTable(sSQL)

        Catch ex As Exception
            'logError(log, "Cannot get the list of parlay payouts. SQL: " & sSQL, ex)
        End Try

        Return Nothing
    End Function

#End Region

End Class

