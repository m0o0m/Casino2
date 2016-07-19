Imports System.Xml

Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils

Namespace CacheUtils

    <Serializable()> Public Class CPlayer
        Inherits CGeneralUser

#Region "Fields"

        Private _AgentID As String = ""
        Private _PlayerTemplateID As String = ""
        Private _DefaultPlayerTemplateID As String = ""
        Private _IsBettingLocked As Boolean = False
        Private _AlertMessage As String = ""
        Private _PasswordLastUpdated As Date
        Private _NumFailedAttempts As Integer = 0
        Private _ForgotPasswordTimestamp As Date
        Private _CreatedDate As Date
        Private _CreatedBy As String = ""
        Private _OriginalAmount As Double
        Private _BalanceForward As Double

        Private _LastLoginDate As Date
        Private _Template As CPlayerTemplate
        Private _PhoneLogin As String = ""
        Private _PhonePassword As String = ""
        Private _OneTimeMessage As String = ""
        Private _SelectedGames As New List(Of String)
        Private _SelectedBetType As String = ""
        Private _IsSuperPlayer As Boolean = False
        Private _RequirePasswordChange As Boolean
        Private _HasCasino As Boolean
        Private _SuperAgentID As String
        Private _IncreaseSpreadMoney As Double
        Private _ColorScheme As String
        Private _BackgroundImage As String
        Private _WagerLimit As Double
        Private _CurrenBalance As Double
        Private _ParlayLimit As Double
        Private _TeaserLimit As Double
        Private _PropLimit As Double
        Private _PendingAmount As Double
        Private _NumPending As Integer
        Private _PendingIfBetAmount As Double
        Private _NumPendingIfBet As Integer
#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            MyBase.New(podrData)
            _CurrenBalance = SafeDouble(podrData("BalanceAmount"))
            _UserID = SafeString(podrData("PlayerID"))
            _AgentID = SafeString(podrData("AgentID"))
            _DefaultPlayerTemplateID = SafeString(podrData("DefaultPlayerTemplateID"))
            _PlayerTemplateID = SafeString(podrData("PlayerTemplateID"))
            _IsBettingLocked = SafeBoolean(podrData("IsBettingLocked"))
            _RequirePasswordChange = SafeBoolean(podrData("RequirePasswordChange"))

            _AlertMessage = SafeString(podrData("AlertMessage"))
            _PasswordLastUpdated = SafeDate(podrData("PasswordLastUpdated"))
            _NumFailedAttempts = SafeInteger(podrData("NumFailedAttempts"))
            _ForgotPasswordTimestamp = SafeDate(podrData("ForgotPasswordTimestamp"))
            _CreatedDate = SafeDate(podrData("CreatedDate"))
            _CreatedBy = SafeString(podrData("CreatedBy"))
            _OriginalAmount = SafeDouble(podrData("OriginalAmount"))
            _BalanceForward = SafeDouble(podrData("BalanceForward"))
            _LastLoginDate = SafeDate(podrData("LastLoginDate"))
            _HasCasino = SafeBoolean(podrData("HasCasino"))
            _ParlayLimit = SafeDouble(podrData("ParlayLimit"))
            _TeaserLimit = SafeDouble(podrData("TeaserLimit"))
            _PropLimit = SafeDouble(podrData("PropLimit"))
            '_SuperAgentID = (New CAgentManager).GetSuperAgentID(_AgentID)
            Dim odrSuperAgent As DataRow = New CAgentManager().GetSuperAgent(_AgentID)
            If odrSuperAgent IsNot Nothing Then
                _ColorScheme = SafeString(odrSuperAgent("ColorScheme"))
                _SuperAgentID = SafeString(odrSuperAgent("AgentID"))
                _BackgroundImage = SafeString(odrSuperAgent("BackgroundImage"))
            End If
            If String.IsNullOrEmpty(_PlayerTemplateID) Then
                _Template = (New CPlayerTemplateManager).GetPlayerTemplate(_DefaultPlayerTemplateID)
            Else
                _Template = (New CPlayerTemplateManager).GetPlayerTemplate(_PlayerTemplateID)
            End If

            If _Template Is Nothing Then
                Throw New Exception("Not yet set player template")
            End If

            _PhoneLogin = SafeString(podrData("PhoneLogin"))
            _PhonePassword = SafeString(podrData("PhonePassword"))

            If Not _IsLocked Then
                _IsLocked = (New CAgentManager).GetLocked(_AgentID)
            End If
            getPendingInfo()
            getPendingIfBet()
            _IncreaseSpreadMoney = SafeInteger(podrData("IncreaseSpreadMoney"))
            _OneTimeMessage = SafeString(podrData("OneTimeMessage"))
            _IsSuperPlayer = SafeString(podrData("IsSuperPlayer")) = "Y"
            _WagerLimit = SafeDouble(podrData("WagerLimit"))
            setValueFromXML(_UserID)
        End Sub

        Private Sub getPendingInfo()
            Dim oEndDate As Date = Date.Now.ToUniversalTime
            Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
            If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                oStartDate.AddDays(-7)
            End If
            Dim odr As DataRow = (New CTicketManager()).GetPlayerPendingAmountInFo(_UserID, oStartDate.AddDays(6))
            If odr IsNot Nothing Then
                _PendingAmount = SafeDouble(odr("TotalRiskAmount"))
                _NumPending = SafeInteger(odr("NumPending"))
            End If

        End Sub

        Private Sub getPendingIfBet()
            Dim oEndDate As Date = Date.Now.ToUniversalTime
            Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
            If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                oStartDate.AddDays(-7)
            End If
            Dim odr As DataRow = (New CTicketManager()).GetPlayerPendingIfBetAmountInFo(_UserID, oStartDate.AddDays(6))
            If odr IsNot Nothing Then
                _PendingIfBetAmount = SafeDouble(odr("TotalRiskAmount"))
                _NumPendingIfBet = SafeInteger(odr("NumPending"))
            End If
        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property AgentID() As String
            Get
                Return _AgentID
            End Get
        End Property

        Public ReadOnly Property SuperAdminID() As String
            Get
                Return (New CAgentManager).GetSuperAdminID(_AgentID)
            End Get
        End Property

        Public ReadOnly Property SuperAgentID() As String
            Get
                Return _SuperAgentID
            End Get
        End Property

        Public ReadOnly Property CurrenBalance() As Double
            Get
                Return _CurrenBalance
            End Get
        End Property

        Public ReadOnly Property ParlayLimit() As Double
            Get
                Return _ParlayLimit
            End Get
        End Property

        Public ReadOnly Property TeaserLimit() As Double
            Get
                Return _TeaserLimit
            End Get
        End Property

        Public ReadOnly Property PropLimit() As Double
            Get
                Return _PropLimit
            End Get
        End Property

        Public ReadOnly Property WagerLimit() As Double
            Get
                Return _WagerLimit
            End Get
        End Property

        Public ReadOnly Property DefaultPlayerTemplateID() As String
            Get
                Return _DefaultPlayerTemplateID
            End Get
        End Property

        Public ReadOnly Property PlayerTemplateID() As String
            Get
                Return _PlayerTemplateID
            End Get
        End Property

        Public ReadOnly Property IsCasinoLocked() As Boolean
            Get
                If _HasCasino Then
                    ' Get Betting Locked of parent
                    Return (New CAgentManager).GetCasinoLocked(_AgentID)
                End If
                Return True
            End Get
        End Property

        Public ReadOnly Property IsBettingLocked() As Boolean
            Get
                If Not _IsBettingLocked Then
                    ' Get Betting Locked of parent
                    Return (New CAgentManager).GetBettingLocked(_AgentID)
                End If

                Return True
            End Get
        End Property

        Public ReadOnly Property RequirePasswordChange() As Boolean
            Get
                Return _RequirePasswordChange
            End Get
        End Property

        Public ReadOnly Property BalanceAmount() As Double
            Get
                Return (New CPlayerManager()).GetBalanceAmount(_UserID)
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

        Public ReadOnly Property OriginalAmount() As Double
            Get
                Return _OriginalAmount
            End Get
        End Property

        Public ReadOnly Property BalanceForward() As Double
            Get
                Return _BalanceForward
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

        Public ReadOnly Property Template() As CPlayerTemplate
            Get
                Return _Template
            End Get
        End Property

        Public ReadOnly Property PhoneLogin() As String
            Get
                Return _PhoneLogin
            End Get
        End Property

        Public ReadOnly Property OneTimeMessage() As String
            Get
                Return _OneTimeMessage
            End Get
        End Property

        Public ReadOnly Property SelectedGames() As List(Of String)
            Get
                Return Me._SelectedGames
            End Get
        End Property

        Public ReadOnly Property ColorScheme() As String
            Get
                Return _ColorScheme
            End Get
        End Property

        Public ReadOnly Property WeeklyBalanceAmount() As Double
            Get
                Dim oEndDate As Date = Date.Now.ToUniversalTime
                Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
                If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                    oStartDate.AddDays(-7)
                End If
                Return (New CTicketManager()).GetPlayerBalanceAmount(_UserID, oStartDate, oStartDate.AddDays(6))
            End Get
        End Property

        Public ReadOnly Property WeeklyAmount() As Double
            Get
                Dim oEndDate As Date = Date.Now.ToUniversalTime
                Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
                If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                    oStartDate.AddDays(-7)
                End If
                Return (New CTicketManager()).GetPlayerAmount(_UserID, oStartDate, oStartDate.AddDays(6))
            End Get
        End Property

        Public ReadOnly Property PendingAmount() As Double
            Get
                'Dim oEndDate As Date = Date.Now.ToUniversalTime
                'Dim oStartDate As Date = GetLastMondayOfDate(oEndDate)
                'If oEndDate.DayOfWeek = DayOfWeek.Monday Or oEndDate.DayOfWeek = DayOfWeek.Tuesday Then
                '    oStartDate.AddDays(-7)
                'End If
                'Return (New CTicketManager()).GetPlayerPendingAmount(_UserID, oStartDate.AddDays(6))
                Return _PendingAmount
            End Get
        End Property

        Public ReadOnly Property NumPending() As Double
            Get
                Return _NumPending
            End Get
        End Property

        Public Property SelectedBetType() As String
            Get
                Return _SelectedBetType
            End Get
            Set(ByVal value As String)
                _SelectedBetType = value
            End Set
        End Property

        Public Property IsSuperPlayer() As Boolean
            Get
                Return _IsSuperPlayer
            End Get
            Set(ByVal value As Boolean)
                _IsSuperPlayer = value
            End Set
        End Property

        Public Property HasCasino() As Boolean
            Get
                Return _HasCasino
            End Get
            Set(ByVal value As Boolean)
                _HasCasino = value
            End Set
        End Property

        Public Property IncreaseSpreadMoney() As Double
            Get
                Return _IncreaseSpreadMoney
            End Get
            Set(ByVal value As Double)
                _IncreaseSpreadMoney = value
            End Set
        End Property

        Public ReadOnly Property BackgroundImage() As String
            Get
                Return _BackgroundImage
            End Get
        End Property

        Public ReadOnly Property PendingIfBetAmount() As Double
            Get
                Return _PendingIfBetAmount
            End Get
        End Property

        Public ReadOnly Property NumPendingIfBet() As Double
            Get
                Return _NumPendingIfBet
            End Get
        End Property


#End Region

#Region "Game Selected use for Player"

        Public Function SaveSelectedGames(ByVal psUserID As String, ByVal poSelectedGames As List(Of String)) As Boolean
            Try
                saveSelectedGamesToXML(psUserID, poSelectedGames)
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        Private FILE_KEY As String = "PLAYER_INFOR_{0}.xml"

        ''save to XML
        Private Sub saveSelectedGamesToXML(ByVal psUserID As String, ByVal poSelectedGames As List(Of String))
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile("SBCSYSTEM", String.Format(FILE_KEY, psUserID))

            Dim oDoc As New XmlDocument()
            Dim oGameType As XmlElement
            'Dim oBetType As XmlElement

            If Not oHandle Is Nothing Then
                oDoc.Load(oHandle.LocalFileName)
                oGameType = CType(oDoc.DocumentElement.SelectSingleNode("GameSelecteds"), XmlElement)
                ' oBetType = CType(oDoc.DocumentElement.SelectSingleNode("BetType"), XmlElement)
                If oGameType Is Nothing Then
                    oGameType = AddXMLChild(oDoc.DocumentElement, "GameSelecteds")
                Else
                    oGameType.RemoveAll()
                End If
                'If oBetType Is Nothing Then
                '    oBetType = AddXMLChild(oDoc.DocumentElement, "BetType")
                'End If
            Else
                oHandle = oDB.GetNewFileHandle()
                oDoc.LoadXml("<PLAYER_INFOR/>")
                oGameType = AddXMLChild(oDoc.DocumentElement, "GameSelecteds")
                'oBetType = AddXMLChild(oDoc.DocumentElement, "BetType")
            End If

            '----save selecteds ---------
            For Each sSelected As String In poSelectedGames
                If sSelected <> "" Then
                    Dim oXmlEl As XmlElement = AddXMLChild(oGameType, "Selected", sSelected)
                End If
            Next

            ' Save bet Type
            'oBetType.InnerText = psBetType

            ' save to filedb
            oDoc.Save(oHandle.LocalFileName)
            oDB.PutFile("SBCSYSTEM", String.Format(FILE_KEY, psUserID), oHandle)
        End Sub

        ''load from db
        Private Sub setValueFromXML(ByVal psUserID As String)
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile("SBCSYSTEM", String.Format(FILE_KEY, psUserID))

            If Not oHandle Is Nothing Then
                Try
                    Dim oDoc As New XmlDocument
                    oDoc.Load(oHandle.LocalFileName)

                    loadSelectedGames(oDoc)
                Catch ex As Exception

                End Try
            End If
        End Sub

        Private Sub loadSelectedGames(ByVal poDoc As XmlDocument)
            Dim oXEGameType As XmlElement = CType(poDoc.DocumentElement.SelectSingleNode("GameSelecteds"), XmlElement)

            If Not oXEGameType Is Nothing Then
                For Each oXmlElement As XmlElement In oXEGameType.ChildNodes
                    Dim sGameType As String = oXmlElement.InnerText

                    If Not Me._SelectedGames.Contains(sGameType) Then
                        Me._SelectedGames.Add(sGameType)
                    End If
                Next
            End If

            'If poDoc.DocumentElement.SelectSingleNode("BetType") IsNot Nothing Then
            '    _SelectedBetType = CType(poDoc.DocumentElement.SelectSingleNode("BetType"), XmlElement).InnerText
            'End If

        End Sub

#End Region

        Public Function CheckPasswowrd(ByVal psPassword As String) As Boolean
            Return (New CPlayerManager()).CheckPassword(Me.UserID, psPassword)
        End Function

        Public Function CheckPhonePasswowrd(ByVal psPhonePassword As String) As Boolean
            Return (New CPlayerManager()).CheckPhonePassword(Me.UserID, psPhonePassword)
        End Function
    End Class

End Namespace