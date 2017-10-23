Imports System.IO
Imports Mono.Cecil
Imports Spaghetti.Tasks

Public Class Project
    Inherits Settings
    Protected Friend Property Input As FileInfo
    Protected Friend Property Output As FileInfo
    Protected Friend Property Abort As Boolean
    Protected Friend Property Loaded As Boolean
    Protected Friend Property Tasks As List(Of Task)
    Protected Friend Property Assembly As AssemblyDefinition
    Protected Friend Property AssemblyType As AssemblyType
    Public Event Status(Sender As Object, Message As String, IsError As Boolean)
    Sub New(Filename As String)
        If (File.Exists(Filename)) Then
            Me.Tasks = New List(Of Task)
            Me.Input = New FileInfo(Path.GetFullPath(Filename))
            Me.Output = New FileInfo(Me.Input.FullName.Replace(".", ".modified."))
            Me.Assembly = AssemblyDefinition.ReadAssembly(Me.Input.FullName)
            Me.AssemblyType = Me.Input.ToStream.TryGetType
            Me.Loaded = True
            Me.UpdateLog(Me, String.Format("Assembly loaded, target '{0}'", Me.AssemblyType))
        Else
            Me.Loaded = False
            Me.UpdateLog(Me, "Assembly not loaded, invalid assembly", True)
        End If
    End Sub
    Public Function Initialize() As Boolean
        Me.Abort = False
        For Each task As Task In Me.Tasks
            task.Init()
            If (Me.Abort) Then Return False
        Next
        Return True
    End Function
    Public Function Begin() As Boolean
        If (Me.Loaded) Then
            Me.UpdateLog(Me, "———————————————————————————————————————————————")
            For Each task As Task In Me.Tasks.Where(Function(x) Not x.IsDependency)
                task.Apply()
                If (Me.Abort) Then Return False
            Next
            Me.UpdateLog(Me, "———————————————————————————————————————————————")
            Return True
        End If
        Return False
    End Function
    Public Sub Finish()
        If (File.Exists(Me.Output.FullName)) Then
            Me.UpdateLog(Me, "Removing old file...")
            Me.Output.Delete()
        End If
        Me.UpdateLog(Me, "Writing new assembly file...")
        Me.Assembly.Write(Me.Output.FullName)
        Me.UpdateLog(Me, String.Format("Finished, saved to '{0}'", Me.Output.Name))
    End Sub
    Protected Friend Function [Module]() As ModuleDefinition
        Return Me.Assembly.MainModule
    End Function
    Protected Friend Sub Load(Task As Task)
        If (Not Me.HasTask(Task.GetType)) Then Me.Tasks.Insert(0, Task)
    End Sub
    Protected Friend Sub UpdateLog(Sender As Object, Message As String, Optional IsError As Boolean = False)
        If (IsError) Then Me.Abort = True
        RaiseEvent Status(Sender, Message, IsError)
    End Sub
    Protected Friend Function HasTask(Type As Type) As Boolean
        Return Me.Tasks.Any AndAlso Me.Tasks.Any(Function(task As Task) task.GetType Is Type)
    End Function
    Protected Friend Function HasTask(Of T)() As Boolean
        Return Me.Tasks.Any AndAlso Me.Tasks.Any(Function(task As Task) TypeOf task Is T)
    End Function
    Protected Friend Function GetTask(Of T)() As T
        If (Me.HasTask(Of T)()) Then Return Me.Tasks.Where(Function(task As Task) TypeOf Task Is T).Cast(Of T)().FirstOrDefault()
    End Function
End Class
