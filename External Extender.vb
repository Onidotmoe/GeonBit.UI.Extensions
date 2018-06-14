Imports System.Xml.Serialization
Imports GeonBit.UI
Imports GeonBit.UI.DataTypes
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content

Public Class ExternalExtender

    ''' <summary>SliderVerticals base textures.</summary>
    Public SliderVerticalTextures As TexturesGetter(Of GeonBit.UI.Entities.SliderVerticalSkin) = New TexturesGetter(Of GeonBit.UI.Entities.SliderVerticalSkin)("textures/SliderVertical_")

    ''' <summary>SliderVerticals mark textures (the sliding piece that shows current value).</summary>
    Public SliderVerticalMarkTextures As TexturesGetter(Of GeonBit.UI.Entities.SliderVerticalSkin) = New TexturesGetter(Of GeonBit.UI.Entities.SliderVerticalSkin)("textures/SliderVertical_", "_mark")

    ''' <summary>Metadata about SliderVertical textures.</summary>
    Public SliderVerticalData As TextureData()

    ''' <summary>GridSelects base textures.</summary>
    Public GridSelectTextures As TexturesGetter(Of GeonBit.UI.Entities.GridSelectSkin) = New TexturesGetter(Of GeonBit.UI.Entities.GridSelectSkin)("textures/GridSelect_")

    ''' <summary>GridSelects mark textures (the dot that shows current value).</summary>
    Public GridSelectDotTextures As TexturesGetter(Of GeonBit.UI.Entities.GridSelectSkin) = New TexturesGetter(Of GeonBit.UI.Entities.GridSelectSkin)("textures/GridSelect_", "_dot")

    ''' <summary>Metadata about GridSelect textures.</summary>
    Public GridSelectData As TextureData()

    ''' <summary>Root for geonbit.ui content</summary>
    Private Root As String

    Sub Initialize(Content As ContentManager, Optional Theme As String = "Vision")
        Root = ("GeonBit.UI/themes/" & Theme & "/")

        SliderVerticalData = New TextureData(System.[Enum].GetValues(GetType(GeonBit.UI.Entities.SliderVerticalSkin)).Length - 1) {}
        For Each Skin As GeonBit.UI.Entities.SliderVerticalSkin In System.[Enum].GetValues(GetType(GeonBit.UI.Entities.SliderVerticalSkin))
            SliderVerticalData(CInt(Skin)) = Content.Load(Of TextureData)(Root & "textures/SliderVertical_" & Skin.ToString().ToLower() & "_md")
        Next
        LoadDefaultStyles(GeonBit.UI.Entities.SliderVertical.DefaultStyle, NameOf(GeonBit.UI.Entities.SliderVertical), Root, Content)

        GridSelectData = New TextureData(System.[Enum].GetValues(GetType(GeonBit.UI.Entities.GridSelectSkin)).Length - 1) {}
        For Each Skin As GeonBit.UI.Entities.GridSelectSkin In System.[Enum].GetValues(GetType(GeonBit.UI.Entities.GridSelectSkin))
            GridSelectData(CInt(Skin)) = Content.Load(Of TextureData)(Root & "textures/GridSelect_" & Skin.ToString().ToLower() & "_md")
        Next
        LoadDefaultStyles(GeonBit.UI.Entities.GridSelect.DefaultStyle, NameOf(GeonBit.UI.Entities.GridSelect), Root, Content)
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

    '''' <summary>
    '''' Font styles (should match the font styles defined in GeonBit.UI engine).
    '''' </summary>
    'Public Enum _FontStyle
    '    Regular
    '    Bold
    '    Italic
    'End Enum
    '''' <summary>
    '''' All the stylesheet possible settings for an entity state.
    '''' </summary>
    'Public Class DefaultStyles
    '    ' entity scale
    '    <XmlElement(IsNullable:=True)>
    '    Public Scale As Single? = Nothing
    '    ' fill color
    '    <XmlElement("Color", IsNullable:=True)>
    '    Public FillColor As Color? = Nothing
    '    ' outline color
    '    <XmlElement("Color", IsNullable:=True)>
    '    Public OutlineColor As Color? = Nothing

    '    ' outline width
    '    <XmlElement(IsNullable:=True)>
    '    Public OutlineWidth As Integer? = Nothing

    '    ' for paragraph only - align to center
    '    <XmlElement(IsNullable:=True)>
    '    Public ForceAlignCenter As Boolean? = Nothing

    '    ' for paragraph only - font style
    '    <XmlElement(IsNullable:=True)>
    '    Public FontStyle As _FontStyle? = Nothing

    '    ' for lists etc - selected highlight background color
    '    <XmlElement("Color", IsNullable:=True)>
    '    Public SelectedHighlightColor As Color? = Nothing

    '    ' shadow color (set to 00000000 for no shadow)
    '    <XmlElement("Color", IsNullable:=True)>
    '    Public ShadowColor As Color? = Nothing

    '    ' shadow offset
    '    <XmlElement("Vector", IsNullable:=True)>
    '    Public ShadowOffset As Vector2? = Nothing

    '    ' padding
    '    <XmlElement("Vector", IsNullable:=True)>
    '    Public Padding As Vector2? = Nothing

    '    ' space before
    '    <XmlElement("Vector", IsNullable:=True)>
    '    Public SpaceBefore As Vector2? = Nothing

    '    ' space after
    '    <XmlElement("Vector", IsNullable:=True)>
    '    Public SpaceAfter As Vector2? = Nothing

    '    ' shadow scale
    '    Public ShadowScale As Single? = Nothing

    '    ' Selection Color if selection is possible
    '    <XmlElement("Color", IsNullable:=True)>
    '    Public SelectionColor As Color? = Nothing
    'End Class
End Class