Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Tickets

Imports WebsiteLibrary

Namespace SBSWebsite
    Partial Class Wagers
        Inherits SBCBL.UI.CSBCUserControl

        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private _sTeaser As String = "Teaser"
        Private _TeaserValue As String = "TeaserValue"
        Private _sParlay As String = "Parlay"
        Private _sReverse As String = "Reverse"
        Private _sStraight As String = "Straight"
        Private _sBetTheBoard As String = "BetTheBoard"
        Public Enum EHandler
            ClearWager
        End Enum
        Public Event BackButtonClick()
        Public Event CustomEvent(ByVal psType As EHandler)

#Region "Properties"
        Public ReadOnly Property IfBetNotify() As String
            Get
                If LCase(BetTypeActive).Contains("if ") Then
                    Dim nIndex As Integer = 1
                    If UserSession.SelectedTicket(SelectedPlayerID) IsNot Nothing Then
                        nIndex = UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count
                    End If
                    Return String.Format(" for your <b>{0}</b> selection", GetOrdinalNumber(nIndex))
                End If

                Return ""
            End Get
        End Property

        Public ReadOnly Property BetTypeActive() As String
            Get
                If SafeString(Session("BetTypeActive")).Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) Then
                    Return "BetTheBoard"
                Else
                    Return SafeString(Session("BetTypeActive"))

                End If
            End Get
        End Property


        Public ReadOnly Property BetTypeA() As String
            Get
                Return SafeString(Session("BetTypeActive"))
            End Get
        End Property

        Public ReadOnly Property IncreaseSpread() As Integer
            Get
                If UserSession.UserType = SBCBL.EUserType.Player Then
                    Return UserSession.PlayerUserInfo.IncreaseSpreadMoney
                Else
                    Return SelectedPlayer.IncreaseSpreadMoney
                End If
            End Get
        End Property

        Public Property BetError() As Boolean
            Get
                Return SafeBoolean(ViewState("BetError"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("BetError") = value
            End Set
        End Property

        Private ReadOnly Property CanBetting() As Boolean
            Get
                Return UserSession.SysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable") OrElse _
                    UserSession.SysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable")
            End Get
        End Property

        Public Property SelectedPlayerID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Player Then
                    Return UserSession.UserID
                Else
                    Return SafeString(ViewState("_SELECTED_PLAYER_ID"))
                End If
            End Get
            Set(ByVal value As String)
                ViewState("_SELECTED_PLAYER_ID") = value
            End Set
        End Property

        Public ReadOnly Property SelectedPlayer() As CPlayer
            Get
                If UserSession.UserType = SBCBL.EUserType.Player Then
                    Return UserSession.PlayerUserInfo
                Else
                    Return UserSession.Cache.GetPlayerInfo(SelectedPlayerID)
                End If
            End Get
        End Property

        Private ReadOnly Property SiteType() As SBCBL.CEnums.ESiteType
            Get
                Dim oCache As New SBCBL.CacheUtils.CCacheManager()
                Dim eSiteType As SBCBL.CEnums.ESiteType = oCache.GetSiteType(Request.Url.Host)
                Return eSiteType
            End Get
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindWagers()

            End If
            ClearMessage()
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If SiteType = SBCBL.CEnums.ESiteType.SBS Then
                btnNewWager.Visible = False
                ' btnBackWager.Visible = UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count = 0
            End If
        End Sub
#Region "Wager"

#Region "Main Repeater"

        Public Sub BindWagers()
            'LogDebug(_log, "thuong" & UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count)
            'ClientAlert("thuong" & UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count, True)
            rptTickets.DataSource = UserSession.SelectedTicket(SelectedPlayerID).Tickets
            rptTickets.DataBind()
            btnCancel.Visible = True
            txtSameAmount.Text = ""
            If UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count >= 1 Then

                ' ClientAlert(BetTypeA, True)
                'ClientAlert(UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).IsForProp, True)
                If Not UserSession.SelectedTicket(SelectedPlayerID).Preview AndAlso Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso BetTypeActive.Equals(_sBetTheBoard) AndAlso Not UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).IsForProp Then
                    pnSameAmount.Visible = True
                    'ClientAlert(UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).IsForProp, True)
                ElseIf BetTypeActive.Equals(_sBetTheBoard) Then

                    'If Not oTicket.IsForProp Then
                    '    rptTicketBets.Items(0).FindControl("tdAmount2").Visible = True
                    '    txtAmount.Visible = True
                    'End If
                End If
            End If
        End Sub

        Protected Sub rptTickets_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptTickets.ItemCommand
            Select Case UCase(SafeString(e.CommandName))
                Case "DEL_TICKET"
                    UserSession.SelectedTicket(SelectedPlayerID).RemoveTickets(SafeString(e.CommandArgument))

                Case Else
                    '' Error
            End Select
            If Not BetError Then
                BindWagers()
            End If
            BetError = False
        End Sub

        Public Sub SaveAmount()
            '' Save BetAmount
            For Each rptItem As RepeaterItem In rptTickets.Items
                If rptItem.ItemType = ListItemType.AlternatingItem OrElse rptItem.ItemType = ListItemType.Item Then
                    Dim txtBet As TextBox = CType(rptItem.FindControl("txtBet"), TextBox)
                    Dim txtBetParlay As TextBox = CType(rptItem.FindControl("txtBetParlay"), TextBox)
                    Dim txtWin As TextBox = CType(rptItem.FindControl("txtWin"), TextBox)
                    Dim sTicketID As String = txtBet.Attributes.Item("TicketID")
                    Dim oTicket As CTicket = UserSession.SelectedTicket(SelectedPlayerID).GetTicket(sTicketID)

                    If oTicket IsNot Nothing Then
                        Select Case UCase(oTicket.TicketType)
                            Case "PARLAY"
                                Dim ddlType As CDropDownList = CType(rptItem.FindControl("ddlType"), CDropDownList)
                                oTicket.TicketOption = ddlType.Value
                                oTicket.TicketOptionText = ddlType.SelectedItem.Text

                                '' Save BuyPoints
                                Dim rptTicketBets As Repeater = CType(rptItem.FindControl("rptTicketBets"), Repeater)
                                saveBuyPoints(rptTicketBets, oTicket)

                                oTicket.BetAmount = SafeDouble(txtBetParlay.Text)

                            Case "REVERSE"

                                Dim ddlType As CDropDownList = CType(rptItem.FindControl("ddlType"), CDropDownList)
                                oTicket.TicketOption = ddlType.Value

                                '' Save BuyPoints
                                Dim rptTicketBets As Repeater = CType(rptItem.FindControl("rptTicketBets"), Repeater)
                                saveBuyPoints(rptTicketBets, oTicket)

                                '' ORIGINAL CODE
                                Dim nWinAmount As Double = CType(rptItem.FindControl("txtWin"), TextBox).Text
                                Dim nBetAmount As Double = 0
                                Dim nNum As Integer = 0
                                For Each oTicketBet As CTicketBet In oTicket.TicketBets
                                    nBetAmount += (CalcRate(oTicketBet.BetPoint + oTicketBet.AddPointMoney) - 1)
                                    nNum += 1
                                Next

                                If nNum > 1 Then
                                    nBetAmount = Math.Round(nWinAmount / (nBetAmount * 2 * (nNum - 1)), 2)
                                    oTicket.BetAmount = nBetAmount

                                    If txtBet IsNot Nothing AndAlso txtBet.Visible Then
                                        ' save original bet amount for validate
                                        oTicket.OriginalBetAmount = SafeDouble(txtBet.Text)
                                    End If
                                End If

                            Case "IF BET"
                                '' Save BuyPoints
                                ' Dim rptTicketBets As Repeater = CType(rptItem.FindControl("rptTicketBets"), Repeater)
                                ' LogDebug(_log, "thuong" & oTicket.TicketBets(0).AddPoint)
                                'LogDebug(_log, "thuong" & oTicket.TicketBets(0).AddPointMoney)

                                ' saveBuyPoints(rptTicketBets, oTicket)
                                'LogDebug(_log, "thuong" & oTicket.TicketBets(0).AddPoint)
                                'LogDebug(_log, "thuong" & oTicket.TicketBets(0).AddPointMoney)

                                If oTicket.SetWinAmount Then
                                    oTicket.BetAmount = SafeDouble(txtBet.Text)
                                Else
                                    oTicket.BetAmount = SafeDouble(txtWin.Text)

                                End If
                            Case "STRAIGHT"
                                '' Save BuyPoints
                                Dim rptTicketBets As Repeater = CType(rptItem.FindControl("rptTicketBets"), Repeater)
                                Dim txtAmount As TextBox = CType(rptTicketBets.Items(0).FindControl("txtAmount"), TextBox)
                                'Dim txtAmount As TextBox = CType(rptTicketBets.Items(0).FindControl("txtAmount"), TextBox)
                                'Dim tdAmount As HtmlTableCell = CType(rptTicketBets.Items(0).FindControl("tdAmount"), HtmlTableCell)
                                'Dim txtSameAmount As TextBox = CType(rptTickets.Items(rptTickets.Items.Count - 1).FindControl("txtSameAmount"), TextBox)
                                If Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso Not oTicket.IsForProp Then
                                    rptTicketBets.Items(0).FindControl("tdAmount2").Visible = True
                                    txtAmount.Visible = True
                                End If

                                'ClientAlert(SafeString(rptTicketBets.FindControl("trTicketBet").ID), True)
                                Dim ddlType As CDropDownList = CType(rptItem.FindControl("ddlType"), CDropDownList)
                                saveBuyPoints(rptTicketBets, oTicket)

                                oTicket.TicketOption = ddlType.Value
                                If Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso Not oTicket.IsForProp Then
                                    If chkCheckAmount.Checked Then
                                        oTicket.BetAmount = SafeDouble(txtSameAmount.Text)
                                    Else
                                        oTicket.BetAmount = SafeDouble(txtAmount.Text)
                                    End If
                                End If


                            Case "TEASER"
                                Dim ddlType As CDropDownList = CType(rptItem.FindControl("ddlType"), CDropDownList)
                                oTicket.TicketOption = ddlType.Value
                                oTicket.BetAmount = SafeDouble(txtBet.Text)
                        End Select

                    End If
                End If
            Next
        End Sub

        Private Sub saveBuyPoints(ByVal prptTicketBet As Repeater, ByRef poTicket As CTicket)
            For Each orptItem As RepeaterItem In prptTicketBet.Items
                If orptItem.ItemType = ListItemType.AlternatingItem OrElse orptItem.ItemType = ListItemType.Item Then
                    Dim ddlBuyPoint As CDropDownList = CType(orptItem.FindControl("ddlBuyPoint"), CDropDownList)
                    Dim btnDelTicketBet As Button = CType(orptItem.FindControl("btnDelTicketBet"), Button)
                    Dim sTicketBetID As String = btnDelTicketBet.CommandArgument.Split("|")(1)
                    Dim oTicketBet As CTicketBet = poTicket.GetTicketBet(sTicketBetID)
                    'ClientAlert(ddlBuyPoint.Value, True)
                    If ddlBuyPoint.Value <> "" Then
                        Dim oBuyPoints As String() = ddlBuyPoint.Value.Split("|")

                        oTicketBet.AddPoint = SafeDouble(oBuyPoints(0))
                        oTicketBet.AddPointMoney = SafeDouble(oBuyPoints(1))
                    Else
                        oTicketBet.AddPoint = 0
                        oTicketBet.AddPointMoney = 0
                    End If
                End If
            Next
        End Sub

        Protected Sub rptTickets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTickets.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim oTicket As CTicket = CType(e.Item.DataItem, CTicket)
                Dim btnNextWager As Button = CType(e.Item.FindControl("btnNextWager"), Button)
                Dim btnPreview As Button = CType(e.Item.FindControl("btnPreview"), Button)
                Dim btnContinue As Button = CType(e.Item.FindControl("btnContinue"), Button)

                ' Invisible TicketType if Current Ticket is If Bet
                'Dim trTicketType As HtmlControl = CType(e.Item.FindControl("trTicketType"), HtmlControl)
                'If UCase(oTicket.TicketType) = "IF BET" AndAlso oTicket.RelatedTicketID <> "" AndAlso oTicket.RelatedTicketID <> oTicket.TicketID Then
                '    trTicketType.Visible = False
                'End If

                '' Bind SubTicketBet
                Dim rptSub As Repeater = CType(e.Item.FindControl("rptTicketBets"), Repeater)
                If rptSub IsNot Nothing Then
                    rptSub.DataSource = oTicket.TicketBets
                    rptSub.DataBind()
                End If

                '' Bind Type
                Dim ddlType As CDropDownList = CType(e.Item.FindControl("ddlType"), CDropDownList)
                If UCase(oTicket.TicketType) <> "STRAIGHT" AndAlso UCase(oTicket.TicketType) <> "IF BET" Then
                    Dim lblWagerName As Literal = CType(e.Item.FindControl("lblWagerName"), Literal)
                    ' lblWagerName.Text = oTicket.NumOfTicketBets & " Team(s) " & lblWagerName.Text
                    ''ticket Option init for first load
                    ddlType.Visible = SBCBL.std.CheckRoundRobin(UserSession.PlayerUserInfo.SuperAgentID)
                    ddlType.DataSource = oTicket.ParseOptions
                    ddlType.DataTextField = "Value"
                    ddlType.DataValueField = "Key"
                    ddlType.DataBind()
                    If oTicket.TicketOption <> "" Then
                        ddlType.Value = oTicket.TicketOption
                    Else
                        If BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) Then
                            Dim oTeaserRule As CTeaserRule
                            oTeaserRule = CType(Session(_TeaserValue), CTeaserRule)
                            ddlType.SelectedValue = oTeaserRule.TeaserRuleID
                        Else
                            ddlType.SelectedIndex = 0
                        End If
                    End If
                Else
                    ddlType.Visible = False
                End If

                Dim lblResult As Label = CType(e.Item.FindControl("lblResult"), Label)
                Dim lblRiskDsp As Label = CType(e.Item.FindControl("lblRiskDsp"), Label)
                Dim lblWinDsp As Label = CType(e.Item.FindControl("lblWinDsp"), Label)
                Dim txtWin As TextBox = CType(e.Item.FindControl("txtWin"), TextBox)
                Dim txtBet As TextBox = CType(e.Item.FindControl("txtBet"), TextBox)

                '' Add javascript event
                If oTicket.NumOfTicketBets > 0 Then
                    Dim sClickAction As String = "javascript: "
                    Select Case UCase(oTicket.TicketType)
                        Case "PARLAY"
                            btnNextWager.Visible = False
                            sClickAction &= "CalcParlay(document.getElementById('{0}'), {1}, document.getElementById('{2}'), document.getElementById('{3}'));"
                            sClickAction = String.Format(sClickAction, txtBet.ClientID, SQLString(oTicket.TicketID), ddlType.ClientID, _
                                                          lblResult.ClientID)
                            ddlType.Attributes.Add("onchange", sClickAction)

                            Dim ddlBuyPoint As CDropDownList = Nothing
                            For Each rptItem As RepeaterItem In rptSub.Items
                                If rptItem.ItemType = ListItemType.AlternatingItem OrElse rptItem.ItemType = ListItemType.Item Then
                                    ddlBuyPoint = CType(rptItem.FindControl("ddlBuyPoint"), CDropDownList)
                                    ddlBuyPoint.Attributes.Add("onblur", sClickAction)
                                End If
                            Next

                            'lblRiskDsp.Text = "Bet: "
                            lblRiskDsp.Visible = False
                            lblWinDsp.Visible = False
                            txtWin.Visible = False

                        Case "REVERSE"
                            lblResult.Visible = False
                            btnNextWager.Visible = False
                            sClickAction &= "CalcBetReverse(this, {0}, document.getElementById('{1}'));"
                            sClickAction = String.Format(sClickAction, SQLString(oTicket.TicketID), txtBet.ClientID)
                            txtWin.Attributes.Add("onblur", sClickAction)

                            sClickAction = "javascript: "
                            sClickAction &= "CalcWinReverse(this, {0}, document.getElementById('{1}'));"
                            sClickAction = String.Format(sClickAction, SQLString(oTicket.TicketID), txtWin.ClientID)

                            'lblRiskDsp.Text = "Bet: "
                            lblRiskDsp.Visible = False
                            Dim rdRiskAmount As RadioButton = CType(e.Item.FindControl("rdRiskAmount"), RadioButton)
                            '   Dim rdWinAmount As RadioButton = CType(e.Item.FindControl("rdWinAmount"), RadioButton)
                            rdRiskAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',true)", txtBet.ClientID, txtWin.ClientID))
                            '  rdWinAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',false)", txtBet.ClientID, txtWin.ClientID))

                            'Case "STRAIGHT", "BETTHEBOARD"
                            '    CType(e.Item.FindControl("rdRiskAmount"), RadioButton).Visible = False
                            '    CType(e.Item.FindControl("lblRiskAmmount"), Literal).Text = "<input type='checkbox' checked='checked'/>Use same amount for All Bets"
                        Case "STRAIGHT", "IF BET"
                            lblResult.Visible = False
                            If UCase(oTicket.TicketType) = "STRAIGHT" Then
                                btnNextWager.Visible = False
                                If Not UserSession.SelectedTicket(SelectedPlayerID).Preview AndAlso Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso Not oTicket.IsForProp Then
                                    rptSub.Items(0).FindControl("tdAmount").Visible = True
                                End If
                            End If
                            Dim ddlBuyPoint As CDropDownList = Nothing
                            For Each rptItem As RepeaterItem In rptSub.Items
                                If rptItem.ItemType = ListItemType.AlternatingItem OrElse rptItem.ItemType = ListItemType.Item Then
                                    ddlBuyPoint = CType(rptItem.FindControl("ddlBuyPoint"), CDropDownList)
                                End If
                            Next

                            sClickAction &= "CalcRiskStraight(this, document.getElementById('{0}'), {1}, document.getElementById('{2}'));"
                            sClickAction = String.Format(sClickAction, ddlBuyPoint.ClientID, SQLString(oTicket.TicketID), txtBet.ClientID)
                            txtWin.Attributes.Add("onblur", sClickAction)

                            sClickAction = "javascript: "
                            sClickAction &= "CalcWinStraight(document.getElementById('{0}'), this, {1}, document.getElementById('{2}'));"
                            sClickAction = String.Format(sClickAction, txtBet.ClientID, SQLString(oTicket.TicketID), txtWin.ClientID)
                            ddlBuyPoint.Attributes.Add("onblur", sClickAction)

                            sClickAction = "javascript: "
                            sClickAction &= "CalcWinStraight(this, document.getElementById('{0}'), {1}, document.getElementById('{2}'));"
                            sClickAction = String.Format(sClickAction, ddlBuyPoint.ClientID, SQLString(oTicket.TicketID), txtWin.ClientID)

                            lblRiskDsp.Text = "Risk: "

                            Dim rdRiskAmount As RadioButton = CType(e.Item.FindControl("rdRiskAmount"), RadioButton)
                            ' Dim rdWinAmount As RadioButton = CType(e.Item.FindControl("rdWinAmount"), RadioButton)
                            rdRiskAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',true)", txtBet.ClientID, txtWin.ClientID))
                            ' rdWinAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',false)", txtBet.ClientID, txtWin.ClientID))

                        Case "TEASER"
                            lblResult.Visible = False
                            btnNextWager.Visible = False
                            sClickAction &= "CalcTeaser(this, {0}, {1}, {2});"
                            sClickAction = String.Format(sClickAction, SQLString(oTicket.TicketID), _
                                                         "document.getElementById('" & txtBet.ClientID & "')", _
                                                         "document.getElementById('" & txtWin.ClientID & "')")
                            ddlType.Attributes.Add("onchange", sClickAction)

                            sClickAction = "javascript: "
                            sClickAction &= "CalcRiskTeaser(this, {0}, document.getElementById('{1}'), document.getElementById('{2}'));"
                            sClickAction = String.Format(sClickAction, SQLString(oTicket.TicketID), ddlType.ClientID, _
                                                          txtBet.ClientID)
                            txtWin.Attributes.Add("onblur", sClickAction)

                            sClickAction = "javascript: "
                            sClickAction &= "CalcWinTeaser(this, {0}, document.getElementById('{1}'), document.getElementById('{2}'));"
                            sClickAction = String.Format(sClickAction, SQLString(oTicket.TicketID), ddlType.ClientID, _
                                                           txtWin.ClientID)

                            lblRiskDsp.Text = "Risk: "
                            Dim rdRiskAmount As RadioButton = CType(e.Item.FindControl("rdRiskAmount"), RadioButton)
                            '' Dim rdWinAmount As RadioButton = CType(e.Item.FindControl("rdWinAmount"), RadioButton)
                            rdRiskAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',true)", txtBet.ClientID, txtWin.ClientID))
                            'rdWinAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',false)", txtBet.ClientID, txtWin.ClientID))
                    End Select

                    txtBet.Attributes.Add("onblur", sClickAction)
                    txtBet.Attributes.Add("TicketID", oTicket.TicketID)
                    txtWin.Attributes.Add("TicketID", oTicket.TicketID)
                End If

                '' Invalid Ticket
                If UserSession.SelectedTicket(SelectedPlayerID).InvalidTickets.Contains(oTicket.TicketID) Then
                    Dim trTicket As HtmlControl = CType(e.Item.FindControl("trTicket"), HtmlControl)
                    If trTicket IsNot Nothing Then
                        trTicket.Attributes.Add("style", "color: Red;")
                    End If
                End If
                Dim lblSperateStraight = CType(e.Item.FindControl("lblSperateStraight"), Literal)
                Dim pnOptionWager = e.Item.FindControl("pnOptionWager")
                Dim lblRiskWin As Literal = e.Item.FindControl("lblRiskWin")
                If BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sReverse, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Contains("If") Then
                    If (oTicket.RiskAmount <> 0 AndAlso oTicket.WinAmount <> 0) Then

                        lblRiskWin.Text = "<td colspan='2' style='text-align:Right'>Amount :  Risking " & FormatCurrency(SafeRound(oTicket.RiskAmount), GetRoundMidPoint).Replace("$", "") & " to Win " & FormatCurrency(SafeRound(oTicket.WinAmount), GetRoundMidPoint).Replace("$", "") & " Unit</td>"
                        lblRiskWin.Visible = True
                    End If
                End If
                If BetTypeActive.Contains("If ") Then
                    If (oTicket.RiskAmount <> 0 AndAlso oTicket.WinAmount <> 0) Then
                        e.Item.FindControl("noticeStraight").Visible = False
                        e.Item.FindControl("pnOptionWager").Visible = False
                    Else
                        e.Item.FindControl("noticeStraight").Visible = True
                        lblRiskWin.Text = "<td colspan='3' style='text-align:Right'></td>"
                    End If
                End If
                If (BetTypeActive.Equals("BetTheBoard", StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase)) Then
                    If (oTicket.RiskAmount <> 0 AndAlso oTicket.WinAmount <> 0) AndAlso (BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) OrElse oTicket.IsForProp) Then

                        lblRiskWin.Text = "<td colspan='2' style='text-align:Right'>Amount :  Risking " & FormatCurrency(SafeRound(oTicket.RiskAmount), GetRoundMidPoint).Replace("$", "") & " to Win " & FormatCurrency(SafeRound(oTicket.WinAmount), GetRoundMidPoint).Replace("$", "") & " Unit</td>"

                    ElseIf BetTypeActive.Equals("BetTheBoard", StringComparison.CurrentCultureIgnoreCase) Then

                        If (oTicket.RiskAmount <> 0 AndAlso oTicket.WinAmount <> 0) Then
                            lblRiskWin.Text = "<td colspan='2' style='text-align:Right'>Amount :  Risking " & FormatCurrency(SafeRound(oTicket.RiskAmount), GetRoundMidPoint).Replace("$", "") & " to Win " & FormatCurrency(SafeRound(oTicket.WinAmount), GetRoundMidPoint).Replace("$", "") & " Unit</td>"
                        Else
                            ' lblRiskWin.Text = "<td colspan='3' style='text-align:Right'></td>"

                        End If


                    Else

                        lblRiskWin.Text = "<td colspan='2' style='text-align:Right'>Amount :  Risking " & FormatCurrency(SafeRound(oTicket.RiskAmount), GetRoundMidPoint).Replace("$", "") & " to Win " & FormatCurrency(SafeRound(oTicket.WinAmount), GetRoundMidPoint).Replace("$", "") & " Unit</td>"

                    End If
                    lblRiskWin.Visible = True
                    pnOptionWager.Visible = False
                    '  lblSperateStraight.Text = "<hr style='position:relative;top:3px'/>"
                    btnSubmit.Visible = True
                    txtPassword.Visible = True
                    lblMessage.Visible = True
                    pnSameAmount.Visible = False
                    tblBetTheboard.Visible = True
                    'ClientAlert(BetTypeActive, True)
                    If Not BetTypeA.Equals("BetIfAll") Then


                        If UserSession.SelectedTicket(SelectedPlayerID).Preview Then
                            btnSubmit.Visible = True
                            txtPassword.Visible = True
                            lblMessage.Visible = True
                        Else
                            pnSameAmount.Visible = True
                            btnSubmit.Visible = False
                            txtPassword.Visible = False
                            lblMessage.Visible = False
                        End If
                    End If

                    If (Not SBCBL.std.SafeString(Session("BetTypeActive")).Equals("Straight") AndAlso Not SBCBL.std.SafeString(Session("BetTypeActive")).Equals("BetTheBoard") AndAlso Not SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If")) Then
                        CType(e.Item.FindControl("tableEnd"), Literal).Text = "</table> </td></tr></table>"
                    End If
                    'ElseIf oTicket.RiskAmount = 0 AndAlso oTicket.WinAmount = 0 Then
                    '    CType(e.Item.FindControl("tableEnd"), Literal).Text = "</table></td></tr></table>"
                Else

                    ' show limit
                    Dim lblBetLimits As Label = e.Item.FindControl("lblBetLimits")

                    If lblBetLimits IsNot Nothing Then
                        Dim oLstGameContext As New List(Of String)
                        Dim oLstGameType As New List(Of String)
                        Dim sContext As String
                        For Each oTicketBet As CTicketBet In oTicket.TicketBets
                            oLstGameType.Add(oTicketBet.GameType)

                            If oTicketBet.BetType.Equals("Spread") Then
                                sContext = oTicketBet.Context
                            Else
                                sContext = oTicketBet.BetType
                            End If
                            If Not oLstGameContext.Contains(sContext) Then
                                oLstGameContext.Add(sContext)
                            End If
                        Next

                        Dim maxBetLimit = oTicket.getMaxBetAmount(oLstGameContext, oLstGameType, SelectedPlayer.Template)
                        Dim minBetLimit = oTicket.getMinBetAmount(SelectedPlayer.Template, UserSession.UserTypeOfBet)

                        lblBetLimits.Text = String.Format("Maximum Wager: {0}. Minimum Wager: {1}", FormatCurrency(maxBetLimit), FormatCurrency(minBetLimit))
                    End If

                    If Not SBCBL.std.SafeString(Session("BetTypeActive")).Contains("If") Then
                        CType(e.Item.FindControl("tableEnd"), Literal).Text = "</table></td></tr></table>" '"<tr><td>"
                    End If
                    If BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) Then
                        CType(e.Item.FindControl("ddlType"), CDropDownList).Style.Add("display", "none")
                    End If
                    'lblRiskWin.Visible = False
                    'pnOptionWager.Visible = True
                    If BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) Then
                        CType(e.Item.FindControl("noticeParlay"), Panel).Visible = True
                        'CType(e.Item.FindControl("noticeStraight"), Panel).Visible = False
                    ElseIf BetTypeActive.Equals(_sReverse, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) Then
                        CType(e.Item.FindControl("noticeStraight"), Panel).Visible = True
                        CType(e.Item.FindControl("noticeParlay"), Panel).Visible = False
                        btnContinue.Visible = False
                    Else
                        'CType(e.Item.FindControl("noticeStraight"), Panel).Visible = True
                        CType(e.Item.FindControl("noticeParlay"), Panel).Visible = False
                        btnPreview.Visible = False

                        If LCase(BetTypeActive).Contains("if ") Then
                            'btnContinue.CssClass = ""
                            btnContinue.Text = "Finished"
                            btnNextWager.Visible = Not UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count > 1
                            btnContinue.Visible = UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count > 1
                        Else
                            btnNextWager.Visible = False
                        End If
                    End If

                    btnSubmit.Visible = False
                    txtPassword.Visible = False
                    tblBetTheboard.Visible = False

                    If UserSession.SelectedTicket(SelectedPlayerID).Preview Then
                        btnSubmit.Visible = True
                        txtPassword.Visible = True
                        tblBetTheboard.Visible = True
                        pnOptionWager.Visible = False
                    End If

                End If




            End If
        End Sub

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
#End Region

#Region "Sub Repeater"

        Protected Sub rptTicketBets_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
            Select Case UCase(SafeString(e.CommandName))
                Case "DEL_TICKETBET"
                    Dim oID As String() = SafeString(e.CommandArgument).Split("|"c)
                    Dim oTicket As CTicket = UserSession.SelectedTicket(SelectedPlayerID).GetTicket(oID(0))
                    If oTicket IsNot Nothing Then
                        If oTicket.TicketBets.Count > 1 Then
                            oTicket.RemoveTicketBets(oID(1))
                        Else
                            UserSession.SelectedTicket(SelectedPlayerID).RemoveTickets(oID(0))

                        End If
                    Else

                    End If
                    'Try
                    '    If UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count = 0 Then
                    '        e.Item.FindControl("trTicketBet").Visible = False
                    '    End If
                    'Catch ex As Exception
                    '    e.Item.FindControl("trTicketBet").Visible = False

                    'End Try

                Case Else
                    '' Error
            End Select
            BindWagers()
        End Sub

        Public Function addPointTeaser(ByVal pnPoint As Double, ByVal psGameType As String, ByVal pbaddPoint As Boolean, ByVal pnAddPoint As Double) As Double
            If pnAddPoint <> 0 OrElse Not BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) Then
                Return pnPoint
            End If
            Dim oTeaserRule As CTeaserRule
            oTeaserRule = CType(Session(_TeaserValue), CTeaserRule)
            If IsFootball(psGameType) Then
                If pbaddPoint Then
                    pnPoint += oTeaserRule.FootbalPoint
                Else
                    pnPoint -= oTeaserRule.FootbalPoint
                End If
            End If
            If IsBasketball(psGameType) Then
                If pbaddPoint Then
                    pnPoint += oTeaserRule.BasketballPoint
                Else
                    pnPoint -= oTeaserRule.BasketballPoint
                End If
            End If
            Return pnPoint
        End Function

        Protected Sub rptTicketBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim oTicketBet As CTicketBet = CType(e.Item.DataItem, CTicketBet)

                Dim lblLine As Label = CType(e.Item.FindControl("lblLine"), Label)
                If lblLine IsNot Nothing Then
                    lblLine.Attributes.Add("Wager", oTicketBet.TicketID)
                    lblLine.Attributes.Add("GameType", oTicketBet.GameType)


                    '' Bind BuyPoint
                    Dim ddlBuyPoint As CDropDownList = CType(e.Item.FindControl("ddlBuyPoint"), CDropDownList)
                    Dim sTicketBetType As String = "PARLAY"
                    Dim oTicket = UserSession.SelectedTicket(SelectedPlayerID).GetTicket(oTicketBet.TicketID)
                    If oTicket IsNot Nothing Then
                        sTicketBetType = oTicket.TicketType
                    End If
                    Select Case UCase(sTicketBetType)
                        Case "STRAIGHT"
                            'ClientAlert(oTicket.IsForProp, True)
                            e.Item.FindControl("tdAmount").Visible = False
                            If oTicket.IsForProp Then
                                e.Item.FindControl("tdAmount").Visible = False
                                e.Item.FindControl("tdAmount2").Visible = False
                            ElseIf Not UserSession.SelectedTicket(SelectedPlayerID).Preview AndAlso Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) Then

                                'ClientAlert(e.Item.FindControl("txtAmount").ID, True)
                                e.Item.FindControl("tdAmount2").Visible = True
                                e.Item.FindControl("tdAmount").Visible = True
                                e.Item.FindControl("txtAmount").Visible = True
                            End If
                        Case "TEASER" ', "REVERSE", "PARLAY", "IF BET"
                            e.Item.FindControl("tdAmount").Visible = False
                        Case "STRAIGHT", "REVERSE", "PARLAY", "IF BET"
                            'ClientAlert("" & oTicketBet.ParseBuyPointOptions.Count, True)
                            Dim olstBuyPoint As List(Of DictionaryEntry) = oTicketBet.ParseBuyPointOptions
                            If olstBuyPoint.Count > 0 Then
                                ddlBuyPoint.DataSource = oTicketBet.ParseBuyPointOptions
                                ddlBuyPoint.DataValueField = "Value"
                                ddlBuyPoint.DataTextField = "Key"
                                ddlBuyPoint.DataBind()
                                If BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) Then
                                    ddlBuyPoint.Style.Add("display", "none")
                                    CType(e.Item.FindControl("lblBuyPoint"), Literal).Text = safeVegass(oTicketBet.ParseBuyPointOptions(0).Key).ToString() '.Replace("0½", "½")
                                End If
                                If Not String.IsNullOrEmpty(oTicketBet.BuyPointValue) Then
                                    If oTicketBet.BuyPointValue.Split("|").Count > 1 AndAlso SafeInteger(oTicketBet.BuyPointValue.Split("|")(1)) > -1 Then
                                        CType(e.Item.FindControl("lblBuyPoint"), Literal).Text = safeVegass(oTicketBet.ParseBuyPointOptions(SafeInteger(oTicketBet.BuyPointValue.Split("|")(1))).Key).ToString().Replace("0½", "½")
                                        ddlBuyPoint.SelectedIndex = SafeInteger(oTicketBet.BuyPointValue.Split("|")(1))
                                    End If
                                End If

                                If oTicketBet.AddPoint <> 0 Then
                                    Dim sValue As String = SafeString(oTicketBet.AddPoint) & "|" & SafeString(oTicketBet.AddPointMoney)
                                    ddlBuyPoint.Value = sValue
                                End If

                                lblLine.Visible = False
                                ddlBuyPoint.Visible = True
                            Else
                                ddlBuyPoint.Visible = True
                            End If

                        Case Else
                            ddlBuyPoint.Visible = True
                    End Select

                    Select Case UCase(oTicketBet.BetType)
                        Case "MONEYLINE"
                            lblLine.Attributes.Add("Status", "")
                            lblLine.Attributes.Add("Point", "")
                            If oTicketBet.IsForProp Then
                                lblLine.Text = FormatPositiveNumber(SafeString(oTicketBet.BetPoint + oTicketBet.AddPointMoney))
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.BetPoint))
                            ElseIf Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) Then

                                lblLine.Text = "Money Line "
                                If oTicketBet.HomeMoneyLine <> 0 Then
                                    lblLine.Text += FormatPositiveNumber(SafeString(oTicketBet.HomeMoneyLine + oTicketBet.AddPointMoney))
                                    lblLine.Attributes.Add("Money", SafeString(oTicketBet.HomeMoneyLine))

                                Else
                                    lblLine.Text += FormatPositiveNumber(SafeString(oTicketBet.AwayMoneyLine + oTicketBet.AddPointMoney))
                                    lblLine.Attributes.Add("Money", SafeString(oTicketBet.AwayMoneyLine))
                                End If
                            End If
                            If IsBaseball(oTicketBet.GameType) Then
                                lblLine.Text += " &nbsp; " & oTicketBet.AwayPitcher & "-" & SafeString(IIf(oTicketBet.AwayPitcherRH, "R", "L")) & IIf(oTicketBet.IsCheckPitcher, " - must Start", " - Action") & " &nbsp; " & oTicketBet.HomePitcher & "-" & SafeString(IIf(oTicketBet.HomePitcherRH, "R", "L")) & IIf(oTicketBet.IsCheckPitcher, " - must Start", " - Action")
                            End If
                        Case "SPREAD"
                            lblLine.Attributes.Add("Status", "")
                            If oTicketBet.HomeSpreadMoney <> 0 Then
                                lblLine.Text = "Spread "
                                If FormatPoint(addPointTeaser(oTicketBet.HomeSpread, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType).Equals("0") Then
                                    lblLine.Text += "PK (" & FormatPositiveNumber(SafeString(oTicketBet.HomeSpreadMoney + oTicketBet.AddPointMoney)) & ")"
                                Else
                                    lblLine.Text += FormatPositiveNumber(FormatPoint(addPointTeaser(oTicketBet.HomeSpread, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType)) & " (" & FormatPositiveNumber(SafeString(oTicketBet.HomeSpreadMoney + oTicketBet.AddPointMoney)) & ")"
                                End If

                                lblLine.Attributes.Add("Point", SafeString(oTicketBet.HomeSpread))
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.HomeSpreadMoney))
                            Else
                                lblLine.Text = "Spread "
                                If FormatPoint(addPointTeaser(oTicketBet.AwaySpread, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType).Equals("0") Then
                                    lblLine.Text += " PK (" & FormatPositiveNumber(SafeString(addPointTeaser(oTicketBet.AwaySpreadMoney, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPointMoney)) & ")"
                                Else
                                    lblLine.Text += FormatPositiveNumber(FormatPoint(addPointTeaser(oTicketBet.AwaySpread, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType)) & " (" & FormatPositiveNumber(SafeString(oTicketBet.AwaySpreadMoney + oTicketBet.AddPointMoney)) & ")"
                                End If

                                lblLine.Attributes.Add("Point", SafeString(oTicketBet.AwaySpread))
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.AwaySpreadMoney))
                            End If

                        Case "TEAMTOTALPOINTS"
                            lblLine.Attributes.Add("Point", SafeString(oTicketBet.TotalPoints))

                            If oTicketBet.TotalPointsOverMoney <> 0 Then
                                lblLine.Text = "Over " & FormatPoint(oTicketBet.TotalPoints + oTicketBet.AddPoint, oTicketBet.GameType).Replace("+"c, "") & " (" & FormatPositiveNumber(SafeString(oTicketBet.TotalPointsOverMoney + oTicketBet.AddPointMoney)) & ")"
                                lblLine.Attributes.Add("Status", "Over ")
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.TotalPointsOverMoney))
                            Else
                                lblLine.Text = "Under " & FormatPoint(oTicketBet.TotalPoints + oTicketBet.AddPoint, oTicketBet.GameType).Replace("+"c, "") & " (" & FormatPositiveNumber(SafeString(oTicketBet.TotalPointsUnderMoney + oTicketBet.AddPointMoney)) & ")"
                                lblLine.Attributes.Add("Status", "Under ")
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.TotalPointsUnderMoney))
                            End If

                        Case "TOTALPOINTS"
                            lblLine.Attributes.Add("Point", SafeString(oTicketBet.TotalPoints))
                            lblLine.Text = " Total Points "
                            If oTicketBet.TotalPointsOverMoney <> 0 Then
                                lblLine.Text += "OVER " & FormatPoint(addPointTeaser(oTicketBet.TotalPoints, oTicketBet.GameType, False, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType).Replace("+"c, "") & " (" & FormatPositiveNumber(SafeString(oTicketBet.TotalPointsOverMoney + oTicketBet.AddPointMoney)) & ")"
                                lblLine.Attributes.Add("Status", "Over ")
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.TotalPointsOverMoney))
                            Else
                                lblLine.Text += "UNDER " & FormatPoint(addPointTeaser(oTicketBet.TotalPoints, oTicketBet.GameType, True, oTicketBet.AddPoint) + oTicketBet.AddPoint, oTicketBet.GameType).Replace("+"c, "") & " (" & FormatPositiveNumber(SafeString(oTicketBet.TotalPointsUnderMoney + oTicketBet.AddPointMoney)) & ")"
                                lblLine.Attributes.Add("Status", "Under ")
                                lblLine.Attributes.Add("Money", SafeString(oTicketBet.TotalPointsUnderMoney))
                            End If

                        Case "DRAW"
                            lblLine.Attributes.Add("Status", "")
                            lblLine.Attributes.Add("Point", "")
                            lblLine.Text = FormatPositiveNumber(SafeString(oTicketBet.DrawMoneyLine + oTicketBet.AddPointMoney))
                            lblLine.Attributes.Add("Money", SafeString(oTicketBet.DrawMoneyLine))

                        Case Else
                            '' Error
                    End Select
                    'lblLine.Visible = True
                    lblLine.Text = safeVegass(lblLine.Text)
                End If

                '' Invalid Ticketbet
                Dim oTicketValid = UserSession.SelectedTicket(SelectedPlayerID).GetTicket(oTicketBet.TicketID)
                If oTicketValid IsNot Nothing AndAlso oTicketValid.InvalidBets.Contains(oTicketBet.TicketBetID) Then
                    Dim trTicketBet As HtmlControl = CType(e.Item.FindControl("trTicketBet"), HtmlControl)
                    If trTicketBet IsNot Nothing Then
                        trTicketBet.Attributes.Add("style", "color: Red;")
                    End If
                End If
            End If

        End Sub

#End Region

        Private Function FormatPositiveNumber(ByVal psNumber As String) As String
            Return IIf(psNumber.Contains("-"), psNumber, IIf(psNumber.Contains("+"), psNumber, "+" & psNumber))
        End Function
        Protected Sub btnClearWagers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click, btnBack.Click
            'ClientAlert("adasds" + CType(sender, Button).ClientID, True)
            UserSession.SelectedTicket(SelectedPlayerID) = Nothing
            BindWagers()
            ShowWager(True)
            RaiseEvent CustomEvent(EHandler.ClearWager)
            RaiseEvent BackButtonClick()
        End Sub

        'Protected Sub btnCancelGame_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelGame.Click, btnCancel.Click
        '    betPickPanel.Visible = False
        '    noticeCancel.Visible = True
        'End Sub

        Protected Sub btnNewWager_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNewWager.Click
            UserSession.SelectedTicket(SelectedPlayerID).NewTicket(SelectedPlayer.SuperAgentID, SelectedPlayer.UserID)
            SaveAmount()
            BindWagers()
        End Sub

        Protected Sub btnNextWager_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles btnPreview.Click
            If Not CanBetting Then
                ClientAlert("All Betting Has Been Disabled. Please Contact Your Administrator.", True)
                Exit Sub
            End If

            Try
                If UserSession.SelectedTicket(SelectedPlayerID).NumOfTickets > 0 Then
                    SaveAmount()

                    '' require input risk amount
                    For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
                        '' UnCheck empty ticket
                        If oTicket.NumOfTicketBets = 0 Then
                            Continue For
                        End If

                        If oTicket.RiskAmount = 0 Then
                            Throw New CTicketException("The Bet Amount has to be different than 0")
                        End If
                    Next
                    ' validate bet amount
                    'UserSession.SelectedTicket(SelectedPlayerID).Validate(SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID)

                Else
                    ClientAlert("You Don't Have Any Wager. Please Choose One.", True)
                End If

            Catch ex As CTicketException
                ClientAlert(ex.Message, True)
                RunJS("js", "if($('#" + chkCheckAmount.ClientID + "')checked ||$('#pnSameAmount') == null)  $('.amount').hide();", True)
                LogError(_log, "Preview wagers: ", ex)
            End Try
            If Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso Not UserSession.SelectedTicket(SelectedPlayerID).LastTicket().IsForProp AndAlso (chkCheckAmount.Checked) Then
                RunJS("jsw", "$('.amount').hide();", True)
            End If
            If LCase(BetTypeActive).Contains("if ") Then
                Response.Redirect("Default.aspx?bettype=" & BetTypeActive)
                Exit Sub
            End If
        End Sub

        Private Sub setTextBoxRiskWin()
            Dim txtWin As TextBox = CType(rptTickets.Items(0).FindControl("txtWin"), TextBox)
            Dim txtBet As TextBox = CType(rptTickets.Items(0).FindControl("txtBet"), TextBox)
            Dim rdRiskAmount As RadioButton = CType(rptTickets.Items(0).FindControl("rdRiskAmount"), RadioButton)
            ' Dim rdWinAmount As RadioButton = CType(rptTickets.Items(0).FindControl("rdWinAmount"), RadioButton)
            'rdRiskAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',true)", txtBet.ClientID, txtWin.ClientID))
            'rdWinAmount.Attributes.Add("Onclick", String.Format("showRisk('{0}','{1}',false)", txtBet.ClientID, txtWin.ClientID))
            If rdRiskAmount.Checked Then
                txtBet.Style.Add("display", "inline-block")
                txtWin.Style.Add("display", "none")
                'Else
                '    txtBet.Style.Add("display", "none")
                '    txtWin.Style.Add("display", "inline-block")
            End If
        End Sub

        Protected Sub btnPreviewGame_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreviewGame.Click
            If Not CanBetting Then
                ClientAlert("All Betting Has Been Disabled. Please Contact Your Administrator.", True)
                Exit Sub
            End If
            setTextBoxRiskWin()
            Try
                If UserSession.SelectedTicket(SelectedPlayerID).NumOfTickets > 0 Then
                    SaveAmount()

                    '' require input risk amount
                    Dim nIndex As Integer = 1
                    For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
                        '' UnCheck empty ticket
                        If oTicket.NumOfTicketBets = 0 Then
                            Continue For
                        End If

                        If oTicket.RiskAmount = 0 Then
                            Throw New CTicketException("The Bet Amount has to be different than 0")
                        End If
                        nIndex += 1
                    Next
                    lblMessage.Text = "Please Enter Your Password To Confirm !"
                    ''lblMessage.Text = "Please Review Wager Carefully & Re-enter Password To Confirm Wager!"
                    ''UserSession.SelectedTicket(SelectedPlayerID).Preview = True
                    'ShowWager(False)

                    ''valid limit
                    'If UserSession.SelectedTicket(SelectedPlayerID).Validate(UserSession.Cache.GetAllOddsRules(SBCBL.std.GetSiteType), _
                    '    SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID) = ECheckStatus.Update Then
                    If UserSession.SelectedTicket(SelectedPlayerID).Validate(SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID) = ECheckStatus.Update Then

                    Else
                        lblMessage.Text = "Please Enter Your Password To Confirm !"
                        UserSession.SelectedTicket(SelectedPlayerID).Preview = True
                    End If
                Else
                    ClientAlert("You Don't Have Any Wager. Please Choose One.", True)
                End If
                pnSameAmount.Visible = False
                btnSubmit.Visible = True
                txtPassword.Visible = True
                tblBetTheboard.Visible = True
                BindWagers()
            Catch ex As CTicketException
                ClientAlert(ex.Message, True)
                RunJS("js", "if($('#" + chkCheckAmount.ClientID + "')checked ||$('#pnSameAmount') != null)  $('.amount').hide();", True)
                BetError = True
                LogError(_log, "Preview wagers: ", ex)
            End Try

        End Sub

        Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles btnPreview.Click
            If Not CanBetting Then
                ClientAlert("All Betting Has Been Disabled. Please Contact Your Administrator.", True)
                Exit Sub
            End If
            setTextBoxRiskWin()
            Try
                If UserSession.SelectedTicket(SelectedPlayerID).NumOfTickets > 0 Then
                    SaveAmount()

                    '' require input risk amount
                    Dim nIndex As Integer = 1
                    For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
                        '' UnCheck empty ticket
                        If oTicket.NumOfTicketBets = 0 Then
                            Continue For
                        End If

                        If oTicket.RiskAmount = 0 Then
                            Throw New CTicketException("The Bet Amount has to be different than 0")
                        End If
                        nIndex += 1
                    Next
                    lblMessage.Text = "Please Enter Your Password To Confirm !"
                    ''lblMessage.Text = "Please Review Wager Carefully & Re-enter Password To Confirm Wager!"
                    ''UserSession.SelectedTicket(SelectedPlayerID).Preview = True
                    'ShowWager(False)

                    ''valid limit
                    'If UserSession.SelectedTicket(SelectedPlayerID).Validate(UserSession.Cache.GetAllOddsRules(SBCBL.std.GetSiteType), _
                    '    SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID) = ECheckStatus.Update Then
                    If UserSession.SelectedTicket(SelectedPlayerID).Validate(SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID) = ECheckStatus.Update Then

                    Else
                        lblMessage.Text = "Please Enter Your Password To Confirm !"
                        UserSession.SelectedTicket(SelectedPlayerID).Preview = True
                    End If
                Else
                    ClientAlert("You Don't Have Any Wager. Please Choose One.", True)
                End If
                pnSameAmount.Visible = False
                btnSubmit.Visible = True
                txtPassword.Visible = True
                tblBetTheboard.Visible = True
            Catch ex As CTicketException
                ClientAlert(ex.Message, True)
                RunJS("js", "if($('#" + chkCheckAmount.ClientID + "')checked ||$('#pnSameAmount') != null)  $('.amount').hide();", True)
                BetError = True
                LogError(_log, "Preview wagers: ", ex)
            End Try

        End Sub
#End Region

#Region "Preview"
        'Private Sub bindPreviewWagers()
        '    dgTickets.DataSource = UserSession.SelectedTicket(SelectedPlayerID).Tickets
        '    dgTickets.DataBind()
        'End Sub

        Private Function SaveWagers() As Boolean
            If Not CanBetting Then
                ClientAlert("All Betting Has Been Disabled. Please Contact Your Administrator", True)
                Return False
            End If
            Try
                Application.Lock()
                If checkPassword() Then
                    'LogDebug(_log, "test : " & UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).TicketBets(0).AddPoint)
                    'UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).TicketBets(0).AddPointValid = 1.5
                    'UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).TicketBets(0).AddPointMoneyValid = -45
                    If UserSession.SelectedTicket(SelectedPlayerID).Validate(SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID) = ECheckStatus.Update Then
                        lblMessage.Text = "<span style=' font-weight:bold;'>Attention:</span> lines have changed. Please review and reconfirm your bet."
                        ' bindPreviewWagers()
                    Else
                        If UserSession.SelectedTicket(SelectedPlayerID).SaveTickets(SelectedPlayer.BalanceAmount, _
                                                                  SelectedPlayer.AgentID, SelectedPlayer.UserID, UserSession.UserID) Then
                            lblMessage.Text = "Wager(s) has been saved successfully"
                            ' lblMessage.Attributes.Item("style") = "font-size: 14px;"
                            'UserSession.SelectedTicket(SelectedPlayerID) = Nothing
                            btnCancel.Visible = False
                            RunJS("js", " $('.amount').hide();", True)

                            Return True

                        Else
                            ClientAlert("Failed to Save Setting", True)
                        End If
                    End If
                Else
                    If Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) AndAlso Not UserSession.SelectedTicket(SelectedPlayerID).LastTicket.IsForProp Then
                        If chkCheckAmount.Checked Then
                            RunJS("js", " $('.amount').hide();", True)
                        End If
                    End If
                    lblMessage.Text = "Please Enter Your Password To Confirm !"
                    ClientAlert("Wrong Password. Please Enter Another Password", True)

                    txtPassword.Focus()
                End If
            Catch exTicker As CTicketException
                ClientAlert(exTicker.Message.Replace("<br/>", ""), True)
            Catch ex As Exception
                _log.Error("Fail to save Wagers. Message: " & ex.Message, ex)
            Finally
                Application.UnLock()
            End Try

            Return False
        End Function

        Private Function checkPassword() As Boolean
            If SafeString(txtPassword.Text) = "" Then
                Return False
            End If

            If SelectedPlayer IsNot Nothing Then
                Select Case UserSession.UserType
                    Case SBCBL.EUserType.CallCenterAgent
                        Return SelectedPlayer.CheckPhonePasswowrd(txtPassword.Text)
                    Case SBCBL.EUserType.Agent
                        Return UserSession.AgentUserInfo.CheckPassword(txtPassword.Text)
                    Case SBCBL.EUserType.Player
                        Return SelectedPlayer.CheckPasswowrd(txtPassword.Text)
                End Select
            End If

            Return False
        End Function

        Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
                If oTicket.RiskAmount < 1 AndAlso Not BetTypeActive.Equals("BetTheBoard", StringComparison.CurrentCultureIgnoreCase) Then
                    ClientAlert("The Bet Amount has to be different than 0", True)
                    Return
                End If
            Next

            Dim oListNotes As New List(Of CActivityNote)
            If Not BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) AndAlso Not BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) Then
                SaveAmount()
            End If
            If SaveWagers() Then
                If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
                    '' save CCAgent bettings to notes
                    LogCCAgentBet()
                End If


                ' BindWagers()
                ShowWager(True)
                UserSession.SelectedTicket(SelectedPlayerID) = Nothing
                'If SiteType = SBCBL.CEnums.ESiteType.SBS Then
                '    ' btnBackWager.Visible = True
                '    '  btnPreview.Visible = False
                'End If
                btnSubmit.Visible = False
                ' btnCancel.Visible = True
                ' btnCancelGame.Visible = False
                txtPassword.Visible = False
                pnSameAmount.Visible = False
                RunJS("js", "if($('#" + chkCheckAmount.ClientID + "')checked ||$('#pnSameAmount') != null)  $('.amount').hide();", True)
            End If
        End Sub

        'Protected Sub btnBackWager_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBackWager.Click
        '    If SiteType = SBCBL.CEnums.ESiteType.SBS Then
        '        'If UserSession.UserType = SBCBL.EUserType.Player Then
        '        '    Response.Redirect("BetAction.aspx")
        '        'Else
        '        '    Response.Redirect("SelectGame.aspx")
        '        'End If
        '        RaiseEvent BackButtonClick()
        '    End If
        'End Sub
        Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
            UserSession.SelectedTicket(SelectedPlayerID).Preview = False
            ClearMessage()
            BindWagers()
            ShowWager(True)
        End Sub

        'Protected Sub dgTickets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgTickets.ItemDataBound
        '    If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
        '        Dim oTicket As CTicket = CType(e.Item.DataItem, CTicket)
        '        Dim sResult As String = "<table border=0 style='text-align:left;text-indent:8px;' cellpadding='2'><tr><td style='text-align:left' nowrap='nowrap'><b>Wager type</b></td><td style='text-align:left'><b>  "

        '        Select Case UCase(oTicket.TicketType)
        '            Case "PARLAY"
        '                sResult &= oTicket.TicketOptionText

        '            Case "REVERSE"
        '                sResult &= oTicket.TicketOption

        '            Case "TEASER"
        '                sResult &= UserSession.Cache.GetTeaserRuleInfo(oTicket.TicketOption).TeaserRuleName

        '            Case Else
        '                sResult &= oTicket.TicketType

        '        End Select

        '        Dim oDetails As New List(Of String)
        '        Dim nIndex As Integer = 1

        '        For Each oTicketBet As CTicketBet In oTicket.TicketBets

        '            Dim sDetail As String = String.Format("<tr><td style='text-align:left'><b>Select #{0} </b></td><td style='text-align:left'> {1}</td></tr><tr><td colspan='2' style='text-align:left'>", nIndex, oTicketBet.GameType)
        '            Select Case UCase(oTicketBet.BetType)
        '                Case "MONEYLINE"
        '                    sDetail &= getDetailByMoneyLine(oTicketBet)
        '                Case "SPREAD"
        '                    sDetail &= getDetailBySpread(oTicketBet)
        '                Case "TOTALPOINTS"
        '                    sDetail &= getDetailByTotalPoints(oTicketBet)
        '                Case "TEAMTOTALPOINTS"
        '                    sDetail &= getDetailByTeamTotalPoints(oTicketBet)
        '                Case "DRAW"
        '                    sDetail &= getDetailByDraw(oTicketBet)
        '            End Select

        '            oDetails.Add(sDetail)
        '            nIndex += 1
        '        Next

        '        sResult &= "</b></td></tr>" & Join(oDetails.ToArray, "</td></tr>") & String.Format("</td></tr><tr><td style='text-align:left'><b>Amount</b></td><td style='text-align:left'>  Risking <b>{0}</b> To Win <b>{1}</b></td></tr></table>", _
        '                                                                             FormatCurrency(SafeRound(oTicket.RiskAmount), GetRoundMidPoint).Replace("$", ""), _
        '                                                                             FormatCurrency(SafeRound(oTicket.WinAmount), GetRoundMidPoint)).Replace("$", "")

        '        CType(e.Item.FindControl("lblTransDetail"), Label).Text = sResult
        '    End If
        'End Sub

        Public Sub LogCCAgentBet()
            'Dim oListNotes As New List(Of CActivityNote)
            For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
                Dim sNote As String = BuidActivityNote(oTicket)
                Dim oNote As New SBCBL.CacheUtils.CActivityNote()
                oNote.CreateDate = Now.ToUniversalTime()
                oNote.FullName = UserSession.CCAgentUserInfo.Name
                oNote.TicketID = oTicket.TicketID
                oNote.UserID = New Guid(UserSession.CCAgentUserInfo.UserID)
                oNote.Note = sNote
                oNote.NoteType = "CCAgent"
                Dim oNoteManager As New SBCBL.Managers.CActivityNotesManager()
                oNoteManager.AddNote(oNote)
                'oListNotes.Add(oNote)
            Next
            'Return oListNotes
        End Sub

        Function BuidActivityNote(ByVal poTicket As CTicket) As String
            Dim sResult As String = "Wager type: "

            Select Case UCase(poTicket.TicketType)
                Case "PARLAY"
                    sResult &= poTicket.TicketOptionText

                Case "REVERSE"
                    sResult &= poTicket.TicketOption

                Case "TEASER"
                    sResult &= UserSession.Cache.GetTeaserRuleInfo(poTicket.TicketOption).TeaserRuleName

                Case Else
                    sResult &= poTicket.TicketType

            End Select

            sResult &= vbLf
            Dim oDetails As New List(Of String)
            Dim nIndex As Integer = 1

            For Each oTicketBet As CTicketBet In poTicket.TicketBets

                Dim sDetail As String = String.Format("Select #{0}:  {1} " & vbLf, nIndex, oTicketBet.GameType)
                Select Case UCase(oTicketBet.BetType)
                    Case "MONEYLINE"
                        sDetail &= getDetailByMoneyLine(oTicketBet)

                    Case "SPREAD"
                        'LogDebug(_log, "test : " & UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).TicketBets(0).AddPoint)
                        sDetail &= getDetailBySpread(oTicketBet)

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        sDetail &= getDetailByTotalPoints(oTicketBet)

                    Case "DRAW"
                        sDetail &= getDetailByDraw(oTicketBet)
                End Select

                oDetails.Add(sDetail & vbLf)
                nIndex += 1
            Next

            sResult &= Join(oDetails.ToArray) & String.Format("Amount : Risking <b>{0}</b> To Win <b>{1}</b>", _
                                                                                 FormatCurrency(SafeRound(poTicket.RiskAmount), GetRoundMidPoint).Replace("$", ""), _
                                                                                 FormatCurrency(SafeRound(poTicket.WinAmount), GetRoundMidPoint)).Replace("$", "")

            Return sResult
        End Function

        Private Function getDetailBySpread(ByVal poTicketbet As CTicketBet) As String
            Dim sMsg As String
            Dim nSpread As Double
            Dim nSpreadMoney As Double
            Dim sTeam As String
            sMsg = "<b>{0}</b>&nbsp; {1} &nbsp;<u><b>{2} ({3})</b></u>"

            If poTicketbet.HomeSpreadMoney <> 0 Then
                nSpread = poTicketbet.HomeSpread
                nSpreadMoney = poTicketbet.HomeSpreadMoney
                sTeam = poTicketbet.HomeTeam
            Else
                nSpread = poTicketbet.AwaySpread
                nSpreadMoney = poTicketbet.AwaySpreadMoney
                sTeam = poTicketbet.AwayTeam
            End If
            Dim sSpread As String = SafeString(nSpread + poTicketbet.AddPoint)
            Dim sSPreadPK As String = sSpread
            If IsSoccer(poTicketbet.GameType) Then
                sSpread = AHFormat(nSpread + poTicketbet.AddPoint)
            Else
                sSpread = SafeString(IIf(sSpread.Contains("-"), sSpread, "+" & sSpread))
            End If
            Dim sSpreadMoney As String = FormatNumber(nSpreadMoney + poTicketbet.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format(sMsg, IIf(LCase(SafeString(poTicketbet.Context)) = "current", "", "&nbsp;" & poTicketbet.Context), sTeam, IIf(SafeDouble(sSPreadPK) = 0, "PK", sSpread), IIf(sSpreadMoney.Contains("-"), sSpreadMoney, "+" & sSpreadMoney))
        End Function

        Private Function getDetailByTotalPoints(ByVal poTicketbet As CTicketBet) As String
            Dim sMsg As String
            Dim nMoney As Double

            If poTicketbet.TotalPointsOverMoney <> 0 Then
                sMsg = "Over"
                nMoney = poTicketbet.TotalPointsOverMoney
            Else
                sMsg = "Under"
                nMoney = poTicketbet.TotalPointsUnderMoney
            End If

            Dim sTotalPointsMoney As String = FormatNumber(nMoney + poTicketbet.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format("<b>{0}</b>&nbsp;{1} <b>-</b> {2} &nbsp;<u><b>{3} {4} ({5})</b></u>", IIf(LCase(SafeString(poTicketbet.Context)) = "current", "", "&nbsp;" & poTicketbet.Context), poTicketbet.HomeTeam, _
                                 poTicketbet.AwayTeam, sMsg, FormatPoint(poTicketbet.TotalPoints + poTicketbet.AddPoint, poTicketbet.GameType).Replace("+"c, ""), IIf(sTotalPointsMoney.Contains("-"), sTotalPointsMoney, "+" & sTotalPointsMoney))
        End Function

        Private Function getDetailByTeamTotalPoints(ByVal poTicketbet As CTicketBet) As String
            Dim sMsg As String
            Dim nMoney As Double

            If poTicketbet.TotalPointsOverMoney <> 0 Then
                sMsg = "Over"
                nMoney = poTicketbet.TotalPointsOverMoney
            Else
                sMsg = "Under"
                nMoney = poTicketbet.TotalPointsUnderMoney
            End If

            Dim sTotalPointsMoney As String = FormatNumber(nMoney + poTicketbet.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format("<b>{0}</b>&nbsp;{1} &nbsp;<u><b>{2} {3} ({4})</b></u>", IIf(LCase(SafeString(poTicketbet.Context)) = "current", "", "&nbsp;" & poTicketbet.Context), IIf(poTicketbet.TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase), poTicketbet.AwayTeam, poTicketbet.HomeTeam), _
                                  sMsg, FormatPoint(poTicketbet.TotalPoints + poTicketbet.AddPoint, poTicketbet.GameType).Replace("+"c, ""), IIf(sTotalPointsMoney.Contains("-"), sTotalPointsMoney, "+" & sTotalPointsMoney))
        End Function

        Private Function getDetailByDraw(ByVal poTicketbet As CTicketBet) As String
            Dim sDrawLine As String = FormatNumber(poTicketbet.DrawMoneyLine + poTicketbet.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format("<b>{0}</b>&nbsp;{1} <b>-</b> {2} &nbsp;<u><b>Draw ({3})</b></u>", IIf(poTicketbet.Context = "Current", "", "&nbsp;" & poTicketbet.Context), _
            poTicketbet.HomeTeam, poTicketbet.AwayTeam, IIf(sDrawLine.Contains("-"), sDrawLine, "+" & sDrawLine))
        End Function

        Private Function getDetailByMoneyLine(ByVal poTicketbet As CTicketBet) As String
            Dim sMsg As String = "<b>{0}</b>&nbsp;{1} &nbsp;<u><b>ML ({2})</b></u>"
            Dim nMoneyLine As Double
            Dim sTeam As String

            If poTicketbet.IsForProp Then
                nMoneyLine = poTicketbet.BetPoint + poTicketbet.AddPointMoney
                sTeam = poTicketbet.Team
            ElseIf Not BetTypeA.Equals("BetIfAll", StringComparison.CurrentCultureIgnoreCase) Then
                If poTicketbet.HomeMoneyLine <> 0 Then
                    nMoneyLine = poTicketbet.HomeMoneyLine + poTicketbet.AddPointMoney
                    sTeam = poTicketbet.HomeTeam
                Else
                    nMoneyLine = poTicketbet.AwayMoneyLine + poTicketbet.AddPointMoney
                    sTeam = poTicketbet.AwayTeam
                End If
            End If

            Dim sMoneyLine As String = FormatNumber(nMoneyLine, GetRoundMidPoint, TriState.UseDefault, TriState.False)
            Return String.Format(sMsg, IIf(LCase(SafeString(poTicketbet.Context)) = "current", "", "&nbsp;" & poTicketbet.Context), sTeam, IIf(sMoneyLine.Contains("-"), sMoneyLine, "+" & sMoneyLine))
        End Function

        Private Function FormatPoint(ByVal pnPoint As Double, ByVal psGameType As String) As String
            If IsSoccer(psGameType) Then
                Return AHFormat(pnPoint)
            End If
            Return SafeString(pnPoint)
        End Function
#End Region

        Public Sub ShowWager(ByVal pbShow As Boolean)
            rptTickets.Visible = pbShow
            dvBtnWager.Visible = pbShow
            'dgTickets.Visible = Not pbShow
            dvBtnReview.Visible = Not pbShow
        End Sub

        Public Sub ClearMessage()
            ' lblMessage.Text = ""
            ' lblMessage.Attributes.Item("style") = "font-size: 11pt;"
        End Sub



    End Class
End Namespace