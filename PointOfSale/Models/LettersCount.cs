namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LettersCount")]
    public partial class LettersCount
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Letters { get; set; }

        public int Count { get; set; }
    }
}
