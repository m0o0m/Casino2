Imports Microsoft.VisualBasic

Public Class CFormatPrettyTime
    Const MINUTESINANHOUR As Long = 60
    Const MINUTESINADAY As Long = 1440 '60 * 24 = 1,440
    Const MINUTESINAMONTH As Long = 43200 '60 * 24 * 30 = 43,200
    Const MINUTESINAYEAR As Long = 525600 '60 * 24 * 365 = 525,600

    Public ReadOnly Property totalMinutes() As Long
        Get
            Dim temp As Long = 0
            temp += years * MINUTESINAYEAR
            temp += months * MINUTESINAMONTH
            temp += days * MINUTESINADAY
            temp += hours * MINUTESINANHOUR
            temp += minutes
            Return temp
        End Get
    End Property

    Private _minutes As Long = 0
    Public Property minutes() As Long
        Get
            Return _minutes
        End Get
        Set(ByVal Value As Long)
            _minutes = Value
        End Set
    End Property

    Private _hours As Long = 0
    Public Property hours() As Long
        Get
            Return _hours
        End Get
        Set(ByVal Value As Long)
            _hours = Value
        End Set
    End Property

    Private _days As Long = 0
    Public Property days() As Long
        Get
            Return _days
        End Get
        Set(ByVal Value As Long)
            _days = Value
        End Set
    End Property

    Private _months As Long = 0
    Public Property months() As Long
        Get
            Return _months
        End Get
        Set(ByVal Value As Long)
            _months = Value
        End Set
    End Property

    Private _years As Long = 0
    Public Property years() As Long
        Get
            Return _years
        End Get
        Set(ByVal Value As Long)
            _years = Value
        End Set
    End Property

    Public Function getPrettyTime() As String
        Dim nYears As Long
        Dim nMonths As Long
        Dim nDays As Long
        Dim nHours As Long
        Dim remainder As Long
        Dim result As String

        Dim tempMinutes As Long
        tempMinutes = totalMinutes
        remainder = tempMinutes Mod MINUTESINAYEAR
        nYears = CLng(Int(tempMinutes / MINUTESINAYEAR))
        tempMinutes = remainder

        remainder = tempMinutes Mod MINUTESINAMONTH
        nMonths = CLng(Int(tempMinutes / MINUTESINAMONTH))
        tempMinutes = remainder

        remainder = tempMinutes Mod MINUTESINADAY
        nDays = CLng(Int(tempMinutes / MINUTESINADAY))
        tempMinutes = remainder

        remainder = tempMinutes Mod MINUTESINANHOUR
        nHours = CLng(Int(tempMinutes / MINUTESINANHOUR))
        tempMinutes = remainder

        result = ""

        If nYears > 0 Then
            result = result & nYears & " yr(s) "
        End If

        If nMonths > 0 Then
            result = result & nMonths & " mth(s) "
        End If

        If nDays > 0 Then
            result = result & nDays & " day(s) "
        End If

        If nHours > 0 Then
            result = result & nHours & " hr(s) "
        End If

        If minutes > 0 Then
            result = result & tempMinutes & " min(s) "
        End If

        Return result
    End Function
End Class
