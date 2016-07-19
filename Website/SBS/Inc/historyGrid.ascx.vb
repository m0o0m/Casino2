Imports System.Data

Imports SBCBL.Managers
Imports SBCBL.std

Namespace SBSWebsite

    Partial Class historyGrid
        Inherits SBCBL.UI.CSBCUserControl

        Private HistoryTicketsCount As Int32 = 0
        Private TotalBets As Double = 0
        Private TotalRisk As Double = 0
        Private TotalWin As Double = 0
        Private TotalBalance As Double = 0

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

            If poHistoryTickets.Rows.Count > 0 Then
                Dim odrTotal As DataRow = poHistoryTickets.NewRow
                odrTotal("TicketID") = newGUID()
                poHistoryTickets.Rows.Add(odrTotal)

                Me.HistoryTicketsCount = poHistoryTickets.Rows.Count
            End If

            grdHistory.DataSource = poHistoryTickets
            grdHistory.Columns(0).Visible = ShowCAgentName
            grdHistory.Columns(7).Visible = ShowPlayerName
            grdHistory.Columns(5).Visible = Not String.IsNullOrEmpty(AgentName)
            grdHistory.DataBind()

            ''if you change position of column please change it in formatDisplayGridWithRowSpan method also.
            If Me.HistoryTicketsCount > 0 Then
                formatDisplayGridWithRowSpan(poHistoryTickets)
                If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
                    formatDisplayGridWithRowSpanCAgent(poHistoryTickets)
                End If

            End If
        End Sub

        Private Sub formatDisplayGridWithRowSpanCAgent(ByVal poHistoryTickets As DataTable)
            Dim oCountLogins As List(Of CItemCount) = (From oRow In poHistoryTickets.AsEnumerable _
                Group oRow By login = SafeString(oRow.Field(Of String)("Login")) Into Count() _
                Where Count >= 2 _
                Select New CItemCount(login, Count)).ToList()

            If oCountLogins.Count = 0 Then
                ' ShowTotal()
                Return
            End If

            Dim nIndex As Integer = 0
            Dim oBackColor As Drawing.Color = Drawing.Color.White

            While nIndex < grdHistory.Items.Count - 1
                Dim oItem As DataGridItem = grdHistory.Items(nIndex)
                oItem.BackColor = oBackColor

                Dim sCagent As String = SafeString(CType(oItem.FindControl("hfCAgent"), HiddenField).Value)
                Dim oCountLogin As CItemCount = oCountLogins.Find(Function(x) x.ItemID = sCagent)

                If oCountLogin IsNot Nothing Then
                    Dim nRowSpan As Integer = oCountLogin.ItemCount
                    oItem.Cells(0).RowSpan = nRowSpan 'CAgent

                    For nRowSpanStart As Integer = 1 To nRowSpan - 1
                        Dim oItemRowSpan As DataGridItem = grdHistory.Items(nRowSpanStart + nIndex)
                        oItemRowSpan.BackColor = oBackColor
                        oItemRowSpan.Cells(0).Visible = False
                    Next

                    nIndex += nRowSpan
                Else
                    nIndex += 1
                End If

                If oBackColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6") Then
                    oBackColor = Drawing.Color.White
                Else
                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6")
                End If
            End While

            ShowTotal()

        End Sub
        Public Sub ShowTotal()
            ''total row
            Dim oTotalItem As DataGridItem = grdHistory.Items(grdHistory.Items.Count - 1)
            If ShowCAgentName Then
                oTotalItem.Cells(0).ColumnSpan = 1
                'oTotalItem.Cells(2).Visible = False
                'oTotalItem.Cells(3).Visible = False

                'oTotalItem.Cells(4).Visible = False
                'oTotalItem.Cells(5).Visible = False
                'oTotalItem.Cells(6).Visible = False
                Return
            End If
            oTotalItem.Cells(0).Visible = True
            If ShowPlayerName Then
                oTotalItem.Cells(1).ColumnSpan = 1
            Else
                oTotalItem.Cells(1).ColumnSpan = 1
            End If
        End Sub

        Public Function BindCAgent(ByVal obj As Object) As String

            If UserSession.UserType = SBCBL.EUserType.CallCenterAgent Then
                Dim oDR As System.Data.DataRowView = CType(obj, System.Data.DataRowView)
                Return SafeString(oDR("Login"))
            End If
            Return ""
        End Function



        Private Sub formatDisplayGridWithRowSpan(ByVal poHistoryTickets As DataTable)
            Dim oCountByTicketIDs As List(Of CItemCount) = (From oRow In poHistoryTickets.AsEnumerable _
                Group oRow By sTicketID = SafeString(oRow.Field(Of Guid)("TicketID")) Into Count() _
                Where Count >= 2 _
                Select New CItemCount(sTicketID, Count)).ToList()

            If oCountByTicketIDs.Count = 0 Then
                Return
            End If

            Dim nIndex As Integer = 0
            Dim oBackColor As Drawing.Color = Drawing.Color.White

            While nIndex < grdHistory.Items.Count - 1
                Dim oItem As DataGridItem = grdHistory.Items(nIndex)
                oItem.BackColor = oBackColor

                Dim sTicketID As String = SafeString(CType(oItem.FindControl("hfTicketID"), HiddenField).Value)
                Dim oCountByTicketID As CItemCount = oCountByTicketIDs.Find(Function(x) x.ItemID = sTicketID)

                If oCountByTicketID IsNot Nothing Then
                    Dim nRowSpan As Integer = oCountByTicketID.ItemCount

                    Dim lblTicket As Label = CType(oItem.FindControl("lblTicket"), Label)
                    lblTicket.Text = nRowSpan.ToString() & " Teams " & lblTicket.Text

                    oItem.Cells(1).RowSpan = nRowSpan 'Internet/Phone
                    oItem.Cells(2).RowSpan = nRowSpan 'Ticket
                    oItem.Cells(3).RowSpan = nRowSpan 'TicketDate
                    oItem.Cells(10).RowSpan = nRowSpan 'Risk/Win7
                    oItem.Cells(11).RowSpan = nRowSpan 'Amount8
                    oItem.Cells(12).RowSpan = nRowSpan 'Balance10
                    oItem.Cells(5).RowSpan = nRowSpan 'Agent

                    'oItem.Cells(4).RowSpan = nRowSpan 'Player
                    oItem.Cells(6).RowSpan = nRowSpan 'sport type
                    oItem.Cells(7).RowSpan = nRowSpan 'Player
                    For nRowSpanStart As Integer = 1 To nRowSpan - 1
                        Dim oItemRowSpan As DataGridItem = grdHistory.Items(nRowSpanStart + nIndex)
                        oItemRowSpan.BackColor = oBackColor
                        oItemRowSpan.Cells(0).Visible = False
                        oItemRowSpan.Cells(1).Visible = False
                        oItemRowSpan.Cells(2).Visible = False
                        oItemRowSpan.Cells(3).Visible = False
                        oItemRowSpan.Cells(10).Visible = False
                        oItemRowSpan.Cells(11).Visible = False
                        oItemRowSpan.Cells(12).Visible = False
                        'oItemRowSpan.Cells(4).Visible = False
                        oItemRowSpan.Cells(7).Visible = False

                        oItemRowSpan.Cells(6).Visible = False 'sport type
                        oItemRowSpan.Cells(5).Visible = False 'agent
                    Next

                    nIndex += nRowSpan
                Else
                    nIndex += 1
                End If

                If oBackColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6") Then
                    oBackColor = Drawing.Color.White
                Else
                    oBackColor = System.Drawing.ColorTranslator.FromHtml("#E6E6E6")
                End If
            End While

            ''total row
            Dim oTotalItem As DataGridItem = grdHistory.Items(grdHistory.Items.Count - 1)
            If ShowPlayerName Then
                oTotalItem.Cells(1).ColumnSpan = 1
            Else
                oTotalItem.Cells(1).ColumnSpan = 1
            End If

            'oTotalItem.Cells(1).Visible = False
            'oTotalItem.Cells(2).Visible = False
            'oTotalItem.Cells(3).Visible = False

            'oTotalItem.Cells(4).Visible = False
            'oTotalItem.Cells(5).Visible = False
        End Sub

        Protected Sub grdHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdHistory.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                Dim oTicketBet As DataRowView = CType(e.Item.DataItem, DataRowView)

                If e.Item.ItemIndex = Me.HistoryTicketsCount - 1 Then
                    e.Item.CssClass = "tableheading"
                    e.Item.Cells(1).Text = "TOTAL"
                    e.Item.Cells(8).Text = FormatNumber(Me.TotalBets, 0) & " BETS"
                    e.Item.Cells(12).Text = SafeString(IIf(Me.TotalBalance >= 0, "", "-")) & FormatNumber(Math.Abs(Me.TotalBalance), Me.RoundMidPoint)
                Else
                    CType(e.Item.FindControl("lblUserPhone"), Label).Text = SafeString(oTicketBet("TypeOfBet"))
                    If UserSession.UserType <> SBCBL.EUserType.Player AndAlso SafeString(oTicketBet("TypeOfBet")).Equals(Phone_Type, StringComparison.OrdinalIgnoreCase) Then
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
                        If Not String.IsNullOrEmpty(hfFileName.Value) AndAlso UserSession.UserType <> SBCBL.EUserType.Player Then
                            Dim lbtRecord As LinkButton = CType(e.Item.FindControl("lbtRecord"), LinkButton)
                            lbtRecord.Visible = True
                            lbtRecord.OnClientClick = String.Format("openDialog('/Utils/ListenMedia.aspx?fname={0}',315,400)", SafeString(hfFileName.Value))
                        End If
                    End If

                    If AssignRecordingLink <> "" AndAlso UCase(SafeString(oTicketBet("TypeOfBet"))) = "PHONE" Then
                        Dim btnAssignRecording As LinkButton = CType(e.Item.FindControl("btnAssignRecording"), LinkButton)
                        Dim sLink As String = AssignRecordingLink & "?TicketID=" & SafeString(oTicketBet("TicketID")) & "&TransactionDate=" & SafeString(oTicketBet("TransactionDate"))
                        btnAssignRecording.OnClientClick = String.Format("openDialog('{0}',500, 600, true, true);return false;", sLink)
                        btnAssignRecording.Visible = True
                    End If
                    CType(e.Item.FindControl("lblAgent"), Label).Text = AgentName
                    Dim sTicketType = GetTicketType(oTicketBet)
                    Dim sContext As String = SafeString(IIf(LCase(SafeString(oTicketBet("Context"))) = "current", "", SafeString(oTicketBet("Context")) & "<br/>"))
                    CType(e.Item.FindControl("lblTicket"), Label).Text = String.Format("{0}<br />#{1}{2}", sTicketType, oTicketBet("TicketNumber"), _
                                            IIf(SafeString(oTicketBet("SubTicketNumber")) <> "", "-" & SafeString(oTicketBet("SubTicketNumber")), ""))
                    CType(e.Item.FindControl("lblTicketDate"), Label).Text = SafeDate(oTicketBet("TransactionDate")) ').ToString()

                    Dim sStatus As String = SafeString(oTicketBet("TicketBetStatus"))
                    CType(e.Item.FindControl("lblResult"), Label).Text = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("TicketBetPush"))), IIf(UCase(sStatus) = "LOSE", "LOSS", sStatus), SafeString(oTicketBet("TicketBetPush")))
                    CType(e.Item.FindControl("lblPlaced"), Label).Text = SafeString(oTicketBet("GameDate"))
                    CType(e.Item.FindControl("lblSport"), Label).Text = SafeString(oTicketBet("GameType"))

                    If ShowPlayerName Then
                        CType(e.Item.FindControl("lblPlayer"), Label).Text = SafeString(oTicketBet("PlayerName"))
                    End If
                    If ShowCAgentName Then
                        CType(e.Item.FindControl("lblCAgent"), Label).Text = SafeString(oTicketBet("Login"))
                    End If

                    ''risk/win
                    Dim nRisk As Double = SafeRound(oTicketBet("RiskAmount"))
                    Dim nWin As Double = SafeRound(oTicketBet("WinAmount"))

                    CType(e.Item.FindControl("lblRiskWin"), Label).Text = String.Format("{0} / {1}", FormatNumber(nRisk, Me.RoundMidPoint), FormatNumber(nWin, Me.RoundMidPoint))


                    ''amount
                    Dim nAmount As Double = SafeRound(oTicketBet("NetAmount")) - nRisk
                    If nAmount < 0 Then
                        CType(e.Item.FindControl("lblAmount"), Label).ForeColor = Drawing.Color.Red
                    End If
                    CType(e.Item.FindControl("lblAmount"), Label).Text = SafeString(IIf(nAmount >= 0, "", "-")) & FormatNumber(Math.Abs(nAmount), Me.RoundMidPoint)

                    Dim sScores As String = "<b>{0}</b> - <b>{1}</b>"
                    Select Case LCase(SafeString(oTicketBet("Context")))
                        Case "current"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayScore")), SafeString(oTicketBet("HomeScore")))

                        Case "1h"
                            sScores = String.Format(sScores, SafeString(oTicketBet("AwayFirstHalfScore")), SafeString(oTicketBet("HomeFirstHalfScore")))

                        Case "2h"
                            sScores = String.Format(sScores, SafeString(SafeInteger(oTicketBet("AwayScore")) - SafeInteger(oTicketBet("AwayFirstHalfScore"))), SafeString(SafeInteger(oTicketBet("HomeScore")) - SafeInteger(oTicketBet("HomeFirstHalfScore"))))

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
                        Me.TotalBalance += nAmount
                        Me.TotalRisk += nRisk
                        Me.TotalWin += nWin
                    End If

                    ''balance

                    If TotalBalance < 0 Then
                        CType(e.Item.FindControl("lblBalance"), Label).ForeColor = Drawing.Color.Red
                    End If

                    CType(e.Item.FindControl("lblBalance"), Label).Text = SafeString(IIf(Me.TotalBalance >= 0, "", "-")) & FormatNumber(Math.Abs(Me.TotalBalance), Me.RoundMidPoint)

                    ''description
                    Dim sHomeTeam As String = SafeString(oTicketBet("HomeTeam")) & " - H"
                    Dim sAwayTeam As String = SafeString(oTicketBet("AwayTeam")) & " - A"
                    If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                        sHomeTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    End If
                    If SafeString(oTicketBet("HomePitcher_TicketBets")) <> "" AndAlso SafeString(oTicketBet("AwayPitcher_TicketBets")) <> "" Then
                        sAwayTeam += "<br/><span style='color:red'>" & SafeString(oTicketBet("AwayPitcher_TicketBets")) & " / " & SafeString(oTicketBet("HomePitcher_TicketBets")) & "</span>"
                    End If

                    If UCase(SafeString(oTicketBet("IsForProp"))).Equals("Y") Then

                        CType(e.Item.FindControl("lblDescription"), Label).Text = String.Format("{0}<br/><span style='color:red'>{1}</span><br/><span style='font-weight:900'>{2}</span><br/>", SafeString(oTicketBet("PropDescription")), SafeString(oTicketBet("PropParticipantName")), SafeString(oTicketBet("PropMoneyLine")))
                    Else
                        Dim sDescription As String = ""
                        If GetGameType.ContainsKey(SafeString(oTicketBet("gametype"))) AndAlso Not IsBasketball(SafeString(oTicketBet("gametype"))) AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Preseason") AndAlso Not GetGameType(SafeString(oTicketBet("gametype"))).Equals("NFL Football") Then
                            sDescription = IIf(String.IsNullOrEmpty(SafeString(oTicketBet("Description"))), "", SafeString(oTicketBet("Description")) & "<br/>")
                        End If
                        Select Case UCase(SafeString(oTicketBet("BetType")))
                            Case "SPREAD"
                                If IsSoccer(oTicketBet("GameType")) AndAlso SafeInteger(oTicketBet("AwayScore")) = SafeInteger(oTicketBet("HomeScore")) Then
                                    Dim nHomeSpread As Double = SafeDouble(oTicketBet("HomeSpread"))
                                    Dim nAwaySpread As Double = SafeDouble(oTicketBet("AwaySpread"))
                                    Dim nHomeSpreadMoney As Double = SafeDouble(oTicketBet("HomeSpreadMoney"))

                                    Dim nSpread As Double = SafeDouble(IIf(nHomeSpreadMoney <> 0, nHomeSpread, nAwaySpread)) + SafeDouble(oTicketBet("AddPoint"))
                                    If nSpread = 0 Then
                                        CType(e.Item.FindControl("lblResult"), Label).Text = "Push"
                                    End If
                                End If
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
                                If IsSoccer(oTicketBet("GameType")) AndAlso SafeInteger(oTicketBet("AwayScore")) = SafeInteger(oTicketBet("HomeScore")) Then
                                    CType(e.Item.FindControl("lblResult"), Label).Text = "LOSS"
                                End If
                            Case "DRAW"
                                sHomeTeam = Replace(sHomeTeam, " - H", "")
                                sAwayTeam = Replace(sAwayTeam, " - A", "")

                                sDescription += getDetailByDraw(sHomeTeam, sAwayTeam, oTicketBet)
                        End Select
                        CType(e.Item.FindControl("lblDescription"), Label).Text = sContext & sDescription
                    End If

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
            Dim nHomeSpread As Double = SafeDouble(poTicketBet("HomeSpread"))
            Dim nAwaySpread As Double = SafeDouble(poTicketBet("AwaySpread"))
            Dim nHomeSpreadMoney As Double = SafeDouble(poTicketBet("HomeSpreadMoney"))
            Dim nAwaySpreadMoney As Double = SafeDouble(poTicketBet("AwaySpreadMoney"))
            Dim sDescription As String
            Dim sChoiceTeam As String = SafeString(IIf(nHomeSpreadMoney <> 0, psHomeTeam, psAwayTeam))

            Dim nSpread As Double = SafeDouble(IIf(nHomeSpreadMoney <> 0, nHomeSpread, nAwaySpread)) + SafeDouble(poTicketBet("AddPoint"))
            Dim sSpread As String = SafeString(nSpread)
            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sSpread = AHFormat(nSpread)
            End If
            Dim nSpreadMoney As Double = SafeRound(IIf(nHomeSpreadMoney <> 0, nHomeSpreadMoney, _
                                             nAwaySpreadMoney)) + SafeRound(poTicketBet("AddPointMoney"))

            Dim sSpreadMoney As String = SafeString(nSpreadMoney)
            If SafeString(poTicketBet("GameType")).Equals("Tennis", StringComparison.CurrentCultureIgnoreCase) OrElse SafeString(poTicketBet("GameType")).Equals("Golf", StringComparison.CurrentCultureIgnoreCase) Then
                sDescription = SafeString(poTicketBet("AwayTeam")) & " / " & SafeString(poTicketBet("HomeTeam"))
                Return String.Format("{2}<br/><b>{0}</b><br/>({3}){1}", sChoiceTeam, sSpreadMoney, sDescription, IIf(nSpread = 0, "PK", IIf(nSpread > 0, "+" & sSpread, sSpread)))
            Else
                Return String.Format("{0}<br/> <b>{1} ({2})</b>", sChoiceTeam, IIf(nSpread = 0, "PK", IIf(nSpread > 0, "+" & sSpread.Replace("+", ""), sSpread)), sSpreadMoney).Replace("++", "+")
            End If
            'Return String.Format("{0}<br/> <b>{1} ({2})</b>", sChoiceTeam, IIf(nSpread = 0, "PK", sSpread), sSpreadMoney)
        End Function

        Private Function getDetailByTotalPoints(ByVal psHomeTeam As String, ByVal psAwayTeam As String, ByVal poTicketBet As DataRowView) As String
            Dim nTotalPoints As Double = SafeDouble(poTicketBet("TotalPoints")) + SafeDouble(poTicketBet("AddPoint"))

            Dim nOverMoney As Double = SafeRound(poTicketBet("TotalPointsOverMoney"))
            Dim nUnderMoney As Double = SafeRound(poTicketBet("TotalPointsUnderMoney"))

            Dim sMsg As String = SafeString(IIf(nOverMoney <> 0, "Over", "Under"))
            Dim nMoney As Double = SafeDouble(IIf(nOverMoney <> 0, nOverMoney, nUnderMoney)) + SafeDouble(poTicketBet("AddPointMoney"))
            Dim sTotalPoints As String = SafeString(nTotalPoints)

            If IsSoccer(SafeString(poTicketBet("gametype"))) Then
                sTotalPoints = AHFormat(nTotalPoints).Replace("+"c, "")
            End If

            Dim sMoney As String = SafeString(nMoney)
            Return String.Format("{0}{1}<br/> <b>{2} {3} ({4})</b>", psHomeTeam, IIf(String.IsNullOrEmpty(psAwayTeam), "", "<br/> " & psAwayTeam), sMsg, _
                                 sTotalPoints, sMoney)
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
            Dim nMoneyLine As Double = IIf(nHomeMoneyLine <> 0, nHomeMoneyLine, nAwayMoneyLine)
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

