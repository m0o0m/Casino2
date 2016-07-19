Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CIPTracesManager
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CIPTracesManager))

        Public Shared Function InsertIPTrace(ByVal psUserID As String, ByVal psLoginName As String, ByVal psIP As String, ByVal psSiteType As String) As Boolean

            Dim bSuccess As Boolean = True
            Dim oInsert As ISQLEditStringBuilder = New CSQLInsertStringBuilder("IPTraces")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oInsert
                    .AppendString("IPTraceID", SQLString(newGUID))
                    .AppendString("LoginName", SQLString(psLoginName))
                    .AppendString("IP", SQLString(psIP))
                    .AppendString("TraceDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("SiteType", SQLString(psSiteType))
                End With

                _log.Debug("Insert IPTraces. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psUserID)

            Catch ex As Exception
                bSuccess = False
                _log.Error("Error trying to insert IPTraces. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function GetIPTraces(ByVal psLoginName As String, ByVal psSiteType As String, Optional ByVal psIP As String = "", Optional ByVal piLastNumDay As Integer = 0) As DataTable
            Dim odtIPTraces As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendOptionalANDCondition("TraceDate", SQLString(Date.Now.ToUniversalTime.AddDays(piLastNumDay).ToString("MM/dd/yyyy")), ">=")
            If psLoginName <> "" Then
                oWhere.AppendANDCondition("LoginName=" & SQLString(psLoginName))
            End If
            If psIP <> "" Then
                oWhere.AppendANDCondition("IP=" & SQLString(psIP))
            End If
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = "SELECT ROW_NUMBER() OVER(order by IP) as NumIndex,LoginName,IP as IpAddress,count(IP) as TimeUsed,max(TraceDate) as LastTimeUsed " & vbCrLf & _
                " FROM IPTraces" & vbCrLf & _
                oWhere.SQL & vbCrLf & " GROUP BY IP, LoginName " & vbCrLf & _
                " ORDER BY LastTimeUsed desc,LoginName , IP "
            _log.Debug("Get the list of IPTraces. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtIPTraces = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get the list of IPTraces. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtIPTraces
        End Function

        Public Function GetIPAlertSearchResult(ByVal psParentID As String, _
                                               ByVal pbIsSuperAdmin As Boolean, _
                                               ByVal psDateFrom As String) As DataTable

            Dim oAgentWhere As New CSQLWhereStringBuilder()
            If pbIsSuperAdmin Then
                oAgentWhere.AppendANDCondition("SuperAdminID=" & SQLString(psParentID))
            Else
                oAgentWhere.AppendANDCondition("AgentID=" & SQLString(psParentID))
            End If

            Dim oTraceDateWhere As New CSQLWhereStringBuilder()
            oTraceDateWhere.AppendANDCondition("IPTraces.TraceDate>=" & SQLString(psDateFrom))

            Dim sSQL_WITH As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS" & vbCrLf & _
                                      "(" & vbCrLf & _
                                      "     SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents" & vbCrLf & _
                                      "     " & oAgentWhere.SQL() & vbCrLf & _
                                      "     UNION ALL" & vbCrLf & _
                                      "     SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                      "     FROM Agents as child" & vbCrLf & _
                                      "     INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID" & vbCrLf & _
                                      ")" & vbCrLf

            Dim sSQL_TraceDate As String = "Select Max(TraceDate) From IPTraces" & vbCrLf & _
                                          oTraceDateWhere.SQL() & vbCrLf & _
                                          "Group By LoginName"

            Dim oIPTracesWhere As New CSQLWhereStringBuilder()
            oIPTracesWhere.AppendANDCondition("IPTraces.TraceDate In (" & sSQL_TraceDate & ")")

            Dim sSQL_Agents As String = "Select (Agents.Login+ ' (' + Agents.Name + ')') as AgentName, '' As Player, IPTraces.IP, IPTraces.LoginName, IPTraces.TraceDate " & vbCrLf & _
                                        "From Agents" & vbCrLf & _
                                        "Inner Join AgentRecursive On Agents.AgentID=AgentRecursive.AgentID" & vbCrLf & _
                                        "Inner Join IPTraces On Agents.Login=IPTraces.LoginName " & vbCrLf & _
                                        oIPTracesWhere.SQL() & vbCrLf

            Dim sSQL_Players As String = "Select (Agents.Login+ ' (' + Agents.Name + ')') as AgentName, (Players.Login+ ' (' + Players.Name + ')') as PlayerName, IPTraces.IP, IPTraces.LoginName, IPTraces.TraceDate " & vbCrLf & _
                                         "From Players" & vbCrLf & _
                                         "Inner Join Agents On Players.AgentID=Agents.AgentID" & vbCrLf & _
                                         "Inner Join AgentRecursive On AgentRecursive.AgentID=Agents.AgentID" & vbCrLf & _
                                         "Inner Join IPTraces On Players.Login=IPTraces.LoginName" & vbCrLf & _
                                         oIPTracesWhere.SQL() & vbCrLf

            Dim sSQL_SuperAdmins As String = "Select SuperAdmins.Name + '(' + SuperAdmins.Login + ')' As AgentName, '' As Player, IPTraces.IP, IPTraces.LoginName, IPTraces.TraceDate " & vbCrLf & _
                                             "From SuperAdmins" & vbCrLf & _
                                             "Inner Join IPTraces On SuperAdmins.Login=IPTraces.LoginName" & vbCrLf & _
                                             oIPTracesWhere.SQL() & vbCrLf & _
                                             "And SuperAdmins.SuperAdminID=" & SQLString(psParentID)

            Dim sSQL As String = ""
            If pbIsSuperAdmin Then
                sSQL &= sSQL_WITH & vbCrLf & _
                        sSQL_Agents & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_Players & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_SuperAdmins
            Else
                sSQL &= sSQL_WITH & vbCrLf & _
                        sSQL_Agents & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_Players & vbCrLf
            End If

            LogDebug(_log, "Get IPAlert search result. SQL: " & sSQL)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oData As New DataTable

            Try
                oData = oDB.getDataTable(sSQL)
            Catch ex As Exception
                LogError(_log, "Error while get IPAlert search result", ex)
            Finally
                oDB.closeConnection()
            End Try

            Return oData
        End Function

        Public Function GetIPTracesByDateRange(ByVal psParentID As String, _
                                               ByVal pbIsSuperAdmin As Boolean, _
                                               ByVal psDateFrom As String) As DataTable

            Dim oAgentWhere As New CSQLWhereStringBuilder()
            If pbIsSuperAdmin Then
                oAgentWhere.AppendANDCondition("SuperAdminID=" & SQLString(psParentID))
            Else
                oAgentWhere.AppendANDCondition("AgentID=" & SQLString(psParentID))
            End If

            Dim oTraceDateWhere As New CSQLWhereStringBuilder()
            oTraceDateWhere.AppendANDCondition("IPTraces.TraceDate>=" & SQLString(psDateFrom))

            Dim sSQL_WITH As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS" & vbCrLf & _
                                      "(" & vbCrLf & _
                                      "     SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents" & vbCrLf & _
                                      "     " & oAgentWhere.SQL() & vbCrLf & _
                                      "     UNION ALL" & vbCrLf & _
                                      "     SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                      "     FROM Agents as child" & vbCrLf & _
                                      "     INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID" & vbCrLf & _
                                      ")" & vbCrLf

            Dim sSQL_Agents As String = "Select IPTraces.LoginName, IPTraces.IP, count(*) as LoginTime, CONVERT(VARCHAR(10),MAX(IPTraces.TraceDate),101) AS TraceDate " & vbCrLf & _
                                        "From Agents" & vbCrLf & _
                                        "Inner Join AgentRecursive On Agents.AgentID=AgentRecursive.AgentID" & vbCrLf & _
                                        "Inner Join IPTraces On Agents.Login=IPTraces.LoginName " & vbCrLf & _
                                        oTraceDateWhere.SQL() & " Group By IPTraces.LoginName, IPTraces.IP, CONVERT(VARCHAR(10),IPTraces.TraceDate,101) " & vbCrLf

            Dim sSQL_Players As String = "Select IPTraces.LoginName, IPTraces.IP, count(*) as LoginTime, CONVERT(VARCHAR(10),MAX(IPTraces.TraceDate),101) AS TraceDate " & vbCrLf & _
                                         "From Players" & vbCrLf & _
                                         "Inner Join Agents On Players.AgentID=Agents.AgentID" & vbCrLf & _
                                         "Inner Join AgentRecursive On AgentRecursive.AgentID=Agents.AgentID" & vbCrLf & _
                                         "Inner Join IPTraces On Players.Login=IPTraces.LoginName" & vbCrLf & _
                                         oTraceDateWhere.SQL() & " Group By IPTraces.LoginName, IPTraces.IP, CONVERT(VARCHAR(10),IPTraces.TraceDate,101) " & vbCrLf

            Dim sSQL_SuperAdmins As String = "Select IPTraces.LoginName, IPTraces.IP, count(*) as LoginTime, CONVERT(VARCHAR(10),MAX(IPTraces.TraceDate),101) AS TraceDate " & vbCrLf & _
                                             "From SuperAdmins" & vbCrLf & _
                                             "Inner Join IPTraces On SuperAdmins.Login=IPTraces.LoginName" & vbCrLf & _
                                             oTraceDateWhere.SQL() & vbCrLf & _
                                             "And SuperAdmins.SuperAdminID=" & SQLString(psParentID) & " Group By IPTraces.LoginName, IPTraces.IP, CONVERT(VARCHAR(10),IPTraces.TraceDate,101) "

            Dim sSQL As String = ""
            If pbIsSuperAdmin Then
                sSQL &= sSQL_WITH & vbCrLf & _
                        sSQL_Agents & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_Players & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_SuperAdmins & vbCrLf & _
                        "Order By LoginName"
            Else
                sSQL &= sSQL_WITH & vbCrLf & _
                        sSQL_Agents & vbCrLf & _
                        "Union All" & vbCrLf & _
                        sSQL_Players & vbCrLf & _
                        "Order By LoginName"
            End If

            LogDebug(_log, "Get IPTraces by date range. SQL: " & sSQL)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oData As New DataTable

            Try
                oData = oDB.getDataTable(sSQL)
            Catch ex As Exception
                LogError(_log, "Error while get IPTraces by date range", ex)
            Finally
                oDB.closeConnection()
            End Try

            Return oData
        End Function

    End Class

End Namespace
