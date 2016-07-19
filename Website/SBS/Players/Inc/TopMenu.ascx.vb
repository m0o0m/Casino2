Namespace Players
    Partial Class TopMenu
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub lbnMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblBalance.Click, lbnHistory.Click, lbnOpenBet.Click, lbnCasino.Click
            Dim sURL As String = ""
            Select Case CType(sender, LinkButton).CommandArgument
                Case "WAGER"
                    sURL = "Default.aspx"
                Case "OPENBET"
                    sURL = "OpenBet.aspx"
                Case "HISTORY"
                    sURL = "History.aspx"
                Case "BALANCE"
                    sURL = "WeekBalance.aspx"
                Case "ACCSTATUS"
                    sURL = "PlayerAccount.aspx"
                Case "CASINO"
                    'update casino amount
                    Dim oCasinoManager As New SBCBL.Managers.CCasinoMananger()
                    oCasinoManager.UpdateCasinoBalance(UserSession.PlayerUserInfo.BalanceAmount, _
                                                       UserSession.PlayerUserInfo.Template.CasinoMaxAmount, _
                                                       UserSession.PlayerUserInfo.Login)

                    'encrypt casino password
                    Dim oTriple As New SBCBL.CCasinoDES()
                    Dim sUserInfo As String = oTriple.Encrypt(UserSession.PlayerUserInfo.Login & SBCBL.std.CASINO_SUFFIX & "|" & UserSession.PlayerUserInfo.Password)

                    sUserInfo = HttpUtility.UrlEncode(sUserInfo)
                    SBCBL.std.OpenPopupWindow(String.Format("http://casino.win1t.com/lobby.jsp?alogin=Y&uinfo={0}&" & _
                                                            "jsessionid=aM7-boIgvWS6VcBgXG&free=1&h1=600&w1=800", sUserInfo), 800, 600, "", _
                                                            False, False, False, False, False, False, True)
                    Exit Sub
            End Select
            Response.Redirect(sURL)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                liBalance.Attributes.Add("class", "itemMenu")
                liOpenBet.Attributes.Add("class", "itemMenu")
                liAccStatus.Attributes.Add("class", "itemMenu")
                liWager.Attributes.Add("class", "border_left_menu")
                liHistory.Attributes.Add("class", "itemMenu")
                Select Case Me.Page.MenuTabName
                    Case "BET_ACTION"
                        lbnWager.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liWager, True)
                    Case "HISTORY"
                        lbnHistory.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liHistory)
                    Case "OPEN_BET"
                        lbnOpenBet.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liOpenBet)
                    Case "ACCOUNT_STATUS"
                        lbnAccStatus.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liAccStatus)
                    Case "BALANCE"
                        lblBalance.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liBalance)
                End Select
                Select Case Me.Page.SubMenuActive
                    Case "ACCOUNT_STATUS"
                        lbtPlayerAccount.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subPlayerAccount)
                    Case "CHANGE_PASSWORD"
                        lbtChangePassword.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subChangePassword)
                    Case "MAIL_INBOX"
                        lbnMailInbox.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subMailInbox)
                End Select

                liCasino.Visible = Not UserSession.PlayerUserInfo.IsCasinoLocked  'UserSession.PlayerUserInfo.HasCasino
            End If

        End Sub

        Public Sub ActiveSubMenu(ByVal pli As HtmlControl)
            pli.Attributes.Add("class", "menu_active")
            pli.Style.Add("display", "inline")
            pli.Style.Add("float", "left")
            pli.Style.Add("height", "28px")
        End Sub

        Public Sub ActiveMenu(ByVal pli As HtmlControl, Optional ByVal bfirstItem As Boolean = False)
            If (bfirstItem) Then
                pli.Attributes.Add("class", "menu_active")
                pli.Style.Add("Border-Left", "#FFF solid 1px")
                Return
            End If
            pli.Attributes.Add("class", "menu_active")
            pli.Style.Add("padding-right", "1px")
            pli.Style.Add("margin-left", "-10px")
            pli.Style.Add("Border-Left", "#FFF solid 1px")
        End Sub
    End Class
End Namespace