using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeepL;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    internal class DeepL : ITranslator
    {
        private Translator _Translator = null;
        private static string _kAuthFilePath = "DeepL_ApiKey.txt";

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

        public void AddTranslationRequest(TranslationRequest request)
        {
            _Requests.Add(request);
        }

        public async Task ForceTranslate()
        {
            foreach (var req in _Requests)
            {
                req.TranslatedText = (await _Translator.TranslateTextAsync(
                req.Text,
                LanguageCode.Chinese,
                LanguageCode.EnglishAmerican
             )).Text;
            }
            _Requests.Clear();
        }

        public bool IsUseable()
        {
            return _Useable;
        }
    }
}