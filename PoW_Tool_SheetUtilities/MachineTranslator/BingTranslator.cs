using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    class BingTranslator
    {
        internal class TranslationResult
        {
            public DetectedLanguage DetectedLanguage { get; set; }
            public TextResult SourceText { get; set; }
            public Translation[] Translations { get; set; }
        }

        internal class DetectedLanguage
        {
            public string Language { get; set; }
            public float Score { get; set; }
        }

        internal class TextResult
        {
            public string Text { get; set; }
            public string Script { get; set; }
        }

        internal class Translation
        {
            public string Text { get; set; }
            public TextResult Transliteration { get; set; }
            public string To { get; set; }
            public Alignment Alignment { get; set; }
            public SentenceLength SentLen { get; set; }
        }

        internal class Alignment
        {
            public string Proj { get; set; }
        }

        internal class SentenceLength
        {
            public int[] SrcSentLen { get; set; }
            public int[] TransSentLen { get; set; }
        }

        private static BingTranslator __Instance;
        public static BingTranslator GetInstance()
        {
            if (__Instance == null)
            {
                __Instance = new BingTranslator();
            }

            return __Instance;
        }


        public bool InitSuccessful = false;
        private HttpClient httpClient = new HttpClient();
        private string authKey;
        private string endPoint;
        private string route = "/translate?api-version=3.0&from=zh-Hans&to=en";
        public BingTranslator()
        {
            //Reading Subscription key from file
            if (!File.Exists("AzureTranslateAuthKey.txt"))
            {
                Console.WriteLine("AzureTranslateAuthKey.txt not found! Not using ML Translation...");
                return;
            }

            if (!File.Exists("AzureTranslateEndpoint.txt"))
            {
                Console.WriteLine("AzureTranslateEndpoint.txt not found! Not using ML Translation...");
                return;
            }


            authKey = File.ReadAllText("AzureTranslateAuthKey.txt");
            endPoint = File.ReadAllText("AzureTranslateEndpoint.txt");

            InitSuccessful = true;
        }

        public async Task<string> Translate(string original)
        {
            if (!InitSuccessful)
            {
                return null;
            }
            //Creating request json
            object[] body = new object[] { new { Text = original } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                //Building actual request
                // Set the method to Post.
                request.Method = HttpMethod.Post;
                // Construct the URI and add headers.
                request.RequestUri = new Uri(endPoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", authKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");

                // Send the request and get response.
                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    /*
                    // Print the detected input language and confidence score.
                    Console.WriteLine("Input: {0}\n Output: {1}", original, o.Translations[0].Text);
                    */

                    // Iterate over the results and print each translation.
                    return o.Translations[0].Text;
                }
            }

            Console.WriteLine("Request failed!");
            return null;
        }
    }
}
