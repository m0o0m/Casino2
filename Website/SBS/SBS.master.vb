Imports SBCBL.UI
Imports SBCBL.CacheUtils
Imports SBCBL.std
Imports SBCBL.Managers
Imports SBCBL.Security

Namespace SBSWebsite
    Partial Class SBS_SBS
        Inherits System.Web.UI.MasterPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
            'oSetting = oSetting.LoadByUrl(Request.Url.Host)


            'If oSetting IsNot Nothing AndAlso oSetting.ColorScheme <> "" Then
            '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", oSetting.ColorScheme)
            '    ltrMenu.Text = "<input type='hidden' id='hfMenu' value='" & oSetting.ColorScheme & "'/>"
            '    lblcolor.Text = oSetting.ColorScheme
            'Else
            '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", "BLUE")
            'End If
            'LoadLogo()

        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'Dim oLogin As New CCustomizeLoginTemplateManager(Server, Request.Url.Host, phContent, Me.Page)
            'oLogin.GetLoginPageHTML(Server.MapPath("/"))
            ' Page.Title = "Viet MVP"

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
                  
                        lblBackupURL2.Text = "Backup URL : " & SafeString(oWhiteLabel.BackupURL)
                        lblCustomerService2.Text = "Customer Service :" & SafeString(oWhiteLabel.SuperAgentPhone)
                End If

            Catch ex As Exception
            End Try
        End Sub

        'Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        '    Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
        '    oSetting = oSetting.LoadByUrl(Request.Url.Host)

        '    ltrMenu.Text = "<input type='hidden' id='hfMenu' value='" & oSetting.ColorScheme & "'/>"
        '    If oSetting IsNot Nothing AndAlso oSetting.ColorScheme <> "" Then
        '        lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", oSetting.ColorScheme)
        '        lblcolor.Text = oSetting.ColorScheme
        '    Else
        '        lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", "BLUE")
        '    End If
        '    LoadLogo()


        'End Sub

        'Protected Sub LoadLogo()
        '    Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        '    oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
        '    If oWhiteLabel IsNot Nothing Then
        '        imgLogo.Src = oWhiteLabel.LogoFileName
        '    End If
        'End Sub
    End Class
End Namespace

