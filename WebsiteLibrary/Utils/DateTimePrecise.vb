Imports System.Diagnostics

Public Class DateTimePrecise
    Private Shared ReadOnly Instance As New DateTimePrecise()

    Public Shared ReadOnly Property Now() As DateTime
        Get
            Return Instance.GetUtcNow().LocalDateTime
        End Get
    End Property

    Public Shared ReadOnly Property UtcNow() As DateTime
        Get
            Return Instance.GetUtcNow().UtcDateTime
        End Get
    End Property

    Public Shared ReadOnly Property NowOffset() As DateTimeOffset
        Get
            Return Instance.GetUtcNow().ToLocalTime()
        End Get
    End Property

    Public Shared ReadOnly Property UtcNowOffset() As DateTimeOffset
        Get
            Return Instance.GetUtcNow()
        End Get
    End Property

    Private Const TicksInOneSecond As Long = 10000000L

    Private ReadOnly _divergentSeconds As Double
    Private ReadOnly _syncSeconds As Double
    Private ReadOnly _stopwatch As Stopwatch
    Private _baseTime As DateTimeOffset

    Public Sub New(Optional syncSeconds As Integer = 1, Optional divergentSeconds As Integer = 1)
        _syncSeconds = syncSeconds
        _divergentSeconds = divergentSeconds

        _stopwatch = New Stopwatch()

        Syncronize()
    End Sub

    Private Sub Syncronize()
        SyncLock _stopwatch
            _baseTime = DateTimeOffset.UtcNow
            _stopwatch.Stop()
            _stopwatch.Reset()
            _stopwatch.Start()
        End SyncLock
    End Sub

    Public Function GetUtcNow() As DateTimeOffset
        Dim now = DateTimeOffset.UtcNow
        Dim elapsed = _stopwatch.Elapsed

        If elapsed.TotalSeconds > _syncSeconds Then
            Syncronize()

            ' account for any time that has passed since the stopwatch was syncronized
            elapsed = _stopwatch.Elapsed
        End If

        '*
        '			 * The Stopwatch has many bugs associated with it, so when we are in doubt of the results
        '			 * we are going to default to DateTimeOffset.UtcNow
        '			 * http://stackoverflow.com/questions/1008345
        '			 *


        ' check for elapsed being less than zero
        If elapsed < TimeSpan.Zero Then
            Return now
        End If

        Dim preciseNow = _baseTime + elapsed

        ' make sure the two clocks don't diverge by more than defined seconds
        If Math.Abs((preciseNow - now).TotalSeconds) > _divergentSeconds Then
            Return now
        End If

        Return _baseTime + elapsed
    End Function
End Class
