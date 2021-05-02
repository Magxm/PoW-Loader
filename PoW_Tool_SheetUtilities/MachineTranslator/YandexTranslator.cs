using System.IO;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
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

        public bool IsUseable()
        {
            return _Useable;
        }

        public string Translate(string original)
        {
            return _Translator.Translate("zh-en", original).Text;
        }
    }
}
