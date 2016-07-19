
Public Class CPageHistoryList
    Inherits ArrayList
    Public Overloads Sub add(ByVal sCaption As String, ByVal sURL As String)
        Me.add(New CPageHistoryItem(sCaption, sURL))
    End Sub

    Default Public Shadows Property Item(ByVal index As Integer) As CPageHistoryItem
        Get
            Return CType(MyBase.Item(index), CPageHistoryItem)
        End Get
        Set(ByVal Value As CPageHistoryItem)
            MyBase.Item(index) = Value
        End Set
    End Property
End Class
