namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewDeliveryChargeCoupon")]
    public partial class ViewDeliveryChargeCoupon
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OfferId { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool IsPriceRange { get; set; }

        public int? FromPrice { get; set; }

        public int? ToPrice { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Percentage { get; set; }

        public int? Type { get; set; }

        [StringLength(50)]
        public string CouponCode { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [StringLength(50)]
        public string OfferName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? Status { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }
    }
}
