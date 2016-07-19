Option Strict On
Imports System.Collections
Public Class CMathEngine

    Private log As log4net.ILog = log4net.LogManager.GetLogger(Me.GetType())

    Private Class mcSymbol
        Implements IComparer

        Public Token As String
        Public Cls As CMathEngine.TOKENCLASS
        Public PrecedenceLevel As PRECEDENCE
        Public tag As String

        Public Delegate Function compare_function(ByVal x As Object, ByVal y As Object) As Integer



        Public Overridable Overloads Function compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            Dim asym, bsym As mcSymbol
            asym = CType(x, mcSymbol)
            bsym = CType(y, mcSymbol)


            If asym.Token > bsym.Token Then Return 1

            If asym.Token < bsym.Token Then Return -1

            If asym.PrecedenceLevel = -1 Or bsym.PrecedenceLevel = -1 Then Return 0

            If asym.PrecedenceLevel > bsym.PrecedenceLevel Then Return 1

            If asym.PrecedenceLevel < bsym.PrecedenceLevel Then Return -1

            Return 0

        End Function

    End Class

    Private Enum PRECEDENCE
        NONE = 0
        LEVEL0 = 1
        LEVEL1 = 2
        LEVEL2 = 3
        LEVEL3 = 4
        LEVEL4 = 5
        LEVEL5 = 6
    End Enum

    Private Enum TOKENCLASS
        KEYWORD = 1
        IDENTIFIER = 2
        NUMBER = 3
        [OPERATOR] = 4
        PUNCTUATION = 5
    End Enum

    Private m_tokens As Collection
    Private m_State(,) As Integer
    Private m_KeyWords() As String
    Private m_colstring As String
    Private Const ALPHA As String = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    Private Const DIGITS As String = "#0123456789"

    Private m_funcs() As String = {"sin", "cos", "tan", "arcsin", "arccos", _
                                 "arctan", "sqrt", "max", "min", "floor", _
                                 "ceiling", "log", "log10", _
                                 "ln", "round", "abs", "neg", "pos"}

    Private m_operators As ArrayList

    Private m_stack As New Stack()

    Private Sub init_operators()

        Dim op As mcSymbol

        m_operators = New ArrayList()

        op = New mcSymbol()
        op.Token = "-"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL1
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "+"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL1
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "*"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL2
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "/"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL2
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "\"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL2
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "%"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL2
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "^"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL3
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "!"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL5
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "&"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL5
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "-"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL4
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "+"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL4
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = "("
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL5
        m_operators.Add(op)

        op = New mcSymbol()
        op.Token = ")"
        op.Cls = TOKENCLASS.[OPERATOR]
        op.PrecedenceLevel = PRECEDENCE.LEVEL0
        m_operators.Add(op)

        m_operators.Sort(op)
    End Sub


    Public Function evaluate(ByVal expression As String) As Double

        Dim symbols As New Queue()


        Try
            If IsNumeric(expression) Then Return CType(expression, Double)

            calc_scan(expression, symbols)

            Return level0(symbols)

        Catch
            log.Error("Invalid expression: " & expression)
        End Try

    End Function

    Private Function calc_op(ByVal op As mcSymbol, ByVal operand1 As Double, Optional ByVal operand2 As Double = Nothing) As Double


        Select Case op.Token.ToLower

            Case "&" ' sample to show addition of custom operator
                Return 5

            Case "^"
                Return (operand1 ^ operand2)

            Case "+"

                Select Case op.PrecedenceLevel
                    Case PRECEDENCE.LEVEL1
                        Return (operand2 + operand1)
                    Case PRECEDENCE.LEVEL4
                        Return operand1
                End Select

            Case "-"
                Select Case op.PrecedenceLevel
                    Case PRECEDENCE.LEVEL1
                        Return (operand1 - operand2)
                    Case PRECEDENCE.LEVEL4
                        Return -1 * operand1
                End Select


            Case "*"
                Return (operand2 * operand1)

            Case "/"
                Return (operand1 / operand2)

            Case "\"
                Return (CLng(operand1) \ CLng(operand2))

            Case "%"
                Return (operand1 Mod operand2)

            Case "!"
                Dim i As Integer
                Dim res As Double = 1

                If operand1 > 1 Then
                    For i = CInt(operand1) To 1 Step -1
                        res = res * i
                    Next

                End If
                Return (res)

        End Select

    End Function

    Private Function calc_function(ByVal func As String, ByVal args As Collection) As Double

        Select Case func.ToLower

            Case "cos"
                Return (Math.Cos(CDbl(args(1))))

            Case "sin"
                Return (Math.Sin(CDbl(args(1))))

            Case "tan"
                Return (Math.Tan(CDbl(args(1))))

            Case "floor"
                Return (Math.Floor(CDbl(args(1))))

            Case "ceiling"
                Return (Math.Ceiling(CDbl(args(1))))

            Case "max"
                Return (Math.Max(CDbl(args(1)), CDbl(args(2))))

            Case "min"
                Return (Math.Min(CDbl(args(1)), CDbl(args(2))))

            Case "arcsin"
                Return (Math.Asin(CDbl(args(1))))


            Case "arccos"
                Return (Math.Acos(CDbl(args(1))))

            Case "arctan"
                Return (Math.Atan(CDbl(args(1))))


            Case "sqrt"
                Return (Math.Sqrt(CDbl(args(1))))

            Case "log"
                Return (Math.Log10(CDbl(args(1))))


            Case "log10"
                Return (Math.Log10(CDbl(args(1))))


            Case "abs"
                Return (Math.Abs(CDbl(args(1))))


            Case "round"
                Return (Math.Round(CDbl(args(1))))

            Case "ln"
                Return (Math.Log(CDbl(args(1))))

            Case "neg"
                Return (-1 * CDbl(args(1)))

            Case "pos"
                Return (+1 * CDbl(args(1)))

        End Select

    End Function

    Private Function identifier(ByVal token As String) As Double

        Select Case token.ToLower

            Case "e"
                Return Math.E
            Case "pi"
                Return Math.PI
            Case Else
                ' look in symbol table....?
        End Select
    End Function

    Private Function is_operator(ByVal token As String, Optional ByVal level As PRECEDENCE = CType(-1, PRECEDENCE), Optional ByRef [operator] As mcSymbol = Nothing) As Boolean

        Try
            Dim op As New mcSymbol()
            op.Token = token
            op.PrecedenceLevel = level
            op.tag = "test"

            Dim ir As Integer = m_operators.BinarySearch(op, op)

            If ir > -1 Then

                [operator] = CType(m_operators(ir), mcSymbol)
                Return True
            End If

            Return False

        Catch
            Return False
        End Try
    End Function

    Private Function is_function(ByVal token As String) As Boolean

        Try
            Dim lr As Integer = Array.BinarySearch(m_funcs, token.ToLower)

            Return (lr > -1)

        Catch
            Return False
        End Try

    End Function


    Public Function calc_scan(ByVal line As String, ByRef symbols As Queue) As Boolean

        Dim sp As Integer  ' start position marker
        Dim cp As Integer  ' current position marker
        Dim col As Integer ' input column
        Dim lex_state As Integer
        Dim cc As Char

        line = line & " " ' add a space as an end marker

        sp = 0
        cp = 0
        lex_state = 1


        Do While cp <= line.Length - 1

            cc = line.Chars(cp)

            ' if cc is not found then IndexOf returns -1 giving col = 2.
            col = m_colstring.IndexOf(cc) + 3

            ' set the input column 
            Select Case col

                Case 2 ' cc wasn't found in the column string

                    If ALPHA.IndexOf(Char.ToUpper(cc)) > 0 Then      ' letter column?
                        col = 1
                    ElseIf DIGITS.IndexOf(Char.ToUpper(cc)) > 0 Then ' number column?
                        col = 2
                    Else ' everything else is assigned to the punctuation column
                        col = 6
                    End If

                Case Is > 5 ' cc was found and is > 5 so must be in operator column
                    col = 7

                    ' case else ' cc was found - col contains the correct column

            End Select

            ' find the new state DSLd on current state and column (determined by input)
            lex_state = m_State(lex_state - 1, col - 1)

            Select Case lex_state

                Case 3 ' function or variable  end state 

                    ' TODO variables aren't supported but substitution 
                    '      could easily be performed here or after
                    '      tokenization

                    Dim sym As New mcSymbol()

                    sym.Token = line.Substring(sp, cp - sp)
                    If is_function(sym.Token) Then
                        sym.Cls = TOKENCLASS.KEYWORD
                    Else
                        sym.Cls = TOKENCLASS.IDENTIFIER
                    End If

                    symbols.Enqueue(sym)

                    lex_state = 1
                    cp = cp - 1

                Case 5 ' number end state
                    Dim sym As New mcSymbol()

                    sym.Token = line.Substring(sp, cp - sp)
                    sym.Cls = TOKENCLASS.NUMBER

                    symbols.Enqueue(sym)

                    lex_state = 1
                    cp = cp - 1

                Case 6 ' punctuation end state
                    Dim sym As New mcSymbol()

                    sym.Token = line.Substring(sp, cp - sp + 1)
                    sym.Cls = TOKENCLASS.PUNCTUATION

                    symbols.Enqueue(sym)

                    lex_state = 1

                Case 7 ' operator end state

                    Dim sym As New mcSymbol()

                    sym.Token = line.Substring(sp, cp - sp + 1)
                    sym.Cls = TOKENCLASS.[OPERATOR]

                    symbols.Enqueue(sym)

                    lex_state = 1

            End Select

            cp += 1
            If lex_state = 1 Then sp = cp

        Loop

        Return True

    End Function

    Private Sub init()

        Dim op As mcSymbol

        Dim state(,) As Integer = {{2, 4, 1, 1, 4, 6, 7}, _
                                   {2, 3, 3, 3, 3, 3, 3}, _
                                   {1, 1, 1, 1, 1, 1, 1}, _
                                   {2, 4, 5, 5, 4, 5, 5}, _
                                   {1, 1, 1, 1, 1, 1, 1}, _
                                   {1, 1, 1, 1, 1, 1, 1}, _
                                   {1, 1, 1, 1, 1, 1, 1}}

        init_operators()


        m_State = state
        'm_colstring = Chr(9) & " " & ".()"
        m_colstring = Chr(9) & " " & System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator & "()"
        For Each op In m_operators
            m_colstring = m_colstring & op.Token
        Next


        Array.Sort(m_funcs)
        m_tokens = New Collection()

    End Sub


    Public Sub New()

        init()

    End Sub

#Region "Recusrsive Descent Parsing Functions"



    Private Function level0(ByRef tokens As Queue) As Double

        Return level1(tokens)

    End Function


    Private Function level1_prime(ByRef tokens As Queue, ByVal result As Double) As Double

        Dim symbol As mcSymbol
        Dim [operator] As mcSymbol = Nothing

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Return result
        End If

        ' binary level1 precedence operators....+, -
        If is_operator(symbol.Token, PRECEDENCE.LEVEL1, [operator]) Then

            tokens.Dequeue()
            result = calc_op([operator], result, level2(tokens))
            result = level1_prime(tokens, result)

        End If


        Return result

    End Function

    Private Function level1(ByRef tokens As Queue) As Double

        Return level1_prime(tokens, level2(tokens))

    End Function

    Private Function level2(ByRef tokens As Queue) As Double

        Return level2_prime(tokens, level3(tokens))
    End Function

    Private Function level2_prime(ByRef tokens As Queue, ByVal result As Double) As Double

        Dim symbol As mcSymbol
        Dim [operator] As mcSymbol = Nothing

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Return result
        End If

        ' binary level2 precedence operators....*, /, \, %

        If is_operator(symbol.Token, PRECEDENCE.LEVEL2, [operator]) Then

            tokens.Dequeue()
            result = calc_op([operator], result, level3(tokens))
            result = level2_prime(tokens, result)

        End If

        Return result

    End Function

    Private Function level3(ByRef tokens As Queue) As Double

        Return level3_prime(tokens, level4(tokens))

    End Function

    Private Function level3_prime(ByRef tokens As Queue, ByVal result As Double) As Double

        Dim symbol As mcSymbol
        Dim [operator] As mcSymbol = Nothing

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Return result
        End If

        ' binary level3 precedence operators....^

        If is_operator(symbol.Token, PRECEDENCE.LEVEL3, [operator]) Then

            tokens.Dequeue()
            result = calc_op([operator], result, level4(tokens))
            result = level3_prime(tokens, result)

        End If


        Return result

    End Function

    Private Function level4(ByRef tokens As Queue) As Double

        Return level4_prime(tokens)
    End Function

    Private Function level4_prime(ByRef tokens As Queue) As Double

        Dim symbol As mcSymbol
        Dim [operator] As mcSymbol = Nothing

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Throw New System.Exception("Invalid expression.")
        End If

        ' unary level4 precedence right associative  operators.... +, -

        If is_operator(symbol.Token, PRECEDENCE.LEVEL4, [operator]) Then

            tokens.Dequeue()
            Return calc_op([operator], level5(tokens))
        Else
            Return level5(tokens)
        End If


    End Function

    Private Function level5(ByVal tokens As Queue) As Double

        Return level5_prime(tokens, level6(tokens))

    End Function

    Private Function level5_prime(ByVal tokens As Queue, ByVal result As Double) As Double

        Dim symbol As mcSymbol
        Dim [operator] As mcSymbol = Nothing

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Return result
        End If

        ' unary level5 precedence left associative operators.... !

        If is_operator(symbol.Token, PRECEDENCE.LEVEL5, [operator]) Then

            tokens.Dequeue()
            Return calc_op([operator], result)

        Else
            Return result
        End If

    End Function

    Private Function level6(ByRef tokens As Queue) As Double

        Dim symbol As mcSymbol

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Throw New System.Exception("Invalid expression.")
            Return 0
        End If

        Dim val As Double


        ' constants, identifiers, keywords, -> expressions
        If symbol.Token = "(" Then ' opening paren of new expression

            tokens.Dequeue()
            val = level0(tokens)

            symbol = CType(tokens.Dequeue, mcSymbol)
            ' closing paren
            If symbol.Token <> ")" Then Throw New System.Exception("Invalid expression.")

            Return val
        Else

            Select Case symbol.Cls

                Case TOKENCLASS.IDENTIFIER
                    tokens.Dequeue()
                    Return identifier(symbol.Token)

                Case TOKENCLASS.KEYWORD
                    tokens.Dequeue()
                    Return calc_function(symbol.Token, arguments(tokens))
                Case TOKENCLASS.NUMBER

                    tokens.Dequeue()
                    m_stack.Push(CDbl(symbol.Token))
                    Return CDbl(symbol.Token)

                Case Else
                    Throw New System.Exception("Invalid expression.")
            End Select
        End If


    End Function

    Private Function arguments(ByVal tokens As Queue) As Collection

        Dim symbol As mcSymbol
        Dim args As New Collection()

        If tokens.Count > 0 Then
            symbol = CType(tokens.Peek, mcSymbol)
        Else
            Throw New System.Exception("Invalid expression.")
            Return Nothing
        End If

        If symbol.Token = "(" Then

            tokens.Dequeue()
            args.Add(level0(tokens))

            symbol = CType(tokens.Dequeue, mcSymbol)
            Do While symbol.Token <> ")"

                If symbol.Token = "," Then
                    args.Add(level0(tokens))
                Else
                    Throw New System.Exception("Invalid expression.")
                    Return Nothing
                End If
                symbol = CType(tokens.Dequeue, mcSymbol)
            Loop

            Return args
        Else
            Throw New System.Exception("Invalid expression.")
            Return Nothing
        End If

    End Function

#End Region

End Class