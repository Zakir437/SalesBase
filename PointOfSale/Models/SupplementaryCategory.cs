namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SupplementaryCategory")]
    public partial class SupplementaryCategory
    {
        public int Id { get; set; }

        public int SupplementaryId { get; set; }

        public int CategoryId { get; set; }

        public bool Status { get; set; }
    }
}
