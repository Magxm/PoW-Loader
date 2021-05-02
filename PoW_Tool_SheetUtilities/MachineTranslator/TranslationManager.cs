using System;
using System.Collections.Generic;
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
            Translators.Add(new YandexTranslator());
            //Translators.Add(new GoogleTranslator());
            //Translators.Add(new BingTranslator());
        }

        public string Translate(string original, string[] beforeContext, string[] afterContext, string standardizedTerm)
        {
            if (string.IsNullOrEmpty(original) || original == "0")
            {
                return original;
            }

            foreach (var translator in Translators)
            {
                if (translator.IsUseable())
                {
                    TranslationRequest req = new TranslationRequest(original, beforeContext, afterContext, standardizedTerm);
                    translator.AddTranslationRequest(req);
                    Task t = translator.ForceTranslate();
                    t.Wait();

                    if (req.TranslatedText != null)
                    {
                        //Sanitizing req.TranslatedText
                        req.TranslatedText = req.TranslatedText.Replace(@"&#39;", @"'");
                        req.TranslatedText = req.TranslatedText.Replace(@"&quot;", @"""");
                        //Friend
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{friend_2}", @"{friend_2} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{friend_1}", @"{friend_1} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{friend_2}", @"{friend_2} {name_2}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{friend_1}", @"{friend_1} {name_2}");
                        //Address
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{address_1}", @"{address_1} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{address_2}", @"{address_2} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{address_3}", @"{address_3} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{address_4}", @"{address_4} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{address_5}", @"{address_5} {name_1}");

                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{address_1}", @"{address_1} {name_2}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{address_2}", @"{address_2} {name_2}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{address_3}", @"{address_3} {name_2}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{address_4}", @"{address_4} {name_2}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{address_5}", @"{address_5} {name_2}");
                        //Junior
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{junior}", @"{junior} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{junior}", @"{junior} {name_2}");
                        //Senior
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_1}{senior}", @"{senior} {name_1}");
                        req.TranslatedText = req.TranslatedText.Replace(@"{name_2}{senior}", @"{senior} {name_2}");

                        //We got a valid translation, returning it...
                        return req.TranslatedText;
                    }
                }
            }

            Console.WriteLine("[ERROR] No translator successfully translated the input " + original);
            return original;
        }
    }
}
