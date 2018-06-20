Imports System
Imports System.Collections.Generic
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.VisualBasic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions.Minor

    Public Class DropDown
        Inherits Entity

        Public Sub New(ByVal size As Vector2, Optional PlaceholderText As String = Nothing, ByVal Optional anchor As Anchor = Anchor.Auto, ByVal Optional offset As Vector2? = Nothing, ByVal Optional skin As PanelSkin = PanelSkin.ListBackground, ByVal Optional listSkin As PanelSkin? = Nothing, ByVal Optional showArrow As Boolean = True)
            MyBase.New(size, anchor, offset)
            'Additions
            If (Not String.IsNullOrEmpty(PlaceholderText)) Then
                _placeholderText = PlaceholderText
            End If

#Region "Un-Changed"
            Padding = Vector2.Zero
            UseActualSizeForCollision = True
            _selectedTextPanel = New Panel(New Vector2(0, SelectedPanelHeight), skin, Anchor.TopLeft)
            _selectedTextParagraph = UserInterface.DefaultParagraph(String.Empty, Anchor.CenterLeft)
            _selectedTextParagraph.UseActualSizeForCollision = False
            _selectedTextParagraph.UpdateStyle(Extensions.Minor.SelectList.DefaultParagraphStyle)
            _selectedTextParagraph.UpdateStyle(DefaultParagraphStyle)
            _selectedTextParagraph.UpdateStyle(DefaultSelectedParagraphStyle)
            _selectedTextPanel.AddChild(_selectedTextParagraph, True)
            _arrowDownImage = New Image(Resources.ArrowDown, New Vector2(ArrowSize, ArrowSize), ImageDrawMode.Stretch, Anchor.CenterRight, New Vector2(-10, 0))
            _selectedTextPanel.AddChild(_arrowDownImage, True)
            _arrowDownImage.Visible = showArrow
            _selectList = New Extensions.Minor.SelectList(New Vector2(0F, size.Y), Anchor.TopCenter, Vector2.Zero, If(listSkin, skin))
            _selectList.SetOffset(New Vector2(0, SelectedPanelHeight))
            _selectList.SpaceBefore = Vector2.Zero
            AddChild(_selectedTextPanel)
            AddChild(_selectList)

            _selectList.OnValueChange = Sub(ByVal entity As Entity)
                                            ListVisible = False
                                            _selectedTextParagraph.Text = (If(SelectedValue, DefaultText))
                                        End Sub

            _selectList.OnClick = Sub(ByVal entity As Entity)
                                      ListVisible = False
                                  End Sub

            _selectList.Visible = False
            _selectedTextPanel.OnClick = Sub(ByVal self As Entity)
                                             ListVisible = Not ListVisible
                                         End Sub

            _selectedTextParagraph.Text = (If(SelectedValue, DefaultText))
            _selectList.UpdateStyle(DefaultStyle)
            _selectList.PropagateEventsTo(Me)
            _selectedTextPanel.PropagateEventsTo(Me)
#End Region

            'Additions
            SelectedPanelHeight = 38
            SelectedTextPanel.Size = New Vector2(SelectedTextPanel.Size.X, SelectedPanelHeight)
            SelectList.SetOffset(New Vector2(0, 38))
            ArrowDownImage.SetOffset(Vector2.Zero)
        End Sub
        Public Property ListVisible As Boolean
            Get
                Return _selectList.Visible
            End Get
            Set(ByVal value As Boolean)
                'Additions
                If (Not (_selectList.Visible AndAlso (Not IsInsideEntity(GetMousePos())))) Then
                    _selectList.Visible = value
                    OnDropDownVisibilityChange()
                End If
            End Set
        End Property

#Region "Un-Changed"

        Public Property DefaultText As String
            Get
                Return _placeholderText
            End Get
            Set(ByVal value As String)
                _placeholderText = value
                If SelectedIndex = -1 Then _selectedTextParagraph.Text = _placeholderText
            End Set
        End Property

        Private _placeholderText As String = "Click to Select"
        Public Shared Shadows DefaultStyle As StyleSheet = New StyleSheet()
        Public Shared DefaultParagraphStyle As StyleSheet = New StyleSheet()
        Public Shared DefaultSelectedParagraphStyle As StyleSheet = New StyleSheet()
        Public Shared Shadows DefaultSize As Vector2 = New Vector2(0F, 220.0F)
        Private _selectedTextPanel As Panel
        Private _selectedTextParagraph As Paragraph
        Private _arrowDownImage As Image
        Private _selectList As Extensions.Minor.SelectList

        Public ReadOnly Property SelectedTextPanel As Panel
            Get
                Return _selectedTextPanel
            End Get
        End Property

        Public Property AllowReselectValue As Boolean
            Get
                Return _selectList.AllowReselectValue
            End Get
            Set(ByVal value As Boolean)
                _selectList.AllowReselectValue = value
            End Set
        End Property

        Public ReadOnly Property SelectList As Extensions.Minor.SelectList
            Get
                Return _selectList
            End Get
        End Property

        Public ReadOnly Property SelectedTextPanelParagraph As Paragraph
            Get
                Return _selectedTextParagraph
            End Get
        End Property

        Public ReadOnly Property ArrowDownImage As Image
            Get
                Return _arrowDownImage
            End Get
        End Property

        Public Shared SelectedPanelHeight As Integer = 67
        Public AutoSetListHeight As Boolean = False
        Public Shared ArrowSize As Integer = 30
        Public OnListChange As EventCallback = Nothing
        Private PriorityBonus As Integer
        Protected Overrides Sub DoOnValueChange()
            ListVisible = False
            MyBase.DoOnValueChange()
        End Sub

        Protected Overrides Function GetDestRectForAutoAnchors() As Rectangle
            _selectedTextPanel.UpdateDestinationRectsIfDirty()
            Return _selectedTextPanel.GetActualDestRect()
        End Function

        Public Overrides Function IsInsideEntity(ByVal point As Vector2) As Boolean
            point += _lastScrollVal.ToVector2()
            Dim rect As Rectangle

            If ListVisible Then
                _selectList.UpdateDestinationRectsIfDirty()
                rect = _selectList.GetActualDestRect()
                rect.Height += SelectedPanelHeight
                rect.Y -= SelectedPanelHeight
            Else
                _selectedTextPanel.UpdateDestinationRectsIfDirty()
                rect = _selectedTextPanel.GetActualDestRect()
            End If

            Return (point.X >= rect.Left AndAlso point.X <= rect.Right AndAlso point.Y >= rect.Top AndAlso point.Y <= rect.Bottom)
        End Function

        Public Overrides ReadOnly Property Priority As Integer
            Get
                Return 100 - _indexInParent + PriorityBonus
            End Get
        End Property

        Private Sub OnDropDownVisibilityChange()
            _arrowDownImage.Texture = If(ListVisible, Resources.ArrowUp, Resources.ArrowDown)
            _selectList.IsFocused = True
            UserInterface.Active.ActiveEntity = _selectList
            _selectList.UpdateDestinationRects()
            If _selectList.Visible Then _selectList.ScrollToSelected()
            MarkAsDirty()

            If AutoSetListHeight Then
                _selectList.MatchHeightToList()
            End If
        End Sub

        Protected Shadows Sub DrawEntity(ByVal spriteBatch As SpriteBatch)
        End Sub

        Protected Overrides Sub DoAfterUpdate()
            If ListVisible Then
                Dim mousePosition = GetMousePos()

                If Input.AnyMouseButtonDown() AndAlso Not IsInsideEntity(mousePosition) Then
                    ListVisible = False
                End If
            End If

            MyBase.DoAfterUpdate()
        End Sub

        Public Property SelectedValue As String
            Get
                Return _selectList.SelectedValue
            End Get
            Set(ByVal value As String)
                _selectList.SelectedValue = value
            End Set
        End Property

        Public Property SelectedIndex As Integer
            Get
                Return _selectList.SelectedIndex
            End Get
            Set(ByVal value As Integer)
                _selectList.SelectedIndex = value
            End Set
        End Property

        Public Property ScrollPosition As Integer
            Get
                Return _selectList.ScrollPosition
            End Get
            Set(ByVal value As Integer)
                _selectList.ScrollPosition = value
            End Set
        End Property

        Public Sub Unselect()
            _selectList.Unselect()
        End Sub

        Public Sub AddItem(ByVal value As String)
            _selectList.AddItem(value)
        End Sub

        Public Sub AddItem(ByVal value As String, ByVal index As Integer)
            _selectList.AddItem(value, index)
        End Sub

        Public Sub RemoveItem(ByVal value As String)
            _selectList.RemoveItem(value)
        End Sub

        Public Sub RemoveItem(ByVal index As Integer)
            _selectList.RemoveItem(index)
        End Sub

        Public Sub ClearItems()
            _selectList.ClearItems()
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return _selectList.Count
            End Get
        End Property

        Public ReadOnly Property Empty As Boolean
            Get
                Return _selectList.Empty
            End Get
        End Property

        Public Overrides Function IsNaturallyInteractable() As Boolean
            Return True
        End Function

        Public Sub ScrollToSelected()
            _selectList.ScrollToSelected()
        End Sub

        Public Sub scrollToEnd()
            _selectList.scrollToEnd()
        End Sub
#End Region

    End Class

End Namespace
