namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccType")]
    public partial class AccType
    {
        public int AccTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
