using PointOfSale.Helpers;
using PointOfSale.Models;
using PointOfSale.ModelViews;
using PointOfSale.ModelViews.Sales;
using PointOfSale.ModelViews.Inventory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.Controllers
{
    public class InventoryController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        public ActionResult Dashboard()
        {
            return View();
        }
        #region Task Scheduler
        public ActionResult TaskScheduler()
        {
            return View();
        }
        #region Inventory Data set
        public JsonResult InvDataSetGenerate()
        {
            try
            {
                InventoryDailyDataSet data = new InventoryDailyDataSet();
                if (db.InventoryDailyDataSets.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)))
                {
                    data = db.InventoryDailyDataSets.FirstOrDefault(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now));
                }
                data.TotalItemSold = 0;
                //total item sold
                if (db.ViewOrderTransactions.Any(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    data.TotalItemSold = (int)db.ViewOrderTransactions.Where(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)).Sum(a => a.Quantity);
                }
                data.TotalItemReturned = 0;
                //total item return
                if (db.StockDailyTopReturns.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)))
                {
                    data.TotalItemReturned = db.StockDailyTopReturns.Where(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)).Sum(s => s.Quantity);
                }
                //total item borrowed
                data.TotalItemBorrowed = 0;
                if (db.ViewBorrowProducts.Any(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    data.TotalItemBorrowed = (int)db.ViewBorrowProducts.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)).Sum(a => a.Quantity);
                }
                //total item wasted
                data.TotalItemWasted = 0;
                if (db.ViewWasteTransactions.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    data.TotalItemWasted = (int)db.ViewWasteTransactions.Where(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(DateTime.Now)).Sum(a => a.Quantity);
                }
                data.Date = now.Date;
                if (data.Id > 0)
                {
                    db.Entry(data).State = EntityState.Modified;
                }
                else
                {
                    db.InventoryDailyDataSets.Add(data);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult InventoryDailyDataSet()
        {
            ViewBag.CountNumber = db.InventoryDailyDataSets.Count();
            return View();
        }
        public PartialViewResult InventoryDailyDataList(int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<InventoryDailyDataSet>();
            if (count > 0)
            {
                list = db.InventoryDailyDataSets.Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.InventoryDailyDataSets.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.InventoryDailyDataSets.Where(m => DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        [EncryptedActionParameter]
        public ActionResult InvDailyDataPrint(int? days, DateTime? from, DateTime? to)
        {
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View();
        }
        #endregion

        #region Top Sold 
        public JsonResult DailyTopSoldGenerate()
        {
            try
            {
                int generateCount = (int)db.MiscFuntions.Find(12).Value; // id 12 is generate count
                StockDailyTopSold topItem;

                if (db.StockDailyTopSolds.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)))
                {
                    List<StockDailyTopSold> soldList = new List<StockDailyTopSold>();
                    soldList = db.StockDailyTopSolds.Where(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)).ToList();
                    foreach (var itemSold in soldList)
                    {
                        db.Entry(itemSold).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }


                if (db.ViewOrderTransactions.Any(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    var soldItems = db.ViewOrderTransactions.Where(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now))
                        .GroupBy(g => new { g.ProductId, g.DistributeId })
                        .Select(a => new { a.FirstOrDefault().ProductId, a.FirstOrDefault().DistributeId, a.FirstOrDefault().ProductName, Quantity = a.Sum(s => s.Quantity) });

                    if (soldItems.Any())
                    {
                        soldItems = soldItems.OrderByDescending(a => a.Quantity).Take(generateCount);
                        var list = soldItems.Select(s => new { s.ProductId, s.ProductName, s.DistributeId, s.Quantity }).ToList();
                        foreach (var item in list)
                        {
                            topItem = new StockDailyTopSold();
                            topItem.ProductId = item.ProductId;
                            topItem.ProductName = item.ProductName;
                            topItem.DistributeId = item.DistributeId;
                            topItem.Quantity = (int)item.Quantity;
                            topItem.Date = now.Date;
                            db.StockDailyTopSolds.Add(topItem);
                            db.SaveChanges();
                        }
                    }
                }

                if (db.GenerateDates.Any(a => a.Type == 1 && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)) == false)
                {
                    GenerateDate date = new GenerateDate();
                    date.Type = 1;
                    date.Date = now.Date;
                    db.GenerateDates.Add(date);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult TopSoldDate()
        {
            ViewBag.CountNumber = db.GenerateDates.Where(a => a.Type == 1).Count();
            return View();
        }
        public PartialViewResult TopSoldDateList(int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<GenerateDate>();
            if (count > 0)
            {
                list = db.GenerateDates.Where(a => a.Type == 1).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.GenerateDates.Where(m => m.Type == 1 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.GenerateDates.Where(m => m.Type == 1 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        [EncryptedActionParameter]
        public ActionResult StockDailyTopSold(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }
        public PartialViewResult StockDailyTopSoldList(DateTime date)
        {
            var list = db.StockDailyTopSolds.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(date)).ToList();
            return PartialView(list);
        }

        [EncryptedActionParameter]
        public ActionResult TopSoldPrint(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }

        #endregion

        #region Top Return
        public JsonResult DailyTopReturnGenerate()
        {
            try
            {
                int generateCount = (int)db.MiscFuntions.Find(12).Value; // id 12 is generate count
                StockDailyTopReturn topItem;
                if (db.StockDailyTopReturns.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)))
                {
                    List<StockDailyTopReturn> returnList = new List<StockDailyTopReturn>();
                    returnList = db.StockDailyTopReturns.Where(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)).ToList();
                    foreach (var itemReturn in returnList)
                    {
                        db.Entry(itemReturn).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }

                if (db.StockDailyReturns.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    var returnItems = db.StockDailyReturns.Where(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(DateTime.Now))
                        .GroupBy(g => new { g.ProductId, g.DistributeId })
                        .Select(a => new { a.FirstOrDefault().ProductId, a.FirstOrDefault().DistributeId, a.FirstOrDefault().ProductName, Quantity = a.Sum(s => s.Quantity) });

                    if (returnItems.Any())
                    {
                        returnItems = returnItems.OrderByDescending(a => a.Quantity).Take(generateCount);
                        var list = returnItems.Select(s => new { s.ProductId, s.DistributeId, s.ProductName, s.Quantity }).ToList();
                        foreach (var item in list)
                        {
                            topItem = new StockDailyTopReturn();
                            topItem.ProductId = item.ProductId;
                            topItem.ProductName = item.ProductName;
                            topItem.DistributeId = item.DistributeId;
                            topItem.Quantity = (int)item.Quantity;
                            topItem.Date = now.Date;
                            db.StockDailyTopReturns.Add(topItem);
                            db.SaveChanges();
                        }
                    }
                }

                if (db.GenerateDates.Any(a => a.Type == 2 && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)) == false)
                {
                    GenerateDate date = new GenerateDate();
                    date.Type = 2;
                    date.Date = now.Date;
                    db.GenerateDates.Add(date);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult TopReturnGenerateDate()
        {
            ViewBag.CountNumber = db.GenerateDates.Where(a => a.Type == 2).Count();
            return View();
        }
        public PartialViewResult TopReturnDateList(int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<GenerateDate>();
            if (count > 0)
            {
                list = db.GenerateDates.Where(a => a.Type == 2).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.GenerateDates.Where(m => m.Type == 2 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.GenerateDates.Where(m => m.Type == 2 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }

        [EncryptedActionParameter]
        public ActionResult StockDailyTopReturn(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }
        public PartialViewResult StockDailyTopReturnList(DateTime date)
        {
            var list = db.StockDailyTopReturns.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(date)).ToList();
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult TopReturnPrint(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }
        #endregion

        #region Stock Daily Sold
        public JsonResult DailySoldGenerate()
        {
            try
            {
                StockDailySold soldItem;
                if (db.ViewOrderTransactions.Any(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)))
                {
                    var soldItems = db.ViewOrderTransactions.Where(a => a.Status == true && a.TransactionType == 1 && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(DateTime.Now))
                        .GroupBy(g => new { g.ProductId, g.DistributeId })
                        .Select(a => new { a.FirstOrDefault().ProductId, a.FirstOrDefault().DistributeId, a.FirstOrDefault().ProductName, Quantity = a.Sum(s => s.Quantity) });

                    if (soldItems.Any())
                    {
                        var list = soldItems.Select(s => new { s.ProductId, s.ProductName, s.DistributeId, s.Quantity }).ToList();
                        foreach (var item in list)
                        {
                            if (db.StockDailySolds.Any(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now) && a.ProductId == item.ProductId && a.DistributeId == item.DistributeId))
                            {
                                soldItem = db.StockDailySolds.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now));
                                if (soldItem.Quantity < item.Quantity)
                                {
                                    soldItem.Quantity = (int)item.Quantity;
                                    db.Entry(soldItem).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                soldItem = new StockDailySold();
                                soldItem.ProductId = item.ProductId;
                                soldItem.ProductName = item.ProductName;
                                soldItem.DistributeId = item.DistributeId;
                                soldItem.Quantity = (int)item.Quantity;
                                soldItem.Date = now.Date;
                                db.StockDailySolds.Add(soldItem);
                            }
                            db.SaveChanges();
                        }
                    }
                }

                if (db.GenerateDates.Any(a => a.Type == 3 && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now)) == false)
                {
                    GenerateDate date = new GenerateDate();
                    date.Type = 3;
                    date.Date = now.Date;
                    db.GenerateDates.Add(date);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DailySoldGenerateDate()
        {
            ViewBag.CountNumber = db.GenerateDates.Where(a => a.Type == 3).Count();
            return View();
        }
        public PartialViewResult DailySoldDateList(int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<GenerateDate>();
            if (count > 0)
            {
                list = db.GenerateDates.Where(a => a.Type == 3).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.GenerateDates.Where(m => m.Type == 3 && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.GenerateDates.Where(m => m.Type == 3 && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        [EncryptedActionParameter]
        public ActionResult StockDailySold(DateTime date)
        {
            ViewBag.Date = date;
            int productId = 0;
            List<ViewProductCategory> categoryList = new List<ViewProductCategory>();
            var productIds = db.StockDailySolds.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(date))
                .GroupBy(g => g.ProductId)
                .Select(s => new { s.FirstOrDefault().ProductId });

            if(productIds.Any())
            {
                foreach(var id in productIds)
                {
                    productId = Convert.ToInt32(id.ProductId);
                    var category = db.ViewProductCategories.Where(a => a.ProductId == productId).ToList();
                    if(category != null)
                    {
                        categoryList.AddRange(category);
                    }
                }
            }
            ViewBag.CategoryList = new SelectList(categoryList.Where(a => a.ProductCategoryStatus == true).GroupBy(a => a.CategoryId).Select(a => new { a.FirstOrDefault().CategoryId, a.FirstOrDefault().CategoryName }), "CategoryId", "CategoryName");
            ViewBag.SubCategoryList = new SelectList(categoryList.Where(a => a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(a => a.CategoryId).Select(a => new { a.FirstOrDefault().SubCategoryId, a.FirstOrDefault().SubCategoryName }), "SubCategoryId", "SubCategoryName");
            return View();
        }

        public JsonResult GetProductList(DateTime date, int? categoryId, int? subCategoryId)
        {
            var aList = new SelectList("", "");
            if(categoryId > 0)
            {
                aList = new SelectList(db.ViewStockDailySolds.Where(a => a.CategoryId == categoryId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(date)).GroupBy(g => g.Id).Select(s => new { s.FirstOrDefault().Id, s.FirstOrDefault().ProductName }), "Id", "ProductName");
            }
            else if(subCategoryId > 0)
            {
                aList = new SelectList(db.ViewStockDailySolds.Where(a => a.SubCategoryId == subCategoryId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(date)).GroupBy(g => g.Id).Select(s => new { s.FirstOrDefault().Id, s.FirstOrDefault().ProductName }), "Id", "ProductName");
            }
            else
            {
                var list = db.StockDailySolds.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(date)).ToList();
                aList = new SelectList(list.Select(s => new { s.Id, s.ProductName }), "Id", "ProductName");
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult StockDailySoldList(DateTime? date, string ids, int? categoryId, int? subCategoryId)
        {
            var list = new List<StockDailySold>();
            if(!string.IsNullOrEmpty(ids))
            {
                foreach(var id in ids.Split(','))
                {
                    long soldId = Convert.ToInt64(id);
                    var dailySold = db.StockDailySolds.FirstOrDefault(a => a.Id == soldId);
                    if(dailySold != null)
                    {
                        list.Add(dailySold);
                    }
                }
            }
            else if(categoryId > 0)
            {
                var soldIdList = db.ViewStockDailySolds.Where(a => a.CategoryId == categoryId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(date)).GroupBy(a => a.Id).Select(s => new { s.FirstOrDefault().Id }).ToList();
                var idList = soldIdList.Select(s => new { s.Id });
                if (idList.Any())
                {
                    foreach(var id in idList)
                    {
                        long soldId = Convert.ToInt64(id.Id);
                        var dailySold = db.StockDailySolds.FirstOrDefault(a => a.Id == soldId);
                        if (dailySold != null)
                        {
                            list.Add(dailySold);
                        }
                    }
                }
            }
            else if(subCategoryId > 0)
            {
                var soldIdList = db.ViewStockDailySolds.Where(a => a.SubCategoryId == subCategoryId && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(date)).GroupBy(a => a.Id).Select(s => new { s.FirstOrDefault().Id }).ToList();
                var idList = soldIdList.Select(s => new { s.Id });
                if (idList.Any())
                {
                    foreach (var id in idList)
                    {
                        long soldId = Convert.ToInt64(id.Id);
                        var dailySold = db.StockDailySolds.FirstOrDefault(a => a.Id == soldId);
                        if (dailySold != null)
                        {
                            list.Add(dailySold);
                        }
                    }
                }
            }
            else
            {
                list = db.StockDailySolds.Where(m => DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(date)).ToList();
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult DailySoldPrint(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }
        #endregion
        #endregion
        
        #region Indent
        public ActionResult IndentHistory()
        {
            ViewBag.CountAll = db.Indents.Where(a => a.Status != 5).Count();
            ViewBag.CountPending = db.Indents.Where(a => a.Status == 1).Count();
            ViewBag.CountPartial = db.Indents.Where(a => a.Status == 2).Count();
            ViewBag.CountApproved = db.Indents.Where(a => a.Status == 3).Count();
            ViewBag.CountCompleted = db.Indents.Where(a => a.Status == 4).Count();
            ViewBag.CountDeleted = db.Indents.Where(a => a.Status == 5).Count();
            return View();
        }
        public ActionResult IndentCreate()
        {
            return View();
        }
        public JsonResult IndentSave(IList<ImportModelItem> IndentData, int createdBy)
        {
            try
            {
                Indent indent = new Indent();
                IndentItem item;
                long count = 1;
                if(db.Indents.Any())
                {
                    count = db.Indents.Max(a => a.Id) + 1;
                }
                indent.VoucherName = "Indent_" + count.ToString("0000");
                indent.Status = 1;
                indent.CreatedBy = createdBy;
                indent.CreatedDate = DateTime.Now;
                db.Indents.Add(indent);

                db.SaveChanges();

                if(IndentData != null)
                {
                    foreach(var data in IndentData)
                    {
                        item = new IndentItem();
                        item.IndentId = indent.Id;
                        item.ProductId = data.ProductId;
                        item.DistributeId = data.DistributeId;
                        item.ProductName = data.ProductName;
                        item.RequestQuantity = (int)data.Quantity;
                        item.RemainingQuantity = (int)data.Quantity;
                        item.Price = data.PeritemCost;
                        item.Comment = data.Comment;
                        item.CreatedBy = createdBy;
                        item.Status = 1;
                        item.CreatedDate = DateTime.Now;
                        db.IndentItems.Add(item);
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
        public PartialViewResult IndentList(int? status, int? days, DateTime? from, DateTime? to, int? count, string selectedId, bool? isProcureList, bool? isWorkOrderList)
        {
            ViewBag.IsProcurementList = false;
            if(isProcureList == true)
            {
                ViewBag.IsProcurementList = true;
            }
            ViewBag.IsWorkOrderList = false;
            if(isWorkOrderList == true)
            {
                ViewBag.IsWorkOrderList = true;
            }
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewIndent>();
            if(!string.IsNullOrEmpty(selectedId))
            {
                foreach(var id in selectedId.Split(','))
                {
                    long indentId = Convert.ToInt64(id);
                    var indent = db.ViewIndents.FirstOrDefault(a => a.Id == indentId);
                    if(indent != null)
                    {
                        list.Add(indent);
                    }
                }
            }
            else if(status > 0)
            {
                if (count > 0)
                {
                    list = db.ViewIndents.Where(a => a.Status == status).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewIndents.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewIndents.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else if(isProcureList == true)
            {
                if(count > 0)
                {
                    list = db.ViewIndents.Where(a => a.Status != 5).Take((int)count).ToList();
                }
                else
                {
                    list = db.ViewIndents.Where(a => a.Status != 5).Take(10).ToList();
                }
            }
            else if(isWorkOrderList == true)
            {
                list = db.ViewIndents.Where(a => a.ProcurementStatus == 2 && a.Status < 4).ToList();
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewIndents.Where(a => a.Status != 5).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewIndents.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewIndents.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.CreatedDate));
        }

        public JsonResult GetIndentVoucherList(int? status, int? days, DateTime? from, DateTime? to, int? procureStatus, string text, long? proRowId, long? wOrderId)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new SelectList("", "");
            if(status > 0)
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.Indents.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.Indents.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.Indents.Where(a => a.Status == status).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            else if(procureStatus > 0)
            {
                list = new SelectList(db.Indents.Where(a => a.ProcurementStatus == procureStatus && a.Status < 4 ).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
            }
            else if(proRowId > 0) // edit in work order
            {
                var product = db.ViewProducts.FirstOrDefault(a => a.RowID == proRowId);
                var indentList = db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2 && a.IndentStatus < 4 && a.ProductId == product.ProductId && a.DistributeId == product.DistributeId)
                    .GroupBy(g => new { g.IndentId })
                    .Select(s => new { s.FirstOrDefault().IndentId,  s.FirstOrDefault().IndentVoucher }).ToList();
                if (indentList.Any())
                {
                    foreach (var indPro in indentList.ToList())
                    {
                        if(db.ViewWorkOrderItems.Any(a => a.IndentId == indPro.IndentId && a.WorkOrderId == wOrderId && a.Status == 1 && a.WorkOrderStatus < 4 && a.ProductId == product.ProductId && a.DistributeId ==product.DistributeId ))
                        {
                            indentList.Remove(indPro);
                            if(indentList.Any() == false)
                            {
                                break;
                            }
                        }
                    }
                    list = new SelectList(indentList, "IndentId", "IndentVoucher");
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.Indents.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.Indents.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.Indents.Where(a => a.Status != 5 ).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IndentVoucherStatusChange(long VoucherId, int status, int CreatedBy)
        {
            try
            {
                var indentVoucher = db.Indents.Find(VoucherId);
                if(status == 3) //voucher approved
                {
                    indentVoucher.Status = status;
                    indentVoucher.ApprovedBy = CreatedBy;
                    indentVoucher.ApprovedDate = DateTime.Now;
                }
                else
                {
                    indentVoucher.UpdatedBy = CreatedBy;
                    indentVoucher.UpdatedDate = DateTime.Now;
                }

                if(status == 5) //voucher delete
                {
                    indentVoucher.Status = status;  
                }
                else if (status == 1) // status 1 for change procurement status 
                {
                    indentVoucher.ProcurementStatus = 1;
                }
                else if (status == 2) // change procurement status 
                {
                    indentVoucher.ProcurementStatus = 2;
                }
                db.Entry(indentVoucher).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult IndentVoucherDetails(long VoucherId, bool? isView)
        {
            ViewBag.IsView = false;
            if(isView == true)
            {
                ViewBag.IsView = true;
            }
            return View(db.ViewIndents.FirstOrDefault(a => a.Id == VoucherId));
        }
        public PartialViewResult IndentItemList(long VoucherId, bool IsView)
        {
            ViewBag.IsView = IsView;
            return PartialView(db.ViewIndentItems.Where(a => a.IndentId == VoucherId && a.Status > 0).ToList());
        }
        public PartialViewResult AddIndentItem(long? itemId, long intentId, bool? isQuantityEdit)
        {
            ViewBag.ItemId = itemId;
            ViewBag.IntendId = intentId;
            ViewBag.IsQuantityEdit = false;
            if(isQuantityEdit == true)
            {
                ViewBag.IsQuantityEdit = true;
            }
            return PartialView();
        }
        public PartialViewResult AddIndentItemPartial(long? itemId, long intentId, bool? isQuantityEdit)
        {
            ViewBag.IsQuantityEdit = isQuantityEdit;
            if(itemId > 0)
            {
                var indentItem = db.IndentItems.Find(itemId);
                if(db.Indents.FirstOrDefault(a => a.Id == indentItem.IndentId).ProcurementStatus == 2)
                {
                    int remainingQty = 1;
                    if (isQuantityEdit == true)
                    {
                        int indentRecv = indentItem.ReceiveQuantity;
                        int woRem = 0;
                        int warehousQty = 0;
                        int mrrQty = 0;
                       
                        if (db.ViewWorkOrderItems.Any(a => a.Status == 1 && a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4))
                        {
                            woRem = db.ViewWorkOrderItems.Where(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4).Sum(s => s.RemainingQty);
                        }
                        if (db.StockWarehouses.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0))
                        {
                            warehousQty = db.StockWarehouses.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0).Sum(s => s.Quantity);
                        }
                        if (db.IndMRRItems.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1))
                        {
                            mrrQty = db.IndMRRItems.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1).Sum(a => a.Quantity);
                        }
                        remainingQty = indentRecv + woRem + warehousQty + mrrQty;
                        if(remainingQty == 0)
                        {
                            remainingQty = 1;
                        }
                    }
                    ViewBag.RemainingQty = remainingQty;

                }
                return PartialView(indentItem);
            }
            else
            {
                var productList = db.ViewProducts.Where(a => a.Status == true).Select(s => new { s.RowID, s.ProductId, s.DistributeId, s.ProductName }).ToList();
                var indentItemList = db.IndentItems.Where(a => a.IndentId == intentId && a.Status > 0).ToList();
                if (indentItemList != null)
                {
                    foreach (var item in indentItemList)
                    {
                        productList.Remove(productList.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId));
                    }
                }
                ViewBag.ProductList = new SelectList(productList, "RowID", "ProductName");
            }
            return PartialView();
        }
        public JsonResult IndentItemUpdate(IndentItem item, long? RowId, int? changeQuantity)
        {
            try
            {
                var indent = db.Indents.Find(item.IndentId);
                if(indent.Status != 1) //status 1 is pending for approve
                {
                    indent.Status = 2; // partial approved
                    db.Entry(indent).State = EntityState.Modified;
                }
                if (item.Id > 0)
                {
                    var indentItem = db.IndentItems.Find(item.Id);
                    if(changeQuantity > 0)
                    {
                        indentItem.ChangeRequest = true;
                        indentItem.ChangeQty = changeQuantity;
                    }
                    else
                    {
                        indentItem.RequestQuantity = item.RequestQuantity;
                        indentItem.RemainingQuantity = item.RequestQuantity;
                        indentItem.Price = item.Price;
                        indentItem.Comment = item.Comment;
                    }
                    indentItem.UpdatedBy = item.CreatedBy;
                    indentItem.UpdatedDate = DateTime.Now;
                    db.Entry(indentItem).State = EntityState.Modified;
                }
                else
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == RowId);
                    item.ProductId = product.ProductId;
                    item.DistributeId = (long)product.DistributeId;
                    item.ProductName = product.ProductName;
                    item.CreatedDate = DateTime.Now;
                    if(indent.Status != 1)
                    {
                        item.Status = 2; //not approve
                    }
                    else
                    {
                        item.Status = 1; // active
                    }
                    db.IndentItems.Add(item);
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckIsIndItemDelete(long itemId)
        {
            try
            {
                var indentItem = db.IndentItems.Find(itemId);

                int remainingQty = 0;
                int indentRecv = indentItem.ReceiveQuantity;
                int woRem = 0;
                int warehousQty = 0;
                int mrrQty = 0;
                if (db.ViewWorkOrderItems.Any(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4))
                {
                    woRem = db.ViewWorkOrderItems.Where(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4).Sum(s => s.RemainingQty);
                }
                if (db.StockWarehouses.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0))
                {
                    warehousQty = db.StockWarehouses.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0).Sum(s => s.Quantity);
                }
                if (db.IndMRRItems.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1))
                {
                    mrrQty = db.IndMRRItems.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1).Sum(a => a.Quantity);
                }
                remainingQty = indentRecv + woRem + warehousQty + mrrQty;
                return Json(remainingQty, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IndentItemStatusChange(long itemId, bool? isChangesApprove, bool? isRemoveChanges, bool? isDeleteItem)
        {
            try
            {
                var indentItem = db.IndentItems.Find(itemId);
                indentItem.Status = 1;
                if (isDeleteItem == true)
                {
                    indentItem.Status = 0;
                }
                if(isChangesApprove == true)
                {
                    indentItem.RequestQuantity = (int)indentItem.ChangeQty;
                    indentItem.RemainingQuantity = (int)indentItem.ChangeQty;

                    indentItem.ChangeQty = 0;
                    indentItem.ChangeRequest = false;
                }
                else if(isRemoveChanges == true)
                {
                    indentItem.ChangeQty = 0;
                    indentItem.ChangeRequest = false;
                }
                db.Entry(indentItem).State = EntityState.Modified;
                db.SaveChanges();

                if(db.IndentItems.Where(a => a.IndentId == indentItem.IndentId && a.Status > 0).Any(a => a.Status == 2) == false)
                {
                    var indent = db.Indents.Find(indentItem.IndentId);
                    indent.Status = 3; //approve
                    db.Entry(indent).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //indent delete
                if (db.IndentItems.Any(a => a.IndentId == indentItem.IndentId && a.Status > 0) == false)
                {
                    var indent = db.Indents.Find(indentItem.IndentId);
                    indent.Status = 5; //delete
                    db.Entry(indent).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult IndentItemRemainingCheck(long indentId, long proRowId)
        {
            var product = db.ViewProducts.FirstOrDefault(a => a.RowID == proRowId);
            var indentItem = db.IndentItems.FirstOrDefault(a => a.IndentId == indentId && a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);

            int remainingQty = 0;
            int woRem = 0;
            int warehousQty = 0;
            int mrrQty = 0;
            if (db.ViewWorkOrderItems.Any(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4))
            {
                woRem = db.ViewWorkOrderItems.Where(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4).Sum(s => s.RemainingQty);
            }
            if (db.StockWarehouses.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0))
            {
                warehousQty = db.StockWarehouses.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0).Sum(s => s.Quantity);
            }
            if (db.IndMRRItems.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1))
            {
                mrrQty = db.IndMRRItems.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1).Sum(a => a.Quantity);
            }
            remainingQty = indentItem.RequestQuantity - (indentItem.ReceiveQuantity + woRem + warehousQty + mrrQty);
            return Json(remainingQty, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Procurement History
        public ActionResult Procurement()
        {
            ViewBag.IndentCount = db.Indents.Where(a => a.Status != 5).Count();
            ViewBag.WorkOrderCount = db.WorkOrders.Where(a => a.Status != 5).Count();

            //minimum product count
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;
            int minimumProCount = 0;
            foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
            {
                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                if (stockProduct != null)
                {
                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                    {
                        minimumProCount++;
                    }
                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                    {
                        minimumProCount++;
                    }
                }
                else
                {
                    minimumProCount++;
                }
            }
            ViewBag.minimumQtyProCount = minimumProCount;
            return View();
        }
        #endregion

        #region MRR in procurement
        public ActionResult IndentMRR()
        {
            return View();
        }
        #endregion

        #region WorkOrder
        public ActionResult CreateWorkOrder()
        {
            return View();
        }
        public JsonResult GetProductListForWorkOrder()
        {
            var list = new SelectList(db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2).GroupBy(g => new { g.ProductId, g.DistributeId }).Select(s => new { s.FirstOrDefault().Id, s.FirstOrDefault().ProductName }).OrderBy(o => o.ProductName), "Id", "ProductName");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult IndProListForWOCreate(string selectedValue)
        {
            List<ViewIndentItem> list = new List<ViewIndentItem>();

            if (!string.IsNullOrEmpty(selectedValue))
            {
                foreach (var id in selectedValue.Split(','))
                {
                    long rowId = Convert.ToInt64(id);
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    if (product != null)
                    {
                        if (db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2 && a.IndentStatus < 4).Any(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId))
                        {
                            var indentItems = db.ViewIndentItems.Where(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId && a.Status == 1 && a.ProcurementStatus == 2);
                            list.AddRange(indentItems);
                        }
                    }
                }
            }
            else
            {
                list = db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2 && a.IndentStatus < 4).OrderBy(o => o.ProductName).ToList();
            }
            return PartialView(list);
        }
        public PartialViewResult WorkOrderProductList(string selectedValue, bool? isListForProc)
        {
            List<ViewIndentItem> list = new List<ViewIndentItem>();

            if(!string.IsNullOrEmpty(selectedValue))
            {
                foreach(var id in selectedValue.Split(','))
                {
                    long rowId = Convert.ToInt64(id);
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    if(product != null)
                    {
                        if(db.ViewIndentItems.Where(a => a.Status ==1 && a.ProcurementStatus == 2 && a.IndentStatus < 4).Any(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId))
                        {
                            var indentItems = db.ViewIndentItems.Where(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId && a.Status == 1 && a.ProcurementStatus == 2);
                            list.AddRange(indentItems);
                        }
                    }
                }
            }
            else
            {
                list = db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2 && a.IndentStatus < 4).OrderBy(o => o.ProductName).ToList();
            }
            ViewBag.IsListForProc = isListForProc;
            return PartialView(list);
        }
        public PartialViewResult WorkOrderPay(IList<ImportModelItem> orderData)
        {
            return PartialView(orderData);
        }
        public JsonResult WorkOrderSave(ImportModel model)
        {
            try
            {
                WorkOrder order = new WorkOrder();
                long countNumber = 1;
                if (db.WorkOrders.Any())
                {
                    countNumber = db.WorkOrders.Max(a => a.Id) + 1;
                }
                order.VoucherName = "WO_" + countNumber.ToString("0000");
                order.SupplierId = model.SupplierId;
                if(model.ImportData.Any(a => a.Cost == 0))
                {
                    order.TotalAmount = 0;
                }
                else
                {
                    order.TotalAmount = model.ImportData.Sum(a => a.Cost);
                }
                order.Status = 1; //pending
                order.CreatedBy = model.UserId;
                order.CreatedDate = DateTime.Now;
                db.WorkOrders.Add(order);

                db.SaveChanges();

                //work order transaction save
                if (model.ImportData != null)
                {
                    WorkOrderItem orderItem;
                    foreach (var item in model.ImportData)
                    {
                        orderItem = new WorkOrderItem();
                        orderItem.WorkOrderId = order.Id;
                        orderItem.IndentId = item.IndentId;
                        orderItem.ProductId = item.ProductId;
                        orderItem.ProductName = item.ProductName;
                        orderItem.DistributeId = item.DistributeId;
                        orderItem.RequestQty = (int)item.Quantity;
                        orderItem.RemainingQty = (int)item.Quantity;
                        orderItem.PerItemPrice = item.PeritemCost;
                        orderItem.Price = item.Cost;
                        orderItem.Status = 1;
                        orderItem.CreatedBy = model.UserId;
                        orderItem.CreatedDate = DateTime.Now;
                        db.WorkOrderItems.Add(orderItem);

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
        public ActionResult WorkOrder()
        {
            return View();
        }
        public PartialViewResult WorkOrderList(int? status, int? days, DateTime? from, DateTime? to, int? count, string selectedId, bool? isListForProc)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewWorkOrder>();
            if (!string.IsNullOrEmpty(selectedId))
            {
                foreach (var id in selectedId.Split(','))
                {
                    long workOrderId = Convert.ToInt64(id);
                    var workOrder = db.ViewWorkOrders.FirstOrDefault(a => a.Id == workOrderId);
                    if (workOrder != null)
                    {
                        list.Add(workOrder);
                    }
                }
            }
            else if (status > 0)
            {
                if (count > 0)
                {
                    list = db.ViewWorkOrders.Where(a => a.Status == status).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewWorkOrders.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewWorkOrders.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewWorkOrders.Where(a => a.Status != 5).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewWorkOrders.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewWorkOrders.Where(m => m.Status != 5 && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            ViewBag.IsListForProc = isListForProc;
            return PartialView(list.OrderByDescending(a => a.CreatedDate));
        }
        public JsonResult GetWorkOrderVoucherList(int? status, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new SelectList("", "");
            if (status > 0)
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.WorkOrders.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.WorkOrders.Where(a => a.Status == status && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.WorkOrders.Where(a => a.Status == status).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = new SelectList(db.WorkOrders.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = new SelectList(db.WorkOrders.Where(a => a.Status != 5 && DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.CreatedDate) <= DbFunctions.TruncateTime(end)).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
                else
                {
                    list = new SelectList(db.WorkOrders.Where(a => a.Status != 5).Select(s => new { s.Id, s.VoucherName }), "Id", "VoucherName");
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult WorkOrderVoucherDetails(long VoucherId, bool? isView)
        {
            ViewBag.IsView = false;
            if (isView == true)
            {
                ViewBag.IsView = true;
            }
            return View(db.ViewWorkOrders.FirstOrDefault(a => a.Id == VoucherId));
        }
        public PartialViewResult WorkOrderItemList(long VoucherId, bool? IsView)
        {
            ViewBag.IsView = false;
            if (IsView == true)
            {
                ViewBag.IsView = true;
            }
            return PartialView(db.ViewWorkOrderItems.Where(a => a.WorkOrderId == VoucherId && a.Status > 0).ToList());
        }
        public JsonResult WorkOrderStatusChange(long id, int status, int CreatedBy)
        {
            try
            {
                var workOrder = db.WorkOrders.Find(id);
                workOrder.Status = status;
                if (status == 3) //voucher approved
                {
                    workOrder.Status = status;
                    workOrder.ApprovedBy = CreatedBy;
                    workOrder.ApprovedDate = DateTime.Now;
                }
                else
                {
                    workOrder.UpdatedBy = CreatedBy;
                    workOrder.UpdatedDate = DateTime.Now;
                }
                db.Entry(workOrder).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AddWorkOrderItem(long? itemId, long workId, bool? isQuantityEdit)
        {
            ViewBag.ItemId = itemId;
            ViewBag.WorkId = workId;
            ViewBag.IsQuantityEdit = false;
            if (isQuantityEdit == true)
            {
                ViewBag.IsQuantityEdit = true;
            }
            return PartialView();
        }
        public PartialViewResult AddWorkOrderItemPartial(long? itemId, long workId, bool? isQuantityEdit)
        {
            ViewBag.IsQuantityEdit = isQuantityEdit;
            if (itemId > 0)
            {
                var workOrderItem = db.WorkOrderItems.Find(itemId);
                if(isQuantityEdit == true)
                {
                    ViewBag.RemainingQty = workOrderItem.ReceiveQty;
                }
                var indentItem = db.IndentItems.FirstOrDefault(a => a.IndentId == workOrderItem.IndentId && a.ProductId == workOrderItem.ProductId && a.DistributeId == workOrderItem.DistributeId);
                int remainingQty = 0;
                int woRem = 0;
                int warehousQty = 0;
                int mrrQty = 0;
                if (db.ViewWorkOrderItems.Any(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4))
                {
                    woRem = db.ViewWorkOrderItems.Where(a => a.IndentId == indentItem.IndentId && a.Status == 1 && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.WorkOrderStatus < 4).Sum(s => s.RemainingQty);
                }
                if (db.StockWarehouses.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0))
                {
                    warehousQty = db.StockWarehouses.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Quantity > 0).Sum(s => s.Quantity);
                }
                if (db.IndMRRItems.Any(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1))
                {
                    mrrQty = db.IndMRRItems.Where(a => a.IndentId == indentItem.IndentId && a.ProductId == indentItem.ProductId && a.DistributeId == indentItem.DistributeId && a.Status == 1).Sum(a => a.Quantity);
                }
                remainingQty = indentItem.RequestQuantity - (indentItem.ReceiveQuantity + woRem + warehousQty + mrrQty);

                ViewBag.Max = workOrderItem.RequestQty + remainingQty;
                return PartialView(workOrderItem);
            }
            else
            {
                var product = new ViewProduct();
                var productList = new List<ViewProduct>();
                //var productList = db.ViewProducts.Where(a => a.Status == true).Select(s => new { s.RowID, s.ProductId, s.DistributeId, s.ProductName }).ToList();

                var indentProList = db.ViewIndentItems.Where(a => a.Status == 1 && a.ProcurementStatus == 2 && a.IndentStatus < 4)
                    .GroupBy(g => new { g.ProductId, g.DistributeId })
                    .Select(s => new { s.FirstOrDefault().ProductId, s.FirstOrDefault().DistributeId }).ToList();
                if(indentProList.Any())
                {
                    foreach(var indPro in indentProList)
                    {
                        product = db.ViewProducts.FirstOrDefault(a => a.ProductId == indPro.ProductId && a.DistributeId == indPro.DistributeId);
                        if(product != null)
                        {
                            productList.Add(product);
                        }
                    }
                }
                ViewBag.ProductList = new SelectList(productList.OrderBy(o => o.ProductName), "RowID", "ProductName");
            }
            return PartialView();
        }
        public JsonResult WorkOrderItemUpdate(WorkOrderItem item, long? RowId, int? changeQuantity)
        {
            try
            {
                var wOrder = db.WorkOrders.Find(item.WorkOrderId);
                if (wOrder.Status != 1) //status 1 is pending for approve
                {
                    wOrder.Status = 2; // partial approved
                    db.Entry(wOrder).State = EntityState.Modified;
                }
                if (item.Id > 0)
                {
                    var workOrderItem = db.WorkOrderItems.Find(item.Id);
                    if (changeQuantity > 0)
                    {
                        workOrderItem.ChangeRequest = true;
                        workOrderItem.ChangeQty = changeQuantity;
                    }
                    else
                    {
                        workOrderItem.RequestQty = item.RequestQty;
                        workOrderItem.RemainingQty = item.RequestQty;
                        workOrderItem.Price = item.Price;
                        workOrderItem.PerItemPrice = item.PerItemPrice;
                    }
                    workOrderItem.UpdatedBy = item.CreatedBy;
                    workOrderItem.UpdatedDate = DateTime.Now;
                    db.Entry(workOrderItem).State = EntityState.Modified;
                }
                else if(RowId > 0)
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == RowId);
                    item.ProductId = product.ProductId;
                    item.DistributeId = (long)product.DistributeId;
                    item.ProductName = product.ProductName;
                    item.RemainingQty = item.RequestQty;
                    item.CreatedDate = DateTime.Now;
                    if (wOrder.Status != 1)
                    {
                        item.Status = 2; //not approve
                    }
                    else
                    {
                        item.Status = 1; // active
                    }
                    db.WorkOrderItems.Add(item);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkOrderItemStatusChange(long itemId, bool? isChangesApprove, bool? isRemoveChanges, bool? isDeleteItem)
        {
            try
            {
                var workOrderItem = db.WorkOrderItems.Find(itemId);
                workOrderItem.Status = 1;
                if (isDeleteItem == true)
                {
                    workOrderItem.Status = 0;
                }
                if (isChangesApprove == true)
                {
                    workOrderItem.RequestQty = (int)workOrderItem.ChangeQty;
                    workOrderItem.RemainingQty = (int)workOrderItem.ChangeQty;
                    workOrderItem.Price = 0;
                    if (workOrderItem.PerItemPrice > 0)
                    {
                        workOrderItem.Price = workOrderItem.RequestQty * workOrderItem.PerItemPrice;
                    }
                    workOrderItem.ChangeQty = 0;
                    workOrderItem.ChangeRequest = false;
                }
                else if (isRemoveChanges == true)
                {
                    workOrderItem.ChangeQty = 0;
                    workOrderItem.ChangeRequest = false;
                }
                db.Entry(workOrderItem).State = EntityState.Modified;
                db.SaveChanges();

                //work order complete 
                if (db.WorkOrderItems.Where(a => a.WorkOrderId == workOrderItem.WorkOrderId && a.Status > 0).Any(a => a.Status == 2 || a.ChangeRequest == true) == false)
                {
                    var workOrder = db.WorkOrders.Find(workOrderItem.WorkOrderId);
                    workOrder.Status = 3; //approve
                    db.Entry(workOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //work order delete
                if(db.WorkOrderItems.Any(a => a.WorkOrderId == workOrderItem.WorkOrderId && a.Status > 0) == false)
                {
                    var workOrder = db.WorkOrders.Find(workOrderItem.WorkOrderId);
                    workOrder.Status = 5; //delete
                    db.Entry(workOrder).State = EntityState.Modified;
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

        #region Work Order Import
        public ActionResult WorkOrderImport()
        {
            return View();
        }
        public PartialViewResult WorkOrderApproveVoucherList()
        {
            return PartialView(db.ViewWorkOrders.Where(a => a.Status == 3 || a.Status == 2).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult WorkOrderImportManual(long id)
        {
            return View(db.ViewWorkOrders.FirstOrDefault(a=> a.Id == id));
        }
        public PartialViewResult WorkOrderItemListForImport(long? id, string selectedId, bool? isPartialImport)
        {
            ViewBag.IsPartialImport = null;
            var list = new List<ViewWorkOrderItem>();
            if(id > 0)
            {
                list = db.ViewWorkOrderItems.Where(a => a.WorkOrderId == id && a.Status == 1).ToList();
            }
            else if(!string.IsNullOrEmpty(selectedId))
            {
                ViewBag.IsPartialImport = isPartialImport;
                foreach (var aId in selectedId.Split(','))
                {
                    long woItemId = Convert.ToInt64(aId);
                    var woItem = db.ViewWorkOrderItems.FirstOrDefault(a => a.Id == woItemId);
                    if(woItem != null)
                    {
                        list.Add(woItem);
                    }
                }
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult WorkOrderImportSelectedItems(long id, string selectedId, bool isPartialFull)
        {
            ViewBag.SelectedIds = selectedId;
            ViewBag.IsPartialFull = isPartialFull;
            return View(db.ViewWorkOrders.FirstOrDefault(a => a.Id == id));
        }
        public PartialViewResult WorkOrderImportPay(IList<ImportModelItem> ImportData, long SupplierId)
        {
            ViewBag.SupplierName = db.Suppliers.Find(SupplierId).Name;
            ViewBag.AvailableDebitAmount = 0;
            if(db.SupplierDebits.Any(a => a.SupplierId == SupplierId))
            {
                ViewBag.AvailableDebitAmount = db.SupplierDebits.FirstOrDefault(a => a.SupplierId == SupplierId).Amount;
            }
            return PartialView(ImportData);
        }
        public JsonResult WorkOrderImportSave(ImportModel model)
        {
            try
            {
                StockWarehouse wProduct;
                WImport wImport = new WImport();
                WImpTran wImpTrans;
                WorkOrderItem woItem;

                //Warehouse import save
                long impCount = 1;
                if(db.WImports.Any())
                {
                    impCount = db.WImports.Count() + 1;
                }
                wImport.WOrderId = model.WorkOrderId;
                wImport.SupplierId = model.SupplierId;
                wImport.VoucherNo = "WImp_" + impCount.ToString("0000");
                wImport.TotalAmount = model.ImportData.Sum(s => s.Cost);
                wImport.PaidAmount = model.Payments.Where(a => (a.PaymentTypeId != 7)).Sum(s => s.AmountPaid);
                wImport.DueAmount = model.Payments.Where(a => (a.PaymentTypeId == 7)).Sum(s => s.AmountPaid);
                wImport.ImportBy = model.UserId;
                wImport.ImportDate = DateTime.Now;
                db.WImports.Add(wImport);

                db.SaveChanges();

                //warehouse import transaction save
                foreach(var item in model.ImportData)
                {
                    //work order item
                    woItem = db.WorkOrderItems.Find(item.WorkOrderItemId);
                    woItem.ReceiveQty = woItem.ReceiveQty + (int)item.Quantity;
                    woItem.RemainingQty = woItem.RemainingQty - (int)item.Quantity;
                    db.Entry(woItem).State = EntityState.Modified;

                    //warehouse import transaction
                    wImpTrans = new WImpTran();
                    wImpTrans.WImpId = wImport.Id;
                    wImpTrans.ProductId = woItem.ProductId;
                    wImpTrans.DistributeId = woItem.DistributeId;
                    wImpTrans.ProductName = woItem.ProductName;
                    wImpTrans.Quantity = (int)item.Quantity;
                    wImpTrans.PeritemCost = item.PeritemCost;
                    wImpTrans.Cost = item.Cost;

                    db.WImpTrans.Add(wImpTrans);

                    //add product to Stock warehouse 
                    if (db.StockWarehouses.Any(a =>a.IndentId == woItem.IndentId && a.ProductId == woItem.ProductId && a.DistributeId == woItem.DistributeId && a.WOId == model.WorkOrderId))
                    {
                        wProduct = db.StockWarehouses.FirstOrDefault(a =>a.IndentId == woItem.IndentId && a.ProductId == woItem.ProductId && a.DistributeId == woItem.DistributeId && a.WOId == model.WorkOrderId);
                        wProduct.Quantity = wProduct.Quantity + (int)item.Quantity;
                        if(wProduct.Status == false)
                        {
                            wProduct.Status = true;
                        }
                        db.Entry(wProduct).State = EntityState.Modified;
                    }
                    else
                    {
                        wProduct = new StockWarehouse();
                        wProduct.WOId = model.WorkOrderId;
                        wProduct.IndentId = item.IndentId;
                        wProduct.Status = true;
                        wProduct.ProductId = woItem.ProductId;
                        wProduct.DistributeId = woItem.DistributeId;
                        wProduct.ProductName = woItem.ProductName;
                        wProduct.Quantity = (int)item.Quantity;
                        db.StockWarehouses.Add(wProduct);
                    }
                    db.SaveChanges();
                }
                //work order completed if all item receive
                if(db.WorkOrderItems.Where(a => a.WorkOrderId == model.WorkOrderId).Any(a => a.RemainingQty > 0) == false)
                {
                    var workOrder = db.WorkOrders.Find(model.WorkOrderId);
                    workOrder.Status = 4;
                    db.Entry(workOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //Payment transaction save
                PaymentTransaction aPaymentTransaction;
                PaymentBody account;
                if (model.Payments != null)
                {
                    foreach (var aPayment in model.Payments)
                    {
                        aPaymentTransaction = new PaymentTransaction();
                        aPaymentTransaction.PaymentId = wImport.Id;
                        aPaymentTransaction.SupplierId = wImport.SupplierId;
                        aPaymentTransaction.Type = 9; // Type 9 for warehouse import payment
                        aPaymentTransaction.IsCreditPayment = false;
                        aPaymentTransaction.MethodId = 5; // method id 5 for purchase order
                        aPaymentTransaction.PaymentTypeId = aPayment.PaymentTypeId;
                        aPaymentTransaction.PaymentBodyId = aPayment.PaymentBodyId;
                        aPaymentTransaction.Amount = aPayment.AmountPaid;
                        aPaymentTransaction.TransactionNo = aPayment.TransactionNo;
                        aPaymentTransaction.Date = DateTime.Now;
                        aPaymentTransaction.CreatedBy = model.UserId;
                        if (aPayment.PaymentTypeId == 8) //Debit payment transaction
                        {
                            aPaymentTransaction.InOut = true; // InOut true for receive payment

                            //decrease supplier debit amount 
                            SupplierDebit supplierDebit = db.SupplierDebits.FirstOrDefault(a => a.SupplierId == model.SupplierId);
                            supplierDebit.Amount = supplierDebit.Amount - aPayment.AmountPaid;
                            db.Entry(supplierDebit).State = EntityState.Modified;

                            //decrease amount from account
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance - aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;
                        }
                        else if (aPayment.PaymentTypeId == 7) // credit payment transaction
                        {
                            aPaymentTransaction.InOut = true; // InOut true for receive payment
                            aPaymentTransaction.IsCreditPayment = true;
                            //decrease amount from account
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance + aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;
                        }
                        else
                        {
                            aPaymentTransaction.InOut = false; // InOut false for release payment
                            
                            //decrease amount from account
                            account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == aPayment.PaymentBodyId);
                            account.Balance = account.Balance - aPayment.AmountPaid;
                            db.Entry(account).State = EntityState.Modified;
                        }
                        db.PaymentTransactions.Add(aPaymentTransaction);

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

        #region Work-order to indent mrr
        public ActionResult WorkOrderToIndent()
        {
            return View();
        }
        public PartialViewResult WorkOrderProducts()
        {
            return PartialView(db.ViewStockWarehouses.Where(a => a.Quantity > 0).ToList());
        }
        public PartialViewResult WorkOrderIndentProducts(long indentId)
        {
            var indentItem = db.IndentItems.Where(a => a.IndentId == indentId && a.Status == 1 && a.RemainingQuantity > 0).ToList();
            return PartialView(indentItem);
        }

        public JsonResult WorkOrderToIndentSave(IList<ImportModelItem> orderData, int createdBy, long indentId)
        {
            try
            {
                if(orderData != null)
                {
                    long countMrr = 1;
                    if(db.IndentMRRs.Any())
                    {
                        countMrr = db.IndentMRRs.Max(a => a.Id) + 1;
                    }
                    IndentMRR mrr = new IndentMRR();
                    mrr.MRR = "MRR_" + countMrr.ToString("000");
                    mrr.IndentId = indentId;
                    mrr.Status = 1; // status 1 pending
                    mrr.CreatedBy = createdBy;
                    mrr.CreatedDate = DateTime.Now;
                    db.IndentMRRs.Add(mrr);
                    db.SaveChanges();

                    foreach(var item in orderData)
                    {
                        //decrease quantity from work order stock products
                        StockWarehouse woProducts = db.StockWarehouses.FirstOrDefault(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId && a.WOId == item.WOId);
                        woProducts.Quantity = woProducts.Quantity - (int)item.Quantity;
                        db.Entry(woProducts).State = EntityState.Modified;

                        //decrease remainingqty and increase receive qty of Indent Item
                        //IndentItem indentProducts = db.IndentItems.FirstOrDefault(a => a.IndentId == indentId && a.ProductId == item.ProductId && a.DistributeId == item.DistributeId);
                        //indentProducts.ReceiveQuantity = indentProducts.ReceiveQuantity + (int)item.Quantity;
                        //indentProducts.RemainingQuantity = indentProducts.RemainingQuantity - (int)item.Quantity;
                        //db.Entry(indentProducts).State = EntityState.Modified;

                        //mrr item create
                        IndMRRItem mrrItem = new IndMRRItem();
                        mrrItem.MRRId = mrr.Id;
                        mrrItem.WOId = item.WOId;
                        mrrItem.IndentId = indentId;
                        mrrItem.ProductId = item.ProductId;
                        mrrItem.DistributeId = item.DistributeId;
                        mrrItem.ProductName = item.ProductName;
                        mrrItem.Quantity = (int)item.Quantity;
                        mrrItem.Status = 1;

                        db.IndMRRItems.Add(mrrItem);

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
        public JsonResult GetMRRList(string text, int? status)
        {
            var list = new SelectList("", "");
            if(!string.IsNullOrEmpty(text))
            {
                if(status > 0)
                {
                    list = new SelectList(db.IndentMRRs.Where(a => a.Status == status && a.MRR.ToLower().Contains(text.ToLower())).OrderByDescending( o => o.CreatedDate).Take(5).Select(s => new { s.Id, s.MRR }), "Id", "MRR");
                }
                else
                {
                    list = new SelectList(db.IndentMRRs.Where(a => a.MRR.ToLower().Contains(text.ToLower())).OrderByDescending( o => o.CreatedDate).Take(5).Select(s => new { s.Id, s.MRR }), "Id", "MRR");
                }
            }
            else
            {
                if(status > 0)
                {
                    list = new SelectList(db.IndentMRRs.Where(a => a.Status == status).OrderByDescending(o => o.CreatedDate).Take(5).Select(s => new { s.Id, s.MRR }), "Id", "MRR");
                }
                else
                {
                    list = new SelectList(db.IndentMRRs.OrderByDescending(o => o.CreatedDate).Take(5).Select(s => new { s.Id, s.MRR }), "Id", "MRR");
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Import By Indent MRR
        public ActionResult ImportByIndentMRR()
        {
            ViewBag.AllCount = db.IndentMRRs.Count();
            ViewBag.PendingCount = db.IndentMRRs.Where(a => a.Status == 1).Count();
            ViewBag.CompletedCount = db.IndentMRRs.Where(a => a.Status == 2).Count();
            return View();
        }
        public PartialViewResult IndentMRRList(int? count, int? status, bool? isListForProc, string selectedId)
        {
            ViewBag.IsListForProc = isListForProc;
            var list = new List<ViewIndentMRR>();
            if(!string.IsNullOrEmpty(selectedId))
            {
                foreach(var id in selectedId.Split(','))
                {
                    long mrrId = Convert.ToInt64(id);
                    var indentMRR = db.ViewIndentMRRs.FirstOrDefault(a => a.Id == mrrId);
                    if(indentMRR != null)
                    {
                        list.Add(indentMRR);
                    }
                }
            }
            else if(status > 0)
            {
                if(count > 0)
                {
                    list = db.ViewIndentMRRs.Where( a=> a.Status == status).Take((int)count).OrderByDescending(a => a.CreatedDate).ToList();
                }
                else
                {
                    list = db.ViewIndentMRRs.Where(a => a.Status == status).Take(10).OrderByDescending(a => a.CreatedDate).ToList();
                }
            }
            else if(count > 0)
            {
                list = db.ViewIndentMRRs.Take((int)count).OrderByDescending(a => a.CreatedDate).ToList();
            }
            else
            {
                list = db.ViewIndentMRRs.OrderByDescending(a => a.CreatedDate).ToList();
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult IndentMRRDetails(long mrrId)
        {
            return View(db.ViewIndentMRRs.FirstOrDefault(a => a.Id == mrrId));
        }
        public PartialViewResult MRRItemList(long mrrId)
        {
            return PartialView(db.ViewIndentMrrItems.Where(a=> a.MRRId == mrrId).ToList());
        }

        public JsonResult IndentMRRImportSave(long mrrId, int createdBy)
        {
            try
            {
                IndentMRR mrr = db.IndentMRRs.Find(mrrId);
                List<IndMRRItem> mrrItems = db.IndMRRItems.Where(a => a.MRRId == mrrId).ToList();
                Stock aStock;
                StockImport stImport;
                ImportTransaction importTransaction;
                Product aProduct;
                IndentItem indentProducts;

                //save stock import
                stImport = new StockImport();
                stImport.MRRId = mrrId;
                stImport.RefferenceNo = "IMV-" + DateTime.Now.ToString("yyyyMMddHHmmssf");
                stImport.IsCredit = false;
                stImport.Date = DateTime.Now;
                stImport.ImportBy = createdBy;
                db.StockImports.Add(stImport);
                db.SaveChanges();

                foreach (var aImport in mrrItems)
                {
                    //decrease remaining qty and increase receive qty of Indent Item
                    indentProducts = db.IndentItems.FirstOrDefault(a => a.IndentId == aImport.IndentId && a.ProductId == aImport.ProductId && a.DistributeId == aImport.DistributeId);
                    indentProducts.ReceiveQuantity = indentProducts.ReceiveQuantity + aImport.Quantity;
                    indentProducts.RemainingQuantity = indentProducts.RemainingQuantity - aImport.Quantity;
                    db.Entry(indentProducts).State = EntityState.Modified;

                    //import transaction save
                    importTransaction = new ImportTransaction();
                    importTransaction.StockImportId = stImport.StockImportId;
                    importTransaction.ProductId = aImport.ProductId;
                    importTransaction.ProductName = aImport.ProductName;
                    importTransaction.DistributeId = aImport.DistributeId;
                    importTransaction.Quantity = aImport.Quantity;
                    db.ImportTransactions.Add(importTransaction);

                    //stock update
                    if (aImport.DistributeId > 0)
                    {
                        aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aImport.ProductId && a.DistributeId == aImport.DistributeId);
                    }
                    else
                    {
                        aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aImport.ProductId);
                    }
                    if (aStock != null)
                    {
                        decimal? newQuantity = aStock.Quantity + aImport.Quantity;
                        aStock.Quantity = (decimal)newQuantity;
                        db.Entry(aStock).State = EntityState.Modified;
                    }
                    else
                    {
                        aStock = new Stock();
                        aStock.ProductId = aImport.ProductId;
                        aStock.DistributeId = aImport.DistributeId;
                        aStock.Quantity = aImport.Quantity;
                        db.Stocks.Add(aStock);
                    }

                    //mrr item complete
                    aImport.Status = 0;
                    db.Entry(aImport).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //mrr complete
                mrr.Status = 2;
                db.Entry(mrr).State = EntityState.Modified;
                db.SaveChanges();

                //indent complete if all item remaining 0
                if(db.IndentItems.Where(a => a.IndentId == mrr.IndentId).Any(a => a.RemainingQuantity > 0) == false)
                {
                    Indent indent = db.Indents.Find(mrr.IndentId);
                    if(indent.Status == 3)
                    {
                        indent.Status = 4; // complete
                        db.Entry(indent).State = EntityState.Modified;
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
        #endregion

        #region Warehouse
        public ActionResult Warehouse()
        {
            return View();
        }
        public PartialViewResult WareHouseProductList()
        {
            return PartialView(db.StockWarehouses.Where( a=> a.Quantity > 0).ToList());
        }
        public ActionResult WImportHistory()
        {
            ViewBag.ImpCount = db.WImports.Count();
            return View();
        }
        public PartialViewResult WImportList(int? count, string importIds)
        {
            var list = new List<ViewWImport>();
            if(count > 0)
            {
                list = db.ViewWImports.OrderByDescending(a => a.ImportDate).Take((int)count).ToList();
            }
            else if(!string.IsNullOrEmpty(importIds))
            {
                foreach(var id in importIds.Split(','))
                {
                    long wImpId = Convert.ToInt64(id);
                    var viewImport = db.ViewWImports.FirstOrDefault(a => a.Id == wImpId);
                    if(viewImport != null)
                    {
                        list.Add(viewImport);
                    }
                }
            }
            else
            {
                list = db.ViewWImports.OrderByDescending(a => a.ImportDate).ToList();
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult WImportTransaction(long importId)
        {
            return View(db.ViewWImports.FirstOrDefault(a => a.Id == importId));
        }
        public PartialViewResult WImpTransList(long importId)
        {
            return PartialView(db.WImpTrans.Where(a => a.WImpId == importId).ToList());
        }
        public JsonResult GetWImpReffList(string text, int? days, DateTime? from, DateTime? to)
        {
            var list = new List<WImport>();
            if (!string.IsNullOrEmpty(text))
            {
                list = db.WImports.Where(a => a.VoucherNo.ToLower().Contains(text.ToLower()))
                        .OrderByDescending(o => o.ImportDate).Take(5).ToList();
            }
            else
            {
                list = db.WImports.OrderByDescending(o => o.ImportDate).Take(5).ToList();
            }
            var selectList = new SelectList(list.Select(s => new { s.Id, s.VoucherNo }).ToList(), "Id", "VoucherNo");
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult WarehouseQtyInfo(long id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult WarehouseQtyInfoList(long id)
        {
            var stockWarehouse = db.StockWarehouses.Find(id);
            return PartialView(db.ViewStockWarehouses.Where(a => a.ProductId == stockWarehouse.ProductId && a.DistributeId == stockWarehouse.DistributeId && a.Quantity > 0).ToList());
        }
        #endregion

        public JsonResult GetProductListForMultiSelect(int? categoryId, bool? IsAvailable)
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

        #region Stock
        public ActionResult StockList()
        {
            return View();
        }
        //Stock list available
        public PartialViewResult StockListPartial(string rowNumbers)
        {
            var productList = new List<ViewStockProduct>();
            if (!string.IsNullOrEmpty(rowNumbers))
            {
                long rowNumber = 0;
                foreach (var id in rowNumbers.Split(','))
                {
                    rowNumber = Convert.ToInt64(id);
                    ViewStockProduct aProduct = db.ViewStockProducts.FirstOrDefault(a => a.RowNumber == rowNumber);
                    if (aProduct != null)
                    {
                        productList.Add(aProduct);
                    }
                }
            }
            else
            {
                productList = db.ViewStockProducts.Where(a => a.Quantity > 0).OrderBy(a => a.ProductName).ToList();
            }
            return PartialView(productList);
        }
        //stock list inventory
        public PartialViewResult StockListInventoryPartial(string rowNumbers)
        {
            var productList = new List<ViewProduct>();
            if (!string.IsNullOrEmpty(rowNumbers))
            {
                long rowNumber = 0;
                foreach (var id in rowNumbers.Split(','))
                {
                    rowNumber = Convert.ToInt64(id);
                    var aProduct = db.ViewProducts.FirstOrDefault(a => a.RowID == rowNumber);
                    productList.Add(aProduct);
                }
            }
            else
            {
                productList = db.ViewProducts.Where(a => a.Status == true).OrderBy(a => a.ProductName).ToList();
            }
            return PartialView(productList);
        }
        //negative stock list
        //public PartialViewResult NegativeStockList()
        //{
        //    return PartialView(db.ViewStockProducts.Where(a => a.Quantity < 0).OrderBy(a => a.ProductName).ToList());
        //}
        //*************stock edit start***********************
        public PartialViewResult StockEdit(int? StockId)
        {
            ViewBag.StockId = StockId;
            return PartialView();
        }
        public PartialViewResult StockEditPartial(int? StockId)
        {
            Stock aStock = db.Stocks.FirstOrDefault(a => a.StockId == StockId);
            StockModel aModel = new StockModel();
            aModel.StockId = aStock.StockId;
            aModel.ProductId = aStock.ProductId;
            aModel.Quantity = aStock.Quantity;
            return PartialView(aModel);
        }
        public JsonResult StockUpdate(StockModel model)
        {
            Stock aStock = db.Stocks.FirstOrDefault(a => a.StockId == model.StockId);
            aStock.Quantity = (decimal)model.Quantity;
            db.Entry(aStock).State = EntityState.Modified;
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

        //*************stock edit end***********************
        #endregion

        #region Fullfillment
        public PartialViewResult MinimumQuantityInfoList(int? count)
        {
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;
            ViewBag.MinimumQuantityPercent = minimumQuantityPercent;

            ViewBag.MinimumQuantityPercent = minimumQuantityPercent;
            List<ViewProduct> productList = new List<ViewProduct>();
            foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
            {
                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                if (stockProduct != null)
                {
                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                    {
                        productList.Add(product);
                    }
                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                    {
                        productList.Add(product);
                    }
                }
                else
                {
                    productList.Add(product);
                }
            }

            if (count > 0)
            {
                productList = productList.Take((int)count).ToList();
            }
            return PartialView(productList);
        }
        #endregion

        #region Supplier Debit pay
        public PartialViewResult SupplierDebitPay(long supplierId)
        {
            ViewBag.SupplierName = db.Suppliers.Find(supplierId).Name;
            ViewBag.DebitAmount = 0;
            if (db.SupplierDebits.Any(a => a.SupplierId == supplierId))
            {
                ViewBag.DebitAmount = db.SupplierDebits.FirstOrDefault(a => a.SupplierId == supplierId).Amount;
            }
            return PartialView();
        }
        public JsonResult SupplierDebitPaySave(ImportModel model)
        {
            try
            {
                //supplier debit payment save
                SupplierDebitPayment suppDebitPay = new SupplierDebitPayment();
                suppDebitPay.SupplierId = model.SupplierId;
                suppDebitPay.Amount = model.Payments.Sum(a => a.AmountPaid);
                suppDebitPay.CreatedBy = model.UserId;
                suppDebitPay.CreatedDate = DateTime.Now;
                db.SupplierDebitPayments.Add(suppDebitPay);

                //supplier debit amount save
                SupplierDebit suppDebit = new SupplierDebit();
                if(db.SupplierDebits.Any(a => a.SupplierId == model.SupplierId))
                {
                    suppDebit = db.SupplierDebits.FirstOrDefault(a => a.SupplierId == model.SupplierId);
                    suppDebit.Amount = suppDebit.Amount + suppDebitPay.Amount;
                    db.Entry(suppDebit).State = EntityState.Modified;
                }
                else
                {
                    suppDebit = new SupplierDebit();
                    suppDebit.SupplierId = model.SupplierId;
                    suppDebit.Amount = suppDebitPay.Amount;
                    db.SupplierDebits.Add(suppDebit);
                }

                db.SaveChanges();

                PaymentBody account;
                if (model.Payments != null)
                {
                    PaymentTransaction paymentTransaction;
                    foreach (var payment in model.Payments)
                    {
                        paymentTransaction = new PaymentTransaction();
                        paymentTransaction.PaymentId = suppDebitPay.Id;
                        paymentTransaction.SupplierId = model.SupplierId;
                        paymentTransaction.Type = 10; // Type 10 for supplier debit Payment
                        paymentTransaction.InOut = false;// Inout false for release payment
                        paymentTransaction.MethodId = 5; // method id 5 for purchase 
                        paymentTransaction.PaymentTypeId = payment.PaymentTypeId;
                        paymentTransaction.PaymentBodyId = payment.PaymentBodyId;
                        paymentTransaction.Amount = payment.AmountPaid;
                        paymentTransaction.TransactionNo = payment.TransactionNo;
                        paymentTransaction.IsCreditPayment = false;
                        paymentTransaction.Date = DateTime.Now;
                        paymentTransaction.CreatedBy = model.UserId;
                        db.PaymentTransactions.Add(paymentTransaction);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == payment.PaymentBodyId);
                        account.Balance = account.Balance - payment.AmountPaid;
                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                //increase supplier debit account amount
                account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == 19); // 19 is a supplier debit account;
                account.Balance = account.Balance + suppDebitPay.Amount;
                db.Entry(account).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Supplier Debit Win
        public PartialViewResult SupplierDebit(decimal? totalPrice, decimal? debitAmount, decimal? availableDebitAmount)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DebitAmount = debitAmount;
            ViewBag.AvailableDebitAmount = availableDebitAmount;
            return PartialView();
        }
        public PartialViewResult SupplierDebitPartial(decimal? totalPrice, decimal? debitAmount, decimal? availableDebitAmount)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DebitAmount = debitAmount;
            ViewBag.AvailableDebitAmount = availableDebitAmount;
            ViewBag.ValidAmount = 0;
            if (totalPrice >= availableDebitAmount)
            {
                ViewBag.ValidAmount = availableDebitAmount;
            }
            else
            {
                ViewBag.ValidAmount = totalPrice;
            }
            return PartialView();
        }
        #endregion

        #region FullfillMent
        public JsonResult GetProInFullFillMent(int tab, int? categoryId, int? subCategoryId, string text)
        {
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;

            var aList = new SelectList("", "");
            var list = new List<ViewProduct>();
            if (tab == 1) // all
            {
                if (categoryId > 0)
                {
                    if(subCategoryId > 0)
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a => a.CategoryId == categoryId && a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                            {

                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                                    {
                                        list.Add(product);
                                    }

                                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                                    {
                                        list.Add(product);
                                    }
                                }
                                else
                                {
                                    list.Add(product);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a => a.CategoryId == categoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true))
                            {
                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                                    {
                                        list.Add(product);
                                    }

                                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                                    {
                                        list.Add(product);
                                    }
                                }
                                else
                                {
                                    list.Add(product);
                                }

                            }
                        }
                    }
                }
                else
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                        if (stockProduct != null)
                        {
                            if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                            {
                                list.Add(product);
                            }
                            var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                            if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                            {
                                list.Add(product);
                            }
                        }
                        else
                        {
                            list.Add(product);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(text))
                {
                    aList = new SelectList(list.Where(a => a.Status == true && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
                else
                {
                    aList = new SelectList(list.Where(a => a.Status == true).Take(5)
                        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
            }
            else if(tab == 2) // red
            {
                list.Clear();
                if (categoryId > 0)
                {
                    if(subCategoryId > 0)
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a =>a.CategoryId == categoryId && a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                            {
                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                                    {
                                        list.Add(product);
                                    }
                                }
                                else
                                {
                                    list.Add(product);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a => a.CategoryId == categoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true))
                            {
                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                                    {
                                        list.Add(product);
                                    }
                                }
                                else
                                {
                                    list.Add(product);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                        if (stockProduct != null)
                        {
                            if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                            {
                                list.Add(product);
                            }
                        }
                        else
                        {
                            list.Add(product);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(text))
                {
                    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
                else
                {
                    aList = new SelectList(list.Where(a => a.Status == true).Take(5)
                        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
            }
            else if(tab == 3) // yellow
            {
                list.Clear();
                if (categoryId > 0)
                {
                    if(subCategoryId > 0)
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a =>a.CategoryId == categoryId && a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                            {
                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                                    {
                                        list.Add(product);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                        {
                            if (db.ViewProductCategories.Any(a => a.CategoryId == categoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true))
                            {
                                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                                if (stockProduct != null)
                                {
                                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                                    {
                                        list.Add(product);
                                    }
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                        if (stockProduct != null)
                        {
                            var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                            if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                            {
                                list.Add(product);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(text))
                {
                    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
                else
                {
                    aList = new SelectList(list.Where(a => a.Status == true).Take(5)
                        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                }
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryInFullFillMent(int tab, string text)
        {
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;

            var aList = new SelectList("", "");

            var list = new List<ViewProduct>();
            var categoryList = new List<ViewProductCategory>();
            if (tab == 1) // all
            {

                if (!string.IsNullOrEmpty(text))
                {
                    aList = new SelectList(db.ViewProductCategories.Where(a => a.CategoryName.ToLower().Contains(text.ToLower())).GroupBy(g => g.CategoryName).Take(5).Select(s => new { s.FirstOrDefault().CategoryId, s.FirstOrDefault().CategoryName }), "CategoryId", "CategoryName");
                }
                else
                {
                    aList = new SelectList(db.ViewProductCategories.Where(a => a.ProductCategoryStatus == true).GroupBy(g => g.CategoryName).Select(s => new { s.FirstOrDefault().CategoryId, s.FirstOrDefault().CategoryName }), "CategoryId", "CategoryName");
                }
            }
            else if (tab == 2) // red
            {
                list.Clear();
                foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                {
                    var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                    if (stockProduct != null)
                    {
                        if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                        {

                            var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                            if(proCategory != null)
                            {
                                categoryList.AddRange(proCategory);
                            }
                        }
                    }
                    else
                    {
                        var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                        if (proCategory != null)
                        {
                            categoryList.AddRange(proCategory);
                        }
                    }
                }

                aList = new SelectList(categoryList.Where(a => a.ProductCategoryStatus == true).GroupBy(g => g.CategoryName).Select(s => new { s.FirstOrDefault().CategoryId, s.FirstOrDefault().CategoryName }), "CategoryId", "CategoryName");

                //if (!string.IsNullOrEmpty(text))
                //{
                //    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
                //else
                //{
                //    aList = new SelectList(categoryList.Where(a => a.Status == true).Take(5)
                //        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}

            }
            else if (tab == 3) // yellow
            {
                list.Clear();
                foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                {
                    var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                    if (stockProduct != null)
                    {
                        var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                        if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                        {
                            var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                            if (proCategory != null)
                            {
                                categoryList.AddRange(proCategory);
                            }
                        }
                    }
                }
                aList = new SelectList(categoryList.Where(a => a.ProductCategoryStatus == true).GroupBy(g => g.CategoryName).Select(s => new { s.FirstOrDefault().CategoryId, s.FirstOrDefault().CategoryName }), "CategoryId", "CategoryName");
                //if (!string.IsNullOrEmpty(text))
                //{
                //    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
                //else
                //{
                //    aList = new SelectList(list.Where(a => a.Status == true).Take(5)
                //        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCategoryInFullFillMent(int tab, int categoryId)
        {
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;

            var aList = new SelectList("", "");

            var list = new List<ViewProduct>();
            var categoryList = new List<ViewProductCategory>();
            if (tab == 1) // all
            {
                aList = new SelectList(db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.SubCategoryId).Select(s => new { s.FirstOrDefault().SubCategoryId, s.FirstOrDefault().SubCategoryName }), "SubCategoryId", "SubCategoryName");
            }
            else if (tab == 2) // red
            {
                list.Clear();
                foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                {
                    var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                    if (stockProduct != null)
                    {
                        if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                        {

                            var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                            if (proCategory != null)
                            {
                                categoryList.AddRange(proCategory);
                            }
                        }
                    }
                    else
                    {
                        var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                        if (proCategory != null)
                        {
                            categoryList.AddRange(proCategory);
                        }
                    }
                }

                aList = new SelectList(categoryList.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.SubCategoryId).Select(s => new { s.FirstOrDefault().SubCategoryId, s.FirstOrDefault().SubCategoryName }), "SubCategoryId", "SubCategoryName");

                //if (!string.IsNullOrEmpty(text))
                //{
                //    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
                //else
                //{
                //    aList = new SelectList(categoryList.Where(a => a.Status == true).Take(5)
                //        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}

            }
            else if (tab == 3) // yellow
            {
                list.Clear();
                foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                {
                    var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                    if (stockProduct != null)
                    {
                        var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                        if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                        {
                            var proCategory = db.ViewProductCategories.Where(a => a.ProductId == product.ProductId);
                            if (proCategory != null)
                            {
                                categoryList.AddRange(proCategory);
                            }
                        }
                    }
                }

                aList = new SelectList(categoryList.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.SubCategoryId).Select(s => new { s.FirstOrDefault().SubCategoryId, s.FirstOrDefault().SubCategoryName }), "SubCategoryId", "SubCategoryName");

                //if (!string.IsNullOrEmpty(text))
                //{
                //    aList = new SelectList(list.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
                //else
                //{
                //    aList = new SelectList(list.Where(a => a.Status == true).Take(5)
                //        .Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
                //}
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult FullFillMentPrint(int? tab, string selectedIds, int? categoryId, int? subCategoryId)
        {
            ViewBag.Tab = tab;
            ViewBag.SelectedIds = selectedIds;
            ViewBag.CategoryId = categoryId;
            ViewBag.SubCategory = subCategoryId;
            return View();
        }
        #endregion

        public JsonResult GetProForMulti(int? categoryId, int? subCategoryId, string text)
        {
            var aList = new SelectList("", "");
            var list = new List<ViewProduct>();
            if (!string.IsNullOrEmpty(text))
            {
                aList = new SelectList(db.ViewProducts.Where(a =>a.Status == true && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
            }
            else
            {
                aList = new SelectList(db.ViewProducts.Where(a => a.Status == true).Take(5).Select(s => new { s.RowID, s.ProductName }), "RowID", "ProductName");
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductListForAutoComplete(string ids, string text)
        {
            List<ViewProduct> productList = new List<ViewProduct>();
            if (!string.IsNullOrEmpty(text))
            {
                productList = db.ViewProducts.Where(a => a.Status == true && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).ToList();
            }
            else
            {
                productList = db.ViewProducts.Where(a => a.Status == true).Take(5).ToList();
            }
            if (!string.IsNullOrEmpty(ids))
            {
                long rowId = 0;
                foreach (var id in ids.Split(','))
                {
                    rowId = Convert.ToInt64(id);
                    if(productList.Any(a => a.RowID == rowId))
                    {
                        productList.Remove(productList.FirstOrDefault(a => a.RowID == rowId));
                    }
                }
            }
            var alist = new SelectList(productList.Select(a => new { a.RowID, a.ProductName }), "RowID", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductInfo(long? rowId)
        {
            if (rowId > 0)
            {
                if (db.ViewProducts.Any(a => a.RowID == rowId))
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    return Json(new { product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem, product.ProductName }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryForDropdown(string text)
        {
            var aList = new SelectList("", "");
            var list = new List<Category>();
            if (!string.IsNullOrEmpty(text))
            {
                aList = new SelectList(db.Categories.Where(a => a.Status == true && a.CategoryName.ToLower().Contains(text.ToLower())).Take(5).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            }
            else
            {
                aList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            }
            return Json(aList, JsonRequestBehavior.AllowGet);
        }

        #region Waste Type
        public ActionResult WasteType()
        {
            return View();
        }
        public PartialViewResult WasteTypeCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult WasteTypeCreatePartial(int? id)
        {
            if (id > 0)
            {
                var wType = db.WasteTypes.Find(id);
                WasteTypeModelView model = new WasteTypeModelView();
                model.Id = wType.Id;
                model.TypeName = wType.TypeName;
                model.Type = wType.Type;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult WTypeSave(WasteTypeModelView model)
        {
            try
            {
                var wType = new WasteType();
                if (model.Id > 0)
                {
                    wType = db.WasteTypes.Find(model.Id);
                    wType.TypeName = model.TypeName;
                    wType.Type = model.Type;
                    wType.UpdatedBy = model.CreatedBy;
                    wType.UpdatedDate = now.Date;
                    db.Entry(wType).State = EntityState.Modified;
                }
                else
                {
                    wType = new WasteType();
                    wType.TypeName = model.TypeName;
                    wType.Type = model.Type;
                    wType.Status = true;
                    wType.CreatedBy = model.CreatedBy;
                    wType.CreatedDate = now.Date;
                    db.WasteTypes.Add(wType);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult WasteTypeList(int? status)
        {
            var list = new List<ViewWasteType>();
            if (status == 1) // active
            {
                list = db.ViewWasteTypes.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewWasteTypes.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewWasteTypes.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewWasteTypes.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.TypeName).ToList());
        }
        public JsonResult ChangeWTypeStatus(int id, int status, int createdBy)
        {
            try
            {
                var wType = db.WasteTypes.Find(id);
                wType.UpdatedBy = createdBy;
                wType.UpdatedDate = now.Date;
                if (status == 1)
                {
                    wType.Status = true; //active
                }
                else if (status == 0)
                {
                    wType.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    wType.Status = null; // delete
                }
                db.Entry(wType).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

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
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Stock waste create
        public ActionResult StockWasteCreate()
        {
            return View();
        }
        public JsonResult StockWasteSave(WasteModel model)
        {
            Waste waste = new Waste();
            WasteProduct wasteProduct;
            Stock stock;
            try
            {
                long count = 1;
                if(db.Wastes.Any())
                {
                    count = db.Wastes.Max(a => a.Id) + 1;
                }
                waste.ReffNo = "WV-" + count.ToString("0000");
                waste.CreatedBy = model.CreatedBy;
                waste.CreatedDate = DateTime.Now;
                waste.Status = 1;
                waste.Type = 1; // 1 for store waste
                db.Wastes.Add(waste);
                db.SaveChanges();
                foreach (var aWasteProduct in model.WasteData)
                {
                    wasteProduct = new WasteProduct();
                    wasteProduct.WasteId = waste.Id;
                    wasteProduct.ProductId = aWasteProduct.ProductId;
                    wasteProduct.DistributeId = aWasteProduct.DistributeId;
                    wasteProduct.ProductName = aWasteProduct.ProductName;
                    wasteProduct.Quantity = aWasteProduct.Quantity;
                    wasteProduct.WasteTypeId = aWasteProduct.WasteTypeId;
                    wasteProduct.Comments = aWasteProduct.Comment;
                    db.WasteProducts.Add(wasteProduct);

                    //decrease qty from stock
                    stock = db.Stocks.FirstOrDefault(a => a.ProductId == aWasteProduct.ProductId && a.DistributeId == aWasteProduct.DistributeId);
                    stock.Quantity = stock.Quantity - aWasteProduct.Quantity;
                    db.Entry(stock).State = EntityState.Modified;

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

        #region Warehouse waste
        public ActionResult WHWaste()
        {
            ViewBag.WasteCount = db.ViewWasteProducts.Where(a => a.WHReffId > 0).Count();
            return View();
        }
        public PartialViewResult WHWasteList(int? days, DateTime? from, DateTime? to, int? count, long? supplierId, bool? isPrint)
        {
            var list = new List<ViewWasteProduct>();
            DateTime? start = from;
            DateTime? end = to;
            if(supplierId > 0)
            {
                if (count > 0)
                {
                    list = db.ViewWasteProducts.Where(a => a.WHReffId > 0 && a.SupplierId == supplierId).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    start = null;
                    end = null;
                    list = db.ViewWasteProducts.Where(m => m.WHReffId > 0 && m.SupplierId == supplierId && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewWasteProducts.Where(m => m.WHReffId > 0 && m.SupplierId == supplierId && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            else
            {
                if (count > 0)
                {
                    list = db.ViewWasteProducts.Where(a => a.WHReffId > 0).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    start = null;
                    end = null;
                    DateTime countDate = DateTime.Now;
                    list = db.ViewWasteProducts.Where(m => m.WHReffId > 0 && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewWasteProducts.Where(m => m.WHReffId > 0 && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }

            ViewBag.IsPrint = isPrint;

            return PartialView(list);
        }
        public ActionResult WarehouseWasteCreate()
        {
            return View();
        }
        public JsonResult GetWarehouseProduct(string ids, string text, long? whId)
        {
            var list = new SelectList("");
            List<ViewStockWarehouse> productList = new List<ViewStockWarehouse>();
            if(whId > 0)
            {
                int quantity = db.StockWarehouses.Find(whId).Quantity;
                return Json(quantity, JsonRequestBehavior.AllowGet);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                productList = db.ViewStockWarehouses.Where(a => a.Quantity > 0 && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).ToList();
            }
            else
            {
                productList = db.ViewStockWarehouses.Where(a => a.Quantity > 0).Take(5).ToList();
            }
            if (!string.IsNullOrEmpty(ids))
            {
                //long whId = 0;
                foreach (var id in ids.Split(','))
                {
                    whId = Convert.ToInt64(id);
                    if (productList.Any(a => a.Id == whId))
                    {
                        productList.Remove(productList.FirstOrDefault(a => a.Id == whId));
                    }
                }
            }
            var alist = new SelectList(productList.Select(a => new { a.Id, ProductName = a.ProductName + " (" + a.WOVoucher + " | " + a.IndentVoucher + ")" }), "Id", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WHWasteSave(WasteModel model)
        {
            Waste waste = new Waste();
            WasteProduct wasteProduct;
            StockWarehouse wProduct;
            WorkOrderItem wItem;
            try
            {
                long count = 1;
                if (db.Wastes.Any())
                {
                    count = db.Wastes.Max(a => a.Id) + 1;
                }
                waste.ReffNo = "WV-" + count.ToString("0000");
                waste.CreatedBy = model.CreatedBy;
                waste.CreatedDate = DateTime.Now;
                waste.Status = 1;
                waste.Type = 2; // 1 for warehouse waste
                db.Wastes.Add(waste);
                db.SaveChanges();
                foreach (var aWasteProduct in model.WasteData)
                {
                    wasteProduct = new WasteProduct();
                    wasteProduct.WasteId = waste.Id;

                    //ware house
                    wProduct = db.StockWarehouses.Find(aWasteProduct.Id);
                    wProduct.Quantity = wProduct.Quantity - aWasteProduct.Quantity;
                    db.Entry(wProduct).State = EntityState.Modified;

                    //work order item
                    wItem = db.WorkOrderItems.FirstOrDefault(a => a.WorkOrderId == wProduct.WOId && a.IndentId == wProduct.IndentId && a.ProductId == wProduct.ProductId && a.DistributeId == wProduct.DistributeId);
                    wItem.ReceiveQty = wItem.ReceiveQty - aWasteProduct.Quantity;
                    wItem.RemainingQty = wItem.RemainingQty + aWasteProduct.Quantity;
                    db.Entry(wItem).State = EntityState.Modified;

                    //work order incomplete
                    if (db.WorkOrders.Find(wProduct.WOId).Status == 4)
                    {
                        var workOrder = db.WorkOrders.Find(wProduct.WOId);
                        workOrder.Status = 3; //incomplete
                        db.Entry(workOrder).State = EntityState.Modified;
                    }

                    wasteProduct.SupplierId = db.WorkOrders.Find(wProduct.WOId).SupplierId;
                    wasteProduct.ProductId = wProduct.ProductId;
                    wasteProduct.DistributeId = wProduct.DistributeId;
                    wasteProduct.ProductName = wProduct.ProductName;
                    wasteProduct.Quantity = aWasteProduct.Quantity;
                    wasteProduct.WasteTypeId = aWasteProduct.WasteTypeId;
                    if(aWasteProduct.WasteTypeId == 1)
                    {
                        wasteProduct.Status = 1;
                    }
                    wasteProduct.Comments = aWasteProduct.Comment;
                    wasteProduct.WHReffId = wProduct.Id;
                    db.WasteProducts.Add(wasteProduct);
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

        #region Waste History
        public ActionResult Waste()
        {
            ViewBag.AllCount = db.ViewWasteProducts.Count();
            ViewBag.PendingCount = db.ViewWasteProducts.Where(a => a.Status == 1).Count();
            ViewBag.CompleteCount = db.ViewWasteProducts.Where(a => a.Status == 2).Count();
            ViewBag.InvalidCount = db.ViewWasteProducts.Where(a => a.Status == 3).Count();
            ViewBag.PartialInvalidCount = db.ViewWasteProducts.Where(a => a.Status == 4).Count();

            //store count
            ViewBag.StoreAllCount = db.ViewWasteProducts.Where(a => a.Type == 1).Count();
            ViewBag.StorePendingCount = db.ViewWasteProducts.Where(a => a.Type == 1 && a.Status == 1).Count();
            ViewBag.StoreCompleteCount = db.ViewWasteProducts.Where(a => a.Type == 1 && a.Status == 2).Count();
            ViewBag.StoreInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 1 && a.Status == 3).Count();
            ViewBag.StorePartialInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 1 && a.Status == 4).Count();

            //Warehouse Count
            ViewBag.WHAllCount = db.ViewWasteProducts.Where(a => a.Type == 2).Count();
            ViewBag.WHPendingCount = db.ViewWasteProducts.Where(a => a.Type == 2 && a.Status == 1).Count();
            ViewBag.WHCompleteCount = db.ViewWasteProducts.Where(a => a.Type == 2 && a.Status == 2).Count();
            ViewBag.WHInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 2 && a.Status == 3).Count();
            ViewBag.WHPartialInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 2 && a.Status == 4).Count();

            //after sale count
            ViewBag.ASAllCount = db.ViewWasteProducts.Where(a => a.Type == 3).Count();
            ViewBag.ASPendingCount = db.ViewWasteProducts.Where(a => a.Type == 3 && a.Status == 1).Count();
            ViewBag.ASCompleteCount = db.ViewWasteProducts.Where(a => a.Type == 3 && a.Status == 2).Count();
            ViewBag.ASInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 3 && a.Status == 3).Count();
            ViewBag.ASPartialInvalidCount = db.ViewWasteProducts.Where(a => a.Type == 3 && a.Status == 4).Count();
            return View();
        }
        public PartialViewResult WasteList(int? days, DateTime? from, DateTime? to, int? count, int? status, int? type, bool? isPrint)
        {
            var list = new List<ViewWasteProduct>();
            DateTime? start = from;
            DateTime? end = to;
            if(status > 0)
            {
                if(type > 0)
                {
                    if (count > 0)
                    {
                        list = db.ViewWasteProducts.Where(a => a.Status == status && a.Type == type).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        start = null;
                        end = null;
                        list = db.ViewWasteProducts.Where(m => m.Status == status && m.Type == type && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewWasteProducts.Where(m => m.Status == status && m.Type == type && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewWasteProducts.Where(a => a.Status == status).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        start = null;
                        end = null;
                        list = db.ViewWasteProducts.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewWasteProducts.Where(m => m.Status == status && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
            }
            else
            {
                if(type > 0)
                {
                    if (count > 0)
                    {
                        list = db.ViewWasteProducts.Where(a => a.Type == type).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        start = null;
                        end = null;
                        list = db.ViewWasteProducts.Where(m => m.Type == type && DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewWasteProducts.Where(m => m.Type == type && DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewWasteProducts.Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        start = null;
                        end = null;
                        list = db.ViewWasteProducts.Where(m => DbFunctions.TruncateTime(m.CreatedDate) == DbFunctions.TruncateTime(countDate.Date)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewWasteProducts.Where(m => DbFunctions.TruncateTime(m.CreatedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.CreatedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                }
               
            }
            ViewBag.IsPrint = isPrint;
            return PartialView(list);
        }

        public PartialViewResult WasteInvalid(long id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult WasteInvalidPartial(long id)
        {
            var wasteProduct = db.WasteProducts.Find(id);
            return PartialView(wasteProduct);
        }

        public JsonResult WasteStatusChange(long id, int CreatedBy, int status, int? quantity)
        {
            try
            {
                var wasteProduct = db.WasteProducts.Find(id);
                wasteProduct.UpdatedBy = CreatedBy;
                wasteProduct.UpdatedDate = DateTime.Now;
                wasteProduct.Status = status;
                if(status == 3) // invalid
                {
                    int type = db.Wastes.Find(wasteProduct.WasteId).Type;
                    if(type == 1) //store
                    {
                        var store = db.Stocks.FirstOrDefault(a => a.ProductId == wasteProduct.ProductId && a.DistributeId == wasteProduct.DistributeId);
                        if (quantity > 0 && quantity != wasteProduct.Quantity) //partial invalid
                        {
                            wasteProduct.Status = 4; // partial invalid
                            wasteProduct.Quantity = wasteProduct.Quantity - (int)quantity;
                            store.Quantity = store.Quantity + (int)quantity;
                        }
                        else
                        {
                            store.Quantity = store.Quantity + wasteProduct.Quantity;
                        }
                        db.Entry(store).State = EntityState.Modified;
                    }
                    else if(type == 2) //warehouse
                    {
                        var warehouse = db.StockWarehouses.Find(wasteProduct.WHReffId);
                        var workOrder = db.WorkOrderItems.FirstOrDefault(a => a.WorkOrderId == warehouse.WOId && a.IndentId == warehouse.IndentId && a.ProductId == wasteProduct.ProductId && a.DistributeId == wasteProduct.DistributeId);

                        if(quantity > 0 && quantity != wasteProduct.Quantity)
                        {
                            wasteProduct.Status = 4; // partial invalid
                            //decrease waste product
                            wasteProduct.Quantity = wasteProduct.Quantity - (int)quantity;

                            //increase qty of warehouse
                            warehouse.Quantity = warehouse.Quantity + (int)quantity;

                            //increase recv qty of work order 
                            workOrder.ReceiveQty = workOrder.ReceiveQty + (int)quantity;
                            workOrder.RemainingQty = workOrder.RemainingQty - (int)quantity;
                        }
                        else
                        {
                            //increase qty of warehouse
                            warehouse.Quantity = warehouse.Quantity + wasteProduct.Quantity;

                            //increase recv qty of work order 
                            workOrder.ReceiveQty = workOrder.ReceiveQty + wasteProduct.Quantity;
                            workOrder.RemainingQty = workOrder.RemainingQty - wasteProduct.Quantity;
                        }
                        db.Entry(warehouse).State = EntityState.Modified;
                        db.Entry(workOrder).State = EntityState.Modified;
                    }
                    else if(type == 3) //after sale
                    {

                    }
                }
                else if(status == 2)
                {

                }
                db.Entry(wasteProduct).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error",JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region waste record print
        [EncryptedActionParameter]
        public ActionResult WastePrint(int? days, DateTime? from, DateTime? to, bool? isWHWastePrint, long? supplierId, int? status, int? type)
        {
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.IsWHWastePrint = isWHWastePrint;
            ViewBag.SupplierId = supplierId;
            ViewBag.Status = status;
            ViewBag.Type = type;
            return View();
        }
        #endregion
        public JsonResult GetSupplierList(string text)
        {
            var list = new SelectList("");
            if(!string.IsNullOrEmpty(text))
            {
                list = new SelectList(db.Suppliers.Where(a => a.Status == true && a.Type == true && a.Name.ToLower().Contains(text.ToLower())).OrderBy(o => o.Name).Take(5).Select(s => new { s.SupplierId, s.Name }), "SupplierId", "Name");
            }
            else
            {
                list = new SelectList(db.Suppliers.Where(a => a.Status == true && a.Type == true).OrderBy(o => o.Name).Take(5).Select(s => new { s.SupplierId, s.Name }), "SupplierId", "Name");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Delivery Charge
        public ActionResult DeliveryCharge()
        {
            return View();
        }
        public PartialViewResult DeliveryChargeCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult DeliveryChargeCreatePartial(int? id)
        {
            if(id > 0)
            {
                var deliveryCharge = db.DeliveryCharges.Find(id);
                DeliveryChargeModelView model = new DeliveryChargeModelView();
                model.Id = id;
                model.FromPrice = deliveryCharge.FromPrice;
                model.ToPrice = deliveryCharge.ToPrice;
                model.IsParcentile = deliveryCharge.IsPercentile;
                model.Amount = deliveryCharge.Amount;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult DeliveryChargeSave(DeliveryChargeModelView model)
        {
            try
            {
                DeliveryCharge deliveryCharge;
                if (model.Id > 0)
                {
                    deliveryCharge = db.DeliveryCharges.Find(model.Id);
                    deliveryCharge.FromPrice = (int)model.FromPrice;
                    deliveryCharge.ToPrice = (int)model.ToPrice;
                    deliveryCharge.Amount = model.Amount;
                    deliveryCharge.IsPercentile = model.IsParcentile;
                    deliveryCharge.UpdatedBy = model.CreatedBy;
                    deliveryCharge.UpdatedDate = now.Date;
                    db.Entry(deliveryCharge).State = EntityState.Modified;
                }
                else
                {
                    deliveryCharge = new DeliveryCharge();
                    deliveryCharge.FromPrice = (int)model.FromPrice;
                    deliveryCharge.ToPrice = (int)model.ToPrice;
                    deliveryCharge.Status = true;
                    deliveryCharge.Amount = model.Amount;
                    deliveryCharge.IsPercentile = model.IsParcentile;
                    deliveryCharge.CreatedBy = model.CreatedBy;
                    deliveryCharge.CreatedDate = now.Date;
                    db.DeliveryCharges.Add(deliveryCharge);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult DeliveryChargeList(int? status)
        {
            var list = new List<ViewDeliveryCharge>();
            if (status == 1) // active
            {
                list = db.ViewDeliveryCharges.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewDeliveryCharges.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewDeliveryCharges.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewDeliveryCharges.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.FromPrice).ToList());
        }

        public JsonResult CheckPriceAvailability(int? fromPrice, int? toPrice, int? price, int? id)
        {
            try
            {
                if(fromPrice > 0 && toPrice > 0)
                {
                    if(id > 0)
                    {
                        if (db.DeliveryCharges.Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.DeliveryCharges.Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.DeliveryCharges.Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice && a.Id != id) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice && a.Id != id)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (db.DeliveryCharges.Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if(db.DeliveryCharges.Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if(db.DeliveryCharges.Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if(price > 0)
                {
                    if(id > 0)
                    {
                        if(db.DeliveryCharges.Any(a => a.Id != id && a.FromPrice <= price && price <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if(db.DeliveryCharges.Any(a => a.FromPrice <= price && price <= a.ToPrice))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                
            }
            catch(Exception)
            {
                return Json("error",JsonRequestBehavior.AllowGet);
            }
            return Json(true,JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangeDeliveryChargeStatus(int id, int status, int createdBy)
        {
            try
            {
                var deliveryCharge = db.DeliveryCharges.Find(id);
                deliveryCharge.UpdatedBy = createdBy;
                deliveryCharge.UpdatedDate = now.Date;
                if (status == 1)
                {
                    deliveryCharge.Status = true; //active
                }
                else if (status == 0)
                {
                    deliveryCharge.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    deliveryCharge.Status = null; // delete
                }
                db.Entry(deliveryCharge).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Inventory Report
        public ActionResult Report()
        {
            DateTime today = DateTime.Today;
            decimal totalSaleDay = 0;
            decimal totalSaleMonth = 0;
            decimal customerDebt = 0;
            decimal supplierDebt = 0;

            //sale item (day)
            ViewBag.TopSaleItemDayList = new SelectList(db.ViewOrderTransactions.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(today) && a.Quantity > 0 && a.TransactionType == 1)
                .GroupBy(a => new { a.ProductId, a.DistributeId })
                .Select(s => new { s.FirstOrDefault().ProductName, Quantity = s.Sum(a => a.Quantity),}).OrderByDescending(a => a.Quantity).Take(10).ToList(), "Quantity", "ProductName");

            //sale item (month)
            ViewBag.TopSaleItemMonthList = new SelectList(db.ViewOrderTransactions.Where(a => a.Status == true && a.OrderDate.Value.Month == today.Month && a.OrderDate.Value.Year == today.Year && a.Quantity > 0 && a.TransactionType == 1)
                .GroupBy(a => new { a.ProductId, a.DistributeId })
                .Select(s => new { s.FirstOrDefault().ProductName, Quantity = s.Sum(a => a.Quantity), }).OrderByDescending(a => a.Quantity).Take(10).ToList(), "Quantity", "ProductName");

            //Total sale (day)
            if(db.ViewPosOrders.Any(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(today)))
            {
                totalSaleDay = db.ViewPosOrders.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(today)).Sum(a => a.InvoiceAmount);
                // if account refil or credit payment in pos then minus from total sale
                if ((db.ViewOrderTransactions.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(today)).Any(a => a.TransactionType != 1)))
                {
                    totalSaleDay = totalSaleDay - db.ViewOrderTransactions.Where(a => a.Status == true && DbFunctions.TruncateTime(a.OrderDate) == DbFunctions.TruncateTime(today) && a.TransactionType != 1).Sum(a => a.Price);
                }
            }
            ViewBag.TotalSale = totalSaleDay;

            //Total sale (month)
            if(db.ViewPosOrders.Any(a => a.Status == true && a.OrderDate.Month == today.Month && a.OrderDate.Year == today.Year))
            {
                totalSaleMonth = db.ViewPosOrders.Where(a => a.Status == true && a.OrderDate.Month == today.Month && a.OrderDate.Year == today.Year).Sum(a => a.InvoiceAmount);
                // if account refil or credit payment in pos then minus from total sale
                if ((db.ViewOrderTransactions.Where(a => a.Status == true && a.OrderDate.Value.Month == today.Month && a.OrderDate.Value.Year == today.Year).Any(a => a.TransactionType != 1)))
                {
                    totalSaleMonth = totalSaleMonth - db.ViewOrderTransactions.Where(a => a.Status == true && a.OrderDate.Value.Month == today.Month && a.OrderDate.Value.Year == today.Year && a.TransactionType != 1).Sum(a => a.Price);
                }
            }
            ViewBag.TotalSaleMonth = totalSaleMonth;

            //Customer debt
            if (db.OrderPayments.Any(a => a.DueAmount > 0))
            {
                customerDebt = (decimal)db.OrderPayments.Where(a => a.DueAmount > 0).Sum(a => a.DueAmount);
            }
            ViewBag.CustomerDebt = customerDebt;

            //supplier debt 
            if(db.StockImports.Any(a => a.DueAmount > 0))
            {
                supplierDebt = db.StockImports.Where(a => a.DueAmount > 0).Sum(a => a.DueAmount);
            }

            //warehouse debt
            if(db.WImports.Any(a => a.DueAmount > 0))
            {
                supplierDebt = supplierDebt + db.WImports.Where(a => a.DueAmount > 0).Sum(a => a.DueAmount);
            }
            ViewBag.SupplierDebt = supplierDebt;

            return View();
        }
        #endregion
    }
}