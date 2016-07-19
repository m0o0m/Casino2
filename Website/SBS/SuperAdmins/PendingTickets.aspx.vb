Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class PendingTickets
        Inherits SBCBL.UI.CSBCPage

        Private ALL As String = "All"

#Region "Property"
        Private ReadOnly Property AgentID() As String
            Get
                Return SafeString(Request("AgentID"))
            End Get
        End Property

        Public ReadOnly Property PlayerID() As String
            Get
                Return SafeString(Request("PlayerID"))
            End Get
        End Property

        Public ReadOnly Property SDate() As Date
            Get
                Return SafeDate(Request("SDate"))
            End Get
        End Property

        Public ReadOnly Property EDate() As Date
            Get
                Return SafeDate(Request("EDate"))
            End Get
        End Property
        Public ReadOnly Property SelectedSubAgent() As String
            Get
                Return SafeString(ddlSubAgent.SelectedValue)
            End Get
        End Property

        Public ReadOnly Property SelectedPlayer() As String
            Get
                Return SafeString(ddlPlayers.SelectedValue)
            End Get
        End Property

        Public ReadOnly Property SelectedTypeOfBet() As String
            Get
                Return SafeString(ddlTypeOfBet.SelectedValue)
            End Get
        End Property

        Private ReadOnly Property SuperID() As String
            Get
                If UserSession.SuperAdminInfo.IsPartner Then
                    Return UserSession.SuperAdminInfo.PartnerOf
                Else
                    Return UserSession.UserID
                End If

            End Get
        End Property

#End Region

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "Pending Tickets"
            Me.SideMenuTabName = "PENDING_WAGERS_REPORTS"
            Me.MenuTabName = "REPORTS MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Pending Tickets")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                initSubAgentAndPlayer()
                tmrRefresh.Enabled = True
            End If
        End Sub

        Private Sub initSubAgentAndPlayer()

            bindAgents()
            ddlSubAgent.Items.Insert(0, ALL)
            If PlayerID <> "" Then

                Dim sSubAgentID = UserSession.Cache.GetPlayerInfo(PlayerID).AgentID
                ddlSubAgent.SelectedValue = sSubAgentID
                bindPlayers(sSubAgentID)
                ddlPlayers.SelectedValue = PlayerID
                bindTickets()
                Return
            End If
            If AgentID <> "" Then
                ddlSubAgent.SelectedValue = Me.AgentID
                '  If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(AgentID) > 0 Then
                bindPlayers(AgentID)
                'End If
                bindTickets()
                Return
            End If
            ddlPlayers.Items.Insert(0, ALL)
            bindTickets()
        End Sub

#Region "Bind Data "
        Private Sub bindAgents()

            ddlSubAgent.DataSource = loadAgents()
            ddlSubAgent.DataTextField = "AgentName"
            ddlSubAgent.DataValueField = "AgentID"
            ddlSubAgent.DataBind()


        End Sub
        Private Sub bindPlayers(ByVal psSubAgetID As String, Optional ByVal plstSubAgetID As List(Of String) = Nothing)

            If plstSubAgetID Is Nothing Then
                ddlPlayers.DataSource = (New CPlayerManager).GetPlayers(psSubAgetID, Nothing)
            Else
                ddlPlayers.DataSource = (New CPlayerManager()).GetAllPlayerByListAgentID(plstSubAgetID)
            End If

            ddlPlayers.DataTextField = "FullName"
            ddlPlayers.DataValueField = "PlayerID"
            ddlPlayers.DataBind()
            ' If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(psSubAgetID) > 0 Then
            ddlPlayers.Items.Insert(0, ALL)
            'End If
        End Sub
#End Region
        Private Sub selectAgents()
            historypanel.Visible = True
            bindAgents()
            ddlSubAgent.SelectedIndex = 1
            Dim oTicketBets As DataTable = (New CTicketManager).GetOpenTicketsByAgent(ddlSubAgent.Value, SDate, EDate)
            ucTicketBetsGrid.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
        End Sub

        Private Sub bindTickets()
            Dim oTicketBets As DataTable
            If Request.QueryString("AwayTeam") IsNot Nothing AndAlso Request.QueryString("HomeTeam") IsNot Nothing Then
                oTicketBets = (New CTicketManager).GetOpenTicketsByAgentPosition(Request.QueryString("Context"), SelectedSubAgent, EDate, Request.QueryString("AwayTeam"), Request.QueryString("HomeTeam"), Request.QueryString("BetType"), SelectedTypeOfBet)
                ucTicketBetsGrid.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
                ucTicketBetsGrid.ShowPlayerColumn = SelectedPlayer.Equals(ALL)
                Return
            End If
            If SelectedSubAgent.Equals(ALL) Then
                ucTicketBetsGrid.LoadTicketBets((New CTicketManager).GetOpenTicketsByAllSubAgent(getListAgentIDBySuperID(UserSession.UserID), EDate, SelectedTypeOfBet), ddlContext.SelectedValue)
                ucTicketBetsGrid.ShowPlayerColumn = True
                Return
            End If
            If SelectedPlayer.Equals(ALL) Then
                oTicketBets = (New CTicketManager).GetOpenTicketsByAllSubAgent(getListSubAgentID(SelectedSubAgent), EDate, SelectedTypeOfBet)
                ucTicketBetsGrid.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
                ucTicketBetsGrid.ShowPlayerColumn = SelectedPlayer.Equals(ALL)
                Return
            End If
           
            oTicketBets = (New CTicketManager).GetOpenTicketsByAgent(SelectedSubAgent, EDate, SelectedPlayer, SelectedTypeOfBet)
            ucTicketBetsGrid.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
            ucTicketBetsGrid.ShowPlayerColumn = SelectedPlayer.Equals(ALL)
        End Sub

        Private Function getListSubAgentID(ByVal psAgentID As String) As List(Of String)
            Dim olstSubAgent As List(Of String) = New List(Of String)
            Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsByAgent(psAgentID, Nothing)
            For Each odrSubAgent As DataRow In odtSubAgent.Rows
                olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))
            Next
            olstSubAgent.Add(psAgentID)
            Return olstSubAgent
        End Function

        Private Function getListAgentIDBySuperID(ByVal psSuperID As String) As List(Of String)
            Dim olstAgent As List(Of String) = New List(Of String)
            Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(psSuperID, Nothing)
            For Each odrAgent As DataRow In odtSubAgent.Rows
                olstAgent.Add(SafeString(odrAgent("AgentID")))
            Next
            olstAgent.Add(psSuperID)
            Return olstAgent
        End Function


        Private Function loadAgents() As DataTable
            Dim odtAgents As New DataTable
            odtAgents.Columns.Add("AgentID", GetType(Guid))
            odtAgents.Columns.Add("AgentName", GetType(String))
            odtAgents.Columns.Add("Login", GetType(String))
            odtAgents.Columns.Add("IsLocked", GetType(String))
            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))

            Dim oAgentManager As New CAgentManager
            Dim dtParents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(SuperID, Nothing)

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
#Region "Event"
        Protected Sub ddlSubAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSubAgent.SelectedIndexChanged

            If SelectedSubAgent.Equals(ALL) Then
                bindPlayers(UserSession.UserID)
                ddlPlayers.Items.Clear()
                bindTickets()
                ddlPlayers.Items.Insert(0, ALL)
                Return
            End If
            bindPlayers(SelectedSubAgent)
            If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(SelectedSubAgent) > 0 Then

                bindTickets()
                Return
            End If
            bindTickets()
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            ucTicketBetsGrid.ResultGrid.CurrentPageIndex = 0
            bindTickets()
        End Sub
        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            ucTicketBetsGrid.ResultGrid.CurrentPageIndex = 0
            bindTickets()
        End Sub

        Protected Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
            bindTickets()
        End Sub

#End Region

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            bindTickets()
        End Sub
    End Class
End Namespace