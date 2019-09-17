namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewWorkOrder")]
    public partial class ViewWorkOrder
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string VoucherName { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal TotalAmount { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal DueAmount { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SupplierId { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal PaidAmount { get; set; }

        [StringLength(50)]
        public string SupplierName { get; set; }
    }
}
