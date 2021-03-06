﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Converter.Web.Helpers
{
    public static class CommonHelper
    {
        public static byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            var reader = new BinaryReader(image.InputStream);
            var imageBytes = reader.ReadBytes((int)image.ContentLength);

            return imageBytes;
        }

        public static List<byte[]> GetRequestBytes(Controller ctrl, int numFiles, string element, string mimetype)
        {
            var list = new List<byte[]>();

            for (int ii = 0; ii < numFiles; ii++)
            {
                var file = ctrl.Request.Files[string.Format("{0}{1}", element, ii)];

                if (file.ContentType.StartsWith(mimetype))
                {
                    var data = ConvertToBytes(file);
                    list.Add(data);
                }
            }

            return list;
        }

        public static string AddSpacesToSentence(string inputTtext)
        {
            if (string.IsNullOrWhiteSpace(inputTtext))
            {
                return string.Empty;
            }

            var outputStringBuilder = new StringBuilder(inputTtext.Length * 2);
            outputStringBuilder.Append(inputTtext[0]);

            for (int i = 1; i < inputTtext.Length; i++)
            {
                if (char.IsUpper(inputTtext[i]) && inputTtext[i - 1] != ' ')
                {
                    outputStringBuilder.Append(' ');
                }

                outputStringBuilder.Append(inputTtext[i]);
            }

            return outputStringBuilder.ToString();
        }

        public static byte[] GetMergePdf(List<byte[]> pdfData)
        {
            var document = new Document();

            byte[] pdfBytes;

            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfCopy(document, memoryStream);

                if (writer == null)
                {
                    return null;
                }

                document.Open();

                foreach (var file in pdfData)
                {
                    var reader = new PdfReader(file);
                    reader.ConsolidateNamedDestinations();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        var page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }

                    var form = reader.AcroForm;

                    if (form != null)
                    {
                        writer.AddDocument(reader);
                    }

                    reader.Close();
                }

                writer.Close();
                document.Close();

                pdfBytes = memoryStream.ToArray();
            }

            return pdfBytes;
        }
    }
}