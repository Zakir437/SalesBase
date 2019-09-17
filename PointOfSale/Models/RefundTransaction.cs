namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RefundTransaction")]
    public partial class RefundTransaction
    {
        public long Id { get; set; }

        public long RefundId { get; set; }

        public decimal PreviousAmount { get; set; }

        public decimal RefundAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public decimal PreviousQuantity { get; set; }

        public decimal RefundQuantity { get; set; }

        public int ProductId { get; set; }
    }
}
