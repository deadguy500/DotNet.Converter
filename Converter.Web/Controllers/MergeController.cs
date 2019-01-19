using Converter.Web.Helpers;
using Converter.Web.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class MergeController : Controller
    {
        private const int NUMBER_OF_FILES = 2;

        [HttpGet]
        public ActionResult Index()
        {
            var model = new PdfMergeModel()
            {
                NumberOfFiles = NUMBER_OF_FILES
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PdfMergeModel model)
        {
            if (ModelState.IsValid)
            {
                var pdfData = CommonHelper.GetRequestBytes(this, NUMBER_OF_FILES, "PdfData", "application/pdf");

                var pdfBytes = GetMergePdf(pdfData);

                return File(pdfBytes, "application/pdf", string.Format("{0}-{1}.pdf", Guid.NewGuid(), DateTime.Now.ToShortDateString()));                
            }

            model.NumberOfFiles = NUMBER_OF_FILES;

            return View(model);
        }

        private byte[] GetMergePdf(List<byte[]> pdfData)
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