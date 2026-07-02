using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using YMplugins.Contracts;

namespace YMplugins.Services
{
    public class CombinePdfService : ICombinePdfService
    {
        private readonly IAutoCadFileService _autoCadFileService;

        public CombinePdfService(IAutoCadFileService autoCadFileService)
        {
            _autoCadFileService = autoCadFileService;
        }

        public string Combine(string[] filenames, string targetPdf)
        {
            string outputPdf = _autoCadFileService.GetActiveDocumentName();
            if (!string.IsNullOrEmpty(targetPdf))
            {
                string directory = Path.GetDirectoryName(outputPdf);
                if (directory != null) outputPdf = Path.Combine(directory, targetPdf);
            }

            string currentFile = null;
            try
            {
                using (FileStream stream = new FileStream(outputPdf, FileMode.Create))
                {
                    Document document = new Document();
                    PdfCopy pdf = new PdfCopy(document, stream);
                    document.Open();
                    foreach (string file in filenames)
                    {
                        currentFile = file;
                        PdfReader reader = new PdfReader(file);
                        try
                        {
                            pdf.AddDocument(reader);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }

                    document.Close();
                }
            }
            catch (Exception e)
            {
                throw new IOException(
                    $"Не удалось объединить PDF (файл: {currentFile ?? outputPdf}): {e.Message}", e);
            }

            // Исходные файлы удаляются только после успешной склейки,
            // чтобы при сбое пользователь не потерял уже напечатанные листы
            foreach (string file in filenames)
            {
                File.Delete(file);
            }

            return outputPdf;
        }
    }
}
