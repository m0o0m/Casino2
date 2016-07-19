Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer
    Partial Class SelectPlayers
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Select Players"
            If Not IsPostBack Then
                bindPlayers()
            End If
        End Sub

        Private Sub bindPlayers()
            ddlPlayers.DataSource = (New CPlayerManager).GetPlayers(SafeString(UserSession.AgentSelectID), Nothing)
            ddlPlayers.DataTextField = "Login"
            ddlPlayers.DataValueField = "PlayerID"
            ddlPlayers.DataBind()
        End Sub

        Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
            'Dim oPlayer As CPlayer = (New SBCBL.Managers.CPlayerManager()).GetPlayer(ddlPlayers.Value, False)
            If ddlPlayers.Items.Count = 0 Then
                ClientAlert("Player is empty", True)
                Return
            End If
            Session("BackAgentID") = UserSession.UserID
            Session("BackAgentName") = UserSession.AgentUserInfo.Login
            'UserSession.Cache.ClearAgentInfo(UserSession.UserID, UserSession.AgentUserInfo.Login)
            FormsAuthentication.SetAuthCookie(ddlPlayers.SelectedItem.Text, True)
            Session("USER_ID") = ddlPlayers.SelectedValue
            Session("USER_TYPE") = SBCBL.EUserType.Player
            Response.Redirect("/SBS/Players/PlayerAccount.aspx")
        End Sub
    End Class
End Namespace

