Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class SuperPositionReport
        Inherits SBCBL.UI.CSBCPage
        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Super Position Report"
            Me.SideMenuTabName = "SUPER_POSITION_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Super Position Report")
        End Sub

        Protected Sub lbtTab_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect("SuperPositionReport.aspx?tab=" & CType(sender, LinkButton).CommandArgument)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If SBCBL.std.SafeString(Request("tab")) = "DETAILS" Then
                tDetail.Attributes("class") = "active"
                ucAgentPositionReport.Visible = True
                ucNewAgentPositionReport.Visible = False
            Else
                ucNewAgentPositionReport.Visible = True
                ucAgentPositionReport.Visible = False
                tAll.Attributes("class") = "active"
            End If
        End Sub
    End Class
End Namespace