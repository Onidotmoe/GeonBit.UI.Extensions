Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions

    Class DialogBox
        Inherits Panel

        Private DefaultBackgroundColor As Color = Color.TransparentBlack
        Private DefaultPanelColor As Color = Color.Black
        Public Header As String = String.Empty
        Public Description As String = String.Empty
        Public Icon As Texture2D
        Public Ok As String = "Ok"
        Public Cancel As String = "Cancel"
        Public Center As Panel
        Public OnDialogResultChange As EventCallback = Nothing
        Private _DialogResult As DialogResult
        Property DialogResult As DialogResult
            Get
                Return _DialogResult
            End Get
            Set
                _DialogResult = Value
                OnDialogResultChange?.Invoke(Me)
            End Set
        End Property
        Sub New(Header As String, Description As String, Ok As String, Cancel As String)
            MyBase.New(USE_DEFAULT_SIZE)

            Me.Ok = Ok
            Me.Cancel = Cancel
            Me.Header = Header
            Me.Description = Description
        End Sub

        Sub New(Header As String, Description As String)
            MyBase.New(USE_DEFAULT_SIZE)

            Me.Header = Header
            Me.Description = Description

            Initialize()
        End Sub

        Sub New(Header As String)
            MyBase.New(USE_DEFAULT_SIZE)

            Me.Header = Header

            Initialize()
            SetBasic(CenterHeader:=True)
        End Sub
        Sub Initialize()
            Size = If(Size = DefaultSize, YOURGAME.Instance.CurrentSize, Size)
        End Sub

        Sub SetBasic(Optional Scale As Single = 3, Optional CenterHeader As Boolean = False)
            Background = New ColoredRectangle(DefaultBackgroundColor, Size, Anchor.Center)
            Center = New Panel((Size / Scale), anchor:=Anchor.Center) With {.FillColor = DefaultPanelColor}

            Dim Button_Size As New Vector2((Center.Size.X / 3), (Center.Size.Y / 5))
            With Center
                .Padding = New Vector2((Center.Size.X / 9), (Center.Size.Y / 7))

                If (Not CenterHeader) Then
                    .AddChild(New Header(Header) With {.Scale = ((Size.X / Size.Y) * 1.1F)})
                    .AddChild(New HorizontalLine())
                    .AddChild(New Paragraph(Description))
                Else
                    .AddChild(New Header(Header, Anchor.TopCenter, offset:=New Vector2(0, (Center.Size.Y / 6))) With {.Scale = ((Size.X / Size.Y) * 1.1F)})
                End If

                .AddChild(New Button(Ok, Anchor.BottomLeft, Button_Size, New Vector2(0, -(Button_Size.Y * 0.2F))) With {.OnClick = Sub() DialogResult = DialogResult.OK})
                .AddChild(New Button(Cancel, Anchor.BottomRight, Button_Size, New Vector2(0, -(Button_Size.Y * 0.2F))) With {.OnClick = Sub() DialogResult = DialogResult.Cancel})
            End With

            AddChild(Background)
            AddChild(Center)
        End Sub
        Sub Show()
            UserInterface.Active.AddEntity(Me).BringToFront()
        End Sub
        Sub Close()
            RemoveFromParent()
            Dispose()
        End Sub

    End Class

End Namespace