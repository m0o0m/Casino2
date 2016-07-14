Imports SBCBL.std

Namespace SBSSuperAdmin

    Partial Class SysSettings
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "System Categories"
            Me.SideMenuTabName = "SYSTEM_SETTINGS"
            Me.MenuTabName = "SYSTEM MANAGEMENT"
            DisplaySubTitlle(Me.Master, "System Categories")
        End Sub
    End Class
End Namespace

