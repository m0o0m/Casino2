Imports SBCBL.std
Imports WebsiteLibrary.DBUtils
Imports System.Text
Namespace Managers

    Public Class CDataManager
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CEmailsManager))

        Public Function DeleteData(ByVal poDate As String) As Boolean
            Dim bSuccess As Boolean = True
            Dim sDeleteSQL As New StringBuilder
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try

                Dim oDeleteGameLine As String = String.Format("DELETE FROM GAMELINES  WHERE createddate<= dateadd(day,-{0},getdate()) and GameID not in (SELECT GameID as RowDelete FROM GAMES  WHERE GameDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'N')='Y') ", poDate)
                Dim oDeleteGame As String = String.Format("DELETE FROM GAMES WHERE GameDate<= dateadd(day,-{0},getdate())  and ISNULL(IsForProp,'')<>'Y'  ", poDate)
                Dim oDeleteTicketBets As String = String.Format("DELETE FROM TicketBets WHERE TicketID in (SELECT TicketID FROM Tickets WHERE TransactionDate<= dateadd(day,-{0},getdate())) and TicketID not in (SELECT TicketID as RowDelete FROM Tickets  WHERE TransactionDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'N')<>'Y') ", poDate)
                Dim oDeleteTickets As String = String.Format("DELETE FROM Tickets WHERE TransactionDate<= dateadd(day,-{0},getdate())  and ISNULL(IsForProp,'')<>'Y' ", poDate)
                sDeleteSQL.Append(oDeleteGameLine)
                sDeleteSQL.Append(oDeleteGame)
                sDeleteSQL.Append(oDeleteTicketBets)
                sDeleteSQL.Append(oDeleteTickets)
                odbSQL.executeNonQuery(sDeleteSQL.ToString(), True)
                _log.Debug("Delete Data. SQL: " & sDeleteSQL.ToString())
                odbSQL.commitTransaction()
            Catch ex As Exception
                bSuccess = False
                odbSQL.commitTransaction()
                _log.Error("Error trying to Delete Data. SQL: " & sDeleteSQL.ToString(), ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function RowDeleteDataGameLine(ByVal poDate As String) As Integer
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sRowDeleteGameLine As String = ""
            Try
                sRowDeleteGameLine = String.Format("SELECT COUNT(GameLineID) as RowDelete FROM GAMELINES  WHERE createddate<= dateadd(day,-{0},getdate())  and GameID not in (SELECT GameID as RowDelete FROM GAMES  WHERE GameDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'N')='Y')  ", poDate)
                Return SafeInteger(odbSQL.getScalerValue(sRowDeleteGameLine))
                _log.Debug("Get Row Delete Data. SQL: " & sRowDeleteGameLine)
            Catch ex As Exception
                Return 0
                _log.Error("Error trying to Get Row Delete Data. SQL: " & sRowDeleteGameLine, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return 0
        End Function

        Public Function RowDeleteDataGame(ByVal poDate As String) As Integer
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sRowDeleteGameLine As String = ""
            Try
                sRowDeleteGameLine = String.Format("SELECT COUNT(GameID) as RowDelete FROM GAMES  WHERE GameDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'')<>'Y' ", poDate)
                Return SafeInteger(odbSQL.getScalerValue(sRowDeleteGameLine))
                _log.Debug("Get Row Delete Data. SQL: " & sRowDeleteGameLine)
            Catch ex As Exception
                Return 0
                _log.Error("Error trying to Get Row Delete Data. SQL: " & sRowDeleteGameLine, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return 0
        End Function

        Public Function RowDeleteDataTicKet(ByVal poDate As String) As Integer
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sRowDeleteGameLine As String = ""
            Try
                sRowDeleteGameLine = String.Format("SELECT COUNT(TicketID) as RowDelete FROM Tickets  WHERE TransactionDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'N')<>'Y'  ", poDate)
                Return SafeInteger(odbSQL.getScalerValue(sRowDeleteGameLine))
                _log.Debug("Get Row Delete Data. SQL: " & sRowDeleteGameLine)
            Catch ex As Exception
                Return 0
                _log.Error("Error trying to Get Row Delete Data. SQL: " & sRowDeleteGameLine, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return 0
        End Function

        Public Function RowDeleteDataTicKetBet(ByVal poDate As String) As Integer
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sRowDeleteGameLine As String = ""
            Try
                sRowDeleteGameLine = String.Format("SELECT COUNT(TicketBetID) as RowDelete FROM TicketBets  WHERE TicketID in (SELECT TicketID FROM Tickets WHERE TransactionDate<= dateadd(day,-{0},getdate())) and TicketID not in (SELECT TicketID as RowDelete FROM Tickets  WHERE TransactionDate<= dateadd(day,-{0},getdate()) and ISNULL(IsForProp,'N')<>'Y') ", poDate)
                Return SafeInteger(odbSQL.getScalerValue(sRowDeleteGameLine))
                _log.Debug("Get Row Delete Data. SQL: " & sRowDeleteGameLine)
            Catch ex As Exception
                Return 0
                _log.Error("Error trying to Get Row Delete Data. SQL: " & sRowDeleteGameLine, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return 0
        End Function

        Public Function GetData(ByVal sTableName As String, ByVal poDate As String) As DataTable
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sRowDeleteGameLine As String = ""
            Dim odtData As DataTable
            Try
                Select Case UCase(sTableName)

                    Case "GAMES"
                        sRowDeleteGameLine = String.Format("SELECT * FROM Games  WHERE GameDate<= dateadd(day,-{1},getdate()) and ISNULL(IsForProp,'N')<>'Y' order by  GameDate desc  ", sTableName, poDate)
                    Case "GAMELINES"
                        sRowDeleteGameLine = String.Format("SELECT * FROM GameLines  WHERE createddate<= dateadd(day,-{1},getdate())  and GameID not in (SELECT GameID as RowDelete FROM GAMES  WHERE GameDate<= dateadd(day,-{1},getdate()) and ISNULL(IsForProp,'N')='Y')  order by createddate desc ", sTableName, poDate)
                    Case "TICKETS"
                        sRowDeleteGameLine = String.Format("SELECT * FROM Tickets  WHERE TransactionDate<= dateadd(day,-{1},getdate()) and ISNULL(IsForProp,'N')<>'Y' order by TransactionDate desc  ", sTableName, poDate)
                    Case "TICKETBETS"
                        sRowDeleteGameLine = String.Format("SELECT *  FROM TicketBets  WHERE TicketID in (SELECT TicketID FROM Tickets WHERE TransactionDate<= dateadd(day,-{1},getdate())) and TicketID not in (SELECT TicketID as RowDelete FROM Tickets  WHERE TransactionDate<= dateadd(day,-{1},getdate()) and ISNULL(IsForProp,'N')<>'Y')   ", sTableName, poDate)
                End Select

                odtData = odbSQL.getDataTable(sRowDeleteGameLine)
                _log.Debug("Get Row Data By Table And Date. SQL: " & sRowDeleteGameLine)
            Catch ex As Exception
                Return Nothing
                _log.Error("Error trying to Get Row Data By Table And Date. SQL: " & sRowDeleteGameLine, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return odtData
        End Function

    End Class
End Namespace
