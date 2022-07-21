using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using PoW_Tool_SheetUtilities.Handler;
using PoW_Tool_SheetUtilities.Handler.BattleAssets;
using PoW_Tool_SheetUtilities.Handler.BufferAssets;
using PoW_Tool_SheetUtilities.Handler.TextAssets;

namespace PoW_Tool_SheetUtilities
{
    internal class SpreadsheetUpdater
    {
        private static readonly List<IFileHandler> _Handlers = new List<IFileHandler>()
        {
        new SkillAssetHandler(),
        new BattleAssetHandler(),
        new MantraAssetHandler(),
        new BufferAssetHandler(),
        new AdjustmentAssetHandler(),
        new CinameticAssetHandler(),
        new NoteDescriptionAssetHandler(),
        new GameFormulaAssetHandler(),
        new HelpAssetHandler(),
        new HelpDescriptionAssetHandler(),
        new TalentAssetHandler(),
        new ShopAssetHandler(),
        new RewardAssetHandler(),
        new RefiningAssetHandler(),
        new MapAssetHandler(),
        new ForgeAssetHandler(),
        new FavorabilityAssetHandler(),
        new EvaluationAssetHandler(),
        new ElectiveAssetHandler(),
        new CharacterExteriorAssetHandler(),
        new CharacterBehaviourAssetHandler(),
        new BookAssetHandler(),
        new TraitAssetHandler(),
        new StringTableAssetHandler(),
        new QuestAssetHandler(),
        new PropsAssetHandler(),
        new NurturanceAssetHandler(),
        new EventCubeAssetHandler(),
        new NpcAssetHandler(),
        new CharacterInfoAssetHandler(),
        new BattleAreaAssetHandler(),
        new AchievementAssetHandler(),
        new AlchemyAssetHandler(),
        new RoundAssetHandler(),
        new RegistrationBonusAssetHandler(),
        new NurturanceIdleAssetHandler(),
        new TalkAssetHandler(),
        };

        public static void UpdateSpreadsheetsFromGameFiles()
        {
            //Get input folder path
            string workingDirectory = Environment.CurrentDirectory;
            string inputFolder = workingDirectory + Path.DirectorySeparatorChar + "Input";
            foreach (IFileHandler handler in _Handlers)
            {
                handler.UpdateSheetFromGameFile(inputFolder);
                Thread.Sleep(5000);
            }
        }

        public static void ExportToMod(string BuildGameDataFromSheet)
        {
            foreach (IFileHandler handler in _Handlers)
            {
                handler.BuildGameDataFromSheet(BuildGameDataFromSheet);
                Thread.Sleep(5000);
            }
        }

        internal static void GetTranslationStats(ref List<TranslationStatEntry> stats)
        {
            foreach (IFileHandler handler in _Handlers)
            {
                handler.GetTranslationStats(ref stats);
                Thread.Sleep(10000);
            }
        }
    }
}