namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewPaySheet")]
    public partial class ViewPaySheet
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Filename { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AssignUserId { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime GenerateDate { get; set; }

        [StringLength(253)]
        public string AssignUserName { get; set; }

        [StringLength(101)]
        public string GenerateBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal TotalAmount { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool IsPaid { get; set; }

        [Key]
        [Column(Order = 8)]
        public bool IsApproved { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(50)]
        public string TransactionNo { get; set; }
    }
}
