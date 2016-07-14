Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class InboxMail
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Inbox Mail"
            Me.SideMenuTabName = "INBOX_MAIL"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Inbox Mail")
        End Sub

    End Class
End Namespace
