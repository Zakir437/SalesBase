namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ViewProduct
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RowID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [StringLength(274)]
        public string ProductName { get; set; }

        public decimal? Price { get; set; }

        public decimal? Cost { get; set; }

        public decimal? MinimumQuantity { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime DateTime { get; set; }

        [StringLength(50)]
        public string MasterProductName { get; set; }

        [StringLength(100)]
        public string MasterUnit { get; set; }

        [StringLength(50)]
        public string QuantityOrName { get; set; }

        public bool? IsQuantity { get; set; }

        public int? SubMasterId { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool IsPointBased { get; set; }

        public int? Points { get; set; }

        [StringLength(50)]
        public string ServiceName { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool IsAfterSaleService { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? ServiceDays { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool Isperishable { get; set; }

        public int? ExpireDays { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool IsFixed { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool IsUsingDefaultQuantity { get; set; }

        [Key]
        [Column(Order = 8)]
        public bool IsUnitWise { get; set; }

        [Key]
        [Column(Order = 9)]
        public bool IsRefundAllow { get; set; }

        [Key]
        [Column(Order = 10)]
        public bool IsDiscountAllow { get; set; }

        [Key]
        [Column(Order = 11)]
        public bool IsPriceChangeAllow { get; set; }

        [Key]
        [Column(Order = 12)]
        public bool IsUniqueItem { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        public string Unit { get; set; }

        [Key]
        [Column(Order = 13)]
        public bool IsDynamic { get; set; }

        public long? DistributeId { get; set; }

        [Key]
        [Column(Order = 14)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 15)]
        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(100)]
        public string Barcode { get; set; }

        public string Categorys { get; set; }

        public string SubCategorys { get; set; }

        public decimal? OfferPrice { get; set; }

        public int? OfferId { get; set; }

        public decimal? PercentageOff { get; set; }

        public decimal? AmountOff { get; set; }
    }
}
