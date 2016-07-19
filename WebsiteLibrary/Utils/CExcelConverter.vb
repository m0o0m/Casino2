Imports WebsiteLibrary.CXMLUtils
Imports WebsiteLibrary.CSBCStd
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports System.Data
Imports System.Collections

''' <summary>
''' Static methods for reading/writing Excel XML schema
''' </summary>
Public Class CExcelConverter

    Public FieldEnums As New Hashtable()

#Region "EXCLUDE COLUMNS"
    Private ExcludeColumns As New System.Collections.Specialized.StringCollection()

    Public Sub AddExcludeColumn(ByVal psColumnName As String)
        ExcludeColumns.Add(UCase(psColumnName))
    End Sub

    Public Sub RemoveExcludeColumn(ByVal psColumnName As String)
        ExcludeColumns.Remove(UCase(psColumnName))
    End Sub
#End Region

#Region "READ IN"
    ''' <summary>
    ''' Main static method which imports XML spreadsheet into DataTable
    ''' </summary>
    ''' <returns>dataTable result</returns>
    Public Function ReadExcelXML(ByVal xc As XmlDocument) As DataTable
        Dim dt As New DataTable()
        Dim nsmgr As New XmlNamespaceManager(xc.NameTable)
        nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office")
        nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel")
        nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet")

        Dim xe As XmlElement = CType(xc.DocumentElement.SelectSingleNode("//ss:Worksheet/ss:Table", nsmgr), XmlElement)
        If (xe Is Nothing) Then
            Return Nothing
        End If

        Dim xl As XmlNodeList = xe.SelectNodes("ss:Row", nsmgr)
        Dim Row As Integer = -1
        Dim Col As Integer = 0

        Dim cols As New Dictionary(Of Integer, String) 'maps the column index to the column name of the datatable being created
        Dim colFields As New Dictionary(Of Integer, String) 'maps the column index to the original column name aka EnumValue
        For Each xi As XmlElement In xl
            If xi.InnerText <> "" Then
                Dim xcells As XmlNodeList = xi.SelectNodes("ss:Cell", nsmgr)
                Col = 0
                For Each xcell As XmlElement In xcells
                    If (Row = -1) And FieldEnums.Count = 0 Then
                        'regular excel format
                        colFields(Col) = xcell.InnerText
                        cols(Col) = xcell.InnerText
                        Col += 1
                    ElseIf (Row = -1) Then
                        'there were enums, so catch this row and do nothing for it
                    ElseIf Row = 0 And FieldEnums.Count > 0 Then
                        Dim sColumnName As String = ""
                        If FieldEnums.ContainsKey(xcell.InnerText) Then
                            sColumnName = SafeString(FieldEnums(xcell.InnerText))
                        Else
                            sColumnName = xcell.InnerText
                        End If

                        If Not dt.Columns.Contains(sColumnName) Then
                            dt.Columns.Add(sColumnName)
                        End If

                        colFields(Col) = xcell.InnerText
                        cols(Col) = sColumnName
                        Col += 1
                    Else
                        If Not (xcell.Attributes("ss:Index") Is Nothing) Then
                            Dim idx As Integer = CInt(xcell.Attributes("ss:Index").InnerText)
                            Col = idx - 1
                        End If

                        Dim sColumnName As String = cols(Col)
                        Dim sFieldColumnName As String = colFields(Col)
                        Dim sValue As String = xcell.InnerText

                        If FieldEnums.ContainsKey(sFieldColumnName) Then
                            If SafeString(xcell.InnerText) = "X" Or SafeString(xcell.InnerText) = "Y" Then
                                sValue = sFieldColumnName
                            End If
                        End If

                        SetCol(dt, Row - 1, sColumnName, sValue, GetType(String))
                        Col += 1
                    End If
                Next

                Row += 1

            End If
        Next

        'drop the excluded columns
        For Col = dt.Columns.Count - 1 To 0 Step -1
            If ExcludeColumns.Contains(UCase(dt.Columns(Col).ColumnName)) Then
                dt.Columns.RemoveAt(Col)
            End If
        Next

        Return dt
    End Function

    ''' <summary>
    ''' Adds row to datatable, manages System.DBNull and so
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="AcceptChanges"></param>
    ''' <returns></returns>
    Private Function AddRow(ByVal dt As DataTable, ByVal AcceptChanges As Boolean) As Integer
        Dim Values(dt.Columns.Count - 1) As Object
        For Column As Integer = 0 To dt.Columns.Count - 1
            If (Not dt.Columns(Column).AllowDBNull) Then
                If (Not IsNothing(dt.Columns(Column).DefaultValue) AndAlso Not dt.Columns(Column).DefaultValue Is GetType(System.DBNull)) Then
                    Values(Column) = dt.Columns(Column).DefaultValue
                End If
            End If
        Next

        dt.Rows.Add(Values)

        If (AcceptChanges) Then
            dt.AcceptChanges()
        End If

        Return dt.Rows.Count - 1
    End Function

    ''' <summary>
    ''' Sets data into datatable in safe manner of row index
    ''' </summary>
    ''' <param name="dt">DataTable to set</param>
    ''' <param name="Row">Ordinal row index</param>
    ''' <param name="ColumnName">name of column to set</param>
    ''' <param name="Value">non/typed value to set</param>
    ''' <param name="TypeOfValue">Becase Value can be null we must know datatype to manage default values</param>
    ''' <returns></returns>
    Private Function SetCol(ByVal dt As DataTable, ByVal Row As Integer, ByVal ColumnName As String, ByVal Value As Object, ByVal TypeOfValue As System.Type) As DataColumn
        If (dt Is Nothing OrElse ColumnName Is Nothing OrElse ColumnName = "") Then
            Return Nothing
        End If

        If (Value Is Nothing) Then
            Value = System.DBNull.Value
        End If

        Dim nIndex As Integer = -1
        Dim dcol As DataColumn = Nothing
        Dim Added As Boolean = False

        If (dt.Columns.Contains(ColumnName)) Then
            dcol = dt.Columns(ColumnName)
        Else
            dcol = dt.Columns.Add(ColumnName, TypeOfValue)
        End If

        If (dcol.ReadOnly) Then
            dcol.ReadOnly = False
        End If

        nIndex = dcol.Ordinal
        ''new empty row appended
        If (dt.Rows.Count = Row AndAlso Row >= 0) Then
            AddRow(dt, False)
            Added = True
        End If

        ''one row
        If (Row >= 0) Then
            If SafeString(dt.Rows(Row)(nIndex)) <> "" Then
                dt.Rows(Row)(nIndex) = SafeString(dt.Rows(Row)(nIndex)) & ";" & SafeString(Value)
            Else
                dt.Rows(Row)(nIndex) = Value
            End If
        ElseIf (Row = -1) Then
            ''all rows
            Try
                For Row = 0 To dt.Rows.Count - 1
                    If (dt.Rows(Row).RowState = DataRowState.Deleted) Then
                        Continue For
                    End If
                    dt.Rows(Row)(nIndex) = Value
                Next
            Catch err As Exception
            End Try
        End If

        Return dcol
    End Function

#End Region

#Region "WRITE OUT"
    Public Function WriteExcelXML(ByVal poDS As DataSet) As XmlDocument
        Dim oDoc As XmlDocument = getDSLExcelXML()

        For Each oTable As DataTable In poDS.Tables
            addDatatable(oTable, oDoc)
        Next

        Return oDoc
    End Function

    Public Function WriteExcelXML(ByVal poDT As DataTable) As XmlDocument
        Dim oDoc As XmlDocument = getDSLExcelXML()
        addDatatable(poDT, oDoc)

        Return oDoc
    End Function

    Private Sub addDatatable(ByVal poDT As DataTable, ByRef poDoc As XmlDocument)
        Dim oWorksheet As XmlElement = AddXMLChild(poDoc.DocumentElement, "Worksheet")
        oWorksheet.SetAttribute("Name", "urn:schemas-microsoft-com:office:spreadsheet", poDT.TableName)

        Dim oWorksheetOptions As XmlElement = AddXMLChild(oWorksheet, "WorksheetOptions")
        oWorksheetOptions.SetAttribute("xmlns", "urn:schemas-microsoft-com:office:excel")

        AddXMLChild(oWorksheetOptions, "ProtectObjects", "False")
        AddXMLChild(oWorksheetOptions, "ProtectScenarios", "False")

        Dim oTable As XmlElement = AddXMLChild(oWorksheet, "Table", , True)

        'list of column names, sorted alphabetically!
        Dim oColumNames As New ArrayList()
        For i As Integer = 0 To poDT.Columns.Count - 1
            oColumNames.Add(poDT.Columns(i).ColumnName)
        Next

        'oColumNames.Sort()

        'add the very first header that might span across columns
        Dim oRow As XmlElement

        If hasEnumColumns(poDT) Then
            oRow = AddXMLChild(oTable, "Row")

            Dim oCell As XmlElement = Nothing
            Dim oData As XmlElement = Nothing
            Dim sCurrentEnumField As String = ""
            Dim nEnumCount As Integer = 0
            Dim sLastColor As String = "sCenterGreen" 'remembers the last style to alternate yellow/green

            For Each sColumnName As String In oColumNames

                If Not ExcludeColumns.Contains(UCase(sColumnName)) Then
                    If isEnumColumn(sColumnName) Then

                        If sCurrentEnumField <> getEnumField(sColumnName) And nEnumCount > 0 Then
                            sLastColor = IIf(sLastColor = "sCenterGreen", "sCenterYellow", "sCenterGreen").ToString
                            oCell = AddXMLChild(oRow, "Cell")
                            oCell.SetAttribute("MergeAcross", "urn:schemas-microsoft-com:office:spreadsheet", CStr(nEnumCount - 1))
                            oCell.SetAttribute("StyleID", "urn:schemas-microsoft-com:office:spreadsheet", sLastColor)

                            oData = AddXMLChild(oCell, "Data")
                            oData.InnerText = sCurrentEnumField
                            oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")

                            nEnumCount = 1 'we output the last enum group, but don't forget to count this new one
                        Else
                            nEnumCount += 1
                        End If

                        sCurrentEnumField = getEnumField(sColumnName)

                    Else

                        If nEnumCount > 0 Then
                            sLastColor = IIf(sLastColor = "sCenterGreen", "sCenterYellow", "sCenterGreen").ToString
                            oCell = AddXMLChild(oRow, "Cell")
                            oCell.SetAttribute("MergeAcross", "urn:schemas-microsoft-com:office:spreadsheet", CStr(nEnumCount - 1))
                            oCell.SetAttribute("StyleID", "urn:schemas-microsoft-com:office:spreadsheet", sLastColor)

                            oData = AddXMLChild(oCell, "Data")
                            oData.InnerText = sCurrentEnumField
                            oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")

                        End If

                        oCell = AddXMLChild(oRow, "Cell")
                        oData = AddXMLChild(oCell, "Data")
                        oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")

                        nEnumCount = 0
                    End If
                End If
            Next

            'if the last set of columns were enum columns, then trigger their output as well
            If nEnumCount > 0 Then
                sLastColor = IIf(sLastColor = "sCenterGreen", "sCenterYellow", "sCenterGreen").ToString
                oCell = AddXMLChild(oRow, "Cell")
                oCell.SetAttribute("MergeAcross", "urn:schemas-microsoft-com:office:spreadsheet", CStr(nEnumCount - 1))
                oCell.SetAttribute("StyleID", "urn:schemas-microsoft-com:office:spreadsheet", sLastColor)

                oData = AddXMLChild(oCell, "Data")
                oData.InnerText = sCurrentEnumField
                oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")
            End If

        End If

        'add the column names
        oRow = AddXMLChild(oTable, "Row")
        For Each sColumnName As String In oColumNames

            If Not ExcludeColumns.Contains(UCase(sColumnName)) Then
                Dim oCell As XmlElement = AddXMLChild(oRow, "Cell")
                oCell.SetAttribute("StyleID", "urn:schemas-microsoft-com:office:spreadsheet", "sCenter")

                Dim oData As XmlElement = AddXMLChild(oCell, "Data")
                oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")

                If isEnumColumn(sColumnName) Then
                    oData.InnerText = getEnumValue(sColumnName)
                Else
                    oData.InnerText = sColumnName
                End If
            End If
        Next

        For Each oDR As DataRow In poDT.Rows
            oRow = AddXMLChild(oTable, "Row")

            For i As Integer = 0 To poDT.Columns.Count - 1
                If Not ExcludeColumns.Contains(UCase(poDT.Columns(i).ColumnName)) Then
                    Dim oCell As XmlElement = AddXMLChild(oRow, "Cell")
                    Dim oData As XmlElement = AddXMLChild(oCell, "Data")

                    Select Case poDT.Columns(i).DataType.ToString
                        Case "System.Double", "System.Decimal", "System.Int32"
                            oData.InnerText = SafeDouble(oDR(i)).ToString
                            oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "Number")
                        Case Else
                            oData.InnerText = SafeString(oDR(i))
                            oData.SetAttribute("Type", "urn:schemas-microsoft-com:office:spreadsheet", "String")
                    End Select
                End If
            Next
        Next

    End Sub

    ''' <summary>
    ''' Runs through the column names and returns true if any are enum columns
    ''' </summary>
    Private Function hasEnumColumns(ByVal poDT As DataTable) As Boolean
        For Each oCol As DataColumn In poDT.Columns
            If isEnumColumn(oCol.ColumnName) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function isEnumColumn(ByVal psColumnName As String) As Boolean
        Return psColumnName.Contains("[") And psColumnName.Contains("]")
    End Function

    ''' <summary>
    ''' returns 'DocType' in DocType[NINA]
    ''' </summary>
    Private Function getEnumField(ByVal psColumnName As String) As String
        Return psColumnName.Substring(0, psColumnName.IndexOf("["))
    End Function

    ''' <summary>
    ''' returns 'NINA' in DocType[NINA]
    ''' </summary>
    Private Function getEnumValue(ByVal psColumnName As String) As String
        Dim sOpenBracket As Integer = psColumnName.IndexOf("[")
        Dim sCloseBracket As Integer = psColumnName.IndexOf("]")
        Return psColumnName.Substring(sOpenBracket + 1, sCloseBracket - sOpenBracket - 1)
    End Function

    Private Function getDSLExcelXML() As XmlDocument
        Dim oDoc As New XmlDocument()
        Dim oNS As New XmlNamespaceManager(oDoc.NameTable)

        Dim sBuilder As New System.Text.StringBuilder()
        With sBuilder
            .Append("<?xml version=""1.0""?>")
            .Append("<?mso-application progid=""Excel.Sheet""?>")
            .Append("<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""")
            .Append("       xmlns:o=""urn:schemas-microsoft-com:office:office""")
            .Append("       xmlns:x=""urn:schemas-microsoft-com:office:excel""")
            .Append("       xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""")
            .Append("       xmlns:html=""http://www.w3.org/TR/REC-html40"">")
            .Append("   <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">")
            .Append("       <Author></Author>")
            .Append("       <LastAuthor></LastAuthor>")
            .Append("       <Created>" & Date.Now.ToUniversalTime.ToString & "</Created>")
            .Append("       <LastSaved>" & Date.Now.ToUniversalTime.ToString & "</LastSaved>")
            .Append("       <Company></Company>")
            .Append("       <Version>11.8107</Version>")
            .Append("   </DocumentProperties>")
            .Append("   <OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">")
            .Append("       <DownloadComponents/>")
            .Append("       <LocationOfComponents HRef=""file:///H:\""/>")
            .Append("   </OfficeDocumentSettings>")
            .Append("   <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">")
            .Append("       <WindowHeight>8580</WindowHeight>")
            .Append("       <WindowWidth>10380</WindowWidth>")
            .Append("       <WindowTopX>360</WindowTopX>")
            .Append("       <WindowTopY>105</WindowTopY>")
            .Append("       <ProtectStructure>False</ProtectStructure>")
            .Append("       <ProtectWindows>False</ProtectWindows>")
            .Append("   </ExcelWorkbook>")
            .Append("</Workbook>")
        End With

        oDoc.LoadXml(sBuilder.ToString())

        Dim oStyles As XmlElement = AddXMLChild(oDoc.DocumentElement, "Styles")
        Dim oStyle As XmlElement = AddXMLChild(oStyles, "Style")
        oStyle.SetAttribute("ID", "urn:schemas-microsoft-com:office:spreadsheet", "Default")
        oStyle.SetAttribute("Name", "urn:schemas-microsoft-com:office:spreadsheet", "Normal")

        AddXMLChild(oStyle, "Alignment")
        AddXMLChild(oStyle, "Borders")
        AddXMLChild(oStyle, "Font")
        AddXMLChild(oStyle, "Interior")
        AddXMLChild(oStyle, "NumberFormat")
        AddXMLChild(oStyle, "Protection")

        '---------------- CENTER titles ---------------'
        oStyle = AddXMLChild(oStyles, "Style")
        oStyle.SetAttribute("ID", "urn:schemas-microsoft-com:office:spreadsheet", "sCenter")

        Dim oAlignment As XmlElement = AddXMLChild(oStyle, "Alignment")
        oAlignment.SetAttribute("Horizontal", "urn:schemas-microsoft-com:office:spreadsheet", "Center")

        '---------------- CENTER/YELLOW titles ---------------'
        oStyle = AddXMLChild(oStyles, "Style")
        oStyle.SetAttribute("ID", "urn:schemas-microsoft-com:office:spreadsheet", "sCenterYellow")

        oAlignment = AddXMLChild(oStyle, "Alignment")
        oAlignment.SetAttribute("Horizontal", "urn:schemas-microsoft-com:office:spreadsheet", "Center")

        Dim oInterior As XmlElement = AddXMLChild(oStyle, "Interior")
        oInterior.SetAttribute("Color", "urn:schemas-microsoft-com:office:spreadsheet", "#FFFF00")
        oInterior.SetAttribute("Pattern", "urn:schemas-microsoft-com:office:spreadsheet", "Solid")

        '---------------- CENTER/GREEN titles ---------------'
        oStyle = AddXMLChild(oStyles, "Style")
        oStyle.SetAttribute("ID", "urn:schemas-microsoft-com:office:spreadsheet", "sCenterGreen")

        oAlignment = AddXMLChild(oStyle, "Alignment")
        oAlignment.SetAttribute("Horizontal", "urn:schemas-microsoft-com:office:spreadsheet", "Center")

        oInterior = AddXMLChild(oStyle, "Interior")
        oInterior.SetAttribute("Color", "urn:schemas-microsoft-com:office:spreadsheet", "#99CC00")
        oInterior.SetAttribute("Pattern", "urn:schemas-microsoft-com:office:spreadsheet", "Solid")


        '''' add the namespace ''''
        oNS.AddNamespace("", "urn:schemas-microsoft-com:office:spreadsheet")
        oNS.AddNamespace("o", "urn:schemas-microsoft-com:office:office")
        oNS.AddNamespace("x", "urn:schemas-microsoft-com:office:excel")
        oNS.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet")
        oNS.AddNamespace("html", "http://www.w3.org/TR/REC-html40")

        Return oDoc
    End Function
#End Region

End Class