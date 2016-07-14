Imports System.Data
Imports SBCBL.std
Imports SBCBL.Managers

Namespace SBSSuperAdmin
    Partial Class BookMakerSetup
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Agent Bookmaker Setup"
            SideMenuTabName = "BOOKMAKER_SETUP"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Agent Bookmaker Setup")
        End Sub
    End Class
End Namespace
