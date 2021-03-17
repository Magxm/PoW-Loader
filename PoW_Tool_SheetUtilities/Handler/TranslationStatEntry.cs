using Google.Apis.Sheets.v4.Data;

using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler
{
    public class TranslationStatEntry
    {
        public string Name = "Unnamed Type";
        public int LineCount = 0;
        public int WordCount = 0;
        public bool MatchAll = false;

        public List<Color> AcceptableColors = new List<Color>();

        public TranslationStatEntry(string name)
        {
            Name = name;
        }
    }
}
