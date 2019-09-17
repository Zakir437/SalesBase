namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewWasteType")]
    public partial class ViewWasteType
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string TypeName { get; set; }

        public bool? Status { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Type { get; set; }
    }
}
