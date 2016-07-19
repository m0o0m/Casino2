Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin

    Partial Class AgentManager
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"

        Public Property ViewLock() As Boolean
            Get
                If ViewState("VIEW_LOCK") Is Nothing Then
                    ViewState("VIEW_LOCK") = False
                End If

                Return CBool(ViewState("VIEW_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_LOCK") = value

                Dim sMessage As String = SafeString(IIf(value, "Unlock", "Lock"))
                btnLock.Text = sMessage
                btnViewLock.Text = "View " & sMessage
            End Set
        End Property

        Property SiteType() As String
            Get
                Return SafeString(ViewState("SiteType"))
            End Get
            Set(ByVal value As String)
                ViewState("SiteType") = value
            End Set
        End Property

        Public Property ViewBettingLock() As Boolean
            Get
                If ViewState("VIEW_BETTING_LOCK") Is Nothing Then
                    ViewState("VIEW_BETTING_LOCK") = False
                End If

                Return CBool(ViewState("VIEW_BETTING_LOCK"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("VIEW_BETTING_LOCK") = value

                Dim sMessage As String = SafeString(IIf(value, "Unlock", "Lock"))
                btnBettingLock.Text = "Betting " & sMessage
                btnViewBettingLock.Text = "View Betting " & sMessage
            End Set
        End Property

#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                bindAgents()
                ucAgentEdit.SiteType = SiteType
            End If
        End Sub

#End Region

#Region "Bind Data"

        Private Sub bindAgents()
            dgAgents.DataSource = loadAgents()
            dgAgents.DataBind()
        End Sub

        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))

            Dim oAgentManager As New CAgentManager
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(UserSession.UserID, Me.ViewBettingLock, Me.ViewLock, txtNameOrLogin.Text)

            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

            If odrParents.Length = 0 Then
                odrParents = dtParents.Select("ParentID IS NOT NULL", "AgentName")
            End If

            For Each drParent As DataRow In odrParents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drParent("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 1) & SafeString(drParent("AgentName"))
                odrAgent("Login") = drParent("Login")
                odrAgent("IsLocked") = drParent("IsLocked")
                odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
                odrAgent("LastLoginDate") = drParent("LastLoginDate")

                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents)
            Next

            Return odtAgents
        End Function

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable)
            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

            For Each drChild As DataRow In odrSubAgents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)

                odrAgent("AgentID") = drChild("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
                odrAgent("Login") = drChild("Login")
                odrAgent("IsLocked") = drChild("IsLocked")
                odrAgent("IsBettingLocked") = drChild("IsBettingLocked")
                odrAgent("LastLoginDate") = drChild("LastLoginDate")

                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents)
            Next
        End Sub

        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
            Dim sLoop As String = ""

            For nIndex As Integer = 0 To pnLoop - 1
                sLoop &= psSource
            Next

            Return sLoop & " "
        End Function

#End Region

        Protected Sub dgAgents_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgAgents.ItemCommand
            Dim sAgentID As String = SafeString(e.CommandArgument).Split("|"c)(0)

            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                    If Not ucAgentEdit.LoadAgentInfo(sAgentID) Then
                        ClientAlert("Can't Load Agent", True)
                        Return
                    End If

                    dgAgents.SelectedIndex = e.Item.ItemIndex

                Case "EDITPLAYER"
                    Response.Redirect("UsersManager.aspx?tab=PLAYER&AgentID=" & sAgentID)
            End Select
        End Sub

        Protected Sub ucAgentEdit_ButtonClick(ByVal sButtonType As String) Handles ucAgentEdit.ButtonClick
            dgAgents.SelectedIndex = -1
            ucAgentEdit.ClearAgentInfo()

            Select Case UCase(sButtonType)
                Case "SAVE UNSUCCESSFUL"
                    ClientAlert("Failed to Save Setting", True)

                Case "SAVE SUCCESSFUL"

                    bindAgents()
            End Select
        End Sub

        Protected Sub btnViewLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLock.Click
            Me.ViewLock = Not Me.ViewLock

            dgAgents.SelectedIndex = -1
            bindAgents()
        End Sub

        Protected Sub btnViewBettingLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewBettingLock.Click
            Me.ViewBettingLock = Not Me.ViewBettingLock

            dgAgents.SelectedIndex = -1
            bindAgents()
        End Sub

        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            dgAgents.SelectedIndex = -1
            bindAgents()
        End Sub

        Private Function getSqlAgentIDs() As List(Of String)
            Dim oSqlAgentIDs As New List(Of String)

            For Each oItem As DataGridItem In dgAgents.Items
                If (CType(oItem.FindControl("chkID"), CheckBox).Checked) Then
                    oSqlAgentIDs.Add(SafeString(CType(oItem.FindControl("lbtEdit"), LinkButton).CommandArgument))
                End If
            Next

            Return oSqlAgentIDs
        End Function

        Protected Sub btnLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLock.Click
            Dim oSqlAgentIDs As List(Of String) = getSqlAgentIDs()

            If oSqlAgentIDs.Count = 0 Then
                ClientAlert("Select Agents To " & LCase(btnLock.Text), True)
                Return
            End If

            If (New CAgentManager).LockAgents(oSqlAgentIDs, Not Me.ViewLock, UserSession.UserID) Then
                dgAgents.SelectedIndex = -1
                bindAgents()
            End If
        End Sub

        Protected Sub btnBettingLock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBettingLock.Click
            Dim oSqlAgentIDs As List(Of String) = getSqlAgentIDs()

            If oSqlAgentIDs.Count = 0 Then
                ClientAlert("Select Agents To " & LCase(btnBettingLock.Text), True)
                Return
            End If

            If (New CAgentManager).BettingLockAgents(oSqlAgentIDs, Not Me.ViewBettingLock, UserSession.UserID) Then
                dgAgents.SelectedIndex = -1
                bindAgents()
            End If
        End Sub

    End Class

End Namespace