Public Class Expressions
    Public MustInherit Class Base
    End Class
    Public Class Float
        Inherits Base
        Public Property Value As Double
        Sub New(Value As Double)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return Me.Value.ToString
        End Function
    End Class
    Public Class Squared
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("squared({0})", Me.Value)
        End Function
    End Class
    Public Class Sqrt
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("sqrt({0})", Me.Value)
        End Function
    End Class
    Public Class Sin
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("sin({0})", Me.Value)
        End Function
    End Class
    Public Class Tan
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("tan({0})", Me.Value)
        End Function
    End Class
    Public Class Cos
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("cos({0})", Me.Value)
        End Function
    End Class
    Public Class Round
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("round({0})", Me.Value)
        End Function
    End Class
    Public Class Log
        Inherits Base
        Public Property Value As Base
        Sub New(Value As Base)
            Me.Value = Value
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("log({0})", Me.Value)
        End Function
    End Class
    Public Class Binary
        Inherits Base
        Public Property Left As Base
        Public Property Right As Base
        Public Property Type As Types
        Sub New(Left As Base, Type As Types, Right As Base)
            Me.Left = Left
            Me.Type = Type
            Me.Right = Right
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("({0} {1} {2})", Me.Left.ToString, Me.Type, Me.Right.ToString)
        End Function
    End Class
End Class
