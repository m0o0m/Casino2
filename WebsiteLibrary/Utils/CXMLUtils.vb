Imports System.Xml

Public Class CXMLUtils

    Public Shared Function GetChildValue(ByVal poParent As XmlNode, ByVal psXPath As String, Optional ByVal poNSManager As XmlNamespaceManager = Nothing) As String
        Dim oChild As XmlNode = Nothing
        If poNSManager Is Nothing Then
            oChild = poParent.SelectSingleNode(psXPath)
        Else
            oChild = poParent.SelectSingleNode(psXPath, poNSManager)
        End If

        If oChild Is Nothing Then
            Return ""
        Else
            Return oChild.InnerText
        End If
    End Function

    Public Shared Function AddXMLChild(ByVal poParent As XmlNode, ByVal psName As String, Optional ByVal psInnerText As String = "", Optional ByVal pbInsertAtFront As Boolean = False, Optional ByVal psPrefix As String = "", Optional ByVal psNameSpace As String = "") As XmlElement
        Dim oXmlEl As XmlElement

        If psPrefix <> "" And psNameSpace <> "" Then
            oXmlEl = poParent.OwnerDocument.CreateElement(psPrefix, psName, psNameSpace)
        ElseIf psNameSpace <> "" Then
            oXmlEl = poParent.OwnerDocument.CreateElement(psName, psNameSpace)
        Else
            oXmlEl = poParent.OwnerDocument.CreateElement(psName, poParent.NamespaceURI)
        End If

        If psInnerText <> "" Then
            oXmlEl.InnerText = psInnerText
        End If

        If pbInsertAtFront AndAlso poParent.ChildNodes.Count > 0 Then
            poParent.InsertBefore(oXmlEl, poParent.ChildNodes(0))
        Else
            poParent.AppendChild(oXmlEl)
        End If

        Return oXmlEl
    End Function

    Public Shared Function CopyXMLElement(ByVal poOriginal As XmlElement, ByVal poCopyTo As XmlElement) As XmlElement
        For Each oAttribute As System.Xml.XmlAttribute In poOriginal.Attributes
            poCopyTo.SetAttribute(oAttribute.Name, oAttribute.Value)
        Next

        poCopyTo.InnerText = poOriginal.InnerText

        Return poCopyTo
    End Function

End Class