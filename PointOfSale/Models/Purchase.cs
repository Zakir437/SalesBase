namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Purchase")]
    public partial class Purchase
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string VoucherName { get; set; }

        public decimal? Paidamount { get; set; }

        public decimal? DueAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        public int TaxPercent { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal? RemainingAmount { get; set; }

        public decimal? DispatchAmount { get; set; }

        public decimal? RefundAmount { get; set; }

        public int Status { get; set; }

        public DateTime DeliveryDate { get; set; }

        public long CustomerId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }
    }
}
