namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WasteTransaction")]
    public partial class WasteTransaction
    {
        public long WasteTransactionId { get; set; }

        public long StockWasteId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        public decimal Quantity { get; set; }
    }
}
