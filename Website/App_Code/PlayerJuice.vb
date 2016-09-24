Imports Microsoft.VisualBasic

Public Class PlayerJuice

    Private _sport As String
    Public Property Sport() As String
        Get
            Return _sport
        End Get
        Set(ByVal value As String)
            _sport = value
        End Set
    End Property

    Private _gameType As String
    Public Property GameType() As String
        Get
            Return _gameType
        End Get
        Set(ByVal value As String)
            _gameType = value
        End Set
    End Property

    Private _betType As Integer?
    Public Property BetType() As Integer?
        Get
            Return _betType
        End Get
        Set(ByVal value As Integer?)
            _betType = value
        End Set
    End Property

    Private _gm As Boolean
    Public Property GM() As Boolean
        Get
            Return _gm
        End Get
        Set(ByVal value As Boolean)
            _gm = value
        End Set
    End Property

    Private _fh As Boolean
    Public Property FH() As Boolean
        Get
            Return _fh
        End Get
        Set(ByVal value As Boolean)
            _fh = value
        End Set
    End Property

    Private _sh As Boolean
    Public Property SH() As Boolean
        Get
            Return _sh
        End Get
        Set(ByVal value As Boolean)
            _sh = value
        End Set
    End Property

    Private _gmValue As Decimal?
    Public Property GmValue() As Decimal?
        Get
            Return _gmValue
        End Get
        Set(ByVal value As Decimal?)
            _gmValue = value
        End Set
    End Property

    Private _gmId As String
    Public Property GmId() As String
        Get
            Return _gmId
        End Get
        Set(ByVal value As String)
            _gmId = value
        End Set
    End Property

    Private _gmCate As String
    Public Property GmCate() As String
        Get
            Return _gmCate
        End Get
        Set(ByVal value As String)
            _gmCate = value
        End Set
    End Property

    Private _fhValue As Decimal?
    Public Property FhValue() As Decimal?
        Get
            Return _fhValue
        End Get
        Set(ByVal value As Decimal?)
            _fhValue = value
        End Set
    End Property

    Private _fhId As String
    Public Property FhId() As String
        Get
            Return _fhId
        End Get
        Set(ByVal value As String)
            _fhId = value
        End Set
    End Property

    Private _fhCate As String
    Public Property FhCate() As String
        Get
            Return _fhCate
        End Get
        Set(ByVal value As String)
            _fhCate = value
        End Set
    End Property

    Private _shValue As Decimal?
    Public Property ShValue() As Decimal?
        Get
            Return _shValue
        End Get
        Set(ByVal value As Decimal?)
            _shValue = value
        End Set
    End Property

    Private _shId As String
    Public Property ShId() As String
        Get
            Return _shId
        End Get
        Set(ByVal value As String)
            _shId = value
        End Set
    End Property

    Private _shCate As String
    Public Property ShCate() As String
        Get
            Return _shCate
        End Get
        Set(ByVal value As String)
            _shCate = value
        End Set
    End Property

    Private _rowIndex As Integer
    Public Property RowIndex() As Integer
        Get
            Return _rowIndex
        End Get
        Set(ByVal value As Integer)
            _rowIndex = value
        End Set
    End Property

End Class
