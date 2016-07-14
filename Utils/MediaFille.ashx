<%@ WebHandler Language="VB" Class="MediaFile" %>

Imports System
Imports System.Web
Imports FileDB
Imports SBCBL.std

Public Class MediaFile : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim sFileName As String = SafeString(context.Request("fname"))
        Dim oDB As New FileDB.CFileDB()
        Dim oHandle As FileDB.CFileHandle = oDB.GetFile(SBCBL.CFileDBKeys.SBC_ASTERIA, sFileName)
        With context
            .Response.ClearHeaders()
            .Response.Clear()
            .Response.ContentType = "audio/mpeg"
            .Response.AddHeader("Content-Type", "audio/mpeg")
            .Response.AddHeader("Content-Disposition", "attachment; filename=""" & sFileName & """")
            .Response.Buffer = True
            Dim oBytes As Byte() = IO.File.ReadAllBytes(oHandle.LocalFileName)
            .Response.OutputStream.Write(oBytes, 0, CInt(oBytes.Length))
        End With
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class