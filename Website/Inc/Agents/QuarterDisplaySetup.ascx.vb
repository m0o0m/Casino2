Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents

    Partial Class Inc_Agents_QuarterDisplaySetup
        Inherits SBCBL.UI.CSBCUserControl
        Private _sGameType As String = SBCBL.std.GetSiteType & " GameType"
        Dim _sCategory As String = SBCBL.std.GetSiteType & " LineOffHour"
        Dim _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim oSysManager As New CSysSettingManager()
#Region "Property"

        Public Property IsSuperAdmin() As Boolean
            Get
                If ViewState("ISSUPERADMIN") IsNot Nothing Then
                    Return CType(ViewState("ISSUPERADMIN"), Boolean)
                Else
                    Return False
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ISSUPERADMIN") = value
            End Set
        End Property


        Private ReadOnly Property UserID() As String
            Get
                If Me.IsSuperAdmin Then
                    Return SafeString(ddlAgents.SelectedValue) & "LineOffHour"
                Else
                    Return UserSession.UserID & "LineOffHour"
                End If
            End Get
        End Property

#End Region

#Region "Page Events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                trAgents.Visible = Me.IsSuperAdmin
                hfIsSuperAdmin.Value = Me.IsSuperAdmin
                BindSportType()
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    If ddlSportType.SelectedValue <> "" And ddlAgents.SelectedValue <> "" Then
                        SetSportType()
                    End If
                Else
                    If ddlSportType.SelectedValue <> "" Then
                        SetSportType()
                    End If
                End If

            End If
        End Sub

#End Region

#Region "Controls Events"

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            SetSportType()
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

            Try

                Dim bSuccess As Boolean = False
                'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()

                'If oGameTypeOnOffManager.GetGameTeamDisplay(Me.UserID, SafeString(ddlSportType.SelectedValue)) IsNot Nothing AndAlso oGameTypeOnOffManager.GetGameTeamDisplay(Me.UserID, SafeString(ddlSportType.SelectedValue)).Count > 0 Then
                '    bSuccess = oGameTypeOnOffManager.UpdateGameTeamDisplayByAgent(Me.UserID, _
                '                                                                SafeString(ddlSportType.SelectedValue), _
                '                                                                SafeInteger(txtOffMinutes.Text), _
                '                                                                SafeInteger(txtDisplayHours.Text))
                'Else
                '    bSuccess = oGameTypeOnOffManager.InsertGameQuarterDisplayByAgent(Me.UserID, _
                '                                                                newGUID(), _
                '                                                                SafeString(ddlSportType.SelectedValue), _
                '                                                                SafeInteger(txtOffMinutes.Text), _
                '                                                                SafeInteger(txtDisplayHours.Text))
                'End If
                'UserSession.Cache.ClearGameTeamDisplay(Me.UserID, ddlSportType.SelectedValue)

                If ddlAgents.SelectedValue.Equals("All", StringComparison.CurrentCultureIgnoreCase) Then
                    bSuccess = saveAllAgent()
                Else
                    If oSysManager.IsExistedCategory(Me.UserID) Then
                        bSuccess = oSysManager.UpdateValue(Me.UserID, ddlSportType.SelectedValue, "OffMinutes", txtOffMinutes.Text)
                        bSuccess = oSysManager.UpdateValue(Me.UserID, ddlSportType.SelectedValue, "DisplayHours", txtDisplayHours.Text)
                    Else
                        bSuccess = oSysManager.AddSysSetting(Me.UserID, "OffMinutes", txtOffMinutes.Text, ddlSportType.SelectedValue)
                        bSuccess = oSysManager.AddSysSetting(Me.UserID, "DisplayHours", txtDisplayHours.Text, ddlSportType.SelectedValue)
                    End If
                    UserSession.Cache.ClearSysSettings(UserID, ddlSportType.SelectedValue)
                End If
                'UserSession.Cache.ClearSysSettings(Me.UserID)
                If bSuccess Then
                    ClientAlert("Setup Successfully Saved", True)
                Else
                    ClientAlert("Setup Failed to Save Setting. Please try again later.", True)
                End If

            Catch ex As Exception
                LogError(_log, "Cannot save setup", ex)
            End Try

        End Sub

        Protected Sub ddlSportType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSportType.SelectedIndexChanged
            'Dim oSysSettingManager As New CSysSettingManager()
            'Dim olstGameTypeOnOff As List(Of CGameTypeOnOff) = UserSession.Cache.GetGameTypeOnOff(Me.UserID, SafeString(ddlSportType.SelectedValue))

            'If olstGameTypeOnOff IsNot Nothing AndAlso olstGameTypeOnOff.Count > 0 Then
            '    Dim oGameTypeOnOff As CGameTypeOnOff = olstGameTypeOnOff(0)
            '    txtOffMinutes.Text = oGameTypeOnOff.OffBefore
            '    txtDisplayHours.Text = oGameTypeOnOff.DisplayBefore
            'Else
            '    Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(_sCategory).Find(Function(x) x.Key = SafeString(ddlSportType.SelectedValue))
            '    If oSys IsNot Nothing AndAlso oSys.Value <> "" Then
            '        txtOffMinutes.Text = SafeInteger(oSys.Value)
            '        txtDisplayHours.Text = SafeInteger(oSys.SubCategory)
            '    Else
            '        txtOffMinutes.Text = ""
            '        txtDisplayHours.Text = ""
            '    End If
            'End If

            SetSportType()
        End Sub

#End Region

#Region "Functions"

        Public Sub bindAgents(ByVal psSuperAdminID As String)
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)
            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "Login"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
            ddlAgents.Items.Insert(0, "All")
        End Sub

        'Private Sub BindTimeOff()
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    Dim odtGameTypeOnOff As DataTable
        '    odtGameTypeOnOff = oGameTypeOnOffManager.GetALLGameTypeOnOffByAgentID(Me.UserID)
        '    dtgTimeOff.DataSource = odtGameTypeOnOff
        '    dtgTimeOff.DataBind()
        '    dtgTimeOff.Visible = odtGameTypeOnOff.Rows.Count > 0
        'End Sub

        Private Sub BindSportType()
            'Dim oSportTypes As List(Of CSysSetting) = SBCBL.Utils.CSerializedObjectCloner.Clone(Of List(Of CSysSetting))((From oSetting As CSysSetting In UserSession.Cache.GetAllSysSettings(_sGameType) _
            '                                    Where SafeBoolean(oSetting.Value) And oSetting.SubCategory = "" And oSetting.Key <> "Proposition" And oSetting.Key <> "Other" And oSetting.Key <> "Baseball" And oSetting.Key <> "Hockey" _
            '                                    And oSetting.Key <> "Soccer" _
            '                                    Order By oSetting.Key _
            '                                    Select oSetting).ToList)
            Dim lstSporttype As New List(Of String)
            lstSporttype.Add("Football")
            lstSporttype.Add("Basketball")

            'ddlSportType.DataTextField = "Key"
            'ddlSportType.DataValueField = "Key"
            ddlSportType.DataSource = lstSporttype
            ddlSportType.DataBind()
        End Sub


        Private Sub SetSportType()
            ' Dim oGameTeamDisplayList As CGameTeamDisplayList = UserSession.Cache.GetGameTeamDisplay(Me.UserID, ddlSportType.SelectedValue)
            'Dim oGameTeamDisplay As CGameTeamDisplay = oGameTeamDisplayList.GetGameTeamDisplay(ddlSportType.SelectedValue)
            If oSysManager.IsExistedSubCategory(UserID, ddlSportType.SelectedValue) Then
                txtOffMinutes.Text = UserSession.Cache.GetSysSettings(UserID, ddlSportType.SelectedValue).GetIntegerValue("OffMinutes")
                txtDisplayHours.Text = UserSession.Cache.GetSysSettings(UserID, ddlSportType.SelectedValue).GetIntegerValue("DisplayHours")
            Else
                txtOffMinutes.Text = ""
                txtDisplayHours.Text = ""
            End If
        End Sub

#End Region

        'Protected Sub dtgTimeOff_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dtgTimeOff.ItemCommand
        '    Try
        '        If e.CommandName.ToUpper = "DELETESETTINGS" Then
        '            Dim poSettingID As String = SafeString(e.CommandArgument)
        '            Dim CGameTypeOnOffLineMng As New CGameTypeOnOffManager()
        '            If CGameTypeOnOffLineMng.DeleteDisplaySettings(Me.UserID, poSettingID) Then
        '                ClientAlert("Successfully Deleted", True)
        '            Else
        '                ClientAlert("Error In Deleting Game Setting", True)
        '            End If
        '            'Clear Cache
        '            UserSession.Cache.ClearGameTypeOnOff(Me.UserID, SafeString(ddlSportType.SelectedValue))
        '            BindTimeOff()
        '        End If

        '    Catch ex As Exception
        '        LogError(_log, "Cannot delete display setting.", ex)
        '    End Try
        'End Sub
        Private Function saveAllAgent() As Boolean
            Dim oAgentManager As New CAgentManager()
            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(UserSession.UserID, Nothing)
            If oData Is Nothing Then
                Return False
            End If
            Dim bSuccess As Boolean = False
            For Each dr As DataRow In oData.Rows
                If oSysManager.IsExistedSubCategory(SafeString(dr("AgentID")).ToString() & "LineOffHour", ddlSportType.SelectedValue) Then
                    bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID").ToString() & "LineOffHour"), ddlSportType.SelectedValue, "OffMinutes", txtOffMinutes.Text)
                    bSuccess = oSysManager.UpdateValue(SafeString(dr("AgentID").ToString() & "LineOffHour"), ddlSportType.SelectedValue, "DisplayHours", txtDisplayHours.Text)
                Else
                    bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID").ToString() & "LineOffHour"), "OffMinutes", txtOffMinutes.Text, ddlSportType.SelectedValue)
                    bSuccess = oSysManager.AddSysSetting(SafeString(dr("AgentID").ToString() & "LineOffHour"), "DisplayHours", txtDisplayHours.Text, ddlSportType.SelectedValue)
                End If
                UserSession.Cache.ClearSysSettings(SafeString(dr("AgentID").ToString() & "LineOffHour"), ddlSportType.SelectedValue)
            Next
            Return bSuccess
        End Function
    End Class
End Namespace