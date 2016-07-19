Imports System.Data
Imports System.Collections
imports System
imports System.Collections.Generic
imports System.Text

Namespace Utils

    Public Class CZipCodeLookup

        Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

        Public Function GetZipData(ByVal psZip As String) As CZipData
            'psZip = psZip.Trim()
            'If psZip.Length > 5 Then psZip = Left(psZip, 5)

            'If psZip.Length = 5 AndAlso IsNumeric(psZip) Then
            '    Try
            '        log.Debug("Looking up zip: " & psZip)

            '        Dim oLookup As New bin.zipcodes.lookupZip()
            '        Dim oResult As Bin.zipcodes.CLookupResult = oLookup.GetZipData(psZip)

            '        Dim oData As New CZipData()

            '        oData.City = oResult.City
            '        oData.County = oResult.County
            '        oData.State = oResult.StateAbbrev

            '        Return oData
            '    Catch err As Exception
            '        log.Error(err)

            '    End Try

            'End If

            Return Nothing

        End Function

    End Class

    Public Class CZipData

        Public City As String
        Public State As String
        Public County As String

    End Class

End Namespace