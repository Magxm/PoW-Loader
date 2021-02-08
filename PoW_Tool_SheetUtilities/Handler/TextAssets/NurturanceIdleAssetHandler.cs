using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class NurturanceIdleAssetHandler : AssetHandler
    {
        public NurturanceIdleAssetHandler()
        {
            SheetId = "1H6jvEzmv0yUmI6JVNPiuuEwOsboc82d73gJfYoc9BvA";
            AssetName = "NurturanceIdle";
            SheetRange = "A2:F";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "NurturanceIdle";
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
                    Name = "Description",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "AddCondition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "RandomCondition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Weights",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}