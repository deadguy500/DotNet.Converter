using Converter.Web.Helpers;
using Converter.Web.Models;
using Converter.Web.Types;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class SvgConvertController : Controller
    {
        private const int NUMBER_OF_FILES = 12;
        private const int BASE_WIDTH = 640;
        private const int BASE_HEIGHT = 512;

        [HttpGet]
        public ActionResult Index()
        {
            var model = new SvgConvertModel()
            {
                Format = FormatType.PNG,
                NumberOfFiles = NUMBER_OF_FILES,
                Height = BASE_HEIGHT,
                Width = BASE_WIDTH
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SvgConvertModel model)
        {
            if (ModelState.IsValid)
            {
                var svgDataList = CommonHelper.GetRequestBytes(this, NUMBER_OF_FILES, "ImageData", "image/svg");
                var bitmap = SvgToBitmap(svgDataList, model.Width, model.Height);

                if (bitmap != null)
                {
                    if (model.Format == FormatType.PNG)
                    {
                        var bitmapStream = new MemoryStream();
                        bitmap.Save(bitmapStream, ImageFormat.Png);
                        bitmapStream.Position = 0;

                        return File(bitmapStream, "image/x-png", string.Format("{0}-{1}.png", Guid.NewGuid(), DateTime.Now.ToShortDateString()));
                    }
                    else if (model.Format == FormatType.PDF)
                    {
                        var pdfBytes = GetPdfStream(bitmap);

                        return File(pdfBytes, "application/pdf", string.Format("{0}-{1}.pdf", Guid.NewGuid(), DateTime.Now.ToShortDateString()));
                    }
                }
            }

            model.NumberOfFiles = NUMBER_OF_FILES;

            return View(model);
        }
        
        private Bitmap SvgToBitmap(List<byte[]> svgList, int width, int height)
        {
            var doc = new Svg.SvgDocument();

            foreach (var svg in svgList)
            {
                using (var svgStream = new MemoryStream(svg))
                {
                    var svgChild = Svg.SvgDocument.Open<Svg.SvgDocument>(svgStream);
                    doc.Children.Add(svgChild);
                }
            }

            return doc.Draw(width, height);
        }

        private byte[] GetPdfStream(Bitmap bitmap)
        {
            var pdfDocument = new Document();
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDocument, pdfStream);

            pdfDocument.Open();
            {
                var pdfImage = iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Png);
                pdfDocument.Add(pdfImage);
            }
            pdfDocument.Close();

            return pdfStream.ToArray();
        }
    }
}