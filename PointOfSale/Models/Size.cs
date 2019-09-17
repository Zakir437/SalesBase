namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Size")]
    public partial class Size
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SizeName { get; set; }

        public bool Type { get; set; }
    }
}
