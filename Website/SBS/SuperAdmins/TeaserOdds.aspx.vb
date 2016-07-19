Imports SBCBL.std

Namespace SBSSuperAdmin

    Partial Class TeaserOdds
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Teaser Setup"
            Me.SideMenuTabName = "TEASER_ODDS"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Teaser Setup")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                ucTeaserOddsManager.LoadTeaserOdds()
            End If
        End Sub

        Protected Sub ucTeaserRulesManager_ButtonClick(ByVal sButtonType As String) Handles ucTeaserRulesManager.ButtonClick
            If sButtonType = "CHANGE TEASER RULES" Then
                ucTeaserOddsManager.LoadTeaserOdds()
            End If
        End Sub

    End Class

End Namespace

