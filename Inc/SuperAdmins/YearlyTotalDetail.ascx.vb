Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class YearlyTotalDetail
        Inherits SBCBL.UI.CSBCUserControl

#Region "Properties"
        Public Property IsYTD() As Boolean
            Get
                Return SafeBoolean(ViewState("YTD"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("YTD") = value
            End Set
        End Property

        Public Property IsWeeklyReport() As Boolean
            Get
                Return SafeBoolean(ViewState("IsWeeklyReport"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("IsWeeklyReport") = value
            End Set
        End Property

        Public Property PlayerID() As String
            Get
                Return SafeString(ViewState("PlayerID"))
            End Get
            Set(ByVal value As String)
                ViewState("PlayerID") = value
            End Set
        End Property

        Private ReadOnly Property SuperID() As String
            Get
                If UserSession.SuperAdminInfo.IsPartner Then
                    Return UserSession.SuperAdminInfo.PartnerOf
                Else
                    Return UserSession.UserID
                End If

            End Get
        End Property

        Private _otblMain As DataTable
#End Region

#Region "Bind Data"

        Public Sub bindWeek()
            Dim nTimeZone As Integer = 0
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                nTimeZone = UserSession.SuperAdminInfo.TimeZone
            ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
                nTimeZone = UserSession.AgentUserInfo.TimeZone
            End If
            Dim oDate As Date = GetMondayOfCurrentWeek(nTimeZone)
            Dim olstWeek As New Dictionary(Of String, String)

            For nIndex As Integer = 1 To 8
                Dim oTemp As Date
                If IsWeeklyReport Then
                    oTemp = oDate.AddDays((nIndex - 1) * -7)
                    olstWeek.Add(oTemp.ToString("MM/dd/yyyy") & " - " & oTemp.AddDays(6).ToString("MM/dd/yyyy"), oTemp.ToString("MM/dd/yyyy"))
                Else
                    oTemp = oDate.AddYears((nIndex - 1) * (-1))
                    olstWeek.Add(oTemp.Year.ToString(), oTemp.ToString("MM/dd/yyyy"))
                End If

            Next

            ddlWeeks.DataSource = olstWeek
            ddlWeeks.DataTextField = "Key"
            ddlWeeks.DataValueField = "value"
            ddlWeeks.DataBind()

            If ddlWeeks.Items.Count > 0 Then
                ddlWeeks.SelectedIndex = 0
            End If
        End Sub

        Public Sub bindReport()
            Dim oSDateFrom As DateTime
            Dim oSDateTo As DateTime
            If Not IsWeeklyReport Then
                oSDateFrom = ("1/1/" & SafeDate(ddlWeeks.Value).Year)
                oSDateTo = ("12/31/" & SafeDate(ddlWeeks.Value).Year)
            Else
                oSDateFrom = SafeDate(ddlWeeks.Value)
                oSDateTo = oSDateFrom.AddDays(6)
            End If

            If IsYTD Then
                _otblMain = (New CTicketManager).GetAllCompletedTicketsListByPlayerID(PlayerID, SafeDate("1/1/" & GetEasternDate.Year), GetEasternDate())
                trTop.Visible = False
            Else
                _otblMain = (New CTicketManager).GetAllCompletedTicketsListByPlayerID(PlayerID, oSDateFrom, oSDateTo)
                trTop.Visible = True

            End If
            bindStraightReport()
            bindOtherReport()


        End Sub

        Public Sub bindStraightReport()
            Dim nTotalBetAmount As Double = 0
            Dim nTotalWinLose As Double = 0
            Dim nTotalOfBet As Integer = 0
            If _otblMain IsNot Nothing Then
                nTotalBetAmount = SafeDouble(_otblMain.Compute("Sum(RiskAmount)", "TicketType = 'Straight'"))
                nTotalWinLose = SafeDouble(_otblMain.Compute("Sum(NetAmount)", "TicketType = 'Straight'"))
                nTotalOfBet = SafeDouble(_otblMain.Compute("Count(TicketCompletedDate)", "TicketType ='Straight'"))
            End If
            Dim dt As DataTable = New DataTable()
            Dim dcTotalOfBet As DataColumn = New DataColumn("TotalOfBet")
            Dim dcTotalBetAmount As DataColumn = New DataColumn("TotalBetAmount")
            Dim dcTotalWinLose As DataColumn = New DataColumn("TotalWinLose")
            Dim dcPlPercentage As DataColumn = New DataColumn("PlPercentage")
            dcTotalOfBet.DefaultValue = FormatNumber(nTotalOfBet, 2)

            dcTotalBetAmount.DefaultValue = FormatNumber(nTotalBetAmount, 2)

            dcTotalWinLose.DefaultValue = FormatNumber(nTotalWinLose, 2)
            dcPlPercentage.DefaultValue = FormatNumber(IIf(nTotalBetAmount = 0, 0, SafeDouble(nTotalWinLose / nTotalBetAmount)) * 100, 2) & "%"
            dt.Columns.Add(dcTotalOfBet)
            dt.Columns.Add(dcTotalBetAmount)
            dt.Columns.Add(dcTotalWinLose)
            dt.Columns.Add(dcPlPercentage)
            dt.Rows.Add(dt.NewRow)
            dgStraight.DataSource = New DataView(dt)
            dgStraight.DataBind()



        End Sub

        Public Sub bindOtherReport()
            Dim nTotalBetAmount As Double = 0
            Dim nTotalWinLose As Double = 0
            Dim nTotalOfBet As Integer = 0
            If _otblMain IsNot Nothing Then
                nTotalBetAmount = SafeDouble(_otblMain.Compute("Sum(RiskAmount)", "TicketType <> 'Straight'"))
                nTotalWinLose = SafeDouble(_otblMain.Compute("Sum(NetAmount)", "TicketType <> 'Straight'"))
                nTotalOfBet = SafeInteger(_otblMain.Compute("Count(TicketCompletedDate)", "TicketType <>'Straight'"))
            End If
            Dim dt As DataTable = New DataTable()
            Dim dcTotalOfBet As DataColumn = New DataColumn("TotalOfBet")
            Dim dcTotalBetAmount As DataColumn = New DataColumn("TotalBetAmount")
            Dim dcTotalWinLose As DataColumn = New DataColumn("TotalWinLose")
            Dim dcPlPercentage As DataColumn = New DataColumn("PlPercentage")

            dcTotalOfBet.DefaultValue = FormatNumber(nTotalOfBet, 2)
            dcTotalBetAmount.DefaultValue = FormatNumber(nTotalBetAmount, 2)
            dcTotalWinLose.DefaultValue = FormatNumber(nTotalWinLose, 2)
            dcPlPercentage.DefaultValue = FormatNumber(IIf(nTotalBetAmount = 0, 0, SafeDouble(nTotalWinLose / nTotalBetAmount)) * 100, 2) & "%"

            dt.Columns.Add(dcTotalOfBet)
            dt.Columns.Add(dcTotalBetAmount)
            dt.Columns.Add(dcTotalWinLose)
            dt.Columns.Add(dcPlPercentage)
            dt.Rows.Add(dt.NewRow)
            dgOther.DataSource = New DataView(dt)
            dgOther.DataBind()



        End Sub

#End Region

#Region "Page Event"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not IsPostBack Then
                bindWeek()
                'bindAgents()
            End If
        End Sub

        Protected Sub ddlWeeks_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
            bindReport()
        End Sub
#End Region
    End Class
End Namespace