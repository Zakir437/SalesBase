namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewCategory")]
    public partial class ViewCategory
    {
        [Key]
        [Column(Order = 0)]
        public int CategoryId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public bool? Status { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public int? RestrictionId { get; set; }

        [StringLength(50)]
        public string Restricion { get; set; }
    }
}
