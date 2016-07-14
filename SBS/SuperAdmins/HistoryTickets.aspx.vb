Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSSuperAdmin
    Partial Class HistoryTickets
        Inherits SBCBL.UI.CSBCPage

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

        Public ReadOnly Property SelectedTypeOfBet() As String
            Get
                Return SafeString(ddlTypeOfBet.SelectedValue)
            End Get
        End Property
#End Region

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.PageTitle = "History Tickets"
            Me.SideMenuTabName = "SAGENT_BALANCE_REPORTS"
            Me.MenuTabName = "REPORTS"
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                bindTickets()
            End If
        End Sub

        Public Sub bindTickets()
            Dim odtHistoryTickets As DataTable

            If AgentID <> "" Then
                Dim oAgentManager As CAgentManager = New CAgentManager()
                ucHistoryGrid.AgentName = SafeString(oAgentManager.GetAgentByAgentID(AgentID)("Login"))
            End If

            If PlayerID <> "" Then
                odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByPlayer(PlayerID, SDate, EDate, SelectedTypeOfBet)

            Else
                odtHistoryTickets = (New CTicketManager).GetALLHistoryTicketsByListAgentID(getListSubAgentID(AgentID), SDate, EDate, SelectedTypeOfBet)
            End If

            ucHistoryGrid.LoadHistoryTickets(odtHistoryTickets)
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

        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            bindTickets()
        End Sub

    End Class
End Namespace