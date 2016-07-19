Imports System.Xml
Imports WebsiteLibrary.CXMLUtils
Imports WebsiteLibrary.DBUtils
Imports log4net
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Web
Imports System.Data
Imports SBCBL.std
Imports SBCBL.CEnums
Imports SBCBL.CFileDBKeys
Imports System.Reflection

Namespace CacheUtils

    <Serializable()> Public Class CGame


#Region "Fields"

        Private _Current As Boolean
        Private _OneH As Boolean
        Private _TwoH As Boolean
        Private _Quarter1 As Boolean
        Private _Quarter2 As Boolean
        Private _Quarter3 As Boolean
        Private _Quarter4 As Boolean
        Private _GameType As String
        Private _BookMaker As String
        Private _Quarter As Boolean
#End Region

#Region "Properties"
        Public Property Quarter() As Boolean
            Get
                Return _Quarter
            End Get
            Set(ByVal value As Boolean)
                _Quarter = value
            End Set
        End Property
        Public Property Current() As Boolean
            Get
                Return _Current
            End Get
            Set(ByVal value As Boolean)
                _Current = value
            End Set
        End Property

        Public Property OneH() As Boolean
            Get
                Return _OneH
            End Get
            Set(ByVal value As Boolean)
                _OneH = value
            End Set
        End Property

        Public Property TwoH() As Boolean
            Get
                Return _TwoH
            End Get
            Set(ByVal value As Boolean)
                _TwoH = value
            End Set
        End Property

        Public Property Quarter1() As Boolean
            Get
                Return _Quarter1
            End Get
            Set(ByVal value As Boolean)
                _Quarter1 = value
            End Set
        End Property

        Public Property Quarter2() As Boolean
            Get
                Return _Quarter2
            End Get
            Set(ByVal value As Boolean)
                _Quarter2 = value
            End Set
        End Property

        Public Property Quarter3() As Boolean
            Get
                Return _Quarter3
            End Get
            Set(ByVal value As Boolean)
                _Quarter3 = value
            End Set
        End Property

        Public Property Quarter4() As Boolean
            Get
                Return _Quarter4
            End Get
            Set(ByVal value As Boolean)
                _Quarter4 = value
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

        Public Property BookMaker() As String
            Get
                Return _BookMaker
            End Get
            Set(ByVal value As String)
                _BookMaker = value
            End Set
        End Property
#End Region

    End Class

End Namespace