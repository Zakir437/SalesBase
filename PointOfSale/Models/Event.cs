namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EventName { get; set; }

        public int SubId { get; set; }

        public int ScheduleId { get; set; }

        public DateTime Validity { get; set; }

        public bool? Status { get; set; }

        public bool? IsCouponApplicable { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
