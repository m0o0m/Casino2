Imports SBCBL.std
Imports WebsiteLibrary.DBUtils

Namespace Managers
    Public Class CAgentManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim strKeyGetAllSubAgentIDs As String = "GetAllSubAgentIDs_"
        Public Function GetActivePlayersByTypeOfBet(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date, ByVal psTypeOfBet As String) As DataTable
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, [FullName], [Level]) AS " & vbCrLf & _
            " (    SELECT AgentID, ParentID, [Login] , 0 as [Level] " & vbCrLf & _
            " FROM Agents WHERE AgentID={0} AND ISNULL(IsLocked,'N') <> 'Y' " & vbCrLf & _
            " UNION ALL " & vbCrLf & _
            " SELECT child.AgentID, child.ParentID,child.Login , AgentRecursive.[Level]+1 as [Level] " & vbCrLf & _
            " FROM Agents as child " & vbCrLf & _
            " INNER JOIN AgentRecursive ON (AgentRecursive.AgentID = child.ParentID AND ISNULL(child.IsLocked,'N') <> 'Y') " & vbCrLf & _
            " ) " & vbCrLf & _
            "  SELECT distinct ar.[Level], ar.FullName AS AgentFullName, p.PlayerID, p.[Login] AS PlayerFullName  " & vbCrLf & _
            " FROM AgentRecursive ar LEFT JOIN Players p ON (ar.AgentID = p.AgentID) inner join Tickets as t on (t.PlayerID= p.PlayerID and t.TransactionDate between '{1}' and '{2}' and TypeOfBet='{3}') " & vbCrLf & _
            " ORDER BY ar.[Level], ar.FullName, PlayerFullName "

            sSQL = String.Format(sSQL, SQLString(psAgentID), SafeString(poStartDate), SafeString(poEndDate), SafeString(psTypeOfBet))
            Dim otblResult As DataTable = Nothing
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                log.Debug("Get All Active Players. SQL: " & sSQL)
                otblResult = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get all Active Players: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return otblResult
        End Function

        Public Function GetActivePlayersByCasino(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, [FullName], [Level]) AS " & vbCrLf & _
            " (    SELECT AgentID, ParentID, [Login] , 0 as [Level] " & vbCrLf & _
            " FROM Agents WHERE AgentID={0} AND ISNULL(IsLocked,'N') <> 'Y' " & vbCrLf & _
            " UNION ALL " & vbCrLf & _
            " SELECT child.AgentID, child.ParentID,child.Login , AgentRecursive.[Level]+1 as [Level] " & vbCrLf & _
            " FROM Agents as child " & vbCrLf & _
            " INNER JOIN AgentRecursive ON (AgentRecursive.AgentID = child.ParentID AND ISNULL(child.IsLocked,'N') <> 'Y') " & vbCrLf & _
            " ) " & vbCrLf & _
            "    SELECT sum(NetAmount-RiskAmount) as NetAmount " & vbCrLf & _
            " FROM AgentRecursive ar LEFT JOIN Players p ON (ar.AgentID = p.AgentID) inner join Tickets as t on (t.PlayerID= p.PlayerID and t.TransactionDate between '1/2/2012' and '1/8/2012') inner join TicketBets as tb on (t.TicketID=tb.TicketID ) inner join Games as g on(g.GameID=tb.GameID and HomeTeam like '%Casino%' or AwayTeam like '%Casino%') "


            sSQL = String.Format(sSQL, SQLString(psAgentID), SafeString(poStartDate), SafeString(poEndDate))
            Dim otblResult As DataTable = Nothing
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                log.Debug("Get All NetAmount casino. SQL: " & sSQL)
                otblResult = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get all NetAmount casino: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return otblResult
        End Function

        Public Function GetActivePlayersForAgent(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, [FullName], [Level]) AS " & vbCrLf & _
            " (    SELECT AgentID, ParentID, [Login] , 0 as [Level] " & vbCrLf & _
            " FROM Agents WHERE AgentID={0} AND ISNULL(IsLocked,'N') <> 'Y' " & vbCrLf & _
            " ) " & vbCrLf & _
            "  SELECT distinct ar.[Level], ar.FullName AS AgentFullName, p.PlayerID, p.[Login] AS PlayerFullName  " & vbCrLf & _
            " FROM AgentRecursive ar LEFT JOIN Players p ON (ar.AgentID = p.AgentID) inner join Tickets as t on (t.PlayerID= p.PlayerID and t.TransactionDate between '{1}' and '{2}') " & vbCrLf & _
            " ORDER BY ar.[Level], ar.FullName, PlayerFullName "

            sSQL = String.Format(sSQL, SQLString(psAgentID), SafeString(poStartDate), SafeString(poEndDate))
            Dim otblResult As DataTable = Nothing
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                log.Debug("Get All Active Players. SQL: " & sSQL)
                otblResult = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get all Active Players: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return otblResult
        End Function

        Public Function GetAllAgents() As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = "SELECT * FROM Agents where isnull(IsLocked,'') <> 'Y' order by Login"

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get list of Agents.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        ''' <summary>
        ''' Get Agent's Info By AgentID
        ''' </summary>
        ''' <param name="psAgentID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByID(ByVal psAgentID As String) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = "SELECT TOP 1 * FROM Agents WHERE AgentID = " & SQLString(psAgentID)
            Try
                log.Debug("Get Agent's Info by AgentID: " & psAgentID)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Agent by ID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        ''' <summary>
        ''' Get all SubAgents of SuperAdmin
        ''' </summary>
        ''' <param name="psSuperAdminID"></param>
        ''' <param name="bpIsBettingLocked"></param>
        ''' <param name="pbIsLocked"></param>
        ''' <param name="psNameOrLogin"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAgentsBySuperID(ByVal psSuperAdminID As String, ByVal bpIsBettingLocked As Boolean?, _
                                           Optional ByVal pbIsLocked As Boolean = False, Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtPlayers As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SuperAdminID=" & SQLString(psSuperAdminID))
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(Name LIKE {0} OR Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName FROM Agents " & oWhere.SQL & " ORDER BY Login, Name "
            log.Debug("Get the list of agents by SuperID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtPlayers = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of agents . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtPlayers
        End Function

        ''' <summary>
        ''' Get all SubAgents of Agent
        ''' </summary>
        ''' <param name="psAgentID"></param>
        ''' <param name="bpIsBettingLocked"></param>
        ''' <param name="pbIsLocked"></param>
        ''' <param name="psNameOrLogin"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAgentsByAgentID(ByVal psAgentID As String, ByVal bpIsBettingLocked As Boolean?, _
                                           Optional ByVal pbIsLocked As Boolean = False , Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("ParentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(Name LIKE {0} OR Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName FROM Agents " & oWhere.SQL & _
            " ORDER BY IsLocked, Login, Name "

            Try
                log.Debug("Get list of subagents by agentID.SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get Agents by AgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAgentByAgentID(ByVal psAgentID As String, Optional ByVal pbIsLocked As Boolean = False ) As DataRow
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            
            Dim sSQL As String = "SELECT *,(Login + ' (' + Name + ')') as FullName FROM Agents " & oWhere.SQL & _
            " ORDER BY IsLocked, Login, Name "

            Try
                log.Debug("Get list of subagents by agentID.SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL).Rows(0)
            Catch ex As Exception
                log.Error("Fails to get Agents by AgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        ''' <summary>
        ''' Lock/Unlock multi Agents
        ''' </summary>
        ''' <param name="pLstAgents"></param>
        ''' <param name="pbLocked"></param>
        ''' <param name="psUpdateBy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LockAgents(ByVal pLstAgents As List(Of String), ByVal pbLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            Try
                Dim oLstAgentID As New List(Of String)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sAgentID As String In pLstAgents
                    Dim oAgentInfo As String() = sAgentID.Split("|"c)
                    oCache.ClearAgentInfo(oAgentInfo(0), oAgentInfo(1))
                    oLstAgentID.Add(oAgentInfo(0))
                Next

                log.Debug("Lock/Unlock these Agents(AgentID|Login): " & Join(oLstAgentID.ToArray(), ","))
                oUpdate = New CSQLUpdateStringBuilder("Agents", String.Format("WHERE AgentID IN ('{0}')", Join(oLstAgentID.ToArray(), "','")))
                oUpdate.AppendString("IsLocked", SQLString(IIf(pbLocked, "Y", "N")))
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                oDB.executeNonQuery(oUpdate, psUpdateBy)

                Return True
            Catch ex As Exception
                log.Error("Fails to lock/unlock agent.SQL: " & SafeString(IIf(oUpdate IsNot Nothing, oUpdate.SQL, "")), ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        ''' <summary>
        ''' BettingLock/BettingUnlock multi Agents
        ''' </summary>
        ''' <param name="pLstAgents"></param>
        ''' <param name="pbBettingLocked"></param>
        ''' <param name="psUpdateBy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BettingLockAgents(ByVal pLstAgents As List(Of String), ByVal pbBettingLocked As Boolean, ByVal psUpdateBy As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing
            Dim oUpdate As CSQLUpdateStringBuilder = Nothing

            Try
                Dim oLstAgentID As New List(Of String)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sAgentID As String In pLstAgents
                    Dim oAgentInfo As String() = sAgentID.Split("|"c)
                    oCache.ClearAgentInfo(oAgentInfo(0), oAgentInfo(1))
                    oLstAgentID.Add(oAgentInfo(0))
                Next

                log.Debug("BettingLock/BettingUnlock these Agents(AgentID|Login): " & Join(oLstAgentID.ToArray(), ","))
                oUpdate = New CSQLUpdateStringBuilder("Agents", String.Format("WHERE AgentID IN ('{0}')", Join(oLstAgentID.ToArray(), "','")))
                oUpdate.AppendString("IsBettingLocked", SQLString(IIf(pbBettingLocked, "Y", "N")))
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                oDB.executeNonQuery(oUpdate, psUpdateBy)

                Return True
            Catch ex As Exception
                log.Error("Fails to lock/unlock agent.SQL: " & oUpdate.SQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function InsertAgent(ByVal psAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, _
                                     ByVal pnTimeZone As Integer, ByVal pbIsBettingLocked As Boolean, ByVal pbIsLocked As Boolean, _
                                      ByVal psLockReason As String, ByVal psParentID As String, ByVal psSuperAdminID As String, _
                                     ByVal piProfitPercentage As Single, ByVal piGrossPercentage As Single, ByVal psSpecialKey As String, _
                                     ByVal pnPlayerNumber As Integer, ByVal psCreatedBy As String, ByVal psSiteType As String, _
                                     ByVal pbIsEnablePlayerTemplate As Boolean, ByVal pbIsEnableBlockPlayer As Boolean, _
                                     ByVal pbRequirePasswordChange As Boolean, ByVal pbHasCasino As Boolean, ByVal pbAddNewSubAgent As Boolean, Optional ByVal pnHasCrediLimitSetting As Boolean = False, Optional ByVal pbIsEnableBettingProfile As Boolean = False, _
                                     Optional ByVal pbHasGameManagement As Boolean = False, Optional ByVal pbHasSystemManagement As Boolean = False, _
                                     Optional ByVal pbIsEnableChangeBookmaker As Boolean = False) As Boolean

            Dim bSuccess As Boolean = False

            Dim oInsert As New CSQLInsertStringBuilder("Agents")
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oInsert
                    .AppendString("AgentID", SQLString(psAgentID))
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    '.AppendString("Password", SQLString(EncryptToHash(psPassword)))
                    .AppendString("Password", SQLString(psPassword))
                    .AppendString("Password", SQLString(psPassword))
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("ProfitPercentage", SQLDouble(piProfitPercentage))
                    .AppendString("GrossPercentage", SQLDouble(piGrossPercentage))
                    .AppendString("SpecialKey", SQLString(psSpecialKey))
                    .AppendString("CurrentPlayerNumber", SQLString(pnPlayerNumber))
                    .AppendString("SiteType ", SQLString(psSiteType))
                    .AppendString("IsEnablePlayerTemplate", SQLString(IIf(pbIsEnablePlayerTemplate, "Y", "N")))
                    .AppendString("IsEnableBlockPlayer", SQLString(IIf(pbIsEnableBlockPlayer, "Y", "N")))
                    .AppendString("RequirePasswordChange", SQLString(IIf(pbRequirePasswordChange, "Y", "N")))
                    .AppendString("IsEnableBettingProfile", SQLString(IIf(pbIsEnableBettingProfile, "Y", "N")))
                    .AppendString("IsEnableChangeBookmaker", SQLString(IIf(pbIsEnableChangeBookmaker, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                    .AppendString("HasGameManagement", SQLString(IIf(pbHasGameManagement, "Y", "N")))
                    .AppendString("HasSystemManagement", SQLString(IIf(pbHasSystemManagement, "Y", "N")))
                    .AppendString("HasCrediLimitSetting", SQLString(IIf(pnHasCrediLimitSetting, "Y", "N")))
                    .AppendString("AddNewSubAgent", SQLString(IIf(pbAddNewSubAgent, "Y", "N")))
                    If psParentID <> "" Then
                        .AppendString("ParentID", SQLString(psParentID))
                        .AppendString("SuperAdminID", "NULL")
                    Else
                        .AppendString("ParentID", "NULL")
                        .AppendString("SuperAdminID", SQLString(psSuperAdminID))
                    End If
                    .AppendString("CreatedDate", SQLString(Date.Now.ToUniversalTime))
                    .AppendString("CreatedBy", SQLString(psCreatedBy))
                End With

                log.Debug("Insert Agent.SQL: " & oInsert.SQL)
                odbSQL.executeNonQuery(oInsert, psCreatedBy)
               ClearAgentCache()
                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to insert Agent. SQL: " & oInsert.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateAgent(ByVal psAgentID As String, ByVal psName As String, ByVal psLogin As String, ByVal psPassword As String, _
                                     ByVal pnTimeZone As Integer, ByVal pbIsBettingLocked As Boolean, ByVal pbIsLocked As Boolean, _
                                      ByVal psLockReason As String, ByVal psParentID As String, ByVal psSuperAdminID As String, _
                                      ByVal piProfitPercentage As Single, ByVal piGrossPercentage As Single, ByVal psSpecialKey As String, _
                                      ByVal pnPlayerNumber As Integer, ByVal psChangedBy As String, ByVal pbIsEnablePlayerTemplate As Boolean, _
                                      ByVal pbIsEnableBlockPlayer As Boolean, ByVal pbHasCasino As Boolean, Optional ByVal pbAddNewSubAgent As Boolean = False, Optional ByVal pnHasCrediLimitSetting As Boolean = False, Optional ByVal psIsEnableBettingProfile As String = "", _
                                      Optional ByVal pbHasGameManagement As Boolean = False, Optional ByVal pbHasSystemManagement As Boolean = False, _
                                      Optional ByVal pbIsEnableChangeBookmaker As Boolean = False) As Boolean

            Dim bSuccess As Boolean = False

            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", "WHERE AgentID= " & SQLString(psAgentID))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                With oUpdate
                    .AppendString("Name", SQLString(psName))
                    .AppendString("Login", SQLString(psLogin))
                    If psPassword <> "" Then
                        '  .AppendString("Password", SQLString(EncryptToHash(psPassword)))
                        .AppendString("Password", SQLString(psPassword))
                        .AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
                    End If
                    .AppendString("TimeZone", SafeString(pnTimeZone))
                    .AppendString("IsBettingLocked", SQLString(IIf(pbIsBettingLocked, "Y", "N")))
                    .AppendString("IsLocked", SQLString(IIf(pbIsLocked, "Y", "N")))
                    .AppendString("LockReason", SQLString(psLockReason))
                    .AppendString("ProfitPercentage", SQLDouble(piProfitPercentage))
                    .AppendString("GrossPercentage", SQLDouble(piGrossPercentage))
                    .AppendString("SpecialKey", SQLString(psSpecialKey))
                    '.AppendString("CurrentPlayerNumber", SQLString(pnPlayerNumber))
                    .AppendString("IsEnablePlayerTemplate", SQLString(IIf(pbIsEnablePlayerTemplate, "Y", "N")))
                    .AppendString("IsEnableBlockPlayer", SQLString(IIf(pbIsEnableBlockPlayer, "Y", "N")))
                    .AppendString("IsEnableChangeBookmaker", SQLString(IIf(pbIsEnableChangeBookmaker, "Y", "N")))
                    .AppendString("HasCasino", SQLString(IIf(pbHasCasino, "Y", "N")))
                    .AppendString("HasGameManagement", SQLString(IIf(pbHasGameManagement, "Y", "N")))
                    .AppendString("HasSystemManagement", SQLString(IIf(pbHasSystemManagement, "Y", "N")))
                    .AppendString("HasCrediLimitSetting", SQLString(IIf(pnHasCrediLimitSetting, "Y", "N")))
                    .AppendString("AddNewSubAgent", SQLString(IIf(pbAddNewSubAgent, "Y", "N")))
                    If psIsEnableBettingProfile <> "" Then
                        .AppendString("IsEnableBettingProfile", SQLString(psIsEnableBettingProfile))
                    End If
                    If psParentID = "" Then
                        .AppendString("ParentID", "NULL")
                        .AppendString("SuperAdminID", SQLString(psSuperAdminID))
                    Else
                        .AppendString("ParentID", SQLString(psParentID))
                        .AppendString("SuperAdminID", "NULL")
                    End If
                End With

                log.Debug("Update Agent. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                ClearAgentCache()
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearAgentInfo(psAgentID, psName)
                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to save Agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function UpdateColorScheme(ByVal psAgentID As String, ByVal psColorScheme As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            '' Get SubAgents
            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", String.Format("WHERE AgentID={0}", SQLString(psAgentID)))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("ColorScheme", SQLString(psColorScheme))
                End With
                log.Debug("Update Agent's ColorScheme. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearAgentInfo(psAgentID, "")
                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to update Agent's ColorScheme. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        Public Function UpdateBackgroundImage(ByVal psAgentID As String, ByVal psBackgroundImage As String, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False
            '' Get SubAgents
            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", String.Format("WHERE AgentID={0}", SQLString(psAgentID)))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Try
                With oUpdate
                    .AppendString("BackgroundImage", SQLString(psBackgroundImage))
                End With
                log.Debug("Update Agent's BackgroundImage. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)
                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearAgentInfo(psAgentID, "")
                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to update Agent's BackgroundImage. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return bSuccess
        End Function

        


        Public Function UpdateSubAgentsPercent(ByVal psAgentID As String, ByVal piProfitPercentage As Single, _
                                               ByVal piGrossPercentage As Single, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False

            '' Get SubAgents
            Dim oLstAgentIDs As New List(Of String)
            oLstAgentIDs = (New CAgentManager).GetAllSubAgentIDs(psAgentID)

            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", String.Format("WHERE AgentID IN('{0}') ", Join(oLstAgentIDs.ToArray(), "','")))
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("ProfitPercentage", SQLDouble(piProfitPercentage))
                    .AppendString("GrossPercentage", SQLDouble(piGrossPercentage))
                End With

                log.Debug("Update Agent's Percentage. SQL: " & oUpdate.SQL)
                odbSQL.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                For Each sAgentID As String In oLstAgentIDs
                    oCache.ClearAgentInfo(psAgentID, "")
                Next

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to update Agent's Percentage. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bSuccess
        End Function

        Public Function ChangePassword(ByVal psAgentID As String, ByVal psCurrentPassword As String, ByVal psNewPassword As String, ByVal psChangedBy As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("Password=" & SQLString(psCurrentPassword))

            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", oWhere.SQL)
            ' oUpdate.AppendString("Password", SQLString(EncryptToHash(psNewPassword)))
            oUpdate.AppendString("Password", SQLString(psNewPassword))
            oUpdate.AppendString("PasswordLastUpdated", SQLString(Date.Now.ToUniversalTime))
            oUpdate.AppendString("RequirePasswordChange", SQLString("N"))

            log.Debug("Change password agent. SQL: " & oUpdate.SQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                Return odbSQL.executeNonQuery(oUpdate, psChangedBy) > 0
            Catch ex As Exception
                log.Error("Cannot change password agent. SQL: " & oUpdate.SQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
        End Function

        Public Function NumOfSubAgents(ByVal psAgentID As String) As Integer
            If SafeString(psAgentID) <> "" Then
                Dim oDB As CSQLDBUtils = Nothing
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("ParentID = " & SQLString(psAgentID))
                oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> " & SQLString("Y"))

                Dim sSQL As String = "SELECT COUNT(*) FROM Agents " & oWhere.SQL

                Try
                    log.Debug("Get number of subagents. SQL: " & sSQL)
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                    Return SafeInteger(oDB.getScalerValue(sSQL))
                Catch ex As Exception
                    log.Error("Fails to get Agents by AgentID.SQL: " & sSQL, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
            End If

            Return 0
        End Function

        Public Function NumOfSubPlayers(ByVal psAgentID As String) As Integer
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition("ISNULL(IsLocked,'') <> " & SQLString("Y"))

            Dim sSQL As String = "SELECT COUNT(*) FROM Players " & oWhere.SQL

            Try
                log.Debug("Get number of subplayers. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return SafeInteger(oDB.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Fails to get Agents by AgentID.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return 0
        End Function

        Public Function GetSuperAdminID(ByVal psAgentID As String) As String
            Dim sSuperAdmin As String = ""
            Dim sKey As String = "GetSuperAdminID_" & psAgentID
            If System.Web.HttpContext.Current.Cache(sKey) Is Nothing Then
                Dim sSQL As String = _
                   "WITH AgentRecursive(ParentID, SuperAdminID) AS" & vbCrLf & _
                   "(" & vbCrLf & _
                   "   SELECT ParentID, SuperAdminID FROM Agents WHERE AgentID=" & SQLString(psAgentID) & vbCrLf & _
                   "   UNION ALL" & vbCrLf & _
                   "   SELECT parent.ParentID, parent.SuperAdminID FROM Agents as parent" & vbCrLf & _
                   "   INNER JOIN AgentRecursive ON AgentRecursive.ParentID=parent.AgentID" & vbCrLf & _
                   ")" & vbCrLf & _
                   "SELECT TOP 1 SuperAdminID FROM AgentRecursive WHERE ParentID IS NULL AND SuperAdminID IS NOT NULL"
                log.Debug("Get super admin id. SQL: " & vbCrLf & sSQL)

                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Try
                    sSuperAdmin = odbSQL.getScalerValue(sSQL)
                Catch ex As Exception
                    log.Error("Cannot get super admin id. SQL: " & vbCrLf & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
                System.Web.HttpContext.Current.Cache.Add(sKey, sSuperAdmin, Nothing, Date.Now.AddMinutes(1000), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(System.Web.HttpContext.Current.Cache(sKey), String)
        End Function

        Public Function GetSuperAgentID(ByVal psAgentID As String) As String
            Dim sAgentAdmin As String = ""

            Dim sSQL As String = _
               "WITH AgentRecursive(ParentID, AgentID) AS" & vbCrLf & _
               "(" & vbCrLf & _
               "   SELECT ParentID, AgentID FROM Agents WHERE AgentID=" & SQLString(psAgentID) & vbCrLf & _
               "   UNION ALL" & vbCrLf & _
               "   SELECT parent.ParentID, parent.AgentID FROM Agents as parent" & vbCrLf & _
               "   INNER JOIN AgentRecursive ON AgentRecursive.ParentID=parent.AgentID" & vbCrLf & _
               ")" & vbCrLf & _
               "SELECT TOP 1 AgentID FROM AgentRecursive WHERE ParentID IS NULL"
            log.Debug("Get Agent admin id. SQL: " & vbCrLf & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                sAgentAdmin = odbSQL.getScalerValue(sSQL)
            Catch ex As Exception
                log.Error("Cannot get Agent admin id. SQL: " & vbCrLf & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return sAgentAdmin
        End Function

        Public Function GetSuperAgent(ByVal psAgentID As String) As DataRow
            Dim sSQL As String = _
               "WITH AgentRecursive(ParentID, AgentID,ColorScheme,BackgroundImage) AS" & vbCrLf & _
               "(" & vbCrLf & _
               "   SELECT ParentID, AgentID,ColorScheme,BackgroundImage FROM Agents WHERE AgentID=" & SQLString(psAgentID) & vbCrLf & _
               "   UNION ALL" & vbCrLf & _
               "   SELECT parent.ParentID, parent.AgentID,parent.ColorScheme,parent.BackgroundImage FROM Agents as parent" & vbCrLf & _
               "   INNER JOIN AgentRecursive ON AgentRecursive.ParentID=parent.AgentID" & vbCrLf & _
               ")" & vbCrLf & _
               "SELECT TOP 1 AgentID,ColorScheme,BackgroundImage FROM AgentRecursive WHERE ParentID IS NULL"
            log.Debug("Get Agent admin id. SQL: " & vbCrLf & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim odtAgentSuper As DataTable = odbSQL.getDataTable(sSQL)
                If odtAgentSuper IsNot Nothing And odtAgentSuper.Rows.Count > 0 Then
                    Return odtAgentSuper.Rows(0)
                End If
            Catch ex As Exception
                log.Error("Cannot get Agent admin . SQL: " & vbCrLf & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function


        Public Function GetBettingLocked(ByVal psAgentID As String) As Boolean
            Dim bBettingLocked As Boolean = True
            Dim sSQL As String = " WITH RecursiveBettingLocked([Level], AgentID, ParentID, BettingLocked) AS " & _
                                " ( " & _
                                    " SELECT 1 as [Level], AgentID, ParentID, IsBettingLocked " & _
                                    " FROM Agents where AgentID={0} " & _
                                    " UNION ALL " & _
                                    " SELECT RecursiveBettingLocked.[Level]+1 as [Level], child.AgentID, child.ParentID, child.IsBettingLocked " & _
                                    " FROM Agents as child " & _
                                    " INNER JOIN RecursiveBettingLocked on RecursiveBettingLocked.ParentID = child.AgentID " & _
                                    " WHERE ISNULL(RecursiveBettingLocked.BettingLocked,'') <> 'Y' " & _
                                " ) " & _
                                " SELECT TOP 1 BettingLocked FROM RecursiveBettingLocked ORDER BY [Level] DESC "

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                sSQL = String.Format(sSQL, SQLString(psAgentID))
                log.Debug("Get BettingLocked. SQL: " & vbCrLf & sSQL)

                bBettingLocked = SafeString(odbSQL.getScalerValue(sSQL)) = "Y"
            Catch ex As Exception
                log.Error("Cannot GetBettingLocked. SQL: " & vbCrLf & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bBettingLocked
        End Function

        Public Function GetCasinoLocked(ByVal psAgentID As String) As Boolean
            Dim bCasinoLocked As Boolean = True
            Dim sSQL As String = " WITH CasinoBettingLocked([Level], AgentID, ParentID, HasCasino) AS " & _
                                " ( " & _
                                    " SELECT 1 as [Level], AgentID, ParentID, HasCasino " & _
                                    " FROM Agents where AgentID={0} " & _
                                    " UNION ALL " & _
                                    " SELECT CasinoBettingLocked.[Level]+1 as [Level], child.AgentID, child.ParentID, child.HasCasino " & _
                                    " FROM Agents as child " & _
                                    " INNER JOIN CasinoBettingLocked on CasinoBettingLocked.ParentID = child.AgentID " & _
                                    " WHERE ISNULL(CasinoBettingLocked.HasCasino,'') = 'Y' " & _
                                " ) " & _
                                " SELECT TOP 1 HasCasino FROM CasinoBettingLocked ORDER BY [Level] DESC "

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                sSQL = String.Format(sSQL, SQLString(psAgentID))
                log.Debug("Get CasinoLocked. SQL: " & vbCrLf & sSQL)

                bCasinoLocked = SafeString(odbSQL.getScalerValue(sSQL)) <> "Y"
            Catch ex As Exception
                log.Error("Cannot get CasinoLocked. SQL: " & vbCrLf & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bCasinoLocked
        End Function

        Public Function GetLocked(ByVal psAgentID As String) As Boolean
            Dim bLocked As Boolean = True
            Dim sSQL As String = "WITH RecursiveLocked([Level], AgentID, ParentID, Locked) AS " & _
                                " ( " & _
                                    " SELECT 1 as [Level], AgentID, ParentID, CASE WHEN Agents.ParentID is NULL " & _
                                                                            " THEN (SELECT SuperAdmins.IsLocked FROM SuperAdmins " & _
                                                                                " WHERE SuperAdmins.SuperAdminID = Agents.SuperAdminID) " & _
                                                                            " ELSE Agents.IsLocked END AS IsLocked " & _
                                    " FROM Agents " & _
                                    " WHERE AgentID={0} " & _
                                    " UNION ALL " & _
                                    " SELECT RecursiveLocked.[Level]+1 as [Level], child.AgentID, child.ParentID, " & _
                                        " CASE WHEN child.ParentID is NULL " & _
                                        " THEN (SELECT SuperAdmins.IsLocked FROM SuperAdmins " & _
                                        " WHERE SuperAdmins.SuperAdminID = child.SuperAdminID) " & _
                                        " ELSE child.IsLocked END AS IsLocked " & _
                                    " FROM Agents as child " & _
                                    " INNER JOIN RecursiveLocked on RecursiveLocked.ParentID = child.AgentID " & _
                                    " WHERE ISNULL(RecursiveLocked.Locked,'') <> 'Y' " & _
                                " ) " & _
                                " SELECT TOP 1 Locked FROM RecursiveLocked ORDER BY [Level] DESC "

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                sSQL = String.Format(sSQL, SQLString(psAgentID))
                log.Debug("Get Locked. SQL: " & vbCrLf & sSQL)

                bLocked = SafeString(odbSQL.getScalerValue(sSQL)) = "Y"
            Catch ex As Exception
                log.Error("Cannot GetLocked. SQL: " & vbCrLf & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return bLocked
        End Function

        Public Shared Function CheckDuplicateLogin(ByVal psLogin As String, ByVal psAgentID As String, ByVal psSiteType As String) As Boolean
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Login = " & SQLString(LCase(psLogin)))
            oWhere.AppendANDCondition("SiteType = " & SQLString(psSiteType))
            oWhere.AppendOptionalANDCondition("AgentID ", SQLString(psAgentID), "<>")

            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = "select top 1 * from Agents " & oWhere.SQL
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                Return oDT.Rows.Count = 0
            Catch ex As Exception
                Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CAgentManager))
                log.Error("Can't CheckDuplicateLogin: " & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try
            Return False
        End Function

        Public Function GetAllAgentsBySuperAdminID(ByVal psSuperAdminID As String, _
                                                   ByVal bpIsBettingLocked As Boolean?, _
                                                   Optional ByVal pbIsLocked As Boolean = False, _
                                                   Optional ByVal psNameOrLogin As String = "", _
                                                   Optional ByVal pbIsSuperAmdin As Boolean = True) As DataTable

            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("ISNULL(IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(Name LIKE {0} OR Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            Dim oWhere1 As New CSQLWhereStringBuilder()
            If pbIsSuperAmdin Then
                oWhere1.AppendANDCondition("SuperAdminID=" & SQLString(psSuperAdminID))
            Else
                oWhere1.AppendANDCondition("AgentID=" & SQLString(psSuperAdminID))
            End If

            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS " & vbCrLf & _
                                    "( " & vbCrLf & _
                                         " SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents" & vbCrLf & _
                                         oWhere1.SQL() & vbCrLf & _
                                         " UNION ALL " & vbCrLf & _
                                         " SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                         " FROM Agents as child " & vbCrLf & _
                                         " INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID " & vbCrLf & _
                                    ") " & vbCrLf & _
                                    "SELECT ar.*, (a.Login+ ' (' + a.Name + ')') as AgentName, a.* " & vbCrLf & _
                                    "FROM AgentRecursive ar " & vbCrLf & _
                                    "INNER JOIN Agents a ON a.AgentID=ar.AgentID " & vbCrLf & _
                                    oWhere.SQL & vbCrLf & _
                                    "ORDER BY ar.AgentLevel, a.Name "

            log.Debug("Get the list of all agents by SuperID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of all agents . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

        Public Function GetAllAgentsByAgent(ByVal psAgentID As String, ByVal bpIsBettingLocked As Boolean?, Optional ByVal pbIsLocked As Boolean = False _
                                           , Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("ISNULL(a.IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(a.IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(a.Name LIKE {0} OR a.Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            'not get parent agent
            oWhere.AppendANDCondition("a.AgentID<>" & SQLString(psAgentID))
            ' get parent agent
            'oWhere.AppendANDCondition("a.AgentID=" & SQLString(psAgentID))
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS " & vbCrLf & _
                                    "( " & vbCrLf & _
                                         " SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents where AgentID=" & SQLString(psAgentID) & vbCrLf & _
                                                " UNION ALL " & vbCrLf & _
                                         " SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                         " FROM Agents as child " & vbCrLf & _
                                         " INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID " & vbCrLf & _
                                    ") " & vbCrLf & _
                                    "SELECT ar.*, (a.Login + ' (' + a.Name + ')') as AgentName, a.* " & vbCrLf & _
                                    "FROM AgentRecursive ar " & vbCrLf & _
                                    "INNER JOIN Agents a ON a.AgentID=ar.AgentID " & vbCrLf & _
                                    oWhere.SQL & vbCrLf & _
                                    "ORDER BY ar.AgentLevel, a.Name "

            log.Debug("Get the list of all agents by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of all agents  by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

        Public Function GetAllAgentSubAgentsByAgent(ByVal psAgentID As String, ByVal bpIsBettingLocked As Boolean?, Optional ByVal pbIsLocked As Boolean = False _
                                           , Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("ISNULL(a.IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(a.IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(a.Name LIKE {0} OR a.Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If

            'not get parent agent
            'oWhere.AppendANDCondition("a.AgentID<>" & SQLString(psAgentID))
            ' get parent agent
            oWhere.AppendANDCondition(String.Format("a.AgentID={0} or a.AgentID<>{0}", SQLString(psAgentID)))
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS " & vbCrLf & _
                                    "( " & vbCrLf & _
                                         " SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents where AgentID=" & SQLString(psAgentID) & vbCrLf & _
                                                " UNION ALL " & vbCrLf & _
                                         " SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                         " FROM Agents as child " & vbCrLf & _
                                         " INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID " & vbCrLf & _
                                    ") " & vbCrLf & _
                                    "SELECT ar.*, (a.Login + ' (' + a.Name + ')') as AgentName, a.* " & vbCrLf & _
                                    "FROM AgentRecursive ar " & vbCrLf & _
                                    "INNER JOIN Agents a ON a.AgentID=ar.AgentID " & vbCrLf & _
                                    oWhere.SQL & vbCrLf & _
                                    "ORDER BY ar.AgentLevel, a.Name "

            log.Debug("Get the list of all agents by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of all agents  by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

        Public Function GetAllAgentsAndSubAgentByAgent(ByVal psAgentID As String, ByVal bpIsBettingLocked As Boolean?, Optional ByVal pbIsLocked As Boolean = False _
                                           , Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()

            oWhere.AppendANDCondition("ISNULL(a.IsLocked, 'N')=" & SQLString(IIf(pbIsLocked, "Y", "N")))
            If bpIsBettingLocked IsNot Nothing Then
                oWhere.AppendANDCondition("ISNULL(a.IsBettingLocked, 'N')=" & SQLString(IIf(SafeBoolean(bpIsBettingLocked), "Y", "N")))
            End If

            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(a.Name LIKE {0} OR a.Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If
            oWhere.AppendANDCondition("a.AgentID=" & SQLString(psAgentID))

            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS " & vbCrLf & _
                                    "( " & vbCrLf & _
                                         " SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents where AgentID=" & SQLString(psAgentID) & vbCrLf & _
                                                " UNION ALL " & vbCrLf & _
                                         " SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                         " FROM Agents as child " & vbCrLf & _
                                         " INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID " & vbCrLf & _
                                    ") " & vbCrLf & _
                                    "SELECT ar.*, (a.Login + ' (' + a.Name + ')') as AgentName, a.* " & vbCrLf & _
                                    "FROM AgentRecursive ar " & vbCrLf & _
                                    "INNER JOIN Agents a ON a.AgentID=ar.AgentID " & vbCrLf & _
                                    oWhere.SQL & vbCrLf & _
                                    "ORDER BY ar.AgentLevel, a.Name "

            log.Debug("Get the list of all GetAllAgentsAndSubAgentByAgent by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of GetAllAgentsAndSubAgentByAgent  by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

        Public Function GetAllAgentsByAgentID(ByVal psAgentID As String, Optional ByVal psNameOrLogin As String = "") As DataTable
            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("a.AgentID<>" & SQLString(psAgentID))
            If psNameOrLogin <> "" Then
                oWhere.AppendANDCondition(String.Format("(a.Name LIKE {0} OR a.Login LIKE {0})", SQLString("%" & psNameOrLogin & "%")))
            End If
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, AgentLevel) AS " & vbCrLf & _
                                    "( " & vbCrLf & _
                                         " SELECT AgentID, ParentID, 1 AS AgentLevel FROM Agents where AgentID=" & SQLString(psAgentID) & vbCrLf & _
                                                " UNION ALL " & vbCrLf & _
                                         " SELECT child.AgentID, child.ParentID, AgentRecursive.AgentLevel+1 AS AgentLevel" & vbCrLf & _
                                         " FROM Agents as child " & vbCrLf & _
                                         " INNER JOIN AgentRecursive ON  child.ParentID = AgentRecursive.AgentID " & vbCrLf & _
                                    ") " & vbCrLf & _
                                    "SELECT ar.*, (a.Login + ' (' + a.Name + ')') as AgentName, a.* " & vbCrLf & _
                                    "FROM AgentRecursive ar " & vbCrLf & _
                                    "INNER JOIN Agents a ON a.AgentID=ar.AgentID " & vbCrLf & _
                                    oWhere.SQL & vbCrLf & _
                                    "ORDER BY ar.AgentLevel, a.Name "
            log.Debug("Get the list of all agents by agentID. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of all agents  by agentID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

        Public Function GetAllSuperAgents(ByVal pSuperAdminID As String) As DataTable
            Dim odtAgents As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SuperAdminID=" & SQLString(pSuperAdminID))
            oWhere.AppendANDCondition("ParentID is null")
            Dim sSQL As String = "SELECT *, (Login+ ' (' + Name + ')') as AgentName FROM Agents" & oWhere.SQL & " ORDER BY Login"
            log.Debug("Get the list of Super agents by SuperAdminID. SQL: " & sSQL)
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                odtAgents = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of all agents  by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtAgents
        End Function

#Region "Agent's SubPlayers dashboard"
        'Public Function GetAllPlayersDashboard2(ByVal psAgentID As String, Optional ByVal poStartDate As Date = Nothing, Optional ByVal pbByTicketNetAmount As Boolean = False) As DataTable
        '    ' Create Standard table
        '    Dim oDay As String() = {"Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Gross", "Net", "PL", "Pending"}
        '    Dim oTable As New DataTable
        '    oTable.Columns.Add("Agent", GetType(String))
        '    oTable.Columns.Add("AgentID", GetType(String))
        '    oTable.Columns.Add("Player", GetType(String))
        '    oTable.Columns.Add("PlayerID", GetType(String))
        '    oTable.Columns.Add("CreditMaxAmount", GetType(String))
        '    oTable.Columns.Add("BalanceAmount", GetType(String))
        '    For Each sDay In oDay
        '        oTable.Columns.Add(sDay, GetType(Double))
        '    Next
        '    Dim dtSubAgent As DataTable = GetAllAgentsByAgent(psAgentID, False)
        '    Dim lstAgentID As New List(Of String)
        '    lstAgentID.Add(psAgentID)
        '    If dtSubAgent IsNot Nothing AndAlso dtSubAgent.Rows.Count > 0 Then
        '        For Each dr As DataRow In dtSubAgent.Rows
        '            lstAgentID.Add(SafeString(dr("AgentID")))
        '        Next

        '    End If

        '    For Each strAgentID As String In lstAgentID
        '        ' Get SubPlayersBet
        '        Dim oMainTable As DataTable = getSubPayersBet(strAgentID, poStartDate)
        '        If oMainTable IsNot Nothing Then
        '            ' Get SubPlayer's Name
        '            Dim oSubPlayers As DataTable = GetSubPlayers(strAgentID)
        '            If oSubPlayers IsNot Nothing Then
        '                ' Get Sum of Amounts
        '                Dim oRow As DataRow
        '                Dim nTotalGross, nTotalNet, nTotalPL, nPercentage As Double
        '                Dim oMondayOfWeek As Date
        '                If poStartDate = Nothing Then
        '                    oMondayOfWeek = GetMondayOfCurrentWeek()
        '                Else
        '                    oMondayOfWeek = poStartDate
        '                End If

        '                '' Get previous Monday 5:30AM by Eastern timezone
        '                Dim oSEasternMon As Date = SafeDate(oMondayOfWeek.ToString("MM/dd/yyyy") & " 06:00:00")

        '                'If oSubPlayers IsNot Nothing AndAlso oSubPlayers.Rows.Count = 0 Then
        '                '    oRow = oTable.NewRow
        '                '    oRow("Agent") = SafeString(GetAgentByAgentID(strAgentID)("Login"))
        '                '    'oTable.Rows.Add(oRow)
        '                '    'oRow = oTable.NewRow
        '                '    oRow("Player") = ""
        '                '    For Each sDay In oDay
        '                '        If Not oTable.Columns.Contains(sDay) Then
        '                '            oTable.Columns.Add(sDay, GetType(Double))

        '                '        End If
        '                '        oRow(sDay) = "0"
        '                '    Next
        '                '    oTable.Rows.Add(oRow)
        '                '    Continue For
        '                'End If

        '                For Each oDRSubPlayer As DataRow In oSubPlayers.Select("", "Login")
        '                    oRow = oTable.NewRow
        '                    oRow("Agent") = SafeString(oDRSubPlayer("AgentLogin"))
        '                    oRow("AgentID") = strAgentID
        '                    oRow("Player") = String.Format("{0}({1})", SafeString(oDRSubPlayer("Login")), SafeString(oDRSubPlayer("Name")))
        '                    oRow("PlayerID") = SafeString(oDRSubPlayer("PlayerID"))
        '                    oRow("CreditMaxAmount") = SafeString(oDRSubPlayer("CreditMaxAmount"))
        '                    oRow("BalanceAmount") = SafeString(oDRSubPlayer("BalanceAmount"))
        '                    Dim bActivePlayer As Boolean = False
        '                    nTotalGross = 0
        '                    nTotalNet = 0
        '                    nTotalPL = 0
        '                    For Each sDay As String In oDay
        '                        If sDay = "Net" OrElse sDay = "Gross" OrElse sDay = "PL" Then
        '                            Continue For
        '                        End If

        '                        Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND PLayerID={0} AND TicketCompletedDate = {1}"
        '                        Select Case sDay
        '                            Case "Mon"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek, "MM/dd/yyyy")))
        '                            Case "Tues"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(1), "MM/dd/yyyy")))
        '                            Case "Wed"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(2), "MM/dd/yyyy")))
        '                            Case "Thurs"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(3), "MM/dd/yyyy")))
        '                            Case "Fri"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(4), "MM/dd/yyyy")))
        '                            Case "Sat"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(5), "MM/dd/yyyy")))
        '                            Case "Sun"
        '                                sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
        '                            Case "Pending"
        '                                sWhere = String.Format("ISNULL(TicketStatus,'Open') IN ('Open', 'Pending') AND " & _
        '                                                       "PLayerID={0} AND TransactionDate <={1}", _
        '                                                       SQLString(oDRSubPlayer("PlayerID")), _
        '                                                       SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
        '                        End Select

        '                        log.Debug(sWhere)
        '                        If sDay <> "Pending" Then
        '                            If pbByTicketNetAmount Then
        '                                oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(TicketNetAmount)", sWhere))
        '                                nTotalGross += SafeDouble(oRow(sDay))
        '                            Else
        '                                oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(NetAmount)", sWhere))
        '                                nTotalGross += SafeDouble(oRow(sDay))
        '                            End If

        '                        Else
        '                            oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(PendingAmount)", sWhere))
        '                        End If
        '                        bActivePlayer = SafeDouble(oRow(sDay)) > 0
        '                    Next

        '                    oRow("Gross") = nTotalGross

        '                    If poStartDate = Nothing Then '' Dashboard for current week
        '                        nTotalNet = nTotalGross

        '                        '' Get Percentage
        '                        '' Gross Percentage
        '                        nPercentage = SafeDouble(oDRSubPlayer("GrossPercentage"))
        '                        If nPercentage <> 0 AndAlso nTotalNet < 0 Then
        '                            nTotalNet = nTotalNet * nPercentage / 100
        '                        End If

        '                        nTotalPL = nTotalNet
        '                        If SafeDouble(oDRSubPlayer("ProfitPercentage")) > 0 Then
        '                            nTotalPL = nTotalPL * SafeDouble(oDRSubPlayer("ProfitPercentage")) / 100
        '                        End If

        '                    Else '' Dashboard for previous week
        '                        Dim oLstPlayerIDs As New List(Of String)
        '                        oLstPlayerIDs.Add(SafeString(oDRSubPlayer("PlayerID")))

        '                        nTotalNet = GetTransactionAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
        '                        nTotalPL = GetTransactionPLAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
        '                    End If

        '                    oRow("Net") = nTotalNet
        '                    oRow("PL") = nTotalPL
        '                    'If nTotalGross > 0 OrElse nTotalNet > 0 OrElse nTotalPL > 0 AndAlso bActivePlayer Then
        '                    oTable.Rows.Add(oRow)
        '                    ' End If

        '                Next

        '                ' Calc Totals Row
        '                'oRow = oTable.NewRow
        '                'oRow("Player") = "Totals"
        '                'For Each sDay As String In oDay
        '                '    oRow(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), ""))
        '                'Next
        '                'oTable.Rows.Add(oRow)

        '            End If
        '        End If
        '    Next
        '    Dim oRowAll = oTable.NewRow
        '    oRowAll("Player") = "Totals"
        '    For Each sDay As String In oDay
        '        oRowAll(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), ""))
        '    Next
        '    oTable.Rows.Add(oRowAll)
        '    Return oTable
        'End Function
        Public Function GetAllPlayersDashboard(ByVal psAgentID As String, Optional ByVal poStartDate As Date = Nothing, Optional ByVal pbByTicketNetAmount As Boolean = False) As DataTable
            ' Create Standard table
            Dim oDay As String() = {"Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Gross", "Net", "PL", "Pending"}
            Dim oTable As New DataTable
            oTable.Columns.Add("Player", GetType(String))
            oTable.Columns.Add("PlayerID", GetType(String))
            oTable.Columns.Add("CreditMaxAmount", GetType(String))
            oTable.Columns.Add("BalanceAmount", GetType(String))
            oTable.Columns.Add("Agent", GetType(String))
            oTable.Columns.Add("AgentID", GetType(String))
            For Each sDay In oDay
                oTable.Columns.Add(sDay, GetType(Double))
            Next
            Dim dtSubAgent As DataTable = GetAllAgentsByAgent(psAgentID, False)
            Dim lstAgentID As New List(Of String)
            lstAgentID.Add(psAgentID)
            If dtSubAgent IsNot Nothing AndAlso dtSubAgent.Rows.Count > 0 Then
                For Each dr As DataRow In dtSubAgent.Rows
                    lstAgentID.Add(SafeString(dr("AgentID")))
                Next

            End If

            For Each strAgentID As String In lstAgentID


                ' Get SubPlayersBet
                Dim oMainTable As DataTable = getSubPayersBet(strAgentID, poStartDate)
                If oMainTable IsNot Nothing Then
                    ' Get SubPlayer's Name
                    Dim oSubPlayers As DataTable = GetSubPlayers(strAgentID)
                    If oSubPlayers IsNot Nothing Then
                        ' Get Sum of Amounts
                        Dim oRow As DataRow
                        Dim nTotalGross, nTotalNet, nTotalPL, nPercentage As Double
                        Dim oMondayOfWeek As Date
                        If poStartDate = Nothing Then
                            oMondayOfWeek = GetMondayOfCurrentWeek()
                        Else
                            oMondayOfWeek = poStartDate
                        End If

                        '' Get previous Monday 5:30AM by Eastern timezone
                        Dim oSEasternMon As Date = SafeDate(oMondayOfWeek.ToString("MM/dd/yyyy") & " 06:00:00")

                        For Each oDRSubPlayer As DataRow In oSubPlayers.Select("", "Login")
                            oRow = oTable.NewRow
                            oRow("Player") = String.Format("{0}({1})", SafeString(oDRSubPlayer("Login")), SafeString(oDRSubPlayer("Name")))
                            oRow("PlayerID") = SafeString(oDRSubPlayer("PlayerID"))
                            oRow("CreditMaxAmount") = SafeString(oDRSubPlayer("CreditMaxAmount"))
                            oRow("BalanceAmount") = SafeString(oDRSubPlayer("BalanceAmount"))
                            oRow("Agent") = SafeString(oDRSubPlayer("AgentLogin"))
                            oRow("AgentID") = strAgentID
                            nTotalGross = 0
                            nTotalNet = 0
                            nTotalPL = 0
                            For Each sDay As String In oDay
                                If sDay = "Net" OrElse sDay = "Gross" OrElse sDay = "PL" Then
                                    Continue For
                                End If

                                Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND PLayerID={0} AND TicketCompletedDate = {1}"
                                Select Case sDay
                                    Case "Mon"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek, "MM/dd/yyyy")))
                                    Case "Tues"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(1), "MM/dd/yyyy")))
                                    Case "Wed"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(2), "MM/dd/yyyy")))
                                    Case "Thurs"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(3), "MM/dd/yyyy")))
                                    Case "Fri"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(4), "MM/dd/yyyy")))
                                    Case "Sat"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(5), "MM/dd/yyyy")))
                                    Case "Sun"
                                        sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                                    Case "Pending"
                                        sWhere = String.Format("ISNULL(TicketStatus,'Open') IN ('Open', 'Pending') AND " & _
                                                               "PLayerID={0} AND TransactionDate <={1}", _
                                                               SQLString(oDRSubPlayer("PlayerID")), _
                                                               SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                                End Select

                                log.Debug(sWhere)
                                If sDay <> "Pending" Then
                                    If pbByTicketNetAmount Then
                                        oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(TicketNetAmount)", sWhere))
                                        nTotalGross += SafeDouble(oRow(sDay))
                                    Else
                                        oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(NetAmount)", sWhere))
                                        nTotalGross += SafeDouble(oRow(sDay))
                                    End If

                                Else
                                    oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(PendingAmount)", sWhere))
                                End If
                            Next

                            oRow("Gross") = nTotalGross

                            If poStartDate = Nothing Then '' Dashboard for current week
                                nTotalNet = nTotalGross

                                '' Get Percentage
                                '' Gross Percentage
                                nPercentage = SafeDouble(oDRSubPlayer("GrossPercentage"))
                                If nPercentage <> 0 AndAlso nTotalNet < 0 Then
                                    nTotalNet = nTotalNet * nPercentage / 100
                                End If

                                nTotalPL = nTotalNet
                                If SafeDouble(oDRSubPlayer("ProfitPercentage")) > 0 Then
                                    nTotalPL = nTotalPL * SafeDouble(oDRSubPlayer("ProfitPercentage")) / 100
                                End If

                            Else '' Dashboard for previous week
                                Dim oLstPlayerIDs As New List(Of String)
                                oLstPlayerIDs.Add(SafeString(oDRSubPlayer("PlayerID")))

                                nTotalNet = GetTransactionAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
                                nTotalPL = GetTransactionPLAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
                            End If

                            oRow("Net") = nTotalNet
                            oRow("PL") = nTotalPL
                            If SafeDouble(oRow("Mon")) = 0 AndAlso SafeDouble(oRow("Tues")) = 0 AndAlso SafeDouble(oRow("Wed")) = 0 AndAlso SafeDouble(oRow("Thurs")) = 0 AndAlso SafeDouble(oRow("Fri")) = 0 AndAlso SafeDouble(oRow("Sat")) = 0 AndAlso SafeDouble(oRow("Sun")) = 0 AndAlso SafeDouble(oRow("Gross")) = 0 AndAlso SafeDouble(oRow("Net")) = 0 AndAlso SafeDouble(oRow("PL")) = 0 AndAlso SafeDouble(oRow("Pending")) = 0 Then
                            Else
                                oTable.Rows.Add(oRow)
                            End If

                        Next

                        ' Calc Totals Row
                        oRow = oTable.NewRow
                        oRow("Player") = "Totals"
                        For Each sDay As String In oDay
                            oRow(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), "AgentID='" & strAgentID & "'"))
                        Next
                        oTable.Rows.Add(oRow)

                    End If
                End If
            Next
            If oTable.Rows.Count > 0 Then
                Dim odr As DataRow = oTable.NewRow
                odr("Agent") = "All"
                odr("Player") = "Totals"
                For Each sDay As String In oDay
                    odr(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), ""))
                Next
                oTable.Rows.Add(odr)
            End If
            Return oTable
        End Function

        Public Function GetPlayersDashboard(ByVal psAgentID As String, Optional ByVal poStartDate As Date = Nothing, Optional ByVal pbByTicketNetAmount As Boolean = False) As DataTable
            ' Create Standard table
            Dim oDay As String() = {"Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Gross", "Net", "PL", "Pending"}
            Dim oTable As New DataTable
            oTable.Columns.Add("Player", GetType(String))
            oTable.Columns.Add("PlayerID", GetType(String))
            oTable.Columns.Add("CreditMaxAmount", GetType(String))
            oTable.Columns.Add("BalanceAmount", GetType(String))
            oTable.Columns.Add("Agent", GetType(String))
            For Each sDay In oDay
                oTable.Columns.Add(sDay, GetType(Double))
            Next

            ' Get SubPlayersBet
            Dim oMainTable As DataTable = getSubPayersBet(psAgentID, poStartDate)
            If oMainTable IsNot Nothing Then
                ' Get SubPlayer's Name
                Dim oSubPlayers As DataTable = GetSubPlayers(psAgentID)
                If oSubPlayers IsNot Nothing Then
                    ' Get Sum of Amounts
                    Dim oRow As DataRow
                    Dim nTotalGross, nTotalNet, nTotalPL, nPercentage As Double
                    Dim oMondayOfWeek As Date
                    If poStartDate = Nothing Then
                        oMondayOfWeek = GetMondayOfCurrentWeek()
                    Else
                        oMondayOfWeek = poStartDate
                    End If

                    '' Get previous Monday 5:30AM by Eastern timezone
                    Dim oSEasternMon As Date = SafeDate(oMondayOfWeek.ToString("MM/dd/yyyy") & " 06:00:00")

                    For Each oDRSubPlayer As DataRow In oSubPlayers.Select("", "Login")
                        oRow = oTable.NewRow
                        oRow("Player") = String.Format("{0}({1})", SafeString(oDRSubPlayer("Login")), SafeString(oDRSubPlayer("Name")))
                        oRow("PlayerID") = SafeString(oDRSubPlayer("PlayerID"))
                        oRow("CreditMaxAmount") = SafeString(oDRSubPlayer("CreditMaxAmount"))
                        oRow("BalanceAmount") = SafeString(oDRSubPlayer("BalanceAmount"))

                        nTotalGross = 0
                        nTotalNet = 0
                        nTotalPL = 0
                        For Each sDay As String In oDay
                            If sDay = "Net" OrElse sDay = "Gross" OrElse sDay = "PL" Then
                                Continue For
                            End If

                            Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND PLayerID={0} AND TicketCompletedDate = {1}"
                            Select Case sDay
                                Case "Mon"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek, "MM/dd/yyyy")))
                                Case "Tues"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(1), "MM/dd/yyyy")))
                                Case "Wed"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(2), "MM/dd/yyyy")))
                                Case "Thurs"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(3), "MM/dd/yyyy")))
                                Case "Fri"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(4), "MM/dd/yyyy")))
                                Case "Sat"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(5), "MM/dd/yyyy")))
                                Case "Sun"
                                    sWhere = String.Format(sWhere, SQLString(oDRSubPlayer("PlayerID")), SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                                Case "Pending"
                                    sWhere = String.Format("ISNULL(TicketStatus,'Open') IN ('Open', 'Pending') AND " & _
                                                           "PLayerID={0} AND TransactionDate <={1}", _
                                                           SQLString(oDRSubPlayer("PlayerID")), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                            End Select

                            log.Debug(sWhere)
                            If sDay <> "Pending" Then
                                If pbByTicketNetAmount Then
                                    oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(TicketNetAmount)", sWhere))
                                    nTotalGross += SafeDouble(oRow(sDay))
                                Else
                                    oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(NetAmount)", sWhere))
                                    nTotalGross += SafeDouble(oRow(sDay))
                                End If

                            Else
                                LogDebug(log, "1234" & oMainTable.Rows.Count)
                                oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(PendingAmount)", sWhere))
                            End If
                        Next

                        oRow("Gross") = nTotalGross

                        If poStartDate = Nothing Then '' Dashboard for current week
                            nTotalNet = nTotalGross

                            '' Get Percentage
                            '' Gross Percentage
                            nPercentage = SafeDouble(oDRSubPlayer("GrossPercentage"))
                            If nPercentage <> 0 AndAlso nTotalNet < 0 Then
                                nTotalNet = nTotalNet * nPercentage / 100
                            End If

                            nTotalPL = nTotalNet
                            If SafeDouble(oDRSubPlayer("ProfitPercentage")) > 0 Then
                                nTotalPL = nTotalPL * SafeDouble(oDRSubPlayer("ProfitPercentage")) / 100
                            End If

                        Else '' Dashboard for previous week
                            Dim oLstPlayerIDs As New List(Of String)
                            oLstPlayerIDs.Add(SafeString(oDRSubPlayer("PlayerID")))

                            nTotalNet = GetTransactionAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
                            nTotalPL = GetTransactionPLAmount(oLstPlayerIDs, oSEasternMon, oSEasternMon.AddDays(7))
                        End If

                        oRow("Net") = nTotalNet
                        oRow("PL") = nTotalPL

                        oTable.Rows.Add(oRow)
                    Next

                    ' Calc Totals Row
                    oRow = oTable.NewRow
                    oRow("Agent") = "All"
                    oRow("Player") = "Totals " & oTable.Select("Player<>'Totals'").Count
                    For Each sDay As String In oDay
                        oRow(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), "True"))
                    Next
                    oTable.Rows.Add(oRow)

                End If
            End If

            Return oTable
        End Function

        Private Function getSubPayersBet(ByVal psAgentID As String, Optional ByVal poStartDate As Date = Nothing) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("t.AgentID = " & SQLString(psAgentID))

            If poStartDate = Nothing Then
                oWhere.AppendANDCondition(String.Format("t.TicketCompletedDate >= {0} OR ISNULL(t.TicketStatus, 'Open') IN ('Open','Pending'))", _
                                                        SQLString(Format(GetMondayOfCurrentWeek, "yyyy-MM-dd"))))
            Else
                oWhere.AppendANDCondition("( " & getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poStartDate.AddDays(6))) & _
                                          " OR ISNULL(t.TicketStatus, 'Open') IN ('Open','Pending'))")
            End If

            Dim sSQL As String = "SELECT Convert(varchar(40),t.PlayerID) AS PlayerID, (t.NetAmount- t.RiskAmount) as NetAmount, t.RiskAmount AS PendingAmount,t.NetAmount as TicketNetAmount,  " & _
            " CAST(CONVERT(NVARCHAR(10), t.TicketCompletedDate, 101) AS SMALLDATETIME) AS TicketCompletedDate, " & _
            " CAST(CONVERT(NVARCHAR(10), t.TransactionDate, 101) AS SMALLDATETIME) AS TransactionDate, t.TicketStatus " & vbCrLf & _
            " FROM Tickets t " & vbCrLf & oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.PlayerID, t.NetAmount, t.RiskAmount, t.TransactionDate, t.TicketCompletedDate, t.TicketStatus "

            Try
                log.Debug("Get main datatable1234 for getSubPayersBet. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get PlayersDashboard.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetSubPlayers(ByVal psAgentID As String) As DataTable
            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("p.AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition("ISNULL(p.IsLocked,'') <> " & SQLString("Y"))

            Dim sSQL As String = "SELECT a.[Login] as AgentLogin,p.PlayerID, p.[Name], p.[Login], a.ProfitPercentage, a.GrossPercentage,pl.CreditMaxAmount ,p.BalanceAmount  " & vbCrLf & _
            "FROM Players p INNER JOIN Agents a ON p.AgentID = a.AgentID  left join PlayerTemplates pl on pl.PlayerTemplateID= p.PlayerTemplateID " & _
            oWhere.SQL & " ORDER BY  p.[Login], p.[Name], p.IsLocked "

            Try
                log.Debug("Get subplayer's info of Agent. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Fails to get SubPlayers for PlayersDashboard.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Private Function GetTransactionAmount(ByVal polstUserIDs As List(Of String), ByVal poSDate As Date, ByVal poEDate As Date, Optional ByVal pbPlayer As Boolean = True) As Double

            If polstUserIDs Is Nothing OrElse polstUserIDs.Count = 0 Then
                '' Empty PlayerID will throw exception
                Return 0
            End If

            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            If pbPlayer Then '' Get Transaction Amount from List of PlayerIDs
                oWhere.AppendANDCondition(String.Format("WithdrawID IN ('{0}')", Join(polstUserIDs.ToArray(), "','")))

            Else '' Get Transaction Amount From List of AgentIDs
                oWhere.AppendANDCondition(String.Format("WithdrawID IN (SELECT PlayerID FROM Players WHERE AgentID IN('{0}'))", Join(polstUserIDs.ToArray(), "','")))
            End If

            oWhere.AppendANDCondition(getSQLDateRange("TransactionDate", SafeString(poSDate), SafeString(poEDate), True))

            Dim sSQL As String = "SELECT SUM(TransactionAmount) as PaidAmount FROM Transactions " & vbCrLf & oWhere.SQL

            Try
                log.Debug("Get TransactionAmount from list of PlayerIDs. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Return SafeDouble(oDB.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Fails to Get TransactionAmount from list of PlayerIDs. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return 0
        End Function

        Private Function GetTransactionPLAmount(ByVal polstUserIDs As List(Of String), ByVal poSDate As Date, ByVal poEDate As Date, Optional ByVal pbPlayer As Boolean = True) As Double

            If polstUserIDs Is Nothing OrElse polstUserIDs.Count = 0 Then
                '' Empty PlayerID will throw exception
                Return 0
            End If

            Dim oDB As CSQLDBUtils = Nothing
            Dim oWhere As New CSQLWhereStringBuilder()
            If pbPlayer Then '' Get Transaction Amount from List of PlayerIDs
                oWhere.AppendANDCondition(String.Format("WithdrawID IN ('{0}')", Join(polstUserIDs.ToArray(), "','")))

            Else '' Get Transaction Amount From List of AgentIDs
                oWhere.AppendANDCondition(String.Format("WithdrawID IN (SELECT PlayerID FROM Players WHERE AgentID IN('{0}'))", Join(polstUserIDs.ToArray(), "','")))
            End If

            oWhere.AppendANDCondition(getSQLDateRange("TransactionDate", SafeString(poSDate), SafeString(poEDate), True))

            Dim sSQL As String = "SELECT SUM(PLAmount) as PLAmount FROM Transactions " & vbCrLf & oWhere.SQL

            Try
                log.Debug("Get TransactionPLAmount from list of PlayerIDs. SQL: " & sSQL)
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                Return SafeDouble(oDB.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Fails to Get TransactionPLAmount from list of PlayerIDs. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return 0
        End Function
#End Region

#Region "Agent's SubAgents dashboard"
        Public Function GetAgentsBalance(ByVal psUserID As String) As Double
            ' Create Standard table
            'Dim oTable As New DataTable

            'oTable.Columns.Add("Agent", GetType(String))
            'oTable.Columns.Add("AgentID", GetType(String))
            'oTable.Columns.Add("ActivePlayers", GetType(Integer))

            ' Get SubAgentsBet
            'Dim oMainTable As DataTable = getSubAgentsBet(psUserID, poStartDate, pbSuperAdmin)
            'If oMainTable IsNot Nothing Then
            ' Get SubPlayer's Name
            Dim oSubAgents As DataTable
            'oSubAgents = GetAgentsByAgentID(psUserID, Nothing)
            oSubAgents = GetAgentsByAgentID(psUserID, Nothing)
            Dim agentbalance As Double
            Dim oMainTable As DataTable = getSubAgentsBet(psUserID, GetMondayOfCurrentWeek(), False)
            Dim oLstSubAgentIDs As List(Of String) = GetAllSubAgentIDs(psUserID)
            oLstSubAgentIDs.Add(psUserID)
            

            Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND AgentID IN ('{0}')"
            sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"))

            Dim sNetAmount As String = "TicketNetAmount"

            '' Get NetAmount greater than 0 first
            agentbalance = SafeDouble(oMainTable.Compute(String.Format("SUM({0})", sNetAmount), _
                                                sWhere & String.Format(" AND {0} > 0", sNetAmount)))

            '' Adding with Percentage of NetAmount less than 0
            'agentbalance = agentbalance + (SafeDouble(oMainTable.Compute(String.Format("SUM({0})", sNetAmount), _
            '  sWhere & String.Format(" AND {0} < 0", sNetAmount))) * nPercentage / 100)


            'If SafeDouble(odr(0)("ProfitPercentage")) > 0 Then
            '    agentbalance = agentbalance * SafeDouble(odr(0)("ProfitPercentage")) / 100
            'End If
            Return agentbalance
        End Function

        Public Function GetAgentsDashboard(ByVal psUserID As String, Optional ByVal poStartDate As Date = Nothing, Optional ByVal pbSuperAdmin As Boolean = False, Optional ByVal pbByTicketNetAmount As Boolean = False) As DataTable
            ' Create Standard table
            Dim oDay As String() = {"Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun", "Gross", "Net", "PL", "Pending"}
            Dim oTable As New DataTable

            oTable.Columns.Add("Agent", GetType(String))
            oTable.Columns.Add("AgentID", GetType(String))
            oTable.Columns.Add("ActivePlayers", GetType(Integer))
            For Each sDay In oDay
                oTable.Columns.Add(sDay, GetType(Double))
            Next

            ' Get SubAgentsBet
            Dim oMainTable As DataTable = getSubAgentsBet(psUserID, poStartDate, pbSuperAdmin)
            If oMainTable IsNot Nothing Then
                ' Get SubPlayer's Name
                Dim oSubAgents As DataTable
                If pbSuperAdmin Then
                    oSubAgents = GetAgentsBySuperID(psUserID, Nothing)
                Else
                    oSubAgents = GetAgentsByAgentID(psUserID, Nothing)
                End If

                If oSubAgents IsNot Nothing Then
                    ' Get Sum of Amounts
                    Dim oRow As DataRow
                    Dim nTotalGross, nTotalNet, nTotalPL, nPercentage As Double
                    Dim oMondayOfWeek As Date
                    If poStartDate = Nothing Then
                        oMondayOfWeek = GetMondayOfCurrentWeek()
                    Else
                        oMondayOfWeek = poStartDate
                    End If

                    '' Get previous Monday 5:30AM by Eastern timezone
                    Dim oEasternMon As Date = SafeDate(oMondayOfWeek.ToString("MM/dd/yyyy") & " 06:00:00")

                    For Each oDRSubAgent As DataRow In oSubAgents.Select("", "Login")
                        oRow = oTable.NewRow
                        oRow("Agent") = SafeString(oDRSubAgent("Login"))

                        If pbSuperAdmin Then
                            ' Get Active Players
                            Dim tblActivePlayers = GetActivePlayers(SafeString(oDRSubAgent("AgentID")), oMondayOfWeek, _
                                                               oMondayOfWeek.AddDays(6))

                            If tblActivePlayers IsNot Nothing Then
                                oRow("ActivePlayers") = tblActivePlayers.Rows.Count
                            End If
                        End If

                        oRow("AgentID") = SafeString(oDRSubAgent("AgentID"))

                        Dim oLstSubAgentIDs As List(Of String) = GetAllSubAgentIDs(SafeString(oDRSubAgent("AgentID")))
                        oLstSubAgentIDs.Add(SafeString(oDRSubAgent("AgentID")))

                        nTotalGross = 0
                        nTotalNet = 0
                        nTotalPL = 0
                        For Each sDay As String In oDay
                            If sDay = "Gross" OrElse sDay = "Net" OrElse sDay = "PL" Then
                                Continue For
                            End If

                            Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND AgentID IN ('{0}') AND TicketCompletedDate = {1}"
                            Select Case sDay
                                Case "Mon"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek, "MM/dd/yyyy")))
                                Case "Tues"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(1), "MM/dd/yyyy")))
                                Case "Wed"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(2), "MM/dd/yyyy")))
                                Case "Thurs"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(3), "MM/dd/yyyy")))
                                Case "Fri"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(4), "MM/dd/yyyy")))
                                Case "Sat"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(5), "MM/dd/yyyy")))
                                Case "Sun"
                                    sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                                Case "Pending"
                                    sWhere = String.Format("ISNULL(TicketStatus,'Open') IN ('Open', 'Pending') AND " & _
                                                           "AgentID IN ('{0}') AND TransactionDate <={1}", _
                                                           Join(oLstSubAgentIDs.ToArray(), "','"), _
                                                           SQLString(Format(oMondayOfWeek.AddDays(6), "MM/dd/yyyy")))
                            End Select

                            log.Debug(sWhere)
                            If sDay <> "Pending" Then

                                If pbByTicketNetAmount Then
                                    oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(TicketNetAmount)", sWhere))
                                    nTotalGross += SafeDouble(oRow(sDay))
                                Else
                                    oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(NetAmount)", sWhere))
                                    nTotalGross += SafeDouble(oRow(sDay))
                                End If

                            Else
                                oRow(sDay) = SafeDouble(oMainTable.Compute("SUM(PendingAmount)", sWhere))
                            End If
                        Next

                        oRow("Gross") = nTotalGross

                        If poStartDate = Nothing Then '' Dashboard for current week
                            nTotalNet = nTotalGross

                            '' Get Percentage
                            '' Gross Percentage
                            nPercentage = SafeDouble(oDRSubAgent("GrossPercentage"))
                            If nPercentage <> 0 Then
                                Dim sWhere As String = "ISNULL(TicketStatus,'Open') NOT IN ('Open', 'Pending') AND AgentID IN ('{0}')"
                                sWhere = String.Format(sWhere, Join(oLstSubAgentIDs.ToArray(), "','"))

                                Dim sNetAmount As String = "NetAmount"
                                If pbByTicketNetAmount Then
                                    sNetAmount = "TicketNetAmount"
                                End If

                                '' Get NetAmount greater than 0 first
                                nTotalNet = SafeDouble(oMainTable.Compute(String.Format("SUM({0})", sNetAmount), _
                                                            sWhere & String.Format(" AND {0} > 0", sNetAmount)))

                                '' Adding with Percentage of NetAmount less than 0
                                nTotalNet = nTotalNet + (SafeDouble(oMainTable.Compute(String.Format("SUM({0})", sNetAmount), _
                                                        sWhere & String.Format(" AND {0} < 0", sNetAmount))) * nPercentage / 100)
                            End If

                            nTotalPL = nTotalNet
                            If SafeDouble(oDRSubAgent("ProfitPercentage")) > 0 Then
                                nTotalPL = nTotalPL * SafeDouble(oDRSubAgent("ProfitPercentage")) / 100
                            End If

                        Else '' Dashboard for previous week

                            nTotalNet = GetTransactionAmount(oLstSubAgentIDs, oEasternMon, oEasternMon.AddDays(7), False)
                            nTotalPL = GetTransactionPLAmount(oLstSubAgentIDs, oEasternMon, oEasternMon.AddDays(7), False)
                        End If

                        oRow("Net") = nTotalNet
                        oRow("PL") = nTotalPL

                        oTable.Rows.Add(oRow)
                    Next

                    ' Calc Totals Row
                    oRow = oTable.NewRow
                    oRow("Agent") = "Totals"
                    For Each sDay As String In oDay
                        oRow(sDay) = SafeDouble(oTable.Compute(String.Format("SUM({0})", sDay), "True"))
                    Next
                    oTable.Rows.Add(oRow)
                End If
            End If

            Return oTable
        End Function

        Private Function getSubAgentsBet(ByVal psAgentId As String, Optional ByVal poStartDate As Date = Nothing, Optional ByVal pbSuperAdmin As Boolean = False) As DataTable

            Dim oDB As CSQLDBUtils = Nothing
            Dim sSQL As String = "SELECT Convert(varchar(40),t.AgentID) AS AgentID, (t.NetAmount - t.RiskAmount) as NetAmount, t.RiskAmount AS PendingAmount,t.NetAmount as TicketNetAmount, " & _
            " CAST(CONVERT(NVARCHAR(10), t.TicketCompletedDate, 101) AS SMALLDATETIME) AS TicketCompletedDate, " & _
            " CAST(CONVERT(NVARCHAR(10), t.TransactionDate, 101) AS SMALLDATETIME) AS TransactionDate, t.TicketStatus " & vbCrLf & _
            " FROM Tickets t " & vbCrLf & " {0} " & vbCrLf & _
            " GROUP BY t.TicketID, t.AgentID, t.NetAmount, t.RiskAmount, t.TransactionDate, t.TicketCompletedDate, t.TicketStatus "

            Try
                Dim oListSubAgentIDs As List(Of String) = GetAllSubAgentIDs(psAgentId, pbSuperAdmin)

                If oListSubAgentIDs.Count > 0 Then
                    Dim oWhere As New CSQLWhereStringBuilder()
                    oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oListSubAgentIDs.ToArray(), "','")))

                    If poStartDate = Nothing Then
                        oWhere.AppendANDCondition(String.Format("(t.TicketCompletedDate >= {0} OR ISNULL(t.TicketStatus, 'Open') IN ('Open','Pending'))", _
                                                                SQLString(Format(GetMondayOfCurrentWeek, "yyyy-MM-dd"))))
                    Else
                        oWhere.AppendANDCondition("( " & getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poStartDate.AddDays(6))) & _
                                                  " OR ISNULL(t.TicketStatus, 'Open') IN ('Open','Pending'))")
                    End If

                    sSQL = String.Format(sSQL, oWhere.SQL)

                    log.Debug("Get main datatable for SubAgentsBet. SQL: " & sSQL)
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                    Return oDB.getDataTable(sSQL)
                End If
            Catch ex As Exception
                log.Error("Fails to get AgentsDashboard.SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return Nothing
        End Function

        Public Function GetAllSubAgentIDs(ByVal psAgentID As String, Optional ByVal pbSuperAdmin As Boolean = False) As List(Of String)
            Dim oLstAgents As New List(Of String)
            Dim oDB As CSQLDBUtils = Nothing
            If System.Web.HttpContext.Current.Cache(strKeyGetAllSubAgentIDs & psAgentID) Is Nothing Then
                Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, [Login], [Level]) AS " & _
                                "( " & _
                                    " SELECT AgentID, ParentID, [Login], 0 as [Level] " & _
                                    " FROM Agents WHERE {0}={1} " & _
                                    " UNION ALL " & _
                                    " SELECT child.AgentID, child.ParentID, child.Login, AgentRecursive.[Level]+1 as [Level] " & _
                                    " FROM Agents as child " & _
                                    " INNER JOIN AgentRecursive ON AgentRecursive.AgentID = child.ParentID " & _
                                ") " & _
                                " SELECT * FROM AgentRecursive"

                Try
                    If pbSuperAdmin Then
                        sSQL = String.Format(sSQL, "SuperAdminID", SQLString(psAgentID))
                    Else
                        sSQL = String.Format(sSQL, "AgentID", SQLString(psAgentID))
                    End If


                    log.Debug("Get AllSubagentsID of Agent. SQL: " & sSQL)
                    oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")

                    Dim oDT As DataTable = oDB.getDataTable(sSQL)
                    For Each oDR As DataRow In oDT.Rows
                        oLstAgents.Add(SafeString(oDR("AgentID")))
                    Next
                Catch ex As Exception
                    log.Error("Fails to get SubAgentIDs.SQL: " & sSQL, ex)
                Finally
                    If oDB IsNot Nothing Then oDB.closeConnection()
                End Try
                System.Web.HttpContext.Current.Cache.Add(strKeyGetAllSubAgentIDs & psAgentID, oLstAgents, Nothing, Date.Now.AddMinutes(200), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
            End If
            Return CType(System.Web.HttpContext.Current.Cache(strKeyGetAllSubAgentIDs & psAgentID), List(Of String))
        End Function

#End Region

        Public Function CheckPassword(ByVal psAgentID As String, ByVal psPassword As String) As Boolean
            Dim oDB As CSQLDBUtils = Nothing

            Dim sSQL As String = String.Format("SELECT COUNT(*) FROM Agents WHERE AgentID={0} AND Password={1}", _
                                               SQLString(psAgentID), SQLString(psPassword))

            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Return SafeInteger(oDB.getScalerValue(sSQL)) > 0

            Catch ex As Exception
                log.Error("Fails to check agent's password. SQL: " & sSQL, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            Return False
        End Function

        Public Function IsExistedLogin(ByVal psLogin As String, ByVal psAgentID As String, ByVal psSiteType As String) As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("Login=" & SQLString(psLogin))
            oWhere.AppendANDCondition("SiteType=" & SQLString(psSiteType))
            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''SuperAdmin
                sSQL = "SELECT count(Login) FROM SuperAdmins " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0

                ''CallCenterAgent
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM CallCenterAgents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''Player
                If Not bExisted Then
                    sSQL = "SELECT count(Login) FROM Players " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If

                ''Agent
                If Not bExisted Then
                    oWhere.AppendOptionalANDCondition("AgentID", SQLString(psAgentID), "<>")
                    sSQL = "SELECT count(Login) FROM Agents " & oWhere.SQL
                    bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0
                End If
            Catch ex As Exception
                log.Error("Cannot check login existed. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function IsExistedSpecialKey(ByVal psSpecialKey As String, Optional ByVal psAgentID As String = "") As Boolean
            Dim bExisted As Boolean = False

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("SpecialKey =" & SQLString(psSpecialKey))
            oWhere.AppendOptionalANDCondition("AgentID", SQLString(psAgentID), "<>")

            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                ''SuperAdmin
                sSQL = "SELECT count(Login) FROM Agents " & oWhere.SQL
                bExisted = SafeInteger(oDb.getScalerValue(sSQL)) > 0

            Catch ex As Exception
                log.Error("Cannot check existed special key. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bExisted
        End Function

        Public Function GetCurrentPlayerNumber(ByVal psAgentID As String) As Integer
            Dim nNum As Integer

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID =" & SQLString(psAgentID))

            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                sSQL = "SELECT CurrentPlayerNumber FROM Agents " & oWhere.SQL
                nNum = SafeInteger(oDb.getScalerValue(sSQL))

            Catch ex As Exception
                log.Error("Cannot get current player number. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return nNum
        End Function

        Public Function IncreaseCurrentPlayerNumber(ByVal psAgentID As String, ByVal pnCurrentPlayer As Integer) As Boolean
            Dim bResult As Boolean

            Dim oWhere As New CSQLWhereStringBuilder()
            oWhere.AppendANDCondition("AgentID =" & SQLString(psAgentID))

            Dim sSQL As String = ""
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim oUpdate As New CSQLUpdateStringBuilder("Agents", oWhere.SQL)
                oUpdate.AppendString("CurrentPlayerNumber", SQLString(pnCurrentPlayer))
                sSQL = oUpdate.SQL

                oDb.executeNonQuery(sSQL)
                bResult = True
            Catch ex As Exception
                log.Error("Cannot increase current player number. SQL: " & sSQL, ex)
            Finally
                oDb.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function HasPlayers(ByVal psAgentID As String) As Boolean
            Dim sSQL As String = "select * from Players where AgentID= {0}"
            sSQL = String.Format(sSQL, SQLString(psAgentID))
            Dim oDT As New DataTable
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oDT = oDb.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get count of player: " & ex.Message, ex)
                Return True
            Finally
                oDb.closeConnection()
            End Try

            Return oDT.Rows.Count > 0
        End Function

        Public Function HasSubAgents(ByVal psAgentID As String) As Boolean
            Dim sSQL As String = "select * from Agents where ParentID= {0}"
            sSQL = String.Format(sSQL, SQLString(psAgentID))
            Dim oDT As New DataTable
            Dim oDb As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                oDT = oDb.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't chack have sub agent: " & ex.Message, ex)
                Return True
            Finally
                oDb.closeConnection()
            End Try

            Return oDT.Rows.Count > 0
        End Function

        Public Function GetActivePlayers(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim sSQL As String = "WITH AgentRecursive(AgentID, ParentID, [FullName], [Level]) AS " & vbCrLf & _
            " (    SELECT AgentID, ParentID, [Login] , 0 as [Level] " & vbCrLf & _
            " FROM Agents WHERE AgentID={0} AND ISNULL(IsLocked,'N') <> 'Y' " & vbCrLf & _
            " UNION ALL " & vbCrLf & _
            " SELECT child.AgentID, child.ParentID,child.Login , AgentRecursive.[Level]+1 as [Level] " & vbCrLf & _
            " FROM Agents as child " & vbCrLf & _
            " INNER JOIN AgentRecursive ON (AgentRecursive.AgentID = child.ParentID AND ISNULL(child.IsLocked,'N') <> 'Y') " & vbCrLf & _
            " ) " & vbCrLf & _
            "  SELECT distinct ar.[Level], ar.FullName AS AgentFullName, p.PlayerID, p.[Login] AS PlayerFullName  " & vbCrLf & _
            " FROM AgentRecursive ar LEFT JOIN Players p ON (ar.AgentID = p.AgentID) inner join Tickets as t on (t.PlayerID= p.PlayerID and t.TransactionDate between '{1}' and '{2}') " & vbCrLf & _
            " ORDER BY ar.[Level], ar.FullName, PlayerFullName "

            sSQL = String.Format(sSQL, SQLString(psAgentID), SafeString(poStartDate), SafeString(poEndDate & " 11:59:59 PM"))
            Dim otblResult As DataTable = Nothing
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                log.Debug("Get All Active Players. SQL: " & sSQL)
                otblResult = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get all Active Players: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return otblResult
        End Function

        Public Function UpdateWeeklyCharge(ByVal psAgentID As String, ByVal pnWeeklyCharge As Double, ByVal psChangedBy As String) As Boolean
            Dim bSuccess As Boolean = False

            Dim oUpdate As New CSQLUpdateStringBuilder("Agents", "WHERE AgentID= " & SQLString(psAgentID))
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)

            Try
                With oUpdate
                    .AppendString("WeeklyCharge", SQLDouble(pnWeeklyCharge))
                End With

                log.Debug("Update Weekly Charge. SQL: " & oUpdate.SQL)
                oDB.executeNonQuery(oUpdate, psChangedBy)

                ' Clear cache
                Dim oCache As New CacheUtils.CCacheManager()
                oCache.ClearAgentInfo(psAgentID, "")

                bSuccess = True
            Catch ex As Exception
                log.Error("Error trying to update Weekly Charge. SQL: " & oUpdate.SQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bSuccess
        End Function

        Sub ClearAgentCache()
            Dim keys As New List(Of String)
            Dim CacheEnum As IDictionaryEnumerator = System.Web.HttpContext.Current.Cache.GetEnumerator()
            While CacheEnum.MoveNext()
                If CacheEnum.Key.ToString().Contains(strKeyGetAllSubAgentIDs) Then
                    System.Web.HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString())
                End If
            End While
        End Sub

#Region "Reports"

        Public Function GetVolumnReportForAgent(ByVal psAgentID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As DataTable
            Dim sSQL As String = "SELECT p.Name, p.[Login], p.PlayerID, BetGames.GameType, SUM(t.BetAmount) Total, " & vbCrLf & _
            "SUM(case when t.TicketType = 'Straight' then t.BetAmount else 0 end) StraightTotal, " & vbCrLf & _
            "SUM(case when t.TicketType in ('Action Reverse', 'Parlay', 'Teaser', 'Proposition') then t.BetAmount else 0 end) FiveComTotal " & vbCrLf & _
            "FROM Players as p " & vbCrLf & _
            "INNER JOIN Tickets as t on t.PlayerID = p.PlayerID " & vbCrLf & _
            "CROSS APPLY ( " & vbCrLf & _
            "    SELECT TOP 1 tb.TicketBetStatus, g.GameDate, g.GameType " & vbCrLf & _
            "    FROM TicketBets as tb  INNER JOIN Games as g on tb.GameID = g.GameID " & vbCrLf & _
            "    WHERE tb.TicketID = t.TicketID " & vbCrLf & _
            ") BetGames " & vbCrLf & _
            "WHERE t.AgentID = {0} and BetGames.TicketBetStatus not in ('Open', 'Deleted') and BetGames.GameDate >= {1} and BetGames.GameDate < {2} " & vbCrLf & _
            "GROUP BY p.PlayerID, p.Name, p.[Login], BetGames.GameType " & vbCrLf & _
            "HAVING SUM(t.BetAmount) > 0"

            sSQL = String.Format(sSQL, SQLString(psAgentID), SQLString(poStartDate), SQLString(poEndDate))
            Dim otblResult As DataTable = Nothing
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                log.Debug("Volumn Report. SQL: " & sSQL)
                otblResult = oDB.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can't get Volumn Report: " & ex.Message, ex)
            Finally
                oDB.closeConnection()
            End Try
            ' convert to report data
            Dim oTable As New DataTable
            If otblResult IsNot Nothing AndAlso otblResult.Rows.Count > 0 Then

                Dim tCols As String() = {"FootBall", "BasketBall", "BaseBall", "Hockey", "Soccer", "Other", "FiveBetTotal", "Total"}
                oTable.Columns.Add("PlayerId", GetType(String))
                oTable.Columns.Add("PlayerName", GetType(String))
                For Each col In tCols
                    oTable.Columns.Add(col, GetType(Double))
                Next

                Dim footBallGameTypes = "'" & String.Join("','", std.FOOTBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)) & "'"
                Dim basketBallGameTypes = "'" & String.Join("','", std.BASKETBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)) & "'"
                Dim baseBallGameTypes = "'" & String.Join("','", std.BASEBALL_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)) & "'"
                Dim hockeyBallGameTypes = "'" & String.Join("','", std.HOCKEY_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)) & "'"
                Dim sockerBallGameTypes = "'" & String.Join("','", std.SOCCER_GAMES.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)) & "'"

                For Each vrow As DataRow In otblResult.Rows
                    Dim matchRows As DataRow() = oTable.Select(String.Format("PlayerId ='{0}'", vrow("PlayerID")))
                    If matchRows.Length = 0 Then
                        Dim drow As DataRow = oTable.NewRow()
                        drow("PlayerId") = vrow("PlayerID")
                        drow("PlayerName") = String.Format("{0}, ({1})", vrow("Name"), vrow("Login"))

                        drow("FootBall") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType in ({0}) and PlayerID = '{1}'", footBallGameTypes, vrow("PlayerID"))))
                        drow("BasketBall") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType in ({0}) and PlayerID = '{1}'", basketBallGameTypes, vrow("PlayerID"))))
                        drow("BaseBall") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType in ({0}) and PlayerID = '{1}'", baseBallGameTypes, vrow("PlayerID"))))
                        drow("Hockey") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType in ({0}) and PlayerID = '{1}'", hockeyBallGameTypes, vrow("PlayerID"))))
                        drow("Soccer") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType in ({0}) and PlayerID = '{1}'", sockerBallGameTypes, vrow("PlayerID"))))
                        drow("Other") = SafeDouble(otblResult.Compute("Sum(StraightTotal)", String.Format("GameType not in ({0}, {1}, {2}, {3}, {4}) and PlayerID = '{5}'", footBallGameTypes, basketBallGameTypes, baseBallGameTypes, hockeyBallGameTypes, sockerBallGameTypes, vrow("PlayerID"))))
                        drow("FiveBetTotal") = SafeDouble(otblResult.Compute("Sum(FiveComTotal)", String.Format("PlayerID = '{0}'", vrow("PlayerID"))))
                        drow("Total") = SafeDouble(otblResult.Compute("Sum(Total)", String.Format("PlayerID = '{0}'", vrow("PlayerID"))))

                        oTable.Rows.Add(drow)

                    End If
                Next

                ' Total Agent Row
                Dim totalAgentrow As DataRow = oTable.NewRow()
                totalAgentrow("PlayerId") = ""
                totalAgentrow("PlayerName") = "Agent Total"

                totalAgentrow("FootBall") = SafeDouble(oTable.Compute("Sum(FootBall)", ""))
                totalAgentrow("BasketBall") = SafeDouble(oTable.Compute("Sum(BasketBall)", ""))
                totalAgentrow("BaseBall") = SafeDouble(oTable.Compute("Sum(BaseBall)", ""))
                totalAgentrow("Hockey") = SafeDouble(oTable.Compute("Sum(Hockey)", ""))
                totalAgentrow("Soccer") = SafeDouble(oTable.Compute("Sum(Soccer)", ""))
                totalAgentrow("Other") = SafeDouble(oTable.Compute("Sum(Other)", ""))
                totalAgentrow("FiveBetTotal") = SafeDouble(oTable.Compute("Sum(FiveBetTotal)", ""))
                totalAgentrow("Total") = SafeDouble(oTable.Compute("Sum(Total)", ""))

                oTable.Rows.Add(totalAgentrow)
            End If

            Return oTable
        End Function

#End Region

    End Class
End Namespace