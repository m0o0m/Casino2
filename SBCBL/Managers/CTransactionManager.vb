Imports SBCBL.std
Imports SBCBL.CEnums
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CTransactionManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetTransactions(ByVal psPaymentID As String, ByVal psWithdrawID As String) As DataTable
            Dim odtTransactions As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PaymentID = " & SQLString(psPaymentID))
            oWhere.AppendANDCondition("WithdrawID = " & SQLString(psWithdrawID))
            oWhere.AppendANDCondition("TransactionDate >= " & SQLDate(GetMondayOfCurrentWeek.AddDays(-24).ToShortDateString))

            Dim sSQL As String = "SELECT '' AS FullName, * FROM Transactions " & oWhere.SQL & _
                " ORDER BY TransactionDate DESC"
            log.Debug(String.Format("Get the list of Transaction by WithdrawID: {0} and PaymentID: {1}. SQL: {2}", psWithdrawID, psPaymentID, sSQL))

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTransactions = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error(String.Format("Cannot get the list of Transaction by WithdrawID: {0} and PaymentID: {1}. SQL: {2}", psWithdrawID, psPaymentID, sSQL), ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTransactions
        End Function

        Public Function GetUsersAmount(ByVal psPaymentID As String, ByVal peUserType As ETransactionUserType) As DataTable
            Dim odtTransactions As DataTable = Nothing

            Dim sSQL As String = "SELECT (u.Login + ' (' + u.Name + ')') as FullName, t.* "

            If peUserType = ETransactionUserType.Agent Then '' Get Player's info
                sSQL &= String.Format(" FROM Players u LEFT OUTER JOIN Transactions t " & _
                                      " ON u.PlayerID = t.WithdrawID AND t.PaymentID = {0} AND t.TransactionDate >=  {1}", _
                                       SQLString(psPaymentID), SQLDate(GetMondayOfCurrentWeek.AddDays(-24).ToShortDateString))
            Else '' Get Agent's Info
                sSQL &= String.Format(" FROM Agents u LEFT OUTER JOIN Transactions t " & _
                                      " ON u.AgentID = t.WithdrawID AND t.PaymentID = {0} AND t.TransactionDate >=  {1}", _
                                       SQLString(psPaymentID), SQLDate(GetMondayOfCurrentWeek.AddDays(-24).ToShortDateString))
            End If

            Dim oWhere As New CSQLWhereStringBuilder
            If peUserType = ETransactionUserType.Agent Then '' Get Player's info by AgentID
                oWhere.AppendANDCondition("u.AgentID = " & SQLString(psPaymentID))
            ElseIf peUserType = ETransactionUserType.SuperAdmin Then '' Get Agent's info By SuperAdminID
                oWhere.AppendANDCondition("u.SuperAdminID = " & SQLString(psPaymentID))
            Else '' Get Agent's Info by SuperAgentID
                oWhere.AppendANDCondition("u.ParentID = " & SQLString(psPaymentID))
            End If
            oWhere.AppendANDCondition("ISNULL(u.IsLocked,'') <> " & SQLString("Y"))

            sSQL &= oWhere.SQL & " ORDER BY u.Login, u.Name, t.TransactionDate DESC"
            log.Debug(String.Format("Get list of User's Amount. SQL: {0}", sSQL))

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTransactions = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error(String.Format("Failt to get list of User's Amount. SQL: {0}", sSQL), ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTransactions
        End Function

        Public Function AddTransaction(ByVal pnTransactionAmount As Double, ByVal psDescription As String, _
                                       ByVal psPaymentID As String, ByVal psWithdrawID As String, _
                                       ByVal peTransactionType As CEnums.ETransactionType, ByVal peUserType As ETransactionUserType) As Boolean
            Dim bResult As Boolean = False

            Dim oEdit As ISQLEditStringBuilder
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim sSQL As String = ""
            Dim sTransactionType As String = ""

            Try


                If peUserType = ETransactionUserType.Agent Then
                    '' Get Player's Amount First
                    sSQL = "SELECT BalanceForward, BalanceAmount, OriginalAmount FROM Players WHERE PlayerID = " & SQLString(psWithdrawID)
                Else
                    '' Get Agent's Amount First
                    sSQL = "SELECT BalanceForward, BalanceAmount FROM Agents WHERE AgentID = " & SQLString(psWithdrawID)
                End If
                Dim otbl As DataTable = odbSQL.getDataTable(sSQL)

                If otbl IsNot Nothing AndAlso otbl.Rows.Count > 0 Then
                    Dim nWithdrawBA, nWithdrawBF, nWithdrawOriginalAmout, nPaymentBA, nAmountPaid, nPercentage As Double
                    nPaymentBA = 0
                    nWithdrawOriginalAmout = 0
                    nPercentage = 0

                    Dim oCurrentDate As Date = Date.Now.ToUniversalTime
                    Dim oStartDate As Date = GetLastMondayOfDate(oCurrentDate)
                    If oCurrentDate.DayOfWeek = DayOfWeek.Monday Or oCurrentDate.DayOfWeek = DayOfWeek.Tuesday Then
                        oStartDate.AddDays(-7)
                    End If

                    If peUserType = ETransactionUserType.Agent Then
                        '' Get Player's Amount First
                        Dim oPlayer As CacheUtils.CPlayer = (New CacheUtils.CCacheManager).GetPlayerInfo(psWithdrawID)
                        nWithdrawOriginalAmout = oPlayer.OriginalAmount
                        nWithdrawBF = oPlayer.BalanceForward
                        nWithdrawBA = (New CTicketManager).GetPLAmount(psWithdrawID, oStartDate, oStartDate.AddDays(6), True)
                    Else
                        '' Get Agent's Amount First
                        Dim oAgent As CacheUtils.CAgent = (New CacheUtils.CCacheManager).GetAgentInfo(psWithdrawID)
                        nWithdrawBF = oAgent.BalanceForward
                        nWithdrawBA = (New CTicketManager).GetPLAmount(psWithdrawID, oStartDate, oStartDate.AddDays(6))
                        nPercentage = oAgent.ProfitPercentage
                    End If

                    'nAmountPaid = SafeRound(nWithdrawBA + nWithdrawBF - nWithdrawOriginalAmout)
                    If nPercentage > 0 Then
                        nAmountPaid = SafeRound((nWithdrawBA * nPercentage / 100) + nWithdrawBF)
                    Else
                        nAmountPaid = SafeRound(nWithdrawBA + nWithdrawBF)
                    End If

                    If peUserType = ETransactionUserType.Agent OrElse peUserType = ETransactionUserType.SuperAgent Then
                        '' Get SuperAgent's Amount or Get Agent's Amount
                        sSQL = "SELECT BalanceAmount FROM Agents WHERE AgentID = " & SQLString(psPaymentID)
                        nPaymentBA = SafeDouble(odbSQL.getScalerValue(sSQL))
                    End If

                    '' Insert Transaction
                    oEdit = New CSQLInsertStringBuilder("Transactions")
                    With oEdit
                        .AppendString("TransactionID", SQLString(newGUID))
                        .AppendString("TransactionDate", SQLDate(GetEasternDate))
                        .AppendString("BalanceForward", SQLDouble(nWithdrawBF))
                        .AppendString("AmountOwed", SQLDouble(nWithdrawBA))
                        If nPercentage > 0 Then
                            .AppendString("Commission", SQLDouble(SafeRound(nWithdrawBA * nPercentage / 100)))
                        End If
                        .AppendString("Description", SQLString(psDescription))
                        .AppendString("WithdrawID", SQLString(psWithdrawID))
                        .AppendString("PaymentID", SQLString(psPaymentID))
                    End With

                    If peTransactionType = CEnums.ETransactionType.Deposit Then
                        nWithdrawBF = pnTransactionAmount + nAmountPaid

                        nPaymentBA = nPaymentBA + pnTransactionAmount
                        sTransactionType = "Deposit"

                        pnTransactionAmount = -pnTransactionAmount
                    Else
                        '' Can not withdraw if Total Amount < 0
                        If nAmountPaid < 0 Then
                            Throw New CTransactionException(String.Format("Please choose Deopsit instead of Withdrawal."))
                        End If

                        '' Can not withdraw amount greater than Balance Amount - Balance Forward
                        If pnTransactionAmount > SafeRound(nAmountPaid) Then
                            Throw New CTransactionException(String.Format("Can not withdraw greater than {0}.", _
                                                                          SafeRound(nAmountPaid)))
                        End If

                        nWithdrawBF = nAmountPaid - pnTransactionAmount

                        nPaymentBA = nPaymentBA - pnTransactionAmount
                        sTransactionType = "Withdraw"
                    End If

                    If peUserType = ETransactionUserType.Agent Then
                        '' NOTE: Now we don't handle this anymore, so this maybe wrong in the future.
                        nWithdrawBA = nWithdrawOriginalAmout - (New CTicketManager).GetPlayerPendingAmount(psWithdrawID, Nothing)
                    ElseIf peUserType = ETransactionUserType.SuperAgent Then
                        nWithdrawBA = 0
                    End If

                    oEdit.AppendString("TransactionAmount", SQLDouble(pnTransactionAmount))
                    oEdit.AppendString("TransactionType", SQLString(sTransactionType))
                    sSQL = oEdit.SQL

                    log.Debug("Insert Transaction. SQL: " & oEdit.SQL)
                    odbSQL.executeNonQuery(oEdit, psWithdrawID, True)

                    If peUserType = ETransactionUserType.Agent Then
                        '' Update Player's Amount
                        oEdit = New CSQLUpdateStringBuilder("Players", "WHERE PLayerID=" & SQLString(psWithdrawID))
                    Else
                        '' Update Agent's Amount
                        oEdit = New CSQLUpdateStringBuilder("Agents", "WHERE AgentID=" & SQLString(psWithdrawID))
                    End If

                    With oEdit
                        .AppendString("BalanceAmount", SQLDouble(nWithdrawBA))
                        .AppendString("BalanceForward", SQLDouble(nWithdrawBF))
                    End With

                    sSQL = oEdit.SQL
                    log.Debug("Update Withdraw's Amount. SQL: " & oEdit.SQL)
                    odbSQL.executeNonQuery(oEdit, psWithdrawID, True)

                    If peUserType = ETransactionUserType.Agent OrElse peUserType = ETransactionUserType.SuperAgent Then
                        '' Update SuperAgent's Amount or Update Agent's Amount
                        oEdit = New CSQLUpdateStringBuilder("Agents", "WHERE AgentID=" & SQLString(psPaymentID))
                        With oEdit
                            .AppendString("BalanceAmount", SQLDouble(nPaymentBA))
                        End With

                        sSQL = oEdit.SQL
                        log.Debug("Update Payment's Amount. SQL: " & oEdit.SQL)
                        odbSQL.executeNonQuery(oEdit, psWithdrawID, True)
                    End If

                    odbSQL.commitTransaction()
                    bResult = True
                End If
            Catch tx As CTransactionException
                Throw New CTransactionException(tx.Message)

            Catch ex As Exception
                log.Error("Error trying to exec SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function DeleteTransaction(ByVal psTransactionID As String, ByVal psDeleteBy As String) As Boolean
            Dim bResult As Boolean = False

            Dim sSQL As String = ""
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("TransactionID = " & SQLString(psTransactionID))
                Dim oDelete As New CSQLDeleteStringBuilder("Transactions", oWhere.SQL)
                sSQL = oDelete.SQL

                odbSQL.executeNonQuery(oDelete, psDeleteBy)

                bResult = True
            Catch ex As Exception
                log.Error("Error trying to exec SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bResult
        End Function
    End Class

    Public Class CTransactionException
        Inherits System.Exception

        Public Sub New(ByVal psMessage As String)
            MyBase.New(psMessage)
        End Sub
    End Class
End Namespace