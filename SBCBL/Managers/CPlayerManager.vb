Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CPlayerManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CPlayerManager))

        Public Function AutoGenerateLogin(ByVal psName As String, ByRef psSiteType As String, Optional ByVal pnMaxNum As Integer = 6) As String
            Dim sLoginStr As String = ""
            Dim bSuccess As Boolean = False

            While (Not bSuccess)
                sLoginStr = SafeString(Left(psName.Replace(" ", ""), 3))

                For nNum As Integer = 1 To pnMaxNum
                    sLoginStr = sLoginStr & RandomNumChar()
                Next

                'check if this login is exist in db
                Dim sError As String = ""
                bSuccess = Not IsExistedLogin(sLoginStr, "", sError, psSiteType)

                If sError <> "" Then
                    log.Error("Cannot auto generate login.")
                    Return ""
                End If

            End While

            Return sLoginStr
        End Function

        Public Function IsExistedLogin(ByVal psLogin As String, ByVal psPlayerID As String, ByRef sError As String, ByRef psSiteType As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Login=" & SQLString(psLogin))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''SuperAdmin
                sSQL = "SELECT count(Login) FROM SuperAdmins " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0

                ''Agent
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM Agents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''CallCenterAgent
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM CallCenterAgents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''Player
                If Not bExisted Then
                    oWhere.AppendOptionalANDCondition("PlayerID", SQLString(psPlayerID), "<>")
                    sSQL = "SELECT count(Login) FROM Players " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                    log.Debug("Cannot check login existed. SQL: " & sSQL)
                End If
            Catch ex As Exception
                sError = "Cannot check login existed"
                log.Error("Cannot check login existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function


        Public Function IsExistedName(ByVal psName As String, ByVal psPlayerID As String, ByRef sError As String, ByRef psSiteType As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Name=" & SQLString(psName))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''SuperAdmin
                sSQL = "SELECT count(Name) FROM SuperAdmins " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0

                ''Agent
                If Not bExisted Then
                    sSQL = "SELECT count(Name) FROM Agents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''CallCenterAgent
                If Not bExisted Then
                    sSQL = "SELECT count(Name) FROM CallCenterAgents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''Player
                If Not bExisted Then
                    oWhere.AppendOptionalANDCondition("PlayerID", SQLString(psPlayerID), "<>")
                    sSQL = "SELECT count(Name) FROM Players " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If
            Catch ex As Exception
                sError = "Cannot check Name existed"
                log.Error("Cannot check Name existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function IsExistedPhoneLogin(ByVal psPhoneLogin As String, ByVal psSiteType As String, Optional ByVal psPlayerID As String = "") As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PhoneLogin=" & SQLString(psPhoneLogin))
            oWhere.AppendOptionalANDCondition("PlayerID", SQLString(psPlayerID), "<>")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = "SELECT count(PhoneLogin) FROM Players " & oWhere.SQL
            log.Debug("Check phone login existed. SQL: " & sSQL)

            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check phone login existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function GetPlayers(ByVal psAgentID As String, ByVal bpIsBettingLocked As Boolean?, Optional ByVal pbIsLocked As Boolean = False, _
                                    Optional ByVal psNameOrLogin As String = "", Optional ByVal psPlayerTemplateID As String = "") As DataTable
            Dim odtPlayers As DataTable = Nothing
            'Dim drAgent As DataRow = (New CAgentManager()).GetAgentByAgentID(psAgentID)
            'Dim Agentletter As String = SafeString((New CAgentManager()).GetAgentByAgentID(psAgentID)("specialkey"))
            '' When create new superadmin we don't have agents
            '' So return nothing if AgentID = ""
            If psAgentID = "" Then
                Return odtPlayers
            End If

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("p.AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition(" ISNULL(p.IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                If bpIsBettingLocked Then
                    oWhere.AppendANDCondition(" ISNULL(p.IsBettingLocked, 'N')='Y'")
                Else
                    oWhere.AppendANDCondition(" ISNULL(p.IsBettingLocked, 'N')='N'")
                End If
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(p.Name LIKE {0} OR p.Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If
            oWhere.AppendOptionalANDCondition("p.PlayerTemplateID", SQLString(psPlayerTemplateID), "=")

            Dim sSQL As String = "SELECT p.*, (p.Login + ' (' + p.Name + ')') as FullName, t.TemplateName FROM Players p " & vbCrLf & _
                "INNER JOIN Agents a ON a.AgentID=p.AgentID " & vbCrLf & _
                "INNER JOIN PlayerTemplates t ON t.PlayerTemplateID=p.DefaultPlayerTemplateID " & vbCrLf & _
                oWhere.SQL & "order by FullName" & vbCrLf
            log.Debug("Get the list of players. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtPlayers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of players. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtPlayers
        End Function

        Public Function GetPlayer(ByVal psPlayerID As String, Optional ByVal pbCareLocked As Boolean = False) As CPlayer
            Dim odrPlayer As DataRow = GetPlayerDataRow(psPlayerID, pbCareLocked)

            If odrPlayer Is Nothing Then
                Return Nothing
            End If

            Return New CPlayer(odrPlayer)
        End Function

        Public Function GetPlayerDataRow(ByVal psPlayerID As String, Optional ByVal pbCareLocked As Boolean = False) As DataRow
            Dim odrPlayer As DataRow = Nothing

            Dim sSQL As String = "SELECT TOP 1 * FROM Players WHERE PlayerID=" & SQLString(psPlayerID) & _
                                    SafeString(IIf(pbCareLocked, " AND ISNULL(IsLocked,'')<>'Y'", ""))
            log.Debug("Get player infor. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim odtPlayer As DataTable = odbSQL.getDataTable(sSQL)

                If odtPlayer.Rows.Count > 0 Then
                    odrPlayer = odtPlayer.Rows(0)
                End If
            Catch ex As Exception
                log.Error("Cannot get player infor. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odrPlayer
        End Function

        Public Function GetPlayerByLogin(ByVal psUserName As String) As CPlayer
            Dim oPlayer As CPlayer = Nothing

            Dim sSQL As String = "SELECT TOP 1 * FROM Players WHERE Login= " & SQLString(psUserName)
            log.Debug("Get player infor. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim odtPlayer As DataTable = odbSQL.getDataTable(sSQL)

                If odtPlayer.Rows.Count > 0 Then
                    oPlayer = New CPlayer(odtPlayer.Rows(0))
                End If
            Catch ex As Exception
                log.Error("Cannot get player infor. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oPlayer
        End Function

        Public Function GetPlayerByPhone(ByVal psPhoneLogin As String, ByVal psPhonePassword As String, ByVal psSiteType As String) As CPlayer
            Dim oPlayer As CPlayer = Nothing

            Dim sSQL As String = "SELECT TOP 1 * FROM Players WHERE PhoneLogin=" & SQLString(psPhoneLogin) & _
                        " AND PhonePassword=" & SQLString(psPhonePassword) & _
                         " AND SiteType=" & SQLString(psSiteType)

            log.Debug("Get player infor by phone login. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim odtPlayer As DataTable = odbSQL.getDataTable(sSQL)

                If odtPlayer.Rows.Count > 0 Then
                    oPlayer = New CPlayer(odtPlayer.Rows(0))
                End If
            Catch ex As Exception
                log.Error("Cannot get player infor by phone login. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oPlayer
        End Function

        Public Function UpdateOriginalAmount(ByVal psPlayerID As String, ByVal pnOriginalAmount As Double) As String
            Dim oCache As New CCacheManager
            Dim oPlayerInfo = oCache.GetPlayerInfo(psPlayerID)

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID ='" + psPlayerID + "' ")
            oUpdate.AppendString("OriginalAmount", SQLString(pnOriginalAmount))
            Dim canUpdateBalanceAmount = oPlayerInfo.OriginalAmount = oPlayerInfo.BalanceAmount
            If canUpdateBalanceAmount Then
                oUpdate.AppendString("BalanceAmount", SQLString(pnOriginalAmount))
            End If
            log.Debug("Update OriginalAmount. SQL: " & oUpdate.SQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim message As String = String.Empty
            Try
                If odbSQL.executeNonQuery(oUpdate.SQL) > 0 Then
                    ''clear cache
                    oCache.ClearPlayerInfo(psPlayerID)
                    message = If(canUpdateBalanceAmount, "Update Successful", "Credit Limit will be applied for next week")
                Else
                    message = "Update Fail"
                End If
            Catch ex As Exception
                message = "Update Fail"
                log.Error("Cannot Update OriginalAmount. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return message
        End Function

        Public Function SetLockPlayers(ByVal poPlayerIDAndLogins As List(Of String), ByVal pbIsLock As Boolean) As Boolean
            Dim bSuccess As Boolean = True

            Dim oPlayerIDs As New List(Of String), oLogins As New List(Of String)

            For Each sItem As String In poPlayerIDAndLogins
                Dim oArgs As String() = sItem.Split("|"c) 'oArgs(0): PlayerID - oArgs(1):Login

                oPlayerIDs.Add(oArgs(0))
                oLogins.Add(oArgs(1))
            Next

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID IN ('" & Join(oPlayerIDs.ToArray(), "','") & "')")
            oUpdate.AppendString("IsLocked", SQLString(IIf(pbIsLock, "Y", "N")))
            log.Debug("Set lock player. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(oUpdate.SQL)

                ''clear cache
                Dim oCache As New CCacheManager

                For nIndex As Integer = 0 To poPlayerIDAndLogins.Count - 1
                    oCache.ClearPlayerInfo(oPlayerIDs(nIndex), oLogins(nIndex))
                Next

                '' Update Casino
                Dim oCasio As New CCasinoMananger()
                oCasio.LockAccount(oLogins, pbIsLock)

            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot set lock player. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function SetBettingLockPlayers(ByVal poPlayerIDAndLogins As List(Of String), ByVal pbIsBettingLocked As Boolean) As Boolean
            Dim bSuccess As Boolean = True

            Dim oPlayerIDs As New List(Of String), oLogins As New List(Of String)

            For Each sItem As String In poPlayerIDAndLogins
                Dim oArgs As String() = sItem.Split("|"c) 'oArgs(0): PlayerID - oArgs(1):Login

                oPlayerIDs.Add(oArgs(0))
                oLogins.Add(oArgs(1))
            Next
            Dim oUpdate As New CSQLUpdateStringBuilder("Players", String.Format("WHERE PlayerID IN ('{0}')", Join(oPlayerIDs.ToArray(), "','")))
            oUpdate.AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
            log.Debug("Seb betting lock players. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(oUpdate.SQL)

                ''clear cache
                Dim oCache As New CCacheManager

                For nIndex As Integer = 0 To poPlayerIDAndLogins.Count - 1
                    oCache.ClearPlayerInfo(oPlayerIDs(nIndex), oLogins(nIndex))
                Next

            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot set betting lock players. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function ChangePassword(ByVal psPlayerID As String, ByVal psCurrentPassword As String, ByVal psNewPassword As String, ByVal psChangedBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PlayerID=" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("Password=" & SQLString(psCurrentPassword))

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", oWhere.SQL)
            oUpdate.AppendString("Password", SQLString(psNewPassword))
            oUpdate.AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
            oUpdate.AppendString("RequirePasswordChange", SQLString("N"))

            log.Debug("Change password player. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psChangedBy) > 0
            Catch ex As Exception
                log.Error("Cannot change password player. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return False
        End Function

        Public Function ChangePhonePassword(ByVal psPlayerID As String, ByVal psNewPhonePassword As String, ByVal psChangedBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PlayerID=" & SQLString(psPlayerID))
            'oWhere.AppendANDCondition("PhonePassword=" & SQLString(psCurrentPhonePassword))

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", oWhere.SQL)
            oUpdate.AppendString("PhonePassword", SQLString(psNewPhonePassword))
            oUpdate.AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))

            log.Debug("Change phone password player. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psChangedBy) > 0
            Catch ex As Exception
                log.Error("Cannot change phone password player. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
        End Function

        Public Function InsertPlayer(ByVal psAgentID As String, ByVal poPlayer As CPlayer, ByVal psCreatedBy As String, ByVal psSiteType As String) As Boolean

            Dim bSuccess As Boolean = True

            Dim oInsert As New CSQLInsertStringBuilder("Players")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oInsert
                    .AppendString("PlayerID", SQLString(newGUID))
                    .AppendString("AgentID", SQLString(psAgentID))
                    .AppendString("Name", SQLString(poPlayer.Name))
                    .AppendString("Login", SQLString(poPlayer.Login))
                    .AppendString("Password", SQLString(poPlayer.Password))
                    .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime()))
                    .AppendString("TimeZone", SafeString(poPlayer.TimeZone))
                    .AppendString("PlayerTemplateID", SQLString(poPlayer.PlayerTemplateID))
                    .AppendString("BalanceAmount", SQLDouble(poPlayer.BalanceAmount))
                    .AppendString("BalanceForward", SQLDouble(0))
                    .AppendString("OriginalAmount", SQLDouble(poPlayer.BalanceAmount))
                    .AppendString("AlertMessage", SQLString(poPlayer.AlertMessage))
                    .AppendString("NumFailedAttempts", SafeString(poPlayer.NumFailedAttempts))
                    .AppendString("IsBettingLocked", SQLString(IIf(poPlayer.IsBettingLocked, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(poPlayer.HasCasino, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(poPlayer.IsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(poPlayer.LockReason))
                    .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("PhoneLogin", SQLString(poPlayer.PhoneLogin))
                    .AppendString("WagerLimit", SQLString(poPlayer.WagerLimit))
                    .AppendString("CreatedBy", SQLString(psCreatedBy))
                    .AppendString("SiteType", SQLString(psSiteType))
                    If poPlayer.Password <> "" Then
                        .AppendString("PhonePassword", SQLString(poPlayer.Password))
                    End If
                End With

                log.Debug("Insert player. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert.SQL)

                ''wrtie log
                'odbSQL.executeNonQuery(oInsert, psCreatedBy)

                ' Insert Casino Account
                Dim oCasino As New CCasinoMananger()
                oCasino.InsertAccount(poPlayer.Login, poPlayer.Password, poPlayer.Name, poPlayer.IsLocked, poPlayer.Template.CasinoMaxAmount)

            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to insert player. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function CreatePresetPlayers(ByVal psAgentID As String, ByVal pnNumberAccs As Integer, ByVal psPrefix As String, ByVal pnStartNumber As Integer, _
                                           ByVal pnTimeZone As Integer, ByVal psDefaultPlayerTemplateID As String, ByVal psCreatedBy As String, _
                                           ByVal psSiteType As String, ByVal pbRequirePasswordChange As Boolean) As Boolean

            If pnNumberAccs = 0 Then
                Return True
            End If

            If pnStartNumber = 0 Then
                pnStartNumber += 1
            End If

            Dim oInsert As New CSQLInsertStringBuilder("Players")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim bResult As Boolean = True
            Dim nAmmount As Double = 0
            If psDefaultPlayerTemplateID <> "" Then
                nAmmount = (New CPlayerTemplateManager()).GetAccountBalance(psDefaultPlayerTemplateID)
            End If

            Try
                Dim nTotalAcct As Integer = pnStartNumber + (pnNumberAccs - 1)
                'For nIndex As Integer = pnStartNumber To nTotalAcct
                '    If IsExistedLogin(psPrefix & nIndex.ToString(), "", "", GetSiteType()) Then
                '        nTotalAcct += 1
                '        Continue For
                '    End If
                Dim nIndex As Integer = pnStartNumber
                While nIndex <= nTotalAcct
                    If IsExistedLogin(psPrefix & nIndex.ToString(), "", "", GetSiteType()) Then
                        nTotalAcct += 1
                    Else
                        With oInsert
                            .AppendString("PlayerID", SQLString(newGUID))
                            .AppendString("AgentID", SQLString(psAgentID))
                            .AppendString("Name", SQLString(psPrefix & nIndex.ToString()))
                            .AppendString("Login", SQLString(psPrefix & nIndex.ToString()))
                            .AppendString("Password", SQLString("12345"))
                            .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime()))
                            .AppendString("TimeZone", SafeString(pnTimeZone))

                            If psDefaultPlayerTemplateID <> "" Then
                                .AppendString("DefaultPlayerTemplateID", SQLString(psDefaultPlayerTemplateID))
                            End If
                            .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                            .AppendString("PhoneLogin", SQLString(psPrefix & nIndex.ToString()))
                            .AppendString("CreatedBy", SQLString(psCreatedBy))
                            .AppendString("SiteType", SQLString(psSiteType))
                            .AppendString("PhonePassword", SQLString("12345"))
                            .AppendString("NumFailedAttempts", "5")
                            .AppendString("BalanceAmount", SafeString(nAmmount))
                            .AppendString("BalanceForward", SQLDouble(0))
                            .AppendString("OriginalAmount", SafeString(nAmmount))
                            .AppendString("RequirePasswordChange", SQLString(IIf(pbRequirePasswordChange, "Y", "N")))
                        End With
                        odbSQL.executeNonQuery(oInsert.SQL)
                        '' Insert Casino Account
                        Dim oCasino As New CCasinoMananger()
                        oCasino.InsertAccount(psPrefix & nIndex.ToString(), "12345", psPrefix & nIndex.ToString(), False, 0)
                    End If
                    nIndex += 1
                End While
                'Next
            Catch ex As Exception
                bResult = False
                log.Error("Can't create preset player: " & ex.Message, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bResult
        End Function


        Private Sub insertPlayerLimit(ByVal pnLimit As Double, ByVal psPlayerID As String, ByVal psCreatedBy As String)
            Dim lstSportTypes = GetAllSportType()
            Dim oPlayerLimitManager As New CPlayerLimitManager()
            For Each sSportType As String In lstSportTypes
                For Each sGameType As String In getListGameType(sSportType)
                    oPlayerLimitManager.InsertTemplateLimit(psPlayerID, sGameType, "Game", pnLimit, pnLimit, pnLimit, psCreatedBy)
                    oPlayerLimitManager.InsertTemplateLimit(psPlayerID, sGameType, "1st", pnLimit, pnLimit, pnLimit, psCreatedBy)
                    oPlayerLimitManager.InsertTemplateLimit(psPlayerID, sGameType, "2nd", pnLimit, pnLimit, pnLimit, psCreatedBy)
                    oPlayerLimitManager.InsertTemplateLimit(psPlayerID, sGameType, "Q", pnLimit, pnLimit, pnLimit, psCreatedBy)
                Next
            Next
        End Sub
        Public Function InsertPlayer(ByVal psPlayerID As String, ByVal psAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String _
                                     , ByVal pnTimeZone As Integer, ByVal psDefaultPlayerTemplateID As String, ByVal pnBalanceAmount As Double, _
                                     ByVal psAlertMessage As String, ByVal pnNumFailedAttempts As Integer, ByVal pbIsSuperPlayer As Boolean, _
                                     ByVal pbIsBettingLocked As Boolean, ByVal pbHasCasino As Boolean, ByVal pbIsLocked As Boolean, ByVal psLockReason As String, _
                                     ByVal psPhoneLogin As String, ByVal psPhonePassword As String, ByVal psCreatedBy As String, _
                                     ByVal psSiteType As String, ByVal pbRequirePasswordChange As Boolean, ByVal pIncreaseSpreadMoney As Double, ByVal pnWagerLimit As Double, ByVal pnParlayLimit As Double, ByVal pnTeaserLimit As Double, ByVal pnPropLimit As Double) As Boolean

            Dim bSuccess As Boolean = True

            Dim oInsert As New CSQLInsertStringBuilder("Players")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oInsert
                    .AppendString("PlayerID", SQLString(psPlayerID))
                    .AppendString("AgentID", SQLString(psAgentID))
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    .AppendString("Password", SQLString(psPassword))
                    .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("DefaultPlayerTemplateID", SQLString(psDefaultPlayerTemplateID))
                    .AppendString("BalanceAmount", SQLDouble(pnBalanceAmount))
                    .AppendString("BalanceForward", SQLDouble(0))
                    .AppendString("OriginalAmount", SQLDouble(pnBalanceAmount))
                    .AppendString("AlertMessage", SQLString(psAlertMessage))
                    .AppendString("NumFailedAttempts", SafeString(pnNumFailedAttempts))
                    .AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                    .AppendString("IsSuperPlayer", SQLString(IIf(pbIsSuperPlayer, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("CreatedBy", SQLString(psCreatedBy))
                    .AppendString("PhoneLogin", SQLString(psPhoneLogin))
                    .AppendString("SiteType", SQLString(psSiteType))
                    .AppendString("RequirePasswordChange", SQLString(IIf(pbRequirePasswordChange, "Y", "N")))
                    .AppendString("IncreaseSpreadMoney", SQLString(pIncreaseSpreadMoney))
                    .AppendString("WagerLimit", SQLString(pnWagerLimit))
                    .AppendString("ParlayLimit", SQLString(pnParlayLimit))
                    .AppendString("TeaserLimit", SQLString(pnTeaserLimit))
                    .AppendString("PropLimit", SQLString(pnPropLimit))
                    If psPhonePassword <> "" Then
                        .AppendString("PhonePassword", SQLString(psPhonePassword))
                    End If
                End With

                log.Debug("Insert player. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert.SQL)

                ' Insert Casino Account
                Dim oCasino As New CCasinoMananger()
                oCasino.InsertAccount(psLogin, psPassword, psName, pbIsLocked, 0)
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to insert player. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function


        Public Function UpdateLimitByUser(ByVal psPlayerID As String, ByVal pnBalanceAmount As Double) As Boolean
            Dim bSuccess As Boolean = True
            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "select BalanceAmount, OriginalAmount from Players where PlayerID= " & SQLString(psPlayerID)
            Dim oDT As DataTable = odbSQL.getDataTable(sSQL)
            Dim nOldBalanceAmount As Double = SafeDouble(oDT.Rows(0)("BalanceAmount"))
            Dim nOldOriginalAmount As Double = SafeDouble(oDT.Rows(0)("OriginalAmount"))
            If nOldBalanceAmount = nOldOriginalAmount Then
                Try
                    oUpdate.AppendString("BalanceAmount", SafeString(pnBalanceAmount))
                    oUpdate.AppendString("OriginalAmount", SafeString(pnBalanceAmount))
                    bSuccess = odbSQL.executeNonQuery(oUpdate, "") > 0

                    ''clear cache
                    log.Debug("debug  to save balance User. SQL: " & oUpdate.SQL)
                Catch ex As Exception
                    bSuccess = False
                    log.Error("Error trying to save balance User. SQL: " & oUpdate.SQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
                Return bSuccess
            End If
        End Function

        Public Function UpdatePlayerCasino(ByVal psAgentID As String, ByVal pbHasCasino As Boolean, ByVal psChangedBy As String) As Boolean

            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE AgentID= " & SQLString(psAgentID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim sSQL As String = "select PlayerID, [login] from Players WHERE AgentID= " & SQLString(psAgentID)
            Dim oDT As DataTable = odbSQL.getDataTable(sSQL)
            Try
                With oUpdate
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                End With

                log.Debug("Update player casino. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                Dim oCache As New CacheUtils.CCacheManager
                For Each odr As DataRow In oDT.Rows
                    oCache.ClearPlayerInfo(SafeString(odr("PlayerID")), SafeString(odr("Login")))
                Next

            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        ''' <summary>
        ''' psOldLogin: use to clear cache palyer infor
        ''' </summary>
        ''' 

        Public Function UpdatePlayer(ByVal psPlayerID As String, ByVal psName As String, ByVal psLogin As String, ByVal psOldLogin As String, ByVal psPassword As String _
                                     , ByVal pnTimeZone As Integer, ByVal psPlayerTemplateID As String _
                                     , ByVal psAlertMessage As String, ByVal pnNumFailedAttempts As Integer, ByVal pbIsBettingLocked As Boolean, ByVal pbHasCasino As Boolean _
                                     , ByVal pbIsLocked As Boolean, ByVal psLockReason As String, ByVal psPhoneLogin As String, ByVal psPhonePassword As String, ByVal psChangedBy As String, ByVal pIncreaseSpreadMoney As Double, ByVal pnWagerLimit As Double, ByVal pnParlayLimit As Double, ByVal pnTeaserLimit As Double, ByVal pnPropLimit As Double) As Boolean

            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Dim sSQL As String = "select BalanceAmount, OriginalAmount from Players where PlayerID= " & SQLString(psPlayerID)
                Dim oDT As DataTable = odbSQL.getDataTable(sSQL)
                Dim nOldBalanceAmount As Double = SafeDouble(oDT.Rows(0)("BalanceAmount"))
                Dim nOldOriginalAmount As Double = SafeDouble(oDT.Rows(0)("OriginalAmount"))

                With oUpdate
                    '' if this user has not bet, change hist ammount also
                    If psPlayerTemplateID <> "" AndAlso nOldBalanceAmount = nOldOriginalAmount Then
                        sSQL = "select  AccountBalance  from  PlayerTemplates where PlayerTemplateID = " & SQLString(psPlayerTemplateID)
                        Dim nNewAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
                        If nNewAmount <> 0 Then
                            .AppendString("BalanceAmount", SQLString(nNewAmount))
                            .AppendString("OriginalAmount", SQLString(nNewAmount))
                        End If
                    End If

                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    If psPassword <> "" Then
                        .AppendString("Password", SQLString(psPassword))
                        .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    End If
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    If Not String.IsNullOrEmpty(psPlayerTemplateID) Then
                        .AppendString("PlayerTemplateID", SQLString(psPlayerTemplateID))
                    End If
                    .AppendString("AlertMessage", SQLString(psAlertMessage))
                    .AppendString("NumFailedAttempts", SafeString(pnNumFailedAttempts))
                    .AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("PhoneLogin", SQLString(psPhoneLogin))
                    .AppendString("IncreaseSpreadMoney", SQLString(pIncreaseSpreadMoney))
                    .AppendString("WagerLimit", SQLString(pnWagerLimit))
                    .AppendString("ParlayLimit", SQLString(pnParlayLimit))
                    .AppendString("TeaserLimit", SQLString(pnTeaserLimit))
                    .AppendString("PropLimit", SQLString(pnPropLimit))
                    If psPhonePassword <> "" Then
                        .AppendString("PhonePassword", SQLString(psPhonePassword))
                    End If
                End With

                log.Debug("Update player. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
                If psLogin <> psOldLogin Then
                    oCache.ClearPlayerInfo(psPlayerID, psOldLogin)
                End If

                ' Update Casino Account
                Dim oCasino As New CCasinoMananger()
                oCasino.UpdateAccount(psOldLogin, psLogin, psPassword, psName, pbIsLocked)

            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function getPlayerTemplateID(ByVal psPlayerID As String) As String
            Dim oPlayer As CPlayer = GetPlayer(psPlayerID, Nothing)
            If String.IsNullOrEmpty(oPlayer.PlayerTemplateID) Then
                Return oPlayer.DefaultPlayerTemplateID
            Else
                Return oPlayer.PlayerTemplateID
            End If
        End Function

        Public Function UpdateAccountBalance(ByVal psPlayerID As String, ByVal pnAccountBalance As Integer, ByVal psChangedBy As String) As Boolean

            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("BalanceAmount", SQLString(pnAccountBalance))
                End With

                log.Debug("Update AccountBalance. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID)
                '' Update Casino Account
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save AccountBalance. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function


        Public Function UpdatePlayer(ByVal psPlayerID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, ByVal psChangedBy As String) As Boolean

            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                   
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    If psPassword <> "" Then
                        .AppendString("Password", SQLString(psPassword))
                        .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    End If
                End With

                log.Debug("Update player. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
                '' Update Casino Account
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function


        Public Function UpdatePlayer(ByVal psPlayerID As String, ByVal psAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psOldLogin As String, ByVal psPassword As String _
                                     , ByVal pnTimeZone As Integer, ByVal psDefaultPlayerTemplateID As String _
                                     , ByVal psAlertMessage As String, ByVal pnNumFailedAttempts As Integer, ByVal pbIsBettingLocked As Boolean, ByVal pbHasCasino As Boolean _
                                     , ByVal pbIsSuperPlayer As Boolean, ByVal pbIsLocked As Boolean, ByVal psLockReason As String, ByVal psPhoneLogin As String, ByVal psPhonePassword As String, ByVal psChangedBy As String, ByVal pIncreaseSpreadMoney As Double, Optional ByVal pnCreditAmount As Double = -1) As Boolean

            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Dim sSQL As String = "select BalanceAmount, OriginalAmount from Players where PlayerID= " & SQLString(psPlayerID)
                Dim oDT As DataTable = odbSQL.getDataTable(sSQL)
                Dim nOldBalanceAmount As Double = SafeDouble(oDT.Rows(0)("BalanceAmount"))
                Dim nOldOriginalAmount As Double = SafeDouble(oDT.Rows(0)("OriginalAmount"))

                With oUpdate
                    '' if this user has not bet, change hist ammount also
                    'If psDefaultPlayerTemplateID <> "" AndAlso nOldBalanceAmount = nOldOriginalAmount Then
                    '    sSQL = "select  AccountBalance  from  PlayerTemplates where PlayerTemplateID = " & SQLString(getPlayerTemplateID(psPlayerID))
                    '    Dim nNewAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
                    '    If nNewAmount <> 0 Then
                    '        .AppendString("BalanceAmount", SQLString(nNewAmount))
                    '        .AppendString("OriginalAmount", SQLString(nNewAmount))
                    '    End If
                    'End If

                    If psDefaultPlayerTemplateID <> "" Then
                        sSQL = "select  AccountBalance,CreditMaxAmount  from  PlayerTemplates where PlayerTemplateID = " & SQLString(getPlayerTemplateID(psPlayerID))
                        Dim dt As DataTable = odbSQL.getDataTable(sSQL)
                        'Dim nNewAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
                        If dt IsNot Nothing Then
                            If nOldBalanceAmount = nOldOriginalAmount Then
                                .AppendString("BalanceAmount", SQLString(dt.Rows(0)("AccountBalance")))
                                .AppendString("OriginalAmount", SQLString(dt.Rows(0)("AccountBalance")))
                            Else
                                .AppendString("BalanceAmount", SQLString(dt.Rows(0)("AccountBalance")))
                            End If
                            If pnCreditAmount > 0 Then
                                .AppendString("OriginalAmount", SQLString(pnCreditAmount))
                            End If

                        End If
                    End If


                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    If psPassword <> "" Then
                        .AppendString("Password", SQLString(psPassword))
                        .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    End If
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("DefaultPlayerTemplateID", SQLString(psDefaultPlayerTemplateID))
                    .AppendString("AlertMessage", SQLString(psAlertMessage))
                    .AppendString("NumFailedAttempts", SafeString(pnNumFailedAttempts))
                    .AppendString("IsSuperPlayer", SQLString(IIf(pbIsSuperPlayer, "Y", "N")))
                    .AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("AgentID", SQLString(psAgentID))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("PhoneLogin", SQLString(psPhoneLogin))
                    .AppendString("IncreaseSpreadMoney", SQLString(pIncreaseSpreadMoney))
                    If psPhonePassword <> "" Then
                        .AppendString("PhonePassword", SQLString(psPhonePassword))
                    End If
                End With

                log.Debug("Update player. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
                If psLogin <> psOldLogin Then
                    oCache.ClearPlayerInfo(psPlayerID, psOldLogin)
                End If

                '' Update Casino Account
                Dim oCasino As New CCasinoMananger()
                oCasino.UpdateAccount(psOldLogin, psLogin, psPassword, psName, pbIsLocked)

            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerTemplateID(ByVal psPlayerID As String, ByVal psPlayerTemplateID As String, ByVal psLogin As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID= " & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                With oUpdate
                    .AppendString("PlayerTemplateID", SQLString(psPlayerTemplateID))
                End With

                log.Debug("Update UpdatePlayerTemplateID. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerBalanceAmount(ByVal psPlayerID As String, ByVal dBalanceAMount As Double, ByVal psLogin As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PlayerID=" & SQLString(psPlayerID))
            Dim oUpdate As New CSQLUpdateStringBuilder("Players", oWhere.SQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("BalanceAmount", SQLString(dBalanceAMount))
                End With

                log.Debug("Update UpdatePlayerTemplateID. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerPresetManual(ByVal psPlayerID As String, ByVal psLogin As String, ByVal pbIsResetManual As Boolean, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("PlayerID=" & SQLString(psPlayerID))
            Dim oUpdate As New CSQLUpdateStringBuilder("Players", oWhere.SQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("IsResetManual", SQLString(IIf(pbIsResetManual, "Y", "N")))
                End With
                log.Debug("Update IsResetManual. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save IsResetManual User. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        ''' <summary>
        ''' psLogin: use to clear cache palyer infor
        ''' </summary>
        Public Function DeletePlayer(ByVal psPlayerID As String, ByVal psLogin As String, ByVal psDeletedBy As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim oDelete As New CSQLDeleteStringBuilder("Players", "WHERE PlayerID=" & SQLString(psPlayerID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                log.Debug("Delete player. SQL: " & oDelete.SQL)
                odbSQL.executeNonQuery(oDelete.SQL)

                ''wrtie log
                'odbSQL.executeNonQuery(oDelete, psDeletedBy)

                ''clear cache
                Dim oCache As New CacheUtils.CCacheManager
                oCache.ClearPlayerInfo(psPlayerID, psLogin)

            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to delete Player. SQL: " & oDelete.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function GetPlayerDashboard(ByVal psPlayerID As String, ByVal poStartDate As Date, ByVal poEndDate As Date, ByVal pnTimeZone As Integer) As DataTable
            ' Create Standard table
            Dim oDayOfWeeks As String() = {"Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Net"}
            Dim oDashboard As New DataTable
            Dim sWhere As String
            Dim oCurrentDate As Date
            oDashboard.Columns.Add("Title")
            For Each sDay In oDayOfWeeks
                oDashboard.Columns.Add(sDay, GetType(Double))
            Next
            oDashboard.Columns.Add("Pending")
            oDashboard.Columns.Add("ThisMonday", GetType(DateTime))
            Dim oTicketData As DataTable = getPlayerDashboardData(psPlayerID, GetLastMondayOfDate(poStartDate), poEndDate)

            If oTicketData IsNot Nothing Then
                Dim oStarMonday As Date = GetLastMondayOfDate(poStartDate)
                Dim oEndSunday As Date = GetNextSundayOfDate(poEndDate)

                '' make sure start date and end date is 00h AM
                oStarMonday = oStarMonday.AddHours(-oStarMonday.Hour)
                oEndSunday = oEndSunday.AddHours(-oEndSunday.Hour)

                Dim nTotalday As Integer = oEndSunday.Subtract(oStarMonday).Days

                For nWeek As Integer = 0 To (CInt(nTotalday / 7))
                    Dim oWeekRow As DataRow = oDashboard.NewRow()
                    oDashboard.Rows.Add(oWeekRow)

                    Dim sMon As String = "", sSun As String = ""
                    Dim nWeekTotal As Double = 0

                    For nDayOfWeek As Integer = 0 To 6
                        '' convert to GMT timzone, we subtract current timeto user timzeone
                        oCurrentDate = SafeDate(oStarMonday.AddDays(nWeek * 7 + nDayOfWeek).ToShortDateString())

                        'Dim sWhere As String = "GameDate = " & SQLString(oCurrentDate.ToString()) & " AND GameDate <" & SQLString(oCurrentDate.AddDays(1))
                        'oTicketData.DefaultView.RowFilter = sWhere
                        'Dim nCurrentdayValue As Double = SafeRound(oTicketData.Compute("SUM(DailyNetAmount)", sWhere))

                        sWhere = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending')  AND (TicketCompletedDate = " & SQLString(oCurrentDate.ToString()) & " AND TicketCompletedDate <" & SQLString(oCurrentDate.AddDays(1)) & ")"
                        Dim nCurrentdayValue As Double = SafeDouble(oTicketData.Compute("SUM(NetAmount)", sWhere))

                        nWeekTotal += nCurrentdayValue

                        Select Case oCurrentDate.DayOfWeek
                            Case DayOfWeek.Monday
                                oWeekRow("Mon") = nCurrentdayValue
                                sMon = oCurrentDate.ToString("MM/dd/yyyy")
                                oWeekRow("ThisMonday") = oCurrentDate

                            Case DayOfWeek.Tuesday
                                oWeekRow("Tues") = nCurrentdayValue

                            Case DayOfWeek.Wednesday
                                oWeekRow("Wed") = nCurrentdayValue

                            Case DayOfWeek.Thursday
                                oWeekRow("Thurs") = nCurrentdayValue

                            Case DayOfWeek.Friday
                                oWeekRow("Fri") = nCurrentdayValue

                            Case DayOfWeek.Saturday
                                oWeekRow("Sat") = nCurrentdayValue

                            Case Else
                                oWeekRow("Sun") = nCurrentdayValue
                                sSun = oCurrentDate.ToString("MM/dd/yyyy")
                        End Select
                    Next


                    sWhere = String.Format("ISNULL(TicketStatus,'Open') IN ('Open', 'Pending') AND " & _
                                                     " TransactionDate <={0}", SQLString(Format(oCurrentDate, "MM/dd/yyyy")))

                    oWeekRow("Pending") = SafeDouble(oTicketData.Compute("SUM(PendingAmount)", sWhere))
                    oWeekRow("Net") = nWeekTotal
                    oWeekRow("Title") = sMon & " - " & sSun
                Next
            End If
            Return oDashboard
        End Function

        Private Function getPlayerDashboardData(ByVal psPlayerID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim oDB As CSQLDBUtils = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("p.PlayerID = " & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(p.IsLocked,'N')<>'Y'")
            'oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'') NOT IN ('Open', 'Pending')")
            'oWhere.AppendANDCondition(getSQLDateRange("g.ameDate", poStarDate, poEndDate))

            If poStartDate = Nothing Then
                oWhere.AppendANDCondition(String.Format("(t.TicketCompletedDate >= {0} OR ISNULL(t.TicketStatus, 'Open') IN ('Open','Pending'))", _
                                                        SQLString(Format(GetMondayOfCurrentWeek, "yyyy-MM-dd"))))
            Else
                oWhere.AppendANDCondition("( " & getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poEndDate)) & _
                                          " OR ISNULL(t.TicketStatus, 'Open')  IN ('Open','Pending'))")
            End If

            Dim sSQL As String = " select Convert(varchar(40),t.PlayerID) AS PlayerID, (t.NetAmount- t.RiskAmount) as NetAmount, " & _
            "t.RiskAmount AS PendingAmount,t.NetAmount as TicketNetAmount, " & _
            "CAST(CONVERT(NVARCHAR(10), t.TicketCompletedDate, 101) AS SMALLDATETIME) AS TicketCompletedDate, " & _
            "CAST(CONVERT(NVARCHAR(10), t.TransactionDate, 101) AS SMALLDATETIME) AS TransactionDate, t.TicketStatus  " & _
            " FROM Tickets t INNER JOIN  Players p ON p.PlayerID = t.PlayerID " & vbCrLf & oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.PlayerID, t.TransactionDate, t.TicketCompletedDate, t.NetAmount,t.RiskAmount, TicketNumber, SubTicketNumber,t.TicketStatus "


            log.Debug("Get the list of player dashboard. SQL: " & sSQL)

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get PlayersDashboard.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetBalanceAmount(ByVal psPlayerID As String) As Double
            Dim nBalance As Double = 0

            Dim sSQL As String = "SELECT BalanceAmount FROM Players WHERE PlayerID=" & SQLString(psPlayerID)
            'log.Debug("Get balance amount player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                nBalance = SafeDouble(odbSQL.getScalerValue(sSQL))

            Catch ex As Exception
                log.Error("Fails to get  balance amount player. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nBalance
        End Function

        Public Function CheckPassword(ByVal psPlayerID As String, ByVal psPassword As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing

            Dim sSQL As String = String.Format("SELECT COUNT(*) FROM Players WHERE PlayerID={0} AND Password={1}", _
                                               SQLString(psPlayerID), SQLString(psPassword))

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return SafeInteger(oDB.getScalerValue(sSQL)) > 0

            Catch ex As Exception
                log.Error("Fails to check player's password. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function CheckPhonePassword(ByVal psPlayerID As String, ByVal psPhonePassword As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing

            Dim sSQL As String = String.Format("SELECT COUNT(*) FROM Players WHERE PlayerID={0} AND PhonePassword={1}", _
                                               SQLString(psPlayerID), SQLString(psPhonePassword))

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return SafeInteger(oDB.getScalerValue(sSQL)) > 0

            Catch ex As Exception
                log.Error("Fails to check player's phonepassword. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function ResetOriginalAmount(ByVal psPlayerID As String, ByVal pnNewOriginalAmount As Double, ByVal psChangeBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing

            Dim sSQL As String = ""

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                Dim oUpdate As New CSQLUpdateStringBuilder("Players", "WHERE PlayerID = " & SQLString(psPlayerID))

                Dim nBalanceForward As Double
                If pnNewOriginalAmount = 0 Then
                    sSQL = "SELECT BalanceAmount, OriginalAmount, BalanceForward FROM Players WHERE PlayerID = " & SQLString(psPlayerID)
                    Dim oTbl As DataTable = oDB.getDataTable(sSQL)
                    If oTbl IsNot Nothing AndAlso oTbl.Rows.Count > 0 Then
                        Dim nBalanceAmount, nOriginalAmount, nBalanceOwed As Double
                        nBalanceAmount = SafeDouble(oTbl.Rows(0)("BalanceAmount"))
                        nOriginalAmount = SafeDouble(oTbl.Rows(0)("OriginalAmount"))
                        nBalanceForward = SafeDouble(oTbl.Rows(0)("BalanceForward"))
                        nBalanceOwed = nBalanceAmount + nBalanceForward - nOriginalAmount

                        oUpdate.AppendString("BalanceForward", SQLDouble(nBalanceOwed))
                    End If
                Else
                    oUpdate.AppendString("BalanceForward", SQLDouble(0))
                End If

                oUpdate.AppendString("BalanceAmount", SQLDouble(pnNewOriginalAmount))
                oUpdate.AppendString("OriginalAmount", SQLDouble(pnNewOriginalAmount))
                sSQL = oUpdate.SQL
                log.Debug("Reset Player's Balance Amount. SQL: " & sSQL)
                bResult = oDB.executeNonQuery(oUpdate, psChangeBy) > 0


            Catch ex As Exception
                log.Error("Fails to check player's Balance Amount. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Sub SendOneTimeMessage(ByVal psMessage As String, ByVal psSiteType As String)

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SiteType = " & SQLString(psSiteType))
            Dim sSQL As String = "update Players set OneTimeMessage = isnull(OneTimeMessage,'') +  " + SQLString(psMessage + "<split>") & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oDB.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Fails to Send player's Message. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
        End Sub

        Public Sub ResetOneTimeMessage(ByVal psPlayerID As String, ByVal psSiteType As String)
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SiteType = " & SQLString(psSiteType))
            oWhere.AppendANDCondition("PlayerID = " & SQLString(psPlayerID))
            Dim sSQL As String = "update Players set OneTimeMessage = '' " & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oDB.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Fails to reset player's Message. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
        End Sub

        Public Function GetAllPlayerIDByListAgentID(ByVal oLstAgentsID As List(Of String)) As List(Of String)
            Dim oLstPlayerIDs As New List(Of String)
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ISNULL(Players.IsLocked,'N')<>'Y'")
            oWhere.AppendANDCondition(String.Format("AgentID IN ('{0}')", Join(oLstAgentsID.ToArray(), "','")))
            Dim sSQL As String = "SELECT PlayerID FROM Players " & oWhere.SQL

            Try
                log.Debug("Get All PlayerID By ListAgentsID. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                For Each oDR As DataRow In oDT.Rows
                    oLstPlayerIDs.Add(SafeString(oDR("PlayerID")))
                Next
            Catch ex As Exception
                log.Error("Fails to get PlayerID by ListAgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return oLstPlayerIDs
        End Function

        Public Function GetAllPlayerByListAgentID(ByVal oLstAgentsID As List(Of String)) As DataTable
            Dim odtPlayer As New DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ISNULL(Players.IsLocked,'N')<>'Y'")
            oWhere.AppendANDCondition(String.Format("AgentID IN ('{0}')", Join(oLstAgentsID.ToArray(), "','")))
            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName FROM Players " & oWhere.SQL

            Try
                log.Debug("Get All PlayerID By ListAgentsID. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                odtPlayer = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get PlayerID by ListAgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return odtPlayer
        End Function

        Public Function GetPlayerByKeywords(ByVal sKeywords As String, ByVal poAgentID As String) As DataTable
            Dim odtPlayers As New DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("Login like '%" & sKeywords & "%'")
            oWhere.AppendANDCondition("Name like '%" & sKeywords & "%'")
            oWhere.AppendANDCondition("AgentID='" & SafeString(poAgentID) & "'")
            Dim sSQL As String = "select * FROM Players" & oWhere.SQL
            ' Dim sSQL As String = "select PlayerID,Name,Login,LastLoginDate,AgentID,Password,Islocked,BalanceAmount,IsBettingLocked,PlayerTemplateID,DefaultPlayerTemplateID,HasCasino from Players" & oWhere.SQL
            Try
                log.Debug("Get Players by Keywords. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                odtPlayers = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Players by keywords.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return odtPlayers
        End Function

        Public Function GetNumPlayersByAgentID(ByVal sAgentsID As String) As DataTable
            Dim odtPlayer As New DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition(String.Format("AgentID = '{0}'", sAgentsID))
            Dim sSQL As String = "SELECT Count([Login]) as NumPlayer FROM Players " & oWhere.SQL
            Try
                log.Debug("GetNumPlayersByAgentID. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                odtPlayer = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get GetNumPlayersByAgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return odtPlayer
        End Function
        Public Function GetPlayersByAgentID(ByVal sAgentsID As String) As DataTable
            Dim odtPlayer As New DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            'Dim Agentletter As String = SafeString((New CAgentManager()).GetAgentByAgentID(sAgentsID)("specialkey"))
            oWhere.AppendANDCondition(String.Format("AgentID = '{0}'", sAgentsID))
            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName FROM Players " & oWhere.SQL
            sSQL += "order by FullName "
            Try
                log.Debug("Get All PlayerID By ListAgentsID. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                odtPlayer = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get PlayerID by ListAgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return odtPlayer
        End Function

        Public Function getNumPlayerByAgentID(ByVal psAgentID As String) As Integer
            Dim oDB As CSQLDBUtils = Nothing
            oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Return SafeInteger(oDB.getScalerValue("select count(PlayerID) from Players  where AgentID='" & psAgentID & "' "))
            Catch ex As Exception
                log.Error("Fails to get num Player by AgentID. ", ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return 0
        End Function

        Public Function GetAllPlayer(ByVal psSiteType As String) As DataTable
            Dim odtPlayer As New DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ISNULL(Players.IsLocked,'N')<>'Y'")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName,(Name+' ('+PhoneLogin+')') as FullPhoneName  FROM Players " & oWhere.SQL & _
            " ORDER BY Login, Name"

            Try
                log.Debug("Get All PlayerD from table Players. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                odtPlayer = oDB.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Fails to get all Player from table Players.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return odtPlayer
        End Function

        Public Function CreditBack(ByVal psPlayerID As String, ByVal psAgentID As String, _
                                   ByVal pnAddAmount As Double, ByVal psDescription As String, _
                                   ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oSQLEdit As ISQLEditStringBuilder

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                Dim oEstDate As Date = GetEasternDate()

                '' 1st: Get Player's Balance Amount
                Dim nCurrentBalanceAmount = GetBalanceAmount(psPlayerID)

                '' 2nd: Create Games
                Dim sGameID As String = newGUID()
                oSQLEdit = New CSQLInsertStringBuilder("Games")
                oSQLEdit.AppendString("GameID", SQLString(sGameID))
                oSQLEdit.AppendString("GameDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("HomeTeam", SQLString(psDescription))
                oSQLEdit.AppendString("GameStatus", SQLString("Credit Back"))
                oSQLEdit.AppendString("FinalCheckStartedProcessing", SQLDate(oEstDate))
                oSQLEdit.AppendString("FinalCheckCompleted", SQLDate(oEstDate))
                oSQLEdit.AppendString("FirstHalfProcessedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("FirstQuaterProcessedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("SecondQuaterProcessedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("ThirdQuaterProcessedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("GameSuspendProcessedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("CheckCompletedDate", SQLDate(oEstDate))
                oDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

                '' 3rd: Create GameLines
                Dim sGameLineID As String = newGUID()
                oSQLEdit = New CSQLInsertStringBuilder("GameLines")
                oSQLEdit.AppendString("GameLineID", SQLString(sGameLineID))
                oSQLEdit.AppendString("GameID", SQLString(sGameID))
                oSQLEdit.AppendString("LastUpdated", SQLDate(oEstDate))
                oSQLEdit.AppendString("Context", SQLString("Current"))
                oSQLEdit.AppendString("HomeMoneyLine", SQLDouble(100))
                oDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

                '' 4th: Create Tickets
                Dim sTicketID As String = newGUID()
                oSQLEdit = New CSQLInsertStringBuilder("Tickets")
                oSQLEdit.AppendString("TicketID", SQLString(sTicketID))
                'oSQLEdit.AppendString("TicketNumber", SQLDouble(0))
                'oSQLEdit.AppendString("SubTicketNumber", SQLDouble(0))
                oSQLEdit.AppendString("AgentID", SQLString(psAgentID))
                oSQLEdit.AppendString("PlayerID", SQLString(psPlayerID))
                oSQLEdit.AppendString("TransactionDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("OrderBy", SQLString(psUpdateBy))
                oSQLEdit.AppendString("TicketType", SQLString("Straight"))

                If pnAddAmount > 0 Then
                    oSQLEdit.AppendString("RiskAmount", SQLDouble(0))
                    oSQLEdit.AppendString("WinAmount", SQLDouble(pnAddAmount))
                    oSQLEdit.AppendString("NetAmount", SQLDouble(pnAddAmount))
                    oSQLEdit.AppendString("TicketStatus", SQLString("WIN"))
                    oSQLEdit.AppendString("BetAmount", SQLDouble(0))
                Else
                    oSQLEdit.AppendString("RiskAmount", SQLDouble(Math.Abs(pnAddAmount)))
                    oSQLEdit.AppendString("WinAmount", SQLDouble(Math.Abs(pnAddAmount)))
                    oSQLEdit.AppendString("NetAmount", SQLDouble(0))
                    oSQLEdit.AppendString("TicketStatus", SQLString("LOSE"))
                    oSQLEdit.AppendString("BetAmount", SQLDouble(Math.Abs(pnAddAmount)))
                End If

                oSQLEdit.AppendString("TypeOfBet", SQLString("Internet"))
                oSQLEdit.AppendString("TicketCompletedDate", SQLDate(oEstDate))
                oSQLEdit.AppendString("NumOfBets", SQLDouble(1))
                oSQLEdit.AppendString("CheckCompletedDate", SQLDate(oEstDate))
                oDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

                '' 5th: Insert TicketBets
                oSQLEdit = New CSQLInsertStringBuilder("TicketBets")
                oSQLEdit.AppendString("TicketBetID", "NEWID()")
                oSQLEdit.AppendString("TicketID", SQLString(sTicketID))
                oSQLEdit.AppendString("BetType", SQLString("MoneyLine"))
                oSQLEdit.AppendString("GameID", SQLString(sGameID))
                oSQLEdit.AppendString("HomeMoneyLine", SQLDouble(100))

                If pnAddAmount > 0 Then
                    oSQLEdit.AppendString("TicketBetStatus", SQLString("WIN"))
                Else
                    oSQLEdit.AppendString("TicketBetStatus", SQLString("LOSE"))
                End If

                oSQLEdit.AppendString("GameLineID", SQLString(sGameLineID))
                oSQLEdit.AppendString("Context", SQLString("Current"))
                oDB.executeNonQuery(oSQLEdit, psUpdateBy, True)

                '' 6th: Update Player's Amount
                oSQLEdit = New CSQLUpdateStringBuilder("Players", "WHERE PlayerID = " & SQLString(psPlayerID))
                oSQLEdit.AppendString("BalanceAmount", SQLDouble(nCurrentBalanceAmount + pnAddAmount))

                oDB.executeNonQuery(oSQLEdit, psUpdateBy, True)
                oDB.commitTransaction()

                Return True
            Catch ex As Exception
                log.Error("Can't credit back to player:" & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function
    End Class

End Namespace