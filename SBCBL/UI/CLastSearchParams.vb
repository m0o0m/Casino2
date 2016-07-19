Namespace UI

    <Serializable()> _
    Public Class CLastSearchParams
        Dim _PageURL As String = ""

        Public Property PageURL() As String
            Get
                Return _PageURL
            End Get
            Set(ByVal value As String)
                _PageURL = value
            End Set
        End Property

#Region "DSL SEARCH FIELDS"

        Dim _FName As String = ""
        Dim _LName As String = ""
        Dim _Email As String = ""
        Dim _LoanType As String = ""
        Dim _SaleStaging As String = ""
        Dim _OperStaging As String = ""
        Dim _UserID As String = ""
        Dim _Channel As String = ""
        Dim _LeadForSale As Boolean = False
        Dim _SavedSearches As String = ""
        Dim _SaveCurrentSearchName As String = ""

        Public Property FName() As String
            Get
                Return _FName
            End Get
            Set(ByVal value As String)
                _FName = value
            End Set
        End Property

        Public Property LName() As String
            Get
                Return _LName
            End Get
            Set(ByVal value As String)
                _LName = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        Public Property LoanType() As String
            Get
                Return _LoanType
            End Get
            Set(ByVal value As String)
                _LoanType = value
            End Set
        End Property

        Public Property SaleStaging() As String
            Get
                Return _SaleStaging
            End Get
            Set(ByVal value As String)
                _SaleStaging = value
            End Set
        End Property

        Public Property OperStaging() As String
            Get
                Return _OperStaging
            End Get
            Set(ByVal value As String)
                _OperStaging = value
            End Set
        End Property

        Public Property UserID() As String
            Get
                Return _UserID
            End Get
            Set(ByVal value As String)
                _UserID = value
            End Set
        End Property

        Public Property Channel() As String
            Get
                Return _Channel
            End Get
            Set(ByVal value As String)
                _Channel = value
            End Set
        End Property

        Public Property LeadForSale() As Boolean
            Get
                Return _LeadForSale
            End Get
            Set(ByVal value As Boolean)
                _LeadForSale = value
            End Set
        End Property

        Public Property SavedSearches() As String
            Get
                Return _SavedSearches
            End Get
            Set(ByVal value As String)
                _SavedSearches = value
            End Set
        End Property

        Public Property SaveCurrentSearchName() As String
            Get
                Return _SaveCurrentSearchName
            End Get
            Set(ByVal value As String)
                _SaveCurrentSearchName = value
            End Set
        End Property

#End Region

#Region "CTOS SEARCH FIELDS"

        Dim _Borrower As String = ""
        Dim _SSN As String = ""
        Dim _SaleStatus As String = ""
        Dim _Processing As String = ""
        Dim _Compliance As String = ""
        Dim _Attorney As String = ""
        Dim _Negotiating As String = ""
        Dim _Closing As String = ""
        Dim _Agent As String = ""
        Dim _Processor As String = ""
        Dim _ComplianceOfficer As String = ""
        Dim _Attorneier As String = ""
        Dim _Negotiator As String = ""
        Dim _Closer As String = ""
        Dim _Branch As String = ""
        Dim _LeadType As String = ""
        Dim _TransactionType As String = ""
        Dim _Released As Boolean = False
        Dim _SortKey As String = ""
        Dim _Lender As String = ""
        Dim _PropertyAddress As String = ""
        Dim _PropertyCity As String = ""
        Dim _AccountNumber As String = ""
        Dim _LenderName As String = ""

        Public Property Borrower() As String
            Get
                Return _Borrower
            End Get
            Set(ByVal value As String)
                _Borrower = value
            End Set
        End Property

        Public Property SSN() As String
            Get
                Return _SSN
            End Get
            Set(ByVal value As String)
                _SSN = value
            End Set
        End Property

        Public Property SaleStatus() As String
            Get
                Return _SaleStatus
            End Get
            Set(ByVal value As String)
                _SaleStatus = value
            End Set
        End Property

        Public Property Processing() As String
            Get
                Return _Processing
            End Get
            Set(ByVal value As String)
                _Processing = value
            End Set
        End Property

        Public Property Compliance() As String
            Get
                Return _Compliance
            End Get
            Set(ByVal value As String)
                _Compliance = value
            End Set
        End Property

        Public Property Attorney() As String
            Get
                Return _Attorney
            End Get
            Set(ByVal value As String)
                _Attorney = value
            End Set
        End Property

        Public Property Negotiating() As String
            Get
                Return _Negotiating
            End Get
            Set(ByVal value As String)
                _Negotiating = value
            End Set
        End Property

        Public Property Closing() As String
            Get
                Return _Closing
            End Get
            Set(ByVal value As String)
                _Closing = value
            End Set
        End Property

        Public Property Agent() As String
            Get
                Return _Agent
            End Get
            Set(ByVal value As String)
                _Agent = value
            End Set
        End Property

        Public Property Processor() As String
            Get
                Return _Processor
            End Get
            Set(ByVal value As String)
                _Processor = value
            End Set
        End Property

        Public Property ComplianceOfficer() As String
            Get
                Return _ComplianceOfficer
            End Get
            Set(ByVal value As String)
                _ComplianceOfficer = value
            End Set
        End Property

        Public Property Attorneier() As String
            Get
                Return _Attorneier
            End Get
            Set(ByVal value As String)
                _Attorneier = value
            End Set
        End Property

        Public Property Negotiator() As String
            Get
                Return _Negotiator
            End Get
            Set(ByVal value As String)
                _Negotiator = value
            End Set
        End Property

        Public Property Closer() As String
            Get
                Return _Closer
            End Get
            Set(ByVal value As String)
                _Closer = value
            End Set
        End Property

        Public Property Branch() As String
            Get
                Return _Branch
            End Get
            Set(ByVal value As String)
                _Branch = value
            End Set
        End Property

        Public Property LeadType() As String
            Get
                Return _LeadType
            End Get
            Set(ByVal value As String)
                _LeadType = value
            End Set
        End Property

        Public Property TransactionType() As String
            Get
                Return _TransactionType
            End Get
            Set(ByVal value As String)
                _TransactionType = value
            End Set
        End Property

        Public Property Released() As Boolean
            Get
                Return _Released
            End Get
            Set(ByVal value As Boolean)
                _Released = value
            End Set
        End Property

        Public Property SortKey() As String
            Get
                Return _SortKey
            End Get
            Set(ByVal value As String)
                _SortKey = value
            End Set
        End Property

        Public Property Lender() As String
            Get
                Return _Lender
            End Get
            Set(ByVal value As String)
                _Lender = value
            End Set
        End Property

        Public Property PropertyAddress() As String
            Get
                Return _PropertyAddress
            End Get
            Set(ByVal value As String)
                _PropertyAddress = value
            End Set
        End Property

        Public Property PropertyCity() As String
            Get
                Return _PropertyCity
            End Get
            Set(ByVal value As String)
                _PropertyCity = value
            End Set
        End Property

        Public Property AccountNumber() As String
            Get
                Return _AccountNumber
            End Get
            Set(ByVal value As String)
                _AccountNumber = value
            End Set
        End Property

        Public Property LenderName() As String
            Get
                Return _LenderName
            End Get
            Set(ByVal value As String)
                _LenderName = value
            End Set
        End Property

#End Region

#Region "SHARE SEARCH FIELDS"

        Dim _State As String = ""
        Dim _ExternalChannel As String = ""
        Dim _LoanNum As String = ""
        Dim _DateStart As String = ""
        Dim _DateEnd As String = ""
        Dim _Phone As String = ""
        Dim _TimeZone As String = ""
        Dim _Campaign As String = ""

        Public Property State() As String
            Get
                Return _State
            End Get
            Set(ByVal value As String)
                _State = value
            End Set
        End Property

        Public Property ExternalChannel() As String
            Get
                Return _ExternalChannel
            End Get
            Set(ByVal value As String)
                _ExternalChannel = value
            End Set
        End Property

        Public Property LoanNum() As String
            Get
                Return _LoanNum
            End Get
            Set(ByVal value As String)
                _LoanNum = value
            End Set
        End Property

        Public Property DateStart() As String
            Get
                Return _DateStart
            End Get
            Set(ByVal value As String)
                _DateStart = value
            End Set
        End Property

        Public Property DateEnd() As String
            Get
                Return _DateEnd
            End Get
            Set(ByVal value As String)
                _DateEnd = value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return _Phone
            End Get
            Set(ByVal value As String)
                _Phone = value
            End Set
        End Property

        Public Property TimeZone() As String
            Get
                Return _TimeZone
            End Get
            Set(ByVal value As String)
                _TimeZone = value
            End Set
        End Property

        Public Property Campaign() As String
            Get
                Return _Campaign
            End Get
            Set(ByVal value As String)
                _Campaign = value
            End Set
        End Property
#End Region

    End Class

End Namespace

