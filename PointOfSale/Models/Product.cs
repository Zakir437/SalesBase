namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        public int ProductId { get; set; }

        public int? SubMasterId { get; set; }

        [StringLength(50)]
        public string QuantityOrName { get; set; }

        public bool? IsQuantity { get; set; }

        public int? MasterUnit { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(100)]
        public string BarCode { get; set; }

        [StringLength(50)]
        public string PLU { get; set; }

        public int Unit { get; set; }

        public decimal Cost { get; set; }

        public decimal Price { get; set; }

        public decimal? MinimalQuantity { get; set; }

        public int? VatRateId { get; set; }

        public bool IsVatIncluded { get; set; }

        public bool IsAfterSaleService { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? ServiceDays { get; set; }

        public bool IsPriceChangeAllow { get; set; }

        public bool IsDiscountAllow { get; set; }

        public bool IsRefundAllow { get; set; }

        public bool IsUnitWise { get; set; }

        public bool Isperishable { get; set; }

        public int? ExpireDays { get; set; }

        public bool IsFixed { get; set; }

        public bool? IsWarrenty { get; set; }

        public int? WarrentyTime { get; set; }

        public bool IsUsingDefaultQuantity { get; set; }

        public int? RestrictionId { get; set; }

        public bool IsPointBased { get; set; }

        public int? Points { get; set; }

        public bool IsUniqueItem { get; set; }

        public bool Status { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime DateTime { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [Required]
        [StringLength(500)]
        public string Image { get; set; }

        public bool IsDynamic { get; set; }

        public bool SizeCheckBox { get; set; }

        public bool ColorCheckBox { get; set; }

        public bool PriceCheckBox { get; set; }

        public bool CostCheckBox { get; set; }

        public bool PluCheckbox { get; set; }

        public bool CodeCheckbox { get; set; }

        public bool MinimumQuantityCheckbox { get; set; }

        public int? SizeType { get; set; }
    }
}
