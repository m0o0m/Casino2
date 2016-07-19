Imports SBCBL.std

Namespace SBSAgents
    Partial Class SiteOption
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Site Option"
            MenuTabName = "SYSTEM_MANAGEMENT"
            SubMenuActive = "SITE_OPTION"
            DisplaySubTitlle(Me.Master, "Site Option")
        End Sub
    End Class
End Namespace