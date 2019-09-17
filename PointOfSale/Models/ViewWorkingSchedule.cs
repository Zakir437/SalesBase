namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewWorkingSchedule")]
    public partial class ViewWorkingSchedule
    {
        [Key]
        [Column(Order = 0)]
        public int WorkingScheduleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string ScheduleName { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime CreadetDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
