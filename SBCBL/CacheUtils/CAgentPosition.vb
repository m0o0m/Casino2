Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils

    <Serializable()> Public Class CAgentPosition

#Region "Property"

        Private _TransactionDate As Date
        Private _GameType As String
        Private _HomeTeam As String
        Private _AwayTeam As String
        Private _AwayRotationNumber As Integer
        Private _HomeRotationNumber As Integer
        Private _BetType As String
        Private _Bet As Integer
        Private _RiskAmount As Double
        Private _WinAmount As Double
    
        Private _BetCurrentSpreadAway As Integer
        Private _Bet1HSpreadAway As Integer
        Private _Bet2HSpreadAway As Integer
        Private _Bet1QSpreadAway As Integer
        Private _Bet2QSpreadAway As Integer
        Private _Bet3QSpreadAway As Integer
        Private _Bet4QSpreadAway As Integer

        Private _BetCurrentSpreadHome As Integer
        Private _Bet1HSpreadHome As Integer
        Private _Bet2HSpreadHome As Integer
        Private _Bet1QSpreadHome As Integer
        Private _Bet2QSpreadHome As Integer
        Private _Bet3QSpreadHome As Integer
        Private _Bet4QSpreadHome As Integer

        Private _BetCurrentTotalPointAway As Integer
        Private _Bet1HTotalPointAway As Integer
        Private _Bet2HTotalPointAway As Integer
        Private _Bet1QTotalPointAway As Integer
        Private _Bet2QTotalPointAway As Integer
        Private _Bet3QTotalPointAway As Integer
        Private _Bet4QTotalPointAway As Integer

        Private _BetCurrentTotalPointHome As Integer
        Private _Bet1HTotalPointHome As Integer
        Private _Bet2HTotalPointHome As Integer
        Private _Bet1QTotalPointHome As Integer
        Private _Bet2QTotalPointHome As Integer
        Private _Bet3QTotalPointHome As Integer
        Private _Bet4QTotalPointHome As Integer

        Private _BetCurrentMLAway As Integer
        Private _Bet1HMLAway As Integer
        Private _Bet2HMLAway As Integer
        Private _Bet1QMLAway As Integer
        Private _Bet2QMLAway As Integer
        Private _Bet3QMLAway As Integer
        Private _Bet4QMLAway As Integer

        Private _BetCurrentMLHome As Integer
        Private _Bet1HMLHome As Integer
        Private _Bet2HMLHome As Integer
        Private _Bet1QMLHome As Integer
        Private _Bet2QMLHome As Integer
        Private _Bet3QMLHome As Integer
        Private _Bet4QMLHome As Integer

        Private _RiskAmount1H As Double
        Private _WinAmount1H As Double
        Private _RiskAmount2H As Double
        Private _WinAmount2H As Double
        Private _RiskAmount1Q As Double
        Private _WinAmount1Q As Double
        Private _RiskAmount2Q As Double
        Private _WinAmount2Q As Double
        Private _RiskAmount3Q As Double
        Private _WinAmount3Q As Double
        Private _RiskAmount4Q As Double
        Private _WinAmount4Q As Double

        Private _RiskAmountCurrentSpreadAway As Double
        Private _WinAmountCurrentSpreadAway As Double
        Private _RiskAmount1HSpreadAway As Double
        Private _WinAmount1HSpreadAway As Double
        Private _RiskAmount2HSpreadAway As Double
        Private _WinAmount2HSpreadAway As Double
        Private _RiskAmount1QSpreadAway As Double
        Private _WinAmount1QSpreadAway As Double
        Private _RiskAmount2QSpreadAway As Double
        Private _WinAmount2QSpreadAway As Double
        Private _RiskAmount3QSpreadAway As Double
        Private _WinAmount3QSpreadAway As Double
        Private _RiskAmount4QSpreadAway As Double
        Private _WinAmount4QSpreadAway As Double

        Private _RiskAmountCurrentSpreadHome As Double
        Private _WinAmountCurrentSpreadHome As Double
        Private _RiskAmount1HSpreadHome As Double
        Private _WinAmount1HSpreadHome As Double
        Private _RiskAmount2HSpreadHome As Double
        Private _WinAmount2HSpreadHome As Double
        Private _RiskAmount1QSpreadHome As Double
        Private _WinAmount1QSpreadHome As Double
        Private _RiskAmount2QSpreadHome As Double
        Private _WinAmount2QSpreadHome As Double
        Private _RiskAmount3QSpreadHome As Double
        Private _WinAmount3QSpreadHome As Double
        Private _RiskAmount4QSpreadHome As Double
        Private _WinAmount4QSpreadHome As Double

        Private _RiskAmountCurrentTotalPointAway As Double
        Private _WinAmountCurrentTotalPointAway As Double
        Private _RiskAmount1HTotalPointAway As Double
        Private _WinAmount1HTotalPointAway As Double
        Private _RiskAmount2HTotalPointAway As Double
        Private _WinAmount2HTotalPointAway As Double
        Private _RiskAmount1QTotalPointAway As Double
        Private _WinAmount1QTotalPointAway As Double
        Private _RiskAmount2QTotalPointAway As Double
        Private _WinAmount2QTotalPointAway As Double
        Private _RiskAmount3QTotalPointAway As Double
        Private _WinAmount3QTotalPointAway As Double
        Private _RiskAmount4QTotalPointAway As Double
        Private _WinAmount4QTotalPointAway As Double

        Private _RiskAmountCurrentTotalPointHome As Double
        Private _WinAmountCurrentTotalPointHome As Double
        Private _RiskAmount1HTotalPointHome As Double
        Private _WinAmount1HTotalPointHome As Double
        Private _RiskAmount2HTotalPointHome As Double
        Private _WinAmount2HTotalPointHome As Double
        Private _RiskAmount1QTotalPointHome As Double
        Private _WinAmount1QTotalPointHome As Double
        Private _RiskAmount2QTotalPointHome As Double
        Private _WinAmount2QTotalPointHome As Double
        Private _RiskAmount3QTotalPointHome As Double
        Private _WinAmount3QTotalPointHome As Double
        Private _RiskAmount4QTotalPointHome As Double
        Private _WinAmount4QTotalPointHome As Double

        Private _RiskAmountCurrentMLAway As Double
        Private _WinAmountCurrentMLAway As Double
        Private _RiskAmount1HMLAway As Double
        Private _WinAmount1HMLAway As Double
        Private _RiskAmount2HMLAway As Double
        Private _WinAmount2HMLAway As Double
        Private _RiskAmount1QMLAway As Double
        Private _WinAmount1QMLAway As Double
        Private _RiskAmount2QMLAway As Double
        Private _WinAmount2QMLAway As Double
        Private _RiskAmount3QMLAway As Double
        Private _WinAmount3QMLAway As Double
        Private _RiskAmount4QMLAway As Double
        Private _WinAmount4QMLAway As Double

        Private _RiskAmountCurrentMLHome As Double
        Private _WinAmountCurrentMLHome As Double
        Private _RiskAmount1HMLHome As Double
        Private _WinAmount1HMLHome As Double
        Private _RiskAmount2HMLHome As Double
        Private _WinAmount2HMLHome As Double
        Private _RiskAmount1QMLHome As Double
        Private _WinAmount1QMLHome As Double
        Private _RiskAmount2QMLHome As Double
        Private _WinAmount2QMLHome As Double
        Private _RiskAmount3QMLHome As Double
        Private _WinAmount3QMLHome As Double
        Private _RiskAmount4QMLHome As Double
        Private _WinAmount4QMLHome As Double

        Private _Context As String

#End Region

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _TransactionDate = SafeDate(podrData("TransactionDate"))
            _GameType = SafeString(podrData("GameType"))
            _HomeTeam = SafeString(podrData("HomeTeam"))
            _AwayTeam = SafeString(podrData("AwayTeam"))
            _AwayRotationNumber = SafeInteger(podrData("AwayRotationNumber"))
            _HomeRotationNumber = SafeInteger(podrData("HomeRotationNumber"))
            _BetType = SafeString(podrData("BetType"))
            ' _Bet = SafeInteger(podrData("Bet"))
            _RiskAmount = SafeDouble(podrData("RiskAmount"))
            _WinAmount = SafeDouble(podrData("WinAmount"))
            _Context = SafeString(podrData("Context"))
        End Sub

        Public ReadOnly Property Context() As String
            Get
                Return _Context
            End Get
        End Property

        Public Property RiskAmountCurrentSpreadAway() As Double
            Get
                Return _RiskAmountCurrentSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount1HSpreadAway() As Double
            Get
                Return _RiskAmount1HSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount2HSpreadAway() As Double
            Get
                Return _RiskAmount2HSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount1QSpreadAway() As Double
            Get
                Return _RiskAmount1QSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount2QSpreadAway() As Double
            Get
                Return _RiskAmount2QSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount3QSpreadAway() As Double
            Get
                Return _RiskAmount3QSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QSpreadAway = value
            End Set
        End Property

        Public Property RiskAmount4QSpreadAway() As Double
            Get
                Return _RiskAmount4QSpreadAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QSpreadAway = value
            End Set
        End Property

        Public Property RiskAmountCurrentSpreadHome() As Double
            Get
                Return _RiskAmountCurrentSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount1HSpreadHome() As Double
            Get
                Return _RiskAmount1HSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount2HSpreadHome() As Double
            Get
                Return _RiskAmount2HSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount1QSpreadHome() As Double
            Get
                Return _RiskAmount1QSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount2QSpreadHome() As Double
            Get
                Return _RiskAmount2QSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount3QSpreadHome() As Double
            Get
                Return _RiskAmount3QSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QSpreadHome = value
            End Set
        End Property

        Public Property RiskAmount4QSpreadHome() As Double
            Get
                Return _RiskAmount4QSpreadHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QSpreadHome = value
            End Set
        End Property

        ''' <summary>
        ''' total
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property WinAmountCurrentSpreadAway() As Double
            Get
                Return _WinAmountCurrentSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentSpreadAway = value
            End Set
        End Property

        Public Property WinAmount1HSpreadAway() As Double
            Get
                Return _WinAmount1HSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1HSpreadAway = value
            End Set
        End Property

        Public Property WinAmount2HSpreadAway() As Double
            Get
                Return _WinAmount2HSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2HSpreadAway = value
            End Set
        End Property

        Public Property WinAmount1QSpreadAway() As Double
            Get
                Return _WinAmount1QSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1QSpreadAway = value
            End Set
        End Property

        Public Property WinAmount2QSpreadAway() As Double
            Get
                Return _WinAmount2QSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2QSpreadAway = value
            End Set
        End Property

        Public Property WinAmount3QSpreadAway() As Double
            Get
                Return _WinAmount3QSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount3QSpreadAway = value
            End Set
        End Property

        Public Property WinAmount4QSpreadAway() As Double
            Get
                Return _WinAmount4QSpreadAway
            End Get
            Set(ByVal value As Double)
                _WinAmount4QSpreadAway = value
            End Set
        End Property


        Public Property WinAmountCurrentSpreadHome() As Double
            Get
                Return _WinAmountCurrentSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentSpreadHome = value
            End Set
        End Property

        Public Property WinAmount1HSpreadHome() As Double
            Get
                Return _WinAmount1HSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1HSpreadHome = value
            End Set
        End Property

        Public Property WinAmount2HSpreadHome() As Double
            Get
                Return _WinAmount2HSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2HSpreadHome = value
            End Set
        End Property

        Public Property WinAmount1QSpreadHome() As Double
            Get
                Return _WinAmount1QSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1QSpreadHome = value
            End Set
        End Property

        Public Property WinAmount2QSpreadHome() As Double
            Get
                Return _WinAmount2QSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2QSpreadHome = value
            End Set
        End Property

        Public Property WinAmount3QSpreadHome() As Double
            Get
                Return _WinAmount3QSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount3QSpreadHome = value
            End Set
        End Property

        Public Property WinAmount4QSpreadHome() As Double
            Get
                Return _WinAmount4QSpreadHome
            End Get
            Set(ByVal value As Double)
                _WinAmount4QSpreadHome = value
            End Set
        End Property

        Public Property WinAmountCurrentTotalPointAway() As Double
            Get
                Return _WinAmountCurrentTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount1HTotalPointAway() As Double
            Get
                Return _WinAmount1HTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1HTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount2HTotalPointAway() As Double
            Get
                Return _WinAmount2HTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2HTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount1QTotalPointAway() As Double
            Get
                Return _WinAmount1QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1QTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount2QTotalPointAway() As Double
            Get
                Return _WinAmount2QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2QTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount3QTotalPointAway() As Double
            Get
                Return _WinAmount3QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount3QTotalPointAway = value
            End Set
        End Property

        Public Property WinAmount4QTotalPointAway() As Double
            Get
                Return _WinAmount4QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _WinAmount4QTotalPointAway = value
            End Set
        End Property


        Public Property WinAmountCurrentTotalPointHome() As Double
            Get
                Return _WinAmountCurrentTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount1HTotalPointHome() As Double
            Get
                Return _WinAmount1HTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1HTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount2HTotalPointHome() As Double
            Get
                Return _WinAmount2HTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2HTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount1QTotalPointHome() As Double
            Get
                Return _WinAmount1QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1QTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount2QTotalPointHome() As Double
            Get
                Return _WinAmount2QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2QTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount3QTotalPointHome() As Double
            Get
                Return _WinAmount3QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount3QTotalPointHome = value
            End Set
        End Property

        Public Property WinAmount4QTotalPointHome() As Double
            Get
                Return _WinAmount4QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _WinAmount4QTotalPointHome = value
            End Set
        End Property

        '''''money line '''''''

        Public Property RiskAmountCurrentMLAway() As Double
            Get
                Return _RiskAmountCurrentMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentMLAway = value
            End Set
        End Property

        Public Property RiskAmount1HMLAway() As Double
            Get
                Return _RiskAmount1HMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HMLAway = value
            End Set
        End Property

        Public Property RiskAmount2HMLAway() As Double
            Get
                Return _RiskAmount2HMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HMLAway = value
            End Set
        End Property

        Public Property RiskAmount1QMLAway() As Double
            Get
                Return _RiskAmount1QMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QMLAway = value
            End Set
        End Property

        Public Property RiskAmount2QMLAway() As Double
            Get
                Return _RiskAmount2QMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QMLAway = value
            End Set
        End Property

        Public Property RiskAmount3QMLAway() As Double
            Get
                Return _RiskAmount3QMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QMLAway = value
            End Set
        End Property

        Public Property RiskAmount4QMLAway() As Double
            Get
                Return _RiskAmount4QMLAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QMLAway = value
            End Set
        End Property

        Public Property RiskAmountCurrentMLHome() As Double
            Get
                Return _RiskAmountCurrentMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentMLHome = value
            End Set
        End Property

        Public Property RiskAmount1HMLHome() As Double
            Get
                Return _RiskAmount1HMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HMLHome = value
            End Set
        End Property

        Public Property RiskAmount2HMLHome() As Double
            Get
                Return _RiskAmount2HMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HMLHome = value
            End Set
        End Property

        Public Property RiskAmount1QMLHome() As Double
            Get
                Return _RiskAmount1QMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QMLHome = value
            End Set
        End Property

        Public Property RiskAmount2QMLHome() As Double
            Get
                Return _RiskAmount2QMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QMLHome = value
            End Set
        End Property

        Public Property RiskAmount3QMLHome() As Double
            Get
                Return _RiskAmount3QMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QMLHome = value
            End Set
        End Property

        Public Property RiskAmount4QMLHome() As Double
            Get
                Return _RiskAmount4QMLHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QMLHome = value
            End Set
        End Property

        Public Property BetCurrentSpreadAway() As Integer
            Get
                Return _BetCurrentSpreadAway
            End Get
            Set(ByVal value As Integer)
                _BetCurrentSpreadAway = value
            End Set
        End Property

        Public Property Bet1HSpreadAway() As Integer
            Get
                Return _Bet1HSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet1HSpreadAway = value
            End Set
        End Property

        Public Property Bet2HSpreadAway() As Integer
            Get
                Return _Bet2HSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet2HSpreadAway = value
            End Set
        End Property

        Public Property Bet1QSpreadAway() As Integer
            Get
                Return _Bet1QSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet1QSpreadAway = value
            End Set
        End Property

        Public Property Bet2QSpreadAway() As Integer
            Get
                Return _Bet2QSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet2QSpreadAway = value
            End Set
        End Property

        Public Property Bet3QSpreadAway() As Integer
            Get
                Return _Bet3QSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet3QSpreadAway = value
            End Set
        End Property

        Public Property Bet4QSpreadAway() As Integer
            Get
                Return _Bet4QSpreadAway
            End Get
            Set(ByVal value As Integer)
                _Bet4QSpreadAway = value
            End Set
        End Property

        Public Property BetCurrentSpreadHome() As Integer
            Get
                Return _BetCurrentSpreadHome
            End Get
            Set(ByVal value As Integer)
                _BetCurrentSpreadHome = value
            End Set
        End Property

        Public Property Bet1HSpreadHome() As Integer
            Get
                Return _Bet1HSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet1HSpreadHome = value
            End Set
        End Property

        Public Property Bet2HSpreadHome() As Integer
            Get
                Return _Bet2HSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet2HSpreadHome = value
            End Set
        End Property

        Public Property Bet1QSpreadHome() As Integer
            Get
                Return _Bet1QSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet1QSpreadHome = value
            End Set
        End Property

        Public Property Bet2QSpreadHome() As Integer
            Get
                Return _Bet2QSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet2QSpreadHome = value
            End Set
        End Property

        Public Property Bet3QSpreadHome() As Integer
            Get
                Return _Bet3QSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet3QSpreadHome = value
            End Set
        End Property

        Public Property Bet4QSpreadHome() As Integer
            Get
                Return _Bet4QSpreadHome
            End Get
            Set(ByVal value As Integer)
                _Bet4QSpreadHome = value
            End Set
        End Property

        Public Property BetCurrentTotalPointAway() As Integer
            Get
                Return _BetCurrentTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _BetCurrentTotalPointAway = value
            End Set
        End Property

        Public Property Bet1HTotalPointAway() As Integer
            Get
                Return _Bet1HTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet1HTotalPointAway = value
            End Set
        End Property

        Public Property Bet2HTotalPointAway() As Integer
            Get
                Return _Bet2HTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet2HTotalPointAway = value
            End Set
        End Property

        Public Property Bet1QTotalPointAway() As Integer
            Get
                Return _Bet1QTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet1QTotalPointAway = value
            End Set
        End Property

        Public Property Bet2QTotalPointAway() As Integer
            Get
                Return _Bet2QTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet2QTotalPointAway = value
            End Set
        End Property

        Public Property Bet3QTotalPointAway() As Integer
            Get
                Return _Bet3QTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet3QTotalPointAway = value
            End Set
        End Property

        Public Property Bet4QTotalPointAway() As Integer
            Get
                Return _Bet4QTotalPointAway
            End Get
            Set(ByVal value As Integer)
                _Bet4QTotalPointAway = value
            End Set
        End Property

        Public Property BetCurrentTotalPointHome() As Integer
            Get
                Return _BetCurrentTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _BetCurrentTotalPointHome = value
            End Set
        End Property

        Public Property Bet1HTotalPointHome() As Integer
            Get
                Return _Bet1HTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet1HTotalPointHome = value
            End Set
        End Property

        Public Property Bet2HTotalPointHome() As Integer
            Get
                Return _Bet2HTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet2HTotalPointHome = value
            End Set
        End Property

        Public Property Bet1QTotalPointHome() As Integer
            Get
                Return _Bet1QTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet1QTotalPointHome = value
            End Set
        End Property

        Public Property Bet2QTotalPointHome() As Integer
            Get
                Return _Bet2QTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet2QTotalPointHome = value
            End Set
        End Property

        Public Property Bet3QTotalPointHome() As Integer
            Get
                Return _Bet3QTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet3QTotalPointHome = value
            End Set
        End Property

        Public Property Bet4QTotalPointHome() As Integer
            Get
                Return _Bet4QTotalPointHome
            End Get
            Set(ByVal value As Integer)
                _Bet4QTotalPointHome = value
            End Set
        End Property


        Public Property BetCurrentMLAway() As Integer
            Get
                Return _BetCurrentMLAway
            End Get
            Set(ByVal value As Integer)
                _BetCurrentMLAway = value
            End Set
        End Property

        Public Property Bet1HMLAway() As Integer
            Get
                Return _Bet1HMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet1HMLAway = value
            End Set
        End Property

        Public Property Bet2HMLAway() As Integer
            Get
                Return _Bet2HMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet2HMLAway = value
            End Set
        End Property

        Public Property Bet1QMLAway() As Integer
            Get
                Return _Bet1QMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet1QMLAway = value
            End Set
        End Property

        Public Property Bet2QMLAway() As Integer
            Get
                Return _Bet2QMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet2QMLAway = value
            End Set
        End Property

        Public Property Bet3QMLAway() As Integer
            Get
                Return _Bet3QMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet3QMLAway = value
            End Set
        End Property

        Public Property Bet4QMLAway() As Integer
            Get
                Return _Bet4QMLAway
            End Get
            Set(ByVal value As Integer)
                _Bet4QMLAway = value
            End Set
        End Property

        Public Property BetCurrentMLHome() As Integer
            Get
                Return _BetCurrentMLHome
            End Get
            Set(ByVal value As Integer)
                _BetCurrentMLHome = value
            End Set
        End Property

        Public Property Bet1HMLHome() As Integer
            Get
                Return _Bet1HMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet1HMLHome = value
            End Set
        End Property

        Public Property Bet2HMLHome() As Integer
            Get
                Return _Bet2HMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet2HMLHome = value
            End Set
        End Property

        Public Property Bet1QMLHome() As Integer
            Get
                Return _Bet1QMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet1QMLHome = value
            End Set
        End Property

        Public Property Bet2QMLHome() As Integer
            Get
                Return _Bet2QMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet2QMLHome = value
            End Set
        End Property

        Public Property Bet3QMLHome() As Integer
            Get
                Return _Bet3QMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet3QMLHome = value
            End Set
        End Property

        Public Property Bet4QMLHome() As Integer
            Get
                Return _Bet4QMLHome
            End Get
            Set(ByVal value As Integer)
                _Bet4QMLHome = value
            End Set
        End Property


        Public Property RiskAmountCurrentTotalPointAway() As Double
            Get
                Return _RiskAmountCurrentTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount1HTotalPointAway() As Double
            Get
                Return _RiskAmount1HTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount2HTotalPointAway() As Double
            Get
                Return _RiskAmount2HTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount1QTotalPointAway() As Double
            Get
                Return _RiskAmount1QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount2QTotalPointAway() As Double
            Get
                Return _RiskAmount2QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount3QTotalPointAway() As Double
            Get
                Return _RiskAmount3QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmount4QTotalPointAway() As Double
            Get
                Return _RiskAmount4QTotalPointAway
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QTotalPointAway = value
            End Set
        End Property

        Public Property RiskAmountCurrentTotalPointHome() As Double
            Get
                Return _RiskAmountCurrentTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmountCurrentTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount1HTotalPointHome() As Double
            Get
                Return _RiskAmount1HTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1HTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount2HTotalPointHome() As Double
            Get
                Return _RiskAmount2HTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2HTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount1QTotalPointHome() As Double
            Get
                Return _RiskAmount1QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount1QTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount2QTotalPointHome() As Double
            Get
                Return _RiskAmount2QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount2QTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount3QTotalPointHome() As Double
            Get
                Return _RiskAmount3QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount3QTotalPointHome = value
            End Set
        End Property

        Public Property RiskAmount4QTotalPointHome() As Double
            Get
                Return _RiskAmount4QTotalPointHome
            End Get
            Set(ByVal value As Double)
                _RiskAmount4QTotalPointHome = value
            End Set
        End Property

        ''' <summary>
        ''' ''''''''Mline
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property WinAmountCurrentMLAway() As Double
            Get
                Return _WinAmountCurrentMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentMLAway = value
            End Set
        End Property

        Public Property WinAmount1HMLAway() As Double
            Get
                Return _WinAmount1HMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1HMLAway = value
            End Set
        End Property

        Public Property WinAmount2HMLAway() As Double
            Get
                Return _WinAmount2HMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2HMLAway = value
            End Set
        End Property

        Public Property WinAmount1QMLAway() As Double
            Get
                Return _WinAmount1QMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount1QMLAway = value
            End Set
        End Property

        Public Property WinAmount2QMLAway() As Double
            Get
                Return _WinAmount2QMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount2QMLAway = value
            End Set
        End Property

        Public Property WinAmount3QMLAway() As Double
            Get
                Return _WinAmount3QMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount3QMLAway = value
            End Set
        End Property

        Public Property WinAmount4QMLAway() As Double
            Get
                Return _WinAmount4QMLAway
            End Get
            Set(ByVal value As Double)
                _WinAmount4QMLAway = value
            End Set
        End Property


        Public Property WinAmountCurrentMLHome() As Double
            Get
                Return _WinAmountCurrentMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmountCurrentMLHome = value
            End Set
        End Property

        Public Property WinAmount1HMLHome() As Double
            Get
                Return _WinAmount1HMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1HMLHome = value
            End Set
        End Property

        Public Property WinAmount2HMLHome() As Double
            Get
                Return _WinAmount2HMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2HMLHome = value
            End Set
        End Property

        Public Property WinAmount1QMLHome() As Double
            Get
                Return _WinAmount1QMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount1QMLHome = value
            End Set
        End Property

        Public Property WinAmount2QMLHome() As Double
            Get
                Return _WinAmount2QMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount2QMLHome = value
            End Set
        End Property

        Public Property WinAmount3QMLHome() As Double
            Get
                Return _WinAmount3QMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount3QMLHome = value
            End Set
        End Property

        Public Property WinAmount4QMLHome() As Double
            Get
                Return _WinAmount4QMLHome
            End Get
            Set(ByVal value As Double)
                _WinAmount4QMLHome = value
            End Set
        End Property

      

        Public Property RiskAmount1H() As Double
            Get
                Return _RiskAmount1H
            End Get
            Set(ByVal value As Double)
                _RiskAmount1H = value
            End Set
        End Property

        Public Property RiskAmount2H() As Double
            Get
                Return _RiskAmount2H
            End Get
            Set(ByVal value As Double)
                _RiskAmount2H = value
            End Set
        End Property

        Public Property RiskAmount1Q() As Double
            Get
                Return _RiskAmount1Q
            End Get
            Set(ByVal value As Double)
                _RiskAmount1Q = value
            End Set
        End Property

        Public Property RiskAmount2Q() As Double
            Get
                Return _RiskAmount2Q
            End Get
            Set(ByVal value As Double)
                _RiskAmount2Q = value
            End Set
        End Property

        Public Property RiskAmount3Q() As Double
            Get
                Return _RiskAmount3Q
            End Get
            Set(ByVal value As Double)
                _RiskAmount3Q = value
            End Set
        End Property

        Public Property RiskAmount4Q() As Double
            Get
                Return _RiskAmount4Q
            End Get
            Set(ByVal value As Double)
                _RiskAmount4Q = value
            End Set
        End Property

        Public Property WinAmount1H() As Double
            Get
                Return _WinAmount1H
            End Get
            Set(ByVal value As Double)
                _WinAmount1H = value
            End Set
        End Property

        Public Property WinAmount2H() As Double
            Get
                Return _WinAmount2H
            End Get
            Set(ByVal value As Double)
                _WinAmount2H = value
            End Set
        End Property

        Public Property WinAmount1Q() As Double
            Get
                Return _WinAmount1Q
            End Get
            Set(ByVal value As Double)
                _WinAmount1Q = value
            End Set
        End Property

        Public Property WinAmount2Q() As Double
            Get
                Return _WinAmount2Q
            End Get
            Set(ByVal value As Double)
                _WinAmount2Q = value
            End Set
        End Property

        Public Property WinAmount3Q() As Double
            Get
                Return _WinAmount3Q
            End Get
            Set(ByVal value As Double)
                _WinAmount3Q = value
            End Set
        End Property

        Public Property WinAmount4Q() As Double
            Get
                Return _WinAmount4Q
            End Get
            Set(ByVal value As Double)
                _WinAmount4Q = value
            End Set
        End Property


        Public ReadOnly Property TransactionDate() As Date
            Get
                Return _TransactionDate
            End Get
        End Property

        Public ReadOnly Property GameType() As String
            Get
                Return _GameType
            End Get
        End Property

        Public ReadOnly Property HomeTeam() As String
            Get
                Return _HomeTeam
            End Get
        End Property

        Public ReadOnly Property AwayTeam() As String
            Get
                Return _AwayTeam
            End Get
        End Property

        Public ReadOnly Property AwayRotationNumber() As Integer
            Get
                Return _AwayRotationNumber
            End Get
        End Property

        Public ReadOnly Property HomeRotationNumber() As Integer
            Get
                Return _HomeRotationNumber
            End Get
        End Property

        Public ReadOnly Property BetType() As String
            Get
                Return _BetType
            End Get
        End Property

        Public ReadOnly Property Bet() As Integer
            Get
                Return _Bet
            End Get
        End Property

        Public ReadOnly Property RiskAmount() As Double
            Get
                Return _RiskAmount
            End Get
        End Property

        Public ReadOnly Property WinAmount() As Double
            Get
                Return _WinAmount
            End Get
        End Property

    End Class
End Namespace
