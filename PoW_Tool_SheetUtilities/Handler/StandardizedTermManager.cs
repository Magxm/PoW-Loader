using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.Handler
{
    internal class StandardizedTermManager
    {
        private static StandardizedTermManager __Instance;

        public static StandardizedTermManager GetInstance()
        {
            if (__Instance == null)
            {
                __Instance = new StandardizedTermManager();
            }

            return __Instance;
        }

        private Dictionary<string, string> StandardizedTerms;

        private static List<string> SheetRanges = new List<string>()
        {
            "Glossary!A6:B",
            "Character Names!A5:B",
            "Gears!A4:B",
            //"Quest Items!A4:B",
            "Skills!A6:B",
            "Buff_Debuff!A2:B",
            "Mantras!A6:B",
            "SkillReference!A4:B",
        };

        private StandardizedTermManager()
        {
            Console.WriteLine("Getting standardized terms from google sheets..");
            StandardizedTerms = new Dictionary<string, string>();

            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string termSpreadsheetId = "1GVyGWijCuuSlgkyOzTLwutRiS-mlCf5Lc35Y-mY7TR4";

            foreach (var range in SheetRanges)
            {
                SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(termSpreadsheetId, range);
                ValueRange response = request.Execute();
                List<IList<object>> values = (List<IList<object>>)response.Values;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (row.Count == 2)
                        {
                            string chineseT = (string)row[0];
                            string engT = (string)row[1];
                            if (!StandardizedTerms.ContainsKey(chineseT))
                            {
                                StandardizedTerms.Add(chineseT, engT);
                            }
                        }
                    }
                }
            }
        }

        public string GetTermLocatorText(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return "";
            }

            string result = string.Copy(original);

            foreach (var term in StandardizedTerms)
            {
                result = result.Replace(term.Key, term.Value);
            }

            return result;
        }
    }
}