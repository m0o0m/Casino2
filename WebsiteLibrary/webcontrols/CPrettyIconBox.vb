Imports System.Web.UI.HtmlControls
Imports System.IO
Imports System.ComponentModel
Imports System.Web.UI.Design
Imports System.Web.UI.Design.WebControls
Imports System.Web.UI

<Designer(GetType(ContainerControlDesigner))> _
<ParseChildren(False)> _
Public Class CPrettyIconBox
    Inherits HtmlContainerControl

    Public Property Title() As String
        Get
            If ViewState("Title") Is Nothing Then
                Return ""
            End If
            Return ViewState("Title").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("Title") = value
        End Set
    End Property

    Public Property TitleStyle() As String
        Get
            If ViewState("TitleStyle") Is Nothing Then
                Return ""
            End If
            Return ViewState("TitleStyle").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("TitleStyle") = value
        End Set
    End Property

    Public Property TitleCssClass() As String
        Get
            If ViewState("TitleCssClass") Is Nothing Then
                Return "IconBoxTitle"
            End If
            Return ViewState("TitleCssClass").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("TitleCssClass") = value
        End Set
    End Property

    Public Property TitleToolTip() As String
        Get
            If ViewState("TitleToolTip") Is Nothing Then
                Return ""
            End If
            Return ViewState("TitleToolTip").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("TitleToolTip") = value
        End Set
    End Property

    Public Property Icon() As String
        Get
            If ViewState("Icon") Is Nothing Then
                Return ""
            End If
            Return ViewState("Icon").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("Icon") = value
        End Set
    End Property

    Public Property IconWidth() As String
        Get
            If ViewState("IconWidth") Is Nothing Then
                Return ""
            End If
            Return "width='" & ViewState("IconWidth").ToString() & "'"
        End Get
        Set(ByVal value As String)
            ViewState("IconWidth") = value
        End Set
    End Property

    Public Property IconHeight() As String
        Get
            If ViewState("IconHeight") Is Nothing Then
                Return ""
            End If
            Return "height='" & ViewState("IconHeight").ToString() & "'"
        End Get
        Set(ByVal value As String)
            ViewState("IconHeight") = value
        End Set
    End Property

    Public Property ShowIcon() As Boolean
        Get
            If ViewState("ShowIcon") Is Nothing Then
                Return True
            End If
            Return UCase(ViewState("ShowIcon").ToString()) = "TRUE"
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowIcon") = value
        End Set
    End Property

    Public Property IconTooltip() As String
        Get
            If ViewState("IconTooltip") Is Nothing Then
                Return ""
            End If
            Return ViewState("IconTooltip").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("IconTooltip") = value
        End Set
    End Property

    Public Property Link() As String
        Get
            If ViewState("Link") Is Nothing Then
                Return ""
            End If
            Return ViewState("Link").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("Link") = value
        End Set
    End Property

    Public Property HasShadow() As Boolean
        Get
            If ViewState("HASSHADOW") Is Nothing Then
                ViewState("HASSHADOW") = True
            End If
            Return CBool(ViewState("HASSHADOW"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("HASSHADOW") = value
        End Set
    End Property

    Public Property ShowBorder() As Boolean
        Get
            If ViewState("ShowBorder") Is Nothing Then
                Return True
            End If
            Return CBool(ViewState("ShowBorder"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowBorder") = value
        End Set
    End Property

    Protected Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim sAttr As String = ""
        Dim sCssClass As String = IIf(ShowBorder, "IconBox", "").ToString()

        For Each sKey As String In Attributes.Keys
            If Array.IndexOf(New String() {"Icon", "Link", "Title", "TitleStyle", "TitleCssClass"}, sKey) >= 0 Then
                Continue For
            End If
            If sKey = "class" Then
                sCssClass = Attributes.Item(sKey)
                Continue For
            End If
            sAttr &= String.Format(" {0}='{1}'", sKey, Attributes.Item(sKey))
        Next

        If HasShadow Then
            writer.Write("<div class='wrap1'><div class='wrap2'><div class='wrap3'>")
        End If

        writer.Write(String.Format("<table width='100%' cellpadding='5' id='{0}' class='{1}' {2}>", Me.ClientID, sCssClass, sAttr))
        If Link <> "" Then
            If ShowIcon Then
                writer.Write(String.Format("<tr><td><a href='{0}'><img border='0' src='{1}' alt='{2}' {3} {4}></a></td>", Link, Icon, IconTooltip, IconWidth, IconHeight))
            Else
                writer.Write("<tr><td></td>")
            End If
            writer.Write(String.Format("<td width='100%'><a href='{0}' style='{1}' class='{2}' title='{4}'><span>{3}</span></a><hr size='1' width='100%'/></td></tr><tr><td colspan='2'>", Link, TitleStyle, TitleCssClass, Title, TitleToolTip))
        Else
            If ShowIcon Then
                writer.Write(String.Format("<tr><td><img border='0' src='{0}' alt='{1}' {2} {3}></td>", Icon, IconTooltip, IconWidth, IconHeight))
            Else
                writer.Write("<tr><td></td>")
            End If
            writer.Write(String.Format("<td width='100%'><span  style='{0}' class='{1}'>{2}</span><hr size='1' width='100%'/></td></tr><tr><td colspan='2'>", TitleStyle, TitleCssClass, Title))
        End If

    End Sub

    Protected Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        writer.Write("</td></tr></table>")

        If HasShadow Then
            writer.Write("</div></div></div>")
        End If
    End Sub

End Class
