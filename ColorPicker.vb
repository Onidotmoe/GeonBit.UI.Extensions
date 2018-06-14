Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports GeonBit.UI.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace GeonBit.UI.Entities

    Public Class ColorPicker
        Inherits Entity
        Implements IDisposable

        ''' <summary>Currently calculated destination rect (eg the region this entity Is drawn on).</summary>
        Public Shadows _destRect As Rectangle

        Private GridSelect As GridSelect
        Private Image_Main As Image
        Private Image_SliderColor As Image
        Private Image_SliderOpacity As Image
        Private Button_OK As Button
        Private Button_Cancel As Button

        Private RadioButton_Hue As RadioButton
        Private RadioButton_Sat As RadioButton
        Private RadioButton_Brightness As RadioButton
        Private Label_Hue As Label
        Private Label_Sat As Label
        Private Label_Brightness As Label
        Private TextInput_Hue As TextInput
        Private TextInput_Sat As TextInput
        Private TextInput_Brightness As TextInput

        Private RadioButton_Red As RadioButton
        Private RadioButton_Green As RadioButton
        Private RadioButton_Blue As RadioButton
        Private Label_Red As Label
        Private Label_Green As Label
        Private Label_Blue As Label
        Private TextInput_Red As TextInput
        Private TextInput_Green As TextInput
        Private TextInput_Blue As TextInput

        Private RadioButton_L As RadioButton
        Private RadioButton_A As RadioButton
        Private RadioButton_B As RadioButton
        Private Label_L As Label
        Private Label_A As Label
        Private Label_B As Label
        Private TextInput_L As TextInput
        Private TextInput_A As TextInput
        Private TextInput_B As TextInput

        Private Label_Cyan As Label
        Private Label_Magenta As Label
        Private Label_Yellow As Label
        Private Label_Key As Label
        Private TextInput_Cyan As TextInput
        Private TextInput_Magenta As TextInput
        Private TextInput_Yellow As TextInput
        Private TextInput_Key As TextInput

        Private TextInput_Opacity As TextInput

        Private Checkbox_WebColorsOnly As CheckBox
        Private SliderColor As SliderVertical
        Private SliderOpacity As SliderVertical
        Private TextInput_HTML As TextInput

        Private Label_Hue_Symbol As Label
        Private Label_Saturation_Symbol As Label
        Private Label_Black_Symbol As Label
        Private Label_Cyan_Symbol As Label
        Private Label_Magenta_Symbol As Label
        Private Label_Yellow_Symbol As Label
        Private Label_Key_Symbol As Label

        Private Panel_CurrentNew As Panel
        Private Label_New As Label
        Private Label_Current As Label
        Private Label_New_HTML As Label
        Private Label_Current_HTMl As Label
        Private Color_PreWebSafe As RGB
        Private _Color_Current As RGB

        Private _Alpha As Integer

        Private Property Alpha As Integer
            Get
                Return _Alpha
            End Get
            Set
                _Alpha = Value
                Image_New.Texture = CreateTexture(Image_New.Texture.Width, Image_New.Texture.Height, RGBToColor(New RGB(RGB.R, RGB.G, RGB.B, Value)))
            End Set
        End Property

        Property Color_New As RGB
            Get
                Return _RGB
            End Get
            Set
                _RGB = Value
                Image_Main_HSB = RGBToHSB(Value)
                Image_SliderColor_HSB = Image_Main_HSB
                Image_New.Texture = CreateTexture(Image_New.Texture.Width, Image_New.Texture.Height, RGBToColor(New RGB(RGB.R, RGB.G, RGB.B, _Alpha)))
                Image_SliderOpacity.Texture = GetColorMapSliderOpacity(Image_SliderOpacity.Texture, Value)
            End Set
        End Property

        Property Color_Current As RGB
            Get
                Return _Color_Current
            End Get
            Set
                _Color_Current = Value
                Image_Current.Texture = CreateTexture(Image_Current.Texture.Width, Image_Current.Texture.Height, RGBToColor(Value))
            End Set
        End Property

        Private Image_New As Image
        Private Image_Current As Image
        Private Image_Main_HSB As New HSB
        Private Image_SliderColor_HSB As HSB
        Private Image_ColorComponent As ColorComponent
        Private Old_Image_ColorComponent As ColorComponent
        Private Old_Image_Main_HSB As New HSB

        Private _HSB As New HSB

        Private Property HSB As HSB
            Get
                Return _HSB
            End Get
            Set
                _HSB = Value
                _RGB = HSBToRGB(Value)
                _CMYK = RGBToCMYK(_RGB)
                _LAB = RGBToLAB(_RGB)
            End Set
        End Property

        Private _RGB As New RGB

        Public Property RGB As RGB
            Get
                Return _RGB
            End Get
            Set
                Value.A = _Alpha
                _RGB = Value
                _HSB = RGBToHSB(Value)
                _CMYK = RGBToCMYK(Value)
                _LAB = RGBToLAB(Value)
            End Set
        End Property

        Private _CMYK As New CMYK

        Public Property CMYK As CMYK
            Get
                Return _CMYK
            End Get
            Set
                _CMYK = Value
                _RGB = CMYKToRGB(Value)
                _HSB = RGBToHSB(_RGB)
                _LAB = RGBToLAB(_RGB)
            End Set
        End Property

        Private _LAB As New LAB

        Public Property LAB As LAB
            Get
                Return _LAB
            End Get
            Set
                _LAB = Value
                _RGB = LABToRGB(Value)
                _CMYK = RGBToCMYK(_RGB)
                _HSB = RGBToHSB(_RGB)
            End Set
        End Property

        Private Size_Input As Vector2
        Private Size_Radio As Vector2
        Private Label_Invalid As Label

        Public DialogResult As DialogResult
        Protected _renderTarget As RenderTarget2D = Nothing
        Private _SliderColor_Value As Integer

        Property SliderColor_Value As Integer
            Get
                Return _SliderColor_Value
            End Get
            Set
                'Inverses the value to flip the slider upside down
                _SliderColor_Value = CInt(SliderColor.Max - Value)
            End Set
        End Property

        Private _SliderOpacity_Value As Integer

        Property SliderOpacity_Value As Integer
            Get
                Return _SliderOpacity_Value
            End Get
            Set
                'Inverses the value to flip the slider upside down
                _SliderOpacity_Value = CInt(SliderOpacity.Max - Value)
            End Set
        End Property

        ''' <summary>A scalable advance colorpicker.</summary>
        ''' <param name="Width">Will ignore size if set.</param>
        ''' <param name="Size">Uses the smallest axes.</param>
        ''' <param name="StartingColor">Defaults to Color.Black</param>
        ''' <param name="OkCancelVisible">If the Ok and Cancel buttons should be visible.</param>
        Sub New(Optional Width As Integer = Nothing, Optional Size As Vector2 = Nothing, Optional StartingColor As Color = Nothing, Optional Anchor As Anchor = Anchor.Auto, Optional Offset As Vector2 = Nothing, Optional OkCancelVisible As Boolean = False)
            _anchor = Anchor
            StartingColor = If(StartingColor = Nothing, Color.Black, StartingColor)

            If (Width <> Nothing) Then
                Size = New Vector2(Width, CInt(Width / 2))
            Else
                Dim MinSize As Single = If((Size.X < Size.Y), Size.X, Size.Y)
                Size = New Vector2(MinSize, (MinSize / 2))
            End If

            Me.Size = Size
            Size_Input = New Vector2(CSng(Size.X * 0.15), CSng(Size.Y * 0.05))
            Size_Radio = New Vector2(CSng(Size.X * 0.05), CSng(Size.Y * 0.05))
            Me._offset = Offset
            InitializeComponent(StartingColor)
        End Sub

        Private Sub InitializeComponent(StartingColor As Color)
            Dim TopLeft As Anchor = Anchor.TopLeft

            Dim ScaleFactor As Single = (Size.X / Size.Y)
            Dim Scale_Radio As Single = (ScaleFactor / 4.0F)
            Dim Scale_Label As Single = (ScaleFactor / 2.8F)
            Dim Scale_Input As Single = (ScaleFactor / 4.2F)
            Dim CharLimit As Integer = 5
            Dim MultiLine As Boolean = False
            Dim SymbolOffsetMod = (Size_Input.X * 1.05F)

            Dim Column_X0 As Single = (Size.Y * 0.05F)
            Dim Column_X1 As Single = (Size.X * 0.48F)
            Dim Column_X2 As Single = (Size.X * 0.53F)
            Dim Column_X3 As Single = (Size.X * 0.56F)
            Dim Column_X4 As Single = (Size.X * 0.74F)
            Dim Column_X5 As Single = (Size.X * 0.79F)
            Dim Column_X6 As Single = (Size.X * 0.82F)

            Dim Row_Y0 As Single = (Size.Y * 0.05F)
            Dim Row_Y1 As Single = (Size.Y * 0.18F)
            Dim Row_Y2 As Single = (Size.Y * 0.38F)
            Dim Row_Y3 As Single = (Size.Y * 0.44F)
            Dim Row_Y4 As Single = (Size.Y * 0.5F)
            Dim Row_Y5 As Single = (Size.Y * 0.58F)
            Dim Row_Y6 As Single = (Size.Y * 0.64F)
            Dim Row_Y7 As Single = (Size.Y * 0.7F)
            Dim Row_Y8 As Single = (Size.Y * 0.76F)
            Dim Row_Y9 As Single = (Size.Y * 0.82F)

            Image_Main_HSB = _HSB
            Image_SliderColor_HSB = _HSB

            Dim TransparentBackgroundPlaceholder As Texture2D = Tetra.Tetra.Instance.Content.Load(Of Texture2D)(Tetra.Tetra.Instance.GUIRoot & Tetra.Tetra.Instance.Theme & "/textures/TransparentBackgroundPlaceholder")

            Background = New ColoredRectangle(Tetra.Tetra.GetService(Of Globals).GiveItem(Of Dictionary.Brush)("CA00").Color, Size, TopLeft)
            Button_OK = New Button("Ok", TopLeft, New Vector2((Size.X * 0.18F), (Size.Y * 0.11F)), New Vector2(Column_X4, Row_Y0)) With {.Scale = 0.5F, .OnClick = Sub() Button_OK_Click()}
            Button_Cancel = New Button("Cancel", TopLeft, New Vector2((Size.X * 0.18F), (Size.Y * 0.11F)), New Vector2(Column_X4, Row_Y1)) With {.Scale = 0.5F, .OnClick = Sub() Button_Cancel_Click()}

            RadioButton_Hue = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y2)) With {.Scale = Scale_Radio, .Padding = New Vector2((Size.X * 0.01F), 0), .Checked = True, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Hue, ColorComponent.Hue)}
            RadioButton_Sat = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y3)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Sat, ColorComponent.Saturation)}
            RadioButton_Brightness = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y4)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Brightness, ColorComponent.Brightness)}
            Label_Hue = New Label("H:", TopLeft, offset:=New Vector2(Column_X2, Row_Y2)) With {.Scale = Scale_Label}
            Label_Sat = New Label("S:", TopLeft, offset:=New Vector2(Column_X2, Row_Y3)) With {.Scale = Scale_Label}
            Label_Brightness = New Label("B:", TopLeft, offset:=New Vector2(Column_X2, Row_Y4)) With {.Scale = Scale_Label}
            TextInput_Hue = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y2)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Hue_OnValueChange()}
            TextInput_Sat = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y3)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Sat_OnValueChange()}
            TextInput_Brightness = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y4)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Black_OnValueChange()}

            RadioButton_Red = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y5)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Red, ColorComponent.Red)}
            RadioButton_Green = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y6)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Green, ColorComponent.Green)}
            RadioButton_Blue = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X1, Row_Y7)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_Blue, ColorComponent.Blue)}
            Label_Red = New Label("R:", TopLeft, offset:=New Vector2(Column_X2, Row_Y5)) With {.Scale = Scale_Label}
            Label_Green = New Label("G:", TopLeft, offset:=New Vector2(Column_X2, Row_Y6)) With {.Scale = Scale_Label}
            Label_Blue = New Label("B:", TopLeft, offset:=New Vector2(Column_X2, Row_Y7)) With {.Scale = Scale_Label}
            TextInput_Red = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y5)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Red_OnValueChange()}
            TextInput_Green = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y6)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Green_OnValueChange()}
            TextInput_Blue = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X3, Row_Y7)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Blue_OnValueChange()}

            RadioButton_L = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X4, Row_Y2)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_L, ColorComponent.Lightness)}
            RadioButton_A = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X4, Row_Y3)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_A, ColorComponent.Green_Red)}
            RadioButton_B = New RadioButton(String.Empty, TopLeft, Size_Radio, New Vector2(Column_X4, Row_Y4)) With {.Scale = Scale_Radio, .OnValueChange = Sub() Color_CheckedChanged(RadioButton_B, ColorComponent.Blue_Yellow)}
            Label_L = New Label("L:", TopLeft, offset:=New Vector2(Column_X5, Row_Y2)) With {.Scale = Scale_Label}
            Label_A = New Label("a:", TopLeft, offset:=New Vector2(Column_X5, Row_Y3)) With {.Scale = Scale_Label}
            Label_B = New Label("b:", TopLeft, offset:=New Vector2(Column_X5, Row_Y4)) With {.Scale = Scale_Label}
            TextInput_L = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y2)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_L_OnValueChange()}
            TextInput_A = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y3)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_A_OnValueChange()}
            TextInput_B = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y4)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_B_OnValueChange()}

            Label_Cyan = New Label("C:", TopLeft, Size_Radio, New Vector2(Column_X5, Row_Y5)) With {.Scale = Scale_Label}
            Label_Magenta = New Label("M:", TopLeft, Size_Radio, New Vector2(Column_X5, Row_Y6)) With {.Scale = Scale_Label}
            Label_Yellow = New Label("Y:", TopLeft, Size_Radio, New Vector2(Column_X5, Row_Y7)) With {.Scale = Scale_Label}
            Label_Key = New Label("K:", TopLeft, Size_Radio, New Vector2(Column_X5, Row_Y8)) With {.Scale = Scale_Label}
            TextInput_Cyan = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y5)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Cyan_OnValueChange()}
            TextInput_Magenta = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y6)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Magenta_OnValueChange()}
            TextInput_Yellow = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y7)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Yellow_OnValueChange()}
            TextInput_Key = New TextInput(MultiLine, Size_Input, TopLeft, New Vector2(Column_X6, Row_Y8)) With {.Scale = Scale_Input, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Key_OnValueChange()}

            Checkbox_WebColorsOnly = New CheckBox("Only Web Colors", TopLeft, New Vector2((Size.X * 0.3F), (Size.Y * 0.2F)), New Vector2((Size.Y * 0.05F), (Size.Y * 0.8F))) With {.Scale = 0.5F, .OnValueChange = Sub() Checkbox_WebColorsOnly_CheckedChanged()}
            TextInput_HTML = New TextInput(False, New Vector2(Size_Input.X, (Size.Y * 0.08F)), TopLeft, New Vector2(Column_X3, (Size.Y * 0.78F))) With {.LimitBySize = False, .CharactersLimit = 8, .OnValueChange = Sub() TextInput_HTML_OnValueChange()}
            TextInput_Opacity = New TextInput(False, New Vector2(Size_Input.X, (Size.Y * 0.08F)), TopLeft, New Vector2((Size.X * 0.38F), (Size.Y * 0.78F))) With {.LimitBySize = False, .CharactersLimit = CharLimit, .OnValueChange = Sub() TextInput_Opacity_OnValueChange()}
            Label_Invalid = New Label("Hello there...", TopLeft, New Vector2((Size.X * 0.6F), (Size.Y * 0.1F)), New Vector2((Size.X * 0.35F), (Size.Y * 0.88F))) With {.Scale = Scale_Label}

            SliderColor = New SliderVertical(0, 255, New Vector2((Size.X * 0.05F), (Size.X * 0.35F)), TopLeft, New Vector2((Size.X * 0.38F), Row_Y0), skin:=SliderVerticalSkin.MarkOnly) With {.OnValueChange = Sub() SliderColor_OnValueChange(), .Padding = Vector2.Zero}
            Image_SliderColor = New Image(CreateTexture(CInt(SliderColor.Size.X * 0.8F), CInt(SliderColor.Size.Y), Color.Transparent), New Vector2((SliderColor.Size.X * 0.8F), SliderColor.Size.Y), offset:=New Vector2(CInt(SliderColor.Size.X * 0.1F), 0))
            Image_SliderColor.Texture = GetColorMapSlider(Image_SliderColor.Texture, Image_SliderColor_HSB)
            SliderColor.Background = Image_SliderColor

            SliderOpacity = New SliderVertical(0, 255, SliderColor.Size, TopLeft, New Vector2((Size.X * 0.43F), Row_Y0), skin:=SliderVerticalSkin.MarkOnly) With {.OnValueChange = Sub() SliderOpacity_OnValueChange(), .Padding = Vector2.Zero}
            Image_SliderOpacity = New Image(CreateTexture(CInt(SliderOpacity.Size.X * 0.8F), CInt(SliderOpacity.Size.Y), Color.Transparent), New Vector2(CInt(SliderOpacity.Size.X * 0.8F), CInt(SliderOpacity.Size.Y)), offset:=New Vector2(CInt(SliderOpacity.Size.X * 0.08F), 0))
            SliderOpacity.Background = Image_SliderOpacity
            Image_SliderOpacity.Background = New Image(TransparentBackgroundPlaceholder, New Vector2(CInt(SliderOpacity.Size.X * 0.75F), CInt(SliderOpacity.Size.Y)), drawMode:=ImageDrawMode.Panel, offset:=New Vector2(1, 0))
            Image_SliderOpacity.Padding = Vector2.Zero

            GridSelect = New GridSelect(New Vector2((Size.X * 0.35F), (Size.X * 0.35F)), anchor:=TopLeft, offset:=New Vector2(Column_X0, Row_Y0)) With {.OnValueChange = Sub() GridSelect_OnValueChange(), .Padding = Vector2.Zero}
            Image_Main = New Image(CreateTexture(CInt(GridSelect.Size.X), CInt(GridSelect.Size.Y), Color.Transparent), GridSelect.Size, anchor:=TopLeft)
            GridSelect.Background = Image_Main

            Label_Hue_Symbol = New Label("°", TopLeft, offset:=New Vector2((Column_X3 + SymbolOffsetMod), Row_Y2)) With {.Scale = Scale_Label}
            Label_Saturation_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X3 + SymbolOffsetMod), Row_Y3)) With {.Scale = Scale_Label}
            Label_Black_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X3 + SymbolOffsetMod), Row_Y4)) With {.Scale = Scale_Label}

            Label_Cyan_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X6 + SymbolOffsetMod), Row_Y5)) With {.Scale = Scale_Label}
            Label_Magenta_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X6 + SymbolOffsetMod), Row_Y6)) With {.Scale = Scale_Label}
            Label_Yellow_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X6 + SymbolOffsetMod), Row_Y7)) With {.Scale = Scale_Label}
            Label_Key_Symbol = New Label("%", TopLeft, offset:=New Vector2((Column_X6 + SymbolOffsetMod), Row_Y8)) With {.Scale = Scale_Label}

            Label_New_HTML = New Label("#", TopLeft, offset:=New Vector2(Column_X3 - (Size.X * 0.02F), (Size.Y * 0.8F))) With {.Scale = Scale_Label}
            Label_Current_HTMl = New Label("#", TopLeft, offset:=New Vector2(Column_X6, (Size.Y * 0.82F))) With {.Scale = Scale_Label}

            Panel_CurrentNew = New Panel(New Vector2((Size.X * 0.15F), (Size.Y * 0.3F)), anchor:=TopLeft, offset:=New Vector2(Column_X3, Row_Y0))
            Label_New = New Label("new", TopLeft, New Vector2((Panel_CurrentNew.Size.X * 0.8F), (Panel_CurrentNew.Size.Y * 0.1F)), offset:=New Vector2((Panel_CurrentNew.Size.X * 0.1F), (Panel_CurrentNew.Size.Y * 0.03F))) With {.Scale = 0.8F, .AlignToCenter = True}
            Label_Current = New Label("current", TopLeft, New Vector2((Panel_CurrentNew.Size.X * 0.8F), (Panel_CurrentNew.Size.Y * 0.1F)), offset:=New Vector2((Panel_CurrentNew.Size.X * 0.1F), (Panel_CurrentNew.Size.Y * 0.8F))) With {.Scale = 0.8F, .AlignToCenter = True}
            Image_New = New Image(CreateTexture(0, 0, Color.Black), New Vector2((Panel_CurrentNew.Size.X * 0.8F), (Panel_CurrentNew.Size.Y * 0.3F)), anchor:=TopLeft, offset:=New Vector2((Panel_CurrentNew.Size.X * 0.1F), (Panel_CurrentNew.Size.Y * 0.2F))) With {.Padding = Vector2.Zero}
            Image_Current = New Image(CreateTexture(0, 0, Color.Black), New Vector2((Panel_CurrentNew.Size.X * 0.8F), (Panel_CurrentNew.Size.Y * 0.3F)), anchor:=TopLeft, offset:=New Vector2((Panel_CurrentNew.Size.X * 0.1F), (Panel_CurrentNew.Size.Y * 0.5F))) With {.Padding = Vector2.Zero}
            Image_New.Background = New Image(TransparentBackgroundPlaceholder, Image_New.Size, anchor:=TopLeft, drawMode:=ImageDrawMode.Panel, offset:=Vector2.Zero)
            Image_Current.Background = New Image(TransparentBackgroundPlaceholder, Image_Current.Size, anchor:=TopLeft, drawMode:=ImageDrawMode.Panel, offset:=Vector2.Zero)

            Panel_CurrentNew.AddChild(Label_New)
            Panel_CurrentNew.AddChild(Image_New)
            Panel_CurrentNew.AddChild(Image_Current)
            Panel_CurrentNew.AddChild(Label_Current)

            RadioButton_Hue.Checked = True

            AddChild(GridSelect)
            AddChild(Button_OK)
            AddChild(Button_Cancel)

            AddChild(RadioButton_Hue)
            AddChild(RadioButton_Sat)
            AddChild(RadioButton_Brightness)
            AddChild(Label_Hue)
            AddChild(Label_Sat)
            AddChild(Label_Brightness)
            AddChild(TextInput_Hue)
            AddChild(TextInput_Sat)
            AddChild(TextInput_Brightness)

            AddChild(RadioButton_Red)
            AddChild(RadioButton_Green)
            AddChild(RadioButton_Blue)
            AddChild(Label_Red)
            AddChild(Label_Green)
            AddChild(Label_Blue)
            AddChild(TextInput_Red)
            AddChild(TextInput_Green)
            AddChild(TextInput_Blue)

            AddChild(RadioButton_L)
            AddChild(RadioButton_A)
            AddChild(RadioButton_B)
            AddChild(Label_L)
            AddChild(Label_A)
            AddChild(Label_B)
            AddChild(TextInput_L)
            AddChild(TextInput_A)
            AddChild(TextInput_B)

            AddChild(Label_Cyan)
            AddChild(Label_Magenta)
            AddChild(Label_Yellow)
            AddChild(Label_Key)
            AddChild(TextInput_Cyan)
            AddChild(TextInput_Magenta)
            AddChild(TextInput_Yellow)
            AddChild(TextInput_Key)

            AddChild(Checkbox_WebColorsOnly)
            AddChild(SliderColor)
            AddChild(SliderOpacity)
            AddChild(TextInput_HTML)
            AddChild(TextInput_Opacity)
            AddChild(Label_Invalid)

            AddChild(Label_Hue_Symbol)
            AddChild(Label_Saturation_Symbol)
            AddChild(Label_Black_Symbol)
            AddChild(Label_Cyan_Symbol)
            AddChild(Label_Magenta_Symbol)
            AddChild(Label_Yellow_Symbol)

            AddChild(Panel_CurrentNew)
            AddChild(Label_New_HTML)
            AddChild(Label_Current_HTMl)

            Dim StartingColor_RGB = ColorToRGB(StartingColor)
            Color_New = StartingColor_RGB
            Color_Current = StartingColor_RGB

            Image_Main.Texture = GetColorMap(Image_Main.Texture, Image_Main_HSB)

            SetSlider(0, Image_ColorComponent)
            SetSliderOpacity(0)
            UpdateTextBoxes()

            Label_Current_HTMl.Text = "#"c & TextInput_HTML.Value
        End Sub

        Private Sub Checkbox_WebColorsOnly_CheckedChanged()
            If (Checkbox_WebColorsOnly.Checked) Then
                Color_PreWebSafe = RGB
                RGB = GetNearestWebSafeColor(_RGB)
            Else
                RGB = Color_PreWebSafe
            End If

            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main(True)
            Image_SliderColor.Texture = GetColorMapSlider(Image_SliderColor.Texture, Image_SliderColor_HSB)
        End Sub

#Region "Exit/Close"

        Private Sub Button_OK_Click()
            DialogResult = DialogResult.OK
            Close()
        End Sub

        Private Sub Button_Cancel_Click()
            DialogResult = DialogResult.Cancel
            Close()
        End Sub

        Private Sub Close()
            Dispose()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            DisposeRenderTarget()
        End Sub

        Private Sub DisposeRenderTarget()
            If (_renderTarget IsNot Nothing) Then
                _renderTarget.Dispose()
                _renderTarget = Nothing
            End If
        End Sub

#End Region

        Private Sub UpdateTextBoxes()
            TextInput_Hue.Value = (Math.Round(HSB.H * 360)).ToString
            TextInput_Sat.Value = (Math.Round(HSB.S * 100)).ToString
            TextInput_Brightness.Value = (Math.Round(HSB.B * 100)).ToString
            TextInput_Red.Value = (Math.Round(RGB.R)).ToString
            TextInput_Green.Value = (Math.Round(RGB.G)).ToString
            TextInput_Blue.Value = (Math.Round(RGB.B)).ToString
            TextInput_Opacity.Value = Alpha.ToString
            TextInput_L.Value = (Math.Round(LAB.L)).ToString
            TextInput_A.Value = (Math.Round(LAB.A)).ToString
            TextInput_B.Value = (Math.Round(LAB.B)).ToString
            TextInput_Cyan.Value = (Math.Round(CMYK.C * 100)).ToString
            TextInput_Magenta.Value = (Math.Round(CMYK.M * 100)).ToString
            TextInput_Yellow.Value = (Math.Round(CMYK.Y * 100)).ToString
            TextInput_Key.Value = (Math.Round(CMYK.K * 100)).ToString
            WriteHexData(RGB)
        End Sub
        ''' <summary>Checks if it has changed before updating, no update if hasn't changed.</summary>
        Sub Update_Image_Main(Optional Force As Boolean = False)
            If (Old_Image_Main_HSB <> Image_Main_HSB) OrElse (Old_Image_ColorComponent <> Image_ColorComponent) OrElse Force Then
                Old_Image_Main_HSB = Image_Main_HSB
                Old_Image_ColorComponent = Image_ColorComponent
                Image_Main.Texture = GetColorMap(Image_Main.Texture, Image_Main_HSB)
            End If
        End Sub

        Private Sub GridSelect_OnValueChange()
            GetColor()

            If ((Not (Image_ColorComponent = ColorComponent.Lightness)) AndAlso (Not (Image_ColorComponent = ColorComponent.Green_Red)) AndAlso (Not (Image_ColorComponent = ColorComponent.Blue_Yellow))) Then
                HSB = Image_Main_HSB
            End If

            Color_New = HSBToRGB(HSB)
            Image_SliderColor.Texture = GetColorMapSlider(Image_SliderColor.Texture, Image_SliderColor_HSB)

            UpdateTextBoxes()
        End Sub

        Private Sub SliderColor_OnValueChange()
            SliderColor_Value = SliderColor.Value

            ApplySliderValue(SliderColor_Value)

            If ((Not (Image_ColorComponent = ColorComponent.Lightness)) AndAlso (Not (Image_ColorComponent = ColorComponent.Green_Red)) AndAlso (Not (Image_ColorComponent = ColorComponent.Blue_Yellow))) Then
                HSB = Image_Main_HSB
            End If

            Color_New = HSBToRGB(Image_SliderColor_HSB)
            Update_Image_Main()
            UpdateTextBoxes()
        End Sub

        Private Sub SliderOpacity_OnValueChange()
            SliderOpacity_Value = SliderOpacity.Value
            TextInput_Opacity.Value = SliderOpacity_Value.ToString
            Alpha = SliderOpacity_Value
            UpdateTextBoxes()
        End Sub

        Private Sub SetSlider(Value As Integer, ColorComponent As ColorComponent)
            If (ColorComponent = Image_ColorComponent) Then
                SliderColor_Value = Value
                SliderColor.SetValueSilent(SliderColor_Value)

                ApplySliderValue(Value)
                Color_New = HSBToRGB(Image_SliderColor_HSB)
                Update_Image_Main()
                UpdateTextBoxes()
            End If
        End Sub

        Private Sub SetSlider_LAB(Value As Integer, ColorComponent As ColorComponent)
            If (ColorComponent = Image_ColorComponent) Then
                SliderColor_Value = CInt(ReverseLAB(Value))
                SliderColor.SetValueSilent(SliderColor_Value)

                ApplySliderValue(Value)
                Color_New = HSBToRGB(Image_SliderColor_HSB)
                Update_Image_Main()
                UpdateTextBoxes()
            End If
        End Sub

        Private Sub SetSliderOpacity(Value As Integer)
            SliderOpacity_Value = Value
            SliderOpacity.SetValueSilent(SliderOpacity_Value)
        End Sub

        Private Sub ApplySliderValue(SliderValue As Integer)
            Select Case Image_ColorComponent
                Case ColorComponent.Hue
                    Image_SliderColor_HSB.H = (SliderValue / 360)
                Case ColorComponent.Saturation
                    Image_SliderColor_HSB.S = (SliderValue / 100)
                Case ColorComponent.Brightness
                    Image_SliderColor_HSB.B = (SliderValue / 100)
                Case ColorComponent.Red
                    Image_SliderColor_HSB = RGBToHSB(New RGB(SliderValue, RGB.G, RGB.B))
                Case ColorComponent.Green
                    Image_SliderColor_HSB = RGBToHSB(New RGB(RGB.R, SliderValue, RGB.B))
                Case ColorComponent.Blue
                    Image_SliderColor_HSB = RGBToHSB(New RGB(RGB.R, RGB.G, SliderValue))
                Case ColorComponent.Lightness
                    LAB = New LAB(SliderValue, LAB.A, LAB.B)
                Case ColorComponent.Green_Red
                    LAB = New LAB(LAB.L, CorrectLAB(SliderValue), LAB.B)
                Case ColorComponent.Blue_Yellow
                    LAB = New LAB(LAB.L, LAB.A, CorrectLAB(SliderValue))
            End Select

            If ((Image_ColorComponent = ColorComponent.Lightness) OrElse (Image_ColorComponent = ColorComponent.Green_Red) OrElse (Image_ColorComponent = ColorComponent.Blue_Yellow)) Then
                Image_SliderColor_HSB = LABToHSB(LAB)
            End If
        End Sub

        ''' <summary>Resets the slider's min and max values to reflect the current active ColorComponent.</summary>
        Private Sub ResetSlider()
            Select Case Image_ColorComponent
                Case ColorComponent.Hue
                    SliderColor.Min = 0
                    SliderColor.Max = 360
                Case ColorComponent.Saturation
                    SliderColor.Min = 0
                    SliderColor.Max = 100
                Case ColorComponent.Brightness
                    SliderColor.Min = 0
                    SliderColor.Max = 100
                Case ColorComponent.Red
                    SliderColor.Min = 0
                    SliderColor.Max = 255
                Case ColorComponent.Green
                    SliderColor.Min = 0
                    SliderColor.Max = 255
                Case ColorComponent.Blue
                    SliderColor.Min = 0
                    SliderColor.Max = 255
                Case ColorComponent.Lightness
                    SliderColor.Min = 0
                    SliderColor.Max = 100
                Case ColorComponent.Green_Red
                    SliderColor.Min = 0
                    SliderColor.Max = 255
                Case ColorComponent.Blue_Yellow
                    SliderColor.Min = 0
                    SliderColor.Max = 255
            End Select

            SliderColor.MarkAsDirty()
            SliderColor_Value = CInt(SliderColor.Max)
        End Sub

        Private Sub GetColor()
            Dim HSB As HSB = Nothing
            Dim X = GridSelect.RelativeValue.X
            Dim Y = GridSelect.RelativeValue.Y

            Select Case Image_ColorComponent
                Case ColorComponent.Hue
                    HSB = New HSB(_HSB.H, X, (1 - Y))
                Case ColorComponent.Saturation
                    HSB = New HSB(X, _HSB.S, (1 - Y))
                Case ColorComponent.Brightness
                    HSB = New HSB(X, (1 - Y), _HSB.B)
                Case ColorComponent.Red
                    HSB = RGBToHSB(New RGB(_RGB.R, (255 - (255 * Y)), (255 * X)))
                Case ColorComponent.Green
                    HSB = RGBToHSB(New RGB((255 - (255 * Y)), (_RGB.G), (255 * X)))
                Case ColorComponent.Blue
                    HSB = RGBToHSB(New RGB((255 * X), (255 - (255 * Y)), (_RGB.B)))
                Case ColorComponent.Lightness
                    LAB = New LAB(_LAB.L, CorrectLAB(255 * X), CorrectLAB(255 * Y))
                Case ColorComponent.Green_Red
                    LAB = New LAB((100 * Y), _LAB.A, CorrectLAB(255 * X))
                Case ColorComponent.Blue_Yellow
                    LAB = New LAB((100 * Y), CorrectLAB(255 * X), _LAB.B)
            End Select

            If ((Not (Image_ColorComponent = ColorComponent.Lightness)) AndAlso (Not (Image_ColorComponent = ColorComponent.Green_Red)) AndAlso (Not (Image_ColorComponent = ColorComponent.Blue_Yellow))) Then
                Image_Main_HSB = HSB
            Else
                Image_Main_HSB = LABToHSB(LAB)
            End If
        End Sub

        Private Function GetColorMap(Old As Texture2D, HSB As HSB) As Texture2D
            Dim Width As Integer = Old.Width
            Dim Height As Integer = Old.Height
            Dim Data As Color() = New Color(Width * Height - 1) {}
            Dim Texture As Texture2D = New Texture2D(Tetra.Tetra.Instance.GraphicsDevice, Width, Height)

            'XY needs to be relative to the ColorComponent's MinMax values.
            'Keep in mind that the actual selected value isn't represented here as that value is constant; until changed with the slider.
            Dim Relative_X, Relative_Y As New Double
            Select Case Image_ColorComponent
                Case ColorComponent.Hue
                    Relative_X = (100 / Width)
                    Relative_Y = (100 / Height)
                Case ColorComponent.Saturation, ColorComponent.Brightness
                    Relative_X = (360 / Width)
                    Relative_Y = (100 / Height)
                Case ColorComponent.Red, ColorComponent.Green, ColorComponent.Blue
                    Relative_X = (255 / Width)
                    Relative_Y = (255 / Height)
                Case ColorComponent.Lightness
                    Relative_X = (255 / Width)
                    Relative_Y = (255 / Height)
                Case ColorComponent.Green_Red, ColorComponent.Blue_Yellow
                    Relative_X = (255 / Width)
                    Relative_Y = (100 / Height)
            End Select

            Parallel.For(0, Height, Sub(Y)
                                        Parallel.For(0, Width, Sub(X)
                                                                   Dim Color As Color

                                                                   Select Case Image_ColorComponent
                                                                       Case ColorComponent.Hue
                                                                           Color = HSBToColor(New HSB(HSB.H, ((Relative_X * X) / 100), (1 - ((Relative_Y * Y) / 100))))
                                                                       Case ColorComponent.Saturation
                                                                           Color = HSBToColor(New HSB(((Relative_X * X) / 360), HSB.S, (1 - ((Relative_Y * Y) / 100))))
                                                                       Case ColorComponent.Brightness
                                                                           Color = HSBToColor(New HSB(((Relative_X * X) / 360), (1 - ((Relative_Y * Y) / 100)), HSB.B))
                                                                       Case ColorComponent.Red
                                                                           Color = New Color(CInt(RGB.R), CInt(255 - (Relative_Y * Y)), CInt(Relative_X * X))
                                                                       Case ColorComponent.Green
                                                                           Color = New Color(CInt(255 - (Relative_Y * Y)), CInt(RGB.G), CInt(Relative_X * X))
                                                                       Case ColorComponent.Blue
                                                                           Color = New Color(CInt(Relative_X * X), CInt(255 - (Relative_Y * Y)), CInt(RGB.B))
                                                                       Case ColorComponent.Lightness
                                                                           Color = LABToColor(New LAB(LAB.L, CorrectLAB(Relative_X * X), CorrectLAB((255 - (Relative_Y * Y)))))
                                                                       Case ColorComponent.Green_Red
                                                                           Color = LABToColor(New LAB((100 - (Relative_Y * Y)), LAB.A, CorrectLAB(Relative_X * X)))
                                                                       Case ColorComponent.Blue_Yellow
                                                                           Color = LABToColor(New LAB((100 - (Relative_Y * Y)), CorrectLAB(Relative_X * X), LAB.B))
                                                                   End Select

                                                                   If Checkbox_WebColorsOnly.Checked Then
                                                                       Color = GetNearestWebSafeColor(Color)
                                                                   End If

                                                                   Data((Y * Width) + X) = Color
                                                               End Sub)
                                    End Sub)

            Texture.SetData(Data)
            Return Texture
        End Function

        Private Function GetColorMapSlider(Old As Texture2D, HSB As HSB) As Texture2D
            Dim Width As Integer = Old.Width
            Dim Height As Integer = Old.Height
            Dim Data As Color() = New Color(Width * Height - 1) {}
            Dim Texture As Texture2D = New Texture2D(Tetra.Tetra.Instance.GraphicsDevice, Width, Height)

            Parallel.For(0, Height, Sub(Y)
                                        Dim Color As Color

                                        Select Case Image_ColorComponent
                                            Case ColorComponent.Hue
                                                Color = HSBToColor(New HSB((1 - (Y / Height)), 1, 1))
                                            Case ColorComponent.Saturation
                                                Color = HSBToColor(New HSB(HSB.H, (1 - (Y / Height)), HSB.B))
                                            Case ColorComponent.Brightness
                                                Color = HSBToColor(New HSB(HSB.H, HSB.S, (1 - (Y / Height))))
                                            Case ColorComponent.Red
                                                Color = New Color((255 - CInt(Math.Round((255 * Y) / Height))), CInt(RGB.G), CInt(RGB.B))
                                            Case ColorComponent.Green
                                                Color = New Color(CInt(RGB.R), (255 - CInt(Math.Round((255 * Y) / Height))), CInt(RGB.B))
                                            Case ColorComponent.Blue
                                                Color = New Color(CInt(RGB.R), CInt(RGB.G), (255 - CInt(Math.Round((255 * Y) / Height))))
                                            Case ColorComponent.Lightness
                                                Color = LABToColor(New LAB(LAB.L, CorrectLAB(Y), CorrectLAB((100 * Y) / Height)))
                                            Case ColorComponent.Green_Red
                                                Color = LABToColor(New LAB(((100 * Y) / Height), LAB.A, CorrectLAB(Y)))
                                            Case ColorComponent.Blue_Yellow
                                                Color = LABToColor(New LAB(((100 * Y) / Height), CorrectLAB(Y), LAB.B))
                                            Case ColorComponent.Cyan
                                                Color = CMYKToColor(New CMYK(((100 - ((100 * Y) / Height)) / 100), CMYK.M, CMYK.Y, CMYK.K))
                                            Case ColorComponent.Magenta
                                                Color = CMYKToColor(New CMYK(CMYK.C, ((100 - ((100 * Y) / Height)) / 100), CMYK.Y, CMYK.K))
                                            Case ColorComponent.Yellow
                                                Color = CMYKToColor(New CMYK(CMYK.C, CMYK.M, ((100 - ((100 * Y) / Height)) / 100), CMYK.K))
                                            Case ColorComponent.Key
                                                Color = CMYKToColor(New CMYK(CMYK.C, CMYK.M, CMYK.Y, ((100 - ((100 * Y) / Height)) / 100)))
                                        End Select

                                        If Checkbox_WebColorsOnly.Checked Then
                                            Color = GetNearestWebSafeColor(Color)
                                        End If

                                        Parallel.For(0, Width, Sub(X)
                                                                   Data((Y * Width) + X) = Color
                                                               End Sub)
                                    End Sub)

            Texture.SetData(Data)
            Return Texture
        End Function

        Private Function GetColorMapSliderOpacity(Old As Texture2D, CurrentColor As RGB) As Texture2D
            Dim Width As Integer = Old.Width
            Dim Height As Integer = Old.Height
            Dim Data As Color() = New Color(Width * Height - 1) {}
            Dim Texture As Texture2D = New Texture2D(Tetra.Tetra.Instance.GraphicsDevice, Width, Height)
            Dim Base As Color = RGBToColor(CurrentColor)
            Dim Relative_Y As Double = (255 / Height)

            Parallel.For(0, Height, Sub(Y)
                                        Dim Color As Color = Base
                                        Color.A = Convert.ToByte(Relative_Y * Y)

                                        Parallel.For(0, Width, Sub(X)
                                                                   Data((Y * Width) + X) = Color
                                                               End Sub)
                                    End Sub)

            Array.Reverse(Data)
            Texture.SetData(Data)
            Return Texture
        End Function

        Private Sub Color_CheckedChanged(RadioButton As RadioButton, ColorComponent As ColorComponent)
            If RadioButton.Checked Then
                Image_ColorComponent = ColorComponent
                Update_Image_Main()
                Image_SliderColor.Texture = GetColorMapSlider(Image_SliderColor.Texture, Image_SliderColor_HSB)
                ResetSlider()
            End If
        End Sub

#Region "TextBoxes"

        Public Async Function DelayTask(Time As Double) As Task
            Await Task.Delay(TimeSpan.FromSeconds(Time))
        End Function

        Private Async Sub TextInput_HTML_OnValueChange()
            Await DelayTask(1.5)

            Dim Text As String = TextInput_HTML.Value.ToUpper()
            Dim HasIllegalCharacter As Boolean = False

            If (Text.Length <= 0) Then
                HasIllegalCharacter = True
            ElseIf (Text.Length < 8) OrElse (text.Length > 8) Then
                Label_Invalid.Text = "Hex must be 8 characters in lenght."
                WriteHexData(_RGB)
                Exit Sub
            Else
                For Each Letter In Text
                    If (Not Char.IsNumber(Letter)) Then
                        If ((Letter <= "A"c) OrElse (Letter >= "F"c)) Then
                            HasIllegalCharacter = True
                            Exit For
                        End If
                    End If
                Next
            End If

            If HasIllegalCharacter Then
                Label_Invalid.Text = "Hex must be a value between 00000000 and FFFFFFFF"
                WriteHexData(_RGB)
                Exit Sub
            End If

            RGB = ParseHexData(Text)
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Opacity_OnValueChange()
            Dim Text As String = TextInput_Opacity.Value

            If (Not (Await IfNotChanged(Text, TextInput_Opacity, "Opacity must be a number between 0 and 255"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 255 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Opacity.Value = "0"
                    Alpha = 0
                    SetSliderOpacity(0)
                Case > 255
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Opacity.Value = "255"
                    Alpha = 255
                    SetSliderOpacity(255)
                Case Else
                    Alpha = Value
                    TextInput_Opacity.Value = Alpha.ToString
                    SetSliderOpacity(Value)
            End Select

            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        ''' <summary>Delays action to allow userinput to finish. Checks if start-input differs from end-input, cancel further actions.</summary>
        Async Function IfNotChanged(Text As String, TextInput As TextInput, InvalidText As String) As Task(Of Boolean)
            Await DelayTask(0.5)

            If (Not Equals(Text, TextInput.Value)) Then
                Return False
            End If
            If ContainsIllegalCharacter(Text) Then
                Label_Invalid.Text = InvalidText
                UpdateTextBoxes()
                TextInput.Value = "0"
                Return False
            End If

            Return True
        End Function

        Function ContainsIllegalCharacter(Text As String) As Boolean
            If (Text.Length <= 0) Then
                Return True
            Else
                Dim Result As Integer
                If Integer.TryParse(Text, Result) Then
                    Return False
                Else
                    Return True
                End If
            End If

            Return False
        End Function

        Private Async Sub TextInput_Hue_OnValueChange()
            Dim Text As String = TextInput_Hue.Value

            If (Not (Await IfNotChanged(Text, TextInput_Hue, "Hue must be a number between 0 and 360"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 360 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Hue.Value = "0"
                    _HSB.H = 0.0
                    SetSlider(0, ColorComponent.Hue)
                Case > 360
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Hue.Value = "360"
                    _HSB.H = 1.0
                    SetSlider(360, ColorComponent.Hue)
                Case Else
                    _HSB.H = (Value / 360)
                    SetSlider(Value, ColorComponent.Hue)
            End Select

            HSB = _HSB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Sat_OnValueChange()
            Dim Text As String = TextInput_Sat.Value

            If (Not (Await IfNotChanged(Text, TextInput_Sat, "Saturation must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Sat.Value = "0"
                    _HSB.S = 0.0
                    SetSlider(0, ColorComponent.Saturation)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Sat.Value = "100"
                    _HSB.S = 1.0
                    SetSlider(100, ColorComponent.Saturation)
                Case Else
                    _HSB.S = (Value / 100)
                    SetSlider(Value, ColorComponent.Saturation)
            End Select

            HSB = _HSB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Black_OnValueChange()
            Dim Text As String = TextInput_Brightness.Value

            If (Not (Await IfNotChanged(Text, TextInput_Brightness, "Brightness must be a number between 0 and 100."))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Brightness.Value = "0"
                    _HSB.B = 0.0
                    SetSlider(0, ColorComponent.Brightness)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Brightness.Value = "100"
                    _HSB.B = 1.0
                    SetSlider(100, ColorComponent.Brightness)
                Case Else
                    _HSB.B = (Value / 100)
                    SetSlider(Value, ColorComponent.Brightness)
            End Select

            HSB = _HSB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Red_OnValueChange()
            Dim Text As String = TextInput_Red.Value

            If (Not (Await IfNotChanged(Text, TextInput_Red, "Red must be a number between 0 and 255"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 255 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Red.Value = "0"
                    _RGB.R = 0
                    SetSlider(0, ColorComponent.Red)
                Case > 255
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Red.Value = "255"
                    _RGB.R = 255
                    SetSlider(255, ColorComponent.Red)
                Case Else
                    _RGB.R = Value
                    SetSlider(Value, ColorComponent.Red)
            End Select

            RGB = _RGB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Green_OnValueChange()
            Dim Text As String = TextInput_Green.Value

            If (Not (Await IfNotChanged(Text, TextInput_Green, "Green must be a number between 0 and 255"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 255 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Green.Value = "0"
                    _RGB.G = 0
                    SetSlider(0, ColorComponent.Green)
                Case > 255
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Green.Value = "255"
                    _RGB.G = 255
                    SetSlider(255, ColorComponent.Green)
                Case Else
                    _RGB.G = Value
                    SetSlider(Value, ColorComponent.Green)
            End Select

            RGB = _RGB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Blue_OnValueChange()
            Dim Text As String = TextInput_Blue.Value

            If (Not (Await IfNotChanged(Text, TextInput_Blue, "Blue must be a number between 0 and 255"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 255 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Blue.Value = "0"
                    _RGB.B = 0
                    SetSlider(0, ColorComponent.Blue)
                Case > 255
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Blue.Value = "255"
                    _RGB.B = 255
                    SetSlider(255, ColorComponent.Blue)
                Case Else
                    _RGB.B = Value
                    SetSlider(Value, ColorComponent.Blue)
            End Select

            RGB = _RGB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Cyan_OnValueChange()
            Dim Text As String = TextInput_Cyan.Value

            If (Not (Await IfNotChanged(Text, TextInput_Cyan, "Cyan must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Cyan.Value = "0"
                    _CMYK.C = 0.0
                    SetSlider(0, ColorComponent.Cyan)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Cyan.Value = "100"
                    _CMYK.C = 1.0
                    SetSlider(100, ColorComponent.Cyan)
                Case Else
                    _CMYK.C = (Value / 100)
                    SetSlider(Value, ColorComponent.Cyan)
            End Select

            CMYK = _CMYK
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Magenta_OnValueChange()
            Dim Text As String = TextInput_Magenta.Value

            If (Not (Await IfNotChanged(Text, TextInput_Magenta, "Magenta must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Magenta.Value = "0"
                    _CMYK.M = 0.0
                    SetSlider(0, ColorComponent.Magenta)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Magenta.Value = "100"
                    _CMYK.M = 1.0
                    SetSlider(100, ColorComponent.Magenta)
                Case Else
                    _CMYK.M = (Value / 100)
                    SetSlider(Value, ColorComponent.Magenta)
            End Select

            CMYK = _CMYK
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Yellow_OnValueChange()
            Dim Text As String = TextInput_Yellow.Value

            If (Not (Await IfNotChanged(Text, TextInput_Yellow, "Yellow must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Yellow.Value = "0"
                    _CMYK.Y = 0.0
                    SetSlider(0, ColorComponent.Yellow)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Yellow.Value = "100"
                    _CMYK.Y = 1.0
                    SetSlider(100, ColorComponent.Yellow)
                Case Else
                    _CMYK.Y = (Value / 100)
                    SetSlider(Value, ColorComponent.Yellow)
            End Select

            CMYK = _CMYK
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_Key_OnValueChange()
            Dim Text As String = TextInput_Key.Value

            If (Not (Await IfNotChanged(Text, TextInput_Key, "Key must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)

            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Key.Value = "0"
                    _CMYK.K = 0.0
                    SetSlider(0, ColorComponent.Key)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_Key.Value = "100"
                    _CMYK.K = 1.0
                    SetSlider(100, ColorComponent.Key)
                Case Else
                    _CMYK.K = (Value / 100)
                    SetSlider(Value, ColorComponent.Key)
            End Select

            CMYK = _CMYK
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_L_OnValueChange()
            Dim Text As String = TextInput_L.Value

            If (Not (Await IfNotChanged(Text, TextInput_L, "Lightness must be a number between 0 and 100"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between 0 and 100 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < 0
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_L.Value = "0"
                    _LAB.L = 0
                    SetSlider(0, ColorComponent.Lightness)
                Case > 100
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_L.Value = "100"
                    _LAB.L = 100
                    SetSlider(100, ColorComponent.Lightness)
                Case Else
                    _LAB.L = Value
                    SetSlider(Value, ColorComponent.Lightness)
            End Select

            LAB = _LAB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_A_OnValueChange()
            Dim Text As String = TextInput_A.Value

            If (Not (Await IfNotChanged(Text, TextInput_A, "Green-Red must be a number between -128 and 127"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between -128 and 127 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < -128
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_A.Value = "-128"
                    _LAB.A = LAB.Min
                    SetSlider_LAB(0, ColorComponent.Green_Red)
                Case > 127
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_A.Value = "127"
                    _LAB.A = LAB.Max
                    SetSlider_LAB(255, ColorComponent.Green_Red)
                Case Else
                    _LAB.A = Value
                    SetSlider_LAB(Value, ColorComponent.Green_Red)
            End Select

            LAB = _LAB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

        Private Async Sub TextInput_B_OnValueChange()
            Dim Text As String = TextInput_B.Value

            If (Not (Await IfNotChanged(Text, TextInput_B, "Blue-Yellow must be a number between -128 and 127"))) Then
                Exit Sub
            End If

            Dim Text_Invalid_Range = "An integer between -128 and 127 is required. Closest value inserted."
            Dim Value As Integer = Integer.Parse(Text)
            Select Case Value
                Case < -128
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_B.Value = "-128"
                    _LAB.B = LAB.Min
                    SetSlider_LAB(0, ColorComponent.Blue_Yellow)
                Case > 127
                    Label_Invalid.Text = Text_Invalid_Range
                    TextInput_B.Value = "127"
                    _LAB.B = LAB.Max
                    SetSlider_LAB(255, ColorComponent.Blue_Yellow)
                Case Else
                    _LAB.B = Value
                    SetSlider_LAB(Value, ColorComponent.Blue_Yellow)
            End Select

            LAB = _LAB
            Color_New = RGB
            UpdateTextBoxes()
            Update_Image_Main()
        End Sub

#End Region

#Region "Converters"

        Private Sub WriteHexData(RGB As RGB)
            Dim Color = RGBToColor(RGB)
            Dim Red As String = Convert.ToString(Color.R, 16)
            Dim Green As String = Convert.ToString(Color.G, 16)
            Dim Blue As String = Convert.ToString(Color.B, 16)
            Dim Alpha As String = Convert.ToString(Convert.ToByte(_Alpha), 16)

            If (Red.Length < 2) Then
                Red = ("0" & Red)
            End If
            If (Green.Length < 2) Then
                Green = ("0" & Green)
            End If
            If (Blue.Length < 2) Then
                Blue = ("0" & Blue)
            End If
            If (Alpha.Length < 2) Then
                Alpha = ("0" & Alpha)
            End If

            TextInput_HTML.Value = (Alpha.ToUpper() & Red.ToUpper() & Green.ToUpper() & Blue.ToUpper())
        End Sub

        Private Function ParseHexData(Hex_Data As String) As RGB
            Hex_Data = "00000000" & Hex_Data
            Hex_Data = Hex_Data.Remove(0, Hex_Data.Length - 8)
            Dim R_Text, G_Text, B_Text, A_Text As String
            Dim R, G, B, A As Integer
            A_Text = Hex_Data.Substring(0, 2)
            R_Text = Hex_Data.Substring(2, 2)
            G_Text = Hex_Data.Substring(4, 2)
            B_Text = Hex_Data.Substring(6, 2)
            R = Integer.Parse(R_Text, Globalization.NumberStyles.HexNumber)
            G = Integer.Parse(G_Text, Globalization.NumberStyles.HexNumber)
            B = Integer.Parse(B_Text, Globalization.NumberStyles.HexNumber)
            A = Integer.Parse(A_Text, Globalization.NumberStyles.HexNumber)

            Alpha = A
            Return New RGB(R, G, B)
        End Function

        Private Function CorrectLAB(Value As Double) As Double
            Return (Value - 128)
        End Function

        Private Function ReverseLAB(Value As Double) As Double
            Return (Value + 128)
        End Function

        Private Function HSBToColor(HSB As HSB) As Color
            Return RGBToColor(HSBToRGB(HSB))
        End Function

        Private Function LABToColor(LAB As LAB) As Color
            Return RGBToColor(LABToRGB(LAB))
        End Function

        Private Function LABToRGB(LAB As LAB) As RGB
            Return XYZToRGB(LABtoXYZ(LAB))
        End Function

        Private Function LABToHSB(LAB As LAB) As HSB
            Return RGBToHSB(LABToRGB(LAB))
        End Function

        Private Function RGBToLAB(RGB As RGB) As LAB
            Return XYZToLAB(RGBToXYZ(RGB))
        End Function

        Public Function LABtoXYZ(LAB As LAB) As XYZ
            Dim X, Y, Z As New Double
            Y = ((LAB.L + 16.0) / 116.0)
            X = ((LAB.A / 500.0) + Y)
            Z = (Y - (LAB.B / 200.0))

            Dim Pow_X = Math.Pow(X, 3.0)
            Dim Pow_Y = Math.Pow(Y, 3.0)
            Dim Pow_Z = Math.Pow(Z, 3.0)

            Dim Less = (216 / 24389)

            If (Pow_X > Less) Then
                X = Pow_X
            Else
                X = ((X - (16.0 / 116.0)) / 7.787)
            End If
            If (Pow_Y > Less) Then
                Y = Pow_Y
            Else
                Y = ((Y - (16.0 / 116.0)) / 7.787)
            End If
            If (Pow_Z > Less) Then
                Z = Pow_Z
            Else
                Z = ((Z - (16.0 / 116.0)) / 7.787)
            End If

            Return New XYZ((X * 95.047), (Y * 100.0), (Z * 108.883))
        End Function

        Private Function XYZToRGB(XYZ As XYZ) As RGB
            Dim X, Y, Z As New Double
            Dim R, G, B As New Double
            Dim Pow As Double = (1.0 / 2.4)
            Dim Less As Double = 0.0031308

            X = (XYZ.X / 100)
            Y = (XYZ.Y / 100)
            Z = (XYZ.Z / 100)

            R = ((X * 3.24071) + (Y * -1.53726) + (Z * -0.498571))
            G = ((X * -0.969258) + (Y * 1.87599) + (Z * 0.0415557))
            B = ((X * 0.0556352) + (Y * -0.203996) + (Z * 1.05707))

            If (R > Less) Then
                R = ((1.055 * Math.Pow(R, Pow)) - 0.055)
            Else
                R *= 12.92
            End If
            If (G > Less) Then
                G = ((1.055 * Math.Pow(G, Pow)) - 0.055)
            Else
                G *= 12.92
            End If
            If (B > Less) Then
                B = ((1.055 * Math.Pow(B, Pow)) - 0.055)
            Else
                B *= 12.92
            End If

            Return New RGB((R * 255), (G * 255), (B * 255))
        End Function

        Private Function RGBToXYZ(RGB As RGB) As XYZ
            Dim X, Y, Z As New Double
            Dim R, G, B As New Double
            Dim Less As Double = 0.04045

            R = (RGB.R / 255)
            G = (RGB.G / 255)
            B = (RGB.B / 255)

            If (R > Less) Then
                R = Math.Pow(((R + 0.055) / 1.055), 2.4)
            Else
                R = (R / 12.92)
            End If
            If (G > Less) Then
                G = Math.Pow(((G + 0.055) / 1.055), 2.4)
            Else
                G = (G / 12.92)
            End If
            If (B > Less) Then
                B = Math.Pow(((B + 0.055) / 1.055), 2.4)
            Else
                B = (B / 12.92)
            End If

            X = ((R * 0.4124) + (G * 0.3576) + (B * 0.1805))
            Y = ((R * 0.2126) + (G * 0.7152) + (B * 0.0722))
            Z = ((R * 0.0193) + (G * 0.1192) + (B * 0.9505))

            Return New XYZ(X * 100, Y * 100, Z * 100)
        End Function

        Private Function XYZToLAB(XYZ As XYZ) As LAB
            Dim X, Y, Z As New Double
            Dim L, A, B As New Double
            Dim Less As Double = 0.008856
            Dim Pow As Double = (1.0 / 3.0)

            X = ((XYZ.X / 100) / 0.9505)
            Y = (XYZ.Y / 100)
            Z = ((XYZ.Z / 100) / 1.089)

            If (X > Less) Then
                X = Math.Pow(X, Pow)
            Else
                X = ((7.787 * X) + (16.0 / 116.0))
            End If
            If (Y > Less) Then
                Y = Math.Pow(Y, Pow)
            Else
                Y = ((7.787 * Y) + (16.0 / 116.0))
            End If
            If (Z > Less) Then
                Z = Math.Pow(Z, Pow)
            Else
                Z = ((7.787 * Z) + (16.0 / 116.0))
            End If

            L = ((116.0 * Y) - 16.0)
            A = (500.0 * (X - Y))
            B = (200.0 * (Y - Z))

            Return New LAB(CInt(L), CInt(A), CInt(B))
        End Function

        Public Shared Function HSBToRGB(HSB As HSB) As RGB
            Select Case 0.0
                Case HSB.S
                    Return New RGB((HSB.B * 255), (HSB.B * 255), (HSB.B * 255))
                Case HSB.B
                    Return New RGB(0, 0, 0)
                Case Else
                    Dim R, G, B As New Double
                    Dim H2 As Double = (HSB.H * 6.0)
                    Dim H2Floor As Integer = CInt(Math.Floor(H2))
                    Dim [Mod] As Double = (H2 - H2Floor)
                    Dim v1 As Double = (HSB.B * (1.0 - HSB.S))
                    Dim v2 As Double = (HSB.B * (1.0 - (HSB.S * [Mod])))
                    Dim v3 As Double = (HSB.B * (1.0 - (HSB.S * (1.0 - [Mod]))))

                    Select Case (H2Floor + 1)
                        Case 0
                            R = HSB.B
                            G = v1
                            B = v2
                        Case 1
                            R = HSB.B
                            G = v3
                            B = v1
                        Case 2
                            R = v2
                            G = HSB.B
                            B = v1
                        Case 3
                            R = v1
                            G = HSB.B
                            B = v3
                        Case 4
                            R = v1
                            G = v2
                            B = HSB.B
                        Case 5
                            R = v3
                            G = v1
                            B = HSB.B
                        Case 6
                            R = HSB.B
                            G = v1
                            B = v2
                        Case 7
                            R = HSB.B
                            G = v3
                            B = v1
                    End Select

                    Return New RGB((R * 255), (G * 255), (B * 255))
            End Select
        End Function

        Public Function RGBToHSB(RGB As RGB) As HSB
            Dim R, G, B As New Double
            Dim H, S, V As New Double
            Dim Max, Min, Delta As New Double

            R = (RGB.R / 255)
            G = (RGB.G / 255)
            B = (RGB.B / 255)

            Max = Math.Max(R, Math.Max(G, B))
            Min = Math.Min(R, Math.Min(G, B))
            Delta = (Max - Min)
            V = Max

            If (Delta = 0) Then
                H = 0
                S = 0
            Else
                S = (Delta / Max)

                Dim Delta_R, Delta_G, Delta_B As New Double
                Delta_R = ((((Max - R) / 6) + (Max / 2)) / Delta)
                Delta_G = ((((Max - G) / 6) + (Max / 2)) / Delta)
                Delta_B = ((((Max - B) / 6) + (Max / 2)) / Delta)

                Select Case Max
                    Case R
                        H = (Delta_B - Delta_G)
                    Case G
                        H = ((1 / 3) + (Delta_R - Delta_B))
                    Case B
                        H = ((2 / 3) + (Delta_G - Delta_R))
                End Select

                Select Case H
                    Case < 0
                        H += 1
                    Case > 1
                        H -= 1
                End Select
            End If

            Return New HSB(H, S, V)
        End Function

        Public Shared Function RGBToCMYK(RGB As RGB) As CMYK
            Dim R As Double = (RGB.R / 255)
            Dim G As Double = (RGB.G / 255)
            Dim B As Double = (RGB.B / 255)

            Dim K As Double = (1 - Math.Max(Math.Max(R, G), B))
            Dim C As Double = (1 - R - K) / (1 - K)
            Dim M As Double = (1 - G - K) / (1 - K)
            Dim Y As Double = (1 - B - K) / (1 - K)

            Return New CMYK(C, M, Y, K)
        End Function

        Public Function CMYKToHSB(CMYK As CMYK) As HSB
            Return RGBToHSB(CMYKToRGB(CMYK))
        End Function

        Public Function CMYKToRGB(CMYK As CMYK) As RGB
            Dim R As Double = ((1 - CMYK.C) * (1 - CMYK.K))
            Dim G As Double = ((1 - CMYK.M) * (1 - CMYK.K))
            Dim B As Double = ((1 - CMYK.Y) * (1 - CMYK.K))

            Return New RGB(R, G, B)
        End Function

        Public Function CMYKToColor(CMYK As CMYK) As Color
            Return RGBToColor(CMYKToRGB(CMYK))
        End Function

        Public Function GetNearestWebSafeColor(Color As Color) As Color
            Return New Color(CInt(Math.Round((Convert.ToInt32(Color.R) / 255) * 5) * 51), CInt(Math.Round((Convert.ToInt32(Color.G) / 255) * 5) * 51), CInt(Math.Round((Convert.ToInt32(Color.B) / 255) * 5) * 51))
        End Function

        Public Function GetNearestWebSafeColor(RGB As RGB) As RGB
            Return New RGB(CInt(Math.Round((RGB.R / 255) * 5) * 51), CInt(Math.Round((RGB.G / 255) * 5) * 51), CInt((Math.Round(RGB.B / 255) * 5) * 51))
        End Function

        Public Function XNAColorToSystemColor(Color As Color) As System.Drawing.Color
            Return System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B)
        End Function

        Public Function RGBToColor(RGB As RGB) As Color
            Return New Color(CInt(RGB.R), CInt(RGB.G), CInt(RGB.B), CInt(RGB.A))
        End Function

        Function ColorToRGB(Color As Color) As RGB
            Return New RGB(Convert.ToInt32(Color.R), Convert.ToInt32(Color.G), Convert.ToInt32(Color.B), Convert.ToInt32(Color.A))
        End Function

        Function ColorToRGB(R As Double, G As Double, B As Double) As RGB
            Return New RGB(R, G, B)
        End Function

        Function ColorToRGB(R As Double, G As Double, B As Double, A As Double) As RGB
            Return New RGB(R, G, B, A)
        End Function

#End Region

        ''' <summary>Call this function when the first update occures.</summary>
        Protected Overrides Sub DoOnFirstUpdate()
            If (_parent IsNot Nothing) Then
                _parent.MarkAsDirty()
                _destRect = New Rectangle(CInt(Parent.InternalDestRect.X + _offset.X), CInt(Parent.InternalDestRect.Y + _offset.Y), CInt(_size.X), CInt(_size.Y))
                SliderColor.Value = CInt(SliderColor.Max)
                SliderColor_Value = SliderColor.Value
            End If
        End Sub

    End Class

    ''' <summary>Specifies identifiers To indicate the Return value Of a dialog box.</summary>
    <Runtime.InteropServices.ComVisible(True)>
    Public Enum DialogResult
        None = 0
        OK = 1
        Cancel = 2
        Abort = 3
        Retry = 4
        Ignore = 5
        Yes = 6
        No = 7
    End Enum

    Public Enum ColorComponent
        Hue
        Saturation
        Brightness
        Red
        Green
        Blue
        Cyan
        Magenta
        Yellow
        Key
        Lightness
        Green_Red
        Blue_Yellow
    End Enum

#Region "Color Classes"

    ''' <summary>
    ''' <para>The Default XNA Color class has a problematic behavior with fractions being considered % of the max 255 color value.</para>
    ''' <para>Which means that any decimal number is considered a fraction even when it is larger than 1.</para>
    ''' <para>This class solves those problems and maintains precision that might otherwise be lost in conversion.</para>
    ''' </summary>
    Public Class RGB
        Public ReadOnly Min As Double = 0.0
        Public ReadOnly Max As Double = 255.0

        Public Sub New()
        End Sub

        Public Sub New(R As Double, G As Double, B As Double)
            Me.R = R
            Me.G = G
            Me.B = B
        End Sub

        Public Sub New(R As Double, G As Double, B As Double, A As Double)
            Me.R = R
            Me.G = G
            Me.B = B
            Me.A = A
        End Sub

        Public Sub New(Color As Color)
            Me.R = Convert.ToInt32(Color.R)
            Me.G = Convert.ToInt32(Color.G)
            Me.B = Convert.ToInt32(Color.B)
            Me.A = Convert.ToInt32(Color.A)
        End Sub

        Private _R As New Double
        Private _G As New Double
        Private _B As New Double
        Private _A As Double = Max

        Public Property R As Double
            Get
                Return _R
            End Get
            Set
                _R = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property G As Double
            Get
                Return _G
            End Get
            Set
                _G = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property B As Double
            Get
                Return _B
            End Get
            Set
                _B = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property A As Double
            Get
                Return _A
            End Get
            Set
                _A = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Overrides Function ToString() As String
            Return (_R.ToString & ":"c & _G.ToString & ":"c & _B.ToString & ":"c & _A.ToString)
        End Function

        Public Shared Operator =(Left As RGB, Right As RGB) As Boolean
            If ((Left.R = Right.R) AndAlso (Left.G = Right.G) AndAlso (Left.B = Right.B) AndAlso (Left.A = Right.A)) Then
                Return True
            Else
                Return False
            End If
        End Operator

        Public Shared Operator <>(Left As RGB, Right As RGB) As Boolean
            Return (Not (Left = Right))
        End Operator

    End Class

    Public Class XYZ
        Public ReadOnly Min As Double = 0

        Public Sub New()
        End Sub

        Public Sub New(X As Double, Y As Double, Z As Double)
            Me.X = X
            Me.Y = Y
            Me.Z = Z
        End Sub

        Private _X As New Double
        Private _Y As New Double
        Private _Z As New Double

        Public Property X As Double
            Get
                Return _X
            End Get
            Set
                _X = LimitInRange(Value, Min, 95.05)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return _Y
            End Get
            Set
                _Y = LimitInRange(Value, Min, 100)
            End Set
        End Property

        Public Property Z As Double
            Get
                Return _Z
            End Get
            Set
                _Z = LimitInRange(Value, Min, 108.9)
            End Set
        End Property

        Overrides Function ToString() As String
            Return (_X.ToString & ":"c & _Y.ToString & ":"c & _Z.ToString)
        End Function

    End Class

    Public Class LAB
        Public ReadOnly Min As Double = -128
        Public ReadOnly Max As Double = 127

        Sub New()
        End Sub

        Sub New(L As Double, A As Double, B As Double)
            Me.L = L
            Me.A = A
            Me.B = B
        End Sub

        Private _L As New Double
        Private _A As New Double
        Private _B As New Double

        Property L As Double
            Get
                Return _L
            End Get
            Set
                _L = LimitInRange(Value, 0, 100)
            End Set
        End Property

        Property A As Double
            Get
                Return _A
            End Get
            Set
                _A = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Property B As Double
            Get
                Return _B
            End Get
            Set
                _B = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Overrides Function ToString() As String
            Return (_L.ToString & ":"c & _A.ToString & ":"c & _B.ToString)
        End Function

    End Class

    Public Class HSB
        Public ReadOnly Min As Double = 0
        Public ReadOnly Max As Double = 1

        Public Sub New()
        End Sub

        Public Sub New(H As Double, S As Double, B As Double)
            Me.H = H
            Me.S = S
            Me.B = B
        End Sub

        Private _H As New Double
        Private _S As New Double
        Private _B As New Double

        Public Property H As Double
            Get
                Return _H
            End Get
            Set
                _H = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property S As Double
            Get
                Return _S
            End Get
            Set
                _S = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property B As Double
            Get
                Return _B
            End Get
            Set
                _B = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Overrides Function ToString() As String
            Return (_H.ToString & ":"c & _S.ToString & ":"c & _B.ToString)
        End Function

        Public Shared Operator =(Left As HSB, Right As HSB) As Boolean
            If ((Left.H = Right.H) AndAlso (Left.S = Right.S) AndAlso (Left.B = Right.B)) Then
                Return True
            Else
                Return False
            End If
        End Operator

        Public Shared Operator <>(Left As HSB, Right As HSB) As Boolean
            Return (Not (Left = Right))
        End Operator

    End Class

    Public Class CMYK
        Public ReadOnly Min As Double = 0
        Public ReadOnly Max As Double = 1

        Public Sub New()
        End Sub

        Public Sub New(C As Double, M As Double, Y As Double, K As Double)
            Me.C = C
            Me.M = M
            Me.Y = Y
            Me.K = K
        End Sub

        Private _C As New Double
        Private _M As New Double
        Private _Y As New Double
        Private _K As New Double

        Public Property C As Double
            Get
                Return _C
            End Get
            Set
                _C = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property M As Double
            Get
                Return _M
            End Get
            Set
                _M = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return _Y
            End Get
            Set
                _Y = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Public Property K As Double
            Get
                Return _K
            End Get
            Set
                _K = LimitInRange(Value, Min, Max)
            End Set
        End Property

        Overrides Function ToString() As String
            Return (_C.ToString & ":"c & _M.ToString & ":"c & _Y.ToString & ":"c & _K.ToString)
        End Function

    End Class

#End Region

    Module MathExtensions

        <Extension()>
        Function LimitInRange(Value As Double, Min As Double, Max As Double) As Double
            Select Case Value
                Case <= Min
                    Return Min
                Case >= Max
                    Return Max
                Case Else
                    If IsNanOrInfinity(Value) Then
                        Return 0.0
                    Else
                        Return Value
                    End If
            End Select
        End Function

        <Extension()>
        Public Function IsNanOrInfinity(Value As Double) As Boolean
            Return Double.IsNaN(Value) OrElse Double.IsInfinity(Value)
        End Function

    End Module

End Namespace