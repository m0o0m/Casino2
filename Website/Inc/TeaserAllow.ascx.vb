Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBCSuperAdmins
    Partial Class TeaserAllow
        Inherits SBCBL.UI.CSBCUserControl
        Dim sCategory As String = "TeaserAllow"
        Dim lstTeaser As New List(Of String)
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Property Teaser() As List(Of String)
            Get
                lstTeaser.Add("4/6")
                lstTeaser.Add("4.5/6.5")
                lstTeaser.Add("5/7")
                lstTeaser.Add("5.5/7.5")
                lstTeaser.Add("8/10")
                lstTeaser.Add("4/13")
                Return lstTeaser
            End Get
            Set(ByVal value As List(Of String))

            End Set
        End Property

        Private Sub bindAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")
            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))
            Next
        End Sub
        Private Sub BindData()
            Dim oSysManager As New CSysSettingManager()
            Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory)
            If oListSettings Is Nothing Then
                Return
            End If
            Dim sAgent As String = ""
            Dim sKey As String = ""
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                sAgent = ddlAgents.SelectedValue
            Else
                sAgent = UserSession.UserID
            End If
            sKey = sAgent & "4/6"
            Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser46.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser46.Checked = False
            End If
            sKey = sAgent & "4.5/6.5"
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser4565.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser4565.Checked = False
            End If
            sKey = sAgent & "5/7"
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser57.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser57.Checked = False
            End If
            sKey = sAgent & "5.5/7.5"
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser5575.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser5575.Checked = False
            End If
            sKey = sAgent & "3/8"
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser38.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser38.Checked = False
            End If
            sKey = sAgent & "4/13"
            oSetting = oListSettings.Find(Function(x) x.Key = sKey)
            If oSetting IsNot Nothing Then
                chkTeaser413.Checked = SafeBoolean(oSetting.Value)
            Else
                chkTeaser413.Checked = False
            End If
        End Sub
        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                BindData()
            End If
        End Sub
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    bindAgents()
                Else
                    trAgent.Visible = False
                End If
                BindData()
            End If
        End Sub
        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
            Dim sKey As String = ""
            Dim sAgent As String = ""
            Try
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    sAgent = ddlAgents.SelectedValue
                    Dim oSysManager As New CSysSettingManager()
                    Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory)
                    sKey = sAgent & "4/6"
                    Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser46.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser46.Checked.ToString())
                    End If
                    sKey = sAgent & "4.5/6.5"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser4565.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser4565.Checked.ToString())
                    End If
                    sKey = sAgent & "5/7"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser57.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser57.Checked.ToString())
                    End If
                    sKey = sAgent & "5.5/7.5"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser5575.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser5575.Checked.ToString())
                    End If
                    sKey = sAgent & "3/8"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser38.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser38.Checked.ToString())
                    End If
                    'ClientAlert(oSetting.Key.ToString() & chkTeaser38.Checked.ToString() & sKey)
                    sKey = sAgent & "4/13"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser413.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser413.Checked.ToString())
                    End If

                Else
                    sAgent = UserSession.UserID
                    Dim oSysManager As New CSysSettingManager()
                    Dim oListSettings As CSysSettingList = UserSession.SysSettings(sCategory)
                    sKey = sAgent & "4/6"
                    Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser46.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser46.Checked.ToString())
                    End If
                    sKey = sAgent & "4.5/6.5"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser4565.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser4565.Checked.ToString())
                    End If
                    sKey = sAgent & "5/7"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser57.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser57.Checked.ToString())
                    End If
                    sKey = sAgent & "5.5/7.5"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser5575.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser5575.Checked.ToString())
                    End If
                    sKey = sAgent & "3/8"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser38.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser38.Checked.ToString())
                    End If
                    'ClientAlert(oSetting.Key.ToString() & chkTeaser38.Checked.ToString() & sKey)
                    sKey = sAgent & "4/13"
                    oSetting = oListSettings.Find(Function(x) x.Key = sKey)
                    If oSetting Is Nothing Then
                        oSysManager.AddSysSetting(sCategory, sKey, chkTeaser413.Checked.ToString())
                    Else
                        oSysManager.UpdateKeyValue(oSetting.SysSettingID, sKey, chkTeaser413.Checked.ToString())
                    End If
                End If
                UserSession.Cache.ClearSysSettings(sCategory)
                ClientAlert("Save successfull", True)
            Catch ex As Exception
                ClientAlert("Save fail")
                _log.Error("loi teaser" & ex.Message)
            End Try
        End Sub
    End Class
End Namespace

