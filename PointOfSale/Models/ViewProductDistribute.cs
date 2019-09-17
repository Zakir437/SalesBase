namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewProductDistribute")]
    public partial class ViewProductDistribute
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        public int? SizeId { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        public int? ColorId { get; set; }

        [StringLength(20)]
        public string ColorName { get; set; }

        public decimal? Price { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string BarCode { get; set; }

        public int? SizeType { get; set; }

        public int? MinimumQuantity { get; set; }

        [StringLength(100)]
        public string PLU { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        public decimal? Cost { get; set; }

        [StringLength(100)]
        public string Unit { get; set; }
    }
}
