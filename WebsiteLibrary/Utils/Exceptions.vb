'class is for general errors that may be triggered by missing/corrupt url parameters
Public Class CURLParamException
	Inherits System.Exception

	Sub New(ByVal sParamName As String)
        MyBase.New(sParamName)
	End Sub

End Class