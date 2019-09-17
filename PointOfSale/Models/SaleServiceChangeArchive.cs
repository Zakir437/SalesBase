namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SaleServiceChangeArchive")]
    public partial class SaleServiceChangeArchive
    {
        public long Id { get; set; }

        public long SaleServiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }
    }
}
