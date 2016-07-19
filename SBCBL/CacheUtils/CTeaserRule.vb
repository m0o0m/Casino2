Imports SBCBL.std

Namespace CacheUtils
    <Serializable()> _
    Public Class CTeaserRule
        Private _sTeaserRuleID As String
        Private _sTeaserRuleName As String
        Private _nMinTeam As Integer
        Private _nMaxTeam As Integer
        Private _sBasketballPoint As Double
        Private _sFootballPoint As Double
        Private _bTiesLose As Boolean

        Public ReadOnly Property TeaserRuleID() As String
            Get
                Return _sTeaserRuleID
            End Get
        End Property

        Public ReadOnly Property TeaserRuleName() As String
            Get
                Return _sTeaserRuleName
            End Get
        End Property

        Public ReadOnly Property MinTeam() As Integer
            Get
                Return _nMinTeam
            End Get
        End Property

        Public ReadOnly Property MaxTeam() As Integer
            Get
                Return _nMaxTeam
            End Get
        End Property

        Public ReadOnly Property BasketballPoint() As Double
            Get
                Return _sBasketballPoint
            End Get
        End Property

        Public ReadOnly Property FootbalPoint() As Double
            Get
                Return _sFootballPoint
            End Get
        End Property

        Public ReadOnly Property TiesLose() As Boolean
            Get
                Return _bTiesLose
            End Get
        End Property

        Public Sub New(ByVal poDR As DataRow)
            _sTeaserRuleID = SafeString(poDR("TeaserRuleID"))
            _sTeaserRuleName = SafeString(poDR("TeaserRuleName"))
            _nMinTeam = SafeInteger(poDR("MinTeam"))
            _nMaxTeam = SafeInteger(poDR("MaxTeam"))
            _sBasketballPoint = SafeDouble(poDR("BasketballPoint"))
            _sFootballPoint = SafeDouble(poDR("FootballPoint"))
            _bTiesLose = SafeString(poDR("IsTiesLose")) = "Y"
        End Sub
    End Class

    <Serializable()> _
    Public Class CTeaserRuleList
        Inherits List(Of CTeaserRule)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal poTable As DataTable)
            Dim oTeaserRule As CTeaserRule

            For Each oDR As DataRow In poTable.Rows
                oTeaserRule = New CTeaserRule(oDR)
                Me.Add(oTeaserRule)
            Next
        End Sub
    End Class

End Namespace