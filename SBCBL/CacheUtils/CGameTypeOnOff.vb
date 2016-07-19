Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CGameTypeOnOff

        Private _GameType As String
        Private _OffBefore As Integer
        Private _DisplayBefore As Integer
        Private _GameTotalPointsDisplay As Integer

        Public Sub New(ByVal odr As DataRow)
            _GameType = SafeString(odr("GameType"))
            _OffBefore = SafeInteger(odr("OffBefore"))
            _DisplayBefore = SafeInteger(odr("DisplayBefore"))
            _GameTotalPointsDisplay = SafeInteger(odr("GameTotalPointsDisplay"))
        End Sub

        Public Property GameType() As String
            Get
                Return _GameType
            End Get
            Set(ByVal value As String)
                _GameType = value
            End Set
        End Property

        Public Property OffBefore() As Integer
            Get
                Return _OffBefore
            End Get
            Set(ByVal value As Integer)
                _OffBefore = value
            End Set
        End Property

        Public Property DisplayBefore() As Integer
            Get
                Return _DisplayBefore
            End Get
            Set(ByVal value As Integer)
                _DisplayBefore = value
            End Set
        End Property

        Public Property GameTotalPointsDisplay() As Integer
            Get
                Return _GameTotalPointsDisplay
            End Get
            Set(ByVal value As Integer)
                _GameTotalPointsDisplay = value
            End Set
        End Property

    End Class
End Namespace

