namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InventoryDailyDataSet")]
    public partial class InventoryDailyDataSet
    {
        public long Id { get; set; }

        public int TotalItemSold { get; set; }

        public int TotalItemReturned { get; set; }

        public int TotalItemBorrowed { get; set; }

        public int TotalItemWasted { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
