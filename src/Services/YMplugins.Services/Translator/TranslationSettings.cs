using System;
using System.Collections.Generic;
using System.Text;

namespace YMplugins.Services.Translator
{
    public class TranslationSettings
    {
        public string SourceLanguage { get; set; } = "auto";
        public string TargetLanguage { get; set; } = "en";
    }
}
