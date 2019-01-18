using Converter.Web.Models;
using Converter.Web.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class HomeController : Controller
    {
        private const int NUMBER_OF_FILES = 12;
        private const int BASE_WIDTH = 640;
        private const int BASE_HEIGHT = 512;

        public ActionResult Index()
        {
            var model = new SvgModel()
            {
                Format = (int)FormatType.PNG,
                NumberOfFiles = NUMBER_OF_FILES,
                Height = BASE_HEIGHT,
                Width = BASE_WIDTH
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SvgModel model)
        {
            if (ModelState.IsValid)
            {
                var svgDataList = GetSvgData();
                var bitmap = SvgToBitmap(svgDataList, model.Width, model.Height);

                var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                if (model.Format == (int)FormatType.PNG)
                {
                    return new FileStreamResult(ms, "image/png");
                }
                else if (model.Format == (int)FormatType.PDF)
                {
                    return new FileStreamResult(ms, "application/pdf");
                }
            }

            return View(model);
        }

        private List<byte[]> GetSvgData()
        {
            var list = new List<byte[]>();

            for (int ii = 0; ii < NUMBER_OF_FILES; ii++)
            {
                var file = Request.Files[string.Format("ImageData{0}", ii)];

                if (file.ContentType.StartsWith("image/svg"))
                {
                    var svgData = ConvertToBytes(file);
                    list.Add(svgData);
                }
            }

            return list;
        }

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            var reader = new BinaryReader(image.InputStream);
            var imageBytes = reader.ReadBytes((int)image.ContentLength);

            return imageBytes;
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
    }
}