Imports SBCBL.std

Namespace SBSAgents
    Partial Class SubAgentReport
        Inherits SBCBL.UI.CSBCPage

#Region "Properties"
        Private ReadOnly Property AgentID() As String
            Get
                Dim sAgentID As String = SafeString(Request("AgentID"))
                If sAgentID = "" Then
                    sAgentID = UserSession.UserID
                End If

                Return sAgentID
            End Get
        End Property

#End Region

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If AgentID = UserSession.UserID Then
                PageTitle = "Agent Balance Reports"
            Else
                PageTitle = String.Format("Agent {0} Balance Reports", UserSession.Cache.GetAgentInfo(AgentID).Login)
            End If
            MenuTabName = "REPORT"
            SubMenuActive = "AGENT_BALANCE_REPORTS"
            DisplaySubTitlle(Me.Master, "Agent Balance Reports")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ucAgentBalanceReport.AgentID = AgentID
                ucAgentBalanceReport.ReloadData()
            End If
        End Sub
    End Class
End Namespace