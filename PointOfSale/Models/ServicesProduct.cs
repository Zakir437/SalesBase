namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServicesProduct")]
    public partial class ServicesProduct
    {
        public long Id { get; set; }

        public long AssId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        public int? ConditionId { get; set; }

        [StringLength(200)]
        public string ConditionObservation { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
