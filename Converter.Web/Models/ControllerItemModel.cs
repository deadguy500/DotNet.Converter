using System;

namespace Converter.Web.Models
{
    [Serializable]
    public class ControllerItemModel
    {
        public string LinkName { get; set; }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }
    }
}