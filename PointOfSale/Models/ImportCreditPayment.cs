namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImportCreditPayment")]
    public partial class ImportCreditPayment
    {
        [Key]
        public long PaymentId { get; set; }

        public long StockImportId { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }
    }
}
