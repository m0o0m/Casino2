Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Public Class CCasinoDES
    'this is our super duper encryption key! do not share these keys this with anyone!
    Private key() As Byte = {51, 32, 13, 74, 35, 76, 57, 48, 39, 110, 211, 112, 213, 65, 215, 67, 117, 98, 112, 120, 32, 110, 244, 121}
    Private iv() As Byte = {22, 11, 98, 50, 11, 67, 59, 110}

    Public Function Encrypt(ByVal plainText As String) As String
        ' Declare a UTF8Encoding object so we may use the GetByte
        ' method to transform the plainText into a Byte array.
        Dim utf8encoder As UTF8Encoding = New UTF8Encoding()
        Dim inputInBytes() As Byte = utf8encoder.GetBytes(plainText)

        ' Create a new TripleDES service provider
        Dim tdesProvider As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider()

        ' The ICryptTransform interface uses the TripleDES
        ' crypt provider along with encryption key and init vector
        ' information
        Dim cryptoTransform As ICryptoTransform = tdesProvider.CreateEncryptor(Me.key, Me.iv)

        ' All cryptographic functions need a stream to output the
        ' encrypted information. Here we declare a memory stream
        ' for this purpose.
        Dim encryptedStream As MemoryStream = New MemoryStream()
        Dim cryptStream As CryptoStream = New CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write)

        ' Write the encrypted information to the stream. Flush the information
        ' when done to ensure everything is out of the buffer.
        cryptStream.Write(inputInBytes, 0, inputInBytes.Length)
        cryptStream.FlushFinalBlock()
        encryptedStream.Position = 0

        ' Read the stream back into a Byte array and return it to the calling
        ' method.
        Dim result(CInt(encryptedStream.Length - 1)) As Byte
        encryptedStream.Read(result, 0, CInt(encryptedStream.Length))
        cryptStream.Close()

        Return System.Convert.ToBase64String(result)
    End Function

    Public Function Decrypt(ByVal psEncryptedString As String) As String
        ' UTFEncoding is used to transform the decrypted Byte Array
        ' information back into a string.
        Try


            If psEncryptedString <> "" Then
                Dim oInputInBytes As Byte() = System.Convert.FromBase64String(psEncryptedString)
                Dim utf8encoder As UTF8Encoding = New UTF8Encoding()
                Dim tdesProvider As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider()

                ' As before we must provide the encryption/decryption key along with
                ' the init vector.
                Dim cryptoTransform As ICryptoTransform = tdesProvider.CreateDecryptor(Me.key, Me.iv)

                ' Provide a memory stream to decrypt information into
                Dim decryptedStream As MemoryStream = New MemoryStream()
                Dim cryptStream As CryptoStream = New CryptoStream(decryptedStream, cryptoTransform, CryptoStreamMode.Write)
                cryptStream.Write(oInputInBytes, 0, oInputInBytes.Length)
                cryptStream.FlushFinalBlock()
                decryptedStream.Position = 0

                ' Read the memory stream and convert it back into a string
                Dim result(CInt(decryptedStream.Length) - 1) As Byte
                decryptedStream.Read(result, 0, CInt(decryptedStream.Length))
                cryptStream.Close()
                Dim myutf As UTF8Encoding = New UTF8Encoding()
                Return myutf.GetString(result)
            End If
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function
End Class
