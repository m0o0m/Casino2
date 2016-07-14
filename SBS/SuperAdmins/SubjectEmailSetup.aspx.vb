Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class SubjectEmailSetup
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Email Subject Setup"
            Me.SideMenuTabName = "EMAIL_SUBJECT_SETUP"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Email Subject Setup")
        End Sub
    End Class
End Namespace
