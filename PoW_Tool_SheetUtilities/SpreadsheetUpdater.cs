using System;
using System.IO;

using PoW_Tool_SheetUtilities.Handler.BattleAssets;
using PoW_Tool_SheetUtilities.Handler.TextAssets;

namespace PoW_Tool_SheetUtilities
{
    internal class SpreadsheetUpdater
    {
        public static void UpdateSpreadsheetsFromGameFiles()
        {
            //Get input folder path
            string workingDirectory = Environment.CurrentDirectory;
            string inputFolder = workingDirectory + Path.DirectorySeparatorChar + "Input";

            new EventCubeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new NpcAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new CharacterInfoAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BattleAreaAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new MantraAssetHandler().UpdateSheetFromGameFile(inputFolder);

            //Updating TextAssets
            string textAssetFolder = inputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "textfiles";
            new TalkAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            new AchievementAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            new AdjustmentAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            new AlchemyAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            //new AnimationMappingAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            new SkillAssetHandler().UpdateSheetFromGameFile(textAssetFolder);
            //Updating battle related Assets
            string battleAssetFolder = inputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle";
            new BufferAssetHandler().UpdateSheetFromGameFile(battleAssetFolder);
        }

        public static void ExportToMod(string outputFolder)
        {
            //Get input folder path
            string textAssetFolder = outputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "textfiles";

            //Exporting TextAssets
            new TalkAssetHandler().BuildGameDataFromSheet(textAssetFolder);
            new AchievementAssetHandler().BuildGameDataFromSheet(textAssetFolder);
            new AdjustmentAssetHandler().BuildGameDataFromSheet(textAssetFolder);
            new AlchemyAssetHandler().BuildGameDataFromSheet(textAssetFolder);
            //new AnimationMappingAssetHandler().BuildGameDataFromSheet(textAssetFolder);
            new SkillAssetHandler().BuildGameDataFromSheet(textAssetFolder);

            //Exporting battle related Assets
            string battleAssetFolder = outputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle";
            new BufferAssetHandler().BuildGameDataFromSheet(battleAssetFolder);
        }
    }
}