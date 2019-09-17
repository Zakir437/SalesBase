namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DailyAttendance")]
    public partial class DailyAttendance
    {
        public long Id { get; set; }

        public int UserId { get; set; }

        public int InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public int AttType { get; set; }

        public int? LeaveType { get; set; }

        public int? IsPaidLeave { get; set; }

        public bool? IsManuallyAtt { get; set; }

        public DateTime? ManuallyInsertedDate { get; set; }
    }
}
