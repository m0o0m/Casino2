Imports System.Xml
Imports System.Web
Imports SBCBL.std
Imports System.Text
Imports System.Web.UI.WebControls

Namespace Managers
    Public Class CCustomizeLoginTemplateManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private _oServer As System.Web.HttpServerUtility
        Private _URL As String
        Private _PlaceHolder As PlaceHolder
        Private _Page As System.Web.UI.Page
        Dim sDefaultTemplate As String = "Blue"

        Public Sub New(ByVal poServer As System.Web.HttpServerUtility, ByVal psURLHost As String, ByVal poPlaceHolder As PlaceHolder, ByVal poPage As Web.UI.Page)
            _oServer = poServer
            _URL = psURLHost
            _PlaceHolder = poPlaceHolder
            _Page = poPage
        End Sub

        Public Function GetLoginPageHTML(ByVal psPath As String) As Boolean
            Try
                Dim sFileName As String = psPath + "Login\Login.xml"
                Dim oWhiteLabel As New SBCBL.CacheUtils.CWhiteLabelSettings()
                Dim oDoc As New XmlDocument()
                Dim iIndex As Integer = 0
                Dim iLength As Integer = 0
                Dim oLiteral As New Literal

                oWhiteLabel = oWhiteLabel.LoadByUrl(_URL)
                oDoc.Load(sFileName)

                Dim sHTML As String = _oServer.HtmlDecode(oDoc.DocumentElement.SelectSingleNode("/templates/template[@name='" & SafeString(IIf(oWhiteLabel.LoginTemplate <> "", oWhiteLabel.LoginTemplate, sDefaultTemplate)) & "']").InnerText)
                If Not String.IsNullOrEmpty(oWhiteLabel.BackgroundLoginImage) Then
                    sHTML = sHTML.Replace("[BACKGROUNDLOGIN]", "<img src='" & oWhiteLabel.BackgroundLoginImage & "' alt='background image' id='bg' />")
                Else
                    sHTML = sHTML.Replace("[BACKGROUNDLOGIN]", "")
                End If
                If Not String.IsNullOrEmpty(oWhiteLabel.LeftBackgroundLoginImage) Then
                    sHTML = sHTML.Replace("[LEFTIMAGELOGIN]", oWhiteLabel.LeftBackgroundLoginImage)
                End If
                If Not String.IsNullOrEmpty(oWhiteLabel.RightBackgroundLoginImage) Then
                    sHTML = sHTML.Replace("[RIGHTIMAGELOGIN]", oWhiteLabel.RightBackgroundLoginImage)
                End If
                If Not String.IsNullOrEmpty(oWhiteLabel.BottomBackgroundLoginImage) Then
                    sHTML = sHTML.Replace("[BOTTOMIMAGELOGIN]", oWhiteLabel.BottomBackgroundLoginImage)
                End If
                sHTML = sHTML.Replace("[LOGO]", oWhiteLabel.LoginLogo)
                sHTML = sHTML.Replace("[TITLE]", "Sport Bet System")
                iLength = sHTML.IndexOf("[LOGIN]")
                oLiteral.Text = sHTML.Substring(0, iLength)
                _PlaceHolder.Controls.Add(oLiteral)

                ReplaceLoginControl()

                iIndex = iLength + "[LOGIN]".Length
                iLength = sHTML.Length - iIndex
                oLiteral = New Literal
                oLiteral.Text = sHTML.Substring(iIndex, iLength)
                _PlaceHolder.Controls.Add(oLiteral)

            Catch ex As Exception
                LogError(log, "GetLoginPageHTML ", ex)
            End Try
            Return False
        End Function

        Public Sub ReplaceLoginControl()
            Dim oControl As Web.UI.Control = _Page.LoadControl("/Login/LoginControl.ascx")
            _PlaceHolder.Controls.Add(oControl)
        End Sub

    End Class
End Namespace

