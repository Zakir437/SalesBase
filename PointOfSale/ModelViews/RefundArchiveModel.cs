using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class RefundArchiveModel
    {
        public long OrderId { get; set; }
        public DateTime Date { get; set; }
    }
}