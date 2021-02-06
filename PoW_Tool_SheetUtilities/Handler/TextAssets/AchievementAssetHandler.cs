using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class AchievementAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:H";

        private class AchievementEntry
        {
            public string ID;
            public string ShortText;
            public string ShortOriginalText;
            public string LongText;
            public string LongOriginalText;
            public string D;

            public string StandardizedTerm_Short = "";
            public string StandardizedTerm_Long = "";
            public int row = -1;

            public bool ShortTextChanged = false;
            public bool LongTextChanged = false;
            public bool MLTranslated = false;

            public AchievementEntry()
            { }

            public AchievementEntry(IList<object> row)
            {
                ID = (string)row[0];
                ShortText = (string)row[1];
                ShortOriginalText = (string)row[2];
                LongText = (string)row[3];
                LongOriginalText = (string)row[4];
                D = (string)row[5];
                if (row.Count > 6)
                {
                    StandardizedTerm_Short = (string)row[6];
                }
                if (row.Count > 7)
                {
                    StandardizedTerm_Short = (string)row[7];
                }
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Achievement"];
                Request updateRequest = new Request();

                var ShortTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = ShortText },
                    UserEnteredFormat = null
                };
                var LongTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = LongText },
                    UserEnteredFormat = null
                };

                var Fields = "userEnteredValue";
                if (ShortTextChanged || LongTextChanged)
                {
                    Fields = "userEnteredValue,userEnteredFormat";
                    if (MLTranslated)
                    {
                        ShortTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };

                        LongTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };
                    }
                    else
                    {
                        ShortTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };

                        LongTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };
                    }
                }

                var Rows = new List<RowData>
                {
                    new RowData()
                    {
                        Values = new List<CellData>()
                        {
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = ID}
                            },
                            ShortTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = ShortOriginalText}
                            },
                            LongTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = LongOriginalText}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = D}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTerm_Short}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTerm_Long}
                            },
                        }
                    }
                };

                if (row >= 1)
                {
                    updateRequest.UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange()
                        {
                            SheetId = 0,
                            StartRowIndex = row,
                            EndRowIndex = row + 1,
                        },
                        Rows = Rows,
                        Fields = Fields
                    };
                }
                else
                {
                    updateRequest.AppendCells = new AppendCellsRequest
                    {
                        Rows = Rows,
                        SheetId = 0,
                        Fields = Fields
                    };
                }

                return updateRequest;
            }
        }

        public void BuildGameDataFromSheet(string outRootPath)
        {
            string outPath = outRootPath + Path.DirectorySeparatorChar + "Achievement.txt";
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Achievement"];

            Console.WriteLine("Getting Achievement Spreadsheet content");

            //Getting all current entries
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            //Clearing talk File
            string rootPath = Path.GetDirectoryName(outPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            File.WriteAllText(outPath, "");

            Console.WriteLine("Constructing Achievement.txt...");
            using (StreamWriter sw = File.AppendText(outPath))
            {
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        sw.Write((string)row[0] + '\t'); //ID
                        sw.Write((string)row[1] + '\t'); //ShortText
                        sw.Write((string)row[3] + '\t'); //LongText
                        sw.Write((string)row[5] + '\t'); //D
                        sw.Write('\r');
                    }
                }
            }
            Console.WriteLine("Done!");
        }

        public void UpdateSheetFromGameFile(string gameFileRootPath)
        {
            string gameFilePath = gameFileRootPath + Path.DirectorySeparatorChar + "Achievement.bytes";

            //Getting all current entries
            Dictionary<string, AchievementEntry> achievementEntries = new Dictionary<string, AchievementEntry>();

            Console.WriteLine("Getting Achievement Spreadsheet content");
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Achievement"];
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AchievementEntry ae = new AchievementEntry(row);
                    ae.row = rowC;
                    achievementEntries[ae.ID] = ae;
                    rowC++;
                }
            }

            //UpdatingRequests
            var updateRequests = new List<Request>();

            Console.WriteLine("Comparing with games version and calculating updates...");
            //Parse every line in the game file
            System.IO.StreamReader reader = new System.IO.StreamReader(gameFilePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //Splitting
                string[] data = line.Split('\t');

                //Parsing data
                string ID = data[0];
                string ShortOriginalText = data[1];
                string LongOriginalText = data[2];
                string D = data[3];

                string StandardizedTerm_Short = StandardizedTermManager.GetInstance().GetTermLocatorText(ShortOriginalText);
                string StandardizedTerm_Long = StandardizedTermManager.GetInstance().GetTermLocatorText(LongOriginalText);

                if (achievementEntries.ContainsKey(ID))
                {
                    //Compare and update
                    AchievementEntry existingEntry = achievementEntries[ID];
                    bool needsUpdate = false;

                    if (!string.IsNullOrEmpty(ShortOriginalText) && existingEntry.ShortOriginalText != ShortOriginalText)
                    {
                        existingEntry.ShortOriginalText = ShortOriginalText;
                        existingEntry.ShortTextChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(LongOriginalText) && existingEntry.LongOriginalText != LongOriginalText)
                    {
                        existingEntry.LongOriginalText = LongOriginalText;
                        existingEntry.LongTextChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(StandardizedTerm_Short) && existingEntry.StandardizedTerm_Short != StandardizedTerm_Short)
                    {
                        existingEntry.StandardizedTerm_Short = StandardizedTerm_Short;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(StandardizedTerm_Long) && existingEntry.StandardizedTerm_Long != StandardizedTerm_Long)
                    {
                        existingEntry.StandardizedTerm_Long = StandardizedTerm_Long;
                        needsUpdate = true;
                    }

                    if (existingEntry.D != D)
                    {
                        existingEntry.D = D;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        updateRequests.Add(existingEntry.ToGoogleSheetUpdateRequest());
                    }
                }
                else
                {
                    //New one
                    AchievementEntry newEntry = new AchievementEntry();
                    newEntry.ID = ID;
                    newEntry.ShortOriginalText = ShortOriginalText;
                    newEntry.ShortText = TranslationManager.GetInstance().Translate(StandardizedTerm_Short);
                    newEntry.ShortTextChanged = true;
                    newEntry.LongOriginalText = LongOriginalText;
                    newEntry.LongText = TranslationManager.GetInstance().Translate(StandardizedTerm_Long);
                    newEntry.D = D;
                    newEntry.LongTextChanged = true;
                    newEntry.MLTranslated = true;
                    newEntry.StandardizedTerm_Short = StandardizedTerm_Short;
                    newEntry.StandardizedTerm_Long = StandardizedTerm_Long;

                    updateRequests.Add(newEntry.ToGoogleSheetUpdateRequest());
                }
            }

            Console.WriteLine("Updating spreadsheet...");
            BatchUpdateSpreadsheetRequest batchUpdate = new BatchUpdateSpreadsheetRequest();
            batchUpdate.Requests = new List<Request>();
            int reqHandled = 0;
            //updateRequests.RemoveRange(2, updateRequests.Count - 2);
            foreach (var req in updateRequests)
            {
                batchUpdate.Requests.Add(req);
                reqHandled++;
                if (batchUpdate.Requests.Count >= 500 || reqHandled >= updateRequests.Count)
                {
                    var updateRequest = gsc.Service.Spreadsheets.BatchUpdate(batchUpdate, talkSpreadsheetId);
                    updateRequest.Execute();
                    //Resetting batch update
                    batchUpdate = new BatchUpdateSpreadsheetRequest();
                    batchUpdate.Requests = new List<Request>();
                }
            }

            Console.WriteLine("Done!");
        }
    }
}