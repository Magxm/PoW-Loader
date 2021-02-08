using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class TalentAssetHandler : AssetHandler
    {
        public TalentAssetHandler()
        {
            SheetId = "1SBkLo3w7gZZ29w2Um3zLOzchMhvoTS41q8LmbOqUyqk";
            AssetName = "Talent";
            SheetRange = "A2:K";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Talent";
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
                    Name = "DescriptionA",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "DescriptionB",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "K",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}