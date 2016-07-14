Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class History
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        Public ReadOnly Property SelectedTypeOfBet() As String
            Get
                Return SafeString(ddlTypeOfBet.SelectedValue)
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "History"
            MenuTabName = "HISTORY"
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                Dim sDateRange As String = SafeString(Request("dateRange"))

                If sDateRange <> "" Then
                    Dim oDates As String() = sDateRange.Split(New Char() {"-"c}, StringSplitOptions.RemoveEmptyEntries)

                    Dim oDateTmp As Date = SafeDate(oDates(0))
                    If oDateTmp <> Nothing And oDateTmp <> DateTime.MinValue Then
                        txtDateFrom.Text = oDateTmp.ToShortDateString
                    End If

                    If oDates.Length = 2 Then
                        oDateTmp = SafeDate(oDates(1))
                        If oDateTmp <> Nothing And oDateTmp <> DateTime.MinValue Then
                            txtDateTo.Text = oDateTmp.ToShortDateString
                        End If
                    End If

                Else
                    txtDateFrom.Text = GetLastMondayOfDate(GetEasternDate).ToShortDateString
                    txtDateTo.Text = GetEasternDate.ToShortDateString
                End If

                bindHistoryTickets()
            End If
        End Sub
#End Region

#Region "Bind data"

        Private Sub bindHistoryTickets()
            '' search by GameDate in EST time
            Dim oDateFrom As DateTime = SafeDate(txtDateFrom.Text)
            Dim oDateTo As DateTime = SafeDate(txtDateTo.Text)

            Dim odtHistoryTickets As DataTable = (New CTicketManager).GetHistoryTicketsByPlayer(UserSession.PlayerUserInfo.UserID, oDateFrom, oDateTo, SelectedTypeOfBet)

            ucHistoryGrid.LoadHistoryTickets(odtHistoryTickets)

        End Sub

#End Region

        Protected Sub txtDateRange_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDateFrom.TextChanged, txtDateTo.TextChanged
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub

        Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub

    End Class

End Namespace