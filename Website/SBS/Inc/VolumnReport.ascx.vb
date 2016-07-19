Imports System.Data
Imports SBCBL
Imports SBCBL.Managers
Imports SBCBL.std

Partial Class SBS_Inc_VolumnReport
    Inherits SBCBL.UI.CSBCUserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            bindWeek()

            bindAgents()

            If (UserSession.UserType = EUserType.Agent) Then
                BindReport()
            End If
        End If

    End Sub

#Region "Properties"

    ReadOnly Property SelectedAgentId() As String
        Get
            If UserSession.UserType = EUserType.SuperAdmin Then
                Return ddlAgents.SelectedValue
            Else
                If String.IsNullOrEmpty(ddlAgents.SelectedValue) Then
                    Return UserSession.UserID
                Else
                    Return ddlAgents.SelectedValue
                End If
            End If
        End Get
    End Property

    ReadOnly Property SelectedFromDate() As Date
        Get
            Return SafeDate(ddlWeeks.Value)
        End Get
    End Property

    ReadOnly Property SelectedToDate() As Date
        Get
            Return SelectedFromDate.AddDays(7)
        End Get
    End Property

#End Region

#Region "Agents And Weeks"

    Private Sub bindAgents()
        Dim dtParents As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

        Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

        For Each drParent As DataRow In odrParents
            Dim sAgentID As String = SafeString(drParent("AgentID"))
            ddlAgents.Items.Add(New ListItem(SafeString(drParent("AgentName")), sAgentID))

            loadSubAgent(sAgentID, dtParents)
        Next

        If (UserSession.UserType = EUserType.Agent) Then
            ddlAgents.Items.Insert(0, New ListItem("Me", ""))
        Else
            ddlAgents.Items.Insert(0, New ListItem("Please Select an Agent", ""))
        End If

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

    Private Sub bindWeek()
        Dim nTimeZome As Integer = 0
        If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
            nTimeZome = UserSession.SuperAdminInfo.TimeZone
        ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
            nTimeZome = UserSession.AgentUserInfo.TimeZone
        End If
        Dim oDate As Date = GetEasternMondayOfCurrentWeek()
        Dim olstWeek As New Dictionary(Of String, String)

        For nIndex As Integer = 1 To 8
            Dim oTemp As Date = oDate.AddDays((nIndex - 1) * -7)
            olstWeek.Add(oTemp.ToString("MM/dd/yyyy") & " - " & oTemp.AddDays(6).ToString("MM/dd/yyyy"), oTemp.ToString("MM/dd/yyyy"))
        Next

        ddlWeeks.DataSource = olstWeek
        ddlWeeks.DataTextField = "Key"
        ddlWeeks.DataValueField = "value"
        ddlWeeks.DataBind()

        'If FromDate <> Date.MinValue Then
        '    ddlWeeks.SelectedValue = FromDate.ToString("MM/dd/yyyy")
        'End If
    End Sub

#End Region

#Region "Report Data"

    Private Sub BindReport()

        rptMain.DataSource = GetAgentsReportData()
        rptMain.DataBind()

    End Sub

    Private Function GetAgentsReportData() As DataTable
        Dim agentsData As DataTable = New DataTable()

        If SelectedAgentId <> "" Then
            Dim agentManager As CAgentManager = New CAgentManager()

            agentsData = agentManager.GetAllAgentSubAgentsByAgent(SelectedAgentId, Nothing)

            ' add current agent
            'Dim currentAgent As DataRow = agentManager.GetAgentByAgentID(SelectedAgentId)
            'agentsData.ImportRow(currentAgent)

        End If

        Return agentsData
    End Function

    Public Sub GetPlayersReportData(ByVal dgSubPlayers As DataGrid, ByVal psAgentID As String)

        Dim volumnData = (New CAgentManager()).GetVolumnReportForAgent(psAgentID, SelectedFromDate, SelectedToDate)

        dgSubPlayers.DataSource = volumnData

        dgSubPlayers.Visible = True
        dgSubPlayers.DataBind()

    End Sub

#End Region

#Region "Control Events"

    Protected Sub ddlWeeks_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
        BindReport()
    End Sub

    Protected Sub ddlAgents_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
        BindReport()
    End Sub

    Protected Sub rptMain_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.EditItem Then
            Dim dgSubPlayers As DataGrid = e.Item.FindControl("dgSubPlayers")
            Dim lblAgentWeekly As Literal = e.Item.FindControl("lblAgentWeekly")
            lblAgentWeekly.Text = "Agent: " & e.Item.DataItem("AgentName")

            GetPlayersReportData(dgSubPlayers, SafeString(e.Item.DataItem("AgentID")))
        End If
    End Sub

    Protected Sub dgSubPlayers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)

    End Sub

#End Region

End Class
