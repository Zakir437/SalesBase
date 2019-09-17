namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IndentMRR")]
    public partial class IndentMRR
    {
        public long Id { get; set; }

        public long IndentId { get; set; }

        [Required]
        [StringLength(50)]
        public string MRR { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
