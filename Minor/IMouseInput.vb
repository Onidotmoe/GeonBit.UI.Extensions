Imports Microsoft.Xna.Framework

Namespace Extensions.Minor

    ''' <summary>
    ''' Mouse buttons.
    ''' </summary>
    Public Enum MouseButton
        '''<summary>Left mouse button.</summary>
        Left
        '''<summary>Right mouse button.</summary>
        Right
        '''<summary>Middle mouse button (scrollwheel when clicked).</summary>
        Middle
        ''' <summary>XButton1, Extra Mappable Button</summary>
        XButton1
        ''' <summary>XButton2, Extra Mappable Button</summary>
        XButton2
    End Enum

    ''' <summary>
    ''' Define the interface GeonBit.UI uses to get mouse Or mouse-Like input from users.
    ''' </summary>
    Public Interface IMouseInput

        ''' <summary>
        ''' Update input (called every frame).
        ''' </summary>
        ''' <param name="gameTime">Update frame game time.</param>
        Sub Update(ByVal gameTime As GameTime)

        ''' <summary>
        ''' Get current mouse poisition.
        ''' </summary>
        ReadOnly Property MousePosition As Vector2

        ''' <summary>
        ''' Get mouse position change since last frame.
        ''' </summary>
        ReadOnly Property MousePositionDiff As Vector2

        ''' <summary>
        ''' Move the cursor to be at the center of the screen.
        ''' </summary>
        ''' <param name="pos">New mouse position.</param>
        Sub UpdateMousePosition(ByVal pos As Vector2)

        ''' <summary>
        ''' Calculate And return current cursor position transformed by a matrix.
        ''' </summary>
        ''' <param name="transform">Matrix to transform cursor position by.</param>
        ''' <returns>Cursor position with optional transform applied.</returns>
        Function TransformMousePosition(ByVal transform As Matrix?) As Vector2

        ''' <summary>
        ''' Check if a given mouse button Is down.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button Is down.</return>
        Function MouseButtonDown(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean

        ''' <summary>
        ''' Return if any of mouse buttons Is down.
        ''' </summary>
        ''' <returns>True if any mouse button Is currently down.</returns>
        Function AnyMouseButtonDown() As Boolean
        ''' <summary>
        ''' Check if a given mouse button was pressed in current frame.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button was pressed in this frame.</return>
        Function MouseButtonPressed(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean

        ''' <summary>
        ''' Return if any mouse button was pressed in current frame.
        ''' </summary>
        ''' <returns>True if any mouse button was pressed in current frame.</returns>
        Function AnyMouseButtonPressed() As Boolean

        ''' <summary>
        ''' Check if a given mouse button was just clicked (eg released after being pressed down)
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button Is clicked.</return>
        Function MouseButtonClick(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean

        ''' <summary>
        ''' Return if any of mouse buttons was clicked this frame.
        ''' </summary>
        ''' <returns>True if any mouse button was clicked.</returns>
        Function AnyMouseButtonClicked() As Boolean
        ''' <summary>
        ''' Check if a given mouse button was released in current frame.
        ''' </summary>
        ''' <param name="button">Mouse button to check.</param>
        ''' <return>True if given mouse button was released in this frame.</return>
        Function MouseButtonReleased(ByVal Optional button As MouseButton = MouseButton.Left) As Boolean
        ''' <summary>
        ''' Return if any mouse button was released this frame.
        ''' </summary>
        ''' <returns>True if any mouse button was released.</returns>
        Function AnyMouseButtonReleased() As Boolean
        ''' <summary>
        ''' Current mouse wheel value.
        ''' </summary>
        ReadOnly Property MouseWheel As Integer

        ''' <summary>
        ''' Mouse wheel change sign (eg 0, 1 Or -1) since last frame.
        ''' </summary>
        ReadOnly Property MouseWheelChange As Integer
    End Interface
End Namespace