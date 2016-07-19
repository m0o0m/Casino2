Imports WebsiteLibrary.CSBCStd

<Serializable()> Public Class CCheckParlayPayout

    Public Category As String = ""
    Public Teams As Integer = Int32.MaxValue
    Public PayoutPercent As Double = 1

    Public Sub New(ByVal podrCurrent As DataRow, ByVal podrTigerSB As DataRow)
        Category = SafeString(podrTigerSB("Category"))
        Teams = SafeInteger(SafeString(podrCurrent("Key")).ToUpper.Replace("TEAMS", "").Trim)

        Dim nCurrent As Double = SafeDouble(podrCurrent("Value"))
        Dim nTigerSB As Double = SafeDouble(podrTigerSB("Value"))

        If nCurrent = 0 OrElse nTigerSB = 0 Then
            PayoutPercent = 1
        Else
            PayoutPercent = Math.Round(nTigerSB / nCurrent, 4, MidpointRounding.AwayFromZero)
        End If
    End Sub

End Class
