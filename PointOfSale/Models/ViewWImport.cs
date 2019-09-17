namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewWImport")]
    public partial class ViewWImport
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long WOrderId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SupplierId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string VoucherNo { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal TotalAmount { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal PaidAmount { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal DueAmount { get; set; }

        [StringLength(101)]
        public string ImportBy { get; set; }

        [Key]
        [Column(Order = 7)]
        public DateTime ImportDate { get; set; }

        [StringLength(50)]
        public string SupplierName { get; set; }

        [StringLength(50)]
        public string WOVoucher { get; set; }
    }
}
