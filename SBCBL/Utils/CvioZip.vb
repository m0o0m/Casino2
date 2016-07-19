Imports System.IO
Imports Zip = ICSharpCode.SharpZipLib.Zip.Compression

'//--Download ICSharpCode.SharpZipLib from
'//--http://www.icsharpcode.net/OpenSource/SharpZipLib/Download.aspx 
Public Class CvioZip
    Shared Function Compress(ByVal bytes() As Byte) As Byte()
        Dim memory As New MemoryStream()
        Dim stream As New Zip.Streams.DeflaterOutputStream(memory, New Zip.Deflater(Zip.Deflater.BEST_COMPRESSION), 131072)
        stream.Write(bytes, 0, bytes.Length)
        stream.Close()
        Return memory.ToArray()
    End Function

    Shared Function Decompress(ByVal bytes() As Byte) As Byte()
        Dim stream As New Zip.Streams.InflaterInputStream(New MemoryStream(bytes))
        Dim memory As New MemoryStream()
        Dim writeData(4096) As Byte
        Dim size As Integer

        While True
            size = stream.Read(writeData, 0, writeData.Length)
            If size > 0 Then
                memory.Write(writeData, 0, size)
            Else
                Exit While
            End If
        End While

        stream.Close()
        Return memory.ToArray()
    End Function
End Class