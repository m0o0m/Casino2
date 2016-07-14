Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Partial Class CallCenter_BetAction
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
            pnlBetAction.Visible = True
            'Me.PlayerID = oPlayer.UserID
            ucBetActions.CallCenterSuperAgentID = Player.SuperAgentID
            ucBetActions.SelectedPlayerID = Player.UserID
            ucBetActions.PlayerCallCenterSelect = Player
            ' UserSession.SelectedTicket(Player.UserID) = Nothing
            ucBetActions.Visible = True
            'ucBetActions.BindBettingData()
        End If

    End Sub

#Region "Page Method"
    <System.Web.Services.WebMethod()> _
    Public Shared Function ResetTicket(ByVal oPlayerID As String) As Boolean
        HttpContext.Current.Session(String.Format("{0}_SELECTED_TICKETS", oPlayerID)) = Nothing
    End Function

#End Region
#End Region

End Class
