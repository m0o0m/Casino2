Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSCallCenterAgents

    Partial Class OpenBets
        Inherits SBCBL.UI.CSBCPage
        Implements WebsiteLibrary.IParentPagePostback

#Region "Page events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Pending Bets"
            MenuTabName = "OPEN_BET"

            If Not IsPostBack Then
                bindCCAgent()
                bindPlayers()
                ddlPlayers.SelectedValue = UserSession.CCAgentUserInfo.PlayerID
                tablePlayer.Visible = UserSession.CCAgentUserInfo.PlayerID = ""
                ucPlayerProfile.Visible = UserSession.CCAgentUserInfo.PlayerID <> ""
            End If
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                loadTickets()
            End If
        End Sub

#End Region

#Region "Bind data"
        Private Sub bindCCAgent()
            ddlCAgent.DataSource = (New CCallCenterAgentManager).GetAgents(SBCBL.std.GetSiteType)
            ddlCAgent.DataTextField = "FullName"
            ddlCAgent.DataValueField = "CallCenterAgentID"
            ddlCAgent.DataBind()
            ddlCAgent.SelectedValue = UserSession.UserID
        End Sub


        Private Sub bindPlayers()
            ddlPlayers.DataSource = (New CPlayerManager).GetAllPlayer(SBCBL.std.GetSiteType)
            ddlPlayers.DataTextField = "FullPhoneName"
            ddlPlayers.DataValueField = "PlayerID"
            ddlPlayers.DataBind()
            ddlPlayers.SelectedIndex = 0
        End Sub

#End Region

        Private Sub loadTickets()
            Dim oTicketBets As DataTable = (New CTicketManager).GetOpenTicketsByCCAgent(ddlCAgent.SelectedValue, ddlPlayers.SelectedValue, Nothing, "Phone")
            ucTicketBetsGridAgent.LoadTicketBets(oTicketBets, ddlContext.SelectedValue)
            ucTicketBetsGridAgent.ShowPlayerColumn = UserSession.CCAgentUserInfo.PlayerID = ""
        End Sub

        Public Sub ChildClose(ByVal sData As String) Implements WebsiteLibrary.IParentPagePostback.ChildClose
            Select Case sData
                Case "REFRESH_PAGE"
                    loadTickets()
            End Select
        End Sub

        Protected Sub ddlPlayers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPlayers.SelectedIndexChanged
            loadTickets()
        End Sub

        Protected Sub ddlCAgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCAgent.SelectedIndexChanged
            loadTickets()
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            loadTickets()
        End Sub
    End Class

End Namespace

