Public Class CSocialSecurityNumber
    Public Shared Function isValidSSN(ByVal sSSN As String) As Boolean
        If IsNothing(sSSN) Then
            Return False
        End If

        sSSN = sSSN.Replace("-", "")
        If sSSN.Length <> 9 Then
            Return False
        End If

        Dim sFirst3 As String = ""
        Dim sSecond2 As String = ""
        Dim sLast4 As String = ""

        parseSSNEX(sSSN, sFirst3, sSecond2, sLast4)
        If IsNumeric(sFirst3) = False Then
            Return False
        ElseIf IsNumeric(sSecond2) = False Then
            Return False
        ElseIf IsNumeric(sLast4) = False Then
            Return False
        End If

        Return True
    End Function

    Public Sub parseSSN(ByVal ssn As String)
        parseSSNEX(ssn, First3, Second2, Last4)
    End Sub

    Public ReadOnly Property FullSSN() As String
        Get
            Return First3 & SSNSeperator & Second2 & SSNSeperator & Last4
        End Get
    End Property

    Public SSNSeperator As String = "-"

    Public First3 As String
    Public Second2 As String
    Public Last4 As String

    Private Shared Sub parseSSNEX(ByVal ssn As String, ByRef first3 As String, ByRef second2 As String, ByRef last4 As String)
        If IsNothing(ssn) Then
            ssn = ""
        End If

        ssn = ssn.Replace("-", "").PadRight(9, " "c)
        first3 = ssn.Substring(0, 3).Trim
        second2 = ssn.Substring(3, 2).Trim
        last4 = ssn.Substring(5, 4).Trim
    End Sub
End Class
