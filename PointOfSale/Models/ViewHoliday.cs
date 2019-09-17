namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewHoliday")]
    public partial class ViewHoliday
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int HolidayPackId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string HolidayName { get; set; }

        public bool? IsMultipleDay { get; set; }

        public int? MonthCount { get; set; }

        [StringLength(50)]
        public string MonthName { get; set; }

        public int? TotalDay { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(10)]
        public string Year { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
