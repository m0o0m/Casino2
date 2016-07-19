Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CAsteriaManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetCallDetails(ByVal psUserExtension As String, ByVal poSDate As DateTime, ByVal poEDate As DateTime) As DataTable
            Dim oData As DataTable = Nothing
            Dim oDB As CPostgreSQLUtils = Nothing

            Dim sSql As String = ""
            Try
                ' oDB = New CPostgreSQLUtils(ASTERIA_CONNECTION_STRING)
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("asterisk_cdr.userfield LIKE '%Audio%'")
                oWhere.AppendANDCondition("disposition='ANSWERED'")
                oWhere.AppendOptionalANDCondition("username", SQLString(psUserExtension), "=")
                oWhere.AppendANDCondition(getSQLDateRange("calldate", SafeString(poSDate), SafeString(poEDate), True))

                sSql = "SELECT asterisk_cdr.*, recorded_calls.*, users.username " & vbCrLf & _
                                     " FROM asterisk_cdr INNER JOIN recorded_calls ON asterisk_cdr.uniqueid=recorded_calls.asterisk_id " & vbCrLf & _
                                     " INNER JOIN users ON recorded_calls.user=users.id " & oWhere.SQL & _
                                     " ORDER BY calldate DESC;"

                LogDebug(log, "Get call details. SQL: " & sSql)

                oData = oDB.GetDataTable(sSql)

            Catch ex As Exception
                LogError(log, "Fail to get call details. SQL:" & sSql, ex)
            Finally
                If oDB IsNot Nothing Then oDB.CloseConnection()
            End Try

            Return oData
        End Function
    End Class
End Namespace