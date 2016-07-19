Imports system.Web.UI
Imports System.Web.UI.WebControls


Public Class CTableToggler
    Inherits System.Web.UI.WebControls.CompositeDataBoundControl


    Protected _HeaderTemplate As ITemplate

    <PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property HeaderTemplate() As ITemplate
        Get
            Return _HeaderTemplate
        End Get
        Set(ByVal value As ITemplate)
            _HeaderTemplate = value
        End Set
    End Property

    Protected _ContentTemplate As ITemplate
    <PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property ContentTemplate() As ITemplate
        Get
            Return _ContentTemplate
        End Get
        Set(ByVal value As ITemplate)
            _ContentTemplate = value
        End Set
    End Property

    'Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
    '    Me.Controls.Add(New LiteralControl("<TABLE cellSpacing=0 cellPadding=0 width=100% border=0><TR><TD class=""GreyBoxHeader""><div onclick=""Effect.toggle(document.all." & Me.ClientID & "tog, 'slide'); updateIcon(document.all." & Me.ClientID & "img);"">"))

    '    If _HeaderTemplate IsNot Nothing Then
    '        Dim oItem As New TemplateItem()
    '        _HeaderTemplate.InstantiateIn(oItem)
    '        oItem.DataBind()
    '        Controls.Add(oItem)
    '    End If
    '    Me.Controls.Add(New LiteralControl("<img id=""" & Me.ClientID & "img"" src=""/CC/Images/navigate_down.gif"" height=""11""></TD></TR>"))


    '    Me.Controls.Add(New LiteralControl("<TR><TD class=""greyBoxcontent""><DIV id=" & Me.ClientID & "tog>"))
    '    If _ContentTemplate IsNot Nothing Then
    '        Dim oItem As New TemplateItem()
    '        _ContentTemplate.InstantiateIn(oItem)
    '        oItem.DataBind()
    '        Controls.Add(oItem)
    '    End If
    '    Me.Controls.Add(New LiteralControl("</DIV</TD></TR></TABLE>"))


    '    MyBase.Render(writer)
    'End Sub

    Private Sub CTableToggler_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "REGISTER_SCRIPTACULOUS") Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "REGISTER_SCRIPTACULOUS", "<script src=""/CC/inc/scripts/prototype.js"" type=""text/javascript""></script><script src=""/CC/inc/scripts/scriptaculous.js?load=effects"" type=""text/javascript""></script>")
        End If

        If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "REGISTER_IMGSWAP") Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "REGISTER_IMGSWAP", _
                    "<script type=""text/javascript"">" & vbCrLf & _
                    "   function updateIcon(img) {" & vbCrLf & _
                    "       img.src = '/CC/images/' + ( img.src.match('navigate_down.gif') ? 'navigate_up.gif' : 'navigate_down.gif' );" & vbCrLf & _
                    "   }" & vbCrLf & _
                    "</script>")
        End If
    End Sub

    Protected Overloads Overrides Function CreateChildControls(ByVal dataSource As System.Collections.IEnumerable, ByVal dataBinding As Boolean) As Integer
        Dim oPanel As New Panel()

        Controls.Clear()
        Controls.Add(oPanel)

        oPanel.Controls.Add(New LiteralControl("<TABLE cellSpacing=0 cellPadding=0 width=100% border=0><TR><TD class=""GreyBoxHeader""><div onclick=""Effect.toggle(document.all." & Me.ClientID & "tog, 'slide'); updateIcon(document.all." & Me.ClientID & "img);"">"))

        If _HeaderTemplate IsNot Nothing Then
            _HeaderTemplate.InstantiateIn(oPanel)
        End If
        oPanel.Controls.Add(New LiteralControl("<img id=""" & Me.ClientID & "img"" src=""/CC/Images/navigate_down.gif"" height=""11""></TD></TR>"))


        oPanel.Controls.Add(New LiteralControl("<TR><TD class=""greyBoxcontent""><DIV id=" & Me.ClientID & "tog>"))
        If _ContentTemplate IsNot Nothing Then
            _ContentTemplate.InstantiateIn(oPanel)
        End If

        oPanel.Controls.Add(New LiteralControl("</DIV</TD></TR></TABLE>"))
        oPanel.DataBind()

        Return 1
    End Function

End Class