Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CCallCenterAgentManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetByID(ByVal psCallCenterAgentID As String) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = "SELECT TOP 1 * FROM CallCenterAgents WHERE CallCenterAgentID = " & SQLString(psCallCenterAgentID)
            Try
                log.Debug("Get CallCenterAgent's Info by CallCenterAgentID: " & psCallCenterAgentID)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get CallCenterAgent by ID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAgents(ByVal psSiteType As String, Optional ByVal pbIsLocked As Boolean = False, Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtPlayers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(Name LIKE {0} OR Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            Dim sSQL As String = "SELECT (Login + ' (' + Name + ')') as FullName ,* FROM CallCenterAgents " & oWhere.SQL & " ORDER BY Login, Name "
            log.Debug("Get the list of CallCenterAgents by SuperID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtPlayers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of CallCenterAgents . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtPlayers
        End Function

        Public Function LockAgents(ByVal pLstCallCenterAgents As List(Of String), ByVal pbLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            Try
                Dim oLstCallCenterAgentID As New List(Of String)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sCallCenterAgentID As String In pLstCallCenterAgents
                    Dim oCallCenterAgentInfo As String() = sCallCenterAgentID.Split("|"c)
                    oCache.ClearAgentInfo(oCallCenterAgentInfo(0), oCallCenterAgentInfo(1))
                    oLstCallCenterAgentID.Add(oCallCenterAgentInfo(0))
                Next

                log.Debug("Lock/Unlock these CallCenterAgents(CallCenterAgentID|Login): " & Join(oLstCallCenterAgentID.ToArray(), ","))
                oUpdate = New CSQLUpdateStringBuilder("CallCenterAgents", String.Format("WHERE CallCenterAgentID IN ('{0}')", Join(oLstCallCenterAgentID.ToArray(), "','")))
                oUpdate.AppendString("IsLocked", SQLString(IIf(pbLocked, "Y", "N")))
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                oDB.executeNonQuery(oUpdate, psUpdateBy)

                Return True
            Catch ex As Exception
                log.Error("Fails to lock/unlock CallCenterAgent.SQL: " & SafeString(IIf(oUpdate IsNot Nothing, oUpdate.SQL, "")), ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function InsertAgent(ByVal psCallCenterAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, _
                                     ByVal pnTimeZone As Integer, ByVal pbIsLocked As Boolean, ByVal psLockReason As String, ByVal psPhoneExtension As String, _
                                     ByVal psCreatedBy As String, ByVal psSiteType As String) As Boolean

            Dim bSuccess As Boolean = False

            Dim oInsert As New CSQLInsertStringBuilder("CallCenterAgents")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oInsert
                    .AppendString("CallCenterAgentID", SQLString(psCallCenterAgentID))
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    .AppendString("Password", SQLString(EncryptToHash(psPassword)))
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("CreatedBy", SQLString(psCreatedBy))
                    .AppendString("PhoneExtension", SQLString(psPhoneExtension))
                    .AppendString("SiteType", SQLString(psSiteType))
                End With

                log.Debug("Insert call center agent.SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psCreatedBy)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to insert call center agent. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateAgent(ByVal psCallCenterAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, _
                                     ByVal pnTimeZone As Integer, ByVal pbIsLocked As Boolean, ByVal psLockReason As String, ByVal psPhoneExtension As String, _
                                     ByVal psChangedBy As String) As Boolean

            Dim bSuccess As Boolean = False

            Dim oUpdate As New CSQLUpdateStringBuilder("CallCenterAgents", "WHERE CallCenterAgentID= " & SQLString(psCallCenterAgentID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    If psPassword <> "" Then
                        .AppendString("Password", SQLString(EncryptToHash(psPassword)))
                        .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    End If
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("PhoneExtension", SQLString(psPhoneExtension))
                End With

                log.Debug("Update call center agent. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearCallCenterAgentInfo(psCallCenterAgentID, psName)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to save call center agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function ChangePassword(ByVal psCallCenterAgentID As String, ByVal psCurrentPassword As String, ByVal psNewPassword As String, ByVal psChangedBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("CallCenterAgentID=" & SQLString(psCallCenterAgentID))
            oWhere.AppendANDCondition("Password=" & SQLString(EncryptToHash(psCurrentPassword)))

            Dim oUpdate As New CSQLUpdateStringBuilder("CallCenterAgents", oWhere.SQL)
            oUpdate.AppendString("Password", SQLString(EncryptToHash(psNewPassword)))
            oUpdate.AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))

            log.Debug("Change password call center agent. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psChangedBy) > 0
            Catch ex As Exception
                log.Error("Cannot change password call center agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
        End Function

        'Public Function CheckDuplicateLogin(ByVal psLogin As String, ByVal psCallCenterAgentID As String) As Boolean
        '    Dim oWhere As New CSQLWhereStringBuilder()
        '    oWhere.AppendANDCondition("Login = " & SQLString(LCase(psLogin)))
        '    oWhere.AppendOptionalANDCondition("CallCenterAgentID ", SQLString(psCallCenterAgentID), "<>")

        '    Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
        '    Dim sSQL As String = "select top 1 * from CallCenterAgents " & oWhere.SQL
        '    Try
        '        Dim oDT As DataTable = oDB.getDataTable(sSQL)
        '        Return oDT.Rows.Count = 0
        '    Catch ex As Exception
        '        Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CCallCenterAgentManager))
        '        log.Error("Can't CheckDuplicateLogin: " & sSQL, ex)
        '    Finally
        '        oDB.closeConnection()
        '    End Try
        '    Return False
        'End Function

        Public Function IsExistedPhoneExtension(ByVal psPhoneExtension As String, ByVal psCallCenterAgentID As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PhoneExtension=" & SQLString(psPhoneExtension))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oWhere.AppendOptionalANDCondition("CallCenterAgentID", SQLString(psCallCenterAgentID), "<>")
                sSQL = "SELECT count(PhoneExtension) FROM CallCenterAgents " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check PhoneExtension existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function


        Public Function IsExistedLogin(ByVal psLogin As String, ByVal psCallCenterAgentID As String, ByVal psSiteType As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Login=" & SQLString(psLogin))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''Player
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM Players " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''Agent
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM Agents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''SuperAdmin
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM SuperAdmins " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''CallCenterAgent
                If Not bExisted Then
                    oWhere.AppendOptionalANDCondition("CallCenterAgentID", SQLString(psCallCenterAgentID), "<>")
                    sSQL = "SELECT count(Login) FROM CallCenterAgents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If
            Catch ex As Exception
                log.Error("Cannot check login existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function
    End Class

End Namespace