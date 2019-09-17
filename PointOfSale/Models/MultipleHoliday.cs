namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MultipleHoliday")]
    public partial class MultipleHoliday
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string MonthName { get; set; }

        public int MonthCount { get; set; }
    }
}
