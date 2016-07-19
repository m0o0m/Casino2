Imports MemcachedProviders.Cache
Imports Enyim.Caching

Namespace CacheUtils
    Public Class CMemcachedManager

        Public Function Add(ByVal key As String, ByVal objValue As Object, ByVal cacheTime As Integer) As Boolean
            If Not CEnyimMemcachedClient.Memcached.Store(Memcached.StoreMode.Set, formatKey(key), objValue, New TimeSpan(0, cacheTime, 0)) Then
                CEnyimMemcachedClient.ResetMemcached()
                Return CEnyimMemcachedClient.Memcached.Store(Memcached.StoreMode.Set, formatKey(key), objValue, New TimeSpan(0, cacheTime, 0))
            End If

            Return True
        End Function

        Public Function Add(ByVal key As String, ByVal objValue As Object, ByVal timeSpan As TimeSpan) As Boolean
            If Not CEnyimMemcachedClient.Memcached.Store(Memcached.StoreMode.Set, formatKey(key), objValue, timeSpan) Then
                CEnyimMemcachedClient.ResetMemcached()
                Return CEnyimMemcachedClient.Memcached.Store(Memcached.StoreMode.Set, formatKey(key), objValue, timeSpan)
            End If

            Return True
        End Function

        Public Function [Get](ByVal key As String) As Object
            Return CEnyimMemcachedClient.Memcached.Get(formatKey(key))
        End Function

        Public Function [Get](Of T)(ByVal key As String) As T
            Return CType(CEnyimMemcachedClient.Memcached.Get(formatKey(key)), T)
        End Function

        Public Function [Get](ByVal ParamArray keys() As String) As System.Collections.Generic.IDictionary(Of String, Object)
            Return CEnyimMemcachedClient.Memcached.Get(formatKey(keys))
        End Function

        Public Function Increment(ByVal key As String, ByVal amount As UInteger) As Long
            Return CEnyimMemcachedClient.Memcached.Increment(formatKey(key), amount)
        End Function

        Public Function Decrement(ByVal key As String, ByVal amount As UInteger) As Long
            Return CEnyimMemcachedClient.Memcached.Decrement(formatKey(key), amount)
        End Function

        Public Function Remove(ByVal key As String) As Boolean
            Return CEnyimMemcachedClient.Memcached.Remove(formatKey(key))
        End Function

        Public Sub RemoveAll()
            CEnyimMemcachedClient.Memcached.FlushAll()
        End Sub

        ' Since memcache doesn't allow space inside a key, we have
        ' to replace it with _ symbol ourselves
        Private Function formatKey(ByVal key As String) As String
            Return Replace(key, " ", "_")
        End Function

        Private Function formatKey(ByVal key() As String) As String()
            Return (From sKey As String In key Select formatKey(sKey)).ToArray
        End Function

    End Class

    Public Class CEnyimMemcachedClient

        Private Shared _Memcached As Enyim.Caching.MemcachedClient = Nothing

        Public Shared ReadOnly Property Memcached() As Enyim.Caching.MemcachedClient
            Get
                If _Memcached Is Nothing Then
                    _Memcached = New Enyim.Caching.MemcachedClient("enyim.com/memcached")
                End If

                Return _Memcached
            End Get
        End Property

        Public Shared Sub ResetMemcached()
            _Memcached = Nothing
        End Sub

    End Class
End Namespace
