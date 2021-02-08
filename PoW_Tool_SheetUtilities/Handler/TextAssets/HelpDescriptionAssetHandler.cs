using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class HelpDescriptionAssetHandler : AssetHandler
    {
        public HelpDescriptionAssetHandler()
        {
            SheetId = "1EzSX3WEzF8F4ABCl0OEaQb65ndfG2ZvViNTthuftuPE";
            AssetName = "HelpDescription";
            SheetRange = "A2:F";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "HelpDescription";
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
                    Name = "ShowCondition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Order",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}