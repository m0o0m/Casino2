Imports WebsiteLibrary.DBUtils
Imports System.Xml
Imports SBCBL.std
Imports System.Text
Imports System.Data
Imports SBCService.CServiceStd
Imports System.Xml.XPath

Public Class CSportsInsightsService
#Region "Sportinside XML service"

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Public Sub execSportsInsights()
        Try
            'LogDebug(log, "New thread spawned for SPORTSINSIGHT")

            If (SPORTSINSIGHT_EXECUTION_START > -1) Then
                'service timer already spawned a thread to collect sportsinsight data and 
                'it still hasn't completed, so check to make sure it's not taking too long
                Dim nTimeExecuting As Double = Timer - SPORTSINSIGHT_EXECUTION_START
                If nTimeExecuting > SPORTSINSIGHTS_MAX_TIMEOUT Then
                    Throw New TimeoutException("SPORTSINSIGHTS Exceeded Max Timeout: " & nTimeExecuting)
                End If

                'sports insight already running, just return
                'LogDebug(log, "Thread already exists for SPORTSINSIGHT for the past: " & nTimeExecuting & " seconds")
                Return
            End If

            SPORTSINSIGHT_EXECUTION_START = Timer 'this thread is now master processing thread for SPORTSINSIGHT
            Dim oStatedTime As DateTime = Now
            If CServiceStd.EXECUTE_SPORTS_INSIGHT_LINE Then

                processSportsInsightLine()
                log.Info("LINEFEED Executed time: " & Now.Subtract(oStatedTime).TotalMilliseconds & " ms")
            End If

            oStatedTime = Now
            If CServiceStd.EXECUTE_SPORTS_INSIGHT_SCORE Then
                processSportsInsightScores()
                log.Info("SCORESFEED Executed time: " & Now.Subtract(oStatedTime).TotalMilliseconds & " ms")
            End If

        Catch err As Exception
            CatchSBCServiceError(log, err)
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        SPORTSINSIGHT_EXECUTION_START = -1
    End Sub

    Private Sub processSportsInsightLine()
        If SPORTSINSIGHTS_LINEFEED_LAST_TIME = 0 Or Timer - SPORTSINSIGHTS_LINEFEED_FULL_REFRESH_TIME > SPORTSINSIGHTS_FULL_REFRESH_SECS Then
            log.Info("FULL REFRESH ON SPORTS INSIGHT!!!")
            SPORTSINSIGHTS_LINEFEED_LAST_TIME = 0
            SPORTSINSIGHTS_LINEFEED_FULL_REFRESH_TIME = Timer
        End If

        '----- BEGIN CALL TO WEB SERVICE -----'
        LogDebug(log, "Exec SPORTSINSIGHT LINEFEED with last time: " & SPORTSINSIGHTS_LINEFEED_LAST_TIME.ToString)

        Dim oService As New Bin.com.sportsinsights.www.DataService()
        oService.Url = SPORTSINSIGHTS_URL
        Dim oRoot As XmlNode = oService.LineFeed(SPORTSINSIGHTS_LINEFEED_LAST_TIME, SPORTSINSIGHTS_KEY)

        LogInfo(log, "SPORTSINSIGHT LINEFEED result: " & oRoot.OuterXml)
        '----- END CALL TO WEB SERVICE -----'


        Dim oSportEvents As XmlNodeList = oRoot.SelectNodes("sport-event")
        Dim nNumGames As Integer = oSportEvents.Count
        Dim nLastTime As Double = SafeDouble(CType(oRoot.SelectSingleNode("sports-content"), XmlElement).GetAttribute("doc-time"))

        LogInfo(log, "SPORTSINSIGHT LINEFEED Returned " & nNumGames & " TOTAL GAME EVENTS")

        'xml is no different from the last xml we already processed, just quit
        If nLastTime = SPORTSINSIGHTS_LINEFEED_LAST_TIME Or nNumGames = 0 Then
            Return
        End If

        Dim nCounter As Integer = 1
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        Try
            For Each oSportEvent As XmlElement In oSportEvents
                Dim oEventData As XmlElement = CType(oSportEvent.SelectSingleNode("event-metadata"), XmlElement)
                Dim oGame As New CGame()
                oGame.GameDate = CDate(oEventData.GetAttribute("event-date-time"))
                oGame.GameType = oEventData.GetAttribute("league")
                oGame.GameStatus = oEventData.GetAttribute("event-status")
                oGame.LastUpdated = Date.Now

                For Each oTeam As XmlElement In oSportEvent.SelectNodes("team")
                    Select Case UCase(CType(oTeam.SelectSingleNode("team-metadata"), XmlElement).GetAttribute("alignment"))
                        Case "HOME"
                            processSportsInsightHomeTeam(oTeam, oGame)
                        Case "AWAY"
                            processSportsInsightAwayTeam(oTeam, oGame)
                    End Select
                Next

                oGame.SaveToDB(oDB, CGame.FindTeamBy.NSS)
                oGame.SaveLinesToDB(oDB, SafeString(oGame.GameID), oGame.GameType)
            Next

            log.Debug("Update LineFeed. Total " & oSportEvents.Count.ToString() & " games")
        Finally
            oDB.closeConnection()
        End Try

        'set SPORTSINSIGHTS_LAST_TIME so we can call sportsinsight with cached data result next time
        SPORTSINSIGHTS_LINEFEED_LAST_TIME = Math.Max(nLastTime, SPORTSINSIGHTS_LINEFEED_LAST_TIME) 'sometimes xml returns doctime of 0??

    End Sub

    Private Sub processSportsInsightScores()
        If SPORTSINSIGHTS_SCOREFEED_LAST_TIME = 0 Or Timer - SPORTSINSIGHTS_SCOREFEED_FULL_REFRESH_TIME > SPORTSINSIGHTS_FULL_REFRESH_SECS Then
            log.Info("FULL REFRESH ON SPORTS INSIGHT SCOREFEED!!!")
            SPORTSINSIGHTS_SCOREFEED_LAST_TIME = 0
            SPORTSINSIGHTS_SCOREFEED_FULL_REFRESH_TIME = Timer
        End If
        '----- BEGIN CALL TO WEB SERVICE -----'
        LogDebug(log, "Exec SPORTSINSIGHT SCOREFEED with last time: " & SPORTSINSIGHTS_SCOREFEED_LAST_TIME.ToString)

        Dim oService As New Bin.com.sportsinsights.www.DataService()
        oService.Url = SPORTSINSIGHTS_URL
        Dim oRoot As XmlNode = oService.ScoreFeed(SPORTSINSIGHTS_SCOREFEED_LAST_TIME, SPORTSINSIGHTS_KEY)

        LogInfo(log, "SPORTSINSIGHT SCOREFEED result: " & oRoot.OuterXml)
        '----- END CALL TO WEB SERVICE -----'

        'DOUBLE CHECK THIS SECTION AND FIX TO WORK WITH SCORE FEED        

        Dim nCounter As Integer = 1
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
        oDB.openConnection()
        Try
            '' retrive data from DB first
            Dim sSQL As String = "select * from Games where gamestatus <> 'final' and (GameDate > " & SQLString(Now.AddDays(-1).ToShortDateString()) & _
                                " and GameDate < " & SQLString(Now.AddDays(2).ToShortDateString()) & ")"
            log.Debug("Get Scorefeed SQL: " & sSQL)
            Dim oDTSource As DataTable = oDB.getDataTable(sSQL)

            Dim nNumGames As Integer = oDTSource.Rows.Count
            LogInfo(log, "SPORTSINSIGHTS SCOREFEED Returned " & nNumGames & " TOTAL GAME EVENTS")

            Dim sHomeRotationNumber As String = ""
            Dim sAwayRotationNumber As String = ""

            For Each oRow As DataRow In oDTSource.Rows
                If CDate(oRow("GameDate")).Hour = 3 Then
                    log.Info("Alert for gamedate: " & SafeString(oRow("GameDate")) & "ID : " & SafeString(oRow("GAMEID")))
                End If
                SingleScoreGameProcess(oRow, oRoot)
            Next
            UpdateTableScore(oDTSource)
        Catch ex As Exception
            log.Error("Error in Update this game : " & ex.Message, ex)
            oDB.rollbackTransaction()
            'Throw ex
        Finally
            oDB.closeConnection()
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="poRow"></param>
    ''' <param name="poRoot"></param>
    ''' <remarks></remarks>
    Private Sub SingleScoreGameProcess(ByVal poRow As DataRow, ByVal poRoot As XmlNode)
        Dim sHomeRotationNumber As String = ""
        Dim sAwayRotationNumber As String = ""
        Dim bFoundGame As Boolean = True

        Try
            'DOUBLE CHECK THIS SECTION AND FIX TO WORK WITH SCORE FEED                    
            Dim oSportEvent As XmlElement = Nothing
            Dim oSportEvents As XmlNodeList = poRoot.SelectNodes("sports-metadata/sports-event")
            Dim oGameDate As DateTime = CDate(poRow("GameDate"))

            Dim sXPath As String = "sports-metadata/sports-event[event-metadata/@league='" & SafeString(poRow("GameType")) & _
                             "' and event-metadata/@event-date-time='" & CDate(poRow("GameDate")).ToString("MM/dd/yyyy hh:mm tt") & _
                             "']/team/team-metadata[@alignment='Away' and @nss=" & SQLString(SafeString(poRow("AwayRotationNumber"))) & "]"

            oSportEvent = CType(poRoot.SelectSingleNode(sXPath), XmlElement)
            log.Debug("XPATH return: " & sXPath)
            log.Debug("GAMEDATE return: " & SafeString(poRow("GameDate")))

            '' if can't find by xpath, maybe gamedate is changed!, try again by scan all nodes
            If oSportEvent Is Nothing Then
                For Each oNode As XmlNode In oSportEvents
                    Dim oEvenDate As DateTime = SafeDate(CType(oNode.SelectSingleNode("event-metadata"), XmlElement).GetAttribute("event-date-time"))
                    If oEvenDate < Now.AddDays(-2) Or oEvenDate > Now.AddDays(2) Then
                        Continue For
                    End If

                    If oEvenDate > oGameDate.AddHours(-18) And oEvenDate < oGameDate.AddHours(18) And _
                                    CType(oNode.SelectNodes("team/team-metadata")(0), XmlElement).GetAttribute("nss") = SafeString(poRow("AwayRotationNumber")) Then
                        oSportEvent = CType(oNode, XmlElement)
                        Exit For
                    End If

                    If oEvenDate > oGameDate.AddHours(-18) And oEvenDate < oGameDate.AddHours(18) And _
                        CType(oNode.SelectNodes("team/team-metadata")(0), XmlElement).GetAttribute("nss") = SafeString(poRow("AwayRotationNumber")) Then
                        oSportEvent = CType(oNode, XmlElement)
                        Exit For
                    End If
                Next
            Else
                '' becarefull, if find by XPath sucessfull, we have to revert to 2 level up 
                oSportEvent = CType(oSportEvent.ParentNode.ParentNode, XmlElement)
            End If

            If oSportEvent Is Nothing Then
                log.Error("Can't find SportEvent: away-nss: " & SafeString(poRow("AwayRotationNumber")) & " ---- Game type: " & SafeString(poRow("Gametype")) & " ---- Game date: " & SafeString(poRow("GameDate")))
                bFoundGame = False
            End If

            '' process data if found game
            If bFoundGame Then
                For Each oTeam As XmlElement In oSportEvent.SelectNodes("team")
                    Select Case UCase(CType(oTeam.SelectSingleNode("team-metadata"), XmlElement).GetAttribute("alignment"))
                        Case "HOME"
                            Dim oMetaData As XmlElement = CType(oTeam.SelectSingleNode("team-metadata"), XmlElement)
                            sHomeRotationNumber = SafeString(oMetaData.GetAttribute("nss"))
                        Case "AWAY"
                            Dim oMetaData As XmlElement = CType(oTeam.SelectSingleNode("team-metadata"), XmlElement)
                            sAwayRotationNumber = SafeString(oMetaData.GetAttribute("nss"))
                    End Select
                Next

                Dim oEventData As XmlElement = CType(oSportEvent.SelectSingleNode("event-metadata"), XmlElement)
                'oRow("GameDate") = CDate(oEventData.GetAttribute("event-date-time"))
                poRow("GameType") = oEventData.GetAttribute("league")
                poRow("GameStatus") = oEventData.GetAttribute("event-status")
                poRow("LastUpdated") = Date.Now

                For Each oTeam As XmlElement In oSportEvent.SelectNodes("team")
                    Select Case UCase(CType(oTeam.SelectSingleNode("team-metadata"), XmlElement).GetAttribute("alignment"))
                        Case "HOME"
                            processSportsInsightHomeTeam(oTeam, poRow)
                        Case "AWAY"
                            processSportsInsightAwayTeam(oTeam, poRow)
                    End Select
                Next

            End If

        Catch ex As Exception
            log.Error("Error in SingleScoreGameProcess: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub UpdateTableScore(ByVal poDTSource As DataTable)
        '' no data to update
        log.Debug("Update scorefeed Data Table")
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
        oDB.openConnection()
        If poDTSource.Rows.Count = 0 Then
            log.Info("No scorefeed updated!")
            Return
        End If

        Try
            oDB.UpdateDataTable(poDTSource, "SELECT TOP 0 * FROM Games", True)
            log.Debug("Update data: there is " & poDTSource.Rows.Count.ToString() & " row ")
            oDB.commitTransaction()
        Catch ex As Exception
            log.Error("Error in Update this game : " & ex.Message, ex)
            oDB.rollbackTransaction()
            ' Throw ex
        Finally
            oDB.closeConnection()
        End Try

    End Sub

    Private Sub processSportsInsightHomeTeam(ByVal poTeam As XmlElement, ByVal poRowGame As DataRow, Optional ByVal poDTGameLine As DataTable = Nothing)
        Dim oMetaData As XmlElement = CType(poTeam.SelectSingleNode("team-metadata"), XmlElement)
        poRowGame("HomeRotationNumber") = SafeInteger(oMetaData.GetAttribute("nss"))
        poRowGame("HomeTeam") = CType(oMetaData.SelectSingleNode("name"), XmlElement).GetAttribute("full")
        poRowGame("HomeScore") = SafeInteger(CType(poTeam.SelectSingleNode("team-stats"), XmlElement).GetAttribute("score"))

        ''update gameline in case this is LINEFEED
        If poDTGameLine IsNot Nothing Then
            'straight spread
            For Each oStraightSpread As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-straight-spread")
                Dim sBookmaker As String = oStraightSpread.GetAttribute("bookmaker-name")
                Dim sContext As String = oStraightSpread.GetAttribute("context")

                Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)

                oLine("HomeSpread") = SafeDouble(oStraightSpread.GetAttribute("line"))
                oLine("HomeSpreadMoney") = SafeDouble(oStraightSpread.GetAttribute("money"))
            Next

            'moneyline
            For Each oMoneyLine As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-moneyline")
                Dim sBookmaker As String = oMoneyLine.GetAttribute("bookmaker-name")
                Dim sContext As String = oMoneyLine.GetAttribute("context")

                Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)
                oLine("HomeMoneyLine") = SafeDouble(oMoneyLine.GetAttribute("line"))
            Next

            'total points
            'IMPORTANT: HOME TEAM total point money = odds for the OVER
            For Each oTotal As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-total")
                Dim sBookmaker As String = oTotal.GetAttribute("bookmaker-name")
                Dim sContext As String = oTotal.GetAttribute("context")

                Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)
                oLine("TotalPoints") = SafeDouble(oTotal.GetAttribute("line"))
                oLine("TotalPointsOverMoney") = SafeDouble(oTotal.GetAttribute("money"))
            Next
        End If
    End Sub

    Private Sub processSportsInsightAwayTeam(ByVal poTeam As XmlElement, ByVal poRowGame As DataRow, Optional ByVal poDTGameLine As DataTable = Nothing)
        Dim oMetaData As XmlElement = CType(poTeam.SelectSingleNode("team-metadata"), XmlElement)
        poRowGame("AwayRotationNumber") = SafeInteger(oMetaData.GetAttribute("nss"))
        poRowGame("AwayTeam") = CType(oMetaData.SelectSingleNode("name"), XmlElement).GetAttribute("full")
        poRowGame("AwayScore") = SafeInteger(CType(poTeam.SelectSingleNode("team-stats"), XmlElement).GetAttribute("score"))

        'straight spread
        For Each oStraightSpread As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-straight-spread")
            Dim sBookmaker As String = oStraightSpread.GetAttribute("bookmaker-name")
            Dim sContext As String = oStraightSpread.GetAttribute("context")

            Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)
            oLine("AwaySpread") = SafeDouble(oStraightSpread.GetAttribute("line"))
            oLine("AwaySpreadMoney") = SafeDouble(oStraightSpread.GetAttribute("money"))
        Next

        'moneyline
        For Each oMoneyLine As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-moneyline")
            Dim sBookmaker As String = oMoneyLine.GetAttribute("bookmaker-name")
            Dim sContext As String = oMoneyLine.GetAttribute("context")

            Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)
            oLine("AwayMoneyLine") = SafeDouble(oMoneyLine.GetAttribute("line"))
        Next

        'total points
        'IMPORTANT: AWAY TEAM total point money = odds for the UNDER
        For Each oTotal As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-total")
            Dim sBookmaker As String = oTotal.GetAttribute("bookmaker-name")
            Dim sContext As String = oTotal.GetAttribute("context")

            Dim oLine As DataRow = GetLine(poDTGameLine, SafeString(poRowGame("GameID")), "SPORTSINSIGHTS", sBookmaker, sContext)
            'oLine.TotalPoints = SafeDouble(oTotal.GetAttribute("line"))
            oLine("TotalPointsUnderMoney") = SafeDouble(oTotal.GetAttribute("money"))
        Next
    End Sub

    Private Sub processSportsInsightHomeTeam(ByVal poTeam As XmlElement, ByVal poGame As CGame)
        Dim oMetaData As XmlElement = CType(poTeam.SelectSingleNode("team-metadata"), XmlElement)
        poGame.HomeTeam.RotationNumber = SafeInteger(oMetaData.GetAttribute("nss"))
        poGame.HomeTeam.Team = CType(oMetaData.SelectSingleNode("name"), XmlElement).GetAttribute("full")

        'straight spread
        For Each oStraightSpread As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-straight-spread")
            Dim sBookmaker As String = oStraightSpread.GetAttribute("bookmaker-name")
            Dim sContext As String = oStraightSpread.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.HomeSpread = SafeDouble(oStraightSpread.GetAttribute("line"))
            oLine.HomeSpreadMoney = SafeDouble(oStraightSpread.GetAttribute("money"))
        Next

        'moneyline
        For Each oMoneyLine As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-moneyline")
            Dim sBookmaker As String = oMoneyLine.GetAttribute("bookmaker-name")
            Dim sContext As String = oMoneyLine.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.HomeMoneyLine = SafeDouble(oMoneyLine.GetAttribute("line"))
        Next

        'total points
        'IMPORTANT: HOME TEAM total point money = odds for the OVER
        For Each oTotal As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-total")
            Dim sBookmaker As String = oTotal.GetAttribute("bookmaker-name")
            Dim sContext As String = oTotal.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.TotalPoints = SafeDouble(oTotal.GetAttribute("line"))
            oLine.TotalPointsOverMoney = SafeDouble(oTotal.GetAttribute("money"))
        Next

        '' adding game context
        'If (SafeString()) Then

    End Sub

    Private Sub processSportsInsightAwayTeam(ByVal poTeam As XmlElement, ByVal poGame As CGame)
        Dim oMetaData As XmlElement = CType(poTeam.SelectSingleNode("team-metadata"), XmlElement)
        poGame.AwayTeam.RotationNumber = SafeInteger(oMetaData.GetAttribute("nss"))
        poGame.AwayTeam.Team = CType(oMetaData.SelectSingleNode("name"), XmlElement).GetAttribute("full")

        'straight spread
        For Each oStraightSpread As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-straight-spread")
            Dim sBookmaker As String = oStraightSpread.GetAttribute("bookmaker-name")
            Dim sContext As String = oStraightSpread.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.AwaySpread = SafeDouble(oStraightSpread.GetAttribute("line"))
            oLine.AwaySpreadMoney = SafeDouble(oStraightSpread.GetAttribute("money"))
        Next

        'moneyline
        For Each oMoneyLine As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-moneyline")
            Dim sBookmaker As String = oMoneyLine.GetAttribute("bookmaker-name")
            Dim sContext As String = oMoneyLine.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.AwayMoneyLine = SafeDouble(oMoneyLine.GetAttribute("line"))
        Next

        'total points
        'IMPORTANT: AWAY TEAM total point money = odds for the UNDER
        For Each oTotal As XmlElement In poTeam.SelectNodes("wagering-stats/wagering-total")
            Dim sBookmaker As String = oTotal.GetAttribute("bookmaker-name")
            Dim sContext As String = oTotal.GetAttribute("context")

            Dim oLine As CLine = poGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            'oLine.TotalPoints = SafeDouble(oTotal.GetAttribute("line"))
            oLine.TotalPointsUnderMoney = SafeDouble(oTotal.GetAttribute("money"))
        Next
    End Sub

    Public Function GetLine(ByVal poData As DataTable, ByVal psGameID As String, ByVal psFeedSource As String, ByVal psBookerName As String, ByVal psContext As String) As DataRow
        Dim sWhere As String = "GameID = {0} and FeedSource = {1} and  Bookmaker = {2} and  Context= {3}"
        sWhere = String.Format(sWhere, SQLString(psGameID), SQLString(psFeedSource), SQLString(psBookerName), SQLString(psContext))
        poData.DefaultView.RowFilter = sWhere

        If poData.DefaultView.ToTable.Rows.Count > 0 Then
            Return poData.DefaultView.ToTable.Rows(0)
        Else
            '' if this gameline doesn't appear, create new row
            Dim oLine As DataRow = poData.NewRow()
            poData.Rows.Add(oLine)
            '' set them gameID to this gameline, otherwhise we won't know which game it belongs to 
            oLine("GameID") = psGameID
            Return oLine
        End If
        Return Nothing
    End Function


#End Region
End Class
