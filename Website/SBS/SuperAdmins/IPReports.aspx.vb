Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class IPReports
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "IP Reports"
            Me.SideMenuTabName = "IP_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "IP Reports")
        End Sub

    End Class
End Namespace