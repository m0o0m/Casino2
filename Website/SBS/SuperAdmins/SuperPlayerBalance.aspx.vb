Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSSuperAdmin

    Partial Class SuperPlayerBalance
        Inherits SBCBL.UI.CSBCPage

#Region "Properties"
        Private ReadOnly Property AgentID() As String
            Get
                Return SafeString(Request("AgentID"))
            End Get
        End Property

        Private ReadOnly Property StartDate() As String
            Get
                Return SafeString(Request("SDate"))
            End Get
        End Property
#End Region

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Player Balance Reports"
            Me.SideMenuTabName = "SPLAYER_BALANCE_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Player Balance Reports")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                If AgentID <> "" Then
                    ucPlayerBalanceReport.AgentID = AgentID
                ElseIf UserSession.SuperAdminInfo.IsPartner Then
                    ucPlayerBalanceReport.SuperID = UserSession.SuperAdminInfo.PartnerOf
                Else
                    ucPlayerBalanceReport.SuperID = UserSession.SuperAdminInfo.UserID
                End If

                ucPlayerBalanceReport.ReloadData()
                If StartDate <> "" Then
                    ucPlayerBalanceReport.SelectedWeek = StartDate
                End If

                bindSubAgents()
            End If
        End Sub

        Private Sub bindSubAgents()
            Dim oAgentManager As New CAgentManager()
            If oAgentManager.NumOfSubAgents(ucPlayerBalanceReport.SelectedAgent) > 0 Then
                ucSubPlayerBalanceReport.AgentID = ucPlayerBalanceReport.SelectedAgent
                ucSubPlayerBalanceReport.ReloadData()

                ucSubPlayerBalanceReport.SelectedWeek = ucPlayerBalanceReport.SelectedWeek
                ucSubPlayerBalanceReport.Visible = True
            Else
                ucSubPlayerBalanceReport.Visible = False
            End If
        End Sub

        Protected Sub ucPlayerBalanceReport_OnChange() Handles ucPlayerBalanceReport.OnChange
            bindSubAgents()
        End Sub
    End Class
End Namespace