<Serializable()> Public Class CCheckAmountPlayer
    Public PlayerID As String
    Public TotalNetAmount As Double

    Public Sub New(ByVal psPlayerID As String, ByVal psTotalNetAmount As Double)
        Me.PlayerID = psPlayerID
        Me.TotalNetAmount = psTotalNetAmount
    End Sub

End Class
