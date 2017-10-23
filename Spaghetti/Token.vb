Public NotInheritable Class Token
    Public Property Index As Integer
    Public Property Length As Integer
    Public Property Value As String
    Public Property Type As Types
    Sub New(Value As String, Type As Types)
        Me.Type = Type
        Me.Value = Value
    End Sub
    Sub New(Value As String, Type As Types, Index As Integer, Length As Integer)
        Me.Type = Type
        Me.Value = Value
        Me.Index = Index
        Me.Length = Length
    End Sub
    Public Shared Function Create(Type As Types) As Token
        Return Token.Create(Type, String.Empty)
    End Function
    Public Shared Function Create(Type As Types, value As String) As Token
        Return Token.Create(Type, value)
    End Function
    Public Shared Function Create(Type As Types, value As String, index As Integer) As Token
        Return New Token(value, Type, index, 0)
    End Function
    Public Shared Function Create(Type As Types, value As String, index As Integer, Len As Integer) As Token
        Return New Token(value, Type, index, Len)
    End Function
    Public Overrides Function ToString() As String
        Return String.Format("{0}", Me.Type)
    End Function
End Class