Imports SBCBL.Managers
Imports SBCBL.std
Imports System.Data
Namespace SBCWebsite
    Partial Class HeadCount
        Inherits SBCBL.UI.CSBCUserControl

        Public ReadOnly Property WeeklyCharge() As Double
            Get
                Dim oAgentManager As New CAgentManager()
                Dim dtAgent As DataTable = oAgentManager.GetByID(UserSession.UserID)
                If dtAgent IsNot Nothing AndAlso dtAgent.Rows.Count > 0 Then
                    Return SafeDouble(dtAgent.Rows(0)("WeeklyCharge"))
                End If

                Return 0
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack() Then
                bindWeek()
                lblAgent.Text = UserSession.AgentUserInfo.Login
                'lblCurrentBalance.Text = UserSession.AgentUserInfo.BalanceAmount
                bindData()
            End If
        End Sub

        Private Sub bindWeek()
            Dim oDate As Date = GetEasternMondayOfCurrentWeek()
            Dim olstWeek As New Dictionary(Of String, String)

            For nIndex As Integer = 1 To 8
                Dim oTemp As Date = oDate.AddDays((nIndex - 1) * -7)
                olstWeek.Add(oTemp.ToString("MM/dd/yyyy") & " - " & oTemp.AddDays(6).ToString("MM/dd/yyyy"), oTemp.ToString("MM/dd/yyyy"))
            Next

            ddlWeeks.DataSource = olstWeek
            ddlWeeks.DataTextField = "Key"
            ddlWeeks.DataValueField = "value"
            ddlWeeks.DataBind()
        End Sub

        Private Sub bindData()
            Dim tblActivePlayersInternet = (New CAgentManager()).GetActivePlayersByTypeOfBet(UserSession.UserID, ddlWeeks.Value, _
                                                              SafeDate(ddlWeeks.Value).AddDays(6), "Internet")
            Dim tblActivePlayersPhone = (New CAgentManager()).GetActivePlayersByTypeOfBet(UserSession.UserID, ddlWeeks.Value, _
                                                              SafeDate(ddlWeeks.Value).AddDays(6), "Phone")

            Dim tblActivePlayersCasino = (New CAgentManager()).GetActivePlayersByCasino(UserSession.UserID, ddlWeeks.Value, _
                                                              SafeDate(ddlWeeks.Value).AddDays(6))

            'Dim tblBalanceActive = (New CAgentManager()).CalBalanceActive(UserSession.UserID, ddlWeeks.Value, _
            '                                                 SafeDate(ddlWeeks.Value).AddDays(6))
            Dim numCasinoProFit As Double = 0
            If tblActivePlayersCasino IsNot Nothing AndAlso tblActivePlayersCasino.Rows.Count > 0 Then
                numCasinoProFit = SafeDouble(tblActivePlayersCasino.Rows(0)("NetAmount"))
            End If
            Dim numInAcc As Integer = tblActivePlayersInternet.Rows.Count
            Dim numPhoneAcc As Integer = tblActivePlayersPhone.Rows.Count
            Dim nPriceIn = FormatNumber(WeeklyCharge, GetRoundMidPoint)
            lblNumAccInternet.Text = numInAcc
            lblNumAccInternet2.Text = numInAcc
            lblInternetPrice.Text = nPriceIn
            lblPhonePrice.Text = nPriceIn
            lblNumAccPhone.Text = numPhoneAcc
            lblNumAccPhone2.Text = numPhoneAcc
            lblTotalIn.Text = numInAcc * nPriceIn
            lblTotalIn2.Text = numInAcc * nPriceIn
            lblTotalPhone.Text = numPhoneAcc * nPriceIn
            lblTotalPhone2.Text = numPhoneAcc * nPriceIn
            'lblPercenstage.Text = UserSession.AgentUserInfo.ProfitPercentage & "%"
            lblTotalAmount.Text = (numInAcc * nPriceIn) + (numPhoneAcc * nPriceIn)
            lblTotalAmount2.Text = (numInAcc * nPriceIn) + (numPhoneAcc * nPriceIn)
            lblCasinoProfit.Text = numCasinoProFit
            'If tblBalanceActive IsNot Nothing AndAlso tblBalanceActive.Rows.Count > 0 Then
            '    Dim odr = tblBalanceActive.Rows(0)
            '    lblTotalMaxCredit.Text = odr("CreditMaxAmount")
            '    lblTotalAccountbalance.Text = odr("BalanceAmount")
            '    lblCurrentBalance.Text = odr("CurrentBalance")
            'End If



        End Sub

        Protected Sub btnSeach_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSeach.Click
            bindData()
        End Sub
    End Class
End Namespace
