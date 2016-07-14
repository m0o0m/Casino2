
Imports SBCBL.UI

Partial Class SBS_Agents_Layout_headerTopBarAgent
    Inherits SBCBL.UI.CSBCUserControl

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim userSession As New CSBCSession()

        If userSession IsNot Nothing And userSession.AgentUserInfo IsNot Nothing And userSession.AgentUserInfo.Login IsNot Nothing Then
            lblLoginId.Text = userSession.AgentUserInfo.Login
        End If

    End Sub

    Protected Sub lbnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbnLogout.Click
        Dim oLogin As New SBCBL.Security.CLoginManager()
        oLogin.LogOut()
    End Sub
End Class
