Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks
Imports GeonBit.UI
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Graphics
Imports MonoGame.Extended.BitmapFonts

Namespace Extensions

    ''' <summary>Allows for selecting system-wide fonts and importing them into the game.</summary>
    Public Class SelectFont
        Inherits Panel

        ''' <summary>Default SelectFont size in pixels.</summary>
        Public Shared Shadows ReadOnly DefaultSize As Vector2 = New Vector2(400.0F, 650.0F)
        Private Fonts As New Dictionary(Of String, SpriteFont)
        Private ReadOnly Characters As Characters = YOURGAME.GetService(Of Globals).Characters
        Private IsCustomStringSet As Boolean
        Private GenerateFontsCancellation As New CancellationTokenSource
        Private IsGeneratorRunning As Boolean
        Private IsGeneratorRestartRequested As Boolean
        Private AvailableStyles As New List(Of System.Drawing.FontStyle)
        Private PreviousSelectedIndexes As New List(Of Integer)
        Property CurrentColor As Microsoft.Xna.Framework.Color = Microsoft.Xna.Framework.Color.Black
        Property SelectedIndexes As New List(Of Integer)
        Property SelectedColor As System.Drawing.Color = YOURGAME.GetService(Of Globals).GiveItem(Of Dictionary.Brush)("CA10").System
        Property SelectedRange As Range = YOURGAME.GetService(Of Settings.Settings).Defaults.Game.DefaultUnicodeRange
        Property SelectedStyle As System.Drawing.FontStyle = System.Drawing.FontStyle.Regular
        Property FontBackgroundColor As Microsoft.Xna.Framework.Color = YOURGAME.GetService(Of Globals).GiveItem(Of Dictionary.Brush)("C000").Color
        Property Custom_String_TextInput As TextInput
        Property Custom_Name_TextInput As TextInput
        Property Custom_Range_Start_TextInput As TextInput
        Property Custom_Range_End_TextInput As TextInput
        Property Custom_Size_TextInput As TextInput
        Property Custom_MissingCharacter_TextInput As TextInput
        Property SelectedMissingCharacter As Char = "*"c '�
        Property Custom_ColorPicker_Button As Button
        Property Custom_Add_Button As Button
        Property Ranges_DropDown As Minor.DropDown
        Property Style_DropDown As Minor.DropDown
        Property SelectEntity As SelectEntity
        Property ShowSystemFonts_CheckBox As CheckBox
        Property OnlySelection_CheckBox As CheckBox
        Property FontSize As Single = 24

        Sub New(Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2 = Nothing)
            MyBase.New(USE_DEFAULT_SIZE, anchor:=Anchor, offset:=Offset)
            Size = DefaultSize
            Initialize()
        End Sub

        Sub New(Size As Vector2, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2 = Nothing)
            MyBase.New(Size, anchor:=Anchor, offset:=Offset)
            Initialize()
        End Sub

        Private Sub Initialize()
            Dim ScaleFactor As Single = (Size.X / Size.Y)
            Dim Scale_Radio As Single = (ScaleFactor / 1.0F)
            Dim Scale_Label As Single = (ScaleFactor / 2.8F)
            Dim Scale_Input As Single = (ScaleFactor / 4.2F)
            Dim Scale_Button As Single = (ScaleFactor / 4.2F)
            Dim Scale_ColorPicker_Button As Single = (ScaleFactor / 4.2F)
            Dim Scale_DropDown As Single = (ScaleFactor / 4.2F)
            Dim MultiLine As Boolean = False
            Dim Size_Input = New Vector2((Size.X * 0.35F), (Size.Y * 0.06F))
            Dim Size_Checkbox = New Vector2((Size.X * 0.75F), (Size.Y * 0.05F))
            Dim Size_DropDown = New Vector2((Size.X * 0.9F), (Size.Y * 0.5F))
            Dim Size_Button = New Vector2((Size.X * 0.15F), (Size.Y * 0.06F))
            Dim Size_ColorPicker_Button = New Vector2((Size.X * 0.07F), (Size.Y * 0.06F))
            Dim Size_Long_Input = New Vector2((Size.X * 0.9F), Size_Input.Y)

            Dim Column_X0 As Single = (Size.X * 0.05F)
            Dim Column_X1 As Single = (Size.X * 0.415F)
            Dim Column_X2 As Single = (Size.X * 0.8F)
            Dim Column_X3 As Single = (Size.X * 0.65F)
            Dim Column_X4 As Single = (Size.X * 0.88F)

            Dim Row_Y0 As Single = (Size.Y * 0.03F)
            Dim Row_Y1 As Single = (Size.Y * 0.1F)
            Dim Row_Y2 As Single = (Size.Y * 0.17F)
            Dim Row_Y3 As Single = (Size.Y * 0.24F)
            Dim Row_Y4 As Single = (Size.Y * 0.31F)
            Dim Row_Y5 As Single = (Size.Y * 0.38F)
            Dim Row_Y6 As Single = (Size.Y * 0.9F)
            Dim Row_Y7 As Single = (Size.Y * 0.94F)

            Dim TopLeft = Anchor.TopLeft
            Dim Interpreter = YOURGAME.GetService(Of Globals).Interpreter

            Background = New ColoredRectangle(YOURGAME.GetService(Of Globals).GiveItem(Of Dictionary.Brush)("CA00").Color, Size, TopLeft)

            Custom_String_TextInput = New TextInput(False, Size_Long_Input, TopLeft, New Vector2(Column_X0, Row_Y0)) With {.Scale = Scale_Input, .ToolTipText = Interpreter.GiveString("GA21"), .CharactersLimit = 30, .OnValueChange = Sub() CustomString_TextInput_OnValueChange()}
            Custom_Name_TextInput = New TextInput(MultiLine, New Vector2((Size_Long_Input.X - Size_ColorPicker_Button.X), Size_Long_Input.Y), TopLeft, New Vector2(Column_X0, Row_Y1)) With {.Scale = Scale_Input, .ToolTipText = Interpreter.GiveString("GA22"), .Value = SelectedRange.Name, .CharactersLimit = 30}
            Custom_Range_Start_TextInput = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X0, Row_Y2)) With {.Scale = Scale_Input, .ToolTipText = Interpreter.GiveString("GA23"), .Value = SelectedRange.Start, .CharactersLimit = 8}
            Custom_Range_End_TextInput = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X1, Row_Y2)) With {.Scale = Scale_Input, .ToolTipText = Interpreter.GiveString("GA24"), .Value = SelectedRange.End, .CharactersLimit = 8}
            Custom_Add_Button = New Button(Interpreter.GiveString("GA20"), TopLeft, Size_Button, New Vector2(Column_X2, Row_Y2)) With {.Scale = Scale_Button, .ToolTipText = Interpreter.GiveString("GA25"), .OnClick = Sub() Custom_Add_Button_OnClick()}

            Ranges_DropDown = New Minor.DropDown(Size_DropDown, SelectedRange.Name, TopLeft, New Vector2(Column_X0, Row_Y3)) With {.Scale = Scale_DropDown, .OnValueChange = Sub() Ranges_DropDown_OnValueChange()}
            Style_DropDown = New Minor.DropDown(Size_DropDown, SelectedStyle.ToString, TopLeft, New Vector2(Column_X0, Row_Y4)) With {.Scale = Scale_DropDown, .OnValueChange = Sub() Style_DropDown_OnValueChange()}
            SelectEntity = New SelectEntity(New Vector2((Size.X * 0.9F), (Size.Y * 0.51F)), TopLeft, New Vector2(Column_X0, Row_Y5)) With {.BorderColor = YOURGAME.GetService(Of Globals).GiveItem(Of Dictionary.Brush)("CA07").Color, .BorderWidth = 4, .OnValueChange = Sub() SelectEntity_OnValueChange()}
            ShowSystemFonts_CheckBox = New CheckBox(Interpreter.GiveString("GA18"), TopLeft, Size_Checkbox, New Vector2(Column_X0, Row_Y6)) With {.Scale = Scale_Radio, .OnValueChange = Sub() ShowSystemFonts_CheckBox_OnValueChange()}
            ShowSystemFonts_CheckBox.Padding = New Vector2(Size.X * 0.05F, 0)
            ShowSystemFonts_CheckBox.TextParagraph.Scale = (Scale_Radio * 1.5F)
            OnlySelection_CheckBox = New CheckBox(Interpreter.GiveString("GA26"), TopLeft, Size_Checkbox, New Vector2(Column_X0, Row_Y7)) With {.Scale = Scale_Radio, .OnValueChange = Sub() OnlySelection_CheckBox_OnValueChange()}
            OnlySelection_CheckBox.Padding = New Vector2(Size.X * 0.05F, 0)
            OnlySelection_CheckBox.TextParagraph.Scale = (Scale_Radio * 1.5F)

            Custom_ColorPicker_Button = New Button(Anchor:=TopLeft, Size:=Size_ColorPicker_Button, Offset:=New Vector2(Column_X4, Row_Y1)) With {.Scale = Scale_ColorPicker_Button, .OnClick = Sub() Custom_ColorPicker_Button_OnClick(), .FillColor = CurrentColor}

            For Each Range In Characters.Ranges
                Ranges_DropDown.AddItem(Range.Name)
            Next

            For Each Style In System.Enum.GetValues(GetType(System.Drawing.FontStyle))
                AvailableStyles.Add(DirectCast(Style, System.Drawing.FontStyle))
                Style_DropDown.AddItem(Style.ToString)
            Next

            AddChild(Custom_String_TextInput)
            AddChild(Custom_Name_TextInput)
            AddChild(Custom_Range_Start_TextInput)
            AddChild(Custom_Range_End_TextInput)
            AddChild(Custom_Add_Button)
            AddChild(Ranges_DropDown)
            AddChild(Style_DropDown)
            AddChild(SelectEntity)
            AddChild(ShowSystemFonts_CheckBox)
            AddChild(OnlySelection_CheckBox)
            AddChild(Custom_ColorPicker_Button)

            GeneratorStart(Force:=True)
        End Sub
        Property CustomString As String
            Get
                Return Custom_String_TextInput.Value
            End Get
            Set
                Custom_String_TextInput.Value = Value
            End Set
        End Property
        Property OnlySelection As Boolean
            Get
                Return OnlySelection_CheckBox.Checked
            End Get
            Set
                OnlySelection_CheckBox.Checked = Value
            End Set
        End Property
        Property ShowSystemFonts As Boolean
            Get
                Return ShowSystemFonts_CheckBox.Checked
            End Get
            Set
                ShowSystemFonts_CheckBox.Checked = Value
            End Set
        End Property
        Function [Get](Name As String) As SpriteFont
            Dim Result = Fonts(Name)

            If (Result IsNot Nothing) Then
                Return Result
            Else
                If TryLoad(Name) Then
                    Return Fonts(Name)
                Else
                    Return YOURGAME.GetService(Of Settings.Settings).Defaults.Game.FallBackFont
                End If
            End If
        End Function
        Private Function TryLoad(Name As String) As Boolean
            Dim Font As SpriteFont = Nothing
            Try
                Font = YOURGAME.Instance.Content.Load(Of SpriteFont)(YOURGAME.GUIRoot & YOURGAME.Instance.Theme & "/fonts/" & Name)
            Catch Exception As ContentLoadException
                Return False
            End Try

            Fonts.Add(Name, Font)
            Return True
        End Function
        Private Sub SelectEntity_OnValueChange()
            If OnlySelection Then
                GeneratorStart(CustomString, Custom_String_TextInput)
            End If
        End Sub

        Private Sub Ranges_DropDown_OnValueChange()
            SelectedRange = Characters.Ranges.Find(Function(F) F.Name = Ranges_DropDown.SelectedValue)

            Custom_Name_TextInput.Value = If((SelectedRange.Name.Length > 30), (SelectedRange.Name.Remove(27) & "..."), SelectedRange.Name)
            Custom_Range_Start_TextInput.Value = SelectedRange.Start
            Custom_Range_End_TextInput.Value = SelectedRange.End

            Debug.WriteLine("ranges changed - redo fonts")
        End Sub

        Private Sub Style_DropDown_OnValueChange()
            SelectedStyle = AvailableStyles.First(Function(F) F.ToString = Style_DropDown.SelectedValue)
            GeneratorRestart(CustomString, Custom_String_TextInput)
        End Sub
        Private Sub Custom_ColorPicker_Button_OnClick()
            Dim Blocker = YOURGAME.GetService(Of Settings.Settings).Defaults.GUI.Blocker
            Dim ColorPicker As New ColorPicker(CInt(YOURGAME.Instance.CurrentSize.X / 2), StartingColor:=CurrentColor, Anchor:=Anchor.Center, OkCancelVisible:=True)
            ColorPicker.OnDialogResultChange = Sub()
                                                   If ColorPicker.DialogResult = DialogResult.OK Then
                                                       CurrentColor = RGBToColor(ColorPicker.Color_New)
                                                       SelectedColor = XNAColorToSystemColor(CurrentColor)
                                                       Custom_ColorPicker_Button.FillColor = CurrentColor
                                                       GeneratorStart(CustomString, Custom_String_TextInput)
                                                   End If

                                                   Blocker.RemoveFromParent()
                                                   Blocker.Dispose()
                                                   ColorPicker.Close()
                                               End Sub

            UserInterface.Active.AddEntity(Blocker).BringToFront()
            ColorPicker.Show()
        End Sub
        Private Sub Custom_Add_Button_OnClick()
            Debug.WriteLine("Add new game font - redo only this font")
        End Sub
        Private Sub ShowSystemFonts_CheckBox_OnValueChange()
            If ShowSystemFonts Then
                ShowSystemFonts_CheckBox.TextParagraph.Text = YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA27")
            Else
                ShowSystemFonts_CheckBox.TextParagraph.Text = YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA18")
            End If
            Debug.WriteLine("only system fonts - redo fonts")
        End Sub

        Private Sub OnlySelection_CheckBox_OnValueChange()
            GeneratorStart(CustomString, Custom_String_TextInput, True, True, True)
            GeneratorRestart(CustomString, Custom_String_TextInput, True)
        End Sub
        Private Async Sub CustomString_TextInput_OnValueChange()
            Dim Text As String = CustomString

            If (Not (Await IfNotChanged(Text, Custom_String_TextInput))) Then
                Exit Sub
            End If

            If (Not String.IsNullOrWhiteSpace(Text)) Then
                IsCustomStringSet = True
                GeneratorStart(Text, Custom_String_TextInput)
            Else
                IsCustomStringSet = False
                CustomString = String.Empty
                GeneratorStart(CustomString, Custom_String_TextInput)
            End If
        End Sub

        Private Sub GeneratorStart(Optional Text As String = "", Optional TextInput As TextInput = Nothing, Optional Force As Boolean = False, Optional Clear As Boolean = False, Optional IgnoreCustomString As Boolean = False)
            SelectEntity.AddRange(GenerateSpriteFonts().Values)
        End Sub
        Private Async Sub GeneratorRestart(Text As String, Optional TextInput As TextInput = Nothing, Optional Force As Boolean = False, Optional Clear As Boolean = False)

        End Sub

        'Private Sub GeneratorStart(Optional Text As String = "", Optional TextInput As TextInput = Nothing, Optional Force As Boolean = False, Optional Clear As Boolean = False, Optional IgnoreCustomString As Boolean = False)
        '    If ((Not IsGeneratorRunning) AndAlso (Not IsGeneratorRestartRequested)) OrElse Clear Then
        '        IsGeneratorRunning = True
        '        SelectEntity.Locked = True

        '        If (Not OnlySelection) OrElse Clear Then
        '            Dim FontPreviews = GenerateSystemFonts(IgnoreCustomString:=IgnoreCustomString)

        '            'Check if CustomString has changed while we were generating FontPreviews
        '            'If it has, don't alter the list as a new GeneratorStart was already triggered.
        '            If Force OrElse Equals(Text, TextInput.Value) Then
        '                If (FontPreviews IsNot Nothing) Then
        '                    SelectedIndexes = SelectEntity.SelectedIndexes
        '                    SelectEntity.ClearItems()
        '                    SelectEntity.AddRange(FontPreviews.Values.ToList)
        '                    SelectEntity.SelectMany(SelectedIndexes)
        '                    SelectEntity.MarkAsDirty()
        '                End If
        '            End If
        '        Else
        '            SelectedIndexes = SelectEntity.SelectedIndexes
        '            If ((SelectedIndexes IsNot Nothing) AndAlso SelectedIndexes.Any) Then
        '                If Force OrElse Equals(Text, TextInput.Value) Then
        '                    Dim FontPreviews = GenerateSystemFonts(SelectedOnly:=True)

        '                    If (FontPreviews IsNot Nothing) Then
        '                        SelectEntity.ReplaceMany(FontPreviews.ToDictionary(Function(F) F.Key, Function(F) DirectCast(F.Value, Entity)))
        '                    End If
        '                End If
        '            End If
        '            If ((PreviousSelectedIndexes IsNot Nothing) AndAlso PreviousSelectedIndexes.Any) Then
        '                If ((SelectedIndexes IsNot Nothing) AndAlso SelectedIndexes.Any) Then
        '                    For Each i In SelectedIndexes
        '                        PreviousSelectedIndexes.Remove(i)
        '                    Next
        '                End If

        '                If PreviousSelectedIndexes.Any Then
        '                    Dim FontPreviews = GenerateSystemFonts(PreviousSelectedOnly:=True, IgnoreCustomString:=True)

        '                    If (FontPreviews IsNot Nothing) Then
        '                        SelectEntity.ReplaceMany(FontPreviews.ToDictionary(Function(F) F.Key, Function(F) DirectCast(F.Value, Entity)))
        '                    End If
        '                End If
        '            End If

        '            PreviousSelectedIndexes = SelectedIndexes
        '            SelectEntity.MarkAsDirty()
        '        End If

        '        SelectEntity.Locked = False
        '        IsGeneratorRunning = False
        '    Else
        '        GeneratorRestart(Text, TextInput)
        '    End If
        'End Sub

        'Private Async Sub GeneratorRestart(Text As String, Optional TextInput As TextInput = Nothing, Optional Force As Boolean = False, Optional Clear As Boolean = False)
        '    If (Not IsGeneratorRestartRequested) OrElse Force Then
        '        IsGeneratorRestartRequested = True
        '        GenerateFontsCancellation.Cancel()

        '        'Give the cancellation time to complete
        '        Await DelayTask(0.2)

        '        If GenerateFontsCancellation.IsCancellationRequested Then
        '            GenerateFontsCancellation = New CancellationTokenSource
        '        End If

        '        IsGeneratorRestartRequested = False
        '        GeneratorStart(Text, TextInput, Force, Clear)
        '    End If
        'End Sub

        '''' <summary>
        '''' <para>Generate all available system fonts as spritefonts.</para>
        '''' <para><remarks>Windows Only atm.</remarks></para>
        '''' </summary>
        'Function GenerateSystemFonts(Range As Range) As List(Of SpriteFont)
        '    '   Dim Path = YOURGAME.Instance.GUIRoot & YOURGAME.Instance.Theme & "/fonts/" & Name
        '    ' Dim RAW = YOURGAME.Instance.Content.Load(Of Byte())(Path)

        '    SelectEnity.AddRange(GenerateFontItems(GetAllInstalledFonts))
        '    'Dim InstalledFonts = GetAllInstalledFonts()
        '    'Dim FontBag As New ConcurrentBag(Of SpriteFont)
        '    'Dim IsCustomStringSet As Boolean = (Not String.IsNullOrWhiteSpace(CustomString))

        '    ''  Parallel.ForEach(InstalledFonts,
        '    ''Sub(InstalledFont)
        '    'Dim ItemSize As New PointF((SelectEnity.Size.X), (SelectEnity.Size.Y * 0.2F))
        '    'Dim DrawPoint As New PointF((ItemSize.X * 0.05F), (ItemSize.Y / 3))

        '    'For i = 0 To (InstalledFonts.Count - 1)
        '    '    Dim InstalledFont = InstalledFonts(i)
        '    '    Dim SpriteFont As SpriteFont = Nothing
        '    '    Dim FontName As String = If(String.IsNullOrWhiteSpace(InstalledFont.Name), YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA19"), InstalledFont.Name)
        '    '    Dim Bitmap As Bitmap = CreateBitmap(CInt(ItemSize.X), CInt(ItemSize.Y)) 'As New Bitmap((10 * FontName.Length), 100, Imaging.PixelFormat.Format16bppRgb555)

        '    '    Using Bitmap
        '    '        Using Graphics As Graphics = Graphics.FromImage(Bitmap)
        '    '            Graphics.Clear(System.Drawing.Color.Transparent)
        '    '            Graphics.TextRenderingHint = Text.TextRenderingHint.AntiAlias
        '    '            Using FontFamily As FontFamily = InstalledFont
        '    '                Using Font As Font = New Font(FontFamily, FontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel)
        '    '                    ' SpriteFont = FontToSpriteFont(Font, Range)
        '    '                    Using SolidBrush As SolidBrush = New SolidBrush(SelectedColor)
        '    '                        Dim StringDraw As String = Nothing

        '    '                        If IsCustomStringSet Then
        '    '                            StringDraw = CustomString
        '    '                        Else
        '    '                            StringDraw = Font.Name
        '    '                        End If

        '    '                        Graphics.DrawString(StringDraw, Font, SolidBrush, DrawPoint)
        '    '                    End Using
        '    '                End Using
        '    '            End Using

        '    '            Graphics.Save()
        '    '            ' Bitmap = System.Drawing.Image.FromHbitmap(Graphics.GetHdc())
        '    '        End Using

        '    '        Dim Image As New Entities.Image(BitmapToTexture2D(Bitmap), New Vector2(ItemSize.X, ItemSize.Y))
        '    '        Image.Background = New ColoredRectangle(FontBackgroundColor, Image.GetActualDestRect.Size.ToVector2, Anchor.TopLeft)
        '    '        SelectEnity.AddItem(Image)

        '    '        ' Dim x As New BitmapFontRegion(New TextureAtlases.TextureRegion2D(BitmapToTexture2D(Bitmap), New Microsoft.Xna.Framework.Rectangle(Microsoft.Xna.Framework.Point.Zero, New Microsoft.Xna.Framework.Point(100, 100))), 0, 0, 0, 0)
        '    '        ' Dim xxx As New BitmapFont("Arial", New List(Of BitmapFontRegion) From {x}, 20)
        '    '        '  Result = BitmapFontToSpriteFont(xxx, New Range() With {.Start = "U+0000", .[End] = "U+007F"})
        '    '    End Using
        '    '    ' FontBag.Add(SpriteFont)
        '    'Next

        '    'End Sub)

        '    'Dim Bitmap As New Bitmap(100, 100, Imaging.PixelFormat.Format16bppRgb555)
        '    'Using Bitmap
        '    '    Using Graphics As Graphics = Graphics.FromImage(Bitmap)
        '    '        Graphics.Clear(Color.White)
        '    '        Using FontFamily As FontFamily = New FontFamily("Blackoak Std")
        '    '            Using Font As Font = New Font(FontFamily, 24, FontStyle.Regular, GraphicsUnit.Pixel)
        '    '                Using SolidBrush As SolidBrush = New SolidBrush(Color.Red)
        '    '                    Graphics.DrawString("A", Font, SolidBrush, New PointF(10, 10))
        '    '                End Using
        '    '            End Using
        '    '        End Using
        '    '    End Using
        '    '    Dim x As New BitmapFontRegion(New TextureAtlases.TextureRegion2D(BitmapToTexture2D(Bitmap), New Microsoft.Xna.Framework.Rectangle(Microsoft.Xna.Framework.Point.Zero, New Microsoft.Xna.Framework.Point(100, 100))), 0, 0, 0, 0)
        '    '    Dim xxx As New BitmapFont("Arial", New List(Of BitmapFontRegion) From {x}, 20)
        '    '    Result = BitmapFontToSpriteFont(xxx, New Ranges() With {.Start = "U+0000", .[End] = "U+007F"})

        '    '    UserInterface.Active.AddEntity(New Entities.Image(BitmapToTexture2D(Bitmap), New Microsoft.Xna.Framework.Vector2(100, 100)))
        '    'End Using

        '    '   Dim y As New Entities.Label("Arial") With {.FontOverride = CType(Result, Microsoft.Xna.Framework.Graphics.SpriteFont)}
        '    '    UserInterface.Active.AddEntity(y)
        '    Return Nothing
        'End Function

        Private Function GenerateSpriteFonts(Optional SelectedOnly As Boolean = False, Optional PreviousSelectedOnly As Boolean = False, Optional IgnoreCustomString As Boolean = False) As SortedDictionary(Of Integer, Entities.Image)
            Dim InstalledBag As New ConcurrentDictionary(Of Integer, FontFamily)
            Dim SpriteFontBag As New ConcurrentDictionary(Of Integer, SpriteFont)
            Dim ProxyBag As New ConcurrentDictionary(Of Integer, ProxySpriteFont)
            Dim TextureBag As New ConcurrentDictionary(Of Integer, Texture2D)
            Dim ImageBag As New ConcurrentDictionary(Of Integer, Entities.Image)
            Dim ItemSize As New PointF(SelectEntity.Size.X, (SelectEntity.Size.Y * 0.2F))
            Dim ItemSizeVector As New Vector2(ItemSize.X, ItemSize.Y)
            Dim ItemWidth As Integer = CInt(ItemSize.X)
            Dim ItemHeight As Integer = CInt(ItemSize.Y)
            Dim DrawVector As New Vector2((ItemSize.X * 0.03F), (ItemSize.Y / 3))
            Dim MissingName = YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA19")
            Dim AllInstalledFonts = GetAllInstalledFonts()

            If SelectedOnly Then
                Parallel.ForEach(SelectedIndexes, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            ElseIf PreviousSelectedOnly Then
                Parallel.ForEach(PreviousSelectedIndexes, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            Else
                Parallel.For(0, AllInstalledFonts.Count, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            End If

            'Create all the proxies in parallel to maintain high performance
            Parallel.ForEach(InstalledBag,
                             Sub(InstalledFont, State)
                                 If GenerateFontsCancellation.Token.IsCancellationRequested Then
                                     State.Stop()
                                 End If

                                 ProxyBag.TryAdd(InstalledFont.Key, FontToSpriteFontProxy(InstalledFont.Value, SelectedRange))
                             End Sub)

            If GenerateFontsCancellation.Token.IsCancellationRequested Then
                GenerateFontsCancellation = New CancellationTokenSource
                Return Nothing
            End If

            'TODO : Remove debug code below
            Dim xxx4 As New Integer

            'Resolve the proxies in serial as graphicsdevice is not parallelable
            For Each ProxyItem In ProxyBag
                If GenerateFontsCancellation.Token.IsCancellationRequested Then
                    GenerateFontsCancellation = New CancellationTokenSource
                    Return Nothing
                End If

                Dim SpriteFont = ProxyItem.Value.Resolve
                Dim StringDraw As String = Nothing

                If ((Not IgnoreCustomString) AndAlso IsCustomStringSet) Then
                    StringDraw = CustomString
                ElseIf (Not String.IsNullOrWhiteSpace(ProxyItem.Value.Name)) Then
                    StringDraw = ProxyItem.Value.Name
                Else
                    StringDraw = MissingName
                End If

                'TODO : Remove debug code below
                'If xxx4 <= 5 Then
                '     xxx4 += 1
                ' ProxyItem.Value.Bitmap.Save(Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop + "\test\" + ProxyItem.Key.ToString + "_" + ProxyItem.Value.Name + ".png")
                '    End If

                SpriteFontBag.TryAdd(ProxyItem.Key, SpriteFont)
                TextureBag.TryAdd(ProxyItem.Key, PreviewSpriteFont(SpriteFont, StringDraw, ItemWidth, ItemHeight, DrawVector))
            Next

            Parallel.ForEach(TextureBag,
                             Sub(TextureItem, State)
                                 If GenerateFontsCancellation.Token.IsCancellationRequested Then
                                     State.Stop()
                                 End If

                                 Dim Image As New Entities.Image(TextureItem.Value, ItemSizeVector)
                                 Image.Background = New ColoredRectangle(FontBackgroundColor, Image.GetActualDestRect.Size.ToVector2, Anchor.TopLeft)
                                 Image.ToolTipText = If(String.IsNullOrWhiteSpace(InstalledBag(TextureItem.Key).Name), MissingName, InstalledBag(TextureItem.Key).Name)
                                 ImageBag.TryAdd(TextureItem.Key, Image)
                             End Sub)

            If GenerateFontsCancellation.Token.IsCancellationRequested Then
                GenerateFontsCancellation = New CancellationTokenSource
                Return Nothing
            End If

            Return New SortedDictionary(Of Integer, Entities.Image)(ImageBag)
        End Function

        Private Function PreviewSpriteFont(SpriteFont As SpriteFont, Text As String, Width As Integer, Height As Integer, Position As Vector2) As Texture2D
            Dim PresentationParameters = YOURGAME.Instance.Graphics.GraphicsDevice.PresentationParameters
            Dim Target As RenderTarget2D = New RenderTarget2D(YOURGAME.Instance.Graphics.GraphicsDevice, Width, Height, False, PresentationParameters.BackBufferFormat, DepthFormat.None, PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents)

            'Dim SpriteFont = YOURGAME.Instance.Content.Load(Of SpriteFont)("GeonBit.UI/themes/" & YOURGAME.Instance.Theme & "/fonts/Regular")
            'Texture2DToBitmapMemoryStream(SpriteFont.Texture).Save(Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop + "\test\" & Text + ".png")
            Text = "ABCDEFGHIJKLMNOPQRSTUVWXZÆØÅ"
            With YOURGAME.Instance
                .GraphicsDevice.SetRenderTarget(Target)
                .GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent)
                .Constructor.SpriteBatch.Begin()
                .Constructor.SpriteBatch.DrawString(SpriteFont, Text, Position, CurrentColor)
                .Constructor.SpriteBatch.End()
                .GraphicsDevice.SetRenderTarget(Nothing)
            End With

            Return Target
        End Function
        ''' <summary>This function is threadsafe. Using proxies allows us to workaround the parallel-limits of the GraphicsDevice.</summary>
        Private Function FontToSpriteFontProxy(FontFamily As FontFamily, Range As Range) As ProxySpriteFont
            Dim CharList As List(Of Char) = GetCharactersFromUnicodeRange(Range)
            Dim GlyphBoundsList As New List(Of Microsoft.Xna.Framework.Rectangle)
            Dim CroppingList As New List(Of Microsoft.Xna.Framework.Rectangle)
            Dim KerningList As New List(Of Vector3)
            Dim Font = New Font(FontFamily, FontSize, SelectedStyle, GraphicsUnit.Pixel)

            'Dim PairList As New List(Of KerningPair)
            'PairList = GetKerningPairs(Font)?.ToList
            'If (PairList IsNot Nothing) Then
            '    For Each Pair In PairList
            '        '  KerningList.Add(New Vector3(Pair.wFirst, Pair.iKernelAmount, Pair.wSecond))
            '    Next
            'Else
            '    'KerningList cannot be empty
            '    For i As Integer = 0 To (CharList.Count - 1)
            '        KerningList.Add(Vector3.Zero)
            '    Next
            'End If

            'If (KerningList.Count < CharList.Count) Then
            ''KerningList cannot be smaller than CharList
            'For i As Integer = 0 To ((CharList.Count - KerningList.Count) - 1)
            '    KerningList.Add(Vector3.Zero)
            'Next
            'End If

            'Kernings seem to not be correctly implemented, we skip them entirely for now
            For i As Integer = 0 To (CharList.Count - 1)
                KerningList.Add(Vector3.Zero)
            Next

            Dim EmHeight = Font.FontFamily.GetEmHeight(SelectedStyle)
            ' Dim Above = Font.FontFamily.GetCellAscent(SelectedStyle)
            ' Dim Beneath = Font.FontFamily.GetCellDescent(SelectedStyle)
            Dim LineSpacing = Font.FontFamily.GetLineSpacing(SelectedStyle)

            'Use EmHeight to create a square that is able to hold all Glyphs
            'We add 25% more space to ensure that we have enough for all Glyphs, we will crop the bitmap at the end.
            'Then we add 10% more space which will be used as margins between characters, which will be cropped individually when the font is rendered
            'This is because some fonts have overlapping characters if they don't have their own margin.
            Dim Square = ((EmHeight * CharList.Count) * 1.25 * 1.1)
            'Use squareroot to get the length of each side
            Dim Side As Integer = CInt(Math.Sqrt(Square))
            Dim EmRoot = (Math.Sqrt(EmHeight) * 1.1)
            Dim FinalBitmap As Bitmap = Nothing

            Using Bitmap As Bitmap = CreateBitmap(Side, Side)
                Using Graphics As Graphics = Graphics.FromImage(Bitmap)
                    Graphics.Clear(Drawing.Color.Transparent)
                    Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    Graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    Graphics.TextRenderingHint = Text.TextRenderingHint.AntiAliasGridFit
                    Graphics.PageUnit = GraphicsUnit.Pixel

                    Dim Brush As SolidBrush = New SolidBrush(SelectedColor)
                    Dim X As New Integer
                    Dim Y As New Integer

                    For i As Integer = 0 To (CharList.Count - 1)
                        Dim C = CharList(i)

                        'If the largest possible character cannot fit in the square, goto next row
                        If ((X + EmRoot) > Side) Then
                            X = 0
                            Y += CInt(EmRoot)
                        End If

                        Dim DrawPoint = New PointF(X, Y)
                        Dim Size = Graphics.MeasureString(C, Font).ToSize

                        Dim Margin_Width = CInt(Size.Width * 1.1)

                        'We add 10% more space to avoid having characters overlap
                        'Because we're adding a margin, we have to center the character in this outer rectangle.
                        GlyphBoundsList.Add(New Microsoft.Xna.Framework.Rectangle(X, Y, Margin_Width, CInt(Size.Height * 1.1)))
                        Graphics.DrawString(C, Font, Brush, DrawPoint)
                        'Think of cropping as the inner rectangle of the Glyphbound rectangle.
                        'We add 5% space on each side
                        CroppingList.Add(New Microsoft.Xna.Framework.Rectangle(CInt(Size.Width * 0.05), CInt(Size.Height * 0.05), Size.Width, Size.Height))
                        X += Margin_Width
                    Next

                    Graphics.Save()

                    'Crop the leftover blank space of the bitmap
                    FinalBitmap = Bitmap.Crop(Bitmap.Width, CInt(Y + EmRoot))
                End Using
            End Using

            'In .Net Font.Height is character Spacing but the name is misleading
            Return New ProxySpriteFont(New Bitmap(FinalBitmap), GlyphBoundsList, CroppingList, CharList, LineSpacing, Font.Height, KerningList, SelectedMissingCharacter, Font.Name)
        End Function
        ''' <summary>Holds SpriteFont properties until they are ready to be resolved in a non-parallel manner.</summary>
        Private Class ProxySpriteFont
            Public Bitmap As Bitmap
            Public CharList As List(Of Char)
            Public GlyphBoundsList As List(Of Microsoft.Xna.Framework.Rectangle)
            Public CroppingList As List(Of Microsoft.Xna.Framework.Rectangle)
            Public KerningList As List(Of Vector3)
            Public Font As Font
            Public MissingCharacter As Char
            Public LineSpacing As Integer
            Public Spacing As Integer
            Public Name As String

            Public Sub New(Bitmap As Bitmap, GlyphBoundsList As List(Of Microsoft.Xna.Framework.Rectangle), CroppingList As List(Of Microsoft.Xna.Framework.Rectangle), CharList As List(Of Char), LineSpacing As Integer, Spacing As Integer, KerningList As List(Of Vector3), MissingCharacter As Char, Name As String)
                Me.Bitmap = Bitmap
                Me.GlyphBoundsList = GlyphBoundsList
                Me.CharList = CharList
                Me.CroppingList = CroppingList
                Me.LineSpacing = LineSpacing
                Me.KerningList = KerningList
                Me.Spacing = Spacing
                Me.MissingCharacter = MissingCharacter
                Me.Name = Name
            End Sub

            Public Function Resolve() As SpriteFont
                Return New SpriteFont(BitmapToTexture2DLockBits(Bitmap), GlyphBoundsList, CroppingList, CharList, LineSpacing, Spacing, KerningList, MissingCharacter)
            End Function
        End Class

        'Function BitmapFontToSpriteFont(BitmapFont As BitmapFont, Range As Range) As SpriteFont
        '    'Dim Glyph = BitmapFont.GetGlyphs("A")
        '    'Dim Texture = Glyph.GetEnumerator.Current.FontRegion.TextureRegion.Texture
        '    Dim CharList As New List(Of Char)
        '    Dim GlyphList As New List(Of BitmapFont.StringGlyphEnumerable)

        '    For i As Integer = CInt("&H" & Range.Start.Remove(0, 2)) To CInt("&H" & Range.End.Remove(0, 2))
        '        CharList.Add(Microsoft.VisualBasic.ChrW(i))
        '        GlyphList.Add(BitmapFont.GetGlyphs(Microsoft.VisualBasic.ChrW(i)))
        '    Next

        '    '  Dim SpriteFont As SpriteFont

        '    '  Dim SpriteFontReader As Microsoft.Xna.Framework.Content.SpriteFontReader

        '    Return Nothing
        'End Function

        'Public Sub New(ByVal texture As Texture2D, ByVal glyphs As List(Of Rectangle), ByVal cropping As List(Of Rectangle), ByVal charMap As List(Of Char), ByVal lineSpacing As Integer, ByVal spacing As Single, ByVal kerning As List(Of Vector3), ByVal defaultCharacter As Char?)
        '    _texture = texture
        '    lineSpacing = lineSpacing
        '    spacing = spacing
        '    _defaultCharacter = defaultCharacter

        '    For i As Integer = 0 To charMap.Count - 1
        '        Dim g As GlyphData = New GlyphData()
        '        g.Glyph = glyphs(i)
        '        g.Cropping = cropping(i)
        '        g.Kerning = kerning(i)
        '        g.CharacterIndex = charMap(i)
        '        characterData.Add(g.CharacterIndex, g)
        '    Next4
        'End Sub

        ''' <summary>Generates Bitmap previews of systemfonts which then go into Image entities to be displayed.</summary>
        ''' <param name="SelectedOnly">If only the currently selected items should be rendered.</param>
        ''' <param name="PreviousSelectedOnly">If only the previously selected items should be rendered.</param>
        ''' <param name="IgnoreCustomString">If userinput should be ignored and have the font names show anyways.</param>
        ''' <returns>The previews and the index of where that font should be placed in the SelectEntity.</returns>
        ''' <remarks>Is too slow and has some weird problems with font color, use the SpriteFont generator instead.</remarks>
        Private Function GenerateSystemFonts(Optional SelectedOnly As Boolean = False, Optional PreviousSelectedOnly As Boolean = False, Optional IgnoreCustomString As Boolean = False) As SortedDictionary(Of Integer, Entities.Image)
            Dim InstalledBag As New ConcurrentDictionary(Of Integer, FontFamily)
            Dim BitmapBag As New ConcurrentDictionary(Of Integer, Bitmap)
            Dim TextureBag As New ConcurrentDictionary(Of Integer, Texture2D)
            Dim ImageBag As New ConcurrentDictionary(Of Integer, Entities.Image)
            Dim ItemSize As New PointF(SelectEntity.Size.X, (SelectEntity.Size.Y * 0.2F))
            Dim ItemSizeVector As New Vector2(ItemSize.X, ItemSize.Y)
            Dim ItemWidth As Integer = CInt(ItemSize.X)
            Dim ItemHeight As Integer = CInt(ItemSize.Y)
            Dim DrawPoint As New PointF((ItemSize.X * 0.03F), (ItemSize.Y / 3))
            Dim MissingName = YOURGAME.GetService(Of Globals).Interpreter.GiveString("GA19")
            Dim AllInstalledFonts = GetAllInstalledFonts()

            If SelectedOnly Then
                Parallel.ForEach(SelectedIndexes, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            ElseIf PreviousSelectedOnly Then
                Parallel.ForEach(PreviousSelectedIndexes, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            Else
                Parallel.For(0, AllInstalledFonts.Count, Sub(i) InstalledBag.TryAdd(i, AllInstalledFonts(i)))
            End If

            Parallel.ForEach(InstalledBag, Sub(InstalledFont, State)
                                               If GenerateFontsCancellation.Token.IsCancellationRequested Then
                                                   State.Stop()
                                               End If

                                               Using Bitmap As Bitmap = CreateBitmap(ItemWidth, ItemHeight)
                                                   Using Graphics As Graphics = Graphics.FromImage(Bitmap)
                                                       Graphics.Clear(Drawing.Color.Transparent)
                                                       Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                                       Graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                                       Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                                       Graphics.TextRenderingHint = Text.TextRenderingHint.AntiAliasGridFit

                                                       Using Font As Font = New Font(InstalledFont.Value, FontSize, SelectedStyle, GraphicsUnit.Pixel)
                                                           Dim StringDraw As String = Nothing

                                                           If ((Not IgnoreCustomString) AndAlso IsCustomStringSet) Then
                                                               StringDraw = CustomString
                                                           ElseIf (Not String.IsNullOrWhiteSpace(Font.Name)) Then
                                                               StringDraw = Font.Name
                                                           Else
                                                               StringDraw = MissingName
                                                           End If

                                                           'TODO : Fix the weird invertion of the colors red and blue but not green?!
                                                           '        Possibly caused by the blendstate
                                                           '        Further testing showed that it worked correctly in a new WinForm project.
                                                           Graphics.DrawString(StringDraw, Font, (New SolidBrush(SelectedColor)), DrawPoint)
                                                       End Using

                                                       Graphics.Save()
                                                   End Using
                                                   BitmapBag.TryAdd(InstalledFont.Key, New Bitmap(Bitmap))
                                               End Using
                                           End Sub)

            For Each BitmapItem In BitmapBag
                If GenerateFontsCancellation.Token.IsCancellationRequested Then
                    GenerateFontsCancellation = New CancellationTokenSource
                    Return Nothing
                End If

                'TODO : Since we are forced to use GraphicsDevice to create textures, this line below slows down the entire process.
                TextureBag.TryAdd(BitmapItem.Key, BitmapToTexture2DLockBits(BitmapItem.Value))
            Next

            Parallel.ForEach(TextureBag, Sub(TextureItem, State)
                                             If GenerateFontsCancellation.Token.IsCancellationRequested Then
                                                 State.Stop()
                                             End If

                                             Dim Image As New Entities.Image(TextureItem.Value, ItemSizeVector)
                                             Image.Background = New ColoredRectangle(FontBackgroundColor, Image.GetActualDestRect.Size.ToVector2, Anchor.TopLeft)
                                             ImageBag.TryAdd(TextureItem.Key, Image)
                                         End Sub)

            If GenerateFontsCancellation.Token.IsCancellationRequested Then
                GenerateFontsCancellation = New CancellationTokenSource
                Return Nothing
            End If

            Return New SortedDictionary(Of Integer, Entities.Image)(ImageBag)
        End Function



    End Class

End Namespace