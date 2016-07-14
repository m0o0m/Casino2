Imports System.Data

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBCWebsite

    Partial Class ticketBetsGridAgent
        Inherits SBCBL.UI.CSBCUserControl

        Private _TempScore As Hashtable
        Private Phone_Type As String = "Phone"

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

        Public ReadOnly Property ResultGrid() As DataGrid
            Get
                Return dgTicketBets
            End Get
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
                dgTicketBets.Columns(4).Visible = value
            End Set
        End Property

        Public Property AssignRecordingLink() As String
            Get
                Return SafeString(ViewState("__ASSIGN_RECORDING_LINK"))
            End Get
            Set(ByVal value As String)
                ViewState("__ASSIGN_RECORDING_LINK") = value
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

        Public Sub LoadTicketBets(ByVal poTicketBets As DataTable, ByVal psContext As String, Optional ByVal pbIsTicker As Boolean = False)
            IsLiveTicker = pbIsTicker
            If IsLiveTicker Then
                pnColor.Visible = True
            End If
            If Not String.IsNullOrEmpty(psContext) Then
                poTicketBets.DefaultView.RowFilter = String.Format("Context like '%{0}'", psContext)
                poTicketBets = poTicketBets.DefaultView.ToTable()
            End If

            If poTicketBets IsNot Nothing AndAlso poTicketBets.Rows.Count > 0 Then
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
        End Sub

        Private Sub formatDisplayGridWithRowSpan(ByVal poTicketbets As DataTable)
            Dim oCountByTicketIDs As List(Of CItemCount) = (From oRow In poTicketbets.AsEnumerable _
                Group oRow By sTicketID = SafeString(oRow.Field(Of Guid)("TicketID")) Into Count() _
                Where Count >= 2 _
                Select New CItemCount(sTicketID, Count)).ToList()

            If oCountByTicketIDs.Count = 0 Then
                Return
            End If

            Dim nIndex As Integer = 0
            Dim oBackColor As Drawing.Color = Drawing.Color.White

            While nIndex < dgTicketBets.Items.Count - 1
                Dim oItem As DataGridItem = dgTicketBets.Items(nIndex)
                oItem.BackColor = oBackColor

                Dim sTicketID As String = SafeString(CType(oItem.FindControl("hfTicketID"), HiddenField).Value)
                Dim oCountByTicketID As CItemCount = oCountByTicketIDs.Find(Function(x) x.ItemID = sTicketID)

                If oCountByTicketID IsNot Nothing Then
                    Dim nRowSpan As Integer = oCountByTicketID.ItemCount

                    Dim lblTicket As Label = CType(oItem.FindControl("lblTicket"), Label)
                    lblTicket.Text = nRowSpan.ToString() & " Teams<br/>" & lblTicket.Text

                    oItem.Cells(0).RowSpan = nRowSpan 'Internet/Phone
                    oItem.Cells(1).RowSpan = nRowSpan 'Ticket
                    oItem.Cells(2).RowSpan = nRowSpan 'Ticket Date
                    oItem.Cells(4).RowSpan = nRowSpan 'Player
                    oItem.Cells(8).RowSpan = nRowSpan 'Risk/Win

                    For nRowSpanStart As Integer = 1 To nRowSpan - 1
                        Dim oItemRowSpan As DataGridItem = dgTicketBets.Items(nRowSpanStart + nIndex)
                        oItemRowSpan.BackColor = oBackColor

                        oItemRowSpan.Cells(0).Visible = False
                        oItemRowSpan.Cells(1).Visible = False
                        oItemRowSpan.Cells(2).Visible = False
                        oItemRowSpan.Cells(4).Visible = False
                        oItemRowSpan.Cells(8).Visible = False
                    Next

                    nIndex += nRowSpan
                Else
                    nIndex += 1
                End If

                If oBackColor = Drawing.Color.AliceBlue Then
                    oBackColor = Drawing.Color.White
                Else
                    oBackColor = Drawing.Color.AliceBlue
                End If
            End While

            ''total row
            Dim oTotalItem As DataGridItem = dgTicketBets.Items(dgTicketBets.Items.Count - 1)
            oTotalItem.Cells(0).ColumnSpan = 3
            oTotalItem.Cells(1).Visible = False
            oTotalItem.Cells(2).Visible = False
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

                If AssignRecordingLink <> "" AndAlso UCase(SafeString(oTicketBet("TypeOfBet"))) = "PHONE" Then
                    Dim btnAssignRecording As LinkButton = CType(e.Item.FindControl("btnAssignRecording"), LinkButton)
                    Dim sLink As String = AssignRecordingLink & "?TicketID=" & SafeString(oTicketBet("TicketID")) & "&TransactionDate=" & SafeString(oTicketBet("TransactionDate"))

                    btnAssignRecording.OnClientClick = String.Format("openDialog('{0}',500, 600, true, true);return false;", sLink)
                    If String.IsNullOrEmpty(SafeString(oTicketBet("SubTicketNumber"))) OrElse SafeInteger(oTicketBet("SubTicketNumber")) = 1 Then
                        btnAssignRecording.Visible = True

                    End If
                End If

                If e.Item.ItemIndex = Me.TicketBetsCount - 1 Then
                    e.Item.CssClass = "tableheading"
                    e.Item.Cells(0).Text = "TOTAL"
                    e.Item.Cells(6).Text = FormatNumber(Me.TotalBets, 0) & " BETS"
                    e.Item.Cells(8).Text = String.Format("{0} / {1}", FormatNumber(Me.TotalRisk, Me.RoundMidPoint), FormatNumber(Me.TotalWin, Me.RoundMidPoint))
                Else

                    If (UserSession.UserType = SBCBL.EUserType.CallCenterAgent OrElse UserSession.UserType = SBCBL.EUserType.Agent OrElse UserSession.UserType = SBCBL.EUserType.SuperAdmin) AndAlso SafeString(oTicketBet("TypeOfBet")).Equals(Phone_Type, StringComparison.OrdinalIgnoreCase) Then
                        ' Dim sAgentName As String = (New CCallCenterAgentManager).GetByID(oTicketBet("OrderBy").ToString()).Rows(0)("Name").ToString()
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

                    CType(e.Item.FindControl("lblUserPhone"), Label).Text = SafeString(oTicketBet("TypeOfBet"))
                    Dim sTicketType = GetTicketType(oTicketBet)
                    Dim sContext As String = SafeString(IIf(LCase(SafeString(oTicketBet("Context"))) = "current", "", SafeString(oTicketBet("Context")) & "<br/>"))
                    CType(e.Item.FindControl("lblTicket"), Label).Text = String.Format("{0}<br />#{1}{2}", sTicketType, oTicketBet("TicketNumber"), _
                                        IIf(SafeString(oTicketBet("SubTicketNumber")) <> "", "-" & SafeString(oTicketBet("SubTicketNumber")), ""))
                    CType(e.Item.FindControl("lblTicketDate"), Label).Text = SafeDate(oTicketBet("TransactionDate")) ').ToString()
                    If UserSession.UserType <> SBCBL.EUserType.CallCenterAgent Then
                        CType(e.Item.FindControl("lblPlayer"), Label).Text = SafeString(oTicketBet("PlayerName"))
                    End If
                    CType(e.Item.FindControl("lblSport"), Label).Text = SafeString(oTicketBet("GameType"))
                    CType(e.Item.FindControl("lblPlaced"), Label).Text = SafeString(oTicketBet("GameDate"))
                    CType(e.Item.FindControl("lblAction"), Label).Text = SafeString(oTicketBet("GameStatus"))

                    ''risk/win
                    Dim nRisk As Double = SafeRound(oTicketBet("RiskAmount"))
                    Dim nWin As Double = SafeRound(oTicketBet("WinAmount"))
                    CType(e.Item.FindControl("lblRiskWin"), Label).Text = String.Format("{0} / {1}", FormatNumber(nRisk, Me.RoundMidPoint), FormatNumber(nWin, Me.RoundMidPoint))
                    If IsLiveTicker Then
                        If 300 < nRisk AndAlso nRisk < 499 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FFCC66")
                        ElseIf 500 < nRisk AndAlso nRisk < 999 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9900")
                        ElseIf 1000 < nRisk AndAlso nRisk < 1999 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF6600")
                        ElseIf 2000 < nRisk AndAlso nRisk < 2999 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#FF3300")
                        ElseIf 3000 < nRisk AndAlso nRisk < 3999 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#00DD00")
                        ElseIf 4000 < nRisk AndAlso nRisk < 4999 Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#008800")
                        ElseIf 5000 < nRisk Then
                            CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell).BackColor = System.Drawing.ColorTranslator.FromHtml("#0000FF")

                        End If
                    End If
                    'CType(CType(e.Item.FindControl("lblRiskWin"), Label).Parent, TableCell)

                    Dim sScores As String = "<b>{0}</b> - <b>{1}</b>"
                    Select Case LCase(SafeString(oTicketBet("Context")))
                        Case "current"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayScore")), SafeString(oTicketBet("HomeScore")))

                        Case "1h"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstHalfScore")), SafeString(oTicketBet("HomeFirstHalfScore")))

                        Case "2h"
                            sScores = String.Format(sScores, SafeString(SafeDouble(oTicketBet("AwayScore")) - SafeDouble(oTicketBet("AwayFirstHalfScore"))), _
                                                    SafeString(SafeDouble(oTicketBet("HomeScore")) - SafeDouble(oTicketBet("HomeFirstHalfScore"))))

                        Case "1q"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstQScore")), SafeString(oTicketBet("HomeFirstQScore")))

                        Case "2q"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwaySecondQScore")), SafeString(oTicketBet("HomeSecondQScore")))

                        Case "3q"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayThirdQScore")), SafeString(oTicketBet("HomeThirdQScore")))

                        Case "4q"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayFourQScore")), SafeString(oTicketBet("HomeFourQScore")))

                        Case Else
                            sScores = ""
                    End Select

                    CType(e.Item.FindControl("lblScore"), Label).Text = sScores

                    ''total bet
                    Dim sTicketID As String = CType(e.Item.FindControl("hfTicketID"), HiddenField).Value

                    If Me.__CurrentTicketID <> sTicketID Then
                        Me.__CurrentTicketID = sTicketID
                        Me.TotalBets += 1
                        Me.TotalRisk += nRisk
                        Me.TotalWin += nWin
                    End If

                    ''description
                    Dim sHomeTeam As String = SafeString(oTicketBet("HomeTeam")) & " - H"
                    Dim sAwayTeam As String = SafeString(oTicketBet("AwayTeam")) & " - A"

                    If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                        sHomeTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    End If
                    If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                        sAwayTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    End If

                    If SafeBoolean(oTicketBet("IsForProp")) Then
                        CType(e.Item.FindControl("lblDescription"), Label).Text = String.Format("{0}<br/><span style='color:red'>{1}</span>", _
                                                                                                SafeString(oTicketBet("PropDescription")), _
                                                                                                SafeString(oTicketBet("PropParticipantName")))
                    Else
                        'Dim sDescription As String = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("Description"))), "", SafeString(oTicketBet("Description")) & "<br/>")
                        Dim sDescription As String = ""
                        If GetGameType.ContainsKey(SafeString(oTicketBet("gametype"))) AndAlso Not IsBasketball(SafeString(oTicketBet("gametype"))) AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Preseason") AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Football") Then
                            sDescription = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("Description"))), "", SafeString(oTicketBet("Description")) & "<br/>")
                        End If
                        Select Case UCase(SafeString(oTicketBet("BetType")))
                            Case "SPREAD"
                                sDescription += getDetailBySpread(sHomeTeam, sAwayTeam, oTicketBet)
                            Case "TOTALPOINTS"
                                sHomeTeam = Replace(sHomeTeam, " - H", "")
                                sAwayTeam = Replace(sAwayTeam, " - A", "")

                                If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                                    sHomeTeam = SafeString(oTicketBet("AwayTeam")) & "/" & sHomeTeam
                                    sDescription += getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                                Else
                                    sHomeTeam = sAwayTeam & " / " & sHomeTeam
                                    sDescription += getDetailByTotalPoints(sHomeTeam, "", oTicketBet)
                                End If
                            Case "TEAMTOTALPOINTS"
                                sHomeTeam = Replace(sHomeTeam, " - H", "")
                                sAwayTeam = Replace(sAwayTeam, " - A", "")

                                sDescription += getDetailByTeamTotalPoints(IIf(SafeString(oTicketBet("TeamTotalName")).Equals("away"), sAwayTeam, sHomeTeam), "", oTicketBet)
                            Case "MONEYLINE"
                                sDescription += getDetailByMoneyLine(sHomeTeam, sAwayTeam, oTicketBet)
                            Case "DRAW"
                                sHomeTeam = Replace(sHomeTeam, " - H", "")
                                sAwayTeam = Replace(sAwayTeam, " - A", "")

                                sDescription += getDetailByDraw(sHomeTeam, sAwayTeam, oTicketBet)
                        End Select

                        CType(e.Item.FindControl("lblDescription"), Label).Text = sContext & sDescription
                    End If
                    Dim sTicketBetID As String = SafeString(oTicketBet("TicketBetID"))
                    Dim sScore As String = SafeString(oTicketBet("AwayScore")) & SafeString(oTicketBet("HomeScore"))

                    '' Check New Changes
                    If LastScores.ContainsKey(sTicketBetID) Then
                        If SafeString(LastScores.Item(sTicketBetID)) <> sScore Then
                            e.Item.Cells(8).Attributes.Add("class", "HighLightBlock")
                        End If
                        _TempScore.Add(sTicketBetID, SafeString(LastScores.Item(sTicketBetID)))
                    Else
                        _TempScore.Add(sTicketBetID, sScore)
                    End If
                End If
            End If
        End Sub

        Private Function GetTicketType(ByVal poTicketBet As DataRowView) As String
            If SafeBoolean(poTicketBet("IsForProp")) Then
                Return "Proposition"
            End If

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
            Dim nHomeSpread As Double = SafeDouble(poTicketBet("HomeSpread"))
            Dim nAwaySpread As Double = SafeDouble(poTicketBet("AwaySpread"))
            Dim nHomeSpreadMoney As Double = SafeDouble(poTicketBet("HomeSpreadMoney"))
            Dim nAwaySpreadMoney As Double = SafeDouble(poTicketBet("AwaySpreadMoney"))
            Dim sDescription As String
            Dim nSpread As Double = SafeDouble(IIf(nHomeSpreadMoney <> 0, nHomeSpread, nAwaySpread)) + SafeDouble(poTicketBet("AddPoint"))
            Dim sSpread As String = SafeString(nSpread)
            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sSpread = AHFormat(nSpread)
            End If
            Dim sChoiceTeam As String = SafeString(IIf(nHomeSpreadMoney <> 0, psHomeTeam, psAwayTeam))

            Dim nSpreadMoney As Double = SafeRound(IIf(nHomeSpreadMoney <> 0, nHomeSpreadMoney, nAwaySpreadMoney)) _
                                                + SafeRound(poTicketBet("AddPointMoney"))

            Dim sSpreadMoney As String = SafeString(nSpreadMoney)
            If SafeString(poTicketBet("GameType")).Equals("Tennis", StringComparison.CurrentCultureIgnoreCase) OrElse SafeString(poTicketBet("GameType")).Equals("Golf", StringComparison.CurrentCultureIgnoreCase) Then
                sDescription = SafeString(poTicketBet("AwayTeam")) & " / " & SafeString(poTicketBet("HomeTeam"))
                Return String.Format("{2}<br/><b>{0}</b><br/>({3}){1}", sChoiceTeam, sSpreadMoney, sDescription, IIf(nSpread = 0, "PK", IIf(nSpread > 0, "+" & sSpread, sSpread)))
            Else
                Return String.Format("{0}<br/> <b>{1} ({2})</b>", sChoiceTeam, IIf(nSpread = 0, "PK", IIf(nSpread > 0, "+" & sSpread, sSpread)), sSpreadMoney).Replace("++", "+")
            End If


        End Function

        Private Function getDetailByTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
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
            Return String.Format("{0}{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", "<br/> " & psAwayTeam), sMsg, _
                                 sTotalPoint, sMoney)
        End Function


        Private Function getDetailByTeamTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
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
            Return String.Format("{0}{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", "<br/> " & psAwayTeam), sMsg, _
                                 sTotalPoint, sMoney)
        End Function

        Private Function getDetailByMoneyLine(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim nHomeMoneyLine As Double = SafeDouble(poTicketbet("HomeMoneyLine"))
            Dim nAwayMoneyLine As Double = SafeDouble(poTicketbet("AwayMoneyLine"))
            Dim sDescription As String
            Dim sChoiceTeam As String = SafeString(IIf(nHomeMoneyLine <> 0, psHomeTeam, psAwayTeam))
            Dim nMoneyLine As Double = SafeDouble(IIf(nHomeMoneyLine <> 0, nHomeMoneyLine, nAwayMoneyLine))
            Dim sMoneyLine As String = SafeString(nMoneyLine)
            If SafeString(poTicketbet("GameType")).Equals("Tennis", StringComparison.CurrentCultureIgnoreCase) OrElse SafeString(poTicketbet("GameType")).Equals("Golf", StringComparison.CurrentCultureIgnoreCase) Then
                sDescription = SafeString(poTicketbet("AwayTeam")) & " / " & SafeString(poTicketbet("HomeTeam"))
                Return String.Format("{2}<br/><b>{0}</b><br/>{1}", sChoiceTeam, sMoneyLine, sDescription)
            Else
                Return String.Format("{0}<br/> <b>{1}</b>", sChoiceTeam, sMoneyLine)
            End If
        End Function

        Private Function getDetailByDraw(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketbet As DataRowView) As String
            Dim nDrawLine As Double = SafeDouble(poTicketbet("DrawMoneyLine")) + SafeDouble(poTicketbet("AddPointMoney"))

            Return String.Format("{0} - {1}<br/> <b>Draw {2}</b>", psHomeTeam, psAwayTeam, FormatNumber(nDrawLine, GetRoundMidPoint, TriState.UseDefault, TriState.False))
        End Function

        Private Function FormatPoint(ByVal pnPoint As Double, ByVal psGameType As String) As String
            If IsSoccer(psGameType) Then
                Return AHFormat(pnPoint)
            End If


            Return FormatNumber(pnPoint, GetRoundMidPoint, TriState.UseDefault, TriState.False)
        End Function
    End Class

End Namespace

