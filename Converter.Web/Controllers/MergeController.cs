using Converter.Web.Helpers;
using Converter.Web.Models;
using System;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class MergeController : Controller
    {
        private const int NUMBER_OF_FILES = 10;

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

                var pdfBytes = CommonHelper.GetMergePdf(pdfData);

                return File(pdfBytes, "application/pdf", string.Format("{0}-{1}.pdf", Guid.NewGuid(), DateTime.Now.ToShortDateString()));                
            }

            model.NumberOfFiles = NUMBER_OF_FILES;

            return View(model);
        }
    }
}