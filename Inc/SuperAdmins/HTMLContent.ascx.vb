Imports SBCBL.std
Imports FileDB

Partial Class Inc_SuperAdmins_HTMLContent
    Inherits SBCBL.UI.CSBCUserControl
    Private FILESYSTEM As String = SBCBL.std.GetSiteType & "SYSTEM"

#Region "property"
    Public Property FileName() As String
        Get
            Return SafeString(ViewState("FileName"))
        End Get
        Set(ByVal value As String)
            ViewState("FileName") = value
        End Set
    End Property
#End Region

    Public Sub loadFileDBContent(ByVal psFileDBType As String)
        Dim sFileName As String = getFileName(psFileDBType)
        FileName = psFileDBType
        If sFileName = "" Then
            Return
        End If

        Dim oFileDB As New FileDB.CFileDB()
        Dim oHandle As FileDB.CFileHandle = oFileDB.GetFile(FILESYSTEM, sFileName)
        If oHandle IsNot Nothing Then
            fckContent.Value = IO.File.ReadAllText(oHandle.LocalFileName)
        Else
            fckContent.Value = ""
        End If
    End Sub

    Private Function getFileName(ByVal psFileDBType As String) As String
        Select Case UCase(psFileDBType)
            Case "RULES"
                Return "RULES.txt"

            Case "ODDS"
                Return "ODDS.txt"

            Case "CCAGENT_LETTER_TEAMPLATE"
                Return "CCAGENT_LETTER_TEAMPLATE.txt"
        End Select

        Return ""
    End Function

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        'If fckContent.Value = "" Then
        '    ClientAlert("Content is required", True)
        '    fckContent.Focus()
        'End If

        Dim sFileName As String = getFileName(FileName)

        If sFileName = "" Then
            ClientAlert("Failed to Save Setting", True)
            Return
        End If

        Dim oFileDB As New FileDB.CFileDB()
        Dim oHandle As FileDB.CFileHandle = oFileDB.GetNewFileHandle()

        IO.File.WriteAllText(oHandle.LocalFileName, fckContent.Value)
        oFileDB.PutFile(FILESYSTEM, sFileName, oHandle)

        ClientAlert("Successfully saved", True)
    End Sub
End Class
