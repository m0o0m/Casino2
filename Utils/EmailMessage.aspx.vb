Imports SBCBL.Managers
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data
Partial Class Utils_EmailMessage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim oEmailsManager As CEmailsManager = New CEmailsManager()
            Dim odtEmail As DataTable = oEmailsManager.GetEmailsByEmailID(Request.QueryString("MailId"))
            lblMessage.Text = odtEmail.Rows(0)("Messages")
        End If
    End Sub
End Class
