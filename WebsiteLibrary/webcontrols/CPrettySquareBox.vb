Imports System.IO
Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Configuration
Imports System.ComponentModel
Imports WebsiteLibrary.CSBCStd
Imports System.Web.UI.Design
Imports System.Web.UI.Design.WebControls

<Designer(GetType(ContainerControlDesigner))> _
<ParseChildren(False)> _
Public Class CPrettySquareBox
    Inherits HtmlContainerControl

    Public Property Title() As String
        Get
            If IsNothing(ViewState("_Title")) Then
                Return ""
            End If
            Return CStr(ViewState("_Title"))
        End Get
        Set(ByVal Value As String)
            ViewState("_Title") = Value.Replace("'", "&#39;").Replace("""", "&#34;")
        End Set
    End Property
    'this type converted doesn't seem to b working @ the moment 

    Public Property Width() As String
        Get
            If IsNothing(ViewState("Width")) Then
                Return ""
            End If
            Return CStr(ViewState("Width"))
        End Get
        Set(ByVal Value As String)
            ViewState("Width") = Value
        End Set
    End Property

    Public Shared Property Skin() As String
        Get
            Return SafeString(System.Web.HttpContext.Current.Session("_Skin"))
        End Get
        Set(ByVal value As String)
            System.Web.HttpContext.Current.Session("_Skin") = value
        End Set
    End Property

    Public Property CellPadding() As String
        Get
            If IsNothing(ViewState("CPSB_CELLPADDING")) Then
                Return ""
            End If
            Return CStr(ViewState("CPSB_CELLPADDING"))
        End Get
        Set(ByVal Value As String)
            ViewState("CPSB_CELLPADDING") = Value
        End Set
    End Property

    Public ReadOnly Property HiddenExpandedValue() As Boolean
        Get
            If Not Page.Request.Params.Get("Expanded" & Me.ClientID) Is Nothing Then
                Return CBool(Page.Request.Params.Get("Expanded" & Me.ClientID))
            End If
            Return False
        End Get
    End Property

    Public Property Expanded() As Boolean
        Get
            If IsNothing(ViewState("Expanded")) Then
                ViewState("Expanded") = True
            End If
            Return CBool(ViewState("Expanded"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("Expanded") = Value
        End Set
    End Property

    Public Property Collapsible() As Boolean
        Get
            If IsNothing(ViewState("Collapsible")) Then
                ViewState("Collapsible") = False
            End If
            Return CBool(ViewState("Collapsible"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("Collapsible") = Value
        End Set
    End Property

    Public Property AllowDoubleClick() As Boolean
        Get
            If IsNothing(ViewState("AllowDoubleClick")) Then
                ViewState("AllowDoubleClick") = False
            End If
            Return CBool(ViewState("AllowDoubleClick"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("AllowDoubleClick") = Value
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

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Dim bExpand As Boolean = HiddenExpandedValue

        If Not Page.IsPostBack Then
            bExpand = Expanded
        End If

        Dim sID As String = Me.ClientID

        writer.Write("<div class='RadGrid RadGrid_" & Skin & "'")
        If Me.Width <> "" Then
            writer.Write(" style=""width: " & Me.Width & ";"" >")
        Else
            writer.Write(">")
        End If

        writer.WriteLine("<div class=""rgMasterTable"">" & _
    "<div class=""rgHeader"">" & _
    IIf(Collapsible, _
        "<div title='Click me' id='header" & sID & "' style=""CURSOlogmeR: pointer;"" onclick=""updateIcon(document.getElementById('img" & sID & "'),document.getElementById('Expanded" & sID & "'), document.getElementById('header" & sID & "'), '" & Title & "', 'tog" & sID & "');"">" & _
        "<img style=""vertical-align: middle; top-margin: 1px;"" id=""img" & sID & """ src=""/CC/Images/" & IIf(bExpand, "minus.gif", "plus.gif").ToString & """ height=""11""> &nbsp; " & _
        "<input id=""Expanded" & sID & """ name=""Expanded" & sID & """ type=""hidden"" value=""" & bExpand & """/>", "").ToString() & _
        Me.Title & _
    IIf(Collapsible, "</div>", "").ToString() & _
    "</div>" & _
    "<div id=""tog" & sID & """ " & IIf(Not bExpand AndAlso Collapsible, " style=""display: none;"" ", "").ToString() & ">")
        'content
        MyBase.Render(writer)

        writer.WriteLine("</div></div></div>")

        If Collapsible Then
            ScriptManager.RegisterClientScriptBlock(Me.Page, GetType(CPrettySquareBox), "SS3", "setTooltip(document.getElementById('img" & Me.ClientID & "'),document.getElementById('header" & Me.ClientID & "'),'" & Title & "');", True)
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "S4", "setTooltip(document.getElementById('img" & Me.ClientID & "'),document.getElementById('header" & Me.ClientID & "'),'" & Title & "');", True)
        End If

    End Sub


    Private Sub CPrettySquareBox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ''Global JS
        If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "REGISTER_IMGSWAP") Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "REGISTER_IMGSWAP", _
                    "<script type=""text/javascript"">" & vbCrLf & _
                    "   function updateIcon(img, hiddenField, header, sTitle, sContentID) {" & vbCrLf & _
                    "       img.src = '/CC/images/' + ( img.src.match('minus.gif') ? 'plus.gif' : 'minus.gif' );" & vbCrLf & _
                    "       hiddenField.value=( img.src.match('minus.gif') ? 'True' : 'False' );" & vbCrLf & _
                    "       document.getElementById(sContentID).style.display = (hiddenField.value.match('True') ? 'block' : 'none' );" & vbCrLf & _
                       " setTooltip(img,header,sTitle) ;" & vbCrLf & _
                    "   }" & vbCrLf & _
                      " function setTooltip(img,header, sTitle) { " & _
                    " if (img == null) return;" & _
                    " var sTogStatus =( img.src.match('minus.gif')? 'collapse' :  'expand'   ); " & _
                    " header.title = 'Click to ' + sTogStatus + ' ' +  sTitle ;  }</script>")
        End If

    End Sub


End Class