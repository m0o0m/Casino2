Imports System
Imports System.Drawing
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DateAndTime
Imports System.Web.Security
Imports System.Text
Imports System.Xml
Imports System.Collections.Generic
Imports System.Linq

Public Class CSBCStd


    ''' <summary>
    ''' Trace a log4net.Debug message
    ''' </summary>
    ''' <param name="poLogger">log4net logger</param>
    ''' <param name="psMessage">The message to trace.</param>
    Public Shared Sub LogDebug(ByVal poLogger As log4net.ILog, ByVal psMessage As String)
        poLogger.Debug(psMessage)
    End Sub

    ''' <summary>
    ''' Trace a log4net.Info message
    ''' </summary>
    ''' <param name="poLogger">log4net logger</param>
    ''' <param name="psMessage">The message to trace.</param>
    Public Shared Sub LogInfo(ByVal poLogger As log4net.ILog, ByVal psMessage As String)
        poLogger.Info(psMessage)
    End Sub

    ''' <summary>
    ''' Write a log4net.Info message with Critical level
    ''' </summary>
    ''' <param name="poLogger">log4net logger</param>
    ''' <param name="psMessage">The message to custom log.</param>
    ''' <param name="poError">The exception thrown</param>
    Public Shared Sub LogCritical(ByVal poLogger As log4net.ILog, ByVal psMessage As String, Optional ByVal poError As Exception = Nothing)
        poLogger.Logger.Log(poLogger.GetType, log4net.Core.Level.Critical, psMessage, poError)
    End Sub

    ''' <summary>
    ''' Trace a log4net.Warn message
    ''' </summary>
    ''' <param name="poLogger">log4net logger</param>
    ''' <param name="psMessage">The action we were attempting to do that caused this error or a general warning message.</param>
    ''' <param name="poError">The exception thrown</param>
    Public Shared Sub LogWarn(ByVal poLogger As log4net.ILog, ByVal psMessage As String, Optional ByVal poError As Exception = Nothing)
        If poError Is Nothing Then
            poLogger.Warn(psMessage)
        Else
            poLogger.Warn(psMessage & ": " & poError.Message, poError)
        End If
    End Sub

    ''' <summary>
    ''' Trace a log4net.Error message
    ''' </summary>
    ''' <param name="poLogger">log4net logger</param>
    ''' <param name="psCurrentActivity">The action we were attempting to do that caused this error</param>
    ''' <param name="poError">The exception thrown</param>
    Public Shared Sub LogError(ByVal poLogger As log4net.ILog, ByVal psCurrentActivity As String, ByVal poError As Exception)
        poLogger.Error(psCurrentActivity & ": " & poError.Message, poError)
    End Sub

    ''' <param name="pnTimeZone">
    ''' The STANDARD GMT offset of the timezone. Ex: PST=-8
    ''' </param>
    ''' <param name="pbEncloseInQuotes">
    ''' Pass this value as true if you want each state to be enclosed in single quotes.
    ''' Very useful so you can just call Join() on the array and insert directly into an SQL query
    ''' </param>
    ''' <returns></returns>
    Public Shared Function getStatesByTimezone(ByVal pnTimeZone As Integer, Optional ByVal pbEncloseInQuotes As Boolean = False) As ArrayList
        Dim oList As New ArrayList()

        Dim sStates As String = ""

        Select Case pnTimeZone
            Case -11
                sStates = "HI"

            Case -9
                sStates = "AK"

            Case -8
                sStates = "CA,OR,NV,WA"

            Case -7
                sStates = "AZ,CO,ID,MT,NM,UT,WY"

            Case -6
                sStates = "AL,AK,IA,IL,KS,LA,MN,MS,MO,NE,ND,OK,SD,TX,WI"

            Case -5
                sStates = "CT,DE,FL,GA,IN,KY,MA,ME,MD,MI,NC,NH,NJ,NY,OH,PA,RI,SC,TN,VA,VT,WV"

            Case Else
                Return oList
        End Select

        oList.AddRange(sStates.Split(","c))

        If pbEncloseInQuotes Then
            Dim oIEnumStates As IEnumerable(Of String) = From oState In oList Order By oState Select SQLString(oState)

            Return New ArrayList(oIEnumStates.ToArray())
        End If

        Return oList
    End Function

    Public Shared Function UpperCaseOnlyFirstLetter(ByVal psWord As String) As String
        Dim sWord As String = LCase(psWord)
        If sWord <> "" Then
            sWord = sWord(0).ToString.ToUpper & sWord.Substring(1)
        End If

        Return sWord
    End Function
    Public Shared Sub UppercaseAllControls(ByVal poParentControl As Control)
        If TypeOf poParentControl Is TextBox Then
            CType(poParentControl, TextBox).Text = CType(poParentControl, TextBox).Text.ToUpper
        ElseIf TypeOf poParentControl Is HtmlInputText Then
            CType(poParentControl, HtmlInputText).Value = CType(poParentControl, HtmlInputText).Value.ToUpper
        End If

        For Each oChild As Control In poParentControl.Controls
            UppercaseAllControls(oChild)
        Next
    End Sub

    Public Shared Sub EnableAllControl(ByVal poParentControl As Control, ByVal pblnEnable As Boolean)
        If TypeOf poParentControl Is TextBox Then
            CType(poParentControl, TextBox).ReadOnly = Not pblnEnable Or CType(poParentControl, TextBox).ReadOnly
        ElseIf TypeOf poParentControl Is HtmlInputText Then
            CType(poParentControl, HtmlInputText).Disabled = Not pblnEnable Or CType(poParentControl, HtmlInputText).Disabled
        ElseIf TypeOf poParentControl Is Button Then
            CType(poParentControl, Button).Enabled = pblnEnable And CType(poParentControl, Button).Enabled
        ElseIf TypeOf poParentControl Is HtmlButton Then
            CType(poParentControl, HtmlButton).Disabled = Not pblnEnable Or CType(poParentControl, HtmlButton).Disabled
        ElseIf TypeOf poParentControl Is CDropDownList Then
            CType(poParentControl, CDropDownList).Enabled = pblnEnable And CType(poParentControl, CDropDownList).Enabled
        ElseIf TypeOf poParentControl Is DropDownList Then
            CType(poParentControl, DropDownList).Enabled = pblnEnable And CType(poParentControl, DropDownList).Enabled
        ElseIf TypeOf poParentControl Is CheckBox Then
            CType(poParentControl, CheckBox).Enabled = pblnEnable And CType(poParentControl, CheckBox).Enabled
        ElseIf TypeOf poParentControl Is ImageButton Then
            CType(poParentControl, ImageButton).Enabled = pblnEnable And CType(poParentControl, ImageButton).Enabled
        ElseIf TypeOf poParentControl Is RadioButton Then
            CType(poParentControl, RadioButton).Enabled = pblnEnable And CType(poParentControl, RadioButton).Enabled
        ElseIf TypeOf poParentControl Is RadioButtonList Then
            CType(poParentControl, RadioButtonList).Enabled = pblnEnable And CType(poParentControl, RadioButtonList).Enabled
        ElseIf TypeOf poParentControl Is CheckBoxList Then
            CType(poParentControl, CheckBoxList).Enabled = pblnEnable And CType(poParentControl, CheckBoxList).Enabled
        ElseIf TypeOf poParentControl Is HtmlInputCheckBox Then
            CType(poParentControl, HtmlInputCheckBox).Disabled = Not pblnEnable Or CType(poParentControl, HtmlInputCheckBox).Disabled
        ElseIf TypeOf poParentControl Is HtmlTextArea Then
            CType(poParentControl, HtmlTextArea).Disabled = Not pblnEnable Or CType(poParentControl, HtmlTextArea).Disabled
        ElseIf TypeOf poParentControl Is FileUpload Then
            CType(poParentControl, FileUpload).Enabled = pblnEnable And CType(poParentControl, FileUpload).Enabled
        ElseIf TypeOf poParentControl Is AjaxControlToolkit.CalendarExtender Then
            CType(poParentControl, AjaxControlToolkit.CalendarExtender).Enabled = pblnEnable And CType(poParentControl, AjaxControlToolkit.CalendarExtender).Enabled
        End If

        For Each oChild As Control In poParentControl.Controls
            EnableAllControl(oChild, pblnEnable)
        Next
    End Sub

    Public Shared Function SafePhone(ByVal psPhone As String) As String
        Dim sPhone As String = System.Text.RegularExpressions.Regex.Replace(SafeString(psPhone), "[^0-9]", "")
        If sPhone.StartsWith("1") And sPhone.Length > 10 Then
            sPhone = sPhone.Substring(1)
        End If

        Return Left(sPhone, 10)
        'Return Left(SafeString(sPhone).Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace("+", ""), 10)
    End Function

    Public Shared Function FormatPhoneNumber(ByVal psUnformattedPhone As String) As String
        Return System.Text.RegularExpressions.Regex.Replace(SafePhone(psUnformattedPhone), _
                                       "(\d{3})(\d{3})(\d{4})", _
                                       "$1-$2-$3")
    End Function

    Public Shared Function GetAreaCodePhone(ByVal psPhoneNumber As String) As String
        If psPhoneNumber.Trim.Length <= 3 Then
            Return psPhoneNumber
        End If
        Return Left(psPhoneNumber, 3)
    End Function

    Public Shared Function GetWithoutAreaCodePhone(ByVal psPhoneNumber As String) As String
        If psPhoneNumber.Trim.Length <= 7 Then
            Return psPhoneNumber
        End If
        Return Right(psPhoneNumber, 7)
    End Function

    Public Shared Function FormatSSN(ByVal psUnformattedSSN As String) As String
        Return System.Text.RegularExpressions.Regex.Replace(psUnformattedSSN, _
                                       "(\d{3})(\d{2})(\d{4})", _
                                       "$1-$2-$3")
    End Function

    Public Shared Function getPageFileName() As String
        Dim sFileName As String = HttpContext.Current.Request.Url.AbsolutePath
        Dim nIndex As Integer = sFileName.LastIndexOf("/")
        If nIndex > -1 And nIndex < sFileName.Length - 1 Then
            Return sFileName.Substring(nIndex + 1)
        Else
            Return sFileName
        End If
    End Function

    '_pbFile  is set when page is loaded cause we can't do 
    'retrieve from the request in the onunload function anymore - therefore, unless we set it and 
    'store it, we won't b able to trace in the onunload function
    'we didn't use "trace" as func name to prevent confusion
    Private Shared _pbFile As String
    Private Shared _pbContext As String
    Public Overloads Shared Sub pbTrace(ByVal sType As String, ByVal s As String)
        If IsNothing(s) = False Then
            s = s.Replace("%", "<PERCENT>")
        End If

        Try
            _pbFile = HttpContext.Current.Request.Url.AbsolutePath
        Catch ex As Exception
            _pbFile = "NOT AVAIL"
        End Try
        Try
            _pbContext = HttpContext.Current.User.Identity.Name
        Catch ex As Exception
            _pbContext = "NOT AVAIL"
        End Try


        'Dim g_oPB As PaulBunyan.PBLogger
        'g_oPB = New PaulBunyan.PBLogger()
        'g_oPB = CreateObject("PB.Logger.1")
        'g_oPB.Component = "WWW"
        'g_oPB.File = _pbFile    '"SOME FILE"
        'g_oPB.Context = _pbContext    '"UKNOWN CONTEXT"
        'g_oPB.Module = "X MODULE"
        'g_oPB.Log(sType, s)
    End Sub

    ''convenient helper function to obtain the current pages connection
    'Public Shared Function openConnectionShared() As System.Data.SqlClient.SqlConnection
    '    Dim oGlobal As CGlobalPage
    '    oGlobal = CType(HttpContext.Current.Handler, CGlobalPage)
    '    Return CType(oGlobal.openConnection(), SqlConnection)
    'End Function


    ''convenient helper function to obtain the current pages connection
    'Public Shared Function openConnectionTransactedShared() As System.Data.SqlClient.SqlConnection
    '    Dim oGlobal As CGlobalPage
    '    oGlobal = CType(HttpContext.Current.Handler, CGlobalPage)
    '    Return CType(oGlobal.openConnectionTransacted(), SqlConnection)
    'End Function

    Public Shared Function SQLString(ByVal s As Object) As String
        If s Is Nothing Then
            Return "null"
        End If
        Return "'" & SafeString(s).Replace("'", "''") & "'"
    End Function

    Public Shared Function SQLDouble(ByVal s As Object) As String
        Return SafeString(SafeDouble(s))
    End Function

    Public Shared Function SafeDate(ByVal s As Object) As Date
        If IsDate(s) Then
            Return CDate(s)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function SafeString(ByVal s As Object) As String
        ' special case to handle Recordset Field objects
        If s Is Nothing OrElse TypeOf s Is DBNull Then
            Return ""
        ElseIf TypeOf s Is Date AndAlso CDate(s) = Date.MinValue Then
            Return ""
        ElseIf TypeOf s Is System.Web.UI.WebControls.ListItem Then
            Return CType(s, System.Web.UI.WebControls.ListItem).Value
        ElseIf TypeOf s Is Guid Then
            Return s.ToString()
        End If

        Return Trim(CStr(s)) 'very odd: if u return trim(s.toString()), then for dates, it will append 12:00:00 AM if there is no time :-T
    End Function

    Public Shared Function SafeLong(ByVal s As Object) As Long
        Dim s2 As String = SafeString(s)

        If s2 = "" Then
            Return 0
        ElseIf IsNumeric(s2) Then
            SafeLong = CLng(s2)
        Else
            SafeLong = 0
        End If
    End Function

    Public Shared Function SafeInteger(ByVal s As Object) As Integer
        Dim s2 As String = SafeString(s)

        If s2 = "" Then
            Return 0
        ElseIf IsNumeric(s2) Then
            SafeInteger = CInt(s2)
        Else
            SafeInteger = 0
        End If
    End Function

    Public Shared Function SafeBoolean(ByVal s As Object) As Boolean
        Dim s2 As String = UCase(SafeString(s))

        Select Case s2
            Case "Y", "YES", "TRUE"
                Return True
        End Select

        Return False
    End Function

    Public Shared Function SafeDouble(ByVal s As Object) As Double
        Dim s2 As String = SafeString(s)

        If IsNumeric(s2) Then
            Return CDbl(s2)
        Else
            Return 0
        End If
    End Function

    Public Shared Function SafeSingle(ByVal s As Object) As Single
        Dim s2 As String = SafeString(s)

        If IsNumeric(s2) Then
            Return CSng(s2)
        Else
            Return 0
        End If
    End Function

    Public Enum eSetTypes
        eSetByText
        eSetByValue
    End Enum

    'most web controls only contain 1 value that we're interested in, this function 
    'returns the text/value of that selected item 
    'normally we'd call this when saving to dataDSL
    Public Shared Function SafeListItemVal(ByVal theList As System.Web.UI.WebControls.ListControl, _
     Optional ByVal sDefault As String = "", Optional ByVal byTextVal As eSetTypes = eSetTypes.eSetByValue) As String
        If theList.SelectedItem Is Nothing Then
            Return sDefault
        End If

        If byTextVal = eSetTypes.eSetByText Then
            Return theList.SelectedItem.Text
        Else
            Return theList.SelectedItem.Value
        End If
    End Function

    'given a bunch of listItems and a given value, this function will return that item's index
    '@unselectAll - true we should auto unselect everything -- since most of the time, we will 
    'reselect the last the item[index] after this function finish!
    'we'd normally call this when loading data from db to list control 
    Public Shared Function getItemIndex(ByVal theList As System.Web.UI.WebControls.ListControl, _
     ByVal sTheVal As String, Optional ByVal sDefault As String = "", Optional ByVal byTextVal As eSetTypes = eSetTypes.eSetByValue, _
     Optional ByVal unselectAll As Boolean = True) As Integer

        If sTheVal = "" Then
            sTheVal = sDefault
        End If

        If unselectAll Then
            'unselect everything 
            Dim iter As ListItem
            For Each iter In theList.Items
                iter.Selected = False
            Next
        End If

        Dim theItem As ListItem = Nothing
        Select Case byTextVal
            Case eSetTypes.eSetByText
                theItem = theList.Items.FindByText(sTheVal)
            Case eSetTypes.eSetByValue
                theItem = theList.Items.FindByValue(sTheVal)
        End Select

        If Not theItem Is Nothing Then
            Return theList.Items.IndexOf(theItem)
        Else
            Return -1
        End If
    End Function

    Public Shared Function getScalerValueShared(ByVal sSQL As String, ByVal oConn As IDbConnection) As String
        Dim cmd As New SqlCommand(sSQL, CType(oConn, SqlConnection))
        Return SafeString(cmd.ExecuteScalar())
    End Function

    'Private Shared _dr As SqlClient.SqlDataReader
    'Public Shared Function getDataReaderShared(ByVal sSQL As String) As SqlClient.SqlDataReader
    '    'Return getDataReader(sSQL, openConnectionShared())
    '    Dim oGlobalPage As CGlobalPage = CType(HttpContext.Current.Handler, CGlobalPage)
    '    Return CType(oGlobalPage.getDataReader(sSQL), SqlDataReader)
    'End Function

    Public Shared Function getDataReaderShared(ByVal sSQL As String, ByVal dbConn As IDbConnection) As System.Data.SqlClient.SqlDataReader
        Dim result As SqlDataReader
        Try
            Dim cmdSelect As New Data.SqlClient.SqlCommand(sSQL, CType(dbConn, SqlConnection))
            result = cmdSelect.ExecuteReader
        Catch ex As Exception
            pbTrace("WARN", "Unable to obtain data reader DSLd on sql:" & sSQL)
            Throw ex
        End Try

        Return result
    End Function

    Public Shared Function getDataTableShared(ByVal sSQL As String, ByVal dbConn As IDbConnection) As System.Data.DataTable
        Dim cmdSelect As New SqlCommand(sSQL, CType(dbConn, SqlConnection))
        Dim dsSelect As New DataSet()
        Dim daSelect As IDataAdapter
        Try
            daSelect = New SqlDataAdapter(cmdSelect)
            daSelect.Fill(dsSelect)
        Catch ex As Exception
            pbTrace("WARN", "Unable to obtain data table DSLd on sql:" & sSQL)
            Throw ex
        End Try
        Return dsSelect.Tables(0)
    End Function

    'Public Shared Function getDataTableShared(ByVal sSQL As String) As System.Data.DataTable
    '    Dim oGlobalPage As CGlobalPage = CType(HttpContext.Current.Handler, CGlobalPage)
    '    Return oGlobalPage.getDataTable(sSQL)
    'End Function


    'returns nicer 32 char GUID w/ no '-'
    Public Shared Function newGUID() As String
        Return System.Guid.NewGuid.ToString()
    End Function

    Public Shared Function getThisFileName() As String
        'returns a path and appends a \
        Dim slashForward As Long, slashBack As Long 'For = /, Back = \
        Dim strFullFilePath As String
        strFullFilePath = HttpContext.Current.Request.Url.AbsoluteUri
        slashForward = strFullFilePath.LastIndexOf("/")
        slashBack = strFullFilePath.LastIndexOf("\")

        Dim sTemp As String = strFullFilePath.Substring(CInt(IIf(slashForward > slashBack, slashForward, slashBack)) + 1)
        Dim arrResult() As String
        'need to get rid of the trailing url perimeters follow by the question mark
        arrResult = Split(sTemp, "?")
        Return arrResult(0)
    End Function

    Public Overloads Shared Function isPostBackFrom(ByVal oControl As Control) As Boolean
        '			traceA("clientid:" & oControl.ClientID)			
        '			traceA("event targ:" & HttpContext.Current.Request("__EVENTTARGET"))
        Dim eventTarget As String = HttpContext.Current.Request("__EVENTTARGET")
        If eventTarget = oControl.ClientID Or HttpContext.Current.Request(oControl.ClientID) <> String.Empty Then
            '		traceA("ispostback return true")
            Return True
        Else
            '	traceA("ispostback return false")
            Return False
        End If
    End Function

    'returns true if postback is from ANY ONE of the parameters
    Public Overloads Shared Function isPostBackFrom(ByVal ParamArray oControls() As Control) As Boolean
        Dim ctl As Control
        For Each ctl In oControls
            If isPostBackFrom(ctl) Then
                Return True
            End If
        Next
        Return False
    End Function

    '********************************************************************************
    '* Name: getSQLDateRange
    '*
    '* Description: returns partial SQL query statement that retrieves records between sStartDate and sEndDate (invlusive!)
    '* Parameters: sFieldName - required string specifying field name
    '*             sStartDate - optional starting date ;  empty/invalid sStartDate omits Starting date from SQL query
    '*             sEndDate   - optional ending date   ;  empty/invalid sStartDate omits ending date from SQL query
    '*             DBType     -  "DB_ACCESS" OR "DB_ODBC"
    '* Sample Usage: ? getSQLDateRange("DateCreated", "6/29/00", "7/3/00", 0) -> (DateCreated >= '6/29/00' AND DateCreated <= '07/03/00 11:59:59 PM')
    '*               ? getSQLDateRange("DateCreated", "6/29/00", "", 0)       ->(DateCreated >= '6/29/00')
    '*               ? getSQLDateRange("DateCreated", "", "", 0)              -> (blank)
    '*               ? getSQLDateRange("DateCreated", "", "2/2/00", 0)        -> (DateCreated <= '2/2/00 11:59:59 PM')
    '* Created: 7/5/00 1:04:54 PM
    '********************************************************************************
    Public Enum eDBTypes
        eDBType_ODBC
        eDBType_ACCESSS
    End Enum

    Public Overloads Shared Function getSQLDateRange(ByVal sFieldName As String, ByVal sStartDate As String, ByVal sEndDate As String, Optional ByVal pbReserveTime As Boolean = False, Optional ByVal DBType As eDBTypes = eDBTypes.eDBType_ODBC) As String
        Dim dStartDate As Date
        Dim dEndDate As Date

        If sStartDate = "" OrElse Not IsDate(sStartDate) Then
            dStartDate = #1/1/1753# 'SqlDateTime.MinValue.Value
        Else
            dStartDate = CDate(sStartDate)
            If dStartDate.Year < 1753 Then dStartDate = #1/1/1753#
        End If

        If sEndDate = "" OrElse Not IsDate(sEndDate) Then
            dEndDate = #1/1/9999#
        Else
            dEndDate = CDate(sEndDate)
            If dEndDate.Year < 1753 Then dEndDate = #1/1/1753#
        End If

        Return getSQLDateRange(sFieldName, dStartDate, dEndDate, pbReserveTime, DBType)
    End Function

    Public Overloads Shared Function getSQLDateRange(ByVal sFieldName As String, Optional ByVal sStartDate As Date = #1/1/1000#, Optional ByVal sEndDate As Date = #1/1/9999#, Optional ByVal pbReserveTime As Boolean = False, Optional ByVal DBType As eDBTypes = eDBTypes.eDBType_ODBC) As String
        Dim sSQL As String = ""
        Dim sSDate As String = ""
        Dim sEDate As String = ""
        Dim sDelim As String = ""
        CAssert.Assert(DBType = eDBTypes.eDBType_ODBC Or DBType = eDBTypes.eDBType_ACCESSS, "Invalid DBType:" & DBType)

        sDelim = CStr(IIf(DBType = eDBTypes.eDBType_ACCESSS, "#", "'"))

        If pbReserveTime Then
            sSDate = SafeString(sStartDate)
            sEDate = SafeString(sEndDate)
        Else
            sSDate = FormatDateTime(sStartDate, DateFormat.ShortDate)
            sEDate = FormatDateTime(sEndDate, DateFormat.ShortDate) & " 11:59:59 PM"
        End If

        If sStartDate <> #1/1/1000# And IsDate(sStartDate) Then
            sSQL = sFieldName & " >= " & sDelim & sSDate & sDelim
        End If

        If sEndDate <> #1/1/9999# And IsDate(sEndDate) Then
            If Len(sSQL) > 0 Then
                sSQL = sSQL & " AND " & sFieldName & " <= " & sDelim & sEDate & sDelim
            Else
                sSQL = sSQL & sFieldName & " <= " & sDelim & sEDate & sDelim
            End If
        End If

        If Len(sSQL) > 0 Then sSQL = "(" & sSQL & ")"
        Return sSQL
    End Function

    Public Overloads Shared Function DSDate(ByVal sDate As Object) As Object
        If IsDate(sDate) AndAlso SafeDate(sDate).Year > 1900 Then
            Return CDate(sDate)
        Else
            Return DBNull.Value
        End If
    End Function
    Public Overloads Shared Function SQLDate(ByVal sDate As String) As String
        If IsDate(sDate) Then
            Return SQLDate(CDate(sDate))
        Else
            Return "NULL"
        End If
    End Function
    Public Overloads Shared Function SQLDate(ByVal theDate As Date) As String
        If theDate = Date.MinValue Then
            Return "NULL"
        Else
            Return SQLString(theDate)
        End If
    End Function

    Public Shared Sub ClientAlert(ByVal psMessage As String, Optional ByVal pbIsAjax As Boolean = False)
        Dim sMsg As String = Replace(psMessage, vbCrLf, "\n")
        'sMsg = Replace(sMsg, "\", "\\")
        'sMsg = Replace(sMsg, "'", "\'")
        RunJS(psMessage, String.Format("alert(""{0}"");", sMsg), pbIsAjax)
    End Sub

    Public Shared Sub ClientRedirect(ByVal psURL As String, Optional ByVal pbIsAjax As Boolean = False)
        RunJS(psURL, String.Format("window.location= '{0}';", psURL), pbIsAjax)
    End Sub

    Public Shared Sub OpenPopupWindow(ByVal psURL As String, Optional ByVal pnWidth As Integer = -1, _
                                        Optional ByVal pnHeight As Integer = -1, _
                                        Optional ByVal psWindowName As String = "", _
                                        Optional ByVal pbStatusBar As Boolean = True, _
                                        Optional ByVal pbToolBar As Boolean = True, _
                                        Optional ByVal pbLocation As Boolean = True, _
                                        Optional ByVal pbMenuBar As Boolean = True, _
                                        Optional ByVal pbResizable As Boolean = True, _
                                        Optional ByVal pbScrollbars As Boolean = True, _
                                        Optional ByVal pbCenterWindow As Boolean = True, _
                                        Optional ByVal pbIsAjax As Boolean = False)


        CAssert.Assert(pbCenterWindow = False Or (pnHeight >= 0 And pnWidth >= 0), _
                        "Height and width must be specified to center the window.")

        Dim sScript As New System.Text.StringBuilder()

        With sScript
            If pbCenterWindow Then
                .Append("var centerWidth = (window.screen.width - " & pnWidth & ") / 2;")
                .Append("var centerHeight = (window.screen.height - " & pnHeight & ") / 2;")
            End If
            .Append("window.open('" & psURL & "','" & psWindowName & "','")
            .Append("status=" & IIf(pbStatusBar, "1", "0").ToString & ",")
            .Append("toolbar=" & IIf(pbToolBar, "1", "0").ToString & ",")
            .Append("location=" & IIf(pbLocation, "1", "0").ToString & ",")
            .Append("menubar=" & IIf(pbMenuBar, "1", "0").ToString & ",")
            .Append("resizable=" & IIf(pbResizable, "1", "0").ToString & ",")
            .Append("scrollbars=" & IIf(pbScrollbars, "1", "0").ToString)
            If pnWidth >= 0 Then .Append(",width=" & pnWidth)
            If pnHeight >= 0 Then .Append(",height=" & pnHeight)
            .Append("');")
        End With

        RunJS(psURL, sScript.ToString(), pbIsAjax)
    End Sub

    Public Shared Sub RunJS(ByVal psKey As String, ByVal psScript As String, Optional ByVal pbIsAjax As Boolean = False)
        Dim thePage As Page = CType(HttpContext.Current.Handler, Page)
        If thePage.ClientScript.IsClientScriptBlockRegistered(GetType(CSBCStd), psKey) Then
            Exit Sub
        End If

        If pbIsAjax Then
            ScriptManager.RegisterStartupScript(thePage, GetType(CSBCStd), psKey, String.Format("<script type=""text/javascript"" language=""javascript"">{0}</script>", psScript), False)
        Else
            thePage.ClientScript.RegisterStartupScript(GetType(CSBCStd), psKey, String.Format("<script type=""text/javascript"" language=""javascript"">{0}</script>", psScript))
        End If
    End Sub

    Public Shared Sub closeWindow(Optional ByVal alertMsg As String = "")
        Dim thePage As Page = CType(HttpContext.Current.Handler, Page)
        'thePage.ClientScript.RegisterStartupScript(Me.GetType(),"CLOSE_WHOLE_WINDOW", "<script>self.opener=null;self.close();</script>")
        If alertMsg <> "" Then
            thePage.Response.Write("<script>alert(""" & alertMsg & """)</script>")
        End If
        thePage.Response.Write("<script>self.opener=null;self.close();</script>")
    End Sub

    Public Shared Function getFirstDayOfMonth(ByVal sDate As DateTime) As DateTime
        Dim dateNow As DateTime = sDate
        Dim tempMonth As Integer, tempYear As Integer, tempDay As Integer

        tempMonth = DatePart(DateInterval.Month, dateNow)
        tempDay = 1
        tempYear = DatePart(DateInterval.Year, dateNow)
        dateNow = New DateTime(tempYear, tempMonth, tempDay)
        Return dateNow
    End Function

    Public Shared Function getLastDayOfMonth(ByVal sDate As DateTime) As DateTime
        Dim dateNow As DateTime = getFirstDayOfMonth(sDate)
        Return dateNow.AddMonths(1).AddDays(-1)
    End Function

    Public Shared Function AgeNow(ByVal DOB As Date) As Double
        'returns current age given dob
        'function also accounts for leap years by dividing by 365.25
        'ie: Print AgeNow(#10/2/80#) <-- note the #!
        ' date returns in the form as above
        AgeNow = Int(DateDiff("y", DOB, Date.Now) / 365.25)
    End Function


    Public Shared Function webPostString(ByVal oDoc As XmlDocument, ByVal sURL As String) As String
        Dim stm As System.IO.Stream
        stm = webPost(oDoc, sURL)
        Dim stmReader As New System.IO.StreamReader(stm)
        Dim sContent As String = stmReader.ReadToEnd
        stmReader.Close()
        stm.Close()


        Return sContent
    End Function

    'TODO: add support for form style request via POST method (http://samples.gotdotnet.com/quickstart/util/srcview.aspx?path=/quickstart/howto/samples/net/WebRequests/clientPOST.src)
    Public Shared Function webPost(ByVal oDoc As XmlDocument, ByVal sURL As String) As System.IO.Stream
        Dim req As System.Net.WebRequest = System.Net.WebRequest.Create(sURL)
        req.Method = "POST"

        'send the data out
        Dim ms As System.IO.MemoryStream = New System.IO.MemoryStream()
        Dim tw As System.Xml.XmlTextWriter = New System.Xml.XmlTextWriter(ms, System.Text.Encoding.GetEncoding("utf-8"))
        oDoc.WriteTo(tw)
        tw.Flush()
        req.ContentLength = ms.Length
        Dim stm As System.IO.Stream = req.GetRequestStream()
        stm.Write(ms.GetBuffer(), 0, CInt(ms.Length))
        stm.Close()

        ms.Close()
        tw.Close()

        'read the data back in
        Dim resp As System.Net.HttpWebResponse = CType(req.GetResponse(), System.Net.HttpWebResponse)
        Return resp.GetResponseStream()
    End Function

    Public Enum etTRUNCATE_AREA
        etTRUNCATE_END = 1
        etTRUNCATE_FRONT = 2
        etTRUNCATE_BOTH = 3
    End Enum
    Public Shared Function truncate(ByRef sInput As String, ByRef sTrailer As String, Optional ByVal truncateArea As etTRUNCATE_AREA = etTRUNCATE_AREA.etTRUNCATE_END) As String
        '********************************************************************************
        '* Name: truncate
        '*
        '* Description: takes input string like "option1 AND option2 AND" and removes the last AND
        '*              if last AND didn't exist, then returns sInput
        '* Parameters: sInput - string to truncate
        '*             sTrailer - last characters to truncate
        '* Sample Usage:
        '* Created: 12/21/2000 12:46:52 PM
        '********************************************************************************
        If truncateArea = etTRUNCATE_AREA.etTRUNCATE_END OrElse truncateArea = etTRUNCATE_AREA.etTRUNCATE_BOTH Then
            If Right(sInput, Len(sTrailer)) = sTrailer Then
                truncate = Mid(sInput, 1, Len(sInput) - Len(sTrailer))
            Else
                truncate = sInput
            End If
        End If

        If truncateArea = etTRUNCATE_AREA.etTRUNCATE_FRONT OrElse truncateArea = etTRUNCATE_AREA.etTRUNCATE_BOTH Then
            If Left(sInput, Len(sTrailer)) = sTrailer Then
                truncate = Mid(sInput, Len(sTrailer) + 1)
            Else
                truncate = sInput
            End If
        End If

        Return sInput
    End Function

    Public Shared Function getCreditScoreByRating(ByVal pIntRating As Integer, Optional ByVal ShiftValue As Boolean = False) As Integer
        Dim score As Integer = 0

        If ShiftValue = True Then pIntRating = pIntRating - 1

        Select Case pIntRating
            Case 0  'perfect
                score = 720
            Case 1  'excellent
                score = 680
            Case 2  'good
                score = 640
            Case 3  'fair
                score = 580
            Case 4  'poor
                score = 550
            Case 5       'I don't know
                score = 640
        End Select
        Return score
    End Function

    Public Shared Function WebPost(ByVal poArguments As List(Of DictionaryEntry), ByVal psURL As String) As String

        Dim oStrBuilder As New System.Text.StringBuilder()
        For Each oItem As DictionaryEntry In poArguments
            oStrBuilder.Append(SafeString(oItem.Key) & "=" & SafeString(oItem.Value))
            If poArguments.IndexOf(oItem) < poArguments.Count - 1 Then
                oStrBuilder.Append("&")
            End If
        Next

        Dim oEncoding As New System.Text.ASCIIEncoding()
        Dim oData As Byte() = oEncoding.GetBytes(oStrBuilder.ToString())

        Dim oRequest As System.Net.HttpWebRequest = CType(System.Net.WebRequest.Create(psURL), System.Net.HttpWebRequest)
        oRequest.Method = "POST"
        oRequest.ContentType = "application/x-www-form-urlencoded"
        oRequest.ContentLength = oData.Length

        Dim oStream As System.IO.Stream = oRequest.GetRequestStream()
        oStream.Write(oData, 0, oData.Length)
        oStream.Close()

        Dim oReader As New System.IO.StreamReader(oRequest.GetResponse().GetResponseStream())
        Return oReader.ReadToEnd()
    End Function

    Public Shared Function webGet(ByVal sURL As String) As String
        Dim req As System.Net.WebRequest = System.Net.WebRequest.Create(sURL)
        req.Method = "GET"

        'read the data back in
        Dim resp As System.Net.HttpWebResponse = CType(req.GetResponse(), System.Net.HttpWebResponse)
        Dim strmReader As New System.IO.StreamReader(resp.GetResponseStream())

        Return strmReader.ReadToEnd()
    End Function

    Public Shared Function EncryptToHash(ByVal psValueToEncrypt As String) As String
        psValueToEncrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(psValueToEncrypt, "SHA1")
        Return psValueToEncrypt
    End Function

    Private Shared Function CreateImageFromString(ByVal sImageText As String) As System.Drawing.Bitmap
        Dim bmpImage As New System.Drawing.Bitmap(1, 1)

        Dim iWidth As Integer = 0
        Dim iHeight As Integer = 0

        'Create the Font object for the image text drawing.
        Dim MyFont As New System.Drawing.Font("Verdana", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)

        'Create a graphics object to measure the text's width and height.
        Dim MyGraphics As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmpImage)

        'This is where the bitmap size is determined.
        iWidth = CInt(MyGraphics.MeasureString(sImageText, MyFont).Width)
        iHeight = CInt(MyGraphics.MeasureString(sImageText, MyFont).Height)

        'Create the bmpImage again with the correct size for the text and font.
        bmpImage = New System.Drawing.Bitmap(bmpImage, New Size(iWidth, iHeight))

        'Add the colors to the new bitmap.
        MyGraphics = System.Drawing.Graphics.FromImage(bmpImage)
        MyGraphics.Clear(System.Drawing.Color.White)
        MyGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        MyGraphics.DrawString(sImageText, MyFont, New System.Drawing.SolidBrush(System.Drawing.Color.Gray), 0, 0)
        MyGraphics.Flush()

        Return bmpImage
    End Function

    Public Shared Function GetClientIP() As String
        Dim sIPAddress As String = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
        If sIPAddress = "" Then
            sIPAddress = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        End If
        Return sIPAddress
    End Function

    Public Shared Function CanUpload(ByVal psContentType As String) As Boolean
        Select Case psContentType
            Case "image/bmp"                    '.bmp
                Return True
            Case "image/pjpeg", "image/jpeg"    '.jpg
                Return True
            Case "image/gif"                    '.gif
                Return True
            Case "image/x-png", "image/png"     '.png
                Return True
            Case "image/x-icon"                 '.ico
                Return True
            Case "application/pdf"              '.pdf
                Return True
            Case "application/msword"           '.doc
                Return True
            Case "application/vnd.ms-excel"     '.xls
                Return True
            Case "audio/mpeg"                   '.mp3
                Return True
            Case "application/kapsulefile"      '.mp3
                Return True
            Case "audio/wav"                    '.wav
                Return True
            Case "application/vnd.openxmlformats-officedocument.wordprocessingml.document" '.docx
                Return True
            Case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"       '.xlsx
                Return True
        End Select
        Return False
    End Function

    Public Shared Function GetDocumentSuffix(ByVal psContentType As String) As String
        Select Case psContentType
            Case "image/bmp"
                Return ".bmp"
            Case "image/pjpeg", "image/jpeg"
                Return ".jpg"
            Case "image/gif"
                Return ".gif"
            Case "image/x-png", "image/png"
                Return ".png"
            Case "image/x-icon"
                Return ".ico"
            Case "application/pdf"
                Return ".pdf"
            Case "application/msword"
                Return ".doc"
            Case "application/vnd.ms-excel"
                Return ".xls"
            Case "audio/mpeg"
                Return ".mp3"
            Case "audio/wav"
                Return ".wav"
        End Select
        Return ""
    End Function

    Public Shared Function HideCreditCardNumber(ByVal psCreditCardNumber As String) As String
        Dim sCreditCardNumber As String = SafeString(psCreditCardNumber)
        If sCreditCardNumber.Length > 4 Then
            Dim i As Integer = 0
            Dim sFirstDigits As String = sCreditCardNumber.Substring(0, sCreditCardNumber.Length - 4)
            Dim sLast4Digits As String = sCreditCardNumber.Remove(0, sCreditCardNumber.Length - 4)

            For Each sChar As Char In sFirstDigits.ToCharArray()
                If IsNumeric(sChar) Then
                    sFirstDigits = sFirstDigits.Replace(sChar, "X"c)
                End If
                i += 1
            Next
            sCreditCardNumber = sFirstDigits & sLast4Digits
        End If
        Return sCreditCardNumber
    End Function

#Region "ENCRYPT/DECRYPT ROUTINES"
    Public Shared Function Encrypt(ByVal key As String, ByVal s As String) As String
        Dim ret As String = ""
        Dim nLen As Integer = Len(s)
        Dim i As Integer, j As Integer = 0

        'pad the key so its at least same length as what we're trying to hash
        If Len(s) > Len(key) Then
            key = key.PadRight(Len(s), "*"c)
        End If

        For i = nLen To 1 Step -1
            j += 1

            Dim k As Integer = Asc(key.Chars(j - 1)) + 129

            ret += Hex(Asc(Mid(s, i, 1)) Xor (k))
            'traceA(Mid(s, i, 1) & " ++ " & k)
        Next

        Return ret
    End Function

    Public Shared Function Decrypt(ByVal key As String, ByVal s As String) As String
        Dim ret As String = ""
        Dim nLen As Integer = Len(s)
        Dim i As Integer, j As Integer = 0

        'pad the key so its at least same length as what we're trying to hash
        If Len(s) > Len(key) Then
            key = key.PadRight(Len(s), "*"c)
        End If

        For i = (nLen - 1) To 1 Step -2
            j += 1

            Dim k As Integer = Asc(key.Chars(SafeInteger(nLen / 2 - j))) + 129

            ret += Chr(CInt("&H" & Mid(s, i, 2)) Xor CInt(k))
            'traceA(Mid(s, i, 2) & " ** " & k)
        Next

        Return ret
    End Function

    Public Shared Function ConvertStringToHex(ByVal sToHex As String) As String
        Dim oTripleDES As New CTripleDES()

        Dim oByte As Byte() = Encoding.Default.GetBytes(oTripleDES.Encrypt(sToHex))

        Dim oSB As New StringBuilder
        For index As Integer = 0 To oByte.Length - 1
            oSB.Append(Hex(oByte(index)))
        Next

        Return oSB.ToString()
    End Function

    Public Shared Function ConvertHexToString(ByVal sHex As String) As String
        Dim oTripleDES As New CTripleDES()

        Dim oByte(SafeInteger(sHex.Length / 2 - 1)) As Byte

        sHex = UCase(sHex)
        For index As Integer = 0 To Len(sHex) - 2 Step 2
            Dim nByte As Integer = SafeInteger(sHex.Chars(index)) * 16
            Select Case sHex.Chars(index + 1)
                Case "1"c
                    nByte = nByte + 1
                Case "2"c
                    nByte = nByte + 2
                Case "3"c
                    nByte = nByte + 3
                Case "4"c
                    nByte = nByte + 4
                Case "5"c
                    nByte = nByte + 5
                Case "6"c
                    nByte = nByte + 6
                Case "7"c
                    nByte = nByte + 7
                Case "8"c
                    nByte = nByte + 8
                Case "9"c
                    nByte = nByte + 9
                Case "A"c
                    nByte = nByte + 10
                Case "B"c
                    nByte = nByte + 11
                Case "C"c
                    nByte = nByte + 12
                Case "D"c
                    nByte = nByte + 13
                Case "E"c
                    nByte = nByte + 14
                Case "F"c
                    nByte = nByte + 15
            End Select
            oByte(SafeInteger(index / 2)) = CByte(nByte)
        Next index

        Return oTripleDES.Decrypt(Encoding.Default.GetString(oByte))
    End Function


#End Region

End Class