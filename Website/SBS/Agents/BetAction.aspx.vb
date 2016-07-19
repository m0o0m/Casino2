Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections

Namespace SBSAgents
    Partial Class BetAction
        Inherits SBCBL.UI.CSBCPage

#Region "Page Events"
        Protected Sub Page_Init2(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Wager"
            MenuTabName = "BET_ACTION"
            DisplaySubTitlle(Me.Master, "Straight Bet")
            TopMenu = CType(Page.Master.FindControl("menuWager"), HtmlAnchor)
            BottomMenu = CType(Page.Master.FindControl("footMenuWager"), HtmlAnchor)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If UserSession.UserType = SBCBL.EUserType.Agent Then

                lblPlayer.Text = "Betting For " & UserSession.Cache.GetPlayerInfo(SafeString(Request.QueryString("PlayerID").Replace("#", ""))).Login
            End If
        End Sub
#End Region

#Region "Page Method"
        <System.Web.Services.WebMethod()> _
        Public Shared Function ResetTicket(ByVal oPlayerID As String) As Boolean
            HttpContext.Current.Session(String.Format("{0}_SELECTED_TICKETS", oPlayerID)) = Nothing
        End Function

#End Region

    End Class
End Namespace