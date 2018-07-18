Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Namespace Extensions
    Public Class Status
        Inherits Panel

        Property FPSCounter As FrameCounter
        Property MouseCoordinates As MouseCoordinates
        Property SystemUsage As SystemUsage
        Property Timer As Timer
        Property ShowFPS As Boolean
        Property ShowMousePosition As Boolean
        Property ShowSystemUsage As Boolean
        Property ShowTimer As Boolean

        Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing)
            MyBase.New(Size, anchor:=Anchor, offset:=Offset)
            FillColor = Nothing
            ClickThrough = True

            Dim Settings = YOURGAME.GetService(Of Settings.Settings).Defaults.Status

            ShowFPS = Settings.ShowFPS
            ShowMousePosition = Settings.ShowMousePosition
            ShowSystemUsage = Settings.ShowSystemUsage
            ShowTimer = Settings.ShowTimer
            AddChild(New Label(YOURGAME.Instance.Version))

            If ShowFPS Then
                FPSCounter = New FrameCounter()
                AddChild(FPSCounter)
            End If
            If ShowMousePosition Then
                MouseCoordinates = New MouseCoordinates()
                AddChild(MouseCoordinates)
            End If
            If ShowSystemUsage Then
                SystemUsage = New SystemUsage()
                AddChild(SystemUsage)
            End If
            If ShowTimer Then
                Timer = New Timer()
                AddChild(Timer)
            End If
        End Sub
    End Class

    Public Module Tools
        Class FrameCounter
            Inherits Label
            Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing)
                MyBase.New(String.Empty, Anchor, Size, Offset)
            End Sub

            Property TotalFrames As Long
            Property TotalSeconds As Single
            Property AverageFramesPerSecond As Single
            Property CurrentFramesPerSecond As Single
            Const MAXIMUM_SAMPLES As Integer = 100
            Private SampleBuffer As New Queue(Of Single)

            Protected Overrides Sub DoBeforeUpdate()
                Dim DeltaTime = CSng(YOURGAME.Instance.GameTime.ElapsedGameTime.TotalSeconds)
                CurrentFramesPerSecond = 1.0F / DeltaTime
                SampleBuffer.Enqueue(CurrentFramesPerSecond)
                If (SampleBuffer.Count > MAXIMUM_SAMPLES) Then
                    SampleBuffer.Dequeue()
                    AverageFramesPerSecond = SampleBuffer.Average(Function(i) i)
                Else
                    AverageFramesPerSecond = CurrentFramesPerSecond
                End If

                TotalFrames += 1
                TotalSeconds += DeltaTime
            End Sub

        End Class

        Class SystemUsage
            Inherits Paragraph

            Private Skip As Date
            Private PerformanceCounterToggle As Boolean
            Private _UsePerfCounter As UsePerformanceCounter
            Private _UseProcess As UseProcess
            ''' <summary>How many seconds between usage updates.</summary>
            ''' <remarks>Defaults to 3.</remarks>
            Public SkipBy As Integer = 3

            Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing, Optional PerformanceCounter As Boolean = False)
                MyBase.New(String.Empty, Anchor, Size, Offset)

                If (Not PerformanceCounter) Then
                    _UseProcess = New UseProcess
                Else
                    _UsePerfCounter = New UsePerformanceCounter
                    PerformanceCounterToggle = True
                End If
            End Sub
            Sub SkipAdd()
                Skip = Date.Now + System.TimeSpan.FromSeconds(SkipBy)
            End Sub
            Function SkipCheck() As Boolean
                Return (Date.Now > Skip)
            End Function
            Protected Overrides Sub DoBeforeUpdate()
                If (Not PerformanceCounterToggle) Then
                    _UseProcess.CallCPU()
                End If
                If SkipCheck() Then
                    SkipAdd()
                    Text = Nothing

                    If (Not PerformanceCounterToggle) Then
                        Text += "CPU : " & _UseProcess.GetCPU & "%"
                        Text += System.Environment.NewLine
                        Text += "RAM : " & _UseProcess.GetMemory
                        Text += System.Environment.NewLine
                        Text += "Threads : " & _UseProcess.GetThreads
                    Else
                        Text += "CPU : " & _UsePerfCounter.GetCPU & "%"
                        Text += System.Environment.NewLine
                        Text += "RAM : " & _UsePerfCounter.GetMemory & "MB"
                    End If
                End If
            End Sub
            Class UseProcess
                Private StartTime As Date = Date.UtcNow
                Private StartProc As System.TimeSpan = Process.GetCurrentProcess().TotalProcessorTime
                Private CPU As Double
                Public Sub CallCPU()
                    CPU = ((Process.GetCurrentProcess().TotalProcessorTime - StartProc).TotalSeconds / (System.Environment.ProcessorCount * Date.UtcNow.Subtract(StartTime).TotalSeconds))
                End Sub
                Public Function GetCPU() As String
                    Return String.Format("{0:0.0}", (CPU * 100))
                End Function
                Public Function GetMemory() As String
                    Return Process.GetCurrentProcess().PrivateMemorySize64.ToSize(SizeUnits.MB)
                End Function
                Public Function GetThreads() As String
                    Return Process.GetCurrentProcess().Threads.Count.ToString
                End Function
            End Class
            ''' <summary>
            ''' Will affect performance much more than using Process.
            ''' </summary>
            Class UsePerformanceCounter
                Private Counter_CPU As PerformanceCounter
                Private Counter_RAM As PerformanceCounter
                Public Sub SetUpUsage()
                    Dim Process = System.Diagnostics.Process.GetCurrentProcess()
                    Dim Name As String = Nothing

                    For Each Instance In New PerformanceCounterCategory("Process").GetInstanceNames()
                        If Instance.StartsWith(Process.ProcessName) Then
                            Using ProcessID = New PerformanceCounter("Process", "ID Process", Instance, True)
                                If Process.Id = CInt(ProcessID.RawValue) Then
                                    Name = Instance
                                    Exit For
                                End If
                            End Using
                        End If
                    Next

                    Counter_CPU = New PerformanceCounter("Process", "% Processor Time", Name, True)
                    Counter_RAM = New PerformanceCounter("Process", "Private Bytes", Name, True)
                End Sub
                Function GetCPU() As String
                    Return (System.Math.Round(Counter_CPU.NextValue() / System.Environment.ProcessorCount, 2)).ToString
                End Function
                Function GetMemory() As String
                    Return (((System.Math.Round(Counter_RAM.NextValue() / 1024) / 1024), 2)).ToString
                End Function
            End Class
        End Class
        'Class SystemUsage
        '    Inherits Paragraph

        '    Private Counter_CPU As PerformanceCounter
        '    Private Counter_RAM As PerformanceCounter
        '    Private Skip As Date
        '    '''<summary>
        '    '''<para>How many seconds between usage updates.</para>
        '    '''<para>Defaults to 3.</para>
        '    '''</summary>
        '    Public SkipBy As Integer = 3
        '    Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing)
        '        MyBase.New(String.Empty, Anchor, Size, Offset)

        '        SetUpUsage()
        '    End Sub

        '    Public Sub SetUpUsage()
        '        Dim Process = System.Diagnostics.Process.GetCurrentProcess()
        '        Dim Name As String = Nothing

        '        For Each Instance In New PerformanceCounterCategory("Process").GetInstanceNames()
        '            If Instance.StartsWith(Process.ProcessName) Then
        '                Using ProcessID = New PerformanceCounter("Process", "ID Process", Instance, True)
        '                    If Process.Id = CInt(ProcessID.RawValue) Then
        '                        Name = Instance
        '                        Exit For
        '                    End If
        '                End Using
        '            End If
        '        Next

        '        Counter_CPU = New PerformanceCounter("Process", "% Processor Time", Name, True)
        '        Counter_RAM = New PerformanceCounter("Process", "Private Bytes", Name, True)
        '        SkipAdd()
        '    End Sub
        '    Function GetCPU() As String
        '        Return (System.Math.Round(Counter_CPU.NextValue() / System.Environment.ProcessorCount, 2)).ToString
        '    End Function
        '    Function GetRAM() As String
        '        Return (System.Math.Round(Counter_RAM.NextValue() / 1024 / 1024, 2)).ToString
        '    End Function
        '    Sub SkipAdd()
        '        Skip = Date.Now + System.TimeSpan.FromSeconds(SkipBy)
        '    End Sub
        '    Function SkipCheck() As Boolean
        '        Return (Date.Now > Skip)
        '    End Function
        '    Protected Overrides Sub DoBeforeUpdate()
        '        If SkipCheck() Then
        '            SkipAdd()

        '            Text = Nothing
        '            Text += "CPU :" & GetCPU() & "%"
        '            Text += System.Environment.NewLine
        '            Text += "RAM :" & GetRAM() & "MB"
        '        End If
        '    End Sub

        'End Class

        Class MouseCoordinates
            Inherits Label
            Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing)
                MyBase.New(String.Empty, Anchor, Size, Offset)
            End Sub

            Protected Overrides Sub DoBeforeUpdate()
                Text = Mouse.GetState.Position.ToString
            End Sub

        End Class

        Class Timer
            Inherits Label
            Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing)
                MyBase.New(String.Empty, Anchor, Size, Offset)

                Watch.Start()
            End Sub

            Public Watch As New Stopwatch

            ''' <summary>
            ''' If second fractions should be displayed.
            ''' </summary>
            Public Fractions As Boolean = False

            Protected Overrides Sub DoBeforeUpdate()
                Text = Watch.Elapsed.ToString(If(Fractions, "mm\:ss\:fffffff", "mm\:ss"))
            End Sub

        End Class

    End Module

End Namespace