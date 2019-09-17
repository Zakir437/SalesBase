using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class ImportModelItem
    {
        public long WOId { get; set; }
        public long WorkOrderItemId { get; set; }
        public long IndentId { get; set; }
        public string IndentVoucher { get; set; }
        public int ProductId { get; set; }
        public long DistributeId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PeritemCost { get; set; }
        public decimal Cost { get; set; }

        public string Comment { get; set; }
    }
}