Imports WebsiteLibrary.CXMLUtils
Imports WebsiteLibrary.CSBCStd
Imports System.Web.HttpUtility
Imports System.Xml

Public Class CAddressParser


    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    'initialize to the default api key
    Private _GoogleAPIKey As String = "ABQIAAAAKOFsR5G0LhSThX4nKEEpshSBlHYHSS9DLYMHonTGuor0AH26JRRzwKJKTEPW4Rw0jQC733ZsXJ4Wfw"

    Public Sub New()
        If Not System.Configuration.ConfigurationManager.AppSettings("GOOGLE_MAP_API_KEY") Is Nothing Then
            _GoogleAPIKey = System.Configuration.ConfigurationManager.AppSettings("GOOGLE_MAP_API_KEY")
        End If
    End Sub

    ''' <summary>
    ''' Returns a CAddress result from the google geocode service. If the function fails, a NULL object is returned.
    ''' </summary>
    'Public Function ParseAddress(ByVal psAddress As String) As CAddress
    '    Try
    '        Dim oURL As New System.Text.StringBuilder()
    '        oURL.Append("http://maps.google.com/maps/geo")
    '        oURL.Append("?q=" & UrlEncode(psAddress))
    '        oURL.Append("&key=" & _GoogleAPIKey)
    '        oURL.Append("&sensor=false")
    '        oURL.Append("&output=xml")

    '        LogDebug(log, "Submitting to google geocode for address (" & psAddress & "): " & oURL.ToString)

    '        Dim sResponse As String = WebsiteLibrary.CSBCStd.webGet(oURL.ToString)

    '        LogDebug(log, "Google geocode result for address (" & psAddress & "): " & sResponse)

    '        Dim oXml As New XmlDocument()
    '        oXml.LoadXml(sResponse)

    '        'XPATH does NOT support default namespaces so we have to specify some kind of prefix to use
    '        Dim oNSManager As New XmlNamespaceManager(oXml.NameTable)
    '        oNSManager.AddNamespace("default", "http://earth.google.com/kml/2.0")
    '        oNSManager.AddNamespace("addr", "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")

    '        'just use the FIRST placemark that's returned
    '        Dim oPlaceMark As XmlElement = CType(oXml.DocumentElement.SelectSingleNode("default:Response/default:Placemark", oNSManager), XmlElement)
    '        If oPlaceMark Is Nothing Then
    '            Throw New Exception("Google Geocode could not find any placemarks for address: " & psAddress)
    '        End If

    '        Dim oAddress As New CAddress()
    '        oAddress.StreetAddress = GetChildValue(oPlaceMark, "addr:AddressDetails/addr:Country/addr:AdministrativeArea/addr:SubAdministrativeArea/addr:Locality/addr:Thoroughfare/addr:ThoroughfareName", oNSManager)
    '        oAddress.City = GetChildValue(oPlaceMark, "addr:AddressDetails/addr:Country/addr:AdministrativeArea/addr:SubAdministrativeArea/addr:Locality/addr:LocalityName", oNSManager)
    '        oAddress.State = GetChildValue(oPlaceMark, "addr:AddressDetails/addr:Country/addr:AdministrativeArea/addr:AdministrativeAreaName", oNSManager)
    '        oAddress.Zip = GetChildValue(oPlaceMark, "addr:AddressDetails/addr:Country/addr:AdministrativeArea/addr:SubAdministrativeArea/addr:Locality/addr:PostalCode/addr:PostalCodeNumber", oNSManager)
    '        oAddress.County = GetChildValue(oPlaceMark, "addr:AddressDetails/addr:Country/addr:AdministrativeArea/addr:SubAdministrativeArea/addr:SubAdministrativeAreaName", oNSManager)
    '        Return oAddress
    '    Catch err As Exception
    '        LogError(log, "Unable to parse address via google geocode", err)
    '    End Try

    '    Return Nothing
    'End Function

End Class

Public Class CAddress

    Public StreetAddress As String
    Public City As String
    Public State As String
    Public Zip As String
    Public County As String

End Class