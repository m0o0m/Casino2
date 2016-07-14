Imports System.Xml
Imports SBCBL.std
Partial Class Inc_SuperAdmins_MessagePlayers
    Inherits System.Web.UI.UserControl
    Dim _log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
    Dim _sFileName As String = "Message_player.xml"
    Private FILESYSTEM As String = SBCBL.std.GetSiteType() & "SYSTEM"

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim sXML = "<Message_player> <One_Time>{0}</One_Time>" & _
                    "<Permanent>{1}</Permanent></Message_player>"
        sXML = String.Format(sXML, SafeString(txtOneTime.Text), SafeString(txtPermanent.Text))

        Try
            Dim oFileDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oFileDB.GetNewFileHandle()

            '' save onetime message to players table
            If SafeString(txtOneTime.Text) <> "" Then
                Dim oPlayerManager As New SBCBL.Managers.CPlayerManager()
                oPlayerManager.SendOneTimeMessage(SafeString(txtOneTime.Text), SBCBL.std.GetSiteType())
            End If

            IO.File.WriteAllText(oHandle.LocalFileName, sXML)
            oFileDB.PutFile(FILESYSTEM, _sFileName, oHandle)

            txtOneTime.Text = ""
            'txtPermanent.Text = ""
            ClientAlert("Successfully Send ", True)
        Catch ex As Exception
            _log.Error("Can't save message to players: " & ex.Message, ex)
            ClientAlert("Unsuccessfully Send", True)
        End Try
    End Sub

    Private Sub LoadMessages()
        Dim oFileDB As New FileDB.CFileDB()
        Dim oHandle As FileDB.CFileHandle = oFileDB.GetFile(FILESYSTEM, _sFileName)

        If oHandle IsNot Nothing Then
            Dim oDoc As New XmlDocument()
            Dim sContent = System.IO.File.ReadAllText(oHandle.LocalFileName)
            If sContent = "" Then
                Return
            End If

            oDoc.LoadXml(sContent)
            'Dim oNodeOneTime As XmlElement = oDoc.SelectSingleNode("Message_player/One_Time")
            'txtOneTime.Text = oNodeOneTime.InnerText
            Dim oNodePermanent As XmlElement = oDoc.SelectSingleNode("Message_player/Permanent")
            txtPermanent.Text = oNodePermanent.InnerText
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadMessages()
        End If
    End Sub
End Class
