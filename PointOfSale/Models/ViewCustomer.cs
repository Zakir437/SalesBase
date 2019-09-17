namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewCustomer")]
    public partial class ViewCustomer
    {
        [Key]
        [Column(Order = 0)]
        public long CustomerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string Email { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool IsCreditAllowed { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "date")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string MembershipNumber { get; set; }

        public int? CreditLimitId { get; set; }

        public decimal? CreditLimit { get; set; }

        public bool? Status { get; set; }

        public long? DebitLimitId { get; set; }

        public decimal? DebitLimit { get; set; }

        public decimal? DebitAmount { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }
    }
}
