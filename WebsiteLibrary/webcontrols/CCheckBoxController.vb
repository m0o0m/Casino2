Imports System.Data.OleDb
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.ComponentModel

'this control works by generating a checkbox that will check/uncheck (client script) a bunch of child checkboxes DSLd on the matchPattern
'to use, either set the matchpattern or the ChildCheckBoxes
'TODO: implement childchekcboxes proeprty so we can use for the places like bureau selection
Public Class CCheckBoxController
    Inherits Control

    Public Enum ControllerTypes
        eCheckBox
        eButton
    End Enum

    Private _ControllerType As ControllerTypes = ControllerTypes.eCheckBox
    Public Property ControllerType() As ControllerTypes
        Get
            Return _ControllerType
        End Get
        Set(ByVal Value As ControllerTypes)
            _ControllerType = Value
        End Set
    End Property

    'checkboxes that matchpattern as a substring will be treated as child and selected/unselected as necessary
    Public Property MatchPattern() As String
        Get
            Return CStr(viewstate("MatchPattern"))
        End Get
        Set(ByVal Value As String)
            viewstate("MatchPattern") = Value
        End Set
    End Property

    'comma deliminated list of child check boxes to control
    'Public Property ChildCheckBoxes() As String
    '    Get

    '    End Get
    '    Set(ByVal Value As String)

    '    End Set
    'End Property

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim s As String = ""
        Select Case ControllerType
            Case ControllerTypes.eCheckBox
                s = String.Format("<input type=checkbox id=""{0}"" onclick=""setChildCheckBoxes(this, '{1}', 'CHK');"" >", Me.ClientID, MatchPattern)
            Case ControllerTypes.eButton
                s = String.Format("<input type=button id=""{0}"" onclick=""setChildCheckBoxes(this, '{1}', 'BTN');"" value='Select All'>", Me.ClientID, MatchPattern)
        End Select

        writer.WriteLine(s)

        MyBase.Render(writer)
    End Sub

    Private Sub injectScript()
        Const SCRIPT_KEY As String = "setChildCheckBoxes"
        If Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType(), SCRIPT_KEY) Then
            Return
        End If

        Dim s As New System.Text.StringBuilder()
        s.Append("<script>" & vbCrLf)
        s.Append("function setChildCheckBoxes(oController, sPrefix, sMode)" & vbCrLf)
        s.Append("{" & vbCrLf)
        s.Append("	for (var i=0; i< document.myform.elements.length;i++)" & vbCrLf)
        s.Append("	{" & vbCrLf)
        s.Append("		var e = document.myform.elements[i];" & vbCrLf)
        s.Append("		if(-1 != e.id.toUpperCase().indexOf( sPrefix.toUpperCase()) &&" & vbCrLf)
        s.Append("		e.type =='checkbox' )" & vbCrLf)
        s.Append("		{" & vbCrLf)
        s.Append("			if ('BTN'==sMode)				" & vbCrLf)
        s.Append("				setCheckBoxByButton (oController, e);" & vbCrLf)
        s.Append("			else" & vbCrLf)
        s.Append("				setCheckBoxByCheckBox (oController,e);" & vbCrLf)
        s.Append("		}" & vbCrLf)
        s.Append("	}" & vbCrLf)
        s.Append("	if ('BTN'==sMode)" & vbCrLf)
        s.Append("		oController.value = oController.value == 'Select All' ? 'Unselect All' : 'Select All';" & vbCrLf)
        s.Append("}" & vbCrLf)

        s.Append("function setCheckBoxByButton(oController, oChild)" & vbCrLf)
        s.Append("{" & vbCrLf)
        s.Append("	oChild.checked = oController.value =='Select All';" & vbCrLf)
        s.Append("}" & vbCrLf)

        s.Append("function setCheckBoxByCheckBox(oController, oChild)" & vbCrLf)
        s.Append("{" & vbCrLf)
        s.Append("	oChild.checked = oController.checked;" & vbCrLf)
        s.Append("}" & vbCrLf)
        s.Append("</script>" & vbCrLf)

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), SCRIPT_KEY, s.ToString)
    End Sub

    Private Sub CCheckBoxController_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        injectScript()
    End Sub
End Class
