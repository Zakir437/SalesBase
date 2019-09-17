namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AmountCoupon")]
    public partial class AmountCoupon
    {
        public long Id { get; set; }

        public long OfferId { get; set; }

        public int Amount { get; set; }

        public bool IsPercentile { get; set; }

        public bool IsInifinte { get; set; }

        public int FromPrice { get; set; }

        public int ToPrice { get; set; }
    }
}
