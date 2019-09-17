namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockDailyReturn")]
    public partial class StockDailyReturn
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public long DistributeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal PerItemPrice { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public bool? DiscountType { get; set; }

        public decimal? DiscountPercent { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
