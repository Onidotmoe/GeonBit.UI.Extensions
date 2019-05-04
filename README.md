# GeonBit.UI.Extensions
Control and Functional Extensions for [GeonBit.UI](https://github.com/RonenNess/GeonBit.UI). Written in VB.Net by an Amatuer.

## ColorPicker
A comprehensive adobe photoshop-like color picker.

![alt text][ColorPicker1]

[ColorPicker1]: https://github.com/VampireMonkey/GeonBit.UI.Extensions/blob/master/Demonstration%20-%20Colorpicker.png?raw=true

## External Extender
What I used to add new controls to the existing controls in GeonBit.UI

## Select2D
A 2D selector.
*demonstrated in the colorpicker.*

## Minor
Some minor changes that either have been done or need further testing/work.

## SliderVertical
Just a vertical version of the existing Slider control.
*demonstrated in the colorpicker.*

## TitleBar
Allows movement of the entire window similar to the default windows titlebar.
You can safely set "Window.IsBorderless = True" and get a custom look to your game.

Update Note : Seems to have problems keeping up at high load.

## SelectFont (WIP-AKA NOT FINISHED)
Allows for systemfonts to be used to generate working spritefonts. Currently only working in Windows.
Note that it's not complete and likely will have more buttons and functions.

The idea is that you should create an unicode range which your selected font then will generate a bitmap with all the locations, croppings, and so on... for your new spritefont.

![alt text][SelectFont & SelectEntity Preview]

[SelectFont & SelectEntity Preview]: https://github.com/VampireMonkey/GeonBit.UI.Extensions/blob/master/SelectFont%20&%20SelectEntity%20-%20Preview.png?raw=true

As you can see, kerning and spacing between characters is incorrect and the end result is that each character is the same distance between all other characters, this is a major problem.

[I've opened an issue in the monogame dev section][1] to maybe get some answers on the way kerning is implemented. As i understand it a bit better after posting that issue, their approach seems valid, but that does make it a bit harder for me since now i have to get the kerning of all characters and i'm not sure i can do that with windows fonts.
If it ends up with that, i might have to create a separate tool to specify kernings manually, should not be too difficult but i'd rather avoid it.

[1]: https://github.com/MonoGame/MonoGame/issues/6371

## SelectEntity
Allows generic Entities to be used for selection.
*demonstrated in the SelectFont.*

## Border
A generic colored border to be placed ontop of entities, which also do not exceed the entity's own boundaries. 
*demonstrated in the SelectFont.*

## Screen
Represents a Screen Entity, inherits from Panel. Makes it easier to manage different UI elements groupped together, like Menus, HUD.
Removing a Screen is Easy using Screen.Pop() and adding a Screen is just as easy Screen.Push, which places the Screen in the top of the ScreenStack.
I've found it easiest to inherit from this class and then add your custom Screen to the Screenstack.
Overriding Init() allows you to do any custom behavior on initialization and still have your default settings applied in PreInit() in the base Screen class.

## ScreenStack
Allows for easy managements of all Screens, using a Screenstack similar to the one in Xcom 2. Allows for Cascading MouseInput until one Screen Blocks it.
The Screens are ordered in a simple list, the Top Screen on the stack is the current visible screen. Default behavior shows all below screens in the stack, you can change this if you want, i have found having multiple screens ontop of each other with EatsMouse = false allows for more flexibility.
You don't have to only use 1 instance of a ScreenStack, you can have as many as you like.

### Note
Monogame.Extended might be require for some bits of the codebase.
Currently only targeting Windows but i would like to have complete platform-indepence at some point.
Some Entities are not yet completed, like SelectFont; they will be completed eventually.

## License
Public Domain + Absorption v1.0

Public Domain :
GeonBit.UI.Extensions is in the Public Domain.

Absorption v1.0 :
Any and all changes to this repository is absorbed into the main license; which is the public domain.
Any and all contributions to this repository is absorbed into the main license; which is the public domain.
You are free to copy and do whatever you want with your copy of this repository.
