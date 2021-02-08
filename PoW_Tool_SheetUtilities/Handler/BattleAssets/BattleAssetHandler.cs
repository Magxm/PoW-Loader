﻿using System;
using System.IO;
using System.Collections.Generic;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace PoW_Tool_SheetUtilities.Handler.BattleAssets
{
    public class BattleAsset : AssetEntry
    {
        public void PopulateByGameAssetRow(string fileName, string[] row)
        {
            //Setting fileName
            Variables[0].SetNewOriginal(fileName);
            //Setting other variables
            int columnIndex = 0;
            for (int i = 1; i < VariableDefinitions.Count; i++) //Skipping filePath
                Variables[i].SetNewOriginal(row[columnIndex++]);
        }

        public BattleAsset(List<AssetVariableDefinition> variableDefinitions)
        {
            VariableDefinitions = variableDefinitions;
            Variables = new AssetVariable[VariableDefinitions.Count];
            for (int i = 0; i < VariableDefinitions.Count; i++)
            {
                AssetVariableDefinition varDef = VariableDefinitions[i];
                Variables[i] = new AssetVariable(varDef);
            }
        }

        public new void AppendToFile(StreamWriter sw, AssetEntry thisEntry)
        {
            for (int i = 1; i < VariableDefinitions.Count; i++)
            {
                if (i > 1)
                {
                    sw.Write('\t');
                }
                Variables[i].AppendToFile(sw);
            }

            sw.Write('\r');
        }
    }

    public class BattleAssetHandler : AssetHandler
    {
        public BattleAsset[] Variables;

        public BattleAssetHandler()
        {
            SheetId = "1Rva4xUTKRovzb8rXKEwC7gzSFjCJVuUwVaP1rymGNi0";
            AssetName = "Battle";
            SheetRange = "A2:M";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "merge" + Path.DirectorySeparatorChar + "Schedule";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "FileName",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "ID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Name",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "BattleSchedules",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "WinTip",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "LoseTip",
                    VariableType = AssetVariableType.Translate
                },
            };
        }

        public new void BuildGameDataFromSheet(string outRootPath)
        {
            string mergeFilePath = outRootPath + Path.DirectorySeparatorChar + FilePathWithoutExtension + OutputExtension;
            Console.WriteLine("Getting " + AssetName + " Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            //Clearing Asset File
            string outDirectory = Path.GetDirectoryName(mergeFilePath);
            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }
            //Resetting file
            File.WriteAllText(mergeFilePath, "");

            string scheduleRelativeFolderPath = Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "schedule";
            string scheduleFolderPath = outRootPath + Path.DirectorySeparatorChar + scheduleRelativeFolderPath;

            //Getting all Sheet entries and dumping them into Talk.txt in right format
            Console.WriteLine("Extracting to " + FilePathWithoutExtension + OutputExtension + " and the schedules folder");
            StreamWriter mergeFileWriter = File.AppendText(mergeFilePath);

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    BattleAsset thisEntry = new BattleAsset(VariableDefinitions);
                    thisEntry.PopulateBySheetRow(row);

                    //Writing to merge file
                    thisEntry.AppendToFile(mergeFileWriter, thisEntry);

                    //Writting to .json file
                    string fileName = thisEntry.Variables[0].OriginalValue;
                    string filePath = scheduleFolderPath + Path.DirectorySeparatorChar + fileName;
                    outDirectory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(outDirectory))
                    {
                        Directory.CreateDirectory(outDirectory);
                    }

                    File.WriteAllText(filePath, "");
                    StreamWriter scheduleFileWriter = File.AppendText(filePath);
                    thisEntry.AppendToFile(scheduleFileWriter, thisEntry);
                    scheduleFileWriter.Close();
                }
            }

            mergeFileWriter.Close();
        }

        public new void UpdateSheetFromGameFile(string inputFolder)
        {
            string scheduleFolderPath = inputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "schedule";

            //Getting all current entries
            Dictionary<string, BattleAsset> entries = new Dictionary<string, BattleAsset>();
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();

            Console.WriteLine("Getting " + AssetName + " Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    BattleAsset ae = new BattleAsset(VariableDefinitions);
                    ae.PopulateBySheetRow(row);
                    ae.Row = rowC;

                    entries[(string)row[0]] = ae;
                    rowC++;
                }
            }

            var updateRequests = new List<Request>();
            Console.WriteLine("Comparing with games version and calculating updates...");
            //Parse every battle schedule file
            string[] files = Directory.GetFiles(scheduleFolderPath, "*.bytes");
            for (int i = 0; i < files.Length; i++)
            {
                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length;

                //Reading file
                string scheduleFilePathTmp = files[i];
                System.IO.StreamReader reader = new System.IO.StreamReader(scheduleFilePathTmp);
                string line = reader.ReadLine();

                //Construction relative .json path
                int lastSplitterIndex = scheduleFilePathTmp.LastIndexOf(Path.DirectorySeparatorChar);
                string scheduleFilePath = scheduleFilePathTmp.Substring(lastSplitterIndex + 1, scheduleFilePathTmp.Length - lastSplitterIndex - 1);
                scheduleFilePath = scheduleFilePath.Replace(".bytes", ".json");

                //Parsing
                string[] data = line.Split('\t');
                BattleAsset entry;
                if (entries.ContainsKey(scheduleFilePath))
                {
                    //Compare and update
                    entry = entries[scheduleFilePath];
                }
                else
                {
                    //New entry
                    entry = new BattleAsset(VariableDefinitions);
                }

                entry.PopulateByGameAssetRow(scheduleFilePath, data);
                List<Request> updateReqs = entry.GetUpdateRequests();
                foreach (Request req in updateReqs)
                {
                    updateRequests.Add(req);
                }

                if (updateRequests.Count >= 500)
                {
                    HandleUpdateRequests(ref updateRequests);
                }
            }

            HandleUpdateRequests(ref updateRequests);
        }
    }
}