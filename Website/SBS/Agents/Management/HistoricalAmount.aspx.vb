Imports SBCBL.std

Namespace SBCSuperAdmins
    Partial Class HistoricalAmount
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                setTabActive(SafeString(Request("tab"))) 'if request tab is empty, default Agent tab
            End If
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("HistoricalAmount.aspx?tab=" & CType(sender, LinkButton).CommandArgument & "&playerID= " & SafeString(Request.QueryString("playerID")))
        End Sub

        Private Sub setTabActive(ByVal psTabKey As String)
            ucHistoricalAmount.PlayerID = Request.QueryString("playerID")
            Select Case True
                Case UCase(psTabKey) = "YEARLY"
                    lbtYearly.CssClass = "selected"
                    ucHistoricalAmount.IsWeeklyReport = False
                    ucHistoricalAmount.IsYTD = False
                    ucHistoricalAmount.bindWeek()
                    ucHistoricalAmount.bindReport()

                Case UCase(psTabKey) = "YTD"
                    lbtYTD.CssClass = "selected"
                    ucHistoricalAmount.IsWeeklyReport = False
                    ucHistoricalAmount.IsYTD = True
                    ucHistoricalAmount.bindWeek()
                    ucHistoricalAmount.bindReport()
                Case Else
                    lbtTabWeekly.CssClass = "selected"
                    ucHistoricalAmount.IsWeeklyReport = True
                    ucHistoricalAmount.IsYTD = False
                    ucHistoricalAmount.bindWeek()
                    ucHistoricalAmount.bindReport()
            End Select
        End Sub
    End Class
End Namespace