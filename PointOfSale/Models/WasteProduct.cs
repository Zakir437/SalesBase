namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WasteProduct")]
    public partial class WasteProduct
    {
        public long Id { get; set; }

        public long WasteId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public int WasteTypeId { get; set; }

        public long? ASReffId { get; set; }

        public long? WHReffId { get; set; }

        public long? SupplierId { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        public int? Status { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
