Imports System.Text.RegularExpressions
Imports Spaghetti.Tasks

Namespace Libraries
    Public Class Syntax
        Inherits Task
        Public Property Index As Integer
        Public Property Stream As List(Of Token)
        Sub New(Parent As Project)
            MyBase.New(Parent, True)
        End Sub
        Public Function Parse(stream As List(Of Token)) As List(Of Expressions.Base)
            Dim expressions As New List(Of Expressions.Base)
            Me.Index = 0
            Me.Stream = stream
            Do
                expressions.Add(Me.Parse)
            Loop Until Me.Peek.Type = Types.EOF
            Return expressions
        End Function
        Private Function Peek() As Token
            If (Me.Index <= Me.Stream.Count - 1) Then
                Return Me.Stream(Me.Index)
            End If
            Return Token.Create(Types.EOF)
        End Function
        Private Function [Next]() As Token
            If (Me.Index <= Me.Stream.Count - 1) Then
                Me.Index += 1
            End If
            Return Me.Peek
        End Function
        Private Sub Match(Expect As Types)
            If (Me.Peek.Type = Expect) Then
                Me.Next()
                Return
            End If
            Throw New Exception(String.Format("Expecting token '{0}' at index {1}", Expect, Me.Index))
        End Sub
        Private Function Parse() As Expressions.Base
            Return Me.AdditionOrSubtraction
        End Function
        Private Function AdditionOrSubtraction() As Expressions.Base
            Dim e As Expressions.Base = Me.MultiplicationOrDivision()
            While (Me.Peek.Type = Types.Plus) OrElse
                  (Me.Peek.Type = Types.Minus)
                Dim Type As Types = Me.Peek.Type
                Me.Next()
                e = New Expressions.Binary(e, Type, Me.MultiplicationOrDivision)
            End While
            Return e
        End Function
        Private Function MultiplicationOrDivision() As Expressions.Base
            Dim e As Expressions.Base = Me.Squared()
            While (Me.Peek.Type = Types.Mult) OrElse
                  (Me.Peek.Type = Types.Div)
                Dim Type As Types = Me.Peek.Type
                Me.Next()
                e = New Expressions.Binary(e, Type, Me.Squared)
            End While
            Return e
        End Function
        Private Function Squared() As Expressions.Base
            Dim e As Expressions.Base = Me.Sqrt()
            While (Me.Peek.Type = Types.Squared)
                Me.Match(Types.Squared)
                Me.Match(Types.Open)
                e = New Expressions.Squared(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Sqrt() As Expressions.Base
            Dim e As Expressions.Base = Me.Sin()
            While (Me.Peek.Type = Types.Sqrt)
                Me.Match(Types.Sqrt)
                Me.Match(Types.Open)
                e = New Expressions.Sqrt(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Sin() As Expressions.Base
            Dim e As Expressions.Base = Me.Tan()
            While (Me.Peek.Type = Types.Sin)
                Me.Match(Types.Sin)
                Me.Match(Types.Open)
                e = New Expressions.Sin(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Tan() As Expressions.Base
            Dim e As Expressions.Base = Me.Cos()
            While (Me.Peek.Type = Types.Tan)
                Me.Match(Types.Tan)
                Me.Match(Types.Open)
                e = New Expressions.Tan(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Cos() As Expressions.Base
            Dim e As Expressions.Base = Me.Round()
            While (Me.Peek.Type = Types.Cos)
                Me.Match(Types.Cos)
                Me.Match(Types.Open)
                e = New Expressions.Cos(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Round() As Expressions.Base
            Dim e As Expressions.Base = Me.Log()
            While (Me.Peek.Type = Types.Round)
                Me.Match(Types.Round)
                Me.Match(Types.Open)
                e = New Expressions.Round(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Log() As Expressions.Base
            Dim e As Expressions.Base = Me.Factor()
            While (Me.Peek.Type = Types.Log)
                Me.Match(Types.Log)
                Me.Match(Types.Open)
                e = New Expressions.Log(Me.Parse)
                Me.Match(Types.Close)
            End While
            Return e
        End Function
        Private Function Factor() As Expressions.Base
            Dim e As Expressions.Base = Nothing
            If (Me.Peek.Type = Types.Number Or
                Me.Peek.Type = Types.Plus) Then
                e = Me.ParseNumber(False)
            ElseIf (Me.Peek.Type = Types.Minus) Then
                e = Me.ParseNumber(True)
            ElseIf (Me.Peek.Type = Types.Open) Then
                e = Me.ParseParentheses
            End If
            Return e
        End Function
        Private Function ParseParentheses() As Expressions.Base
            Me.Match(Types.Open)
            Dim e As Expressions.Base = Me.Parse
            Me.Match(Types.Close)
            Return e
        End Function
        Private Function ParseNumber(Optional Sign As Boolean = False) As Expressions.Base
            Dim value As String = String.Empty
            If (Sign) Then
                Me.Match(Types.Minus)
                value = Me.Peek.Value
                Me.Next()
                Return New Expressions.Float(Double.Parse(value, Settings.Numbers, Me.Parent.Culture) * -1.0R)
            End If
            value = Me.Peek.Value
            Me.Next()
            Return New Expressions.Float(Double.Parse(value, Settings.Numbers, Me.Parent.Culture))
        End Function
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Syntax"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Abstract Syntax Tree"
            End Get
        End Property
    End Class
End Namespace