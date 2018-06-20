Imports System.Collections.Generic
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.VisualBasic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports MonoGame.Extended

Namespace Extensions.Minor

    'Public Class TextInput
    '    Inherits Entities.TextInput

    '    Sub New(multiline As Boolean, size As Vector2, Optional anchor As Anchor = Anchor.Auto, Optional offset As Vector2? = Nothing, Optional skin As PanelSkin = PanelSkin.ListBackground)
    '        MyBase.New(multiline, size, anchor, offset, skin)
    '    End Sub

    '    Private MouseDown_PositionStart As Vector2
    '    Private MouseDown_PositionEnd As Vector2
    '    Private HasSelection As Boolean
    '    Private Selection_Start As Integer
    '    Private Selection_End As Integer

    '    ' scrollbar to use if text height exceed the input box size
    '    Private _scrollbar As VerticalScrollbar
    '    Private _value As String = String.Empty

    '    ''' <summary>ignores OnValueChange when setting the value.</summary>
    '    Public Sub SetValueSilent(Value As String)
    '        _value = Value
    '    End Sub

    '    ''' <summary>The actual displayed text, after wordwrap And other processing. 
    '    ''' note: only the text currently visible by scrollbar.</summary>
    '    Dim _actualDisplayText As String = String.Empty
    '    ''' <summary>
    '    ''' Handle mouse down event, called every frame while down.
    '    ''' </summary>
    '    Protected Overrides Sub DoWhileMouseDown()
    '        MouseDown_PositionEnd = GetMousePos()
    '        CheckIfSelected()
    '    End Sub

    '    ''' <summary>
    '    ''' Handle mouse down event.
    '    ''' </summary>
    '    Protected Overrides Sub DoOnMouseDown()
    '        MouseDown_PositionStart = GetMousePos()
    '    End Sub
    '    ''' <summary>
    '    ''' Handle mouse up event.
    '    ''' </summary>
    '    Protected Overrides Sub DoOnMouseReleased()
    '        MouseDown_PositionEnd = GetMousePos()
    '        CheckIfSelected()
    '    End Sub

    '    Private Sub CheckIfSelected()
    '        'If (Not String.IsNullOrEmpty(Value)) Then
    '        '    If (MouseDown_PositionStart.X <> MouseDown_PositionEnd.X) Then
    '        '        Dim Relative_X = (_offset - MouseDown_PositionStart)


    '        '        Selection_Start = CInt(Relative_X.X * TextParagraph.Size.X)

    '        '        HasSelection = True
    '        '    Else
    '        '        HasSelection = False
    '        '    End If
    '        'Else
    '        '    HasSelection = False
    '        'End If
    '    End Sub


    '    Private Sub HandleSelection(SpriteBatch As SpriteBatch)
    '        If HasSelection Then


    '            'If HasSelection Then


    '            '  Dim SelectionRectangle As New RectangleF(,, (Size.X *), (Size.Y * 0.8))


    '            SpriteBatch.Begin()
    '            'ShapeExtensions.FillRectangle(spriteBatch, SelectionRectangle, SelectedHighlightColor)
    '            SpriteBatch.End()
    '            'End If
    '        End If
    '    End Sub

    '    ''' <summary>
    '    ''' Draw the entity.
    '    ''' </summary>
    '    ''' <param name="spriteBatch">Sprite batch to draw on.</param>
    '    ''' <param name="phase">The phase we are currently drawing.</param>
    '    Protected Shadows Sub DrawEntity(ByVal spriteBatch As SpriteBatch)
    '        ' call base draw function to draw the panel part
    '        MyBase.DrawEntity(spriteBatch)
    '        ' get which paragraph we currently show - real Or placeholder
    '        Dim showPlaceholder As Boolean = Not (IsFocused OrElse _value.Length > 0)
    '        ' get actual processed string
    '        Dim currParagraph As Paragraph = If(showPlaceholder, PlaceholderParagraph, TextParagraph)
    '        _actualDisplayText = PrepareInputTextForDisplay(showPlaceholder, IsFocused)

    '        HandleSelection(spriteBatch)

    '        ' for multiline only - handle scrollbar visibility And max
    '        If _multiLine AndAlso _actualDisplayText IsNot Nothing Then
    '            ' get how many lines can fit in the textbox And how many lines display text actually have
    '            Dim linesFit As Integer = CInt(_destRectInternal.Height / CInt((System.Math.Max(currParagraph.GetCharacterActualSize().Y, 1))))
    '            Dim linesInText As Integer = _actualDisplayText.Split(CChar(vbLf)).Length

    '            ' if there are more lines than can fit, show scrollbar And manage scrolling
    '            If linesInText > linesFit Then
    '                ' fix paragraph width to leave room for the scrollbar
    '                Dim prevWidth As Single = currParagraph.Size.X
    '                currParagraph.Size = New Vector2(_destRectInternal.Width / GlobalScale - 20, 0)

    '                If currParagraph.Size.X <> prevWidth Then
    '                    ' update size And re-calculate lines in text
    '                    _actualDisplayText = PrepareInputTextForDisplay(showPlaceholder, IsFocused)
    '                    linesInText = _actualDisplayText.Split(CChar(vbLf)).Length
    '                End If

    '                ' set scrollbar max And steps
    '                _scrollbar.Max = CUInt(System.Math.Max(linesInText - linesFit, 2))
    '                _scrollbar.StepsCount = _scrollbar.Max
    '                _scrollbar.Visible = True
    '                ' update text to fit scrollbar. first, rebuild the text with just the visible segment
    '                Dim lines As List(Of String) = New List(Of String)(_actualDisplayText.Split(CChar(vbLf)))
    '                Dim from As Integer = System.Math.Min(_scrollbar.Value, lines.Count - 1)
    '                Dim size As Integer = System.Math.Min(linesFit, lines.Count - from)
    '                lines = lines.GetRange(from, size)
    '                _actualDisplayText = String.Join(vbLf, lines)
    '                currParagraph.Text = _actualDisplayText
    '            Else
    '                ' if no need for scrollbar make it invisible
    '                currParagraph.Size = Vector2.Zero
    '                _scrollbar.Visible = False
    '            End If
    '        End If

    '        TextParagraph.Visible = Not showPlaceholder
    '        PlaceholderParagraph.Visible = showPlaceholder
    '    End Sub



    'End Class

End Namespace