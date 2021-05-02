using System;
using System.Collections.Generic;

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
            Translators.Add(new YandexTranslator());
            //Translators.Add(new GoogleTranslator());
            //Translators.Add(new BingTranslator());
        }

        public string Translate(string original)
        {
            /*
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
                        result = result.Replace(@"&#39;", @"'");
                        result = result.Replace(@"&quot;", @"""");
                        //Friend
                        result = result.Replace(@"{name_1}{friend_2}", @"{friend_2} {name_1}");
                        result = result.Replace(@"{name_1}{friend_1}", @"{friend_1} {name_1}");
                        result = result.Replace(@"{name_2}{friend_2}", @"{friend_2} {name_2}");
                        result = result.Replace(@"{name_2}{friend_1}", @"{friend_1} {name_2}");
                        //Address
                        result = result.Replace(@"{name_1}{address_1}", @"{address_1} {name_1}");
                        result = result.Replace(@"{name_1}{address_2}", @"{address_2} {name_1}");
                        result = result.Replace(@"{name_1}{address_3}", @"{address_3} {name_1}");
                        result = result.Replace(@"{name_1}{address_4}", @"{address_4} {name_1}");
                        result = result.Replace(@"{name_1}{address_5}", @"{address_5} {name_1}");

                        result = result.Replace(@"{name_2}{address_1}", @"{address_1} {name_2}");
                        result = result.Replace(@"{name_2}{address_2}", @"{address_2} {name_2}");
                        result = result.Replace(@"{name_2}{address_3}", @"{address_3} {name_2}");
                        result = result.Replace(@"{name_2}{address_4}", @"{address_4} {name_2}");
                        result = result.Replace(@"{name_2}{address_5}", @"{address_5} {name_2}");
                        //Junior
                        result = result.Replace(@"{name_1}{junior}", @"{junior} {name_1}");
                        result = result.Replace(@"{name_2}{junior}", @"{junior} {name_2}");
                        //Senior
                        result = result.Replace(@"{name_1}{senior}", @"{senior} {name_1}");
                        result = result.Replace(@"{name_2}{senior}", @"{senior} {name_2}");

                        //We got a valid translation, returning it...
                        return result;
                    }
                }
            }

            Console.WriteLine("[ERROR] No translator successfully translated the input " + original);
            return original;
            */
            return original;
        }
    }
}
