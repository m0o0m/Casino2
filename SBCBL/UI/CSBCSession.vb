Imports SBCBL.CEnums
Imports SBCBL.std
Imports SBCBL.CacheUtils
Imports System.Web
Imports System.Xml
Imports WebsiteLibrary.CXMLUtils

Namespace UI
    Public Class CSBCSession

#Region "Properties"

        Public ReadOnly Property Cache() As CCacheManager
            Get
                If HttpContext.Current.Session("CACHE_MANAGER") Is Nothing Then
                    HttpContext.Current.Session("CACHE_MANAGER") = New CCacheManager()
                End If

                Return CType(System.Web.HttpContext.Current.Session("CACHE_MANAGER"), CCacheManager)
            End Get
        End Property

        Public ReadOnly Property UserID() As String
            Get
                Return SafeString(HttpContext.Current.Session("USER_ID"))
            End Get
        End Property

        Public ReadOnly Property AgentSelectID() As String
            Get
                If String.IsNullOrEmpty(SafeString(HttpContext.Current.Session("AGENT_SELECT_ID"))) Then
                    Return UserID
                Else
                    Return SafeString(HttpContext.Current.Session("AGENT_SELECT_ID"))
                End If
            End Get
        End Property

        Public ReadOnly Property UserType() As EUserType
            Get
                If HttpContext.Current.Session("USER_TYPE") Is Nothing Then
                    HttpContext.Current.Session("USER_TYPE") = EUserType.Player
                End If
                Return CType(HttpContext.Current.Session("USER_TYPE"), EUserType)
            End Get
        End Property

        Public ReadOnly Property UserTypeOfBet() As ETypeOfBet
            Get
                Return If(UserType = EUserType.CallCenterAgent, ETypeOfBet.Phone, ETypeOfBet.Internet)
            End Get
        End Property

        Public ReadOnly Property AgentUserInfo() As CAgent
            Get
                If UserType <> EUserType.Agent Then
                    Throw New Exception("This is not agent user")
                End If
                Return Cache.GetAgentInfo(UserID)
            End Get
        End Property

        Public ReadOnly Property CCAgentUserInfo() As CCallCenterAgent
            Get
                If UserType <> EUserType.CallCenterAgent Then
                    Throw New Exception("This is not call center agent user")
                End If
                Return Cache.GetCallCenterAgentInfo(UserID)
            End Get
        End Property

        Public ReadOnly Property PlayerUserInfo() As CPlayer
            Get
                If UserType <> EUserType.Player Then
                    Throw New Exception("This is not player user")
                End If
                Return Cache.GetPlayerInfo(UserID)
            End Get
        End Property

        Public ReadOnly Property SuperAdminInfo() As CSuperAdmin
            Get
                If UserType <> EUserType.SuperAdmin Then
                    Throw New Exception("This is not SuperAdmin user")
                End If
                Return Cache.GetSuperAdminInfo(UserID)
            End Get
        End Property

        Public ReadOnly Property SysSettings(ByVal psCategory As String) As CSysSettingList
            Get
                Return Me.Cache.GetSysSettings(psCategory)
            End Get
        End Property

        Public ReadOnly Property SysSettings(ByVal psCategory As String, ByVal psSubCategory As String) As CSysSettingList
            Get
                Return Me.Cache.GetSysSettings(psCategory, psSubCategory)
            End Get
        End Property

        Public Property SelectedTicket(ByVal psPlayerID As String) As Tickets.CSelectedTickets
            Get
                Dim sKey As String = String.Format("{0}_SELECTED_TICKETS", psPlayerID)
                If HttpContext.Current.Session(sKey) Is Nothing Then
                    'If UserType = EUserType.CallCenterAgent Then
                    '    HttpContext.Current.Session(sKey) = New Tickets.CSelectedTickets(ETypeOfBet.Phone)
                    'Else
                    '    HttpContext.Current.Session(sKey) = New Tickets.CSelectedTickets(ETypeOfBet.Internet)
                    'End If
                    HttpContext.Current.Session(sKey) = New Tickets.CSelectedTickets(UserTypeOfBet)
                End If

                Return CType(HttpContext.Current.Session(sKey), Tickets.CSelectedTickets)
            End Get
            Set(ByVal value As Tickets.CSelectedTickets)
                HttpContext.Current.Session(String.Format("{0}_SELECTED_TICKETS", psPlayerID)) = value
            End Set
        End Property

        ''' <summary>
        ''' If psPlayerID is empty then get current player
        ''' </summary>
        Public ReadOnly Property SelectedGameTypes(Optional ByVal psPlayerID As String = "") As List(Of String)
            Get
                If Me.UserType = EUserType.Player Then
                    If HttpContext.Current.Session("__SELECTED_GAME_TYPES") Is Nothing Then
                        Dim oSelecteds As New List(Of String)

                        If psPlayerID = "" Then
                            oSelecteds = Me.PlayerUserInfo.SelectedGames
                        Else
                            oSelecteds = Cache.GetPlayerInfo(psPlayerID).SelectedGames
                        End If

                        HttpContext.Current.Session("__SELECTED_GAME_TYPES") = Utils.CSerializedObjectCloner.Clone(Of List(Of String))(oSelecteds)
                    End If

                    Return CType(HttpContext.Current.Session("__SELECTED_GAME_TYPES"), List(Of String))
                End If

                If psPlayerID = "" Then
                    Throw New Exception("Must set player id")
                End If

                Return Cache.GetPlayerInfo(psPlayerID).SelectedGames
            End Get
        End Property

        ''' <summary>
        ''' If psPlayerID is empty then get current player
        ''' </summary>
        Public Property SelectedBetType(Optional ByVal psPlayerID As String = "") As String
            Get
                If Me.UserType = EUserType.Player Then
                    If HttpContext.Current.Session("__SELECTED_BET_TYPES") Is Nothing Then
                        Dim sSelectedType As String = ""

                        If psPlayerID = "" Then
                            HttpContext.Current.Session("__SELECTED_BET_TYPES") = Me.PlayerUserInfo.SelectedBetType
                        Else
                            HttpContext.Current.Session("__SELECTED_BET_TYPES") = Cache.GetPlayerInfo(psPlayerID).SelectedBetType
                        End If
                    End If

                    Return SafeString(HttpContext.Current.Session("__SELECTED_BET_TYPES"))
                End If

                If psPlayerID = "" Then
                    Throw New Exception("Must set player id")
                End If

                Return Cache.GetPlayerInfo(psPlayerID).SelectedBetType
            End Get
            Set(ByVal value As String)
                If Me.UserType = EUserType.Player Then
                    Me.PlayerUserInfo.SelectedBetType = value
                    HttpContext.Current.Session("__SELECTED_BET_TYPES") = value
                Else
                    If psPlayerID = "" Then
                        Throw New Exception("Must set player id")
                    End If
                    Cache.GetPlayerInfo(psPlayerID).SelectedBetType = value
                End If
            End Set
        End Property

        Public ReadOnly Property ShowOneBookmaker() As Boolean
            Get
                If HttpContext.Current.Session("_Show_One_Book_maker") Is Nothing Then
                    Dim sCategory As String = SBCBL.std.GetSiteType & " LineRules"
                    Dim bShowOneBookMaker As Boolean = False
                    Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
                    Dim sKey As String = "ShowOneBookMaker"
                    Dim oShowOneBookMaker As CSysSetting = oSysManager.GetAllSysSettings(sCategory).Find(Function(x) x.Key = sKey)
                    If oShowOneBookMaker IsNot Nothing Then
                        bShowOneBookMaker = oShowOneBookMaker.Value = "Y"
                    End If
                    HttpContext.Current.Session("_Show_One_Book_maker") = bShowOneBookMaker
                End If
                Return SafeBoolean(HttpContext.Current.Session("_Show_One_Book_maker"))
            End Get
        End Property

        Public ReadOnly Property UseTigerBookmaker() As Boolean
            Get
                If HttpContext.Current.Session("_Use_Tiger_Book_maker") Is Nothing Then
                    Dim sCategory As String = SBCBL.std.GetSiteType & " LineRules"
                    Dim bTigerSBBookMaker As Boolean = False
                    Dim oSysManager As New SBCBL.Managers.CSysSettingManager()
                    Dim sKey As String = "TigerSBBookMaker"
                    Dim oTigerSBBookMaker As CSysSetting = oSysManager.GetAllSysSettings(sCategory).Find(Function(x) x.Key = sKey)
                    If oTigerSBBookMaker IsNot Nothing Then
                        bTigerSBBookMaker = oTigerSBBookMaker.Value = "Y"
                    End If
                    HttpContext.Current.Session("_Use_Tiger_Book_maker") = bTigerSBBookMaker
                End If
                Return SafeBoolean(HttpContext.Current.Session("_Use_Tiger_Book_maker"))
            End Get
        End Property

        Public ReadOnly Property FirstHalfOff(ByVal psGametype As String) As Boolean
            Get
                Dim oCacheManager As New CacheUtils.CCacheManager()
                Dim oSys = oCacheManager.GetSysSettings("HalfLineOff", "1H").Find(Function(x) x.Key = psGametype And UCase(x.Value) = "TRUE")
                Return oSys IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property SecondHalfTimeOff(ByVal psGameType As String) As Integer
            Get
                'Dim sGameType As String = SBCBL.std.GetSiteType & " GameType"
                Dim oCachemanager As New CCacheManager()
                'Dim oSysGame As CSysSetting = oCachemanager.GetAllSysSettings(sGameType).Find(Function(x) UCase(x.Key) = UCase(psGameType) AndAlso x.SubCategory <> "")
                'Dim oSysSport As CSysSetting = oCachemanager.GetSysSettings(sGameType).Find(Function(x) UCase(x.SysSettingID) = UCase(oSysGame.SubCategory))

                If IsFootball(psGameType) Or IsBasketball(psGameType) Or IsSoccer(psGameType) Then
                    Dim sCategory As String = "SBS TimeOff2H"
                    Dim sKey As String = "TimeOff2H"
                    Dim oListSettings As CSysSettingList = SysSettings(sCategory)
                    Dim oSetting As CSysSetting = oListSettings.Find(Function(x) UCase(x.Key) = UCase(sKey))
                    If oSetting Is Nothing Then
                        Return 0
                    Else
                        Return SafeInteger(oSetting.Value)
                    End If

                Else
                    Return 0
                End If
            End Get
        End Property

        Public ReadOnly Property IsSecondhalfOff(ByVal psGametype As String) As Boolean
            Get
                Dim oCacheManager As New CacheUtils.CCacheManager()
                Dim oSys = oCacheManager.GetSysSettings("HalfLineOff", "2H").Find(Function(x) x.Key = psGametype And UCase(x.Value) = "TRUE")
                Return oSys IsNot Nothing
            End Get
        End Property

        Public Property HasIPAlert() As Boolean
            Get
                If HttpContext.Current.Session("HAS_IPALERT") Is Nothing Then
                    HttpContext.Current.Session("HAS_IPALERT") = Cache.checkIPAlert(UserType, UserID)
                End If
                Return CType(HttpContext.Current.Session("HAS_IPALERT"), Boolean)
            End Get
            Set(ByVal value As Boolean)
                HttpContext.Current.Session("HAS_IPALERT") = value
            End Set
        End Property

#End Region

#Region "Convert to LocalTime, GMT"

        Public Function ConvertToEST(ByVal psDate As String) As Date
            If IsDate(psDate) Then
                Return ConvertToEST(CDate(psDate))
            End If
            Return Nothing
        End Function

        Public Function ConvertToEST(ByVal poDate As Date) As Date
            Select Case UserType
                Case EUserType.Agent
                    Return AgentUserInfo.ConvertToEST(poDate)
                Case EUserType.CallCenterAgent
                    Return CCAgentUserInfo.ConvertToEST(poDate)
                Case EUserType.Player
                    Return PlayerUserInfo.ConvertToEST(poDate)
                Case EUserType.SuperAdmin
                    Return SuperAdminInfo.ConvertToEST(poDate)
                Case Else
                    '' Error
                    Return Nothing
            End Select
        End Function

        Public Function ConvertToLocalTime(ByVal psDate As String) As Date
            If IsDate(psDate) Then
                Return ConvertToLocalTime(CDate(psDate))
            End If
            Return Nothing
        End Function

        Public Function ConvertToLocalTime(ByVal poDate As Date) As Date
            Select Case UserType
                Case EUserType.Agent
                    Return AgentUserInfo.ConvertToLocalTime(poDate)
                Case EUserType.CallCenterAgent
                    Return CCAgentUserInfo.ConvertToLocalTime(poDate)
                Case EUserType.Player
                    Return PlayerUserInfo.ConvertToLocalTime(poDate)
                Case EUserType.SuperAdmin
                    Return SuperAdminInfo.ConvertToLocalTime(poDate)
                Case Else
                    '' Error
                    Return Nothing
            End Select
        End Function

        Public Function ConvertEasternDateToLocal(ByVal psDate As String) As Date
            If IsDate(psDate) Then
                Return ConvertEasternDateToLocal(CDate(psDate))
            End If
            Return Nothing
        End Function

        Public Function ConvertEasternDateToLocal(ByVal poDate As Date) As Date
            If IsDate(poDate) Then
                '' sportinside server is GMT-5, add 5 hours to get GMT time
                poDate = poDate.AddHours(5)
            End If

            Return ConvertToEST(poDate)
        End Function

        Public Function ConvertToGMT(ByVal poDate As Date) As Date
            Select Case UserType
                Case EUserType.Agent
                    Return AgentUserInfo.ConvertToGMT(poDate)
                Case EUserType.CallCenterAgent
                    Return CCAgentUserInfo.ConvertToGMT(poDate)
                Case EUserType.Player
                    Return PlayerUserInfo.ConvertToGMT(poDate)
                Case EUserType.SuperAdmin
                    Return SuperAdminInfo.ConvertToGMT(poDate)
                Case Else
                    '' Error
                    Return Nothing
            End Select
        End Function

        Public Function ConvertToGMT(ByVal psDate As String) As Date
            If IsDate(psDate) Then
                Return ConvertToGMT(CDate(psDate))
            End If
            Return Nothing
        End Function

#End Region

#Region "Game Selected use for Player"

        ''' <summary>
        ''' If psPlayerID is empty then get current player
        ''' </summary>
        Public Function IsExistedSelectedGameType(ByVal psGameType As String, ByVal psPlayerID As String) As Boolean
            Return Me.SelectedGameTypes(psPlayerID).Find(Function(x) UCase(x) = UCase(psGameType)) IsNot Nothing
        End Function

        ''' <summary>
        ''' If psPlayerID is empty then get current player
        ''' </summary>
        Public Sub RemoveSelectedGameType(ByVal psGameType As String, ByVal psPlayerID As String)
            Me.SelectedGameTypes(psPlayerID).RemoveAll(Function(x) x.Equals(psGameType))
        End Sub

        ''' <summary>
        ''' If psPlayerID is empty then get current player
        ''' </summary>
        Public Sub AddSelectedGameType(ByVal psGameType As String, ByVal psPlayerID As String)
            RemoveSelectedGameType(psGameType, psPlayerID)
            Me.SelectedGameTypes(psPlayerID).Add(psGameType)
        End Sub

#End Region
    End Class

End Namespace