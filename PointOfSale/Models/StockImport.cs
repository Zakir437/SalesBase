namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockImport")]
    public partial class StockImport
    {
        public long StockImportId { get; set; }

        [Required]
        [StringLength(50)]
        public string RefferenceNo { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public decimal? ReturnAmount { get; set; }

        public decimal TotalCost { get; set; }

        public bool IsCredit { get; set; }

        public DateTime Date { get; set; }

        public long SupplierId { get; set; }

        public int ImportBy { get; set; }

        public bool? IsReturn { get; set; }

        public int? ReturnBy { get; set; }

        public DateTime? ReturnDate { get; set; }

        public int? BranchId { get; set; }

        public int? CompanyId { get; set; }

        public long? MRRId { get; set; }
    }
}
