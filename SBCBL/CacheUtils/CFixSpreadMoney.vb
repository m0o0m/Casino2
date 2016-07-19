Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CFixSpreadMoney

        Private _NumFixedSpreadMoneyCurrent As Integer
        Private _NumFixedSpreadMoneyH As Integer
        Private _NumFlatSpreadMoney As Single
        Private _NumFixedSpreadMoneyToTalGame As Integer
        Private _UseFixedSpreadMoney As Boolean
        Private _UseFlatSpreadMoney As Boolean
        Private _UseDefaultBookMaker As Boolean


        Public Sub New(ByVal odr As DataRow)
            _NumFixedSpreadMoneyCurrent = SafeInteger(odr("NumFixedSpreadMoneyCurrent"))
            _NumFixedSpreadMoneyToTalGame = SafeInteger(odr("NumFixedSpreadMoneyGameTotal"))
            _NumFixedSpreadMoneyH = SafeInteger(odr("NumFixedSpreadMoneyH"))
            _NumFlatSpreadMoney = SafeSingle(odr("NumFlatSpreadMoney"))
            _UseFixedSpreadMoney = SafeBoolean(odr("UseFixedSpreadMoney"))
            _UseFlatSpreadMoney = SafeBoolean(odr("UseFlatSpreadMoney"))
            _UseDefaultBookMaker = SafeBoolean(odr("UseDefaultBookmaker"))
        End Sub


        Public Property NumFixedSpreadMoneyCurrent() As Integer
            Get
                Return _NumFixedSpreadMoneyCurrent
            End Get
            Set(ByVal value As Integer)
                _NumFixedSpreadMoneyCurrent = value
            End Set
        End Property

        Public Property NumFixedSpreadMoneyToTalGame() As Integer
            Get
                Return _NumFixedSpreadMoneyToTalGame
            End Get
            Set(ByVal value As Integer)
                _NumFixedSpreadMoneyToTalGame = value
            End Set
        End Property

        Public Property NumFixedSpreadMoneyH() As Integer
            Get
                Return _NumFixedSpreadMoneyH
            End Get
            Set(ByVal value As Integer)
                _NumFixedSpreadMoneyH = value
            End Set
        End Property

        Public Property NumFlatSpreadMoney() As Single
            Get
                Return _NumFlatSpreadMoney
            End Get
            Set(ByVal value As Single)
                _NumFlatSpreadMoney = value
            End Set
        End Property

        Public Property UseFixedSpreadMoney() As Boolean
            Get
                Return _UseFixedSpreadMoney
            End Get
            Set(ByVal value As Boolean)
                _UseFixedSpreadMoney = value
            End Set
        End Property

        Public Property UseFlatSpreadMoney() As Boolean
            Get
                Return _UseFlatSpreadMoney
            End Get
            Set(ByVal value As Boolean)
                _UseFlatSpreadMoney = value
            End Set
        End Property

        Public Property UseDefaultBookMaker() As Boolean
            Get
                Return _UseDefaultBookMaker
            End Get
            Set(ByVal value As Boolean)
                _UseDefaultBookMaker = value
            End Set
        End Property
    End Class
End Namespace
