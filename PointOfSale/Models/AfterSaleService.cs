namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AfterSaleService")]
    public partial class AfterSaleService
    {
        public long Id { get; set; }

        [StringLength(50)]
        public string WorkOrderNo { get; set; }

        [Required]
        [StringLength(50)]
        public string TemporaryWorkOrderNo { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; }

        public long OrderTransactionId { get; set; }

        public decimal? ServiceChargeTotal { get; set; }

        public int StepCounter { get; set; }

        public int Status { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
