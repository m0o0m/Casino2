Imports SBCBL.std
Imports FileDB

Namespace SBCWebsite

    Partial Class Inc_contentFileDB
        Inherits System.Web.UI.UserControl

        Public Sub LoadFileDBContent(ByVal psFileDBType As String)
            Dim sFileName As String = "C:\FileDB\SBSSYSTEM\" & getFileName(psFileDBType)

            'If sFileName = "" Then
            '    Return
            'End If

            'Dim oFileDB As New FileDB.CFileDB()
            'Dim oHandle As FileDB.CFileHandle = Nothing  '= oFileDB.GetFile("SBCSYSTEM", sFileName)


            'Select Case UCase(SBCBL.std.GetSiteType)
            '    Case "SBC"
            '        oHandle = (oFileDB.GetFile("SBCSYSTEM", sFileName))

            '    Case "SBS"
            '        oHandle = (oFileDB.GetFile("SBSSYSTEM", sFileName))
            'End Select


            'If oHandle IsNot Nothing Then
            ltrContent.Text = IO.File.ReadAllText(sFileName)
            'End If
        End Sub

        Private Function getFileName(ByVal psFileDBType As String) As String
            Select Case UCase(psFileDBType)
                Case "RULES"
                    Return "RULES.htm"

                Case "ODDS"
                    Return "ODDS.htm"

                Case "CCAGENT_LETTER_TEAMPLATE"
                    Return "CCAGENT_LETTER_TEAMPLATE.txt"
            End Select

            Return ""
        End Function

    End Class

End Namespace

