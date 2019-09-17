namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewAccount")]
    public partial class ViewAccount
    {
        [Key]
        [Column(Order = 0)]
        public int PaymentBodyId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string PaymentBodyName { get; set; }

        [StringLength(50)]
        public string PaymentTypeName { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InOut { get; set; }

        public decimal? Balance { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
