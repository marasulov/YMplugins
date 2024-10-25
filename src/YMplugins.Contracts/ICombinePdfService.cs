namespace YMplugins.Contracts;

public interface ICombinePdfService
{
    string Combine(string[] filenames, string outputFileName);
}