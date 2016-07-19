
Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils

    <Serializable()> Public Class CCallCenterAgent
        Inherits CGeneralUser

#Region "Fields"

        Private _AlertMessage As String = ""
        Private _PasswordLastUpdated As Date
        Private _NumFailedAttempts As Integer = 0
        Private _ForgotPasswordTimestamp As Date
        Private _CreatedDate As Date
        Private _CreatedBy As String = ""
        Private _LastLoginDate As Date
        Private _PhoneExtension As String = ""
        Private _PlayerID As String = ""

#End Region

#Region "Constructor"

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            MyBase.New(podrData)

            _UserID = SafeString(podrData("CallCenterAgentID"))
            _AlertMessage = SafeString(podrData("AlertMessage"))
            _PasswordLastUpdated = SafeDate(podrData("PasswordLastUpdated"))
            _NumFailedAttempts = SafeInteger(podrData("NumFailedAttempts"))
            _ForgotPasswordTimestamp = SafeDate(podrData("ForgotPasswordTimestamp"))
            _CreatedDate = SafeDate(podrData("CreatedDate"))
            _CreatedBy = SafeString(podrData("CreatedBy"))
            _LastLoginDate = SafeDate(podrData("LastLoginDate"))
            _PhoneExtension = SafeString(podrData("PhoneExtension"))
        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property AlertMessage() As String
            Get
                Return _AlertMessage
            End Get
        End Property

        Public ReadOnly Property PasswordLastUpdated() As Date
            Get
                Return _PasswordLastUpdated
            End Get
        End Property

        Public ReadOnly Property NumFailedAttempts() As Integer
            Get
                Return _NumFailedAttempts
            End Get
        End Property

        Public ReadOnly Property ForgotPasswordTimestamp() As Date
            Get
                Return _ForgotPasswordTimestamp
            End Get
        End Property

        Public ReadOnly Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
        End Property

        Public ReadOnly Property CreatedBy() As String
            Get
                Return _CreatedBy
            End Get
        End Property

        Public ReadOnly Property LastLoginDate() As Date
            Get
                Return _LastLoginDate
            End Get
        End Property

        Public ReadOnly Property PhoneExtension() As String
            Get
                Return _PhoneExtension
            End Get
        End Property
        Public Property PlayerID() As String
            Get
                Return _PlayerID
            End Get
            Set(ByVal value As String)
                _PlayerID = SafeString(value)
            End Set
        End Property

#End Region

    End Class

End Namespace
