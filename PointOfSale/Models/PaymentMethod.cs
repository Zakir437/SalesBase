namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PaymentMethod")]
    public partial class PaymentMethod
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string MethodName { get; set; }

        public int Type { get; set; }

        public bool Status { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
