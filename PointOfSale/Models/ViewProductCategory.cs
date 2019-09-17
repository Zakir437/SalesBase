namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewProductCategory")]
    public partial class ViewProductCategory
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool ProductCategoryStatus { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(100)]
        public string SubCategoryName { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubCategoryId { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool ProductSubCategoryStatus { get; set; }
    }
}
