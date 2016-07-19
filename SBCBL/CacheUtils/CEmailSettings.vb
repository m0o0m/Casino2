Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace CacheUtils

    Public Class CEmailSettings

        Public ServerName As String = ""
        Public IncomingMailServer As String = ""
        Public OutgoingMailServer As String = ""
        Public OutgoingMailRequiresAuthentication As Boolean = False
        Public RequiresSSL As Boolean = False
        Public EmailLogin As String = ""
        Public EmailPassword As String = ""
        Public EmailServerID As String = ""

        Public Sub New(ByVal psEmailLogin As String, ByVal psEmailPassword As String)
            EmailLogin = psEmailLogin
            EmailPassword = psEmailPassword
        End Sub

        Private Sub loadData(ByVal poDR As DataRow)
            ServerName = SafeString(poDR("ServerName"))
            IncomingMailServer = SafeString(poDR("IncomingMailServer"))
            OutgoingMailServer = SafeString(poDR("OutgoingMailServer"))
            OutgoingMailRequiresAuthentication = SafeString(poDR("OutgoingMailRequiresAuthentication")) = "Y"
            RequiresSSL = SafeString(poDR("RequiresSSL")) = "Y"
        End Sub

    End Class

End Namespace