namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PosOrder")]
    public partial class PosOrder
    {
        [Key]
        public long OrderId { get; set; }

        public long? RefundOrderId { get; set; }

        public long? NewOrderId { get; set; }

        public long? PurchaseOrderId { get; set; }

        public long? ASReffId { get; set; }

        public int? OrderType { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        public decimal OrderAmount { get; set; }

        public decimal? OrderAmountAfterDiscount { get; set; }

        public decimal? OrderAmountDiscount { get; set; }

        public decimal? InvoiceTotalDiscount { get; set; }

        public decimal? ItemDiscount { get; set; }

        public decimal SubTotalPrice { get; set; }

        public decimal? SubTotalBeforeDiscount { get; set; }

        public decimal? Discount { get; set; }

        public bool? DiscountType { get; set; }

        public decimal? DiscValue { get; set; }

        public decimal? DiscWorkValue { get; set; }

        public long? DiscountId { get; set; }

        public decimal? CouponDiscount { get; set; }

        public bool? CouponDiscType { get; set; }

        public decimal? CouponDiscValue { get; set; }

        public decimal? CoupDiscWorkValue { get; set; }

        public long? CouponDiscountId { get; set; }

        public int? OfferType { get; set; }

        public decimal Tax { get; set; }

        public int TaxPercent { get; set; }

        public bool TaxFunc { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? DelChargeId { get; set; }

        public decimal? DelChargeAmount { get; set; }

        public bool? DelChargeType { get; set; }

        public decimal? DeliveryCharge { get; set; }

        public decimal? DelChargeBeforeDisc { get; set; }

        public int? DelDiscPercent { get; set; }

        public decimal? DelDiscValue { get; set; }

        public long? DelDiscId { get; set; }

        public int? CouponDelPercent { get; set; }

        public decimal? CouponDelValue { get; set; }

        public long? CouponDelId { get; set; }

        public int? DelOfferType { get; set; }

        public long? CouponId { get; set; }

        public int? CouponType { get; set; }

        public int? InsertType { get; set; }

        public int CreatedBy { get; set; }

        public decimal InvoiceAmount { get; set; }

        public long PaymentId { get; set; }

        public long? CustomerId { get; set; }

        public bool Status { get; set; }

        public bool? RefundStatus { get; set; }

        public bool? IsItemRefund { get; set; }

        public bool? IsServiceItem { get; set; }

        public bool? IsPointBasedItem { get; set; }

        public int? TotalPoints { get; set; }

        public int? RefundBy { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
