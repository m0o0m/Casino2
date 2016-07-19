Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents
    Partial Class MaxPerGame24h
        Inherits SBCBL.UI.CSBCUserControl
        Dim strMaxperGame24h As String = "MAXPERGAME_24H"
        Dim strMaxperGame24hTimer As String = "MAXPERGAME_24H_TIMER"
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    bindAgents()
                Else
                    trAgent.Visible = False
                End If
                bindSportType()
                Dim lstMaxPerGame24H As CSysSettingList
                Dim lstMaxPerGame24HTimer As CSysSettingList
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    lstMaxPerGame24H = UserSession.Cache.GetSysSettings(strMaxperGame24h & ddlAgents.SelectedValue)
                    lstMaxPerGame24HTimer = UserSession.Cache.GetSysSettings(strMaxperGame24hTimer & ddlAgents.SelectedValue)
                Else
                    lstMaxPerGame24H = UserSession.Cache.GetSysSettings(strMaxperGame24h & UserSession.UserID)
                    lstMaxPerGame24HTimer = UserSession.Cache.GetSysSettings(strMaxperGame24hTimer & UserSession.UserID)
                End If
                If lstMaxPerGame24H IsNot Nothing Then
                    txtMaxPerGame.Text = lstMaxPerGame24H.GetValue(ddlSportType.SelectedValue)
                Else
                    txtMaxPerGame.Text = ""
                End If
                If lstMaxPerGame24HTimer IsNot Nothing Then
                    txtTime.Text = lstMaxPerGame24HTimer.GetValue(ddlSportType.SelectedValue)
                Else
                    txtTime.Text = ""
                End If
            End If
        End Sub

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

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Dim oSysManager As New CSysSettingManager()
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
                Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")
                'For Each drParent As DataRow In odrParents
                '    Dim sAgentID As String = SafeString(drParent("AgentID"))
                '    If oSysManager.IsExistedKey(strMaxperGame24h & sAgentID, "", ddlSportType.SelectedValue) Then
                '        oSysManager.UpdateValue(strMaxperGame24h & sAgentID, "", ddlSportType.SelectedValue, txtMaxPerGame.Text)
                '    Else
                '        oSysManager.AddSysSetting(strMaxperGame24h & sAgentID, ddlSportType.SelectedValue, txtMaxPerGame.Text, "")
                '    End If
                '    UserSession.Cache.ClearSysSettings(strMaxperGame24h & sAgentID)
                'Next
                Dim sAgentID As String = ddlAgents.SelectedValue
                If oSysManager.IsExistedKey(strMaxperGame24h & sAgentID, "", ddlSportType.SelectedValue) Then
                    oSysManager.UpdateValue(strMaxperGame24h & sAgentID, "", ddlSportType.SelectedValue, txtMaxPerGame.Text)
                Else
                    oSysManager.AddSysSetting(strMaxperGame24h & sAgentID, ddlSportType.SelectedValue, txtMaxPerGame.Text, "")
                End If
                If oSysManager.IsExistedKey(strMaxperGame24hTimer & sAgentID, "", ddlSportType.SelectedValue) Then
                    oSysManager.UpdateValue(strMaxperGame24hTimer & sAgentID, "", ddlSportType.SelectedValue, txtTime.Text)
                Else
                    oSysManager.AddSysSetting(strMaxperGame24hTimer & sAgentID, ddlSportType.SelectedValue, txtTime.Text, "")
                End If
                UserSession.Cache.ClearSysSettings(strMaxperGame24h & sAgentID)
                UserSession.Cache.ClearSysSettings(strMaxperGame24hTimer & sAgentID)
            Else
                If oSysManager.IsExistedKey(strMaxperGame24h & UserSession.UserID, "", ddlSportType.SelectedValue) Then
                    oSysManager.UpdateValue(strMaxperGame24h & UserSession.UserID, "", ddlSportType.SelectedValue, txtMaxPerGame.Text)
                Else
                    oSysManager.AddSysSetting(strMaxperGame24h & UserSession.UserID, ddlSportType.SelectedValue, txtMaxPerGame.Text, "")
                End If
                If oSysManager.IsExistedKey(strMaxperGame24hTimer & UserSession.UserID, "", ddlSportType.SelectedValue) Then
                    oSysManager.UpdateValue(strMaxperGame24hTimer & UserSession.UserID, "", ddlSportType.SelectedValue, txtTime.Text)
                Else
                    oSysManager.AddSysSetting(strMaxperGame24hTimer & UserSession.UserID, ddlSportType.SelectedValue, txtTime.Text, "")
                End If
                UserSession.Cache.ClearSysSettings(strMaxperGame24h & UserSession.UserID)
                UserSession.Cache.ClearSysSettings(strMaxperGame24hTimer & UserSession.UserID)
            End If



        End Sub

        Protected Sub ddlGameTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                Dim lstMaxPerGame24H1 As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24h & ddlAgents.SelectedValue)
                If lstMaxPerGame24H1 IsNot Nothing Then
                    txtMaxPerGame.Text = lstMaxPerGame24H1.GetValue(ddlSportType.SelectedValue)
                Else
                    txtMaxPerGame.Text = ""
                End If

            Else
                Dim lstMaxPerGame24H1 As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24h & UserSession.UserID)
                If lstMaxPerGame24H1 IsNot Nothing Then
                    txtMaxPerGame.Text = lstMaxPerGame24H1.GetValue(ddlSportType.SelectedValue)
                Else
                    txtMaxPerGame.Text = ""
                End If
            End If
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                Dim lstMaxPerGame24HTimer As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24hTimer & ddlAgents.SelectedValue)
                If lstMaxPerGame24HTimer IsNot Nothing Then
                    txtTime.Text = lstMaxPerGame24HTimer.GetValue(ddlSportType.SelectedValue)
                Else
                    txtTime.Text = ""
                End If

            Else
                Dim lstMaxPerGame24HTimer As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24hTimer & UserSession.UserID)
                If lstMaxPerGame24HTimer IsNot Nothing Then
                    txtTime.Text = lstMaxPerGame24HTimer.GetValue(ddlSportType.SelectedValue)
                Else
                    txtTime.Text = ""
                End If
            End If
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                Dim lstMaxPerGame24H As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24h & ddlAgents.SelectedValue)
                Dim lstMaxPerGame24HTimer As CSysSettingList = UserSession.Cache.GetSysSettings(strMaxperGame24hTimer & ddlAgents.SelectedValue)
                If lstMaxPerGame24H IsNot Nothing Then
                    txtMaxPerGame.Text = lstMaxPerGame24H.GetValue(ddlSportType.SelectedValue)
                Else
                    txtMaxPerGame.Text = ""
                   
                End If
                If lstMaxPerGame24HTimer IsNot Nothing Then
                    txtTime.Text = lstMaxPerGame24HTimer.GetValue(ddlSportType.SelectedValue)
                Else
                    txtTime.Text = ""
                End If
            End If
        End Sub
    End Class
End Namespace