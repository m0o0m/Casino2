Imports SBCBL.std

Namespace SBSAgents

    Partial Class AgentAccount
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Account Status"
            SubMenuActive = "ACCOUNT_STATUS"
            MenuTabName = "ACCOUNT_STATUS"
            DisplaySubTitlle(Me.Master, "Account Status")
        End Sub
    End Class
End Namespace

