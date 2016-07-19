Imports System.Collections.Specialized
Imports System.Web
Imports System.Xml

Imports log4net

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
        <NonSerialized()> Private _MemcachedManager As CMemcachedManager

        Private ReadOnly Property MemcachedManager() As CMemcachedManager
            Get
                If _MemcachedManager Is Nothing Then
                    _MemcachedManager = New CMemcachedManager()
                End If

                Return _MemcachedManager
            End Get
        End Property

#Region "USER METHODS"
        Public Function GetAgentInfo(ByVal psAgentID As String) As CAgent
            Dim sKey As String = "AGENT_INFO_" & psAgentID
            Dim oAgentInfo As CAgent = CType(MemcachedManager.Get(sKey), CAgent)
            If oAgentInfo Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
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

                    MemcachedManager.Add(sKey, oAgentInfo, CACHE_TIME)
                    MemcachedManager.Add("USER_NAME_" & psAgentID, oAgentInfo.Name, CACHE_TIME)
                Catch ex As Exception
                    log.Error("Fail to GetUserInfo By ID: " & psAgentID, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If
            Return oAgentInfo
        End Function

        Public Sub ClearAgentInfo(ByVal psAgentID As String, ByVal psLoginName As String)
            Dim sUserName As String = psLoginName

            MemcachedManager.Remove("AGENT_INFO_" & psAgentID)
            MemcachedManager.Remove("ROLE_" & psLoginName)
            MemcachedManager.Remove("USER_NAME_" & psAgentID)

        End Sub

        Public Function GetCallCenterAgentInfo(ByVal psCallCenterAgentID As String) As CCallCenterAgent
            Dim sKey As String = "CALL_CENTER_AGENT_INFO_" & psCallCenterAgentID
            Dim oCCAgentInfo As CCallCenterAgent = CType(MemcachedManager.Get(sKey), CCallCenterAgent)
            If oCCAgentInfo Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
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

                    MemcachedManager.Add(sKey, oCCAgentInfo, CACHE_TIME)
                    MemcachedManager.Add("USER_NAME_" & psCallCenterAgentID, oCCAgentInfo.Name, CACHE_TIME)
                Catch ex As Exception
                    log.Error("Fail to GetUserInfo By ID: " & psCallCenterAgentID, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If
            Return oCCAgentInfo
        End Function

        Public Sub ClearCallCenterAgentInfo(ByVal psCallCenterAgentID As String, ByVal psLoginName As String)
            Dim sUserName As String = psLoginName

            MemcachedManager.Remove("CALL_CENTERAGENT_INFO_" & psCallCenterAgentID)
            MemcachedManager.Remove("ROLE_" & psLoginName)
            MemcachedManager.Remove("USER_NAME_" & psCallCenterAgentID)

        End Sub

        Public Function GetPlayerInfo(ByVal psPlayerID As String, Optional ByVal pbCareLocked As Boolean = True) As CPlayer
            Dim sKey As String = "PLAYER_INFO_" & psPlayerID
            Dim oPlayerInfo As CPlayer = CType(MemcachedManager.Get(sKey), CPlayer)
            If oPlayerInfo Is Nothing Then
                If psPlayerID = "Test" Then
                    oPlayerInfo = New CPlayer()
                Else
                    oPlayerInfo = (New CPlayerManager).GetPlayer(psPlayerID, pbCareLocked)

                    If oPlayerInfo Is Nothing Then
                        Throw New CCacheManagerException("Unable to find player user: " & psPlayerID)
                    End If
                End If

                MemcachedManager.Add(sKey, oPlayerInfo, CACHE_TIME)
                MemcachedManager.Add("USER_NAME_" & psPlayerID, oPlayerInfo.Name, CACHE_TIME)
            End If

            Return oPlayerInfo
        End Function

        Public Sub ClearPlayerInfo(ByVal psPlayerID As String, Optional ByVal psLoginName As String = "")
            Dim sUserName As String = psLoginName
            Dim sKey As String = ""
            If sUserName = "" Then
                sUserName = Me.GetPlayerInfo(psPlayerID, False).Login
            End If
            sKey = "PLAYER_INFO_" & psPlayerID
            MemcachedManager.Remove(sKey)
            sKey = "ROLE_" & psLoginName
            MemcachedManager.Remove(sKey)
            sKey = "USER_NAME_" & psPlayerID
            MemcachedManager.Remove(sKey)

        End Sub

        Public Function GetSuperAdminInfo(ByVal psSuperAdminID As String) As CSuperAdmin
            Dim sKey As String = "SUPERADMIN_INFO_" & psSuperAdminID
            Dim oSuperAdminInfo As CSuperAdmin = CType(MemcachedManager.Get(sKey), CSuperAdmin)
            If oSuperAdminInfo Is Nothing Then
                Dim oDB As CSQLDBUtils = Nothing
                Try
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

                    MemcachedManager.Add(sKey, oSuperAdminInfo, CACHE_TIME)
                Catch ex As Exception
                    log.Error("Fail to load SuperAdminInfo By ID: " & psSuperAdminID & " " & ex.Message, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If

            Return oSuperAdminInfo
        End Function

        Public Sub ClearSuperAdminInfo(ByVal psSuperAdminID As String, Optional ByVal psLoginName As String = "")
            Dim psSuperAdminName As String = psLoginName

            If psSuperAdminName = "" Then
                psSuperAdminName = Me.GetSuperAdminInfo(psSuperAdminID).Login
            End If

            MemcachedManager.Remove("SUPERADMIN_INFO_" & psSuperAdminID)
            MemcachedManager.Remove("ROLE_" & psSuperAdminName)
        End Sub

#End Region

#Region "Teaser Rules"

        Public Function GetTeaserRuleInfo(ByVal psTeaserRuleID As String) As CTeaserRule
            Dim sKey As String = "TEASER_RULE_INFO_" & psTeaserRuleID
            Dim oTeaserRule As CTeaserRule = CType(MemcachedManager.Get(sKey), CTeaserRule)
            If oTeaserRule Is Nothing Then
                Dim oTeaserRuleManager As New CTeaserRuleManager()
                Dim otbl As DataTable = oTeaserRuleManager.GetByID(psTeaserRuleID)

                If otbl IsNot Nothing AndAlso otbl.Rows.Count > 0 Then
                    oTeaserRule = New CTeaserRule(otbl.Rows(0))

                    MemcachedManager.Add(sKey, oTeaserRule, CACHE_TIME)
                Else
                    Throw New CCacheManagerException("Unable to find TeaserRule: " & psTeaserRuleID)
                End If
            End If

            Return oTeaserRule
        End Function

        Public Function GetTeaserRules() As CTeaserRuleList
            Dim sKey As String = String.Format("{0}_TEASER_RULE_LIST", SBCBL.std.GetSiteType)
            Dim olstTeaserRule As CTeaserRuleList = CType(MemcachedManager.Get(sKey), CTeaserRuleList)
            If olstTeaserRule Is Nothing Then
                Dim oTeaserRuleManager As New CTeaserRuleManager()
                Dim otbl As DataTable = oTeaserRuleManager.GetTeaserRules(Nothing, SBCBL.std.GetSiteType)

                If otbl IsNot Nothing Then
                    olstTeaserRule = New CTeaserRuleList(otbl)

                    MemcachedManager.Add(sKey, olstTeaserRule, CACHE_TIME)
                Else
                    Throw New CCacheManagerException("Unable to get TeaserRules")
                End If
            End If

            Return olstTeaserRule
        End Function

        Public Sub ClearTeaserRuleInfo(ByVal psTeaserRuleID As String)
            MemcachedManager.Remove("TEASER_RULE_INFO_" & psTeaserRuleID)
            ClearTeaserRules()
        End Sub

        Public Sub ClearTeaserRules()
            MemcachedManager.Remove(String.Format("{0}_TEASER_RULE_LIST", SBCBL.std.GetSiteType))
        End Sub

#End Region

#Region "Sys Settings"
        Public Function GetAllSysSettings(ByVal psCategorty As String) As CSysSettingList
            Dim sKey As String = "SBC_ALL_SYS_SETTINGS_" & UCase(psCategorty)
            Dim oSettings As CSysSettingList = CType(MemcachedManager.Get(sKey), CSysSettingList)
            If oSettings Is Nothing Then
                oSettings = (New CSysSettingManager).GetAllSysSettings(psCategorty)

                MemcachedManager.Add(sKey, oSettings, CACHE_TIME)
            End If

            Return oSettings
        End Function

        Public Sub ClearAllSysSettings(ByVal psCategorty As String)
            MemcachedManager.Remove("SBC_ALL_SYS_SETTINGS_" & UCase(psCategorty))
        End Sub

        Public Function GetSysSettings(ByVal psCategorty As String) As CSysSettingList
            Dim sKey As String = "SBC_SYS_SETTINGS_" & UCase(psCategorty)
            Dim oSettings As CSysSettingList = CType(MemcachedManager.Get(sKey), CSysSettingList)
            If MemcachedManager.Get(sKey) Is Nothing Then
                oSettings = New CSysSettingList()
                Dim odtSettings As DataTable = (New CSysSettingManager).GetSysSettings(psCategorty)

                If odtSettings IsNot Nothing Then
                    For Each odrSetting As DataRow In odtSettings.Rows
                        If SafeString(odrSetting("Key")) <> "" Then
                            oSettings.Add(New CSysSetting(odrSetting))
                        End If
                    Next
                End If

                MemcachedManager.Add(sKey, oSettings, CACHE_TIME)
            End If

            Return oSettings
        End Function

        Public Sub ClearSysSettings(ByVal psCategorty As String)
            MemcachedManager.Remove("SBC_SYS_SETTINGS_" & UCase(psCategorty))
        End Sub

        Public Function GetSysSettings(ByVal psCategorty As String, ByVal psSucCategory As String) As CSysSettingList
            Dim sKey As String = UCase(String.Format("SBC_SYS_SETTINGS_{0}_{1}", psCategorty, psSucCategory))
            Dim oSettings As CSysSettingList = CType(MemcachedManager.Get(sKey), CSysSettingList)
            If oSettings Is Nothing Then
                oSettings = New CSysSettingList
                Dim odtSettings As DataTable = (New CSysSettingManager).GetSysSettings(psCategorty, psSucCategory)

                If odtSettings IsNot Nothing Then
                    For Each odrSetting As DataRow In odtSettings.Rows
                        If SafeString(odrSetting("Key")) <> "" Then
                            oSettings.Add(New CSysSetting(odrSetting))
                        End If
                    Next
                End If

                MemcachedManager.Add(sKey, oSettings, CACHE_TIME)
            End If

            Return oSettings
        End Function

        Public Sub ClearSysSettings(ByVal psCategorty As String, ByVal psSucCategory As String)
            MemcachedManager.Remove(UCase(String.Format("SBC_SYS_SETTINGS_{0}_{1}", psCategorty, psSucCategory)))
        End Sub

#End Region

#Region "Odds Rules"
        Public Function GetAllOddsRules(ByVal psSiteType As String) As List(Of COddsRule)
            Dim sKey As String = psSiteType & "_ALL_ODDS_RULES"
            Dim olstCOddsRule As List(Of COddsRule) = CType(MemcachedManager.Get(sKey), List(Of COddsRule))
            If olstCOddsRule Is Nothing Then
                olstCOddsRule = New List(Of COddsRule)
                Dim odtCOddsRule As DataTable = (New COddsRuleManager).GetALLOddsRules()
                For Each odrCOddsRule As DataRow In odtCOddsRule.Rows
                    Dim oCOddsRule As COddsRule = New COddsRule(odrCOddsRule)
                    olstCOddsRule.Add(oCOddsRule)
                Next

                MemcachedManager.Add(sKey, olstCOddsRule, CACHE_TIME)
            End If

            Return olstCOddsRule
        End Function

        Public Sub ClearAllOddsRules(ByVal psSiteType As String)
            MemcachedManager.Remove(psSiteType & "_ALL_ODDS_RULES")
        End Sub

        Public Function GetOddsRulesByID(ByVal psID As String) As COddsRule
            Dim sKey As String = "SBC_ALL_ODDS_RULES_" & UCase(psID)
            Dim oCOddsRule As COddsRule = CType(MemcachedManager.Get(sKey), COddsRule)
            If oCOddsRule Is Nothing Then
                Dim odtCOddsRule As DataTable = (New COddsRuleManager()).GetOddsRulesByID(psID)

                If odtCOddsRule IsNot Nothing Then
                    oCOddsRule = New COddsRule(odtCOddsRule.Rows(0))
                    MemcachedManager.Add(sKey, oCOddsRule, CACHE_TIME)
                End If
            End If
            Return oCOddsRule
        End Function

        Public Sub ClearOddsRulesByID(ByVal psID As String)
            MemcachedManager.Remove("SBC_ALL_ODDS_RULES_" & UCase(psID))
        End Sub

#End Region

#Region "SiteType"
        Public Function GetSiteType(ByVal psURL As String) As CEnums.ESiteType
            Dim sKey As String = "SITETYPE_" & UCase(psURL)

            If MemcachedManager.Get(sKey) Is Nothing Then
                Dim oWhiteLabelSettingManager As New SBCBL.CWhiteLabelSettingManager()
                Dim eSiteType As CEnums.ESiteType = ConvertStringToSiteType(oWhiteLabelSettingManager.GetSiteTypeByURL(psURL))

                MemcachedManager.Add(sKey, eSiteType, CACHE_TIME)
            End If
            Return CType(MemcachedManager.Get(sKey), CEnums.ESiteType)
        End Function

        Public Sub ClearSiteType(ByVal psURL As String)
            MemcachedManager.Remove("SITETYPE_" & UCase(psURL))
        End Sub
#End Region

        Public Class CCacheManagerException
            Inherits System.Exception

            Public Sub New(ByVal psMessage As String)
                MyBase.New(psMessage)
            End Sub

        End Class

    End Class

End Namespace
