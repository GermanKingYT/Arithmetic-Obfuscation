Public Class Value
    Public Property Number As Double
    Public Property Expression As String
    Sub New(number As Double)
        Me.Number = number
        Me.Expression = number.ToString.Replace(",", ".")
    End Sub
    Sub New(number As Double, Expression As String)
        Me.Number = number
        Me.Expression = Expression.Replace(",", ".")
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}", Me.Number)
    End Function
End Class