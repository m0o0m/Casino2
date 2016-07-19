Imports SBCBL.std


Public Class CServiceStd
    Public Shared SPORTBET_URL_LINE As String = System.Configuration.ConfigurationManager.AppSettings("SPORTBET_URL_LINE")
    Public Shared SPORTBET_URL_SCORE As String = System.Configuration.ConfigurationManager.AppSettings("SPORTBET_URL_SCORE")
    Public Shared SPORTSINSIGHTS_KEY As String = System.Configuration.ConfigurationManager.AppSettings("SPORTSINSIGHTS_KEY")
    Public Shared SPORTSINSIGHTS_URL As String = System.Configuration.ConfigurationManager.AppSettings("SPORTSINSIGHTS_URL")
    Public Shared SPORTSINSIGHTS_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTSINSIGHTS_MAX_TIMEOUT")) 'timeout in seconds
    Public Shared SPORTSINSIGHTS_FULL_REFRESH_SECS As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTSINSIGHTS_FULL_REFRESH_SECS")) 'full xml refresh in seconds
    Public Shared SPORTSINSIGHT_EXECUTION_START As Double = -1 '-1 value means we don't already have a thread executing the line feed
    Public Shared SPORTS_BOOKMAKER_AGENT_EXECUTION_START As Double = -1

    Public Shared SPORTSINSIGHTS_LINEFEED_LAST_TIME As Double = 0 'the last time value to send to sports insights
    Public Shared SPORTSINSIGHTS_LINEFEED_FULL_REFRESH_TIME As Double = 0 'timer value when sports insights did a full refresh

    Public Shared SPORTSINSIGHTS_SCOREFEED_LAST_TIME As Double = 0 'the last time value to send to sports insights
    Public Shared SPORTSINSIGHTS_SCOREFEED_FULL_REFRESH_TIME As Double = 0 'timer value when sports insights did a full refresh

    Public Shared EXECUTE_SPORTS_INSIGHT_LINE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_SPORTS_INSIGHT_LINE") = "Y"
    Public Shared EXECUTE_SPORTS_INSIGHT_SCORE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_SPORTS_INSIGHT_SCORE") = "Y"

    Public Shared EXECUTE_SPORTBET_LINE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_SPORTBET_LINE") = "Y"
    Public Shared EXECUTE_SPORTBET_SCORE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_SPORTBET_SCORE") = "Y"

    Public Shared EXECUTE_CALCULATE_FINAL_RESULT As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CALCULATE_FINAL_RESULT") = "Y"
    Public Shared EXECUTE_CALCULATE_FINAL_RESULT_START As Double = -1 '-1 value means we don't already have a thread executing the calculate final result
    Public Shared EXECUTE_CALCULATE_FINAL_RESULT_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CALCULATE_FINAL_RESULT_MAX_TIMEOUT")) 'timeout in seconds
    Public Shared EXECUTE_LOCK_GAME As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_LOCK_GAME") = "Y"

    Public Shared CLEAR_CACHE_BETTING_ENABLE_URL As String = System.Configuration.ConfigurationManager.AppSettings("CLEAR_CACHE_BETTING_ENABLE_URL")
    Public Shared SMTP_TO As String = System.Configuration.ConfigurationManager.AppSettings("SMTP_TO")

    Public Shared ODDSMINER_URL_LINES As String = System.Configuration.ConfigurationManager.AppSettings("ODDSMINER_URL_LINES")
    Public Shared ODDSMINER_URL_SCORES As String = System.Configuration.ConfigurationManager.AppSettings("ODDSMINER_URL_SCORES")
    Public Shared ODDSMINER_USER As String = System.Configuration.ConfigurationManager.AppSettings("ODDSMINER_USER")
    Public Shared ODDSMINER_PASS As String = System.Configuration.ConfigurationManager.AppSettings("ODDSMINER_PASS")
    Public Shared ODDSMINER_LAST_EXEC As String
    Public Shared ODDSMINER_LAST_SCOREFEED_EXEC As String
    Public Shared ODDSMINER_EXECUTION_START As Double = -1
    Public Shared ODDSMINER_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("ODDSMINER_MAX_TIMEOUT"))
    Public Shared EXECUTE_ODDSMINER_LINE As Boolean = SafeString(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_ODDSMINER_LINE")) = "Y"
    Public Shared EXECUTE_ODDSMINER_SCORE As Boolean = SafeString(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_ODDSMINER_SCORE")) = "Y"

    Public Shared SPORTOPTIONS_URL As String = SafeString(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_URL"))
    Public Shared SPORTOPTIONS_USER As String = SafeString(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_USER"))
    Public Shared SPORTOPTIONS_PASSWORD As String = SafeString(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_PASSWORD"))
    Public Shared SPORTOPTIONS_VERSION As String = SafeString(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_VERSION"))
    Public Shared SPORTOPTIONS_PORT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_PORT"))
    Public Shared SPORTOPTIONS_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_MAX_TIMEOUT"))
    Public Shared EXECUTE_SPORTOPTIONS As Boolean = SafeString(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_SPORTOPTIONS")) = "Y"
    Public Shared SPORTOPTIONS_MAX_RESUME As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_MAX_RESUME"))
    Public Shared SPORTOPTIONS_RESUME_TIME As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_RESUME_TIME"))
    Public Shared SPORTOPTIONS_CRASH_TIME As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("SPORTOPTIONS_CRASH_TIME"))
    Public Shared SPORTOPTIONS_LAST_CRASH As DateTime = Date.MinValue ' means never crash before

    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE_TIME_ZONE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_RESET_ACCOUNT_BALANCE_TIME_ZONE"))
    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_RESET_ACCOUNT_BALANCE") = "Y"
    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE_START As Double = -1 '-1 value means we don't already have a thread executing the calculate final result
    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_RESET_ACCOUNT_BALANCE_MAX_TIMEOUT")) 'timeout in seconds
    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE_HOUR As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_RESET_ACCOUNT_BALANCE_HOUR"))
    Public Shared EXECUTE_RESET_ACCOUNT_BALANCE_MINUTE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_RESET_ACCOUNT_BALANCE_MINUTE"))
    Public Shared RUN_ONCE_RESET_ACCOUNT_BALANCE As Boolean = False

    Public Shared EXECUTE_WEEKLY_CHARGE_TIME_ZONE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_WEEKLY_CHARGE_TIME_ZONE"))
    Public Shared EXECUTE_WEEKLY_CHARGE As Boolean = System.Configuration.ConfigurationManager.AppSettings("EXECUTE_WEEKLY_CHARGE") = "Y"
    Public Shared EXECUTE_WEEKLY_CHARGE_START As Double = -1 '-1 value means we don't already have a thread executing the calculate final result
    Public Shared EXECUTE_WEEKLY_CHARGE_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_WEEKLY_CHARGE_MAX_TIMEOUT")) 'timeout in seconds
    Public Shared EXECUTE_WEEKLY_CHARGE_HOUR As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_WEEKLY_CHARGE_HOUR"))
    Public Shared EXECUTE_WEEKLY_CHARGE_MINUTE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_WEEKLY_CHARGE_MINUTE"))
    Public Shared RUN_ONCE_WEEKLY_CHARGE As Boolean = False

    Public Shared EXECUTE_PINNACLE_PROPS As Boolean = SafeString(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_PINNACLE_PROPS")) = "Y"

    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE_TIME_ZONE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CASINO_ACCOUNT_BALANCE_TIME_ZONE"))
    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE As Boolean = SafeBoolean(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CASINO_ACCOUNT_BALANCE"))
    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE_START As Double = -1 '-1 value means we don't already have a thread executing the calculate final result
    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE_MAX_TIMEOUT As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CASINO_ACCOUNT_BALANCE_MAX_TIMEOUT")) 'timeout in seconds
    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE_HOUR As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CASINO_ACCOUNT_BALANCE_HOUR"))
    Public Shared EXECUTE_CASINO_ACCOUNT_BALANCE_MINUTE As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_CASINO_ACCOUNT_BALANCE_MINUTE"))
    Public Shared RUN_ONCE_CASINO_ACCOUNT_BALANCE As Boolean = False

    Public Shared EXECUTE_MANIPULATION_UPDATE As Boolean = SafeString(System.Configuration.ConfigurationManager.AppSettings("EXECUTE_MANIPULATION_UPDATE")) = "Y"

    Public Shared PINNACLE_FEED_URL As String = SafeString(System.Configuration.ConfigurationManager.AppSettings("PINNACLE_FEED_URL"))

    Public Shared Sub CatchSBCServiceError(ByVal poLog As log4net.ILog, Optional ByVal poError As Exception = Nothing)
        LogError(poLog, "Unexpected error with SBCService", poError)

        'Set BETTING_ENABLED = false 
        Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Try
            Dim oWhere As New WebsiteLibrary.DBUtils.CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Category = " & SQLString("BettingSetup"))
            oWhere.AppendANDCondition("SubCategory = " & SQLString("BettingEnable"))

            Dim oUpdate As New WebsiteLibrary.DBUtils.CSQLUpdateStringBuilder("SysSettings", oWhere.SQL)
            oUpdate.AppendString("[Value]", SQLString("False"))

            LogDebug(poLog, "Set BETTING_ENABLE is FALSE")
            oDB.executeNonQuery(oUpdate.SQL)


            'Clear Website's cache
            LogDebug(poLog, "Clear Website's cache")
            webPost(New List(Of DictionaryEntry), CLEAR_CACHE_BETTING_ENABLE_URL)

            'Send Email 
            LogDebug(poLog, "Send Email Alerts")
            Dim sTo As String = SMTP_TO
            Dim oBcc As New List(Of String)
            If SMTP_TO.Contains(";") AndAlso SMTP_TO.Split(";"c).Length > 1 Then
                sTo = SMTP_TO.Split(";"c)(0)
                For nIndex As Integer = 0 To SMTP_TO.Split(";"c).Length
                    oBcc.Add(SMTP_TO.Split(";"c)(nIndex))
                Next
            End If

            '' Get Local IP
            Dim oLocalIP As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName)
            Dim sLocalIP As String = oLocalIP.AddressList.GetValue(0).ToString

            Dim oMessage As New System.Net.Mail.MailMessage(SMTP_FROM, sTo, String.Format("[{0}] SBCService had a timeout error.", sLocalIP), _
                                                            String.Format("{0} - SBCService throw timeout error at ", sLocalIP) & _
                                                            Date.Now.ToUniversalTime & " GMT <br />" & _
                                                            SafeString(IIf(poError IsNot Nothing, poError.ToString, "")))

            If oBcc.Count > 0 Then
                For Each sAddress As String In oBcc
                    oMessage.Bcc.Add(sAddress)
                Next
            End If
            SendEmail(oMessage)

        Catch ex As Exception
            LogError(poLog, "Fail to Update BETTING_ENABLE to Fail", ex)
        Finally
            oDB.closeConnection()
        End Try
    End Sub

    Public Shared Function GamePeriodToHalf(ByVal psGameType As String, ByVal pnPeriod As Integer) As Integer
        'Basketball and Football 	
        Select Case psGameType
            Case "AFL Football", "CFL Football", "NCAA Football", "NFL Football", "NFL Preseason", _
            "NBA Basketball", "WNBA Basketball"

                Return SafeInteger(IIf(pnPeriod < 3, 1, 2))
                ''Baseball:
            Case "MLB AL Baseball", "MLB Baseball", "MLB NL Baseball", "NCAA Baseball"
                Return SafeInteger(IIf(pnPeriod < 6, 1, 2))
                '   Hockey
            Case "NHL Hockey", "NCAA Hockey"
                Return SafeInteger(IIf(pnPeriod < 2, 1, 2))
                'Soccer:
            Case "Argentina", "Brazil", "Bundesliga", "Euro Cups", "La Liga", "Ligue 1", "Mexican", _
            "MLS", "Netherlands", "Portugal", "Premier", "Scotland", "Serie A", "Soccer", "Super Liga", "World Cup", _
                "NCAA Basketball", "WNCAA Basketball", "Euro", "Champions League", "Europa League", "Copa America", _
                "Carling Cup", "FA Cup", "Concacaf"
                Return SafeInteger(pnPeriod)

            Case Else
                Return 1
        End Select
    End Function

    Public Shared Function VegasOdds(ByVal pnOdd As Double) As Double
        If pnOdd = 0 Then
            Return pnOdd
        End If

        Dim nLastDigit As Double = Math.Abs(pnOdd) Mod 10

        If nLastDigit < 3 Then
            If pnOdd > 0 Then
                pnOdd -= nLastDigit
            Else
                pnOdd += nLastDigit
            End If
        ElseIf nLastDigit > 7 Then
            If pnOdd > 0 Then
                pnOdd += 10 - nLastDigit
            Else
                pnOdd -= 10 - nLastDigit
            End If

        Else
            If pnOdd > 0 Then
                pnOdd += 5 - nLastDigit
            Else
                pnOdd -= 5 - nLastDigit
            End If

        End If

        If pnOdd = -100 Then
            pnOdd = 100
        End If

        Return pnOdd
    End Function

    Public Shared Function CrisToVegasOdds(ByVal pnOdd As Double, ByVal psGameType As String, ByVal psContext As String) As Double
        If pnOdd = 0 Then
            Return pnOdd
        End If

        If IsFootball(psGameType) OrElse IsBasketball(psGameType) Then
            If psContext = "Current" OrElse psContext = "1H" Then
                pnOdd += 5

                If pnOdd > 0 AndAlso pnOdd < 100 Then
                    pnOdd = pnOdd - 200
                End If
                If pnOdd < 0 AndAlso pnOdd > -100 Then
                    pnOdd = pnOdd + 200
                End If
            End If
        End If

        Return pnOdd
    End Function

    Public Shared Function Reduce5Percentage(ByVal pnJuice As Double) As Double
        If pnJuice = 0 Then
            Return pnJuice
        End If

        pnJuice -= 5

        If pnJuice > 0 AndAlso pnJuice < 100 Then
            pnJuice = pnJuice - 200
        End If
        If pnJuice < 0 AndAlso pnJuice > -100 Then
            pnJuice = pnJuice + 200
        End If

        Return pnJuice
    End Function
End Class
