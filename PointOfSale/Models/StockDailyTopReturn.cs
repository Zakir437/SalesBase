namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockDailyTopReturn")]
    public partial class StockDailyTopReturn
    {
        public long Id { get; set; }

        public int ProductId { get; set; }

        public long? DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
