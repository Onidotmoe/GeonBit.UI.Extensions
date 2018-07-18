Imports GeonBit.UI
Imports GeonBit.UI.DataTypes
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Graphics

Namespace Extensions
    Public Class ExternalExtender

        ''' <summary>SliderVerticals base textures.</summary>
        Public SliderVerticalTextures As TexturesGetter(Of SliderVerticalSkin) = New TexturesGetter(Of SliderVerticalSkin)("textures/SliderVertical_")
        ''' <summary>SliderVerticals mark textures (the sliding piece that shows current value).</summary>
        Public SliderVerticalMarkTextures As TexturesGetter(Of SliderVerticalSkin) = New TexturesGetter(Of SliderVerticalSkin)("textures/SliderVertical_", "_mark")
        ''' <summary>Metadata about SliderVertical textures.</summary>
        Public SliderVerticalData As TextureData()

        ''' <summary>Select2D base textures.</summary>
        Public Select2DTextures As TexturesGetter(Of Select2DSkin) = New TexturesGetter(Of Select2DSkin)("textures/Select2D_")
        ''' <summary>Select2D mark textures (the dot that shows current value).</summary>
        Public Select2DDotTextures As TexturesGetter(Of Select2DSkin) = New TexturesGetter(Of Select2DSkin)("textures/Select2D_", "_dot")
        ''' <summary>Metadata about Select2D textures.</summary>
        Public Select2DData As TextureData()

        ''' <summary>SelectEntity base textures.</summary>
        Public SelectEntityTextures As TexturesGetter(Of SelectEntitySkin) = New TexturesGetter(Of SelectEntitySkin)("textures/SelectEntity_")
        ''' <summary>Metadata about SelectEntity textures.</summary>
        Public SelectEntityData As TextureData()

        ''' <summary>Root for geonbit.ui content</summary>
        Private Root As String
        Public Transparent As Texture2D

        Sub Initialize(Content As ContentManager, Optional Theme As String = "Vision")
            Root = ("GeonBit.UI/themes/" & Theme & "/")

            LoadContent(Of SliderVerticalSkin)(Content, SliderVerticalData, NameOf(SliderVertical), SliderVertical.DefaultStyle)
            LoadContent(Of Select2DSkin)(Content, Select2DData, NameOf(Select2D), Select2D.DefaultStyle)
            LoadContent(Of SelectEntitySkin)(Content, SelectEntityData, NameOf(SelectEntity), SelectEntity.DefaultStyle)

            Transparent = Content.Load(Of Texture2D)(Root & "textures/Transparent")
        End Sub

        Sub LoadContent(Of SkinType)(Content As ContentManager, ByRef TextureData As TextureData(), Name As String, ByRef DefaultStyle As Entities.StyleSheet)
            Dim Enums = System.[Enum].GetValues(GetType(SkinType))
            TextureData = New TextureData(Enums.Length - 1) {}
            For Each Skin As SkinType In Enums
                TextureData(CInt(CObj(Skin))) = Content.Load(Of TextureData)(Root & "textures/" & Name & "_" & Skin.ToString.ToLower() & "_md")
            Next
            LoadDefaultStyles(DefaultStyle, Name, Root, Content)
        End Sub

        ''' <summary>
        ''' Load default stylesheets for a given entity name And put values inside the sheet.
        ''' </summary>
        ''' <param name="sheet">StyleSheet to load.</param>
        ''' <param name="entityName">Entity unique identifier for file names.</param>
        ''' <param name="themeRoot">Path of the current theme root directory.</param>
        ''' <param name="content">Content manager to allow us to load xmls.</param>
        Private Sub LoadDefaultStyles(ByRef Sheet As Entities.StyleSheet, ByVal EntityName As String, ByVal ThemeRoot As String, ByVal Content As ContentManager)
            Dim stylesheetBase As String = ThemeRoot & "styles/" & EntityName
            FillDefaultStyles(Sheet, Entities.EntityState.[Default], Content.Load(Of DefaultStyles)(stylesheetBase & "-Default"))
            FillDefaultStyles(Sheet, Entities.EntityState.MouseHover, Content.Load(Of DefaultStyles)(stylesheetBase & "-MouseHover"))
            FillDefaultStyles(Sheet, Entities.EntityState.MouseDown, Content.Load(Of DefaultStyles)(stylesheetBase & "-MouseDown"))
        End Sub

        ''' <summary>
        ''' Fill a set of default styles into a given stylesheet.
        ''' </summary>
        ''' <param name="sheet">StyleSheet to fill.</param>
        ''' <param name="state">State to fill values for.</param>
        ''' <param name="styles">Default styles, as loaded from xml file.</param>
        Private Sub FillDefaultStyles(ByRef Sheet As Entities.StyleSheet, ByVal State As Entities.EntityState, ByVal Styles As DefaultStyles)
            If Styles.FillColor IsNot Nothing Then
                Sheet(State.ToString() & "." & "FillColor") = New StyleProperty(CType(Styles.FillColor, Color))
            End If

            If Styles.FontStyle IsNot Nothing Then
                Sheet(State.ToString() & "." & "FontStyle") = New StyleProperty(CInt(Styles.FontStyle))
            End If

            If Styles.ForceAlignCenter IsNot Nothing Then
                Sheet(State.ToString() & "." & "ForceAlignCenter") = New StyleProperty(CBool(Styles.ForceAlignCenter))
            End If

            If Styles.OutlineColor IsNot Nothing Then
                Sheet(State.ToString() & "." & "OutlineColor") = New StyleProperty(CType(Styles.OutlineColor, Color))
            End If

            If Styles.OutlineWidth IsNot Nothing Then
                Sheet(State.ToString() & "." & "OutlineWidth") = New StyleProperty(CInt(Styles.OutlineWidth))
            End If

            If Styles.Scale IsNot Nothing Then
                Sheet(State.ToString() & "." & "Scale") = New StyleProperty(CSng(Styles.Scale))
            End If

            If Styles.SelectedHighlightColor IsNot Nothing Then
                Sheet(State.ToString() & "." & "SelectedHighlightColor") = New StyleProperty(CType(Styles.SelectedHighlightColor, Color))
            End If

            If Styles.ShadowColor IsNot Nothing Then
                Sheet(State.ToString() & "." & "ShadowColor") = New StyleProperty(CType(Styles.ShadowColor, Color))
            End If

            If Styles.ShadowOffset IsNot Nothing Then
                Sheet(State.ToString() & "." & "ShadowOffset") = New StyleProperty(CType(Styles.ShadowOffset, Vector2))
            End If

            If Styles.Padding IsNot Nothing Then
                Sheet(State.ToString() & "." & "Padding") = New StyleProperty(CType(Styles.Padding, Vector2))
            End If

            If Styles.SpaceBefore IsNot Nothing Then
                Sheet(State.ToString() & "." & "SpaceBefore") = New StyleProperty(CType(Styles.SpaceBefore, Vector2))
            End If

            If Styles.SpaceAfter IsNot Nothing Then
                Sheet(State.ToString() & "." & "SpaceAfter") = New StyleProperty(CType(Styles.SpaceAfter, Vector2))
            End If

            If Styles.ShadowScale IsNot Nothing Then
                Sheet(State.ToString() & "." & "ShadowScale") = New StyleProperty(CSng(Styles.ShadowScale))
            End If
        End Sub
    End Class
End Namespace