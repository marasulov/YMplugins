using System.IO;
using Newtonsoft.Json;
using YMplugins.Contracts;

namespace YMplugins.Services.SettingsReader
{
    public class SettingsService : ISettingsService
    {
        private string _settingsFileName;
        private readonly Config _config;

        public SettingsService()
        {

            string assemblyFolder =
                "C:\\Users\\yusufzhon.marasulov\\source\\repos\\YMplugins\\src\\Services\\YMplugins.Settings\\";

#if !DEBUG
                assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

            _settingsFileName = Path.Combine(assemblyFolder, "Settings.json");
        }

        public Config LoadSettings()
        {
            var config = new Config();
            if (File.Exists(_settingsFileName))
            {
                var jsonContent = File.ReadAllText(_settingsFileName);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    config = JsonConvert.DeserializeObject<Config>(jsonContent);
                }

            }

            return config;
        }

        /// <summary>
        /// Get dict file path from config
        /// </summary>
        /// <returns>dict file path</returns>
        public string? GetDictFilePath()
        {
            if (_config == null || string.IsNullOrEmpty(_config.DictFilePath)) return null;

            return _config.DictFilePath;
        }

    }

    public class Config
    {
        public string? DictFilePath { get; set; }
    }
}
