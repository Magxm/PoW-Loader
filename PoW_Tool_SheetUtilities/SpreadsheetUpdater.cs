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

            new BattleAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new TraitAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new StringTableAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new QuestAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new PropsAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new NurturanceAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new EventCubeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new NpcAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new CharacterInfoAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BattleAreaAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new MantraAssetHandler().UpdateSheetFromGameFile(inputFolder);

            //OLD HANDLERS
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

            new BattleAssetHandler().BuildGameDataFromSheet(outputFolder);
            new TraitAssetHandler().BuildGameDataFromSheet(outputFolder);
            new StringTableAssetHandler().BuildGameDataFromSheet(outputFolder);
            new QuestAssetHandler().BuildGameDataFromSheet(outputFolder);
            new PropsAssetHandler().BuildGameDataFromSheet(outputFolder);
            new NurturanceAssetHandler().BuildGameDataFromSheet(outputFolder);
            new EventCubeAssetHandler().BuildGameDataFromSheet(outputFolder);
            new NpcAssetHandler().BuildGameDataFromSheet(outputFolder);
            new CharacterInfoAssetHandler().BuildGameDataFromSheet(outputFolder);
            new BattleAreaAssetHandler().BuildGameDataFromSheet(outputFolder);
            new MantraAssetHandler().BuildGameDataFromSheet(outputFolder);

            //OLD HANDLERS
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