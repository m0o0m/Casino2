Imports SBCBL.Managers
Imports SBCBL.std
Namespace SBSWebsite
    Partial Class SBS_CallCenter_Default
        Inherits SBCBL.UI.CSBCPage

        Private Property PlayerID() As String
            Get
                Return SafeString(ViewState("__PLAYERID"))
            End Get
            Set(ByVal value As String)
                ViewState("__PLAYERID") = value
            End Set
        End Property

#Region "Page events"
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Your Homepage"
            MenuTabName = "HOME"
        End Sub
#End Region

        Protected Sub ucPlayerProfile_LoginSucceed(ByVal oPlayer As SBCBL.CacheUtils.CPlayer) Handles ucPlayerProfile.LoginSucceed
            UserSession.CCAgentUserInfo.PlayerID = oPlayer.UserID
            Response.Redirect("SelectGame.aspx")
        End Sub

        Protected Sub ucPlayerProfile_Logout() Handles ucPlayerProfile.Logout
            UserSession.SelectedTicket(Me.PlayerID) = Nothing
            Me.PlayerID = ""
        End Sub

    End Class

End Namespace
