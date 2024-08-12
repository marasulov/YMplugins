using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Linq;
using YMplugins.Services.Translator;
using static System.Net.Mime.MediaTypeNames;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace YMplugins.Models.Acad2022.Services
{
    public class TextProcessor
    {
        private readonly DictionaryService _dictionaryService;
        private readonly ITextTranslator _textTranslator;
        private readonly Dictionary<string, string> _dict;
        private string _fromLanguage = "ru";
        private string _targetLanguage = "en";
        private readonly TranslationSettings _translationSettings;

        public TextProcessor(DictionaryService dictionaryService, ITextTranslator textTranslator, TranslationSettings translationSettings)
        {
            _dictionaryService = dictionaryService;
            _textTranslator = textTranslator;
            _dict = new Dictionary<string, string>();
            _translationSettings = translationSettings;
        }

        public void ProcessTexts(TranslationSettings translationSettings)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database destdb = doc.Database;
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (var acTrans = Active.Database.TransactionManager.StartTransaction())
                {
                    var acSSPrompt = Active.Editor.GetSelection(CreateTextAndTableFilter());
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        var acSSet = acSSPrompt.Value;
                        var selectedIds = acSSet.GetObjectIds();
                        string regex = "[^A-Za-z0-9]+";
                        string separator = "\n";
                        var preserveOriginalText = false;
                        if (selectedIds.Length > 0)
                        {
                            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("\nSave native text?");
                            pKeyOpts.Keywords.Add("Yes");
                            pKeyOpts.Keywords.Add("No");

                            PromptResult pKeyRes = Active.Editor.GetKeywords(pKeyOpts);

                            if (pKeyRes.Status == PromptStatus.OK && pKeyRes.StringResult == "Yes")
                            {
                                preserveOriginalText = true;
                                PromptStringOptions pStrOpts = new PromptStringOptions(
                                    "\nВведите сепаратор между оригиналом и переводом (нажмите Enter для принятия сепаратора \"-\" для text и /n для mtext):");
                                PromptResult pStrRes = Active.Editor.GetString(pStrOpts);

                                if (pStrRes.Status == PromptStatus.OK)
                                {
                                    separator = pStrRes.StringResult;
                                    if (string.IsNullOrEmpty(separator))
                                    {
                                        separator = "\n";
                                    }
                                }
                            }

                            foreach (ObjectId acSSObj in selectedIds)
                            {
                                if (acSSObj != null)
                                {
                                    var acEnt = acTrans.GetObject(acSSObj, OpenMode.ForWrite) as Entity;

                                    switch (acEnt)
                                    {
                                        case MText mText:
                                            ProcessMText(mText, separator, preserveOriginalText);
                                            break;
                                        case DBText dbText:
                                            ProcessDbText(dbText, separator, preserveOriginalText);
                                            break;
                                        case Table table:
                                            ProcessTable(table, separator, preserveOriginalText);
                                            break;
                                        case Leader leader:
                                            ProcessLeader(leader, separator);
                                            break;
                                        case MLeader mLeader:
                                            ProcessMLeader(mLeader, regex);
                                            break;
                                        case BlockReference blockRef:
                                            ProcessBlockReference(blockRef, regex);
                                            break;
                                    }
                                }
                            }

                            acTrans.Commit();
                        }


                    }
                }
            }

            _dictionaryService.UpdateDictionaryAndSaveToJson(_dict);
        }


        private SelectionFilter CreateTextAndTableFilter()
        {
            var filterList = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "TEXT,MTEXT,ACAD_TABLE,LEADER,MULTILEADER,ATTDEF,ATTRIB,INSERT")
            };

            return new SelectionFilter(filterList);
        }

        private void ProcessMText(MText mText, string separator, bool preserveOriginalText = false)
        {

            var text = GetClearString(mText.Contents);

            //if (ShouldTranslate(text, regex))
            //{
            mText.Contents = preserveOriginalText ? $"{mText.Contents}{separator}{TranslateText(text)}" : TranslateText(text);
            //}
        }

        private void ProcessDbText(DBText dbText, string separator, bool preserveOriginalText = false)
        {
            string text = dbText.TextString.Trim();
            if (separator == "\n") separator = "-";
            //if (ShouldTranslate(text))
            //{
            dbText.TextString = preserveOriginalText ? $"{text}{separator}{TranslateText(text)}" : TranslateText(text);
            //}
        }

        private void ProcessTable(Table table, string separator, bool preserveOriginalText = false)
        {
            for (int row = 0; row < table.Rows.Count; row++)
            {
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    string cellContent = table.Cells[row, col].TextString.Trim();
                    if (!string.IsNullOrWhiteSpace(cellContent))
                    {
                        var clearedText = GetClearString(cellContent);
                        //if (ShouldTranslate(clearedText, regex))
                        //{
                        table.Cells[row, col].TextString = preserveOriginalText ? $"{cellContent}{separator}{TranslateText(clearedText)}" : TranslateText(clearedText);
                        //}
                    }
                }
            }
        }

        private string GetClearString(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;
            using (MText mText = new MText { Contents = source })
            {
                var text = mText.Text.Trim();
                if (text.Contains("\r\n"))
                {
                    string newtext = text.Replace("\r\n", "");
                    text = newtext;
                }


                return text;
            }
        }

        private void ProcessLeader(Leader leader, string separator)
        {
            var mtextId = leader.Annotation;
            if (mtextId != ObjectId.Null)
            {
                var mText = mtextId.GetObject(OpenMode.ForRead) as MText;
                if (mText != null)
                {
                    ProcessMText(mText, separator);
                }
            }
        }

        private void ProcessMLeader(MLeader mLeader, string sepatator)
        {
            var mText = mLeader.MText;
            if (mText != null)
            {
                ProcessMText(mText, sepatator);
            }
        }


        private void ProcessAttribute(AttributeReference attRef, string regex)
        {
            string text = attRef.TextString.Trim();
            //if (ShouldTranslate(text))
            //{
            attRef.TextString = TranslateText(text);
            //}
        }

        private void ProcessBlockReference(BlockReference blockRef, string separator)
        {
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                if (attId != ObjectId.Null)
                {
                    var attRef = attId.GetObject(OpenMode.ForWrite) as AttributeReference;
                    if (attRef != null)
                    {
                        ProcessAttribute(attRef, separator);
                    }
                }
            }

            var blockTableRecord = blockRef.DynamicBlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord;
            if (blockTableRecord != null)
            {
                foreach (ObjectId id in blockTableRecord)
                {
                    var entity = id.GetObject(OpenMode.ForRead) as Entity;
                    if (entity != null)
                    {
                        if (entity is MText mText)
                        {
                            ProcessMText(mText, separator);
                        }
                        else if (entity is DBText dbText)
                        {
                            ProcessDbText(dbText, separator);
                        }
                    }
                }
            }
        }

        private bool ShouldTranslate(string text)
        {
            return !IsLatin(text);
        }

        private string TranslateText(string text)
        {

            if (!_dictionaryService.TextFromJson.ContainsKey(text))
            {
                string translatedText = _textTranslator.Translate(text, _translationSettings.SourceLanguage, _translationSettings.TargetLanguage);
                if (translatedText != null)
                {
                    Active.Editor.WriteMessage($"\n{text} не переведен");
                }
                
                return translatedText;
            }
            else
            {
                string translatedText = _dictionaryService.TextFromJson[text];
                Active.Editor.WriteMessage($"\n{text} переведен из базы");
                return translatedText;
            }
        }

        private bool IsLatin(string text)
        {
            return text.All(c => c < 128);
        }

        private void Translate(string textStr)
        {
            //if (!IsLatin(textStr))
            //{
            //    if (!_dictionaryService.TextFromJson.ContainsKey(textStr))
            //    {
            //        string translatedText = _textTranslator.Translate(textStr);
            //        mText.Contents = translatedText;
            //        _dict.Add(textStr, translatedText);
            //        editor.WriteMessage($"\n{textStr} добавлен в базу");
            //    }
            //    else
            //    {
            //        mText.Contents = _dictionaryService.TextFromJson[textStr];
            //        editor.WriteMessage($"\n{textStr} переведен из базы");
            //    }
            //}
        }
    }

}
