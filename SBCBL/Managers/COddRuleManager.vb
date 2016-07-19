Imports SBCBL.std
Imports WebsiteLibrary.DBUtils
Imports System.Data

Namespace Managers
    Public Class COddRuleManager
        Private _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function InsertOddRule(ByVal psAgentID As String, ByVal pnLowerThan As Integer, ByVal pnGreaterThan As Integer, ByVal pnIncrease As Integer, ByVal psUserID As String, Optional ByVal psIslock As String = "N") As Boolean

            Dim bSuccess As Boolean = True
            Dim oInsert As ISQLEditStringBuilder = New CSQLInsertStringBuilder("OddRules")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oInsert
                    .AppendString("OddRuleID", SQLString(newGUID))
                    .AppendString("AgentID", SQLString(psAgentID))
                    .AppendString("LowerThan", SQLString(pnLowerThan))
                    .AppendString("GreaterThan", SQLString(pnGreaterThan))
                    .AppendString("Increase", SQLString(pnIncrease))
                    .AppendString("Islock", SQLString(psIslock))
                End With

                _log.Debug("Insert OddRule. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psUserID)

            Catch ex As Exception
                bSuccess = False
                _log.Error("Error trying to insert OddRule. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateOddRuleByID(ByVal psOddRuleID As String, ByVal pnLowerThan As Integer, ByVal pnGreaterThan As Integer, ByVal pnIncrease As Integer, ByVal psChangedBy As String, Optional ByVal psIslock As String = "N") As Boolean
            Dim bSuccess As Boolean = False
            Dim oUpdate As New CSQLUpdateStringBuilder("OddRules", "WHERE OddRuleID= " & SQLString(psOddRuleID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("LowerThan", SQLString(pnLowerThan))
                    .AppendString("GreaterThan", SQLString(pnGreaterThan))
                    .AppendString("Increase", SQLString(pnIncrease))
                    .AppendString("Islock", SQLString(psIslock))
                End With
                _log.Debug("Update OddRule. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                bSuccess = True
            Catch ex As Exception
                _log.Error("Error trying to update OddRule . SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function DeleteOddRule(ByVal psOddRuleID As String, ByVal psDeleteBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim sSQL As String = ""
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("OddRuleID = " & SQLString(psOddRuleID))
                Dim oDelete As New CSQLDeleteStringBuilder("OddRules", oWhere.SQL)
                sSQL = oDelete.SQL

                odbSQL.executeNonQuery(oDelete, psDeleteBy)

                bResult = True
            Catch ex As Exception
                _log.Error("Error OddRule trying to exec SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function GetOddRulesByOddRuleID(ByVal psOddRuleID As String) As DataTable
            Dim odtOddRules As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("OddRuleID=" & SQLString(psOddRuleID))
            Dim sSQL As String = "SELECT Distinct * from OddRules" & oWhere.SQL
            _log.Debug("Get the list of OddRules. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtOddRules = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Subject OddRules by psOddRuleID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtOddRules
        End Function

        Public Function GetOddRulesByAgentID(ByVal psAgentID As String) As DataTable
            Dim odtOddRules As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID=" & SQLString(psAgentID))
            Dim sSQL As String = "SELECT Distinct * from OddRules" & oWhere.SQL
            _log.Debug("Get the list of OddRules. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtOddRules = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Subject OddRules by AgentID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtOddRules
        End Function
    End Class
End Namespace
