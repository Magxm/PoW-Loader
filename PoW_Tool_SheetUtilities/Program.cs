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

        private static void Main(string[] args)
        {
            Console.WriteLine("Do you want to update the spreadsheets after a game update or do you want to build the English Mod data from the spreadsheets?");
            Console.WriteLine("     1: Update after game update");
            Console.WriteLine("     2: Build English Mod data");

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