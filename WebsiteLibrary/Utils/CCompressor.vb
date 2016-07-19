Imports System.IO
Imports System.IO.Compression

Public Class CCompressor

    Public Shared Function Compress(ByVal poData As Byte()) As Byte()
        Dim oOutput As New MemoryStream()
        Dim oZip As New GZipStream(oOutput, CompressionMode.Compress, True)
        oZip.Write(poData, 0, poData.Length)
        oZip.Close()

        Return oOutput.ToArray()
    End Function

    Public Shared Function Decompress(ByVal poData As Byte()) As Byte()
        Dim oInput As New MemoryStream()
        oInput.Write(poData, 0, poData.Length)
        oInput.Position = 0
        Dim oZip As New GZipStream(oInput, CompressionMode.Decompress, True)

        Dim oOutput As New MemoryStream()
        Dim oBuff(64) As Byte
        Dim nRead As Integer = -1
        nRead = oZip.Read(oBuff, 0, oBuff.Length)

        While (nRead > 0)
            oOutput.Write(oBuff, 0, nRead)
            nRead = oZip.Read(oBuff, 0, oBuff.Length)
        End While

        oZip.Close()
        Return oOutput.ToArray()
    End Function

End Class