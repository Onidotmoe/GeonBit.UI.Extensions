Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework

Namespace Extensions

    Class Console
        Inherits Panel

        Private TextInput As TextInput
        Private Log As Paragraph

        Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2? = Nothing)
            MyBase.New(Size, anchor:=Anchor, offset:=Offset)

        End Sub

        Sub Parse(Input As String)

        End Sub

        Sub Execute()

        End Sub
    End Class
    Module ConsoleCommands

    End Module

End Namespace