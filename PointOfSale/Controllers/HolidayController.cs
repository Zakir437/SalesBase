using PointOfSale.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.ModelViews.Holiday;
using System.Data.Entity;
using PointOfSale.Helpers;

namespace PointOfSale.Controllers
{
    public class HolidayController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        #region Holiday 
        public ActionResult Holiday()
        {
            return View();
        }
        public PartialViewResult HolidayPackageCreate(int? holidayId)
        {
            ViewBag.HolidayPackId = holidayId;
            return PartialView();
        }
        public PartialViewResult _HolidayPackageCreate(int? holidayId)
        {
            if(holidayId > 0)
            {
                var package = db.HolidayPackageLists.Find(holidayId);
                HolidayPachakgeModelView model = new HolidayPachakgeModelView();
                model.HolidayPackId = package.HolidayPackId;
                model.HolidayPackName = package.HolidayPackName;
                model.NoOfPaidLeave = package.NoOfPaidLeave;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult HolidayPackSave(HolidayPachakgeModelView model)
        {
            HolidayPackageList package;
            try
            {
                if(model.HolidayPackId > 0)
                {
                    package = db.HolidayPackageLists.Find(model.HolidayPackId);
                    package.HolidayPackName = model.HolidayPackName;
                    package.NoOfPaidLeave = model.NoOfPaidLeave;
                    package.UpdatedBy = model.CreatedBy;
                    package.UpdatedDate = now;
                    db.Entry(package).State = EntityState.Modified;
                }
                else
                {
                    package = new HolidayPackageList();
                    package.HolidayPackName = model.HolidayPackName;
                    package.NoOfPaidLeave = model.NoOfPaidLeave;
                    package.CreatedBy = (int)model.CreatedBy;
                    package.CreatedDate = now;
                    db.HolidayPackageLists.Add(package);
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult HolidayPackageList()
        {
            return PartialView(db.ViewHolidayPackages.ToList());
        }
        #endregion
        #region Holiday Details
        [EncryptedActionParameter]
        public ActionResult HolidayPackDetails(int packageId)
        {
            return View(db.ViewHolidayPackages.FirstOrDefault(a => a.HolidayPackId == packageId));
        }
        public PartialViewResult HolidayList(int? packageId)
        {
            return PartialView(db.ViewHolidays.Where(a => a.HolidayPackId == packageId).OrderBy(a => a.Date).ToList());
        }
        public PartialViewResult HolidayCreate(int? holidayId, int packageId)
        {
            ViewBag.HolidayId = holidayId;
            ViewBag.PackageId = packageId;
            return PartialView();
        }
        public PartialViewResult _HolidayCreate(int? holidayId)
        {
            HolidayModelView model = new HolidayModelView();
            if(holidayId > 0)
            {
                var aHoliday = db.Holidays.Find(holidayId);
                model.Id = aHoliday.Id;
                model.HolidayPackId = aHoliday.HolidayPackId;
                model.HolidayName = aHoliday.HolidayName;
                model.IsMultipleDay = (bool)aHoliday.IsMultipleDay;
                model.Date = aHoliday.Date;
                model.StartDate = aHoliday.Date;
                model.EndDate = aHoliday.EndDate;
            }
            return PartialView(model);
        }
        public JsonResult HolidaySave(HolidayModelView model)
        {
            try
            {
                int TotalDay = 1;
                DateTime start = Convert.ToDateTime(model.StartDate);
                DateTime end = Convert.ToDateTime(model.EndDate);
                if (model.IsMultipleDay)
                {
                    TotalDay = new DateDifference(start, end).Days;
                    model.Date = model.StartDate;
                    TotalDay = TotalDay + 1;
                }
                string monthName = Convert.ToDateTime(model.Date).ToString("MMMM");
                int monthCount = Convert.ToInt16(Convert.ToDateTime(model.Date).ToString("MM"));
                string year = Convert.ToDateTime(model.Date).Year.ToString();
                Holiday holiday;
                if (model.Id > 0)
                {
                    holiday = db.Holidays.Find(model.Id);
                    holiday.HolidayName = model.HolidayName;
                    holiday.IsMultipleDay = model.IsMultipleDay;
                    holiday.MonthName = monthName;
                    holiday.MonthCount = monthCount;
                    holiday.TotalDay = TotalDay;
                    holiday.Date = (DateTime)model.Date;
                    if (model.IsMultipleDay)
                    {
                        holiday.EndDate = model.EndDate;
                    }
                    holiday.Year = year;
                    holiday.UpdatedDate = now;
                    holiday.UpdatedBy = model.CreatedBy;
                    db.Entry(holiday).State = EntityState.Modified;
                }
                else
                {
                    holiday = new Holiday();
                    holiday.HolidayPackId = (int)model.HolidayPackId;
                    holiday.HolidayName = model.HolidayName;
                    holiday.IsMultipleDay = model.IsMultipleDay;
                    holiday.MonthName = monthName;
                    holiday.MonthCount = monthCount;
                    holiday.TotalDay = TotalDay;
                    holiday.Date = (DateTime)model.Date;
                    holiday.Year = year;
                    if (model.IsMultipleDay)
                    {
                        holiday.EndDate = model.EndDate;
                    }
                    holiday.CreatedDate = now;
                    holiday.CreatedBy = (int)model.CreatedBy;
                    db.Holidays.Add(holiday);
                }
                db.SaveChanges();
                if (model.IsMultipleDay)
                {
                    var dates = new List<DateTime>();
                    for (var dt = start; dt <= end; dt = dt.AddDays(1))
                    {
                        dates.Add(dt);
                    }
                    if (db.MultipleHolidays.Where(m => m.ParentId == holiday.Id).Any())
                    {
                        var alreadyExsitsDate = db.MultipleHolidays.Where(m => m.ParentId == holiday.Id).ToList();
                        foreach (var AddedDate in alreadyExsitsDate)
                        {
                            db.MultipleHolidays.Remove(AddedDate);
                            db.SaveChanges();
                        }
                    }
                    foreach (var d in dates)
                    {
                        MultipleHoliday multiple = new MultipleHoliday();
                        multiple.ParentId = holiday.Id;
                        multiple.Date = d.Date;
                        multiple.MonthName = Convert.ToDateTime(d.Date).ToString("MMMM");
                        multiple.MonthCount = Convert.ToInt16(Convert.ToDateTime(model.Date).ToString("MM"));
                        db.MultipleHolidays.Add(multiple);
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
    }
}