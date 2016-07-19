Public Class CPageHistoryItem
    Public _Caption As String
    Public _URL As String

    Public Property Caption() As String
        Get
            Return _Caption
        End Get
        Set(ByVal value As String)
            _Caption = value
        End Set
    End Property

    Public Property URL() As String
        Get
            Return _URL
        End Get
        Set(ByVal value As String)
            _URL = value
        End Set
    End Property

    Public Sub New(ByVal sCaption As String, ByVal sURL As String)
        Caption = sCaption
        URL = sURL
    End Sub
End Class