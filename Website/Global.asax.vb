Imports Microsoft.VisualBasic
Imports log4net.Appender


Public Class Global_asax
    Inherits System.Web.HttpApplication

    Public Sub Application_Start(ByVal Sender As Object, ByVal E As EventArgs)
        'log4net.Config.XmlConfigurator.Configure()

        'Dim sIP As String = HttpContext.Current.Request.ServerVariables("LOCAL_ADDR").ToString
        'log4net.GlobalContext.Properties("IP") = sIP

        'If Array.IndexOf(New String() {"127.0.0.1"}, sIP) >= 0 Then
        '    For Each oAppender As log4net.Appender.IAppender In log4net.LogManager.GetRepository().GetAppenders()
        '        If oAppender.Name = "SmtpAppender" Then
        '            oAppender.Close()
        '        End If
        '    Next
        'End If

        'store to database.
    End Sub

	Public Sub Application_BeginRequest(ByVal sender As Object, ByVal E As EventArgs)
        ''-- Problem: when move ASP.NET applications to Integrated mode on IIS 7.0. 'Request is not available in this context' exception occur in Application_Start event.
        ''-- Workaround: moves first-request initialization from Application_Start to Application_BeginRequest
        ''-- Look at URL: http://mvolo.com/blogs/serverside/archive/2007/11/10/Integrated-mode-Request-is-not-available-in-this-context-in-Application_5F00_Start.aspx

        Dim oHttpApp As HttpApplication = CType(sender, HttpApplication)
        FirstRequestInitialization.Initialize(oHttpApp.Context)
		
    End Sub
	
    Protected Sub Global_asax_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
        Dim oErr As Exception = Server.GetLastError()
        Dim log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        log.Error(oErr.Message, oErr)

        If (System.Configuration.ConfigurationManager.AppSettings("ENVIRONMENT") <> "PRODUCTION") Then
            Return
        End If

        Dim oMail As New System.Net.Mail.MailMessage(SBCBL.std.SMTP_FROM, SBCBL.std.SYSTEM_EMAIL, oErr.Message, BuildMessage(oErr))
        oMail.IsBodyHtml = True
        SBCBL.std.SendEmail(oMail)

    End Sub

    Protected Sub Global_asax_PostAuthenticateRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PostAuthenticateRequest
        If Request.IsAuthenticated Then
            HttpContext.Current.User = New System.Security.Principal.GenericPrincipal(User.Identity, SBCBL.Security.CLoginManager.GetRoles(User.Identity.Name))
        End If
    End Sub

    Private Function BuildMessage(ByVal poException As Exception) As String
        Dim strMessage As New System.Text.StringBuilder()

        strMessage.Append("<style type=""text/css"">")
        strMessage.Append("<!--")
        strMessage.Append(".basix {")
        strMessage.Append("font-family: Verdana, Arial, Helvetica, sans-serif")
        strMessage.Append("font-size: 12px")
        strMessage.Append("}")
        strMessage.Append(".header1 {")
        strMessage.Append("font-family: Verdana, Arial, Helvetica, sans-serif")
        strMessage.Append("font-size: 12px")
        strMessage.Append("font-weight: bold")
        strMessage.Append("color: #000099")
        strMessage.Append("}")
        strMessage.Append(".tlbbkground1 {")
        strMessage.Append("background-color: #000099")
        strMessage.Append("}")
        strMessage.Append("-->")
        strMessage.Append("</style>")

        strMessage.Append("<table width=""85%"" border=""0"" align=""center"" cellpadding=""5"" cellspacing=""1"" class=""tlbbkground1"">")
        strMessage.Append("<tr bgcolor=""#eeeeee"">")
        strMessage.Append("<td colspan=""2"" class=""header1"">Page Error</td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>IP Address</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix"">" & Request.ServerVariables("REMOTE_ADDR") & "</td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User Agent</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix"">" & Request.ServerVariables("HTTP_USER_AGENT") & "</td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Page</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix"">" & Request.Url.AbsoluteUri & "</td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Time</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix"">" & System.DateTime.Now & " PST</td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix"">" & User.Identity.Name & " </td>")
        strMessage.Append("</tr>")
        strMessage.Append("<tr>")
        strMessage.Append("<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Details</td>")
        strMessage.Append("<td bgcolor=""#FFFFFF"" class=""basix""><pre>" & poException.ToString() & "</pre></td>")
        strMessage.Append("</tr>")
        strMessage.Append("</table>")

        Return strMessage.ToString()
    End Function

End Class

''-- URL: http://mvolo.com/blogs/serverside/archive/2007/11/10/Integrated-mode-Request-is-not-available-in-this-context-in-Application_5F00_Start.aspx
Public Class FirstRequestInitialization

    Private Shared bInitializedAlready As Boolean = False
    Private Shared oInitializedLock As New Object

    'Initialize only on the first request
    Public Shared Sub Initialize(ByVal poContext As HttpContext)
        If bInitializedAlready Then
            Return
        End If

        SyncLock oInitializedLock
            If Not bInitializedAlready Then
                bInitializedAlready = True

                ''Log4View
                log4net.Config.XmlConfigurator.Configure()

                Dim sIP As String = poContext.Request.ServerVariables("LOCAL_ADDR").ToString
                log4net.GlobalContext.Properties("IP") = sIP

                If Array.IndexOf(New String() {"127.0.0.1"}, sIP) >= 0 Then
                    For Each oAppender As log4net.Appender.IAppender In log4net.LogManager.GetRepository().GetAppenders()
                        If oAppender.Name = "SmtpAppender" Then
                            oAppender.Close()
                        End If
                    Next
                End If
            End If
        End SyncLock
    End Sub

End Class