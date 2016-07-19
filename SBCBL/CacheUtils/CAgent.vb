Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils

    <Serializable()> Public Class CAgent
        Inherits CGeneralUser

#Region "Fields"

        Private _ParentID As String = ""
        Private _SuperAdminID As String = ""
        Private _IsBettingLocked As Boolean = False

        Private _AlertMessage As String = ""
        Private _PasswordLastUpdated As Date
        Private _NumFailedAttempts As Integer = 0
        Private _ForgotPasswordTimestamp As Date
        Private _CreatedDate As Date
        Private _CreatedBy As String = ""
        Private _BalanceForward As Double
        Private _BalanceAmount As Double
        Private _ProfitPercentage As Single
        Private _GrossPercentage As Single
        Private _LastLoginDate As Date
        Private _WeeklyCharge As Double
        Private _IsEnablePlayerTemplate As Boolean
        Private _IsEnableBlockPlayer As Boolean
        Private _SpecialKey As String
        Private _RequirePasswordChange As Boolean
        Private _IsEnableBettingProfile As Boolean
        Private _HasGameManagement As Boolean
        Private _HasSystemManagement As Boolean
        Private _IsSuperAgent As Boolean
        Private _SuperAgentID As String
        Private _IsEnableChangeBookmaker As Boolean
        Private _ColorScheme As String
        Private _BackgroundImage As String
        Private _CurrentPlayerNumber As Integer
        Private _HasCrediLimitSetting As Boolean
        Private _AddNewSubAgent As Boolean
#End Region

#Region "Constructor"

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            MyBase.New(podrData)

            _UserID = SafeString(podrData("AgentID"))
            _ParentID = SafeString(podrData("ParentID"))
            _SuperAdminID = SafeString(podrData("SuperAdminID"))
            _IsBettingLocked = SafeString(podrData("IsBettingLocked")) = "Y"

            _AlertMessage = SafeString(podrData("AlertMessage"))
            _PasswordLastUpdated = SafeDate(podrData("PasswordLastUpdated"))
            _NumFailedAttempts = SafeInteger(podrData("NumFailedAttempts"))
            _ForgotPasswordTimestamp = SafeDate(podrData("ForgotPasswordTimestamp"))
            _CreatedDate = SafeDate(podrData("CreatedDate"))
            _CreatedBy = SafeString(podrData("CreatedBy"))
            _BalanceForward = SafeDouble(podrData("BalanceForward"))
            _BalanceAmount = SafeDouble(podrData("BalanceAmount"))
            _ProfitPercentage = SafeSingle(podrData("ProfitPercentage"))
            _GrossPercentage = SafeSingle(podrData("GrossPercentage"))
            _SpecialKey = SafeString(podrData("SpecialKey"))
            _LastLoginDate = SafeDate(podrData("LastLoginDate"))
            _WeeklyCharge = SafeDouble(podrData("WeeklyCharge"))
            _IsEnablePlayerTemplate = SafeBoolean(podrData("IsEnablePlayerTemplate"))
            _IsEnableBlockPlayer = SafeBoolean(podrData("IsEnableBlockPlayer"))
            _RequirePasswordChange = SafeBoolean(podrData("RequirePasswordChange"))
            _IsEnableBettingProfile = SafeString(podrData("IsEnableBettingProfile")) = "Y"
            _HasGameManagement = SafeString(podrData("HasGameManagement")) = "Y"
            _HasSystemManagement = SafeString(podrData("HasSystemManagement")) = "Y"
            _IsSuperAgent = Not String.IsNullOrEmpty(SafeString(podrData("SuperAdminID")))
            '_SuperAgentID = (New CAgentManager).GetSuperAgentID(_UserID)
            _IsEnableChangeBookmaker = SafeString(podrData("IsEnableChangeBookmaker")) = "Y"
            _HasCrediLimitSetting = SafeString(podrData("HasCrediLimitSetting")) = "Y"
            _AddNewSubAgent = SafeString(podrData("AddNewSubAgent")) = "Y"
            Dim odrSuperAgent As DataRow = New CAgentManager().GetSuperAgent(_UserID)
            If odrSuperAgent IsNot Nothing Then
                _ColorScheme = SafeString(odrSuperAgent("ColorScheme"))
                _SuperAgentID = SafeString(odrSuperAgent("AgentID"))
                _BackgroundImage = SafeString(odrSuperAgent("BackgroundImage"))
                _CurrentPlayerNumber = SafeInteger(podrData("CurrentPlayerNumber"))
            End If

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property AddNewSubAgent() As Boolean
            Get
                Return _AddNewSubAgent
            End Get
        End Property

        Public ReadOnly Property HasCrediLimitSetting() As Boolean
            Get
                Return _HasCrediLimitSetting
            End Get
        End Property

        Public ReadOnly Property ParentID() As String
            Get
                Return _ParentID
            End Get
        End Property

        Public ReadOnly Property CurrentPlayerNumber() As Integer
            Get
                Return _CurrentPlayerNumber
            End Get
        End Property

        Public ReadOnly Property SuperAgentID() As String
            Get
                Return _SuperAgentID
            End Get
        End Property

        Public ReadOnly Property SuperAdminID() As String
            Get
                If _SuperAdminID = "" Then

                    _SuperAdminID = (New CAgentManager).GetSuperAdminID(_UserID)

                    If _SuperAdminID = "" Then
                        Throw New Exception("Agent not belong to super admin")
                    End If
                End If

                Return _SuperAdminID
            End Get
        End Property

        Public ReadOnly Property IsBettingLocked() As Boolean
            Get
                If Not _IsBettingLocked Then
                    ' Get Betting Locked of parent
                    Return (New CAgentManager).GetBettingLocked(_UserID)

                End If

                Return True
            End Get
        End Property

        Public ReadOnly Property RequirePasswordChange() As Boolean
            Get
                Return _RequirePasswordChange
            End Get
        End Property

        Public ReadOnly Property WeeklyCharge() As Double
            Get
                Return _WeeklyCharge
            End Get
        End Property
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

        Public ReadOnly Property BalanceAmount() As Double
            Get
                Return _BalanceAmount
            End Get
        End Property

        Public ReadOnly Property BalanceForward() As Double
            Get
                Return _BalanceForward
            End Get
        End Property

        Public ReadOnly Property ProfitPercentage() As Single
            Get
                Return _ProfitPercentage
            End Get
        End Property

        Public ReadOnly Property GrossPercentage() As Single
            Get
                Return _GrossPercentage
            End Get
        End Property

        Public ReadOnly Property SpecialKey() As String
            Get
                Return _SpecialKey
            End Get
        End Property

        Public ReadOnly Property IsEnablePlayerTemplate() As Boolean
            Get
                Return SafeBoolean(_IsEnablePlayerTemplate)
            End Get
        End Property

        Public ReadOnly Property IsEnableBlockPlayer() As Boolean
            Get
                Return SafeBoolean(_IsEnableBlockPlayer)
            End Get
        End Property

        Public ReadOnly Property IsEnableBettingProfile() As Boolean
            Get
                Return SafeBoolean(_IsEnableBettingProfile)
            End Get
        End Property

        Public ReadOnly Property HasGameManagement() As Boolean
            Get
                Return _HasGameManagement
            End Get
        End Property

        Public ReadOnly Property HasSystemManagement() As Boolean
            Get
                Return _HasSystemManagement
            End Get
        End Property

        Public ReadOnly Property ColorScheme() As String
            Get
                Return _ColorScheme
            End Get
        End Property

        Public ReadOnly Property BackgroundImage() As String
            Get
                Return _BackgroundImage
            End Get
        End Property

        Public ReadOnly Property IsSuperAgent() As Boolean
            Get
                Return _IsSuperAgent
            End Get
        End Property
        Public Function PLAmount() As Double
            Dim nWinLoseAmount As Double = 0
            Dim oEndDate As Date = Date.Now.ToUniversalTime
            Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
            If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                oStartDate.AddDays(-7)
            End If

            nWinLoseAmount = (New CTicketManager).GetPLAmount(_UserID, oStartDate, oStartDate.AddDays(6))

            Return nWinLoseAmount
        End Function

        Public Property IsEnableChangeBookmaker() As Boolean
            Get
                Return _IsEnableChangeBookmaker
            End Get
            Set(ByVal value As Boolean)
                _IsEnableChangeBookmaker = value
            End Set
        End Property


#End Region

        Public Function CheckPassword(ByVal psPassword As String) As Boolean
            Return (New CAgentManager()).CheckPassword(Me.UserID, psPassword)
        End Function
    End Class

End Namespace