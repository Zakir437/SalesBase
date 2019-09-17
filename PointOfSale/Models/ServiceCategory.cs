namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServiceCategory")]
    public partial class ServiceCategory
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public int CategoryId { get; set; }

        public bool Status { get; set; }
    }
}
