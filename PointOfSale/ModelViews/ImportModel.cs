using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class ImportModel
    {
       public int UserId { get; set; }
       public long ImportId { get; set; }
       public long WorkOrderId { get; set; }
       public int MethodId { get; set; }
       public IList<ImportModelItem> ImportData { get; set; }
       public IList<PaymentModel> Payments { get; set; }
       public string Comments { get; set; }
       public long SupplierId { get; set; }
       public long CustomerId { get; set; }
       public decimal CreditAmount { get; set; }
       
       //for purchase order
       public DateTime DeliveryDate { get; set; }
       public int TaxPercent { get; set; }
       public decimal TaxAmount { get; set; }
       public decimal TotalCost { get; set; }
    }
}