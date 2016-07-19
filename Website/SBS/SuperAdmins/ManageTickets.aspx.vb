Imports SBCBL.std
Imports System.Data
Imports SBCBL.Managers

Namespace SBSSuperAdmin
    Partial Class ManageTickets
        Inherits SBCBL.UI.CSBCPage
        Private HistoryTicketsCount As Int32 = 0
        Private TotalBets As Double = 0
        Private TotalRisk As Double = 0
        Private TotalWin As Double = 0
        Private TotalBalance As Double = 0
        Private All As String = "All"
        Private __CurrentTicketID As String = "__CURRENT"
        Private RoundMidPoint As Double = GetRoundMidPoint()

        Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PageTitle = "Manage Player Tickets"
            SideMenuTabName = "MANAGE_TICKETS"
            Me.MenuTabName = "GAME MANAGEMENT"
            DisplaySubTitlle(Me.Master, "Manage Player Tickets")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'If Not IsPostBack Then
            '    grdTickets.DataSource = Nothing
            '    grdTickets.DataBind()

            '    BindAgents()
            '    ddlPlayer.DataBind()

            'End If
        End Sub

        '#Region "Bind Data"

        '        Private Sub BindAgents()
        '            ddlAgents.DataSource = loadAgents()
        '            ddlAgents.DataTextField = "AgentName"
        '            ddlAgents.DataValueField = "AgentID"
        '            ddlAgents.DataBind()
        '        End Sub

        '        Private Sub BindPlayers()
        '            Dim oPlayer As New SBCBL.Managers.CPlayerManager()
        '            Dim oDT As DataTable = oPlayer.GetPlayers(ddlAgents.SelectedValue, Nothing)
        '            ddlPlayer.DataSource = oDT
        '            ddlPlayer.DataTextField = "Login"
        '            ddlPlayer.DataValueField = "PlayerID"
        '            ddlPlayer.DataBind()
        '        End Sub

        '#End Region

        '        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        '            SearchTickets()
        '        End Sub

        '        Private Sub SearchTickets()
        '            Dim oTicketManager As New SBCBL.Managers.CTicketManager()

        '            If Not String.IsNullOrEmpty(txtTicket.Text) AndAlso SafeInteger(txtTicket.Text) = 0 Then
        '                ClientAlert("Ticket Number Is Wrong")
        '                Return
        '            End If

        '            If ddlAgents.SelectedValue.Equals(All) Then
        '                Dim oDT As DataTable = oTicketManager.SearchTicketByAllSubAgent(getListAgentIDBySuperID(UserSession.UserID), SafeInteger(txtTicket.Text))
        '                Dim oDTDeleted = oTicketManager.GetDeletedTickets()
        '                '' get records of this month
        '                Dim oDate As DateTime = SBCBL.std.GetLastMondayOfDate(GetEasternDate())
        '                oDTDeleted.DefaultView.RowFilter = "DeleteTime > " & SQLString(oDate)
        '                Dim nRowCount As Integer = oDTDeleted.Rows.Count
        '                oDTDeleted = oDTDeleted.DefaultView.ToTable()
        '                If nRowCount <> oDTDeleted.Rows.Count Then
        '                    oTicketManager.CleanDeletedTickets(oDate)
        '                End If
        '                For Each oRowDeleted In oDTDeleted.Rows
        '                    Dim oRow As DataRow = oDT.NewRow()
        '                    oRow("TypeOfBet") = oRowDeleted("TypeOfBet")
        '                    oRow("PlayerName") = oRowDeleted("Player")
        '                    oRow("TicketType") = oRowDeleted("TicketType")
        '                    oRow("TicketNumber") = oRowDeleted("TicketNumber")
        '                    oRow("SubTicketNumber") = oRowDeleted("SubTicketNumber")
        '                    oRow("GameType") = oRowDeleted("GameType")
        '                    '' in datatable, we save description in BetType for deleted tickets
        '                    oRow("BetType") = oRowDeleted("Description")
        '                    oRow("RiskAmount") = oRowDeleted("RiskAmount")
        '                    oRow("WinAmount") = oRowDeleted("WinAmount")
        '                    oRow("NetAmount") = oRowDeleted("NetAmount")
        '                    oRow("GameDate") = oRowDeleted("GameDate")
        '                    oRow("TransactionDate") = oRowDeleted("TransactionDate")
        '                    oRow("TicketID") = Guid.NewGuid.ToString()
        '                    oRow("TicketStatus") = "Deleted"
        '                    oRow("TicketBetStatus") = "Deleted at " & SafeString(UserSession.ConvertToEST(SafeDate(oRowDeleted("DeleteTime"))))
        '                    oDT.Rows.Add(oRow)
        '                Next

        '                oDT.DefaultView.Sort = "TicketNumber , SubTicketNumber ,  TransactionDate DESC ,  TicketID, TicketType"
        '                oDT = oDT.DefaultView.ToTable()

        '                grdTickets.DataSource = oDT
        '                grdTickets.DataBind()
        '                ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
        '                If grdTickets.Items.Count > 1 Then
        '                    formatDisplayGridWithRowSpan(oDT)
        '                End If
        '                Return
        '            End If
        '            If ddlPlayer.SelectedValue.Equals(All) Then
        '                Dim oDT As DataTable = oTicketManager.SearchTicketByAllSubAgent(getListSubAgentID(ddlAgents.SelectedValue), SafeInteger(txtTicket.Text))
        '                grdTickets.DataSource = oDT
        '                grdTickets.DataBind()
        '                ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
        '                If grdTickets.Items.Count > 1 Then
        '                    formatDisplayGridWithRowSpan(oDT)
        '                End If
        '                Return
        '            End If

        '            Dim oDTTicket As DataTable = oTicketManager.SearchTicketByAgent(ddlAgents.SelectedValue, SafeInteger(txtTicket.Text), ddlPlayer.SelectedValue)
        '            grdTickets.DataSource = oDTTicket
        '            grdTickets.DataBind()
        '            ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
        '            If grdTickets.Items.Count > 1 Then
        '                formatDisplayGridWithRowSpan(oDTTicket)
        '            End If



        '        End Sub

        '        Private Sub formatDisplayGridWithRowSpan(ByVal poTicketbets As DataTable)
        '            Dim oCountByTicketIDs As List(Of CItemCount) = (From oRow In poTicketbets.AsEnumerable _
        '                Group oRow By sTicketID = SafeString(oRow.Field(Of Guid)("TicketID")) Into Count() _
        '                Where Count >= 2 _
        '                Select New CItemCount(sTicketID, Count)).ToList()

        '            If oCountByTicketIDs.Count = 0 Then
        '                Return
        '            End If

        '            Dim nIndex As Integer = 0
        '            Dim oBackColor As Drawing.Color = Drawing.Color.White

        '            While nIndex < grdTickets.Items.Count - 1
        '                Dim oItem As DataGridItem = grdTickets.Items(nIndex)
        '                oItem.BackColor = oBackColor

        '                Dim sTicketID As String = SafeString(CType(oItem.FindControl("hfTicketID"), HiddenField).Value)
        '                Dim oCountByTicketID As CItemCount = oCountByTicketIDs.Find(Function(x) x.ItemID = sTicketID)

        '                If oCountByTicketID IsNot Nothing Then
        '                    Dim nRowSpan As Integer = oCountByTicketID.ItemCount

        '                    Dim lblTicket As Label = CType(oItem.FindControl("lblTicket"), Label)
        '                    lblTicket.Text = nRowSpan.ToString() & " Teams<br/>" & lblTicket.Text
        '                    oItem.Cells(0).RowSpan = nRowSpan 'Player
        '                    oItem.Cells(1).RowSpan = nRowSpan 'Internet/Phone
        '                    oItem.Cells(2).RowSpan = nRowSpan 'Ticket
        '                    oItem.Cells(3).RowSpan = nRowSpan 'TicketDate
        '                    oItem.Cells(6).RowSpan = nRowSpan 'Risk/Win
        '                    oItem.Cells(7).RowSpan = nRowSpan 'Amount
        '                    oItem.Cells(11).RowSpan = nRowSpan 'Command

        '                    For nRowSpanStart As Integer = 1 To nRowSpan - 1

        '                        'If (nRowSpanStart + nIndex) < grdTickets.Items.Count Then

        '                        Dim oItemRowSpan As DataGridItem = grdTickets.Items(nRowSpanStart + nIndex)
        '                        oItemRowSpan.BackColor = oBackColor

        '                        oItemRowSpan.Cells(0).Visible = False
        '                        oItemRowSpan.Cells(1).Visible = False
        '                        oItemRowSpan.Cells(2).Visible = False
        '                        oItemRowSpan.Cells(3).Visible = False
        '                        oItemRowSpan.Cells(6).Visible = False
        '                        oItemRowSpan.Cells(7).Visible = False
        '                        oItemRowSpan.Cells(11).Visible = False
        '                        'End If
        '                    Next

        '                    nIndex += nRowSpan
        '                Else
        '                    nIndex += 1
        '                End If

        '                If oBackColor = Drawing.Color.AliceBlue Then
        '                    oBackColor = Drawing.Color.White
        '                Else
        '                    oBackColor = Drawing.Color.AliceBlue
        '                End If
        '            End While
        '        End Sub

        '        Protected Sub grdTickets_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdTickets.ItemCommand
        '            Dim txtAmount As TextBox = CType(e.Item.FindControl("txtAmount"), TextBox)
        '            Dim btnUpdate As Button = CType(e.Item.FindControl("btnUpdate"), Button)
        '            Dim btnCancel As Button = CType(e.Item.FindControl("btnCancel"), Button)

        '            Select Case e.CommandName
        '                Case "DELETE"
        '                    Dim oTicketManager As New SBCBL.Managers.CTicketManager()
        '                    Dim HFPlayerID As HiddenField = CType(e.Item.FindControl("hFPlayerID"), HiddenField)
        '                    Dim oDeletedTicket As New SBCBL.CDeletedTicket()
        '                    oDeletedTicket.TypeOfBet = CType(e.Item.FindControl("lblUserPhone"), Label).Text
        '                    oDeletedTicket.Player = CType(e.Item.FindControl("lblPlayer"), Label).Text
        '                    oDeletedTicket.TicketType = CType(e.Item.FindControl("HFTicketType"), HiddenField).Value
        '                    oDeletedTicket.TicketNumber = SafeInteger(CType(e.Item.FindControl("HFTicketNumber"), HiddenField).Value)
        '                    oDeletedTicket.SubTicketNumber = SafeInteger(CType(e.Item.FindControl("HFSubTicketNumber"), HiddenField).Value)
        '                    oDeletedTicket.GameType = CType(e.Item.FindControl("lblSport"), Label).Text
        '                    oDeletedTicket.Description = CType(e.Item.FindControl("lblDescription"), Label).Text
        '                    oDeletedTicket.RiskAmount = SafeInteger(CType(e.Item.FindControl("HFRiskAmount"), HiddenField).Value)
        '                    oDeletedTicket.WinAmount = SafeInteger(CType(e.Item.FindControl("HFWinAmount"), HiddenField).Value)
        '                    oDeletedTicket.NetAmount = SafeInteger(CType(e.Item.FindControl("HFNetAmount"), HiddenField).Value)
        '                    oDeletedTicket.GameDate = SafeDate(CType(e.Item.FindControl("lblPlaced"), Label).Text)
        '                    oDeletedTicket.TransactionDate = SafeDate(CType(e.Item.FindControl("lblTicketDate"), Label).Text)

        '                    If oTicketManager.DeleteTicket(e.CommandArgument, HFPlayerID.Value, oDeletedTicket) Then
        '                        ClientAlert("Successfully Deleted", True)
        '                        SearchTickets()
        '                    Else
        '                        ClientAlert("Unsuccessfully Deleted")
        '                    End If
        '            End Select
        '        End Sub

        '        Protected Sub grdTickets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdTickets.ItemDataBound
        '            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
        '                Dim oTicketBet As DataRowView = CType(e.Item.DataItem, DataRowView)

        '                'If e.Item.ItemIndex = grdTickets.Items.Count - 1 Then

        '                'Else
        '                CType(e.Item.FindControl("lblPlayer"), Label).Text = SafeString(oTicketBet("PlayerName"))
        '                CType(e.Item.FindControl("lblUserPhone"), Label).Text = SafeString(oTicketBet("TypeOfBet"))
        '                Dim sTicketType = GetTicketType(oTicketBet)
        '                Dim sContext As String = SafeString(IIf(LCase(SafeString(oTicketBet("Context"))) = "current", "", SafeString(oTicketBet("Context")) & "<br/>"))
        '                CType(e.Item.FindControl("lblTicket"), Label).Text = String.Format("{0}<br />#{1}{2}", sTicketType, oTicketBet("TicketNumber"), _
        '                                        IIf(SafeString(oTicketBet("SubTicketNumber")) <> "", "-" & SafeString(oTicketBet("SubTicketNumber")), ""))
        '                CType(e.Item.FindControl("lblTicketDate"), Label).Text = UserSession.ConvertToEST(SafeString(oTicketBet("TransactionDate"))).ToString()

        '                Dim sStatus As String = SafeString(oTicketBet("TicketBetStatus"))
        '                CType(e.Item.FindControl("lblResult"), Label).Text = IIf(UCase(sStatus) = "LOSE", "LOSS", sStatus)

        '                CType(e.Item.FindControl("lblPlaced"), Label).Text = SafeString(oTicketBet("GameDate"))
        '                CType(e.Item.FindControl("lblSport"), Label).Text = SafeString(oTicketBet("GameType"))

        '                ''risk/win
        '                Dim nRisk As Double = SafeRound(oTicketBet("RiskAmount"))
        '                Dim nWin As Double = SafeRound(oTicketBet("WinAmount"))

        '                CType(e.Item.FindControl("lblRiskWin"), Label).Text = String.Format("{0} / {1}", FormatNumber(nRisk, Me.RoundMidPoint), FormatNumber(nWin, Me.RoundMidPoint))

        '                ''amount
        '                Dim nAmount As Double = SafeRound(oTicketBet("NetAmount")) - nRisk
        '                CType(e.Item.FindControl("lblAmount"), Label).Text = SafeString(IIf(nAmount >= 0, "", "-")) & FormatNumber(Math.Abs(nAmount), Me.RoundMidPoint)

        '                Dim sScores As String = "<b>{0}</b> - <b>{1}</b>"
        '                Select Case LCase(SafeString(oTicketBet("Context")))
        '                    Case "current"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwayScore")), SafeString(oTicketBet("HomeScore")))

        '                    Case "1h"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstHalfScore")), SafeString(oTicketBet("HomeFirstHalfScore")))

        '                    Case "2h"
        '                        sScores = String.Format(sScores, SafeString(SafeInteger(oTicketBet("AwayScore")) - SafeInteger(oTicketBet("AwayFirstHalfScore"))), SafeString(SafeInteger(oTicketBet("HomeScore")) - SafeInteger(oTicketBet("HomeFirstHalfScore"))))

        '                    Case "1q"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstQScore")), SafeString(oTicketBet("HomeFirstQScore")))

        '                    Case "2q"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwaySecondQScore")), SafeString(oTicketBet("HomeSecondQScore")))

        '                    Case "3q"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwayThirdQScore")), SafeString(oTicketBet("HomeThirdQScore")))

        '                    Case "4q"
        '                        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFourQScore")), SafeString(oTicketBet("HomeFourQScore")))

        '                    Case Else
        '                        sScores = ""
        '                End Select

        '                CType(e.Item.FindControl("lblScore"), Label).Text = sScores

        '                ''total bet
        '                Dim sTicketID As String = CType(e.Item.FindControl("hfTicketID"), HiddenField).Value

        '                If Me.__CurrentTicketID <> sTicketID Then
        '                    Me.__CurrentTicketID = sTicketID

        '                    Me.TotalBets += 1
        '                    Me.TotalBalance += nAmount
        '                    Me.TotalRisk += nRisk
        '                    Me.TotalWin += nWin
        '                End If

        '                ''description
        '                Dim sHomeTeam As String = SafeString(oTicketBet("HomeTeam"))
        '                Dim sAwayTeam As String = SafeString(oTicketBet("AwayTeam"))
        '                If SafeString(oTicketBet("HomePitcher")) <> "" Then
        '                    sHomeTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("HomePitcher")) & "</span>"
        '                End If
        '                If SafeString(oTicketBet("AwayPitcher")) <> "" Then
        '                    sAwayTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher")) & "</span>"
        '                End If

        '                Dim sDescription As String = ""

        '                '' in datatable ticket , we save description in BetType for deleted tickets
        '                If UCase(SafeString(oTicketBet("TicketStatus"))) = "DELETED" Then
        '                    sDescription = SafeString(oTicketBet("BetType"))
        '                Else
        '                    Select Case UCase(SafeString(oTicketBet("BetType")))
        '                        Case "SPREAD"
        '                            sDescription = getDetailBySpread(sHomeTeam, sAwayTeam, oTicketBet)
        '                        Case "TOTALPOINTS"
        '                            sDescription = getDetailByTotalPoints(sHomeTeam, sAwayTeam, oTicketBet)
        '                        Case "MONEYLINE"
        '                            sDescription = getDetailByMoneyLine(sHomeTeam, sAwayTeam, oTicketBet)
        '                        Case "DRAW"
        '                            sDescription = getDetailByDraw(sHomeTeam, sAwayTeam, oTicketBet)
        '                    End Select
        '                End If



        '                CType(e.Item.FindControl("lblDescription"), Label).Text = sContext & sDescription
        '            ElseIf e.Item.ItemType = ListItemType.Footer Then
        '                e.Item.CssClass = "tableheading2"
        '                e.Item.Cells(4).Text = "TOTAL"
        '                e.Item.Cells(5).Text = FormatNumber(Me.TotalBets, 0) & " BETS"
        '                e.Item.Cells(7).Text = SafeString(IIf(Me.TotalBalance >= 0, "", "-")) & FormatNumber(Math.Abs(Me.TotalBalance), Me.RoundMidPoint)
        '            End If
        '        End Sub

        '        Private Function GetTicketType(ByVal poTicketBet As DataRowView) As String
        '            Dim sType = SafeString(poTicketBet("TicketType"))
        '            Dim oDT As DataTable = CType(grdTickets.DataSource, DataTable)

        '            Select Case sType
        '                Case "Parlay"
        '                    Dim nTicketNumber As Integer = SafeInteger(poTicketBet("TicketNumber"))
        '                    Dim nCountTicket As Integer = 0
        '                    ' count how many ticket has same ticket number with this ticket
        '                    Dim bRobin As Boolean = False
        '                    For Each oRow As DataRow In oDT.Rows
        '                        Dim nSubTicketNumber As Integer = SafeInteger(oRow("SubTicketNumber"))
        '                        Dim nRowTicketNumber As Integer = SafeInteger(oRow("TicketNumber"))
        '                        If nRowTicketNumber <> nTicketNumber Then
        '                            Continue For
        '                        End If

        '                        If nSubTicketNumber > 1 Then
        '                            bRobin = True
        '                            Continue For
        '                        End If
        '                    Next

        '                    If bRobin Then
        '                        Return "Round Robin Parlay"
        '                    End If

        '                Case "Teaser"
        '                    Return SafeString(poTicketBet("TeaserRuleName"))
        '            End Select
        '            Return sType
        '        End Function

        '        Private Function getDetailBySpread(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
        '            Dim nHomeSpread As Double = SafeDouble(poTicketBet("HomeSpread"))
        '            Dim nAwaySpread As Double = SafeDouble(poTicketBet("AwaySpread"))
        '            Dim nHomeSpreadMoney As Double = SafeDouble(poTicketBet("HomeSpreadMoney"))
        '            Dim nAwaySpreadMoney As Double = SafeDouble(poTicketBet("AwaySpreadMoney"))

        '            Dim sChoiceTeam As String = SafeString(IIf(nHomeSpreadMoney <> 0, psHomeTeam, psAwayTeam))

        '            Dim nSpread As Double = SafeDouble(IIf(nHomeSpreadMoney <> 0, nHomeSpread, nAwaySpread)) + SafeDouble(poTicketBet("AddPoint"))
        '            Dim sSpread As String = SafeString(nSpread)
        '            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
        '                sSpread = AHFormat(nSpread)
        '            End If
        '            Dim nSpreadMoney As Double = SafeRound(IIf(nHomeSpreadMoney <> 0, nHomeSpreadMoney, _
        '                                             nAwaySpreadMoney)) + SafeRound(poTicketBet("AddPointMoney"))

        '            Dim sSpreadMoney As String = SafeString(nSpreadMoney)

        '            Return String.Format("{0}<br/> <b>{1} ({2})</b>", sChoiceTeam, sSpread, sSpreadMoney)
        '        End Function

        '        Private Function getDetailByTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
        '            Dim nTotalPoints As Double = SafeDouble(poTicketBet("TotalPoints")) + SafeDouble(poTicketBet("AddPoint"))

        '            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
        '            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

        '            Dim sMsg As String = SafeString(IIf(nOverMoney <> 0, "Over", "Under"))
        '            Dim nMoney As Double = SafeDouble(IIf(nOverMoney <> 0, nOverMoney, nUnderMoney)) + SafeDouble(poTicketBet("AddPointMoney"))
        '            Dim sTotalPoints As String = SafeString(nTotalPoints)

        '            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
        '                sTotalPoints = AHFormat(nTotalPoints)
        '            End If

        '            Dim sMoney As String = SafeString(nMoney)
        '            Return String.Format("{0}<br/>{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, psAwayTeam, sMsg, _
        '                                 sTotalPoints, sMoney)
        '        End Function

        '        Private Function getDetailByMoneyLine(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
        '            Dim nHomeMoneyLine As Double = SafeDouble(poTicketbet("HomeMoneyLine"))
        '            Dim nAwayMoneyLine As Double = SafeDouble(poTicketbet("AwayMoneyLine"))

        '            Dim sChoiceTeam As String = SafeString(IIf(nHomeMoneyLine <> 0, psHomeTeam, psAwayTeam))
        '            Dim nMoneyLine As Double = IIf(nHomeMoneyLine <> 0, nHomeMoneyLine, nAwayMoneyLine)

        '            Dim sMoneyLine As String = SafeString(nMoneyLine)
        '            Return String.Format("{0}<br/> <b>{1}</b>", sChoiceTeam, sMoneyLine)
        '        End Function

        '        Private Function getDetailByDraw(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
        '            Dim nDrawLine As Double = SafeDouble(poTicketbet("DrawMoneyLine")) + SafeDouble(poTicketbet("AddPointMoney"))

        '            Return String.Format("{0} - {1}<br/> <b>Draw {2}</b>", psHomeTeam, psAwayTeam, FormatNumber(nDrawLine, GetRoundMidPoint, TriState.UseDefault, TriState.False))
        '        End Function

        '        Private Function FormatPoint(ByVal pnPoint As Double, ByVal psGameType As String) As String
        '            If IsSoccer(psGameType) Then
        '                Return AHFormat(pnPoint)
        '            End If
        '            Return FormatNumber(pnPoint, GetRoundMidPoint, TriState.UseDefault, TriState.False)
        '        End Function

        '        Protected Sub grdTickets_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles grdTickets.PageIndexChanged
        '            grdTickets.CurrentPageIndex = e.NewPageIndex
        '            SearchTickets()
        '        End Sub

        '        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
        '            txtTicket.Text = ""
        '            ddlAgents.SelectedValue = All
        '            ddlPlayer.SelectedValue = All
        '            'ddlPlayer.Items.Add(New ListItem("choose one Agent", ""))
        '        End Sub

        '        Protected Sub ddlAgents_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAgents.SelectedIndexChanged
        '            If ddlAgents.SelectedValue = All Then
        '                ddlPlayer.Items.Clear()
        '                ddlPlayer.Items.Add(New ListItem(All, All))
        '                ddlPlayer.SelectedValue = All
        '                ' ddlPlayer.Items.Add(New ListItem("choose one Agent", ""))
        '            Else
        '                BindPlayers()
        '            End If
        '        End Sub

        '        Private Function loadAgents() As DataTable
        '            Dim odtAgents As New DataTable
        '            odtAgents.Columns.Add("AgentID", GetType(Guid))
        '            odtAgents.Columns.Add("AgentName", GetType(String))
        '            odtAgents.Columns.Add("Login", GetType(String))
        '            odtAgents.Columns.Add("IsLocked", GetType(String))
        '            odtAgents.Columns.Add("IsBettingLocked", GetType(String))
        '            odtAgents.Columns.Add("LastLoginDate", GetType(DateTime))

        '            Dim oAgentManager As New SBCBL.Managers.CAgentManager()
        '            Dim dtParents As DataTable = oAgentManager.GetAllAgentsBySuperAdminID(UserSession.UserID, Nothing)

        '            Dim odrParents As DataRow() = dtParents.Select("ParentID IS NULL", "AgentName")

        '            If odrParents.Length = 0 Then
        '                odrParents = dtParents.Select("ParentID IS NOT NULL", "AgentName")
        '            End If

        '            For Each drParent As DataRow In odrParents
        '                Dim odrAgent As DataRow = odtAgents.NewRow
        '                odtAgents.Rows.Add(odrAgent)

        '                odrAgent("AgentID") = drParent("AgentID")
        '                odrAgent("AgentName") = loopString("----", SafeInteger(drParent("AgentLevel")) - 1) & SafeString(drParent("AgentName"))
        '                odrAgent("Login") = drParent("Login")
        '                odrAgent("IsLocked") = drParent("IsLocked")
        '                odrAgent("IsBettingLocked") = drParent("IsBettingLocked")
        '                odrAgent("LastLoginDate") = drParent("LastLoginDate")
        '                loadSubAgent(SafeString(drParent("AgentID")), dtParents, odtAgents)
        '            Next

        '            Return odtAgents
        '        End Function

        '        Public Sub loadSubAgent(ByVal psParentAgentID As String, ByVal podtParents As DataTable, ByRef odtAgents As DataTable)
        '            Dim odrSubAgents As DataRow() = podtParents.Select("ParentID=" & SQLString(psParentAgentID), "AgentName")

        '            For Each drChild As DataRow In odrSubAgents
        '                Dim odrAgent As DataRow = odtAgents.NewRow
        '                odtAgents.Rows.Add(odrAgent)

        '                odrAgent("AgentID") = drChild("AgentID")
        '                odrAgent("AgentName") = loopString("----", SafeInteger(drChild("AgentLevel")) - 1) & SafeString(drChild("AgentName"))
        '                odrAgent("Login") = drChild("Login")
        '                odrAgent("IsLocked") = drChild("IsLocked")
        '                odrAgent("IsBettingLocked") = drChild("IsBettingLocked")
        '                odrAgent("LastLoginDate") = drChild("LastLoginDate")

        '                loadSubAgent(SafeString(drChild("AgentID")), podtParents, odtAgents)
        '            Next
        '        End Sub

        '        Private Function loopString(ByVal psSource As String, ByVal pnLoop As Integer) As String
        '            Dim sLoop As String = ""

        '            For nIndex As Integer = 0 To pnLoop - 1
        '                sLoop &= psSource
        '            Next
        '            Return sLoop & " "
        '        End Function

        '        Private Function getListSubAgentID(ByVal psAgentID As String) As List(Of String)
        '            Dim olstSubAgent As List(Of String) = New List(Of String)
        '            Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsByAgent(psAgentID, Nothing)
        '            For Each odrSubAgent As DataRow In odtSubAgent.Rows
        '                olstSubAgent.Add(SafeString(odrSubAgent("AgentID")))
        '            Next
        '            olstSubAgent.Add(psAgentID)
        '            Return olstSubAgent
        '        End Function

        '        Private Function getListAgentIDBySuperID(ByVal psSuperID As String) As List(Of String)
        '            Dim olstAgent As List(Of String) = New List(Of String)
        '            Dim odtSubAgent As DataTable = (New CAgentManager).GetAllAgentsBySuperAdminID(psSuperID, Nothing)
        '            For Each odrAgent As DataRow In odtSubAgent.Rows
        '                olstAgent.Add(SafeString(odrAgent("AgentID")))
        '            Next
        '            olstAgent.Add(psSuperID)
        '            Return olstAgent
        '        End Function

        '        Private Sub deleteTickets()
        '            Dim bDeleteSuccess As Boolean = False
        '            For Each ticketItem As DataGridItem In grdTickets.Items
        '                If ticketItem.FindControl("chkDelete") IsNot Nothing AndAlso CType(ticketItem.FindControl("chkDelete"), CheckBox).Checked Then

        '                    Dim oTicketManager As New SBCBL.Managers.CTicketManager()
        '                    Dim HFPlayerID As HiddenField = CType(ticketItem.FindControl("hFPlayerID"), HiddenField)
        '                    Dim oDeletedTicket As New SBCBL.CDeletedTicket()
        '                    oDeletedTicket.TypeOfBet = CType(ticketItem.FindControl("lblUserPhone"), Label).Text
        '                    oDeletedTicket.Player = CType(ticketItem.FindControl("lblPlayer"), Label).Text
        '                    oDeletedTicket.TicketType = CType(ticketItem.FindControl("HFTicketType"), HiddenField).Value
        '                    oDeletedTicket.TicketNumber = SafeInteger(CType(ticketItem.FindControl("HFTicketNumber"), HiddenField).Value)
        '                    oDeletedTicket.SubTicketNumber = SafeInteger(CType(ticketItem.FindControl("HFSubTicketNumber"), HiddenField).Value)
        '                    oDeletedTicket.GameType = CType(ticketItem.FindControl("lblSport"), Label).Text
        '                    oDeletedTicket.Description = CType(ticketItem.FindControl("lblDescription"), Label).Text
        '                    oDeletedTicket.RiskAmount = SafeInteger(CType(ticketItem.FindControl("HFRiskAmount"), HiddenField).Value)
        '                    oDeletedTicket.WinAmount = SafeInteger(CType(ticketItem.FindControl("HFWinAmount"), HiddenField).Value)
        '                    oDeletedTicket.NetAmount = SafeInteger(CType(ticketItem.FindControl("HFNetAmount"), HiddenField).Value)
        '                    oDeletedTicket.GameDate = SafeDate(CType(ticketItem.FindControl("lblPlaced"), Label).Text)
        '                    oDeletedTicket.TransactionDate = SafeDate(CType(ticketItem.FindControl("lblTicketDate"), Label).Text)
        '                    bDeleteSuccess = oTicketManager.DeleteTicket(CType(ticketItem.FindControl("chkDelete"), CheckBox).ValidationGroup, HFPlayerID.Value, oDeletedTicket)
        '                End If
        '            Next
        '            If bDeleteSuccess Then
        '                ClientAlert("Delete Successfull", True)
        '            Else
        '                ClientAlert("Delete Fail", True)
        '            End If
        '            SearchTickets()
        '        End Sub

        '        Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        '            deleteTickets()
        '        End Sub
    End Class
End Namespace