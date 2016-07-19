Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.DBUtils

Namespace CacheUtils

    <Serializable()> Public Class CPlayerTemplate

#Region "Fields"

        Private _PlayerTemplateID As String = ""
        Private _SuperAdminID As String = ""
        Private _LastSavedBy As String = ""
        Private _LastSavedDate As DateTime
        Private _TemplateName As String = ""
        Private _AccountBalance As Double = 0
        Private _CreditMaxAmount As Double = 0
        Private _CasinoMaxAmount As Double = 0
        Private _CreditMinBetPhone As Double = 0
        Private _CreditMinBetInternet As Double = 0
        Private _CreditWagerPerGame As Double = 0
        Private _CreditMaxParlay As Double = 0
        Private _CreditMaxTeaserParlay As Double = 0
        Private _CreditMaxReverseActionParlay As Double = 0

        Private _Limits As New CPlayerTemplateLimitList
        Private _PlayerJuice As New CPlayerJuiceList

        Private _Max1Q As Double = 0
        Private _Max2Q As Double = 0
        Private _Max3Q As Double = 0
        Private _Max4Q As Double = 0
        Private _Max1H As Double = 0
        Private _Max2H As Double = 0
        Private _MaxSingle As Double = 0
        Private _IsDefaultPlayerTemplate As String

#End Region

#Region "Constructors"

        Public Sub New()
            _PlayerTemplateID = newGUID()
        End Sub

        Public Sub New(ByVal podrData As DataRow)

            _PlayerTemplateID = SafeString(podrData("PlayerTemplateID"))
            _SuperAdminID = SafeString(podrData("SuperAdminID"))
            _LastSavedBy = SafeString(podrData("LastSavedBy"))
            _LastSavedDate = SafeDate(podrData("LastSavedDate"))
            _TemplateName = SafeString(podrData("TemplateName"))
            _AccountBalance = SafeDouble(podrData("AccountBalance"))
            _CreditMaxAmount = SafeDouble(podrData("CreditMaxAmount"))
            _CasinoMaxAmount = SafeDouble(podrData("CasinoMaxAmount"))
            _CreditMinBetPhone = SafeDouble(podrData("CreditMinBetPhone"))
            _CreditMinBetInternet = SafeDouble(podrData("CreditMinBetInternet"))
            _CreditWagerPerGame = SafeDouble(podrData("CreditWagerPerGame"))
            _CreditMaxParlay = SafeDouble(podrData("CreditMaxParlay"))
            _CreditMaxTeaserParlay = SafeDouble(podrData("CreditMaxTeaserParlay"))
            _CreditMaxReverseActionParlay = SafeDouble(podrData("CreditMaxReverseActionParlay"))

            _Limits = (New CPlayerTemplateLimitManager).GetPlayerTemplates(_PlayerTemplateID)
            _PlayerJuice = (New CPlayerJuiceManager).GetPlayerJuice(_PlayerTemplateID)
            _Max1Q = SafeDouble(podrData("Max1Q"))
            _Max2Q = SafeDouble(podrData("Max2Q"))
            _Max3Q = SafeDouble(podrData("Max3Q"))
            _Max4Q = SafeDouble(podrData("Max4Q"))
            _Max1H = SafeDouble(podrData("Max1H"))
            _Max2H = SafeDouble(podrData("Max2H"))
            _MaxSingle = SafeDouble(podrData("MaxSingle"))
            _IsDefaultPlayerTemplate = SafeString(podrData("IsDefaultPlayerTemplate"))
        End Sub

#End Region

#Region "Properties"

        Public Property PlayerTemplateID() As String
            Get
                Return _PlayerTemplateID
            End Get
            Set(ByVal value As String)
                _PlayerTemplateID = value
            End Set
        End Property

        Public Property SuperAdminID() As String
            Get
                Return _SuperAdminID
            End Get
            Set(ByVal value As String)
                _SuperAdminID = value
            End Set
        End Property

        Public Property LastSaveBy() As String
            Get
                Return _LastSavedBy
            End Get
            Set(ByVal value As String)
                _LastSavedBy = value
            End Set
        End Property

        Public Property LastSavedDate() As Date
            Get
                Return _LastSavedDate
            End Get
            Set(ByVal value As Date)
                _LastSavedDate = value
            End Set
        End Property

        Public Property TemplateName() As String
            Get
                Return _TemplateName
            End Get
            Set(ByVal value As String)
                _TemplateName = value
            End Set
        End Property

        Public Property AccountBalance() As Double
            Get
                Return _AccountBalance
            End Get
            Set(ByVal value As Double)
                _AccountBalance = value
            End Set
        End Property

        Public Property CreditMaxAmount() As Double
            Get
                Return _CreditMaxAmount
            End Get
            Set(ByVal value As Double)
                _CreditMaxAmount = value
            End Set
        End Property

        Public Property CasinoMaxAmount() As Double
            Get
                Return _CasinoMaxAmount
            End Get
            Set(ByVal value As Double)
                _CasinoMaxAmount = value
            End Set
        End Property

        Public Property CreditMinBetPhone() As Double
            Get
                Return _CreditMinBetPhone
            End Get
            Set(ByVal value As Double)
                _CreditMinBetPhone = value
            End Set
        End Property

        Public Property CreditMinBetInternet() As Double
            Get
                Return _CreditMinBetInternet
            End Get
            Set(ByVal value As Double)
                _CreditMinBetInternet = value
            End Set
        End Property

        Public Property CreditWagerPerGame() As Double
            Get
                Return _CreditWagerPerGame
            End Get
            Set(ByVal value As Double)
                _CreditWagerPerGame = value
            End Set
        End Property

        Public Property CreditMaxParlay() As Double
            Get
                Return _CreditMaxParlay
            End Get
            Set(ByVal value As Double)
                _CreditMaxParlay = value
            End Set
        End Property

        Public Property CreditMaxTeaserParlay() As Double
            Get
                Return _CreditMaxTeaserParlay
            End Get
            Set(ByVal value As Double)
                _CreditMaxTeaserParlay = value
            End Set
        End Property

        Public Property CreditMaxReverseActionParlay() As Double
            Get
                Return _CreditMaxReverseActionParlay
            End Get
            Set(ByVal value As Double)
                _CreditMaxReverseActionParlay = value
            End Set
        End Property

        Public ReadOnly Property Limits() As CPlayerTemplateLimitList
            Get
                Return _Limits
            End Get
        End Property

        Public ReadOnly Property Limits(ByVal psGameType As String) As CPlayerTemplateLimitList
            Get
                Return _Limits.GetLimits(psGameType)
            End Get
        End Property

        Public ReadOnly Property PlayerJuice() As CPlayerJuiceList
            Get
                Return _PlayerJuice
            End Get
        End Property

        Public Property Max1Q() As Double
            Get
                Return _Max1Q
            End Get
            Set(ByVal value As Double)
                _Max1Q = value
            End Set
        End Property

        Public Property Max2Q() As Double
            Get
                Return _Max2Q
            End Get
            Set(ByVal value As Double)
                _Max2Q = value
            End Set
        End Property

        Public Property Max3Q() As Double
            Get
                Return _Max3Q
            End Get
            Set(ByVal value As Double)
                _Max3Q = value
            End Set
        End Property

        Public Property Max4Q() As Double
            Get
                Return _Max4Q
            End Get
            Set(ByVal value As Double)
                _Max4Q = value
            End Set
        End Property

        Public Property Max1H() As Double
            Get
                Return _Max1H
            End Get
            Set(ByVal value As Double)
                _Max1H = value
            End Set
        End Property

        Public Property Max2H() As Double
            Get
                Return _Max2H
            End Get
            Set(ByVal value As Double)
                _Max2H = value
            End Set
        End Property

        Public Property MaxSingle() As Double
            Get
                Return _MaxSingle
            End Get
            Set(ByVal value As Double)
                _MaxSingle = value
            End Set
        End Property

        Public ReadOnly Property IsDefaultPlayerTemplate() As String
            Get
                Return _IsDefaultPlayerTemplate
            End Get
        End Property

#End Region

    End Class

    <Serializable()> Public Class CPlayerTemplateList
        Inherits List(Of CPlayerTemplate)

    End Class

End Namespace

