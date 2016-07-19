Imports System.Data
Imports SBCBL.std

Partial Class SBS_SuperAdmins_Main
    Inherits SBCBL.UI.CSBCPage
    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.PageTitle = "Main Menu"
        'Me.SideMenuTabName = "IP_ALERT"
        'Me.MenuTabName = "REPORTS MANAGEMENT"
        DisplaySubTitlle(Me.Master, "Main Menu")
    End Sub

    Private ReadOnly Property MenuType() As String
        Get
            Return SafeString(Request.QueryString("MenuType"))
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindMenu()
        End If
    End Sub
    Private Sub BindMenu()
        Dim oMenu As New Dictionary(Of String, DataTable)
        Dim oSubMenu As DataTable
        oSubMenu = GetMenuTable()
        If MenuType.Equals("sys",StringComparison.CurrentCultureIgnoreCase) Then
            '' Sub SysTem Manager
            'Users Manager
            oSubMenu.Rows.Add(New Object() {"User Settings", "User Settings", "USERS_MANAGER", "UsersManager.aspx"})
            'White Label
            If UserSession.SuperAdminInfo.IsManager Then

                'System Settings
                oSubMenu.Rows.Add(New Object() {"System Categories", "System Categories", "SYSTEM_SETTINGS", "SysSettings.aspx"})

                'Player Templates
                oSubMenu.Rows.Add(New Object() {"Templates Settings", "Templates Settings ", "PLAYER_TEMPLATES", "PlayerTemplates.aspx"})

                oSubMenu.Rows.Add(New Object() {"White Label Settings", "White Label Settings ", "WHITE_LABEL", "WhiteLabelSettings.aspx"})

                'HTML Content
                oSubMenu.Rows.Add(New Object() {"Rules-Odds-Payout Display", "Rules-Odds-Payout Display", "HTML_CONTENTS_MANAGER", "ContentManager.aspx"})

                'Allow Option
                oSubMenu.Rows.Add(New Object() {"Betting Settings", "Betting Settings", "ALLOW_BETTING", "AllowBetting.aspx"})

            End If

            'Subject Email Setup
            oSubMenu.Rows.Add(New Object() {"Email Subject Setup", "Email Subject Setup", "EMAIL_SUBJECT_SETUP", "SubjectEmailSetup.aspx"})

            'Inbox Email Setup
            oSubMenu.Rows.Add(New Object() {"Inbox Mail", "Inbox Mail", "INBOX_MAIL", "InboxMail.aspx"})

            'Compose Email Setup
            oSubMenu.Rows.Add(New Object() {"Compose Mail", "Compose Mail", "COMPOSE_MAIL", "ComposeMail.aspx"})
        End If

        'oMenu.Add("SYSTEM MANAGEMENT", oSubMenu)
        If MenuType.Equals("game", StringComparison.CurrentCultureIgnoreCase) Then
            '' Sub Game Manager
            If UserSession.SuperAdminInfo.IsManager Then

                
                'Teaser Odds
                oSubMenu.Rows.Add(New Object() {"Teaser Setup", "Teaser Setup", "TEASER_ODDS", "TeaserOdds.aspx"})
                
                'Manual Game Score
                oSubMenu.Rows.Add(New Object() {"Update Game Scores", "Manual Muti Games", "MANUAL_GAME", "ManualGames.aspx"})

                'Manual Quarter Score
                oSubMenu.Rows.Add(New Object() {"Update Quarter Scores", "Manual Games Quaters", "MANUAL_QUATERS", "ManualQuaters.aspx"})

                'Manual PropPosition
                oSubMenu.Rows.Add(New Object() {"Update Proposition", "Update Proposition", "MANUAL_PROPOSITION", "ManualProposition.aspx"})

                oSubMenu.Rows.Add(New Object() {"Add new game", "Add new games", "ADD_NEW_GAMES", "AddNewGames.aspx"})
            End If
            'GameType
            oSubMenu.Rows.Add(New Object() {"Game Type Setup", "Game Type Setup", "GAME_TYPE", "GameType.aspx"})

            'Agent BookMaker Setup
            oSubMenu.Rows.Add(New Object() {"Agent BookMaker Setup", "Agent BookMaker Setup", "BOOKMAKER_SETUP", "BookMakerSetup.aspx"})

            ' Preset Rules
            oSubMenu.Rows.Add(New Object() {"Manage Rules", "Manage Rules", "PRESET_RULES", "PresetRules.aspx"})

            'Manage Tickets
            oSubMenu.Rows.Add(New Object() {"Manage Tickets", "Manage Bet Tickets", "MANAGE_TICKETS", "ManageTickets.aspx"})

            'Parlay Odds
            oSubMenu.Rows.Add(New Object() {"Parlay Setup", "Parlay Setup", "PARLAY_REVERSE_RULES", "ParlayReverseRules.aspx"})

            'Manual Lines
            oSubMenu.Rows.Add(New Object() {"Line On & Off", "Line On & Off", "LINES_MONITOR", "LockGameBet.aspx"})

            'Manual Quarter Line
            oSubMenu.Rows.Add(New Object() {"Quarter Lines", "Manual Muti Game Lines", "MANUAL_LINE", "ManualLines.aspx"})

            '    oMenu.Add("GAME MANAGEMENT", oSubMenu)
        End If
        If MenuType.Equals("report", StringComparison.CurrentCultureIgnoreCase) Then
            '' Sub Reports Manager





            'Agent Balance
            oSubMenu.Rows.Add(New Object() {"Agent Balance Reports", "Agent Balance Reports", "SAGENT_BALANCE_REPORTS", "SuperAgentBalance.aspx"})

            'Player Balance
            oSubMenu.Rows.Add(New Object() {"Player Balance Reports", "Player Balance Reports", "SPLAYER_BALANCE_REPORTS", "weeklyfigure.aspx"})

            'Pending tickets
            oSubMenu.Rows.Add(New Object() {"Pending Wagers Reports", "Pending Wagers Reports", "PENDING_WAGERS_REPORTS", "PendingTickets.aspx"})

            'P/L Reports
            oSubMenu.Rows.Add(New Object() {"P/L Reports", "P/L Reports", "PL_REPORTS", "PLReports.aspx"})

            oSubMenu.Rows.Add(New Object() {"Live Ticker", "Live Ticker", "LIVE_TICKER", "LiveTicker.aspx"})

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

            ' Volumn Report
            oSubMenu.Rows.Add(New Object() {"Volumn Report", "Volumn Report", "VOLUMNREPORT", "VolumnReport.aspx"})

        End If
        rptSubMenu.DataSource = oSubMenu
        rptSubMenu.DataBind()
        '            oMenu.Add("REPORTS MANAGEMENT", oSubMenu)
       
    End Sub

    Private Function GetMenuTable() As DataTable
        Dim odtMenus As New DataTable
        odtMenus.Columns.Add("MenuText", GetType(String))
        odtMenus.Columns.Add("MenuToolTip", GetType(String))
        odtMenus.Columns.Add("MenuValue", GetType(String)) 'as the same value of Me.Page.SideMenuTabName, use for set menu active
        odtMenus.Columns.Add("MenuUrl", GetType(String))

        Return odtMenus
    End Function
End Class
