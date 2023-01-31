using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.Json;

namespace ChiitransLite.translation
{
    internal class GoogleTranslator : ITranslator
    {
        public string Translate(string text)
        {
            const string targetLanguage = "en";
            text = text.Replace('「', '"').Replace('」', '"');
            string encodedText = Uri.EscapeDataString(text);
            string url = "https://translation.googleapis.com/language/translate/v2?key= "
                + $"{GetApiKey()}&target={targetLanguage}&q={encodedText}";

            string translation = "";
            using (var client = new WebClient())
            {
                try
                {
                    var response = client.DownloadString(url);
                    var jsonDoc = JsonDocument.Parse(response);
                    var translatedText = jsonDoc.RootElement
                        .GetProperty("data")
                        .GetProperty("translations")[0]
                        .GetProperty("translatedText")
                        .GetString();
                    translation = WebUtility.HtmlDecode(translatedText);
                    translation = translation.Replace("â”€", "─")
                        .Replace("â€¦",".");
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Failed to translate text. Error: " + ex.Message);
                }
            }
            return translation;
        }

        private string GetApiKey()
        {
            string apiKey = System.Environment.GetEnvironmentVariable(
                "GOOGLE_TRANSLATE_API_KEY", EnvironmentVariableTarget.User);
            if (apiKey != null)
                return apiKey;
            else
                MessageBox.Show("Please input a Google Translate API Key in the Options window.");
            return null;
        }
    }
}
