Imports SBCEngine.CEngineStd
Imports WebsiteLibrary.CSBCStd

<Serializable()> Public Class CCheckTicket
    Public TicketID As String = ""
    Public TicketType As String = ""

    Private _TicketStatus As String = ""

    Public Property TicketStatus() As String
        Get
            If _TicketStatus = "" Then
                _TicketStatus = STATUS_OPEN
            End If

            Return UCase(_TicketStatus)
        End Get
        Set(ByVal value As String)
            _TicketStatus = value
        End Set
    End Property

    Private _TicketStatusOld As String = ""

    Public ReadOnly Property IsStatusChanged() As Boolean
        Get
            Return UCase(_TicketStatus) <> UCase(_TicketStatusOld)
        End Get
    End Property

    Private _sRelatedTicketID As String = ""
    Public ReadOnly Property RelatedTicketID() As String
        Get
            Return _sRelatedTicketID
        End Get
    End Property

    Private _nSubTicketNumber As Integer
    Public ReadOnly Property SubTicketNumber() As Integer
        Get
            Return _nSubTicketNumber
        End Get
    End Property

    Public PendingStatus As String = ""

    Public PlayerID As String = ""
    Public AgentID As String = ""

    Public RiskAmount As Double = 0
    Public WinAmount As Double = 0

    Public NetAmount As Double = 0

    Public TicketBets As New List(Of CCheckTicketBet)

    Public TeaserRuleID As String
    Public IsLoseStillCheckBets As Boolean = False

    Public SiteType As String = ""
    Public TicketCompletedDate As Date

    Public Sub New(ByVal psTicketID As String)
        Me.TicketID = psTicketID
    End Sub

    Public Sub New(ByVal podrTicketBet As DataRow)
        With Me
            .TicketID = SafeString(podrTicketBet("TicketID"))
            .TicketType = SafeString(podrTicketBet("TicketType"))
            .TicketStatus = UCase(SafeString(podrTicketBet("TicketStatus")))
            ._TicketStatusOld = .TicketStatus
            .AgentID = SafeString(podrTicketBet("AgentID"))
            .PlayerID = SafeString(podrTicketBet("PlayerID"))
            .RiskAmount = SafeDouble(podrTicketBet("RiskAmount"))
            .WinAmount = SafeDouble(podrTicketBet("WinAmount"))
            .TeaserRuleID = SafeString(podrTicketBet("TeaserRuleID"))
            .IsLoseStillCheckBets = UCase(SafeString(podrTicketBet("IsLoseStillCheckBets"))) = "Y"
            .SiteType = SafeString(podrTicketBet("SiteType"))
            _sRelatedTicketID = SafeString(podrTicketBet("RelatedTicketID"))
            _nSubTicketNumber = SafeInteger(podrTicketBet("SubTicketNumber"))
            .PendingStatus = UCase(SafeString(podrTicketBet("PendingStatus")))
            .TicketCompletedDate = SafeDate(podrTicketBet("TicketCompletedDate"))
        End With
    End Sub

    Public Function GetTicketBet(ByVal psTicketBetID As String) As CCheckTicketBet
        Return Me.TicketBets.Find(Function(x) UCase(x.TicketBetID) = UCase(psTicketBetID))
    End Function

    ''' <summary>
    ''' psBetStatus is empty return count all bets
    ''' </summary>
    Public Function CountBets(Optional ByVal psBetStatus As String = "") As Int32
        If psBetStatus <> "" Then
            Return Me.TicketBets.Where(Function(x) UCase(x.TicketBetStatus) = UCase(psBetStatus)).Count()
        End If
        Return Me.TicketBets.Count
    End Function

End Class
