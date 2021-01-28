using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    class GoogleTranslator : ITranslator
    {
        bool Useable = false;
        TranslationServiceClient translationServiceClient;

        public GoogleTranslator()
        {
            Console.WriteLine("Connecting to Google Translator API...");
            var translationServiceSettings = new TranslationServiceSettings();

            translationServiceClient = new TranslationServiceClientBuilder
            {
                CredentialsPath = "credentials_service.json"
            }.Build();

            Useable = true;
            Console.WriteLine("Connected!");
        }

        public bool IsUseable()
        {
            return Useable;
        }

        public string Translate(string original)
        {
            if (!IsUseable())
            {
                return null;
            }

            if (string.IsNullOrEmpty(original))
            {
                return "";
            }

            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    original,
                },
                SourceLanguageCode = "zh",
                TargetLanguageCode = "en",
                ParentAsLocationName = new LocationName("pow-english-mod", "global"),
            };

            TranslateTextResponse response = translationServiceClient.TranslateText(request);
            foreach (Translation translation in response.Translations)
            {
                return translation.TranslatedText;
            }

            Useable = false;
            return null;
        }
    }
}
