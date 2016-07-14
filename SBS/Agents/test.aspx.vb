
Partial Class SBS_Agents_test
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ucSummaryReport.ReloadData()
        End If

    End Sub
End Class
