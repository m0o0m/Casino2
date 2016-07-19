Imports System.Xml
Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class MessagePlayers
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Message To All Player"
            Me.SideMenuTabName = "MESSAGE_ALL_PLAYER"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Message To All Player")
        End Sub

       
    End Class
End Namespace