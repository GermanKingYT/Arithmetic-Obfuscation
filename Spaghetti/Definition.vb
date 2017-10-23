Imports Mono.Cecil
Imports Mono.Cecil.Cil

Public Class Definition
    Public Property Type As Type
    Public Property Opcode As OpCode
    Public Property Operand As Object
    Public Property HasOperand As Boolean
    Sub New(Opcode As OpCode)
        Me.Opcode = Opcode
        Me.HasOperand = False
    End Sub
    Sub New(Opcode As OpCode, Operand As Object)
        Me.Opcode = Opcode
        Me.Operand = Operand
        Me.HasOperand = True
        Me.Type = Operand.GetType
    End Sub
    Public Function IsFloat() As Boolean
        Return Me.Type Is GetType(Double)
    End Function
    Public Function IsMethodRef() As Boolean
        Return Me.Type.BaseType Is GetType(Reflection.MethodInfo)
    End Function
    Public Function GetFloat() As Double
        Return Convert.ToDouble(Me.Operand)
    End Function
    Public Function GetMethodRef(Type As TypeReference) As MethodReference
        Return Type.Module.Import(CType(Me.Operand, Reflection.MethodInfo))
    End Function
    Public Overrides Function ToString() As String
        If (Me.HasOperand) Then
            Return String.Format("{0}[{1}]", Me.Opcode, Me.Operand)
        Else
            Return Me.Opcode.ToString
        End If
    End Function
End Class
