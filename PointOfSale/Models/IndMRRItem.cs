namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IndMRRItem")]
    public partial class IndMRRItem
    {
        public long Id { get; set; }

        public long MRRId { get; set; }

        public long IndentId { get; set; }

        public long WOId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public int Status { get; set; }
    }
}
