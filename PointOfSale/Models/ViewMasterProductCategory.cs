namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewMasterProductCategory")]
    public partial class ViewMasterProductCategory
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MasterProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool MasterCategoryStatus { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubCategoryId { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool MasterSubCategoryStatus { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(100)]
        public string SubCategoryName { get; set; }

        public bool? SubCategoryStatus { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MasterSubCategoryId { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public bool? CategoryStatus { get; set; }
    }
}
