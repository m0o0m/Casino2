
Imports SBCBL.Managers
Imports SBCBL.UI

Partial Class SBS_Agents_Layout_breadCrumbAgent
    Inherits SBCBL.UI.CSBCUserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Dim userSession As New CSBCSession()
            Dim nAgentBalance As Double = 0

            Dim oAgentManager As New CAgentManager()
            nAgentBalance = oAgentManager.GetAgentsBalance(userSession.AgentUserInfo.UserID)
            lblAgentBalance.Text = FormatCurrency(nAgentBalance)

            Dim wcManager As New CWeeklyChargeManager()
            ltrRentalBalance.Text = FormatCurrency(wcManager.GetTotalWeeklyUnpaidChargeByAgent(userSession.AgentUserInfo.UserID))

        End If
    End Sub

End Class
