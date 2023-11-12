using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class TalkAssetHandler : AssetHandler
    {
        public TalkAssetHandler()
        {
            SheetId = "1lmIjyewgIZfy2tqanCz1h1aPaKDcsbMOKOu1W5sjQzw";
            AssetName = "Talk";
            SheetRange = "A2:O";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Talk";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "ID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "TalkerID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "EmotionType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "MessageType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Message",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "FailTalkId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "NextTalkType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "NextTalkId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Animation",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Condition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Behaviour",
                    VariableType = AssetVariableType.Translate,
                    AutoML = false,
                    IsScriptField_FilterNoText = true,
                },
            };
        }

        //Custom Handling of this one because of it's size and google api liking to complain if one requests all at once
        public override void GetTranslationStats(ref List<TranslationStatEntry> stats)
        {
            Console.WriteLine("Calculating Translation Statistic for " + AssetName);
            for (int k = 0; k < 7; k++)
            {
                SpreadsheetsResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Get(SheetId);
                request.Ranges = "A" + (k * 10000 + 2) + ":O" + ((k + 1) * 10000 + 2);
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

        public override void ExportTranslatedLinesToCSV(string outPath, ref List<Color> acceptableColors)
        {
            string outFilePath = outPath + Path.DirectorySeparatorChar + FilePathWithoutExtension + ".csv";
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

            Console.WriteLine("Extracting to " + FilePathWithoutExtension + ".csv");
            StreamWriter sw = File.AppendText(outFilePath);

            //Write header
            sw.Write("Translated");
            sw.Write('\t');
            sw.Write("Original");

            sw.Write('\n');

            int rI = 0;
            for (int k = 0; k < 7; k++)
            {
                SpreadsheetsResource.GetRequest request2 = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Get(SheetId);
                request2.Ranges = "A" + (k * 10000 + 2) + ":O" + ((k + 1) * 10000 + 2);
                request2.IncludeGridData = true;
                Spreadsheet sheet = request2.Execute();
                IList<GridData> grid = sheet.Sheets[0].Data;

                if (values != null && values.Count > 0)
                {
                    for (int gridI = 0; gridI < grid.Count; gridI++)
                    {
                        var gridData = grid[gridI];
                        //For each row
                        for (int rowI = 0; rowI < gridData.RowData.Count; rowI++)
                        {
                            var row = gridData.RowData[rowI];

                            SheetCellWithColor[] rowRaw = new SheetCellWithColor[row.Values.Count];
                            for (int i = 0; i < row.Values.Count; i++)
                            {
                                rowRaw[i] = new SheetCellWithColor(row.Values[i]);
                            }

                            if (rI >= values.Count)
                            {
								break;
							}

                            var rowValues = values[rI];
                            AssetEntry thisEntry = new AssetEntry(VariableDefinitions);
                            thisEntry.PopulateBySheetRow(rowValues);
                            thisEntry.AppendToCSV(sw, thisEntry, rowRaw, ref acceptableColors);

                            rI++;
                        }
                    }
                }

                Thread.Sleep(3000);

            }

            sw.Close();
        }

        
    }
}