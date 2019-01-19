using Converter.Web.Types;
using System;

namespace Converter.Web.Models
{
    [Serializable]
    public class SvgConvertModel
    {
        public FormatType Format { get; set; }

        public int NumberOfFiles { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}