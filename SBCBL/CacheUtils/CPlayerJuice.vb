Imports SBCBL.std
Imports SBCBL.Utils
Imports WebsiteLibrary.DBUtils
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CPlayerJuice

#Region "Fields"
        Private _PlayerJuiceManagementID As String = ""
        Private _PlayerTemplateID As String = ""
        Private _SportType As String = ""
        Private _JuiceType As String = ""
        Private _SpreadSetup As Double = 0
        Private _1HSpreadSetup As Double = 0
        Private _2HSPreadSetup As Double = 0
        Private _TotalPointSetup As Double = 0
        Private _1HTotalPointSetup As Double = 0
        Private _2HTotalPointSetup As Double = 0
        Private _MLineSetup As Double = 0
        Private _1HMLineSetup As Double = 0
        Private _2HMLineSetup As Double = 0
        Private _TeamTotalPointSetup As Double = 0
#End Region

#Region "Constructors"

        Public Sub New()
            _PlayerJuiceManagementID = newGUID()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _PlayerJuiceManagementID = SafeString(podrData("PlayerJuiceManagementID"))
            _PlayerTemplateID = SafeString(podrData("PlayerTemplateID"))
            _SportType = SafeString(podrData("SportType"))
            _JuiceType = SafeString(podrData("JuiceType"))
            _SpreadSetup = SafeDouble(podrData("SpreadSetup"))
            _1HSpreadSetup = SafeDouble(podrData("1HSpreadSetup"))
            _2HSPreadSetup = SafeDouble(podrData("2HSpreadSetup"))
            _TotalPointSetup = SafeDouble(podrData("TotalPointSetup"))
            _1HTotalPointSetup = SafeDouble(podrData("1HTotalPointSetup"))
            _2HTotalPointSetup = SafeDouble(podrData("2HTotalPointSetup"))
            _MLineSetup = SafeDouble(podrData("MLineSetup"))
            _1HMLineSetup = SafeDouble(podrData("1HMLineSetup"))
            _2HMLineSetup = SafeDouble(podrData("2HMLineSetup"))
            _TeamTotalPointSetup = SafeDouble(podrData("TeamTotalPointSetup"))
        End Sub
#End Region

#Region "Properties"
        Public Property PlayerJuiceManagementID() As String
            Get
                Return _PlayerJuiceManagementID
            End Get
            Set(ByVal value As String)
                _PlayerJuiceManagementID = value
            End Set
        End Property

        Public Property playerTemplateID() As String
            Get
                Return _PlayerTemplateID
            End Get
            Set(ByVal value As String)
                _PlayerTemplateID = value
            End Set
        End Property

        Public Property SportType() As String
            Get
                Return _SportType
            End Get
            Set(ByVal value As String)
                _SportType = value
            End Set
        End Property

        Public Property JuiceType() As String
            Get
                Return _JuiceType
            End Get
            Set(ByVal value As String)
                _JuiceType = value
            End Set
        End Property

        Public Property SpreadSetup() As Double
            Get
                Return _SpreadSetup
            End Get
            Set(ByVal value As Double)
                _SpreadSetup = value
            End Set
        End Property

        Public Property _1stHSpreadSetup() As Double
            Get
                Return _1HSpreadSetup
            End Get
            Set(ByVal value As Double)
                _1HSpreadSetup = value
            End Set
        End Property

        Public Property _2ndHSPreadSetup() As Double
            Get
                Return _2HSPreadSetup
            End Get
            Set(ByVal value As Double)
                _2HSPreadSetup = value
            End Set
        End Property

        Public Property TotalPointSetup() As Double
            Get
                Return _TotalPointSetup
            End Get
            Set(ByVal value As Double)
                _TotalPointSetup = value
            End Set
        End Property

        Public Property _1stHTotalPointSetup() As Double
            Get
                Return _1HTotalPointSetup
            End Get
            Set(ByVal value As Double)
                _1HTotalPointSetup = value
            End Set
        End Property

        Public Property _2ndHTotalPointSetup() As Double
            Get
                Return _2HTotalPointSetup
            End Get
            Set(ByVal value As Double)
                _2HTotalPointSetup = value
            End Set
        End Property

        Public Property MLineSetup() As Double
            Get
                Return _MLineSetup
            End Get
            Set(ByVal value As Double)
                _MLineSetup = value
            End Set
        End Property

        Public Property _1stHMLineSetup() As Double
            Get
                Return _1HMLineSetup
            End Get
            Set(ByVal value As Double)
                _1HMLineSetup = value
            End Set
        End Property

        Public Property _2ndMLineSetup() As Double
            Get
                Return _2HMLineSetup
            End Get
            Set(ByVal value As Double)
                _2HMLineSetup = value
            End Set
        End Property

        Public Property TeamTotalPointSetup() As Double
            Get
                Return _TeamTotalPointSetup
            End Get
            Set(ByVal value As Double)
                _TeamTotalPointSetup = value
            End Set
        End Property


#End Region

    End Class

    <Serializable()> Public Class CPlayerJuiceList
        Inherits List(Of CPlayerJuice)
        <NonSerialized()> Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CPlayerJuice))

        Public Function GetPlayerJuiceConfig(ByVal poPlayerJuice As CPlayerJuice, ByVal psBetType As String) As Double
            Select Case psBetType.ToUpper
                Case "CURRENTSPREAD"
                    Return poPlayerJuice.SpreadSetup
                Case "1HSPREAD"
                    Return poPlayerJuice._1stHSpreadSetup
                Case "2HSPREAD"
                    Return poPlayerJuice._2ndHSPreadSetup
                Case "CURRENTTOTALPOINTS"
                    Return poPlayerJuice.TotalPointSetup
                Case "1HTOTALPOINTS"
                    Return poPlayerJuice._1stHTotalPointSetup
                Case "2HTOTALPOINTS"
                    Return poPlayerJuice._2ndHTotalPointSetup
                Case "CURRENTMONEYLINE"
                    Return poPlayerJuice.MLineSetup
                Case "1HMONEYLINE"
                    Return poPlayerJuice._1stHMLineSetup
                Case "2HMONEYLINE"
                    Return poPlayerJuice._2ndMLineSetup
                Case "CURRENTTEAMTOTALPOINTS"
                    Return poPlayerJuice.TeamTotalPointSetup
                Case Else
                    Return 0
            End Select
        End Function

        Public Function CheckSportType(ByVal psGameType As String) As String
            Dim sSportType As String = ""
            If IsSoccer(psGameType.ToUpper) Then
                sSportType = "Soccer"
            ElseIf IsBaseball(psGameType.ToUpper) Then
                sSportType = "Baseball"
            ElseIf IsBasketball(psGameType.ToUpper) Then
                sSportType = "Basketball"
            ElseIf IsFootball(psGameType.ToUpper) Then
                sSportType = "Football"
            ElseIf IsHockey(psGameType.ToUpper) Then
                sSportType = "Hockey"
            ElseIf IsOtherGameType(psGameType.ToUpper) Then
                sSportType = "Other"
            Else
                sSportType = "Proposition"
            End If
            Return sSportType
        End Function

        Public Function GetPlayerJuiceInList(ByVal psGameType As String, ByVal psJuiceType As String) As CPlayerJuice
            Dim sSportType As String = UCase(CheckSportType(UCase(psGameType)))
           
            Return Me.Find(Function(oJuice) UCase(oJuice.SportType) = sSportType _
                               AndAlso UCase(oJuice.JuiceType) = UCase(psJuiceType))
        End Function


        Public Function GetFavConfig(ByVal psGameType As String, ByVal psBetType As String) As Double
            Dim oPlayerJuice As CPlayerJuice = GetPlayerJuiceInList(psGameType, "Favorite")
            If oPlayerJuice IsNot Nothing Then
                Return GetPlayerJuiceConfig(oPlayerJuice, psBetType)
            End If
        End Function

        Public Function GetUndConfig(ByVal psGameType As String, ByVal psBetType As String) As Double
            Dim oPlayerJuice = GetPlayerJuiceInList(psGameType, "Underdog")
            If oPlayerJuice IsNot Nothing Then
                Return GetPlayerJuiceConfig(oPlayerJuice, psBetType)
            End If
        End Function

    End Class

End Namespace
