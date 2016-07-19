Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Utils

    ''View at url: http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    Public Class CSerializedObjectCloner

        Public Shared Function Clone(Of T)(ByVal poRealObject As T) As T
            If Not GetType(T).IsSerializable Then
                Throw New ArgumentException("The type must be serializable.", "poRealObject")
            End If
            If Object.ReferenceEquals(poRealObject, Nothing) Then
                Return CType(Nothing, T)
            End If

            Using oStream As Stream = New MemoryStream
                Dim oFormatter As IFormatter = New BinaryFormatter
                oFormatter.Serialize(oStream, poRealObject)

                oStream.Seek(0, SeekOrigin.Begin)

                Return DirectCast(oFormatter.Deserialize(oStream), T)
            End Using
        End Function

    End Class

End Namespace

