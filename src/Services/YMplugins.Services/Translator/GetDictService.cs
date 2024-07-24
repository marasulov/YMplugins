using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YMplugins.Contracts;

namespace YMplugins.Services.Translator
{
    public class GetDictService : IDictReader
    {
        private readonly ISettingsService _service;
        private Dictionary<string, string>? _dictDb;

        public GetDictService(ISettingsService service)
        {
            _service = service;
        }

        private void CheckDictFile(string jsonFile)
        {
            if (!File.Exists(jsonFile))
            {
                File.Create(jsonFile).Dispose();
                using (TextWriter tw = new StreamWriter(jsonFile))
                {
                    tw.WriteLine("{\"Общие данные\":\"Common data\",\n \"Спецификация\":\"Specification\"}");
                    tw.Close();
                }
            }

            string jsonFileToRead = File.ReadAllText(jsonFile);
            _dictDb = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFileToRead);
        }

        public Dictionary<string, string> GetDictDb()
        {
            CheckDictFile(_service.GetDictFilePath());
            return _dictDb;
        }

    }

    
}
