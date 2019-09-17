namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReligionList")]
    public partial class ReligionList
    {
        [Key]
        public int ReligionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Religion { get; set; }
    }
}
