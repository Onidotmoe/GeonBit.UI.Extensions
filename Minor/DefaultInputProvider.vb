Imports System
Imports GeonBit.UI
Imports Microsoft
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Namespace Extensions.Minor
    Public Enum SpecialChars
        Null = 0
        ArrowLeft = 1
        ArrowRight = 2
        ArrowUp = 3
        ArrowDown = 4
        Backspace = 8
        Space = 32
        Delete = 127
    End Enum
    ''' <summary>
    ''' Implement Mouse Input And Keyboard Input for GeonBit.UI + provide some helpful utils you can use externally.
    ''' This Is the object we provide to GeonBit.UI by default, if no other input providers were set by user.
    ''' </summary>
    Public Class DefaultInputProvider
        Implements Extensions.Minor.IMouseInput
        Implements IKeyboardInput

        ' store current & previous keyboard states so we can detect key release
        Private _newKeyboardState As KeyboardState
        Private _oldKeyboardState As KeyboardState
        ' store current & previous mouse states so we can detect key release And diff
        Private _newMouseState As MouseState
        Private _oldMouseState As MouseState
        Private _newMousePos As Vector2
        Private _oldMousePos As Vector2

        ' store current frame gametime
        Private _currTime As GameTime

        ' store all Key values in lookup array
        Private _allKeyValues As Keys()
        ''' <summary>An artificial "lag" after a key Is pressed when typing text input, to prevent mistake duplications.</summary>

        Public KeysTypeCooldown As Single = 0.6F
        ' last character that was pressed down

        Private _currCharacterInput As Char = VisualBasic.ChrW(SpecialChars.Null)
        ' last key that provide character input And was pressed

        Private _currCharacterInputKey As Keys = Keys.Escape
        ' keyboard input cooldown for textual input
        Private _keyboardInputCooldown As Single = 0F

        ' true when a New keyboard key Is pressed
        Private _newKeyIsPressed As Boolean = False
        ' current capslock state
        Private _capslock As Boolean = False
        ''' <summary>
        ''' Current mouse wheel value.
        ''' </summary>
        Public Property MouseWheel As Integer Implements IMouseInput.MouseWheel

        ''' <summary>
        ''' Mouse wheel change sign (eg 0, 1 Or -1) since last frame.
        ''' </summary>
        Public Property MouseWheelChange As Integer Implements IMouseInput.MouseWheelChange
        ''' <summary>
        ''' Create the input helper.
        ''' </summary>
        Public Sub New()
            _allKeyValues = CType(System.[Enum].GetValues(GetType(Keys)), Keys())

            ' init keyboard states
            _newKeyboardState = _oldKeyboardState
            ' init mouse states
            _newMouseState = _oldMouseState
            _newMousePos = New Vector2(_newMouseState.X, _newMouseState.Y)

            ' call first update to get starting positions
            Update(New GameTime())
        End Sub
        ''' <summary>
        ''' Current frame game time.
        ''' </summary>
        Public ReadOnly Property CurrGameTime As GameTime
            Get
                Return _currTime
            End Get
        End Property
        ''' <summary>
        ''' Update current states.
        ''' If used outside GeonBit.UI, this function should be called first thing inside your game 'Update()' function,
        ''' And before you make any use of this class.
        ''' </summary>
        ''' <param name="gameTime">Current game time.</param>
        Public Sub Update(ByVal gameTime As GameTime) Implements IKeyboardInput.Update, IMouseInput.Update
            ' store game time
            _currTime = gameTime
            ' store previous states
            _oldMouseState = _newMouseState
            _oldKeyboardState = _newKeyboardState
            ' get New states
            _newMouseState = Mouse.GetState()
            _newKeyboardState = Keyboard.GetState()
            ' get mouse position
            _oldMousePos = _newMousePos
            _newMousePos = New Vector2(_newMouseState.X, _newMouseState.Y)
            ' get mouse wheel state
            Dim prevMouseWheel As Integer = MouseWheel
            MouseWheel = _newMouseState.ScrollWheelValue
            MouseWheelChange = System.Math.Sign(MouseWheel - prevMouseWheel)

            ' update capslock state
            If _newKeyboardState.IsKeyDown(Keys.CapsLock) AndAlso Not _oldKeyboardState.IsKeyDown(Keys.CapsLock) Then
                _capslock = Not _capslock
            End If

            ' decrease keyboard input cooldown time
            If _keyboardInputCooldown > 0F Then
                _newKeyIsPressed = False
                _keyboardInputCooldown -= CSng(gameTime.ElapsedGameTime.TotalSeconds)
            End If

            ' if current text input key Is no longer down, reset text input character
            If _currCharacterInput <> VisualBasic.ChrW(SpecialChars.Null) AndAlso Not _newKeyboardState.IsKeyDown(_currCharacterInputKey) Then
                _currCharacterInput = VisualBasic.ChrW(SpecialChars.Null)
            End If

            ' send key-down events
            For Each key In _allKeyValues

                If _newKeyboardState.IsKeyDown(key) AndAlso Not _oldKeyboardState.IsKeyDown(key) Then
                    OnKeyPressed(key)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Move the cursor to be at the center of the screen.
        ''' </summary>
        ''' <param name="pos">New mouse position.</param>
        Public Sub UpdateMousePosition(ByVal pos As Vector2) Implements IMouseInput.UpdateMousePosition
            ' move mouse position back to center
            Mouse.SetPosition(CInt(pos.X), CInt(pos.Y))
            _newMousePos = pos
            _oldMousePos = pos
        End Sub

        ''' <summary>
        ''' Calculate And return current cursor position transformed by a matrix.
        ''' </summary>
        ''' <param name="transform">Matrix to transform cursor position by.</param>
        ''' <returns>Cursor position with optional transform applied.</returns>
        Public Function TransformMousePosition(ByVal transform As Matrix?) As Vector2 Implements IMouseInput.TransformMousePosition
            Dim newMousePos = _newMousePos

            If transform IsNot Nothing Then
                Return Vector2.Transform(newMousePos, transform.Value) - New Vector2(transform.Value.Translation.X, transform.Value.Translation.Y)
            End If

            Return newMousePos
        End Function

        ''' <summary>
        ''' Called every time a keyboard key Is pressed (called once on the frame key was pressed).
        ''' </summary>
        ''' <param name="key">Key code that Is being pressed on this frame.</param>
        Protected Sub OnKeyPressed(ByVal key As Keys)
            NewKeyTextInput(key)
        End Sub

        ''' <summary>
        ''' This update the character the user currently type down, for text input.
        ''' This function called whenever a New key Is pressed down, And becomes the current input
        ''' until it Is released.
        ''' </summary>
        ''' <param name="key">Key code that Is being pressed down on this frame.</param>
        Private Sub NewKeyTextInput(ByVal key As Keys)
            ' reset cooldown time And set New key pressed = true
            _keyboardInputCooldown = KeysTypeCooldown
            _newKeyIsPressed = True
            ' get if shift Is currently down
            Dim isShiftDown As Boolean = _newKeyboardState.IsKeyDown(Keys.LeftShift) OrElse _newKeyboardState.IsKeyDown(Keys.RightShift)

            ' set curr input key, but also keep the previous key in case we need to revert
            Dim prevKey As Keys = _currCharacterInputKey
            _currCharacterInputKey = key

            ' handle special keys And characters
            Select Case key
                Case Keys.Space
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.Space)
                    Return
                Case Keys.Left
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.ArrowLeft)
                    Return
                Case Keys.Right
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.ArrowRight)
                    Return
                Case Keys.Up
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.ArrowUp)
                    Return
                Case Keys.Down
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.ArrowDown)
                    Return
                Case Keys.Delete
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.Delete)
                    Return
                Case Keys.Back
                    _currCharacterInput = VisualBasic.ChrW(SpecialChars.Backspace)
                    Return
                Case Keys.CapsLock, Keys.RightShift, Keys.LeftShift
                    _newKeyIsPressed = False
                    Return

                ' line break
                Case Keys.Enter
                    _currCharacterInput = CChar(VisualBasic.vbLf)
                    Return

                ' number 0
                Case Keys.D0, Keys.NumPad0
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D0), ")"c, "0"c)
                    Return
                ' number 9
                Case Keys.D9, Keys.NumPad9
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D9), "("c, "9"c)
                    Return
                ' number 8
                Case Keys.D8, Keys.NumPad8
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D8), "*"c, "8"c)
                    Return
                ' number 7
                Case Keys.D7, Keys.NumPad7
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D7), "&"c, "7"c)
                    Return
                ' number 6
                Case Keys.D6, Keys.NumPad6
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D6), "^"c, "6"c)
                    Return
                ' number 5
                Case Keys.D5, Keys.NumPad5
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D5), "%"c, "5"c)
                    Return
                ' number 4
                Case Keys.D4, Keys.NumPad4
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D4), "$"c, "4"c)
                    Return
                ' number 3
                Case Keys.D3, Keys.NumPad3
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D3), "#"c, "3"c)
                    Return
                ' number 2
                Case Keys.D2, Keys.NumPad2
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D2), "@"c, "2"c)
                    Return
                ' number 1
                Case Keys.D1, Keys.NumPad1
                    _currCharacterInput = If((isShiftDown AndAlso key = Keys.D1), "!"c, "1"c)
                    Return
                ' question mark
                Case Keys.OemQuestion
                    _currCharacterInput = If(isShiftDown, "?"c, "/"c)
                    Return
                ' quotes
                Case Keys.OemQuotes
                    _currCharacterInput = If(isShiftDown, """"c, "'"c)
                    Return
                ' semicolon
                Case Keys.OemSemicolon
                    _currCharacterInput = If(isShiftDown, ":"c, ";"c)
                    Return
                ' tilde
                Case Keys.OemTilde
                    _currCharacterInput = If(isShiftDown, "~"c, "`"c)
                    Return
                ' open brackets
                Case Keys.OemOpenBrackets
                    _currCharacterInput = If(isShiftDown, "{"c, "["c)
                    Return
                ' close brackets
                Case Keys.OemCloseBrackets
                    _currCharacterInput = If(isShiftDown, "}"c, "]"c)
                    Return
                ' add
                Case Keys.OemPlus, Keys.Add
                    _currCharacterInput = If((isShiftDown OrElse key = Keys.Add), "+"c, "="c)
                    Return
                ' substract
                Case Keys.OemMinus, Keys.Subtract
                    _currCharacterInput = If(isShiftDown, "_"c, "-"c)
                    Return
                ' decimal dot
                Case Keys.OemPeriod, Keys.Decimal
                    _currCharacterInput = If(isShiftDown, ">"c, "."c)
                    Return
                ' divide
                Case Keys.Divide
                    _currCharacterInput = If(isShiftDown, "?"c, "/"c)
                    Return
                ' multiply
                Case Keys.Multiply
                    _currCharacterInput = "*"c
                    Return
                ' backslash
                Case Keys.OemBackslash
                    _currCharacterInput = If(isShiftDown, "|"c, "\"c)
                    Return
                ' comma
                Case Keys.OemComma
                    _currCharacterInput = If(isShiftDown, "<"c, ","c)
                    Return
                ' tab
                Case Keys.Tab
                    _currCharacterInput = " "c
                    Return
                    ' Not a special char - revert last character input key And continue processing.
                Case Else
                    _currCharacterInputKey = prevKey
            End Select

            ' get current key thats getting pressed as a string
            Dim lastCharPressedStr As String = key.ToString()
            ' if character Is Not a valid char but a special key we don't want to handle, skip
            ' (note: Keys that are characters should have length of 1)
            If lastCharPressedStr.Length > 1 Then
                Return
            End If
            ' set current key as the current text input key
            _currCharacterInputKey = key
            ' get current capslock state And invert if shift Is down
            Dim capsLock As Boolean = _capslock

            If isShiftDown Then
                capsLock = Not capsLock
            End If
            ' fix case And set as current char pressed
            _currCharacterInput = (If(capsLock, lastCharPressedStr.ToUpper(), lastCharPressedStr.ToLower()))(0)
        End Sub
        ''' <summary>
        ''' Get textual input from keyboard.
        ''' If user enter keys it will push them into string, if delete Or backspace will remove chars, etc.
        ''' This also handles keyboard cooldown, to make it feel Like windows-input.
        ''' </summary>
        ''' <param name="txt">String to push text input into.</param>
        ''' <param name="lineWidth">How many characters can fit in a line.</param>
        ''' <param name="pos">Position to insert / remove characters. -1 to push at the end of string. After done, will contain actual New caret position.</param>
        ''' <returns>String after text input applied on it.</returns>
        Public Function GetTextInput(ByVal txt As String, ByVal lineWidth As Integer, ByRef pos As Integer) As String Implements IKeyboardInput.GetTextInput
            ' if need to skip due to cooldown time
            If Not _newKeyIsPressed AndAlso _keyboardInputCooldown > 0F Then
                Return txt
            End If
            ' if no valid characters are currently input
            If _currCharacterInput = VisualBasic.ChrW(SpecialChars.Null) Then
                Return txt
            End If
            ' get default position
            If pos = -1 Then
                pos = txt.Length
            End If
            ' handle special chars
            Select Case _currCharacterInput
                Case VisualBasic.ChrW(SpecialChars.ArrowLeft)

                    If System.Threading.Interlocked.Decrement(pos) < 0 Then
                        pos = 0
                    End If

                    Return txt
                Case VisualBasic.ChrW(SpecialChars.ArrowRight)

                    If System.Threading.Interlocked.Increment(pos) > txt.Length Then
                        pos = txt.Length
                    End If

                    Return txt
                Case VisualBasic.ChrW(SpecialChars.ArrowUp)
                    pos -= lineWidth

                    If pos < 0 Then
                        pos = 0
                    End If

                    Return txt
                Case VisualBasic.ChrW(SpecialChars.ArrowDown)
                    pos += lineWidth

                    If pos > txt.Length Then
                        pos = txt.Length
                    End If

                    Return txt
                Case VisualBasic.ChrW(SpecialChars.Backspace)
                    pos -= 1
                    Return If((pos < txt.Length AndAlso pos >= 0 AndAlso txt.Length > 0), txt.Remove(pos, 1), txt)
                Case VisualBasic.ChrW(SpecialChars.Delete)
                    Return If((pos < txt.Length AndAlso txt.Length > 0), txt.Remove(pos, 1), txt)
            End Select

            ' add current character
            Return txt.Insert(Math.Min(System.Threading.Interlocked.Increment(pos), pos - 1), _currCharacterInput.ToString())
        End Function
        ''' <summary>
        ''' Get current mouse poisition.
        ''' </summary>
        Public ReadOnly Property MousePosition As Vector2 Implements IMouseInput.MousePosition
            Get
                Return _newMousePos
            End Get
        End Property
        ''' <summary>
        ''' Get mouse position change since last frame.
        ''' </summary>
        ''' <return>Mouse position change as a 2d vector.</return>
        Public ReadOnly Property MousePositionDiff As Vector2 Implements IMouseInput.MousePositionDiff
            Get
                Return _newMousePos - _oldMousePos
            End Get
        End Property
        ''' <summary>
        ''' Check if a given mouse button Is down.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button Is down.</return>
        Public Function MouseButtonDown(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean Implements IMouseInput.MouseButtonDown
            Return GetMouseButtonState(button) = ButtonState.Pressed
        End Function

        ''' <summary>
        ''' Return if any of mouse buttons Is down.
        ''' </summary>
        ''' <returns>True if any mouse button Is currently down.</returns>
        Public Function AnyMouseButtonDown() As Boolean Implements IMouseInput.AnyMouseButtonDown
            Return MouseButtonDown(MouseButton.Left) OrElse MouseButtonDown(MouseButton.Right) OrElse MouseButtonDown(MouseButton.Middle) OrElse MouseButtonDown(MouseButton.XButton1) OrElse MouseButtonDown(MouseButton.XButton2)
        End Function
        ''' <summary>
        ''' Check if a given mouse button was released in current frame.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button was released in this frame.</return>
        Public Function MouseButtonReleased(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean Implements IMouseInput.MouseButtonReleased
            Return GetMouseButtonState(button) = ButtonState.Released AndAlso GetMousePreviousButtonState(button) = ButtonState.Pressed
        End Function
        ''' <summary>
        ''' Return if any mouse button was released this frame.
        ''' </summary>
        ''' <returns>True if any mouse button was released.</returns>
        Public Function AnyMouseButtonReleased() As Boolean Implements IMouseInput.AnyMouseButtonReleased
            Return MouseButtonReleased(MouseButton.Left) OrElse MouseButtonReleased(MouseButton.Right) OrElse MouseButtonReleased(MouseButton.Middle) OrElse MouseButtonReleased(MouseButton.XButton1) OrElse MouseButtonReleased(MouseButton.XButton2)
        End Function
        ''' <summary>
        ''' Check if a given mouse button was pressed in current frame.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button was pressed in this frame.</return>
        Public Function MouseButtonPressed(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean Implements IMouseInput.MouseButtonPressed
            Return GetMouseButtonState(button) = ButtonState.Pressed AndAlso GetMousePreviousButtonState(button) = ButtonState.Released
        End Function
        ''' <summary>
        ''' Return if any mouse button was pressed in current frame.
        ''' </summary>
        ''' <returns>True if any mouse button was pressed in current frame..</returns>
        Public Function AnyMouseButtonPressed() As Boolean Implements IMouseInput.AnyMouseButtonPressed
            Return MouseButtonPressed(MouseButton.Left) OrElse MouseButtonPressed(MouseButton.Right) OrElse MouseButtonPressed(MouseButton.Middle) OrElse MouseButtonPressed(MouseButton.XButton1) OrElse MouseButtonPressed(MouseButton.XButton2)
        End Function
        ''' <summary>
        ''' Check if a given mouse button was just clicked (eg released after being pressed down)
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button Is clicked.</return>
        Public Function MouseButtonClick(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean Implements IMouseInput.MouseButtonClick
            Return GetMouseButtonState(button) = ButtonState.Released AndAlso GetMousePreviousButtonState(button) = ButtonState.Pressed
        End Function
        ''' <summary>
        ''' Return if any of mouse buttons was clicked this frame.
        ''' </summary>
        ''' <returns>True if any mouse button was clicked.</returns>
        Public Function AnyMouseButtonClicked() As Boolean Implements IMouseInput.AnyMouseButtonClicked
            Return MouseButtonClick(MouseButton.Left) OrElse MouseButtonClick(MouseButton.Right) OrElse MouseButtonClick(MouseButton.Middle) OrElse MouseButtonClick(MouseButton.XButton1) OrElse MouseButtonClick(MouseButton.XButton2)
        End Function
        ''' <summary>
        ''' Return the state of a mouse button (up / down).
        ''' </summary>
        ''' <param name="button">Button to check.</param>
        ''' <returns>Mouse button state.</returns>
        Private Function GetMouseButtonState(ByVal Optional button As MouseButton = MouseButton.Left) As ButtonState
            Select Case button
                Case MouseButton.Left
                    Return _newMouseState.LeftButton
                Case MouseButton.Right
                    Return _newMouseState.RightButton
                Case MouseButton.Middle
                    Return _newMouseState.MiddleButton
                Case MouseButton.XButton1
                    Return _newMouseState.XButton1
                Case MouseButton.XButton2
                    Return _newMouseState.XButton2
            End Select

            Return ButtonState.Released
        End Function
        ''' <summary>
        ''' Return the state of a mouse button (up / down), in previous frame.
        ''' </summary>
        ''' <param name="button">Button to check.</param>
        ''' <returns>Mouse button state.</returns>
        Private Function GetMousePreviousButtonState(ByVal Optional button As MouseButton = MouseButton.Left) As ButtonState
            Select Case button
                Case MouseButton.Left
                    Return _oldMouseState.LeftButton
                Case MouseButton.Right
                    Return _oldMouseState.RightButton
                Case MouseButton.Middle
                    Return _oldMouseState.MiddleButton
                Case MouseButton.XButton1
                    Return _oldMouseState.XButton1
                Case MouseButton.XButton2
                    Return _oldMouseState.XButton2
            End Select

            Return ButtonState.Released
        End Function
        ''' <summary>
        ''' Check if a given keyboard key Is down.
        ''' </summary>
        ''' <param name="key">Key button to check.</param>
        ''' <return>True if given key button Is down.</return>
        Public Function IsKeyDown(ByVal key As Keys) As Boolean
            Return _newKeyboardState.IsKeyDown(key)
        End Function
        ''' <summary>
        ''' Check if a given keyboard key was previously pressed down And now released in this frame.
        ''' </summary>
        ''' <param name="key">Key button to check.</param>
        ''' <return>True if given key button was just released.</return>
        Public Function IsKeyReleased(ByVal key As Keys) As Boolean
            Return _oldKeyboardState.IsKeyDown(key) AndAlso _newKeyboardState.IsKeyUp(key)
        End Function

    End Class

End Namespace