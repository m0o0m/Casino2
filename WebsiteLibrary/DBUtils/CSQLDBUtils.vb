Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Xml.Serialization
Imports WebsiteLibrary.CSBCStd
Imports System.Collections

Namespace DBUtils
    ''' <summary>
    ''' This class provides for more convenient access into the SQL Server.  
    ''' Sample Proper use that avoids connection leaks:
    ''' 
    ''' Using oDB As New WebsiteLibrary.DBUtils.CSQLDBUtils(std.CONNECTIONSTRING)
    ''' Dim s As String = oDB.getScalerValue("SELECT top 10 * From Loans")
    ''' End Using
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' * when working w/ transactions, there is a potential deadlock (exception type  'System.Data.SqlClient.SqlException')
    ''' if we try to retrieve a field value, in a separate connection, from a row that's already being modified in an active transaction,
    ''' in a transacted connection.  The work around is if we detect a current transaction, we use that transacted connection
    ''' for all queries.
    ''' * also, we dont really need to worrry about opening a connection to retrieve data, in client 
    ''' code we should worry more about closing the connection asap so that it can be pooled
    ''' </remarks>
    Public Class CSQLDBUtils
        Implements IDBUtils
        Implements IDisposable


        Public EnableCacheMetaData As Boolean 'keeps a copy of the cached data columns SELECT statements in shared variable for efficiency reasons
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Private _connectionString As String

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="sMainConnection"></param>
        ''' <param name="psAuditConnection">Pass empty string for Audit Connection String if no auditing will be done</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal sMainConnection As String, ByVal psAuditConnection As String)
            _connectionString = sMainConnection
            EnableAuditLog = False
            If Safestring(psAuditConnection) <> "" Then
                _sAuditConnection = Safestring(psAuditConnection)
                EnableAuditLog = True
            End If
        End Sub

        '' Don't create this constructor anymore.
        '' Use previous contructor to remind everyone add AuditConnection string for every tables relate to Cases
        'Public Sub New(ByVal sMainConnection As String)
        '    _connectionString = sMainConnection
        'End Sub

        'Use for write auditLog in difference DB
        Private _sAuditConnection As String = ""
        Private _bEnableAuditLog As Boolean = False

        Public Property EnableAuditLog() As Boolean
            Get
                Return _bEnableAuditLog
            End Get
            Set(ByVal value As Boolean)
                If Safestring(_sAuditConnection) <> "" Then
                    _bEnableAuditLog = value
                End If
            End Set
        End Property

        Private _Conn As SqlConnection
        Private _ConnTransacted As SqlConnection
        Private _sharedTransaction As SqlTransaction 'associated w/ _ConnTransacted

        'one of the problems with filling datatables is that dataset table has n fields defined and the db has n+1 fields defined, then when we fill the datatable using SELECT * and try to update, it will fail!
        'key of hashtable = table name 
        'Public Shared SelectCache As New Hashtable()

        Private _CommandTimeout As Integer = 90
        Public Property CommandTimeout() As Integer
            Get
                Return _CommandTimeout
            End Get
            Set(ByVal Value As Integer)
                _CommandTimeout = Value
            End Set
        End Property

        Public Sub closeConnection() Implements IDBUtils.closeConnection
            If IsNothing(_lastDataReader) = False AndAlso _lastDataReader.IsClosed = False Then
                _lastDataReader.Close()
            End If
            If IsNothing(_Conn) = False AndAlso _Conn.State <> ConnectionState.Closed Then
                _Conn.Close()
            End If
            _Conn = Nothing

            If False = IsNothing(_ConnTransacted) AndAlso _ConnTransacted.State <> ConnectionState.Closed Then
                _sharedTransaction = Nothing
                _ConnTransacted.Close()
            End If
            _ConnTransacted = Nothing

        End Sub

        Public Function openConnection() As System.Data.IDbConnection Implements IDBUtils.openConnection
            If IsNothing(_Conn) Then

                _Conn = New SqlConnection(_connectionString)
                _Conn.Open()
            End If
            Return _Conn
        End Function

        Public Function openConnectionTransacted() As System.Data.IDbConnection Implements IDBUtils.openConnectionTransacted
            If IsNothing(_ConnTransacted) Then
                _ConnTransacted = New SqlConnection(_connectionString)
                _ConnTransacted.Open()
            End If

            'ensure there is a valid transaction since the transaction might be invalidated after commit/rollback
            If IsNothing(_sharedTransaction) Then
                _sharedTransaction = _ConnTransacted.BeginTransaction
            ElseIf IsNothing(_sharedTransaction.Connection) Then
                _sharedTransaction = _ConnTransacted.BeginTransaction
            End If

            Return _ConnTransacted
        End Function

        'if there is no current transaction then that means no sql was executed or the connection was never opened -- 
        Public Sub commitTransaction() Implements IDBUtils.commitTransaction
            If IsNothing(_sharedTransaction) Then
                Throw New ApplicationException("No transaction to commit.  Please check that transacted connection was opened or there was some sql executed against a transacted connection!")
            End If
            _sharedTransaction.Commit()
            _sharedTransaction = Nothing 'turn off transaction 
        End Sub

        Public Sub rollbackTransaction() Implements IDBUtils.rollbackTransaction
            If IsNothing(_sharedTransaction) Then
                Throw New ApplicationException("No transaction to commit.  Please check that transacted connection was opened or there was some sql executed against a transacted connection!")
            End If
            _sharedTransaction.Rollback()
            _sharedTransaction = Nothing 'turn off transaction 
        End Sub

        Private Function isTransactionActive() As Boolean
            If IsNothing(_sharedTransaction) Then
                Return False
            ElseIf IsNothing(_sharedTransaction.Connection) Then
                Return False
            End If

            Return True
        End Function

        Public Function executeNonQuery(ByVal poDeleteSQL As CSQLDeleteStringBuilder, _
                                        ByVal psChangedBy As String, Optional ByVal isTransacted As Boolean = False) As Integer _
                                        Implements IDBUtils.executeNonQuery

            Dim cmd As SqlCommand
            If isTransacted Then
                cmd = New SqlCommand(poDeleteSQL.SQL, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(poDeleteSQL.SQL, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("ExecNonQuery:" & poDeleteSQL.SQL, "SQL")
            Dim oAuditor As CAuditor = Nothing
            Try
                'Write Audit Log
                If EnableAuditLog Then
                    oAuditor = New CAuditor(_sAuditConnection)
                    Dim oDT As DataTable = getDataTable(String.Format("SELECT * FROM {0} {1}", poDeleteSQL.TableName, poDeleteSQL.Conditions))
                    oAuditor.AppendAuditLog(oDT, poDeleteSQL, psChangedBy)
                End If

                'Set EnableAuditLog for next times
                If Safestring(_sAuditConnection) <> "" Then
                    EnableAuditLog = True
                End If

                'Exec SQL
                cmd.CommandTimeout = CommandTimeout
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                Throw New CSQLException(poDeleteSQL.SQL & getDeadlockSummary(sqlEx), sqlEx)
            Finally
                If oAuditor IsNot Nothing Then oAuditor.closeConnection()
            End Try
        End Function

        Public Function executeNonQuery(ByVal poEditSQL As ISQLEditStringBuilder, ByVal psChangedBy As String, _
                                        Optional ByVal isTransacted As Boolean = False) As Integer _
                                        Implements IDBUtils.executeNonQuery
            If TypeOf poEditSQL Is CSQLInsertStringBuilder Then
                Return executeInsertQuery(CType(poEditSQL, CSQLInsertStringBuilder), psChangedBy, isTransacted)
            ElseIf TypeOf poEditSQL Is CSQLUpdateStringBuilder Then
                Return executeUpdateQuery(CType(poEditSQL, CSQLUpdateStringBuilder), psChangedBy, isTransacted)
            End If

            Return 0
        End Function

        Private Function executeInsertQuery(ByVal poInsertSQL As CSQLInsertStringBuilder, ByVal psChangedBy As String, _
                                        Optional ByVal isTransacted As Boolean = False) As Integer
            Dim cmd As SqlCommand
            If isTransacted Then
                cmd = New SqlCommand(poInsertSQL.SQL, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(poInsertSQL.SQL, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("ExecNonQuery:" & poInsertSQL.SQL, "SQL")
            Dim oAuditor As CAuditor = Nothing
            Try
                'Write Audit Log
                If EnableAuditLog Then
                    oAuditor = New CAuditor(_sAuditConnection)
                    oAuditor.AppendAuditLog(poInsertSQL, psChangedBy)
                End If

                'Set EnableAuditLog for next times
                If Safestring(_sAuditConnection) <> "" Then
                    EnableAuditLog = True
                End If

                'Exec SQL
                cmd.CommandTimeout = CommandTimeout
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                Throw New CSQLException(poInsertSQL.SQL & getDeadlockSummary(sqlEx), sqlEx)
            Finally
                If oAuditor IsNot Nothing Then oAuditor.closeConnection()
            End Try
        End Function

        Private Function executeUpdateQuery(ByVal poUpdateSQL As CSQLUpdateStringBuilder, ByVal psChangedBy As String, _
                                        Optional ByVal isTransacted As Boolean = False) As Integer
            Dim cmd As SqlCommand
            If isTransacted Then
                cmd = New SqlCommand(poUpdateSQL.SQL, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(poUpdateSQL.SQL, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("ExecNonQuery:" & poUpdateSQL.SQL, "SQL")
            Dim oAuditor As CAuditor = Nothing
            Try
                ''Write Audit Log
                'If EnableAuditLog Then
                '    oAuditor = New CAuditor(_sAuditConnection)
                '    Dim oDT As DataTable = getDataTable(String.Format("SELECT * FROM {0} {1}", poUpdateSQL.TableName, poUpdateSQL.Conditions))
                '    oAuditor.AppendAuditLog(oDT, poUpdateSQL, psChangedBy)
                'End If

                ''Set EnableAuditLog for next times
                'If Safestring(_sAuditConnection) <> "" Then
                '    EnableAuditLog = True
                'End If

                'Exec SQL
                cmd.CommandTimeout = CommandTimeout
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                Throw New CSQLException(poUpdateSQL.SQL & getDeadlockSummary(sqlEx), sqlEx)
            Finally
                If oAuditor IsNot Nothing Then oAuditor.closeConnection()
            End Try
        End Function

        Public Function executeNonQuery(ByVal psSQL As String, Optional ByVal isTransacted As Boolean = False) As Integer Implements IDBUtils.executeNonQuery
            Dim cmd As SqlCommand
            If isTransacted Then
                cmd = New SqlCommand(psSQL, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(psSQL, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("ExecNonQuery:" & psSQL, "SQL")
            Try
                'Exec SQL
                cmd.CommandTimeout = CommandTimeout
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                Throw New CSQLException(psSQL & getDeadlockSummary(sqlEx), sqlEx)
            End Try
        End Function

        Public Function executeStoredProcedure(ByVal pSql As String, ByVal isTransacted As Boolean, ByVal ParamArray poSqlParameter() As SqlParameter) As Integer
            Dim cmd As SqlCommand
            If isTransacted Then
                cmd = New SqlCommand(pSql, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(pSql, CType(openConnection(), SqlConnection))
            End If

            If poSqlParameter.Length > 0 Then
                For index As Integer = 0 To poSqlParameter.Length - 1
                    cmd.Parameters.Add(poSqlParameter(index))
                Next
            End If

            Trace.WriteLine("ExecNonQuery:" & pSql, "SQL")
            Try
                cmd.CommandTimeout = CommandTimeout
                cmd.CommandType = CommandType.StoredProcedure
                Return cmd.ExecuteNonQuery()
            Catch sqlEx As SqlClient.SqlException
                Throw New CSQLException(pSql & getDeadlockSummary(sqlEx), sqlEx)
            End Try

        End Function

        'last datareader sql that was executed, saved for debuggin purposes
        Private _LastDataReaderSQL As String
        Private _lastDataReader As IDataReader
        Public Function getDataReader(ByVal sql As String) As System.Data.IDataReader Implements IDBUtils.getDataReader
            checkDataReaderStillOpen()

            Dim cmd As SqlCommand
            If isTransactionActive() Then
                cmd = New SqlCommand(sql, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(sql, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("GetDataReader:" & sql, "SQL")

            Try
                cmd.CommandTimeout = CommandTimeout
                _lastDataReader = cmd.ExecuteReader
                _LastDataReaderSQL = sql
            Catch sqlEx As SqlClient.SqlException
                Trace.WriteLine("SQL Syntax Error:" & sql & vbCrLf & sqlEx.ToString(), "ERR")
                Throw New CSQLException(sql & getDeadlockSummary(sqlEx), sqlEx)
            End Try

            Return _lastDataReader
        End Function

        Public Function getDataTable(ByVal sql As String) As System.Data.DataTable Implements IDBUtils.getDataTable
            checkDataReaderStillOpen()
            Dim ds As New DataSet()
            Dim da As SqlDataAdapter
            If isTransactionActive() Then
                da = New SqlDataAdapter(sql, CType(openConnectionTransacted(), SqlConnection))
                da.SelectCommand.Transaction = _sharedTransaction
            Else
                da = New SqlDataAdapter(sql, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("GetDataTable:" & sql, "SQL")
            Try
                da.SelectCommand.CommandTimeout = CommandTimeout
                da.Fill(ds)
            Catch sqlEx As SqlClient.SqlException
                Trace.WriteLine("SQL Syntax Error:" & sql & vbCrLf & sqlEx.ToString(), "ERR")
                Throw New CSQLException(sql & getDeadlockSummary(sqlEx), sqlEx)
            End Try

            Return ds.Tables(0)
        End Function

        Public Function getScalerValue(ByVal sql As String) As String Implements IDBUtils.getScalerValue
            checkDataReaderStillOpen()
            Dim cmd As SqlCommand
            If isTransactionActive() Then
                cmd = New SqlCommand(sql, CType(openConnectionTransacted(), SqlConnection))
                cmd.Transaction = _sharedTransaction
            Else
                cmd = New SqlCommand(sql, CType(openConnection(), SqlConnection))
            End If

            Trace.WriteLine("GetScalerValue:" & sql, "SQL")
            Try
                cmd.CommandTimeout = CommandTimeout
                'Dim execResult As Object = cmd.ExecuteScalar

                Return Safestring(cmd.ExecuteScalar)
            Catch sqlEx As SqlClient.SqlException
                Trace.WriteLine("SQL Syntax Error:" & sql & vbCrLf & sqlEx.ToString(), "ERR")
                Throw New CSQLException(sql & getDeadlockSummary(sqlEx), sqlEx)
            End Try
        End Function
#Region "String Manip Routines"
        'READ UNCOMMITTED - Implements dirty read, or isolation level 0 locking, which means that no 
        ' shared locks are issued and no exclusive locks are honored. When this option is set, it 
        ' is possible to read uncommitted or dirty data; values in the data can be changed and rows 
        ' can appear or disappear in the data set before the end of the transaction. This option 
        ' has the same effect as setting NOLOCK on all tables in all SELECT statements in a 
        ' transaction. This is the least restrictive of the four isolation levels.
        Public Shared Function SQLReadUncommitted(ByVal psSQL As String) As String
            Dim sResult As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & psSQL & " ; SET TRANSACTION ISOLATION LEVEL READ COMMITTED;"
            Return sResult
        End Function
        Public Overloads Shared Function Safestring(ByVal oNull As DBNull) As String
            Return ""
        End Function

        Public Overloads Shared Function SafeString(ByVal s As Object) As String
            ' special case to handle Recordset Field objects
            If s Is Nothing OrElse TypeOf s Is DBNull Then
                Return ""
            ElseIf TypeOf s Is Guid Then
                'we really don't want to add too many else clauses here, but this GUID check seems like it will keep our code DSL
                'more maintainable since it will help our code avoid having to know the format of the GUID.  We read from db as-is, we write as-is.
                Return s.ToString()
            Else
                Return Trim(CStr(s)) 'very odd: if u return trim(s.toString()), then for dates, it will append 12:00:00 AM if there is no time :-T
            End If
        End Function

        Public Overloads Shared Function SQLString(ByVal s As Object) As String
            Return "'" & Safestring(s).Replace("'", "''") & "'"
        End Function

        'only used when fine control is needed  over how a string should be escaped w/ or w/o trimming
        'avoid using if possible
        Public Overloads Shared Function SQLString(ByVal s As String, ByVal pbTrim As Boolean) As String
            If s = "" Then
                Return "''"
            End If
            If pbTrim Then
                Return "'" & s.Trim().Replace("'", "''") & "'"
            Else
                Return "'" & s.Replace("'", "''") & "'"
            End If
        End Function
#End Region

        Private Sub checkDataReaderStillOpen()
            If False = IsNothing(_lastDataReader) Then
                If _lastDataReader.IsClosed = False Then
                    Throw New InvalidOperationException("There is already a data reader open w/ the sql:" & _LastDataReaderSQL)
                End If
            End If
        End Sub

        Private Shared _cachedSelects As New Hashtable()
        Private Function buildSelect(ByVal poDataTable As DataTable) As String
            Dim sqlFields As New System.Text.StringBuilder()
            sqlFields.Append("SELECT ")
            Dim i As Integer
            For i = 0 To poDataTable.Columns.Count - 2
                Dim oCol As DataColumn = poDataTable.Columns(i)
                sqlFields.Append("[" & oCol.ColumnName & "], ")
            Next
            sqlFields.Append("[" & poDataTable.Columns(poDataTable.Columns.Count - 1).ColumnName & "]") 'handle last column 
            sqlFields.Append(" FROM " & poDataTable.TableName & " ")

            Return sqlFields.ToString
        End Function




        Public Overloads Sub FillDataTable(ByVal dt As DataTable, ByVal poWhereClauseWithWhere As CSQLWhereStringBuilder, Optional ByVal psOrderClause As String = "")
            If dt.Columns.Count <= 0 Then
                Throw New ArgumentException("Datatable passed in must already have columns defined!  Otherwise, please use other FillDataTable overload.")
            End If

            Dim sqlFields As New System.Text.StringBuilder()

            If Me.EnableCacheMetaData Then
                If False = _cachedSelects.ContainsKey(dt.TableName) Then
                    SyncLock (_cachedSelects.SyncRoot)
                        _cachedSelects(dt.TableName) = buildSelect(dt)
                    End SyncLock
                End If

                sqlFields.Append(CStr(_cachedSelects(dt.TableName)))
            Else
                sqlFields.Append(buildSelect(dt))
            End If

            If False = IsNothing(poWhereClauseWithWhere) Then
                sqlFields.Append(poWhereClauseWithWhere.SQL)
            End If
            sqlFields.Append(" " & psOrderClause)

            Trace.WriteLine("Dynamic SQL for Fill: " & sqlFields.ToString)

            Try
                FillDataTable(dt, sqlFields.ToString)
            Catch sqlEx As SqlException
                Throw New CSQLException(sqlFields.ToString & getDeadlockSummary(sqlEx), sqlEx)
            End Try
        End Sub

        Public Overloads Sub FillDataTable(ByVal dt As DataTable, ByVal sqlSelect As String) Implements IDBUtils.FillDataTable
            Dim oConn As SqlConnection
            If isTransactionActive() Then
                oConn = CType(openConnectionTransacted(), SqlConnection)
            Else
                oConn = CType(openConnection(), SqlConnection)
            End If

            Dim oAdapter As New SqlDataAdapter(sqlSelect, oConn)
            oAdapter.SelectCommand.Transaction = _sharedTransaction
            oAdapter.SelectCommand.CommandTimeout = Me.CommandTimeout
            Try
                oAdapter.Fill(dt)
            Catch sqlEx As SqlException
                Throw New CSQLException(sqlSelect & getDeadlockSummary(sqlEx), sqlEx)
            End Try
        End Sub

        'note: the command builder generates code that will acocunt for db concurrency violations.  if no row is updated, that means the row has been
        'modified and a dbexception will be thrown
        'refer to this for more info on concurrency: http://msdn.microsoft.com/msdnmag/issues/03/04/DataConcurrency/default.aspx
        Public Overloads Function UpdateDataTable(ByVal dt As DataTable, ByVal sqlSelectMetaData As String, ByVal isTransacted As Boolean) As Integer Implements IDBUtils.UpdateDataTable
            Dim oConn As SqlConnection
            If isTransacted Then
                oConn = CType(openConnectionTransacted(), SqlConnection)
            Else
                oConn = CType(openConnection(), SqlConnection)
            End If

            Dim oAdapter As New SqlDataAdapter(sqlSelectMetaData, oConn)
            Dim cm As New SqlCommandBuilder(oAdapter)
            cm.QuotePrefix = "["
            cm.QuoteSuffix = "]"
            If isTransactionActive() Then
                'this must be done prior to update so that the command builder will auto enlist the other command objects into the transaction!
                '    'REF: http://groups.google.com/groups?hl=en&lr=&ie=UTF-8&oe=UTF-8&threadm=olb8du423jcslrjp78m1p09lufhktncagd%404ax.com&rnum=5&prev=/groups%3Fhl%3Den%26lr%3D%26ie%3DUTF-8%26oe%3DUTF-8%26q%3Dcommandbuilder%2Btransaction%2B%2522Execute%2Brequires%2Bthe%2Bcommand%2Bto%2Bhave%2Ba%2Btransaction%2Bobject%2Bwhen%2Bthe%2Bconnection%2Bassigned%2Bto%2Bthe%2Bcommand%2Bis%2Bin%2Ba%2Bpending%2Blocal%2Btransaction.%2B%2BThe%2BTransaction%2Bproperty%2Bof%2Bthe%2Bcommand%2Bhas%2Bnot%2Bbeen%2Binitialized.%2522
                oAdapter.SelectCommand.Transaction = _sharedTransaction
            End If
            Try
                oAdapter.InsertCommand = cm.GetInsertCommand
                oAdapter.UpdateCommand = cm.GetUpdateCommand
                oAdapter.DeleteCommand = cm.GetDeleteCommand

                oAdapter.InsertCommand.CommandTimeout = Me.CommandTimeout
                oAdapter.UpdateCommand.CommandTimeout = Me.CommandTimeout
                oAdapter.DeleteCommand.CommandTimeout = Me.CommandTimeout
            Catch sqlEx As SqlException
                Throw New CSQLException(sqlEx.Message, sqlEx)
            End Try

            'retrieve the update/insert commands so that we can change their timeouts.  Once that's done, the 
            Dim nAffectedCount As Integer
            Try


                nAffectedCount = oAdapter.Update(dt)
            Catch dbErr As DBConcurrencyException
                Throw dbErr
            Catch sqlEx As SqlException
                If dt.Rows.Count = 1 Then
                    Throw New CSQLException("Unable to update row in datatable (" & dt.TableName & ") :" & getRowTostring(dt.Rows(0)) & vbCrLf & " Error Message:" & sqlEx.Message, sqlEx)
                Else
                    Dim sError As New System.Text.StringBuilder()
                    sError.Append("Unable to update row in datatable (" & dt.TableName & ") :")
                    Dim oRow As DataRow
                    For Each oRow In dt.Rows
                        sError.Append(getRowTostring(oRow) & vbCrLf)
                    Next
                    sError.Append("SQL Error Message: " & sqlEx.Message)
                    Throw New CSQLException(sError.ToString, sqlEx)
                End If
            End Try
            Return nAffectedCount
        End Function

        'safe method to update datatable when the db may have more fields than the datatable we are trying to update
        'otherwise you will get invalid operation/missing datacolumn errors
        Public Overloads Function UpdateDataTable(ByVal dt As DataTable, ByVal isTransacted As Boolean) As Integer
            If dt.Columns.Count <= 0 Then
                Throw New ArgumentException("Datatable passed in must already have columns defined!  Otherwise, please use other FillDataTable overload.")
            End If

            Dim sqlFields As New System.Text.StringBuilder()

            If Me.EnableCacheMetaData Then
                If False = _cachedSelects.ContainsKey(dt.TableName) Then
                    SyncLock (_cachedSelects.SyncRoot)
                        _cachedSelects(dt.TableName) = buildSelect(dt)
                    End SyncLock
                End If

                sqlFields.Append(CStr(_cachedSelects(dt.TableName)))
            Else
                sqlFields.Append(buildSelect(dt))
            End If

            Return UpdateDataTable(dt, sqlFields.ToString, isTransacted)
        End Function

        Private Function getRowTostring(ByVal oRow As DataRow) As String
            Dim sResult As String = ""
            Dim tbl As DataTable = oRow.Table
            Dim col As DataColumn
            For Each col In tbl.Columns
                Trace.WriteLine("type: " & col.ColumnName & ": " & col.DataType.ToString)
                If col.DataType.Equals(GetType(String)) Then
                    sResult += col.ColumnName & "=" & SQLString(Safestring(oRow(col.ColumnName))) & ","
                ElseIf col.DataType.Equals(GetType(Date)) Then
                    sResult += col.ColumnName & "=" & SQLString(Safestring(oRow(col.ColumnName))) & ","
                Else
                    sResult += col.ColumnName & "=" & Safestring(oRow(col.ColumnName)) & ", "
                End If

            Next
            If sResult <> "" Then
                sResult = sResult.TrimEnd(","c)
                sResult += " [Where SafeGuardClauseHere]" 'append this so we don't accidently screw things up when doing Update XXX Set .... 
            End If
            Return sResult
        End Function

        Private Function getDeadlockSummary(ByVal ex As SqlException) As String
            Dim sMsg As String = ex.Message.ToUpper
            If sMsg.IndexOf("DEADLOCKED") = -1 And sMsg.IndexOf("TIMEOUT EXPIRED") = -1 Then
                Return ""
            End If

            Dim dt As DataTable
            Try
                dt = getDataTable("exec sp_lock")
            Catch exDead As System.Exception
                LogError(log, "getDeadlockSummary", exDead)
                Debug.Write("Unable to retrieve deadlock info: " & exDead.ToString)
                Return vbCrLf & "Unable to obtain deadlock info."
            End Try

            Dim dr As DataRow
            Dim sBuffer As New System.Text.StringBuilder()
            Dim nCount As Integer
            For Each dr In dt.Rows
                If Safestring(dr("Mode")) = "X" Then 'exclusive lock 
                    nCount += 1
                    Dim sObjName As String = getScalerValue("Select Name From sysObjects where id=" & Safestring(dr("objID")))
                    Dim sPID As String = Safestring(dr("SPID"))
                    Dim sBuff As String
                    Try
                        Dim dtBuff As DataTable = getDataTable(String.Format("dbcc inputbuffer ({0})", sPID))
                        sBuff = Safestring(dtBuff.Rows(0)("EventInfo"))
                    Catch exBuff As System.Exception
                        sBuff = "Unable to retrieve buffer: " & exBuff.ToString
                    End Try

                    sBuffer.AppendFormat("SPID: {0} Object:{1} SQL:{2}" & vbCrLf, sPID, sObjName, sBuff)

                End If
            Next

            Return vbCrLf & " Deadlock summary ( " & nCount & " blockings " & ") : " & sBuffer.ToString
        End Function

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free unmanaged resources when explicitly called
                    closeConnection()
                End If

                ' TODO: free shared unmanaged resources
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

    Public Class CSQLException
        Inherits System.Exception

        Sub New(ByVal psError As String, ByVal poError As Exception)
            MyBase.New(psError, poError)
        End Sub
    End Class
End Namespace