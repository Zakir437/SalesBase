using PointOfSale.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PointOfSale.Helpers;
using PointOfSale.ModelViews.Attendance;

namespace PointOfSale.Controllers
{
    public class AttendanceController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        #region Attendance
        public ActionResult Attendance()
        {
            return View();
        }
        public PartialViewResult UserLists()
        {
            return PartialView(db.ViewUserAttInputs.Where(a => a.Status == 1 && a.JoinDate != null).ToList());
        }
        public PartialViewResult _LeaveType(int? attId)
        {
            ViewBag.AttId = attId;
            return PartialView();
        }
        public PartialViewResult _LeaveTypePartial(int? attId)
        {
            ViewBag.LeaveType = new SelectList(db.LeaveTypes.Where(a => a.Status == 1), "Id", "Name");
            if (attId > 0)
            {
                var attd = db.DailyAttendances.Find(attId);
                return PartialView(attd);
            }
            return PartialView();
        }
        public JsonResult SaveAttendance(DailyAttendance model)
        {
            try
            {
                model.InsertedDate = DateTime.Now;
                if (model.Id > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    db.DailyAttendances.Add(model);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Attendance History
        [EncryptedActionParameter]
        public ActionResult SpecificUserAttHistory(int userId)
        {
            ViewBag.UserId = userId;
            var user = db.UserInformations.Find(userId);
            ViewBag.UserName = user.FirstName + " " + user.MiddleName + " " + user.LastName;
            return PartialView();
        }
        public PartialViewResult _SpecificUserAttHistory(int? userId, int? count, int? days, DateTime? from, DateTime? to)
        {
            var list = new List<ViewDailyAttendance>();
            DateTime? start = from;
            DateTime? end = to;
            if(userId > 0)
            {
                if (count > 0)
                {
                    list = db.ViewDailyAttendances.Where(a => a.UserId == userId).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewDailyAttendances.Where(m => m.UserId ==  userId && DbFunctions.TruncateTime(m.InsertedDate) == DbFunctions.TruncateTime(countDate)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewDailyAttendances.Where(m => m.UserId == userId  && DbFunctions.TruncateTime(m.InsertedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.InsertedDate) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.InsertedDate).ToList());
        }
        public  ActionResult AttendanceLog()
        {
            ViewBag.UserList = new SelectList(db.UserInformations.Where(a => a.Status == 1).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return View();
        }
        public PartialViewResult AttendanceLogList(int? attType, string selectedUser, int? days, DateTime? from, DateTime? to)
        {
            var list = new List<ViewDailyAttendance>();
            DateTime? start = from;
            DateTime? end = to;
            if (attType > 0)
            {
                if (!string.IsNullOrEmpty(selectedUser))
                {
                    foreach(var id in selectedUser.Split(','))
                    {
                        int userId = Convert.ToInt32(id);
                        var userAttendance = new List<ViewDailyAttendance>();
                        if (days == 1)
                        {
                            DateTime countDate = DateTime.Now;
                            userAttendance = db.ViewDailyAttendances.Where(m => m.UserId == userId && m.AttType == attType && DbFunctions.TruncateTime(m.InsertedDate) == DbFunctions.TruncateTime(countDate)).ToList();
                        }
                        if (days > 1)
                        {
                            int day = Convert.ToInt32(days - 1);
                            start = DateTime.Now.AddDays(-(day));
                            end = DateTime.Now;
                        }
                        if (start != null && end != null)
                        {
                            userAttendance = db.ViewDailyAttendances.Where(m => m.UserId == userId && m.AttType == attType && DbFunctions.TruncateTime(m.InsertedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.InsertedDate) <= DbFunctions.TruncateTime(end)).ToList();
                        }
                        if ((days == null || days == 0) && from == null)
                        {
                            userAttendance = db.ViewDailyAttendances.Where(a => a.UserId == userId && a.AttType == attType).ToList();
                        }
                        list.AddRange(userAttendance);
                    }
                }
                else
                {
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewDailyAttendances.Where(m => m.AttType == attType && DbFunctions.TruncateTime(m.InsertedDate) == DbFunctions.TruncateTime(countDate)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewDailyAttendances.Where(m => m.AttType == attType && DbFunctions.TruncateTime(m.InsertedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.InsertedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                    if ((days == null || days == 0) && from == null)
                    {
                        list = db.ViewDailyAttendances.Where(a => a.AttType == attType).ToList();
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedUser))
                {
                    foreach (var id in selectedUser.Split(','))
                    {
                        int userId = Convert.ToInt32(id);
                        var userAttendance = new List<ViewDailyAttendance>();
                        if (days == 1)
                        {
                            DateTime countDate = DateTime.Now;
                            userAttendance = db.ViewDailyAttendances.Where(m => m.UserId == userId && DbFunctions.TruncateTime(m.InsertedDate) == DbFunctions.TruncateTime(countDate)).ToList();
                        }
                        if (days > 1)
                        {
                            int day = Convert.ToInt32(days - 1);
                            start = DateTime.Now.AddDays(-(day));
                            end = DateTime.Now;
                        }
                        if (start != null && end != null)
                        {
                            userAttendance = db.ViewDailyAttendances.Where(m => m.UserId == userId && DbFunctions.TruncateTime(m.InsertedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.InsertedDate) <= DbFunctions.TruncateTime(end)).ToList();
                        }
                        if ((days == null || days == 0) && from == null)
                        {
                            userAttendance = db.ViewDailyAttendances.Where(a => a.UserId == userId).ToList();
                        }
                        list.AddRange(userAttendance);
                    }
                }
                else
                {
                    if (days == 1)
                    {
                        DateTime countDate = DateTime.Now;
                        list = db.ViewDailyAttendances.Where(m => DbFunctions.TruncateTime(m.InsertedDate) == DbFunctions.TruncateTime(countDate)).ToList();
                    }
                    if (days > 1)
                    {
                        int day = Convert.ToInt32(days - 1);
                        start = DateTime.Now.AddDays(-(day));
                        end = DateTime.Now;
                    }
                    if (start != null && end != null)
                    {
                        list = db.ViewDailyAttendances.Where(m => DbFunctions.TruncateTime(m.InsertedDate) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.InsertedDate) <= DbFunctions.TruncateTime(end)).ToList();
                    }
                    if((days == null || days == 0) && from == null)
                    {
                        list = db.ViewDailyAttendances.ToList();
                    }
                }
            }
            return PartialView(list.OrderByDescending(a => a.InsertedDate).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult AttendanceHistoryPrint(int? attType, string selectedUser, int? days, DateTime? from, DateTime? to)
        {
            ViewBag.AttType = attType;
            ViewBag.SelectedUser = selectedUser;
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View();
        }
        #endregion
        #region Manually Attendance
        public PartialViewResult ManuallyAtt()
        {
            return PartialView();
        }
        public PartialViewResult _ManuallyAtt()
        {
            ViewBag.UserList = new SelectList(db.UserInformations.Where(a => a.Status == 1).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return PartialView();
        }
        public PartialViewResult AttConfirm(int id)
        {
            return PartialView(db.DailyAttendances.Find(id));
        }
        public JsonResult ManuallyAttSave(ManuallyAttModelView model, bool check)
        {
            var att = new DailyAttendance();
            att = db.DailyAttendances.FirstOrDefault(a => a.UserId == model.UserId && DbFunctions.TruncateTime(a.InsertedDate) == DbFunctions.TruncateTime(model.DateTime));
            if (check == true)
            {
                if (att != null)
                {
                    return Json(att.Id, JsonRequestBehavior.AllowGet);
                }
            }
            if(att != null)
            {
                att.AttType = 1;
                att.InsertedBy = (int)model.CreatedBy;
                att.InsertedDate = model.DateTime;
                att.IsManuallyAtt = true;
                att.ManuallyInsertedDate = DateTime.Now;
                db.Entry(att).State = EntityState.Modified;
            }
            else
            {
                att = new DailyAttendance();
                att.UserId = model.UserId;
                att.AttType = 1;
                att.InsertedBy = (int)model.CreatedBy;
                att.InsertedDate = model.DateTime;
                att.ManuallyInsertedDate = DateTime.Now;
                att.IsManuallyAtt = true;
                db.DailyAttendances.Add(att);
            }
            try
            {
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}