Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.Managers
Imports System.Collections
Imports System.Data
Partial Class SBS_Agents_Inc_Game_SelectGame
    Inherits SBCBL.UI.CSBCUserControl
    Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
    Private _SuperBookmakerValue As String = SBCBL.std.GetSiteType & " BookmakerType"
    Private _sOffTimeCategory As String = SBCBL.std.GetSiteType & " LineOffHour"
    Private _bSelectedPropGame As Boolean = False
    Private _bHavePropGame As Boolean = False
    Private _Straight As String = "Straight"
    Private _BetTheBoard As String = "BetTheBoard"
    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private Property Availablegame() As Dictionary(Of String, Boolean)
        Get
            If ViewState("Availablegame") Is Nothing Then
                ViewState("Availablegame") = New Dictionary(Of String, Boolean)
            End If
            Return CType(ViewState("Availablegame"), Dictionary(Of String, Boolean))
        End Get
        Set(ByVal value As Dictionary(Of String, Boolean))
            ViewState("Availablegame") = value
        End Set
    End Property

    Private Property AvailablegamePart() As Dictionary(Of String, Integer())
        Get
            If ViewState("AvailablegamePart") Is Nothing Then
                ViewState("AvailablegamePart") = New Dictionary(Of String, Integer())
            End If
            Return CType(ViewState("AvailablegamePart"), Dictionary(Of String, Integer()))
        End Get
        Set(ByVal value As Dictionary(Of String, Integer()))
            ViewState("AvailablegamePart") = value
        End Set
    End Property

    Public Property BetActionLink() As String
        Get
            Return SafeString(ViewState("BetActionLink"))
        End Get
        Set(ByVal value As String)
            ViewState("BetActionLink") = value
        End Set
    End Property

    Public ReadOnly Property SelectedPlayer() As CPlayer
        Get
            If UserSession.UserType = SBCBL.EUserType.Player Then
                Return UserSession.PlayerUserInfo
            Else
                If Not String.IsNullOrEmpty(SelectedPlayerID) Then
                    Return UserSession.Cache.GetPlayerInfo(SelectedPlayerID)
                Else
                    Return Nothing
                End If

            End If
        End Get
    End Property

    Public ReadOnly Property BetType() As String
        Get
            Return SafeString(Request.QueryString("bettype"))
        End Get
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

    '' set super Agent Id from call center
    Public Property CallCenterAgentID() As String
        Get
            Return SafeString(ViewState("_CALLCENTER_AGENT_ID"))
        End Get
        Set(ByVal value As String)
            ViewState("_CALLCENTER_AGENT_ID") = value
        End Set
    End Property

    'Public ReadOnly Property ContextFilterQuery(ByVal psGameType As String, ByVal psContext As String) As String
    '    Get
    '        Dim sKey = "ContextFilterQuery_" & psGameType & "_" & psContext
    '        If Session(sKey) Is Nothing Then
    '            Session(sKey) = CreateContextFilterQuery(psGameType, psContext)
    '        End If
    '        Return Session(sKey).ToString()
    '    End Get
    'End Property

    Public Property Game() As Dictionary(Of String, CGame)
        Get
            If ViewState("Game") Is Nothing Then
                ViewState("Game") = New Dictionary(Of String, CGame)
            End If
            Return CType(ViewState("Game"), Dictionary(Of String, CGame))
        End Get
        Set(ByVal value As Dictionary(Of String, CGame))

            ViewState("Game") = value
        End Set
    End Property

    Public Property GameSelect() As Dictionary(Of String, CGame)
        Get
            If Session("GameSelect") Is Nothing Then
                Session("GameSelect") = New Dictionary(Of String, CGame)
            End If
            Return CType(Session("GameSelect"), Dictionary(Of String, CGame))
        End Get
        Set(ByVal value As Dictionary(Of String, CGame))

            Session("GameSelect") = value
        End Set
    End Property

    Public ReadOnly Property AgentCategory() As String
        Get
            If UserSession.UserType = SBCBL.EUserType.Agent AndAlso UserSession.AgentUserInfo.IsSuperAgent Then
                Return UserSession.UserID + " SportAllow"
            End If
            Return SuperAgentId + " SportAllow"
        End Get
    End Property

    Public ReadOnly Property AgentgameTypeDisaple() As List(Of String)
        Get
            Dim oMainGameTypeDisable As New List(Of String)
            Dim oMainGameType As List(Of SBCBL.CacheUtils.CSysSetting)
            Dim oCacheManager As New CCacheManager()

            oMainGameType = oCacheManager.GetAllSysSettings(AgentCategory)
            If oMainGameType.Count > 0 Then
                oMainGameTypeDisable = (From oSystemSetting As SBCBL.CacheUtils.CSysSetting In oMainGameType Where oSystemSetting.Value.Equals("No", StringComparison.CurrentCultureIgnoreCase) Select oSystemSetting.Key).ToList
            Else
                oMainGameType = oCacheManager.GetAllSysSettings(_sGameType)
                oMainGameTypeDisable = (From oSystemSetting As SBCBL.CacheUtils.CSysSetting In oMainGameType Where oSystemSetting.Value.Equals("No", StringComparison.CurrentCultureIgnoreCase) Select oSystemSetting.Key).ToList
            End If
            Return oMainGameTypeDisable
        End Get
    End Property

    Public ReadOnly Property SuperAgentId() As String
        Get
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                Return UserSession.AgentUserInfo.SuperAgentID
            ElseIf UserSession.UserType = SBCBL.EUserType.Player Then
                Return UserSession.PlayerUserInfo.SuperAgentID
            Else
                Return CallCenterAgentID
            End If
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then
            If LCase(BetType).Contains("if ") Then
                Dim nIndex As Integer = 1
                If UserSession.SelectedTicket(SelectedPlayerID) IsNot Nothing Then
                    nIndex = UserSession.SelectedTicket(SelectedPlayerID).Tickets.Count + 1
                End If

                ' lblSelectGameMsg.Text = String.Format("Select a sport for the <b>{0}</b> selection of the if-bet.", GetOrdinalNumber(nIndex))
            Else
                'lblSelectGameMsg.Text = String.Format("Select one or more sports for your {0}.", BetType.Replace("BetTheBoard", "Straight Bet(s)"))
            End If

            Session("BetTypeProp") = False
            ' lblBetType.Text = BetType.Replace("Reverse", "Action Reverse").Replace("BetTheBoard", "Straight Bet(s)")
            If UCase(BetType) = "TEASER" Then
                TeaserType.Visible = True
                pnSelectGame.Visible = False '.Style.Add("display", "none")
                dvPropGame.Visible = False
            ElseIf UCase(BetType) = "PROP" Then
                TeaserType.Visible = False
                pnSelectGame.Visible = False '.Style.Add("display", "none")
                dvPropGame.Visible = True
            Else
                TeaserType.Visible = False
                pnSelectGame.Visible = True '.Style.Add("display", "")
                dvPropGame.Visible = False
            End If

            ' bindSuperInfo()
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                BindSubAgents()
                BindPlayers()
                dvPlayer.Visible = True
                GameSelect.Clear()
                lbtAgentContinue.Text = "<a href='#' onclick='javascript:CheckEmptyUser(this)' >Continue</a>"
                btnContinue.Visible = False
            Else
                lbtAgentContinue.Visible = False
                btnContinue.Visible = True
            End If

            ' If Not String.IsNullOrEmpty(SelectedPlayerID) Then
            'Dim oCacheManager As CCacheManager = New CCacheManager()
            'DisplayInfo()
            ' ltlLoginID.Text = oCacheManager.GetPlayerInfo(SelectedPlayerID).Login
            ' ltlBalance.Text = FormatNumber(oCacheManager.GetPlayerInfo(SelectedPlayerID).BalanceAmount, SBCBL.std.GetRoundMidPoint())
            ' End If

            'If BetType.Equals(_BetTheBoard, StringComparison.CurrentCultureIgnoreCase) Then
            '    bettypeTitle.Visible = False
            '    lblBetTheBoard.Text = "Display the board by selecting one or more sports below.."
            'End If


            bindGameTypes()




        End If

    End Sub

    'Public Sub DisplayInfo()
    '    If Not String.IsNullOrEmpty(SelectedPlayerID) Then
    '        Dim oCacheManager As CCacheManager = New CCacheManager()
    '        ltlBalance.Text = FormatNumber(oCacheManager.GetPlayerInfo(SelectedPlayerID).BalanceAmount, SBCBL.std.GetRoundMidPoint())
    '        ltlCreditLimit.Text = FormatNumber(oCacheManager.GetPlayerInfo(SelectedPlayerID).Template.CreditMaxAmount, SBCBL.std.GetRoundMidPoint())
    '        ltlPendingAmount.Text = FormatNumber(oCacheManager.GetPlayerInfo(SelectedPlayerID).PendingAmount, SBCBL.std.GetRoundMidPoint())

    '    End If
    'End Sub

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
        'If Not String.IsNullOrEmpty(SafeString(odtSuperAdmin.Rows(0)("Wagering"))) Then
        '    lblWagering.Text = " <strong>Wagering #:</strong> " & SafeString(odtSuperAdmin.Rows(0)("Wagering")) & "<br/>"
        'End If

        'If Not String.IsNullOrEmpty(SafeString(odtSuperAdmin.Rows(0)("CustomerService"))) Then
        '    lblCustomerService.Text = " <strong>Customer Service #:</strong>" & SafeString(odtSuperAdmin.Rows(0)("CustomerService"))
        'End If

    End Sub

    Private Sub BindSubAgents()

        Dim oMng As New SBCBL.Managers.CAgentManager()
        ddlSubAgents.DataSource = oMng.GetAgentsByAgentID(UserSession.UserID, Nothing)
        ddlSubAgents.DataValueField = "AgentID"
        ddlSubAgents.DataTextField = "FullName"
        ddlSubAgents.DataBind()
        wager.Style.Add("padding-top", "25px")
        dvSubAgents.Visible = ddlSubAgents.Items.Count > 1
    End Sub

    <System.Web.Services.WebMethod()> _
    Private Sub BindPlayers()

        Dim oMng As New SBCBL.Managers.CPlayerManager()
        If ddlSubAgents.Value <> "" Then
            ddlPlayers.DataSource = oMng.GetPlayers(ddlSubAgents.Value, Nothing)
        Else
            ddlPlayers.DataSource = oMng.GetPlayers(UserSession.UserID, Nothing)
        End If

        ddlPlayers.DataValueField = "PlayerID"
        ddlPlayers.DataTextField = "FullName"
        ddlPlayers.DataBind()
    End Sub

    Protected Sub ddlSubAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSubAgents.SelectedIndexChanged
        BindPlayers()
    End Sub

#Region "Game Types"

    Private Sub bindGameTypes()
        'Dim oGameTypes As List(Of CSysSetting) = SBCBL.Utils.CSerializedObjectCloner.Clone(Of List(Of CSysSetting))((From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
        '                                          Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And oSetting.Key <> "Proposition" _
        '                                          Order By oSetting.Key _
        '                                          Select oSetting).ToList())

        'Dim oGameTypesLeft As New List(Of CSysSetting)
        'Dim lstGameTypes As New List(Of CSysSetting)
        'lstGameTypes.Add(oGameTypes.Item(2))
        'lstGameTypes.Add(oGameTypes.Item(1))
        'lstGameTypes.Add(oGameTypes.Item(0))
        'lstGameTypes.Add(oGameTypes.Item(3))
        'lstGameTypes.Add(oGameTypes.Item(5))
        'lstGameTypes.Add(oGameTypes.Item(4))
        Dim lstSportTypes As New List(Of String)
        lstSportTypes.Add("Live Game")
        lstSportTypes.Add("Football")
        lstSportTypes.Add("Basketball")
        lstSportTypes.Add("Baseball")
        lstSportTypes.Add("Hockey")
        lstSportTypes.Add("Soccer")
        lstSportTypes.Add("Other Sports")
        'lstSportTypes.Add("Baseball")



        Dim lstGameType As New List(Of String)
        'For Each osys As String In lstSportTypes  'oGameTypes
        '    'Dim sSysSettingID As String = osys.SysSettingID
        '    'Dim oListSubSys As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
        '    '                                       Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = sSysSettingID _
        '    '                                       And Not AgentgameTypeDisaple.Contains(oSetting.Key) _
        '    '                                       Order By oSetting.Key _
        '    '                                       Select oSetting).ToList()
        '    For Each oSysetting As CSysSetting In oListSubSys
        '        lstGameType.Add(oSysetting.Key)
        '    Next
        'Next
        lstGameType.AddRange(getListGameType("Live Game"))
        lstGameType.AddRange(getListGameType("Football"))
        lstGameType.AddRange(getListGameType("Basketball"))
        lstGameType.AddRange(getListGameType("Baseball"))
        lstGameType.AddRange(getListGameType("Hockey"))
        Dim lstSoccer As List(Of String) = getListGameType("Soccer")
        lstSoccer.Sort()
        lstGameType.AddRange(lstSoccer)
        lstGameType.AddRange(getListGameType("Other Sports"))

        Dim odtAvailablegame As DataTable = GetAvailableGameType(lstGameType)
        If odtAvailablegame IsNot Nothing Then
            For Each odr As DataRow In odtAvailablegame.Rows
                Dim oContext As Integer() = {SafeInteger(odr("FullGame")), SafeInteger(odr("FirstHalf")), SafeInteger(odr("SecondHalf")), SafeInteger(odr("Quarter"))}
                AvailablegamePart(SafeString(odr("GameType"))) = oContext
                If SafeInteger(odr("FullGame")) > 0 OrElse SafeInteger(odr("FirstHalf")) > 0 OrElse SafeInteger(odr("SecondHalf")) > 0 OrElse SafeInteger(odr("Quarter")) > 0 Then
                    Availablegame(SafeString(odr("GameType"))) = True
                Else
                    Availablegame(SafeString(odr("GameType"))) = False
                End If

            Next
        End If
        ''end
        rptGameType.DataSource = lstSportTypes ' lstGameTypes
        rptGameType.DataBind()


        'liProp.Visible = oGameTypes.Count > 0

        ' Dim oSysManager As New CSysSettingManager()
        Dim oGameTypes As List(Of CSysSetting)
        Dim oSysProp = UserSession.Cache.GetAllSysSettings(_sGameType).Find(Function(x) x.Key = "Proposition")
        Dim lstPropGameType As New List(Of String)
        If oSysProp IsNot Nothing Then
            oGameTypes = UserSession.Cache.GetAllSysSettings(_sGameType).FindAll(Function(x) x.SubCategory = oSysProp.SysSettingID And UCase(x.Value) = "YES" And Not AgentgameTypeDisaple.Contains(x.Key))
            lstPropGameType = (From oGameType In oGameTypes Select oGameType.Key).ToList()
            Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetGameTypeAvailableProps(GetEasternDate(), lstPropGameType)
            Dim lstPropgamTypeAvailable As New List(Of String)
            If odtGames IsNot Nothing Then


                For Each oRow As DataRow In odtGames.Rows

                    lstPropgamTypeAvailable.Add(SafeString(oRow("GameType")))
                Next
                rptPropGame.DataSource = lstPropgamTypeAvailable
                rptPropGame.DataBind()
            End If
        End If
    End Sub


    '''''''''''''''''''''''''start check game have gameLine''''''''''''''''''''''''''''''''''''''''''''''''

    '' this create filter to avoide weird line
    'Private Function CreateContextFilterQuery(ByVal psGameType As String, ByVal psContext As String) As String
    '    Dim sCategory As String = SBCBL.std.GetSiteType & " LineRules"
    '    ' Dim oSysManager As New CSysSettingManager()
    '    Dim oCachemanager As New CCacheManager()
    '    Dim oListWhere As New List(Of String)
    '    Dim oSysGame As CSysSetting = oCachemanager.GetAllSysSettings(_sGameType).Find(Function(x) x.Key = psGameType AndAlso x.SubCategory <> "")
    '    Dim oSysSport As CSysSetting = oCachemanager.GetSysSettings(_sGameType).Find(Function(x) x.SysSettingID = oSysGame.SubCategory)

    '    Dim sGameTypeKey As String = oSysSport.Key.Replace(" ", "_")
    '    Dim sSubCategory As String = sGameTypeKey & "_" & psContext

    '    'Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
    '    '                                    Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" _
    '    '                                    Order By oSetting.SubCategory, oSetting.Key _
    '    '                                    Select oSetting).ToList

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
            Return GetBookMakerType(psGameType, UserSession.PlayerUserInfo.AgentID, SafeString(UserSession.PlayerUserInfo.AgentID & "_BookmakerType"))
        End If
        If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
            Return GetBookMakerType(psGameType, UserSession.Cache.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID).AgentID, SafeString(UserSession.Cache.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID).AgentID & "_BookmakerType"))

        End If

        Return ""
    End Function
    Private Shared Function SortGameType( _
        ByVal x As String, ByVal y As String) As Boolean

        Return String.Compare(x, y)

    End Function

    Protected Sub rptGameType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptGameType.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            'Dim rptSubGameTypeParent As Repeater = CType(e.Item.FindControl("rptSubGameTypeParent"), Repeater)

            ''Dim oSys As CSysSetting = CType(e.Item.DataItem, CSysSetting)
            '

            'Dim oListSubSys As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                    Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = oSys.SysSettingID _
            '                                    And Not AgentgameTypeDisaple.Contains(oSetting.Key) _
            '                                    Order By oSetting.Key _
            '                                    Select oSetting).ToList()

            ' ''list contain all game for one gameType
            'Dim listSubSystem As New List(Of CSysSetting)
            'For Each oSystem As CSysSetting In oListSubSys
            '    ' If Availablegame.ContainsKey(oSystem.Key) AndAlso Availablegame(oSystem.Key) Then
            '    listSubSystem.Add(oSystem)
            '    '  End If

            'Next
            '' GetAvailableGameType(lstGameType)
            ' '' oListDevides is an array of List(Of CSysSetting), we devide sub menu to many columns
            'Dim oListDevides(listSubSystem.Count \ 4) As List(Of CSysSetting)
            'For nIndex As Integer = 0 To listSubSystem.Count - 1
            '    If oListDevides(nIndex \ 4) Is Nothing Then
            '        oListDevides(nIndex \ 4) = New List(Of CSysSetting)()
            '    End If

            '    oListDevides(nIndex \ 4).Add(listSubSystem(nIndex))
            'Next

            ''If listSubSystem.Count = 0 Then
            ''    CType(e.Item.FindControl("ltlNotice"), Literal).Text = "No Game"
            ''End If
            ''Dim imgSoccer = CType(e.Item.FindControl("imgSoccer"), Image)
            ''imgSoccer.ImageUrl = "/Images/Soccer/"
            ''imgSoccer.Visible = False

            'rptSubGameTypeParent.DataSource = oListDevides
            'rptSubGameTypeParent.DataBind()

            Dim sSportType As String = CType(e.Item.DataItem, String)

            'Dim rptSubGameTypeParent As Repeater = CType(e.Item.FindControl("rptSubGameTypeParent"), Repeater)
            'rptSubGameTypeParent.DataSource = lstGameType
            'rptSubGameTypeParent.DataBind()
            Dim lstGameType As List(Of String) = getListGameType(sSportType)
            For Each StrGameDis As String In AgentgameTypeDisaple
                If lstGameType.Contains(StrGameDis) Then
                    lstGameType.Remove(StrGameDis)
                End If
            Next

            Dim rptSubGameType As Repeater = CType(e.Item.FindControl("rptSubGameType"), Repeater)
            If sSportType.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
                lstGameType.Sort()
            End If
            rptSubGameType.DataSource = lstGameType
            rptSubGameType.DataBind()

        End If
    End Sub



    'Protected Sub rptSubGameTypeParent_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
    '    'Dim rptSubGameType As Repeater = CType(e.Item.FindControl("rptSubGameType"), Repeater)
    '    'rptSubGameType.DataSource = e.Item.DataItem
    '    'rptSubGameType.DataBind()
    '    Dim lstSportType As New List(Of String)
    '    lstSportType.Add(e.Item.DataItem)
    '    Dim rptSubGameType As Repeater = CType(e.Item.FindControl("rptSubGameType"), Repeater)
    '    rptSubGameType.DataSource = lstSportType
    '    rptSubGameType.DataBind()
    'End Sub

    Protected Sub rptSubGameType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' Dim sGameType As String = CType(e.Item.DataItem, CSysSetting).Key
            Dim sGameType As String = SafeString(e.Item.DataItem)
            'Dim imgSoccer = CType(e.Item.FindControl("imgSoccer"), Image)
            'If IsSoccer(sGameType) Then
            '    imgSoccer.Visible = True
            '    imgSoccer.ImageUrl = "/Images/Soccer/" & sGameType & ".png"
            'Else
            '    imgSoccer.Visible = False
            'End If
            If IsBaseball(sGameType) Then
                CType(e.Item.FindControl("lblOneH"), Label).Text = "1st 5 innings"
                ' CType(e.Item.FindControl("lblTwoH"), Label).Text = "2st 5 innings"
            Else
                CType(e.Item.FindControl("lblOneH"), Label).Text = "1st Half"
                CType(e.Item.FindControl("lblTwoH"), Label).Text = "2nd Half"
            End If
            Dim sBookmaker As String = UserSession.SysSettings(GetAgentBookMaker(sGameType)).GetValue(sGameType)
            Dim chkOneH, chkTwoH, chkCurrent, chkQuarter1, chkQuarter2, chkQuarter3, chkQuarter4 As CheckBox
            Dim liOneH, liTwoH, liQuarter1, liQuarter2, liQuarter3, liQuarter4 As HtmlControl

            liOneH = CType(e.Item.FindControl("liOneH"), HtmlControl)
            liTwoH = CType(e.Item.FindControl("liTwoH"), HtmlControl)
            'liCurrent = CType(e.Item.FindControl("liCurrent"), HtmlControl)
            liQuarter1 = CType(e.Item.FindControl("liQuarter1"), HtmlControl)
            'liQuarter2 = CType(e.Item.FindControl("liQuarter2"), HtmlControl)
            'liQuarter3 = CType(e.Item.FindControl("liQuarter3"), HtmlControl)
            'liQuarter4 = CType(e.Item.FindControl("liQuarter4"), HtmlControl)

            liQuarter1.Visible = False
            'liQuarter2.Visible = False
            'liQuarter3.Visible = False
            'liQuarter4.Visible = False
            liOneH.Visible = False
            liTwoH.Visible = False
            'liCurrent.Visible = False

            Dim lblOneH, lblTwoH, lblQuarter1, lblQuarter2, lblQuarter3, lblQuarter4, lblGameType As Label
            Dim lblGame As Literal
            'Dim oGame As CGame
            'oGame = CType(Game(sGameType & "_GAMEPART_" & sBookmaker), CGame)
            chkQuarter1 = CType(e.Item.FindControl("chkQuarter1"), CheckBox)
            'chkQuarter2 = CType(e.Item.FindControl("chkQuarter2"), CheckBox)
            'chkQuarter3 = CType(e.Item.FindControl("chkQuarter3"), CheckBox)
            'chkQuarter4 = CType(e.Item.FindControl("chkQuarter4"), CheckBox)
            chkOneH = CType(e.Item.FindControl("chkOneH"), CheckBox)
            chkTwoH = CType(e.Item.FindControl("chkTwoH"), CheckBox)
            chkCurrent = CType(e.Item.FindControl("chkCurrent"), CheckBox)
            lblOneH = CType(e.Item.FindControl("lblOneH"), Label)
            lblTwoH = CType(e.Item.FindControl("lblTwoH"), Label)
            lblGameType = CType(e.Item.FindControl("lblGameType"), Label)
            'lblCurrent = CType(e.Item.FindControl("lblCurrent"), Label)
            lblQuarter1 = CType(e.Item.FindControl("lblQuarter1"), Label)
            lblQuarter2 = CType(e.Item.FindControl("lblQuarter2"), Label)
            lblQuarter3 = CType(e.Item.FindControl("lblQuarter3"), Label)
            lblQuarter4 = CType(e.Item.FindControl("lblQuarter4"), Label)
            lblGame = CType(e.Item.FindControl("lblGame"), Literal)
            chkOneH.Visible = False
            chkTwoH.Visible = False
            lblOneH.Visible = False
            lblTwoH.Visible = False
            'chkCurrent.Visible = False
            ' lblCurrent.Visible = False
            lblQuarter1.Visible = False
            'lblQuarter2.Visible = False
            'lblQuarter3.Visible = False
            'lblQuarter4.Visible = False
            chkQuarter1.Visible = False
            'chkQuarter2.Visible = False
            'chkQuarter3.Visible = False
            'chkQuarter4.Visible = False

            lblOneH.CssClass = ""
            lblTwoH.CssClass = ""
            lblGameType.CssClass = ""
            lblQuarter1.CssClass = ""
            'lblQuarter2.CssClass = ""
            'lblQuarter3.CssClass = ""
            'lblQuarter4.CssClass = ""
            Dim oGameHalfTimeOff As CGameHalfTimeOFF = UserSession.Cache.GetGameHalfTimeOFF(SuperAgentId).GetGameHalfTimeOff(sGameType)
            If oGameHalfTimeOff IsNot Nothing Then
                'LogDebug(_log, ":" + sGameType + "--" + AvailablegamePart.Keys.Count().ToString())
                If AvailablegamePart.ContainsKey(sGameType) AndAlso Not oGameHalfTimeOff.FirstHOff AndAlso AvailablegamePart(sGameType)(1) And Not UserSession.FirstHalfOff(sGameType) Then
                    chkOneH.Visible = True
                    lblOneH.Visible = True
                    liOneH.Visible = True
                    lblOneH.ForeColor = Drawing.Color.Blue
                    'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                    '    lblOneH.CssClass = "underline"
                    'End If

                End If
                If AvailablegamePart.ContainsKey(sGameType) AndAlso Not oGameHalfTimeOff.SecondHOff AndAlso AvailablegamePart(sGameType)(2) And Not UserSession.IsSecondhalfOff(sGameType) Then
                    chkTwoH.Visible = True
                    lblTwoH.Visible = True
                    'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                    '    lblTwoH.CssClass = "underline"
                    'End If
                    liTwoH.Visible = True
                    lblTwoH.ForeColor = Drawing.Color.Blue
                End If
            Else
                ' LogDebug(_log, ":" + sGameType + "-d-" + AvailablegamePart.Keys.Count().ToString())
                If AvailablegamePart.ContainsKey(sGameType) AndAlso AvailablegamePart(sGameType)(1) Then
                    chkOneH.Visible = True
                    lblOneH.Visible = True
                    'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                    '    lblOneH.CssClass = "underline"
                    'End If
                    liOneH.Visible = True
                    lblOneH.ForeColor = Drawing.Color.Blue
                End If
                If AvailablegamePart.ContainsKey(sGameType) AndAlso AvailablegamePart(sGameType)(2) Then
                    chkTwoH.Visible = True
                    lblTwoH.Visible = True
                    'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                    '    lblTwoH.CssClass = "underline"
                    'End If
                    liTwoH.Visible = True
                    lblTwoH.ForeColor = Drawing.Color.Blue
                End If
            End If

            If AvailablegamePart.ContainsKey(sGameType) AndAlso AvailablegamePart(sGameType)(0) Then
                chkCurrent.Visible = True
                'If BetType.Equals(_BetTheBoard) Then
                '    lblGame.Text = "<span > (Game)</span>"
                'End If
                ''lblCurrent.Visible = True
                'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                '    lblGameType.CssClass = "underline"
                'End If
                lblGameType.ForeColor = Drawing.Color.Blue
            End If

            If AvailablegamePart.ContainsKey(sGameType) AndAlso AvailablegamePart(sGameType)(3) Then
                chkQuarter1.Visible = True
                lblQuarter1.Visible = True
                'chkQuarter2.Visible = True
                'lblQuarter2.Visible = True
                'chkQuarter3.Visible = True
                'lblQuarter3.Visible = True
                'chkQuarter4.Visible = True
                'lblQuarter4.Visible = True
                'If BetType.Equals("Straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
                '    lblQuarter1.CssClass = "underline"
                '    lblQuarter2.CssClass = "underline"
                '    lblQuarter3.CssClass = "underline"
                '    lblQuarter4.CssClass = "underline"
                'End If
                liQuarter1.Visible = True
                'liQuarter2.Visible = True
                'liQuarter3.Visible = True
                'liQuarter4.Visible = True
                lblQuarter1.ForeColor = Drawing.Color.Blue
                lblQuarter2.ForeColor = Drawing.Color.Blue
                lblQuarter3.ForeColor = Drawing.Color.Blue
                lblQuarter4.ForeColor = Drawing.Color.Blue
            End If
            'If BetType.Equals("straight", StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ") Then
            '    'chkOneH.Visible = False
            '    'chkTwoH.Visible = False
            '    'chkCurrent.Visible = False
            '    'chkQuarter1.Visible = False
            '    'chkQuarter2.Visible = False
            '    'chkQuarter3.Visible = False
            '    'chkQuarter4.Visible = False
            '    liQuarter1.Visible = False
            '    liQuarter2.Visible = False
            '    liQuarter3.Visible = False
            '    liQuarter4.Visible = False


            'End If

            'liQuarter1.Style.Add("display", "none")
            'liQuarter2.Style.Add("display", "none")
            'liQuarter3.Style.Add("display", "none")
            'liQuarter4.Style.Add("display", "none")
            'liOneH.Style.Add("display", "none")
            'liTwoH.Style.Add("display", "none")
        End If


    End Sub



    '' <summary>
    '' Set Betting mode is Single Game or Proposition</summary>
    '' <param name="poBetMode">poBetMode = true : Single Game, otherwise is Proposition</param>
    '' <remarks></remarks>


    'Protected Sub rptPropGame_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPropGame.ItemDataBound
    '    If e.Item.ItemType = ListItemType.Footer Or e.Item.ItemType = ListItemType.Header Then
    '        ltlNotice.Visible = Not _bHavePropGame
    '        Return
    '    End If
    '    Dim sGameType As String = SafeString(e.Item.DataItem)
    '    Dim lblPropType As Label = CType(e.Item.FindControl("lblPropType"), Label)
    '    Dim chkPropType As CheckBox = CType(e.Item.FindControl("chkPropType"), CheckBox)
    '    'Dim oToday = GetEasternDate()
    '    ' Dim oSysManager As New CSysSettingManager()
    '    'Dim sGameType As String = lblPropType.Text
    '    'Dim odtGames As DataTable = New SBCBL.Managers.CGameManager().GetAvailableProps(sGameType, oToday)
    '    '       chkPropType.Visible = odtGames.Rows.Count > 0
    '    '        lblPropType.Visible = odtGames.Rows.Count > 0

    '    'If Not odtGames.Rows.Count > 0 Then
    '    _bHavePropGame = True
    '    chkPropType.Checked = False
    '    sGameType = "Prop_" & lblPropType.Text
    '    SetSelectedGame(sGameType, False)
    '    ' End If

    'End Sub

#End Region
    Protected Sub lblGameType_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not (BetType.Equals(_Straight, StringComparison.CurrentCultureIgnoreCase) OrElse LCase(BetType).Contains("if ")) Then
            Return
        End If
        If UserSession.UserType = SBCBL.EUserType.Agent Then
            SelectedPlayerID = ddlPlayers.SelectedValue
        End If
        UserSession.SelectedBetType(Me.SelectedPlayerID) = "SINGLE"

        Session("BetTypeActive") = BetType
        If Not String.IsNullOrEmpty(BetActionLink) AndAlso Not String.IsNullOrEmpty(Me.SelectedPlayerID) Then
            ''clear all game
            UserSession.SelectedGameTypes(Me.SelectedPlayerID).Clear()
            ''get game checked
            GameSelect.Clear()
            'For Each oGameTypeItem As RepeaterItem In rptGameType.Items
            '    Dim rptSubGameType As Repeater = CType(oGameTypeItem.FindControl("rptSubGameType"), Repeater)
            '    For Each oSubGameTypeItem As RepeaterItem In rptSubGameType.Items
            '        Dim chkOneH, chkTwoH, chkCurrent, chkQuarter As CheckBox
            '        chkOneH = CType(oSubGameTypeItem.FindControl("chkOneH"), CheckBox)
            '        chkTwoH = CType(oSubGameTypeItem.FindControl("chkTwoH"), CheckBox)
            '        chkCurrent = CType(oSubGameTypeItem.FindControl("chkCurrent"), CheckBox)
            '        chkQuarter = CType(rptSubGameType.Items(k).FindControl("chkQuarter"), CheckBox)

            '    Next

            'Next
            ''get game checked
            '  SelectBetType()

            'Dim oGame As New CGame()
            'oGame.Current = True
            'oGame.OneH = True
            'oGame.TwoH = True
            'oGame.Quarter1 = True
            'oGame.Quarter2 = True
            'oGame.Quarter3 = True
            'oGame.Quarter4 = True
            'oGame.GameType = CType(sender, Button).Text
            'GameSelect(oGame.GameType) = oGame
            'UserSession.SelectedGameTypes(Me.SelectedPlayerID).Add(oGame.GameType)
            Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
            If olstGameType Is Nothing OrElse olstGameType.Count = 0 Then
                ClientAlert("Please Select Game")
            Else
                Response.Redirect(BetActionLink & String.Format("?PlayerID={0}", Me.SelectedPlayerID))
            End If

        Else
            ClientAlert("Please Select A Player")
        End If
    End Sub

    Private Sub GetTeaser(ByVal psTeaserSelect As String)
        Dim oCache As New CCacheManager()
        For Each oTeaserRule As CTeaserRule In oCache.GetTeaserRules()
            '   oDicItem = New DictionaryEntry(oTeaserRule.TeaserRuleID, oTeaserRule.TeaserRuleName)
            If psTeaserSelect.Equals(oTeaserRule.BasketballPoint & "/" & oTeaserRule.FootbalPoint) Then
                Session("TeaserValue") = oTeaserRule
                Return
            End If
        Next
    End Sub

    Protected Sub Teaser_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        If UserSession.UserType = SBCBL.EUserType.Agent Then
            SelectedPlayerID = ddlPlayers.SelectedValue
        End If
        GetTeaser(CType(sender, LinkButton).CommandArgument)
        Session("BetTypeActive") = BetType
        If Not String.IsNullOrEmpty(BetActionLink) AndAlso Not String.IsNullOrEmpty(Me.SelectedPlayerID) Then
            ''clear all game
            UserSession.SelectedGameTypes(Me.SelectedPlayerID).Clear()
            ''get game checked
            GetSelectTeaserGame(rptGameType)
            Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
            If olstGameType Is Nothing OrElse olstGameType.Count = 0 Then
                ClientAlert("Please Select Game1")
            Else
                Response.Redirect(BetActionLink & String.Format("?PlayerID={0}", Me.SelectedPlayerID))
            End If

        Else
            ClientAlert("Please Select A Player")
        End If

    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If UserSession.UserType = SBCBL.EUserType.Agent Then
            SelectedPlayerID = ddlPlayers.SelectedValue
        End If
        UserSession.SelectedBetType(Me.SelectedPlayerID) = "SINGLE"
        Session("BetTypeActive") = BetType
        If Not String.IsNullOrEmpty(BetActionLink) AndAlso Not String.IsNullOrEmpty(Me.SelectedPlayerID) Then
            ''clear all game
            UserSession.SelectedGameTypes(Me.SelectedPlayerID).Clear()
            ''get game checked
            _log.Error("goi")
            SelectBetType()

            Dim olt As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
            _log.Debug("xxx" + olt.Count.ToString())
            Dim olstGameType As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
            '_log.Debug("sasdasdsa" + olstGameType.Count)

            If olstGameType Is Nothing OrElse olstGameType.Count = 0 Then
                ClientAlert("Please Select Game2")
            Else
                Response.Redirect(BetActionLink & String.Format("?PlayerID={0}", Me.SelectedPlayerID))
            End If

        Else
            ClientAlert("Please Select A Player")
        End If

    End Sub

    'Protected Sub chkPropType_Click(ByVal sender As Object, ByVal e As System.EventArgs)
    '    CheckSelectPropGame(rptPropGame)
    'End Sub

    Protected Sub lbtPropType_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session("BetTypeProp") = True
        UserSession.SelectedGameTypes(SelectedPlayerID).Clear()
        SetSelectedGame("Prop_" + sender.Text, True)
        UserSession.SelectedBetType(Me.SelectedPlayerID) = IIf(True, "PROP", "SINGLE").ToString()
        Session("BetTypeActive") = _BetTheBoard
        Response.Redirect(BetActionLink & String.Format("?PlayerID={0}", Me.SelectedPlayerID))
    End Sub


    Public Sub GetSelectGame(ByVal rptGameType As Repeater)
        ' For i As Integer = 0 To rptGameType.Items.Count - 1
        '  Dim rptSubGameTypeParent As Repeater = CType(rptGameType.Items(i).FindControl("rptSubGameTypeParent"), Repeater)
        Dim sGameType As String
        For j As Integer = 0 To rptGameType.Items.Count - 1
            _log.Debug(j)
            Dim rptSubGameType As Repeater = CType(rptGameType.Items(j).FindControl("rptSubGameType"), Repeater)
            For k As Integer = 0 To rptSubGameType.Items.Count - 1
                'Dim chkGameType As CheckBox = CType(rptSubGameType.Items(k).FindControl("chkGameType"), CheckBox)
                Dim chkOneH, chkTwoH, chkCurrent, chkQuarter1, chkQuarter2, chkQuarter3, chkQuarter4 As CheckBox
                chkOneH = CType(rptSubGameType.Items(k).FindControl("chkOneH"), CheckBox)
                chkTwoH = CType(rptSubGameType.Items(k).FindControl("chkTwoH"), CheckBox)
                chkCurrent = CType(rptSubGameType.Items(k).FindControl("chkCurrent"), CheckBox)
                chkQuarter1 = CType(rptSubGameType.Items(k).FindControl("chkQuarter1"), CheckBox)
                'chkQuarter2 = CType(rptSubGameType.Items(k).FindControl("chkQuarter2"), CheckBox)
                'chkQuarter3 = CType(rptSubGameType.Items(k).FindControl("chkQuarter3"), CheckBox)
                'chkQuarter4 = CType(rptSubGameType.Items(k).FindControl("chkQuarter4"), CheckBox)
                Dim lblGameType As Label = CType(rptSubGameType.Items(k).FindControl("lblGameType"), Label)
                Dim btnGameType = CType(rptSubGameType.Items(k).FindControl("btnGameType"), Button)
                'If chkGameType.Checked Then

                '' save check 1h 2h current to session after sent to Betaction
                Dim oGame As New CGame()
                If (chkOneH.Checked) Then
                    oGame.OneH = True
                End If
                If (chkTwoH.Checked) Then
                    oGame.TwoH = True
                End If
                If chkCurrent.Checked Then
                    oGame.Current = True
                End If
                If chkQuarter1.Checked Then
                    oGame.Quarter1 = True
                    oGame.Quarter2 = True
                    oGame.Quarter3 = True
                    oGame.Quarter4 = True
                End If
                'If chkQuarter2.Checked Then
                '    oGame.Quarter2 = True
                'End If
                'If chkQuarter3.Checked Then
                '    oGame.Quarter3 = True
                'End If
                'If chkQuarter4.Checked Then
                '    oGame.Quarter4 = True
                'End If

                sGameType = btnGameType.Text
                oGame.GameType = sGameType
                If GameSelect.ContainsKey(sGameType) Then
                    GameSelect.Remove(sGameType)
                End If
                GameSelect.Add(sGameType, oGame)
                _log.Debug(sGameType + "--" + SafeString(chkCurrent.Checked))
                If chkOneH.Checked OrElse chkTwoH.Checked OrElse chkCurrent.Checked OrElse chkQuarter1.Checked Then
                    SetSelectedGame(sGameType, True)
                Else
                    SetSelectedGame(sGameType, False)
                End If
                Dim olt As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
                _log.Debug(sGameType & olt.Count.ToString())
            Next
        Next
        'Next

    End Sub

    Public Sub GetSelectTeaserGame(ByVal rptGameType As Repeater)
        For i As Integer = 0 To rptGameType.Items.Count - 1
            'Dim rptSubGameTypeParent As Repeater = CType(rptGameType.Items(i).FindControl("rptSubGameTypeParent"), Repeater)
            Dim sGameType As String
            For j As Integer = 0 To rptGameType.Items.Count - 1
                Dim rptSubGameType As Repeater = CType(rptGameType.Items(j).FindControl("rptSubGameType"), Repeater)
                For k As Integer = 0 To rptSubGameType.Items.Count - 1
                    'Dim chkGameType As CheckBox = CType(rptSubGameType.Items(k).FindControl("chkGameType"), CheckBox)
                    Dim chkOneH, chkTwoH, chkCurrent, chkQuarter As CheckBox
                    chkOneH = CType(rptSubGameType.Items(k).FindControl("chkOneH"), CheckBox)
                    chkTwoH = CType(rptSubGameType.Items(k).FindControl("chkTwoH"), CheckBox)
                    chkCurrent = CType(rptSubGameType.Items(k).FindControl("chkCurrent"), CheckBox)
                    chkQuarter = CType(rptSubGameType.Items(k).FindControl("chkQuarter"), CheckBox)
                    Dim btnGameType = CType(rptSubGameType.Items(k).FindControl("btnGameType"), Button)
                    Dim lblGameType As Label = CType(rptSubGameType.Items(k).FindControl("lblGameType"), Label)
                    'If chkGameType.Checked Then
                    sGameType = btnGameType.Text
                    _log.Debug(sGameType)
                    '' save check 1h 2h current to session after sent to Betaction
                    If (AvailablegamePart.ContainsKey(sGameType) AndAlso AvailablegamePart(sGameType)(0)) AndAlso (IsFootball(sGameType) OrElse IsBasketball(sGameType)) Then
                        Dim oGame As New CGame()
                        oGame.Current = True
                        oGame.GameType = sGameType
                        If GameSelect.ContainsKey(sGameType) Then
                            GameSelect.Remove(sGameType)
                            SetSelectedGame(sGameType, False)
                        End If
                        GameSelect.Add(sGameType, oGame)
                        SetSelectedGame(sGameType, True)
                    End If
                Next
            Next
        Next
    End Sub


    'Private Sub SetPageBetMode(ByVal rptGameType As Repeater, ByVal bbetMode As Boolean)
    '    For i As Integer = 0 To rptGameType.Items.Count - 1
    '        Dim rptSubGameTypeParent As Repeater = CType(rptGameType.Items(i).FindControl("rptSubGameTypeParent"), Repeater)

    '        For j As Integer = 0 To rptSubGameTypeParent.Items.Count - 1
    '            Dim rptSubGameType As Repeater = CType(rptSubGameTypeParent.Items(j).FindControl("rptSubGameType"), Repeater)
    '            For k As Integer = 0 To rptSubGameType.Items.Count - 1
    '                'Dim chkGameType As CheckBox = CType(rptSubGameType.Items(k).FindControl("chkGameType"), CheckBox)
    '                'Dim lblGameType As Label = CType(rptSubGameType.Items(k).FindControl("lblGameType"), Label)
    '                Dim chkOneH, chkTwoH, chkCurrent, chkQuarter As CheckBox
    '                chkOneH = CType(rptSubGameType.Items(k).FindControl("chkOneH"), CheckBox)
    '                chkTwoH = CType(rptSubGameType.Items(k).FindControl("chkTwoH"), CheckBox)
    '                chkCurrent = CType(rptSubGameType.Items(k).FindControl("chkCurrent"), CheckBox)
    '                chkQuarter = CType(rptSubGameType.Items(k).FindControl("chkQuarter"), CheckBox)
    '                chkOneH.Enabled = bbetMode
    '                chkOneH.Checked = False
    '                chkTwoH.Enabled = bbetMode
    '                chkTwoH.Checked = False
    '                chkCurrent.Enabled = bbetMode
    '                chkCurrent.Checked = False
    '                chkQuarter.Enabled = bbetMode
    '                chkQuarter.Checked = False
    '            Next
    '        Next
    '    Next

    'End Sub

    'Public Sub CheckSelectPropGame(ByVal rptGameType As Repeater)

    '    'For k As Integer = 0 To rptGameType.Items.Count - 1
    '    '    Dim chkPropType As CheckBox = CType(rptGameType.Items(k).FindControl("chkPropType"), CheckBox)
    '    '    Dim lblPropType As Label = CType(rptGameType.Items(k).FindControl("lblPropType"), Label)
    '    '    If chkPropType.Checked Then
    '    '        'SetPageBetMode(rptGameType, False)
    '    '        Return
    '    '    End If
    '    'Next
    '    'SetPageBetMode(rptGameType, True)
    'End Sub

    'Public Sub GetSelectPropGame(ByVal rptGameType As Repeater)

    '    For k As Integer = 0 To rptGameType.Items.Count - 1
    '        Dim chkPropType As CheckBox = CType(rptGameType.Items(k).FindControl("chkPropType"), CheckBox)
    '        Dim lblPropType As Label = CType(rptGameType.Items(k).FindControl("lblPropType"), Label)
    '        If chkPropType.Checked Then
    '            SetSelectedGame("Prop_" + lblPropType.Text, True)
    '            _bSelectedPropGame = True
    '        Else
    '            SetSelectedGame("Prop_" + lblPropType.Text, False)

    '        End If
    '    Next
    '    UserSession.SelectedBetType(Me.SelectedPlayerID) = IIf(_bSelectedPropGame, "PROP", "SINGLE").ToString()
    'End Sub

    ''get gametype when choice checkbox in select game for 3 repeater left and right and dropgame
    Protected Sub SelectBetType()
        '  GetSelectPropGame(rptPropGame)
        'If Not _bSelectedPropGame Then

        GetSelectGame(rptGameType)
        ' End If

    End Sub

    Private Sub SetSelectedGame(ByVal psType As String, ByVal pbSelected As Boolean)
        If String.IsNullOrEmpty(Me.SelectedPlayerID) Then
            Return
        End If
        If psType <> "" Then
            If Not pbSelected Then
                UserSession.RemoveSelectedGameType(psType, Me.SelectedPlayerID)
            Else
                UserSession.AddSelectedGameType(psType, Me.SelectedPlayerID)
                Dim olt As List(Of String) = UserSession.SelectedGameTypes(Me.SelectedPlayerID)
                _log.Debug("aaaa" + olt.Count.ToString())
            End If

        End If
    End Sub

    'Public Function ShowPartgame() As String
    '    If BetType.Equals(_BetTheBoard) Then
    '        Return ""
    '    Else
    '        Return "" '"style='display:none'"
    '    End If
    'End Function

    Public Function showImgIcon(ByVal psSportType As String, ByVal psGameType As String) As String
        If psSportType.Equals("Soccer", StringComparison.CurrentCultureIgnoreCase) Then
            Return "<img src='/Images/Soccer/Portugal.png'/>"
        Else
            Return ""
        End If
    End Function

#Region "Available game type"

    Private Function GetAvailableGameType(ByVal plstGameType As List(Of String)) As DataTable
        Dim dicBookMaker As New Dictionary(Of String, String)
        Dim oToday = UserSession.ConvertToEST(DateTime.Now.ToUniversalTime()).AddDays(-1)
        For Each sGameType As String In plstGameType
            Dim sBookmaker As String = UserSession.SysSettings(GetAgentBookMaker(sGameType)).GetValue(sGameType)
            If (sGameType.Contains("Football") OrElse sGameType.Contains("Basketball")) AndAlso (UserSession.Cache.GetFixSpreadMoney(SuperAgentId) IsNot Nothing AndAlso UserSession.Cache.GetFixSpreadMoney(SuperAgentId).UseFixedSpreadMoney) Then
                dicBookMaker(sGameType) = SuperAgentId & " Manipulation"
            Else
                dicBookMaker(sGameType) = sBookmaker
            End If
        Next
        Dim odtGames = New SBCBL.Managers.CGameManager().GetExistsAvailableGames(dicBookMaker, oToday.AddDays(3), True, True, False, "", SuperAgentId, True)
        Return odtGames
    End Function





    'Private Function CheckAvailableGameType(ByVal psGameType As String) As Boolean
    '    Dim bHaveGame As Boolean = False
    '    Dim oGame As New CGame
    '    Dim sBookmaker As String = UserSession.SysSettings(GetAgentBookMaker(psGameType)).GetValue(psGameType)
    '    ' '' if doesn't found book maker, just return, SINCE NOW WE DOESNT FILTER BY BOOK MAKER            

    '    If sBookmaker = "" Then
    '        Return False
    '    End If

    '    ''Get primary bookmakers by gametype
    '    Dim sKey As String = SBCBL.std.GetSiteType & "_AVAILABLE_GAMES_" & psGameType & "_" & sBookmaker
    '    ''get 1h 2h by gameType and BookMaker
    '    Dim sKeyGamePart As String = SBCBL.std.GetSiteType & "_GAMEPART_" & psGameType & "_" & sBookmaker
    '    'Dim obAvailableGames As Boolean = False
    '    If Cache(sKey) Is Nothing Then
    '        bHaveGame = LoadGameAvailable(psGameType, sBookmaker, oGame)
    '        Cache.Insert(sKey, bHaveGame, Nothing, Now.AddSeconds(20), System.Web.Caching.Cache.NoSlidingExpiration)
    '        Cache.Insert(sKeyGamePart, oGame, Nothing, Now.AddSeconds(20), System.Web.Caching.Cache.NoSlidingExpiration)

    '    End If
    '    bHaveGame = CType(Cache(sKey), Boolean)
    '    If bHaveGame AndAlso Game.ContainsKey(psGameType & "_GAMEPART_" & sBookmaker) Then
    '        Game.Remove(sKeyGamePart)
    '        Game.Add(psGameType & "_GAMEPART_" & sBookmaker, CType(Cache(sKeyGamePart), CGame))
    '    ElseIf bHaveGame Then
    '        Game.Add(psGameType & "_GAMEPART_" & sBookmaker, CType(Cache(sKeyGamePart), CGame))
    '    End If
    '    Return bHaveGame
    'End Function

    'Private Function LoadGameAvailable(ByVal psGameType As String, ByVal psBookMaker As String, ByRef oGame As CGame) As Boolean
    '    Dim oToday = UserSession.ConvertToEST(DateTime.Now.ToUniversalTime()) 'GetEasternDate()
    '    Dim bHaveGame As Boolean = False
    '    ' Dim oGame As New CGame
    '    oGame.GameType = psGameType
    '    oGame.BookMaker = psBookMaker
    '    '' Filter by offline time 
    '    ' Dim oSysSettingManager As New CSysSettingManager()
    '    Dim oSys As CSysSetting = UserSession.Cache.GetAllSysSettings(_sOffTimeCategory).Find(Function(x) x.Key = psGameType)

    '    ''Get games
    '    Dim oGameShows As New Dictionary(Of String, DataRow())
    '    Dim odtGames As DataTable
    '    'If SelectedPlayer IsNot Nothing AndAlso SelectedPlayer.IsSuperPlayer Then
    '    '    odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableGamesForSuper(psGameType, oToday, IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), psBookMaker), SuperAgentId)
    '    'Else
    '    'odtGames = New SBCBL.Managers.CGameManager().GetAvailableGames(psGameType, oToday, True, False, "", IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), psBookMaker))
    '    '    odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday, True, False, "", IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), psBookMaker), SuperAgentId)

    '    'End If

    '    odtGames = New SBCBL.Managers.CGameManager().GetAgentAvailableGames(psGameType, oToday, True, True, False, "", IIf(UserSession.UseTigerBookmaker, SafeString(SBCBL.CFileDBKeys.TIGER_SB_BOOKMAKER), psBookMaker), SuperAgentId, True)
    '    Dim nTotalGame As Integer = 0
    '    Dim oListLines As New Dictionary(Of String, DataRow)
    '    Dim oDTResult As DataTable = odtGames.Clone()
    '    Dim nCountGames As Integer = 0

    '    oGameShows.Add("Current", Nothing)
    '    oGameShows.Add("1H", Nothing)
    '    oGameShows.Add("2H", Nothing)

    '    For Each sContext As String In oGameShows.Keys.ToArray()
    '        '' reset the list of line for new Context
    '        'oListLines.Clear()

    '        '' dont apply rule to checkbox
    '        Dim sWhere As String = "" 'CreateContextFilterQuery(psGameType, sContext)

    '        'If SelectedPlayer IsNot Nothing AndAlso SelectedPlayer.IsSuperPlayer Then
    '        '    sWhere = ""
    '        'End If

    '        If sWhere <> "" Then
    '            sWhere &= " and Context = " & SQLString(sContext)
    '        Else
    '            sWhere = "Context = " & SQLString(sContext)
    '        End If

    '        ''Get Game online / offline upto game context
    '        Select Case UCase(sContext)
    '            Case "CURRENT"
    '                sWhere &= " and isnull(IsGameBetLocked,'')  <> 'Y'"
    '            Case "1H"
    '                sWhere &= " and isnull(IsFirstHalfLocked,'')  <> 'Y'"
    '            Case "2H"
    '                sWhere &= " and isnull(IsSecondHalfLocked,'')  <> 'Y'"
    '        End Select

    '        ' '' filter by line off time
    '        'If UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.Value) <> 0 Then
    '        '    sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", SafeInteger(oSys.Value).ToString())
    '        'End If

    '        'If UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.SubCategory) <> 0 Then
    '        '    sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (SafeInteger(oSys.SubCategory) * 60).ToString())
    '        'End If
    '        ''Game time on/off agentset
    '        ' Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
    '        ' Dim odtGameOnOff As DataTable = oGameTypeOnOffManager.GetGameTypeOnOffByGameType(AgentId, psGameType)
    '        Dim lstGameOnOff As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(SuperAgentId, psGameType)
    '        If lstGameOnOff IsNot Nothing AndAlso lstGameOnOff.Count > 0 Then
    '            Dim oGameOnOff As CGameTypeOnOff = lstGameOnOff(0)
    '            '' filter by line off time for agent
    '            If Not IIf(SelectedPlayer IsNot Nothing, True, False) And UCase(sContext) <> "2H" Then
    '                sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", oGameOnOff.OffBefore)
    '            End If

    '            If Not IIf(SelectedPlayer IsNot Nothing, True, False) And UCase(sContext) <> "2H" Then
    '                sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (oGameOnOff.DisplayBefore * 60).ToString())
    '            End If

    '        Else
    '            '' filter by line off time for super
    '            If Not IIf(SelectedPlayer IsNot Nothing, True, False) And UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.Value) <> 0 Then
    '                sWhere &= String.Format(" and isnull(MinuteBefore,0) > {0}", SafeInteger(oSys.Value).ToString())
    '            End If

    '            If Not IIf(SelectedPlayer IsNot Nothing, True, False) And UCase(sContext) <> "2H" And oSys IsNot Nothing AndAlso SafeInteger(oSys.SubCategory) <> 0 Then
    '                sWhere &= String.Format(" and isnull(MinuteBefore,0) < {0}", (SafeInteger(oSys.SubCategory) * 60).ToString())
    '            End If
    '        End If


    '        odtGames.DefaultView.RowFilter = sWhere
    '        Dim oDTFiltered As DataTable = odtGames.DefaultView.ToTable()

    '        '' 2H line off: turn off 2H line after a period time that set by super, applies to football, basketball, soccer
    '        If sContext = "2H" Then
    '            Dim nTimeoff As Integer = UserSession.Cache.GetGameType2HOnOff(SuperAgentId)
    '            If nTimeoff <= 0 Then
    '                nTimeoff = UserSession.SecondHalfTimeOff(psGameType)
    '            End If
    '            If nTimeoff <> -1 Then
    '                sWhere = "MinuteBefore2H  <= " & SafeString(nTimeoff)
    '                sWhere &= " and gamedate < " & SQLString(GetEasternDate())
    '                oDTFiltered.DefaultView.RowFilter = sWhere
    '                oDTFiltered = oDTFiltered.DefaultView.ToTable()
    '            End If
    '        End If
    '        If oDTFiltered.Rows.Count > 0 Then

    '            Select Case UCase(sContext)
    '                Case "CURRENT"
    '                    oGame.Current = True
    '                Case "1H"
    '                    oGame.OneH = True
    '                Case "2H"
    '                    oGame.TwoH = True
    '            End Select
    '            bHaveGame = True
    '            'Return True
    '        End If
    '    Next
    '    'If bHaveGame AndAlso Not Game.ContainsKey(psGameType & "_AVAILABLE_GAMES_" & psBookMaker) Then
    '    '    Game.Add(psGameType & "_AVAILABLE_GAMES_" & psBookMaker, oGame)
    '    'End If
    '    Return bHaveGame
    '    '  Return False
    'End Function



#End Region


End Class



