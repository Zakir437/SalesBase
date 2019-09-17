namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewServicesCondition")]
    public partial class ViewServicesCondition
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ServicesProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ConditionId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string ConditionObservation { get; set; }

        [StringLength(50)]
        public string ConditionName { get; set; }

        public long? AssId { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "date")]
        public DateTime CreatedDate { get; set; }
    }
}
