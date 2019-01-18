using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Converter.Web.Models
{

    [Serializable]
    public class SvgModel
    {
        public int Format { get; set; }

        public int NumberOfFiles { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}