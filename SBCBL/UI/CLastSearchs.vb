Imports SBCBL.std
Imports System.Collections.Generic

Namespace UI
    <Serializable()> _
    Public Class CLastSearchs

        Dim _LastSearchs As New List(Of CLastSearchParams)

        Public Function GetLastSearchParams(ByVal psPageUrl As String) As CLastSearchParams
            For index As Integer = 0 To _LastSearchs.Count - 1
                Dim oCLastSearchParams As CLastSearchParams = _LastSearchs(index)
                If oCLastSearchParams.PageURL = psPageUrl Then
                    Return oCLastSearchParams
                End If
            Next
            Return Nothing
        End Function

        Public Function GetLastSearchURL() As String
            If _LastSearchs.Count > 0 Then
                Return _LastSearchs(_LastSearchs.Count - 1).PageURL
            End If
            Return ""
        End Function

        Public Sub PushLastSearchParams(ByVal poCLastSearchParams As CLastSearchParams)
            For index As Integer = 0 To _LastSearchs.Count - 1
                Dim oCLastSearchParams As CLastSearchParams = _LastSearchs(index)
                If oCLastSearchParams.PageURL = poCLastSearchParams.PageURL Then
                    _LastSearchs.RemoveAt(index)
                    Exit For
                End If
            Next
            _LastSearchs.Add(poCLastSearchParams)
        End Sub

        Public Function GetSortKey(ByVal psPageUrl As String) As String
            Dim oCLastSearchParams As CLastSearchParams = GetLastSearchParams(psPageUrl)
            If oCLastSearchParams IsNot Nothing Then
                Return GetLastSearchParams(psPageUrl).SortKey
            End If
            Return ""
        End Function
    End Class
End Namespace

