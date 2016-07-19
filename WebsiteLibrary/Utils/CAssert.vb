Public Class CAssert

    Public Shared Sub Assert(ByVal pbCondition As Boolean, ByVal psMessage As String)
        If Not pbCondition Then
            Throw New CAssertException(psMessage)
        End If
    End Sub

End Class

Public Class CAssertException
    Inherits System.Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class