Imports SBCBL.UI
Imports SBCBL.CacheUtils
Imports SBCBL.std

Partial Class SBS_Inc_Login_defaultLoginForm
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim oLogin As New CCustomizeLoginTemplateManager(Server, Request.Url.Host, phContent, Me.Page)
        'oLogin.GetLoginPageHTML(Server.MapPath("/"))
        ' Page.Title = "Viet MVP"

        ucLogin2.BPlayer = True
        ucLogin.BPlayer = True
        LoadLogo()
    End Sub

    Protected Sub LoadLogo()
        Dim userSession As New CSBCSession()
        Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        Try
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                imgLogo.Src = oWhiteLabel.LoginLogo
                If Not String.IsNullOrEmpty(oWhiteLabel.BackgroundLoginImage) Then
                    Panel2.Style.Add("Background", "url(" & oWhiteLabel.BackgroundLoginImage & ") no-repeat center top black")
                    Panel2.Visible = True
                    Panel1.Visible = False
                    lblBackupURL.Text = "Backup URL : " & SafeString(oWhiteLabel.BackupURL)
                    lblCustomerService.Text = "Customer Service :" & SafeString(oWhiteLabel.SuperAgentPhone)

                Else
                    lblBackupURL2.Text = "Backup URL : " & SafeString(oWhiteLabel.BackupURL)
                    lblCustomerService2.Text = "Customer Service :" & SafeString(oWhiteLabel.SuperAgentPhone)
                    Panel1.Visible = True
                    Panel2.Visible = False
                End If
            End If

        Catch ex As Exception
        End Try
    End Sub

End Class
