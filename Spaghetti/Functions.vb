Public Class Functions
    Public Property Format As String
    Public Property Method As Func(Of Double, Double, Double)
    Sub New(Format As String, Method As Func(Of Double, Double, Double))
        Me.Format = Format
        Me.Method = Method
    End Sub
    Public Function GetResult(x As Double, y As Double) As Double
        Return Me.Method.Invoke(x, y)
    End Function
    Public Function GetFormat() As String
        Return Me.Format
    End Function
    Public Overrides Function ToString() As String
        Return Me.Format
    End Function
End Class