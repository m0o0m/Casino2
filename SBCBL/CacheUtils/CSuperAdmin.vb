Imports SBCBL.std

Namespace CacheUtils

    <Serializable()> Public Class CSuperAdmin
        Inherits CGeneralUser

#Region "Fields"

        Private _IsMananger As Boolean = False
        Private _ChargePerAccount As Double = 0
        Private _IsPartner As Boolean = False
        Private _PartnerOf As String = ""
        Private _Wagering As String = ""
        Private _CustomerService As String = ""
        Private _SiteURL As String = ""
#End Region

#Region "Constructors"

        Public Sub New()
            'we used to enforce the pbIsSystem but now we allow creating empty cuserinfo objects
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            MyBase.New(podrData)
            _SiteURL = SafeString(podrData("SiteURL"))
            _UserID = SafeString(podrData("SuperAdminID"))
            _IsMananger = SafeString(podrData("IsManager")) = "Y"
            _IsPartner = SafeString(podrData("IsPartner")) = "Y"
            _PartnerOf = SafeString(podrData("PartnerOf"))
            _ChargePerAccount = SafeDouble(podrData("ChargePerAccount"))
            _Wagering = SafeString(podrData("Wagering"))
            _CustomerService = SafeString(podrData("CustomerService"))
        End Sub

#End Region

#Region "Properties"
        Public ReadOnly Property SiteURL() As String
            Get
                Return _SiteURL
            End Get
        End Property

        Public ReadOnly Property IsManager() As Boolean
            Get
                Return _IsMananger
            End Get
        End Property

        Public ReadOnly Property IsPartner() As Boolean
            Get
                Return _IsPartner
            End Get
        End Property

        Public ReadOnly Property PartnerOf() As String
            Get
                Return _PartnerOf
            End Get
        End Property

        Public ReadOnly Property ChargePerAccount() As Double
            Get
                Return _ChargePerAccount
            End Get
        End Property

        Public ReadOnly Property CustomerService() As String
            Get
                Return _CustomerService
            End Get
        End Property

        Public ReadOnly Property Wagering() As String
            Get
                Return _Wagering
            End Get
        End Property

#End Region

    End Class

End Namespace

