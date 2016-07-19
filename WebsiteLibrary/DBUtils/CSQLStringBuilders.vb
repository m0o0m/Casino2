Imports System.Collections
Namespace DBUtils
    Public Interface ISQLEditStringBuilder
        ReadOnly Property TableName() As String
        ReadOnly Property Columns() As ArrayList
        ReadOnly Property Values() As ArrayList
        ReadOnly Property SQL() As String
        Sub AppendString(ByVal sField As String, ByVal sValue As String)
        Sub Reset(Optional ByVal psTableName As String = "")
    End Interface

    'helper function to buffer appendstrings and retrieve the final sql later
    Public Class CSQLEditStringBuilder
        Implements ISQLEditStringBuilder

        Private _sTableName As String
        Private _LstFields As New ArrayList()
        Private _LstValues As New ArrayList()
        Private _sCondition As String
        Private _sType As String

        Public Sub New(ByVal psTable As String, ByVal psType As String, Optional ByVal psConditionWithWhereClause As String = "")
            _sTableName = psTable
            _sType = psType
            _sCondition = psConditionWithWhereClause
        End Sub

        Public Sub AppendString(ByVal sField As String, ByVal sValue As String) Implements ISQLEditStringBuilder.AppendString
            _LstFields.Add(sField)
            _LstValues.Add(sValue)
        End Sub

        Private Function insertSQL() As String
            Dim oInsertBuilder As New CSQLInsertStringBuilder(TableName)
            Dim i As Integer
            For i = 0 To _LstFields.Count - 1
                oInsertBuilder.AppendString(CStr(_LstFields(i)), CStr(_LstValues(i)))
            Next

            Return oInsertBuilder.SQL
        End Function

        Private Function updateSQL() As String
            Dim oUpdateBuilder As New CSQLUpdateStringBuilder(TableName, Conditions)
            Dim i As Integer
            For i = 0 To _LstFields.Count - 1
                oUpdateBuilder.AppendString(CStr(_LstFields(i)), CStr(_LstValues(i)))
            Next

            Return oUpdateBuilder.SQL
        End Function

        Private Function deleteSQL() As String
            Return ""
        End Function

        Public Sub Reset(Optional ByVal psTableName As String = "") Implements ISQLEditStringBuilder.Reset
            _LstFields.Clear()
            _LstValues.Clear()

            If psTableName <> "" Then
                _sTableName = psTableName
            End If
        End Sub

        Public ReadOnly Property Columns() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Columns
            Get
                Return _LstFields
            End Get
        End Property

        Public ReadOnly Property Values() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Values
            Get
                Return _LstValues
            End Get
        End Property

        Public ReadOnly Property SQL() As String Implements ISQLEditStringBuilder.SQL
            Get
                Select Case UCase(Type)
                    Case "I"
                        Return insertSQL()
                    Case "U"
                        Return updateSQL()
                    Case Else
                        Return deleteSQL()
                End Select
            End Get
        End Property

        Public ReadOnly Property TableName() As String Implements ISQLEditStringBuilder.TableName
            Get
                Return _sTableName
            End Get
        End Property

        ''' <summary>
        ''' Set Type of SQL
        ''' I: Insert
        ''' U: Update
        ''' D: Delete
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type() As String
            Get
                Return _sType
            End Get
            Set(ByVal value As String)
                _sType = value
            End Set
        End Property

        Public Property Conditions() As String
            Get
                Return _sCondition
            End Get
            Set(ByVal value As String)
                _sCondition = value
            End Set
        End Property
    End Class

    ' this class assists in building the "(field1, field2, field3) values('a', 'b', 3)" part
    ' of an INSERT SQL statement
    Public Class CSQLInsertStringBuilder
        Implements ISQLEditStringBuilder

        Public Sub New(ByVal sTable As String)
            _sTableName = sTable
            _oFields = New CSQLCommaStringBuilder()
            _oValues = New CSQLCommaStringBuilder()
        End Sub

        Public Sub Reset(Optional ByVal psTableName As String = "") Implements ISQLEditStringBuilder.Reset
            _oFields.Reset()
            _oValues.Reset()

            If psTableName <> "" Then
                _sTableName = psTableName
            End If
        End Sub

        Public ReadOnly Property SQL() As String Implements ISQLEditStringBuilder.SQL
            Get
                Dim sSQL As String = ""
                If _sTableName <> String.Empty Then
                    sSQL = "Insert into " & _sTableName & " "
                End If
                sSQL += "(" & _oFields.SQL & ") VALUES(" & _oValues.SQL & ")"

                Return sSQL
            End Get
        End Property

        Public ReadOnly Property TableName() As String Implements ISQLEditStringBuilder.TableName
            Get
                Return _sTableName
            End Get
        End Property

        Public Sub AppendString(ByVal sField As String, ByVal sValue As String) Implements ISQLEditStringBuilder.AppendString
            For nIndex As Integer = 0 To _oFields.Columns.Count - 1
                If CSBCStd.SafeString(_oFields.Columns(nIndex)) = sField Then
                    _oValues.Columns(nIndex) = sValue
                    Exit Sub
                End If
            Next
            _oFields.AppendString(sField)
            _oValues.AppendString(sValue)
        End Sub

        Public ReadOnly Property Columns() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Columns
            Get
                Return _oFields.Columns
            End Get
        End Property

        Public ReadOnly Property Values() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Values
            Get
                Return _oValues.Columns
            End Get
        End Property

        Private _sTableName As String
        Private _oFields As CSQLCommaStringBuilder
        Private _oValues As CSQLCommaStringBuilder
    End Class

    ' this class assists in building the "SET field1='a', field2='b', field3='c'" part
    ' of an UPDATE SQL statement
    Public Class CSQLUpdateStringBuilder
        Implements ISQLEditStringBuilder

        Private _TopRecordsOnly As Integer = -1

        Public Sub New(ByVal sTableName As String, ByVal sConditionWithWhereClause As String, Optional ByVal nTopRecordsOnly As Integer = -1)
            _sTableName = sTableName
            _sCondition = sConditionWithWhereClause
            _oFields = New CSQLCommaStringBuilder()
            _oValues = New CSQLCommaStringBuilder()

            _TopRecordsOnly = nTopRecordsOnly
        End Sub

        Public ReadOnly Property SQL() As String Implements ISQLEditStringBuilder.SQL
            Get
                Dim sSQL As String = ""
                If _sTableName = String.Empty And _sCondition = String.Empty Then
                    Throw New Exception("SQL Update builder requires you to specify BOTH table and condition")
                End If

                If _TopRecordsOnly > 0 Then
                    sSQL = "UPDATE TOP(" & _TopRecordsOnly & ")" & _sTableName
                Else
                    sSQL = "UPDATE " & _sTableName
                End If

                Dim oLstTemp As New ArrayList()
                For i As Integer = 0 To Columns.Count - 1
                    oLstTemp.Add(CStr(Columns(i)) & " = " & CStr(Values(i)))
                Next

                sSQL += " SET " & Join(oLstTemp.ToArray(), ", ")

                If _sCondition <> String.Empty Then
                    sSQL += " " & _sCondition
                End If

                Return sSQL
            End Get
        End Property

        Public ReadOnly Property TableName() As String Implements ISQLEditStringBuilder.TableName
            Get
                Return _sTableName
            End Get
        End Property

        Public ReadOnly Property IsEmpty() As Boolean
            Get
                Return _oFields.IsEmpty
            End Get
        End Property

        'this only resets all the appends
        Public Sub Reset(Optional ByVal psTableName As String = "") Implements ISQLEditStringBuilder.Reset
            _oFields.Reset()
            _oValues.Reset()

            If psTableName <> "" Then
                _sTableName = psTableName
            End If
        End Sub

        Public Sub AppendString(ByVal sField As String, ByVal sValue As String) Implements ISQLEditStringBuilder.AppendString
            For nIndex As Integer = 0 To _oFields.Columns.Count - 1
                If CSBCStd.SafeString(_oFields.Columns(nIndex)) = sField Then
                    _oValues.Columns(nIndex) = sValue
                    Exit Sub
                End If
            Next
            _oFields.AppendString(sField)
            _oValues.AppendString(sValue)
        End Sub

        Public ReadOnly Property Columns() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Columns
            Get
                Return _oFields.Columns
            End Get
        End Property

        Public ReadOnly Property Values() As System.Collections.ArrayList Implements ISQLEditStringBuilder.Values
            Get
                Return _oValues.Columns
            End Get
        End Property

        Public Property Conditions() As String
            Get
                Return _sCondition
            End Get
            Set(ByVal value As String)
                _sCondition = value
            End Set
        End Property

        Private _sTableName As String
        Private _sCondition As String
        Private _oFields As CSQLCommaStringBuilder
        Private _oValues As CSQLCommaStringBuilder
    End Class

    Public Class CSQLDeleteStringBuilder
        Public Sub New(ByVal sTableName As String, ByVal sConditionWithWhereClause As String)
            _sTableName = sTableName
            _sCondition = sConditionWithWhereClause
        End Sub

        Public ReadOnly Property SQL() As String
            Get
                Dim sSQL As String = ""
                If _sTableName = String.Empty And _sCondition = String.Empty Then
                    Throw New Exception("SQL Delete builder requires you to specify BOTH table and condition")
                End If

                sSQL = "DELETE " & _sTableName

                If _sCondition <> String.Empty Then
                    sSQL += " " & _sCondition
                End If

                Return sSQL
            End Get
        End Property

        Public Property Conditions() As String
            Get
                Return _sCondition
            End Get
            Set(ByVal value As String)
                _sCondition = value
            End Set
        End Property

        Public Property TableName() As String
            Get
                Return _sTableName
            End Get
            Set(ByVal value As String)
                _sTableName = value
            End Set
        End Property

        Private _sTableName As String
        Private _sCondition As String
    End Class

    Public Class CSQLWhereStringBuilder
        Public ReadOnly Property SQL() As String
            Get
                If Not IsEmpty Then
                    Return " WHERE " & m_sSQL
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property IsEmpty() As Boolean
            Get
                Return m_sSQL = ""
            End Get
        End Property

        Public Sub Reset()
            m_sSQL = ""
        End Sub

        ' Append an optional condition to the SQL Where clause.  The optional condition is only applied
        ' if the optional value is not empty
        Public Sub AppendOptionalANDCondition(ByVal sField As String, ByVal sOptionalValue As String, ByVal sOperator As String)
            If sOptionalValue <> "" AndAlso sOptionalValue <> "''" AndAlso sOptionalValue <> "'%'" AndAlso sOptionalValue <> "'%%'" Then
                AppendANDCondition(sField & " " & sOperator & " " & sOptionalValue)
            End If
        End Sub

        Public Sub AppendDateEquals(ByVal psDateField As String, ByVal poDate As Date)
            If poDate = Date.MinValue Then
                Return
            End If

            Dim sCondition As String = "(" & psDateField & ">=" & CSBCStd.SQLString(poDate.ToShortDateString()) & " AND " & psDateField & "<" & CSBCStd.SQLString(poDate.AddDays(1).ToShortDateString()) & ")"
            If m_sSQL = "" Then
                m_sSQL = sCondition
            Else
                m_sSQL = m_sSQL & " AND " & sCondition
            End If
        End Sub

        Public Sub AppendANDCondition(ByVal sCondition As String)
            If sCondition = "" Then
                Return
            End If

            If m_sSQL = "" Then
                m_sSQL = sCondition
            Else
                m_sSQL = m_sSQL & " AND " & sCondition
            End If
        End Sub

        Private m_sSQL As String
    End Class

    Public Class CSQLCommaStringBuilder
        Public ReadOnly Property SQL() As String
            Get
                Return Join(_LstElem.ToArray(), ", ")
            End Get
        End Property

        Public ReadOnly Property IsEmpty() As Boolean
            Get
                Return _LstElem.Count = 0
            End Get
        End Property

        Public Sub Reset()
            _LstElem = New ArrayList()
        End Sub

        Public Sub AppendString(ByVal sElem As String)
            _LstElem.Add(sElem)
        End Sub

        Public ReadOnly Property Columns() As ArrayList
            Get
                Return _LstElem
            End Get
        End Property

        Private _LstElem As ArrayList = New ArrayList()
    End Class

End Namespace