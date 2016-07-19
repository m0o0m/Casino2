Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CWeeklyChargeManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetWeeklyChargeByAgentID(ByVal psSuperAdminID As String, ByVal psAgentID As String, ByVal poStartDate As Date) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("c.SuperAdminID = " & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition("c.AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition(getSQLDateRange("c.ChargeDate", SafeString(poStartDate), SafeString(poStartDate.AddDays(6))))

            Dim sSQL As String = "SELECT a.Login + ' (' + a.Name + ')' AS FullName, c.* FROM WeeklyCharges c LEFT JOIN Agents a ON c.AgentID = a.AgentID " & _
            oWhere.SQL & " ORDER BY a.Login "

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                log.Debug("Get Weekly Charge By SuperAdmin. SQL: " & sSQL)
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Weekly Charge By SuperAdmin. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetWeeklyCharge(ByVal psSuperAdminID As String, ByVal poStartDate As Date) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("c.SuperAdminID = " & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition(getSQLDateRange("c.ChargeDate", SafeString(poStartDate), SafeString(poStartDate.AddDays(6))))

            Dim sSQL As String = "SELECT a.Login + ' (' + a.Name + ')' AS FullName, c.* FROM WeeklyCharges c LEFT JOIN Agents a ON c.AgentID = a.AgentID " & _
            oWhere.SQL & " ORDER BY a.Login "

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                log.Debug("Get Weekly Charge By SuperAdmin. SQL: " & sSQL)
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Weekly Charge By SuperAdmin. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetWeeklyChargeByAgentID(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("c.AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition(getSQLDateRange("c.ChargeDate", SafeString(poStartDate), SafeString(poEndDate)))

            Dim sSQL As String = "SELECT a.Login + ' (' + a.Name + ')' AS FullName, c.* FROM WeeklyCharges c LEFT JOIN Agents a ON c.AgentID = a.AgentID " & _
            oWhere.SQL & " ORDER BY a.Login "

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                log.Debug("Get Weekly Charge By SuperAdmin. SQL: " & sSQL)
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Weekly Charge By SuperAdmin. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function AddWeeklyCharge(ByVal psSuperAdminID As String, ByVal psAgentID As String, _
                                        ByVal pnActivePlayers As Integer, ByVal pnChargeAmount As Double, _
                                        ByVal psDetails As String, ByVal poChargeDate As Date, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = ""
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                Dim oInsert As New CSQLInsertStringBuilder("WeeklyCharges")
                oInsert.AppendString("WeeklyChargeID", "NEWID()")
                oInsert.AppendString("ChargeDate", SQLDate(poChargeDate))

                oInsert.AppendString("SuperAdminID", SQLString(psSuperAdminID))
                oInsert.AppendString("AgentID", SQLString(psAgentID))
                oInsert.AppendString("ActivePlayers", SQLDouble(pnActivePlayers))
                oInsert.AppendString("ChargeAmount", SQLDouble(pnChargeAmount))
                oInsert.AppendString("Description", SQLString(psDetails))

                sSQL = oInsert.SQL
                log.Debug("Add Weekly Charge. SQL: " & sSQL)
                oDB.executeNonQuery(oInsert, psChangedBy)

                bSuccess = True
            Catch ex As Exception
                log.Error("Fails to Add Weekly Charge. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateWeeklyCharge(ByVal psSuperAdminID As String, ByVal psAgentID As String, _
                                     ByVal pnActivePlayers As Integer, ByVal pnChargeAmount As Double, _
                                     ByVal psDetails As String, ByVal psChangedBy As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As Boolean
            Dim bSuccess As Boolean = False
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = ""

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SuperAdminID=" & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition("AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition(getSQLDateRange("ChargeDate", SafeString(poStartDate), SafeString(poEndDate)))
            Dim oUpdate As New CSQLUpdateStringBuilder("WeeklyCharges", oWhere.SQL)

            Try
                With oUpdate
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    .AppendString("ChargeDate", SQLDate(poEndDate))
                    .AppendString("ActivePlayers", SQLDouble(pnActivePlayers))
                    .AppendString("ChargeAmount", SQLDouble(pnChargeAmount))
                    .AppendString("Description", SQLString(psDetails))
                    sSQL = oUpdate.SQL
                    log.Debug("Update Weekly Charge. SQL: " & sSQL)
                    oDB.executeNonQuery(oUpdate, psChangedBy)

                    bSuccess = True
                End With
            Catch ex As Exception
                log.Error("Fails to Update Weekly Charge. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateWeeklyChargePaidDate(ByVal psSuperAdminID As String, ByVal psAgentID As String, _
                                     ByVal psWeeklyChargeID As String, ByVal poPaidDate As Date) As Boolean
            Dim bSuccess As Boolean = False
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = ""

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SuperAdminID=" & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition("AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("WeeklyChargeID=" & SQLString(psWeeklyChargeID))
            Dim oUpdate As New CSQLUpdateStringBuilder("WeeklyCharges", oWhere.SQL)

            Try
                With oUpdate
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

                    .AppendString("PaidDate", SQLDate(poPaidDate))
                    sSQL = oUpdate.SQL
                    log.Debug("Update Weekly Charge Paid Date. SQL: " & sSQL)
                    oDB.executeNonQuery(oUpdate, psSuperAdminID)

                    bSuccess = True
                End With
            Catch ex As Exception
                log.Error("Fails to Update Weekly Charge Paid Date. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function GetTotalWeeklyUnpaidChargeByAgent(ByVal psAgentID As String) As Double
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition("PaidDate Is NULL")

            Dim sSQL As String = "SELECT SUM(ChargeAmount) FROM WeeklyCharges " & oWhere.SQL

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                log.Debug("Get Total Weekly Unpaid Charge By Agent. SQL: " & sSQL)
                Return SafeDouble(oDB.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Fails to Get Total Weekly Unpaid Charge By Agent. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return 0
        End Function

    End Class
End Namespace