using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class AlchemyAssetHandler : AssetHandler
    {
        public AlchemyAssetHandler()
        {
            SheetId = "1mEpb_0IGXA-DlU30uz6kLZye4fzCqA7Dga9Qqg1YEgk";
            AssetName = "Alchemy";
            SheetRange = "A2:E";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Alchemy";
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
                    Name = "Name",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "ID2",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}