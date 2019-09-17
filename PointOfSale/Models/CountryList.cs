namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CountryList")]
    public partial class CountryList
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryCode { get; set; }

        [StringLength(50)]
        public string DialingCode { get; set; }

        [StringLength(100)]
        public string Nationality { get; set; }
    }
}
