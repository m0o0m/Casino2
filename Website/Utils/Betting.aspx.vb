Imports SBCBL
Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.Tickets
Imports SBCBL.UI
Imports SBCBL.CacheUtils

Partial Class Betting
    Inherits SBCBL.UI.CSBCPage
    Private Shared _log As log4net.ILog = log4net.LogManager.GetLogger(GetType(Betting))

    Private Shared Function UserType() As EUserType
        If HttpContext.Current.Session("USER_TYPE") Is Nothing Then
            HttpContext.Current.Session("USER_TYPE") = EUserType.Player
        End If
        Return CType(HttpContext.Current.Session("USER_TYPE"), EUserType)
    End Function

    Private Shared Function UserID() As String
        Return SafeString(HttpContext.Current.Session("USER_ID"))
    End Function

    Public Shared Function SelectedTicket(ByVal psPlayerID As String) As Tickets.CSelectedTickets
        Dim sKey As String = String.Format("{0}_SELECTED_TICKETS", psPlayerID)
        If HttpContext.Current.Session(sKey) Is Nothing Then
            If UserType() = EUserType.CallCenterAgent Then
                HttpContext.Current.Session(sKey) = New Tickets.CSelectedTickets(ETypeOfBet.Phone)
            Else
                HttpContext.Current.Session(sKey) = New Tickets.CSelectedTickets(ETypeOfBet.Internet)
            End If
        End If

        Return CType(HttpContext.Current.Session(sKey), Tickets.CSelectedTickets)
    End Function

    Private Shared Function CalcRate(ByVal pnRate As Double) As Double
        If pnRate < 0 Then
            pnRate = (-pnRate + 100) / -pnRate
        ElseIf pnRate > 0 Then
            pnRate = (pnRate + 100) / 100
        Else
            pnRate = 0
        End If

        Return pnRate
    End Function

    Private Shared Sub LogCCAgentBet(ByVal psPlayerID As String)
        Dim oCache As New CacheUtils.CCacheManager()
        Dim sCCAgentName As String = oCache.GetCallCenterAgentInfo(UserID).Name

        For Each oTicket As CTicket In SelectedTicket(psPlayerID).Tickets
            Dim sNote As String = BuidActivityNote(oTicket, oCache)
            Dim oNote As New SBCBL.CacheUtils.CActivityNote()
            oNote.CreateDate = Now.ToUniversalTime()
            oNote.FullName = sCCAgentName
            oNote.TicketID = oTicket.TicketID
            oNote.UserID = New Guid(UserID)
            oNote.Note = sNote
            oNote.NoteType = "CCAgent"
            Dim oNoteManager As New SBCBL.Managers.CActivityNotesManager()
            oNoteManager.AddNote(oNote)
        Next
    End Sub

    Private Shared Function BuidActivityNote(ByVal poTicket As CTicket, ByVal poCache As CacheUtils.CCacheManager) As String
        Dim sResult As String = "Wager type: "

        Select Case UCase(poTicket.TicketType)
            Case "PARLAY"
                sResult &= poTicket.TicketOptionText

            Case "REVERSE", "IF BET"
                sResult &= poTicket.TicketOption

            Case "TEASER"
                sResult &= poCache.GetTeaserRuleInfo(poTicket.TicketOption).TeaserRuleName

            Case Else
                sResult &= poTicket.TicketType

        End Select

        sResult &= vbLf
        Dim oDetails As New List(Of String)
        Dim nIndex As Integer = 1

        For Each oTicketBet As CTicketBet In poTicket.TicketBets

            Dim sDetail As String = String.Format("Select #{0}:  {1} " & vbLf, nIndex, oTicketBet.GameType)
            Select Case UCase(oTicketBet.BetType)
                Case "MONEYLINE"
                    sDetail &= oTicketBet.GetDetailByMoneyLine()

                Case "SPREAD"
                    sDetail &= oTicketBet.GetDetailBySpread()

                Case "TOTALPOINTS"
                    sDetail &= oTicketBet.GetDetailByTotalPoints()

                Case "DRAW"
                    sDetail &= oTicketBet.GetDetailByDraw()
            End Select

            oDetails.Add(sDetail & vbLf)
            nIndex += 1
        Next

        sResult &= Join(oDetails.ToArray) & String.Format("Amount: Risking <b>{0}</b> To Win <b>{1}</b>", _
                                                                             FormatNumber(SafeRound(poTicket.RiskAmount), GetRoundMidPoint), _
                                                                             FormatNumber(SafeRound(poTicket.WinAmount), GetRoundMidPoint))

        Return sResult
    End Function

    Private Shared Function CanBetting(ByVal psPlayerID As String) As Boolean
        Dim UserSession As New CSBCSession()
        If psPlayerID <> "" Then
            Return (UserSession.SysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable") OrElse _
            UserSession.SysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable")) AndAlso _
            Not UserSession.Cache.GetPlayerInfo(psPlayerID).IsBettingLocked AndAlso _
            UserSession.Cache.GetPlayerInfo(psPlayerID).OriginalAmount > 0
        Else
            Return (UserSession.SysSettings("BettingSetup", "BettingEnable").GetBooleanValue("BettingEnable") OrElse _
            UserSession.SysSettings("BettingSetup", "OverrideBettingEnable").GetBooleanValue("OverrideBettingEnable"))
        End If

    End Function

    Private Shared Function CheckPassword(ByVal poPlayer As CacheUtils.CPlayer, ByVal psPassword As String) As Boolean
        If SafeString(psPassword) = "" Then
            Return False
        End If

        If poPlayer IsNot Nothing Then
            Select Case UserType()
                Case SBCBL.EUserType.CallCenterAgent
                    Return poPlayer.CheckPhonePasswowrd(psPassword)

                Case SBCBL.EUserType.Agent
                    Dim oCache As New CacheUtils.CCacheManager()
                    Return oCache.GetAgentInfo(UserID).CheckPassword(psPassword)

                Case SBCBL.EUserType.Player
                    Return poPlayer.CheckPasswowrd(psPassword)

            End Select
        End If

        Return False
    End Function

    Private Shared Function ValidGameType(ByVal psAction As String, ByVal psGameType As String, ByVal psContext As String) As String
        Select Case UCase(psAction)
            Case "TEASER"
                If Not (IsBasketball(psGameType) OrElse _
                        IsFootball(psGameType)) Then
                    Return String.Format("{0} doesn't have Teaser type.", psGameType)
                End If
            Case "IF BET"
                If Not (IsFootball(psGameType) OrElse IsBasketball(psGameType) _
                        OrElse IsBaseball(psGameType) OrElse IsHockey(psGameType)) Then
                    Return String.Format("{0} doesn't have If Bet type.", psGameType)
                End If

                If UCase(psContext) <> "CURRENT" Then
                    Return String.Format("{0} doesn't have If Bet type.", psContext)
                End If
        End Select

        Return ""
    End Function

    Private Shared Function GetTicketBet(ByVal psBetType As String, ByVal psTeam As String, ByVal pnSpread As Double, ByVal pnMoney As Double, ByVal psDescription As String, ByVal pbTeamTotalOver As Boolean) As CTicketBet
        Dim oTicketBet As CTicketBet = Nothing

        Select Case psBetType
            Case "Spread"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = psBetType
                oTicketBet.Description = psDescription
                If psTeam = "HOME" Then
                    oTicketBet.HomeSpread = pnSpread
                    oTicketBet.HomeSpreadMoney = pnMoney
                Else
                    oTicketBet.AwaySpread = pnSpread
                    oTicketBet.AwaySpreadMoney = pnMoney
                End If

            Case "TotalPoints"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = psBetType
                oTicketBet.TotalPoints = pnSpread
                oTicketBet.Description = psDescription
                If psTeam = "HOME" Then
                    oTicketBet.TotalPointsUnderMoney = pnMoney
                Else
                    oTicketBet.TotalPointsOverMoney = pnMoney
                End If
            Case "TeamTotalPoints"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = psBetType
                oTicketBet.TotalPoints = pnSpread
                oTicketBet.Description = psDescription

                If psTeam = "HOME" Then
                    If pbTeamTotalOver Then
                        oTicketBet.TotalPointsOverMoney = pnMoney
                        oTicketBet.TeamTotalName = "home"
                    Else
                        oTicketBet.TotalPointsUnderMoney = pnMoney
                        oTicketBet.TeamTotalName = "home"
                    End If

                Else
                    If pbTeamTotalOver Then
                        oTicketBet.TotalPointsOverMoney = pnMoney
                        oTicketBet.TeamTotalName = "away"
                    Else
                        oTicketBet.TotalPointsUnderMoney = pnMoney
                        oTicketBet.TeamTotalName = "away"
                    End If

                End If

            Case "MoneyLine"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = psBetType
                oTicketBet.Description = psDescription
                If psTeam = "HOME" Then
                    oTicketBet.HomeMoneyLine = pnMoney
                Else
                    oTicketBet.AwayMoneyLine = pnMoney
                End If

            Case "Draw"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = psBetType
                oTicketBet.DrawMoneyLine = pnMoney
                oTicketBet.Description = psDescription
            Case "Prop"
                oTicketBet = New CTicketBet()
                oTicketBet.BetType = "MoneyLine"
                oTicketBet.IsForProp = True
                oTicketBet.PropMoneyLine = pnMoney

            Case Else
                '' Error
        End Select

        Return oTicketBet
    End Function

#Region "For SBS"
    <System.Web.Services.WebMethod()> _
    Public Shared Function SBSBetting(ByVal psSelectedPlayerID As String, ByVal psGameID As String, ByVal psGameLineID As String, ByVal psActionType As String, _
                                   ByVal psGameType As String, ByVal psBookmaker As String, ByVal psContext As String, ByVal psGameDate As String, _
                                   ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psAwayTeamNumber As String, _
                                   ByVal psHomeTeamNumber As String, ByVal psAwayPitcher As String, ByVal psHomePitcher As String, _
                                   ByVal psAwayPitcherRH As String, ByVal psHomePitcherRH As String, ByVal psBetType As String, _
                                   ByVal psTeam As String, ByVal pnSpread As Double, ByVal pnMoney As Double, ByVal psCircled As String, ByVal psFav As String, ByVal pschkID As String, ByVal psDescription As String, ByVal psBuyPointValue As String, ByVal pbIsCheckPitcher As String) As String
        Dim sError As String = ""
      
        psAwayTeam = Replace(psAwayTeam, "&quot;", "'")
        psHomeTeam = Replace(psHomeTeam, "&quot;", "'")
        psDescription = Replace(psDescription, "&quot;", "'")
        Try
            '''''check parlay for odd>15
            Dim oCacheManager As CCacheManager = New CCacheManager()
            Dim lstCategory = oCacheManager.GetSysSettings("MAX_PARLAY_IN_GAME_MVP")
            Dim nMaxParlayInGame As Single
            If lstCategory IsNot Nothing Then
                nMaxParlayInGame = SafeSingle(lstCategory.GetValue(GetSportType(psGameType)))
            End If
          '  sError = psBetType & Math.Abs(pnSpread) & SelectedTicket(psSelectedPlayerID).BetAmount
            'sError = psActionType & "nMaxParlayInGame " & nMaxParlayInGame
            If SelectedTicket(psSelectedPlayerID).LastTicket IsNot Nothing Then
                For Each oBet As CTicketBet In SelectedTicket(psSelectedPlayerID).LastTicket.TicketBets
                    If psBetType.Equals("Spread", StringComparison.CurrentCultureIgnoreCase) AndAlso oBet.BetType.Equals("Spread", StringComparison.CurrentCultureIgnoreCase) AndAlso psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) AndAlso IsSoccer(psGameType) AndAlso (oBet.HomeTeam.Equals(psHomeTeam) OrElse oBet.AwayTeam.Equals(psAwayTeam)) Then
                        sError = "Parlay is invalid"
                    End If
                    If psBetType.Equals("TOTALPOINTS", StringComparison.CurrentCultureIgnoreCase) AndAlso oBet.BetType.Equals("TOTALPOINTS", StringComparison.CurrentCultureIgnoreCase) AndAlso psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) AndAlso IsSoccer(psGameType) AndAlso (oBet.HomeTeam.Equals(psHomeTeam) OrElse oBet.AwayTeam.Equals(psAwayTeam)) Then
                        sError = "Parlay is invalid"
                    End If

                    ' Don't check allownace when they are same key
                    'If Not oBet.GameLineID.Equals(psGameLineID, StringComparison.CurrentCultureIgnoreCase) Then
                    '    Exit For
                    'End If
                    ' sError = oBet.BetType
                    '  sError = oBet.Team & psBetType & nMaxParlayInGame & ":" & Math.Abs(oBet.HomeSpread) & psActionType & ":" & oBet.GameLineID & ":" & psGameLineID
                    Dim nSpread = pnSpread
                    Dim oGameLineManager = New Managers.CGameLineManager()
                    If psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) AndAlso Not psContext.Equals("Current", StringComparison.CurrentCultureIgnoreCase) Then

                        If psTeam.Equals("HOME") Then
                            nSpread = oGameLineManager.GetGameLine(psGameID).HomeSpread
                            ' sError = psTeam & nSpread
                        Else
                            nSpread = oGameLineManager.GetGameLine(psGameID).AwaySpread
                            'sError = psTeam & nSpread
                        End If
                    End If
                    If psBetType.Equals("Spread", StringComparison.CurrentCultureIgnoreCase) Then

                        If nMaxParlayInGame > 0 AndAlso oBet.GameLineID.Equals(psGameLineID, StringComparison.CurrentCultureIgnoreCase) AndAlso (IsFootball(psGameType) OrElse IsBasketball(psGameType)) Then
                            If (psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) OrElse psActionType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase)) AndAlso Math.Abs(nSpread) >= nMaxParlayInGame Then
                                ' sError = Math.Abs(pnSpread) & ">=" & nMaxParlayInGame
                                sError = String.Format("{0} for this game : {1} - {2} is not allowed", psActionType, psAwayTeam, psHomeTeam)
                            End If
                        End If
                    Else

                        If oBet.HomeSpread <> 0 Then
                            ' sError = oBet.Team & nMaxParlayInGame & ":" & Math.Abs(oBet.HomeSpread) & psActionType & ":" & oBet.GameLineID & ":" & psGameLineID
                            If nMaxParlayInGame > 0 AndAlso oBet.GameLineID.Equals(psGameLineID, StringComparison.CurrentCultureIgnoreCase) AndAlso (IsFootball(psGameType) OrElse IsBasketball(psGameType)) Then
                                ' sError = Math.Abs(oBet.HomeSpread) & psActionType & ":" & oBet.GameLineID & ":" & psGameLineID
                                nSpread = oGameLineManager.GetGameLine(oBet.GameID).HomeSpread
                                If (psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) OrElse psActionType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase)) AndAlso Math.Abs(nSpread) >= nMaxParlayInGame Then
                                    ' sError = Math.Abs(pnSpread) & ">=" & nMaxParlayInGame
                                    sError = String.Format("{0} for this game : {1} - {2} is not allowed", psActionType, psAwayTeam, psHomeTeam)
                                End If
                            End If
                        Else
                            ' sError = oBet.HomeSpread & ":" & nMaxParlayInGame & ":" & Math.Abs(oBet.HomeSpread) & psActionType & ":" & oBet.GameLineID & ":" & psGameLineID
                            If nMaxParlayInGame > 0 AndAlso oBet.GameLineID.Equals(psGameLineID, StringComparison.CurrentCultureIgnoreCase) AndAlso (IsFootball(psGameType) OrElse IsBasketball(psGameType)) Then
                                nSpread = oGameLineManager.GetGameLine(oBet.GameID).AwaySpread
                                If (psActionType.Equals("Parlay", StringComparison.CurrentCultureIgnoreCase) OrElse psActionType.Equals("Reverse", StringComparison.CurrentCultureIgnoreCase)) AndAlso Math.Abs(nSpread) >= nMaxParlayInGame Then
                                    ' sError = Math.Abs(pnSpread) & ">=" & nMaxParlayInGame
                                    sError = String.Format("{0} for this game : {1} - {2} is not allowed", psActionType, psAwayTeam, psHomeTeam)
                                End If
                            End If
                        End If



                    End If




                Next
            End If

            If SelectedTicket(psSelectedPlayerID).Preview Then ' Don't allow betting while preview wagers
                sError = "Please press Cancel or Back button before you can make any wager adjustment."
            End If

            If sError = "" Then ' Check valid gametype
                sError = ValidGameType(psActionType, psGameType, psContext)
            End If

            If sError = "" Then
                Dim oTicketBet As CTicketBet = GetTicketBet(psBetType, psTeam, pnSpread, pnMoney, psDescription, SafeBoolean(psFav))

                If oTicketBet IsNot Nothing Then
                    oTicketBet.GameID = psGameID
                    oTicketBet.GameLineID = psGameLineID
                    oTicketBet.GameType = psGameType
                    oTicketBet.Bookmaker = psBookmaker
                    oTicketBet.Context = psContext
                    oTicketBet.BuyPointValue = psBuyPointValue
                    oTicketBet.GameDate = SafeDate(psGameDate)
                    oTicketBet.IsFavorite = SafeBoolean(psFav)
                    If Not oTicketBet.IsForProp Then
                        If IsBaseball(psGameType) Then
                            oTicketBet.IsCheckPitcher = SafeBoolean(pbIsCheckPitcher)
                        End If
                        oTicketBet.AwayTeam = psAwayTeam
                        oTicketBet.HomeTeam = psHomeTeam
                        oTicketBet.AwayTeamNumber = psAwayTeamNumber
                        oTicketBet.HomeTeamNumber = psHomeTeamNumber
                        oTicketBet.HomePitcher = psHomePitcher
                        oTicketBet.AwayPitcher = psAwayPitcher
                        oTicketBet.HomePitcherRH = SafeBoolean(psHomePitcherRH)
                        oTicketBet.AwayPitcherRH = SafeBoolean(psAwayPitcherRH)
                    Else
                        oTicketBet.PropParticipantName = psAwayTeam
                        oTicketBet.PropRotationNumber = psAwayTeamNumber
                    End If

                    Dim bAlreadyAdd As Boolean = False
                    If UCase(psActionType) <> "STRAIGHT" AndAlso UCase(psActionType) <> "IF BET" Then
                        If SelectedTicket(psSelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                                UCase(SelectedTicket(psSelectedPlayerID).LastTicket.TicketType) = UCase(psActionType) Then

                            sError = SelectedTicket(psSelectedPlayerID).LastTicket.AddTicketBet(oTicketBet)
                            bAlreadyAdd = True
                        End If
                    End If

                    If Not bAlreadyAdd Then
                        If SelectedTicket(psSelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                                SelectedTicket(psSelectedPlayerID).LastTicket.NumOfTicketBets = 0 Then
                            SelectedTicket(psSelectedPlayerID).RemoveTickets(SelectedTicket(psSelectedPlayerID).LastTicket.TicketID)
                        End If
                        Dim UserSession As New CSBCSession()
                        Dim oTicket As CTicket = New CTicket(psActionType, UserSession.Cache.GetPlayerInfo(psSelectedPlayerID).SuperAgentID, psSelectedPlayerID)
                        sError = oTicket.AddTicketBet(oTicketBet)

                        If sError = "" Then
                            sError = SelectedTicket(psSelectedPlayerID).AddTicket(oTicket)
                        End If

                    End If

                    If Not oTicketBet.IsForProp Then
                        oTicketBet.IsCircled = SafeBoolean(psCircled)
                    End If

                    If oTicketBet.IsCircled AndAlso (UCase(psActionType) = "STRAIGHT" OrElse UCase(psActionType) = "IF BET") Then
                        '' Create OddsRulesEngine
                        Dim olstGameID As New List(Of String)
                        olstGameID.Add(oTicketBet.GameID)

                        Dim oCache As New CacheUtils.CCacheManager()
                        Dim oOddsRules As New COddRulesEngine(olstGameID, _
                                                              oCache.GetPlayerInfo(psSelectedPlayerID).SuperAdminID, False, oCache.GetPlayerInfo(psSelectedPlayerID).Template, oCache.GetPlayerInfo(psSelectedPlayerID).SuperAgentID, psSelectedPlayerID)

                        If Not oTicketBet.ValidateStraightCircled(oOddsRules, psTeam) Then
                            SelectedTicket(psSelectedPlayerID).LastTicket.RemoveTicketBets(oTicketBet.TicketBetID)
                            sError = "You are exceeding the maximum allowed for this Circled game."
                        End If
                    End If

                Else
                    sError = "Can not betting right now. Please try again later."
                End If

            End If
        Catch ex As Exception
            sError = "Can not betting right now. Please try again later."
            _log.Error("Betting Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""chkID"":""{1}""}}"

        Return String.Format(sResult, sError, pschkID)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function SBSRemoveBet(ByVal psSelectedPlayerID As String, ByVal psGameID As String, ByVal psGameLineID As String, ByVal psActionType As String, _
                                   ByVal psGameType As String, ByVal psBookmaker As String, ByVal psContext As String, ByVal psGameDate As String, _
                                   ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psAwayTeamNumber As String, _
                                   ByVal psHomeTeamNumber As String, ByVal psAwayPitcher As String, ByVal psHomePitcher As String, _
                                   ByVal psAwayPitcherRH As String, ByVal psHomePitcherRH As String, ByVal psBetType As String, _
                                   ByVal psTeam As String, ByVal pnSpread As Double, ByVal pnMoney As Double, ByVal psCircled As String, ByVal pschkID As String) As String
        Dim sError As String = ""
        psAwayTeam = replace(psAwayTeam, "&quot;", "'")
        psHomeTeam = replace(psHomeTeam, "&quot;", "'")

        Try
            If SelectedTicket(psSelectedPlayerID).Preview Then ' Don't allow betting while preview wagers
                sError = "Please press Cancel or Back button before you can make any wager adjustment."
            End If

            If sError = "" Then ' Check valid gametype
                sError = ValidGameType(psActionType, psGameType, psContext)
            End If

            If sError = "" Then
                Dim oTicketBet As CTicketBet = GetTicketBet(psBetType, psTeam, pnSpread, pnMoney, "", False)

                If oTicketBet IsNot Nothing Then
                    oTicketBet.GameID = psGameID
                    oTicketBet.GameLineID = psGameLineID
                    oTicketBet.GameType = psGameType
                    oTicketBet.Bookmaker = psBookmaker
                    oTicketBet.Context = psContext
                    oTicketBet.GameDate = SafeDate(psGameDate)

                    If Not oTicketBet.IsForProp Then
                        oTicketBet.AwayTeam = psAwayTeam
                        oTicketBet.HomeTeam = psHomeTeam
                        oTicketBet.AwayTeamNumber = psAwayTeamNumber
                        oTicketBet.HomeTeamNumber = psHomeTeamNumber
                        oTicketBet.HomePitcher = psHomePitcher
                        oTicketBet.AwayPitcher = psAwayPitcher
                        oTicketBet.HomePitcherRH = SafeBoolean(psHomePitcherRH)
                        oTicketBet.AwayPitcherRH = SafeBoolean(psAwayPitcherRH)
                    Else
                        oTicketBet.PropParticipantName = psAwayTeam
                        oTicketBet.PropRotationNumber = psAwayTeamNumber
                    End If

                    For Each oBet As CTicketBet In SelectedTicket(psSelectedPlayerID).LastTicket.TicketBets
                        If oBet.GameID = oTicketBet.GameID AndAlso oBet.Bookmaker = oTicketBet.Bookmaker Then
                            If oBet.Team = oTicketBet.Team AndAlso oBet.TeamNumber = oTicketBet.TeamNumber _
                            AndAlso oBet.BetType = oTicketBet.BetType AndAlso oBet.Context = oTicketBet.Context Then '' Check exist TicketBet
                                '' Remove TicketBet
                                SelectedTicket(psSelectedPlayerID).LastTicket.RemoveTicketBets(oBet.TicketBetID)
                            End If
                        End If
                    Next

                Else
                    sError = "Can not remove bet right now. Please try again later."
                End If

            End If
        Catch ex As Exception
            sError = "Can not remove bet right now. Please try again later."
            _log.Error("Betting Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""chkID"":""{1}""}}"

        Return String.Format(sResult, sError, pschkID)
    End Function
#End Region

#Region "For SBC"
    <System.Web.Services.WebMethod()> _
    Public Shared Function Betting(ByVal psSelectedPlayerID As String, ByVal psGameID As String, ByVal psGameLineID As String, ByVal psActionType As String, _
                                   ByVal psGameType As String, ByVal psBookmaker As String, ByVal psContext As String, ByVal psGameDate As String, _
                                   ByVal psAwayTeam As String, ByVal psHomeTeam As String, ByVal psAwayTeamNumber As String, _
                                   ByVal psHomeTeamNumber As String, ByVal psAwayPitcher As String, ByVal psHomePitcher As String, _
                                   ByVal psAwayPitcherRH As String, ByVal psHomePitcherRH As String, ByVal psBetType As String, _
                                   ByVal psTeam As String, ByVal pnSpread As Double, ByVal pnMoney As Double, ByVal psCircled As String, ByVal psDescription As String) As String
        Dim sError As String = ""
        Dim sHTML As String = ""
        Dim sParentID As String = ""

        psAwayTeam = Replace(psAwayTeam, "&quot;", "'")
        psHomeTeam = Replace(psHomeTeam, "&quot;", "'")

        Try
          
            If SelectedTicket(psSelectedPlayerID).Preview Then ' Don't allow betting while preview wagers
                sError = "Please press Cancel or Back button before you can make any wager adjustment."
            End If

            If sError = "" Then ' Check valid gametype
                sError = ValidGameType(psActionType, psGameType, psContext)
            End If

            If sError = "" Then
                Dim oTicketBet As CTicketBet = GetTicketBet(psBetType, psTeam, pnSpread, pnMoney, psDescription, False)

                If oTicketBet IsNot Nothing Then
                    oTicketBet.GameID = psGameID
                    oTicketBet.GameLineID = psGameLineID
                    oTicketBet.GameType = psGameType
                    oTicketBet.Bookmaker = psBookmaker
                    oTicketBet.Context = psContext
                    oTicketBet.GameDate = SafeDate(psGameDate)

                    If Not oTicketBet.IsForProp Then
                        oTicketBet.AwayTeam = psAwayTeam
                        oTicketBet.HomeTeam = psHomeTeam
                        oTicketBet.AwayTeamNumber = psAwayTeamNumber
                        oTicketBet.HomeTeamNumber = psHomeTeamNumber
                        oTicketBet.HomePitcher = psHomePitcher
                        oTicketBet.AwayPitcher = psAwayPitcher
                        oTicketBet.HomePitcherRH = SafeBoolean(psHomePitcherRH)
                        oTicketBet.AwayPitcherRH = SafeBoolean(psAwayPitcherRH)
                    Else
                        oTicketBet.PropParticipantName = psAwayTeam
                        oTicketBet.PropRotationNumber = psAwayTeamNumber
                    End If

                    Dim bAlreadyAdd As Boolean = False
                    If UCase(psActionType) <> "STRAIGHT" AndAlso UCase(psActionType) <> "IF BET" Then
                        If SelectedTicket(psSelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                                UCase(SelectedTicket(psSelectedPlayerID).LastTicket.TicketType) = UCase(psActionType) Then

                            sError = SelectedTicket(psSelectedPlayerID).LastTicket.AddTicketBet(oTicketBet)
                            bAlreadyAdd = True
                        End If
                    End If

                    If Not bAlreadyAdd Then
                        If SelectedTicket(psSelectedPlayerID).LastTicket IsNot Nothing AndAlso _
                                SelectedTicket(psSelectedPlayerID).LastTicket.NumOfTicketBets = 0 Then
                            SelectedTicket(psSelectedPlayerID).RemoveTickets(SelectedTicket(psSelectedPlayerID).LastTicket.TicketID)
                        End If

                        Dim oTicket As CTicket = New CTicket(psActionType, New CacheUtils.CCacheManager().GetPlayerInfo(psSelectedPlayerID).SuperAgentID, psSelectedPlayerID)
                        sError = oTicket.AddTicketBet(oTicketBet)

                        If sError = "" Then
                            sError = SelectedTicket(psSelectedPlayerID).AddTicket(oTicket)
                        End If

                    End If

                    If Not oTicketBet.IsForProp Then
                        oTicketBet.IsCircled = SafeBoolean(psCircled)
                    End If

                    If oTicketBet.IsCircled AndAlso (UCase(psActionType) = "STRAIGHT" OrElse UCase(psActionType) = "IF BET") Then
                        '' Create OddsRulesEngine
                        Dim olstGameID As New List(Of String)
                        olstGameID.Add(oTicketBet.GameID)

                        Dim oCache As New CacheUtils.CCacheManager()
                        Dim oOddsRules As New COddRulesEngine(olstGameID, _
                                                              oCache.GetPlayerInfo(psSelectedPlayerID).SuperAdminID, False, oCache.GetPlayerInfo(psSelectedPlayerID).Template, oCache.GetPlayerInfo(psSelectedPlayerID).SuperAgentID, psSelectedPlayerID)

                        If Not oTicketBet.ValidateStraightCircled(oOddsRules, psTeam) Then
                            SelectedTicket(psSelectedPlayerID).LastTicket.RemoveTicketBets(oTicketBet.TicketBetID)
                            sError = "You are exceeding the maximum allowed for this Circled game."
                        End If
                    End If

                    If sError = "" Then
                        sHTML = SelectedTicket(psSelectedPlayerID).LastTicket.HTML
                        sParentID = SelectedTicket(psSelectedPlayerID).LastTicket.TicketID
                    End If
                Else
                    sError = "Can not betting right now. Please try again later."
                End If

            End If
        Catch ex As Exception
            sError = "Can not betting right now. Please try again later."
            _log.Error("Betting Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""ParentID"":""{1}"",""HTML"":""{2}""}}"

        Return String.Format(sResult, sError, sParentID, sHTML)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function ClearWagers(ByVal psSelectedPlayerID As String) As Boolean
        Dim sKey As String = String.Format("{0}_SELECTED_TICKETS", psSelectedPlayerID)
        If HttpContext.Current.Session(sKey) IsNot Nothing Then
            HttpContext.Current.Session(sKey) = Nothing
        End If

        Return True
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function NewWager(ByVal psSelectedPlayerID As String) As String
        SelectedTicket(psSelectedPlayerID).NewTicket(New CacheUtils.CCacheManager().GetPlayerInfo(psSelectedPlayerID).SuperAgentID, psSelectedPlayerID)

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""ParentID"":""{1}"",""HTML"":""{2}""}}"

        Return String.Format(sResult, "", SelectedTicket(psSelectedPlayerID).LastTicket.TicketID, _
                                   SelectedTicket(psSelectedPlayerID).LastTicket.HTML)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function RemoveBet(ByVal psSelectedPlayerID As String, ByVal psTicketID As String, ByVal psTicketBetID As String) As String
        Dim sError As String = ""
        Dim sHTML As String = ""

        Try
            Dim oTicket As CTicket = SelectedTicket(psSelectedPlayerID).GetTicket(psTicketID)
            If oTicket IsNot Nothing Then
                If oTicket.TicketBets.Count > 1 Then
                    oTicket.RemoveTicketBets(psTicketBetID)
                    sHTML = oTicket.HTML()
                Else
                    SelectedTicket(psSelectedPlayerID).RemoveTickets(psTicketID)
                End If
            End If

        Catch ex As Exception
            sError = "Can not delete bet right now. Please try again later."
            _log.Error("RemoveBet Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""ParentID"":""{1}"",""HTML"":""{2}""}}"

        Return String.Format(sResult, sError, psTicketID, sHTML)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function BuyPoints(ByVal psSelectedPlayerID As String, ByVal psTicketID As String, ByVal psTicketBetID As String, ByVal psPoints As String) As String
        Dim sError As String = ""
        Dim sValue As String = ""
        Dim sWinAmount As String = "0"
        Dim sWinAmountID As String = ""

        Try
            Dim oTicket As CTicket = SelectedTicket(psSelectedPlayerID).GetTicket(psTicketID)
            If oTicket IsNot Nothing Then ' Check exist Ticket
                Dim oTicketBet As CTicketBet = oTicket.GetTicketBet(psTicketBetID)

                If oTicketBet IsNot Nothing Then ' Check exist TicketBet
                    If psPoints <> "" Then ' BuyPoint or not
                        Dim oBuyPoints As String() = psPoints.Split("|")

                        oTicketBet.AddPoint = SafeDouble(oBuyPoints(0))
                        oTicketBet.AddPointMoney = SafeDouble(oBuyPoints(1))
                    Else
                        oTicketBet.AddPoint = 0
                        oTicketBet.AddPointMoney = 0
                    End If

                    If oTicketBet.AddPoint <> 0 Then
                        sValue = SafeString(oTicketBet.AddPoint) & "|" & SafeString(oTicketBet.AddPointMoney)
                    End If
                End If

                oTicket.ReCalcAmount()
                Select Case UCase(oTicket.TicketType)
                    Case "PARLAY"
                        sWinAmountID = String.Format("lblResult{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.RiskAmount) & "/" & SafeString(oTicket.WinAmount)

                    Case Else
                        sWinAmountID = String.Format("txtWin{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.WinAmount)

                End Select
            End If

        Catch ex As Exception
            sError = "Can not buy points right now. Please try again later."
            _log.Error("BuyPoints Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""BuyPointID"":""ddlBuyPoint{1}"",""BuyPointValue"":""{2}"",""WinAmountID"":""{3}"",""WinAmount"":""{4}""}}"

        Return String.Format(sResult, sError, psTicketBetID, sValue, sWinAmountID, sWinAmount)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function SaveAmount(ByVal psSelectedPlayerID As String, ByVal psTicketID As String, ByVal psBetAmount As String) As String
        Dim sError As String = ""
        Dim sWinAmount As String = "0"
        Dim sWinAmountID As String = ""

        Try
            Dim oTicket As CTicket = SelectedTicket(psSelectedPlayerID).GetTicket(psTicketID)
            If oTicket IsNot Nothing Then ' Check exist Ticket
                Select Case UCase(oTicket.TicketType)
                    Case "PARLAY"
                        oTicket.BetAmount = SafeDouble(psBetAmount)

                        sWinAmountID = String.Format("lblResult{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.RiskAmount) & "/" & SafeString(oTicket.WinAmount)

                    Case "REVERSE"
                        Dim nWinAmount As Double = SafeDouble(psBetAmount) * 4
                        Dim nBetAmount As Double = 0
                        Dim nNum As Integer = 0
                        For Each oTicketBet As CTicketBet In oTicket.TicketBets
                            nBetAmount += (CalcRate(oTicketBet.BetPoint + oTicketBet.AddPointMoney) - 1)
                            nNum += 1
                        Next

                        If nNum > 1 Then
                            nBetAmount = Math.Round(nWinAmount / (nBetAmount * 2 * (nNum - 1)), 2)
                            oTicket.BetAmount = nBetAmount
                        End If

                        sWinAmountID = String.Format("txtWin{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.WinAmount)

                    Case "STRAIGHT", "IF BET"
                        oTicket.BetAmount = SafeDouble(psBetAmount)

                        sWinAmountID = String.Format("txtWin{0}", oTicket.TicketID)
                        sWinAmount = SafeString(Math.Round(oTicket.WinAmount))

                    Case "TEASER"
                        oTicket.BetAmount = SafeDouble(psBetAmount)

                        sWinAmountID = String.Format("txtWin{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.WinAmount)

                End Select

            End If

        Catch ex As Exception
            sError = "Can not place bet amount right now. Please try again later."
            _log.Error("SaveAmount Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""WinAmountID"":""{1}"",""WinAmount"":""{2}""}}"

        Return String.Format(sResult, sError, sWinAmountID, sWinAmount)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function TicketOption(ByVal psSelectedPlayerID As String, ByVal psTicketID As String, _
                                        ByVal psTicketOption As String, ByVal psTicketOptionText As String) As String
        Dim sError As String = ""
        Dim sWinAmount As String = "0"
        Dim sWinAmountID As String = ""

        Try
            Dim oTicket As CTicket = SelectedTicket(psSelectedPlayerID).GetTicket(psTicketID)
            If oTicket IsNot Nothing Then ' Check exist Ticket

                oTicket.TicketOption = psTicketOption
                oTicket.TicketOptionText = psTicketOptionText

                Select Case UCase(oTicket.TicketType)
                    Case "PARLAY"
                        sWinAmountID = String.Format("lblResult{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.RiskAmount) & "/" & SafeString(oTicket.WinAmount)

                    Case Else
                        oTicket.TicketOption = psTicketOption

                        sWinAmountID = String.Format("txtWin{0}", oTicket.TicketID)
                        sWinAmount = SafeString(oTicket.WinAmount)

                End Select

            End If

        Catch ex As Exception
            sError = "Can not change ticket type right now. Please try again later."
            _log.Error("TicketOption Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""WinAmountID"":""{1}"",""WinAmount"":""{2}""}}"

        Return String.Format(sResult, sError, sWinAmountID, sWinAmount)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function PreviewWagers(ByVal psSelectedPlayerID As String) As String
        Dim sError As String = ""
        Dim sHTML As String = ""

        Try
            SelectedTicket(psSelectedPlayerID).CalcRiskAmount = False
            SelectedTicket(psSelectedPlayerID).Preview = True
            sHTML = SelectedTicket(psSelectedPlayerID).HTML()
        Catch ex As Exception
            sError = "Can not go preview wagers right now. Please try again later."
            _log.Error("PreviewWagers Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""Warnning"":""{1}"",""HTML"":""{2}""}}"

        Return String.Format(sResult, sError, "Please Review Wager Carefully & Re-enter Password To Confirm Wager!", sHTML)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function LoadWagers(ByVal psSelectedPlayerID As String) As String
        Dim sError As String = ""
        Dim sHTML As String = ""

        Try
            HttpContext.Current.Session("backWager") = True
            SelectedTicket(psSelectedPlayerID).Preview = False
            sHTML = SelectedTicket(psSelectedPlayerID).HTML()
        Catch ex As Exception
            sError = "Can not go back right now. Please try again later."
            _log.Error("LoadWagers Error :" & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""Warnning"":""{1}"",""HTML"":""{2}""}}"

        Return String.Format(sResult, sError, "", sHTML)
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function SaveWagers(ByVal psSelectedPlayerID As String, ByVal psPassword As String) As String
        Dim sError As String = ""
        Dim sWarnning As String = ""
        Dim sHTML As String = ""
        Dim sPendingAmount As String = ""
        Dim sBalanceAmount As String = ""

        Try
            Dim oCache As New CacheUtils.CCacheManager()
            Dim oPlayer As CacheUtils.CPlayer = oCache.GetPlayerInfo(psSelectedPlayerID)

            If Not CanBetting(psSelectedPlayerID) Then ' Check Current Player can betting or not
                If oPlayer.IsBettingLocked Then '' Locked betting
                    sError = "Your account has been locked for placing a bet. Please contact your Agent."
                ElseIf oPlayer.OriginalAmount = 0 Then '' Empty Amount
                    sError = "Please call your Agent to reactivate your account."
                Else '' Service crash, lock betting whole users
                    sError = "All betting has been disabled. Please contact your administrator for more information."
                End If

            ElseIf CheckPassword(oPlayer, psPassword) Then ' Check User's confirm password
                If SelectedTicket(psSelectedPlayerID).Validate(oPlayer.SuperAdminID, psSelectedPlayerID, oPlayer.Template, oPlayer.BalanceAmount, oPlayer.IsSuperPlayer) = ECheckStatus.Update Then
                    sWarnning = "<span style='font-size:13px; font-weight:bold;'>Attention:</span> Lines Have Changed. Please Review And Reconfirm Your Bet."
                    sHTML = SelectedTicket(psSelectedPlayerID).HTML()

                Else ' All validate, begin save tickets
                    If SelectedTicket(psSelectedPlayerID).SaveTickets(oPlayer.BalanceAmount, _
                                                              oPlayer.AgentID, oPlayer.UserID, UserID) Then ' Save successfully
                        sWarnning = "Wager(s) has been saved successfully"
                        sHTML = ""

                        '' Get Player's PendingAmount
                        Dim oTicketManager As New SBCBL.Managers.CTicketManager()
                        sPendingAmount = FormatNumber(SafeRound(oTicketManager.GetPlayerPendingAmount(psSelectedPlayerID, Nothing)), GetRoundMidPoint())

                        '' Get Player's BalanceAmount
                        sBalanceAmount = FormatNumber(SafeRound(oPlayer.BalanceAmount), GetRoundMidPoint())

                        If UserType() = EUserType.CallCenterAgent Then
                            '' save CCAgent bettings to notes
                            LogCCAgentBet(psSelectedPlayerID)
                        End If

                        Dim sKey As String = String.Format("{0}_SELECTED_TICKETS", psSelectedPlayerID)
                        HttpContext.Current.Session(sKey) = Nothing
                    Else ' Fail
                        sError = "Fail to save wager(s)"
                    End If
                End If
            Else ' Confirm password is not validate
                sError = "Wrong password. Please input again."
            End If
        Catch exTicker As CTicketException
            sError = Replace(exTicker.Message, vbCrLf, "\n")
        Catch ex As Exception
            sError = "Can not save wager right now. Please try again later."
            _log.Error("Fail to save Wagers. Message: " & ex.Message, ex)
        End Try

        Dim sResult As String = "{{""ErrorMessage"":""{0}"",""Warnning"":""{1}"",""HTML"":""{2}"",""PendingAmount"":""{3}"",""BalanceAmount"":""{4}""}}"

        Return String.Format(sResult, sError, sWarnning, sHTML, sPendingAmount, sBalanceAmount)
    End Function

#End Region
End Class
