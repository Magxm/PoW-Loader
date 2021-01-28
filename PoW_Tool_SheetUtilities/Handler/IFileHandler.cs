namespace PoW_Tool_SheetUtilities.Handler
{
    interface IFileHandler
    {
        void UpdateSheetFromGameFile(string gameFilePath);
        void BuildGameDataFromSheet(string outRootPath);
    }
}
