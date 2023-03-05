using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class EndingIdsAssetHandler : AssetHandler
    {
        public EndingIdsAssetHandler()
        {
            SheetId = "1bRH1P32iWeGKKBS1TSNbk__2IqEVzmJ9fu_k8eD4TKg";
            AssetName = "EndingIds";
            SheetRange = "A2:A";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "EndingIds";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "Id",
                    VariableType = AssetVariableType.NoTranslate
                }
            };
        }
    }
}