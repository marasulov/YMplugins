using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using YMplugins.Contracts;

namespace YMplugins.Services.SettingsReader
{
    public class SettingsReader : ISettingsReader
    {
        private readonly string _jsonFileName;

        public SettingsReader()
        {
            string assemblyFolder =
                "C:\\Users\\yusufzhon.marasulov\\source\\repos\\UzlePlugins\\UzlePlugins.Settings\\bin\\Debug\\net48\\";

#if !DEBUG
                assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

            _jsonFileName = Path.Combine(assemblyFolder, "Settings.json");
        }

      



        static string GetAssemblyDirectory(System.Reflection.Assembly assembly)
        {
            return System.IO.Path.GetDirectoryName(assembly.Location);
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
