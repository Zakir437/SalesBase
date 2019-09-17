namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WImport")]
    public partial class WImport
    {
        public long Id { get; set; }

        public long WOrderId { get; set; }

        public long SupplierId { get; set; }

        [Required]
        [StringLength(50)]
        public string VoucherNo { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public int ImportBy { get; set; }

        public DateTime ImportDate { get; set; }
    }
}
