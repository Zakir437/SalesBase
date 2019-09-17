using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public long OrderTransactionId { get; set; }
        public long POItemId { get; set; }
        public int ProductId { get; set;}
        public long DistributeId { get; set; }
        public string ProductName { get; set; }
        public int TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal PerItemPrice { get; set; }
        public decimal PerItemTotalPrice { get; set; }
        public decimal Price { get; set; }
        //Discount
        public decimal Discount { get; set; }
        public decimal DiscountPercent { get; set; }
        public int DiscType { get; set; }
        public decimal DiscountAmount { get; set; }
        public int DiscountId { get; set; }
        //Coupon Discount
        public decimal CouponDiscount { get; set; }
        public decimal CouponDiscPercent { get; set; }
        public int CouponDiscType { get; set; }
        public decimal CouponDiscAmount { get; set; }
        public long CouponDiscId { get; set; }
        public int SubOfferId { get; set; }

        public int OfferType { get; set; }
        public long OfferId { get; set; }

        public bool IsRefundAllow { get; set; }
        public bool IsBorrow { get; set; }
        public int AssociateId { get; set; }
        public bool IsAfterSaleService { get; set; }
        public int ServiceDays { get; set; }
        public string ServiceName { get; set; }
        public int ServiceTypeId { get; set; }
        public bool IsPointBased { get; set; }
        public int Points { get; set; }
        public bool IsUniqueItem { get; set; }
        public string SerialNumber { get; set; }
    }
}