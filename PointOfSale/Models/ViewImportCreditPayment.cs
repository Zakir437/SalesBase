namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewImportCreditPayment")]
    public partial class ViewImportCreditPayment
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PaymentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SupplierId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string RefferenceNo { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal AmountPaid { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long StockImportId { get; set; }
    }
}
