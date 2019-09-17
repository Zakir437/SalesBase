namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderPayment")]
    public partial class OrderPayment
    {
        public long OrderPaymentId { get; set; }

        public long? PreviousId { get; set; }

        public long OrderId { get; set; }

        public long? ReferenceOrderId { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal? DueAmount { get; set; }

        public bool IsDuePayment { get; set; }

        public decimal? ReturnAmount { get; set; }

        public bool Status { get; set; }

        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }
    }
}
