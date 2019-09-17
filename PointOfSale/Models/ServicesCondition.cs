namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServicesCondition")]
    public partial class ServicesCondition
    {
        public long Id { get; set; }

        public long ServicesProductId { get; set; }

        public int ConditionId { get; set; }

        [Required]
        [StringLength(200)]
        public string ConditionObservation { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }
    }
}
