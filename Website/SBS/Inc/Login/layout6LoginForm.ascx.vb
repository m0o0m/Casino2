Imports SBCBL.std
Imports SBCBL.Security

Partial Class SBS_Inc_Login_layout6LoginForm
    Inherits System.Web.UI.UserControl

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Protected Sub Login1_Authenticate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.AuthenticateEventArgs) Handles Login1.Authenticate
        ' enable cookie in iframe, reference: http://aspnetresources.com/blog/frames_webforms_and_rejected_cookies.aspx 
        HttpContext.Current.Response.AddHeader("p3p", "CP=""IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT""")
        execLogin(Login1.UserName, Login1.Password)
    End Sub

    Private Sub execLogin(ByVal psUserName As String, ByVal psPassword As String)


        Dim sURL As String = ""
        Try
            log.Debug("Trying to log in: " & psUserName)
            'Dim oUserType As SBCBL.CacheUtils.EUserType
            Dim oLogin As New CLoginManager()
            sURL = oLogin.Login(psUserName, psPassword, WebsiteLibrary.CSBCStd.GetClientIP(), Request.Url.Host, False, True)

        Catch ex As CSecurityException
            Login1.FindControl("FailureText").Visible = True
            Login1.FailureText = ex.Message
            log.Info("cannot login: " & ex.Message, ex)
            'Login1.FindControl("lbnForgotPassword").Visible = True

            Return
        Catch ex As Exception
            Login1.FindControl("FailureText").Visible = True
            ' Login1.FailureText = "Sorry we are unable to log you in at this time. Please check your username and password and try again."
            Login1.FailureText = "Login error.<br/> Please check your username/password."
            log.Error("Unexpected error cannot login: " & ex.Message, ex)

            'Login1.FindControl("lbnForgotPassword").Visible = True

            Return
        End Try
        If sURL <> "" Then
            redirect(psUserName, sURL)
        End If
    End Sub

    Private Sub redirect(ByVal psUsername As String, ByVal psURL As String)
        log.Debug(psUsername & " : Login successful.")

        FormsAuthentication.SetAuthCookie(psUsername, False)
        Login1.Enabled = True
        Response.Redirect(psURL, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' DisplayMobile()
            Login1.FindControl("UserName").Focus()

            If Not Request("autologinid") Is Nothing Then
                Try
                    Dim sAutoLoginID As String = SafeString(Request("autologinid"))
                    If sAutoLoginID.Contains("_") Then
                        Dim sLogin As String = Trim(ConvertHexToString(sAutoLoginID.Split("_"c)(0)))
                        Dim sPassword As String = Trim(ConvertHexToString(sAutoLoginID.Split("_"c)(1)))

                        execLogin(sLogin, sPassword)
                    End If
                Catch ex As Exception
                    log.Error("AutoLogin Error :" & ex.Message, ex)
                End Try
            End If
        End If

        LoadLogo()
    End Sub

    Protected Sub LoadLogo()
        Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        Try
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then

                If Not String.IsNullOrEmpty(oWhiteLabel.LoginLogo) Then
                    imgLogo.Src = oWhiteLabel.LoginLogo
                    imgLogo.Visible = True
                    ltrLogoName.Visible = False
                End If

                If Not String.IsNullOrEmpty(oWhiteLabel.CopyrightName) Then
                    ltrCopywright.Text = oWhiteLabel.CopyrightName
                End If

            End If

        Catch ex As Exception
        End Try
    End Sub

    Public Sub ChangeUser()
        CType(Login1.FindControl("UserNameLabel"), Label).Text = "Agent"
    End Sub

End Class
