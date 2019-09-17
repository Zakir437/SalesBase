namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccountName")]
    public partial class AccountName
    {
        [Key]
        public int AccId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int CashType { get; set; }

        public int AccType { get; set; }

        public decimal Balance { get; set; }

        public decimal? TotalCreditLimit { get; set; }

        public decimal? TransactionHigestAmount { get; set; }

        public int? NoOfTranPerMonth { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
