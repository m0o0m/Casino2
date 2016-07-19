Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Partial Class SBS_CallCenter_SelectGame
    Inherits SBCBL.UI.CSBCPage

    Private ReadOnly Property Player() As CPlayer
        Get
            Dim oCacheManager As CCacheManager = New CCacheManager()
            If String.IsNullOrEmpty(UserSession.CCAgentUserInfo.PlayerID) Then
                Response.Redirect("Default.aspx")
            End If
            Return oCacheManager.GetPlayerInfo(UserSession.CCAgentUserInfo.PlayerID)
        End Get
    End Property

#Region "Page events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        PageTitle = "Bet Action"
        MenuTabName = "BET_ACTION"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'pnlBetAction.Visible = True
            ''Me.PlayerID = oPlayer.UserID
            ucSelectGame.CallCenterAgentID = (New CAgentManager).GetSuperAgentID(Player.AgentID)
            ucSelectGame.SelectedPlayerID = Player.UserID
            'UserSession.SelectedTicket(Player.UserID) = Nothing
            'ucBetActions.Visible = True
            'ucBetActions.BindBettingData()
        End If

    End Sub
#End Region

End Class
