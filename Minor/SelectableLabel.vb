Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions.Minor
    Public Class SelectableLabel
        Inherits Label

        Sub New(Optional Text As String = "", Optional Anchor As Anchor = Anchor.Auto, Optional Size As Vector2? = Nothing, Optional Offset As Vector2? = Nothing)
            MyBase.New(Text, Anchor, Size, Offset)
        End Sub

    End Class

End Namespace