namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewImportTransaction")]
    public partial class ViewImportTransaction
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ImportTransactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal Quantity { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal Cost { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal PeritemCost { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        public bool? IsReturn { get; set; }

        [StringLength(101)]
        public string ReturnBy { get; set; }

        public DateTime? ReturnDate { get; set; }

        public decimal? ReturnQuantity { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long StockImportId { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(50)]
        public string RefferenceNo { get; set; }

        [StringLength(50)]
        public string SupplierName { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SupplierId { get; set; }
    }
}
