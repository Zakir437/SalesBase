namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkOrder")]
    public partial class WorkOrder
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string VoucherName { get; set; }

        public long SupplierId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DueAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
