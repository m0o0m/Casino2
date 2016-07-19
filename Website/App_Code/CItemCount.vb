Imports System.Xml.Serialization

<Serializable()> Public Class CItemCount

    Public ItemID As String = ""
    Public ItemCount As Integer = 0

    Public Sub New(ByVal psItemID As String, ByVal pnItemCount As Integer)
        Me.ItemID = psItemID
        Me.ItemCount = pnItemCount
    End Sub

End Class
