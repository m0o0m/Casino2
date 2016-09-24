Imports System.Runtime.InteropServices
Imports SBCBL.std
Imports SBCBL.CEnums
Imports SBCBL.CacheUtils
Imports WebsiteLibrary.DBUtils

Namespace Tickets
    <Serializable()> _
    Public Class CSelectedTickets
        Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CSelectedTickets))

        Private _olstTickets As CTicketList
        Private _sInvalidTickets As String
        Private _eTypeOfBet As ETypeOfBet
        Private _bCalcRiskAmount As Boolean = True
        Private _bPreview As Boolean

        Public Sub New(ByVal peTypeOfBet As ETypeOfBet)
            _eTypeOfBet = peTypeOfBet
            _olstTickets = New CTicketList()
        End Sub

#Region "Properties"

        Public Property CalcRiskAmount() As Boolean
            Get
                Return _bCalcRiskAmount
            End Get
            Set(ByVal value As Boolean)
                _bCalcRiskAmount = value
            End Set
        End Property

        Public Property Preview() As Boolean
            Get
                Return _bPreview
            End Get
            Set(ByVal value As Boolean)
                _bPreview = value
            End Set
        End Property

        Public ReadOnly Property TypeOfBet() As ETypeOfBet
            Get
                Return _eTypeOfBet
            End Get
        End Property

        Public ReadOnly Property Tickets() As CTicket()
            Get
                Return _olstTickets.ToArray()
            End Get
        End Property

        Public ReadOnly Property InvalidTickets() As String
            Get
                Return SafeString(_sInvalidTickets)
            End Get
        End Property

        Public ReadOnly Property LastTicket() As CTicket
            Get
                If _olstTickets.Count > 0 Then
                    Return _olstTickets.Last
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property HTML() As String
            Get
                Dim sHTML As String = ""

                If Me.NumOfTickets > 0 Then
                    Dim nIndex As Integer = 0

                    For Each oTicket As CTicket In Me._olstTickets
                        If Me.Preview Then
                            sHTML &= String.Format("<tr class='{0}'><td>", SafeString(IIf(nIndex Mod 2 = 0, "gametable_odd", "gametable_even")))
                            sHTML &= oTicket.PreviewHTML() & "</td></tr>"

                        Else
                            If CalcRiskAmount Then
                                oTicket.CalcRiskAmountByWinAmount()
                            End If
                            sHTML &= String.Format("<div id='div{0}' style='width: 100%;'>{1}</div>", oTicket.TicketID, oTicket.HTML())
                        End If

                        nIndex += 1
                    Next

                    If Me.Preview Then
                        sHTML = "<table Width='98%' CellPadding='3' CellSpacing='2' CssClass='gamebox'>" & _
                        "<tr><td class='tableheading2' style='text-algin: center;'> Trans. Detail </td></tr>" & sHTML & "</table>"
                    End If
                End If

                Return sHTML
            End Get
        End Property
#End Region

#Region "Methods"
        Public Sub NewTicket(ByVal psSuperAgentID As String, ByVal psPlayerID As String)
            Dim oTicket As CTicket
            oTicket = New CTicket("", psSuperAgentID, psPlayerID)

            _olstTickets.Add(oTicket)
        End Sub

        ''' <summary>
        ''' Validate All Wagers
        ''' </summary>
        ''' <param name="plstOddsRules"></param>
        ''' <param name="psSuperAdminID"></param>
        ''' <param name="psPlayerID"></param>
        ''' <param name="poPlayerTemplate"></param>
        ''' <param name="pnRemainAmount"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Validate(ByVal psSuperAdminID As String, _
                                 ByVal psPlayerID As String, ByVal poPlayerTemplate As CPlayerTemplate, _
                                 ByVal pnRemainAmount As Double, ByVal pbSuperPlayer As Boolean, Optional ByVal pnIncreaseSpread As Integer = 0, Optional ByVal psSuperAgentID As String = "") As ECheckStatus
            _sInvalidTickets = ""
            _log.Debug("BEGIN check valid Tickets1")
            Dim bResult As Boolean = True
            Dim nAmount As Double = pnRemainAmount
            Dim oTblPlayerBet As DataTable = Nothing

            '' Create OddsRulesEngine
            Dim oOddsRulesEngine As New COddRulesEngine(getGameIDs(), psSuperAdminID, False, poPlayerTemplate, psSuperAgentID, psPlayerID)

            '' Get All Player's Betting
            Dim oTicketManager As New Managers.CTicketManager()
            oTblPlayerBet = oTicketManager.GetWinAmountByPlayer(getGameIDs(), psPlayerID)

            _log.Debug("BEGIN check valid Tickets")
            Dim oResult As ECheckStatus = ECheckStatus.Success
            Dim nIndex As Integer = 1
            For Each oTicket As CTicket In _olstTickets
                '' UnCheck empty ticket
                If oTicket.NumOfTicketBets = 0 Then
                    Continue For
                End If

                If oTicket.RiskAmount = 0 Then
                    _sInvalidTickets &= oTicket.TicketID & ";"
                    Throw New CTicketException("The Bet Amount has to be different than 0")
                End If
                Dim oStatus As ECheckStatus = oTicket.Validate(oOddsRulesEngine, oTblPlayerBet, poPlayerTemplate, nAmount, TypeOfBet, pbSuperPlayer, pnIncreaseSpread)
                LogDebug(_log, "oTicket.Validate")
                If oTicket.IsCircle AndAlso (UCase(oTicket.TicketType) = "PARLAY" Or UCase(oTicket.TicketType) = "REVERSE") Then
                    'Dim oGameTypeOnOffManager As New SBCBL.Managers.CGameTypeOnOffManager()
                    'Dim odtCircleGameValue As DataTable = oGameTypeOnOffManager.GetCircleGameValues(psSuperAgentID)
                    Dim nCircleBet As Double = 0
                    Dim oCacheManager As New CCacheManager()
                    If Not String.IsNullOrEmpty(psSuperAgentID) AndAlso oCacheManager.GetSysSettings(psSuperAgentID & "CircleSettings").GetIntegerValue("ParlayReverse") > 0 Then
                        nCircleBet = SafeDouble(oCacheManager.GetSysSettings(psSuperAgentID & "CircleSettings").GetIntegerValue("ParlayReverse"))
                        If oTicket.BetAmount > nCircleBet Then
                            Throw New CTicketException("You are exceeding the maximum allowed for this Circled game.")
                        End If
                    Else
                        Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
                        Dim sCategory = SBCBL.std.GetSiteType & " MaxCircled"
                        Dim oListSettings As CSysSettingList = oSysManager.GetAllSysSettings(sCategory)
                        Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = "ParlayReverse")

                        '' Dont allowd betting if this bet ammount is over the Max Circled Amount Setting
                        If oSetting IsNot Nothing AndAlso SafeDouble(oSetting.Value) > 0 Then
                            If oTicket.BetAmount > SafeDouble(oSetting.Value) Then
                                Throw New CTicketException("You are exceeding the maximum allowed for this Circled game.")
                            End If
                        End If
                    End If

                ElseIf oTicket.IsCircle AndAlso (UCase(oTicket.TicketType) = "STRAIGHT" OrElse UCase(oTicket.TicketType) = "IF BET") Then
                    Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
                    Dim sCategory = SBCBL.std.GetSiteType & " MaxCircled"
                    Dim oListSettings As CSysSettingList = oSysManager.GetAllSysSettings(sCategory)
                    Dim oSetting As CSysSetting = oListSettings.Find(Function(x) x.Key = "Straight")

                    '' Dont allowd betting if this bet ammount is over the Max Circled Amount Setting
                    If oSetting IsNot Nothing AndAlso SafeDouble(oSetting.Value) > 0 Then
                        If oTicket.BetAmount > SafeDouble(oSetting.Value) Then
                            Throw New CTicketException("You are exceeding the maximum allowed for this Circled game.")
                        End If
                    End If
                End If

                '' Validate for If bet
                If UCase(oTicket.TicketType) = "IF BET" Then
                    If oTicket.RelatedTicketID = "" Then
                        Throw New CTicketException("2 Selections are required for If Bet.")
                    End If
                End If

                If oResult < oStatus Then
                    oResult = oStatus
                End If

                nAmount -= oTicket.RiskAmount
                nIndex += 1
            Next

            _log.Debug("END check valid Tickets")
            Return oResult
        End Function

        ''' <summary>
        ''' Add Wager
        ''' </summary>
        ''' <param name="poTicket"></param>
        ''' <remarks></remarks>
        Public Function AddTicket(ByVal poTicket As CTicket) As String
            _log.Debug("START Add Ticket.")
            If Not Preview AndAlso poTicket IsNot Nothing AndAlso poTicket.TicketBets.Count > 0 Then
                If (UCase(poTicket.TicketType) = "STRAIGHT" OrElse UCase(poTicket.TicketType) = "IF BET") Then
                    Dim oTicketBet, oCurrTicketBet As CTicketBet
                    oTicketBet = poTicket.TicketBets(0)

                    For Each oCurrTicket As CTicket In _olstTickets
                        '' Don't allow user bet twice in same game for straight wager
                        If oCurrTicket.TicketBets.Count > 0 AndAlso UCase(oCurrTicket.TicketType) = UCase(poTicket.TicketType) Then
                            oCurrTicketBet = oCurrTicket.TicketBets(0)

                            If oTicketBet.GameID = oCurrTicketBet.GameID AndAlso _
                            oTicketBet.Team = oCurrTicketBet.Team AndAlso _
                            oTicketBet.TeamNumber = oCurrTicketBet.TeamNumber AndAlso _
                            oTicketBet.BetType = oCurrTicketBet.BetType AndAlso _
                            oTicketBet.Context = oCurrTicketBet.Context Then
                                If (UCase(oTicketBet.BetType) = "TOTALPOINTS" Or UCase(oTicketBet.BetType) = "TEAMTOTALPOINTS") Then
                                    If oTicketBet.TotalPointsOverMoney <> 0 Then
                                        If oCurrTicketBet.TotalPointsOverMoney <> 0 Then
                                            _log.Debug("Add Ticket unsuccessfully.")
                                            Return "This Bet has been selected."
                                        End If
                                    Else
                                        If oCurrTicketBet.TotalPointsUnderMoney <> 0 Then
                                            _log.Debug("Add Ticket unsuccessfully.")
                                            Return "This Bet has been selected."
                                        End If
                                    End If

                                Else
                                    _log.Debug("Add Ticket unsuccessfully.")
                                    Return "This Bet has been selected."
                                End If
                            End If
                        End If
                    Next
                End If

                _log.Debug("Add Ticket successfully.")
                If LastTicket IsNot Nothing AndAlso UCase(poTicket.TicketType) = "IF BET" AndAlso _
                LastTicket.TicketOption = poTicket.TicketOption Then
                    If Me.LastTicket.RelatedTicketID = "" Then
                        LastTicket.RelatedTicketID = LastTicket.TicketID
                        poTicket.RelatedTicketID = LastTicket.TicketID
                    Else
                        poTicket.RelatedTicketID = LastTicket.RelatedTicketID
                    End If
                End If
                _olstTickets.Add(poTicket)
            Else
                _log.Debug("Add Ticket unsuccessfully.")
            End If

            _log.Debug("END Add Ticket.")

            Return ""
        End Function

        ''' <summary>
        ''' Get Wager by ID
        ''' </summary>
        ''' <param name="psTicketID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTicket(ByVal psTicketID As String) As CTicket
            Return _olstTickets.GetTicketByID(psTicketID)
        End Function

        ''' <summary>
        ''' Remove Wagers
        ''' </summary>
        ''' <param name="poTicketIDs"></param>
        ''' <remarks></remarks>
        Public Sub RemoveTickets(ByVal poTicketIDs As String)
            For Each sTicketID As String In poTicketIDs.Split(","c)
                _log.Debug("Remove Ticket. TicketID: " & sTicketID)
                _olstTickets.RemoveByID(sTicketID)
            Next
        End Sub

        Public Function NumOfTickets() As Integer
            Return _olstTickets.Count
        End Function

        ''' <summary>
        ''' Get all selected GameID
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function getGameIDs() As List(Of String)
            Dim lstGameIDs As New List(Of String)
            lstGameIDs.Add(newGUID())

            For Each oTicket As CTicket In _olstTickets
                For Each oTicketBet As CTicketBet In oTicket.TicketBets
                    lstGameIDs.Add(oTicketBet.GameID)
                Next
            Next

            Return lstGameIDs
        End Function

        ''' <summary>
        ''' Save Wagers to DB
        ''' </summary>
        ''' <param name="pnRemainAmount"></param>
        ''' <param name="psAgentID"></param>
        ''' <param name="psPlayerID"></param>
        ''' <param name="psOrderBy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveTickets(ByVal pnRemainAmount As Double, ByVal psAgentID As String, _
                                    ByVal psPlayerID As String, ByVal psOrderBy As String,Optional ByRef  poListCTicket As List(Of CTicket) = Nothing) As Boolean

            _log.Debug("BEGIN SaveTickets")
            Dim bResult As Boolean = False

            Dim oDB As CSQLDBUtils = Nothing
            Try
                oDB = New CSQLDBUtils(SBC_CONNECTION_STRING, SBC2_CONNECTION_STRING)
                Dim oGenTickets As New CTicketList
                Dim oParlayTickets As New CTicketList
                Dim nCurrTicketNumber As Integer
                Dim oPrevTicket As CTicket = Nothing
                Dim nIndex As Integer = 1
                poListCTicket = new List(Of CTicket)

                For Each oTicket As CTicket In _olstTickets
                    '' UnCheck empty ticket
                    If oTicket.NumOfTicketBets = 0 Then
                        Continue For
                    End If
                    _log.Debug("BEGIN Insert Ticket. TicketID: " & oTicket.TicketID)

                    If Not (oPrevTicket IsNot Nothing AndAlso UCase(oTicket.TicketType) = "IF BET" AndAlso oPrevTicket.RelatedTicketID <> "") Then
                        '' Auto generate unique TicketNumber here
                        nCurrTicketNumber = NextTicketNumber(oDB)
                        LogDebug(_log, "TicketNumber: " & nCurrTicketNumber)
                        nIndex = 1
                    Else
                        nIndex += 1
                    End If
                    
                    If UCase(oTicket.TicketType) = "PARLAY" AndAlso UCase(oTicket.TicketOption) <> "PARLAY" Then
                        _log.Debug("Generate Round Robin Parlay Tickets")
                        Dim olstGenTickets As CTicketList = oTicket.GenerateParlay()
                        '' This parlay ticket will be replace by a list of round robin
                        oParlayTickets.Add(oTicket)
                        oGenTickets.AddRange(olstGenTickets)
                        nIndex = 1
                        For Each oGenTicket As CTicket In olstGenTickets
                            SaveTicket(oDB, oGenTicket, psAgentID, psPlayerID, psOrderBy, nCurrTicketNumber, nIndex)
                            oTicket.TicketNumber = nCurrTicketNumber
                            oTicket.SubTicketNumber = nIndex
                            poListCTicket.Add(oTicket)

                            nIndex += 1
                            
                        Next
                    Else
                        SaveTicket(oDB, oTicket, psAgentID, psPlayerID, psOrderBy, nCurrTicketNumber, nIndex)
                        oTicket.TicketNumber = nCurrTicketNumber
                        oTicket.SubTicketNumber = nIndex
                        poListCTicket.Add(oTicket)

                    End If

                    oPrevTicket = oTicket
                Next

                '' renew the Tickets property
                '_olstTickets.AddRange(oGenTickets)
                'For Each oTicket As CTicket In oParlayTickets
                '    _olstTickets.RemoveByID(oTicket.TicketID)
                'Next

                '' Update User's BalanceAmount
                Dim oUpdate As New CSQLUpdateStringBuilder("Players", String.Format("WHERE PlayerID={0}", SQLString(psPlayerID)))
                oUpdate.AppendString("BalanceAmount", SafeString(pnRemainAmount - _olstTickets.TotalAmount))
                oDB.executeNonQuery(oUpdate, psOrderBy, True)

                oDB.commitTransaction()
                bResult = True

            Catch ex As Exception
                bResult = False
                _log.Error("Fails to save Tickets. Message: " & ex.Message, ex)
            Finally
                If oDB IsNot Nothing Then oDB.closeConnection()
            End Try

            _log.Debug("END SaveTickets")
            Return bResult
        End Function

        Private Sub SaveTicket(ByVal poDB As CSQLDBUtils, ByVal poTicket As CTicket, _
                               ByVal psAgentID As String, ByVal poPlayerID As String, _
                               ByVal poOrderBy As String, ByVal pnTicketNumber As Integer, _
                               Optional ByVal pnSubTicketNumber As Integer = 1)
            _log.Debug("BEGIN Insert Ticket. TicketID: " & poTicket.TicketID)

            ''Save Tickets
            Dim oInsert As CSQLInsertStringBuilder

            '' Save TicketBets
            For Each oTicketBet As CTicketBet In poTicket.TicketBets
                oInsert = New CSQLInsertStringBuilder("TicketBets")
                oInsert.AppendString("TicketBetID", SQLString(oTicketBet.TicketBetID))
                oInsert.AppendString("TicketID", SQLString(poTicket.TicketID))
                oInsert.AppendString("BetType", SQLString(oTicketBet.BetType))
                oInsert.AppendString("HomeSpread", SQLString(oTicketBet.HomeSpread))
                oInsert.AppendString("HomeSpreadMoney", SQLString(oTicketBet.HomeSpreadMoney))
                oInsert.AppendString("AwaySpread", SQLString(oTicketBet.AwaySpread))
                oInsert.AppendString("AwaySpreadMoney", SQLString(oTicketBet.AwaySpreadMoney))
                oInsert.AppendString("HomeMoneyLine", SQLString(oTicketBet.HomeMoneyLine))
                oInsert.AppendString("AwayMoneyLine", SQLString(oTicketBet.AwayMoneyLine))
                oInsert.AppendString("TotalPoints", SQLString(oTicketBet.TotalPoints))
                oInsert.AppendString("TotalPointsOverMoney", SQLString(oTicketBet.TotalPointsOverMoney))
                oInsert.AppendString("TotalPointsUnderMoney", SQLString(oTicketBet.TotalPointsUnderMoney))
                oInsert.AppendString("GameID", SQLString(oTicketBet.GameID))
                oInsert.AppendString("GameLineID", SQLString(oTicketBet.GameLineID))
                oInsert.AppendString("AddPoint", SQLString(oTicketBet.AddPoint))
                oInsert.AppendString("AddPointMoney", SQLString(oTicketBet.AddPointMoney))
                oInsert.AppendString("HomePitcher", SQLString(oTicketBet.HomePitcher))
                oInsert.AppendString("AwayPitcher", SQLString(oTicketBet.AwayPitcher))
                oInsert.AppendString("DrawMoneyLine", SQLString(oTicketBet.DrawMoneyLine))
                oInsert.AppendString("Context", SQLString(oTicketBet.Context))
                oInsert.AppendString("PropParticipantName", SQLString(oTicketBet.PropParticipantName))
                oInsert.AppendString("PropRotationNumber", SQLString(oTicketBet.PropRotationNumber))
                oInsert.AppendString("PropMoneyLine", SQLString(oTicketBet.PropMoneyLine))
                oInsert.AppendString("TeamTotalName", SQLString(oTicketBet.TeamTotalName))
                oInsert.AppendString("TicketBetStatus", SQLString("Open"))
                oInsert.AppendString("Description", SQLString(oTicketBet.Description))
                oInsert.AppendString("IsCheckPitcher", SQLString(IIf(oTicketBet.IsCheckPitcher, "Y", "N")))

                _log.Debug("Insert Bet. TickBetID: " & oTicketBet.TicketBetID)
                poDB.executeNonQuery(oInsert, poOrderBy, True)
            Next

            oInsert = New CSQLInsertStringBuilder("Tickets")
            oInsert.AppendString("TicketID", SQLString(poTicket.TicketID))
            oInsert.AppendString("RelatedTicketID", SQLString(poTicket.RelatedTicketID))
            oInsert.AppendString("TicketNumber", SQLString(pnTicketNumber))
            oInsert.AppendString("SubTicketNumber", SQLString(pnSubTicketNumber))

            oInsert.AppendString("AgentID", SQLString(psAgentID))
            oInsert.AppendString("PlayerID", SQLString(poPlayerID))
            oInsert.AppendString("TransactionDate", SQLDate(GetEasternDate))
            oInsert.AppendString("OrderBy", SQLString(poOrderBy))
            Select Case UCase(poTicket.TicketType)
                Case "REVERSE", "IF BET"
                    oInsert.AppendString("TicketType", SQLString(poTicket.TicketOption))
                Case "TEASER"
                    oInsert.AppendString("TicketType", SQLString(poTicket.TicketType))
                    oInsert.AppendString("TeaserRuleID", SQLString(poTicket.TicketOption))
                Case Else
                    oInsert.AppendString("TicketType", SQLString(poTicket.TicketType))
            End Select

            oInsert.AppendString("RiskAmount", SQLString(SafeRound(poTicket.RiskAmount)))
            oInsert.AppendString("WinAmount", SQLString(SafeRound(poTicket.WinAmount)))
            oInsert.AppendString("BetAmount", SQLString(SafeRound(poTicket.BetAmount)))
            oInsert.AppendString("TicketStatus", SQLString("Open"))
            oInsert.AppendString("PendingStatus", SQLString("Open"))

            Select Case TypeOfBet
                Case ETypeOfBet.Internet
                    oInsert.AppendString("TypeOfBet", SQLString("Internet"))
                Case ETypeOfBet.Phone
                    oInsert.AppendString("TypeOfBet", SQLString("Phone"))
            End Select

            oInsert.AppendString("NumOfBets", SQLString(poTicket.NumOfTicketBets))

            oInsert.AppendString("IsForProp", SQLString(IIf(poTicket.IsForProp, "Y", "N")))

            poDB.executeNonQuery(oInsert, poOrderBy, True)
            _log.Debug("END Insert Ticket. TicketID: " & poTicket.TicketID)
        End Sub

        Private Function NextTicketNumber(ByVal poDB As CSQLDBUtils) As Integer
            Dim nCurrentTicketNumber As Integer = 0
            Dim sSQL As String = "SELECT TOP 1 TicketNumber FROM Tickets ORDER BY TicketNumber DESC"
            nCurrentTicketNumber = SafeInteger(poDB.getScalerValue(sSQL)) + 1

            Return nCurrentTicketNumber
        End Function

#End Region

    End Class

    Public Enum ECheckStatus
        Success
        Update
    End Enum

    Public Class CTicketException
        Inherits System.Exception

        Public Sub New(ByVal psMessage As String)
            MyBase.New(psMessage)
        End Sub
    End Class
End Namespace