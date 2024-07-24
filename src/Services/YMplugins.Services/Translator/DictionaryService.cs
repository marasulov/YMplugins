using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace YMplugins.Services.Translator
{
    public class DictionaryService
    {
        private readonly string _jsonFileName;
        private Dictionary<string, string> _textFromJson;

        public DictionaryService(string jsonFileName)
        {
            _jsonFileName = jsonFileName;
            _textFromJson = LoadDictionaryFromJson();
        }

        public Dictionary<string, string> TextFromJson => _textFromJson;

        private Dictionary<string, string> LoadDictionaryFromJson()
        {
            if (File.Exists(_jsonFileName))
            {
                var jsonContent = File.ReadAllText(_jsonFileName);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
                return dictionary ?? new Dictionary<string, string>();
            }
            return new Dictionary<string, string>();
        }

        public void UpdateDictionaryAndSaveToJson(Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                if (!_textFromJson.ContainsKey(item.Key))
                {
                    _textFromJson.Add(item.Key, item.Value);
                }
            }

            string jsonToWrite = JsonConvert.SerializeObject(_textFromJson);
            File.WriteAllText(_jsonFileName, jsonToWrite);
        }
    }

}
