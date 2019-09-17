namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PosOrderTransaction")]
    public partial class PosOrderTransaction
    {
        [Key]
        public long OrderTransactionId { get; set; }

        public long OrderId { get; set; }

        public long? PurchaseOrderId { get; set; }

        public long? POItemId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public long? CustomerId { get; set; }

        public int TransactionType { get; set; }

        public int? RoundNumber { get; set; }

        public decimal PerItemPrice { get; set; }

        public decimal? PerItemTotal { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public decimal OrderedQuantity { get; set; }

        public int? IsLocked { get; set; }

        public int? OfferType { get; set; }

        public decimal? Discount { get; set; }

        public bool? DiscountType { get; set; }

        public decimal? DiscValue { get; set; }

        public int? DiscId { get; set; }

        public decimal? CouponDiscount { get; set; }

        public bool? CouponDiscType { get; set; }

        public decimal? CouponDiscValue { get; set; }

        public long? CouponDiscId { get; set; }

        public int? SubOfferId { get; set; }

        public int? IsFeatured { get; set; }

        public int? VoidedBy { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime? VoidDateTime { get; set; }

        public bool? IsRefundAllow { get; set; }

        public int? RefundStatus { get; set; }

        public int? RefundBy { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public decimal? RefundQuantity { get; set; }

        public bool? IsBorrow { get; set; }

        public long? AssociateId { get; set; }

        public bool? IsBorrowPaid { get; set; }

        public bool? IsAfterSaleService { get; set; }

        public int? ServiceDays { get; set; }

        [StringLength(50)]
        public string ServiceName { get; set; }

        public int? ServiceTypeId { get; set; }

        public bool? IsPointBased { get; set; }

        public int? Points { get; set; }

        public bool IsUniqueItem { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        public bool Status { get; set; }
    }
}
