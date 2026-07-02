
using System.Reflection;
using Ionic.Zlib;
using System.Text;
using PiaNO;
using PiaNO.Plot;
using YMplugins.Models.Autocad2024.Utils;
//using iTextSharp.text.pdf.parser;
//using iTextSharp.text.pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Text.RegularExpressions;

class Program
{
    public class MyPiaNode : PiaNode
    {
        public void AccessInnerData()
        {
            var innerData = this.InnerData;
            Console.WriteLine(innerData);
        }
    }
    static void Main(string[] args)
    {
        //var format = FormatFinder.FindFormatWithScale(1189,420, 1);

        //string supportPath = @"D:\from comp\docs\repos\TransmittalCreator\TransmittalCreator\bin\Debug";
        //string configName = Path.Combine(supportPath, "Uzle.pmp");
        //var pdfConfig = new PlotterConfiguration(configName);
        //var canonicalModel = pdfConfig.CanonicalModel;

        //foreach (PiaNode a in pdfConfig)
        //{
        //    string aNodeName = a.ToString();


        //    Console.WriteLine("PiaNode=={0}", aNodeName);
        //    if (aNodeName == "udm")
        //    {
        //        foreach (PiaNode node in a)
        //        {
        //            Console.WriteLine(node.ToString());
        //        }
        //    }



        //    foreach (PiaNode b in a)
        //    {
        //        string bNodeName = b.ToString();

        //        Console.WriteLine("\tPiaNode=={0}", bNodeName);

        //    }
        //}


        //pdfConfig.TruetypeAsText = true;


        //pdfConfig.Write(Path.Combine(supportPath, "DWG To PDF - NoLayersOrBookmarks.pc3"));

        //Console.WriteLine($"CanonicalFamily: {pdfConfig.CanonicalFamily}");
        //Console.WriteLine($"DriverPath: {pdfConfig.DriverPath}");
        //Console.WriteLine($"DriverTagline: {pdfConfig.DriverTagline}");
        //Console.WriteLine($"DriverType: {pdfConfig.DriverType}");
        //Console.WriteLine($"DriverVersion: {pdfConfig.DriverVersion}");


        //var properties = pdfConfig.GetType().GetProperties();
        //foreach (var prop in properties)
        //{
        //    var value = prop.GetValue(pdfConfig);

        //    Console.WriteLine($"{prop.Name}: {value}");
        //}

        //var methods = pdfConfig.GetType().GetMethods();
        //foreach (var method in methods)
        //{
        //    Console.WriteLine($"Method: {method.Name}");
        //}

        //ChangePc3(1500.ToString(), 1600.ToString(), "1500x1600");
        //ChangePmp(1500.ToString(), 1600.ToString(), "1500x1600");


        string pdfPath = args.Length > 0 ? args[0] : Console.ReadLine();

        try
        {
            Console.WriteLine("Извлечение текста и поиск формата...\n");

            var (textElements, format) = PdfTextExtractor.ExtractTextWithFormat(pdfPath);

            // Вывод информации о формате
            Console.WriteLine("Информация о формате чертежа:");
            if (format != null)
            {
                Console.WriteLine($"Найден формат: {format.Format}");
                Console.WriteLine($"Ключевое слово: {format.FormatKeyword}");
                Console.WriteLine($"Значение формата: {format.FormatValue}");
                Console.WriteLine($"Расстояние между элементами: {format.Distance:F2}");
            }
            else
            {
                Console.WriteLine("Формат не найден");
            }

            Console.WriteLine("\nВесь текст с координатами:");
            foreach (var element in textElements)
            {
                Console.WriteLine(element);
            }

            // Вывод в файл
            string outputPath = "text_coordinates.txt";
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("Текст\tX\tY\tШирина\tВысота");
                foreach (var element in textElements)
                {
                    writer.WriteLine(
                        $"{element.Text}\t{element.X:F2}\t{element.Y:F2}\t{element.Width:F2}\t{element.Height:F2}"
                    );
                }

                writer.WriteLine("\nИнформация о формате:");
                if (format != null)
                {
                    writer.WriteLine($"Формат: {format.Format}");
                    writer.WriteLine($"Координаты ключевого слова: X={format.FormatKeyword.X:F2}, Y={format.FormatKeyword.Y:F2}");
                    writer.WriteLine($"Координаты значения: X={format.FormatValue.X:F2}, Y={format.FormatValue.Y:F2}");
                    writer.WriteLine($"Расстояние: {format.Distance:F2}");
                }
            }

            Console.WriteLine($"\nПодробные результаты сохранены в файл: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке файла: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }



    //public static void ExtractTextFromCustomArea(string pdfPath)
    //{
    //    PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath));

    //    for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
    //    {
    //        PdfPage page = pdfDoc.GetPage(pageNum);

    //        // Получаем размеры страницы
    //        Rectangle pageSize = page.GetPageSize();

    //        // Координаты нижнего правого угла страницы
    //        float lowerRightX = pageSize.GetRight();
    //        float lowerRightY = pageSize.GetBottom();

    //        // Определяем ширину и высоту области чтения (185 влево и 55 вверх)
    //        float regionWidth = 185;
    //        float regionHeight = 55;
            
    //        // Проверяем, чтобы область не выходила за пределы страницы
    //        float x = Math.Max(lowerRightX - regionWidth, pageSize.GetLeft());
    //        float y = Math.Max(lowerRightY, pageSize.GetBottom());
    //        float width = Math.Min(regionWidth, lowerRightX - pageSize.GetLeft());
    //        float height = Math.Min(regionHeight, pageSize.GetTop() - lowerRightY);

    //        // Если ширина или высота меньше нуля, значит координаты вне границ
    //        if (width <= 0 || height <= 0)
    //        {
    //            Console.WriteLine("Область вне границ страницы на странице " + pageNum);
    //            continue;
    //        }

    //        // Создаём прямоугольник области для чтения текста
    //        Rectangle rect = new Rectangle(x, y, width, height);

    //        // Извлекаем текст из указанной области
    //        FilteredTextEventListener listener = new FilteredTextEventListener(new LocationTextExtractionStrategy(),
    //            new TextRegionEventFilter(rect));

    //        string text = PdfTextExtractor.GetTextFromPage(page, listener);
    //        Console.WriteLine("Text from custom area on page " + pageNum + ": " + text);
    //    }

    //    pdfDoc.Close();
    //}
    public static void ChangePc3(string width, string height, string area)
    {
        string fileName1 = @"C:\Users\yusufzhon.marasulov\AppData\Roaming\Autodesk\AutoCAD 2022\R24.1\enu\Plotters\DWG_To_PDF_Uzle1.pc3";
        //string content = "";
        //using (FileStream fs = File.Open(fileName1, FileMode.Open, FileAccess.Read))
        //{
        //    fs.Seek(60L, SeekOrigin.Begin);
        //    using (ZlibStream zs = new ZlibStream(fs, CompressionMode.Decompress))
        //    {
        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //        using (StreamReader sr = new StreamReader(zs, Encoding.GetEncoding(1251)))
        //        {
        //            content = sr.ReadToEnd();
                    
        //            content = content.Replace(@"user_defined_model_pathname=C:\Users\yusufzhon.marasulov\AppData\Roaming\Autodesk\AutoCAD 2022\R24.1\enu\Plotters\PMP Files\Uzle2.pmp", $"actual_printable_bounds_urx={width}");
        //            content = content.Replace("actual_printable_bounds_ury=500.0", $"actual_printable_bounds_ury={height}");
        //            content = content.Replace("printable_bounds_urx=100.0", $"printable_bounds_urx={width}");
        //            content = content.Replace("printable_bounds_ury=500.0", $"printable_bounds_ury={height}");
        //            content = content.Replace("printable_area=50000.0", $"printable_area={area}");
        //            content = content.Replace("media_bounds{    urx = 100.0    ury = 500.0   }", "media_bounds{    urx = " + height + "    ury = " + width + "   }");
        //            using (FileStream fs_out = File.Open(fileName1 + ".txt", FileMode.Create, FileAccess.ReadWrite))
        //            {
        //                fs_out.Write(Encoding.Default.GetBytes(content), 0, Encoding.Default.GetBytes(content).Length);
        //            }
        //        }
        //    }
        //}
        string fileName = fileName1 + ".txt";
        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
            {
                String pref_s = "PIAFILEVERSION_2.0,PC3VER1,compress\r\npmzlibcodec";
                long decompresse_stream_size = fs.Length;
                long compressed_stream_size = fs.Length;
                string s = sr.ReadToEnd();
                using (FileStream fs_out = File.Open(@"C:\Users\yusufzhon.marasulov\AppData\Roaming\Autodesk\AutoCAD 2022\R24.1\enu\Plotters\DWG_To_PDF_Uzle2.pc3", FileMode.Create, FileAccess.ReadWrite))
                {
                    using (ZlibStream zs = new ZlibStream(fs_out, CompressionMode.Compress,
                                                             CompressionLevel.BestCompression, false))
                    {
                        fs_out.Write(Encoding.Default.GetBytes(pref_s), 0, Encoding.Default.GetBytes(pref_s).Length);
                        fs_out.Write(BitConverter.GetBytes(new ZlibCodec(CompressionMode.Compress).Adler32), 0, 4);
                        fs_out.Write(BitConverter.GetBytes(decompresse_stream_size), 0, 4);
                        fs_out.Write(BitConverter.GetBytes(compressed_stream_size), 0, 4);

                        zs.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetBytes(s).Length);
                    }
                }
            }
        }
    }
    public static void ChangePmp(string width, string height, string area)
    {
        string fileName1 = @"C:\Users\yusufzhon.marasulov\AppData\Roaming\Autodesk\AutoCAD 2022\R24.1\enu\Plotters\PMP Files\Uzle1.pmp";
        //string content = "";
        //using (FileStream fs = File.Open(fileName1, FileMode.Open, FileAccess.Read))
        //{
        //    fs.Seek(60L, SeekOrigin.Begin);
        //    using (ZlibStream zs = new ZlibStream(fs, CompressionMode.Decompress))
        //    {
        //        using (StreamReader sr = new StreamReader(zs, Encoding.GetEncoding(1251)))
        //        {
        //            content = sr.ReadToEnd();
        //            content = content.Replace("media_bounds_urx=100.0", $"media_bounds_urx={width}");
        //            content = content.Replace("media_bounds_ury=500.0", $"media_bounds_ury={height}");
        //            content = content.Replace("printable_bounds_urx=100.0", $"printable_bounds_urx={width}");
        //            content = content.Replace("printable_bounds_ury=500.0", $"printable_bounds_ury={height}");
        //            content = content.Replace("printable_area=50000.0", $"printable_area={area}");
        //            using (FileStream fs_out = File.Open(fileName1 + ".txt", FileMode.Create, FileAccess.ReadWrite))
        //            {
        //                fs_out.Write(Encoding.Default.GetBytes(content), 0, Encoding.Default.GetBytes(content).Length);
        //            }
        //        }
        //    }
        //}
        string fileName = fileName1 + ".txt";
        using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
            {
                String pref_s = "PIAFILEVERSION_2.0,PC3VER1,compress\r\npmzlibcodec";
                long decompresse_stream_size = fs.Length;
                long compressed_stream_size = fs.Length;
                string s = sr.ReadToEnd();
                using (FileStream fs_out = File.Open(@"C:\Users\yusufzhon.marasulov\AppData\Roaming\Autodesk\AutoCAD 2022\R24.1\enu\Plotters\PMP Files\Uzle1.pmp", FileMode.Create, FileAccess.ReadWrite))
                {
                    using (ZlibStream zs = new ZlibStream(fs_out, CompressionMode.Compress,
                                                             CompressionLevel.BestCompression, false))
                    {
                        fs_out.Write(Encoding.Default.GetBytes(pref_s), 0, Encoding.Default.GetBytes(pref_s).Length);
                        fs_out.Write(BitConverter.GetBytes(new ZlibCodec(CompressionMode.Compress).Adler32), 0, 4);
                        fs_out.Write(BitConverter.GetBytes(decompresse_stream_size), 0, 4);
                        fs_out.Write(BitConverter.GetBytes(compressed_stream_size), 0, 4);
                        zs.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetBytes(s).Length);
                    }
                }
            }
        }
    }


}

public class TextLocationStrategy : LocationTextExtractionStrategy
{
    private readonly List<TextInfo> textInfos = new();

    public override void EventOccurred(IEventData data, EventType type)
    {
        if (!type.Equals(EventType.RENDER_TEXT))
            return;

        var renderInfo = (TextRenderInfo)data;
        var baseline = renderInfo.GetBaseline();
        var rectangle = baseline.GetBoundingRectangle();

        textInfos.Add(new TextInfo(
            renderInfo.GetText().Trim(),
            rectangle.GetX(),
            rectangle.GetY(),
            rectangle.GetWidth(),
            rectangle.GetHeight()
        ));
    }

    public List<TextInfo> GetTextLocations()
    {
        return textInfos.OrderByDescending(t => t.Y)
                       .ThenBy(t => t.X)
                       .ToList();
    }
}

public class TextInfo
{
    public string Text { get; }
    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }

    public TextInfo(string text, float x, float y, float width, float height)
    {
        Text = text;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"Text: {Text}, X: {X:F2}, Y: {Y:F2}, W: {Width:F2}, H: {Height:F2}";
    }
}

public class DrawingFormat
{
    public string Format { get; set; }
    public TextInfo FormatKeyword { get; set; }
    public TextInfo FormatValue { get; set; }
    public float Distance { get; set; }
}

public class PdfTextExtractor
{
    private const float MAX_FORMAT_DISTANCE = 50f; // Максимальное расстояние между словом "Формат" и значением

    public static (List<TextInfo> AllText, DrawingFormat Format) ExtractTextWithFormat(string pdfPath, int? pageNumber = null)
    {
        using var pdfDoc = new PdfDocument(new PdfReader(pdfPath));

        int pageNum = pageNumber ?? pdfDoc.GetNumberOfPages();
        var page = pdfDoc.GetPage(pageNum);
        var strategy = new TextLocationStrategy();

        new PdfCanvasProcessor(strategy).ProcessPageContent(page);
        var textElements = strategy.GetTextLocations();

        var format = FindDrawingFormat(textElements);

        return (textElements, format);
    }

    private static DrawingFormat FindDrawingFormat(List<TextInfo> textElements)
    {
        // Находим элемент со словом "Формат"
        var formatKeyword = textElements.FirstOrDefault(t =>
            t.Text.Equals("Формат", StringComparison.OrdinalIgnoreCase));

        if (formatKeyword == null)
            return null;

        // Ищем ближайший текстовый элемент справа от слова "Формат"
        var nearbyElements = textElements
            .Where(t =>
                t != formatKeyword &&
                Math.Abs(t.Y - formatKeyword.Y) < 5 && // примерно та же строка
                t.X > formatKeyword.X && // правее слова "Формат"
                t.X - (formatKeyword.X + formatKeyword.Width) < MAX_FORMAT_DISTANCE) // не слишком далеко
            .OrderBy(t => t.X)
            .ToList();

        var formatValue = nearbyElements.FirstOrDefault();
        if (formatValue == null)
            return null;

        return new DrawingFormat
        {
            Format = formatValue.Text.Trim(),
            FormatKeyword = formatKeyword,
            FormatValue = formatValue,
            Distance = formatValue.X - (formatKeyword.X + formatKeyword.Width)
        };
    }
}



