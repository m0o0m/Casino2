Imports SBCBL.std
Imports SBCBL.Managers

Namespace SBCAgents

    Partial Class Agents_Inc_TopMenu
        Inherits SBCBL.UI.CSBCUserControl
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())


        Protected Sub lbnMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim sURL As String = ""

            Select Case CType(sender, LinkButton).CommandArgument
                Case "WAGER"
                    sURL = "/SBS/Agents/SelectGame.aspx"
                Case "USER_MANAGEMENT"
                    sURL = "/SBS/Agents/Management/Players.aspx"
                Case "HOME"
                    sURL = "/SBS/Agents/OpenBets.aspx"
                Case "REPORT"
                    sURL = "/SBS/Agents/Management/PlayersReports.aspx"
                Case "ACCSTATUS"
                    sURL = "/SBS/Agents/AgentAccount.aspx"
                Case "GAME_MANAGEMENT"
                    sURL = "/SBS/Agents/Management/OddSetting.aspx"
                Case "SYSTEM_MANAGEMENT"
                    sURL = "/SBS/Agents/Management/ConfigureLogo.aspx"
            End Select

            If sURL = "" Then
                ClientAlert("Not Yet Set URL To Redirect", True)
                LogInfo(log, "URL is Empty")
                Return
            End If

            Response.Redirect(sURL)
        End Sub

#Region "Page Event"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'If Not IsPostBack Then
            '    If UserSession.AgentUserInfo IsNot Nothing Then
            '        If UserSession.AgentUserInfo.IsSuperAgent Then
            '            With UserSession.AgentUserInfo
            '                If .HasGameManagement Then
            '                    lbtGameManagement.Visible = True
            '                    liGameManager.Visible = True
            '                    ulSubReport.Attributes.CssStyle.Add("margin-left", "-340px")
            '                Else
            '                    liGameManager.Visible = False
            '                    lbtGameManagement.Visible = False
            '                    ulSubReport.Attributes.CssStyle.Add("margin-left", "-310px")
            '                End If

            '                If .HasSystemManagement Then
            '                    lbtnSystemManagement.Visible = True
            '                    liSystemManager.Visible = True
            '                Else
            '                    lbtnSystemManagement.Visible = False
            '                    liSystemManager.Visible = False
            '                End If
            '            End With
            '        Else ' is not SuperAgent
            '            lbtGameManagement.Visible = False
            '            ulSubReport.Attributes.CssStyle.Add("margin-left", "-290px")
            '            lbtnSystemManagement.Visible = False
            '        End If
            '    End If
            'End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                pnGameManagement.Visible = UserSession.AgentUserInfo.HasGameManagement
                ' pnUserManagement.Visible = UserSession.AgentUserInfo.HasSystemManagement
            End If
            'If Not IsPostBack Then
            '    liUserManagement.Attributes.Add("class", "itemMenu")
            '    liHomepage.Attributes.Add("class", "border_left_menu")
            '    liGameManager.Attributes.Add("class", "itemMenu")
            '    liAwer.Attributes.Add("class", "itemMenu")
            '    liSystemManager.Attributes.Add("class", "itemMenu")
            '    liAccountStatus.Attributes.Add("class", "itemMenu")
            '    liReport.Attributes.Add("class", "itemMenu")
            '    Select Case Me.Page.MenuTabName
            '        Case "HOME"
            '            lbnHomepage.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liHomepage, True)
            '        Case "BET_ACTION"
            '            lbnWager.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liAwer)
            '        Case "SYSTEM_MANAGEMENT"
            '            lbtnSystemManagement.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liSystemManager)
            '        Case "USER_MANAGEMENT"
            '            lbnUserManagement.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liUserManagement)
            '        Case "GAME_MANAGEMENT"
            '            lbtGameManagement.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liGameManager)
            '        Case "REPORT"
            '            lbnReport.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liReport)
            '        Case "ACCOUNT_STATUS"
            '            lbnAccStatus.ForeColor = Drawing.Color.Yellow
            '            ActiveMenu(liAccountStatus)
            '    End Select
            '    Select Case Me.Page.SubMenuActive
            '        Case "ACCOUNT_STATUS"
            '            lbtPlayerAccount.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPlayerAccount)
            '        Case "CHANGE_PASSWORD"
            '            lbtChangePassword.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subChangePassword)
            '        Case "PLAYERS"
            '            lbnPlayers.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPlayers)
            '        Case "AGENTS"
            '            lbnAgents.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subAgents)
            '        Case "GAMES_SETTING"
            '            lbnGamesSettings.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subGamesSettings)
            '        Case "ODDS_SETTING"
            '            lbnGamesOdds.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subGamesOdds)
            '        Case "PLAYER_REPORT"
            '            lbnPlayerReport.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPlayerReport)
            '        Case "AGENT_POSITION_REPORTS"
            '            lbnPositionReport.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPositionReport)
            '        Case "AGENT_BALANCE_REPORTS"
            '            lbnAgentReport.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subAgentReport)
            '        Case "PL_REPORTS"
            '            lbnPLReport.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPLReport)
            '        Case "IP_REPORTS"
            '            lbnIPReport.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subIPReport)
            '        Case "HISTORY"
            '            lbnHistory.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subHistory)
            '        Case "OPEN_BET"
            '            lbnPending.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subPending)
            '        Case "TRANSACTIONS"
            '            lbnTransaction.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subTransaction)
            '        Case "CONFIG_LOGO"
            '            lbnConfigLogo.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subConfigLogo)
            '        Case "SITE_OPTION"
            '            lbnSiteOption.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subSiteOption)
            '        Case "MAIL_INBOX"
            '            lbnMailInbox.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subMailInbox)
            '        Case "GAME_MANUAL"
            '            lbnGameManual.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subGameManual)
            '        Case "IP_ALERT"
            '            lbnIPAlert.Style.Add("color", "#ffff00")
            '            ActiveSubMenu(subIPAlert)
            '    End Select
            'End If

            'CType(Page.Master.FindControl("imgLogo"), HtmlImage).Src = "/SBS/images/SBSLogo_small.png"
        End Sub
#End Region

        'Public Sub ActiveSubMenu(ByVal pli As HtmlControl)
        '    pli.Attributes.Add("class", "menu_active")
        '    pli.Style.Add("display", "inline")
        '    pli.Style.Add("float", "left")
        '    pli.Style.Add("height", "28px")
        'End Sub

        'Public Sub ActiveMenu(ByVal pli As HtmlControl, Optional ByVal bfirstItem As Boolean = False)
        '    If (bfirstItem) Then
        '        pli.Attributes.Add("class", "menu_active")
        '        pli.Style.Add("Border-Left", "#FFF solid 1px")
        '        Return
        '    End If
        '    pli.Attributes.Add("class", "menu_active")
        '    pli.Style.Add("padding-right", "1px")
        '    pli.Style.Add("margin-left", "-10px")
        '    pli.Style.Add("Border-Left", "#FFF solid 1px")
        'End Sub
    End Class

End Namespace
