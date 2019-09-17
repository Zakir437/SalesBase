namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewStockDailySold")]
    public partial class ViewStockDailySold
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public int? ProductId { get; set; }

        public long? DistributeId { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool ProductCategoryStatus { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string SubCategoryName { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubCategoryId { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool ProductSubCategoryStatus { get; set; }
    }
}
