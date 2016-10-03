Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSAgents
    Partial Class History
        Inherits SBCBL.UI.CSBCPage

        Private ALL As String = "All"

#Region "Properties"

        Private ReadOnly Property AgentID() As String
            Get
                Dim sAgentID As String = SafeString(Request("AgentID"))
                Return sAgentID
            End Get
        End Property

        Private ReadOnly Property PlayerID() As String
            Get
                Dim sPlayerID As String = SafeString(Request("PlayerID"))

                Return SafeString(sPlayerID)
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

#End Region

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If AgentID = UserSession.UserID Then
                PageTitle = "Your History"
            Else
                If AgentID <> "" Then
                    PageTitle = String.Format("Your Sub-Agent ({0})'s History", UserSession.Cache.GetAgentInfo(AgentID).Name)
                Else
                    PageTitle = String.Format("Your Sub-Agent ({0})'s History", UserSession.Cache.GetAgentInfo(UserSession.UserID).Name)
                End If

            End If            
            MenuTabName = "HOME"
            SubMenuActive = "HISTORY"
            DisplaySubTitlle(Me.Master, "History Bet")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then

                initSubAgentAndPlayer()

                If SDate <> Nothing Then
                    ucDateFrom.Value = SDate
                Else
                    ucDateFrom.Value = GetLastMondayOfDate(GetEasternDate)
                End If

                If EDate <> Nothing Then
                    ucDateTo.Value = EDate
                Else
                    ucDateTo.Value = GetLastMondayOfDate(GetEasternDate).AddDays(6)
                End If


                Dim sPlayerID As String = SafeString(Request("PlayerID"))
                ddlPlayers.SelectedValue = sPlayerID
                bindHistoryTickets()
            End If
        End Sub

#End Region
        Private Sub initSubAgentAndPlayer()
            'If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(UserSession.UserID) > 0 Then
            '    divSubAgent.Visible = True
            '    lblSubAgent.Visible = True
            'End If
            bindSubAgent()
            If Me.AgentID <> "" Then
                ddlSubAgent.SelectedValue = Me.AgentID
            End If
            If SelectedSubAgent.Equals(ALL) Then
                bindPlayers(UserSession.UserID)
            Else
                bindPlayers(SelectedSubAgent)
            End If

            If PlayerID <> "" Then
                ddlPlayers.SelectedValue = PlayerID
            End If
            'If AgentID <> "" Then
            '    If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(AgentID) > 0 Then
            '        ddlPlayers.Items.Insert(0, ALL)

            '    End If

            '    Return
            'End If
            'If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(UserSession.UserID) > 0 Then
            'ddlPlayers.Items.Insert(0, ALL)
            'End If
        End Sub
#Region "Bind data"
        Private Sub bindSubAgent()
            ddlSubAgent.DataSource = (New CAgentManager).GetAllAgentsByAgent(UserSession.UserID, Nothing)
            ddlSubAgent.DataTextField = "AgentName"
            ddlSubAgent.DataValueField = "AgentID"
            ddlSubAgent.DataBind()
            ddlSubAgent.Items.Insert(0, ALL)
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
            ddlPlayers.Items.Insert(0, ALL)
        End Sub

        Private Function checkConditions() As Boolean
            If ucDateFrom.Value = DateTime.MinValue AndAlso ucDateTo.Value = DateTime.MinValue Then
                ClientAlert("Please Choose Date Range To Search", True)
                ucDateFrom.Focus() : Return False
            End If

            If ucDateFrom.Value <> DateTime.MinValue AndAlso ucDateTo.Value <> DateTime.MinValue Then
                If ucDateFrom.Value > ucDateTo.Value Then
                    ClientAlert("Date To Must Be Greater Than Date From", True)
                    ucDateTo.Focus() : Return False
                End If
            End If

            Return True
        End Function

        Private Sub bindHistoryTickets()
            Dim oTransFrom As DateTime = SafeDate(ucDateFrom.Value)
            Dim oTransTo As DateTime = SafeDate(ucDateTo.Value)
            Dim odtHistoryTickets As DataTable
            If SelectedSubAgent.Equals(ALL) AndAlso Not SelectedPlayer.Equals(ALL) Then
                odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByAgent(UserSession.UserID, oTransFrom, oTransTo, SelectedPlayer, SelectedTypeOfBet)
            ElseIf SelectedSubAgent.Equals(ALL) AndAlso SelectedPlayer.Equals(ALL) Then
                odtHistoryTickets = (New CTicketManager).GetALLHistoryTicketsByListAgentID(getListSubAgentID(UserSession.UserID), oTransFrom, oTransTo, SelectedTypeOfBet)
            ElseIf SelectedPlayer.Equals(ALL) Then
                odtHistoryTickets = (New CTicketManager).GetALLHistoryTicketsByListAgentID(getListSubAgentID(SelectedSubAgent), oTransFrom, oTransTo, SelectedTypeOfBet)
            Else
                odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByAgent(SelectedSubAgent, oTransFrom, oTransTo, SelectedPlayer, SelectedTypeOfBet)

            End If

            'ucHistoryGrid.ShowPlayerName = SelectedPlayer = ALL
            ucHistoryGrid.LoadHistoryTickets(odtHistoryTickets)
            If PlayerID <> "" Or Me.AgentID <> "" Then
                'ucHistoryGrid.ResultGrid.Columns(5).Visible = PlayerID = ""
                ucHistoryGrid.ShowPlayerName = PlayerID = ""
            Else
                'ucHistoryGrid.ResultGrid.Columns(5).Visible = SelectedPlayer = ALL
                ucHistoryGrid.ShowPlayerName = SelectedPlayer = ALL
            End If
        End Sub

#End Region

#Region "Event"

        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            bindHistoryTickets()
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            If checkConditions() Then
                ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
                ucHistoryGrid.ResultGrid.Columns(4).Visible = PlayerID = ""
                bindHistoryTickets()
            End If
        End Sub

        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
            ucDateFrom.Value = GetLastMondayOfDate(Date.Now.ToUniversalTime)
            ucDateTo.Value = Date.Now.ToUniversalTime
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub

        Protected Sub ddlSubAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSubAgent.SelectedIndexChanged

            If SelectedSubAgent.Equals(ALL) Then
                bindPlayers(UserSession.UserID)
                ' ddlPlayers.Items.Insert(0, ALL)
                bindHistoryTickets()
                Return
            End If
            bindPlayers(SelectedSubAgent)
            'If (New SBCBL.Managers.CAgentManager()).NumOfSubAgents(SelectedSubAgent) > 0 Then
            '    ddlPlayers.Items.Insert(0, ALL)
            'End If
            bindHistoryTickets()
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            bindHistoryTickets()
        End Sub
#End Region

        Private Function getListSubAgentID(ByVal psAgentID As String) As List(Of String)
            Dim olstSubAgent As List(Of String) = New List(Of String)
            Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsByAgent(psAgentID, Nothing)
            For Each odrSubAgent As DataRow In odtSubAgent.Rows
                olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))
            Next
            olstSubAgent.Add(psAgentID)
            Return olstSubAgent
        End Function

    End Class
End Namespace