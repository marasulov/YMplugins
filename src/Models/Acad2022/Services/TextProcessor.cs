using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.EditorInput.IoC;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gile.AutoCAD.Extension;
using YMplugins.Services.Translator;

namespace YMplugins.Models.Acad2022.Services
{
    public class TextProcessor
    {
        private readonly DictionaryService _dictionaryService;
        private readonly ITextTranslator _textTranslator;
        private readonly Dictionary<string, string> _dict;

        public TextProcessor(DictionaryService dictionaryService, ITextTranslator textTranslator)
        {
            _dictionaryService = dictionaryService;
            _textTranslator = textTranslator;
            _dict = new Dictionary<string, string>();
        }

        public void ProcessTexts()
        {
            using (var acTrans = Active.Database.TransactionManager.StartTransaction())
            {
                var acSSPrompt = Active.Editor.GetSelection(new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, "TEXT,MTEXT,ACAD_TABLE,MULTILEADER,INSERT,ATTDEF,ATTRIB") }));

                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    var acSSet = acSSPrompt.Value;
                    var selectedIds = acSSet.GetObjectIds();
                    string regex = "[^A-Za-z0-9]+";

                    foreach (ObjectId acSSObj in selectedIds)
                    {
                        if (acSSObj != null)
                        {
                            var acEnt = acTrans.GetObject(acSSObj, OpenMode.ForWrite) as Entity;

                            switch (acEnt)
                            {
                                case MText mText:
                                    ProcessMText(mText, regex);
                                    break;
                                case DBText dbText:
                                    ProcessDBText(dbText, regex);
                                    break;
                                case Table table:
                                    ProcessTable(table, regex);
                                    break;
                                case Leader leader:
                                    ProcessLeader(leader, regex);
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

            _dictionaryService.UpdateDictionaryAndSaveToJson(_dict);
        }
        private void ProcessBlockReference(BlockReference blockRef, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                if (attId != ObjectId.Null)
                {
                    var attRef = attId.GetObject(OpenMode.ForWrite) as AttributeReference;
                    if (attRef != null)
                    {
                        ProcessAttributeReference(attRef, regex);
                    }
                }
            }
        }

        private void ProcessMText(MText mText, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            TextEditor textEditor = TextEditor.CreateTextEditor(mText);
            textEditor.SelectAll();
            TextEditorSelection selection = textEditor.Selection;
            selection.RemoveAllFormatting();
            string mtextStr = selection.SelectionString.Trim();

            try
            {
                if (!IsLatin(mtextStr) && Regex.IsMatch(mtextStr, regex))
                {
                    if (!_dictionaryService.TextFromJson.ContainsKey(mtextStr))
                    {
                        string translatedText = _textTranslator.Translate(mtextStr);
                        mText.Contents = translatedText;
                        _dict.Add(mtextStr, translatedText);
                        editor.WriteMessage($"\n{mtextStr} добавлен в базу");
                    }
                    else
                    {
                        mText.Contents = _dictionaryService.TextFromJson[mtextStr];
                        editor.WriteMessage($"\n{mtextStr} переведен из базы");
                    }
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nОшибка при обработке MText '{mtextStr}': {ex.Message}");
            }
        }

        private void ProcessDBText(DBText dbText, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            string dbTextStr = dbText.TextString.Trim();

            try
            {
                if (!IsLatin(dbTextStr) && Regex.IsMatch(dbTextStr, regex))
                {
                    if (!_dictionaryService.TextFromJson.ContainsKey(dbTextStr))
                    {
                        string translatedText = _textTranslator.Translate(dbTextStr);
                        dbText.TextString = translatedText;
                        _dict.Add(dbTextStr, translatedText);
                        editor.WriteMessage($"\n{dbTextStr} добавлен в базу");
                    }
                    else
                    {
                        dbText.TextString = _dictionaryService.TextFromJson[dbTextStr];
                        editor.WriteMessage($"\n{dbTextStr} переведен из базы");
                    }
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nОшибка при обработке DBText '{dbTextStr}': {ex.Message}");
            }
        }

        private void ProcessTable(Table table, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            for (int row = 0; row < table.Rows.Count; row++)
            {
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    var cellContent = table.Cells[row, col].TextString.Trim();

                    try
                    {
                        if (!IsLatin(cellContent) && Regex.IsMatch(cellContent, regex))
                        {
                            if (!_dictionaryService.TextFromJson.ContainsKey(cellContent))
                            {
                                string translatedText = _textTranslator.Translate(cellContent);
                                table.Cells[row, col].TextString = translatedText;
                                _dict.Add(cellContent, translatedText);
                                editor.WriteMessage($"\n{cellContent} добавлен в базу");
                            }
                            else
                            {
                                table.Cells[row, col].TextString = _dictionaryService.TextFromJson[cellContent];
                                editor.WriteMessage($"\n{cellContent} переведен из базы");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        editor.WriteMessage($"\nОшибка при обработке ячейки таблицы '{cellContent}': {ex.Message}");
                    }
                }
            }
        }

        private void ProcessLeader(Leader leader, string regex)
        {
            var mtextId = leader.Annotation;
            if (mtextId != ObjectId.Null)
            {
                var mText = mtextId.GetObject(OpenMode.ForRead) as MText;
                if (mText != null)
                {
                    ProcessMText(mText, regex);
                }
            }
        }

        private void ProcessMLeader(MLeader mLeader, string regex)
        {
            var mText = mLeader.MText;

            if (mText != null)
            {
                ProcessMText(mText, regex);
            }
        }

        private void ProcessAttributeDefinition(AttributeDefinition attDef, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            string attDefStr = attDef.TextString.Trim();

            try
            {
                if (!IsLatin(attDefStr) && Regex.IsMatch(attDefStr, regex))
                {
                    if (!_dictionaryService.TextFromJson.ContainsKey(attDefStr))
                    {
                        string translatedText = _textTranslator.Translate(attDefStr);
                        attDef.TextString = translatedText;
                        _dict.Add(attDefStr, translatedText);
                        editor.WriteMessage($"\n{attDefStr} добавлен в базу");
                    }
                    else
                    {
                        attDef.TextString = _dictionaryService.TextFromJson[attDefStr];
                        editor.WriteMessage($"\n{attDefStr} переведен из базы");
                    }
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nОшибка при обработке AttributeDefinition '{attDefStr}': {ex.Message}");
            }
        }

        private void ProcessAttributeReference(AttributeReference attRef, string regex)
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            string attRefStr = attRef.TextString.Trim();

            try
            {
                if (!IsLatin(attRefStr) && Regex.IsMatch(attRefStr, regex))
                {
                    if (!_dictionaryService.TextFromJson.ContainsKey(attRefStr))
                    {
                        string translatedText = _textTranslator.Translate(attRefStr);
                        attRef.TextString = translatedText;
                        _dict.Add(attRefStr, translatedText);
                        editor.WriteMessage($"\n{attRefStr} добавлен в базу");
                    }
                    else
                    {
                        attRef.TextString = _dictionaryService.TextFromJson[attRefStr];
                        editor.WriteMessage($"\n{attRefStr} переведен из базы");
                    }
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nОшибка при обработке AttributeReference '{attRefStr}': {ex.Message}");
            }
        }

        private bool IsLatin(string input)
        {
            // Проверка на латиницу
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
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
