Imports System.Data
Imports Microsoft.VisualBasic
Imports WebsiteLibrary.CSBCStd

Namespace DBUtils

    Public Class CPostgreSQLUtils

        Private _Connection As OleDb.OleDbConnection

        Public Sub New(ByVal psConnectionString As String)
            _Connection = New OleDb.OleDbConnection(psConnectionString)
            _Connection.Open()
        End Sub

        Public Function GetDataTable(ByVal psSQL As String) As DataTable
            Try
                Dim oDA As New OleDb.OleDbDataAdapter(psSQL, _Connection)
                Dim oDT As New DataTable()
                oDA.Fill(oDT)

                Return oDT
            Catch sqlEx As SqlClient.SqlException
                Throw New DBUtils.CSQLException(psSQL, sqlEx)
            End Try
        End Function

        Public Sub CloseConnection()
            If IsNothing(_Connection) = False AndAlso _Connection.State <> ConnectionState.Closed Then
                _Connection.Close()
            End If
            _Connection = Nothing
        End Sub

        Protected Overrides Sub Finalize()
            CloseConnection()
        End Sub


    End Class

End Namespace