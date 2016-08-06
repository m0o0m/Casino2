Imports System.Data

Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class History
        Inherits SBCBL.UI.CSBCPage

#Region "Page events"

        'Public ReadOnly Property SelectedTypeOfBet() As String
        '    Get
        '        Return SafeString(ddlTypeOfBet.SelectedValue)
        '    End Get
        'End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "History"
            MenuTabName = "HISTORY"
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                'Dim sDateRange As String = SafeString(Request("dateRange"))

                'If sDateRange <> "" Then
                '    Dim oDates As String() = sDateRange.Split(New Char() {"-"c}, StringSplitOptions.RemoveEmptyEntries)

                '    Dim oDateTmp As Date = SafeDate(oDates(0))
                '    If oDateTmp <> Nothing And oDateTmp <> DateTime.MinValue Then
                '        txtDateFrom.Text = oDateTmp.ToShortDateString
                '    End If

                '    If oDates.Length = 2 Then
                '        oDateTmp = SafeDate(oDates(1))
                '        If oDateTmp <> Nothing And oDateTmp <> DateTime.MinValue Then
                '            txtDateTo.Text = oDateTmp.ToShortDateString
                '        End If
                '    End If

                'Else
                '    txtDateFrom.Text = GetLastMondayOfDate(GetEasternDate).ToShortDateString
                '    txtDateTo.Text = GetEasternDate.ToShortDateString
                'End If
                BindWeeklyDropdownList()

                bindHistoryTickets()
            End If
        End Sub
#End Region

#Region "Bind data"

        Private Sub bindHistoryTickets()
            '' search by GameDate in EST time
            Dim oDateFrom As DateTime = SafeDate(ddlWeekly.SelectedValue)
            Dim oDateTo As DateTime = oDateFrom.AddDays(7)
            Dim sTicketStatus As List(Of String) = New List(Of String)()
            If cbWin.Checked Then
                sTicketStatus.Add("Win")
            End If
            If cbLose.Checked Then
                sTicketStatus.Add("Lose")
            End If
            If cbCanceled.Checked Then
                sTicketStatus.Add("Canceled")
            End If


            Dim odtHistoryTickets As DataTable = (New CTicketManager).GetHistoryTicketsByPlayerWithTicketStatus(UserSession.PlayerUserInfo.UserID, oDateFrom, oDateTo, sTicketStatus)

            ucHistoryGrid.LoadHistoryTickets(odtHistoryTickets)

        End Sub

#End Region

        'Protected Sub txtDateRange_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDateFrom.TextChanged, txtDateTo.TextChanged
        '    ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
        '    bindHistoryTickets()
        'End Sub

        'Protected Sub ddlTypeOfBet_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTypeOfBet.SelectedIndexChanged
        '    ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
        '    bindHistoryTickets()
        'End Sub

        Protected Sub btnRefreshHistory_Click(sender As Object, e As EventArgs) Handles btnRefreshHistory.Click
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub
        Protected Sub ddlWeekly_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlWeekly.SelectedIndexChanged
            ucHistoryGrid.ResultGrid.CurrentPageIndex = 0
            bindHistoryTickets()
        End Sub


        Public Function FirstDayInWeek(ByVal datetime As DateTime) As DateTime
            Dim monday As DateTime = datetime.AddDays((Today.DayOfWeek - DayOfWeek.Monday) * -1)

            Return monday
        End Function

        Protected Sub BindWeeklyDropdownList()
            Dim currentMonday As DateTime = FirstDayInWeek(Today)
            Dim lasMonday As DateTime = FirstDayInWeek(Today.AddMonths(-3))

            ' Add current week
            Dim currentWeek As ListItem = New ListItem("Current Week", currentMonday.ToString("MM/dd/yyyy"))
            currentWeek.Selected = True
            ddlWeekly.Items.Add(currentWeek)

            Dim tempMonday = currentMonday.AddDays(-7)
            While (lasMonday < tempMonday)
                Dim sValue As String = tempMonday.ToString("MM/dd/yyyy")
                Dim tempWeek As ListItem = New ListItem(sValue, sValue)
                tempWeek.Selected = False
                ddlWeekly.Items.Add(New ListItem(sValue, sValue))

                tempMonday = tempMonday.AddDays(-7)
            End While
        End Sub

    End Class

End Namespace