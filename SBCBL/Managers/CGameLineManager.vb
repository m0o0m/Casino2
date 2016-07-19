Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CGameLineManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CGameLineManager))

     

        Public Function SaveManualLine(ByVal poGameLine As CGameLine) As Boolean
            Dim bResult As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(poGameLine.GameID))
            oWhere.AppendANDCondition("FeedSource=" & SQLString(poGameLine.FeedSource))
            oWhere.AppendANDCondition("Context=" & SQLString(poGameLine.Context))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(poGameLine.Bookmaker))

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oEdit As ISQLEditStringBuilder

            Try
                Dim sID As String = SafeString(oDB.getScalerValue("SELECT GameLineID FROM GameLines " & oWhere.SQL))
                If sID <> "" Then
                    oEdit = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(sID))
                Else
                    oEdit = New CSQLInsertStringBuilder("GameLines")
                    oEdit.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                    oEdit.AppendString("GameID", SQLString(poGameLine.GameID))
                    oEdit.AppendString("FeedSource", SQLString(poGameLine.FeedSource))
                    oEdit.AppendString("Bookmaker", SQLString(poGameLine.Bookmaker))
                    oEdit.AppendString("Context", SQLString(poGameLine.Context))
                End If

                With oEdit
                    .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))

                    .AppendString("HomeSpread", SQLDouble(poGameLine.HomeSpread))
                    .AppendString("HomeSpreadMoney", SQLDouble(poGameLine.HomeSpreadMoney))

                    .AppendString("AwaySpread", SQLDouble(poGameLine.AwaySpread))
                    .AppendString("AwaySpreadMoney", SQLDouble(poGameLine.AwaySpreadMoney))

                    .AppendString("HomeMoneyLine", SQLDouble(poGameLine.HomeMoneyLine))
                    .AppendString("AwayMoneyLine", SQLDouble(poGameLine.AwayMoneyLine))

                    .AppendString("TotalPoints", SQLDouble(poGameLine.TotalPoints))
                    .AppendString("TotalPointsOverMoney", SQLDouble(poGameLine.TotalPointsOverMoney))
                    .AppendString("TotalPointsUnderMoney", SQLDouble(poGameLine.TotalPointsUnderMoney))

                End With

                log.Debug("Save Manual GameLine. SQL: " & oEdit.SQL)
                oDB.executeNonQuery(oEdit, "")
                bResult = True
            Catch ex As Exception
                log.Error("Fail to Save Manual GameLine: " & ex.Message, ex)

            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Sub UpdateGame(ByVal psGameID As String, ByVal psGameDate As String, ByVal psSecondHalfTime As String, ByVal pAwayRotationNumber As Integer, ByVal pHomeRotationNumber As Integer, ByVal pDrawRotationNumber As Integer, ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psGameType As String, ByVal psAwayScore As String, ByVal psHomeScore As String, ByVal psContext As String)
            Dim bResult As Boolean = False
            Dim oUpdateGameLine = New CSQLUpdateStringBuilder("Games", String.Format("WHERE GameID={0}", SQLString(psGameID)))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdateGameLine
                    .AppendString("AwayRotationNumber", SQLString(pAwayRotationNumber))
                    .AppendString("HomeRotationNumber", SQLString(pHomeRotationNumber))
                    .AppendString("AwayTeam", SQLString(psAwayTeam))
                    .AppendString("HomeTeam", SQLString(psHomeTeam))
                    .AppendString("GameDate", SQLString(psGameDate))
                    .AppendString("SecondHalfTime", SQLString(psSecondHalfTime))
                    If pDrawRotationNumber <> 0 Then
                        .AppendString("DrawRotationNumber", SQLString(pDrawRotationNumber))
                    End If
                    If Not String.IsNullOrEmpty(psAwayScore) AndAlso Not String.IsNullOrEmpty(psHomeScore) Then
                        If psContext.Equals("1H", StringComparison.CurrentCultureIgnoreCase) Then
                            .AppendString("IsFirstHalfFinished", "'Y'")
                            .AppendString("AwayFirstHalfScore", SQLDouble(psAwayScore))
                            .AppendString("HomeFirstHalfScore", SQLDouble(psHomeScore))
                        ElseIf psContext.Equals("2H", StringComparison.CurrentCultureIgnoreCase) Then
                            .AppendString("AwaySecondQScore", SQLDouble(psAwayScore))
                            .AppendString("HomeSecondQScore", SQLDouble(psHomeScore))
                        Else
                            .AppendString("AwayScore", SQLDouble(psAwayScore))
                            .AppendString("HomeScore", SQLDouble(psHomeScore))
                            .AppendString("GameStatus", "'Final'")
                        End If


                    End If
                End With
                log.Debug("update Manual new Game  .  SQL: " & oUpdateGameLine.SQL)
                odbSQL.executeNonQuery(oUpdateGameLine, "")
                bResult = True
            Catch ex As Exception
                log.Error("Fail to update new GameLine: " & ex.Message, ex)
            Finally
                If odbSQL IsNot Nothing Then odbSQL.closeConnection()
            End Try
        End Sub
        Public Function SaveNewGame(ByVal psGameDate As String, ByVal psSecondHalfTime As String, ByVal pAwayRotationNumber As Integer, ByVal pHomeRotationNumber As Integer, ByVal pDrawRotationNumber As Integer, ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psGameType As String) As String
            Dim bResult As Boolean = False
            Dim sGameID As String = Guid.NewGuid.ToString()
            Dim oInsertGameLine As New CSQLInsertStringBuilder("Games")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oInsertGameLine
                    .AppendString("GameID", SQLString(sGameID))
                    .AppendString("AwayRotationNumber", SQLString(pAwayRotationNumber))
                    .AppendString("HomeRotationNumber", SQLString(pHomeRotationNumber))
                    .AppendString("AwayTeam", SQLString(psAwayTeam))
                    .AppendString("HomeTeam", SQLString(psHomeTeam))
                    .AppendString("GameDate", SQLString(psGameDate))
                    .AppendString("GameType", SQLString(psGameType))
                    .AppendString("SecondHalfTime", SQLString(psSecondHalfTime))
                    If pDrawRotationNumber <> 0 Then
                        .AppendString("DrawRotationNumber", SQLString(pDrawRotationNumber))
                    End If
                    .AppendString("IsForProp", SQLString("N"))
                End With
                log.Debug("Save Manual new  Game  .  SQL: " & oInsertGameLine.SQL)
                odbSQL.executeNonQuery(oInsertGameLine, "")
                bResult = True
            Catch ex As Exception
                log.Error("Fail to Save new GameLine: " & ex.Message, ex)
            Finally
                If odbSQL IsNot Nothing Then odbSQL.closeConnection()
            End Try
            Return sGameID
        End Function
        Public Function GetGameLinesDataTable(ByVal psGameID As String) As DataTable
            Dim odtGameLines As DataTable = Nothing

            Dim sSQL As String = "SELECT * FROM GameLines WHERE GameID=" & SQLString(psGameID)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtGameLines = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of game lines. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtGameLines
        End Function

        Public Function GetGameLine(ByVal psGameID As String) As CGameLine
            Dim odtGameLines As DataTable = GetGameLinesDataTable(psGameID)

            If odtGameLines IsNot Nothing Then
                For Each odrGameLine As DataRow In odtGameLines.Rows
                    If SafeString(odrGameLine("Context")).Equals("Current", StringComparison.CurrentCultureIgnoreCase) Then
                        Return New CGameLine(odrGameLine)
                    End If
                Next
            End If

            Return Nothing
        End Function

        Public Function GetGameLines(ByVal psGameID As String) As List(Of CGameLine)
            Dim odtGameLines As DataTable = GetGameLinesDataTable(psGameID)
            Dim oGameLines As List(Of CGameLine) = Nothing

            If odtGameLines IsNot Nothing Then
                oGameLines = New List(Of CGameLine)

                For Each odrGameLine As DataRow In odtGameLines.Rows
                    oGameLines.Add(New CGameLine(odrGameLine))
                Next
            End If

            Return oGameLines
        End Function

        Public Function SaveLine(ByVal poGameLine As CGameLine) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(poGameLine.GameID))
            oWhere.AppendANDCondition("FeedSource=" & SQLString(poGameLine.FeedSource))
            oWhere.AppendANDCondition("Context=" & SQLString(poGameLine.Context))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(poGameLine.Bookmaker))

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oSQL As ISQLEditStringBuilder = Nothing

            Try
                Dim sID As String = SafeString(odbSQL.getScalerValue("SELECT GameLineID FROM GameLines " & oWhere.SQL))
                If sID <> "" Then
                    oSQL = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(sID))
                Else
                    oSQL = New CSQLInsertStringBuilder("GameLines")
                    oSQL.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                    oSQL.AppendString("GameID", SQLString(poGameLine.GameID))
                    oSQL.AppendString("FeedSource", SQLString(poGameLine.FeedSource))
                    oSQL.AppendString("Bookmaker", SQLString(poGameLine.Bookmaker))
                    oSQL.AppendString("Context", SQLString(poGameLine.Context))
                End If

                With oSQL
                    .AppendString("LastUpdated", SQLDate(GetEasternDate()))

                    If poGameLine.HomeSpread <> 0 Then
                        .AppendString("HomeSpread", poGameLine.HomeSpread.ToString)
                    End If

                    If poGameLine.HomeSpreadMoney <> 0 Then
                        .AppendString("HomeSpreadMoney", poGameLine.HomeSpreadMoney.ToString)
                    End If

                    If poGameLine.AwaySpread <> 0 Then
                        .AppendString("AwaySpread", poGameLine.AwaySpread.ToString)
                    End If

                    If poGameLine.AwaySpreadMoney <> 0 Then
                        .AppendString("AwaySpreadMoney", poGameLine.AwaySpreadMoney.ToString)
                    End If

                    If poGameLine.HomeMoneyLine <> 0 Then
                        .AppendString("HomeMoneyLine", poGameLine.HomeMoneyLine.ToString)
                    End If

                    If poGameLine.AwayMoneyLine <> 0 Then
                        .AppendString("AwayMoneyLine", poGameLine.AwayMoneyLine.ToString)
                    End If

                    If poGameLine.TotalPoints <> 0 Then
                        .AppendString("TotalPoints", poGameLine.TotalPoints.ToString)
                    End If

                    If poGameLine.TotalPointsOverMoney <> 0 Then
                        .AppendString("TotalPointsOverMoney", poGameLine.TotalPointsOverMoney.ToString)
                    End If

                    If poGameLine.TotalPointsUnderMoney <> 0 Then
                        .AppendString("TotalPointsUnderMoney", poGameLine.TotalPointsUnderMoney.ToString)
                    End If

                End With
                odbSQL.executeNonQuery(oSQL, "")
                Return True
            Catch ex As Exception
                log.Error("Can't Save Line: " & ex.Message, ex)
                Return False

            Finally
                If odbSQL IsNot Nothing Then odbSQL.closeConnection()
            End Try
        End Function

        Public Function DeleteGameLine(ByVal psID As String) As Boolean
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "delete from Gamelines where gamelineID = " & SQLString(psID)

            Try
                odbSQL.executeNonQuery(sSQL)
                Return True
            Catch ex As Exception
                log.Error("Can't delete line " & psID & ". SQL: " & ex.Message, ex)
                Return False

            Finally
                If odbSQL IsNot Nothing Then odbSQL.closeConnection()
            End Try
        End Function


        Public Function DeleteGameQuarterLine(ByVal psID As String, ByVal psBookmaker As String) As Boolean
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("gamelineID=" & SQLString(psID))
            oWhere.AppendANDCondition("BookMaker=" & SQLString(psBookmaker))
            Dim sSQL As String = "delete from Gamelines " & oWhere.SQL
            Try
                odbSQL.executeNonQuery(sSQL)
                Return True
            Catch ex As Exception
                log.Error("Can't delete line " & psID & ". SQL: " & ex.Message, ex)
                Return False

            Finally
                If odbSQL IsNot Nothing Then odbSQL.closeConnection()
            End Try
        End Function


        Public Function UpdatePropStatus(ByVal psGameLineID As String, ByVal psPropStatus As String, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                If psGameLineID <> "" Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameLineID =" & SQLString(psGameLineID))
                    Dim oUpdate As New CSQLUpdateStringBuilder("GameLines", oWhere.SQL)
                    oUpdate.AppendString("PropStatus", SQLString(psPropStatus))
                    log.Debug("Update GameLines PropStatus. SQL: " & oUpdate.SQL)
                    oDB.executeNonQuery(oUpdate, psChangeBy)
                    bResult = True
                End If
            Catch ex As Exception
                log.Error("Cannot Update Game PropStatus. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function SaveManualLinePinaccles(ByVal poGameLine As CGameLine, ByVal psSuperID As String) As Boolean
            Dim bResult As Boolean = False


            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oEdit As ISQLEditStringBuilder

            Try
                Dim oAgentManager As New CAgentManager()
                Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperID, Nothing)

                For Each dr As DataRow In oData.Rows

                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID=" & SQLString(poGameLine.GameID))
                    oWhere.AppendANDCondition("FeedSource=" & SQLString(poGameLine.FeedSource))
                    oWhere.AppendANDCondition("Context=" & SQLString(poGameLine.Context))
                    oWhere.AppendANDCondition("Bookmaker=" & SQLString(poGameLine.Bookmaker))
                    Dim sID As String = SafeString(oDB.getScalerValue("SELECT GameLineID FROM GameLines " & oWhere.SQL))
                    If sID <> "" Then
                        oEdit = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(sID))
                    Else

                        oEdit = New CSQLInsertStringBuilder("GameLines")
                        oEdit.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                        oEdit.AppendString("GameID", SQLString(poGameLine.GameID))
                        oEdit.AppendString("FeedSource", SQLString(poGameLine.FeedSource))
                        oEdit.AppendString("Bookmaker", SQLString(poGameLine.Bookmaker))
                        oEdit.AppendString("Context", SQLString(poGameLine.Context))
                    End If

                    With oEdit
                        .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))

                        .AppendString("HomeSpread", SQLDouble(poGameLine.HomeSpread))
                        .AppendString("HomeSpreadMoney", SQLDouble(poGameLine.HomeSpreadMoney))

                        .AppendString("AwaySpread", SQLDouble(poGameLine.AwaySpread))
                        .AppendString("AwaySpreadMoney", SQLDouble(poGameLine.AwaySpreadMoney))

                        .AppendString("HomeMoneyLine", SQLDouble(poGameLine.HomeMoneyLine))
                        .AppendString("AwayMoneyLine", SQLDouble(poGameLine.AwayMoneyLine))

                        .AppendString("TotalPoints", SQLDouble(poGameLine.TotalPoints))
                        .AppendString("TotalPointsOverMoney", SQLDouble(poGameLine.TotalPointsOverMoney))
                        .AppendString("TotalPointsUnderMoney", SQLDouble(poGameLine.TotalPointsUnderMoney))

                    End With

                    log.Debug("Save Manual GameLine. SQL: " & oEdit.SQL)
                    oDB.executeNonQuery(oEdit, "")
                    bResult = True
                Next

            Catch ex As Exception
                log.Error("Fail to Save Manual GameLine: " & ex.Message, ex)

            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function SaveManualLine(ByVal poGameLine As CGameLine, ByVal psSuperID As String) As Boolean
            Dim bResult As Boolean = False


            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oEdit As ISQLEditStringBuilder

            Try
                Dim oAgentManager As New CAgentManager()
                Dim oData As DataTable = oAgentManager.GetAgentsBySuperID(psSuperID, Nothing)

                For Each dr As DataRow In oData.Rows

                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition("GameID=" & SQLString(poGameLine.GameID))
                    oWhere.AppendANDCondition("FeedSource=" & SQLString(poGameLine.FeedSource))
                    oWhere.AppendANDCondition("Context=" & SQLString(poGameLine.Context))
                    oWhere.AppendANDCondition("Bookmaker=" & SQLString(SafeString(dr("AgentID"))))
                    Dim sID As String = SafeString(oDB.getScalerValue("SELECT GameLineID FROM GameLines " & oWhere.SQL))
                    If sID <> "" Then
                        oEdit = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(sID))
                    Else

                        oEdit = New CSQLInsertStringBuilder("GameLines")
                        oEdit.AppendString("GameLineID", SQLString(Guid.NewGuid()))
                        oEdit.AppendString("GameID", SQLString(poGameLine.GameID))
                        oEdit.AppendString("FeedSource", SQLString(poGameLine.FeedSource))
                        oEdit.AppendString("Bookmaker", SQLString(SafeString(dr("AgentID"))))
                        oEdit.AppendString("Context", SQLString(poGameLine.Context))
                    End If

                    With oEdit
                        .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))

                        .AppendString("HomeSpread", SQLDouble(poGameLine.HomeSpread))
                        .AppendString("HomeSpreadMoney", SQLDouble(poGameLine.HomeSpreadMoney))

                        .AppendString("AwaySpread", SQLDouble(poGameLine.AwaySpread))
                        .AppendString("AwaySpreadMoney", SQLDouble(poGameLine.AwaySpreadMoney))

                        .AppendString("HomeMoneyLine", SQLDouble(poGameLine.HomeMoneyLine))
                        .AppendString("AwayMoneyLine", SQLDouble(poGameLine.AwayMoneyLine))

                        .AppendString("TotalPoints", SQLDouble(poGameLine.TotalPoints))
                        .AppendString("TotalPointsOverMoney", SQLDouble(poGameLine.TotalPointsOverMoney))
                        .AppendString("TotalPointsUnderMoney", SQLDouble(poGameLine.TotalPointsUnderMoney))

                    End With

                    log.Debug("Save Manual GameLine. SQL: " & oEdit.SQL)
                    oDB.executeNonQuery(oEdit, "")
                    bResult = True
                Next

            Catch ex As Exception
                log.Error("Fail to Save Manual GameLine: " & ex.Message, ex)

            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function RemoveWarning(ByVal psGameLineID As String) As Boolean
            Dim sSQL As String = "update gamelines set Iswarn= null,WarnReason= null where gamelineid= " & SQLString(psGameLineID)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                oDB.executeNonQuery(sSQL)
                Return True
            Catch ex As Exception
                log.Error("Can't Remove Warning: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function GetColumnOnTableGames() As List(Of String)
            Dim sSQL As String = "select column_name AS Columns " & vbCrLf & _
                   " from information_schema.columns where table_name = 'GameLines'"
            log.Debug("Get Column name in table GameLine. SQL: " & sSQL)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim lstFieldName As New List(Of String)
            Try
                Dim odtColumn As DataTable = oDB.getDataTable(sSQL)

                For Each odr As DataRow In odtColumn.Rows
                    lstFieldName.Add(SafeString(odr(0)))
                Next
                Return lstFieldName
            Catch ex As Exception
                log.Error("Cannot Get Column name in table GameLines. SQL " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return lstFieldName
        End Function

        Public Function CheckExistsGameLines(ByVal psGameID As String, ByVal psGameDate As String, ByVal psGameType As String, ByVal psHomeTeam As String _
                                             , ByVal psAwayTeam As String, ByVal psContext As String, ByVal psBookmaker As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Games.GameID=" & SQLString(psGameID))
            oWhere.AppendANDCondition("GameDate=" & SQLString(psGameDate))
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition("HomeTeam=" & SQLString(psHomeTeam))
            oWhere.AppendANDCondition("AwayTeam=" & SQLString(psAwayTeam))
            oWhere.AppendANDCondition("Context=" & SQLString(psContext))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(psBookmaker))
            Dim sSQL As String = "select ManualSetting from GameLines inner join Games on GameLines.GameID=Games.gameID  " & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim odtColumn As DataTable = oDB.getDataTable(sSQL)
                Return odtColumn.Rows.Count > 0
            Catch ex As Exception
                log.Error("Check Exist Agent Manual Fail. SQL " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function

        Public Function InsertGameLineManual(ByVal psGameLineID As String, ByVal psBookmaker As String) As String
            Dim lstFieldName As List(Of String)
            Dim bSuccess As Boolean = False
            Dim sGameLineID As String = newGUID()
            lstFieldName = GetColumnOnTableGames()
            lstFieldName.Remove("Bookmaker")
            Dim sFieldInsert As String = Join(lstFieldName.ToArray(), ",")
            lstFieldName.Remove("GameLineID")
            Dim sSQL As String = String.Format("insert into GameLines({0},Bookmaker) select '{4}' as GameLineID,{1},'{2}' as Bookmaker from GameLines where GameLineID='{3}'", sFieldInsert, Join(lstFieldName.ToArray(), ","), psBookmaker, psGameLineID, sGameLineID)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            log.Debug("Insert Game Line Agent Manual :" & sSQL)
            Try
                odbSQL.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Error trying to insert Agent manual  game Line. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return sGameLineID
        End Function

        Public Function UpdateBlockGame(ByVal psGameLineID As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oEdit As ISQLEditStringBuilder

            Try
                oEdit = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(psGameLineID))
                With oEdit
                    .AppendString("LastUpdated", SQLDate(std.GetEasternDate()))
                    .AppendString("ManualSetting", SQLString("Y"))
                    .AppendString("GameLineOff", SQLString("Y"))
                End With
                log.Debug("update Manual GameLine. SQL: " & oEdit.SQL)
                oDB.executeNonQuery(oEdit, "")
                bResult = True
            Catch ex As Exception
                log.Error("Fail to update Manual GameLine: " & ex.Message, ex)

            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function


        Public Function UpdateManualLine(ByVal poGameLine As CGameLine) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim oEdit As ISQLEditStringBuilder

            Try
                oEdit = New CSQLUpdateStringBuilder("GameLines", "WHERE GameLineID=" & SQLString(poGameLine.GameLineID))
                With oEdit
                    .AppendString("LastUpdated", SQLDate(Date.Now.ToUniversalTime()))
                    .AppendString("ManualSetting", SQLString(IIf(poGameLine.ManualSetting, "Y", "N")))
                    .AppendString("GameLineOff", SQLString(IIf(poGameLine.GameLineOff, "Y", "N")))
                    .AppendString("IsCircle", SQLString(IIf(poGameLine.IsCircle, "Y", "N")))
                    .AppendString("HomeSpread", SQLDouble(poGameLine.HomeSpread))
                    .AppendString("HomeSpreadMoney", SQLDouble(poGameLine.HomeSpreadMoney))

                    .AppendString("AwaySpread", SQLDouble(poGameLine.AwaySpread))
                    .AppendString("AwaySpreadMoney", SQLDouble(poGameLine.AwaySpreadMoney))

                    .AppendString("HomeMoneyLine", SQLDouble(poGameLine.HomeMoneyLine))
                    .AppendString("AwayMoneyLine", SQLDouble(poGameLine.AwayMoneyLine))

                    .AppendString("TotalPoints", SQLDouble(poGameLine.TotalPoints))
                    .AppendString("TotalPointsOverMoney", SQLDouble(poGameLine.TotalPointsOverMoney))
                    .AppendString("TotalPointsUnderMoney", SQLDouble(poGameLine.TotalPointsUnderMoney))

                End With

                log.Debug("update Manual GameLine. SQL: " & oEdit.SQL)
                oDB.executeNonQuery(oEdit, "")
                bResult = True
            Catch ex As Exception
                log.Error("Fail to update Manual GameLine: " & ex.Message, ex)

            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function ExistAgentManual(ByVal psGameLineID As String, ByVal psBookmaker As String) As Boolean

            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("GameLineID=" & SQLString(psGameLineID))
            oWhere.AppendANDCondition("Bookmaker=" & SQLString(psBookmaker))
            Dim sSQL As String = "select ManualSetting from GameLines " & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim odtColumn As DataTable = oDB.getDataTable(sSQL)
                Return odtColumn.Rows.Count > 0
            Catch ex As Exception
                log.Error("Check Exist Agent Manual Fail. SQL " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False

        End Function

        Public Function GetGameLinesByID(ByVal psGameLineID As String) As DataRow
            Dim odtGameLines As DataTable = Nothing

            Dim sSQL As String = "SELECT * FROM GameLines inner join Games on GameLines.GameID=Games.GameID WHERE GameLineID=" & SQLString(psGameLineID)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtGameLines = odbSQL.getDataTable(sSQL)
                If odtGameLines IsNot Nothing AndAlso odtGameLines.Rows.Count > 0 Then
                    Return odtGameLines.Rows(0)
                End If
            Catch ex As Exception
                log.Error("Cannot get the list of game lines. SQL: " & sSQL, ex)
                Return Nothing
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

    End Class
End Namespace

