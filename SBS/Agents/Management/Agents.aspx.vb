Imports SBCBL.std

Namespace SBSAgents
    Partial Class Agents
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Agents"
            MenuTabName = "USER_MANAGEMENT"
            SubMenuActive = "AGENTS"
            DisplaySubTitlle(Me.Master, "Agents")
        End Sub
    End Class
End Namespace