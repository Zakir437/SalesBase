namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DivisionList")]
    public partial class DivisionList
    {
        [Key]
        public int DivisionId { get; set; }

        [Required]
        [StringLength(50)]
        public string DivisionName { get; set; }
    }
}
