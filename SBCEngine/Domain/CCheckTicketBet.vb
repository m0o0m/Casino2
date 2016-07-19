Imports SBCEngine.CEngineStd
Imports WebsiteLibrary.CSBCStd

<Serializable()> Public Class CCheckTicketBet

    Public GameID As String = ""
    Public GameLineID As String = "" ''user for game is proposition
    Public PropParticipantName As String = "" ''user for game is proposition
    Public TicketID As String = ""
    Public TicketBetID As String = ""
    Public BetType As String = ""
    Public OddsRatio As Double = 0
    Public Context As String = ""
    Public TeamTotalName As String = ""
        
    Private _ChoiceTeam As String = ""

    Public ReadOnly Property ChoiceTeam() As String
        Get
            If _ChoiceTeam = "" Then
                Select Case UCase(Me.BetType)
                    Case "SPREAD"
                        _ChoiceTeam = SafeString(IIf(Me.HomeSpreadMoney <> 0, "HOME", "AWAY"))

                    Case "MONEYLINE"
                        _ChoiceTeam = SafeString(IIf(Me.HomeMoneyLine <> 0, "HOME", "AWAY"))

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        _ChoiceTeam = SafeString(IIf(Me.TotalPointsOverMoney <> 0, "OVER", "UNDER"))
                End Select
            End If

            Return _ChoiceTeam
        End Get
    End Property

    Private _ChoiceMoney As Double = 0

    Public ReadOnly Property ChoiceMoney() As Double
        Get
            If Me._ChoiceMoney = 0 Then
                Select Case UCase(Me.BetType)
                    Case "SPREAD"
                        Me._ChoiceMoney = SafeDouble(IIf(Me.ChoiceTeam = "HOME", Me.HomeSpreadMoney, Me.AwaySpreadMoney)) + Me.AddPointMoney

                    Case "MONEYLINE"
                        Me._ChoiceMoney = SafeDouble(IIf(Me.ChoiceTeam = "HOME", Me.HomeMoneyLine, Me.AwayMoneyLine))

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        Me._ChoiceMoney = SafeDouble(IIf(Me.ChoiceTeam = "OVER", Me.TotalPointsOverMoney, Me.TotalPointsUnderMoney)) + Me.AddPointMoney
                End Select
            End If

            Return _ChoiceMoney
        End Get
    End Property

    Private _ChoiceSpreadOrPoint As Double = 0

    Public ReadOnly Property ChoiceSpreadOrPoint() As Double
        Get
            If Me._ChoiceSpreadOrPoint = 0 Then
                Select Case UCase(Me.BetType)
                    Case "SPREAD"
                        Me._ChoiceSpreadOrPoint = SafeDouble(IIf(Me.ChoiceTeam = "HOME", Me.HomeSpread, Me.AwaySpread)) + Me.AddPoint

                    Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                        Me._ChoiceSpreadOrPoint = Me.TotalPoints + SafeDouble(IIf(Me.ChoiceTeam = "UNDER", Math.Abs(Me.AddPoint), -Math.Abs(Me.AddPoint)))
                End Select
            End If

            Return _ChoiceSpreadOrPoint
        End Get
    End Property

    Private _TicketBetStatus As String = ""

    Public Property TicketBetStatus() As String
        Get
            If _TicketBetStatus = "" Then
                _TicketBetStatus = STATUS_OPEN
            End If

            Return UCase(_TicketBetStatus)
        End Get
        Set(ByVal value As String)
            _TicketBetStatus = value
        End Set
    End Property

    Private _TicketBetStatusOld As String = ""

    Public ReadOnly Property IsStatusChanged() As Boolean
        Get
            Return UCase(_TicketBetStatus) <> UCase(_TicketBetStatusOld)
        End Get
    End Property

    Public HomeSpread As Double = 0
    Public HomeSpreadMoney As Double = 0
    Public AwaySpread As Double = 0
    Public AwaySpreadMoney As Double = 0

    Public HomePitcher As String = ""
    Public AwayPitcher As String = ""

    Public HomeMoneyLine As Double = 0
    Public AwayMoneyLine As Double = 0
    Public DrawMoneyLine As Double = 0

    Public TotalPoints As Double = 0
    Public TotalPointsOverMoney As Double = 0
    Public TotalPointsUnderMoney As Double = 0

    Public AddPoint As Double
    Public AddPointMoney As Double

    Public PropMoneyLine As Double = 0

    Private _PropStatus As String = ""

    Public Property PropStatus() As String
        Get
            If _PropStatus = "" Then
                _PropStatus = STATUS_LOSE
            End If

            Return UCase(_PropStatus)
        End Get
        Set(ByVal value As String)
            _PropStatus = value
        End Set
    End Property

    Private _bIsCheckPitcher As Boolean
    Public ReadOnly Property IsCheckPitcher() As Boolean
        Get
            Return _bIsCheckPitcher
        End Get
    End Property

    Public Sub New(ByVal psTicketBetID As String)
        Me.TicketBetID = psTicketBetID
    End Sub

    Public Sub New(ByVal podrTicketBet As DataRow)
        With Me
            .TicketBetID = SafeString(podrTicketBet("TicketBetID"))
            .GameID = SafeString(podrTicketBet("GameID"))
            .GameLineID = UCase(SafeString(podrTicketBet("GameLineID"))) ''use for game is proposition
            .PropParticipantName = SafeString(podrTicketBet("PropParticipantName")) ''use for game is proposition
            .TicketID = SafeString(podrTicketBet("TicketID"))
            .BetType = UCase(SafeString(podrTicketBet("BetType")))
            .TicketBetStatus = UCase(SafeString(podrTicketBet("TicketBetStatus")))
            ._TicketBetStatusOld = .TicketBetStatus
            .HomeSpread = SafeDouble(podrTicketBet("HomeSpread"))
            .HomeSpreadMoney = SafeDouble(podrTicketBet("HomeSpreadMoney"))
            .AwaySpread = SafeDouble(podrTicketBet("AwaySpread"))
            .AwaySpreadMoney = SafeDouble(podrTicketBet("AwaySpreadMoney"))
            .HomeMoneyLine = SafeDouble(podrTicketBet("HomeMoneyLine"))
            .AwayMoneyLine = SafeDouble(podrTicketBet("AwayMoneyLine"))
            .DrawMoneyLine = SafeDouble(podrTicketBet("DrawMoneyLine"))
            .TotalPoints = SafeDouble(podrTicketBet("TotalPoints"))
            .TotalPointsOverMoney = SafeDouble(podrTicketBet("TotalPointsOverMoney"))
            .TotalPointsUnderMoney = SafeDouble(podrTicketBet("TotalPointsUnderMoney"))
            .AddPoint = SafeDouble(podrTicketBet("AddPoint"))
            .AddPointMoney = SafeDouble(podrTicketBet("AddPointMoney"))
            .HomePitcher = UCase(SafeString(podrTicketBet("BetHomePitcher")))
            .AwayPitcher = UCase(SafeString(podrTicketBet("BetAwayPitcher")))
            .Context = UCase(SafeString(podrTicketBet("Context")))
            .PropMoneyLine = SafeDouble(podrTicketBet("PropMoneyLine"))
            .PropStatus = UCase(SafeString(podrTicketBet("PropStatus")))
            .TeamTotalName = UCase(SafeString(podrTicketBet("TeamTotalName")))
            _bIsCheckPitcher = SafeBoolean(podrTicketBet("IsCheckPitcher"))
        End With
    End Sub

    Public Sub RenewMoneyForSoccerOnly()
        Dim nNewChoiceMoney As Double = Me.ChoiceMoney

        If nNewChoiceMoney < 0 Then
            nNewChoiceMoney = nNewChoiceMoney * 2
        Else
            nNewChoiceMoney = nNewChoiceMoney / 2
        End If

        Select Case UCase(Me.BetType)
            Case "SPREAD"
                If Me.ChoiceTeam = "HOME" Then
                    Me.HomeSpreadMoney = nNewChoiceMoney
                Else
                    Me.AwaySpreadMoney = nNewChoiceMoney
                End If

            Case "TOTALPOINTS", "TEAMTOTALPOINTS"
                If Me.ChoiceTeam = "OVER" Then
                    Me.TotalPointsOverMoney = nNewChoiceMoney
                Else
                    Me.TotalPointsUnderMoney = nNewChoiceMoney
                End If
        End Select

        ''set is zero to re-get choice money
        Me._ChoiceMoney = 0
    End Sub

End Class
