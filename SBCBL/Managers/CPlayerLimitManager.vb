Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports WebsiteLibrary.DBUtils
Imports System.Data
Namespace Managers
    Public Class CPlayerLimitManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetGameType(ByVal psKey As String) As DataTable
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim sSQL As String = "SELECT * FROM syssettings WHERE SubCategory=(SELECT cast(SysSettingID as varchar(100)) " & _
                                                                                                "FROM syssettings WHERE [key]=" & SQLString(psKey) & " AND category='sbs gametype' AND isnull(subcategory,'')='') " & _
                                                                                  "AND category='sbs gametype'"
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                LogError(log, "Get Game Type ", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetData(ByVal pGameTypes As List(Of String)) As CPlayerTemplateLimitList
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim sSQL As String = "SELECT * " & _
                                                    "FROM PlayerTemplateLimits WHERE GameType in ('" & Join(pGameTypes.ToArray, "','") & "')"

                Dim odt As DataTable = odbSQL.getDataTable(sSQL)
                Dim oPlayerTemplateLimitList As New CPlayerTemplateLimitList
                For Each oRow As DataRow In odt.Rows
                    oPlayerTemplateLimitList.Add(New CPlayerTemplateLimit(oRow))
                Next
                Return oPlayerTemplateLimitList
            Catch ex As Exception
                LogError(log, "Get limit template by game type ", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetDataTable(ByVal poPlayerTemplateID As String, ByVal pGameTypes As List(Of String)) As DataTable

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim sSQL As String = "SELECT MaxSingle,MaxParlay,MaxTeaser,MaxReverse,GameType,Context " & _
                                                   "FROM PlayerTemplateLimits WHERE GameType in ('" & Join(pGameTypes.ToArray, "','") & "') AND " & _
                                                   "PlayerTemplateID='" & poPlayerTemplateID & "'"
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                LogError(log, "Get DataTable of Player template", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function UpdateData(ByVal poSaveTable As DataTable, ByVal poPlayerTemplateLimitsID As String, ByVal psGameType As String) As Boolean
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try

                Dim sql = "DELETE FROM PlayerTemplateLimits WHERE PlayerTemplateID='" & poPlayerTemplateLimitsID & "'" & _
                                                                      " AND GameType IN(" & psGameType & "')"
                LogDebug(log, "defore delete" & sql)
                odbSQL.executeNonQuery(sql)
                LogDebug(log, "after delete" & sql)
                For Each oDR As DataRow In poSaveTable.Rows
                    Dim sSQL = "Insert into PlayerTemplateLimits (PlayerTemplateLimitID,PlayerTemplateID,GameType,Context,MaxParlay,MaxTeaser,MaxReverse,Total,ML,MaxSingle) " & _
                               "Values('" & SafeString(oDR("PlayerTemplateLimitID")) & "','" & SafeString(oDR("PlayerTemplateID")) & "','" & _
                                           SafeString(oDR("GameType")) & "','" & SafeString(oDR("Context")) & "','" & _
                                           SafeString(oDR("MaxSingle")) & "','" & SafeString(oDR("MaxSingle")) & "','" & _
                                             SafeString(oDR("MaxSingle")) & "','" & SafeString(oDR("MaxSingle")) & "','" & _
                                           SafeString(oDR("MaxSingle")) & "','" & SafeString(oDR("MaxSingle")) & "')"
                    LogDebug(log, "update data" & sSQL)
                    odbSQL.executeNonQuery(sSQL)
                    'Dim band As Boolean = False
                    'Dim SQL As String = "Update  PlayerTemplateLimits set "
                    'SQL += " MaxSingle= '" & SafeString(oDR("MaxSingle")) & "'"
                    'SQL += ", MaxParlay= '" & SafeString(oDR("MaxSingle")) & "'"
                    'SQL += ", MaxTeaser= '" & SafeString(oDR("MaxSingle")) & "'"
                    'SQL += ", MaxReverse= '" & SafeString(oDR("MaxSingle")) & "'"
                    'SQL += " WHERE PlayerTemplateID='" & poPlayerTemplateLimitsID & "'" & _
                    '                                                    " AND GameType IN(" & psGameType & "')"
                    'LogDebug(log, "update data" & SQL)
                    'odbSQL.executeNonQuery(SQL)
                    ' LogDebug(log, "Save Template Limit SQL: " & sSQL)
                Next
                Return True
            Catch ex As Exception
                LogError(log, "Save Player Limit Seting", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return False
        End Function

        Public Function SaveData(ByVal poSaveTable As DataTable, ByVal poPlayerTemplateLimitsID As String, ByVal psGameType As String) As Boolean
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odbSQL.executeNonQuery("DELETE FROM PlayerTemplateLimits WHERE PlayerTemplateID='" & poPlayerTemplateLimitsID & "'" & _
                                                                         " AND GameType IN(" & psGameType & "')")
                For Each oDR As DataRow In poSaveTable.Rows
                    Dim sSQL = "Insert into PlayerTemplateLimits (PlayerTemplateLimitID,PlayerTemplateID,GameType,Context,MaxParlay,MaxTeaser,MaxReverse,MaxSingle) " & _
                                "Values('" & SafeString(oDR("PlayerTemplateLimitID")) & "','" & SafeString(oDR("PlayerTemplateID")) & "','" & _
                                            SafeString(oDR("GameType")) & "','" & SafeString(oDR("Context")) & "','" & _
                                            SafeString(oDR("MaxParlay")) & "','" & SafeString(oDR("MaxTeaser")) & "','" & _
                                            SafeString(oDR("MaxReverse")) & "','" & SafeString(oDR("MaxSingle")) & "')"
                    odbSQL.executeNonQuery(sSQL)
                    LogDebug(log, "Save Template Limit SQL: " & sSQL)
                Next
                Return True
            Catch ex As Exception
                LogError(log, "Save Player Limit Seting", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return False
        End Function

        Public Function GetDataTableJuice(ByVal poPlayerTemplateID As String, ByVal pGameTypes As List(Of String)) As DataTable

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim sSQL As String = "SELECT PlayerTemplateLimitID, Spread,Total,ML,GameType,Context " & _
                                                   "FROM PlayerTemplateLimits WHERE MaxParlay is NULL and MaxTeaser is NULL and MaxReverse is NULL and GameType in ('" & Join(pGameTypes.ToArray, "','") & "') AND " & _
                                                   "PlayerTemplateID='" & poPlayerTemplateID & "'"
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                LogError(log, "Get DataTable of Player template", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function getMinStraightBet(ByVal poPlayerTemplateID As String, ByVal pGameTypes As String, ByVal psContext As String, ByVal psBetType As String) As Double
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("PlayerTemplateID='" & poPlayerTemplateID & "'")
                'oWhere.AppendANDCondition("Context in ('" & Join(plstContext.ToArray, "','") & "')")
                'oWhere.AppendANDCondition("GameType in ('" & Join(pGameTypes.ToArray, "','") & "')")
                oWhere.AppendANDCondition("Context = " & SQLString(psContext))
                oWhere.AppendANDCondition("GameType = " & SQLString(pGameTypes))
                oWhere.AppendANDCondition("MaxParlay is NULL and MaxTeaser is NULL and MaxReverse is NULL ")
                Dim sSQL As String = "SELECT {0} " & _
                                                   "FROM PlayerTemplateLimits   " & oWhere.SQL
                sSQL = String.Format(sSQL, psBetType)
                Return SafeDouble(odbSQL.getScalerValue(sSQL))
            Catch ex As Exception
                LogError(log, "Get Straight of Player template", ex)
            Finally
                odbSQL.closeConnection()
            End Try
        End Function

        Public Function InsertTemplateLimit(ByVal psPlayerTemplateID As String, ByVal psGameType As String, ByVal psContext As String, ByVal pnSpread As Double, ByVal pnTotal As Double, ByVal pnML As Double, ByVal psCreatedBy As String) As Boolean
            Dim oInsert As New CSQLInsertStringBuilder("PlayerTemplateLimits")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim bSuccess As Boolean = False
            Try
                With oInsert
                    .AppendString("PlayerTemplateLimitID", SQLString(newGUID()))
                    .AppendString("PlayerTemplateID", SQLString(psPlayerTemplateID))
                    .AppendString("GameType", SQLString(psGameType))
                    .AppendString("Context", SQLString(psContext))
                    .AppendString("Spread", SQLDouble(pnSpread))
                    .AppendString("ToTal", SQLDouble(pnTotal))
                    .AppendString("ML", SQLDouble(pnML))
                End With

                log.Debug("Insert PlayerTemplateLimits.SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psCreatedBy)

                bSuccess = True
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to insert PlayerTemplateLimits. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function updateTemplateLimit(ByVal PlayerTemplateLimitID As String, ByVal psGameType As String, ByVal psContext As String, ByVal psCreatedBy As String, Optional ByVal pnSpread As String = "", Optional ByVal pnTotal As String = "", Optional ByVal pnML As String = "") As Boolean
            Dim oUpdate As New CSQLUpdateStringBuilder("PlayerTemplateLimits", " where PlayerTemplateLimitID=" & SQLString(PlayerTemplateLimitID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim bSuccess As Boolean = False
            Try
                With oUpdate
                    .AppendString("GameType", SQLString(psGameType))
                    .AppendString("Context", SQLString(psContext))
                    If pnSpread <> "" Then
                        .AppendString("Spread", SQLDouble(pnSpread))
                    End If
                    If pnTotal <> "" Then
                        .AppendString("ToTal", SQLDouble(pnTotal))
                    End If
                    If pnML <> "" Then
                        .AppendString("ML", SQLDouble(pnML))
                    End If
                End With
                log.Debug("Update PlayerTemplateLimits.SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psCreatedBy, False)
                bSuccess = True
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to update PlayerTemplateLimits. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function


    End Class
End Namespace
