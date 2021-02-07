using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class AdjustmentAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:Q";

        private class AdjustmentEntry
        {
            public string ID;
            public string TitleText;
            public string OriginalTitle;
            public string SecondTitle;
            public string OriginalSecondTitle;
            public string Description;
            public string OriginalDescription;
            public string Element1;
            public string Element2;
            public string J;
            public string H;
            public string NpcId1;
            public string NpcId2;
            public string NpcId3;
            public string StandardizedTermLocator_Title = "";
            public string StandardizedTermLocator_SecondTitle = "";
            public string StandardizedTermLocator_Description = "";

            public int row = -1;

            public bool TitleChanged = false;
            public bool SecondTitleChanged = false;
            public bool DescriptionChanged = false;
            public bool MLTranslated = false;

            public AdjustmentEntry()
            {
            }

            public AdjustmentEntry(IList<object> row)
            {
                ID = (string)row[0];
                TitleText = (string)row[1];
                OriginalTitle = (string)row[2];
                SecondTitle = (string)row[3];
                OriginalSecondTitle = (string)row[4];
                Description = (string)row[5];
                OriginalDescription = (string)row[6];
                Element1 = (string)row[7];
                Element2 = (string)row[8];
                J = (string)row[9];
                H = (string)row[10];
                NpcId1 = (string)row[11];
                NpcId2 = (string)row[12];
                NpcId3 = (string)row[13];
                if (row.Count > 14)
                {
                    StandardizedTermLocator_Title = (string)row[14];
                }
                if (row.Count > 15)
                {
                    StandardizedTermLocator_SecondTitle = (string)row[15];
                }
                if (row.Count > 16)
                {
                    StandardizedTermLocator_Description = (string)row[16];
                }
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Adjustment"];
                Request updateRequest = new Request();

                var TitleTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = TitleText },
                    UserEnteredFormat = null
                };
                var SecondTitleTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = SecondTitle },
                    UserEnteredFormat = null
                };
                var DescriptionTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Description },
                    UserEnteredFormat = null
                };

                var Fields = "userEnteredValue";

                if (TitleChanged || SecondTitleChanged || DescriptionChanged)
                {
                    Fields = "userEnteredValue,userEnteredFormat";
                    if (MLTranslated)
                    {
                        TitleTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };

                        SecondTitleTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };

                        DescriptionTextCellData.UserEnteredFormat = new CellFormat()
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
                        TitleTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };
                        SecondTitleTextCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };

                        DescriptionTextCellData.UserEnteredFormat = new CellFormat()
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
                            TitleTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalTitle}
                            },
                            SecondTitleTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalSecondTitle}
                            },
                            DescriptionTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalDescription}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Element1}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Element2}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = J}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = H}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = NpcId1}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = NpcId2}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = NpcId3}
                            },
                             new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Title}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_SecondTitle}
                            },
                             new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Description}
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
            string outPath = outRootPath + Path.DirectorySeparatorChar + "Adjustment.txt";
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Adjustment"];

            Console.WriteLine("Getting Adjustment Spreadsheet content");

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

            Console.WriteLine("Constructing Adjustment.txt...");
            using (StreamWriter sw = File.AppendText(outPath))
            {
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        sw.Write((string)row[0] + '\t'); //ID
                        sw.Write((string)row[1] + '\t'); //Title
                        sw.Write((string)row[3] + '\t'); //SecondTitle
                        sw.Write((string)row[5] + '\t'); //Description
                        sw.Write((string)row[7] + '\t'); //Element1
                        sw.Write((string)row[8] + '\t'); //Element2
                        sw.Write((string)row[9] + '\t'); //J
                        sw.Write((string)row[10] + '\t'); //H
                        sw.Write((string)row[11] + '\t'); //NPC ID 1
                        sw.Write((string)row[12] + '\t'); //NPC ID 2
                        sw.Write((string)row[13]); //NPC ID 3
                        sw.Write('\r');
                    }
                }
            }
            Console.WriteLine("Done!");
        }

        public void UpdateSheetFromGameFile(string gameFileRootPath)
        {
            string gameFilePath = gameFileRootPath + Path.DirectorySeparatorChar + "Adjustment.bytes";

            //Getting all current entries
            Dictionary<string, AdjustmentEntry> adjustmentEntries = new Dictionary<string, AdjustmentEntry>();

            Console.WriteLine("Getting Adjustment Spreadsheet content");
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Adjustment"];
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AdjustmentEntry ae = new AdjustmentEntry(row);
                    ae.row = rowC;
                    adjustmentEntries[ae.ID] = ae;
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
                string OriginalTitle = data[1];
                string OriginalSecondTitle = data[2];
                string OriginalDescription = data[3];
                string Element1 = data[4];
                string Element2 = data[5];
                string J = data[6];
                string H = data[7];
                string NpcId1 = data[8];
                string NpcId2 = data[9];
                string NpcId3 = data[10];
                string StandardizedTermLocator_Title = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalTitle);
                string StandardizedTermLocator_SecondTitle = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalSecondTitle);
                string StandardizedTermLocator_Description = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalDescription);

                if (adjustmentEntries.ContainsKey(ID))
                {
                    //Compare and update
                    AdjustmentEntry existingEntry = adjustmentEntries[ID];
                    bool needsUpdate = false;

                    if (!string.IsNullOrEmpty(OriginalTitle) && existingEntry.OriginalTitle != OriginalTitle)
                    {
                        existingEntry.OriginalTitle = OriginalTitle;
                        existingEntry.TitleChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalSecondTitle) && existingEntry.OriginalSecondTitle != OriginalSecondTitle)
                    {
                        existingEntry.OriginalSecondTitle = OriginalSecondTitle;
                        existingEntry.SecondTitleChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalDescription) && existingEntry.OriginalDescription != OriginalDescription)
                    {
                        existingEntry.OriginalDescription = OriginalDescription;
                        existingEntry.DescriptionChanged = true;
                        needsUpdate = true;
                    }

                    if (existingEntry.Element1 != Element1)
                    {
                        existingEntry.Element1 = Element1;
                        needsUpdate = true;
                    }

                    if (existingEntry.Element2 != Element2)
                    {
                        existingEntry.Element2 = Element2;
                        needsUpdate = true;
                    }

                    if (existingEntry.J != J)
                    {
                        existingEntry.J = J;
                        needsUpdate = true;
                    }

                    if (existingEntry.H != H)
                    {
                        existingEntry.H = H;
                        needsUpdate = true;
                    }

                    if (existingEntry.NpcId1 != NpcId1)
                    {
                        existingEntry.NpcId1 = NpcId1;
                        needsUpdate = true;
                    }

                    if (existingEntry.NpcId2 != NpcId2)
                    {
                        int sLength = NpcId2.Length;
                        int s2Length = existingEntry.NpcId2.Length;
                        existingEntry.NpcId2 = NpcId2;
                        needsUpdate = true;
                    }

                    if (existingEntry.NpcId3 != NpcId3)
                    {
                        existingEntry.NpcId3 = NpcId3;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Title != StandardizedTermLocator_Title)
                    {
                        existingEntry.StandardizedTermLocator_Title = StandardizedTermLocator_Title;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_SecondTitle != StandardizedTermLocator_SecondTitle)
                    {
                        existingEntry.StandardizedTermLocator_SecondTitle = StandardizedTermLocator_SecondTitle;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Description != StandardizedTermLocator_Description)
                    {
                        existingEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;
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
                    AdjustmentEntry newEntry = new AdjustmentEntry();
                    newEntry.ID = ID;
                    newEntry.TitleText = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Title);
                    newEntry.OriginalTitle = OriginalTitle;
                    newEntry.SecondTitle = TranslationManager.GetInstance().Translate(StandardizedTermLocator_SecondTitle);
                    newEntry.OriginalSecondTitle = OriginalSecondTitle;
                    newEntry.Description = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Description);
                    newEntry.OriginalDescription = OriginalDescription;
                    newEntry.Element1 = Element1;
                    newEntry.Element2 = Element2;
                    newEntry.J = J;
                    newEntry.H = H;
                    newEntry.NpcId1 = NpcId1;
                    newEntry.NpcId2 = NpcId2;
                    newEntry.NpcId3 = NpcId3;
                    newEntry.StandardizedTermLocator_Title = StandardizedTermLocator_Title;
                    newEntry.StandardizedTermLocator_SecondTitle = StandardizedTermLocator_SecondTitle;
                    newEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;

                    newEntry.TitleChanged = true;
                    newEntry.SecondTitleChanged = true;
                    newEntry.DescriptionChanged = true;
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