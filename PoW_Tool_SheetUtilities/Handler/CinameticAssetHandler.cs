using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PoW_Tool_SheetUtilities.Handler
{
    internal interface IPatchableNode
    {
        List<string> GetToTranslateList();
    }

    internal class PatchableNameChange : IPatchableNode
    {
        private string Surname;
        private string Name;
        public string WholeFunction;

        public PatchableNameChange(string surname, string name, string wholeFunction)
        {
            this.Surname = surname;
            this.Name = name;
            this.WholeFunction = wholeFunction;
            //Console.WriteLine(surname + " | " + name + " | " + wholeFunction);
        }

        public List<string> GetToTranslateList()
        {
            return new List<string>()
            {
                WholeFunction
            };
        }
    }

    internal class CinameticAssetHandler : AssetHandler
    {
        public CinameticAssetHandler()
        {
            SheetId = "1MENoj_ycnHNAjFAo64RJru4YY80Bhxo9Z2lqgUPCwkc";
            AssetName = "Cinametics";
            SheetRange = "A2:C";
            FilePathWithoutExtension = "";
            OutputExtension = ".json";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "Translate",
                    VariableType = AssetVariableType.Translate
                },
            };
        }

        static private string ChangeCharacterNameRegexPattern = @"(""{ \\""ChangCharactereName\\"" : \\"".*\\"", \\""(.*)\\"", \\""(.*)\\""})";
        static private Regex r = new Regex(ChangeCharacterNameRegexPattern);

        public List<IPatchableNode> GetPatchableNodes(string data)
        {
            List<IPatchableNode> patchableNodes = new List<IPatchableNode>();

            Match m = r.Match(data);
            while (m.Success)
            {
                //Console.WriteLine("----------");
                string whole = m.Groups[1].ToString();
                string surname = m.Groups[2].ToString();
                string name = m.Groups[3].ToString();
                patchableNodes.Add(new PatchableNameChange(surname, name, whole));

                m = m.NextMatch();
            }

            return patchableNodes;
        }

        public override void UpdateSheetFromGameFile(string outRootPath)
        {
            string cinameticFolder = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + "cinematic";
            Console.WriteLine("Getting Cinametic Patching Spreadsheet content");

            SpreadsheetsResource.ValuesResource.GetRequest request = GoogleSheetConnector.GetInstance().Service.Spreadsheets.Values.Get(SheetId, SheetRange);
            ValueRange response = request.Execute();
            List<IList<object>> values = (List<IList<object>>)response.Values;

            Dictionary<string, AssetEntry> entries = new Dictionary<string, AssetEntry>();

            int rowC = 1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    AssetEntry ae = new AssetEntry(VariableDefinitions);
                    ae.PopulateBySheetRow(row);
                    ae.Row = rowC;
                    string index = (string)row[1];
                    if (entries.ContainsKey(index))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Detected multiple entries with key " + index + " in row " + +(ae.Row + 1) + " ! Using last one...");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    entries[index] = ae;

                    rowC++;
                }
            }

            var updateRequests = new List<Request>();
            Console.WriteLine("Searching for important entries in cinametic files...");
            string[] files = Directory.GetFiles(cinameticFolder, "*.bytes");
            List<string> ToTranslate = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length;
                string filePath = files[i];
                string dataRaw = File.ReadAllText(filePath);

                List<IPatchableNode> patchables = GetPatchableNodes(dataRaw);

                foreach (var patchable in patchables)
                {
                    List<string> translateables = patchable.GetToTranslateList();
                    foreach (string translateable in translateables)
                    {
                        ToTranslate.Add(translateable);
                    }
                }
            }

            foreach (string s in ToTranslate)
            {
                AssetEntry entry;
                if (entries.ContainsKey(s))
                {
                    //Compare and update
                    entry = entries[s];
                }
                else
                {
                    //New entry
                    entry = new AssetEntry(VariableDefinitions);
                    entries[s] = entry;
                }

                entry.PopulateByGameAssetRow(new string[] { s });
                List<Request> updateReqs = entry.GetUpdateRequests();
                foreach (Request req in updateReqs)
                {
                    updateRequests.Add(req);
                }

                if (updateRequests.Count >= 2500)
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

            string cinameticFolder = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + "cinematic";
            string outputFileDirectory = outRootPath + Path.DirectorySeparatorChar + "cinematic";
            if (!Directory.Exists(outputFileDirectory))
            {
                Directory.CreateDirectory(outputFileDirectory);
            }

            Console.WriteLine("Patching Cinematic files from " + cinameticFolder + " into " + outputFileDirectory);
            string[] files = Directory.GetFiles(cinameticFolder, "*.bytes");

            for (int i = 0; i < files.Length; i++)
            {
                Console.Title = "[" + AssetName + "] Processing " + i + "/" + files.Length;
                string filePath = files[i];
                string data = File.ReadAllText(filePath);

                foreach (var patch in patches)
                {
                    data = data.Replace(patch.Variables[0].OriginalValue, patch.Variables[0].Translation);
                }

                string outFilePath = filePath.Replace(cinameticFolder, outputFileDirectory);
                outFilePath = outFilePath.Replace(".bytes", ".json");
                File.WriteAllText(outFilePath, data);
            }

            Console.WriteLine("Done!");
            Console.WriteLine("");
        }
    }
}
