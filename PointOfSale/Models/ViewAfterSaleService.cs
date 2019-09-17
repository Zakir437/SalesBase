namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewAfterSaleService")]
    public partial class ViewAfterSaleService
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [StringLength(50)]
        public string WorkOrderNo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string TemporaryWorkOrderNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string InvoiceNo { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderTransactionId { get; set; }

        public decimal? ServiceChargeTotal { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StepCounter { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 6, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }
    }
}
