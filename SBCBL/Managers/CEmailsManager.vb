Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CEmailsManager
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CEmailsManager))


        Public Function InsertEmail(ByVal psUserID As String, ByVal psFromID As String, ByVal psToID As String, ByVal psSiteType As String, _
                                           ByVal psSubject As String, ByVal psMessages As String, ByVal psReplyToAddress As String, Optional ByVal psIsSuperNotice As String = "N") As Boolean

            Dim bSuccess As Boolean = True
            Dim oInsert As ISQLEditStringBuilder = New CSQLInsertStringBuilder("Emails")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oInsert
                    .AppendString("EmailID", SQLString(newGUID))
                    .AppendString("FromID", SQLString(psFromID))
                    .AppendString("ToID", SQLString(psToID))
                    .AppendString("SentDate", SQLString(GetEasternDate))
                    .AppendString("SiteType", SQLString(psSiteType))
                    .AppendString("Subject", SQLString(psSubject))
                    .AppendString("IsMailOpen", SQLString("N"))
                    .AppendString("IsSuperMailOpen", SQLString("N"))
                    .AppendString("Messages", SQLString(psMessages))
                    .AppendString("ReplyToAddress", SQLString(psReplyToAddress))
                    .AppendString("IsSuperNotice", SQLString(psIsSuperNotice))
                End With

                _log.Debug("Insert Emails. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psUserID)

            Catch ex As Exception
                bSuccess = False
                _log.Error("Error trying to insert Emails. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function InsertReplyEmail(ByVal psUserID As String, ByVal psFromID As String, ByVal psToID As String, ByVal psSiteType As String, _
                                          ByVal psSubject As String, ByVal psMessages As String, ByVal psReplyToAddress As String, ByVal psReplyID As String) As Boolean

            Dim bSuccess As Boolean = True
            Dim oInsert As ISQLEditStringBuilder = New CSQLInsertStringBuilder("Emails")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oInsert
                    .AppendString("EmailID", SQLString(newGUID))
                    .AppendString("ReplyID", SQLString(psReplyID))
                    .AppendString("FromID", SQLString(psFromID))
                    .AppendString("ToID", SQLString(psToID))
                    .AppendString("SentDate", SQLString(GetEasternDate))
                    .AppendString("SiteType", SQLString(psSiteType))
                    .AppendString("Subject", SQLString(psSubject))
                    .AppendString("Messages", SQLString(psMessages))
                    .AppendString("IsAnswer", SQLString("Y"))
                    .AppendString("IsMailOpen", SQLString("N"))
                    .AppendString("IsSuperMailOpen", SQLString("N"))
                    .AppendString("ReplyToAddress", SQLString(psReplyToAddress))
                End With

                _log.Debug("Insert ReplyEmails. SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psUserID)

            Catch ex As Exception
                bSuccess = False
                _log.Error("Error trying to insert ReplyEmails. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function GetEmailsByUserID(ByVal psUserID As String, ByVal psSiteType As String, Optional ByVal psIsAnswer As String = "") As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            'oWhere.AppendOptionalANDCondition("IsAnswer", SQLString(psIsAnswer), "=")
            If String.IsNullOrEmpty(psIsAnswer) Then
                oWhere.AppendANDCondition("ReplyID is null")
            Else
                oWhere.AppendANDCondition("isnull(IsAnswer,'') =" & SQLString(psIsAnswer))
            End If

            oWhere.AppendANDCondition(String.Format("(FromID='{0}' or ToID='{0}' )", psUserID))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("isnull(IsDelete,'') <> 'Y'")
            Dim sSQL As String = "SELECT IsSuperNotice, EmailID,FromID,ToID,Subject,ReplyID, SUBSTRING(Messages, 1, 400) as Messages,IsAnswer,SentDate,isnull(IsMailOpen,'N') as IsMailOpen ,isnull(IsSuperMailOpen,'N') as IsSuperMailOpen from Emails" & vbCrLf & _
                 oWhere.SQL & " ORDER BY SentDate desc "
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Emails by UserID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtEmailsByUsers
        End Function

        Public Function GetEmailsBySuperAdminID(ByVal psSuperAdminID As String, ByVal psSubject As String, ByVal psSiteType As String) As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ToID=" & SQLString(psSuperAdminID))
            oWhere.AppendOptionalANDCondition("Subject", SQLString(psSubject), "=")
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("isnull(IsDelete,'') <> 'Y'")
            Dim sSQL As String = "SELECT IsSuperNotice,EmailID,FromID,Subject, IsAnswer,SentDate,isnull(IsMailOpen,'N') as IsMailOpen ,isnull(IsSuperMailOpen,'N') as IsSuperMailOpen, SUBSTRING(Messages, 1, 400) as Messages from Emails" & vbCrLf & _
                 oWhere.SQL & " ORDER BY SentDate desc "
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Emails by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtEmailsByUsers
        End Function

        Public Function GetEmailsByEmailID(ByVal psEmailID As String) As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("EmailID=" & SQLString(psEmailID))
            Dim sSQL As String = "SELECT IsSuperNotice,EmailID,Subject, Messages,ReplyToAddress,FromID,ToID,IsAnswer,isnull(IsMailOpen,'N') as IsMailOpen ,isnull(IsSuperMailOpen,'N') as IsSuperMailOpen from Emails" & vbCrLf & _
                 oWhere.SQL & " ORDER BY SentDate desc "
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Emails by EmailID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return odtEmailsByUsers
        End Function

        Public Function GetEmailsByReplyID(ByVal psReplyID As String) As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ReplyID=" & SQLString(psReplyID))
            Dim sSQL As String = "SELECT IsSuperNotice,EmailID,Subject, Messages,ReplyToAddress,FromID,ToID,isnull(IsMailOpen,'N') as IsMailOpen ,isnull(IsSuperMailOpen,'N') as IsSuperMailOpen from Emails" & vbCrLf & _
                 oWhere.SQL & " ORDER BY SentDate desc "
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Emails by ReplyID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return odtEmailsByUsers
        End Function

        Public Function GetReplyEmailsByUserID(ByVal psUserID As String) As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ReplyID is not null")
            oWhere.AppendANDCondition("ToID=" & SQLString(psUserID))
            Dim sSQL As String = "SELECT IsSuperNotice,EmailID,Subject,  SUBSTRING(Messages, 1, 400) as Messages ,ReplyToAddress,FromID,ToID,IsAnswer,ReplyID,SentDate,isnull(IsMailOpen,'N') as IsMailOpen ,isnull(IsSuperMailOpen,'N') as IsSuperMailOpen from Emails" & vbCrLf & _
                 oWhere.SQL & " ORDER BY SentDate desc "
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  reply Emails by UserID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return odtEmailsByUsers
        End Function

        Public Function UpdateEmailAnswerByID(ByVal psEmailID As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            Dim oUpdate As New CSQLUpdateStringBuilder("Emails", "WHERE EmailID= " & SQLString(psEmailID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("IsAnswer", SQLString("Y"))
                End With
                _log.Debug("Update Email. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                bSuccess = True
            Catch ex As Exception
                _log.Error("Error trying to update Email Answer. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateEmailOpenByID(ByVal psEmailOpenID As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            Dim oUpdate As New CSQLUpdateStringBuilder("Emails", "WHERE EmailID= " & SQLString(psEmailOpenID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("IsMailOpen", SQLString("Y"))
                End With
                _log.Debug("Update Email. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                bSuccess = True
            Catch ex As Exception
                _log.Error("Error trying to update Open Email. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateSuperEmailOpenByID(ByVal psIEmailID As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            Dim oUpdate As New CSQLUpdateStringBuilder("Emails", "WHERE EmailID= " & SQLString(psIEmailID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("IsSuperMailOpen", SQLString("Y"))
                End With
                _log.Debug("Update Email. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                bSuccess = True
            Catch ex As Exception
                _log.Error("Error trying to update Super Open Email. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateReplyEmailOpenByID(ByVal psEmailOpenID As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            Dim oUpdate As New CSQLUpdateStringBuilder("Emails", "WHERE ReplyID= " & SQLString(psEmailOpenID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("IsMailOpen", SQLString("Y"))
                End With
                _log.Debug("Update Email. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                bSuccess = True
            Catch ex As Exception
                _log.Error("Error trying to Update Reply Email OPen . SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function GetSubjectEmails(ByVal psSuperAdminID As String, ByVal psSiteType As String) As DataTable
            Dim odtEmailsByUsers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ToID=" & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("isnull(IsDelete,'') <> 'Y'")
            Dim sSQL As String = "SELECT Distinct Subject from Emails" & oWhere.SQL
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtEmailsByUsers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                _log.Error("Cannot get  Subject Emails by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtEmailsByUsers
        End Function

        Public Function CheckExistsEmailReply(ByVal psUserId As String) As Boolean
            Dim bExistsEmailReply As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ToID=" & SQLString(psUserId))
            oWhere.AppendANDCondition("isnull(IsDelete,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(IsMailOpen,'') <> 'Y'")
            Dim sSQL As String = "SELECT EmailID from Emails" & oWhere.SQL
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                bExistsEmailReply = odbSQL.getDataTable(sSQL).Rows.Count > 0
            Catch ex As Exception
                _log.Error("Cannot get  Subject Emails by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bExistsEmailReply
        End Function

        Public Function CheckSuperExistsEmailOpen(ByVal psUserId As String) As Boolean
            Dim bExistsEmailReply As Boolean = False
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ToID=" & SQLString(psUserId))
            oWhere.AppendANDCondition("isnull(IsDelete,'') <> 'Y'")
            oWhere.AppendANDCondition("isnull(IsSuperMailOpen,'') <> 'Y'")
            Dim sSQL As String = "SELECT EmailID from Emails" & oWhere.SQL
            _log.Debug("Get the list of Emails. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                bExistsEmailReply = odbSQL.getDataTable(sSQL).Rows.Count > 0
            Catch ex As Exception
                _log.Error("Cannot get  Subject Emails by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bExistsEmailReply
        End Function


    End Class
End Namespace