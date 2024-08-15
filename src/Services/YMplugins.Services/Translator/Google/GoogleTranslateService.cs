using Google.Cloud.Translation.V2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace YMplugins.Services.Translator.Google
{
    public class GoogleTranslateService : ITextTranslator
    {
        private readonly TranslationClient _client;
        private string _fromLanguage;
        private string _targetLanguage;

        public GoogleTranslateService()
        {
            //_client = TranslationClient.CreateFromApiKey(apiKey);
        }

        public string Translate(string text, string fromLanguage = "auto", string targetLanguage = "en")
        {
            _fromLanguage = fromLanguage;
            _targetLanguage = targetLanguage;
            try
            {
                string transtext = TranslateText(text, fromLanguage, targetLanguage);
                //string transtext = "TranslateText(text, fromLanguage, targetLanguage)";
                return transtext;
            }
            catch (Exception e)
            {

                return $"{e.Message} - {e.StackTrace}";
            }

        }

        public static string TranslateText(string input, string sourceLanguage, string targetLanguage)
        {
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLanguage}&tl={targetLanguage}&dt=t&q={Uri.EscapeUriString(input)}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //object result = JsonConvert.DeserializeObject<List<object>>(new HttpClient().GetStringAsync(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", "ru", "en", (object)Uri.EscapeUriString(input))).Result)[0];
                    //string result = await client.GetStringAsync(url);

                    string result = client.GetStringAsync(url).GetAwaiter().GetResult();

                    // Разбираем ответ
                    JArray jsonArray = JArray.Parse(result);
                    List<string> translations = new List<string>();

                    foreach (var item in jsonArray[0])
                    {
                        translations.Add(item[0].ToString());
                    }

                    // Объединяем все переводы в одну строку и возвращаем
                    return string.Join(" ", translations);
                }
            }
            catch (Exception e)
            {
                
                return $"{e.Message} - {e.StackTrace}";
            }
           
        }

        //private static string LanguageEnumToIdentifier(string language)
        //{
        //    string empty = string.Empty;
        //    Translator.EnsureInitialized();
        //    LanguageModeMap.TryGetValue(language, out empty);
        //    return empty;
        //}

    }

}
