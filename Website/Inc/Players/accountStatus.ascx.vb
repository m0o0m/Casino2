Imports System.Drawing
Imports SBCBL.std
Imports System.Data
Imports SBCBL.Managers

Namespace SBCPlayer

    Partial Class accountStatus
        Inherits SBCBL.UI.CSBCUserControl

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Me.IsPostBack Then
                loadAccountStatus()
            End If
        End Sub

        Private Sub loadAccountStatus()
            Dim curBalance = SafeRound(UserSession.PlayerUserInfo.OriginalAmount)
            lblCurrentBalance.Text = curBalance
            If curBalance < 0 Then
                lblCurrentBalance.ForeColor = Color.Red
            End If

            Dim pendingAmt = SafeRound(UserSession.PlayerUserInfo.PendingAmount)
            lblPendingAmount.Text = pendingAmt
            If pendingAmt < 0 Then
                lblPendingAmount.ForeColor = Color.Red
            End If

            Dim avaiBalance = SafeRound(UserSession.PlayerUserInfo.BalanceAmount)
            lblAvailableBalance.Text = avaiBalance
            If avaiBalance < 0 Then
                lblAvailableBalance.ForeColor = Color.Red
            End If

            lblAcct.Text = UserSession.PlayerUserInfo.Name

            Dim oDate As Date = SBCBL.std.GetEasternMondayOfCurrentWeek()

            Dim oTickets As DataTable = (New CPlayerManager()).GetPlayerDashboard(UserSession.UserID, oDate, oDate.AddDays(6), UserSession.PlayerUserInfo.TimeZone)
            If (oTickets.Rows.Count > 0) Then
                Dim tWeekAmt = SafeRound(oTickets.Rows(0)("Net"))
                lblThisWeek.Text = tWeekAmt

                If tWeekAmt < 0 Then
                    lblThisWeek.ForeColor = Color.Red
                End If
            End If

            oTickets = (New CPlayerManager()).GetPlayerDashboard(UserSession.UserID, oDate.AddDays(-7), oDate.AddDays(-1), UserSession.PlayerUserInfo.TimeZone)
            If (oTickets.Rows.Count > 0) Then
                Dim lWeekAmt = SafeRound(oTickets.Rows(0)("Net"))
                lblLastWeek.Text = lWeekAmt

                If lWeekAmt < 0 Then
                    lblLastWeek.ForeColor = Color.Red
                End If
            End If
        End Sub

    End Class

End Namespace

