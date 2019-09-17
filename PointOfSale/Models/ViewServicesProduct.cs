namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewServicesProduct")]
    public partial class ViewServicesProduct
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AssId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        public int? ConditionId { get; set; }

        [StringLength(200)]
        public string ConditionObservation { get; set; }

        [StringLength(50)]
        public string ConditionName { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime CreatedDate { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        [StringLength(50)]
        public string WorkOrderNo { get; set; }

        public int? ASStatus { get; set; }
    }
}
