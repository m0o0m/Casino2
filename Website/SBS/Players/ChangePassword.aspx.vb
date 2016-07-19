Imports SBCBL.std

Namespace SBSPlayer
    Partial Class ChangePassword
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Change Password"
            SubMenuActive = "CHANGE_PASSWORD"
            MenuTabName = "ACCOUNT_STATUS"
            DisplaySubTitlle(Me.Master, "Change Password")
        End Sub
    End Class
End Namespace

