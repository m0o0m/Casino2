Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class PLReports
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "P/L Reports"
            Me.SideMenuTabName = "PL_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "P/L Reports")
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)

            Select Case True
                Case UCase(psTabKey) = "WEEKLY"
                    tWeekly.Attributes("class") = "active"
                    ucPLReport.IsDailyReport = False
                    ucPLReport.IsYTD = False
                    ucPLReport.bindReport()

                Case UCase(psTabKey) = "YTD"
                    tYTD.Attributes("class") = "active"
                    ucPLReport.IsDailyReport = False
                    ucPLReport.IsYTD = True
                    ucPLReport.bindReport()
                Case Else
                    tDaily.Attributes("class") = "active"
                    ucPLReport.IsDailyReport = True
                    ucPLReport.IsYTD = False
                    ucPLReport.bindReport()
            End Select
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            'setTabActive(CType(sender, LinkButton).CommandArgument)
            Response.Redirect("PLReports.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

    End Class
End Namespace