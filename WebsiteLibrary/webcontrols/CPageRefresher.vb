Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.ComponentModel
Imports WebsiteLibrary.CSBCStd

<Designer(GetType(CPageRefresherDesigner))> _
Public Class CPageRefresher
    Inherits System.Web.UI.Control
    Implements IPostBackEventHandler

    Public Event PageRefreshed(ByVal sender As Object, ByVal e As System.EventArgs)

    <DefaultValue(GetType(String), ""), Description("Expression or function to execute that will return a boolean to indicate whether page refreshing should take place")> _
        Public Property RefreshConditionExpression() As String
        Get
            If IsNothing(viewstate("RefreshConditionExpression")) Then
                Return ""
            End If

            Return CStr(viewstate("RefreshConditionExpression"))
        End Get
        Set(ByVal Value As String)
            viewstate("RefreshConditionExpression") = Value
        End Set
    End Property

    <DefaultValue(GetType(Long), "1000"), Description("Time in milliseconds to wait before refreshing page")> _
    Public Property Interval() As Long
        Get
            If IsNothing(viewstate("Interval")) Then
                Return 1000
            End If

            Return CLng(viewstate("Interval"))
        End Get
        Set(ByVal Value As Long)
            If Value <= 0 Then
                Throw New ArgumentException("Value must be greater than 0")
            End If
            viewstate("Interval") = Value
        End Set
    End Property

    Private Property _StartTime() As DateTime
        Get
            If IsNothing(viewstate("StartTime")) Then
                Return DateTime.MinValue
            End If
            Return CDate(viewstate("StartTime"))
        End Get
        Set(ByVal Value As DateTime)
            viewstate("StartTime") = Value
        End Set
    End Property

    'Private _Enabled As Boolean = True
    <DefaultValue(True)> _
    Public Property Enabled() As Boolean
        Get
            If IsNothing(viewstate("Enabled")) Then
                viewstate("Enabled") = True
            End If

            Return CBool(viewstate("Enabled"))
        End Get
        Set(ByVal Value As Boolean)
            viewstate("Enabled") = Value
        End Set
    End Property

    Private Sub CPageRefresher_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If _StartTime = DateTime.MinValue Then
            _StartTime = DateTime.Now
        End If
    End Sub

    Private Sub emitRefreshScript()

        Dim script As String = ""
        If RefreshConditionExpression <> "" Then
            script = String.Format("if({0}==true)", RefreshConditionExpression)
        End If

        Dim sInfiniteRefresher As String = "<script>" & vbCrLf & _
                                                           "function pRefresher" & Interval & "(sPostBackStr) {" & vbCrLf & _
                                                           "    setTimeout(sPostBackStr, 0);" & vbCrLf & _
                                                           "    setTimeout(""pRefresher" & Interval & "(\"""" + sPostBackStr + ""\"")"", " & Interval & ");" & vbCrLf & _
                                                           "} " & vbCrLf & _
                                                           "</script>"
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "INF_REFRESHER_" & Interval, sInfiniteRefresher)

        script += String.Format("<script>setTimeout(""pRefresher" & Interval & "(\""{0}\"")"", " & Interval & ");</script>", Page.ClientScript.GetPostBackEventReference(Me, ""))

        If Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "REFRESH_PAGE") Then
            Exit Sub
        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "REFRESH_PAGE", script)
    End Sub

    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        RaiseEvent PageRefreshed(Me, New EventArgs())
    End Sub

    Private Sub CPageRefresher_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        'we emit the script as late as possible so any event can cancel page refreshing
        If Me.Enabled = False Then
            Exit Sub
        End If

        emitRefreshScript()
    End Sub
End Class


Public Class CPageRefresherDesigner
    Inherits System.Web.UI.Design.ControlDesigner

    Public Overrides Function GetDesignTimeHtml() As String
        ' Component is the instance of the component or control that
        ' this designer object is associated with. This property is 
        ' inherited from System.ComponentModel.ComponentDesigner.
        Dim oPageRefresher As CPageRefresher = CType(Component, CPageRefresher)

        Dim sw As New System.IO.StringWriter()
        Dim tw As New System.Web.UI.HtmlTextWriter(sw)

        'put validator control's error text into this label's text

        Dim placeHolder As New Label()

        placeHolder.Font.Name = "Wingdings 2"
        placeHolder.Text = Chr(135)
        placeHolder.ToolTip = "Refresh page every :" & oPageRefresher.Interval & " milliseconds"
        If oPageRefresher.Enabled Then
            placeHolder.BackColor = System.Drawing.Color.LightBlue
        Else
            placeHolder.BackColor = System.Drawing.Color.Gray
        End If

        placeHolder.RenderControl(tw)

        Return sw.ToString()
    End Function
End Class