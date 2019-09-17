namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewSupplier")]
    public partial class ViewSupplier
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        public bool? Status { get; set; }

        [Key]
        [Column(Order = 2)]
        public long SupplierId { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SupplierSince { get; set; }

        public decimal? DebitAmount { get; set; }
    }
}
