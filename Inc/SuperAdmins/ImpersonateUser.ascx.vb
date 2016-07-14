Imports SBCBL.Security
Imports SBCBL.std

Namespace SBCSuperAdmins
    Partial Class ImpersonateUser
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub txtImpersonateUser_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtImpersonateUser.TextChanged
            Dim oLogin As New CLoginManager()
            Dim sUsername As String = SafeString(txtImpersonateUser.Text)

            If Not ValidLogin(sUsername) Then
                Page.ClientScript.RegisterStartupScript(Page.GetType, "Invalid User", "<script language=javascript>alert('Unable to find the user');</script>")
                Exit Sub
            End If

            Dim sURL As String
            Try
                sURL = oLogin.Login(sUsername, "", "", "", True, True, True)
                If sURL <> "" Then
                    FormsAuthentication.SetAuthCookie(sUsername, False)
                    If Request.IsAuthenticated Then
                        HttpContext.Current.User = New System.Security.Principal.GenericPrincipal(Page.User.Identity, CLoginManager.GetRoles(Page.User.Identity.Name))
                    End If
                    Response.Redirect(sURL)
                Else
                    Page.ClientScript.RegisterStartupScript(Page.GetType, "Login Failed", "<script language=javascript>alert('Redirect URL is empty.');</script>")
                End If
            Catch ex As Exception
                Page.ClientScript.RegisterStartupScript(Page.GetType, "Login Failed", "<script language=javascript>alert('" & ex.Message & "');</script>")
                Return
            End Try
        End Sub
    End Class
End Namespace