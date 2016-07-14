Imports SBCBL.std
Namespace SBCWebsite

    Partial Class Inc_Login
        Inherits UserControl

        Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
            If Not Page.IsValid Then
                Return
            End If

            Dim sLogin As String = ConvertStringToHex(SafeString(txtLogin.Text))
            Dim sPassword As String = ConvertStringToHex(SafeString(txtPassword.Text))
            Dim sautologinid As String = sLogin & "_" & sPassword
            Response.Redirect("login.aspx?autologinid=" & sautologinid)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                txtLogin.Focus()
            End If
        End Sub
    End Class
End Namespace
