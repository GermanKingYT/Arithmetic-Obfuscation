Imports System.Reflection.Emit
Imports System.IO

Public Class Main
    Private Property Project As Project
    Private Property Filename As String
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AddLog("Drap & Drop an assembly onto the form")
    End Sub
    Private Sub Main_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If (files.Any AndAlso File.Exists(files.Single)) Then
            Me.Filename = files.Single
            Me.btnStart.Enabled = True
            Me.AddLog(String.Format("Target       {0}", Path.GetFileName(Me.Filename)), True)
        End If
    End Sub
    Private Sub Main_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        Me.Project = New Project(Me.Filename)
        AddHandler Me.Project.Status, AddressOf Me.EventStatus
        Me.Project.Tasks.Add(New Tasks.Masker(Project))
        Call New Threading.Thread(AddressOf Me.Begin) With {.IsBackground = True}.Start()
    End Sub
    Private Sub Begin()
        If (Project.Loaded) Then
            Me.ChangeUI(False)
            Me.Project.Initialize()
            Me.Project.Begin()
            Me.Project.Finish()
            Me.ChangeUI(True)
        Else
            Me.AddLog("No valid input")
        End If
    End Sub
    Private Sub EventStatus(Sender As Object, Message As String, IsError As Boolean)
        Me.AddLog(String.Format("{0}", Message))
    End Sub
    Private Sub ChangeUI(state As Boolean)
        If (Me.InvokeRequired) Then
            Me.Invoke(Sub() Me.ChangeUI(state))
        Else
            Me.btnStart.Enabled = state
        End If
    End Sub
    Private Sub AddLog(Message As String, Optional Reset As Boolean = False)
        If (Me.InvokeRequired) Then
            Me.Invoke(Sub() Me.AddLog(Message, Reset))
        Else
            If (Reset) Then Me.DebugLog.Clear()
            Me.DebugLog.AppendText(String.Format("{0}{1}", Message, Environment.NewLine))
            Me.DebugLog.SelectionStart = Me.DebugLog.Text.Length
            Me.DebugLog.ScrollToCaret()
        End If
    End Sub
End Class
