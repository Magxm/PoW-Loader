using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class GameFormulaAssetHandler : AssetHandler
    {
        public GameFormulaAssetHandler()
        {
            SheetId = "1OfXl478TjtD8KOBBnyWx79BAa2dn-ySlAavDquqTPRA";
            AssetName = "GameFormula";
            SheetRange = "A2:F";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "GameFormula";
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
                    Name = "Alias",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "Formular",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}