namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WImpTran
    {
        [Key]
        public long WImpTransId { get; set; }

        public long WImpId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal PeritemCost { get; set; }

        public decimal Cost { get; set; }
    }
}
