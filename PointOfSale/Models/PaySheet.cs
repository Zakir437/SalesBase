namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PaySheet")]
    public partial class PaySheet
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionNo { get; set; }

        public int AssignUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Filename { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }

        public bool IsApproved { get; set; }

        public DateTime GenerateDate { get; set; }

        public int GenerateBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        public int? PaidBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PaidDate { get; set; }

        public int? ApprovedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ApprovedDate { get; set; }
    }
}
