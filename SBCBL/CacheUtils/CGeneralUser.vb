Imports SBCBL.std
Imports System.Data

Namespace CacheUtils

    <Serializable()> Public Class CGeneralUser

#Region "Fields"

        Protected _UserID As String = ""
        Protected _Login As String = ""
        Protected _Password As String = ""
        Protected _Name As String = ""
        Protected _TimeZone As Integer = 0
        Protected _IsLocked As Boolean = False
        Protected _LockReason As String = ""

#End Region

#Region "Constructor"

        Public Sub New()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _Login = SafeString(podrData("Login"))
            _Password = SafeString(podrData("Password"))
            _Name = SafeString(podrData("Name"))
            _TimeZone = SafeInteger(podrData("TimeZone"))
            _IsLocked = SafeString(podrData("IsLocked")) = "Y"
            _LockReason = SafeString(podrData("LockReason"))
        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property UserID() As String
            Get
                Return _UserID
            End Get
        End Property

        Public ReadOnly Property Login() As String
            Get
                Return _Login
            End Get
        End Property

        Public ReadOnly Property Password() As String
            Get
                Return _Password
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return _Name
            End Get
        End Property

        Public ReadOnly Property TimeZone() As Integer
            Get
                Return _TimeZone
            End Get
        End Property

        Public ReadOnly Property LockReason() As String
            Get
                Return _LockReason
            End Get
        End Property

        Public ReadOnly Property IsLocked() As Boolean
            Get
                Return _IsLocked
            End Get
        End Property

#End Region

#Region "Converts"

        ''' <summary>
        ''' ADDS the timezone offset to the GMT time
        ''' </summary>
        Public Function ConvertToEST(ByVal poDate As Date) As Date
            Dim oDate As DateTime = poDate
            If oDate > DateTime.MinValue Then
                If SafeBoolean((New UI.CSBCSession).Cache.GetSysSettings("DayLight").GetValue("DayLight")) Then
                    oDate = oDate.AddHours(1)
                End If
                Return oDate.AddHours(-5)
            End If

            Return Nothing
        End Function

        '''' <summary>
        '''' ADDS the timezone offset to the GMT time
        '''' </summary>
        Public Function ConvertToLocalTime(ByVal poDate As Date) As Date
            Dim oDate As DateTime = poDate
            If oDate > DateTime.MinValue Then
                Dim oTimeZone As TimeZone = System.TimeZone.CurrentTimeZone
                If oTimeZone.IsDaylightSavingTime(oDate) Then
                    oDate = oDate.AddHours(1)
                End If
                Return oDate.AddHours(TimeZone)
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' SUBTRACTS the timezone offset from the local time to get to the GMT time
        ''' </summary>
        Public Function ConvertToGMT(ByVal poDate As Date) As Date
            Dim oDate As DateTime = poDate
            If oDate > DateTime.MinValue Then
                Dim oTimeZone As TimeZone = System.TimeZone.CurrentTimeZone
                If oTimeZone.IsDaylightSavingTime(oDate) Then
                    oDate = oDate.AddHours(-1)
                End If
                Return oDate.AddHours(-1 * TimeZone)
            End If

            Return Nothing
        End Function

#End Region

    End Class

End Namespace