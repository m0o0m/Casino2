Imports SBCBL.std
Imports SBCBL.Managers
Imports System.Data

Namespace SBCSuperAdmin
    Partial Class PLReport
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
        Public Property IsDailyReport() As Boolean
            Get
                Return SafeBoolean(ViewState("_IS_DAILY_REPORT"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_IS_DAILY_REPORT") = value
            End Set
        End Property

        Public ReadOnly Property SelectedAgent() As String
            Get
                Return SafeString(ddlAgents.SelectedValue)
            End Get
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
        Private ALL As String = "ALL"
#End Region

#Region "Bind Data"

        Private Sub bindAgents()
            bindWeek()
            Dim oAgentManager As New CAgentManager()
            If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                ddlAgents.DataSource = oAgentManager.GetAgentsBySuperID(SuperID, Nothing)
            Else
                ddlAgents.DataSource = oAgentManager.GetAgentsByAgentID(UserSession.UserID, Nothing)
            End If
            ddlAgents.DataTextField = "FullName"
            ddlAgents.DataValueField = "AgentID"
            ddlAgents.DataBind()
            ddlAgents.Items.Insert(0, ALL)
            ddlAgents.SelectedIndex = 0
        End Sub

        Private Sub bindWeek()
            'Dim nTimeZone As Integer = 0
            'If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
            '    nTimeZone = UserSession.SuperAdminInfo.TimeZone
            'ElseIf UserSession.UserType = SBCBL.EUserType.Agent Then
            '    nTimeZone = UserSession.AgentUserInfo.TimeZone
            'End If

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

            If ddlWeeks.Items.Count > 0 Then
                ddlWeeks.SelectedIndex = 0
            End If
        End Sub

        Public Sub bindReport()
            Dim oSDate As DateTime = SafeDate(ddlWeeks.Value)
            Dim oLstAgentID As New List(Of String)
            'bind report for Super when select all
            If SelectedAgent.Equals(ALL) Then
                If UserSession.UserType = SBCBL.EUserType.SuperAdmin Then
                    Dim oAgentManager As New CAgentManager
                    Dim dtParents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(SuperID, Nothing)
                    'Dim oLstAgentID As New List(Of String)
                    For Each oDR As DataRow In dtParents.Rows
                        oLstAgentID.Add(SafeString(oDR("AgentID")))
                    Next
                    If Not IsYTD Then
                        _otblMain = (New CTicketManager).GetAllCompletedTicketsListByAgentID(oLstAgentID, oSDate, oSDate.AddDays(6))
                    End If
                Else
                    'bind report for agent have subagent when select all
                    oLstAgentID = (New CAgentManager()).GetAllSubAgentIDs(UserSession.UserID)
                    oLstAgentID.Add(UserSession.UserID)
                    If Not IsYTD Then
                        _otblMain = (New CTicketManager).GetAllCompletedTicketsListByAgentID(oLstAgentID, oSDate, oSDate.AddDays(6))
                    End If
                    End If
            Else
                    oLstAgentID = (New CAgentManager()).GetAllSubAgentIDs(SelectedAgent)
                    oLstAgentID.Add(SelectedAgent)
                    If Not IsYTD Then
                        _otblMain = (New CTicketManager).GetAllCompletedTicketsListByAgentID(oLstAgentID, oSDate, oSDate.AddDays(6))
                    End If
                End If


                Dim oDay As String()

                If IsDailyReport Then
                    oDay = New String() {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"}
                Else
                    oDay = New String() {"Week"}
                End If

                If IsYTD Then
                '_otblMain = (New CTicketManager).GetAllCompletedTicketsListByAgentID(oLstAgentID, SafeDate("1/1/" & GetEasternDate.Year), GetEasternDate())
                _otblMain = (New CTicketManager).GetAllCompletedTicketsListByAgentID(oLstAgentID, SafeDate("1/1/2000"), GetEasternDate())
                    pnlWeeks.Visible = False
                Else
                    pnlWeeks.Visible = True

                End If

                dgDailyPLReport.DataSource = oDay
                dgDailyPLReport.DataBind()

                dgDailyPLReport.Columns(0).Visible = IsDailyReport
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

                bindAgents()
            End If
        End Sub

        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
            bindReport()
        End Sub
        Protected Sub ddlWeeks_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWeeks.SelectedIndexChanged
            bindReport()
        End Sub
#End Region

        Protected Sub dgDailyPLReport_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgDailyPLReport.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim sDay As String = SafeString(e.Item.DataItem)
                Dim oDay As Date = SafeDate(ddlWeeks.Value)
                Dim sWhere As String = ""
                Dim nHandleAmount, nNetAmount As Double

                If IsDailyReport Then
                    sWhere = " TicketCompletedDate = {0}"

                    Select Case sDay
                        Case "Mon"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay, "MM/dd/yyyy")))
                        Case "Tue"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(1), "MM/dd/yyyy")))
                        Case "Wed"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(2), "MM/dd/yyyy")))
                        Case "Thu"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(3), "MM/dd/yyyy")))
                        Case "Fri"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(4), "MM/dd/yyyy")))
                        Case "Sat"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(5), "MM/dd/yyyy")))
                        Case "Sun"
                            sWhere = String.Format(sWhere, SQLString(Format(oDay.AddDays(6), "MM/dd/yyyy")))
                    End Select

                Else
                    sWhere = " True"

                End If
                If _otblMain IsNot Nothing Then
                    nHandleAmount = SafeRound(_otblMain.Compute("SUM(RiskAmount)", sWhere))
                    nNetAmount = SafeRound(_otblMain.Compute("SUM(NetAmount)", sWhere))
                Else
                    nHandleAmount = 0
                    nNetAmount = 0
                End If
                If FormatNumber(nHandleAmount, GetRoundMidPoint()) < 0 Then
                    CType(e.Item.FindControl("lblHandle"), Label).ForeColor = Drawing.Color.Red
                End If
                If FormatNumber(nNetAmount, GetRoundMidPoint()) < 0 Then
                    CType(e.Item.FindControl("lblNet"), Label).ForeColor = Drawing.Color.Red
                End If
                CType(e.Item.FindControl("lblDay"), Label).Text = sDay
                CType(e.Item.FindControl("lblHandle"), Label).Text = FormatNumber(nHandleAmount, GetRoundMidPoint())
                CType(e.Item.FindControl("lblNet"), Label).Text = FormatNumber(nNetAmount, GetRoundMidPoint())
                If nHandleAmount > 0 Then
                    CType(e.Item.FindControl("lblHold"), Label).Text = FormatNumber((nNetAmount / nHandleAmount) * 100, 2)
                    If FormatNumber((nNetAmount / nHandleAmount) * 100, 2) < 0 Then
                        CType(e.Item.FindControl("lblHold"), Label).ForeColor = Drawing.Color.Red
                    End If
                End If

            End If
        End Sub

    End Class
End Namespace
