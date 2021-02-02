namespace PoW_Tool_SheetUtilities.Handler
{
    interface IFileHandler
    {
        void UpdateSheetFromGameFile(string gameFileRootPath);
        void BuildGameDataFromSheet(string outRootPath);
    }
}
