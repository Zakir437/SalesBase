using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class PaymentModel
    {
        public int PaymentTypeId { get; set; }
        public int PaymentBodyId { get; set; }
        public decimal AmountPaid { get; set; }
        public string TransactionNo { get; set; }
    }
}