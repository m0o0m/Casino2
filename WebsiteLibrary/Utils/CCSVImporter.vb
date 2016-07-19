Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports Microsoft.VisualBasic
Imports WebsiteLibrary.CSBCStd

Public Class CCSVImporter

    Public Shared Function Import(ByVal psData As String, Optional ByVal pnPreviewRows As Integer = -1) As DataTable
        Dim oDT As New DataTable()
        Dim sRegEx As String = "(?<comma>(,){1})|(?<quotes>""(?<innerquote>.*?)"")|(?<normal>[^,""]*)"
        Dim oReg As New System.Text.RegularExpressions.Regex(sRegEx)

        Dim sData As String = psData.Replace("\r", "")

        ' Make sure this work for all CSV version
        Dim breakLine As String = vbCr

        If sData.Contains(vbCrLf) Then
            breakLine = vbCrLf
        ElseIf sData.Contains(vbLf) Then
            breakLine = vbLf
        End If

        If sData.EndsWith(breakLine) Then
            sData = sData.Remove(sData.Length - 1)
        End If

        'for every line in the csv
        Dim nRow As Integer = 0
        For Each sRow As String In Split(sData, breakLine)

            'Just only parse Preview Rows
            If pnPreviewRows > 0 AndAlso nRow >= pnPreviewRows Then Exit For

            'table header with column names
            If (nRow = 0) Then

                Dim nCol As Integer = 0
                For Each oMatch As System.Text.RegularExpressions.Match In oReg.Matches(sRow)

                    If (oMatch.Groups("comma").Value = ",") Then
                        'we found a comma so check for the scenario of 
                        'missing column title, so ,, appears next to each other

                        nCol += 1
                        If oDT.Columns.Count < nCol Then
                            oDT.Columns.Add("Column" & nCol)
                        End If

                    ElseIf (oMatch.Groups("quotes").Value <> "") Then
                        oDT.Columns.Add(SafeString(oMatch.Groups("innerquote").Value))

                    ElseIf (oMatch.Groups("normal").Value <> "") Then
                        oDT.Columns.Add(SafeString(oMatch.Groups("normal").Value))

                    End If
                Next
            Else
                Dim oDR As DataRow = oDT.NewRow()
                oDT.Rows.Add(oDR)

                'populate the cells in this row
                Dim nCol As Integer = 0

                For Each oMatch As System.Text.RegularExpressions.Match In oReg.Matches(sRow)
                    If (oMatch.Groups("comma").Value = ",") Then
                        nCol += 1 'we found a comma so increment the column index

                    ElseIf (oMatch.Groups("quotes").Value <> "" And nCol < oDT.Columns.Count) Then
                        oDR(nCol) = oMatch.Groups("innerquote").Value

                    ElseIf (oMatch.Groups("normal").Value <> "" And nCol < oDT.Columns.Count) Then
                        oDR(nCol) = oMatch.Groups("normal").Value
                    End If
                Next
            End If

            nRow += 1
        Next

        Return oDT
    End Function

End Class
