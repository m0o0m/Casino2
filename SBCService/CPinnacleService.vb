Imports System.Xml
Imports System.Data
Imports SBCBL.std
Imports SBCService.CServiceStd

Public Class CPinnacleService
#Region "Private Variables"

    Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
#End Region

    Public Function ExecutePinnacleFeed() As Boolean
        _log.Info("START REQUEST PINNACLE PROPS XML.")
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
        Try
            Dim sXML As String = SBCBL.std.webGet(PINNACLE_FEED_URL)
            Dim oDoc As New XmlDocument()
            oDoc.LoadXml(sXML)
            'event[starts-with(league,'M')]
            Dim oListEvents As XmlNodeList = oDoc.SelectNodes("pinnacle_line_feed/events/event")
            Dim sGameType As String = ""

            For Each oEvent As XmlElement In oListEvents
                '''''''''''''''''addd soccer start'''''''''''''''''
                Dim oListParticipants As XmlNodeList = oEvent.SelectNodes("participants/participant")
                If SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Turkey SL", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Greek", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Portuguese", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Scot Premier", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Belgian", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("EuroCh Q W", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("UEFA EURO", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Brazil Ser A", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Brazil Ser B", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Serie B", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Serie A", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("FIFA WCQ SAm", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Intl Friendl", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Argentine", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Argentina B", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Danish 1", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Norwegian 1", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("La Liga", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Segunda Liga", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("UEFA EURO", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("UEFA U21 Ch.", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("US Open Cup", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("USA (MLS)", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("AFC CL", StringComparison.CurrentCultureIgnoreCase) OrElse _
                                SafeString(oEvent.SelectSingleNode("league").InnerText).Equals("Australia", StringComparison.CurrentCultureIgnoreCase) Then
                    If oListParticipants.Count = 3 AndAlso SafeString(oEvent.SelectSingleNode("IsLive").InnerText).Equals("NO", StringComparison.CurrentCultureIgnoreCase) Then

                        Dim moneyline As XmlNode = oEvent.SelectSingleNode("periods/period/moneyline")
                        Dim spread As XmlNode = oEvent.SelectSingleNode("periods/period/spread")
                        Dim total As XmlNode = oEvent.SelectSingleNode("periods/period/total")

                        Dim oAwayParticipants As XmlNode = oListParticipants(1)
                        Dim oHomeParticipants As XmlNode = oListParticipants(0)
                        Dim oDrawParticipants As XmlNode = oListParticipants(2)

                        Dim psGameDate As String = oEvent.SelectSingleNode("event_datetimeGMT").InnerText
                        If Now.IsDaylightSavingTime() Then
                            psGameDate = SafeString(SafeDate(psGameDate).AddHours(1))
                        End If
                        Dim pnAwayRotationNumber As Integer = SBCBL.std.SafeInteger(oAwayParticipants.SelectSingleNode("rotnum").InnerText)
                        Dim pnHomeRotationNumber As Integer = SBCBL.std.SafeInteger(oHomeParticipants.SelectSingleNode("rotnum").InnerText)
                        Dim pnDrawnRotationNumber As Integer = SBCBL.std.SafeInteger(oDrawParticipants.SelectSingleNode("rotnum").InnerText)
                        Dim psAwayTeam As String = oAwayParticipants.SelectSingleNode("participant_name").InnerText
                        Dim psHomeTeam As String = oHomeParticipants.SelectSingleNode("participant_name").InnerText
                        Dim psGameType As String = oEvent.SelectSingleNode("league").InnerText.Replace("Turkey SL", "Turkey").Replace("Greek", "Greece").Replace("Portuguese", "Portugal").Replace("Scot Premier", "Scotland").Replace("Belgian", "Belgium").Replace("Israel Prem", "Israel").Replace("EuroCh Q W", "Euro Cups").Replace("UEFA EURO", "Euro Cups").Replace("Brazil Ser A", "Brazil").Replace("Brazil Ser B", "Brazil B").Replace("FIFA WCQ SAm", "World Cup").Replace("Intl Friendl", "Intl Friendly").Replace("J1 League", "Japan League 1").Replace("J2 League", "Japan League 2").Replace("Argentine", "Argentine").Replace("Argentina B", "Argentina B").Replace("Danish 1", "Denmark").Replace("Norwegian 1", "Norway").Replace("La Liga", "La Liga").Replace("Segunda Liga", "Segunda").Replace("Serie A", "Serie A").Replace("Serie B", "Serie B").Replace("UEFA U21 Ch.", "Euro Under 21").Replace("US Open Cup", "US Cup").Replace("USA (MLS)", "MLS").Replace("AFC CL", "Asian Cups")
                        Dim psAwaySpread As String = ""
                        Dim psAwaySpreadMoney As String = ""
                        Dim psHomeSpreadMoney As String = ""
                        Dim psTotalPoint As String = ""
                        Dim psTotalPointsOverMoney As String = ""
                        Dim psTotalPointsUnderMoney As String = ""
                        Dim psHomeMoney As String = ""
                        Dim psAwayMoneyLine As String = ""
                        Dim psDrawMoney As String = ""
                        If spread IsNot Nothing Then
                            psAwaySpread = safeValueSoccer(spread.SelectSingleNode("spread_visiting").InnerText)
                            psAwaySpreadMoney = spread.SelectSingleNode("spread_adjust_visiting").InnerText
                            psHomeSpreadMoney = spread.SelectSingleNode("spread_adjust_home").InnerText
                        End If
                        If total IsNot Nothing Then
                            psTotalPoint = safeValueSoccer(total.SelectSingleNode("total_points").InnerText)
                            psTotalPointsOverMoney = total.SelectSingleNode("over_adjust").InnerText
                            psTotalPointsUnderMoney = total.SelectSingleNode("under_adjust").InnerText
                        End If
                        If moneyline IsNot Nothing Then
                            psHomeMoney = moneyline.SelectSingleNode("moneyline_home").InnerText
                            psAwayMoneyLine = moneyline.SelectSingleNode("moneyline_visiting").InnerText
                            psDrawMoney = moneyline.SelectSingleNode("moneyline_draw").InnerText
                        End If
                        If Not UCase(psAwayTeam).Contains("AWAY TEAMS") Then
                            AddGame(SafeString(SafeDate(psGameDate).AddHours(-5)), pnAwayRotationNumber, pnHomeRotationNumber, pnDrawnRotationNumber, psAwayTeam, psHomeTeam, psGameType, _
                           psAwaySpread, psAwaySpreadMoney, psHomeSpreadMoney, psHomeMoney, psAwayMoneyLine, psTotalPoint, psTotalPointsOverMoney, psTotalPointsUnderMoney, psDrawMoney)
                        End If
                    End If
                End If
                '''''''''''''''''soccer end''''''''''''''''''''
                If Not IsSelectNode(oEvent, sGameType) Then
                    Continue For
                End If
                Dim oGame As New CGame()
                oGame.IsUpdateGameDate = True

                Dim bSaveGame As Boolean = True
                oGame.GameType = sGameType
                oGame.GameDate = SafeDate(oEvent.SelectSingleNode("event_datetimeGMT").InnerText)
                oGame.IsForProp = True
                oGame.PropDescription = SafeString(oEvent.SelectSingleNode("league").InnerText) & " - " & SafeString(oEvent.SelectSingleNode("description").InnerText)
                oGame.ExtPropNumber = SafeString(oEvent.SelectSingleNode("gamenumber").InnerText)

                '' save Teams to game lines
                ' Dim oListParticipants As XmlNodeList = oEvent.SelectNodes("participants/participant")

                For Each oParticipants As XmlElement In oListParticipants
                    If oParticipants.SelectSingleNode("odds/moneyline_value") Is Nothing Then
                        bSaveGame = False
                        Continue For
                    End If
                    Dim sPropRotationNumber As String = oParticipants.SelectSingleNode("rotnum").InnerText
                    Dim oGameLine As CLine = oGame.GetPropLine("sportoption", "", sPropRotationNumber)
                    oGameLine.Context = "Proposition"
                    oGameLine.PropParticipantName = oParticipants.SelectSingleNode("participant_name").InnerText

                    oGameLine = oGame.GetPropLine("sportoption", "Pinnacle", sPropRotationNumber)
                    oGameLine.PropParticipantName = oParticipants.SelectSingleNode("participant_name").InnerText
                    oGameLine.Context = "Proposition"
                    oGameLine.PropMoneyLine = SafeDouble(oParticipants.SelectSingleNode("odds/moneyline_value").InnerText)
                Next

                '' cancel to save this game
                If Not bSaveGame Then
                    Continue For
                End If

                oGame.SavePropToDB(oDB)
                oGame.SavePropLinesToDB(oDB)
            Next

        Catch ex As Exception

            _log.Error("Cant save Pinnacle Props: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
    End Function

    Public Function IsSelectNode(ByVal poNode As XmlElement, ByRef psGameType As String) As Boolean
        Dim bIsMultiTeam As Boolean = False
        Dim bValidGame As Boolean = False
        If poNode.SelectSingleNode("sporttype").InnerText.EndsWith("Props") AndAlso poNode.SelectSingleNode("participants").ChildNodes.Count > 2 Then
            bIsMultiTeam = True
        End If
        If poNode.SelectSingleNode("sporttype").InnerText.EndsWith("Futures") AndAlso poNode.SelectSingleNode("participants").ChildNodes.Count > 2 Then
            bIsMultiTeam = True
        End If

        Select Case True
            Case poNode.SelectSingleNode("sporttype").InnerText.Contains("Football") Or poNode.SelectSingleNode("sporttype").InnerText.Contains("NFL")
                psGameType = "Football"
                bValidGame = True
            Case poNode.SelectSingleNode("sporttype").InnerText.Contains("Baseball")
                psGameType = "Baseball"
                bValidGame = True
            Case poNode.SelectSingleNode("sporttype").InnerText.Contains("WNBA") Or poNode.SelectSingleNode("sporttype").InnerText.Contains("NBA") Or _
                 poNode.SelectSingleNode("sporttype").InnerText.Contains("NCAA Basketball") Or poNode.SelectSingleNode("sporttype").InnerText.Contains("WNCAA Basketball")
                psGameType = "Basketball"
                bValidGame = True
            Case poNode.SelectSingleNode("sporttype").InnerText.Contains("Hockey")
                psGameType = "Hockey"
                bValidGame = True
            Case poNode.SelectSingleNode("sporttype").InnerText.Contains("Soccer")
                psGameType = "Soccer"
                bValidGame = True
        End Select
        Return bIsMultiTeam And bValidGame
    End Function
End Class
