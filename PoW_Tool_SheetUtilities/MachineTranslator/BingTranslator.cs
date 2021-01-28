using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    class BingTranslator : ITranslator
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
        internal class Alignment
        {
            public string Proj { get; set; }
        }

        internal class Translation
        {
            public string Text { get; set; }
            public TextResult Transliteration { get; set; }
            public string To { get; set; }
            public Alignment Alignment { get; set; }
            public SentenceLength SentLen { get; set; }
        }

        internal class SentenceLength
        {
            public int[] SrcSentLen { get; set; }
            public int[] TransSentLen { get; set; }
        }

        private bool Useable = false;
        public bool IsUseable()
        {
            return Useable;
        }

        private HttpClient httpClient = new HttpClient();
        private string authKey;
        private string endPoint;
        private string route = "/translate?api-version=3.0&from=zh-Hans&to=en";
        public BingTranslator()
        {
            //Reading Subscription key from file
            if (!File.Exists("AzureTranslateAuthKey.txt"))
            {
                Console.WriteLine("AzureTranslateAuthKey.txt not found! Not using Bing translator...");
                return;
            }

            if (!File.Exists("AzureTranslateEndpoint.txt"))
            {
                Console.WriteLine("AzureTranslateEndpoint.txt not found! Not using Bing translator...");
                return;
            }


            authKey = File.ReadAllText("AzureTranslateAuthKey.txt");
            endPoint = File.ReadAllText("AzureTranslateEndpoint.txt");

            Useable = true;
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
                var responseTask = httpClient.SendAsync(request).ConfigureAwait(false);
                HttpResponseMessage response = responseTask.GetAwaiter().GetResult();
                // Read response as a string.
                var resultTask = response.Content.ReadAsStringAsync();
                var result =resultTask.GetAwaiter().GetResult();
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
            Useable = false;
            return null;
        }
    }
}
