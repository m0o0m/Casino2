Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class ManualPropPosition
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Update Proposition"
            Me.SideMenuTabName = "MANUAL_PROPOSITION"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Update Proposition")
        End Sub

    End Class
End Namespace