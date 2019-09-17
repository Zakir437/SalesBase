namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewService")]
    public partial class ViewService
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal ServiceCharge { get; set; }

        public bool? Status { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public string Categorys { get; set; }

        public string SubCategorys { get; set; }
    }
}