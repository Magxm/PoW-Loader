using System.IO;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Yandex.Translator;
    class YandexTranslator : ITranslator
    {
        string _APIKey;
        bool _Useable = false;
        IYandexTranslator _Translator;

        public YandexTranslator()
        {
            if (!File.Exists("YandexApiKey.txt"))
            {
                return;
            }

            _APIKey = File.ReadAllLines("YandexApiKey.txt")[0];
            _Useable = true;
            _Translator = Yandex.Translator(api => api.ApiKey(_APIKey).Format(ApiDataFormat.Json));
        }

        List<TranslationRequest> _Requests = new List<TranslationRequest>();
        public void AddTranslationRequest(TranslationRequest request)
        {
            _Requests.Add(request);
            Task t = ForceTranslate();
            t.Wait();
        }

        public async Task ForceTranslate()
        {
            foreach (var req in _Requests)
            {
                req.TranslatedText = _Translator.Translate("zh-en", req.Text).Text;
            }
            _Requests.Clear();
        }

        public bool IsUseable()
        {
            return _Useable;
        }
    }
}
