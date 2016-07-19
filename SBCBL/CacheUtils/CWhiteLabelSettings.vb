Imports System.Xml
Imports log4net
Imports SBCBL.std
Imports SBCBL.Utils
Imports SBCBL.CacheUtils
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils
Imports System.Web

Namespace CacheUtils
    Public Class CWhiteLabelSettings

        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Public WhiteLabelSettingID As String = ""
        Public SiteURL As String = ""
        Public CopyrightName As String = ""
        Public LogoFileName As String = ""
        Public WelComeImage As String = ""
        Public SiteType As String = ""
        Public Favicon As String
        Public ColorScheme As String = ""
        Public BackgroundImage As String = ""
        Public BackgroundLoginImage As String = ""
        Public LoginTemplate As String = ""
        Public LoginLogo As String = ""
        Public LeftBackgroundLoginImage As String
        Public RightBackgroundLoginImage As String
        Public BottomBackgroundLoginImage As String
        Public SuperAgentPhone As String
        Public BackupURL As String
        Private Const CACHE_TIME As Integer = 20 'minutes until each cache entry expires

        Public Function InsertNew() As Boolean
            Dim oInsert As New WebsiteLibrary.DBUtils.CSQLInsertStringBuilder("WhiteLabelSettings")
            oInsert.AppendString("WhiteLabelSettingID", SQLString(WhiteLabelSettingID))
            oInsert.AppendString("SiteURL", SQLString(SiteURL))
            oInsert.AppendString("CopyrightName", SQLString(CopyrightName))
            oInsert.AppendString("LogoFileName", SQLString(LogoFileName))
            oInsert.AppendString("Favicon", SQLString(Favicon))
            oInsert.AppendString("SiteType", SQLString(SiteType))
            oInsert.AppendString("ColorScheme", SQLString(ColorScheme))
            oInsert.AppendString("WelComeImage", SQLString(WelComeImage))
            oInsert.AppendString("BackgroundImage", SQLString(BackgroundImage))
            oInsert.AppendString("BackgroundLoginImage", SQLString(BackgroundLoginImage))
            oInsert.AppendString("LoginTemplate", SQLString(LoginTemplate))
            oInsert.AppendString("LogoLoginImage", SQLString(LoginLogo))
            oInsert.AppendString("LeftBackgroundLoginImage", SQLString(LeftBackgroundLoginImage))
            oInsert.AppendString("RightBackgroundLoginImage", SQLString(RightBackgroundLoginImage))
            oInsert.AppendString("BottomBackgroundLoginImage", SQLString(BottomBackgroundLoginImage))
            oInsert.AppendString("SuperAgentPhone", SQLString(SuperAgentPhone))
            oInsert.AppendString("BackupURL", SQLString(BackupURL))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oInsert.SQL)
                Return True
            Catch ex As Exception
                _log.Error("Cannot Insert New Whitelabel setting. SQL: " & oInsert.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function

        Public Shared Function Delete(ByVal psWhiteLabelSettingID As String) As Boolean
            Dim sSQL As String = "delete from WhiteLabelSettings where WhiteLabelSettingID = " & SQLString(psWhiteLabelSettingID)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(sSQL)
                Return True
            Catch ex As Exception
                Throw ex
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function

        Public Function Update(Optional ByVal bremove As Boolean = False) As Boolean
            Dim oInsert As New WebsiteLibrary.DBUtils.CSQLUpdateStringBuilder("WhiteLabelSettings", "where WhiteLabelSettingID = " & SQLString(WhiteLabelSettingID))
            oInsert.AppendString("SiteURL", SQLString(SiteURL))
            oInsert.AppendString("CopyrightName", SQLString(CopyrightName))
            oInsert.AppendString("ColorScheme", SQLString(ColorScheme))
            oInsert.AppendString("LoginTemplate", SQLString(LoginTemplate))
            If LogoFileName <> "" Then
                oInsert.AppendString("LogoFileName", SQLString(LogoFileName))
            End If
            If WelComeImage <> "" Then
                oInsert.AppendString("WelComeImage", SQLString(WelComeImage))
            End If
            If Favicon <> "" Then
                oInsert.AppendString("Favicon", SQLString(Favicon))
            End If

            If BackgroundImage <> "" Then
                oInsert.AppendString("BackgroundImage", SQLString(BackgroundImage))
            End If

            If BackgroundLoginImage <> "" Then
                oInsert.AppendString("BackgroundLoginImage", SQLString(BackgroundLoginImage))
            End If
            If LoginLogo <> "" Then
                oInsert.AppendString("LogoLoginImage", SQLString(LoginLogo))
            End If

            oInsert.AppendString("SuperAgentPhone", SQLString(SuperAgentPhone))

            If LeftBackgroundLoginImage <> "" Then
                oInsert.AppendString("LeftBackgroundLoginImage", SQLString(LeftBackgroundLoginImage))
            End If
            If RightBackgroundLoginImage <> "" Then
                oInsert.AppendString("RightBackgroundLoginImage", SQLString(RightBackgroundLoginImage))
            End If
            If BottomBackgroundLoginImage <> "" Then
                oInsert.AppendString("BottomBackgroundLoginImage", SQLString(BottomBackgroundLoginImage))
            End If
            If BackupURL <> "" Then
                oInsert.AppendString("BackupURL", SQLString(BackupURL))
            End If

            If bremove Then
                oInsert.AppendString("BackgroundImage", SQLString(BackgroundImage))
                oInsert.AppendString("BackgroundLoginImage", SQLString(BackgroundLoginImage))
                oInsert.AppendString("LogoLoginImage", SQLString(LoginLogo))
                oInsert.AppendString("LeftBackgroundLoginImage", SQLString(LeftBackgroundLoginImage))
                oInsert.AppendString("RightBackgroundLoginImage", SQLString(RightBackgroundLoginImage))
                oInsert.AppendString("BottomBackgroundLoginImage", SQLString(BottomBackgroundLoginImage))
            End If
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oInsert.SQL)
                Return True
            Catch ex As Exception
                _log.Error("Cannot Update Whitelabel setting. SQL: " & oInsert.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function

        Public Function LoadByUrl(ByVal psURL As String) As CWhiteLabelSettings
            If psURL.StartsWith("www.") Then
                psURL = psURL.Substring("www.".Length)
            End If

            Dim sKey As String = "WHITE_LABEL_" & psURL
            If HttpContext.Current.Cache(sKey) IsNot Nothing Then
                Return CType(HttpContext.Current.Cache(sKey), CWhiteLabelSettings)
            End If

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "select top 1 * from WhiteLabelSettings where SiteURL = " & SQLString(psURL)
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                If oDT.Rows.Count > 0 Then
                    Dim oSetting As New CWhiteLabelSettings
                    oSetting.CopyrightName = SafeString(oDT.Rows(0)("CopyrightName"))
                    oSetting.Favicon = SafeString(oDT.Rows(0)("Favicon"))
                    oSetting.LogoFileName = SafeString(oDT.Rows(0)("LogoFileName"))
                    oSetting.SiteURL = SafeString(oDT.Rows(0)("SiteURL"))
                    oSetting.WhiteLabelSettingID = SafeString(oDT.Rows(0)("WhiteLabelSettingID"))
                    oSetting.ColorScheme = SafeString(oDT.Rows(0)("ColorScheme"))
                    oSetting.WelComeImage = SafeString(oDT.Rows(0)("WelComeImage"))
                    oSetting.BackgroundImage = SafeString(oDT.Rows(0)("BackgroundImage"))
                    oSetting.BackgroundLoginImage = SafeString(oDT.Rows(0)("BackgroundLoginImage"))
                    oSetting.LoginTemplate = SafeString(oDT.Rows(0)("LoginTemplate"))
                    oSetting.LoginLogo = SafeString(oDT.Rows(0)("LogoLoginImage"))
                    oSetting.LeftBackgroundLoginImage = SafeString(oDT.Rows(0)("LeftBackgroundLoginImage"))
                    oSetting.RightBackgroundLoginImage = SafeString(oDT.Rows(0)("RightBackgroundLoginImage"))
                    oSetting.BottomBackgroundLoginImage = SafeString(oDT.Rows(0)("BottomBackgroundLoginImage"))
                    oSetting.SuperAgentPhone = SafeString(oDT.Rows(0)("SuperAgentPhone"))
                    oSetting.BackupURL = SafeString(oDT.Rows(0)("BackupURL"))
                    HttpContext.Current.Cache.Add(sKey, oSetting, Nothing, Date.Now.AddMinutes(CACHE_TIME), Nothing, Caching.CacheItemPriority.Default, Nothing)
                    Return oSetting
                End If
            Catch ex As Exception
                _log.Error("Cannot Load by URL. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function LoadByID(ByVal psID As String) As CWhiteLabelSettings
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "select top 1 * from WhiteLabelSettings where WhiteLabelSettingID = " & SQLString(psID)
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                If oDT.Rows.Count > 0 Then
                    Dim oSetting As New CWhiteLabelSettings
                    oSetting.CopyrightName = SafeString(oDT.Rows(0)("CopyrightName"))
                    oSetting.Favicon = SafeString(oDT.Rows(0)("Favicon"))
                    oSetting.LogoFileName = SafeString(oDT.Rows(0)("LogoFileName"))
                    oSetting.SiteURL = SafeString(oDT.Rows(0)("SiteURL"))
                    oSetting.WhiteLabelSettingID = SafeString(oDT.Rows(0)("WhiteLabelSettingID"))
                    oSetting.ColorScheme = SafeString(oDT.Rows(0)("ColorScheme"))
                    oSetting.WelComeImage = SafeString(oDT.Rows(0)("WelComeImage"))
                    oSetting.BackgroundImage = SafeString(oDT.Rows(0)("BackgroundImage"))
                    oSetting.BackgroundLoginImage = SafeString(oDT.Rows(0)("BackgroundLoginImage"))
                    oSetting.LoginTemplate = SafeString(oDT.Rows(0)("LoginTemplate"))
                    oSetting.LoginLogo = SafeString(oDT.Rows(0)("LogoLoginImage"))
                    oSetting.LeftBackgroundLoginImage = SafeString(oDT.Rows(0)("LeftBackgroundLoginImage"))
                    oSetting.RightBackgroundLoginImage = SafeString(oDT.Rows(0)("RightBackgroundLoginImage"))
                    oSetting.BottomBackgroundLoginImage = SafeString(oDT.Rows(0)("BottomBackgroundLoginImage"))
                    oSetting.SuperAgentPhone = SafeString(oDT.Rows(0)("SuperAgentPhone"))
                    oSetting.BackupURL = SafeString(oDT.Rows(0)("BackupURL"))
                    Return oSetting
                End If
            Catch ex As Exception
                _log.Error("Cannot LoadByID . SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadAll(ByVal psSiteType As String) As DataTable
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SiteType = " & SQLString(LCase(psSiteType)))
            Dim sSQL As String = "select  * from WhiteLabelSettings" & oWhere.SQL & " order by SiteURL"
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                Return oDT
            Catch ex As Exception
                Throw ex
            Finally
                oDB.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Shared Function CheckDuplicateURL(ByVal psURL As String, ByVal psWhiteLabelSettingID As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SiteURL = " & SQLString(LCase(psURL)))
            oWhere.AppendOptionalANDCondition("WhiteLabelSettingID ", SQLString(psWhiteLabelSettingID), "<>")

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "select top 1 * from WhiteLabelSettings " & oWhere.SQL
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                Return oDT.Rows.Count = 0
            Catch ex As Exception
                Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CWhiteLabelSettings))
                log.Error("Can't CheckDuplicateURL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function
    End Class
End Namespace