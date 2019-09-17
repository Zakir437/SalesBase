namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewAccountName")]
    public partial class ViewAccountName
    {
        [Key]
        [Column(Order = 0)]
        public int AccId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CashType { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccType { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal Balance { get; set; }

        public decimal? TotalCreditLimit { get; set; }

        public decimal? TransactionHigestAmount { get; set; }

        public int? NoOfTranPerMonth { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        [StringLength(50)]
        public string AccTypeName { get; set; }
    }
}
