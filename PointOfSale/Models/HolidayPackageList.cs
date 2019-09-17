namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HolidayPackageList")]
    public partial class HolidayPackageList
    {
        [Key]
        public int HolidayPackId { get; set; }

        [Required]
        [StringLength(100)]
        public string HolidayPackName { get; set; }

        public int NoOfPaidLeave { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
