'class to indicate whether a process saved an image and if so, what was the filename
Public Class CFileSaveResult
    'factory methods to help w/ consrtruction 

    'constructor to indicate image was saved or not but no error occured -- 
    'ie: user may have submitted form w/o specifying a  non-required imag
    Public Shared Function createSuccessExecute(ByVal sFileName As String) As CFileSaveResult
        Dim result As New CFileSaveResult()
        result.FileName = sFileName
        Return result
    End Function

    Public Shared Function createFailureSave(ByVal sErrorMessage As String) As CFileSaveResult
        Dim result As New CFileSaveResult()
        result.ErrorMessage = sErrorMessage
        Return result
    End Function

    Public Shared Function isValidImageExtension(ByVal sFilename As String) As Boolean
        Dim result As Boolean = False
        Dim fp As New CFilePathParser(sFilename)
        Dim i As Integer
        For i = 0 To ImageExtensions.Length - 1
            If ImageExtensions(i) = fp.Extension.ToUpper Then
                result = True
            End If
        Next

        Return result
    End Function

    Public Shared ImageExtensions() As String = {"GIF", "JPG", "JPEG", "PNG"}

    Private Sub New()
    End Sub

    Public ReadOnly Property WasSaved() As Boolean
        Get
            Return FileName <> ""
        End Get
    End Property

    Public ReadOnly Property hasError() As Boolean
        Get
            Return ErrorMessage <> ""
        End Get
    End Property

    Public ErrorMessage As String = ""
    Public FileName As String = ""
End Class