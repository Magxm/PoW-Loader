using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class EndingTranslationsAssetHandler : AssetHandler
    {
        public EndingTranslationsAssetHandler()
        {
            SheetId = "1tk7OHn9G6nevJ4D7xKBe-5eiYJx1X6CkD3A8ia-fsj8";
            AssetName = "EndingTranslations";
            SheetRange = "A2:C";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "EndingTranslations";
            OutputExtension = ".csv";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "TextId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Translated",
                    VariableType = AssetVariableType.Translate
                }
            };
        }
    }
}