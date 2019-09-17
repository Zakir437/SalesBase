using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Models;
using PointOfSale.ModelViews;

namespace PointOfSale.Controllers
{
    public class RemoteValidatonController : Controller
    {
        PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        public JsonResult UserNameExist(int? Id, string UserName)
        {
            bool exists = (from m in db.Users.Where(m => (Id.HasValue) ? (m.Id != Id && m.Username == UserName) : (m.Username == UserName)) select m).Any();
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult PasswordExist(int? Id, string Password)
        {
            bool exists = (from m in db.Users.Where(m => (Id.HasValue) ? (m.Id != Id && m.Password == Password) : (m.Password == Password)) select m).Any();
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult BarcodeNumberExists(int? Id, string BarCode)
        {
            bool exists = (from m in db.Products.Where(m => (Id.HasValue) ? (m.ProductId != Id && m.BarCode == BarCode) : (m.BarCode == BarCode)) select m).Any();
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult CategoryNameExist(int? Id, string CategoryName)
        {
            bool exists = db.Categories.Any(m => (Id.HasValue) ? (m.CategoryId != Id && m.CategoryName == CategoryName) : (m.CategoryName == CategoryName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult SubCategoryNameExist(int? Id, string Name)
        {
            bool exists = db.SubCategories.Any(m => (Id.HasValue) ? (m.SubCategoryId != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ProductTypeExist(int? Id, string ProductTypeName)
        {
            bool exists = db.ProductType_Table.Any(m => (Id.HasValue) ? (m.ProductTypeId != Id && m.ProductTypeName == ProductTypeName) : (m.ProductTypeName == ProductTypeName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult BrandNameExist(int? Id, string BrandName)
        {
            bool exists = db.Brands.Any(m => (Id.HasValue) ? (m.BrandId != Id && m.BrandName == BrandName) : (m.BrandName == BrandName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        public JsonResult UnitNameExist(int? Id, string UnitName)
        {
            bool exists = db.Units.Any(m => (Id.HasValue) ? (m.UnitId != Id && m.UnitName == UnitName) : (m.UnitName == UnitName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //Country of region exist
        public JsonResult COONameExist(int? Id, string COOName)
        {
            bool exists = db.CountryOfOrigin_Table.Any(m => (Id.HasValue) ? (m.COOId != Id && m.COOName == COOName) : (m.COOName == COOName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //supplier email is exist
        public JsonResult SupplierEmailExist(int? Id, string Email)
        {
            bool exists = db.Suppliers.Any(m => (Id.HasValue) ? (m.SupplierId != Id && m.Email == Email) : (m.Email == Email));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //Customer email is exist
        public JsonResult CustomerEmailExist(int? Id, string Email)
        {
            bool exists = db.Customers.Any(m => (Id.HasValue) ? (m.CustomerId != Id && m.Email == Email) : (m.Email == Email));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        #region REMOTE VALIDATION FOR AACCOUNTING MODULE
        public JsonResult AccountGrpNameExsits(int? AccId, string AccountName)
        {
            bool exsits = (from m in db.AccountNames.Where(m => (AccId.HasValue) ? (m.AccId != AccId && m.Name == AccountName) : (m.Name == AccountName)) select m).Any();
            if (exsits) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        #endregion
        #region Payment Method Name exists
        public JsonResult PaymentMethodNameExists(int? Id, string MethodName)
        {
            bool exists = db.PaymentMethods.Any(m => (Id.HasValue) ? (m.Id != Id && m.MethodName == MethodName) : (m.MethodName == MethodName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        #endregion
        public JsonResult IsUserNameExists(int? Id, string UserName)
        {
            bool exists = db.UserLogins.Any(m => (Id.HasValue) ? (m.UserId != Id && m.Username == UserName) : (m.Username == UserName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //*********************Leave Type Name Exists**************************************
        public JsonResult LeaveTypeNameExists(int? Id, string Name)
        {
            bool exists = db.LeaveTypes.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //******************Holiday Pack name ***************************
        public JsonResult HolidayPackNameExists(int? HolidayPackId, string HolidayPackName)
        {
            bool exists = db.HolidayPackageLists.Any(m => (HolidayPackId.HasValue) ? (m.HolidayPackId != HolidayPackId && m.HolidayPackName == HolidayPackName) : (m.HolidayPackName == HolidayPackName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        //***********************User email exist****************************************
        public JsonResult UserEmailExist(int? UserId, string EmailAddress)
        {
            bool exists = db.UserInformations.Any(m => (UserId.HasValue) ? (m.UserId != UserId && m.EmailAddress == EmailAddress) : (m.EmailAddress == EmailAddress));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //************************sub master product name exist***************************
        public JsonResult SubProductNameExist(int? Id, string Name)
        {
            bool exists = db.MasterProducts.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        //****************************Tag Name exist*************************
        public JsonResult TagNameExist(int? Id, string TagName)
        {
            bool exists = db.Tags.Any(m => (Id.HasValue) ? (m.Id != Id && m.TagName == TagName) : (m.TagName == TagName));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        //****************************Restriction Name exist*************************
        public JsonResult RestrictionNameExist(int? Id, string Name)
        {
            bool exists = db.Restrictions.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        } 
        //****************************Credit limit Name exist*************************
        public JsonResult CreditLimitNameExist(int? Id, string Name)
        {
            bool exists = db.CreditLimits.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        //****************************Customer membership number exist*************************
        public JsonResult CustomerMembershipNumberExist(int? Id, string Number)
        {
            bool exists = db.Customers.Any(m => (Id.HasValue) ? (m.CustomerId != Id && m.MembershipNumber == Number) : (m.MembershipNumber == Number));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
        //****************************Supplementary name exist*************************
        public JsonResult SupplementaryNameExist(int? Id, string Name)
        {
            bool exists = db.Supplementaries.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ServiceNameExist(int? Id, string Name)
        {
            bool exists = db.Services.Any(m => (Id.HasValue) ? (m.Id != Id && m.Name == Name) : (m.Name == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult SerialNumberExist(string SerialNumber)
        {
            bool exists = db.PosOrderTransactions.Any(m =>  m.SerialNumber == SerialNumber);
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult WasteTypeExist(int? Id, string Name)
        {
            bool exists = db.WasteTypes.Any(m => (Id.HasValue) ? (m.Id != Id && m.TypeName == Name) : (m.TypeName == Name));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult CouponCodeExist(long? Id, string Code)
        {
            bool exists = db.Offers.Any(m => (Id.HasValue) ? (m.Id != Id && m.Coupon == Code) : (m.Coupon == Code));
            if (exists) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }
    }
}