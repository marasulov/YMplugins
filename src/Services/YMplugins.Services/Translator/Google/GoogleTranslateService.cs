using Google.Cloud.Translation.V2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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

            //var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={targetLanguage}&dt=t&q={HttpUtility.UrlEncode(text)}";
            //object result = JsonConvert.DeserializeObject<List<object>>(new HttpClient().GetStringAsync(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", "ru", "en", (object)Uri.EscapeUriString(text))).Result)[0];
            string transtext = TranslateText(text, fromLanguage, targetLanguage);
            //string transtext = "TranslateText(text, fromLanguage, targetLanguage)";
            return transtext;
        }


        //public string TranslateText(string input, string sourceLanguage, string targetLanguage)
        //{
        //    object obj1 = JsonConvert.DeserializeObject<List<object>>(new HttpClient().GetStringAsync(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", "ru", "en", (object)Uri.EscapeUriString(input))).Result)[0];
        //    string str = "";
        //    // ISSUE: reference to a compiler-generated field
        //    if (Translator.\u003C\u003Eo__14.\u003C\u003Ep__0 == null)
        //    {
        //        // ISSUE: reference to a compiler-generated field
        //        Translator.\u003C\u003Eo__14.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(Translator)));
        //    }
        //    // ISSUE: reference to a compiler-generated field
        //    // ISSUE: reference to a compiler-generated field
        //    foreach (object obj2 in Translator.\u003C\u003Eo__14.\u003C\u003Ep__0.Target((CallSite)Translator.\u003C\u003Eo__14.\u003C\u003Ep__0, obj1))
        //    {
        //        IEnumerator enumerator = (obj2 as IEnumerable).GetEnumerator();
        //        enumerator.MoveNext();
        //        str += string.Format(" {0}", (object)Convert.ToString(enumerator.Current));
        //    }
        //    if (str.Length > 1)
        //        str = str.Substring(1);
        //    return str;
        //}

        public static string TranslateText(string input, string sourceLanguage, string targetLanguage)
        {
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLanguage}&tl={targetLanguage}&dt=t&q={Uri.EscapeUriString(input)}";

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

        //public string GetTranslate(string url)
        //{

        //var webClient = new WebClient
        //{
        //    Encoding = System.Text.Encoding.UTF8
        //};
        //var result = webClient.DownloadString(url);
        //object obj1 = JsonConvert.DeserializeObject<List<object>>(new HttpClient().GetStringAsync(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", (object)Translator.LanguageEnumToIdentifier(sourceLanguage), (object)Translator.LanguageEnumToIdentifier(targetLanguage), (object)Uri.EscapeUriString(input))).Result)[0];
        //if (result.Contains("windows-1251"))
        //{
        //    webClient.Encoding = System.Text.Encoding.GetEncoding("windows-1251");
        //    result = webClient.DownloadString(url);
        //}
        //else if (result.Contains("ISO-8859-2"))
        //{
        //    webClient.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-2");
        //    result = webClient.DownloadString(url);
        //}
        //try
        //{
        //    result = _targetLanguage == "en" ? GetParsedTranslatedText(result) : GetParsedTranslatedTextFromEn(result);
        //    return result;
        //}
        //catch
        //{
        //    return "Error";
        //}
        //}

        private static string GetParsedTranslatedText(string? text)
        {
            string pattern = @"null,null,3,null,null";

            string[] parts = Regex.Split(text, pattern);

            var newParts = parts.Take(parts.Length - 1).ToArray();
            var newList = new List<string>();
            for (int i = 1; i < newParts.Length; i++)
            {
                var newSplit = newParts[i].Split(new[] { "]," }, StringSplitOptions.None);
                newList.Add(newSplit[2]);

            }
            var newtr = new List<string>();
            foreach (var s in newList)
            {
                var ss = s.Split(new []{ "\",\"" }, StringSplitOptions.None );
                newtr.Add(ss[0].Substring(2));
            }

            newtr.Insert(0, newParts[0].Substring(4, newParts[0].IndexOf("\"", 4, StringComparison.Ordinal) - 4));

           return string.Join(" ", newtr);
        }

        private static string GetParsedTranslatedTextFromEn(string? text)
        {
            string pattern = @"null,null,3,null,null";

            string[] parts = Regex.Split(text, pattern);

            var newParts = parts.Take(parts.Length - 1).ToArray();

            var newList = new List<string>();
            for (int i = 1; i < newParts.Length; i++)
            {
                var newSplit = newParts[i].Split(new []{ "]," },StringSplitOptions.None);
                newList.Add(newSplit[newSplit.Length-1]);

            }

            newList.Insert(0, parts[0].Replace("[[", ""));
            
            var newtr = new List<string>();

            foreach (var s in newList)
            {
                var newS = s.Replace("[\"", "");
     
                var newT = newS.Split(new[] { "\",\"" }, StringSplitOptions.None)[0];
    
                newtr.Add(newT);
            }

            return string.Join(" ", newtr);
        }


        

        //private static string LanguageEnumToIdentifier(string language)
        //{
        //    string empty = string.Empty;
        //    Translator.EnsureInitialized();
        //    Translator._languageModeMap.TryGetValue(language, out empty);
        //    return empty;
        //}

        //private static void EnsureInitialized()
        //{
        //    if (Translator._languageModeMap != null)
        //        return;
        //    Translator._languageModeMap = new Dictionary<string, string>();
        //    Translator._languageModeMap.Add("Detect language", "auto");
        //    Translator._languageModeMap.Add("Afrikaans", "af");
        //    Translator._languageModeMap.Add("Albanian", "sq");
        //    Translator._languageModeMap.Add("Arabic", "ar");
        //    Translator._languageModeMap.Add("Armenian", "hy");
        //    Translator._languageModeMap.Add("Azerbaijani", "az");
        //    Translator._languageModeMap.Add("Basque", "eu");
        //    Translator._languageModeMap.Add("Belarusian", "be");
        //    Translator._languageModeMap.Add("Bengali", "bn");
        //    Translator._languageModeMap.Add("Bulgarian", "bg");
        //    Translator._languageModeMap.Add("Catalan", "ca");
        //    Translator._languageModeMap.Add("Chinese", "zh-CN");
        //    Translator._languageModeMap.Add("Croatian", "hr");
        //    Translator._languageModeMap.Add("Czech", "cs");
        //    Translator._languageModeMap.Add("Danish", "da");
        //    Translator._languageModeMap.Add("Dutch", "nl");
        //    Translator._languageModeMap.Add("English", "en");
        //    Translator._languageModeMap.Add("Esperanto", "eo");
        //    Translator._languageModeMap.Add("Estonian", "et");
        //    Translator._languageModeMap.Add("Filipino", "tl");
        //    Translator._languageModeMap.Add("Finnish", "fi");
        //    Translator._languageModeMap.Add("French", "fr");
        //    Translator._languageModeMap.Add("Galician", "gl");
        //    Translator._languageModeMap.Add("German", "de");
        //    Translator._languageModeMap.Add("Georgian", "ka");
        //    Translator._languageModeMap.Add("Greek", "el");
        //    Translator._languageModeMap.Add("Haitian Creole", "ht");
        //    Translator._languageModeMap.Add("Hebrew", "iw");
        //    Translator._languageModeMap.Add("Hindi", "hi");
        //    Translator._languageModeMap.Add("Hungarian", "hu");
        //    Translator._languageModeMap.Add("Icelandic", "is");
        //    Translator._languageModeMap.Add("Indonesian", "id");
        //    Translator._languageModeMap.Add("Irish", "ga");
        //    Translator._languageModeMap.Add("Italian", "it");
        //    Translator._languageModeMap.Add("Japanese", "ja");
        //    Translator._languageModeMap.Add("Korean", "ko");
        //    Translator._languageModeMap.Add("Lao", "lo");
        //    Translator._languageModeMap.Add("Latin", "la");
        //    Translator._languageModeMap.Add("Latvian", "lv");
        //    Translator._languageModeMap.Add("Lithuanian", "lt");
        //    Translator._languageModeMap.Add("Macedonian", "mk");
        //    Translator._languageModeMap.Add("Malay", "ms");
        //    Translator._languageModeMap.Add("Maltese", "mt");
        //    Translator._languageModeMap.Add("Norwegian", "no");
        //    Translator._languageModeMap.Add("Persian", "fa");
        //    Translator._languageModeMap.Add("Polish", "pl");
        //    Translator._languageModeMap.Add("Portuguese", "pt");
        //    Translator._languageModeMap.Add("Romanian", "ro");
        //    Translator._languageModeMap.Add("Russian", "ru");
        //    Translator._languageModeMap.Add("Serbian", "sr");
        //    Translator._languageModeMap.Add("Slovak", "sk");
        //    Translator._languageModeMap.Add("Slovenian", "sl");
        //    Translator._languageModeMap.Add("Spanish", "es");
        //    Translator._languageModeMap.Add("Swahili", "sw");
        //    Translator._languageModeMap.Add("Swedish", "sv");
        //    Translator._languageModeMap.Add("Tamil", "ta");
        //    Translator._languageModeMap.Add("Telugu", "te");
        //    Translator._languageModeMap.Add("Thai", "th");
        //    Translator._languageModeMap.Add("Turkish", "tr");
        //    Translator._languageModeMap.Add("Ukrainian", "uk");
        //    Translator._languageModeMap.Add("Urdu", "ur");
        //    Translator._languageModeMap.Add("Vietnamese", "vi");
        //    Translator._languageModeMap.Add("Welsh", "cy");
        //    Translator._languageModeMap.Add("Yiddish", "yi");
        //}
    }

    

    
}
