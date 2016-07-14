Imports SBCBL.std

Namespace SBSSuperAdmin

    Partial Class SuperAgentBalance
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Agent Balance Reports"
            Me.SideMenuTabName = "SAGENT_BALANCE_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Agent Balance Reports")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If UserSession.SuperAdminInfo.IsPartner Then
                    ucAgentBalanceReport.SuperID = UserSession.SuperAdminInfo.PartnerOf
                Else
                    ucAgentBalanceReport.SuperID = UserSession.SuperAdminInfo.UserID
                End If
                ucAgentBalanceReport.ReloadData()
            End If
        End Sub
    End Class
End Namespace