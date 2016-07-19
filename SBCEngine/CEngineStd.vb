Imports WebsiteLibrary.CSBCStd

Public Class CEngineStd

    Public Shared STATUS_OPEN As String = UCase(System.Configuration.ConfigurationManager.AppSettings("STATUS_OPEN"))
    Public Shared STATUS_WIN As String = UCase(System.Configuration.ConfigurationManager.AppSettings("STATUS_WIN"))
    Public Shared STATUS_LOSE As String = UCase(System.Configuration.ConfigurationManager.AppSettings("STATUS_LOSE"))
    Public Shared STATUS_CANCELLED As String = UCase(System.Configuration.ConfigurationManager.AppSettings("STATUS_CANCELLED"))
    Public Shared STATUS_PENDING As String = UCase(System.Configuration.ConfigurationManager.AppSettings("STATUS_PENDING"))
    Public Shared SOCCER_GAMES As String = UCase(System.Configuration.ConfigurationManager.AppSettings("SOCCER_GAMES"))
    Public Shared BASEBALL_GAMES As String = UCase(System.Configuration.ConfigurationManager.AppSettings("BASEBALL_GAMES"))
    Public Shared FOOTBALL_GAMES As String = UCase(System.Configuration.ConfigurationManager.AppSettings("FOOTBALL_GAMES"))
    Public Shared BASKETBALL_GAMES As String = UCase(System.Configuration.ConfigurationManager.AppSettings("BASKETBALL_GAMES"))

    Public Shared MINUTES_GAME_SUSPEND_PROCESSED As Integer = SafeInteger(System.Configuration.ConfigurationManager.AppSettings("MINUTES_GAME_SUSPEND_PROCESSED"))

    Public Shared Function IsContains(ByVal psSource As String, ByVal ParamArray poValues() As String) As Boolean
        For Each sValue As String In poValues
            If psSource.Contains(sValue) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Shared Function GetCurrentEasternDate() As Date
        Dim oCurrent As DateTime = Now.ToUniversalTime()

        If Now.IsDaylightSavingTime() Then
            oCurrent = oCurrent.AddHours(1)
        End If

        Return oCurrent.AddHours(-5)
    End Function

End Class

Public Enum CheckGameMode
    Suspend
    FirstQuater
    SecondQuater
    ThirdQuater
    FirstHalf
    Final 'Second Halft, Four Quater, Final
    Proposition
End Enum
