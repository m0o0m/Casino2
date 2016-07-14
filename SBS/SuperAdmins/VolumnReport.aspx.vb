Imports SBCBL.std

Partial Class SBS_SuperAdmins_VolumnReport
    Inherits SBCBL.UI.CSBCPage


    Protected Sub Page_Init1(sender As Object, e As EventArgs) Handles Me.Init
        PageTitle = "Volumn Report"
        SideMenuTabName = "VOLUMN_REPORT"
        Me.MenuTabName = "VOLUMNREPORT"
        DisplaySubTitlle(Me.Master, "Volumn Report")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

End Class
