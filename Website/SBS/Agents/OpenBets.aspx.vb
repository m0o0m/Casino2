Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSAgents

    Partial Class OpenBets
        Inherits SBCBL.UI.CSBCPage
        Private ALL As String = "All"

#Region "Property"
        Private ReadOnly Property AgentID() As String
            Get
                Dim sAgentID As String = SafeString(Request("AgentID"))

                'If sAgentID = "" Then
                '    sAgentID = UserSession.UserID
                'End If

                Return sAgentID
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
                If ddlSubAgent.SelectedValue <> "" AndAlso Not ddlSubAgent.SelectedValue.Equals(ALL) Then
                    Return SafeString(ddlSubAgent.SelectedValue)
                End If
                Return UserSession.UserID
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
            PageTitle = "Pending Bets"            
            MenuTabName = "HOME"
            SubMenuActive = "OPEN_BET"
            DisplaySubTitlle(Me.Master, "Pending Bets")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then

                divSubAgent.Visible = True
                bindSubAgent()

                initSubAgentAndPlayer()

                bindPlayers(SelectedSubAgent)


                'If AgentID <> "" Then
                '    ' Default if we have agentID then we will show the first item to show data
                '    ' Apply for pending in Players and Reports
                '    ddlSubAgent.SelectedValue = ALL
                'End If

                If PlayerID <> "" Then
                    ddlPlayers.SelectedValue = PlayerID
                End If
                bindTickets()
                ' tmrRefresh.Enabled = True

            End If
        End Sub

#End Region

        Private Sub initSubAgentAndPlayer()

            If Me.AgentID <> "" Then
                ddlSubAgent.SelectedValue = Me.AgentID
            Else
                ddlSubAgent.SelectedValue = ALL
            End If
            If ddlSubAgent.SelectedValue = "" Then
                bindPlayers(UserSession.UserID)
                Return
            End If

            '  If ddlSubAgent.SelectedValue.Equals(ALL) Then
            ' bindPlayers(SelectedSubAgent)
            '  End If
            If ddlSubAgent.SelectedValue <> "" AndAlso ddlPlayers.SelectedValue <> "" Then
                bindTickets()
            End If

        End Sub


#Region "Bind data"

        Private Sub bindSubAgent()
            ddlSubAgent.DataSource = (New CAgentManager).GetAllAgentsByAgent(UserSession.UserID, Nothing)
            ddlSubAgent.DataTextField = "AgentName"
            ddlSubAgent.DataValueField = "AgentID"
            ddlSubAgent.DataBind()
            ddlSubAgent.Items.Insert(0, New ListItem("All", "All"))
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

        Private Sub bindTickets()
            Dim oTicketBets As DataTable
            If Request.QueryString("AwayTeam") IsNot Nothing AndAlso Request.QueryString("HomeTeam") IsNot Nothing Then
                oTicketBets = (New CTicketManager).GetOpenTicketsByAgentPosition(Request.QueryString("Context"), SelectedSubAgent, EDate, Request.QueryString("AwayTeam"), Request.QueryString("HomeTeam"), Request.QueryString("BetType"), SelectedTypeOfBet)
                ucTicketBetsGridAgent.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
                ucTicketBetsGridAgent.ShowPlayerColumn = SelectedPlayer.Equals(ALL)
                Return
            End If
            If ddlSubAgent.SelectedValue.Equals(ALL) AndAlso Not ddlPlayers.SelectedValue.Equals(ALL) Then 'ALL - ""
                ucTicketBetsGridAgent.LoadTicketBets((New CTicketManager).GetOpenTicketsByAgent(UserSession.UserID, EDate, ddlPlayers.Value, SelectedTypeOfBet), ddlContext.SelectedValue)
                Return
            End If
            If ddlSubAgent.SelectedValue.Equals(ALL) AndAlso ddlPlayers.SelectedValue.Equals(ALL) Then 'ALL - ALL
                ucTicketBetsGridAgent.LoadTicketBets((New CTicketManager).GetOpenTicketsByAllSubAgent(getListSubAgentID(UserSession.UserID), EDate, SelectedTypeOfBet), ddlContext.SelectedValue)
                Return
            End If
            If SelectedPlayer.Equals(ALL) Then
                oTicketBets = (New CTicketManager).GetOpenTicketsByAllSubAgent(getListSubAgentID(SelectedSubAgent), EDate, SelectedTypeOfBet)
            Else
                oTicketBets = (New CTicketManager).GetOpenTicketsByAgent(SelectedSubAgent, EDate, ddlPlayers.Value, SelectedTypeOfBet)
            End If

            ucTicketBetsGridAgent.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
        End Sub

#End Region

        Protected Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
            bindTickets()
        End Sub

        Protected Sub ddlSubAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSubAgent.SelectedIndexChanged

            ddlPlayers.Items.Clear()
            bindPlayers(SelectedSubAgent)


            If SelectedSubAgent.Equals(ALL) Then
                ucTicketBetsGridAgent.Visible = True
                Return
            ElseIf SelectedSubAgent <> "" AndAlso Not SelectedSubAgent.Equals(ALL) Then
                ucTicketBetsGridAgent.Visible = True
            Else 'selectedvalue is ""
                ucTicketBetsGridAgent.Visible = False
                ddlTypeOfBet.SelectedIndex = 0
                Return
            End If
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            ucTicketBetsGridAgent.ResultGrid.CurrentPageIndex = 0
            bindTickets()
            ucTicketBetsGridAgent.ShowPlayerColumn = ddlPlayers.Value = ALL
        End Sub

        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            bindTickets()
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            bindTickets()
        End Sub

        Private Function getListSubAgentID(ByVal psAgentID As String) As List(Of String)
            Dim olstSubAgent As List(Of String) = New List(Of String)
            If psAgentID <> "" Then
                Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsByAgent(psAgentID, Nothing)
                For Each odrSubAgent As DataRow In odtSubAgent.Rows
                    olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))
                Next
                olstSubAgent.Add(psAgentID)
            End If
            Return olstSubAgent
        End Function

        
    End Class

End Namespace

