Imports WebsiteLibrary.DBUtils
Imports System.Xml
Imports SBCBL.std
Imports System.Text
Imports System.Data
Imports SBCService.CServiceStd
Imports System.Xml.XPath
Imports SBCEngine

Public Class CGame
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Public GameID As Guid = Guid.NewGuid()
    Public GameDate As DateTime
    Public GameType As String
    Public LastUpdated As DateTime = Date.Now
    Public GameStatus As String

    Public HomeTeam As New CGameTeam()
    Public AwayTeam As New CGameTeam()

    Public HomeScore As Integer = -1
    Public AwayScore As Integer = -1
    Public HomePitcher As String
    Public AwayPitcher As String
    Public HomePitcherRightHand As Boolean
    Public AwayPitcherRightHand As Boolean
    Public IsFirstHalfFinished As Boolean = False
    Public IsUpdateGameDate As Boolean = False
    Public IsForProp As Boolean = False

    Public HomeFirstQScore As Integer = -1
    Public HomeSecondQScore As Integer = -1
    Public HomeThirdQScore As Integer = -1
    Public HomeFourQScore As Integer = -1
    Public AwayFirstQScore As Integer = -1
    Public AwaySecondQScore As Integer = -1
    Public AwayThirdQScore As Integer = -1
    Public AwayFourQScore As Integer = -1
    Public DrawRotationNumber As Integer = -1
    Public PropDescription As String
    Public ExtPropNumber As String
    Public Description As String
    Public IsCircle As Boolean
    Public IsAddedGame As Boolean
    Public IsExtraGame As Boolean

    Private Lines As New Hashtable 'maps bookmakername+context to the cline object

    Public Function GetLine(ByVal psFeedSource As String, ByVal psBookerName As String, ByVal psContext As String) As CLine
        Dim sKey As String = psFeedSource & "_" & psBookerName & "_" & psContext
        Dim oLine As CLine = CType(Lines(sKey), CLine)
        If oLine Is Nothing Then
            oLine = New CLine()
            oLine.GameLineID = Guid.NewGuid()
            oLine.FeedSource = psFeedSource
            oLine.Bookmaker = psBookerName
            'oLine.Context = StrConv(psContext, vbProperCase)
            oLine.Context = psContext
            Lines(sKey) = oLine
        End If
        Return oLine
    End Function

    Public Function GetPropLine(ByVal psFeedSource As String, ByVal psBookerName As String, ByVal psPropRotationNumber As String) As CLine
        Dim sKey As String = psFeedSource & "_" & psBookerName & "_" & psPropRotationNumber
        Dim oLine As CLine = CType(Lines(sKey), CLine)
        If oLine Is Nothing Then
            oLine = New CLine()
            oLine.GameLineID = Guid.NewGuid()
            oLine.FeedSource = psFeedSource
            oLine.Bookmaker = psBookerName
            oLine.PropRotationNumber = psPropRotationNumber
            Lines(sKey) = oLine
        End If
        Return oLine
    End Function

    Public Sub SaveToDB(ByVal poDB As CSQLDBUtils, ByVal poFindBy As FindTeamBy)

        'LogDebug(log, GameType & " : " & AwayTeam.Team & " @ " & HomeTeam.Team)
        Dim oNow As DateTime = GetEasternDate().AddHours(-36)
        oNow = oNow.AddHours(-5)

        ' first try with find by team name

        'check if this game is already in DB
        Dim oWhere As New CSQLWhereStringBuilder()
        'oWhere.AppendANDCondition("GameType=" & SQLString(GameType))
        'oWhere.AppendANDCondition("CONVERT(VARCHAR, Gamedate , 101) = " & SQLString(SafeDate(GameDate).ToString("MM/dd/yyyy")))
        oWhere.AppendANDCondition("( Gamedate > " & SQLString(SafeDate(GameDate).AddHours(-24)) & _
                                    " and Gamedate < " & SQLString(SafeDate(GameDate).AddHours(28)) & ")")
        oWhere.AppendANDCondition("Gamedate > " & SQLString(oNow))

        If poFindBy = FindTeamBy.NSS OrElse poFindBy = FindTeamBy.Both Then
            oWhere.AppendANDCondition("HomeRotationNumber=" & HomeTeam.RotationNumber)
            oWhere.AppendANDCondition("AwayRotationNumber=" & AwayTeam.RotationNumber)
        End If

        'If poFindBy = FindTeamBy.TeamName OrElse poFindBy = FindTeamBy.Both Then
        oWhere.AppendANDCondition("HomeTeam = " & SQLString(Replace(HomeTeam.Team, "'", " ")))
        oWhere.AppendANDCondition("AwayTeam = " & SQLString(Replace(AwayTeam.Team, "'", " ")))
        'End If

        If Me.PropDescription = "Series Price" Then
            log.Debug("Check if this game is already in DB: " & "SELECT top 1 * FROM Games " & oWhere.SQL & " ORDER BY GameDate ")
        End If

        'log.Debug("Check if this game is already in DB: " & oWhere.SQL)
        Dim oDTCheck As DataTable = poDB.getDataTable("SELECT top 1 * FROM Games " & oWhere.SQL & " ORDER BY GameDate ")
        Dim sID As String = ""
        Dim sGameLastStatus As String = ""
        If oDTCheck.Rows.Count > 0 Then
            sID = SafeString(oDTCheck.Rows(0)("GameID"))
            sGameLastStatus = SafeString(oDTCheck.Rows(0)("GameStatus"))

            '' check if game duplicate
            If oDTCheck.Rows.Count > 1 Then
                Dim sDuplicatedID As String = SafeString(oDTCheck.Rows(1)("GameID"))
                Dim sSQL As String = String.Format("update TicketBets set GameID={0} set GameID= {1}", SQLString(sID), SQLString(sDuplicatedID))
                poDB.executeNonQuery(sSQL)
                sSQL = String.Format("update GameLines set GameID={0} set GameID= {1}", SQLString(sID), SQLString(sDuplicatedID))
                poDB.executeNonQuery(sSQL)
                sSQL = String.Format("delete from Games where GameID= {0}", SQLString(sDuplicatedID))
                poDB.executeNonQuery(sSQL)
            End If
        Else
            '' if cant find with team name, try again with just NSS
            '' reset where conditions
            oWhere = New CSQLWhereStringBuilder()
            'oWhere.AppendANDCondition("GameType=" & SQLString(GameType))
            'oWhere.AppendANDCondition("CONVERT(VARCHAR, Gamedate , 101) = " & SQLString(SafeDate(GameDate).ToString("MM/dd/yyyy")))
            oWhere.AppendANDCondition("( Gamedate > " & SQLString(SafeDate(GameDate).AddHours(-24)) & _
                                        " and Gamedate < " & SQLString(SafeDate(GameDate).AddHours(28)) & ")")
            oWhere.AppendANDCondition("Gamedate > " & SQLString(oNow))

            oWhere.AppendANDCondition("HomeRotationNumber=" & HomeTeam.RotationNumber)
            oWhere.AppendANDCondition("AwayRotationNumber=" & AwayTeam.RotationNumber)
            oDTCheck = poDB.getDataTable("SELECT top 1 * FROM Games " & oWhere.SQL)

            If oDTCheck.Rows.Count > 0 Then
                sID = SafeString(oDTCheck.Rows(0)("GameID"))
            End If
        End If

        Dim oSQL As ISQLEditStringBuilder = Nothing
        If sID <> "" Then
            GameID = New Guid(sID)
            oSQL = New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(GameID))

            '' just update gamedate if the service allowed
            If IsUpdateGameDate Then
                oSQL.AppendString("GameDate", SQLDate(GameDate))
            End If
            oSQL.AppendString("IsForProp", IIf(IsForProp, "'Y'", "'N'").ToString())
            If SafeString(oDTCheck.Rows(0)("OriginHomePitcher")) = "" OrElse SafeString(oDTCheck.Rows(0)("OriginAwayPitcher")) = "" Then
                oSQL.AppendString("OriginHomePitcher", SQLString(Replace(HomePitcher, "'", " ")))
                oSQL.AppendString("OriginAwayPitcher", SQLString(Replace(AwayPitcher, "'", " ")))
            End If

            'LogDebug(log, "Exist Game: " & sID)
        Else
            '' dont save game if no pitcher now
            If GameType = "MLB Baseball" And (HomePitcher = "" OrElse AwayPitcher = "") Then
                Return
            End If

            oSQL = New CSQLInsertStringBuilder("Games")
            oSQL.AppendString("GameID", SQLString(GameID))
            oSQL.AppendString("GameType", SQLString(GameType))
            oSQL.AppendString("HomeRotationNumber", SQLString(HomeTeam.RotationNumber))
            oSQL.AppendString("AwayRotationNumber", SQLString(AwayTeam.RotationNumber))
            oSQL.AppendString("GameDate", SQLDate(GameDate))
            oSQL.AppendString("IsFirstHalfFinished", "'N'")
            oSQL.AppendString("IsForProp", IIf(IsForProp, "'Y'", "'N'").ToString())
            oSQL.AppendString("OriginHomePitcher", SQLString(HomePitcher))
            oSQL.AppendString("OriginAwayPitcher", SQLString(AwayPitcher))

        End If

        If DrawRotationNumber <> -1 Then
            oSQL.AppendString("DrawRotationNumber", SafeString(DrawRotationNumber))
        End If

        If HomeScore <> -1 Then
            oSQL.AppendString("HomeScore", SQLString(HomeScore))
        End If
        If AwayScore <> -1 Then
            oSQL.AppendString("AwayScore", SQLString(AwayScore))
        End If

        If HomeFirstQScore <> -1 Then
            oSQL.AppendString("HomeFirstQScore", SQLString(HomeFirstQScore))
        End If

        If HomeSecondQScore <> -1 Then
            oSQL.AppendString("HomeSecondQScore", SQLString(HomeSecondQScore))
        End If

        If HomeThirdQScore <> -1 Then
            oSQL.AppendString("HomeThirdQScore", SQLString(HomeThirdQScore))
        End If

        If HomeFourQScore <> -1 Then
            oSQL.AppendString("HomeFourQScore", SQLString(HomeFourQScore))
        End If
        If AwayFirstQScore <> -1 Then
            oSQL.AppendString("AwayFirstQScore", SQLString(HomeFourQScore))
        End If
        If AwaySecondQScore <> -1 Then
            oSQL.AppendString("AwaySecondQScore", SQLString(AwaySecondQScore))
        End If
        If AwayThirdQScore <> -1 Then
            oSQL.AppendString("AwayThirdQScore", SQLString(AwayThirdQScore))
        End If

        If AwayFourQScore <> -1 Then
            oSQL.AppendString("AwayFourQScore", SQLString(AwayFourQScore))
        End If

        If Not String.IsNullOrEmpty(Description) And Description <> "" Then
            oSQL.AppendString("Description", SQLString(Description))
        End If
        oSQL.AppendString("LastUpdated", SQLDate(LastUpdated.ToUniversalTime))
        If sGameLastStatus <> "Final" Then
            oSQL.AppendString("GameStatus", SQLString(GameStatus))
        End If

        If UCase(GameStatus) = "Time" Then
            log.Info("Status before TIME: " & sGameLastStatus & ". Current status: " & GameStatus)
        End If

        ' game status change to time, mean 2H is begun
        If (sGameLastStatus = "1st H" Or sGameLastStatus = "2nd Q") And GameStatus = "Time" Then
            log.Info("SecondHalfTime for game: " & GameID.ToString())
            oSQL.AppendString("SecondHalfTime", SQLString(DateTime.Now.ToUniversalTime()))
        End If

        oSQL.AppendString("GameType", SQLString(GameType))
        oSQL.AppendString("HomeTeam", SQLString(Replace(HomeTeam.Team, "'", " ")))
        oSQL.AppendString("AwayTeam", SQLString(Replace(AwayTeam.Team, "'", " ")))
        oSQL.AppendString("HomePitcher", SQLString(HomePitcher))
        oSQL.AppendString("AwayPitcher", SQLString(AwayPitcher))
        oSQL.AppendString("HomePitcherRightHand", SQLString(IIf(HomePitcherRightHand, "Y", "N")))
        oSQL.AppendString("HomePitcherRightHand", SQLString(IIf(AwayPitcherRightHand, "Y", "N")))
        oSQL.AppendString("PropDescription", SQLString(PropDescription))
        If ExtPropNumber <> "" Then
            oSQL.AppendString("ExtPropNumber", SQLString(ExtPropNumber))
        End If
        oSQL.AppendString("IsCircle", SQLString(IIf(IsCircle, "Y", "N")))
        oSQL.AppendString("IsAddedGame", SQLString(IIf(IsAddedGame, "Y", "N")))
        poDB.executeNonQuery(oSQL, "")

        '' if this is extra game, lock betting!
        If IsExtraGame Then
            CGameRule.LockDiv2Game(GameID.ToString(), poDB)
        End If
    End Sub

    Public Sub SavePropToDB(ByVal poDB As CSQLDBUtils)
        'LogDebug(log, GameType & " : " & AwayTeam.Team & " @ " & HomeTeam.Team)
        Dim oNow As DateTime = GetEasternDate().AddHours(-41)

        'check if this game is already in DB by NSS
        Dim oWhere As New CSQLWhereStringBuilder()
        oWhere.AppendANDCondition("GameType=" & SQLString(GameType))
        oWhere.AppendANDCondition("Gamedate > " & SQLString(oNow.AddDays(-30)))
        oWhere.AppendANDCondition("ExtPropNumber = " & SQLString(ExtPropNumber))

        'log.Debug("Check if this game is already in DB: " & oWhere.SQL)
        Dim oDTCheck As DataTable = poDB.getDataTable("SELECT top 1 * FROM Games " & oWhere.SQL & " ORDER BY GameDate desc")
        Dim sID As String = ""

        If oDTCheck.Rows.Count > 0 Then
            sID = SafeString(oDTCheck.Rows(0)("GameID"))
        Else
            '' if cant find with team name, try again with PropDescription
            '' reset where conditions
            oWhere.Reset()
            oWhere.AppendANDCondition("GameType=" & SQLString(GameType))
            oWhere.AppendANDCondition("Gamedate > " & SQLString(oNow.AddDays(-30)))
            oWhere.AppendANDCondition("PropDescription = " & SQLString(PropDescription))

            oDTCheck = poDB.getDataTable("SELECT top 1 * FROM Games " & oWhere.SQL & " ORDER BY GameDate desc")

            If oDTCheck.Rows.Count > 0 Then
                sID = SafeString(oDTCheck.Rows(0)("GameID"))
            End If
        End If

        Dim oSQL As ISQLEditStringBuilder = Nothing
        If sID <> "" Then
            GameID = New Guid(sID)
            oSQL = New CSQLUpdateStringBuilder("Games", "WHERE GameID=" & SQLString(GameID))

            '' just update gamedate if the service allowed
            If IsUpdateGameDate Then
                oSQL.AppendString("GameDate", SQLDate(GameDate))
            End If

            'LogDebug(log, "Exist Game: " & sID)
        Else
            oSQL = New CSQLInsertStringBuilder("Games")
            oSQL.AppendString("GameID", SQLString(GameID))
            oSQL.AppendString("GameType", SQLString(GameType))
            oSQL.AppendString("HomeRotationNumber", SQLString(HomeTeam.RotationNumber))
            oSQL.AppendString("AwayRotationNumber", SQLString(AwayTeam.RotationNumber))
            oSQL.AppendString("GameDate", SQLDate(GameDate))
            oSQL.AppendString("IsFirstHalfFinished", "'N'")
        End If

        oSQL.AppendString("IsForProp", "'Y'")
        oSQL.AppendString("PropDescription", SQLString(PropDescription))
        If ExtPropNumber <> "" Then
            oSQL.AppendString("ExtPropNumber", SQLString(ExtPropNumber))
        End If
        oSQL.AppendString("IsCircle", SQLString(IIf(IsCircle, "Y", "N")))
        oSQL.AppendString("IsAddedGame", SQLString(IIf(IsAddedGame, "Y", "N")))
        poDB.executeNonQuery(oSQL, "")
    End Sub

    Public Sub SaveLinesToDB(ByVal poDB As CSQLDBUtils, ByVal psGameID As String, ByVal psGameType As String)
        '' Copy CRIS, Pinnacle Lines to Vegas3,Vegas2
        Dim olstKeys(Lines.Keys.Count) As String
        Lines.Keys.CopyTo(olstKeys, 0)

        For Each sKey As String In olstKeys
            If SafeString(sKey) <> "" AndAlso Lines(sKey) IsNot Nothing Then
                Dim oLine As CLine = CType(Lines(sKey), CLine)
                If oLine.Bookmaker = "Pinnacle" Then
                    Dim oVegas2 As CLine = CType(oLine.Clone(), CLine)

                    oVegas2.Bookmaker = "Vegas2"
                    oVegas2.HomeSpreadMoney = VegasOdds(oVegas2.HomeSpreadMoney)
                    oVegas2.AwaySpreadMoney = VegasOdds(oVegas2.AwaySpreadMoney)
                    oVegas2.HomeMoneyLine = VegasOdds(oVegas2.HomeMoneyLine)
                    oVegas2.AwayMoneyLine = VegasOdds(oVegas2.AwayMoneyLine)
                    oVegas2.TotalPointsOverMoney = VegasOdds(oVegas2.TotalPointsOverMoney)
                    oVegas2.TotalPointsUnderMoney = VegasOdds(oVegas2.TotalPointsUnderMoney)
                    If oVegas2.DrawMoneyLine <> -1 Then
                        oVegas2.DrawMoneyLine = VegasOdds(oVegas2.DrawMoneyLine)
                    End If
                    oVegas2.PropMoneyLine = VegasOdds(oVegas2.PropMoneyLine)
                    oVegas2.AwayTeamTotalPointsOverMoney = VegasOdds(oVegas2.AwayTeamTotalPointsOverMoney)
                    oVegas2.AwayTeamTotalPointsUnderMoney = VegasOdds(oVegas2.AwayTeamTotalPointsUnderMoney)
                    oVegas2.HomeTeamTotalPointsOverMoney = VegasOdds(oVegas2.HomeTeamTotalPointsOverMoney)
                    oVegas2.HomeTeamTotalPointsUnderMoney = VegasOdds(oVegas2.HomeTeamTotalPointsUnderMoney)

                    Dim sHashKey As String = oVegas2.FeedSource & "_" & oVegas2.Bookmaker & "_" & oVegas2.Context
                    Lines(sHashKey) = oVegas2

                ElseIf oLine.Bookmaker = "CRIS" Then
                    Dim oVegas3 As CLine = CType(oLine.Clone(), CLine)

                    oVegas3.Bookmaker = "Vegas3"
                    oVegas3.HomeSpreadMoney = CrisToVegasOdds(oVegas3.HomeSpreadMoney, psGameType, oVegas3.Context)
                    oVegas3.AwaySpreadMoney = CrisToVegasOdds(oVegas3.AwaySpreadMoney, psGameType, oVegas3.Context)
                    oVegas3.HomeMoneyLine = oVegas3.HomeMoneyLine
                    oVegas3.AwayMoneyLine = oVegas3.AwayMoneyLine
                    oVegas3.TotalPointsOverMoney = CrisToVegasOdds(oVegas3.TotalPointsOverMoney, psGameType, oVegas3.Context)
                    oVegas3.TotalPointsUnderMoney = CrisToVegasOdds(oVegas3.TotalPointsUnderMoney, psGameType, oVegas3.Context)
                    oVegas3.DrawMoneyLine = oVegas3.DrawMoneyLine
                    oVegas3.PropMoneyLine = oVegas3.PropMoneyLine
                    oVegas3.AwayTeamTotalPointsOverMoney = oVegas3.AwayTeamTotalPointsOverMoney
                    oVegas3.AwayTeamTotalPointsUnderMoney = oVegas3.AwayTeamTotalPointsUnderMoney
                    oVegas3.HomeTeamTotalPointsOverMoney = oVegas3.HomeTeamTotalPointsOverMoney
                    oVegas3.HomeTeamTotalPointsUnderMoney = oVegas3.HomeTeamTotalPointsUnderMoney

                    Dim sHashKey As String = oVegas3.FeedSource & "_" & oVegas3.Bookmaker & "_" & oVegas3.Context
                    Lines(sHashKey) = oVegas3
                ElseIf oLine.Bookmaker = "5Dimes" Then
                    Dim oAction As CLine = CType(oLine.Clone(), CLine)
                    oAction.Bookmaker = "Action"
                    oAction.HomeSpreadMoney = CrisToVegasOdds(oAction.HomeSpreadMoney, psGameType, oAction.Context)
                    oAction.AwaySpreadMoney = CrisToVegasOdds(oAction.AwaySpreadMoney, psGameType, oAction.Context)
                    oAction.HomeMoneyLine = oAction.HomeMoneyLine
                    oAction.AwayMoneyLine = oAction.AwayMoneyLine
                    oAction.TotalPointsOverMoney = CrisToVegasOdds(oAction.TotalPointsOverMoney, psGameType, oAction.Context)
                    oAction.TotalPointsUnderMoney = CrisToVegasOdds(oAction.TotalPointsUnderMoney, psGameType, oAction.Context)
                    oAction.DrawMoneyLine = oAction.DrawMoneyLine
                    oAction.PropMoneyLine = oAction.PropMoneyLine
                    oAction.AwayTeamTotalPointsOverMoney = oAction.AwayTeamTotalPointsOverMoney
                    oAction.AwayTeamTotalPointsUnderMoney = oAction.AwayTeamTotalPointsUnderMoney
                    oAction.HomeTeamTotalPointsOverMoney = oAction.HomeTeamTotalPointsOverMoney
                    oAction.HomeTeamTotalPointsUnderMoney = oAction.HomeTeamTotalPointsUnderMoney

                    Dim sHashKey As String = oAction.FeedSource & "_" & oAction.Bookmaker & "_" & oAction.Context
                    Lines(sHashKey) = oAction
                End If

            End If
        Next

        For Each sKey As String In Lines.Keys
            Dim oLine As CLine = CType(Lines(sKey), CLine)
            Dim sGameID As String = GameID.ToString()
            If psGameID <> "" Then
                sGameID = psGameID
            End If

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(sGameID))
            oWhere.AppendANDCondition("FeedSource=" & SQLString(oLine.FeedSource))
            oWhere.AppendANDCondition("Context=" & SQLString(oLine.Context))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(oLine.Bookmaker))

            Dim oDT As DataTable = poDB.getDataTable("SELECT * FROM GameLines " & oWhere.SQL)
            Dim oSQL As ISQLEditStringBuilder = Nothing
            If oDT.Rows.Count <> 0 Then
                oSQL = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(oDT.Rows(0)("GameLineID")))
                Dim nOldHomeSpread As Double = SafeDouble(oDT.Rows(0)("HomeSpread"))
                Dim nOldHomeSpreadMoney As Double = SafeDouble(oDT.Rows(0)("HomeSpreadMoney"))
                Dim nOldAwaySpread As Double = SafeDouble(oDT.Rows(0)("AwaySpread"))
                Dim nOldAwaySpreadMoney As Double = SafeDouble(oDT.Rows(0)("AwaySpreadMoney"))
                Dim nOldTotalPoints As Double = SafeDouble(oDT.Rows(0)("TotalPoints"))
                Dim nOldTotalPointsOverMoney As Double = SafeDouble(oDT.Rows(0)("TotalPointsOverMoney"))
                Dim nOldTotalPointsUnderMoney As Double = SafeDouble(oDT.Rows(0)("TotalPointsUnderMoney"))

                oLine.HomeSpread = GetOdd(psGameType, nOldHomeSpread, oLine.HomeSpread)
                oLine.HomeSpreadMoney = GetJuice(psGameType, nOldHomeSpreadMoney, oLine.HomeSpreadMoney)
                oLine.AwaySpread = GetOdd(psGameType, nOldAwaySpread, oLine.AwaySpread)
                oLine.AwaySpreadMoney = GetJuice(psGameType, nOldAwaySpreadMoney, oLine.AwaySpreadMoney)
                oLine.TotalPoints = GetOdd(psGameType, nOldTotalPoints, oLine.TotalPoints)
                oLine.TotalPointsOverMoney = GetJuice(psGameType, nOldTotalPointsOverMoney, oLine.TotalPointsOverMoney)
                oLine.TotalPointsUnderMoney = GetJuice(psGameType, nOldTotalPointsUnderMoney, oLine.TotalPointsUnderMoney)
            Else
                'LogDebug(log, GameType & " : " & AwayTeam.Team & " @ " & HomeTeam.Team & " : " & sKey)
                oSQL = New CSQLInsertStringBuilder("GameLines")
                oSQL.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                oSQL.AppendString("GameID", SQLString(sGameID))
                oSQL.AppendString("FeedSource", SQLString(oLine.FeedSource))
                oSQL.AppendString("Bookmaker", SQLString(oLine.Bookmaker))
                oSQL.AppendString("Context", SQLString(oLine.Context))
                oSQL.AppendString("CreatedDate", SQLString(Now.ToUniversalTime()))
            End If
            With oSQL
                .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))

                If oLine.HomeSpread <> 0 Then
                    .AppendString("HomeSpread", oLine.HomeSpread.ToString)
                End If

                If oLine.HomeSpreadMoney <> 0 Then
                    .AppendString("HomeSpreadMoney", oLine.HomeSpreadMoney.ToString)
                End If

                If oLine.AwaySpread <> 0 Then
                    .AppendString("AwaySpread", oLine.AwaySpread.ToString)
                End If

                If oLine.AwaySpreadMoney <> 0 Then
                    .AppendString("AwaySpreadMoney", oLine.AwaySpreadMoney.ToString)
                End If

                If oLine.HomeMoneyLine <> 0 Then
                    .AppendString("HomeMoneyLine", oLine.HomeMoneyLine.ToString)
                End If

                If oLine.AwayMoneyLine <> 0 Then
                    .AppendString("AwayMoneyLine", oLine.AwayMoneyLine.ToString)
                End If

                If oLine.TotalPoints <> 0 Then
                    .AppendString("TotalPoints", oLine.TotalPoints.ToString)
                End If

                If oLine.TotalPointsOverMoney <> 0 Then
                    .AppendString("TotalPointsOverMoney", oLine.TotalPointsOverMoney.ToString)
                End If

                If oLine.TotalPointsUnderMoney <> 0 Then
                    .AppendString("TotalPointsUnderMoney", oLine.TotalPointsUnderMoney.ToString)
                End If

                If oLine.DrawMoneyLine <> -1 Then
                    .AppendString("DrawMoneyLine", SafeString(oLine.DrawMoneyLine))
                End If
                '' start insert teamtotal
                If oLine.AwayTeamTotalPoints <> 0 Then
                    .AppendString("AwayTeamTotalPoints", SafeString(oLine.AwayTeamTotalPoints))
                    If oLine.AwayTeamTotalPointsOverMoney <> 0 Then
                        .AppendString("AwayTeamTotalPointsOverMoney", SafeString(oLine.AwayTeamTotalPointsOverMoney))
                    End If
                    If oLine.AwayTeamTotalPointsUnderMoney <> 0 Then
                        .AppendString("AwayTeamTotalPointsUnderMoney", SafeString(oLine.AwayTeamTotalPointsUnderMoney))
                    End If
                End If
                If oLine.HomeTeamTotalPoints <> 0 Then
                    .AppendString("HomeTeamTotalPoints", SafeString(oLine.HomeTeamTotalPoints))
                    If oLine.HomeTeamTotalPointsOverMoney <> 0 Then
                        .AppendString("HomeTeamTotalPointsOverMoney", SafeString(oLine.HomeTeamTotalPointsOverMoney))
                    End If
                    If oLine.HomeTeamTotalPointsUnderMoney <> 0 Then
                        .AppendString("HomeTeamTotalPointsUnderMoney", SafeString(oLine.HomeTeamTotalPointsUnderMoney))
                    End If
                End If
                ''end insert teamtotal

                .AppendString("HPitcher", SQLString(oLine.HPitcher))
                .AppendString("APitcher", SQLString(oLine.APitcher))
                .AppendString("HPitcherRightHand", SQLString(IIf(oLine.HPitcherRightHand, "Y", "N")))
                .AppendString("APitcherRightHand", SQLString(IIf(oLine.APitcherRightHand, "Y", "N")))

                .AppendString("PropParticipantName", SQLString(oLine.PropParticipantName))
                .AppendString("PropRotationNumber", SQLString(oLine.PropRotationNumber))
                .AppendString("PropMoneyLine", SafeString(oLine.PropMoneyLine))
                .AppendString("PropMoneyLine", SafeString(oLine.PropMoneyLine))
                .AppendString("IsLockedByPreset", SQLString(IIf(oLine.IsLockedByPreset, "Y", "N")))
            End With
            poDB.executeNonQuery(oSQL, "")

        Next
    End Sub

    Public Sub SavePropLinesToDB(ByVal poDB As CSQLDBUtils, Optional ByVal psGameID As String = "")

        For Each sKey As String In Lines.Keys
            Dim oLine As CLine = CType(Lines(sKey), CLine)
            Dim sGameID As String = GameID.ToString()
            If psGameID <> "" Then
                sGameID = psGameID
            End If

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(sGameID))
            oWhere.AppendANDCondition("FeedSource=" & SQLString(oLine.FeedSource))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(oLine.Bookmaker))
            oWhere.AppendANDCondition("PropParticipantName=" & SQLString(oLine.PropParticipantName))

            Dim sID As String = SafeString(poDB.getScalerValue("SELECT GameLineID FROM GameLines " & oWhere.SQL))
            Dim oSQL As ISQLEditStringBuilder = Nothing
            If sID <> "" Then
                oSQL = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(sID))
            Else
                'LogDebug(log, GameType & " : " & AwayTeam.Team & " @ " & HomeTeam.Team & " : " & sKey)
                oSQL = New CSQLInsertStringBuilder("GameLines")
                oSQL.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                oSQL.AppendString("GameID", SQLString(sGameID))
                oSQL.AppendString("FeedSource", SQLString(oLine.FeedSource))
                oSQL.AppendString("Bookmaker", SQLString(oLine.Bookmaker))
                oSQL.AppendString("Context", SQLString(oLine.Context))
            End If

            With oSQL
                .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))

                If oLine.HomeSpread <> 0 Then
                    .AppendString("HomeSpread", oLine.HomeSpread.ToString)
                End If

                If oLine.HomeSpreadMoney <> 0 Then
                    .AppendString("HomeSpreadMoney", oLine.HomeSpreadMoney.ToString)
                End If

                If oLine.AwaySpread <> 0 Then
                    .AppendString("AwaySpread", oLine.AwaySpread.ToString)
                End If

                If oLine.AwaySpreadMoney <> 0 Then
                    .AppendString("AwaySpreadMoney", oLine.AwaySpreadMoney.ToString)
                End If

                If oLine.HomeMoneyLine <> 0 Then
                    .AppendString("HomeMoneyLine", oLine.HomeMoneyLine.ToString)
                End If

                If oLine.AwayMoneyLine <> 0 Then
                    .AppendString("AwayMoneyLine", oLine.AwayMoneyLine.ToString)
                End If

                If oLine.TotalPoints <> 0 Then
                    .AppendString("TotalPoints", oLine.TotalPoints.ToString)
                End If

                If oLine.TotalPointsOverMoney <> 0 Then
                    .AppendString("TotalPointsOverMoney", oLine.TotalPointsOverMoney.ToString)
                End If

                If oLine.TotalPointsUnderMoney <> 0 Then
                    .AppendString("TotalPointsUnderMoney", oLine.TotalPointsUnderMoney.ToString)
                End If

                If oLine.DrawMoneyLine <> -1 Then
                    .AppendString("DrawMoneyLine", SafeString(oLine.DrawMoneyLine))
                End If
                .AppendString("HPitcher", SQLString(oLine.HPitcher))
                .AppendString("APitcher", SQLString(oLine.APitcher))
                .AppendString("HPitcherRightHand", SQLString(IIf(oLine.HPitcherRightHand, "Y", "N")))
                .AppendString("APitcherRightHand", SQLString(IIf(oLine.APitcherRightHand, "Y", "N")))

                .AppendString("PropParticipantName", SQLString(oLine.PropParticipantName))
                .AppendString("PropRotationNumber", SQLString(oLine.PropRotationNumber))

                '' if PropMoneyLine is positive, mulyiply to 0.8, if negative, multiply to 1.1
                If oLine.PropMoneyLine > 0 Then
                    oLine.PropMoneyLine = Math.Round(oLine.PropMoneyLine * 0.85, 0)
                Else
                    oLine.PropMoneyLine = Math.Round(oLine.PropMoneyLine * 1.1)
                End If
                .AppendString("PropMoneyLine", SafeString(oLine.PropMoneyLine))
            End With
            poDB.executeNonQuery(oSQL, "")
        Next
    End Sub

    Private Function GetOdd(ByVal psGameType As String, ByVal pnOldOdd As Double, ByVal pnNewOdd As Double) As Double
        ' DO NOT update if Odd increase or decrease over 0.5 point
        ' Apply for Soccer game only
        If IsSoccer(psGameType) AndAlso pnOldOdd <> 0 Then
            If Math.Abs(pnOldOdd - pnNewOdd) > 0.5 Then
                Return pnOldOdd
            Else
                Return pnNewOdd
            End If
        End If

        Return pnNewOdd
    End Function

    Private Function GetJuice(ByVal psGameType As String, ByVal pnOldJuice As Double, ByVal pnNewJuice As Double) As Double
        ' DO NOT update if Juice increase or decrease over 5 percentage
        ' Apply for Soccer game only
        If IsSoccer(psGameType) AndAlso pnOldJuice <> 0 Then
            If Math.Abs(pnOldJuice - pnNewJuice) > 5 Then
                Return pnOldJuice
            Else
                Return pnNewJuice
            End If
        End If

        Return pnNewJuice
    End Function

    Public Enum FindTeamBy
        NSS = 0
        TeamName = 1
        Both = 2
    End Enum
End Class

Public Class CGameTeam
    Public Team As String
    Public RotationNumber As Integer
End Class

