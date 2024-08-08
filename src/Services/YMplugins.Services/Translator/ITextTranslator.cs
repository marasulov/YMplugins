using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YMplugins.Services.Translator
{
    public interface ITextTranslator
    {
        string Translate(string text, string sourceLang = "auto", string targetLanguage = "en");
    }
}
