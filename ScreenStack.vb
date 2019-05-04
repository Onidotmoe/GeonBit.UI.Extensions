Imports System.Collections.Generic
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions
    ''' <summary>Manages a deck of screens layered ontop of each other.</summary>
    Public Class ScreenStack
        Inherits Panel

        Property Screens As New List(Of Screen)

        ''' <summary>Adds the Screen to the Top of the Stack</summary>
        Sub Push(Screen As Screen)
            Screens.Insert(0, Screen)
            AddChild(Screen)
        End Sub
        ''' <summary>Pops the Current Top Screen and Pushes the Specified one to the Top.</summary>
        Sub PopAndPush(Screen As Screen)
            Screens(0).Pop()
            Screen.Push()
        End Sub
        ''' <summary>Takes an existing Screen at the specified index in the ScreenStack and moves it to the Top.</summary>
        Sub PushToTop(i As Integer)
            Dim Screen As Screen = Screens(i)
            Screens.RemoveAt(i)
            Push(Screen)
        End Sub
        ''' <summary>Removes the TopMost Screen and Displays the one below it</summary>
        Sub PopTopMost()
            Screens(0).RemoveFromParent()
            Screens.RemoveAt(0)
            Screens(0).Visible = True
        End Sub
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

        Public Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing, Optional Skin As PanelSkin = PanelSkin.None)
            MyBase.New(Size, Skin, Anchor, Offset)
            PreInit()
            Init()
        End Sub

        Private Sub PreInit()
            FillColor = Nothing
            SetAnchorAndOffset(Anchor.TopLeft, Vector2.Zero)
            Padding = Vector2.Zero

            UserInterface.Active.AddEntity(Me)
        End Sub

        ''' <summary>Hides all Screens in the ScreenStack</summary>
        Sub HideAll()
            For Each Screen In Screens
                Screen.Visible = False
            Next
        End Sub

        Overridable Sub Init()
        End Sub
        ''' <summary>Removes the specified Screen from the ScreenStack</summary>
        Sub Pop(Screen As Screen)
            Dim i = Screens.FindIndex(Function(F) F.InstanceID = Screen.InstanceID)
            Screen.RemoveFromParent()
            Screens.RemoveAt(i)
        End Sub
        ''' <summary>Removes all Screens with the specified ID.</summary>
        Sub PopByID(ID As String)
            For i = (Screens.Count - 1) To 0 Step -1
                If (Screens(i).ID = ID) Then
                    Screens(i).Pop()
                End If
            Next
        End Sub
        ''' <summary>Removes all Screens in the specified Group.</summary>
        Sub PopByGroup(Group As String)
            For i = (Screens.Count - 1) To 0 Step -1
                If (Screens(i).Group = Group) Then
                    Screens(i).Pop()
                End If
            Next
        End Sub
        ''' <summary>If any Screen with the specified Group name exists in the ScreenStack.</summary>
        Function AnyGroup(Group As String) As Boolean
            Return Screens.Exists(Function(F) F.Group = Group)
        End Function

        ''' <summary>Returns the first Screen with the matching ID.</summary>
        Function GetScreenByID(ID As String) As Screen
            Return Screens.Find(Function(F) F.ID = ID)
        End Function
        Sub MouseMoved(Current As Vector2, Moved As Vector2)
            Screens(0).MouseMoved(Current, Moved)

            If (Not Screens(0).EatsCommands) Then
                For i = 1 To (Screens.Count - 1)
                    If (Not Screens(i).EatsCommands) Then
                        Screens(i).MouseMoved(Current, Moved)
                    Else
                        Exit For
                    End If
                Next
            End If
        End Sub
        ''' <summary>Cascades the Command down to all Screens from the current Screen until EatsCommands = true</summary>
        Sub DoCommand(Command As String, ParamArray Objects() As Object)
            Screens(0).DoCommand(Command, Objects)

            If (Not Screens(0).EatsCommands) Then
                For i = 1 To (Screens.Count - 1)
                    If (Not Screens(i).EatsCommands) Then
                        Screens(i).DoCommand(Command, Objects)
                    Else
                        Exit For
                    End If
                Next
            End If
        End Sub

    End Class
End Namespace