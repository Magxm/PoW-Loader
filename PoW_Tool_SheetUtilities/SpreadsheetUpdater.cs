using System;
using System.IO;
using System.Threading;

using PoW_Tool_SheetUtilities.Handler.BattleAssets;
using PoW_Tool_SheetUtilities.Handler.BufferAssets;
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
            Thread.Sleep(5000);
            new TalentAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new ShopAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new RewardAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new RefiningAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new MapAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new ForgeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new FavorabilityAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new EvaluationAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new ElectiveAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new CharacterExteriorAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new CharacterBehaviourAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new BookAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new TraitAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new StringTableAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new QuestAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new PropsAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new NurturanceAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new EventCubeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new NpcAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new CharacterInfoAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new BattleAreaAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new MantraAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new TalkAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new AchievementAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new AdjustmentAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new AlchemyAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new SkillAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(5000);
            new BufferAssetHandler().UpdateSheetFromGameFile(inputFolder);
        }

        public static void ExportToMod(string outputFolder)
        {
            new TalentAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new ShopAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new RewardAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new RefiningAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new MapAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new ForgeAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new FavorabilityAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new EvaluationAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new ElectiveAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new CharacterExteriorAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new CharacterBehaviourAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new BookAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new BattleAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new TraitAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new StringTableAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new QuestAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new PropsAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new NurturanceAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new EventCubeAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new NpcAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new CharacterInfoAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new BattleAreaAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new MantraAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new TalkAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new AchievementAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new AdjustmentAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new AlchemyAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new SkillAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(5000);
            new BufferAssetHandler().BuildGameDataFromSheet(outputFolder);
        }

        internal static void GetTranslationStats(ref int proofReadCount, ref int translatedCount, ref int needsCheckCount, ref int mTLCount, ref int otherCount)
        {
            new BattleAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new TalentAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new ShopAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            // Thread.Sleep(5000);
            new RewardAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new RefiningAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new MapAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new ForgeAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new FavorabilityAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new EvaluationAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new ElectiveAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new CharacterExteriorAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new CharacterBehaviourAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new BookAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new TraitAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new StringTableAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new QuestAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new PropsAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new NurturanceAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new EventCubeAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new NpcAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new CharacterInfoAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new BattleAreaAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new MantraAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            //new TalkAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new AchievementAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new AdjustmentAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new AlchemyAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new SkillAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            //Thread.Sleep(5000);
            new BufferAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
        }
    }
}