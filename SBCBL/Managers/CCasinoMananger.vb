Imports SBCBL.std

Namespace Managers
    Public Class CCasinoMananger
        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CCasinoMananger))

        Public Function LockAccount(ByVal poCasinoLogin As List(Of String), ByVal pbLock As Boolean) As Boolean
            Dim oAseDB As OleDb.OleDbConnection = Nothing

            Try
                ' Create ASE connection
                oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
                oAseDB.Open()

                Dim oCommand As OleDb.OleDbCommand
                Dim sSQL As String = String.Format("UPDATE casino_account SET LOCKED = {0} WHERE EMAIL IN ('{1}{2}')", _
                                                   SafeString(IIf(pbLock, 0, 1)), Join(poCasinoLogin.ToArray(), CASINO_SUFFIX & "','"), _
                                                   CASINO_SUFFIX)
                log.Debug("Lock Casino account. SQL: " & sSQL)

                oCommand = New OleDb.OleDbCommand(sSQL, oAseDB)
                oCommand.CommandType = CommandType.Text
                oCommand.ExecuteNonQuery()

                Return True
            Catch ex As Exception
                log.Error("Fail to lock casino account. Message: " & ex.Message, ex)
            Finally
                If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            End Try

            Return False
        End Function

        Public Function UpdateCasinoBalance(ByVal pnPlayerBalance As Double, ByVal pnCasinoLimit As Double, ByVal psPlayerLogin As String) As Boolean
            Dim oAseDB As OleDb.OleDbConnection = Nothing

            Try
                If pnPlayerBalance < pnCasinoLimit Then
                    ' Create ASE connection
                    oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
                    oAseDB.Open()

                    Dim oCommand As OleDb.OleDbCommand

                    ' Get CasinoID
                    Dim nCasinoID As Integer = 0
                    Dim sSQL As String = "SELECT AID FROM casino_account WHERE EMAIL = " & SQLString(psPlayerLogin & CASINO_SUFFIX)
                    log.Debug("Get CasinoID. SQL: " & sSQL)

                    oCommand = New OleDb.OleDbCommand(sSQL, oAseDB)
                    oCommand.CommandType = CommandType.Text
                    nCasinoID = SafeInteger(oCommand.ExecuteScalar())

                    If nCasinoID > 0 Then
                        sSQL = String.Format("UPDATE casino_account SET BALANCE = {0} WHERE AID = {1}", _
                                           SafeString(pnPlayerBalance), SafeString(nCasinoID))
                        log.Debug("Update Casino amount. SQL: " & sSQL)

                        oCommand.CommandText = sSQL
                        oCommand.ExecuteNonQuery()
                    End If
                    
                End If

                Return True
            Catch ex As Exception
                log.Error("Fail to update casino amount. Message: " & ex.Message, ex)
            Finally
                If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            End Try

            Return False
        End Function

        Public Function ChangePass(ByVal psCasinoLogin As String, ByVal psNewPassword As String) As Boolean
            Dim oAseDB As OleDb.OleDbConnection = Nothing

            Try
                ' Create ASE connection
                oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
                oAseDB.Open()

                Dim oCommand As OleDb.OleDbCommand
                Dim sSQL As String = String.Format("UPDATE casino_account SET PASSWORD = {0} WHERE EMAIL = {1}", _
                                                   SQLString(psNewPassword), SQLString(psCasinoLogin & CASINO_SUFFIX))
                log.Debug("Change Casino account password. SQL: " & sSQL)

                oCommand = New OleDb.OleDbCommand(sSQL, oAseDB)
                oCommand.CommandType = CommandType.Text
                oCommand.ExecuteNonQuery()

                Return True
            Catch ex As Exception
                log.Error("Fail to change casino account password. Message: " & ex.Message, ex)
            Finally
                If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            End Try

            Return False
        End Function

        Public Function UpdateAccount(ByVal psOldCasinoLogin As String, ByVal psNewCasinoLogin As String, _
                ByVal psCasinoPass As String, ByVal psName As String, ByVal pbIsLocked As Boolean) As Boolean
            Dim oAseDB As OleDb.OleDbConnection = Nothing

            Try
                ' Create ASE connection
                oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
                oAseDB.Open()

                Dim oCommand As OleDb.OleDbCommand
                Dim sSQL As String

                If psCasinoPass <> "" Then
                    sSQL = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, " & _
                                                   "FIRSTNAME = {1}, PASSWORD = {2}, EMAIL = {3} WHERE EMAIL = {4}", _
                                                   SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), _
                                                   SQLString(psCasinoPass), SQLString(psNewCasinoLogin & CASINO_SUFFIX), _
                                                   SQLString(psOldCasinoLogin & CASINO_SUFFIX))
                Else
                    sSQL = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, " & _
                                                   "FIRSTNAME = {1}, EMAIL = {2} WHERE EMAIL = {3}", _
                                                   SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), _
                                                    SQLString(psNewCasinoLogin & CASINO_SUFFIX), _
                                                   SQLString(psOldCasinoLogin & CASINO_SUFFIX))
                End If
                log.Debug("Update Casino account. SQL: " & sSQL)

                oCommand = New OleDb.OleDbCommand(sSQL, oAseDB)
                oCommand.CommandType = CommandType.Text
                oCommand.ExecuteNonQuery()

                Return True
            Catch ex As Exception
                log.Error("Fail to update casino account. Message: " & ex.Message, ex)
            Finally
                If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            End Try

            Return False
        End Function

        Public Function InsertAccount(ByVal psCasinoLogin As String, ByVal psCasinoPass As String, _
                                       ByVal psName As String, ByVal pbIsLocked As Boolean, ByVal pnBalance As Double) As Boolean
            Dim oAseDB As OleDb.OleDbConnection = Nothing

            Try
                ' Create ASE connection
                oAseDB = New OleDb.OleDbConnection(CASINO_CONNECTION_STRING)
                oAseDB.Open()

                Dim nCasinoID As Integer = 0
                Dim oCommand As OleDb.OleDbCommand
                oCommand = New OleDb.OleDbCommand()
                oCommand.Connection = oAseDB
                oCommand.CommandType = CommandType.StoredProcedure

                ' Add User
                oCommand.CommandText = "casino_proc_AddAccount"
                oCommand.Parameters.AddWithValue("@email", psCasinoLogin & CASINO_SUFFIX)
                oCommand.Parameters.AddWithValue("@password", psCasinoPass)
                oCommand.ExecuteNonQuery()

                ' Get UserID
                oCommand.CommandText = "casino_proc_GetUserID"
                oCommand.Parameters.Clear()
                oCommand.Parameters.AddWithValue("@email", psCasinoLogin & CASINO_SUFFIX)
                oCommand.Parameters.AddWithValue("@pass", psCasinoPass)

                nCasinoID = SafeInteger(oCommand.ExecuteScalar())

                If nCasinoID > 0 Then ' Add User successfully
                    ' Update User's Info
                    Dim sSQL As String = String.Format("UPDATE casino_account SET LOCKED = {0}, NICKNAME= {1}, FIRSTNAME = {1}, BALANCE= {2} WHERE AID = {3}", _
                                                   SafeString(IIf(pbIsLocked, 0, 1)), SQLString(psName), SafeString(pnBalance), SafeString(nCasinoID))
                    log.Debug("insert Casino account. SQL: " & sSQL)

                    oCommand.CommandType = CommandType.Text
                    oCommand.CommandText = sSQL
                    oCommand.Parameters.Clear()

                    oCommand.ExecuteNonQuery()
                End If

                Return True
            Catch ex As Exception
                log.Error("Fail to insert casino account. Message: " & ex.Message, ex)
            Finally
                If oAseDB IsNot Nothing AndAlso oAseDB.State <> ConnectionState.Closed Then oAseDB.Close()
            End Try

            Return False
        End Function
    End Class
End Namespace