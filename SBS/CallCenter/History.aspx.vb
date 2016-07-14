Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data

Namespace SBSCallCenterAgents
    Partial Class History
        Inherits SBCBL.UI.CSBCPage
        Implements WebsiteLibrary.IParentPagePostback
        Private ReadOnly ALL As String = "ALL"

#Region "Properties"

        Private ReadOnly Property CCAgentID() As String
            Get
                Dim sCCAgentID As String = SafeString(Request("CCAgentID"))

                If sCCAgentID = "" Then
                    sCCAgentID = UserSession.UserID
                End If

                Return sCCAgentID
            End Get
        End Property

        Public ReadOnly Property SelectedCAgent() As String
            Get
                Return SafeString(ddlCAgent.SelectedValue)
            End Get
        End Property

        Public ReadOnly Property SelectedPlayer() As String
            Get
                Return SafeString(ddlPlayers.SelectedValue)
            End Get
        End Property
#End Region

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Your History"
            MenuTabName = "HISTORY"
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                txtDateFrom.Text = GetMondayOfCurrentWeek().ToShortDateString
                txtDateTo.Text = GetMondayOfCurrentWeek().AddDays(6).ToShortDateString
                bindCCAgent()
                bindPlayer()
                bindHistoryTickets()
            End If
        End Sub

#End Region

#Region "Bind data"


        Private Sub bindCCAgent()
            ddlCAgent.DataSource = (New CCallCenterAgentManager).GetAgents(SBCBL.std.GetSiteType)
            ddlCAgent.DataTextField = "FullName"
            ddlCAgent.DataValueField = "CallCenterAgentID"
            ddlCAgent.OptionalText = ALL
            ddlCAgent.OptionalValue = ALL
            ddlCAgent.DataBind()
            ddlCAgent.SelectedValue = UserSession.UserID
        End Sub
        Private Sub bindPlayer()
            ddlPlayers.DataSource = (New CPlayerManager).GetAllPlayer(SBCBL.std.GetSiteType)
            ddlPlayers.DataTextField = "FullPhoneName"
            ddlPlayers.DataValueField = "PlayerID"
            ddlPlayers.OptionalText = ALL
            ddlPlayers.OptionalValue = ALL
            ddlPlayers.DataBind()
            ddlPlayers.SelectedIndex = 0
        End Sub

        Private Sub bindHistoryTickets()
            Dim oTransFrom As DateTime = UserSession.ConvertToGMT(txtDateFrom.Text)
            Dim oTransTo As DateTime = UserSession.ConvertToGMT(txtDateTo.Text)
            Dim odtHistoryTickets As DataTable
            If SelectedCAgent.Equals(ALL) Then
                odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByALLCCAgent(oTransFrom, oTransTo, SBCBL.std.GetSiteType, "", hasPhoneLog())
                ucHistoryGrid.ShowCAgentName = True
                ucHistoryGrid.ShowPlayerName = True
            Else
                If SelectedPlayer.Equals(ALL) Then
                    odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByCCAgent(SelectedCAgent, oTransFrom, oTransTo, "", hasPhoneLog())
                    ucHistoryGrid.ShowPlayerName = True
                    ucHistoryGrid.ShowCAgentName = False
                Else
                    odtHistoryTickets = (New CTicketManager).GetHistoryTicketsByCCAgent(SelectedCAgent, oTransFrom, oTransTo, SelectedPlayer, hasPhoneLog())
                    ucHistoryGrid.ShowPlayerName = False
                    ucHistoryGrid.ShowCAgentName = False
                End If

            End If
            If odtHistoryTickets IsNot Nothing Then
                ucHistoryGrid.LoadHistoryTickets(odtHistoryTickets)
            End If
            'ucHistoryGrid.ResultGrid.Columns(5).Visible = True

        End Sub

#End Region

        Private Function hasPhoneLog() As String

            Dim sHasPhoneLog As String = ""
            If rdYes.Checked Then
                sHasPhoneLog = "Y"
            End If
            If rdNo.Checked Then
                sHasPhoneLog = "N"
            End If
            Return sHasPhoneLog
        End Function

        Public Sub ChildClose(ByVal sData As String) Implements WebsiteLibrary.IParentPagePostback.ChildClose
            Select Case sData
                Case "REFRESH_PAGE"
                    bindHistoryTickets()
            End Select
        End Sub
#Region "event"
        Protected Sub rdNo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdNo.CheckedChanged
            bindHistoryTickets()
        End Sub

        Protected Sub rdYes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdYes.CheckedChanged
            bindHistoryTickets()
        End Sub

        Protected Sub rdDontCare_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdDontCare.CheckedChanged
            bindHistoryTickets()
        End Sub

        Protected Sub ddlCAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCAgent.SelectedIndexChanged
            bindHistoryTickets()
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            bindHistoryTickets()
        End Sub

        Protected Sub txtDateRange_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDateFrom.TextChanged, txtDateTo.TextChanged
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub
#End Region
    End Class

End Namespace