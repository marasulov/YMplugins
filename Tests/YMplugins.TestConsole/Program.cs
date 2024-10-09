
using YMplugins.Models.Autocad2022.Utils;

class Program
{
    static void Main(string[] args)
    {
        var format = FormatFinder.FindFormatWithScale(2970, 4200);
        Console.WriteLine($"format {format.Format} scale {format.Scale}");
        
    }
}


