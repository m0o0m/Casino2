Public Class CDataGridOperator
    Inherits CDataGridPageSort

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        MyBase.Render(writer)
        If Me.Items.Count = 0 And Me.Visible = True Then
            writer.Write("No data in table at this time.<br>")
        End If
    End Sub


    Private Sub CDataGridOperator_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'if no class was specified then use the standard look - we do check here so it doesn't auto add the html tags and make our code messy
        If Me.HeaderStyle.CssClass = "" Then
            Me.HeaderStyle.CssClass = "tableHead"
        End If
        If Me.AlternatingItemStyle.CssClass = "" Then
            Me.AlternatingItemStyle.CssClass = "tableItemAlt"
        End If
        If Me.ItemStyle.CssClass = "" Then
            Me.ItemStyle.CssClass = "tableItem"
        End If

    End Sub
End Class
