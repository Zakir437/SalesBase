namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CountryOfOrigin_Table
    {
        [Key]
        public int COOId { get; set; }

        [StringLength(50)]
        public string COOName { get; set; }

        [StringLength(50)]
        public string CultureName { get; set; }

        public int? Status { get; set; }
    }
}
