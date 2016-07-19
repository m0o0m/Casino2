Imports System.Xml

Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils

Namespace CacheUtils
    <Serializable()> Public Class COddsRule
#Region "Fields"

        Private _ID As String = ""
        Private _GreaterThan As Double
        Private _LowerThan As Double
        Private _IsOddRuleLocked As String = ""
        Private _Increase As Single


#End Region
#Region "Properties"
        Public ReadOnly Property ID() As String
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property GreaterThan() As Double
            Get
                Return _GreaterThan
            End Get
        End Property

        Public ReadOnly Property LowerThan() As Double
            Get
                Return _LowerThan
            End Get
        End Property

        Public ReadOnly Property Increase() As Single
            Get
                Return _Increase
            End Get
        End Property

        Public ReadOnly Property IsOddRuleLocked() As Boolean
            Get
                Return _IsOddRuleLocked = "Y"
            End Get
        End Property

#End Region
#Region "Constructors"
        Public Sub New()
        End Sub
        Public Sub New(ByVal podrData As DataRow, Optional ByVal pbAgent As Boolean = False)
            If pbAgent Then
                _ID = SafeString(podrData("OddRuleID"))
                _GreaterThan = SafeDouble(podrData("GreaterThan"))
                _LowerThan = SafeDouble(podrData("LowerThan"))
                _Increase = SafeSingle(podrData("Increase"))
                _IsOddRuleLocked = SafeString(podrData("Islock"))
            Else
                _ID = SafeString(podrData("OddRuleID"))
                _GreaterThan = SafeDouble(podrData("GreaterThan"))
                _LowerThan = SafeDouble(podrData("LowerThan"))
                _Increase = SafeSingle(podrData("Increase"))
                _IsOddRuleLocked = SafeString(podrData("Islock"))
            End If
            
        End Sub
#End Region
    End Class
  


End Namespace

