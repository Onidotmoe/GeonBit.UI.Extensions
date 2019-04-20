Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions
    Public Class Screen
        Inherits Panel

        Overridable Property Group As String
        Overridable Property ID As String
        Overridable Property Title As String
        Property InstanceID As Integer
        ''' <summary>Prevents MouseInput from cascading to the screens below this one.</summary>
        Property EatsMouse As Boolean = True
        Sub New()
            MyBase.New(Game.Instance.CurrentSize, anchor:=Anchor.TopLeft)
            PreInit()
            Init()
        End Sub

        Sub New(Size As Vector2)
            MyBase.New(Size, anchor:=Anchor.TopLeft)
            PreInit()
            Init()
        End Sub
        Sub New(ID As String, Optional Title As String = "")
            MyBase.New(Game.Instance.CurrentSize, anchor:=Anchor.TopLeft)

            If (Title IsNot Nothing) Then
                Me.Title = Title
            Else
                Me.Title = Globals.Interpreter.GiveString(ID)
            End If

            PreInit()
            Init()
        End Sub

        Public Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing, Optional Skin As PanelSkin = PanelSkin.None)
            MyBase.New(Size, Skin, Anchor, Offset)
            PreInit()
            Init()
        End Sub
        Private Sub PreInit()
            FillColor = Nothing
        End Sub
        Overridable Sub Init()
        End Sub

        ''' <summary>Removes this Screen from the ScreenStack</summary>
        Sub Pop()
            Constructor.Screenstack.Pop(Me)
        End Sub
        ''' <summary>Adds this Screen to the Top of the ScreenStack</summary>
        Sub Push()
            Constructor.Screenstack.Push(Me)
        End Sub

        Overridable Sub MouseMoved(Current As Vector2, Moved As Vector2)
        End Sub

        Overridable Sub MousePressed(Button As Input.ButtonState)
        End Sub
    End Class
End Namespace