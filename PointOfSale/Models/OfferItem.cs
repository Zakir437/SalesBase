namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OfferItem")]
    public partial class OfferItem
    {
        public long Id { get; set; }

        public long OfferId { get; set; }

        public long? MasterOfferId { get; set; }

        public int? SubOfferId { get; set; }

        public int? ScheduleId { get; set; }

        public int? Type { get; set; }

        public bool? IsDependency { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public bool IsFree { get; set; }

        public decimal PercentageOff { get; set; }

        public decimal AmountOff { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public bool? Status { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
