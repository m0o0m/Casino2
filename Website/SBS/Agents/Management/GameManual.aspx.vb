Imports SBCBL.std

Namespace SBSAgents
    Partial Class GameManual
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Game Manual"
            MenuTabName = "GAME_MANAGEMENT"
            SubMenuActive = "GAME_MANUAL"
            DisplaySubTitlle(Me.Master, "Quarter Line Setup")
        End Sub
    End Class
End Namespace
