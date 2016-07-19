
Partial Class SBS_Agents_Agent
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
            oSetting = oSetting.LoadByUrl(Request.Url.Host)
            '  imgLogo.Visible = oSetting IsNot Nothing
            ' ucContentFileDB.LoadFileDBContent("CCAGENT_LETTER_TEAMPLATE")
        End If
    End Sub

    Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.Click
        Dim oLogin As New SBCBL.Security.CLoginManager()
        Dim oSBCPage = New SBCBL.UI.CSBCPage()
        oSBCPage.UserSession.CCAgentUserInfo.PlayerID = ""
        oLogin.LogOut()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'ucAgentTopMenu.Visible = CType(Page, SBCBL.UI.CSBCPage).UserSession.CCAgentUserInfo.PlayerID = ""
        'ucTopMenu.Visible = CType(Page, SBCBL.UI.CSBCPage).UserSession.CCAgentUserInfo.PlayerID <> ""
        'Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
        'oSetting = oSetting.LoadByUrl(Request.Url.Host)
        'If oSetting IsNot Nothing AndAlso oSetting.ColorScheme <> "" Then
        '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", oSetting.ColorScheme)
        '    ltrMenu.Text = "<input type='hidden' id='hfMenu' value='" & oSetting.ColorScheme & "' />"
        'Else
        '    lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", "BLUE")
        'End If
        ''If oSetting.BackgroundImage <> "" Then
        ''    divMain.Style.Add("background-image", "'" & oSetting.BackgroundImage & "'")
        ''End If
        'LoadLogo()
    End Sub

    Protected Sub LoadLogo()
        Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
        If oWhiteLabel IsNot Nothing Then
            '  imgLogo.Src = oWhiteLabel.LogoFileName
        End If
    End Sub

End Class

