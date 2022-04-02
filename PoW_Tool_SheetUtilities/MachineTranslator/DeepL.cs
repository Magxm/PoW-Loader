using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DeepL;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    internal class DeepL : ITranslator
    {
        private Translator _Translator = null;
        private static string _kAuthFilePath = "DeepL_ApiKey.txt";
        private DateTime _LastTranslatedTime = new DateTime(0);
        private static int _kMSBetweenTranslations = 100;

        private bool _Useable = false;

        public DeepL()
        {
            if (!File.Exists(_kAuthFilePath))
            {
                return;
            }

            var apiKey = File.ReadAllLines(_kAuthFilePath)[0];
            _Translator = new Translator(apiKey);
            _Useable = true;
        }

        private List<TranslationRequest> _Requests = new List<TranslationRequest>();

        public void AddTranslationRequest(ref TranslationRequest request)
        {
            _Requests.Add(request);
        }

        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");

        public static bool IsChinese(char c)
        {
            return cjkCharRegex.IsMatch(c.ToString());
        }

        public async Task ForceTranslate()
        {
            foreach (var req in _Requests)
            {
                if (req.Text.Length == 0)
                {
                    req.TranslatedText = "";
                    continue;
                }

                //If the text contains no chinese characters, we just return the text itself
                if (!req.Standardized.Any(z => IsChinese(z)))
                {
                    req.TranslatedText = req.Standardized;
                    continue;
                }

                string inpText = req.Standardized;
                inpText = inpText.Replace("⋯", "...");
                inpText = inpText.Replace("。", "");

                var currentTime = DateTime.Now;
                var msPassed = (currentTime - _LastTranslatedTime).Milliseconds;
                if (msPassed < _kMSBetweenTranslations)
                {
                    Thread.Sleep(_kMSBetweenTranslations - msPassed);
                }

                req.TranslatedText = (await _Translator.TranslateTextAsync(
                        req.Standardized,
                        LanguageCode.Chinese,
                        LanguageCode.EnglishAmerican,
                        new TextTranslateOptions { PreserveFormatting = true }
                    )
                ).Text;
                _LastTranslatedTime = DateTime.Now;
            }
            _Requests.Clear();
        }

        public bool IsUseable()
        {
            return _Useable;
        }
    }
}