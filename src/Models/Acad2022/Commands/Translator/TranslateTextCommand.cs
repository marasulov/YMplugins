using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Gile.AutoCAD.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Formatting = System.Xml.Formatting;

namespace YMplugins.Models.Acad2022.Commands.Translator
{

    internal class TranslateTextCommand
    {
        static string filename = @"c:\temp\dict.json";

        [CommandMethod("TranslateToTarget")]
        public void TranslateText()
        {


            // Пример использования словаря
            if (dictionary.TryGetValue("Общие данные", out string value))
            {
                Console.WriteLine($"Общие данные: {value}");
            }

            Dictionary<string, string> textFromJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFileToRead);

            TypedValue[] acTypValAr = new TypedValue[4];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Operator, "<or"), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "TEXT"), 1);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 2);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Operator, "or>"), 3);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            using (Transaction acTrans = Active.Database.TransactionManager.StartTransaction())
            {
                PromptSelectionResult acSSPrompt = Active.Editor.GetSelection(acSelFtr);

                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPrompt.Value;
                    ObjectId[] selectedIds = acSSet.GetObjectIds();

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    string regex = "[^A-Za-z0-9]+";

                    foreach (ObjectId acSSObj in selectedIds)
                    {
                        if (acSSObj != null)
                        {

                            Entity acEnt = acTrans.GetObject(acSSObj, OpenMode.ForWrite) as Entity;

                            if (acEnt.GetType() == typeof(MText))
                            {
                                MText mText = ((MText)acEnt);
                                TextEditor textEditor = TextEditor.CreateTextEditor(mText);
                                textEditor.SelectAll();
                                TextEditorSelection selection = textEditor.Selection;
                                selection.RemoveAllFormatting();
                                string mtextStr = selection.SelectionString.Trim();

                                try
                                {
                                    if (!textFromJson.ContainsKey(mtextStr) && !IsLatin(mtextStr))
                                    {
                                        if (Regex.IsMatch(mtextStr, regex))
                                        {
                                            string TranslatedMtext = TranslateYandex(mtextStr, "ru-en");
                                            //string TranslatedMtext = MtextStr + "translated text";
                                            //Application.ShowAlertDialog(TranslatedMtext);

                                            //textEditor = TextEditor.CreateTextEditor(mText);
                                            //textEditor.SelectAll();
                                            //textEditor.ClearSelection();

                                            mText.Contents = TranslatedMtext;
                                            //textEditor.Close(TextEditor.ExitStatus.ExitSave);
                                            dict.Add(mtextStr, TranslatedMtext);
                                            Active.Editor.WriteMessage(mtextStr + "добавлен в базу");
                                            //Application.ShowAlertDialog(MtextStr + "добавлен в базу");
                                        }

                                    }
                                    else
                                    {
                                        mText.Contents = textFromJson[mtextStr];
                                        Active.Editor.WriteMessage(mtextStr + "добавлен в базу");
                                        //Application.ShowAlertDialog(MtextStr + "переведен из базы");
                                    }
                                }
                                catch
                                {
                                    //mText.Contents = textFromJson[MtextStr];
                                    Active.Editor.WriteMessage(mtextStr + "ошибка");
                                    //Application.ShowAlertDialog(MtextStr + "есть или уже есть");
                                }
                            }
                            if (acEnt.GetType() == typeof(DBText))
                            {
                                DBText dBText = ((DBText)acEnt);
                                string DbtextStr = dBText.TextString.Trim();
                                Active.Editor.WriteMessage("\n This is Dbtext " + DbtextStr);
                                try
                                {
                                    if (!textFromJson.ContainsKey(DbtextStr) && !IsLatin(DbtextStr))
                                    {
                                        //Application.ShowAlertDialog(Regex.IsMatch(DbtextStr, regex).ToString());
                                        if (Regex.IsMatch(DbtextStr, regex))
                                        {
                                            string TranslatedDbtext = Translate(DbtextStr);
                                            //string TranslatedDbtext = DbtextStr + "translated text";
                                            dict.Add(DbtextStr, TranslatedDbtext);

                                            //Application.ShowAlertDialog(DbtextStr + "добавлен");
                                            dBText.TextString = TranslatedDbtext;
                                            Active.Editor.WriteMessage(DbtextStr + "добавлен");
                                        }
                                    }
                                    else
                                    {
                                        dBText.TextString = textFromJson[DbtextStr];
                                        Active.Editor.WriteMessage(dBText + "переведен из базы");
                                        //Application.ShowAlertDialog(MtextStr + "переведен из базы");
                                    }
                                }
                                catch
                                {
                                    Active.Editor.WriteMessage(DbtextStr + "значение ключа пустое или ключ латиница");
                                    //Application.ShowAlertDialog(DbtextStr + "значение ключа пустое или ключ латиница");
                                }
                            }

                        }
                    }
                    acTrans.Commit();

                    foreach (var item in dict)
                    {
                        if (textFromJson.ContainsKey(item.Key))
                        {
                            Active.Editor.WriteMessage("\n в базе есть слово  " + item);
                        }
                        else
                        {
                            textFromJson.Add(item.Key, item.Value);
                        }
                    }

                    string JsonToWrite = JsonConvert.SerializeObject(textFromJson, Formatting.Indented);
                    File.WriteAllText(filename, JsonToWrite);
                }
            }
        }
    }
}
