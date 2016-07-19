Imports SBCBL.UI
Imports SBCBL.CacheUtils
Imports SBCBL.Managers

Partial Class SBS_Agents_Agent
    Inherits System.Web.UI.MasterPage

    Dim defaultConyRight As String = ""

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim userSession As New CSBCSession()
        Dim oSetting As New SBCBL.CacheUtils.CWhiteLabelSettings()
        oSetting = oSetting.LoadByUrl(Request.Url.Host)
        If oSetting IsNot Nothing AndAlso Not String.IsNullOrEmpty(oSetting.ColorScheme) Then
            'lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", oSetting.ColorScheme)

            If Not String.IsNullOrWhiteSpace(oSetting.CopyrightName) Then
                defaultConyRight = oSetting.CopyrightName
            End If
        Else
            'lnkStyle.Href = String.Format("/SBS/Inc/Styles/{0}.css", "BLACK")
        End If
       
        LoadLogo()
     
        lbnIPAlert.Visible = userSession.HasIPAlert

        ltrCopyRight.Text = defaultConyRight

    End Sub

    Protected Sub LoadLogo()
        Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
        If oWhiteLabel IsNot Nothing Then
            imgLogo.Src = oWhiteLabel.LogoFileName
        End If
    End Sub

    Protected Sub lbnIPAlert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnIPAlert.Click
        Response.Redirect("/SBS/Agents/Management/IPAlert.aspx")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim userSession As New CSBCSession()
            Session("BackAgentID") = Nothing
            Session("BackAgentName") = Nothing
            Dim nAgentBalance As Double = 0
            Dim oAgentManager As New CAgentManager()
            nAgentBalance = oAgentManager.GetAgentsBalance(userSession.AgentUserInfo.UserID)
        End If
    End Sub
End Class

