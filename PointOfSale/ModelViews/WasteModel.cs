using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class WasteModel
    {
        public int UserId { get; set; }
        public IList<WasteItemModel> WasteData { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
    }
}