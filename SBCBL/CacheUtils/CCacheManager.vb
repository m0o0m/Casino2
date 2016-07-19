Imports System.Collections.Specialized
Imports System.Web
Imports System.Xml
Imports log4net
Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.Utils
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils

Namespace CacheUtils

    <Serializable()> _
    Public Class CCacheManager
        <NonSerialized()> Private Shared log As ILog = log4net.LogManager.GetLogger(GetType(CCacheManager))
        Private Const CACHE_TIME As Integer = 20 'minutes until each cache entry expires
        Private Const CACHE_TIME_LOAN_COUNT As Integer = 10 'minutes until each cache entry expires

#Region "USER METHODS"
        Public Function GetAgentInfo(ByVal psAgentID As String) As CAgent
            Dim sKey As String = "AGENT_INFO_" & psAgentID
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
                    Dim oAgentInfo As CAgent
                    If psAgentID = "Test" Then
                        oAgentInfo = New CAgent()
                    Else
                        oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                        Dim oWhere As New CSQLWhereStringBuilder()
                        oWhere.AppendANDCondition("AgentID = " & SQLString(psAgentID))
                        oWhere.AppendANDCondition("ISNULL(IsLocked,'')<>" & SQLString("Y"))

                        Dim oDT As DataTable = oDB.getDataTable("SELECT * FROM Agents " & oWhere.SQL)

                        If oDT.Rows.Count > 0 Then
                            oAgentInfo = New CAgent(oDT.Rows(0))
                        Else
                            Throw New CCacheManagerException("Unable to find agent user: " & psAgentID)
                        End If
                    End If

                    HttpContext.Current.Cache.Add(sKey, oAgentInfo, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                    HttpContext.Current.Cache.Add("USER_NAME_" & psAgentID, oAgentInfo.Name, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Catch ex As Exception
                    log.Error("Fail to GetUserInfo By ID: " & psAgentID, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If
            Return CType(HttpContext.Current.Cache(sKey), CAgent)
        End Function

        Public Sub ClearAgentInfo(ByVal psAgentID As String, ByVal psLoginName As String)
            Dim sUserName As String = psLoginName

            HttpContext.Current.Cache.Remove("AGENT_INFO_" & psAgentID)
            HttpContext.Current.Cache.Remove("ROLE_" & psLoginName)
            HttpContext.Current.Cache.Remove("USER_NAME_" & psAgentID)

        End Sub

        Public Function GetCallCenterAgentInfo(ByVal psCallCenterAgentID As String) As CCallCenterAgent
            Dim sKey As String = "CALL_CENTER_AGENT_INFO_" & psCallCenterAgentID
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
                    Dim oCCAgentInfo As CCallCenterAgent = Nothing
                    If psCallCenterAgentID = "Test" Then
                        oCCAgentInfo = New CCallCenterAgent()
                    Else
                        oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                        Dim oWhere As New CSQLWhereStringBuilder()
                        oWhere.AppendANDCondition("CallCenterAgentID = " & SQLString(psCallCenterAgentID))
                        oWhere.AppendANDCondition("ISNULL(IsLocked,'')<>" & SQLString("Y"))

                        Dim oDT As DataTable = oDB.getDataTable("SELECT * FROM CallCenterAgents " & oWhere.SQL)

                        If oDT.Rows.Count > 0 Then
                            oCCAgentInfo = New CCallCenterAgent(oDT.Rows(0))
                        Else
                            Throw New CCacheManagerException("Unable to find CallCenterAgent user: " & psCallCenterAgentID)
                        End If
                    End If

                    HttpContext.Current.Cache.Add(sKey, oCCAgentInfo, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                    HttpContext.Current.Cache.Add("USER_NAME_" & psCallCenterAgentID, oCCAgentInfo.Name, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Catch ex As Exception
                    log.Error("Fail to GetUserInfo By ID: " & psCallCenterAgentID, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If
            Return CType(HttpContext.Current.Cache(sKey), CCallCenterAgent)
        End Function

        Public Sub ClearCallCenterAgentInfo(ByVal psCallCenterAgentID As String, ByVal psLoginName As String)
            Dim sUserName As String = psLoginName

            HttpContext.Current.Cache.Remove("CALL_CENTERAGENT_INFO_" & psCallCenterAgentID)
            HttpContext.Current.Cache.Remove("ROLE_" & psLoginName)
            HttpContext.Current.Cache.Remove("USER_NAME_" & psCallCenterAgentID)

        End Sub

        Public Function GetBetLimitJuice(ByVal psPlayerID As String) As List(Of CPlayerTemplateLimit)
            Dim sKey As String = "PLAYER_TEMPLATE_LIMIT_" & psPlayerID
            Dim lstPlayerTemplateLimit As New List(Of CPlayerTemplateLimit)
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim odt As DataTable = (New CPlayerLimitManager).GetDataTableJuice(psPlayerID, getAllGameType())
                For Each odr As DataRow In odt.Rows
                    Dim oPlayerTemplateLimit As CPlayerTemplateLimit = New CPlayerTemplateLimit(odr)
                    lstPlayerTemplateLimit.Add(oPlayerTemplateLimit)
                Next
                HttpContext.Current.Cache.Add(sKey, lstPlayerTemplateLimit, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(HttpContext.Current.Cache(sKey), List(Of CPlayerTemplateLimit))
        End Function

        Public Sub ClearBetLimitJuice(ByVal psPlayerID As String)
            Dim sKey As String = "PLAYER_TEMPLATE_LIMIT_"
            sKey = "PLAYER_TEMPLATE_LIMIT_" & psPlayerID
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        Public Function GetPlayerInfo(ByVal psPlayerID As String, Optional ByVal pbCareLocked As Boolean = True) As CPlayer
            Dim sKey As String = "PLAYER_INFO_" & psPlayerID

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oPlayerInfo As CPlayer = Nothing

                If psPlayerID = "Test" Then
                    oPlayerInfo = New CPlayer()
                Else
                    oPlayerInfo = (New CPlayerManager).GetPlayer(psPlayerID, pbCareLocked)

                    If oPlayerInfo Is Nothing Then
                        Throw New CCacheManagerException("Unable to find player user: " & psPlayerID)
                    End If
                End If

                HttpContext.Current.Cache.Add(sKey, oPlayerInfo, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                HttpContext.Current.Cache.Add("USER_NAME_" & psPlayerID, oPlayerInfo.Name, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If

            Return CType(HttpContext.Current.Cache(sKey), CPlayer)
        End Function

        Public Sub ClearPlayerInfo(ByVal psPlayerID As String, Optional ByVal psLoginName As String = "")
            Dim sUserName As String = psLoginName
            Dim sKey As String = ""
            If sUserName = "" Then
                sUserName = Me.GetPlayerInfo(psPlayerID, False).Login
            End If
            sKey = "PLAYER_INFO_" & psPlayerID
            HttpContext.Current.Cache.Remove(sKey)
            sKey = "ROLE_" & psLoginName
            HttpContext.Current.Cache.Remove(sKey)
            sKey = "USER_NAME_" & psPlayerID
            HttpContext.Current.Cache.Remove(sKey)

        End Sub

        Public Function GetSuperAdminInfo(ByVal psSuperAdminID As String) As CSuperAdmin
            Dim sKey As String = "SUPERADMIN_INFO_" & psSuperAdminID
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
                    Dim oSuperAdminInfo As CSuperAdmin = Nothing
                    If psSuperAdminID = "Text" Then
                        oSuperAdminInfo = New CSuperAdmin()
                    Else
                        oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                        Dim oData As DataTable = oDB.getDataTable( _
                        "SELECT * FROM SuperAdmins   where SuperAdminID=" & SQLString(psSuperAdminID))

                        If oData.Rows.Count = 0 Then
                            Throw New CCacheManagerException("Unable to find SuperAdmin user: " & psSuperAdminID)
                        End If

                        oSuperAdminInfo = New CSuperAdmin(oData.Rows(0))
                    End If

                    HttpContext.Current.Cache.Add(sKey, oSuperAdminInfo, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Catch ex As Exception
                    log.Error("Fail to load SuperAdminInfo By ID: " & psSuperAdminID & " " & ex.Message, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If

            Return CType(HttpContext.Current.Cache(sKey), CSuperAdmin)
        End Function

        Public Sub ClearSuperAdminInfo(ByVal psSuperAdminID As String, Optional ByVal psLoginName As String = "")
            Dim psSuperAdminName As String = psLoginName

            If psSuperAdminName = "" Then
                psSuperAdminName = Me.GetSuperAdminInfo(psSuperAdminID).Login
            End If

            HttpContext.Current.Cache.Remove("SUPERADMIN_INFO_" & psSuperAdminID)
            HttpContext.Current.Cache.Remove("ROLE_" & psSuperAdminName)
        End Sub

#End Region

#Region "Teaser Rules"

        Public Function GetTeaserRuleInfo(ByVal psTeaserRuleID As String) As CTeaserRule
            Dim sKey As String = "TEASER_RULE_INFO_" & psTeaserRuleID
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oTeaserRuleManager As New CTeaserRuleManager()
                Dim otbl As DataTable = oTeaserRuleManager.GetByID(psTeaserRuleID)

                If otbl IsNot Nothing AndAlso otbl.Rows.Count > 0 Then
                    Dim oTeaserRule As New CTeaserRule(otbl.Rows(0))

                    HttpContext.Current.Cache.Add(sKey, oTeaserRule, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Else
                    Throw New CCacheManagerException("Unable to find TeaserRule: " & psTeaserRuleID)
                End If
            End If

            Return CType(HttpContext.Current.Cache(sKey), CTeaserRule)
        End Function

        Public Function GetTeaserRules() As CTeaserRuleList
            Dim sKey As String = String.Format("{0}_TEASER_RULE_LIST", SBCBL.std.GetSiteType)
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oTeaserRuleManager As New CTeaserRuleManager()
                Dim otbl As DataTable = oTeaserRuleManager.GetTeaserRules(Nothing, SBCBL.std.GetSiteType)

                If otbl IsNot Nothing Then
                    Dim olstTeaserRule As New CTeaserRuleList(otbl)

                    HttpContext.Current.Cache.Add(sKey, olstTeaserRule, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Else
                    Throw New CCacheManagerException("Unable to get TeaserRules")
                End If
            End If

            Return CType(HttpContext.Current.Cache(sKey), CTeaserRuleList)
        End Function

        Public Sub ClearTeaserRuleInfo(ByVal psTeaserRuleID As String)
            HttpContext.Current.Cache.Remove("TEASER_RULE_INFO_" & psTeaserRuleID)
            ClearTeaserRules()
        End Sub

        Public Sub ClearTeaserRules()
            HttpContext.Current.Cache.Remove(String.Format("{0}_TEASER_RULE_LIST", SBCBL.std.GetSiteType))
        End Sub

#End Region

#Region "Sys Settings"
        Public Function GetAllSysSettings(ByVal psCategorty As String) As CSysSettingList
            Dim sKey As String = "SBC_ALL_SYS_SETTINGS_" & UCase(psCategorty)

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oSettings As CSysSettingList
                oSettings = (New CSysSettingManager).GetAllSysSettings(psCategorty)

                HttpContext.Current.Cache.Add(sKey, oSettings, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If

            Return CType(HttpContext.Current.Cache(sKey), CSysSettingList)
        End Function

        Public Sub ClearAllSysSettings(ByVal psCategorty As String)
            HttpContext.Current.Cache.Remove("SBC_ALL_SYS_SETTINGS_" & UCase(psCategorty))
        End Sub

        Public Function GetSysSettings(ByVal psCategorty As String) As CSysSettingList
            Dim sKey As String = "SBC_SYS_SETTINGS_" & UCase(psCategorty)

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oSettings As New CSysSettingList
                Dim odtSettings As DataTable = (New CSysSettingManager).GetSysSettings(psCategorty)

                If odtSettings IsNot Nothing Then
                    For Each odrSetting As DataRow In odtSettings.Rows
                        If SafeString(odrSetting("Key")) <> "" Then
                            oSettings.Add(New CSysSetting(odrSetting))
                        End If
                    Next
                End If

                HttpContext.Current.Cache.Add(sKey, oSettings, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If

            Return CType(HttpContext.Current.Cache(sKey), CSysSettingList)
        End Function

        Public Function GetSysSettingAgentBookMakers(ByVal psCategorty As String, ByVal psAgentCategorty As String) As CSysSettingList
            Dim sKey As String = "AGENT_BOOKMAKER_" & psAgentCategorty & UCase(psCategorty)

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oSettings As New CSysSettingList
                Dim odtSettings As DataTable = (New CSysSettingManager).GetSysSettings(psCategorty)

                If odtSettings IsNot Nothing Then
                    For Each odrSetting As DataRow In odtSettings.Rows
                        If SafeString(odrSetting("Key")) <> "" AndAlso SafeString(odrSetting("Key")) <> "Manipulation" Then
                            oSettings.Add(New CSysSetting(odrSetting))
                        End If
                    Next
                End If

                HttpContext.Current.Cache.Add(sKey, oSettings, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If

            Return CType(HttpContext.Current.Cache(sKey), CSysSettingList)
        End Function

        Public Sub ClearSysSettings(ByVal psCategorty As String)
            HttpContext.Current.Cache.Remove("SBC_SYS_SETTINGS_" & UCase(psCategorty))
        End Sub

        Public Function GetSysSettings(ByVal psCategorty As String, ByVal psSucCategory As String) As CSysSettingList
            Dim sKey As String = UCase(String.Format("SBC_SYS_SETTINGS_{0}_{1}", psCategorty, psSucCategory))

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oSettings As New CSysSettingList
                Dim odtSettings As DataTable = (New CSysSettingManager).GetSysSettings(psCategorty, psSucCategory)

                If odtSettings IsNot Nothing Then
                    For Each odrSetting As DataRow In odtSettings.Rows
                        If SafeString(odrSetting("Key")) <> "" Then
                            oSettings.Add(New CSysSetting(odrSetting))
                        End If
                    Next
                End If

                HttpContext.Current.Cache.Add(sKey, oSettings, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If

            Return CType(HttpContext.Current.Cache(sKey), CSysSettingList)
        End Function

        Public Sub ClearSysSettings(ByVal psCategorty As String, ByVal psSucCategory As String)
            HttpContext.Current.Cache.Remove(UCase(String.Format("SBC_SYS_SETTINGS_{0}_{1}", psCategorty, psSucCategory)))
        End Sub

#End Region

#Region "Odds Rules"
        'Public Function GetAllOddsRules(ByVal psSiteType As String) As List(Of COddsRule)
        '    Dim sKey As String = psSiteType & "_ALL_ODDS_RULES"

        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim olstCOddsRule As List(Of COddsRule) = New List(Of COddsRule)
        '        Dim odtCOddsRule As DataTable = (New COddsRuleManager).GetALLOddsRules()
        '        For Each odrCOddsRule As DataRow In odtCOddsRule.Rows
        '            Dim oCOddsRule As COddsRule = New COddsRule(odrCOddsRule)
        '            olstCOddsRule.Add(oCOddsRule)
        '        Next

        '        HttpContext.Current.Cache.Add(sKey, olstCOddsRule, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If

        '    Return CType(HttpContext.Current.Cache(sKey), List(Of COddsRule))
        'End Function

        Public Sub ClearAllOddsRules(ByVal psSiteType As String)
            HttpContext.Current.Cache.Remove(psSiteType & "_ALL_ODDS_RULES")
        End Sub

        'Public Function GetOddsRulesByID(ByVal psID As String) As COddsRule
        '    Dim sKey As String = "SBC_ALL_ODDS_RULES_" & UCase(psID)

        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim oCOddsRule As New COddsRule
        '        Dim odtCOddsRule As DataTable = (New COddsRuleManager()).GetOddsRulesByID(psID)

        '        If odtCOddsRule IsNot Nothing Then
        '            HttpContext.Current.Cache.Add(sKey, New COddsRule(odtCOddsRule.Rows(0)), Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '        End If
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), COddsRule)
        'End Function

        Public Sub ClearOddsRulesByID(ByVal psID As String)
            HttpContext.Current.Cache.Remove("SBC_ALL_ODDS_RULES_" & UCase(psID))
        End Sub

        Public Function GetParplaySetUp(ByVal psAgentID As String) As Integer
            Dim sKey As String = "PARPLAY_SET_UP" & UCase(psAgentID)
            ' Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            Dim oSysSettingManager = New CSysSettingManager()
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                If oSysSettingManager.CheckExistSysSetting(psAgentID.ToString() & "MaxSelection", "", "MaxSelection") Then
                    HttpContext.Current.Cache.Add(sKey, oSysSettingManager.GetSystemSetting(psAgentID & "MaxSelection", "MaxSelection")("Value"), Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                Else
                    HttpContext.Current.Cache.Add(sKey, 0, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                End If
            End If
            Return CType(HttpContext.Current.Cache(sKey), Integer)
        End Function

        Public Sub ClearParplaySetUp(ByVal psAgentID As String)
            HttpContext.Current.Cache.Remove("PARPLAY_SET_UP" & UCase(psAgentID))
        End Sub

        'Public Function GetGameTypeOnOff(ByVal psAgentID As String, ByVal psGameType As String, ByVal bCurrentGame As Boolean) As List(Of CGameTypeOnOff)
        '    Dim sKey As String = "GAMETYPE_ON_OFF_" & SafeString(bCurrentGame) & UCase(psAgentID) & UCase(psGameType)
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    Dim lstGameTypeOnOff As New List(Of CGameTypeOnOff)
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim odt As DataTable = oGameTypeOnOffManager.GetGameTypeOnOffByGameType(psAgentID, psGameType, bCurrentGame)
        '        For Each odr As DataRow In odt.Rows
        '            lstGameTypeOnOff.Add(New CGameTypeOnOff(odr))
        '        Next
        '        HttpContext.Current.Cache.Add(sKey, lstGameTypeOnOff, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), List(Of CGameTypeOnOff))
        'End Function

        Public Sub ClearGameTypeOnOff(ByVal psAgentID As String, ByVal psGameType As String, ByVal bCurrentGame As Boolean)
            Dim sKey As String = "GAMETYPE_ON_OFF_" & SafeString(bCurrentGame) & UCase(psAgentID) & UCase(psGameType)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        'Public Function GetGameType2HOnOff(ByVal psAgentID As String) As Integer
        '    Dim sKey As String = "GAMETYPE_ON_OFF_2H" & UCase(psAgentID)
        '    'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        HttpContext.Current.Cache.Add(sKey, oGameTypeOnOffManager.GetValue2HGameTypeOnOff(psAgentID), Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), Integer)
        'End Function

        Public Sub ClearGameOddRule(ByVal psAgentID As String)
            Dim sKey As String = "GAMETYPE_ODD_RULE" & UCase(psAgentID)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub


        Public Function GetGameOddRule(ByVal psAgentID As String) As List(Of COddsRule)
            Dim plstOddsRules As New List(Of COddsRule)
            Dim sKey As String = "GAMETYPE_ODD_RULE" & UCase(psAgentID)
            Dim oOddRuleManager As New COddRuleManager()
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim odtGameRule As DataTable = oOddRuleManager.GetOddRulesByAgentID(psAgentID)
                For Each odr As DataRow In odtGameRule.Rows
                    plstOddsRules.Add(New COddsRule(odr, True))
                Next
                HttpContext.Current.Cache.Add(sKey, plstOddsRules, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(HttpContext.Current.Cache(sKey), List(Of COddsRule))
        End Function

        Public Sub ClearGameType2HOnOff(ByVal psAgentID As String)
            Dim sKey As String = "GAMETYPE_ON_OFF_2H" & UCase(psAgentID)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        'Public Function GetFixSpreadMoney(ByVal psAgentID As String) As CFixSpreadMoney
        '    Dim sKey As String = "GAME_FIX_SPREAD_MONEY" & UCase(psAgentID)
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim odtFixSpreadMoney As DataTable = oGameTypeOnOffManager.GetSpreadMoneyValues(psAgentID)
        '        If odtFixSpreadMoney IsNot Nothing AndAlso odtFixSpreadMoney.Rows.Count > 0 Then
        '            Dim oFixSpreadMoney As New CFixSpreadMoney(odtFixSpreadMoney.Rows(0))
        '            HttpContext.Current.Cache.Add(sKey, oFixSpreadMoney, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '        Else
        '            Return Nothing
        '        End If
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), CFixSpreadMoney)
        'End Function

        Public Sub ClearFixSpreadMoney(ByVal psAgentID As String)
            Dim sKey As String = "GAME_FIX_SPREAD_MONEY" & UCase(psAgentID)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        'Public Function GetGameHalfTimeOFF(ByVal psAgentID As String) As CGameHalfTimeOffList
        '    Dim sKey As String = "GAME_HALF_TIME_OFF" & UCase(psAgentID)
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim lstGameHalfTimeOff As CGameHalfTimeOffList = oGameTypeOnOffManager.GetGameHTimeOff(psAgentID)
        '        HttpContext.Current.Cache.Add(sKey, lstGameHalfTimeOff, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), CGameHalfTimeOffList)
        'End Function

        Public Sub ClearGameHalfTimeOFF(ByVal psAgentID As String)
            Dim sKey As String = "GAME_HALF_TIME_OFF" & UCase(psAgentID)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        'Public Function GetGameHalfTimeDisplay() As CGameHalfTimeDisplayList
        '    Dim sKey As String = "GAME_HALF_TIME_DISPLAY"

        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim lstGameHalfTimeDisplay As  =GetSysSettings("2HOFF")
        '        HttpContext.Current.Cache.Add(sKey, lstGameHalfTimeDisplay, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), CGameHalfTimeDisplayList)
        'End Function

        'Public Sub ClearGameHalfTimeDisplay(ByVal psAgentID As String)
        '    Dim sKey As String = "GAME_HALF_TIME_DISPLAY" & UCase(psAgentID)
        '    HttpContext.Current.Cache.Remove(sKey)
        'End Sub

        'Public Function GetGameTeamDisplay(ByVal psAgentID As String, ByVal psSportType As String) As CGameTeamDisplayList
        '    Dim sKey As String = "GAME_TEAM_DISPLAY" & UCase(psAgentID) & UCase(psSportType)
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim lstGameTeamDisplay As CGameTeamDisplayList = oGameTypeOnOffManager.GetGameTeamDisplay(psAgentID, SafeString(psSportType))
        '        HttpContext.Current.Cache.Add(sKey, lstGameTeamDisplay, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), CGameTeamDisplayList)
        'End Function

        Public Sub ClearGameTeamDisplay(ByVal psAgentID As String, ByVal psSportType As String)
            Dim sKey As String = "GAME_TEAM_DISPLAY" & UCase(psAgentID) & UCase(psSportType)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub

        'Public Function GetGameQuarterOff(ByVal psAgentID As String, ByVal psSportType As String, ByVal psContext As String) As Boolean
        '    Dim sKey As String = "GAME_QUARTER_OFF" & UCase(psAgentID) & UCase(psSportType) & UCase(psContext)
        '    Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
        '    If HttpContext.Current.Cache(sKey) Is Nothing Then
        '        Dim oOffLine As Boolean = oGameTypeOnOffManager.GetGameQuarterOffByAgent(psAgentID, psSportType, psContext)
        '        HttpContext.Current.Cache.Add(sKey, oOffLine, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
        '    End If
        '    Return CType(HttpContext.Current.Cache(sKey), Boolean)
        'End Function

        Public Sub ClearGameQuarterOff(ByVal psAgentID As String, ByVal psSportType As String, ByVal psContext As String)
            Dim sKey As String = "GAME_QUARTER_OFF" & UCase(psAgentID) & UCase(psSportType) & UCase(psContext)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub


        Public Function GetJuiceControl(ByVal psAgentID As String, ByVal psSportType As String, ByVal psContext As String, ByVal psGameType As String) As Integer
            Dim sKey As String = "JUICE_CONTROL" & UCase(psAgentID) & UCase(psSportType) & UCase(psContext) & UCase(psGameType)
            'Dim oGameTypeOnOffManager As New CGameTypeOnOffManager()
            If HttpContext.Current.Cache(sKey) Is Nothing Then
                'Dim nJuice As Integer = oGameTypeOnOffManager.GetJuiceControlByID(psAgentID, psSportType, psContext)
                Dim oSettings As New CSysSettingList
                Dim nJuice As Integer = GetSysSettings(psAgentID & "_Juice", psSportType).GetIntegerValue(psContext, psGameType) '(New CSysSettingManager).GetSysSettings("").get

                HttpContext.Current.Cache.Add(sKey, nJuice, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(HttpContext.Current.Cache(sKey), Integer)
        End Function

        Public Sub ClearJuiceControl(ByVal psAgentID As String, ByVal psSportType As String, ByVal psContext As String, ByVal psGameType As String)
            Dim sKey As String = "JUICE_CONTROL" & UCase(psAgentID) & UCase(psSportType) & UCase(psContext) & UCase(psGameType)
            HttpContext.Current.Cache.Remove(sKey)
        End Sub


#End Region

#Region "SiteType"
        Public Function GetSiteType(ByVal psURL As String) As CEnums.ESiteType
            Dim sKey As String = "SITETYPE_" & UCase(psURL)

            If HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim oWhiteLabelSettingManager As New CWhiteLabelSettingManager()
                Dim eSiteType As CEnums.ESiteType = ConvertStringToSiteType(oWhiteLabelSettingManager.GetSiteTypeByURL(psURL))

                HttpContext.Current.Cache.Add(sKey, eSiteType, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(HttpContext.Current.Cache(sKey), CEnums.ESiteType)
        End Function

        Public Sub ClearSiteType(ByVal psURL As String)
            HttpContext.Current.Cache.Remove("SITETYPE_" & UCase(psURL))
        End Sub
#End Region

#Region "IPALert"

        Public Function checkIPAlert(ByVal poUserType As EUserType, ByVal psUserID As String) As Boolean
            'Dim sKey As String = "HAS_IPALERT_" & psUserID

            'If HttpContext.Current.Cache(sKey) Is Nothing Then
            '    Dim sDateFrom As String = Date.Now().AddDays(-14).ToShortDateString() & " 00:00:00"
            '    Dim sDateTo As String = Date.Now().ToShortDateString() & " 11:59:59"

            '    Dim oIPTracesManager As New CIPTracesManager()
            '    Dim oDataSearch As DataTable = Nothing
            '    Dim oDataIPTrace As DataTable = Nothing

            '    If poUserType = EUserType.SuperAdmin Then
            '        oDataSearch = oIPTracesManager.GetIPAlertSearchResult(psUserID, True, sDateFrom)
            '        oDataIPTrace = oIPTracesManager.GetIPTracesByDateRange(psUserID, True, sDateFrom)
            '    Else
            '        oDataSearch = oIPTracesManager.GetIPAlertSearchResult(psUserID, False, sDateFrom)
            '        oDataIPTrace = oIPTracesManager.GetIPTracesByDateRange(psUserID, False, sDateFrom)
            '    End If

            '    Dim sIP As String = ""
            '    Dim sLoginName As String = ""
            '    Dim bFind As Boolean = False

            '    For Each oRow As DataRow In oDataSearch.Rows
            '        sIP = SafeString(oRow("IP"))

            '        '' 1. First check IP
            '        bFind = SafeInteger(oDataIPTrace.Compute("Count(LoginName)", "IP=" & SQLString(sIP))) > 1

            '        If bFind Then
            '            Exit For
            '        Else
            '            '' 2. Second check LoginName
            '            bFind = SafeInteger(oDataIPTrace.Compute("Count(IP)", "LoginName=" & SQLString(sLoginName))) > 1

            '            If bFind Then Exit For
            '        End If

            '    Next
            '    ''thuong can sua
            '    HttpContext.Current.Cache.Add(sKey, bFind, Nothing, Date.Now.AddDays(100), Nothing, Caching.CacheItemPriority.Default, Nothing)
            'End If

            'Return CType(HttpContext.Current.Cache(sKey), Boolean)
            Return False
        End Function

#End Region

        Public Class CCacheManagerException
            Inherits System.Exception

            Public Sub New(ByVal psMessage As String)
                MyBase.New(psMessage)
            End Sub

        End Class

    End Class

End Namespace
