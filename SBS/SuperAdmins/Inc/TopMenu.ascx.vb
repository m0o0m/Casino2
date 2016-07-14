Imports System.Data
Imports SBCBL.std

Namespace SBSSuperAdmins

    Partial Class TopMenu
        Inherits SBCBL.UI.CSBCUserControl

#Region "Page's Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                '  BindMenu()
            End If

        End Sub

        'Protected Sub rptSubMenu_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        '    Select Case UCase(e.CommandName)
        '        Case "CLICK"
        '            Dim sURL As String = SafeString(e.CommandArgument)
        '            If sURL <> "" Then
        '                Response.Redirect(sURL)
        '            End If
        '    End Select
        'End Sub

        'Protected Sub rptMenu_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMenu.ItemDataBound
        '    If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
        '        Dim oSubMenu As KeyValuePair(Of String, DataTable) = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable))
        '        'Dim rptSubMenu As Repeater = CType(e.Item.FindControl("rptSubMenu"), Repeater)
        '        Dim lbtn As LinkButton = CType(e.Item.FindControl("lbtnMenu"), LinkButton)

        '        'rptSubMenu.DataSource = oSubMenu.Value
        '        'rptSubMenu.DataBind()
        '    End If
        'End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            '  SetMenuActive(Me.Page.MenuTabName, Me.Page.SideMenuTabName)
        End Sub

#End Region

#Region "Functions"

        Private Sub BindMenu()
            Dim oMenu As New Dictionary(Of String, DataTable)
            Dim oSubMenu As DataTable

            '' Sub SysTem Manager
            oSubMenu = GetMenuTable()

            'System Settings
            oSubMenu.Rows.Add(New Object() {"System Categories", "System Categories", "SYSTEM_SETTINGS", "SysSettings.aspx"})

            'Player Templates
            oSubMenu.Rows.Add(New Object() {"Templates Settings", "Templates Settings ", "PLAYER_TEMPLATES", "PlayerTemplates.aspx"})

            'Users Manager
            oSubMenu.Rows.Add(New Object() {"User Settings", "User Settings", "USERS_MANAGER", "UsersManager.aspx"})

            'White Label
            oSubMenu.Rows.Add(New Object() {"White Label Settings", "White Label Settings ", "WHITE_LABEL", "WhiteLabelSettings.aspx"})

            'HTML Content
            oSubMenu.Rows.Add(New Object() {"Rules-Odds-Payout Display", "Rules-Odds-Payout Display", "HTML_CONTENTS_MANAGER", "ContentManager.aspx"})


            'Allow Option
            oSubMenu.Rows.Add(New Object() {"Betting Settings", "Betting Settings", "ALLOW_BETTING", "AllowBetting.aspx"})

            'Subject Email Setup
            oSubMenu.Rows.Add(New Object() {"Email Subject Setup", "Email Subject Setup", "EMAIL_SUBJECT_SETUP", "SubjectEmailSetup.aspx"})

            'Inbox Email Setup
            oSubMenu.Rows.Add(New Object() {"Inbox Mail", "Inbox Mail", "INBOX_MAIL", "InboxMail.aspx"})

            'Compose Email Setup
            oSubMenu.Rows.Add(New Object() {"Compose Mail", "Compose Mail", "COMPOSE_MAIL", "ComposeMail.aspx"})

            oMenu.Add("SYSTEM MANAGEMENT", oSubMenu)

            '' Sub Game Manager
            oSubMenu = GetMenuTable()

            'GameType
            oSubMenu.Rows.Add(New Object() {"Game Type Setup", "Game Type Setup", "GAME_TYPE", "GameType.aspx"})

            'Agent BookMaker Setup
            oSubMenu.Rows.Add(New Object() {"Agent BookMaker Setup", "Agent BookMaker Setup", "BOOKMAKER_SETUP", "BookMakerSetup.aspx"})

            ' Preset Rules
            oSubMenu.Rows.Add(New Object() {"Manage Rules", "Manage Rules", "PRESET_RULES", "PresetRules.aspx"})

            'Parlay Odds
            oSubMenu.Rows.Add(New Object() {"Parlay Setup", "Parlay Setup", "PARLAY_REVERSE_RULES", "ParlayReverseRules.aspx"})

            'Teaser Odds
            oSubMenu.Rows.Add(New Object() {"Teaser Setup", "Teaser Setup", "TEASER_ODDS", "TeaserOdds.aspx"})

            'Manual Lines
            oSubMenu.Rows.Add(New Object() {"Line On & Off", "Line On & Off", "LINES_MONITOR", "LockGameBet.aspx"})

            'Manual Quarter Line
            oSubMenu.Rows.Add(New Object() {"Quarter Lines", "Manual Muti Game Lines", "MANUAL_LINE", "ManualLines.aspx"})

            'Manage Tickets
            oSubMenu.Rows.Add(New Object() {"Manage Tickets", "Manage Bet Tickets", "MANAGE_TICKETS", "ManageTickets.aspx"})

            'Manual Game Score
            oSubMenu.Rows.Add(New Object() {"Update Game Scores", "Manual Muti Games", "MANUAL_GAME", "ManualGames.aspx"})

            'Manual Quarter Score
            oSubMenu.Rows.Add(New Object() {"Update Quarter Scores", "Manual Games Quaters", "MANUAL_QUATERS", "ManualQuaters.aspx"})

            'Manual PropPosition
            oSubMenu.Rows.Add(New Object() {"Update Proposition", "Update Proposition", "MANUAL_PROPOSITION", "ManualProposition.aspx"})

            oMenu.Add("GAME MANAGEMENT", oSubMenu)

            '' Sub Reports Manager
            oSubMenu = GetMenuTable()





            'Agent Balance
            oSubMenu.Rows.Add(New Object() {"Agent Balance Reports", "Agent Balance Reports", "SAGENT_BALANCE_REPORTS", "SuperAgentBalance.aspx"})

            'Player Balance
            oSubMenu.Rows.Add(New Object() {"Player Balance Reports", "Player Balance Reports", "SPLAYER_BALANCE_REPORTS", "SuperPlayerBalance.aspx"})

            'Pending tickets
            oSubMenu.Rows.Add(New Object() {"Pending Wagers Reports", "Pending Wagers Reports", "PENDING_WAGERS_REPORTS", "PendingTickets.aspx"})

            'P/L Reports
            oSubMenu.Rows.Add(New Object() {"P/L Reports", "P/L Reports", "PL_REPORTS", "PLReports.aspx"})

            'IP Reports
            oSubMenu.Rows.Add(New Object() {"IP Reports", "IP Reports", "IP_REPORTS", "IPReports.aspx"})

            'IP Alert
            oSubMenu.Rows.Add(New Object() {"IP Alert", "IP Alert", "IP_ALERT", "IPAlert.aspx"})

            'Position Reports
            oSubMenu.Rows.Add(New Object() {"Agent Position Reports", "Agent Position Reports", "SUPER_POSITION_REPORTS", "SuperPositionReport.aspx"})

            'Message To All Player
            oSubMenu.Rows.Add(New Object() {"Message To All Player", "Message To All Player", "MESSAGE_ALL_PLAYER", "MessagePlayers.aspx"})

            'CCAgent betting logs
            oSubMenu.Rows.Add(New Object() {"Call Center Activity", "Call Center Activity", "BET_BY_PHONE", "Betbyphone.aspx"})

            'Transactions Manager
            oSubMenu.Rows.Add(New Object() {"Transactions Manager", "Transactions Manager", "TRANSACTIONS", "Transactions.aspx"})

            oMenu.Add("REPORTS MANAGEMENT", oSubMenu)


            '   rptMenu.DataSource = oMenu
            ' rptMenu.DataBind()
        End Sub

        Private Function GetMenuTable() As DataTable
            Dim odtMenus As New DataTable
            odtMenus.Columns.Add("MenuText", GetType(String))
            odtMenus.Columns.Add("MenuToolTip", GetType(String))
            odtMenus.Columns.Add("MenuValue", GetType(String)) 'as the same value of Me.Page.SideMenuTabName, use for set menu active
            odtMenus.Columns.Add("MenuUrl", GetType(String))

            Return odtMenus
        End Function

        'Private Sub SetMenuActive(ByVal psMenu As String, ByVal psSubMenu As String)
        '    For Each oMenu As RepeaterItem In rptMenu.Items
        '        If Not (oMenu.ItemType = ListItemType.AlternatingItem OrElse oMenu.ItemType = ListItemType.Item) Then
        '            Continue For
        '        End If

        '        '' Set selected main menu
        '        Dim liMenu As HtmlControl = CType(oMenu.FindControl("liMenu"), HtmlControl)
        '        If liMenu.Attributes("menu") = psMenu Then
        '            liMenu.Attributes("class") = "selected"
        '        Else
        '            liMenu.Attributes("class") = ""
        '        End If

        '        Dim rptSubMenu As Repeater = CType(oMenu.FindControl("rptSubMenu"), Repeater)
        '        For Each oSubMenu As RepeaterItem In rptSubMenu.Items
        '            If Not (oSubMenu.ItemType = ListItemType.AlternatingItem OrElse oSubMenu.ItemType = ListItemType.Item) Then
        '                Continue For
        '            End If

        '            '' Set selected sub menu
        '            Dim tdSubMenu As HtmlControl = CType(oSubMenu.FindControl("tdSubMenu"), HtmlControl)
        '            Dim lbnsubmenu As LinkButton = CType(oSubMenu.FindControl("lbtnSubMenu"), LinkButton)
        '            If tdSubMenu.Attributes("menu") = psSubMenu Then
        '                tdSubMenu.Attributes("class") = "selected"
        '                lbnsubmenu.ForeColor = Drawing.Color.Yellow
        '            Else
        '                tdSubMenu.Attributes("class") = ""
        '            End If
        '        Next
        '    Next
        'End Sub

#End Region


    End Class

End Namespace
