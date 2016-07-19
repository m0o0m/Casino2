Imports System.Web.Caching
Imports System.Text.RegularExpressions
Imports WebsiteLibrary.DBUtils
Imports WebsiteLibrary.CSBCStd
Imports SBCBL.std
Imports SBCBL.Managers
Imports SBCBL.Utils
Imports SBCBL.CacheUtils

Namespace Security
    Public Class CLoginHelper
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CLoginHelper))

        Public Shared Function GetLoginInfo(ByVal pbIsAdminLogin As Boolean, ByVal psLogin As String, ByVal psPassword As String, ByVal psSiteType As String, Optional ByVal pbImpersonate As Boolean = False) As DataRow
            Dim oInfo As DataRow = Nothing
            Dim oDB As CSQLDBUtils = Nothing
            If pbImpersonate Then
                System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.SuperAdmin
            Else
                System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.Player
            End If


            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                '' For SuperAdmin, CallCenter, Agent Only
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("Login=" & SQLString(psLogin))
                If Not pbImpersonate Then
                    oWhere.AppendANDCondition("Password=" & SQLString(psPassword))
                End If
                oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))

                Dim oWhereSuper As New CSQLWhereStringBuilder()
                oWhereSuper.AppendANDCondition("Login=" & SQLString(psLogin))
                If Not pbImpersonate Then
                    oWhereSuper.AppendANDCondition("Password=" & SQLString(EncryptToHash(psPassword)))
                End If
                oWhereSuper.AppendANDCondition("SiteType=" & SQLString(psSiteType))
                '' For Player Only
                Dim oWherePlayer As New CSQLWhereStringBuilder()
                oWherePlayer.AppendANDCondition("Login=" & SQLString(psLogin))
                If Not pbImpersonate Then
                    oWherePlayer.AppendANDCondition("Password=" & SQLString(psPassword))
                End If
                oWherePlayer.AppendANDCondition("SiteType=" & SQLString(psSiteType))

                '' Search in Players table first
                Dim sQR As String = "SELECT * FROM Players " & oWherePlayer.SQL
                Dim oDT As DataTable = oDB.getDataTable(sQR)
                If oDT.Rows.Count > 0 Then
                    oInfo = oDT.Rows(0)
                    System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.Player
                    _log.InfoFormat("login: '{0}' in Players table. ", psLogin)
                Else
                    '' Then search in Agents table
                    sQR = "SELECT * FROM Agents " & oWhere.SQL
                    oDT = oDB.getDataTable(sQR)
                    If oDT.Rows.Count > 0 Then
                        oInfo = oDT.Rows(0)
                        System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.Agent
                        _log.InfoFormat("login: '{0}' in Agents table. ", psLogin)
                    Else
                        '' Then search in CallCenterAgents table
                        sQR = "SELECT * FROM CallCenterAgents " & oWhere.SQL
                        oDT = oDB.getDataTable(sQR)
                        If oDT.Rows.Count > 0 Then
                            oInfo = oDT.Rows(0)
                            System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.CallCenterAgent
                            _log.InfoFormat("login: '{0}' in CallCenterAgents table. ", psLogin)
                        Else
                            '' Then search in SuperAdmins table
                            sQR = "SELECT * FROM SuperAdmins " & oWhereSuper.SQL
                            oDT = oDB.getDataTable(sQR)
                            If oDT.Rows.Count > 0 Then
                                oInfo = oDT.Rows(0)
                                System.Web.HttpContext.Current.Session("USER_TYPE") = EUserType.SuperAdmin
                                _log.InfoFormat(pbImpersonate & "login: '{0}' in SuperAdmins table. ", psLogin)
                            Else
                                '' Error
                                _log.DebugFormat(pbImpersonate & "Can't login. SQL : {0},{1},{2}", sQR, psPassword, EncryptToHash(psPassword))
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                _log.Error("Error logging in: " & ex.Message, ex)
                Throw New Exception("Sorry there was an error trying to login. An administrator has been notified. Please try again later.")
            Finally
                If Not oDB Is Nothing Then
                    oDB.closeConnection()
                End If
            End Try

            Return oInfo
        End Function

        Public Shared Sub LoadRolesToCache(ByVal psLogin As String, ByVal poCache As Cache, ByVal psKey As String, ByVal psSiteType As String)
            Dim oRoles As New ArrayList()
            Dim bBreak As Boolean = False

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""

            Try
                Dim sSiteType As String = GetSiteType() & "_"
                If Not bBreak Then
                    sSQL = String.Format("select top 1 * from Players where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sSQL).Rows.Count > 0 Then
                        oRoles.Add(sSiteType & "PLAYER")
                        bBreak = True
                    End If
                End If
                If Not bBreak Then
                    sSQL = String.Format("select top 1 * from Agents where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sSQL).Rows.Count > 0 Then
                        oRoles.Add(sSiteType & "AGENT")
                        bBreak = True
                    End If
                End If
                If Not bBreak Then
                    sSQL = String.Format("select top 1 * from CallCenterAgents where login={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sSQL).Rows.Count > 0 Then
                        oRoles.Add(sSiteType & "CALLCENTERAGENT")
                        bBreak = True
                    End If
                End If
                If Not bBreak Then
                    sSQL = String.Format("select top 1 * from SuperAdmins where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sSQL).Rows.Count > 0 Then
                        oRoles.Add(sSiteType & "SUPER")
                        bBreak = True
                    End If
                End If

            Catch ex As Exception
                _log.Error("Can't load role to cache. SQL : " & sSQL, ex)
            Finally
                If Not oDB Is Nothing Then oDB.closeConnection()
            End Try

            poCache.Add(psKey, oRoles.ToArray(GetType(String)), Nothing, Date.Now.AddMinutes(60), Nothing, CacheItemPriority.Default, Nothing)
        End Sub

        Public Shared Function CheckLoginInfo(ByVal poInfo As DataRow, ByVal pbIsAdminLogin As Boolean, ByVal psIP As String, ByVal psHost As String) As String
            If poInfo Is Nothing Then
                If pbIsAdminLogin Then
                    Return "Unable to find the user. Please check the username and try again."
                Else
                    Return "Unable to log in. Please check your login and password."
                End If
                _log.Info("Invalid username/password.")
            End If

            Dim bLocked As Boolean = True

            '' Check User's IsLocked
            bLocked = SafeString(poInfo("IsLocked")) = "Y"

            '' Check Parent's User
            Dim oUserType As EUserType = CType(System.Web.HttpContext.Current.Session("USER_TYPE"), EUserType)

            If Not bLocked And oUserType <> EUserType.SuperAdmin And oUserType <> EUserType.CallCenterAgent Then
                '' Both User's Type: PLAYER Or AGENT
                bLocked = (New Managers.CAgentManager).GetLocked(SafeString(poInfo("AgentID")))
            End If

            If bLocked Then
                _log.Info(SafeString(poInfo("Login")) & " has been locked.")
                Return "Your account has been locked. Please contact an administrator."
            End If
            Dim dicURL As Dictionary(Of String, String) = GetSiteURL()
            Dim strSuperAdminID As String
            If oUserType <> EUserType.SuperAdmin AndAlso Not String.IsNullOrEmpty(SafeString(poInfo("AgentID"))) Then
                strSuperAdminID = (New CAgentManager).GetSuperAdminID(SafeString(poInfo("AgentID")))
            Else
                strSuperAdminID = SafeString(poInfo("SuperAdminID"))
            End If
            If dicURL(strSuperAdminID) = "" OrElse dicURL(strSuperAdminID).Contains(psHost) Then
                Return ""
            Else
                Return "User can not login this page"
            End If
            Return ""
        End Function

        Public Shared Sub SaveLoginLog(ByVal psUserID As String, ByVal psLogin As String, ByVal pbSuccess As Boolean, _
                                       ByVal psIP As String, ByVal psDomain As String)
            Dim oDB As CSQLDBUtils = Nothing
            Dim oDB2 As CSQLDBUtils = Nothing
            Try
                oDB2 = New CSQLDBUtils(SBC2_CONNECTION_STRING, "")

                Dim oQR As ISQLEditStringBuilder

                oQR = New CSQLInsertStringBuilder("LoginLogs")
                oQR.AppendString("LoginLogID", SQLString(newGUID))
                oQR.AppendString("Login", SQLString(psLogin))
                oQR.AppendString("LoginDate", "GetUtcDate()")
                oQR.AppendString("Success", IIf(pbSuccess, "'Y'", "'N'").ToString)
                oQR.AppendString("RemoteIP", SQLString(psIP))
                If String.IsNullOrEmpty(psUserID) Then
                    oQR.AppendString("UserID", "NULL")
                Else
                    oQR.AppendString("UserID", SQLString(psUserID))
                End If

                oQR.AppendString("Domain", SQLString(psDomain))

                oDB2.executeNonQuery(oQR.SQL)

                If pbSuccess Then
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                    Select Case CType(System.Web.HttpContext.Current.Session("USER_TYPE"), EUserType)
                        Case EUserType.Agent
                            oQR = New CSQLUpdateStringBuilder("Agents", String.Format( _
                                                             "WHERE AgentID={0}" _
                                                             , SQLString(psUserID)))
                        Case EUserType.SuperAdmin
                            oQR = New CSQLUpdateStringBuilder("SuperAdmins", String.Format( _
                                                            "WHERE SuperAdminID={0}" _
                                                            , SQLString(psUserID)))
                        Case EUserType.Player
                            oQR = New CSQLUpdateStringBuilder("Players", String.Format( _
                                                            "WHERE PlayerID={0}" _
                                                            , SQLString(psUserID)))
                        Case EUserType.CallCenterAgent
                            oQR = New CSQLUpdateStringBuilder("CallCenterAgents", String.Format( _
                                                            "WHERE CallCenterAgentID={0}" _
                                                            , SQLString(psUserID)))
                    End Select

                    oQR.AppendString("LastLoginDate", SQLDate(Date.Now.ToUniversalTime))

                    oDB.executeNonQuery(CType(oQR, CSQLUpdateStringBuilder), "")
                End If

            Catch ex As Exception
                _log.Warn("Error updating loginlog/lastlogindate: " & ex.Message, ex)
            Finally
                If Not oDB Is Nothing Then oDB.closeConnection()
                If Not oDB2 Is Nothing Then oDB2.closeConnection()
            End Try
        End Sub

        Public Shared Sub IncreaseNumberAttempts(ByVal psLogin As String, ByVal psSiteType As String)
            Dim oDB As CSQLDBUtils = Nothing
            Dim sQR As String = ""
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                '' Check User's Type
                Dim sTableName As String = ""
                Dim bBreak As Boolean = False
                sQR = String.Format("select top 1 * from Agents where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                If oDB.getDataTable(sQR).Rows.Count > 0 Then
                    sTableName = "Agents"
                    bBreak = True
                End If
                If Not bBreak Then
                    sQR = String.Format("select top 1 * from Players where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sQR).Rows.Count > 0 Then
                        sTableName = "Players"
                        bBreak = True
                    End If
                End If

                If Not bBreak Then
                    sQR = String.Format("select top 1 * from SuperAdmins where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sQR).Rows.Count > 0 Then
                        sTableName = "SuperAdmins"
                        bBreak = True
                    End If
                End If

                If Not bBreak Then
                    sQR = String.Format("select top 1 * from CallCenterAgents where login ={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType))
                    If oDB.getDataTable(sQR).Rows.Count > 0 Then
                        sTableName = "CallCenterAgents"
                        bBreak = True
                    End If
                End If

                ' Exit thread if wrong Login
                If sTableName = "" Then
                    Exit Sub
                End If

                'check if the # is over 10
                Dim oDT As DataTable = oDB.getDataTable(String.Format("SELECT * FROM {0} WHERE Login={1} and SiteType={2}", sTableName, SQLString(psLogin), SQLString(psSiteType)))
                If oDT.Rows.Count > 0 Then
                    Dim nCount As Integer = SafeInteger(oDT.Rows(0)("NumFailedAttempts"))
                    Dim nNumLoginAttempt As Integer = SafeInteger(oDT.Rows(0)("NumLoginAttempts"))
                    If nNumLoginAttempt = 0 Then
                        nNumLoginAttempt = 10
                    End If
                    'increase the number of attempts
                    Dim oUpdate As New CSQLUpdateStringBuilder(sTableName, String.Format("WHERE Login={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType)))
                    oUpdate.AppendString("NumFailedAttempts", SafeString(nCount + 1))

                    sQR = oUpdate.SQL
                    oDB.executeNonQuery(oUpdate, "")

                    If nCount >= nNumLoginAttempt Then
                        oUpdate = New CSQLUpdateStringBuilder(sTableName, String.Format("WHERE Login={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType)))
                        oUpdate.AppendString("IsLocked", SQLString("Y"))
                        oUpdate.AppendString("LockReason", SQLString("Out of " & nNumLoginAttempt & " times login fail"))

                        sQR = oUpdate.SQL
                        oDB.executeNonQuery(oUpdate, "")

                        _log.Info("Lock User: " & psLogin & " - NumFailedAttempts: " & nCount.ToString)
                    End If
                End If

            Catch ex As Exception
                _log.DebugFormat("Can't execute query : {0}", sQR)
                'Throw New Exception(String.Format("Can't execute query : {0}", sQR))
            Finally
                If Not oDB Is Nothing Then oDB.closeConnection()
            End Try
        End Sub

        Public Shared Sub ResetNumberAttempts(ByVal psLogin As String, ByVal psSiteType As String)
            Dim oDB As CSQLDBUtils = Nothing
            Dim sQR As String = ""
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                Dim sTableName As String = "Players"
                Select Case CType(System.Web.HttpContext.Current.Session("USER_TYPE"), EUserType)
                    Case EUserType.Agent
                        sTableName = "Agents"
                    Case EUserType.SuperAdmin
                        sTableName = "SuperAdmins"
                End Select

                Dim oUpdate As New CSQLUpdateStringBuilder(sTableName, String.Format("WHERE Login={0} and SiteType={1}", SQLString(psLogin), SQLString(psSiteType)))
                oUpdate.AppendString("NumFailedAttempts", "0")

                sQR = oUpdate.SQL

                oDB.executeNonQuery(oUpdate, "")

            Catch ex As Exception
                _log.DebugFormat("Can't execute query : {0}", sQR)
                'Throw New Exception(String.Format("Can't execute query : {0}", sQR))
            Finally
                If Not oDB Is Nothing Then oDB.closeConnection()
            End Try
        End Sub

    End Class

End Namespace