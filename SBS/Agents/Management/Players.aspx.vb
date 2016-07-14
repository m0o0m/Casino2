Imports SBCBL.std

Namespace SBSAgents.Management

    Partial Class Players
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Players"
            MenuTabName = "USER_MANAGEMENT"
            SubMenuActive = "PLAYERS"
            DisplaySubTitlle(Me.Master, "Players")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                '  ucPlayerManager.AgentID = UserSession.AgentUserInfo.UserID
            End If
        End Sub

    End Class

End Namespace
