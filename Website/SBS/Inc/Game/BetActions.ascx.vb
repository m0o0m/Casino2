Imports SBCBL
Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.Managers
Imports System.Collections
Imports System.Data
Imports WebsiteLibrary
Imports System.IO
Imports System.Security
Imports System.Security.AccessControl
Partial Class SBS_Inc_Game_BetActions
    Inherits SBCBL.UI.CSBCUserControl

    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
    Private _sHightlightJS As String = ""
    Private _TempLastRecords As New Dictionary(Of String, Object())
    Private _oOddsRuleEngine As COddRulesEngine
    Private _sOffTimeCategory As String = SBCBL.std.GetSiteType & " LineOffHour"

    Private _SuperBookmakerValue As String = SBCBL.std.GetSiteType & " BookmakerType"
    Private _sParlay As String = "Parlay"
    Private __BetIfAll As String = "BetIfAll"
    Private __IfWin As String = "If Win"
    Private __IfWinOrPush As String = "If Win or Push"
    Private _sReverse As String = "Reverse"
    Private _sStraight As String = "Straight"
    Private _sBetTheBoard As String = "BetTheBoard"
    Private _sTeaser As String = "Teaser"
    Private CATEGORY_CURRENT As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_CURRENT"
    Private CATEGORY_TIGERSB As String = SBCBL.std.GetSiteType & "_PARLAY_PAYOUT_TIGERSB"
    Private _oListAddedGames As New Dictionary(Of String, Data.DataTable)()
    Private _bHaveBettingData As Boolean = False
    Private _nOFFTeamTotal As Integer = 99999999
    Private _Description As String = ""
    Public _contextGame As Integer = 1
    Public _context1H As Integer = 1

#Region "Properties"
    Public ReadOnly Property IfBetOrdinal() As String
        Get
            If LCase(BetTypeActive).Contains("if ") Then
                Dim nIndex As Integer = 1
                If UserSession.SelectedTicket(SelectedPlayerID) IsNot Nothing Then
                    nIndex = UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count + 1
                End If
                Return GetOrdinalNumber(nIndex)
            End If

            Return ""
        End Get
    End Property

    Private Property HighlightRecords() As Dictionary(Of String, String)
        Get
            If Session("HightlightRecords") Is Nothing Then
                Session("HightlightRecords") = New Dictionary(Of String, String)
            End If
            Return CType(Session("HightlightRecords"), Dictionary(Of String, String))
        End Get
        Set(ByVal value As Dictionary(Of String, String))
            Session("HightlightRecords") = value
        End Set
    End Property

    'Protected Function HighlightGame(ByVal psGameLineID As String) As String
    '    If HighlightRecords.ContainsKey(psGameLineID) Then
    '        If Now.Subtract(SafeDate(HighlightRecords(psGameLineID))).TotalSeconds < 4 Then
    '            Return "background-color:Yellow;"
    '        Else
    '            HighlightRecords.Remove(psGameLineID)
    '            Return ""
    '        End If
    '    End If
    '    Return ""
    'End Function


    Protected ReadOnly Property TeaserJson() As String
        Get
            Dim sResult As String = ""
            Dim sSubTeams As String
            For Each oTeaserRule As CTeaserRule In UserSession.Cache.GetTeaserRules()

                sSubTeams = ""
                For Each oTeaserTeam As CSysSetting In UserSession.SysSettings(SBCBL.std.GetSiteType & " TEASER ODDS", oTeaserRule.TeaserRuleID)
                    sSubTeams &= "{ ""TeamMember"": """ & oTeaserTeam.Key & """, ""Value"": """ & oTeaserTeam.Value & """ }, "
                Next

                sResult &= "{ ""RuleID"": """ & SafeString(oTeaserRule.TeaserRuleID) & """, ""BasketballPoint"": """ & _
                SafeString(oTeaserRule.BasketballPoint) & """, ""FootballPoint"": """ & oTeaserRule.FootbalPoint & """, ""MinTeam"": """ & _
                oTeaserRule.MinTeam & """, ""MaxTeam"": """ & oTeaserRule.MaxTeam & """, ""Teams"": [" & _
                sSubTeams & "{ ""TeamMember"": """", ""Value"": """" }] }, "
            Next

            Return "{ ""TeaserRules"": [ " & sResult & "{ ""RuleID"": """", ""Teams"": [{ ""TeamMember"": """", ""Value"": """" }] } ] };"
        End Get
    End Property

    Protected ReadOnly Property ParlayJson() As String
        Get
            Dim sResult As String = ""
            Dim nCurrent, nTigerSB As Double
            Dim olstCurrentPayout, olstTigerSBPlayout As CSysSettingList
            olstCurrentPayout = UserSession.Cache.GetSysSettings(CATEGORY_CURRENT)
            olstTigerSBPlayout = UserSession.Cache.GetSysSettings(CATEGORY_TIGERSB)

            For nIndex As Integer = 2 To 15
                nCurrent = olstCurrentPayout.GetDoubleValue(String.Format("{0} Teams", nIndex))
                nTigerSB = olstTigerSBPlayout.GetDoubleValue(String.Format("{0} Teams", nIndex))

                If nCurrent = 0 OrElse nTigerSB = 0 Then
                    sResult &= "{ ""ParlayID"": """ & nIndex & " Teams"", ""ParlayPayout"": """ & 1 & """}, "
                Else
                    sResult &= "{ ""ParlayID"": """ & nIndex & " Teams"", ""ParlayPayout"": """ & Math.Round((nTigerSB / nCurrent), 4) & """}, "
                End If

            Next

            Return "{ ""Parlays"": [ " & sResult & "{ ""ParlayID"": """", ""ParlayPayout"": ""1""} ] };"
        End Get
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property


    Protected ReadOnly Property IsDebug() As Boolean
        Get
            Return SafeString(Request("Debug")) = "Y"
        End Get
    End Property

    Private ReadOnly Property ShowSpread() As Boolean
        Get
            If ViewState("SHOW_SPREAD") Is Nothing Then
                ViewState("SHOW_SPREAD") = UserSession.SysSettings("ActionType").GetBooleanValue("Spread")
            End If

            Return CType(ViewState("SHOW_SPREAD"), Boolean)
        End Get
    End Property

    Private ReadOnly Property ShowTotalPoints() As Boolean
        Get
            If ViewState("SHOW_TOTAL_POINTS") Is Nothing Then
                ViewState("SHOW_TOTAL_POINTS") = UserSession.SysSettings("ActionType").GetBooleanValue("TotalPoints")
            End If

            Return CType(ViewState("SHOW_TOTAL_POINTS"), Boolean)
        End Get
    End Property

    Private ReadOnly Property ShowMoneyLine() As Boolean
        Get
            If ViewState("SHOW_MONEY_LINE") Is Nothing Then
                ViewState("SHOW_MONEY_LINE") = UserSession.SysSettings("ActionType").GetBooleanValue("MoneyLine")
            End If

            Return CType(ViewState("SHOW_MONEY_LINE"), Boolean)
        End Get
    End Property

    Private ReadOnly Property CanBetting() As Boolean
        Get
            If SelectedPlayerID <> "" Then
                Return (UserSession.SysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable") OrElse _
                UserSession.SysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable")) AndAlso _
                Not UserSession.Cache.GetPlayerInfo(SelectedPlayerID).IsBettingLocked AndAlso _
                UserSession.Cache.GetPlayerInfo(SelectedPlayerID).OriginalAmount > 0
            Else
                Return (UserSession.SysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable") OrElse _
                UserSession.SysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable"))
            End If

        End Get
    End Property

    Public ReadOnly Property BetTypeActive() As String
        Get
            hfBetTypeActive.Value = SafeString(Session("BetTypeActive"))
            Return SafeString(Session("BetTypeActive"))
        End Get
    End Property

    Public Property StraightError() As Boolean
        Get
            Return SafeBoolean(ViewState("StraightError"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("StraightError") = value
        End Set
    End Property

    Public Property SelectedPlayerID() As String
        Get
            If ViewState("_SELECTED_PLAYER_ID") Is Nothing Then
                If UserSession.UserType = SBCBL.EUserType.Player Then
                    ViewState("_SELECTED_PLAYER_ID") = UserSession.UserID
                End If
            End If
            ' Return "298EE9B3-0678-4519-9DAA-BA8A6A787B36"
            Return SafeString(ViewState("_SELECTED_PLAYER_ID"))
        End Get
        Set(ByVal value As String)
            ViewState("_SELECTED_PLAYER_ID") = value
            'ucWagers.SelectedPlayerID = value
        End Set
    End Property

    Public Property CallCenterSuperAgentID() As String
        Get
            Return SafeString(ViewState("_CALLCENTER_SUPER_AGENT_ID"))
        End Get
        Set(ByVal value As String)
            ViewState("_CALLCENTER_SUPER_AGENT_ID") = value
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

    ''when user click back in browser will go to BackLink
    Public Property BackLink() As String
        Get
            Return SafeString(ViewState("BackLink"))
        End Get
        Set(ByVal value As String)
            ViewState("BackLink") = value
        End Set
    End Property

    'Public ReadOnly Property ContextFilterQuery(ByVal psGameType As String, ByVal psContext As String) As String
    '    Get
    '        Dim sKey = ""
    '        psContext = UCase(psContext)
    '        Select Case (True)
    '            Case IsSoccer(psGameType)
    '                sKey = "ContextFilterQuery_Soccer_" & psContext
    '            Case IsBaseball(psGameType)
    '                sKey = "ContextFilterQuery_Baseball_" & psContext
    '            Case IsHockey(psGameType)
    '                sKey = "ContextFilterQuery_Baseball_" & psContext
    '            Case IsBaseball(psGameType)
    '                sKey = "ContextFilterQuery_Hockey_" & psContext
    '            Case IsOtherGameType(psGameType)
    '                sKey = "ContextFilterQuery_Other_" & psContext
    '            Case IsFootball(psGameType)
    '                sKey = "ContextFilterQuery_Football_" & psContext
    '            Case IsGolf(psGameType)
    '                sKey = "ContextFilterQuery_Golf_" & psContext
    '            Case IsTennis(psGameType)
    '                sKey = "ContextFilterQuery_Tennis_" & psContext
    '            Case Else
    '                Return ""
    '        End Select
    '        If Cache(sKey) Is Nothing Then
    '            Cache.Add(sKey, CreateContextFilterQuery(psGameType, psContext), Nothing, Date.Now.AddMinutes(10), Nothing, Caching.CacheItemPriority.Default, Nothing)
    '        End If
    '        Return Cache(sKey).ToString()
    '    End Get
    'End Property

    Public ReadOnly Property SuperAgentId() As String
        Get
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                Return UserSession.AgentUserInfo.SuperAgentID
            ElseIf UserSession.UserType = SBCBL.EUserType.Player Then
                Return UserSession.PlayerUserInfo.SuperAgentID
            Else
                Return CallCenterSuperAgentID
            End If
        End Get
    End Property

    Public Property PlayerCallCenterSelect() As CPlayer
        Get
            Return CType(ViewState("PLAYER_CALLCENTER_SELECT"), CPlayer)
        End Get
        Set(ByVal value As CPlayer)
            ViewState("PLAYER_CALLCENTER_SELECT") = value
        End Set
    End Property

    Public ReadOnly Property IncreaseSpread() As Integer
        Get
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                Return SelectedPlayer.IncreaseSpreadMoney
            ElseIf UserSession.UserType = SBCBL.EUserType.Player Then
                Return UserSession.PlayerUserInfo.IncreaseSpreadMoney
            Else
                Return PlayerCallCenterSelect.IncreaseSpreadMoney
            End If
        End Get
    End Property

#End Region

#Region "Page events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ViewState("Title") = Nothing
        If Request.QueryString("clear_cache") IsNot Nothing Then
            If Request.QueryString("clear_cache").Equals("remove_cache", StringComparison.CurrentCultureIgnoreCase) Then
                Cache.Remove("clear_cache")
            End If
            If Request.QueryString("clear_cache").Equals("clear_cache", StringComparison.CurrentCultureIgnoreCase) Then
                Cache.Add("clear_cache", "R", Nothing, Date.Now.AddMinutes(100), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)

            End If
            If Request.QueryString("clear_cache").Equals("clear", StringComparison.CurrentCultureIgnoreCase) Then
                Dim o As New CGameManager()

                o.UpdateDate()
            End If

        End If

        If Not IsPostBack Then

            If Not checkBetting() Then
                Return
            End If
            If (UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP") AndAlso
                (BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) _
                    OrElse BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) _
                    OrElse BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) _
                    OrElse BetTypeActive.Equals(_sReverse, StringComparison.CurrentCultureIgnoreCase)) Then
                pnBuyPoint.Visible = True
            End If
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                SelectedPlayerID = SafeString(Request.QueryString("PlayerID").Replace("#", ""))
            End If
            If UserSession.UserType = SBCBL.EUserType.Player Then
                SelectedPlayerID = SafeString(UserSession.PlayerUserInfo.UserID)
            End If
            If Not String.IsNullOrEmpty(SelectedPlayerID) Then
                Dim oCacheManager As CCacheManager = New CCacheManager()
                'ltlLoginID.Text = oCacheManager.GetPlayerInfo(SelectedPlayerID).Login
            End If
            If (UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP") AndAlso ((BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase)) OrElse (BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase))) Then
                pnBetTheBoard.Visible = True
            End If
            If BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sStraight, StringComparison.CurrentCultureIgnoreCase) Then
                pnBetTheBoard.Visible = False
            End If

            If Me.SelectedPlayerID <> "" Then
                '' select bet type, single game or prop
                'SetPageBetMode(UserSession.SelectedBetType(Me.SelectedPlayerID) <> "PROP")
                ' bindActionTypes()
                ClearWager()
                BindBettingData()
                bindSuperInfo()
            End If

            'Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
            'oSetting = oSetting.LoadByUrl(Request.Url.Host)
            'If UserSession.UserType = SBCBL.EUserType.Agent AndAlso Not String.IsNullOrEmpty(UserSession.AgentUserInfo.ColorScheme) Then
            '    hfColor.Value = UserSession.AgentUserInfo.ColorScheme
            'ElseIf UserSession.UserType = SBCBL.EUserType.Player AndAlso Not String.IsNullOrEmpty(UserSession.PlayerUserInfo.ColorScheme) Then
            '    hfColor.Value = UserSession.PlayerUserInfo.ColorScheme
            'ElseIf oSetting IsNot Nothing AndAlso oSetting.ColorScheme <> "" Then
            '    hfColor.Value = oSetting.ColorScheme
            'End If

            ' ClearWager()
            'ActiveMenu()
            ucWagers.Visible = Not pnBetAction.Visible
            hfIsWagers.Value = Not pnBetAction.Visible

            dvHeaderIfBet.Visible = LCase(BetTypeActive).Contains("if ")
            dvBottomIfBet.Visible = dvHeaderIfBet.Visible
            If dvHeaderIfBet.Visible Then
                dvHeaderOtherBet.Style("display") = "none"
                dvBottomOtherBet.Style("display") = "none"
            Else
                dvHeaderOtherBet.Style("display") = "block"
                dvBottomOtherBet.Style("display") = "block"
            End If
        End If

        'DisplayInfo()

        ucWagers.SelectedPlayerID = SelectedPlayerID
        If UserSession.SelectedGameTypes(Me.SelectedPlayerID).Count <= 0 AndAlso pnBetAction.Visible Then
            Response.Redirect(BackLink)
        End If

        SetActiveMenu()
        SetNavigationButton()

    End Sub

    Private Sub SetActiveMenu()
        Select Case UCase(BetTypeActive)
            Case "TEASER"
                Me.Page.CurrentPageName = "Sport_Teaser"
            Case "PARLAY"
                Me.Page.CurrentPageName = "Sport_Parlay"
            Case "REVERSE"
                Me.Page.CurrentPageName = "Sport_IfBet"
            Case "PROP"
                Me.Page.CurrentPageName = "Sport_Prop"
            Case "IF WIN OR PUSH"
                Me.Page.CurrentPageName = "Sport_IfBet"
            Case "IF WIN"
                Me.Page.CurrentPageName = "Sport_IfBet"
            Case "BETIFALL"
                Me.Page.CurrentPageName = "Sport_Straight"
            Case "IFBETREVERSE"
                Me.Page.CurrentPageName = "Sport_IfBet"
        End Select
    End Sub

    Private Sub SetNavigationButton()
        Select Case UCase(BetTypeActive)
            Case "TEASER"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh"
            Case "PARLAY"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh"
            Case "REVERSE"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh Lines"
            Case "PROP"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh"
            Case "IF WIN OR PUSH"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh Lines"
            Case "IF WIN"
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh Lines"
            Case "BETIFALL"
                btnMainMenu.Text = "Main Menu"
                btnUpdateLines.Text = "Refresh"
            Case Else
                btnMainMenu.Text = "Wager Menu"
                btnUpdateLines.Text = "Refresh"
        End Select
    End Sub

    'Protected Sub btnUpdateLines_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLines.Click, lbtUpdateLineTop.Click
    '    'BindGameLines()
    '    ViewState("Title") = Nothing
    '    BetTypeActive = _sStraight
    '    ClearWager()
    '    ActiveMenu()
    'End Sub
#End Region


    Private Sub bindSuperInfo()
        Dim oSuperManager As New CSuperUserManager()
        Dim odtSuperAdmin As New DataTable
        If UserSession.UserType = SBCBL.EUserType.Player Then
            odtSuperAdmin = oSuperManager.GetSuperByID(UserSession.PlayerUserInfo.SuperAdminID)
        End If
        If UserSession.UserType = SBCBL.EUserType.Agent Then
            odtSuperAdmin = oSuperManager.GetSuperByID(UserSession.AgentUserInfo.SuperAdminID)
        End If
        If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
            Dim oCacheManager As CCacheManager = New CCacheManager()
            odtSuperAdmin = oSuperManager.GetSuperByID(oCacheManager.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID).SuperAdminID)
        End If

    End Sub

    Private Function checkBetting() As Boolean
        '' Allow Player Betting only when BETTING_ENABLE =TRUE or OVERRIDE_BETTING_ENABLE = TRUE
        If Not CanBetting Then
            If SelectedPlayerID <> "" Then
                If UserSession.Cache.GetPlayerInfo(SelectedPlayerID).IsBettingLocked Then '' Locked betting
                    lblMessage.Text = "Your account has been locked for placing a bet. Please contact your agent."
                ElseIf UserSession.Cache.GetPlayerInfo(SelectedPlayerID).OriginalAmount = 0 Then '' Empty Amount
                    lblMessage.Text = "Please call your agent to reactivate your account."
                Else '' Service crash, lock betting whole users
                    lblMessage.Text = "All betting has been disabled. Please contact your administrator for more information."
                End If

            Else '' Service crash, lock betting whole users
                lblMessage.Text = "All betting has been disabled. Please contact your administrator for more information."
            End If

            rptMain.Visible = False
            ucWagers.Visible = False
            hfIsWagers.Value = False
            rptProps.Visible = False
            lblMessage.Visible = True
        Else
            lblMessage.Visible = False
        End If

        Return CanBetting
    End Function

    Public Sub BindBettingData()
        BindGameLines()
        If UserSession.SelectedBetType(Me.SelectedPlayerID) = "PROP" Then
            'lblQuarterOnly.Text = "true"
            Return
        End If
        Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
        For Each sGameType As String In olstGameType
            If IsBasketball(sGameType) OrElse IsFootball(sGameType) Then
                Dim oGame As CGame = CType(Session("GameSelect"), Dictionary(Of String, CGame))(sGameType)
                If oGame.Current Then
                    'lblDisableTeaser.Text = ""
                    Return
                End If
            End If
        Next
        'lblDisableTeaser.Text = "disable"
        For Each sGameType As String In olstGameType
            If IsBasketball(sGameType) OrElse IsFootball(sGameType) Then
                Dim oGame As CGame = CType(Session("GameSelect"), Dictionary(Of String, CGame))(sGameType)
                If oGame.Current Or oGame.OneH Or oGame.TwoH Then
                    'lblQuarterOnly.Text = ""
                    Return
                End If
            Else
                'lblQuarterOnly.Text = ""
                Return
            End If
        Next
        'lblQuarterOnly.Text = "true"
    End Sub

    Public Sub BindGameLines()
        '' NOTICE: this way to help last update time display right, and the server time must be PST
        Dim oCurrent As DateTime = UserSession.ConvertToEST(DateTime.Now.ToUniversalTime())
        ' lblCurrent.Text = SafeString(oCurrent)
        '' lblLastUpdatetop.ForeColor = btnUpdateLines.ForeColor
        ''lblLastUpdatetop.Text = "(Last Update: " & SafeString(oCurrent) & " EST)"
        lblLastUpdateBottom.Text = "Lines effective : " & SafeString(oCurrent) & " EST"
        '_sHightlightJS = ""
        '' store current result to LastRecords for compare later
        _TempLastRecords.Clear()

        If Not checkBetting() Then
            Exit Sub
        End If

        If Me.SelectedPlayerID = "" Then
            Return
        End If

        Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)

        If UserSession.SelectedBetType(Me.SelectedPlayerID) = "PROP" Then
            ' DisableMenu()
            olstGameType = olstGameType.FindAll(Function(x) x.StartsWith("Prop_"))

        Else
            olstGameType = olstGameType.FindAll(Function(x) Not x.StartsWith("Prop_"))
        End If

        '' Create Private 
        'OddsRulesEngine_oOddsRuleEngine = New COddRulesEngine(UserSession.Cache.GetAllOddsRules(SBCBL.std.GetSiteType), olstGameType, SelectedPlayer.SuperAdminID, True, SelectedPlayer.Template, SelectedPlayer.SuperAgentID)
        _oOddsRuleEngine = New COddRulesEngine(olstGameType, SelectedPlayer.SuperAdminID, True, SelectedPlayer.Template, SelectedPlayer.SuperAgentID)

        LoadGameTypes(olstGameType)

        'show mgs if dont have data
        If Not UserSession.SelectedBetType(Me.SelectedPlayerID) = "PROP" Then
            rptMain.Visible = _bHaveBettingData
            lblBettingMsg.Visible = Not _bHaveBettingData
        End If


    End Sub

    Private Sub LoadGameTypes(ByVal poGameTypes As List(Of String))

        If poGameTypes.Count = 0 Then
            pnBetAction.Visible = False

            Return
        End If
        If UserSession.SelectedBetType(Me.SelectedPlayerID) = "SINGLE" Or UserSession.SelectedBetType(Me.SelectedPlayerID) = "" Then
            'Dim oAddedGame(poGameTypes.Count) As String
            'For nIndex As Integer = 0 To poGameTypes.Count - 1
            '    oAddedGame(nIndex) = poGameTypes(nIndex) & " ADDED GAME"
            'Next
            'poGameTypes.AddRange(oAddedGame)
            'poGameTypes.Sort()

            rptMain.DataSource = poGameTypes
            rptMain.DataBind()
        Else
            '' Prop game type has Prop_ prefix, we have to remove this
            For nIndex As Integer = 0 To poGameTypes.Count - 1
                poGameTypes(nIndex) = poGameTypes(nIndex).Remove(0, "Prop_".Length)
            Next
            rptProps.DataSource = poGameTypes
            rptProps.DataBind()
        End If
    End Sub

    ''get bookmaker from agent to super
    Public Function GetBookMakerType(ByVal psKey As String, ByVal psAgentID As String, ByVal psBookMakerType As String) As String
        Dim sAgentID As String = psAgentID
        Dim sBookMakerType As String = psBookMakerType
        While True
            Dim sBookMakerValue As String = UserSession.Cache.GetSysSettings(sBookMakerType).GetValue(psKey)
            If Not String.IsNullOrEmpty(sBookMakerValue) Then
                Return sBookMakerType
            End If
            If Not String.IsNullOrEmpty(UserSession.Cache.GetAgentInfo(sAgentID).ParentID) Then
                sAgentID = UserSession.Cache.GetAgentInfo(sAgentID).ParentID
                sBookMakerType = sAgentID + "_BookmakerType"
            Else
                Return _SuperBookmakerValue
            End If
        End While

        Return _SuperBookmakerValue
    End Function

    ''get BookMaker for load game
    Private Function GetAgentBookMaker(ByVal psGameType As String) As String
        If UserSession.UserType = SBCBL.EUserType.Agent Then
            Return GetBookMakerType(psGameType, UserSession.UserID, SafeString(UserSession.UserID & "_BookmakerType"))
        End If
        If UserSession.UserType = SBCBL.EUserType.Player Then
            Return GetBookMakerType(psGameType, UserSession.PlayerUserInfo.SuperAgentID, SafeString(UserSession.PlayerUserInfo.SuperAgentID & "_BookmakerType"))
        End If
        If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
            Return GetBookMakerType(psGameType, UserSession.Cache.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID).AgentID, SafeString(UserSession.Cache.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID).AgentID & "_BookmakerType"))
        End If
        Return "Pinnacle2"
    End Function

    Private Function LoadGames(ByVal psGameType As String, ByVal poRepeater As Repeater, Optional ByVal oDTAdded As Data.DataTable = Nothing) As Boolean
        ''Get primary bookmakers by gametype
        Dim sBookmaker As String = ""
        Dim oToday = UserSession.ConvertToEST(DateTime.Now.ToUniversalTime()) 'GetEasternDate()
        ' Dim oDTAddedGames As DataTable
        'If (psGameType.Contains("Football") OrElse psGameType.Contains("Basketball")) AndAlso (UserSession.Cache.GetFixSpreadMoney(SuperAgentId) IsNot Nothing AndAlso UserSession.Cache.GetFixSpreadMoney(SuperAgentId).UseFixedSpreadMoney) Then
        '    sBookmaker = SuperAgentId & " Manipulation"
        'Else
        '    sBookmaker = UserSession.SysSettings(GetAgentBookMaker(psGameType)).GetValue(psGameType)
        'End If
        If psGameType.Contains("Live") Then
            sBookmaker = "Pinnacle2"
        Else
            sBookmaker = UserSession.SysSettings(GetAgentBookMaker(psGameType.Replace("Live", "").Trim())).GetValue(psGameType.Replace("Live", "").Trim())
        End If
        ' '' if doesn't found book maker, just return, SINCE NOW WE DOESNT FILTER BY BOOK MAKER

        If oDTAdded Is Nothing AndAlso sBookmaker = "" Then
            poRepeater.DataSource = Nothing
            poRepeater.DataBind()
            Return False
        End If
        _log.Debug(sBookmaker)
        '' get game select from page select game
        Dim oGame As CGame = CType(Session("GameSelect"), Dictionary(Of String, CGame))(psGameType)

        '' Filter by offline time 
        Dim oSysSettingManager As New CSysSettingManager()
        Dim oSys As CSysSetting = UserSession.Cache.GetAllSysSettings(_sOffTimeCategory).Find(Function(x) x.Key = psGameType)

        ''Get games
        ' Dim oGameShows As New Dictionary(Of String, DataTable)
        Dim odtGames As DataTable
        Dim nTotalGame As Integer = 0
        Dim oListLines As New Dictionary(Of String, DataRow)


        'LogDebug(_log, "thuong :" & psGameType)
        If psGameType.Contains("Live") Then
            odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday.AddDays(-1), True, True, False, Nothing, IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), sBookmaker), SuperAgentId, True)
        Else
            odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday, True, True, False, Nothing, IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), sBookmaker), SuperAgentId, True)

        End If

        'odtGames = New SBCBL.Managers.CGameManager().GetAvailableGamesForSuper(psGameType, oToday, sBookmaker)

        Dim oGameShows As New Dictionary(Of String, DataTable)
        'Dim oGameHalfTimeOff As CGameHalfTimeOFF = UserSession.Cache.GetGameHalfTimeOFF(SuperAgentId).GetGameHalfTimeOff(psGameType)
        If oGame.Current Then
            If Cache.Get("clear_cache") Is Nothing Then
                oGameShows.Add("Current", Nothing)
            End If
        End If
        If UserSession.Cache.GetSysSettings(SuperAgentId & "PartTimeDisplay", psGameType) IsNot Nothing Then
            If Not UserSession.FirstHalfOff(psGameType) AndAlso oGame.OneH And Not UserSession.Cache.GetSysSettings(SuperAgentId & "PartTimeDisplay", psGameType).GetBooleanValue("FirstHOff") Then
                oGameShows.Add("1H", Nothing)
            End If
            If Not UserSession.IsSecondhalfOff(psGameType) AndAlso oGame.TwoH And Not UserSession.Cache.GetSysSettings(SuperAgentId & "PartTimeDisplay", psGameType).GetBooleanValue("SecondHOff") Then
                If Cache.Get("clear_cache") Is Nothing Then
                    oGameShows.Add("2H", Nothing)
                End If
            End If
        Else
            If Not UserSession.FirstHalfOff(psGameType) AndAlso oGame.OneH Then
                oGameShows.Add("1H", Nothing)
            End If
            If Not UserSession.IsSecondhalfOff(psGameType) AndAlso oGame.TwoH Then
                oGameShows.Add("2H", Nothing)
            End If
        End If
        If UCase(BetTypeActive.Replace(_sBetTheBoard, _sStraight)) = "STRAIGHT" OrElse BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) Then
            If Not UserSession.Cache.GetSysSettings(SuperAgentId, psGameType).GetBooleanValue("1QOff") Then
                If oGame.Quarter1 Then
                    oGameShows.Add("1Q", Nothing)
                End If

            End If
            If Not UserSession.Cache.GetSysSettings(SuperAgentId, psGameType).GetBooleanValue("2QOff") Then
                If oGame.Quarter2 Then
                    oGameShows.Add("2Q", Nothing)
                End If
            End If
            If Not UserSession.Cache.GetSysSettings(SuperAgentId, psGameType).GetBooleanValue("3QOff") Then
                If oGame.Quarter3 Then
                    oGameShows.Add("3Q", Nothing)
                End If

            End If
            If Not UserSession.Cache.GetSysSettings(SuperAgentId, psGameType).GetBooleanValue("4QOff") Then
                If oGame.Quarter4 Then
                    oGameShows.Add("4Q", Nothing)
                End If
            End If
        End If



        For Each sContext As String In oGameShows.Keys.ToArray()
            '' reset the list of line for new Context
            oListLines.Clear()


            Dim sWhere As String = "" 'ContextFilterQuery(psGameType, sContext) '""
            If sWhere <> "" Then
                sWhere &= " and Context = " & SQLString(sContext) & " and (LiveOpen is null  or  LiveOpen='Y') "
            Else
                sWhere = "Context = " & SQLString(sContext) & " and  (LiveOpen is null  or  LiveOpen='Y') "
            End If

            'LineOffHour Dim lstGameOnOffCurrent As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(SuperAgentId, psGameType, True)
            'Dim lstGameOnOff1H As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(SuperAgentId, psGameType, False)
            If Not UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour").Count > 0 Then


                'Dim oGameOnOffCurrent As CGameTypeOnOff = lstGameOnOffCurrent(0)
                '' filter by line off time for agent
                If UCase(sContext) <> "2H" Then
                    If UCase(sContext) = "1H" AndAlso UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").Count > 0 Then 'AndAlso lstGameOnOff1H.Count > 0 Then
                        'Dim oGameOnOff1H As CGameTypeOnOff = lstGameOnOff1H(0)
                        sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").GetIntegerValue("OffBefore", psGameType)) 'oGameOnOff1H.OffBefore)
                    ElseIf UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").Count > 0 Then
                        'ClientAlert(UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("OffBefore", psGameType))
                        If Not psGameType.Contains("Live") Then
                            sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("OffBefore", psGameType))
                        End If
                    End If
                End If
                If UCase(sContext) <> "2H" Then
                    If UCase(sContext) = "1H" AndAlso UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").Count > 0 Then 'AndAlso lstGameOnOff1H.Count > 0 Then
                        'Dim oGameOnOff1H As CGameTypeOnOff = lstGameOnOff1H(0)
                        sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").GetIntegerValue("DisplayBefore", psGameType) * 60).ToString()) '(oGameOnOff1H.DisplayBefore * 60).ToString())
                    ElseIf UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").Count > 0 Then
                        'ClientAlert((UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("DisplayBefore", psGameType) * 60).ToString())
                        If Not psGameType.Contains("Live") Then
                            sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("DisplayBefore", psGameType) * 60).ToString()) '(oGameOnOffCurrent.DisplayBefore * 60).ToString())
                        End If
                    End If
                End If

            Else
                '' filter by line off time for super
                If UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.Value) <> 0 Then
                    sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", SafeInteger(oSys.Value).ToString())
                End If

                If UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.SubCategory) <> 0 Then
                    sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (SafeInteger(oSys.SubCategory) * 60).ToString())
                End If
            End If

            '' Quarter line off: turn off Quarter line after a period time that set by super, applies to football, basketball
            If IsFootball(psGameType) Or IsBasketball(psGameType) Then

                If sContext = "1Q" Or sContext = "2Q" Or sContext = "3Q" Or sContext = "4Q" Then
                    Dim nQTimeoff As Integer = 0
                    Dim nQTimeDisplay As Integer = 0
                    Dim sSportType = IIf(IsBasketball(psGameType), "Basketball", "Football")
                    'Dim oGameQuarterDisplayList As CGameTeamDisplayList = UserSession.Cache.GetGameTeamDisplay(SuperAgentId, sSportType)
                    'Dim oGameQuarterDisplay As CGameTeamDisplay = oGameQuarterDisplayList.GetGameTeamDisplay(sSportType)
                    If UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour") IsNot Nothing Then
                        nQTimeoff = UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", sSportType).GetIntegerValue("OffMinutes", psGameType)
                        nQTimeDisplay = UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", sSportType).GetIntegerValue("DisplayHours", psGameType)
                        If nQTimeoff <> 0 Then
                            sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", nQTimeoff)

                        End If
                        If nQTimeDisplay <> 0 Then
                            sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (nQTimeDisplay * 60).ToString())
                        End If
                    End If
                End If
            End If

            odtGames.DefaultView.RowFilter = sWhere

            Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()
            ' oDTFiltered.DefaultView.Sort = " dategroup,IsAddedGame asc"
            '' 2H line off: turn off 2H line after a period time that set by super, applies to football, basketball, soccer
            If sContext = "2H" Then
                Dim nTimeoff As Integer = 0
                '  Dim oGameHalfTimeDisplayList As CGameHalfTimeDisplayList = UserSession.Cache.GetGameHalfTimeDisplay(SuperAgentId)
                ' nTimeoff = oGameHalfTimeDisplayList.GetGameHalfTimeDisplayByGameType(psGameType)
                nTimeoff = UserSession.SecondHalfTimeOff(psGameType)
                If nTimeoff <> 0 Then
                    sWhere = "MinuteBefore2H  <= " & SafeString(nTimeoff)
                    sWhere &= " and gamedate < " & SQLString(GetEasternDate())
                    oDTFiltered.DefaultView.RowFilter = sWhere
                    ' oDTFiltered.DefaultView.Sort = " dategroup,IsAddedGame asc"
                    'oDTFiltered.DefaultView.Sort = " GameDate asc"
                    'oDTFiltered = oDTFiltered.DefaultView.ToTable()

                End If
            End If

            oDTFiltered.DefaultView.Sort = " GameDate asc"
            oDTFiltered = oDTFiltered.DefaultView.ToTable()
            oGameShows(sContext) = oDTFiltered
            If oDTFiltered Is Nothing OrElse oDTFiltered.Rows.Count = 0 Then
                oGameShows.Remove(sContext)
            End If

        Next
        poRepeater.DataSource = oGameShows
        poRepeater.DataBind()

        Return nTotalGame > 0
    End Function

    '' this create filter to avoide weird line
    'Private Function CreateContextFilterQuery(ByVal psGameType As String, ByVal psContext As String) As String
    '    Dim sCategory As String = SBCBL.std.GetSiteType & " LineRules"
    '    Dim oSysManager As New CSysSettingManager()
    '    Dim oCachemanager As New CCacheManager()
    '    Dim oListWhere As New List(Of String)
    '    Dim oSysGame As CSysSetting = oCachemanager.GetAllSysSettings(_sGameType).Find(Function(x) x.Key = psGameType AndAlso x.SubCategory <> "")
    '    Dim oSysSport As CSysSetting = oCachemanager.GetSysSettings(_sGameType).Find(Function(x) x.SysSettingID = oSysGame.SubCategory)

    '    Dim sGameTypeKey As String = oSysSport.Key.Replace(" ", "_")
    '    Dim sSubCategory As String = sGameTypeKey & "_" & psContext

    '    Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory, sSubCategory)
    '    Dim oSpreadMoneyGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyGT")
    '    Dim nAwaySpreadMoneyGT As Double = 0
    '    If oSpreadMoneyGT IsNot Nothing Then
    '        nAwaySpreadMoneyGT = SafeDouble(oSpreadMoneyGT.Value)
    '    End If
    '    Dim oSpreadMoneyLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "AwaySpreadMoneyLT")
    '    Dim nAwaySpreadMoneyLT As Double = 0
    '    If oSpreadMoneyLT IsNot Nothing Then
    '        nAwaySpreadMoneyLT = SafeDouble(oSpreadMoneyLT.Value)
    '    End If

    '    oSpreadMoneyGT = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyGT")
    '    Dim nHomeSpreadMoneyGT As Integer = 0
    '    If oSpreadMoneyGT IsNot Nothing Then
    '        nHomeSpreadMoneyGT = SafeDouble(oSpreadMoneyGT.Value)
    '    End If

    '    oSpreadMoneyLT = oListSettings.Find(Function(x) x.Key = "HomeSpreadMoneyLT")
    '    Dim nHomeSpreadMoneyLT As Integer = 0
    '    If oSpreadMoneyLT IsNot Nothing Then
    '        nHomeSpreadMoneyLT = SafeDouble(oSpreadMoneyLT.Value)
    '    End If

    '    Dim oTotalPointGT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointGT")
    '    Dim nTotalPointGT As Double = -1000
    '    If oTotalPointGT IsNot Nothing Then
    '        nTotalPointGT = SafeDouble(oTotalPointGT.Value)
    '    End If

    '    Dim oTotalPointLT As CSysSetting = oListSettings.Find(Function(x) x.Key = "TotalPointLT")
    '    Dim nTotalPointLT As Double = -1000
    '    If oTotalPointLT IsNot Nothing Then
    '        nTotalPointLT = SafeDouble(oTotalPointLT.Value)
    '    End If

    '    If IsBaseball(psGameType) Or IsHockey(psGameType) Then
    '        If nAwaySpreadMoneyGT <> 0 And nAwaySpreadMoneyLT <> 0 Then
    '            oListWhere.Add(String.Format("(AwayMoneyLine >  {0} and AwayMoneyLine < {1})", SafeString(nAwaySpreadMoneyGT), SafeString(nAwaySpreadMoneyLT)))
    '        End If
    '        If nHomeSpreadMoneyGT <> 0 And nHomeSpreadMoneyLT <> 0 Then
    '            oListWhere.Add(String.Format("(HomeMoneyLine >  {0} and HomeMoneyLine  < {1})", SafeString(nHomeSpreadMoneyGT), SafeString(nHomeSpreadMoneyLT)))
    '        End If
    '    End If

    '    If nAwaySpreadMoneyGT <> 0 And nAwaySpreadMoneyLT <> 0 Then
    '        oListWhere.Add(String.Format("(AwaySpreadMoney >  {0} and AwaySpreadMoney < {1})", SafeString(nAwaySpreadMoneyGT), SafeString(nAwaySpreadMoneyLT)))
    '    End If

    '    If nHomeSpreadMoneyGT <> 0 And nHomeSpreadMoneyLT <> 0 Then
    '        oListWhere.Add(String.Format("(HomeSpreadMoney >  {0} and HomeSpreadMoney  < {1})", SafeString(nHomeSpreadMoneyGT), SafeString(nHomeSpreadMoneyLT)))
    '    End If

    '    If nTotalPointGT <> -1000 And (nTotalPointLT <> -1000) Then
    '        oListWhere.Add(String.Format("(TotalPoints  >  {0} and TotalPoints < {1})", SafeString(nTotalPointGT), nTotalPointLT.ToString()))
    '    End If

    '    If oListWhere.Count = 0 Then
    '        oListWhere.Add("1 = 1")
    '    End If

    '    Return "(" & String.Join(" and ", oListWhere.ToArray()) & ")"
    'End Function

    'Private Sub bindActionTypes()
    '    Dim oSysSettings As New List(Of String)

    '    '' Disable Teaser if didn't setup teaser rule yet
    '    Dim oTeaserRules As CTeaserRuleList = UserSession.Cache.GetTeaserRules()
    '    For Each oSys As CSysSetting In UserSession.SysSettings("BetType")
    '        If oSys.Key = _sTeaser AndAlso oTeaserRules.Count = 0 Then
    '            Continue For
    '        End If
    '        oSysSettings.Add(oSys.Key)
    '    Next

    '    If Not oSysSettings.Contains(_sParlay) Then
    '        Parlay.Visible = False
    '    Else
    '        Parlay.Visible = True
    '    End If
    '    If Not oSysSettings.Contains(_sReverse) Then
    '        Reverse.Visible = False
    '    Else
    '        Reverse.Visible = True
    '    End If
    '    If Not oSysSettings.Contains(_sStraight) Then
    '        Straight.Visible = False
    '    Else
    '        Straight.Visible = True
    '    End If
    '    If Not oSysSettings.Contains(_sTeaser) Then
    '        Teaser.Visible = False
    '    Else
    '        Teaser.Visible = True
    '    End If

    'End Sub

#Region "Bet functions"
    Public Overrides Property Visible() As Boolean
        Get
            Return MyBase.Visible
        End Get
        Set(ByVal value As Boolean)
            '' Rebind wagers when user want to bet more games
            'If value Then
            '    ucWagers.BindWagers()
            'End If

            MyBase.Visible = value
        End Set
    End Property

    Private Function checkValid(ByVal psGameType As String) As Boolean


        Dim sInvalid As String = ""
        If psGameType <> "" Then
            sInvalid = validGameType(UCase(BetTypeActive), psGameType)
        End If

        If sInvalid <> "" Then
            ClientAlert(sInvalid, True)
            Return False
        End If

        If UserSession.SelectedTicket(Me.SelectedPlayerID).Preview Then
            ClientAlert("Please Press Cancel Or Back Button Before You Make Any Wager Adjustment.", True)
            Return False
        End If

        Return True
    End Function

    Private Function validGameType(ByVal psAction As String, ByVal psGameType As String) As String
        Select Case UCase(psAction)
            Case "TEASER"
                If Not (IsBasketball(psGameType) OrElse _
                        IsFootball(psGameType)) Then
                    Return String.Format("{0} Doesn't Have Teaser Type.", psGameType)
                End If

        End Select

        Return ""
    End Function

    Private Function getSpeadBet(ByVal pnSpread As Double, ByVal pnSpreadMoney As String, Optional ByVal psChoice As String = "HOME") As CTicketBet
        Dim oTicketBet As New CTicketBet()
        oTicketBet.BetType = "Spread"

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

    Private Function getTotalPointsBet(ByVal pnTotalPoints As Double, ByVal pnTotalPointsMoney As String, Optional ByVal psChoice As String = "OVER") As CTicketBet
        Dim oTicketBet As New CTicketBet()
        oTicketBet.BetType = "TotalPoints"
        oTicketBet.TotalPoints = pnTotalPoints

        Select Case UCase(psChoice)
            Case "OVER"
                oTicketBet.TotalPointsOverMoney = pnTotalPointsMoney

            Case "UNDER"
                oTicketBet.TotalPointsUnderMoney = pnTotalPointsMoney
        End Select

        Return oTicketBet
    End Function

    Private Function getTeamTotalTotalPointsBet(ByVal pnTotalPoints As Double, ByVal pnTotalPointsMoney As String, Optional ByVal psChoice As String = "OVER") As CTicketBet
        Dim oTicketBet As New CTicketBet()
        oTicketBet.BetType = "TeamTotalPoints"
        oTicketBet.TotalPoints = pnTotalPoints
        Select Case UCase(psChoice)
            Case "OVER"
                oTicketBet.TotalPointsOverMoney = pnTotalPointsMoney
            Case "UNDER"
                oTicketBet.TotalPointsUnderMoney = pnTotalPointsMoney
        End Select

        Return oTicketBet
    End Function

    Private Function getMoneyLineBet(ByVal pnMoneyLine As Double, Optional ByVal psChoice As String = "HOME") As CTicketBet
        Dim oTicketBet As New CTicketBet()
        oTicketBet.BetType = "MoneyLine"

        Select Case UCase(psChoice)
            Case "HOME"
                oTicketBet.HomeMoneyLine = pnMoneyLine

            Case "AWAY"
                oTicketBet.AwayMoneyLine = pnMoneyLine
        End Select

        Return oTicketBet
    End Function

    Private Function getPropLineBet(ByVal pnMoneyLine As Double) As CTicketBet
        Dim oTicketBet As New CTicketBet()
        oTicketBet.BetType = "MoneyLine"

        oTicketBet.PropMoneyLine = pnMoneyLine
        oTicketBet.IsForProp = True

        Return oTicketBet
    End Function

#End Region

#Region "Game Types"

#End Region

    Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
        Dim sType As String = SafeString(e.Item.DataItem)
        Dim rptBets As Repeater = e.Item.FindControl("rptBets")


        If Not sType.EndsWith("ADDED GAME") Then

            LoadGames(sType, rptBets)
        Else
            rptBets.Visible = False
        End If
    End Sub

    Protected Sub rptBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Footer Or e.Item.ItemType = ListItemType.Header Or e.Item.ItemType = ListItemType.Pager Then
            Return
        End If
        'Dim lblGameType = CType(e.Item.FindControl("lblGameType"), Label)
        Dim lblGameDate = CType(e.Item.FindControl("lblGameDate"), Label)
        'Dim lblGameContext = CType(e.Item.FindControl("lblGameContext"), Label)
        Dim rptGameLines As Repeater = CType(e.Item.FindControl("rptGameLines"), Repeater)
        Dim oDT As DataTable = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable)).Value
        If oDT IsNot Nothing AndAlso oDT.Rows.Count > 0 Then
            'lblGameType.Text = oDT.Rows(0)("GameType").Replace("NCAA Football", "College Football").Replace("CFL Football", "Canadian Football").Replace("AFL Football", "Arena Football")
            'lblGameContext.Text = "( " + oDT.Rows(0)("Context").Replace("Current", "Game").Replace("1H", "1st Half").Replace("2H", "2st Half").Replace("1Q", "1st Quarter").Replace("2Q", "2nd Quarter").Replace("3Q", "3rd Quarter").Replace("4Q", "4th Quarter") + " )"
            'lblGameDate.Text = "Line from : " + CType(oDT.Rows(0)("GameDate"), Date).ToString("MM/dd/yyyy")

            ' show maximum wager
            Dim lblLimitWager = CType(e.Item.FindControl("lblLimitWager"), Label)
            If lblLimitWager IsNot Nothing Then
                Dim tm = New CTicket(BetTypeActive.Replace(_sBetTheBoard, _sStraight).Replace(__BetIfAll, _sStraight), SuperAgentId, SelectedPlayerID)
                Dim maxBetLimit = tm.getMaxBetAmount(New List(Of String)(New String() {oDT.Rows(0)("Context")}), New List(Of String)(New String() {UCase(oDT.Rows(0)("GameType"))}), SelectedPlayer.Template)
                Dim minBetLimit = tm.getMinBetAmount(SelectedPlayer.Template, UserSession.UserTypeOfBet)

                lblLimitWager.Text = String.Format("Maximum Wager: {0}. Minimum Wager: {1}", FormatCurrency(maxBetLimit), FormatCurrency(minBetLimit))
            End If

        End If

        If chkTeamTotal.Checked AndAlso oDT.Rows.Count > 0 Then
            If (Not BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) AndAlso Not BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) AndAlso Not BetTypeActive.Equals(_sStraight, StringComparison.CurrentCultureIgnoreCase)) OrElse SafeDouble(oDT.Rows(0)("AwayTeamTotalPoints")) = 0 OrElse (SafeInteger(oDT.Rows(0)("MinuteBefore")) / 60) > _nOFFTeamTotal Then
                e.Item.Visible = False
            Else
                rptGameLines.DataSource = oDT
                rptGameLines.DataBind()
            End If
        Else
            rptGameLines.DataSource = oDT
            rptGameLines.DataBind()
        End If

    End Sub

    Protected Sub rptGameLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim lblGameContext = CType(e.Item.FindControl("lblGameContext"), Label)
            lblGameContext.Text = e.Item.DataItem("Context").Replace("Current", "Game").Replace("1H", "1st Half").Replace("2H", "2st Half").Replace("1Q", "1st Quarter").Replace("2Q", "2nd Quarter").Replace("3Q", "3rd Quarter").Replace("4Q", "4th Quarter").ToString()
            If SafeString(ViewState("OddEven")) = "odd" Then
                ViewState("OddEven") = "even"
            Else
                ViewState("OddEven") = "odd"
            End If
            _bHaveBettingData = True
            ' Dim ltlTitle As Literal = CType(e.Item.FindControl("ltlTitle"), Literal)

            Dim lblDescription As Literal = CType(e.Item.FindControl("lblDescription"), Literal)
            Dim trDescription As HtmlTableRow = CType(e.Item.FindControl("trDescription"), HtmlTableRow)
            Dim trtop As HtmlTableRow = CType(e.Item.FindControl("trtop"), HtmlTableRow)
            'Dim thGameLineHeader As HtmlTableCell = CType(e.Item.FindControl("thGameLineHeader"), HtmlTableCell)
            Dim sAddedGame As String = CType(e.Item.Parent.Parent.Parent.Parent, RepeaterItem).DataItem.ToString.Replace(SafeString(e.Item.DataItem("GameType")) + " ADDED GAME", "<span class='add_game'>(ADDED GAME)</span>")

            If e.Item.DataItem("Context") = "Current" Then
                _contextGame += 1
                _context1H = 1
            ElseIf e.Item.DataItem("Context") = "1H" Then
                _contextGame = 1
                _context1H += 1
            End If
            If Not sAddedGame.Contains("ADDED GAME") Then
                sAddedGame = ""
            End If
            If SafeString(e.Item.DataItem("IsAddedGame")).Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
                sAddedGame = "(ADDED GAME)"
            End If
            If Not chkTeamTotal.Checked OrElse (Not BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) AndAlso Not BetTypeActive.Equals(_sStraight, StringComparison.CurrentCultureIgnoreCase) AndAlso Not BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase)) OrElse SafeDouble(e.Item.DataItem("AwayTeamTotalPoints")) = 0 OrElse (SafeInteger(e.Item.DataItem("MinuteBefore")) / 60) > _nOFFTeamTotal Then
                CType(e.Item.FindControl("tdTeam_Total_HomeUnder"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTeam_Total_HomeOver"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTeam_Total_AwayUnder"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTeam_Total_AwayOver"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdDescript1"), HtmlTableCell).ColSpan = 6
                CType(e.Item.FindControl("tdDescript2"), HtmlTableCell).ColSpan = 6
                CType(e.Item.FindControl("tdDrawLast"), HtmlTableCell).Visible = True
                CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).Visible = False
            End If
            Dim sTitle As String = String.Format(" <h3 class='top_head_title'> {0} </h3>   <span class='red'>({2})</span> - Game - <span>{3}</span><span class='red'>{1}</span>", SafeString(e.Item.DataItem("GameType")), sAddedGame, SafeString(e.Item.DataItem("Context")).Replace("Current", "FullGame"), SafeDate(e.Item.DataItem("GameDate")).ToString("MM/dd/yyyy"))
            Dim sDescription = SafeString(e.Item.DataItem("Description"))
            '  ltlTitle.Text = sTitle
            lblDescription.Text = "* Game Note: " & sDescription
            If Description.Equals(sDescription, StringComparison.CurrentCultureIgnoreCase) Then
                lblDescription.Visible = False
                trDescription.Visible = False
            Else
                trDescription.Visible = True
                lblDescription.Visible = True
                Description = sDescription
            End If
            trDescription.Attributes("class") = "offering_pair_" & IIf(SafeString(ViewState("OddEven")).Equals("odd", StringComparison.CurrentCultureIgnoreCase), "even", "odd") & " " & trDescription.Attributes("class")
            If ViewState("Title") Is Nothing Then
                ViewState("Title") = sTitle
                'If SafeString(e.Item.DataItem("Context")).Contains("Q") Then
                '    'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#6699CD   "
                '    'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#6699CD   "
                '    'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#6699CD   "
                '    'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#6699CD   "
                '    'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#6699CD   "
                '    trtop.BgColor = "#6699CD "

                'ElseIf SafeString(e.Item.DataItem("Context")).Contains("1H") Then
                '    'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#8BB381"
                '    trtop.BgColor = "#8BB381"
                'ElseIf SafeString(e.Item.DataItem("Context")).Contains("2H") Then
                '    'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#8BB381"
                '    'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#8BB381"
                '    trtop.BgColor = "#8BB381"
                'End If
            Else
                If CType(ViewState("Title"), String).Equals(sTitle) Then
                    '  ltlTitle.Visible = False
                    'thGameLineHeader.Visible = False
                    e.Item.FindControl("tdSpread").Visible = True
                    e.Item.FindControl("tdTotal").Visible = True
                    e.Item.FindControl("tdTeamTotal").Visible = False
                    e.Item.FindControl("tdMLine").Visible = True
                    e.Item.FindControl("tdTeam").Visible = True
                    e.Item.FindControl("tdDate").Visible = True
                    e.Item.FindControl("tdNum").Visible = True
                    'e.Item.FindControl("trLoad").Visible = False
                Else
                    ViewState("Title") = sTitle
                    ' ltlTitle.Visible = True
                    ' thGameLineHeader.Visible = True
                    ' thGameLineHeader.Style.Add("padding-top", "20px")
                    If SafeString(e.Item.DataItem("Context")).Contains("Q") Then
                        'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#6699CD   "
                        'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#6699CD   "
                        'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#6699CD   "
                        'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#6699CD   "
                        'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#6699CD   "
                        ' trtop.BgColor = "#6699CD"
                    ElseIf SafeString(e.Item.DataItem("Context")).Contains("1H") Then
                        'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#8BB381"
                        'trtop.BgColor = "#8BB381"
                    ElseIf SafeString(e.Item.DataItem("Context")).Contains("2H") Then
                        'CType(e.Item.FindControl("tdSpread"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTotal"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdMLine"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTeam"), HtmlTableCell).BgColor = "#8BB381"
                        'CType(e.Item.FindControl("tdTeamTotal"), HtmlTableCell).BgColor = "#8BB381"
                        'trtop.BgColor = "#8BB381"
                    Else
                        e.Item.FindControl("tdSpread").Visible = True
                        e.Item.FindControl("tdTotal").Visible = True
                        e.Item.FindControl("tdMLine").Visible = True
                        e.Item.FindControl("tdTeam").Visible = True
                        e.Item.FindControl("tdDate").Visible = True
                        e.Item.FindControl("tdNum").Visible = True
                        'e.Item.FindControl("trLoad").Visible = True

                    End If

                End If
            End If
            'If SafeString(e.Item.DataItem("Context")).Contains("Q") Then
            '    CType(e.Item.FindControl("trInfo"), HtmlTableRow).BgColor = "#000066"
            'ElseIf SafeString(e.Item.DataItem("Context")).Contains("1H") Then
            '    CType(e.Item.FindControl("trInfo"), HtmlTableRow).BgColor = "#658017"
            'ElseIf SafeString(e.Item.DataItem("Context")).Contains("2H") Then
            '    CType(e.Item.FindControl("trInfo"), HtmlTableRow).BgColor = "#123456"
            'Else
            '    CType(e.Item.FindControl("trInfo"), HtmlTableRow).BgColor = "#c35a21"
            'End If

            ' e.Item.FindControl("tdSpread").Visible = False 'Me.ShowSpread
            e.Item.FindControl("tdSpread2").Visible = Me.ShowSpread
            e.Item.FindControl("tdSpread3").Visible = Me.ShowSpread

            'e.Item.FindControl("tdTotal").Visible = False 'Me.ShowTotalPoints
            e.Item.FindControl("tdTotal2").Visible = Me.ShowTotalPoints
            e.Item.FindControl("tdTotal3").Visible = Me.ShowTotalPoints

            'e.Item.FindControl("tdMLine").Visible = False 'Me.ShowMoneyLine
            e.Item.FindControl("tdMLine2").Visible = Me.ShowMoneyLine
            e.Item.FindControl("tdMLine3").Visible = Me.ShowMoneyLine

            e.Item.FindControl("lblAwayPitcher").Visible = SafeString(e.Item.DataItem("AwayPitcher")) <> ""
            CType(e.Item.FindControl("lblAwayPitcher"), Literal).Text = "<br/>" & e.Item.DataItem("AwayPitcher") & " - "
            e.Item.FindControl("lblAwayPitcherRightHand").Visible = SafeString(e.Item.DataItem("AwayPitcher")) <> ""
            CType(e.Item.FindControl("lblAwayPitcherRightHand"), Literal).Text = SafeString(IIf(SBCBL.std.SafeString(e.Item.DataItem("AwayPitcherRightHand")) = "Y", "R", "L"))

            e.Item.FindControl("lblHomePitcher").Visible = SafeString(e.Item.DataItem("HomePitcher")) <> ""
            CType(e.Item.FindControl("lblHomePitcher"), Literal).Text = "<br/>" & e.Item.DataItem("HomePitcher") & " - "
            e.Item.FindControl("lblHomePitcherRightHand").Visible = SafeString(e.Item.DataItem("HomePitcher")) <> ""
            CType(e.Item.FindControl("lblHomePitcherRightHand"), Literal).Text = SafeString(IIf(SBCBL.std.SafeString(e.Item.DataItem("HomePitcherRightHand")) = "Y", "R", "L"))


            Dim oData As DataRowView = CType(e.Item.DataItem, DataRowView)

            If SafeDouble(oData("DrawMoneyLine")) <> 0 Then
                Dim trDraw As HtmlControl = e.Item.FindControl("trDraw")
                trDraw.Visible = True
            End If

            If SafeString(oData("IsCircle")) = "Y" Then
                Dim trCircle As HtmlControl = e.Item.FindControl("trCircle")
                trCircle.Visible = True
            End If

            Dim lblGameTypeHeader As Label = CType(e.Item.FindControl("lblGameTypeHeader"), Label)
            Dim lblAwaySpread As Label = CType(e.Item.FindControl("lblAwaySpread"), Label)
            Dim lblAwayTotal As Label = CType(e.Item.FindControl("lblAwayTotal"), Label)
            Dim lblHomeSpread As Label = CType(e.Item.FindControl("lblHomeSpread"), Label)
            Dim lblHomeTotal As Label = CType(e.Item.FindControl("lblHomeTotal"), Label)
            Dim lblAwayMoneyLine As Label = CType(e.Item.FindControl("lblAwayMoneyLine"), Label)
            Dim lblAwayMoneyLine2 As Label = CType(e.Item.FindControl("lblAwayMoneyLine2"), Label)
            Dim lblHomeMoney As Label = CType(e.Item.FindControl("lblHomeMoney"), Label)
            Dim lblHomeMoney2 As Label = CType(e.Item.FindControl("lblHomeMoney2"), Label)
            Dim lblDrawMoney As Label = CType(e.Item.FindControl("lblDrawMoney"), Label)
            Dim lblAwayTeamTotalPointsOverMoney As Literal = CType(e.Item.FindControl("lblAwayTeamTotalPointsOverMoney"), Literal)
            Dim lblAwayTeamTotalPointsUnderMoney As Literal = CType(e.Item.FindControl("lblAwayTeamTotalPointsUnderMoney"), Literal)
            Dim lblHomeTeamTotalPointsOverMoney As Literal = CType(e.Item.FindControl("lblHomeTeamTotalPointsOverMoney"), Literal)
            Dim lblHomeTeamTotalPointsUnderMoney As Literal = CType(e.Item.FindControl("lblHomeTeamTotalPointsUnderMoney"), Literal)
            Dim hfInfo As HiddenField = CType(e.Item.FindControl("hfInfo"), HiddenField)

            Dim ddlBuyPointHomeTotal As CDropDownList = CType(e.Item.FindControl("ddlBuyPointHomeTotal"), CDropDownList)
            Dim ddlBuyPointAwayTotal As CDropDownList = CType(e.Item.FindControl("ddlBuyPointAwayTotal"), CDropDownList)
            Dim ddlBuyPointHomeSpread As CDropDownList = CType(e.Item.FindControl("ddlBuyPointHomeSpread"), CDropDownList)
            Dim ddlBuyPointAwaySpread As CDropDownList = CType(e.Item.FindControl("ddlBuyPointAwaySpread"), CDropDownList)


            Dim chkSelectAwaySpread As CheckBox = CType(e.Item.FindControl("chkSelectAwaySpread"), CheckBox)
            Dim chkSelectHomeSpread As CheckBox = CType(e.Item.FindControl("chkSelectHomeSpread"), CheckBox)
            Dim chkSelectHomeTotal As CheckBox = CType(e.Item.FindControl("chkSelectHomeTotal"), CheckBox)
            Dim chkSelectAwayTotal As CheckBox = CType(e.Item.FindControl("chkSelectAwayTotal"), CheckBox)
            Dim chkSelectAwayMLine As CheckBox = CType(e.Item.FindControl("chkSelectAwayMLine"), CheckBox)
            Dim chkSelectAwayMLine2 As CheckBox = CType(e.Item.FindControl("chkSelectAwayMLine2"), CheckBox)
            Dim chkSelectHomeMLine As CheckBox = CType(e.Item.FindControl("chkSelectHomeMLine"), CheckBox)
            Dim chkSelectHomeMLine2 As CheckBox = CType(e.Item.FindControl("chkSelectHomeMLine2"), CheckBox)
            Dim chkSelectDraw As CheckBox = CType(e.Item.FindControl("chkSelectDraw"), CheckBox)

            Dim rdSelectDraw As CheckBox = CType(e.Item.FindControl("rdSelectDraw"), CheckBox)
            Dim rdSelectAwayMLine As RadioButton = CType(e.Item.FindControl("rdSelectAwayMLine"), RadioButton)
            Dim rdSelectAwayMLine2 As RadioButton = CType(e.Item.FindControl("rdSelectAwayMLine2"), RadioButton)
            Dim rdSelectHomeMLine2 As RadioButton = CType(e.Item.FindControl("rdSelectHomeMLine2"), RadioButton)
            Dim rdSelectHomeMLine As RadioButton = CType(e.Item.FindControl("rdSelectHomeMLine"), RadioButton)
            Dim rdSelectAwayTotal As CheckBox = CType(e.Item.FindControl("rdSelectAwayTotal"), CheckBox)
            Dim rdSelectHomeTotal As CheckBox = CType(e.Item.FindControl("rdSelectHomeTotal"), CheckBox)
            Dim rdSelectHomeSpread As RadioButton = CType(e.Item.FindControl("rdSelectHomeSpread"), RadioButton)
            Dim rdSelectAwaySpread As RadioButton = CType(e.Item.FindControl("rdSelectAwaySpread"), RadioButton)

            Dim rdAwayTeamTotalOver As RadioButton = CType(e.Item.FindControl("rdAwayTeamTotalOver"), RadioButton)
            Dim rdAwayTeamTotalUnder As RadioButton = CType(e.Item.FindControl("rdAwayTeamTotalUnder"), RadioButton)
            Dim rdHomeTeamTotalOver As RadioButton = CType(e.Item.FindControl("rdHomeTeamTotalOver"), RadioButton)
            Dim rdHomeTeamTotalUnder As RadioButton = CType(e.Item.FindControl("rdHomeTeamTotalUnder"), RadioButton)
            '''''''''''''''begin set juice'''''''''''''
            Dim bAwaySpreadJuice As Boolean
            Dim bHomeSpreadJuice As Boolean
            Dim bAwayMoneyLineJuice As Boolean
            Dim bHomeMoneyLineJuice As Boolean
            Dim bAwayTotalJuice As Boolean = True
            Dim bHomeTotalJuice As Boolean = False
            Dim nAwaySpread As Double = SafeDouble(oData("AwaySpread"))
            Dim nHomeSpread As Double = SafeDouble(oData("HomeSpread"))
            Dim nTotalPoint As Double = SafeDouble(oData("TotalPoints"))

            'hfAwaySpreadJuice.Value = IIf(nAwaySpread < nHomeSpread, "True", "False")
            'hfHomeSpreadJuice.Value = IIf(nAwaySpread < nHomeSpread, "False", "True")
            'If nAwaySpread = nHomeSpread Then
            '    hfAwaySpreadJuice.Value = "True"
            '    hfHomeSpreadJuice.Value = "False"
            'End If
            'hfAwayMoneyLineJuice.Value = hfAwaySpreadJuice.Value
            'hfHomeMoneyLineJuice.Value = hfHomeSpreadJuice.Value
            'hfAwayTotalJuice.Value = "True"
            'hfHomeTotalJuice.Value = "False"
            'bAwaySpreadJuice = IIf(nAwaySpread < nHomeSpread, True, False)
            'bHomeSpreadJuice = IIf(nAwaySpread < nHomeSpread, False, True)
            'If nAwaySpread = nHomeSpread Then
            '    bAwaySpreadJuice = True
            '    bHomeSpreadJuice = False
            'End If
            'bAwayMoneyLineJuice = bAwaySpreadJuice
            'bHomeMoneyLineJuice = bHomeSpreadJuice


            getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
            '''''''''''''''end set juice'''''''''''''''

            lblGameTypeHeader.Text = SafeString(oData("GameType"))
            Dim nTotalPointsTeam As Double = 0
            Dim nMoneyLine As Double
            Dim nTotalPoints As Double = 0
            Dim sGameID, sContext, sGameType As String
            sGameID = SafeString(oData("GameID"))
            sGameType = SafeString(oData("GameType"))
            'oData("Context") = "2H"
            sContext = SafeString(oData("Context"))
            hfInfo.Value = SafeString(oData("GameLineID"))

            nAwaySpread = addPointTeaser(nAwaySpread, sGameType, True)
            nHomeSpread = addPointTeaser(nHomeSpread, sGameType, True)
            Dim nTotalPointsOver = addPointTeaser(nTotalPoint, sGameType, False)
            Dim nTotalPointsUnder = addPointTeaser(nTotalPoint, sGameType, True)
            '' Away Spread 
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "Spread", _
                                                      SafeDouble(oData("AwaySpreadMoney")), SafeBoolean(bAwaySpreadJuice), IncreaseSpread)


            lblAwaySpread.Text = SafeString(IIf(nAwaySpread > 0, "&nbsp;+" & safeVegass(nAwaySpread), "&nbsp;" & safeVegass(nAwaySpread))) & SafeString(IIf(nMoneyLine > 0, " &nbsp; +" & nMoneyLine, " &nbsp;" & nMoneyLine))
            If nAwaySpread = 0 Then
                lblAwaySpread.Text = "PK" & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            End If
            If UCase(lblAwaySpread.Text) = "PK 0" OrElse UCase(lblAwaySpread.Text) = "PK&NBSP;0" Then
                lblAwaySpread.Text = ""
            End If

            chkSelectAwaySpread.Visible = nMoneyLine <> 0
            CType(e.Item.FindControl("txtMoneyAwaySpread"), TextBox).Visible = nMoneyLine <> 0
            ParseBuyPointOptions(nAwaySpread, nMoneyLine, "Spread", sContext, sGameType, ddlBuyPointAwaySpread, lblAwaySpread)
            Dim sClick As String = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "Spread", "AWAY", _
            SafeDouble(oData("AwaySpread")), GetMoneyGameIncreaseSpreadML(SafeString(oData("GameID")), oData("GameType"), "Away", oData("Context"), "Spread", SafeDouble(oData("AwaySpreadMoney")), SafeString(oData("GameLineID")), SafeDouble(oData("AwaySpread")), SafeDouble(oData("HomeSpread"))), _
            oData("IsCircle"), getFavout(SafeDouble(oData("AwaySpread")), SafeDouble(oData("HomeSpread")), "Away"), ValidDescription(SBCBL.std.SafeString(oData("Description"))), SafeString(ddlBuyPointAwaySpread.ClientID), "")
            chkSelectAwaySpread.Attributes("OnClick") = sClick
            rdSelectAwaySpread.Attributes("OnClick") = sClick


            ' HightGameLight(sGameID, sContext, lblAwaySpread)
            ''total-Team over
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "TeamTotalPoints", _
                                                      SafeDouble(oData("AwayTeamTotalPointsOverMoney")), True, "away")

            lblAwayTeamTotalPointsOverMoney.Text = String.Format("Over{0}", IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TeamTotalPoints", "AWAY", _
            SBCBL.std.SafeDouble(oData("AwayTeamTotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Away", oData("Context"), "TotalPoints", SBCBL.std.SafeDouble(oData("AwayTeamTotalPointsOverMoney")), SBCBL.std.SafeString(oData("GameLineID")), "True"), _
            oData("IsCircle"), "True", ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "")
            rdAwayTeamTotalOver.Attributes("OnClick") = sClick


            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "TeamTotalPoints", _
                                                      SafeDouble(oData("AwayTeamTotalPointsUnderMoney")), False, "away")
            lblAwayTeamTotalPointsUnderMoney.Text = String.Format("Under{0}", IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
                       """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"");", _
                       oData("GameID"), oData("GameLineID"), oData("GameType"), _
                       oData("BookMaker"), oData("Context"), oData("GameDate"), _
                       oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
                       oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
                       oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TeamTotalPoints", "AWAY", _
                       SBCBL.std.SafeDouble(oData("AwayTeamTotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Away", oData("Context"), "TotalPoints", SBCBL.std.SafeDouble(oData("AwayTeamTotalPointsUnderMoney")), SBCBL.std.SafeString(oData("GameLineID")), "True"), _
                       oData("IsCircle"), "False", ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "")
            rdAwayTeamTotalUnder.Attributes("OnClick") = sClick
            ''total-Team under
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "TeamTotalPoints", _
                                                      SafeDouble(oData("HomeTeamTotalPointsOverMoney")), True, "home")
            lblHomeTeamTotalPointsOverMoney.Text = String.Format("Over{0}", IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))

            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TeamTotalPoints", "HOME", _
            SBCBL.std.SafeDouble(oData("HomeTeamTotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "HOME", oData("Context"), "TeamTotalPoints", SBCBL.std.SafeDouble(oData("HomeTeamTotalPointsUnderMoney")), SBCBL.std.SafeString(oData("GameLineID")), "False"), _
            oData("IsCircle"), "True", ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "")
            rdHomeTeamTotalOver.Attributes("OnClick") = sClick
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "TeamTotalPoints", _
                                                     SafeDouble(oData("HomeTeamTotalPointsUnderMoney")), False, "home")
            lblHomeTeamTotalPointsUnderMoney.Text = String.Format("Under{0}", IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TeamTotalPoints", "HOME", _
            SBCBL.std.SafeDouble(oData("HomeTeamTotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "HOME", oData("Context"), "TeamTotalPoints", SBCBL.std.SafeDouble(oData("HomeTeamTotalPointsOverMoney")), SBCBL.std.SafeString(oData("GameLineID")), "False"), _
            oData("IsCircle"), "False", ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "")
            rdHomeTeamTotalUnder.Attributes("OnClick") = sClick
            '' check if total point is locked
            If Not GetTPointLock(oData) Then
                nTotalPoints = nTotalPointsOver
            Else
                nTotalPoints = 0
            End If
            '' Away TotalPoints
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "TotalPoints", _
                                                       SafeDouble(oData("TotalPointsOverMoney")), True)
            lblAwayTotal.Text = "Over&nbsp;" & safeVegass(nTotalPoints) & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            CType(e.Item.FindControl("txtMoneyAwayTotal"), TextBox).Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            CType(e.Item.FindControl("txtMoneyHomeTotal"), TextBox).Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            ''chk visible
            CType(e.Item.FindControl("chkSelectAwayTotal"), CheckBox).Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            CType(e.Item.FindControl("chkSelectHomeTotal"), CheckBox).Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            lblAwayTotal.Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            lblHomeTotal.Visible = nTotalPoints <> 0 AndAlso nMoneyLine <> 0
            If lblAwayTotal.Visible Then
                ParseBuyPointOptions(nTotalPoints, nMoneyLine, "TotalPoints", sContext, sGameType, ddlBuyPointAwayTotal, lblAwayTotal, True)
            End If
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TotalPoints", "AWAY", _
            SBCBL.std.SafeDouble(oData("TotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "away", oData("Context"), "TotalPoints", SBCBL.std.SafeDouble(oData("TotalPointsOverMoney")), SBCBL.std.SafeString(oData("GameLineID")), "False"), _
            oData("IsCircle"), "False", ValidDescription(SBCBL.std.SafeString(oData("Description"))), SafeString(ddlBuyPointAwayTotal.ClientID), "")
            chkSelectAwayTotal.Attributes("Onclick") = sClick
            rdSelectAwayTotal.Attributes("Onclick") = sClick
            '' Away MoneyLine
            If GetMLLock(oData) Then
                nMoneyLine = 0
            Else
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "MoneyLine", _
                                                       SafeDouble(oData("AwayMoneyLine")), bAwayMoneyLineJuice)
            End If
            lblAwayMoneyLine.Text = SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            CType(e.Item.FindControl("txtMoneyAwayMLine"), TextBox).Visible = nMoneyLine <> 0
            CType(e.Item.FindControl("txtMoneyHomeMLine"), TextBox).Visible = nMoneyLine <> 0
            ''chk visible
            CType(e.Item.FindControl("chkSelectAwayMLine"), CheckBox).Visible = nMoneyLine <> 0
            CType(e.Item.FindControl("chkSelectHomeMLine"), CheckBox).Visible = nMoneyLine <> 0
            lblHomeMoney.Visible = nMoneyLine <> 0
            lblAwayMoneyLine.Visible = nMoneyLine <> 0
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "MoneyLine", "AWAY", _
            0, GetMoneyGameIncreaseSpreadML(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Away", oData("Context"), "MoneyLine", SBCBL.std.SafeDouble(oData("AwayMoneyLine")), SBCBL.std.SafeString(oData("GameLineID")), SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread"))), _
            oData("IsCircle"), getFavout(SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread")), "Away"), ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "[IsCheckPitcher]")
            rdSelectAwayMLine.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "False")
            rdSelectAwayMLine2.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "True")
            chkSelectAwayMLine.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "False")
            chkSelectAwayMLine2.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "True")


            '' Home Spread
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "Spread", _
                                                      SafeDouble(oData("HomeSpreadMoney")), bHomeSpreadJuice, IncreaseSpread)

            lblHomeSpread.Text = SafeString(IIf(nHomeSpread > 0, "&nbsp;+" & safeVegass(nHomeSpread), "&nbsp;" & safeVegass(nHomeSpread))) & "&nbsp;" & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            If nHomeSpread = 0 Then
                lblHomeSpread.Text = "PK" & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            End If
            If UCase(lblHomeSpread.Text) = "PK 0" OrElse UCase(lblHomeSpread.Text) = "PK&NBSP;0" Then
                lblHomeSpread.Text = ""
            End If

            CType(e.Item.FindControl("txtMoneyHomeSpread"), TextBox).Visible = nMoneyLine <> 0
            chkSelectHomeSpread.Visible = nMoneyLine <> 0
            ParseBuyPointOptions(nHomeSpread, nMoneyLine, "Spread", sContext, sGameType, ddlBuyPointHomeSpread, lblHomeSpread)
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "Spread", "HOME", _
            SBCBL.std.SafeDouble(oData("HomeSpread")), GetMoneyGameIncreaseSpreadML(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Home", oData("Context"), "Spread", SBCBL.std.SafeDouble(oData("HomeSpreadMoney")), SBCBL.std.SafeString(oData("GameLineID")), SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread"))), _
            oData("IsCircle"), getFavout(SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread")), "Home"), ValidDescription(SBCBL.std.SafeString(oData("Description"))), ddlBuyPointHomeSpread.ClientID, "")
            chkSelectHomeSpread.Attributes("Onclick") = sClick
            rdSelectHomeSpread.Attributes("Onclick") = sClick

            '' Home TotalPoints
            nTotalPoints = nTotalPointsUnder
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "TotalPoints", _
                                                       SafeDouble(oData("TotalPointsUnderMoney")), False)
            lblHomeTotal.Text = "Under&nbsp;" & safeVegass(nTotalPoints) & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            lblHomeTotal.Enabled = SafeDouble(nTotalPoints) <> 0 AndAlso nMoneyLine <> 0
            If lblHomeTotal.Visible Then
                ParseBuyPointOptions(nTotalPoints, nMoneyLine, "TotalPoints", sContext, sGameType, ddlBuyPointHomeTotal, lblHomeTotal)
            End If
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
             """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
             oData("GameID"), oData("GameLineID"), oData("GameType"), _
             oData("BookMaker"), oData("Context"), oData("GameDate"), _
             oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
             oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
             oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "TotalPoints", "HOME", _
             SBCBL.std.SafeDouble(oData("TotalPoints")), GetMoneyGameIncreaseTotal(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Home", oData("Context"), "TotalPoints", SBCBL.std.SafeDouble(oData("TotalPointsUnderMoney")), SBCBL.std.SafeString(oData("GameLineID")), "False"), _
             oData("IsCircle"), "False", ValidDescription(SBCBL.std.SafeString(oData("Description"))), ddlBuyPointHomeTotal.ClientID, "")
            chkSelectHomeTotal.Attributes("Onclick") = sClick
            rdSelectHomeTotal.Attributes("Onclick") = sClick
            If GetMLLock(oData) Then
                nMoneyLine = 0
            Else
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "MoneyLine", _
                                                           SafeDouble(oData("HomeMoneyLine")), bHomeMoneyLineJuice)
            End If
            lblHomeMoney.Text = SafeString(IIf(nMoneyLine > 0, "+&nbsp;" & nMoneyLine, "&nbsp;" & nMoneyLine))

            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "MoneyLine", "HOME", _
            0, GetMoneyGameIncreaseSpreadML(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Home", oData("Context"), "MoneyLine", SBCBL.std.SafeDouble(oData("HomeMoneyLine")), SBCBL.std.SafeString(oData("GameLineID")), SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread"))), _
            oData("IsCircle"), getFavout(SBCBL.std.SafeDouble(oData("AwaySpread")), SBCBL.std.SafeDouble(oData("HomeSpread")), "Home"), ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "[IsCheckPitcher]")
            rdSelectHomeMLine.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "false")
            rdSelectHomeMLine2.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "true")
            chkSelectHomeMLine.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "false")
            chkSelectHomeMLine2.Attributes("Onclick") = sClick.Replace("[IsCheckPitcher]", "true")


            ' Draw MoneyLine
            nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "Draw", _
                                                       SafeDouble(oData("DrawMoneyLine")), False)
            lblDrawMoney.Text = SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            CType(e.Item.FindControl("txtMoneyDraw"), TextBox).Visible = nMoneyLine <> 0
            chkSelectDraw.Visible = nMoneyLine <> 0
            sClick = String.Format("javascript: return Betting(this,""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}""," & _
            """{8}"",""{9}"",""{10}"",""{11}"",""{12}"",""{13}"",""{14}"",""{15}"",{16},{17},""{18}"",""{19}"",""{20}"",""{21}"",""{22}"");", _
            oData("GameID"), oData("GameLineID"), oData("GameType"), _
            oData("BookMaker"), oData("Context"), oData("GameDate"), _
            oData("AwayTeam"), oData("HomeTeam"), oData("AwayRotationNumber"), _
            oData("HomeRotationNumber"), oData("AwayPitcher"), oData("HomePitcher"), _
            oData("AwayPitcherRightHand"), oData("HomePitcherRightHand"), "Draw", "", _
            0, GetMoneyGameIncrease(SBCBL.std.SafeString(oData("GameID")), oData("GameType"), "Away", oData("Context"), "Draw", SBCBL.std.SafeDouble(oData("DrawMoneyLine")), ""), _
            oData("IsCircle"), "", ValidDescription(SBCBL.std.SafeString(oData("Description"))), "", "")
            chkSelectDraw.Attributes("Onclick") = sClick
            rdSelectDraw.Attributes("Onclick") = sClick
            If IsSoccer(SafeString(oData("GameType"))) Then
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "Spread", _
                                                             SafeDouble(oData("AwaySpreadMoney")), bAwaySpreadJuice)

                lblAwaySpread.Text = safeVegass(AHFormat(nAwaySpread)) & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
                If nAwaySpread = 0 Then
                    lblAwaySpread.Text = "PK&nbsp;" & SafeString(IIf(nMoneyLine > 0, "+" & nMoneyLine, nMoneyLine))
                End If
                If UCase(lblAwaySpread.Text) = "PK 0" OrElse UCase(lblAwaySpread.Text) = "PK&NBSP;0" Then
                    lblAwaySpread.Text = ""
                End If

                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "TotalPoints", _
                                                           SafeDouble(oData("TotalPointsOverMoney")), True)
                lblAwayTotal.Text = "Over&nbsp;" & safeVegass(AHFormat(nTotalPointsOver)).Replace("+"c, "") & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;&nbsp;" & nMoneyLine))
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "Spread", _
                                                        SafeDouble(oData("HomeSpreadMoney")), bHomeSpreadJuice)
                lblHomeSpread.Text = safeVegass(AHFormat(nHomeSpread)) & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))


                If nHomeSpread = 0 Then
                    lblHomeSpread.Text = "PK&nbsp;" & SafeString(nMoneyLine)
                End If
                If UCase(lblHomeSpread.Text) = "PK 0" OrElse UCase(lblHomeSpread.Text) = "PK&NBSP;0" Then
                    lblHomeSpread.Text = ""
                End If

                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Home", sContext, "TotalPoints", _
                                                           SafeDouble(oData("TotalPointsUnderMoney")), False)
                lblHomeTotal.Text = "Under&nbsp;" & safeVegass(AHFormat(nTotalPointsUnder)).Replace("+"c, "") & " " & SafeString(IIf(nMoneyLine > 0, "&nbsp;+" & nMoneyLine, "&nbsp;" & nMoneyLine))
            End If

            'Dim chkSelectHomeTeamTotalOver As CheckBox = CType(e.Item.FindControl("chkSelectHomeTeamTotalOver"), CheckBox)
            'Dim chkSelectHomeTeamTotalUnder As CheckBox = CType(e.Item.FindControl("chkSelectHomeTeamTotalUnder"), CheckBox)

            'chkSelectHomeTeamTotalOver.Attributes("onclick") = "teamTotal_onclick(this,'" & chkSelectHomeTeamTotalUnder.ClientID & "')"
            'chkSelectHomeTeamTotalUnder.Attributes("onclick") = "teamTotal_onclick(this,'" & chkSelectHomeTeamTotalOver.ClientID & "')"


            'Dim chkSelectAwayTeamTotalOver As CheckBox = CType(e.Item.FindControl("chkSelectAwayTeamTotalOver"), CheckBox)
            'Dim chkSelectAwayTeamTotalUnder As CheckBox = CType(e.Item.FindControl("chkSelectAwayTeamTotalUnder"), CheckBox)

            'chkSelectAwayTeamTotalOver.Attributes("onclick") = "teamTotal_onclick(this,'" & chkSelectAwayTeamTotalUnder.ClientID & "')"
            'chkSelectAwayTeamTotalUnder.Attributes("onclick") = "teamTotal_onclick(this,'" & chkSelectAwayTeamTotalOver.ClientID & "')"

            e.Item.FindControl("rdSelectAwaySpread").Visible = e.Item.FindControl("txtMoneyAwaySpread").Visible AndAlso ShowRadio()
            rdSelectAwayMLine.Visible = e.Item.FindControl("txtMoneyAwayMLine").Visible AndAlso ShowRadio()

            e.Item.FindControl("rdSelectAwayTotal").Visible = e.Item.FindControl("txtMoneyAwayTotal").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdSelectHomeSpread").Visible = e.Item.FindControl("txtMoneyHomeSpread").Visible AndAlso ShowRadio()
            rdSelectHomeMLine.Visible = e.Item.FindControl("txtMoneyHomeMLine").Visible AndAlso ShowRadio()

            e.Item.FindControl("rdSelectHomeTotal").Visible = e.Item.FindControl("txtMoneyHomeTotal").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdHomeTeamTotalUnder").Visible = e.Item.FindControl("txtMoneyHomeTeamTotalUnder").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdHomeTeamTotalOver").Visible = e.Item.FindControl("txtMoneyHomeTeamTotalOver").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdAwayTeamTotalUnder").Visible = e.Item.FindControl("txtMoneyAwayTeamTotalUnder").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdAwayTeamTotalOver").Visible = e.Item.FindControl("txtMoneyAwayTeamTotalOver").Visible AndAlso ShowRadio()
            e.Item.FindControl("rdSelectDraw").Visible = e.Item.FindControl("txtMoneyDraw").Visible AndAlso ShowRadio()

            e.Item.FindControl("txtMoneyAwaySpread").Visible = e.Item.FindControl("txtMoneyAwaySpread").Visible AndAlso ShowTextBox()
            e.Item.FindControl("txtMoneyAwayMLine").Visible = e.Item.FindControl("txtMoneyAwayMLine").Visible AndAlso ShowTextBox()
            e.Item.FindControl("txtMoneyAwayMLine2").Visible = e.Item.FindControl("txtMoneyAwayMLine").Visible
            e.Item.FindControl("txtMoneyAwayTotal").Visible = e.Item.FindControl("txtMoneyAwayTotal").Visible AndAlso ShowTextBox()
            e.Item.FindControl("txtMoneyHomeSpread").Visible = e.Item.FindControl("txtMoneyHomeSpread").Visible AndAlso ShowTextBox()
            e.Item.FindControl("txtMoneyHomeMLine").Visible = e.Item.FindControl("txtMoneyHomeMLine").Visible AndAlso ShowTextBox()
            e.Item.FindControl("txtMoneyHomeMLine2").Visible = e.Item.FindControl("txtMoneyHomeMLine").Visible
            e.Item.FindControl("txtMoneyHomeTotal").Visible = e.Item.FindControl("txtMoneyHomeTotal").Visible AndAlso ShowTextBox()
            If SafeBoolean(oData("IsGameTeamTPointLocked")) Then
                e.Item.FindControl("tdTeam_Total_HomeOver").Visible = False
                e.Item.FindControl("tdTeam_Total_HomeUnder").Visible = False

                e.Item.FindControl("tdTeam_Total_AwayOver").Visible = False
                e.Item.FindControl("tdTeam_Total_AwayUnder").Visible = False

                e.Item.FindControl("lblAwayTeamTotalPointsUnderMoney").Visible = False
                e.Item.FindControl("lblHomeTeamTotalPointsUnderMoney").Visible = False
                e.Item.FindControl("txtMoneyHomeTeamTotalUnder").Visible = False
                e.Item.FindControl("lblHomeTeamTotalPointsOverMoney").Visible = False
                e.Item.FindControl("lblAwayTeamTotalPointsOverMoney").Visible = False
                e.Item.FindControl("txtMoneyHomeTeamTotalOver").Visible = False
                e.Item.FindControl("txtMoneyAwayTeamTotalUnder").Visible = False
                e.Item.FindControl("txtMoneyAwayTeamTotalOver").Visible = False
            Else

                e.Item.FindControl("txtMoneyHomeTeamTotalUnder").Visible = e.Item.FindControl("txtMoneyHomeTeamTotalUnder").Visible AndAlso ShowTextBox()
                e.Item.FindControl("txtMoneyHomeTeamTotalOver").Visible = e.Item.FindControl("txtMoneyHomeTeamTotalOver").Visible AndAlso ShowTextBox()
                e.Item.FindControl("txtMoneyAwayTeamTotalUnder").Visible = e.Item.FindControl("txtMoneyAwayTeamTotalUnder").Visible AndAlso ShowTextBox()
                e.Item.FindControl("txtMoneyAwayTeamTotalOver").Visible = e.Item.FindControl("txtMoneyAwayTeamTotalOver").Visible AndAlso ShowTextBox()

                e.Item.FindControl("chkAwayTeamTotalOver").Visible = e.Item.FindControl("chkAwayTeamTotalOver").Visible AndAlso ShowCheckBox()
                e.Item.FindControl("chkAwayTeamTotalUnder").Visible = e.Item.FindControl("chkAwayTeamTotalUnder").Visible AndAlso ShowCheckBox()

                e.Item.FindControl("chkHomeTeamTotalOver").Visible = e.Item.FindControl("chkHomeTeamTotalOver").Visible AndAlso ShowCheckBox()
                e.Item.FindControl("chkHomeTeamTotalUnder").Visible = e.Item.FindControl("chkHomeTeamTotalUnder").Visible AndAlso ShowCheckBox()


            End If

            e.Item.FindControl("txtMoneyDraw").Visible = e.Item.FindControl("txtMoneyDraw").Visible AndAlso ShowTextBox()
            ' e.Item.FindControl("txtMoneyAwayTeamTotal").Visible = ShowTextBox()
            ' e.Item.FindControl("txtMoneyHomeTeamTotal").Visible = ShowTextBox()

            e.Item.FindControl("chkSelectAwaySpread").Visible = e.Item.FindControl("chkSelectAwaySpread").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectAwayMLine").Visible = e.Item.FindControl("chkSelectAwayMLine").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectAwayMLine2").Visible = e.Item.FindControl("chkSelectAwayMLine").Visible
            e.Item.FindControl("chkSelectAwayTotal").Visible = e.Item.FindControl("chkSelectAwayTotal").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectHomeSpread").Visible = e.Item.FindControl("chkSelectHomeSpread").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectHomeMLine").Visible = e.Item.FindControl("chkSelectHomeMLine").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectHomeMLine2").Visible = e.Item.FindControl("chkSelectHomeMLine").Visible
            e.Item.FindControl("chkSelectHomeTotal").Visible = e.Item.FindControl("chkSelectHomeTotal").Visible AndAlso ShowCheckBox()
            e.Item.FindControl("chkSelectDraw").Visible = e.Item.FindControl("chkSelectDraw").Visible AndAlso ShowCheckBox()
            If IsBaseball(sGameType) Then
                lblAwayMoneyLine2.Text = lblAwayMoneyLine.Text
                lblHomeMoney2.Text = lblHomeMoney.Text
                rdSelectHomeMLine2.Visible = rdSelectHomeMLine.Visible
                rdSelectAwayMLine2.Visible = rdSelectAwayMLine.Visible
                chkSelectHomeMLine2.Visible = chkSelectHomeMLine.Visible
                chkSelectAwayMLine2.Visible = chkSelectAwayMLine.Visible
            Else
                lblAwayMoneyLine2.Visible = False
                lblHomeMoney2.Visible = False
                rdSelectHomeMLine2.Visible = False
                rdSelectAwayMLine2.Visible = False
                chkSelectHomeMLine2.Visible = False
                chkSelectAwayMLine2.Visible = False
                e.Item.FindControl("txtMoneyAwayMLine2").Visible = False
                e.Item.FindControl("txtMoneyHomeMLine2").Visible = False
            End If
            ' e.Item.FindControl("chkSelectAwayTeamTotal").Visible = ShowTextBox()
            'e.Item.FindControl("chkSelectHomeTeamTotal").Visible = ShowTextBox()
            If chkTeamTotal.Checked Then

                ''''show team total start''''''''''''
                CType(e.Item.FindControl("tdMLine"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdMLine2"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdMLine3"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTotal"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTotal2"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdTotal3"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdSpread"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdSpread2"), HtmlTableCell).Visible = False
                CType(e.Item.FindControl("tdSpread3"), HtmlTableCell).Visible = False
                ''''show teamtotal end'''''''''''''''
            End If

        End If

            LogInfo(_log, "rptGameLines_ItemDataBound")
    End Sub

    Protected Sub BetGame(ByVal bSetWinAmount As Boolean, ByVal sender As Object, ByVal pbFavorite As Boolean, ByVal poData As DataRow, ByVal ddlBuyPoint As CDropDownList, ByVal pbIsCheckPitcher As Boolean, Optional ByVal pbPropGame As Boolean = False)
        Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
        _oOddsRuleEngine = New COddRulesEngine(olstGameType, SelectedPlayer.SuperAdminID, True, SelectedPlayer.Template, SelectedPlayer.SuperAgentID)

        StraightError = False
        '' Item Checked
        Dim sHABet As String = ""
        Dim sHABetA As String = ""
        Dim sTeamTotalName As String = ""
        'Dim oHTKBet As New HiddenField
        Dim nMoneyLine As Double
        Dim chkItem As CheckBox
        Dim txtItem As TextBox = Nothing
        Dim item As RepeaterItem
        '' check type of bet is not straight or other  If TypeOf (sender) Is CheckBox is not straights
        If TypeOf (sender) Is CheckBox Then
            chkItem = CType(sender, CheckBox)
            If TypeOf (chkItem.Parent.Parent.Parent) Is RepeaterItem Then
                ''for drawn
                item = CType(chkItem.Parent.Parent.Parent, RepeaterItem)
            Else
                item = CType(chkItem.Parent.Parent, RepeaterItem)
            End If
        Else
            txtItem = CType(sender, TextBox)
            'ClientAlert(txtItem.Text, True)
            If TypeOf (txtItem.Parent.Parent.Parent) Is RepeaterItem Then
                ''for drawn
                item = CType(txtItem.Parent.Parent.Parent, RepeaterItem)
            ElseIf TypeOf (txtItem.Parent.Parent) Is RepeaterItem Then
                item = CType(txtItem.Parent.Parent, RepeaterItem)
            Else
                item = CType(txtItem.Parent.Parent.Parent.Parent.Parent, RepeaterItem)
            End If
            If pbPropGame Then
                GoTo PropGame
            End If
            chkItem = item.FindControl(txtItem.ID.Replace("txtMoney", "chkSelect"))
            '' if textbox have data add to ticket else remove ticket

            If chkItem Is Nothing Then ''for team total
                '''''''''' for team total start''''''''''''''
                If txtItem.ID.Contains("Away") Then

                    '  oHTKBet = CType(item.FindControl("hftkSelectAwayTeamTotal"), HiddenField)
                    ' Dim chkSelectAwayTeamTotalOver As CheckBox = CType(item.FindControl("chkSelectAwayTeamTotalOver"), CheckBox)
                    'Dim chkSelectAwayTeamTotalUnder As CheckBox = CType(item.FindControl("chkSelectAwayTeamTotalUnder"), CheckBox)
                    ''check lick check box in away
                    If txtItem.ID.Contains("txtMoneyAwayTeamTotalOver") Then
                        nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "TeamTotalPoints", _
                                                      SafeDouble(poData("AwayTeamTotalPointsOverMoney")), True, "away")
                        sHABetA = SafeDouble(poData("AwayTeamTotalPoints")) & "|" & nMoneyLine 'CType(item.FindControl("hfaSelectAwayTeamTotalOver"), HiddenField).Value
                        sHABet = "OVER_POINT_TEAM_TOTAL"
                        pbFavorite = True
                    ElseIf txtItem.ID.Contains("txtMoneyAwayTeamTotalUnder") Then

                        nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "TeamTotalPoints", _
                                                     SafeDouble(poData("AwayTeamTotalPointsUnderMoney")), False, "away")

                        sHABetA = SafeDouble(poData("AwayTeamTotalPoints")) & "|" & nMoneyLine ' CType(item.FindControl("hfaSelectAwayTeamTotalUnder"), HiddenField).Value
                        sHABet = "UNDER_POINT_TEAM_TOTAL"
                        pbFavorite = False
                    Else
                        Return
                    End If
                    sTeamTotalName = "away"
                Else

                    '  oHTKBet = CType(item.FindControl("hftkSelectHomeTeamTotal"), HiddenField)
                    'Dim chkSelectHomeTeamTotalOver As CheckBox = CType(item.FindControl("chkSelectHomeTeamTotalOver"), CheckBox)
                    'Dim chkSelectHomeTeamTotalUnder As CheckBox = CType(item.FindControl("chkSelectHomeTeamTotalUnder"), CheckBox)
                    ''check lick check box in home
                    If txtItem.ID.Contains("txtMoneyHomeTeamTotalOver") Then

                        nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "TeamTotalPoints", _
                                                      SafeDouble(poData("HomeTeamTotalPointsOverMoney")), True, "home")
                        sHABetA = SafeDouble(poData("HomeTeamTotalPoints")) & "|" & nMoneyLine 'CType(item.FindControl("hfaSelectHomeTeamTotalOver"), HiddenField).Value
                        sHABet = "OVER_POINT_TEAM_TOTAL"
                        pbFavorite = True
                    ElseIf txtItem.ID.Contains("txtMoneyHomeTeamTotalUnder") Then
                        nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "TeamTotalPoints", _
                                                     SafeDouble(poData("HomeTeamTotalPointsUnderMoney")), False, "home")
                        sHABetA = SafeDouble(poData("HomeTeamTotalPoints")) & "|" & nMoneyLine 'CType(item.FindControl("hfaSelectHomeTeamTotalUnder"), HiddenField).Value
                        sHABet = "UNDER_POINT_TEAM_TOTAL"
                        pbFavorite = False
                    Else
                        Return
                    End If
                    sTeamTotalName = "home"
                End If
                ''''''''''''' for team total end''''''''''''''
            End If
        End If
PropGame:
        Dim oTicketBet As CTicketBet = Nothing
        Dim sChoice As String = ""
        Dim arrBuyPoin As Array
        Select Case True 'UCase(SafeString(sHABet))
            Case txtItem.ID.Contains("AwaySpread") '"AWAY_SPREAD", "HOME_SPREAD"thuong
                sChoice = "AWAY"
                If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible AndAlso ddlBuyPoint.SelectedIndex > 0 Then
                    arrBuyPoin = ddlBuyPoint.SelectedValue.Split("|")
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "Spread", _
                                             SafeDouble(poData("AwaySpreadMoney")) + SafeDouble(arrBuyPoin(1)), pbFavorite, IncreaseSpread)

                    oTicketBet = getSpeadBet(SafeDouble(SafeString(arrBuyPoin(0))) + SafeDouble(SafeString(poData("AwaySpread"))), nMoneyLine, sChoice)
                Else
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "Spread", _
                                             SafeDouble(poData("AwaySpreadMoney")), pbFavorite, IncreaseSpread)

                    oTicketBet = getSpeadBet(SafeDouble(SafeString(poData("AwaySpread"))), nMoneyLine, sChoice)
                End If

            Case txtItem.ID.Contains("HomeSpread")
                sChoice = "HOME"
                If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible AndAlso ddlBuyPoint.SelectedIndex > 0 Then
                    arrBuyPoin = ddlBuyPoint.SelectedValue.Split("|")
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "Spread", _
                                            SafeDouble(poData("HomeSpreadMoney")) + SafeDouble(arrBuyPoin(1)), pbFavorite, IncreaseSpread)

                    oTicketBet = getSpeadBet(SafeDouble(SafeString(poData("HomeSpread"))) + SafeDouble(arrBuyPoin(0)), nMoneyLine, sChoice)
                Else
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "Spread", _
                                            SafeDouble(poData("HomeSpreadMoney")), pbFavorite, IncreaseSpread)

                    oTicketBet = getSpeadBet(SafeDouble(SafeString(poData("HomeSpread"))), nMoneyLine, sChoice)
                End If

            Case txtItem.ID.Contains("AwayTotal") '"OVER_POINT", "UNDER_POINT"
                If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible AndAlso ddlBuyPoint.SelectedIndex > 0 Then
                    arrBuyPoin = ddlBuyPoint.SelectedValue.Split("|")
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "TotalPoints", _
                                                      SafeDouble(poData("TotalPointsOverMoney")) + SafeDouble(arrBuyPoin(1)), True)
                    sChoice = "OVER"
                    oTicketBet = getTotalPointsBet(SafeDouble(poData("TotalPoints")) + SafeDouble(arrBuyPoin(0)) _
                                                    , nMoneyLine _
                                                    , sChoice)
                    oTicketBet.TeamTotalName = sTeamTotalName
                Else

                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "TotalPoints", _
                                                 SafeDouble(poData("TotalPointsOverMoney")), True)
                    sChoice = "OVER"
                    oTicketBet = getTotalPointsBet(SafeDouble(poData("TotalPoints")) _
                                                    , nMoneyLine _
                                                    , sChoice)
                    oTicketBet.TeamTotalName = sTeamTotalName

                End If

            Case txtItem.ID.Contains("HomeTotal") '"OVER_POINT", "UNDER_POINT"
                If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible AndAlso ddlBuyPoint.SelectedIndex > 0 Then
                    arrBuyPoin = ddlBuyPoint.SelectedValue.Split("|")
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "TotalPoints", _
                                                               SafeDouble(poData("TotalPointsUnderMoney")) + SafeDouble(arrBuyPoin(1)), False)
                    sChoice = "UNDER"
                    oTicketBet = getTotalPointsBet(SafeDouble(poData("TotalPoints")) + SafeDouble(arrBuyPoin(0)) _
                                                    , nMoneyLine _
                                                    , sChoice)
                    oTicketBet.TeamTotalName = sTeamTotalName
                Else
                    arrBuyPoin = ddlBuyPoint.SelectedValue.Split("|")
                    nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "TotalPoints", _
                                                               SafeDouble(poData("TotalPointsUnderMoney")), False)
                    sChoice = "UNDER"
                    oTicketBet = getTotalPointsBet(SafeDouble(poData("TotalPoints")) _
                                                    , nMoneyLine _
                                                    , sChoice)
                    oTicketBet.TeamTotalName = sTeamTotalName
                End If
            Case (UCase(SafeString(sHABet)) = "OVER_POINT_TEAM_TOTAL" OrElse UCase(SafeString(sHABet)) = "UNDER_POINT_TEAM_TOTAL")
                Dim oArgs As String() = SafeString(sHABetA).Split("|"c)
                If oArgs.Count = 2 Then
                    sChoice = SafeString(IIf(UCase(SafeString(sHABet)) = "OVER_POINT_TEAM_TOTAL", "OVER", "UNDER"))

                    oTicketBet = getTeamTotalTotalPointsBet(SafeDouble(oArgs(0)) _
                                                    , SafeDouble(oArgs(1)) _
                                                    , sChoice)
                    oTicketBet.TeamTotalName = sTeamTotalName
                End If

            Case txtItem.ID.Contains("MoneyAwayMLine") '"AWAY_LINE"
                sChoice = "AWAY"
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "MoneyLine", _
                                                       SafeDouble(poData("AwayMoneyLine")), pbFavorite)
                oTicketBet = getMoneyLineBet(nMoneyLine, sChoice)
            Case txtItem.ID.Contains("MoneyHomeMLine")
                sChoice = "HOME"
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Home", poData("Context"), "MoneyLine", _
                                                          SafeDouble(poData("HomeMoneyLine")), pbFavorite)
                oTicketBet = getMoneyLineBet(nMoneyLine, sChoice)
            Case txtItem.ID.Contains("MoneyDraw") '"DRAW_LINE"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = "Draw"
                nMoneyLine = _oOddsRuleEngine.GetMoneyLine(SafeString(poData("GameID")), poData("GameType"), "Away", poData("Context"), "Draw", _
                                                     SafeDouble(poData("DrawMoneyLine")), False)
                oTicketBet.DrawMoneyLine = nMoneyLine
                If Not BetTypeActive.Replace(__BetIfAll, _sStraight).Replace(_sBetTheBoard, _sStraight).Equals(_sStraight, StringComparison.CurrentCultureIgnoreCase) Then
                    'chkItem.Checked = False
                    ClientAlert(String.Format("Draw Is Not Allowed With {0} Wager", BetTypeActive), True)
                    Return
                End If
            Case txtItem.ID.Contains("PropMoneyLine") '"PROP_LINE"
                oTicketBet = getPropLineBet(SafeDouble(CType(item.FindControl("lblPropMoneyLine"), Label).Text))
            Case Else
                '' Error
        End Select

        Try

            If oTicketBet IsNot Nothing Then
                oTicketBet.IsFavorite = pbFavorite
                'oTicketBet.IsUnderdog = pbUnderdog
                'If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible Then
                '    oTicketBet.BuyPointValue = ddlBuyPoint.ClientID & "|" & ddlBuyPoint.SelectedIndex
                'End If
                If IsBaseball(SafeString(poData("GameType"))) Then
                    oTicketBet.IsCheckPitcher = pbIsCheckPitcher
                End If
                oTicketBet.GameID = SafeString(poData("GameID")) 'CType(item.FindControl("hfGameID"), HiddenField).Value
                oTicketBet.GameLineID = SafeString(poData("GameLineID"))  'CType(item.FindControl("hfGameLineID"), HiddenField).Value
                oTicketBet.GameType = UCase(SafeString(poData("GameType")))  'UCase(SafeString(CType(item.FindControl("hfGameType"), HiddenField).Value))
                oTicketBet.Bookmaker = SafeString(poData("Bookmaker")) ' CType(item.FindControl("hfBookmaker"), HiddenField).Value
                oTicketBet.Context = SafeString(poData("Context")) 'CType(item.FindControl("hfContext"), HiddenField).Value
                oTicketBet.GameDate = SafeDate(poData("GameDate")) 'SafeDate(CType(item.FindControl("hfGameDate"), HiddenField).Value)
                If Not pbPropGame Then
                    oTicketBet.Description = SafeString(poData("Description")) 'CType(item.FindControl("hfDescription"), HiddenField).Value
                    oTicketBet.AwayTeam = CType(item.FindControl("lblAwayTeam"), Literal).Text
                    oTicketBet.HomeTeam = CType(item.FindControl("lblHomeTeam"), Literal).Text
                    oTicketBet.AwayTeamNumber = CType(item.FindControl("lblAwayNumber"), Literal).Text
                    oTicketBet.HomeTeamNumber = CType(item.FindControl("lblHomeNumber"), Literal).Text
                    oTicketBet.HomePitcher = SafeString(poData("HomePitcher")) 'CType(item.FindControl("hfHPitcher"), HiddenField).Value
                    oTicketBet.AwayPitcher = SafeString(poData("AwayPitcher")) 'CType(item.FindControl("hfAPitcher"), HiddenField).Value
                    oTicketBet.HomePitcherRH = SafeBoolean(poData("HomePitcherRightHand")) 'SafeBoolean(CType(item.FindControl("hfHPitcherRH"), HiddenField).Value)
                    oTicketBet.AwayPitcherRH = SafeBoolean(poData("AwayPitcherRightHand")) 'SafeBoolean(CType(item.FindControl("hfAPitcherRH"), HiddenField).Value)
                Else
                    oTicketBet.PropParticipantName = CType(item.FindControl("lbnPropParticipantName"), Literal).Text
                    oTicketBet.PropRotationNumber = CType(item.FindControl("lblPropRotationNumber"), Literal).Text
                End If
                If ddlBuyPoint IsNot Nothing AndAlso ddlBuyPoint.Visible AndAlso Not String.IsNullOrEmpty(ddlBuyPoint.SelectedValue) Then
                    oTicketBet.AddPointValid = ddlBuyPoint.SelectedValue.Split("|")(0)
                    oTicketBet.AddPointMoneyValid = ddlBuyPoint.SelectedValue.Split("|")(1)
                End If
                oTicketBet.SetWinAmount = bSetWinAmount
                Dim sError As String = ""
                Dim bAlreadyAdd As Boolean = False
                '' Create OddsRulesEngine
                Dim olstGameID As New List(Of String)
                Dim sActionType = BetTypeActive.Replace(_sBetTheBoard, _sStraight).Replace(__BetIfAll, _sStraight)
                olstGameID.Add(oTicketBet.GameID)
                'Dim oOddsRules As New COddRulesEngine(UserSession.Cache.GetAllOddsRules(SBCBL.std.GetSiteType), olstGameID, SelectedPlayer.SuperAdminID, False, SelectedPlayer.Template, SelectedPlayer.SuperAgentID)
                Dim oOddsRules As New COddRulesEngine(olstGameID, SelectedPlayer.SuperAdminID, False, SelectedPlayer.Template, SelectedPlayer.SuperAgentID)

                If UCase(sActionType) <> "STRAIGHT" Then
                    If UserSession.SelectedTicket(SelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                            UCase(UserSession.SelectedTicket(SelectedPlayerID).LastTicket.TicketType) = UCase(sActionType) Then
                        Try
                            sError = UserSession.SelectedTicket(SelectedPlayerID).LastTicket.AddTicketBet(oTicketBet)
                        Catch ex As Exception
                            ClientAlert(sActionType & "Is Not Allowed")
                            'chkItem.Checked = False
                            Return
                        End Try
                        bAlreadyAdd = True
                        If String.IsNullOrEmpty(sError) Then

                            'xem lai oHTKBet.Value = String.Format("{0}|{1}", UserSession.SelectedTicket(SelectedPlayerID).LastTicket.TicketID, oTicketBet.TicketBetID)

                        End If
                    End If
                End If
                'ClientAlert(txtItem.Text + sActionType, True)
                If Not bAlreadyAdd Then
                    If UserSession.SelectedTicket(SelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                            UserSession.SelectedTicket(SelectedPlayerID).LastTicket.NumOfTicketBets = 0 Then
                        UserSession.SelectedTicket(SelectedPlayerID).RemoveTickets(UserSession.SelectedTicket(SelectedPlayerID).LastTicket.TicketID)
                    End If

                    Dim oTicket As CTicket = New CTicket(sActionType, SuperAgentId, SelectedPlayerID)

                    Try
                        sError = oTicket.AddTicketBet(oTicketBet)

                        If sError = "" Then
                            sError = UserSession.SelectedTicket(SelectedPlayerID).AddTicket(oTicket)
                        End If
                    Catch ex As Exception
                        ClientAlert(sActionType & "Is Not Allowed")
                        'chkItem.Checked = False
                        Return
                    End Try
                    If String.IsNullOrEmpty(sError) Then
                        'xem lai  oHTKBet.Value = String.Format("{0}|{1}", UserSession.SelectedTicket(SelectedPlayerID).LastTicket.TicketID, oTicketBet.TicketBetID)
                        'ClientAlert(txtItem.Text, True)
                        If Not String.IsNullOrEmpty(txtItem.Text) Then
                            oTicket.BetAmount = SafeDouble(txtItem.Text)
                        End If
                    End If
                End If
                If Not oTicketBet.IsForProp AndAlso CType(item.FindControl("HFCircled"), HiddenField) IsNot Nothing Then
                    oTicketBet.IsCircled = CType(item.FindControl("HFCircled"), HiddenField).Value = "Y"
                End If
                Dim sTeamSide As String = ""
                Select Case sChoice
                    Case "OVER"
                        sTeamSide = "AWAY"
                    Case "UNDER"
                        sTeamSide = "HOME"
                    Case Else
                        sTeamSide = sChoice
                End Select
                If oTicketBet.IsCircled AndAlso (UCase(sActionType) = "STRAIGHT" OrElse sActionType.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase)) Then
                    If Not oTicketBet.ValidateStraightCircled(oOddsRules, sTeamSide, SuperAgentId) Then
                        'UserSession.SelectedTicket(SelectedPlayerID).LastTicket.RemoveTicketBets(oTicketBet.TicketBetID)
                        ''UserSession.SelectedTicket(SelectedPlayerID).RemoveTickets(oTicketBet.TicketBetID)
                        '' DeleteTicket(oTicketBet.TicketBetID)
                        StraightError = True
                        sError = "You Are Exceeding The Maximum Allowed For Circled Game."
                    End If
                End If

                ' validate game
                UserSession.SelectedTicket(SelectedPlayerID).Validate(SelectedPlayer.SuperAdminID, SelectedPlayerID, SelectedPlayer.Template, SelectedPlayer.BalanceAmount, SelectedPlayer.IsSuperPlayer, IncreaseSpread, SelectedPlayer.SuperAgentID)

                If sError <> "" Then
                    ClientAlert(sError, True)
                    'chkItem.Checked = False
                End If
            End If
        Catch ex As CTicketException
            StraightError = True
            ClientAlert(ex.Message, True)
            LogError(_log, "Bet Game ", ex)
        End Try
    End Sub

    ''bet for bet straight
    Public Function BetStraight() As Boolean
        'If UCase(BetTypeActive) <> UCase(__BetIfAll) OrElse UCase(BetTypeActive) <> UCase(_sBetTheBoard) Then
        '    Return True
        'Else
        '    UserSession.SelectedTicket(SelectedPlayerID) = Nothing
        'End If

        If UCase(BetTypeActive) = UCase(_sParlay) Then
            Return True

        End If
        If UCase(BetTypeActive) = UCase(_sReverse) Then
            Return True
        End If

        If UCase(BetTypeActive) = UCase(_sTeaser) Then
            Return True
        End If

        If BetTypeActive.Contains("If ") Then
            Return True
        End If

        UserSession.SelectedTicket(SelectedPlayerID) = Nothing

        'ClientAlert("", True)
        'If UCase(BetTypeActive) <> UCase(_sBetTheBoard) Then
        '    Return True
        'Else
        'UserSession.SelectedTicket(SelectedPlayerID) = Nothing
        ' End If
        Dim oGameLineManager As New CGameLineManager()
        ''bet game for prop
        If UserSession.SelectedBetType(Me.SelectedPlayerID) = "PROP" Then
            For i As Integer = 0 To rptProps.Items.Count - 1
                Dim rptPropLines As Repeater = CType(rptProps.Items(i).FindControl("rptPropLines"), Repeater)

                For j As Integer = 0 To rptPropLines.Items.Count - 1
                    Dim rptPropTeams As Repeater = CType(rptPropLines.Items(j).FindControl("rptPropTeams"), Repeater)
                    For k As Integer = 0 To rptPropTeams.Items.Count - 1

                        Dim txtMoneyPropMoneyLine As TextBox = CType(rptPropTeams.Items(k).FindControl("txtMoneyPropMoneyLine"), TextBox)

                        If Not String.IsNullOrEmpty(txtMoneyPropMoneyLine.Text) Then
                            Dim hfInfoProp As HiddenField = CType(rptPropTeams.Items(k).FindControl("hfInfoProp"), HiddenField)
                            Dim sGameLineID As String = hfInfoProp.Value
                            BetGame(True, txtMoneyPropMoneyLine, Nothing, oGameLineManager.GetGameLinesByID(sGameLineID), Nothing, False, True)
                            If StraightError Then
                                Return False
                            End If
                        End If
                    Next
                Next
            Next
            Return True
        End If
        For i As Integer = 0 To rptMain.Items.Count - 1
            Dim rptBets As Repeater = CType(rptMain.Items(i).FindControl("rptBets"), Repeater)
            For j As Integer = 0 To rptBets.Items.Count - 1
                Dim rptGameLines As Repeater = CType(rptBets.Items(j).FindControl("rptGameLines"), Repeater)
                For k As Integer = 0 To rptGameLines.Items.Count - 1


                    Dim ddlBuyPointHomeTotal As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointHomeTotal"), CDropDownList)
                    Dim ddlBuyPointAwayTotal As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointAwayTotal"), CDropDownList)
                    Dim ddlBuyPointHomeSpread As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointHomeSpread"), CDropDownList)
                    Dim ddlBuyPointAwaySpread As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointAwaySpread"), CDropDownList)
                    Dim txtMoneyAwaySpread As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwaySpread"), TextBox)
                    Dim txtMoneyAwayMLine As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayMLine"), TextBox)
                    Dim txtMoneyAwayTotal As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayTotal"), TextBox)
                    Dim txtMoneyHomeSpread As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeSpread"), TextBox)
                    Dim txtMoneyHomeMLine As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeMLine"), TextBox)
                    Dim txtMoneyAwayMLine2 As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayMLine2"), TextBox)
                    Dim txtMoneyHomeMLine2 As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeMLine2"), TextBox)
                    Dim txtMoneyHomeTotal As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeTotal"), TextBox)
                    Dim txtMoneyAwayTeamTotalOver As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayTeamTotalOver"), TextBox)
                    Dim txtMoneyAwayTeamTotalUnder As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayTeamTotalUnder"), TextBox)
                    Dim txtMoneyHomeTeamTotalOver As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeTeamTotalOver"), TextBox)
                    Dim txtMoneyHomeTeamTotalUnder As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeTeamTotalUnder"), TextBox)
                    Dim txtMoneyDraw As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyDraw"), TextBox)



                    Dim lblMoneyAwaySpread As Label = CType(rptGameLines.Items(k).FindControl("lblAwaySpread"), Label)
                    Dim lblMoneyAwayMLine As Label = CType(rptGameLines.Items(k).FindControl("lblAwayMoneyLine"), Label)
                    Dim lblMoneyAwayTotal As Label = CType(rptGameLines.Items(k).FindControl("lblAwayTotal"), Label)
                    Dim lblMoneyHomeSpread As Label = CType(rptGameLines.Items(k).FindControl("lblHomeSpread"), Label)
                    Dim lblMoneyHomeMLine As Label = CType(rptGameLines.Items(k).FindControl("lblHomeMoney"), Label)
                    Dim lblMoneyAwayMLine2 As Label = CType(rptGameLines.Items(k).FindControl("lblAwayMoneyLine2"), Label)
                    Dim lblMoneyHomeMLine2 As Label = CType(rptGameLines.Items(k).FindControl("lblHomeMoney2"), Label)
                    Dim lblMoneyHomeTotal As Label = CType(rptGameLines.Items(k).FindControl("lblHomeTotal"), Label)
                    Dim lblMoneyAwayTeamTotalOver As Literal = CType(rptGameLines.Items(k).FindControl("lblAwayTeamTotalPointsOverMoney"), Literal)
                    Dim lblMoneyAwayTeamTotalUnder As Literal = CType(rptGameLines.Items(k).FindControl("lblAwayTeamTotalPointsUnderMoney"), Literal)
                    Dim lblMoneyHomeTeamTotalOver As Literal = CType(rptGameLines.Items(k).FindControl("lblHomeTeamTotalPointsOverMoney"), Literal)
                    Dim lblMoneyHomeTeamTotalUnder As Literal = CType(rptGameLines.Items(k).FindControl("lblHomeTeamTotalPointsUnderMoney"), Literal)
                    Dim lblMoneyDraw As Label = CType(rptGameLines.Items(k).FindControl("lblDrawMoney"), Label)


                    Dim chkSelectAwaySpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwaySpread"), CheckBox)
                    Dim chkSelectAwayMLine As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayMLine"), CheckBox)
                    Dim chkSelectAwayTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayTotal"), CheckBox)
                    Dim chkSelectHomeSpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeSpread"), CheckBox)
                    Dim chkSelectHomeMLine As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeMLine"), CheckBox)
                    Dim chkSelectAwayMLine2 As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayMLine2"), CheckBox)
                    Dim chkSelectHomeMLine2 As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeMLine2"), CheckBox)
                    Dim chkSelectHomeTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeTotal"), CheckBox)
                    Dim chkAwayTeamTotalOver As CheckBox = CType(rptGameLines.Items(k).FindControl("chkAwayTeamTotalOver"), CheckBox)
                    Dim chkAwayTeamTotalUnder As CheckBox = CType(rptGameLines.Items(k).FindControl("chkAwayTeamTotalUnder"), CheckBox)
                    Dim chkHomeTeamTotalOver As CheckBox = CType(rptGameLines.Items(k).FindControl("chkHomeTeamTotalOver"), CheckBox)
                    Dim chkHomeTeamTotalUnder As CheckBox = CType(rptGameLines.Items(k).FindControl("chkHomeTeamTotalUnder"), CheckBox)
                    Dim chkSelectDraw As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectDraw"), CheckBox)



                    Dim bAwaySpreadJuice As Boolean
                    Dim bHomeSpreadJuice As Boolean
                    Dim bAwayMoneyLineJuice As Boolean
                    Dim bHomeMoneyLineJuice As Boolean
                    Dim bAwayTotalJuice As Boolean = True
                    Dim bHomeTotalJuice As Boolean = False

                    Dim hfInfo As HiddenField = CType(rptGameLines.Items(k).FindControl("hfInfo"), HiddenField)
                    Dim sGameLineID As String = hfInfo.Value
                    If chkAwayTeamTotalOver.Checked OrElse txtMoneyAwayTeamTotalOver.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyAwayTeamTotalOver.Text.Substring(2).Contains("+"), txtMoneyAwayTeamTotalOver, False, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If

                    End If
                    If chkAwayTeamTotalUnder.Checked OrElse txtMoneyAwayTeamTotalUnder.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyAwayTeamTotalUnder.Text.Substring(2).Contains("+"), txtMoneyAwayTeamTotalUnder, False, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If
                    End If
                    If chkHomeTeamTotalOver.Checked OrElse txtMoneyHomeTeamTotalOver.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyHomeTeamTotalOver.Text.Substring(2).Contains("+"), txtMoneyHomeTeamTotalOver, False, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If
                    End If

                    If chkHomeTeamTotalUnder.Checked OrElse txtMoneyHomeTeamTotalUnder.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyHomeTeamTotalUnder.Text.Substring(2).Contains("+"), txtMoneyHomeTeamTotalUnder, False, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If
                    End If

                    If chkSelectAwaySpread.Checked OrElse txtMoneyAwaySpread.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                        Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                        getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                        BetGame(lblMoneyAwaySpread.Text.Substring(8).Contains("+"), txtMoneyAwaySpread, bAwaySpreadJuice, oData, ddlBuyPointAwaySpread, False)
                        If StraightError Then
                            Return False
                        End If
                    End If
                    'If chkSelectHomeSpread.Checked Then
                    '    Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                    '    Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                    '    Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                    '    getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                    '    BetGame(lblMoneyAwayMLine.Text.Substring(2).Contains("+"), txtMoneyAwayMLine, bAwayMoneyLineJuice, oData, ddlBuyPointHomeSpread, False)
                    '    If StraightError Then
                    '        Return False
                    '    End If
                    'End If
                    'If chkSelectDraw.Visible OrElse txtMoneyDraw.Text <> "" Then
                    '    Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                    '    Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                    '    Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                    '    getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                    '    BetGame(lblMoneyAwayMLine2.Text.Substring(2).Contains("+"), txtMoneyDraw, bAwayMoneyLineJuice, oData, ddlBuyPointHomeSpread, True)
                    '    If StraightError Then
                    '        Return False
                    '    End If
                    'End If
                    'If chkSelectAwayMLine.Checked Then
                    '    Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                    '    BetGame(lblMoneyAwayTotal.Text.Substring(8).Contains("+"), txtMoneyAwayTotal, True, oData, ddlBuyPointAwayTotal, False)
                    '    If StraightError Then
                    '        Return False
                    '    End If
                    'End If
                    If chkSelectHomeSpread.Checked OrElse txtMoneyHomeSpread.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                        Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                        getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                        BetGame(lblMoneyHomeSpread.Text.Substring(8).Contains("+"), txtMoneyHomeSpread, bHomeSpreadJuice, oData, ddlBuyPointHomeSpread, False)
                        If StraightError Then
                            Return False
                        End If
                    End If
                    If chkSelectHomeMLine.Checked OrElse txtMoneyHomeMLine.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                        Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                        getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                        BetGame(lblMoneyHomeMLine.Text.Substring(2).Contains("+"), txtMoneyHomeMLine, bHomeMoneyLineJuice, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If
                    End If
                    If chkSelectAwayMLine.Checked OrElse txtMoneyAwayMLine.Text <> "" Then 'AndAlso Not String.IsNullOrEmpty(txtMoneyAwayMLine.Text) Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
                        Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
                        getTeamJuice(nAwaySpread, nHomeSpread, bAwaySpreadJuice, bHomeSpreadJuice, bAwayMoneyLineJuice, bHomeMoneyLineJuice)
                        BetGame(lblMoneyAwayMLine.Text.Substring(2).Contains("+"), txtMoneyAwayMLine, bHomeMoneyLineJuice, oData, Nothing, True)
                        If StraightError Then
                            Return False
                        End If
                    End If
                    If chkSelectAwayTotal.Checked OrElse txtMoneyAwayTotal.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyAwayTotal.Text.Substring(8).Contains("+"), txtMoneyAwayTotal, False, oData, ddlBuyPointAwayTotal, False)
                        If StraightError Then
                            Return False
                        End If
                    End If

                    If chkSelectHomeTotal.Checked OrElse txtMoneyHomeTotal.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyHomeTotal.Text.Substring(8).Contains("+"), txtMoneyHomeTotal, False, oData, ddlBuyPointHomeTotal, False)
                        If StraightError Then
                            Return False
                        End If
                    End If


                    If chkSelectDraw.Checked OrElse txtMoneyDraw.Text <> "" Then
                        Dim oData = oGameLineManager.GetGameLinesByID(sGameLineID)
                        BetGame(lblMoneyDraw.Text.Substring(2).Contains("+"), txtMoneyDraw, Nothing, oData, Nothing, False)
                        If StraightError Then
                            Return False
                        End If
                    End If
                Next
            Next
        Next
        Return True
    End Function

    'Protected Sub SelectBetType(ByVal sender As Object, ByVal e As System.EventArgs)
    '    ViewState("Title") = Nothing
    '    If UserSession.SelectedBetType(Me.SelectedPlayerID) = "PROP" Then
    '        Response.Redirect("default.aspx")
    '        Return
    '    End If
    '    If ucWagers.Visible Then
    '        pnBetAction.Visible = True
    '        ucWagers.Visible = False
    '        hfIsWagers.Value = False
    '        ' BindGameLines()
    '        BetTypeActive = CType(sender, LinkButton).Parent.ID
    '        ClearWager()
    '        ActiveMenu()
    '        ucWagers.ShowWager(True)
    '        Return
    '    End If
    '    Dim sBetType As String = CType(sender, LinkButton).CommandArgument
    '    If String.IsNullOrEmpty(sBetType) Then
    '        Return
    '    Else
    '        BetTypeActive = CType(sender, LinkButton).Parent.ID
    '        ActiveMenu()
    '        ClearWager()
    '    End If
    'End Sub

    'Public Sub GetSelectGame(ByVal psBetType As String)
    '    For i As Integer = 0 To rptMain.Items.Count - 1
    '        Dim rptBets As Repeater = CType(rptMain.Items(i).FindControl("rptBets"), Repeater)

    '        For j As Integer = 0 To rptBets.Items.Count - 1
    '            Dim rptGameLines As Repeater = CType(rptBets.Items(j).FindControl("rptGameLines"), Repeater)
    '            For k As Integer = 0 To rptGameLines.Items.Count - 1
    '                Dim txtMoneyAwaySpread As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwaySpread"), TextBox)
    '                Dim txtMoneyAwayMLine As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayMLine"), TextBox)
    '                Dim txtMoneyAwayTotal As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyAwayTotal"), TextBox)
    '                Dim txtMoneyHomeSpread As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeSpread"), TextBox)
    '                Dim txtMoneyHomeMLine As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeMLine"), TextBox)
    '                Dim txtMoneyHomeTotal As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyHomeTotal"), TextBox)
    '                Dim txtMoneyDraw As TextBox = CType(rptGameLines.Items(k).FindControl("txtMoneyDraw"), TextBox)
    '                Dim chkSelectAwaySpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwaySpread"), CheckBox)
    '                Dim chkSelectAwayMLine As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayMLine"), CheckBox)
    '                Dim chkSelectAwayTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayTotal"), CheckBox)
    '                Dim chkSelectHomeSpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeSpread"), CheckBox)
    '                Dim chkSelectHomeMLine As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeMLine"), CheckBox)
    '                Dim chkSelectHomeTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeTotal"), CheckBox)
    '                Dim chkSelectDraw As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectDraw"), CheckBox)
    '                Dim oDisplay As String
    '                If psBetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) Then
    '                    oDisplay = "inline-block"
    '                Else
    '                    oDisplay = "none"
    '                End If
    '                txtMoneyAwaySpread.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyAwayMLine.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyAwayTotal.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyHomeSpread.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyHomeMLine.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyHomeTotal.Attributes.Add("style", "display:" & oDisplay)
    '                txtMoneyDraw.Attributes.Add("style", "display:" & oDisplay)
    '                If oDisplay = "none" Then
    '                    oDisplay = "inline-block"
    '                Else
    '                    oDisplay = "none"
    '                End If
    '                chkSelectAwaySpread.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectAwayMLine.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectAwayTotal.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectHomeSpread.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectHomeMLine.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectHomeTotal.Attributes.Add("style", "display:" & oDisplay)
    '                chkSelectDraw.Attributes.Add("style", "display:" & oDisplay)
    '            Next
    '        Next
    '    Next
    'End Sub

    Protected Function GetSpreadTitle(ByVal psGameType As String, ByVal psTitle As String) As String
        If psGameType = "MLB AL Baseball" Or psGameType = "MLB NL Baseball" Or psGameType = "MLB Baseball" Then
            If psTitle = "Spread" Then
                Return "Run Line"
            Else
                Return "Total Runs"
            End If
        ElseIf psGameType = "NHL Hockey" Then
            If psTitle = "Spread" Then
                Return "Puck Line"
            End If
        End If
        Return psTitle
    End Function

    Protected Sub rptProps_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProps.ItemDataBound
        Dim sType As String = SafeString(e.Item.DataItem)
        Dim rptPropLines As Repeater = e.Item.FindControl("rptPropLines")
        '' load game by game type to each of rptBets
        LoadPropGames(sType, rptPropLines)
    End Sub

    Private Sub LoadPropGames(ByVal psSportType As String, ByVal poRepeater As Repeater)
        Dim oRemoveOldGame As New Dictionary(Of String, String)

        Dim oToday = GetEasternDate()
        Dim oGameShows As New Dictionary(Of String, DataTable)
        Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAvailableProps(psSportType, oToday)
        Dim oListGameIDs As New List(Of String)
        For Each oRow As DataRow In odtGames.Rows
            If oListGameIDs.Contains(SafeString(oRow("GameID"))) Then
                Continue For
            End If
            odtGames.DefaultView.RowFilter = "GameID = " & SQLString(SafeString(oRow("GameID"))) & "  and Bookmaker= 'Pinnacle'"
            Dim oDTLines As DataTable = odtGames.DefaultView.ToTable()
            oDTLines.DefaultView.RowFilter = " isnull(Propmoneyline,0) <>0  "
            Dim nCountLines As Integer = oDTLines.DefaultView.ToTable().Rows.Count
            ' Get the pinacle prop if there is only line of Pinnacle Bookmaker
            If nCountLines = 0 Then
                odtGames.DefaultView.RowFilter = "GameID = " & SQLString(SafeString(oRow("GameID"))) & " and Bookmaker= 'Pinnacle'"
                oDTLines = odtGames.DefaultView.ToTable()
            End If
            ''remove game old
            If oRemoveOldGame.ContainsKey(SafeString(oRow("PropDescription"))) Then
                oGameShows.Remove(oRemoveOldGame(SafeString(oRow("PropDescription"))))
                oListGameIDs.Remove(oRemoveOldGame(SafeString(oRow("PropDescription"))))
            End If
            oGameShows(SafeString(oRow("GameID"))) = oDTLines
            oListGameIDs.Add(SafeString(oRow("GameID")))
            oRemoveOldGame(SafeString(oRow("PropDescription"))) = SafeString(oRow("GameID"))
        Next
        poRepeater.DataSource = oGameShows
        poRepeater.DataBind()
        poRepeater.Visible = oGameShows.Count > 0
    End Sub

    Protected Sub rptPropLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim oDTGame As DataTable = CType(e.Item.DataItem, KeyValuePair(Of String, DataTable)).Value
        If oDTGame.Rows.Count = 0 Then
            'e.Item.FindControl("divProp").Visible = False
            Return
        End If

        CType(e.Item.FindControl("lblGameDate"), Literal).Text = SafeDate(oDTGame.Rows(0)("GameDate")).ToString("MM/dd/yyyy")
        CType(e.Item.FindControl("lblHourDate"), Literal).Text = SafeDate(oDTGame.Rows(0)("GameDate")).ToString("hh:mm") + String.Format("{0:tt}", CDate(oDTGame.Rows(0)("GameDate")))
        CType(e.Item.FindControl("lblPropDes"), Literal).Text = SafeString(oDTGame.Rows(0)("PropDescription"))
        Dim rptPropTeams As Repeater = CType(e.Item.FindControl("rptPropTeams"), Repeater)
        rptPropTeams.DataSource = oDTGame
        rptPropTeams.DataBind()
    End Sub

    Protected Sub rptPropTeams_ItemdataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim lblPropMoneyLine As Label = CType(e.Item.FindControl("lblPropMoneyLine"), Label)
        Dim nMoneyLine As Double = SafeDouble(CType(e.Item.DataItem, DataRowView)("PropMoneyLine"))
        lblPropMoneyLine.Text = IIf(nMoneyLine > 0, "+" & nMoneyLine, "-" & nMoneyLine)
        'CType(e.Item.FindControl("hfaSelectPropMoneyLine"), HiddenField).Value = SafeString(nMoneyLine)
        'CType(e.Item.FindControl("chkSelectPropMoneyLine"), CheckBox).Visible = False
        lblPropMoneyLine.Enabled = nMoneyLine <> 0
    End Sub

    Protected Sub chkTeamTotal_click(ByVal sender As Object, ByVal e As System.EventArgs)
        BindBettingData()
        bindSuperInfo()
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count = 0 OrElse UserSession.SelectedTicket(SelectedPlayerID).Tickets(0).TicketBets.Count = 1 Then
            Select Case BetTypeActive
                Case _sParlay, _sReverse, _sTeaser
                    ClientAlert("You have to select at least 2 selections", True)
                    Return

            End Select
        End If
        ViewState("Title") = Nothing
        If Not BetStraight() Then
            Return
        End If
        If UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count = 0 Then
            Select Case BetTypeActive
                Case __BetIfAll, _sStraight, _sBetTheBoard
                    ClientAlert("You have to select at least 1 selection", True)
            End Select
            Return
        End If
        If BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) OrElse BetTypeActive.Equals(_sReverse, StringComparison.CurrentCultureIgnoreCase) Then
            checkUpdateBuyPoint()
        End If
        pnBetAction.Visible = False
        ucWagers.Visible = True
        hfIsWagers.Value = True
        'UserSession.SelectedGameTypes(Me.SelectedPlayerID).Clear()
        ucWagers.SelectedPlayerID = SelectedPlayerID
        ucWagers.BindWagers()
    End Sub

    Public Sub ClearWager()
        If Not String.IsNullOrEmpty(Me.SelectedPlayerID) Then
            'BindBettingData()
            'GetSelectGame(BetTypeActive)
            If Not LCase(BetTypeActive).Contains("if ") OrElse (UserSession.SelectedTicket(SelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                                                                UCase(UserSession.SelectedTicket(SelectedPlayerID).LastTicket.TicketType) <> "IF BET") Then
                UserSession.SelectedTicket(SelectedPlayerID) = Nothing
            End If
        End If
    End Sub
    'Active mune select game(change backgound)
    'Public Sub ActiveMenu()
    '    Straight.Style.Add("Color", "")
    '    'lbtStraight.Style.Add("Color", "")
    '    Parlay.Style.Add("Color", "")
    '    'lbtParlay.Style.Add("Color", "")
    '    Teaser.Style.Add("Color", "")
    '    'lbtTeaser.Style.Add("Color", "")
    '    Reverse.Style.Add("Color", "")
    '    'lbtReverse.Style.Add("Color", "")
    '    Select Case UCase(BetTypeActive)
    '        Case ""
    '            Straight.Style.Add("Color", "Yellow")
    '            ' lbtStraight.Style.Add("Color", "Yellow")
    '        Case "STRAIGHT"
    '            Straight.Style.Add("Color", "Yellow")
    '            ' lbtStraight.Style.Add("Color", "Yellow")
    '        Case "PARLAY"
    '            Parlay.Style.Add("Color", "Yellow")
    '            ' lbtParlay.Style.Add("Color", "Yellow")
    '        Case "TEASER"
    '            Teaser.Style.Add("Color", "Yellow")
    '            'lbtTeaser.Style.Add("Color", "Yellow")
    '        Case "REVERSE"
    '            Reverse.Style.Add("Color", "Yellow")
    '            ' lbtReverse.Style.Add("Color", "Yellow")
    '    End Select
    'End Sub

    ''Disable Menu when select Prop Game
    'Public Sub DisableMenu()
    '    Parlay.Disabled = True
    '    lbtParlay.Enabled = False
    '    Teaser.Disabled = True
    '    lbtTeaser.Enabled = False
    '    Reverse.Disabled = True
    '    lbtReverse.Enabled = False
    'End Sub

    ''psTiketID include string ticketId and ticketbetID
    Public Sub DeleteTicket(ByVal psTiketID As String)
        Dim oID As String() = SafeString(psTiketID).Split("|"c)
        Dim oTicket As CTicket = UserSession.SelectedTicket(SelectedPlayerID).GetTicket(oID(0))
        If oTicket IsNot Nothing Then
            If oTicket.TicketBets.Count > 1 Then
                oTicket.RemoveTicketBets(oID(1))
            Else
                UserSession.SelectedTicket(SelectedPlayerID).RemoveTickets(oID(0))
            End If
        End If
    End Sub

    Private Function GetTPointLock(ByVal poRow As DataRowView) As Boolean
        'Dim olstGameTypeOnOffCurrent As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(SuperAgentId, SafeString(poRow("GameType")), True)
        'Dim olstGameTypeOnOff1H As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(SuperAgentId, SafeString(poRow("GameType")), False)

        Select Case UCase(SafeString(poRow("Context")))
            Case "CURRENT"
                'ClientAlert("time minh set :" & (60 * (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("GameTotalPointsDisplay", SafeString(poRow("GameType"))))) & ":::: Time cua game " & SafeInteger(poRow("MinuteBefore")))
                'ClientAlert(UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").Count)
                'ClientAlert(NminuteBeforeSet < SafeInteger(poRow("MinuteBefore")))
                If UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").Count > 0 Then
                    Dim NminuteBeforeSet As Double = (60 * (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("GameTotalPointsDisplay", SafeString(poRow("GameType")))))
                    'ClientAlert( (60 * (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("GameTotalPointsDisplay", SafeString(poRow("GameType"))) < SafeInteger(poRow("MinuteBefore")))))
                    Return SafeString(poRow("IsGameTPointLocked")) = "Y" Or NminuteBeforeSet < SafeInteger(poRow("MinuteBefore"))
                End If
                Return SafeString(poRow("IsGameTPointLocked")) = "Y"
            Case "1H"
                If UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").Count > 0 Then
                    Dim NminuteBeforeSet As Double = (60 * (UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "1H").GetIntegerValue("GameTotalPointsDisplay", SafeString(poRow("GameType")))))
                    Return SafeString(poRow("IsFirstHalfTPointLocked")) = "Y" Or NminuteBeforeSet < SafeInteger(poRow("MinuteBefore"))
                End If
            Case "2H"
                Return SafeString(poRow("IsSecondHalfTPointLocked")) = "Y"
            Case "1Q", "2Q", "3Q", "4Q"
                If UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour").Count Then 'olstGameTypeOnOffCurrent Is Nothing OrElse olstGameTypeOnOffCurrent.Count = 0 Then
                    Return False
                End If
                Return (60 * UserSession.Cache.GetSysSettings(SuperAgentId & "LineOffHour", "Current").GetIntegerValue("GameTotalPointsDisplay", SafeString(poRow("GameType"))) < SafeInteger(poRow("MinuteBefore")))
        End Select
        Return False
    End Function

    Private Function GetMLLock(ByVal poRow As DataRowView) As Boolean

        Dim sCategory = SBCBL.std.GetSiteType & " MoneyLineOff"
        Dim sGameType As String = SafeString(poRow("GameType"))
        Dim oSysManager As New CSysSettingManager()
        Dim oCachemanager As New CCacheManager()
        Dim oSysGame As CSysSetting = oCachemanager.GetAllSysSettings(_sGameType).Find(Function(x) x.Key.Equals(sGameType, StringComparison.CurrentCultureIgnoreCase) AndAlso x.SubCategory <> "")
        Dim oSysSport As CSysSetting
        Dim sSportType As String
        If oSysGame IsNot Nothing Then
            oSysSport = oCachemanager.GetAllSysSettings(_sGameType).Find(Function(x) x.SysSettingID = oSysGame.SubCategory)
            sSportType = oSysSport.Key
        Else
            sSportType = sGameType.Replace(" Live", "")
        End If
        Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory, "GT")
        Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sSportType)

        '' check money line off limitation
        '' disable money line betting when away spead over limitation
        If oSetting IsNot Nothing AndAlso SafeDouble(oSetting.Value) <> 0 AndAlso SafeDouble(poRow("AwaySpread")) > SafeDouble(oSetting.Value) Then
            Return True
        End If

        oListSettings = UserSession.SysSettings(sCategory, "LT")
        oSetting = oListSettings.Find(Function(x) x.Key = sSportType)

        '' check money line off limitation
        '' disable money line betting when away spead over limitation
        If oSetting IsNot Nothing AndAlso SafeDouble(oSetting.Value) <> 0 AndAlso SafeDouble(poRow("AwaySpread")) < SafeDouble(oSetting.Value) Then
            Return True
        End If
        Select Case UCase(SafeString(poRow("Context")))
            Case "CURRENT"
                Return SafeString(poRow("IsGameMLLocked")) = "Y"
            Case "1H"
                Return SafeString(poRow("IsFirstHalfMLLocked")) = "Y"
            Case "2H"
                Return SafeString(poRow("IsSecondHalfMLLocked")) = "Y"
        End Select
        Return False
    End Function

    Public Sub BackButtonClick() Handles ucWagers.BackButtonClick
        pnBetAction.Visible = True
        ucWagers.Visible = False
        hfIsWagers.Value = False
        BindGameLines()
        ' BetTypeActive = _sStraight
        ClearWager()
        '  ActiveMenu()
    End Sub


    '' get money increase for page method
    Public Function GetMoneyGameIncrease(ByVal psGameID As String, ByVal psGameType As String, _
                                     ByVal psTeam As String, ByVal psContext As String, _
                                     ByVal psBetType As String, ByVal pnOdds As Double, ByVal psGameLineID As String) As Double
        Dim nMoneyLine As Double
        nMoneyLine = _oOddsRuleEngine.GetMoneyLine(psGameID, psGameType, psTeam, psContext, psBetType, _
                                                                     pnOdds, 0, IncreaseSpread)
        '_oOddsRuleEngine.GetMoneyLine(sGameID, sGameType, "Away", sContext, "TotalPoints", _
        '                                               SafeDouble(oData("TotalPointsOverMoney")))

        Return nMoneyLine
    End Function

    Public Function GetMoneyGameIncreaseSpreadML(ByVal psGameID As String, ByVal psGameType As String, _
                                     ByVal psTeam As String, ByVal psContext As String, _
                                     ByVal psBetType As String, ByVal pnOdds As Double, ByVal psGameLineID As String, ByVal pnAwaySpread As Double, ByVal pnHomeSpread As Double) As Double

        Dim bFavorite As Boolean
        Dim nAwaySpread As Single = pnAwaySpread
        Dim nHomeSpread As Single = pnHomeSpread
        If nAwaySpread < nHomeSpread Then
            If psTeam.Equals("Away") Then
                bFavorite = True
            Else
                bFavorite = False
            End If
        End If
        If nHomeSpread < nAwaySpread Then
            If psTeam.Equals("Away") Then
                bFavorite = False
            Else
                bFavorite = True
            End If
        End If
        If nAwaySpread = nHomeSpread Then
            If (psTeam.Equals("Away")) Then
                bFavorite = True
            Else
                bFavorite = False
            End If
        End If
        Return _oOddsRuleEngine.GetMoneyLine(psGameID, psGameType, psTeam, psContext, psBetType, _
                                                                      pnOdds, bFavorite, IncreaseSpread)
    End Function

    Public Function GetMoneyGameIncreaseTotal(ByVal psGameID As String, ByVal psGameType As String, _
                                 ByVal psTeam As String, ByVal psContext As String, _
                                 ByVal psBetType As String, ByVal pnOdds As Double, ByVal psGameLineID As String, ByVal pbFavorite As String) As Double

        Return _oOddsRuleEngine.GetMoneyLine(psGameID, psGameType, psTeam, psContext, psBetType, _
                                                                      pnOdds, SafeBoolean(pbFavorite), IncreaseSpread)
    End Function

    Protected Sub btnUpdateLines_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLines.Click
        ViewState("Title") = Nothing
        BindBettingData()
    End Sub


    'Protected Sub lbnBackGame_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnBackGame.Click, lbnBackToGame2.Click
    '    If UserSession.UserType = SBCBL.EUserType.Player Then
    '        Response.Redirect("Default.aspx")
    '    Else
    '        Response.Redirect("SelectGame.aspx")
    '    End If
    'End Sub

    Public Function getFavout(ByVal pnAwaySpread As Double, ByVal pnHomeSpread As Double, ByVal psTeam As String) As String
        'Dim nAwaySpread As Single = SafeSingle(oData("AwaySpread"))
        'Dim nHomeSpread As Single = SafeSingle(oData("HomeSpread"))
        Dim sFavout As String = ""
        If psTeam.Equals("Away") Then
            sFavout = IIf(pnAwaySpread < pnHomeSpread, "True", "False")
            If pnAwaySpread = pnHomeSpread Then
                sFavout = "True"
            End If
        Else
            sFavout = IIf(pnAwaySpread < pnHomeSpread, "False", "True")
            If pnAwaySpread = pnHomeSpread Then
                sFavout = "False"
            End If
        End If
        Return sFavout
    End Function

    Public Function ValidDescription(ByVal psDescription As String) As String
        Return psDescription.Replace("'", "&quot;")
    End Function

    'Public Sub HightGameLight(ByVal psGameLineID As String, ByVal psContext As String, ByVal poGameLine As Label)
    '    psGameLineID &= (psContext & SelectedPlayerID)
    '    If (HighlightRecords IsNot Nothing And HighlightRecords.ContainsKey(psGameLineID & poGameLine.ClientID)) Then
    '        If Not HighlightRecords(psGameLineID & poGameLine.ClientID).Equals(poGameLine.Text) Then
    '            'poGameLine.BackColor = Drawing.Color.Yellow
    '            poGameLine.Style.Add("background", "url(/SBS/images/bg_flash.gif)")
    '            ' poGameLine.Style.Add("color", "red")
    '            ' poGameLine.Style.Add("background", "Yellow")
    '            'lbtUpdateLineTop.BackColor = Drawing.Color.Yellow
    '            HighlightRecords.Remove(psGameLineID & poGameLine.ClientID)

    '        End If

    '    End If
    '    HighlightRecords(psGameLineID & poGameLine.ClientID) = poGameLine.Text



    'End Sub


    Private Sub getTeamJuice(ByVal pnAwaySpread As Single, ByVal pnHomeSpread As Single, ByRef bAwaySpreadJuice As Boolean, ByRef bHomeSpreadJuice As Boolean, ByRef bAwayMoneyLineJuice As Boolean, ByRef bHomeMoneyLineJuice As Boolean)
        bAwaySpreadJuice = IIf(pnAwaySpread < pnHomeSpread, True, False)
        bHomeSpreadJuice = IIf(pnAwaySpread < pnHomeSpread, False, True)
        If pnAwaySpread = pnHomeSpread Then
            bAwaySpreadJuice = True
            bHomeSpreadJuice = False
        End If
        bAwayMoneyLineJuice = bAwaySpreadJuice
        bHomeMoneyLineJuice = bHomeSpreadJuice
    End Sub

    Public Function ShowTextBox() As Boolean
        'If BetTypeActive.Equals("BetTheBoard", StringComparison.CurrentCultureIgnoreCase) Then
        '    Return True
        'Else
        '    Return False
        'End If

        If BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) OrElse _
         BetTypeActive.Equals(__IfWin, StringComparison.CurrentCultureIgnoreCase) OrElse _
         BetTypeActive.Equals(__IfWinOrPush, StringComparison.CurrentCultureIgnoreCase) Then
            Return True
        Else
            Return False
        End If
        Return False
    End Function

    Public Function ShowCheckBox() As Boolean
        If BetTypeActive.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse _
         LCase(BetTypeActive).Contains("if ") OrElse _
         BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function ShowRadio() As Boolean
        If BetTypeActive.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetTypeActive).Contains("if ") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function addPointTeaser(ByVal pnPoint As Double, ByVal psGameType As String, ByVal pbaddPoint As Boolean) As Double
        Dim oTeaserRule As CTeaserRule
        If BetTypeActive.Equals(_sTeaser, StringComparison.CurrentCultureIgnoreCase) Then
            oTeaserRule = CType(Session("TeaserValue"), CTeaserRule)
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
        End If
        Return pnPoint
    End Function

    Public Function getCssClass() As String
        Return "offering_pair_" & SBCBL.std.SafeString(ViewState("OddEven"))
    End Function

    Public Function getImage(ByVal psGameType As String, ByVal psTeam As String) As String
        If psGameType.Equals("Boxing", StringComparison.CurrentCultureIgnoreCase) Then
            Return "Other/Boxing.jpg"
        ElseIf psGameType.Equals("Golf", StringComparison.CurrentCultureIgnoreCase) Then
            Return "Other/Golf.jpg"
        ElseIf psGameType.Equals("Nascar", StringComparison.CurrentCultureIgnoreCase) Then
            Return "Other/Nascar.jpg"
        ElseIf psGameType.Equals("Tennis", StringComparison.CurrentCultureIgnoreCase) Then
            Return "Other/Tennis.jpg"
        Else
            Return psGameType.Replace(" Baseball", "").Replace(" Hockey", "").Replace(" Basketball", "").Replace(" Football", "") & " Gif/" & psTeam & ".gif"
        End If
    End Function

    Public Function getHeader() As String
        If SafeBoolean(Session("BetTypeProp")) AndAlso BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) Then
            Return "Please enter amounts for one or more wagers in the charts below"
        ElseIf BetTypeActive.Equals(_sReverse) Then
            Return "Please select two items from the chart below"
        ElseIf BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) Then
            Return "Please enter amounts for one or more games in chart below"
        Else
            Return "Please select items from the chart below"

        End If
    End Function

    Public Sub ParseBuyPointOptions(ByVal pnJuice As Double, ByVal pnBetPoint As Double, ByVal psBetType As String, ByVal psContext As String, ByVal psGameType As String, ByVal ddlBuyPoint As CDropDownList, ByVal lblOdd As Label, Optional ByVal bTotalOver As Boolean = False)
        Try
            'ddlBuyPoint.Visible = True
            'lblOdd.Visible = False

            Dim olstDic As New List(Of DictionaryEntry)
            If chkBuyPoint.Checked AndAlso (BetTypeActive.Equals(_sBetTheBoard, StringComparison.CurrentCultureIgnoreCase) _
                                                OrElse BetTypeActive.Equals(_sParlay, StringComparison.CurrentCultureIgnoreCase) _
                                                OrElse BetTypeActive.Equals(__BetIfAll, StringComparison.CurrentCultureIgnoreCase) _
                                                OrElse BetTypeActive.Equals(_sReverse, StringComparison.CurrentCultureIgnoreCase)) Then
                '' Only Football and Basketball can buy points
                '' Allow buy point when juice is between -110 and -100
                Dim bBuyPoint As Boolean = UCase(psBetType) <> "MONEYLINE" AndAlso _
                UCase(psContext) = "CURRENT" AndAlso pnBetPoint >= -110 AndAlso pnBetPoint <= -100 _
                AndAlso (IsFootball(psGameType) OrElse IsBasketball(psGameType))

                If bBuyPoint Then

                    ''Single Parlay
                    Dim oDicItem As New DictionaryEntry(SafeString(pnJuice) & "   " & SafeString(If(pnBetPoint = -100, 100, pnBetPoint)), "")
                    olstDic.Add(oDicItem)

                    If IsFootball(psGameType) Then
                        '' OFF 3: From +3 to +3 1/2 | From -3 to -2 1/2
                        '' ON 3: From +2 1/2 to +3 | From -3 1/2 to -3
                        If pnJuice = 3 OrElse pnJuice = -3 OrElse pnJuice = 2.5 OrElse pnJuice = -3.5 Then
                            oDicItem = New DictionaryEntry(SafeString(pnJuice + 0.5) & " (" & SafeString(pnBetPoint - 10) & ")", "0.5|-10")
                            olstDic.Add(oDicItem)
                        End If

                    End If
                    '' Buy from 1/2 to 1 1/2 points
                    If olstDic.Count = 1 Then
                        Dim nAddpoint As Double
                        For nPoint As Double = 0.5 To 1.5 Step 0.5
                            nAddpoint = nPoint
                            If UCase(psBetType) = "TOTALPOINTS" AndAlso bTotalOver Then
                                nAddpoint = -nPoint
                            End If
                            oDicItem = New DictionaryEntry(SafeString(pnJuice + nAddpoint) & "   " & SafeString(pnBetPoint + (-nPoint * 20)), _
                                                           SafeString(nAddpoint) & "|" & SafeString(-nPoint * 20))
                            olstDic.Add(oDicItem)
                        Next
                    End If
                End If

                If olstDic IsNot Nothing AndAlso olstDic.Count > 0 AndAlso Not String.IsNullOrEmpty(lblOdd.Text) Then
                    ddlBuyPoint.DataSource = olstDic
                    ddlBuyPoint.DataValueField = "Value"
                    ddlBuyPoint.DataTextField = "Key"
                    ddlBuyPoint.DataBind()
                    ddlBuyPoint.Visible = True
                    lblOdd.Visible = False
                End If

            End If

        Catch ex As Exception
            LogError(_log, "loi" & ex.Message, ex)
        End Try

    End Sub
    '' update buypoint for parlay
    Public Sub checkUpdateBuyPoint()
        For i As Integer = 0 To rptMain.Items.Count - 1
            Dim rptBets As Repeater = CType(rptMain.Items(i).FindControl("rptBets"), Repeater)
            For j As Integer = 0 To rptBets.Items.Count - 1
                Dim rptGameLines As Repeater = CType(rptBets.Items(j).FindControl("rptGameLines"), Repeater)
                For k As Integer = 0 To rptGameLines.Items.Count - 1
                    Dim ddlBuyPointHomeTotal As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointHomeTotal"), CDropDownList)
                    Dim ddlBuyPointAwayTotal As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointAwayTotal"), CDropDownList)
                    Dim ddlBuyPointHomeSpread As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointHomeSpread"), CDropDownList)
                    Dim ddlBuyPointAwaySpread As CDropDownList = CType(rptGameLines.Items(k).FindControl("ddlBuyPointAwaySpread"), CDropDownList)

                    Dim chkSelectHomeSpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeSpread"), CheckBox)
                    Dim chkSelectAwaySpread As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwaySpread"), CheckBox)
                    Dim chkSelectHomeTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectHomeTotal"), CheckBox)
                    Dim chkSelectAwayTotal As CheckBox = CType(rptGameLines.Items(k).FindControl("chkSelectAwayTotal"), CheckBox)

                    If chkSelectHomeSpread.Checked Then
                        updateBuyPoint(ddlBuyPointHomeSpread)
                    End If
                    If chkSelectHomeTotal.Checked Then
                        updateBuyPoint(ddlBuyPointHomeTotal)
                    End If
                    If chkSelectAwayTotal.Checked Then
                        updateBuyPoint(ddlBuyPointAwayTotal)
                    End If
                    If chkSelectAwaySpread.Checked Then
                        updateBuyPoint(ddlBuyPointAwaySpread)
                    End If


                Next
            Next
        Next

    End Sub

    Public Sub updateBuyPoint(ByVal ddlBuyPoint As CDropDownList)
        For Each oTicket As CTicket In UserSession.SelectedTicket(SelectedPlayerID).Tickets
            For Each oTicketbet As CTicketBet In oTicket.TicketBets
                If oTicketbet.BuyPointValue.Contains(ddlBuyPoint.ClientID) Then
                    oTicketbet.BuyPointValue = ddlBuyPoint.ClientID & "|" & ddlBuyPoint.SelectedIndex


                End If
                'ClientAlert("aaa", True)
                'oTicketbet.AddPointValid = 1.5
                'oTicketbet.AddPointMoneyValid = -45
            Next
        Next

    End Sub

    Protected Sub btnMainMenu_Click(sender As Object, e As EventArgs) Handles btnMainMenu.Click
        Dim selectedBetTypeActive As String = UCase(BetTypeActive)

        If( (selectedBetTypeActive = "REVERSE" ) OR (selectedBetTypeActive = "IF WIN" ) OR (selectedBetTypeActive = "IF WIN OR PUSH" )) Then
            Response.Redirect("Default.aspx?bettype=IfBetReverse")
        Else 
            Response.Redirect(String.Format("Default.aspx?bettype={0}", BetTypeActive))
        End If
    End Sub
End Class
