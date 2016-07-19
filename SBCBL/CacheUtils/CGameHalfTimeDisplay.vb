Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CGameHalfTimeDisplay

        Private _SportType As String
        Private _SecondHOff As Integer

        Public Sub New(ByVal psSportType As String, ByVal pnSecondHOff As Integer)
            _SportType = psSportType
            _SecondHOff = pnSecondHOff
        End Sub

        Public Property SportType() As String
            Get
                Return _SportType
            End Get
            Set(ByVal value As String)
                _SportType = value
            End Set
        End Property

        Public Property SecondHOff() As Integer
            Get
                Return _SecondHOff
            End Get
            Set(ByVal value As Integer)
                _SecondHOff = value
            End Set
        End Property
    End Class

    <Serializable()> Public Class CGameHalfTimeDisplayList
        Inherits List(Of CGameHalfTimeDisplay)

        Public Function GetGameHalfTimeDisplay(ByVal psSportType As String) As CGameHalfTimeDisplay
            Dim lstGameHalfTimeDisplay As List(Of CGameHalfTimeDisplay) = (From oGameHalfTimeDisplay As CGameHalfTimeDisplay In Me _
                                                            Where oGameHalfTimeDisplay.SportType = psSportType _
                                                            Order By oGameHalfTimeDisplay.SportType _
                                                            Select oGameHalfTimeDisplay).ToList
            If lstGameHalfTimeDisplay IsNot Nothing AndAlso lstGameHalfTimeDisplay.Count > 0 Then
                Return CType(lstGameHalfTimeDisplay(0), CGameHalfTimeDisplay)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetGameHalfTimeDisplayByGameType(ByVal psGameType As String) As Integer
            If IsBaseball(psGameType) Then
                If GetGameHalfTimeDisplay("Baseball") IsNot Nothing Then
                    Return GetGameHalfTimeDisplay("Baseball").SecondHOff
                Else
                    Return 0
                End If

            End If
            If IsFootball(psGameType) Then
                If GetGameHalfTimeDisplay("Football") IsNot Nothing Then
                    Return GetGameHalfTimeDisplay("Football").SecondHOff
                Else
                    Return 0
                End If

            End If
            If IsBasketball(psGameType) Then
                If GetGameHalfTimeDisplay("Basketball") IsNot Nothing Then
                    Return GetGameHalfTimeDisplay("Basketball").SecondHOff
                Else
                    Return 0
                End If

            End If
            If IsHockey(psGameType) Then
                If GetGameHalfTimeDisplay("Hockey") IsNot Nothing Then
                    Return GetGameHalfTimeDisplay("Hockey").SecondHOff
                Else
                    Return 0
                End If

            End If
            If IsSoccer(psGameType) Then
                If GetGameHalfTimeDisplay("Soccer") IsNot Nothing Then
                    Return GetGameHalfTimeDisplay("Soccer").SecondHOff
                Else
                    Return 0
                End If

            End If
            Return 0
        End Function


    End Class

End Namespace
