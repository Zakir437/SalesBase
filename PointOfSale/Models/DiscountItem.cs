namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiscountItem")]
    public partial class DiscountItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public bool IsFree { get; set; }

        public decimal PercentageOff { get; set; }

        public decimal AmountOff { get; set; }

        public decimal Price { get; set; }

        public decimal OfferPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? ScheduleId { get; set; }

        public bool IsEditable { get; set; }

        public bool? Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
