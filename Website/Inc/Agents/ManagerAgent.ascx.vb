Imports System.Data

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSAgents
    Partial Class ManagerAgent
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"

        Public ReadOnly Property ViewLock() As Boolean
            Get
                Return rdLockAcct.Checked
            End Get
          
        End Property

        Public ReadOnly Property ViewBettingLock() As Boolean
            Get
                Return rdLockBetAcct.Checked
            End Get
        End Property

        Public Property AgentInfo() As String
            Get
                Return SafeString(ViewState("AgentID"))
            End Get
            Set(ByVal value As String)
                ViewState("AgentID") = value
            End Set
        End Property
#End Region

#Region "Page events"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                rdAllAcct.Checked = True
                bindAgents()
                ucAgentEdit.Visible = False
                pnAgentManager.Visible = True
                btnAddNew.Visible = GetAddNewSubAgent(UserSession.AgentUserInfo.UserID)
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
            Dim dtParents As DataTable
            Dim sAgentID As New List(Of String)
            Dim lstAgentID As New List(Of String)
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(Date))
            odtAgents.Columns.Add("NumOfAgent", GetType(Integer))
            odtAgents.Columns.Add("NumOfPlayer", GetType(Integer))
            odtAgents.Columns.Add("HasCasino", GetType(String))
            Dim oAgentManager As New CAgentManager
            Dim oPlayerManager As New CPlayerManager
            If rdAllAcct.Checked Then
                dtParents = oAgentManager.GetAllAgentsByAgentID(UserSession.UserID, txtNameOrLogin.Text)
            Else
                dtParents = oAgentManager.GetAllAgentsByAgent(UserSession.UserID, ViewBettingLock, ViewLock, txtNameOrLogin.Text)
            End If
            Dim nMinAgentLevel As Integer = SafeInteger(dtParents.Compute("MIN(AgentLevel)", ""))
            Dim odrParents As DataRow() = dtParents.Select("AgentLevel=" & SafeString(nMinAgentLevel), "AgentName")

            For Each drParent As DataRow In odrParents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)
                lstAgentID.Add(SafeString(drParent("AgentID")))
                odrAgent("AgentID") = drParent("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 2) & SafeString(drParent("AgentName"))
                odrAgent("Login") = drParent("Login")
                odrAgent("IsLocked") = drParent("IsLocked")
                odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
                odrAgent("LastLoginDate") = drParent("LastLoginDate")
                odrAgent("NumOfAgent") = oAgentManager.GetAllAgentsByAgentID(SafeString(drParent("AgentID"))).Rows.Count
                odrAgent("HasCasino") = drParent("HasCasino")
                sAgentID.Clear()
                sAgentID.Add(SafeString(drParent("AgentID")))
                odrAgent("NumOfPlayer") = oPlayerManager.GetAllPlayerByListAgentID(sAgentID).Rows.Count
                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents, lstAgentID)
            Next
            For Each drParent As DataRow In dtParents.Rows
                If Not lstAgentID.Contains(SafeString(drParent("AgentID"))) Then
                    Dim odrAgent As DataRow = odtAgents.NewRow
                    odtAgents.Rows.Add(odrAgent)
                    odrAgent("AgentID") = drParent("AgentID")
                    odrAgent("AgentName") = SafeString(drParent("AgentName"))
                    odrAgent("Login") = drParent("Login")
                    odrAgent("IsLocked") = drParent("IsLocked")
                    odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
                    odrAgent("LastLoginDate") = drParent("LastLoginDate")
                    odrAgent("HasCasino") = drParent("HasCasino")
                    sAgentID.Clear()
                    sAgentID.Add(SafeString(drParent("AgentID")))
                    odrAgent("NumOfPlayer") = oPlayerManager.GetAllPlayerByListAgentID(sAgentID).Rows.Count
                    odrAgent("NumOfAgent") = oAgentManager.GetAllAgentsByAgentID(SafeString(drParent("AgentID"))).Rows.Count
                    loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents, lstAgentID)

                End If

            Next
            Return odtAgents
        End Function

        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable, ByRef lstAgentID As List(Of String))
            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")
            Dim oAgentManager As New CAgentManager
            Dim oPlayerManager As New CPlayerManager
            Dim sAgentID As New List(Of String)
            For Each drChild As DataRow In odrSubAgents
                Dim odrAgent As DataRow = odtAgents.NewRow
                odtAgents.Rows.Add(odrAgent)
                lstAgentID.Add(SafeString(drChild("AgentID")))
                odrAgent("AgentID") = drChild("AgentID")
                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 2) & SafeString(drChild("AgentName"))
                odrAgent("Login") = drChild("Login")
                odrAgent("IsLocked") = drChild("IsLocked")
                odrAgent("LastLoginDate") = drChild("LastLoginDate")
                odrAgent("HasCasino") = drChild("HasCasino")
                odrAgent("IsBettingLocked") = drChild("IsBettingLocked")
                odrAgent("NumOfAgent") = oAgentManager.GetAllAgentsByAgentID(SafeString(drChild("AgentID"))).Rows.Count
                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents, lstAgentID)
                sAgentID.Clear()
                sAgentID.Add(SafeString(drChild("AgentID")))
                odrAgent("NumOfPlayer") = oPlayerManager.GetAllPlayerByListAgentID(sAgentID).Rows.Count
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
            Dim sAgentInfo = SafeString(e.CommandArgument)
            'AgentInfo = SafeString(sAgentID.Split("|"c)(0))
            Dim hfLock As HiddenField = CType(e.Item.FindControl("hfLock"), HiddenField)
            Dim hfBettingLock As HiddenField = CType(e.Item.FindControl("hfBettingLock"), HiddenField)
            Dim lbnLock As LinkButton = CType(e.Item.FindControl("lbnLock"), LinkButton)
            Dim lbnBettingLock As LinkButton = CType(e.Item.FindControl("lbnBettingLock"), LinkButton)
            Select Case UCase(e.CommandName)
                Case "EDITUSER"
                    If Not ucAgentEdit.LoadAgentInfo(sAgentInfo.Split("|"c)(0)) Then
                        ClientAlert("Can't Load Agent", True)
                        Return
                    Else
                        ucAgentEdit.Visible = True
                        pnAgentManager.Visible = False
                        ucAgentEdit.bindPAgents()
                    End If
                Case "LOCK"
                    Dim oSqlAgentIDs As New List(Of String)
                    oSqlAgentIDs.Add(sAgentInfo)
                    If String.IsNullOrEmpty(sAgentInfo) Then
                        ClientAlert("Select Agents To " & LCase(lbnLock.Text), True)
                        Return
                    Else
                        If (New CAgentManager).LockAgents(oSqlAgentIDs, IIf(hfLock.Value.Equals("Y", StringComparison.CurrentCultureIgnoreCase), False, True), UserSession.UserID) Then
                            AgentInfo = Nothing
                            bindAgents()
                        End If
                    End If
                Case "BETTING LOCK"
                    Dim oSqlAgentIDs As New List(Of String)
                    oSqlAgentIDs.Add(sAgentInfo)
                    If String.IsNullOrEmpty(sAgentInfo) Then
                        ClientAlert("Select Agents To " & LCase(lbnBettingLock.Text), True)
                        Return
                    Else
                        If (New CAgentManager).BettingLockAgents(oSqlAgentIDs, IIf(hfBettingLock.Value.Equals("Y", StringComparison.CurrentCultureIgnoreCase), False, True), UserSession.UserID) Then
                            dgAgents.SelectedIndex = -1
                            AgentInfo = Nothing
                            bindAgents()

                        End If
                    End If
            End Select
            dgAgents.SelectedIndex = e.Item.ItemIndex
        End Sub

        Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
            bindAgents()
        End Sub

        Protected Sub chbLockAcct_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdAllAcct.CheckedChanged, rdLockAcct.CheckedChanged, rdLockBetAcct.CheckedChanged
            bindAgents()
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
            pnAgentManager.Visible = True
            ucAgentEdit.Visible = False
        End Sub

        Protected Sub btnAddNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNew.Click
            ucAgentEdit.Visible = True
            pnAgentManager.Visible = False
            ucAgentEdit.bindPAgents()
        End Sub

        Protected Sub dgAgents_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgAgents.PageIndexChanged
            dgAgents.CurrentPageIndex = e.NewPageIndex
            bindAgents()
        End Sub
    End Class
End Namespace
