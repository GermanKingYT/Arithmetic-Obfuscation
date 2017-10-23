Imports Mono.Cecil
Imports Spaghetti.Tasks

Namespace Libraries
    Public Class Classifier
        Inherits Task
        Sub New(Parent As Project)
            MyBase.New(Parent, True)
            '// Load dependencies
            Me.Parent.Load(New References(Me.Parent))
            Me.Parent.Load(New Collector(Me.Parent))
        End Sub
        Public Overrides Sub Init()
            For i As Integer = 1 To Me.Parent.Distribution
                Dim ns As String = Randomizer.Name(16, Settings.Marker)
                Dim name As String = Randomizer.Name(16, Settings.Marker)
                Me.Parent.Module.Types.Add(New TypeDefinition(ns, name, Settings.DefType, Me.Parent.GetTask(Of References).Get("object")))
            Next
            MyBase.Init()
        End Sub
        Public Function Random() As TypeDefinition
            Return Me.Parent.GetTask(Of Collector).Container.Where(Function(x) x.Type.Name.StartsWith(Settings.Marker)).Random(1).FirstOrDefault.Type
        End Function
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Classifier"
            End Get
        End Property
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Creates random classes"
            End Get
        End Property
    End Class
End Namespace