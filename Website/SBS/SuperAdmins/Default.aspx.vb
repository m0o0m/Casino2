Namespace SBSSuperAdmin

    Partial Class _Default
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Super Admin"
            SideMenuTabName = "HOME"
            Response.Redirect("SysSettings.aspx")
        End Sub

    End Class


End Namespace
