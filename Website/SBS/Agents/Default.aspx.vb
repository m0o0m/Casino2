Namespace SBSAgent
    Partial Class SBS_Agents_Default
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If UserSession.AgentUserInfo.RequirePasswordChange Then '' Redirect to change password page
                Response.Redirect("AccountStatus.aspx?RequireChangePass=Y")
            End If

            PageTitle = "Home"
            MenuTabName = "HOME"
        End Sub
    End Class
End Namespace
