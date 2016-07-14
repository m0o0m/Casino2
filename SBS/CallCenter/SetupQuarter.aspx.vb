Imports SBCBL.std
Imports System.Data


Namespace SBSCallCenterAgents
    Partial Class SetupQuarter
        Inherits SBCBL.UI.CSBCPage
        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Setup Quater Line"
            SideMenuTabName = "MANUAL_LINE"
            SubMenuActive = "MANUAL_LINE"
            DisplaySubTitlle(Me.Master, "Setup Quater Line")
        End Sub


    End Class
End Namespace