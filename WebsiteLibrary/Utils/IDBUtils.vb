Imports System.Data

'interface that provides functions to encapsulate the dirty day to day routines involving
'commands, data readers, dataset, connection , and scalerexecution
Public Interface IDBUtils
    Function openConnection() As IDbConnection

    Function openConnectionTransacted() As IDbConnection
    Sub commitTransaction()
    Sub rollbackTransaction()

    'closes both connection and transacted connection
    'in asp, this method should be invoked on page unload
    Sub closeConnection()

    Function UpdateDataTable(ByVal dt As DataTable, ByVal sqlSelectMetaData As String, ByVal isTransacted As Boolean) As Integer
    Sub FillDataTable(ByVal dt As DataTable, ByVal sqlSelect As String)

    'used to execute insert/delete/update statements
    Function executeNonQuery(ByVal poEditSQL As WebsiteLibrary.DBUtils.ISQLEditStringBuilder, ByVal psChangedBy As String, _
                             Optional ByVal isTransacted As Boolean = False) As Integer
    Function executeNonQuery(ByVal poDeleteSQL As WebsiteLibrary.DBUtils.CSQLDeleteStringBuilder, ByVal psChangedBy As String, _
                             Optional ByVal isTransacted As Boolean = False) As Integer

    Function executeNonQuery(ByVal psSQL As String, Optional ByVal isTransacted As Boolean = False) As Integer

    'uses the non transacted conn to return single value from SELECT sql statement 
    Function getScalerValue(ByVal sql As String) As String

    Function getDataReader(ByVal sql As String) As IDataReader
    Function getDataTable(ByVal sql As String) As DataTable
End Interface

