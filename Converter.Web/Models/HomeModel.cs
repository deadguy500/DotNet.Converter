using System;
using System.Collections.Generic;

namespace Converter.Web.Models
{
    [Serializable]
    public class HomeModel
    {
        public HomeModel()
        {
            this.List = new List<ControllerItemModel>();
        }

        public List<ControllerItemModel> List { get; set; }
    }
}