using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.Handler
{
    internal class EndingScenesHandler : AssetHandler
    {
        public EndingScenesHandler()
        {
            SheetId = "";
            AssetName = "EndingScenes";
            SheetRange = "";
            FilePathWithoutExtension = "";
            OutputExtension = "";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "Translate",
                    VariableType = AssetVariableType.Translate
                },
            };
        }

        private List<string> GetEntriesFromGameFiles()
        {
            string inputFolder = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "EndingScenes";
        }

        public override void UpdateSheetFromGameFile(string outRootPath)
        {
        }

        public override void BuildGameDataFromSheet(string outRootPath)
        {
        }
    }
}
