namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeliveryChargeCoupon")]
    public partial class DeliveryChargeCoupon
    {
        public int Id { get; set; }

        public long OfferId { get; set; }

        public bool IsPriceRange { get; set; }

        public int? FromPrice { get; set; }

        public int? ToPrice { get; set; }

        public int Percentage { get; set; }
    }
}
