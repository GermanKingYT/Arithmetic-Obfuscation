Imports System.IO
Imports System.Runtime.CompilerServices
Imports Mono.Cecil

Public Module Extensions
    <Extension> Public Function IsDesirable(t As TypeDefinition) As Boolean
        Return Not t.Name = "<Module>" AndAlso
               Not t.Name.StartsWith("<") AndAlso
               Not t.Name.Contains("__") AndAlso
               Not t.Name.Contains("`") AndAlso
               Not t.IsRuntimeSpecialName AndAlso
               Not t.IsSpecialName AndAlso
               Not t.FullName.Contains("My")
    End Function
    <Extension> Public Function IsDesirable(m As MethodDefinition) As Boolean
        Return Not m.IsRuntime AndAlso
               Not m.IsVirtual AndAlso
               Not m.IsAbstract AndAlso
               Not m.IsNative AndAlso
               Not m.IsPInvokeImpl AndAlso
               Not m.IsUnmanaged AndAlso
               Not m.IsAssembly AndAlso
               Not m.FullName.Contains("My") AndAlso
               Not m.Name.StartsWith("<") AndAlso
               Not m.Name.Contains("`") AndAlso
               Not m.Name.Contains("__")
    End Function
    <Extension> Public Function IsDesirable(p As PropertyDefinition) As Boolean
        Return Not p.IsSpecialName
    End Function
    <Extension> Public Function IsDesirable(f As FieldDefinition) As Boolean
        Return Not f.IsSpecialName AndAlso
               Not f.Name = "version" AndAlso
               Not f.Name = "collection" AndAlso
               Not f.Name = "next" AndAlso
               Not f.Name = "current" AndAlso
               Not f.Name = "container" AndAlso
               Not f.Name = "Array" AndAlso
               Not f.Name.StartsWith("<")
    End Function
    <Extension> Public Function Decimals(n As Double) As Integer
        Dim places As Integer = 0, value As Double = Math.Abs(n)
        value -= CInt(value)
        While value > 0
            places += 1
            value *= 10
            value -= CInt(value)
        End While
        Return places
    End Function
    <Extension> Public Function ToStream(File As FileInfo) As Stream
        If (File.Exists) Then Return New FileStream(File.FullName, FileMode.Open, FileAccess.Read)
        Return New MemoryStream
    End Function
    <Extension> Public Function ToStream(str As String) As StreamReader
        Dim ms As New MemoryStream
        Dim writer As New StreamWriter(ms)
        writer.Write(str)
        writer.Flush()
        ms.Position = 0
        Return New StreamReader(ms)
    End Function
    <Extension> Public Function TryGetType(Stream As Stream, Optional Close As Boolean = False) As AssemblyType
        Try
            Dim buffer() As Byte = New Byte(3) {}
            Stream.Seek(&H3C, SeekOrigin.Begin)
            Stream.Read(buffer, 0, 4)
            Dim peoffset As UInt32 = BitConverter.ToUInt32(buffer, 0)
            Stream.Seek(peoffset + &H5C, SeekOrigin.Begin)
            Stream.Read(buffer, 0, 1)
            If buffer(0) = 3 Then
                Return AssemblyType.ConsoleApplication
            ElseIf buffer(0) = 2 Then
                Return AssemblyType.WindowApplication
            Else
                Return AssemblyType.DynamicLinkLibrary
            End If
        Catch ex As Exception
            Return AssemblyType.Invalid
        Finally
            Stream.Position = 0
            If (Close) Then
                Stream.Close()
                Stream.Dispose()
            End If
        End Try
    End Function
    <Extension> Public Function Random(Of T)(source As IEnumerable(Of T), count As Integer) As IEnumerable(Of T)
        Return source.Shuffle.Take(count)
    End Function
    <Extension> Public Function Shuffle(Of T)(source As IEnumerable(Of T)) As IEnumerable(Of T)
        Return source.OrderBy(Function(x) Guid.NewGuid())
    End Function
End Module
