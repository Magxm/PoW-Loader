using System;
using System.IO;
using System.Threading.Tasks;

using PoW_Tool_SheetUtilities.MachineTranslator;

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
            int exampleEntryUnemptyEntries = 0;
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
            Console.WriteLine("     2: ../../Mod/Mods/EnglishTranslate/Config");

            string outputFolder;
            var input = Console.ReadKey().KeyChar;
            Console.WriteLine("\r          \n");
            switch (input)
            {
                case '1':
                    outputFolder = workingDirectory + Path.DirectorySeparatorChar + "Output";
                    break;

                case '2':
                    outputFolder = workingDirectory + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Mod/Mods/EnglishTranslate/Config/";
                    break;

                default:
                    Console.WriteLine("ERROR: Invalid option!");
                    return;
            }

            SpreadsheetUpdater.ExportToMod(outputFolder);
        }

        private static void GetTranslationStats()
        {
            int proofReadCount = 0, translatedCount = 0, needsCheckCount = 0, MTLCount = 0, otherCount = 0;
            SpreadsheetUpdater.GetTranslationStats(ref proofReadCount, ref translatedCount, ref needsCheckCount, ref MTLCount, ref otherCount);

            Console.WriteLine("Stats:");
            Console.WriteLine("\tProofread Lines:\t\t" + proofReadCount);
            Console.WriteLine("\tTranslated Lines:\t\t" + translatedCount);
            Console.WriteLine("\tMarked as needing check(Usually due to game update) Lines:\t\t" + needsCheckCount);
            Console.WriteLine("\tMachine Translated Lines:\t\t" + MTLCount);
            Console.WriteLine("\tLines marked in unknown cell color (Manually marked):\t\t" + otherCount);
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
                    Console.WriteLine("Selected updating spreadsheets after game update!");
                    UpdateSpreadsheets();
                    break;

                case '2':
                    Console.WriteLine("Selected building English Mod data from spreadsheets");
                    Export();
                    break;

                case '3':
                    Console.WriteLine("Checking Asset Formats");
                    CheckAssetFormat();
                    break;

                case '4':
                    Console.WriteLine("Get Translation Stats");
                    GetTranslationStats();
                    break;

                default:
                    Console.WriteLine("ERROR: Invalid option!");
                    break;
            }
            /*
            string test = "段红儿三阶-时间达标通告(触发於第一年四月中旬-望庐诀课后)(Movie中判定好感阶级大于一才触发对话)";
            Console.WriteLine("Original: " + test);
            BingTranslator bt = BingTranslator.GetInstance();
            Task<string> t = bt.Translate(test);
            t.Wait();

            Console.WriteLine("Translated: " + t.Result);
            */
            Console.WriteLine("Finished! Press any button to exit...");
            Console.ReadKey();
        }
    }
}