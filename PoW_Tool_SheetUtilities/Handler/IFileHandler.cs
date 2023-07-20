using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace PoW_Tool_SheetUtilities.Handler
{
    interface IFileHandler
    {
        void UpdateSheetFromGameFile(string gameFileRootPath);
        void BuildGameDataFromSheet(string outRootPath);
        void GetTranslationStats(ref List<TranslationStatEntry> stats);
        void ExportTranslatedLinesToCSV(string outPath, ref List<Color> acceptableColors);
    }
}
