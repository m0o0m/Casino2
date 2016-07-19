Imports System.Data
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSAgents

    Partial Class DifferenceAmount
        Inherits SBCBL.UI.CSBCPage
        Private ALL As String = "All"


#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Difference Amount"
            MenuTabName = "HOME"
            SubMenuActive = "Difference Amount"
            DisplaySubTitlle(Me.Master, "Difference Amount")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                bindTickets()

                tmrRefresh.Enabled = True

            End If
        End Sub

#End Region

#Region "Bind data"


        Private Sub bindTickets()
            Dim strGameID, strGameType, strContext, strAgentID As String
            strGameID = Request.QueryString("GameID")
            strGameType = Request.QueryString("GameType")
            strContext = Request.QueryString("Context")
            strAgentID = Request.QueryString("AgentID")
            If Not String.IsNullOrEmpty(strAgentID) Then
                ucTicketBetsGridAgent.LoadTicketBets((New CTicketManager).GetDiffOpenTicketsByAllSubAgent(strGameID, strGameType, strContext, getListSubAgentID(strAgentID), Nothing, "All"), strContext)
            Else
                ucTicketBetsGridAgent.LoadTicketBets((New CTicketManager).GetDiffOpenTicketsByAllSubAgent(strGameID, strGameType, strContext, getListSubAgentID(""), Nothing, "All"), strContext)
            End If
            'ucTicketBetsGridAgent.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
        End Sub

#End Region

        Protected Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
            bindTickets()
        End Sub


        Private Function getListSubAgentID(ByVal psSuperID As String) As List(Of String)
            Dim olstSubAgent As List(Of String) = New List(Of String)
            Dim odtSubAgent As DataTable
            If psSuperID <> "" Then
                odtSubAgent = (New CAgentManager).GetAllAgentsByAgentID(psSuperID, False)
            Else
                odtSubAgent = (New CAgentManager).GetAllAgentsBySuperAdminID(UserSession.UserID, False)
            End If
            For Each odrSubAgent As DataRow In odtSubAgent.Rows

                olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))

            Next
            If Not psSuperID = "" Then
                olstSubAgent.Add(psSuperID)
            End If

            Return olstSubAgent
        End Function


    End Class

End Namespace

