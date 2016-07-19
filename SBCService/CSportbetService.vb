Imports WebsiteLibrary.DBUtils
Imports System.Xml
Imports SBCBL.std
Imports System.Text
Imports System.Data
Imports SBCService.CServiceStd
Imports System.Xml.XPath

Public Class CSportbetService
#Region "SportBet XML service"
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Dim _oGameTypes As List(Of String)
    Dim _SportBetGameTypes As Hashtable
    Private ReadOnly Property GameTypes() As List(Of String)
        Get
            If _oGameTypes Is Nothing Then
                _oGameTypes = LoadGameTypes()
            End If
            Return _oGameTypes
        End Get
    End Property

    Public Sub execSportBet()
        ProcessSportBetLine()
        ProcessSportBetScore()
    End Sub

    Private ReadOnly Property SportBetGameTypes() As Hashtable
        Get
            If _SportBetGameTypes Is Nothing Then
                _SportBetGameTypes = New Hashtable()
                _SportBetGameTypes.Add("395", "NCAA BASKETBALL WOMEN")
                _SportBetGameTypes.Add("4", "NCAA BASKETBALL")
                _SportBetGameTypes.Add("5", "MLB Baseball")
                _SportBetGameTypes.Add("3", "NBA Basketball")
                _SportBetGameTypes.Add("7", "NHL Hockey")
                _SportBetGameTypes.Add("50", "Soccer")
                _SportBetGameTypes.Add("88", "Soccer")
                _SportBetGameTypes.Add("166", "Soccer")
                _SportBetGameTypes.Add("330", "Soccer")
            End If
            Return _SportBetGameTypes
        End Get
    End Property

#Region "Line Feed"
    Private Sub ProcessSportBetLine()
        '' download xml data
        Dim sXML As String = WebsiteLibrary.CSBCStd.webGet(SPORTBET_URL_LINE)
        Dim oListGames As New List(Of CGame)
        Dim oRoot As New XmlDocument()
        oRoot.LoadXml(sXML)
        Dim oSports As XmlNodeList = oRoot.SelectNodes("SportLines/sport")
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

        LogInfo(log, "SPORTBET LINEFEED Returned " & oSports.Count.ToString() & " TOTAL GAME TYPES")
        Try
            For Each oSport As XmlNode In oSports
                '' just retrive CBB, MLB, NBA, NHL,SOC
                Dim sGameType As String = SafeString(oSport.Attributes("IdLeague").Value)
                If SportBetGameTypes.ContainsKey(sGameType) Then
                    '' store the list of game that each sport return 
                    Dim oGames As CGame() = ProcessSportGroup(oSport).ToArray()
                    oListGames.AddRange(oGames)
                    LogInfo(log, "SPORTBET LINEFEED RETURNED " & oGames.Length.ToString() & " GAMES FOR GAME TYPE [" + SafeString(oSport.Attributes("type").Value) & "]")
                End If
            Next


            LogInfo(log, "SPORTBET LINEFEED RETURNED " & oListGames.Count.ToString() & " TOTAL GAMES")
            log.Debug("START TO SAVE ALL GAMES TO DB")

            ''save all game to DB
            For Each oGame As CGame In oListGames
                oGame.SaveToDB(oDB, CGame.FindTeamBy.NSS)
                oGame.SaveLinesToDB(oDB, SafeString(oGame.GameID), oGame.GameType)
            Next
        Catch ex As Exception
            log.Error("Can't save game to DB: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try
    End Sub

    '' proccessing each of node <game> nested in group <game>
    Private Function ProcessSportGroup(ByVal poSport As XmlNode) As List(Of CGame)
        Dim oGameReturn As New List(Of CGame)
        Dim sSBGameType As String = SafeString(poSport.Attributes("type").Value)
        Dim oGameNodes As XmlNodeList = poSport.SelectNodes("game")

        For Each oGameSingleNode As XmlNode In oGameNodes
            Dim oGame As New CGame()
            oGame.GameType = SafeString(SportBetGameTypes(sSBGameType))
            oGame.GameDate = SafeDate(oGameSingleNode.SelectSingleNode("date").InnerText)
            oGame.AwayTeam.RotationNumber = SafeInteger(oGameSingleNode.SelectSingleNode("awaynss").InnerText)
            oGame.AwayTeam.Team = SafeString(oGameSingleNode.SelectSingleNode("awayteam").InnerText)
            oGame.HomeTeam.RotationNumber = SafeInteger(oGameSingleNode.SelectSingleNode("homenss").InnerText)
            oGame.HomeTeam.Team = SafeString(oGameSingleNode.SelectSingleNode("hometeam").InnerText)
            oGame.LastUpdated = DateTime.Now

            Dim sBookmaker As String = "Sportbet"
            Dim sContext As String = "Current"
            Dim oLine As CLine = oGame.GetLine("SPORTSINSIGHTS", sBookmaker, sContext)
            oLine.Bookmaker = sBookmaker
            oLine.Context = sContext
            oLine.FeedSource = sBookmaker

            Dim sTotalOver As String = SafeString(oGameSingleNode.SelectSingleNode("totalover").InnerText) '' totalover value: o128-110
            Dim sTotalUnder As String = SafeString(oGameSingleNode.SelectSingleNode("totalunder").InnerText)
            Dim sHomePread As String = SafeString(oGameSingleNode.SelectSingleNode("homespread").InnerText)
            Dim sVisitorSpread As String = SafeString(oGameSingleNode.SelectSingleNode("visitorspread").InnerText)

            If sHomePread <> "" Then
                oLine.HomeSpread = SafeDouble(ParseToNumber(sHomePread)(0))
                oLine.HomeSpreadMoney = SafeDouble(ParseToNumber(sHomePread)(1))
                oLine.AwaySpread = SafeDouble(ParseToNumber(sVisitorSpread)(0))
                oLine.AwaySpreadMoney = SafeDouble(ParseToNumber(sVisitorSpread)(1))
            End If

            oLine.HomeMoneyLine = SafeDouble(oGameSingleNode.SelectSingleNode("HomeMoneyLine").InnerText)
            oLine.AwayMoneyLine = SafeDouble(oGameSingleNode.SelectSingleNode("AwayMoneyLine").InnerText)

            'IMPORTANT: HOME TEAM total point money = odds for the OVER
            If sTotalOver <> "" Then
                oLine.TotalPoints = ParseToNumber(sTotalOver)(1)
                oLine.TotalPointsOverMoney = ParseToNumber(sTotalOver)(0)
                oLine.TotalPointsUnderMoney = ParseToNumber(sTotalUnder)(0)
            End If
            oGameReturn.Add(oGame)
        Next
        Return oGameReturn
    End Function

#End Region

#Region "Score Feed"
    Private Sub ProcessSportBetScore()
        log.Info("STARTING UPDATE SCORE")
        Dim sXML As String = WebsiteLibrary.CSBCStd.webGet(SPORTBET_URL_SCORE)
        Dim oListGames As New List(Of CGame)
        Dim oRoot As New XmlDocument()
        oRoot.LoadXml(sXML)
        Dim oGames As XmlNodeList = oRoot.SelectNodes("Scores/Game")
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
        Try

        '' retrive data from DB first
            Dim sSQL As String = "select * from Games where gamestatus <> 'final' and (GameDate > " & SQLString(Now.AddDays(-1).ToShortDateString()) & _
                                " and GameDate < " & SQLString(Now.AddDays(2).ToShortDateString()) & ")"
            log.Debug("Get Scorefeed SQL: " & sSQL)
            Dim oDTSource As DataTable = oDB.getDataTable(sSQL)

            Dim nNumGames As Integer = oDTSource.Rows.Count
            LogInfo(log, "SPORTSINSIGHTS SCOREFEED Returned " & nNumGames & " TOTAL GAME EVENTS")
            For Each oRow As DataRow In oDTSource.Rows
                SingleScoreGameProcess(oRow, oRoot)
            Next
            UpdateTableScore(oDTSource)
        Catch ex As Exception
            log.Error("Cant Process SportBet Score: " & ex.Message, ex)
        Finally
            oDB.closeConnection()
        End Try

    End Sub

    Private Sub SingleScoreGameProcess(ByVal poRow As DataRow, ByVal poRoot As XmlNode)
        Dim sXPATH As String = "Scores/Game[VisitorNss = {0} and HomeNss = {1} and GameDate ={2} ]"
        Try
            sXPATH = String.Format(sXPATH, SQLString(poRow("AwayRotationNumber")), SQLString(poRow("HomeRotationNumber")), SQLString(poRow("GameDate")))
            Dim oGameNode As XmlNode = poRoot.SelectSingleNode(sXPATH)
            If oGameNode Is Nothing Then
                log.Error("Can't find xmlnode with xpath: " & sXPATH)
                Return
            End If

            poRow("AwayScore") = SafeInteger(oGameNode.SelectSingleNode("VisitorScore").InnerText)
            poRow("HomeScore") = SafeInteger(oGameNode.SelectSingleNode("HomeScore").InnerText)
            poRow("LastUpdated") = Date.Now.ToUniversalTime()
            log.Debug("Game Score: VisitorNss: " + SafeString(poRow("AwayRotationNumber")) + ", HomeNss: " + SafeString(poRow("HomeRotationNumber")) + ",AwayScore: " + SafeString(poRow("AwayScore")) + ", HomeScore: " + SafeString(poRow("HomeScore")))
        Catch ex As Exception
            log.Error("can't processs game score. XPTAH: " + sXPATH)
            log.Error(ex.Message, ex)
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
#End Region

    ''  sportbet service return information by pair of values : TotalPoint+ TotalPointmaoney, HomeSpread + HomeSpreadMoney  so we have to parse it to right values
    Private Function ParseToNumber(ByVal psNodeText As String) As Double()
        '' remove "o" or "u" character
        psNodeText = psNodeText.Replace("o", "").Replace("u", "")
        '' get the last position 
        Dim nLastOperator As Integer = psNodeText.LastIndexOf("-"c)
        If nLastOperator < psNodeText.LastIndexOf("+"c) Then
            nLastOperator = psNodeText.LastIndexOf("+"c)
        End If
        Dim nValue1 As Double = SafeDouble(psNodeText.Substring(0, psNodeText.Length - nLastOperator))
        Dim nValue2 As Double = SafeDouble(psNodeText.Substring(nLastOperator))
        Return New Double() {nValue1, nValue2}
    End Function

    '' return list of game types in Syssettings
    Private Function LoadGameTypes() As List(Of String)
        Dim sSQL As String = "select * from SysSettings where Category= 'GameType'"
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
        Dim oDT As DataTable = Nothing
        Dim oListGameTypes As New List(Of String)
        Try
            oDT = oDB.getDataTable(sSQL)
            For Each oRow As DataRow In oDT.Rows
                oListGameTypes.Add(SafeString(oRow("Key")))
            Next
        Catch ex As Exception
            log.Error("Can't get gametype from DB")
        Finally
            oDB.closeConnection()
        End Try
        Return oListGameTypes
    End Function

    Public Function MapGameType(ByVal psSportBetType As String) As String
        If SportBetGameTypes.ContainsKey(psSportBetType) Then
            Return SafeString(SportBetGameTypes(psSportBetType))
        End If
        Return ""
    End Function

#End Region
End Class


