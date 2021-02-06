using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.BattleAssets
{
    internal class BufferAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:P";

        private class BuffEntry
        {
            public string FileName;
            public string ID;
            public string BuffName;
            public string OriginalBuffName;
            public string Description;
            public string OriginalDescription;
            public string Tooltip;
            public string OriginalTooltip;
            public string BuffCategory;
            public string PosNeg;
            public string N;
            public string O;
            public string Script;

            public string StandardizedTermLocator_BuffName = "";
            public string StandardizedTermLocator_Description = "";
            public string StandardizedTermLocator_Tooltip = "";

            public int row = -1;

            public bool SomeOriginalChanged = false;
            public bool MLTranslated = false;

            public BuffEntry()
            {
            }

            public BuffEntry(IList<object> row)
            {
                FileName = (string)row[0];
                ID = (string)row[1];
                BuffName = (string)row[2];
                OriginalBuffName = (string)row[3];
                StandardizedTermLocator_BuffName = (string)row[4];
                Description = (string)row[5];
                OriginalDescription = (string)row[6];
                StandardizedTermLocator_Description = (string)row[7];
                Tooltip = (string)row[8];
                OriginalTooltip = (string)row[9];
                StandardizedTermLocator_Tooltip = (string)row[10];
                BuffCategory = (string)row[11];
                PosNeg = (string)row[12];
                N = (string)row[13];
                O = (string)row[14];
                Script = (string)row[15];
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Buffer"];
                Request updateRequest = new Request();

                var NameTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = BuffName },
                    UserEnteredFormat = null
                };

                var DescriptionTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Description },
                    UserEnteredFormat = null
                };

                var TooltipTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Tooltip },
                    UserEnteredFormat = null
                };

                var Fields = "userEnteredValue";

                if (SomeOriginalChanged)
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

                        TooltipTextCellData.UserEnteredFormat = new CellFormat()
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
                        TooltipTextCellData.UserEnteredFormat = new CellFormat()
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
                                UserEnteredValue = new ExtendedValue(){StringValue = FileName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = ID}
                            },
                            //Name
                            NameTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalBuffName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_BuffName}
                            },
                            //Description
                            DescriptionTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalDescription}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Description}
                            },
                            //Tooltip
                            TooltipTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalTooltip}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Tooltip}
                            },
                            //Others
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = BuffCategory}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = PosNeg}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = N}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = O}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Script}
                            }
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

        public void BuildGameDataFromSheet(string battleDirectory)
        {
            string buffersPath = battleDirectory + Path.DirectorySeparatorChar + "buffer";
            string mergeFilePath = battleDirectory + Path.DirectorySeparatorChar + "merge" + Path.DirectorySeparatorChar + "Buffer.txt";
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Buffer"];

            Console.WriteLine("Getting Buff/Debuff Spreadsheet content");

            //Getting all current entries
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            //Clearing folders
            string mergeRootFolder = Path.GetDirectoryName(mergeFilePath);
            if (!Directory.Exists(mergeRootFolder))
            {
                Directory.CreateDirectory(mergeRootFolder);
            }
            if (!Directory.Exists(buffersPath))
            {
                Directory.CreateDirectory(buffersPath);
            }
            File.WriteAllText(mergeFilePath, "");

            Console.WriteLine("Constructing Buff JSONs and merger file...");
            using (StreamWriter mergeFileWriter = File.AppendText(mergeFilePath))
            {
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        string dataString = (string)row[1] + '\t'; //ID
                        dataString += (string)row[2] + '\t'; //Buff name
                        dataString += (string)row[5] + '\t'; //Description
                        dataString += (string)row[8] + '\t'; //Tooltip
                        dataString += (string)row[11] + '\t'; //Buff Category
                        dataString += (string)row[12] + '\t';  //Pos/Neg
                        dataString += (string)row[13] + '\t'; //N
                        dataString += (string)row[14] + '\t'; //O
                        dataString += (string)row[15]; //Script

                        string bufferFilePath = buffersPath + Path.DirectorySeparatorChar + (string)row[0];
                        File.WriteAllText(bufferFilePath, dataString);

                        mergeFileWriter.Write(dataString + '\r');
                    }
                }
            }
            Console.WriteLine("Done!");
        }

        public void UpdateSheetFromGameFile(string battleDirectory)
        {
            string bufferFolderPath = battleDirectory + Path.DirectorySeparatorChar + "buffer";

            //Getting all current entries
            Dictionary<string, BuffEntry> buffEntries = new Dictionary<string, BuffEntry>();

            Console.WriteLine("Getting Buff/Debuff Spreadsheet content");
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Buffer"];
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    BuffEntry ae = new BuffEntry(row);
                    ae.row = rowC;
                    buffEntries[ae.FileName] = ae;
                    rowC++;
                }
            }

            //UpdatingRequests
            var updateRequests = new List<Request>();

            Console.WriteLine("Comparing with games version and calculating updates...");
            //Parse every buff file
            string[] files = Directory.GetFiles(bufferFolderPath, "*.bytes");
            foreach (string bufferFilePathTmp in files)
            {
                //Reading file
                System.IO.StreamReader reader = new System.IO.StreamReader(bufferFilePathTmp);
                string line = reader.ReadLine();

                //Construction relative .json path
                int lastSplitterIndex = bufferFilePathTmp.LastIndexOf(Path.DirectorySeparatorChar);
                string bufferFilePath = bufferFilePathTmp.Substring(lastSplitterIndex + 1, bufferFilePathTmp.Length - lastSplitterIndex - 1);
                bufferFilePath = bufferFilePath.Replace(".bytes", ".json");
                //Splitting
                string[] data = line.Split('\t');
                //Parsing data

                string ID = data[0];
                string OriginalBuffName = data[1];
                string OriginalDescription = data[2];
                string OriginalTooltip = data[3];
                string BuffCategory = data[4];
                string PosNeg = data[5];
                string N = data[6];
                string O = data[7];
                string Script = data[8];

                string StandardizedTermLocator_BuffName = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalBuffName);
                string StandardizedTermLocator_Description = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalDescription);
                string StandardizedTermLocator_Tooltip = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalTooltip);

                if (buffEntries.ContainsKey(bufferFilePath))
                {
                    //Compare and update
                    BuffEntry existingEntry = buffEntries[bufferFilePath];
                    bool needsUpdate = false;

                    if (existingEntry.ID != ID)
                    {
                        existingEntry.ID = ID;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalBuffName) && existingEntry.OriginalBuffName != OriginalBuffName)
                    {
                        existingEntry.OriginalBuffName = OriginalBuffName;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalDescription) && existingEntry.OriginalDescription != OriginalDescription)
                    {
                        existingEntry.OriginalDescription = OriginalDescription;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalTooltip) && existingEntry.OriginalTooltip != OriginalTooltip)
                    {
                        existingEntry.OriginalTooltip = OriginalTooltip;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (existingEntry.BuffCategory != BuffCategory)
                    {
                        existingEntry.BuffCategory = BuffCategory;
                        needsUpdate = true;
                    }
                    if (existingEntry.PosNeg != PosNeg)
                    {
                        existingEntry.PosNeg = PosNeg;
                        needsUpdate = true;
                    }
                    if (existingEntry.N != N)
                    {
                        existingEntry.N = N;
                        needsUpdate = true;
                    }
                    if (existingEntry.O != O)
                    {
                        existingEntry.O = O;
                        needsUpdate = true;
                    }
                    if (existingEntry.Script != Script)
                    {
                        existingEntry.Script = Script;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_BuffName != StandardizedTermLocator_BuffName)
                    {
                        existingEntry.StandardizedTermLocator_BuffName = StandardizedTermLocator_BuffName;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Description != StandardizedTermLocator_Description)
                    {
                        existingEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Tooltip != StandardizedTermLocator_Tooltip)
                    {
                        existingEntry.StandardizedTermLocator_Tooltip = StandardizedTermLocator_Tooltip;
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
                    BuffEntry newEntry = new BuffEntry();
                    newEntry.FileName = bufferFilePath;
                    newEntry.ID = ID;
                    newEntry.BuffName = TranslationManager.GetInstance().Translate(StandardizedTermLocator_BuffName);
                    newEntry.OriginalBuffName = OriginalBuffName;
                    newEntry.StandardizedTermLocator_BuffName = StandardizedTermLocator_BuffName;
                    newEntry.Description = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Description);
                    newEntry.OriginalDescription = OriginalDescription;
                    newEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;
                    newEntry.Tooltip = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Tooltip);
                    newEntry.OriginalTooltip = OriginalTooltip;
                    newEntry.StandardizedTermLocator_Tooltip = StandardizedTermLocator_Tooltip;
                    newEntry.BuffCategory = BuffCategory;
                    newEntry.PosNeg = PosNeg;
                    newEntry.N = N;
                    newEntry.O = O;
                    newEntry.Script = Script;

                    newEntry.SomeOriginalChanged = true;
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

            Console.WriteLine("Done! Updated " + reqHandled + " Entries!");
        }
    }
}