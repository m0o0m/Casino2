Imports SBCBL.UI
Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Managers
Imports SBCBL.Security

Namespace SBSWebsite
    Partial Class SBS_Default
        Inherits System.Web.UI.Page
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'Dim oLogin As New CCustomizeLoginTemplateManager(Server, Request.Url.Host, phContent, Me.Page)
            'oLogin.GetLoginPageHTML(Server.MapPath("/"))
            ucLogin.ChangeUser()
            ucLogin.BPlayer = False
            Page.Title = "Viet MVP"
        End Sub

        Private Sub execLogin(ByVal psUserName As String, ByVal psPassword As String)
            Dim sURL As String = ""
            Try
                log.Debug("Trying to log in: " & psUserName)

                Dim oLogin As New CLoginManager()

                sURL = oLogin.Login(psUserName, psPassword, WebsiteLibrary.CSBCStd.GetClientIP(), Request.Url.Host, True, False)
            Catch ex As Exception
                Return
            End Try

            If sURL <> "" Then
                redirect(psUserName, sURL)
            End If

        End Sub

        Private Sub redirect(ByVal psUsername As String, ByVal psURL As String)
            log.Debug(psUsername & " : Login successful.")
            FormsAuthentication.SetAuthCookie(psUsername, False)
            Response.Redirect(psURL, False)
        End Sub

        'Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        '    LoginControl1.ChangeUser()
        'End Sub
    End Class
End Namespace