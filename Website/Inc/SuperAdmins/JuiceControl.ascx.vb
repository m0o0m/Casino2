Imports SBCBL
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class JuiceControl
        Inherits SBCBL.UI.CSBCUserControl
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Dim oCache As New CCacheManager()
        Dim oSysManager As New CSysSettingManager()

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindSportType()
                bindBetType()
                BindGameType()
                bindAgents()
                bindJuice()
                If UserSession.UserType = EUserType.Agent Then
                    trAgent.Visible = False
                End If
            End If
        End Sub

#Region "Bind Data"

        Private Sub bindAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")
            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))
            Next
        End Sub

        Private Sub bindSportType()
            Dim oSportTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
                                                 Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And Not oSetting.Key.Equals("Other", StringComparison.CurrentCultureIgnoreCase) And Not oSetting.Key.Equals("proposition", StringComparison.CurrentCultureIgnoreCase) _
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
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                txtJuice.Text = UserSession.Cache.GetJuiceControl(ddlAgents.SelectedValue, ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue)
            Else
                txtJuice.Text = UserSession.Cache.GetJuiceControl(UserSession.UserID, ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue)
            End If
        End Sub

#End Region

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            'Dim skey As String = ddlSportType.SelectedValue

            Dim agentId As String = UserSession.UserID

            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                agentId = ddlAgents.SelectedValue

                If agentId.Equals("All") Then
                    SaveJuiceAllAgent()
                    bindJuice()
                    Return
                End If
            End If
            
            If oSysManager.CheckExistSysSetting(agentId & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue) Then
                LogDebug(_log, "save")
                'oGameTypeOnOff.UpdateJuiceControl(UserSession.UserID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
                oSysManager.UpdateValue(agentId & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, SafeInteger(txtJuice.Text), psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)
            Else
                'oGameTypeOnOff.InsertJuiceControl(UserSession.UserID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
                LogDebug(_log, "insert")
                oSysManager.AddSysSetting(agentId & "_Juice", ddlContext.SelectedValue, SafeInteger(txtJuice.Text), ddlSportType.SelectedValue, psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)

            End If

            'UserSession.Cache.ClearSysSettings(UCase(agentId) & UCase(ddlSportType.SelectedValue) & UCase(ddlContext.SelectedValue))
            'UserSession.Cache.ClearJuiceControl(agentId & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue)
            UserSession.Cache.ClearSysSettings(agentId & "_Juice", ddlSportType.SelectedValue)
            UserSession.Cache.ClearJuiceControl(UCase(agentId), UCase(ddlSportType.SelectedValue), UCase(ddlContext.SelectedValue), ddlGameType.SelectedValue, ddlBetType.SelectedValue)
            bindJuice()
            ClientAlert("Submit successfully", True)
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            BindGameType()
            bindJuice()
        End Sub

        Protected Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            BindGameType()
            bindJuice()
        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged, ddlGameType.SelectedIndexChanged, ddlBetType.SelectedIndexChanged
            bindJuice()
        End Sub

        Private Sub SaveJuiceAllAgent()
            'Dim oGameTypeOnOff = New CGameTypeOnOffManager()
            Dim oAgentManager = New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            If oData Is Nothing Then
                Return
            End If
            Try
                For Each dr As DataRow In oData.Rows
                    Dim sAgentID As String = SafeString(dr("AgentID"))
                    'If oGameTypeOnOff.CheckExistsJuiceControlByAgent(sAgentID, ddlSportType.SelectedValue, ddlContext.SelectedValue) Then
                    '    oGameTypeOnOff.UpdateJuiceControl(sAgentID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
                    'Else
                    '    oGameTypeOnOff.InsertJuiceControl(sAgentID, ddlSportType.SelectedValue, SafeInteger(txtJuice.Text), ddlContext.SelectedValue)
                    'End If
                    If oSysManager.CheckExistSysSetting(sAgentID & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, ddlGameType.SelectedValue, ddlBetType.SelectedValue) Then
                        oSysManager.UpdateValue(sAgentID & "_Juice", ddlSportType.SelectedValue, ddlContext.SelectedValue, SafeInteger(txtJuice.Text), psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)
                    Else
                        oSysManager.AddSysSetting(sAgentID & "_Juice", ddlContext.SelectedValue, SafeInteger(txtJuice.Text), ddlSportType.SelectedValue, psOrther:=ddlGameType.SelectedValue, psOtherType:=ddlBetType.SelectedValue)

                    End If
                    UserSession.Cache.ClearSysSettings(sAgentID & "_Juice", ddlSportType.SelectedValue)
                    UserSession.Cache.ClearJuiceControl(UCase(sAgentID), UCase(ddlSportType.SelectedValue), UCase(ddlContext.SelectedValue), ddlGameType.SelectedValue, ddlBetType.SelectedValue)
                Next
            Catch ex As Exception
                ClientAlert("Save Fail", True)
            End Try
            ClientAlert("Save Successful", True)
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
End Namespace

