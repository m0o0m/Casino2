Public NotInheritable Class TimestampHelper
    Private Sub New()
    End Sub
    Public Shared ReadOnly UnixStart As DateTimeOffset
    Public Shared ReadOnly MaxUnixSeconds As Long
    Public Shared ReadOnly MaxUnixMilliseconds As Long
    Public Shared ReadOnly MaxUnixMicroseconds As Long

    Public Const TicksInOneMicrosecond As Long = 10L
    Public Const TicksInOneMillisecond As Long = 10000L
    Public Const TicksInOneSecond As Long = 10000000L

    Shared Sub New()
        UnixStart = New DateTimeOffset(1970, 1, 1, 0, 0, 0, _
            TimeSpan.Zero)
        MaxUnixSeconds = Convert.ToInt64((DateTimeOffset.MaxValue - UnixStart).TotalSeconds)
        MaxUnixMilliseconds = Convert.ToInt64((DateTimeOffset.MaxValue - UnixStart).TotalMilliseconds)
        MaxUnixMicroseconds = Convert.ToInt64((DateTimeOffset.MaxValue - UnixStart).Ticks / TicksInOneMicrosecond)
    End Sub

    ''' <summary>
    ''' Allows for the use of alternative timestamp providers.
    ''' </summary>
    Public Shared UtcNow As Func(Of DateTimeOffset) = Function() DateTimePrecise.UtcNowOffset

    Public Shared Function ToCassandraTimestamp(dt As DateTimeOffset) As Long
        ' we are using the microsecond format from 1/1/1970 00:00:00 UTC same as the Cassandra server
        Return CLng((dt - UnixStart).Ticks / TicksInOneMicrosecond)
    End Function

    Public Shared Function FromCassandraTimestamp(ts As Long) As DateTimeOffset
        ' convert a timestamp in seconds to ticks
        ' ** this should never happen, but it is in here for good measure **
        If ts <= MaxUnixSeconds Then
            ts *= TicksInOneSecond
        End If

        ' convert a timestamp in milliseconds to ticks
        If ts <= MaxUnixMilliseconds Then
            ts *= TicksInOneMillisecond
        End If

        ' convert a timestamp in microseconds to ticks
        If ts <= MaxUnixMicroseconds Then
            ts *= TicksInOneMicrosecond
        End If

        Return UnixStart.AddTicks(ts)
    End Function
End Class
