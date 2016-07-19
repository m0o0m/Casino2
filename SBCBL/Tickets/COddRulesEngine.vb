Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace Tickets
    Public Class COddRulesEngine
        ' Private _tblMain As DataTable
        Private _tblMainSBS As DataTable
        'Private _oOddsRules As List(Of COddsRule)
        Private _SuperAgent As String
        Private _template As CPlayerTemplate

        Public Sub New(ByVal plstConditions As List(Of String), _
                     ByVal psSuperAdminID As String, ByVal pbGameType As Boolean, ByVal poPlayerTemplate As CPlayerTemplate, ByVal psSuperAgent As String)
            ' _oOddsRules = plstOddsRules
            _SuperAgent = psSuperAgent
            Dim oTicketsManager As New CTicketManager
            '  _tblMain = oTicketsManager.GetRiskAmountsBySuperAdmin(plstConditions, psSuperAdminID, pbGameType)
            If Not String.IsNullOrEmpty(psSuperAgent) Then
                _tblMainSBS = oTicketsManager.GetRiskAmountsBySuperAgent(plstConditions, psSuperAgent, pbGameType)
            End If
            _template = poPlayerTemplate
        End Sub

        ''Public Sub New(ByVal plstOddsRules As List(Of COddsRule), ByVal plstConditions As List(Of String), _
        ''               ByVal psSuperAdminID As String, ByVal pbGameType As Boolean, ByVal poPlayerTemplate As CPlayerTemplate, ByVal psSuperAgent As String)
        ''    _oOddsRules = plstOddsRules
        ''    _SuperAgent = psSuperAgent
        ''    Dim oTicketsManager As New CTicketManager
        ''    _tblMain = oTicketsManager.GetRiskAmountsBySuperAdmin(plstConditions, psSuperAdminID, pbGameType)
        ''    If Not String.IsNullOrEmpty(psSuperAgent) Then
        ''        _tblMainSBS = oTicketsManager.GetRiskAmountsBySuperAgent(plstConditions, psSuperAgent, pbGameType)
        ''    End If
        ''    _template = poPlayerTemplate
        ''End Sub

        ''' <summary>
        ''' Check Lock Game
        ''' </summary>
        ''' <param name="psGameID"></param>
        ''' <param name="psTeam">Away(Over)|Home(Under)</param>
        ''' <param name="psContext">Current|1H|2H</param>
        ''' <param name="psBetType">MoneyLine|Spread|TotalPoints|Draw</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Function IsLockGame(ByVal psGameID As String, ByVal psTeam As String, ByVal psContext As String, ByVal psBetType As String, ByVal oTicketBet As CTicketBet, Optional ByVal psGameLineID As String = "") As Boolean
        '    Dim nDiffAmount As Double = 0
        '    Dim lstOddsRules As New List(Of COddsRule)
        '    '' Get difference amount
        '    nDiffAmount = GetDiffAmountSuperAgent(psGameID, psTeam, psContext, psBetType, oTicketBet.RiskAmount, oTicketBet.TeamTotalName)
        '    lstOddsRules = New CacheUtils.CCacheManager().GetGameOddRule(_SuperAgent)
        '    For Each oOddsRules As COddsRule In lstOddsRules
        '        '' Valid Condition
        '        If oOddsRules.IsOddRuleLocked AndAlso oOddsRules.GreaterThan <= nDiffAmount _
        '        AndAlso nDiffAmount <= oOddsRules.LowerThan Then
        '            If Not String.IsNullOrEmpty(psGameLineID) Then
        '                Dim oGameLineManager As New CGameLineManager()
        '                '' check exists gameline in manual game after that update lock game
        '                If Not oGameLineManager.CheckExistsGameLines(oTicketBet.GameID, SafeString(oTicketBet.GameDate), oTicketBet.GameType, oTicketBet.HomeTeam, oTicketBet.AwayTeam, oTicketBet.Context, _SuperAgent) Then
        '                    psGameLineID = oGameLineManager.InsertGameLineManual(psGameLineID, _SuperAgent)
        '                End If
        '                oGameLineManager.UpdateBlockGame(psGameLineID)
        '            End If
        '            Return True
        '        End If
        '    Next
        '    Return False
        'End Function

        Public Function Juice(ByVal psGameType As String, ByVal psContext As String) As Integer
            Dim oCache As New CacheUtils.CCacheManager()
            Dim nJuice As Integer = SafeInteger(oCache.GetJuiceControl(_SuperAgent, GetSportType(psGameType), psContext, psGameType))
            If nJuice = 0 Then
                nJuice = SafeInteger(oCache.GetJuiceControl(_SuperAgent, GetSportType(psGameType), psContext, String.Empty))
            End If
            Return nJuice
        End Function

        ''' <summary>
        ''' Return MoneyLine
        ''' </summary>
        ''' <param name="psGameID"></param>
        ''' <param name="psGameType"></param>
        ''' <param name="psTeam">Away(Over)|Home(Under)</param>
        ''' <param name="psContext">Current|1H|2H</param>
        ''' <param name="psBetType">MoneyLine|Spread|TotalPoints|Draw</param>
        ''' <param name="pnOdds">Odds for current line</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMoneyLine(ByVal psGameID As String, ByVal psGameType As String, _
                                     ByVal psTeam As String, ByVal psContext As String, _
                                     ByVal psBetType As String, ByVal pnOdds As Double, ByVal pbFavorite As Boolean, Optional ByVal psTeamTotalName As String = "", Optional ByVal pnIncreaseSpread As Integer = 0, Optional ByVal pbBuypoint As Boolean = False) As Double
            Dim nMoneyLine As Double = 0
            Dim nDiffAmount As Double = 0
            Dim nJuice As Double = 0
            ''add juice for super set  for sport type
            If pnOdds = 0 Then
                Return 0
            End If
            'ClientAlert(pnOdds & psContext & Juice(psGameType, psContext) & psBetType, True)
            If psContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) OrElse psContext.Equals("1H", StringComparison.CurrentCultureIgnoreCase) OrElse psContext.Equals("2H", StringComparison.CurrentCultureIgnoreCase) Then
                ' Original Code
                'pnOdds += Juice(psGameType, psContext)

                ' Trung Edit:
                ' new rule: not apply juice for Totals if Sport Type is BaseBall or Hockey
                Dim juiceVal As Integer = 0
                If Not ((IsBaseball(psGameType) OrElse IsHockey(psGameType)) AndAlso psBetType.Equals("TotalPoints", StringComparison.CurrentCultureIgnoreCase)) Then
                    juiceVal = Juice(psGameType, psContext)
                End If

                pnOdds += juiceVal
            End If
            'If psContext.Equals("2H", StringComparison.CurrentCultureIgnoreCase) AndAlso IsSoccer(psGameType) Then

            '    pnOdds += Juice(psGameType, psContext)
            'End If
            '   'ClientAlert(":" & pnOdds, True)
            ''check juice game for 1h,2h flat money + juice/2
            Dim oCache As New CacheUtils.CCacheManager()
            Dim nNumFixedSpreadMoneyH As Double = 0
            'If oCache.GetFixSpreadMoney(_SuperAgent) IsNot Nothing Then
            '    If psBetType.Equals("TeamTotalPoints", StringComparison.CurrentCultureIgnoreCase) Then
            '        nNumFixedSpreadMoneyH = oCache.GetSysSettings(_SuperAgent & "FixexdSpreadMoney").GetDoubleValue("HalfSpreadMoney") 'oCache.GetFixSpreadMoney(_SuperAgent).NumFixedSpreadMoneyToTalGame
            '    Else
            '        nNumFixedSpreadMoneyH = oCache.GetFixSpreadMoney(_SuperAgent & "FixexdSpreadMoney").NumFixedSpreadMoneyH
            '    End If
            'End If

            If psContext.Contains("H") And nNumFixedSpreadMoneyH <> 0 And Not psBetType.Equals("MoneyLine", StringComparison.CurrentCultureIgnoreCase) Then
                If (nNumFixedSpreadMoneyH > 0 And pnOdds < 0) OrElse (nNumFixedSpreadMoneyH < 0 And pnOdds > 0) Then
                    nMoneyLine = ((pnOdds + nNumFixedSpreadMoneyH) - 200) / 2
                Else
                    nMoneyLine = (pnOdds + nNumFixedSpreadMoneyH) / 2
                End If
                nMoneyLine = Math.Ceiling(nMoneyLine)
            Else
                If nNumFixedSpreadMoneyH <> 0 AndAlso psBetType.Equals("TeamTotalPoints", StringComparison.CurrentCultureIgnoreCase) Then
                    If (nNumFixedSpreadMoneyH > 0 And pnOdds < 0) OrElse (nNumFixedSpreadMoneyH < 0 And pnOdds > 0) Then
                        nMoneyLine = ((pnOdds + nNumFixedSpreadMoneyH) - 200) / 2
                    Else
                        nMoneyLine = (pnOdds + nNumFixedSpreadMoneyH) / 2
                    End If
                    nMoneyLine = Math.Ceiling(nMoneyLine)
                Else
                    nMoneyLine = pnOdds
                End If

            End If
            'ClientAlert(pnOdds & psContext & psBetType, True)
            '' Check Game is not Lock
            'If Not IsLockGame(psGameID, psTeam, psContext, psBetType) Then
            ''check juice game
            Dim oLOddsRules As New List(Of COddsRule)
            If Not String.IsNullOrEmpty(_SuperAgent) Then
                oLOddsRules = New CacheUtils.CCacheManager().GetGameOddRule(_SuperAgent)
            End If
            'Dim oFixSpreadMoney As CFixSpreadMoney = New CCacheManager().GetFixSpreadMoney(_SuperAgent)
            ''ClientAlert(pbBuypoint & ":" & nMoneyLine, True)
            'If Not pbBuypoint Then
            '    'nMoneyLine = oFixSpreadMoney.NumFlatSpreadMoney AndAlso nMoneyLine = oFixSpreadMoney.NumFixedSpreadMoneyH AndAlso nMoneyLine = oFixSpreadMoney.NumFixedSpreadMoneyToTalGame Then
            '    nMoneyLine = getFixSpreadMoney(psGameType, nMoneyLine, _SuperAgent, pnIncreaseSpread, psContext, psBetType)

            'End If
            ' Trung: add get flat juice setup
            nMoneyLine = getFixSpreadMoney(psGameType, nMoneyLine, _SuperAgent, pnIncreaseSpread, psContext, psBetType)
            ' ClientAlert(pbBuypoint & ":" & nMoneyLine, True)
            'ClientAlert(pnOdds & psContext & Juice(psGameType, psContext) & psBetType, True)
            If Not psGameType.Equals("Other", StringComparison.CurrentCultureIgnoreCase) AndAlso Not psBetType.Equals("Draw", StringComparison.CurrentCultureIgnoreCase) Then
                If pbFavorite Then
                    nJuice = _template.PlayerJuice.GetFavConfig(psGameType, psContext & psBetType)
                Else
                    nJuice = _template.PlayerJuice.GetUndConfig(psGameType, psContext & psBetType)
                End If
                If nMoneyLine <> 0 Then
                    nMoneyLine = nMoneyLine + nJuice 'CalValue(nMoneyLine, SafeInteger(nJuice))
                End If

            End If
            '' Get difference amount
            ''0 because  load only show not bet not 
            nDiffAmount = GetDiffAmountSuperAgent(psGameID, psTeam, psContext, psBetType, 0, psTeamTotalName)
            ''increase spread bet for each player
            For Each oOddsRules As COddsRule In oLOddsRules
                '' Valid Condition
                If oOddsRules.GreaterThan <= nDiffAmount AndAlso nDiffAmount <= oOddsRules.LowerThan Then
                    If nDiffAmount > 0 Then '' Increase MoneyLine
                        'If nMoneyLine > 0 AndAlso nMoneyLine < 100 Then
                        '    nMoneyLine = nMoneyLine - 200
                        'End If
                        'nMoneyLine = CalIncrease(nMoneyLine, SafeInteger(oOddsRules.Increase))
                        nMoneyLine = nMoneyLine - oOddsRules.Increase
                    End If
                    Exit For
                End If
            Next
            If nMoneyLine > 0 AndAlso nMoneyLine < 100 Then
                nMoneyLine = nMoneyLine - 200
            End If
            If nMoneyLine < 0 AndAlso nMoneyLine > -100 Then
                nMoneyLine = nMoneyLine + 200
            End If
            ' End If
            ' 'ClientAlert(nMoneyLine & ":" & pnOdds, True)
            Return nMoneyLine
        End Function


        Public Function getFixSpreadMoney(ByVal psGameType As String, ByVal psDefaultMoney As Double, ByVal pSuperAgentId As String, ByVal pnIncreaseSpread As Integer, ByVal psContext As String, ByVal psBetType As String) As Double
            'Dim oFixSpreadMoney As CFixSpreadMoney = New CCacheManager().GetFixSpreadMoney(pSuperAgentId)
            Dim oCache = New CCacheManager()
            Dim flatJuiceKey = pSuperAgentId & "FixexdSpreadMoney"
            Dim flatSettings = oCache.GetSysSettings(flatJuiceKey)
            ' return default
            If flatSettings Is Nothing OrElse flatSettings.Count = 0 Then
                Return psDefaultMoney
            End If

            ''ClientAlert(psGameType & oFixSpreadMoney.UseFlatSpreadMoney, True)Preseason
            ' OLD logic
            ' (UCase(psGameType).Contains("FOOTBALL") OrElse UCase(psGameType).Contains("BASKETBALL") OrElse UCase(psGameType).Contains("PRESEASON")) _
            If (IsFootball(psGameType) OrElse IsBasketball(psGameType) OrElse UCase(psGameType).Contains("PRESEASON")) _
             AndAlso psContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) _
             AndAlso (psBetType.Equals("Spread", StringComparison.CurrentCultureIgnoreCase) OrElse psBetType.Equals("TotalPoints", StringComparison.CurrentCultureIgnoreCase)) Then
                If flatSettings.GetBooleanValue("IsFlatSpreadMoney") Then
                    Dim nNumFlatSpreadMoney As Single = flatSettings.GetIntegerValue("FlatSpreadMoney")
                    '  Return CalValue(nNumFlatSpreadMoney, pnIncreaseSpread)
                    'ClientAlert("" & nNumFlatSpreadMoney, True)
                    Return nNumFlatSpreadMoney
                Else
                    'ClientAlert("default ko", True)
                    Return psDefaultMoney 'CalValue(psDefaultMoney, pnIncreaseSpread)
                End If
            End If
            'ClientAlert("default2", True)
            Return psDefaultMoney
        End Function

        Public Function GetDiffAmountSuperAgent(ByVal psGameID As String, ByVal psTeam As String, ByVal psContext As String, ByVal psBetType As String, ByVal pnRiskAmount As Double, ByVal psTeamTotalName As String) As Double
            Dim nDiffAmount As Double = 0
            Dim sWhere As String = String.Format("GameID = {0} AND Context = {1} AND BetType = {2}", _
                                                 SQLString(psGameID), SQLString(psContext), SQLString(psBetType))

            Select Case UCase(psBetType)
                Case "MONEYLINE"
                    sWhere &= " AND {0}MoneyLine <> 0"
                Case "SPREAD"
                    sWhere &= " AND {0}SpreadMoney <> 0"
                Case "TOTALPOINTS"
                    sWhere &= " AND TotalPoints{0}Money <> 0"
                Case "TEAMTOTALPOINTS"
                    sWhere &= " AND TotalPoints{0}Money <> 0"
                Case Else ''DRAW
                    sWhere &= " AND DrawMoneyLine <> 0"

            End Select

            If UCase(psBetType) <> "DRAW" Then
                If (UCase(psBetType) <> "TOTALPOINTS" And UCase(psBetType) <> "TEAMTOTALPOINTS") Then '' MONEYLINE | SPREAD
                    nDiffAmount = SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere, "Away")))
                    nDiffAmount -= SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere, "Home")))

                ElseIf UCase(psBetType) = "TOTALPOINTS" Then
                    nDiffAmount = SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere, "Over")))
                    nDiffAmount -= SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere, "Under")))

                ElseIf UCase(psBetType) = "TEAMTOTALPOINTS" Then
                    If psTeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                        nDiffAmount = SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere & " And TeamTotalName='Away'", "Over")))
                        nDiffAmount -= SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere & " And TeamTotalName='Away'", "Under")))
                    Else
                        nDiffAmount = SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere & " And TeamTotalName='Home'", "Over")))
                        nDiffAmount -= SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", String.Format(sWhere & " And TeamTotalName='Home'", "Under")))
                    End If

                End If

                If UCase(psTeam) <> "AWAY" Then '' HOME
                    nDiffAmount = -nDiffAmount

                End If

            Else '' DRAW
                nDiffAmount = SafeDouble(_tblMainSBS.Compute("SUM(RiskAmount)", sWhere))

            End If
            nDiffAmount += pnRiskAmount
            Return nDiffAmount
        End Function

    End Class
End Namespace
