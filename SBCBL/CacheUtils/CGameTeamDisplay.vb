Imports SBCBL.std
Imports SBCBL.Managers

Namespace CacheUtils
    <Serializable()> Public Class CGameTeamDisplay

        Private _SportType As String
        Private _GameTimeOffLine As Integer
        Private _GameTimeDisplay As Integer

        Public Sub New(ByVal psSportType As String, ByVal pnGameTimeOffLine As Integer, ByVal pnGameTimeDisplay As Integer)
            _SportType = psSportType
            _GameTimeOffLine = pnGameTimeOffLine
            _GameTimeDisplay = pnGameTimeDisplay
        End Sub

        Public Property SportType() As String
            Get
                Return _SportType
            End Get
            Set(ByVal value As String)
                _SportType = value
            End Set
        End Property

        Public Property GameTimeOffLine() As Integer
            Get
                Return _GameTimeOffLine
            End Get
            Set(ByVal value As Integer)
                _GameTimeOffLine = value
            End Set
        End Property

        Public Property GameTimeDisplay() As Integer
            Get
                Return _GameTimeDisplay
            End Get
            Set(ByVal value As Integer)
                _GameTimeDisplay = value
            End Set
        End Property
    End Class

    <Serializable()> Public Class CGameTeamDisplayList
        Inherits List(Of CGameTeamDisplay)

        Public Function GetGameTeamDisplay(ByVal psSportType As String) As CGameTeamDisplay
            Dim lstGameTeamDisplay As List(Of CGameTeamDisplay) = (From oGameTeamDisplay As CGameTeamDisplay In Me _
                                                            Where oGameTeamDisplay.SportType = psSportType _
                                                            Order By oGameTeamDisplay.SportType _
                                                            Select oGameTeamDisplay).ToList
            If lstGameTeamDisplay IsNot Nothing AndAlso lstGameTeamDisplay.Count > 0 Then
                Return CType(lstGameTeamDisplay(0), CGameTeamDisplay)
            Else
                Return Nothing
            End If
        End Function
    End Class

End Namespace
