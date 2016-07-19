Imports SBCBL.std
Imports SBCBL.CacheUtils

Imports WebsiteLibrary.DBUtils

Namespace Managers

    Public Class CTicketManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CTicketManager))

        Public Function GetOpenTicketsByPlayer(ByVal psPlayerID As String, ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Return GetOpenTicketsByCCAgent("", psPlayerID, poTransToDate, psTypeOfBet)
        End Function

        Public Function GetPositionReportByAllSubAgent(ByVal oLstAgentIDs As List(Of String), ByVal poTransToDate As Date, ByVal psGameType As String, Optional ByVal pbStartDate As Boolean = False, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendANDCondition("TicketType='Straight'")
            oWhere.AppendANDCondition("GameType=" & SQLString(psGameType))
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            If pbStartDate Then
                oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", poTransToDate.ToString("MM/dd/yyyy"), SafeString(poTransToDate)))
            Else
                oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            End If
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            'oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets    " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TransactionDate, t.TicketNumber, t.SubTicketNumber, t.TicketID, t.TicketType,  p.Name  "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetOpenTicketsByCCAgent(ByVal psCCAgentID As String, ByVal psPlayerID As String, ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing

            Dim oGameDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oGameDate.IsDaylightSavingTime() Then
                oGameDate.AddHours(1)
            End If

            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendOptionalANDCondition("t.OrderBy", SQLString(psCCAgentID), "=")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition("isnull(t.TicketStatus,'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oGameDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , a.Name as AgentName, TeaserRuleName, p.PhoneLogin as PlayerName,CallCenterAgents.Login, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "INNER JOIN Agents a ON a.AgentID=t.AgentID " & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID=tr.TeaserRuleID" & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ON t.OrderBy=CallCenterAgents.CallCenterAgentID " & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TicketNumber , t.SubTicketNumber ,  t.TransactionDate DESC ,  t.TicketID, t.TicketType, AgentName"
            log.Debug("Get the list of tickets by player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of tickets by player. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetOpenTicketsByAgentPosition(ByVal psContext As String, ByVal psAgentID As String, ByVal poTransToDate As Date, ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psBetType As String, Optional ByVal psTypeOfBet As String = "All", Optional ByVal plstGameType As List(Of String) = Nothing) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                If psTypeOfBet.Equals("Prop") Then
                    oWhere.AppendANDCondition("t.IsForProp = 'Y'")
                ElseIf psTypeOfBet.Contains("IF") Then
                    oWhere.AppendANDCondition("t.TicketType like  'If%'")
                Else
                    oWhere.AppendANDCondition("t.TicketType = " & SQLString(psTypeOfBet))
                End If

            End If
            If plstGameType IsNot Nothing AndAlso plstGameType.Count > 0 Then
                oWhere.AppendANDCondition(String.Format("g.GameType in ('{0}') ", Join(plstGameType.ToArray(), "','")))
            End If
            oWhere.AppendANDCondition("tb.Context=" & SQLString(psContext))
            oWhere.AppendANDCondition("tb.BetType=" & SQLString(psBetType))
            oWhere.AppendANDCondition("g.AwayTeam=" & SQLString(psAwayTeam))
            oWhere.AppendANDCondition("g.HomeTeam=" & SQLString(psHomeTeam))
            oWhere.AppendANDCondition("t.AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            'oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TransactionDate, t.TicketNumber, t.SubTicketNumber, t.TicketID, t.TicketType,  p.Name  "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetOpenTicketsByAgent(ByVal psAgentID As String, ByVal poTransToDate As Date, Optional ByVal psPlayerID As String = "", Optional ByVal psTypeOfBet As String = "All", Optional ByVal plstGameType As List(Of String) = Nothing, Optional ByVal pnMoney As Double = 0) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                If psTypeOfBet.Equals("Prop") Then
                    oWhere.AppendANDCondition("t.IsForProp = 'Y'")
                ElseIf psTypeOfBet.Contains("IF") Then
                    oWhere.AppendANDCondition("t.TicketType like  'If%'")
                Else
                    oWhere.AppendANDCondition("t.TicketType = " & SQLString(psTypeOfBet))
                End If

            End If
            If plstGameType IsNot Nothing AndAlso plstGameType.Count > 0 Then
                oWhere.AppendANDCondition(String.Format("g.GameType in ('{0}') ", Join(plstGameType.ToArray(), "','")))
            End If
            If pnMoney > 0 Then

                oWhere.AppendANDCondition("t.BetAmount>=" & SQLString(pnMoney))
            End If
            oWhere.AppendANDCondition("t.AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            'oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TransactionDate, t.TicketNumber, t.SubTicketNumber, t.TicketID, t.TicketType,  p.Name  "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetDiffOpenTicketsByAllSubAgent(ByVal psGameID As String, ByVal psBetType As String, ByVal psGameContext As String, ByVal oLstAgentIDs As List(Of String), ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            oWhere.AppendANDCondition("g.GameID='" + psGameID + "'")
            oWhere.AppendANDCondition("t.TicketType='Straight'")
            oWhere.AppendANDCondition("tb.Context='" + psGameContext + "'")
            oWhere.AppendANDCondition("tb.BetType='" + psBetType + "'")
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            'oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets    " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TransactionDate, t.TicketNumber, t.SubTicketNumber, t.TicketID, t.TicketType,  p.Name  "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetOpenTicketsByAllSubAgent(ByVal oLstAgentIDs As List(Of String), ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All", Optional ByVal pnMoney As Double = 0) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            If pnMoney > 0 Then

                oWhere.AppendANDCondition("t.BetAmount>=" & SQLString(pnMoney))
            End If
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            'oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets    " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TransactionDate, t.TicketNumber, t.SubTicketNumber, t.TicketID, t.TicketType,  p.Name  "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function


        Public Function GetOpenTicketsByCCAgent(ByVal psCCAgentID As String, ByVal poTransToDate As Date, Optional ByVal psPlayerID As String = "") As DataTable
            Dim odtTickets As DataTable = Nothing

            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("t.OrderBy=" & SQLString(psCCAgentID))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending' ) ")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition(getSQLDateRange("t.TransactionDate", "", SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.PhoneLogin as PlayerName, TeaserRuleName  " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON tr.TeaserRuleID =t.TeaserRuleID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY  t.TransactionDate , t.TicketNumber , t.SubTicketNumber,t.TicketID, t.TicketType,  p.PhoneLogin "
            log.Debug("Get the list of tickets by cc agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by cc agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetAllCompletedTickets(ByVal poTransFromDate As Date, ByVal poTransToDate As Date) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') NOT IN ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT CONVERT(DateTime, CONVERT(Char(10), t.TicketCompletedDate, 101)) as TicketCompletedDate, " & _
            " t.RiskAmount, (t.NetAmount - t.RiskAmount) as NetAmount " & vbCrLf & _
            "FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.TicketCompletedDate, t.RiskAmount, t.NetAmount "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of sport tickets . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetAllCompletedTicketsByAgentID(ByVal psAgentID As String, ByVal poTransFromDate As Date, ByVal poTransToDate As Date) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') NOT IN ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.AgentID = " & SQLString(psAgentID))
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT CONVERT(DateTime, CONVERT(Char(10), t.TicketCompletedDate, 101)) as TicketCompletedDate, " & _
            " t.RiskAmount, (t.NetAmount - t.RiskAmount) as NetAmount " & vbCrLf & _
            "FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.TicketCompletedDate, t.RiskAmount, t.NetAmount "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of sport tickets . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetAllCompletedTicketsListByPlayerID(ByVal psPlayerID As String, ByVal poTransFromDate As Date, ByVal poTransToDate As Date) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') NOT IN ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT CONVERT(DateTime, CONVERT(Char(10), t.TicketCompletedDate, 101)) as TicketCompletedDate, " & _
            " t.RiskAmount, (t.NetAmount - t.RiskAmount) as NetAmount ,t.TicketType" & vbCrLf & _
            "FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.TicketCompletedDate, t.RiskAmount, t.NetAmount,t.TicketType "
            log.Debug("Get the list of tickets by Player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of sport Player . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function


        Public Function GetAllCompletedTicketsListByAgentID(ByVal oLstAgentIDs As List(Of String), ByVal poTransFromDate As Date, ByVal poTransToDate As Date) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') NOT IN ('Open', 'Pending')")
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT CONVERT(DateTime, CONVERT(Char(10), t.TicketCompletedDate, 101)) as TicketCompletedDate, " & _
            " t.RiskAmount, (t.NetAmount - t.RiskAmount) as NetAmount ,t.TicketType" & vbCrLf & _
            "FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & _
            " GROUP BY t.TicketID, t.TicketCompletedDate, t.RiskAmount, t.NetAmount,t.TicketType "
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of sport tickets . SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetHistoryTicketsByPlayer(ByVal psPlayerID As String _
                                                , ByVal poTransFromDate As Date, ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendANDCondition("t.PlayerID=" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(tb.TicketBetStatus,'OPEN') <> 'OPEN' ")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*,g.*, TeaserRuleName,ce.Login as CAgentLoginName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets   " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr on t.TeaserRuleID = tr.TeaserRuleID" & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                "  ORDER BY t.TicketNumber , t.SubTicketNumber, t.TicketID, g.Gamedate , t.TicketType "
            log.Debug("Get the list of history tickets by player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of history tickets by player. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetHistoryTicketsByAgent(ByVal psAgentID As String _
                                                        , ByVal poTransFromDate As Date, ByVal poTransToDate As Date _
                                                        , Optional ByVal psPlayerID As String = "", Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing
            If psPlayerID.Equals("All", StringComparison.CurrentCultureIgnoreCase) Then
                psPlayerID = ""
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendANDCondition("t.AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("ISNULL(tb.TicketBetStatus,'OPEN') <> 'OPEN' ")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Login as PlayerName, TeaserRuleName, ce.Login as CAgentLoginName , tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY  t.TicketNumber , t.SubTicketNumber,t.TicketID,g.Gamedate , t.TicketType,  p.Name"
            log.Debug("Get the list of history tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot the list of history tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetALLHistoryTicketsByListAgentID(ByVal oLstAgentIDs As List(Of String) _
                                                        , ByVal poTransFromDate As Date, ByVal poTransToDate As Date, Optional ByVal psTypeOfBet As String = "All") As DataTable
            Dim odtTickets As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder
            If Not psTypeOfBet.Equals("All", StringComparison.OrdinalIgnoreCase) Then
                oWhere.AppendANDCondition("t.TypeOfBet = " & SQLString(psTypeOfBet))
            End If
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition("ISNULL(tb.TicketBetStatus,'OPEN') <> 'OPEN' ")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.Name as PlayerName, p.Login as Player , TeaserRuleName , ce.Login as CAgentLoginName , tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID =tr.TeaserRuleID " & vbCrLf & _
                "LEFT OUTER JOIN CallCenterAgents ce on t.OrderBy =ce.CallCenterAgentID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY  t.TicketNumber , t.SubTicketNumber,t.TicketID,g.Gamedate , t.TicketType,  p.Name"
            log.Debug("Get the list of history tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot the list of history tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetHistoryTicketsByCCAgent(ByVal psCCAgentID As String _
                                                          , ByVal poTransFromDate As Date, ByVal poTransToDate As Date _
                                                          , Optional ByVal psPlayerID As String = "", Optional ByVal psHasPhoneLog As String = "") As DataTable
            Dim odtTickets As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psHasPhoneLog.Equals("") Then
                If psHasPhoneLog.Equals("Y") Then
                    oWhere.AppendANDCondition("t.AsteriskID is not null")
                Else
                    oWhere.AppendANDCondition("t.AsteriskID is null")
                End If
            End If
            'oWhere.AppendANDCondition("t.CallCenterAgentID <> null")
            oWhere.AppendANDCondition("t.OrderBy=" & SQLString(psCCAgentID))
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'OPEN') NOT IN ('OPEN','PENDING')")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.PhoneLogin as PlayerName, TeaserRuleName ,CallCenterAgents.Login,CallCenterAgents.Name as  CallCenterAgentName,tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets  " & vbCrLf & _
                 "FROM Tickets t " & vbCrLf & _
                 "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                 "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                  "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                 "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                 "INNER JOIN CallCenterAgents ON t.OrderBy=CallCenterAgents.CallCenterAgentID " & vbCrLf & _
                 "LEFT OUTER JOIN TeaserRules tr ON tr.TeaserRuleID =t.TeaserRuleID " & vbCrLf & _
                 oWhere.SQL & vbCrLf & _
                 " ORDER BY CallCenterAgents.Login desc , t.SubTicketNumber, t.TicketID, g.Gamedate DESC, t.TicketType,  p.PhoneLogin"
            log.Debug("Get the list of history tickets by cc agents. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot the list of history tickets by cc agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetHistoryTicketsByALLCCAgent(ByVal poTransFromDate As Date, ByVal poTransToDate As Date, ByVal psSiteType As String _
                                                         , Optional ByVal psPlayerID As String = "", Optional ByVal psHasPhoneLog As String = "") As DataTable
            Dim odtTickets As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder
            If Not psHasPhoneLog.Equals("") Then
                If psHasPhoneLog.Equals("Y") Then
                    oWhere.AppendANDCondition("t.AsteriskID is not null")
                Else
                    oWhere.AppendANDCondition("t.AsteriskID is null")
                End If
            End If
            'oWhere.AppendANDCondition("t.CallCenterAgentID <> null")
            oWhere.AppendANDCondition("CallCenterAgents.SiteType=" & SQLString(psSiteType))
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'OPEN') NOT IN ('OPEN','PENDING')")
            oWhere.AppendOptionalANDCondition("t.PlayerID", SQLString(psPlayerID), "=")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poTransFromDate), SafeString(poTransToDate)))

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , p.PhoneLogin as PlayerName, TeaserRuleName ,CallCenterAgents.Login,CallCenterAgents.Name as  CallCenterAgentName, tb.HomePitcher as HomePitcher_TicketBets , tb.AwayPitcher as AwayPitcher_TicketBets " & vbCrLf & _
                 "FROM Tickets t " & vbCrLf & _
                 "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                 "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                  "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                 "INNER JOIN Players p ON p.PlayerID=t.PlayerID" & vbCrLf & _
                 "INNER JOIN CallCenterAgents ON t.OrderBy=CallCenterAgents.CallCenterAgentID " & vbCrLf & _
                 "LEFT OUTER JOIN TeaserRules tr ON tr.TeaserRuleID =t.TeaserRuleID " & vbCrLf & _
                 oWhere.SQL & vbCrLf & _
                 " ORDER BY CallCenterAgents.Login desc , t.SubTicketNumber, t.TicketID, g.Gamedate DESC, t.TicketType,  p.PhoneLogin"
            log.Debug("Get the list of history tickets by cc agents. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot the list of history tickets by cc agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function


        Public Function CountTicketsByTeaserRule(ByVal poSqlTeaserRuleIDs As List(Of String)) As DataTable
            Dim odtCounts As DataTable = Nothing

            If poSqlTeaserRuleIDs.Count > 0 Then
                Dim sSQL As String = "SELECT TeaserRuleID, COUNT(TicketID) AS CountTickets FROM Tickets " & vbCrLf & _
                    "WHERE TeaserRuleID IN (" & Join(poSqlTeaserRuleIDs.ToArray, ",") & ") " & vbCrLf & _
                    "GROUP BY TeaserRuleID"

                log.Debug("Get the list of count tickets by teaser rule. SQL: " & sSQL)
                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")


                Try
                    odtCounts = odbSQL.getDataTable(sSQL)

                Catch ex As Exception
                    log.Error("Cannot get the list of count tickets by teaser rule. SQL: " & sSQL, ex)
                Finally
                    odbSQL.closeConnection()
                End Try
            End If

            Return odtCounts
        End Function

        Public Function GetBetAmountByAgent(ByVal psAgentID As String, ByVal Context As String) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("t.AgentID=" & SQLString(psAgentID))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.TicketType='Straight'")
            'oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished='N') )")
            oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")
            oWhere.AppendANDCondition("gl.Context=" & SQLString(Context))

            Dim sSQL As String = "select tb.GameID,t.BetAmount,tb.BetType,tb.HomeSpreadMoney,tb.AwaySpreadMoney,tb.TotalPointsOverMoney, " & _
                "tb.TotalPointsUnderMoney,tb.HomeMoneyLine,tb.AwayMoneyLine" & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                oWhere.SQL
            log.Debug("Get the list of SumBetAmount by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetBetAmountByListAgentID(ByVal oLstAgentsID As List(Of String), ByVal Context As String) As DataTable
            Dim odtTickets As DataTable = Nothing
            '' Check gamedate also
            Dim oDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            If oDate.IsDaylightSavingTime() Then
                oDate.AddHours(-1)
            End If
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentsID.ToArray(), "','")))
            oWhere.AppendANDCondition("isnull(t.TicketStatus, 'Open') in ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.TicketType='Straight'")
            oWhere.AppendANDCondition("ISNULL(g.IsLocked,'N')<>'Y'")
            oWhere.AppendANDCondition("ISNULL(g.IsForProp,'N')<>'Y'")
            oWhere.AppendANDCondition("gl.Context=" & SQLString(Context))

            Dim sSQL As String = "select tb.GameID, t.BetAmount, tb.BetType, tb.HomeSpreadMoney, tb.AwaySpreadMoney, tb.TotalPointsOverMoney, tb.DrawMoneyLine, " & _
                "tb.TotalPointsUnderMoney, tb.HomeMoneyLine, tb.AwayMoneyLine" & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                oWhere.SQL
            log.Debug("Get the list of tickets by agent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)

            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetPlayerBalanceAmount(ByVal psPlayerID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As Double
            Dim nWeeklyBalanceAmount As Double

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("t.PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(t.Ticketstatus,'Open') not in ('OPEN','PENDING')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poEndDate)))

            Dim sSQL As String = "Select SUM(NetAmount) as TotalNetAmount " & vbCrLf & _
                "FROM (SELECT t.TicketID, t.NetAmount FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & " GROUP BY t.TicketID, t.TicketCompletedDate, t.NetAmount " & ") AS Temp"

            log.Debug("Get Sum of NetAmount by PlayerID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                nWeeklyBalanceAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Cannot get Player's BalanceAmount. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nWeeklyBalanceAmount
        End Function

        Public Function GetPlayerAmount(ByVal psPlayerID As String, ByVal poStartDate As Date, ByVal poEndDate As Date) As Double
            Dim nWeeklyBalanceAmount As Double

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("t.PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(t.Ticketstatus,'Open') not in ('OPEN','PENDING')")
            oWhere.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poEndDate)))

            Dim sSQL As String = "Select SUM(NetAmount-RiskAmount) as TotalAmount " & vbCrLf & _
                "FROM (SELECT t.TicketID, t.NetAmount,t.RiskAmount FROM Tickets t " & vbCrLf & _
            oWhere.SQL & vbCrLf & " GROUP BY t.TicketID, t.TicketCompletedDate, t.NetAmount,t.RiskAmount " & ") AS Temp"

            log.Debug("Get Sum of Amount by PlayerID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                nWeeklyBalanceAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Cannot get Player's BalanceAmount. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nWeeklyBalanceAmount
        End Function

        Public Function GetPlayerPendingAmount(ByVal psPlayerID As String, ByVal poEndDate As Date) As Double
            Dim nWeeklyBalanceAmount As Double

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(Ticketstatus,'OPEN') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("TransactionDate", "", SafeString(poEndDate)))

            Dim sSQL As String = "Select SUM(RiskAmount) as TotalRiskAmount " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
              oWhere.SQL

            log.Debug("Get Sum of RiskAmount by PlayerID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                nWeeklyBalanceAmount = SafeDouble(odbSQL.getScalerValue(sSQL))
            Catch ex As Exception
                log.Error("Cannot get Player's PendingAmount. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nWeeklyBalanceAmount
        End Function

        Public Function GetPlayerPendingAmountInFo(ByVal psPlayerID As String, ByVal poEndDate As Date) As DataRow
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("ISNULL(Ticketstatus,'OPEN') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("TransactionDate", "", SafeString(poEndDate)))

            Dim sSQL As String = "Select SUM(RiskAmount) as TotalRiskAmount,count(TicketID) as NumPending " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
              oWhere.SQL

            log.Debug("Get Sum of RiskAmount by PlayerID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim odt As DataTable = odbSQL.getDataTable(sSQL)
                If odt IsNot Nothing AndAlso odt.Rows.Count > 0 Then
                    Return odt.Rows(0)
                End If
            Catch ex As Exception
                log.Error("Cannot get Player's PendingAmount. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetPlayerPendingIfBetAmountInFo(ByVal psPlayerID As String, ByVal poEndDate As Date) As DataRow
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("PlayerID =" & SQLString(psPlayerID))
            oWhere.AppendANDCondition("t.TicketType like 'If %'")
            oWhere.AppendANDCondition("ISNULL(Ticketstatus,'OPEN') in ('Open', 'Pending')")
            oWhere.AppendANDCondition(getSQLDateRange("TransactionDate", "", SafeString(poEndDate)))

            Dim sSQL As String = "Select SUM(RiskAmount) as TotalRiskAmount,count(TicketID) as NumPending " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
              oWhere.SQL

            log.Debug("Get Sum of RiskAmount by PlayerID. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim odt As DataTable = odbSQL.getDataTable(sSQL)
                If odt IsNot Nothing AndAlso odt.Rows.Count > 0 Then
                    Return odt.Rows(0)
                End If
            Catch ex As Exception
                log.Error("Cannot get Player's PendingAmount. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Function GetPLAmount(ByVal psUserID As String, ByVal poStartDate As Date, ByVal poEndDate As Date, Optional ByVal pbPlayer As Boolean = False) As Double
            Dim nPLAmount As Double

            '' Get SubAgents
            Dim oLstAgentIDs As New List(Of String)
            If Not pbPlayer Then
                oLstAgentIDs = (New CAgentManager).GetAllSubAgentIDs(psUserID)
            End If

            '' SQL For Total NetAmount
            Dim oWhereNet As New CSQLWhereStringBuilder
            If pbPlayer Then
                oWhereNet.AppendANDCondition("t.PlayerID = " & SQLString(psUserID))
            Else
                oWhereNet.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            End If

            oWhereNet.AppendANDCondition("ISNULL(t.Ticketstatus, 'Open') not in ('OPEN','PENDING')")
            oWhereNet.AppendANDCondition(getSQLDateRange("t.TicketCompletedDate", SafeString(poStartDate), SafeString(poEndDate)))

            Dim sSQLNet As String = "Select SUM( NetAmount - RiskAmount) as WinLoseAmount" & vbCrLf & _
                "FROM (SELECT t.TicketID, t.NetAmount, t.RiskAmount FROM Tickets t " & vbCrLf & _
            oWhereNet.SQL & vbCrLf & " GROUP BY t.TicketID, t.TicketCompletedDate, t.NetAmount, t.RiskAmount " & ") AS Temp "

            log.Debug("Get Agent's PLAmount. SQL: " & sSQLNet)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                nPLAmount = SafeDouble(odbSQL.getScalerValue(sSQLNet))
            Catch ex As Exception
                log.Error("Cannot get Agent's PLAmount. SQL: " & sSQLNet, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return nPLAmount
        End Function

        Public Function SearchTicketByAllSubAgent(ByVal oLstAgentIDs As List(Of String), ByVal pnTicket As Integer) As Data.DataTable
            Dim odtTickets As DataTable = Nothing

            'Dim oGameDate = DateTime.Now.ToUniversalTime().AddHours(-5)
            'If oGameDate.IsDaylightSavingTime() Then
            '    oGameDate.AddHours(1)
            'End If
            Dim oWhere As New CSQLWhereStringBuilder
            If pnTicket <> 0 Then
                oWhere.AppendANDCondition("t.TicketNumber =" & SafeString(pnTicket))
            End If
            oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(oLstAgentIDs.ToArray(), "','")))
            oWhere.AppendANDCondition("isnull(t.TicketStatus,'Open') in ('Open', 'Pending', 'Cancelled')")
            ' oWhere.AppendANDCondition("( (gl.Context <> '2H' and g.gameDate > " & SQLString(oGameDate) & ") or (gl.Context = '2H' and IsFirstHalfFinished<>'Y') )")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , TeaserRuleName ,p.Login as PlayerName, p.PlayerID " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID =t.PlayerID " & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID=tr.TeaserRuleID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TicketNumber , t.SubTicketNumber ,  t.TransactionDate DESC ,  t.TicketID, t.TicketType"
            log.Debug("Get the list of tickets by all subagent. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of tickets by all subagent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function SearchTicketByAgent(ByVal psAgentID As String, ByVal pnTicket As Integer, Optional ByVal psPlayerID As String = "") As Data.DataTable
            Dim odtTickets As DataTable = Nothing
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("t.AgentID =" & SQLString(psAgentID))

            If Not String.IsNullOrEmpty(psPlayerID) Then
                oWhere.AppendANDCondition("p.PlayerID = " & SQLString(psPlayerID))
            End If

            If pnTicket <> 0 Then
                oWhere.AppendANDCondition("t.TicketNumber =" & SafeString(pnTicket))
            End If
            oWhere.AppendANDCondition("isnull(t.TicketStatus,'Open') in ('Open', 'Pending', 'Cancelled')")

            Dim sSQL As String = "SELECT t.*, tb.*, g.* , TeaserRuleName ,p.Login as PlayerName, p.PlayerID " & vbCrLf & _
                "FROM Tickets t " & vbCrLf & _
                "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                "INNER JOIN GameLines gl ON tb.GameLineID=gl.GameLineID " & vbCrLf & _
                "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                "INNER JOIN Players p ON p.PlayerID =t.PlayerID " & vbCrLf & _
                "LEFT OUTER JOIN TeaserRules tr ON t.TeaserRuleID=tr.TeaserRuleID" & vbCrLf & _
                oWhere.SQL & vbCrLf & _
                " ORDER BY t.TicketNumber , t.SubTicketNumber ,  t.TransactionDate DESC ,  t.TicketID, t.TicketType"
            log.Debug("Get the list of tickets by player. SQL: " & sSQL)

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtTickets = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Cannot get the list of tickets by agent. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetDeletedTickets() As Data.DataTable
            Dim sSQL As String = "select * from DeletedTickets"
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                Dim oDT As DataTable = oDB.getDataTable(sSQL)
                Return oDT
            Catch ex As Exception
                log.Error("Can't get DeletedTickets: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
            Return Nothing
        End Function

        Public Sub CleanDeletedTickets(ByVal poDate As DateTime)
            Dim sSQL As String = "delete from DeletedTickets where DeleteTime < " & SQLString(poDate)
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Try
                oDB.executeNonQuery(sSQL)
            Catch ex As Exception
                log.Error("Can't Clean DeletedTickets: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try
        End Sub

        Public Function DeleteTicket(ByVal psTicketID As String, ByVal psPlayerID As String, ByVal poDeletedTicket As CDeletedTicket) As Boolean
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim bResult As Boolean = True
            Try
                Dim nNewAmount As Double = getNewBalanceAmountPlayer(psTicketID, oDB)

                ' Copy to DeletedTicketbets table
                Dim oInsert As New WebsiteLibrary.DBUtils.CSQLInsertStringBuilder("DeletedTickets")
                oInsert.AppendString("DeletedTicketID", "newid()")
                oInsert.AppendString("TypeOfBet", SQLString(poDeletedTicket.TypeOfBet))
                oInsert.AppendString("Player", SQLString(poDeletedTicket.Player))
                oInsert.AppendString("TicketType", SQLString(poDeletedTicket.TicketType))
                oInsert.AppendString("TicketNumber", SafeString(poDeletedTicket.TicketNumber))
                oInsert.AppendString("SubTicketNumber", SafeString(poDeletedTicket.SubTicketNumber))
                oInsert.AppendString("GameType", SQLString(poDeletedTicket.GameType))
                oInsert.AppendString("Description", SQLString(poDeletedTicket.Description))
                oInsert.AppendString("RiskAmount", SafeString(poDeletedTicket.RiskAmount))
                oInsert.AppendString("WinAmount", SafeString(poDeletedTicket.WinAmount))
                oInsert.AppendString("NetAmount", SafeString(poDeletedTicket.NetAmount))
                oInsert.AppendString("GameDate", SQLString(poDeletedTicket.GameDate))
                oInsert.AppendString("TransactionDate", SQLString(poDeletedTicket.TransactionDate))
                oInsert.AppendString("DeleteTime", "GETUTCDATE()")
                oDB.executeNonQuery(oInsert.SQL, True)

                '' Delete from ticketbets
                Dim sSQL As String = "delete from Ticketbets where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' Delete from Tickets
                sSQL = "delete from Tickets where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' Set back amount money
                sSQL = "Update Players set BalanceAmount= " & SQLString(nNewAmount) & " where PlayerID= " & SQLString(psPlayerID)
                log.Debug("Set back player ammount: " & sSQL)
                oDB.executeNonQuery(sSQL, True)

                oDB.commitTransaction()
            Catch ex As Exception
                bResult = False
                oDB.rollbackTransaction()
                log.Error("Failed to delete Ticket: " & psTicketID, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function NewDeleteTicket(ByVal psTicketID As String, ByVal psPlayerID As String) As Boolean
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim bResult As Boolean = True
            Try
                Dim nNewAmount As Double = getNewBalanceAmountPlayer(psTicketID, oDB)

                '' update ticketbets status to Deleted
                Dim sSQL As String = "UPDATE Ticketbets set TicketBetStatus = 'Deleted' where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' update Tickets status to cancelled
                sSQL = "UPDATE Tickets set TicketStatus = 'CANCELLED', TicketCompletedDate = GETUTCDATE() where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' Set back amount money
                sSQL = "Update Players set BalanceAmount= " & SQLString(nNewAmount) & " where PlayerID= " & SQLString(psPlayerID)
                log.Debug("Set back player ammount: " & sSQL)
                oDB.executeNonQuery(sSQL, True)

                oDB.commitTransaction()
            Catch ex As Exception
                bResult = False
                oDB.rollbackTransaction()
                log.Error("Failed to delete Ticket: " & psTicketID, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Public Function UnDeleteTicket(ByVal psTicketID As String, ByVal psPlayerID As String) As Boolean
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Dim bResult As Boolean = True
            Try
                Dim nNewAmount As Double = getSubtractBalanceAmountPlayer(psTicketID, oDB)

                If nNewAmount < 0 Then
                    Return False
                End If

                '' update ticketbets status to Deleted
                Dim sSQL As String = "UPDATE Ticketbets set TicketBetStatus = 'Open' where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' update Tickets status to cancelled
                sSQL = "UPDATE Tickets set TicketStatus = 'Open', TicketCompletedDate = null where TicketID= " & SQLString(psTicketID)
                oDB.executeNonQuery(sSQL, True)

                '' Set back amount money
                sSQL = "Update Players set BalanceAmount= " & SQLString(nNewAmount) & " where PlayerID= " & SQLString(psPlayerID)
                log.Debug("Set back player ammount: " & sSQL)
                oDB.executeNonQuery(sSQL, True)

                oDB.commitTransaction()
            Catch ex As Exception
                bResult = False
                oDB.rollbackTransaction()
                log.Error("Failed to delete Ticket: " & psTicketID, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bResult
        End Function

        Private Function getNewBalanceAmountPlayer(ByVal psTicketID As String, ByVal poSQL As CSQLDBUtils) As Double
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'') IN ('Open', 'Pending')")
            oWhere.AppendANDCondition("t.TicketID =  " & SQLString(psTicketID))

            Dim sSQL As String = "select p.PlayerID, (ISNULL(p.BalanceAmount,0) + ISNULL(SUM(RiskAmount),0)) as NewBalanceAmount " & vbCrLf & _
                    "FROM Players p " & vbCrLf & _
                    "INNER JOIN Tickets t on t.PlayerID=p.PlayerID " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " GROUP BY p.PlayerID, p.BalanceAmount"

            Dim oDT As DataTable = poSQL.getDataTable(sSQL)
            If oDT.Rows.Count = 0 Then
                Throw New Exception("Can't find TicketID: " & psTicketID & ". SQL: " & sSQL)
            End If
            Return SafeDouble(oDT.Rows(0)("NewBalanceAmount"))
        End Function

        Private Function getSubtractBalanceAmountPlayer(ByVal psTicketID As String, ByVal poSQL As CSQLDBUtils) As Double
            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("ISNULL(t.TicketStatus,'') = 'CANCELLED'")
            oWhere.AppendANDCondition("ISNULL(tb.TicketBetStatus,'') = 'Deleted'")
            oWhere.AppendANDCondition("t.TicketID =  " & SQLString(psTicketID))

            Dim sSQL As String = "select p.PlayerID, (ISNULL(p.BalanceAmount,0) - ISNULL(SUM(RiskAmount),0)) as NewBalanceAmount " & vbCrLf & _
                    "FROM Players p " & vbCrLf & _
                    "INNER JOIN Tickets t on t.PlayerID=p.PlayerID " & vbCrLf & _
                    "INNER JOIN Ticketbets tb on tb.TicketID= t.TicketID " & vbCrLf & _
                    oWhere.SQL & vbCrLf & _
                    " GROUP BY p.PlayerID, p.BalanceAmount"

            Dim oDT As DataTable = poSQL.getDataTable(sSQL)
            If oDT.Rows.Count = 0 Then
                Throw New Exception("Can't find TicketID: " & psTicketID & ". SQL: " & sSQL)
            End If
            Return SafeDouble(oDT.Rows(0)("NewBalanceAmount"))
        End Function

#Region "Calc BetAmount for OddsRules"
        Public Function GetRiskAmountsBySuperAdmin(ByVal plstConditions As List(Of String), ByVal psSuperAdminID As String, ByVal pbGameType As Boolean) As DataTable
            Dim odtTickets As DataTable = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""

            Try
                '' Get all Agents
                Dim olstAgents As List(Of String)
                Dim oAgentManager As New CAgentManager()
                olstAgents = oAgentManager.GetAllSubAgentIDs(psSuperAdminID, True)

                If olstAgents.Count > 0 Then
                    Dim oWhere As New CSQLWhereStringBuilder
                    oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') IN ('Open', 'Pending')")
                    oWhere.AppendANDCondition("t.TicketType = 'Straight'")
                    oWhere.AppendANDCondition("tb.Context IN ('Current', '1H', '2H')")

                    If pbGameType Then
                        oWhere.AppendANDCondition(String.Format("g.GameType IN ('{0}')", Join(plstConditions.ToArray(), "','")))
                    Else
                        oWhere.AppendANDCondition(String.Format("tb.GameID IN ('{0}')", Join(plstConditions.ToArray(), "','")))
                    End If

                    oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(olstAgents.ToArray(), "','")))

                    sSQL = "SELECT CONVERT(varchar(40),tb.GameID) as GameID, t.RiskAmount, tb.Context, tb.BetType, tb.HomeSpreadMoney, " & _
                    " tb.AwaySpreadMoney, tb.TotalPointsOverMoney, tb.TotalPointsUnderMoney, tb.HomeMoneyLine, tb.AwayMoneyLine, tb.DrawMoneyLine," & _
                        "tb.TotalPointsUnderMoney,tb.HomeMoneyLine,tb.AwayMoneyLine,tb.TeamTotalName" & vbCrLf & _
                        "FROM Tickets t " & vbCrLf & _
                        "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                        "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                        oWhere.SQL
                    log.Debug("Get the list of RiskAmount by SuperAdminID. SQL: " & sSQL)

                    odtTickets = odbSQL.getDataTable(sSQL)
                End If

            Catch ex As Exception
                log.Error("Can not get the list of RiskAmount by SuperAdminID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

        Public Function GetListTicketIDByPlayerID(ByVal plstGameIDContext As List(Of String()), ByVal psPlayerID As String, ByVal psTicketType As String) As List(Of String)
            Dim odtTickets As DataTable = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""
            Dim sGameCondition As String = " "
            Dim lstTicketID As New List(Of String)
            Try
                Dim oAgentManager As New CAgentManager()
                If plstGameIDContext.Count > 0 Then
                    Dim arrGameIDContext As String() = plstGameIDContext(0)
                    Dim oWhere As New CSQLWhereStringBuilder
                    oWhere.AppendANDCondition(String.Format("t.PlayerID = '{0}'", psPlayerID))
                    oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') IN ('Open', 'Pending')")
                    oWhere.AppendANDCondition(String.Format("t.TicketType  = '{0}'", psTicketType))
                    oWhere.AppendANDCondition(String.Format("tb.BetType  = '{0}'", arrGameIDContext(2)))
                    oWhere.AppendANDCondition(String.Format("tb.GameID= '{0}'", arrGameIDContext(0)))
                    oWhere.AppendANDCondition(String.Format("tb.Context= '{0}'", arrGameIDContext(1)))
                    sSQL = "select distinct t.TicketID from Tickets as t inner join TicketBets as tb on t.TicketID=tb.TicketID  " & vbCrLf & _
                    oWhere.SQL
                    log.Debug("Get  lstTicketID by PlayerID. SQL: " & sSQL)
                    odtTickets = odbSQL.getDataTable(sSQL)
                    For Each dr As DataRow In odtTickets.Rows
                        lstTicketID.Add(SafeString(dr("TicketID")))
                    Next
                Else
                    Return lstTicketID
                End If

            Catch ex As Exception
                log.Error("Can not get lstTicketID by PlayerID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return lstTicketID
        End Function

        Public Function GetNumTicketbetByTicketID(ByVal psTicketID As String) As Integer
            If System.Web.HttpContext.Current.Cache("psTicketID" & psTicketID) Is Nothing Then
                Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
                Dim odtTickets As DataTable = Nothing
                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition(String.Format("t.TicketID = '{0}'", psTicketID))
                Dim sSQL As String = "select count(t.TicketID) as NumTicketID from Tickets as t inner join TicketBets as tb on t.TicketID=tb.TicketID  " & vbCrLf & _
                oWhere.SQL
                log.Debug("Get Num Ticketbet By TicketID . SQL: " & sSQL)
                odtTickets = odbSQL.getDataTable(sSQL)
                If odtTickets IsNot Nothing AndAlso odtTickets.Rows.Count > 0 Then
                    'Return SafeInteger(odtTickets.Rows(0)("NumTicketID"))
                    System.Web.HttpContext.Current.Cache.Add("psTicketID" & psTicketID, SafeInteger(odtTickets.Rows(0)("NumTicketID")), Nothing, Date.Now.AddMinutes(2), Nothing, System.Web.Caching.CacheItemPriority.Default, Nothing)
                End If
            End If
            Return CType(System.Web.HttpContext.Current.Cache("psTicketID" & psTicketID), Integer)
        End Function

        Public Function GetRiskAmountsByPlayerID(ByVal plstGameIDContext As List(Of String()), ByVal psPlayerID As String, ByVal psTicketType As String) As Double
            Dim odtTickets As DataTable = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""
            Dim sGameCondition As String = " "
            Dim nRiskAmount As Double = 0
            Dim nRiskAmountTicket As Double = 0 'riskamount for each ticket
            Dim lstTicketID As New List(Of String)
            lstTicketID = GetListTicketIDByPlayerID(plstGameIDContext, psPlayerID, psTicketType)
            Try
                Dim oAgentManager As New CAgentManager()
                If lstTicketID.Count > 0 Then
                    For Each sTicketID As String In lstTicketID
                        'Dim nRiskAmountTicket As Double = 0
                        'nRiskAmountTicket = nRiskAmount


                        If GetNumTicketbetByTicketID(sTicketID) <> plstGameIDContext.Count Then
                            Continue For
                        End If
                        For Each arrGameIDContext As String() In plstGameIDContext
                            nRiskAmountTicket = 0
                            Dim oWhere As New CSQLWhereStringBuilder
                            oWhere.AppendANDCondition(String.Format("t.PlayerID = '{0}'", psPlayerID))
                            oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') IN ('Open', 'Pending')")
                            oWhere.AppendANDCondition(String.Format("t.TicketID  = '{0}'", sTicketID))
                            oWhere.AppendANDCondition(String.Format("tb.BetType  = '{0}'", arrGameIDContext(2)))
                            oWhere.AppendANDCondition(String.Format("t.TicketType  = '{0}'", psTicketType))
                            oWhere.AppendANDCondition(String.Format("tb.GameID= '{0}'", arrGameIDContext(0)))
                            oWhere.AppendANDCondition(String.Format("tb.Context= '{0}'", arrGameIDContext(1)))
                            sSQL = "select sum(RiskAmount) as  RiskAmount from Tickets as t inner join TicketBets as tb on t.TicketID=tb.TicketID  " & vbCrLf & _
                            oWhere.SQL
                            log.Debug("Get  RiskAmount by PlayerID. SQL: " & sSQL)
                            odtTickets = odbSQL.getDataTable(sSQL)
                            If SafeDouble(odtTickets.Rows(0)("RiskAmount")) = 0 Then
                                'nRiskAmount = nRiskAmountTicket
                                nRiskAmountTicket = 0
                                Exit For
                            Else
                                'nRiskAmount += SafeDouble(odtTickets.Rows(0)("RiskAmount"))
                                nRiskAmountTicket = SafeDouble(odtTickets.Rows(0)("RiskAmount"))
                            End If
                        Next
                        nRiskAmount += nRiskAmountTicket
                    Next


                Else
                    Return 0
                End If

            Catch ex As Exception
                log.Error("Can not get RiskAmount by PlayerID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try
            Return nRiskAmount
        End Function

        Public Function GetRiskAmountsBySuperAgent(ByVal plstConditions As List(Of String), ByVal psSuperAgentID As String, ByVal pbGameType As Boolean) As DataTable
            Dim odtTickets As DataTable = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""

            Try
                '' Get all Agents
                Dim olstAgents As List(Of String)
                Dim oAgentManager As New CAgentManager()
                olstAgents = oAgentManager.GetAllSubAgentIDs(psSuperAgentID, False)

                If olstAgents.Count > 0 Then
                    Dim oWhere As New CSQLWhereStringBuilder
                    oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') IN ('Open', 'Pending')")
                    oWhere.AppendANDCondition("t.TicketType = 'Straight'")
                    oWhere.AppendANDCondition("tb.Context IN ('Current', '1H', '2H')")

                    If pbGameType Then
                        oWhere.AppendANDCondition(String.Format("g.GameType IN ('{0}')", Join(plstConditions.ToArray(), "','")))
                    Else
                        oWhere.AppendANDCondition(String.Format("tb.GameID IN ('{0}')", Join(plstConditions.ToArray(), "','")))
                    End If

                    oWhere.AppendANDCondition(String.Format("t.AgentID IN ('{0}')", Join(olstAgents.ToArray(), "','")))

                    sSQL = "SELECT CONVERT(varchar(40),tb.GameID) as GameID, t.RiskAmount, tb.Context, tb.BetType, tb.HomeSpreadMoney, " & _
                    " tb.AwaySpreadMoney, tb.TotalPointsOverMoney, tb.TotalPointsUnderMoney, tb.HomeMoneyLine, tb.AwayMoneyLine, tb.DrawMoneyLine," & _
                        "tb.TotalPointsUnderMoney,tb.HomeMoneyLine,tb.AwayMoneyLine,tb.TeamTotalName" & vbCrLf & _
                        "FROM Tickets t " & vbCrLf & _
                        "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                        "INNER JOIN Games g ON g.GameID=tb.GameID " & vbCrLf & _
                        oWhere.SQL
                    log.Debug("Get the list of RiskAmount by SuperAdminID. SQL: " & sSQL)

                    odtTickets = odbSQL.getDataTable(sSQL)
                End If

            Catch ex As Exception
                log.Error("Can not get the list of RiskAmount by SuperAgentID. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function

#End Region

#Region "Calc WinAmount for Wager Win Limit Per Game (Player Template)"
        Public Function GetWinAmountByPlayer(ByVal plstGameIDs As List(Of String), ByVal psPlayerID As String) As DataTable
            Dim odtTickets As DataTable = Nothing
            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")
            Dim sSQL As String = ""

            Try
                '' List of GameIDs must have at least 1 Unique Key
                If plstGameIDs Is Nothing Then
                    plstGameIDs = New List(Of String)
                End If
                If plstGameIDs.Count = 0 Then
                    plstGameIDs.Add(newGUID())
                End If

                Dim oWhere As New CSQLWhereStringBuilder
                oWhere.AppendANDCondition("ISNULL(t.TicketStatus, 'Open') IN ('Open', 'Pending')")
                oWhere.AppendANDCondition("t.TicketType = 'Straight'")
                'oWhere.AppendANDCondition("tb.Context IN ('Current', '1H', '2H')")
                oWhere.AppendANDCondition("t.PlayerID =" & SQLString(psPlayerID))
                oWhere.AppendANDCondition(String.Format("tb.GameID IN ('{0}')", Join(plstGameIDs.ToArray(), "','")))

                sSQL = "SELECT GameLineID,PropMoneyLine,CONVERT(varchar(40),tb.GameID) as GameID,t.RiskAmount,t.WinAmount, tb.Context, tb.BetType, tb.HomeSpreadMoney, " & _
                " tb.AwaySpreadMoney, tb.TotalPointsOverMoney, tb.TotalPointsUnderMoney, tb.HomeMoneyLine, tb.AwayMoneyLine, tb.DrawMoneyLine," & _
                    "tb.TotalPointsUnderMoney,tb.HomeMoneyLine,tb.AwayMoneyLine,TeamTotalName" & vbCrLf & _
                    "FROM Tickets t " & vbCrLf & _
                    "INNER JOIN TicketBets tb ON tb.TicketID=t.TicketID " & vbCrLf & _
                    oWhere.SQL
                log.Debug("Get the list of RiskAmount by Player. SQL: " & sSQL)

                odtTickets = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error("Can not get the list of RiskAmount by Player. SQL: " & sSQL, ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return odtTickets
        End Function
#End Region

#Region "Asteria"
        Public Function AssignRecording(ByVal psTicketID As String, ByVal psAsteriskID As String, _
                                        ByVal poCallDate As DateTime, ByVal psCallerID As String, _
                                        ByVal psRecordingFile As String, ByVal pnCallDuration As Integer, _
                                        ByVal psAssignBy As String) As Boolean
            Dim oDB As New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
            Dim sSQL As String = ""
            Dim bResult As Boolean = False
            Try
                Dim oWhere As New CSQLWhereStringBuilder()
                oWhere.AppendANDCondition("TicketID = " & SQLString(psTicketID))

                Dim oUpdate As New CSQLUpdateStringBuilder("Tickets", oWhere.SQL)
                oUpdate.AppendString("AsteriskID", SQLString(psAsteriskID))
                oUpdate.AppendString("CallTimestamp", SQLDate(poCallDate))
                oUpdate.AppendString("CallerID", SQLString(psCallerID))
                oUpdate.AppendString("CallDuration", SQLString(pnCallDuration))
                oUpdate.AppendString("RecordingFile", SQLString(psRecordingFile))

                sSQL = oUpdate.SQL
                log.Debug("Assign Recording: " & sSQL)
                oDB.executeNonQuery(oUpdate, psAssignBy)

                bResult = True
            Catch ex As Exception
                log.Error("Failed to Assign Recording. SQL :" & sSQL, ex)
            Finally
                oDB.closeConnection()
            End Try

            Return bResult
        End Function
#End Region

        Public Enum EAmoutCompare
            None = 0
            GreaterThan = 1
            LowerThan = 2
            Equal = 3
        End Enum
    End Class
End Namespace

