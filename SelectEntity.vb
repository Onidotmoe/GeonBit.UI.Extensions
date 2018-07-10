Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions

    Structure ParagraphData
        Public SelectEnity As SelectEntity
        Public Index As Integer

        Public Sub New(SelectEnity As SelectEntity, Index As Integer)
            Me.SelectEnity = SelectEnity
            Me.Index = Index
        End Sub
    End Structure
    Public Enum SelectEntitySkin
        [Default] = 0
    End Enum

    ''' <summary>
    ''' Allows for Generic Entities to be used for selection.
    ''' </summary>
    Public Class SelectEntity
        Inherits Panel

        ''' <summary>Currently selected items.</summary>
        Private Selected As New Dictionary(Of Integer, Entity)
        ''' <summary> store list last known internal size, so we'll know if size changed and we need to re-create the list</summary>
        Private _PrevSize As Point = Point.Zero
        Private _Scrollbar As VerticalScrollbar
        Private _isDirty As Boolean
        ''' <summary>indicate that we had a resize event while Not being visible</summary>
        Private _HadResizeWhileNotVisible As Boolean = False
        Private _destRect As Rectangle
        Private _destRectVersion As Integer = 0
        Private _parentLastDestRectVersion As UInteger = 0
        Private Const Marker As String = "SelectEntityMarker"
        ''' <summary>Currently Visible Items.</summary>
        Private VisibleItems As New List(Of Entity)
        ''' <summary>Extra space (in pixels) between items on Y axis.</summary>
        Public ExtraSpaceBetweenLines As Integer = 0
        ''' <summary>Items' scale in list.</summary>
        Public ItemsScale As Single = 1.0F
        ''' <summary>Special callback to execute when list size changes.</summary>
        Public OnListChange As EventCallback = Nothing
        ''' <summary>When set to true, users cannot change the currently selected value.
        ''' Note: unlike the basic entity "Locked" that prevent all input from entity And its children,
        ''' this method of locking will still allow users to scroll through the list, thus making it useable
        ''' as a read-only list entity.</summary>
        Public LockSelection As Boolean = False
        ''' <summary>Default styling for SelectEntity entities. Note: loaded from UI theme xml file.</summary>
        Public Shared DefaultEntityStyle As New StyleSheet
        ''' <summary>Default styling for the SelectEntity itself. Note: loaded from UI theme xml file.</summary>
        Public Shared Shadows DefaultStyle As New StyleSheet
        ''' <summary>Locked item-index is based on the List property.</summary>
        Property LockedItems As New Dictionary(Of Integer, Boolean)
        ''' <summary>If provided, will Not be able to add more than this number of items.</summary>
        Public MaxItems As Integer = 0
        ''' <summary>Default SelectEntity size in pixels.</summary>
        Public Shared Shadows DefaultSize As Vector2 = New Vector2(360.0F, 320.0F)
        Public BorderColor As Color = Color.White
        Public BorderWidth As Integer = 2

        ''' <summary>The item's width will be used to scale it to fit the SelectEnity.</summary>
        Property ScaleToFit As Boolean = True
        Property MultiSelect As Boolean = False
        Property List As New List(Of Entity)
        ''' <summary>Create the SelectEntity.</summary>
        ''' <param name="size">List size.</param>
        ''' <param name="anchor">Position anchor.</param>
        ''' <param name="offset">Offset from anchor position.</param>
        ''' <param name="skin">SelectEnity skin, eg which texture to use.</param>
        Public Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing, Optional Skin As SelectEntitySkin = SelectEntitySkin.Default)
            MyBase.New(Size, anchor:=Anchor, offset:=Offset)
            Initialize()
        End Sub
        ''' <summary>Create the SelectEntity with default values.</summary>
        ''' <param name="anchor">Position anchor.</param>
        ''' <param name="offset">Offset from anchor position.</param>
        Public Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing)
            Me.New(USE_DEFAULT_SIZE, Anchor, Offset)
            Size = DefaultSize
            Initialize()
        End Sub
        Private Sub Initialize()
            UpdateStyle(DefaultStyle)
            _Scrollbar = New VerticalScrollbar(0, 10, Anchor.CenterRight)
            _Scrollbar.Value = 0
            _Scrollbar.OnValueChange = Sub() Scrollbar_OnValueChange()
            _Scrollbar.Visible = False
            AddChild(_Scrollbar, False)

            'Moves focus to scrollbar so we can scroll while mouseover
            OnMouseEnter = Sub() UserInterface.Active.ActiveEntity = _Scrollbar
        End Sub
        ''' <summary>Move items in and out of visibility when scrolling.</summary>
        Private Sub Scrollbar_OnValueChange()
            OnResize()
        End Sub
        Private Sub ScaleVisibleItems()
            ScaleMany(VisibleItems)
        End Sub
        Private Sub ScaleAllItems()
            ScaleMany(List)
        End Sub
        Private Sub ScaleMany(Source As IEnumerable(Of Entity))
            If ScaleToFit Then
                Dim ScaleSize As Vector2 = Size
                If _Scrollbar.IsVisible Then
                    ScaleSize.X -= _Scrollbar.Size.X
                End If

                Parallel.ForEach(Source, Sub(Item) Item.Size *= (ScaleSize.X / Item.Size.X))
            End If
        End Sub
        Private Sub ScaleSingle(Item As Entity)
            If ScaleToFit Then
                Dim ScaleSize As Vector2 = Size
                If _Scrollbar.IsVisible Then
                    ScaleSize.X -= _Scrollbar.Size.X
                End If

                Item.Size *= (ScaleSize.X / Item.Size.X)
            End If
        End Sub

        ''' <summary>Called every frame before drawing Is done.</summary>
        ''' <param name="spriteBatch">SpriteBatch to draw on.</param>
        Protected Overrides Sub OnBeforeDraw(SpriteBatch As SpriteBatch)
            MyBase.OnBeforeDraw(SpriteBatch)

            If _HadResizeWhileNotVisible Then
                OnResize()
            End If
        End Sub
        ''' <summary>Draw the entity.</summary>
        ''' <param name="spriteBatch">Sprite batch to draw on.</param>
        ''' <param name="phase">The phase we are currently drawing.</param>
        Protected Overrides Sub DrawEntity(SpriteBatch As SpriteBatch, Phase As DrawPhase)
            If (_PrevSize.Y <> _destRectInternal.Size.Y) OrElse _HadResizeWhileNotVisible Then
                _PrevSize = _destRectInternal.Size
                OnResize()
            End If

            MyBase.DrawEntity(SpriteBatch, Phase)
        End Sub
        ''' <summary>When list Is resized (also called on init), create the entities to show item values And init graphical stuff.</summary>
        Protected Overridable Sub OnResize()
            If (Not IsVisible()) Then
                _HadResizeWhileNotVisible = True
                Exit Sub
            End If

            _HadResizeWhileNotVisible = False
            ClearChildren()
            VisibleItems.Clear()
            UpdateDestinationRects()

            For i As Integer = 0 To (_List.Count - 1)
                Dim Entity As Entity = _List(i)

                If (i < ScrollPosition) Then
                    Continue For
                End If

                With Entity
                    .PromiscuousClicksMode = True
                    .UpdateStyle(DefaultEntityStyle)
                    .Scale *= ItemsScale
                    .SpaceAfter = New Vector2(0, ExtraSpaceBetweenLines + 4)
                    'Ensure that we can still select items in the blank space between items
                    .ExtraMargin.Y = CInt((ExtraSpaceBetweenLines + 4) / 2)
                    .AttachedData = New ParagraphData(Me, i)
                    .UseActualSizeForCollision = False
                    ScaleSingle(Entity)
                    .PropagateEventsTo(Me)
                    AddChild(Entity)
                    VisibleItems.Add(Entity)
                    .OnClick = Sub() Item_OnClick(Entity)
                    .UpdateDestinationRects()
                    'Ensures that the marker doesn't move when resizing the entity.
                    If Selected.ContainsKey(i) Then
                        ResetMarker(Entity)
                    End If
                    Dim IsLocked As Boolean = False
                    LockedItems.TryGetValue(i, IsLocked)
                    .Locked = IsLocked

                    'TODO : When new items are added, the last item is not displayed, even tho there is room for it, it will reappear on scrolling back and forth
                    If (.GetActualDestRect.Bottom > (_destRect.Bottom - _scaledPadding.Y)) Then
                        Hide(Entity)
                        Exit For
                    End If
                End With
            Next

            If (VisibleItems.Any AndAlso (VisibleItems.Count < _List.Count)) Then
                AddChild(_Scrollbar, False)
                _Scrollbar.Max = CUInt(_List.Count - VisibleItems.Count)

                If (_Scrollbar.Max < 2) Then
                    _Scrollbar.Max = 2
                End If

                _Scrollbar.StepsCount = _Scrollbar.Max
                _Scrollbar.Visible = True
                ScaleAllItems()
            Else
                _Scrollbar.Visible = False
                ScrollPosition = 0
            End If
        End Sub
        Private Sub Item_OnClick(Item As Entity)
            Dim Data As ParagraphData = DirectCast(Item.AttachedData, ParagraphData)

            If (Not Data.SelectEnity.LockSelection) Then
                Data.SelectEnity.SelectToggle(Data.Index)
            End If
        End Sub

        ''' <summary>Add value to list.</summary>
        ''' <param name="value">Value to add.</param>
        Public Sub AddItem(Value As Entity)
            If ((MaxItems <> 0) AndAlso (Count >= MaxItems)) Then
                Exit Sub
            End If

            _List.Add(Value)
            LockedItems.Add((List.Count + 1), Value.IsLocked)
            OnListChanged()
        End Sub
        ''' <summary>Add value to list at a specific index.</summary>
        ''' <param name="value">Value to add.</param>
        ''' <param name="index">Index to insert the New item into.</param>
        Public Sub AddItem(Value As Entity, Index As Integer)
            If ((MaxItems <> 0) AndAlso (Count >= MaxItems)) Then
                Exit Sub
            End If

            _List.Insert(Index, Value)
            LockedItems.Add(Index, Value.IsLocked)
            OnListChanged()
        End Sub

        ''' <summary>Add a range of values. Triggers only one OnListChanged event compared to adding a single item.</summary>
        Public Sub AddRange(Values As IEnumerable(Of Entity))
            For Each Value In Values
                If ((MaxItems <> 0) AndAlso (Count >= MaxItems)) Then
                    Exit For
                End If

                _List.Add(Value)
                LockedItems.Add((List.Count + 1), Value.IsLocked)
            Next

            OnListChanged()
        End Sub
        ''' <summary>Remove item from the list, by index.</summary>
        ''' <param name="index">Index of the item to remove.</param>
        Public Sub RemoveItem(Index As Integer)
            _List.RemoveAt(Index)
            LockedItems.Remove(Index)
            OnListChanged()
        End Sub
        ''' <summary>Remove value from the list.</summary>
        ''' <param name="value">Value to remove.</param>
        Public Sub RemoveItem(Value As Entity)
            _List.Remove(Value)
            LockedItems.Remove(Selected.First(Function(F) F.Value Is Value).Key)
            OnListChanged()
        End Sub

        ''' <summary>
        ''' Replaces the existing old value with the new one.
        ''' </summary>
        Public Sub ReplaceItem(Old As Entity, [New] As Entity)
            Dim Oldex = _List.IndexOf(Old)

            If (Oldex > -1) Then
                ReplaceItem(Oldex, [New])
            End If
        End Sub
        ''' <summary>
        ''' Replaces the existing item with the new one at a specified index.
        ''' </summary>
        Public Sub ReplaceItem(Index As Integer, [New] As Entity)
            Dim WasSelected = IsSelected(Index)

            _List.RemoveAt(Index)
            LockedItems.Remove(Index)

            _List.Insert(Index, [New])
            LockedItems.Add(Index, [New].IsLocked)

            If WasSelected Then
                Selected.Remove(Index)
                ApplyMarker(_List(Index))
                Selected.Add(Index, _List(Index))
            End If

            OnListChanged()
        End Sub
        ''' <summary>Replaces many items. Triggers only one OnListChanged event compared to replacing a single item.</summary>
        Public Sub ReplaceMany(Items As IDictionary(Of Integer, Entity))
            For Each Item In Items
                Dim WasSelected = IsSelected(Item.Key)

                'Ensure that we are checking a valid index, if it doesn't exist, continue the process.
                If (_List.Count > Item.Key) Then
                    _List.RemoveAt(Item.Key)
                    LockedItems.Remove(Item.Key)
                End If

                _List.Insert(Item.Key, Item.Value)
                LockedItems.Add(Item.Key, Item.Value.IsLocked)

                If WasSelected Then
                    Selected.Remove(Item.Key)
                    ApplyMarker(_List(Item.Key))
                    Selected.Add(Item.Key, _List(Item.Key))
                End If
            Next

            OnListChanged()
        End Sub

        Private Sub OnListChanged()
            If (_parent IsNot Nothing) Then
                OnResize()
            End If

            OnListChange?.Invoke(Me)
        End Sub
        Public Property ScrollPosition As Integer
            Get
                Return _Scrollbar.Value
            End Get
            Set
                _Scrollbar.Value = Value
            End Set
        End Property

        ''' <summary>Select item by value.</summary>
        ''' <param name="value">Item value to select.</param>
        Sub [Select](Value As Entity)
            If (Value Is Nothing) OrElse Selected.ContainsValue(Value) Then
                Exit Sub
            End If

            Dim Index = _List.IndexOf(Value)
            If (Index = -1) Then
                If UserInterface.Active.SilentSoftErrors Then
                    Exit Sub
                End If
                Throw New Exceptions.NotFoundException("Value doesn't exist in List!")
            End If

            CheckMultiSelect()
            ApplyMarker(Value)
            Selected.Add(Index, Value)
            DoOnValueChange()
        End Sub

        ''' <summary>Select item by index.</summary>
        ''' <param name="index">Item index to select.</param>
        ''' <param name="relativeToScrollbar">If true, index will be relative to scrollbar current position.</param>
        Sub [Select](Index As Integer, Optional RelativeToScrollbar As Boolean = False)
            If RelativeToScrollbar Then
                Index += ScrollPosition
            End If

            If (Index >= -1) Then
                If (Index >= _List.Count) Then
                    If UserInterface.Active.SilentSoftErrors Then
                        Exit Sub
                    End If
                    Throw New Exceptions.NotFoundException("Index was out of range!")
                End If

                CheckMultiSelect()
                ApplyMarker(_List(Index))
                Selected.Add(Index, _List(Index))
            End If

            DoOnValueChange()
        End Sub

        ''' <summary>If not selected, select. If selected, deselect.</summary>
        Public Sub SelectToggle(Index As Integer, Optional RelativeToScrollbar As Boolean = False)
            If RelativeToScrollbar Then
                Index += ScrollPosition
            End If

            If (Index >= -1) Then
                If (Index >= _List.Count) Then
                    If UserInterface.Active.SilentSoftErrors Then
                        Exit Sub
                    End If
                    Throw New Exceptions.NotFoundException("Index was out of range!")
                End If

                Dim WasSelected = IsSelected(Index)
                CheckMultiSelect()

                If (Not WasSelected) Then
                    ApplyMarker(_List(Index))
                    Selected.Add(Index, _List(Index))
                Else
                    Deselect(Index)
                End If
            End If

            DoOnValueChange()
        End Sub

        ''' <summary>Select item by index. Triggers only one DoOnValueChange event compared to selecting a single item.</summary>
        Public Sub SelectMany(Indexes As List(Of Integer))
            If (Indexes IsNot Nothing) Then
                For Each i In Indexes
                    Dim Value = _List(i)

                    CheckMultiSelect()
                    ApplyMarker(Value)
                    Selected.Add(i, Value)
                Next

                DoOnValueChange()
            End If
        End Sub

        ''' <summary>If MultiSelect is false, clears all selections.</summary>
        Private Sub CheckMultiSelect()
            If (Not MultiSelect) Then
                DeselectAll()
            End If
        End Sub
        Public Function IsSelected(Index As Integer) As Boolean
            Return Selected.ContainsKey(Index)
        End Function
        ''' <summary>Deselects item by index.</summary>
        Public Sub Deselect(Index As Integer)
            Selected.Remove(Index)
            RemoveMarker(Index)
        End Sub
        ''' <summary>Deselects item by value.</summary>
        Public Sub Deselect(Value As Entity)
            Deselect(Selected.First(Function(F) F.Value Is Value).Key)
        End Sub
        Public Sub DeselectAll()
            RemoveMarkers()
            Selected.Clear()
        End Sub
        Private Sub ApplyMarker(Value As Entity)
            Dim Border As New Border(BorderColor, BorderWidth, Value.Size)
            Border.Identifier = Marker
            Value.AddChild(Border)
            Border.BringToFront()
        End Sub
        Private Sub ResetMarker(Value As Entity)
            RemoveMarker(Value)
            ApplyMarker(Value)
        End Sub
        Private Sub RemoveMarker(Item As Entity)
            For Each Child In Item.GetChildren
                If (Child.Identifier = Marker) Then
                    Child.RemoveFromParent()
                    Exit For
                End If
            Next
        End Sub
        Private Sub RemoveMarker(Index As Integer)
            RemoveMarker(List(Index))
        End Sub
        ''''' <summary>Removes selection marker from all items.</summary>
        ''' <param name="Force">If all items should be checked for a marker, instead of only selected.</param>
        Private Sub RemoveMarkers(Optional Force As Boolean = False)
            If (Not Force) Then
                Parallel.ForEach(Selected, Sub(Item) RemoveMarker(List(Item.Key)))
            Else
                Parallel.ForEach(List, Sub(Item) RemoveMarker(Item))
            End If
        End Sub
        Private Sub Hide(Index As Integer)
            VisibleItems(Index).RemoveFromParent()
            VisibleItems.RemoveAt(Index)
        End Sub
        Private Sub Hide(Item As Entity)
            Item.RemoveFromParent()
            VisibleItems.Remove(Item)
        End Sub
        ''' <summary>
        ''' Move scrollbar to currently selected item.
        ''' If MultiSelect is true, moves to first item.
        ''' </summary>
        Public Sub ScrollToSelected()
            If ((_Scrollbar IsNot Nothing) AndAlso _Scrollbar.Visible AndAlso Selected.Any) Then
                ScrollPosition = Selected.First.Key
            End If
        End Sub

        ''' <summary>Move scrollbar to first item in list.</summary>
        Public Sub ScrollToTop()
            If ((_Scrollbar IsNot Nothing) AndAlso _Scrollbar.Visible AndAlso Selected.Any) Then
                ScrollPosition = Selected.Keys.Min
            End If
        End Sub
        ''' <summary>Move scrollbar to last item in list.</summary>
        Public Sub ScrollToEnd()
            If ((_Scrollbar IsNot Nothing) AndAlso _Scrollbar.Visible AndAlso Selected.Any) Then
                ScrollPosition = Selected.Keys.Max
            End If
        End Sub

        ReadOnly Property SelectedIndexes As List(Of Integer)
            Get
                Return If(Selected.Any, Selected.Keys.ToList, Nothing)
            End Get
        End Property

        ReadOnly Property SelectedValues As List(Of Entity)
            Get
                Return If(Selected.Any, Selected.Values.ToList, Nothing)
            End Get
        End Property
        ReadOnly Property NotSelectedIndexes As List(Of Integer)
            Get
                If (Not List.Any) Then
                    Return Nothing
                End If

                Dim Result As New List(Of Integer)
                For i = 0 To (List.Count - 1)
                    If (Not IsSelected(i)) Then
                        Result.Add(i)
                    End If
                Next

                Return Result
            End Get
        End Property
        ReadOnly Property NotSelectedValues As List(Of Entity)
            Get
                If (Not List.Any) Then
                    Return Nothing
                End If

                Dim Result As New List(Of Entity)
                For i = 0 To (List.Count - 1)
                    If (Not IsSelected(i)) Then
                        Result.Add(List(i))
                    End If
                Next

                Return Result
            End Get
        End Property
        ''' <summary>Remove all items from all lists.</summary>
        Public Sub ClearItems()
            List.Clear()
            LockedItems.Clear()
            VisibleItems.Clear()
            Selected.Clear()
            ClearChildren()

            OnListChanged()
        End Sub
        ''' <summary>Count of items in list.</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return _List.Count
            End Get
        End Property

        ''' <summary>Is the list currently empty.</summary>
        Public ReadOnly Property Empty As Boolean
            Get
                Return (_List.Count = 0)
            End Get
        End Property

        ''' <summary>Is the list a natrually-interactable entity.</summary>
        ''' <returns>True</returns>
        Public Overrides Function IsNaturallyInteractable() As Boolean
            Return True
        End Function
        Public Overrides Sub UpdateDestinationRects()
            _destRect = CalcDestRect()
            _destRectInternal = CalcInternalRect()
            _isDirty = False
            _destRectVersion += 1

            'If (_parent IsNot Nothing) Then
            '_parentLastDestRectVersion = _parent._destRectVersion
            'End If
        End Sub
    End Class

End Namespace