namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ViewBorrowProduct
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderTransactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal Quantity { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal Price { get; set; }

        public bool? IsBorrow { get; set; }

        public bool? IsBorrowPaid { get; set; }

        [StringLength(50)]
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(50)]
        public string AssociateName { get; set; }

        public long? AssociateId { get; set; }
    }
}
