Imports SBCBL.Security
Imports SBCBL.UI
Imports SBCBL.std
Imports SBCBL.CFileDBKeys
Imports SBCBL.CacheUtils

Namespace SBCWebsite
    Partial Class Login
        Inherits System.Web.UI.Page

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Private Sub execLogin(ByVal psUserName As String, ByVal psPassword As String)
            Dim sURL As String = ""
            Try
                log.Debug("Trying to log in: " & psUserName)

                Dim oLogin As New CLoginManager()

                sURL = oLogin.Login(psUserName, psPassword, WebsiteLibrary.CSBCStd.GetClientIP(), Request.Url.Host, False, False)
            Catch ex As Exception
                Return
            End Try

            If sURL <> "" Then
                redirect(psUserName, sURL)
            End If

        End Sub


        Private Sub redirect(ByVal psUsername As String, ByVal psURL As String)
            log.Debug(psUsername & " : Login successful.")
            FormsAuthentication.SetAuthCookie(psUsername, True)
            Response.Redirect(psURL, False)
        End Sub
    End Class
End Namespace