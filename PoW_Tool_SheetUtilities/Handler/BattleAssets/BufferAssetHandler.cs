using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.BufferAssets
{
    public class BufferAsset : AssetEntry
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

        public BufferAsset(List<AssetVariableDefinition> variableDefinitions)
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
            sw.Write('\n');
        }
    }

    public class BufferAssetHandler : AssetHandler
    {
        public BufferAsset[] Variables;

        public BufferAssetHandler()
        {
            SheetId = "149AKUIe9dudiedmI99tGd0WxC-YmE30TgaZYEo2vNyg";
            AssetName = "Buffer";
            SheetRange = "A2:P";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "merge" + Path.DirectorySeparatorChar + "Buffer";
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
                    Name = "BuffName",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Tooltip",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Buff Category",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "PosNeg",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "N",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "O",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Script",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }

        public override void BuildGameDataFromSheet(string outRootPath)
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

            string scheduleRelativeFolderPath = Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "buffer";
            string bufferFolderPath = outRootPath + Path.DirectorySeparatorChar + scheduleRelativeFolderPath;

            //Getting all Sheet entries and dumping them into Talk.txt in right format
            Console.WriteLine("Extracting to " + FilePathWithoutExtension + OutputExtension + " and the buffer folder");
            StreamWriter mergeFileWriter = File.AppendText(mergeFilePath);

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    BufferAsset thisEntry = new BufferAsset(VariableDefinitions);
                    thisEntry.PopulateBySheetRow(row);

                    //Writing to merge file
                    thisEntry.AppendToFile(mergeFileWriter, thisEntry);

                    //Writting to .json file
                    string fileName = thisEntry.Variables[0].OriginalValue;
                    string filePath = bufferFolderPath + Path.DirectorySeparatorChar + fileName;
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

        public override void UpdateSheetFromGameFile(string inputFolder)
        {
            string bufferFolderPath = inputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "buffer";

            //Getting all current entries
            Dictionary<string, BufferAsset> entries = new Dictionary<string, BufferAsset>();
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
                    BufferAsset ae = new BufferAsset(VariableDefinitions);
                    ae.PopulateBySheetRow(row);
                    ae.Row = rowC;

                    entries[(string)row[0]] = ae;
                    ae.PopulateKnownTranslations();
                    rowC++;
                }
            }

            var updateRequests = new List<Request>();
            Console.WriteLine("Comparing with games version and calculating updates...");
            //Parse every battle schedule file
            string[] files = Directory.GetFiles(bufferFolderPath, "*.bytes");
            for (int i = 0; i < files.Length; i++)
            {
                //Reading file
                string bufferFilePathTmp = files[i];

                //Construction relative .json path
                int lastSplitterIndex = bufferFilePathTmp.LastIndexOf(Path.DirectorySeparatorChar);
                string bufferFilePath = bufferFilePathTmp.Substring(lastSplitterIndex + 1, bufferFilePathTmp.Length - lastSplitterIndex - 1);
                bufferFilePath = bufferFilePath.Replace(".bytes", ".json");

                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length + " => " + bufferFilePath;

                //Parsing
                string content = File.ReadAllText(bufferFilePathTmp);

                string[] data = content.Split('\t');
                data[data.Length - 1] = data[data.Length - 1].Replace("\r", "").Replace("\n", "");

                BufferAsset entry;
                if (entries.ContainsKey(bufferFilePath))
                {
                    //Compare and update
                    entry = entries[bufferFilePath];
                }
                else
                {
                    //New entry
                    entry = new BufferAsset(VariableDefinitions);
                }

                entry.PopulateByGameAssetRow(bufferFilePath, data);
                List<Request> updateReqs = entry.GetUpdateRequests();
                foreach (Request req in updateReqs)
                {
                    updateRequests.Add(req);
                }

                if (updateRequests.Count >= 100)
                {
                    HandleUpdateRequests(ref updateRequests);
                }
            }

            HandleUpdateRequests(ref updateRequests);
        }

        public override void ExportTranslatedLinesToCSV(string outPath, ref List<Color> acceptableColors)
        {
        }
    }
}