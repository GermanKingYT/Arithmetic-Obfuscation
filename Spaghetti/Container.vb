Imports Mono.Cecil

Public Class Container
    Public Property Type As TypeDefinition
    Public Property Events As List(Of EventDefinition)
    Public Property Fields As List(Of FieldDefinition)
    Public Property Methods As List(Of MethodDefinition)
    Public Property Properties As List(Of PropertyDefinition)
    Sub New(Type As TypeDefinition)
        Me.Type = Type
        Me.Events = New List(Of EventDefinition)
        Me.Fields = New List(Of FieldDefinition)
        Me.Methods = New List(Of MethodDefinition)
        Me.Properties = New List(Of PropertyDefinition)
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0} Methods: {1} Fields: {2} Properties: {3}", Me.Type.Name, Me.Methods.Count, Me.Fields.Count, Me.Properties.Count)
    End Function
End Class