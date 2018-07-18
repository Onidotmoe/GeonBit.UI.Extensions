Imports System
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions

    Public Enum SliderVerticalSkin
        [Default] = 0
        MarkOnly = 1
    End Enum

    Public Class SliderVertical
        Inherits Slider

        Private _skin As SliderVerticalSkin

        ' frame And mark actual height
        Protected _frameActualHeight As Single = 0F

        Protected _markHeight As Integer = 20

        ''' <summary>Currently calculated destination rect (eg the region this entity Is drawn on).</summary>
        Public Shadows _destRect As Rectangle
        ''' <summary>Default styling for the SliderVertical itself. Note: loaded from UI theme xml file.</summary>
        Public Shared Shadows DefaultStyle As New StyleSheet
        ''' <summary>Default slider size for when no size Is provided Or when -1 Is set for either width Or height.</summary>
        Public Shadows DefaultSize As Vector2 = New Vector2(30.0F, 0F)

        Sub New(min As UInteger, max As UInteger, Optional size As Vector2 = Nothing, Optional anchor As Anchor = Anchor.Auto, Optional offset As Vector2 = Nothing, Optional skin As SliderVerticalSkin = SliderVerticalSkin.[Default])
            ' set as dirty (eg need to recalculate destination rect)
            MarkAsDirty()
            ' store style
            _skin = skin
            ' store min And max And set default value
            Me.Min = min
            Me.Max = max
            ' set default steps count
            _stepsCount = max - min
            ' set starting value to center
            _value = CInt(min + (max - min) / 2)
            ' update default style
            UpdateStyle(DefaultStyle)

            ' store size, anchor and offset
            Dim defaultSize As Vector2 = EntityDefaultSize
            _size = If((size = Nothing), defaultSize, size)
            _offset = If((offset = Nothing), Vector2.Zero, offset)
            _anchor = anchor
            ' check default size on specific axises
            If (_size.X = -1) Then
                _size.X = defaultSize.X
            End If
            If (_size.Y = -1) Then
                _size.Y = defaultSize.Y
            End If
        End Sub

        ''' <summary>
        ''' Handle while mouse Is down event.
        ''' The Slider entity override this function to handle sliding mark up And down, instead of left-right.
        ''' </summary>
        Protected Overrides Sub DoWhileMouseDown()
            ' get mouse position and apply scroll value
            Dim mousePos = GetMousePos(_lastScrollVal.ToVector2())
            ' if in the middle calculate value based on mouse position
            If (mousePos.Y >= _destRect.Y + _frameActualHeight * 0.5) AndAlso (mousePos.Y <= _destRect.Bottom - _frameActualHeight * 0.5) Then
                Dim relativePos As Single = (mousePos.Y - _destRect.Y - _frameActualHeight * 0.5F - _markHeight * 0.5F)
                Dim internalHeight As Single = (_destRect.Height - _frameActualHeight) - _markHeight * 0.5F
                Dim relativeVal As Single = (relativePos / internalHeight)
                Value = CInt(System.Math.Round(Min + relativeVal * (Max - Min)))
            End If
            ' call event handler
            WhileMouseDown?.Invoke(Me)
        End Sub

        ''' <summary>
        ''' Handle mouse down event.
        ''' The Slider entity override this function to handle sliding mark up And down, instead of left-right.
        ''' </summary>
        Protected Overrides Sub DoOnMouseReleased()
            ' get mouse position And apply scroll value

            Dim mousePos = GetMousePos(_lastScrollVal.ToVector2())
            ' if mouse Is on the min side, decrease by 1
            If (mousePos.Y <= (_destRect.Y + _frameActualHeight)) Then
                Value = _value - GetStepSize()

                ' else if mouse Is on the max side, increase by 1
            ElseIf (mousePos.Y >= (_destRect.Bottom - _frameActualHeight)) Then
                Value = _value + GetStepSize()
            End If

            ' Call base Function
            MyBase.DoOnMouseReleased()
        End Sub

        ''' <summary>Call this function when the first update occures.</summary>
        Protected Overrides Sub DoOnFirstUpdate()
            If _parent IsNot Nothing Then
                _parent.MarkAsDirty()
                _destRect = New Rectangle(CInt(Parent.InternalDestRect.X + _offset.X), CInt(Parent.InternalDestRect.Y + _offset.Y), CInt(_size.X), CInt(_size.Y))
            End If
        End Sub

        ''' <summary>Draw the entity.</summary>
        ''' <param name="spriteBatch">Sprite batch to draw on.</param>
        Protected Overrides Sub DrawEntity(SpriteBatch As SpriteBatch, Phase As DrawPhase)
            Dim Texture As Texture2D = YOURGAME.Extender.SliderVerticalTextures(_skin)
            Dim MarkTexture As Texture2D = YOURGAME.Extender.SliderVerticalMarkTextures(_skin)
            Dim Data As DataTypes.TextureData = YOURGAME.Extender.SliderVerticalData(CInt(_skin))
            Dim FrameHeight As Single = Data.FrameHeight
            UserInterface.Active.DrawUtils.DrawSurface(SpriteBatch, Texture, _destRect, New Vector2(0F, FrameHeight), 1, FillColor)
            Dim frameSizeTexture As Vector2 = New Vector2(Texture.Width, Texture.Height * FrameHeight)
            Dim frameSizeRender As Vector2 = frameSizeTexture
            Dim ScaleYfac As Single = _destRect.Width / frameSizeRender.X
            Dim markWidth As Integer = _destRect.Width
            _markHeight = CInt(((CSng(MarkTexture.Height) / CSng(MarkTexture.Width)) * CSng(markWidth)))
            _frameActualHeight = FrameHeight * Texture.Height * ScaleYfac
            Dim markY As Single = _destRect.Y + _frameActualHeight + _markHeight * 0.5F + (_destRect.Height - _frameActualHeight * 2 - _markHeight) * (GetValueAsPercent())
            ' _destRect.X - 1, has to happen as there's an offset that needs to be compensated
            ' TODO : Figure out why the offset is there to begin with
            Dim markDest As Rectangle = New Rectangle(_destRect.X - 1, CInt(CInt(Math.Round(markY)) - _markHeight / 2), markWidth, _markHeight)
            UserInterface.Active.DrawUtils.DrawImage(SpriteBatch, MarkTexture, markDest, FillColor)
        End Sub

        ''' <summary>
        ''' Called every frame after update.
        ''' Slider override this function to handle wheel scroll while pointing on parent entity - we still want to capture that.
        ''' </summary>
        Protected Overrides Sub DoAfterUpdate()
            ' if the active entity Is self Or parent, listen to mousewheel
            If (_isInteractable AndAlso Equals(UserInterface.Active.ActiveEntity, Me) OrElse Equals(UserInterface.Active.ActiveEntity, _parent) OrElse (UserInterface.Active.ActiveEntity IsNot Nothing AndAlso UserInterface.Active.ActiveEntity.IsDeepChildOf(_parent))) Then
                If (Input.MouseWheelChange <> 0) Then
                    Value = _value - Input.MouseWheelChange * GetStepSize()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handle when mouse wheel scroll And this entity Is the active entity.
        ''' Note: Slider entity override this Function To change slider value based On wheel scroll, which Is inverted.
        ''' </summary>
        Protected Overrides Sub DoOnMouseWheelScroll()
            Value = _value - Input.MouseWheelChange * GetStepSize()
        End Sub

        ''' <summary>Ignores OnValueChange when setting the value.</summary>
        Public Sub SetValueSilent(Value As Integer)
            _value = Value
        End Sub

    End Class

End Namespace