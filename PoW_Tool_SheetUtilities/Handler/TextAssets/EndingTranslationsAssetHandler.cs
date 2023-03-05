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
            SheetId = "1FBrzsHGaGfHO7jVHpD1roQRS7Rqbl2Ia";
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