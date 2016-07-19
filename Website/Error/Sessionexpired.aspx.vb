
Partial Class Sessionexpired
    Inherits System.Web.UI.Page

    Protected Sub btnGoToLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGoToLogin.Click
        Response.Redirect("/")
    End Sub
End Class
