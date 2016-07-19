Imports SBCBL.CEnums
Imports System.IO
Imports SBCBL.std
Imports System.Web.UI
Imports System.Web
Imports WebsiteLibrary.DBUtils
Imports SBCBL.UI

Namespace UI

    Public Class CSBCPage
        Inherits WebsiteLibrary.CGlobalPage
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        ' this attribs are to prevent the vs.net designer from trying to display UserSession
        <System.ComponentModel.BrowsableAttribute(False), System.ComponentModel.DesignerSerializationVisibilityAttribute(0)> _
        Public ReadOnly Property UserSession() As CSBCSession
            Get
                Return New CSBCSession()
            End Get
        End Property


        Protected Overrides Sub SavePageStateToPersistenceMedium(ByVal viewState As Object)
            Dim oWriter As New StringWriter()

            Dim oFormat As New LosFormatter
            oFormat.Serialize(oWriter, viewState)

            Dim sViewStateStr As String = oWriter.ToString()

            Dim oBytes As Byte() = System.Convert.FromBase64String(sViewStateStr)
            oBytes = CvioZip.Compress(oBytes)

            Dim sStateStr As String = System.Convert.ToBase64String(oBytes)

            ScriptManager.RegisterHiddenField(Me, "__VSTATE", sStateStr)
        End Sub

        Protected Overrides Function LoadPageStateFromPersistenceMedium() As Object
            Dim vState As String = Me.Request.Form("__VSTATE")
            If SafeString(vState) <> "" Then
                Try
                    Dim bytes As Byte() = System.Convert.FromBase64String(vState)
                    bytes = CvioZip.Decompress(bytes)

                    Return New LosFormatter().Deserialize(System.Convert.ToBase64String(bytes))
                Catch ex As Exception
                    log.Error("Err occurred in Function LoadPageStateFromPersistenceMedium.  __VSTATE=" & vState, ex.InnerException)
                End Try
            End If
            Return MyBase.LoadPageStateFromPersistenceMedium
        End Function

        <System.Web.Services.WebMethod()> _
        Public Shared Function ZipLookup(ByVal psZipcode As String) As String
            Dim oZipLookup As New Utils.CZipCodeLookup()
            Try
                Dim oZipData As Utils.CZipData = oZipLookup.GetZipData(psZipcode)
                If oZipData Is Nothing OrElse oZipData.City Is Nothing OrElse oZipData.County Is Nothing Then
                    ''return a blank if cann't find the zip code
                    Return "NOT_FOUND"
                Else
                    Dim sResult As String = "FOUND_ZIP:" & oZipData.City & "||" & oZipData.County & "||" & oZipData.State & "||" & psZipcode
                    Return sResult
                End If
            Catch ex As Exception
                '_log.Error("Error at zip lookup:" & ex.Message, ex)
            End Try
            Return ""
        End Function

        Protected Overrides Sub OnPreInit(ByVal e As System.EventArgs)
            MyBase.OnPreInit(e)
        End Sub

        Private Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
            Try
                If UserSession.UserID = "" Then
                    Throw New Exception("Session Expired")
                End If
            Catch ex As Exception
                log.Error(ex.Message, ex.InnerException)
                Response.Redirect("/sbs/default.aspx")
            End Try
        End Sub

    End Class

End Namespace