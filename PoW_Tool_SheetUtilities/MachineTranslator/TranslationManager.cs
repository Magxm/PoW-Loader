using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    internal class TranslationManager
    {
        private List<ITranslator> Translators;

        private static TranslationManager __Instance;

        public static TranslationManager GetInstance()
        {
            if (__Instance == null)
            {
                __Instance = new TranslationManager();
            }

            return __Instance;
        }

        private TranslationManager()
        {
            Translators = new List<ITranslator>();
            Translators.Add(new GoogleTranslator());
            Translators.Add(new BingTranslator());
        }

        public string Translate(string original)
        {
            if (string.IsNullOrEmpty(original) || original == "0")
            {
                return original;
            }

            foreach (var translator in Translators)
            {
                if (translator.IsUseable())
                {
                    string result = translator.Translate(original);
                    if (result != null)
                    {
                        //Sanitizing result
                        result = result.Replace("&#39;", "'");

                        //We got a valid translation, returning it...
                        return result;
                    }
                }
            }

            Console.WriteLine("[ERROR] No translator successfully translated the input " + original);
            return original;
        }
    }
}