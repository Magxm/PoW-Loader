# Path Of Wuxia ModLoader + English Patch

Join our discord! 
https://discord.com/invite/PH4Z4Dn

-------------------------------------------
BepInEx is the actual underlying ModLoader.
This repository contains a ModAPI plugin containing base functionalities for Mods (WIP).
It currently allows dynamic registration of external Asset folders, which allows several mods to register asset overrides.

Additionally this repository contains a EnglishPatch Plugin which registers Mods/EnglishTranslate as a resource folder (So place the translated Asset files into Mods/EnglishTranslate) and fixes several issues with displaying the English Text.

The Output folder contains an almost full release after building, containing more or less up to date English files and an already configured BepInEx.
If BepInEx is updated/replaced by a new verion it is important to update the BepInEx.cfg and replace the following Entry.
```ini
[Preloader.Entrypoint]

## The local filename of the assembly to target.
# Setting type: String
# Default value: UnityEngine.CoreModule.dll
Assembly = UnityEngine.CoreModule.dll

## The name of the type in the entrypoint assembly to search for the entrypoint method.
# Setting type: String
# Default value: Application
Type = Object

## The name of the method in the specified entrypoint assembly and type to hook and load Chainloader from.
# Setting type: String
# Default value: .cctor
Method = .cctor
```

otherwise the Entrypoint will be too late and the language files are already loaded.

