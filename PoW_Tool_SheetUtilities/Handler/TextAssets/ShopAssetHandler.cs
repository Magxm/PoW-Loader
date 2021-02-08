using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class ShopAssetHandler : AssetHandler
    {
        public ShopAssetHandler()
        {
            SheetId = "1yMrxb-OUnwbP-HKee4BIG8ChqW8tmDKLBhWwpz2XlUU";
            AssetName = "Shop";
            SheetRange = "A2:G";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Shop";
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
                    Name = "PropsId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "ShopPeriods",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Condition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "IsRepeat",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}