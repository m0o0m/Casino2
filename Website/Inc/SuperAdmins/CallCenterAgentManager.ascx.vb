Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class CallCenterAgentManager
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"

        Public Property ViewLock() As Boolean
            Get
                If ViewState("VIEW_LOCK") Is Nothing Then
                    ViewState("VIEW_LOCK") = False
                End If

                Return CBool(ViewState("VIEW_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_LOCK") = value

                Dim sMessage As String = SafeString(IIf(value, "Unlock", "Lock"))
                btnLock.Text = sMessage
                btnViewLock.Text = "View " & sMessage
            End Set
        End Property
        Public Property SiteType() As String
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As String)
                ViewState("SiteType") = value
            End Set
        End Property

#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgents()
                ucCCAgentEdit.SiteType = SiteType
            End If
        End Sub

#End Region

        Private Sub bindAgents()
            Dim oAgentManager As New CCallCenterAgentManager

            dgAgents.DataSource = oAgentManager.GetAgents(SBCBL.std.GetSiteType, Me.ViewLock, txtNameOrLogin.Text)
            dgAgents.DataBind()
        End Sub

        Protected Sub dgAgents_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgAgents.ItemCommand
            Dim sCallCenterAgentID As String = SafeString(e.CommandArgument)
            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                    If Not ucCCAgentEdit.LoadAgentInfo(sCallCenterAgentID) Then
                        ClientAlert("Can't Load Agent", True)
                        Return
                    End If

                    dgAgents.SelectedIndex = e.Item.ItemIndex
            End Select
        End Sub

        Protected Sub ucCCAgentEdit_ButtonClick(ByVal sButtonType As String) Handles ucCCAgentEdit.ButtonClick
            dgAgents.SelectedIndex = -1
            ucCCAgentEdit.ClearAgentInfo()

            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)

                Case "SAVE SUCCESSFUL"

                    bindAgents()
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock

            dgAgents.SelectedIndex = -1
            bindAgents()
        End Sub

        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            dgAgents.SelectedIndex = -1
            bindAgents()
        End Sub

        Private Function getSqlCallCenterAgentIDs() As List(Of String)
            Dim oSqlAgentIDs As New List(Of String)

            For Each oItem As DataGridItem In dgAgents.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oSqlAgentIDs.Add(SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument))
                End If
            Next

            Return oSqlAgentIDs
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oSqlCallCenterAgentIDs As List(Of String) = getSqlCallCenterAgentIDs()

            If oSqlCallCenterAgentIDs.Count = 0 Then
                ClientAlert("Select Agents To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CCallCenterAgentManager).LockAgents(oSqlCallCenterAgentIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgAgents.SelectedIndex = -1
                bindAgents()
            End If
        End Sub

    End Class

End Namespace