using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities
{
    internal class KnownTranslationHandler
    {
        private static KnownTranslationHandler __instance = null;

        public static KnownTranslationHandler GetInstance()
        {
            if (__instance == null)
            {
                __instance = new KnownTranslationHandler();
            }

            return __instance;
        }

        private Dictionary<string, string> _KnownTranslations = new Dictionary<string, string>();

        public string? GetKnownTranslation(string original)
        {
            string outValue;
            if (!_KnownTranslations.TryGetValue(original, out outValue))
                outValue = null;

            return outValue;
        }

        public void AddTranslation(string original, string translation)
        {
            if (string.IsNullOrEmpty(original) || original == "" || original == "0")
                return;

            if (string.IsNullOrEmpty(translation) || translation == "" || translation == "0")
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Suspicious Translation added to KnownTranslationManager \"" + original + "\" => \"" + translation + "\"");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            _KnownTranslations[original] = translation;
        }
    }
}