namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewStockImport")]
    public partial class ViewStockImport
    {
        [Key]
        [Column(Order = 0)]
        public long StockImportId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RefferenceNo { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal TotalCost { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool IsCredit { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal DueAmount { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal PaidAmount { get; set; }

        public bool? IsReturn { get; set; }

        [StringLength(101)]
        public string ImportBy { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SupplierId { get; set; }

        [StringLength(50)]
        public string SupplierName { get; set; }

        [StringLength(50)]
        public string MRR { get; set; }
    }
}
