namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImportTransaction")]
    public partial class ImportTransaction
    {
        public long ImportTransactionId { get; set; }

        public long StockImportId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public long DistributeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal PeritemCost { get; set; }

        public decimal Cost { get; set; }

        public bool? IsReturn { get; set; }

        public int? ReturnBy { get; set; }

        public DateTime? ReturnDate { get; set; }

        public decimal? ReturnQuantity { get; set; }
    }
}
