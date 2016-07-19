Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CTicketBetManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CTicketBetManager))

        Public Function GetTicketBets(ByVal psTicketID As String) As DataTable
            Dim odtTicketBets As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("tb.TicketID=" & SQLString(psTicketID))
            
            Dim sSQL As String = "SELECT tb.*, g.* FROM TicketBets tb " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & oWhere.SQL

            log.Debug("Get the list of ticket bets. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTicketBets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of ticket bets. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTicketBets
        End Function

    End Class

End Namespace
