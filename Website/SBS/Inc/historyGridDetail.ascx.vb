Imports System.Data
Imports System.Globalization
Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSWebsite

    Partial Class historyGridDetail
        Inherits SBCBL.UI.CSBCUserControl

        Private HistoryTicketsCount As Int32 = 0
        Private TotalBets As Double = 0
        Private TotalRisk As Double = 0
        Private TotalWin As Double = 0
        Private TotalBalance As Double = 0
        Private TotalResult As Double = 0

        Private Phone_Type As String = "Phone"
        Private __CurrentTicketID As String = "__CURRENT"
        Private RoundMidPoint As Double = GetRoundMidPoint()

        Public Property ShowPlayerName() As Boolean
            Get
                Return SafeBoolean(ViewState("_SHOW_PLAYER_NAME"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_SHOW_PLAYER_NAME") = value
            End Set
        End Property

        Public Property ShowCAgentName() As Boolean
            Get
                Return SafeBoolean(ViewState("_SHOW_PLAYER_CAGENT_NAME"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("_SHOW_PLAYER_CAGENT_NAME") = value
            End Set
        End Property

        Public Property AgentName() As String
            Get
                Return SafeString(ViewState("AgentName"))
            End Get
            Set(ByVal value As String)
                ViewState("AgentName") = value
            End Set
        End Property

        Public ReadOnly Property ResultGrid() As DataGrid
            Get
                Return grdHistory
            End Get
        End Property

        Public Property AssignRecordingLink() As String
            Get
                Return SafeString(ViewState("__ASSIGN_RECORDING_LINK"))
            End Get
            Set(ByVal value As String)
                ViewState("__ASSIGN_RECORDING_LINK") = value
            End Set
        End Property

        Public Sub LoadHistoryTickets(ByVal poHistoryTickets As DataTable)

                Dim odrTotal As DataRow = poHistoryTickets.NewRow
                odrTotal("TicketID") = newGUID()
                poHistoryTickets.Rows.Add(odrTotal)

                Me.HistoryTicketsCount = poHistoryTickets.Rows.Count

            grdHistory.DataSource = poHistoryTickets
            grdHistory.DataBind()

            ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
            If Me.HistoryTicketsCount > 0 Then
                formatDisplayGridWithRowSpan(poHistoryTickets)
            End If
        End Sub



        Private Sub formatDisplayGridWithRowSpan(ByVal poHistoryTickets As DataTable)
            Dim oCountByTicketIDs As List(Of CItemCount) = (From oRow In poHistoryTickets.AsEnumerable
                                                            Group oRow By sTicketID = SafeString(oRow.Field(Of Guid)("TicketID")) Into Count()
                                                            Where Count >= 2
                                                            Select New CItemCount(sTicketID, Count)).ToList()

            Dim nIndex As Integer = 0
            Dim oBackColor As Drawing.Color = Drawing.Color.White
            Dim alternateCount As Integer = 0
            Dim betTypeForAlternate As String = ""
            Dim contextForAlternate As String = ""
            Dim ticketIdforAlternate As String = ""


            While nIndex < grdHistory.Items.Count - 1
                Dim oItem As DataGridItem = grdHistory.Items(nIndex)
                oItem.BackColor = oBackColor

                Dim sBetType As String = SafeString(CType(oItem.FindControl("hfTicketType"), HiddenField).Value)
                Dim sContext As String = SafeString(CType(oItem.FindControl("hfContext"), HiddenField).Value)
                Dim sTicketID As String = SafeString(CType(oItem.FindControl("hfTicketID"), HiddenField).Value)
                Dim oCountByTicketID As CItemCount = oCountByTicketIDs.Find(Function(x) x.ItemID = sTicketID)

                ' Row alternate
                If Not betTypeForAlternate.Equals(sBetType) Then
                    alternateCount = 1
                ElseIf Not ticketIdforAlternate.Equals(sTicketID) And (betTypeForAlternate.Equals(sBetType) Or (sBetType.Contains("If Win") And betTypeForAlternate.Contains("Reverse")) Or (sBetType.Contains("Reverse") And betTypeForAlternate.Contains("If Win")))
                    If sBetType.Contains("Straight") Then
                        If Not (contextForAlternate.Equals(sContext) Or (sContext.Contains(contextForAlternate))) Then
                            alternateCount = 1
                        Else
                            alternateCount += 1
                        End If
                    Else
                        alternateCount += 1
                    End If

                End If


                betTypeForAlternate = SafeString(IIf(sBetType.Contains("Reverse"), "If Win", sBetType))
                ticketIdforAlternate = sTicketID
                contextForAlternate = SafeString(IIf(sContext.Contains("q"), "q", sContext))


                ' Row color
                Select Case True
                    Case sBetType.Contains("Straight")
                        Select Case LCase(sContext)
                            Case "current"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#efefef")
                                End If
                            Case "1h"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#cde4fd")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#b5d8ff")
                                End If

                            Case "2h"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#b2f8b7")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#6cf475")
                                End If

                            Case "1q"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#faa833")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#fab450")
                                End If

                            Case "2q"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#faa833")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#fab450")
                                End If

                            Case "3q"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#faa833")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#fab450")
                                End If

                            Case "4q"
                                If (alternateCount Mod 2) = 0 Then
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#faa833")
                                Else
                                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#fab450")
                                End If
                            Case Else
                                oBackColor = System.Drawing.ColorTranslator.FromHtml("#efefef")
                        End Select
                    Case sBetType.Contains("Parlay")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#ede56f")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f1e64e")
                        End If
                    Case sBetType.Contains("Teaser")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f08080")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f59596")
                        End If
                    Case sBetType.Contains("If Win") Or sBetType.Contains("Reverse")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#efefef")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")
                        End If
                    Case Else
                        oBackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")
                End Select

                oItem.BackColor = oBackColor


                If oCountByTicketID IsNot Nothing Then
                    Dim nRowSpan As Integer = oCountByTicketID.ItemCount

                    Dim lblTicket As Label = CType(oItem.FindControl("lblWagerType"), Label)
                    lblTicket.Text = nRowSpan.ToString() & " Teams " & lblTicket.Text

                    oItem.Cells(0).RowSpan = nRowSpan ' **
                    oItem.Cells(1).RowSpan = nRowSpan 'Ticket Date
                    oItem.Cells(2).RowSpan = nRowSpan 'Ticket Number
                    oItem.Cells(4).RowSpan = nRowSpan 'Type
                    oItem.Cells(5).RowSpan = nRowSpan 'Risk
                    oItem.Cells(6).RowSpan = nRowSpan 'Win
                    oItem.Cells(7).RowSpan = nRowSpan 'Result

                    For nRowSpanStart As Integer = 1 To nRowSpan - 1
                        Dim oItemRowSpan As DataGridItem = grdHistory.Items(nRowSpanStart + nIndex)
                        oItemRowSpan.BackColor = oBackColor

                        oItemRowSpan.Cells(0).Visible = False
                        oItemRowSpan.Cells(1).Visible = False
                        oItemRowSpan.Cells(2).Visible = False
                        oItemRowSpan.Cells(4).Visible = False
                        oItemRowSpan.Cells(5).Visible = False
                        oItemRowSpan.Cells(6).Visible = False
                        oItemRowSpan.Cells(7).Visible = False

                        Dim ltrIfBet As Literal = CType(oItemRowSpan.FindControl("ltrIfBet"), Literal)
                        If ltrIfBet IsNot Nothing Then
                            ltrIfBet.Visible = False
                        End If
                    Next

                    nIndex += nRowSpan
                Else
                    nIndex += 1
                End If

            End While

            ''total row
            Dim oTotalItem As DataGridItem = grdHistory.Items(grdHistory.Items.Count - 1)

            oTotalItem.Cells(0).Visible = False
            oTotalItem.Cells(1).ColumnSpan = 7
            oTotalItem.Cells(1).HorizontalAlign = HorizontalAlign.Right
            oTotalItem.Cells(3).Visible = False
            oTotalItem.Cells(4).Visible = False
            oTotalItem.Cells(5).Visible = False
            oTotalItem.Cells(6).Visible = False
            oTotalItem.Cells(7).Visible = False

        End Sub

        Protected Sub grdHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdHistory.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oTicketBet As DataRowView = CType(e.Item.DataItem, DataRowView)

                If e.Item.ItemIndex = Me.HistoryTicketsCount - 1 Then
                    e.Item.Cells(1).Text = "Total:"
                    e.Item.Cells(2).Text = FormatNumber(Me.TotalResult, 2)
                    e.Item.Cells(2).CssClass = "bold"
                    If Me.TotalResult < 0
                        e.Item.Cells(2).ForeColor = System.Drawing.ColorTranslator.FromHtml("#b30000")
                    End If
                Else
                    Dim sTicketType = GetTicketType(oTicketBet)

                    CType(e.Item.FindControl("lblTicketDate"), Label).Text = SafeDate(oTicketBet("TransactionDate"))
                    CType(e.Item.FindControl("lblTicketNumber"), Label).Text = SafeString(oTicketBet("TicketNumber"))

                    ' Game
                    Dim gameType = SafeString(oTicketBet("GameType"))
                    Dim riskAmount = SafeDouble(oTicketBet("RiskAmount"))
                    Dim winAmount = SafeDouble(oTicketBet("WinAmount"))

                    If sTicketType.Contains("If Win") Or sTicketType.Contains("Reverse") Then
                        CType(e.Item.FindControl("ltrIfBet"), Literal).Text = sTicketType
                        CType(e.Item.FindControl("ltrRiskWin"), Literal).Text = String.Format("<span>Risking: {0}</span> <span>Win: {1}</span>", FormatNumber(riskAmount, 2), FormatNumber(winAmount, 2))
                    End If

                    CType(e.Item.FindControl("ltrSportGameTeam"), Literal).Text = String.Format("{0} - {1}", GetSportType(gameType), gameType)

                    CType(e.Item.FindControl("lblWagerType"), Label).Text = sTicketType
                    CType(e.Item.FindControl("hfBetType"), HiddenField).Value = sTicketType
                    CType(e.Item.FindControl("hfContext"), HiddenField).Value = SafeString(oTicketBet("Context"))



                    ''total bet
                    Dim sTicketID As String = CType(e.Item.FindControl("hfTicketID"), HiddenField).Value

                    'risk/win
                    CType(e.Item.FindControl("lblRisk"), Label).Text = FormatNumber(riskAmount, 2)
                    CType(e.Item.FindControl("lblWin"), Label).Text = FormatNumber(winAmount, 2)

                    ' Result
                    Dim nAmount As Double = SafeRound(oTicketBet("NetAmount")) - riskAmount
                    Dim lblResult As Label = CType(e.Item.FindControl("lblResult"), Label)
                    lblResult.Text = FormatNumber(nAmount, 2)
                    If nAmount < 0 Then
                        lblResult.ForeColor = System.Drawing.ColorTranslator.FromHtml("#b30000")
                    End If


                    If Me.__CurrentTicketID <> sTicketID Then
                        Me.__CurrentTicketID = sTicketID
                        Me.TotalBets += 1

                        Me.TotalRisk += riskAmount
                        Me.TotalWin += winAmount
                        Me.TotalResult += nAmount
                    End If

                    ''description
                    Dim sHomeTeam As String = SafeString(oTicketBet("HomeTeam")) '& " - H"
                    Dim sAwayTeam As String = SafeString(oTicketBet("AwayTeam")) '& " - A"

                    Dim sDescription As String = ""
                    If UCase(SafeString(oTicketBet("IsForProp"))).Equals("Y") Then
                        sDescription = getDetailForPop(sHomeTeam, sAwayTeam, oTicketBet)
                    Else
                        If GetGameType.ContainsKey(SafeString(oTicketBet("gametype"))) AndAlso Not IsBasketball(SafeString(oTicketBet("gametype"))) AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Preseason") AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Football") Then
                            sDescription = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("Description"))), "", SafeString(oTicketBet("Description")) & "<br/>")
                        End If
                        Select Case UCase(SafeString(oTicketBet("BetType")))
                            Case "SPREAD"
                                sDescription = getDetailBySpread(sHomeTeam, sAwayTeam, oTicketBet)
                            Case "TOTALPOINTS"
                                If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                                    sHomeTeam = SafeString(oTicketBet("AwayTeam")) & "/" & sHomeTeam
                                    sDescription = getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                                Else
                                    sHomeTeam = sAwayTeam & "/" & sHomeTeam
                                    sDescription = getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                                End If
                            Case "TEAMTOTALPOINTS"
                                sDescription = getDetailByTeamTotalPoints(IIf(SafeString(oTicketBet("TeamTotalName")).Equals("away"), sAwayTeam, sHomeTeam), "", oTicketBet)
                            Case "MONEYLINE"
                                sDescription = getDetailByMoneyLine(sHomeTeam, sAwayTeam, oTicketBet)
                            Case "DRAW"
                                sDescription = getDetailByDraw(sHomeTeam, sAwayTeam, oTicketBet)
                        End Select

                    End If

                    CType(e.Item.FindControl("ltrGameTeam"), Literal).Text = sDescription

                End If
            End If
        End Sub

        Private Function GetTicketType(ByVal poTicketBet As DataRowView) As String
            Dim sType = SafeString(poTicketBet("TicketType"))
            Dim oDT As DataTable = CType(grdHistory.DataSource, DataTable)

            Select Case sType
                Case "Parlay"
                    Dim nTicketNumber As Integer = SafeInteger(poTicketBet("TicketNumber"))
                    Dim nCountTicket As Integer = 0
                    ' count how many ticket has same ticket number with this ticket
                    Dim bRobin As Boolean = False
                    For Each oRow As DataRow In oDT.Rows
                        Dim nSubTicketNumber As Integer = SafeInteger(oRow("SubTicketNumber"))
                        Dim nRowTicketNumber As Integer = SafeInteger(oRow("TicketNumber"))
                        If nRowTicketNumber <> nTicketNumber Then
                            Continue For
                        End If

                        If nSubTicketNumber > 1 Then
                            bRobin = True
                            Continue For
                        End If
                    Next

                    If bRobin Then
                        Return "Round Robin Parlay"
                    End If

                Case "Teaser"
                    Return SafeString(poTicketBet("TeaserRuleName"))
            End Select
            Return sType
        End Function

        Private Function getDetailBySpread(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim gameDate As Date = SafeDate(poTicketBet("GameDate"))
            Dim ticketBetStatus As String = SafeString(poTicketBet("TicketBetStatus"))
            Dim sContext = SafeString(poTicketBet("Context"))
            Dim sGameType As String = SafeString(poTicketBet("GameType"))
            Dim nHomeSpread As Double = SafeDouble(poTicketBet("HomeSpread"))
            Dim nAwaySpread As Double = SafeDouble(poTicketBet("AwaySpread"))
            Dim nHomeSpreadMoney As Double = SafeDouble(poTicketBet("HomeSpreadMoney"))
            Dim nAwaySpreadMoney As Double = SafeDouble(poTicketBet("AwaySpreadMoney"))
            Dim nSpread As Double = SafeDouble(IIf(nHomeSpreadMoney <> 0, nHomeSpread, nAwaySpread)) + SafeDouble(poTicketBet("AddPoint"))
            Dim sSpread As String = SafeString(nSpread)
            If IsSoccer(sGameType) Then
                sSpread = safeVegass(AHFormat(nSpread))
            End If
            sSpread = SafeString(IIf(nSpread = 0, "PK", safeVegass(sSpread)))

            Dim sChoiceTeam As String = SafeString(IIf(nHomeSpreadMoney <> 0, psHomeTeam, psAwayTeam))

            Dim nSpreadMoney As Double = SafeRound(IIf(nHomeSpreadMoney <> 0, nHomeSpreadMoney, nAwaySpreadMoney)) + SafeRound(poTicketBet("AddPointMoney"))

            Dim sSpreadMoney As String = SafeString(IIf(nSpreadMoney > 0, IIf(nSpreadMoney = 100, "Even", "+" & nSpreadMoney), nSpreadMoney))
            Dim rotationNumber As String = SafeString(IIf(nHomeSpreadMoney <> 0, SafeDouble(poTicketBet("HomeRotationNumber")), SafeDouble(poTicketBet("AwayRotationNumber"))))

            Dim regulationOnly = IIf(IsSoccer(SafeString(poTicketBet("GameType"))), "<b>Regualation Only</b>", "")
            Dim gameBet = String.Format("<div class='baseline'><b>{0} {1}</b> for the {3} {2}</div>", sSpread, sSpreadMoney, regulationOnly, ContextFormat(sContext))

            Dim sRotationNumber = SafeString("<b class='gm-number'>[" & rotationNumber & "]</b>&nbsp;")

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'>" & sRotationNumber & "<b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet


            Return htmlString

        End Function


        Private Function getDetailByTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketBet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketBet("GameDate"))
            Dim sContext = SafeString(poTicketBet("Context"))
            Dim ticketBetStatus As String = SafeString(poTicketBet("TicketBetStatus"))
            Dim nTotalPoints As Double = SafeDouble(poTicketBet("TotalPoints")) + SafeDouble(poTicketBet("AddPoint"))
            Dim sTotalPoint As String = SafeString(nTotalPoints)
            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sTotalPoint = AHFormat(nTotalPoints).Replace("+"c, "")
            End If
            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

            Dim sMsg As String = SafeString(IIf(nOverMoney <> 0, "Over", "Under"))
            Dim nMoney As Double = SafeRound(IIf(nOverMoney <> 0, nOverMoney, nUnderMoney)) + SafeRound(poTicketBet("AddPointMoney"))

            Dim sMoney As String = SafeString(IIf(nMoney > 0, IIf(nMoney = 100, "Even", "+" & nMoney), nMoney))

            Dim mustStarPitcher = GetMustStart(poTicketBet, sGameType)

            Dim regulationOnly = IIf(IsSoccer(SafeString(poTicketBet("GameType"))), "<b>Regualation Only</b>", "")
            Dim gameBet = String.Format("<div class='baseline'>{0} <b>{1} {2}</b> for the {5} {3} {4}</div>", sMsg, safeVegass(sTotalPoint), sMoney, regulationOnly, mustStarPitcher, ContextFormat(sContext))

            Dim sChoiceTeam = String.Format("{0} {1}", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", " - " & psAwayTeam))

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString

            'Return String.Format("{0}{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", "<br/> " & psAwayTeam), sMsg,
            '                    sTotalPoint, sMoney)
        End Function

        Private Function getDetailByTeamTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketBet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketBet("GameDate"))
            Dim sContext = SafeString(poTicketBet("Context"))
            Dim ticketBetStatus As String = SafeString(poTicketBet("TicketBetStatus"))
            Dim nTotalPoints As Double = SafeDouble(poTicketBet("TotalPoints")) + SafeDouble(poTicketBet("AddPoint"))
            Dim sTotalPoint As String = SafeString(nTotalPoints)
            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sTotalPoint = AHFormat(nTotalPoints).Replace("+"c, "")
            End If
            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

            Dim sMsg As String = SafeString(IIf(nOverMoney <> 0, "Over", "Under"))
            Dim nMoney As Double = SafeRound(IIf(nOverMoney <> 0, nOverMoney, nUnderMoney)) + SafeRound(poTicketBet("AddPointMoney"))

            Dim sMoney As String = SafeString(IIf(nMoney > 0, IIf(nMoney = 100, "Even", "+" & nMoney), nMoney))
            Dim regulationOnly = IIf(IsSoccer(SafeString(poTicketBet("GameType"))), "<b>Regualation Only</b>", "")

            Dim gameBet = String.Format("<div class='baseline'>{0} <b>{1} {2}</b> for the {4} {3}</div>", sMsg, safeVegass(sTotalPoint), sMoney, regulationOnly, ContextFormat(sContext))

            Dim sChoiceTeam = String.Format("{0} {1}", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", " - " & psAwayTeam))

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString
        End Function

        Private Function getDetailByMoneyLine(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim sContext = SafeString(poTicketbet("Context"))
            Dim ticketBetStatus As String = SafeString(poTicketbet("TicketBetStatus"))
            Dim nHomeMoneyLine As Double = SafeDouble(poTicketbet("HomeMoneyLine"))
            Dim nAwayMoneyLine As Double = SafeDouble(poTicketbet("AwayMoneyLine"))
            Dim sChoiceTeam As String = SafeString(IIf(nHomeMoneyLine <> 0, psHomeTeam, psAwayTeam))
            Dim nMoneyLine As Double = SafeDouble(IIf(nHomeMoneyLine <> 0, nHomeMoneyLine, nAwayMoneyLine))
            Dim sMoneyLine As String = SafeString(IIf(nMoneyLine > 0, "+" & nMoneyLine, nMoneyLine))
            Dim rotationNumber As String = SafeString(IIf(nHomeMoneyLine <> 0, SafeDouble(poTicketbet("HomeRotationNumber")), SafeDouble(poTicketbet("AwayRotationNumber"))))

            Dim mustStarPitcher = GetMustStart(poTicketbet, sGameType)

            Dim regulationOnly = IIf(IsSoccer(sGameType), "<b>Regualation Only</b>", "")
            Dim gameBet = String.Format("<div class='baseline'>Money Line <b>{0}</b> for the {3} {1} {2}</div>", sMoneyLine, regulationOnly, mustStarPitcher, ContextFormat(sContext))

            Dim sRotationNumber = SafeString("<b class='gm-number'>[" & rotationNumber & "]</b>&nbsp;")

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'>" & sRotationNumber & "<b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet


            Return htmlString

        End Function

        Private Function getDetailByDraw(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim nDrawLine As Double = SafeDouble(poTicketbet("DrawMoneyLine")) + SafeDouble(poTicketbet("AddPointMoney"))
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim ticketBetStatus As String = SafeString(poTicketbet("TicketBetStatus"))
            Dim sContext = SafeString(poTicketbet("Context"))

            Dim sDrawLine As String = SafeString(IIf(nDrawLine > 0, "+" & nDrawLine, nDrawLine))

            Dim mustStarPitcher = GetMustStart(poTicketbet, sGameType)

            Dim regulationOnly = IIf(IsSoccer(sGameType), "<b>Regualation Only</b>", "")
            Dim gameBet = String.Format("<div class='baseline'>Money Line <b>{0}</b> for the {3} {1} {2}</div>", nDrawLine, regulationOnly, mustStarPitcher, ContextFormat(sContext))

            Dim sRotationNumber = ""

            Dim sChoiceTeam = String.Format("Draw({0} vs {1})", psHomeTeam, psAwayTeam)

            Dim htmlString As String = "<div class='baseline'>" & sRotationNumber & "<b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString

            'Return "THis is draw" ' htmlString
        End Function

        Private Function getDetailForPop(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim ticketBetStatus As String = SafeString(poTicketbet("TicketBetStatus"))
            Dim sPopDescription = SafeString(poTicketbet("PropDescription"))
            Dim sPropParticipantName = SafeString(poTicketbet("PropParticipantName"))
            Dim sPropMoneyLine = SafeString(poTicketbet("PropMoneyLine"))
            Dim sContext = SafeString(poTicketbet("Context"))

            Dim gameBet = String.Format("<div class='baseline'><b>{0}</b> for the {2} {1}</div>", sPropMoneyLine, sPopDescription, ContextFormat(sContext))

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sPropParticipantName & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketBetStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString

            'Return "THis is draw" ' htmlString
        End Function

        Private Function FormatPoint(ByVal pnPoint As Double, ByVal psGameType As String) As String
            If IsSoccer(psGameType) Then
                Return AHFormat(pnPoint)
            End If
            Return FormatNumber(pnPoint, GetRoundMidPoint, TriState.UseDefault, TriState.False)
        End Function

        Private Function DenoteOfSportType(ByVal gameType As String) As String
            Select Case True
                Case IsFootball(gameType) Or IsBasketball(gameType)
                    Return "Points"
                Case IsBaseball(gameType)
                    Return "Runs"
                Case IsHockey(gameType) Or IsSoccer(gameType)
                    Return "Goals"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetMustStart(ByVal poTicketbet As DataRowView, ByVal sGameType As String) As String
            If Not IsBaseball(sGameType) Then
                Return ""
            End If

            Dim mustStarPitcher = ""

            If SafeString(poTicketbet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(poTicketbet("AwayPitcher_TicketBets")) <> "" Then
                Dim sHomePitcher = SafeString(poTicketbet("HomePitcher_TicketBets"))
                Dim sAwayPitcher = SafeString(poTicketbet("AwayPitcher_TicketBets"))
                mustStarPitcher = String.Format("<i>{0}(must start) {1}(must start)</i>", sHomePitcher, sAwayPitcher)
            End If
            Return mustStarPitcher
        End Function

        Private Function ContextFormat(ByVal sContext As String) As String
            Select Case LCase(sContext)
                Case "current"
                    Return "Game"

                Case "1h"
                    Return "1st Half"

                Case "2h"
                    Return "2nd Half"

                Case "1q"
                    Return "1st Quarter"

                Case "2q"
                    Return "2nd Quarter"

                Case "3q"
                    Return "3rd Quarter"

                Case "4q"
                    Return "4th Quarter"

                Case Else
                    Return ""
            End Select
        End Function

        Private Function CustomUpperTitleCase(ByVal sTitle As String) As String
            Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
            Return ti.ToTitleCase(sTitle.ToLower())
        End Function
    End Class

End Namespace

