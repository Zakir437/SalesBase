namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Offer")]
    public partial class Offer
    {
        public long Id { get; set; }

        public long? MasterOfferId { get; set; }

        public int? SubOfferId { get; set; }

        public int? ScheduleId { get; set; }

        public int? Type { get; set; }

        [StringLength(50)]
        public string OfferName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsEditable { get; set; }

        public bool? Status { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }

        public decimal? ActualPrice { get; set; }

        public decimal? OfferPrice { get; set; }

        public decimal? DiscPercentage { get; set; }

        public decimal? DiscAmount { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
