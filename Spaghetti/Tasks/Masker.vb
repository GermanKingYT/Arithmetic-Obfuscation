Imports Mono.Cecil
Imports Mono.Cecil.Cil

Namespace Tasks
    Public Class Masker
        Inherits Task
        Sub New(Parent As Project)
            MyBase.New(Parent)
            '// Load dependencies
            Me.Parent.Load(New Libraries.Lexer(Me.Parent))
            Me.Parent.Load(New Libraries.Syntax(Me.Parent))
            Me.Parent.Load(New Libraries.Classifier(Me.Parent))
        End Sub
        Public Overrides Sub Init()
            Me.Parent.UpdateLog(Me, String.Format("Functions    [{0}]", Me.Parent.Functions.Count))
            Me.Parent.UpdateLog(Me, String.Format("Pool         [{0}]", String.Join(" ", Me.Parent.Sequence)))
            MyBase.Init()
        End Sub
        Public Overrides Sub Apply()
            If (Me.Parent.HasTask(Of Libraries.Collector)()) Then
                For i As Integer = 1 To Me.Parent.Iterations
                    For Each container As Container In Me.Parent.GetTask(Of Libraries.Collector).Container.ToList
                        For Each method As MethodDefinition In container.Methods.ToList
                            If (Me.Parent.Abort) Then Exit Sub
                            For Each instruction As Instruction In method.Body.Instructions.ToList
                                Select Case instruction.OpCode
                                    Case OpCodes.Ldc_I4_S   '// SByte
                                        Me.Create(container.Type, method, instruction)
                                    Case OpCodes.Ldc_I4     '// Int32
                                        Me.Create(container.Type, method, instruction)
                                    Case OpCodes.Ldc_I8     '// Int64
                                        Me.Create(container.Type, method, instruction)
                                    Case OpCodes.Ldc_R4     '// Single
                                        Me.Create(container.Type, method, instruction)
                                    Case OpCodes.Ldc_R8     '// Double
                                        Me.Create(container.Type, method, instruction)
                                End Select
                            Next
                        Next
                    Next
                    Me.Parent.GetTask(Of Libraries.Collector).Refresh()
                Next
            End If
            MyBase.Apply()
        End Sub
        Private Sub Create(Type As TypeDefinition, Method As MethodDefinition, inst As Instruction)
            Dim number As Double = Math.Round(Convert.ToDouble(inst.Operand), 1)
            Dim conv As MethodReference = Nothing
            If (number.Decimals <= 1) Then
                Me.Parent.UpdateLog(Me, String.Format("-> Resolving {0} at {1}()", number, Method.Name))
                If (TypeOf inst.Operand Is Short) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToInt16", New System.Type() {GetType(Double)}))
                ElseIf (TypeOf inst.Operand Is Integer) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToInt32", New System.Type() {GetType(Double)}))
                ElseIf (TypeOf inst.Operand Is Long) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToInt64", New System.Type() {GetType(Double)}))
                ElseIf (TypeOf inst.Operand Is Decimal) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToDecimal", New System.Type() {GetType(Double)}))
                ElseIf (TypeOf inst.Operand Is SByte) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToInt32", New System.Type() {GetType(Double)}))
                ElseIf (TypeOf inst.Operand Is Single) Then
                    conv = Type.Module.Import(GetType(Convert).GetMethod("ToSingle", New System.Type() {GetType(Double)}))
                End If
                Me.Inject(Me.GetEquations(number), Type, Method, inst, conv)
            End If
        End Sub
        Private Sub Inject(Results As List(Of String), Type As TypeDefinition, Method As MethodDefinition, inst As Instruction, Optional Convert As MethodReference = Nothing)
            If (Results.Any) Then
                Me.Parent.UpdateLog(Me, String.Format(" - {0}{1}", Results.Single, Environment.NewLine))
                Dim target As TypeDefinition = Me.Parent.GetTask(Of Libraries.Classifier).Random
                Dim proxy As MethodDefinition = Me.CreateMethod(Me.GenerateBody(Me.Parent.GetTask(Of Libraries.Lexer).GetStream(Results.First.ToStream)), target)
                Dim processor As ILProcessor = Method.Body.GetILProcessor
                processor.InsertAfter(inst, processor.Create(OpCodes.Call, proxy))
                If (Convert IsNot Nothing) Then
                    Dim index As Integer = processor.Body.Instructions.IndexOf(inst) + 2
                    processor.Body.Instructions.Insert(index, processor.Create(OpCodes.Call, Convert))
                End If
                target.Methods.Add(proxy)
                processor.Remove(inst)
            End If
        End Sub
        Private Function CreateMethod(body As List(Of Definition), Type As TypeDefinition) As MethodDefinition
            Dim Method As New MethodDefinition(Randomizer.Name(16), Settings.DefMethod, Type.Module.Import(GetType(Double)))
            Dim processor As ILProcessor = Method.Body.GetILProcessor
            For Each def As Definition In body
                If (Not def.HasOperand) Then
                    processor.Emit(def.Opcode)
                ElseIf (def.HasOperand AndAlso def.IsFloat) Then
                    processor.Emit(def.Opcode, def.GetFloat)
                ElseIf (def.HasOperand AndAlso def.IsMethodRef) Then
                    processor.Emit(def.Opcode, def.GetMethodRef(Type))
                End If
            Next
            processor.Emit(OpCodes.Ret)
            Return Method
        End Function
        Private Function GetEquations(Target As Double) As List(Of String)
            Dim result As New List(Of String)
            Me.Search(Target, result, Me.Parent.Sequence.Select(Function(x As Double) New Value(x)).ToList)
            Return result
        End Function
        Private Function GetExpression(left As Value, right As Value, func As Functions) As String
            Return String.Format(func.Format, left.Expression, right.Expression)
        End Function
        Private Function GetResult(left As Value, right As Value, operation As Functions) As Double
            Return operation.GetResult(left.Number, right.Number)
        End Function
        Private Sub Create(sequence As List(Of Value), i As Integer, j As Integer, values As List(Of Value), func As Functions)
            For x As Integer = 0 To sequence.Count - 1
                If (x <> i AndAlso x <> j) Then
                    values.Add(sequence(x))
                End If
            Next
            values.Add(New Value(Me.GetResult(sequence(i), sequence(j), func), Me.GetExpression(sequence(i), sequence(j), func)))
        End Sub
        Private Sub Search(Target As Double, expressions As List(Of String), sequence As List(Of Value))
            If sequence.Count = 0 Then
                Return
            ElseIf sequence.Count = 1 Then
                Dim value As Value = sequence(0)
                If (Math.Abs(value.Number - Target) <= Me.Parent.Neglecting) Then
                    expressions.Add(value.Expression)
                End If
                Return
            Else
                Dim buffer As New List(Of Value)
                For i As Integer = 0 To sequence.Count - 1
                    For j As Integer = i + 1 To sequence.Count - 1
                        For Each func As Functions In Me.Parent.Functions
                            If (expressions.Count >= 1 Or Me.Parent.Abort) Then Return
                            buffer.Clear()
                            Me.Create(sequence, i, j, buffer, func)
                            Me.Search(Target, expressions, buffer)
                        Next
                    Next
                Next
            End If
        End Sub
        Private Function GenerateBody(tokens As List(Of Token)) As List(Of Definition)
            Dim instructions As New List(Of Definition)
            For Each e As Expressions.Base In Me.Parent.GetTask(Of Libraries.Syntax).Parse(tokens)
                Me.ParseExpression(e, instructions)
            Next
            Return instructions
        End Function
        Private Function ParseExpression(e As Expressions.Base, buffer As List(Of Definition)) As Expressions.Base
            If (TypeOf e Is Expressions.Binary) Then
                Me.ParseFunction(CType(e, Expressions.Binary), buffer)
            ElseIf (TypeOf e Is Expressions.Sqrt) Then
                Me.ParseFunction(CType(e, Expressions.Sqrt), buffer)
            ElseIf (TypeOf e Is Expressions.Squared) Then
                Me.ParseFunction(CType(e, Expressions.Squared), buffer)
            ElseIf (TypeOf e Is Expressions.Sin) Then
                Me.ParseFunction(CType(e, Expressions.Sin), buffer)
            ElseIf (TypeOf e Is Expressions.Tan) Then
                Me.ParseFunction(CType(e, Expressions.Tan), buffer)
            ElseIf (TypeOf e Is Expressions.Cos) Then
                Me.ParseFunction(CType(e, Expressions.Cos), buffer)
            ElseIf (TypeOf e Is Expressions.Log) Then
                Me.ParseFunction(CType(e, Expressions.Log), buffer)
            ElseIf (TypeOf e Is Expressions.Round) Then
                Me.ParseFunction(CType(e, Expressions.Round), buffer)
            ElseIf (TypeOf e Is Expressions.Float) Then
                Me.ParseValue(CType(e, Expressions.Float), buffer)
            End If
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Log, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Log", New System.Type() {GetType(Double)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Sin, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Sin", New System.Type() {GetType(Double)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Tan, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Tan", New System.Type() {GetType(Double)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Cos, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Cos", New System.Type() {GetType(Double)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Sqrt, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Sqrt", New System.Type() {GetType(Double)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Squared, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Dup))
            buffer.Add(New Definition(OpCodes.Mul))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Round, buffer As List(Of Definition)) As Expressions.Base
            Dim value As Expressions.Base = Me.ParseExpression(e.Value, buffer)
            buffer.Add(New Definition(OpCodes.Ldc_I4_1))
            buffer.Add(New Definition(OpCodes.Call, GetType(Math).GetMethod("Round", New System.Type() {GetType(Double), GetType(Integer)})))
            Return e
        End Function
        Private Function ParseFunction(e As Expressions.Binary, buffer As List(Of Definition)) As Expressions.Base
            Dim left As Expressions.Base = Me.ParseExpression(e.Left, buffer)
            Dim right As Expressions.Base = Me.ParseExpression(e.Right, buffer)
            If (e.Type = Types.Plus) Then
                buffer.Add(New Definition(OpCodes.Add))
            ElseIf (e.Type = Types.Minus) Then
                buffer.Add(New Definition(OpCodes.Sub))
            ElseIf (e.Type = Types.Mult) Then
                buffer.Add(New Definition(OpCodes.Mul))
            ElseIf (e.Type = Types.Div) Then
                buffer.Add(New Definition(OpCodes.Div))
            End If
            Return e
        End Function
        Private Function ParseValue(e As Expressions.Float, buffer As List(Of Definition)) As Expressions.Base
            buffer.Add(New Definition(OpCodes.Ldc_R8, e.Value))
            Return e
        End Function
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Masker"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Masks all constant numbers with random generated equations"
            End Get
        End Property
    End Class
End Namespace