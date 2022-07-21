﻿using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoW_Tool_SheetUtilities.Handler
{
    public enum AssetVariableType
    {
        Translate,
        MachineTL,
        NoTranslate,
    }

    public class AssetVariableDefinition
    {
        public string Name;
        public AssetVariableType VariableType;
        public bool AutoML = true;
        public bool IsScriptField_FilterNoText = false;

        public override string ToString()
        {
            return "(AssetVariableDefinition) " + Name + " [" + VariableType.ToString() + "]";
        }
    }

    public class SheetCellWithColor
    {
        public string Value = "";
        public Color Color = null;

        public SheetCellWithColor(CellData cell)
        {
            if (cell.UserEnteredValue != null)
            {
                Value = cell.UserEnteredValue.StringValue;
            }
            if (cell.UserEnteredFormat != null)
            {
                Color = cell.UserEnteredFormat.BackgroundColor;
            }
        }
    }

    public class AssetVariable
    {
        private static bool IsSameColor(Color a, Color b)
        {
            if (a == null || b == null)
            {
                return a == null && b == null;
            }
            if (a.Red == null) a.Red = 0;
            if (a.Green == null) a.Green = 0;
            if (a.Blue == null) a.Blue = 0;
            if (b.Red == null) b.Red = 0;
            if (b.Green == null) b.Green = 0;
            if (b.Blue == null) b.Blue = 0;

            return a.Blue == b.Blue && a.Red == b.Red && a.Green == b.Green;
        }

        public static Color NeedsCheckColor = new Color()
        {
            Alpha = 1.0f,
            Red = 0.8f,
            Green = 0.8f,
            Blue = 0.0196078438f
        };

        public static Color MTLColor = new Color()
        {
            Alpha = 1.0f,
            Red = 1.0f,
            Green = 0.65882355f,
            Blue = 0.0f
        };

        public static Color StandardizedTermColor = new Color()
        {
            Alpha = 1.0f,
            Red = 0.91764705882f,
            Green = 0.81960784313f,
            Blue = 0.86274509803f
        };

        public static Color DoNotTouchColor = new Color()
        {
            Alpha = 1.0f,
            Red = 0.6f,
            Green = 0.6f,
            Blue = 0.6f
        };

        public static Color TranslatedColor = new Color()
        {
            Alpha = 1.0f,
            Red = 0.71372549019f,
            Green = 0.8431372549f,
            Blue = 0.65882352941f
        };

        public static Color ProofReadColor = new Color()
        {
            Alpha = 1.0f,
            Red = 0.78823529411f,
            Green = 0.85490196078f,
            Blue = 0.9725490196f
        };

        public AssetVariableDefinition Definition;

        public string NewOriginalValue;
        public string NewStandardizedTermLocator;

        public string Translation;
        public string OriginalValue;
        public string StandardizedTermLocator;

        public AssetVariable(AssetVariableDefinition varDef)
        {
            Definition = varDef;
        }

        private string GetOneColumnAndGoForward(IList<object> row, ref int columnIndex)
        {
            if (columnIndex < row.Count)
            {
                return (string)row[columnIndex++];
            }

            columnIndex++;
            return "";
        }

        public void ParseSheetData(IList<object> row, ref int columnIndex)
        {
            if (Definition.VariableType == AssetVariableType.NoTranslate)
            {
                //Simple variable
                OriginalValue = GetOneColumnAndGoForward(row, ref columnIndex);
                Translation = OriginalValue;
                StandardizedTermLocator = OriginalValue;
            }
            else if (Definition.VariableType == AssetVariableType.MachineTL)
            {
                //Translation + Original
                Translation = GetOneColumnAndGoForward(row, ref columnIndex);
                OriginalValue = GetOneColumnAndGoForward(row, ref columnIndex);
                StandardizedTermLocator = OriginalValue;
            }
            else
            {
                /*
                    We want to translate this variable:
                    There will be 4 Entries: Translated, Original Value and Standardized Term Locator
                */
                Translation = GetOneColumnAndGoForward(row, ref columnIndex); ;
                OriginalValue = GetOneColumnAndGoForward(row, ref columnIndex); ;
                StandardizedTermLocator = GetOneColumnAndGoForward(row, ref columnIndex); ;
            }
        }

        public void SetNewOriginal(string value)
        {
            NewOriginalValue = value;
            NewStandardizedTermLocator = StandardizedTermManager.GetInstance().GetTermLocatorText(NewOriginalValue);
        }

        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");

        private static bool IsChineseCharacter(char c)
        {
            return cjkCharRegex.IsMatch(c.ToString());
        }

        private static bool IsStringContainsChineseCharacters(string s)
        {
            return s.Any(c => IsChineseCharacter(c));
        }

        public List<Request> GenerateUpdateRequests(int row, ref int columnIndex)
        {
            List<Request> updateRequests = new List<Request>();
            if (Definition.VariableType == AssetVariableType.NoTranslate)
            {
                //Simple variable. We just compare and update if needed
                if (OriginalValue != NewOriginalValue)
                {
                    Request updateValueReq = new Request();
                    updateValueReq.UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange()
                        {
                            SheetId = 0,
                            StartRowIndex = row,
                            EndRowIndex = row + 1,
                            StartColumnIndex = columnIndex,
                            EndColumnIndex = columnIndex + 1
                        },
                        Rows = new List<RowData>
                        {
                            new RowData()
                            {
                                Values = new List<CellData>()
                                {
                                    new CellData()
                                    {
                                        UserEnteredValue = new ExtendedValue() { StringValue = NewOriginalValue }
                                    },
                                }
                            },
                        },
                        Fields = "userEnteredValue"
                    };
                    updateRequests.Add(updateValueReq);
                    OriginalValue = NewOriginalValue;
                }
                columnIndex++; //Original Value
            }
            else if (Definition.VariableType == AssetVariableType.MachineTL)
            {
                if (OriginalValue != NewOriginalValue)
                {
                    //We get a new ML Translation
                    Translation = MachineTranslator.TranslationManager.GetInstance().Translate(NewOriginalValue, new string[0], new string[0], NewStandardizedTermLocator);

                    //We update the translation and the original
                    Request updateReq = new Request();
                    updateReq.UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange()
                        {
                            SheetId = 0,
                            StartRowIndex = row,
                            EndRowIndex = row + 1,
                            StartColumnIndex = columnIndex,
                            EndColumnIndex = columnIndex + 2
                        },
                        Rows = new List<RowData>()
                            {
                                new RowData()
                                {
                                    Values = new List<CellData>()
                                    {
                                        new CellData()
                                        {
                                            UserEnteredValue = new ExtendedValue() { StringValue = Translation},
                                        },
                                        new CellData()
                                        {
                                            UserEnteredValue = new ExtendedValue() { StringValue = NewOriginalValue},
                                        },
                                    }
                                },
                            },
                        Fields = "userEnteredValue",
                    };
                    updateRequests.Add(updateReq);
                }
                columnIndex++; //Translation
                columnIndex++; //OriginalValue
            }
            else
            {
                //A variable we translate. We check if original changed.
                bool MLTranslationAdded = false;
                if (!string.IsNullOrEmpty(NewOriginalValue) && string.IsNullOrEmpty(Translation))
                {
                    MLTranslationAdded = true;
                    if (Definition.AutoML)
                    {
                        Translation = MachineTranslator.TranslationManager.GetInstance().Translate(NewOriginalValue, new string[0], new string[0], NewStandardizedTermLocator);
                    }
                    else
                    {
                        Translation = OriginalValue;
                    }
                }

                if (OriginalValue != NewOriginalValue || MLTranslationAdded)
                {
                    //We want to first mark the translation as needing to be rechecked
                    {
                        Request updateReq = new Request();
                        Color fColor = NeedsCheckColor;

                        if (string.IsNullOrEmpty(Translation) || Translation == "0")
                        {
                            fColor = TranslatedColor;
                        }
                        else if (MLTranslationAdded)
                        {
                            fColor = MTLColor;
                        }

                        if (Definition.IsScriptField_FilterNoText)
                        {
                            Translation = NewOriginalValue;
                            if (!IsStringContainsChineseCharacters(NewOriginalValue))
                            {
                                fColor = TranslatedColor;
                            }
                            else
                            {
                                fColor = NeedsCheckColor;
                            }
                        }

                        updateReq.UpdateCells = new UpdateCellsRequest
                        {
                            Range = new GridRange()
                            {
                                SheetId = 0,
                                StartRowIndex = row,
                                EndRowIndex = row + 1,
                                StartColumnIndex = columnIndex,
                                EndColumnIndex = columnIndex + 1
                            },
                            Rows = new List<RowData>()
                            {
                                new RowData()
                                {
                                    Values = new List<CellData>()
                                    {
                                        new CellData()
                                        {
                                            UserEnteredValue = new ExtendedValue() { StringValue = Translation},
                                            UserEnteredFormat = new CellFormat()
                                            {
                                                BackgroundColor = fColor
                                            }
                                        },
                                    }
                                },
                            },
                            Fields = "userEnteredValue,userEnteredFormat",
                        };
                        columnIndex++; //Translation
                        updateRequests.Add(updateReq);
                    }
                    //We update OriginalValue
                    OriginalValue = NewOriginalValue;
                    {
                        //Then we prepare the update request for these
                        Request updateValueReq = new Request();
                        updateValueReq.UpdateCells = new UpdateCellsRequest
                        {
                            Range = new GridRange()
                            {
                                SheetId = 0,
                                StartRowIndex = row,
                                EndRowIndex = row + 1,
                                StartColumnIndex = columnIndex,
                                EndColumnIndex = columnIndex + 1
                            },
                            Rows = new List<RowData>
                            {
                                new RowData()
                                {
                                    Values = new List<CellData>()
                                    {
                                        new CellData()
                                        {
                                            UserEnteredValue = new ExtendedValue() { StringValue = OriginalValue }
                                        },
                                    }
                                },
                            },
                            Fields = "userEnteredValue"
                        };
                        columnIndex++; //Original
                        updateRequests.Add(updateValueReq);
                    }
                }
                else
                {
                    columnIndex++; //Translation
                    columnIndex++; //Original
                }

                //Checking Standardized Term Locator.
                if (NewStandardizedTermLocator != StandardizedTermLocator)
                {
                    Request updateValueReq = new Request();
                    updateValueReq.UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange()
                        {
                            SheetId = 0,
                            StartRowIndex = row,
                            EndRowIndex = row + 1,
                            StartColumnIndex = columnIndex,
                            EndColumnIndex = columnIndex + 1
                        },
                        Rows = new List<RowData>
                        {
                            new RowData()
                            {
                                Values = new List<CellData>()
                                {
                                    new CellData()
                                    {
                                        UserEnteredValue = new ExtendedValue() { StringValue = NewStandardizedTermLocator }
                                    },
                                }
                            },
                        },
                        Fields = "userEnteredValue"
                    };
                    updateRequests.Add(updateValueReq);
                }
                columnIndex++; //StandarizedTermLocator
            }

            //if (Translation.Contains(@"""") && !Translation.Contains(@"\"""))
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Questionable quotes in translation " + Translation + " in variable with definition " + Definition.ToString());
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
            return updateRequests;
        }

        public List<CellData> GenerateAppendCellData()
        {
            if (Definition.VariableType == AssetVariableType.NoTranslate)
            {
                return new List<CellData>()
                {
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = NewOriginalValue },
                        UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = DoNotTouchColor,
                        }
                    }
                };
            }
            else if (Definition.VariableType == AssetVariableType.MachineTL)
            {
                Translation = MachineTranslator.TranslationManager.GetInstance().Translate(NewOriginalValue, new string[0], new string[0], NewStandardizedTermLocator);
                return new List<CellData>()
                {
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = Translation },
                        UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = DoNotTouchColor,
                        }
                    },
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = NewOriginalValue},
                        UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = DoNotTouchColor,
                        }
                    },
                };
            }
            else
            {
                if (Definition.AutoML)
                {
                    Translation = MachineTranslator.TranslationManager.GetInstance().Translate(NewOriginalValue, new string[0], new string[0], NewStandardizedTermLocator);
                }
                else
                {
                    Translation = OriginalValue;
                }

                Color fColor = MTLColor;
                if (string.IsNullOrEmpty(Translation) || Translation == "0" || (Definition.IsScriptField_FilterNoText && !IsStringContainsChineseCharacters(NewOriginalValue)))
                {
                    fColor = TranslatedColor;
                }

                return new List<CellData>()
                {
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = Translation },
                        UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = fColor,
                        }
                    },
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = NewOriginalValue},
                    },
                    new CellData()
                    {
                        UserEnteredValue = new ExtendedValue() { StringValue = NewStandardizedTermLocator},
                        UserEnteredFormat = new CellFormat()
                        {
                            BackgroundColor = StandardizedTermColor,
                        }
                    }
                };
            }
        }

        public void AppendToFile(StreamWriter sw)
        {
            if (Definition.VariableType == AssetVariableType.NoTranslate)
            {
                sw.Write(OriginalValue);
            }
            else if (Definition.VariableType == AssetVariableType.MachineTL)
            {
                sw.Write(OriginalValue);
            }
            else
            {
                sw.Write(Translation);
            }
        }

        private int _GetWordCount(string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }

        public void CalculateTranslationStats(SheetCellWithColor[] rowRaw, ref int columnIndex, ref List<TranslationStatEntry> stats)
        {
            if (Definition.VariableType == AssetVariableType.NoTranslate)
            {
                columnIndex++; //Original
                return;
            }
            else if (Definition.VariableType == AssetVariableType.MachineTL)
            {
                columnIndex++; //MTL
                columnIndex++; //Original
                return;
            }
            else
            {
                Color colorOfEntry = rowRaw[columnIndex].Color;
                string value = rowRaw[columnIndex++].Value; //Translation
                columnIndex++;//Original
                columnIndex++;//Standardized Term Locator
                if (string.IsNullOrWhiteSpace(value) || Translation == "0")
                {
                    return;
                }

                for (int i = 0; i < stats.Count; i++)
                {
                    bool foundMatch = false;
                    for (int cI = 0; cI < stats[i].AcceptableColors.Count; cI++)
                    {
                        Color acceptableColor = stats[i].AcceptableColors[cI];
                        if (stats[i].MatchAll || IsSameColor(colorOfEntry, acceptableColor))
                        {
                            stats[i].LineCount += 1;
                            stats[i].WordCount += _GetWordCount(value);
                            foundMatch = true;
                            break;
                        }
                    }
                    if (foundMatch) break;
                }
            }
        }
    }

    public class AssetEntry
    {
        public List<AssetVariableDefinition> VariableDefinitions;
        public AssetVariable[] Variables;
        public int Row = -1;

        public bool FoundInGameData = false;

        public AssetEntry()
        {
        }

        public AssetEntry(List<AssetVariableDefinition> variableDefinitions)
        {
            VariableDefinitions = variableDefinitions;
            Variables = new AssetVariable[VariableDefinitions.Count];
            for (int i = 0; i < VariableDefinitions.Count; i++)
            {
                AssetVariableDefinition varDef = VariableDefinitions[i];
                Variables[i] = new AssetVariable(varDef);
            }
        }

        public void PopulateBySheetRow(IList<object> row)
        {
            int columnIndex = 0;
            for (int i = 0; i < VariableDefinitions.Count; i++)
                Variables[i].ParseSheetData(row, ref columnIndex);
        }

        public void PopulateByGameAssetRow(string[] row)
        {
            FoundInGameData = true;
            int columnIndex = 0;
            for (int i = 0; i < VariableDefinitions.Count; i++)
                Variables[i].SetNewOriginal(row[columnIndex++]);
        }

        public List<Request> GetUpdateRequests()
        {
            List<Request> requests = new List<Request>();
            if (Row >= 1)
            {
                int columnIndex = 0;
                for (int i = 0; i < VariableDefinitions.Count; i++)
                {
                    List<Request> newReuests = Variables[i].GenerateUpdateRequests(Row, ref columnIndex);
                    foreach (Request req in newReuests)
                    {
                        requests.Add(req);
                    }
                }
            }
            else
            {
                List<CellData> cellData = new List<CellData>();
                for (int i = 0; i < VariableDefinitions.Count; i++)
                {
                    List<CellData> newCellData = Variables[i].GenerateAppendCellData();
                    foreach (CellData cd in newCellData)
                    {
                        cellData.Add(cd);
                    }
                }

                Request appendRequest = new Request();
                appendRequest.AppendCells = new AppendCellsRequest()
                {
                    Rows = new List<RowData>()
                    {
                        new RowData()
                        {
                            Values = cellData
                        }
                    },
                    SheetId = 0,
                    Fields = "userEnteredValue,userEnteredFormat"
                };
                requests.Add(appendRequest);
            }

            return requests;
        }

        public void AppendToFile(StreamWriter sw, AssetEntry thisEntry)
        {
            for (int i = 0; i < VariableDefinitions.Count; i++)
            {
                if (i > 0)
                {
                    sw.Write('\t');
                }
                Variables[i].AppendToFile(sw);
            }

            sw.Write('\r');
            sw.Write('\n');
        }

        public void CalculateTranslationStats(SheetCellWithColor[] rowRaw, ref List<TranslationStatEntry> stats)
        {
            int columnIndex = 0;
            for (int i = 0; i < Variables.Length; i++)
            {
                Variables[i].CalculateTranslationStats(rowRaw, ref columnIndex, ref stats);
            }
        }
    }

    public class AssetHandler : IFileHandler
    {
        public List<AssetVariableDefinition> VariableDefinitions = new List<AssetVariableDefinition>();
        public string SheetId;
        public string AssetName;
        public string SheetRange;
        public string FilePathWithoutExtension;
        public string OutputExtension;

        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public virtual void BuildGameDataFromSheet(string outRootPath)
        {
            string outFilePath = outRootPath + Path.DirectorySeparatorChar + FilePathWithoutExtension + OutputExtension;
            Console.WriteLine("Getting " + AssetName + " Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            //Clearing Asset File
            string outDirectory = Path.GetDirectoryName(outFilePath);
            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }
            //Resetting file
            File.WriteAllText(outFilePath, "");

            //Getting all Sheet entries and dumping them into output text asset in right format
            Console.WriteLine("Extracting to " + FilePathWithoutExtension + OutputExtension);
            StreamWriter sw = File.AppendText(outFilePath);

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AssetEntry thisEntry = new AssetEntry(VariableDefinitions);
                    thisEntry.PopulateBySheetRow(row);
                    thisEntry.AppendToFile(sw, thisEntry);
                }
            }

            sw.Close();
        }

        public void HandleUpdateRequests(ref List<Request> updateRequests)
        {
            if (updateRequests.Count > 0)
            {
                Console.WriteLine("Sending " + updateRequests.Count + " Requests to Google API...");
                BatchUpdateSpreadsheetRequest batchUpdate = new BatchUpdateSpreadsheetRequest();
                batchUpdate.Requests = updateRequests;
                var updateRequest = GoogleSheetConnector.GetInstance().Service.Spreadsheets.BatchUpdate(batchUpdate, SheetId);
                updateRequest.Execute();

                updateRequests.Clear();
            }
        }

        public virtual void UpdateSheetFromGameFile(string inputFolder)
        {
            string gameFilePath = inputFolder + Path.DirectorySeparatorChar + FilePathWithoutExtension + ".bytes";
            //Getting all current entries
            Dictionary<string, AssetEntry> entries = new Dictionary<string, AssetEntry>();

            Console.WriteLine("Getting " + AssetName + " Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AssetEntry ae = new AssetEntry(VariableDefinitions);
                    ae.PopulateBySheetRow(row);
                    ae.Row = rowC;
                    string index = (string)row[0];
                    if (entries.ContainsKey(index))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Detected multiple entries with key " + index + "! Using last one...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    entries[index] = ae;
                    rowC++;
                }
            }

            var updateRequests = new List<Request>();
            Console.WriteLine("Comparing with games version and calculating updates...");
            //Parse every line in the game asset file
            System.IO.StreamReader reader = new System.IO.StreamReader(gameFilePath);
            string[] lines = File.ReadAllLines(gameFilePath);

            //Testing row count
            if (lines.Length > 0)
            {
                string line = lines[0];
                string[] data = line.Split('\t');
                if (data.Length != VariableDefinitions.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CRITICAL: " + AssetName + " changed format!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Console.Title = "[" + AssetName + "] Processing " + i + "/" + lines.Length;
                //Splitting
                string[] data = line.Split('\t');
                AssetEntry entry;
                if (entries.ContainsKey(data[0]))
                {
                    //Compare and update
                    entry = entries[data[0]];
                }
                else
                {
                    //New entry
                    entry = new AssetEntry(VariableDefinitions);
                    entries[data[0]] = entry;
                }

                entry.PopulateByGameAssetRow(data);
                List<Request> updateReqs = entry.GetUpdateRequests();
                foreach (Request req in updateReqs)
                {
                    updateRequests.Add(req);
                }

                if (updateRequests.Count >= 200)
                {
                    HandleUpdateRequests(ref updateRequests);
                }
            }

            HandleUpdateRequests(ref updateRequests);

            foreach (var aeE in entries)
            {
                AssetEntry ae = aeE.Value;
                if (!ae.FoundInGameData)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unreferenced Entry " + aeE.Key + " in Row " + (ae.Row + 1));
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            Console.WriteLine("Done!");
            Console.WriteLine("");
        }

        public virtual void GetTranslationStats(ref List<TranslationStatEntry> stats)
        {
            Console.WriteLine("Calculating Translation Statistic for " + AssetName);
            SpreadsheetsResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Get(SheetId);
            request.Ranges = SheetRange;
            request.IncludeGridData = true;
            Spreadsheet sheet = request.Execute();
            IList<GridData> grid = sheet.Sheets[0].Data;
            //Getting each range (should only be one)
            AssetEntry tmpEntry = new AssetEntry(VariableDefinitions);
            foreach (GridData gridData in grid)
            {
                //For each row
                foreach (var row in gridData.RowData)
                {
                    SheetCellWithColor[] rowRaw = new SheetCellWithColor[row.Values.Count];
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        rowRaw[i] = new SheetCellWithColor(row.Values[i]);
                    }

                    tmpEntry.CalculateTranslationStats(rowRaw, ref stats);
                }
            }
        }
    }
}