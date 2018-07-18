Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions
    ''' <summary>
    ''' A border that doesn't exceed its parent size.
    ''' </summary>
    ''' <remarks>Its parent has to have padding to vector2.zero for correct alignment which can also be set in the skin files.</remarks>
    Public Class Border
        Inherits Entities.Entity

        Public Top As ColoredRectangle
        Public Bottom As ColoredRectangle
        Public Left As ColoredRectangle
        Public Right As ColoredRectangle
        Private Color As Color
        Private Width As Integer

        ''' <summary>
        ''' Default border styling. Override this dictionary to change the way default borders appear.
        ''' </summary>
        Public Shared Shadows DefaultStyle As StyleSheet = New StyleSheet()

        Public Sub New(Color As Color, Width As Integer, Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing)
            MyBase.New(Size, Anchor, Offset)

            Me.Color = Color
            Me.Width = Width

            ClickThrough = True

            Top = New ColoredRectangle(Color, New Vector2(Size.X, Width), anchor:=Anchor.TopLeft)
            Bottom = New ColoredRectangle(Color, New Vector2(Size.X, Width), anchor:=Anchor.BottomLeft)
            Left = New ColoredRectangle(Color, New Vector2(Width, Size.Y), anchor:=Anchor.CenterLeft)
            Right = New ColoredRectangle(Color, New Vector2(Width, Size.Y), anchor:=Anchor.CenterRight)

            AddChild(Top)
            AddChild(Bottom)
            AddChild(Left)
            AddChild(Right)
        End Sub

        Public Sub SetColor(Color As Color)
            Me.Color = Color
            Top.FillColor = Color
            Bottom.FillColor = Color
            Left.FillColor = Color
            Right.FillColor = Color
        End Sub

        Public Sub SetWidth(Width As Integer)
            Me.Width = Width
            Top.Size = New Vector2(Size.X, Width)
            Bottom.Size = New Vector2(Size.X, Width)
            Left.Size = New Vector2(Width, Size.Y)
            Right.Size = New Vector2(Width, Size.Y)
        End Sub

    End Class
End Namespace