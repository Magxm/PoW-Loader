using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class HelpAssetHandler : AssetHandler
    {
        public HelpAssetHandler()
        {
            SheetId = "1-sEJ0P04YRUNOMmXc_qq8Aj8leu6Xhj8-BzAcWfTL1U";
            AssetName = "Help";
            SheetRange = "A2:K";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Help";
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
                    Name = "MainTab",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "IsMainEntry",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "MainEntryId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Name",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "ShowCondition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "ImageName",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "ExteriorId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "HelpDescriptions",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}