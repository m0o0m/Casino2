Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CPlayerTemplateManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CPlayerTemplateManager))

#Region "Insert, Update, Delete methods"

        Public Function InsertPlayerTemplate(ByVal poPlayerTemplate As CPlayerTemplate, ByVal psSiteType As String, Optional ByVal psIsDefaultPlayerTemplate As String = "Y") As Boolean
            Dim bSuccess As Boolean = True

            Dim oInsert As New CSQLInsertStringBuilder("PlayerTemplates")
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oInsert.AppendString("PlayerTemplateID", SQLString(poPlayerTemplate.PlayerTemplateID))
                oInsert.AppendString("SuperAdminID", SQLString(poPlayerTemplate.SuperAdminID))
                oInsert.AppendString("TemplateName", SQLString(poPlayerTemplate.TemplateName))
                oInsert.AppendString("LastSavedBy", SQLString(poPlayerTemplate.LastSaveBy))
                oInsert.AppendString("LastSavedDate", SQLString(poPlayerTemplate.LastSavedDate))
                oInsert.AppendString("AccountBalance", SQLString(poPlayerTemplate.AccountBalance))
                oInsert.AppendString("CreditMaxAmount", SQLString(poPlayerTemplate.CreditMaxAmount))
                oInsert.AppendString("CasinoMaxAmount", SQLString(poPlayerTemplate.CasinoMaxAmount))
                oInsert.AppendString("CreditMinBetPhone", SQLString(poPlayerTemplate.CreditMinBetPhone))
                oInsert.AppendString("CreditMinBetInternet", SQLString(poPlayerTemplate.CreditMinBetInternet))
                oInsert.AppendString("CreditWagerPerGame", SQLString(poPlayerTemplate.CreditWagerPerGame))
                oInsert.AppendString("CreditMaxParlay", SQLString(poPlayerTemplate.CreditMaxParlay))
                oInsert.AppendString("CreditMaxTeaserParlay", SQLString(poPlayerTemplate.CreditMaxTeaserParlay))
                oInsert.AppendString("CreditMaxReverseActionParlay", SQLString(poPlayerTemplate.CreditMaxReverseActionParlay))
                oInsert.AppendString("SiteType", SQLString(psSiteType))
                oInsert.AppendString("IsDefaultPlayerTemplate", SQLString(psIsDefaultPlayerTemplate))
                oInsert.AppendString("Max1Q", SQLString(poPlayerTemplate.Max1Q))
                oInsert.AppendString("Max2Q", SQLString(poPlayerTemplate.Max2Q))
                oInsert.AppendString("Max3Q", SQLString(poPlayerTemplate.Max3Q))
                oInsert.AppendString("Max4Q", SQLString(poPlayerTemplate.Max4Q))
                oInsert.AppendString("Max1H", SQLString(poPlayerTemplate.Max1H))
                oInsert.AppendString("Max2H", SQLString(poPlayerTemplate.Max2H))
                oInsert.AppendString("MaxSingle", SQLString(poPlayerTemplate.MaxSingle))

                oDB.executeNonQuery(oInsert.SQL)
                LogInfo(log, "InsertPlayerTemplate: " + oInsert.SQL)
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to insert PlayerTemplate. SQL: " & oInsert.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerTemplate(ByVal poPlayerTemplate As CPlayerTemplate, Optional ByVal psIsDefaultPlayerTemplate As String = "Y") As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("PlayerTemplates", String.Format("WHERE PlayerTemplateID={0} and IsDefaultPlayerTemplate={1} ", SQLString(poPlayerTemplate.PlayerTemplateID), SQLString(psIsDefaultPlayerTemplate)))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oUpdate.AppendString("TemplateName", SQLString(poPlayerTemplate.TemplateName))
                oUpdate.AppendString("LastSavedBy", SQLString(poPlayerTemplate.LastSaveBy))
                oUpdate.AppendString("LastSavedDate", SQLString(poPlayerTemplate.LastSavedDate))
                oUpdate.AppendString("AccountBalance", SQLString(poPlayerTemplate.AccountBalance))
                oUpdate.AppendString("CreditMaxAmount", SQLString(poPlayerTemplate.CreditMaxAmount))
                oUpdate.AppendString("CasinoMaxAmount", SQLString(poPlayerTemplate.CasinoMaxAmount))
                oUpdate.AppendString("CreditMinBetPhone", SQLString(poPlayerTemplate.CreditMinBetPhone))
                oUpdate.AppendString("CreditMinBetInternet", SQLString(poPlayerTemplate.CreditMinBetInternet))
                oUpdate.AppendString("CreditWagerPerGame", SQLString(poPlayerTemplate.CreditWagerPerGame))
                oUpdate.AppendString("CreditMaxParlay", SQLString(poPlayerTemplate.CreditMaxParlay))
                oUpdate.AppendString("CreditMaxTeaserParlay", SQLString(poPlayerTemplate.CreditMaxTeaserParlay))
                oUpdate.AppendString("CreditMaxReverseActionParlay", SQLString(poPlayerTemplate.CreditMaxReverseActionParlay))
                oUpdate.AppendString("IsDefaultPlayerTemplate", SQLString(psIsDefaultPlayerTemplate))
                oUpdate.AppendString("Max1Q", SQLString(poPlayerTemplate.Max1Q))
                oUpdate.AppendString("Max2Q", SQLString(poPlayerTemplate.Max2Q))
                oUpdate.AppendString("Max3Q", SQLString(poPlayerTemplate.Max3Q))
                oUpdate.AppendString("Max4Q", SQLString(poPlayerTemplate.Max4Q))
                oUpdate.AppendString("Max1H", SQLString(poPlayerTemplate.Max1H))
                oUpdate.AppendString("Max2H", SQLString(poPlayerTemplate.Max2H))
                oUpdate.AppendString("MaxSingle", SQLString(poPlayerTemplate.MaxSingle))

                oDB.executeNonQuery(oUpdate.SQL)
                LogInfo(log, "updatePlayerTemplate: " + oUpdate.SQL)
            Catch ex As Exception
                log.Error("Error trying to update PlayerTemplate. SQL: " & oUpdate.SQL, ex)
                bSuccess = False
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerTemplate2(ByVal poPlayerTemplate As CPlayerTemplate, Optional ByVal psIsDefaultPlayerTemplate As String = "Y") As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("PlayerTemplates", String.Format("WHERE PlayerTemplateID={0}", SQLString(poPlayerTemplate.PlayerTemplateID)))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oUpdate.AppendString("TemplateName", SQLString(poPlayerTemplate.TemplateName))
                oUpdate.AppendString("LastSavedBy", SQLString(poPlayerTemplate.LastSaveBy))
                oUpdate.AppendString("LastSavedDate", SQLString(poPlayerTemplate.LastSavedDate))
                oUpdate.AppendString("AccountBalance", SQLString(poPlayerTemplate.AccountBalance))
                oUpdate.AppendString("CreditMaxAmount", SQLString(poPlayerTemplate.CreditMaxAmount))
                oUpdate.AppendString("CasinoMaxAmount", SQLString(poPlayerTemplate.CasinoMaxAmount))
                oUpdate.AppendString("CreditMinBetPhone", SQLString(poPlayerTemplate.CreditMinBetPhone))
                oUpdate.AppendString("CreditMinBetInternet", SQLString(poPlayerTemplate.CreditMinBetInternet))
                oUpdate.AppendString("CreditWagerPerGame", SQLString(poPlayerTemplate.CreditWagerPerGame))
                oUpdate.AppendString("CreditMaxParlay", SQLString(poPlayerTemplate.CreditMaxParlay))
                oUpdate.AppendString("CreditMaxTeaserParlay", SQLString(poPlayerTemplate.CreditMaxTeaserParlay))
                oUpdate.AppendString("CreditMaxReverseActionParlay", SQLString(poPlayerTemplate.CreditMaxReverseActionParlay))
                oUpdate.AppendString("IsDefaultPlayerTemplate", SQLString(psIsDefaultPlayerTemplate))
                'oUpdate.AppendString("Max1Q", SQLString(poPlayerTemplate.Max1Q))
                'oUpdate.AppendString("Max2Q", SQLString(poPlayerTemplate.Max2Q))
                'oUpdate.AppendString("Max3Q", SQLString(poPlayerTemplate.Max3Q))
                'oUpdate.AppendString("Max4Q", SQLString(poPlayerTemplate.Max4Q))
                'oUpdate.AppendString("Max1H", SQLString(poPlayerTemplate.Max1H))
                'oUpdate.AppendString("Max2H", SQLString(poPlayerTemplate.Max2H))
                oUpdate.AppendString("MaxSingle", SQLString(poPlayerTemplate.MaxSingle))

                oDB.executeNonQuery(oUpdate.SQL)
            Catch ex As Exception
                log.Error("Error trying to update PlayerTemplate. SQL: " & oUpdate.SQL, ex)
                bSuccess = False
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdatePlayerMaxPerGameTemplate(ByVal poPlayerTemplate As CPlayerTemplate) As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("PlayerTemplates", String.Format("WHERE PlayerTemplateID={0}", SQLString(poPlayerTemplate.PlayerTemplateID)))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                'oUpdate.AppendString("TemplateName", SQLString(poPlayerTemplate.TemplateName))
                oUpdate.AppendString("LastSavedBy", SQLString(poPlayerTemplate.LastSaveBy))
                oUpdate.AppendString("LastSavedDate", SQLString(poPlayerTemplate.LastSavedDate))
                'oUpdate.AppendString("AccountBalance", SQLString(poPlayerTemplate.AccountBalance))
                'oUpdate.AppendString("CreditMaxAmount", SQLString(poPlayerTemplate.CreditMaxAmount))
                'oUpdate.AppendString("CasinoMaxAmount", SQLString(poPlayerTemplate.CasinoMaxAmount))
                'oUpdate.AppendString("CreditMinBetPhone", SQLString(poPlayerTemplate.CreditMinBetPhone))
                'oUpdate.AppendString("CreditMinBetInternet", SQLString(poPlayerTemplate.CreditMinBetInternet))
                'oUpdate.AppendString("CreditWagerPerGame", SQLString(poPlayerTemplate.CreditWagerPerGame))
                'oUpdate.AppendString("CreditMaxParlay", SQLString(poPlayerTemplate.CreditMaxParlay))
                'oUpdate.AppendString("CreditMaxTeaserParlay", SQLString(poPlayerTemplate.CreditMaxTeaserParlay))
                'oUpdate.AppendString("CreditMaxReverseActionParlay", SQLString(poPlayerTemplate.CreditMaxReverseActionParlay))
                'oUpdate.AppendString("Max1Q", SQLString(poPlayerTemplate.Max1Q))
                'oUpdate.AppendString("Max2Q", SQLString(poPlayerTemplate.Max2Q))
                'oUpdate.AppendString("Max3Q", SQLString(poPlayerTemplate.Max3Q))
                'oUpdate.AppendString("Max4Q", SQLString(poPlayerTemplate.Max4Q))
                'oUpdate.AppendString("Max1H", SQLString(poPlayerTemplate.Max1H))
                'oUpdate.AppendString("Max2H", SQLString(poPlayerTemplate.Max2H))
                oUpdate.AppendString("MaxSingle", SQLString(poPlayerTemplate.MaxSingle))

                oDB.executeNonQuery(oUpdate.SQL)
            Catch ex As Exception
                log.Error("Error trying to update PlayerTemplate. SQL: " & oUpdate.SQL, ex)
                bSuccess = False
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function DeletePlayerTemplate(ByVal poPlayerTemplateID As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim oDelete As New CSQLDeleteStringBuilder("PlayerTemplates", "WHERE PlayerTemplateID=" & SQLString(poPlayerTemplateID))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oDB.executeNonQuery(oDelete.SQL)
            Catch ex As Exception
                log.Error("Error trying to delete Player Template. SQL: " & oDelete.SQL, ex)
                bSuccess = False
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

#End Region

#Region "Get data methods"

        Public Function GetPlayerTemplate(ByVal psPlayerTemplateID As String) As CPlayerTemplate
            Dim oPlayerTemplate As CPlayerTemplate = Nothing

            Dim sSQL As String = "SELECT TOP 1 * FROM PlayerTemplates WHERE PlayerTemplateID=" & SQLString(psPlayerTemplateID)
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim oDT As DataTable = oDb.getDataTable(sSQL)

                If oDT.Rows.Count > 0 Then
                    oPlayerTemplate = New CPlayerTemplate(oDT.Rows(0))
                End If
            Catch ex As Exception
                log.Error("Cannot get player template. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return oPlayerTemplate
        End Function

        Public Function GetPlayerTemplates(ByVal psSiteType As String) As DataTable
            Dim odtTemplates As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("IsDefaultPlayerTemplate='Y'")
            Dim sSQL As String = "SELECT distinct * FROM PlayerTemplates " & oWhere.SQL & vbCrLf & _
                "ORDER BY TemplateName"

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTemplates = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of player templates. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTemplates
        End Function

        Public Function GetAccountBalance(ByVal psPlayerTemplateID As String) As Double
            Dim nAccountBalance As Double = 0

            Dim sSQL As String = "SELECT AccountBalance FROM PlayerTemplates WHERE PlayerTemplateID=" _
            & SQLString(psPlayerTemplateID)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                nAccountBalance = SafeDouble(odbSQL.getScalerValue(sSQL))

            Catch ex As Exception
                log.Error("Cannot get balance amount player template. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nAccountBalance
        End Function

#End Region

#Region "Check data methods"

        Public Function IsExistedTemplateName(ByVal psTemplateName As String, ByVal psSiteType As String, Optional ByVal psPlayerTemplateID As String = "") As Boolean
            Dim bExisted As Boolean = True

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("TemplateName=" & SQLString(psTemplateName))
            oWhere.AppendOptionalANDCondition("PlayerTemplateID", SQLString(psPlayerTemplateID), "<>")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = "SELECT COUNT(TemplateName) FROM PlayerTemplates " & oWhere.SQL
            LogDebug(log, "Check Existed template.SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                bExisted = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check existed player template name. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bExisted
        End Function

#End Region

        Public Function CreditMaxAmountByPlayerID(ByVal psPlayerID As String) As Double
            Dim nCreditMaxAmount As Double
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PlayerID=" & SQLString(psPlayerID))
            Dim sSQL As String = "SELECT CreditMaxAmount FROM Players INNER JOIN PlayerTemplates ON  Players.PlayerTemplateID=PlayerTemplates.PlayerTemplateID " & oWhere.SQL
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                nCreditMaxAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Cannot get CreditMaxAmount from player template. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return nCreditMaxAmount
        End Function



    End Class

End Namespace

