Imports SBCBL.std
Imports SBCBL.Managers

Namespace SBSAgents

    Partial Class Inc_Agents_accountStatus
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                bindPlayers()
                loadAccountStatus()
            End If
        End Sub

        Private Sub bindPlayers()
            ddlPlayers.OptionalText = "My Status"
            ddlPlayers.DataSource = (New CPlayerManager).GetPlayers(UserSession.AgentUserInfo.UserID, Nothing)
            ddlPlayers.DataTextField = "FullName"
            ddlPlayers.DataValueField = "PlayerID"
            ddlPlayers.DataBind()
        End Sub

        Private Sub loadAccountStatus()
            lblCreditLimit.Text = ""
            lblLastLoginDate.Text = ""
            lblAvailableBalance.Text = ""

            If UserSession.AgentUserInfo.LastLoginDate <> Nothing Then
                lblLastLoginDate.Text = SafeString(UserSession.AgentUserInfo.LastLoginDate)
            End If

        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            If ddlPlayers.Value <> "" Then
                lblCreditLimit.Text = FormatNumber(UserSession.Cache.GetPlayerInfo(ddlPlayers.Value).Template.CreditMaxAmount, SBCBL.std.GetRoundMidPoint())
                lblAvailableBalance.Text = FormatNumber(SafeRound(UserSession.Cache.GetPlayerInfo(ddlPlayers.Value).BalanceAmount), GetRoundMidPoint)

                If UserSession.Cache.GetPlayerInfo(ddlPlayers.Value).LastLoginDate <> Nothing Then
                    lblLastLoginDate.Text = SafeString(UserSession.Cache.GetPlayerInfo(ddlPlayers.Value).LastLoginDate)
                End If
            Else
                loadAccountStatus()
            End If
        End Sub
    End Class

End Namespace

