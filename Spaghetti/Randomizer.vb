Imports Spaghetti.Tasks

Public Class Randomizer
    Public Shared Property Instance As Random = New Random(Environment.TickCount)
    Public Shared Function Int(min As Integer, max As Integer) As Integer
        Return Randomizer.Instance.Next(min, max)
    End Function
    Public Shared Function Float() As Double
        Return Randomizer.Instance.NextDouble
    End Function
    Public Shared Function Shuffle(min As Integer, max As Integer, len As Integer) As Double()
        Dim buffer As New List(Of Double)
        For i As Integer = 1 To len
            buffer.Add(Randomizer.Int(min, max))
        Next
        Return buffer.ToArray
    End Function
    Public Shared Function ShuffleF(min As Integer, max As Integer, len As Integer, Optional factor As Double = 1.0R) As Double()
        Dim buffer As New List(Of Double), value As Double = 0
        For i As Integer = 1 To len
            If (factor = 1.0R) Then
                value = Randomizer.Int(min, max) + Math.Round(Randomizer.Float, 2)
            Else
                value = Randomizer.Int(min, max) + Math.Round(Randomizer.Float, 2) / factor
            End If
            buffer.Add(value)
        Next
        Return buffer.ToArray
    End Function
    Public Shared Function Name(len As Integer, Optional prefix As String = "") As String
        Dim chars As Char() = New Char(len - 1) {}
        For i As Integer = 0 To len - 1
            chars(i) = Settings.Chars(Randomizer.Int(0, Settings.Chars.Length))
        Next
        Return String.Format("{0}{1}", prefix, New String(chars))
    End Function
End Class
