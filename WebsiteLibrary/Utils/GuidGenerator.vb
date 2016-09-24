Imports System.Net
Imports System.Net.NetworkInformation

''' <summary>
''' Used for generating UUID based on RFC 4122.
''' </summary>
''' <seealso href="http://www.ietf.org/rfc/rfc4122.txt">RFC 4122 - A Universally Unique IDentifier (UUID) URN Namespace</seealso>
Public NotInheritable Class GuidGenerator
    Private Sub New()
    End Sub
    Private Shared ReadOnly Random As Random
    Private Shared ReadOnly Lock As New Object()

    Private Shared _lastTimestampForNoDuplicatesGeneration As DateTimeOffset = TimestampHelper.UtcNow()

    ' number of bytes in uuid
    Private Const ByteArraySize As Integer = 16

    ' multiplex variant info
    Private Const VariantByte As Integer = 8
    Private Const VariantByteMask As Integer = &H3F
    Private Const VariantByteShift As Integer = &H80

    ' multiplex version info
    Private Const VersionByte As Integer = 7
    Private Const VersionByteMask As Integer = &HF
    Private Const VersionByteShift As Integer = 4

    ' indexes within the uuid array for certain boundaries
    Private Const TimestampByte As Byte = 0
    Private Const GuidClockSequenceByte As Byte = 8
    Private Const NodeByte As Byte = 10

    ' offset to move from 1/1/0001, which is 0-time for .NET, to gregorian 0-time of 10/15/1582
    Private Shared ReadOnly GregorianCalendarStart As New DateTimeOffset(1582, 10, 15, 0, 0, 0, _
        TimeSpan.Zero)

    Public Shared Property GuidGeneration() As GuidGeneration
        Get
            Return m_GuidGeneration
        End Get
        Set(value As GuidGeneration)
            m_GuidGeneration = Value
        End Set
    End Property
    Private Shared m_GuidGeneration As GuidGeneration

    Public Shared Property NodeBytes() As Byte()
        Get
            Return m_NodeBytes
        End Get
        Set(value As Byte())
            m_NodeBytes = Value
        End Set
    End Property
    Private Shared m_NodeBytes As Byte()
    Public Shared Property ClockSequenceBytes() As Byte()
        Get
            Return m_ClockSequenceBytes
        End Get
        Set(value As Byte())
            m_ClockSequenceBytes = Value
        End Set
    End Property
    Private Shared m_ClockSequenceBytes As Byte()

    Shared Sub New()
        Random = New Random()

        GuidGeneration = GuidGeneration.NoDuplicates
        NodeBytes = GenerateNodeBytes()
        ClockSequenceBytes = GenerateClockSequenceBytes()
    End Sub

    ''' <summary>
    ''' Generates a random value for the node.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GenerateNodeBytes() As Byte()
        Dim node = New Byte(5) {}

        Random.NextBytes(node)
        Return node
    End Function

    ''' <summary>
    ''' Generates a node based on the first 6 bytes of an IP address.
    ''' </summary>
    ''' <param name="ip"></param>
    Public Shared Function GenerateNodeBytes(ip As IPAddress) As Byte()
        If ip Is Nothing Then
            Throw New ArgumentNullException("ip")
        End If

        Dim bytes = ip.GetAddressBytes()

        If bytes.Length < 6 Then
            Throw New ArgumentOutOfRangeException("ip", "The passed in IP address must contain at least 6 bytes.")
        End If

        Dim node = New Byte(5) {}
        Array.Copy(bytes, node, 6)

        Return node
    End Function

    ''' <summary>
    ''' Generates a node based on the bytes of the MAC address.
    ''' </summary>
    ''' <param name="mac"></param>
    ''' <remarks>The machines MAC address can be retrieved from <see cref="NetworkInterface.GetPhysicalAddress"/>.</remarks>
    Public Shared Function GenerateNodeBytes(mac As PhysicalAddress) As Byte()
        If mac Is Nothing Then
            Throw New ArgumentNullException("mac")
        End If

        Dim node = mac.GetAddressBytes()

        Return node
    End Function

    ''' <summary>
    ''' Generates a random clock sequence.
    ''' </summary>
    Public Shared Function GenerateClockSequenceBytes() As Byte()
        Dim bytes = New Byte(1) {}
        Random.NextBytes(bytes)
        Return bytes
    End Function

    ''' <summary>
    ''' In order to maintain a constant value we need to get a two byte hash from the DateTime.
    ''' </summary>
    Public Shared Function GenerateClockSequenceBytes(dt As DateTime) As Byte()
        Dim utc = dt.ToUniversalTime()
        Return GenerateClockSequenceBytes(utc.Ticks)
    End Function

    ''' <summary>
    ''' In order to maintain a constant value we need to get a two byte hash from the DateTime.
    ''' </summary>
    Public Shared Function GenerateClockSequenceBytes(dt As DateTimeOffset) As Byte()
        Dim utc = dt.ToUniversalTime()
        Return GenerateClockSequenceBytes(utc.Ticks)
    End Function

    Public Shared Function GenerateClockSequenceBytes(ticks As Long) As Byte()
        Dim bytes = BitConverter.GetBytes(ticks)

        If bytes.Length = 0 Then
            Return New Byte() {&H0, &H0}
        End If

        If bytes.Length = 1 Then
            Return New Byte() {&H0, bytes(0)}
        End If

        Return New Byte() {bytes(0), bytes(1)}
    End Function

    Public Shared Function GetUuidVersion(guid As Guid) As GuidVersion
        Dim bytes As Byte() = guid.ToByteArray()
        Return DirectCast((bytes(VersionByte) And &HFF) >> VersionByteShift, GuidVersion)
    End Function

    Public Shared Function GetDateTimeOffset(guid As Guid) As DateTimeOffset
        Dim bytes As Byte() = guid.ToByteArray()

        ' reverse the version
        bytes(VersionByte) = bytes(VersionByte) And CByte(VersionByteMask)
        bytes(VersionByte) = bytes(VersionByte) Or CByte(CByte(GuidVersion.TimeBased) >> VersionByteShift)

        Dim timestampBytes As Byte() = New Byte(7) {}
        Array.Copy(bytes, TimestampByte, timestampBytes, 0, 8)

        Dim timestamp As Long = BitConverter.ToInt64(timestampBytes, 0)
        Dim ticks As Long = timestamp + GregorianCalendarStart.Ticks

        Return New DateTimeOffset(ticks, TimeSpan.Zero)
    End Function

    Public Shared Function GetDateTime(guid As Guid) As DateTime
        Return GetDateTimeOffset(guid).DateTime
    End Function

    Public Shared Function GetLocalDateTime(guid As Guid) As DateTime
        Return GetDateTimeOffset(guid).LocalDateTime
    End Function

    Public Shared Function GetUtcDateTime(guid As Guid) As DateTime
        Return GetDateTimeOffset(guid).UtcDateTime
    End Function

    Public Shared Function GenerateTimeBasedGuid() As Guid
        Select Case GuidGeneration
            Case GuidGeneration.Fast
                Return GenerateTimeBasedGuid(TimestampHelper.UtcNow(), ClockSequenceBytes, NodeBytes)

                'Case GuidGeneration.NoDuplicates
            Case Else
                SyncLock Lock
                    Dim ts = TimestampHelper.UtcNow()

                    If ts <= _lastTimestampForNoDuplicatesGeneration Then
                        ClockSequenceBytes = GenerateClockSequenceBytes()
                    End If

                    _lastTimestampForNoDuplicatesGeneration = ts

                    Return GenerateTimeBasedGuid(ts, ClockSequenceBytes, NodeBytes)
                End SyncLock
        End Select
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTime) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), NodeBytes)
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTimeOffset) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), NodeBytes)
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTime, mac As PhysicalAddress) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), GenerateNodeBytes(mac))
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTimeOffset, mac As PhysicalAddress) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), GenerateNodeBytes(mac))
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTime, ip As IPAddress) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), GenerateNodeBytes(ip))
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTimeOffset, ip As IPAddress) As Guid
        Return GenerateTimeBasedGuid(dateTime, GenerateClockSequenceBytes(dateTime), GenerateNodeBytes(ip))
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTime, clockSequence As Byte(), node As Byte()) As Guid
        Return GenerateTimeBasedGuid(New DateTimeOffset(dateTime), clockSequence, node)
    End Function

    Public Shared Function GenerateTimeBasedGuid(dateTime As DateTimeOffset, clockSequence As Byte(), node As Byte()) As Guid
        If clockSequence Is Nothing Then
            Throw New ArgumentNullException("clockSequence")
        End If

        If node Is Nothing Then
            Throw New ArgumentNullException("node")
        End If

        If clockSequence.Length <> 2 Then
            Throw New ArgumentOutOfRangeException("clockSequence", "The clockSequence must be 2 bytes.")
        End If

        If node.Length <> 6 Then
            Throw New ArgumentOutOfRangeException("node", "The node must be 6 bytes.")
        End If

        Dim ticks As Long = (dateTime - GregorianCalendarStart).Ticks
        Dim guid As Byte() = New Byte(ByteArraySize - 1) {}
        Dim timestamp As Byte() = BitConverter.GetBytes(ticks)

        ' copy node
        Array.Copy(node, 0, guid, NodeByte, Math.Min(6, node.Length))

        ' copy clock sequence
        Array.Copy(clockSequence, 0, guid, GuidClockSequenceByte, Math.Min(2, clockSequence.Length))

        ' copy timestamp
        Array.Copy(timestamp, 0, guid, TimestampByte, Math.Min(8, timestamp.Length))

        ' set the variant
        guid(VariantByte) = guid(VariantByte) And CByte(VariantByteMask)
        guid(VariantByte) = guid(VariantByte) Or CByte(VariantByteShift)

        ' set the version
        guid(VersionByte) = guid(VersionByte) And CByte(VersionByteMask)
        guid(VersionByte) = guid(VersionByte) Or CByte(CByte(GuidVersion.TimeBased) << VersionByteShift)

        Return New Guid(guid)
    End Function

End Class

Public Enum GuidVersion
    TimeBased = &H1
    Reserved = &H2
    NameBased = &H3
    Random = &H4
End Enum

Public Enum GuidGeneration
    Fast
    NoDuplicates
End Enum