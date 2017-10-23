Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports Spaghetti.Tasks

Namespace Libraries
    Public Class References
        Inherits Task
        Public Property Generic As Dictionary(Of String, TypeReference)
        Sub New(Parent As Project)
            MyBase.New(Parent, True)
            Me.Generic = New Dictionary(Of String, TypeReference)
            Me.Generic.Add("void", Me.Parent.Module.TypeSystem.Void)
            Me.Generic.Add("object", Me.Parent.Module.TypeSystem.Object)
            Me.Generic.Add("string", Me.Parent.Module.TypeSystem.String)
            Me.Generic.Add("char", Me.Parent.Module.TypeSystem.Char)
            Me.Generic.Add("int16", Me.Parent.Module.TypeSystem.Int16)
            Me.Generic.Add("int32", Me.Parent.Module.TypeSystem.Int32)
            Me.Generic.Add("int64", Me.Parent.Module.TypeSystem.Int64)
            Me.Generic.Add("sbyte", Me.Parent.Module.TypeSystem.SByte)
            Me.Generic.Add("byte", Me.Parent.Module.TypeSystem.Byte)
            Me.Generic.Add("double", Me.Parent.Module.TypeSystem.Double)
            Me.Generic.Add("single", Me.Parent.Module.TypeSystem.Single)
            Me.Generic.Add("boolean", Me.Parent.Module.TypeSystem.Boolean)
        End Sub
        Default Public ReadOnly Property [Get](ref As String) As TypeReference
            Get
                Return Me.Generic(ref)
            End Get
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "References"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "References"
            End Get
        End Property
    End Class
End Namespace