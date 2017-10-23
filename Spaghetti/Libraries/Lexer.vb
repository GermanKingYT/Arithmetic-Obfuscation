Imports System.IO
Imports System.Text.RegularExpressions
Imports Spaghetti.Tasks

Namespace Libraries
    Public Class Lexer
        Inherits Task
        Sub New(Parent As Project)
            MyBase.New(Parent, True)
        End Sub
        Public Function GetStream(stream As StreamReader, ParamArray ignore As Types()) As List(Of Token)
            Try
                If (stream.BaseStream.Length > 0) Then
                    Dim tokens As New List(Of Token), match As Match, index As Integer = 0
                    Dim context As String = stream.ReadToEnd, len As Integer = context.Length
                    Do
                        Dim found As Boolean = False
                        For Each Rule As KeyValuePair(Of Types, String) In Me.Parent.Definitions
                            match = Me.Match(Rule.Key, context)
                            If (match.Success) Then
                                context = context.Remove(match.Index, match.Length)
                                If (Not ignore.Contains(Rule.Key)) Then
                                    tokens.Add(New Token(match.Value, Rule.Key, index, match.Length))
                                End If
                                found = True
                                index += match.Length
                            End If
                            If (found) Then Exit For
                        Next
                        If (Not found) Then
                            Throw New Exception(String.Format("Unexpected symbol '{0}'", context(0)))
                        End If
                    Loop Until index = len
                    tokens.Add(Token.Create(Types.EOF, String.Empty, index))
                    Return tokens
                Else
                    Throw New Exception("Stream contains no data")
                    Return Nothing
                End If
            Finally
                stream.Dispose()
            End Try
        End Function
        Private Function Match(Types As Types, Str As String) As Match
            Return New Regex(Me.Parent.Definitions(Types), RegexOptions.Singleline Or RegexOptions.IgnoreCase).Match(Str)
        End Function
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Lexer"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Lexer"
            End Get
        End Property
    End Class
End Namespace