Imports System.Collections
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Net.Mail
Imports System.Web.Configuration
Imports SBCBL.CacheUtils
Imports WebsiteLibrary.DBUtils
Imports SBCBL.Managers
Imports System.Web.UI
Imports System.Xml

Public Class std
    Inherits WebsiteLibrary.CSBCStd

    Public Shared validLoginRegEx As String = "[a-zA-Z0-9_]"
    Public Shared validEmailRegEx As String = "^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"

    'Private Shared log As log4net.ILog = log4net.LogManager.GetLogger(GetType(std))

#Region "App Settings"
    Public Shared SBC_CONNECTION_STRING As String = WebConfigurationManager.ConnectionStrings("SBC_CONNECTION_STRING").ConnectionString
    Public Shared SBC2_CONNECTION_STRING As String = WebConfigurationManager.ConnectionStrings("SBC2_CONNECTION_STRING").ConnectionString
    ' Public Shared ASTERIA_CONNECTION_STRING As String = WebConfigurationManager.ConnectionStrings("ASTERIA_CONNECTION_STRING").ConnectionString
    Public Shared CASINO_CONNECTION_STRING As String = WebConfigurationManager.ConnectionStrings("CASINO_CONNECTION_STRING").ConnectionString

    Public Shared US_EMAIL_ADDRESS As String = WebConfigurationManager.AppSettings("US_EMAIL_ADDRESS")
    Public Shared SMTP_SERVER As String = WebConfigurationManager.AppSettings("SMTP_SERVER")
    Public Shared POP_SERVER As String = WebConfigurationManager.AppSettings("POP_SERVER")
    Public Shared AUTHENTICATION_SERVER As Boolean = SafeString(WebConfigurationManager.AppSettings("AUTHENTICATION_SERVER")) = "Y"
    Public Shared SSL_SERVER As Boolean = SafeString(WebConfigurationManager.AppSettings("SSL_SERVER")) = "Y"
    Public Shared SMTP_FROM As String = WebConfigurationManager.AppSettings("SMTP_FROM")
    Public Shared SMTP_LOGIN As String = WebConfigurationManager.AppSettings("SMTP_LOGIN")
    Public Shared SMTP_PASSWORD As String = WebConfigurationManager.AppSettings("SMTP_PASSWORD")
    Public Shared SYSTEM_EMAIL As String = WebConfigurationManager.AppSettings("SYSTEM_EMAIL")

    Public Shared SBC_PURCHASE_EMAIL As String = WebConfigurationManager.AppSettings("SBC_PURCHASE_EMAIL")
    Public Shared SBC_BILLING_EMAIL As String = WebConfigurationManager.AppSettings("SBC_BILLING_EMAIL")
    Public Shared SBC_ACTIVE_ACCOUNT_EMAIL As String = WebConfigurationManager.AppSettings("SBC_ACTIVE_ACCOUNT_EMAIL")

    Public Shared SUPPORT_URL As String = WebConfigurationManager.AppSettings("SUPPORT_URL")
    Public Shared SUPPORT_FROM As String = WebConfigurationManager.AppSettings("SUPPORT_FROM")
    Public Shared SUPPORT_SERVER As String = WebConfigurationManager.AppSettings("SUPPORT_SERVER")
    Public Shared SUPPORT_LOGIN As String = WebConfigurationManager.AppSettings("SUPPORT_LOGIN")
    Public Shared SUPPORT_PASSWORD As String = WebConfigurationManager.AppSettings("SUPPORT_PASSWORD")
    Public Shared SBS_SYSTEM As String = WebConfigurationManager.AppSettings("SBS_SYSTEM")
    Public Shared CASINO_SUFFIX As String = WebConfigurationManager.AppSettings("CASINO_SUFFIX")
    Public Shared AGENT_ID_DEFAULT As String = WebConfigurationManager.AppSettings("AGENT_ID_DEFAULT")

    Public Shared SOCCER_GAMES As String = WebConfigurationManager.AppSettings("SOCCER_GAMES")
    Public Shared BASEBALL_GAMES As String = WebConfigurationManager.AppSettings("BASEBALL_GAMES")
    Public Shared FOOTBALL_GAMES As String = WebConfigurationManager.AppSettings("FOOTBALL_GAMES")
    Public Shared BASKETBALL_GAMES As String = WebConfigurationManager.AppSettings("BASKETBALL_GAMES")
    Public Shared HOCKEY_GAMES As String = WebConfigurationManager.AppSettings("HOCKEY_GAMES")
    'Public Shared OTHER_GAMES As String = WebConfigurationManager.AppSettings("OTHER_GAMES")

#End Region

    
   
#Region "Send Email"

    Public Shared Sub SendEmail(ByVal psFrom As String, ByVal psTo As String, ByVal psSubject As String, ByVal psBody As String, ByVal poCEmailSettings As CEmailSettings, Optional ByVal psToBCC As String = "")
        Dim oMessage As New MailMessage(psFrom, psTo, psSubject, psBody)
        oMessage.IsBodyHtml = True

        If psToBCC <> "" Then
            oMessage.Bcc.Add(psToBCC)
        End If

        Dim oThread As New Threading.Thread(AddressOf SendEmailAsync)
        oThread.Priority = Threading.ThreadPriority.BelowNormal 'priority is lower so that the aspx can return without waiting for connection to email server, etc

        Dim oParams As New ArrayList()
        oParams.Add(oMessage)
        oParams.Add(poCEmailSettings)

        oThread.Start(oParams)
    End Sub

    Public Shared Sub SendEmail(ByVal poMessage As MailMessage)
        Dim oThread As New Threading.Thread(AddressOf SendEmailAsync)

        Dim oParams As New ArrayList()
        oParams.Add(poMessage)

        Dim oEmailSetting As New CEmailSettings(SMTP_LOGIN, SMTP_PASSWORD)
        oEmailSetting.IncomingMailServer = POP_SERVER
        oEmailSetting.OutgoingMailServer = SMTP_FROM
        oEmailSetting.OutgoingMailRequiresAuthentication = AUTHENTICATION_SERVER
        oEmailSetting.RequiresSSL = SSL_SERVER

        oParams.Add(oEmailSetting)

        oThread.Priority = Threading.ThreadPriority.BelowNormal 'priority is lower so that the aspx can return without waiting for connection to email server, etc
        oThread.Start(oParams)
    End Sub

    Public Shared Sub SendEmail(ByVal poMessage As MailMessage, ByVal poEmailSettings As CEmailSettings)
        Dim oThread As New Threading.Thread(AddressOf SendEmailAsync)

        Dim oParams As New ArrayList()
        oParams.Add(poMessage)
        oParams.Add(poEmailSettings)

        oThread.Priority = Threading.ThreadPriority.BelowNormal 'priority is lower so that the aspx can return without waiting for connection to email server, etc
        oThread.Start(oParams)
    End Sub

    Public Shared Sub SendEmailNoSync(ByVal poMessage As MailMessage)
        Try
            Dim oClient As New SmtpClient(SMTP_SERVER)
            oClient.EnableSsl = True
            oClient.Credentials = New System.Net.NetworkCredential(SMTP_LOGIN, SMTP_PASSWORD)

            poMessage.Sender = poMessage.From

            oClient.Send(poMessage)
        Catch ex As Exception
            'trace the error
            Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(std))
            log.Error("Unable to send email", ex)
        End Try
    End Sub

    Private Shared Sub SendEmailAsync(ByVal poMessage As Object)
        Try
            Dim oClient As SmtpClient
            Dim oParams As ArrayList = CType(poMessage, ArrayList)

            Try
                Dim oEmailSettings As CEmailSettings = CType(oParams(1), CEmailSettings)
                oClient = New SmtpClient(oEmailSettings.OutgoingMailServer)

                If oEmailSettings.OutgoingMailRequiresAuthentication Then
                    oClient.Credentials = New System.Net.NetworkCredential(oEmailSettings.EmailLogin, oEmailSettings.EmailPassword)
                End If

                oClient.EnableSsl = oEmailSettings.RequiresSSL
                oClient.Send(CType(oParams(0), MailMessage))

            Catch ex As Exception
                oClient = New SmtpClient(SMTP_SERVER)
                oClient.EnableSsl = True
                oClient.Credentials = New System.Net.NetworkCredential(SMTP_LOGIN, SMTP_PASSWORD)

                Dim oMail As MailMessage = CType(oParams(0), MailMessage)
                oMail.Sender = oMail.From

                oClient.Send(oMail)
            End Try
        Catch ex As Exception
            'trace the error
            Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(std))
            log.Error("Unable to send email : " & ex.Message & ex.ToString, ex)
        End Try
    End Sub
#End Region

    Public Shared Function ValidLogin(ByVal psLogin As String) As Boolean
        If (New Regex(validLoginRegEx)).IsMatch(psLogin) Or (New Regex(validEmailRegEx)).IsMatch(psLogin) Then
            Return True
        End If

        Return False
    End Function

    Public Shared Function ValidPassword(ByVal psPassword As String) As Boolean
        If psPassword = "" Then
            Return True
        End If

        Dim oRegex As New Regex(validLoginRegEx)
        Return oRegex.IsMatch(psPassword)

    End Function

    Public Shared Function ValidEmail(ByVal psEmail As String) As Boolean
        If SafeString(psEmail) = "" Then
            Return True
        End If

        Dim oRegex As New Regex(validEmailRegEx)
        Return oRegex.IsMatch(psEmail)
    End Function

    Public Shared Function RandomNumChar() As Char
        Return ChrW(48 + CInt(Rnd() * 9))
    End Function

    Public Shared Function GetEasternMondayOfCurrentWeek(Optional ByVal pnTimeZone As Integer = 0) As Date
        Dim oCurrent As DateTime = GetEasternDate()

        Select Case oCurrent.DayOfWeek
            Case DayOfWeek.Monday
                Return oCurrent
            Case DayOfWeek.Tuesday
                Return oCurrent.AddDays(-1)
            Case DayOfWeek.Wednesday
                Return oCurrent.AddDays(-2)
            Case DayOfWeek.Thursday
                Return oCurrent.AddDays(-3)
            Case DayOfWeek.Friday
                Return oCurrent.AddDays(-4)
            Case DayOfWeek.Saturday
                Return oCurrent.AddDays(-5)
            Case Else 'Sunday
                Return oCurrent.AddDays(-6)
        End Select
    End Function

    Public Shared Function GetMondayOfCurrentWeek(Optional ByVal pnTimeZone As Integer = 0) As Date
        Dim oCurrent As DateTime = Date.Now.ToUniversalTime().AddHours(pnTimeZone)

        Select Case oCurrent.DayOfWeek
            Case DayOfWeek.Monday
                Return oCurrent
            Case DayOfWeek.Tuesday
                Return oCurrent.AddDays(-1)
            Case DayOfWeek.Wednesday
                Return oCurrent.AddDays(-2)
            Case DayOfWeek.Thursday
                Return oCurrent.AddDays(-3)
            Case DayOfWeek.Friday
                Return oCurrent.AddDays(-4)
            Case DayOfWeek.Saturday
                Return oCurrent.AddDays(-5)
            Case Else 'Sunday
                Return oCurrent.AddDays(-6)
        End Select
    End Function

    Public Shared Function GetLastMondayOfDate(ByVal poDate As Date) As Date
        Select Case poDate.DayOfWeek
            Case DayOfWeek.Monday
                Return poDate
            Case DayOfWeek.Tuesday
                Return poDate.AddDays(-1)
            Case DayOfWeek.Wednesday
                Return poDate.AddDays(-2)
            Case DayOfWeek.Thursday
                Return poDate.AddDays(-3)
            Case DayOfWeek.Friday
                Return poDate.AddDays(-4)
            Case DayOfWeek.Saturday
                Return poDate.AddDays(-5)
            Case Else 'Sunday
                Return poDate.AddDays(-6)
        End Select
    End Function

    Public Shared Function GetNextSundayOfDate(ByVal poDate As Date) As Date
        Select Case poDate.DayOfWeek
            Case DayOfWeek.Monday
                Return poDate
            Case DayOfWeek.Tuesday
                Return poDate.AddDays(-1)
            Case DayOfWeek.Wednesday
                Return poDate.AddDays(-2)
            Case DayOfWeek.Thursday
                Return poDate.AddDays(-3)
            Case DayOfWeek.Friday
                Return poDate.AddDays(-4)
            Case DayOfWeek.Saturday
                Return poDate.AddDays(-5)
            Case Else 'Sunday
                Return poDate.AddDays(-6)
        End Select
    End Function

    ''' <summary>
    ''' Get Eastern Datetime
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEasternDate() As Date
        Dim oDate As DateTime = Now.ToUniversalTime()
        If Now.IsDaylightSavingTime() Then
            oDate = oDate.AddHours(1)
        End If
        oDate = oDate.AddHours(-5)

        Return oDate
    End Function

#Region "Rounding Option"

    Public Shared Function SafeRound(ByVal poSource As Object) As Double
        Dim nSource As Double = SafeDouble(poSource)

        If nSource = 0 Then
            Return 0
        End If
        Return Math.Ceiling(nSource)
        'Select Case UCase((New CCacheManager).GetSysSettings("ROUNDING OPTION").GetValue("Rounding"))
        '    Case "UP"
        '        Return Math.Ceiling(nSource)

        '    Case "DOWN"
        '        Return Math.Floor(nSource)

        '    Case "NEAREST"
        '        Return Math.Round(nSource)
        'End Select

        'Return Math.Round(nSource, 2) 'NONE
    End Function

    Public Shared Function GetRoundMidPoint() As Integer
        Select Case UCase((New CCacheManager).GetSysSettings("ROUNDING OPTION").GetValue("Rounding"))
            Case "UP", "DOWN", "NEAREST"
                Return 0

            Case Else   'NONE
                Return 2
        End Select
    End Function

#End Region

#Region "GameType"

    Public Shared Function IsSoccer(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "ARGENTINE", "BRAZIL", "BUNDESLIGA", "LA LIGA", _
            "LIGUE 1", "MLS", "NETHERLANDS", _
            "PORTUGAL", "PREMIER", "SCOTLAND", "SERIE A", "WORLD CUP", "EURO CUPS", "SUPER LIGA", "COPA AMERICA", _
            "CARLING CUP", "FA CUP", "CONCACAF", "TURKEY", "JAPAN LEAGUE 2", "JAPAN LEAGUE 1", "INTL FRIENDLY", "ARGENTINA B", "BRAZIL B", _
            "DENMARK", "AUSTRALIA", "SPAIN", "SERIE B", "EUROPA LEAGUE", "CHAMPIONS LEAGUE", "EURO UNDER 21", "SEGUNDA", "WORLD CUP ASIA", "WORLD CUP S. AMERICA", "WORLD CUP EUROPE", "WORLD CUP CONCACAF", "US CUP", "ASIAN CUPS", "RUSSIA", "SPAIN", "SWEDEN", "SWITZERLAND", "UEFA", "USA", "AUSTRIA", "BELGIUM", "BARZIL", "GERMANY", "CHILE", "DENMARK", "ENGLISH", "FINLAND", "FRANCE", "IRELAND", "ITALY", "JAPAN", "MEXICAN", "NORWAY", "OLYMPICS"
                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function IsFootball(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "AFL FOOTBALL", "CFL FOOTBALL", "NCAA FOOTBALL", "NFL FOOTBALL", "NFL FOOTBALL LIVE", "NFL PRESEASON", "UFL FOOTBALL"

                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function IsBaseball(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "MLB AL BASEBALL", "MLB NL BASEBALL", "MLB BASEBALL LIVE", "MLB BASEBALL", "NCAA BASEBALL"

                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function IsHockey(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "NHL HOCKEY", "NCAA HOCKEY"

                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function IsBasketball(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "NBA BASKETBALL", "NCAA BASKETBALL LIVE", "NBA BASKETBALL LIVE", "NCAA BASKETBALL", "WNBA BASKETBALL", "WNBA BASKETBALL LIVE", _
            "WNCAA BASKETBALL", "NCAA BASKETBALL", "WNCAA BASKETBALL"

                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function GetSportType(ByVal psGameType As String) As String
        Select Case UCase(psGameType)
            Case "NCAA BASKETBALL LIVE", "NBA BASKETBALL LIVE", "NBA BASKETBALL", "NCAA BASKETBALL", "WNBA BASKETBALL", "WNBA BASKETBALL LIVE", _
            "WNCAA BASKETBALL", "NCAA BASKETBALL", "WNCAA BASKETBALL"
                Return "basketball"
            Case "NHL HOCKEY", "NCAA HOCKEY"
                Return "Hockey"
            Case "MLB AL BASEBALL", "MLB NL BASEBALL", "MLB BASEBALL LIVE", "MLB BASEBALL", "MLB BASEBALL LIVE", "NCAA BASEBALL"
                Return "Baseball"
            Case "AFL FOOTBALL", "CFL FOOTBALL", "NCAA FOOTBALL", "NFL FOOTBALL", "NFL FOOTBALL LIVE", "NFL PRESEASON", "UFL FOOTBALL"
                Return "Football"
            Case "ARGENTINE", "BRAZIL", "BUNDESLIGA", "LA LIGA", _
       "LIGUE 1", "MEXICAN", "MLS", "NETHERLANDS", _
       "PORTUGAL", "PREMIER", "SCOTLAND", "SERIE A", "WORLD CUP", "EURO CUPS", _
       "SUPER LIGA", "COPA AMERICA", "CARLING CUP", "FA CUP", "CONCACAF", "TURKEY", "JAPAN LEAGUE 2", "JAPAN LEAGUE 1", "INTL FRIENDLY", "ARGENTINA B", "BRAZIL B", _
           "DENMARK", "SERIE B", "SPAIN", "EURO UNDER 21", "WORLD CUP ASIA", "WORLD CUP S. AMERICA", "WORLD CUP EUROPE", "WORLD CUP CONCACAF", "NORWAY", "SEGUNDA", "US CUP", "ASIAN CUPS", "RUSSIA", "SPAIN", "SWEDEN", "SWITZERLAND", "UEFA", "USA", "AUSTRIA", "BELGIUM", "BARZIL", "GERMANY", "CHILE", "DENMARK", "ENGLISH", "FINLAND", "FRANCE", "IRELAND", "ITALY", "JAPAN", "MEXICAN", "OLYMPICS"
                Return "Soccer"
            Case "GOLF"
                Return "Golf"
            Case "TENNIS"
                Return "Tennis"
            Case Else
                Return ""
        End Select

    End Function

    Public Shared Function IsOtherGameType(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "BOXING", "NASCAR", "GOLF", "TENNIS"

                bResult = True

            Case Else
                bResult = False

        End Select
        Return bResult
    End Function

    Public Shared Function IsGolf(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "GOLF"
                bResult = True
            Case Else
                bResult = False
        End Select
        Return bResult
    End Function

    Public Shared Function IsTennis(ByVal psGameType As String) As Boolean
        Dim bResult As Boolean = True

        Select Case UCase(psGameType)
            Case "TENNIS"
                bResult = True
            Case Else
                bResult = False
        End Select
        Return bResult
    End Function

    Public Shared Function AHFormat(ByVal pnNumber As Double) As String
        Dim nInt As Double = Math.Truncate(pnNumber)
        Dim nDec As Double = SafeDouble(pnNumber - nInt)
        Dim nHome, nAway As Double

        If nDec < 0 Then
            nHome = -1
            nAway = -1
        Else
            nHome = 1
            nAway = 1
        End If

        nDec = Math.Abs(nDec)
        If nDec <> 0.3D And nDec <> 0.8D Then
            Return SafeString(IIf(pnNumber > 0, "+" & pnNumber, pnNumber))
        End If

        Select Case nDec
            Case 0.3D
                nHome *= 0
                nAway *= 0.5

            Case 0.8D
                nHome *= 0.5
                nAway *= 1

        End Select

        nHome += nInt
        nAway += nInt

        Return SafeString(IIf(nHome > 0, "+" & nHome, nHome)) & "," & SafeString(nAway)
    End Function

    Public Shared Function GetGameType() As System.Collections.Generic.Dictionary(Of String, String)

        Dim oGameType As New System.Collections.Generic.Dictionary(Of String, String)()

        '' Football
        oGameType("AFL Football") = "AFL Football"
        oGameType("CFL Football") = "CFL Football"
        oGameType("NCAA Football") = "NCAA Football"
        oGameType("NFL Football") = "NFL Football"
        'oGameType("NFL Football Live") = "NFL Football Live"
        oGameType("NFL Preseason") = "NFL Preseason"
        oGameType("UFL Football") = "UFL Football"
        '' Basketball
        oGameType("NBA Basketball") = "NBA Basketball"
        oGameType("NCAA Basketball") = "NCAA Basketball"
        oGameType("WNBA Basketball") = "WNBA Basketball"
        'oGameType("WNBA Basketball Live") = "WNBA Basketball Live"
        oGameType("WNCAA Basketball") = "WNCAA Basketball"

        '' Baseball
        oGameType("MLB AL Baseball") = "MLB AL Baseball"
        oGameType("MLB NL Baseball") = "MLB NL Baseball"
        oGameType("MLB Baseball") = "MLB Baseball"
        'oGameType("MLB Baseball Live") = "MLB Baseball Live"
        oGameType("NCAA Baseball") = "NCAA Baseball"

        '' Hockey
        oGameType("NHL Hockey") = "NHL Hockey"
        oGameType("NCAA Hockey") = "NCAA Hockey"

        '' Soccer
        oGameType("Argentine") = "Argentine"
        oGameType("Argentina B") = "Argentina B"
        oGameType("Brazil") = "Brazil"
        oGameType("Brazil B") = "Brazil B"
        oGameType("Bundesliga") = "Bundesliga"
        oGameType("Carling Cup") = "Carling Cup"
        oGameType("Champions League") = "Champions League"
        oGameType("Concacaf") = "Concacaf"
        oGameType("Copa America") = "Copa America"
        oGameType("Euro") = "Euro"
        oGameType("Europa League") = "Europa League"
        oGameType("FA Cup") = "FA Cup"
        oGameType("La Liga") = "La Liga"
        oGameType("Super Liga") = "Super Liga"
        oGameType("Ligue 1") = "Ligue 1"
        oGameType("Mexican") = "Mexican"
        oGameType("MLS") = "MLS"
        oGameType("Spain") = "Spain"
        oGameType("Netherlands") = "Netherlands"
        oGameType("Portugal") = "Portugal"
        oGameType("Premier") = "Premier"
        oGameType("Scotland") = "Scotland"
        oGameType("Serie A") = "Serie A"
        oGameType("World Cup") = "World Cup"
        oGameType("Austria") = "Austria"
        oGameType("Australia") = "Australia"
        oGameType("Greece") = "Greece"
        oGameType("Japan") = "Japan"
        oGameType("Turkey") = "Turkey"
        oGameType("Belgium") = "Belgium"
        oGameType("Israel") = "Israel"
        oGameType("Russia") = "Russia"
        oGameType("Spain") = "Spain"
        oGameType("Sweden") = "Sweden"
        oGameType("Switzerland") = "Switzerland"
        oGameType("UEFA") = "UEFA"
        oGameType("USA") = "USA"
        oGameType("Germany") = "Germany"
        oGameType("Chile") = "Chile"
        oGameType("Denmark") = "Denmark"
        oGameType("English") = "English"
        oGameType("Finland") = "Finland"
        oGameType("France") = "France"
        oGameType("Ireland") = "Ireland"
        oGameType("Italy") = "Italy"
        oGameType("Mexican") = "Mexican"
        oGameType("Norway") = "Norway"
        oGameType("World Cup ASIA") = "World Cup ASIA"
        oGameType("Euro Under 21") = "Euro Under 21"
        oGameType("World Cup CONCACAF") = "World Cup CONCACAF"
        oGameType("World Cup Europe") = "World Cup Europe"
        oGameType("World Cup S. America") = "World Cup S. America"
        oGameType("Olympics") = "Olympics"
        oGameType("INTL FRIENDLY") = "INTL FRIENDLY"
        ''tennis sport
        oGameType("Tennis") = "Tennis"
        '' golf Sport 
        oGameType("Golf") = "Golf"
        '' Other
        oGameType("Boxing") = "Boxing"
        oGameType("NASCAR") = "NASCAR"


        Return oGameType
    End Function
#End Region

#Region "SiteType"
    Public Shared Function ConvertStringToSiteType(ByVal psSiteType As String) As CEnums.ESiteType
        Select Case UCase(SafeString(psSiteType))
            Case "SBC"
                Return CEnums.ESiteType.SBS

            Case "SBS"
                Return CEnums.ESiteType.SBS

            Case Else
                Return CEnums.ESiteType.SBS

        End Select

    End Function

    Public Shared Function GetSiteType() As String
        Return "SBS"
        'Dim oCache As New SBCBL.CacheUtils.CCacheManager()
        'Dim eSiteType As SBCBL.CEnums.ESiteType = oCache.GetSiteType(System.Web.HttpContext.Current.Request.Url.Host)
        'Return eSiteType.ToString()
    End Function
#End Region

#Region "SubTitle"

    Public Shared Sub DisplaySubTitlle(ByVal poMasterPage As MasterPage, ByVal psSubTitle As String)
        If poMasterPage IsNot Nothing Then
            Dim odivSubTitle As HtmlControls.HtmlGenericControl = CType(poMasterPage.FindControl("divSubTitle"), HtmlControls.HtmlGenericControl)
            If odivSubTitle IsNot Nothing Then
                odivSubTitle.Visible = True
                odivSubTitle.InnerText = psSubTitle
            End If
        End If
    End Sub

#End Region

    Public Shared Function DownloadFileToLocal(ByVal psFolder As String, ByVal psLink As String, ByVal psFileName As String) As Boolean
        Dim bResult As Boolean = False

        Try
            Using client As New System.Net.WebClient()
                Dim oFileBytes() As Byte = client.DownloadData(psLink)

                If oFileBytes IsNot Nothing Then

                    Dim oDB As New FileDB.CFileDB()

                    '' Create New Folder is not exist
                    If Not oDB.Exist(psFolder) Then
                        oDB.CreateDatabase(psFolder)
                    End If

                    Dim oHandle As FileDB.CFileHandle = oDB.GetFile(psFolder, psFileName)

                    If oHandle Is Nothing Then
                        oHandle = oDB.GetNewFileHandle()
                    End If

                    ' save to filedb
                    File.WriteAllBytes(oHandle.LocalFileName, oFileBytes)
                    oDB.PutFile(psFolder, psFileName, oHandle)
                End If
            End Using

            bResult = True
        Catch ex As Exception
            'log.Error("Fail to download file from link: " & psLink, ex)
        End Try

        Return bResult
    End Function

    Public Shared Function GetTeamplateAgentSetting() As String
        Dim oDB As New FileDB.CFileDB()
        Dim oHandle As FileDB.CFileHandle = oDB.GetNewFileHandle()
        If Not oDB.Exist(SBS_SYSTEM + "\" & AGENT_ID_DEFAULT) Then
            'log.Error("Agent Default not Exists : " & AGENT_ID_DEFAULT)
            Return ""
        End If
        oHandle = oDB.GetFile(SBS_SYSTEM & "\" & AGENT_ID_DEFAULT, "AgentSettingGame.xml")
        Dim sFileName As String = "/Utils/TeamplateAgentSetting.xml"
        Dim oDoc As New XmlDocument()
        oDoc.Load(oHandle.LocalFileName) '(System.Web.HttpContext.Current.Server.MapPath("../../" & sFileName))
        Dim sAgentTemplate As String = System.Web.HttpContext.Current.Server.HtmlDecode(oDoc.DocumentElement.InnerXml)
        Return sAgentTemplate
    End Function

    Public Shared Function GetBettingProfileSubAgent(ByVal psAgentID As String) As Boolean
        Dim sAgentID As String = psAgentID
        Dim oCacheManager As New CCacheManager()
        While True
            If Not oCacheManager.GetAgentInfo(sAgentID).IsEnableBettingProfile Then
                Return False
            End If
            If Not String.IsNullOrEmpty(oCacheManager.GetAgentInfo(sAgentID).ParentID) Then
                sAgentID = oCacheManager.GetAgentInfo(sAgentID).ParentID
            Else
                Return oCacheManager.GetAgentInfo(psAgentID).IsEnableBettingProfile
            End If
        End While
        Return True
    End Function

    Public Shared Function GetPlayerTemplateSubAgent(ByVal psAgentID As String) As Boolean
        Dim oCacheManager As New CCacheManager()
        Dim sAgentID As String = psAgentID
        While True
            If oCacheManager.GetAgentInfo(sAgentID) Is Nothing OrElse Not oCacheManager.GetAgentInfo(sAgentID).IsEnablePlayerTemplate Then
                Return False
            End If
            If Not String.IsNullOrEmpty(oCacheManager.GetAgentInfo(sAgentID).ParentID) Then
                sAgentID = oCacheManager.GetAgentInfo(sAgentID).ParentID
            Else
                Return oCacheManager.GetAgentInfo(psAgentID).IsEnablePlayerTemplate
            End If
        End While
        Return True
    End Function

    Public Shared Function GetHasCrediLimitSetting(ByVal psAgentID As String) As Boolean
        Dim oCacheManager As New CCacheManager()
        Dim sAgentID As String = psAgentID
        While True
            If Not oCacheManager.GetAgentInfo(sAgentID).HasCrediLimitSetting Then
                Return False
            End If
            If Not String.IsNullOrEmpty(oCacheManager.GetAgentInfo(sAgentID).ParentID) Then
                sAgentID = oCacheManager.GetAgentInfo(sAgentID).ParentID
            Else
                Return oCacheManager.GetAgentInfo(psAgentID).HasCrediLimitSetting
            End If
        End While
        Return True
    End Function

    Public Shared Function GetAddNewSubAgent(ByVal psAgentID As String) As Boolean
        Dim oCacheManager As New CCacheManager()
        Dim sAgentID As String = psAgentID
        While True
            If Not oCacheManager.GetAgentInfo(sAgentID).AddNewSubAgent Then
                Return False
            End If
            If Not String.IsNullOrEmpty(oCacheManager.GetAgentInfo(sAgentID).ParentID) Then
                sAgentID = oCacheManager.GetAgentInfo(sAgentID).ParentID
            Else
                Return oCacheManager.GetAgentInfo(psAgentID).AddNewSubAgent
            End If
        End While
        Return True
    End Function

    Public Shared Function GetOrdinalNumber(ByVal pnIndex As Integer) As String
        Select Case pnIndex
            Case 1
                Return "First"
            Case 2
                Return "Second"
            Case 3
                Return "Third"
            Case 4
                Return "Fourth"
            Case 5
                Return "Fifth"
            Case 6
                Return "Sixth"
            Case 7
                Return "Seventh"
            Case 8
                Return "Eighth"
            Case 9
                Return "Ninth"
            Case 10
                Return "Tenth"
            Case 11
                Return "Eleventh"
            Case 12
                Return "Twelfth"
            Case 13
                Return "Thirdteenth"
            Case Else
                Dim nLastDigit As Integer = pnIndex Mod 10
                Dim sOrdinal As String = "th"

                If nLastDigit = 1 Then
                    sOrdinal = "st"
                ElseIf nLastDigit = 2 Then
                    sOrdinal = "nd"
                ElseIf nLastDigit = 3 Then
                    sOrdinal = "rd"
                Else
                    sOrdinal = "th"
                End If
                Return pnIndex.ToString & sOrdinal
        End Select
    End Function

    Public Shared Function safeVegass(ByVal pnPoint As Double) As String
        If SafeString(pnPoint).Contains("-0.5,") OrElse SafeString(pnPoint).Contains("0.5,") OrElse SafeString(pnPoint).Contains(",-0.5") OrElse SafeString(pnPoint).Contains(",0.5") OrElse SafeString(pnPoint).Equals("0.5") OrElse SafeString(pnPoint).Equals("0.5") OrElse SafeString(pnPoint).Equals("-0.5") Then
            Return SafeString(pnPoint).Replace("0.5", "½")
        Else
            Return SafeString(pnPoint).Replace(".5", "½")
        End If
    End Function

    Public Shared Function safeVegass(ByVal pnPoint As String) As String
        If pnPoint.Contains("-0.5") OrElse pnPoint.Contains("+0.5") OrElse pnPoint.Contains("-0.5,") OrElse pnPoint.Contains("0.5,") OrElse pnPoint.Contains(",-0.5") OrElse pnPoint.Contains(",0.5") OrElse (pnPoint.Length = 3 AndAlso pnPoint.Equals("0.5")) OrElse pnPoint.Equals("+0.5") OrElse pnPoint.Equals("-0.5") Then
            Return IIf(SafeDouble(pnPoint) > 0, SafeString(pnPoint).Replace("0.5", "½"), SafeString(pnPoint).Replace("0.5", "½")).ToString()
        Else
            Return IIf(SafeDouble(pnPoint) > 0, SafeString(pnPoint).Replace(".5", "½"), SafeString(pnPoint).Replace(".5", "½")).ToString()
        End If
    End Function

    Public Shared Function GetAllSportType() As List(Of String)
        Dim lstSportTypes As New List(Of String)
        lstSportTypes.Add("Football")
        lstSportTypes.Add("Basketball")
        lstSportTypes.Add("Baseball")
        lstSportTypes.Add("Hockey")
        lstSportTypes.Add("Soccer")
        lstSportTypes.Add("Other Sports")
        Return lstSportTypes
    End Function

    Public Shared Function getListGameType(ByVal psSporttype As String) As List(Of String)
        Dim lstGameType As New List(Of String)
        Select Case (psSporttype)
            Case "Football"
                lstGameType.Add("NFL Preseason")
                lstGameType.Add("NFL Football")
                lstGameType.Add("NCAA Football")
                lstGameType.Add("CFL Football")
                lstGameType.Add("AFL Football")
                lstGameType.Add("UFL Football")

            Case "Basketball"
                lstGameType.Add("NBA Basketball")
                lstGameType.Add("NCAA Basketball")
                lstGameType.Add("WNBA Basketball")
                lstGameType.Add("WNCAA Basketball")
                'lstGameType.Add("FIBA Basketball")
            Case "Baseball"
                lstGameType.Add("MLB Baseball")
                lstGameType.Add("NCAA Baseball")
                'lstGameType.Add("Classic")
            Case "Live Game"
                lstGameType.Add("MLB Baseball Live")
                lstGameType.Add("NFL Football Live")
                lstGameType.Add("WNBA Basketball Live")
                lstGameType.Add("NBA Basketball Live")
                lstGameType.Add("NCAA Basketball Live")
            Case "Hockey"

                lstGameType.Add("NHL Hockey")
                lstGameType.Add("NCAA Hockey")
            Case "Soccer"
                lstGameType.Add("Premier")
                lstGameType.Add("La Liga")
                lstGameType.Add("Serie A")
                lstGameType.Add("Ligue 1")
                lstGameType.Add("Bundesliga")
                lstGameType.Add("Champions League")
                lstGameType.Add("Europa League")
                lstGameType.Add("Concacaf")
                lstGameType.Add("Euro")
                lstGameType.Add("Copa America")
                lstGameType.Add("Carling Cup")
                lstGameType.Add("FA Cup")
                lstGameType.Add("Argentine")
                lstGameType.Add("Argentina B")
                lstGameType.Add("Brazil")
                lstGameType.Add("Brazil B")
                lstGameType.Add("Mexican")
                lstGameType.Add("MLS")
                lstGameType.Add("Netherlands")
                lstGameType.Add("Portugal")
                lstGameType.Add("Scotland")
                lstGameType.Add("Super Liga")
                lstGameType.Add("World Cup")
                lstGameType.Add("Australia")
                lstGameType.Add("Austria")
                lstGameType.Add("Greece")
                lstGameType.Add("Japan")
                lstGameType.Add("Turkey")
                lstGameType.Add("Belgium")
                lstGameType.Add("Israel")
                lstGameType.Add("Russia")
                lstGameType.Add("Spain")
                lstGameType.Add("Sweden")
                lstGameType.Add("Switzerland")
                lstGameType.Add("UEFA")
                lstGameType.Add("USA")
                lstGameType.Add("Spain")
                lstGameType.Add("Germany")
                lstGameType.Add("Chile")
                lstGameType.Add("Denmark")
                lstGameType.Add("English")
                lstGameType.Add("Finland")
                lstGameType.Add("France")
                lstGameType.Add("Ireland")
                lstGameType.Add("Italy")
                lstGameType.Add("Mexican")
                lstGameType.Add("Norway")
                lstGameType.Add("Euro Under 21")
                lstGameType.Add("World Cup S. America")
                lstGameType.Add("World Cup Europe")
                lstGameType.Add("World Cup CONCACAF")
                lstGameType.Add("World Cup ASIA")
                lstGameType.Add("Olympics")
                lstGameType.Add("INTL FRIENDLY")
            Case "Other Sports"
                lstGameType.Add("Tennis")
                lstGameType.Add("NASCAR")
                'lstGameType.Add("IndyCar")
                lstGameType.Add("Golf")
                lstGameType.Add("Boxing")
                'lstGameType.Add("MMA")
        End Select
        Return lstGameType
    End Function

    Public Shared Function getAllGameType() As List(Of String)
        Dim lstGameType As New List(Of String)
        lstGameType.Add("NFL Football")
        lstGameType.Add("NCAA Football")
        lstGameType.Add("CFL Football")
        lstGameType.Add("AFL Football")
        lstGameType.Add("UFL Football")
        lstGameType.Add("NFL Preseason")
        lstGameType.Add("NBA Basketball")
        lstGameType.Add("NBA Basketball Live")
        lstGameType.Add("NCAA Basketball Live")
        lstGameType.Add("NCAA Basketball")
        lstGameType.Add("WNBA Basketball")
        lstGameType.Add("WNCAA Basketball")
        lstGameType.Add("MLB Baseball")
        lstGameType.Add("MLB Baseball Live")
        lstGameType.Add("NCAA Baseball")
        lstGameType.Add("NHL Hockey")
        lstGameType.Add("NCAA Hockey")
        lstGameType.Add("Premier")
        lstGameType.Add("La Liga")
        lstGameType.Add("Serie A")
        lstGameType.Add("Ligue 1")
        lstGameType.Add("Bundesliga")
        lstGameType.Add("Champions League")
        lstGameType.Add("Europa League")
        lstGameType.Add("Concacaf")
        lstGameType.Add("Euro")
        lstGameType.Add("Copa America")
        lstGameType.Add("Carling Cup")
        lstGameType.Add("FA Cup")
        lstGameType.Add("Argentine")
        lstGameType.Add("Argentina B")
        lstGameType.Add("Brazil")
        lstGameType.Add("Brazil B")
        lstGameType.Add("Mexican")
        lstGameType.Add("MLS")
        lstGameType.Add("Netherlands")
        lstGameType.Add("Portugal")
        lstGameType.Add("Scotland")
        lstGameType.Add("Super Liga")
        lstGameType.Add("World Cup")
        lstGameType.Add("Australia")
        lstGameType.Add("Austria")
        lstGameType.Add("Greece")
        lstGameType.Add("Japan")
        lstGameType.Add("Turkey")
        lstGameType.Add("Belgium")
        lstGameType.Add("Israel")
        lstGameType.Add("Russia")
        lstGameType.Add("Spain")
        lstGameType.Add("Sweden")
        lstGameType.Add("Switzerland")
        lstGameType.Add("UEFA")
        lstGameType.Add("USA")
        lstGameType.Add("Germany")
        lstGameType.Add("Chile")
        lstGameType.Add("Spain")
        lstGameType.Add("Denmark")
        lstGameType.Add("English")
        lstGameType.Add("Finland")
        lstGameType.Add("France")
        lstGameType.Add("Ireland")
        lstGameType.Add("Italy")
        lstGameType.Add("Mexican")
        lstGameType.Add("Norway")
        lstGameType.Add("Euro Under 21")
        lstGameType.Add("World Cup S. America")
        lstGameType.Add("World Cup Europe")
        lstGameType.Add("World Cup CONCACAF")
        lstGameType.Add("World Cup ASIA")
        lstGameType.Add("Olympics")
        lstGameType.Add("Tennis")
        lstGameType.Add("NASCAR")
        lstGameType.Add("Golf")
        lstGameType.Add("Boxing")

        Return lstGameType
    End Function

    Public Shared Function CheckRoundRobin(ByVal psAgentID As String) As Boolean
        Dim _sRoundRobinKey As String = "Round_" & psAgentID
        Dim osys As SBCBL.CacheUtils.CSysSettingList = (New SBCBL.CacheUtils.CCacheManager()).GetSysSettings("Round_Robin")
        If osys IsNot Nothing Then
            If String.IsNullOrEmpty(osys.GetValue(_sRoundRobinKey)) Then
                Return True
            Else
                Return osys.GetBooleanValue(_sRoundRobinKey)
            End If
        Else
            Return True
        End If
    End Function

    Public Shared Function GetSiteURL() As Dictionary(Of String, String)
        Dim sKey As String = "SITE_URL"
        If System.Web.HttpContext.Current.Cache(sKey) Is Nothing Then
            Dim oSuperManager As New CSuperUserManager()
            Dim dicSiteURl As New Dictionary(Of String, String)
            Dim odt = oSuperManager.GetSuperAdmins("SBS", False)
            For Each odr As DataRow In odt.Rows
                If String.IsNullOrEmpty(SafeString(odr("SiteURL"))) AndAlso String.IsNullOrEmpty(SafeString(odr("SiteURLBackup"))) Then
                    dicSiteURl(SafeString(odr("SuperAdminID"))) = ""
                Else
                    dicSiteURl(SafeString(odr("SuperAdminID"))) = (SafeString(odr("SiteURL")) & "_" & SafeString(odr("SiteURLBackup")))
                End If
            Next
            System.Web.HttpContext.Current.Cache.Add(sKey, dicSiteURl, Nothing, Date.Now.AddMinutes(10), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
        End If
        Return CType(System.Web.HttpContext.Current.Cache.Get(sKey), Dictionary(Of String, String))
    End Function

  
End Class
