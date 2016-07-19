Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CTeaserRuleManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        ''' <summary>
        ''' Get TeaserRule's Info By ID
        ''' </summary>
        ''' <param name="psTeaserRuleID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByID(ByVal psTeaserRuleID As String) As DataTable
            If SafeString(psTeaserRuleID) <> "" Then
                Dim oDB As CSQLDBUtils = Nothing
                Dim sSQL As String = "SELECT TOP 1 * FROM TeaserRules WHERE TeaserRuleID = " & SQLString(psTeaserRuleID)
                Try
                    log.Debug("Get TeaserRule's Info by TeaserRuleID: " & psTeaserRuleID)
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                    Return oDB.getDataTable(sSQL)
                Catch ex As Exception
                    log.Error("Fails to get TeaserRule by ID.SQL: " & sSQL, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Lock/Unlock multi TeaserRules
        ''' </summary>
        ''' <param name="pLstTeaserRuleIDs"></param>
        ''' <param name="pbLocked"></param>
        ''' <param name="psUpdateBy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LockTeaserRules(ByVal pLstTeaserRuleIDs As List(Of String), ByVal pbLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            Try
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sTeaserRuleID As String In pLstTeaserRuleIDs
                    oCache.ClearTeaserRuleInfo(sTeaserRuleID)
                Next

                log.Debug("Lock/Unlock these TeaserRules: " & Join(pLstTeaserRuleIDs.ToArray(), ","))
                oUpdate = New CSQLUpdateStringBuilder("TeaserRules", String.Format("WHERE TeaserRuleID IN ('{0}')", Join(pLstTeaserRuleIDs.ToArray(), "','")))
                oUpdate.AppendString("IsLocked", SQLString(IIf(pbLocked, "Y", "N")))
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                oDB.executeNonQuery(oUpdate, psUpdateBy)

                Return True
            Catch ex As Exception
                log.Error("Fails to lock/unlock TeaserRules.SQL: " & SafeString(IIf(oUpdate IsNot Nothing, oUpdate.SQL, "")), ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function ChangeTeaserRuleIndexes(ByVal poTeaserRuleIndexs As Hashtable) As Boolean
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim oIDs As New List(Of String)(From oKey As Object In poTeaserRuleIndexs.Keys Select SQLString(oKey))
            Dim sSqlSchema As String = "SELECT TeaserRuleID, TeaserRuleIndex FROM TeaserRules " & vbCrLf & _
                "WHERE TeaserRuleID IN (" & Join(oIDs.ToArray, ",") & ")"

            Try
                Dim odtIndexes As DataTable = odbSQL.getDataTable(sSqlSchema)

                For Each oKey As Object In poTeaserRuleIndexs.Keys
                    Dim odrIndexes As DataRow() = odtIndexes.Select("TeaserRuleID=" & SQLString(oKey))

                    If odrIndexes.Length > 0 Then
                        odrIndexes(0)("TeaserRuleIndex") = SafeInteger(poTeaserRuleIndexs(oKey))
                    Else
                        Dim odrIndex As DataRow = odtIndexes.NewRow
                        odtIndexes.Rows.Add(odrIndex)

                        odrIndex("TeaserRuleID") = oKey
                        odrIndex("TeaserRuleIndex") = poTeaserRuleIndexs(oKey)
                    End If
                Next

                For i As Integer = odtIndexes.Rows.Count - 1 To 0 Step -1
                    If Not poTeaserRuleIndexs.ContainsKey(odtIndexes.Rows(i)("TeaserRuleID")) Then
                        odtIndexes.Rows(i).Delete()
                    End If
                Next

                odbSQL.UpdateDataTable(odtIndexes, "SELECT TOP 0 TeaserRuleID, TeaserRuleIndex FROM TeaserRules", False)
                Return True

            Catch ex As Exception
                log.Error("Fails to change tearse rules index", ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return False
        End Function

        Public Function InsertTeaserRule(ByVal psTeaserRuleID As String, ByVal psName As String, ByVal pnMinTeam As Integer, _
                                    ByVal pnMaxTeam As Integer, ByVal psBasketballPoint As String, _
                                    ByVal psFootballPoint As String, ByVal pbIsTiesLose As Boolean, _
                                    ByVal psCreatedBy As String, ByVal psSiteType As String) As Boolean

            Dim bSuccess As Boolean = False

            Dim oInsert As New CSQLInsertStringBuilder("TeaserRules")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Dim nMaxIndex As Integer = SafeInteger(odbSQL.getScalerValue("SELECT MAX(TeaserRuleIndex) FROM TeaserRules where SiteType=" & SQLString(psSiteType))) + 1

                With oInsert
                    .AppendString("TeaserRuleID", SQLString(psTeaserRuleID))
                    .AppendString("TeaserRuleName", SQLString(psName))
                    .AppendString("MinTeam", SQLString(pnMinTeam))
                    .AppendString("MaxTeam", SQLString(pnMaxTeam))
                    .AppendString("BasketballPoint", SafeString(psBasketballPoint))
                    .AppendString("FootballPoint", SQLString(psFootballPoint))
                    .AppendString("IsTiesLose", SQLString(IIf(pbIsTiesLose, "Y", "N")))
                    .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("CreatedBy", SQLString(psCreatedBy))
                    .AppendString("TeaserRuleIndex", SafeString(nMaxIndex))
                    .AppendString("SiteType", SQLString(psSiteType))
                End With

                log.Debug("Insert TeaserRule.SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psCreatedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearTeaserRules()

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to insert TeaserRule. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateTeaserRule(ByVal psTeaserRuleID As String, ByVal psName As String, ByVal pnMinTeam As Integer, _
                                    ByVal pnMaxTeam As Integer, ByVal psBasketballPoint As String, _
                                    ByVal psFootballPoint As String, ByVal pbIsTiesLose As Boolean, _
                                    ByVal psChangedBy As String, ByVal psSiteType As String) As Boolean

            Dim bSuccess As Boolean = False

            Dim oUpdate As New CSQLUpdateStringBuilder("TeaserRules", String.Format("WHERE TeaserRuleID={0} and SiteType={1} ", SQLString(psTeaserRuleID), SQLString(psSiteType)))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("TeaserRuleName", SQLString(psName))
                    .AppendString("MinTeam", SQLString(pnMinTeam))
                    .AppendString("MaxTeam", SQLString(pnMaxTeam))
                    .AppendString("BasketballPoint", SafeString(psBasketballPoint))
                    .AppendString("FootballPoint", SQLString(psFootballPoint))
                    .AppendString("IsTiesLose", SQLString(IIf(pbIsTiesLose, "Y", "N")))
                End With

                log.Debug("Update TeaserRule. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearTeaserRuleInfo(psTeaserRuleID)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to save Agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function DeleteTeaserRule(ByVal psTeaserRuleID As String, ByVal psDeletedBy As String) As Boolean
            Dim bSuccess As Boolean = False

            Dim oDelete As New CSQLDeleteStringBuilder("TeaserRules", String.Format("WHERE TeaserRuleID= {0}", SQLString(psTeaserRuleID)))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                log.Debug("Delete teaser rule. SQL: " & oDelete.SQL)
                odbSQL.executeNonQuery(oDelete, psDeletedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearTeaserRuleInfo(psTeaserRuleID)

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to deleet teaser rule. SQL: " & oDelete.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function GetTeaserRules(ByVal pbLocked As Boolean, ByVal psSiteType As String) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'N')= " & SQLString(IIf(pbLocked, "Y", "N")))

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "SELECT * FROM TeaserRules " & oWhere.SQL & " ORDER BY TeaserRuleIndex, TeaserRuleName"
            Try
                log.Debug("Get TeaserRules. SQL: " & sSQL)
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fail to get all TeaserRules: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetTeaserRules(ByVal psSiteType As String) As DataTable
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "SELECT * FROM TeaserRules where SiteType=" & SQLString(psSiteType) & " ORDER BY TeaserRuleIndex, TeaserRuleName"
            Try
                log.Debug("Get TeaserRules. SQL: " & sSQL)
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fail to get all TeaserRules: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function IsExistName(ByVal psTeaserRuleName As String, ByVal psTeaserRuleID As String, ByVal psSiteType As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("TeaserRuleName = " & SQLString(psTeaserRuleName))
            oWhere.AppendANDCondition("SiteType = " & SQLString(psSiteType))
            oWhere.AppendOptionalANDCondition("TeaserRuleID ", SQLString(psTeaserRuleID), "<>")

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "SELECT COUNT(*) AS NUM FROM TeaserRules " & oWhere.SQL
            Try
                Return SafeInteger(oDB.getScalerValue(sSQL)) <> 0
            Catch ex As Exception
                log.Error("Fail to check exist TeaserRule's Name: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return False
        End Function

    End Class

End Namespace