namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewDailyAttendance")]
    public partial class ViewDailyAttendance
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [StringLength(101)]
        public string InsertedBy { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime InsertedDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AttType { get; set; }

        [StringLength(50)]
        public string LeaveTypeName { get; set; }

        public int? LeaveType { get; set; }

        [StringLength(101)]
        public string UserName { get; set; }

        public string Picture { get; set; }

        public int? IsPaidLeave { get; set; }
    }
}
