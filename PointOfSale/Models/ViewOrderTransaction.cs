namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewOrderTransaction")]
    public partial class ViewOrderTransaction
    {
        [Key]
        [Column(Order = 0)]
        public long OrderTransactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal PerItemPrice { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal Quantity { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal OrderedQuantity { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public bool? DiscountType { get; set; }

        public bool? IsRefundAllow { get; set; }

        public int? RefundStatus { get; set; }

        public int? RefundBy { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public decimal? RefundQuantity { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionType { get; set; }

        [StringLength(50)]
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }

        public bool? IsBorrow { get; set; }

        public bool? IsBorrowPaid { get; set; }

        public bool? IsAfterSaleService { get; set; }

        public int? ServiceDays { get; set; }

        public bool? IsPointBased { get; set; }

        public int? Points { get; set; }

        [StringLength(50)]
        public string ServiceName { get; set; }

        public int? ServiceTypeId { get; set; }

        public long? CustomerId { get; set; }

        [StringLength(50)]
        public string CustomerName { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        [Key]
        [Column(Order = 9)]
        public bool IsUniqueItem { get; set; }

        [Key]
        [Column(Order = 10)]
        public bool Status { get; set; }

        public long? AssociateId { get; set; }

        public long? PurchaseOrderId { get; set; }

        public long? POItemId { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        public decimal? PerItemTotal { get; set; }

        public decimal? DiscValue { get; set; }

        public int? OfferType { get; set; }

        public decimal? CouponDiscount { get; set; }

        public bool? CouponDiscType { get; set; }

        public decimal? CouponDiscValue { get; set; }

        public int? DiscId { get; set; }

        public long? CouponDiscId { get; set; }

        public int? SubOfferId { get; set; }
    }
}
