using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace ChatApp.Models
{
    public class Transslate
    {
        private const string Key = "d22838a5d64a45bead6314e8d56fbab3";

        private static readonly HttpClient client = new HttpClient
        {
            DefaultRequestHeaders = { { "Ocp-Apim-Subscription-Key", Key } }
        };

        public static async Task<string> Translate(string text, string language)
        {
           
            //var uri = "https://api.microsofttranslator.com/V2/Http.svc/translate?" + $"to={language}&text={encodedText}";
            var uri = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";  // + $"to={language}&text={encodedText}";
            TranslateData tdata = new TranslateData {
                Text = text
            };
            object[] body = new object[] { tdata };
            var requestBody = JsonConvert.SerializeObject(body);
            var result = "";

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                // Construct the URI and add headers.
                request.RequestUri = new Uri(uri + $"&to={language}" );
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");


                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                    // Iterate over the deserialized results.
                    foreach (TranslationResult o in deserializedOutput)
                    {
                        // Print the detected input language and confidence score.
                        Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                        // Iterate over the results and print each translation.
                        foreach (Translation t in o.Translations)
                        {
                            Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                            result = t.Text;
                        }
                    }
                }
            }

            
           // var result = await client.GetStreamAsync(uri);
            return result;


        }
       

        public class TranslateData
        {
            public string Text;

        }

        public class TranslationResult
        {
            public DetectedLanguage DetectedLanguage { get; set; }
            public TextResult SourceText { get; set; }
            public Translation[] Translations { get; set; }
        }

        public class DetectedLanguage
        {
            public string Language { get; set; }
            public float Score { get; set; }
        }

        public class TextResult
        {
            public string Text { get; set; }
            public string Script { get; set; }
        }

        public class Translation
        {
            public string Text { get; set; }
            public TextResult Transliteration { get; set; }
            public string To { get; set; }
            public Alignment Alignment { get; set; }
            public SentenceLength SentLen { get; set; }
        }
        public class Alignment
        {
            public string Proj { get; set; }
        }

        public class SentenceLength
        {
            public int[] SrcSentLen { get; set; }
            public int[] TransSentLen { get; set; }
        }
    }
}
