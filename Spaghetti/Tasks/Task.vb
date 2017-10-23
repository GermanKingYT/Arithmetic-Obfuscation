Namespace Tasks
    Public MustInherit Class Task
        Protected Friend IsDependency As Boolean
        Protected Friend Property Parent As Project
        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property Description As String
        Sub New(Parent As Project, Optional IsDependency As Boolean = False)
            Me.Parent = Parent
            Me.IsDependency = IsDependency
        End Sub
        Public Overridable Sub Init()
        End Sub
        Public Overridable Sub Apply()
        End Sub
        Public Overrides Function ToString() As String
            Return Me.Name
        End Function
    End Class
End Namespace