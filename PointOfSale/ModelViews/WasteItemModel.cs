using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class WasteItemModel
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public long DistributeId { get; set; }
        public int Quantity { get; set; }
        public int WasteTypeId { get; set; }
        public string Comment { get; set; }
    }
}