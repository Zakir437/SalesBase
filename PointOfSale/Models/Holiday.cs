namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Holiday")]
    public partial class Holiday
    {
        public int Id { get; set; }

        public int HolidayPackId { get; set; }

        [Required]
        [StringLength(100)]
        public string HolidayName { get; set; }

        public bool? IsMultipleDay { get; set; }

        [StringLength(50)]
        public string MonthName { get; set; }

        public int? MonthCount { get; set; }

        public int? TotalDay { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(10)]
        public string Year { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
