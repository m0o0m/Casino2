Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Data
Imports WebsiteLibrary.DBUtils
Imports SBCBL.Managers
Imports SBCBL.UI
Imports SBCBL.UI.CSBCSession
Imports System.Web

Namespace Tickets
    <Serializable()> _
    Public Class CTicketBet
        Implements IComparable(Of CTicketBet)
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CTicketBet))
        Public SetWinAmount As Boolean = True
        Public TicketBetID As String
        Public BetType As String 'Should be: MoneyLine, Spread or TotalPoints
        Public HomeSpread As Double
        Public HomeSpreadMoney As Double
        Public AwaySpread As Double
        Public AwaySpreadMoney As Double
        Public HomeMoneyLine As Double
        Public AwayMoneyLine As Double
        Public TotalPoints As Double
        Public TotalPointsOverMoney As Double
        Public TotalPointsUnderMoney As Double
        Public GameID As String
        Public GameLineID As String
        Public AddPoint As Double
        Public AddPointMoney As Double
        Public AddPointValid As Double
        Public AddPointMoneyValid As Double
        Public HomePitcher As String
        Public AwayPitcher As String
        Public DrawMoneyLine As Double
        Public Context As String
        Public PropMoneyLine As Double
        Public PropParticipantName As String
        Public PropRotationNumber As String
        Public Description As String
        Public IsCheckPitcher As Boolean = True
        Public BuyPointValue As String
        '' Dont save to DB
        Public GameType As String
        Public GameDate As DateTime
        Public HomePitcherRH As Boolean
        Public AwayPitcherRH As Boolean
        Public Bookmaker As String
        Public HomeTeam As String
        Public AwayTeam As String
        Public HomeTeamNumber As String
        Public AwayTeamNumber As String
        Public TicketID As String
        Public IsForProp As Boolean = False
        Public IsCircled As Boolean = False
        Public IsUpdate As Boolean = False
        Public IsFavorite As Boolean = False
        ' Public IsUnderdog As Boolean = False
        Public RiskAmount As Double
        Public TeamTotalName As String
        Public sOffTimeCategory As String = SBCBL.std.GetSiteType & " LineOffHour"

        Public Sub New()
            TicketBetID = newGUID()
        End Sub

        Public Sub New(ByVal poTicketBet As CTicketBet)
            TicketBetID = newGUID()
            With poTicketBet
                SetWinAmount = .SetWinAmount
                BetType = .BetType
                HomeSpread = .HomeSpread
                HomeSpreadMoney = .HomeSpreadMoney
                AwaySpread = .AwaySpread
                AwaySpreadMoney = .AwaySpreadMoney
                HomeMoneyLine = .HomeMoneyLine
                AwayMoneyLine = .AwayMoneyLine
                TotalPoints = .TotalPoints
                TotalPointsOverMoney = .TotalPointsOverMoney
                TotalPointsUnderMoney = .TotalPointsUnderMoney
                GameID = .GameID
                GameLineID = .GameLineID
                AddPoint = .AddPoint
                AddPointMoney = .AddPointMoney
                HomePitcher = .HomePitcher
                AwayPitcher = .AwayPitcher
                DrawMoneyLine = .DrawMoneyLine
                Context = .Context
                PropMoneyLine = .PropMoneyLine
                PropParticipantName = .PropParticipantName
                PropRotationNumber = .PropRotationNumber
                Description = .Description
                '' Dont save to DB
                GameType = .GameType
                GameDate = .GameDate
                HomePitcherRH = .HomePitcherRH
                AwayPitcherRH = .AwayPitcherRH
                Bookmaker = .Bookmaker
                HomeTeam = .HomeTeam
                AwayTeam = .AwayTeam
                HomeTeamNumber = .HomeTeamNumber
                AwayTeamNumber = .AwayTeamNumber
                TicketID = .TicketID
                IsForProp = .IsForProp
                IsFavorite = .IsFavorite
                ' IsUnderdog = .IsUnderdog
                RiskAmount = .RiskAmount
                TeamTotalName = .TeamTotalName
                IsCheckPitcher = .IsCheckPitcher
                BuyPointValue = .BuyPointValue
            End With
        End Sub

        Public ReadOnly Property Team() As String
            Get
                Dim sResult As String
                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        If IsForProp Then
                            sResult = Me.PropParticipantName
                        Else
                            If Me.HomeMoneyLine <> 0 Then
                                sResult = Me.HomeTeam
                            Else
                                sResult = Me.AwayTeam
                            End If
                        End If

                    Case "SPREAD"
                        If Me.HomeSpreadMoney <> 0 Then
                            sResult = Me.HomeTeam
                        Else
                            sResult = Me.AwayTeam
                        End If

                    Case "TOTALPOINTS"
                        sResult = String.Format("{0} / {1} ", Me.HomeTeam, Me.AwayTeam)

                    Case "TEAMTOTALPOINTS"
                        If Me.TeamTotalName.Equals("home", StringComparison.CurrentCultureIgnoreCase) Then
                            sResult = String.Format("{0} ({1}) ", Me.HomeTeam, Me.HomeTeamNumber)
                        Else
                            sResult = String.Format("{0} ({1}) ", Me.AwayTeam, Me.AwayTeamNumber)
                        End If

                    Case "DRAW"
                        sResult = String.Format("{0} ({1}) vs {2} ({3})", Me.HomeTeam, Me.HomeTeamNumber, Me.AwayTeam, Me.AwayTeamNumber)

                    Case Else
                        sResult = ""
                End Select

                Return sResult
            End Get
        End Property

        Public ReadOnly Property TeamNumber() As String
            Get
                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        If IsForProp Then
                            Return Me.PropRotationNumber
                        Else
                            If Me.HomeMoneyLine <> 0 Then
                                Return Me.HomeTeamNumber
                            Else
                                Return Me.AwayTeamNumber
                            End If
                        End If

                    Case "SPREAD"
                        If Me.HomeSpreadMoney <> 0 Then
                            Return Me.HomeTeamNumber
                        Else
                            Return Me.AwayTeamNumber
                        End If

                    Case Else
                        Return ""
                End Select
            End Get
        End Property

        Public ReadOnly Property SysSettingKey() As String
            Get
                Dim sParlayKey As String = ""

                Select Case UCase(Me.Context)
                    Case "CURRENT"
                        sParlayKey = ""
                    Case "1H"
                        sParlayKey = "1H"
                    Case "2H"
                        sParlayKey = "2H"
                    Case Else
                        sParlayKey = "Q"
                End Select

                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        sParlayKey &= "ML"
                    Case "SPREAD"
                        sParlayKey &= "Spread"
                    Case "TOTALPOINTS"
                        sParlayKey &= "Total"
                End Select

                Return sParlayKey
            End Get
        End Property

        Public ReadOnly Property SportType() As String
            Get
                Select Case True
                    Case IsFootball(Me.GameType)
                        Return "Football"

                    Case IsBaseball(Me.GameType)
                        Return "Baseball"

                    Case IsBasketball(Me.GameType)
                        Return "Basketball"

                    Case IsHockey(Me.GameType)
                        Return "Hockey"

                    Case IsSoccer(Me.GameType)
                        Return "Soccer"

                    Case IsOtherGameType(Me.GameType)
                        Return "Other"

                    Case Else
                        Return "Proposition"

                End Select
            End Get
        End Property

        Public ReadOnly Property HTML(Optional ByVal pbCheckBuyPoint As Boolean = False) As String
            Get
                ' HTML result
                Dim sHTML As String = "<tr><td>{2}</td><td nowrap='nowrap'><b>{3}</b></td><td nowrap='nowrap'>{4}</td><td nowrap='nowrap'>{5}</td>" & _
                "<td><input type='submit' id='btnDel{1}' name='btnDel{1}' value='Del' " & _
                "onclick='javascript:return RemoveBet(\""{0}\"",\""{1}\"");' " & _
                "title='Delete TicketBet' /></td></tr>"
                Dim sLine As String = ""

                ' Only Straight, Reverse, Parlay Wager have buy points feature
                If pbCheckBuyPoint Then
                    Dim olstBuyPoint As List(Of DictionaryEntry) = Me.ParseBuyPointOptions

                    If olstBuyPoint.Count > 0 Then ' Can Buy Point
                        ' Display dropdownlist buy points
                        sLine = String.Format("<select name='ddl{0}' id='ddlBuyPoint{0}' class='textInput' " & _
                                              "onblur='javascript: return BuyPoints(this, \""{1}\"", \""{0}\"");' >", Me.TicketBetID, Me.TicketID)

                        Dim sValue As String = ""
                        If Me.AddPoint <> 0 Then
                            sValue = SafeString(Me.AddPoint) & "|" & SafeString(Me.AddPointMoney)
                        End If

                        For Each oBuyPoint As DictionaryEntry In olstBuyPoint
                            If sValue = SafeString(oBuyPoint.Value) Then
                                sLine &= String.Format("<option selected='selected' value='{0}'>{1}</option>", oBuyPoint.Value, oBuyPoint.Key)
                            Else
                                sLine &= String.Format("<option value='{0}'>{1}</option>", oBuyPoint.Value, oBuyPoint.Key)
                            End If
                        Next

                        sLine &= "</select>"
                    End If
                End If

                Dim sStatus, sPoint, sMoney, sRate, sDisplay As String
                sStatus = ""
                sPoint = "0"
                sMoney = "0"
                sDisplay = "0"
                sRate = SafeString(Me.BetPoint)

                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        sStatus = ""
                        sPoint = ""
                        If Me.IsForProp Then
                            sDisplay = SafeString(Me.BetPoint + Me.AddPointMoney)
                            sMoney = SafeString(Me.BetPoint)
                        Else
                            If Me.HomeMoneyLine <> 0 Then
                                sDisplay = SafeString(Me.HomeMoneyLine + Me.AddPointMoney)
                                sMoney = SafeString(Me.HomeMoneyLine)
                            Else
                                sDisplay = SafeString(Me.AwayMoneyLine + Me.AddPointMoney)
                                sMoney = SafeString(Me.AwayMoneyLine)
                            End If
                        End If

                    Case "SPREAD"
                        sStatus = ""
                        If Me.HomeSpreadMoney <> 0 Then
                            If FormatPoint(Me.HomeSpread + Me.AddPoint, Me.GameType).Equals("0") Then
                                sDisplay = "PK (" & SafeString(Me.HomeSpreadMoney + Me.AddPointMoney) & ")"
                            Else
                                sDisplay = FormatPoint(Me.HomeSpread + Me.AddPoint, Me.GameType) & " (" & SafeString(Me.HomeSpreadMoney + Me.AddPointMoney) & ")"
                            End If

                            sPoint = SafeString(Me.HomeSpread)
                            sMoney = SafeString(Me.HomeSpreadMoney)
                        Else
                            If FormatPoint(Me.AwaySpread + Me.AddPoint, Me.GameType).Equals("0") Then
                                sDisplay = " PK (" & SafeString(Me.AwaySpreadMoney + Me.AddPointMoney) & ")"
                            Else
                                sDisplay = FormatPoint(Me.AwaySpread + Me.AddPoint, Me.GameType) & " (" & SafeString(Me.AwaySpreadMoney + Me.AddPointMoney) & ")"
                            End If

                            sPoint = SafeString(Me.AwaySpread)
                            sMoney = SafeString(Me.AwaySpreadMoney)
                        End If

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        sPoint = SafeString(Me.TotalPoints)

                        If Me.TotalPointsOverMoney <> 0 Then
                            sDisplay = "Over " & FormatPoint(Me.TotalPoints + Me.AddPoint, Me.GameType) & " (" & SafeString(Me.TotalPointsOverMoney + Me.AddPointMoney) & ")"
                            sStatus = "Over "
                            sMoney = SafeString(Me.TotalPointsOverMoney)
                        Else
                            sDisplay = "Under " & FormatPoint(Me.TotalPoints + Me.AddPoint, Me.GameType) & " (" & SafeString(Me.TotalPointsUnderMoney + Me.AddPointMoney) & ")"
                            sStatus = "Under "
                            sMoney = SafeString(Me.TotalPointsUnderMoney)
                        End If

                    Case "DRAW"
                        sStatus = ""
                        sPoint = ""
                        sDisplay = SafeString(Me.DrawMoneyLine + Me.AddPointMoney)
                        sMoney = SafeString(Me.DrawMoneyLine)

                    Case Else
                        '' Error
                End Select

                ' Display GameLine
                If sLine <> "" Then ' Can not buy point
                    sDisplay = ""
                End If

                sLine &= String.Format("<span id='span{0}' name='span{0}' Wager='{1}' GameType='{2}' " & _
                                      "Point='{3}' Status='{4}' Money='{5}' Rate='{6}'>{7}</span>", _
                                      Me.TicketBetID, Me.TicketID, Me.GameType, sPoint, sStatus, sMoney, sRate, sDisplay)


                sHTML = String.Format(sHTML, Me.TicketID, Me.TicketBetID, Me.TeamNumber, Me.Team, _
                                      SafeString(IIf(LCase(SafeString(Me.Context)) = "current", "", Me.Context)), sLine)

                Return sHTML
            End Get
        End Property

        Public ReadOnly Property PreviewHTML(ByVal pnIndex As Integer) As String
            Get
                ' HTML result
                Dim sHTML As String = String.Format("<tr><td>Select #{0}: </td><td>{1}</td></tr><tr><td></td><td {2}>", _
                                                    SafeString(pnIndex), Me.GameType, SafeString(IIf(Me.IsUpdate, "style=\""color: red;\""", "")))

                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        sHTML &= GetDetailByMoneyLine()

                    Case "SPREAD"
                        sHTML &= GetDetailBySpread()

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        sHTML &= GetDetailByTotalPoints()

                    Case "DRAW"
                        sHTML &= GetDetailByDraw()
                End Select

                sHTML &= "</td></tr>"

                Return sHTML
            End Get
        End Property

        Public Function GetDetailBySpread() As String
            Dim sMsg As String
            Dim nSpread As Double
            Dim nSpreadMoney As Double
            Dim sTeam As String
            sMsg = "<b>{0}</b>&nbsp; {1} &nbsp;<u><b>{2} ({3})</b></u>"

            If Me.HomeSpreadMoney <> 0 Then
                nSpread = Me.HomeSpread
                nSpreadMoney = Me.HomeSpreadMoney
                sTeam = Me.HomeTeam
            Else
                nSpread = Me.AwaySpread
                nSpreadMoney = Me.AwaySpreadMoney
                sTeam = Me.AwayTeam
            End If
            Dim sSpread As String = SafeString(nSpread + Me.AddPoint)
            Dim sSPreadPK As String = sSpread
            If IsSoccer(Me.GameType) Then
                sSpread = AHFormat(nSpread + Me.AddPoint)
            Else
                sSpread = SafeString(IIf(sSpread.Contains("-"), sSpread, "+" & sSpread))
            End If
            Dim sSpreadMoney As String = FormatNumber(nSpreadMoney + Me.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format(sMsg, IIf(LCase(SafeString(Me.Context)) = "current", "", "&nbsp;" & Me.Context), sTeam, IIf(SafeDouble(sSPreadPK) = 0, "PK", sSpread), sSpreadMoney)
        End Function

        Public Function GetDetailByTotalPoints() As String
            Dim sMsg As String
            Dim nMoney As Double

            If Me.TotalPointsOverMoney <> 0 Then
                sMsg = "Over"
                nMoney = Me.TotalPointsOverMoney
            Else
                sMsg = "Under"
                nMoney = Me.TotalPointsUnderMoney
            End If

            Dim sTotalPointsMoney As String = FormatNumber(nMoney + Me.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format("<b>{0}</b>&nbsp;{1} <b>-</b> {2} &nbsp;<u><b>{3} {4} ({5})</b></u>", _
                                 IIf(LCase(SafeString(Me.Context)) = "current", "", "&nbsp;" & Me.Context), Me.HomeTeam, _
                                 Me.AwayTeam, sMsg, FormatPoint(Me.TotalPoints + Me.AddPoint, Me.GameType).Replace("+"c, ""), IIf(sTotalPointsMoney.Contains("-"), sTotalPointsMoney, "+" & sTotalPointsMoney))
        End Function

        Public Function GetDetailByDraw() As String
            Dim sDrawLine As String = FormatNumber(Me.DrawMoneyLine + Me.AddPointMoney, GetRoundMidPoint, TriState.UseDefault, TriState.False)

            Return String.Format("<b>{0}</b>&nbsp;{1} <b>-</b> {2} &nbsp;<u><b>Draw ({3})</b></u>", _
                                 IIf(Me.Context = "Current", "", "&nbsp;" & Me.Context), _
            Me.HomeTeam, Me.AwayTeam, IIf(sDrawLine.Contains("-"), sDrawLine, "+" & sDrawLine))
        End Function

        Public Function GetDetailByMoneyLine() As String
            Dim sMsg As String = "<b>{0}</b>&nbsp;{1} &nbsp;<u><b>ML ({2})</b></u>"
            Dim nMoneyLine As Double
            Dim sTeam As String

            If Me.IsForProp Then
                nMoneyLine = Me.BetPoint + Me.AddPointMoney
                sTeam = Me.Team
            Else
                If Me.HomeMoneyLine <> 0 Then
                    nMoneyLine = Me.HomeMoneyLine + Me.AddPointMoney
                    sTeam = Me.HomeTeam
                Else
                    nMoneyLine = Me.AwayMoneyLine + Me.AddPointMoney
                    sTeam = Me.AwayTeam
                End If
            End If

            Dim sMoneyLine As String = FormatNumber(nMoneyLine, GetRoundMidPoint, TriState.UseDefault, TriState.False)
            Return String.Format(sMsg, IIf(LCase(SafeString(Me.Context)) = "current", "", "&nbsp;" & Me.Context), sTeam, IIf(sMoneyLine.Contains("-"), sMoneyLine, "+" & sMoneyLine))
        End Function

        Private Function FormatPoint(ByVal pnPoint As Double, ByVal psGameType As String) As String
            If IsSoccer(psGameType) Then
                Return AHFormat(pnPoint)
            End If
            Return SafeString(pnPoint)
        End Function

        Public Function CompareTo(ByVal poTicketBet As CTicketBet) As Integer Implements System.IComparable(Of CTicketBet).CompareTo
            Return TicketBetID.CompareTo(poTicketBet.TicketBetID)
        End Function

        Public ReadOnly Property BetPoint() As Double
            Get
                Select Case UCase(Me.BetType)
                    Case "MONEYLINE"
                        If IsForProp Then
                            Return Me.PropMoneyLine
                        Else
                            If Me.HomeMoneyLine <> 0 Then
                                Return Me.HomeMoneyLine
                            Else
                                Return Me.AwayMoneyLine
                            End If
                        End If

                    Case "SPREAD"
                        If Me.HomeSpreadMoney <> 0 Then
                            Return Me.HomeSpreadMoney
                        Else
                            Return Me.AwaySpreadMoney
                        End If

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        If Me.TotalPointsOverMoney <> 0 Then
                            Return Me.TotalPointsOverMoney
                        Else
                            Return Me.TotalPointsUnderMoney
                        End If

                    Case "DRAW"
                        Return Me.DrawMoneyLine

                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        Public Function ValidateStraightCircled(ByVal poOddsRules As COddRulesEngine, ByVal psTeamChoice As String, Optional ByVal psSuperAgentID As String = "") As Boolean
            '' check diff amount if over Limit that be set in SA
            Dim bResult As Boolean = False
            Dim nDiff As Double
            nDiff = poOddsRules.GetDiffAmountSuperAgent(Me.GameID, psTeamChoice, Me.Context, BetType, Me.RiskAmount, Me.TeamTotalName)
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            'Dim odtCircleGameValue As DataTable = oGameTypeOnOffManager.GetCircleGameValues(psSuperAgentID)
            Dim nCircleBet As Double = 0
            Dim oCacheManager As New CCacheManager()
            If oCacheManager.GetSysSettings(psSuperAgentID & "CircleSettings").GetIntegerValue("Straight") > 0 Then
                nCircleBet = SafeDouble(oCacheManager.GetSysSettings(psSuperAgentID & "CircleSettings").GetIntegerValue("Straight"))
                bResult = (nDiff <= nCircleBet)
            Else
                Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
                Dim sCategory = SBCBL.std.GetSiteType & " MaxCircled"
                Dim oListSettings As CSysSettingList = oSysManager.GetAllSysSettings(sCategory)
                '' Dont allow bet if Diff amount > max ammount set by SA
                Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = "Straight")
                If oSetting IsNot Nothing AndAlso SafeDouble(oSetting.Value) > 0 Then
                    bResult = (nDiff <= SafeDouble(oSetting.Value))
                End If
            End If
            Return bResult
        End Function

        ''' <summary>
        ''' This function's used to validate one bet when user place bet
        ''' </summary>
        ''' <param name="poOddsRules"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Validate(ByVal poOddsRules As COddRulesEngine, ByVal pbSuperPlayer As Boolean, ByVal pnSecondHalfTimeOff As Integer, ByVal pnIncreaseSpread As Integer, ByVal pnRiskAmount As Double, ByVal pnQuarterOffLine As Integer) As ECheckStatus
            '' Check betting is not out of date
            Me.RiskAmount = pnRiskAmount
            _log.Debug("BEGIN check valid TicketBetID: " & TicketBetID)
            Dim oResult As ECheckStatus = ECheckStatus.Success

            Dim oDB As CSQLDBUtils = Nothing
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Dim oToday As Date = GetEasternDate()
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("gl.GameLineID = " & SQLString(GameLineID))
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> 'Y'")
                oWhere.AppendANDCondition("isnull(GameStatus,'') <> 'Final'")
                oWhere.AppendANDCondition("gl.Bookmaker = " & SQLString(Bookmaker))

                If pbSuperPlayer Then
                    oWhere.AppendANDCondition("datediff( hour,Gamedate," & SQLString(oToday) & ") < 4 ")
                Else
                    If Not IsForProp Then
                        'oWhere.AppendANDCondition("((gl.Context <> '2H'  and g.GameDate > " & SQLString(oToday) & ") or (Context = '2H' and IsFirstHalfFinished = 'N' {0}) )")

                        If Not pbSuperPlayer Then
                            '' 2H line off: turn off 2H line after a period time that set by super, applies to football, basketball, soccer
                            Dim oSysSettingManager As New CSysSettingManager()
                            Dim oSys As CSysSetting = oSysSettingManager.GetAllSysSettings(sOffTimeCategory).Find(Function(x) UCase(x.Key) = Me.GameType)
                            LogDebug(_log, "Me.GameType" & Me.GameType & ":sOffTimeCategory" & sOffTimeCategory)
                            ' If context is 2H
                            If UCase(Me.Context) = "2H" Then
                                oWhere.AppendANDCondition("Context = '2H' and IsFirstHalfFinished = 'N'")
                                oWhere.AppendANDCondition(" GameDate < " & SQLString(std.GetEasternDate()))
                                If pnSecondHalfTimeOff <> 0 Then

                                    oWhere.AppendANDCondition(" DATEDIFF( minute,SecondHalfTime," & _
                                         SQLString(Now.ToUniversalTime()) & ") <= " & SafeString(pnSecondHalfTimeOff))
                                End If
                                ' If context is Quarter
                            ElseIf (UCase(Me.Context) = "1Q" Or UCase(Me.Context) = "2Q" Or UCase(Me.Context) = "3Q" Or UCase(Me.Context) = "4Q") And pnQuarterOffLine > 0 Then
                                oWhere.AppendANDCondition("Context in ('1Q','2Q','3Q','4Q')")
                                oWhere.AppendANDCondition("GameDate > " & SQLString(std.GetEasternDate()))
                                If pnQuarterOffLine <> 0 Then
                                    oWhere.AppendANDCondition(" DATEDIFF( minute," & SQLString(oToday) & ",GameDate)>=" & SafeString(pnQuarterOffLine))

                                End If
                            Else
                                'IF context is FullGame OR 1H
                                oWhere.AppendANDCondition("gl.Context <> '2H'  and g.GameDate > " & SQLString(oToday))
                                'If oSys IsNot Nothing AndAlso SafeInteger(oSys.Value) <> 0 Then
                                'oWhere.AppendANDCondition(" isnull(DATEDIFF(minute," & _
                                'SQLString(oToday) & " , GameDate),0) > " & SafeInteger(oSys.Value).ToString())
                                'End If
                                If oSys IsNot Nothing AndAlso SafeInteger(oSys.SubCategory) <> 0 Then
                                    oWhere.AppendANDCondition(" isnull(DATEDIFF(minute," & _
                                 SQLString(oToday) & " , GameDate),0) < " & _
                                 (SafeInteger(oSys.SubCategory) * 60).ToString())
                                End If

                            End If
                        End If

                    End If
                End If

                Dim sSQL As String = _
                String.Format("SELECT TOP 1 gl.*, g.HomePitcher, g.AwayPitcher " & _
                      "FROM GameLines gl INNER JOIN Games g ON gl.GameID = g.GameID {0} ORDER BY gl.LastUpdated DESC", oWhere.SQL)

                _log.Debug("Check TicketBet. SQL: " & sSQL)

                Dim oTbl As DataTable = oDB.getDataTable(sSQL)
                If oTbl.Rows.Count > 0 Then
                    '' Check HomePitcher
                    If SafeString(HomePitcher) <> SafeString(oTbl.Rows(0)("HomePitcher")) Then
                        HomePitcher = SafeString(oTbl.Rows(0)("HomePitcher"))
                        oResult = ECheckStatus.Update
                    End If
                    '' Check AwayPitcher
                    If SafeString(AwayPitcher) <> SafeString(oTbl.Rows(0)("AwayPitcher")) Then
                        AwayPitcher = SafeString(oTbl.Rows(0)("AwayPitcher"))
                        oResult = ECheckStatus.Update
                    End If

                    '' Check BetType
                    Select Case UCase(BetType)
                        Case "MONEYLINE"
                            If IsForProp Then
                                If PropMoneyLine <> SafeDouble(oTbl.Rows(0)("PropMoneyLine")) Then
                                    PropMoneyLine = SafeDouble(oTbl.Rows(0)("PropMoneyLine"))
                                    oResult = ECheckStatus.Update
                                End If
                            Else
                                If HomeMoneyLine <> 0 Then
                                    'If poOddsRules.IsLockGame(GameID, "Home", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If HomeMoneyLine <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeMoneyLine")), IsFavorite, "", pnIncreaseSpread) Then
                                        HomeMoneyLine = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeMoneyLine")), IsFavorite, "", pnIncreaseSpread)
                                        oResult = ECheckStatus.Update
                                    End If
                                End If

                                If AwayMoneyLine <> 0 Then
                                    'If poOddsRules.IsLockGame(GameID, "Away", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If AwayMoneyLine <> poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayMoneyLine")), IsFavorite, "", pnIncreaseSpread) Then
                                        AwayMoneyLine = poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayMoneyLine")), IsFavorite, "", pnIncreaseSpread)
                                        oResult = ECheckStatus.Update
                                    End If
                                End If
                            End If

                        Case "SPREAD"
                            'LogDebug(_log, HomeSpread & "::" & oTbl.Rows(0)("HomeSpread").ToString() & " truoc + Me.AddPoint :" & Me.AddPoint & " : Me.AddPointMoney " & Me.AddPointMoney)
                            If HomeSpread <> 0 AndAlso HomeSpread <> (SafeDouble(oTbl.Rows(0)("HomeSpread")) + Me.AddPointValid) Then
                                HomeSpread = (SafeDouble(oTbl.Rows(0)("HomeSpread")) + Me.AddPointValid)
                                oResult = ECheckStatus.Update
                            End If
                            If HomeSpreadMoney <> 0 Then
                                'If poOddsRules.IsLockGame(GameID, "Home", Context, BetType, Me, GameLineID) Then
                                '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                ' LogDebug(_log, "truoc1 + Me.AddPoint :" & Me.AddPoint & " : Me.AddPointMoney " & Me.AddPointMoney)
                                If HomeSpreadMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeSpreadMoney")), IsFavorite, "", pnIncreaseSpread, Me.AddPoint > 0) + AddPointMoneyValid Then
                                    HomeSpreadMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeSpreadMoney")), IsFavorite, "", pnIncreaseSpread, Me.AddPoint > 0) + AddPointMoneyValid
                                    AddPoint = 0
                                    AddPointMoney = 0
                                    'LogDebug(_log, "truoc2 + Me.AddPoint :" & Me.AddPoint & " : Me.AddPointMoney " & Me.AddPointMoney)
                                    oResult = ECheckStatus.Update
                                End If
                            End If

                            If AwaySpread <> 0 AndAlso AwaySpread <> SafeDouble(oTbl.Rows(0)("AwaySpread")) + Me.AddPointValid Then
                                AwaySpread = SafeDouble(oTbl.Rows(0)("AwaySpread")) + Me.AddPoint
                                oResult = ECheckStatus.Update
                            End If
                            If AwaySpreadMoney <> 0 Then
                                'If poOddsRules.IsLockGame(GameID, "Away", Context, BetType, Me, GameLineID) Then
                                '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                If AwaySpreadMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwaySpreadMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid Then
                                    AwaySpreadMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwaySpreadMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid
                                    AddPoint = 0
                                    AddPointMoney = 0
                                    oResult = ECheckStatus.Update
                                End If
                            End If

                        Case "TOTALPOINTS"
                            If TotalPoints <> SafeDouble(oTbl.Rows(0)("TotalPoints")) + Me.AddPointValid Then
                                TotalPoints = SafeDouble(oTbl.Rows(0)("TotalPoints")) + Me.AddPointValid
                                oResult = ECheckStatus.Update
                            End If

                            If TotalPointsUnderMoney <> 0 Then
                                'If poOddsRules.IsLockGame(GameID, "Home", Context, BetType, Me, GameLineID) Then
                                '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                If TotalPointsUnderMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("TotalPointsUnderMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid Then
                                    TotalPointsUnderMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("TotalPointsUnderMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid
                                    AddPoint = 0
                                    AddPointMoney = 0
                                    oResult = ECheckStatus.Update
                                End If
                            End If

                            If TotalPointsOverMoney <> 0 AndAlso TotalPointsOverMoney <> SafeDouble(oTbl.Rows(0)("TotalPointsOverMoney")) + Me.AddPointMoneyValid Then
                                'If poOddsRules.IsLockGame(GameID, "Away", Context, BetType, Me, GameLineID) Then
                                '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                If TotalPointsOverMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("TotalPointsOverMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid Then
                                    TotalPointsOverMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("TotalPointsOverMoney")), IsFavorite, "", pnIncreaseSpread) + Me.AddPointMoneyValid
                                    AddPoint = 0
                                    AddPointMoney = 0
                                    oResult = ECheckStatus.Update
                                End If
                            End If

                        Case "TEAMTOTALPOINTS"
                            If TeamTotalName.Equals("away", StringComparison.CurrentCultureIgnoreCase) Then
                                If TotalPoints <> SafeDouble(oTbl.Rows(0)("AwayTeamTotalPoints")) Then
                                    TotalPoints = SafeDouble(oTbl.Rows(0)("AwayTeamTotalPoints"))
                                    oResult = ECheckStatus.Update
                                End If

                                ''check for away under/over money
                                If TotalPointsUnderMoney <> 0 Then
                                    'If poOddsRules.IsLockGame(GameID, "home", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If TotalPointsUnderMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayTeamTotalPointsUnderMoney")), IsFavorite, "away", pnIncreaseSpread) Then
                                        TotalPointsUnderMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayTeamTotalPointsUnderMoney")), IsFavorite, "away", pnIncreaseSpread)
                                        AddPoint = 0
                                        AddPointMoney = 0
                                        oResult = ECheckStatus.Update
                                    End If
                                End If

                                If TotalPointsOverMoney <> 0 AndAlso TotalPointsOverMoney <> SafeDouble(oTbl.Rows(0)("TotalPointsOverMoney")) + Me.AddPointMoney Then
                                    'If poOddsRules.IsLockGame(GameID, "Away", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If TotalPointsOverMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayTeamTotalPointsOverMoney")), IsFavorite, "away", pnIncreaseSpread) Then
                                        TotalPointsOverMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("AwayTeamTotalPointsOverMoney")), IsFavorite, "away", pnIncreaseSpread)
                                        AddPoint = 0
                                        AddPointMoney = 0
                                        oResult = ECheckStatus.Update
                                    End If
                                End If


                            Else
                                If TotalPoints <> SafeDouble(oTbl.Rows(0)("HomeTeamTotalPoints")) Then
                                    TotalPoints = SafeDouble(oTbl.Rows(0)("HomeTeamTotalPoints"))
                                    oResult = ECheckStatus.Update
                                End If

                                ''check for home under/over money
                                If TotalPointsUnderMoney <> 0 Then
                                    'If poOddsRules.IsLockGame(GameID, "home", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If TotalPointsUnderMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeTeamTotalPointsUnderMoney")), IsFavorite, "home", pnIncreaseSpread) Then
                                        TotalPointsUnderMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeTeamTotalPointsUnderMoney")), IsFavorite, "home", pnIncreaseSpread)
                                        AddPoint = 0
                                        AddPointMoney = 0
                                        oResult = ECheckStatus.Update
                                    End If
                                End If

                                If TotalPointsOverMoney <> 0 AndAlso TotalPointsOverMoney <> SafeDouble(oTbl.Rows(0)("TotalPointsOverMoney")) Then
                                    'If poOddsRules.IsLockGame(GameID, "Away", Context, BetType, Me, GameLineID) Then
                                    '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                                    If TotalPointsOverMoney <> poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeTeamTotalPointsOverMoney")), IsFavorite, "home", pnIncreaseSpread) Then
                                        TotalPointsOverMoney = poOddsRules.GetMoneyLine(GameID, GameType, "Away", Context, BetType, SafeDouble(oTbl.Rows(0)("HomeTeamTotalPointsOverMoney")), IsFavorite, "home", pnIncreaseSpread)
                                        AddPoint = 0
                                        AddPointMoney = 0
                                        oResult = ECheckStatus.Update
                                    End If
                                End If

                            End If

                        Case "DRAW"
                            'If poOddsRules.IsLockGame(GameID, "Home", Context, BetType, Me, GameLineID) Then
                            '    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                            If DrawMoneyLine <> poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("DrawMoneyLine")), IsFavorite, "", pnIncreaseSpread) Then
                                DrawMoneyLine = poOddsRules.GetMoneyLine(GameID, GameType, "Home", Context, BetType, SafeDouble(oTbl.Rows(0)("DrawMoneyLine")), IsFavorite, "", pnIncreaseSpread)
                                oResult = ECheckStatus.Update
                            End If
                    End Select
                Else
                    Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
                End If
            Catch exTicket As CTicketException
                Throw New CTicketException(exTicket.Message)

            Catch ex As Exception
                _log.Error("Fail to load lastest GameLine. Message: " & ex.Message, ex)
                Throw New CTicketException(String.Format("This {0} bet has been locked.", Team))
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            _log.Debug("END check valid TicketBetID: " & TicketBetID)
            Return oResult
        End Function

        Public Function ParseBuyPointOptions() As List(Of DictionaryEntry)
            Dim olstDic As New List(Of DictionaryEntry)

            '' Only Football and Basketball can buy points
            '' Allow buy point when juice is between -110 and -100
            '' First character of TeamNumber = 1: 1st Half | 3: 2nd Half
            Dim bBuyPoint As Boolean = UCase(Me.BetType) <> "MONEYLINE" AndAlso _
            UCase(Context) = "CURRENT" AndAlso BetPoint >= -110 AndAlso BetPoint <= -100 _
            AndAlso (IsFootball(Me.GameType) OrElse IsBasketball(Me.GameType))

            If bBuyPoint Then
                Dim sPrefix As String = ""
                If (UCase(Me.BetType) = "TOTALPOINTS" Or UCase(Me.BetType) = "TEAMTOTALPOINTS") Then
                    If Me.TotalPointsOverMoney <> 0 Then
                        sPrefix = "Over "
                    Else
                        sPrefix = "Under "
                    End If
                End If

                ''Single Parlay
                Dim oDicItem As New DictionaryEntry(sPrefix & SafeString(Juice()) & " (" & SafeString(BetPoint) & ")", "")
                olstDic.Add(oDicItem)

                If UCase(GameType).Contains("NFL FOOTBALL") OrElse UCase(GameType).Contains("NCAA FOOTBALL") Then
                    '' OFF 3: From +3 to +3 1/2 | From -3 to -2 1/2
                    '' ON 3: From +2 1/2 to +3 | From -3 1/2 to -3
                    If Juice() = 3 OrElse Juice() = -3 OrElse Juice() = 2.5 OrElse Juice() = -3.5 Then
                        oDicItem = New DictionaryEntry(SafeString(Juice() + 0.5) & " (" & SafeString(BetPoint - 10) & ")", "0.5|-10")
                        olstDic.Add(oDicItem)
                    End If

                End If

                '' Buy from 1/2 to 1 1/2 points
                If olstDic.Count = 1 Then
                    Dim nAddpoint As Double
                    For nPoint As Double = 0.5 To 1.5 Step 0.5
                        nAddpoint = nPoint

                        If (UCase(Me.BetType) = "TOTALPOINTS" Or UCase(Me.BetType) = "TEAMTOTALPOINTS") AndAlso Me.TotalPointsOverMoney <> 0 Then
                            nAddpoint = -nPoint
                        End If

                        oDicItem = New DictionaryEntry(sPrefix & SafeString(Juice() + nAddpoint) & " (" & SafeString(BetPoint + (-nPoint * 20)) & ")", _
                                                       SafeString(nAddpoint) & "|" & SafeString(-nPoint * 20))
                        olstDic.Add(oDicItem)
                    Next
                End If
            End If

            Return olstDic
        End Function

        Private Function Juice() As Double
            Select Case UCase(Me.BetType)
                Case "SPREAD"
                    If Me.HomeSpreadMoney <> 0 Then
                        Return Me.HomeSpread
                    Else
                        Return Me.AwaySpread
                    End If

                Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                    Return Me.TotalPoints

                Case Else
                    Return 0
            End Select
        End Function
    End Class

    <Serializable()> _
    Public Class CTicketBetList
        Inherits List(Of CTicketBet)

        Public Function GetTicketBetByID(ByVal psTicketBetID As String) As CTicketBet
            Return Me.Find(Function(x) x.TicketBetID.ToUpper() = psTicketBetID.ToUpper())

        End Function

        Public Function RemoveByID(ByVal psTicketBetID As String) As CTicketBet
            Dim poTicketBet As CTicketBet = GetTicketBetByID(psTicketBetID)

            If poTicketBet IsNot Nothing Then
                Me.Remove(poTicketBet)
            End If

            Return poTicketBet
        End Function

    End Class
End Namespace
