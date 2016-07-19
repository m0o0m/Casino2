Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.CacheUtils
Imports System.Data
Imports System.Collections

Namespace SBSAgents
    Partial Class SBC_Agents_SelectGame
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init2(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Wager"
            MenuTabName = "BET_ACTION"
            DisplaySubTitlle(Me.Master, "Select Game")
            TopMenu = CType(Page.Master.FindControl("menuWager"), HtmlAnchor)
            BottomMenu = CType(Page.Master.FindControl("footMenuWager"), HtmlAnchor)
        End Sub
    End Class
End Namespace

