Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CSysSettingManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CSysSettingManager))
        Dim strKeyTimeLineOff As String = "TimeLineOff"
        Public Function GetSysSetting(ByVal psSysSettingID As String) As DataRow
            Dim odrSysSetting As DataRow = Nothing

            Dim sSQL As String = "SELECT * FROM SysSettings WHERE SysSettingID=" & SQLString(psSysSettingID)
            log.Debug("Get sys setting infor. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim odtSysSetitngs As DataTable = odbSQL.getDataTable(sSQL)

                If odtSysSetitngs.Rows.Count > 0 Then
                    odrSysSetting = odtSysSetitngs.Rows(0)
                End If

            Catch ex As Exception
                log.Error("Cannot get Sys Setting infor. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odrSysSetting
        End Function

        Public Function GetSysSettings() As DataTable
            Dim odtSysSettings As DataTable = Nothing

            Dim sSQL As String = "SELECT * FROM SysSettings ORDER BY ItemIndex, Category, SubCategory, [Key]  "
            log.Debug("Get the list of sys settings. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtSysSettings = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of Sys Setting. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtSysSettings
        End Function

        Public Function GetSysSettings(ByVal psCategory As String) As DataTable
            Dim odtSysSettings As DataTable = Nothing
            If System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psCategory) Is Nothing Then
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
                oWhere.AppendANDCondition("ISNULL(SubCategory,'')=''")

                Dim sSQL As String = "SELECT * FROM SysSettings " & oWhere.SQL & " ORDER BY ItemIndex, Category, [Key]  "
                log.Debug("Get the list of sys settings by category. SQL: " & sSQL)

                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Try
                    odtSysSettings = odbSQL.getDataTable(sSQL)

                Catch ex As Exception
                    log.Error("Cannot get the list of Sys Setting by category. SQL: " & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
                System.Web.HttpContext.Current.Cache.Add(strKeyTimeLineOff & psCategory, odtSysSettings, Nothing, Date.Now.AddMinutes(100), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psCategory), DataTable)
        End Function

        Public Function GetSysSettings(ByVal psCategory As String, ByVal psSubCategory As String) As DataTable
            Dim odtSysSettings As DataTable = Nothing
            If System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psCategory & psSubCategory) Is Nothing Then
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
                oWhere.AppendANDCondition("ISNULL(SubCategory,'')=" & SQLString(psSubCategory))

                Dim sSQL As String = "SELECT * FROM SysSettings " & oWhere.SQL & " ORDER BY ItemIndex, Category, SubCategory, [Key]  "
                log.Debug("Get the list of sys settings by category and subcategory. SQL: " & sSQL)

                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Try
                    odtSysSettings = odbSQL.getDataTable(sSQL)

                Catch ex As Exception
                    log.Error("Cannot get the list of Sys Setting by category and subcategory. SQL: " & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
                System.Web.HttpContext.Current.Cache.Add(strKeyTimeLineOff & psCategory & psSubCategory, odtSysSettings, Nothing, Date.Now.AddMinutes(100), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psCategory & psSubCategory), DataTable)
        End Function

        Public Function GetAllSysSettings(ByVal psCategory As String) As CSysSettingList
            Dim oSysSettings As New CSysSettingList
            If System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & "GetAllSysSettings" & psCategory) Is Nothing Then
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("Category=" & SQLString(psCategory))

                Dim sSQL As String = "SELECT * FROM SysSettings " & oWhere.SQL & " ORDER BY ItemIndex, Category, [Key] ASC"
                log.Debug("Get the list of sys settings by category. SQL: " & sSQL)

                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Try
                    Dim odtSysSettings As DataTable = odbSQL.getDataTable(sSQL)

                    For Each odrSysSetting As DataRow In odtSysSettings.Rows
                        oSysSettings.Add(New CSysSetting(odrSysSetting))
                    Next

                Catch ex As Exception
                    log.Error("Cannot get the list of Sys Setting by category. SQL: " & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
                System.Web.HttpContext.Current.Cache.Add(strKeyTimeLineOff & "GetAllSysSettings" & psCategory, oSysSettings, Nothing, Date.Now.AddMinutes(100), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & "GetAllSysSettings" & psCategory), CSysSettingList)
        End Function

        Public Function GetCategories() As DataTable
            Dim odtCategories As DataTable = Nothing

            Dim sSQL As String = "SELECT Category FROM SysSettings Group By Category ORDER BY Category"
            log.Debug("Get the list of categories. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtCategories = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of categories. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtCategories
        End Function

        Public Function GetSubCategories(ByVal psCategory As String) As DataTable
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
            Dim odtCategories As DataTable = Nothing

            Dim sSQL As String = "SELECT DISTINCT SubCategory FROM SysSettings " & oWhere.SQL & " ORDER BY SubCategory"

            log.Debug("Get the list of SubCategories. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtCategories = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of SubCategories. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtCategories
        End Function

        Public Function GetSystemSetting(ByVal psCategory As String, ByVal psKey As String) As DataRow
            If System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psKey & psCategory) Is Nothing Then

                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
                oWhere.AppendANDCondition("[key]=" & SQLString(psKey))
                Dim odtCategories As New DataTable

                Dim sSQL As String = "SELECT TOP 1 * FROM SysSettings  " & oWhere.SQL & " ORDER BY SysSettingID "

                log.Debug("Get the GetSystemSetting by psCategory and psKey. SQL: " & sSQL)

                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Try
                    odtCategories = odbSQL.getDataTable(sSQL)

                Catch ex As Exception
                    log.Error("Cannot  the GetSystemSetting by psCategory and psKey. SQL: " & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try

                If odtCategories.Rows.Count > 0 Then
                    System.Web.HttpContext.Current.Cache.Add(strKeyTimeLineOff & psKey & psCategory, odtCategories.Rows(0), Nothing, Date.Now.AddMinutes(100), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
                Else
                    Return Nothing
                End If

            End If
            Return CType(System.Web.HttpContext.Current.Cache(strKeyTimeLineOff & psKey & psCategory), DataRow)
        End Function

        'thuong
        Public Function CheckExistSysSetting(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String) As Boolean
            Dim oExistSysSetting As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Key] = " & SQLString(psKey))
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("[SubCategory]=" & SQLString(psSubCategoryName))

            Dim sSQL As String = "SELECT count(SysSettingID) FROM SysSettings " & oWhere.SQL

            log.Debug("Check Exist syssetting . SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oExistSysSetting = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot Check Exist syssetting . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oExistSysSetting
        End Function

        'Public Function CheckExistSysSetting(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String, ByVal psOther As String) As Boolean
        '    Dim oExistSysSetting As Boolean = False
        '    Dim oWhere As New CSQLWhereStringBuilder
        '    oWhere.AppendANDCondition("[Key] = " & SQLString(psKey))
        '    oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
        '    oWhere.AppendANDCondition("[SubCategory]=" & SQLString(psSubCategoryName))
        '    oWhere.AppendOptionalANDCondition("[Orther]", SQLString(psOther), "=")

        '    Dim sSQL As String = "SELECT count(SysSettingID) FROM SysSettings " & oWhere.SQL

        '    log.Debug("Check Exist syssetting . SQL: " & sSQL)

        '    Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

        '    Try
        '        oExistSysSetting = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
        '    Catch ex As Exception
        '        log.Error("Cannot Check Exist syssetting . SQL: " & sSQL, ex)
        '    Finally
        '        odbSQL.closeConnection()
        '    End Try

        '    Return oExistSysSetting
        'End Function

        Public Function CheckExistSysSetting(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String, _
                                             ByVal psOther As String, ByVal psOtherType As String) As Boolean
            Dim oExistSysSetting As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Key] = " & SQLString(psKey))
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("[SubCategory]=" & SQLString(psSubCategoryName))
            oWhere.AppendOptionalANDCondition("[Orther]", SQLString(psOther), "=")
            oWhere.AppendOptionalANDCondition("[OtherType]", SQLString(psOtherType), "=")

            Dim sSQL As String = "SELECT count(SysSettingID) FROM SysSettings " & oWhere.SQL

            log.Debug("Check Exist syssetting . SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oExistSysSetting = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot Check Exist syssetting . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oExistSysSetting
        End Function

        Public Function IsExistSysSetting(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psOrther As String) As Boolean
            Dim oExistSysSetting As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Orther] = " & SQLString(psOrther))
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("[SubCategory]=" & SQLString(psSubCategoryName))

            Dim sSQL As String = "SELECT count(*) FROM SysSettings " & oWhere.SQL

            log.Debug("Check Exist syssetting . SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oExistSysSetting = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
                'log.Debug("Check Exist syssetting . SQL: " & SafeInteger(odbSQL.getScalerValue(sSQL)))
            Catch ex As Exception
                log.Error("Cannot Check Exist syssetting . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oExistSysSetting
        End Function

        Public Function CheckExistSysSetting(ByVal psSysSettingID As String, ByVal psBookmakerType As String) As Boolean
            Dim oExistSysSetting As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("SysSettingID = " & SQLString(psSysSettingID))
            oWhere.AppendANDCondition("Category = " & SQLString(psBookmakerType))

            Dim sSQL As String = "SELECT count(SysSettingID) FROM SysSettings " & oWhere.SQL

            log.Debug("Check Exist syssetting . SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oExistSysSetting = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot Check Exist syssetting . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oExistSysSetting
        End Function

        Public Function CheckExistBookMakerType(ByVal psBookmakerType As String) As Boolean
            Dim oExistBookMakerType As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category = " & SQLString(psBookmakerType))
            Dim sSQL As String = "SELECT count(SysSettingID) FROM SysSettings " & oWhere.SQL

            log.Debug("Check Exist BookMakerType . SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oExistBookMakerType = SafeInteger(odbSQL.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot Check Exist BookMakerType . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return oExistBookMakerType
        End Function


#Region "Edit,Add,Delete"

        Public Function UpdateCategoryName(ByVal psName As String, ByVal psNewName As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category=" & SQLString(psName))

            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", oWhere.SQL)
            oUpdate.AppendString("Category", SQLString(psNewName))

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oDB.executeNonQuery(oUpdate.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update Sys Settings. SQL: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateSubCategoryName(ByVal psCategoryName As String, ByVal psOldSubCategory As String, ByVal psNewSubCategory As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("SubCategory=" & SQLString(psOldSubCategory))

            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", oWhere.SQL)
            oUpdate.AppendString("SubCategory", SQLString(psNewSubCategory))

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                oDB.executeNonQuery(oUpdate.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update Sub Category. SQL: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function IsExistedCategory(ByVal psCategoryName As String) As Boolean
            Dim bExist As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))

            Dim sSQL As String = "SELECT Count(*) FROM SysSettings" & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                bExist = SafeInteger(oDB.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check existed sys setting category. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bExist
        End Function

        Public Function IsExistedSubCategory(ByVal psCategoryName As String, ByVal psSubCategoryName As String, Optional ByVal psOldSubCategory As String = "") As Boolean
            Dim bExist As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("[SubCategory]=" & SQLString(psSubCategoryName))
            oWhere.AppendOptionalANDCondition("[SubCategory]", SQLString(psOldSubCategory), "<>")

            Dim sSQL As String = "SELECT Count(*) FROM SysSettings" & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            log.Debug("Get number Sub Categories. SQL:" & sSQL)
            Try
                bExist = SafeInteger(oDB.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check existed sys setting sub category. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bExist
        End Function

        Public Function IsExistedKey(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String, Optional ByVal psSysSettingID As String = "") As Boolean
            Dim bExist As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("ISNULL(SubCategory,'')=" & SQLString(psSubCategoryName))
            oWhere.AppendANDCondition("[Key]=" & SQLString(psKey))
            oWhere.AppendOptionalANDCondition("SysSettingID", SQLString(psSysSettingID), "<>")

            Dim sSQL As String = "SELECT Count(*) FROM SysSettings" & oWhere.SQL
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                bExist = SafeInteger(oDB.getScalerValue(sSQL)) > 0
            Catch ex As Exception
                log.Error("Cannot check existed sys setting key. SQL: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bExist
        End Function

        Public Function UpdateKeyValue(ByVal psSysSettingID As String, ByVal psNewKey As String, ByVal psNewValue As String, _
                                       Optional ByVal psItemIndex As Integer = 0, Optional ByVal psSubCategory As String = "", Optional ByVal psOrther As String = "", _
                                       Optional ByVal psOtherType As String = "") As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", "WHERE SysSettingID=" & SQLString(psSysSettingID))
            With oUpdate
                .AppendString("[Key]", SQLString(psNewKey))
                .AppendString("[Value]", SQLString(StrConv(psNewValue, VbStrConv.ProperCase)))
                If psItemIndex <> 0 Then
                    .AppendString("[ItemIndex]", SQLString(psItemIndex))
                End If
                If psSubCategory <> "" Then
                    .AppendString("[SubCategory]", SQLString(psSubCategory))
                End If
                If psOrther <> "" Then
                    .AppendString("[Orther]", SQLString(psOrther))
                End If
                If psOtherType <> "" Then
                    .AppendString("[OtherType]", SQLString(psOtherType))
                End If
            End With

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oUpdate.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update new Key & Value for Category. Error: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateValue(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String, ByVal psNewValue As String, _
                                       Optional ByVal psSysSettingID As String = "", Optional ByVal psOrther As String = "", Optional ByVal psOtherType As String = "") As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("ISNULL(SubCategory,'')=" & SQLString(psSubCategoryName))
            oWhere.AppendANDCondition("ISNULL([Key],'')=" & SQLString(psKey))
            oWhere.AppendOptionalANDCondition("SysSettingID", SQLString(psSysSettingID), "=")
            oWhere.AppendOptionalANDCondition("[Orther]", SQLString(psOrther), "=")
            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", oWhere.SQL)
            With oUpdate
                .AppendString("[Value]", SQLString(psNewValue))
                If psOrther <> "" Then
                    .AppendString("[Orther]", SQLString(psOrther))
                End If
                If psOtherType <> "" Then
                    .AppendString("[OtherType]", SQLString(psOtherType))
                End If
            End With
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oUpdate.SQL)
                ClearTimeOffCache()
                LogDebug(log, "update " & oUpdate.SQL)
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update new Value for SysSettings. Error: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateByKey(ByVal psSysSettingID As String, ByVal psNewKey As String, ByVal psNewValue As String, _
                                       ByVal psItemIndex As Integer, ByVal psSubCategory As String, ByVal psOrther As String, _
                                       ByVal psOtherType As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", "WHERE SysSettingID=" & SQLString(psSysSettingID))
            With oUpdate
                .AppendString("[Key]", SQLString(psNewKey))
                .AppendString("[Value]", SQLString(StrConv(psNewValue, VbStrConv.ProperCase)))
                .AppendString("[ItemIndex]", SQLString(psItemIndex))
                .AppendString("[SubCategory]", SQLString(psSubCategory))
                .AppendString("[Orther]", SQLString(psOrther))
                .AppendString("[OtherType]", SQLString(psOtherType))
            End With

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oUpdate.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update new Key & Value for Category. Error: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function GetValue(ByVal psCategoryName As String, ByVal psSubCategoryName As String, ByVal psKey As String, _
                                      Optional ByVal psSysSettingID As String = "") As DataRow
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("[Category]=" & SQLString(psCategoryName))
            oWhere.AppendANDCondition("ISNULL(SubCategory,'')=" & SQLString(psSubCategoryName))
            oWhere.AppendANDCondition("ISNULL([Key],'')=" & SQLString(psKey))
            oWhere.AppendOptionalANDCondition("SysSettingID", SQLString(psSysSettingID), "=")


            Dim odtCategories As New DataTable

            Dim sSQL As String = "SELECT TOP 1 * FROM SysSettings  " & oWhere.SQL

            log.Debug("Get the GetSystemSetting by psCategory and psKey and psSubCategory. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtCategories = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot  the GetSystemSetting by psCategory and psKey and psSubCategory. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            If odtCategories IsNot Nothing AndAlso odtCategories.Rows.Count > 0 Then
                Return odtCategories.Rows(0)
            Else
                Return Nothing
            End If


        End Function

        Public Function AddSysSetting(ByVal psCategoryName As String, ByVal psKey As String, ByVal psValue As String, _
                                      Optional ByVal psSubCategory As String = "", Optional ByVal psItemIndex As Integer = 0, Optional ByVal psOrther As String = "", _
                                      Optional ByVal psOtherType As String = "") As Boolean
            Dim bSuccess As Boolean = True
            Dim oInsert As New CSQLInsertStringBuilder("SysSettings")
            With oInsert
                .AppendString("[SysSettingID]", SQLString(newGUID()))
                .AppendString("[Category]", SQLString(psCategoryName))
                .AppendString("[Key]", SQLString(psKey))
                .AppendString("[Value]", SQLString(StrConv(psValue, VbStrConv.ProperCase)))
                .AppendString("[SubCategory]", SQLString(psSubCategory))
                If psItemIndex <> 0 Then
                    .AppendString("[ItemIndex]", SQLString(psItemIndex))
                End If
                If psOrther <> "" Then
                    .AppendString("[Orther]", SQLString(psOrther))
                End If
                If psOtherType <> "" Then
                    .AppendString("[OtherType]", SQLString(psOtherType))
                End If
            End With

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oInsert.SQL)
                log.Debug("Cannot add new Sys Setting. Error:" & oInsert.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot add new Sys Setting. Error:" & oInsert.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function AddSubCategory(ByVal psCategory As String, ByVal psSubCategory As String, Optional ByVal psItemIndex As Integer = 0) As Boolean
            Dim bSuccsess As Boolean = True
            Dim oInsert As New CSQLInsertStringBuilder("SysSettings")
            With oInsert
                .AppendString("[SysSettingID]", SQLString(newGUID()))
                .AppendString("[Category]", SQLString(psCategory))
                .AppendString("[SubCategory]", SQLString(psSubCategory))
                If psItemIndex <> 0 Then
                    .AppendString("[ItemIndex]", SQLString(psItemIndex))
                End If
            End With

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(oInsert.SQL)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccsess = False
                log.Error("Cannot add new Sub Category. Error:" & oInsert.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccsess
        End Function

        Public Function DeleteSetting(ByVal psSettingID As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim sSQL As String = "DELETE FROM SysSettings WHERE SysSettingID=" & SQLString(psSettingID)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
                ClearTimeOffCache()
            Catch ex As Exception
                log.Error("Cannot delete this Setting. SQL: " & sSQL, ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function DeleteSettingByCategory(ByVal psCategory As String) As Boolean
            Dim bSuccess As Boolean = True

            Dim sSQL As String = "DELETE FROM SysSettings WHERE Category=" & SQLString(psCategory)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
                ClearTimeOffCache()
            Catch ex As Exception
                log.Error("Cannot delete this Setting. SQL: " & sSQL, ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function DeleteSettingBySubCategory(ByVal psCategory As String, ByVal psSubCategory As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
            oWhere.AppendANDCondition("ISNULL(SubCategory,'')=" & SQLString(psSubCategory))

            Dim sSQL As String = "DELETE FROM SysSettings " & oWhere.SQL
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
                ClearTimeOffCache()
            Catch ex As Exception
                log.Error("Cannot delete this Sub Category. SQL: " & sSQL, ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function DeleteSettingByKey(ByVal psCategory As String, ByVal psKey As String) As Boolean
            Dim oListKey = New List(Of String)()
            oListKey.Add(psKey)
            DeleteSettingByKey(psCategory, oListKey)
            ClearTimeOffCache()
        End Function

        Public Function DeleteSettingByKey(ByVal psCategory As String, ByVal polstKeys As List(Of String)) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("Category=" & SQLString(psCategory))
            oWhere.AppendANDCondition(String.Format("ISNULL([Key],'') IN ('{0}')", Join(polstKeys.ToArray(), "','")))

            Dim sSQL As String = "DELETE FROM SysSettings " & oWhere.SQL
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
                ClearTimeOffCache()
            Catch ex As Exception
                log.Error("Cannot delete this Sub Category. SQL: " & sSQL, ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function


        Public Function DeleteAllBookMaker(ByVal psCategory As String, ByVal psKeys As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition(String.Format("Category like '%{0}%'", psCategory))
            oWhere.AppendANDCondition(String.Format("ISNULL([Key],'') = '{0}'", psKeys))

            Dim sSQL As String = "DELETE FROM SysSettings " & oWhere.SQL
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odbSQL.executeNonQuery(sSQL)
                ClearTimeOffCache()
            Catch ex As Exception
                log.Error("Cannot delete these Sub Category. SQL: " & sSQL, ex)
                bSuccess = False
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateSysSettings(ByVal psCategory As String, ByVal poSysSettings As List(Of CSysSetting)) As Boolean
            Dim bSuccess As Boolean = True
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim oIDs As New List(Of String)

                Dim sSQL As String = "SELECT * FROM SysSettings WHERE Category=" & SQLString(psCategory)
                Dim odtSysSettings As DataTable = odbSQL.getDataTable(sSQL)

                For Each oSysSetting As CSysSetting In poSysSettings
                    Dim odrFinds As DataRow() = odtSysSettings.Select("SysSettingID=" & SQLString(oSysSetting.SysSettingID))
                    Dim odrSysSetting As DataRow = Nothing

                    If odrFinds.Length = 0 Then
                        odrSysSetting = odtSysSettings.NewRow()
                        odrSysSetting("SysSettingID") = oSysSetting.SysSettingID
                        odrSysSetting("Category") = oSysSetting.Category
                        odrSysSetting("SubCategory") = oSysSetting.SubCategory
                        If oSysSetting.ItemIndex <> 0 Then
                            odrSysSetting("ItemIndex") = oSysSetting.ItemIndex
                        End If

                        odtSysSettings.Rows.Add(odrSysSetting)
                    Else
                        odrSysSetting = odrFinds(0)
                    End If

                    odrSysSetting("Key") = oSysSetting.Key
                    odrSysSetting("Value") = oSysSetting.Value

                    oIDs.Add(UCase(oSysSetting.SysSettingID))
                Next

                For nIndex As Integer = odtSysSettings.Rows.Count - 1 To 0 Step -1
                    If Not oIDs.Contains(UCase(SafeString(odtSysSettings.Rows(nIndex)("SysSettingID")))) Then
                        odtSysSettings.Rows(nIndex).Delete()
                    End If
                Next

                odbSQL.UpdateDataTable(odtSysSettings, "SELECT TOP 0 * FROM SysSettings", False)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Cannot update Sys Settings. Error:" & ex.Message, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateBookMaker(ByVal psID As String, ByVal psValue As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim oUpdate As New CSQLUpdateStringBuilder("SysSettings", "WHERE SysSettingID= " & SQLString(psID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("Value", SQLString(StrConv(psValue, VbStrConv.ProperCase)))
                End With
                log.Debug("Update SysSettings. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                ClearTimeOffCache()
            Catch ex As Exception
                bSuccess = False
                log.Error("Error trying to save SysSettings. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function CopyParplayReverser(ByVal psSuperAgentID As String) As Boolean
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim bSuccsess = True
            Dim sSQL As String = ""
            Try
                sSQL = String.Format("insert into SysSettings(SysSettingID,Category,[Key],[Value],SubCategory,ItemIndex) select newID() as  SysSettingID,replace(Category,'{1}','{0}') as Category,[Key],[Value],SubCategory,ItemIndex  from SysSettings where [Category] like '{1}%' ", psSuperAgentID, std.AGENT_ID_DEFAULT)
                oDB.executeNonQuery(sSQL)
            Catch ex As Exception
                bSuccsess = False
                log.Error("Cannot add new Sub Category. Error:" & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return bSuccsess
        End Function

#End Region

        Sub ClearTimeOffCache()
            Dim keys As New List(Of String)
            Dim CacheEnum As IDictionaryEnumerator = System.Web.HttpContext.Current.Cache.GetEnumerator()
            While CacheEnum.MoveNext()
                If CacheEnum.Key.ToString().Contains(strKeyTimeLineOff) Then
                    System.Web.HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString())
                End If
            End While
        End Sub

    End Class
End Namespace