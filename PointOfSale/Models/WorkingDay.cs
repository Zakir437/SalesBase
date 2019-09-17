namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingDay")]
    public partial class WorkingDay
    {
        public int WorkingDayId { get; set; }

        public int WorkingScheduleId { get; set; }

        [Required]
        [StringLength(10)]
        public string Day { get; set; }
    }
}
