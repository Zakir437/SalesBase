using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class OrderModel
    {
        public long? OrderId { get; set; }
        public int? MethodId { get; set; }
        public int? SalesmanId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public IList<OrderItemModel> OrderProducts { get; set; }
        public IList<PaymentModel> Payments { get; set; }

        public decimal OrderAmount { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public decimal OrderAmountBeforeDiscount { get; set; }

        public decimal DiscountItem { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal SubTotalBeforeDiscount { get; set; }
        //discount
        public decimal Discount { get; set; }
        public int DiscType { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountWorkAmount { get; set; }
        public long DiscountId { get; set; }
        //coupon discount
        public decimal CouponDiscount { get; set; }
        public int CouponDiscType { get; set; }
        public decimal CouponDiscPercent { get; set; }
        public decimal CouponDiscountAmount { get; set; }
        public decimal CouponDiscWorkAmount { get; set; }
        public long CouponDiscId { get; set; }
        //Tax
        public decimal Tax { get; set; }
        public int TaxPercent { get; set; }
        public int? TaxFunc { get; set; }
        public decimal TotalPrice { get; set; }
        //Delivery Charge
        public decimal DeliveryCharge { get; set; }
        public decimal DelChargeBeforeDisc { get; set; }
        public decimal DeliveryChargeAmount { get; set; }
        public int DelChargeType { get; set; }
        public decimal DeliveryChargePercent { get; set; }
        public int DeliveryChargeId { get; set; }
        //Delivery charge Discount
        public int DelChargeDiscPercent { get; set; }
        public decimal DeliveryChargeDiscount { get; set; }
        public long DelDiscId { get; set; }
        //Coupon Delivery charge discount
        public int CouponDelChargeDiscPercent { get; set; }
        public decimal CouponDelChargeDiscount { get; set; }
        public long CouponDelivId { get; set; }

        public decimal InvoiceAmount { get; set; }

        public decimal? AfterDiscPrice { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? PaymentBodyId { get; set; }
        public decimal? AmountPaid { get; set; }
        public string TransactionNo { get; set; }
        public decimal? ReturnAmount { get; set; }
        //customer info
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool? IsCreditAllow { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? AvailableCreditAmount { get; set; }
        public bool? IsDebitAllow { get; set; }
        public decimal? DebitAmount { get; set; }

        //Service and Points
        public bool IsServiceItems { get; set; }
        public bool IsPointItems { get; set; }
        public int TotalPoints { get; set; }

        //refund pay
        public string InvoiceNo { get; set; }
        public decimal InvoiceTotal { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal PayAmount { get; set; }

        //coupon offer
        public long OfferId { get; set; }
        public string OfferName { get; set; }
        public string Coupon { get; set; }
        public string CouponAmount { get; set; }
        public int CouponType { get; set; }

        public int CreatedBy { get; set; }

    }
}