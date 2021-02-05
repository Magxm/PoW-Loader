using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class SkillAssetHandler : IFileHandler
    {
        public static string SheetRange = "A2:AI";

        private class SkillEntry
        {
            public string ID;
            public string Name;
            public string OriginalName;
            public string PreviousOriginalName;
            public string Description;
            public string OriginalDescription;
            public string PreviousOriginalDescription;
            public string SkillSetName;
            public string OriginalSkillSetName;
            public string PreviousOriginalSkillSetName;
            public string N;
            public string RequirementType;
            public string RequirementAmount;
            public string Q;
            public string R;
            public string S;
            public string T;
            public string U;
            public string V;
            public string W;
            public string X;
            public string Y;
            public string Z;
            public string AA;
            public string AB;
            public string AC;
            public string AD;
            public string AE;
            public string AF;
            public string AG;
            public string Script;

            public string StandardizedTermLocator_Name = "";
            public string StandardizedTermLocator_Description = "";
            public string StandardizedTermLocator_SkillSetName = "";

            public int row = -1;

            public bool SomeOriginalChanged = false;
            public bool MLTranslated = false;

            public SkillEntry()
            {
            }

            public SkillEntry(IList<object> row)
            {
                ID = (string)row[0];
                Name = (string)row[1];
                OriginalName = (string)row[2];
                PreviousOriginalName = (string)row[3];
                StandardizedTermLocator_Name = (string)row[4];
                Description = (string)row[5];
                OriginalDescription = (string)row[6];
                PreviousOriginalDescription = (string)row[7];
                StandardizedTermLocator_Description = (string)row[8];
                SkillSetName = (string)row[9];
                OriginalSkillSetName = (string)row[10];
                PreviousOriginalSkillSetName = (string)row[11];
                StandardizedTermLocator_SkillSetName = (string)row[12];
                N = (string)row[13];
                RequirementType = (string)row[14];
                RequirementAmount = (string)row[15];
                Q = (string)row[16];
                R = (string)row[17];
                S = (string)row[18];
                T = (string)row[19];
                U = (string)row[20];
                V = (string)row[21];
                W = (string)row[22];
                X = (string)row[23];
                Y = (string)row[24];
                Z = (string)row[25];
                AA = (string)row[26];
                AB = (string)row[27];
                AC = (string)row[28];
                AD = (string)row[29];
                AE = (string)row[30];
                AF = (string)row[31];
                AG = (string)row[32];
                Script = (string)row[33];
            }

            public Request ToGoogleSheetUpdateRequest()
            {
                GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
                string talkSpreadsheetId = gsc.SpreadsheetIDs["Skill"];
                Request updateRequest = new Request();

                var NameCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Name },
                    UserEnteredFormat = null
                };

                var DescriptionCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = Description },
                    UserEnteredFormat = null
                };

                var SkillSetNameTextCellData = new CellData()
                {
                    UserEnteredValue = new ExtendedValue() { StringValue = SkillSetName },
                    UserEnteredFormat = null
                };

                var Fields = "userEnteredValue";

                if (SomeOriginalChanged)
                {
                    Fields = "userEnteredValue,userEnteredFormat";
                    if (MLTranslated)
                    {
                        NameCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };

                        DescriptionCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 1.0f,
                                Green = 0.66f,
                                Blue = 0.0f
                            }
                        };

                        SkillSetNameTextCellData.UserEnteredFormat = new CellFormat()
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
                        NameCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };

                        DescriptionCellData.UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = new Color()
                            {
                                Alpha = 1.0f,
                                Red = 0.8f,
                                Green = 0.8f,
                                Blue = 0.02f
                            }
                        };

                        SkillSetNameTextCellData.UserEnteredFormat = new CellFormat()
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
                            //Name
                            NameCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = PreviousOriginalName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Name}
                            },
                            //Description
                            DescriptionCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalDescription}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = PreviousOriginalDescription}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_Description}
                            },
                            //SkillSetName
                           SkillSetNameTextCellData,
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = OriginalSkillSetName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = PreviousOriginalSkillSetName}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = StandardizedTermLocator_SkillSetName}
                            },
                            //Others
                             new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = N}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = RequirementType}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = RequirementAmount}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Q}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = R}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = S}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = T}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = U}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = V}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = W}
                            },
                             new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = X}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Y}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Z}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AA}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AB}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AC}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AD}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AE}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AF}
                            },
                             new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = AG}
                            },
                            new CellData()
                            {
                                UserEnteredValue = new ExtendedValue(){StringValue = Script}
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
            string outPath = outRootPath + Path.DirectorySeparatorChar + "Skill.txt";
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Skill"];

            Console.WriteLine("Getting Skill Spreadsheet content");

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

            Console.WriteLine("Constructing Skill.txt...");
            using (StreamWriter sw = File.AppendText(outPath))
            {
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        sw.Write((string)row[0] + '\t'); //ID
                        sw.Write((string)row[1] + '\t'); //Name
                        sw.Write((string)row[5] + '\t'); //Description
                        sw.Write((string)row[9] + '\t'); //Skillset
                        sw.Write((string)row[13] + '\t'); //N
                        sw.Write((string)row[14] + '\t'); //Requirement Type
                        sw.Write((string)row[15] + '\t'); //Requirement Amount
                        sw.Write((string)row[16] + '\t'); //Q
                        sw.Write((string)row[17] + '\t'); //R
                        sw.Write((string)row[18] + '\t'); //S
                        sw.Write((string)row[19] + '\t'); //T
                        sw.Write((string)row[20] + '\t'); //U
                        sw.Write((string)row[21] + '\t'); //V
                        sw.Write((string)row[22] + '\t'); //W
                        sw.Write((string)row[23] + '\t'); //X
                        sw.Write((string)row[24] + '\t'); //Y
                        sw.Write((string)row[25] + '\t'); //Z
                        sw.Write((string)row[26] + '\t'); //AA
                        sw.Write((string)row[27] + '\t'); //AB
                        sw.Write((string)row[28] + '\t'); //AC
                        sw.Write((string)row[29] + '\t'); //AD
                        sw.Write((string)row[30] + '\t'); //AE
                        sw.Write((string)row[31] + '\t'); //AF
                        sw.Write((string)row[32] + '\t'); //AG
                        sw.Write((string)row[33] + '\t'); //Script
                        sw.Write('\r');
                    }
                }
            }
            Console.WriteLine("Done!");
        }

        public void UpdateSheetFromGameFile(string gameFileRootPath)
        {
            string gameFilePath = gameFileRootPath + Path.DirectorySeparatorChar + "Skill.bytes";

            //Getting all current entries
            Dictionary<string, SkillEntry> skillEntries = new Dictionary<string, SkillEntry>();

            Console.WriteLine("Getting Skill Spreadsheet content");
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();
            string talkSpreadsheetId = gsc.SpreadsheetIDs["Skill"];
            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(talkSpreadsheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;
            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    SkillEntry ae = new SkillEntry(row);
                    ae.row = rowC;
                    skillEntries[ae.ID] = ae;
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
                string OriginalDescription = data[2];
                string OriginalSkillSetName = data[3];
                string N = data[4];
                string RequirementType = data[5];
                string RequirementAmount = data[6];
                string Q = data[7];
                string R = data[8];
                string S = data[9];
                string T = data[10];
                string U = data[11];
                string V = data[12];
                string W = data[13];
                string X = data[14];
                string Y = data[15];
                string Z = data[16];
                string AA = data[17];
                string AB = data[18];
                string AC = data[19];
                string AD = data[20];
                string AE = data[21];
                string AF = data[22];
                string AG = data[23];
                string Script = data[24];

                string StandardizedTermLocator_Name = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalName);
                string StandardizedTermLocator_Description = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalDescription);
                string StandardizedTermLocator_SkillSetName = StandardizedTermManager.GetInstance().GetTermLocatorText(OriginalSkillSetName);

                if (skillEntries.ContainsKey(ID))
                {
                    //Compare and update
                    SkillEntry existingEntry = skillEntries[ID];
                    bool needsUpdate = false;

                    if (!string.IsNullOrEmpty(OriginalName) && existingEntry.OriginalName != OriginalName)
                    {
                        if (string.IsNullOrEmpty(existingEntry.Name))
                        {
                            TranslationManager.GetInstance().Translate(StandardizedTermLocator_Name);
                        }
                        existingEntry.PreviousOriginalName = existingEntry.OriginalName;
                        existingEntry.OriginalName = OriginalName;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalDescription) && existingEntry.OriginalDescription != OriginalDescription)
                    {
                        if (string.IsNullOrEmpty(existingEntry.Description))
                        {
                            TranslationManager.GetInstance().Translate(StandardizedTermLocator_Description);
                        }
                        existingEntry.PreviousOriginalDescription = existingEntry.OriginalDescription;
                        existingEntry.OriginalDescription = OriginalDescription;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (!string.IsNullOrEmpty(OriginalSkillSetName) && existingEntry.OriginalSkillSetName != OriginalSkillSetName)
                    {
                        if (string.IsNullOrEmpty(existingEntry.SkillSetName))
                        {
                            TranslationManager.GetInstance().Translate(StandardizedTermLocator_SkillSetName);
                        }
                        existingEntry.PreviousOriginalSkillSetName = existingEntry.OriginalSkillSetName;
                        existingEntry.OriginalSkillSetName = OriginalSkillSetName;
                        existingEntry.SomeOriginalChanged = true;
                        needsUpdate = true;
                    }

                    if (existingEntry.N != N)
                    {
                        existingEntry.N = N;
                        needsUpdate = true;
                    }

                    if (existingEntry.RequirementType != RequirementType)
                    {
                        existingEntry.RequirementType = RequirementType;
                        needsUpdate = true;
                    }

                    if (existingEntry.RequirementAmount != RequirementAmount)
                    {
                        existingEntry.RequirementAmount = RequirementAmount;
                        needsUpdate = true;
                    }

                    if (existingEntry.Q != Q)
                    {
                        existingEntry.Q = Q;
                        needsUpdate = true;
                    }

                    if (existingEntry.R != R)
                    {
                        existingEntry.R = R;
                        needsUpdate = true;
                    }

                    if (existingEntry.S != N)
                    {
                        existingEntry.S = S;
                        needsUpdate = true;
                    }

                    if (existingEntry.T != N)
                    {
                        existingEntry.T = T;
                        needsUpdate = true;
                    }

                    if (existingEntry.U != U)
                    {
                        existingEntry.U = U;
                        needsUpdate = true;
                    }

                    if (existingEntry.V != V)
                    {
                        existingEntry.V = V;
                        needsUpdate = true;
                    }
                    if (existingEntry.W != W)
                    {
                        existingEntry.W = W;
                        needsUpdate = true;
                    }

                    if (existingEntry.X != X)
                    {
                        existingEntry.X = X;
                        needsUpdate = true;
                    }
                    if (existingEntry.Y != Y)
                    {
                        existingEntry.Y = Y;
                        needsUpdate = true;
                    }
                    if (existingEntry.Z != Z)
                    {
                        existingEntry.Z = Z;
                        needsUpdate = true;
                    }
                    if (existingEntry.AA != AA)
                    {
                        existingEntry.AA = AA;
                        needsUpdate = true;
                    }
                    if (existingEntry.AB != AB)
                    {
                        existingEntry.AB = AB;
                        needsUpdate = true;
                    }
                    if (existingEntry.AC != AC)
                    {
                        existingEntry.AC = AC;
                        needsUpdate = true;
                    }
                    if (existingEntry.AD != AD)
                    {
                        existingEntry.AD = AD;
                        needsUpdate = true;
                    }
                    if (existingEntry.AE != AE)
                    {
                        existingEntry.AE = AE;
                        needsUpdate = true;
                    }
                    if (existingEntry.AF != AF)
                    {
                        existingEntry.AF = AF;
                        needsUpdate = true;
                    }
                    if (existingEntry.AG != AG)
                    {
                        existingEntry.AG = AG;
                        needsUpdate = true;
                    }

                    if (existingEntry.Script != Script)
                    {
                        existingEntry.Script = Script;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Name != StandardizedTermLocator_Name)
                    {
                        existingEntry.StandardizedTermLocator_Name = StandardizedTermLocator_Name;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_Description != StandardizedTermLocator_Description)
                    {
                        existingEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;
                        needsUpdate = true;
                    }

                    if (existingEntry.StandardizedTermLocator_SkillSetName != StandardizedTermLocator_SkillSetName)
                    {
                        existingEntry.StandardizedTermLocator_SkillSetName = StandardizedTermLocator_SkillSetName;
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
                    SkillEntry newEntry = new SkillEntry();
                    newEntry.ID = ID;
                    newEntry.Name = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Name);
                    newEntry.OriginalName = OriginalName;
                    newEntry.StandardizedTermLocator_Name = StandardizedTermLocator_Name;
                    newEntry.Description = TranslationManager.GetInstance().Translate(StandardizedTermLocator_Description);
                    newEntry.OriginalDescription = OriginalDescription;
                    newEntry.StandardizedTermLocator_Description = StandardizedTermLocator_Description;
                    newEntry.SkillSetName = TranslationManager.GetInstance().Translate(StandardizedTermLocator_SkillSetName);
                    newEntry.OriginalSkillSetName = OriginalSkillSetName;
                    newEntry.StandardizedTermLocator_SkillSetName = StandardizedTermLocator_SkillSetName;
                    newEntry.N = N;
                    newEntry.RequirementType = RequirementType;
                    newEntry.RequirementAmount = RequirementAmount;
                    newEntry.Q = Q;
                    newEntry.R = R;
                    newEntry.S = S;
                    newEntry.T = T;
                    newEntry.U = U;
                    newEntry.V = V;
                    newEntry.W = W;
                    newEntry.X = X;
                    newEntry.Y = Y;
                    newEntry.Z = Z;
                    newEntry.AA = AA;
                    newEntry.AB = AB;
                    newEntry.AC = AC;
                    newEntry.AD = AD;
                    newEntry.AE = AE;
                    newEntry.AF = AF;
                    newEntry.AG = AG;
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

            Console.WriteLine("Done!");
        }
    }
}