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
    class AchievementAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:F";
        private class AchievementEntry
        {
            public string ID;
            public string ShortText;
            public string ShortOriginalText;
            public string LongText;
            public string LongOriginalText;
            public string D;
            public int row = -1;

            public bool ShortTextChanged = false;
            public bool ShortTextMLTranslated = false;
            public bool LongTextChanged = false;
            public bool LongTextMLTranslated = false;

            public AchievementEntry()
            {}
            public AchievementEntry(IList<object> row)
            {
                ID = (string)row[0];
                ShortText = (string)row[1];
                ShortOriginalText = (string)row[2];
                LongText = (string)row[3];
                LongOriginalText = (string)row[4];
                D = (string)row[5];
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
                    if (ShortTextMLTranslated)
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
                    }

                    if (LongTextMLTranslated)
                    {
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
            throw new NotImplementedException();
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
                    newEntry.ShortText = TranslationManager.GetInstance().Translate(ShortOriginalText);
                    newEntry.ShortTextChanged = true;
                    newEntry.ShortTextMLTranslated = true;
                    newEntry.LongOriginalText = LongOriginalText;
                    newEntry.LongText = TranslationManager.GetInstance().Translate(LongOriginalText);
                    newEntry.D = D;
                    newEntry.LongTextChanged = true;
                    newEntry.LongTextMLTranslated = true;

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
