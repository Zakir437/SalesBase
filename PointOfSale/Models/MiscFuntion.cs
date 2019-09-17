namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MiscFuntion")]
    public partial class MiscFuntion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool? Status { get; set; }

        public int? Value { get; set; }

        public int? TaxRate { get; set; }

        public int? MinimumQuantity { get; set; }

        public int? SalaryPayDay { get; set; }

        public TimeSpan? Starttime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool? Is24hours { get; set; }

        public int? Age { get; set; }

        public decimal? PointsValue { get; set; }

        public int? Points { get; set; }
    }
}
