using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    internal class TranslationManager
    {
        private readonly List<ITranslator> Translators;

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
            Translators = new List<ITranslator>
            {
                new DeepL()
               // new YandexTranslator()
                //new GoogleTranslator()
                //new BingTranslator()
            };
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
                    translator.AddTranslationRequest(ref req);
                    Task t = translator.ForceTranslate();
                    t.Wait();

                    if (req.TranslatedText != null)
                    {
                        //Sanitizing req.TranslatedText
                        req.TranslatedText = req.TranslatedText.Replace(@"&#39;", @"'");
                        req.TranslatedText = req.TranslatedText.Replace(@"&quot;", @"""");

                        //Case sensitive fix
                        req.TranslatedText = req.TranslatedText.Replace(@"{Name", @"{name");
                        req.TranslatedText = req.TranslatedText.Replace(@"{Friend", @"{friend");
                        req.TranslatedText = req.TranslatedText.Replace(@"{address", @"{address");
                        req.TranslatedText = req.TranslatedText.Replace(@"{Junior", @"{junior");
                        req.TranslatedText = req.TranslatedText.Replace(@"{Senior", @"{senior");

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

                        //Escape sequences
                        req.TranslatedText = req.TranslatedText.Replace("\\ \" ", "\\\"");
                        req.TranslatedText = req.TranslatedText.Replace("\\ \"", "\\\"");

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