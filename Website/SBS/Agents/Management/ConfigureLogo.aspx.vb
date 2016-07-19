Imports SBCBL.std

Namespace SBSAgents
    Partial Class ConfigureLogo
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Upload Image"
            MenuTabName = "SYSTEM_MANAGEMENT"
            SubMenuActive = "CONFIG_LOGO"
            DisplaySubTitlle(Me.Master, "Upload Image")
        End Sub
    End Class
End Namespace


