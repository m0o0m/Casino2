Imports System.Web.UI
Imports System.Drawing
Imports WebsiteLibrary.CSBCStd

Namespace UI

    Public Class CSBCReportControl
        Inherits CSBCUserControl

        Protected Const LAST_ROW As Integer = -1
        Protected Const LAST_COL As Integer = -1

        Private moReportTitle As String = String.Empty

        Protected Overridable Sub HtmlToPdf(ByVal poControl As Control, ByVal psFilename As String, ByVal poPageSize As iTextSharp.text.Rectangle)
            Dim swriter As New System.IO.StringWriter()
            Dim hwriter As New Html32TextWriter(swriter)

            hwriter.Write("<html><body>")
            poControl.RenderControl(hwriter)
            hwriter.Write("</body></html>")

            Dim oHtml As New System.IO.MemoryStream(System.Text.Encoding.Default.GetBytes(swriter.GetStringBuilder().ToString()))

            HtmlToPdf(oHtml, psFilename, poPageSize)
        End Sub

        Protected Overridable Sub HtmlToPdf(ByVal poHtml As System.IO.Stream, ByVal psFilename As String, ByVal poPageSize As iTextSharp.text.Rectangle)
            Response.ClearHeaders()
            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Type", "application/pdf")
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", psFilename))
            Response.Buffer = False
            Response.End()
        End Sub

        Protected Property ReportTitle() As String
            Get
                Return moReportTitle
            End Get
            Set(ByVal value As String)
                moReportTitle = value
            End Set
        End Property

     
        Protected Overridable Sub HtmlToCsv(ByRef poTarget As System.IO.Stream, ByVal poTable As System.Web.UI.WebControls.Table, Optional ByVal poColumnIgnores() As Integer = Nothing, Optional ByVal poRowIgnores() As Integer = Nothing)
            Dim oWriter As New System.IO.StreamWriter(poTarget)
            Dim sLine As String = ""
            Dim nRowIdx As Integer = 0
            Dim nColIdx As Integer = 0
            Dim oRowSpans As New Dictionary(Of Integer, Integer)
            Dim oRowSpansText As New Dictionary(Of Integer, String)

            If Not poRowIgnores Is Nothing Then
                For idx As Integer = 0 To poRowIgnores.Length - 1
                    Select Case poRowIgnores(idx)
                        Case LAST_ROW
                            poRowIgnores(idx) = poTable.Rows.Count - 1
                    End Select
                Next
            End If
            If Not poColumnIgnores Is Nothing Then
                For idx As Integer = 0 To poColumnIgnores.Length - 1
                    Select Case poColumnIgnores(idx)
                        Case LAST_COL
                            poColumnIgnores(idx) = poTable.Rows(0).Cells.Count
                    End Select
                Next
            End If

            For Each oRow As System.Web.UI.WebControls.TableRow In poTable.Rows
                nRowIdx += 1
                If Not poRowIgnores Is Nothing AndAlso Array.IndexOf(poRowIgnores, nRowIdx) >= 0 Then
                    Continue For
                End If

                sLine = ""
                nColIdx = 0

                For Each oCell As System.Web.UI.WebControls.TableCell In oRow.Cells
                    nColIdx += 1
                    While oRowSpans.ContainsKey(nColIdx)
                        If oRowSpans(nColIdx) > 0 Then
                            sLine &= String.Format(",""{0}""", oRowSpansText(nColIdx))
                            oRowSpans(nColIdx) -= 1
                            nColIdx += 1
                        Else
                            oRowSpans.Remove(nColIdx)
                            oRowSpansText.Remove(nColIdx)
                        End If
                    End While
                    If Not poColumnIgnores Is Nothing AndAlso Array.IndexOf(poColumnIgnores, nColIdx) >= 0 Then
                        Continue For
                    End If
                    If oCell.RowSpan > 1 Then
                        If Not oRowSpans.ContainsKey(nColIdx) Then
                            oRowSpans.Add(nColIdx, 0)
                            oRowSpansText.Add(nColIdx, "")
                        End If
                        oRowSpans(nColIdx) += oCell.RowSpan - 1
                        oRowSpansText(nColIdx) = InnerText(oCell)
                    End If
                    sLine &= String.Format(",""{0}""", InnerText(oCell))
                Next
                If sLine.Length > 0 Then
                    sLine = sLine.Substring(1)
                    oWriter.WriteLine(sLine)
                End If
            Next

            oWriter.Flush()
        End Sub

        Protected Overridable Sub HtmlToCsv(ByVal poTable As System.Web.UI.WebControls.Table, ByVal psFilename As String, Optional ByVal poColumnIgnores() As Integer = Nothing, Optional ByVal poRowIgnores() As Integer = Nothing)
            Response.ClearHeaders()
            Response.Clear()
            Response.ContentType = "application/csv"
            Response.AddHeader("Content-Type", "application/csv")
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", psFilename))
            Response.Buffer = False
            HtmlToCsv(Response.OutputStream, poTable, poColumnIgnores, poRowIgnores)
            Response.End()
        End Sub

        Protected Overridable Sub HtmlToCsv(ByVal poStream As System.IO.Stream, ByVal poTable As DataTable, ByVal psFilename As String)
            Response.ClearHeaders()
            Response.Clear()
            Response.ContentType = "application/csv"
            Response.AddHeader("Content-Type", "application/csv")
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", psFilename))
            Response.Buffer = False
            Dim oExport As New WebsiteLibrary.CCSVExporter()
            oExport.Export(poStream, poTable)
            Response.End()
        End Sub

        Private Function InnerText(ByVal poCell As System.Web.UI.WebControls.TableCell) As String
            Dim sText As String = poCell.Text

            sText = sText.Replace("&nbsp;", "")
            sText = sText.Replace("<strong>", "")
            sText = sText.Replace("</strong>", "")

            Return sText
        End Function

        Protected Overridable Function RefineTable(ByVal poSource As System.Web.UI.WebControls.Table, Optional ByVal poColumnIgnores() As Integer = Nothing, Optional ByVal poRowIgnores() As Integer = Nothing) As System.Web.UI.WebControls.Table
            Dim oTarget As New System.Web.UI.WebControls.Table
            Dim oRowTag As System.Web.UI.WebControls.TableRow = Nothing
            Dim oRowSrc As System.Web.UI.WebControls.TableRow = Nothing
            Dim oCelTag As System.Web.UI.WebControls.TableCell = Nothing
            Dim oCelSrc As System.Web.UI.WebControls.TableCell = Nothing
            Dim nRowIdx As Integer = 0
            Dim nColIdx As Integer = 0
            Dim oRowSpans As New Dictionary(Of Integer, Integer)
            Dim oRowSpansText As New Dictionary(Of Integer, String)

            If Not poRowIgnores Is Nothing Then
                For idx As Integer = 0 To poRowIgnores.Length - 1
                    Select Case poRowIgnores(idx)
                        Case LAST_ROW
                            poRowIgnores(idx) = poSource.Rows.Count - 1
                    End Select
                Next
            End If
            If Not poColumnIgnores Is Nothing Then
                For idx As Integer = 0 To poColumnIgnores.Length - 1
                    Select Case poColumnIgnores(idx)
                        Case LAST_COL
                            poColumnIgnores(idx) = poSource.Rows(0).Cells.Count
                    End Select
                Next
            End If

            oTarget.GridLines = WebControls.GridLines.Both
            For Each oRowSrc In poSource.Rows
                nRowIdx += 1
                If Not poRowIgnores Is Nothing AndAlso Array.IndexOf(poRowIgnores, nRowIdx) >= 0 Then
                    Continue For
                End If

                oRowTag = New System.Web.UI.WebControls.TableRow
                oTarget.Rows.Add(oRowTag)
                nColIdx = 0
                For Each oCelSrc In oRowSrc.Cells
                    nColIdx += 1
                    If Not poColumnIgnores Is Nothing AndAlso Array.IndexOf(poColumnIgnores, nColIdx) >= 0 Then
                        Continue For
                    End If
                    While oRowSpans.ContainsKey(nColIdx)
                        If oRowSpans(nColIdx) > 0 Then
                            oCelTag = New System.Web.UI.WebControls.TableCell
                            oRowTag.Cells.Add(oCelTag)
                            oCelTag.Text = oRowSpansText(nColIdx)
                            oRowSpans(nColIdx) -= 1
                            nColIdx += 1
                        Else
                            oRowSpans.Remove(nColIdx)
                            oRowSpansText.Remove(nColIdx)
                        End If
                    End While
                    If oCelSrc.RowSpan > 1 Then
                        If Not oRowSpans.ContainsKey(nColIdx) Then
                            oRowSpans.Add(nColIdx, 0)
                            oRowSpansText.Add(nColIdx, "")
                        End If
                        oRowSpans(nColIdx) += oCelSrc.RowSpan - 1
                        oRowSpansText(nColIdx) = oCelSrc.Text
                    End If

                    oCelTag = New System.Web.UI.WebControls.TableCell
                    oRowTag.Cells.Add(oCelTag)
                    oCelTag.Text = oCelSrc.Text
                Next
            Next

            Return oTarget
        End Function

        Protected Overridable Function CenterAlignTable(ByVal poTable As System.Web.UI.WebControls.Table) As System.Web.UI.WebControls.Table
            For Each oRow As System.Web.UI.WebControls.TableRow In poTable.Rows
                For Each oCell As System.Web.UI.WebControls.TableCell In oRow.Cells
                    oCell.HorizontalAlign = WebControls.HorizontalAlign.Center
                Next
            Next
            Return poTable
        End Function
    End Class

End Namespace
