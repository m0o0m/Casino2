Imports SBCBL.std
Imports SBCBL.Utils

Imports WebsiteLibrary.DBUtils

Namespace CacheUtils

    <Serializable()> Public Class CPlayerTemplateLimit

#Region "Fields"

        Private _PlayerTemplateLimitID As String = ""
        Private _PlayerTemplateID As String = ""
        Private _GameType As String = ""
        Private _Context As String = ""
        Private _MaxSingle As Double = 0
        Private _MaxParlay As Double = 0
        Private _MaxTeaser As Double = 0
        Private _MaxReverse As Double = 0

        Private _ParlayLimit As Double = 0
        Private _TeaserLimit As Double = 0
        Private _PropLimit As Double = 0
        Private _WagerLimit As Double = 0

#End Region

#Region "Constructors"

        Public Sub New()
            _PlayerTemplateLimitID = newGUID()
        End Sub

        Public Sub New(ByVal podrData As DataRow)
            _PlayerTemplateLimitID = SafeString(podrData("PlayerTemplateLimitID"))
            _PlayerTemplateID = SafeString(podrData("PlayerTemplateID"))
            _GameType = SafeString(podrData("GameType"))
            _Context = SafeString(podrData("Context"))
            _MaxParlay = SafeDouble(podrData("MaxParlay"))
            _MaxReverse = SafeDouble(podrData("MaxReverse"))
            _MaxSingle = SafeDouble(podrData("MaxSingle"))
            _MaxTeaser = SafeDouble(podrData("MaxTeaser"))
            _ParlayLimit = SafeDouble(podrData("ParlayLimit"))
            _TeaserLimit = SafeDouble(podrData("TeaserLimit"))
            _PropLimit = SafeDouble(podrData("PropLimit"))
            _WagerLimit = SafeDouble(podrData("WagerLimit"))
        End Sub

#End Region

#Region "Properties"

        Public Property PlayerTemplateLimitID() As String
            Get
                Return _PlayerTemplateLimitID
            End Get
            Set(ByVal value As String)
                _PlayerTemplateLimitID = value
            End Set
        End Property

        Public Property PlayerTemplateID() As String
            Get
                Return _PlayerTemplateID
            End Get
            Set(ByVal value As String)
                _PlayerTemplateID = value
            End Set
        End Property

        Public Property GameType() As String
            Get
                Return _GameType
            End Get
            Set(ByVal value As String)
                _GameType = value
            End Set
        End Property

        Public Property Context() As String
            Get
                Return _Context
            End Get
            Set(ByVal value As String)
                _Context = value
            End Set
        End Property

        Public Property MaxParlay() As Double
            Get
                Return _MaxParlay
            End Get
            Set(ByVal value As Double)
                _MaxParlay = value
            End Set
        End Property

        Public Property MaxReverse() As Double
            Get
                Return _MaxReverse
            End Get
            Set(ByVal value As Double)
                _MaxReverse = value
            End Set
        End Property

        Public Property MaxSingle() As Double
            Get
                Return _MaxSingle
            End Get
            Set(ByVal value As Double)
                _MaxSingle = value
            End Set
        End Property

        Public Property MaxTeaser() As Double
            Get
                Return _MaxTeaser
            End Get
            Set(ByVal value As Double)
                _MaxTeaser = value
            End Set
        End Property

        Public Property WagerLimit() As Double
            Get
                Return _WagerLimit
            End Get
            Set(ByVal value As Double)
                _WagerLimit = value
            End Set
        End Property
        Public Property ParlayLimit() As Double
            Get
                Return _ParlayLimit
            End Get
            Set(ByVal value As Double)
                _ParlayLimit = value
            End Set
        End Property
        Public Property TeaserLimit() As Double
            Get
                Return _TeaserLimit
            End Get
            Set(ByVal value As Double)
                _TeaserLimit = value
            End Set
        End Property
        Public Property PropLimit() As Double
            Get
                Return _PropLimit
            End Get
            Set(ByVal value As Double)
                _PropLimit = value
            End Set
        End Property
#End Region

    End Class

    Public Enum LimitName
        MaxSingle
        MaxParlay
        MaxTeaser
        MaxReverse
    End Enum

    <Serializable()> Public Class CPlayerTemplateLimitList
        Inherits List(Of CPlayerTemplateLimit)

        <NonSerialized()> Private log As log4net.ILog = log4net.LogManager.GetLogger(GetType(CPlayerTemplateLimitList))

        Public Overloads Sub Sort()
            Try
                Dim oSorter As New CPropertiesSorter(Of CPlayerTemplateLimit)
                oSorter.AddSort("GameType")
                oSorter.AddSort("Context")

                Me.Sort(oSorter)
            Catch ex As Exception
                log.Error("Cannot sort by GameType, Context", ex)
            End Try
        End Sub

        Public Function GetLimits(ByVal psGameType As String) As CPlayerTemplateLimitList
            Dim oLimits As List(Of CPlayerTemplateLimit) = (From oLimit As CPlayerTemplateLimit In Me _
                                                            Where oLimit.GameType = psGameType _
                                                            Order By oLimit.GameType, oLimit.Context _
                                                            Select oLimit).ToList

            Return CType(oLimits, CPlayerTemplateLimitList)
        End Function

#Region "By GameType, Context"

        Public Function GetLimit(ByVal psGameType As String, ByVal psContext As String) As CPlayerTemplateLimit
            Return Me.Find(Function(oLimit) UCase(oLimit.GameType) = UCase(psGameType) _
                                                            AndAlso UCase(oLimit.Context) = UCase(psContext))
        End Function

        Public Function GetLimitValue(ByVal psGameType As String, ByVal psContext As String, ByVal poLimitName As LimitName) As Double
            Dim oLimit As CPlayerTemplateLimit = GetLimit(psGameType, psContext)

            If oLimit IsNot Nothing Then
                Select Case poLimitName
                    Case LimitName.MaxParlay
                        Return oLimit.MaxParlay
                    Case LimitName.MaxReverse
                        Return oLimit.MaxReverse
                    Case LimitName.MaxSingle
                        Return oLimit.MaxSingle
                    Case LimitName.MaxTeaser
                        Return oLimit.MaxTeaser
                End Select
            End If
            Return 0
        End Function

#End Region

#Region "By Context: after get the list of limits by GameType"

        ''' <summary>
        ''' Use it after get the list of limits by GameType
        ''' </summary>
        Public Function GetLimit(ByVal psContext As String) As CPlayerTemplateLimit
            Return Me.Find(Function(oLimit) UCase(oLimit.Context) = UCase(psContext))
        End Function

        ''' <summary>
        ''' Use it after get the list of limits by GameType
        ''' </summary>
        Public Function GetLimitValue(ByVal psContext As String, ByVal poLimitName As LimitName) As Double
            Dim oLimit As CPlayerTemplateLimit = GetLimit(psContext)

            If oLimit IsNot Nothing Then
                Select Case poLimitName
                    Case LimitName.MaxParlay
                        Return oLimit.MaxParlay

                    Case LimitName.MaxReverse
                        Return oLimit.MaxReverse

                    Case LimitName.MaxSingle
                        Return oLimit.MaxSingle

                    Case LimitName.MaxTeaser
                        Return oLimit.MaxTeaser
                End Select

            End If
            Return 0
        End Function

#End Region

    End Class

End Namespace

