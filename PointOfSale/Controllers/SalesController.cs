using PointOfSale.Helpers;
using PointOfSale.Models;
using PointOfSale.ModelViews;
using PointOfSale.ModelViews.POS;
using PointOfSale.ModelViews.Sales;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.Controllers
{
    [RoutePrefix("Sales")]
    public class SalesController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion

        #region Sales/Order
        public ActionResult Order()
        {
            ViewBag.CountNumber = db.PosOrders.Where(a => a.Status == true).Count();
            return View();
        }
        public PartialViewResult OrderList(string selectedId, int? count, int? days, DateTime? from, DateTime? to, long? customerId)
        {
            DateTime? start = from;
            DateTime? end = to;
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            if (!string.IsNullOrEmpty(selectedId))
            {
                list.Clear();
                foreach (var id in selectedId.Split(','))
                {
                    var orderId = Convert.ToInt64(id);
                    ViewPosOrder aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    if (aOrder != null)
                    {
                        list.Add(aOrder);
                    }
                }
                list = list.OrderByDescending(o => o.OrderDate).ToList();
            }
            if (customerId > 0)
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
            else
            {
                if (count > 0)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == true).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date))
                        .OrderByDescending(a => a.OrderDate)
                        .ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end))
                        .OrderByDescending(a => a.OrderDate)
                        .ToList();
                }
            }
            return PartialView(list);
        }
        public JsonResult GetOrderList(string text, int? days, DateTime? from, DateTime? to, bool? IsServiceList)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewPosOrder>();
            if (IsServiceList == true)
            {
                list = db.ViewPosOrders.Where(a => a.IsServiceItem == true && a.Status == true).ToList();
            }
            else if(!string.IsNullOrEmpty(text))
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date) && m.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }

                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end) && m.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if(days == 0 || days == null)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == true && a.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate).Take(5).ToList();
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if(days == 0 || days == null)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == true)
                        .OrderByDescending(o => o.OrderDate).Take(5).ToList();
                }
            }
            var orderList = new SelectList(list.Select(s => new { s.OrderId, s.OrderNumber }).ToList(), "OrderId", "OrderNumber");
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult OrdersPrint(int? days, DateTime? from, DateTime? to, string selectedId, bool? isPrintWithDetails, bool? isInactive)
        {
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.SelectedId = selectedId;
            ViewBag.IsPrintWithDetails = isPrintWithDetails;
            ViewBag.IsInactive = isInactive;
            return View();
        }
        public PartialViewResult OrderListForPrint(int? days, DateTime? from, DateTime? to, string selectedId, bool? isPrintWithDetails, bool? isInactive)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewPosOrder>();
            if(!string.IsNullOrEmpty(selectedId))
            {
                start = null;
                end = null;
                foreach(var id in selectedId.Split(','))
                {
                    long orderId = Convert.ToInt64(id);
                    var order = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    if(order != null)
                    {
                        list.Add(order);
                    }
                }
            }
            else
            {
                if(isInactive == true)
                {
                    if (days == 1)
                    {
                        start = null;
                        end = null;
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    else if (days > 1)
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
                    if (days == 1)
                    {
                        start = null;
                        end = null;
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPosOrders.Where(m => m.Status == true && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    else if (days > 1)
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
            }
            ViewBag.IsPrintWithDetails = false;
            if(isPrintWithDetails == true)
            {
                ViewBag.IsPrintWithDetails = true;
            }
            return PartialView(list.OrderByDescending(a => a.OrderDate).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult OrderDetails(long OrderId, long? currentOrderId, bool? isViewLastOrder)
        {
            if(isViewLastOrder == true)
            {
                var refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == OrderId);
                if (refundOrder != null)
                {
                    if (refundOrder.NewOrderId > 0)
                    {
                        OrderId = (long)refundOrder.NewOrderId;
                        refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == OrderId);
                        while (refundOrder.NewOrderId > 0)
                        {
                            OrderId = (long)refundOrder.NewOrderId;
                            refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == OrderId);
                        }
                    }
                }
            }
            ViewBag.OrderId = OrderId;
            ViewBag.CurrentOrderId = currentOrderId;
            return View();
        }
        public PartialViewResult OrderDetailsPartial(long orderId, long? currentOrderId)
        {

            ViewBag.CurrentVoucherName = null;
            if(currentOrderId > 0)
            {
                ViewBag.CurrentVoucherName = db.PosOrders.Find(currentOrderId).OrderNumber;
            }
            return PartialView(db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId));
        }
        public PartialViewResult OrderPaymentPartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult OrderArchivePartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult OrderArchiveList(long orderId)
        {
            ViewBag.CurrentId = orderId;
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            var refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            if (refundOrder != null)
            {
                list.Add(refundOrder);
                if (refundOrder.RefundOrderId > 0)
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
            return PartialView(list.OrderBy(a => a.OrderDate).ToList());
        }
        public PartialViewResult OrderRefundProductsPartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult OrderRefundProductList(long orderId)
        {
            List<StockDailyReturn> returnItems = new List<StockDailyReturn>();
            List<long> ids = new List<long>();

            var refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            if (refundOrder != null)
            {
                if (refundOrder.RefundOrderId > 0)
                {
                    orderId = (long)refundOrder.RefundOrderId;
                    ids.Add(orderId);
                    refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    while (refundOrder.RefundOrderId > 0)
                    {
                        orderId = (long)refundOrder.RefundOrderId;
                        ids.Add(orderId);
                        refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    }
                }
            }

            if(ids != null)
            {
                foreach(var id in ids)
                {
                    var returnItem = db.StockDailyReturns.Where(a => a.OrderId == id).ToList();
                    if(returnItem != null)
                    {
                        returnItems.AddRange(returnItem);
                    }
                }
            }
            return PartialView(returnItems);
        }

        public PartialViewResult OrderedProducts(long orderId, bool? isReturnPartial, string selectedId, bool? orderStatus)
        {
            ViewBag.IsReturnPartial = isReturnPartial;
            ViewBag.OrderStatus = true;
            if(orderStatus == false)
            {
                ViewBag.OrderStatus = false;
            }
            var list = new List<ViewOrderTransaction>();
            if(!string.IsNullOrEmpty(selectedId))
            {
                foreach(var id in selectedId.Split(','))
                {
                    long itemId = Convert.ToInt64(id);
                    var orderItem = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == itemId);
                    if(orderItem != null)
                    {
                        list.Add(orderItem);
                    }
                }
            }
            else
            {
                list = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult OrderTransactionPrint(long? orderId, int? type, bool? IsOriginal, DateTime? refundDate)
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
            else if (refundDate != null)
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
            ViewBag.Type = type;
            return View(aOrder);
        }
        public JsonResult GetOrderTransactionList(long? orderId, string serialNumber, long? serviceOrderId, long? orderTransId)
        {
            var list = new List<PosOrderTransaction>();

            if (orderId > 0)
            {
                list = db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == false && a.Quantity > 0).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else if (serviceOrderId > 0)
            {
                if (db.PosOrderTransactions.Where(a => a.OrderId == serviceOrderId && a.IsAfterSaleService == true).Count() > 1)
                {
                    return Json(new { IsMultipleService = true }, JsonRequestBehavior.AllowGet);
                }
                else if (db.PosOrderTransactions.Where(a => a.OrderId == serviceOrderId && a.IsAfterSaleService == true).Count() == 1)
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
                    return Json(new { IsExist = IsExist, CustomerId = customerId, IsMultipleService = false, orderTransaction.SerialNumber, TransId = orderTransaction.OrderTransactionId, IsExpire = isExpire, orderTransaction.ProductName, orderTransaction.ProductId, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (orderTransId > 0)
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
                if (db.PosOrderTransactions.Any(a => a.SerialNumber == serialNumber && a.IsAfterSaleService == true))
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
                        return Json(new { saleService.TemporaryWorkOrderNo, saleService.WorkOrderNo, saleService.Status, ServiceOrderDate = saleService.CreatedDate, serviceCreatedBy = saleService.CreatedBy, saleService.DeliveryDate, IsExist = IsExist, orderTransaction.SerialNumber, orderTransaction.ProductName, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { IsExist = IsExist, CustomerId = customerId, orderTransaction.SerialNumber, TransId = orderTransaction.OrderTransactionId, IsExpire = isExpire, orderTransaction.ProductName, orderTransaction.ProductId, orderTransaction.OrderDate, ExpireDate = expireDate, orderTransaction.OrderNumber }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("notFound", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult PaymentHistoryPartial(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }
        public PartialViewResult PaymentHistoryList(long? customerId, int? accId, long? orderId, int? days, DateTime? from, DateTime? to, int? count, bool? isDebitTrans, long? poId)
        {
            List<ViewPayment> list = new List<ViewPayment>();
            if (accId > 0)
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
                    while (newList.Any(a => a.RefOrderId > 0))
                    {
                        refOrderId = (long)newList.FirstOrDefault().RefOrderId;
                        newList = db.ViewPayments.Where(a => (a.PaymentId == refOrderId || a.CreditPaymentId == refOrderId) && a.Type == 1).ToList();
                        list.AddRange(newList);
                    }
                }
            }
            else if (customerId > 0)
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
            else if(poId > 0)
            {
                var posOrderIds = db.PosOrders.Where(a => a.PurchaseOrderId == poId).Select(s => new { s.OrderId }).ToList();
                if(posOrderIds != null)
                {
                    foreach(var id in posOrderIds )
                    {
                        var paymentList = db.ViewPayments.Where(a => (a.PaymentId == id.OrderId || a.CreditPaymentId == id.OrderId) && a.Type == 1).ToList();
                        if(paymentList != null)
                        {
                            list.AddRange(paymentList);
                        }
                    }
                }
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        public PartialViewResult RefundInfo(long orderId)
        {
            ViewBag.OrderId = orderId;
            return PartialView();
        }

        public PartialViewResult RefundInfoList(long orderId)
        {
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            var refundOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
            if (refundOrder != null)
            {
                if (refundOrder.RefundOrderId > 0)
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
        public ActionResult OrderVoucherPrint(long orderId)
        {
            return View(db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId));
        }
        #endregion

        #region Return Item
        public PartialViewResult RefundAlert(long orderId)
        {
            bool isRefundableItemExist = false;
            if (db.PosOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == true && a.Quantity > 0).Any())
            {
                isRefundableItemExist = true;
            }
            ViewBag.OrderId = orderId;
            ViewBag.IsRefundableItemExist = isRefundableItemExist;
            var list = db.ViewOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == false && a.Quantity > 0).ToList();
            return PartialView(list);
        }
        public PartialViewResult RefundPay(long? orderId, IList<OrderItemModel>  RefundItems)
        {
            decimal subTotalPrice = 0;
            decimal totalPrice = 0;
            decimal discount = 0;
            decimal taxAmount = 0;
            decimal itemTotalPrice = 0;
            decimal quantity = 0;
            var posOrder = new ViewPosOrder();
            OrderModel model = new OrderModel();
            OrderItemModel itemModel;
            List<OrderItemModel> itemModelList = new List<OrderItemModel>();
            if (orderId > 0)
            {
                posOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                model.OrderId = posOrder.OrderId;
                model.InvoiceNo = posOrder.OrderNumber;
                model.CustomerId = posOrder.CustomerId;
                model.CustomerName = posOrder.CustomerName;
                model.InvoiceTotal = posOrder.InvoiceAmount;
                model.TaxPercent = posOrder.TaxPercent;
                model.TaxFunc = 0;
                if (posOrder.TaxFunc == true)
                {
                    model.TaxFunc = 1;
                }
                if (posOrder.DiscountType == true)
                {
                    model.DiscType = 1;
                }
                else
                {
                    model.DiscType = 0;
                }
                model.DiscountPercent = model.DiscountPercent;
                if(RefundItems != null)
                {
                    var orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId).ToList();
                    foreach (var item in orderTransactionList)
                    {
                        itemModel = new OrderItemModel();
                        itemModel.ProductId = item.ProductId;
                        itemModel.ProductName = item.ProductName;
                        itemModel.Price = item.Price;
                        itemModel.PerItemPrice = item.PerItemPrice;
                        itemModel.Discount = 0;
                        itemModel.DiscType = 0;
                        itemModel.DiscountPercent = 0;
                        itemModel.IsUniqueItem = false;
                        itemModel.TransactionType = item.TransactionType;
                        if(item.IsUniqueItem == true)
                        {
                            itemModel.IsUniqueItem = true;
                        }
                        itemModel.IsAfterSaleService = false;
                        itemModel.IsBorrow = false;
                        if(item.IsBorrow == true)
                        {
                            itemModel.IsBorrow = true;
                        }
                        if (item.IsAfterSaleService == true)
                        {
                            itemModel.IsAfterSaleService = true;
                        }
                        itemModel.SerialNumber = item.SerialNumber;
                        itemModel.ServiceDays = (int)item.ServiceDays;
                        itemModel.ServiceName = item.ServiceName;
                        if (item.DiscountType == true)
                        {
                            itemModel.DiscType = 1;
                        }
                        itemModel.Discount = (decimal)item.Discount;
                        itemModel.Quantity = item.Quantity;
                        if(RefundItems.Any(a => a.OrderTransactionId == item.OrderTransactionId))
                        {
                            quantity = RefundItems.FirstOrDefault(a => a.OrderTransactionId == item.OrderTransactionId).Quantity;
                            if (item.Quantity > quantity)
                            {
                                itemModel.Quantity = item.Quantity - quantity;
                                itemTotalPrice = itemModel.Quantity * item.PerItemPrice;
                                discount = 0;
                                if (item.Discount > 0)
                                {
                                    discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                                    itemTotalPrice = itemTotalPrice - discount;
                                    itemModel.Discount = discount;
                                }
                                itemModel.Price = itemTotalPrice;
                                subTotalPrice = subTotalPrice + itemModel.Price;
                                itemModelList.Add(itemModel);
                            }
                        }
                        else
                        {
                            subTotalPrice = subTotalPrice + itemModel.Price;
                            itemModelList.Add(itemModel);
                        }
                    }

                    model.OrderProducts = itemModelList;

                    totalPrice = subTotalPrice;
                    if (posOrder.TaxPercent > 0 && posOrder.TaxFunc == true)
                    {
                        taxAmount = totalPrice * ((decimal)posOrder.TaxPercent / 100);
                        totalPrice = totalPrice + taxAmount;
                    }
                    if (posOrder.Discount > 0)
                    {
                        discount = 0;
                        discount = totalPrice * ((decimal)posOrder.Discount / 100);
                        totalPrice = totalPrice - discount;
                        model.Discount = discount;
                    }
                    if (posOrder.TaxPercent > 0 && posOrder.TaxFunc == false)
                    {
                        taxAmount = totalPrice * ((decimal)posOrder.TaxPercent / 100);
                        totalPrice = totalPrice + taxAmount;
                    }
                    model.SubTotalPrice = subTotalPrice;
                    model.TotalPrice = totalPrice;
                    model.Tax = taxAmount;
                    model.RefundAmount = model.InvoiceTotal - model.TotalPrice;
                }
                else
                {
                    //if any item refund allow false
                    if (db.PosOrderTransactions.Where(a => a.OrderId == orderId).Any(a => a.IsRefundAllow == false))
                    {
                        // non refundable items 
                        var orderTransactionList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId && a.IsRefundAllow == false).ToList();
                        foreach (var item in orderTransactionList)
                        {
                            itemModel = new OrderItemModel();
                            itemModel.ProductId = item.ProductId;
                            itemModel.ProductName = item.ProductName;
                            itemModel.Price = item.Price;
                            itemModel.PerItemPrice = item.PerItemPrice;
                            itemModel.Discount = 0;
                            itemModel.DiscType = 0;
                            itemModel.DiscountPercent = 0;
                            itemModel.IsUniqueItem = item.IsUniqueItem;
                            itemModel.TransactionType = item.TransactionType;
                            if (item.DiscountType == true)
                            {
                                itemModel.DiscType = 1;
                            }
                            if (item.Discount > 0)
                            {
                                itemModel.Discount = (decimal)item.Discount;
                            }
                            if (item.Discount > 0)
                            {
                                itemModel.DiscountPercent = (decimal)item.Discount;
                            }
                            itemModel.Quantity = item.Quantity;
                            itemModelList.Add(itemModel);
                        }

                        model.OrderProducts = itemModelList;

                        subTotalPrice = orderTransactionList.Sum(a => a.Price);

                        totalPrice = subTotalPrice;
                        if (posOrder.TaxPercent > 0 && posOrder.TaxFunc == true)
                        {
                            taxAmount = totalPrice * ((decimal)posOrder.TaxPercent / 100);
                            totalPrice = totalPrice + taxAmount;
                        }
                        if (posOrder.Discount > 0)
                        {
                            discount = 0;
                            discount = totalPrice * ((decimal)posOrder.Discount / 100);
                            totalPrice = totalPrice - discount;
                            model.Discount = discount;
                        }
                        if (posOrder.TaxPercent > 0 && posOrder.TaxFunc == false)
                        {
                            taxAmount = totalPrice * ((decimal)posOrder.TaxPercent / 100);
                            totalPrice = totalPrice + taxAmount;
                        }
                        model.SubTotalPrice = subTotalPrice;
                        model.TotalPrice = totalPrice;
                        model.Tax = taxAmount;
                        model.RefundAmount = model.InvoiceTotal - model.TotalPrice;
                    }
                    else
                    {
                        model.Discount = (decimal)posOrder.Discount;
                        model.RefundAmount = posOrder.InvoiceAmount;
                        model.Tax = posOrder.Tax;
                        model.TotalPoints = 0;
                        if (posOrder.TotalPoints > 0)
                        {
                            model.TotalPoints = (int)posOrder.TotalPoints;
                        }
                    }
                }
            }
            //if (model.CustomerId > 0)
            //{
            //    decimal? prevDue = 0;
            //    decimal? CreditLimit = 0;
            //    model.AvailableCreditAmount = 0;
            //    model.IsCreditAllow = false;
            //    model.IsDebitAllow = false;
            //    model.DebitAmount = 0;
            //    var customer = db.ViewCustomers.FirstOrDefault(a => a.CustomerId == model.CustomerId);
            //    //customer credit
            //    if (customer.IsCreditAllowed)
            //    {
            //        model.IsCreditAllow = true;
            //        //calcualte customer previous due
            //        var customerCreditList = db.ViewCreditCustomers.Where(a => a.CustomerId == model.CustomerId && a.DueAmount > 0).ToList();
            //        if (customerCreditList.Any())
            //        {
            //            prevDue = customerCreditList.Sum(a => a.DueAmount);
            //        }
            //        //customer credit limit
            //        if (customer.CreditLimit > 0)
            //        {
            //            CreditLimit = customer.CreditLimit;
            //        }
            //        if (prevDue >= CreditLimit)
            //        {
            //            model.IsCreditAllow = false;
            //        }
            //        else
            //        {
            //            model.AvailableCreditAmount = CreditLimit - prevDue;
            //        }
            //    }
            //    //customer debit
            //    if (customer.DebitLimitId > 0)
            //    {
            //        if (customer.DebitAmount > 0)
            //        {
            //            model.IsDebitAllow = true;
            //            model.DebitAmount = customer.DebitAmount;
            //        }
            //    }
            //}
            return PartialView(model);
        }
        // single or full refund
        public JsonResult RefundSave(int createdBy, long orderId, IList<PaymentModel> Payments, IList<OrderItemModel> refundItem)
        {
            try
            {
                decimal subTotalPrice = 0;
                decimal totalPrice = 0;
                decimal discount = 0;
                decimal taxAmount = 0;
                decimal itemTotalPrice = 0;
                decimal quantity = 0;
                int totalPoints = 0;

                PosOrderTransaction aTransaction;
                Stock aStock;
                PaymentBody account;
                StockDailyReturn returnItem;
                PosOrder refundOrder = new PosOrder();
                refundOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == orderId);
                var ordertransaction = db.PosOrderTransactions.Where(a => a.OrderId == orderId).ToList();

                //order save
                PosOrder aOrder = new PosOrder();
                aOrder.RefundOrderId = refundOrder.OrderId;
                aOrder.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmssf");
                aOrder.OrderDate = DateTime.Now;
                aOrder.CreatedBy = createdBy;
                aOrder.Discount = refundOrder.Discount;
                aOrder.DiscountType = refundOrder.DiscountType;
                aOrder.TaxPercent = refundOrder.TaxPercent;
                aOrder.TaxFunc = refundOrder.TaxFunc;
                aOrder.Status = true;
                aOrder.CustomerId = refundOrder.CustomerId;
                aOrder.IsServiceItem = refundOrder.IsServiceItem;
                aOrder.IsPointBasedItem = refundOrder.IsPointBasedItem;
                aOrder.OrderType = refundOrder.OrderType;
                aOrder.PurchaseOrderId = refundOrder.PurchaseOrderId;
                db.PosOrders.Add(aOrder);

                db.SaveChanges();

                if (refundItem != null)
                {
                    foreach (var item in ordertransaction)
                    {
                        aTransaction = new PosOrderTransaction();
                        aTransaction.OrderId = aOrder.OrderId;
                        aTransaction.ProductId = item.ProductId;
                        aTransaction.DistributeId = item.DistributeId;
                        aTransaction.PurchaseOrderId = item.PurchaseOrderId;
                        aTransaction.POItemId = item.POItemId;
                        aTransaction.ProductName = item.ProductName;
                        aTransaction.TransactionType = item.TransactionType;
                        aTransaction.Price = item.Price;
                        aTransaction.IsRefundAllow = item.IsRefundAllow;
                        aTransaction.Quantity = item.Quantity;
                        aTransaction.OrderedQuantity = item.Quantity;
                        aTransaction.PerItemPrice = item.PerItemPrice;
                        aTransaction.Discount = item.Discount;
                        aTransaction.DiscountType = item.DiscountType;
                        aTransaction.IsBorrow = item.IsBorrow;
                        aTransaction.AssociateId = item.AssociateId;
                        aTransaction.IsAfterSaleService = item.IsAfterSaleService;
                        aTransaction.ServiceDays = item.ServiceDays;
                        aTransaction.ServiceName = item.ServiceName;
                        aTransaction.ServiceTypeId = item.ServiceTypeId;
                        aTransaction.IsPointBased = item.IsPointBased;
                        aTransaction.Points = item.Points;
                        aTransaction.CustomerId = item.CustomerId;
                        aTransaction.IsUniqueItem = item.IsUniqueItem;
                        if (aTransaction.IsUniqueItem == true)
                        {
                            aTransaction.Quantity = 1;
                            aTransaction.OrderedQuantity = 1;
                        }
                        aTransaction.SerialNumber = item.SerialNumber;
                        aTransaction.Status = true;

                        if (refundItem.Any(a => a.OrderTransactionId == item.OrderTransactionId))
                        {
                            quantity = refundItem.FirstOrDefault(a => a.OrderTransactionId == item.OrderTransactionId).Quantity;
                            if (item.Quantity > quantity)
                            {
                                
                                aTransaction.Quantity = item.Quantity - quantity;
                                itemTotalPrice = aTransaction.Quantity * item.PerItemPrice;
                                if (aTransaction.Points > 0)
                                {
                                    aTransaction.Points = aTransaction.Points * (int)aTransaction.Quantity;
                                    totalPoints = totalPoints + (int)aTransaction.Points;
                                }
                                discount = 0;
                                if (item.Discount > 0)
                                {
                                    discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                                    itemTotalPrice = itemTotalPrice - discount;
                                    aTransaction.Discount = discount;
                                }
                                aTransaction.Price = itemTotalPrice;
                                subTotalPrice = subTotalPrice + aTransaction.Price;

                                db.PosOrderTransactions.Add(aTransaction);
                            }
                            //Save daily stock return
                            returnItem = new StockDailyReturn();
                            returnItem.OrderId = item.OrderId;
                            returnItem.ProductId = aTransaction.ProductId;
                            returnItem.ProductName = aTransaction.ProductName;
                            returnItem.DistributeId = aTransaction.DistributeId;
                            returnItem.Quantity = quantity;
                            returnItem.DiscountType = aTransaction.DiscountType;
                            returnItem.PerItemPrice = aTransaction.PerItemPrice;
                            itemTotalPrice = returnItem.Quantity * returnItem.PerItemPrice;
                            if (aTransaction.Discount > 0)
                            {
                                discount = 0;
                                if (aTransaction.Discount > 0)
                                {
                                    discount = (decimal)(itemTotalPrice * (aTransaction.Discount / 100));
                                    itemTotalPrice = itemTotalPrice - discount;
                                    returnItem.Discount = discount;
                                }
                            }
                            returnItem.Price = itemTotalPrice;
                            returnItem.Date = now.Date;
                            db.StockDailyReturns.Add(returnItem);

                            //reduce or add product from stock
                            if (aTransaction.DistributeId > 0)
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aTransaction.ProductId && a.DistributeId == aTransaction.DistributeId);
                            }
                            else
                            {
                                aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aTransaction.ProductId);
                            }
                            //if this order product available
                            if (aStock != null)
                            {
                                aStock.Quantity = aStock.Quantity + quantity;
                                db.Entry(aStock).State = EntityState.Modified;
                            }

                            //if purchase order transaction
                            if (item.POItemId > 0)
                            {
                                var purchaseItem = db.PurchaseTransactions.Find(item.POItemId);
                                if (purchaseItem != null)
                                {
                                    purchaseItem.ReceiveQty = (int)(purchaseItem.ReceiveQty - item.Quantity);
                                    purchaseItem.RemainingQty = (int)(purchaseItem.RemainingQty + item.Quantity);
                                    db.Entry(purchaseItem).State = EntityState.Modified;
                                }
                            }
                        }
                        else
                        {
                            if(aTransaction.Points > 0)
                            {
                                totalPoints = totalPoints + (int)aTransaction.Points;
                            }
                            subTotalPrice = subTotalPrice + aTransaction.Price;
                            db.PosOrderTransactions.Add(aTransaction);
                        }

                        item.Status = false;
                        item.RefundBy = createdBy;
                        item.RefundDateTime = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                else
                {
                    foreach(var item in ordertransaction)
                    {
                        if(item.IsRefundAllow == false)
                        {
                            aTransaction = new PosOrderTransaction();
                            aTransaction.OrderId = aOrder.OrderId;
                            aTransaction.ProductId = item.ProductId;
                            aTransaction.DistributeId = item.DistributeId;
                            aTransaction.PurchaseOrderId = item.PurchaseOrderId;
                            aTransaction.POItemId = item.POItemId;
                            aTransaction.ProductName = item.ProductName;
                            aTransaction.TransactionType = item.TransactionType;
                            aTransaction.Price = item.Price;
                            aTransaction.IsRefundAllow = item.IsRefundAllow;
                            aTransaction.Quantity = item.Quantity;
                            aTransaction.OrderedQuantity = item.Quantity;
                            aTransaction.PerItemPrice = item.PerItemPrice;
                            aTransaction.Discount = item.Discount;
                            aTransaction.DiscountType = item.DiscountType;
                            aTransaction.IsBorrow = item.IsBorrow;
                            aTransaction.AssociateId = item.AssociateId;
                            aTransaction.IsAfterSaleService = item.IsAfterSaleService;
                            aTransaction.ServiceDays = item.ServiceDays;
                            aTransaction.ServiceName = item.ServiceName;
                            aTransaction.ServiceTypeId = item.ServiceTypeId;
                            aTransaction.IsPointBased = item.IsPointBased;
                            aTransaction.Points = item.Points;
                            aTransaction.CustomerId = item.CustomerId;
                            aTransaction.IsUniqueItem = item.IsUniqueItem;
                            if (aTransaction.IsUniqueItem == true)
                            {
                                aTransaction.Quantity = 1;
                                aTransaction.OrderedQuantity = 1;
                            }
                            aTransaction.SerialNumber = item.SerialNumber;
                            aTransaction.Status = true;
                            if (aTransaction.Points > 0)
                            {
                                totalPoints = totalPoints + (int)aTransaction.Points;
                            }
                            db.PosOrderTransactions.Add(aTransaction);
                            subTotalPrice = subTotalPrice + item.Price;
                        }
                        else
                        {
                            //Save daily stock return

                            returnItem = new StockDailyReturn();
                            returnItem.OrderId = item.OrderId;
                            returnItem.ProductId = item.ProductId;
                            returnItem.ProductName = item.ProductName;
                            returnItem.DistributeId = item.DistributeId;
                            returnItem.Quantity = item.Quantity;
                            returnItem.DiscountType = item.DiscountType;
                            returnItem.PerItemPrice = item.PerItemPrice;
                            itemTotalPrice = returnItem.Quantity * returnItem.PerItemPrice;
                            if (item.Discount > 0)
                            {
                                discount = 0;
                                if (item.Discount > 0)
                                {
                                    discount = (decimal)(itemTotalPrice * (item.Discount / 100));
                                    itemTotalPrice = itemTotalPrice - discount;
                                    returnItem.Discount = discount;
                                }
                            }
                            returnItem.Price = itemTotalPrice;
                            returnItem.Date = now.Date;
                            db.StockDailyReturns.Add(returnItem);
                           
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

                            //if purchase order transaction
                            if (item.POItemId > 0)
                            {
                                var purchaseItem = db.PurchaseTransactions.Find(item.POItemId);
                                if (purchaseItem != null)
                                {
                                    purchaseItem.ReceiveQty = (int)(purchaseItem.ReceiveQty - item.Quantity);
                                    purchaseItem.RemainingQty = (int)(purchaseItem.RemainingQty + item.Quantity);
                                    db.Entry(purchaseItem).State = EntityState.Modified;
                                }
                            }
                        }

                        item.Status = false;
                        item.RefundBy = createdBy;
                        item.RefundDateTime = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }

                //purchase order transaction status changed
                if (aOrder.PurchaseOrderId > 0)
                {
                    var purchaseOrder = db.Purchases.Find(aOrder.PurchaseOrderId);
                    if (purchaseOrder != null)
                    {
                        if (purchaseOrder.Status == 4) // 4 complete 
                        {
                            purchaseOrder.Status = 3; // 3 approved
                            db.Entry(purchaseOrder).State = EntityState.Modified;
                        }
                    }
                }

                if (subTotalPrice > 0)
                {
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
                    aOrder.TotalPoints = totalPoints;
                    db.Entry(aOrder).State = EntityState.Modified;

                    //order payment save
                    OrderPayment payment = new OrderPayment();
                    payment.PreviousId = refundOrder.PaymentId;
                    payment.ReferenceOrderId = refundOrder.OrderId;
                    payment.OrderId = aOrder.OrderId;
                    payment.Amount = Payments.Sum(a => a.AmountPaid);
                    payment.Status = false;
                    payment.AmountPaid = Payments.Sum(a => a.AmountPaid);
                    payment.DueAmount = 0;
                    payment.IsDuePayment = false;
                    payment.ReturnAmount = 0;
                    payment.Date = DateTime.Now;
                    payment.CreatedBy = createdBy;
                    db.OrderPayments.Add(payment);

                    db.SaveChanges();

                    aOrder.PaymentId = payment.OrderPaymentId;
                    db.Entry(aOrder).State = EntityState.Modified;

                    refundOrder.NewOrderId = aOrder.OrderId;
                }
                else
                {
                    aOrder.Status = false;
                    db.Entry(aOrder).State = EntityState.Deleted;
                }
                refundOrder.Status = false;
                refundOrder.RefundBy = createdBy;
                refundOrder.RefundDateTime = DateTime.Now;
                db.Entry(refundOrder).State = EntityState.Modified;
                db.SaveChanges();
                //save payment transaction
                if (Payments != null)
                {
                    PaymentTransaction aPaymentTransaction;
                    //Payment transaction save
                    foreach (var aPayment in Payments)
                    {
                        aPaymentTransaction = new PaymentTransaction();
                        if(subTotalPrice > 0)
                        {
                            aPaymentTransaction.PaymentId = aOrder.OrderId;
                            aPaymentTransaction.RefOrderId = refundOrder.OrderId;
                        }
                        else
                        {
                            // if full refund 
                            aPaymentTransaction.PaymentId = refundOrder.OrderId;
                        }
                        aPaymentTransaction.CustomerId = refundOrder.CustomerId;
                        aPaymentTransaction.Type = 1; // Type 1 for order payment
                        aPaymentTransaction.InOut = false;
                        aPaymentTransaction.MethodId = 1;
                        aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                        aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                        aPaymentTransaction.Amount = aPayment.AmountPaid;
                        aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                        aPaymentTransaction.Date = DateTime.Now;
                        aPaymentTransaction.IsCreditPayment = false;
                        aPaymentTransaction.CreatedBy = createdBy;

                        db.PaymentTransactions.Add(aPaymentTransaction);

                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                        account.Balance = account.Balance - aPayment.AmountPaid;
                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }


                    //Increase purchase order refund amount and decrease dispatch amount if purchase order exist
                    if(refundOrder.PurchaseOrderId > 0)
                    {
                        var purchaseOrder = db.Purchases.Find(refundOrder.PurchaseOrderId);
                        if(purchaseOrder != null)
                        {
                            decimal poRefundPrice = 0;
                            poRefundPrice = Payments.Sum(a => a.AmountPaid);

                            if(purchaseOrder.DispatchAmount > 0)
                            {
                                purchaseOrder.DispatchAmount = purchaseOrder.DispatchAmount - poRefundPrice;
                            }

                            if(purchaseOrder.RefundAmount > 0)
                            {
                                purchaseOrder.RefundAmount = purchaseOrder.RefundAmount + poRefundPrice;
                            }
                            else
                            {
                                purchaseOrder.RefundAmount = poRefundPrice;
                            }
                            db.Entry(purchaseOrder).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Inactive Sales Order
        public ActionResult InactiveSalesOrder()
        {
            ViewBag.CountNumber = db.PosOrders.Where(a => a.Status == false).Count();
            return View();
        }
        public PartialViewResult InactiveOrderList(string selectedId, int? count, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            List<ViewPosOrder> list = new List<ViewPosOrder>();
            if (!string.IsNullOrEmpty(selectedId))
            {
                list.Clear();
                foreach (var id in selectedId.Split(','))
                {
                    var orderId = Convert.ToInt64(id);
                    ViewPosOrder aOrder = db.ViewPosOrders.FirstOrDefault(a => a.OrderId == orderId);
                    if (aOrder != null)
                    {
                        list.Add(aOrder);
                    }
                }
                list = list.OrderByDescending(o => o.OrderDate).ToList();
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewPosOrders.Where(a => a.Status == false).OrderByDescending(a => a.OrderDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date))
                        .OrderByDescending(a => a.OrderDate)
                        .ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end))
                        .OrderByDescending(a => a.OrderDate)
                        .ToList();
                }
            }
            return PartialView(list);
        }
        public JsonResult GetInactiveOrderList(string text, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<PosOrder>();
            if (!string.IsNullOrEmpty(text))
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.PosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date) && m.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }

                if (start != null && end != null)
                {
                    list = db.PosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end) && m.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days == 0 || days == null)
                {
                    list = db.PosOrders.Where(a => a.Status == false && a.OrderNumber.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.OrderDate).Take(5).ToList();
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.PosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) == DbFunctions.TruncateTime(countDate.Date))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.PosOrders.Where(m => m.Status == false && DbFunctions.TruncateTime(m.OrderDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.OrderDate) <= DbFunctions.TruncateTime(end))
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5).ToList();
                }
                else if (days == 0 || days == null)
                {
                    list = db.PosOrders.Where(a => a.Status == false)
                        .OrderByDescending(o => o.OrderDate).Take(5).ToList();
                }
            }
            var orderList = new SelectList(list.Select(s => new { s.OrderId, s.OrderNumber }).ToList(), "OrderId", "OrderNumber");
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Purchase Order
        public ActionResult PurchaseOrder()
        {
            ViewBag.CountAll = db.Purchases.Where(a => a.Status != 5).Count();
            ViewBag.CountPending = db.Purchases.Where(a => a.Status == 1).Count();
            ViewBag.CountPartial = db.Purchases.Where(a => a.Status == 2).Count();
            ViewBag.CountApproved = db.Purchases.Where(a => a.Status == 3).Count();
            ViewBag.CountCompleted = db.Purchases.Where(a => a.Status == 4).Count();
            ViewBag.CountDeleted = db.Purchases.Where(a => a.Status == 5).Count();
            return View();
        }
        public ActionResult PurchaseOrderCreate()
        {
            ViewBag.TaxPercent = db.MiscFuntions.Find(2).TaxRate;
            return View();
        }
        public JsonResult GetProductList(string ids, string text)
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
        public JsonResult GetCustomerList(string text)
        {
            var list = new List<Customer>();
            var alist = new SelectList(db.Customers.Where(a => a.Status == true).Select(a => new { a.CustomerId, a.Name }), "CustomerId", "Name");
            //if (!string.IsNullOrEmpty(text))
            //{
            //    list = db.Suppliers.Where(a => a.Name.ToLower().Contains(text.ToLower()) && a.Status == true).OrderBy(a => a.Name).ToList();
            //    alist = new SelectList(list.Select(a => new { Name = a.Name, SupplierId = a.SupplierId }), "SupplierId", "Name");
            //}
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PurchaseOrderSave(ImportModel model)
        {
            try
            {
                Purchase pOrder = new Purchase();
                long countNumber = 1;
                if (db.Purchases.Any())
                {
                    countNumber = db.Purchases.Max(a => a.Id) + 1;
                }
                pOrder.VoucherName = "PO_" + countNumber.ToString("0000");
                pOrder.Status = 1;
                pOrder.CustomerId = model.CustomerId;
                pOrder.DeliveryDate = model.DeliveryDate;
                pOrder.TotalAmount = model.TotalCost;
                pOrder.TaxPercent = model.TaxPercent;
                pOrder.TaxAmount = model.TaxAmount;
                pOrder.CreatedBy = model.UserId;
                pOrder.CreatedDate = DateTime.Now;
                pOrder.Comments = model.Comments;
                db.Purchases.Add(pOrder);

                db.SaveChanges();

                if (model.ImportData != null)
                {
                    PurchaseTransaction pOrderTrans;
                    foreach (var data in model.ImportData)
                    {
                        pOrderTrans = new PurchaseTransaction();
                        pOrderTrans.PurchaseId = pOrder.Id;
                        pOrderTrans.ProductId = data.ProductId;
                        pOrderTrans.DistributeId = data.DistributeId;
                        pOrderTrans.ProductName = data.ProductName;
                        pOrderTrans.RequestQty = (int)data.Quantity;
                        pOrderTrans.RemainingQty = (int)data.Quantity;
                        pOrderTrans.Status = 1;
                        pOrderTrans.PeritemCost = data.PeritemCost;
                        pOrderTrans.Cost = data.Cost;
                        pOrderTrans.CreatedBy = model.UserId;
                        pOrderTrans.CreatedDate = DateTime.Now;
                        db.PurchaseTransactions.Add(pOrderTrans);

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
        public JsonResult GetPurchaseOrderVoucherList(string text, int? status, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new SelectList("", "");
            if (status > 0)
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.Purchases.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.Purchases.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.Purchases.Where(a => a.Status == status).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.Purchases.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.Purchases.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.Purchases.Where(a => a.Status != 5).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult PurchaseOrderList(int? status, int? days, DateTime? from, DateTime? to, int? count, string selectedId)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewPurchaseOrder>();
            if (!string.IsNullOrEmpty(selectedId))
            {
                foreach (var id in selectedId.Split(','))
                {
                    long pOrderId = Convert.ToInt64(id);
                    var pOrder = db.ViewPurchaseOrders.FirstOrDefault(a => a.Id == pOrderId);
                    if (pOrder != null)
                    {
                        list.Add(pOrder);
                    }
                }
            }
            else if (status > 0)
            {
                if (count > 0)
                {
                    list = db.ViewPurchaseOrders.Where(a => a.Status == status).OrderByDescending(a => a.CreatedDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPurchaseOrders.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPurchaseOrders.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewPurchaseOrders.Where(a => a.Status != 5).OrderByDescending(a => a.CreatedDate).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPurchaseOrders.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPurchaseOrders.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.CreatedDate));
        }
        [EncryptedActionParameter]
        public ActionResult PurchaseOrderDetails(long id, bool? isView)
        {
            ViewBag.IsView = false;
            if (isView == true)
            {
                ViewBag.IsView = true;
            }
            return View(db.ViewPurchaseOrders.FirstOrDefault(a => a.Id == id));
        }
        public PartialViewResult PurchaseTransList(long id, bool IsView)
        {
            ViewBag.IsView = IsView;
            return PartialView(db.ViewPurchaseOrderTransactions.Where(a => a.PurchaseId == id && a.Status > 0).ToList());
        }
        public JsonResult PurchaseOrderStatusChange(long id, int status, int CreatedBy)
        {
            try
            {
                var purchaseOrder = db.Purchases.Find(id);
                purchaseOrder.Status = status;
                if (status == 3) //voucher approved
                {
                    purchaseOrder.ApprovedBy = CreatedBy;
                    purchaseOrder.ApprovedDate = DateTime.Now;
                }
                else
                {
                    purchaseOrder.UpdatedBy = CreatedBy;
                    purchaseOrder.UpdatedDate = DateTime.Now;
                }
                db.Entry(purchaseOrder).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AddPurchaseOrderItem(long purchaseId, long? itemId, bool? isQuantityEdit)
        {
            ViewBag.PurchaseId = purchaseId;
            ViewBag.ItemId = itemId;
            ViewBag.IsQuantityEdit = false;
            if (isQuantityEdit == true)
            {
                ViewBag.IsQuantityEdit = true;
            }
            return PartialView();
        }
        public PartialViewResult AddPurchaseOrderItemPartial(long? itemId, long purchaseId, bool? isQuantityEdit)
        {
            ViewBag.IsQuantityEdit = isQuantityEdit;
            var productList = db.ViewProducts.Where(a => a.Status == true).Select(s => new { s.RowID, s.ProductId, s.DistributeId, s.ProductName }).ToList();
            var purchaseOrderList = db.PurchaseTransactions.Where(a => a.PurchaseId == purchaseId && a.Status > 0).ToList();
            if (purchaseOrderList != null)
            {
                foreach (var item in purchaseOrderList)
                {
                    productList.Remove(productList.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId));
                }
            }
            ViewBag.ProductList = new SelectList(productList, "RowID", "ProductName");
            if (itemId > 0)
            {
                var purchaseItem = db.PurchaseTransactions.Find(itemId);
                return PartialView(purchaseItem);
            }
            return PartialView();
        }

        public JsonResult PurchaseOrderItemUpdate(PurchaseTransaction item, long? RowId, int? changeQuantity)
        {
            try
            {
                decimal taxPercent = 0;
                decimal taxAmount = 0;
                decimal totalAmount = 0;
                var purchase = db.Purchases.Find(item.PurchaseId);

                if (purchase.Status != 1) //status 1 is pending for approve
                {
                    purchase.Status = 2; // partial approved
                    db.Entry(purchase).State = EntityState.Modified;
                }
                if (item.Id > 0) //purchase item update
                {
                    var purchaseItem = db.PurchaseTransactions.Find(item.Id);
                    item.Status = purchaseItem.Status;
                    if (changeQuantity > 0)
                    {
                        purchaseItem.ChangeRequest = true;
                        purchaseItem.ChangeQty = changeQuantity;
                    }
                    else
                    {
                        purchaseItem.RequestQty = item.RequestQty;
                        purchaseItem.RemainingQty = item.RequestQty;
                        purchaseItem.PeritemCost = item.PeritemCost;
                        purchaseItem.Cost = item.Cost;
                    }
                    purchaseItem.UpdatedBy = item.CreatedBy;
                    purchaseItem.UpdatedDate = DateTime.Now;
                    db.Entry(purchaseItem).State = EntityState.Modified;
                }
                else if(RowId > 0) // purchase item create
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == RowId);
                    item.RemainingQty = item.RequestQty;
                    item.ProductId = product.ProductId;
                    item.DistributeId = (long)product.DistributeId;
                    item.ProductName = product.ProductName;
                    item.CreatedDate = DateTime.Now;
                    if (purchase.Status != 1)
                    {
                        item.Status = 2; //not approve
                    }
                    else
                    {
                        item.Status = 1; // active
                    }
                    db.PurchaseTransactions.Add(item);
                }
                db.SaveChanges();

                //calculate purchase total amount
                if (item.Status == 1)
                {
                    taxPercent = purchase.TaxPercent;
                    totalAmount = (decimal)db.PurchaseTransactions.Where(a => a.PurchaseId == item.PurchaseId && a.Status == 1 && a.Cost > 0).Sum(a => a.Cost);

                    taxAmount = totalAmount * (taxPercent / 100);
                    totalAmount = totalAmount + taxAmount;

                    purchase.TotalAmount = totalAmount;
                    purchase.TaxAmount = taxAmount;
                    db.Entry(purchase).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult PurchaseOrderItemStatusChange(long itemId, bool? isChangesApprove, bool? isRemoveChanges, bool? isDeleteItem, bool? isItemApprove)
        {
            try
            {
                var purchaseItem = db.PurchaseTransactions.Find(itemId);
                purchaseItem.Status = 1;
                if (isDeleteItem == true)
                {
                    purchaseItem.Status = 0;
                }
                if (isChangesApprove == true)
                {
                    purchaseItem.RequestQty = (int)purchaseItem.ChangeQty;
                    purchaseItem.RemainingQty = (int)purchaseItem.ChangeQty;
                    purchaseItem.Cost = purchaseItem.RequestQty * purchaseItem.PeritemCost;
                    purchaseItem.ChangeQty = 0;
                    purchaseItem.ChangeRequest = false;
                }
                else if (isRemoveChanges == true)
                {
                    purchaseItem.ChangeQty = 0;
                    purchaseItem.ChangeRequest = false;
                }
                db.Entry(purchaseItem).State = EntityState.Modified;
                db.SaveChanges();

                //purchase total amount
                if (isChangesApprove == true || isDeleteItem == true || isItemApprove == true)
                {
                    decimal taxPercent = 0;
                    decimal totalAmount = 0;
                    decimal taxAmount = 0;

                    var purchase = db.Purchases.Find(purchaseItem.PurchaseId);
                    taxPercent = purchase.TaxPercent;
                    totalAmount = (decimal)db.PurchaseTransactions.Where(a => a.PurchaseId == purchase.Id && a.Status == 1 && a.Cost > 0).Sum(a => a.Cost);

                    taxAmount = totalAmount * (taxPercent / 100);
                    totalAmount = totalAmount + taxAmount;

                    purchase.TotalAmount = totalAmount;
                    purchase.TaxAmount = taxAmount;
                    db.Entry(purchase).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //purchase status update
                if (db.PurchaseTransactions.Where(a => a.PurchaseId == purchaseItem.PurchaseId && a.Status > 0).Any(a => a.Status == 2) == false)
                {
                    var purchase = db.Purchases.Find(purchaseItem.PurchaseId);
                    purchase.Status = 3; //approve
                    db.Entry(purchase).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseOrderProducts(long? customerId)
        {
            var list = new SelectList("", "");
            if (customerId > 0)
            {
                list = new SelectList(db.ViewPurchaseOrders.Where(a => (a.Status == 3 || a.Status == 2) && a.CustomerId == customerId).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult PO_SalesOrder(long poId)
        {
            ViewBag.POId = poId;
            return PartialView();
        }
        public PartialViewResult PO_SalesOrderList(long poId, bool? isWithDetails)
        {
            ViewBag.Details = false;
            if(isWithDetails == true)
            {
                ViewBag.Details = true;
            }
            return PartialView(db.ViewPosOrders.Where(a => a.PurchaseOrderId == poId && a.Status == true).ToList());
        }
        #endregion

        #region Purchase Order Print
        [EncryptedActionParameter]
        public ActionResult PO_Print(long poId, int type)
        {
            ViewBag.Type = type;
            return View(db.ViewPurchaseOrders.FirstOrDefault(a => a.Id == poId));
        }
        #endregion

        #region Customer Create
        public PartialViewResult CustomerCreate()
        {
            return PartialView();
        }
        public PartialViewResult CustomerCreatePartial()
        {
            ViewBag.CreditLimit = new SelectList(db.CreditLimits.Where(a => a.Status == true).Select(s => new { s.Id, Name = s.Name + "(" + s.Limit + ")" }), "Id", "Name");
            return PartialView();
        }
        public JsonResult CustomerCreateSave(CustomerModelView model)
        {
            try
            {
                Customer aCustomer = new Customer();
                aCustomer.Name = model.Name;
                aCustomer.Email = model.Email;
                aCustomer.Phone = model.Phone;
                aCustomer.IsCreditAllowed = model.IsCreditAllowed;
                aCustomer.CreditLimitId = model.CreditLimitId;
                aCustomer.Address = model.Address;
                aCustomer.MembershipNumber = model.MembershipNumber;
                aCustomer.CreatedBy = (int)model.CreatedBy;
                aCustomer.CreatedDate = DateTime.Now;
                aCustomer.Status = true;
                db.Customers.Add(aCustomer);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region After sale service
        public ActionResult SaleService()
        {
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult SaleServiceCreate(long? assId)
        {
            ViewBag.ASaleServiceId = assId;
            ViewBag.ConditionList = new SelectList(db.Conditions.Select(s => new { s.Id, s.Name }), "Id", "Name");
            return View();
        }
        public PartialViewResult AfterSaleServiceList(int? status, long? assId)
        {
            ViewBag.Status = status;
            var list = new List<ViewAfterSaleService>();
            if (status > 0 && (assId == null || assId == 0))
            {
                list = db.ViewAfterSaleServices.Where(a => a.Status == status).ToList();
            }
            else if (assId > 0)
            {
                list = db.ViewAfterSaleServices.Where(a => a.Id == assId).ToList();
            }
            else
            {
                list = db.ViewAfterSaleServices.Where(a => a.Status != 1).ToList();
            }
            return PartialView(list);
        }
        public PartialViewResult SaleServiceStatusChange(long assId)
        {
            ViewBag.Id = assId;
            return PartialView();
        }
        public PartialViewResult SaleServiceStatusChangePartial(long id)
        {
            int status = db.AfterSaleServices.Find(id).Status;
            ViewBag.Status = status;
            ViewBag.StatusList = new SelectList(db.ServiceStatus.Where(a => a.Status != 1).Select(s => new { s.Id, s.Name }), "Id", "Name");
            return PartialView();
        }
        public JsonResult SaleServiceStatusChangeSave(long assId, int status, int createdBy)
        {
            try
            {
                var saleService = db.AfterSaleServices.Find(assId);
                if (saleService.Status != status)
                {
                    saleService.Status = status;
                    saleService.UpdatedBy = createdBy;
                    saleService.UpdatedDate = now.Date;
                    db.Entry(saleService).State = EntityState.Modified;

                    var statusName = db.ServiceStatus.Find(status).Name;

                    SaleServiceChangeArchive aChange = new SaleServiceChangeArchive();
                    aChange.SaleServiceId = assId;
                    aChange.Type = "Status Change -" + statusName;
                    aChange.CreatedBy = createdBy;
                    aChange.CreatedDate = now.Date;
                    db.SaleServiceChangeArchives.Add(aChange);

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ServicesTab()
        {
            ViewBag.ServiceStatusList = new SelectList(db.ServiceStatus.Where(a => a.Status != 1).Select(s => new { s.Id, s.Name }), "Id", "Name");
            return PartialView();
        }

        public PartialViewResult IncompleteTab()
        {
            return PartialView();
        }

        public JsonResult GetAfterSaleService(int searchType, int? status, string text)
        {
            var list = new List<ViewAfterSaleService>();
            if (!string.IsNullOrEmpty(text))
            {
                if (status > 0)
                {
                    if (searchType == 1) //search by serial number
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.SerialNumber.ToLower().Contains(text.ToLower()) && a.Status == status).ToList();
                    }
                    else if (searchType == 2) //search by invoice no
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.InvoiceNo.ToLower().Contains(text.ToLower()) && a.Status == status).ToList();
                    }
                    else if (searchType == 3) // search by work order no
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.WorkOrderNo.ToLower().Contains(text.ToLower()) && a.Status == status).ToList();
                    }
                    else if (searchType == 4) // search by temporary order no
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.TemporaryWorkOrderNo.ToLower().Contains(text.ToLower()) && a.Status == status).ToList();
                    }
                }
                else
                {
                    if (searchType == 1) //search by serial number
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.SerialNumber.ToLower().Contains(text.ToLower()) && a.Status != 1).ToList();
                    }
                    else if (searchType == 2) //search by invoice no
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.InvoiceNo.ToLower().Contains(text.ToLower()) && a.Status != 1).ToList();
                    }
                    else if (searchType == 3) // search by work order no
                    {
                        list = db.ViewAfterSaleServices.Where(a => a.WorkOrderNo.ToLower().Contains(text.ToLower()) && a.Status != 1).ToList();
                    }
                }

            }
            else
            {
                if (status > 0)
                {
                    list = db.ViewAfterSaleServices.Where(a => a.Status == status).ToList();
                }
                else
                {
                    list = db.ViewAfterSaleServices.Where(a => a.Status != 1).ToList();
                }
            }
            var selectList = new SelectList("", "");
            if (searchType == 1) //search by serial number
            {
                selectList = new SelectList(list.Select(s => new { s.Id, s.SerialNumber }), "Id", "SerialNumber");
            }
            else if (searchType == 2)//search by invoice no
            {
                selectList = new SelectList(list.Select(s => new { s.Id, s.InvoiceNo }), "Id", "InvoiceNo");
            }
            else if (searchType == 3) // search by work order no
            {
                selectList = new SelectList(list.Select(s => new { s.Id, s.WorkOrderNo }), "Id", "WorkOrderNo");
            }
            else if (searchType == 4) // search by temporary order no
            {
                selectList = new SelectList(list.Select(s => new { s.Id, s.TemporaryWorkOrderNo }), "Id", "TemporaryWorkOrderNo");
            }
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSerial()
        {
            var serialNumberList = new SelectList(db.PosOrderTransactions.Where(a => a.Status == true && a.IsAfterSaleService == true && a.IsUniqueItem == true).Select(s => new { s.SerialNumber, s.OrderTransactionId }), "OrderTransactionId", "SerialNumber");
            return Json(serialNumberList, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ServiceExpireInfo(long orderTransId)
        {
            var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransId && a.Status == true);
            //calculate service days
            int serviceDays = 0;
            DateTime orderDate = new DateTime();
            DateTime expireDate = new DateTime();
            orderDate = (DateTime)orderTransaction.OrderDate;
            if (orderTransaction.ServiceDays > 0)
            {
                serviceDays = (int)orderTransaction.ServiceDays;
            }
            expireDate = orderDate.AddDays(serviceDays);
            ViewBag.ExpireDate = expireDate;
            return PartialView(orderTransaction);
        }
        public PartialViewResult MultiProServiceInfo(long orderId)
        {
            var orderTransList = db.ViewOrderTransactions.Where(a => a.OrderId == orderId && a.IsAfterSaleService == true && a.Status == true).ToList();
            return PartialView(orderTransList);
        }
        //Customer 
        public PartialViewResult ServicesCustomerCreatePartial(long? customerId, long? assId)
        {
            ServiceCustomerModelView model = new ServiceCustomerModelView();
            if (customerId > 0)
            {
                var customer = db.Customers.Find(customerId);
                model.FirstName = customer.Name;
                model.Email = customer.Email;
                model.Mobile = customer.Phone;
                model.AddressLine1 = customer.Address;
                return PartialView(model);
            }
            else if (assId > 0)
            {
                var servicesCustomer = db.ServicesCustomers.FirstOrDefault(a => a.ASSId == assId);
                model.FirstName = servicesCustomer.FirstName;
                model.LastName = servicesCustomer.LastName;
                model.Email = servicesCustomer.Email;
                model.Mobile = servicesCustomer.Mobile;
                model.AlternateMobile = servicesCustomer.AlternateMobile;
                model.AddressLine1 = servicesCustomer.Addressline1;
                model.AddressLine2 = servicesCustomer.Addressline2;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult AfterSaleServiceSave(long orderTransId, int createdBy, int? conditionId, string conditionObservation)
        {
            long tempOrder = 0;
            try
            {
                var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransId);

                tempOrder = db.AfterSaleServices.Count() + 1;

                //after sale service save
                AfterSaleService aSaleService = new AfterSaleService();
                aSaleService.TemporaryWorkOrderNo = "TempOrder_" + tempOrder.ToString("000");
                aSaleService.InvoiceNo = orderTransaction.OrderNumber;
                aSaleService.OrderTransactionId = orderTransId;
                aSaleService.Status = 1;
                aSaleService.StepCounter = 1;
                aSaleService.CreatedBy = createdBy;
                aSaleService.CreatedDate = now.Date;
                db.AfterSaleServices.Add(aSaleService);
                db.SaveChanges();

                //service product save
                ServicesProduct sProduct = new ServicesProduct();
                sProduct.AssId = aSaleService.Id;
                sProduct.ProductId = orderTransaction.ProductId;
                sProduct.DistributeId = orderTransaction.DistributeId;
                sProduct.ProductName = orderTransaction.ProductName;
                sProduct.SerialNumber = orderTransaction.SerialNumber;
                sProduct.ConditionId = conditionId;
                sProduct.ConditionObservation = conditionObservation;
                sProduct.CreatedBy = createdBy;
                sProduct.CreatedDate = now.Date;
                db.ServicesProducts.Add(sProduct);
                db.SaveChanges();

                return Json(aSaleService.Id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetServicesProduct(string ids, string text)
        {
            var list = new SelectList("");
            List<ViewServicesProduct> productList = new List<ViewServicesProduct>();
            if (!string.IsNullOrEmpty(text))
            {
                productList = db.ViewServicesProducts.Where(a => a.ASStatus != 1 && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).ToList();
            }
            else
            {
                productList = db.ViewServicesProducts.Where(a => a.ASStatus != 1).Take(5).ToList();
            }
            if (!string.IsNullOrEmpty(ids))
            {
                long servicesProductId = 0;
                foreach (var id in ids.Split(','))
                {
                    servicesProductId = Convert.ToInt64(id);
                    if (productList.Any(a => a.Id == servicesProductId))
                    {
                        productList.Remove(productList.FirstOrDefault(a => a.Id == servicesProductId));
                    }
                }
            }
            var alist = new SelectList(productList.Select(a => new { a.Id, ProductName = a.ProductName + "(" + a.WorkOrderNo + ")" }), "Id", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ServicesCustomerSave(ServicesCustomer customer)
        {
            try
            {
                if (customer.Id > 0)
                {
                    //customer update
                    var existingCustomer = db.ServicesCustomers.Find(customer.Id);
                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.Mobile = customer.Mobile;
                    existingCustomer.AlternateMobile = customer.AlternateMobile;
                    existingCustomer.Addressline1 = customer.Addressline1;
                    existingCustomer.Addressline2 = customer.Addressline2;
                    existingCustomer.UpdatedBy = customer.UpdatedBy;
                    existingCustomer.UpdatedDate = now.Date;
                    db.Entry(existingCustomer).State = EntityState.Modified;

                    //change save 
                    SaleServiceChangeArchive aChange = new SaleServiceChangeArchive();
                    aChange.SaleServiceId = existingCustomer.ASSId;
                    aChange.Type = "Customer Update";
                    aChange.CreatedBy = (int)customer.UpdatedBy;
                    aChange.CreatedDate = now.Date;
                    db.SaleServiceChangeArchives.Add(aChange);
                }
                else
                {
                    //customer save
                    customer.CreatedDate = now.Date;
                    db.ServicesCustomers.Add(customer);

                    //after sale service step update 

                    var aSaleService = db.AfterSaleServices.Find(customer.ASSId);
                    aSaleService.StepCounter = 2;
                    db.Entry(aSaleService).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ServicesSupplementarySave(string suppIds, long aSalerServiceId, int createdBy)
        {
            try
            {
                ServicesSupplimentary supp;
                int suppId = 0;
                if (!string.IsNullOrEmpty(suppIds))
                {
                    foreach (var id in suppIds.Split(','))
                    {
                        suppId = Convert.ToInt32(id);
                        supp = new ServicesSupplimentary();
                        supp.AssId = aSalerServiceId;
                        supp.SupplementaryId = suppId;
                        supp.Status = true;
                        supp.CreatedBy = createdBy;
                        supp.CreatedDate = now.Date;
                        db.ServicesSupplimentaries.Add(supp);
                        db.SaveChanges();
                    }
                }

                //after sale service step update 

                var aSaleService = db.AfterSaleServices.Find(aSalerServiceId);
                aSaleService.StepCounter = 3;
                db.Entry(aSaleService).State = EntityState.Modified;

                db.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ServicesServiceSave(string serviceIds, decimal totalCharge, long aSaleServiceId, int createdBy)
        {
            try
            {
                ServicesService service;
                int serviceId = 0;

                if (!string.IsNullOrEmpty(serviceIds))
                {
                    foreach (var id in serviceIds.Split(','))
                    {
                        serviceId = Convert.ToInt32(id);
                        service = new ServicesService();
                        service.AssId = aSaleServiceId;
                        service.ServiceId = serviceId;
                        service.CreatedBy = createdBy;
                        service.CreatedDate = now.Date;
                        db.ServicesServices.Add(service);
                        db.SaveChanges();
                    }
                }
                //after sale service step update 

                var aSaleService = db.AfterSaleServices.Find(aSaleServiceId);
                aSaleService.StepCounter = 4;
                aSaleService.ServiceChargeTotal = totalCharge;
                db.Entry(aSaleService).State = EntityState.Modified;

                db.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ServiceDeliverySave(DateTime deliveryDate, long aSaleServiceId, int createdBy)
        {
            try
            {
                //after sale service step update 
                long totalService = 0;
                totalService = db.AfterSaleServices.Where(a => a.Status != 1).Count() + 1;

                var aSaleService = db.AfterSaleServices.Find(aSaleServiceId);
                aSaleService.WorkOrderNo = "service_" + totalService.ToString("000");
                aSaleService.StepCounter = 5;
                aSaleService.Status = 2;
                aSaleService.DeliveryDate = deliveryDate;
                db.Entry(aSaleService).State = EntityState.Modified;

                db.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        #region condition edit
        public PartialViewResult ProductConditionEdit(long assId)
        {
            ViewBag.AfterSaleServiceId = assId;
            return PartialView();
        }
        public PartialViewResult ProductConditionEditPartial()
        {
            ViewBag.ConditionList = new SelectList(db.Conditions.Select(s => new { s.Id, s.Name }), "Id", "Name");
            return PartialView();
        }
        public JsonResult ConditionSave(long assId, int conditionId, string observation, int createdBy)
        {
            try
            {
                var conditionProduct = db.ServicesProducts.FirstOrDefault(a => a.AssId == assId);
                if (conditionProduct.ConditionId > 0)
                {
                    ServicesCondition sCondition = new ServicesCondition();
                    sCondition.ServicesProductId = conditionProduct.Id;
                    sCondition.ConditionId = (int)conditionProduct.ConditionId;
                    sCondition.ConditionObservation = conditionProduct.ConditionObservation;
                    sCondition.CreatedBy = createdBy;
                    sCondition.CreatedDate = now.Date;
                    db.ServicesConditions.Add(sCondition);
                }
                conditionProduct.ConditionId = conditionId;
                conditionProduct.ConditionObservation = observation;
                conditionProduct.UpdatedBy = createdBy;
                conditionProduct.UpdatedDate = now.Date;
                db.Entry(conditionProduct).State = EntityState.Modified;



                var conditionName = db.Conditions.Find(conditionId).Name;
                //change save
                SaleServiceChangeArchive aChange = new SaleServiceChangeArchive();
                aChange.SaleServiceId = assId;
                aChange.Type = "Condition Change - " + conditionName;
                aChange.CreatedBy = createdBy;
                aChange.CreatedDate = now.Date;
                db.SaleServiceChangeArchives.Add(aChange);

                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Contact info edit
        public PartialViewResult ContactEdit(long assId)
        {
            ViewBag.Id = assId;
            return PartialView();
        }
        public PartialViewResult ContactEditPartial(long assId)
        {
            ServiceCustomerModelView model = new ServiceCustomerModelView();
            var servicesCustomer = db.ServicesCustomers.FirstOrDefault(a => a.ASSId == assId);
            model.CustomerId = servicesCustomer.Id;
            model.FirstName = servicesCustomer.FirstName;
            model.LastName = servicesCustomer.LastName;
            model.Email = servicesCustomer.Email;
            model.Mobile = servicesCustomer.Mobile;
            model.AlternateMobile = servicesCustomer.AlternateMobile;
            model.AddressLine1 = servicesCustomer.Addressline1;
            model.AddressLine2 = servicesCustomer.Addressline2;
            return PartialView(model);
        }
        #endregion

        #region Supplementary Edit
        public PartialViewResult SupplementaryEdit(long assId)
        {
            ViewBag.Id = assId;
            ViewBag.ProductId = db.ServicesProducts.FirstOrDefault(a => a.AssId == assId).ProductId;
            return PartialView();
        }
        public PartialViewResult SupplementaryEditPartial(long assId)
        {
            return PartialView(db.ViewServicesSupplementaries.Where(a => a.AssId == assId && a.Status == true).ToList());
        }
        public JsonResult SupplementaryItemUpdate(string ids, long assId, int createdBy)
        {
            try
            {
                List<int> suppIds = new List<int>();
                int id = 0;
                ServicesSupplimentary sSupplimentary;
                if (!string.IsNullOrEmpty(ids))
                {
                    foreach (var suppId in ids.Split(','))
                    {
                        id = Convert.ToInt32(suppId);
                        suppIds.Add(id);
                        if (db.ServicesSupplimentaries.Any(a => a.SupplementaryId == id && a.AssId == assId))
                        {
                            sSupplimentary = db.ServicesSupplimentaries.FirstOrDefault(a => a.SupplementaryId == id && a.AssId == assId);
                            if (sSupplimentary.Status == false)
                            {
                                sSupplimentary.Status = true;
                                sSupplimentary.UpdatedBy = createdBy;
                                sSupplimentary.UpdatedDate = now.Date;
                                db.Entry(sSupplimentary).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            sSupplimentary = new ServicesSupplimentary();
                            sSupplimentary.AssId = assId;
                            sSupplimentary.SupplementaryId = id;
                            sSupplimentary.Status = true;
                            sSupplimentary.CreatedBy = createdBy;
                            sSupplimentary.CreatedDate = now.Date;
                            db.ServicesSupplimentaries.Add(sSupplimentary);
                        }
                        db.SaveChanges();
                    }
                }


                //change save 
                SaleServiceChangeArchive aChange = new SaleServiceChangeArchive();
                aChange.SaleServiceId = assId;
                aChange.Type = "Supplementary Change";
                aChange.CreatedBy = createdBy;
                aChange.CreatedDate = now.Date;
                db.SaleServiceChangeArchives.Add(aChange);
                db.SaveChanges();

                //services supplimentary inactive if not exist
                var sSuppList = db.ServicesSupplimentaries.Where(a => a.AssId == assId && a.Status == true).Select(s => new { s.SupplementaryId }).ToList();
                if (sSuppList.Any())
                {
                    foreach (var suppId in sSuppList)
                    {
                        if (suppIds.Any(a => a.Equals(suppId.SupplementaryId)) == false)
                        {
                            sSupplimentary = db.ServicesSupplimentaries.FirstOrDefault(a => a.AssId == assId && a.SupplementaryId == suppId.SupplementaryId);
                            sSupplimentary.Status = false;
                            db.Entry(sSupplimentary).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region complete temprary order
        public JsonResult GetAfterSaleServiceInfo(int step, long assId)
        {
            if (step == 1)
            {
                var aSaleService = db.AfterSaleServices.Find(assId);
                var serviceProduct = db.ServicesProducts.FirstOrDefault(a => a.AssId == assId);
                var orderTransaction = db.ViewOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == aSaleService.OrderTransactionId);
                //calculate service days
                int serviceDays = 0;
                DateTime orderDate = new DateTime();
                DateTime expireDate = new DateTime();
                orderDate = (DateTime)orderTransaction.OrderDate;
                if (orderTransaction.ServiceDays > 0)
                {
                    serviceDays = (int)orderTransaction.ServiceDays;
                }
                expireDate = orderDate.AddDays(serviceDays);
                return Json(
                    new
                    {
                        serviceProduct.ProductId,
                        serviceProduct.ProductName,
                        serviceProduct.SerialNumber,
                        serviceProduct.ConditionId,
                        serviceProduct.ConditionObservation,
                        OrderDate = orderDate,
                        ExpireDate = expireDate,
                        InvoiceNo = aSaleService.InvoiceNo,
                        aSaleService.StepCounter
                    }, JsonRequestBehavior.AllowGet);
            }
            else if (step == 3)
            {
                var suppItems = db.ViewServicesSupplementaries.Where(a => a.AssId == assId && a.Status == true).ToList();
                if (suppItems.Any())
                {
                    return Json(suppItems.Select(s => new { s.SupplementaryId, s.SupplementaryName }), JsonRequestBehavior.AllowGet);
                }
            }
            else if (step == 4)
            {
                var serviceItems = db.ViewServicesServices.Where(a => a.AssId == assId).ToList();
                if (serviceItems.Any())
                {
                    return Json(serviceItems.Select(s => new { s.ServiceId, s.ServiceName, s.ServiceCharge }), JsonRequestBehavior.AllowGet);
                }
            }
            return Json("notFound", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Service order Print
        [EncryptedActionParameter]
        public ActionResult ServiceOrderPrint(long aSaleServiceId)
        {
            return View(db.ViewAfterSaleServices.FirstOrDefault(a => a.Id == aSaleServiceId));
        }
        #endregion

        #region Condition History
        public PartialViewResult ConditionHistory(long assId)
        {
            ViewBag.Id = assId;
            return PartialView();
        }
        public PartialViewResult ConditionHistoryList(long id)
        {
            ViewBag.Id = id;
            return PartialView(db.ViewServicesConditions.Where(a => a.AssId == id).ToList());
        }
        #endregion

        #endregion

        #region Product service after sale
        public ActionResult ProductServiceInfo()
        {
            return View();
        }
        public PartialViewResult AllServiceTab(int? serviceType)
        {
            ViewBag.ServiceType = 0;
            if (serviceType > 0)
            {
                ViewBag.ServiceType = serviceType;
                ViewBag.VoucherList = new SelectList(db.ViewOrderTransactions.Where(a => a.ServiceTypeId == serviceType && a.Quantity > 0).Select(s => new { s.OrderId, s.OrderNumber }), "OrderId", "OrderNumber");
                ViewBag.CustomerList = new SelectList(db.ViewOrderTransactions.Where(a => a.ServiceTypeId == serviceType && a.Quantity > 0 && a.CustomerId > 0).GroupBy(g => g.CustomerId).Select(s => new { s.FirstOrDefault().CustomerId, s.FirstOrDefault().CustomerName }), "CustomerId", "CustomerName");
            }
            else
            {
                ViewBag.VoucherList = new SelectList(db.PosOrders.Where(a => a.Status == true && a.IsServiceItem == true).Select(s => new { s.OrderId, s.OrderNumber }), "OrderId", "OrderNumber");
                ViewBag.CustomerList = new SelectList(db.ViewOrderTransactions.Where(a => a.IsAfterSaleService == true && a.Quantity > 0 && a.CustomerId > 0).GroupBy(g => g.CustomerId).Select(s => new { s.FirstOrDefault().CustomerId, s.FirstOrDefault().CustomerName }), "CustomerId", "CustomerName");
            }
            return PartialView();
        }
        public PartialViewResult ProductServiceList(int? serviceType, long? orderId, long? customerId)
        {
            List<ViewOrderTransaction> transList = new List<ViewOrderTransaction>();
            if (serviceType > 0)
            {
                if (orderId > 0)
                {
                    transList = db.ViewOrderTransactions.Where(a => a.ServiceTypeId == serviceType && a.OrderId == orderId).ToList();
                }
                else if (customerId > 0)
                {
                    transList = db.ViewOrderTransactions.Where(a => a.ServiceTypeId == serviceType && a.CustomerId == customerId).ToList();
                }
                else
                {
                    transList = db.ViewOrderTransactions.Where(a => a.ServiceTypeId == serviceType).ToList();
                }
            }
            else
            {
                if (orderId > 0)
                {
                    transList = db.ViewOrderTransactions.Where(a => a.IsAfterSaleService == true && a.OrderId == orderId).ToList();
                }
                else if (customerId > 0)
                {
                    transList = db.ViewOrderTransactions.Where(a => a.IsAfterSaleService == true && a.CustomerId == customerId).ToList();
                }
                else
                {
                    transList = db.ViewOrderTransactions.Where(a => a.IsAfterSaleService == true).ToList();
                }
            }
            return PartialView(transList.OrderByDescending(a => a.OrderDate).ToList());
        }
        #endregion

        #region supplementary items
        public ActionResult SupplementaryItems()
        {
            return View();
        }
        public PartialViewResult SupplementaryCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult SupplementaryCreatePartial(int? id)
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            if (id > 0)
            {
                var supplementary = db.Supplementaries.Find(id);
                SupplementaryModelView model = new SupplementaryModelView();
                model.Id = supplementary.Id;
                model.Name = supplementary.Name;
                model.IsRequestConditionInfo = supplementary.IsRequestConditionInfo;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult SupplementarySave(SupplementaryModelView model)
        {
            try
            {
                int id = 0;
                List<int> ids = new List<int>();
                Supplementary aSupplementary;
                if (model.Id > 0)
                {
                    aSupplementary = db.Supplementaries.Find(model.Id);
                    aSupplementary.Name = model.Name;
                    aSupplementary.IsRequestConditionInfo = model.IsRequestConditionInfo;
                    aSupplementary.UpdatedBy = model.CreatedBy;
                    aSupplementary.UpdatedDate = now.Date;
                    db.Entry(aSupplementary).State = EntityState.Modified;
                }
                else
                {
                    aSupplementary = new Supplementary();
                    aSupplementary.Name = model.Name;
                    aSupplementary.IsRequestConditionInfo = model.IsRequestConditionInfo;
                    aSupplementary.Status = true;
                    aSupplementary.CreatedBy = model.CreatedBy;
                    aSupplementary.CreatedDate = now.Date;
                    db.Supplementaries.Add(aSupplementary);
                }
                db.SaveChanges();

                //supplementary category save
                SupplementaryCategory suppCategory;
                if (!string.IsNullOrEmpty(model.CategoryIds))
                {
                    ids.Clear();
                    foreach (var category in model.CategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(category);
                        ids.Add(id);
                        if (db.SupplementaryCategories.Any(a => a.SupplementaryId == aSupplementary.Id && a.CategoryId == id))
                        {
                            suppCategory = db.SupplementaryCategories.FirstOrDefault(a => a.SupplementaryId == aSupplementary.Id && a.CategoryId == id);
                            suppCategory.Status = true;
                            db.Entry(suppCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            suppCategory = new SupplementaryCategory();
                            suppCategory.SupplementaryId = aSupplementary.Id;
                            suppCategory.CategoryId = id;
                            suppCategory.Status = true;
                            db.SupplementaryCategories.Add(suppCategory);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.Id > 0)
                {
                    //supplimentary category inactive if not exist
                    var assignCategoryIds = db.SupplementaryCategories.Where(a => a.SupplementaryId == aSupplementary.Id && a.Status == true).Select(s => new { s.CategoryId }).ToList();
                    if (assignCategoryIds.Any())
                    {
                        foreach (var category in assignCategoryIds)
                        {
                            if (ids.Any(a => a.Equals(category.CategoryId)) == false)
                            {
                                suppCategory = db.SupplementaryCategories.FirstOrDefault(a => a.SupplementaryId == aSupplementary.Id && a.CategoryId == category.CategoryId);
                                suppCategory.Status = false;
                                db.Entry(suppCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //supplementary subcategory save
                SupplementarySubCategory suppSubCategory;
                if (!string.IsNullOrEmpty(model.SubCategoryIds))
                {
                    ids.Clear();
                    foreach (var subId in model.SubCategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(subId);
                        ids.Add(id);
                        if (db.SupplementarySubCategories.Any(a => a.SupplementaryId == aSupplementary.Id && a.SubCategoryId == id))
                        {
                            suppSubCategory = db.SupplementarySubCategories.FirstOrDefault(a => a.SupplementaryId == aSupplementary.Id && a.SubCategoryId == id);
                            suppSubCategory.Status = true;
                            db.Entry(suppSubCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            suppSubCategory = new SupplementarySubCategory();
                            suppSubCategory.SupplementaryId = aSupplementary.Id;
                            suppSubCategory.SubCategoryId = id;
                            suppSubCategory.Status = true;
                            db.SupplementarySubCategories.Add(suppSubCategory);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.Id > 0)
                {
                    //supplementary sub category inactive if not exist
                    var assignSubCategorys = db.SupplementarySubCategories.Where(a => a.SupplementaryId == aSupplementary.Id && a.Status == true).ToList();
                    if (assignSubCategorys.Any())
                    {
                        foreach (var sub in assignSubCategorys)
                        {
                            if (ids.Any(a => a.Equals(sub.SubCategoryId)) == false)
                            {
                                suppSubCategory = db.SupplementarySubCategories.FirstOrDefault(a => a.SupplementaryId == aSupplementary.Id && a.SubCategoryId == sub.SubCategoryId);
                                suppSubCategory.Status = false;
                                db.Entry(suppSubCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult SupplementaryItemList(int? status)
        {
            var list = new List<ViewSupplementary>();
            if (status == 1) // active
            {
                list = db.ViewSupplementaries.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewSupplementaries.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewSupplementaries.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewSupplementaries.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeSupplementaryStatus(int id, int status, int updateBy)
        {
            try
            {
                var supplementary = db.Supplementaries.Find(id);
                if (status == 1)
                {
                    supplementary.Status = true; //active
                }
                else if (status == 0)
                {
                    supplementary.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    supplementary.Status = null; // delete
                }
                supplementary.UpdatedBy = updateBy;
                supplementary.UpdatedDate = now.Date;
                db.Entry(supplementary).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplementaryItems(string ids, string text, int? productId)
        {
            List<Supplementary> suppList = new List<Supplementary>();
            if (productId > 0)
            {
                var proCategory = db.ViewProductCategories.Where(a => a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true && a.ProductId == productId).ToList();
                if (proCategory.Any())
                {
                    foreach (var aCategory in proCategory)
                    {
                        if (db.SupplementaryCategories.Any(a => a.CategoryId == aCategory.CategoryId && a.Status == true))
                        {
                            if (db.SupplementarySubCategories.Any(a => a.SubCategoryId == aCategory.SubCategoryId && a.Status == true))
                            {

                                var suppSubCategoryList = db.SupplementarySubCategories.Where(a => a.SubCategoryId == aCategory.SubCategoryId && a.Status == true).ToList();
                                foreach (var suppSubCate in suppSubCategoryList)
                                {
                                    suppList.AddRange(db.Supplementaries.Where(a => a.Id == suppSubCate.SupplementaryId && a.Status == true).ToList());
                                }
                            }
                            else
                            {
                                var suppCategoryList = db.SupplementaryCategories.Where(a => a.CategoryId == aCategory.CategoryId && a.Status == true).ToList();
                                foreach (var suppCateg in suppCategoryList)
                                {
                                    suppList.AddRange(db.Supplementaries.Where(a => a.Id == suppCateg.SupplementaryId && a.Status == true).ToList());
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(','))
                {
                    var suppId = Convert.ToInt32(id);
                    var list = suppList.FirstOrDefault(a => a.Id == suppId);
                    suppList.Remove(list);
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                suppList = suppList.Where(a => a.Name.ToLower().Contains(text.ToLower())).ToList();
            }
            var alist = new SelectList(suppList.Select(a => new { a.Id, a.Name }), "Id", "Name");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Service 
        public ActionResult Service()
        {
            return View();
        }
        public PartialViewResult ServiceCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult ServiceCreatePartial(int? id)
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            if (id > 0)
            {
                var service = db.Services.Find(id);
                ServiceModelView model = new ServiceModelView();
                model.Id = service.Id;
                model.Name = service.Name;
                model.ServiceCharge = service.ServiceCharge;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult ServiceSave(ServiceModelView model)
        {
            try
            {
                int id = 0;
                List<int> ids = new List<int>();
                Service service;
                if (model.Id > 0)
                {
                    service = db.Services.Find(model.Id);
                    service.Name = model.Name;
                    service.ServiceCharge = model.ServiceCharge;
                    service.UpdatedBy = model.CreatedBy;
                    service.UpdatedDate = now.Date;
                    db.Entry(service).State = EntityState.Modified;
                }
                else
                {
                    service = new Service();
                    service.Name = model.Name;
                    service.ServiceCharge = model.ServiceCharge;
                    service.Status = true;
                    service.CreatedBy = model.CreatedBy;
                    service.CreatedDate = now.Date;
                    db.Services.Add(service);
                }
                db.SaveChanges();

                //service category save
                ServiceCategory serviceCategory;
                if (!string.IsNullOrEmpty(model.CategoryIds))
                {
                    ids.Clear();
                    foreach (var category in model.CategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(category);
                        ids.Add(id);
                        if (db.ServiceCategories.Any(a => a.ServiceId == service.Id && a.CategoryId == id))
                        {
                            serviceCategory = db.ServiceCategories.FirstOrDefault(a => a.ServiceId == service.Id && a.CategoryId == id);
                            serviceCategory.Status = true;
                            db.Entry(serviceCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            serviceCategory = new ServiceCategory();
                            serviceCategory.ServiceId = service.Id;
                            serviceCategory.CategoryId = id;
                            serviceCategory.Status = true;
                            db.ServiceCategories.Add(serviceCategory);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.Id > 0)
                {
                    //service category inactive if not exist
                    var assignCategoryIds = db.ServiceCategories.Where(a => a.ServiceId == service.Id && a.Status == true).Select(s => new { s.CategoryId }).ToList();
                    if (assignCategoryIds.Any())
                    {
                        foreach (var category in assignCategoryIds)
                        {
                            if (ids.Any(a => a.Equals(category.CategoryId)) == false)
                            {
                                serviceCategory = db.ServiceCategories.FirstOrDefault(a => a.ServiceId == service.Id && a.CategoryId == category.CategoryId);
                                serviceCategory.Status = false;
                                db.Entry(serviceCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //service subcategory save
                ServiceSubCategory serviceSubCategory;
                if (!string.IsNullOrEmpty(model.SubCategoryIds))
                {
                    ids.Clear();
                    foreach (var subId in model.SubCategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(subId);
                        ids.Add(id);
                        if (db.ServiceSubCategories.Any(a => a.ServiceId == service.Id && a.SubCategoryId == id))
                        {
                            serviceSubCategory = db.ServiceSubCategories.FirstOrDefault(a => a.ServiceId == service.Id && a.SubCategoryId == id);
                            serviceSubCategory.Status = true;
                            db.Entry(serviceSubCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            serviceSubCategory = new ServiceSubCategory();
                            serviceSubCategory.ServiceId = service.Id;
                            serviceSubCategory.SubCategoryId = id;
                            serviceSubCategory.Status = true;
                            db.ServiceSubCategories.Add(serviceSubCategory);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.Id > 0)
                {
                    //service sub category inactive if not exist
                    var assignSubCategorys = db.ServiceSubCategories.Where(a => a.ServiceId == service.Id && a.Status == true).ToList();
                    if (assignSubCategorys.Any())
                    {
                        foreach (var sub in assignSubCategorys)
                        {
                            if (ids.Any(a => a.Equals(sub.SubCategoryId)) == false)
                            {
                                serviceSubCategory = db.ServiceSubCategories.FirstOrDefault(a => a.ServiceId == service.Id && a.SubCategoryId == sub.SubCategoryId);
                                serviceSubCategory.Status = false;
                                db.Entry(serviceSubCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ServiceList(int? status)
        {
            var list = new List<ViewService>();
            if (status == 1) // active
            {
                list = db.ViewServices.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewServices.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewServices.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewServices.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeServiceStatus(int id, int status, int updateBy)
        {
            try
            {
                var service = db.Services.Find(id);
                if (status == 1)
                {
                    service.Status = true; //active
                }
                else if (status == 0)
                {
                    service.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    service.Status = null; // delete
                }
                service.UpdatedBy = updateBy;
                service.UpdatedDate = now.Date;
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetService(string ids, string text, int? productId, int? serviceId)
        {
            List<Service> serviceList = new List<Service>();
            if (serviceId > 0)
            {
                var service = db.Services.Find(serviceId);
                if (service != null)
                {
                    return Json(service, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("notFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (productId > 0)
            {
                var proCategory = db.ViewProductCategories.Where(a => a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true && a.ProductId == productId).ToList();
                if (proCategory.Any())
                {
                    foreach (var aCategory in proCategory)
                    {
                        if (db.ServiceCategories.Any(a => a.CategoryId == aCategory.CategoryId && a.Status == true))
                        {
                            if (db.ServiceSubCategories.Any(a => a.SubCategoryId == aCategory.SubCategoryId && a.Status == true))
                            {

                                var serviceSubCategoryList = db.ServiceSubCategories.Where(a => a.SubCategoryId == aCategory.SubCategoryId && a.Status == true).ToList();
                                foreach (var serviceSubCate in serviceSubCategoryList)
                                {
                                    serviceList.AddRange(db.Services.Where(a => a.Id == serviceSubCate.ServiceId && a.Status == true).ToList());
                                }
                            }
                            else
                            {
                                var serviceCategoryList = db.ServiceCategories.Where(a => a.CategoryId == aCategory.CategoryId && a.Status == true).ToList();
                                foreach (var serviceCategory in serviceCategoryList)
                                {
                                    serviceList.AddRange(db.Services.Where(a => a.Id == serviceCategory.ServiceId && a.Status == true).ToList());
                                }
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(','))
                {
                    serviceId = Convert.ToInt32(id);
                    var list = serviceList.FirstOrDefault(a => a.Id == serviceId);
                    serviceList.Remove(list);
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                serviceList = serviceList.Where(a => a.Name.ToLower().Contains(text.ToLower())).ToList();
            }
            var alist = new SelectList(serviceList.Select(a => new { a.Id, a.Name }), "Id", "Name");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region After sale waste 
        [EncryptedActionParameter]
        public ActionResult AfterSaleWasteCreate(long assId)
        {
            return View(db.ViewAfterSaleServices.FirstOrDefault(a => a.Id == assId));
        }
        public PartialViewResult WastePayWin(long id)
        {
            return PartialView(db.ViewAfterSaleServices.FirstOrDefault(a => a.Id == id));
        }
        public JsonResult CheckProductAvailablity(long id)
        {
            var servicesProduct = db.ServicesProducts.FirstOrDefault(a => a.AssId == id);
            //check product in store
            bool isProductAvailable = false;
            if(db.Stocks.Any(a => a.ProductId == servicesProduct.ProductId && a.DistributeId == servicesProduct.DistributeId && a.Quantity > 0))
            {
                isProductAvailable = true;
            }
            return Json(isProductAvailable, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AfterSaleWasteSave(long Id, int WasteTypeId, int CreatedBy, string Comment, IList<PaymentModel> Payments, bool? IsReplace)
        {
            
            Waste waste = new Waste();
            WasteProduct wasteProduct;
            ServicesProduct sProduct;
            AfterSaleService aService;
            try
            {
                //Waste create
                long count = 1;
                if (db.Wastes.Any())
                {
                    count = db.Wastes.Max(a => a.Id) + 1;
                }
                waste.ReffNo = "WV-" + count.ToString("0000");
                waste.CreatedBy = CreatedBy;
                waste.CreatedDate = DateTime.Now;
                waste.Status = 1;
                waste.Type = 3; // 3 for sale service waste
                db.Wastes.Add(waste);

                db.SaveChanges();

                wasteProduct = new WasteProduct();
                wasteProduct.WasteId = waste.Id;

                //services product

                aService = db.AfterSaleServices.Find(Id);

                sProduct = db.ServicesProducts.FirstOrDefault(a => a.AssId == Id);

                //after sale service complete
                aService.Status = 3; //complete
                db.Entry(aService).State = EntityState.Modified;

                wasteProduct.ProductId = sProduct.ProductId;
                wasteProduct.DistributeId = sProduct.DistributeId;
                wasteProduct.ProductName = sProduct.ProductName;
                wasteProduct.Quantity = 1;
                wasteProduct.WasteTypeId = WasteTypeId;
                wasteProduct.Comments = Comment;
                wasteProduct.ASReffId = sProduct.AssId;
                db.WasteProducts.Add(wasteProduct);
                
                db.SaveChanges();

                if(IsReplace == true)
                {
                    //decrease store quantity
                    Stock stock = db.Stocks.FirstOrDefault(a => a.ProductId == sProduct.ProductId && a.DistributeId == sProduct.DistributeId);
                    if(stock != null)
                    {
                        stock.Quantity = stock.Quantity - 1;
                        db.Entry(stock).State = EntityState.Modified;
                    }
                    //sales order create
                    //order save
                    PosOrder aOrder = new PosOrder();
                    aOrder.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmssf");
                    aOrder.OrderType = 3; // after sale service
                    aOrder.ASReffId = Id; //after sale reff. id
                    aOrder.OrderDate = DateTime.Now;
                    aOrder.CreatedBy = CreatedBy;
                    aOrder.Status = true;
                    db.PosOrders.Add(aOrder);
                    db.SaveChanges();

                    PosOrderTransaction aTransaction = new PosOrderTransaction();
                    aTransaction.OrderId = aOrder.OrderId;
                    aTransaction.ProductId = sProduct.ProductId;
                    aTransaction.DistributeId = sProduct.DistributeId;
                    aTransaction.ProductName = sProduct.ProductName;
                    aTransaction.TransactionType = 1;
                    aTransaction.IsRefundAllow = false;
                    aTransaction.Quantity = 1;
                    aTransaction.OrderedQuantity = 1;
                    aTransaction.Status = true;
                    aTransaction.IsRefundAllow = false;
                    db.PosOrderTransactions.Add(aTransaction);
                    
                    //order payment save
                    OrderPayment payment = new OrderPayment();
                    payment.OrderId = aOrder.OrderId;
                    payment.Status = true; //debit payment;
                    payment.DueAmount = 0;
                    payment.IsDuePayment = false;
                    payment.Date = DateTime.Now;
                    payment.CreatedBy = CreatedBy;
                    db.OrderPayments.Add(payment);

                    db.SaveChanges();

                    //order paymentid save
                    aOrder = db.PosOrders.FirstOrDefault(a => a.OrderId == aOrder.OrderId);
                    aOrder.PaymentId = payment.OrderPaymentId;
                    db.Entry(aOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //payment transaction create
                    if (Payments != null)
                    {
                        PaymentTransaction aPaymentTransaction;
                        PaymentBody account;
                        foreach (var aPayment in Payments)
                        {
                            aPaymentTransaction = new PaymentTransaction();
                            aPaymentTransaction.PaymentId = aService.Id;
                            aPaymentTransaction.Type = 11; // Type 9 for warehouse import payment
                            aPaymentTransaction.IsCreditPayment = false;
                            aPaymentTransaction.MethodId = 1; // method id 5 for purchase order
                            aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                            aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                            aPaymentTransaction.Amount = aPayment.AmountPaid;
                            aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                            aPaymentTransaction.Date = DateTime.Now;
                            aPaymentTransaction.CreatedBy = CreatedBy;
                            aPaymentTransaction.InOut = false; // InOut false for release payment

                            //decrease amount from account
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance - aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;

                            db.PaymentTransactions.Add(aPaymentTransaction);

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult GetWasteTypeList(int? type)
        {
            var list = new SelectList("");
            if(type > 0)
            {
                list = new SelectList(db.WasteTypes.Where(a => a.Status == true && a.Type == type).Select(s => new { s.Id, s.TypeName }), "Id", "TypeName");
            }
            else
            {
                list = new SelectList(db.WasteTypes.Where(a => a.Status == true).Select(s => new { s.Id, s.TypeName }), "Id", "TypeName");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCategory(string text, int? categoryId, int? productId, int? supplementaryId, int? serviceId)
        {
            if (categoryId > 0)
            {
                var category = db.Categories.Find(categoryId);
                if (category != null)
                {
                    return Json(category, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else if (productId > 0)
            {
                var proCategories = db.ProductCategories.Where(a => a.ProductId == productId && a.Status == true).Join(db.Categories, pc => pc.CategoryId, c => c.CategoryId, (pc, c) => new { Text = c.CategoryName, Value = c.CategoryId });
                if (proCategories.Any())
                {
                    return Json(proCategories, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (supplementaryId > 0)
            {
                var suppCategory = db.SupplementaryCategories.Where(a => a.SupplementaryId == supplementaryId && a.Status == true).Join(db.Categories, sc => sc.CategoryId, c => c.CategoryId, (sc, c) => new { Text = c.CategoryName, Value = c.CategoryId });
                if (suppCategory.Any())
                {
                    return Json(suppCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (serviceId > 0)
            {
                var serviceCategory = db.ServiceCategories.Where(a => a.ServiceId == serviceId && a.Status == true).Join(db.Categories, sc => sc.CategoryId, c => c.CategoryId, (sc, c) => new { Text = c.CategoryName, Value = c.CategoryId });
                if (serviceCategory.Any())
                {
                    return Json(serviceCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            var categoryList = new List<Category>();
            if (!string.IsNullOrEmpty(text))
            {
                categoryList = db.Categories.Where(a => a.CategoryName.ToLower().Contains(text.ToLower()) && a.Status == true).ToList();
            }
            else
            {
                categoryList = db.Categories.Where(a => a.Status == true).OrderBy(o => o.CategoryName).ToList();
            }
            var alist = new SelectList(categoryList.Select(a => new { a.CategoryId, a.CategoryName }), "CategoryId", "CategoryName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCategories(int? categoryId, string categoryIds, bool? isMultiselect, int? masterProId, int? productId, int? supplementaryId, int? serviceId)
        {
            var subCategory = new List<SubCategory>();
            if (categoryId > 0)
            {
                subCategory = db.SubCategories.Where(a => a.CategoryId == categoryId && a.Status == true).ToList();
            }
            else if (!string.IsNullOrEmpty(categoryIds))
            {
                foreach (var id in categoryIds.Split(','))
                {
                    categoryId = Convert.ToInt32(id);
                    var subCategoryList = db.SubCategories.Where(a => a.CategoryId == categoryId && a.Status == true).ToList();
                    subCategory.AddRange(subCategoryList);
                }
                if (isMultiselect == false)
                {
                    return Json(subCategory, JsonRequestBehavior.AllowGet);
                }
            }
            else if (masterProId > 0)
            {
                var masterSubCategory = db.ViewMasterProductCategories
                                    .Where(a => a.MasterProductId == masterProId && a.MasterCategoryStatus == true && a.MasterSubCategoryStatus == true)
                                    .Select(s => new { Text = s.SubCategoryName, Value = s.SubCategoryId })
                                    .ToList();
                if (masterSubCategory.Any())
                {
                    return Json(masterSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (productId > 0)
            {
                //var proSubCategory = db.ProductCategories.Where(a => a.ProductId == productId && a.Status == true)
                //                    .Join(db.ProductSubCategories.Where(a => a.Status == true), pc => pc.Id, psc => psc.ParentId, (pc, psc) => new { psc.SubCategoryId })
                //                    .Join(db.SubCategories, psc => psc.SubCategoryId, sc => sc.SubCategoryId, (psc, sc) => new { Text = sc.Name, Value = sc.SubCategoryId })
                //                    .ToList();

                var proSubCategory = db.ViewProductCategories.Where(a => a.ProductId == productId && a.ProductSubCategoryStatus == true && a.ProductCategoryStatus == true).Select(s => new { Text = s.SubCategoryName, Value = s.SubCategoryId }).ToList();

                if (proSubCategory.Any())
                {
                    return Json(proSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (supplementaryId > 0)
            {
                var suppSubCategory = db.SupplementarySubCategories.Where(a => a.SupplementaryId == supplementaryId && a.Status == true).Join(db.SubCategories, ssc => ssc.SubCategoryId, sc => sc.SubCategoryId, (ssc, sc) => new { Text = sc.Name, Value = sc.SubCategoryId }).ToList();
                if (suppSubCategory.Any())
                {
                    return Json(suppSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if (serviceId > 0)
            {
                var serviceSubCategory = db.ServiceSubCategories.Where(a => a.ServiceId == serviceId && a.Status == true).Join(db.SubCategories, ssc => ssc.SubCategoryId, sc => sc.SubCategoryId, (ssc, sc) => new { Text = sc.Name, Value = sc.SubCategoryId }).ToList();
                if (serviceSubCategory.Any())
                {
                    return Json(serviceSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            var subList = new SelectList(subCategory.OrderBy(a => a.Name).Select(s => new { s.SubCategoryId, s.Name }), "SubCategoryId", "Name");
            return Json(subList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryByMasterProductId(int masterProductId)
        {
            var categoryList = db.ViewMasterProductCategories.Where(a => a.MasterProductId == masterProductId && a.MasterCategoryStatus == true).Select(s => new { Text = s.CategoryName, Value = s.CategoryId }).ToList();
            if (categoryList.Any())
            {
                return Json(categoryList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NotFound", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTag(string ids, string text)
        {
            var tagList = db.Tags.Where(a => a.Status == true).OrderBy(o => o.TagName).ToList();
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var id in ids.Split(','))
                {
                    var tagId = Convert.ToInt32(id);
                    var list = tagList.FirstOrDefault(a => a.Id == tagId);
                    tagList.Remove(list);
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                tagList = tagList.Where(a => a.TagName.ToLower().Contains(text.ToLower())).ToList();
            }
            var alist = new SelectList(tagList.Select(a => new { a.Id, a.TagName }), "Id", "TagName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTagByProductId(int id)
        {
            if (id > 0)
            {
                var tagList = db.ViewTagItems.Where(a => a.OwnerId == id && a.Type == 1 && a.Status == true).Select(s => new { Text = s.TagName, Value = s.TagId }).ToList();
                return Json(tagList, JsonRequestBehavior.AllowGet);
            }
            return Json("NotFound", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTagByMasterProductId(int id)
        {
            if (id > 0)
            {
                var tagList = db.ViewTagItems.Where(a => a.OwnerId == id && a.Type == 2 && a.Status == true).Select(s => new { Text = s.TagName, Value = s.TagId }).ToList();
                return Json(tagList, JsonRequestBehavior.AllowGet);
            }
            return Json("NotFound", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByRowId(long rowId)
        {
            if (db.ViewProducts.Any(a => a.RowID == rowId))
            {
                var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                return Json(new { product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem, product.ProductName, product.Price }, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }
        #region Configuration
        public ActionResult Configuration()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View();
        }
        #region Product
        //************Product Create start**************************
        public PartialViewResult ProductCreate(int? id)
        {
            ViewBag.ProductId = id;
            return PartialView();
        }
        public PartialViewResult ProductCreatePartial(int? id)
        {
            ViewBag.SubProduct = new SelectList(db.ViewMasterProducts.Where(a => a.Status == true).Select(s => new { s.Id, Name = s.Name + "(" + s.BrandName + ")" }), "Id", "Name");
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            ViewBag.UnitList = new SelectList(db.Units.Where(s => s.Status == 1), "UnitId", "UnitName");
            ViewBag.VateRateList = new SelectList(db.Vats, "VatId", "Name");
            ViewBag.RestrictionList = new SelectList(db.Restrictions.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            ViewBag.ServiceTypeList = new SelectList(db.ServiceTypes.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");

            if (id > 0)
            {
                Product aProduct = db.Products.FirstOrDefault(a => a.ProductId == id);
                ProductModelView model = new ProductModelView();
                model.ProductId = aProduct.ProductId;
                model.SubMasterId = aProduct.SubMasterId;
                model.QuantityOrName = aProduct.QuantityOrName;
                model.MasterUnit = aProduct.MasterUnit;
                model.ProductName = aProduct.ProductName;
                //model.ProductCategoryId = aProduct.ProductCategoryId;
                //model.SubCategoryId = aProduct.SubCategoryId;
                model.Code = aProduct.Code;
                model.BarCode = aProduct.BarCode;
                model.PLU = aProduct.PLU;
                model.Unit = aProduct.Unit;
                model.Price = aProduct.Price;
                model.Cost = aProduct.Cost;
                model.MinimalQuantity = aProduct.MinimalQuantity;
                model.VatRateId = aProduct.VatRateId;
                model.IsVatIncluded = aProduct.IsVatIncluded;
                model.IsPriceChangeAllow = aProduct.IsPriceChangeAllow;
                model.IsDiscountAllow = aProduct.IsDiscountAllow;
                model.IsRefundAllow = aProduct.IsRefundAllow;
                model.Isperishable = aProduct.Isperishable;
                model.ExpireDays = aProduct.ExpireDays;
                model.IsFixed = aProduct.IsFixed;
                model.IsAfterSaleService = aProduct.IsAfterSaleService;
                model.ServiceTypeId = aProduct.ServiceTypeId;
                model.ServiceDays = aProduct.ServiceDays;
                model.IsUsingDefaultQuantity = aProduct.IsUsingDefaultQuantity;
                model.IsUnitWise = aProduct.IsUnitWise;
                model.RestrictionId = aProduct.RestrictionId;
                model.IsPointBased = aProduct.IsPointBased;
                model.Points = aProduct.Points;
                model.IsUniqueItem = aProduct.IsUniqueItem;

                model.IsDynamic = aProduct.IsDynamic;
                model.SizeCheckBox = aProduct.SizeCheckBox;
                model.ColorCheckbox = aProduct.ColorCheckBox;
                model.PriceCheckBox = aProduct.PriceCheckBox;
                model.SizeType = aProduct.SizeType;
                model.CostCheckBox = aProduct.CostCheckBox;
                model.CodeCheckbox = aProduct.CodeCheckbox;
                model.PluCheckbox = aProduct.PluCheckbox;
                model.MinimumQuantityCheckbox = aProduct.MinimumQuantityCheckbox;

                model.Description = aProduct.Description;
                model.Color = aProduct.Color;
                model.Image = aProduct.Image;

                if (model.IsDynamic == true)
                {
                    ProductDistributeItems item;
                    List<ProductDistributeItems> items = new List<ProductDistributeItems>();
                    var distributeItems = db.ProductDistributes.Where(a => a.ProductId == id && a.Status == true).ToList();
                    if (distributeItems != null)
                    {
                        foreach (var distItem in distributeItems)
                        {
                            item = new ProductDistributeItems();
                            item.Id = distItem.Id;
                            item.SizeId = distItem.SizeId;
                            item.ColorId = distItem.ColorId;
                            item.Price = distItem.Price;
                            item.BarCode = distItem.BarCode;
                            item.Cost = distItem.Cost;
                            item.Code = distItem.Code;
                            item.Plu = distItem.PLU;
                            item.MQuantity = distItem.MinimumQuantity;
                            items.Add(item);
                        }
                    }
                    model.DistributeItems = items;
                }
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult GetSize(int sizeType)
        {
            var list = new SelectList("");
            if (sizeType == 1) // size letter
            {
                list = new SelectList(db.Sizes.Where(a => a.Type == true).Select(s => new { s.Id, s.SizeName }), "Id", "SizeName");
            }
            else if (sizeType == 2) //size number
            {
                list = new SelectList(db.Sizes.Where(a => a.Type == false).Select(s => new { s.Id, s.SizeName }), "Id", "SizeName");
            }
            else if (sizeType == 3)
            {
                list = new SelectList(db.Units.Where(s => s.Status == 1).Select(s => new { Id = s.UnitId, SizeName = s.UnitName }), "Id", "SizeName");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetColor()
        {
            var list = new SelectList(db.Colors.Select(s => new { s.Id, s.ColorName }), "Id", "ColorName");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ProductEdit(int id)
        {
            ViewBag.ProductId = id;
            return PartialView();
        }
        //product save
        public JsonResult ProductCreateSave(ProductModelView model)
        {
            string filefullName = ""; //image file name
            int id = 0;
            List<int> ids = new List<int>();
            Product aProduct = new Product();
            ProSalePrice aProPrice = new ProSalePrice();
            try
            {
                if (Request.Files.Count > 0) //save image
                {
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        if (fileContent != null && fileContent.ContentLength > 0)
                        {
                            filefullName = model.ProductName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssf").ToString() + "_" + model.Image;
                            var path = Path.Combine(Server.MapPath("~/Images/ProductImage"), filefullName);
                            var resizeImg = Image.FromStream(fileContent.InputStream, true, true);

                            using (var productImg = ScaleImage(resizeImg, 64, 64))
                            {
                                productImg.Save(path);
                            }
                        }
                    }
                }
                if (model.ProductId > 0) // update product
                {
                    aProduct = db.Products.FirstOrDefault(a => a.ProductId == model.ProductId);
                    aProduct.ProductName = model.ProductName;
                    aProduct.SubMasterId = model.SubMasterId;
                    if (model.SubMasterId > 0)
                    {
                        aProduct.QuantityOrName = model.QuantityOrName;
                        aProduct.MasterUnit = model.MasterUnit;

                        bool isNumeric = int.TryParse(aProduct.QuantityOrName, out int n);

                        if (isNumeric)
                        {
                            aProduct.IsQuantity = true;
                        }
                        else
                        {
                            aProduct.IsQuantity = false;
                        }
                    }
                    aProduct.Code = model.Code;
                    //aProduct.ProductCategoryId = model.ProductCategoryId;
                    //aProduct.SubCategoryId = model.SubCategoryId;
                    aProduct.BarCode = model.BarCode;
                    aProduct.PLU = model.PLU;
                    aProduct.Unit = model.Unit;
                    aProduct.Cost = model.Cost;
                    aProduct.Price = model.Price;
                    aProduct.MinimalQuantity = model.MinimalQuantity;
                    aProduct.VatRateId = model.VatRateId;
                    aProduct.IsVatIncluded = model.IsVatIncluded;
                    aProduct.IsPriceChangeAllow = model.IsPriceChangeAllow;
                    aProduct.IsDiscountAllow = model.IsDiscountAllow;
                    aProduct.IsRefundAllow = model.IsRefundAllow;
                    aProduct.Isperishable = model.Isperishable;
                    aProduct.ExpireDays = model.ExpireDays;
                    aProduct.IsFixed = model.IsFixed;
                    aProduct.IsAfterSaleService = model.IsAfterSaleService;
                    aProduct.ServiceTypeId = model.ServiceTypeId;
                    aProduct.ServiceDays = model.ServiceDays;
                    aProduct.IsUsingDefaultQuantity = model.IsUsingDefaultQuantity;
                    aProduct.IsUnitWise = model.IsUnitWise;
                    aProduct.RestrictionId = model.RestrictionId;
                    aProduct.IsPointBased = model.IsPointBased;
                    aProduct.Points = model.Points;
                    aProduct.IsUniqueItem = model.IsUniqueItem;
                    aProduct.IsDynamic = model.IsDynamic;
                    aProduct.SizeCheckBox = model.SizeCheckBox;
                    aProduct.ColorCheckBox = model.ColorCheckbox;
                    aProduct.PriceCheckBox = model.PriceCheckBox;
                    aProduct.CostCheckBox = model.CostCheckBox;
                    aProduct.CodeCheckbox = model.CodeCheckbox;
                    aProduct.PluCheckbox = model.PluCheckbox;
                    aProduct.MinimumQuantityCheckbox = model.MinimumQuantityCheckbox;
                    aProduct.SizeType = model.SizeType;
                    aProduct.Description = model.Description;
                    aProduct.UpdatedBy = model.CreatedBy;
                    aProduct.DateUpdated = DateTime.Now;
                    aProduct.Color = model.Color;
                    if (!string.IsNullOrEmpty(filefullName))
                    {
                        aProduct.Image = filefullName;
                    }
                    db.Entry(aProduct).State = EntityState.Modified;
                    if (aProduct.IsDynamic == false && aProduct.PriceCheckBox == false)
                    {
                        aProPrice = db.ProSalePrices.Where(a => a.ProductId == model.ProductId).OrderByDescending(a => a.Id).FirstOrDefault();
                        if (aProPrice != null)
                        {
                            if (aProPrice.Price != model.Price)
                            {
                                aProPrice = new ProSalePrice();
                                aProPrice.ProductId = (int)model.ProductId;
                                aProPrice.Price = model.Price;
                                aProPrice.CreatedDate = DateTime.Now;
                                aProPrice.CreatedBy = model.CreatedBy;
                                db.ProSalePrices.Add(aProPrice);
                            }
                        }
                        else
                        {
                            aProPrice = new ProSalePrice();
                            aProPrice.ProductId = (int)model.ProductId;
                            aProPrice.Price = model.Price;
                            aProPrice.CreatedDate = DateTime.Now;
                            aProPrice.CreatedBy = model.CreatedBy;
                            db.ProSalePrices.Add(aProPrice);
                        }
                    }
                    db.SaveChanges();
                }
                else //create product
                {
                    aProduct.SubMasterId = model.SubMasterId;
                    if (model.SubMasterId > 0)
                    {
                        aProduct.QuantityOrName = model.QuantityOrName;
                        aProduct.MasterUnit = model.MasterUnit;
                        bool isNumeric = int.TryParse(aProduct.QuantityOrName, out int n);
                        if (isNumeric)
                        {
                            aProduct.IsQuantity = true;
                        }
                        else
                        {
                            aProduct.IsQuantity = false;
                        }
                    }
                    aProduct.ProductName = model.ProductName;
                    aProduct.Code = model.Code;
                    //aProduct.ProductCategoryId = model.ProductCategoryId;
                    //aProduct.SubCategoryId = model.SubCategoryId;
                    aProduct.BarCode = model.BarCode;
                    aProduct.PLU = model.PLU;
                    aProduct.Unit = model.Unit;
                    aProduct.Price = model.Price;
                    aProduct.Cost = model.Cost;
                    aProduct.MinimalQuantity = model.MinimalQuantity;
                    aProduct.VatRateId = model.VatRateId;
                    aProduct.IsVatIncluded = model.IsVatIncluded;
                    aProduct.IsPriceChangeAllow = model.IsPriceChangeAllow;
                    aProduct.IsDiscountAllow = model.IsDiscountAllow;
                    aProduct.IsRefundAllow = model.IsRefundAllow;
                    aProduct.Isperishable = model.Isperishable;
                    aProduct.ExpireDays = model.ExpireDays;
                    aProduct.IsFixed = model.IsFixed;
                    aProduct.IsAfterSaleService = model.IsAfterSaleService;
                    aProduct.ServiceTypeId = model.ServiceTypeId;
                    aProduct.ServiceDays = model.ServiceDays;
                    aProduct.IsUsingDefaultQuantity = model.IsUsingDefaultQuantity;
                    aProduct.IsUnitWise = model.IsUnitWise;
                    aProduct.RestrictionId = model.RestrictionId;
                    aProduct.IsPointBased = model.IsPointBased;
                    aProduct.Points = model.Points;
                    aProduct.IsUniqueItem = model.IsUniqueItem;
                    aProduct.IsDynamic = model.IsDynamic;
                    aProduct.SizeCheckBox = model.SizeCheckBox;
                    aProduct.ColorCheckBox = model.ColorCheckbox;
                    aProduct.PriceCheckBox = model.PriceCheckBox;
                    aProduct.CostCheckBox = model.CostCheckBox;
                    aProduct.CodeCheckbox = model.CodeCheckbox;
                    aProduct.PluCheckbox = model.PluCheckbox;
                    aProduct.MinimumQuantityCheckbox = model.MinimumQuantityCheckbox;
                    aProduct.SizeType = model.SizeType;
                    aProduct.Description = model.Description;
                    aProduct.DateTime = DateTime.Now;
                    aProduct.Color = model.Color;
                    aProduct.Image = "missingImg.png";
                    if (!string.IsNullOrEmpty(filefullName))
                    {
                        aProduct.Image = filefullName;
                    }
                    aProduct.Status = true;
                    aProduct.CreatedBy = model.CreatedBy;
                    db.Products.Add(aProduct);
                    db.SaveChanges();

                    if (aProduct.IsDynamic == false && aProduct.PriceCheckBox == false)
                    {
                        //Product sale price save
                        aProPrice = new ProSalePrice();
                        aProPrice.ProductId = aProduct.ProductId;
                        aProPrice.Price = model.Price;
                        aProPrice.CreatedDate = DateTime.Now;
                        aProPrice.CreatedBy = model.CreatedBy;
                        db.ProSalePrices.Add(aProPrice);
                        db.SaveChanges();
                    }
                }
                //selected tag save
                ids = new List<int>();
                TagItem tagItem;
                if (!string.IsNullOrEmpty(model.TagIds))
                {
                    foreach (var tagid in model.TagIds.Split(','))
                    {
                        id = Convert.ToInt32(tagid);
                        ids.Add(id);
                        if (db.TagItems.Any(a => a.TagId == id && a.Type == 1 && a.OwnerId == aProduct.ProductId))
                        {
                            tagItem = db.TagItems.FirstOrDefault(a => a.TagId == id && a.Type == 1 && a.OwnerId == aProduct.ProductId);
                            tagItem.Status = true;
                            tagItem.UpdatedBy = model.CreatedBy;
                            tagItem.UpdatedDate = now.Date;
                            db.Entry(tagItem).State = EntityState.Modified;
                        }
                        else
                        {
                            tagItem = new TagItem();
                            tagItem.TagId = id;
                            tagItem.OwnerId = aProduct.ProductId;
                            tagItem.Type = 1;
                            tagItem.Status = true;
                            tagItem.CreatedBy = (int)model.CreatedBy;
                            tagItem.CreatedDate = now.Date;
                            db.TagItems.Add(tagItem);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.ProductId > 0)
                {
                    //product tag inactive
                    var assignTagIds = db.TagItems.Where(a => a.OwnerId == aProduct.ProductId && a.Type == 1 && a.Status == true).Select(s => new { s.TagId }).ToList();
                    if (assignTagIds.Any())
                    {
                        foreach (var tag in assignTagIds)
                        {
                            if (ids.Any(a => a.Equals(tag.TagId)) == false)
                            {
                                tagItem = db.TagItems.FirstOrDefault(a => a.TagId == tag.TagId && a.Type == 1 && a.OwnerId == aProduct.ProductId);
                                tagItem.Status = false;
                                db.Entry(tagItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                //Product Category save 
                ProductCategory productCategory;
                if (!string.IsNullOrEmpty(model.CategoryIds))
                {
                    ids.Clear();
                    foreach (var category in model.CategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(category);
                        ids.Add(id);
                        if (db.ProductCategories.Any(a => a.ProductId == aProduct.ProductId && a.CategoryId == id))
                        {
                            productCategory = db.ProductCategories.FirstOrDefault(a => a.ProductId == aProduct.ProductId && a.CategoryId == id);
                            productCategory.Status = true;
                            productCategory.UpdatedBy = model.CreatedBy;
                            productCategory.UpdatedDate = now.Date;
                            db.Entry(productCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            productCategory = new ProductCategory();
                            productCategory.ProductId = aProduct.ProductId;
                            productCategory.CategoryId = id;
                            productCategory.Status = true;
                            productCategory.CreatedBy = (int)model.CreatedBy;
                            productCategory.CreatedDate = now.Date;
                            db.ProductCategories.Add(productCategory);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.ProductId > 0)
                {
                    //product category inactive
                    var assignCategoryIds = db.ProductCategories.Where(a => a.ProductId == aProduct.ProductId && a.Status == true).Select(s => new { s.CategoryId }).ToList();
                    if (assignCategoryIds.Any())
                    {
                        foreach (var category in assignCategoryIds)
                        {
                            if (ids.Any(a => a.Equals(category.CategoryId)) == false)
                            {
                                productCategory = db.ProductCategories.FirstOrDefault(a => a.CategoryId == category.CategoryId && a.ProductId == aProduct.ProductId);
                                productCategory.Status = false;
                                db.Entry(productCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //Product sub category save
                ProductSubCategory proSubCategory;
                SubCategory subCategory;
                if (!string.IsNullOrEmpty(model.SubCategoryIds))
                {
                    ids.Clear();
                    foreach (var subId in model.SubCategoryIds.Split(','))
                    {
                        id = Convert.ToInt32(subId);
                        ids.Add(id);
                        subCategory = db.SubCategories.Find(id);
                        productCategory = db.ProductCategories.FirstOrDefault(a => a.CategoryId == subCategory.CategoryId && a.ProductId == aProduct.ProductId);
                        if (productCategory != null)
                        {
                            if (db.ProductSubCategories.Any(a => a.ParentId == productCategory.Id && a.SubCategoryId == id))
                            {
                                proSubCategory = db.ProductSubCategories.FirstOrDefault(a => a.ParentId == productCategory.Id && a.SubCategoryId == id);
                                proSubCategory.Status = true;
                                proSubCategory.UpdatedBy = model.CreatedBy;
                                proSubCategory.UpdatedDate = now.Date;
                                db.Entry(proSubCategory).State = EntityState.Modified;
                            }
                            else
                            {
                                proSubCategory = new ProductSubCategory();
                                proSubCategory.ParentId = productCategory.Id;
                                proSubCategory.SubCategoryId = id;
                                proSubCategory.Status = true;
                                proSubCategory.CreatedBy = (int)model.CreatedBy;
                                proSubCategory.CreatedDate = now.Date;
                                db.ProductSubCategories.Add(proSubCategory);
                            }
                            db.SaveChanges();
                        }
                    }
                }
                if (model.ProductId > 0)
                {
                    //Product sub category inactive
                    var assignSubCategorys = db.ProductCategories.Where(a => a.ProductId == aProduct.ProductId).Join(db.ProductSubCategories, c => c.Id, sc => sc.ParentId, (c, sc) => new { sc.Id, sc.SubCategoryId }).ToList();
                    if (assignSubCategorys.Any())
                    {
                        foreach (var sub in assignSubCategorys)
                        {
                            if (ids.Any(a => a.Equals(sub.SubCategoryId)) == false)
                            {
                                proSubCategory = db.ProductSubCategories.Find(sub.Id);
                                proSubCategory.Status = false;
                                db.Entry(proSubCategory).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //product distribute item save 
                if (model.IsDynamic == true)
                {
                    ProductDistribute distribute;
                    if (model.DistributeItems != null)
                    {
                        foreach (var item in model.DistributeItems)
                        {
                            if (item.Id > 0)
                            {
                                distribute = db.ProductDistributes.Find(item.Id);
                                distribute.SizeId = item.SizeId;
                                distribute.ColorId = item.ColorId;
                                distribute.Price = item.Price;
                                distribute.BarCode = item.BarCode;
                                distribute.Cost = item.Cost;
                                distribute.SizeName = item.SizeName;
                                distribute.SizeType = model.SizeType;
                                distribute.Code = item.Code;
                                distribute.PLU = item.Plu;
                                distribute.MinimumQuantity = item.MQuantity;
                                db.Entry(distribute).State = EntityState.Modified;
                            }
                            else
                            {
                                distribute = new ProductDistribute();
                                distribute.ProductId = aProduct.ProductId;
                                distribute.SizeId = item.SizeId;
                                distribute.ColorId = item.ColorId;
                                distribute.Price = item.Price;
                                distribute.BarCode = item.BarCode;
                                distribute.Cost = item.Cost;
                                distribute.SizeName = item.SizeName;
                                distribute.SizeType = model.SizeType;
                                distribute.Code = item.Code;
                                distribute.PLU = item.Plu;
                                distribute.MinimumQuantity = item.MQuantity;
                                distribute.Status = true;
                                db.ProductDistributes.Add(distribute);
                            }
                            db.SaveChanges();

                            if (aProduct.PriceCheckBox == true)
                            {
                                aProPrice = db.ProSalePrices.Where(a => a.ProductId == distribute.ProductId && a.DistributeId == distribute.Id).OrderByDescending(a => a.Id).FirstOrDefault();
                                if (aProPrice != null)
                                {
                                    if (aProPrice.Price != model.Price)
                                    {
                                        aProPrice = new ProSalePrice();
                                        aProPrice.ProductId = distribute.ProductId;
                                        aProPrice.DistributeId = distribute.Id;
                                        aProPrice.Price = model.Price;
                                        aProPrice.CreatedDate = DateTime.Now;
                                        aProPrice.CreatedBy = model.CreatedBy;
                                        db.ProSalePrices.Add(aProPrice);
                                    }
                                }
                                else
                                {
                                    aProPrice = new ProSalePrice();
                                    aProPrice.ProductId = distribute.ProductId;
                                    aProPrice.DistributeId = distribute.Id;
                                    aProPrice.Price = model.Price;
                                    aProPrice.CreatedDate = DateTime.Now;
                                    aProPrice.CreatedBy = model.CreatedBy;
                                    db.ProSalePrices.Add(aProPrice);
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //if product distribute item cancel
                if (!string.IsNullOrEmpty(model.CancelDistributeIds))
                {
                    ProductDistribute distribute;
                    long proDistId = 0;

                    foreach (var distId in model.CancelDistributeIds.Split(','))
                    {
                        proDistId = Convert.ToInt64(distId);
                        distribute = db.ProductDistributes.Find(proDistId);
                        distribute.Status = false;
                        db.Entry(distribute).State = EntityState.Modified;
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
        //************Product Create end**************************

        //***************product list *********************
        public PartialViewResult ProductList(string productIds, int? categoryId)
        {
            List<ViewMainProduct> productList = new List<ViewMainProduct>();
            ViewMainProduct product = new ViewMainProduct();
            if (!string.IsNullOrEmpty(productIds))
            {
                int productId = 0;
                foreach (var id in productIds.Split(','))
                {
                    productId = Convert.ToInt32(id);
                    product = db.ViewMainProducts.FirstOrDefault(a => a.ProductId == productId);
                    if (product != null)
                    {
                        productList.Add(product);
                    }
                }
            }
            else if (categoryId > 0)
            {
                var productIdList = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.ProductId).Select(s => s.FirstOrDefault().ProductId).ToList();
                if (productIdList.Any())
                {
                    foreach (var id in productIdList)
                    {
                        product = db.ViewMainProducts.FirstOrDefault(a => a.ProductId == id);
                        if (product != null)
                        {
                            productList.Add(product);
                        }
                    }
                }
            }
            else
            {
                productList = db.ViewMainProducts.ToList();
            }
            return PartialView(productList.OrderBy(a => a.ProductName).ToList());
        }
        //public PartialViewResult ProductInfo(int? productId)
        //{
        //    var product = db.ViewProducts.FirstOrDefault(a => a.ProductId == productId);
        //    return PartialView(product);
        //}
        //Product Price Change 
        public PartialViewResult ProductPriceChange(int productId, long? distributeId)
        {
            ViewBag.ProductId = productId;
            ViewBag.DistributeId = distributeId;
            return PartialView();
        }
        public PartialViewResult ProductPriceChangePartial(int productId, long? distributeId)
        {
            ProductModelView model = new ProductModelView();
            if (distributeId > 0)
            {
                var product = db.ViewProducts.FirstOrDefault(a => a.ProductId == productId && a.DistributeId == distributeId);
                if (product != null)
                {
                    model.Price = (decimal)product.Price;
                }
            }
            else
            {
                var product = db.Products.FirstOrDefault(a => a.ProductId == productId);
                model.Price = product.Price;
            }
            return PartialView(model);
        }
        public JsonResult PriceUpdate(int ProductId, long? distributeId, decimal Price)
        {
            try
            {
                var product = db.Products.FirstOrDefault(a => a.ProductId == ProductId);
                if (product.IsDynamic == true && product.PriceCheckBox == true)
                {
                    var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == ProductId && a.Id == distributeId);
                    subProduct.Price = Price;
                }
                else
                {
                    product.Price = Price;
                    db.Entry(product).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduct(int? categoryId, bool? IsAvailable)
        {
            if (IsAvailable == true) // stock available 
            {
                var productList = new SelectList(db.ViewStockProducts.Where(a => a.Quantity > 0).OrderBy(o => o.ProductName).Select(a => new { a.RowNumber, a.ProductName }), "RowNumber", "ProductName");
                return Json(productList, JsonRequestBehavior.AllowGet);
            }
            else if (IsAvailable == false) //stock inventory
            {
                var productList = new SelectList(db.ViewProducts.Where(a => a.Status == true).Select(a => new { a.RowID, a.ProductName }), "RowID", "ProductName");
                return Json(productList, JsonRequestBehavior.AllowGet);
            }
            var alist = new SelectList("");
            var products = new List<Product>();
            if (categoryId > 0)
            {
                var productIdList = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.ProductId).Select(s => s.FirstOrDefault().ProductId).ToList();
                if (productIdList.Any())
                {
                    foreach (var id in productIdList)
                    {
                        var product = db.Products.FirstOrDefault(a => a.ProductId == id);
                        if (product != null)
                        {
                            products.Add(product);
                        }
                    }
                }
            }
            else
            {
                products = db.Products.Where(a => a.Status == true).ToList();
            }
            alist = new SelectList(products.OrderBy(o => o.ProductName).Select(a => new { a.ProductName, a.ProductId }), "ProductId", "ProductName");

            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        //delete product
        public JsonResult DeleteProduct(int? productId)
        {
            Product aProduct = db.Products.FirstOrDefault(a => a.ProductId == productId);
            db.Entry(aProduct).State = EntityState.Deleted;
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
        //product active/inactive
        public JsonResult ChangeProductStatus(int id, int? status)
        {
            Product aProduct = db.Products.Find(id);
            if (status == 1)
            {
                aProduct.Status = true;
            }
            else
            {
                aProduct.Status = false;
            }
            db.Entry(aProduct).State = EntityState.Modified;
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
        public PartialViewResult MinimumQuantity(int productId, long? distributeId)
        {
            ViewBag.ProductId = productId;
            ViewBag.DistributeId = distributeId;
            return PartialView();
        }
        public PartialViewResult MinimumQuantityPartial(int productId, long? distributeId)
        {
            ProductModelView model = new ProductModelView();
            if (distributeId > 0)
            {
                var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == productId && a.Id == distributeId);
                if (subProduct != null)
                {
                    model.MinimalQuantity = subProduct.MinimumQuantity;
                }
            }
            else
            {
                var product = db.Products.FirstOrDefault(a => a.ProductId == productId);
                model.MinimalQuantity = product.MinimalQuantity;
            }
            return PartialView(model);
        }
        public JsonResult MinimalUpdate(int productId, long? distributeId, int minimumQuantity)
        {
            try
            {
                if (distributeId > 0)
                {
                    var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == productId && a.Id == distributeId);
                    subProduct.MinimumQuantity = minimumQuantity;
                    db.Entry(subProduct).State = EntityState.Modified;
                }
                else
                {
                    var product = db.Products.FirstOrDefault(a => a.ProductId == productId);
                    product.MinimalQuantity = minimumQuantity;
                    db.Entry(product).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //Image resize
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        #region Product Info
        [EncryptedActionParameter]
        public ActionResult ProductInfo(int productId)
        {
            return View(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId));
        }
        public PartialViewResult ImportProductList(int productId)
        {
            return PartialView(db.ViewImportTransactions.Where(a => a.ProductId == productId).OrderByDescending(a => a.Date).ToList());
        }
        public PartialViewResult SaleProductList(int productId)
        {
            return PartialView(db.ViewOrderTransactions.Where(a => a.ProductId == productId && a.Status == true).OrderByDescending(a => a.OrderDate).ToList());
        }
        #endregion

        #endregion

        #region Master Product
        [Route("Configuration/MasterProduct")]
        public ActionResult MasterProduct()
        {
            return View();
        }

        public PartialViewResult MasterProductList(int? status)
        {
            var list = new List<ViewMasterProduct>();
            if (status == 1)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == true).ToList();
            }
            else if (status == 0)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == false).ToList();
            }
            else if (status == 2)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewMasterProducts.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public PartialViewResult MasterProductCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult MasterProductCreatePartial(int? id)
        {
            MasterProductModelView model = new MasterProductModelView();
            ViewBag.BrandList = new SelectList(db.Brands.Where(a => a.Status == true).Select(s => new { s.BrandId, s.BrandName }), "BrandId", "BrandName");
            ViewBag.ProductList = new SelectList(db.Products.Where(a => a.Status == true).Select(s => new { s.ProductId, s.ProductName }), "ProductId", "ProductName");
            ViewBag.UnitList = new SelectList(db.Units.Where(s => s.Status == 1), "UnitId", "UnitName");

            if (id > 0)
            {
                var masterProduct = db.ViewMasterProducts.FirstOrDefault(a => a.Id == id);
                model.Id = masterProduct.Id;
                model.Brand = masterProduct.BrandName;
                model.Name = masterProduct.Name;
            }
            return PartialView(model);
        }
        public JsonResult MasterProductSave(MasterProductModelView model)
        {
            try
            {
                var brand = db.Brands.FirstOrDefault(a => a.BrandName.ToLower() == model.Brand.ToLower());
                if (brand != null)
                {
                    model.BrandId = brand.BrandId;
                }
                else
                {
                    Brand abrand = new Brand();
                    abrand.BrandName = model.Brand;
                    abrand.CreatedBy = model.CreatedBy;
                    abrand.CreatedDate = now.Date;
                    abrand.Status = true;
                    db.Brands.Add(abrand);
                    db.SaveChanges();
                    model.BrandId = abrand.BrandId;
                }
                MasterProduct subProduct;
                if (model.Id > 0)
                {
                    subProduct = db.MasterProducts.Find(model.Id);
                    subProduct.Name = model.Name;
                    subProduct.BrandId = model.BrandId;
                    subProduct.UpdatedBy = model.CreatedBy;
                    subProduct.UpdatedDate = now.Date;
                    db.Entry(subProduct).State = EntityState.Modified;
                }
                else
                {
                    subProduct = new MasterProduct();
                    subProduct.Name = model.Name;
                    subProduct.BrandId = model.BrandId;
                    subProduct.Status = true;
                    subProduct.CreatedBy = model.CreatedBy;
                    subProduct.CreatedDate = now.Date;
                    db.MasterProducts.Add(subProduct);
                }
                db.SaveChanges();
                if (model.ProductId > 0)
                {
                    var product = db.Products.Find(model.ProductId);
                    product.SubMasterId = subProduct.Id;
                    product.QuantityOrName = model.QuantityOrName;
                    product.MasterUnit = model.MasterUnit;
                    bool isNumeric = int.TryParse(product.QuantityOrName, out int n);
                    if (isNumeric)
                    {
                        product.IsQuantity = true;
                    }
                    else
                    {
                        product.IsQuantity = false;
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //Master product active/inactive/delete
        public JsonResult ChangeMasterProductStatus(int id, int? status)
        {
            var masterProduct = db.MasterProducts.Find(id);
            if (status == 1)
            {
                masterProduct.Status = true; //active
            }
            else if (status == 0)
            {
                masterProduct.Status = false; //Inactive
            }
            else
            {
                masterProduct.Status = null; // delete
            }
            db.Entry(masterProduct).State = EntityState.Modified;
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
        public PartialViewResult SubProduct(long masterId)
        {
            ViewBag.MasterId = masterId;
            return PartialView();
        }
        public PartialViewResult SubProductList(long masterId)
        {
            return PartialView(db.ViewProducts.Where(a => a.SubMasterId == masterId && a.Status == true).ToList());
        }
        #endregion

        #region Master Product Tag
        [EncryptedActionParameter]
        [Route("Configuration/MasterProduct/MasterProductTag")]
        public ActionResult MasterProductTag(int? id)
        {
            return View(db.ViewMasterProducts.FirstOrDefault(a => a.Id == id));
        }
        public JsonResult GetTagById(int tagId)
        {
            var tag = db.Tags.Find(tagId);
            return Json(tag, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult MasterTagList(int id, bool? isView)
        {
            ViewBag.IsView = false;
            if (isView == true)
            {
                ViewBag.IsView = true;
            }
            return PartialView(db.ViewTagItems.Where(a => a.OwnerId == id && a.Type == 2 && a.Status == true).ToList());
        }
        public JsonResult MasterTagSave(IList<TagItem> items, long[] deleteItemIds, int CreatedBy)
        {
            try
            {
                TagItem tagItem;
                //Tag item save
                foreach (TagItem item in items)
                {
                    if (item.Id > 0)
                    {
                        tagItem = db.TagItems.Find(item.Id);
                        tagItem.UpdatedBy = CreatedBy;
                        tagItem.UpdatedDate = now.Date;
                        db.Entry(tagItem).State = EntityState.Modified;
                    }
                    else
                    {
                        item.Type = 2; // type 2 for Master Product Tag
                        item.Status = true;
                        item.CreatedBy = CreatedBy;
                        item.CreatedDate = now.Date;
                        db.TagItems.Add(item);
                    }
                    db.SaveChanges();
                }
                // tag item delete
                if (deleteItemIds != null)
                {
                    foreach (var id in deleteItemIds)
                    {
                        tagItem = db.TagItems.Find(id);
                        tagItem.Status = null;
                        db.Entry(tagItem).State = EntityState.Modified;
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
        public PartialViewResult MasterTag(int masterId)
        {
            ViewBag.Id = masterId;
            return PartialView();
        }
        #endregion

        #region Master Product Category
        [EncryptedActionParameter]
        [Route("Configuration/MasterProduct/MasterProductCategory")]
        public ActionResult MasterProductCategory(int id)
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View(db.ViewMasterProducts.FirstOrDefault(a => a.Id == id));
        }
        public PartialViewResult MasterSubCategoryList(int id, bool? isView)
        {
            ViewBag.IsView = false;
            if (isView == true)
            {
                ViewBag.IsView = true;
            }
            return PartialView(db.ViewMasterProductCategories.Where(a => a.MasterProductId == id && a.MasterCategoryStatus == true && a.MasterSubCategoryStatus == true).ToList());
        }
        public JsonResult MasterProductCategorySave(IList<MasterProductSubCategoryModelView> subCategorys, string categoryIds, int masterProductId, int CreatedBy, long[] deleteSubCategoryIds)
        {
            try
            {
                int categoryId = 0;
                MasterProductCategory masterCategory;
                MasterProductSubCategory masterSubCategory;
                List<int> categoryIdList = new List<int>();
                if (!string.IsNullOrEmpty(categoryIds))
                {
                    foreach (var id in categoryIds.Split(','))
                    {
                        //add category
                        categoryId = Convert.ToInt32(id);
                        categoryIdList.Add(categoryId);
                        if (db.MasterProductCategories.Any(a => a.MasterProductId == masterProductId && a.CategoryId == categoryId))
                        {
                            masterCategory = db.MasterProductCategories.FirstOrDefault(a => a.MasterProductId == masterProductId && a.CategoryId == categoryId);
                            masterCategory.Status = true;
                            masterCategory.UpdatedBy = CreatedBy;
                            masterCategory.UpdatedDate = now.Date;
                            db.Entry(masterCategory).State = EntityState.Modified;
                        }
                        else
                        {
                            masterCategory = new MasterProductCategory();
                            masterCategory.MasterProductId = masterProductId;
                            masterCategory.CategoryId = categoryId;
                            masterCategory.Status = true;
                            masterCategory.CreatedBy = CreatedBy;
                            masterCategory.CreatedDate = now.Date;
                            db.MasterProductCategories.Add(masterCategory);
                        }
                        db.SaveChanges();
                        //add subcategory
                        foreach (var subCategory in subCategorys.Where(a => a.CategoryId == categoryId))
                        {
                            if (db.ViewMasterProductCategories.Any(a => a.MasterProductId == masterProductId && a.CategoryId == categoryId && a.SubCategoryId == subCategory.SubCategoryId))
                            {
                                masterSubCategory = db.MasterProductSubCategories.Find(db.ViewMasterProductCategories.FirstOrDefault(a => a.MasterProductId == masterProductId && a.CategoryId == categoryId && a.SubCategoryId == subCategory.SubCategoryId).MasterSubCategoryId);
                                masterSubCategory.Status = true;
                                masterSubCategory.UpdatedBy = CreatedBy;
                                masterSubCategory.UpdatedDate = now.Date;
                                db.Entry(masterSubCategory).State = EntityState.Modified;
                            }
                            else
                            {
                                masterSubCategory = new MasterProductSubCategory();
                                masterSubCategory.ParentId = masterCategory.Id;
                                masterSubCategory.SubCategoryId = subCategory.SubCategoryId;
                                masterSubCategory.Status = true;
                                masterSubCategory.CreatedBy = CreatedBy;
                                masterSubCategory.CreatedDate = now.Date;
                                db.MasterProductSubCategories.Add(masterSubCategory);
                            }
                            db.SaveChanges();
                        }
                    }
                }
                // subcategory delete
                if (deleteSubCategoryIds != null)
                {
                    foreach (var id in deleteSubCategoryIds)
                    {
                        masterSubCategory = db.MasterProductSubCategories.Find(id);
                        masterSubCategory.Status = false;
                        db.Entry(masterSubCategory).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                //category delete
                var masterCategoryList = db.MasterProductCategories.Where(a => a.MasterProductId == masterProductId).ToList();
                if (masterCategoryList.Any())
                {
                    foreach (var category in masterCategoryList)
                    {
                        if (categoryIdList.Any(a => a.Equals(category.CategoryId)) == false)
                        {
                            category.Status = false;
                            db.Entry(category).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult MasterCategory(int masterId)
        {
            ViewBag.MasterId = masterId;
            return PartialView();
        }
        #endregion

        #region Restriction
        [Route("Configuration/Restriction")]
        public ActionResult Restriction()
        {
            return View();
        }
        public PartialViewResult RestrictionCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult RestrictionCreatePartial(int? id)
        {
            if (id > 0)
            {
                var restriction = db.Restrictions.Find(id);
                RestrictionModelView model = new RestrictionModelView();
                model.Id = restriction.Id;
                model.Name = restriction.Name;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult RestrictionSave(RestrictionModelView model)
        {
            try
            {
                Restriction restriction;
                if (model.Id > 0)
                {
                    restriction = db.Restrictions.Find(model.Id);
                    restriction.Name = model.Name;
                    restriction.UpdatedBy = model.CreatedBy;
                    restriction.UpdatedDate = now.Date;
                    db.Entry(restriction).State = EntityState.Modified;
                }
                else
                {
                    restriction = new Restriction();
                    restriction.Name = model.Name;
                    restriction.Status = true;
                    restriction.CreatedBy = model.CreatedBy;
                    restriction.CreatedDate = now.Date;
                    db.Restrictions.Add(restriction);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult RestrictionList(int? status)
        {
            var list = new List<ViewRestriction>();
            if (status == 1) // active
            {
                list = db.ViewRestrictions.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewRestrictions.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewRestrictions.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewRestrictions.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeRestrictionStatus(int id, int status)
        {
            var restriction = db.Restrictions.Find(id);
            if (status == 1)
            {
                restriction.Status = true; //active
            }
            else if (status == 0)
            {
                restriction.Status = false; //Inactive
            }
            else if (status == 2)
            {
                restriction.Status = null; // delete
            }
            db.Entry(restriction).State = EntityState.Modified;
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

        #region Vat
        [Route("Configuration/Vat")]
        public ActionResult Vat()
        {
            return View();
        }
        //Vat Create
        public PartialViewResult VatCreate(int? vatId)
        {
            ViewBag.VatId = vatId;
            return PartialView();
        }
        public PartialViewResult VatCreatePartial(int? vatId)
        {
            if (vatId > 0)
            {
                var aVat = db.Vats.FirstOrDefault(a => a.VatId == vatId);
                VatModelView model = new VatModelView();
                model.VatId = aVat.VatId;
                model.Rate = aVat.Rate;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult VatSave(VatModelView model)
        {
            Vat aVat;
            if (model.VatId > 0) //vat update
            {
                aVat = db.Vats.FirstOrDefault(a => a.VatId == model.VatId);
                aVat.Rate = model.Rate;
                aVat.Name = model.Rate + "%";
                aVat.UpdateBy = model.CreatedBy;
                aVat.UpdateDate = DateTime.Now;
                db.Entry(aVat).State = EntityState.Modified;
            }
            else //vat create
            {
                aVat = new Vat();
                aVat.Name = model.Rate + "%";
                aVat.Rate = model.Rate;
                aVat.Status = 1;
                aVat.CreatedBy = model.CreatedBy;
                aVat.CreatedDate = DateTime.Now;
                db.Vats.Add(aVat);
            }
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
        //Vat delete
        public JsonResult DeleteVat(int? vatId)
        {
            if (vatId > 0)
            {
                var aVat = db.Vats.FirstOrDefault(a => a.VatId == vatId);
                db.Entry(aVat).State = EntityState.Deleted;
            }
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
        // vat active/inactive
        public JsonResult ChangeVatStatus(int? vatId, int? status, int? userId)
        {
            var aVat = db.Vats.FirstOrDefault(a => a.VatId == vatId);
            aVat.Status = status;
            aVat.UpdateBy = userId;
            aVat.UpdateDate = DateTime.Now;
            db.Entry(aVat).State = EntityState.Modified;
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
        // Vat Rate List
        public PartialViewResult VatList()
        {
            return PartialView(db.ViewVats.ToList());
        }
        #endregion

        #region Schedule
        [Route("Configuration/Schedule")]
        public ActionResult Schedule()
        {
            return View();
        }
        public PartialViewResult ScheduleCreate(int? id, bool? isCreateFromEvent)
        {
            ViewBag.Id = id;
            ViewBag.IsMultiSchedule = false;
            if (id > 0)
            {
                ViewBag.IsMultiSchedule = db.Schedules.Find(id).IsMultiSchedule;
            }
            ViewBag.IsCreateFromEvent = false;
            if (isCreateFromEvent == true)
            {
                ViewBag.IsCreateFromEvent = true;
            }
            return PartialView();
        }
        public PartialViewResult ScheduleCreatePartial(int? id)
        {
            if (id > 0)
            {
                var schedule = db.Schedules.Find(id);
                ScheduleModelView model = new ScheduleModelView();
                model.Id = schedule.Id;
                model.Name = schedule.Name;
                return PartialView(model);
            }
            return PartialView();
        }
        public PartialViewResult ScheduleItemList(int? id, bool IsMultiple)
        {
            ViewBag.IsMultiple = IsMultiple;
            return PartialView(db.ScheduleItems.Where(a => a.ScheduleId == id && a.Status == true).OrderBy(o => o.StartTime).ToList());
        }
        public JsonResult ScheduleSave(ScheduleModelView model)
        {
            try
            {
                Schedule schedule;
                ScheduleItem schItem;
                if (model.Id > 0)
                {
                    schedule = db.Schedules.Find(model.Id);
                    schedule.Name = model.Name;
                    schedule.UpdatedBy = model.CreatedBy;
                    schedule.UpdatedDate = now.Date;
                    db.Entry(schedule).State = EntityState.Modified;
                }
                else
                {
                    schedule = new Schedule();
                    schedule.Name = model.Name;
                    schedule.IsMultiSchedule = model.IsMultiSchedule;
                    schedule.Status = true;
                    schedule.CreatedBy = model.CreatedBy;
                    schedule.CreatedDate = now.Date;
                    db.Schedules.Add(schedule);
                }
                db.SaveChanges();
                if (model.ScheduleItems != null)
                {
                    foreach (var item in model.ScheduleItems)
                    {
                        if (item.Id > 0)
                        {
                            schItem = db.ScheduleItems.Find(item.Id);
                            schItem.Name = item.Name;
                            schItem.StartTime = item.StartTime.TimeOfDay;
                            schItem.EndTime = item.EndTime.TimeOfDay;
                            schItem.UpdatedBy = model.CreatedBy;
                            schItem.UpdatedDate = now.Date;
                            db.Entry(schItem).State = EntityState.Modified;
                        }
                        else
                        {
                            schItem = new ScheduleItem();
                            schItem.ScheduleId = schedule.Id;
                            schItem.Name = item.Name;
                            schItem.StartTime = item.StartTime.TimeOfDay;
                            schItem.EndTime = item.EndTime.TimeOfDay;
                            schItem.Status = true;
                            schItem.CreatedBy = model.CreatedBy;
                            schItem.CreatedDate = now.Date;
                            db.ScheduleItems.Add(schItem);
                        }
                        db.SaveChanges();
                    }
                }
                if (model.deleteIds != null)
                {
                    foreach (var id in model.deleteIds)
                    {
                        schItem = db.ScheduleItems.Find(id);
                        schItem.Status = false;
                        db.Entry(schItem).State = EntityState.Modified;
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
        public PartialViewResult ScheduleList(int? status)
        {
            var list = new List<ViewSchedule>();
            if (status == 1) // active
            {
                list = db.ViewSchedules.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewSchedules.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewSchedules.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewSchedules.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeScheduleStatus(int id, int status)
        {
            var schedule = db.Schedules.Find(id);
            if (status == 1)
            {
                schedule.Status = true; //active
            }
            else if (status == 0)
            {
                schedule.Status = false; //Inactive
            }
            else if (status == 2)
            {
                schedule.Status = null; // delete
            }
            db.Entry(schedule).State = EntityState.Modified;
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
        public JsonResult GetSchedule()
        {
            return Json(db.Schedules.Where(a => a.Status == true).OrderBy(o => o.Name).Select(s => new { s.Id, s.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Delivery Charge Coupon
        public ActionResult DeliveryChargeCoupon()
        {
            return View();
        }
        public PartialViewResult DChargeCouponCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult DChargeCouponCreatePartial(int? id)
        {
            if(id > 0)
            {
                var dChargeCoupon = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.Id == id);
                CouponModelView model = new CouponModelView();
                model.Id = dChargeCoupon.Id;
                model.OfferId = dChargeCoupon.OfferId;
                model.Code = dChargeCoupon.CouponCode;
                model.Percentage = dChargeCoupon.Percentage;
                model.StartDate = dChargeCoupon.StartDate;
                model.EndDate = dChargeCoupon.EndDate;
                model.IsPriceRange = dChargeCoupon.IsPriceRange;
                model.FromPrice = dChargeCoupon.FromPrice;
                model.ToPrice = dChargeCoupon.ToPrice;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult DeliveryChargeCouponSave(CouponModelView model)
        {
            try
            {
                Offer offer;
                DeliveryChargeCoupon coupon;
                if (model.Id > 0)
                {
                    coupon = db.DeliveryChargeCoupons.Find(model.Id);
                    coupon.Percentage = model.Percentage;
                    coupon.IsPriceRange = model.IsPriceRange;
                    coupon.FromPrice = model.FromPrice;
                    coupon.ToPrice = model.ToPrice;
                    db.Entry(coupon).State = EntityState.Modified;

                    offer = db.Offers.Find(coupon.OfferId);
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
                    offer.Coupon = model.Code;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {

                    offer = new Offer();
                    offer.Type = 3;
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
                    offer.Coupon = model.Code;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    offer.Status = true;
                    db.Offers.Add(offer);

                    db.SaveChanges();

                    coupon = new DeliveryChargeCoupon();
                    coupon.OfferId = offer.Id;
                    coupon.Percentage = model.Percentage;
                    coupon.IsPriceRange = model.IsPriceRange;
                    if(model.IsPriceRange == true)
                    {
                        coupon.FromPrice = model.FromPrice;
                        coupon.ToPrice = model.ToPrice;
                    }
                    db.DeliveryChargeCoupons.Add(coupon);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult DChargeCouponList()
        {
            return PartialView(db.ViewDeliveryChargeCoupons.ToList());
        }
        #endregion

        #region Amount Coupon
        public ActionResult AmountCoupon()
        {
            return View();
        }
        public PartialViewResult AmountCouponCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult AmountCouponCreatePartial(int? id)
        {
            if (id > 0)
            {
                var amountCoupon = db.ViewAmountCoupons.FirstOrDefault(a => a.Id == id);
                CouponModelView model = new CouponModelView();
                model.Id = amountCoupon.Id;
                model.OfferId = amountCoupon.OfferId;
                model.Code = amountCoupon.CouponCode;
                model.Amount = amountCoupon.Amount;
                model.IsPercentile = amountCoupon.IsPercentile;
                model.IsInfinite = amountCoupon.IsInifinte;
                model.StartDate = amountCoupon.StartDate;
                model.EndDate = amountCoupon.EndDate;
                model.FromPrice = amountCoupon.FromPrice;
                model.ToPrice = amountCoupon.ToPrice;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult AmountCouponSave(CouponModelView model)
        {
            try
            {
                Offer offer;
                AmountCoupon coupon;
                if (model.Id > 0)
                {
                    coupon = db.AmountCoupons.Find(model.Id);
                    coupon.Amount = model.Amount;
                    coupon.IsPercentile = model.IsPercentile;
                    coupon.IsInifinte = model.IsInfinite;
                    coupon.FromPrice = (int)model.FromPrice;
                    if(model.IsInfinite == true)
                    {
                        coupon.ToPrice = 0;
                    }
                    else
                    {
                        coupon.ToPrice = (int)model.ToPrice;
                    }
                    db.Entry(coupon).State = EntityState.Modified;


                    offer = db.Offers.Find(coupon.OfferId);
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
                    offer.Coupon = model.Code;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {
                    offer = new Offer();
                    offer.Type = 4;
                    if (model.IsDateValidity)
                    {
                        offer.StartDate =(DateTime) model.StartDate;
                        offer.EndDate = (DateTime) model.EndDate;
                    }
                    else
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);

                        offer.StartDate = DateTime.Now;
                        offer.EndDate = date;
                    }
                    offer.Coupon = model.Code;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    offer.Status = true;
                    db.Offers.Add(offer);

                    db.SaveChanges();

                    coupon = new AmountCoupon();
                    coupon.OfferId = offer.Id;
                    coupon.Amount = model.Amount;
                    coupon.IsPercentile = model.IsPercentile;
                    coupon.IsInifinte = model.IsInfinite;
                    coupon.FromPrice = (int)model.FromPrice;
                    if (model.IsInfinite == true)
                    {
                        coupon.ToPrice = 0;
                    }
                    else
                    {
                        coupon.ToPrice = (int)model.ToPrice;
                    }
                    db.AmountCoupons.Add(coupon);
                }
               db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AmountCouponList()
        {
            return PartialView(db.ViewAmountCoupons.ToList());
        }
        #endregion


    }
}