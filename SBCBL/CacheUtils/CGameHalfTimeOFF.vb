Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CGameHalfTimeOFF

        Private _GameType As String
        Private _FirstHOff As Boolean
        Private _SecondHOff As Boolean
        Private _TeamTotal As Boolean

        Public Sub New(ByVal psGameType As String, ByVal pbFirstHOff As Boolean, ByVal pbSecondHOff As Boolean, ByVal pbTeamTotal As Boolean)
            _GameType = psGameType
            _FirstHOff = pbFirstHOff
            _SecondHOff = pbSecondHOff
            _TeamTotal = pbTeamTotal
        End Sub

        Public Property GameType() As String
            Get
                Return _GameType
            End Get
            Set(ByVal value As String)
                _GameType = value
            End Set
        End Property

        Public Property FirstHOff() As Boolean
            Get
                Return _FirstHOff
            End Get
            Set(ByVal value As Boolean)
                _FirstHOff = value
            End Set
        End Property

        Public Property SecondHOff() As Boolean
            Get
                Return _SecondHOff
            End Get
            Set(ByVal value As Boolean)
                _SecondHOff = value
            End Set
        End Property

        Public Property TeamTotalOff() As Boolean
            Get
                Return _TeamTotal
            End Get
            Set(ByVal value As Boolean)
                _TeamTotal = value
            End Set
        End Property
    End Class

    <Serializable()> Public Class CGameHalfTimeOffList
        Inherits List(Of CGameHalfTimeOFF)

        Public Function GetGameHalfTimeOff(ByVal psGameType As String) As CGameHalfTimeOFF
            Dim lstGameHalfTimeOFF As List(Of CGameHalfTimeOFF) = (From oGameHalfTimeOFF As CGameHalfTimeOFF In Me _
                                                            Where oGameHalfTimeOFF.GameType = psGameType _
                                                            Order By oGameHalfTimeOFF.GameType _
                                                            Select oGameHalfTimeOFF).ToList
            If lstGameHalfTimeOFF IsNot Nothing AndAlso lstGameHalfTimeOFF.Count > 0 Then
                Return CType(lstGameHalfTimeOFF(0), CGameHalfTimeOFF)
            Else
                Return Nothing
            End If
        End Function

    End Class
End Namespace
