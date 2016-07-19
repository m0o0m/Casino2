Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports WebsiteLibrary.DBUtils
Imports System.Data
Namespace Managers
    Public Class CPlayerJuiceManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function InsertPlayerJuiceSetup(ByVal oPlayerJuice As CPlayerJuice) As Boolean

            Dim bSuccess As Boolean = True

            Dim oInsert As New CSQLInsertStringBuilder("PlayerJuiceManagements")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oInsert
                    .AppendString("PlayerJuiceManagementID", SQLString(newGUID))
                    .AppendString("PlayerTemplateID", SQLString(oPlayerJuice.playerTemplateID))
                    .AppendString("SportType", SQLString(oPlayerJuice.SportType))
                    .AppendString("JuiceType", SQLString(oPlayerJuice.JuiceType))
                    .AppendString("SpreadSetup", SQLString(oPlayerJuice.SpreadSetup))
                    .AppendString("[1HSpreadSetup]", SQLString(oPlayerJuice._1stHSpreadSetup))
                    .AppendString("[2HSpreadSetup]", SQLString(oPlayerJuice._2ndHSPreadSetup))
                    .AppendString("TotalPointSetup", SQLString(oPlayerJuice.TotalPointSetup))
                    .AppendString("[1HTotalPointSetup]", SQLString(oPlayerJuice._1stHTotalPointSetup))
                    .AppendString("[2HTotalPointSetup]", SQLString(oPlayerJuice._2ndHTotalPointSetup))
                    .AppendString("MLineSetup", SQLString(oPlayerJuice.MLineSetup))
                    .AppendString("[1HMLineSetup]", SQLString(oPlayerJuice._1stHMLineSetup))
                    .AppendString("[2HMLineSetup]", SQLString(oPlayerJuice._2ndMLineSetup))
                    .AppendString("TeamTotalPointSetup", SQLString(oPlayerJuice.TeamTotalPointSetup))
                End With
                log.Debug("Insert player Juice setup. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert.SQL)

            Catch ex As Exception
                bSuccess = False
                LogError(log, "Error in Insert PlayerJuice Setup", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdatePlayerJuiceSetup(ByVal psPlayerJuiceManagementID As String, ByVal poPleyerJuice As CPlayerJuice) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PlayerJuiceManagementID=" & SQLString(psPlayerJuiceManagementID))
            Dim oUpdate As New CSQLUpdateStringBuilder("PlayerJuiceManagements", oWhere.SQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("SpreadSetup", SQLString(poPleyerJuice.SpreadSetup))
                    .AppendString("[1HSpreadSetup]", SQLString(poPleyerJuice._1stHSpreadSetup))
                    .AppendString("[2HSpreadSetup]", SQLString(poPleyerJuice._2ndHSPreadSetup))
                    .AppendString("TotalPointSetup", SQLString(poPleyerJuice.TotalPointSetup))
                    .AppendString("[1HTotalPointSetup]", SQLString(poPleyerJuice._1stHTotalPointSetup))
                    .AppendString("[2HTotalPointSetup]", SQLString(poPleyerJuice._2ndHTotalPointSetup))
                    .AppendString("MLineSetup", SQLString(poPleyerJuice.MLineSetup))
                    .AppendString("[1HMLineSetup]", SQLString(poPleyerJuice._1stHMLineSetup))
                    .AppendString("[2HMLineSetup]", SQLString(poPleyerJuice._2ndMLineSetup))
                    .AppendString("TeamTotalPointSetup", SQLString(poPleyerJuice.TeamTotalPointSetup))
                End With
                log.Debug("Update player Juice setup. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate.SQL)

            Catch ex As Exception
                LogError(log, "Cannot update Player juice setup", ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function GetPlayerJuice(ByVal psPlayertemplateID As String, ByVal psSportType As String) As CPlayerJuiceList
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("SportType=" & SQLString(psSportType))
                oWhere.AppendANDCondition("PlayerTemplateID=" & SQLString(psPlayertemplateID))
                Dim sSQL = "SELECT * FROM PlayerJuiceManagements" & oWhere.SQL
                Dim oDTPlayerJuice As DataTable = odbSQL.getDataTable(sSQL)

                Dim oPlayerJuiceList As New CPlayerJuiceList
                For Each oRow As DataRow In oDTPlayerJuice.Rows
                    oPlayerJuiceList.Add(New CPlayerJuice(oRow))
                Next
                Return oPlayerJuiceList
            Catch ex As Exception
                LogError(log, "GetPlayerJuice b templateID and sportType ", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetPlayerJuice(ByVal psPlayerTemplateID As String) As CPlayerJuiceList
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim oWhere As New CSQLWhereStringBuilder
                ' oWhere.AppendANDCondition("a.PlayerTemplateID=b.PlayerTemplateID")
                oWhere.AppendANDCondition("a.PlayerTemplateID=" & SQLString(psPlayerTemplateID))
                Dim sSQL = "SELECT * FROM PlayerJuiceManagements a " & _
                "INNER JOIN PlayerTemplates b ON a.PlayerTemplateID=b.PlayerTemplateID " & oWhere.SQL
                Dim oDTPlayerJuice As DataTable = odbSQL.getDataTable(sSQL)
                Dim oPlayerJuiceList As New CPlayerJuiceList
                For Each oRow As DataRow In oDTPlayerJuice.Rows
                    oPlayerJuiceList.Add(New CPlayerJuice(oRow))
                Next
                Return oPlayerJuiceList
            Catch ex As Exception
                LogError(log, "GetPlayerJuice by templateID", ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function
    End Class
End Namespace
