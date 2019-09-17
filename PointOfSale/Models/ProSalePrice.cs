namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProSalePrice")]
    public partial class ProSalePrice
    {
        public long Id { get; set; }

        public int ProductId { get; set; }

        public long? DistributeId { get; set; }

        public decimal Price { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
