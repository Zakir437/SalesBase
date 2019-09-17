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
using System.Data;
using PointOfSale.ModelViews.Sales;

namespace PointOfSale.Controllers
{
    public class StockController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        //******************Import********************
        #region Import
        public ActionResult Import()
        {
            return View();
        }
        //Pay in import
        public PartialViewResult ImportPay(ImportModel model, long? orderTransId)
        {
            if(orderTransId > 0)
            {   
                var orderTransaction = db.PosOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransId);
                model.SupplierId = (long)orderTransaction.AssociateId;
                ImportModelItem item = new ImportModelItem();
                item.ProductName = orderTransaction.ProductName;
                item.Quantity = orderTransaction.Quantity;
                item.PeritemCost = orderTransaction.PerItemPrice;
                item.Cost = orderTransaction.Price;

                IList<ImportModelItem> itemList = new List<ImportModelItem>();
                itemList.Add(item);
                model.ImportData = itemList;
            }
            return PartialView(model);
        }
        public PartialViewResult ImportPayPartial(ImportModel model)
        {
            //ViewBag.TotalCost = totalCost;
            return PartialView(model);
        }
        public JsonResult ImportSave(ImportModel model, long? orderTransId)
        {
            Stock aStock;
            StockImport stImport;
            ImportTransaction importTransaction;
            Product aProduct;
            PaymentTransaction paymentTransaction;
            PaymentBody account;
            try
            {
                if(orderTransId > 0)
                {
                    var orderTransaction = db.PosOrderTransactions.FirstOrDefault(a => a.OrderTransactionId == orderTransId);
                    model.MethodId = 5; // 5 for import transaction 
                    model.SupplierId = (long)orderTransaction.AssociateId;
                    ImportModelItem item = new ImportModelItem();
                    item.ProductId = orderTransaction.ProductId;
                    item.ProductName = orderTransaction.ProductName;
                    item.Quantity = orderTransaction.Quantity;
                    item.PeritemCost = orderTransaction.PerItemPrice;
                    item.Cost = orderTransaction.Price;
                    if(orderTransaction.DistributeId > 0)
                    {
                        item.DistributeId = (long)orderTransaction.DistributeId;
                    }
                    IList<ImportModelItem> itemList = new List<ImportModelItem>();
                    itemList.Add(item);
                    model.ImportData = itemList;

                    orderTransaction.IsBorrowPaid = true;
                    db.Entry(orderTransaction).State = EntityState.Modified;
                }

                //save stock import
                stImport = new StockImport();
                stImport.RefferenceNo = "IMV-" + DateTime.Now.ToString("yyyyMMddHHmmssf");
                stImport.TotalCost = model.ImportData.Sum(a => a.Cost);
                if (model.Payments != null)
                {
                    stImport.PaidAmount = model.Payments.Sum(a => a.AmountPaid);
                }
                else
                {
                    stImport.PaidAmount = 0;
                }
                if (model.CreditAmount > 0)
                {
                    stImport.IsCredit = true;
                    stImport.DueAmount = model.CreditAmount;
                }
                else
                {
                    stImport.IsCredit = false;
                    stImport.DueAmount = 0;
                }
                stImport.Date = DateTime.Now;
                stImport.SupplierId = model.SupplierId;
                stImport.ImportBy = model.UserId;
                stImport.Comments = model.Comments;
                db.StockImports.Add(stImport);

                db.SaveChanges();

                if (model.Payments != null)
                {
                    foreach (var payment in model.Payments)
                    {
                        paymentTransaction = new PaymentTransaction();
                        paymentTransaction.PaymentId = stImport.StockImportId;
                        if(orderTransId > 0) //associate
                        {
                            paymentTransaction.AssociateId = stImport.SupplierId;
                            paymentTransaction.Type = 2; // Type 3 for associate import Payment
                        }
                        else
                        {
                            paymentTransaction.SupplierId = stImport.SupplierId;
                            paymentTransaction.Type = 3; // Type 3 for Import Payment
                        }
                        paymentTransaction.InOut = false;// Inout false for release payment
                        paymentTransaction.MethodId = model.MethodId;
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
                //save stock import transaction
                foreach (var aImport in model.ImportData)
                {
                    //import transaction save
                    importTransaction = new ImportTransaction();
                    importTransaction.StockImportId = stImport.StockImportId;
                    importTransaction.ProductId = aImport.ProductId;
                    importTransaction.ProductName = aImport.ProductName;
                    importTransaction.DistributeId = aImport.DistributeId;
                    importTransaction.Quantity = aImport.Quantity;
                    importTransaction.PeritemCost = aImport.PeritemCost;
                    importTransaction.Cost = aImport.Cost;
                    db.ImportTransactions.Add(importTransaction);

                    if (orderTransId == null || orderTransId == 0)
                    {
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

                        //Product cost update 
                        aProduct = db.Products.FirstOrDefault(a => a.ProductId == aImport.ProductId);
                        if (aProduct.IsDynamic == true && aProduct.PriceCheckBox == true) //if sub product exist with price
                        {
                            var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == aImport.ProductId && a.Id == aImport.DistributeId);
                            subProduct.Cost = aImport.PeritemCost;
                            db.Entry(subProduct).State = EntityState.Modified;
                        }
                        else
                        {
                            aProduct.Cost = aImport.PeritemCost;
                            db.Entry(aProduct).State = EntityState.Modified;
                        }

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        //Import credit pay
        public PartialViewResult ImportCreditPay(decimal totalDue, long? supplierId)
        {
            ViewBag.TotalDue = totalDue;
            ViewBag.SupplierName = db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId).Name;
            return PartialView();
        }
        public PartialViewResult ImportCreditPayPartial(decimal totalDue, bool isTotalPay)
        {
            ViewBag.TotalDue = totalDue;
            ViewBag.IsTotalPay = isTotalPay;
            return PartialView();
        }
        public JsonResult ImportCreditPaySave(IList<PaymentModel> Payments, long? ImportId, long? SupplierId, bool IsTotalPay, int UserId, int? MethodId)
        {
            PaymentTransaction aPayment;
            PaymentBody account;
            try
            {
                if (IsTotalPay == true)
                {
                    decimal amount = 0;
                    decimal amountPaid = 0;
                    foreach (var paymentTransaction in Payments)
                    {
                        amount = paymentTransaction.AmountPaid;
                        var creditList = db.StockImports.Where(a => a.SupplierId == SupplierId && a.DueAmount > 0).OrderBy(a => a.Date).ToList();
                        if (creditList != null)
                        {
                            foreach (var list in creditList)
                            {
                                if (amount > 0)
                                {
                                    var payment = db.StockImports.FirstOrDefault(a => a.StockImportId == list.StockImportId);
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
                                    payment.PaidAmount = payment.PaidAmount + amountPaid;
                                    db.Entry(payment).State = EntityState.Modified;

                                    aPayment = new PaymentTransaction();
                                    aPayment.PaymentId = list.StockImportId;
                                    aPayment.Type = 3; // Type 3 for Import Payment
                                    aPayment.InOut = false; // false for release Payment
                                    aPayment.MethodId = (int)MethodId;
                                    aPayment.PaymentTypeId = paymentTransaction.PaymentTypeId;
                                    aPayment.PaymentBodyId = paymentTransaction.PaymentBodyId;
                                    aPayment.Amount = amountPaid;
                                    aPayment.TransactionNo = paymentTransaction.TransactionNo;
                                    aPayment.IsCreditPayment = true;
                                    aPayment.Date = DateTime.Now;
                                    aPayment.CreatedBy = UserId;
                                    db.PaymentTransactions.Add(aPayment);


                                    //add amount in account balance
                                    account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == paymentTransaction.PaymentBodyId);
                                    account.Balance = account.Balance - paymentTransaction.AmountPaid;

                                    db.Entry(account).State = EntityState.Modified;



                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else
                {
                    var payment = db.StockImports.FirstOrDefault(a => a.StockImportId == ImportId);
                    payment.PaidAmount = payment.PaidAmount + payment.DueAmount;
                    payment.DueAmount = 0;
                    db.Entry(payment).State = EntityState.Modified;
                    foreach (var list in Payments)
                    {
                        aPayment = new PaymentTransaction();
                        aPayment.PaymentId = payment.StockImportId;
                        aPayment.Type = 3; // Type 3 for Import Payment
                        aPayment.InOut = false; // false for release Payment
                        aPayment.MethodId = (int)MethodId;
                        aPayment.PaymentTypeId = list.PaymentTypeId;
                        aPayment.PaymentBodyId = list.PaymentBodyId;
                        aPayment.Amount = list.AmountPaid;
                        aPayment.TransactionNo = list.TransactionNo;
                        aPayment.IsCreditPayment = true;
                        aPayment.Date = DateTime.Now;
                        aPayment.CreatedBy = UserId;
                        db.PaymentTransactions.Add(aPayment);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                        account.Balance = account.Balance - list.AmountPaid;

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
        //******************Import History ***********
        #region Import History
        public ActionResult ImportHistory()
        {
            ViewBag.Count = db.StockImports.Where(a => a.IsReturn != true).Count();
            return View();
        }
        public PartialViewResult ImportList(string importIds, int? count)
        {
            var importHistoryList = new List<ViewStockImport>();
            //var importHistoryList = db.ViewImportHistories.OrderByDescending(a => a.Date).ToList();
            if (!string.IsNullOrEmpty(importIds))
            {
                importHistoryList.Clear();
                foreach(var aImportId in importIds.Split(','))
                {
                    var Id = Convert.ToInt64(aImportId);
                    ViewStockImport aImportHistory = db.ViewStockImports.FirstOrDefault(a => a.StockImportId == Id);
                    importHistoryList.Add(aImportHistory);
                }
            }
            else if(count > 0)
            {
                importHistoryList = db.ViewStockImports.Where(a => a.IsReturn != true).OrderByDescending(a => a.Date).Take((int) count).ToList();
            }
            else
            {
                importHistoryList = db.ViewStockImports.Where(a => a.IsReturn != true).ToList();
            }
            return PartialView(importHistoryList);
        }
        //Import list for import multiselect
        public JsonResult GetImportList(string text)
        {
            var list = new SelectList("", "");
            if (!string.IsNullOrEmpty(text))
            {
                list = new SelectList(db.StockImports.Where(a => a.IsReturn != true && a.RefferenceNo.ToLower().Contains(text.ToLower())).OrderByDescending(o => o.Date).Take(5).Select(s => new { s.StockImportId, s.RefferenceNo }), "StockImportId", "RefferenceNo");
            }
            else
            {
                list = new SelectList(db.StockImports.Where(a => a.IsReturn != true).OrderByDescending(o => o.Date).Take(5).Select(s => new { s.StockImportId, s.RefferenceNo }), "StockImportId", "RefferenceNo");
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        // View Import transaction by import id
        [EncryptedActionParameter]
        public ActionResult ImportTransaction(long importId)
        {
            return View(db.ViewStockImports.FirstOrDefault(a=>a.StockImportId == importId));
        }
        public PartialViewResult ImportTransactionList(long importId, bool? isPrint)
        {
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            return PartialView(db.ViewImportTransactions.Where(a => a.StockImportId == importId && a.Quantity > 0).ToList());
        }
        public PartialViewResult ImportTransactionUpdate(long importTransactionId)
        {
            ViewBag.ImportTransactionId = importTransactionId;
            return PartialView();
        }
        public PartialViewResult ImportTransactionUpdatePartial(long importTransactionId)
        {
            ImportTransaction aImportTransaction = db.ImportTransactions.FirstOrDefault(a => a.ImportTransactionId == importTransactionId);
            ImportTransactionModel model = new ImportTransactionModel();
            model.ImportTransactionId = aImportTransaction.ImportTransactionId;
            model.Quantity = aImportTransaction.Quantity;
            model.Cost = aImportTransaction.Cost;
            return PartialView(model);
        }
        //Import transaction update save
        public JsonResult ImportUpdate(ImportTransactionModel model)
        {
            decimal quantity = 0;
            try
            {
                ImportTransaction aImportTransaction = db.ImportTransactions.FirstOrDefault(a => a.ImportTransactionId == model.ImportTransactionId);
                Stock aStock = db.Stocks.FirstOrDefault(a => a.ProductId == aImportTransaction.ProductId && a.DistributeId == aImportTransaction.DistributeId);
                StockImport aStockImport = db.StockImports.FirstOrDefault(a => a.StockImportId == aImportTransaction.StockImportId);
                Product aProduct;
                //check product quantity increase/decrease
                quantity = model.Quantity - aImportTransaction.Quantity;
                //import transaction save
                aImportTransaction.Quantity = model.Quantity;
                aImportTransaction.PeritemCost = model.Cost / model.Quantity;
                aImportTransaction.Cost = model.Cost;
                db.Entry(aImportTransaction).State = EntityState.Modified;

                //Product cost update 
                aProduct = db.Products.FirstOrDefault(a => a.ProductId == aImportTransaction.ProductId);
                if(aProduct.IsDynamic == true && aProduct.PriceCheckBox == true)
                {
                    var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == aImportTransaction.ProductId && a.Id == aImportTransaction.DistributeId);
                    subProduct.Cost = model.Cost / model.Quantity;
                    db.Entry(subProduct).State = EntityState.Modified;
                }
                else
                {
                    aProduct.Cost = model.Cost / model.Quantity;
                    db.Entry(aProduct).State = EntityState.Modified;
                }

                //update stock quantity of product
                aStock.Quantity = aStock.Quantity + quantity;
                db.Entry(aStock).State = EntityState.Modified;
                db.SaveChanges();

                List<ImportTransaction> importTransactionList = db.ImportTransactions.Where(a => a.StockImportId == aStockImport.StockImportId).ToList();
                //update Import total cost
                aStockImport.TotalCost = importTransactionList.Sum(a => a.Cost);
                db.Entry(aStockImport).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult ImportTransactionPrint(long stockImportId)
        {
            return View(db.ViewStockImports.FirstOrDefault( a=> a.StockImportId == stockImportId));
        }
        #endregion
        //******************Minimum Quantity**********
        #region Minimum Quantity
        public ActionResult MinimumQuantity()
        {
            int redTotal = 0;
            int yellowTotal = 0;
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;
            List<ViewProduct> productList = new List<ViewProduct>();

            //red and yellow total
            foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
            {
                var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                if (stockProduct != null)
                {
                    if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                    {
                        redTotal++;
                    }
                    var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                    if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                    {
                        yellowTotal++;
                    }
                }
                else
                {
                    redTotal++;
                }
            }

            ViewBag.RedTotal = redTotal;
            ViewBag.YellowTotal = yellowTotal;
            return View();
        }
        public PartialViewResult MinimumQuantityInfoList(int? count, int? tab, string selectedIds, int? categoryId, int? subCategoryId)
        {
            int? minimumQuantityPercent = db.MiscFuntions.FirstOrDefault(a => a.Status == true && a.Id == 3).MinimumQuantity;

            ViewBag.MinimumQuantityPercent = minimumQuantityPercent;
            List<ViewProduct> productList = new List<ViewProduct>();
            //var productList = db.Products.Where(a => a.Status == true).ToList();
            if(!string.IsNullOrEmpty(selectedIds))
            {
                foreach(var id in selectedIds.Split(','))
                {
                    long rowId = Convert.ToInt64(id);
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    if(product != null)
                    {
                        productList.Add(product);
                    }
                }
            }
            else if (tab == 2) // for red
            {
                if (categoryId > 0 && (subCategoryId == 0 || subCategoryId == null))
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
                                    productList.Add(product);
                                }
                            }
                            else
                            {
                                productList.Add(product);
                            }
                        }
                    }
                }
                else if(subCategoryId > 0)
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        if (db.ViewProductCategories.Any(a => a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                        {
                            var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                            if (stockProduct != null)
                            {
                                if (stockProduct.Quantity <= product.MinimumQuantity || stockProduct.Quantity <= 0)
                                {
                                    productList.Add(product);
                                }
                            }
                            else
                            {
                                productList.Add(product);
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
                                productList.Add(product);
                            }
                        }
                        else
                        {
                            productList.Add(product);
                        }
                    }
                }
            }
            else if(tab == 3) // for yellow
            {
                productList.Clear();
                if (categoryId > 0 && (subCategoryId == 0 || subCategoryId == null))
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
                                    productList.Add(product);
                                }
                            }
                        }
                    }
                }
                else if(subCategoryId > 0)
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        if (db.ViewProductCategories.Any(a => a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                        {
                            var stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId);
                            if (stockProduct != null)
                            {
                                var minimumQuantityWithPercent = product.MinimumQuantity + (product.MinimumQuantity * ((decimal)minimumQuantityPercent / 100));
                                if (stockProduct.Quantity <= minimumQuantityWithPercent && stockProduct.Quantity > product.MinimumQuantity)
                                {
                                    productList.Add(product);
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
                                productList.Add(product);
                            }
                        }
                    }
                }
            }
            else
            {
                if (categoryId > 0 && (subCategoryId == 0 || subCategoryId == null))
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
                    }

                    //foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    //{
                    //    if (db.ViewProductCategories.Any(a => a.CategoryId == categoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true))
                    //    {
                    //        productList.Add(product);
                    //    }
                    //}
                }
                else if(subCategoryId > 0)
                {
                    foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    {
                        if (db.ViewProductCategories.Any(a => a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
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
                    }

                    //foreach (var product in db.ViewProducts.Where(a => a.Status == true).ToList())
                    //{
                    //    if (db.ViewProductCategories.Any(a => a.SubCategoryId == subCategoryId && a.ProductId == product.ProductId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true))
                    //    {
                    //        productList.Add(product);
                    //    }
                    //}
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
                    //productList = db.ViewProducts.ToList();
                }
            }
            return PartialView(productList);
        }
        #endregion
        //******************Product Waste*************
        #region Waste
        public ActionResult Waste()
        {
            return View();
        }
        public JsonResult WasteSave(WasteModel model)
        {
            StockWaste waste = new StockWaste();
            WasteTransaction wasteTransaction;
            Stock stockProduct = new Stock();
            try
            {
                waste.WasteVoucherNo = "WV-" + DateTime.Now.ToString("yyyyMMddHHmmssf");
                waste.Comments = model.Comments;
                waste.CreatedBy = model.UserId;
                waste.CreatedDate = DateTime.Now;
                db.StockWastes.Add(waste);
                db.SaveChanges();
                foreach (var aWasteProduct in model.WasteData)
                {
                    stockProduct = db.Stocks.FirstOrDefault(a => a.ProductId == aWasteProduct.ProductId && a.DistributeId == aWasteProduct.DistributeId);
                    stockProduct.Quantity = stockProduct.Quantity - aWasteProduct.Quantity;
                    db.Entry(stockProduct).State = EntityState.Modified;

                    wasteTransaction = new WasteTransaction();
                    wasteTransaction.StockWasteId = waste.StockWasteId;
                    wasteTransaction.ProductId = aWasteProduct.ProductId;
                    wasteTransaction.DistributeId = aWasteProduct.DistributeId;
                    wasteTransaction.Quantity = aWasteProduct.Quantity;
                    db.WasteTransactions.Add(wasteTransaction);

                    db.SaveChanges();
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion
        //*****************Waste History**************
        #region Waste History
        public ActionResult WasteHistory()
        {
            return View();
        }
        //Get waste productlist for waste multiselect
        public JsonResult GetWasteProductList()
        {
            var wasteList = new SelectList(db.StockWastes.OrderByDescending(a => a.CreatedDate).Select(s => new { s.StockWasteId, s.WasteVoucherNo }), "StockWasteId", "WasteVoucherNo");
            return Json(wasteList, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult WasteProductList(string ids)
        {
            var wasteProductList = db.ViewStockWastes.OrderByDescending(a => a.CreatedDate).ToList();
            if (!string.IsNullOrEmpty(ids))
            {
                wasteProductList.Clear();
                foreach (var aStockWasteId in ids.Split(','))
                {
                    var Id = Convert.ToInt64(aStockWasteId);
                    ViewStockWaste aWasteProduct = db.ViewStockWastes.FirstOrDefault(a => a.StockWasteId == Id);
                    wasteProductList.Add(aWasteProduct);
                }
            }
            return PartialView(wasteProductList);
        }
        [EncryptedActionParameter]
        public ActionResult WasteTransaction(long stockWasteId)
        {
            return View(db.ViewStockWastes.FirstOrDefault(a=>a.StockWasteId == stockWasteId));
        }
        public PartialViewResult WasteTransactionList(long wasteId)
        {
            return PartialView(db.ViewWasteTransactions.Where(a=>a.StockWasteId == wasteId));
        }
        #endregion
        //*****************Waste Record***************
        #region Waste Record
        public ActionResult WasteRecord()
        {
            return View();
        }
        public PartialViewResult WasteRecordList()
        {
            return PartialView(db.ViewWasteTransactions.ToList());
        }
        #endregion
        //*****************Product Info***************
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
        //*****************Import Return***************
        #region Import Return 
        public PartialViewResult ReturnAlert(long importId)
        {
            var stockList = new List<ViewStockProduct>();
            var importList = db.ImportTransactions.Where(a => a.StockImportId == importId && a.Quantity > 0).ToList();
            foreach(var list in importList)
            {
                var stock = db.ViewStockProducts.FirstOrDefault(a => a.ProductId == list.ProductId && a.DistributeId == list.DistributeId);
                //if product not available in stock 
                if (list.Quantity > stock.Quantity)
                {
                    stockList.Add(stock);
                }
            }
            return PartialView(stockList);
        }
        public PartialViewResult ReturnPay(long? importId,decimal? returnQuantity, long? impTransId)
        {
            var import = new ViewStockImport();
            var importList = new List<ViewImportTransaction>();
            if (importId > 0)
            {
                import = db.ViewStockImports.FirstOrDefault(a => a.StockImportId == importId);
                importList = db.ViewImportTransactions.Where(a => a.StockImportId == importId && a.Quantity > 0).ToList();
                foreach (var list in importList)
                {
                    var stock = db.Stocks.FirstOrDefault(a => a.ProductId == list.ProductId);
                    if (list.Quantity > stock.Quantity)
                    {
                        list.Quantity = stock.Quantity;
                        list.Cost = list.Quantity * list.PeritemCost;
                    }
                }
                import.TotalCost = importList.Sum(a => a.Cost);
            }
            else if(impTransId > 0)
            {
                var imptrans = db.ViewImportTransactions.FirstOrDefault(a => a.ImportTransactionId == impTransId);
                imptrans.Quantity =(decimal)returnQuantity;
                imptrans.Cost = imptrans.Quantity * imptrans.PeritemCost;
                importList.Add(imptrans);

                import = db.ViewStockImports.FirstOrDefault(a => a.StockImportId == imptrans.StockImportId);
                import.TotalCost = imptrans.Cost;
            }
            ViewBag.ImportTransaction = importList;
            ViewBag.ImpTransId = impTransId;
            return PartialView(import);
        }
        public JsonResult ReturnSave(ImportModel model, long? impTransId)
        {
            try
            {
                PaymentTransaction paymentTrans;
                PaymentBody account;
                decimal totalPayment = 0;
                decimal totalCost = model.ImportData.Sum(a => a.Cost);
                var impReturn = new ImportReturn();
                impReturn.ImportId = model.ImportId;
                impReturn.TotalAmount = totalCost;
                impReturn.ReturnBy = model.UserId;
                impReturn.Date = DateTime.Now;
                db.ImportReturns.Add(impReturn);
                db.SaveChanges();

                var returnImport = new ImportModelItem();
                var stock = new Stock();
                ImpReturnTran impReturnTrans;
                var import = db.StockImports.FirstOrDefault(a => a.StockImportId == model.ImportId);
                if(impTransId > 0)
                {
                    var importTrans = db.ImportTransactions.FirstOrDefault(a => a.ImportTransactionId == impTransId);
                    returnImport = model.ImportData.FirstOrDefault(a => a.ProductId == importTrans.ProductId && a.DistributeId == importTrans.DistributeId);
                    importTrans.Quantity = importTrans.Quantity - returnImport.Quantity;
                    importTrans.ReturnQuantity = returnImport.Quantity;
                    importTrans.Cost = importTrans.Quantity * importTrans.PeritemCost;
                    importTrans.IsReturn = true;
                    importTrans.ReturnBy = model.UserId;
                    importTrans.ReturnDate = DateTime.Now;

                    db.Entry(importTrans).State = EntityState.Modified;

                    //Import return transaction save 
                    impReturnTrans = new ImpReturnTran();
                    impReturnTrans.ReturnId = impReturn.ReturnId;
                    impReturnTrans.ProductId = returnImport.ProductId;
                    impReturnTrans.DistributeId = returnImport.DistributeId;
                    impReturnTrans.ProductName = returnImport.ProductName;
                    impReturnTrans.Quantity = returnImport.Quantity;
                    impReturnTrans.PeritemCost = returnImport.PeritemCost;
                    impReturnTrans.Cost = returnImport.Cost;
                    db.ImpReturnTrans.Add(impReturnTrans);

                    //update stock quantity
                    stock = db.Stocks.FirstOrDefault(a => a.ProductId == importTrans.ProductId && a.DistributeId == importTrans.DistributeId);
                    stock.Quantity = stock.Quantity - returnImport.Quantity;
                    db.Entry(stock).State = EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    var importTrans = db.ImportTransactions.Where(a => a.StockImportId == model.ImportId && a.Quantity > 0).ToList();
                    foreach (var list in importTrans)
                    {
                        returnImport = model.ImportData.FirstOrDefault(a => a.ProductId == list.ProductId && a.DistributeId == list.DistributeId);
                        list.Quantity = list.Quantity - returnImport.Quantity;
                        list.ReturnQuantity = returnImport.Quantity;
                        list.Cost = list.Quantity * list.PeritemCost;
                        list.IsReturn = true;
                        list.ReturnBy = model.UserId;
                        list.ReturnDate = DateTime.Now;
                        db.Entry(list).State = EntityState.Modified;

                        //Import return transaction save 
                        impReturnTrans = new ImpReturnTran();
                        impReturnTrans.ReturnId = impReturn.ReturnId;
                        impReturnTrans.ProductId = returnImport.ProductId;
                        impReturnTrans.ProductName = returnImport.ProductName;
                        impReturnTrans.DistributeId = returnImport.DistributeId;
                        impReturnTrans.Quantity = returnImport.Quantity;
                        impReturnTrans.PeritemCost = returnImport.PeritemCost;
                        impReturnTrans.Cost = returnImport.Cost;
                        db.ImpReturnTrans.Add(impReturnTrans);

                        //update stock quantity
                        stock = db.Stocks.FirstOrDefault(a => a.ProductId == list.ProductId && a.DistributeId == list.DistributeId);
                        stock.Quantity = stock.Quantity - returnImport.Quantity;
                        db.Entry(stock).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                //save payment transaction
                if (model.Payments != null)
                {
                    totalPayment = model.Payments.Sum(a => a.AmountPaid);
                    foreach (var list in model.Payments)
                    {
                        paymentTrans = new PaymentTransaction();
                        paymentTrans.PaymentId = impReturn.ReturnId;
                        paymentTrans.SupplierId = import.SupplierId;
                        paymentTrans.Type = 4; // Type 4 for Import return payment;
                        paymentTrans.InOut = true; // true for receive payment
                        paymentTrans.MethodId = model.MethodId;
                        paymentTrans.PaymentTypeId = list.PaymentTypeId;
                        paymentTrans.PaymentBodyId = list.PaymentBodyId;
                        paymentTrans.Amount = list.AmountPaid;
                        paymentTrans.IsCreditPayment = false;
                        paymentTrans.TransactionNo = list.TransactionNo;
                        paymentTrans.CreatedBy = model.UserId;
                        paymentTrans.Date = DateTime.Now;
                        db.PaymentTransactions.Add(paymentTrans);

                        //add amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                        account.Balance = account.Balance + list.AmountPaid;

                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                import.TotalCost = db.ImportTransactions.Where(a => a.StockImportId == model.ImportId).Sum(a => a.Cost);
                if(model.CreditAmount > 0)
                {
                    import.DueAmount = import.DueAmount - model.CreditAmount;
                }
                if(totalPayment > 0)
                {
                    import.PaidAmount = import.PaidAmount - totalPayment;
                }
                if (import.TotalCost == 0)
                {
                    import.IsReturn = true;
                    import.ReturnBy = model.UserId;
                    import.ReturnDate = DateTime.Now;
                }
                db.Entry(import).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //Single return alert
        public PartialViewResult SinglReturnAlert(long impTransId)
        {
            var stock = new ViewStockProduct();
            var impTrans = db.ImportTransactions.FirstOrDefault(a => a.ImportTransactionId == impTransId && a.Quantity > 0);
            stock = db.ViewStockProducts.FirstOrDefault(a => a.ProductId == impTrans.ProductId && a.DistributeId == impTrans.DistributeId);
            ViewBag.ReturnQuantity = impTrans.Quantity;
            if (impTrans.Quantity > stock.Quantity)
            {
                ViewBag.ReturnQuantity = stock.Quantity;
            }
            else
            {
                stock = new ViewStockProduct();
            }
            return PartialView(stock);
        }

        #endregion
        //*****************Product borrow info***************
        #region Borrow Products
        public ActionResult BorrowProducts()
        {
            ViewBag.ShopList = new SelectList(db.Suppliers.Where(a => a.Status == true && a.Type == false).Select(s => new { s.Name, s.SupplierId }), "SupplierId", "Name");
            return View();
        }
        public PartialViewResult BorrowProductList(int status, string associateIds, bool? isPrint)
        {
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            List<ViewBorrowProduct> borrowProducts = new List<ViewBorrowProduct>();
            if(!string.IsNullOrEmpty(associateIds))
            {
                foreach(var id in associateIds.Split(','))
                {
                    long associateId = Convert.ToInt64(id);
                    if(status == 1)
                    {
                       var products = db.ViewBorrowProducts.Where(a => a.AssociateId == associateId).ToList();
                       borrowProducts.AddRange(products);
                    }
                    else if(status == 2)
                    {
                        var products = db.ViewBorrowProducts.Where(a => a.AssociateId == associateId && a.IsBorrowPaid == true).ToList();
                        borrowProducts.AddRange(products);
                    }
                    else if(status == 3)
                    {
                        var products = db.ViewBorrowProducts.Where(a => a.AssociateId == associateId && a.IsBorrowPaid != true).ToList();
                        borrowProducts.AddRange(products);
                    }
                }
            }
            else
            {
                if (status == 1) // all
                {
                    borrowProducts = db.ViewBorrowProducts.ToList();
                }
                else if (status == 2) // paid
                {
                    borrowProducts = db.ViewBorrowProducts.Where(a => a.IsBorrowPaid == true).ToList();
                }
                else if (status == 3) // unpaid
                {
                    borrowProducts = db.ViewBorrowProducts.Where(a => a.IsBorrowPaid != true).ToList();
                }
            }
            return PartialView(borrowProducts);
        }
        [EncryptedActionParameter]
        public ActionResult BorrowProductsPrint(int status, string associateIds)
        {
            ViewBag.Status = status;
            ViewBag.AssociateIds = associateIds;
            return View();
        }

        #endregion
    }
}