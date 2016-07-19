Imports SBCBL.std
Imports SBCBL.CEnums
Imports WebsiteLibrary.DBUtils
Imports System.Xml

Namespace Managers

    Public Class CWhiteLabelSettingManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetURLBySiteType(ByVal psSiteType As String) As DataRow
            Dim odtWhiteLabelSetting As DataTable = Nothing

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("SiteType = " & SQLString(psSiteType))

            Dim sSQL As String = "SELECT top 1 * FROM WhiteLabelSettings " & oWhere.SQL
            log.Debug(String.Format("Get the list of WhiteLabelSetting by SiteType: {0}. SQL: {1}", psSiteType, sSQL))

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                odtWhiteLabelSetting = odbSQL.getDataTable(sSQL)
            Catch ex As Exception
                log.Error(String.Format("Cannot get the list of WhiteLabelSetting by psSiteType: {0}, SQL: {1}", psSiteType, sSQL), ex)
            Finally
                odbSQL.closeConnection()
            End Try

            If odtWhiteLabelSetting.Rows.Count > 0 Then
                Return odtWhiteLabelSetting.Rows(0)
            Else

                Return Nothing
            End If
        End Function

        Public Function GetSiteTypeByURL(ByVal psSiteURL As String) As String
            Dim sSiteType As String = ""

            Dim oWhere As New CSQLWhereStringBuilder
            oWhere.AppendANDCondition("SiteURL = " & SQLString(psSiteURL))

            Dim sSQL As String = "SELECT top 1 * FROM WhiteLabelSettings " & oWhere.SQL
            log.Debug(String.Format("Get the list of WhiteLabelSetting by SiteURL: {0}. SQL: {1}", psSiteURL, sSQL))

            Dim odbSQL As New CSQLDBUtils(SBC_CONNECTION_STRING, "")

            Try
                Dim oTbl As DataTable = odbSQL.getDataTable(sSQL)

                If oTbl IsNot Nothing AndAlso oTbl.Rows.Count > 0 Then
                    sSiteType = SafeString(oTbl.Rows(0)("SiteType"))
                End If
            Catch ex As Exception
                log.Error(String.Format("Cannot get the list of WhiteLabelSetting by psSiteURL: {0}, SQL: {1}", psSiteURL, sSQL), ex)
            Finally
                odbSQL.closeConnection()
            End Try

            Return sSiteType
        End Function

        Public Function GetLoginTemplates(ByVal psPath As String) As List(Of String)
            Dim oLoginTemplate As New List(Of String)
            Dim sFileName As String = psPath + "Login\Login.xml"
            Dim oDoc As New XmlDocument()
            oDoc.Load(sFileName)
            If oDoc IsNot Nothing Then
                For Each node As XmlNode In oDoc.DocumentElement.SelectNodes("/templates/template")
                    oLoginTemplate.Add(node.Attributes("name").Value)
                Next
            End If
            Return oLoginTemplate
        End Function

    End Class
End Namespace

