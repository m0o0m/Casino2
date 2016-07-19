Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CSuperUserManager
        Private _sMaskFone As String = "___-___-____"
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public Function UpdateSuper(ByVal psSuperID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, _
                                    ByVal pnTimeZone As Integer, ByVal psSiteURL As String, ByVal psSiteURLpBackup As String, ByVal pbIsLocked As Boolean, ByVal pbIsManager As Boolean, _
                                    ByVal psLockReason As String, ByVal psChangedBy As String, Optional ByVal psWagering As String = "", Optional ByVal psCustomerService As String = "") As Boolean

            Dim bSuccess As Boolean = False
            System.Web.HttpContext.Current.Cache.Remove("SITE_URL")
            Dim oUpdate As New CSQLUpdateStringBuilder("SuperAdmins", "WHERE SuperAdminID= " & SQLString(psSuperID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("Name", SQLString(psName))
                    If psPassword <> "" Then
                        .AppendString("Password", SQLString(EncryptToHash(psPassword)))
                    End If
                    .AppendString("Login", SQLString(SafeString(psLogin)))
                    .AppendString("SiteURL", SQLString(SafeString(psSiteURL)))
                    .AppendString("SiteURLBackup", SQLString(SafeString(psSiteURLpBackup)))
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("IsManager", SQLString(IIf(pbIsManager, "Y", "N")))
                    If psWagering.Equals(_sMaskFone) Then
                        .AppendString("Wagering", "NULL")
                    Else
                        .AppendString("Wagering", SQLString(psWagering))
                    End If
                    If psCustomerService.Equals(_sMaskFone) Then
                        .AppendString("CustomerService", "NULL")
                    Else
                        .AppendString("CustomerService", SQLString(psCustomerService))
                    End If
                End With

                log.Debug("Update SuperAdmin. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearSuperAdminInfo(psSuperID)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to save SuperAdmin. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function InsertSuper(ByVal psSuperID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, ByVal psSiteURL As String, ByVal psSiteURLBackup As String, _
                                   ByVal pnTimeZone As Integer, ByVal pbIsLocked As Boolean, ByVal pbIsManager As Boolean, ByVal psLockReason As String, ByVal psCreatedBy As String, ByVal psSiteType As String, Optional ByVal pbPartner As Boolean = False, _
                                   Optional ByVal psWagering As String = "", Optional ByVal psCustomerService As String = "") As Boolean

            Dim bSuccess As Boolean = False
            System.Web.HttpContext.Current.Cache.Remove("SITE_URL")
            Dim oInsert As New CSQLInsertStringBuilder("SuperAdmins")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                With oInsert
                    .AppendString("SuperAdminID", SQLString(psSuperID))
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    .AppendString("SiteURL", SQLString(psSiteURL))
                    .AppendString("SiteURLBackup", SQLString(psSiteURLBackup))
                    .AppendString("Password", SQLString(EncryptToHash(psPassword)))
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("IsManager", SQLString(IIf(pbIsManager, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("SiteType", SQLString(psSiteType))
                    .AppendString("IsPartner", SQLString(IIf(pbPartner, "Y", "N")))
                    If (pbPartner) Then
                        .AppendString("PartnerOf", SQLString(psCreatedBy))
                    End If
                    If psSiteType.Equals(SBCBL.CEnums.ESiteType.SBS.ToString) Then
                        If psWagering.Equals(_sMaskFone) Then
                            .AppendString("Wagering", "NULL")
                        Else
                            .AppendString("Wagering", SQLString(psWagering))
                        End If
                        If psCustomerService.Equals(_sMaskFone) Then
                            .AppendString("CustomerService", "NULL")
                        Else
                            .AppendString("CustomerService", SQLString(psCustomerService))
                        End If
                    End If
                End With

                log.Debug("Insert Super .SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psCreatedBy)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to insert Super. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function GetPartners(ByVal psSiteType As String, ByVal psSuperID As String, ByVal pbIsLocked As Boolean) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N') = " & SQLString(SafeString(IIf(pbIsLocked, "Y", "N"))))
            oWhere.AppendANDCondition("ISNULL(IsPartner, 'N') = 'Y'")
            oWhere.AppendANDCondition("PartnerOf = " & SQLString(psSuperID))

            Dim sSQL As String = "select * from SuperAdmins " & oWhere.SQL

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of Partners . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetSuperAdmins(ByVal psSiteType As String, ByVal pbIsLocked As Boolean) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N') = " & SQLString(SafeString(IIf(pbIsLocked, "Y", "N"))))
            oWhere.AppendANDCondition("ISNULL(IsPartner, 'N') = 'N'")

            Dim sSQL As String = "select * from SuperAdmins " & oWhere.SQL

            Try
                Return odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of Super . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return Nothing
        End Function

        ''' <summary>
        ''' Get Super admin by ID
        ''' </summary>
        ''' <param name="psSuperID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSuperByID(ByVal psSuperID As String) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = "SELECT TOP 1 * FROM SuperAdmins WHERE SuperAdminID = " & SQLString(psSuperID)
            Try
                log.Debug("Get Admin's Info by SuperAdminID: " & psSuperID)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Agent by ID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function LockSuperUsers(ByVal poLstSupers As List(Of String), ByVal pbLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            Try
                Dim oLstSuperIDs As New List(Of String)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sItem As String In poLstSupers
                    Dim oSuperInfo As String() = sItem.Split("|"c)
                    '' chek if the input value is valid
                    If oSuperInfo.Count = 2 Then
                        oCache.ClearAgentInfo(oSuperInfo(0), oSuperInfo(1))
                        oLstSuperIDs.Add(oSuperInfo(0))
                    Else
                        log.Error("Wrong super info: " & sItem)
                        Continue For
                    End If
                Next

                log.Debug("Lock/Unlock these Agents(AgentID|Login): " & Join(oLstSuperIDs.ToArray(), ","))
                oUpdate = New CSQLUpdateStringBuilder("SuperAdmins", String.Format("WHERE SuperAdminID IN ('{0}')", Join(oLstSuperIDs.ToArray(), "','")))
                oUpdate.AppendString("IsLocked", SQLString(IIf(pbLocked, "Y", "N")))
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                oDB.executeNonQuery(oUpdate, psUpdateBy)

                Return True
            Catch ex As Exception
                log.Error("Fails to lock/unlock agent.SQL: " & SafeString(IIf(oUpdate IsNot Nothing, oUpdate.SQL, "")), ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        'Public Shared Function CheckDuplicateLogin(ByVal psLogin As String, ByVal psSuperAdminID As String) As Boolean
        '    Dim oWhere As New CSQLWhereStringBuilder()
        '    oWhere.AppendANDCondition("Login = " & SQLString(LCase(psLogin)))
        '    oWhere.AppendOptionalANDCondition("SuperAdminID ", SQLString(psSuperAdminID), "<>")

        '    Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
        '    Dim sSQL As String = "select top 1 * from SuperAdmins " & oWhere.SQL
        '    Try
        '        Dim oDT As DataTable = oDB.getDataTable(sSQL)
        '        Return oDT.Rows.Count = 0
        '    Catch ex As Exception
        '        Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CSuperUserManager))
        '        log.Error("Can't CheckDuplicateLogin: " & sSQL, ex)
        '    Finally
        '        oDB.closeConnection()
        '    End Try
        '    Return False
        'End Function

        Public Function IsExistedLogin(ByVal psLogin As String, ByVal psSuperAdminID As String, ByVal psSiteType As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Login=" & SQLString(psLogin))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''CallCenterAgent
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM CallCenterAgents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

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
                    oWhere.AppendOptionalANDCondition("SuperAdminID", SQLString(psSuperAdminID), "<>")
                    sSQL = "SELECT count(Login) FROM SuperAdmins " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

            Catch ex As Exception
                log.Error("Cannot check login existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function IsExistedWagering(ByVal psWagering As String, ByVal psSiteType As String, ByVal psSuperAdminID As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Wagering=" & SQLString(psWagering))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendOptionalANDCondition("SuperAdminID", SQLString(psSuperAdminID), "<>")
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try

                sSQL = "SELECT count(Wagering) FROM SuperAdmins " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check Wagering existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function IsExistedCustomerService(ByVal psCustomerService As String, ByVal psSiteType As String, ByVal psSuperAdminID As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("CustomerService=" & SQLString(psCustomerService))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendOptionalANDCondition("SuperAdminID", SQLString(psSuperAdminID), "<>")
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try

                sSQL = "SELECT count(CustomerService) FROM SuperAdmins " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check CustomerService existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function UpdateWeeklyCharge(ByVal psSuperID As String, ByVal pnWeeklyCharge As Double, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False

            Dim oUpdate As New CSQLUpdateStringBuilder("SuperAdmins", "WHERE SuperAdminID= " & SQLString(psSuperID))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("ChargePerAccount", SQLDouble(pnWeeklyCharge))
                End With

                log.Debug("Update Weekly Charge. SQL: " & oUpdate.SQL)
                oDB.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearSuperAdminInfo(psSuperID)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to update Weekly Charge. SQL: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function ChangePartnerPassword(ByVal psPartnerID As String, ByVal psCurrentPassword As String, ByVal psNewPassword As String, ByVal psChangedBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SuperAdminID=" & SQLString(psPartnerID))
            oWhere.AppendANDCondition("Password=" & SQLString(EncryptToHash(psCurrentPassword)))

            Dim oUpdate As New CSQLUpdateStringBuilder("SuperAdmins", oWhere.SQL)
            oUpdate.AppendString("Password", SQLString(EncryptToHash(psNewPassword)))

            log.Debug("Change password agent. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psChangedBy) > 0
            Catch ex As Exception
                log.Error("Cannot change password agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
        End Function

    End Class
End Namespace