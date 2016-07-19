Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CPlayerTemplateLimitManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CPlayerTemplateLimitManager))

#Region "Save methods"

        Public Function SavePlayerTemplateLimits(ByVal psPlayerTemplateID As String, ByVal poPlayerTemplateLimits As CPlayerTemplateLimitList) As Boolean
            Dim bSuccess As Boolean = True
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                log.Debug("Save player template limits.")

                Dim oTempLimitIDs As New List(Of String)

                'load template limit
                Dim sSQL As String = "SELECT * FROM PlayerTemplateLimits WHERE PlayerTemplateID=" & SQLString(psPlayerTemplateID) & " ORDER BY GameType, Context"
                Dim odtTempLimits As DataTable = oDB.getDataTable(sSQL)

                For Each oTempLimit As CPlayerTemplateLimit In poPlayerTemplateLimits
                    Dim odrTempLimit As DataRow = Nothing
                    Dim odrFinds As DataRow() = odtTempLimits.Select("PlayerTemplateLimitID=" & SQLString(oTempLimit.PlayerTemplateLimitID))

                    If odrFinds.Length = 0 Then
                        odrTempLimit = odtTempLimits.NewRow
                        odrTempLimit("PlayerTemplateLimitID") = oTempLimit.PlayerTemplateLimitID
                        odrTempLimit("PlayerTemplateID") = psPlayerTemplateID

                        odtTempLimits.Rows.Add(odrTempLimit)
                    Else
                        odrTempLimit = odrFinds(0)
                    End If

                    odrTempLimit("GameType") = oTempLimit.GameType
                    odrTempLimit("Context") = oTempLimit.Context
                    odrTempLimit("MaxParlay") = oTempLimit.MaxParlay
                    odrTempLimit("MaxReverse") = oTempLimit.MaxReverse
                    odrTempLimit("MaxSingle") = oTempLimit.MaxSingle
                    odrTempLimit("MaxTeaser") = oTempLimit.MaxTeaser

                    oTempLimitIDs.Add(UCase(oTempLimit.PlayerTemplateLimitID))
                Next

                ''customize template limits
                For i As Integer = odtTempLimits.Rows.Count - 1 To 0 Step -1
                    If Not oTempLimitIDs.Contains(UCase(SafeString(odtTempLimits.Rows(i)("PlayerTemplateLimitID")))) Then
                        odtTempLimits.Rows(i).Delete()
                    End If
                Next

                ''save to DB
                oDB.UpdateDataTable(odtTempLimits, "SELECT TOP 0 * FROM PlayerTemplateLimits", False)

            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot save player template limits. Error: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

#End Region

#Region "Get data methods"

        Public Function GetPlayerTemplates(ByVal psPlayerTemplateID As String) As CPlayerTemplateLimitList
            Dim oLimits As New CPlayerTemplateLimitList

            Dim sSQL As String = "SELECT * FROM PlayerTemplateLimits " & vbCrLf & _
                "WHERE PlayerTemplateID=" & SQLString(psPlayerTemplateID) & " ORDER BY GameType, Context"
            log.Debug("Get player template limits. SQL: " & sSQL)

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Dim odtTempLimits As DataTable = oDB.getDataTable(sSQL)

                For Each odrTempLimit As DataRow In odtTempLimits.Rows
                    Dim oTempLimit As New CPlayerTemplateLimit

                    oTempLimit.PlayerTemplateLimitID = SafeString(odrTempLimit("PlayerTemplateLimitID"))
                    oTempLimit.PlayerTemplateID = SafeString(odrTempLimit("PlayerTemplateID"))
                    oTempLimit.GameType = SafeString(odrTempLimit("GameType"))
                    oTempLimit.Context = SafeString(odrTempLimit("Context"))
                    oTempLimit.MaxParlay = SafeDouble(odrTempLimit("MaxParlay"))
                    oTempLimit.MaxReverse = SafeDouble(odrTempLimit("MaxReverse"))
                    oTempLimit.MaxSingle = SafeDouble(odrTempLimit("MaxSingle"))
                    oTempLimit.MaxTeaser = SafeDouble(odrTempLimit("MaxTeaser"))

                    oLimits.Add(oTempLimit)
                Next

            Catch ex As Exception
                log.Error("Cannot get player template limits. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return oLimits
        End Function

        Public Function GetMinPlayerTemplates(ByVal pLstContext As List(Of String), ByVal psPlayerTemplateID As String, ByVal pLstGameType As List(Of String), ByVal psBetType As String) As DataTable
            Dim sSQL As String = ""
            Dim oWhere As New CSQLWhereStringBuilder()
            Select Case (UCase(psBetType.Replace("Min", "")))
                Case "SINGLE"
                    'oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxSingle,0)<>0 ")
                    sSQL = "select  MinSingle  from({0}) as MinPlayerTemplates where MinSingle is not null order by  MinSingle asc"
                Case "PARLAY"
                    ' oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxParlay,0)<>0 ")
                    sSQL = "select MinParlay  from({0}) as MinPlayerTemplates where MinParlay is not null order by  MinParlay asc"
                Case "TEASER"
                    ' oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxTeaser,0)<>0  ")
                    sSQL = "select MinTeaser from({0}) as MinPlayerTemplates where MinTeaser is not null order by  MinTeaser asc"
                Case "REVERSE"
                    'oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxReverse,0)<>0  ")
                    sSQL = "select MinReverse from ({0}) as MinPlayerTemplates where MinReverse is not null  order by  MinReverse asc"
            End Select
            Dim sSQLUnion As String = ""
            For i As Integer = 0 To pLstContext.Count - 1
                Dim sSQLLimit As String = GetSQLMinPlayerTemplates(pLstContext(i), psPlayerTemplateID, pLstGameType(i), psBetType)
                If i = 0 Then
                    sSQLUnion += sSQLLimit
                Else
                    If Not sSQLUnion.Contains(sSQLLimit) Then
                        sSQLUnion += " union all " & GetSQLMinPlayerTemplates(pLstContext(i), psPlayerTemplateID, pLstGameType(i), psBetType)
                    End If
                End If
            Next
            sSQL = String.Format(sSQL, sSQLUnion)
            log.Debug(" get player min limit: " & sSQL)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get min player template limits. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetSQLMinPlayerTemplates(ByVal psContext As String, ByVal psPlayerTemplateID As String, ByVal psGameType As String, ByVal psBetType As String) As String
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PlayerTemplates.PlayerTemplateID=" & SQLString(psPlayerTemplateID))
            Dim sSQL As String = ""
            Select Case (UCase(psBetType.Replace("Min", "")))
                Case "SINGLE"
                    oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxSingle,0)<>0 ")
                    sSQL = "select MIN(PlayerTemplateLimits.MaxSingle) as MinSingle"
                Case "PARLAY"
                    oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxParlay,0)<>0 ")
                    sSQL = "select MIN(PlayerTemplateLimits.MaxParlay) as MinParlay"
                Case "TEASER"
                    oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxTeaser,0)<>0 ")
                    sSQL = "select MIN(PlayerTemplateLimits.MaxTeaser) as MinTeaser"
                Case "REVERSE"
                    oWhere.AppendANDCondition("isnull(PlayerTemplateLimits.MaxReverse,0)<>0  ")
                    sSQL = "select MIN(PlayerTemplateLimits.MaxReverse) as MinReverse "
            End Select
            oWhere.AppendANDCondition(String.Format("Context in ({0})", SQLString(psContext)))
            oWhere.AppendANDCondition("GameType= " & SQLString(psGameType))
            sSQL += " from PlayerTemplateLimits inner join  PlayerTemplates on PlayerTemplateLimits.PlayerTemplateID= PlayerTemplates.PlayerTemplateID " & vbCrLf & _
            oWhere.SQL.Replace("''", "'")
            Return sSQL
        End Function


#End Region

    End Class

End Namespace

