using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class AchievementAssetHandler : AssetHandler
    {
        public AchievementAssetHandler()
        {
            SheetId = "1M4b0Io5_KMvyoobHybAqrGJdU_qfrmh74U4OPT_k7dM";
            AssetName = "Achievement";
            SheetRange = "A2:H";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Achievement";
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
                    Name = "ShortVersionText",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "LongVersionText",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "H",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}