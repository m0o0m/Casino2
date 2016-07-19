Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

<System.ComponentModel.Designer(GetType(HtmlControl))> _
Public Class CPopupBox
    Inherits HtmlContainerControl

#Region "Property"
    Public Property Display() As Boolean
        Get
            If ViewState("_Display") IsNot Nothing Then
                Return CBool(ViewState("_Display"))
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("_Display") = value
        End Set
    End Property

    Public Property Width() As String
        Get
            If ViewState("_Width") IsNot Nothing Then
                Return CType(ViewState("_Width"), String)
            Else
                Return "600"
            End If
        End Get
        Set(ByVal value As String)
            ViewState("_Width") = value
        End Set
    End Property

    Public Property Height() As String
        Get
            If ViewState("_Height") IsNot Nothing Then
                Return CType(ViewState("_Height"), String)
            Else
                Return "200"
            End If
        End Get
        Set(ByVal value As String)
            ViewState("_Height") = value
        End Set
    End Property

    Public Property Top() As String
        Get
            If ViewState("_Top") IsNot Nothing Then
                Return CType(ViewState("_Top"), String)
            Else
                Return "150"
            End If
        End Get
        Set(ByVal value As String)
            ViewState("_Top") = value
        End Set
    End Property

    Public Property Left() As String
        Get
            If ViewState("_Left") IsNot Nothing Then
                Return CType(ViewState("_Left"), String)
            Else
                Return "300"
            End If
        End Get
        Set(ByVal value As String)
            ViewState("_Left") = value
        End Set
    End Property
#End Region

    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        Dim sDisplay As String = IIf(Display, "block", "none").ToString()        
        Dim sID As String = Me.ClientID
        Dim sSubWidth As String = (CType(Width, Integer) - 46).ToString()

        writer.Write("<div id=""" & sID & """ style=""display:" & sDisplay & """>")
        writer.Write("<table border=""0"" class=""SPopupMainTable"" cellpadding=""0"" cellspacing=""0"" style=""width:" & Width & "px; top:" & Top & "px; left:" & Left & "px"">")

        '' Header
        writer.Write("  <tr><td class=""SPopupTopLeft"" align=""Right""></td>")            
        writer.Write("      <td class=""SPopupTopMiddle"" width=""" & sSubWidth & "px""></td>")
        writer.Write("      <td class=""SPopupTopRight"">")
        writer.Write("          <a href=""#"" onclick=""document.getElementById('" & sID & "').style.display='none';"">")
        writer.Write("              <img src=""/SBS/Images/close.jpg"" width=""23px"" height=""20px"" border=""0"" />")
        writer.Write("          </a>")
        writer.Write("      </td></tr>")

        '' Content
        writer.Write("  <tr><td class=""SPopupMiddleLeft"" style=""height:" & Height & "px""></td>")
        writer.Write("      <td style=""width:" & sSubWidth & "; height:" & Height & """>")
        writer.Write("          <table style=""width:" & sSubWidth & "px; height:" & Height & "px; background-color:#FFFFFF""><tr><td valign=""Top"">")
        MyBase.Render(writer)
        writer.Write("          </td></tr></table>")
        writer.Write("      </td>")
        writer.Write("      <td class=""SPopupMiddleRight"" style=""height:" & Height & "px""></td></tr>")

        '' Bottom
        writer.Write("  <tr><td class=""SPopupBottomLeft""></td>")
        writer.Write("      <td class=""SPopupBottomMiddle"" width=""" & sSubWidth & "px""></td>")
        writer.Write("      <td class=""SPopupBottomRight""></td></tr>")

        writer.Write("</table></div>")
    End Sub

End Class
