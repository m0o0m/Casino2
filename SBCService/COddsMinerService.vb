Imports SBCBL.std
Imports SBCService.CServiceStd
Imports System.Xml
Imports WebsiteLibrary.DBUtils

Public Class COddsMinerService
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Public Sub ExecOddsMiner()
        Try
            'LogDebug(log, "New thread spawned for SPORTSINSIGHT")

            If (ODDSMINER_EXECUTION_START > -1) Then
                'service timer already spawned a thread to collect sportsinsight data and 
                'it still hasn't completed, so check to make sure it's not taking too long
                Dim nTimeExecuting As Double = Timer - ODDSMINER_EXECUTION_START
                If nTimeExecuting > ODDSMINER_MAX_TIMEOUT Then
                    Throw New TimeoutException("ODDSMINER Exceeded Max Timeout: " & nTimeExecuting)
                End If

                'sports insight already running, just return
                'LogDebug(log, "Thread already exists for SPORTSINSIGHT for the past: " & nTimeExecuting & " seconds")
                Return
            End If

            ODDSMINER_EXECUTION_START = Timer 'this thread is now master processing thread for SPORTSINSIGHT
            Dim oStatedTime As DateTime = Now
            If EXECUTE_ODDSMINER_LINE Then

                ExecOddsMinerLines()
                log.Info("LINEFEED Executed time: " & Now.Subtract(oStatedTime).TotalMilliseconds & " ms")
            End If

            oStatedTime = Now
            If EXECUTE_ODDSMINER_SCORE Then
                ExecOddsMinerScores()
                log.Info("SCORESFEED Executed time: " & Now.Subtract(oStatedTime).TotalMilliseconds & " ms")
            End If

        Catch err As Exception
            CatchSBCServiceError(log, err)
        End Try

        'reset timer means we're done with the processing thread and another one can start!
        ODDSMINER_EXECUTION_START = -1
    End Sub

    Public Sub ExecOddsMinerLines()
        '' Get all lastest Games from OddsMiner
        LogDebug(log, "Exec ODDSMINER LINEFEED with last time: " & SafeString(ODDSMINER_LAST_EXEC))

        Dim sURL As String = ODDSMINER_URL_LINES & String.Format("?ident={0}&passwd={1}&spid=1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25&ot=1,3,4&all_odds=2&days=2", ODDSMINER_USER, ODDSMINER_PASS)
        If SafeString(ODDSMINER_LAST_EXEC) <> "" Then
            sURL &= "&last=" & ODDSMINER_LAST_EXEC
        End If
        LogInfo(log, "ODDSMINER LINEFEED URL: " & sURL)

        Dim sOddsMinerLines As String = webGet(sURL)
        '' Fail to get Lines from OddsMiner
        If sOddsMinerLines.IndexOf("error:") = 0 Then
            ODDSMINER_LAST_EXEC = ""
            LogInfo(log, "Fail to get OddsMiner LineFeed. Message: " & sOddsMinerLines)
            Exit Sub
        End If

        Dim oOddsDoc As New XmlDocument()
        oOddsDoc.LoadXml(sOddsMinerLines)
        LogInfo(log, "ODDSMINER LINEFEED Returned " & oOddsDoc.SelectNodes("//matches/match").Count & " TOTAL GAME EVENTS")

        '' Don't exec if they return old gamelines
        Dim sLastReturn As String = oOddsDoc.DocumentElement.GetAttribute("timestamp")
        If ODDSMINER_LAST_EXEC = sLastReturn Then
            Exit Sub
        End If

        Dim oDB As WebsiteLibrary.DBUtils.CSQLDBUtils = Nothing
        Try
            oDB = New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            For Each oMatch As XmlElement In oOddsDoc.SelectNodes("//matches/match")
                Dim oGame As New CGame()
                '' Change to Eastern timezone
                oGame.GameDate = SafeDate(oMatch.SelectSingleNode("time").InnerText)
                If oGame.GameDate.IsDaylightSavingTime() Then
                    oGame.GameDate = oGame.GameDate.AddHours(1)
                End If
                oGame.GameDate = oGame.GameDate.AddHours(-5)

                oGame.GameType = MapGameType(oMatch.SelectSingleNode("group").InnerText)
                oGame.LastUpdated = DateTime.Now

                oGame.HomeTeam.Team = MapTeamName(oMatch.SelectSingleNode("hteam").InnerText)
                oGame.AwayTeam.Team = MapTeamName(oMatch.SelectSingleNode("ateam").InnerText)

                '' Get GameLine of each bookmaker
                Dim oLine As CLine
                Dim oGameLine As XmlNodeList
                Dim oLastestOdd As XmlNode
                For Each oBookMaker As XmlElement In oMatch.SelectNodes("bookmaker")
                    Dim sBookmaker As String = oBookMaker.GetAttribute("name")

                    '' ot=1 | 2way / Moneyline
                    oGameLine = oBookMaker.SelectNodes("offer[@ot=1]")
                    If oGameLine.Count > 0 Then
                        '' Get lastest line
                        oLastestOdd = oGameLine(0).SelectSingleNode("odds[@i=0]")
                        oLine = oGame.GetLine("ODDSMINER", sBookmaker, "")
                        oLine.HomeMoneyLine = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o1").InnerText))
                        oLine.AwayMoneyLine = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o3").InnerText))
                    End If

                    '' ot=3 | Spreads
                    oGameLine = oBookMaker.SelectNodes("offer[@ot=3]")
                    If oGameLine.Count > 0 Then
                        '' Get lastest line
                        oLastestOdd = oGameLine(0).SelectSingleNode("odds[@i=0]")
                        Dim nHomeSpread As Double = SafeDouble(oLastestOdd.SelectSingleNode("o3").InnerText)

                        oLine = oGame.GetLine("ODDSMINER", sBookmaker, "")
                        oLine.HomeSpread = nHomeSpread
                        oLine.HomeSpreadMoney = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o1").InnerText))

                        oLine.AwaySpread = -nHomeSpread
                        oLine.AwaySpreadMoney = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o2").InnerText))
                    End If

                    '' ot=4 | Totals/ Over Under
                    oGameLine = oBookMaker.SelectNodes("offer[@ot=4]")
                    If oGameLine.Count > 0 Then
                        '' Get lastest line
                        oLastestOdd = oGameLine(0).SelectSingleNode("odds[@i=0]")
                        Dim nTotalPoint As Double = SafeDouble(oLastestOdd.SelectSingleNode("o3").InnerText)

                        oLine = oGame.GetLine("ODDSMINER", sBookmaker, "")
                        oLine.TotalPoints = nTotalPoint
                        oLine.TotalPointsOverMoney = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o1").InnerText))

                        oLine.TotalPointsUnderMoney = USFactorFormat(SafeDouble(oLastestOdd.SelectSingleNode("o2").InnerText))
                    End If
                Next

                oGame.SaveToDB(oDB, CGame.FindTeamBy.TeamName)
                oGame.SaveLinesToDB(oDB, SafeString(oGame.GameID), oGame.GameType)
            Next

            ODDSMINER_LAST_EXEC = sLastReturn
        Finally
            If oDB IsNot Nothing Then oDB.closeConnection()
        End Try

    End Sub

    Public Sub ExecOddsMinerScores()
        '' Get all lastest Game's score from OddsMiner
        LogDebug(log, "Exec ODDSMINER SCOREFEED with last time: " & SafeString(ODDSMINER_LAST_SCOREFEED_EXEC))

        Dim sURL As String = ODDSMINER_URL_SCORES & String.Format("?ident={0}&passwd={1}&spid=1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25&days=2", ODDSMINER_USER, ODDSMINER_PASS)
        If SafeString(ODDSMINER_LAST_SCOREFEED_EXEC) <> "" Then
            sURL &= "&last=" & ODDSMINER_LAST_SCOREFEED_EXEC
        End If
        LogInfo(log, "ODDSMINER SCOREFEED URL: " & sURL)

        Dim sOddsMinerScores As String = webGet(sURL)
        '' Fail to get Scores from OddsMiner
        If sOddsMinerScores.IndexOf("error:") = 0 Then
            ODDSMINER_LAST_SCOREFEED_EXEC = ""
            LogInfo(log, "Fail to get OddsMiner Scores. Message: " & sOddsMinerScores)
            Exit Sub
        End If

        Dim oOddsDoc As New XmlDocument()
        oOddsDoc.LoadXml(webGet(sURL))
        LogInfo(log, "ODDSMINER SCOREFEED Returned " & oOddsDoc.SelectNodes("//matches/match").Count & " TOTAL GAME'S SCORE")

        '' Don't exec if they return old gameScores
        Dim sLastReturn As String = oOddsDoc.DocumentElement.GetAttribute("timestamp")
        If ODDSMINER_LAST_SCOREFEED_EXEC = sLastReturn Then
            Exit Sub
        End If

        Dim oDB As WebsiteLibrary.DBUtils.CSQLDBUtils = Nothing
        Try
            oDB = New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Dim oGameDate As DateTime
            Dim sGameID, sHomeTeam, sAwayTeam, sGameType, sGameStatus, sScore As String
            Dim oGameResult As XmlNode
            '' Get all Games
            For Each oMatch As XmlElement In oOddsDoc.SelectNodes("//matches/match")

                '' Change to Eastern timezone
                oGameDate = SafeDate(oMatch.SelectSingleNode("time").InnerText)
                If oGameDate.IsDaylightSavingTime() Then
                    oGameDate = oGameDate.AddHours(1)
                End If
                oGameDate = oGameDate.AddHours(-5)

                sGameType = MapGameType(oMatch.SelectSingleNode("group").InnerText)
                sHomeTeam = MapTeamName(oMatch.SelectSingleNode("hteam").InnerText)
                sAwayTeam = MapTeamName(oMatch.SelectSingleNode("ateam").InnerText)

                '' Get Game's result
                oGameResult = oMatch.SelectSingleNode("results")
                If oGameResult IsNot Nothing AndAlso oGameResult.HasChildNodes Then
                    If oGameResult.SelectSingleNode("result[@id=1]") IsNot Nothing Then
                        sGameStatus = MapGameStatus(oGameResult.SelectSingleNode("status").InnerText)
                        sScore = SafeString(oGameResult.SelectSingleNode("result[@id=1]").InnerText)

                        '' Get GameID first
                        Dim oWhere As New CSQLWhereStringBuilder()
                        oWhere.AppendANDCondition("GameStatus <> 'FINAL'")
                        oWhere.AppendANDCondition("GameDate > " & SQLString(Now.AddDays(-1).ToShortDateString())) ' ????
                        oWhere.AppendANDCondition("GameDate < " & SQLString(Now.AddDays(2).ToShortDateString()))  '????
                        oWhere.AppendANDCondition("CONVERT(VARCHAR, GameDate , 101) = " & SQLString(oGameDate.ToString("MM/dd/yyyy")))
                        oWhere.AppendANDCondition("HomeTeam = " & SQLString(sHomeTeam))
                        oWhere.AppendANDCondition("AwayTeam = " & SQLString(sAwayTeam))
                        oWhere.AppendANDCondition("GameType = " & SQLString(sGameType))

                        Dim sSQL As String = "SELECT top 1 GameID, HomeScore, AwayScore FROM Games " & oWhere.SQL
                        log.Debug("Get GameID SQL: " & sSQL)

                        sGameID = ""
                        Dim oTblGame As DataTable = oDB.getDataTable(sSQL)
                        If oTblGame IsNot Nothing AndAlso oTblGame.Rows.Count > 0 Then
                            sGameID = SafeString(oTblGame.Rows(0)("GameID"))
                        End If

                        '' Update Game'score
                        If sGameID <> "" Then
                            Dim oUpdate As New CSQLUpdateStringBuilder("Games", "WHERE GameID = " & SQLString(sGameID))
                            oUpdate.AppendString("GameDate", SQLString(oGameDate))
                            oUpdate.AppendString("GameSTatus", SQLString(sGameStatus))

                            Dim nHScore, nAScore As Integer
                            nHScore = SafeInteger(sScore.Split("-"c)(0))
                            nAScore = SafeInteger(sScore.Split("-"c)(1))

                            If SafeInteger(oTblGame.Rows(0)("HomeScore")) < nHScore Then
                                oUpdate.AppendString("HomeScore", SQLString(nHScore))
                            End If
                            If SafeInteger(oTblGame.Rows(0)("AwayScore")) < nAScore Then
                                oUpdate.AppendString("AwayScore", SQLString(nAScore))
                            End If

                            oDB.executeNonQuery(oUpdate, "")
                        End If
                    End If
                End If
            Next

            ODDSMINER_LAST_SCOREFEED_EXEC = sLastReturn
        Finally
            If oDB IsNot Nothing Then oDB.closeConnection()
        End Try

    End Sub

#Region "Mapping"
    Private Function MapGameStatus(ByVal psGameStatus As String) As String
        Select Case psGameStatus
            Case "Fin", "Int", "Abd", "Post" 'Finish, Abandoned
                Return "Final"
            Case Else
                Return psGameStatus
        End Select
    End Function

    Private Function USFactorFormat(ByVal pnFactor As Double) As Double
        pnFactor = Math.Abs(pnFactor)

        If pnFactor <= 1 Then
            pnFactor = 0
        ElseIf pnFactor < 2 Then
            pnFactor = -100 / (pnFactor - 1)
        Else
            pnFactor = (pnFactor - 1) * 100
        End If

        Return Math.Round(pnFactor, 0)
    End Function

    Private Function MapGameType(ByVal psGroup As String) As String
        Dim sGroup As String = psGroup.Split(" "c)(0)

        Select Case sGroup
            'Case "AFBAFL"
            'Case "AFBARENA"
            'Case "AFBCFL"
            'Case "AFBNCAA"
            'Case "AFBNFL"
            'Case "ARAFL"
            'Case "BAAUS"
            'Case "BAAUT"
            'Case "BABUL"
            'Case "BACZE"
            'Case "BAECUP"
            'Case "BAENG"
            'Case "BAFIN"
            'Case "BAFRA"
            'Case "BAGER"
            'Case "BAGRE"
            'Case "BAINT"
            'Case "BAISR"
            'Case "BAITA"
            'Case "BALTU"
            Case "BANBA"
                Return "NBA Basketball"

            Case "BANCAA"
                Return "NCAA Basketball"

                'Case "BAPOL"
                'Case "BAROM"
                'Case "BARUS"
                'Case "BASLO"
                'Case "BASPA"
                'Case "BASWE"
                'Case "BATUR"
                'Case "BAUKR"
            Case "BAWNBA"
                Return "WNBA Basketball"

                'Case "BBINT"
                'Case "BBJPN"
                'Case "BBMBL"
            Case "BBMLB"
                Return "MLB Baseball"

                'Case "BBUSA"
                'Case "BI"
                'Case "BOB"
                'Case "BOXING"
                'Case "CCSKI"
                'Case "CRAUS"
                'Case "CRENG"
                'Case "CRIND"
                'Case "CRINT"
                'Case "CRWCLUB"
                'Case "CRZAF"
                'Case "CURL"
                'Case "CYCLING"
                'Case "DARTS"
                'Case "FBACUP"
                'Case "FBAFCUP"
                'Case "FBALB"
                'Case "FBALG"
                'Case "FBAND"
                'Case "FBARG"
                'Case "FBARM"
                'Case "FBASCUP"
                'Case "FBAUS"
                'Case "FBAUT"
                'Case "FBAZE"
                'Case "FBBEL"
                'Case "FBBHZ"
                'Case "FBBIH"
                'Case "FBBLR"
                'Case "FBBOL"
                'Case "FBBRA"
                'Case "FBBRN"
                'Case "FBBUL"
                'Case "FBCAN"
                'Case "FBCHI"
                'Case "FBCHN"
                'Case "FBCOL"
                'Case "FBCRI"
                'Case "FBCRO"
                'Case "FBCYP"
                'Case "FBCZE"
                'Case "FBDEN"
                'Case "FBDUT"
                'Case "FBECU"
                'Case "FBECUP"
                'Case "FBEGY"
                'Case "FBENG"
                'Case "FBEST"
                'Case "FBFAR"
                'Case "FBFIN"
                'Case "FBFRA"
                'Case "FBGEO"
                'Case "FBGER"
                'Case "FBGRE"
                'Case "FBGUA"
                'Case "FBHKG"
                'Case "FBHUN"
                'Case "FBICE"
                'Case "FBICUP"
                'Case "FBIDN"
                'Case "FBIND"
                'Case "FBINT"
                'Case "FBINTY"
                'Case "FBIRE"
                'Case "FBIRI"
                'Case "FBIRQ"
                'Case "FBISR"
                'Case "FBITA"
                'Case "FBJAM"
                'Case "FBJOR"
                'Case "FBJPN"
                'Case "FBKAZ"
                'Case "FBKOR"
                'Case "FBKSA"
                'Case "FBKUW"
                'Case "FBLAT"
                'Case "FBLBN"
                'Case "FBLBY"
                'Case "FBLIE"
                'Case "FBLTU"
                'Case "FBLUX"
                'Case "FBMAR"
                'Case "FBMEX"
                'Case "FBMKD"
                'Case "FBMLS"
                'Case "FBMLT"
                'Case "FBMNE"
                'Case "FBMOL"
                'Case "FBMYS"
                'Case "FBNACUP"
                'Case "FBNIR"
                'Case "FBNOR"
                'Case "FBNZL"
                'Case "FBOMA"
                'Case "FBPAR"
                'Case "FBPER"
                'Case "FBPOL"
                'Case "FBPOR"
                'Case "FBQAT"
                'Case "FBROM"
                'Case "FBRUS"
                'Case "FBSACUP"
                'Case "FBSCO"
                'Case "FBSGP"
                'Case "FBSLO"
                'Case "FBSMR"
                'Case "FBSPA"
                'Case "FBSRB"
                'Case "FBSVK"
                'Case "FBSWE"
                'Case "FBSWI"
                'Case "FBSYR"
                'Case "FBTHA"
                'Case "FBTOUR"
                'Case "FBTUN"
                'Case "FBTUR"
                'Case "FBUAE"
                'Case "FBUKR"
                'Case "FBURU"
                'Case "FBUSL"
                'Case "FBUZB"
                'Case "FBVEN"
                'Case "FBVNM"
                'Case "FBWAL"
                'Case "FBYEM"
                'Case "FBYUG"
                'Case "FBZAF"
                'Case "FLFIN"
                'Case "FSKAT"
                'Case "FSSKI"
            Case "GOLF"
                Return "Golf"

                'Case "HBAUT"
                'Case "HBBLR"
                'Case "HBCRO"
                'Case "HBCZE"
                'Case "HBDEN"
                'Case "HBECUP"
                'Case "HBFIN"
                'Case "HBFRA"
                'Case "HBGER"
                'Case "HBHUN"
                'Case "HBINT"
                'Case "HBITA"
                'Case "HBNOR"
                'Case "HBPOL"
                'Case "HBPOR"
                'Case "HBROM"
                'Case "HBRUS"
                'Case "HBSLO"
                'Case "HBSPA"
                'Case "HBSVK"
                'Case "HBSWE"
                'Case "HBSWI"
                'Case "HOECUP"
                'Case "HOINT"
                'Case "HOITA"
                'Case "IBNOR"
                'Case "IBSWE"
                'Case "ICAUT"
                'Case "ICCZE"
                'Case "ICDEN"
                'Case "ICECUP"
                'Case "ICFIN"
                'Case "ICFRA"
                'Case "ICGER"
                'Case "ICINT"
                'Case "ICITA"
                'Case "ICNA"
                'Case "ICNHL"
                'Case "ICNOR"
                'Case "ICRUS"
                'Case "ICSLO"
                'Case "ICSVK"
                'Case "ICSWE"
                'Case "ICSWI"
                'Case "LUGE"
                'Case "MISC"
                'Case "MMA"
                'Case "MOTOR"
                'Case "NC"
                'Case "NPPFIN"
                'Case "PPFIN"
                'Case "RGAUS"
                'Case "RGECUP"
                'Case "RGENG"
                'Case "RGFRA"
                'Case "RGGER"
                'Case "RGINT"
                'Case "RGITA"
                'Case "RGNRL"
                'Case "RGNZL"
                'Case "RGSPA"
                'Case "RGZAF"
                'Case "RHITA"
                'Case "RLENG"
                'Case "RLINT"
                'Case "RLNRL"
                'Case "SKEL"
                'Case "SKI"
                'Case "SKIJUMP"
                'Case "SNOOKER"
                'Case "SNOWB"
                'Case "SSKAT"
                'Case "STSSKAT"
            Case "TENNIS"
                Return "Tennis"

                'Case "VBAUT"
                'Case "VBBEL"
                'Case "VBCZE"
                'Case "VBDEN"
                'Case "VBDUT"
                'Case "VBECUP"
                'Case "VBFIN"
                'Case "VBFRA"
                'Case "VBGER"
                'Case "VBGRE"
                'Case "VBINT"
                'Case "VBITA"
                'Case "VBJPN"
                'Case "VBNOR"
                'Case "VBPOL"
                'Case "VBPOR"
                'Case "VBRUS"
                'Case "VBSLO"
                'Case "VBSPA"
                'Case "VBSVK"
                'Case "VBSWI"
                'Case "VBTUR"
                'Case "WBAINT"
                'Case "WBAITA"
                'Case "WBI"
                'Case "WBOB"
                'Case "WCCSKI"
                'Case "WCURL"
                'Case "WFBINT"
                'Case "WFBSWE"
                'Case "WFSKAT"
                'Case "WFSSKI"
                'Case "WHBAUT"
                'Case "WHBCRO"
                'Case "WHBDEN"
                'Case "WHBECUP"
                'Case "WHBFIN"
                'Case "WHBFRA"
                'Case "WHBGER"
                'Case "WHBHUN"
                'Case "WHBINT"
                'Case "WHBNOR"
                'Case "WHBPOL"
                'Case "WHBROM"
                'Case "WHBRUS"
                'Case "WHBSLO"
                'Case "WHBSPA"
                'Case "WHBSWE"
                'Case "WHBSWI"
                'Case "WHOINT"
                'Case "WICINT"
                'Case "WLUGE"
                'Case "WPECUP"
                'Case "WPINT"
                'Case "WSKEL"
                'Case "WSKI"
                'Case "WSKIJUMP"
                'Case "WSNOWB"
                'Case "WSSKAT"
                'Case "WSTSSKAT"
                'Case "WVBAUT"
                'Case "WVBDEN"
                'Case "WVBECUP"
                'Case "WVBFIN"
                'Case "WVBGER"
                'Case "WVBINT"
                'Case "WVBITA"
                'Case "WVBNOR"
                'Case "WVBSLO"
                'Case "WVBSPA"
                'Case "WVBSWI"
                'Case "WVBTUR"
                'Case "WWPINT"
        End Select

        Return psGroup
    End Function

    Private Function MapTeamName(ByVal psTeamName As String) As String
        Dim sMapTeam As String = GetBaseballTeam(psTeamName)
        If sMapTeam = "" Then
            sMapTeam = GetBasketballTeam(psTeamName)
        End If

        If sMapTeam = "" Then
            sMapTeam = psTeamName
        End If

        Return sMapTeam
    End Function

    Private Function GetBasketballTeam(ByVal psTeamName As String) As String
        Select Case psTeamName
            Case "76ers"
                Return "Philadelphia 76ers"

            Case "Bobcats"
                Return "Charlotte Bobcats"

            Case "Bucks"
                Return "Milwaukee Bucks"

            Case "Bulls"
                Return "Chicago Bulls"

            Case "Cavaliers"
                Return "Cleveland Cavaliers"

            Case "Celtics"
                Return "Boston Celtics"

            Case "Clippers"
                Return "Los Angeles Clippers"

            Case "East"
                Return "East" 'Can't map

            Case "Grizzlies"
                Return "Memphis Grizzlies"

            Case "Hawks"
                Return "Atlanta Hawks"

            Case "Heat"
                Return "Miami Heat"

            Case "Hornets"
                Return "New Orleans Hornets"

            Case "Jazz"
                Return "Utah Jazz"

            Case "Kings"
                Return "Sacramento Kings"

            Case "Knicks"
                Return "New York Knicks"

            Case "Lakers"
                Return "Los Angeles Lakers"

            Case "Magic"
                Return "Orlando Magic"

            Case "Mavericks"
                Return "Dallas Mavericks"

            Case "Nuggets"
                Return "Denver Nuggets"

            Case "Nets"
                Return "New Jersey Nets"

            Case "Oklahoma City Thunder"
                Return "Oklahoma City Thunder"

            Case "Pacers"
                Return "Indiana Pacers"

            Case "Pistons"
                Return "Detroit Pistons"

            Case "Raptors"
                Return "Toronto Raptors"

            Case "Rockets"
                Return "Houston Rockets"

            Case "Rookies"
                Return "Rookies" 'can't map

            Case "Sophomores"
                Return "Sophomores" 'can't map

            Case "Spurs"
                Return "San Antonio Spurs"

            Case "Suns"
                Return "Phoenix Suns"

            Case "Supersonics"
                Return "Supersonics" 'can't map

            Case "Timberwolves"
                Return "Minnesota Timberwolves"

            Case "Trailblazers"
                Return "Portland Trail Blazers"

            Case "Warriors"
                Return "Golden State Warriors"

            Case "West"
                Return "West" 'can't map

            Case "Wizards"
                Return "Washington Wizards"

            Case Else
                Return ""

        End Select

    End Function

    Private Function GetBaseballTeam(ByVal psTeamName As String) As String
        Select Case psTeamName
            Case "Arizona Diamondbacks"
                Return "Arizona D-Backs"

            Case "Tampa Bay Devil Rays"
                Return "Tampa Bay Rays"

            Case Else
                Return ""

        End Select
    End Function
#End Region
End Class
