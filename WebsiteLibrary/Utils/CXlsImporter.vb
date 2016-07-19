
Imports System.Data
Imports System.Data.OleDb
Imports System
Imports System.IO
Imports System.Collections.Specialized

<Serializable()> _
Public Class CXlsImporter 


    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Public Enum EExcelVersion
        Excel2003 = 0
        Excel2007 = 1
    End Enum

    Public ExcelVersion As EExcelVersion = EExcelVersion.Excel2007
    Private _FileHandle As FileDB.CFileHandle

    ''' <summary>
    ''' Gets the connection string depending on the type of Excel file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetConnectionString() As String
        Select Case ExcelVersion
            Case EExcelVersion.Excel2003
                Return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;MAXSCANROWS=15;READONLY=FALSE"""

            Case Else 'default office 2007
                Return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;HDR=YES;IMEX=1;MAXSCANROWS=15;READONLY=FALSE"""

        End Select
    End Function

    Public Sub New(ByVal value As System.IO.Stream)
        Dim oData(CInt(value.Length)) As Byte

        value.Read(oData, 0, CInt(value.Length))

        Me.Load(oData)

    End Sub

    Public Sub New(ByVal value() As Byte)
        Me.Load(value)

    End Sub

    Public Sub New(ByVal localFile As FileDB.CFileHandle)
        _FileHandle = localFile

    End Sub

    Private Sub Load(ByVal value() As Byte)
        Dim oDB As New FileDB.CFileDB()
        _FileHandle = oDB.GetNewFileHandle()

        'oHandle.LocalFileName
        Dim oFile As New FileStream(_FileHandle.LocalFileName, FileMode.Create, FileAccess.Write)

        oFile.Write(value, 0, value.Length)

        oFile.Close()

    End Sub

    ''' <summary>
    ''' Exports the data on 1 sheet of the excel workbook to a datatable
    ''' </summary>
    ''' <param name="psSheetName">The sheet name that is returned from GetSheets()</param>
    Public Function GetData(ByVal psSheetName As String) As DataTable
        Dim sConString As String = String.Format(GetConnectionString(), _FileHandle.LocalFileName)
        Dim oRet As DataTable = Nothing

        Using conn As New OleDbConnection(sConString)
            CSBCStd.LogDebug(log, "Began import excel file: " & psSheetName)

            Using cmd As New OleDbCommand("select * from [" & psSheetName & "];", conn)
                Try
                    conn.Open()

                    Dim adap As New OleDbDataAdapter(cmd)
                    oRet = New DataTable(psSheetName)

                    adap.Fill(oRet)

                Catch ex As Exception
                    CSBCStd.LogError(log, "Error trying to import excel file", ex)
                    Throw ex
                Finally
                    conn.Close()

                End Try
            End Using

        End Using

        Return oRet

    End Function

    ''' <summary>
    ''' Returns the list of Sheet Names in this excel workbook
    ''' </summary>
    Public Function GetSheets() As StringCollection
        Dim oDT As DataTable = Me.GetSchema()
        Dim oRet As New StringCollection

        If Me IsNot Nothing Then
            For Each row As DataRow In oDT.Rows
                oRet.Add(row("table_name").ToString())

            Next

        End If

        Return oRet

    End Function

    Private Function GetSchema() As DataTable
        Dim oRet As DataTable = Nothing

        Dim sConString As String = String.Format(GetConnectionString(), _FileHandle.LocalFileName)

        Using conn As New OleDbConnection(sConString)

            Try
                conn.Open()

                oRet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

            Catch ex As Exception
                Throw ex

            Finally
                conn.Close()

            End Try


        End Using

        Return oRet

    End Function

End Class