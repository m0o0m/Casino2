Imports SBCBL.std

Namespace SBSAgents

    Partial Class OddSetting
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Odds Setting"
            MenuTabName = "GAME_MANAGEMENT"
            SubMenuActive = "ODDS_SETTING"
            DisplaySubTitlle(Me.Master, "Odds Setting")
        End Sub

    End Class

End Namespace

