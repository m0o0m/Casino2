Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CAgentGameSettingManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function CheckExistCircle(ByVal poGameID As String, ByVal poAgentID As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(poGameID))
            oWhere.AppendANDCondition("AgentID=" & SQLString(poAgentID))
            Dim sSQL As String = String.Format("SELECT * FROM AgentGameSettings {0}", oWhere.SQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim oDT As New DataTable
            Try
                oDT = odbSQL.getDataTable(sSQL)
                Return oDT.Rows.Count > 0
            Catch ex As Exception
                LogError(log, "Error in check Circle", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return False
        End Function

        Public Function InsertGameCircle(ByVal poGameID As String, ByVal poAgentID As String, ByVal pbGameCircle As Boolean) As Boolean
            Dim bSuccess As Boolean = False

            Dim poAgentGameSetting As String = newGUID()
            Dim sSQL As New CSQLInsertStringBuilder("AgentGameSettings")
            sSQL.AppendString("AgentGameSettingID", SQLString(poAgentGameSetting))
            sSQL.AppendString("GameID", SQLString(poGameID))
            sSQL.AppendString("AgentID", SQLString(poAgentID))
            sSQL.AppendString("IsGameCircle", SQLString(IIf(pbGameCircle = True, "Y", "N")))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                If odbSQL.executeNonQuery(sSQL, "") > 0 Then
                    bSuccess = True
                End If
            Catch ex As Exception
                LogError(log, "Cannot save Game Circle", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateGameCircle(ByVal poGameID As String, ByVal poAgentID As String, ByVal pbGameCircle As Boolean) As Boolean
            Dim bSuccess As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("GameID=" & SQLString(poGameID))
            oWhere.AppendANDCondition("AgentID=" & SQLString(poAgentID))

            Dim sSQL As New CSQLUpdateStringBuilder("AgentGameSettings", oWhere.SQL)
            sSQL.AppendString("IsGameCircle", SQLString(IIf(pbGameCircle = True, "Y", "N")))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                If odbSQL.executeNonQuery(sSQL, "") > 0 Then
                    bSuccess = True
                End If
            Catch ex As Exception
                LogError(log, "Cannot save Game Circle", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function
    End Class
End Namespace