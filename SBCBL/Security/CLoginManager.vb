Imports System.Web
Imports System.Web.Caching
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports SBCBL.Utils
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports SBCBL.CEnums
Imports WebsiteLibrary.DBUtils
Imports WebsiteLibrary.CSBCStd

Namespace Security

    Public Class CLoginManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function Login(ByVal psLogin As String, ByVal psPassword As String, ByVal psIP As String, ByVal psHost As String, ByVal pbIsAdminLogin As Boolean, ByVal pbPlayer As Boolean, Optional ByVal pbImpersonate As Boolean = False) As String
            Dim sUrl As String = "/default.aspx"

            '' In this login page , We need to store /images/ and USER_TYPE, so that we can know where to select information


            'Dim oUserInfo As DataRow = CLoginHelper.GetLoginInfo(pbIsAdminLogin, psLogin, psPassword, std.GetSiteType, pbImpersonate)
            Dim oUserInfo As DataRow = CLoginHelper.GetLoginInfo(pbIsAdminLogin, psLogin, psPassword, std.GetSiteType, pbImpersonate)
            ''Check normal user
            Dim sThrowMsg As String = CLoginHelper.CheckLoginInfo(oUserInfo, pbIsAdminLogin, psIP, psHost)

            If sThrowMsg = "" Then
                Dim oCache As New CCacheManager()

                Dim oSiteType As CEnums.ESiteType = CType([Enum].Parse(GetType(CEnums.ESiteType), SafeString(oUserInfo("SiteType"))), CEnums.ESiteType)
                If Not pbPlayer AndAlso CType(System.Web.HttpContext.Current.Session("USER_TYPE"), EUserType) = EUserType.Player Then
                    Throw New CSecurityException("Cannot login by player")
                End If

                Select Case CType(System.Web.HttpContext.Current.Session("USER_TYPE"), EUserType)
                    Case EUserType.Agent
                        '' sUrl = "/" & oSiteType.ToString() & "/Agents/default.aspx"
                        If GetSiteType().Equals("SBS") Then
                            If pbPlayer Then
                                sUrl = "/" & oSiteType.ToString() & "/Agents/Management/PlayersReports.aspx?tab=ALL_PLAYERS"
                            Else
                                sUrl = "/" & oSiteType.ToString() & "/Agents/Management/PlayersReports.aspx?tab=ALL_PLAYERS"
                            End If
                        End If
                        System.Web.HttpContext.Current.Session("USER_ID") = SafeString(oUserInfo("AgentID"))
                        System.Web.HttpContext.Current.Session("AGENT_SELECT_ID") = SafeString(oUserInfo("AgentID"))
                        oCache.ClearAgentInfo(SafeString(oUserInfo("AgentID")), psLogin)

                    Case EUserType.SuperAdmin
                        If SafeString(oUserInfo("IsPartner")) = "Y" Then
                            sUrl = "/" & oSiteType.ToString() & "/SuperAdmins/SuperAgentBalance.aspx"
                        Else
                            sUrl = "/" & oSiteType.ToString() & "/SuperAdmins/SuperAgentBalance.aspx"
                        End If
                        System.Web.HttpContext.Current.Session("USER_ID") = SafeString(oUserInfo("SuperAdminID"))
                        oCache.ClearSuperAdminInfo(SafeString(oUserInfo("SuperAdminID")), psLogin)
                    Case EUserType.Player
                        sUrl = "/" & oSiteType.ToString() & "/Players/Default.aspx?bettype=BetIfAll"
                        ' sUrl = "/" & oSiteType.ToString() & "/Players/default.aspx"
                        System.Web.HttpContext.Current.Session("USER_ID") = SafeString(oUserInfo("PlayerID"))
                        oCache.ClearPlayerInfo(SafeString(oUserInfo("PlayerID")), psLogin)

                    Case EUserType.CallCenterAgent
                        sUrl = "/" & oSiteType.ToString() & "/CallCenter/default.aspx"
                        System.Web.HttpContext.Current.Session("USER_ID") = SafeString(oUserInfo("CallCenterAgentID"))
                        oCache.ClearCallCenterAgentInfo(SafeString(oUserInfo("CallCenterAgentID")), psLogin)
                End Select

                If Not pbIsAdminLogin Then
                    '' Save login log
                    CLoginHelper.ResetNumberAttempts(psLogin, std.GetSiteType)
                    CLoginHelper.SaveLoginLog(SafeString(System.Web.HttpContext.Current.Session("USER_ID")), psLogin, True, _
                                              psIP, HttpContext.Current.Request.Url.Host)

                    '' Save IPTraces for user login
                    Managers.CIPTracesManager.InsertIPTrace(SafeString(System.Web.HttpContext.Current.Session("USER_ID")), psLogin, psIP, std.GetSiteType)
                End If

            Else
                CLoginHelper.SaveLoginLog(SafeString(System.Web.HttpContext.Current.Session("USER_ID")), psLogin, False, _
                                              psIP, HttpContext.Current.Request.Url.Host)
                CLoginHelper.IncreaseNumberAttempts(psLogin, std.GetSiteType())
                Throw New CSecurityException(sThrowMsg)
            End If

            Return sUrl
        End Function

        Public Sub LogOut(Optional ByVal pbRedirectToLogin As Boolean = True)
            System.Web.Security.FormsAuthentication.SignOut()
            HttpContext.Current.Session.Abandon()
            If pbRedirectToLogin Then
                HttpContext.Current.Response.Redirect("/default.aspx")
            End If
        End Sub

        Public Shared Function GetRoles(ByVal psLogin As String) As String()
            Dim oCache As Cache = HttpContext.Current.Cache
            Dim sKey As String = String.Format("ROLE_{0}_{1}", std.GetSiteType(), psLogin)

            If oCache(sKey) Is Nothing Then
                '' since now, we will get role from cache
                CLoginHelper.LoadRolesToCache(psLogin, oCache, sKey, std.GetSiteType())
            End If

            Return CType(oCache(sKey), String())
        End Function

    End Class

End Namespace
