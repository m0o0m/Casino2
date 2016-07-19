Imports WebsiteLibrary.CSBCStd

<Serializable()> Public Class CCheckTeaserOdds

    Public TeaserRuleID As String
    Public Teams As Integer
    Public Payout As Double
    Public MinTeam As Integer
    Public MaxTeam As Integer
    Public IsTiesLose As Boolean

    Public Sub New(ByVal psTeaserRuleID As String, ByVal pnTeams As Integer)
        Me.TeaserRuleID = psTeaserRuleID
        Me.Teams = pnTeams
    End Sub

    Public Sub New(ByVal podrTeaerOdd As DataRow)
        With Me
            .TeaserRuleID = SafeString(podrTeaerOdd("TeaserRuleID"))
            .Teams = SafeInteger(podrTeaerOdd("Key"))
            .Payout = SafeDouble(podrTeaerOdd("Value"))
            .MinTeam = SafeInteger(podrTeaerOdd("MinTeam"))
            .MaxTeam = SafeInteger(podrTeaerOdd("MaxTeam"))
            .IsTiesLose = SafeString(podrTeaerOdd("IsTiesLose")) = "Y"
        End With
    End Sub

End Class
