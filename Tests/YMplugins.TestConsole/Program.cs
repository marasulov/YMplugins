
using YMplugins.Models.Autocad2022.Utils;

class Program
{
    static void Main(string[] args)
    {
        var format = FormatFinder.FindFormatWithScale(1189,420, 1);
        Console.WriteLine($"format {format} scale {format}");
        
    }
}


