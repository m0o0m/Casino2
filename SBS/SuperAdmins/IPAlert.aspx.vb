Imports SBCBL.std

Namespace SBSSuperAdmin
    Partial Class IPAlert
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "IP Alert"
            Me.SideMenuTabName = "IP_ALERT"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "IP Alert")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucIPAlert.AgentLabel = "Agent"
            End If
        End Sub

    End Class
End Namespace