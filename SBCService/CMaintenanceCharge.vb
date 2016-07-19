Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils

Public Class CMaintenanceCharge
    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())


    Public Sub WeeklyCharge(ByVal poMonday As Date)
        Dim oEndDate As Date = SafeDate(poMonday.ToShortDateString)

        '' GetSuperAgent First
        Dim dtParents As DataTable = GetSuperAgent()

        If dtParents IsNot Nothing Then
            Dim nWeeklyMaintenance As Double
            Dim nActivePlayers As Integer = 0
            Dim oTblActivePlayers, oTblWeekly As DataTable
            Dim sDetails As String = ""

            Dim oAgentManager As New CAgentManager()
            Dim oWeeklyChargeManager As New CWeeklyChargeManager()

            ' Exec Weekly Maintenance for each super agent
            For Each oDR As DataRow In dtParents.Rows
                nWeeklyMaintenance = SafeDouble(oDR("WeeklyCharge"))
                nActivePlayers = 0
                sDetails = ""

                If nWeeklyMaintenance <> 0 Then
                    '' Get Total Active Players
                    oTblActivePlayers = oAgentManager.GetActivePlayers(SafeString(oDR("AgentID")), oEndDate.AddDays(-7), _
                                                               oEndDate.AddDays(-1))
                    If oTblActivePlayers IsNot Nothing Then
                        nActivePlayers = oTblActivePlayers.Rows.Count
                        sDetails = GetDetails(oTblActivePlayers)
                    End If
                    nWeeklyMaintenance = nWeeklyMaintenance * nActivePlayers

                    ' Check Exist Weekly Maintenance
                    oTblWeekly = oWeeklyChargeManager.GetWeeklyChargeByAgentID(SafeString(oDR("AgentID")), oEndDate.AddDays(-7), _
                                                                            oEndDate.AddDays(-1))

                    ' Perform charge on active players
                    If oTblWeekly Is Nothing OrElse oTblWeekly.Rows.Count = 0 Then
                        oWeeklyChargeManager.AddWeeklyCharge(SafeString(oDR("SuperAdminID")), SafeString(oDR("AgentID")), _
                                                        nActivePlayers, nWeeklyMaintenance, sDetails, oEndDate.AddDays(-1), "")
                    Else
                        oWeeklyChargeManager.UpdateWeeklyCharge(SafeString(oDR("SuperAdminID")), SafeString(oDR("AgentID")), _
                                                     nActivePlayers, nWeeklyMaintenance, sDetails, "", oEndDate.AddDays(-7), _
                                                     oEndDate.AddDays(-1))
                    End If
                End If
            Next
        End If

    End Sub

    Private Function GetSuperAgent() As DataTable
        Dim otblAgent As DataTable = New DataTable()
        Dim oWhere As New CSQLWhereStringBuilder
        oWhere.AppendANDCondition("SuperAdminID IS NOT NULL ")
        Dim sSQL As String = "SELECT * FROM Agents " & vbCrLf & oWhere.SQL & " ORDER BY Login "

        log.Debug("Get All Super Agent. SQL: " & sSQL)

        Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
        Try
            otblAgent = odbSQL.getDataTable(sSQL)
        Catch ex As Exception
            log.Error("Cannot get All Super Agent. SQL: " & sSQL, ex)
        Finally
            odbSQL.closeConnection()
        End Try

        Return otblAgent
    End Function

    Private Function GetDetails(ByVal poTblActivePlayers As DataTable) As String
        Dim sResult As String = ""
        If poTblActivePlayers IsNot Nothing Then
            sResult = "<table width='100%'><tr><td class='tableheading' style='text-align: center;height:12px'>Agent Login (Name)</td><td class='tableheading' style='text-align: center;'>Player Login (Name)</td></tr>"

            Dim sAgentName As String = ""
            For Each oDR As DataRow In poTblActivePlayers.Rows
                If sAgentName <> SafeString(oDR("AgentFullName")) Then
                    sAgentName = SafeString(oDR("AgentFullName"))
                    sResult &= String.Format("<tr><td style='text-align: left;'>{0}</td><td>{1}</td></tr>", _
                                        sAgentName, SafeString(oDR("PlayerFullName")))
                Else
                    sResult &= String.Format("<tr><td style='text-align: left;'>{0}</td><td>{1}</td></tr>", _
                                        "", SafeString(oDR("PlayerFullName")))
                End If
            Next

            sResult &= "</table>"
        End If

        Return sResult
    End Function

End Class
