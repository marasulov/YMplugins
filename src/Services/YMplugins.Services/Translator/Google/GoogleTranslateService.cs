using Google.Cloud.Translation.V2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

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
        //    LanguageModeMap.TryGetValue(language, out empty);
        //    return empty;
        //}

        private static void EnsureInitialized()
        {
            if (LanguageModeMap != null)
                return;
            LanguageModeMap = new Dictionary<string, string>();
            LanguageModeMap.Add("Detect language", "auto");
            LanguageModeMap.Add("Afrikaans", "af");
            LanguageModeMap.Add("Albanian", "sq");
            LanguageModeMap.Add("Arabic", "ar");
            LanguageModeMap.Add("Armenian", "hy");
            LanguageModeMap.Add("Azerbaijani", "az");
            LanguageModeMap.Add("Basque", "eu");
            LanguageModeMap.Add("Belarusian", "be");
            LanguageModeMap.Add("Bengali", "bn");
            LanguageModeMap.Add("Bulgarian", "bg");
            LanguageModeMap.Add("Catalan", "ca");
            LanguageModeMap.Add("Chinese", "zh-CN");
            LanguageModeMap.Add("Croatian", "hr");
            LanguageModeMap.Add("Czech", "cs");
            LanguageModeMap.Add("Danish", "da");
            LanguageModeMap.Add("Dutch", "nl");
            LanguageModeMap.Add("English", "en");
            LanguageModeMap.Add("Esperanto", "eo");
            LanguageModeMap.Add("Estonian", "et");
            LanguageModeMap.Add("Filipino", "tl");
            LanguageModeMap.Add("Finnish", "fi");
            LanguageModeMap.Add("French", "fr");
            LanguageModeMap.Add("Galician", "gl");
            LanguageModeMap.Add("German", "de");
            LanguageModeMap.Add("Georgian", "ka");
            LanguageModeMap.Add("Greek", "el");
            LanguageModeMap.Add("Haitian Creole", "ht");
            LanguageModeMap.Add("Hebrew", "iw");
            LanguageModeMap.Add("Hindi", "hi");
            LanguageModeMap.Add("Hungarian", "hu");
            LanguageModeMap.Add("Icelandic", "is");
            LanguageModeMap.Add("Indonesian", "id");
            LanguageModeMap.Add("Irish", "ga");
            LanguageModeMap.Add("Italian", "it");
            LanguageModeMap.Add("Japanese", "ja");
            LanguageModeMap.Add("Korean", "ko");
            LanguageModeMap.Add("Lao", "lo");
            LanguageModeMap.Add("Latin", "la");
            LanguageModeMap.Add("Latvian", "lv");
            LanguageModeMap.Add("Lithuanian", "lt");
            LanguageModeMap.Add("Macedonian", "mk");
            LanguageModeMap.Add("Malay", "ms");
            LanguageModeMap.Add("Maltese", "mt");
            LanguageModeMap.Add("Norwegian", "no");
            LanguageModeMap.Add("Persian", "fa");
            LanguageModeMap.Add("Polish", "pl");
            LanguageModeMap.Add("Portuguese", "pt");
            LanguageModeMap.Add("Romanian", "ro");
            LanguageModeMap.Add("Russian", "ru");
            LanguageModeMap.Add("Serbian", "sr");
            LanguageModeMap.Add("Slovak", "sk");
            LanguageModeMap.Add("Slovenian", "sl");
            LanguageModeMap.Add("Spanish", "es");
            LanguageModeMap.Add("Swahili", "sw");
            LanguageModeMap.Add("Swedish", "sv");
            LanguageModeMap.Add("Tamil", "ta");
            LanguageModeMap.Add("Telugu", "te");
            LanguageModeMap.Add("Thai", "th");
            LanguageModeMap.Add("Turkish", "tr");
            LanguageModeMap.Add("Ukrainian", "uk");
            LanguageModeMap.Add("Urdu", "ur");
            LanguageModeMap.Add("Vietnamese", "vi");
            LanguageModeMap.Add("Welsh", "cy");
            LanguageModeMap.Add("Yiddish", "yi");
        }

        public static Dictionary<string, string> LanguageModeMap { get; set; }
    }

    

    
}
