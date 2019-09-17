namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServiceSubCategory")]
    public partial class ServiceSubCategory
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public int SubCategoryId { get; set; }

        public bool Status { get; set; }
    }
}
