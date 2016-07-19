Imports System.Xml
Imports System.Web
Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils

Namespace Managers
    Public Class CSubjectEmailManager

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())
        Private Const ODDS_RULES As Integer = 1
        Private Const RULE As Integer = 0
        Private Const FILE_NAME As String = "SUBJECT_EMAIL.xml"
        Private FILESYSTEM As String = "SBCSYSTEM"
        Private Const YES As String = "Y"
        Private Const NO As String = "N"
        Private Const ID As String = "ID"
        Private Const SUBJECT_EMAIL As String = "SUBJECT EMAIL"
        Private Const SUBJECT As String = "Subject"

        Private ReadOnly Property SiteType() As SBCBL.CEnums.ESiteType
            Get
                Dim oCache As New SBCBL.CacheUtils.CCacheManager()
                Dim eSiteType As SBCBL.CEnums.ESiteType = oCache.GetSiteType(HttpContext.Current.Request.Url.Host)
                Return eSiteType
            End Get
        End Property

        Public Sub New()
            If SiteType = SBCBL.CEnums.ESiteType.SBS Then
                FILESYSTEM = "SBSSYSTEM"
            End If
        End Sub

        Public Function GetSubjectEmailID(ByVal psID As String) As DataTable
            Dim odtSubjectEmail As DataTable = New DataTable()
            odtSubjectEmail.Columns.Add(ID)
            odtSubjectEmail.Columns.Add(SUBJECT)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("//Subject[@ID='{0}']", psID)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                If oNodes.Count = 0 Then
                    Return Nothing
                End If
                Dim drSubjectEmail As DataRow = odtSubjectEmail.NewRow()
                drSubjectEmail(ID) = SafeString(psID)
                drSubjectEmail(SUBJECT_EMAIL) = SafeString(oNodes(0).InnerText)
                odtSubjectEmail.Rows.Add(drSubjectEmail)
                Return odtSubjectEmail
            End If
            Return odtSubjectEmail
        End Function

        Public Function GetALLSubjectEmail() As DataTable
            Dim odtOddsRule As DataTable = New DataTable()
            odtOddsRule.Columns.Add("ID")
            odtOddsRule.Columns.Add(SUBJECT)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = "SUBJECT_EMAIL/Subject"
                Dim olistRulesNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                For Each oRuleNode As XmlNode In olistRulesNodes
                    Dim drSubjectEmail As DataRow = odtOddsRule.NewRow()
                    drSubjectEmail("ID") = SafeString(oRuleNode.Attributes("ID").Value)
                    drSubjectEmail(SUBJECT) = SafeString(oRuleNode.InnerText)
                    odtOddsRule.Rows.Add(drSubjectEmail)
                Next
                Return odtOddsRule
            End If
            Return odtOddsRule
        End Function

        Public Function InsertSubjectEmail(ByVal psID As String, ByVal psSubject As String) As Boolean
            Try
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
                If oHandle Is Nothing Then
                    oHandle = oDB.GetNewFileHandle()
                    IO.File.WriteAllText(oHandle.LocalFileName, "<?xml version=""1.0"" encoding=""UTF-8""?><SUBJECT_EMAIL></SUBJECT_EMAIL>")
                End If
                oDoc.Load(oHandle.LocalFileName)
                Dim oSubject As XmlNode = oDoc.CreateElement("Subject")
                Dim oSubjectID As XmlAttribute = oDoc.CreateAttribute("ID")
                oSubjectID.Value = psID
                oSubject.Attributes.Append(oSubjectID)
                oSubject.InnerText = psSubject
                oDoc.DocumentElement.AppendChild(oSubject)
                oDoc.Save(oHandle.LocalFileName)
                oDB.PutFile(FILESYSTEM, FILE_NAME, oHandle)
            Catch ex As Exception
                log.Error("Fail to insert Email Subject", ex)
                Return False
            End Try
            Return True
        End Function

        Public Function UpdateSubectEmail(ByVal psID As String, ByVal psSubject As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//Subject[@ID='{0}']", psID)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(0).InnerText = psSubject
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function DeleteSubectEmailByID(ByVal psID As String) As Boolean
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//Subject[@ID='{0}']", psID)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(RULE).ParentNode.RemoveChild(oNodes(RULE))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function CheckSubjectEmailExist(ByVal psID As String, ByVal psSubjectEmail As String) As Boolean
            Dim odtSubjectEmail As DataTable = GetALLSubjectEmail()
            Dim odrSubjectEmail As DataRow()
            If String.IsNullOrEmpty(psID) Then
                odrSubjectEmail = odtSubjectEmail.Select(String.Format("Subject='{0}'", psSubjectEmail))
                If odrSubjectEmail.Length > 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                odrSubjectEmail = odtSubjectEmail.Select(String.Format("ID<>'{0}' and Subject={1} ", psID, SQLString(psSubjectEmail)))
                If odrSubjectEmail.Length > 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
            Return False
        End Function
    End Class
End Namespace