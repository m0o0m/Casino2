Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class Weeklyfigure
        Inherits SBCBL.UI.CSBCPage
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Player Balance Reports"
            Me.SideMenuTabName = "SPLAYER_BALANCE_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Player Balance Reports")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If not IsPostBack Then
                bindPAgents()
                'ucSummaryReport.AgentID = ddlAgents.SelectedValue
                'ucSummaryReport.ReloadData()
                If Request.QueryString("AgentID") IsNot Nothing Then
                    ucSummaryReport.FromDate = SafeDate(Request.QueryString("SDate"))
                    ucSummaryReport.ToDate = SafeDate(Request.QueryString("EDate"))
                    ucSummaryReport.AgentID = Request.QueryString("AgentID")
                    ddlAgents.SelectedValue = Request.QueryString("AgentID")
                    ucSummaryReport.bindWeek()
                    ucSummaryReport.ReloadData()
                End If
                ucSummaryReport.ShowWeekList = True
                ucSummaryReport.ShowAgentList = True
            End If
        End Sub


        Private Sub bindPAgents()
            Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            For Each drParent As DataRow In odrParents
                Dim sAgentID As String = SafeString(drParent("AgentID"))
                ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

                loadSubAgent(sAgentID, dtParents)
            Next

            ddlAgents.Items.Insert(0, "")
        End Sub

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtAgents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim sText As String = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                Dim sAgentID As String = SafeString(drChild("AgentID"))

                ddlAgents.Items.Add(New ListItem(sText, sAgentID))

                loadSubAgent(sAgentID, podtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""

            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next

            Return sLoop
        End Function

        'Private Sub bindAgents()
        '    Dim oAgentManager As New CAgentManager()
        '    ddlAgents.DataSource = oAgentManager.GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)
        '    ddlAgents.DataTextField = "AgentName"
        '    ddlAgents.DataValueField = "AgentID"
        '    ddlAgents.DataBind()
        '    ddlAgents.Items.Insert(0, New ListItem("All", "All"))
        'End Sub
        Protected Sub SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            ucSummaryReport.AgentID = ddlAgents.SelectedValue
            ucSummaryReport.FromDate = Nothing
            ucSummaryReport.ReloadData()
        End Sub
    End Class
End Namespace

