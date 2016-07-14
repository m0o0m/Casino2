
Imports SBCBL.CacheUtils
Imports SBCBL.UI

Partial Class SBS_Players_Layout_Layout4_headerTopBarLayout4
    Inherits SBCBL.UI.CSBCUserControl


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            If Session("BackAgentID") IsNot Nothing Then
                liBackToAgent.Visible = True
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        LoadLogo()
    End Sub

    Protected Sub LoadLogo()
        Dim userSession As New CSBCSession()
        Try
            lblUser.Text = userSession.PlayerUserInfo.Name
        Catch ex As Exception
        End Try

        Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
        Try
            oWhiteLabel = oWhiteLabel.LoadByUrl(Request.Url.Host)
            If oWhiteLabel IsNot Nothing Then
                imgLogo.Src = oWhiteLabel.LogoFileName
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnBacktoAgent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBacktoAgent.ServerClick
        Try
            Dim oCache As New CCacheManager()
            FormsAuthentication.SetAuthCookie(Session("BackAgentName").ToString(), True)
            Session("USER_ID") = Session("BackAgentID")
            Session("AGENT_SELECT_ID") = Session("BackAgentID")
            Session("USER_TYPE") = SBCBL.EUserType.Agent
            oCache.ClearAgentInfo(Session("BackAgentID"), Session("BackAgentName"))
            Response.Redirect("/SBS/Agents/SelectPlayers.aspx", False)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.ServerClick
        Dim oLogin As New SBCBL.Security.CLoginManager()
        oLogin.LogOut()
    End Sub

End Class
