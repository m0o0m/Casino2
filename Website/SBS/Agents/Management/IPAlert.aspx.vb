Imports SBCBL.std

Namespace SBSAgents
    Partial Class IPAlert
        Inherits SBCBL.UI.CSBCPage

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "IP Alert"
            Me.SubMenuActive = "IP_ALERT"
            Me.MenuTabName = "REPORT"
            DisplaySubTitlle(Me.Master, "IP Alert")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucIPAlert.AgentLabel = "Sub Agent"
            End If
        End Sub

    End Class
End Namespace