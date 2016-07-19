Imports SBCBL.std
Imports SBCEngine
Imports SBCEngine.CEngineStd

Imports NUnit.Framework

Namespace TestSBCService

    <TestFixture()> Public Class CTestCheckGameEngine

        Private AgentID As String = "28FE4CB7-1975-495C-8AC8-B2FFCB825DB0" 'agent1
        Private PlayerID As String = "2B5510F2-670C-4374-861F-84DD3A28E240" 'huy70776
        Private BalanceAmountPlayer As Double = 100000

#Region "Test SOCCER Spread"

        ''HomeScore 
        ''AwayScore
        ''Spread
        ''BetHomeNegativeOddsExpected
        ''BetHomeNotNegativeOddsExpected
        ''BetAwayNegativeOddsExpected
        ''BetAwayNotNegativeOddsExpected
        ''TotalNetAmoutPlayerExpected (default: risk 100 -> win 120 )
        Shared SoccerSpread_ObjectTest() As Object = _
        { _
           New Object() {2, 1, 0.25, 1, 1, -1, -1, 440} _
            , New Object() {2, 1, 0.5, 1, 1, -1, -1, 440} _
            , New Object() {2, 1, 0.75, 0.5, 1, -1, -0.5, 430} _
            , New Object() {2, 1, 1, 0, 1, -1, 0, 420} _
            , New Object() {2, 1, 1.25, -0.5, 1, -1, 0.5, 430} _
            , New Object() {2, 1, 1.5, -1, 1, -1, 1, 440} _
        }

        <Test(), TestCaseSource("SoccerSpread_ObjectTest")> _
        Public Sub TestSoccerSpread(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer _
                                        , ByVal pnSpread As Double _
                                        , ByVal pnBetHomeNegativeOddsExpected As Double, ByVal pnBetHomeNotNegativeOddsExpected As Double _
                                        , ByVal pnBetAwayNegativeOddsExpected As Double, ByVal pnBetAwayNotNegativeOddsExpected As Double _
                                        , ByVal pnTotalNetAmountPlayerExpected As Double)

            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckSoccerSpreadBets(pnHomeScore, pnAwayScore, pnSpread, 100, 120)
            oCheckEngine.ExecuteCheck()

            Dim oTickets As List(Of CCheckTicket) = oCheckEngine.Tickets

            Assert.AreEqual(4, oTickets.Count, "There are 4 soccer spread bets for this game")
            Assert.AreEqual(100000 - (100 * 4), Me.BalanceAmountPlayer, "After soccer spread bets, balance amount must be " & FormatNumber(100000 - (100 * 4)))

            Dim sMessage As String = "", pnAbsSpread As Double = Math.Abs(pnSpread)

            ''Spread 1: home, spread is negative ----------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Gocentel(-{2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnAbsSpread, getStatus(pnBetHomeNegativeOddsExpected), FormatPercent(pnBetHomeNegativeOddsExpected))
            Assert.AreEqual(pnBetHomeNegativeOddsExpected, oTickets(0).TicketBets(0).OddsRatio, sMessage)

            ''Spread 2: home, spread is not negative ------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Gocentel(-{2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnAbsSpread, getStatus(pnBetHomeNotNegativeOddsExpected), FormatPercent(pnBetHomeNotNegativeOddsExpected))
            Assert.AreEqual(pnBetHomeNotNegativeOddsExpected, oTickets(1).TicketBets(0).OddsRatio, sMessage)

            ''Spread 3: away, spread is negative ----------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Gocentel(-{2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnAbsSpread, getStatus(pnBetAwayNegativeOddsExpected), FormatPercent(pnBetAwayNegativeOddsExpected))
            Assert.AreEqual(pnBetAwayNegativeOddsExpected, oTickets(2).TicketBets(0).OddsRatio, sMessage)

            ''Spread 4: away, spread is not negative ------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Gocentel(-{2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnAbsSpread, getStatus(pnBetAwayNotNegativeOddsExpected), FormatPercent(pnBetAwayNotNegativeOddsExpected))
            Assert.AreEqual(pnBetAwayNotNegativeOddsExpected, oTickets(3).TicketBets(0).OddsRatio, sMessage)

            ''Net Amount of Player ---------------------------------------------------------------------------------------
            Dim oAmountPlayer As CCheckAmountPlayer = oCheckEngine.AmountPlayers.Find(Function(x) x.PlayerID = Me.PlayerID)
            Assert.AreEqual(pnTotalNetAmountPlayerExpected, oAmountPlayer.TotalNetAmount, "Always have 4 tickets of this test with default is risk 100 -> win 120. " & _
                            "Total net amount player expected: " & FormatNumber(pnTotalNetAmountPlayerExpected))
        End Sub

        Private Function getCheckSoccerSpreadBets(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer, ByVal pnABSspread As Double _
                                            , ByVal pnRiskAmount As Double, ByVal pnWinAmount As Double) As CCheckGameEngine
            Dim oCheckEngine As New CCheckGameEngine
            oCheckEngine.RoundingOption = "NONE"

            ''init game
            Dim oGame As CCheckGame = addGame("SOCCER", "Gocentel", "Duy Tan Plaza", pnHomeScore, pnAwayScore)
            oCheckEngine.Games.Add(oGame)

            ''Spread 1: home, spread is negative ----------------------------------------
            Dim oTicket As CCheckTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addSpread(oGame.GameID, oTicket.TicketID, -pnABSspread, 120, "home"))

            ''Spread 2: bet home, home spread is not negative -------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addSpread(oGame.GameID, oTicket.TicketID, pnABSspread, 120, "home"))

            ''Spread 3: bet away, away spread is negative ----------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addSpread(oGame.GameID, oTicket.TicketID, -pnABSspread, 120, "away"))

            ''Spread 4: bet away, away spread is not negative ------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addSpread(oGame.GameID, oTicket.TicketID, pnABSspread, 120, "away"))

            ''-------------------------
            Return oCheckEngine
        End Function

#End Region

#Region "Test SOCCER Total Points"

        ''HomeScore 
        ''AwayScore
        ''TotalPoints
        ''BetUnderExpectedOdds
        ''BetOverOddsExpected
        ''TotalNetAmoutPlayerExpected (default: risk 100 -> win 120 )
        Shared SoccerTotalPoints_ObjectTest() As Object = _
        { _
           New Object() {2, 1, 3.25, 0.5, -0.5, 210} _
            , New Object() {2, 1, 3.5, 1, -1, 220} _
            , New Object() {2, 1, 3, 0, 0, 200} _
            , New Object() {2, 1, 2.75, -0.5, 0.5, 210} _
            , New Object() {2, 1, 2.5, -1, 1, 220} _
        }

        <Test(), TestCaseSource("SoccerTotalPoints_ObjectTest")> _
        Public Sub TestSoccerTotalPoints(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer _
                                        , ByVal pnTotalPoints As Double _
                                        , ByVal pnBetUnderExpectedOdds As Double, ByVal pnBetOverOddsExpected As Double _
                                        , ByVal pnTotalNetAmountPlayerExpected As Double)

            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckSoccerTotalPointsBet(pnHomeScore, pnAwayScore, pnTotalPoints, 100, 120)
            oCheckEngine.ExecuteCheck()

            Dim oTickets As List(Of CCheckTicket) = oCheckEngine.Tickets

            Assert.AreEqual(2, oTickets.Count, "There are 2 soccer total points bets for this game")
            Assert.AreEqual(100000 - (100 * 2), Me.BalanceAmountPlayer, "After soccer total points bets, balance amount must be " & FormatNumber(100000 - (100 * 2)))

            Dim sMessage As String = ""

            ''total points 1: bet under ---------------------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Under({2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnTotalPoints, getStatus(pnBetUnderExpectedOdds), FormatPercent(pnBetUnderExpectedOdds))
            Assert.AreEqual(pnBetUnderExpectedOdds, oTickets(0).TicketBets(0).OddsRatio, sMessage)

            ''total points 2: bet over ---------------------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Over({2}) - Expected: {3} {4}" _
                                     , pnHomeScore, pnAwayScore, pnTotalPoints, getStatus(pnBetOverOddsExpected), FormatPercent(pnBetOverOddsExpected))
            Assert.AreEqual(pnBetOverOddsExpected, oTickets(1).TicketBets(0).OddsRatio, sMessage)

            ''Net Amount of Player ---------------------------------------------------------------------------------------
            Dim oAmountPlayer As CCheckAmountPlayer = oCheckEngine.AmountPlayers.Find(Function(x) x.PlayerID = Me.PlayerID)
            Assert.AreEqual(pnTotalNetAmountPlayerExpected, oAmountPlayer.TotalNetAmount, "Always have 2 tickets of this test with default is risk 100 -> win 120. " & _
                            "Total net amount player expected: " & FormatNumber(pnTotalNetAmountPlayerExpected))
        End Sub

        Private Function getCheckSoccerTotalPointsBet(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer, ByVal pnTotalPoints As Double _
                                              , ByVal pnRiskAmount As Double, ByVal pnWinAmount As Double) As CCheckGameEngine
            Dim oCheckEngine As New CCheckGameEngine
            oCheckEngine.RoundingOption = "NONE"

            ''init game
            Dim oGame As CCheckGame = addGame("SOCCER", "Gocentel", "Duy Tan Plaza", pnHomeScore, pnAwayScore)
            oCheckEngine.Games.Add(oGame)

            ''total points 1: under ---------------------------------------------------
            Dim oTicket As CCheckTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addTotalPoints(oGame.GameID, oTicket.TicketID, pnTotalPoints, 120, "under"))

            ''total points 2: over ---------------------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addTotalPoints(oGame.GameID, oTicket.TicketID, pnTotalPoints, 120, "over"))

            ''-------------------------
            Return oCheckEngine
        End Function

#End Region

#Region "Test SOCCER Money Line"


        ''HomeScore 
        ''AwayScore
        ''BetHomeOddsExpected
        ''BetAwayOddsExpected
        ''BetDrawOddsExpected
        ''TotalNetAmoutPlayerExpected (default: risk 100 -> win 120 )
        Shared SoccerMoneyLine_ObjectTest() As Object = _
        { _
           New Object() {2, 1, 1, -1, -1, 220} _
            , New Object() {1, 1, -1, -1, 1, 220} _
            , New Object() {1, 2, -1, 1, -1, 220} _
        }

        <Test(), TestCaseSource("SoccerMoneyLine_ObjectTest")> _
        Public Sub TestSoccerMoneyLine(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer _
                                        , ByVal pnBetHomeOddsExpected As Double, ByVal pnBetAwayOddsExpected As Double _
                                        , ByVal pnBetDrawOddsExpected As Double, ByVal pnTotalNetAmountPlayerExpected As Double)

            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckSoccerMoneyLineBet(pnHomeScore, pnAwayScore, 100, 120)
            oCheckEngine.ExecuteCheck()

            Dim oTickets As List(Of CCheckTicket) = oCheckEngine.Tickets

            Assert.AreEqual(3, oTickets.Count, "There are 3 soccer moneyline bets for this game")
            Assert.AreEqual(100000 - (100 * 3), Me.BalanceAmountPlayer, "After soccer total points bets, balance amount must be " & FormatNumber(100000 - (100 * 3)))

            Dim sMessage As String = ""

            ''money line 1: bet home ---------------------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Gocentel - Expected: {2} {3}" _
                                     , pnHomeScore, pnAwayScore, getStatus(pnBetHomeOddsExpected), FormatPercent(pnBetHomeOddsExpected))
            'Assert.AreEqual("huyngo", oTickets(0).TicketBets(0).TicketBetStatus, sMessage)
            Assert.AreEqual(pnBetHomeOddsExpected, oTickets(0).TicketBets(0).OddsRatio, sMessage)

            ''money line 2: away ---------------------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Duy Tan Plaza - Expected: {2} {3}" _
                                     , pnHomeScore, pnAwayScore, getStatus(pnBetAwayOddsExpected), FormatPercent(pnBetAwayOddsExpected))
            Assert.AreEqual(pnBetAwayOddsExpected, oTickets(1).TicketBets(0).OddsRatio, sMessage)

            ''money line 3: away ---------------------------------------------------
            sMessage = String.Format("Gocentel({0}) vs Duy Tan Plaza({1}) | Bet: Duy Tan Plaza - Expected: {2} {3}" _
                                     , pnHomeScore, pnAwayScore, getStatus(pnBetDrawOddsExpected), FormatPercent(pnBetDrawOddsExpected))
            Assert.AreEqual(pnBetDrawOddsExpected, oTickets(2).TicketBets(0).OddsRatio, sMessage)

            ''Net Amount of Player ---------------------------------------------------------------------------------------
            Dim nTotalNetAmount As Double = 0

            Dim oAmountPlayer As CCheckAmountPlayer = oCheckEngine.AmountPlayers.Find(Function(x) x.PlayerID = Me.PlayerID)
            If oAmountPlayer IsNot Nothing Then nTotalNetAmount = oAmountPlayer.TotalNetAmount

            Assert.AreEqual(pnTotalNetAmountPlayerExpected, nTotalNetAmount, "Always have 2 tickets of this test with default is risk 100 -> win 120. " & _
                            "Total net amount player expected: " & FormatNumber(pnTotalNetAmountPlayerExpected))
        End Sub

        Private Function getCheckSoccerMoneyLineBet(ByVal pnHomeScore As Integer, ByVal pnAwayScore As Integer _
                                                , ByVal pnRiskAmount As Double, ByVal pnWinAmount As Double) As CCheckGameEngine
            Dim oCheckEngine As New CCheckGameEngine
            oCheckEngine.RoundingOption = "NONE"

            ''init game
            Dim oGame As CCheckGame = addGame("SOCCER", "Gocentel", "Duy Tan Plaza", pnHomeScore, pnAwayScore)
            oCheckEngine.Games.Add(oGame)

            ''money line 1: home ---------------------------------------------------
            Dim oTicket As CCheckTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addMoneyLine(oGame.GameID, oTicket.TicketID, 120, "home"))

            ''money line 2: away ---------------------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addMoneyLine(oGame.GameID, oTicket.TicketID, 120, "away"))

            'money line 3: away ---------------------------------------------------
            oTicket = addTicket("Straight", pnRiskAmount, pnWinAmount)
            oCheckEngine.Tickets.Add(oTicket)

            Me.BalanceAmountPlayer -= pnRiskAmount
            oTicket.TicketBets.Add(addMoneyLine(oGame.GameID, oTicket.TicketID, 120, "draw"))

            ''-------------------------
            Return oCheckEngine
        End Function

#End Region

#Region "Test Parlay"

        <Test()> Public Sub TestParlays_5Finals_1Lose()
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckParlay()

            ''set 2 games is final
            With oCheckEngine.Games(3) ''bet home: moneyline 160
                .GameStatus = "FINAL"
                .HomeScore = 77
                .AwayScore = 69
            End With
            With oCheckEngine.Games(4) ''bet away: spread -6/-133
                .GameStatus = "FINAL"
                .HomeScore = 71
                .AwayScore = 76
            End With
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual(5378.4, oCheckEngine.Tickets(0).WinAmount, "Win Amount: " & FormatCurrency(5378.4) & _
                                "after calculate payout parlay (http://www.jazzsports.com/services/parlaycalculator.php)")

            Assert.AreEqual("LOSE", oCheckEngine.Tickets(0).TicketStatus, "1 lose -> LOSE")

            Assert.AreEqual(0, oCheckEngine.Tickets(0).NetAmount, "1 lose -> Net amount expected: 0")
        End Sub

        <Test()> Public Sub TestParlays_5Finals_5Cancelleds()
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckParlay()

            With oCheckEngine.Games(0) ''bet home: spread -8/-110
                .HomeScore = 9
                .AwayScore = 1
            End With
            With oCheckEngine.Games(1) ''bet over: total points 192
                .HomeScore = 100
                .AwayScore = 92
            End With

            ''set 2 games is final
            With oCheckEngine.Games(3) ''bet home: moneyline 160
                .GameStatus = "FINAL"
                .HomeScore = 77
                .AwayScore = 77
            End With
            With oCheckEngine.Games(4) ''bet away: spread -6/-133
                .GameStatus = "FINAL"
                .HomeScore = 70
                .AwayScore = 76
            End With
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual(5378.4, oCheckEngine.Tickets(0).WinAmount, "Win Amount: " & FormatCurrency(5378.4) & _
                                "after calculate payout parlay (http://www.jazzsports.com/services/parlaycalculator.php)")

            Assert.AreEqual("CANCELLED", oCheckEngine.Tickets(0).TicketStatus, "5 cancelleds -> CANCELLED")

            Assert.AreEqual(100, oCheckEngine.Tickets(0).NetAmount, "5 cancelleds -> Net amount expected = risk amount: 100")
        End Sub

        <Test()> Public Sub TestParlays_5Finals_3Wins_2Cancelleds()
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckParlay()

            ''set 2 games is final
            With oCheckEngine.Games(3) ''bet home: moneyline 160
                .GameStatus = "FINAL"
                .HomeScore = 77
                .AwayScore = 69
            End With
            With oCheckEngine.Games(4) ''bet away: spread -6/-133
                .GameStatus = "FINAL"
                .HomeScore = 70
                .AwayScore = 76
            End With
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual(5378.4, oCheckEngine.Tickets(0).WinAmount, "Win Amount: " & FormatCurrency(5378.4) & _
                                "after calculate payout parlay (http://www.jazzsports.com/services/parlaycalculator.php)")

            Assert.AreEqual("WIN", oCheckEngine.Tickets(0).TicketStatus, "3 wins, 2 cancelled -> WIN")

            Assert.AreEqual(947.61, oCheckEngine.Tickets(0).NetAmount, "3 wins, 2 cancelled. Risk 100, 3 wins(-110, -110, 160) -> Net amount expected: 947.61")
        End Sub

        <Test()> Public Sub TestParlays_3Finals_2NotFinals()
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckParlay()
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual(5378.4, oCheckEngine.Tickets(0).WinAmount, "Win Amount: " & FormatCurrency(5378.4) & _
                                "after calculate payout parlay (http://www.jazzsports.com/services/parlaycalculator.php)")

            Assert.AreEqual("PENDING", oCheckEngine.Tickets(0).TicketStatus, "Have 2 games not final -> Status: PENDING")
        End Sub

        <Test()> Public Sub TestParlays_3Finals_2NotFinals_Continute_5Finals()
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckParlay()
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual(5378.4, oCheckEngine.Tickets(0).WinAmount, "Win Amount: " & FormatCurrency(5378.4) & _
                                "after calculate payout parlay (http://www.jazzsports.com/services/parlaycalculator.php)")

            Assert.AreEqual("PENDING", oCheckEngine.Tickets(0).TicketStatus, "Have 2 games not final -> Status: PENDING")

            ''set 2 games is final
            With oCheckEngine.Games(3) ''bet home: moneyline 160
                .GameStatus = "FINAL"
                .HomeScore = 77
                .AwayScore = 69
            End With
            With oCheckEngine.Games(4) ''bet away: spread -6/-133
                .GameStatus = "FINAL"
                .HomeScore = 70
                .AwayScore = 77
            End With
            oCheckEngine.ExecuteCheck()

            Assert.AreEqual("WIN", oCheckEngine.Tickets(0).TicketStatus, "3 wins, 2 cancelled -> WIN")

            Assert.AreEqual(1660.12, oCheckEngine.Tickets(0).NetAmount, "5 wins. Risk 100, 3 wins(-110, -110, 160, -133) -> Net amount expected: 1660.12")
        End Sub

        Private Function getCheckParlay() As CCheckGameEngine
            Me.BalanceAmountPlayer = 100000
            Dim oCheckEngine As New CCheckGameEngine
            oCheckEngine.RoundingOption = "NONE"

            '-----Add Games-----'
            ''final game, GameID:454610DC-41E7-42C3-9DE9-003FFF637619
            Dim oGameFinal1 As CCheckGame = addGame("NCAA Basketball", "Missouri KC", "Centenary", 83, 69)
            oCheckEngine.Games.Add(oGameFinal1)

            ''final game, GameID:AFF522C9-F336-4D0F-88B9-0004DD06EDCD
            Dim oGameFinal2 As CCheckGame = addGame("NBA Basketball", "Boston Celtics", "Cleveland Cavaliers", 88, 108)
            oCheckEngine.Games.Add(oGameFinal2)

            ''final game, GameID:AE34F3E0-FD2A-42A8-A1D0-FD5B4F3E536D
            Dim oGameFinal3 As CCheckGame = addGame("NCAA Basketball", "Nevada", "New Mexico State", 55, 55)
            oCheckEngine.Games.Add(oGameFinal3)

            ''not final game, GameID:416E7291-D480-449B-A69B-00006845C4C4
            Dim oGameNotFinal1 As CCheckGame = addGame("Golf", "C. Campbell", "J. Byrd", 0, 0, "ONE HALF")
            oCheckEngine.Games.Add(oGameNotFinal1)

            ''not final game, GameID:AF56629C-65B9-433A-BCA1-0008B95625E4
            Dim oGameNotFinal2 As CCheckGame = addGame("Tennis", "L. Pous Tio", "V. Williams", 0, 0, "SET ONE")
            oCheckEngine.Games.Add(oGameNotFinal2)

            '-----Add Ticket-----'
            Dim oTicket As CCheckTicket = addTicket("Parlay", 100, 0) '-> calculate win amount after add ticket bets
            oTicket.SiteType = "SBC"
            oCheckEngine.Tickets.Add(oTicket)

            '-----Add Ticket Bets-----'
            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -8, -110, "home"))
            oTicket.TicketBets.Add(addTotalPoints(oGameFinal2.GameID, oTicket.TicketID, 192, -110, "over"))
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal3.GameID, oTicket.TicketID, 230, "away"))

            oTicket.TicketBets.Add(addMoneyLine(oGameNotFinal1.GameID, oTicket.TicketID, 160, "home"))
            oTicket.TicketBets.Add(addSpread(oGameNotFinal2.GameID, oTicket.TicketID, -6, -133, "away"))

            ''reset win amount for ticket
            oTicket.WinAmount = oCheckEngine.CalculatePayoutParlay(oTicket.RiskAmount, oTicket.SiteType, oTicket.TicketBets)

            '------------------'
            Return oCheckEngine
        End Function

#End Region

#Region "Test Reverse"

        ''ActionType 
        ''Remark
        ''WinWinPayoutExpected
        ''WinPushPayoutExpected
        ''WinLosePayoutExpected
        ''PushPushPayoutExpected
        ''PushLosePayoutExpected
        ''LoseLosePayoutExpected
        Shared Reverse_ObjectTest() As Object = _
        { _
           New Object() {"ACTION REVERSE", "Payout = Net - Risk", 400, 200, -120, 0, -220, -220} _
            , New Object() {"WIN REVERSE", "Payout = Net - Risk", 400, 100, -120, 0, -110, -220} _
        }

        <Test(), TestCaseSource("Reverse_ObjectTest")> _
        Public Sub TestReverse(ByVal psActionType As String, ByVal psRemark As String _
                               , ByVal pnWinWinPayoutExpected As Double, ByVal pnWinPushPayoutExpected As Double, ByVal pnWinLosePayoutExpected As Double _
                               , ByVal pnPushPushPayoutExpected As Double, ByVal pnPushLosePayoutExpected As Double _
                               , ByVal pnLoseLosePayoutExpected As Double)
            Me.BalanceAmountPlayer = 100000

            Dim oCheckEngine As CCheckGameEngine = getCheckReverse(psActionType)
            oCheckEngine.ExecuteCheck()

            ''win amount
            For Each oTicket As CCheckTicket In oCheckEngine.Tickets
                Assert.AreEqual(400, oTicket.WinAmount, "Win Amount: " & FormatCurrency(400) & _
                                "after calculate payout reverse (http://www.sbrforum.com/Betting+Tools/Reverse+Bets.aspx)")
            Next

            ''win + win -> WIN
            Assert.AreEqual("WIN", oCheckEngine.Tickets(0).TicketStatus, "win + win -> WIN")
            Assert.AreEqual(pnWinWinPayoutExpected, oCheckEngine.Tickets(0).NetAmount - 220, "win + win -> WIN, payout expected: " & FormatCurrency(pnWinWinPayoutExpected))

            ''win + push -> WIN
            Assert.AreEqual("WIN", oCheckEngine.Tickets(1).TicketStatus, "win + push -> WIN")
            Assert.AreEqual(pnWinPushPayoutExpected, oCheckEngine.Tickets(1).NetAmount - 220, "win + push -> WIN, payout expected: " & FormatCurrency(pnWinPushPayoutExpected))

            ''win + lose -> LOSE
            Assert.AreEqual("LOSE", oCheckEngine.Tickets(2).TicketStatus, "win + lose -> LOSE")
            Assert.AreEqual(pnWinLosePayoutExpected, oCheckEngine.Tickets(2).NetAmount - 220, "win + lose -> LOSE, payout expected: " & FormatCurrency(pnWinLosePayoutExpected))

            ''push + push -> PUSH
            Assert.AreEqual("CANCELLED", oCheckEngine.Tickets(3).TicketStatus, "push + push -> PUSH")
            Assert.AreEqual(pnPushPushPayoutExpected, oCheckEngine.Tickets(3).NetAmount - 220, "push + push -> PUSH, payout expected: " & FormatCurrency(pnPushPushPayoutExpected))

            ''push + lose -> LOSE
            Assert.AreEqual("LOSE", oCheckEngine.Tickets(4).TicketStatus, "push + lose -> LOSE")
            Assert.AreEqual(pnPushLosePayoutExpected, oCheckEngine.Tickets(4).NetAmount - 220, "push + lose -> LOSE, payout expected: " & FormatCurrency(pnPushLosePayoutExpected))

            ''lose + lose -> LOSE
            Assert.AreEqual("LOSE", oCheckEngine.Tickets(5).TicketStatus, "lose + lose -> LOSE")
            Assert.AreEqual(pnLoseLosePayoutExpected, oCheckEngine.Tickets(5).NetAmount - 220, "lose + lose -> LOSE, payout expected: " & FormatCurrency(pnLoseLosePayoutExpected))
        End Sub

        Private Function getCheckReverse(Optional ByVal psType As String = "ACTION REVERSE") As CCheckGameEngine
            Me.BalanceAmountPlayer = 100000
            Dim oCheckEngine As New CCheckGameEngine
            oCheckEngine.RoundingOption = "NONE"

            '-----Add Games-----'
            ''final game, GameID:454610DC-41E7-42C3-9DE9-003FFF637619
            Dim oGameFinal1 As CCheckGame = addGame("NCAA Basketball", "Missouri KC", "Centenary", 83, 69)
            oCheckEngine.Games.Add(oGameFinal1)

            ''final game, GameID:AFF522C9-F336-4D0F-88B9-0004DD06EDCD
            Dim oGameFinal2 As CCheckGame = addGame("NBA Basketball", "Boston Celtics", "Cleveland Cavaliers", 88, 108)
            oCheckEngine.Games.Add(oGameFinal2)

            ''final game, GameID:AE34F3E0-FD2A-42A8-A1D0-FD5B4F3E536D
            Dim oGameFinal3 As CCheckGame = addGame("NCAA Basketball", "Nevada", "New Mexico State", 55, 55)
            oCheckEngine.Games.Add(oGameFinal3)

            ''not final game, GameID:416E7291-D480-449B-A69B-00006845C4C4
            Dim oGameNotFinal1 As CCheckGame = addGame("Golf", "C. Campbell", "J. Byrd", 0, 0, "ONE HALF")
            oCheckEngine.Games.Add(oGameNotFinal1)

            ''not final game, GameID:AF56629C-65B9-433A-BCA1-0008B95625E4
            Dim oGameNotFinal2 As CCheckGame = addGame("Tennis", "L. Pous Tio", "V. Williams", 0, 0, "SET ONE")
            oCheckEngine.Games.Add(oGameNotFinal2)

            '-----WIN, WIN-----'
            Dim oTicket As CCheckTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -8, -110, "home")) 'WIN 83:69
            oTicket.TicketBets.Add(addTotalPoints(oGameFinal2.GameID, oTicket.TicketID, 192, -110, "over")) 'WIN 88:108

            '-----WIN, PUSH-----'
            oTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -8, -110, "home")) 'WIN 83:69
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal3.GameID, oTicket.TicketID, -110)) 'PUSH 55:55

            '-----WIN, LOSE-----'
            oTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -8, -110, "home")) 'WIN 83:69
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal2.GameID, oTicket.TicketID, -110, "home")) 'LOSE 88:108

            '-----PUSH, PUSH-----'
            oTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -14, -110, "home")) 'PUSH 83:69
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal3.GameID, oTicket.TicketID, -110, "home")) 'PUSH 55:55

            '-----PUSH, LOSE-----'
            oTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -14, -110, "home")) 'PUSH 83:69
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal2.GameID, oTicket.TicketID, -110, "home")) 'LOSE 88:108

            '-----LOSE, LOSE-----'
            oTicket = addTicket(psType, 0, 0)
            oCheckEngine.Tickets.Add(oTicket)

            oTicket.TicketBets.Add(addSpread(oGameFinal1.GameID, oTicket.TicketID, -8, -110, "away")) 'LOSE 83:69
            oTicket.TicketBets.Add(addMoneyLine(oGameFinal2.GameID, oTicket.TicketID, -110, "home")) 'LOSE 88:108

            ''calculate risk, win
            For Each oTicketTmp As CCheckTicket In oCheckEngine.Tickets
                oTicketTmp.TicketStatus = STATUS_WIN

                For Each oTicketBetTmp As CCheckTicketBet In oTicketTmp.TicketBets
                    oTicketBetTmp.TicketBetStatus = STATUS_WIN
                Next

                oTicketTmp.RiskAmount = 220
                oTicketTmp.WinAmount = oCheckEngine.CalculatePayoutReverse(220, oTicketTmp.TicketBets)

                ''reset status to check game
                oTicketTmp.TicketStatus = STATUS_OPEN

                For Each oTicketBetTmp As CCheckTicketBet In oTicketTmp.TicketBets
                    oTicketBetTmp.TicketBetStatus = STATUS_OPEN
                Next
            Next

            '------------------'
            Return oCheckEngine
        End Function

#End Region

        Private Function getStatus(ByVal pnOddsRatio As String) As String
            Select Case pnOddsRatio
                Case Is > 0
                    Return "WIN"
                Case Is < 0
                    Return "LOSE"
            End Select

            Return "CANCELLED"
        End Function

        Private Function addGame(ByVal psGameType As String, ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal pnHomeScore As Integer _
                                 , ByVal pnAwayScore As Integer, Optional ByVal psGameStatus As String = "FINAL") As CCheckGame
            Dim oGame As New CCheckGame(newGUID())

            With oGame
                .GameType = psGameType
                .HomeTeam = psHomeTeam
                .AwayTeam = psAwayTeam
                .HomeScore = pnHomeScore
                .AwayScore = pnAwayScore
                .GameStatus = psGameStatus
            End With

            Return oGame
        End Function

        Private Function addTicket(ByVal psTicketType As String, ByVal pnRiskAmount As Double, ByVal pnWinAmount As Double) As CCheckTicket
            Dim oTicket As New CCheckTicket(newGUID)
            oTicket.TicketType = psTicketType

            With oTicket
                .PlayerID = Me.PlayerID
                .RiskAmount = pnRiskAmount
                .WinAmount = pnWinAmount
            End With

            Return oTicket
        End Function

        Private Function addSpread(ByVal psGameID As String, ByVal psTicketID As String, ByVal pnSpread As Double, ByVal pnSpreadMoney As Double, Optional ByVal psChoice As String = "home") As CCheckTicketBet
            Dim oTicketBet As New CCheckTicketBet(newGUID)
            oTicketBet.GameID = psGameID
            oTicketBet.TicketID = psTicketID
            oTicketBet.BetType = "SPREAD"

            Select Case UCase(psChoice)
                Case "HOME"
                    oTicketBet.HomeSpread = pnSpread
                    oTicketBet.HomeSpreadMoney = pnSpreadMoney
                Case "AWAY"
                    oTicketBet.AwaySpread = pnSpread
                    oTicketBet.AwaySpreadMoney = pnSpreadMoney
            End Select

            Return oTicketBet
        End Function

        Private Function addTotalPoints(ByVal psGameID As String, ByVal psTicketID As String, ByVal pnTotalPoints As Double, ByVal pnMoney As Double, Optional ByVal psChoice As String = "under") As CCheckTicketBet
            Dim oTicketBet As New CCheckTicketBet(newGUID)
            oTicketBet.GameID = psGameID
            oTicketBet.TicketID = psTicketID
            oTicketBet.BetType = "TOTALPOINTS"
            oTicketBet.TotalPoints = pnTotalPoints

            Select Case UCase(psChoice)
                Case "UNDER"
                    oTicketBet.TotalPointsUnderMoney = pnMoney
                Case "OVER"
                    oTicketBet.TotalPointsOverMoney = pnMoney
            End Select

            Return oTicketBet
        End Function

        Private Function addMoneyLine(ByVal psGameID As String, ByVal psTicketID As String, ByVal pnMoneyLine As Double, Optional ByVal psChoice As String = "home") As CCheckTicketBet
            Dim oTicketBet As New CCheckTicketBet(newGUID)
            oTicketBet.GameID = psGameID
            oTicketBet.TicketID = psTicketID
            oTicketBet.BetType = "MONEYLINE"

            Select Case UCase(psChoice)
                Case "HOME"
                    oTicketBet.HomeMoneyLine = pnMoneyLine
                Case "AWAY"
                    oTicketBet.AwayMoneyLine = pnMoneyLine
                Case "DRAW"
                    oTicketBet.BetType = "DRAW"
                    oTicketBet.DrawMoneyLine = pnMoneyLine
            End Select

            Return oTicketBet
        End Function

    End Class

End Namespace

