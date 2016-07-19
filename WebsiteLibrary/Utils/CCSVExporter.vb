Imports WebsiteLibrary.CSBCStd
Imports System.Text
Imports System.Data

Public Class CCSVExporter

    Public UseQuotes As Boolean = True

    Public Sub Export(ByRef poStream As System.IO.Stream, ByVal poData As DataTable)

        'if poData is nothing then export ""
        If poData Is Nothing Then
            Return
        End If

        Dim oCSVExport As StringBuilder = New StringBuilder

        Dim nColumns As Integer = poData.Columns.Count

        Dim oWriter As New System.IO.StreamWriter(poStream)

        'add Title to CVS string
        For i As Integer = 0 To nColumns - 1
            AppendDataToCSV(oCSVExport, SafeString(poData.Columns(i).ColumnName))
            oCSVExport.Append(",")
        Next
        oCSVExport.Remove(oCSVExport.Length - 1, 1)
        oCSVExport.Append(vbCrLf)

        'add All Row in datatable to CSV file
        If poData.Rows.Count > 0 Then
            For i As Integer = 0 To poData.Rows.Count - 1
                For j As Integer = 0 To nColumns - 1
                    AppendDataToCSV(oCSVExport, SafeString(poData.Rows(i)(j)))
                    
                    oCSVExport.Append(",")
                Next
                oCSVExport.Remove(oCSVExport.Length - 1, 1)
                oCSVExport.Append(vbCrLf)

            Next
            oCSVExport.Remove(oCSVExport.Length - 1, 1)
            oWriter.Write(oCSVExport.ToString)
            oWriter.Flush()

        End If

    End Sub

    Private Sub AppendDataToCSV(ByRef poStringBuider As StringBuilder, ByVal psData As String)
        Dim sData As String = psData
        If sData.Contains(vbCrLf) Or sData.Contains(",") Then
            sData = sData.Replace(vbCrLf, " ").Replace(",", " ")
        End If
        If UseQuotes Then
            poStringBuider.Append("""")
            poStringBuider.Append(sData)
            poStringBuider.Append("""")
        Else
            poStringBuider.Append(sData)
        End If
    End Sub

End Class