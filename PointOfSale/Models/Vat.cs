namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vat")]
    public partial class Vat
    {
        public int VatId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Rate { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public int? Type { get; set; }

        public int? Status { get; set; }

        public int? CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }

        public int? UpdateBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdateDate { get; set; }
    }
}
