Imports System.Threading.Tasks
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions

    'TODO : Use a frame extender for the windows.forms.form frame instead, this is too dependent on game fps and available cpu time

    ''' <summary>
    ''' <para>Custom TitleBar.</para>
    ''' <para>Allows for moving the window, closing, and minimizing.</para>
    ''' </summary>
    Public Class TitleBar
        Inherits DrawableGameComponent

        ''' <summary>Retract if False, otherwise Extract if True.</summary>
        Private ExtractingOrRetracting As Boolean
        Private TriggerZone As Panel
        Public _TitleBar As Panel
        ''' <summary>Uses its IsVisible to determine if underlying controls in other parts of the program should halt.</summary>
        Public _Exit As Panel
        Private TitleBar_MovementStepExtract As Single
        Private TitleBar_MovementStepRetract As Single
        Private TitleBar_MovementDone As Boolean = True
        Private Backwards As Single
        Private Size As Vector2
        Private MousePosition_Last As Point
        Private MousePosition_Start As Point
        Private IsMoving As Boolean
        Public Property Button_Menu As Button
        Public Property Button_Close As Button
        Public Property Button_Minimize As Button
        Public Sub New(Game As Game)
            MyBase.New(Game)
            Dim GameBounds = Game.GraphicsDevice.Viewport.Bounds.Size.ToVector2
            Size = New Vector2(GameBounds.X, GameBounds.Y * 0.045F)
            TitleBar_MovementStepExtract = ((Size.Y / YOURGAME.GetService(Of Settings.Settings).Defaults.Game.UserFPS) * 1.0F)
            TitleBar_MovementStepRetract = ((Size.Y / YOURGAME.GetService(Of Settings.Settings).Defaults.Game.UserFPS) * 0.4F)

            _TitleBar = New Panel(Size, anchor:=Anchor.TopLeft, offset:=New Vector2(0, -Size.Y))
            With _TitleBar
                .FillColor = YOURGAME.GetService(Of Globals).GiveItem(Of Brush)("CA09").Color
                .OnMouseDown = Sub() TitleBar_OnMouseDown()
                .OnMouseReleased = Sub() TitleBar_OnMouseReleased()
                .OnMouseLeave = Sub() TriggerZone_OnMouseLeave()
            End With

            TriggerZone = New Panel(Size, anchor:=Anchor.TopLeft)
            With TriggerZone
                .FillColor = Nothing
                .OnMouseEnter = Sub() TriggerZone_OnMouseEnter()
                .OnMouseLeave = Sub() TriggerZone_OnMouseLeave()
                .WhileMouseHover = Sub() TriggerZone_WhileMouseHover()
            End With

            _Exit = New Panel(GameBounds, anchor:=Anchor.TopLeft)
            With _Exit
                .FillColor = YOURGAME.GetService(Of Globals).GiveItem(Of Brush)("CA03").Color
                .AddChild(New Label(YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA16"), Anchor.Center, offset:=New Vector2(0, (-GameBounds.Y * 0.1F))))
                Dim OkCancel_Size = (GameBounds * 0.1F)
                Dim OkCancel_Offset = ((OkCancel_Size.X / 2) + (OkCancel_Size.X * 0.1F))

                .AddChild(New Button(YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA17"), Anchor.Center, OkCancel_Size, New Vector2(-OkCancel_Offset, 0)) With {.OnClick = Sub() Button_Ok_OnClick()})
                .AddChild(New Button(YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA15"), Anchor.Center, OkCancel_Size, New Vector2(OkCancel_Offset, 0)) With {.OnClick = Sub() Button_Cancel_OnClick()})
                .Visible = False
            End With

            Dim Button_Size = New Vector2((Size.X * 0.03F), Size.Y)
            Button_Menu = New Button(Anchor:=Anchor.TopLeft, Size:=Button_Size)
            With Button_Menu
                .AddChild(New Image(YOURGAME.Instance.Content.Load(Of Texture2D)(YOURGAME.GUIRoot & YOURGAME.Instance.Theme & "/textures/icons/" & "Logo - 45x45"), Button_Size, anchor:=Anchor.TopLeft) With {.ClickThrough = True})
                .OnClick = Sub() Button_Menu_OnClick()
            End With
            _TitleBar.AddChild(Button_Menu)

            Button_Close = New Button("Ⓧ", Anchor.TopRight, Button_Size)
            With Button_Close
                .ButtonParagraph.AlignToCenter = True
                .OnClick = Sub() Button_Close_OnClick()
            End With
            _TitleBar.AddChild(Button_Close)

            Button_Minimize = New Button("▬", Anchor.TopRight, Button_Size, New Vector2(+Button_Size.X, 0))
            With Button_Minimize
                .OnClick = Sub() Button_Minimize_OnClick()
            End With
            _TitleBar.AddChild(Button_Minimize)

            UserInterface.Active.AddEntity(TriggerZone)
            UserInterface.Active.AddEntity(_TitleBar)
            UserInterface.Active.AddEntity(_Exit)
        End Sub
        Private Sub Button_Menu_OnClick()
            YOURGAME.GetComponent(Of GUI).Options.Open()
        End Sub
        Private Sub Button_Close_OnClick()
            _Exit.Visible = True
        End Sub
        Private Sub Button_Ok_OnClick()
            YOURGAME.Instance.Exit()
        End Sub
        Private Sub Button_Cancel_OnClick()
            _Exit.Visible = False
        End Sub
        Private Sub Button_Minimize_OnClick()
            YOURGAME.Minimize()
        End Sub
        Public Overrides Sub Update(GameTime As GameTime)
            If (Not TitleBar_MovementDone) Then
                If ExtractingOrRetracting Then
                    Extracting()
                Else
                    Retracting()
                End If
            End If
            If IsMoving Then
                If (Input.Mouse.GetState().LeftButton = Input.ButtonState.Pressed) Then
                    Window_Move()
                    _TitleBar.SetOffset(Vector2.Zero)
                    TitleBar_MovementDone = True
                Else
                    IsMoving = False
                End If
            End If
        End Sub
        Private Sub Extracting()
            _TitleBar.SetOffset(New Vector2(0, (_TitleBar.GetRelativeOffset.Y + TitleBar_MovementStepExtract)))

            If (_TitleBar.GetRelativeOffset.Y >= Size.Y) Then
                _TitleBar.SetOffset(New Vector2(0, Size.Y))
                TitleBar_MovementDone = True
            End If
        End Sub
        Private Sub Retracting()
            'Workaround because adding negative values doesn't work as intended on Vector2
            Backwards -= TitleBar_MovementStepRetract
            _TitleBar.SetOffset(New Vector2(0, (_TitleBar.GetRelativeOffset.Y + Backwards)))

            If (_TitleBar.GetRelativeOffset.Y <= -Size.Y) Then
                _TitleBar.SetOffset(New Vector2(0, -Size.Y))
                TitleBar_MovementDone = True
                Backwards = 0
            End If
        End Sub
        Private Sub TriggerZone_WhileMouseHover()
            If (_TitleBar.GetRelativeOffset.Y <> Size.Y) Then
                TriggerZone_OnMouseEnter()
            End If
        End Sub
        Private Sub TriggerZone_OnMouseEnter()
            TitleBar_MovementDone = False
            ExtractingOrRetracting = True
        End Sub
        Private Async Function DelayTask(Time As Double) As Task
            Await Task.Delay(System.TimeSpan.FromSeconds(Time))
        End Function
        Private Async Sub TriggerZone_OnMouseLeave()
            'Allow a margin of time to pass before trying to retract
            Await DelayTask(1.0)

            'If cursor has returned, don't retract
            If (Not _TitleBar.IsMouseOver) Then
                TitleBar_MovementDone = False
                ExtractingOrRetracting = False
            End If
        End Sub

        Private Sub TitleBar_OnMouseDown()
            MousePosition_Last = New Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y)
            MousePosition_Start = Input.Mouse.GetState.Position
            IsMoving = True
        End Sub
        Private Sub TitleBar_OnMouseReleased()
            IsMoving = False
        End Sub
        ''' <summary>
        ''' Mouse movement is monitored on the OS level, to minimize the influence of the update rate on window movement.
        ''' </summary>
        Private Sub Window_Move()
            Dim CurrentMousePosition = New Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y)

            If (MousePosition_Last <> CurrentMousePosition) Then
                Dim RelativeMousePosition = Input.Mouse.GetState.Position
                Dim Movement As New Point

                If (RelativeMousePosition.X < MousePosition_Start.X) Then
                    Movement.X = (RelativeMousePosition.X - MousePosition_Start.X)
                Else
                    Movement.X = System.Math.Abs(MousePosition_Start.X - RelativeMousePosition.X)
                End If
                If (RelativeMousePosition.Y < MousePosition_Start.Y) Then
                    Movement.Y = (RelativeMousePosition.Y - MousePosition_Start.Y)
                Else
                    Movement.Y = System.Math.Abs(MousePosition_Start.Y - RelativeMousePosition.Y)
                End If

                YOURGAME.Instance.Window.Position += Movement
            End If
        End Sub

    End Class

End Namespace