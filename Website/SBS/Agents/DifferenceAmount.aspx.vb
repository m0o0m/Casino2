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
            Dim strGameID, strGameType, strContext As String
            strGameID = Request.QueryString("GameID")
            strGameType = Request.QueryString("GameType")
            strContext = Request.QueryString("Context")
            ucTicketBetsGridAgent.LoadTicketBets((New CTicketManager).GetDiffOpenTicketsByAllSubAgent(strGameID, strGameType, strContext, getListSubAgentID(UserSession.UserID), Nothing, "All"), strContext)
            'ucTicketBetsGridAgent.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
        End Sub

#End Region

        Protected Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
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

