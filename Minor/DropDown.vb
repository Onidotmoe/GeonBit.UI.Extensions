Imports System
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions.Minor

    Public Class DropDown
        Inherits Entity

        Public OnDropDownVisibilityChanged As EventCallback
        Public Sub New(ByVal size As Vector2, Optional PlaceholderText As String = Nothing, ByVal Optional anchor As Anchor = Anchor.Auto, ByVal Optional offset As Vector2? = Nothing, ByVal Optional skin As PanelSkin = PanelSkin.ListBackground, ByVal Optional listSkin As PanelSkin? = Nothing, ByVal Optional showArrow As Boolean = True)
            MyBase.New(size, anchor, offset)
            'Additions
            If (Not String.IsNullOrEmpty(PlaceholderText)) Then
                _placeholderText = PlaceholderText
            End If

#Region "Un-Changed"
            UseActualSizeForCollision = True
            SelectedTextPanel = New Panel(New Vector2(0, SelectedPanelHeight), skin, Anchor.TopLeft)
            SelectedTextPanelParagraph = UserInterface.DefaultParagraph(String.Empty, Anchor.CenterLeft)
            SelectedTextPanelParagraph.UseActualSizeForCollision = False
            SelectedTextPanelParagraph.UpdateStyle(Extensions.Minor.SelectList.DefaultParagraphStyle)
            SelectedTextPanelParagraph.UpdateStyle(DefaultParagraphStyle)
            SelectedTextPanelParagraph.UpdateStyle(DefaultSelectedParagraphStyle)
            SelectedTextPanel.AddChild(SelectedTextPanelParagraph, True)
            ArrowDownImage = New Image(Resources.ArrowDown, New Vector2(ArrowSize, ArrowSize), ImageDrawMode.Stretch, Anchor.CenterRight, New Vector2(-10, 0))
            SelectedTextPanel.AddChild(ArrowDownImage, True)
            ArrowDownImage.Visible = showArrow
#End Region
            SelectList = New Extensions.Minor.SelectList(New Vector2(0F, size.Y), Anchor.TopCenter, Vector2.Zero, If(listSkin, skin))
#Region "Un-Changed"
            SelectList.SetOffset(New Vector2(0, SelectedPanelHeight))
            SelectList.SpaceBefore = Vector2.Zero
            AddChild(SelectedTextPanel)
            AddChild(SelectList)

            SelectList.OnValueChange = Sub(ByVal entity As Entity)
                                           ListVisible = False
                                           SelectedTextPanelParagraph.Text = (If(SelectedValue, DefaultText))
                                       End Sub

            SelectList.OnClick = Sub(ByVal entity As Entity)
                                     ListVisible = False
                                 End Sub

            SelectList.Visible = False
            SelectedTextPanel.OnClick = Sub(ByVal self As Entity)
                                            ListVisible = Not ListVisible
                                        End Sub

            SelectedTextPanelParagraph.Text = (If(SelectedValue, DefaultText))
            SelectList.UpdateStyle(DefaultStyle)
            SelectList.PropagateEventsTo(Me)
            SelectedTextPanel.PropagateEventsTo(Me)
#End Region

            'Additions
            SelectedPanelHeight = 38
            SelectedTextPanel.Size = New Vector2(SelectedTextPanel.Size.X, SelectedPanelHeight)
            SelectList.SetOffset(New Vector2(0, 38))
            ArrowDownImage.SetOffset(Vector2.Zero)
        End Sub
        Public Property ListVisible As Boolean
            Get
                Return SelectList.Visible
            End Get
            Set(ByVal value As Boolean)
                'Additions
                If (Not (SelectList.Visible AndAlso (Not IsInsideEntity(GetMousePos())))) Then
                    SelectList.Visible = value
                    OnDropDownVisibilityChange()
                End If
            End Set
        End Property

        Protected Overridable Sub DoOnDropDownVisibilityChanged()
            OnDropDownVisibilityChanged?.Invoke(Me)
        End Sub

#Region "Un-Changed"

        Public Property DefaultText As String
            Get
                Return _placeholderText
            End Get
            Set(ByVal value As String)
                _placeholderText = value
                If SelectedIndex = -1 Then SelectedTextPanelParagraph.Text = _placeholderText
            End Set
        End Property

        Private _placeholderText As String = "Click to Select"
        Public Shared Shadows DefaultStyle As StyleSheet = New StyleSheet()
        Public Shared DefaultParagraphStyle As StyleSheet = New StyleSheet()
        Public Shared DefaultSelectedParagraphStyle As StyleSheet = New StyleSheet()
        Public Shared Shadows DefaultSize As Vector2 = New Vector2(0F, 220.0F)

        Public ReadOnly Property SelectedTextPanel As Panel

        Public Property AllowReselectValue As Boolean
            Get
                Return SelectList.AllowReselectValue
            End Get
            Set(ByVal value As Boolean)
                SelectList.AllowReselectValue = value
            End Set
        End Property
        Public Property SelectList As Extensions.Minor.SelectList
        Public ReadOnly Property SelectedTextPanelParagraph As Paragraph
        Public ReadOnly Property ArrowDownImage As Image
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
            SelectedTextPanel.UpdateDestinationRectsIfDirty()
            Return SelectedTextPanel.GetActualDestRect()
        End Function
        Public Overrides Function IsInsideEntity(ByVal point As Vector2) As Boolean
            point += _lastScrollVal.ToVector2()
            Dim rect As Rectangle

            If ListVisible Then
                SelectList.UpdateDestinationRectsIfDirty()
                rect = SelectList.GetActualDestRect()
                rect.Height += SelectedPanelHeight
                rect.Y -= SelectedPanelHeight
            Else
                SelectedTextPanel.UpdateDestinationRectsIfDirty()
                rect = SelectedTextPanel.GetActualDestRect()
            End If

            Return (point.X >= rect.Left AndAlso point.X <= rect.Right AndAlso point.Y >= rect.Top AndAlso point.Y <= rect.Bottom)
        End Function

        Public Overrides ReadOnly Property Priority As Integer
            Get
                Return 100 - _indexInParent + PriorityBonus
            End Get
        End Property

        Private Sub OnDropDownVisibilityChange()
            ArrowDownImage.Texture = If(ListVisible, Resources.ArrowUp, Resources.ArrowDown)
            SelectList.IsFocused = True
            UserInterface.Active.ActiveEntity = SelectList
            SelectList.UpdateDestinationRects()
            If SelectList.Visible Then
                SelectList.ScrollToSelected()
            End If
            MarkAsDirty()

            If AutoSetListHeight Then
                SelectList.MatchHeightToList()
            End If
#End Region
            OnDropDownVisibilityChanged?.Invoke(Me)
        End Sub

#Region "Un-Changed"

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
                Return SelectList.SelectedValue
            End Get
            Set(ByVal value As String)
                SelectList.SelectedValue = value
            End Set
        End Property

        Public Property SelectedIndex As Integer
            Get
                Return SelectList.SelectedIndex
            End Get
            Set(ByVal value As Integer)
                SelectList.SelectedIndex = value
            End Set
        End Property

        Public Property ScrollPosition As Integer
            Get
                Return SelectList.ScrollPosition
            End Get
            Set(ByVal value As Integer)
                SelectList.ScrollPosition = value
            End Set
        End Property

        Public Sub Unselect()
            SelectList.Unselect()
        End Sub

        Public Sub AddItem(ByVal value As String)
            SelectList.AddItem(value)
        End Sub

        Public Sub AddItem(ByVal value As String, ByVal index As Integer)
            SelectList.AddItem(value, index)
        End Sub

        Public Sub RemoveItem(ByVal value As String)
            SelectList.RemoveItem(value)
        End Sub

        Public Sub RemoveItem(ByVal index As Integer)
            SelectList.RemoveItem(index)
        End Sub

        Public Sub ClearItems()
            SelectList.ClearItems()
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return SelectList.Count
            End Get
        End Property

        Public ReadOnly Property Empty As Boolean
            Get
                Return SelectList.Empty
            End Get
        End Property

        Public Overrides Function IsNaturallyInteractable() As Boolean
            Return True
        End Function

        Public Sub ScrollToSelected()
            SelectList.ScrollToSelected()
        End Sub

        Public Sub scrollToEnd()
            SelectList.scrollToEnd()
        End Sub
#End Region

    End Class

End Namespace