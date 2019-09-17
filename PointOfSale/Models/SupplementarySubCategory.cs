namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SupplementarySubCategory")]
    public partial class SupplementarySubCategory
    {
        public int Id { get; set; }

        public int SupplementaryId { get; set; }

        public int SubCategoryId { get; set; }

        public bool Status { get; set; }
    }
}
