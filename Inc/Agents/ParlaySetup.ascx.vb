Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents
    Partial Class ParlaySetup
        Inherits SBCBL.UI.CSBCUserControl
        Dim _sParlayRules As String = SBCBL.std.GetSiteType & " ParlayRules"
        Dim _sMaxSelection As String = "MaxSelection"
        Dim _sRoundRobinCategory As String = "Round_Robin"
        Dim oSysManager As New CSysSettingManager()

#Region "Property"
        Public Property Title() As String
            Get
                Return SafeString(ViewState("Title"))
            End Get
            Set(ByVal value As String)
                ViewState("Title") = value
            End Set
        End Property

        Public ReadOnly Property sAgentID() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID.ToString()
                ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    Return ddlAgents.Value
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property ParlayRule() As String
            Get
                If UserSession.UserType = SBCBL.EUserType.Agent Then
                    Return UserSession.UserID.ToString() & " ParlayRules"
                ElseIf UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    Return ddlAgents.Value & " ParlayRules"
                End If
                Return Nothing
            End Get

        End Property

        Public ReadOnly Property getRoundRobinKey As String
            Get
                Return "Round_" & sAgentID
            End Get
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                BindRoundRobin()
                trAgents.Visible = IIf(UserSession.UserType = SBCBL.EUserType.SuperAdmin, True, False)
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    BindAgents()
                Else
                    bindData()
                End If
            End If
        End Sub

        Private Sub BindAgents()
            Dim oAgentManager As New CAgentManager()

            Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(IIf(UserSession.UserType = SBCBL.EUserType.SuperAdmin, UserSession.UserID, UserSession.SuperAdminInfo.UserID), Nothing)
            ddlAgents.DataSource = oData
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
        End Sub

        Private Sub bindData()

            Dim nMaxParplaySelect = UserSession.Cache.GetSysSettings(ParlayRule).GetIntegerValue(_sMaxSelection)
            If nMaxParplaySelect <= 0 Then
                txtMaxSelection.Text = UserSession.Cache.GetSysSettings(ParlayRule).GetIntegerValue(_sMaxSelection)
            Else
                txtMaxSelection.Text = nMaxParplaySelect
            End If

        End Sub


        Public Sub BindRoundRobin()
            chkRoundRobin.Checked = SBCBL.std.CheckRoundRobin(sAgentID)
        End Sub

        Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            If SafeInteger(txtMaxSelection.Text) <= 0 Then
                ClientAlert("Parplay Setup Max Selection Is Invalid", True)
                Return
            End If
            'Dim nMaxParplaySelect = oGameTypeOnOffManager.GetValueParplaySetup(sAgentID.ToString)
            If Not oSysManager.CheckExistSysSetting(ParlayRule, "", "MaxSelection") Then
                ' oGameTypeOnOffManager.InsertParplaySetup(sAgentID.ToString, txtMaxSelection.Text)
                oSysManager.AddSysSetting(ParlayRule, "MaxSelection", txtMaxSelection.Text)
            Else
                'oGameTypeOnOffManager.UpdateParplaySetup(sAgentID.ToString, txtMaxSelection.Text)
                oSysManager.UpdateValue(ParlayRule, "", "MaxSelection", txtMaxSelection.Text)
            End If
            UserSession.Cache.ClearSysSettings(ParlayRule)
            UserSession.Cache.ClearParplaySetUp(ParlayRule)
            ClientAlert("Successfully Setup", True)
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            If ddlAgents.Value <> "" Then
                bindData()
                BindRoundRobin()
            Else
                txtMaxSelection.Text = ""
            End If

        End Sub

        Protected Sub btnSaveRoundRobin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveRoundRobin.Click
            Dim oSysSettingManager As New CSysSettingManager()
                If oSysSettingManager.IsExistedKey(_sRoundRobinCategory, "", getRoundRobinKey) Then
                    oSysSettingManager.UpdateValue(_sRoundRobinCategory, "", getRoundRobinKey, chkRoundRobin.Checked)
                Else
                    oSysSettingManager.AddSysSetting(_sRoundRobinCategory, getRoundRobinKey, chkRoundRobin.Checked, "", 0)
                End If
            UserSession.Cache.ClearSysSettings(_sRoundRobinCategory)
            BindRoundRobin()
            ClientAlert("Save Successfully", True)
        End Sub
    End Class
End Namespace