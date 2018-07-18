Imports System
Imports System.Collections.Generic
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions

    ''' <summary>
    ''' <para>Used to display animated images, supported formats are : Gif, WebP.</para>
    ''' <para>Can also be used to display a gallery with given intervals.</para>
    ''' </summary>
    Public Class ImageAnimated
        Inherits Entity

        Property Looping As Boolean
        Property HideControls As Boolean
        Property IsGallery As Boolean
        Property Gallery As List(Of AnimatedObject)
        ''' <summary>How long should each piece be displayed. If 0 will display for full duration.</summary>
        Property GalleryTimePerPiece As Single
        ''' <summary>How long should pass after a piece finish before moving on to the next piece.</summary>
        Property GalleryTimeWaitAfterPiece As Single

        ''' <summary>Default Animated size in pixels.</summary>
        Public Shared Shadows DefaultSize As Vector2 = New Vector2(400.0F, 300.0F)
        ''' <param name="HideControls">Hides the controls(pause/play).</param>
        Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2 = Nothing, Optional HideControls As Boolean = False)
            MyBase.New(USE_DEFAULT_SIZE, anchor:=Anchor, offset:=Offset)
            Size = DefaultSize
            Initialize()
        End Sub
        ''' <param name="HideControls">Hides the controls(pause/play).</param>
        Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2 = Nothing, Optional HideControls As Boolean = False)
            MyBase.New(Size, anchor:=Anchor, offset:=Offset)
            Initialize()
        End Sub

        Private Sub Initialize()

        End Sub

    End Class

    ''' <summary>
    ''' Holds everything needed to display an animated image in an ImageAnimated entity.
    ''' </summary>
    Public Class AnimatedObject
        Inherits Fingerprint

        Property Looping As Boolean
        Property Interval As Single
        Property Frames As Integer
        ''' <summary>Allows frames to be omitted when playing.</summary>
        Property DropInterval As Integer
        Property ScaleToFit As Boolean
        Property Size As Vector2
        Property Duration As TimeSpan
        Property Path As String
        Property Format As AnimatedFormats
        Property RAW As Byte()

    End Class

    Public Enum AnimatedFormats
        Gif
        WebP
    End Enum

End Namespace