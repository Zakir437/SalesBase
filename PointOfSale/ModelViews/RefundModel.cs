using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class RefundModel
    {
        public int? ProductId { get; set; }
        public decimal? RefundQuantity { get; set; }
    }
}