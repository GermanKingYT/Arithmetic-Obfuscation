Imports Mono.Cecil
Imports Spaghetti.Tasks

Namespace Libraries
    Public Class Collector
        Inherits Task
        Public Property Container As List(Of Container)
        Sub New(Parent As Project)
            MyBase.New(Parent, True)
        End Sub
        Public Overrides Sub Init()
            Me.Refresh()
            MyBase.Init()
        End Sub
        Protected Friend Sub Refresh()
            Me.Container = New List(Of Container)
            For Each m As ModuleDefinition In Me.Parent.Assembly.Modules
                For Each t As TypeDefinition In m.Types
                    Dim current As New Container(t)
                    If (t.IsDesirable) Then
                        For Each field As FieldDefinition In t.Fields
                            current.Fields.Add(field)
                        Next
                        For Each method As MethodDefinition In t.Methods
                            If (method.IsDesirable) Then
                                current.Methods.Add(method)
                            End If
                        Next
                        For Each prop As PropertyDefinition In t.Properties
                            If (prop.IsDesirable) Then
                                current.Properties.Add(prop)
                            End If
                        Next
                        For Each ev As EventDefinition In t.Events
                            current.Events.Add(ev)
                        Next
                        Me.Container.Add(current)
                    End If
                Next
            Next
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Collector"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Collects all objects within assembly"
            End Get
        End Property
    End Class
End Namespace