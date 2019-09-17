namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GenerateDate")]
    public partial class GenerateDate
    {
        public long Id { get; set; }

        public int Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
