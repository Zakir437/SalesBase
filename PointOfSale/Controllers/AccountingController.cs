using PointOfSale.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Models;
using System.Configuration;
using PointOfSale.Helpers;
using System.Net;
using System.Data.Entity;
using PointOfSale.ModelViews.Accounting;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PointOfSale.Controllers
{
    public class AccountingController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        #region Account Group 
        public ActionResult AccountGroup()
        {
            return View();
        }
        public PartialViewResult AddAccountGroup(int? Acc_Group_Id)
        {
            AccountGroupModelView model = new AccountGroupModelView();
            if (Acc_Group_Id != null)
            {
                AccountName acc_grp = db.AccountNames.Find(Acc_Group_Id);
                model.AccId = acc_grp.AccId;
                model.AccountName = acc_grp.Name;
                model.Description = acc_grp.Description;
                model.Balance = acc_grp.Balance;
                model.Acc_Type = acc_grp.AccType;
                model.Acc_CashType = acc_grp.CashType;
            }
            ViewBag.Acc_Type = new SelectList(db.AccTypes, "AccTypeId", "Name", model.Acc_Type);
            List<SelectListItem> allUserList = new SelectList(db.Users.Where(m => m.Status == true).Select(s => new { UserId = s.Id, UserName = s.FirstName + " " + s.LastName  }), "UserId", "UserName").ToList();
            ViewBag.allUserList = allUserList;
            return PartialView(model);
        }
        public JsonResult SaveAccGroup(AccountGroupModelView model, string AllUserId)
        {
            try
            {
                AccountName acc_grp = new AccountName();
                acc_grp.Name = model.AccountName;
                acc_grp.CashType = model.Acc_CashType;
                acc_grp.Balance = model.Balance;
                if (acc_grp.CashType == 1 || acc_grp.CashType == 3)
                {
                    acc_grp.TotalCreditLimit = model.TotalCreditLimit;
                    acc_grp.TransactionHigestAmount = model.TransactionHigestAmount;
                    acc_grp.NoOfTranPerMonth = model.NoOfTranPerMonth;
                    acc_grp.AccType = Convert.ToInt32(model.Acc_Type);
                }
                acc_grp.Description = model.Description;
                acc_grp.CreatedBy = model.CreatedBy;
                acc_grp.CreatedDate = DateTime.Now;
                db.AccountNames.Add(acc_grp);

                db.SaveChanges();
                if (AllUserId != null)
                {
                    foreach (var id in AllUserId.Split(','))
                    {
                        AccNameAssignedWithUser accname = new AccNameAssignedWithUser();
                        accname.UserId = Convert.ToInt32(id);
                        accname.AccId = acc_grp.AccId;
                        accname.UserType = 1;
                        accname.AssignedDate = now;
                        accname.AssignedBy = model.CreatedBy;
                        db.AccNameAssignedWithUsers.Add(accname);
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
        public PartialViewResult _ShowAllAcc_Group()
        {
            return PartialView(/*db.ViewAccountNames.ToList()*/);
        }
        #endregion
        #region Account Details
        [EncryptedActionParameter]
        public ActionResult AccountDetails(int? AccId)
        {
            if (AccId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.ViewAccounts.FirstOrDefault(m => m.PaymentBodyId == AccId);
            return View(model);
        }
        public PartialViewResult PaymentTransaction(int accId, int? methodId, int? days, DateTime? from, DateTime? to, int? count, int? inout, bool? isPrint)
        {
            //if payment transaction view in method page it show type of payment
            ViewBag.TransForMethod = false;
            DateTime? start = from;
            DateTime? end = to;
            bool isIn = false;
            if(inout == 1)
            {
                isIn = true;
            }
            var list =new List<ViewPayment>();
            if(accId > 0 && methodId == 0)
            {
                if(inout > 0)
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.PaymentBodyId == accId && a.Status != false && a.InOut == isIn).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.Status != false && m.InOut == isIn && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.Status != false && m.InOut == isIn && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.PaymentBodyId == accId && a.Status != false).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                    }
                }
            }
            else if(accId > 0 && methodId > 0)
            {
                if(inout > 0)
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.PaymentBodyId == accId && a.InOut == isIn && a.MethodId == methodId && a.Status != false).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.InOut == isIn && m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.InOut == isIn && m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.PaymentBodyId == accId && a.MethodId == methodId && a.Status != false).OrderByDescending(a => a.Date).Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                    }
                }
            }
            else if(methodId > 0 && accId == 0)
            {
                ViewBag.TransForMethod = true;
                if (count > 0)
                {
                    list = db.ViewPayments.Where(a => a.MethodId == methodId && a.Status != false).OrderByDescending(a => a.Date).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPayments.Where(m => m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPayments.Where(m =>  m.MethodId == methodId && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                }
            }
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            return PartialView(list);
        }
        //Payment Transaction Print
        [EncryptedActionParameter]
        public ActionResult PaymentTransactionPrint(int? accId, int? methodId, int? days, DateTime? from, DateTime? to, int? inout)
        {
            ViewBag.Inout = inout;
            ViewBag.MethodId = methodId;
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.AccId = accId;
            var list = new ViewAccount();
            if(accId > 0)
            {
                list = db.ViewAccounts.FirstOrDefault(a => a.PaymentBodyId == accId);
            }
            return View(list);
        }
        #endregion
        #region Payment Method Details
        [EncryptedActionParameter]
        public ActionResult PaymentMethodDetails(int methodId)
        {
            return View(db.PaymentMethods.FirstOrDefault(a => a.Id == methodId));
        }
        #endregion
        #region Salary pay
        public ActionResult PaySalary()
        {
            return View();
        }
        public PartialViewResult UserListForSalary()
        {
            return PartialView(db.ViewUserLists.Where(a =>a.Status == 1 && a.JoinDate != null && a.SalaryPackageId > 0 && a.WorkingScheduleId > 0 && a.HolidayPackId > 0 ).ToList());
        }
        public PartialViewResult ShowSalaryPackageDetails(int salaryId, string detailsType)
        {
            ViewBag.Type = detailsType;
            return PartialView(db.Salaries.Find(salaryId));
        }
        #region Single salary pay
        public PartialViewResult SalaryPayWin(int? transId, int? userId, bool? isAllSalaryPay)
        {
            ViewBag.TransId = transId;
            ViewBag.UserId = userId;
            ViewBag.IsAllPay = isAllSalaryPay;
            var list = new List<ViewEmpSalary>();
            ViewEmpSalary salary;
            if(transId > 0)
            {
                
                salary = db.ViewEmpSalaries.FirstOrDefault(a => a.Id == transId);
                list.Add(salary);
            }
            else if(userId > 0)
            {
                if (isAllSalaryPay == true) // current + due
                {
                    list = db.ViewEmpSalaries.Where(a => a.UserId == userId && (a.PaidAmount == 0 || a.DueAmount > 0)).ToList();
                }
                else
                {
                    list = db.ViewEmpSalaries.Where(a => a.UserId == userId && a.DueAmount > 0).ToList();
                }
            }
            return PartialView(list.OrderBy(a => a.StartDate).ToList());
        }
        // salary pay save
        public JsonResult SalaryPaySave(int? UserId, int? TransId, int MethodId, int PaidBy, IList<PaymentModel> Payments, bool? isAllPay)
        {
            try
            {
                PaymentTransaction paymentTrans;
                PaymentBody account;
                bool isCreditPayment = true;
                if (TransId > 0)
                {
                    EmpSalaryPayment empSalary;
                    empSalary = db.EmpSalaryPayments.Find(TransId);
                    if(empSalary.PaidAmount == 0 && empSalary.DueAmount == 0)
                    {
                        isCreditPayment = false;
                    }
                    empSalary.PaidDate = now.Date;
                    empSalary.PaidBy = PaidBy;
                    empSalary.PaidAmount = empSalary.PaidAmount + Payments.Sum(a => a.AmountPaid);
                    empSalary.DueAmount = empSalary.TotalAmount - empSalary.PaidAmount;
                    if(empSalary.DueAmount == 0)
                    {
                        empSalary.Status = 1;
                    }
                    db.Entry(empSalary).State = EntityState.Modified;
                    foreach(var list in Payments)
                    {
                        paymentTrans = new PaymentTransaction();
                        paymentTrans.PaymentId = empSalary.Id;
                        paymentTrans.Type = 5; // Type 5 for salary payment 
                        paymentTrans.InOut = false; // InOut false for release payment
                        paymentTrans.MethodId = MethodId;
                        paymentTrans.PaymentTypeId = list.PaymentTypeId;
                        paymentTrans.PaymentBodyId = list.PaymentBodyId;
                        paymentTrans.Amount = list.AmountPaid;
                        paymentTrans.TransactionNo = list.TransactionNo;
                        paymentTrans.Date = now.Date;
                        paymentTrans.CreatedBy = PaidBy;
                        paymentTrans.IsCreditPayment = isCreditPayment;
                        paymentTrans.Status = true;
                        db.PaymentTransactions.Add(paymentTrans);

                        //decrease amount in account balance
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                        account.Balance = account.Balance - list.AmountPaid;
                        db.Entry(account).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                else if (UserId > 0)
                {
                    decimal amount = 0;
                    decimal amountPaid = 0;
                    var empSalaryList = new List<EmpSalaryPayment>();
                    foreach (var list in Payments)
                    {
                        amount = list.AmountPaid;
                        if(isAllPay == true)
                        {
                            empSalaryList = db.EmpSalaryPayments.Where(a => a.UserId == UserId && (a.PaidAmount == 0 || a.DueAmount > 0)).OrderBy(a => a.StartDate).ToList();
                        }
                        else
                        {
                            empSalaryList = db.EmpSalaryPayments.Where(a => a.UserId == UserId && a.DueAmount > 0).OrderBy(a => a.StartDate).ToList();
                        }
                        if (empSalaryList != null)
                        {
                            foreach (var empSalary in empSalaryList)
                            {
                                if (amount > 0)
                                {
                                    if(empSalary.PaidAmount == 0 && empSalary.DueAmount == 0)
                                    {
                                        isCreditPayment = false;
                                        if (amount >= empSalary.TotalAmount)
                                        {
                                            amountPaid = empSalary.TotalAmount;
                                            amount = amount - empSalary.TotalAmount;
                                        }
                                        else
                                        {
                                            amountPaid = amount;
                                            empSalary.DueAmount = empSalary.TotalAmount - amount;
                                            amount = 0;
                                        }
                                    }
                                    else
                                    {
                                        isCreditPayment = true;
                                        if (amount >= empSalary.DueAmount)
                                        {
                                            amountPaid = empSalary.DueAmount;
                                            amount = amount - empSalary.DueAmount;
                                            empSalary.DueAmount = 0;
                                        }
                                        else
                                        {
                                            amountPaid = amount;
                                            empSalary.DueAmount = empSalary.DueAmount - amount;
                                            amount = 0;
                                        }
                                    }
                                    empSalary.PaidAmount = empSalary.PaidAmount + amountPaid;
                                    empSalary.PaidDate = now.Date;
                                    empSalary.PaidBy = PaidBy;
                                    if(empSalary.DueAmount == 0)
                                    {
                                        empSalary.Status = 1;
                                    }
                                    db.Entry(empSalary).State = EntityState.Modified;

                                    paymentTrans = new PaymentTransaction();
                                    paymentTrans.PaymentId = empSalary.Id;
                                    paymentTrans.Type = 5; // Type 5 for salary payment 
                                    paymentTrans.InOut = false; // InOut false for release payment
                                    paymentTrans.MethodId = MethodId;
                                    paymentTrans.PaymentTypeId = list.PaymentTypeId;
                                    paymentTrans.PaymentBodyId = list.PaymentBodyId;
                                    paymentTrans.Amount = amountPaid;
                                    paymentTrans.TransactionNo = list.TransactionNo;
                                    paymentTrans.Date = now.Date;
                                    paymentTrans.CreatedBy = PaidBy;
                                    paymentTrans.Status = true;
                                    paymentTrans.IsCreditPayment = isCreditPayment;
                                    db.PaymentTransactions.Add(paymentTrans);

                                    //decrease amount in account balance
                                    account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                                    account.Balance = account.Balance - amountPaid;
                                    db.Entry(account).State = EntityState.Modified;

                                    db.SaveChanges();
                                }
                            }
                        }
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
        #region PaySheet salary pay
        public PartialViewResult TotalSalaryPayWin(long transId)
        {
            return PartialView(db.ViewPaySheets.FirstOrDefault(a => a.Id == transId));
        }
        public JsonResult TotalSalaryPaySave(int CreatedBy, int MethodId, long TransId, IList<PaymentModel> Payments)
        {
            try
            {
                var xmlFileInfo = db.PaySheets.Find(TransId);
                xmlFileInfo.IsPaid = true;
                xmlFileInfo.PaidBy = CreatedBy;
                xmlFileInfo.PaidDate = now.Date;
                db.Entry(xmlFileInfo).State = EntityState.Modified;
                PaymentTransaction paymentTrans;
                PaymentBody account;
                if (Payments != null)
                {
                    foreach (var list in Payments)
                    {
                        paymentTrans = new PaymentTransaction();
                        paymentTrans.PaymentId = TransId;
                        paymentTrans.Type = 6; // Type 6 for salary paysheet payment 
                        paymentTrans.InOut = false; // InOut false for release payment
                        paymentTrans.MethodId = MethodId;
                        paymentTrans.PaymentTypeId = list.PaymentTypeId;
                        paymentTrans.PaymentBodyId = list.PaymentBodyId;
                        paymentTrans.Amount = list.AmountPaid;
                        paymentTrans.TransactionNo = list.TransactionNo;
                        paymentTrans.Date = now.Date;
                        paymentTrans.CreatedBy = CreatedBy;
                        paymentTrans.IsCreditPayment = false;
                        paymentTrans.Status = true;
                        db.PaymentTransactions.Add(paymentTrans);

                        //decrease amount in account balance
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
        public JsonResult SalaryPayApproved(long transId, int createdBy)
        {
            try
            {
                PaymentTransaction paymentTrans;
                EmpSalaryPayment empSalary;
                long empSalaryId = 0;
                var paySheetPaymentTrans = db.PaymentTransactions.FirstOrDefault(a => a.PaymentId == transId && a.Type == 6 && a.MethodId == 2);
                var paySheet = db.PaySheets.Find(transId);
                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), paySheet.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                foreach (XElement ex in xmlList)
                {
                    empSalaryId = Convert.ToInt64(ex.Element("Id").Value);
                    //Payment Transaction update
                    paymentTrans = new PaymentTransaction();
                    paymentTrans.PaymentId = empSalaryId;
                    paymentTrans.Type = 5; // Type 5 for salary payment 
                    paymentTrans.InOut = false; // InOut false for release payment
                    paymentTrans.MethodId = 2; // 2 for salary 
                    paymentTrans.PaymentTypeId = paySheetPaymentTrans.PaymentTypeId;
                    paymentTrans.PaymentBodyId = paySheetPaymentTrans.PaymentBodyId;
                    paymentTrans.Amount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                    paymentTrans.TransactionNo = paySheetPaymentTrans.TransactionNo;
                    paymentTrans.Date = now.Date;
                    paymentTrans.CreatedBy = createdBy;
                    paymentTrans.IsCreditPayment = false;
                    paymentTrans.PaySheetId = transId;
                    paymentTrans.Status = true;
                    db.PaymentTransactions.Add(paymentTrans);

                    //Salary Transaction update
                    empSalary = db.EmpSalaryPayments.Find(empSalaryId);
                    empSalary.PaidAmount = empSalary.TotalAmount;
                    empSalary.PaidBy = createdBy;
                    empSalary.PaidDate = now.Date;
                    empSalary.Status = 1;
                    db.Entry(empSalary).State = EntityState.Modified;

                    //Xml file update
                    ex.Remove();
                    ex.Element("Status").SetValue(empSalary.Status);
                    ex.Element("PaidAmount").SetValue(empSalary.TotalAmount);
                    ex.Element("PaidBy").SetValue(createdBy);
                    ex.Element("PaidDate").SetValue(now.Date);
                    xmlDoc.Element("ArrayOfEmpSalaryXmlModelView").Add(ex);
                    db.SaveChanges();
                }
                xmlDoc.Save(Paths);

                //paysheet update
                paySheet.IsApproved = true;
                paySheet.ApprovedBy = createdBy;
                paySheet.ApprovedDate = now.Date;
                db.Entry(paySheet).State = EntityState.Modified;

                //paysheet payment transaction update;
                paySheetPaymentTrans.Status = false;
                db.Entry(paySheetPaymentTrans).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Pay complete
        [EncryptedActionParameter]
        public ActionResult PayComplete(long paySheetId)
        {
            ViewBag.PaySheetId = paySheetId;
            return View();
        }
        public PartialViewResult PaySheetList(long paySheetId)
        {
            var empSalaryList = new List<EmpSalaryXmlModelView>();
            EmpSalaryXmlModelView empSalary;
            if (paySheetId > 0)
            {
                var paySheetInfo = db.PaySheets.Find(paySheetId);

                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), paySheetInfo.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                foreach (XElement ex in xmlList)
                {
                    empSalary = new EmpSalaryXmlModelView();
                    empSalary.Id = Convert.ToInt64(ex.Element("Id").Value);
                    empSalary.TotalAmount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                    empSalary.ActualSalary = Convert.ToDecimal(ex.Element("ActualSalary").Value);
                    empSalary.TotalPresent = Convert.ToInt32(ex.Element("TotalPresent").Value);
                    empSalary.TotalPaidLeave = Convert.ToInt32(ex.Element("TotalPaidLeave").Value);
                    empSalary.TotalWorkingDays = Convert.ToInt32(ex.Element("TotalWorkingDays").Value);
                    empSalary.TotalHoliday = Convert.ToInt32(ex.Element("TotalHoliday").Value);
                    empSalary.DueAmount = Convert.ToDecimal(ex.Element("DueAmount").Value);
                    empSalary.PaidAmount = Convert.ToDecimal(ex.Element("PaidAmount").Value);
                    empSalary.StartDate = Convert.ToDateTime(ex.Element("StartDate").Value);
                    empSalary.EndDate = Convert.ToDateTime(ex.Element("EndDate").Value);
                    empSalary.GeneratedDate = Convert.ToDateTime(ex.Element("GeneratedDate").Value);
                    empSalary.UserId = Convert.ToInt32(ex.Element("UserId").Value);
                    empSalary.EmpName = ex.Element("EmpName").Value;
                    empSalary.Picture = ex.Element("Picture").Value;
                    empSalary.AssignUserId = Convert.ToInt32(ex.Element("AssignUserId").Value);
                    empSalary.XmlGenerateDate = Convert.ToDateTime(ex.Element("XmlGenerateDate").Value);
                    empSalary.XmlGenerateBy = Convert.ToInt32(ex.Element("XmlGenerateBy").Value);

                    empSalaryList.Add(empSalary);
                }
            }
            return PartialView(empSalaryList);
        }
        [EncryptedActionParameter]
        public ActionResult UnPaid(string unPaidIds, long paySheetId)
        {
            ViewBag.UnpaidIds = unPaidIds;
            ViewBag.PaySheetId = paySheetId;
            return View();
        }
        public PartialViewResult UnpaidList(string unPaidIds, long paySheetId)
        {
            var empSalaryList = new List<EmpSalaryXmlModelView>();
            var unPaidList = new List<EmpSalaryXmlModelView>();
            EmpSalaryXmlModelView empSalary;
            if (!string.IsNullOrEmpty(unPaidIds) && paySheetId > 0)
            {
                var paySheetInfo = db.PaySheets.Find(paySheetId);

                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), paySheetInfo.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                foreach (XElement ex in xmlList)
                {
                    empSalary = new EmpSalaryXmlModelView();
                    empSalary.Id = Convert.ToInt64(ex.Element("Id").Value);
                    empSalary.TotalAmount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                    empSalary.ActualSalary = Convert.ToDecimal(ex.Element("ActualSalary").Value);
                    empSalary.TotalPresent = Convert.ToInt32(ex.Element("TotalPresent").Value);
                    empSalary.TotalPaidLeave = Convert.ToInt32(ex.Element("TotalPaidLeave").Value);
                    empSalary.TotalWorkingDays = Convert.ToInt32(ex.Element("TotalWorkingDays").Value);
                    empSalary.TotalHoliday = Convert.ToInt32(ex.Element("TotalHoliday").Value);
                    empSalary.DueAmount = Convert.ToDecimal(ex.Element("DueAmount").Value);
                    empSalary.PaidAmount = Convert.ToDecimal(ex.Element("PaidAmount").Value);
                    empSalary.StartDate = Convert.ToDateTime(ex.Element("StartDate").Value);
                    empSalary.EndDate = Convert.ToDateTime(ex.Element("EndDate").Value);
                    empSalary.GeneratedDate = Convert.ToDateTime(ex.Element("GeneratedDate").Value);
                    empSalary.UserId = Convert.ToInt32(ex.Element("UserId").Value);
                    empSalary.EmpName = ex.Element("EmpName").Value;
                    empSalary.Picture = ex.Element("Picture").Value;
                    empSalary.AssignUserId = Convert.ToInt32(ex.Element("AssignUserId").Value);
                    empSalary.XmlGenerateDate = Convert.ToDateTime(ex.Element("XmlGenerateDate").Value);
                    empSalary.XmlGenerateBy = Convert.ToInt32(ex.Element("XmlGenerateBy").Value);

                    empSalaryList.Add(empSalary);
                }
                foreach(var id in unPaidIds.Split(','))
                {
                    long transId = Convert.ToInt64(id);
                    empSalary = empSalaryList.FirstOrDefault(a => a.Id == transId);
                    unPaidList.Add(empSalary);
                }
            }
            return PartialView(unPaidList);
        }
        public JsonResult PayCompleteWithUnPaid(long transId, int createdBy, string unPaidIds)
        {
            try
            {
                PaymentTransaction paymentTrans;
                EmpSalaryPayment empSalaryPayment;
                long empSalaryId = 0;
                bool isPaid = true;
                decimal totalDue = 0;
                var paySheetPaymentTrans = db.PaymentTransactions.FirstOrDefault(a => a.PaymentId == transId && a.Type == 6 && a.MethodId == 2);
                var paySheet = db.PaySheets.Find(transId);
                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), paySheet.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                foreach (XElement ex in xmlList)
                {
                    empSalaryId = Convert.ToInt64(ex.Element("Id").Value);
                    empSalaryPayment = db.EmpSalaryPayments.Find(empSalaryId);
                    foreach (var id in unPaidIds.Split(','))
                    {
                        SalaryTryToPay tryToPay;
                        if (empSalaryId == Convert.ToInt64(id))
                        {
                            isPaid = false;
                            totalDue = totalDue + Convert.ToDecimal(ex.Element("TotalAmount").Value);
                            
                            tryToPay = new SalaryTryToPay();
                            tryToPay.EmpSalaryPaymentId = empSalaryId;
                            tryToPay.Date = now.Date;
                            tryToPay.CreatedBy = createdBy;
                            db.SalaryTryToPays.Add(tryToPay);
                            //Salary Transaction update
                            empSalaryPayment.DueAmount = empSalaryPayment.TotalAmount;
                            empSalaryPayment.Status = 2;
                            db.Entry(empSalaryPayment).State = EntityState.Modified;

                            //Xml file update
                            ex.Remove();
                            ex.Element("DueAmount").SetValue(empSalaryPayment.TotalAmount);
                            ex.Element("Status").SetValue(empSalaryPayment.Status);
                            xmlDoc.Element("ArrayOfEmpSalaryXmlModelView").Add(ex);
                        }
                    }
                    if (isPaid == true)
                    {
                        //Payment transaction save
                        paymentTrans = new PaymentTransaction();
                        paymentTrans.PaymentId = empSalaryId;
                        paymentTrans.Type = 5; // Type 5 for salary payment 
                        paymentTrans.InOut = false; // InOut false for release payment
                        paymentTrans.MethodId = 2; // 2 for salary 
                        paymentTrans.PaymentTypeId = paySheetPaymentTrans.PaymentTypeId;
                        paymentTrans.PaymentBodyId = paySheetPaymentTrans.PaymentBodyId;
                        paymentTrans.Amount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                        paymentTrans.TransactionNo = paySheetPaymentTrans.TransactionNo;
                        paymentTrans.Date = now.Date;
                        paymentTrans.CreatedBy = createdBy;
                        paymentTrans.IsCreditPayment = false;
                        paymentTrans.PaySheetId = transId;
                        paymentTrans.Status = true;
                        db.PaymentTransactions.Add(paymentTrans);

                        //Salary Transaction update
                        empSalaryPayment.PaidAmount = empSalaryPayment.TotalAmount;
                        empSalaryPayment.PaidBy = createdBy;
                        empSalaryPayment.PaidDate = now.Date;
                        empSalaryPayment.Status = 1;
                        db.Entry(empSalaryPayment).State = EntityState.Modified;

                        //Xml file update
                        ex.Remove();
                        ex.Element("Status").SetValue(empSalaryPayment.Status);
                        ex.Element("PaidAmount").SetValue(empSalaryPayment.TotalAmount);
                        ex.Element("PaidBy").SetValue(createdBy);
                        ex.Element("PaidDate").SetValue(now.Date);
                        xmlDoc.Element("ArrayOfEmpSalaryXmlModelView").Add(ex);
                    }
                    db.SaveChanges();
                    isPaid = true;
                }
                xmlDoc.Save(Paths);
                
                if (totalDue > 0)
                {
                    PaymentBody account;
                    //return salary payment transaction save
                    paymentTrans = new PaymentTransaction();
                    paymentTrans.PaymentId = paySheetPaymentTrans.PaymentId;
                    paymentTrans.Type = 7; // Type 7 for salary return payment transaction 
                    paymentTrans.InOut = true; // InOut true for receive payment
                    paymentTrans.MethodId = 2; // 2 for salary 
                    paymentTrans.PaymentTypeId = paySheetPaymentTrans.PaymentTypeId;
                    paymentTrans.PaymentBodyId = paySheetPaymentTrans.PaymentBodyId;
                    paymentTrans.Amount = totalDue;
                    paymentTrans.TransactionNo = paySheetPaymentTrans.TransactionNo;
                    paymentTrans.Date = now.Date;
                    paymentTrans.CreatedBy = createdBy;
                    paymentTrans.IsCreditPayment = false;
                    paymentTrans.Status = false;
                    db.PaymentTransactions.Add(paymentTrans);

                    //return salary add to accounts
                    account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == paySheetPaymentTrans.PaymentBodyId);
                    account.Balance = account.Balance + totalDue;
                    db.Entry(account).State = EntityState.Modified;
                }
                paySheet.IsApproved = true;
                paySheet.ApprovedBy = createdBy;
                paySheet.ApprovedDate = now.Date;
                db.Entry(paySheet).State = EntityState.Modified;

                paySheetPaymentTrans.Status = false;
                db.Entry(paySheetPaymentTrans).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
        #region User salary Generate
        #region Up to day salary Generate
        public JsonResult CreateUserSalary(int userId, int CreatedBy)
        {
            try
            {
                EmpSalaryPayment empSalary;
                DateTime lastGenerateDate;
                DateTime date;
                var userInfo = db.UserInformations.Find(userId);
                decimal salary = 0;
                int i = 0;
                int totalPresent = 0;
                int totalHoliday = 0;
                int workingDays = 0;
                var workingDaysList = db.WorkingDays.Where(a => a.WorkingScheduleId == userInfo.WorkingScheduleId).ToList();
                List<Holiday> holiDayList;
                List<MultipleHoliday> multipleHolidaList;
                List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
                {
                    lastGenerateDate = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.EndDate).FirstOrDefault().EndDate;
                    date = lastGenerateDate.AddMonths(1);
                    for (date = date; (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                    {
                        empSalary = new EmpSalaryPayment();

                        //calculate total present
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                        //calculate total holiday
                        holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                        if (holiDayList.Any())
                        {
                            totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                            foreach (var list in holiDayList)
                            {
                                multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                                if (multipleHolidaList.Any())
                                {
                                    foreach (var multiList in multipleHolidaList)
                                    {
                                        dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                    }
                                }
                                else
                                {
                                    dayOfWeekList.Add(list.Date.DayOfWeek);
                                }
                            }
                        }
                        //calculate total working days
                        DayOfWeek days = new DayOfWeek();
                        for (i = 0; i < 7; i++)
                        {
                            days = (DayOfWeek)(i);
                            if (workingDaysList.Any(a => a.Day == days.ToString()))
                            {
                                workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                            }
                            else
                            {
                                totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                            }
                        }
                        empSalary.TotalWorkingDays = workingDays;
                        empSalary.TotalHoliday = totalHoliday;
                        workingDays = workingDays - totalHoliday;
                        
                        //calculate salary
                        if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                        {
                            salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                        }
                        else 
                        {
                            salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                        }
                        empSalary.ActualSalary = salary;
                        salary = (salary / workingDays) * totalPresent;

                        //salary save 
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");
                        empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                        empSalary.UserId = userId;
                        empSalary.TotalAmount = salary;
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                        empSalary.DueAmount = salary;
                        if (date.Month == now.Month && date.Year == now.Year)
                        {
                            empSalary.DueAmount = 0;
                        }
                        empSalary.PaidAmount = 0;
                        empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                        if (date.Month == now.Month && date.Year == now.Year)
                        {
                            empSalary.EndDate = now;
                        }
                        else
                        {
                            empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                        }
                        empSalary.GeneratedDate = now;
                        empSalary.GenerateBy = CreatedBy;
                        empSalary.Status = 0;
                        db.EmpSalaryPayments.Add(empSalary);
                        db.SaveChanges();

                        workingDays = 0;
                        totalHoliday = 0;
                        salary = 0;
                        totalPresent = 0;
                        dayOfWeekList = new List<DayOfWeek>();
                    }
                }
                else
                {
                    DateTime? joinDate = userInfo.JoinDate;
                    for (date = joinDate.Value; (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                    {
                        empSalary = new EmpSalaryPayment();

                        //calculate total present
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                        //calculate total holiday
                        holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                        if (holiDayList.Any())
                        {
                            totalHoliday =(int)holiDayList.Sum(a => a.TotalDay);
                            foreach (var list in holiDayList)
                            {
                                multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                                if (multipleHolidaList.Any())
                                {
                                    foreach (var multiList in multipleHolidaList)
                                    {
                                        dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                    }
                                }
                                else
                                {
                                    dayOfWeekList.Add(list.Date.DayOfWeek);
                                }
                            }
                        }
                        //calculate total working days
                        DayOfWeek days = new DayOfWeek();
                        for (i = 0; i < 7; i++)
                        {
                            days = (DayOfWeek)(i);
                            if (workingDaysList.Any(a => a.Day == days.ToString()))
                            {
                                workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                            }
                            else
                            {
                                totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                            }
                        }
                        empSalary.TotalWorkingDays = workingDays;
                        empSalary.TotalHoliday = totalHoliday;
                        workingDays = workingDays - totalHoliday;

                        //calculate salary
                        if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                        {
                            salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                        }
                        else
                        {
                            salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                        }
                        empSalary.ActualSalary = salary;
                        salary = (salary / workingDays) * totalPresent;

                        //salary save 
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");
                        empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                        empSalary.UserId = userId;
                        empSalary.TotalAmount = salary;
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                        empSalary.DueAmount = salary;
                        if(date.Month == now.Month && date.Year == now.Year)
                        {
                            empSalary.DueAmount = 0;
                        }
                        empSalary.PaidAmount = 0;
                        if(date.Month == joinDate.Value.Month && date.Year == joinDate.Value.Year)
                        {
                            empSalary.StartDate = (DateTime)joinDate;
                        }
                        else
                        {
                            empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                        }
                        if (date.Month == now.Month && date.Year == now.Year)
                        {
                            empSalary.EndDate = now;
                        }
                        else
                        {
                            empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                        }
                        empSalary.GeneratedDate = now;
                        empSalary.GenerateBy = CreatedBy;
                        empSalary.Status = 0;
                        db.EmpSalaryPayments.Add(empSalary);
                        db.SaveChanges();

                        workingDays = 0;
                        totalHoliday = 0;
                        salary = 0;
                        totalPresent = 0;
                        dayOfWeekList = new List<DayOfWeek>();
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
        #region Full Month salary Generate
        public JsonResult FullMonthSalaryGenerate(int userId, int createdBy, bool? check,bool? currentDate, DateTime? generateDate)
        {
            try
            {
                int i = 0;
                DateTime date;
                var userInfo = db.UserInformations.Find(userId);
                if (check == true)
                {
                    if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
                    {
                        date = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.EndDate).FirstOrDefault().EndDate;
                        date = date.AddMonths(1);
                        if (date.Month != now.Month)
                        {
                            return Json("MultipleMonth", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        date = userInfo.JoinDate.Value;
                        if (date.Month != now.Month)
                        {
                            return Json("MultipleMonth", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                decimal salary = 0;
                int totalPresent = 0;
                int totalHoliday = 0;
                int workingDays = 0;
                var workingDaysList = db.WorkingDays.Where(a => a.WorkingScheduleId == userInfo.WorkingScheduleId).ToList();
                List<Holiday> holiDayList;
                List<MultipleHoliday> multipleHolidaList;
                List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                EmpSalaryPayment empSalary;
                DateTime joinDate =(DateTime)userInfo.JoinDate;
                if(currentDate == true)
                {
                    date = now;
                    empSalary = new EmpSalaryPayment();

                    //calculate total present
                    totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                    //calculate total holiday
                    holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                    if (holiDayList.Any())
                    {
                        totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                        foreach (var list in holiDayList)
                        {
                            multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                            if (multipleHolidaList.Any())
                            {
                                foreach (var multiList in multipleHolidaList)
                                {
                                    dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                }
                            }
                            else
                            {
                                dayOfWeekList.Add(list.Date.DayOfWeek);
                            }
                        }
                    }
                    //calculate total working days
                    DayOfWeek days = new DayOfWeek();
                    for (i = 0; i < 7; i++)
                    {
                        days = (DayOfWeek)(i);
                        if (workingDaysList.Any(a => a.Day == days.ToString()))
                        {
                            workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                        }
                        else
                        {
                            totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                        }
                    }
                    empSalary.TotalWorkingDays = workingDays;
                    empSalary.TotalHoliday = totalHoliday;
                    workingDays = workingDays - totalHoliday;

                    //calculate salary
                    if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                    {
                        salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                    }
                    else
                    {
                        salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                    }

                    empSalary.ActualSalary = salary;
                    salary = (salary / workingDays) * totalPresent;

                    //salary save 
                    Random generator = new Random();
                    string random = generator.Next(0, 1000000).ToString("D6");
                    empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                    empSalary.UserId = userId;
                    empSalary.TotalAmount = salary;
                    empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                    empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                    empSalary.DueAmount = salary;
                    if (date.Month == now.Month && date.Year == now.Year)
                    {
                        empSalary.DueAmount = 0;
                    }
                    if(joinDate.Month == date.Month && joinDate.Year == date.Year)
                    {
                        empSalary.StartDate = new DateTime(date.Year, date.Month, joinDate.Day);
                    }
                    else
                    {
                        empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                    }
                    empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    empSalary.PaidAmount = 0;
                    empSalary.GeneratedDate = now;
                    empSalary.GenerateBy = createdBy;
                    empSalary.Status = 0;
                    db.EmpSalaryPayments.Add(empSalary);
                    db.SaveChanges();
                }
                else
                {
                    if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
                    {
                        date = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.EndDate).FirstOrDefault().EndDate;
                        date = date.AddMonths(1);
                    }
                    else
                    {
                        date = userInfo.JoinDate.Value;
                    }
                    for (date = date; (date.Month <= generateDate.Value.Month || date.Year < generateDate.Value.Year); date = date.AddMonths(1))
                    {
                        empSalary = new EmpSalaryPayment();

                        //calculate total present and paid leave
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                        //calculate total holiday
                        holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                        if (holiDayList.Any())
                        {
                            totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                            foreach (var list in holiDayList)
                            {
                                multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                                if (multipleHolidaList.Any())
                                {
                                    foreach (var multiList in multipleHolidaList)
                                    {
                                        dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                    }
                                }
                                else
                                {
                                    dayOfWeekList.Add(list.Date.DayOfWeek);
                                }
                            }
                        }
                        //calculate total working days
                        DayOfWeek days = new DayOfWeek();
                        for (i = 0; i < 7; i++)
                        {
                            days = (DayOfWeek)(i);
                            if (workingDaysList.Any(a => a.Day == days.ToString()))
                            {
                                workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                            }
                            else
                            {
                                totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                            }
                        }
                        empSalary.TotalWorkingDays = workingDays;
                        empSalary.TotalHoliday = totalHoliday;
                        workingDays = workingDays - totalHoliday;

                        //calculate salary
                        if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                        {
                            salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                        }
                        else
                        {
                            salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                        }
                        empSalary.ActualSalary = salary;
                        salary = (salary / workingDays) * totalPresent;

                        //salary save 
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");
                        empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                        empSalary.UserId = userId;
                        empSalary.TotalAmount = salary;
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                        empSalary.DueAmount = salary;
                        if (date.Month == now.Month && date.Year == now.Year)
                        {
                            empSalary.DueAmount = 0;
                        }
                        empSalary.PaidAmount = 0;
                        if (joinDate.Month == date.Month && joinDate.Year == date.Year)
                        {
                            empSalary.StartDate = new DateTime(date.Year, date.Month, joinDate.Day);
                        }
                        else
                        {
                            empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                        }
                        empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                        empSalary.GeneratedDate = now;
                        empSalary.GenerateBy = createdBy;
                        empSalary.Status = 0;
                        db.EmpSalaryPayments.Add(empSalary);
                        db.SaveChanges();

                        workingDays = 0;
                        totalHoliday = 0;
                        salary = 0;
                        totalPresent = 0;
                        dayOfWeekList = new List<DayOfWeek>();
                    }
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GenerateMonthSelect(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult _GenerateMonthSelect(int userId)
        {
            var userInfo = db.UserInformations.Find(userId);
            DateTime date;
            List<MultipleMonthModelView> multipleMonthList = new List<MultipleMonthModelView>();
            MultipleMonthModelView aMonth;
            if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
            {
                date = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.EndDate).FirstOrDefault().EndDate;
                for (date = date.AddMonths(1); (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                {
                    aMonth = new MultipleMonthModelView();
                    aMonth.MonthName = Convert.ToDateTime(date).ToString("MMMM yyyy");
                    aMonth.MonthDate = date;
                    multipleMonthList.Add(aMonth);
                }
            }
            else
            {
                for (date = userInfo.JoinDate.Value; (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                {
                    aMonth = new MultipleMonthModelView();
                    aMonth.MonthName = Convert.ToDateTime(date).ToString("MMMM yyyy");
                    aMonth.MonthDate = date;
                    multipleMonthList.Add(aMonth);
                }
            }
            ViewBag.MonthList = new SelectList(multipleMonthList, "MonthDate", "MonthName");
            MultipleMonthModelView model = new MultipleMonthModelView();
            model.UserId = userId;
            return PartialView(model);
        }
        #endregion
        #region Up to Date Salary Generate
        public PartialViewResult UptoDateSalaryGenerate(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult _UptoDateSalaryGenerate(int userId)
        {
            var userInfo = db.UserInformations.Find(userId);
            DateTime date;
            List<MultipleMonthModelView> multipleMonthList = new List<MultipleMonthModelView>();
            MultipleMonthModelView aMonth;
            if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
            {
                date = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.StartDate).FirstOrDefault().StartDate;
                for (date = date.AddMonths(1); (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                {
                    aMonth = new MultipleMonthModelView();
                    aMonth.MonthName = Convert.ToDateTime(date).ToString("MMMM yyyy");
                    aMonth.MonthDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)); ;
                    multipleMonthList.Add(aMonth);
                }
            }
            else
            {
                for (date = userInfo.JoinDate.Value; (date.Month <= now.Month || date.Year < now.Year); date = date.AddMonths(1))
                {
                    aMonth = new MultipleMonthModelView();
                    aMonth.MonthName = Convert.ToDateTime(date).ToString("MMMM yyyy");
                    aMonth.MonthDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    multipleMonthList.Add(aMonth);
                }
            }
            ViewBag.List = new SelectList(multipleMonthList, "MonthDate", "MonthName");
            MultipleMonthModelView model = new MultipleMonthModelView();
            model.UserId = userId;
            return PartialView(model);
        }
        public JsonResult UpToDateSalaryGenerateSave(MultipleMonthModelView model)
        {
            try
            {
                int i = 0;
                DateTime date;
                int userId = model.UserId;
                DateTime selectedDate = model.SelectedDate;
                var userInfo = db.UserInformations.Find(userId);
                decimal salary = 0;
                int totalPresent = 0;
                int totalHoliday = 0;
                int workingDays = 0;
                var workingDaysList = db.WorkingDays.Where(a => a.WorkingScheduleId == userInfo.WorkingScheduleId).ToList();
                List<Holiday> holiDayList;
                List<MultipleHoliday> multipleHolidaList;
                List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                EmpSalaryPayment empSalary;
                DateTime joinDate = (DateTime)userInfo.JoinDate;
                if (db.EmpSalaryPayments.Any(a => a.UserId == userId))
                {
                    date = db.EmpSalaryPayments.Where(a => a.UserId == userId).OrderByDescending(a => a.EndDate).FirstOrDefault().EndDate;
                    date = date.AddMonths(1);
                }
                else
                {
                    date = userInfo.JoinDate.Value;
                }
                for (date = date; (date.Month <= selectedDate.Month || date.Year < selectedDate.Year); date = date.AddMonths(1))
                {
                    empSalary = new EmpSalaryPayment();
                    //calculate total present and paid leave
                    if (date.Month == selectedDate.Month)
                    {
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year && a.InsertedDate.Day <= selectedDate.Day) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();
                    }
                    else
                    {
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();
                    }
                    //calculate total holiday
                    holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                    if (holiDayList.Any())
                    {
                        totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                        foreach (var list in holiDayList)
                        {
                            multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                            if (multipleHolidaList.Any())
                            {
                                foreach (var multiList in multipleHolidaList)
                                {
                                    dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                }
                            }
                            else
                            {
                                dayOfWeekList.Add(list.Date.DayOfWeek);
                            }
                        }
                    }
                    //calculate total working days
                    DayOfWeek days = new DayOfWeek();
                    for (i = 0; i < 7; i++)
                    {
                        days = (DayOfWeek)(i);
                        if (workingDaysList.Any(a => a.Day == days.ToString()))
                        {
                            workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                        }
                        else
                        {
                            totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                        }
                    }
                    empSalary.TotalWorkingDays = workingDays;
                    empSalary.TotalHoliday = totalHoliday;
                    workingDays = workingDays - totalHoliday;

                    //calculate salary
                    if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                    {
                        salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                    }
                    else
                    {
                        salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                    }
                    empSalary.ActualSalary = salary;
                    salary = (salary / workingDays) * totalPresent;

                    //salary save 
                    Random generator = new Random();
                    string random = generator.Next(0, 1000000).ToString("D6");
                    empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                    empSalary.UserId = userId;
                    empSalary.TotalAmount = salary;
                    if (date.Month == selectedDate.Month && date.Year == selectedDate.Year)
                    {
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year && a.InsertedDate.Day <= selectedDate.Day) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year && a.InsertedDate.Day <= selectedDate.Day) && a.IsPaidLeave == 1).Count();
                    }
                    else
                    {
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                    }
                    empSalary.DueAmount = salary;
                    if (date.Month == now.Month && date.Year == now.Year)
                    {
                        empSalary.DueAmount = 0;
                    }
                    empSalary.PaidAmount = 0;
                    if (joinDate.Month == date.Month && joinDate.Year == date.Year)
                    {
                        empSalary.StartDate = new DateTime(date.Year, date.Month, joinDate.Day);
                    }
                    else
                    {
                        empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                    }
                    if (date.Month == selectedDate.Month && date.Year == selectedDate.Year)
                    {
                       empSalary.EndDate = new DateTime(date.Year, date.Month, selectedDate.Day);
                    }
                    else
                    {
                        empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    }
                    empSalary.GeneratedDate = now;
                    empSalary.GenerateBy = model.CreatedBy;
                    empSalary.Status = 0;
                    db.EmpSalaryPayments.Add(empSalary);
                    db.SaveChanges();

                    workingDays = 0;
                    totalHoliday = 0;
                    salary = 0;
                    totalPresent = 0;
                    dayOfWeekList = new List<DayOfWeek>();
                }
            }    
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Specific Date salary Generate
        public PartialViewResult SpecificDateSalaryGenerate(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult _SpecificDateSalaryGenerate(int userId)
        {
            var userInfo = db.UserInformations.Find(userId);
            DateTime date;
            List<MultipleMonthModelView> multipleMonthList = new List<MultipleMonthModelView>();
            MultipleMonthModelView aMonth;
            for(date = userInfo.JoinDate.Value; (date.Month < now.Month || date.Year < now.Year); date = date.AddMonths(1))
            {
                if(db.EmpSalaryPayments.Any(a => a.UserId == userId && (a.StartDate.Month == date.Month && a.StartDate.Year == date.Year)) == false)
                {
                    aMonth = new MultipleMonthModelView();
                    aMonth.MonthName = Convert.ToDateTime(date).ToString("MMMM yyyy");
                    aMonth.MonthDate = date;
                    multipleMonthList.Add(aMonth);
                }
            }
            ViewBag.List = new SelectList(multipleMonthList, "MonthDate", "MonthName");
            MultipleMonthModelView model = new MultipleMonthModelView();
            model.UserId = userId;
            return PartialView(model);
        }
        public JsonResult SpecificSalaryGenerateSave(MultipleMonthModelView model)
        {
            try
            {
                int i = 0;
                DateTime date = model.MonthDate;
                int userId = model.UserId;
                var userInfo = db.UserInformations.Find(userId);
                decimal salary = 0;
                int totalPresent = 0;
                int totalHoliday = 0;
                int workingDays = 0;
                var workingDaysList = db.WorkingDays.Where(a => a.WorkingScheduleId == userInfo.WorkingScheduleId).ToList();
                List<Holiday> holiDayList;
                List<MultipleHoliday> multipleHolidaList;
                List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                EmpSalaryPayment empSalary;
                DateTime joinDate = (DateTime)userInfo.JoinDate;

                empSalary = new EmpSalaryPayment();
                //calculate total present and paid leave
                totalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                //calculate total holiday
                holiDayList = db.Holidays.Where(a => a.HolidayPackId == userInfo.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                if (holiDayList.Any())
                {
                    totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                    foreach (var list in holiDayList)
                    {
                        multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                        if (multipleHolidaList.Any())
                        {
                            foreach (var multiList in multipleHolidaList)
                            {
                                dayOfWeekList.Add(multiList.Date.DayOfWeek);
                            }
                        }
                        else
                        {
                            dayOfWeekList.Add(list.Date.DayOfWeek);
                        }
                    }
                }
                //calculate total working days
                DayOfWeek days = new DayOfWeek();
                for (i = 0; i < 7; i++)
                {
                    days = (DayOfWeek)(i);
                    if (workingDaysList.Any(a => a.Day == days.ToString()))
                    {
                        workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                    }
                    else
                    {
                        totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                    }
                }
                empSalary.TotalWorkingDays = workingDays;
                empSalary.TotalHoliday = totalHoliday;
                workingDays = workingDays - totalHoliday;

                //calculate salary
                if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                {
                    salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                }
                else
                {
                    salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                }
                empSalary.ActualSalary = salary;
                salary = (salary / workingDays) * totalPresent;

                //salary save 

                Random generator = new Random();
                string random = generator.Next(0, 1000000).ToString("D6");
                empSalary.TransactionNo = "Payroll_" + userId + date.Month + date.Year + random;

                empSalary.UserId = userId;
                empSalary.TotalAmount = salary;
                empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                empSalary.DueAmount = salary;
                empSalary.PaidAmount = 0;
                if (joinDate.Month == date.Month && joinDate.Year == date.Year)
                {
                    empSalary.StartDate = new DateTime(date.Year, date.Month, joinDate.Day);
                }
                else
                {
                    empSalary.StartDate = new DateTime(date.Year, date.Month, 1);
                }
                empSalary.EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                empSalary.GeneratedDate = now;
                empSalary.GenerateBy = model.CreatedBy;
                empSalary.Status = 0;
                db.EmpSalaryPayments.Add(empSalary);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Generate all current month salary
        public JsonResult GenerateAllCurrentSalary(int generateBy)
        {
            try
            {
                int i = 0;
                DateTime date;
                DateTime startDate;
                DateTime endDate;
                bool generate = false;
                int userId = 0;
                var userList = db.UserInformations.Where(a => a.Status == 1 && (a.JoinDate.Value.Month < now.Month || a.JoinDate.Value.Year < now.Year) && a.SalaryPackageId > 0 && a.WorkingScheduleId > 0 && a.HolidayPackId > 0).ToList();
                decimal salary = 0;
                int totalPresent = 0;
                int totalHoliday = 0;
                int workingDays = 0;
                List<Holiday> holiDayList;
                List<MultipleHoliday> multipleHolidaList;
                List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                EmpSalaryPayment empSalary;
                List<WorkingDay> workingDaysList;
                foreach (var user in userList)
                {
                    userId = user.UserId;
                    date = now.Date.AddMonths(-1);
                    startDate = new DateTime(date.Year, date.Month, 1);
                    endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                    empSalary = db.EmpSalaryPayments.FirstOrDefault(a => a.UserId == userId && a.StartDate.Month == date.Month && a.StartDate.Year == date.Year);
                    if(empSalary != null)
                    {
                        if(empSalary.StartDate.Date != startDate.Date || empSalary.EndDate.Date != endDate.Date)
                        {
                            db.Entry(empSalary).State = EntityState.Deleted;
                            generate = true;
                        }
                        else
                        {
                            generate = false;
                        }
                    }
                    else
                    {
                        generate = true;
                    }
                    if(generate == true)
                    {
                        workingDaysList = db.WorkingDays.Where(a => a.WorkingScheduleId == user.WorkingScheduleId).ToList();
                        empSalary = new EmpSalaryPayment();
                        //calculate total present and paid leave
                        totalPresent = db.DailyAttendances.Where(a => a.UserId == user.UserId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && (a.AttType == 1 || a.IsPaidLeave == 1)).Count();

                        //calculate total holiday
                        holiDayList = db.Holidays.Where(a => a.HolidayPackId == user.HolidayPackId && (a.Date.Month == date.Month)).ToList();
                        if (holiDayList.Any())
                        {
                            totalHoliday = (int)holiDayList.Sum(a => a.TotalDay);
                            foreach (var list in holiDayList)
                            {
                                multipleHolidaList = db.MultipleHolidays.Where(a => a.ParentId == list.Id).ToList();
                                if (multipleHolidaList.Any())
                                {
                                    foreach (var multiList in multipleHolidaList)
                                    {
                                        dayOfWeekList.Add(multiList.Date.DayOfWeek);
                                    }
                                }
                                else
                                {
                                    dayOfWeekList.Add(list.Date.DayOfWeek);
                                }
                            }
                        }
                        //calculate total working days
                        DayOfWeek days = new DayOfWeek();
                        for (i = 0; i < 7; i++)
                        {
                            days = (DayOfWeek)(i);
                            if (workingDaysList.Any(a => a.Day == days.ToString()))
                            {
                                workingDays += GetOccuranceOfWeekday(date.Year, date.Month, days);
                            }
                            else
                            {
                                totalHoliday -= dayOfWeekList.Count(a => a.Equals(days));
                            }
                        }
                        empSalary.TotalWorkingDays = workingDays;
                        empSalary.TotalHoliday = totalHoliday;
                        workingDays = workingDays - totalHoliday;

                        //calculate salary
                        if (db.Salaries.Any(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)))
                        {
                            salary = db.Salaries.FirstOrDefault(a => a.UserId == userId && (a.EffectiveDate.Month == date.Month && a.EffectiveDate.Year == date.Year)).Amount;
                        }
                        else
                        {
                            salary = db.Salaries.Where(a => a.UserId == userId && a.EffectiveDate < date).OrderByDescending(a => a.EffectiveDate).FirstOrDefault().Amount;
                        }
                        empSalary.ActualSalary = salary;
                        salary = (salary / workingDays) * totalPresent;

                        //salary save 
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");
                        empSalary.TransactionNo = "Payroll_"+ userId + date.Month + date.Year + random;

                        empSalary.UserId = userId;
                        empSalary.TotalAmount = salary;
                        empSalary.TotalPresent = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.AttType == 1).Count();
                        empSalary.TotalPaidLeave = db.DailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == date.Month && a.InsertedDate.Year == date.Year) && a.IsPaidLeave == 1).Count();
                        empSalary.DueAmount = 0;
                        empSalary.PaidAmount = 0;
                        empSalary.StartDate = startDate;
                        empSalary.EndDate = endDate;
                        empSalary.Status = 0;
                        empSalary.GeneratedDate = now;
                        empSalary.GenerateBy = generateBy;
                        db.EmpSalaryPayments.Add(empSalary);
                        db.SaveChanges();

                        workingDays = 0;
                        totalHoliday = 0;
                        salary = 0;
                        totalPresent = 0;
                        dayOfWeekList = new List<DayOfWeek>();
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
        #endregion
        public PartialViewResult ShowUserDueSalary(int userId)
        {
            return PartialView(db.EmpSalaryPayments.Where(a => a.UserId == userId && a.DueAmount > 0).ToList());
        }
        #region Calculate weekday of month
        private int GetOccuranceOfWeekday(int Year, int Month, DayOfWeek Weekday)
        {
            int ReturnValue = 0;
            DateTime MyDate = new DateTime(Year, Month, 1);
            int Start = 1;
            if (Weekday != MyDate.DayOfWeek)
            {
                Start = -(MyDate.DayOfWeek - Weekday - 1);
                if (Start <= 0)
                {
                    ReturnValue = -1;
                }
            }
            while (Start <= DateTime.DaysInMonth(Year, Month))
            {
                ReturnValue += 1;
                Start += 7;
            }
            return ReturnValue;
        }
        #endregion
        #region All Salary Log
        public ActionResult SalaryLog()
        {
            ViewBag.UserList = new SelectList(db.UserInformations.Where(a => a.Status == 1 && a.JoinDate != null && a.SalaryPackageId > 0 && a.WorkingScheduleId > 0 && a.HolidayPackId > 0).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return View();
        }
        public PartialViewResult SalaryList(string selectedUser, int? days, DateTime? from, DateTime? to, bool? isPrint)
        {
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            var list = new List<ViewEmpSalary>();
            DateTime? start = from;
            DateTime? end = to;
            if (!string.IsNullOrEmpty(selectedUser))
            {
                foreach (var id in selectedUser.Split(','))
                {
                    int userId = Convert.ToInt32(id);
                    var trans = new List<ViewEmpSalary>();
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        trans = db.ViewEmpSalaries.Where(m => m.UserId == userId && m.Status != 3 &&  ((DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(countDate)) || (DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(countDate)))).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        trans = db.ViewEmpSalaries.Where(m => m.UserId == userId && m.Status != 3 && ((DbFunctions.TruncateTime(m.StartDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(end)) || (DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.EndDate) <= DbFunctions.TruncateTime(end)))).ToList();
                    }
                    if ((days == null || days == 0) && from == null)
                    {
                        trans = db.ViewEmpSalaries.Where(a => a.UserId == userId && a.Status != 3).ToList();
                    }
                    list.AddRange(trans);
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewEmpSalaries.Where(m => m.Status != 3 && ((DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(countDate)) || (DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(countDate)))).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewEmpSalaries.Where(m => m.Status != 3 && ((DbFunctions.TruncateTime(m.StartDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(end)) || (DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.EndDate) <= DbFunctions.TruncateTime(end)))).ToList();
                }
                if ((days == null || days == 0) && from == null)
                {
                    list = db.ViewEmpSalaries.Where(a => a.Status != 3).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.StartDate).ToList());
        }
        public JsonResult DeleteSalaryTransaction(long transId, int createdBy)
        {
            try
            {
                var aTransaction = db.EmpSalaryPayments.Find(transId);
                aTransaction.Status = 3; // status 3 delete
                aTransaction.UpdatedBy = createdBy;
                aTransaction.UpdatedDate = now.Date;
                db.Entry(aTransaction).State = EntityState.Modified;
                db.SaveChanges();
                var paytrans = db.PaymentTransactions.Where(a => a.Type == 5 && a.MethodId == 2 && a.PaymentId == transId && a.Status != false).ToList();
                if (paytrans.Any())
                {
                    PaymentBody account;

                    PaymentTransaction paymentTrans;
                    foreach (var list in paytrans)
                    {
                        //payment transaction inactive
                        list.Status = false;
                        db.Entry(list).State = EntityState.Modified;

                        paymentTrans = new PaymentTransaction();
                        paymentTrans.PaymentId = list.PaymentId;
                        paymentTrans.Type = 8; // Type 8 for salary return payment transaction 
                        paymentTrans.InOut = true; // InOut true for receive payment
                        paymentTrans.MethodId = 2; // 2 for salary 
                        paymentTrans.PaymentTypeId = list.PaymentTypeId;
                        paymentTrans.PaymentBodyId = list.PaymentBodyId;
                        paymentTrans.Amount = list.Amount;
                        paymentTrans.TransactionNo = list.TransactionNo;
                        paymentTrans.Date = now.Date;
                        paymentTrans.CreatedBy = createdBy;
                        paymentTrans.IsCreditPayment = false;
                        paymentTrans.Status = true;
                        paymentTrans.PaySheetId = list.PaySheetId;
                        db.PaymentTransactions.Add(paymentTrans);

                        //add transaction amount to account
                        account = new PaymentBody();
                        account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == list.PaymentBodyId);
                        account.Balance = account.Balance + list.Amount;
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
        [EncryptedActionParameter]
        public ActionResult SalaryInfoPrint(string selectedUser, int? days, DateTime? from, DateTime? to)
        {
            ViewBag.SelectedUser = selectedUser;
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View();
        }
        #endregion
        #region Current Month Salary Log
        public ActionResult CurrentMonthSalaryLog()
        {
            return View();
        }
        public PartialViewResult CurrentMonthSalaryList(bool? isPrint)
        {
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            var list = new List<ViewEmpSalary>();

            DateTime date = now.AddMonths(-1);
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            list = db.ViewEmpSalaries.Where(a => a.Status != 3 && DbFunctions.TruncateTime(a.StartDate) == DbFunctions.TruncateTime(startDate) && DbFunctions.TruncateTime(a.EndDate) == DbFunctions.TruncateTime(endDate)).ToList();
            //var fileName = date.ToString("MMMM yyyy") + ".xml";
            //var path = Server.MapPath("~/XmlFile/EmpSalary/");
            //path = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), fileName);
            //DataSet ds = new DataSet();
            //ds.ReadXml(path);
            //DataView dvPrograms;
            //dvPrograms = ds.Tables[0].DefaultView;
            //dvPrograms.Sort = "Id";
            //foreach (DataRowView dr in dvPrograms)
            //{
            //    var model = new ViewEmpSalary();
            //    model.Id = Convert.ToInt32(dr[0]);
            //    model.TotalAmount = Convert.ToDecimal(dr[1]);
            //    model.TotalPresent = Convert.ToInt32(dr[2]);
            //    model.TotalPaidLeave = Convert.ToInt32(dr[3]);
            //    model.TotalWorkingDays = Convert.ToInt32(dr[4]);
            //    model.TotalHoliday = Convert.ToInt32(dr[5]);
            //    model.DueAmount = Convert.ToDecimal(dr[6]);
            //    model.PaidAmount = Convert.ToDecimal(dr[7]);
            //    if(dr[8] != DBNull.Value)
            //    {
            //        model.PaidBy = Convert.ToInt32(dr[8]);
            //        model.PaidDate = Convert.ToDateTime(dr[9]);
            //    }
            //    model.StartDate = Convert.ToDateTime(dr[10]);
            //    model.EndDate = Convert.ToDateTime(dr[11]);
            //    model.GeneratedDate = Convert.ToDateTime(dr[12]);
            //    model.UserId = Convert.ToInt32(dr[13]);
            //    model.Title = dr[14].ToString();
            //    model.FirstName = dr[15].ToString();
            //    model.LastName = dr[16].ToString();
            //    model.Picture = dr[17].ToString();
            //    model.ActualSalary = Convert.ToDecimal(dr[18]);
            //    model.AssignUserId = Convert.ToInt32(dr[19]);

            //    list.Add(model);
            //}
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult SalaryLogPrint(int? selectedUserId, int? createdBy)
        {
            if(selectedUserId > 0)
            {
                try
                {
                    PaySheet info = new PaySheet();
                    EmpSalaryXmlModelView xmlModel;
                    List<EmpSalaryXmlModelView> xmlList = new List<EmpSalaryXmlModelView>();

                    DateTime date = now.AddMonths(-1);
                    DateTime startDate = new DateTime(date.Year, date.Month, 1);
                    DateTime endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                    var empSalaryList = db.EmpSalaryPayments.Where(a => DbFunctions.TruncateTime(a.StartDate) == DbFunctions.TruncateTime(startDate) && DbFunctions.TruncateTime(a.EndDate) == DbFunctions.TruncateTime(endDate)).ToList();
                    if(empSalaryList.Any())
                    {
                        foreach (var list in empSalaryList)
                        {
                            list.AssignUserId = selectedUserId;
                            db.Entry(list).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var viewEmpSalaryList = new List<ViewEmpSalary>();
                        viewEmpSalaryList = db.ViewEmpSalaries.Where(a => DbFunctions.TruncateTime(a.StartDate) == DbFunctions.TruncateTime(startDate) && DbFunctions.TruncateTime(a.EndDate) == DbFunctions.TruncateTime(endDate)).ToList();

                        foreach (var list in viewEmpSalaryList)
                        {
                            xmlModel = new EmpSalaryXmlModelView();
                            xmlModel.Id = list.Id;
                            xmlModel.ActualSalary = list.ActualSalary;
                            xmlModel.TotalAmount = list.TotalAmount;
                            xmlModel.TotalPresent = list.TotalPresent;
                            xmlModel.TotalPaidLeave = list.TotalPaidLeave;
                            xmlModel.TotalWorkingDays = list.TotalWorkingDays;
                            xmlModel.TotalHoliday = list.TotalHoliday;
                            xmlModel.DueAmount = list.DueAmount;
                            xmlModel.PaidAmount = list.PaidAmount;
                            xmlModel.PaidBy = list.PaidBy;
                            xmlModel.PaidDate = list.PaidDate;
                            xmlModel.StartDate = list.StartDate;
                            xmlModel.EndDate = list.EndDate;
                            xmlModel.GeneratedDate = list.GeneratedDate;
                            xmlModel.UserId = list.UserId;
                            xmlModel.EmpName = list.Title + " " + list.FirstName + " " + list.MiddleName + " " + list.LastName;
                            xmlModel.Picture = list.Picture;
                            xmlModel.AssignUserId = (int)selectedUserId;
                            xmlModel.XmlGenerateDate = DateTime.Now;
                            xmlModel.XmlGenerateBy = (int)createdBy;
                            xmlModel.UpdatedBy = null;
                            xmlModel.UpdatedDate = null;
                            xmlModel.Status = list.Status;
                            xmlList.Add(xmlModel);
                        }
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");

                        string fileName = date.ToString("MMMMyyyy_") + selectedUserId + random + ".xml";
                        string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                        if (!Directory.Exists(Paths))
                        {
                            Directory.CreateDirectory(Paths);
                        }
                        Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), fileName);

                        //xml Create
                        XmlSerializer writer = new XmlSerializer(typeof(List<EmpSalaryXmlModelView>));
                        FileStream file = System.IO.File.Create(Paths);
                        writer.Serialize(file, xmlList);
                        file.Close();

                        //pay sheet information save
                        info.TransactionNo = "PaySheet_" + date.ToString("yyyyMM") + DateTime.Now.ToString("ddHHmmssf");
                        info.AssignUserId = (int)selectedUserId;
                        info.Filename = fileName;
                        info.StartDate = startDate;
                        info.EndDate = endDate;
                        info.IsPaid = false;
                        info.IsApproved = false;
                        info.TotalAmount = viewEmpSalaryList.Sum(a => a.TotalAmount);
                        info.GenerateDate = DateTime.Now;
                        info.GenerateBy = (int)createdBy;
                        db.PaySheets.Add(info);
                        db.SaveChanges();
                    }
                }
                catch(Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                return View(db.ViewUserLists.FirstOrDefault(a => a.UserId == selectedUserId));
            }
            return View();
        }
        public PartialViewResult AssignUser()
        {
            ViewBag.UserList = new SelectList(db.UserInformations.Where(a => a.Status == 1 && a.JoinDate != null && a.SalaryPackageId > 0 && a.WorkingScheduleId > 0 && a.HolidayPackId > 0).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return PartialView();
        }
        #endregion
        public ActionResult XmlCreate()
        {
            string fileName = "March2018_4145077.xml"; 

            string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
            if (!Directory.Exists(Paths))
            {
                Directory.CreateDirectory(Paths);
            }
            Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), fileName);

            var empSalary = new List<ViewEmpSalary>();
            //empSalary = db.EmpSalaryPayments.ToList();

            //xml Create

            //System.Xml.Serialization.XmlSerializer writer = 
            //    new System.Xml.Serialization.XmlSerializer(typeof(List<EmpSalaryPayment>));
            
            ////var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            //FileStream file = System.IO.File.Create(Paths);

            //writer.Serialize(file, empSalary);
            //file.Close();


            //xml read

            //string xmlData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";//Path of the xml script  
            //DataSet ds = new DataSet();//Using dataset to read xml file  
            //ds.ReadXml(Paths);
            ////empSalary = new List<EmpSalaryPayment>();
            //empSalary = (from rows in ds.Tables[0].AsEnumerable()
            //            select new EmpSalaryPayment
            //            {
            //                Id = Convert.ToInt32(rows[0].ToString()), //Convert row to int  
            //                TotalAmount = Convert.ToDecimal(rows[2]),
            //                DueAmount = Convert.ToDecimal(rows[3]),
            //                PaidAmount = Convert.ToDecimal(rows[4]),
            //            }).ToList();

            //xml read
            //DataSet ds = new DataSet();
            //ds.ReadXml(Paths);
            //DataView dvPrograms;
            //dvPrograms = ds.Tables[0].DefaultView;
            //dvPrograms.Sort = "Id";
            //foreach (DataRowView dr in dvPrograms)
            //{
            //    var model = new ViewEmpSalary();
            //    model.Id = Convert.ToInt32(dr[0]);
            //    model.TotalAmount = Convert.ToDecimal(dr[1]);
            //    model.TotalPresent = Convert.ToInt32(dr[2]);
            //    model.TotalPaidLeave = Convert.ToInt32(dr[3]);
            //    model.TotalWorkingDays = Convert.ToInt32(dr[4]);
            //    model.TotalHoliday = Convert.ToInt32(dr[5]);
            //    model.DueAmount = Convert.ToDecimal(dr[6]);
            //    model.PaidAmount = Convert.ToDecimal(dr[7]);
            //    if (dr[8] != DBNull.Value)
            //    {
            //        model.PaidBy = Convert.ToInt32(dr[8]);
            //        model.PaidDate = Convert.ToDateTime(dr[9]);
            //    }
            //    model.StartDate = Convert.ToDateTime(dr[10]);
            //    model.EndDate = Convert.ToDateTime(dr[11]);
            //    model.GeneratedDate = Convert.ToDateTime(dr[12]);
            //    model.UserId = Convert.ToInt32(dr[13]);
            //    model.Title = dr[14].ToString();
            //    model.FirstName = dr[15].ToString();
            //    model.LastName = dr[16].ToString();
            //    model.Picture = dr[17].ToString();
            //    model.ActualSalary = Convert.ToDecimal(dr[18]);
            //    model.AssignUserId = Convert.ToInt32(dr[19]);

            //    empSalary.Add(model);
            //}

            var salary = new ViewEmpSalary();
            salary = db.ViewEmpSalaries.FirstOrDefault();
            salary.TotalAmount = 500;
            //xml Edit
            XDocument xmlDoc = XDocument.Load(Paths);
            var items = (from item in xmlDoc.Descendants("ViewEmpSalary") select item).ToList();
            XElement selected = items.Where(p => p.Element("Id").Value == salary.Id.ToString()).FirstOrDefault();
            
            //selected.Remove();
            //xmlDoc.Save(Paths);
            xmlDoc.Element("ViewEmpSalaries").Add(new XElement("ViewEmpSalary",
                new XElement("Id", salary.Id),
                new XElement("TotalAmount", salary.TotalAmount),
                new XElement("TotalPresent", salary.TotalPresent),
                new XElement("TotalPaidLeave", salary.TotalPaidLeave),
                new XElement("TotalWorkingDays", salary.TotalWorkingDays),
                new XElement("TotalHoliday", salary.TotalHoliday),
                new XElement("DueAmount", salary.DueAmount),
                new XElement("PaidAmount", salary.PaidAmount),
                new XElement("PaidBy", salary.PaidBy),
                new XElement("PaidDate", salary.PaidDate),
                new XElement("StartDate", salary.StartDate),
                new XElement("EndDate", salary.EndDate),
                new XElement("GeneratedDate", salary.GeneratedDate),
                new XElement("UserId", salary.UserId),
                new XElement("Title", salary.Title),
                new XElement("FirstName", salary.FirstName),
                new XElement("LastName", salary.LastName),
                new XElement("Picture", salary.Picture),
                new XElement("ActualSalary", salary.ActualSalary),
                new XElement("AssignUserId", salary.AssignUserId)
            ));
            //xmlDoc.Save(Paths);

            return View(empSalary);
        }
        #region  Pay sheet
        public ActionResult XmlFile()
        {
            ViewBag.UserList = new SelectList(db.UserInformations.Where(a => a.Status == 1 && a.JoinDate != null && a.SalaryPackageId > 0 && a.WorkingScheduleId > 0 && a.HolidayPackId > 0).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return View();
        }
        public PartialViewResult XmlFileInfoList(string selectedUser, int? days, DateTime? from, DateTime? to)
        {
            var list = new List<ViewPaySheet>();
            DateTime? start = from;
            DateTime? end = to;
            if (!string.IsNullOrEmpty(selectedUser))
            {
                foreach (var id in selectedUser.Split(','))
                {
                    int userId = Convert.ToInt32(id);
                    var trans = new List<ViewPaySheet>();
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        trans = db.ViewPaySheets.Where(m => m.AssignUserId == userId && (DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(countDate) || DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(countDate))).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        trans = db.ViewPaySheets.Where(m => m.AssignUserId == userId && (DbFunctions.TruncateTime(m.StartDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(end)) || (DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.EndDate) <= DbFunctions.TruncateTime(end))).ToList();
                    }
                    if ((days == null || days == 0) && from == null)
                    {
                        trans = db.ViewPaySheets.Where(a => a.AssignUserId == userId).ToList();
                    }
                    list.AddRange(trans);
                }
            }
            else
            {
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPaySheets.Where(m => DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(countDate) || DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(countDate)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPaySheets.Where(m => (DbFunctions.TruncateTime(m.StartDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(end)) || (DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.EndDate) <= DbFunctions.TruncateTime(end))).ToList();
                }
                if ((days == null || days == 0) && from == null)
                {
                    list = db.ViewPaySheets.ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.StartDate).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult XmlFileInfoDetails(long xmlFileId)
        {
            if(xmlFileId > 0)
            {
                var xmlFileInfo = db.PaySheets.Find(xmlFileId);
                ViewBag.StartDate = xmlFileInfo.StartDate;
                ViewBag.EndDate = xmlFileInfo.EndDate;
                ViewBag.xmlFileId = xmlFileId;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }
        public PartialViewResult XmlFileInfoDetailsList(long xmlFileId, bool? isPrint, bool? isReceiptPrint)
        {
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            ViewBag.isReceiptPrint = false;
            if(isReceiptPrint == true)
            {
                ViewBag.isReceiptPrint = true;
            }
            var empSalaryList = new List<EmpSalaryXmlModelView>();
            EmpSalaryXmlModelView empSalary;
            if(xmlFileId > 0)
            {
                var xmlFileInfo = db.PaySheets.Find(xmlFileId);

                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), xmlFileInfo.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                foreach(XElement ex in xmlList)
                {
                    empSalary = new EmpSalaryXmlModelView();
                    empSalary.Id = Convert.ToInt64(ex.Element("Id").Value);
                    empSalary.TotalAmount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                    empSalary.ActualSalary = Convert.ToDecimal(ex.Element("ActualSalary").Value);
                    empSalary.TotalPresent = Convert.ToInt32(ex.Element("TotalPresent").Value);
                    empSalary.TotalPaidLeave = Convert.ToInt32(ex.Element("TotalPaidLeave").Value);
                    empSalary.TotalWorkingDays = Convert.ToInt32(ex.Element("TotalWorkingDays").Value);
                    empSalary.TotalHoliday = Convert.ToInt32(ex.Element("TotalHoliday").Value);
                    empSalary.DueAmount = Convert.ToDecimal(ex.Element("DueAmount").Value);
                    empSalary.PaidAmount = Convert.ToDecimal(ex.Element("PaidAmount").Value);
                    empSalary.StartDate = Convert.ToDateTime(ex.Element("StartDate").Value);
                    empSalary.EndDate = Convert.ToDateTime(ex.Element("EndDate").Value);
                    empSalary.GeneratedDate = Convert.ToDateTime(ex.Element("GeneratedDate").Value);
                    empSalary.UserId = Convert.ToInt32(ex.Element("UserId").Value);
                    empSalary.EmpName = ex.Element("EmpName").Value;
                    empSalary.Picture = ex.Element("Picture").Value;
                    empSalary.AssignUserId = Convert.ToInt32(ex.Element("AssignUserId").Value);
                    empSalary.XmlGenerateDate = Convert.ToDateTime(ex.Element("XmlGenerateDate").Value);
                    empSalary.XmlGenerateBy = Convert.ToInt32(ex.Element("XmlGenerateBy").Value);

                    empSalaryList.Add(empSalary);
                }
            }
            return PartialView(empSalaryList.OrderBy(a => a.ActualSalary).ToList());
        }
        public PartialViewResult EmpSalaryEdit(long transId, long xmlFileId)
        {
            ViewBag.xmlFileId = xmlFileId;
            ViewBag.TransId = transId;
            return PartialView();
        }
        public PartialViewResult _EmpSalaryEdit(long transId, long xmlFileId)
        {
            var empSalary = new EmpSalaryXmlModelView();
            if (transId > 0 && xmlFileId > 0)
            {
                var xmlFileInfo = db.PaySheets.Find(xmlFileId);

                string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), xmlFileInfo.Filename);
                XDocument xmlDoc = XDocument.Load(Paths);
                var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                XElement ex = xmlList.Where(p => p.Element("Id").Value == transId.ToString()).FirstOrDefault();

                empSalary.Id = Convert.ToInt64(ex.Element("Id").Value);
                empSalary.TotalAmount = Convert.ToDecimal(ex.Element("TotalAmount").Value);
                empSalary.TotalPresent = Convert.ToInt32(ex.Element("TotalPresent").Value);
                empSalary.TotalPaidLeave = Convert.ToInt32(ex.Element("TotalPaidLeave").Value);
                empSalary.TotalWorkingDays = Convert.ToInt32(ex.Element("TotalWorkingDays").Value);
                empSalary.TotalHoliday = Convert.ToInt32(ex.Element("TotalHoliday").Value);
            }
            return PartialView(empSalary);
        }
        public JsonResult EmpSalaryUpdate(EmpSalaryXmlModelView empSalary, long xmlFileId)
        {
            try
            {
                if(xmlFileId > 0)
                {
                    var xmlFileInfo = db.PaySheets.Find(xmlFileId);

                    string Paths = Server.MapPath("~/XmlFile/EmpSalary/");
                    Paths = Path.Combine(Server.MapPath("~/XmlFile/EmpSalary/"), xmlFileInfo.Filename);
                    XDocument xmlDoc = XDocument.Load(Paths);
                    var xmlList = (from item in xmlDoc.Descendants("EmpSalaryXmlModelView") select item).ToList();
                    XElement ex = xmlList.Where(p => p.Element("Id").Value == empSalary.Id.ToString()).FirstOrDefault();

                    //remove existing
                    ex.Remove();
                    xmlDoc.Save(Paths);
                    //add new data
                    ex.Element("TotalAmount").SetValue(empSalary.TotalAmount);
                    ex.Element("TotalPresent").SetValue(empSalary.TotalPresent);
                    ex.Element("TotalPaidLeave").SetValue(empSalary.TotalPaidLeave);
                    ex.Element("TotalWorkingDays").SetValue(empSalary.TotalWorkingDays);
                    ex.Element("TotalHoliday").SetValue(empSalary.TotalHoliday);
                    ex.Element("UpdatedBy").SetValue(empSalary.UpdatedBy);
                    ex.Element("UpdatedDate").SetValue(now.Date);
                    xmlDoc.Element("ArrayOfEmpSalaryXmlModelView").Add(ex);
                    
                    xmlDoc.Save(Paths);

                    xmlFileInfo.UpdatedBy = empSalary.UpdatedBy;
                    xmlFileInfo.UpdatedDate = now.Date;
                    db.Entry(xmlFileInfo).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        [EncryptedActionParameter]
        public ActionResult EmpSalaryXmlPrint(long? xmlFileId, bool? isReceiptPrint)
        {
            ViewBag.isReceiptPrint = false;
            if (isReceiptPrint == true)
            {
                ViewBag.isReceiptPrint = true;
            }
            if (xmlFileId == null || xmlFileId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(db.ViewPaySheets.FirstOrDefault(a => a.Id == xmlFileId));
        }
        #endregion
        #region Sale and purchase ledger
        [EncryptedActionParameter]
        public ActionResult SalesAndPurchaseLedger(DateTime fromDate, DateTime toDate)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();
        }
        public PartialViewResult SaleOrPurchaseLedgerWin()
        {
            return PartialView();
        }
        public PartialViewResult SaleOrPurchaseLedgerWinPartial()
        {
            return PartialView();
        }
        public PartialViewResult SaleOrPurchaseList(DateTime? FromDate, DateTime? ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewStockImport> importList = new List<ViewStockImport>();
            List<Ledger> ledgerList = new List<Ledger>();
            List<ViewPosOrder> saleList = new List<ViewPosOrder>();
            if (FromDate != null && ToDate != null)
            {
                importList = db.ViewStockImports.Where(a => a.Date >= FromDate && a.Date <= ToDate).ToList();
                if (importList != null)
                {
                    foreach (var list in importList)
                    {
                        var ledger = new Ledger();
                        ledger.ImportId = list.StockImportId;
                        ledger.Name = list.SupplierName;
                        ledger.AmountPaid = list.PaidAmount;
                        ledger.Date = list.Date;
                        ledger.IsDebit = false;
                        ledgerList.Add(ledger);
                    }
                }
                saleList = db.ViewPosOrders.Where(a => DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
                if (saleList != null)
                {
                    foreach (var list in saleList)
                    {
                        var ledger = new Ledger();
                        ledger.Name = list.CustomerName;
                        ledger.OrderId = list.OrderId;
                        ledger.AmountPaid = list.AmountPaid;
                        ledger.Date = list.OrderDate;
                        ledger.IsDebit = list.IsDebit;
                        ledgerList.Add(ledger);
                    }
                }
            }
            return PartialView(ledgerList.OrderBy(a => a.Date));
        }
        //Print sales / purchase ledger
        [EncryptedActionParameter]
        public ActionResult SalesOrPurchaseLedgerPrint(DateTime? FromDate, DateTime? ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewStockImport> importList = new List<ViewStockImport>();
            List<Ledger> ledgerList = new List<Ledger>();
            List<ViewPosOrder> saleList = new List<ViewPosOrder>();
            if (FromDate != null && ToDate != null)
            {
                importList = db.ViewStockImports.Where(a => a.Date >= FromDate && a.Date <= ToDate).ToList();
                if (importList != null)
                {
                    foreach (var list in importList)
                    {
                        var ledger = new Ledger();
                        ledger.ImportId = list.StockImportId;
                        ledger.Name = list.SupplierName;
                        ledger.AmountPaid = list.PaidAmount;
                        ledger.IsDebit = false;
                        ledger.Date = list.Date;
                        ledgerList.Add(ledger);
                    }
                }
                saleList = db.ViewPosOrders.Where(a => DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
                if (saleList != null)
                {
                    foreach (var list in saleList)
                    {
                        var ledger = new Ledger();
                        ledger.OrderId = list.OrderId;
                        ledger.Name = list.CustomerName;
                        ledger.AmountPaid = list.AmountPaid;
                        ledger.Date = list.OrderDate;
                        ledger.IsDebit = list.IsDebit;
                        ledgerList.Add(ledger);
                    }
                }
            }
            return View(ledgerList.OrderBy(a => a.Date));
        }
        #endregion
        #region Credit ledger
        [EncryptedActionParameter]
        public ActionResult AllCreditLedger(DateTime fromDate, DateTime toDate)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();
        }
        public PartialViewResult CreditLedger()
        {
            return PartialView();
        }
        public PartialViewResult CreditLedgerPartial()
        {
            return PartialView();
        }
        public PartialViewResult CreditLedgerList(DateTime FromDate, DateTime ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewStockImport> importList = new List<ViewStockImport>();
            List<Ledger> ledgerList = new List<Ledger>();
            List<ViewPosOrder> saleList = new List<ViewPosOrder>();
            if (FromDate != null && ToDate != null)
            {
                importList = db.ViewStockImports.Where(a => a.Date >= FromDate && a.Date <= ToDate && a.DueAmount > 0).ToList();
                if (importList != null)
                {
                    foreach (var list in importList)
                    {
                        var ledger = new Ledger();
                        ledger.ImportId = list.StockImportId;
                        ledger.Name = list.SupplierName;
                        ledger.Due = list.DueAmount;
                        ledger.Date = list.Date;
                        ledgerList.Add(ledger);
                    }
                }
                saleList = db.ViewPosOrders.Where(a => a.Status == true && a.DueAmount > 0 && DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
                if (saleList != null)
                {
                    foreach (var list in saleList)
                    {
                        var ledger = new Ledger();
                        ledger.OrderId = list.OrderId;
                        ledger.Name = list.CustomerName;
                        ledger.Due = list.DueAmount;
                        ledger.Date = list.OrderDate;
                        ledgerList.Add(ledger);
                    }
                }
            }
            return PartialView(ledgerList.OrderBy(a => a.Date));
        }
        //Credit ledger print
        [EncryptedActionParameter]
        public ActionResult CreditLedgerPrint(DateTime FromDate, DateTime ToDate)
        {
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            List<ViewStockImport> importList = new List<ViewStockImport>();
            List<Ledger> ledgerList = new List<Ledger>();
            List<ViewPosOrder> saleList = new List<ViewPosOrder>();
            if (FromDate != null && ToDate != null)
            {
                importList = db.ViewStockImports.Where(a => a.Date >= FromDate && a.Date <= ToDate && a.DueAmount > 0).ToList();
                if (importList != null)
                {
                    foreach (var list in importList)
                    {
                        var ledger = new Ledger();
                        ledger.ImportId = list.StockImportId;
                        ledger.Name = list.SupplierName;
                        ledger.Due = list.DueAmount;
                        ledger.Date = list.Date;
                        ledgerList.Add(ledger);
                    }
                }
                saleList = db.ViewPosOrders.Where(a => a.Status == true && a.DueAmount > 0 && DbFunctions.TruncateTime(a.OrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(a.OrderDate) <= DbFunctions.TruncateTime(ToDate)).ToList();
                if (saleList != null)
                {
                    foreach (var list in saleList)
                    {
                        var ledger = new Ledger();
                        ledger.Name = list.CustomerName;
                        ledger.Due = list.DueAmount;
                        ledger.Date = list.OrderDate;
                        ledgerList.Add(ledger);
                    }
                }
            }
            return View(ledgerList.OrderBy(a => a.Date));
        }
        #endregion
        #region Salary Accounts
        public ActionResult SalaryAccounts()
        {
            return View();
        }
        public PartialViewResult SalaryAccountList()
        {
            return PartialView(db.ViewAssignedMethodNames.Where(a => a.MethodId == 2).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult SalaryAccountDetails(int? accId)
        {
            if (accId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(db.ViewAccounts.FirstOrDefault(m => m.PaymentBodyId == accId));
        }
        public PartialViewResult SalaryPaymentTransaction(int? accId, int? days, DateTime? from, DateTime? to, int? count, bool? isInactive, bool? isPrint)
        {
            DateTime? start = from;
            DateTime? end = to;
            decimal? totalAmount = 0;
            var list = new List<ViewPayment>();
            if(accId > 0)
            {
                if (count > 0)
                {
                    list = db.ViewPayments.Where(a => a.PaymentBodyId == accId && a.MethodId == 2 && a.Status != false).OrderByDescending(a => a.Date).ToList();
                    totalAmount = list.Sum(a => a.Amount);
                    list = list.Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.MethodId == 2 && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                    totalAmount = list.Sum(a => a.Amount);
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPayments.Where(m => m.PaymentBodyId == accId && m.MethodId == 2 && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                    totalAmount = list.Sum(a => a.Amount);
                }
            }
            else
            {
                if(isInactive == true)
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.MethodId == 2 && a.Status == false).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                        list = list.Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.MethodId == 2 && m.Status == false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.MethodId == 2 && m.Status == false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        list = db.ViewPayments.Where(a => a.MethodId == 2 && a.Status != false).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                        list = list.Take((int)count).ToList();
                    }
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewPayments.Where(m => m.MethodId == 2 && m.Status != false && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewPayments.Where(m => m.MethodId == 2 && m.Status != false && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).OrderByDescending(a => a.Date).ToList();
                        totalAmount = list.Sum(a => a.Amount);
                    }
                }
            }
            ViewBag.TotalAmount = totalAmount;
            ViewBag.IsPrint = false;
            if(isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            return PartialView(list);
        }
        [EncryptedActionParameter]
        public ActionResult SalaryPaymentTransactionPrint(int? accId, int? days, DateTime? from, DateTime? to, bool? isInactive)
        {
            ViewBag.AccId = accId;
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.IsInactive = isInactive;
            var list = new ViewAccount();
            if (accId > 0)
            {
                list = db.ViewAccounts.FirstOrDefault(a => a.PaymentBodyId == accId);
            }
            return View(list);
        }
        public JsonResult DeletePaymentTransaction(long transId, int createdBy)
        {
            try
            {
                PaymentTransaction paymentTrans;
                PaymentBody account;
                //payment transaction inactive
                var payTrans = db.PaymentTransactions.Find(transId);
                payTrans.Status = false;
                db.Entry(payTrans).State = EntityState.Modified;

                //add due amount to salary transaction
                var salaryPaymentTrans = db.EmpSalaryPayments.Find(payTrans.PaymentId);
                salaryPaymentTrans.PaidAmount = salaryPaymentTrans.PaidAmount - payTrans.Amount;
                salaryPaymentTrans.DueAmount = salaryPaymentTrans.DueAmount + payTrans.Amount;
                salaryPaymentTrans.Status = 0;
                salaryPaymentTrans.UpdatedBy = createdBy;
                salaryPaymentTrans.UpdatedDate = now.Date;
                db.Entry(salaryPaymentTrans).State = EntityState.Modified;

                //create salary return transaction
                paymentTrans = new PaymentTransaction();
                paymentTrans.PaymentId = payTrans.PaymentId;
                paymentTrans.Type = 8; // Type 8 for salary return payment transaction 
                paymentTrans.InOut = true; // InOut true for receive payment
                paymentTrans.MethodId = 2; // 2 for salary 
                paymentTrans.PaymentTypeId = payTrans.PaymentTypeId;
                paymentTrans.PaymentBodyId = payTrans.PaymentBodyId;
                paymentTrans.Amount = payTrans.Amount;
                paymentTrans.TransactionNo = payTrans.TransactionNo;
                paymentTrans.Date = now.Date;
                paymentTrans.CreatedBy = createdBy;
                paymentTrans.IsCreditPayment = false;
                paymentTrans.Status = true;
                paymentTrans.PaySheetId = payTrans.PaySheetId;
                db.PaymentTransactions.Add(paymentTrans);

                //add transaction amount to account
                account = new PaymentBody();
                account = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == payTrans.PaymentBodyId);
                account.Balance = account.Balance + payTrans.Amount;
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
        #region all salary payment transaction
        public ActionResult AllSalaryPaymentTransaction()
        {
            return View(db.PaymentMethods.FirstOrDefault(a => a.Id == 2));
        }
        #endregion
        #region Temporary salary payment Transaction
        public ActionResult TemporaryTransaction()
        {
            return View();
        }
        #endregion

        #region SalesOrPurchaseLedger
        public ActionResult SalesOrPurchaseLedger()
        {
            return View();
        }

        #endregion







    }
}