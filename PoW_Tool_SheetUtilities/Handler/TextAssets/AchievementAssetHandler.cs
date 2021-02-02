using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    class AchievementAssetHandler : IFileHandler
    {
        private class AchievementEntry
        {
            string ID;
            string ShortText;
            string ShortOriginalText;
            string LongText;
            string LongOriginalText;
            string D;

            AchievementEntry(IList<object> row)
            {
                ID = (string)row[0];
                ShortText = (string)row[1];
                ShortOriginalText = (string)row[2];
                LongText = (string)row[3];
                LongOriginalText = (string)row[4];
                D = (string)row[5];
            }
        }

        public void BuildGameDataFromSheet(string outRootPath)
        {
            throw new NotImplementedException();
        }

        public void UpdateSheetFromGameFile(string gameFilePath)
        {
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Achievement"];
        }
    }
}
