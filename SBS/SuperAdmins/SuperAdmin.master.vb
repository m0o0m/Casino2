Imports SBCBL.UI

Namespace SBSSuperAdmin
    Partial Class SuperAdmin
        Inherits System.Web.UI.MasterPage

        Dim defaultConyRight As String = ""

        Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.Click
            Dim oLogin As New SBCBL.Security.CLoginManager()
            oLogin.LogOut()
        End Sub

        Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
            Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
            oSetting = oSetting.LoadByUrl(Request.Url.Host)

            If oSetting IsNot Nothing Then
                If Not String.IsNullOrWhiteSpace(oSetting.CopyrightName) Then
                    defaultConyRight = oSetting.CopyrightName
                End If
            End If

            ltrCopyRight.Text = defaultConyRight
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                'Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
                'Dim oUserSession As New CSBCSession()

                'If Not String.IsNullOrEmpty(oUserSession.SuperAdminInfo.SiteURL) AndAlso Not Request.Url.Host.Equals(oUserSession.SuperAdminInfo.SiteURL,StringComparison.CurrentCultureIgnoreCase) Then
                '    Response.Redirect("/sbs/default.aspx")
                '    'Response.Write(oUserSession.SuperAdminInfo.SiteURL)
                'End If

                Dim oCache As New SBCBL.CacheUtils.CCacheManager()
                ucImpersonateUser.Visible = oCache.GetSysSettings("ImpersonateUser").GetBooleanValue("Impersonate")
            End If
            If New SBCBL.UI.CSBCSession().SuperAdminInfo.IsPartner Then
                ucTopMenu.Visible = False
                ucPartnerTopMenu.Visible = True
            End If
        End Sub

        Protected Sub LoadLogo()
            'Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
            'oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            'If oWhiteLabel IsNot Nothing Then
            '    imgLogo.Src = oWhiteLabel.LogoFileName
            'End If
            imgLogo.Visible = False
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            LoadLogo()

            Dim userSession As New CSBCSession()
            lbnIPAlert.Visible = userSession.HasIPAlert

            'Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
            'oSetting = oSetting.LoadByUrl(Request.Url.Host)

            'Dim colorTheme As String = CType(If(oSetting IsNot Nothing AndAlso Not String.IsNullOrEmpty(oSetting.ColorScheme), oSetting.ColorScheme.ToLower(), ""), String)

            'Dim newThemeNames = New String() {"layout1", "layout4", "layout5"}
            'If oSetting IsNot Nothing AndAlso newThemeNames.Contains(colorTheme) Then
            '    colorTheme = ""
            'End If

            'If oSetting IsNot Nothing AndAlso colorTheme <> "" Then
            '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", oSetting.ColorScheme)
            'Else
            '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", "BLUE")
            'End If
            ' ltrMenu.Text = "<input type='hidden' id='hfMenu' value='" & oSetting.ColorScheme & "' />"
            'If oSetting.BackgroundImage <> "" Then
            '    divMain.Style.Add("background-image", "'" & oSetting.BackgroundImage & "'")
            'End If
            LoadLogo()
            'Dim userSession As New CSBCSession()
            'lbnIPAlert.Visible = userSession.HasIPAlert

        End Sub

        Protected Sub lbnIPAlert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnIPAlert.Click
            Response.Redirect("/SBS/SuperAdmins/IPAlert.aspx")
        End Sub

    End Class

End Namespace