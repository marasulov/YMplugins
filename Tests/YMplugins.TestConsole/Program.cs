// See https://aka.ms/new-console-template for more information
//using Google.Cloud.Translation.V2;

//string credentialPath = $"C:\\Users\\yusufzhon.marasulov\\Documents\\unique-rarity-430509-s3-1f1d9dd17232.json";
//Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);


//TranslationClient client = TranslationClient.Create();


//string textToTranslate = "PIG launcher!";
//string targetLanguage = "ru"; // Код языка (например, 'ru' для русского)


//var response = client.TranslateText(textToTranslate, targetLanguage);

//Console.WriteLine($"Original Text: {textToTranslate}");
//Console.WriteLine($"Translated Text: {response.TranslatedText}");


using System.Text.RegularExpressions;

var jsonContent = File.ReadAllText("C:\\Temp\\text2.json");
//var regex = new Regex("\"(.*?)\"", RegexOptions.Singleline);

string pattern = @"null,null,3,null,null";

string[] parts = Regex.Split(jsonContent, pattern);

foreach (var part in parts)
{
    Console.WriteLine(part);
}

Console.WriteLine("-----------------------");
var newParts = parts.Take(parts.Length - 1).ToArray();
var firstEl = parts.Take(0);

for (int i = 1; i < newParts.Length; i++)
{
    var t = newParts[1].Split(",");
}

foreach (var part in newParts)
{
   
    Console.WriteLine(part);
}

Console.WriteLine("-----------------------");
var newList = new List<string>();
for (int i = 1; i < newParts.Length; i++)
{
    var newSplit = newParts[i].Split("],");

    foreach (var part in newSplit)
    {
        Console.WriteLine(part);
    }


    newList.Add(newSplit[3]);

}

newList.Insert(0, parts[0].Replace("[[",""));
Console.WriteLine("-----------------------");
var newtr = new List<string>();

foreach (var s in newList)
{
    //var ss = s.Split("\",\"");
    //foreach (var part in s)
    //{
    //    Console.WriteLine(part);
    //}
    var newS = s.Replace("[\"", "");
    Console.WriteLine(newS);
    var newT = newS.Split("\",\"")[0];
    Console.WriteLine(newT);
    newtr.Add(newT);
}

//newtr.Insert(0, newParts[0].Substring(6, newParts[0].IndexOf("\"", 6, StringComparison.Ordinal) -6));

var translatedText = string.Join(" ", newtr);
    
Console.WriteLine();