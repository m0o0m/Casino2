Imports System.IO
Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Configuration
Imports System.ComponentModel
Imports websitelibrary.CSBCStd

Public Class CHelpLink
    Inherits WebControl

    Public Property ImageURL() As String
        Get
            If IsNothing(ViewState("ImageURL")) Then
                ViewState("ImageURL") = "/CC/Images/Helpico.gif"
            End If
            Return CStr(ViewState("ImageURL"))
        End Get
        Set(ByVal Value As String)
            ViewState("ImageURL") = Value
        End Set
    End Property

    Public Property HelpPageName() As String
        Get
            If IsNothing(ViewState("HelpPageName")) Then
                ViewState("HelpPageName") = ""
            End If
            Return CStr(ViewState("HelpPageName"))
        End Get
        Set(ByVal Value As String)
            ViewState("HelpPageName") = Value
        End Set
    End Property

    Public Property HelpToolTip() As String
        Get
            If IsNothing(ViewState("HelpToolTip")) Then
                ViewState("HelpToolTip") = "Help"
            End If
            Return CStr(ViewState("HelpToolTip"))
        End Get
        Set(ByVal Value As String)
            ViewState("HelpToolTip") = Value
        End Set
    End Property

    Public Property WidthPopup() As String
        Get
            If IsNothing(ViewState("WidthPopup")) Then
                ViewState("WidthPopup") = "800"
            End If
            Return CStr(ViewState("WidthPopup"))
        End Get
        Set(ByVal Value As String)
            ViewState("WidthPopup") = Value
        End Set
    End Property

    Public Property HeightPopup() As String
        Get
            If IsNothing(ViewState("HeightPopup")) Then
                ViewState("HeightPopup") = "600"
            End If
            Return CStr(ViewState("HeightPopup"))
        End Get
        Set(ByVal Value As String)
            ViewState("HeightPopup") = Value
        End Set
    End Property

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        If SafeString(Me.ImageURL).Trim.Length = 0 Then Return

        Dim oImg As New HtmlImage
        With oImg
            .ID = "img" + Me.UniqueID
            .Alt = Me.HelpToolTip
            .Src = Me.ImageURL
            .Style.Add("vertical-align", "middle")
            .Style.Add("cursor", "hand")
            If SafeString(Me.HelpPageName).Trim.Length > 0 Then
                Dim sSupportURL As String = System.Web.Configuration.WebConfigurationManager.AppSettings("SUPPORT_URL")
                .Attributes.Add("onClick", String.Format("helpPopup('{0}', {1}, {2});", sSupportURL + Me.HelpPageName, Me.WidthPopup, Me.HeightPopup))

            End If
        End With

        oImg.RenderControl(writer)
    End Sub

    Private Sub CHelpLink_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterScript()
    End Sub

    Private Sub RegisterScript()
        Dim sScript As New System.Text.StringBuilder()

        With sScript
            .AppendLine("<script type='text/javascript'>")
            .AppendLine("function helpPopup(sURL, width, height)")
            .AppendLine("{")
            .AppendLine("   var centerWidth = window.screen.width / 2;")
            .AppendLine("   var centerHeight = window.screen.height / 2;")
            .Append("       window.open(sURL,'Help','status=0,toolbar=0,location=0,menubar=0,resizable=1")
            .AppendLine("       ,width='+ width + ',height=' + height +',scrollbars=0');")
            .AppendLine("}")
            .AppendLine("</script>")
        End With

        If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "HELP_POPUP") Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "HELP_POPUP", sScript.ToString())
        End If
    End Sub

End Class