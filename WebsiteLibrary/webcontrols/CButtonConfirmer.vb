Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Web.UI

<Designer(GetType(CButtonConfirmerDesigner))> _
Public Class CButtonConfirmer
    Inherits System.Web.UI.Control
    'you may want to disable this controls viewstate when:
    ' You are binding a diff value to the confirmExpression/attachto at runtime which would cause the viewstate size to change 
    ' Viewstate is used for consistency reason and in the event we need to refer to the property on postback, it's readily available...
    ' Setting the prop @ design time will incur 0 viewstate size overhead.

    <Description("The name of the control that the client side confirmation code will be injected.")> _
    Public Property AttachTo() As String
        Get
            If viewstate("AttachTo") Is Nothing Then
                Return ""
            End If
            Return CType(viewstate("AttachTo"), String)
        End Get
        Set(ByVal Value As String)
            viewstate("AttachTo") = Value
        End Set
    End Property

    <Description("The expression to evaluate which will determine whether or not the control continues exectuion. Ex: ""confirm('Are you sure you want to delete')""")> _
    Public Property ConfirmExpression() As String
        Get
            If viewstate("ConfirmExpression") Is Nothing Then
                Return ""
            End If
            Return CType(viewstate("ConfirmExpression"), String)
        End Get
        Set(ByVal Value As String)
            viewstate("ConfirmExpression") = Value
        End Set
    End Property


    Private Sub CButtonConfirmer_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        If ConfirmExpression = "" Then
            Return 'don't bind to onclick if expression is empty.  Just ignore this control
        End If

        Dim sConfirm As String = "if(!" & ConfirmExpression & ") return false;"
        Dim ctl As Control = Parent.FindControl(AttachTo)

        If (Not IsNothing(ctl)) Then
            If TypeOf ctl Is WebControl AndAlso CSBCStd.SafeString(CType(ctl, WebControl).Attributes("onClick")).IndexOf(sConfirm) < 0 Then
                CType(ctl, WebControl).Attributes("onClick") = sConfirm & CType(ctl, WebControl).Attributes("onClick")
            ElseIf TypeOf ctl Is HtmlControls.HtmlControl AndAlso CSBCStd.SafeString(CType(ctl, HtmlControls.HtmlControl).Attributes("onClick")).IndexOf(sConfirm) < 0 Then
                CType(ctl, HtmlControls.HtmlControl).Attributes("onClick") = sConfirm & CType(ctl, HtmlControls.HtmlControl).Attributes("onClick")
            End If
        End If
    End Sub

End Class

Public Class CButtonConfirmerDesigner
    Inherits System.Web.UI.Design.ControlDesigner

    Public Overrides Function GetDesignTimeHtml() As String
        Return "ButtonConfirmer: " & CType(Me.Component, CButtonConfirmer).AttachTo
    End Function
End Class