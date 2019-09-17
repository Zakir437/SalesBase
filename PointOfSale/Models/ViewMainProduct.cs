namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ViewMainProduct
    {
        [Key]
        [Column(Order = 0)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal Cost { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal Price { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool IsDynamic { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime DateTime { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Categorys { get; set; }

        public string SubCategorys { get; set; }

        [StringLength(100)]
        public string Unit { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(500)]
        public string Image { get; set; }
    }
}
