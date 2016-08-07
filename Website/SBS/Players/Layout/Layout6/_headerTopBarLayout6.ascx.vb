
Imports SBCBL.CacheUtils
Imports SBCBL.UI
Imports SBCBL.std
Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports SBCBL.Managers

Partial Class SBS_Players_Layout_Layout6_headerTopBarLayout6
    Inherits SBCBL.UI.CSBCUserControl


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            If Session("BackAgentID") IsNot Nothing Then
                'liBackToAgent.Visible = True
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        LoadLogo()
        LoadAccountStatus()
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

    'Protected Sub btnBacktoAgent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBacktoAgent.ServerClick
    '    Try
    '        Dim oCache As New CCacheManager()
    '        FormsAuthentication.SetAuthCookie(Session("BackAgentName").ToString(), True)
    '        Session("USER_ID") = Session("BackAgentID")
    '        Session("AGENT_SELECT_ID") = Session("BackAgentID")
    '        Session("USER_TYPE") = SBCBL.EUserType.Agent
    '        oCache.ClearAgentInfo(Session("BackAgentID"), Session("BackAgentName"))
    '        Response.Redirect("/SBS/Agents/SelectPlayers.aspx", False)
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.ServerClick
        Dim oLogin As New SBCBL.Security.CLoginManager()
        oLogin.LogOut()
    End Sub


    Private Sub LoadAccountStatus()

        Dim pendingAmt = SafeRound(UserSession.PlayerUserInfo.PendingAmount)
        lblPendingAmount.Text = pendingAmt.ToString("N", CultureInfo.InvariantCulture)
        If pendingAmt < 0 Then
            lblPendingAmount.ForeColor = Color.Red
        End If

        Dim avaiBalance = SafeRound(UserSession.PlayerUserInfo.BalanceAmount)
        lblAvailableBalance.Text = avaiBalance.ToString("N", CultureInfo.InvariantCulture)
        If avaiBalance < 0 Then
            lblAvailableBalance.ForeColor = Color.Red
        End If

        Dim oDate As Date = SBCBL.std.GetEasternMondayOfCurrentWeek()

        Dim oTickets As DataTable = (New CPlayerManager()).GetPlayerDashboard(UserSession.UserID, oDate, oDate.AddDays(6), UserSession.PlayerUserInfo.TimeZone)
        If (oTickets.Rows.Count > 0) Then
            Dim tWeekAmt = SafeRound(oTickets.Rows(0)("Net"))
            lblThisWeek.Text = tWeekAmt.ToString("N", CultureInfo.InvariantCulture)

            If tWeekAmt < 0 Then
                lblThisWeek.ForeColor = Color.Red
            End If
        End If


    End Sub

End Class
