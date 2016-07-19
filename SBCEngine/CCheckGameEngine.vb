Imports SBCEngine.CEngineStd

Imports WebsiteLibrary.CSBCStd
Imports WebsiteLibrary.DBUtils

Public Class CCheckGameEngine

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

#Region "Properties"

    Private ReadOnly Property IsSoccerGame(ByVal psGameType As String) As Boolean
        Get
            Return IsContains(SOCCER_GAMES, ";" & psGameType & ";")
        End Get
    End Property

    Private ReadOnly Property IsBaseballGame(ByVal psGameType As String) As Boolean
        Get
            Return IsContains(BASEBALL_GAMES, ";" & psGameType & ";")
        End Get
    End Property

    Private ReadOnly Property IsGolfGame(ByVal psGameType As String) As Boolean
        Get
            Return UCase(psGameType) = "GOLF"
        End Get
    End Property

    Public ReadOnly Property CanCheckBet(ByVal poGame As CCheckGame, ByVal psContext As String) As Boolean
        Get
            If poGame.IsProposition Then
                Return IsContains(poGame.GameStatus, "FINAL")
            End If

            Select Case UCase(psContext)
                Case "1Q"
                    Return IsContains(poGame.GameStatus, "FINAL") OrElse poGame.CurrentQuater > 1

                Case "2Q"
                    Return IsContains(poGame.GameStatus, "FINAL") OrElse poGame.CurrentQuater > 2 _
                            OrElse (poGame.CurrentQuater = 2 AndAlso IsContains(poGame.GameStatus, "TIME"))
                Case "3Q"
                    Return IsContains(poGame.GameStatus, "FINAL") OrElse poGame.CurrentQuater > 3 _
                            OrElse (poGame.CurrentQuater = 3 AndAlso IsContains(poGame.GameStatus, "TIME"))
                Case "1H"
                    Return IsContains(poGame.GameStatus, "TIME", "FINAL") OrElse poGame.IsFirstHalfFinished
            End Select

            '2H, 4Q, Current
            Return IsContains(poGame.GameStatus, "FINAL")
        End Get
    End Property

    Private ReadOnly Property MaxGameDate(ByVal poTicket As CCheckTicket) As Date
        Get
            Return (From oTicketBet As CCheckTicketBet In poTicket.TicketBets _
                    Join oGame In Games On oGame.GameID Equals oTicketBet.GameID _
                    Select oGame.GameDate).Max
        End Get
    End Property

    Private ReadOnly Property MinLoseGameDate(ByVal poTicket As CCheckTicket) As Date
        Get
            Return (From oTicketBet As CCheckTicketBet In poTicket.TicketBets _
                    Join oGame In Games On oGame.GameID Equals oTicketBet.GameID _
                    Where oTicketBet.TicketBetStatus = STATUS_LOSE Select oGame.GameDate).Min
        End Get
    End Property

#End Region

#Region "Fields"

    Public RoundingOption As String = "UP" '"NEAREST"

    Public Games As New List(Of CCheckGame)
    Public Tickets As New List(Of CCheckTicket)
    Public TeaserOdds As New List(Of CCheckTeaserOdds)
    Public ParlayPayouts As New List(Of CCheckParlayPayout)
    Public AmountPlayers As New List(Of CCheckAmountPlayer)

#End Region

#Region "Set values"

    Public Sub SetValues(ByVal podtData As DataTable)
        For Each odrData As DataRow In podtData.Rows
            ''set value game object
            If getGame(SafeString(odrData("GameID"))) Is Nothing Then
                Me.Games.Add(New CCheckGame(odrData))
            End If

            ''set value ticket object
            Dim oTicket As CCheckTicket = getTicket(SafeString(odrData("TicketID")))

            If oTicket Is Nothing Then
                oTicket = New CCheckTicket(odrData)
                Me.Tickets.Add(oTicket)
            End If

            ''set value ticket bet object
            If oTicket.GetTicketBet(SafeString(odrData("TicketBetID"))) Is Nothing Then
                oTicket.TicketBets.Add(New CCheckTicketBet(odrData))
            End If
        Next
    End Sub


    'Public Sub SetParlayPayouts(ByVal podtCurrent As DataTable, ByVal podtTigerSB As DataTable)
    '    For nTeam As Int32 = 2 To 15
    '        Dim sWhere As String = "Key='" & nTeam.ToString() & " Teams'"

    '        Dim oCurrentFinds As DataRow() = podtCurrent.Select(sWhere)
    '        Dim oTigerSBFinds As DataRow() = podtTigerSB.Select(sWhere)

    '        If oCurrentFinds.Length > 0 AndAlso oTigerSBFinds.Length > 0 Then
    '            Me.ParlayPayouts.Add(New CCheckParlayPayout(oCurrentFinds(0), oTigerSBFinds(0)))
    '        End If
    '    Next
    'End Sub

    Public Sub SetTeaserOddValues(ByVal podtData As DataTable)
        For Each odrData As DataRow In podtData.Rows
            If getTeaserOdds(SafeString(odrData("TeaserRuleID")), SafeInteger(odrData("Key"))) Is Nothing Then
                Me.TeaserOdds.Add(New CCheckTeaserOdds(odrData))
            End If
        Next
    End Sub

#End Region

#Region "Execute check"

    Public Function ExecuteCheck() As Boolean
        Me.Tickets.RemoveAll(Function(x) x.TicketBets.Count = 0)

        For nIndex As Integer = 0 To Me.Tickets.Count - 1
            Dim oTicket As CCheckTicket = Me.Tickets(nIndex)
            log.DebugFormat("#{0}: Check Ticket: {1} - {2} - {3}", nIndex + 1, oTicket.TicketID, oTicket.TicketStatus, oTicket.TicketType)

            Dim sTicketType As String = UCase(oTicket.TicketType)

            Select Case True
                Case sTicketType.Contains("STRAIGHT"), sTicketType.Contains("IF ") ' If Win; If Win or Push
                    executeCheck_STRAIGHT(oTicket)

                Case sTicketType.Contains("PARLAY")
                    executeCheck_PARLAY(oTicket)

                Case sTicketType.Contains("TEASER")
                    executeCheck_TEASER(oTicket)

                Case sTicketType.Contains("REVERSE") 'Action Reverse, Win Reverse
                    executeCheck_REVERSE(oTicket)
            End Select

            log.DebugFormat("#{0}: End Check Ticket: Status: {1}", nIndex + 1, oTicket.TicketStatus)
            log.Debug("            ")
        Next

        ''calculator banalance amount of players
        calculateBanlanceAmountPlayers()

        Return True
    End Function

    Private Sub calculateBanlanceAmountPlayers()
        Me.AmountPlayers = _
            (From oTicket As CCheckTicket In Me.Tickets _
             Where oTicket.NetAmount <> 0 AndAlso oTicket.IsStatusChanged _
                AndAlso Not (oTicket.TicketStatus = STATUS_OPEN OrElse oTicket.TicketStatus = STATUS_PENDING) _
             Group oTicket By oTicket.PlayerID Into PlayerGroup = Group _
             Let TotalNetAmount As Double = PlayerGroup.Sum(Function(x) x.NetAmount) _
             Select New CCheckAmountPlayer(PlayerID, TotalNetAmount)).ToList()
    End Sub

#End Region

#Region "Execute Straight"

    Public Function executeCheck_STRAIGHT(ByRef oTicket As CCheckTicket)
        ''1: check ticket bet
        Dim oTicketBet As CCheckTicketBet = oTicket.TicketBets(0)
        Dim oGame As CCheckGame = getGame(oTicketBet.GameID)

        If (oTicketBet.Context = "1H" AndAlso Not oGame.IsFirstHalfFinished AndAlso oGame.IsGameSuspend) OrElse _
         (oTicketBet.Context <> "1H" AndAlso oGame.IsGameSuspend) Then
            checkStatusTicketBetWithGameSuspend(oTicketBet, oGame)

        ElseIf Me.CanCheckBet(oGame, oTicketBet.Context) Then
            If oGame.IsProposition Then
                checkStatusTicketBetWithGameProposition(oTicketBet, oGame)
            Else
                checkStatusTicketBet(oTicketBet, oGame)
            End If
        End If

        ''2: check ticket
        If UCase(oTicket.TicketType).Contains("IF ") Then ' If Win/If Win or Push
            If oTicket.SubTicketNumber = 1 Then ' First Selection
                oTicket.TicketStatus = oTicket.TicketBets(0).TicketBetStatus
                oTicket.TicketCompletedDate = oGame.GameDate

                executeCheck_IFBET(oTicket)
            Else ' Second Selection
                oTicket.PendingStatus = oTicket.TicketBets(0).TicketBetStatus
                oTicket.TicketCompletedDate = oGame.GameDate

                ' Get First Selection
                Dim oFirstTicket As CCheckTicket = Me.getTicket(oTicket.RelatedTicketID)

                ' Check status of first selection
                If oFirstTicket IsNot Nothing AndAlso oFirstTicket.TicketStatus <> STATUS_OPEN Then
                    ' Go to check FirstSelection
                    executeCheck_IFBET(oFirstTicket)
                End If
            End If
        Else
            oTicket.TicketStatus = oTicket.TicketBets(0).TicketBetStatus
        End If

        ''3: calculator net amount
        If oTicket.IsStatusChanged Then
            oTicket.TicketCompletedDate = oGame.GameDate
            calculateNetAmount_STRAIGHT(oTicket)
        End If

        Return True
    End Function

    Public Sub executeCheck_IFBET(ByRef oFirstTicket As CCheckTicket)
        ' Get second selection
        Dim oLstSecondTickets As List(Of CCheckTicket) = Me.getRelatedTicket(oFirstTicket.RelatedTicketID)

        If oLstSecondTickets IsNot Nothing Then
            For Each oSecondTicket As CCheckTicket In oLstSecondTickets
                Select Case UCase(oFirstTicket.TicketType)
                    Case "IF WIN"
                        If oFirstTicket.TicketStatus <> STATUS_WIN Then
                            ' Set second selection is CANCELLED
                            oSecondTicket.TicketStatus = STATUS_CANCELLED

                            ' calculator net amount
                            If oSecondTicket.IsStatusChanged Then
                                oSecondTicket.TicketCompletedDate = oFirstTicket.TicketCompletedDate
                                calculateNetAmount_STRAIGHT(oSecondTicket)
                            End If
                        Else
                            ' Check Status of second bet
                            If oSecondTicket.PendingStatus <> STATUS_OPEN Then
                                oSecondTicket.TicketStatus = oSecondTicket.PendingStatus

                                ' calculator net amount
                                If oSecondTicket.IsStatusChanged Then
                                    calculateNetAmount_STRAIGHT(oSecondTicket)
                                End If
                            End If
                        End If
                    Case Else '' "IF WIN OR PUSH"
                        If oFirstTicket.TicketStatus <> STATUS_WIN AndAlso oFirstTicket.TicketStatus <> STATUS_CANCELLED Then
                            ' Set second selection is CANCELLED
                            oSecondTicket.TicketStatus = STATUS_CANCELLED

                            ' calculator net amount
                            If oSecondTicket.IsStatusChanged Then
                                oSecondTicket.TicketCompletedDate = oFirstTicket.TicketCompletedDate
                                calculateNetAmount_STRAIGHT(oSecondTicket)
                            End If
                        Else
                            ' Check Status of second bet
                            If oSecondTicket.PendingStatus <> STATUS_OPEN Then
                                oSecondTicket.TicketStatus = oSecondTicket.PendingStatus

                                ' calculator net amount
                                If oSecondTicket.IsStatusChanged Then
                                    calculateNetAmount_STRAIGHT(oSecondTicket)
                                End If
                            End If
                        End If
                End Select
            Next
        End If
    End Sub

    Private Sub calculateNetAmount_STRAIGHT(ByRef oTicket As CCheckTicket)
        Select Case oTicket.TicketStatus
            Case STATUS_LOSE
                oTicket.NetAmount = safeRound(oTicket.RiskAmount * (1 - Math.Abs(oTicket.TicketBets(0).OddsRatio)))

            Case STATUS_WIN
                oTicket.NetAmount = safeRound((oTicket.WinAmount * oTicket.TicketBets(0).OddsRatio) + oTicket.RiskAmount)

            Case STATUS_CANCELLED
                oTicket.NetAmount = safeRound(oTicket.RiskAmount)
        End Select
    End Sub

#End Region

#Region "Execute Parlay"

    Public Function executeCheck_PARLAY(ByRef oTicket As CCheckTicket)
        ''1: check ticket bets
        checkStatusTicketBets(oTicket)

        ''2: check ticket
        checkStatusTicket(oTicket)

        ''3: calculator net amount
        calculateNetAmount_PARLAY(oTicket)

        Return True
    End Function

    Private Sub calculateNetAmount_PARLAY(ByRef poTicket As CCheckTicket)
        Select Case poTicket.TicketStatus
            Case STATUS_LOSE, STATUS_PENDING, STATUS_OPEN
                poTicket.NetAmount = 0
                Return

            Case STATUS_CANCELLED
                poTicket.NetAmount = safeRound(poTicket.RiskAmount)

            Case STATUS_WIN
                Dim oCustomizeTicketBets As List(Of CCheckTicketBet) = poTicket.TicketBets.FindAll(Function(x) x.TicketBetStatus = STATUS_WIN)

                For Each oTicketBet As CCheckTicketBet In oCustomizeTicketBets
                    Dim oGame As CCheckGame = getGame(oTicketBet.GameID)

                    If IsSoccerGame(oGame.GameType) AndAlso oTicketBet.OddsRatio = 0.5 Then
                        oTicketBet.RenewMoneyForSoccerOnly()
                    End If
                Next

                If oCustomizeTicketBets.Count > 0 Then
                    poTicket.NetAmount = safeRound(poTicket.RiskAmount + CalculatePayoutParlay(poTicket.RiskAmount, poTicket.SiteType, oCustomizeTicketBets))
                End If
        End Select
    End Sub

#End Region

#Region "Execute Teaser"

    Public Function executeCheck_TEASER(ByRef oTicket As CCheckTicket)
        ''1: check ticket bets
        checkStatusTicketBets(oTicket)

        ''2: check ticket 
        ''if ticket has teaser odd with tie is lose rule
        ''then change ticket bet status is lose where it's status is cancelled
        Dim oTeaserOdd As CCheckTeaserOdds = getTeaserOdds(oTicket.TeaserRuleID, oTicket.TicketBets.Count)

        If oTeaserOdd IsNot Nothing AndAlso oTeaserOdd.IsTiesLose Then
            Dim oTicketBets = _
                    From oTicketBet As CCheckTicketBet In oTicket.TicketBets _
                    Join oGame As CCheckGame In Me.Games On UCase(oGame.GameID) Equals UCase(oTicketBet.GameID) _
                    Where oTicketBet.TicketBetStatus = STATUS_CANCELLED AndAlso Not oGame.IsGameSuspend _
                    Select oTicketBet

            For Each oTicketBet As CCheckTicketBet In oTicketBets
                oTicketBet.TicketBetStatus = STATUS_LOSE
            Next
        End If

        checkStatusTicket_TEASER(oTicket)

        ''3: calculator net amount
        calculateNetAmount_TEASER(oTicket)

        Return True
    End Function

    Private Sub checkStatusTicket_TEASER(ByRef oTicket As CCheckTicket)
        If oTicket.CountBets(STATUS_LOSE) > 0 Then
            oTicket.TicketStatus = STATUS_LOSE
            oTicket.TicketCompletedDate = Me.MinLoseGameDate(oTicket)
            oTicket.IsLoseStillCheckBets = oTicket.CountBets(STATUS_OPEN) > 0

        ElseIf oTicket.CountBets(STATUS_CANCELLED) = oTicket.CountBets Then
            oTicket.TicketStatus = STATUS_CANCELLED
            oTicket.TicketCompletedDate = Me.MaxGameDate(oTicket)

        Else
            Dim nOpens As Int32 = oTicket.CountBets(STATUS_OPEN)

            If nOpens > 0 Then
                oTicket.TicketStatus = IIfStr(nOpens = oTicket.CountBets, STATUS_OPEN, STATUS_PENDING)
            Else
                oTicket.TicketStatus = IIfStr(oTicket.CountBets(STATUS_WIN) = 1, STATUS_CANCELLED, STATUS_WIN)
                oTicket.TicketCompletedDate = Me.MaxGameDate(oTicket)
            End If
        End If
    End Sub

    Private Sub calculateNetAmount_TEASER(ByRef poTicket As CCheckTicket)
        Select Case poTicket.TicketStatus
            Case STATUS_LOSE, STATUS_PENDING, STATUS_OPEN
                poTicket.NetAmount = 0
                Return

            Case STATUS_CANCELLED
                poTicket.NetAmount = safeRound(poTicket.RiskAmount)

            Case STATUS_WIN
                Dim oCustomizeTicketBets As List(Of CCheckTicketBet) = poTicket.TicketBets.FindAll(Function(x) x.TicketBetStatus = STATUS_WIN)

                If oCustomizeTicketBets.Count >= 2 Then
                    Dim oTeaserOdd As CCheckTeaserOdds = getTeaserOdds(poTicket.TeaserRuleID, oCustomizeTicketBets.Count)

                    If oTeaserOdd Is Nothing Then
                        poTicket.NetAmount = safeRound(poTicket.RiskAmount + poTicket.WinAmount)
                    Else
                        poTicket.NetAmount = safeRound(poTicket.RiskAmount + CalculatePayoutTeaser(oTeaserOdd.Payout, poTicket.RiskAmount, oCustomizeTicketBets))
                    End If
                End If
        End Select
    End Sub

#End Region

#Region "Execute Reverse"

    Public Function executeCheck_REVERSE(ByRef oTicket As CCheckTicket)
        ''1: check ticket bets
        checkStatusTicketBets(oTicket)

        ''2: check ticket
        checkStatusTicket_REVERSE(oTicket)

        ''2: calculator net amount
        calculateNetAmount_REVERSE(oTicket)

        Return True
    End Function

    Private Sub checkStatusTicket_REVERSE(ByRef oTicket As CCheckTicket)
        Dim nOpens As Int32 = oTicket.CountBets(STATUS_OPEN)

        If nOpens > 0 Then
            oTicket.TicketStatus = IIfStr(nOpens = oTicket.CountBets, STATUS_OPEN, STATUS_PENDING)

        Else
            If oTicket.CountBets(STATUS_LOSE) > 0 Then
                oTicket.TicketStatus = STATUS_LOSE
            Else
                oTicket.TicketStatus = IIfStr(oTicket.CountBets(STATUS_CANCELLED) = oTicket.CountBets, STATUS_CANCELLED, STATUS_WIN)
            End If
            oTicket.TicketCompletedDate = Me.MaxGameDate(oTicket)
        End If
    End Sub

    Private Sub calculateNetAmount_REVERSE(ByRef poTicket As CCheckTicket)
        Select Case poTicket.TicketStatus
            Case STATUS_PENDING, STATUS_OPEN
                poTicket.NetAmount = 0
                Return

            Case STATUS_CANCELLED
                poTicket.NetAmount = safeRound(poTicket.RiskAmount)

            Case STATUS_WIN, STATUS_LOSE
                poTicket.NetAmount = safeRound(poTicket.RiskAmount + CalculatePayoutReverse(poTicket.RiskAmount, poTicket.TicketBets, poTicket.TicketType))
        End Select
    End Sub

#End Region

#Region "Check: ticket status, ticket bet status"

    ''use for check status ticket bets: parlay, teaser, reverse
    Private Sub checkStatusTicketBets(ByRef oTicket As CCheckTicket)
        For Each oTicketBet As CCheckTicketBet In oTicket.TicketBets.FindAll(Function(x) x.TicketBetStatus = STATUS_OPEN)
            Dim oGame As CCheckGame = getGame(oTicketBet.GameID)

            Select Case True
                Case (oTicketBet.Context = "1H" AndAlso Not oGame.IsFirstHalfFinished AndAlso oGame.IsGameSuspend) OrElse _
                    (oTicketBet.Context <> "1H" AndAlso oGame.IsGameSuspend)
                    checkStatusTicketBetWithGameSuspend(oTicketBet, oGame)

                Case Me.CanCheckBet(oGame, oTicketBet.Context)
                    checkStatusTicketBet(oTicketBet, oGame)
            End Select
        Next
    End Sub

    ''use for check status ticket bet 
    Private Sub checkStatusTicketBet(ByRef oTicketBet As CCheckTicketBet, ByVal poGame As CCheckGame)
        log.DebugFormat("   - Check Ticket Bet: {0} - {1}", oTicketBet.TicketBetID, oTicketBet.TicketBetStatus)

        log.DebugFormat("       + Start Check: {7} - Home('{0}': {1}) - Away('{2}': {3}) | {4} - {5} {6}" _
                            , poGame.HomeTeam, FormatNumber(poGame.ChoiceHomeScore(oTicketBet.Context), 0) _
                            , poGame.AwayTeam, FormatNumber(poGame.ChoiceAwayScore(oTicketBet.Context), 0) _
                            , oTicketBet.ChoiceTeam, oTicketBet.BetType _
                            , IIf(oTicketBet.ChoiceSpreadOrPoint <> 0, oTicketBet.ChoiceSpreadOrPoint, "") _
                            , oTicketBet.Context)

        Select Case oTicketBet.BetType
            Case "SPREAD"
                Dim nScoreCompare As Int32 = 0, nScoreBet As Double = oTicketBet.ChoiceSpreadOrPoint
                If Me.IsGolfGame(poGame.GameType) Then nScoreBet = -nScoreBet

                If oTicketBet.ChoiceTeam = "HOME" Then
                    nScoreCompare = poGame.ChoiceAwayScore(oTicketBet.Context)
                    nScoreBet += poGame.ChoiceHomeScore(oTicketBet.Context)
                Else
                    nScoreCompare = poGame.ChoiceHomeScore(oTicketBet.Context)
                    nScoreBet += poGame.ChoiceAwayScore(oTicketBet.Context)
                End If

                Select Case nScoreBet
                    Case nScoreCompare
                        oTicketBet.TicketBetStatus = STATUS_CANCELLED

                    Case Is > nScoreCompare
                        oTicketBet.TicketBetStatus = STATUS_WIN
                        If Me.IsGolfGame(poGame.GameType) Then oTicketBet.TicketBetStatus = STATUS_LOSE

                    Case Is < nScoreCompare
                        oTicketBet.TicketBetStatus = STATUS_LOSE
                        If Me.IsGolfGame(poGame.GameType) Then oTicketBet.TicketBetStatus = STATUS_WIN
                End Select

            Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                Dim nTotalPointsGame As Int32 = 0

                If oTicketBet.BetType = "TEAMTOTALPOINTS" Then
                    nTotalPointsGame = SafeInteger(IIf(oTicketBet.TeamTotalName = "AWAY", poGame.ChoiceAwayScore(oTicketBet.Context), poGame.ChoiceHomeScore(oTicketBet.Context)))
                Else
                    nTotalPointsGame = poGame.ChoiceHomeScore(oTicketBet.Context) + poGame.ChoiceAwayScore(oTicketBet.Context)
                End If

                Select Case oTicketBet.ChoiceSpreadOrPoint
                    Case nTotalPointsGame
                        oTicketBet.TicketBetStatus = STATUS_CANCELLED

                    Case Is > nTotalPointsGame
                        oTicketBet.TicketBetStatus = IIfStr(oTicketBet.ChoiceTeam = "OVER", STATUS_LOSE, STATUS_WIN)

                    Case Is < nTotalPointsGame
                        oTicketBet.TicketBetStatus = IIfStr(oTicketBet.ChoiceTeam = "OVER", STATUS_WIN, STATUS_LOSE)
                End Select

            Case "MONEYLINE"
                Dim nChoiceAwayScore As Int32 = poGame.ChoiceAwayScore(oTicketBet.Context)

                Select Case poGame.ChoiceHomeScore(oTicketBet.Context)
                    Case nChoiceAwayScore
                        oTicketBet.TicketBetStatus = IIfStr(Me.IsSoccerGame(poGame.GameType), STATUS_LOSE, STATUS_CANCELLED)

                    Case Is > nChoiceAwayScore
                        oTicketBet.TicketBetStatus = IIfStr(oTicketBet.ChoiceTeam = "HOME", STATUS_WIN, STATUS_LOSE)
                        If Me.IsGolfGame(poGame.GameType) Then
                            oTicketBet.TicketBetStatus = IIfStr(oTicketBet.TicketBetStatus = STATUS_WIN, STATUS_LOSE, STATUS_WIN)
                        End If

                    Case Is < nChoiceAwayScore
                        oTicketBet.TicketBetStatus = IIfStr(oTicketBet.ChoiceTeam = "AWAY", STATUS_WIN, STATUS_LOSE)
                        If Me.IsGolfGame(poGame.GameType) Then
                            oTicketBet.TicketBetStatus = IIfStr(oTicketBet.TicketBetStatus = STATUS_WIN, STATUS_LOSE, STATUS_WIN)
                        End If
                End Select

            Case "DRAW"
                If Me.IsSoccerGame(poGame.GameType) Then
                    oTicketBet.TicketBetStatus = IIfStr(poGame.ChoiceHomeScore(oTicketBet.Context) = poGame.ChoiceAwayScore(oTicketBet.Context) _
                                                                 , STATUS_WIN, STATUS_LOSE)
                End If
        End Select

        If oTicketBet.TicketBetStatus <> STATUS_OPEN Then
            If Me.IsBaseballGame(poGame.GameType) AndAlso oTicketBet.IsCheckPitcher Then
                If Not (IsContains(poGame.OriginHomePitcher, "UNDECIDED", "TBA") OrElse IsContains(poGame.OriginAwayPitcher, "UNDECIDED", "TBA")) Then
                    If oTicketBet.HomePitcher <> poGame.HomePitcher OrElse oTicketBet.AwayPitcher <> poGame.AwayPitcher Then
                        oTicketBet.TicketBetStatus = STATUS_CANCELLED
                    End If
                End If
            End If

            Dim nOddsRatio As Double = 1
            If IsSoccerGame(poGame.GameType) Then nOddsRatio = getOdds_SOCCER(oTicketBet, poGame)

            Select Case oTicketBet.TicketBetStatus
                Case STATUS_WIN
                    oTicketBet.OddsRatio = nOddsRatio

                Case STATUS_LOSE
                    oTicketBet.OddsRatio = -nOddsRatio

                Case STATUS_CANCELLED
                    oTicketBet.OddsRatio = 0
            End Select
        End If

        log.DebugFormat("       + End Check: {0}", oTicketBet.TicketBetStatus)
    End Sub

    'use for check status ticket bet with game is suspend
    Private Sub checkStatusTicketBetWithGameSuspend(ByRef oTicketBet As CCheckTicketBet, ByVal poGame As CCheckGame)
        log.DebugFormat("   - Check Ticket Bet: {0} - {1}", oTicketBet.TicketBetID, oTicketBet.TicketBetStatus)
        log.DebugFormat("       + Start Check: {7} - Game Is Suspend - Home('{0}': {1}) - Away('{2}': {3}) | {4} - {5} {6}" _
                            , poGame.HomeTeam, FormatNumber(poGame.ChoiceHomeScore(oTicketBet.Context), 0) _
                            , poGame.AwayTeam, FormatNumber(poGame.ChoiceAwayScore(oTicketBet.Context), 0) _
                            , oTicketBet.ChoiceTeam, oTicketBet.BetType _
                            , IIf(oTicketBet.ChoiceSpreadOrPoint <> 0, oTicketBet.ChoiceSpreadOrPoint, "") _
                            , oTicketBet.Context)

        oTicketBet.TicketBetStatus = STATUS_CANCELLED
        oTicketBet.OddsRatio = 0

        log.DebugFormat("       + End Check: {0}", oTicketBet.TicketBetStatus)
    End Sub

    'use for check status ticket bet with game is proposition
    Private Sub checkStatusTicketBetWithGameProposition(ByRef oTicketBet As CCheckTicketBet, ByVal poGame As CCheckGame)
        log.DebugFormat("   - Check Ticket Bet: {0} - {1}", oTicketBet.TicketBetID, oTicketBet.TicketBetStatus)
        log.DebugFormat("       + Start Check: - Game Proposition - Participant Name '{0}': {1} ({2})" _
                            , oTicketBet.PropParticipantName, oTicketBet.BetType, oTicketBet.PropMoneyLine)

        Select Case oTicketBet.PropStatus
            Case STATUS_LOSE
                oTicketBet.TicketBetStatus = STATUS_LOSE
                oTicketBet.OddsRatio = -1

            Case STATUS_CANCELLED
                oTicketBet.TicketBetStatus = STATUS_CANCELLED
                oTicketBet.OddsRatio = 0

            Case STATUS_WIN
                oTicketBet.TicketBetStatus = STATUS_WIN
                oTicketBet.OddsRatio = 1
        End Select

        log.DebugFormat("       + End Check: {0}", oTicketBet.TicketBetStatus)
    End Sub

    ''use for check status ticket: parlay, teaser
    Private Sub checkStatusTicket(ByRef oTicket As CCheckTicket)
        If oTicket.CountBets(STATUS_LOSE) > 0 Then
            oTicket.TicketStatus = STATUS_LOSE
            oTicket.TicketCompletedDate = Me.MinLoseGameDate(oTicket)
            oTicket.IsLoseStillCheckBets = oTicket.CountBets(STATUS_OPEN) > 0

        Else
            Dim nOpens As Int32 = oTicket.CountBets(STATUS_OPEN)
            If nOpens > 0 Then
                oTicket.TicketStatus = IIfStr(nOpens = oTicket.CountBets, STATUS_OPEN, STATUS_PENDING)
            Else
                oTicket.TicketStatus = IIfStr(oTicket.CountBets(STATUS_CANCELLED) = oTicket.CountBets, STATUS_CANCELLED, STATUS_WIN)
                oTicket.TicketCompletedDate = Me.MaxGameDate(oTicket)
            End If
        End If
    End Sub

    Private Function getOdds_SOCCER(ByVal oTicketBet As CCheckTicketBet, ByVal poGame As CCheckGame)
        Dim nOddsRatio As Double = oTicketBet.OddsRatio

        Select Case oTicketBet.BetType
            Case "SPREAD"
                Dim nPeriodScore As Double = 0

                If oTicketBet.ChoiceTeam = "HOME" Then
                    nPeriodScore += Math.Abs(poGame.ChoiceHomeScore(oTicketBet.Context) + oTicketBet.ChoiceSpreadOrPoint - poGame.ChoiceAwayScore(oTicketBet.Context))
                Else
                    nPeriodScore += Math.Abs(poGame.ChoiceAwayScore(oTicketBet.Context) + oTicketBet.ChoiceSpreadOrPoint - poGame.ChoiceHomeScore(oTicketBet.Context))
                End If

                nOddsRatio = SafeDouble(IIf(nPeriodScore >= 0.5, 1, 0.5))

            Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                Dim nTotalPointsGame As Int32 = 0

                If oTicketBet.BetType = "TEAMTOTALPOINTS" Then
                    nTotalPointsGame = SafeInteger(IIf(oTicketBet.TeamTotalName = "AWAY", poGame.ChoiceAwayScore(oTicketBet.Context), poGame.ChoiceHomeScore(oTicketBet.Context)))
                Else
                    nTotalPointsGame = poGame.ChoiceHomeScore(oTicketBet.Context) + poGame.ChoiceAwayScore(oTicketBet.Context)
                End If

                nOddsRatio = SafeDouble(IIf(Math.Abs(nTotalPointsGame - oTicketBet.ChoiceSpreadOrPoint) >= 0.5, 1, 0.5))

            Case "MONEYLINE", "DRAW"
                nOddsRatio = 1
        End Select

        Return nOddsRatio
    End Function

#End Region

#Region "Calculate Payout: parlay, reverse, teaser"

    Public Function CalculatePayoutParlay(ByVal pnRiskAmount As Double, ByVal psSiteType As String, ByVal poCustomizeTicketBets As List(Of CCheckTicketBet)) As Double
        Dim nAmount As Double = pnRiskAmount

        For Each oTicketBet As CCheckTicketBet In poCustomizeTicketBets
            Dim nPriceBet As Double = 0

            If oTicketBet.ChoiceMoney < 0 Then
                nPriceBet = (-oTicketBet.ChoiceMoney + 100) / -oTicketBet.ChoiceMoney
            ElseIf oTicketBet.ChoiceMoney > 0 Then
                nPriceBet = (oTicketBet.ChoiceMoney + 100) / 100
            End If

            If nPriceBet <> 0 Then
                nAmount *= Math.Round(nPriceBet, 4, MidpointRounding.AwayFromZero)
            End If
        Next

        Dim nParlayAmount As Double = nAmount - pnRiskAmount
        Dim nParlayPercent As Double = getParlayPercent(poCustomizeTicketBets.Count, psSiteType)

        ''Parlay amount
        Return Math.Round(nParlayAmount * nParlayPercent, 2, MidpointRounding.AwayFromZero)
    End Function

    Public Function CalculatePayoutTeaser(ByVal pnPayout As Double, ByVal pnRiskAmount As Double, ByVal poCustomizeTicketBets As List(Of CCheckTicketBet)) As Double
        Return Math.Round((pnPayout * pnRiskAmount) / 100, 2, MidpointRounding.AwayFromZero)
    End Function

    Public Function CalculatePayoutReverse(ByVal pnRiskAmount As Double, ByVal poTicketBets As List(Of CCheckTicketBet), Optional ByVal psAction As String = "ACTION REVERSE") As Double
        If poTicketBets.Count < 2 Then
            Return 0 'reverse must be larger or equal 2 bets
        End If

        Dim nPayouts As Double = 0

        'RiskAmount = BetAmount * NumOfBets * (NumOfBets -1)
        Dim nBetAmount As Double = pnRiskAmount / (poTicketBets.Count * (poTicketBets.Count - 1))

        ''reverse calculator
        Dim oTicketBetsCustomize As List(Of CCheckTicketBet) = _
            (From oTicketBet As CCheckTicketBet In poTicketBets _
             Join oGame As CCheckGame In Me.Games On UCase(oGame.GameID) Equals UCase(oTicketBet.GameID) _
             Where Not oGame.IsGameSuspend _
             Select oTicketBet).ToList()

        For Each oTicketBet As CCheckTicketBet In oTicketBetsCustomize
            nPayouts += getPayoutEachBetReverse(nBetAmount, oTicketBet)

            If oTicketBet.TicketBetStatus = STATUS_WIN OrElse (oTicketBet.TicketBetStatus = STATUS_CANCELLED AndAlso UCase(psAction) = "ACTION REVERSE") Then
                For Each oTicketBetOther As CCheckTicketBet In oTicketBetsCustomize
                    If oTicketBetOther.TicketBetID <> oTicketBet.TicketBetID Then
                        nPayouts += getPayoutEachBetReverse(nBetAmount, oTicketBetOther)
                    End If
                Next
            End If
        Next

        If nPayouts <> 0 Then
            nPayouts = Math.Round(nPayouts, 2, MidpointRounding.AwayFromZero)
        End If

        Return nPayouts
    End Function

    Private Function getPayoutEachBetReverse(ByVal pnBetAmount As Double, ByVal poTicketBet As CCheckTicketBet) As Double
        Select Case poTicketBet.TicketBetStatus
            Case "LOSE"
                Return -pnBetAmount

            Case "WIN"
                Dim nPriceBet As Double = 0

                If poTicketBet.ChoiceMoney < 0 Then
                    nPriceBet = Math.Round(1 - 100 / poTicketBet.ChoiceMoney, 4, MidpointRounding.AwayFromZero)
                Else
                    nPriceBet = Math.Round(1 + poTicketBet.ChoiceMoney / 100, 4, MidpointRounding.AwayFromZero)
                End If

                Return pnBetAmount * (nPriceBet - 1)
        End Select

        Return 0
    End Function

#End Region

#Region "Process"

    Private Function safeRound(ByVal pnSource As Double) As Double
        Select Case UCase(Me.RoundingOption)
            Case "UP"
                Return Math.Ceiling(pnSource)

            Case "DOWN"
                Return Math.Floor(pnSource)

            Case "NONE"
                Return Math.Round(pnSource, 2)
        End Select

        Return Math.Round(pnSource) 'NEAREST
    End Function

    Private Function getGame(ByVal psGameID As String) As CCheckGame
        Return Me.Games.Find(Function(x) UCase(x.GameID) = UCase(psGameID))
    End Function

    Private Function getTicket(ByVal psTicketID As String) As CCheckTicket
        Return Me.Tickets.Find(Function(x) UCase(x.TicketID) = UCase(psTicketID))
    End Function

    Private Function getRelatedTicket(ByVal psTicketID As String) As List(Of CCheckTicket)
        Return Me.Tickets.FindAll(Function(x) x.TicketID <> x.RelatedTicketID AndAlso UCase(x.RelatedTicketID) = UCase(psTicketID))
    End Function

    Private Function getTeaserOdds(ByVal psTeaserRuleID As String, ByVal pnTeams As Integer) As CCheckTeaserOdds
        Return Me.TeaserOdds.Find(Function(x) UCase(x.TeaserRuleID) = UCase(psTeaserRuleID) AndAlso x.Teams = pnTeams)
    End Function

    Private Function getParlayPercent(ByVal pnTeams As Integer, ByVal psSiteType As String) As Double
        Dim oPayout As CCheckParlayPayout = Me.ParlayPayouts.Find(Function(x) (x.Teams = pnTeams AndAlso x.Category.Contains(psSiteType)))

        If oPayout IsNot Nothing Then
            Return oPayout.PayoutPercent
        End If

        Return 1
    End Function

    Private Function IIfStr(ByVal pnExpression As Boolean, ByVal poTruePart As Object, ByVal poFlasePart As Object) As String
        Return SafeString(IIf(pnExpression, poTruePart, poFlasePart))
    End Function

#End Region

End Class


