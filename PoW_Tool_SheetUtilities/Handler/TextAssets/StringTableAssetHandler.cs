using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class StringTableAssetHandler : AssetHandler
    {
        public StringTableAssetHandler()
        {
            SheetId = "1WB82p-nhbjZBj4HwAPT5lREF6iRv6GnWC9LHP80f7M4";
            AssetName = "StringTable";
            SheetRange = "A2:F";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "StringTable";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "ID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "Text",
                    VariableType = AssetVariableType.Translate
                },
            };
        }
    }
}