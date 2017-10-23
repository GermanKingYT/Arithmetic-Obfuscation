Module Module1
    Public ValueE As Integer = 1000
    Sub Main()
        Console.WriteLine("Value A: {0}", ValueA)
        Console.WriteLine("Value B: {0}", ValueB)
        Console.WriteLine("Value C: {0}", ValueC)
        Console.Read()
    End Sub
    Public Function ValueA() As Int16
        Return 10 + ValueD
    End Function
    Public Function ValueB() As Int32
        Return 200
    End Function
    Public Function ValueC() As Int64
        Return 2000 + ValueE
    End Function
    Public ReadOnly Property ValueD As Integer
        Get
            Return 100 - 10
        End Get
    End Property
End Module
