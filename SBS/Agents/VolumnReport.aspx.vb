Imports SBCBL.std

Partial Class SBS_Agents_VolumnReport
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        PageTitle = "Volumn Report"
        SideMenuTabName = "VOLUMN_REPORT"
        Me.MenuTabName = "VOLUMNREPORT"
        DisplaySubTitlle(Me.Master, "Volumn Report")
    End Sub

End Class
