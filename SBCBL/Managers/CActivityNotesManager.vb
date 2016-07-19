Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CActivityNotesManager
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function AddNote(ByVal poNote As CacheUtils.CActivityNote) As Boolean
            Dim bResult As Boolean = False
            Dim oInsert As New WebsiteLibrary.DBUtils.CSQLInsertStringBuilder("ActivityNotes")
            oInsert.AppendString("ActivityNoteID", SQLString(newGUID()))
            oInsert.AppendString("CreatedDate", SQLString(poNote.CreateDate))
            oInsert.AppendString("UserID", SQLString(poNote.UserID))
            oInsert.AppendString("FullName", SQLString(poNote.FullName))
            If poNote.TicketID IsNot Nothing Then
                oInsert.AppendString("TicketID", SQLString(poNote.TicketID))
            End If
            oInsert.AppendString("Note", SQLString(poNote.Note))
            oInsert.AppendString("NoteType", SQLString(poNote.NoteType))
            Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oInsert.SQL)
                bResult = True
            Catch ex As Exception
                _log.Error("Cant insert new activity log: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return bResult
        End Function

        Public Function GetAgentNotes(ByVal psCCAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date, ByVal psSiteType As String, Optional ByVal psPlayerLogin As String = "") As DataTable
            Dim oDT As New DataTable
            Dim oWhere As New WebsiteLibrary.DBUtils.CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("NoteType =" & SQLString("CCAgent"))
            oWhere.AppendANDCondition("ActivityNotes.CreatedDate >=" & SQLString(poStartDate))
            oWhere.AppendANDCondition("ActivityNotes.CreatedDate <=" & SQLString(poEndDate.AddDays(1)))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            If psPlayerLogin <> "" Then
                oWhere.AppendANDCondition("Players.Login = " & SQLString(psPlayerLogin))
            Else
                oWhere.AppendOptionalANDCondition("UserID", SQLString(psCCAgentID), "=")
            End If


            Dim sSQL As String = "select ActivityNotes.*, Players.Login as PlayerName, Tickets.TicketNumber from ActivityNotes inner join Tickets on ActivityNotes.TicketID = Tickets.TicketID " & _
                                 " inner join Players on Players.PlayerID= Tickets.PlayerID " & oWhere.SQL & _
                                 " order by Players.Login, ActivityNotes.CreatedDate desc"
            Dim oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                _log.Debug("Get Agent Notes: " & sSQL)
                oDT = oDB.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Can't select Agent Notes: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return oDT

        End Function
    End Class
End Namespace