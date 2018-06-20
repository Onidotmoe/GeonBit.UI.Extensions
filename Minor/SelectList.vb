'Imports Microsoft.Xna.Framework
'Imports Microsoft.Xna.Framework.Graphics
'Imports System.Collections.Generic
'Imports GeonBit.UI.DataTypes
'Imports GeonBit.UI.Entities
'Imports GeonBit.UI
'Imports System

'Namespace Extensions.Minor
'#Region "Un-Changed"

'    Structure ParagraphData
'        Public list As SelectList
'        Public relativeIndex As Integer

'        Public Sub New(ByVal _list As SelectList, ByVal _relativeIndex As Integer)
'            list = _list
'            relativeIndex = _relativeIndex
'        End Sub
'    End Structure
'#End Region
'    Public Class SelectList
'        Inherits Entities.SelectList
'#Region "Un-Changed"

'        Private _value As String
'        Private _index As Integer = -1
'        Private _prevSize As Point = Point.Zero
'        Private _paragraphs As List(Of Paragraph) = New List(Of Paragraph)()
'        Private _scrollbar As VerticalScrollbar
'        Protected _destRect As Rectangle
'        Private _hadResizeWhileNotVisible As Boolean
'        Private _list As List(Of String) = New List(Of String)()
'        Private _parentLastDestRectVersion As Rectangle
'        Private _destRectVersion As Integer
'        Private _isDirty As Boolean
'#End Region
'        Public Sub New(ByVal size As Vector2, ByVal Optional anchor As Anchor = Anchor.Auto, ByVal Optional offset As Vector2? = Nothing, ByVal Optional skin As PanelSkin = PanelSkin.ListBackground)
'            MyBase.New(size, anchor, offset, skin)
'            UpdateStyle(DefaultStyle)
'            'Addition
'            _scrollbar = New VerticalScrollbar(0, 10, Anchor.CenterRight, offset:=Vector2.Zero)
'            _scrollbar.Value = 0
'            _scrollbar.Visible = False
'            ' _scrollbar._hiddenInternalEntity = True
'            AddChild(_scrollbar, False)
'        End Sub
'#Region "Un-Changed"
'        Public Shadows Sub AddItem(ByVal value As String)
'            If MaxItems <> 0 AndAlso Count >= MaxItems Then
'                Return
'            End If

'            _list.Add(value)
'            OnListChanged()
'        End Sub
'        Public Shadows Sub AddItem(ByVal value As String, ByVal index As Integer)
'            If MaxItems <> 0 AndAlso Count >= MaxItems Then
'                Return
'            End If

'            _list.Insert(index, value)
'            OnListChanged()
'        End Sub
'        Public Shadows Property SelectedValue As String
'            Get
'                Return _value
'            End Get
'            Set(ByVal value As String)
'                [Select](value)
'            End Set
'        End Property
'#End Region
'        Protected Overrides Sub DoOnFirstUpdate()
'            UserInterface.Active.OnEntitySpawn?.Invoke(Me)

'            If _parent IsNot Nothing Then
'                _parent.MarkAsDirty()
'            End If

'            'Addition
'            _scrollbar.SetOffset(Vector2.Zero)
'        End Sub

'#Region "Un-Changed"
'        Public Overrides Sub UpdateDestinationRects()
'            _destRect = CalcDestRect()
'            _destRectInternal = CalcInternalRect()
'            _isDirty = False
'            _destRectVersion += 1

'            'If _parent IsNot Nothing Then
'            '   _parentLastDestRectVersion = _parent._destRectVersion
'            'End If
'        End Sub
'        Protected Shadows Sub [Select](ByVal value As String)
'            If Not AllowReselectValue AndAlso value = _value Then
'                Return
'            End If

'            If value Is Nothing Then
'                _value = value
'                _index = -1
'                DoOnValueChange()
'                Return
'            End If

'            _index = _list.IndexOf(value)

'            If _index = -1 Then
'                _value = Nothing
'                If UserInterface.Active.SilentSoftErrors Then Return
'                Throw New Exceptions.NotFoundException("Value to set not found in list!")
'            End If

'            _value = value
'            DoOnValueChange()
'        End Sub
'        Protected Shadows Sub [Select](ByVal index As Integer, ByVal Optional relativeToScrollbar As Boolean = False)
'            If relativeToScrollbar Then
'                index += _scrollbar.Value
'            End If

'            If Not AllowReselectValue AndAlso index = _index Then
'                Return
'            End If

'            If index >= -1 AndAlso index >= _list.Count Then
'                If UserInterface.Active.SilentSoftErrors Then Return
'                Throw New Exceptions.NotFoundException("Invalid list index to select!")
'            End If

'            _value = If(index > -1, _list(index), Nothing)
'            _index = index
'            DoOnValueChange()
'        End Sub
'        Protected Overrides Sub OnResize()
'            If Not IsVisible() Then
'                _hadResizeWhileNotVisible = True
'                Return
'            End If

'            _hadResizeWhileNotVisible = False
'            ClearChildren()
'            _paragraphs.Clear()
'            UpdateDestinationRects()
'            Dim i As Integer = 0

'            While True
'                Dim paragraph As Paragraph = UserInterface.DefaultParagraph(".", Anchor.Auto)
'                paragraph.PromiscuousClicksMode = True
'                paragraph.WrapWords = False
'                paragraph.UpdateStyle(DefaultParagraphStyle)
'                paragraph.Scale = paragraph.Scale * ItemsScale
'                paragraph.SpaceAfter = paragraph.SpaceAfter + New Vector2(0, ExtraSpaceBetweenLines - 2)
'                paragraph.ExtraMargin.Y = CInt(ExtraSpaceBetweenLines / 2 + 3)
'                paragraph.AttachedData = New ParagraphData(Me, Math.Min(System.Threading.Interlocked.Increment(i), i - 1))
'                paragraph.UseActualSizeForCollision = False
'                paragraph.Size = New Vector2(0, paragraph.GetCharacterActualSize().Y + ExtraSpaceBetweenLines)
'                paragraph.BackgroundColorPadding = New Point(CInt(Padding.X), 5)
'                paragraph.BackgroundColorUseBoxSize = True
'                ' paragraph._hiddenInternalEntity = True
'                paragraph.PropagateEventsTo(Me)
'                AddChild(paragraph)
'                OnCreatedListParagraph(paragraph)
'                _paragraphs.Add(paragraph)
'                paragraph.OnClick = Sub(ByVal entity As Entity)
'                                        Dim data As ParagraphData = CType(entity.AttachedData, ParagraphData)

'                                        If Not data.list.LockSelection Then
'                                            data.list.[Select](data.relativeIndex, True)
'                                        End If
'                                    End Sub

'                paragraph.UpdateDestinationRects()
'                If (paragraph.GetActualDestRect().Bottom > _destRect.Bottom - _scaledPadding.Y) OrElse i > _list.Count Then
'                    RemoveChild(paragraph)
'                    _paragraphs.Remove(paragraph)
'                    Exit While
'                End If
'            End While

'            If _paragraphs.Count > 0 AndAlso _paragraphs.Count < _list.Count Then
'                AddChild(_scrollbar, False)
'                _scrollbar.Max = CUInt((_list.Count - _paragraphs.Count))

'                If _scrollbar.Max < 2 Then
'                    _scrollbar.Max = 2
'                End If

'                _scrollbar.StepsCount = _scrollbar.Max
'                _scrollbar.Visible = True
'            Else
'                _scrollbar.Visible = False

'                If _scrollbar.Value > 0 Then
'                    _scrollbar.Value = 0
'                End If
'            End If
'        End Sub

'        Protected Overrides Sub DrawEntity(ByVal spriteBatch As SpriteBatch) ', ByVal phase As DrawPhase)
'            If (_prevSize.Y <> _destRectInternal.Size.Y) OrElse _hadResizeWhileNotVisible Then
'                OnResize()
'            End If

'            _prevSize = _destRectInternal.Size
'            MyBase.DrawEntity(spriteBatch) ', phase)

'            For i As Integer = 0 To _paragraphs.Count - 1
'                Dim item_index As Integer = i + CInt(_scrollbar.Value)
'                Dim par = _paragraphs(i)

'                If item_index < _list.Count Then
'                    par.Text = _list(item_index)
'                    par.BackgroundColor.A = 0
'                    par.Visible = True

'                    If ClipTextIfOverflow Then
'                        Dim charWidth = par.GetCharacterActualSize().X
'                        Dim toClip = (charWidth * par.Text.Length) - _destRectInternal.Width

'                        If toClip > 0 Then
'                            Dim charsToClip = CInt(System.Math.Ceiling(toClip / charWidth)) + AddWhenClipping.Length + 1

'                            If charsToClip < par.Text.Length Then
'                                par.Text = par.Text.Substring(0, par.Text.Length - charsToClip) + AddWhenClipping
'                            Else
'                                par.Text = AddWhenClipping
'                            End If
'                        End If
'                    End If

'                    Dim isLocked As Boolean = False
'                    LockedItems.TryGetValue(item_index, isLocked)
'                    par.Locked = isLocked
'                Else
'                    par.Visible = False
'                    par.Text = String.Empty
'                End If
'            Next

'            Dim selectedParagraphIndex As Integer = _index

'            If selectedParagraphIndex <> -1 Then
'                Dim i As Integer = selectedParagraphIndex - _scrollbar.Value

'                If i >= 0 AndAlso i < _paragraphs.Count Then
'                    Dim paragraph As Paragraph = _paragraphs(i)
'                    Dim destRect As Rectangle = paragraph.GetActualDestRect()
'                    paragraph.State = EntityState.MouseDown
'                    paragraph.BackgroundColor = GetActiveStyle("SelectedHighlightColor").asColor
'                End If
'            End If
'        End Sub
'#End Region
'    End Class
'End Namespace
