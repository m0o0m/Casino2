Imports WebsiteLibrary.CSBCStd
Imports SBCEngine.CEngineStd

<Serializable()> Public Class CCheckGame

    Public GameID As String = ""
    Public GameType As String = ""
    Public GameStatus As String = ""
    Public GameDate As Date

    Public HomeTeam As String = ""
    Public HomePitcher As String = ""
    Public OriginHomePitcher As String = ""
    Public HomeScore As Integer = 0
    Public HomeFirstHalfScore As Integer = 0
    Public HomeFirstQScore As Integer = 0
    Public HomeSecondQScore As Integer = 0
    Public HomeThirdQScore As Integer = 0
    Public HomeFourQScore As Integer = 0

    Public AwayTeam As String = ""
    Public AwayPitcher As String = ""
    Public OriginAwayPitcher As String = ""
    Public AwayScore As Integer = 0
    Public AwayFirstHalfScore As Integer = 0
    Public AwayFirstQScore As Integer = 0
    Public AwaySecondQScore As Integer = 0
    Public AwayThirdQScore As Integer = 0
    Public AwayFourQScore As Integer = 0

    Public IsFirstHalfFinished As Boolean = False
    Public CurrentQuater As Integer = 0
    Public IsGameSuspend As Boolean = False
    Public IsProposition As Boolean = False

    Public Sub New(ByVal psGameID As String)
        Me.GameID = psGameID
    End Sub

    Public Sub New(ByVal poData As DataRow)
        With Me
            .GameID = SafeString(poData("GameID"))
            .GameType = UCase(SafeString(poData("GameType")))
            .GameStatus = UCase(SafeString(poData("GameStatus")))
            .GameDate = SafeDate(poData("GameDate"))

            .HomeTeam = SafeString(poData("HomeTeam"))
            .HomePitcher = UCase(SafeString(poData("GameHomePitcher")))
            .OriginHomePitcher = UCase(SafeString(poData("OriginHomePitcher")))
            .HomeScore = SafeInteger(poData("HomeScore"))
            .HomeFirstHalfScore = SafeInteger(poData("HomeFirstHalfScore"))
            .HomeFirstQScore = SafeInteger(poData("HomeFirstQScore"))
            .HomeSecondQScore = SafeInteger(poData("HomeSecondQScore"))
            .HomeThirdQScore = SafeInteger(poData("HomeThirdQScore"))
            .HomeFourQScore = SafeInteger(poData("HomeFourQScore"))

            .AwayTeam = SafeString(poData("AwayTeam"))
            .AwayPitcher = UCase(SafeString(poData("GameAwayPitcher")))
            .OriginAwayPitcher = UCase(SafeString(poData("OriginAwayPitcher")))
            .AwayScore = SafeInteger(poData("AwayScore"))
            .AwayFirstHalfScore = SafeInteger(poData("AwayFirstHalfScore"))
            .AwayFirstQScore = SafeInteger(poData("AwayFirstQScore"))
            .AwaySecondQScore = SafeInteger(poData("AwaySecondQScore"))
            .AwayThirdQScore = SafeInteger(poData("AwayThirdQScore"))
            .AwayFourQScore = SafeInteger(poData("AwayFourQScore"))

            .IsFirstHalfFinished = UCase(SafeString(poData("IsFirstHalfFinished"))) = "Y"
            .CurrentQuater = SafeInteger(poData("CurrentQuater"))

            If IsContains(Me.GameStatus, "CANCELLED", "PONED", "SUSP") Then
                .IsGameSuspend = checkGameSuspend(SafeDate(poData("GameDate")))
            End If
            .IsProposition = UCase(SafeString(poData("IsForProp"))) = "Y"
        End With
    End Sub

    Public ReadOnly Property ChoiceHomeScore(ByVal psTicketBetContext As String) As Integer
        Get
            Select Case UCase(psTicketBetContext)
                Case "1Q"
                    Return Me.HomeFirstQScore
                Case "2Q"
                    Return Me.HomeSecondQScore
                Case "3Q"
                    Return Me.HomeThirdQScore
                Case "4Q"
                    'Return Me.HomeScore - (Me.HomeFirstQScore + Me.HomeSecondQScore + Me.HomeThirdQScore)
                    Return Me.HomeFourQScore
                Case "1H"
                    Return Me.HomeFirstHalfScore
                Case "2H"
                    Return Me.HomeScore - Me.HomeFirstHalfScore
            End Select

            Return Me.HomeScore
        End Get
    End Property

    Public ReadOnly Property ChoiceAwayScore(ByVal psTicketBetContext As String) As Integer
        Get
            Select Case UCase(psTicketBetContext)
                Case "1Q"
                    Return Me.AwayFirstQScore
                Case "2Q"
                    Return Me.AwaySecondQScore
                Case "3Q"
                    Return Me.AwayThirdQScore
                Case "4Q"
                    'Return Me.AwayScore - (Me.AwayFirstQScore + Me.AwaySecondQScore + Me.AwayThirdQScore)
                    Return Me.AwayFourQScore
                Case "1H"
                    Return Me.AwayFirstHalfScore
                Case "2H"
                    Return Me.AwayScore - Me.AwayFirstHalfScore
            End Select

            Return Me.AwayScore
        End Get
    End Property

    Private Function checkGameSuspend(ByVal poGameDate As DateTime) As Boolean
        Dim nProcessMitutes As Integer = MINUTES_GAME_SUSPEND_PROCESSED
        If nProcessMitutes <= 0 Then nProcessMitutes = 5 * 60 + 1

        Return (GetCurrentEasternDate() - poGameDate).TotalMinutes >= nProcessMitutes
    End Function

End Class
