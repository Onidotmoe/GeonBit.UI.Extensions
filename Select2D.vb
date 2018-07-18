Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions

    Public Enum Select2DSkin
        [Default] = 0
    End Enum

    ''' <summary>Allows selection of a point in a 2D plane.</summary>
    Public Class Select2D
        Inherits Entity

        Private _skin As Select2DSkin

        ' frame And mark actual height
        Protected _frameActualHeight As Single = 0F

        Protected _dotHeight As Integer = 10

        ''' <summary>Currently calculated destination rect (eg the region this entity Is drawn on).</summary>
        Public Shadows _destRect As Rectangle
        ''' <summary>Default styling for the Select2D itself. Note: loaded from UI theme xml file.</summary>
        Public Shared Shadows DefaultStyle As New StyleSheet
        ''' <summary>Default Select2D size for when no size Is provided Or when -1 Is set for either width Or height.</summary>
        Public Shadows DefaultSize As Vector2 = New Vector2(30.0F, 30.0F)

        Private Point As New Point
        Private DotAlign As Vector2
        Private OldPoint As New Point
        Public Value As New Point

        ''' <summary>
        ''' <para>Return the XY values relative to its own Width and Height.</para>
        ''' <para>Range is 0-1.</para>
        ''' </summary>
        Public RelativeValue As New Vector2

        Sub New(Optional size As Vector2 = Nothing, Optional anchor As Anchor = Anchor.Auto, Optional offset As Vector2 = Nothing, Optional skin As Select2DSkin = Select2DSkin.[Default])
            ' set as dirty (eg need to recalculate destination rect)
            MarkAsDirty()

            _skin = skin
            Dim DefaultSize As Vector2 = EntityDefaultSize
            _size = If((size = Nothing), DefaultSize, size)
            _offset = If((offset = Nothing), Vector2.Zero, offset)
            _anchor = anchor

            UpdateStyle(DefaultStyle)

            ' check default size on specific axises
            If (_size.X = -1) Then
                _size.X = DefaultSize.X
            End If
            If (_size.Y = -1) Then
                _size.Y = DefaultSize.Y
            End If
        End Sub

        ''' <summary>Handle while mouse is down event.</summary>
        Protected Overrides Sub DoWhileMouseDown()
            Dim mousePos = GetMousePos(DotAlign)

            Point.X = CInt(mousePos.X)
            Point.Y = CInt(mousePos.Y)

            'Make sure the Dot stays inside the Select2D
            If (Point.X <> OldPoint.X) OrElse (Point.Y <> OldPoint.Y) Then
                Dim DotTexture As Texture2D = YOURGAME.Instance.Extender.Select2DDotTextures(_skin)
                Dim DotAlignWidth As Integer = CInt(DotTexture.Width / 2)
                Dim DotAlignHeight As Integer = CInt(DotTexture.Height / 2)
                Dim MaxX As Integer = CInt(Size.X)
                Dim MaxY As Integer = CInt(Size.Y)

                'Store the actual value before we move the Dot to center
                Value = Point
                RelativeValue = New Vector2((Point.X / Size.X), (Point.Y / Size.Y))

                Point.X = (Point.X - DotAlignWidth)
                Point.Y = (Point.Y - DotAlignHeight)

                Select Case (Point.X - DotAlignWidth)
                    Case < 0
                        Point.X = 0
                    Case > MaxX
                        Point.X = (MaxX - DotTexture.Width)
                End Select
                Select Case (Point.Y - DotAlignHeight)
                    Case < 0
                        Point.Y = 0
                    Case > MaxY
                        Point.Y = (MaxY - DotTexture.Height)
                End Select
                Select Case (Point.X + DotTexture.Width)
                    Case > MaxX
                        Point.X = (MaxX - DotTexture.Width)
                End Select
                Select Case (Point.Y + DotTexture.Height)
                    Case > MaxY
                        Point.Y = (MaxY - DotTexture.Height)
                End Select

                OldPoint = Point
                DoOnValueChange()
            End If

            MyBase.DoWhileMouseDown()
        End Sub

        '''' <summary>
        '''' Handle mouse down event.
        '''' The Select2D entity override this function to handle sliding mark up And down, instead of left-right.
        '''' </summary>
        'Protected Overrides Sub DoOnMouseReleased()
        '    Dim mousePos = GetMousePos()

        '    ' Call base Function
        '    MyBase.DoOnMouseReleased()
        'End Sub

        ''' <summary>Call this function when the first update occures.</summary>
        Protected Overrides Sub DoOnFirstUpdate()
            If (_parent IsNot Nothing) Then
                _parent.MarkAsDirty()
                _destRect = New Rectangle(CInt(Parent.InternalDestRect.X + _offset.X), CInt(Parent.InternalDestRect.Y + _offset.Y), CInt(_size.X), CInt(_size.Y))
                DotAlign = New Vector2(-_destRect.X, -_destRect.Y)
            End If
        End Sub

        ''' <summary>Draw the entity.</summary>
        ''' <param name="spriteBatch">Sprite batch to draw on.</param>
        Protected Overrides Sub DrawEntity(SpriteBatch As SpriteBatch, Phase As DrawPhase)
            Dim Texture As Texture2D = YOURGAME.Extender.Select2DTextures(_skin)
            Dim DotTexture As Texture2D = YOURGAME.Extender.Select2DDotTextures(_skin)
            Dim Data As DataTypes.TextureData = YOURGAME.Extender.Select2DData(CInt(_skin))
            UserInterface.Active.DrawUtils.DrawSurface(SpriteBatch, Texture, _destRect, New Vector2(Data.FrameWidth, Data.FrameHeight), 1, FillColor)
            Dim DotDest As Rectangle = New Rectangle((_destRect.X + Point.X), (_destRect.Y + Point.Y), DotTexture.Width, DotTexture.Height)
            UserInterface.Active.DrawUtils.DrawImage(SpriteBatch, DotTexture, DotDest, FillColor)
            MyBase.DrawEntity(SpriteBatch, Phase)
        End Sub

    End Class

End Namespace