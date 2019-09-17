namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewOffer")]
    public partial class ViewOffer
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [StringLength(50)]
        public string OfferName { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool IsEditable { get; set; }

        public bool? Status { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        public int? SubOfferId { get; set; }

        public int? Type { get; set; }

        public long? MasterOfferId { get; set; }

        public int? ScheduleId { get; set; }

        [StringLength(50)]
        public string ScheduleName { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime EndDate { get; set; }

        public decimal? ActualPrice { get; set; }

        public decimal? OfferPrice { get; set; }

        public decimal? DiscPercentage { get; set; }

        public decimal? DiscAmount { get; set; }

        public int? DeliveryChargeCouponId { get; set; }

        public long? AmountCouponId { get; set; }
    }
}
