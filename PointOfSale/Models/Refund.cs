namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Refund")]
    public partial class Refund
    {
        public long RefundId { get; set; }

        public long OrderId { get; set; }

        public decimal RefundAmount { get; set; }

        public int RefundBy { get; set; }

        public DateTime Date { get; set; }
    }
}
