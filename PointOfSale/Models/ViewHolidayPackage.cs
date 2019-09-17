namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewHolidayPackage")]
    public partial class ViewHolidayPackage
    {
        [Key]
        [Column(Order = 0)]
        public int HolidayPackId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string HolidayPackName { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NoOfPaidLeave { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }
    }
}
