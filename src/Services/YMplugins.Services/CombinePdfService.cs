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

            using (FileStream stream = new FileStream(outputPdf, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;
                try
                {
                    document.Open();
                    foreach (string file in filenames)
                    {
                        reader = new PdfReader(file);
                        pdf.AddDocument(reader);
                        reader.Close();
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }

            return outputPdf;
        }
    }
}
