Imports Microsoft.VisualBasic
Imports Microsoft.Xna.Framework
Imports MonoGame.Extended.Input.InputListeners

Public Class User
  ''' <summary>
    ''' Key, The name of the Key
    ''' Command, The Subroutine the key activates
    ''' </summary>
    Property Binds As Binds
    Property Input As New Extensions.Minor.DefaultInputProvider

    ' Should be updated from the Game class' Update subroutine
    Sub Update(GameTime As GameTime)
        ' If you game allows players to play in windowed mode or having their cursor off the game's window, then checking for mouse being inside the game window would be a good idea
        ' However not mandatory
        If (IsMouseInsideWindow()) Then
            ' Updates the Geonbit.UI InputHandler
            Input.Update(GameTime)
            
            MouseMoved(Input.MousePosition, Input.MousePositionDiff)
            HandleInput()
        End If
    End Sub

    
    Sub MouseMoved(Current As Vector2, Moved As Vector2)
        ' Send MouseMoved to this class then to this Subroutine that then pushes it into the ScreenStack for it to cascade down all the possible Screens
        ' You can add another parameter to differentiate between users
        Screenstack.MouseMoved(Current, Moved)
    End Sub

    Sub HandleInput()
        For Each Bind In Binds.Mouse
            If Input.MouseButtonDown(CType(Bind.Key, Extensions.Minor.MouseButton)) Then
                RunBind(Bind.Command, True)
            ElseIf Input.MouseButtonReleased(CType(Bind.Key, Extensions.Minor.MouseButton)) Then
                RunBind(Bind.Command, False)
            End If
        Next

        For Each Bind In Binds.Keyboard
            If Input.IsKeyDown(CType(Bind.Key, Input.Keys)) Then
                RunBind(Bind.Command, True)
            ElseIf Input.IsKeyReleased(CType(Bind.Key, Input.Keys)) Then
                RunBind(Bind.Command, False)
            End If
        Next

        Select Case Input.MouseWheelChange
            Case > 0
                For Each Bind In Binds.Special
                    If (Bind.Key = "MouseWheelUp") Then
                        RunBind(Bind.Command)
                    End If
                Next
            Case < 0
                For Each Bind In Binds.Special
                    If (Bind.Key = "MouseWheelDown") Then
                        RunBind(Bind.Command)
                    End If
                Next
        End Select
    End Sub

    ' We call the command in a class instance, this way allows for extreme abstraction which gives us flexibility to have multiple commands run by the same input
    ' This however -should- be slowed then hardcoding all the inputs but this allows the user to use any XNA input they want to run any command
    Sub RunBind(Command As String, Bool As Boolean)
        CallByName(YourClass.Commands, Command, CallType.Method, Bool)
    End Sub
    Sub RunBind(Command As String)
        CallByName(YourClass.Commands, Command, CallType.Method)
    End Sub

End Class

Public Class Binds
    ' List of Binds triggered by Keyboard Input
    <XmlArray>
    Property Keyboard As List(Of Bind)
    ' List of Binds triggered by Mouse Input
    <XmlArray>
    Property Mouse As List(Of Bind)
    ' List of Binds triggered by Special Input, like Controller or other non-mouse/keyboard inputs
    <XmlArray>
    Property Special As List(Of Bind)
End Class
Public Class Bind
    ' Key is stored as a number, the same which Monogame Keyboard uses
    <XmlAttribute>
    Property Key As String
    ' The command that you want to run with this Bind, i use strings
    <XmlAttribute>
    Property Command As String
    ' This is just to make it easier to read for a human once serialized/deserialized
    <XmlAttribute>
    Property Name As String
End Class

' This is the main class that holds all your commands for your game
Public Class Commands

    Public Sub NotBound(Bool As Boolean) : End Sub

    Public Sub TestInput(Bool As Boolean)
        ' We only want this command to fire when the Input key is Up, meaning when it has been released otherwise the input will continuously fire
        If (Not Bool) Then
            ' Command will be sent to the screenstack until the command is eaten by a screen
            ' This means that this command can be fired on multiple screens if the first screen doesn't eats the command before it's sent to the second screen
            SendToScreenStack("TestInput")
        End If
    End Sub
    
    Public Sub SendToScreenStack(Command As String)
        Screenstack.DoCommand(Command)
    End Sub
End Class

<--! Below is how you can serialize your Binds class into a human readable xml file, as you can see the Name of the bind is arbitrary but is crucial since the key is a XNA input Enum -->
<?xml version="1.0" encoding="utf-16"?>
<Binds>
  <Mouse>
    <Bind Name="LeftClick" Key="0" Command="TestInput" />
  </Mouse>
</Binds>

