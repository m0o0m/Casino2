Imports System.Data.SqlClient
Imports WebsiteLibrary.DBUtils
Imports WebsiteLibrary.CSBCStd
Imports System.Data

Namespace DBUtils
    Public Class CAuditor
        Implements IDisposable
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private _sConnectionString As String
        Private _CommandTimeout As Integer = 90
        Private _Conn As SqlConnection

#Region "Constructor"
        Public Sub New(ByVal sConnection As String)
            _sConnectionString = sConnection
        End Sub
#End Region

#Region "Properties"
        Public Property CommandTimeout() As Integer
            Get
                Return _CommandTimeout
            End Get
            Set(ByVal Value As Integer)
                _CommandTimeout = Value
            End Set
        End Property
#End Region

#Region "Functions"
        Public Sub closeConnection()
            If IsNothing(_Conn) = False AndAlso _Conn.State <> ConnectionState.Closed Then
                _Conn.Close()
            End If
            _Conn = Nothing
        End Sub

        Public Function openConnection() As System.Data.IDbConnection
            If IsNothing(_Conn) Then
                _Conn = New SqlConnection(_sConnectionString)
                _Conn.Open()
            End If
            Return _Conn
        End Function

        Private Function executeNonQuery(ByVal sql As String, Optional ByVal isTransacted As Boolean = False) As Integer
            Dim cmd As SqlCommand
            cmd = New SqlCommand(sql, CType(openConnection(), SqlConnection))
            Try
                cmd.CommandTimeout = CommandTimeout
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                log.Error("Can not insert into Audit table. SQL: " & sql, sqlEx)
            End Try

        End Function

        Public Sub AppendAuditLog(ByVal poInsertSQL As CSQLInsertStringBuilder, ByVal psChangedBy As String)
            Dim oInsert As New CSQLInsertStringBuilder(poInsertSQL.TableName & "Logs")
            oInsert.AppendString(poInsertSQL.TableName & "LogID", SQLString(newGUID()))
            oInsert.AppendString("SavedDate", "GETUTCDATE()")
            If SafeString(psChangedBy) <> "" Then
                oInsert.AppendString("SavedBy", SQLString(psChangedBy))
            End If
            oInsert.AppendString("SavedStatus", SQLString("I"))

            For i As Integer = 0 To poInsertSQL.Columns.Count - 1
                oInsert.AppendString(String.Format("[{0}]", SafeString(poInsertSQL.Columns(i))), SafeString(poInsertSQL.Values(i)))
            Next

            executeNonQuery(oInsert.SQL())
        End Sub

        Public Sub AppendAuditLog(ByVal pTbl As DataTable, ByVal poDeleteSQL As CSQLDeleteStringBuilder, ByVal psChangedBy As String)
            If pTbl IsNot Nothing AndAlso pTbl.Rows.Count > 0 Then
                'insert a record for every record affected by this delete
                For Each oDR As DataRow In pTbl.Rows
                    Dim oInsert As New CSQLInsertStringBuilder(poDeleteSQL.TableName & "Logs")
                    oInsert.AppendString(poDeleteSQL.TableName & "LogID", SQLString(newGUID()))
                    oInsert.AppendString("SavedDate", "GETUTCDATE()")
                    If SafeString(psChangedBy) <> "" Then
                        oInsert.AppendString("SavedBy", SQLString(psChangedBy))
                    End If
                    oInsert.AppendString("SavedStatus", SQLString("D"))

                    For Each oColumn As DataColumn In pTbl.Columns
                        If Not IsDBNull(oDR(oColumn.ColumnName)) Then
                            oInsert.AppendString(String.Format("[{0}]", oColumn.ColumnName), SQLString(oDR(oColumn.ColumnName)))
                        End If
                    Next

                    executeNonQuery(oInsert.SQL())
                Next
            End If
        End Sub

        Public Sub AppendAuditLog(ByVal pTbl As DataTable, ByVal poUpdateSQL As CSQLUpdateStringBuilder, ByVal psChangedBy As String)
            If pTbl IsNot Nothing AndAlso pTbl.Rows.Count > 0 Then
                'Write Logs updated row
                For Each oDR As DataRow In pTbl.Rows
                    'Prepare data to insert to table Logs
                    Dim oInsert As New CSQLInsertStringBuilder(poUpdateSQL.TableName & "Logs")
                    oInsert.AppendString(poUpdateSQL.TableName & "LogID", SQLString(newGUID()))
                    oInsert.AppendString("SavedDate", "GETUTCDATE()")
                    If SafeString(psChangedBy) <> "" Then
                        oInsert.AppendString("SavedBy", SQLString(psChangedBy))
                    End If
                    oInsert.AppendString("SavedStatus", SQLString("U"))
                    'Get Row's Info
                    For Each oColumn As DataColumn In pTbl.Columns
                        If Not IsDBNull(oDR(oColumn.ColumnName)) Then
                            oInsert.AppendString(String.Format("[{0}]", oColumn.ColumnName), SQLString(oDR(oColumn.ColumnName)))
                        End If
                    Next
                    'Update's info
                    For i As Integer = 0 To poUpdateSQL.Columns.Count - 1
                        oInsert.AppendString(String.Format("[{0}]", SafeString(poUpdateSQL.Columns(i))), SafeString(poUpdateSQL.Values(i)))
                    Next

                    executeNonQuery(oInsert.SQL())
                Next
            End If
        End Sub
#End Region

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free other state (managed objects).
                End If

                closeConnection()
            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace