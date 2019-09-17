namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewSchedule")]
    public partial class ViewSchedule
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool IsMultiSchedule { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string Name { get; set; }

        public bool? Status { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }
    }
}
