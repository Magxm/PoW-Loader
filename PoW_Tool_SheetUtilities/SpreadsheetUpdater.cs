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

            new HelpAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new HelpDescriptionAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BattleAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new TalkAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(30000);
            new TalentAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new ShopAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new RewardAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(10000);
            new RefiningAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new MapAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new ForgeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new FavorabilityAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(10000);
            new EvaluationAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new ElectiveAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new CharacterExteriorAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new CharacterBehaviourAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BookAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(10000);
            new TraitAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new StringTableAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new QuestAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new PropsAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new NurturanceAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(10000);
            new EventCubeAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new NpcAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new CharacterInfoAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BattleAreaAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new MantraAssetHandler().UpdateSheetFromGameFile(inputFolder);
            Thread.Sleep(10000);
            new AchievementAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new AdjustmentAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new AlchemyAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new SkillAssetHandler().UpdateSheetFromGameFile(inputFolder);
            new BufferAssetHandler().UpdateSheetFromGameFile(inputFolder);
        }

        public static void ExportToMod(string outputFolder)
        {
            new TalkAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(60000);
            new TalentAssetHandler().BuildGameDataFromSheet(outputFolder);
            new ShopAssetHandler().BuildGameDataFromSheet(outputFolder);
            new RewardAssetHandler().BuildGameDataFromSheet(outputFolder);
            new RefiningAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new MapAssetHandler().BuildGameDataFromSheet(outputFolder);
            new ForgeAssetHandler().BuildGameDataFromSheet(outputFolder);
            new FavorabilityAssetHandler().BuildGameDataFromSheet(outputFolder);
            new EvaluationAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new ElectiveAssetHandler().BuildGameDataFromSheet(outputFolder);
            new CharacterExteriorAssetHandler().BuildGameDataFromSheet(outputFolder);
            new CharacterBehaviourAssetHandler().BuildGameDataFromSheet(outputFolder);
            new BookAssetHandler().BuildGameDataFromSheet(outputFolder);
            new BattleAssetHandler().BuildGameDataFromSheet(outputFolder);
            new TraitAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new StringTableAssetHandler().BuildGameDataFromSheet(outputFolder);
            new QuestAssetHandler().BuildGameDataFromSheet(outputFolder);
            new PropsAssetHandler().BuildGameDataFromSheet(outputFolder);
            new NurturanceAssetHandler().BuildGameDataFromSheet(outputFolder);
            new EventCubeAssetHandler().BuildGameDataFromSheet(outputFolder);
            new NpcAssetHandler().BuildGameDataFromSheet(outputFolder);
            new CharacterInfoAssetHandler().BuildGameDataFromSheet(outputFolder);
            Thread.Sleep(10000);
            new BattleAreaAssetHandler().BuildGameDataFromSheet(outputFolder);
            new MantraAssetHandler().BuildGameDataFromSheet(outputFolder);
            new AchievementAssetHandler().BuildGameDataFromSheet(outputFolder);
            new AdjustmentAssetHandler().BuildGameDataFromSheet(outputFolder);
            new AlchemyAssetHandler().BuildGameDataFromSheet(outputFolder);
            new SkillAssetHandler().BuildGameDataFromSheet(outputFolder);
            new BufferAssetHandler().BuildGameDataFromSheet(outputFolder);
            new HelpAssetHandler().BuildGameDataFromSheet(outputFolder);
            new HelpDescriptionAssetHandler().BuildGameDataFromSheet(outputFolder);
        }

        internal static void GetTranslationStats(ref int proofReadCount, ref int translatedCount, ref int needsCheckCount, ref int mTLCount, ref int otherCount)
        {
            new TalkAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            Thread.Sleep(30000);
            new BattleAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new TalentAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new ShopAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new RewardAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new RefiningAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new MapAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new ForgeAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new FavorabilityAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new EvaluationAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new ElectiveAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new CharacterExteriorAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new CharacterBehaviourAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new BookAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new TraitAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new StringTableAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new QuestAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new PropsAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            Thread.Sleep(30000);
            new NurturanceAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new EventCubeAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new NpcAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new CharacterInfoAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new BattleAreaAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new MantraAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            Thread.Sleep(30000);
            new AchievementAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new AdjustmentAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new AlchemyAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new SkillAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new BufferAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new HelpAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
            new HelpDescriptionAssetHandler().GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref mTLCount, ref otherCount);
        }
    }
}