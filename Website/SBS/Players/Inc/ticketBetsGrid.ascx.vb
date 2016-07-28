Imports System.Data
Imports System.Globalization

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSPlayer

    Partial Class ticketBetsGrid
        Inherits SBCBL.UI.CSBCUserControl

        Private Phone_Type As String = "Phone"
        Private _TempScore As Hashtable
        Private Property LastScores() As Hashtable
            Get
                If ViewState("_LAST_SCORES") Is Nothing Then
                    Return New Hashtable
                End If

                Return CType(ViewState("_LAST_SCORES"), Hashtable)
            End Get
            Set(ByVal value As Hashtable)
                ViewState("_LAST_SCORES") = value
            End Set
        End Property

        Public Property ShowPlayerColumn() As Boolean
            Get
                If ViewState("__SHOWPLAYERCOLUMN") Is Nothing Then
                    ViewState("__SHOWPLAYERCOLUMN") = True
                End If
                Return CBool(ViewState("__SHOWPLAYERCOLUMN"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("__SHOWPLAYERCOLUMN") = value
                dgTicketBets.Columns(3).Visible = value
            End Set
        End Property

        Public Property IsLiveTicker() As Boolean
            Get
                Return SafeBoolean(ViewState("ISTICKER"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("ISTICKER") = value
            End Set
        End Property

        Public ReadOnly Property ResultGrid() As DataGrid
            Get
                Return dgTicketBets
            End Get
        End Property

        Public Sub LoadTicketBets(ByVal poTicketBets As DataTable, ByVal psContext As String, Optional ByVal gameType As String = Nothing, Optional ByVal pbIsTicker As Boolean = False)
            IsLiveTicker = pbIsTicker
            If IsLiveTicker Then
                pnColor.Visible = True
            End If
            Me.TicketBetsCount = 0
            Me.TotalBets = 0
            Me.TotalRisk = 0
            Me.TotalWin = 0
            Dim filter As String = ""
            If Not String.IsNullOrEmpty(psContext) Then
                filter = String.Format("AND Context like '%{0}' ", psContext)
            End If
            If Not String.IsNullOrEmpty(gameType) Then
                Dim listGameType As List(Of String) = getListGameType(gameType)
                If listGameType.Count > 0 Then
                    Dim inGameType As String = ""

                    For Each gt As String In listGameType
                        inGameType += "'" + gt + "',"
                    Next

                    filter += String.Format("AND GameType in ({0}) ", inGameType.TrimEnd(CType(",", Char)))
                End If
            End If

            If Not String.IsNullOrEmpty(filter) Then
                poTicketBets.DefaultView.RowFilter = filter.Substring(3) 'remove first AND
                poTicketBets = poTicketBets.DefaultView.ToTable()
            End If


            If poTicketBets.Rows.Count > 0 Then
                Dim odrTotal As DataRow = poTicketBets.NewRow
                odrTotal("TicketID") = newGUID()
                poTicketBets.Rows.Add(odrTotal)

                Me.TicketBetsCount = poTicketBets.Rows.Count
            End If

            '' Create new Temp Score
            _TempScore = New Hashtable()

            dgTicketBets.DataSource = poTicketBets
            dgTicketBets.DataBind()

            '' Save Last Score
            LastScores = _TempScore

            ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
            If Me.TicketBetsCount > 0 Then
                formatDisplayGridWithRowSpan(poTicketBets)
            End If
            If UserSession.UserType = SBCBL.EUserType.Player Then
                ShowPlayerColumn = False
            End If
        End Sub

        Private Sub formatDisplayGridWithRowSpan(ByVal poTicketbets As DataTable)
            Dim oCountByTicketIDs As List(Of CItemCount) = (From oRow In poTicketbets.AsEnumerable
                                                            Group oRow By sTicketID = SafeString(oRow.Field(Of Guid)("TicketID")) Into Count()
                                                            Where Count >= 2
                                                            Select New CItemCount(sTicketID, Count)).ToList()

            If oCountByTicketIDs.Count = 0 Then
                Return
            End If

            Dim nIndex As Integer = 0
            Dim alternateCount As Integer = 0
            Dim betTypeForAlternate As String = ""
            Dim ticketIdforAlternate As String = ""
            Dim oBackColor As Drawing.Color = Drawing.Color.White

            While nIndex < dgTicketBets.Items.Count - 1
                Dim oItem As DataGridItem = dgTicketBets.Items(nIndex)

                Dim betType As String = SafeString(CType(oItem.FindControl("hfBetType"), HiddenField).Value)
                Dim sTicketID As String = SafeString(CType(oItem.FindControl("hfTicketID"), HiddenField).Value)
                Dim oCountByTicketID As CItemCount = oCountByTicketIDs.Find(Function(x) x.ItemID = sTicketID)


                ' Row alternate
                If Not betTypeForAlternate.Equals(betType) Then
                    alternateCount = 1
                ElseIf Not ticketIdforAlternate.Equals(sTicketID) And (betTypeForAlternate.Equals(betType) Or (betType.Contains("If Win") And betTypeForAlternate.Contains("Reverse")) Or (betType.Contains("Reverse") And betTypeForAlternate.Contains("If Win")) ) 
                    alternateCount += 1
                End If

                betTypeForAlternate = SafeString( IIf(betType.Contains("Reverse"), "If Win", betType) )
                ticketIdforAlternate = sTicketID

                ' Row color
                Select Case True
                    Case betType.Contains("Straight")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#efefef")
                        End If
                    Case betType.Contains("Parlay")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#ede56f")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f1e64e")
                        End If
                    Case betType.Contains("Teaser")
                        If (alternateCount Mod 2) = 0 Then
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f08080")
                        Else
                            oBackColor = System.Drawing.ColorTranslator.FromHtml("#f59596")
                        End If
                    Case betType.Contains("If Win") Or betType.Contains("Reverse")
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
                    lblTicket.Text = nRowSpan.ToString() & " Teams<br/>" & lblTicket.Text

                    oItem.Cells(0).RowSpan = nRowSpan '**
                    oItem.Cells(1).RowSpan = nRowSpan 'Ticket Date
                    oItem.Cells(2).RowSpan = nRowSpan 'Ticket Number
                    oItem.Cells(4).RowSpan = nRowSpan 'Method
                    oItem.Cells(6).RowSpan = nRowSpan 'Game Type
                    oItem.Cells(7).RowSpan = nRowSpan 'Risk
                    oItem.Cells(8).RowSpan = nRowSpan 'Win

                    For nRowSpanStart As Integer = 1 To nRowSpan - 1
                        Dim oItemRowSpan As DataGridItem = dgTicketBets.Items(nRowSpanStart + nIndex)
                        oItemRowSpan.BackColor = oBackColor

                        oItemRowSpan.Cells(0).Visible = False
                        oItemRowSpan.Cells(1).Visible = False
                        oItemRowSpan.Cells(2).Visible = False
                        oItemRowSpan.Cells(4).Visible = False
                        oItemRowSpan.Cells(6).Visible = False
                        oItemRowSpan.Cells(7).Visible = False
                        oItemRowSpan.Cells(8).Visible = False

                        Dim ltrIfBet As Literal = CType(oItemRowSpan.FindControl("ltrIfBet"), Literal)
                        ltrIfBet.Visible = False
                    Next

                    nIndex += nRowSpan
                Else
                    nIndex += 1
                End If

            End While

            ''total row
            Dim oTotalItem As DataGridItem = dgTicketBets.Items(dgTicketBets.Items.Count - 1)
            oTotalItem.Cells(0).Visible = False
            oTotalItem.Cells(1).ColumnSpan = 5
            oTotalItem.Cells(2).Visible = False
            oTotalItem.Cells(3).Visible = False
            oTotalItem.Cells(4).Visible = False
            oTotalItem.Cells(5).HorizontalAlign = HorizontalAlign.Center
        End Sub

        Private TicketBetsCount As Int32 = 0

        Private TotalBets As Double = 0
        Private TotalRisk As Double = 0
        Private TotalWin As Double = 0

        Private __CurrentTicketID As String = "__CURRENT"
        Private RoundMidPoint As Double = GetRoundMidPoint()

        Protected Sub dgTicketBets_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgTicketBets.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oTicketBet As DataRowView = CType(e.Item.DataItem, DataRowView)

                If e.Item.ItemIndex = Me.TicketBetsCount - 1 Then
                    e.Item.CssClass = "row-total"
                    e.Item.Cells(5).Text = "Total"
                    e.Item.Cells(5).Width = New Unit("10%")
                    e.Item.Cells(6).Text = FormatNumber(Me.TotalRisk + SafeDouble(oTicketBet("RiskAmount")), 2)
                Else
                    Dim sTicketType = GetTicketType(oTicketBet)

                    CType(e.Item.FindControl("lblTicketDate"), Label).Text = SafeDate(oTicketBet("TransactionDate"))
                    CType(e.Item.FindControl("lblTicketNumber"), Label).Text = SafeString(oTicketBet("TicketNumber"))
                    CType(e.Item.FindControl("lblMethod"), Label).Text = SafeString(oTicketBet("TypeOfBet")).FirstOrDefault()

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

                    CType(e.Item.FindControl("lblRisk"), Label).Text = FormatNumber(SafeDouble(oTicketBet("RiskAmount")), 2)
                    CType(e.Item.FindControl("lblWin"), Label).Text = FormatNumber(SafeDouble(oTicketBet("WinAmount")), 2)

                    If UserSession.UserType = SBCBL.EUserType.SuperAdmin Or UserSession.UserType = SBCBL.EUserType.Agent Then
                        CType(e.Item.FindControl("lblPlayer"), Label).Text = SafeString(oTicketBet("PlayerName"))
                    Else
                        'CType(e.Item.FindControl("lblPlayer"), Label).Visible = False
                        ShowPlayerColumn = False

                    End If


                    If (UserSession.UserType = SBCBL.EUserType.CallCenterAgent OrElse UserSession.UserType = SBCBL.EUserType.Agent OrElse UserSession.UserType = SBCBL.EUserType.SuperAdmin) AndAlso SafeString(oTicketBet("TypeOfBet")).Equals(Phone_Type, StringComparison.OrdinalIgnoreCase) Then
                        'Dim sAgentName As String = (New CCallCenterAgentManager).GetByID(oTicketBet("OrderBy").ToString()).Rows(0)("Name").ToString()
                        Dim sAgentName As String = ""
                        If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
                            sAgentName = SafeString(oTicketBet("Login"))
                        Else
                            sAgentName = SafeString(oTicketBet("CAgentLoginName"))
                        End If
                        CType(e.Item.FindControl("lblCAgentName"), Label).Text = SafeString(sAgentName)
                        CType(e.Item.FindControl("pnlPhoneDetail"), Panel).Visible = True
                        Dim hfFileName As HiddenField = CType(e.Item.FindControl("hfFileName"), HiddenField)
                        If Not String.IsNullOrEmpty(hfFileName.Value) Then
                            Dim lbtRecord As LinkButton = CType(e.Item.FindControl("lbtRecord"), LinkButton)
                            lbtRecord.Visible = True
                            lbtRecord.OnClientClick = String.Format("openDialog('/Utils/ListenMedia.aspx?fname={0}',315,400)", SafeString(hfFileName.Value))
                        End If
                    End If

                    'Dim sScores As String = "<b>{0}</b> - <b>{1}</b>"
                    'Select Case LCase(SafeString(oTicketBet("Context")))
                    '    Case "current"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwayScore")), SafeString(oTicketBet("HomeScore")))

                    '    Case "1h"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstHalfScore")), SafeString(oTicketBet("HomeFirstHalfScore")))

                    '    Case "2h"
                    '        sScores = String.Format(sScores, SafeString(SafeInteger(oTicketBet("AwayScore")) - SafeInteger(oTicketBet("AwayFirstHalfScore"))), SafeString(SafeInteger(oTicketBet("HomeScore")) - SafeInteger(oTicketBet("HomeFirstHalfScore"))))

                    '    Case "1q"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstQScore")), SafeString(oTicketBet("HomeFirstQScore")))

                    '    Case "2q"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwaySecondQScore")), SafeString(oTicketBet("HomeSecondQScore")))

                    '    Case "3q"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwayThirdQScore")), SafeString(oTicketBet("HomeThirdQScore")))

                    '    Case "4q"
                    '        sScores = String.Format(sScores, SafeString(oTicketBet("AwayFourQScore")), SafeString(oTicketBet("HomeFourQScore")))

                    '    Case Else
                    '        sScores = ""
                    'End Select

                    'CType(e.Item.FindControl("lblScore"), Label).Text = sScores

                    ''total bet
                    Dim sTicketID As String = CType(e.Item.FindControl("hfTicketID"), HiddenField).Value

                    ''risk/win
                    Dim nRisk As Double = SafeRound(oTicketBet("RiskAmount"))
                    Dim nWin As Double = SafeRound(oTicketBet("WinAmount"))
                    'CType(e.Item.FindControl("lblRiskWin"), Label).Text = String.Format("{0} / {1}", FormatNumber(nRisk, Me.RoundMidPoint), FormatNumber(nWin, Me.RoundMidPoint))
                    'If IsLiveTicker Then
                    '    If 300 < nRisk AndAlso nRisk < 499 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC66")
                    '    ElseIf 500 < nRisk AndAlso nRisk < 999 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9900")
                    '    ElseIf 1000 < nRisk AndAlso nRisk < 1999 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF6600")
                    '    ElseIf 2000 < nRisk AndAlso nRisk < 2999 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF3300")
                    '    ElseIf 3000 < nRisk AndAlso nRisk < 3999 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#00DD00")
                    '    ElseIf 4000 < nRisk AndAlso nRisk < 4999 Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#008800")
                    '    ElseIf 5000 < nRisk Then
                    '        CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#0000FF")

                    '    End If
                    'End If

                    If Me.__CurrentTicketID <> sTicketID Then
                        Me.__CurrentTicketID = sTicketID
                        Me.TotalBets += 1

                        Me.TotalRisk += nRisk
                        Me.TotalWin += nWin
                    End If

                    ''description
                    Dim sHomeTeam As String = SafeString(oTicketBet("HomeTeam")) '& " - H"
                    Dim sAwayTeam As String = SafeString(oTicketBet("AwayTeam")) '& " - A"
                    'If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                    '    sHomeTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    'End If
                    'If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                    '    sAwayTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    'End If
                    Dim sDescription As String = ""
                    If UCase(SafeString(oTicketBet("IsForProp"))).Equals("Y") Then
                        sDescription = getDetailForPop(sHomeTeam, sAwayTeam, oTicketBet)
                    Else
                        'Dim sDescription As String = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("Description"))), "", SafeString(oTicketBet("Description")) & "<br/>")

                        'ClientAlert(SafeString(oTicketBet("gametype")), True)
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

                    'Dim sTicketBetID As String = SafeString(oTicketBet("TicketBetID"))
                    'Dim sScore As String = SafeString(oTicketBet("AwayScore")) & SafeString(oTicketBet("HomeScore"))

                    ''' Check New Changes
                    'If LastScores.ContainsKey(sTicketBetID) Then
                    '    If SafeString(LastScores.Item(sTicketBetID)) <> sScore Then
                    '        e.Item.Cells(7).Attributes.Add("class", "HighLightBlock")
                    '    End If
                    '    _TempScore(sTicketBetID) = SafeString(LastScores.Item(sTicketBetID))
                    'Else
                    '    _TempScore(sTicketBetID) = sScore
                    'End If
                End If
            End If
        End Sub

        Private Function GetTicketType(ByVal poTicketBet As DataRowView) As String
            Dim sType = SafeString(poTicketBet("TicketType"))
            Dim oDT As DataTable = CType(dgTicketBets.DataSource, DataTable)

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
            Dim ticketStatus As String = SafeString(poTicketBet("TicketStatus"))
            Dim sContext = SafeString(poTicketbet("Context"))
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

            Dim nSpreadMoney As Double = SafeRound(IIf(nHomeSpreadMoney <> 0, nHomeSpreadMoney, nAwaySpreadMoney)) _
                                                + SafeRound(poTicketBet("AddPointMoney"))

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
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
            htmlString += gameBet


            Return htmlString

        End Function


        Private Function getDetailByTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketBet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketBet("GameDate"))
            Dim sContext = SafeString(poTicketbet("Context"))
            Dim ticketStatus As String = SafeString(poTicketBet("TicketStatus"))
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
            Dim gameBet = String.Format("<div class='baseline'>{0} <b>{1} {2}</b> for the {5} {3} {4}</div>", sMsg, sTotalPoint, sMoney, regulationOnly, mustStarPitcher, ContextFormat(sContext))

            Dim sChoiceTeam = String.Format("{0} {1}", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", " - " & psAwayTeam))

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString

            'Return String.Format("{0}{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", "<br/> " & psAwayTeam), sMsg,
            '                    sTotalPoint, sMoney)
        End Function

        Private Function getDetailByTeamTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketBet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketBet("GameDate"))
            Dim sContext = SafeString(poTicketbet("Context"))
            Dim ticketStatus As String = SafeString(poTicketBet("TicketStatus"))
            Dim nTotalPoints As Double = SafeDouble(poTicketBet("TotalPoints")) + SafeDouble(poTicketBet("AddPoint"))
            Dim sTotalPoint As String = SafeString(nTotalPoints)
            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sTotalPoint = AHFormat(nTotalPoints).Replace("+"c, "")
            End If
            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

            Dim sMsg As String = SafeString(IIf(nOverMoney <> 0, "Over", "Under"))
            Dim nMoney As Double = SafeRound(IIf(nOverMoney <> 0, nOverMoney, nUnderMoney)) + SafeRound(poTicketBet("AddPointMoney"))

            Dim sMoney As String = SafeString(nMoney)
            Dim regulationOnly = IIf(IsSoccer(SafeString(poTicketBet("GameType"))), "<b>Regualation Only</b>", "")

            Dim gameBet = String.Format("<div class='baseline'>{0} <b>{1} {2}</b> for the {4} {3}</div>", sMsg, sTotalPoint, sMoney, regulationOnly, ContextFormat(sContext))

            Dim sChoiceTeam = String.Format("{0} {1}", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", " - " & psAwayTeam))

            If IsTennis(sGameType) OrElse IsGolf(sGameType) Then
                sChoiceTeam = psAwayTeam & " - " & psHomeTeam
            End If

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString
        End Function

        Private Function getDetailByMoneyLine(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim sContext = SafeString(poTicketbet("Context"))
            Dim ticketStatus As String = SafeString(poTicketbet("TicketStatus"))
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
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
            htmlString += gameBet


            Return htmlString

        End Function

        Private Function getDetailByDraw(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim nDrawLine As Double = SafeDouble(poTicketbet("DrawMoneyLine")) + SafeDouble(poTicketbet("AddPointMoney"))
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim ticketStatus As String = SafeString(poTicketbet("TicketStatus"))
            Dim sContext = SafeString(poTicketbet("Context"))

            Dim mustStarPitcher = GetMustStart(poTicketbet, sGameType)

            Dim regulationOnly = IIf(IsSoccer(sGameType), "<b>Regualation Only</b>", "")
            Dim gameBet = String.Format("<div class='baseline'>Money Line <b>{0}</b> for the {3} {1} {2}</div>", nDrawLine, regulationOnly, mustStarPitcher, ContextFormat(sContext))

            Dim sRotationNumber = ""

            Dim sChoiceTeam = String.Format("Draw({0} vs {1})", psHomeTeam, psAwayTeam)

            Dim htmlString As String = "<div class='baseline'>" & sRotationNumber & "<b class='gm-team'>" & sChoiceTeam & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
            htmlString += gameBet

            Return htmlString

            'Return "THis is draw" ' htmlString
        End Function

        Private Function getDetailForPop(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim sGameType As String = SafeString(poTicketbet("GameType"))
            Dim gameDate As Date = SafeDate(poTicketbet("GameDate"))
            Dim ticketStatus As String = SafeString(poTicketbet("TicketStatus"))
            Dim sPopDescription = SafeString(poTicketbet("PropDescription"))
            Dim sPropParticipantName = SafeString(poTicketbet("PropParticipantName"))
            Dim sPropMoneyLine = SafeString(poTicketbet("PropMoneyLine"))
            Dim sContext = SafeString(poTicketbet("Context"))

            Dim gameBet = String.Format("<div class='baseline'><b>{0}</b> for the {2} {1}</div>", sPropMoneyLine, sPopDescription, ContextFormat(sContext))

            Dim htmlString As String = "<div class='baseline'><b class='gm-team'>" & sPropParticipantName & "</b> "
            htmlString += "<span class='gm-date'>" & gameDate.ToString("MM/dd/yyyy") & "</span>&nbsp;<span class='gm-time'>(" & gameDate.ToString("HH:mm tt") & ")</span>&nbsp;"
            htmlString += "<span class='gm-status'>(" & CustomUpperTitleCase(ticketStatus) & ")</span> </div>"
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
                            return "Game"

                        Case "1h"
                            return "1st Half"

                        Case "2h"
                            return "2nd Half"

                        Case "1q"
                            return "1st Quarter"

                        Case "2q"
                            return "2nd Quarter"

                        Case "3q"
                            return "3rd Quarter"

                        Case "4q"
                            return "4th Quarter"

                        Case Else
                            return ""
                    End Select
        End Function

        Private Function CustomUpperTitleCase(ByVal sTitle As String) As String
            Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
            Return ti.ToTitleCase(sTitle.ToLower())                    
        End Function

    End Class

End Namespace

