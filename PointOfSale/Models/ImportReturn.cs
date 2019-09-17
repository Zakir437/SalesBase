namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImportReturn")]
    public partial class ImportReturn
    {
        [Key]
        public long ReturnId { get; set; }

        public long ImportId { get; set; }

        public decimal TotalAmount { get; set; }

        public int ReturnBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
