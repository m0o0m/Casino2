Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Partial Class SBS_Agents_Management_Inc_GameCircledSettings
    Inherits SBCBL.UI.CSBCUserControl
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

#End Region

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            trAgents.Visible = Me.IsSuperAdmin
        End If
    End Sub

#End Region

#Region "Controls Events"

    Protected Sub btnSaveCircled_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveCircled.Click
        Dim sAgentID As String = ""
        If Me.IsSuperAdmin Then
            sAgentID = SafeString(ddlAgents.SelectedValue)
        Else
            sAgentID = UserSession.UserID
        End If

        If sAgentID <> "" Then
            UpdateCirCleSettings(sAgentID)
        End If

    End Sub

    Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
        GetCircleSettings(SafeString(ddlAgents.SelectedValue))
    End Sub

#End Region

#Region "Functions"

    Public Sub bindAgents(ByVal psSuperAdminID As String)
        Dim oAgentManager As New CAgentManager()
        Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperAdminID, Nothing)

        ddlAgents.DataSource = oData
        ddlAgents.DataTextField = "FullName"
        ddlAgents.DataValueField = "AgentID"
        ddlAgents.DataBind()
    End Sub

    Public Sub GetCircleSettings(ByVal psAgentID As String)
        'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        'Dim odtCircleGame As DataTable = oGameTypeOnOffManager.GetCircleGameValues(psAgentID & "")
        'If odtCircleGame.Rows.Count > 0 Then
        '    txtStraight.Text = SafeString(odtCircleGame.Rows(0)(1))
        '    txtPnR.Text = SafeString(odtCircleGame.Rows(0)(2))
        'Else
        '    ClearCircleSettings()
        'End If
        txtStraight.Text = UserSession.Cache.GetSysSettings(psAgentID & "CircleSettings").GetIntegerValue("Straight")
        txtPnR.Text = UserSession.Cache.GetSysSettings(psAgentID & "CircleSettings").GetIntegerValue("ParlayReverse")
    End Sub

    Public Sub UpdateCirCleSettings(ByVal psAgentID As String)
        Try
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            Dim nStraight As Integer = SafeInteger(txtStraight.Text)
            Dim nPnR As Integer = SafeInteger(txtPnR.Text)
            Dim bSuccess As Boolean = False

            If nStraight <= 0 Then
                ClientAlert("Straight Is Not Valid ", True)
                txtStraight.Focus()
                Return
            End If
            If nPnR <= 0 Then
                ClientAlert("Parlay & Reverse Is Not Valid ", True)
                txtPnR.Focus()
                Return
            End If

            'If oGameTypeOnOffManager.GetCircleGame(psAgentID) > 0 Then
            '    bSuccess = oGameTypeOnOffManager.UpdateCircleGame(psAgentID, nStraight, nPnR)
            'Else
            '    bSuccess = oGameTypeOnOffManager.InsertGameCircle(psAgentID, newGUID, nStraight, nPnR)
            'End If
            If oSysManager.CheckExistSysSetting(psAgentID & "CircleSettings", "", "Straight") Then
                bSuccess = oSysManager.UpdateValue(psAgentID & "CircleSettings", "", "Straight", nStraight)
                bSuccess = oSysManager.UpdateValue(psAgentID & "CircleSettings", "", "ParlayReverse", nPnR)
            Else
                bSuccess = oSysManager.AddSysSetting(psAgentID & "CircleSettings", "Straight", nStraight)
                bSuccess = oSysManager.AddSysSetting(psAgentID & "CircleSettings", "", "ParlayReverse", nPnR)

            End If
            UserSession.Cache.ClearSysSettings(psAgentID & "CircleSettings")

            If bSuccess Then
                ClientAlert("Successfully Saved ", True)
            Else
                ClientAlert("Failed to Save Setting", True)
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub ClearCircleSettings()
        txtStraight.Text = ""
        txtPnR.Text = ""
    End Sub

#End Region

End Class
