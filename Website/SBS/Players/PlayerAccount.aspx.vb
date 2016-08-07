Imports SBCBL.std

Namespace SBSPlayer
    Partial Class PlayerAccount
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Account Status"
            SubMenuActive = "ACCOUNT_STATUS"
            MenuTabName = "ACCOUNT_STATUS"
            DisplaySubTitlle(Me.Master, "Account Status")
            CurrentPageName = "Account_Account"
        End Sub
    End Class
End Namespace
