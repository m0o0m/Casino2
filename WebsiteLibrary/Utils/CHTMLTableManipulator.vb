Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DateAndTime

Public Class CHTMLTableManipulator
    Inherits HtmlTable

    Public Function addRowWithCells(ByVal ParamArray oCells() As HtmlTableCell) As HtmlTableRow
        Dim oResult As New HtmlTableRow()
        Dim oCell As HtmlTableCell

        For Each oCell In oCells
            oResult.Cells.Add(oCell)
        Next
        Me.Rows.Add(oResult)
        Return oResult
    End Function

    Public Function addRowWithCellsText(ByVal ParamArray oTexts() As String) As HtmlTableRow
        Dim oResult As New HtmlTableRow()
        Dim oText As String
        Dim oCell As HtmlTableCell
        For Each oText In oTexts
            oCell = New HtmlTableCell()
            oCell.InnerHtml = oText
            oResult.Cells.Add(oCell)
        Next
        Me.Rows.Add(oResult)
        Return oResult
    End Function
End Class
