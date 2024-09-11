using System.Diagnostics;

public class AutoCADLauncher
{
    public void StartAutoCAD()
    {
        var process = new Process();
        process.StartInfo.FileName = @"C:\Program Files\Autodesk\AutoCAD 2024\acad.exe"; // Проверьте путь
        process.StartInfo.Arguments = @"/product ACAD /language ""en-US"" /nologo /nohardware /b";
        process.StartInfo.WorkingDirectory = @"C:\Program Files\Autodesk\AutoCAD";
        process.Start();

        // Подключение отладчика к вашему процессу (если нужно)
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
    }
}