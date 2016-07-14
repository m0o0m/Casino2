Imports SBCBL
Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class SuperManager
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

        Public Property SiteType() As CEnums.ESiteType
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As CEnums.ESiteType)
                ViewState("SiteType") = value
            End Set
        End Property
#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucSuperEdit.SiteType = SiteType
                bindSuper()
            End If
        End Sub

#End Region

        Private Sub bindSuper()
            Dim oSuperManager As New CSuperUserManager
            dgSuper.DataSource = oSuperManager.GetSuperAdmins(SBCBL.std.GetSiteType, Me.ViewLock)
            dgSuper.DataBind()
        End Sub

        Protected Sub dgSuper_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgSuper.ItemCommand
            Dim sSuperID As String = SafeString(e.CommandArgument).Split("|"c)(0)
            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                    If Not ucSuperEdit.LoadSuperInfo(sSuperID) Then
                        ClientAlert("Can't Load Super", True)
                        Return
                    End If

                    dgSuper.SelectedIndex = e.Item.ItemIndex
            End Select
        End Sub

        Protected Sub ucSuperEdit_ButtonClick(ByVal sButtonType As String) Handles ucSuperEdit.ButtonClick
            dgSuper.SelectedIndex = -1
            ucSuperEdit.ClearSuperInfo()

            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting ", True)

                Case "SAVE SUCCESSFUL"

                    bindSuper()
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock
            dgSuper.SelectedIndex = -1
            bindSuper()
        End Sub

        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            dgSuper.SelectedIndex = -1
            bindSuper()
        End Sub

        Private Function getSqlSuperIDs() As List(Of String)
            Dim oSqlSuperIDs As New List(Of String)

            For Each oItem As DataGridItem In dgSuper.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oSqlSuperIDs.Add(SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument))
                End If
            Next

            Return oSqlSuperIDs
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oSqlSuperAdminIDs As List(Of String) = getSqlSuperIDs()

            If oSqlSuperAdminIDs.Count = 0 Then
                ClientAlert("Select Admins To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CSuperUserManager).LockSuperUsers(oSqlSuperAdminIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgSuper.SelectedIndex = -1
                bindSuper()
            End If
        End Sub
        Protected Sub dgSuper_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgSuper.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim odata As DataRowView = CType(e.Item.DataItem, DataRowView)
                If SafeBoolean(odata("visible")) Then
                    e.Item.Visible = False
                End If
            End If
        End Sub
        'Protected Sub ddlManager_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlManager.SelectedIndexChanged
        '    bindSuper()
        'End Sub
    End Class

End Namespace