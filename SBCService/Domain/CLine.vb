Public Class CLine
    Implements ICloneable

    Public GameLineID As Guid
    Public LastUpdated As DateTime
    Public Context As String
    Public Bookmaker As String
    Public HomeSpread As Double
    Public HomeSpreadMoney As Double
    Public AwaySpread As Double
    Public AwaySpreadMoney As Double
    Public HomeMoneyLine As Double
    Public AwayMoneyLine As Double
    Public TotalPoints As Double
    Public TotalPointsOverMoney As Double
    Public TotalPointsUnderMoney As Double
    Public FeedSource As String
    Public DrawMoneyLine As Double = -1
    Public HPitcher As String
    Public APitcher As String
    Public HPitcherRightHand As Boolean
    Public APitcherRightHand As Boolean
    Public PropParticipantName As String
    Public PropRotationNumber As String
    Public PropMoneyLine As Double
    Public IsChange As Boolean = False
    Public IsLockedByPreset As Boolean = False
    Public AwayTeamTotalPoints As Double
    Public AwayTeamTotalPointsOverMoney As Double
    Public AwayTeamTotalPointsUnderMoney As Double
    Public HomeTeamTotalPoints As Double
    Public HomeTeamTotalPointsOverMoney As Double
    Public HomeTeamTotalPointsUnderMoney As Double

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New CLine With {.GameLineID = Guid.NewGuid, .LastUpdated = LastUpdated, _
                               .Context = Context, .Bookmaker = Bookmaker, .HomeSpread = HomeSpread, _
                               .HomeSpreadMoney = HomeSpreadMoney, .AwaySpread = AwaySpread, _
                               .AwaySpreadMoney = AwaySpreadMoney, .HomeMoneyLine = HomeMoneyLine, _
                               .AwayMoneyLine = AwayMoneyLine, .TotalPoints = TotalPoints, _
                               .TotalPointsOverMoney = TotalPointsOverMoney, .TotalPointsUnderMoney = TotalPointsUnderMoney, _
                               .FeedSource = FeedSource, .DrawMoneyLine = DrawMoneyLine, .HPitcher = HPitcher, _
                               .APitcher = APitcher, .HPitcherRightHand = HPitcherRightHand, _
                               .APitcherRightHand = APitcherRightHand, .PropParticipantName = PropParticipantName, _
                               .PropRotationNumber = PropRotationNumber, .PropMoneyLine = PropMoneyLine, _
                               .IsChange = IsChange, .IsLockedByPreset = IsLockedByPreset, _
                               .AwayTeamTotalPoints = AwayTeamTotalPoints, .AwayTeamTotalPointsOverMoney = AwayTeamTotalPointsOverMoney, _
                               .AwayTeamTotalPointsUnderMoney = AwayTeamTotalPointsUnderMoney, .HomeTeamTotalPoints = HomeTeamTotalPoints, _
                               .HomeTeamTotalPointsOverMoney = HomeTeamTotalPointsOverMoney, .HomeTeamTotalPointsUnderMoney = HomeTeamTotalPointsUnderMoney}
    End Function
End Class