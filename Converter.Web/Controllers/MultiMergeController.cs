using Converter.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Converter.Web.Controllers
{
    public class MultiMergeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> pdfFiles)
        {
            if (pdfFiles != null)
            {
                var pdfDataList = new List<byte[]>();

                foreach (var file in pdfFiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.ContentType.StartsWith("application/pdf"))
                        {
                            var data = CommonHelper.ConvertToBytes(file);
                            pdfDataList.Add(data);
                        }
                    }
                }

                var pdfBytes = CommonHelper.GetMergePdf(pdfDataList);

                return File(pdfBytes, "application/pdf", string.Format("{0}-{1}.pdf", Guid.NewGuid(), DateTime.Now.ToShortDateString()));
            }

            return View();
        }
    }
}