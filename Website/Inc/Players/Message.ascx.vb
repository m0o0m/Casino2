Imports System.Xml

Namespace Players
    Partial Class Message
        Inherits SBCBL.UI.CSBCUserControl

        Dim _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Dim _sFileName As String = "Message_player.xml"
        Private FILESYSTEM As String = SBCBL.std.GetSiteType() & "SYSTEM"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
          
        End Sub

        Private Sub LoadMessage()
            Dim sOneTime As String = ""
            Dim sPamernent As String = ""
            Dim sMessage = Page.UserSession.PlayerUserInfo.OneTimeMessage
            If sMessage <> "" Then
                Dim sSplit As Char = Char.ConvertFromUtf32(221)
                Dim oListMessage() As String = sMessage.Replace("<split>", sSplit).Split(sSplit)
                For Each sText In oListMessage
                    If SBCBL.std.SafeString(sText) <> "" Then
                        sOneTime += String.Format("<li>{0}</li>", sText)
                    End If
                Next
            End If

            ' load Permanent Message
            Dim oFileDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oFileDB.GetFile(FILESYSTEM, _sFileName)
            If oHandle IsNot Nothing Then
                Dim oDoc As New XmlDocument()
                Dim sContent = System.IO.File.ReadAllText(oHandle.LocalFileName)
                If sContent = "" Then
                    Return
                End If

                oDoc.LoadXml(sContent)

                Dim oNodePermanent As XmlElement = oDoc.SelectSingleNode("Message_player/Permanent")
                sPamernent = oNodePermanent.InnerText
            End If
            Dim sReturn As String = ""

            If sPamernent <> "" Then
                sOneTime += "<li>" & sPamernent & "</li>"
            End If

            If sOneTime <> "" Then
                sReturn = sOneTime
                lblOut.Text = sReturn
            End If

          

            lblOut.Text = sReturn
            Me.Visible = sOneTime <> "" Or sPamernent <> ""

            If sOneTime <> "" Then
                '' reset this user message
                Dim oPlayerManager As New SBCBL.Managers.CPlayerManager()
                oPlayerManager.ResetOneTimeMessage(UserSession.PlayerUserInfo.UserID, SBCBL.std.GetSiteType())
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                LoadMessage()
            End If
        End Sub
    End Class
End Namespace
