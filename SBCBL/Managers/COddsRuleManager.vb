Imports System.Xml
Imports System.Web
Imports SBCBL.std
Imports SBCBL.Managers
Imports WebsiteLibrary.CXMLUtils

Namespace Managers
    Public Class COddsRuleManager
        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Private Const ODDS_RULES As Integer = 1
        Private Const RULE As Integer = 0
        Private Const FILE_NAME As String = "Odds_Rules.xml"
        Private FILESYSTEM As String = "SBCSYSTEM"
        Private Const YES As String = "Y"
        Private Const NO As String = "N"
        Private Const ID As String = "ID"
        Private Const LOWERTHAN As String = "LowerThan"
        Private Const GREATERTHAN As String = "GreaterThan"
        Private Const INCREASE As String = "Increase"
        Private Const ISODDRULELOCKED As String = "IsOddRuleLocked"

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


        Public Function GetOddsRulesByID(ByVal psID As String) As DataTable

            Dim odtOddsRule As DataTable = New DataTable()
            odtOddsRule.Columns.Add(ID)
            odtOddsRule.Columns.Add(LOWERTHAN)
            odtOddsRule.Columns.Add(GREATERTHAN)
            odtOddsRule.Columns.Add(INCREASE)
            odtOddsRule.Columns.Add(ISODDRULELOCKED)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = String.Format("//Rule[@ID='{0}']/Condition", psID)
                Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                Dim drOddRule As DataRow = odtOddsRule.NewRow()
                drOddRule(ID) = SafeString(psID)
                drOddRule(LOWERTHAN) = SafeString(oNodes(RULE).Attributes(LOWERTHAN).Value)
                drOddRule(GREATERTHAN) = SafeString(oNodes(RULE).Attributes(GREATERTHAN).Value)
                drOddRule(INCREASE) = SafeString(oNodes(RULE).Attributes(INCREASE).Value)
                drOddRule(ISODDRULELOCKED) = SafeString(oNodes(RULE).Attributes(ISODDRULELOCKED).Value)
                odtOddsRule.Rows.Add(drOddRule)
                Return odtOddsRule
            End If
            Return odtOddsRule
        End Function


        Public Function GetALLOddsRules() As DataTable

            Dim odtOddsRule As DataTable = New DataTable()
            odtOddsRule.Columns.Add("ID")
            odtOddsRule.Columns.Add(LOWERTHAN)
            odtOddsRule.Columns.Add(GREATERTHAN)
            odtOddsRule.Columns.Add(INCREASE)
            odtOddsRule.Columns.Add(ISODDRULELOCKED)
            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                oDoc.Load(oHandle.LocalFileName)
                Dim sXMLPath As String = "Odds_Rules/Rule"
                Dim olistRulesNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)

                For Each oRuleNode As XmlNode In olistRulesNodes
                    Dim drOddRule As DataRow = odtOddsRule.NewRow()
                    drOddRule("ID") = SafeString(oRuleNode.Attributes("ID").Value)
                    drOddRule(LOWERTHAN) = SafeString(oRuleNode.ChildNodes(0).Attributes(LOWERTHAN).Value)
                    drOddRule(GREATERTHAN) = SafeString(oRuleNode.ChildNodes(0).Attributes(GREATERTHAN).Value)
                    drOddRule(INCREASE) = SafeString(oRuleNode.ChildNodes(0).Attributes("Increase").Value)
                    drOddRule(ISODDRULELOCKED) = SafeString(oRuleNode.ChildNodes(0).Attributes(ISODDRULELOCKED).Value)
                    odtOddsRule.Rows.Add(drOddRule)
                Next


                Return odtOddsRule
            End If
            Return odtOddsRule
        End Function

        Public Function InsertOddsRules(ByVal psID As String, ByVal psLowerThan As String, ByVal psGreaterThan As String, ByVal psiIncrease As String, ByVal pbIsOddRuleLocked As Boolean) As Boolean
            Try
                Dim oDoc As New XmlDocument()
                Dim oDB As New FileDB.CFileDB()
                Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)

                If oHandle Is Nothing Then
                    oHandle = oDB.GetNewFileHandle()
                    IO.File.WriteAllText(oHandle.LocalFileName, "<?xml version=""1.0"" encoding=""UTF-8""?><Odds_Rules></Odds_Rules>")
                End If

                oDoc.Load(oHandle.LocalFileName)
                Dim oRule As XmlNode = oDoc.CreateElement("Rule")
                Dim oCondition As XmlNode = oDoc.CreateElement("Condition")
                Dim oLowerThan As XmlAttribute = oDoc.CreateAttribute(LOWERTHAN)
                oLowerThan.Value = psLowerThan
                Dim oGreaterThan As XmlAttribute = oDoc.CreateAttribute(GREATERTHAN)
                oGreaterThan.Value = psGreaterThan
                Dim oIncrease As XmlAttribute = oDoc.CreateAttribute(INCREASE)
                oIncrease.Value = SafeString(psiIncrease)
                Dim opbIsOddRuleLocked As XmlAttribute = oDoc.CreateAttribute(ISODDRULELOCKED)
                opbIsOddRuleLocked.Value = SafeString(IIf(pbIsOddRuleLocked, YES, NO))
                oCondition.Attributes.Append(oLowerThan)
                oCondition.Attributes.Append(oGreaterThan)
                oCondition.Attributes.Append(oIncrease)
                oCondition.Attributes.Append(opbIsOddRuleLocked)
                Dim oRuleID As XmlAttribute = oDoc.CreateAttribute("ID")
                oRuleID.Value = psID
                oRule.Attributes.Append(oRuleID)
                oRule.AppendChild(oCondition)
                oDoc.DocumentElement.AppendChild(oRule)
                oDoc.Save(oHandle.LocalFileName)
                oDB.PutFile(FILESYSTEM, FILE_NAME, oHandle)
            Catch ex As Exception
                log.Error("Fail to insert Odds Rules", ex)
                Return False
            End Try

            Return True
        End Function

        Public Function UpdateOddsRules(ByVal psID As String, ByVal psLowerThan As String, ByVal psGreaterThan As String, ByVal psiIncrease As String, ByVal pbIsOddRuleLocked As Boolean) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//Rule[@ID='{0}']/Condition", psID)
                    Dim oNodes As XmlNodeList = oDoc.SelectNodes(sXMLPath)
                    oNodes(RULE).Attributes(LOWERTHAN).Value = SafeString(psLowerThan)
                    oNodes(RULE).Attributes(GREATERTHAN).Value = SafeString(psGreaterThan)
                    oNodes(RULE).Attributes(INCREASE).Value = SafeString(psiIncrease)
                    oNodes(RULE).Attributes(ISODDRULELOCKED).Value = SafeString(IIf(pbIsOddRuleLocked, YES, NO))
                    oDoc.Save(oHandle.LocalFileName)
                    oDB.PutFile(FILESYSTEM, FILE_NAME, oHandle)
                Catch ex As Exception
                    Return False
                End Try
            End If
            Return True
        End Function

        Public Function DeleteOddsRulesByID(ByVal psID As String) As Boolean

            Dim oDoc As New XmlDocument()
            Dim oDB As New FileDB.CFileDB()
            Dim oHandle As FileDB.CFileHandle = oDB.GetFile(FILESYSTEM, FILE_NAME)
            If Not oHandle Is Nothing Then
                Try
                    oDoc.Load(oHandle.LocalFileName)
                    Dim sXMLPath As String = String.Format("//Rule[@ID='{0}']", psID)
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

    End Class
End Namespace
