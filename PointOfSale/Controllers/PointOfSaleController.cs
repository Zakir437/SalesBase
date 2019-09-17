using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Models;
using PointOfSale.ModelViews;
using System.Data.Entity;
using PointOfSale.Helpers;
using System.Configuration;
using PointOfSale.ModelViews.POS;

namespace PointOfSale.Controllers
{
    public class PointOfSaleController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        //*********************POS main page************************************
        #region Order
        //pos main page
        public ActionResult Index()
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("CookieUserInfo");
            if (cookie != null)
            {
                ViewBag.CustomerList = new SelectList(db.Customers.Where(a => a.Status == true).Select(s => new { s.CustomerId, s.Name }), "CustomerId", "Name");
                //Tax info
                ViewBag.Tax = db.MiscFuntions.FirstOrDefault(a => a.Id == 2).TaxRate;
                ViewBag.TaxStatus = 0;
                var taxStatus = db.MiscFuntions.FirstOrDefault(a => a.Id == 2).Status;
                if (taxStatus == true)
                {
                    ViewBag.TaxStatus = 1;
                }
                else
                {
                    ViewBag.Tax = 0;
                }
                var taxFunction = db.MiscFuntions.FirstOrDefault(a => a.Id == 4).Status;
                ViewBag.TaxFunc = 0;
                if (taxFunction == true)
                {
                    ViewBag.TaxFunc = 1;
                }
                //Delivery charge function 
                var deliveryFunc = db.MiscFuntions.FirstOrDefault(a => a.Id == 13).Status;
                ViewBag.DeliveryChargeIsAuto = 1; // 1 for auto
                if (deliveryFunc == false)
                {
                    ViewBag.DeliveryChargeIsAuto = 0; // 0 for manual
                }
                return this.View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        //pos main page product list
        public PartialViewResult ProductList(bool? isListView)
        {
            ViewBag.IsListView = isListView;
            var list = db.ViewProducts.Where(a => a.Status == true).Distinct().ToList();
            return PartialView(list);
        }
        public PartialViewResult PurchaseOrderProductList(long purchaseId, bool? isListView)
        {
            ViewBag.IsListView = isListView;
            return PartialView(db.ViewPurchaseOrderTransactions.Where(a => a.PurchaseId == purchaseId && a.Status == 1 && a.RemainingQty > 0).ToList());
        }
        //get product for add to cart search by product id/barcode/rowid 
        public JsonResult GetProduct(int? productId,long? distributeId, string barcode, long? rowId, int? categoryId, int? subCategoryId, int? fromPrice, int? toPrice, int? tagId)
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                if(db.ViewProducts.Any(a => a.Barcode == barcode))
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.Barcode == barcode);
                    return Json(new { product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem }, JsonRequestBehavior.AllowGet);
                }
            }
            else if(productId > 0)
            {
                if(distributeId > 0)
                {
                    if(db.ViewProducts.Any(a => a.DistributeId == distributeId))
                    {
                        return Json(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId && a.DistributeId == distributeId), JsonRequestBehavior.AllowGet);
                    }
                }
                else if(db.Products.Any(a => a.ProductId == productId))
                {
                    return Json(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId), JsonRequestBehavior.AllowGet);
                }
            }
            else if(rowId > 0)
            {
                if(db.ViewProducts.Any(a => a.RowID == rowId))
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    return Json(new { product.Price, product.Categorys, product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem, product.ProductName }, JsonRequestBehavior.AllowGet);
                }
            }
            else if(categoryId > 0)
            {
                var productList = new List<ViewProduct>();
                var productCategorys = new List<ViewProductCategory>();
                if (subCategoryId > 0)
                {
                    productCategorys = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true).ToList();
                }
                else
                {
                    productCategorys = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true).ToList();
                }
                var productIds = productCategorys.GroupBy(g => g.ProductId).Select(s => new { s.FirstOrDefault().ProductId }).ToList();
                if (productIds.Any())
                {
                    foreach(var id in productIds)
                    {
                        var products = db.ViewProducts.Where(a => a.ProductId == id.ProductId).ToList();
                        productList.AddRange(products);
                    }
                }

                if ((fromPrice >= 0) && (toPrice > 0) && (fromPrice < toPrice))
                {
                    productList = productList.Where(a => a.Price >= fromPrice && a.Price <= toPrice).ToList();
                }

                //if (subCategoryId > 0)
                //{
                //    productList = db.ViewProducts.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true && a.Status == true).ToList();
                //}
                //else
                //{
                //    productList = db.ViewProducts.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true && a.Status == true).ToList();
                //}
                //if ((fromPrice >= 0) && (toPrice > 0) && (fromPrice < toPrice))
                //{
                //    productList = productList.Where(a => a.Price >= fromPrice && a.Price <= toPrice).ToList();
                //}
                return Json(productList.OrderBy(a => a.Price).Select(s => new { s.ProductId, s.ProductName, s.Price, s.Points, s.DistributeId, s.RowID }).ToList(), JsonRequestBehavior.AllowGet);
            }
            else if (tagId > 0)
            {
                var productList = new List<ViewProduct>();
                var productIds = db.TagItems.Where( a=> a.TagId == tagId && a.Status == true && a.Type == 1).GroupBy(g => g.OwnerId).Select(s => new { s.FirstOrDefault().OwnerId }).ToList();
                if (productIds.Any())
                {
                    foreach (var id in productIds)
                    {
                        var products = db.ViewProducts.Where(a => a.ProductId == id.OwnerId && a.Status == true).ToList();
                        if(products != null)
                        {
                            productList.AddRange(products);
                        }
                    }
                }
                if (productList.Any())
                {
                    if ((fromPrice >= 0) && (toPrice > 0) && (fromPrice < toPrice))
                    {
                        productList = productList.Where(a => a.Price >= fromPrice && a.Price <= toPrice).ToList();
                    }
                    return Json(productList.OrderBy(a => a.Price), JsonRequestBehavior.AllowGet);
                }
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCouponList()
        {
            var list = new SelectList("", "");
            list = new SelectList(db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == true && a.StartDate <= DateTime.Now && DateTime.Now <= a.EndDate).Select(s => new { s.Id, s.Coupon }), "Id", "Coupon");
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCouponInfo(string couponCode, decimal invoiceAmount, decimal deliveryCharge, IList<OrderItemModel> orderProducts)
        {
            var offer = db.Offers.FirstOrDefault(a => a.IsCouponApplicable == true && a.Status == true && a.Coupon.ToLower() == couponCode.ToLower());
            if(offer != null)
            {
                if(offer.StartDate <= DateTime.Now && DateTime.Now <= offer.EndDate)
                {
                    if (offer.Type == 2) // product coupon
                    {
                        var offerItem = new List<OfferItem>();
                        var existingItem = new List<OfferItem>();
                        int quantity = 0;
                        if (offer.SubOfferId == 1 ) // B1G1
                        {
                            if (orderProducts != null)
                            {
                                var b1g1Products = db.OfferItems.FirstOrDefault(a => a.OfferId == offer.Id);
                                if(orderProducts.Any(a => a.ProductId == b1g1Products.ProductId && a.DistributeId == b1g1Products.DistributeId))
                                {
                                    var b1g1ExistingProduct = orderProducts.FirstOrDefault(a => a.ProductId == b1g1Products.ProductId && a.DistributeId == b1g1Products.DistributeId);
                                    quantity = (int)b1g1ExistingProduct.Quantity / 2;
                                    if(quantity > 0)
                                    {
                                        return Json(new { offer.OfferName, b1g1ExistingProduct.Id, quantity, Type = 2, subOfferId = 1, OfferId = offer.Id, offerItemId = b1g1Products.Id }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        return Json("notValid", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json("notValid", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if(offer.SubOfferId == 2) // B2G1
                        {
                            if (orderProducts != null)
                            {
                                var b12g1Products = db.OfferItems.FirstOrDefault(a => a.OfferId == offer.Id);

                                if (orderProducts.Any(a => a.ProductId == b12g1Products.ProductId && a.DistributeId == b12g1Products.DistributeId))
                                {
                                    var b2g1ExistingProduct = orderProducts.FirstOrDefault(a => a.ProductId == b12g1Products.ProductId && a.DistributeId == b12g1Products.DistributeId);
                                    quantity = (int)b2g1ExistingProduct.Quantity / 3;
                                    if (quantity > 0)
                                    {
                                        return Json(new { offer.OfferName, b2g1ExistingProduct.Id, quantity, Type = 2, subOfferId = 2, OfferId = offer.Id, offerItemId = b12g1Products.Id }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        return Json("notValid", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json("notValid", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if(orderProducts != null)
                            {
                                offerItem = db.OfferItems.Where(a => a.OfferId == offer.Id).ToList();
                                foreach(var item in offerItem)
                                {
                                    if(orderProducts.Any(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId))
                                    {
                                        //OfferId to OfferItemId
                                        item.OfferId = item.Id; //this offerId is now offer Item Id
                                        //Id to count Id in pos
                                        item.Id = orderProducts.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId).Id;
                                        existingItem.Add(item);
                                    }
                                }
                                if(existingItem.Any())
                                {
                                    return Json(new { existingItem, offer.OfferName,  Percentage = offer.DiscPercentage, Type = 2, subOfferId = 0, OfferId = offer.Id }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json("notValid", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (offer.Type == 3) // delivery charge coupon
                    {
                        decimal discountDeliveryCharge = 0;
                        if (deliveryCharge == 0)
                        {
                            return Json("notValid", JsonRequestBehavior.AllowGet);
                        }
                        var deliveryChargeCoupon = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.OfferId == offer.Id);
                        if (deliveryChargeCoupon.IsPriceRange)
                        {
                            if (deliveryChargeCoupon.FromPrice <= invoiceAmount && invoiceAmount <= deliveryChargeCoupon.ToPrice)
                            {
                                discountDeliveryCharge = deliveryCharge * ((decimal)deliveryChargeCoupon.Percentage / 100);
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            discountDeliveryCharge = deliveryCharge * ((decimal)deliveryChargeCoupon.Percentage / 100);
                        }
                        return Json(new { offer.OfferName, discountDeliveryCharge, deliveryChargeCoupon.Percentage , Type = 3, OfferId = offer.Id, delDiscId = deliveryChargeCoupon.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else if (offer.Type == 4) // amount coupon
                    {
                        var amountCoupon = db.ViewAmountCoupons.FirstOrDefault(a => a.OfferId == offer.Id);
                        decimal invoiceDiscount = 0;
                        if(amountCoupon.IsInifinte)
                        {
                            if(amountCoupon.FromPrice <= invoiceAmount)
                            {
                                if(amountCoupon.IsPercentile == true)
                                {
                                    invoiceDiscount = invoiceAmount * ((decimal)amountCoupon.Amount / 100);
                                }
                                else
                                {
                                    invoiceDiscount = amountCoupon.Amount;
                                }
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (amountCoupon.FromPrice <= invoiceAmount && invoiceAmount <= amountCoupon.ToPrice)
                            {
                                if (amountCoupon.IsPercentile == true)
                                {
                                    invoiceDiscount = invoiceAmount * ((decimal)amountCoupon.Amount / 100);
                                }
                                else
                                {
                                    invoiceDiscount = amountCoupon.Amount;
                                }
                            }
                            else
                            {
                                return Json("notValid", JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { offer.OfferName, invoiceDiscount, amountCoupon.Amount, amountCoupon.IsPercentile, amountCoupon.Id, Type = 4, OfferId = offer.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if(offer.EndDate < DateTime.Now)
                    {
                        return Json("expired", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("notyet", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(offer, JsonRequestBehavior.AllowGet);
        }

        //order save from pos main page
        public JsonResult OrderSave(OrderModel model)
        {
            PosOrderTransaction aTransaction;
            Stock aStock;
            PaymentBody account;
            bool IsDuePay = false;
            bool IsDebitPay = false;
            decimal debitPay = 0;
            decimal duePay = 0;
            decimal amountPaid = 0;
            try
            {
                //order save
                PosOrder aOrder = new PosOrder();
                aOrder.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmssf");
                aOrder.OrderType = 1; // sale order
                if (model.PurchaseOrderId > 0)
                {
                    aOrder.OrderType = 2; //purchase order
                    aOrder.PurchaseOrderId = model.PurchaseOrderId;
                }
                aOrder.OrderDate = DateTime.Now;
                aOrder.CreatedBy = model.CreatedBy;
                aOrder.OrderAmount = model.OrderAmount;
                if(model.InvoiceDiscount > 0)
                {
                    aOrder.OrderAmountDiscount = model.InvoiceDiscount;
                    aOrder.OrderAmountAfterDiscount = model.OrderAmount - model.InvoiceDiscount;
                    //Invoice total discount
                    aOrder.InvoiceTotalDiscount = model.InvoiceDiscount;
                    if(model.DelChargeBeforeDisc > 0)
                    {
                        aOrder.InvoiceTotalDiscount = model.InvoiceDiscount + (model.DelChargeBeforeDisc - model.DeliveryCharge);
                    }
                }
                if(model.DiscountItem > 0)
                {
                    aOrder.ItemDiscount = model.DiscountItem;
                }
                aOrder.SubTotalPrice = model.SubTotalPrice;
                if(model.SubTotalBeforeDiscount > 0)
                {
                    aOrder.SubTotalBeforeDiscount = model.SubTotalBeforeDiscount;
                }
                if (model.Discount > 0)
                {
                    aOrder.Discount = model.Discount;
                    aOrder.DiscountType = false;
                    if (model.DiscType == 1)
                    {
                        aOrder.DiscountType = true;
                    }
                    aOrder.DiscValue = model.DiscountAmount;
                    aOrder.DiscWorkValue = model.DiscountWorkAmount;
                    aOrder.DiscountId = model.DiscountId;

                    aOrder.OfferType = 1; // Manual Discount
                    if (aOrder.DiscountId > 0)
                    {
                        aOrder.OfferType = 2; // Auto Discount
                    }
                }
                if(model.CouponDiscount > 0)
                {

                    if(model.CouponDiscountAmount > model.DiscountAmount)
                    {
                        aOrder.OfferType = 3; // Coupon
                    }
                    //Coupon discount  
                    aOrder.CouponDiscount = model.CouponDiscount;
                    aOrder.CouponDiscType = false;
                    if (model.CouponDiscType == 1)
                    {
                        aOrder.CouponDiscType = true;
                    }
                    aOrder.CouponDiscValue = model.CouponDiscountAmount;
                    aOrder.CoupDiscWorkValue = model.CouponDiscWorkAmount;
                    aOrder.CouponDiscountId = model.CouponDiscId;
                }
                //Tax
                aOrder.Tax = model.Tax;
                aOrder.TaxPercent = model.TaxPercent;
                if (model.TaxFunc == 1)
                {
                    aOrder.TaxFunc = true; // true for tax on actual amount
                }
                else
                {
                    aOrder.TaxFunc = false; // false for tax on discounted amount
                }
                aOrder.TotalAmount = model.TotalPrice;
                //Delivery Charge
                aOrder.DelChargeAmount = model.DeliveryChargeAmount;
                aOrder.DelChargeType = false; // percentile
                if (model.DelChargeType == 1)
                {
                    aOrder.DelChargeType = true; // static
                }
                aOrder.DeliveryCharge = model.DeliveryCharge;
                aOrder.DelChargeBeforeDisc = model.DelChargeBeforeDisc;
                aOrder.DelChargeId = model.DeliveryChargeId;

                //Delivery charge discount
                if(model.DelDiscId > 0)
                {
                    aOrder.DelDiscId = model.DelDiscId;
                    aOrder.DelDiscPercent = model.DelChargeDiscPercent;
                    aOrder.DelDiscValue = model.DeliveryChargeDiscount;
                    aOrder.DelOfferType = 2; // auto
                }
                //Delivery coupon
                if(model.CouponDelivId > 0)
                {
                    if(model.CouponDelChargeDiscount > model.DeliveryChargeDiscount)
                    {
                        aOrder.DelOfferType = 3; // coupon
                    }
                    aOrder.CouponDelId = model.CouponDelivId;
                    aOrder.CouponDelPercent = model.CouponDelChargeDiscPercent;
                    aOrder.CouponDelValue = model.CouponDelChargeDiscount;
                }
                aOrder.InvoiceAmount = model.InvoiceAmount;
                aOrder.Status = true;
                aOrder.CustomerId = model.CustomerId;
                aOrder.IsServiceItem = model.IsServiceItems;
                aOrder.IsPointBasedItem = model.IsPointItems;
                aOrder.TotalPoints = model.TotalPoints;
                if(model.OfferId > 0)
                {
                    aOrder.CouponId = model.OfferId;
                    aOrder.CouponType = model.CouponType;
                    aOrder.InsertType = 1;
                }
                db.PosOrders.Add(aOrder);

                db.SaveChanges();

                //order transaction save
                foreach (var aProduct in model.OrderProducts)
                {
                    // all transaction of order save
                    //Transaction Type 1 for product
                    //Transaction Type 2 for credit payment
                    //Transaction Type 3 for  Debit account refill
                    aTransaction = new PosOrderTransaction();
                    aTransaction.OrderId = aOrder.OrderId;
                    aTransaction.ProductId = aProduct.ProductId;
                    aTransaction.DistributeId = aProduct.DistributeId;
                    aTransaction.ProductName = aProduct.ProductName;
                    aTransaction.TransactionType = aProduct.TransactionType;
                    aTransaction.Price = aProduct.Price;
                    aTransaction.IsRefundAllow = aProduct.IsRefundAllow;
                    if (aProduct.TransactionType == 1)
                    {

                        aTransaction.PurchaseOrderId = model.PurchaseOrderId;
                        aTransaction.POItemId = aProduct.POItemId;
                        aTransaction.Quantity = aProduct.Quantity;
                        aTransaction.OrderedQuantity = aProduct.Quantity;
                        aTransaction.PerItemPrice = aProduct.PerItemPrice;
                        aTransaction.PerItemTotal = aProduct.PerItemTotalPrice;
                        if (aProduct.PerItemTotalPrice == 0)
                        {
                            if(aTransaction.Quantity > 1)
                            {
                                aTransaction.PerItemTotal = aProduct.Price;
                            }
                            else
                            {
                                aTransaction.PerItemTotal = aProduct.PerItemPrice;
                            }
                        }
                        //Discount item
                        if (aProduct.Discount > 0)
                        {
                            aTransaction.Discount = aProduct.Discount;
                            if (aProduct.DiscType == 1)
                            {
                                aTransaction.DiscountType = true;
                            }
                            else
                            {
                                aTransaction.DiscountType = false;
                            }
                            aTransaction.DiscValue = aProduct.DiscountAmount;
                            aTransaction.DiscId = aProduct.DiscountId;

                            aTransaction.OfferType = 1; // Manuanl Discount
                            if (aTransaction.DiscId > 0)
                            {
                                aTransaction.OfferType = 2; // Auto Discount
                            }
                        }
                        if(aProduct.CouponDiscId > 0)
                        {
                            if(aProduct.CouponDiscAmount > aProduct.DiscountAmount)
                            {
                                aTransaction.OfferType = 3;
                            }
                            aTransaction.CouponDiscount = aProduct.CouponDiscount;
                            if (aProduct.CouponDiscType == 1)
                            {
                                aTransaction.CouponDiscType = true;
                            }
                            else
                            {
                                aTransaction.CouponDiscType = false;
                            }
                            aTransaction.CouponDiscValue = aProduct.CouponDiscAmount;
                            aTransaction.CouponDiscId = aProduct.CouponDiscId;
                            aTransaction.SubOfferId = aProduct.SubOfferId;
                        }
                        aTransaction.IsBorrow = aProduct.IsBorrow;
                        aTransaction.AssociateId = aProduct.AssociateId;
                        aTransaction.IsAfterSaleService = aProduct.IsAfterSaleService;

                        aTransaction.ServiceDays = aProduct.ServiceDays;
                        aTransaction.ServiceName = aProduct.ServiceName;
                        aTransaction.ServiceTypeId = aProduct.ServiceTypeId;
                        aTransaction.IsPointBased = aProduct.IsPointBased;
                        aTransaction.Points = aProduct.Points;
                        aTransaction.CustomerId = model.CustomerId;
                        aTransaction.IsUniqueItem = aProduct.IsUniqueItem;

                        if (aTransaction.IsUniqueItem == true)
                        {
                            aTransaction.Quantity = 1;
                            aTransaction.OrderedQuantity = 1;
                        }
                        aTransaction.SerialNumber = aProduct.SerialNumber;
                        aTransaction.Status = true;
                        db.PosOrderTransactions.Add(aTransaction);

                        // no stock count change for borrowed item
                        if(aTransaction.AssociateId == 0 || aTransaction.AssociateId == null)
                        {
                            //reduce product from stock
                            if (aTransaction.DistributeId > 0)
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aProduct.ProductId && a.DistributeId == aProduct.DistributeId);
                            }
                            else
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aProduct.ProductId);
                            }
                            //if this order product available
                            if (aStock != null)
                            {
                                var newQuantity = aStock.Quantity - aProduct.Quantity;
                                aStock.Quantity = newQuantity;
                                db.Entry(aStock).State = EntityState.Modified;
                            }
                            //if this order product not available
                            else
                            {
                                aStock = new Stock();
                                aStock.ProductId = aProduct.ProductId;
                                aStock.Quantity = 0 - aProduct.Quantity;
                                aStock.DistributeId = aProduct.DistributeId;
                                db.Stocks.Add(aStock);
                            }
                        }
                        //if purchase order transaction
                        if (aProduct.POItemId > 0)
                        {
                            var purchaseItem = db.PurchaseTransactions.Find(aProduct.POItemId);
                            if(purchaseItem != null)
                            {
                                purchaseItem.ReceiveQty = (int)(purchaseItem.ReceiveQty + aProduct.Quantity);
                                purchaseItem.RemainingQty = (int)(purchaseItem.RemainingQty - aProduct.Quantity);
                                db.Entry(purchaseItem).State = EntityState.Modified;
                            }
                        }
                    }
                    if (aProduct.TransactionType == 2) // due payment
                    {
                        IsDuePay = true;
                        duePay = aProduct.Price;
                    }

                    if (aProduct.TransactionType == 3) // debit refill
                    {
                        IsDebitPay = true;
                        debitPay = aProduct.Price;
                    }

                    db.SaveChanges();
                }

                //purchase order transaction completed if every item remaining 0
                var purchaseOrder = new Purchase();

                if(aOrder.PurchaseOrderId > 0)
                {
                    purchaseOrder = db.Purchases.Find(aOrder.PurchaseOrderId);
                    if(purchaseOrder != null)
                    {
                        if(purchaseOrder.Status == 3)
                        {
                            long poId = purchaseOrder.Id;
                            if(db.PurchaseTransactions.Any(a => a.PurchaseId == purchaseOrder.Id && a.RemainingQty > 0) == false)
                            {
                                purchaseOrder.Status = 4;
                                db.Entry(purchaseOrder).State = EntityState.Modified;
                            }
                        }
                    }
                }

                if (model.Payments != null)
                {
                    amountPaid = model.Payments.Sum(s => s.AmountPaid);
                }
                else
                {
                    amountPaid = 0;
                }
                //order payment save
                OrderPayment payment = new OrderPayment();
                payment.Amount = model.TotalPrice;
                payment.OrderId = aOrder.OrderId;
                payment.AmountPaid = amountPaid;
                payment.Status = true; //debit payment;
                if (model.CreditAmount > 0)
                {
                    payment.AmountPaid = amountPaid - (decimal)model.CreditAmount;
                    payment.DueAmount = model.CreditAmount;
                    payment.IsDuePayment = true;

                    //increase purchase order dispact due amount 
                    if(aOrder.PurchaseOrderId > 0)
                    {
                        if(purchaseOrder.DueAmount > 0)
                        {
                            purchaseOrder.DueAmount = purchaseOrder.DueAmount + payment.DueAmount;
                        }
                        else
                        {
                            purchaseOrder.DueAmount =  payment.DueAmount;
                        }
                        db.Entry(purchaseOrder).State = EntityState.Modified;
                    }
                }
                else
                {
                    payment.DueAmount = 0;
                    payment.IsDuePayment = false;
                }
                //increase purchase order dispacth total amount 
                if (aOrder.PurchaseOrderId > 0)
                {
                    amountPaid = payment.AmountPaid;
                    if (model.ReturnAmount > 0)
                    {
                        amountPaid = payment.AmountPaid - (decimal)model.ReturnAmount;
                    }
                    if(amountPaid > 0)
                    {
                        if (purchaseOrder.DispatchAmount > 0)
                        {
                            purchaseOrder.DispatchAmount = purchaseOrder.DispatchAmount + amountPaid;
                        }
                        else
                        {
                            purchaseOrder.DispatchAmount = amountPaid;
                        }
                        db.Entry(purchaseOrder).State = EntityState.Modified;
                    }
                }
                payment.ReturnAmount = model.ReturnAmount;
                payment.Date = DateTime.Now;
                payment.CreatedBy = model.CreatedBy;
                db.OrderPayments.Add(payment);

                db.SaveChanges();

                //order paymentid save

                aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == aOrder.OrderId);
                aOrder.PaymentId = payment.OrderPaymentId;
                db.Entry(aOrder).State = EntityState.Modified;

                db.SaveChanges();

                //save payment transaction
                if (model.Payments != null)
                {
                    decimal duePaid = 0;
                    decimal amount = 0;
                    decimal amountPay = 0;

                    PaymentTransaction aPaymentTransaction;
                    //Payment transaction save
                    foreach (var aPayment in model.Payments)
                    {
                        aPaymentTransaction = new PaymentTransaction();
                        amount = aPayment.AmountPaid;
                        if (IsDuePay == true && aPayment.PaymentTypeId != 7) // due pay    // PaymentTypeId 7 for Credit payment transaction
                        {
                            amountPaid = 0;
                            if(duePaid < duePay)
                            {
                                if(amount > (duePay - duePaid))
                                {
                                    amountPay = (duePay - duePaid);
                                    amount = amount - amountPay;
                                }
                                else
                                {
                                    amountPay = amount;
                                    amount = 0;
                                }
                                var creditList = db.ViewCreditCustomers.Where(a => a.CustomerId == model.CustomerId && a.DueAmount > 0).OrderBy(a => a.OrderDate).ToList();
                                if(creditList != null)
                                {
                                    foreach (var list in creditList)
                                    {
                                        if(amountPay > 0)
                                        {
                                            payment = db.OrderPayments.FirstOrDefault(a => a.OrderId == list.OrderId);
                                            if (amountPay >= payment.DueAmount)
                                            {
                                                amountPaid = (decimal)payment.DueAmount;
                                                amountPay = amountPay - (decimal)payment.DueAmount;
                                                payment.DueAmount = 0;
                                            }
                                            else
                                            {
                                                amountPaid = amountPay;
                                                payment.DueAmount = payment.DueAmount - amountPay;
                                                amountPay = 0;
                                            }
                                            payment.AmountPaid = payment.AmountPaid + amountPaid;
                                            db.Entry(payment).State = EntityState.Modified;

                                            //payment transaction save
                                            aPaymentTransaction.PaymentId = aOrder.OrderId;
                                            aPaymentTransaction.CustomerId = aOrder.CustomerId;
                                            aPaymentTransaction.CreditPaymentId = payment.OrderId;
                                            aPaymentTransaction.Type = 1; // Type 1 for order payment 
                                            aPaymentTransaction.InOut = true; // InOut true for receive payment
                                            aPaymentTransaction.MethodId = (int)model.MethodId;
                                            aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                                            aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                                            aPaymentTransaction.Amount = amountPaid;
                                            aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                                            aPaymentTransaction.Date = DateTime.Now;
                                            aPaymentTransaction.CreatedBy = model.CreatedBy;
                                            aPaymentTransaction.IsCreditPayment = true;
                                            db.PaymentTransactions.Add(aPaymentTransaction);
                                            if (aPayment.PaymentTypeId == 8) //Debit payment transaction
                                            {
                                                //decrease customer debit amount 
                                                DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                                                customerDebit.Amount = customerDebit.Amount - amountPaid;
                                                db.Entry(customerDebit).State = EntityState.Modified;
                                            }
                                            //add amount in account balance
                                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                                            account.Balance = account.Balance + amountPaid;
                                            db.Entry(account).State = EntityState.Modified;
                                            
                                            duePaid = duePaid + amountPaid;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        //if (IsDebitPay == true && amount > 0 && (aPayment.PaymentTypeId != 8 || aPayment.PaymentTypeId != 7)) // debit refill
                        //{
                        //    if(debitPaid < debitPay )
                        //    {
                        //        if (amount > (debitPay - debitPaid))
                        //        {
                        //            amountPay = (debitPay - debitPaid);
                        //            amount = amount - amountPay;
                        //        }
                        //        else
                        //        {
                        //            amountPay = amount;
                        //            amount = 0;
                        //        }
                        //        //debit account refill
                        //        DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                        //        customerDebit.Amount = customerDebit.Amount + amountPay;
                        //        db.Entry(customerDebit).State = EntityState.Modified;

                        //        debitPaid = debitPaid + amountPay;

                        //        aPaymentTransaction.PaymentId = aOrder.OrderId;
                        //        aPaymentTransaction.Type = 10; // Type 10 for customer debit refill payment
                        //        aPaymentTransaction.InOut = true; // InOut true for receive payment
                        //        aPaymentTransaction.MethodId = (int)model.MethodId;
                        //        aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                        //        aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                        //        aPaymentTransaction.Amount = amountPay;
                        //        aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                        //        aPaymentTransaction.Date = DateTime.Now;
                        //        aPaymentTransaction.IsCreditPayment = false;
                        //        aPaymentTransaction.CreatedBy = (int)model.SalesmanId;

                        //        db.PaymentTransactions.Add(aPaymentTransaction);

                        //        //add amount in account balance
                        //        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                        //        account.Balance = account.Balance + amountPay;

                        //        db.Entry(account).State = EntityState.Modified;
                        //    }
                        //    //db.SaveChanges();
                        //}
                        if (amount > 0) //order payment
                        {
                            aPaymentTransaction = new PaymentTransaction();
                            aPaymentTransaction.PaymentId = aOrder.OrderId;
                            aPaymentTransaction.CustomerId = aOrder.CustomerId;
                            aPaymentTransaction.Type = 1; // Type 1 for order payment
                            if(aPayment.PaymentTypeId == 7)
                            {
                                aPaymentTransaction.InOut = false; // InOut false for release payment
                            }
                            else
                            {
                                aPaymentTransaction.InOut = true; // InOut true for receive payment
                            }
                            aPaymentTransaction.MethodId = (int)model.MethodId;
                            aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                            aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                            aPaymentTransaction.Amount = amount;
                            aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                            aPaymentTransaction.Date = DateTime.Now;
                            aPaymentTransaction.IsCreditPayment = false;
                            aPaymentTransaction.CreatedBy = model.CreatedBy;

                            db.PaymentTransactions.Add(aPaymentTransaction);

                            if (aPayment.PaymentTypeId == 8) //Debit payment transaction
                            {
                                //decrease customer debit amount 
                                DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                                customerDebit.Amount = customerDebit.Amount - amount;
                                db.Entry(customerDebit).State = EntityState.Modified;
                            }
                            else if(aPayment.PaymentTypeId == 7) // credit payment transaction
                            {
                            
                            }
                            //add amount in account balance
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance + amount;
                            db.Entry(account).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                //customer debit account refill 
                if(IsDebitPay)
                {
                    //debit account refill
                    DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                    customerDebit.Amount = customerDebit.Amount + debitPay;
                    db.Entry(customerDebit).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { aOrder.OrderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        //get productList for barcode searchbox 
        public JsonResult GetProductList()
        {
            var alist = new SelectList(db.ViewProducts.Where(a=>a.Status == true).Select(a => new { a.RowID, a.Barcode }), "RowID", "Barcode");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        // get productlist for product name searchbox
        //Using Import,WasteCreate,POS page
        public JsonResult GetProductListForText(string ids, string text)
        {
            List<ViewProduct> productList = new List<ViewProduct>();
            if (!string.IsNullOrEmpty(text))
            {
                productList = db.ViewProducts.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).ToList();
            }
            else
            {
                productList = db.ViewProducts.Where(a => a.Status == true).ToList();
            }
            if (!string.IsNullOrEmpty(ids))
            {
                long rowId = 0;
                foreach (var id in ids.Split(','))
                {
                    rowId = Convert.ToInt64(id);
                    var list = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    if (list != null)
                    {
                        productList.Remove(list);
                    }
                }
            }
            var alist = new SelectList(productList.Select(a => new { a.RowID, a.ProductName }), "RowID", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        // get product search by barcode
        public JsonResult GetProductByBarCode(string BarCode)
        {
            if(!string.IsNullOrEmpty(BarCode))
            {
                Product aProduct = db.Products.Where(a=>a.Status == true).FirstOrDefault(a => a.BarCode == BarCode);
                if(aProduct != null)
                {
                    return Json(aProduct.ProductId, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //*********************Order History************************************
        #region Order History
        //all order info page
        //public ActionResult Orders()
        //{
        //    return View();
        //}
        //all order info list
        //use multiple => Order, Customer Invoice Info
        public PartialViewResult OrderList(string selectedId, long? customerId, int? count, int? days, DateTime? from, DateTime? to, bool? isRefundList)
        {
            DateTime? start = from;
            DateTime? end = to;
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            if(customerId > 0)
            {
                if (count > 0)
                {
                    list = db.ViewPosOrders.Where(a => a.CustomerId == customerId && a.Status == true).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.CustomerId == customerId && m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.CustomerId == customerId && m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else if(isRefundList == true)
            {
                if (count > 0)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == false).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == true).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            if (!string.IsNullOrEmpty(selectedId))
            {
                list.Clear();
                foreach(var id in selectedId.Split(','))
                {
                    var orderId = Convert.ToInt64(id);
                    ViewPosOrder aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    if(aOrder != null)
                    {
                        list.Add(aOrder);
                    }
                }
            }
            list = list.OrderByDescending(a => a.OrderDate).ToList();
            return PartialView(list);
        }
        //get all order list for refund
        public JsonResult GetOrderList(long? orderId, int? days, DateTime? from, DateTime? to, bool? IsrefundList, bool? IsServiceList)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<PosOrder>();
            if(IsServiceList == true)
            {
                list = db.PosOrders.Where(a => a.IsServiceItem == true && a.Status == true).ToList();
            }
            else if(IsrefundList == true)
            {
                if (days == 0 || days == null && from == null)
                {
                    list = db.PosOrders.Where(a => a.IsItemRefund == true).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.PosOrders.Where(m => m.IsItemRefund == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.PosOrders.Where(m => m.IsItemRefund == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (days == 0 || days == null && from == null)
                {
                    list = db.PosOrders.Where(a => a.Status == true).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.PosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.PosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            var orderList = new SelectList(list.OrderByDescending(a => a.OrderDate).GroupBy(g => g.OrderId).Select(s => new { OrderId = s.FirstOrDefault().OrderId, OrderNumber = s.FirstOrDefault().OrderNumber }).ToList(), "OrderId", "OrderNumber");
            return Json(orderList,JsonRequestBehavior.AllowGet);
        }
        //Get order transaction by orderId
        public JsonResult GetOrderTransactionList(long? orderId, string serialNumber, long? serviceOrderId, long? orderTransId)
        {
            var list = new List<PosOrderTransaction>();

            if (orderId > 0)
            {
                list = db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == false && a.Quantity > 0).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else if(serviceOrderId > 0)
            {
                if(db.PosOrderTransactions.Where(a=> a.OrderId == serviceOrderId && a.IsAfterSaleService == true).Count() > 1)
                {
                    return Json(new { IsMultipleService = true }, JsonRequestBehavior.AllowGet);
                }
                else if(db.PosOrderTransactions.Where(a => a.OrderId == serviceOrderId && a.IsAfterSaleService == true).Count() == 1)
                {
                    var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderId == serviceOrderId && a.IsAfterSaleService == true);
                    //calculate service days
                    long customerId = 0;
                    int serviceDays = 0;
                    DateTime orderDate = new DateTime();
                    DateTime expireDate = new DateTime();
                    bool isExpire = false;
                    bool IsExist = false;
                    orderDate = (DateTime)orderTransaction.OrderDate;
                    if (orderTransaction.ServiceDays > 0)
                    {
                        serviceDays = (int)orderTransaction.ServiceDays;
                    }
                    expireDate = orderDate.AddDays(serviceDays);
                    if (expireDate < now)
                    {
                        isExpire = true;
                    }
                    if (orderTransaction.CustomerId > 0)
                    {
                        customerId = (long)orderTransaction.CustomerId;
                    }

                    //if already exist to after sale service
                    if (db.ViewAfterSaleServices.Any(a => a.OrderTransactionId == orderTransaction.OrderTransactionId))
                    {
                        var saleService = db.ViewAfterSaleServices.FirstOrDefault(a => a.OrderTransactionId == orderTransaction.OrderTransactionId);
                        IsExist = true;
                        return Json(new { IsMultipleService = false, saleService.TemporaryWorkOrderNo, saleService.WorkOrderNo, saleService.Status, ServiceOrderDate = saleService.CreatedDate, serviceCreatedBy = saleService.CreatedBy, saleService.DeliveryDate, IsExist = IsExist, orderTransaction.SerialNumber, orderTransaction.ProductName, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { IsExist = IsExist, CustomerId = customerId, IsMultipleService = false, orderTransaction.SerialNumber,  TransId = orderTransaction.OrderTransactionId, IsExpire = isExpire, orderTransaction.ProductName, orderTransaction.ProductId, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                }
            }
            else if(orderTransId > 0)
            {
                var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransId);
                //calculate service days
                long customerId = 0;
                int serviceDays = 0;
                DateTime orderDate = new DateTime();
                DateTime expireDate = new DateTime();
                bool isExpire = false;
                bool IsExist = false;
                orderDate = (DateTime)orderTransaction.OrderDate;
                if (orderTransaction.ServiceDays > 0)
                {
                    serviceDays = (int)orderTransaction.ServiceDays;
                }
                expireDate = orderDate.AddDays(serviceDays);
                if (expireDate < now)
                {
                    isExpire = true;
                }
                if (orderTransaction.CustomerId > 0)
                {
                    customerId = (long)orderTransaction.CustomerId;
                }
                //if already exist to after sale service
                if (db.ViewAfterSaleServices.Any(a => a.OrderTransactionId == orderTransId))
                {
                    var saleService = db.ViewAfterSaleServices.FirstOrDefault(a => a.OrderTransactionId == orderTransId);
                    IsExist = true;
                    return Json(new { saleService.TemporaryWorkOrderNo, saleService.WorkOrderNo, saleService.Status, ServiceOrderDate = saleService.CreatedDate, serviceCreatedBy = saleService.CreatedBy, saleService.DeliveryDate, IsExist = IsExist, orderTransaction.SerialNumber, orderTransaction.ProductName, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { IsExist = IsExist, CustomerId = customerId, orderTransaction.SerialNumber, TransId = orderTransaction.OrderTransactionId, IsExpire = isExpire, orderTransaction.ProductName, orderTransaction.ProductId, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
            }
            else if (!string.IsNullOrEmpty(serialNumber))
            {
                if(db.PosOrderTransactions.Any(a => a.SerialNumber == serialNumber && a.IsAfterSaleService == true))
                {
                    var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.SerialNumber == serialNumber);
                    //calculate service days
                    long customerId = 0;
                    int serviceDays = 0;
                    DateTime orderDate = new DateTime();
                    DateTime expireDate = new DateTime();
                    bool isExpire = false;
                    bool IsExist = false;
                    orderDate = (DateTime)orderTransaction.OrderDate;
                    if (orderTransaction.ServiceDays > 0)
                    {
                        serviceDays = (int)orderTransaction.ServiceDays;
                    }
                    expireDate = orderDate.AddDays(serviceDays);
                    if (expireDate < now)
                    {
                        isExpire = true;
                    }
                    if (orderTransaction.CustomerId > 0)
                    {
                        customerId = (long)orderTransaction.CustomerId;
                    }
                    //if already exist to after sale service
                    if (db.ViewAfterSaleServices.Any(a => a.SerialNumber == serialNumber))
                    {
                        var saleService = db.ViewAfterSaleServices.FirstOrDefault(a => a.SerialNumber == serialNumber);
                        IsExist = true;
                        return Json(new {saleService.TemporaryWorkOrderNo, saleService.WorkOrderNo ,saleService.Status, ServiceOrderDate = saleService.CreatedDate, serviceCreatedBy = saleService.CreatedBy, saleService.DeliveryDate, IsExist = IsExist, orderTransaction.SerialNumber, orderTransaction.ProductName, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { IsExist = IsExist, CustomerId = customerId, orderTransaction.SerialNumber, TransId = orderTransaction.OrderTransactionId, IsExpire = isExpire, orderTransaction.ProductName, orderTransaction.ProductId, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("notFound", JsonRequestBehavior.AllowGet);
        }
        //Order Print
        public ActionResult OrdersPrint(string q)
        {
            long customerId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                customerId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.CustomerId = customerId;
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View();
        }
        public PartialViewResult OrderListForPrint(long? customerId, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewPosOrder>();
            if(customerId > 0)
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.CustomerId == customerId && m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.CustomerId == customerId && m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a=>a.OrderDate).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult OrderTransactionPrint(long? orderId,bool? IsOriginal, DateTime? refundDate)
        {
            decimal subTotalPrice = 0;
            decimal totalPrice = 0;
            decimal discount = 0;
            decimal taxAmount = 0;
            decimal itemTotalPrice = 0;
            var aOrder = new ViewPosOrder();
            var orderTransactionList = new List<ViewOrderTransaction>();
            if (IsOriginal == true)
            {
                orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
                aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                foreach (var item in orderTransactionList)
                {
                    discount = 0;
                    itemTotalPrice = item.OrderedQuantity * item.PerItemPrice;
                    if (item.Discount > 0)
                    {
                        discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                        itemTotalPrice = itemTotalPrice - discount;
                        item.Discount = discount;
                    }
                    subTotalPrice = subTotalPrice + itemTotalPrice;
                    item.Price = itemTotalPrice;
                    item.Quantity = item.OrderedQuantity;
                    item.RefundStatus = 0;
                }
                totalPrice = subTotalPrice;
                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
                {
                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                    totalPrice = totalPrice + taxAmount;
                }
                if (aOrder.Discount > 0)
                {
                    discount = 0;
                    discount = totalPrice * ((decimal)aOrder.Discount / 100);
                    totalPrice = totalPrice - discount;
                    aOrder.Discount = discount;
                }
                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
                {
                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                    totalPrice = totalPrice + taxAmount;
                }
                aOrder.SubTotalPrice = subTotalPrice;
                aOrder.InvoiceAmount = totalPrice;
                aOrder.Tax = taxAmount;
                ViewBag.OrderTransList = orderTransactionList;
            }
            else if(refundDate != null)
            {
                var refundList = db.ViewPosRefunds.Where(a => a.OrderId == orderId && DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(refundDate))
                   .GroupBy(a => a.ProductId)
                   .Select(s => new { ProductId = s.FirstOrDefault().ProductId, Quantity = s.Sum(a => a.RefundQuantity) })
                   .ToList();

                orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
                aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                foreach (var item in orderTransactionList)
                {
                    discount = 0;
                    var aRefundList = refundList.FirstOrDefault(a => a.ProductId == item.ProductId);
                    if (aRefundList != null)
                    {
                        item.Quantity = item.OrderedQuantity - aRefundList.Quantity;
                    }
                    else
                    {
                        item.Quantity = item.OrderedQuantity;
                    }
                    itemTotalPrice = item.Quantity * item.PerItemPrice;
                    if (item.Discount > 0)
                    {
                        discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                        itemTotalPrice = itemTotalPrice - discount;
                        item.Discount = discount;
                    }
                    subTotalPrice = subTotalPrice + itemTotalPrice;
                    item.Price = itemTotalPrice;
                    item.RefundStatus = 0;
                }
                totalPrice = subTotalPrice;
                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
                {
                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                    totalPrice = totalPrice + taxAmount;
                }
                if (aOrder.Discount > 0)
                {
                    discount = 0;
                    discount = totalPrice * ((decimal)aOrder.Discount / 100);
                    totalPrice = totalPrice - discount;
                    aOrder.Discount = discount;
                }
                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
                {
                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                    totalPrice = totalPrice + taxAmount;
                }
                aOrder.SubTotalPrice = subTotalPrice;
                aOrder.InvoiceAmount = totalPrice;
                aOrder.Tax = taxAmount;
                ViewBag.OrderTransList = orderTransactionList.Where(a => a.Quantity > 0).ToList();
            }
            else
            {
                aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId && a.Quantity > 0).ToList();
                ViewBag.OrderTransList = orderTransactionList;
            }
            return View(aOrder);
        }
        //order original voucher 
        [EncryptedActionParameter]
        public ActionResult OriginalVoucher(long orderId)
        {
            decimal subTotalPrice = 0;
            decimal totalPrice = 0;
            decimal discount = 0;
            decimal taxAmount = 0;
            decimal itemTotalPrice = 0;
            var orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
            var aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            foreach(var item in orderTransactionList)
            {
                discount = 0;
                itemTotalPrice = item.OrderedQuantity * item.PerItemPrice;
                if(item.Discount > 0)
                {
                    discount =(decimal)(itemTotalPrice * (item.Discount / 100));
                    itemTotalPrice = itemTotalPrice - discount;
                    item.Discount = discount;
                }
                subTotalPrice = subTotalPrice + itemTotalPrice;
                item.Price = itemTotalPrice;
            }
            totalPrice = subTotalPrice;
            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
            {
                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                totalPrice = totalPrice + taxAmount;
            }
            if (aOrder.Discount > 0)
            {
                discount = 0;
                discount = totalPrice * ((decimal)aOrder.Discount / 100);
                totalPrice = totalPrice - discount;
                aOrder.Discount = discount;
            }
            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
            {
                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                totalPrice = totalPrice + taxAmount;
            }
            aOrder.SubTotalPrice = subTotalPrice;
            aOrder.InvoiceAmount = totalPrice;
            aOrder.Tax = taxAmount;
            ViewBag.OrderTransList = orderTransactionList;
            return View(aOrder);
        }
        #endregion
        //*********************Numpad ******************************************
        #region Numpad
        public PartialViewResult Numpad(int isDeliveryChargeAuto)
        {
            ViewBag.IsDeliveryChargeAuto = isDeliveryChargeAuto;
            return PartialView();
        }
        public PartialViewResult DiscounNumpad()
        {
            return PartialView();
        }
        public PartialViewResult RefundQuantityNumpad()
        {
            return PartialView();
        }
        #endregion
        //*********************Refund Function**********************************
        #region Refund
        // Refund page
        [EncryptedActionParameter]
        public ActionResult Refund(long OrderId)
        {
            return View(db.ViewPosOrders.FirstOrDefault(a => a.OrderId == OrderId));
        }
        //Refund alert
        public PartialViewResult RefundAlert(long orderId)
        {
            bool isRefundableItemExist = false;
            if(db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == true && a.Quantity > 0).Any())
            {
                isRefundableItemExist = true;
            }
            ViewBag.OrderId = orderId;
            ViewBag.IsRefundableItemExist = isRefundableItemExist;
            var list = db.ViewOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == false && a.Quantity > 0).ToList();
            return PartialView(list);
        }
        //all ordered product in refund page
        public PartialViewResult OrderedProducts(long? orderId)
        {
            return PartialView(db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList());
        }
        //******************** refund create from refund page ***************************
        //Refund single item 
        public PartialViewResult RefundCreate(long? orderTransactionId)
        {
            ViewBag.orderTransactionId = orderTransactionId;
            return PartialView();
        }
        public PartialViewResult RefundCreatePartial(long? orderTransactionId)
        {
            var aOrderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransactionId);
            return PartialView(aOrderTransaction);
        }
        // single or full refund
        public JsonResult RefundSave(long? orderTransactionId, decimal? quantity, int userId, long? orderId, int? methodId, IList<PaymentModel> Payments)
        {
            decimal totalPrice = 0;
            decimal? itemPrice = 0;
            decimal itemTotalPrice = 0;
            decimal newQuantity = 0;
            decimal? previousQuantity = 0;
            decimal subTotalPrice = 0;
            decimal taxAmount = 0;
            decimal discount = 0;
            decimal previousAmount = 0;
            PosOrder aOrder = new PosOrder();
            PosOrderTransaction aTransaction = new PosOrderTransaction();
            if (orderId > 0)
            {
                 aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == orderId);
            }
            if(orderTransactionId > 0)
            {
                aTransaction = db.PosOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransactionId);
                aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == aTransaction.OrderId);
            }
            RefundTransaction refundTrans;
            Refund aRefund;
            PaymentTransaction refundPayment;
            PaymentBody account;
            try
            {
                //refund save
                aRefund = new Refund();
                aRefund.OrderId = aOrder.OrderId;
                aRefund.RefundAmount = Payments.Sum(a => a.AmountPaid);
                aRefund.RefundBy = userId;
                aRefund.Date = DateTime.Now;
                db.Refunds.Add(aRefund);
                db.SaveChanges();
                if(orderId > 0)
                {
                    previousAmount = aOrder.InvoiceAmount;
                    var orderTransaction = db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.Quantity > 0).ToList();
                    //refund all ordered transaction for an order
                    foreach (var aOrdertTransaction in orderTransaction)
                    {
                        if (aOrdertTransaction.IsRefundAllow == true)
                        {
                            subTotalPrice = 0;
                            previousQuantity = aOrdertTransaction.Quantity;
                            aOrdertTransaction.Quantity = 0;
                            aOrdertTransaction.Price = 0;
                            aOrdertTransaction.RefundStatus = 1;
                            aOrdertTransaction.RefundQuantity = previousQuantity;
                            aOrdertTransaction.RefundBy = userId;
                            aOrdertTransaction.RefundDateTime = DateTime.Now;

                            db.Entry(aOrdertTransaction).State = EntityState.Modified;
                            db.SaveChanges();

                            subTotalPrice = db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.Quantity > 0).Sum(s => s.Price);
                            totalPrice = subTotalPrice;

                            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
                            {
                                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                                totalPrice = totalPrice + taxAmount;
                            }
                            if (aOrder.Discount > 0)
                            {
                                discount = totalPrice * ((decimal)aOrder.Discount / 100);
                                totalPrice = totalPrice - discount;
                            }
                            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
                            {
                                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                                totalPrice = totalPrice + taxAmount;
                            }
                            //refund product save
                            refundTrans = new RefundTransaction();
                            refundTrans.RefundId = aRefund.RefundId;
                            refundTrans.PreviousAmount = previousAmount;
                            refundTrans.RefundAmount = previousAmount - totalPrice;
                            refundTrans.CurrentAmount = totalPrice;
                            refundTrans.PreviousQuantity = (decimal)previousQuantity;
                            refundTrans.RefundQuantity = (decimal)previousQuantity;
                            refundTrans.ProductId = aOrdertTransaction.ProductId;
                            db.RefundTransactions.Add(refundTrans);
                            db.SaveChanges();
                            previousAmount = totalPrice;
                        }
                    }
                }
                else
                {
                    newQuantity = (decimal)(aTransaction.Quantity - quantity); //quantity after refund
                    itemPrice = aTransaction.Price / aTransaction.Quantity;
                    itemTotalPrice = (decimal)(newQuantity * itemPrice);
                    previousQuantity = aTransaction.Quantity;

                    totalPrice = (aTransaction.PerItemPrice * newQuantity);
                    discount = totalPrice * ((decimal)aTransaction.Discount / 100);

                    //ordered product save after refund
                    aTransaction.Quantity = newQuantity;
                    aTransaction.Price = itemTotalPrice;
                    aTransaction.Discount = discount;
                    aTransaction.RefundStatus = 1;
                    aTransaction.RefundQuantity = quantity;
                    aTransaction.RefundBy = userId;
                    aTransaction.RefundDateTime = DateTime.Now;

                    db.Entry(aTransaction).State = EntityState.Modified;
                    db.SaveChanges();

                    totalPrice = 0;
                    discount = 0;

                    subTotalPrice = (decimal)db.PosOrderTransactions.Where(a => a.OrderId == aTransaction.OrderId && a.Quantity > 0).Sum(s => s.Price);
                    totalPrice = subTotalPrice;
                    if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
                    {
                        taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                        totalPrice = totalPrice + taxAmount;
                    }
                    if (aOrder.Discount > 0)
                    {
                        discount = totalPrice * ((decimal)aOrder.Discount / 100);
                        totalPrice = totalPrice - discount;
                    }
                    if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
                    {
                        taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                        totalPrice = totalPrice + taxAmount;
                    }
                    //refund product save
                    refundTrans = new RefundTransaction();
                    refundTrans.RefundId = aRefund.RefundId;
                    refundTrans.PreviousAmount = aOrder.InvoiceAmount;
                    refundTrans.RefundAmount = aOrder.InvoiceAmount - totalPrice;
                    refundTrans.CurrentAmount = totalPrice;
                    refundTrans.PreviousQuantity = (decimal)previousQuantity;
                    refundTrans.RefundQuantity = (decimal)quantity;
                    refundTrans.ProductId = (int)aTransaction.ProductId;
                    db.RefundTransactions.Add(refundTrans);
                    db.SaveChanges();
                }
                if (totalPrice > 0)
                {
                    aOrder.InvoiceAmount = totalPrice;
                    aOrder.SubTotalPrice = subTotalPrice;
                    if(aOrder.Discount > 0)
                    {
                        aOrder.Discount = discount;
                    }
                    aOrder.IsItemRefund = true;
                    if (aOrder.TaxPercent > 0)
                    {
                        aOrder.Tax = taxAmount;
                    }
                }
                else
                {
                    aOrder.Status = false;
                    aOrder.RefundStatus = true;
                    aOrder.RefundBy = userId;
                    aOrder.IsItemRefund = true;
                    aOrder.RefundDateTime = DateTime.Now;
                }
                db.Entry(aOrder).State = EntityState.Modified;
                db.SaveChanges();

                //refund payment save
                if (Payments != null)
                {
                    foreach (var payment in Payments)
                    {
                        refundPayment = new PaymentTransaction();
                        refundPayment.PaymentId = aRefund.RefundId;
                        refundPayment.Type = 2; // Type 2 for refund payment
                        refundPayment.InOut = false;// InOut false for release payment
                        refundPayment.MethodId = (int)methodId;
                        refundPayment.PaymentTypeId = payment.PaymentTypeId;
                        refundPayment.PaymentBodyId = payment.PaymentBodyId;
                        refundPayment.Amount = payment.AmountPaid;
                        refundPayment.TransactionNo = payment.TransactionNo;
                        refundPayment.CreatedBy = userId;
                        refundPayment.Date = DateTime.Now;
                        refundPayment.IsCreditPayment = false;
                        db.PaymentTransactions.Add(refundPayment);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == payment.PaymentBodyId);
                        account.Balance = account.Balance - payment.AmountPaid;

                        db.Entry(account).State = EntityState.Modified;


                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //*********************Full refund***************************
        //public JsonResult RefundOrder(long? orderId, int? userId)
        //{
        //    decimal discount = 0;
        //    decimal totalPrice = 0;
        //    decimal? quantity = 0;
        //    decimal subTotalPrice = 0;
        //    decimal taxAmount = 0;
        //    decimal previousAmount = 0;
        //    PosOrder aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == orderId);
        //    List<PosOrderTransaction> orderTransaction  = db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.Quantity > 0).ToList();
        //    PosRefund orderRefund;
        //    previousAmount = aOrder.TotalPrice;
        //    try
        //    {
        //        //refund all ordered transaction for an order
        //        foreach (var aOrdertTransaction in orderTransaction)
        //        {
        //            if(aOrdertTransaction.IsRefundAllow == true)
        //            {
        //                subTotalPrice = 0;
        //                quantity = aOrdertTransaction.Quantity;
        //                aOrdertTransaction.Quantity = 0;
        //                aOrdertTransaction.Price = 0;
        //                aOrdertTransaction.RefundStatus = 1;
        //                aOrdertTransaction.RefundQuantity = quantity;
        //                aOrdertTransaction.RefundBy = userId;
        //                aOrdertTransaction.RefundDateTime = DateTime.Now;

        //                db.Entry(aOrdertTransaction).State = EntityState.Modified;
        //                db.SaveChanges();

        //                var aTransactionList = db.PosOrderTransactions.Where(a => a.OrderId == orderId).ToList();
        //                foreach (var list in aTransactionList)
        //                {
        //                    subTotalPrice = subTotalPrice + (decimal)list.Price;
        //                }
        //                totalPrice = subTotalPrice;
        //                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
        //                {
        //                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
        //                    totalPrice = totalPrice + taxAmount;
        //                }
        //                if (aOrder.DiscPercent > 0)
        //                {
        //                    discount = totalPrice * ((decimal)aOrder.DiscPercent / 100);
        //                    totalPrice = totalPrice - discount;
        //                }
        //                if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
        //                {
        //                    taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
        //                    totalPrice = totalPrice + taxAmount;
        //                }
        //                //refund product save
        //                orderRefund = new PosRefund();
        //                orderRefund.OrderId = orderId;
        //                orderRefund.PreviousAmount = previousAmount;
        //                orderRefund.RefundAmount = previousAmount - totalPrice;
        //                orderRefund.CurrentAmount = totalPrice;
        //                orderRefund.PreviousQuantity = quantity;
        //                orderRefund.RefundQuantity = quantity;
        //                orderRefund.ProductId = aOrdertTransaction.ProductId;
        //                orderRefund.RefundBy = userId;
        //                orderRefund.RefundDateTime = DateTime.Now;

        //                db.PosRefunds.Add(orderRefund);

        //                if (totalPrice > 0)
        //                {
        //                    aOrder.TotalPrice = totalPrice;
        //                    aOrder.SubTotalPrice = subTotalPrice;
        //                    aOrder.Discount = discount;
        //                    aOrder.IsItemRefund = true;
        //                    if (aOrder.TaxPercent > 0)
        //                    {
        //                        aOrder.Tax = taxAmount;
        //                    }
        //                }
        //                else
        //                {
        //                    aOrder.Status = false;
        //                    aOrder.RefundStatus = true;
        //                    aOrder.RefundBy = userId;
        //                    aOrder.IsItemRefund = true;
        //                    aOrder.RefundDateTime = DateTime.Now;
        //                }
        //                db.Entry(aOrder).State = EntityState.Modified;
        //                db.SaveChanges();
        //                previousAmount = totalPrice;
        //            }
        //        }
        //    }
        //    catch(Exception)
        //    {
        //        return Json("error", JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("success", JsonRequestBehavior.AllowGet);
        //}
        //order info for refund in pos main page
        public PartialViewResult OrderedInfo(long orderId)
        {
            return PartialView(db.PosOrders.Find(orderId));
        }
        //order refund from pos main page
        public JsonResult OrderRefund(OrderModel model)
        {
            decimal totalPrice = 0;
            decimal? refundQuantity = 0;
            decimal? previousQuantity = 0;
            decimal subTotalPrice = 0;
            decimal taxAmount = 0;
            decimal previousAmount = 0;
            decimal disCount = 0;
            bool refund = false;
            PosOrder aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == model.OrderId);
            List<PosOrderTransaction> orderTransaction = db.PosOrderTransactions.Where(a => a.OrderId == model.OrderId && a.Quantity > 0).ToList();
            RefundTransaction refundTrans;
            Refund aRefund;
            PaymentTransaction refundPayment;
            PaymentBody account;
            previousAmount = aOrder.InvoiceAmount;
            try
            {
                //refund save
                aRefund = new Refund();
                aRefund.OrderId = (long)model.OrderId;
                aRefund.RefundAmount = model.Payments.Sum(a => a.AmountPaid);
                aRefund.RefundBy = (int)model.SalesmanId;
                aRefund.Date = DateTime.Now;
                db.Refunds.Add(aRefund);
                db.SaveChanges();
                //refund transaction save
                foreach (var aTransaction in orderTransaction)
                {
                    OrderItemModel aProduct = null;
                    if (model.OrderProducts != null)
                    {
                        aProduct = model.OrderProducts.FirstOrDefault(a => a.ProductId == aTransaction.ProductId);
                        if (aProduct != null)
                        {
                            if (aProduct.Quantity < aTransaction.Quantity) //refund product if quantity changed 
                            {
                                previousQuantity = aTransaction.Quantity;
                                refundQuantity = aTransaction.Quantity - aProduct.Quantity;
                                aTransaction.RefundQuantity = aTransaction.Quantity - aProduct.Quantity;
                                aTransaction.Quantity = aProduct.Quantity;
                                aTransaction.Price = aProduct.Price;
                                aTransaction.Discount = aProduct.Discount;
                                aTransaction.RefundStatus = 1;
                                aTransaction.RefundBy = model.SalesmanId;
                                aTransaction.RefundDateTime = DateTime.Now;
                                db.Entry(aTransaction).State = EntityState.Modified;
                                refund = true;
                            }
                        }
                        //refund all quantity of product 
                        else
                        {
                            previousQuantity = aTransaction.Quantity;
                            refundQuantity = aTransaction.Quantity;
                            aTransaction.RefundStatus = 1;
                            aTransaction.RefundBy = model.SalesmanId;
                            aTransaction.RefundDateTime = DateTime.Now;
                            aTransaction.RefundQuantity = aTransaction.Quantity;
                            aTransaction.Quantity = 0;
                            aTransaction.Price = 0;
                            db.Entry(aTransaction).State = EntityState.Modified;
                            refund = true;
                        }
                    }
                    else
                    {
                        previousQuantity = aTransaction.Quantity;
                        refundQuantity = aTransaction.Quantity;
                        aTransaction.RefundStatus = 1;
                        aTransaction.RefundBy = model.SalesmanId;
                        aTransaction.RefundDateTime = DateTime.Now;
                        aTransaction.RefundQuantity = aTransaction.Quantity;
                        aTransaction.Quantity = 0;
                        aTransaction.Price = 0;
                        db.Entry(aTransaction).State = EntityState.Modified;
                        refund = true;
                    }
                    if (refund == true)
                    {
                        db.SaveChanges();

                        var aTransactionList = db.PosOrderTransactions.Where(a => a.OrderId == model.OrderId && a.Quantity > 0).ToList();
                        subTotalPrice = 0;
                        foreach (var list in aTransactionList)
                        {
                            subTotalPrice = subTotalPrice + (decimal)list.Price;
                        }
                        totalPrice = subTotalPrice;
                        if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
                        {
                            taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                            totalPrice = totalPrice + taxAmount;
                        }
                        if (aOrder.Discount > 0)
                        {
                            disCount = totalPrice * ((decimal)aOrder.Discount / 100);
                            totalPrice = totalPrice - disCount;
                        }
                        if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
                        {
                            taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                            totalPrice = totalPrice + taxAmount;
                        }
                        refundTrans = new RefundTransaction();
                        refundTrans.RefundId = aRefund.RefundId;
                        refundTrans.PreviousAmount = previousAmount;
                        refundTrans.RefundAmount = previousAmount - totalPrice;
                        refundTrans.CurrentAmount = totalPrice;
                        refundTrans.PreviousQuantity = (decimal)previousQuantity;
                        refundTrans.RefundQuantity = (decimal)refundQuantity;
                        refundTrans.ProductId = (int)aTransaction.ProductId;
                        db.RefundTransactions.Add(refundTrans);
                        db.SaveChanges();

                        previousAmount = totalPrice;
                        refund = false;
                    }
                }
                //refund payment save
                if (model.Payments != null)
                {
                    foreach (var payment in model.Payments)
                    {
                        refundPayment = new PaymentTransaction();
                        refundPayment.PaymentId = aRefund.RefundId;
                        refundPayment.Type = 2; // Type 2 for refund payment
                        refundPayment.InOut = false;// InOut false for release payment
                        refundPayment.MethodId = (int)model.MethodId;
                        refundPayment.PaymentTypeId = payment.PaymentTypeId;
                        refundPayment.PaymentBodyId = payment.PaymentBodyId;
                        refundPayment.Amount = payment.AmountPaid;
                        refundPayment.TransactionNo = payment.TransactionNo;
                        refundPayment.CreatedBy = (int)model.SalesmanId;
                        refundPayment.Date = DateTime.Now;
                        refundPayment.IsCreditPayment = false;
                        db.PaymentTransactions.Add(refundPayment);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == payment.PaymentBodyId);
                        account.Balance = account.Balance - payment.AmountPaid;

                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                if (model.TotalPrice == 0) //change order info an order 
                {
                    aOrder.Status = false;
                    aOrder.RefundBy = model.SalesmanId;
                    aOrder.RefundStatus = true;
                    aOrder.IsItemRefund = true;
                    aOrder.RefundDateTime = DateTime.Now;
                }
                else //change order info after refund a product
                {
                    aOrder.InvoiceAmount = model.TotalPrice;
                    aOrder.Discount = disCount;
                    aOrder.SubTotalPrice = model.SubTotalPrice;
                    aOrder.Tax = model.Tax;
                    aOrder.IsItemRefund = true;
                }
                db.Entry(aOrder).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult RefundInfo(long orderId)
        {
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            var refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            if(refundOrder != null)
            {
                if(refundOrder.RefundOrderId > 0)
                {
                    orderId = (long)refundOrder.RefundOrderId;
                    refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    list.Add(refundOrder);
                    while (refundOrder.RefundOrderId > 0)
                    {
                        orderId = (long)refundOrder.RefundOrderId;
                        refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                        list.Add(refundOrder);
                    }
                }
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult RefundArchive(long orderId)
        {
            var dateList = new List<RefundArchiveModel>();
            RefundArchiveModel model;
            var order = db.PosOrders.FirstOrDefault(a => a.OrderId == orderId);
            model = new RefundArchiveModel();
            model.OrderId = order.OrderId;
            model.Date = order.OrderDate.Date;
            dateList.Add(model);
            var refundList = db.ViewPosRefunds.Where(a => a.OrderId == orderId).ToList()
                .GroupBy(a => a.Date.Date)
                .Select(s => new { s.FirstOrDefault().OrderId, s.FirstOrDefault().Date });
            if(refundList.Any())
            {
                foreach(var alist in refundList)
                {
                    model = new RefundArchiveModel();
                    model.OrderId = (long)alist.OrderId;
                    model.Date = alist.Date.Date;
                    dateList.Add(model);
                }
            }
            return View(dateList.OrderBy(a => a.Date));
        }
        [EncryptedActionParameter]
        public ActionResult RefundVoucher(long orderId, DateTime date)
        {
            ViewBag.RefundDate = date;
            var refundList = db.ViewPosRefunds.Where(a => a.OrderId == orderId && DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(date))
                .GroupBy(a => a.ProductId)
                .Select(s => new { ProductId = s.FirstOrDefault().ProductId, OrderId = s.FirstOrDefault().OrderId, Quantity = s.Sum(a => a.RefundQuantity) })
                .ToList();

            decimal subTotalPrice = 0;
            decimal totalPrice = 0;
            decimal discount = 0;
            decimal taxAmount = 0;
            decimal itemTotalPrice = 0;
            var orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
            var aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            foreach (var item in orderTransactionList)
            {
                discount = 0;
                var aRefundList = refundList.FirstOrDefault(a => a.ProductId == item.ProductId);
                if(aRefundList != null)
                {
                    item.Quantity = item.OrderedQuantity - aRefundList.Quantity;
                }
                else
                {
                    item.Quantity = item.OrderedQuantity;
                }
                itemTotalPrice = item.Quantity * item.PerItemPrice;
                if (item.Discount > 0)
                {
                    discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                    itemTotalPrice = itemTotalPrice - discount;
                    item.Discount = discount;
                }
                subTotalPrice = subTotalPrice + (decimal)itemTotalPrice;
                item.Price = itemTotalPrice;
            }
            totalPrice = subTotalPrice;
            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
            {
                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                totalPrice = totalPrice + taxAmount;
            }
            if (aOrder.Discount > 0)
            {
                discount = 0;
                discount = totalPrice * ((decimal)aOrder.Discount / 100);
                totalPrice = totalPrice - discount;
                aOrder.Discount = discount;
            }
            if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
            {
                taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
                totalPrice = totalPrice + taxAmount;
            }
            aOrder.SubTotalPrice = subTotalPrice;
            aOrder.InvoiceAmount = totalPrice;
            aOrder.Tax = taxAmount;
            ViewBag.OrderTransList = orderTransactionList.Where(a => a.Quantity > 0).ToList();
            return View(aOrder);
        }
        [EncryptedActionParameter]
        public ActionResult OrderCurrentVoucher(long orderId)
        {
            return View(db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId));
        }
        //Refund pay in pos
        //public PartialViewResult RefundPay(OrderModel model, long? OrderTransId)
        //{
        //    decimal subTotalPrice = 0;
        //    decimal totalPrice = 0;
        //    decimal discount = 0;
        //    decimal taxAmount = 0;
        //    decimal itemTotalPrice = 0;
        //    var refundList = new List<ViewOrderTransaction>();
        //    var aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == model.OrderId);
        //    var orderTransList = new List<ViewOrderTransaction>();
        //    if (OrderTransId > 0)
        //    {
        //        var orderTrans = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == OrderTransId);
        //        var newLIst = model.OrderProducts.FirstOrDefault(a => a.ProductId == orderTrans.ProductId);
        //        if (newLIst != null)
        //        {
        //            if (orderTrans.Quantity > newLIst.Quantity)
        //            {
        //                orderTrans.Quantity = orderTrans.Quantity - newLIst.Quantity;
        //                itemTotalPrice = orderTrans.Quantity * orderTrans.PerItemPrice;
        //                if (orderTrans.DiscountPercent > 0)
        //                {
        //                    discount = (decimal)(itemTotalPrice * (orderTrans.DiscountPercent / 100));
        //                    itemTotalPrice = itemTotalPrice - discount;
        //                    orderTrans.Discount = discount;
        //                }
        //                orderTrans.Price = itemTotalPrice;
        //                refundList.Add(orderTrans);
        //            }
        //        }
        //        else
        //        {
        //            refundList.Add(orderTrans);
        //        }
        //    }
        //    else
        //    {
        //        orderTransList = db.ViewOrderTransactions.Where(a => a.OrderId == model.OrderId && a.Quantity > 0).ToList();
        //        foreach (var list in orderTransList)
        //        {
        //            if (model.OrderProducts != null)
        //            {
        //                var newLIst = model.OrderProducts.FirstOrDefault(a => a.ProductId == list.ProductId);
        //                if (newLIst != null)
        //                {
        //                    if (list.Quantity > newLIst.Quantity)
        //                    {
        //                        list.Quantity = list.Quantity - newLIst.Quantity;
        //                        itemTotalPrice = list.Quantity * list.PerItemPrice;
        //                        if (list.DiscountPercent > 0)
        //                        {
        //                            discount = (decimal)(itemTotalPrice * (list.DiscountPercent / 100));
        //                            itemTotalPrice = itemTotalPrice - discount;
        //                            list.Discount = discount;
        //                        }
        //                        list.Price = itemTotalPrice;
        //                        refundList.Add(list);
        //                    }
        //                }
        //                else
        //                {
        //                    refundList.Add(list);
        //                }
        //            }
        //            else if(list.IsRefundAllow == true)
        //            {
        //                refundList.Add(list);
        //            }
        //        }
        //    }
        //    subTotalPrice = refundList.Sum(s => s.Price);
        //    totalPrice = subTotalPrice;
        //    if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == true)
        //    {
        //        taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
        //        totalPrice = totalPrice + taxAmount;
        //    }
        //    if (aOrder.DiscPercent > 0)
        //    {
        //        discount = 0;
        //        discount = totalPrice * (aOrder.DiscPercent / 100);
        //        totalPrice = totalPrice - discount;
        //        aOrder.Discount = discount;
        //    }
        //    if (aOrder.TaxPercent > 0 && aOrder.TaxFunc == false)
        //    {
        //        taxAmount = totalPrice * ((decimal)aOrder.TaxPercent / 100);
        //        totalPrice = totalPrice + taxAmount;
        //    }
        //    aOrder.SubTotalPrice = decimal.Round(subTotalPrice, 2, MidpointRounding.AwayFromZero);
        //    aOrder.TotalPrice = decimal.Round(totalPrice, 2, MidpointRounding.AwayFromZero);
        //    aOrder.Tax = taxAmount;
        //    ViewBag.RefundList = refundList;
        //    return PartialView(aOrder);
        //}


        public PartialViewResult RefundPay(OrderModel model, long? orderId)
        {

            if(orderId > 0)
            {
                var posOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                model.OrderId = posOrder.OrderId;
                model.InvoiceNo = posOrder.OrderNumber;
                model.TotalPrice = posOrder.InvoiceAmount;
                model.SubTotalPrice = posOrder.SubTotalPrice;
                model.CustomerId = posOrder.CustomerId;
                model.CustomerName = posOrder.CustomerName;
                model.Tax = posOrder.Tax;
                if(posOrder.TaxFunc == true)
                {
                    model.TaxFunc = 1;
                }
                else
                {
                    model.TaxFunc = 0;
                }
                model.TaxPercent = posOrder.TaxPercent;
                model.Discount = (decimal)posOrder.Discount;
                if(posOrder.DiscountType == true)
                {
                    model.DiscType = 1;
                }
                else
                {
                    model.DiscType = 0;
                }
                model.InvoiceTotal = posOrder.InvoiceAmount;
                model.RefundAmount = posOrder.InvoiceAmount;
                model.TotalPoints = 0;
                if(posOrder.TotalPoints > 0)
                {
                    model.TotalPoints = (int)posOrder.TotalPoints;
                }

            }

            if (model.CustomerId > 0)
            {
                decimal? prevDue = 0;
                decimal? CreditLimit = 0;
                model.AvailableCreditAmount = 0;
                model.IsCreditAllow = false;
                model.IsDebitAllow = false;
                model.DebitAmount = 0;
                var customer = db.ViewCustomers.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                //customer credit
                if (customer.IsCreditAllowed)
                {
                    model.IsCreditAllow = true;
                    //calcualte customer previous due
                    var customerCreditList = db.ViewCreditCustomers.Where(a => a.CustomerId == model.CustomerId && a.DueAmount > 0).ToList();
                    if (customerCreditList.Any())
                    {
                        prevDue = customerCreditList.Sum(a => a.DueAmount);
                    }
                    //customer credit limit
                    if (customer.CreditLimit > 0)
                    {
                        CreditLimit = customer.CreditLimit;
                    }
                    if (prevDue >= CreditLimit)
                    {
                        model.IsCreditAllow = false;
                    }
                    else
                    {
                        model.AvailableCreditAmount = CreditLimit - prevDue;
                    }
                }
                //customer debit
                if (customer.DebitLimitId > 0)
                {
                    if (customer.DebitAmount > 0)
                    {
                        model.IsDebitAllow = true;
                        model.DebitAmount = customer.DebitAmount;
                    }
                }
            }
            return PartialView(model);
        }
        #endregion
        //*********************payment by cash partial view*********************
        #region Cash
        public PartialViewResult Cash(decimal? totalPrice, decimal? cashAmount, bool? isRelease,bool? isReturn, decimal? cashPayment, int? methodId,bool? defaultType, bool? isSupplierDebitPay)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.CashAmount = cashAmount;
            ViewBag.IsRelease = isRelease;
            ViewBag.CashPayment = cashPayment;
            ViewBag.IsReturn = isReturn;
            ViewBag.MethodId = methodId;
            ViewBag.DefaultType = defaultType;
            ViewBag.IsSupplierDebitPay = isSupplierDebitPay;
            return PartialView();
        }
        public PartialViewResult CashPartial(decimal? totalPrice, decimal? cashAmount, bool? isRelease, decimal? cashPayment, bool? isReturn, int? methodId,bool? defaultType, bool? isSupplierDebitPay)
        {
            ViewBag.CashPayment = cashPayment;
            ViewBag.IsRelease = isRelease;
            ViewBag.IsReturn = isReturn;
            ViewBag.DefaultType = defaultType;
            ViewBag.TotalPrice = totalPrice;
            var inOut = 2;
            if (defaultType == true)
            {
                inOut = 1;
            }
            ViewBag.CashBody = new SelectList(db.ViewAssignedMethodNames.Where(a => a.MethodId == methodId && a.Status == true && a.PaymentTypeId == 1 && (a.InOut == 0 || a.InOut == inOut)).Select(a => new { a.AccountName, a.AccountId}), "AccountId", "AccountName");
            
            CashModel model = new CashModel();
            var defaultId = db.DefaultAccounts.FirstOrDefault(a => a.MethodId == methodId && a.Type == defaultType && a.PaymentTypeId == 1);
            if (defaultId != null)
            {
                model.CashBodyId = defaultId.AccountId;
            }
            if (cashAmount > 0)
            {
                model.AmountPaid = cashAmount;
            }

            if(isSupplierDebitPay == true)
            {
                ViewBag.DefaultType = null;
            }
            return PartialView(model);
        }
        #endregion
        //*********************payment by card partial view*********************
        #region Card
        public PartialViewResult Card(decimal? totalPrice, bool? isRelease, decimal? cardPayment, bool? isReturn,int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            ViewBag.CardPayment = cardPayment;
            ViewBag.TotalPrice = totalPrice;
            ViewBag.IsRelease = isRelease;
            ViewBag.IsReturn = isReturn;
            ViewBag.MethodId = methodId;
            ViewBag.DefaultType = defaultType;
            ViewBag.IsSupplierDebitPay = isSupplierDebitPay;
            return PartialView();
        }
        public PartialViewResult CardPartial(decimal? totalPrice, bool? isRelease, decimal? cardPayment, bool? isReturn, int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            var inOut = 2;
            if (defaultType == true)
            {
                inOut = 1;
            }
            ViewBag.PaymentBody = new SelectList(db.ViewAssignedMethodNames.Where(a => a.MethodId == methodId && a.Status == true && a.PaymentTypeId == 2 && (a.InOut == 0 || a.InOut == inOut)).Select(a => new { a.AccountName, a.AccountId }), "AccountId", "AccountName");

            CardModel model = new CardModel();
            var defaultId = db.DefaultAccounts.FirstOrDefault(a => a.MethodId == methodId && a.Type == defaultType && a.PaymentTypeId == 2);
            if (defaultId != null)
            {
                model.PaymentBodyId = defaultId.AccountId;
            }
            //if (isRelease == true || cardPayment > 0)
            //{
            //    ViewBag.PaymentBody = new SelectList(db.PaymentBodies.Where(a => a.PaymentTypeId == 2 && a.InOut == 2).Select(s => new { Name = s.Name + " ("+ s.SerialNumber +")", PaymentBodyId = s.PaymentBodyId}), "PaymentBodyId", "Name");
            //}
            //else
            //{
            //    ViewBag.PaymentBody = new SelectList(db.PaymentBodies.Where(a => a.PaymentTypeId == 2 && a.InOut == 1).Select(s => new { Name = s.Name + " (" + s.SerialNumber + ")", PaymentBodyId = s.PaymentBodyId }), "PaymentBodyId", "Name");
            //}
            ViewBag.TotalPrice = totalPrice;
            ViewBag.IsRelease = isRelease;
            ViewBag.CardPayment = cardPayment;
            ViewBag.IsReturn = isReturn;
            ViewBag.DefaultType = defaultType;
            if (isSupplierDebitPay == true)
            {
                ViewBag.DefaultType = null;
            }
            return PartialView(model);
        }
        #endregion
        //*********************payment by Bkash partial view*****
        #region Bkash
        public PartialViewResult Bcash(decimal? totalPrice, bool? isRelease, decimal? cashPayment, bool? isReturn, int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.IsRelease = isRelease;
            ViewBag.CashPayment = cashPayment;
            ViewBag.IsReturn = isReturn;
            ViewBag.MethodId = methodId;
            ViewBag.DefaultType = defaultType;
            ViewBag.IsSupplierDebitPay = isSupplierDebitPay;
            return PartialView();
        }
        public PartialViewResult BcashPartial(decimal? totalPrice, bool? isRelease, decimal? cashPayment, bool? isReturn, int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            //if(isRelease == true || cashPayment > 0)
            //{
            //    ViewBag.BkashBody = new SelectList(db.PaymentBodies.Where(a => a.PaymentTypeId == 3 && a.InOut == 2), "PaymentBodyId", "Name");
            //}
            //else
            //{
            //    ViewBag.BkashBody = new SelectList(db.PaymentBodies.Where(a => a.PaymentTypeId == 3 && a.InOut == 1), "PaymentBodyId", "Name");
            //}
            var inOut = 2;
            if (defaultType == true)
            {
                inOut = 1;
            }
            ViewBag.BkashBody = new SelectList(db.ViewAssignedMethodNames.Where(a => a.MethodId == methodId && a.Status == true && a.PaymentTypeId == 3 && (a.InOut == 0 || a.InOut == inOut)).Select(a => new { a.AccountName, a.AccountId }), "AccountId", "AccountName");
            BcashModel model = new BcashModel();
            var defaultId = db.DefaultAccounts.FirstOrDefault(a => a.MethodId == methodId && a.Type == defaultType && a.PaymentTypeId == 3);
            if (defaultId != null)
            {
                model.bodyId = defaultId.AccountId;
            }
            ViewBag.TotalPrice = totalPrice;
            ViewBag.IsRelease = isRelease;
            ViewBag.CashPayment = cashPayment;
            ViewBag.IsReturn = isReturn;
            ViewBag.DefaultType = defaultType;
            if (isSupplierDebitPay == true)
            {
                ViewBag.DefaultType = null;
            }
            return PartialView(model);
        }
        #endregion
        //*********************payment by Check partial view*****
        #region Check
        public PartialViewResult Check(decimal? totalPrice, int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.MethodId = methodId;
            ViewBag.DefaultType = defaultType;
            ViewBag.IsSupplierDebitPay = isSupplierDebitPay;
            return PartialView();
        }
        public PartialViewResult CheckPartial(decimal? totalPrice, int? methodId, bool? defaultType, bool? isSupplierDebitPay)
        {
            var inOut = 2;
            if (defaultType == true)
            {
                inOut = 1;
            }
            ViewBag.CheckBody = new SelectList(db.ViewAssignedMethodNames.Where(a => a.MethodId == methodId && a.Status == true && a.PaymentTypeId == 6 && (a.InOut == 0 || a.InOut == inOut)).Select(a => new { a.AccountName, a.AccountId }), "AccountId", "AccountName");
            CheckModel model = new CheckModel();
            var defaultId = db.DefaultAccounts.FirstOrDefault(a => a.MethodId == methodId && a.Type == defaultType && a.PaymentTypeId == 6);
            if (defaultId != null)
            {
                model.CheckBodyId = defaultId.AccountId;
            }
            ViewBag.TotalPrice = totalPrice;
            ViewBag.IsSupplierDebitPay = isSupplierDebitPay;
            return PartialView(model);
        }
        #endregion 
        //*********************partial view of add product UnitWise to cart*****
        #region Unit Wise
        public PartialViewResult Unitwise(int productId)
        {
            return PartialView(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId));
        }
        public PartialViewResult UnitwisePartial()
        {
            return PartialView();
        }
        #endregion
        //*********************import and sale report **************************
        #region Report
        public ActionResult Report()
        {
            return View();
        }
        //total cost report 
        public PartialViewResult TotalCost()
        {
            return PartialView();
        }
        public PartialViewResult TotalCostPartial()
        {
            return PartialView();
        }
        public PartialViewResult TotalCostList(DateTime? FromDate, DateTime? ToDate)
        {
            decimal totalCost = 0;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var stockImportList = db.ViewStockImports.ToList();
            if(FromDate != null && ToDate != null)
            {
                stockImportList = stockImportList.Where(a => a.Date >= FromDate && a.Date <= ToDate).ToList();
                //calculate totalcost
                foreach(var aImport in stockImportList)
                {
                    totalCost = totalCost + aImport.TotalCost;
                }
                ViewBag.TotalCost = totalCost;
            }
            return PartialView(stockImportList);
        }
        //total sale report
        public PartialViewResult TotalSale()
        {
            return PartialView();
        }
        public PartialViewResult TotalSalePartial()
        {
            return PartialView();
        }
        //Total sale List
        public PartialViewResult TotalSaleList(DateTime FromDate, DateTime ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewPosOrder> totalSaleList = new List<ViewPosOrder>();
            if(FromDate != null && ToDate != null)
            {
                totalSaleList = db.ViewPosOrders.Where( a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
            }
            return PartialView(totalSaleList);
        }
        //View Import Transaction
        public PartialViewResult ImportTransaction(long importId)
        {
            ViewBag.ImportId = importId;
            return PartialView(db.ViewStockImports.FirstOrDefault(a => a.StockImportId == importId));
        }
        //public PartialViewResult ImportTransactionPartial(long importId)
        //{
        //    return PartialView(db.ViewImportTransactions.Where(a => a.StockImportId == importId).ToList());
        //}
        //View Sale Transaction
        public PartialViewResult OrderTransaction(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult OrderTranPartial(long orderId)
        {
            return PartialView(db.ViewPosOrders.FirstOrDefault(a=>a.OrderId == orderId));
        }
        #endregion
        //*********************Print report ************************************
        #region Print Function
        //Print Sale report
        [EncryptedActionParameter]
        public ActionResult PrintSaleReport(DateTime FromDate, DateTime ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewPosOrder> totalSaleList = new List<ViewPosOrder>();
            if (FromDate != null && ToDate != null)
            {
                totalSaleList = db.ViewPosOrders.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
            }
            return View(totalSaleList);
        }
        //Print Total Cost Report
        [EncryptedActionParameter]
        public ActionResult PrintImportReport(DateTime? FromDate, DateTime? ToDate, decimal? TotalCost)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.TotalCost = TotalCost;
            var stockImportList = db.ViewStockImports.ToList();
            if (FromDate != null && ToDate != null)
            {
                stockImportList = stockImportList.Where(a => a.Date >= FromDate && a.Date <= ToDate).ToList();
            }
            return View(stockImportList);
        }
        #endregion
        //*********************Availability of Product**************************
        #region Availability Of Product
        public PartialViewResult AvailabilityOfProduct(int productId, long? distributeId)
        {
            var stockProduct = new ViewStockProduct();
            if(distributeId > 0)
            {
                stockProduct = db.ViewStockProducts.FirstOrDefault(a => a.ProductId == productId && a.DistributeId == distributeId);
            }
            else
            {
                stockProduct = db.ViewStockProducts.FirstOrDefault(a => a.ProductId == productId);
            }
            return PartialView(stockProduct);
        }
        #endregion
        //*********************Checkout*****************************************
        #region Checkout
        public PartialViewResult Checkout(OrderModel model)
        {
            ViewBag.IsDebitPayByCredit = false;
            if(model.CustomerId > 0)
            {
                decimal? prevDue = 0;
                decimal? CreditLimit = 0;
                model.AvailableCreditAmount = 0;
                model.IsCreditAllow = false;
                model.IsDebitAllow = false;
                model.DebitAmount = 0;
                var customer = db.ViewCustomers.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                //customer credit
                if (customer.IsCreditAllowed)
                {
                    model.IsCreditAllow = true;
                    //calcualte customer previous due
                    var customerCreditList = db.ViewCreditCustomers.Where(a => a.CustomerId == model.CustomerId && a.DueAmount > 0).ToList();
                    if(customerCreditList.Any())
                    {
                        prevDue = customerCreditList.Sum(a => a.DueAmount);
                    }
                    //customer credit limit
                    if(customer.CreditLimit > 0)
                    {
                        CreditLimit = customer.CreditLimit;
                    }
                    if(prevDue >= CreditLimit)
                    {
                        model.IsCreditAllow = false;
                    }
                    else
                    {
                        model.AvailableCreditAmount = CreditLimit - prevDue;
                    }
                }
                //customer debit
                if(customer.DebitLimitId > 0)
                {
                    if(customer.DebitAmount > 0)
                    {
                        model.IsDebitAllow = true;
                        model.DebitAmount = customer.DebitAmount;
                    }
                }

                //10 for IsCreditPayByCredit function
                var miscFunc = db.MiscFuntions.Find(10);
                if(miscFunc.Status == true)
                {
                    ViewBag.IsDebitPayByCredit = true;
                }
            }
            return PartialView(model);
        }
        #endregion
        //*********************Add Client***************************************
        #region Client
        public PartialViewResult Client()
        {
            return PartialView();
        }
        public PartialViewResult ClientList()
        {
            return PartialView(db.ViewCustomers.Where(a => a.Status == true).ToList());
        }
        #endregion
        //*********************Credit***************************************
        #region Credit
        public PartialViewResult Credit(decimal? totalPrice, decimal? creditAmount, decimal? dueAmount, decimal? availabelCredit)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.CreditAmount = creditAmount;
            ViewBag.DueAmount = dueAmount;
            //customer available credit amount
            ViewBag.AvailabelCredit = availabelCredit;
            return PartialView();
        }
        public PartialViewResult CreditPartial(decimal? totalPrice, decimal? creditAmount, decimal? dueAmount, decimal? availabelCredit)
        {
            if(dueAmount == null)
            {
                dueAmount = 0;
            }
            
            if(availabelCredit == null)
            {
                availabelCredit = 0;
            }
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DueAmount = dueAmount;
            //customer available credit amount
            ViewBag.AvailabelCredit = availabelCredit;
            if(creditAmount > 0)
            {
                CreditModel model = new CreditModel();
                model.CreditAmount = creditAmount;
                return PartialView(model);
            }
            return PartialView();
        }

        #endregion
        //*********************Debit***************************************
        #region Debit
        public PartialViewResult Debit(decimal? totalPrice, decimal? debitAmount, decimal? availableDebitAmount)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DebitAmount = debitAmount;
            ViewBag.AvailableDebitAmount = availableDebitAmount;
            return PartialView();
        }
        public PartialViewResult DebitPartial(decimal? totalPrice, decimal? debitAmount, decimal? availableDebitAmount)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DebitAmount = debitAmount;
            ViewBag.AvailableDebitAmount = availableDebitAmount;
            ViewBag.ValidAmount = 0;
            if(totalPrice >= availableDebitAmount)
            {
                ViewBag.ValidAmount = availableDebitAmount;
            }
            else
            {
                ViewBag.ValidAmount = totalPrice;
            }
            if (debitAmount > 0)
            {
                DebitModelView model = new DebitModelView();
                model.DebitAmount = debitAmount;
                return PartialView(model);
            }
            return PartialView();
        }
        #endregion
        //*********************Order Voucher***************************************
        [EncryptedActionParameter]
        public ActionResult OrderVoucher(long orderId)
        {
            return View(db.ViewPosOrders.FirstOrDefault(a=>a.OrderId == orderId));
        }
        #region Credit Customer and Credit Payment
        public PartialViewResult CustomerCreditList(string selectedId, int? count, int? days, DateTime? from, DateTime? to, bool? isPrint)
        {
            if (isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            else
            {
                ViewBag.IsPrint = false;
            }
            var list = new List<ViewCreditCustomer>();
            DateTime? start = from;
            DateTime? end = to;
            if (count > 0)
            {
                list = db.ViewCreditCustomers.Where(a => a.DueAmount > 0).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewCreditCustomers.Where(m => m.DueAmount > 0 && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewCreditCustomers.Where(m => m.DueAmount > 0 && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
            }
            if (!string.IsNullOrEmpty(selectedId))
            {
                list.Clear();
                foreach (var id in selectedId.Split(','))
                {
                    var customerId = Convert.ToInt64(id);
                    var aCredit = db.ViewCreditCustomers.Where(a=> a.CustomerId == customerId && a.DueAmount > 0).ToList();
                    list.AddRange(aCredit);
                }
            }
            return PartialView(list.OrderByDescending(a => a.OrderDate).ToList());
        }
        public JsonResult GetCreditCustomerList(int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = db.ViewCreditCustomers.Where(a => a.DueAmount > 0).ToList();
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = list.Where(m =>  m.OrderDate.Date == countDate.Date).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = list.Where(m => m.OrderDate.Date >= start.Value.Date && m.OrderDate.Date <= end.Value.Date).ToList();
            }
            var customerList = new SelectList(list.GroupBy(a => a.CustomerId).Select(s => new { CustomerId = s.FirstOrDefault().CustomerId, Name = s.FirstOrDefault().Name }).ToList(), "CustomerId", "Name");
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }
        //Credit customer Print
        public ActionResult CreditCustomerPrint(string q)
        {
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult CustomerCredit(long? customerId)
        {
            ViewBag.CustomerId = customerId;
            return View();
        }
        public PartialViewResult CustomerCreditPartial(long? customerId)
        {
            return PartialView(db.ViewCreditCustomers.Where(a => a.CustomerId == customerId && a.DueAmount > 0).OrderBy(o => o.OrderDate).ToList());
        }
        public PartialViewResult CreditPay(decimal? totalDue)
        {
            ViewBag.TotalDue = totalDue;
            return PartialView();
        }
        public JsonResult CreditPaySave(IList<PaymentModel> Payments, int UserId, long OrderId, decimal? ReturnAmount, bool IsTotalPay, long CustomerId, int MethodId)
        {
            PaymentBody account;
            try
            {
                if(IsTotalPay == true)
                {
                    decimal? amount = 0;
                    decimal? amountPaid = 0;
                    long paymentTransactionId = 0;
                    foreach (var paymentTransaction in Payments)
                    {
                        amount = paymentTransaction.AmountPaid;
                        var creditList = db.ViewCreditCustomers.Where(a => a.CustomerId == CustomerId && a.DueAmount > 0).OrderBy(a => a.OrderDate).ToList();
                        if(creditList != null)
                        {
                            foreach (var list in creditList)
                            {
                                if (amount > 0)
                                {
                                    var payment = db.OrderPayments.FirstOrDefault(a => a.OrderId == list.OrderId);
                                    if (amount >= payment.DueAmount)
                                    {
                                        amountPaid = payment.DueAmount;
                                        amount = amount - payment.DueAmount;
                                        payment.DueAmount = 0;
                                    }
                                    else
                                    {
                                        amountPaid = amount;
                                        payment.DueAmount = payment.DueAmount - amount;
                                        amount = 0;
                                    }
                                    payment.AmountPaid = payment.AmountPaid + (decimal)amountPaid;
                                    db.Entry(payment).State = EntityState.Modified;

                                    //if purchase order exist 
                                    if (db.PosOrders.FirstOrDefault(a => a.OrderId == list.OrderId).PurchaseOrderId > 0)
                                    {
                                        decimal poAmountPaid = 0;
                                        poAmountPaid = (decimal)amountPaid;
                                        long purchaseId = (long)db.PosOrders.Find(list.OrderId).PurchaseOrderId;
                                        var purchasOrder = db.Purchases.Find(purchaseId);

                                        if (purchasOrder != null)
                                        {
                                            if (purchasOrder.DueAmount > 0)
                                            {
                                                purchasOrder.DueAmount = purchasOrder.DueAmount - poAmountPaid;
                                            }

                                            if (purchasOrder.DispatchAmount > 0)
                                            {
                                                purchasOrder.DispatchAmount = purchasOrder.DispatchAmount + poAmountPaid;
                                            }
                                            else
                                            {
                                                purchasOrder.DispatchAmount = poAmountPaid;
                                            }
                                            db.Entry(purchasOrder).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }

                                    var paymentTrans = new PaymentTransaction();
                                    paymentTrans.PaymentId = payment.OrderId;
                                    paymentTrans.Type = 1; // Type 1 for order payment 
                                    paymentTrans.InOut = true; // InOut true for receive payment
                                    paymentTrans.MethodId = MethodId;
                                    paymentTrans.PaymentTypeId = paymentTransaction.PaymentTypeId;
                                    paymentTrans.PaymentBodyId = paymentTransaction.PaymentBodyId;
                                    paymentTrans.Amount = (decimal)amountPaid;
                                    paymentTrans.TransactionNo = paymentTransaction.TransactionNo;
                                    paymentTrans.Date = DateTime.Now;
                                    paymentTrans.CreatedBy = UserId;
                                    paymentTrans.IsCreditPayment = true;
                                    db.PaymentTransactions.Add(paymentTrans);
                                    db.SaveChanges();

                                    //add amount in account balance
                                    account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == paymentTransaction.PaymentBodyId);
                                    account.Balance = account.Balance + payment.AmountPaid;

                                    db.Entry(account).State = EntityState.Modified;


                                    paymentTransactionId = paymentTrans.PaymentTransactionId;
                                }
                            }
                        }
                        if(amount > 0)
                        {
                            var aPayment = db.PaymentTransactions.FirstOrDefault(a => a.PaymentTransactionId == paymentTransactionId);
                            var payment = db.OrderPayments.FirstOrDefault(a => a.OrderId == aPayment.PaymentId);
                            payment.AmountPaid = payment.AmountPaid + (decimal)amount;
                            payment.ReturnAmount = payment.ReturnAmount + amount;
                            db.Entry(payment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var payment = db.OrderPayments.FirstOrDefault(a => a.OrderId == OrderId);
                    payment.AmountPaid = payment.AmountPaid + (decimal)payment.DueAmount;
                    payment.DueAmount = 0;
                    if (ReturnAmount > 0)
                    {
                        payment.ReturnAmount = ReturnAmount;
                    }
                    db.Entry(payment).State = EntityState.Modified;
                    db.SaveChanges();

                    //if purchase order exist 
                    if (db.PosOrders.FirstOrDefault(a => a.OrderId == OrderId).PurchaseOrderId > 0)
                    {
                        decimal poAmountPaid = 0;
                        poAmountPaid = Payments.Sum(a => a.AmountPaid);
                        if(ReturnAmount > 0)
                        {
                            poAmountPaid = poAmountPaid - (decimal)ReturnAmount;
                        }
                        long purchaseId =(long)db.PosOrders.Find(OrderId).PurchaseOrderId;
                        var purchasOrder = db.Purchases.Find(purchaseId);

                        if(purchasOrder != null)
                        {
                            if(purchasOrder.DueAmount > 0)
                            {
                                purchasOrder.DueAmount = purchasOrder.DueAmount - poAmountPaid;
                            }

                            if(purchasOrder.DispatchAmount > 0)
                            {
                                purchasOrder.DispatchAmount = purchasOrder.DispatchAmount + poAmountPaid;
                            }
                            else
                            {
                                purchasOrder.DispatchAmount = poAmountPaid;
                            }
                            db.Entry(purchasOrder).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    foreach (var list in Payments)
                    {
                        var aPayment = new PaymentTransaction();
                        aPayment.PaymentId = OrderId;
                        aPayment.Type = 1; // type 1 for order payment
                        aPayment.InOut = true; // InOut true for receive payment
                        aPayment.MethodId = MethodId;
                        aPayment.PaymentTypeId = list.PaymentTypeId;
                        aPayment.PaymentBodyId = list.PaymentBodyId;
                        aPayment.Amount = list.AmountPaid;
                        aPayment.TransactionNo = list.TransactionNo;
                        aPayment.Date = DateTime.Now;
                        aPayment.CreatedBy = UserId;
                        aPayment.IsCreditPayment = true;
                        db.PaymentTransactions.Add(aPayment);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                        account.Balance = account.Balance + payment.AmountPaid;

                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Order Payment History
        [EncryptedActionParameter]
        public ActionResult PaymentHistory(long customerId, bool iscreditHistory)
        {
            ViewBag.IscreditHistory = iscreditHistory;
            return View(db.Customers.Find(customerId));
        }
        public PartialViewResult CreditPaymentHistory(long? customerId, int? days, DateTime? from, DateTime? to, int? count, bool? isPrint)
        {
            if(isPrint == true)
            {
                ViewBag.IsPrint = isPrint;
            }
            else
            {
                ViewBag.IsPrint = false;
            }
            List<ViewOrderTransaction> list = new List<ViewOrderTransaction>();
            DateTime? start = from;
            DateTime? end = to;
            if (count > 0)
            {
                list = db.ViewOrderTransactions.Where(a => a.CustomerId == customerId && a.TransactionType == 2).OrderByDescending(o => o.OrderDate).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 2 && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 2 && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.OrderDate));
        }
        public PartialViewResult CreditPaymentHistoryPartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult CreditPaymentHistoryList(long orderId)
        {
            var creditPaymentList = db.ViewPayments.Where(a => a.PaymentId == orderId && a.Type == 1 && a.IsCreditPayment == true).ToList();
            return PartialView(creditPaymentList);
        }
        public PartialViewResult PaymentHistoryList(long? customerId,long? supplierId, int? accId, long? orderId, int? days, DateTime? from, DateTime? to, int? count, bool? isDebitTrans)
        {
            List<ViewPayment> list = new List<ViewPayment>();
            if(accId > 0)
            {
                list = db.ViewPayments.Where(a => a.PaymentBodyId == accId).ToList();
            }
            else if (orderId > 0)
            {
                list = db.ViewPayments.Where(a => (a.PaymentId == orderId || a.CreditPaymentId == orderId) && a.Type == 1).ToList();
                if (list.Any(a => a.RefOrderId > 0))
                {
                    long refOrderId = (long)list.FirstOrDefault().RefOrderId;
                    var newList = db.ViewPayments.Where(a => (a.PaymentId == refOrderId || a.CreditPaymentId == refOrderId) && a.Type == 1).ToList();
                    list.AddRange(newList);
                    while(newList.Any(a => a.RefOrderId > 0))
                    {
                        refOrderId = (long)newList.FirstOrDefault().RefOrderId;
                        newList = db.ViewPayments.Where(a => (a.PaymentId == refOrderId || a.CreditPaymentId == refOrderId) && a.Type == 1).ToList();
                        list.AddRange(newList);
                    }
                }
            }
            else if(customerId > 0)
            {
                DateTime? start = from;
                DateTime? end = to;
                if (isDebitTrans == true)
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.CustomerId == customerId && a.PaymentBodyId == 17).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.CustomerId == customerId && m.PaymentBodyId == 17 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.CustomerId == customerId && m.PaymentBodyId == 17 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.CustomerId == customerId).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.CustomerId == customerId && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.CustomerId == customerId && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
            }
            else if(supplierId > 0)
            {
                DateTime? start = from;
                DateTime? end = to;
                if (isDebitTrans == true)
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.SupplierId == supplierId && a.PaymentBodyId == 19).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.SupplierId == supplierId && m.PaymentBodyId == 19 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.SupplierId == supplierId && m.PaymentBodyId == 19 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.SupplierId == supplierId).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.SupplierId == supplierId && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.SupplierId == supplierId && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        public PartialViewResult PaymentHistoryPartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public ActionResult PaymentHistoryPrint(string q)
        {
            long customerId = 0;
            int days = 0;
            int listType = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                customerId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
                listType = Convert.ToInt32(Convert.ToUInt32(splitbyand[4].Split('=')[1]));
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.ListType = listType;
            return View(db.Customers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        [EncryptedActionParameter]
        public ActionResult CreditPaymentLedger(long? customerId)
        {
            ViewBag.CustomerId = customerId;
            return View(db.Customers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        public PartialViewResult CreditPaymentLedgerList(long? customerId, int? count, int? days, DateTime? from, DateTime? to, bool? isPrint)
        {
            if (isPrint == true)
            {
                ViewBag.IsPrint = isPrint;
            }
            else
            {
                ViewBag.IsPrint = false;
            }
            List<ViewPayment> list = new List<ViewPayment>();
            DateTime? start = from;
            DateTime? end = to;
            if (count > 0)
            {
                list = db.ViewPayments.Where(a => a.CustomerId == customerId && (a.IsCreditPayment == true || a.PaymentBodyId == 16)).OrderByDescending(o => o.Date).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewPayments.Where(m => m.CustomerId == customerId && (m.IsCreditPayment == true || m.PaymentBodyId == 16) && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewPayments.Where(m => m.CustomerId == customerId && (m.IsCreditPayment == true || m.PaymentBodyId == 16) && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        public ActionResult CreditPaymentLedgerPrint(string q)
        {
            long customerId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                customerId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View(db.Customers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        #endregion

        #region Customer add by membership Card no
        public JsonResult GetCustomerByCardNoOrId(string cardNo, long? id)
        {
            CustomerModelView model = new CustomerModelView();
            Customer customer = new Customer(); 
            if(!string.IsNullOrEmpty(cardNo))
            {
                customer = db.Customers.FirstOrDefault(a => a.MembershipNumber == cardNo);
            }
            else if(id > 0)
            {
                customer = db.Customers.FirstOrDefault(a => a.CustomerId == id);
            }
            if (customer != null)
            {
                model.CustomerId = customer.CustomerId;
                model.Name = customer.Name;
                model.IsCreditAllowed = customer.IsCreditAllowed;
                model.CreditAmount = 0;
                if(customer.IsCreditAllowed == true)
                {
                    decimal totalDue = 0;
                    if (db.ViewPosOrders.Any(a => a.CustomerId == customer.CustomerId && a.DueAmount > 0))
                    {
                        totalDue = db.ViewPosOrders.Where(a => a.CustomerId == customer.CustomerId && a.DueAmount > 0).Sum(s => s.DueAmount).Value;
                    }
                    model.CreditAmount = totalDue;
                }
                model.IsDebitAccounts = false;
                model.DebitLimit = 0;
                model.AvailableDebit = 0;
                if (customer.DebitLimitId > 0)
                {
                    model.IsDebitAccounts = true;
                    var debitAccount = db.DebitLimits.FirstOrDefault(a => a.CustomerId == customer.CustomerId);
                    if (debitAccount != null)
                    {
                        model.IsDebitAccounts = true;
                        model.DebitLimit = debitAccount.Limit;
                        model.AvailableDebit = debitAccount.Amount;
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Custom Offer 
        public ActionResult CustomOffer()
        {
            return View();
        }
        //public PartialViewResult OfferCreate(int? id)
        //{
        //    ViewBag.Id = id;
        //    return PartialView();
        //}
        public PartialViewResult OfferEdit(long? id, bool? IsEvent, long? eventId)
        {
            ViewBag.Id = id;
            ViewBag.EventId = eventId;
            ViewBag.IsEvent = false;
            if(IsEvent == true)
            {
                ViewBag.IsEvent = true;
            }
            return PartialView();
        }
        public PartialViewResult OfferCreatePartial(long? id)
        {
            ViewBag.ScheduleList = new SelectList(db.Schedules.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            if (id > 0)
            {
                var offerModel = new OfferModelView();
                var offer = db.Offers.Find(id);
                offerModel.Id = offer.Id;
                offerModel.MasterOfferId = offer.MasterOfferId;
                offerModel.ScheduleId = offer.ScheduleId;
                offerModel.OfferName = offer.OfferName;
                //offerModel.ValidityDate = offer.Validity;
                offerModel.IsEditable = offer.IsEditable;
                return PartialView(offerModel);
            }
            return PartialView();
        }
        public JsonResult OfferSave(OfferModelView model)
        {
            try
            {
                var offer = new Offer();
                if (model.Id > 0)
                {
                    offer = db.Offers.Find(model.Id);
                    offer.OfferName = model.OfferName;
                    if (model.MasterOfferId == 0 || model.MasterOfferId == null)
                    {
                        offer.ScheduleId = model.ScheduleId;
                        offer.IsEditable = model.IsEditable;
                        if (model.IsDateValidity)
                        {
                            //offer.Validity = (DateTime)model.ValidityDate;
                        }
                        else
                        {
                            DateTime date;
                            date = now.AddDays((int)model.ValidityDays).Date;
                            date = date.Add(model.ValidityTime.Value.TimeOfDay);
                           // offer.Validity = date;
                        }
                    }
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                    db.SaveChanges();

                    //Schedule update in item
                    if(model.MasterOfferId == 0 || model.MasterOfferId == null)
                    {
                        var offerItemList = db.OfferItems.Where(a => a.OfferId == offer.Id && a.Status == true).ToList();
                        if (offerItemList.Any())
                        {
                            foreach (var item in offerItemList)
                            {
                                item.ScheduleId = model.ScheduleId;
                                //item.Validity = (DateTime)offer.Validity;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                   
                }
                else
                {
                    offer = new Offer();
                    offer.Type = model.type;
                    offer.OfferName = model.OfferName;
                    offer.IsEditable = model.IsEditable;
                    offer.ScheduleId = model.ScheduleId;
                    if(model.IsDateValidity)
                    {
                        //offer.Validity = (DateTime)model.ValidityDate;
                    }
                    else
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);
                        //offer.Validity = date;
                    }
                    offer.Status = true;
                    offer.ActualPrice = 0;
                    offer.OfferPrice = 0;
                    offer.DiscAmount = 0;
                    offer.DiscPercentage = 0;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    db.Offers.Add(offer);
                    db.SaveChanges();
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult OfferList(int? status, bool? isInPOS, int? productId, long? offerId, long? eventId, DateTime? date, int? sorterType)
        {
            ViewBag.isInPOS = false;
            if (isInPOS == true)
            {
                ViewBag.isInPOS = true;
            }
            var list = new List<ViewOffer>();
            ViewOffer offer;
            if (productId > 0)
            {
                var offerIds = db.OfferItems.Where(a => a.ProductId == productId && a.OfferId > 0 && a.Type == 2).GroupBy(a => a.OfferId).Select(a => new { a.FirstOrDefault().OfferId }).ToList();
                if(offerIds != null)
                {
                    foreach(var id in offerIds)
                    {
                        offer = db.ViewOffers.FirstOrDefault(a => a.Id == id.OfferId && a.Type == 2);
                        if(offer != null)
                        {
                            list.Add(offer);
                        }

                    }
                }
            }
            else if(offerId > 0)
            {
                offer = db.ViewOffers.FirstOrDefault(a => a.Id == offerId && a.Type == 2);
                if(offer != null)
                {
                    list.Add(offer);
                }
            }
            else if(eventId > 0)
            {
                if (status == 1) // active
                {
                    list = db.ViewOffers.Where(a => a.Status == true && a.MasterOfferId == eventId).ToList();
                }
                else if (status == 0) // inactive
                {
                    list = db.ViewOffers.Where(a => a.Status == false && a.MasterOfferId == eventId).ToList();
                }
                else if (status == 2) // delete
                {
                    list = db.ViewOffers.Where(a => a.Status == null && a.MasterOfferId == eventId).ToList();
                }
                else
                {
                    list = db.ViewOffers.Where(a => a.Status != null && a.MasterOfferId == eventId).ToList();
                }
            }
            else if(date != null)
            {
                list = db.ViewOffers.Where(a => a.Type == 2 && DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(a.StartDate)).ToList();
            }
            else
            {
                if(eventId == null)
                {
                    if (status == 1) // active
                    {
                        list = db.ViewOffers.Where(a => a.Status == true && a.Type == 2).ToList();
                    }
                    else if (status == 0) // inactive
                    {
                        list = db.ViewOffers.Where(a => a.Status == false && a.Type == 2).ToList();
                    }
                    else if (status == 2) // delete
                    {
                        list = db.ViewOffers.Where(a => a.Status == null && a.Type == 2).ToList();
                    }
                    else
                    {
                        list = db.ViewOffers.Where(a => a.Status != null && a.Type == 2).ToList();
                    }
                }
            }

            if(sorterType > 0)
            {
                if(sorterType == 1) //discount amount hight to low
                {
                    list = list.OrderByDescending(a => a.DiscAmount).ToList();
                }
                else if(sorterType == 2) //discount amount low to high
                {
                    list = list.OrderBy(a => a.DiscAmount).ToList();
                }
                else if(sorterType == 3) // discount percentile hight to low
                {
                    list = list.OrderByDescending(a => a.DiscPercentage).ToList();
                }
                else if(sorterType == 4) // discount percentile low to high
                {
                    list = list.OrderBy(a => a.DiscPercentage).ToList();
                }
            }
            return PartialView(list);
        }
        public JsonResult ChangeOfferStatus(int id, int status)
        {
            var offer = db.Offers.Find(id);
            if (status == 1)
            {
                offer.Status = true; //active
            }
            else if (status == 0)
            {
                offer.Status = false; //Inactive
            }
            else if(status == 2)
            {
                offer.Status = null; // delete
            }
            db.Entry(offer).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Offer
        public ActionResult Offer()
        {
            ViewBag.ProductList = new SelectList(db.Products.Where(a => a.Status == true).Select(s => new { s.ProductId, s.ProductName }), "ProductId", "ProductName");
            ViewBag.OfferList = new SelectList(db.Offers.Where(a => a.Status == true && a.Type == 2).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
            ViewBag.EventList = new SelectList(db.Events.Where(a => a.Status == true).Select(s => new { s.Id, s.EventName }), "Id", "EventName");
            return View();
        }
        public ActionResult OfferCreate()
        {
            return View();
        }
        #region Sale offer
        [EncryptedActionParameter]
        public ActionResult SaleOffer(bool? isCoupon)
        {
            //if isCoupon true => offer create with coupon code
            ViewBag.IsCoupon = false;
            if(isCoupon == true)
            {
                ViewBag.IsCoupon = true;
            }
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult SaleOfferCreate(int subOfferId, bool isCoupon)
        {
            ViewBag.SubOfferId = subOfferId;
            ViewBag.IsCoupon = isCoupon;
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            ViewBag.ScheduleList = new SelectList(db.Schedules.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            return View();
        }
        public JsonResult SaleOfferSave(OfferModelView model)
        {
            try
            {
                Offer offer = new Offer();
                if (model.IsDateValidity)
                {
                    offer.StartDate = (DateTime)model.StartDate;
                    offer.EndDate = (DateTime)model.EndDate;
                }
                else
                {
                    DateTime date;
                    date = now.AddDays((int)model.ValidityDays).Date;
                    date = date.Add(model.ValidityTime.Value.TimeOfDay);

                    offer.StartDate = DateTime.Now;
                    offer.EndDate = date;
                }
                if (model.IsSingleOffer == false) 
                {
                    offer.MasterOfferId = model.MasterOfferId;
                    offer.SubOfferId = model.SubOfferId;
                    offer.ScheduleId = model.ScheduleId;
                    offer.Type = model.type;
                    offer.OfferName = model.OfferName;
                    offer.IsEditable = model.IsEditable;
                    offer.IsCouponApplicable = model.IsCouponApplicable;
                    offer.Coupon = model.Coupon;
                    offer.Status = true;
                    offer.ActualPrice = model.ActualPrice;
                    offer.OfferPrice = model.OfferPrice;
                    offer.DiscAmount = model.DiscAmount;
                    offer.DiscPercentage = model.DiscPercentage;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    db.Offers.Add(offer);

                    //db.SaveChanges();
                }
                //offer item save
                foreach (OfferItem item in model.Items)
                {
                    item.OfferId = offer.Id;
                    item.StartDate = offer.StartDate;
                    item.EndDate = offer.EndDate;
                    item.ScheduleId = model.ScheduleId;
                    item.Status = true;
                    item.CreatedBy = model.CreatedBy;
                    item.CreatedDate = now.Date;
                    db.OfferItems.Add(item);

                    //db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Event offer
        public ActionResult EventOffer()
        {
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult Events(int subId)
        {
            ViewBag.SubId = subId;
            return View();
        }
        public PartialViewResult EventCreate(int subId, int? id, bool? isEventPage)
        {
            ViewBag.SubId = subId;
            ViewBag.Id = id;
            ViewBag.IsEventPage = false;
            if(isEventPage == true)
            {
                ViewBag.IsEventPage = true;
            }
            return PartialView();
        }
        public PartialViewResult EventCreatePartial(int? id)
        {
            ViewBag.ScheduleList = new SelectList(db.Schedules.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            if (id > 0)
            {
                var aEvent = db.Events.Find(id);
                EventModelView model = new EventModelView();
                model.Id = aEvent.Id;
                model.EventName = aEvent.EventName;
                model.ValidityDate = aEvent.Validity;
                model.ScheduleId = aEvent.ScheduleId;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult EventSave(EventModelView model)
        {
            try
            {
                Event aEvent;
                List<Offer> offers;
                List<OfferItem> offerItems;
                if (model.Id > 0)
                {
                    aEvent = db.Events.Find(model.Id);
                    aEvent.EventName = model.EventName;
                    if(model.ScheduleId > 0)
                    {
                        aEvent.ScheduleId = (int)model.ScheduleId;
                    }
                    else
                    {
                        aEvent.ScheduleId = 0;
                    }
                    if (model.IsDateValidity)
                    {
                        aEvent.Validity = (DateTime)model.ValidityDate;
                    }
                    else
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);
                        aEvent.Validity = date;
                    }
                    aEvent.UpdatedBy = model.CreatedBy;
                    aEvent.UpdatedDate = now.Date;
                    db.Entry(aEvent).State = EntityState.Modified;

                    db.SaveChanges();

                    //update offer
                    offers = db.Offers.Where(a => a.MasterOfferId == aEvent.Id).ToList();
                    if(offers.Any())
                    {
                        foreach(var offer in offers)
                        {
                            //offer.Validity = aEvent.Validity;
                            offer.ScheduleId = aEvent.ScheduleId;
                            offer.UpdatedBy = model.CreatedBy;
                            offer.UpdatedDate = now.Date;
                            db.Entry(offer).State = EntityState.Modified;
                            db.SaveChanges();

                            //update offer items
                            offerItems = db.OfferItems.Where(a => a.OfferId == offer.Id).ToList();
                            if(offerItems.Any())
                            {
                                foreach(var item in offerItems)
                                {
                                    //item.Validity = aEvent.Validity;
                                    item.ScheduleId = aEvent.ScheduleId;
                                    item.UpdatedBy = model.CreatedBy;
                                    item.UpdatedDate = now.Date;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else
                {
                    aEvent = new Event();
                    aEvent.SubId = model.SubOfferId;
                    if (model.ScheduleId > 0)
                    {
                        aEvent.ScheduleId = (int)model.ScheduleId;
                    }
                    else
                    {
                        aEvent.ScheduleId = 0;
                    }
                    aEvent.EventName = model.EventName;
                    if (model.IsDateValidity)
                    {
                        aEvent.Validity = (DateTime)model.ValidityDate;
                    }
                    else
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);
                        aEvent.Validity = date;
                    }
                    aEvent.Status = true;
                    aEvent.CreatedBy = model.CreatedBy;
                    aEvent.CreatedDate = now.Date;
                    db.Events.Add(aEvent);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult EventList(int? status, int? subId, int? productId, long? offerId, long? eventId, DateTime? date)
        {
            var list = new List<ViewEvent>();
            if(productId > 0)
            {
                var eventIds = db.OfferItems.Where(a => a.ProductId == productId && a.MasterOfferId > 0).GroupBy(a => a.MasterOfferId).Select(s => new { s.FirstOrDefault().MasterOfferId }).ToList();
                if(eventIds != null)
                {
                    foreach(var id in eventIds)
                    {
                        list.Add(db.ViewEvents.FirstOrDefault(a => a.Id == id.MasterOfferId));
                    }
                }
            }
            else if(offerId > 0)
            {
                var id = db.Offers.Find(offerId).MasterOfferId;
                if(id > 0)
                {
                    list.Add(db.ViewEvents.FirstOrDefault(a => a.Id == id));
                }
            }
            else if(eventId > 0)
            {
                list.Add(db.ViewEvents.FirstOrDefault(a => a.Id == eventId));
            }
            else if(date != null)
            {
                list = db.ViewEvents.Where(a => DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(a.Validity)).ToList();
            }
            else if(subId > 0)
            {
                if (status == 1) // active
                {
                    list = db.ViewEvents.Where(a => a.SubId == subId && a.Status == true).ToList();
                }
                else if (status == 0) // inactive
                {
                    list = db.ViewEvents.Where(a => a.SubId == subId && a.Status == false).ToList();
                }
                else if (status == 2) // delete
                {
                    list = db.ViewEvents.Where(a => a.SubId == subId && a.Status == null).ToList();
                }
                else
                {
                    list = db.ViewEvents.Where(a => a.SubId == subId && a.Status != null).ToList();
                }
            }
            else
            {
                list = db.ViewEvents.ToList();
            }
            return PartialView(list.OrderBy(a => a.EventName).ToList());
        }
        public JsonResult ChangeEventStatus(long id, int status)
        {
            var aEvent = db.Events.Find(id);
            if (status == 1)
            {
                aEvent.Status = true; //active
            }
            else if (status == 0)
            {
                aEvent.Status = false; //Inactive
            }
            else if (status == 2)
            {
                aEvent.Status = null; // delete
            }
            db.Entry(aEvent).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult EventDetails(long eventId)
        {
            return View(db.ViewEvents.FirstOrDefault(a => a.Id == eventId));
        }
        [EncryptedActionParameter]
        public ActionResult EventDiscountType(long masterId, int subId, int scheduleId, DateTime validity)
        {
            ViewBag.MasterId = masterId;
            ViewBag.SubId = subId;
            ViewBag.ScheduleId = scheduleId;
            ViewBag.Validity = validity;
            return PartialView();
        }
        [EncryptedActionParameter]
        public ActionResult EventOfferCreate(long masterId, int subId, int scheduleId, int evntDiscType, DateTime validity)
        {
            ViewBag.SubId = subId;
            ViewBag.masterId = masterId;
            ViewBag.EvntDiscType = evntDiscType;
            ViewBag.ScheduleId = scheduleId;
            ViewBag.Validity = validity;
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View();
        }
        public JsonResult EventOfferSave(OfferModelView model, IList<OfferItem> items, bool? isSingleOffer)
        {
            try
            {
                Offer offer = new Offer();
                if (isSingleOffer == false)
                {
                    offer.MasterOfferId = model.MasterOfferId;
                    offer.SubOfferId = model.SubOfferId;
                    offer.ScheduleId = model.ScheduleId;
                    offer.Type = model.type;
                    offer.OfferName = model.OfferName;
                    offer.IsEditable = model.IsEditable;
                    //offer.Validity =(DateTime) model.ValidityDate;
                    offer.Status = true;
                    offer.ActualPrice = model.ActualPrice;
                    offer.OfferPrice = model.OfferPrice;
                    offer.DiscAmount = model.DiscAmount;
                    offer.DiscPercentage = model.DiscPercentage;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    db.Offers.Add(offer);
                    db.SaveChanges();
                }
                //offer item save
                foreach (OfferItem item in items)
                {
                    item.OfferId = offer.Id;
                    item.Status = true;
                    item.CreatedBy = model.CreatedBy;
                    item.CreatedDate = now.Date;
                    db.OfferItems.Add(item);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult OfferProductList(int? productId, long? offerId, long? eventId, DateTime? date, int? sorterType, bool? isSingleOffer, bool? isCouponOffer)
        {
            var offerItems = new List<ViewOfferItem>();
            if(productId > 0)
            {
                if(isCouponOffer == true)
                {
                    offerItems = db.ViewOfferItems.Where(a => a.ProductId == productId && a.IsCouponApplicable == true).ToList();
                }
                else
                {
                    offerItems = db.ViewOfferItems.Where(a => a.ProductId == productId).ToList();
                }
            }
            else if(offerId > 0)
            {
                offerItems = db.ViewOfferItems.Where(a => a.OfferId == offerId).ToList();
            }
            else if(eventId > 0)
            {
                offerItems = db.ViewOfferItems.Where(a => a.MasterOfferId == eventId).ToList();
            }
            else if(date != null)
            {
                offerItems = db.ViewOfferItems.Where(a =>  DbFunctions.TruncateTime(date) <= DbFunctions.TruncateTime(a.StartDate)).ToList();
            }
            else if(isCouponOffer == true)
            {
                offerItems = db.ViewOfferItems.Where(a => a.IsCouponApplicable == true).ToList();
            }
            else
            {
                offerItems = db.ViewOfferItems.ToList();
            }

            ViewBag.IsSingleOffer = false;
            if (isSingleOffer == true) // single offer
            {
                ViewBag.IsSingleOffer = true;
                offerItems = offerItems.Where(a => a.SubOfferId == 4 && a.OfferId == 0 && a.IsCouponApplicable != true).ToList();
            }

            if(sorterType > 0)
            {
                if(sorterType == 1) // amount hight to low
                {
                    offerItems = offerItems.OrderByDescending(a => a.AmountOff).ToList();
                }
                else if(sorterType == 2) // amount low to high
                {
                    offerItems = offerItems.OrderBy(a => a.AmountOff).ToList();
                }
                else if(sorterType == 3) // percentile high to low
                {
                    offerItems = offerItems.OrderByDescending(a => a.PercentageOff).ToList();
                }
                else if(sorterType == 4) // percentile low to high
                {
                    offerItems = offerItems.OrderBy(a => a.PercentageOff).ToList();
                }
            }

            return PartialView(offerItems);
        }
        #endregion

        #region Product Points
        public ActionResult Points()
        {
            return View();
        }
        public PartialViewResult ListProduct()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return PartialView();
        }
        public PartialViewResult SelectedProduct(string ids)
        {
            ViewBag.Ids = ids;
            return PartialView();
        }
        public PartialViewResult SelectedProList(string ids)
        {
            var productList = new List<ViewProduct>();
            if(!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(','))
                {
                    long rowId = Convert.ToInt64(id);
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    if (product != null)
                    {
                        productList.Add(product);
                    }
                }
            }
            return PartialView(productList);
        }
        public JsonResult PointsSave(IList<PointsModelView> products, int CreatedBy )
        {
            try
            {
                Product product;
                if(products != null)
                {
                    foreach(var item in products)
                    {
                        product = db.Products.Find(item.Id);
                        if(product.Points != item.Points)
                        {
                            product.IsPointBased = true;
                            product.Points = item.Points;
                            product.UpdatedBy = CreatedBy;
                            product.DateUpdated = now.Date;
                            db.Entry(product).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Coupon
        public ActionResult Coupon()
        {
            return View();
        }
        public ActionResult CouponCreate()
        {
            return View();
        }
        public PartialViewResult ListProductInCoupon()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return PartialView();
        }
        public PartialViewResult SelectedProductInCupon(string ids)
        {
            ViewBag.Ids = ids;
            return PartialView();
        }
        public PartialViewResult SelectedProListInCoupon(string ids)
        {
            var productList = new List<Product>();
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(','))
                {
                    int productId = Convert.ToInt32(id);
                    var product = db.Products.Find(productId);
                    if (product != null)
                    {
                        productList.Add(product);
                    }
                }
            }
            return PartialView(productList);
        }
        public PartialViewResult CouponCreatePartial(long? offerId, long? eventId, long? offerItemId)
        {
            ViewBag.OfferId = offerId;
            ViewBag.EventId = eventId;
            ViewBag.OfferItemId = offerItemId;
            return PartialView();
        }
        public PartialViewResult _CouponCreatePartial(long? offerId, long? eventId, long? offerItemId)
        {
            OfferModelView modelView = new OfferModelView();
            if(offerId > 0)
            {
                var offer = db.Offers.Find(offerId);
                modelView.Id = offer.Id;
                modelView.IsCouponApplicable = (offer.IsCouponApplicable == true ? true : false);
                modelView.Coupon = offer.Coupon;
            }
            else if(eventId > 0)
            {
                var aEvent = db.Events.Find(eventId);
                modelView.MasterOfferId = aEvent.Id;
                modelView.IsCouponApplicable = (aEvent.IsCouponApplicable == true ? true : false);
                modelView.Coupon = aEvent.Coupon;
            }
            else if(offerItemId > 0) //single offer
            {
                var singleOffer = db.OfferItems.Find(offerItemId);
                modelView.OfferItemId = singleOffer.Id;
                modelView.IsCouponApplicable = (singleOffer.IsCouponApplicable == true ? true : false);
                modelView.Coupon = singleOffer.Coupon;
            }
            return PartialView(modelView);
        }
        public JsonResult CouponSave(OfferModelView model)
        {
            try
            {
                Offer offer;
                List<OfferItem> offerItems;
                List<Offer> offers;
                Event aEvent;
                if (model.MasterOfferId > 0) //event
                {
                    //event update
                    aEvent = db.Events.Find(model.MasterOfferId);
                    aEvent.IsCouponApplicable = model.IsCouponApplicable;
                    aEvent.Coupon = model.Coupon;
                    aEvent.UpdatedBy = model.CreatedBy;
                    aEvent.UpdatedDate = now.Date;
                    db.Entry(aEvent).State = EntityState.Modified;
                    db.SaveChanges();

                    //offer update
                    offers = db.Offers.Where(a => a.MasterOfferId == model.MasterOfferId && a.Status == true).ToList();
                    if(offers.Any())
                    {
                        foreach(var aOffer in offers)
                        {
                            aOffer.IsCouponApplicable = model.IsCouponApplicable;
                            aOffer.Coupon = model.Coupon;
                            aOffer.UpdatedBy = model.CreatedBy;
                            aOffer.UpdatedDate = now.Date;
                            db.Entry(aOffer).State = EntityState.Modified;
                            db.SaveChanges();

                            //offer item update
                            offerItems = db.OfferItems.Where(a => a.OfferId == aOffer.Id).ToList();
                            if (offerItems.Any())
                            {
                                foreach (var item in offerItems)
                                {
                                    item.IsCouponApplicable = model.IsCouponApplicable;
                                    item.Coupon = model.Coupon;
                                    item.UpdatedBy = model.CreatedBy;
                                    item.UpdatedDate = now.Date;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else if(model.Id > 0) // offer
                {
                    offer = db.Offers.Find(model.Id);
                    offer.IsCouponApplicable = model.IsCouponApplicable;
                    offer.Coupon = model.Coupon;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                    db.SaveChanges();

                    offerItems = db.OfferItems.Where(a => a.OfferId == offer.Id).ToList();
                    if (offerItems.Any())
                    {
                        foreach (var item in offerItems)
                        {
                            item.IsCouponApplicable = model.IsCouponApplicable;
                            item.Coupon = model.Coupon;
                            item.UpdatedBy = model.CreatedBy;
                            item.UpdatedDate = now.Date;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else if(model.OfferItemId > 0) // single offer
                {
                    var singleOffer = db.OfferItems.Find(model.OfferItemId);
                    singleOffer.IsCouponApplicable = model.IsCouponApplicable;
                    singleOffer.Coupon = model.Coupon;
                    singleOffer.UpdatedBy = model.CreatedBy;
                    singleOffer.UpdatedDate = now.Date;
                    db.Entry(singleOffer).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //#region add Cash & Percentitle coupon
        //[EncryptedActionParameter]
        //public ActionResult AddCoupon(int type)
        //{
        //    ViewBag.Type = type;
        //    return View();
        //}
        //public PartialViewResult AddCouponPartial(int type)
        //{
        //    ViewBag.Type = type;
        //    return PartialView();
        //}
        //public PartialViewResult _AddCouponPartial(int type)
        //{
        //    ViewBag.Type = type;
        //    return PartialView();
        //}
        //public PartialViewResult CouponList(int type)
        //{
        //    ViewBag.Type = type;
        //    return PartialView(db.Coupons.Where(a => a.Type == type).ToList());
        //}
        //public JsonResult CustomCouponSave(CouponModelView model)
        //{
        //    try
        //    {
        //        Coupon coupon;
        //        if (model.Id > 0)
        //        {
        //            coupon = db.Coupons.Find(model.Id);
        //            coupon.Code = model.Code;
        //            coupon.Amount = model.Amount;
        //            coupon.Percentile = model.Percentile;
        //            coupon.MinimumPurchase = model.MinimumPurchase;
        //            coupon.MaximumAmount = model.MaximumAmount;
        //            coupon.Type = model.Type;
        //            coupon.ValidityDate = (DateTime)model.ValidityDate;
        //            coupon.UpdatedBy = model.CreatedBy;
        //            coupon.UpdatedDate = now.Date;
        //            db.Entry(coupon).State = EntityState.Modified;
        //        }
        //        else
        //        {
        //            coupon = new Coupon();
        //            coupon.Code = model.Code;
        //            coupon.Amount = model.Amount;
        //            coupon.Percentile = model.Percentile;
        //            coupon.MinimumPurchase = model.MinimumPurchase;
        //            coupon.MaximumAmount = model.MaximumAmount;
        //            coupon.Type = model.Type;
        //            coupon.Status = true;
        //            coupon.ValidityDate = (DateTime)model.ValidityDate;
        //            coupon.CreatedBy = model.CreatedBy;
        //            coupon.CreatedDate = now.Date;
        //            db.Coupons.Add(coupon);
        //        }
        //        //db.SaveChanges();
        //    }
        //    catch(Exception)
        //    {

        //    }
        //    return Json("success", JsonRequestBehavior.AllowGet);
        //}
        //#endregion

        #endregion

        #endregion

        #region Offer Items
        [EncryptedActionParameter]
        public ActionResult OfferItem(int id)
        {
            return View(db.ViewOffers.FirstOrDefault(a => a.Id == id));
        }
        //[EncryptedActionParameter]
        //public ActionResult AddOfferItem(int id)
        //{
        //    return View(db.ViewOffers.FirstOrDefault(a => a.Id == id));
        //}
        public PartialViewResult OfferItemList(int offerId)
        {
            return PartialView(db.ViewOfferItems.Where(a => a.OfferId == offerId && a.Status == true).ToList());
        }
        public JsonResult OfferItemSave(Offer offer, IList<OfferItem> items, long[] deleteItemIds)
        {
            try
            {
                OfferItem offerItem;
                Offer aOffer;

                //offer update
                aOffer = db.Offers.Find(offer.Id);
                aOffer.ActualPrice = offer.ActualPrice;
                aOffer.OfferPrice = offer.OfferPrice;
                aOffer.DiscPercentage = offer.DiscPercentage;
                aOffer.DiscAmount = offer.DiscAmount;
                aOffer.UpdatedBy = offer.UpdatedBy;
                aOffer.UpdatedDate = now.Date;
                db.Entry(aOffer).State = EntityState.Modified;
                db.SaveChanges();

                //offer item save
                foreach (OfferItem item in items)
                {
                    if (item.Id > 0)
                    {
                        offerItem = db.OfferItems.Find(item.Id);
                        offerItem.Quantity = item.Quantity;
                        offerItem.IsFree = item.IsFree;
                        offerItem.PercentageOff = item.PercentageOff;
                        offerItem.AmountOff = item.AmountOff;
                        offerItem.Price = item.Price;
                        offerItem.UpdatedBy = item.CreatedBy;
                        offerItem.UpdatedDate = now.Date;
                        db.Entry(offerItem).State = EntityState.Modified;
                    }
                    else
                    {
                        item.Status = true;
                        item.CreatedDate = now.Date;
                        db.OfferItems.Add(item);
                    }
                   db.SaveChanges();
                }

                // offer item delete
                if (deleteItemIds != null)
                {
                    foreach (var id in deleteItemIds)
                    {
                        offerItem = db.OfferItems.Find(id);
                        offerItem.Status = null;
                        db.Entry(offerItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Product Info
        //public JsonResult GetProductInfoById(int? productId, int? categoryId, int? subCategoryId, int? tagId, int? fromPrice, int? toPrice)
        //{
        //    if(productId > 0)
        //    {
        //        var product = db.ViewProducts.Where(a => a.ProductId == productId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true)
        //                      .GroupBy(a => a.ProductId)
        //                      .Select(s => new { s.FirstOrDefault().ProductName,  s.FirstOrDefault().CategoryName, Categorys = s.GroupBy(g => g.CategoryId).Select(sc => sc.FirstOrDefault().CategoryName), s.FirstOrDefault().Price, s.FirstOrDefault().Points });
        //        if (product != null)
        //        {
        //            return Json(product, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else if(categoryId > 0)
        //    {
        //        var productList = new List<ViewProduct>();
        //        if(subCategoryId > 0)
        //        {
        //            productList = db.ViewProducts.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true && a.Status == true).ToList();
        //        }
        //        else
        //        {
        //            productList = db.ViewProducts.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true && a.Status == true).ToList();
        //        }
        //        if((fromPrice >= 0) && (toPrice > 0) && (fromPrice < toPrice))
        //        {
        //            productList = productList.Where(a => a.Price >= fromPrice && a.Price <= toPrice).ToList();
        //        }
        //        return Json(productList.OrderBy(a => a.Price).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName, s.FirstOrDefault().Price, s.FirstOrDefault().Points }).ToList(), JsonRequestBehavior.AllowGet);
        //    }
        //    else if(tagId > 0)
        //    {
        //        var productList = db.ViewTagItems.Where(a => a.TagId == tagId && a.Type == 1 && a.Status == true && a.ProductStatus == true).Select(s => new { s.ProductName, ProductId = s.OwnerId, s.Price, Points = s.ProductPoints }).ToList();
        //        if (productList.Any())
        //        {
        //            if ((fromPrice >= 0) && (toPrice > 0) && (fromPrice < toPrice))
        //            {
        //                productList = productList.Where(a => a.Price >= fromPrice && a.Price <= toPrice).ToList();
        //            }
        //            return Json(productList.OrderBy(a => a.Price), JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json("error", JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Get offer
        public PartialViewResult GetOffer(IList<OrderItemModel> model)
        {
            List<Offer> offerList = new List<Offer>();
            if (model != null)
            {
                List<OfferItem> items = new List<OfferItem>();
                foreach (var id in model)
                {
                    var offerItems = db.OfferItems.Where(a => a.ProductId == id.ProductId && a.Status == true).ToList();
                    items.AddRange(offerItems);
                }
                var offerId = items.GroupBy(g => new { g.OfferId }).Select(s => new { s.FirstOrDefault().OfferId }).ToList();

                foreach(var id in offerId)
                {
                    if(db.Offers.Any(a => a.Id == id.OfferId && a.Status == true))
                    {
                        var isExist = true;
                        var offerItems = db.OfferItems.Where(a => a.OfferId == id.OfferId && a.Status == true).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, Quantity = s.Sum(a => a.Quantity) }).ToList();
                        foreach (var product in offerItems)
                        {
                            if (model.Any(a => a.ProductId == product.ProductId && a.Quantity >= product.Quantity) == false)
                            {
                                isExist = false;
                            }
                        }
                        //if (isExist == true)
                        //{
                        //    var offer = db.Offers.Find(id.OfferId);
                        //    offerList.Add(offer);
                        //}

                        var offer = db.Offers.Find(id.OfferId);
                        offerList.Add(offer);
                    }
                }
            }
            return PartialView(offerList);
        }
        public PartialViewResult GetOfferPartial(List<OfferItem> model)
        {
            return PartialView();
        }
        #endregion

        #region Offer Dashboard
        public PartialViewResult OfferItemTab()
        {
            ViewBag.ProductList = new SelectList(db.ViewOfferItems.Where(a => a.Status == true).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName }), "ProductId", "ProductName");
            ViewBag.OfferList = new SelectList(db.Offers.Where(a => a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
            ViewBag.EventList = new SelectList(db.Events.Where(a => a.Status == true).Select(s => new { s.Id, s.EventName }), "Id", "EventName");
            return PartialView();
        }
        public PartialViewResult SingleOfferItemTab()
        {
            ViewBag.ProductList = new SelectList(db.ViewOfferItems.Where(a => a.Status == true && a.SubOfferId == 4 && a.OfferId == 0).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName }), "ProductId", "ProductName");
            return PartialView();
        }
        public PartialViewResult OfferTab()
        {
            ViewBag.ProductList = new SelectList(db.ViewOfferItems.Where(a => a.Status == true && a.Type == 2).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName }), "ProductId", "ProductName");
            ViewBag.OfferList = new SelectList(db.Offers.Where(a => a.Status == true && a.Type == 2).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
            return PartialView();
        }
        public PartialViewResult EventTab()
        {
            ViewBag.ProductList = new SelectList(db.ViewOfferItems.Where(a => a.Status == true && a.MasterOfferId > 0).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName }), "ProductId", "ProductName");
            ViewBag.OfferList = new SelectList(db.Offers.Where(a => a.Status == true && a.Type == 1).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
            ViewBag.EventList = new SelectList(db.Events.Where(a => a.Status == true).Select(s => new { s.Id, s.EventName }), "Id", "EventName");
            return PartialView();
        }
        public PartialViewResult CouponTab()
        {
            ViewBag.ProductList = new SelectList(db.ViewOfferItems.Where(a => a.Status == true && a.IsCouponApplicable == true).GroupBy(a => a.ProductId).Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().ProductName }), "ProductId", "ProductName");
            return PartialView();
        }
        #endregion

        #region Credit payment from pos page
        public PartialViewResult CreditPayment(long customerId)
        {
            ViewBag.CustomerId = customerId;
            return PartialView();
        }
        public PartialViewResult CreditPaymentPartial(long customerId)
        {
            decimal totalDue = 0;
            if(db.ViewCreditCustomers.Any(a => a.CustomerId == customerId && a.DueAmount > 0))
            {
                totalDue = db.ViewCreditCustomers.Where(a => a.CustomerId == customerId && a.DueAmount > 0).Sum(s => s.DueAmount).Value;
            }
            var customer = db.ViewCustomers.FirstOrDefault(a => a.CustomerId == customerId);
            CustomerModelView model = new CustomerModelView();
            model.CustomerId = customerId;
            model.CreditLimit = (decimal)customer.CreditLimit;
            model.CreditAmount = totalDue;

            var debitAccount = db.DebitLimits.FirstOrDefault(a => a.CustomerId == customerId);
            if(debitAccount != null)
            {
                model.IsDebitAccounts = true;
                model.DebitLimit = debitAccount.Limit;
                model.AvailableDebit = debitAccount.Amount;
            }
            return PartialView(model);
        }
        #endregion

        #region customer Debit account refill 
        public PartialViewResult DebitRefill(long customerId)
        {
            ViewBag.CustomerId = customerId;
            return PartialView();
        }
        public PartialViewResult DebitRefillPartial(long customerId)
        {
            CustomerModelView model = new CustomerModelView();
            var debitAccount = db.DebitLimits.FirstOrDefault(a => a.CustomerId == customerId);
            if(debitAccount != null)
            {
                model.IsDebitAccounts = true;
                model.CustomerId = debitAccount.CustomerId;
                model.DebitLimit = debitAccount.Limit;
                model.AvailableDebit = debitAccount.Amount;
            }
            else
            {
                model.IsDebitAccounts = false;
            }
            return PartialView(model);
        }
        public PartialViewResult DebitRefillList(long customerId, int? days, DateTime? from, DateTime? to, int? count)
        {
            var list = new List<ViewOrderTransaction>();
            DateTime? start = from;
            DateTime? end = to;
            if (count > 0)
            {
                list = db.ViewOrderTransactions.Where(a => a.CustomerId == customerId && a.TransactionType == 3).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 3 && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 3 && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.OrderDate).ToList());
        }
        public ActionResult DebitRefillPrint(string q)
        {
            long customerId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                customerId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View(db.Customers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        public PartialViewResult DebitLedgerList(long customerId, int? days, DateTime? from, DateTime? to, int? count)
        {
            List<Ledger> ledgerList = new List<Ledger>();
            Ledger ledger;
            var debitList = new List<ViewPayment>();
            var creditList = new List<ViewOrderTransaction>();

            DateTime? start = from;
            DateTime? end = to;
            if (count > 0)
            {
                creditList = db.ViewOrderTransactions.Where(a => a.CustomerId == customerId && a.TransactionType == 3).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                debitList = db.ViewPayments.Where(a => a.CustomerId == customerId && a.PaymentBodyId == 17).OrderByDescending(a => a.Date).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                creditList = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 3 && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                debitList = db.ViewPayments.Where(m => m.CustomerId == customerId && m.PaymentBodyId == 17 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                creditList = db.ViewOrderTransactions.Where(m => m.CustomerId == customerId && m.TransactionType == 3 && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end)).ToList();
                debitList = db.ViewPayments.Where(m => m.CustomerId == customerId && m.PaymentBodyId == 17 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            if (creditList.Any())
            {
                foreach (var credit in creditList)
                {
                    ledger = new Ledger();
                    ledger.OrderId = credit.OrderId;
                    ledger.OrderNumber = credit.OrderNumber;
                    ledger.IsDebit = false;
                    ledger.Amount = credit.Price;
                    ledger.Date = (DateTime)credit.OrderDate;
                    ledgerList.Add(ledger);
                }
            }
            if(debitList.Any())
            {
                foreach(var debit in debitList)
                {
                    ledger = new Ledger();
                    ledger.OrderId = debit.PaymentId;
                    ledger.OrderNumber = debit.OrderNumber;
                    ledger.IsDebit = true;
                    ledger.Amount = debit.Amount;
                    ledger.Date = debit.Date;
                    ledgerList.Add(ledger);
                }
            }
            if(count > 0)
            {
                ledgerList = ledgerList.OrderByDescending(a => a.Date).Take((int)count).ToList();
            }
            return PartialView(ledgerList.OrderByDescending(a => a.Date).ToList());
        }
        public ActionResult DebitLedgerPrint(string q)
        {
            long customerId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                customerId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View(db.Customers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        #endregion

        #region set associate to product
        public PartialViewResult SetAssociate(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult SetAssociatePartial()
        {
            ViewBag.AssociateList = new SelectList(db.Suppliers.Where(a => a.Status == true && a.Type == false).Select(s => new { s.SupplierId, s.Name }), "SupplierId", "Name");
            return PartialView();
        }
        #endregion

        #region Product serial number
        public PartialViewResult SerialNumber(int productId)
        {
            return PartialView(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId));
        }
        public PartialViewResult SerialNumberPartial()
        {
            return PartialView();
        }
        #endregion

        #region Order refund save
        public JsonResult OrderRefundSave(OrderModel model)
       {
            PosOrderTransaction aTransaction;
            Stock aStock;
            PaymentBody account;
            bool IsDebitPay = false;
            decimal debitPay = 0;
            decimal amountPaid = 0;
            decimal quantity = 0;
            decimal prevQuantity = 0;
            decimal discount = 0;
            decimal itemTotalPrice = 0;
            try
            {
                var refundOrder = db.PosOrders.Find(model.OrderId);
                PosOrder aOrder = new PosOrder();
                if (model.OrderProducts != null)
                {
                    //order save
                    aOrder.RefundOrderId = model.OrderId;
                    aOrder.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmssf");
                    aOrder.CustomerId = refundOrder.CustomerId;
                    aOrder.OrderDate = DateTime.Now;
                    aOrder.CreatedBy = (int)model.SalesmanId;
                    aOrder.InvoiceAmount = model.InvoiceAmount;
                    aOrder.Discount = model.Discount;
                    aOrder.Discount = model.DiscountPercent;
                    if (model.DiscType == 1)
                    {
                        aOrder.DiscountType = true;
                    }
                    else
                    {
                        aOrder.DiscountType = false;
                    }
                    aOrder.Tax = model.Tax;
                    aOrder.TaxPercent = model.TaxPercent;
                    if (model.TaxFunc == 1)
                    {
                        aOrder.TaxFunc = true; // true for tax on actual amount
                    }
                    else
                    {
                        aOrder.TaxFunc = false; // false for tax on discounted amount
                    }
                    aOrder.SubTotalPrice = model.SubTotalPrice;
                    aOrder.Status = true;
                    aOrder.IsServiceItem = model.IsServiceItems;
                    aOrder.IsPointBasedItem = model.IsPointItems;
                    aOrder.TotalPoints = model.TotalPoints;
                    aOrder.OrderType = refundOrder.OrderType;
                    aOrder.PurchaseOrderId = refundOrder.PurchaseOrderId;
                    db.PosOrders.Add(aOrder);
                    db.SaveChanges();

                    refundOrder.NewOrderId = aOrder.OrderId;

                    if (model.Payments != null)
                    {
                        amountPaid = model.Payments.Sum(s => s.AmountPaid);
                    }
                    else
                    {
                        amountPaid = 0;
                    }

                    //order payment save
                    OrderPayment payment = new OrderPayment();
                    payment.PreviousId = refundOrder.PaymentId;
                    payment.ReferenceOrderId = refundOrder.OrderId;
                    payment.OrderId = aOrder.OrderId;
                    if (model.RefundAmount > 0)
                    {
                        payment.Amount = model.RefundAmount;
                        payment.Status = false; //credit payment
                    }
                    else if (model.PayAmount > 0)
                    {
                        payment.Amount = model.PayAmount;
                        payment.Status = true; //debit payment
                    }
                    payment.AmountPaid = amountPaid;
                    if (model.CreditAmount > 0)
                    {
                        payment.AmountPaid = amountPaid - (decimal)model.CreditAmount;
                        payment.DueAmount = model.CreditAmount;
                        payment.IsDuePayment = true;
                    }
                    else
                    {
                        payment.DueAmount = 0;
                        payment.IsDuePayment = false;
                    }
                    payment.ReturnAmount = model.ReturnAmount;
                    payment.Date = DateTime.Now;
                    payment.CreatedBy = (int)model.SalesmanId;
                    db.OrderPayments.Add(payment);

                    db.SaveChanges();
                    //order paymentid save
                    aOrder.PaymentId = payment.OrderPaymentId;
                    db.Entry(aOrder).State = EntityState.Modified;

                    db.SaveChanges();

                    //order transaction save
                    foreach (var aProduct in model.OrderProducts)
                    {
                        // all transaction of order save
                        aTransaction = new PosOrderTransaction();
                        aTransaction.OrderId = aOrder.OrderId;
                        aTransaction.ProductId = aProduct.ProductId;
                        aTransaction.DistributeId = aProduct.DistributeId;
                        aTransaction.ProductName = aProduct.ProductName;
                        aTransaction.TransactionType = aProduct.TransactionType;
                        aTransaction.Price = aProduct.Price;
                        aTransaction.IsRefundAllow = aProduct.IsRefundAllow;
                        aTransaction.Quantity = aProduct.Quantity;
                        aTransaction.OrderedQuantity = aProduct.Quantity;
                        aTransaction.PerItemPrice = aProduct.PerItemPrice;
                        aTransaction.Discount = aProduct.Discount;
                        if (aProduct.DiscType == 1)
                        {
                            aTransaction.DiscountType = true;
                        }
                        else
                        {
                            aTransaction.DiscountType = false;
                        }
                        aTransaction.IsBorrow = aProduct.IsBorrow;
                        aTransaction.AssociateId = aProduct.AssociateId;
                        aTransaction.IsAfterSaleService = aProduct.IsAfterSaleService;
                        aTransaction.ServiceDays = aProduct.ServiceDays;
                        aTransaction.ServiceName = aProduct.ServiceName;
                        aTransaction.ServiceTypeId = aProduct.ServiceTypeId;
                        aTransaction.IsPointBased = aProduct.IsPointBased;
                        aTransaction.Points = aProduct.Points;
                        aTransaction.CustomerId = model.CustomerId;
                        aTransaction.IsUniqueItem = aProduct.IsUniqueItem;
                        if (aTransaction.IsUniqueItem == true)
                        {
                            aTransaction.Quantity = 1;
                            aTransaction.OrderedQuantity = 1;
                        }
                        aTransaction.SerialNumber = aProduct.SerialNumber;
                        aTransaction.Status = true;
                        db.PosOrderTransactions.Add(aTransaction);
                        //reduce or add product from stock
                        if (aProduct.DistributeId > 0)
                        {
                            aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aProduct.ProductId && a.DistributeId == aProduct.DistributeId);
                        }
                        else
                        {
                            aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aProduct.ProductId);
                        }
                        //if this order product available
                        if (aStock != null)
                        {
                            prevQuantity = 0;
                            if (db.PosOrderTransactions.Any(a => a.Status == true && a.OrderId == model.OrderId && a.ProductId == aProduct.ProductId))
                            {
                                prevQuantity = db.PosOrderTransactions.FirstOrDefault(a => a.OrderId == model.OrderId && a.ProductId == aProduct.ProductId).Quantity;
                                if (prevQuantity < aProduct.Quantity)
                                {
                                    quantity = aProduct.Quantity - prevQuantity;
                                    aStock.Quantity = aStock.Quantity - quantity;
                                }
                                else if (prevQuantity > aProduct.Quantity)
                                {
                                    quantity = prevQuantity - aProduct.Quantity;
                                    aStock.Quantity = aStock.Quantity + quantity;
                                }
                            }
                            else
                            {
                                aStock.Quantity = aStock.Quantity - aProduct.Quantity;
                            }
                            db.Entry(aStock).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    //full refund
                    var orderTransactions = db.PosOrderTransactions.Where(a => a.OrderId == model.OrderId && a.Status == true).ToList();
                    if(orderTransactions != null)
                    {
                        foreach (var item in orderTransactions)
                        {
                            //reduce or add product from stock
                            if (item.DistributeId > 0)
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId);
                            }
                            else
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == item.ProductId);
                            }
                            //if this order product available
                            if (aStock != null)
                            {
                                aStock.Quantity = aStock.Quantity + item.Quantity;
                                db.Entry(aStock).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                }

                //order and ordertransaction inactive
                refundOrder.Status = false;
                refundOrder.RefundBy = model.SalesmanId;
                refundOrder.RefundDateTime = DateTime.Now;
                db.Entry(refundOrder).State = EntityState.Modified;
                db.SaveChanges();

                var orderTransaction = db.PosOrderTransactions.Where(a => a.OrderId == model.OrderId && a.Status == true).ToList();
                if(orderTransaction.Any())
                {
                    foreach(var transaction in orderTransaction)
                    {
                        transaction.Status = false;
                        transaction.RefundBy = model.SalesmanId;
                        transaction.RefundDateTime = DateTime.Now;
                        db.Entry(transaction).State = EntityState.Modified;

                        StockDailyReturn returnItem;
                        if (model.OrderProducts.Any(a => a.ProductId == transaction.ProductId && a.DistributeId == transaction.DistributeId))
                        {
                            var product = model.OrderProducts.FirstOrDefault(a => a.ProductId == transaction.ProductId && a.DistributeId == transaction.DistributeId);
                            if(transaction.Quantity > product.Quantity)
                            {
                                returnItem = new StockDailyReturn();
                                returnItem.OrderId = transaction.OrderId;
                                returnItem.ProductId = transaction.ProductId;
                                returnItem.ProductName = transaction.ProductName;
                                returnItem.DistributeId = transaction.DistributeId;
                                returnItem.Quantity =transaction.Quantity - product.Quantity;
                                returnItem.DiscountType = transaction.DiscountType;
                                returnItem.PerItemPrice = transaction.PerItemPrice;
                                itemTotalPrice = returnItem.Quantity * returnItem.PerItemPrice;
                                if (transaction.Discount > 0)
                                {
                                    discount = 0;
                                    if (transaction.Discount > 0)
                                    {
                                        discount = (decimal)(itemTotalPrice * (transaction.Discount / 100));
                                        itemTotalPrice = itemTotalPrice - discount;
                                        returnItem.Discount = discount;
                                    }
                                }
                                returnItem.Price = itemTotalPrice;
                                returnItem.Date = now.Date;
                                db.StockDailyReturns.Add(returnItem);

                                //if purchase order transaction
                                if (transaction.POItemId > 0)
                                {
                                    var purchaseItem = db.PurchaseTransactions.Find(transaction.POItemId);
                                    if (purchaseItem != null)
                                    {
                                        purchaseItem.ReceiveQty = (int)(purchaseItem.ReceiveQty - returnItem.Quantity);
                                        purchaseItem.RemainingQty = (int)(purchaseItem.RemainingQty + returnItem.Quantity);
                                        db.Entry(purchaseItem).State = EntityState.Modified;
                                    }
                                }
                            }
                        }
                        else
                        {
                            returnItem = new StockDailyReturn();
                            returnItem.OrderId = transaction.OrderId;
                            returnItem.ProductId = transaction.ProductId;
                            returnItem.ProductName = transaction.ProductName;
                            returnItem.DistributeId = transaction.DistributeId;
                            returnItem.Quantity = transaction.Quantity;
                            returnItem.DiscountType = transaction.DiscountType;
                            returnItem.PerItemPrice = transaction.PerItemPrice;
                            itemTotalPrice = returnItem.Quantity * returnItem.PerItemPrice;
                            if (transaction.Discount > 0)
                            {
                                discount = 0;
                                if (transaction.Discount > 0)
                                {
                                    discount = (decimal)(itemTotalPrice * (transaction.Discount / 100));
                                    itemTotalPrice = itemTotalPrice - discount;
                                    returnItem.Discount = discount;
                                }
                            }
                            returnItem.Price = itemTotalPrice;
                            returnItem.Date = now.Date;
                            db.StockDailyReturns.Add(returnItem);

                            //if purchase order transaction
                            if (transaction.POItemId > 0)
                            {
                                var purchaseItem = db.PurchaseTransactions.Find(transaction.POItemId);
                                if (purchaseItem != null)
                                {
                                    purchaseItem.ReceiveQty = (int)(purchaseItem.ReceiveQty - returnItem.Quantity);
                                    purchaseItem.RemainingQty = (int)(purchaseItem.RemainingQty + returnItem.Quantity);
                                    db.Entry(purchaseItem).State = EntityState.Modified;
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
                if(model.RefundAmount > 0)
                {
                    //Increase purchase order refund amount and decrease dispatch amount if purchase order exist
                    if (refundOrder.PurchaseOrderId > 0)
                    {
                        var purchaseOrder = db.Purchases.Find(refundOrder.PurchaseOrderId);
                        if (purchaseOrder != null)
                        {
                            decimal poRefundPrice = 0;
                            poRefundPrice = model.RefundAmount;
                            if (purchaseOrder.DispatchAmount > 0)
                            {
                                purchaseOrder.DispatchAmount = purchaseOrder.DispatchAmount - poRefundPrice;
                            }
                            if (purchaseOrder.RefundAmount > 0)
                            {
                                purchaseOrder.RefundAmount = purchaseOrder.RefundAmount + poRefundPrice;
                            }
                            else
                            {
                                purchaseOrder.RefundAmount = poRefundPrice;
                            }

                            //purchase order inactive
                            if (purchaseOrder.Status == 4) // 4 complete 
                            {
                                purchaseOrder.Status = 3; // 3 approved
                            }
                            db.Entry(purchaseOrder).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                //save payment transaction
                if (model.Payments != null)
                {
                    PaymentTransaction aPaymentTransaction;
                    //Payment transaction save
                    foreach (var aPayment in model.Payments)
                    {
                        aPaymentTransaction = new PaymentTransaction();
                        aPaymentTransaction.CustomerId = refundOrder.CustomerId;
                        if (model.OrderProducts != null)
                        {
                            aPaymentTransaction.PaymentId = aOrder.OrderId;
                            aPaymentTransaction.RefOrderId = refundOrder.OrderId;
                        }
                        else
                        {
                            // if full refund 
                            aPaymentTransaction.PaymentId = refundOrder.OrderId;
                        }
                        aPaymentTransaction.PaymentId = aOrder.OrderId;
                        aPaymentTransaction.RefOrderId = refundOrder.OrderId;
                        aPaymentTransaction.Type = 1; // Type 1 for order payment
                        if(model.RefundAmount > 0)
                        {
                            aPaymentTransaction.InOut = false; //InOut false for release payment
                        }
                        else if(model.PayAmount > 0)
                        {
                            if (aPayment.PaymentTypeId == 7)
                            {
                                aPaymentTransaction.InOut = false; // InOut false for release payment
                            }
                            else
                            {
                                aPaymentTransaction.InOut = true; // InOut true for receive payment
                            }
                        }
                        aPaymentTransaction.MethodId = (int)model.MethodId;
                        aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                        aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                        aPaymentTransaction.Amount = aPayment.AmountPaid;
                        aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                        aPaymentTransaction.Date = DateTime.Now;
                        aPaymentTransaction.IsCreditPayment = false;
                        aPaymentTransaction.CreatedBy = (int)model.SalesmanId;

                        db.PaymentTransactions.Add(aPaymentTransaction);


                        if(model.RefundAmount > 0)
                        {
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance - aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;
                        }
                        else if(model.PayAmount > 0)
                        {
                            if (aPayment.PaymentTypeId == 8) //Debit payment transaction
                            {
                                //decrease customer debit amount 
                                DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                                customerDebit.Amount = customerDebit.Amount - aPayment.AmountPaid;
                                db.Entry(customerDebit).State = EntityState.Modified;
                            }
                            else if (aPayment.PaymentTypeId == 7) // credit payment transaction
                            {

                            }
                            
                            //add amount in account balance
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance + aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;
                            
                        }
                        db.SaveChanges();
                    }
                }
                //customer debit account refill 
                if (IsDebitPay)
                {
                    //debit account refill
                    DebitLimit customerDebit = db.DebitLimits.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                    customerDebit.Amount = customerDebit.Amount + debitPay;
                    db.Entry(customerDebit).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Supplier Debit 
        public PartialViewResult SupplierDebitRefillList(long supplierId)
        {
            return PartialView(db.SupplierDebitPayments.Where( a=> a.SupplierId == supplierId).ToList());
        }
        #endregion

        #region Delivery charge check
        public JsonResult CheckDeliveryCharge(decimal totalPrice)
        {
            decimal deliveryCharge = 0;
            decimal discountDeliveryCharge = 0;
            int discountDeliveryPercentage = 0;
            try
            {
                var dChargeInfo = new DeliveryCharge();
                if (db.DeliveryCharges.Any(a => a.Status == true && a.FromPrice <= totalPrice && totalPrice <= a.ToPrice))
                {
                    dChargeInfo = db.DeliveryCharges.FirstOrDefault(a => a.Status == true && a.FromPrice <= totalPrice && totalPrice <= a.ToPrice);
                }
                else
                {
                    dChargeInfo = db.DeliveryCharges.Where(a => a.Status == true ).OrderByDescending(a => a.ToPrice).FirstOrDefault(a => a.FromPrice <= totalPrice && a.ToPrice < totalPrice );
                }
                if (dChargeInfo.Id > 0)
                {
                    //IsPercentile true percentile
                    //IsPercentile false static/cash
                    if (dChargeInfo.IsPercentile == true)
                    {
                        deliveryCharge = totalPrice * ((decimal)dChargeInfo.Amount / 100);
                    }
                    else
                    {
                        deliveryCharge = dChargeInfo.Amount;
                    }

                    int delDiscId = 0;
                    //check discount delivery charge
                    if(db.ViewDeliveryChargeCoupons.Where( a=> a.IsCouponApplicable == false && a.Status == true).Any(a => a.FromPrice <= totalPrice && totalPrice <= a.ToPrice && a.StartDate <= DateTime.Now && DateTime.Now <= a.EndDate))
                    {
                        var discountDelivery = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.Status == true && a.FromPrice <= totalPrice && totalPrice <= a.ToPrice && a.StartDate <= DateTime.Now && DateTime.Now <= a.EndDate);
                        if(discountDelivery.Id > 0)
                        {
                            delDiscId = discountDelivery.Id;
                            discountDeliveryCharge = deliveryCharge * ((decimal)discountDelivery.Percentage / 100);
                            discountDeliveryPercentage = discountDelivery.Percentage;
                        }
                    }
                    return Json(new { deliveryCharge,  dChargeInfo.IsPercentile, dChargeInfo.Amount, discountDeliveryCharge, discountDeliveryPercentage, DeliveryId = dChargeInfo.Id, delDiscId }, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception)
            {
                return Json("error",JsonRequestBehavior.AllowGet);
            }
            return Json("notFound", JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult CheckAmountDiscount(decimal totalPrice)
        {
            decimal invoiceDiscount = 0;
            try
            {
                var discAmount = new ViewAmountCoupon();

                if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.Status == true && a.StartDate <= DateTime.Now && DateTime.Now <= a.EndDate && ((a.FromPrice <= totalPrice && totalPrice <= a.ToPrice) || (a.FromPrice <= totalPrice && a.IsInifinte == true))))
                {
                    discAmount = db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.Status == true && a.StartDate <= DateTime.Now && DateTime.Now <= a.EndDate && ((a.FromPrice <= totalPrice && totalPrice <= a.ToPrice) || (a.FromPrice <= totalPrice && a.IsInifinte == true)));
                    if (discAmount.Id > 0)
                    {
                        //IsPercentile true percentile
                        //IsPercentile false static/cash
                        if (discAmount.IsPercentile == true)
                        {
                            invoiceDiscount = totalPrice * ((decimal)discAmount.Amount / 100);
                        }
                        else
                        {
                            invoiceDiscount = discAmount.Amount;
                        }
                        return Json(new { invoiceDiscount, discAmount.IsPercentile, discAmount.Amount, discAmount.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("notFound",JsonRequestBehavior.AllowGet);
        }

        #region Invoice info Details
        public PartialViewResult InvoiceInfo(OrderModel model, long? orderId)
        {
            return PartialView(model);
        }
        public PartialViewResult InvoiceInfoByOrderId(long orderId)
        {
            var posOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            ViewBag.PosOrderTrans = db.PosOrderTransactions.Where(a => a.OrderId == orderId).ToList();
            return PartialView(posOrder);
        }
        #endregion
    }
}