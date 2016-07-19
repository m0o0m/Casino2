Imports System.Reflection

Namespace Utils

    Public Enum SortCommand
        Ascending
        Descending
    End Enum

    Public Enum SortDataType
        _String = 1
        _Double = 2
        _Integer = 3
        _DateTime = 4
    End Enum

    Public Class CPropertiesSorter(Of T)
        Implements IComparer(Of T)

        Private _SortDataType As String = ""
        Private _SortQuery As String = ""

        ''' <summary>
        ''' The sort string used to perform the sort. Can sort on multiple fields.
        ''' Use the property names of the class and basic SQL Syntax.
        ''' 
        ''' Example: "LastName DESC, FirstName"
        ''' </summary>
        Public ReadOnly Property SortQuery() As String
            Get
                Return Me._SortQuery.Trim(New Char() {" "c, ","c})
            End Get
        End Property

        Private ReadOnly Property SortDataTypes() As String
            Get
                Return Me._SortDataType.Trim(New Char() {" "c, ","c})
            End Get
        End Property

        Public Sub AddSort(ByVal psPropertyName As String, Optional ByVal poSortDataType As SortDataType = SortDataType._String, Optional ByVal poSortCommand As SortCommand = SortCommand.Ascending)
            Me._SortQuery &= IIf(Me._SortQuery <> "", ", ", "").ToString() & psPropertyName.Trim() & IIf(poSortCommand = SortCommand.Ascending, " ASC", " DESC").ToString()
            Me._SortDataType &= IIf(Me._SortDataType <> "", ", ", "").ToString() & poSortDataType
        End Sub

        Public Sub ResetSort()
            Me._SortQuery = ""
            Me._SortDataType = ""
        End Sub

        ''' <summary>
        ''' This is an implementation of IComparer(Of T).Compare
        ''' Can sort on multiple fields, or just one.
        ''' </summary>
        Public Function Compare(ByVal poT1 As T, ByVal poT2 As T) As Integer Implements IComparer(Of T).Compare
            If Me.SortQuery <> "" Then
                Dim oT_Type As Type = GetType(T)
                Dim oT_Info As PropertyInfo = Nothing

                Dim oFieldSorts() As String = Me.SortQuery.Split(","c)
                Dim oSortDataTypes() As String = Me.SortDataTypes.Split(","c)


                For nIndex As Int32 = 0 To oFieldSorts.Length - 1
                    Dim sFieldSort As String = oFieldSorts(nIndex)
                    Dim oSortDataType As SortDataType = CType([Enum].Parse(GetType(SortDataType), ToStr(oSortDataTypes(nIndex))), SortDataType)
                    Dim bIsDescending As Boolean = False

                    Dim sField As String = ToStr(sFieldSort.Replace(" ASC", ""))
                    If sFieldSort.EndsWith(" DESC") Then
                        sField = ToStr(sFieldSort.Replace(" DESC", ""))
                        bIsDescending = True
                    End If

                    oT_Info = oT_Type.GetProperty(sField)
                    If oT_Info Is Nothing Then
                        Throw New MissingFieldException(String.Format("The property ""{0}"" does not exist in type ""{1}""", sField, oT_Type.ToString()))
                    End If

                    Dim nCompare As Integer = CompareByDataType(oSortDataType, oT_Info.GetValue(poT1, Nothing), oT_Info.GetValue(poT2, Nothing))
                    If nCompare <> 0 Then
                        If bIsDescending Then
                            Return nCompare * -1
                        End If

                        Return nCompare
                    End If
                Next
            End If

            Return 0
        End Function

        Private Function CompareByDataType(ByVal poSortDataType As SortDataType, ByVal poValue1 As Object, ByVal poValue2 As Object) As Integer
            Select Case poSortDataType
                Case SortDataType._Double
                    Return ToDouble(poValue1).CompareTo(ToDouble(poValue2))

                Case SortDataType._Integer
                    Return ToInt32(poValue1).CompareTo(ToInt32(poValue2))

                Case SortDataType._DateTime
                    Return ToDate(poValue1).CompareTo(ToDate(poValue2))

            End Select

            Return ToStr(poValue1).CompareTo(ToStr(poValue2))
        End Function

#Region "Convert"

        Public Function ToDate(ByVal source As Object) As Date
            If IsDate(source) Then
                Return CDate(source)
            End If

            Return Date.MinValue
        End Function

        Public Function ToStr(ByVal source As Object) As String
            If source Is Nothing OrElse TypeOf source Is DBNull Then
                Return ""
            ElseIf TypeOf source Is Date AndAlso CDate(source) = Date.MinValue Then
                Return ""
            End If

            Return Trim(CStr(source))
        End Function

        Public Function ToInt32(ByVal source As Object) As Int32
            Dim s2 As String = ToStr(source)

            If s2 = "" Then
                Return 0
            ElseIf IsNumeric(s2) Then
                Return CInt(s2)
            End If

            Return 0
        End Function

        Public Function ToDouble(ByVal source As Object) As Double
            Dim s2 As String = ToStr(source)

            If IsNumeric(s2) Then
                Return CDbl(s2)
            End If

            Return 0
        End Function

#End Region

    End Class

End Namespace

