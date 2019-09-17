using PointOfSale.Models;
using PointOfSale.ModelViews.HR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PointOfSale.Helpers;
using System.Net;

namespace PointOfSale.Controllers
{
    public class HumanResourceController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        #region User Create
        public ActionResult AllUserLists()
        {
            return View();
        }
        public ActionResult AddEmployee()
        {
            UserInformationModelView model = new UserInformationModelView();
            ViewBag.Religion = new SelectList(db.ReligionLists, "ReligionId", "Religion");
            ViewBag.DivisionList = new SelectList(db.DivisionLists, "DivisionId", "DivisionName");
            ViewBag.CountryId = new SelectList(db.CountryLists, "CountryCode", "CountryName");
            ViewBag.TitleId = new SelectList(db.NameTitles, "Id", "Name");
            return View(model);
        }
        public JsonResult AddEmployeeInfo(UserInformationModelView model, IEnumerable<HttpPostedFileBase> files)
        {
            string fullName = "";
            string NtlId_FrntImg = "";
            string NtlId_BckImg = "";
            int ColumnId = 0;
            string Paths = Server.MapPath("~/Images/UserPicture/");
            if (!Directory.Exists(Paths))
            {
                Directory.CreateDirectory(Paths);
            }
            UserInformation user = new UserInformation();
            user.Title = model.Title;
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth == null ? now : model.DateOfBirth;
            user.Nationality = model.Nationality ?? "";
            user.NationalId = model.NationalId;
            user.Phone = model.Phone;
            user.Gender = model.Gender ?? 0;
            user.Religion = model.Religion;
            user.EmailAddress = model.EmailAddress;
            user.MobileNo = model.MobileNo;
            user.ParAddress = model.ParAddress ?? "";
            user.ParAddressLine1 = model.ParAddressLine1;
            user.ParCountry = model.ParCountry ?? "";
            user.ParDivisionId = model.ParDivisionId;
            user.ParCity = model.ParCity;
            user.ParArea = model.ParArea;
            user.ParState = model.ParState;
            user.ParPotalCode = model.ParPotalCode;
            user.PictureOriginalName = model.PictureOriginalName;
            user.WorkingScheduleId = 2; //Default working schedule
            user.HolidayPackId = 1; // Default holiday package
            user.CreatedBy = model.CreatedBy;
            user.CreatedDate = now;
            //user.JoinDate = model.JoinDate == null ? now : model.JoinDate;
            user.Status = 0;
            //user.EmpId = model.EmpId ?? "Emp-" + model.FirstName;
            //user.DeptId = model.DeptId;
            //user.LineId = model.LineId;
            //user.UnitId = model.UnitId;
            //user.MachineId = model.MachineId;
            //user.AssginSalary = false;
            if (model.SamePresentAddress == true)
            {
                user.PreAddress = model.ParAddress;
                user.PreAddressLine1 = model.ParAddressLine1;
                user.PreCountry = model.ParCountry;
                user.PreDivisionId = model.ParDivisionId;
                user.PreCity = model.ParCity;
                user.PrePostalCode = model.ParPotalCode;
                user.PreArea = model.ParArea;
                user.PreState = model.ParState;
                user.SamePresentAddress = true;
            }
            else
            {
                user.PreAddress = model.PreAddress;
                user.PreAddressLine1 = model.PreAddressLine1;
                user.PreCountry = model.PreCountry;
                user.PreState = model.PreState;
                user.PreDivisionId = model.PreDivisionId;
                user.PreCity = model.PreCity;
                user.PrePostalCode = model.PrePostalCode;
                user.PreArea = model.PreArea;
                user.SamePresentAddress = false;
            }
            try
            {
                db.UserInformations.Add(user);
                db.SaveChanges();
                ColumnId = user.UserId;
            }
            catch (Exception)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            if (files != null)
            {
                int i = 0;
                UserInformation anuser = db.UserInformations.Find(user.UserId);
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        Random generator = new Random();
                        string random = generator.Next(0, 1000000).ToString("D6");
                        string s = file.FileName;
                        int idx = s.LastIndexOf('.');
                        string fileName = s.Substring(0, idx);
                        string extension = s.Substring(idx);
                        byte[] imageByte = null;
                        BinaryReader rdr = new BinaryReader(file.InputStream);
                        imageByte = rdr.ReadBytes((int)file.ContentLength);
                        anuser.ImageData = imageByte;
                        if (i == 0)
                        {
                            fullName = "Emp" + ColumnId + random + extension;
                            anuser.Picture = fullName;
                            Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/Original"), fullName);
                            file.SaveAs(Paths);
                            WebImage imgForGallery = new WebImage(file.InputStream);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/thumb"), fullName);
                            imgForGallery.Save(PathsResize);
                        }
                        else if (i == 1)
                        {
                            NtlId_FrntImg = "NtlId_FrntImg" + ColumnId + random + extension;
                            anuser.NationalIdFontImg = NtlId_FrntImg;

                            Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), NtlId_FrntImg);
                            file.SaveAs(Paths);
                            WebImage imgForGallery = new WebImage(file.InputStream);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/thumb"), NtlId_FrntImg);
                            imgForGallery.Save(PathsResize);
                        }
                        else
                        {
                            NtlId_BckImg = "NtlId_BckImg" + ColumnId + random + extension;
                            anuser.NationalIdBackImg = NtlId_BckImg;
                            Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), NtlId_BckImg);
                            file.SaveAs(Paths);
                            WebImage imgForGallery = new WebImage(file.InputStream);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/thumb"), NtlId_BckImg);
                            imgForGallery.Save(PathsResize);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            fullName = "blank-person" + ColumnId;
                            System.IO.File.Copy(Server.MapPath("~/Images/Icon/blank-person.jpg"), Server.MapPath("~/Images/UserPicture/Original/" + fullName + ".jpg"));
                            anuser.Picture = fullName + ".jpg";
                            var blankImage = System.IO.File.ReadAllBytes(Server.MapPath("~/Images/Icon/blank-person.jpg"));
                            WebImage imgForGallery = new WebImage(blankImage);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/thumb"), fullName + ".jpg");
                            imgForGallery.Save(PathsResize);
                        }
                    }
                    i++;
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult _AllUserLists(int? status)
        {
            var list = new List<ViewUserList>();
            if (status == null)
            {
                list = db.ViewUserLists.Where( a=> a.Status != 2).ToList();
            }
            else
            {
                list = db.ViewUserLists.Where(a => a.Status == status).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.CreatedDate));
        }
        #endregion
        #region User Information Details and Edit
        [EncryptedActionParameter]
        public ActionResult UserInfoDetails(int? userId, bool isDisplay)
        {
            ViewBag.IsDisplay = isDisplay;
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var user = db.ViewUserLists.FirstOrDefault(a => a.UserId == userId);
                return View(user);
            }

        }
        public PartialViewResult _ShowUserInfo(int userid, bool isDisplay)
        {
            ViewBag.IsDisplay = isDisplay;
            //var user = db.ViewUserLists.FirstOrDefault(a => a.UserId == userid);
            return PartialView(db.ViewUserLists.FirstOrDefault(a => a.UserId == userid));
        }
        #region Edit Basic Information
        public PartialViewResult EditBasicInformationPopUp(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult EditBasicInformation(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.Title = user.Title;
            model.FirstName = user.FirstName;
            model.MiddleName = user.MiddleName;
            model.LastName = user.LastName;
            model.Picture = user.Picture;
            model.NationalIdFontImg = user.NationalIdFontImg;
            model.NationalIdBackImg = user.NationalIdBackImg;
            model.DateOfBirth = user.DateOfBirth;
            model.NationalId = user.NationalId;
            model.Nationality = user.Nationality;
            model.Religion = user.Religion;
            model.Gender = user.Gender;
            ViewBag.Religion = new SelectList(db.ReligionLists, "ReligionId", "Religion", model.Religion);
            ViewBag.CountryId = new SelectList(db.CountryLists, "CountryCode", "CountryName");
            ViewBag.TitleId = new SelectList(db.NameTitles, "Id", "Name");
            return PartialView(model);
        }
        public JsonResult PersonalInformationUpdate(UserInformationModelView model, IEnumerable<HttpPostedFileBase> files)
        {
            int ColumnId = 0;
            string NtlId_FrntImg = "";
            string NtlId_BckImg = "";
            try
            {
                string Paths = Server.MapPath("~/Images/UserPicture/");
                if (!Directory.Exists(Paths))
                {
                    Directory.CreateDirectory(Paths);
                }
                UserInformation user = db.UserInformations.Find(model.UserId);
                user.Title = model.Title;
                user.FirstName = model.FirstName;
                user.MiddleName = model.MiddleName;
                user.LastName = model.LastName;
                user.DateOfBirth = model.DateOfBirth;
                user.Nationality = model.Nationality;
                user.NationalId = model.NationalId;
                user.Gender = model.Gender.Value;
                user.Religion = model.Religion;
                user.UpdatedBy = model.CreatedBy;
                user.UpdatedDate = now;
                ColumnId = model.UserId;
                if (files != null)
                {
                    int i = 0;
                    foreach (var file in files)
                    {
                        string s = file.FileName;
                        int idx = s.LastIndexOf('.');
                        string fileName = s.Substring(0, idx);
                        string extension = s.Substring(idx);
                        if (i == 0)
                        {
                            NtlId_FrntImg = "NtlId_FrntImg" + ColumnId + extension;
                            user.NationalIdFontImg = NtlId_FrntImg;
                            Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), NtlId_FrntImg);
                            file.SaveAs(Paths);
                            WebImage imgForGallery = new WebImage(file.InputStream);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/thumb"), NtlId_FrntImg);
                            imgForGallery.Save(PathsResize);
                        }
                        else
                        {
                            NtlId_BckImg = "NtlId_BckImg" + ColumnId + extension;
                            user.NationalIdBackImg = NtlId_BckImg;
                            Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), NtlId_BckImg);
                            file.SaveAs(Paths);
                            WebImage imgForGallery = new WebImage(file.InputStream);
                            if (imgForGallery.Width > 70)
                            {
                                imgForGallery.Resize(29, 29, false, true);
                            }
                            string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/thumb"), NtlId_BckImg);
                            imgForGallery.Save(PathsResize);
                        }
                        i++;
                    }
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Edit Contact Information
        public PartialViewResult EditContactInfoPopup(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult EditContactInfo(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.MobileNo = user.MobileNo;
            model.EmailAddress = user.EmailAddress;
            return PartialView(model);
        }
        public JsonResult ContactInformationUpdate(UserInformationModelView model)
        {
            try
            {
                UserInformation user = db.UserInformations.Find(model.UserId);
                user.EmailAddress = model.EmailAddress;
                user.MobileNo = model.MobileNo;
                user.UpdatedBy = model.CreatedBy;
                user.UpdatedDate = now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Edit Permanent Address
        public PartialViewResult EditPermanentAddressPopUp(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult EditPermanentAddress(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.ParAddress = user.ParAddress;
            model.ParAddressLine1 = user.ParAddressLine1;
            model.ParArea = user.ParArea;
            model.ParState = user.ParState;
            model.ParCity = user.ParCity;
            model.ParDivisionId = user.ParDivisionId;
            model.ParPotalCode = user.ParPotalCode;
            model.ParCountry = user.ParCountry;
            model.SamePresentAddress = user.SamePresentAddress;
            ViewBag.CountryId = new SelectList(db.CountryLists, "CountryCode", "CountryName");
            ViewBag.DivisionList = new SelectList(db.DivisionLists, "DivisionId", "DivisionName");
            return PartialView(model);
        }
        public JsonResult PermanentAddressUpdate(UserInformationModelView model)
        {
            UserInformation user = db.UserInformations.Find(model.UserId);
            user.ParAddress = model.ParAddress;
            user.ParAddressLine1 = model.ParAddressLine1;
            user.ParCountry = model.ParCountry;
            user.ParDivisionId = model.ParDivisionId;
            user.ParState = model.ParState;
            user.ParCity = model.ParCity;
            user.ParArea = model.ParArea;
            user.ParPotalCode = model.ParPotalCode;
            user.UpdatedBy = model.CreatedBy;
            user.UpdatedDate = now;
            user.SamePresentAddress = model.SamePresentAddress;
            if (model.SamePresentAddress == true)
            {
                user.PreAddress = model.ParAddress;
                user.PreAddressLine1 = model.ParAddressLine1;
                user.PreCountry = model.ParCountry;
                user.PreDivisionId = model.ParDivisionId;
                user.PreCity = model.ParCity;
                user.PrePostalCode = model.ParPotalCode;
                user.PreArea = model.ParArea;
                user.PreState = model.ParState;
            }
            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Edit Present Address
        public PartialViewResult EditPresentAddressPopUp(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult EditPresentAddress(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.PreAddress = user.PreAddress;
            model.PreAddressLine1 = user.PreAddressLine1;
            model.PreState = user.PreState;
            model.PreArea = user.PreArea;
            model.PreCity = user.PreCity;
            model.PreCountry = user.PreCountry;
            model.PreDivisionId = user.PreDivisionId;
            model.PrePostalCode = user.PrePostalCode;
            ViewBag.CountryId = new SelectList(db.CountryLists, "CountryCode", "CountryName");
            ViewBag.DivisionList = new SelectList(db.DivisionLists, "DivisionId", "DivisionName");
            return PartialView(model);
        }
        public JsonResult PresentAddressUpdate(UserInformationModelView model)
        {
            UserInformation user = db.UserInformations.Find(model.UserId);
            user.PreAddress = model.PreAddress;
            user.PreAddressLine1 = model.PreAddressLine1;
            user.PreCountry = model.PreCountry;
            user.PreDivisionId = model.PreDivisionId;
            user.PreCity = model.PreCity;
            user.PreArea = model.PreArea;
            user.PrePostalCode = model.PrePostalCode;
            user.PreState = model.PreState;
            user.UpdatedBy = model.CreatedBy;
            user.UpdatedDate = now;
            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Edit User Image
        public PartialViewResult _EditUserImage(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public ActionResult EditUserImage(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.Picture = user.Picture;
            return PartialView(model);
        }
        public JsonResult UpdateUserPicture(int UserId, int CreatedBy, HttpPostedFileBase files)
        {
            int ColumnId = UserId;
            string fullName = "";
            UserInformation user = db.UserInformations.Find(UserId);
            if (user.Picture != null && user.Picture != "")
            {
                string FilePath = Path.Combine(Server.MapPath("~/Images/UserPicture/Original"), user.Picture);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }
                string FilePathThumb = Path.Combine(Server.MapPath("~/Images/UserPicture/thumb"), user.Picture);
                if (System.IO.File.Exists(FilePathThumb))
                {
                    System.IO.File.Delete(FilePathThumb);
                }
            }
            if (files != null)
            {
                Random generator = new Random();
                string random = generator.Next(0, 1000000).ToString("D6");
                string s = files.FileName;
                int idx = s.LastIndexOf('.');
                string fileName = s.Substring(0, idx);
                string extension = s.Substring(idx);
                fullName = "Emp" + user.UserId + random + extension;
                byte[] imageByte = null;
                BinaryReader rdr = new BinaryReader(files.InputStream);
                imageByte = rdr.ReadBytes((int)files.ContentLength);
                user.Picture = fullName;
                user.ImageData = imageByte;
                db.Entry(user).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    string Paths = "";
                    Paths = Path.Combine(Server.MapPath("~/Images/UserPicture/Original"), fullName);
                    files.SaveAs(Paths);
                    WebImage imgForGallery = new WebImage(files.InputStream);
                    if (imgForGallery.Width > 70)
                    {
                        imgForGallery.Resize(29, 29, false, true);
                    }
                    string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/thumb"), fullName);
                    imgForGallery.Save(PathsResize);
                }
                catch (Exception)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Random generator = new Random();
                string random = generator.Next(0, 1000000).ToString("D6");
                string s = "blank-person.jpg";
                int idx = s.LastIndexOf('.');
                string fileName = s.Substring(0, idx);
                string extension = s.Substring(idx);
                fullName = "Emp" + user.UserId + random + extension;
                try
                {
                    System.IO.File.Copy(Server.MapPath("~/Images/Icon/blank-person.jpg"), Server.MapPath("~/Images/UserPicture/Original/" + fullName));
                    var blankImage = System.IO.File.ReadAllBytes(Server.MapPath("~/Images/Icon/blank-person.jpg"));
                    WebImage imgForGallery = new WebImage(blankImage);
                    if (imgForGallery.Width > 70)
                    {
                        imgForGallery.Resize(29, 29, false, true);
                    }
                    string PathsResize = Path.Combine(Server.MapPath("~/Images/UserPicture/thumb"), fullName);
                    imgForGallery.Save(PathsResize);

                    user.Picture = fullName;
                    user.ImageData = blankImage;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(fullName, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Change User Status Active/Inactive/Delete
        public JsonResult ChangeUserStatus(int id, int status)
        {
            var user = db.UserInformations.Find(id);
            user.Status = status;
            db.Entry(user).State = EntityState.Modified;

            var uLogin = db.UserLogins.FirstOrDefault(a => a.UserId == id);
            if(uLogin != null)
            {
                uLogin.Status = status;
                db.Entry(uLogin).State = EntityState.Modified;
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
        public JsonResult DeleteUser(int id)
        {
            var user = db.UserInformations.Find(id);
            user.Status = 2;
            db.Entry(user).State = EntityState.Modified;

            var uLogin = db.UserLogins.FirstOrDefault(a => a.UserId == id);
            if (uLogin != null)
            {
                uLogin.Status = 2;
                db.Entry(uLogin).State = EntityState.Modified;
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
        #endregion
        #region NID display/Download
        public ActionResult DisplayNationalIdImages(int userId)
        {
            var user = db.UserInformations.Find(userId);
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = user.UserId;
            model.NationalIdFontImg = user.NationalIdFontImg;
            model.NationalIdBackImg = user.NationalIdBackImg;
            return PartialView(model);
        }
        public FileResult DownloadNIDFront(int userId)
        {
            UserInformation model = db.UserInformations.Find(userId);
            string fullPath = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), model.NationalIdFontImg);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            string fileName = model.NationalIdFontImg;
            string orginalFilename = fileName.Split('_')[1];
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, orginalFilename);
        }
        public FileResult DownloadNIDBack(int userId)
        {
            UserInformation model = db.UserInformations.Find(userId);
            string fullPath = Path.Combine(Server.MapPath("~/Images/UserPicture/NationalId/Original"), model.NationalIdBackImg);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            string fileName = model.NationalIdBackImg;
            string orginalFilename = fileName.Split('_')[1];
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, orginalFilename);
        }
        #endregion
        #endregion
        #region User Login Set and Edit
        public JsonResult SetLoginInfo(int userId, int createdBy)
        {
            var ulogin = new UserLogin();
            var user = db.UserInformations.Find(userId);
            user.UserType = 1;
            db.Entry(user).State = EntityState.Modified;

            ulogin.Username = user.LastName.Substring(0, 1) + user.FirstName.Substring(0, 1);
            ulogin.Username = ulogin.Username.ToUpper();

            LettersCount lc = db.LettersCounts.FirstOrDefault(a => a.Letters == ulogin.Username);

            int count = lc.Count + 1;
            ulogin.Username = ulogin.Username + count.ToString("000");
            ulogin.Password = ulogin.Username + "POS";
            ulogin.UserId = user.UserId;
            ulogin.Status = 1;
            ulogin.CreatedDate = now;
            ulogin.CreatedBy = createdBy;

            db.UserLogins.Add(ulogin);

            lc.Count = count;
            db.Entry(lc).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json(ulogin,JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult EditUserLoginInfo(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult _EditUserLoginInfo(int userId)
        {
            var uLogin = db.UserLogins.FirstOrDefault(a => a.UserId == userId);
            UserLoginModelView model = new UserLoginModelView();
            model.UserId = uLogin.UserId;
            model.Username = uLogin.Username;
            model.Password = uLogin.Password;
            return PartialView(model);
        }
        public JsonResult UpdateLoginInfo(int userId, string username,string password, int createdBy)
        {
            var uLogin = db.UserLogins.FirstOrDefault(a => a.UserId == userId);
            if(username != null)
            {
                uLogin.Username = username;
            }
            else if(password != null)
            {
                uLogin.Password = password;
            }
            uLogin.UpdatedBy = createdBy;
            uLogin.UpdatedDate = now;
            db.Entry(uLogin).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error",JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Attendance
        public PartialViewResult _UserAttendanceTab(int userId)
        {
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult UserAllAttendList(int userId)
        {
            var list = db.ViewDailyAttendances.Where(a => a.UserId == userId && (a.InsertedDate.Month == now.Month && a.InsertedDate.Year == now.Year)).ToList();
            return PartialView(list.OrderBy(a => a.InsertedDate));
        }
        #endregion
        #region Get User List 
        public JsonResult GetUserList(string text)
        {
            var userlist = new List<UserInformation>();
            if (!string.IsNullOrEmpty(text))
            {
                userlist = db.UserInformations.Where(a => a.Status == 1 && (a.FirstName.ToLower().Contains(text.ToLower()) || a.LastName.ToLower().Contains(text.ToLower()))).ToList();
            }
            else
            {
                userlist = db.UserInformations.Where(a => a.Status == 1).ToList();
            }
            var alist = new SelectList(userlist.OrderBy(a => a.FirstName).Select(s => new { s.UserId, Name = s.FirstName + " " + s.MiddleName + " " + s.LastName }).ToList(), "UserId", "Name");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Working Schedule
        public ActionResult WorkingSchedule()
        {
            return View();
        }
        public PartialViewResult WorkingScheduleCreate(int? scheduleId)
        {
            ViewBag.ScheduleId = scheduleId;
            return PartialView();
        }
        public PartialViewResult _WorkingScheduleCreate(int? scheduleId)
        {
            WorkingScheduleModelView model = new WorkingScheduleModelView();
            if(scheduleId > 0)
            {
                var schedule = db.WorkingSchedules.Find(scheduleId);
                model.ScheduleId = schedule.WorkingScheduleId;
                model.ScheduleName = schedule.ScheduleName;
                var workingDays = db.WorkingDays.Where(a => a.WorkingScheduleId == schedule.WorkingScheduleId).ToList();
                model.WorkingDays = workingDays;
            }
            return PartialView(model);
        }
        public JsonResult WorkingScheduleSave(WorkingScheduleModelView model)
        {
            try
            {
                WorkingSchedule schedule;
                WorkingDay workingDay;
                if (model.ScheduleId > 0)
                {
                    schedule = db.WorkingSchedules.Find(model.ScheduleId);
                    schedule.ScheduleName = model.ScheduleName;
                    schedule.UpdatedBy = model.CreatedBy;
                    schedule.UpdatedDate = now;
                    db.Entry(schedule).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var day in model.WorkingDays)
                    {
                        var isExist = db.WorkingDays.Any(a => a.WorkingScheduleId == model.ScheduleId && a.Day == day.Day);
                        if (!isExist)
                        {
                            workingDay = new WorkingDay();
                            workingDay.WorkingScheduleId = schedule.WorkingScheduleId;
                            workingDay.Day = day.Day;
                            db.WorkingDays.Add(workingDay);
                            db.SaveChanges();
                        }
                    }
                    var days = db.WorkingDays.Where(a => a.WorkingScheduleId == model.ScheduleId).ToList();
                    foreach (var day in days)
                    {
                        var isExist = model.WorkingDays.Any(a => a.Day == day.Day);
                        if (!isExist)
                        {
                            db.Entry(day).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    schedule = new WorkingSchedule();
                    schedule.ScheduleName = model.ScheduleName;
                    schedule.CreatedBy = (int)model.CreatedBy;
                    schedule.CreadetDate = now;
                    db.WorkingSchedules.Add(schedule);
                    db.SaveChanges();

                    foreach (var day in model.WorkingDays)
                    {
                        workingDay = new WorkingDay();
                        workingDay.WorkingScheduleId = schedule.WorkingScheduleId;
                        workingDay.Day = day.Day;
                        db.WorkingDays.Add(workingDay);
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

        public PartialViewResult WorkingScheduleList()
        {
            return PartialView(db.ViewWorkingSchedules.ToList());
        }
        public JsonResult GetWorkingDays(int scheduleId)
        {
            var list = db.WorkingDays.Where(a => a.WorkingScheduleId == scheduleId).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _WorkingScheduleTab(int? scheduleId, bool? isDisplay)
        {
            ViewBag.ScheduleId = scheduleId;
            ViewBag.isDisplay = isDisplay;
            return PartialView();
        }
        public PartialViewResult SelectedSchedule(int scheduleId, string reason, string view)
        {
            WorkingScheduleModelView model = new WorkingScheduleModelView();
            var workingSchedule = db.WorkingSchedules.Find(scheduleId);
            model.ScheduleId = workingSchedule.WorkingScheduleId;
            model.ScheduleName = workingSchedule.ScheduleName;
            model.WorkingDays = db.WorkingDays.Where(a => a.WorkingScheduleId == scheduleId).ToList();
            return PartialView(model);
        }
        #endregion
        #region Salary 
        public PartialViewResult _UserSalaryTab(int userId, int? salaryPackageId, bool? isDisplay)
        {
            ViewBag.isDisplay = isDisplay;
            ViewBag.SalaryId = salaryPackageId;
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult AssignSalary(int userId, int? salaryId)
        {
            SalaryModelView model = new SalaryModelView();
            model.UserId = userId;
            if(salaryId > 0)
            {
                var salary = db.Salaries.Find(salaryId);
                model.Id = salary.Id;
                model.Amount = salary.Amount;
                model.EffectiveDate = salary.EffectiveDate;
            }
            return PartialView(model);
        }
        public JsonResult AssignSalarySave(SalaryModelView model)
        {
            var salary = new Salary();
            try
            {
                if (model.Id > 0)
                {
                    salary = db.Salaries.Find(model.Id);
                    if (salary.EffectiveDate.Month == now.Month && salary.EffectiveDate.Year == now.Year)
                    {
                        salary.Amount = model.Amount;
                        salary.UpdatedBy = model.CreatedBy;
                        salary.UpdatedDate = now;
                        db.Entry(salary).State = EntityState.Modified;
                    }
                    else
                    {
                        salary = new Salary();
                        salary.UserId = model.UserId;
                        salary.Amount = model.Amount;
                        salary.EffectiveDate = (DateTime)model.EffectiveDate;
                        salary.AssignBy = model.CreatedBy;
                        salary.AssignDate = now;
                        db.Salaries.Add(salary);
                    }
                }
                else
                {
                    salary = new Salary();
                    salary.UserId = model.UserId;
                    salary.Amount = model.Amount;
                    salary.EffectiveDate =(DateTime)model.EffectiveDate;
                    salary.AssignBy = model.CreatedBy;
                    salary.AssignDate = now;
                    db.Salaries.Add(salary);
                }
                db.SaveChanges();
                var user = db.UserInformations.Find(model.UserId);
                user.SalaryPackageId = salary.Id;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ShowSalary(int salaryId)
        {
            var salary = db.Salaries.Find(salaryId);
            return PartialView(salary);
        }
        public PartialViewResult SalaryList(int UserId, bool isPaidList)
        {
            var list = new List<ViewEmpSalary>();
            if(isPaidList == true)
            {
                list = db.ViewEmpSalaries.Where(a => a.UserId == UserId && a.DueAmount == 0 && a.PaidAmount > 0).ToList();
            }
            else
            {
                list = db.ViewEmpSalaries.Where(a => a.UserId == UserId && a.DueAmount > 0).ToList();
            }
            return PartialView(list);
        }
        //public JsonResult UserSalaryGenerate()
        //{
        //    DateTime date = now;
            
        //    int days = DateTime.DaysInMonth(now.Year,now.Month);
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region join Date
        public PartialViewResult _JoinDateTab(int userId, bool? isDisplay)
        {
            ViewBag.isDisplay = isDisplay;
            ViewBag.UserId = userId;
            return PartialView();
        }
        public PartialViewResult SetJoinDate(int userId)
        {
            UserInformationModelView model = new UserInformationModelView();
            model.UserId = userId;
            return PartialView(model);
        }
        public JsonResult JoinDateSave(UserInformationModelView model)
        {
            try
            {
                var userInfo = db.UserInformations.Find(model.UserId);
                userInfo.JoinDate = model.JoinDate;
                db.Entry(userInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult showJoinDate(int userId)
        {
            ViewBag.JoinDate = db.UserInformations.Find(userId).JoinDate;
            return PartialView();
        }
        #endregion
    }
}