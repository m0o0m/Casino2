Imports SBCBL.std

Namespace SBSAgents
    Partial Class IPReports
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "IP Reports"
            MenuTabName = "REPORT"
            SubMenuActive = "IP_REPORTS"
            DisplaySubTitlle(Me.Master, "IP Reports")
        End Sub

    End Class
End Namespace