Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections

Namespace SBSAgents
    Partial Class BetAction
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init2(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Wager"
            MenuTabName = "BET_ACTION"
            DisplaySubTitlle(Me.Master, "Straight Bet")
            TopMenu = CType(Page.Master.FindControl("menuWager"), HtmlAnchor)
            BottomMenu = CType(Page.Master.FindControl("footMenuWager"), HtmlAnchor)
        End Sub

#Region "Page Method"
        <System.Web.Services.WebMethod()> _
        Public Shared Function ResetTicket(ByVal oPlayerID As String) As Boolean
            HttpContext.Current.Session(String.Format("{0}_SELECTED_TICKETS", oPlayerID)) = Nothing
        End Function

#End Region

    End Class
End Namespace