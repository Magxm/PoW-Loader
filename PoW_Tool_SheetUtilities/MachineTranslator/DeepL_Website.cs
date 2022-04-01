/*
Heavily copied and adapted from https://github.com/bbepis/XUnity.AutoTranslator

MIT License

Copyright (c) 2018 Bepis

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Several changes and improvements were made to make it work with current DeepL versions and improve stability.
We are using the DeepL Website API. Sadly we cannot afford an actual developer API without taking donations and we don't plan to do so this early in the games development cycle.
*/

using SimpleJSON;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PoW_Tool_SheetUtilities.MachineTranslator
{
    public static class HttpResponseMessageExtensions
    {
        public static void ThrowIfBlocked(this HttpResponseMessage msg)
        {
            if (msg.StatusCode == (HttpStatusCode)429)
            {
                throw new Exception("Too many requests!");
            }
        }
    }

    [Serializable]
    public class BlockedException : Exception
    {
        public BlockedException()
        { }

        public BlockedException(string message) : base(message)
        {
        }

        public BlockedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BlockedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    internal class DeepL_Website : ITranslator
    {
        private class UntranslatedTextInfo
        {
            public string UntranslatedText { get; set; }

            public List<TranslationPart> TranslationParts { get; set; }
        }

        public class TranslationPart
        {
            public bool IsTranslatable { get; set; }

            public string Value { get; set; }
        }

        private const float MinDelaySeconds = 5;
        private const float MaxDelaySeconds = 10;

        private static readonly Regex NewlineSplitter = new Regex(@"([\s]*[\r\n]+[\s]*)");
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private static readonly string HttpsServicePointTemplateUrl = "https://www2.deepl.com/jsonrpc";
        private static readonly string HttpsTranslateUserSite = "https://www.deepl.com/translator";
        private static readonly string HttpsTranslateStateSetup = "https://www.deepl.com/PHP/backend/clientState.php?request_type=jsonrpc&il=EN";
        private static readonly Random RandomNumbers = new Random();

        private SemaphoreSlim _Sem;

        private HttpClient _Client;
        private HttpClientHandler _Handler;
        private bool _HasSetup = false;
        private int _TranslationCount = 0;
        private int _ResetAfter = RandomNumbers.Next(75, 125);
        private long _ID;

        public DeepL_Website()
        {
            ServicePointManager.SecurityProtocol |=
   SecurityProtocolType.Ssl3
   | SecurityProtocolType.Tls
   | SecurityProtocolType.Tls11
   | SecurityProtocolType.Tls12;

            _Sem = new SemaphoreSlim(1, 1);
        }

        public void Reset()
        {
            _HasSetup = false;
        }

        private void CreateClientAndHandler()
        {
            if (_Client != null)
            {
                _Client.Dispose();
            }

            _Handler = new HttpClientHandler();
            _Handler.CookieContainer = new CookieContainer();
            _Handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _Client = new HttpClient(_Handler, true);
            _Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");
        }

        public async Task EnsureSetupState()
        {
            if (!_HasSetup || _TranslationCount % _ResetAfter == 0)
            {
                _ResetAfter = RandomNumbers.Next(75, 125);
                _HasSetup = true;
                _ID = 10000 * (long)(10000 * RandomNumbers.NextDouble());

                CreateClientAndHandler();

                // Setup TKK and cookies
                await SetupState();
            }
        }

        public async Task SetupState()
        {
            _TranslationCount = 0;

            await RequestWebsite();
            await GetClientState();
        }

        private enum RequestType
        {
            Website,
            ClientState,
            Translation,
        }

        private void AddHeaders(HttpRequestMessage request, HttpContent content, RequestType requestType)
        {
            if (requestType == RequestType.Website)
            {
                request.Headers.TryAddWithoutValidation("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8,de;q=0.7");
                request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                //request.Headers.TryAddWithoutValidation("Referer", Referer);
                //request.Headers.TryAddWithoutValidation("Origin", Origin);
                request.Headers.TryAddWithoutValidation("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"");
                request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "none");
                request.Headers.TryAddWithoutValidation("Cache-Control", "max-age=0");
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            else if (requestType == RequestType.ClientState)
            {
                request.Headers.TryAddWithoutValidation("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8,de;q=0.7");
                request.Headers.TryAddWithoutValidation("Accept", "*/*");
                request.Headers.TryAddWithoutValidation("Referer", "https://www.deepl.com/translator");
                request.Headers.TryAddWithoutValidation("Origin", "https://www.deepl.com/");
                request.Headers.TryAddWithoutValidation("Sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"");
                request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                //request.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
                //request.Headers.TryAddWithoutValidation("Cache-Control", "max-age=0");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            else
            {
                //Translation request
                request.Headers.TryAddWithoutValidation("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8,de;q=0.7");
                request.Headers.TryAddWithoutValidation("Accept", "*/*");
                request.Headers.TryAddWithoutValidation("Referer", "https://www.deepl.com/translator");
                request.Headers.TryAddWithoutValidation("Origin", "https://www.deepl.com/");
                request.Headers.TryAddWithoutValidation("Sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"");
                request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "same-site");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");
                //request.Headers.TryAddWithoutValidation("Cache-Control", "max-age=0");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            request.Headers.TryAddWithoutValidation("DNT", "1");
        }

        public async Task RequestWebsite()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, HttpsTranslateUserSite);
            AddHeaders(request, null, RequestType.Website);

            using var response = await _Client.SendAsync(request);
            response.ThrowIfBlocked();
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync();
        }

        public async Task GetClientState()
        {
            _ID++;

            // construct json content
            var builder = new StringBuilder();
            builder.Append("{\"jsonrpc\":\"2.0\",\"method\":\"getClientState\",\"params\":{\"v\":\"20180814\",\"clientVars\":{\"showAppOnboarding\":true,\"uid\":\"79b731d6-4aae-4649-b536-2f546e5ce8ea\"}},\"id\":");
            builder.Append(_ID);
            builder.Append("}");
            var content = builder.ToString();
            var stringContent = new StringContent(content);

            using var request = new HttpRequestMessage(HttpMethod.Post, HttpsTranslateStateSetup);
            AddHeaders(request, stringContent, RequestType.ClientState);
            request.Content = stringContent;

            using var response = await _Client.SendAsync(request);
            response.ThrowIfBlocked();
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync();
        }

        public bool IsUseable()
        {
            return true;
        }

        private Mutex RequestListManipMutex = new Mutex();
        private List<TranslationRequest> _Requests = new List<TranslationRequest>();
        public const int _MaxTranslationsPerPacket = 10;

        public void AddTranslationRequest(TranslationRequest request)
        {
            RequestListManipMutex.WaitOne();
            _Requests.Add(request);
            RequestListManipMutex.ReleaseMutex();
            if (_Requests.Count >= _MaxTranslationsPerPacket)
            {
                ForceTranslate();
            }
        }

        private DateTime _LastTimeTranslated = DateTime.FromFileTimeUtc(0);
        private List<TranslationRequest> _CurrentlyTranslating;

        public async Task ForceTranslate()
        {
            double spanDiff = MaxDelaySeconds - MinDelaySeconds;
            double waitIntervalMs = MaxDelaySeconds + (RandomNumbers.NextDouble() * spanDiff) * 1000;
            double timeWeNeedToWait = waitIntervalMs - DateTime.Now.Subtract(_LastTimeTranslated).Milliseconds;
            if (timeWeNeedToWait > 0)
            {
                Thread.Sleep((int)Math.Floor(timeWeNeedToWait));
            }

            try
            {
                await _Sem.WaitAsync();
                RequestListManipMutex.WaitOne();
                _CurrentlyTranslating = _Requests;
                _Requests = new List<TranslationRequest>();
                RequestListManipMutex.ReleaseMutex();

                await EnsureSetupState();

                _TranslationCount++;
                _ID++;

                // construct json content
                long r = (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
                long n = 1;

                var builder = new StringBuilder();
                builder.Append("{\"jsonrpc\":\"2.0\",\"method\": \"LMT_handle_jobs\",\"params\":{\"jobs\":[");

                List<UntranslatedTextInfo> untranslatedTextInfos = new List<UntranslatedTextInfo>();
                foreach (var translationRequest in _CurrentlyTranslating)
                {
                    List<TranslationPart> parts = NewlineSplitter
                       .Split(translationRequest.Text)
                       .Select(x => new TranslationPart { Value = x, IsTranslatable = !NewlineSplitter.IsMatch(x) })
                       .ToList();

                    var usableParts = parts
                        .Where(x => x.IsTranslatable)
                        .Select(x => x.Value)
                        .ToArray();

                    for (int i = 0; i < usableParts.Length; i++)
                    {
                        var usablePart = usableParts[i];

                        builder.Append("{\"kind\":\"default\",\"preferred_num_beams\":1,\"raw_en_sentence\":\""); // yes.. "en" no matter what source language is used
                        builder.Append(JsonHelper.Escape(usablePart));

                        var addedContext = new HashSet<string>();
                        builder.Append("\",\"raw_en_context_before\":[");
                        bool addedAnyBefore = false;
                        foreach (var contextBefore in translationRequest.PreContext)
                        {
                            if (!addedContext.Contains(contextBefore))
                            {
                                builder.Append("\"");
                                builder.Append(JsonHelper.Escape(contextBefore));
                                builder.Append("\"");
                                builder.Append(",");
                                addedAnyBefore = true;
                            }
                        }
                        for (int j = 0; j < i; j++)
                        {
                            if (!addedContext.Contains(usableParts[j]))
                            {
                                builder.Append("\"");
                                builder.Append(JsonHelper.Escape(usableParts[j]));
                                builder.Append("\"");
                                builder.Append(",");
                                addedAnyBefore = true;
                            }
                        }
                        if (addedAnyBefore)
                        {
                            builder.Remove(builder.Length - 1, 1);
                        }

                        builder.Append("],\"raw_en_context_after\":[");
                        bool addedAnyAfter = false;
                        for (int j = i + 1; j < usableParts.Length; j++)
                        {
                            if (!addedContext.Contains(usableParts[j]))
                            {
                                builder.Append("\"");
                                builder.Append(JsonHelper.Escape(usableParts[j]));
                                builder.Append("\"");
                                builder.Append(",");
                                addedAnyAfter = true;
                            }
                        }
                        foreach (var contextAfter in translationRequest.PostContext)
                        {
                            if (!addedContext.Contains(contextAfter))
                            {
                                builder.Append("\"");
                                builder.Append(JsonHelper.Escape(contextAfter));
                                builder.Append("\"");
                                builder.Append(",");
                                addedAnyAfter = true;
                            }
                        }
                        if (addedAnyAfter)
                        {
                            builder.Remove(builder.Length - 1, 1);
                        }
                        //builder.Append("],\"quality\":\"fast\"},");
                        builder.Append("]},");

                        n += usablePart.Count(c => c == 'i');
                    }

                    untranslatedTextInfos.Add(new UntranslatedTextInfo { TranslationParts = parts, UntranslatedText = translationRequest.Text });
                }
                builder.Remove(builder.Length - 1, 1); // remove final ","

                var timestamp = r + (n - r % n);

                builder.Append("],\"lang\":{\"user_preferred_langs\":[\"");
                builder.Append("en".ToUpperInvariant());
                builder.Append("\",\"");
                builder.Append("zh".ToUpperInvariant());
                builder.Append("\"],\"source_lang_user_selected\":\"");
                builder.Append("zh".ToUpperInvariant());
                builder.Append("\",\"target_lang\":\"");
                builder.Append("en".ToUpperInvariant());
                builder.Append("\"},\"priority\":1,\"commonJobParams\":{\"formality\":null},\"timestamp\":");
                builder.Append(timestamp.ToString(CultureInfo.InvariantCulture));
                builder.Append("},\"id\":");
                builder.Append(_ID);
                builder.Append("}");
                var content = builder.ToString();

                var stringContent = new StringContent(content);

                using var request = new HttpRequestMessage(HttpMethod.Post, HttpsServicePointTemplateUrl);
                AddHeaders(request, stringContent, RequestType.Translation);
                request.Content = stringContent;

                // create request
                using var response = await _Client.SendAsync(request);
                response.ThrowIfBlocked();
                response.EnsureSuccessStatusCode();

                var str = await response.Content.ReadAsStringAsync();

                ExtractTranslation(str, untranslatedTextInfos);
                _LastTimeTranslated = DateTime.Now;
            }
            catch (BlockedException)
            {
                Reset();

                throw;
            }
            finally
            {
                _Sem.Release();
            }
        }

        private void ExtractTranslation(string data, List<UntranslatedTextInfo> untranslatedTextInfos)
        {
            var obj = JSON.Parse(data);

            var translations = obj["result"]["translations"].AsArray;

            int transIdx = 0;
            for (int i = 0; i < untranslatedTextInfos.Count; i++)
            {
                var parts = untranslatedTextInfos[i].TranslationParts;

                var fullTranslatedText = new StringBuilder();
                foreach (var part in parts)
                {
                    if (part.IsTranslatable)
                    {
                        var translation = translations[transIdx++];
                        var beams = translation["beams"].AsArray;
                        if (beams.Count > 0)
                        {
                            var beam = beams[0];
                            var sentence = beam["postprocessed_sentence"].ToString();
                            var translatedText = JsonHelper.Unescape(sentence.Substring(1, sentence.Length - 2));
                            fullTranslatedText.Append(translatedText);
                        }
                    }
                    else
                    {
                        fullTranslatedText.Append(part.Value);
                    }
                }

                var t = fullTranslatedText.ToString();
                if (string.IsNullOrWhiteSpace(t))
                {
                    throw new Exception("Found no valid translations in beam!");
                }

                Console.WriteLine(t);
                _CurrentlyTranslating[i].TranslatedText = t;
            }
        }
    }
}