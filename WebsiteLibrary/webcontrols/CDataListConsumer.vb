Public Class CDataListConsumer
    Inherits CDataGridPageSort

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
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

        MyBase.Render(writer)
    End Sub
End Class
