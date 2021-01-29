using System;
using System.Collections.Generic;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class TalkAssetHandler : IFileHandler
    {
        public void BuildGameDataFromSheet(string outRootPath)
        {
            throw new System.NotImplementedException();
        }

        private class TalkEntry
        {
            public string DialogID = null;
            public string TalkerID = null;
            public string C = null;
            public string D = null;
            public string Text = null;
            public string OriginalText = null;
            public string PreviousOriginalText = null;
            public string NextDialogID = null;
            public string I = null;
            public string AlternativeNextDialogID = null;
            public string K = null;
            public string Script = null;
            public string SecondScript = null;
            public string TranslatorNotes = "";
            public string StandardizedTermLocator = "";
            public int row = -1;

            public bool TextChanged = false;
            public bool TextMLTranslated = false;

            public TalkEntry(string dialogID)
            {
                DialogID = dialogID;
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Talk"];
                Request updateRequest = new Request();

                var TextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Text },
                    UserEnteredFormat = null
                };
                var Fields = "userEnteredValue";

                if (TextChanged)
                {
                    Fields = "userEnteredValue,userEnteredFormat";
                    if (TextMLTranslated)
                    {
                        TextCellData.UserEnteredFormat = new CellFormat()
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
                        TextCellData.UserEnteredFormat = new CellFormat()
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
                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = DialogID },
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = TalkerID }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = C }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = D }
                                    },

                                    TextCellData,

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = OriginalText }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = PreviousOriginalText }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = NextDialogID }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = I }
                                    },

                                   new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = AlternativeNextDialogID }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = K }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = Script }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = SecondScript }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = TranslatorNotes }
                                    },

                                    new CellData() {
                                        UserEnteredValue = new ExtendedValue() { StringValue = StandardizedTermLocator }
                                    },
                                },
                            },
                        };

                if (row >= 2)
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

        public void UpdateSheetFromGameFile(string gameFilePath)
        {
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Talk"];

            Console.WriteLine("Getting Talk Spreadsheet content");

            //Getting all current entries
            Dictionary<string, TalkEntry> talkEntries = new Dictionary<string, TalkEntry>();

            string range = "A2:O";
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, range);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    TalkEntry te = new TalkEntry((string)row[0]);
                    te.TalkerID = (string)row[1];
                    te.C = (string)row[2];
                    te.D = (string)row[3];
                    te.Text = (string)row[4];
                    te.OriginalText = (string)row[5];
                    te.PreviousOriginalText = (string)row[6];
                    te.NextDialogID = (string)row[7];
                    te.I = (string)row[8];
                    te.AlternativeNextDialogID = (string)row[9];
                    te.K = (string)row[10];
                    te.Script = (string)row[11];
                    te.SecondScript = (string)row[12];
                    if (row.Count > 13)
                    {
                        te.TranslatorNotes = (string)row[13];
                    }
                    if (row.Count > 14)
                    {
                        te.StandardizedTermLocator = (string)row[14];
                    }

                    te.row = rowC;

                    talkEntries[te.DialogID] = te;
                    rowC++;
                }
            }
            //UpdatingRequests
            var updateRequests = new List<Request>();

            var standardizedTermLocator = StandardizedTermManager.GetInstance();

            //Parse every line in the game file
            System.IO.StreamReader reader = new System.IO.StreamReader(gameFilePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //We first parse the line into the parts
                string[] data = line.Split('\t');

                string dialogID = data[0];
                string talkerID = data[1];
                string valueC = data[2];
                string valueD = data[3];
                string originalText = data[4];
                string nextDialogId = data[5];
                string valueI = data[6];
                string alternativeNextDialogID = data[7];
                string valueK = data[8];
                string script = data[9];
                string secondScript = data[10];
                string standardizedTermText = standardizedTermLocator.GetTermLocatorText(originalText);

                if (talkEntries.ContainsKey(dialogID))
                {
                    //Compare and update
                    TalkEntry existingEntry = talkEntries[dialogID];
                    bool needsUpdate = false;

                    //First we take care of originalText, previousOriginalText and Text
                    if (!string.IsNullOrEmpty(originalText) && existingEntry.OriginalText != originalText)
                    {
                        existingEntry.TextChanged = true;

                        //Update the original Text and previous Text

                        existingEntry.PreviousOriginalText = existingEntry.OriginalText;
                        existingEntry.OriginalText = originalText;
                        existingEntry.StandardizedTermLocator = standardizedTermText;
                        needsUpdate = true;
                    }

                    if (string.IsNullOrEmpty(existingEntry.StandardizedTermLocator)  && !string.IsNullOrEmpty(standardizedTermText))
                    {
                        existingEntry.StandardizedTermLocator = standardizedTermText;
                        needsUpdate = true;
                    }

                    //Updating TalkerID
                    if (existingEntry.TalkerID != talkerID)
                    {
                        existingEntry.TalkerID = talkerID;
                        needsUpdate = true;
                    }
                    //Updating NextDialogID
                    if (existingEntry.TalkerID != talkerID)
                    {
                        existingEntry.TalkerID = talkerID;
                        needsUpdate = true;
                    }

                    //Updating NextDialogID
                    if (existingEntry.NextDialogID != nextDialogId)
                    {
                        existingEntry.NextDialogID = nextDialogId;
                        needsUpdate = true;
                    }

                    //Updating AlternativeNextDialogID
                    if (existingEntry.AlternativeNextDialogID != alternativeNextDialogID)
                    {
                        existingEntry.AlternativeNextDialogID = alternativeNextDialogID;
                        needsUpdate = true;
                    }


                    //Updating C, D, I and K
                    if (existingEntry.C != valueC)
                    {
                        existingEntry.C = valueC;
                        needsUpdate = true;
                    }

                    if (existingEntry.D != valueD)
                    {
                        existingEntry.D = valueD;
                        needsUpdate = true;
                    }

                    if (existingEntry.I != valueI)
                    {
                        existingEntry.I = valueI;
                        needsUpdate = true;
                    }

                    if (existingEntry.K != valueK)
                    {
                        existingEntry.K = valueK;
                        needsUpdate = true;
                    }



                    //Updating Script and SecondScript

                    if (existingEntry.Script != script)
                    {
                        existingEntry.Script = script;
                        needsUpdate = true;
                    }

                    if (existingEntry.SecondScript != secondScript)
                    {
                        existingEntry.SecondScript = secondScript;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        updateRequests.Add(existingEntry.ToGoogleSheetUpdateRequest());
                    }
                }
                else
                {
                    //TalkEntry is unknown, add a new one
                    TalkEntry newEntry = new TalkEntry(dialogID);
                    newEntry.TalkerID = talkerID;
                    newEntry.C = valueC;
                    newEntry.D = valueD;
                    newEntry.OriginalText = originalText;
                    newEntry.NextDialogID = nextDialogId;
                    newEntry.K = valueK;
                    newEntry.AlternativeNextDialogID = alternativeNextDialogID;
                    newEntry.I = valueI;
                    newEntry.Script = script;
                    newEntry.SecondScript = secondScript;
                    newEntry.StandardizedTermLocator = standardizedTermText;

                    newEntry.TextChanged = true;

                    //Doing intial ML Translation
                    var translatedText = TranslationManager.GetInstance().Translate(originalText);
                    newEntry.Text = translatedText;
                    newEntry.TextMLTranslated = true;


                    rowC++;
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