using PoW_Tool_SheetUtilities.Handler;
using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities
{
    internal class Program
    {
        private static void UpdateSpreadsheets()
        {
            SpreadsheetUpdater.UpdateSpreadsheetsFromGameFiles();
        }

        private static void CheckAssetFormat()
        {
            Console.WriteLine("Enter File Name:");
            string fileName = Console.ReadLine();
            var filePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar + "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + fileName + ".bytes";

            string line;
            int variableAmount = 0;

            string[] exampleEntry = null;
            System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split('\t');
                variableAmount = data.Length;
                if (exampleEntry == null)
                {
                    exampleEntry = new string[variableAmount];
                }
                for (int i = 0; i < variableAmount; i++)
                {
                    if (!string.IsNullOrEmpty(data[i]) && (string.IsNullOrEmpty(exampleEntry[i]) || exampleEntry[i] == "0" || data[i].Length > exampleEntry[i].Length))
                    {
                        exampleEntry[i] = data[i];
                    }
                }
            }

            if (exampleEntry != null)
            {
                Console.WriteLine("Variable count: " + variableAmount);
                Console.WriteLine("Example values (this is not an existing entry, but glued together so all values are as representative as possible): ");
                for (int i = 0; i < variableAmount; i++)
                {
                    Console.WriteLine("\t\t" + i + ": " + exampleEntry[i]);
                }
            }

            reader.Close();
        }

        private static void Export()
        {
            string workingDirectory = Environment.CurrentDirectory;
            Console.WriteLine("Where to export to?");
            Console.WriteLine("     1: /Output");
            Console.WriteLine("     2: ../../Mod/ModResources/EnglishTranslate/Config");

            string outputFolder;
            var input = Console.ReadKey().KeyChar;
            Console.WriteLine("\r          \n");
            switch (input)
            {
                case '1':
                    outputFolder = workingDirectory + Path.DirectorySeparatorChar + "Output";
                    break;

                case '2':
                    outputFolder = workingDirectory + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Mod/ModResources/EnglishTranslate/Config/";
                    break;

                default:
                    Console.WriteLine("ERROR: Invalid option!");
                    return;
            }

            SpreadsheetUpdater.ExportToMod(outputFolder);
        }

        private static void GetTranslationStats()
        {
            List<TranslationStatEntry> stats = new List<TranslationStatEntry>();
            //Proofread Stats
            TranslationStatEntry proofReadStats = new TranslationStatEntry("Proofread");
            proofReadStats.AcceptableColors.Add(AssetVariable.ProofReadColor);
            proofReadStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color() //Cyan
            {
                Alpha = 1.0f,
                Red = 0.0f / 255,
                Green = 0xFF / 255,
                Blue = 0xFF / 255,
            });
            proofReadStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0xC9 / 255,
                Green = 0xDA / 255,
                Blue = 0xF8 / 255,
            });
            proofReadStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0xA4 / 255,
                Green = 0xC2 / 255,
                Blue = 0xF4 / 255,
            });
            proofReadStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0x6D / 255,
                Green = 0x9E / 255,
                Blue = 0xEB / 255,
            });
            proofReadStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0x3C / 255,
                Green = 0x78 / 255,
                Blue = 0xD8 / 255,
            });

            stats.Add(proofReadStats);

            //Translated Stats
            TranslationStatEntry translatedStats = new TranslationStatEntry("Translated");
            translatedStats.AcceptableColors.Add(AssetVariable.TranslatedColor);
            translatedStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0x93 / 255,
                Green = 0xC4 / 255,
                Blue = 0x7D / 255,
            });
            translatedStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0xD9 / 255,
                Green = 0xEA / 255,
                Blue = 0xD3 / 255,
            });
            translatedStats.AcceptableColors.Add(new Google.Apis.Sheets.v4.Data.Color()
            {
                Alpha = 1.0f,
                Red = 0x34 / 255,
                Green = 0xA8 / 255,
                Blue = 0x53 / 255,
            });

            stats.Add(translatedStats);

            //Needs manual check Stats
            TranslationStatEntry manualCheckingReqStats = new TranslationStatEntry("Needs manual Checking (usually due to game update)");
            manualCheckingReqStats.AcceptableColors.Add(AssetVariable.NeedsCheckColor);
            stats.Add(manualCheckingReqStats);

            //MTL
            TranslationStatEntry mtlStats = new TranslationStatEntry("Machine Translated");
            mtlStats.AcceptableColors.Add(AssetVariable.MTLColor);
            stats.Add(mtlStats);

            //Others (Unknown)
            TranslationStatEntry unknownStats = new TranslationStatEntry("Marked in unknown cell color (Manually marked)");
            unknownStats.MatchAll = true;
            stats.Add(unknownStats);

            SpreadsheetUpdater.GetTranslationStats(ref stats);

            foreach (TranslationStatEntry statEntry in stats)
            {
                Console.WriteLine(statEntry.Name + " => Lines: " + statEntry.LineCount.ToString() + " (Words: " + statEntry.WordCount.ToString() + " )");
            }
        }

        private static void TestTranslation()
        {
            string PreContext = "凭你那点Cheap Tricks，也敢做我们的对手？哈哈哈！一边凉快去吧！";
            string Text = "Earth Dragon Sect没教过你们礼数？";
            string PostContext = "你⋯⋯什麽意思?";

            DeepL_Website translator = new DeepL_Website();

            while (true)
            {
                for (int i = 0; i < 150; ++i)
                {
                    var req = new TranslationRequest(Text, new string[] { PreContext }, new string[] { PostContext }, Text);
                    translator.AddTranslationRequest(req);
                }
                Task t = translator.ForceTranslate();
                t.Wait();
            }
        }

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            Console.WriteLine("Do you want to update the spreadsheets after a game update or do you want to build the English Mod data from the spreadsheets?");
            Console.WriteLine("     1: Update after game update");
            Console.WriteLine("     2: Build English Mod data");
            Console.WriteLine("     3: Get Asset Formats");
            Console.WriteLine("     4: Get Translation Stats");

            var input = Console.ReadKey().KeyChar;
            Console.WriteLine("\r          \n");
            switch (input)
            {
                case '1':
                    Console.WriteLine("Updating spreadsheets after game update...");
                    UpdateSpreadsheets();
                    break;

                case '2':
                    Console.WriteLine("Nuilding English Mod data from spreadsheets...");
                    Export();
                    break;

                case '3':
                    Console.WriteLine("Checking Asset Formats...");
                    CheckAssetFormat();
                    break;

                case '4':
                    Console.WriteLine("Getting Translation Stats...");
                    GetTranslationStats();
                    break;

                default:
                    Console.WriteLine("ERROR: Invalid option!");
                    break;
            }

            Console.WriteLine("Finished! Press any button to exit...");
            Console.ReadKey();
        }
    }
}