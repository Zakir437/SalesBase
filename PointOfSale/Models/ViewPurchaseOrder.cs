namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewPurchaseOrder")]
    public partial class ViewPurchaseOrder
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string VoucherName { get; set; }

        public decimal? Paidamount { get; set; }

        public decimal? DueAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime DeliveryDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CustomerId { get; set; }

        [StringLength(50)]
        public string CustomerName { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        [StringLength(101)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public decimal? RemainingAmount { get; set; }

        public decimal? DispatchAmount { get; set; }

        public decimal? RefundAmount { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaxPercent { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal TaxAmount { get; set; }
    }
}
