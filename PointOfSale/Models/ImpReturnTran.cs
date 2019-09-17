namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ImpReturnTran
    {
        public long Id { get; set; }

        public long ReturnId { get; set; }

        public decimal Quantity { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public long DistributeId { get; set; }

        public decimal PeritemCost { get; set; }

        public decimal Cost { get; set; }
    }
}
