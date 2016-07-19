Imports SBCBL.CacheUtils
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class ComposeMail
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Compose Mail"
            Me.SideMenuTabName = "COMPOSE_MAIL"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Compose Mail")
        End Sub
    End Class
End Namespace
