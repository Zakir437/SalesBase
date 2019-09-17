namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PaymentTransaction")]
    public partial class PaymentTransaction
    {
        public long PaymentTransactionId { get; set; }

        public long PaymentId { get; set; }

        public long? CreditPaymentId { get; set; }

        public int Type { get; set; }

        public int PaymentTypeId { get; set; }

        public int PaymentBodyId { get; set; }

        public decimal Amount { get; set; }

        public bool InOut { get; set; }

        public int MethodId { get; set; }

        [StringLength(50)]
        public string TransactionNo { get; set; }

        public bool? IsCreditPayment { get; set; }

        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }

        public bool? Status { get; set; }

        public long? PaySheetId { get; set; }

        public long? RefOrderId { get; set; }

        public long? CustomerId { get; set; }

        public long? SupplierId { get; set; }

        public long? AssociateId { get; set; }
    }
}
