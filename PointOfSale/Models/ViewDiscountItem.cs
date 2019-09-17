namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewDiscountItem")]
    public partial class ViewDiscountItem
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool IsFree { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal PercentageOff { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal AmountOff { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal Price { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal OfferPrice { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 10)]
        public DateTime EndDate { get; set; }

        [Key]
        [Column(Order = 11)]
        public bool IsEditable { get; set; }

        public bool? Status { get; set; }

        public int? ScheduleId { get; set; }

        [StringLength(50)]
        public string ScheduleName { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 12)]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
