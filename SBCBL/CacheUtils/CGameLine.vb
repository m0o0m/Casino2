Imports SBCBL.std

Namespace CacheUtils

    <Serializable()> Public Class CGameLine

#Region "Fields"
        Private _DrawMoneyLine As Double = 0
        Private _GameLineID As String = ""
        Private _GameID As String = ""
        Private _Context As String = ""
        Private _Bookmaker As String = ""
        Private _FeedSource As String = ""
        Private _ManualSetting As Boolean = False
        Private _GameLineOff As Boolean = False
        Private _IsCircle As Boolean = False
        Private _HomeSpread As Double = 0
        Private _HomeSpreadMoney As Double = 0
        Private _AwaySpread As Double = 0
        Private _AwaySpreadMoney As Double = 0
        Private _HomeMoneyLine As Double = 0
        Private _AwayMoneyLine As Double = 0
        Private _TotalPoints As Double = 0
        Private _TotalPointsOverMoney As Double = 0
        Private _TotalPointsUnderMoney As Double = 0

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _DrawMoneyLine = SafeDouble(podrData("DrawMoneyLine"))
            _GameLineID = SafeString(podrData("GameLineID"))
            _GameID = SafeString(podrData("GameID"))
            _Context = SafeString(podrData("Context"))
            _Bookmaker = SafeString(podrData("Bookmaker"))
            _FeedSource = SafeString(podrData("FeedSource"))
            _HomeSpread = SafeDouble(podrData("HomeSpread"))
            _HomeSpreadMoney = SafeDouble(podrData("HomeSpreadMoney"))
            _AwaySpread = SafeDouble(podrData("AwaySpread"))
            _AwaySpreadMoney = SafeDouble(podrData("AwaySpreadMoney"))
            _HomeMoneyLine = SafeDouble(podrData("HomeMoneyLine"))
            _AwayMoneyLine = SafeDouble(podrData("AwayMoneyLine"))
            _TotalPoints = SafeDouble(podrData("TotalPoints"))
            _TotalPointsOverMoney = SafeDouble(podrData("TotalPointsOverMoney"))
            _TotalPointsUnderMoney = SafeDouble(podrData("TotalPointsUnderMoney"))
            _ManualSetting = SafeString(podrData("ManualSetting")).Equals("Y", StringComparison.CurrentCultureIgnoreCase)
            _GameLineOff = SafeString(podrData("GameLineOff")).Equals("Y", StringComparison.CurrentCultureIgnoreCase)
            _IsCircle = SafeString(podrData("IsCircle")).Equals("Y", StringComparison.CurrentCultureIgnoreCase)

            AwayTeamTotalPoints = SafeDouble(podrData("AwayTeamTotalPoints"))
            AwayTeamTotalPointsOverMoney = SafeDouble(podrData("AwayTeamTotalPointsOverMoney"))
            AwayTeamTotalPointsUnderMoney = SafeDouble(podrData("AwayTeamTotalPointsUnderMoney"))

            HomeTeamTotalPoints = SafeDouble(podrData("HomeTeamTotalPoints"))
            HomeTeamTotalPointsOverMoney = SafeDouble(podrData("HomeTeamTotalPointsOverMoney"))
            HomeTeamTotalPointsUnderMoney = SafeDouble(podrData("HomeTeamTotalPointsUnderMoney"))
        End Sub

#End Region

#Region "Properties"

        Public Property GameLineID() As String
            Get
                Return _GameLineID
            End Get
            Set(ByVal value As String)
                _GameLineID = value
            End Set
        End Property

        Public Property GameID() As String
            Get
                Return _GameID
            End Get
            Set(ByVal value As String)
                _GameID = value
            End Set
        End Property

        Public Property Context() As String
            Get
                Return _Context
            End Get
            Set(ByVal value As String)
                _Context = value
            End Set
        End Property

        Public Property Bookmaker() As String
            Get
                Return _Bookmaker
            End Get
            Set(ByVal value As String)
                _Bookmaker = value
            End Set
        End Property

        Public Property FeedSource() As String
            Get
                Return _FeedSource
            End Get
            Set(ByVal value As String)
                _FeedSource = value
            End Set
        End Property

        Public Property HomeSpread() As Double
            Get
                Return _HomeSpread
            End Get
            Set(ByVal value As Double)
                _HomeSpread = value
            End Set
        End Property

        Public Property HomeSpreadMoney() As Double
            Get
                Return _HomeSpreadMoney
            End Get
            Set(ByVal value As Double)
                _HomeSpreadMoney = value
            End Set
        End Property

        Public Property AwaySpread() As Double
            Get
                Return _AwaySpread
            End Get
            Set(ByVal value As Double)
                _AwaySpread = value
            End Set
        End Property

        Public Property AwaySpreadMoney() As Double
            Get
                Return _AwaySpreadMoney
            End Get
            Set(ByVal value As Double)
                _AwaySpreadMoney = value
            End Set
        End Property

        Public Property HomeMoneyLine() As Double
            Get
                Return _HomeMoneyLine
            End Get
            Set(ByVal value As Double)
                _HomeMoneyLine = value
            End Set
        End Property

        Public Property AwayMoneyLine() As Double
            Get
                Return _AwayMoneyLine
            End Get
            Set(ByVal value As Double)
                _AwayMoneyLine = value
            End Set
        End Property

        Public Property TotalPoints() As Double
            Get
                Return _TotalPoints
            End Get
            Set(ByVal value As Double)
                _TotalPoints = value
            End Set
        End Property

        Public Property TotalPointsOverMoney() As Double
            Get
                Return _TotalPointsOverMoney
            End Get
            Set(ByVal value As Double)
                _TotalPointsOverMoney = value
            End Set
        End Property

        Public Property TotalPointsUnderMoney() As Double
            Get
                Return _TotalPointsUnderMoney
            End Get
            Set(ByVal value As Double)
                _TotalPointsUnderMoney = value
            End Set
        End Property

        Public Property ManualSetting() As Boolean
            Get
                Return _ManualSetting
            End Get
            Set(ByVal value As Boolean)
                _ManualSetting = value
            End Set
        End Property

        Public Property IsCircle() As Boolean
            Get
                Return _IsCircle
            End Get
            Set(ByVal value As Boolean)
                _IsCircle = value
            End Set
        End Property

        Public Property GameLineOff() As Boolean
            Get
                Return _GameLineOff
            End Get
            Set(ByVal value As Boolean)
                _GameLineOff = value
            End Set
        End Property
        Public Property DrawMoneyLine() As Double
            Get
                Return _DrawMoneyLine
            End Get
            Set(ByVal value As Double)
                _DrawMoneyLine = value
            End Set
        End Property

        Public Property AwayTeamTotalPoints() As Double

        Public Property AwayTeamTotalPointsOverMoney() As Double

        Public Property AwayTeamTotalPointsUnderMoney() As Double

        Public Property HomeTeamTotalPoints() As Double

        Public Property HomeTeamTotalPointsOverMoney() As Double

        Public Property HomeTeamTotalPointsUnderMoney() As Double

#End Region


    End Class

End Namespace

