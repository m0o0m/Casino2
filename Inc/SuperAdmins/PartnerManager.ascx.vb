Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class PartnerManager
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
                bindPartners()
                ucPartnerEdit.SiteType = SiteType
            End If
        End Sub
#End Region

        Private Sub bindPartners()
            Dim oPartnerManager As New CSuperUserManager()

            dgPartners.DataSource = oPartnerManager.GetPartners(SBCBL.std.GetSiteType, UserSession.UserID, Me.ViewLock)
            dgPartners.DataBind()
        End Sub

        Protected Sub dgAgents_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPartners.ItemCommand
            Dim sPartnerID As String = SafeString(e.CommandArgument)
            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                    If Not ucPartnerEdit.LoadPartnerInfo(sPartnerID) Then
                        ClientAlert("Can't Load Partner.", True)
                        Return
                    End If

                    dgPartners.SelectedIndex = e.Item.ItemIndex
            End Select
        End Sub

        Protected Sub ucPartnerEdit_ButtonClick(ByVal sButtonType As String) Handles ucPartnerEdit.ButtonClick
            dgPartners.SelectedIndex = -1
            ucPartnerEdit.ClearPartnerInfo()

            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)

                Case "SAVE SUCCESSFUL"

                    bindPartners()
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock

            dgPartners.SelectedIndex = -1
            bindPartners()
        End Sub

        Private Function getSqlPartnerIDs() As List(Of String)
            Dim oSqlPartnerIDs As New List(Of String)

            For Each oItem As DataGridItem In dgPartners.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oSqlPartnerIDs.Add(SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument))
                End If
            Next

            Return oSqlPartnerIDs
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oSqlSuperAdminIDs As List(Of String) = getSqlPartnerIDs()

            If oSqlSuperAdminIDs.Count = 0 Then
                ClientAlert("Select Partner To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CSuperUserManager).LockSuperUsers(oSqlSuperAdminIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgPartners.SelectedIndex = -1
                bindPartners()
            End If
        End Sub

    End Class

End Namespace