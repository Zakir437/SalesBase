namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductDistribute")]
    public partial class ProductDistribute
    {
        public long Id { get; set; }

        public int ProductId { get; set; }

        [StringLength(50)]
        public string SizeName { get; set; }

        public int? SizeId { get; set; }

        public int? SizeType { get; set; }

        public int? ColorId { get; set; }

        public decimal? Price { get; set; }

        public decimal? Cost { get; set; }

        [Required]
        [StringLength(100)]
        public string BarCode { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(100)]
        public string PLU { get; set; }

        public int? MinimumQuantity { get; set; }

        public bool Status { get; set; }
    }
}
