namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewPosOrder")]
    public partial class ViewPosOrder
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }

        public long? RefundOrderId { get; set; }

        public long? NewOrderId { get; set; }

        public long? PurchaseOrderId { get; set; }

        public decimal? DueAmount { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal AmountPaid { get; set; }

        public decimal? ReturnAmount { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal Amount { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(50)]
        public string CustomerName { get; set; }

        [StringLength(101)]
        public string RefundBy { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool IsDebit { get; set; }

        [StringLength(50)]
        public string POVoucher { get; set; }

        [StringLength(50)]
        public string ASReff { get; set; }

        public long? ASReffId { get; set; }

        public int? OrderType { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        public decimal? ItemDiscount { get; set; }

        public decimal? SubTotalBeforeDiscount { get; set; }

        public decimal? DiscValue { get; set; }

        public decimal? DiscWorkValue { get; set; }

        public decimal? CouponDiscount { get; set; }

        public bool? CouponDiscType { get; set; }

        public decimal? CouponDiscValue { get; set; }

        public decimal? CoupDiscWorkValue { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal Tax { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaxPercent { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool TaxFunc { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? DelChargeId { get; set; }

        public decimal? DelChargeAmount { get; set; }

        public bool? DelChargeType { get; set; }

        public decimal? DeliveryCharge { get; set; }

        public decimal? DelChargeBeforeDisc { get; set; }

        public int? DelDiscPercent { get; set; }

        public decimal? DelDiscValue { get; set; }

        public int? CouponDelPercent { get; set; }

        public decimal? CouponDelValue { get; set; }

        public int? DelOfferType { get; set; }

        public long? CouponId { get; set; }

        public int? CouponType { get; set; }

        public int? InsertType { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal InvoiceAmount { get; set; }

        public long? CustomerId { get; set; }

        [Key]
        [Column(Order = 9)]
        public bool Status { get; set; }

        public bool? IsItemRefund { get; set; }

        public bool? IsServiceItem { get; set; }

        public bool? RefundStatus { get; set; }

        public bool? IsPointBasedItem { get; set; }

        public int? TotalPoints { get; set; }

        public DateTime? RefundDateTime { get; set; }

        [Key]
        [Column(Order = 10)]
        public DateTime OrderDate { get; set; }

        [Key]
        [Column(Order = 11)]
        public decimal SubTotalPrice { get; set; }

        public long? DelDiscId { get; set; }

        public long? CouponDelId { get; set; }

        public long? CouponDiscountId { get; set; }

        public long? DiscountId { get; set; }

        public decimal? OrderAmountAfterDiscount { get; set; }

        public decimal? OrderAmountDiscount { get; set; }

        public decimal? InvoiceTotalDiscount { get; set; }

        public int? OfferType { get; set; }

        [Key]
        [Column(Order = 12)]
        public decimal OrderAmount { get; set; }

        public decimal? Discount { get; set; }

        public bool? DiscountType { get; set; }

        [StringLength(50)]
        public string OfferName { get; set; }

        [StringLength(50)]
        public string Coupon { get; set; }
    }
}
