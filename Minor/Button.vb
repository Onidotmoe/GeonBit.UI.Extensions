Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Public Class Button
    Inherits Entities.Button

    Sub New(Optional Text As String = "", Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing, Optional Skin As ButtonSkin = ButtonSkin.Default)
        MyBase.New(Text, Skin, Anchor, Size, Offset)
    End Sub

End Class