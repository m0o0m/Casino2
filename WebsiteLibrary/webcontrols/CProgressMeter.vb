Imports System
Imports System.Drawing
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel


Public Class CProgressMeter
    Inherits WebControl

    ' Member variables
    Dim _percentage As Integer = 0
    Dim _cellCount As Integer = 20
    Dim _fillImageUrl As String = ""
    Dim _barImageUrl As String = ""
    Dim _imageGeneratorUrl As String = ""

    Public Sub New()
        BackColor = System.Drawing.Color.LightGray
        ForeColor = System.Drawing.Color.Blue
        BorderColor = Color.Empty

        Width = Unit.Pixel(100)
        Height = Unit.Pixel(16)
    End Sub

    Public Property PercentageStep() As Integer
        Get
            Return CInt(100 / _cellCount)
        End Get
        Set(ByVal value As Integer)
            If 100 Mod value <> 0 Then
                Throw New ArgumentException("The percentage step value must be divisible by 100")
            End If

            _cellCount = CInt(100 / value)
        End Set
    End Property

    Public Property FillImageUrl() As String
        Get
            Return _fillImageUrl
        End Get
        Set(ByVal value As String)
            _fillImageUrl = value
        End Set
    End Property

    Public Property BarImageUrl() As String
        Get
            Return _barImageUrl
        End Get
        Set(ByVal value As String)
            _barImageUrl = value
        End Set
    End Property

    Public Property ImageGeneratorUrl() As String
        Get
            Return _imageGeneratorUrl
        End Get
        Set(ByVal value As String)
            _imageGeneratorUrl = value
        End Set
    End Property

    Public Property Percentage() As Integer
        Get
            Return _percentage
        End Get
        Set(ByVal value As Integer)
            If (value > 100) Then    ' Greater than 100 is still 100
                _percentage = 100
            ElseIf (value < 0) Then ' Less than 0 is stil 0
                _percentage = 0
            Else
                _percentage = value
            End If
        End Set
    End Property

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim width As Integer = CInt(Me.Width.Value)

        writer.Write("<div style='display: inline;'>")
        If (BorderColor <> Color.Empty) Then
            writer.Write("<table border='0' cellspacing='0' cellpadding='1' bgColor='" & _
                ColorTranslator.ToHtml(BorderColor) & "'><tr><td>")
        End If

        writer.Write("<table border='0' cellspacing='0' cellpadding='0' height='" & Height.ToString & "' bgColor='" & ColorTranslator.ToHtml(BackColor) & "' width='" & Width & "'><tr>")
        writer.Write("<td bgColor='" & ColorTranslator.ToHtml(ForeColor) & "'><img src='/CC/images/spacer.gif' height='" & Height.ToString & "' width='" & Percentage / 100 * width & "'></td>")
        writer.Write("<td bgColor='" & ColorTranslator.ToHtml(BackColor) & "'><img src='/CC/images/spacer.gif' height='" & Height.ToString & "' width='" & width - (Percentage / 100 * width) & "'></td>")
        writer.Write("</tr></table>")

        If (BorderColor <> Color.Empty) Then
            writer.Write("</td></tr></table>")
        End If
        writer.Write("</div>")
    End Sub

    'Protected Overrides Sub Render(ByVal output As HtmlTextWriter)
    '    Dim width As Integer = CInt(Me.Width.Value)

    '    If (ImageGeneratorUrl <> "") Then

    '        Dim sBorderColor As String = ""
    '        If (BorderColor <> Color.Empty) Then
    '            sBorderColor = "&bc=" & ColorTranslator.ToHtml(BorderColor)

    '            output.Write(String.Format("<img src='{0}?w={1}&h={2}&p={3}&fc={4}&bk={5}{6}' border='0' width='{1}' height='{2}'>", _
    '                  ImageGeneratorUrl, width, Height.ToString(), Percentage, ColorTranslator.ToHtml(ForeColor), ColorTranslator.ToHtml(BackColor), _
    '                  BorderColor))
    '        End If
    '    Else

    '        ' border ??
    '        If (BorderColor <> Color.Empty) Then
    '            output.Write("<table border='0' cellspacing='0' cellpadding='1' bgColor='" & _
    '            ColorTranslator.ToHtml(BorderColor) & "'><tr><td>")
    '        End If

    '        If (BarImageUrl = "") Then
    '            output.Write("<table border='0' cellspacing='0' cellpadding='0' height='" & Height.ToString & "' bgColor='" & ColorTranslator.ToHtml(BackColor) & "'><tr>")

    '            Dim cellWidth As Integer = CInt(width / _cellCount)
    '            Dim curPercentage As Integer = 0
    '            Dim cellColor As String = ""

    '            Dim cellValue As String = ""
    '            If (Page.Request.Browser.Browser.ToUpper() = "NETSCAPE") Then
    '                If (FillImageUrl <> "") Then
    '                    cellValue = "<img src='" & FillImageUrl & "' border='0' width='" & cellWidth & "'>"
    '                Else
    '                    cellValue = "&nbsp"
    '                End If
    '            End If


    '            ' Create the cells
    '            For i As Integer = 0 To _cellCount - 1
    '                If (curPercentage < Percentage) Then
    '                    cellColor = " bgColor='" & ColorTranslator.ToHtml(ForeColor) & "'"
    '                Else
    '                    cellColor = ""
    '                End If

    '                If (i = 0) Then
    '                    output.Write("<td height='" & Height.ToString & "' width='" & cellWidth & "'" & cellColor & ">" & cellValue & "</td>")
    '                Else
    '                    output.Write("<td width='" & cellWidth & "'" & cellColor & ">" & cellValue & "</td>")
    '                End If

    '                curPercentage += PercentageStep
    '            Next

    '            output.Write("</tr></table>")

    '        Else
    '            ' Use a image as the bar 
    '            Dim imageWidth As Integer = CInt((Percentage / 100.0) * width)

    '            output.Write("<table border='0' cellpadding='0' cellSpacing='0' bgColor='" & ColorTranslator.ToHtml(BackColor) & "'><tr><td width='" & width & "'>")
    '            output.Write("<img src='" & BarImageUrl & "' width='" & imageWidth & "' height='" & Height.ToString & "'>")
    '            output.Write("</td></tr></table>")
    '        End If

    '        If (BorderColor <> Color.Empty) Then
    '            output.Write("</td></tr></table>")
    '        End If
    '    End If
    'End Sub
End Class