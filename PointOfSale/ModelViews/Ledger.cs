using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class Ledger
    {
        public long? ImportId { get; set; }
        public long? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Name { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal Amount { get; set; }
        public bool IsDebit { get; set; }
        public decimal? Due { get; set; }
        public DateTime Date { get; set; }
    }
}