namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewCreditCustomer")]
    public partial class ViewCreditCustomer
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        public decimal? DueAmount { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(50)]
        public string LastName { get; set; }

        [Key]
        [Column(Order = 8)]
        public DateTime OrderDate { get; set; }

        public long? PurchaseOrderId { get; set; }

        [StringLength(50)]
        public string POVoucher { get; set; }

        [Key]
        [Column(Order = 9)]
        public decimal InvoiceAmount { get; set; }
    }
}
