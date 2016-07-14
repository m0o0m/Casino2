Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Partial Class SBS_Players_Default
    Inherits SBCBL.UI.CSBCPage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If UserSession.PlayerUserInfo.RequirePasswordChange Then '' Redirect to change password page
            Response.Redirect("AccountStatus.aspx?RequireChangePass=Y")
        End If
        'MenuTabName = "BET_ACTION"
        'PageTitle = "Wager"
        'DisplaySubTitlle(Me.Master, "Select Game")
        'TopMenu = CType(Page.Master.FindControl("menuWager"), HtmlAnchor)
        'BottomMenu = CType(Page.Master.FindControl("footMenuWager"), HtmlAnchor)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ucSelectGame.SelectedPlayerID = UserSession.PlayerUserInfo.UserID
        End If
    End Sub
End Class
