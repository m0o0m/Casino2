Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports WebsiteLibrary.CSBCStd

'this page inherits the UI.Page and adds all the std functions that are normally used
Public Class CGlobalPage
    Inherits System.Web.UI.Page

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private _HelpPageName As String = ""
    Public Property HelpPageName() As String
        Get
            Return _HelpPageName
        End Get
        Set(ByVal value As String)
            _HelpPageName = value
        End Set
    End Property
    Private _PageTitle As String = ""
    Public Property PageTitle() As String
        Get
            Return _PageTitle
        End Get
        Set(ByVal value As String)
            _PageTitle = value
        End Set
    End Property

    Private _TopMenu As HtmlAnchor = Nothing
    Public Property TopMenu() As HtmlAnchor
        Get
            Return _TopMenu
        End Get
        Set(ByVal value As HtmlAnchor)
            _TopMenu = value
        End Set
    End Property

    Private _BottomMenu As HtmlAnchor = Nothing
    Public Property BottomMenu() As HtmlAnchor
        Get
            Return _BottomMenu
        End Get
        Set(ByVal value As HtmlAnchor)
            _BottomMenu = value
        End Set
    End Property

    Private _PageTitleURL As String = "#"
    Public Property PageTitleURL() As String
        Get
            Return _PageTitleURL
        End Get
        Set(ByVal value As String)
            _PageTitleURL = value
        End Set
    End Property
    Private _PageSubTitle As String = ""
    Public Property PageSubTitle() As String
        Get
            Return _PageSubTitle
        End Get
        Set(ByVal value As String)
            _PageSubTitle = value
        End Set
    End Property

    Private _MenuTabName As String = ""
    Public Property MenuTabName() As String
        Get
            Return _MenuTabName
        End Get
        Set(ByVal value As String)
            _MenuTabName = value
        End Set
    End Property

    Private _SubMenuActive As String = ""
    Public Property SubMenuActive() As String
        Get
            Return _SubMenuActive
        End Get
        Set(ByVal value As String)
            _SubMenuActive = value
        End Set
    End Property

    Private _SideMenuTabName As String = ""
    Public Property SideMenuTabName() As String
        Get
            Return _SideMenuTabName
        End Get
        Set(ByVal value As String)
            _SideMenuTabName = value
        End Set
    End Property

    Private _PageImageURL As String = ""
    Public Property PageImageURL() As String
        Get
            Return _PageImageURL
        End Get
        Set(ByVal value As String)
            _PageImageURL = value
        End Set
    End Property

    Private _CurrentPageName As String = ""
    Public Property CurrentPageName() As String
        Get
            Return _CurrentPageName
        End Get
        Set(ByVal value As String)
            _CurrentPageName = value
        End Set
    End Property

    Public ReadOnly Property PopupCallBackData() As String
        Get
            Return Server.UrlDecode(SafeString(Request("__EVENTARGUMENT")))
        End Get
    End Property

    <System.ComponentModel.BrowsableAttribute(False), System.ComponentModel.DesignerSerializationVisibilityAttribute(0)> _
    Public Shared ReadOnly Property ImageDir() As String
        Get
            Return "/images/"
        End Get
    End Property

    Public Sub SetWindowStatus(ByVal psStatus As String, Optional ByVal pbIsAjaxPostback As Boolean = False)
        If pbIsAjaxPostback Then
            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.GetType(), "WINDOW_STATUS", "<script>window.status='" & psStatus & "';</script>", False)
        Else
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "WINDOW_STATUS", "<script>window.status='" & psStatus & "';</script>")
        End If
    End Sub

    'assuming that this page is a popup, will close the window and optionaly cause parent to refresh
    Public Overloads Sub closeMe(Optional ByVal refreshParent As Boolean = True)
        If refreshParent Then
            Response.Redirect("~/inc/closeSelf.aspx?resubmit_opener=true")
        Else
            Response.Redirect("~/inc/closeSelf.aspx")
        End If
    End Sub

    Public Overloads Sub closeMe(ByVal RedirectParentURL As String)
        Response.Redirect("~/inc/closeSelf.aspx?url=" & Server.UrlEncode(RedirectParentURL))
    End Sub

    Public Overloads Sub closeMe(ByVal callBackData As CParentCallBackData)
        If callBackData.IsPostBackClient = True Then
            Response.Redirect("~/inc/closeSelf.aspx?CBClient=Y&CBData=" & Server.UrlEncode(callBackData.Data))
        Else
            Response.Redirect("~/inc/closeSelf.aspx?CB=Y&CBData=" & Server.UrlEncode(callBackData.Data))
        End If
    End Sub

    Public Overloads Sub closeMeCallBackButtonClick(ByVal callBackData As CParentCallBackData, ByVal buttonClientID As String)
        Dim sUrl As String = "~/inc/closeSelf.aspx?{0}=Y&CBData={1}&ButtonClientID={2}"

        sUrl = String.Format(sUrl, IIf(callBackData.IsPostBackClient, "CBClient", "CB"), Server.UrlEncode(callBackData.Data), buttonClientID)

        Response.Redirect(sUrl)
    End Sub

    Public Sub DownloadFile(ByVal psFileName As String)
        Response.ContentType = "application/octet-stream"
        Response.AddHeader("Content-Disposition", "attachment; filename=""" & psFileName & """")
        Response.End()
    End Sub

    Public Sub DownloadPDF(ByVal psFileName As String)
        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Type", "application/pdf")
        Response.AddHeader("Content-Disposition", "attachment;  filename=""" & psFileName & """")
        Response.End()
    End Sub

    Public Sub refreshPage(ByVal nWaitMilliseconds As Long)
        Dim script As String = String.Format("setTimeout('document.forms[0].submit();', {0});", nWaitMilliseconds)
        script = "<script>" & script & "</script>"
        Dim SCRIPT_KEY As String = "REFRESH_PAGE"
        If Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), SCRIPT_KEY) Then
            Exit Sub
        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_KEY, script)
    End Sub

    Public Overloads Sub setStartupPosition(ByVal sStartupAnchor As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "STARTUP_SCROLL", String.Format("<script>location.href='#{0}';</script>", sStartupAnchor))
    End Sub

    Public Overloads Sub setStartupPosition(ByVal oStartupAnchor As HtmlAnchor)
        setStartupPosition(oStartupAnchor.Name)
    End Sub

    Public Sub setLoadUpFocus(ByVal mFocusCtrl As Control)
        Dim thePage As Page = CType(HttpContext.Current.Handler, Page)
        Dim focusCtrlID As String
        focusCtrlID = mFocusCtrl.ClientID
        If thePage.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), Format("onFocus_{0}", focusCtrlID)) Then
            Exit Sub
        End If
        thePage.ClientScript.RegisterClientScriptBlock(Me.GetType(), String.Format("onFocus_{0}", focusCtrlID), String.Format("<script>document.all.{0}.focus();</script>", focusCtrlID))
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
        If SafeString(Request("JUSTTOUCHING")) = "Y" Then
            If Request("URL").IndexOf("login.aspx") = -1 Then
                Response.End()
            End If
        End If

        If (Not Context.Session Is Nothing) Then
            If (Session.IsNewSession) Then
                Dim sCookieHeader As String = Request.Headers("Cookie")
                If ((Not sCookieHeader Is Nothing) AndAlso (sCookieHeader.IndexOf("ASP.NET_SessionId") >= 0)) Then
                    Response.Redirect("/login.aspx")
                End If
            End If
        End If

        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
    End Sub

    'util function to help trace down which control caused error
    Public Sub traceErrorControls()
        Dim ctl1 As IValidator
        For Each ctl1 In Me.Validators
            If TypeOf ctl1 Is BaseValidator Then
                Dim ctl As BaseValidator
                ctl = CType(ctl1, BaseValidator)
                If ctl.ControlToValidate <> "" Then
                    log.Debug(ctl.ID & " failed to validate " & ctl.ControlToValidate)
                End If
            End If
        Next
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        'we use overloaded because it's more predictable than events :-T

        If SafeString(Request("autoSize")) = "Y" Then
            Dim s As String
            Const ScriptBlockID As String = "/inc/CommonControls/CPopupWidget.js"
            If Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), ScriptBlockID) = False Then
                'inject required scripts 
                s = "<script src=""/inc/CommonControls/CPopupWidget.js""></script>"
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), ScriptBlockID, s)
            End If

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "AUTO_RESIZE_WIN", "<script>autoResizeWindow();</script>")
        End If

        If Request("__EVENTTARGET") = "__POPUP_CB" Then
            checkParentPagePostback(Me)
        End If

        Page.Title = PageTitle
        If TopMenu IsNot Nothing Then
            TopMenu.Style.Add("color", "Cyan")
        End If
        'If BottomMenu IsNot Nothing Then
        '    BottomMenu.Style.Add("color", "Cyan")
        'End If

    End Sub

    Private Sub checkParentPagePostback(ByVal poControl As Control)
        If TypeOf poControl Is IParentPagePostback Then
            'calling getPostBackClientEvent will trigger the function __doPostBack() to be declared along w/ the hidden events
            Dim unusedString As String = ClientScript.GetPostBackEventReference(Page, "")
            CType(poControl, IParentPagePostback).ChildClose(Request("__EVENTARGUMENT"))
        End If

        For Each oChildControl As Control In poControl.Controls
            checkParentPagePostback(oChildControl)
        Next
    End Sub
End Class