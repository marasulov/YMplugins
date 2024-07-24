using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace YMplugins.Settings.Translator
{
    public class TranslatorSettings
    {
        public string DeeplApiKey { get; set; } = null;
        public string SourceLanguage { get; set; } = "English";
        public string TargetLanguage { get; set; } = "Russian";
        public List<string> IgnoreParameters { get; set; } = new();
        public List<string> IgnoreValues { get; set; } = new();
        public bool IsPaidPlan = false;
        public SortedList<string, string> Languages { get; set; } = DeeplLanguageCodes.LanguageCodes;
        private static string _jsonPath = string.Empty;

        /// <summary>
        /// Loads the settings from a JSON file.
        /// </summary>
        /// <returns>An instance of the Settings class with the loaded settings.</returns>
        //public static TranslatorSettings LoadFromJson()
        //{
        //    if (_jsonPath == string.Empty)
        //    {
        //        _jsonPath = GetJsonPath();
        //    }

        //    try
        //    {
        //        var json = File.ReadAllText(_jsonPath);
        //        return JsonConvert.DeserializeObject<Settings>(json);
        //    }
        //    catch
        //    {
        //        return new Settings();
        //    }
        //}

        public static TranslatorSettings LoadFromJson(string revitVersion)
        {
            if (_jsonPath == string.Empty)
            {
                _jsonPath = GetJsonPath(revitVersion);
            }

            try
            {
                var json = File.ReadAllText(_jsonPath);
                return JsonConvert.DeserializeObject<TranslatorSettings>(json);
            }
            catch
            {
                return new TranslatorSettings();
            }
        }

        //public void SaveToJson()
        //{
        //    var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        //    File.WriteAllText(_jsonPath, json);
        //}

        //public void SaveToJson()
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    };
        //    var json = JsonSerializer.Serialize(this, options);
        //    File.WriteAllText(_jsonPath, json);
        //}

        /// <summary>
        /// Gets the path of the JSON file with settings, corresponding to an active Revit version.
        /// </summary>
        /// <returns>A string representing the full path of the JSON file.</returns>
        internal static string GetJsonPath(string revitVersion)
        {
            //string revitVersion = RevitUtils.App.VersionNumber;
            string roamingAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string addinName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string settingsDirectory = Path.Combine(roamingAppDataPath, "Autodesk", "Revit", "Addins", revitVersion, addinName, "Settings");

            Directory.CreateDirectory(settingsDirectory);

            string jsonPath = Path.Combine(settingsDirectory, "settings.json");
            return jsonPath;
        }
    }
}
