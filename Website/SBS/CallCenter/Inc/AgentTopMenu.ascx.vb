Imports SBCBL.std

Namespace SBCCallCenterAgents

    Partial Class AgentTopMenu
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub lbnMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim sURL As String = ""

            Select Case CType(sender, LinkButton).CommandArgument
                Case "HOME"
                    sURL = "/SBS/CallCenter/Default.aspx"
                Case "BET_ACTION"
                    sURL = "/SBS/CallCenter/BetAction.aspx"
                Case "OPEN_BET"
                    sURL = "/SBS/CallCenter/OpenBets.aspx"
                Case "HISTORY"
                    sURL = "/SBS/CallCenter/History.aspx"
                Case "ACCOUNT_STATUS"
                    sURL = "/SBS/CallCenter/AccountStatus.aspx"
                Case "GAME_MANAGER"
                    sURL = "/SBS/CallCenter/LinesMonitor.aspx"
            End Select

            If sURL = "" Then
                ClientAlert("Not Yet Set URL To Redirect", True) : Return
            End If

            Response.Redirect(sURL)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                liHome.Attributes.Add("class", "border_left_menu")
                liPending.Attributes.Add("class", "itemMenu")
                liGameManager.Attributes.Add("class", "itemMenu")
                liAccountInfo.Attributes.Add("class", "itemMenu")
                liHistory.Attributes.Add("class", "itemMenu")
                Select Case Me.Page.MenuTabName
                    Case "HOME"
                        lbnHomepage.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liHome, True)
                    Case "GAME_MANAGER"
                        lbnGameManager.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liGameManager)
                    Case "OPEN_BET"
                        lbnOpenBet.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liPending)
                    Case "HISTORY"
                        lbnHistory.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liHistory)
                    Case "ACCOUNT_STATUS"
                        lbnAccountStatus.ForeColor = Drawing.Color.Yellow
                        ActiveMenu(liAccountInfo)
                End Select

                Select Case Me.Page.SubMenuActive
                    Case "LINES_MONITOR", "MANUAL_LINE", "MANUAL_QUATERS", "MANUAL_GAME"
                        lbnGameManager.Style.Add("color", "Yellow")
                        ActiveMenu(liGameManager)

                End Select
                Select Case Me.Page.SubMenuActive
                    Case "LINES_MONITOR"
                        lbnLinesMonitor.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subLinesMonitor)
                    Case "MANUAL_LINE"
                        lbnSetupQuarter.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subSetupQuarter)
                    Case "MANUAL_QUATERS"
                        lbnUpdateQuarterScores.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subUpdateQuarterScores)
                    Case "MANUAL_GAME"
                        lbnUpdateGameScores.Style.Add("color", "#ffff00")
                        ActiveSubMenu(subUpdateGameScores)
                End Select

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
                pli.Attributes.Add("border-left", "#FFF")
                Return
            End If
            pli.Attributes.Add("class", "menu_active")
            pli.Style.Add("padding-right", "1px")
            pli.Style.Add("margin-left", "-10px")
            pli.Attributes.Add("border-left", "#FFF")
        End Sub
    End Class

End Namespace
