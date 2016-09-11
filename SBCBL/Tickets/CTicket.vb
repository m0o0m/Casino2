Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.UI
Imports SBCBL.Managers

Namespace Tickets
    <Serializable()> _
    Public Class CTicket
        Implements IComparable(Of CTicket)

        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CTicket))

        Private _sTicketID As String
        Private _sTicketType As String 'Should be: Straight, Parlay, Action Reverse, Win Reverse, Teaser
        Private _nRiskAmount As Double
        Private _nWinAmount As Double
        Private _nWinAmountInit As Double
        Private _nRiskAmountInit As Double
        Private _olstTicketBets As CTicketBetList
        Private _sInvalidBets As String
        Private _nBetAmount As Double
        Private _sTicketOption As String
        Private _olstsTicketRoundRobinOption As List(Of Integer)
        Private _sSuperAgentID As String
        Private _sPlayerID As String
        Public RelatedTicketID As String
        Public TicketNumber As Integer
        Public SubTicketNumber As Integer

        '' Dont save to DB
        Public LastIndex As Integer
        Public TicketOptionText As String
        Public IsForProp As Boolean = False
        Public OriginalBetAmount As Double

        Public _sMaxSelection As String = "MaxSelection"
        Private _sParlayType As String = SBCBL.std.GetSiteType & " ParlayType"
        Private _sReverseType As String = SBCBL.std.GetSiteType & " ReverseType"
        Private _sParlayRules As String = SBCBL.std.GetSiteType & " ParlayRules"
        Private _sBWParlayRules As String = SBCBL.std.GetSiteType & " BWParlayRules"
        Private CATEGORY_CURRENT As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_CURRENT"
        Private CATEGORY_TIGERSB As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_TIGERSB"

        Public Sub New(ByVal psTicketType As String, ByVal psSuperAgentID As String, ByVal psPlayerID As String)
            _sTicketID = newGUID()
            If LCase(psTicketType).Contains("if ") Then
                _sTicketType = "If Bet"
                _sTicketOption = psTicketType
            Else
                _sTicketType = psTicketType
            End If

            _olstTicketBets = New CTicketBetList()
            _sSuperAgentID = psSuperAgentID
            _sPlayerID = psPlayerID
        End Sub

        Public Sub New(ByVal poTicket As CTicket)
            _sTicketID = newGUID()
            _sTicketType = poTicket.TicketType
            _nBetAmount = poTicket.BetAmount
            _nRiskAmount = poTicket.RiskAmount
            _nWinAmount = poTicket.WinAmount
            _sInvalidBets = poTicket.InvalidBets
            _sTicketOption = poTicket.TicketOption
            _sSuperAgentID = poTicket.SuperAgentID
            LastIndex = poTicket.LastIndex
            RelatedTicketID = poTicket.RelatedTicketID
            TicketOptionText = poTicket.TicketOptionText
            IsForProp = poTicket.IsForProp
            _sPlayerID = poTicket.PlayerID
            _olstTicketBets = New CTicketBetList()
            For Each oTicketBet As CTicketBet In poTicket.TicketBets
                oTicketBet.RiskAmount = RiskAmount
                _olstTicketBets.Add(New CTicketBet(oTicketBet))
            Next
        End Sub

#Region "Properties"
        Public ReadOnly Property PlayerID() As String
            Get
                Return _sPlayerID
            End Get
        End Property

        Public ReadOnly Property IsCircle() As Boolean
            Get
                For Each oTB As CTicketBet In _olstTicketBets
                    If oTB.IsCircled Then
                        Return True
                    End If
                Next
            End Get
        End Property

        Public ReadOnly Property TicketID() As String
            Get
                Return _sTicketID
            End Get
        End Property

        Public ReadOnly Property TicketType() As String
            Get
                Return _sTicketType
            End Get
        End Property

        Public ReadOnly Property RiskAmount() As Double
            Get
                Return _nRiskAmount
            End Get
        End Property

        Public ReadOnly Property WinAmount() As Double
            Get
                Return _nWinAmount
            End Get
        End Property

        Public ReadOnly Property TicketBets() As CTicketBet()
            Get
                Return _olstTicketBets.ToArray()
            End Get
        End Property

        Public ReadOnly Property InvalidBets() As String
            Get
                Return SafeString(_sInvalidBets)
            End Get
        End Property

        Public ReadOnly Property SetWinAmount() As Boolean
            Get
                ' LogDebug(_log, "thuong" & _olstTicketBets(0).SetWinAmount & "--" & _olstTicketBets.Count)
                Return SafeBoolean(_olstTicketBets(0).SetWinAmount)
            End Get
        End Property

        Public Property BetAmount() As Double
            Get
                Return _nBetAmount
            End Get
            Set(ByVal value As Double)
                If _nBetAmount <> value Then
                    _nBetAmount = value
                    ReCalcAmount()
                End If
            End Set
        End Property

        Public Property TicketOption() As String
            Get
                Return _sTicketOption
            End Get
            Set(ByVal value As String)
                If _sTicketOption <> value Then
                    _sTicketOption = value
                    ReCalcAmount()
                End If
            End Set
        End Property

        Public Property TicketRoundRobinOption() As List(Of Integer)
            Get
                Return _olstsTicketRoundRobinOption
            End Get
            Set(ByVal value As List(Of Integer))
                _olstsTicketRoundRobinOption = value
            End Set
        End Property

        Public ReadOnly Property NumOfTicketBets() As Integer
            Get
                Return _olstTicketBets.Count
            End Get
        End Property

        Public ReadOnly Property SuperAgentID() As String
            Get
                Return _sSuperAgentID
            End Get
        End Property

        Public ReadOnly Property HTML() As String
            Get
                Dim sHTML, sCalcAmount, sOptions As String
                sHTML = String.Format("<table border='0' cellspacing='1' cellpadding='0' width='98%' align='center' style='margin-bottom: 10px'>" & _
                                          "<tr><td colspan='5' align='left'><span class='org_txt'>{0}</span><span class='blue_txt'> Wager</span></td>" & _
                                          "</tr><tr class='tableheading'><td width='24'>NSS</td><td width='*'>Team</td><td width='25'></td>" & _
                                          "<td width='30'>Line</td><td width='20'></td></tr>", Me.TicketType)
                sCalcAmount = ""
                sOptions = ""

                If Me.NumOfTicketBets > 0 Then
                    Dim bCheckBuypoint As Boolean

                    For Each oTicketBet As CTicketBet In Me._olstTicketBets
                        Select Case UCase(Me.TicketType)
                            Case "PARLAY"
                                bCheckBuypoint = True
                                sCalcAmount = String.Format("Bet: <input name='txtBet{0}' id='txtBet{0}' type='text' value='{1}' maxlength='10' class='textInput' " & _
                                                            "onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:SaveAmount(this, \""{0}\"");' " & _
                                                            "TicketID='{0}' style='width:50px;text-align: right; padding-left: 2px;' /> " & _
                                                            "Risk/Win: <span id='lblResult{0}' name='lblResult{0}' >{2}/{3}</span>", _
                                                            Me.TicketID, SafeString(Me.BetAmount), SafeString(Me.RiskAmount), SafeString(Me.WinAmount))

                            Case "STRAIGHT", "IF BET"
                                bCheckBuypoint = True
                                sCalcAmount = String.Format("Risk: <input name='txtBet{0}' id='txtBet{0}' type='text' value='{1}' maxlength='10' class='textInput' " & _
                                                            "onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:SaveAmount(this, \""{0}\"");' TicketID='{0}' " & _
                                                            "style='width:50px;text-align: right; padding-left: 2px;' /> Win: " & _
                                                            "<input name='txtWin{0}' id='txtWin{0}' type='text' value='{2}' maxlenght='10'  class='textInput' " & _
                                                            "class='inputText' tabindex='-1' onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:CalcRiskStraight(this, document.getElementById(\""ddlBuyPoint{3}\""), \""{0}\"", " & _
                                                            "document.getElementById(\""txtBet{0}\""));' TicketID='{0}' " & _
                                                            "style='width:50px;text-align: right; padding-left: 2px;' />", _
                                                            Me.TicketID, SafeString(Me.RiskAmount), _
                                                            SafeString(Math.Round(Me.WinAmount)), Me.TicketBets(0).TicketBetID)
                                ' Straight wager only have 1 ticketbet so we can get Me.TicketBets(0).TicketBetID without any logical problem

                            Case "REVERSE"
                                bCheckBuypoint = True
                                sCalcAmount = String.Format("Bet: <input name='txtBet{0}' id='txtBet{0}' type='text' value='{1}' maxlength='10' class='textInput' " & _
                                                            "onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:SaveAmount(this, \""{0}\"");' " & _
                                                            "TicketID='{0}' style='width:50px;text-align: right; padding-left: 2px;' /> Win: " & _
                                                            "<input name='txtWin{0}' id='txtWin{0}' type='text' value='{2}' maxlenght='10'  class='textInput' " & _
                                                            "class='inputText' tabindex='-1' onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:CalcBetReverse(this, \""{0}\"", " & _
                                                            "document.getElementById(\""txtBet{0}\""));' TicketID='{0}' " & _
                                                            "style='width:50px;text-align: right; padding-left: 2px;' />", _
                                                            Me.TicketID, SafeString(Me.BetAmount), _
                                                            SafeString(Me.WinAmount))

                            Case Else 'Teaser
                                bCheckBuypoint = False
                                sCalcAmount = String.Format("Risk: <input name='txtBet{0}' id='txtBet{0}' type='text' value='{1}' maxlength='10' class='textInput' " & _
                                                            "onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:SaveAmount(this, \""{0}\"");' TicketID='{0}' " & _
                                                            "style='width:50px;text-align: right; padding-left: 2px;' /> Win: " & _
                                                            "<input name='txtWin{0}' id='txtWin{0}' type='text' value='{2}' maxlenght='10'  class='textInput' " & _
                                                            "class='inputText' tabindex='-1' onkeypress='javascript:return inputNumber(this,event, false);' " & _
                                                            "onblur='javascript:CalcRiskTeaser(this, \""{0}\"", document.getElementById(\""ddlType{0}\""), " & _
                                                            "document.getElementById(\""txtBet{0}\""));' TicketID='{0}' " & _
                                                            "style='width:50px;text-align: right; padding-left: 2px;' />", _
                                                            Me.TicketID, SafeString(Me.RiskAmount), SafeString(Me.WinAmount))

                        End Select
                        sHTML &= oTicketBet.HTML(bCheckBuypoint)
                    Next

                    '' Get List of Ticket Options
                    Dim olstTicketOptions As List(Of DictionaryEntry) = Me.ParseOptions
                    If olstTicketOptions.Count > 0 Then ' Straight wager don't have TicketOption
                        sOptions = String.Format("<select name='ddlType{0}' id='ddlType{0}' class='textInput' " & _
                                                 "onblur='javascript: TicketOption(this, \""{0}\"");'>", Me.TicketID)

                        If Me._sTicketOption = "" Then ' Dafault is fisrt item
                            Me._sTicketOption = SafeString(olstTicketOptions.Item(0).Key)
                        End If

                        For Each oOption As DictionaryEntry In olstTicketOptions
                            If Me._sTicketOption = SafeString(oOption.Key) Then
                                sOptions &= String.Format("<option selected='selected' value='{0}'>{1}</option>", _
                                                          SafeString(oOption.Key), SafeString(oOption.Value))
                            Else
                                sOptions &= String.Format("<option value='{0}'>{1}</option>", _
                                                          SafeString(oOption.Key), SafeString(oOption.Value))
                            End If
                        Next

                        sOptions &= "</select>"
                    End If

                    sHTML &= String.Format("<tr><td></td><td colspan='4'><div style='border-top: 1px solid #ccc;'</div></td></tr>" & _
                                           "<tr><td colspan='5' algin='right' nowrap='nowrap'><div style='float: right;'>{0} {1}</div></td></tr></table>", sOptions, sCalcAmount)
                Else
                    sHTML &= "</table>"
                End If

                Return sHTML
            End Get
        End Property

        Public ReadOnly Property PreviewHTML() As String
            Get
                Dim sPreviewHTML As String = ""

                If Me.NumOfTicketBets > 0 Then
                    sPreviewHTML = "<table width='100%'><tr><td width='20px' nowrap='nowrap'>Wager type:</td><td><b>"

                    Select Case UCase(Me.TicketType)
                        Case "PARLAY"
                            sPreviewHTML &= Me.TicketOptionText

                        Case "REVERSE"
                            sPreviewHTML &= Me.TicketOption

                        Case "TEASER"
                            Dim oCache As New CacheUtils.CCacheManager()
                            sPreviewHTML &= oCache.GetTeaserRuleInfo(Me.TicketOption).TeaserRuleName

                        Case Else
                            sPreviewHTML &= Me.TicketType

                    End Select

                    sPreviewHTML &= "</b></td></tr>"

                    Dim nIndex As Integer = 1
                    For Each oTicketBet As CTicketBet In Me._olstTicketBets

                        sPreviewHTML &= oTicketBet.PreviewHTML(nIndex)
                        nIndex += 1
                    Next

                    sPreviewHTML &= String.Format("</td></tr><tr><td>Amount:</td><td> Risking <b>{0}</b> To Win <b>{1}</b></td></tr></table>", _
                                                  FormatNumber(SafeRound(Me.RiskAmount), GetRoundMidPoint), _
                                                  FormatNumber(SafeRound(Me.WinAmount), GetRoundMidPoint))
                End If

                Return sPreviewHTML
            End Get
        End Property

#End Region

#Region "Methods"
        Public Function GetTicketBet(ByVal psTicketBetID As String) As CTicketBet
            Return _olstTicketBets.GetTicketBetByID(psTicketBetID)
        End Function

        Private Function CalcRate(ByVal pnRate As Double) As Double
            If pnRate < 0 Then
                pnRate = (-pnRate + 100) / -pnRate
            ElseIf pnRate > 0 Then
                pnRate = (pnRate + 100) / 100
            Else
                pnRate = 0
            End If

            Return pnRate
        End Function

        Public Function Validate(ByVal poOddsRules As COddRulesEngine, ByVal poTblPlayerBet As DataTable, ByVal poPlayerTemplate As CPlayerTemplate, _
                                 ByVal pnRemainAmount As Double, ByVal pbTypeOfBet As CEnums.ETypeOfBet, ByVal pbSuperPlayer As Boolean, ByVal pnIncreaseSpread As Integer) As ECheckStatus

            Dim bCheckRiskAmount As Boolean = False
            _sInvalidBets = ""
            Dim sError As String = ""
            Dim oLstGameType As New List(Of String)
            Dim oLstGameContext As New List(Of String)
            Dim oLstGameIDContext As New List(Of String())
            'Dim nRiskAmount As Double
            Dim nMaxperGame As Double = 500
            Dim strSportType24H As String = ""
            _log.Debug("BEGIN check valid TicketID: " & _sTicketID)

            '' Check enough amount to place bet
            If pnRemainAmount < Me.RiskAmount Then
                sError = "You have exceeded your credit limit" & vbCrLf
                _log.Debug("Not enough amount.")
            End If

            If UCase(TicketType) <> "STRAIGHT" AndAlso UCase(TicketType) <> "IF BET" Then
                If _olstTicketBets.Count < 2 Then
                    sError &= String.Format("Your minimum selection allowed is 2 for {0} wager.", TicketType) & vbCrLf
                    _log.Debug(String.Format("Your minimum selection allowed is 2 for {0} wager.", TicketType))
                End If

                If UCase(TicketType) = "TEASER" Then
                    If SafeString(Me._sTicketOption) = "" Then
                        sError &= "Please choose TeaserRule for Teaser wager." & vbCrLf
                        _log.Debug("Teaser Wager doesn't have TeaserRule.")
                    Else
                        Dim oCache As New CCacheManager()
                        Dim oTeaserRule As CTeaserRule = oCache.GetTeaserRuleInfo(Me._sTicketOption)

                        If oTeaserRule IsNot Nothing Then
                            If _olstTicketBets.Count < oTeaserRule.MinTeam Then
                                sError &= String.Format("Your minimum selection allowed is {0} for {1} TeaserRule.", oTeaserRule.MinTeam, oTeaserRule.TeaserRuleName) & vbCrLf
                            End If

                            If _olstTicketBets.Count > oTeaserRule.MaxTeam Then
                                sError &= String.Format("Your maximum selection allowed is {0} for {1} TeaserRule.", oTeaserRule.MaxTeam, oTeaserRule.TeaserRuleName) & vbCrLf
                            End If
                        End If
                    End If
                End If
            End If

            If sError <> "" Then
                Throw New CTicketException(sError)
            End If

            Dim oResult As ECheckStatus = ECheckStatus.Success

            Dim oCacheMng As New CCacheManager()
            'Dim oGameHalfTimeDisplayList As CGameHalfTimeDisplayList = oCacheMng.GetGameHalfTimeDisplay(SuperAgentID)
            Dim bGame24H As Boolean = False
            For Each oTicketBet As CTicketBet In _olstTicketBets
                strSportType24H = GetSportType(oTicketBet.GameType)
                If oCacheMng.GetSysSettings("MAXPERGAME_24H_TIMER" & SuperAgentID) IsNot Nothing And oCacheMng.GetSysSettings("MAXPERGAME_24H_TIMER" & SuperAgentID).GetDoubleValue(strSportType24H) > 0 Then
                    If oTicketBet.GameDate.Subtract(GetEasternDate).TotalMinutes > (oCacheMng.GetSysSettings("MAXPERGAME_24H_TIMER" & SuperAgentID).GetDoubleValue(strSportType24H) * 60) Then
                        ' LogDebug(_log, GetEasternDate() & "-" & oTicketBet.GameDate & "thuong 24h:" & oTicketBet.GameDate.Subtract(GetEasternDate).TotalMinutes)
                        bGame24H = True
                        'Dim oSysManager As New CSysSettingManager()
                        'Dim odr As DataRow = oSysManager.GetValue("MAXPERGAME_24H" & SuperAgentID, "", strSportType24H)
                        'Dim odrTimer As DataRow = oSysManager.GetValue("MAXPERGAME_24H_TIMER" & SuperAgentID, "", strSportType24H)
                        '  LogDebug(_log, strSportType24H & "_24h_" & odr("Value").ToString())
                        'If odr IsNot Nothing Then
                        '    If SafeDouble(odr("Value").ToString()) > 0 AndAlso nMaxperGame > SafeDouble(odr("Value").ToString()) Then
                        '        nMaxperGame = SafeDouble(odr("Value").ToString())
                        '        'LogDebug(_log, "nMaxperGame 1 :" & nMaxperGame)
                        '    End If
                        '    'Else
                        '    '    nMaxperGame = 500

                        If oCacheMng.GetSysSettings("MAXPERGAME_24H" & SuperAgentID) IsNot Nothing Then
                            If SafeDouble(oCacheMng.GetSysSettings("MAXPERGAME_24H" & SuperAgentID).GetValue(strSportType24H)) > 0 AndAlso nMaxperGame > (SafeDouble(oCacheMng.GetSysSettings("MAXPERGAME_24H" & SuperAgentID).GetValue(strSportType24H))) Then
                                nMaxperGame = SafeDouble(oCacheMng.GetSysSettings("MAXPERGAME_24H" & SuperAgentID).GetValue(strSportType24H))
                                'LogDebug(_log, "nMaxperGame 1 :" & nMaxperGame)
                            End If

                        End If
                    End If
                End If
                If oTicketBet.AwaySpreadMoney > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.HomeSpreadMoney > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.TotalPointsOverMoney > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.TotalPointsUnderMoney > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.HomeMoneyLine > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.AwayMoneyLine > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.DrawMoneyLine > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.TotalPointsOverMoney > 0 Then
                    bCheckRiskAmount = True
                End If
                If oTicketBet.TotalPointsUnderMoney > 0 Then
                    bCheckRiskAmount = True
                End If

                oLstGameIDContext.Add(New String() {oTicketBet.GameID, oTicketBet.Context, oTicketBet.BetType})
                ''quarter off:turn off quarter line after a period time that set by superagent, applies to football, basketball
                Dim sSportType As String = ""
                Dim nOffQuarter As Integer = 0
                If IsBasketball(oTicketBet.GameType) Then
                    sSportType = "Basketball"
                End If
                If IsFootball(oTicketBet.GameType) Then
                    sSportType = "Football"
                End If
                If Not String.IsNullOrEmpty(sSportType) Then
                    'Dim oGameQuarterDisplayList As CGameTeamDisplayList = oCacheMng.GetGameTeamDisplay(SuperAgentID, sSportType)
                    'Dim oGameQuarterDisplay As CGameTeamDisplay = oGameQuarterDisplayList.GetGameTeamDisplay(sSportType)
                    'If oGameQuarterDisplay IsNot Nothing Then
                    '    nOffQuarter = oGameQuarterDisplay.GameTimeOffLine
                    'End If
                    If oCacheMng.GetSysSettings("") IsNot Nothing Then
                        nOffQuarter = oCacheMng.GetSysSettings(SuperAgentID & "LineOffHour", sSportType).GetIntegerValue("OffMinutes")
                    End If
                End If

                '' 2H line off: turn off 2H line after a period time that set by super, applies to football, basketball, soccer
                ' Dim oSysSettingManager As New CSysSettingManager()

                Dim nSecondHalfTimeOff As Integer = New SBCBL.UI.CSBCSession().SecondHalfTimeOff(oTicketBet.GameType)
                'Dim nSecondHalfTimeOff As Integer = oGameHalfTimeDisplayList.GetGameHalfTimeDisplayByGameType(oTicketBet.GameType)
                LogDebug(_log, "thuong :" & oTicketBet.AddPoint & " :AddPointMoney " & oTicketBet.AddPointMoney)
                Dim oStatus As ECheckStatus = oTicketBet.Validate(poOddsRules, pbSuperPlayer, nSecondHalfTimeOff, pnIncreaseSpread, RiskAmount, nOffQuarter)

                oTicketBet.IsUpdate = oStatus = ECheckStatus.Update

                If oStatus = ECheckStatus.Update Then
                    Me.ReCalcAmount()
                End If
                If oResult < oStatus Then
                    oResult = oStatus
                End If
                'If Not oLstGameType.Contains(oTicketBet.GameType) Then
                oLstGameType.Add(oTicketBet.GameType)
                'End If

                'Dim ss As String = oTicketBet.Context + "','" + oTicketBet.BetType

                ' End If
                Dim sContext As String
                If oTicketBet.BetType.Equals("Spread") Then
                    sContext = oTicketBet.Context
                Else
                    sContext = oTicketBet.BetType
                End If
                If Not oLstGameContext.Contains(sContext) Then
                    oLstGameContext.Add(sContext)
                End If
            Next

            '' fixbug: validate bet amount for "Reverse"
            Dim validateBetAmount As Double = BetAmount
            If TicketType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase) OrElse TicketType.Equals("Action Reverse", StringComparison.CurrentCultureIgnoreCase) Then
                validateBetAmount = OriginalBetAmount
            End If

            '' Check limit betting(setup by superadmin)
            Dim nMaxBetAmount As Double
            If validateBetAmount < getMinBetAmount(poPlayerTemplate, pbTypeOfBet) Then
                sError &= String.Format("Minimum {0} bet is {1}", TicketType, getMinBetAmount(poPlayerTemplate, pbTypeOfBet)) & vbCrLf
                _log.Debug(String.Format("Bet Amount less than {0}.", getMinBetAmount(poPlayerTemplate, pbTypeOfBet)))
            End If

            '' Check Max Win per Game. Only Apply for Straight Wager 
            If UCase(TicketType) = "STRAIGHT" OrElse UCase(TicketType) = "IF BET" Then
                'Dim lstPlayerTemplateLimit As List(Of CPlayerTemplateLimit) = (New CacheUtils.CCacheManager()).GetBetLimitJuice(PlayerID)
                'Dim nMaxStraght As Double = lstPlayerTemplateLimit.FindAll(Function(x) x.Context = "" AndAlso x.Context)
                ' If (WinAmount + getTotalWinBetting(poTblPlayerBet)) > getMaxBetAmount(oLstGameContext, oLstGameType) Then
                If bCheckRiskAmount Then
                    ' _log.Debug(WinAmount & "--" & SafeString(bCheckRiskAmount.ToString()) & "=" & SafeString(RiskAmount.ToString()) & "-" & getTotalRiskBetting(poTblPlayerBet) & "-" & getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate) & "You are exceeding the maximum allowed for this game.")
                    If bGame24H Then
                        '  LogDebug(_log, "nMaxperGame" & nMaxperGame)
                        If (RiskAmount + getTotalRiskBetting(poTblPlayerBet)) > nMaxperGame Then
                            sError &= "You are exceeding the maximum limit for this game." & vbCrLf & nMaxperGame
                            _log.Debug("You are exceeding the maximum allowed for this game." & vbCrLf & nMaxperGame)
                        End If
                    Else
                        If (RiskAmount + getTotalRiskBetting(poTblPlayerBet)) > getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate) Then
                            sError &= "You are exceeding the maximum limit for this game." & vbCrLf & getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate)
                            _log.Debug("You are exceeding the maximum allowed for this game." & getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate))
                        End If
                    End If

                Else
                    If bGame24H Then
                        LogDebug(_log, "nMaxperGame" & nMaxperGame)
                        If (WinAmount + getTotalWinBetting(poTblPlayerBet)) > nMaxperGame Then
                            sError &= "You are exceeding the maximum limit for this game." & vbCrLf & nMaxperGame
                            _log.Debug("You are exceeding the maximum allowed for this game." & nMaxperGame)
                        End If
                    Else
                        If (WinAmount + getTotalWinBetting(poTblPlayerBet)) > getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate) Then
                            sError &= "You are exceeding the maximum limit for this game." & vbCrLf & getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate)
                            _log.Debug("You are exceeding the maximum allowed for this game." & getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate))
                        End If
                    End If

                End If
            Else

                If bGame24H Then
                    nMaxBetAmount = nMaxperGame
                Else
                    nMaxBetAmount = getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate)
                End If

                '' fix bug validate bet amount for "Reverse"
                If validateBetAmount > nMaxBetAmount Then ' BetAmount > nMaxBetAmount
                    sError &= String.Format("Maximum {0} bet is {1}", TicketType, nMaxBetAmount) & vbCrLf
                    _log.Debug(String.Format("Bet Amount greater than {0}.", nMaxBetAmount))
                End If
                If UCase(TicketType) = "PARLAY" AndAlso UCase(_sTicketOption) <> "PARLAY" Then
                    If sError <> "" Then
                        Throw New CTicketException(sError)
                    Else
                        Return oResult
                    End If

                End If

                ' Trung: Remove unnecessary validate amount for "Reverse"
                ''nMaxBetAmount = getMaxBetAmount(oLstGameContext, oLstGameType)
                'Dim nBetAmount As Double
                'If TicketType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase) OrElse TicketType.Equals("Action Reverse", StringComparison.CurrentCultureIgnoreCase) Then
                '    ' nBetAmount = RoundRiskReverse(BetAmount)
                '    'Else
                '    nBetAmount = BetAmount
                'End If
                'If nBetAmount > nMaxBetAmount Then
                '    sError &= String.Format("Maximum {0} bet is {1}", TicketType, nMaxBetAmount) & vbCrLf
                '    _log.Debug(String.Format("Bet Amount greater than {0}.", nMaxBetAmount))
                'End If

            End If
            ''check riskAmount for parlay,reverse,teaser
            If UCase(TicketType) <> "STRAIGHT" AndAlso UCase(TicketType) <> "IF BET" Then
                If UCase(TicketType) = "PARLAY" AndAlso UCase(_sTicketOption) <> "PARLAY" Then
                    If sError <> "" Then
                        Throw New CTicketException(sError)
                    Else
                        Return oResult
                    End If

                End If
                If sError = "" Then
                    'Dim oTicketManager As New CTicketManager()
                    'nRiskAmount = oTicketManager.GetRiskAmountsByPlayerID(oLstGameIDContext, PlayerID, TicketType.Replace("Reverse", "Action Reverse"))
                    ' '' if reverse riskamount = riskamount/2
                    'If TicketType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase) OrElse TicketType.Equals("Action Reverse", StringComparison.CurrentCultureIgnoreCase) Then
                    '    nRiskAmount += RoundRiskReverse(BetAmount)
                    'Else
                    '    nRiskAmount += RiskAmount
                    'End If
                    'Dim nMaxBetAmount As Double = getMaxBetAmount(oLstGameContext, oLstGameType, poPlayerTemplate)
                    If validateBetAmount > nMaxBetAmount Then ' nRiskAmount > nMaxBetAmount
                        sError = "You are exceeding the maximum allowed for this game." & nMaxBetAmount
                    End If
                End If
            End If

            If sError <> "" Then
                Throw New CTicketException(sError)
            End If

            _log.Debug("END check valid TicketID: " & _sTicketID)

            Return oResult
        End Function

        Private Function RoundRiskReverse(ByVal pnBetAmount As Double) As Double
            Dim nChargeBetAmount As Double = pnBetAmount Mod 100
            Dim nRiskAmountRever As Double = SafeDouble(IIf(nChargeBetAmount >= 50, (pnBetAmount - nChargeBetAmount) + 100, (pnBetAmount - nChargeBetAmount)))
            Return nRiskAmountRever
        End Function

        'Private Function getTotalRiskBetting(ByVal poTblPlayerBet As DataTable) As Double
        '    If _olstTicketBets.Count = 0 OrElse poTblPlayerBet Is Nothing OrElse poTblPlayerBet.Rows.Count = 0 Then
        '        Return 0
        '    End If
        '    Dim sWhere As String = ""

        '    If _olstTicketBets(0).IsForProp Then
        '        sWhere = String.Format("GameLineID = {0} AND Context = {1} AND BetType = {2}", _
        '                                   SQLString(_olstTicketBets(0).GameLineID), _
        '                                   SQLString(_olstTicketBets(0).Context), _
        '                                   SQLString(_olstTicketBets(0).BetType))
        '    Else
        '        If _olstTicketBets(0).Context.Contains("Q") Then
        '            sWhere = String.Format("GameID = {0} AND Context in ('1Q','2Q','3Q','4Q') AND BetType = {1}", _
        '                                                    SQLString(_olstTicketBets(0).GameID), _
        '                                                    SQLString(_olstTicketBets(0).BetType))
        '        Else
        '            sWhere = String.Format("GameID = {0} AND Context = {1} AND BetType = {2}", _
        '                                                       SQLString(_olstTicketBets(0).GameID), _
        '                                                       SQLString(_olstTicketBets(0).Context), _
        '                                                       SQLString(_olstTicketBets(0).BetType))
        '        End If

        '    End If
        '    Select Case UCase(_olstTicketBets(0).BetType)
        '        Case "MONEYLINE"
        '            If _olstTicketBets(0).IsForProp Then
        '                sWhere &= " AND PropMoneyLine <> 0"
        '                Return SafeDouble(poTblPlayerBet.Compute("SUM(PropMoneyLine)", sWhere))
        '            Else
        '                If _olstTicketBets(0).HomeMoneyLine <> 0 Then
        '                    sWhere &= " AND HomeMoneyLine <> 0"
        '                Else
        '                    sWhere &= " AND AwayMoneyLine <> 0"
        '                End If
        '            End If

        '        Case "SPREAD"
        '            If _olstTicketBets(0).HomeSpreadMoney <> 0 Then
        '                sWhere &= " AND HomeSpreadMoney <> 0"
        '            Else
        '                sWhere &= " AND AwaySpreadMoney <> 0"
        '            End If

        '        Case "TOTALPOINTS"
        '            If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
        '                sWhere &= " AND TotalPointsOverMoney <> 0"
        '            Else
        '                sWhere &= " AND TotalPointsUnderMoney <> 0"
        '            End If
        '        Case "TEAMTOTALPOINTS"
        '            If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
        '                If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
        '                    sWhere &= " And TeamTotalName='away' AND TotalPointsOverMoney <> 0"
        '                Else
        '                    sWhere &= " And TeamTotalName='home' AND TotalPointsOverMoney <> 0"
        '                End If

        '            Else
        '                If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
        '                    sWhere &= " And TeamTotalName='away' AND TotalPointsUnderMoney <> 0"
        '                Else
        '                    sWhere &= " And TeamTotalName='home' AND TotalPointsUnderMoney <> 0"
        '                End If
        '            End If
        '        Case "DRAW"
        '            sWhere &= " AND DrawMoneyLine <> 0"

        '    End Select

        '    Return SafeDouble(poTblPlayerBet.Compute("SUM(RiskAmount)", sWhere))
        'End Function

        Private Function getTotalWinBetting(ByVal poTblPlayerBet As DataTable) As Double
            'If _olstTicketBets.Count = 0 OrElse poTblPlayerBet Is Nothing OrElse poTblPlayerBet.Rows.Count = 0 Then
            '    Return 0
            'End If

            'Dim sWhere As String = String.Format("GameID = {0} AND Context = {1} AND BetType = {2}", _
            '                                     SQLString(_olstTicketBets(0).GameID), _
            '                                     SQLString(_olstTicketBets(0).Context), _
            '                                     SQLString(_olstTicketBets(0).BetType))

            'Select Case UCase(_olstTicketBets(0).BetType)
            '    Case "MONEYLINE"
            '        If _olstTicketBets(0).IsForProp Then
            '            sWhere &= " AND PropMoneyLine <> 0"
            '        Else
            '            If _olstTicketBets(0).HomeMoneyLine <> 0 Then
            '                sWhere &= " AND HomeMoneyLine <> 0"
            '            Else
            '                sWhere &= " AND AwayMoneyLine <> 0"
            '            End If
            '        End If

            '    Case "SPREAD"
            '        If _olstTicketBets(0).HomeSpreadMoney <> 0 Then
            '            sWhere &= " AND HomeSpreadMoney <> 0"
            '        Else
            '            sWhere &= " AND AwaySpreadMoney <> 0"
            '        End If

            '    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
            '        If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
            '            sWhere &= " AND TotalPointsOverMoney <> 0"
            '        Else
            '            sWhere &= " AND TotalPointsUnderMoney <> 0"
            '        End If

            '    Case "DRAW"
            '        sWhere &= " AND DrawMoneyLine <> 0"

            'End Select

            'Return SafeDouble(poTblPlayerBet.Compute("SUM(WinAmount)", sWhere))
            If _olstTicketBets.Count = 0 OrElse poTblPlayerBet Is Nothing OrElse poTblPlayerBet.Rows.Count = 0 Then
                Return 0
            End If
            Dim sWhere As String = ""

            If _olstTicketBets(0).IsForProp Then
                sWhere = String.Format("GameLineID = {0} AND Context = {1} AND BetType = {2}", _
                                           SQLString(_olstTicketBets(0).GameLineID), _
                                           SQLString(_olstTicketBets(0).Context), _
                                           SQLString(_olstTicketBets(0).BetType))
            Else
                If _olstTicketBets(0).Context.Contains("Q") Then
                    sWhere = String.Format("GameID = {0} AND Context in ('1Q','2Q','3Q','4Q') AND BetType = {1}", _
                                                            SQLString(_olstTicketBets(0).GameID), _
                                                            SQLString(_olstTicketBets(0).BetType))
                Else
                    sWhere = String.Format("GameID = {0} AND Context = {1} AND BetType = {2}", _
                                                               SQLString(_olstTicketBets(0).GameID), _
                                                               SQLString(_olstTicketBets(0).Context), _
                                                               SQLString(_olstTicketBets(0).BetType))
                End If

            End If



            Select Case UCase(_olstTicketBets(0).BetType)
                Case "MONEYLINE"
                    If _olstTicketBets(0).IsForProp Then
                        sWhere &= " AND PropMoneyLine <> 0"
                        Return SafeDouble(poTblPlayerBet.Compute("SUM(PropMoneyLine)", sWhere))
                    Else
                        If _olstTicketBets(0).HomeMoneyLine <> 0 Then
                            sWhere &= " AND HomeMoneyLine <> 0"
                        Else
                            sWhere &= " AND AwayMoneyLine <> 0"
                        End If
                    End If

                Case "SPREAD"
                    If _olstTicketBets(0).HomeSpreadMoney <> 0 Then
                        sWhere &= " AND HomeSpreadMoney <> 0"
                    Else
                        sWhere &= " AND AwaySpreadMoney <> 0"
                    End If

                Case "TOTALPOINTS"
                    If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
                        sWhere &= " AND TotalPointsOverMoney <> 0"
                    Else
                        sWhere &= " AND TotalPointsUnderMoney <> 0"
                    End If
                Case "TEAMTOTALPOINTS"
                    If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
                        If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                            sWhere &= " And TeamTotalName='away' AND TotalPointsOverMoney <> 0"
                        Else
                            sWhere &= " And TeamTotalName='home' AND TotalPointsOverMoney <> 0"
                        End If

                    Else
                        If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                            sWhere &= " And TeamTotalName='away' AND TotalPointsUnderMoney <> 0"
                        Else
                            sWhere &= " And TeamTotalName='home' AND TotalPointsUnderMoney <> 0"
                        End If
                    End If
                Case "DRAW"
                    sWhere &= " AND DrawMoneyLine <> 0"

            End Select

            Return SafeDouble(poTblPlayerBet.Compute("SUM(WinAmount)", sWhere))
        End Function

        Private Function getTotalRiskBetting(ByVal poTblPlayerBet As DataTable) As Double
            If _olstTicketBets.Count = 0 OrElse poTblPlayerBet Is Nothing OrElse poTblPlayerBet.Rows.Count = 0 Then
                Return 0
            End If
            Dim sWhere As String = ""

            If _olstTicketBets(0).IsForProp Then
                sWhere = String.Format("GameLineID = {0} AND Context = {1} AND BetType = {2}", _
                                           SQLString(_olstTicketBets(0).GameLineID), _
                                           SQLString(_olstTicketBets(0).Context), _
                                           SQLString(_olstTicketBets(0).BetType))
            Else
                If _olstTicketBets(0).Context.Contains("Q") Then
                    sWhere = String.Format("GameID = {0} AND Context in ('1Q','2Q','3Q','4Q') AND BetType = {1}", _
                                                            SQLString(_olstTicketBets(0).GameID), _
                                                            SQLString(_olstTicketBets(0).BetType))
                Else
                    sWhere = String.Format("GameID = {0} AND Context = {1} AND BetType = {2}", _
                                                               SQLString(_olstTicketBets(0).GameID), _
                                                               SQLString(_olstTicketBets(0).Context), _
                                                               SQLString(_olstTicketBets(0).BetType))
                End If

            End If



            Select Case UCase(_olstTicketBets(0).BetType)
                Case "MONEYLINE"
                    If _olstTicketBets(0).IsForProp Then
                        sWhere &= " AND PropMoneyLine <> 0"
                        Return SafeDouble(poTblPlayerBet.Compute("SUM(PropMoneyLine)", sWhere))
                    Else
                        If _olstTicketBets(0).HomeMoneyLine <> 0 Then
                            sWhere &= " AND HomeMoneyLine <> 0"
                        Else
                            sWhere &= " AND AwayMoneyLine <> 0"
                        End If
                    End If

                Case "SPREAD"
                    If _olstTicketBets(0).HomeSpreadMoney <> 0 Then
                        sWhere &= " AND HomeSpreadMoney <> 0"
                    Else
                        sWhere &= " AND AwaySpreadMoney <> 0"
                    End If

                Case "TOTALPOINTS"
                    If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
                        sWhere &= " AND TotalPointsOverMoney <> 0"
                    Else
                        sWhere &= " AND TotalPointsUnderMoney <> 0"
                    End If
                Case "TEAMTOTALPOINTS"
                    If _olstTicketBets(0).TotalPointsOverMoney <> 0 Then
                        If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                            sWhere &= " And TeamTotalName='away' AND TotalPointsOverMoney <> 0"
                        Else
                            sWhere &= " And TeamTotalName='home' AND TotalPointsOverMoney <> 0"
                        End If

                    Else
                        If _olstTicketBets(0).TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                            sWhere &= " And TeamTotalName='away' AND TotalPointsUnderMoney <> 0"
                        Else
                            sWhere &= " And TeamTotalName='home' AND TotalPointsUnderMoney <> 0"
                        End If
                    End If
                Case "DRAW"
                    sWhere &= " AND DrawMoneyLine <> 0"

            End Select

            Return SafeDouble(poTblPlayerBet.Compute("SUM(RiskAmount)", sWhere))
        End Function

        ''' <summary>
        ''' Return Maximum Bet Amount configured from SA
        ''' </summary>
        ''' <param name="poPlayerTemplate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getMaxBetAmount(ByVal oLstGameContext As List(Of String), ByVal poLstGameType As List(Of String), ByVal poPlayerTemplate As CPlayerTemplate) As Double
            Select Case UCase(TicketType)

                Case "STRAIGHT", "IF BET"
                    Return PlayerMaxBetAmount(oLstGameContext, poPlayerTemplate, poLstGameType, "MinSingle")
                Case "REVERSE"
                    '  Return poPlayerTemplate.CreditMaxReverseActionParlay
                    Return PlayerMaxBetAmount(oLstGameContext, poPlayerTemplate, poLstGameType, "MinReverse")
                Case "PARLAY"
                    'Return poPlayerTemplate.CreditMaxParlay
                    Return PlayerMaxBetAmount(oLstGameContext, poPlayerTemplate, poLstGameType, "MinParlay")
                Case "TEASER"
                    ' Return poPlayerTemplate.CreditMaxTeaserParlay
                    Return PlayerMaxBetAmount(oLstGameContext, poPlayerTemplate, poLstGameType, "MinTeaser")
                Case Else
                    Return 0

            End Select

        End Function

        Public Function GetMinPlayerTemplates(ByVal pLstContext As List(Of String), ByVal poPlayerTemplate As CPlayerTemplate, ByVal pLstGameType As List(Of String), ByVal psBetType As String) As Double
            Dim lstPlayerTemplateLimit As New List(Of Double)
            LogDebug(_log, "pLstGameType :" & pLstGameType(0))
            pLstGameType = (From gameType As String In pLstGameType Select UCase(gameType)).ToList()
            Select Case UCase(psBetType)
                Case "MINSINGLE"
                    lstPlayerTemplateLimit = (From PlayerTemplateLimit As CPlayerTemplateLimit In poPlayerTemplate.Limits _
                    Where (pLstContext.Contains(PlayerTemplateLimit.Context) Or pLstContext.Contains("TeamTotalPoints")) And pLstGameType.Contains(UCase(PlayerTemplateLimit.GameType)) _
                            Order By PlayerTemplateLimit.MaxSingle _
                     Select PlayerTemplateLimit.MaxSingle).ToList

                Case "MINREVERSE"
                    lstPlayerTemplateLimit = (From PlayerTemplateLimit As CPlayerTemplateLimit In poPlayerTemplate.Limits _
                    Where pLstContext.Contains(PlayerTemplateLimit.Context) And pLstGameType.Contains(UCase(PlayerTemplateLimit.GameType)) _
                            Order By PlayerTemplateLimit.MaxReverse _
                     Select PlayerTemplateLimit.MaxReverse).ToList
                Case "MINPARLAY"
                    lstPlayerTemplateLimit = (From PlayerTemplateLimit As CPlayerTemplateLimit In poPlayerTemplate.Limits _
                  Where pLstContext.Contains(PlayerTemplateLimit.Context) And pLstGameType.Contains(UCase(PlayerTemplateLimit.GameType)) _
                          Order By PlayerTemplateLimit.MaxParlay _
                   Select PlayerTemplateLimit.MaxParlay).ToList
                Case "MINTEASER"
                    lstPlayerTemplateLimit = (From PlayerTemplateLimit As CPlayerTemplateLimit In poPlayerTemplate.Limits _
                      Where pLstContext.Contains(PlayerTemplateLimit.Context) And pLstGameType.Contains(UCase(PlayerTemplateLimit.GameType)) _
                              Order By PlayerTemplateLimit.MaxTeaser _
                       Select PlayerTemplateLimit.MaxTeaser).ToList
            End Select
            If lstPlayerTemplateLimit.Count > 0 Then
                LogDebug(_log, lstPlayerTemplateLimit.Count & "lstPlayerTemplateLimit" & lstPlayerTemplateLimit(0))
                Return lstPlayerTemplateLimit(0)
            End If

            Return -1


        End Function


        Private Function PlayerMaxBetAmount(ByVal oLstGameContext As List(Of String), ByVal poPlayerTemplate As CPlayerTemplate, ByVal poLstGameType As List(Of String), ByVal psBetType As String) As Double
            'Dim oPlayerTemplateLimit As New CPlayerTemplateLimit()
            'oPlayerTemplateLimit.
            'Dim oPlayerTemplateLimitManager As New SBCBL.Managers.CPlayerTemplateLimitManager()
            'Dim odtPlayerTemplateLimit As DataTable = oPlayerTemplateLimitManager.GetMinPlayerTemplates(oLstGameContext, poPlayerTemplate.PlayerTemplateID, poLstGameType, psBetType)
            'If odtPlayerTemplateLimit IsNot Nothing AndAlso odtPlayerTemplateLimit.Rows.Count > 0 Then
            '' max player can bet it seted in player limit
            'Dim nMaxPlayerBet As Double = SafeDouble(odtPlayerTemplateLimit.Rows(0)(psBetType))

            Dim nMaxPlayerBet As Double = GetMinPlayerTemplates(oLstGameContext, poPlayerTemplate, poLstGameType, psBetType)
            LogDebug(_log, "nMaxPlayerBet:" & nMaxPlayerBet)
            If nMaxPlayerBet > -1 Then
                Select Case UCase(TicketType)
                    Case "STRAIGHT", "IF BET"
                        Select Case oLstGameContext(0)
                            Case "1H"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case "2H"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case "1Q"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case "2Q"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case "3Q"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case "4Q"
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                            Case Else
                                Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                        End Select
                    Case "REVERSE"
                        ' Return SafeDouble(IIf(poPlayerTemplate.CreditMaxReverseActionParlay >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.CreditMaxReverseActionParlay))
                        Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                    Case "PARLAY"
                        ' Return SafeDouble(IIf(poPlayerTemplate.CreditMaxParlay >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.CreditMaxParlay))
                        Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                    Case "TEASER"
                        Return SafeDouble(IIf(poPlayerTemplate.MaxSingle >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.MaxSingle))
                        'Return SafeDouble(IIf(poPlayerTemplate.CreditMaxTeaserParlay >= nMaxPlayerBet, nMaxPlayerBet, poPlayerTemplate.CreditMaxTeaserParlay))
                End Select
            End If
            '' if not row return value in player template
            Select Case UCase(TicketType)
                Case "STRAIGHT", "IF BET"
                    Select Case oLstGameContext(0)
                        Case "1H"
                            Return poPlayerTemplate.Max1H
                        Case "2H"
                            Return poPlayerTemplate.Max2H
                        Case "1Q"
                            Return poPlayerTemplate.Max1Q
                        Case "2Q"
                            Return poPlayerTemplate.Max2Q
                        Case "3Q"
                            Return poPlayerTemplate.Max3Q
                        Case "4Q"
                            Return poPlayerTemplate.Max4Q
                        Case Else
                            Return poPlayerTemplate.MaxSingle
                    End Select
                Case "REVERSE"
                    ' Return poPlayerTemplate.CreditMaxReverseActionParlay
                    Return poPlayerTemplate.MaxSingle
                Case "PARLAY"
                    'Return poPlayerTemplate.CreditMaxParlay
                    Return poPlayerTemplate.MaxSingle
                Case "TEASER"
                    'Return poPlayerTemplate.CreditMaxTeaserParlay
                    Return poPlayerTemplate.MaxSingle
            End Select
        End Function

        ''' <summary>
        ''' Return Minimum Bet Amount configured from SA
        ''' </summary>
        ''' <param name="poPlayerTemplate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getMinBetAmount(ByVal poPlayerTemplate As CPlayerTemplate, ByVal pbTypeOfBet As CEnums.ETypeOfBet) As Double
            If pbTypeOfBet = CEnums.ETypeOfBet.Internet Then
                Return poPlayerTemplate.CreditMinBetInternet

            Else
                Return poPlayerTemplate.CreditMinBetPhone

            End If

        End Function

        ''' <summary>
        ''' Add Wager's item
        ''' </summary>
        ''' <param name="poTicketBet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddTicketBet(ByVal poTicketBet As CTicketBet) As String
            _log.Debug("BEGIN add TicketBet. TicketID: " & _sTicketID)

            Dim oCache As New CCacheManager()
            For Each oBet As CTicketBet In _olstTicketBets
                If oBet.GameID = poTicketBet.GameID AndAlso oBet.Bookmaker = poTicketBet.Bookmaker Then
                    If oBet.Team = poTicketBet.Team AndAlso oBet.TeamNumber = poTicketBet.TeamNumber _
                    AndAlso oBet.BetType = poTicketBet.BetType AndAlso oBet.Context = poTicketBet.Context Then '' Check exist TicketBet
                        If Not oBet.BetType.Equals("TeamTotalPoints", StringComparison.CurrentCultureIgnoreCase) Then
                            Return "This Bet has been selected."
                        End If
                    ElseIf oBet.BetType = poTicketBet.BetType AndAlso oBet.Context = poTicketBet.Context Then '' Check opposite team
                        If Not oBet.BetType.Equals("TeamTotalPoints", StringComparison.CurrentCultureIgnoreCase) Then
                            If Not oBet.Context.Equals("Proposition", StringComparison.CurrentCultureIgnoreCase) Then
                                Return "Can not select opposite team at same bet type."
                            End If

                        End If

                    End If
                End If
            Next

            Dim sError As String = ""
            Select Case UCase(TicketType)
                Case "PARLAY"
                    sError = validateParlayTicket(poTicketBet)

                Case "REVERSE"
                    sError = validateReverseTicket(poTicketBet)

                Case "TEASER"
                    sError = validateTeaserTicket(poTicketBet)

            End Select

            If sError <> "" Then
                _log.Debug("Fail to add TicketBet. Message: " & sError)
                Return sError
            End If

            _log.Debug("Add TicketBet successfully.")
            _olstTicketBets.Add(poTicketBet)
            Me.IsForProp = poTicketBet.IsForProp

            poTicketBet.TicketID = Me.TicketID
            poTicketBet.RiskAmount = Me.RiskAmount
            ReCalcAmount()

            _log.Debug("END add TicketBet. TicketID: " & _sTicketID)

            Return ""
        End Function

        ''' <summary>
        ''' Remove Wager's items
        ''' </summary>
        ''' <param name="poTicketBetIDs"></param>
        ''' <remarks></remarks>
        Public Sub RemoveTicketBets(ByVal poTicketBetIDs As String)
            For Each sTicketBetID As String In poTicketBetIDs.Split(","c)
                _log.Debug("Remove TicketBet. TicketBetID: " & sTicketBetID)
                _olstTicketBets.RemoveByID(sTicketBetID)
            Next

            ReCalcAmount()
        End Sub

        ''' <summary>
        ''' Use to generate Ticket's options
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ParseOptions() As List(Of DictionaryEntry)
            Dim olstDic As List(Of DictionaryEntry)

            Select Case UCase(TicketType)
                Case "PARLAY"
                    olstDic = ParseParlayOptions()
                Case "REVERSE"
                    olstDic = ParseReverseOptions()
                Case "TEASER"
                    olstDic = ParseTeaserOptions()
                Case "IF BET"
                    olstDic = ParseIfBetOptions()
                Case Else
                    olstDic = New List(Of DictionaryEntry)
            End Select

            Return olstDic
        End Function

        Public Function RoundRobinOptions() As List(Of DictionaryEntry)
            Dim olstDic As List(Of DictionaryEntry)

            Select Case UCase(TicketType)
                Case "PARLAY"
                    olstDic = ParseParlayOptionsCheckbox()
                Case Else
                    olstDic = New List(Of DictionaryEntry)
            End Select

            Return olstDic
        End Function

        Public Sub ReCalcAmount()
            _nRiskAmount = 0
            _nWinAmount = 0

            If _nBetAmount = 0 Then
                Exit Sub
            End If

            Select Case UCase(TicketType)
                Case "PARLAY"
                    ReCalcParlayAmount()
                Case "REVERSE"
                    ReCalcReverseAmount()
                Case "TEASER"
                    ReCalcTeaserAmount()
                    'CalcRiskAmountByWinAmount()
                Case "STRAIGHT", "IF BET"
                    ' _log.Debug(WinAmount & "-- " & SafeString(RiskAmount.ToString()) & "-You are exceeding the maximum allowed for this game.")

                    ReCalcStraightAmount()
                    ' _log.Debug(WinAmount & "-- " & SafeString(RiskAmount.ToString()) & "-You are exceeding the maximum allowed for this game.")

                    If Not SetWinAmount Then
                        CalcRiskAmountByWinAmount()
                    End If
            End Select

        End Sub

#End Region

#Region "PARLAY"
        Private Function validateParlayTicket(ByVal poTicketBet As CTicketBet) As String
            Dim oCache As New CCacheManager()

            '' Get Setup Parlay for each GameType
            Dim olstParlayType As CSysSettingList = Nothing
            Dim olstParlayRules As CSysSettingList = Nothing
            Dim olstBWParlayRules As CSysSettingList = Nothing

            '' if exists config parplay  not get default 
            If oCache.GetSysSettings(SuperAgentID & " ParlayType", "Football").Count > 0 Then
                _sParlayType = SuperAgentID & " ParlayType"
                _sReverseType = SuperAgentID & " ReverseType"
                _sParlayRules = SuperAgentID & " ParlayRules"

            End If
            If oCache.GetSysSettings(SuperAgentID & " BWParlayRules", "Football").Count > 0 Then
                _sBWParlayRules = SuperAgentID & " BWParlayRules"
            End If
            Select Case True
                Case IsFootball(poTicketBet.GameType)
                    olstParlayType = oCache.GetSysSettings(_sParlayType, "Football")
                    olstParlayRules = oCache.GetSysSettings(_sParlayRules, "Football")
                    olstBWParlayRules = oCache.GetSysSettings(_sBWParlayRules, "Football")

                Case IsBasketball(poTicketBet.GameType)
                    olstParlayType = oCache.GetSysSettings(_sParlayType, "Basketball")
                    olstParlayRules = oCache.GetSysSettings(_sParlayRules, "Basketball")
                    olstBWParlayRules = oCache.GetSysSettings(_sBWParlayRules, "Basketball")

                Case IsBaseball(poTicketBet.GameType)
                    olstParlayType = oCache.GetSysSettings(_sParlayType, "Baseball")
                    olstParlayRules = oCache.GetSysSettings(_sParlayRules, "Baseball")
                    olstBWParlayRules = oCache.GetSysSettings(_sBWParlayRules, "Baseball")

                Case IsHockey(poTicketBet.GameType)
                    olstParlayType = oCache.GetSysSettings(_sParlayType, "Hockey")
                    olstParlayRules = oCache.GetSysSettings(_sParlayRules, "Hockey")
                    olstBWParlayRules = oCache.GetSysSettings(_sBWParlayRules, "Hockey")

                Case IsSoccer(poTicketBet.GameType)
                    olstParlayType = oCache.GetSysSettings(_sParlayType, "Soccer")
                    olstParlayRules = oCache.GetSysSettings(_sParlayRules, "Soccer")
                    olstBWParlayRules = oCache.GetSysSettings(_sBWParlayRules, "Soccer")

            End Select

            '' Get Setup Parlay for each Type of Game: Current | 1H | 2H
            '' Check Parlay allowance
            If olstParlayType Is Nothing OrElse olstParlayRules Is Nothing OrElse _
            Not olstParlayType.GetBooleanValue(poTicketBet.SysSettingKey) Then
                _log.Debug(String.Format("{0} {1} is not allowed with Parlay Wager.", poTicketBet.Context, poTicketBet.BetType))
                Return String.Format("{0} {1} is not allowed with Parlay wager.", poTicketBet.Context, poTicketBet.BetType)
            End If

            '' Check Combined condition
            ' Between Games
            If olstBWParlayRules IsNot Nothing Then
                For Each oBet As CTicketBet In _olstTicketBets
                    ' Don't check allownace when they are same key
                    If oBet.GameID = poTicketBet.GameID OrElse oBet.GameType <> poTicketBet.GameType Then
                        Continue For
                    End If

                    If Not (olstBWParlayRules.GetBooleanValue(oBet.SysSettingKey & "_" & poTicketBet.SysSettingKey) OrElse _
                        olstBWParlayRules.GetBooleanValue(poTicketBet.SysSettingKey & "_" & oBet.SysSettingKey)) Then
                        Return "This is not a Valid Combination between games."
                    End If
                Next
            End If

            ' In Game
            For Each oBet As CTicketBet In _olstTicketBets
                If oBet.GameID = poTicketBet.GameID Then
                    If Not (olstParlayRules.GetBooleanValue(oBet.SysSettingKey & "_" & poTicketBet.SysSettingKey) OrElse _
                    olstParlayRules.GetBooleanValue(poTicketBet.SysSettingKey & "_" & oBet.SysSettingKey)) Then
                        Return "This is not a Valid Combination in game."
                    End If
                End If
            Next

            '' Check Max Selection for each Parlay
            '  Dim oGameTypeOnOffManager As New SBCBL.Managers.CGameTypeOnOffManager()
            ' Dim nMaxParplaySelect = oCache.GetParplaySetUp(SuperAgentID) 'oGameTypeOnOffManager.GetValueParplaySetup(sAgentID)
            Dim maxparplaySelect As String = SuperAgentID & " ParlayRules"
            ''check agent if exist agent setup get agent setup parlay
            'If nMaxParplaySelect <= 0 Then
            'maxparplaySelect = GetSiteType() & " ParlayRules"
            If _olstTicketBets.Count >= oCache.GetSysSettings(maxparplaySelect).GetIntegerValue(_sMaxSelection) Then
                Return String.Format("Your maximum selection allowed is {0} for Parlay wager.", _
                                     oCache.GetSysSettings(maxparplaySelect).GetIntegerValue(_sMaxSelection))
            End If
            'Else
            'If _olstTicketBets.Count >= nMaxParplaySelect Then
            'Return String.Format("Your maximum selection allowed is {0} for Parlay wager.", nMaxParplaySelect)
            'End If
            '  End If
            Return ""
        End Function

        ''' <summary>
        ''' Calculate N!/R!
        ''' </summary>
        ''' <param name="pnN"></param>
        ''' <param name="pnR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Factorial(ByVal pnN As Integer, ByVal pnR As Integer) As Integer
            Dim nResult As Integer = 1
            If (pnR < 2) Then pnR = 1

            For nIndex As Integer = pnR + 1 To pnN
                nResult *= nIndex
            Next

            Return nResult
        End Function

        ''' <summary>
        ''' Calculate pnN choose pnR
        ''' </summary>
        ''' <param name="pnN"></param>
        ''' <param name="pnR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Combinatorial(ByVal pnN As Integer, ByVal pnR As Integer) As Integer
            Return SafeInteger(Factorial(pnN, pnR) / Factorial((pnN - pnR), 1))
        End Function

        ''' <summary>
        ''' Generate all possiable combinations
        ''' </summary>
        ''' <param name="pnItems">Remain items of each combination</param>
        ''' <param name="polstCombinations">All possibale combinations</param>
        ''' <param name="pnIndex">Begin position</param>
        ''' <param name="psItems">Contain position of bet in each combination</param>
        ''' <remarks></remarks>
        Private Sub EnumCombinations(ByVal pnItems As Integer, ByRef polstCombinations As List(Of String), Optional ByVal pnIndex As Integer = 0, Optional ByVal psItems As String = "")
            If pnItems = 0 Then
                polstCombinations.Add(psItems)
                Exit Sub
            End If

            For i As Integer = pnIndex To _olstTicketBets.Count - pnItems
                EnumCombinations(pnItems - 1, polstCombinations, i + 1, psItems & SafeString(IIf(psItems <> "", "|", "")) & i)
            Next
        End Sub

        ''' <summary>
        ''' Calculation Win/Risk amount
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ReCalcParlayAmount()
            If UCase(_sTicketOption) = "PARLAY" Then
                '' Single parlay
                LogDebug(_log, "Calc Single Parlay Amount")
                _nRiskAmount = _nBetAmount

                _nWinAmount = _nRiskAmount
                For Each oTicketBet In _olstTicketBets
                    _nWinAmount *= CalcRate(oTicketBet.BetPoint + oTicketBet.AddPointMoney)
                Next

                _nWinAmount -= _nRiskAmount
                _nWinAmount = CustomParlayAmount(_olstTicketBets.Count, _nWinAmount)
                _nWinAmount = Math.Round(_nWinAmount, 2)
            Else
                '' Round robin
                'Dim nItems As Integer
                'If Right(_sTicketOption, 1) = "s" Then
                '    nItems = SafeInteger(Left(_sTicketOption, Len(_sTicketOption) - 1))
                '    LogDebug(_log, String.Format("Calc At Most {0} Parlays Amount", nItems))
                'Else
                '    nItems = SafeInteger(_sTicketOption)
                '    LogDebug(_log, String.Format("Calc Exactly {0} Parlays Amount", nItems))
                'End If

                For Each i In _olstsTicketRoundRobinOption
                    _nRiskAmount += _nBetAmount * Combinatorial(_olstTicketBets.Count, i)

                    '' Get all possiable combinaations
                    Dim olstEnumCombinatons As New List(Of String)
                    EnumCombinations(i, olstEnumCombinatons)

                    For Each sCombination As String In olstEnumCombinatons
                        Dim nMaxParlayLine As Double = 1
                        Dim nMaxParlayItem As Integer = 0
                        For Each sItem As String In sCombination.Split("|"c)
                            nMaxParlayLine *= CalcRate(_olstTicketBets.ElementAt(SafeInteger(sItem)).BetPoint + _olstTicketBets.ElementAt(SafeInteger(sItem)).AddPointMoney)
                            nMaxParlayItem += 1
                        Next
                        _nWinAmount += Math.Round(CustomParlayAmount(nMaxParlayItem, (nMaxParlayLine - 1) * _nBetAmount), 2)
                    Next

                    'If Right(_sTicketOption, 1) <> "s" Then
                    '    '' Exactly round robin
                    '    Exit For
                    'End If
                Next


                'For i As Integer = nItems To 2 Step -1
                '    _nRiskAmount += _nBetAmount * Combinatorial(_olstTicketBets.Count, i)

                '    '' Get all possiable combinaations
                '    Dim olstEnumCombinatons As New List(Of String)
                '    EnumCombinations(i, olstEnumCombinatons)

                '    For Each sCombination As String In olstEnumCombinatons
                '        Dim nMaxParlayLine As Double = 1
                '        Dim nMaxParlayItem As Integer = 0
                '        For Each sItem As String In sCombination.Split("|"c)
                '            nMaxParlayLine *= CalcRate(_olstTicketBets.ElementAt(SafeInteger(sItem)).BetPoint + _olstTicketBets.ElementAt(SafeInteger(sItem)).AddPointMoney)
                '            nMaxParlayItem += 1
                '        Next
                '        _nWinAmount += Math.Round(CustomParlayAmount(nMaxParlayItem, (nMaxParlayLine - 1) * _nBetAmount), 2)
                '    Next

                '    If Right(_sTicketOption, 1) <> "s" Then
                '        '' Exactly round robin
                '        Exit For
                '    End If
                'Next

                _nWinAmount = Math.Round(_nWinAmount, 2)
                _nRiskAmount = Math.Round(_nRiskAmount, 2)
            End If
        End Sub

        Private Function CustomParlayAmount(ByVal pnTicketBets As Integer, ByVal pnAmount As Double) As Double

            Dim oCache As New CCacheManager()

            Dim olstCurrentPayout, olstTigerSBPlayout As CSysSettingList
            olstCurrentPayout = oCache.GetSysSettings(CATEGORY_CURRENT)
            olstTigerSBPlayout = oCache.GetSysSettings(CATEGORY_TIGERSB)

            Dim nCurrent, nTigerSB As Double
            nCurrent = olstCurrentPayout.GetDoubleValue(String.Format("{0} Teams", pnTicketBets))
            nTigerSB = olstTigerSBPlayout.GetDoubleValue(String.Format("{0} Teams", pnTicketBets))

            If nCurrent = 0 OrElse nTigerSB = 0 Then
                Return pnAmount
            Else
                Return pnAmount * Math.Round((nTigerSB / nCurrent), 4)
            End If
        End Function

        ''' <summary>
        ''' Use to generate Parlay's options
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseParlayOptions() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            ''Single Parlay
            Dim oDicItem As New DictionaryEntry("Parlay", "Single Parlay")
            olstDic.Add(oDicItem)

            '' Robin Round
            If _olstTicketBets.Count() > 2 Then
                oDicItem = New DictionaryEntry("2", "Round Robin: 2s")
                olstDic.Add(oDicItem)

                For nIndex As Integer = 3 To _olstTicketBets.Count()
                    Dim sValue As String = String.Format("{0}s", nIndex)

                    If nIndex <> _olstTicketBets.Count Then
                        oDicItem = New DictionaryEntry(SafeString(nIndex), "Round Robin: " & sValue)
                        olstDic.Add(oDicItem)
                    End If

                    sValue = String.Format("{0}s", 2)
                    For nBind As Integer = 3 To nIndex

                        sValue &= String.Format(",{0}s", nBind)
                    Next

                    oDicItem = New DictionaryEntry(SafeString(nIndex) & "s", "Round Robin: " & sValue)
                    olstDic.Add(oDicItem)
                Next
            End If

            Return olstDic
        End Function

        Private Function ParseParlayOptionsCheckbox() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            '' Robin Round
            If _olstTicketBets.Count() > 2 Then
                For i As Integer = 2 To _olstTicketBets.Count()
                    Dim oDicItem As New DictionaryEntry(i, String.Format("{0}'s x {1}", i, Combinatorial(_olstTicketBets.Count, i)))
                    olstDic.Add(oDicItem)
                Next
            End If

            Return olstDic
        End Function

        ''' <summary>
        ''' Gnerate all possiable parlay tickets
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GenerateParlay() As CTicketList
            LogDebug(_log, "BEGIN Generate Parlay. ParlayOption: " & _sTicketOption)
            Dim olstTickets As New CTicketList

            If UCase(_sTicketOption) = "PARLAY" Then
                '' Single parlay
                LogDebug(_log, "Generate single parlay ticket.")
                olstTickets.Add(Me)
            Else
                '' Round robin
                Dim nItems As Integer
                If Right(_sTicketOption, 1) = "s" Then
                    nItems = SafeInteger(Left(_sTicketOption, Len(_sTicketOption) - 1))
                    LogDebug(_log, String.Format("Generate At Most {0} Parlays Amount", nItems))
                Else
                    nItems = SafeInteger(_sTicketOption)
                    LogDebug(_log, String.Format("Generate Exactly {0} Parlays Amount", nItems))
                End If

                For i As Integer = nItems To 2 Step -1
                    LogDebug(_log, String.Format("Generate Round robin {0}s", i))
                    CreateParlayTickets(i, olstTickets, _nBetAmount)

                    If Right(_sTicketOption, 1) <> "s" Then
                        '' Exactly round robin
                        Exit For
                    End If
                Next
            End If

            LogDebug(_log, "END Generate Parlay.")
            Return olstTickets
        End Function

        ''' <summary>
        ''' Generate all possiable combinations
        ''' </summary>
        ''' <param name="pnItems">Remain items in each combination</param>
        ''' <param name="polstTickets">All possiable combinations</param>
        ''' <param name="pnBetAmount">Bet amount in each combination</param>
        ''' <param name="pnIndex">Start position</param>
        ''' <param name="poTicket">Contain all bets in each combination</param>
        ''' <remarks></remarks>
        Private Sub CreateParlayTickets(ByVal pnItems As Integer, ByRef polstTickets As CTicketList, ByVal pnBetAmount As Double, _
                                        Optional ByVal pnIndex As Integer = 0, Optional ByVal poTicket As CTicket = Nothing)
            If pnItems = 0 Then
                polstTickets.Add(poTicket)
                Exit Sub
            End If

            For i As Integer = pnIndex To _olstTicketBets.Count - pnItems
                Dim oGenTicket As CTicket
                If poTicket Is Nothing Then
                    oGenTicket = New CTicket(Me.TicketType, SuperAgentID, PlayerID)
                    oGenTicket.BetAmount = pnBetAmount
                    oGenTicket.TicketOption = "Parlay"
                Else
                    oGenTicket = New CTicket(poTicket)
                End If

                oGenTicket.AddTicketBet(New CTicketBet(_olstTicketBets.Item(i)))
                CreateParlayTickets(pnItems - 1, polstTickets, pnBetAmount, i + 1, oGenTicket)
            Next
        End Sub
#End Region

#Region "REVERSE"
        Private Function validateReverseTicket(ByVal poTicketBet As CTicketBet) As String
            Dim oCache As New CCacheManager()

            '' Get Setup Parlay for each GameType
            Dim olstReverseType As CSysSettingList = Nothing
            Dim olstReverseRules As CSysSettingList = Nothing
            Dim olstBWReverseRules As CSysSettingList = Nothing
            
            If oCache.GetSysSettings(SuperAgentID & " ParlayType", "Football").Count > 0 Then
                _sParlayType = SuperAgentID & " ParlayType"
                _sReverseType = SuperAgentID & " ReverseType"
                _sParlayRules = SuperAgentID & " ParlayRules"

            End If
            If oCache.GetSysSettings(SuperAgentID & " BWParlayRules", "Football").Count > 0 Then
                _sBWParlayRules = SuperAgentID & " BWParlayRules"
            End If
            Select Case True
                Case IsFootball(poTicketBet.GameType)
                    olstReverseType = oCache.GetSysSettings(_sReverseType, "Football")
                    olstReverseRules = oCache.GetSysSettings(_sParlayRules, "Football")
                    olstBWReverseRules = oCache.GetSysSettings(_sBWParlayRules, "Football")

                Case IsBasketball(poTicketBet.GameType)
                    olstReverseType = oCache.GetSysSettings(_sReverseType, "Basketball")
                    olstReverseRules = oCache.GetSysSettings(_sParlayRules, "Basketball")
                    olstBWReverseRules = oCache.GetSysSettings(_sBWParlayRules, "Basketball")

                Case IsBaseball(poTicketBet.GameType)
                    olstReverseType = oCache.GetSysSettings(_sReverseType, "Baseball")
                    olstReverseRules = oCache.GetSysSettings(_sParlayRules, "Baseball")
                    olstBWReverseRules = oCache.GetSysSettings(_sBWParlayRules, "Baseball")

                Case IsHockey(poTicketBet.GameType)
                    olstReverseType = oCache.GetSysSettings(_sReverseType, "Hockey")
                    olstReverseRules = oCache.GetSysSettings(_sParlayRules, "Hockey")
                    olstBWReverseRules = oCache.GetSysSettings(_sBWParlayRules, "Hockey")

                Case IsSoccer(poTicketBet.GameType)
                    olstReverseType = oCache.GetSysSettings(_sReverseType, "Soccer")
                    olstReverseRules = oCache.GetSysSettings(_sParlayRules, "Soccer")
                    olstBWReverseRules = oCache.GetSysSettings(_sBWParlayRules, "Soccer")

            End Select

            '' Get Setup Reverse for each Type of Game: Current | 1H | 2H
            '' Check Reverse allowance
            If olstReverseType Is Nothing OrElse olstReverseRules Is Nothing OrElse _
            Not olstReverseType.GetBooleanValue(poTicketBet.SysSettingKey) Then
                _log.Debug(String.Format("{0} {1} is not allowed with Reverse Wager.", poTicketBet.Context, poTicketBet.BetType))
                Return String.Format("{0} {1} is not allowed with Reverse wager.", poTicketBet.Context, poTicketBet.BetType)
            End If

            '' Check Combined condition
            ' Between Games
            If olstBWReverseRules IsNot Nothing Then
                For Each oBet As CTicketBet In _olstTicketBets
                    ' Don't check allownace when they are same key
                    If oBet.GameID = poTicketBet.GameID OrElse oBet.GameType <> poTicketBet.GameType Then
                        Continue For
                    End If

                    If Not (olstBWReverseRules.GetBooleanValue(oBet.SysSettingKey & "_" & poTicketBet.SysSettingKey) OrElse _
                        olstBWReverseRules.GetBooleanValue(poTicketBet.SysSettingKey & "_" & oBet.SysSettingKey)) Then
                        Return "This is not a Valid Combination between games."
                    End If
                Next
            End If

            ' In Games
            For Each oBet As CTicketBet In _olstTicketBets
                If oBet.GameID = poTicketBet.GameID Then
                    If Not (olstReverseRules.GetBooleanValue(oBet.SysSettingKey & "_" & poTicketBet.SysSettingKey) OrElse _
                    olstReverseRules.GetBooleanValue(poTicketBet.SysSettingKey & "_" & oBet.SysSettingKey)) Then
                        Return "This is not a Valid Combination in game."
                    End If
                End If
            Next

            '' Check Max Selection for each Reverse
            If _olstTicketBets.Count = 2 Then
                Return "Your maximum selection allowed is 2 for Reverse wager."
            End If

            Return ""
        End Function

        ''' <summary>
        ''' Use to generate Reverse's options
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseReverseOptions() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            Dim oDicItem As New DictionaryEntry("Action Reverse", "Action Reverse")
            olstDic.Add(oDicItem)

            oDicItem = New DictionaryEntry("Win Reverse", "Win Reverse")
            olstDic.Add(oDicItem)

            Return olstDic
        End Function

        Private Sub ReCalcReverseAmount()
            LogDebug(_log, "Calc Reverse Amount")
            _nRiskAmount = BetAmount * _olstTicketBets.Count * (_olstTicketBets.Count - 1)

            For nIndex As Integer = 0 To _olstTicketBets.Count - 1
                _nWinAmount += (CalcRate(_olstTicketBets.ElementAt(nIndex).BetPoint + _olstTicketBets.ElementAt(nIndex).AddPointMoney) - 1) * BetAmount * 2 * (_olstTicketBets.Count - 1)
            Next
            _nWinAmount = Math.Round(_nWinAmount, 2)
        End Sub
#End Region

#Region "TEASER"
        Private Function validateTeaserTicket(ByVal poTicketBet As CTicketBet) As String
            If Not (IsBasketball(poTicketBet.GameType) OrElse IsFootball(poTicketBet.GameType)) Then
                _log.Debug(String.Format("{0} is not allowed with Teaser Wager.", poTicketBet.SportType))
                Return String.Format("{0} is not allowed with Teaser Wager.", poTicketBet.SportType)
            End If

            If UCase(poTicketBet.Context) <> "CURRENT" Then
                _log.Debug("1H ( or 2H ) is not allowed with Teaser Wager.")
                Return "1H ( or 2H ) is not allowed with Teaser Wager."
            End If

            If _olstTicketBets.Count = 9 Then
                Return "Your maximum selection allowed is 9 for Teaser wager."
            End If

            If UCase(poTicketBet.BetType) = "MONEYLINE" Then
                Return "MoneyLine is not allowed with Teaser Wager."
            End If

            Return ""
        End Function

        ''' <summary>
        ''' Use to generate Teaser's options
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseTeaserOptions() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            Dim oDicItem As DictionaryEntry
            Dim oCache As New CCacheManager()
            For Each oTeaserRule As CTeaserRule In oCache.GetTeaserRules()
                oDicItem = New DictionaryEntry(oTeaserRule.TeaserRuleID, oTeaserRule.TeaserRuleName)
                olstDic.Add(oDicItem)
            Next

            Return olstDic
        End Function

        Private Sub ReCalcTeaserAmount()
            LogDebug(_log, "Calc Teaser Amount")
            _nRiskAmount = _nBetAmount

            If SafeString(Me._sTicketOption) <> "" Then
                _nWinAmount = _nRiskAmount

                Dim oCache As New CCacheManager()
                Dim oSysSetting As CSysSettingList = oCache.GetSysSettings(SBCBL.std.GetSiteType & " TEASER ODDS", Me._sTicketOption)
                Dim nFactor As Double = 0

                If oSysSetting IsNot Nothing Then
                    nFactor = oSysSetting.GetDoubleValue(SafeString(_olstTicketBets.Count))
                    _nWinAmount *= CalcRate(nFactor)
                    _nWinAmount -= _nRiskAmount
                End If

                _nWinAmount = Math.Round(_nWinAmount, 2)

                addTeaserPoints()
            End If
        End Sub

        Private Sub addTeaserPoints()
            Dim oTeaserRule As CTeaserRule = (New CCacheManager()).GetTeaserRuleInfo(Me._sTicketOption)
            Dim nAddPoint As Double = 0

            If oTeaserRule IsNot Nothing Then
                For Each oTicketBet As CTicketBet In _olstTicketBets
                    If IsBasketball(oTicketBet.GameType) Then
                        nAddPoint = oTeaserRule.BasketballPoint
                    ElseIf IsFootball(oTicketBet.GameType) Then
                        nAddPoint = oTeaserRule.FootbalPoint
                    End If

                    oTicketBet.AddPoint = nAddPoint
                    If oTicketBet.TotalPoints <> 0 Then
                        If oTicketBet.TotalPointsOverMoney <> 0 Then
                            oTicketBet.AddPoint = -nAddPoint
                        End If
                    End If
                Next
            End If
        End Sub
#End Region

#Region "STRAIGHT"
        Private Sub ReCalcStraightAmount()
            LogDebug(_log, "Calc Straight Amount")
            _nRiskAmount = _nBetAmount

            _nWinAmount = _nRiskAmount
            If _olstTicketBets.Last IsNot Nothing Then
                _nWinAmount *= CalcRate(_olstTicketBets.Last.BetPoint + _olstTicketBets.Last.AddPointMoney)
            End If

            _nWinAmount -= _nRiskAmount
            _nWinAmount = Math.Round(_nWinAmount, 2)
        End Sub

        Public Sub CalcRiskAmountByWinAmount()

            _nWinAmountInit = _nBetAmount
            _nRiskAmountInit = (_nWinAmountInit * _nRiskAmount) / _nWinAmount
            _nRiskAmountInit = Math.Round(_nRiskAmountInit, 2)
            _nRiskAmount = _nRiskAmountInit
            _nWinAmount = _nBetAmount


        End Sub
#End Region

#Region "IF BET"
        ''' <summary>
        ''' Use to generate Reverse's options
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseIfBetOptions() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            Dim oDicItem As New DictionaryEntry("If Win", "If Win Only")
            olstDic.Add(oDicItem)

            oDicItem = New DictionaryEntry("If Win or Push", "If Win or Push")
            olstDic.Add(oDicItem)

            Return olstDic
        End Function
#End Region

        Public Function CompareTo(ByVal poTicket As CTicket) As Integer Implements System.IComparable(Of CTicket).CompareTo
            Return TicketID.CompareTo(poTicket.TicketID)
        End Function
    End Class

    <Serializable()> _
    Public Class CTicketList
        Inherits List(Of CTicket)

        Public Function GetTicketByID(ByVal psTicketID As String) As CTicket
            Return Me.Find(Function(x) UCase(x.TicketID) = UCase(psTicketID))

        End Function

        Public Function RemoveByID(ByVal psTicketID As String) As CTicket
            Dim poTicket As CTicket = GetTicketByID(psTicketID)

            If poTicket IsNot Nothing Then
                Me.Remove(poTicket)
            End If

            Return poTicket
        End Function

        Public Function TotalAmount() As Double
            Dim nAmount As Double = 0

            For Each oTicket As CTicket In Me
                nAmount += oTicket.RiskAmount
            Next

            Return nAmount
        End Function
    End Class
End Namespace