Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class HistoryDetail
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

       Public ReadOnly Property StartDate() As String
            Get
                Return SafeString(Request.QueryString("date"))
            End Get

        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "History"
            MenuTabName = "HISTORY"
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
             If Not Me.IsPostBack Then
                bindHistoryTickets()
            End If
        End Sub
#End Region

#Region "Bind data"

        Private Sub bindHistoryTickets()
            '' search by GameDate in EST time
            Dim oDateFrom As DateTime = SafeDate(StartDate)
            Dim oDateTo As DateTime = oDateFrom

            lblActivityDate.Text = "Activity For " & oDateFrom.ToString("ddd dd, MMM MM")

            Dim odtHistoryTickets As DataTable = (New CTicketManager).GetHistoryTicketsByPlayer(UserSession.PlayerUserInfo.UserID, oDateFrom, oDateTo, "All")

            ucHistoryGridDetail.LoadHistoryTickets(odtHistoryTickets)

        End Sub

#End Region

    End Class

End Namespace