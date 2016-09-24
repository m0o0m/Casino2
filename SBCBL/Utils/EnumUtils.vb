Imports System.Reflection
Imports System.ComponentModel

Public Class EnumUtils

    Public Shared Function GetEnumDescription(e As [Enum]) As String

        Dim t As Type = e.GetType()

        Dim desAttrs = t.GetField([Enum].GetName(t, e)).GetCustomAttributes(GetType(DescriptionAttribute), True)

        If desAttrs IsNot Nothing AndAlso desAttrs.Length > 0 Then
            Dim attr = CType(desAttrs(0), DescriptionAttribute)
            If attr IsNot Nothing Then
                Return attr.Description
            End If
        End If

        Return e.ToString

    End Function

    Public Shared Function GetEnumDescriptions(Of EType)() As List(Of CNameValue)
        Dim n As Integer = 0
        ' get values to poll
        Dim enumValues As Array = [Enum].GetValues(GetType(EType))
        ' storage for the result
        'Dim Descr(enumValues.Length - 1) As String
        Dim ENVs = New List(Of CNameValue)
        ' get description or text for each value
        For Each value As [Enum] In enumValues
            'Descr(n) = GetEnumDescription(value)
            ENVs.Add(New CNameValue With {.Name = GetEnumDescription(value), .Value = Convert.ToInt32(value).ToString()})
            n += 1
        Next

        'Return Descr
        Return ENVs
    End Function

    Public Shared Function GetEnumInt(Of EType)(enumVal As EType) As Integer
        Return Convert.ToInt32(enumVal)
    End Function

    Public Shared Function ParseEnum(Of EType)(value As String) As EType
        Return DirectCast([Enum].Parse(GetType(EType), value), EType)
    End Function

    Public Shared Function ParseEnum(Of EType)(value As Integer) As EType
        Return DirectCast([Enum].ToObject(GetType(EType), value), EType)
    End Function

End Class


Public Class CNameValue

    Private _name As String
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _value As String
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

    Private _description As String
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

End Class
