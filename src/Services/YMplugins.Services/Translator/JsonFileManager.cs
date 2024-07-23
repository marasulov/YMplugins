using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;

namespace YMplugins.Services.Translator
{
    public class JsonFileManager
    {
        
        public JsonFileManager(ISettingsReader reader)
        {

        }

        public void CheckFile(string jsonFile)
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
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFileToRead);
        }
    }

    
}
