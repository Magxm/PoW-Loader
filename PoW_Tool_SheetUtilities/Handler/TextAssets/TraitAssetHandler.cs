using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class TraitAssetHandler : AssetHandler
    {
        public TraitAssetHandler()
        {
            SheetId = "10PlTu_pE2ZUqYXm9VZRFREuCwqyRiXZQ6m-GRWZDsPI";
            AssetName = "Trait";
            SheetRange = "A2:O";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Trait";
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
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "IsInitial",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Point",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "CheckChainTraitList",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "AddChainTraitList",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "TraitEffects",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "InitialRewards",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}