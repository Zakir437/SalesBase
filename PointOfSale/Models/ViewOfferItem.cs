namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewOfferItem")]
    public partial class ViewOfferItem
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OfferId { get; set; }

        public int? SubOfferId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime EndDate { get; set; }

        [StringLength(100)]
        public string ProductName { get; set; }

        public decimal? ProductPrice { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool IsFree { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal PercentageOff { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal AmountOff { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal Quantity { get; set; }

        [Key]
        [Column(Order = 9)]
        public decimal Price { get; set; }

        public bool? Status { get; set; }

        public int? Type { get; set; }

        public long? MasterOfferId { get; set; }

        public string Categorys { get; set; }

        [StringLength(50)]
        public string ScheduleName { get; set; }
    }
}
