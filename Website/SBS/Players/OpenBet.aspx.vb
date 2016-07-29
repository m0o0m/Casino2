Imports System.Data
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class OpenBet
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"
        Public ReadOnly Property StartDate() As String
            Get
                Return SafeString(Request.QueryString("SDate"))
            End Get

        End Property

        Public ReadOnly Property EndDate() As String
            Get
                Return SafeString(Request.QueryString("EDate"))
            End Get

        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Pending Bets"
            MenuTabName = "OPEN_BET"            
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                'tmrRefresh.Enabled = True
                bindTickets()
            End If
        End Sub

        Public ReadOnly Property SelectedTypeOfBet() As String
            Get
                Return SafeString(ddlTypeOfBet.SelectedValue)
            End Get
        End Property
#End Region

#Region "Bind data"

        Private Sub bindTickets()
            Dim oTicketBets As DataTable

            If StartDate <> "" AndAlso EndDate <> "" Then
                oTicketBets = (New CTicketManager).GetOpenTicketsByPlayer(UserSession.PlayerUserInfo.UserID, EndDate, SelectedTypeOfBet)
            Else
                oTicketBets = (New CTicketManager).GetOpenTicketsByPlayer(UserSession.PlayerUserInfo.UserID, Nothing, SelectedTypeOfBet)
            End If

            ucTicketBetsGrid.LoadTicketBets(oTicketBets, ddlContext.SelectedValue, ddlGameType.SelectedValue)
        End Sub

        

#End Region

        'Protected Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
        '    bindTickets()
        'End Sub

        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            bindTickets()
        End Sub

        Protected Sub ddlContext_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlContext.SelectedIndexChanged
            bindTickets()
        End Sub

        Protected Sub ddlGameType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlGameType.SelectedIndexChanged
            bindTickets()
        End Sub

    End Class

End Namespace

