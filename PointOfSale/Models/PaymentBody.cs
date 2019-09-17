namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PaymentBody")]
    public partial class PaymentBody
    {
        public int PaymentBodyId { get; set; }

        public int PaymentTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public decimal? Balance { get; set; }

        public int? PaymentMethod { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }

        public int InOut { get; set; }

        public bool Status { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
