Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports System.Data
Imports SBCBL.Managers

Namespace SBSAgents

    Partial Class GamePartTimeDisplaySetup
        Inherits SBCBL.UI.CSBCUserControl
        Dim oSysManager As New CSysSettingManager()
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"

#Region "property"

        Private ReadOnly Property SuperAgentID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID & "PartTimeDisplay"
                Else
                    Return ddlAgents.SelectedValue & "PartTimeDisplay"
                End If
            End Get
        End Property

#End Region


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindGameType()
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    bindAgents(UserSession.UserID)
                    trAgents.Visible = True
                End If
            End If

        End Sub

#Region "Bind Data"

        Private Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)
            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
            ddlAgents.Items.Insert(0, "All")
        End Sub



        Private Sub BindGameType()
            'Dim oGameTypes As List(Of CSysSetting) = (From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                      Where SafeBoolean(oSetting.Value) And oSetting.SubCategory <> "" _
            '                                      Order By oSetting.Key, oSetting.SubCategory Ascending _
            '                                      Select oSetting).ToList
            ddlGameType.DataTextField = "Key"
            ddlGameType.DataValueField = "Key"
            ddlGameType.DataSource = GetGameType()
            ddlGameType.DataBind()
        End Sub

#End Region

#Region "Event"

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            SetGameType()

        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameType.SelectedIndexChanged
            SetGameType()
        End Sub

      

        Protected Sub btnSaveOffLine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveOffLine.Click
            If String.IsNullOrEmpty(SuperAgentID) Then
                ClientAlert("Please, Select Agent", True)
                Return
            End If
            'Dim GameTypeOnOffManager As New CGameTypeOnOffManager()
            Dim bSuccess As Boolean
            Dim oGameHalfTimeOFF As New CGameHalfTimeOFF(ddlGameType.SelectedValue, SafeBoolean(chk1HOFF.Checked), SafeBoolean(chk2HOFF.Checked), SafeBoolean(chkTeamtotalOFF.Checked))
            'If GameTypeOnOffManager.CheckExistsGameHTimeOff(SuperAgentID, ddlGameType.SelectedValue) Then
            '    bSuccess = GameTypeOnOffManager.UpdateGameHalfTimeOff(SuperAgentID, oGameHalfTimeOFF)
            'Else
            '    bSuccess = GameTypeOnOffManager.InsertGameHalfTimeOff(SuperAgentID, oGameHalfTimeOFF)
            'End If
            If ddlAgents.SelectedValue.Equals("All", StringComparison.CurrentCultureIgnoreCase) Then
                bSuccess = saveAllAgent()
            Else
                If oSysManager.CheckExistSysSetting(SuperAgentID, ddlGameType.SelectedValue, "FirstHOff") Then
                    bSuccess = oSysManager.UpdateValue(SuperAgentID, ddlGameType.SelectedValue, "FirstHOff", oGameHalfTimeOFF.FirstHOff)
                    bSuccess = oSysManager.UpdateValue(SuperAgentID, ddlGameType.SelectedValue, "SecondHOff", oGameHalfTimeOFF.SecondHOff)
                    bSuccess = oSysManager.UpdateValue(SuperAgentID, ddlGameType.SelectedValue, "TeamTotalOff", oGameHalfTimeOFF.TeamTotalOff)
                Else
                    bSuccess = oSysManager.AddSysSetting(SuperAgentID, "FirstHOff", oGameHalfTimeOFF.FirstHOff, ddlGameType.SelectedValue)
                    bSuccess = oSysManager.AddSysSetting(SuperAgentID, "SecondHOff", oGameHalfTimeOFF.SecondHOff, ddlGameType.SelectedValue)
                    bSuccess = oSysManager.AddSysSetting(SuperAgentID, "TeamTotalOff", oGameHalfTimeOFF.TeamTotalOff, ddlGameType.SelectedValue)
                End If
            End If

            If bSuccess Then
                UserSession.Cache.ClearSysSettings(SuperAgentID)
                'UserSession.Cache.ClearGameHalfTimeOFF(SuperAgentID)
                ClientAlert("Successfully Saved", True)
            Else
                ClientAlert("Failed to Save Setting", True)
            End If
        End Sub
#End Region

        Private Sub SetGameType()
            'Dim oGameHalfTimeOffList As CGameHalfTimeOffList = UserSession.Cache.GetGameHalfTimeOFF(SuperAgentID)
            'Dim oGameHalfTimeOff As CGameHalfTimeOFF = oGameHalfTimeOffList.GetGameHalfTimeOff(ddlGameType.SelectedValue)
            If oSysManager.IsExistedCategory(SuperAgentID) Then
                chk1HOFF.Checked = UserSession.Cache.GetSysSettings(SuperAgentID, ddlGameType.SelectedValue).GetBooleanValue("FirstHOff") 'oGameHalfTimeOff.FirstHOff
                chk2HOFF.Checked = UserSession.Cache.GetSysSettings(SuperAgentID, ddlGameType.SelectedValue).GetBooleanValue("SecondHOff") 'oGameHalfTimeOff.SecondHOff
                chkTeamtotalOFF.Checked = UserSession.Cache.GetSysSettings(SuperAgentID, ddlGameType.SelectedValue).GetBooleanValue("TeamTotalOff") 'oGameHalfTimeOff.TeamTotalOff
            Else
                chk1HOFF.Checked = False
                chk2HOFF.Checked = False
                chkTeamtotalOFF.Checked = False
            End If
        End Sub

        Private Function saveAllAgent() As Boolean
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            If oData Is Nothing Then
                Return False
            End If
            Dim bSuccess As Boolean = False
            Dim oGameHalfTimeOFF As New CGameHalfTimeOFF(ddlGameType.SelectedValue, SafeBoolean(chk1HOFF.Checked), SafeBoolean(chk2HOFF.Checked), SafeBoolean(chkTeamtotalOFF.Checked))
            For Each dr As DataRow In oData.Rows
                If oSysManager.CheckExistSysSetting(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), ddlGameType.SelectedValue, "FirstHOff") Then
                    bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), ddlGameType.SelectedValue, "FirstHOff", oGameHalfTimeOFF.FirstHOff)
                    bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), ddlGameType.SelectedValue, "SecondHOff", oGameHalfTimeOFF.SecondHOff)
                    bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), ddlGameType.SelectedValue, "TeamTotalOff", oGameHalfTimeOFF.TeamTotalOff)
                Else
                    bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), "FirstHOff", oGameHalfTimeOFF.FirstHOff, ddlGameType.SelectedValue)
                    bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), "SecondHOff", oGameHalfTimeOFF.SecondHOff, ddlGameType.SelectedValue)
                    bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"), "TeamTotalOff", oGameHalfTimeOFF.TeamTotalOff, ddlGameType.SelectedValue)
                End If
                UserSession.Cache.ClearSysSettings(SafeString(dr("AgentID").ToString() & "PartTimeDisplay"))
            Next
            Return bSuccess
        End Function
    End Class

End Namespace

