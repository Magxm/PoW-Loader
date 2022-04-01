using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoW_Tool_SheetUtilities.Handler.BattleAssets
{
    public class BattleAssetHandler : AssetHandler
    {
        private static string BattleResultChangeWinTipRegexPattern = "\"{ \\\"BattleResultChangeWinTip\\\" : \\\".*\\\"} \"";
        private static Regex _BattleResultChangeWinTipRegex = new Regex(BattleResultChangeWinTipRegexPattern);

        private static string BattleResultChangeLoseTipRegexPattern = "\"{ \\\"BattleResultChangeLoseTip\\\" : \\\".*\\\"} \"";
        private static Regex BattleResultChangeLoseTipRegex = new Regex(BattleResultChangeLoseTipRegexPattern);

        private static string BattleResultAddSecondaryGoalRegexPattern = "\"{ \\\"BattleResultAddSecondaryGoal\\\" : \\\".*\\\"} \"";
        private static Regex BattleResultAddSecondaryGoalRegex = new Regex(BattleResultAddSecondaryGoalRegexPattern);

        public BattleAssetHandler()
        {
            SheetId = "1Rva4xUTKRovzb8rXKEwC7gzSFjCJVuUwVaP1rymGNi0";
            AssetName = "Battle";
            SheetRange = "A2:D";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "merge" + Path.DirectorySeparatorChar + "Schedule";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "Translate",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Type",
                    VariableType = AssetVariableType.NoTranslate
                }
            };
        }

        public class ToPatchInstance
        {
            public string ToPatch;
            public string Type;

            public ToPatchInstance(string v, string t)
            {
                ToPatch = v;
                Type = t;
            }

            public int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 23 + (ToPatch == null ? 0 : ToPatch.GetHashCode());
                    hash = hash * 23 + (Type == null ? 0 : Type.GetHashCode());
                    return hash;
                }
            }
        }

        private class ToPatchInstanceComparer : IEqualityComparer<ToPatchInstance>
        {
            public bool Equals(ToPatchInstance a, ToPatchInstance b)
            {
                return a.ToPatch == b.ToPatch && a.Type == b.Type;
            }

            public int GetHashCode(ToPatchInstance obj)
            {
                return obj.GetHashCode();
            }
        }

        public List<ToPatchInstance> GetPatchableInstances(string data)
        {
            List<ToPatchInstance> patchables = new List<ToPatchInstance>();

            Match m = _BattleResultChangeWinTipRegex.Match(data);
            while (m.Success)
            {
                //Console.WriteLine("----------");
                patchables.Add(new ToPatchInstance(m.Value, "WinTip"));

                m = m.NextMatch();
            }

            m = BattleResultChangeLoseTipRegex.Match(data);
            while (m.Success)
            {
                //Console.WriteLine("----------");
                patchables.Add(new ToPatchInstance(m.Value, "LoseTip"));

                m = m.NextMatch();
            }

            m = BattleResultAddSecondaryGoalRegex.Match(data);
            while (m.Success)
            {
                //Console.WriteLine("----------");
                patchables.Add(new ToPatchInstance(m.Value, "AddSecondaryGoal"));

                m = m.NextMatch();
            }

            return patchables;
        }

        public override void UpdateSheetFromGameFile(string inputFolder)
        {
            string scheduleFolderPath = inputFolder + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "schedule";

            //Getting all current entries
            Dictionary<int, AssetEntry> entries = new Dictionary<int, AssetEntry>();
            GoogleSheetConnector gsc = GoogleSheetConnector.GetInstance();

            Console.WriteLine("Getting " + AssetName + " Patching Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = gsc.Service.Spreadsheets.Values.Get(SheetId, SheetRange);
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
                    string text = (string)row[1];
                    string type = (string)row[3];
                    int hash = new ToPatchInstance(text, type).GetHashCode();

                    if (entries.ContainsKey(hash))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Detected multiple entries for text " + text + " in row " + (ae.Row + 1) + " ! Using last one...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    entries[hash] = ae;

                    rowC++;
                }
            }

            var updateRequests = new List<Request>();
            Console.WriteLine("Searching for important entries in battle schedule files...");
            //Parse every battle schedule file
            string[] files = Directory.GetFiles(scheduleFolderPath, "*.bytes");

            List<ToPatchInstance> toPatch = new List<ToPatchInstance>();
            for (int i = 0; i < files.Length; i++)
            {
                string scheduleFilePathTmp = files[i];

                //Construction relative .json path
                int lastSplitterIndex = scheduleFilePathTmp.LastIndexOf(Path.DirectorySeparatorChar);
                string scheduleFilePath = scheduleFilePathTmp.Substring(lastSplitterIndex + 1, scheduleFilePathTmp.Length - lastSplitterIndex - 1);
                scheduleFilePath = scheduleFilePath.Replace(".bytes", ".json");

                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length + " => " + scheduleFilePath;

                //Reading file
                string content = File.ReadAllText(scheduleFilePathTmp);

                //Creating JSON object
                try
                {
                    JObject obj = JObject.Parse(content);

                    toPatch.Add(new ToPatchInstance((string)obj["WinTip"], "WinTip"));
                    toPatch.Add(new ToPatchInstance((string)obj["LoseTip"], "LoseTip"));

                    List<ToPatchInstance> toPatchRegexResult = GetPatchableInstances(content);
                    foreach (ToPatchInstance v in toPatchRegexResult)
                    {
                        toPatch.Add(v);
                    }
                }
                catch (Exception e)
                {
                    ///Might be outdated file/wrong format
                    Console.WriteLine("Outdated or otherwise broken battle schedule file " + scheduleFilePath);
                }
            }

            toPatch = toPatch.Distinct(new ToPatchInstanceComparer()).ToList();

            Console.WriteLine("Patching...");
            for (int i = 0; i < toPatch.Count; i++)
            {
                var e = toPatch[i];
                if (e.ToPatch == null || e.ToPatch.Length == 0)
                {
                    continue;
                }

                Console.Title = "[" + AssetName + "] Patching " + i + "/" + toPatch.Count + " => " + e.ToPatch;
                AssetEntry entry;
                int hash = e.GetHashCode();
                if (entries.ContainsKey(hash))
                {
                    //Compare and update
                    entry = entries[hash];
                }
                else
                {
                    //New entry
                    entry = new AssetEntry(VariableDefinitions);
                    entries[hash] = entry;
                }

                entry.PopulateByGameAssetRow(new string[] { e.ToPatch, e.Type });
                List<Request> updateReqs = entry.GetUpdateRequests();
                foreach (Request req in updateReqs)
                {
                    updateRequests.Add(req);
                }

                if (updateRequests.Count >= 10)
                {
                    HandleUpdateRequests(ref updateRequests);
                }
            }
            HandleUpdateRequests(ref updateRequests);
        }

        public override void BuildGameDataFromSheet(string outRootPath)
        {
            Console.WriteLine("Getting " + AssetName + " Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            List<AssetEntry> patches = new List<AssetEntry>();
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AssetEntry thisEntry = new AssetEntry(VariableDefinitions);
                    thisEntry.PopulateBySheetRow(row);
                    patches.Add(thisEntry);
                }
            }
            //Clearing Asset File
            string scheduleRelativeFolderPath = Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "schedule";
            string scheduleFolderPath = outRootPath + Path.DirectorySeparatorChar + scheduleRelativeFolderPath;
            string mergeFileInputPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + FilePathWithoutExtension + OutputExtension;
            string mergeFileOutPath = outRootPath + Path.DirectorySeparatorChar + FilePathWithoutExtension + OutputExtension;
            string outDirectory = Path.GetDirectoryName(scheduleFolderPath);
            if (!Directory.Exists(scheduleFolderPath))
            {
                Directory.CreateDirectory(scheduleFolderPath);
            }

            //Resetting merge file
            File.WriteAllText(mergeFileOutPath, "");

            //We patch every input file
            string inputFolderPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "battle" + Path.DirectorySeparatorChar + "schedule";
            Console.WriteLine("Patching battle schedule files from " + inputFolderPath + " into " + scheduleFolderPath);
            string[] files = Directory.GetFiles(scheduleFolderPath, "*.bytes");
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length + " => " + filePath;
                string data = File.ReadAllText(filePath);

                foreach (var patch in patches)
                {
                    data = data.Replace(patch.Variables[0].OriginalValue, patch.Variables[0].Translation);
                }

                string outFilePath = filePath.Replace(inputFolderPath, outDirectory);
                outFilePath = outFilePath.Replace(".bytes", ".json");
                File.WriteAllText(outFilePath, data);
            }
            Console.WriteLine("Patching merged battle schedule file!");
            Console.Title = "[" + AssetName + "] Processing merged file...";
            string content = File.ReadAllText(mergeFileInputPath);
            foreach (var patch in patches)
            {
                content = content.Replace(patch.Variables[0].OriginalValue, patch.Variables[0].Translation);
            }
            File.WriteAllText(content, "");

            Console.WriteLine("Done!");
            Console.WriteLine("");
        }
    }
}