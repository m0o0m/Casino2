Imports System.Data
Imports SBCBL.Managers
Imports SBCBL.CacheUtils
Imports SBCBL
Imports SBCBL.std

Partial Class SBS_Inc_PlayerJuiceControl
    Inherits UI.CSBCUserControl
    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private _sGameType As String = std.GetSiteType & " GameType"
    Dim oCache As New CCacheManager()
    Dim oSysManager As New CSysSettingManager()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            bindSportType()
            BindGameType()
            bindBetType()
            If UserSession.UserType = SBCBL.EUserType.Agent Then
                trAgent.Visible = False
            Else
                bindAgents()
            End If

            bindPlayers()
            bindJuice()

        End If
    End Sub

#Region "Bind Data"

    Private Sub bindAgents()
        Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
        Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")
        For Each drParent As DataRow In odrParents
            Dim sAgentID As String = std.SafeString(drParent("AgentID"))
            ddlAgents.Items.Add(New ListItem(std.SafeString(drParent("AgentName")), sAgentID))
        Next
    End Sub

    Private Sub bindPlayers()
        Dim dtsPlayers As DataTable
        If UserSession.UserType = EUserType.Agent Then
            dtsPlayers = (New CPlayerManager).GetPlayers(UserSession.AgentUserInfo.UserID, Nothing)
        Else
            dtsPlayers = (New CPlayerManager).GetPlayers(ddlAgents.SelectedValue, Nothing)
        End If

        ddlPlayer.DataSource = dtsPlayers
        ddlPlayer.DataTextField = "FullName"
        ddlPlayer.DataValueField = "PlayerID"
        ddlPlayer.DataBind()
    End Sub

    Private Sub bindSportType()
        Dim oSportTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                             Where std.SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And Not oSetting.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) And Not oSetting.Key.Equals("proposition", StringComparison.CurrentCultureIgnoreCase) _
                                             Order By oSetting.SubCategory, oSetting.Key _
                                             Select oSetting).ToList

        ddlSportType.DataSource = oSportTypes
        ddlSportType.DataTextField = "Key"
        ddlSportType.DataValueField = "Key"
        ddlSportType.DataBind()
    End Sub

    Private Sub bindBetType()
        ddlBetType.DataSource = EnumUtils.GetEnumDescriptions(Of CEnums.EBetType)()
        ddlBetType.DataTextField = "Name"
        ddlBetType.DataValueField = "Value"
        ddlBetType.DataBind()
    End Sub

    Private Sub bindJuice()
        txtJuice.Text = UserSession.Cache.GetPlayerJuiceControl(ddlPlayer.SelectedValue, ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue)
    End Sub

#End Region

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim playerId As String = ddlPlayer.SelectedValue

        If oSysManager.CheckExistSysSetting(playerId & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue) Then
            LogDebug(_log, "save")
            'oGameTypeOnOff.UpdateJuiceControl(UserSession.UserID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
            oSysManager.UpdateValue(playerId & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, std.SafeInteger(txtJuice.Text), psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)
        Else
            'oGameTypeOnOff.InsertJuiceControl(UserSession.UserID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
            LogDebug(_log, "insert")
            oSysManager.AddSysSetting(playerId & "_Juice", ddlContext.SelectedValue, std.SafeInteger(txtJuice.Text), ddlSportType.SelectedValue, psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)

        End If

        UserSession.Cache.ClearSysSettings(playerId & "_Juice", ddlSportType.SelectedValue)
        UserSession.Cache.ClearPlayerJuiceControl(UCase(playerId), UCase(ddlSportType.SelectedValue), UCase(ddlContext.SelectedValue), ddlGameType.SelectedValue, ddlBetType.SelectedValue)
        bindJuice()
        ClientAlert("Submit successfully", True)
    End Sub

    Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
        bindPlayers()
        BindGameType()
        bindJuice()
    End Sub

    Protected Sub ddlPlayer_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayer.SelectedIndexChanged
        bindJuice()
    End Sub

    Protected Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
        BindGameType()
        bindJuice()
    End Sub

    Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged, ddlGameType.SelectedIndexChanged, ddlBetType.SelectedIndexChanged
        bindJuice()
    End Sub

    Private Sub BindGameType()
        Dim sportType As String = ddlSportType.SelectedValue.ToLower()

        Dim gameTypes As String()
        Select Case sportType
            Case "football"
                gameTypes = std.FOOTBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
            Case "basketball"
                gameTypes = std.BASKETBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
            Case "soccer"
                gameTypes = std.SOCCER_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
            Case "baseball"
                gameTypes = std.BASEBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
            Case "hockey"
                gameTypes = std.HOCKEY_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)
            Case Else
                gameTypes = New String() {}
        End Select

        ddlGameType.DataSource = gameTypes
        ddlGameType.DataBind()
    End Sub


End Class
