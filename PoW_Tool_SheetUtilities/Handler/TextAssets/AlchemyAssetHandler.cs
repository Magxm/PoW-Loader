using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class AlchemyAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:E";

        private class AlchemyEntry
        {
            public string ID;
            public string Name;
            public string OriginalName;
            public string ID2;
            public string StandardizedTermLocator_Name = "";

            public int row;

            public bool NameChanged = false;
            public bool MLTranslated = false;

            public AlchemyEntry()
            {
            }

            public AlchemyEntry(IList<object> row)
            {
                ID = (string)row[0];
                Name = (string)row[1];
                OriginalName = (string)row[2];
                ID2 = (string)row[3];
                if (row.Count > 4)
                {
                    StandardizedTermLocator_Name = (string)row[4];
                }
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Alchemy"];
                Request updateRequest = new Request();

                var NameTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Name },
                    UserEnteredFormat = null
                };

                var Fields = "userEnteredValue";

                if (NameChanged)
                {
                    Fields = "userEnteredValue,userEnteredFormat";
                    if (MLTranslated)
                    {
                        NameTextCellData.UserEnteredFormat = new CellFormat()
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
                        NameTextCellData.UserEnteredFormat = new CellFormat()
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
                            NameTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = ID2}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Name}
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
            string outPath = outRootPath + Path.DirectorySeparatorChar + "Alchemy.txt";
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Alchemy"];

            Console.WriteLine("Getting Alchemy Spreadsheet content");

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

            Console.WriteLine("Constructing Alchemy.txt...");
            using (StreamWriter sw = File.AppendText(outPath))
            {
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        sw.Write((string)row[0] + '\t'); //ID
                        sw.Write((string)row[1] + '\t'); //Name
                        sw.Write((string)row[3] + '\t'); //ID2
                        sw.Write('\r');
                    }
                }
            }
            Console.WriteLine("Done!");
        }

        public void UpdateSheetFromGameFile(string gameFileRootPath)
        {
            string gameFilePath = gameFileRootPath + Path.DirectorySeparatorChar + "Alchemy.bytes";

            //Getting all current entries
            Dictionary<string, AlchemyEntry> alchemyEntries = new Dictionary<string, AlchemyEntry>();

            Console.WriteLine("Getting Alchemy Spreadsheet content");
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Alchemy"];
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AlchemyEntry ae = new AlchemyEntry(row);
                    ae.row = rowC;
                    alchemyEntries[ae.ID] = ae;
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
                string OriginalName = data[1];
                string ID2 = data[2];
                string StandardizedTermLocator_Name = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalName);

                if (alchemyEntries.ContainsKey(ID))
                {
                    //Compare and update
                    AlchemyEntry existingEntry = alchemyEntries[ID];
                    bool needsUpdate = false;

                    if (!string.IsNullOrEmpty(OriginalName) && existingEntry.OriginalName != OriginalName)
                    {
                        existingEntry.OriginalName = OriginalName;
                        existingEntry.NameChanged = true;
                        needsUpdate = true;
                    }

                    if (existingEntry.ID2 != ID2)
                    {
                        existingEntry.ID2 = ID2;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Name != StandardizedTermLocator_Name)
                    {
                        existingEntry.StandardizedTermLocator_Name = StandardizedTermLocator_Name;
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
                    AlchemyEntry newEntry = new AlchemyEntry();
                    newEntry.ID = ID;
                    newEntry.Name = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Name);
                    newEntry.OriginalName = OriginalName;
                    newEntry.ID2 = ID2;

                    newEntry.StandardizedTermLocator_Name = StandardizedTermLocator_Name;
                    newEntry.NameChanged = true;
                    newEntry.MLTranslated = true;

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