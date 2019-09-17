using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Models;
using PointOfSale.ModelViews;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using PointOfSale.Helpers;
using PointOfSale.ModelViews.Configuration;
using System.Configuration;
using PointOfSale.ModelViews.Sales;

namespace PointOfSale.Controllers
{
    public class ConfigurationController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        //***************** Miscellaneous Function ************************
        #region MiscFunction
        public ActionResult MiscFunction()
        {
            return View();
        }
        public PartialViewResult MiscFuncList()
        {
            return PartialView(db.MiscFuntions.ToList());
        }
        public JsonResult MiscFuncStatusChange(int miscId, int status)
        {
            try
            {
                var miscFunc = db.MiscFuntions.Find(miscId);
                if (status == 1)
                {
                    miscFunc.Status = true;
                }
                else
                {
                    miscFunc.Status = false;
                }
                db.Entry(miscFunc).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***************** Salary pay day ************************
        #region Salary Pay day edit
        public PartialViewResult SalaryPayDay(int? miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult _SalaryPayDay(int? miscId)
        {
            var payDay = db.MiscFuntions.Find(miscId);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = miscId;
            model.PayDay = payDay.SalaryPayDay;
            return PartialView(model);
        }
        public JsonResult SalaryPayDayUpdate(MiscFuncModel model)
        {
            var aFunction = db.MiscFuntions.Find(model.MiscId);
            aFunction.SalaryPayDay = model.PayDay;
            db.Entry(aFunction).State = EntityState.Modified;
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
        //***********************Shop time**********************************
        #region ShopTime
        public PartialViewResult ShopTime(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult ShopTimePartial(int id)
        {
            var shopTime = db.MiscFuntions.Find(id);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = shopTime.Id;
            model.Is24Hours = (bool)shopTime.Is24hours;
            if(shopTime.Is24hours == false)
            {
                model.StartTime = Convert.ToDateTime(shopTime.Starttime.ToString());
                model.EndTime = Convert.ToDateTime(shopTime.EndTime.ToString());
            }
            return PartialView(model);
        }
        public JsonResult ShopTimeSave(MiscFuncModel model)
        {
            try
            {
                var shopTime = db.MiscFuntions.Find(model.MiscId);
                shopTime.Is24hours = model.Is24Hours;
                if (model.Is24Hours)
                {
                    shopTime.Starttime = null;
                    shopTime.EndTime = null;
                }
                else
                {
                    shopTime.Starttime = model.StartTime.Value.TimeOfDay;
                    shopTime.EndTime = model.EndTime.Value.TimeOfDay;
                }
                db.Entry(shopTime).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);

            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Age Restricted edit**********************************
        #region age Restricted 
        public PartialViewResult AgeRestricted(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult AgeRestrictedPartial(int id)
        {
            var ageRestricted = db.MiscFuntions.Find(id);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = ageRestricted.Id;
            model.Age = (int)ageRestricted.Age;
            return PartialView(model);
        }
        public JsonResult AgeRestrictedSave(MiscFuncModel model)
        {
            try
            {
                var ageRestricted = db.MiscFuntions.Find(model.MiscId);
                ageRestricted.Age = model.Age;
                db.Entry(ageRestricted).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************points value edit**********************************
        #region Loyalty Points
        public PartialViewResult PointsValue(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult PointsValuePartial(int id)
        {
            var pointsValue = db.MiscFuntions.Find(id);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = pointsValue.Id;
            model.PointsValue = (decimal)pointsValue.PointsValue;
            model.Points = (int)pointsValue.Points;
            model.Status = pointsValue.Status;
            return PartialView(model);
        }
        public JsonResult PointsValueSave(MiscFuncModel model)
        {
            try
            {
                var pointsValue = db.MiscFuntions.Find(model.MiscId);
                pointsValue.PointsValue = model.PointsValue;
                pointsValue.Points = model.Points;
                db.Entry(pointsValue).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //*************** Tax Edit ****************************************
        #region Tax Edit
        public PartialViewResult TaxEdit(int? miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult TaxEditPartial(int? miscId)
        {
            var aFunction = db.MiscFuntions.FirstOrDefault(a => a.Id == miscId);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = aFunction.Id;
            model.Rate = aFunction.TaxRate;
            return PartialView(model);
        }
        public JsonResult TaxUpdate(MiscFuncModel model)
        {
            var aFunction = db.MiscFuntions.FirstOrDefault(a => a.Id == model.MiscId);
            aFunction.TaxRate = model.Rate;
            db.Entry(aFunction).State = EntityState.Modified;
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
        // tax active/inactive
        public JsonResult ChangeTaxStatus(int? taxId, int? status)
        {
            var tax = db.MiscFuntions.FirstOrDefault(a => a.Id == taxId);
            if(status == 1)
            {
                tax.Status = true;
            }
            else
            {
                tax.Status = false;
            }
            db.Entry(tax).State = EntityState.Modified;
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
        //******************Tax Function Edit *****************************
        #region Tax Function Edit
        public PartialViewResult TaxFunction(int? miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult TaxFunctionPartial(int? miscId)
        {
            var taxFunc = db.MiscFuntions.FirstOrDefault(a => a.Id == miscId);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = taxFunc.Id;
            model.TaxFunction = taxFunc.Status;
            return PartialView(model);
        }
        public JsonResult TaxFunctionUpdate(MiscFuncModel model)
        {
            var aFunction = db.MiscFuntions.FirstOrDefault(a => a.Id == model.MiscId);
            aFunction.Status =(bool)model.TaxFunction;
            db.Entry(aFunction).State = EntityState.Modified;
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
        //**************Minimum Quantity Percent Edit**********************
        #region Minimum Quantity Percent
        public PartialViewResult MinimumQuantityPercent(int? miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult MinimumQuantityPercentPartial(int? miscId)
        {
            var miscFunc = db.MiscFuntions.FirstOrDefault(a => a.Id == miscId);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = miscFunc.Id;
            model.MinimumQuantity = miscFunc.MinimumQuantity;
            return PartialView(model);
        }
        public JsonResult MinimumQuantityUpdate(MiscFuncModel model)
        {
            var aFunction = db.MiscFuntions.FirstOrDefault(a => a.Id == model.MiscId);
            aFunction.MinimumQuantity = model.MinimumQuantity;
            db.Entry(aFunction).State = EntityState.Modified;
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
        // minimum quantity active/inactive
        public JsonResult ChangeMinimumQuantityStatus(int? miniQuantityId, int? status)
        {
            var miniQuantity = db.MiscFuntions.FirstOrDefault(a => a.Id == miniQuantityId);
            if (status == 1)
            {
                miniQuantity.Status = true;
            }
            else
            {
                miniQuantity.Status = false;
            }
            db.Entry(miniQuantity).State = EntityState.Modified;
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
        //**************Generate Count**********************
        #region Generate Count
        public PartialViewResult GenerateCount(int miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult GenerateCountPartial(int miscId)
        {
            var miscFunc = db.MiscFuntions.Find(miscId);
            MiscFuncModel model = new MiscFuncModel();
            model.MiscId = miscFunc.Id;
            model.Value = miscFunc.Value;
            return PartialView(model);
        }
        public JsonResult GenerateCountUpdate(int MiscId, int Value)
        {
            try
            {
                var aFunction = db.MiscFuntions.Find(MiscId);
                aFunction.Value = Value;
                db.Entry(aFunction).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        //****************Delivery Charge function edit*****************
        public PartialViewResult DeliveryChargeFunc(int miscId)
        {
            ViewBag.MiscId = miscId;
            return PartialView();
        }
        public PartialViewResult DeliveryChargeFuncPartial(int miscId)
        {
            //status true for auto
            //status false for manual
            ViewBag.Status = db.MiscFuntions.Find(miscId).Status;
            return PartialView();
        }
        public JsonResult DeliveryChargeFuncUpdate(int MiscId, bool IsAuto)
        {
            try
            {
                var aFunction = db.MiscFuntions.Find(MiscId);
                aFunction.Status = IsAuto;
                db.Entry(aFunction).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        //***********************User**************************************
        #region User
        public new ActionResult User()
        {
            return View();
        }
        //****************User Create *********************
        public PartialViewResult CreateUser(int? id)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public PartialViewResult UserCreatePartial(int? id)
        {
            if (id > 0)
            {
                User aUser = db.Users.FirstOrDefault(a => a.Id == id);
                UserRegisterModelView model = new UserRegisterModelView();
                model.Id = aUser.Id;
                model.FirstName = aUser.FirstName;
                model.LastName = aUser.LastName;
                model.UserName = aUser.Username;
                model.Password = aUser.Password;
                model.UserId = aUser.UserId;
                return PartialView(model);
            }
            return PartialView();
        }
        //user save
        public JsonResult CreateUserSave(UserRegisterModelView model)
        {
            User aUser = new User();
            if (model.Id > 0) //user update
            {
                aUser = db.Users.FirstOrDefault(a => a.Id == model.Id);
                aUser.FirstName = model.FirstName;
                aUser.LastName = model.LastName;
                aUser.Username = model.UserName;
                aUser.Password = model.Password;
                aUser.UserId = model.UserId;
                db.Entry(aUser).State = EntityState.Modified;
            }
            else //user create
            {
                aUser.FirstName = model.FirstName;
                aUser.LastName = model.LastName;
                aUser.Username = model.UserName;
                aUser.UserId = model.UserId;
                aUser.Password = model.Password;
                aUser.Status = true;
                db.Users.Add(aUser);
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
        //user list
        public PartialViewResult UserList()
        {
            return PartialView(db.Users.ToList());
        }
        //user active/inactive
        public JsonResult ChangeUserStatus(int? id, int? status)
        {
            User aUser = db.Users.FirstOrDefault(a => a.Id == id);
            if (status == 1)
            {
                aUser.Status = true;
            }
            else
            {
                aUser.Status = false;
            }
            db.Entry(aUser).State = EntityState.Modified;
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
        //user delete
        public JsonResult DeleteUser(int? id)
        {
            User aUser = db.Users.FirstOrDefault(a => a.Id == id);
            db.Entry(aUser).State = EntityState.Deleted;
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
        public JsonResult GetUserList()
        {
            var alist = new SelectList(db.Users.Where(a => a.Status == true).Select(a => new { Name = a.FirstName + " " + a.LastName, UserId = a.Id }), "UserId", "Name");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Product***********************************
        #region Product
        //************Product Create start**************************
        public ActionResult Product()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View();
        }
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

                if(model.IsDynamic == true)
                {
                    ProductDistributeItems item;
                    List<ProductDistributeItems> items = new List<ProductDistributeItems>();
                    var distributeItems = db.ProductDistributes.Where(a => a.ProductId == id && a.Status == true).ToList();
                    if(distributeItems != null)
                    {
                        foreach(var distItem in distributeItems)
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
            if(sizeType == 1) // size letter
            {
                list = new SelectList(db.Sizes.Where(a => a.Type == true).Select(s => new { s.Id, s.SizeName }), "Id", "SizeName");
            }
            else if(sizeType == 2) //size number
            {
                list = new SelectList(db.Sizes.Where(a => a.Type == false).Select(s => new { s.Id, s.SizeName }), "Id", "SizeName");
            }
            else if(sizeType == 3)
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
                    if(product != null)
                    {
                        productList.Add(product);
                    }
                }
            }
            else if(categoryId > 0)
            {
                var productIdList = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.ProductSubCategoryStatus == true).GroupBy(g => g.ProductId).Select(s => s.FirstOrDefault().ProductId).ToList();
                if(productIdList.Any())
                {
                    foreach(var id in productIdList)
                    {
                        product = db.ViewMainProducts.FirstOrDefault(a => a.ProductId == id);
                        if(product != null)
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
        //Product Info 
        //Use in Stock page
        public PartialViewResult ProductInfo(int? productId)
        {
            var product = db.ViewProducts.FirstOrDefault(a => a.ProductId == productId);
            return PartialView(product);
        }
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
            if(distributeId > 0)
            {
                var product = db.ViewProducts.FirstOrDefault(a => a.ProductId == productId && a.DistributeId == distributeId);
                if(product != null)
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
                if(product.IsDynamic == true && product.PriceCheckBox == true)
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
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //Get product for multiselect
        //use multiple : product page, stock page
        public JsonResult GetProductList(int? categoryId, bool? IsAvailable)
        {
            if (IsAvailable == true) // stock available 
            {
                var productList = new SelectList(db.ViewStockProducts.Where(a => a.Quantity > 0).OrderBy(o => o.ProductName).Select(a => new { a.RowNumber, a.ProductName }), "RowNumber", "ProductName");
                return Json(productList, JsonRequestBehavior.AllowGet);
            }
            else if(IsAvailable == false) //stock inventory
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
            alist = new SelectList(products.OrderBy(o => o.ProductName).Select(a => new { a.ProductName,a.ProductId }), "ProductId", "ProductName");

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
        //Minimal quantity Update 
        //Uses on Stock page,Product page
        public PartialViewResult MinimumQuantity(int productId, long? distributeId)
        {
            ViewBag.ProductId = productId;
            ViewBag.DistributeId = distributeId;
            return PartialView();
        }
        public PartialViewResult MinimumQuantityPartial(int productId, long? distributeId)
        {
            ProductModelView model = new ProductModelView();
            if(distributeId > 0)
            {
                var subProduct = db.ProductDistributes.FirstOrDefault(a => a.ProductId == productId && a.Id == distributeId);
                if(subProduct != null)
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
        #endregion
        //***********************Master Product***********************************
        #region Master Product
        public ActionResult SubMasterProduct()
        {
            return View();
        }
        public PartialViewResult MasterProductList(int? status)
        {
            var list = new List<ViewMasterProduct>();
            if(status == 1)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == true).ToList();
            }
            else if(status == 0)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == false).ToList();
            }
            else if(status == 2)
            {
                list = db.ViewMasterProducts.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewMasterProducts.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public PartialViewResult SubProductCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult SubProductCreatePartial(int? id)
        {
            SubProductModelView model = new SubProductModelView();
            ViewBag.BrandList = new SelectList(db.Brands.Where(a => a.Status == true).Select(s => new { s.BrandId, s.BrandName }), "BrandId", "BrandName");
            ViewBag.ProductList = new SelectList(db.Products.Where(a => a.Status == true).Select(s => new { s.ProductId,s.ProductName }), "ProductId", "ProductName");
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
        public JsonResult SubProductSave(SubProductModelView model)
        {
            try
            {
                var brand = db.Brands.FirstOrDefault(a => a.BrandName.ToLower() == model.Brand.ToLower());
                if(brand != null)
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
                if(model.Id > 0)
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
                if(model.ProductId > 0)
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
            catch(Exception)
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
            else if(status == 0)
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
        //***********************Master Product Tag***********************************
        #region Master Product Tag
        [EncryptedActionParameter]
        public ActionResult MasterProductTag(int? id)
        {
            return View(db.ViewMasterProducts.FirstOrDefault(a => a.Id == id));
        }
        public PartialViewResult MasterTagList(int id, bool? isView)
        {
            ViewBag.IsView = false;
            if(isView == true)
            {
                ViewBag.IsView = true;
            }
            return PartialView(db.ViewTagItems.Where(a => a.OwnerId == id && a.Type == 2 && a.Status == true).ToList());
        }
        public JsonResult MasterTagSave(IList<TagItem> items,long[] deleteItemIds, int CreatedBy)
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
        //***********************Master Product Category***********************************
        #region Master Product Category
        [EncryptedActionParameter]
        public ActionResult MasterProductCategory(int id)
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View(db.ViewMasterProducts.FirstOrDefault(a => a.Id == id));
        }
        public ActionResult MasterSubCategoryList(int id, bool? isView)
        {
            ViewBag.IsView = false; 
            if(isView == true)
            {
                ViewBag.IsView = true;
            }
            return PartialView(db.ViewMasterProductCategories.Where(a => a.MasterProductId == id && a.MasterCategoryStatus == true && a.MasterSubCategoryStatus == true).ToList());
        }
        public JsonResult MasterProductCategorySave(IList<ModelViews.Sales.MasterProductSubCategoryModelView> subCategorys, string categoryIds, int masterProductId, int CreatedBy, long[] deleteSubCategoryIds)
        {
            try
            {
                int categoryId = 0;
                MasterProductCategory masterCategory;
                MasterProductSubCategory masterSubCategory;
                List<int> categoryIdList = new List<int>();
                if(!string.IsNullOrEmpty(categoryIds))
                {
                    foreach(var id in categoryIds.Split(','))
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
                        foreach(var subCategory in subCategorys.Where(a => a.CategoryId == categoryId))
                        {
                            if(db.ViewMasterProductCategories.Any(a => a.MasterProductId == masterProductId && a.CategoryId == categoryId && a.SubCategoryId == subCategory.SubCategoryId))
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
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryByMasterProductId(int masterProductId)
        {
            var categoryList = db.ViewMasterProductCategories.Where(a => a.MasterProductId == masterProductId &&  a.MasterCategoryStatus == true).Select(s => new { Text = s.CategoryName, Value = s.CategoryId }).ToList();
            if(categoryList.Any())
            {
                return Json(categoryList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NotFound", JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult MasterCategory(int masterId)
        {
            ViewBag.MasterId = masterId;
            return PartialView();
        }
        #endregion
        //***********************Category**********************************
        #region Category
        //Category Create
        public ActionResult Category()
        {
            return View();
        }
        public PartialViewResult CategoryCreate(int? id)
        {
            ViewBag.CategoryId = id;
            return PartialView();
        }
        public PartialViewResult CategoryCreatePartial(int? id)
        {
            ViewBag.RestrictionList = new SelectList(db.Restrictions.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            if (id > 0)
            {
                CategoryModelView model = new CategoryModelView();
                Category aCategory = db.Categories.FirstOrDefault(a => a.CategoryId == id);
                model.CategoryId = aCategory.CategoryId;
                model.CategoryName = aCategory.CategoryName;
                model.RestrictionId = aCategory.RestrictionId;
                return PartialView(model);
            }
            return PartialView();
        }
        //category save
        public JsonResult CategorySave(CategoryModelView model)
        {
            Category aCategory = new Category();
            if (model.CategoryId > 0) // category update
            {
                aCategory = db.Categories.FirstOrDefault(a => a.CategoryId == model.CategoryId);
                aCategory.CategoryName = model.CategoryName;
                aCategory.RestrictionId = model.RestrictionId;
                aCategory.UpdatedBy = model.CreatedBy;
                aCategory.UpdatedDate = DateTime.Now;
                db.Entry(aCategory).State = EntityState.Modified;
            }
            else // category create
            {
                aCategory.CategoryName = model.CategoryName;
                aCategory.RestrictionId = model.RestrictionId;
                aCategory.CreatedBy = model.CreatedBy;
                aCategory.CreatedDate = DateTime.Now;
                aCategory.Status = true;
                db.Categories.Add(aCategory);
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
        public PartialViewResult CategoryList(int? status)
        {
            var list = new List<ViewCategory>();
            if (status == 1) // active
            {
                list = db.ViewCategories.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewCategories.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewCategories.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewCategories.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.CategoryName).ToList());
        }
        //category active/inactive/delete
        public JsonResult ChangeCategoryStatus(int id, int status)
        {
            try
            {
                var category = db.Categories.Find(id);
                if (status == 1)
                {
                    category.Status = true; //active
                }
                else if (status == 0)
                {
                    category.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    category.Status = null; // delete
                }
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategory(string text, int? categoryId, int? productId, int? supplementaryId, int? serviceId)
        {
            if(categoryId > 0)
            {
                var category = db.Categories.Find(categoryId);
                if(category != null)
                {
                    return Json(category, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else if(productId > 0)
            {
                var proCategories = db.ProductCategories.Where(a => a.ProductId == productId && a.Status == true).Join(db.Categories, pc => pc.CategoryId , c => c.CategoryId, (pc, c) => new { Text = c.CategoryName, Value = c.CategoryId });
                if(proCategories.Any())
                {
                    return Json(proCategories, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if(supplementaryId > 0)
            {
                var suppCategory = db.SupplementaryCategories.Where(a => a.SupplementaryId == supplementaryId && a.Status == true).Join(db.Categories, sc => sc.CategoryId, c => c.CategoryId, (sc, c) => new { Text = c.CategoryName, Value = c.CategoryId });
                if(suppCategory.Any())
                {
                    return Json(suppCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if(serviceId > 0)
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
        #endregion
        //***********************Sub Category**********************************
        #region Sub Category
        public ActionResult SubCategory()
        {
            return View();
        }
        public PartialViewResult SubCategoryCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult SubCategoryCreatePartial(int? id)
        {
            ViewBag.RestrictionList = new SelectList(db.Restrictions.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            if (id > 0)
            {
                var subCategory = db.SubCategories.Find(id);
                SubCategoryModelView model = new SubCategoryModelView();
                model.SubCategoryId = subCategory.SubCategoryId;
                model.CategoryId = subCategory.CategoryId;
                model.Name = subCategory.Name;
                model.RestrictionId = subCategory.RestrictionId;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult SubCategorySave(SubCategoryModelView model)
        {
            try
            {
                SubCategory subCategory;
                if (model.SubCategoryId > 0)
                {
                    subCategory = db.SubCategories.Find(model.SubCategoryId);
                    subCategory.Name = model.Name;
                    subCategory.CategoryId = model.CategoryId;
                    subCategory.RestrictionId = model.RestrictionId;
                    subCategory.UpdatedBy = model.CreatedBy;
                    subCategory.UpdatedDate = now.Date;
                    db.Entry(subCategory).State = EntityState.Modified;
                }
                else
                {
                    subCategory = new SubCategory();
                    subCategory.Name = model.Name;
                    subCategory.RestrictionId = model.RestrictionId;
                    subCategory.CategoryId = model.CategoryId;
                    subCategory.Status = true;
                    subCategory.CreatedBy = model.CreatedBy;
                    subCategory.CreatedDate = now.Date;
                    db.SubCategories.Add(subCategory);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult SubCategoryList(int? status)
        {
            var list = new List<ViewSubCategory>();
            if (status == 1) // active
            {
                list = db.ViewSubCategories.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewSubCategories.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewSubCategories.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewSubCategories.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeSubCategoryStatus(int id, int status)
        {
            try
            {
                var subCategory = db.SubCategories.Find(id);
                if (status == 1)
                {
                    subCategory.Status = true; //active
                }
                else if (status == 0)
                {
                    subCategory.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    subCategory.Status = null; // delete
                }
                db.Entry(subCategory).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCategories(int? categoryId, string categoryIds, bool? isMultiselect, int? masterProId, int? productId, int? supplementaryId, int? serviceId)
        {
            var subCategory = new List<SubCategory>();
            if(categoryId > 0)
            {
                subCategory = db.SubCategories.Where(a => a.CategoryId == categoryId && a.Status == true).ToList();
            }
            else if(!string.IsNullOrEmpty(categoryIds))
            {
                foreach(var id in categoryIds.Split(','))
                {
                    categoryId = Convert.ToInt32(id);
                    var subCategoryList = db.SubCategories.Where(a => a.CategoryId == categoryId && a.Status == true).ToList();
                    subCategory.AddRange(subCategoryList);
                }
                if(isMultiselect == false)
                {
                    return Json(subCategory, JsonRequestBehavior.AllowGet);
                }
            }
            else if(masterProId > 0)
            {
                var masterSubCategory = db.ViewMasterProductCategories
                                    .Where(a => a.MasterProductId == masterProId && a.MasterCategoryStatus == true && a.MasterSubCategoryStatus == true)
                                    .Select(s => new { Text = s.SubCategoryName, Value = s.SubCategoryId })
                                    .ToList();
                if(masterSubCategory.Any())
                {
                    return Json(masterSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if(productId > 0)
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
            else if(supplementaryId > 0)
            {
                var suppSubCategory = db.SupplementarySubCategories.Where(a => a.SupplementaryId == supplementaryId && a.Status == true).Join(db.SubCategories, ssc => ssc.SubCategoryId, sc => sc.SubCategoryId, (ssc, sc) => new { Text = sc.Name, Value = sc.SubCategoryId } ).ToList();
                if (suppSubCategory.Any())
                {
                    return Json(suppSubCategory, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else if(serviceId > 0)
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
            var subList = new SelectList(subCategory.OrderBy(a => a.Name).Select(s => new { s.SubCategoryId,s.Name }), "SubCategoryId", "Name");
            return Json(subList, JsonRequestBehavior.AllowGet);
        }

        #endregion
        //***********************Unit**************************************
        #region Unit
        //*********************Product unit create *************************
        public ActionResult Unit()
        {
            return View();
        }
        public PartialViewResult UnitCreate(int? id)
        {
            ViewBag.UnitId = id;
            return PartialView();
        }
        public PartialViewResult UnitCreatePartial(int? id)
        {
            if (id > 0)
            {
                Unit aUnit = db.Units.FirstOrDefault(a => a.UnitId == id);
                UnitModelView model = new UnitModelView();
                model.UnitId = aUnit.UnitId;
                model.UnitName = aUnit.UnitName;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult UnitCreateSave(UnitModelView model)
        {
            Unit aUnit;
            if (model.UnitId > 0)
            {
                aUnit = db.Units.FirstOrDefault(a => a.UnitId == model.UnitId);
                aUnit.UnitName = model.UnitName;
                aUnit.UpdatedBy = model.CreatedBy;
                aUnit.UpdatedDate = DateTime.Now;
                db.Entry(aUnit).State = EntityState.Modified;
            }
            else
            {
                aUnit = new Unit();
                aUnit.UnitName = model.UnitName;
                aUnit.CreatedBy = model.CreatedBy;
                aUnit.CreatedDate = DateTime.Now;
                aUnit.Status = 1;
                db.Units.Add(aUnit);
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
        //**************** product unit create end **********************
        //product unit list
        public PartialViewResult UnitList()
        {
            return PartialView(db.Units.ToList());
        }
        // product unit delete
        public JsonResult DeleteUnit(int? unitId)
        {
            Unit aUnit = db.Units.FirstOrDefault(a => a.UnitId == unitId);
            db.Entry(aUnit).State = EntityState.Deleted;
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
        //product unit active/inactive
        public JsonResult ChangeUnitStatus(int? id, int? status)
        {
            Unit aUnit = db.Units.FirstOrDefault(a => a.UnitId == id);
            aUnit.Status = status;
            db.Entry(aUnit).State = EntityState.Modified;
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
        //***********************Brand*************************************
        #region Brand
        public ActionResult Brand()
        {
            return View();
        }
        public PartialViewResult BrandCreate(int? id)
        {
            ViewBag.BrandId = id;
            return PartialView();
        }
        public PartialViewResult BrandCreatePartial(int? id)
        {
            if (id > 0)
            {
                Brand aBrand = db.Brands.Find(id);
                BrandModelView model = new BrandModelView();
                model.BrandId = aBrand.BrandId;
                model.BrandName = aBrand.BrandName;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult BrandCreateSave(BrandModelView model)
        {
            Brand abrand;
            if (model.BrandId > 0)
            {
                abrand = db.Brands.Find(model.BrandId);
                abrand.BrandName = model.BrandName;
                abrand.UpdatedBy = model.CreatedBy;
                abrand.UpdatedDate = now.Date;
                db.Entry(abrand).State = EntityState.Modified;
            }
            else
            {
                abrand = new Brand();
                abrand.BrandName = model.BrandName;
                abrand.CreatedBy = model.CreatedBy;
                abrand.CreatedDate = now.Date;
                abrand.Status = true;
                db.Brands.Add(abrand);
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
        //********************** Product Brand Create end*****************
        //product brand list
        public PartialViewResult BrandList()
        {
            return PartialView(db.Brands.ToList());
        }
        //product brand delete
        public JsonResult DeleteBrand(int brandId)
        {
            Brand aBrand = db.Brands.Find(brandId);
            aBrand.Status = null;
            db.Entry(aBrand).State = EntityState.Modified;
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
        //product brand status active/inactive
        public JsonResult ChangeBrandStatus(int id, int status)
        {
            Brand aBrand = db.Brands.Find(id);
            if(status == 1)
            {
                aBrand.Status = true;
            }
            else
            {
                aBrand.Status = false;
            }
            db.Entry(aBrand).State = EntityState.Modified;
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
        //***********************Vat***************************************
        #region Vat
        //public ActionResult Vat()
        //{
        //    return View();
        //}
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
                VatModel model = new VatModel();
                model.VatId = aVat.VatId;
                model.Rate = aVat.Rate;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult VatSave(VatModel model)
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
        //***********************Payment Type *****************************
        #region Payment Type
        //Payment Type Create
        public ActionResult PaymentType()
        {
            return View();
        }
        public PartialViewResult PaymentTypeCreate(int? id)
        {
            ViewBag.PaymentTypeId = id;
            return PartialView();
        }
        public PartialViewResult PaymentTypeCreatePartial(int? id)
        {
            if (id > 0)
            {
                var aType = db.PaymentTypes.FirstOrDefault(a => a.PaymentTypeId == id);
                PaymentTypeModel model = new PaymentTypeModel();
                model.PaymentTypeId = aType.PaymentTypeId;
                model.Name = aType.Name;
                model.Type = aType.Type;
                return PartialView(model);
            }
            return PartialView();
        }
        // payment type save
        public JsonResult PaymentTypeSave(PaymentTypeModel model)
        {
            PaymentType aType;
            if (model.PaymentTypeId > 0) // payment type update
            {
                aType = db.PaymentTypes.FirstOrDefault(a => a.PaymentTypeId == model.PaymentTypeId);
                aType.Name = model.Name;
                aType.Type = model.Type;
                aType.UpdatedBy = model.CreatedBy;
                aType.UpdatedDate = DateTime.Now;
                db.Entry(aType).State = EntityState.Modified;
            }
            else // payment type create
            {
                aType = new PaymentType();
                aType.Name = model.Name;
                aType.Type = model.Type;
                aType.Status = 1;
                aType.CreatedBy = (int)model.CreatedBy;
                aType.CreatedDate = DateTime.Now;
                db.PaymentTypes.Add(aType);
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
        //***************payment type create end ****************************

        //****************payment type delete **********************
        public JsonResult DeletePaymentType(int? paymentTypeId)
        {
            var aType = db.PaymentTypes.FirstOrDefault(a => a.PaymentTypeId == paymentTypeId);
            db.Entry(aType).State = EntityState.Deleted;
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
        //payment type list
        public PartialViewResult PaymentTypeList()
        {
            return PartialView(db.PaymentTypes.ToList());
        }
        //payment type active/Inactive
        public JsonResult ChangePaymentTypeStatus(int id, int status, int userId)
        {
            PaymentType aType = db.PaymentTypes.FirstOrDefault(a => a.PaymentTypeId == id);
            aType.Status = status;
            aType.UpdatedBy = userId;
            aType.UpdatedDate = DateTime.Now;
            db.Entry(aType).State = EntityState.Modified;
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
        //***********************Accounts / Payment body**********************************
        #region Accounts / PaymentBody
        //Payment body Create
        public ActionResult PaymentBody()
        {
            return View();
        }
        public PartialViewResult PaymentBodyCreate(int? id)
        {
            ViewBag.PaymentBodyId = id;
            return PartialView();
        }
        public PartialViewResult PaymentBodyCreatePartial(int? id)
        {
            ViewBag.PaymentType = new SelectList(db.PaymentTypes.Where(a => a.Status != 0), "PaymentTypeId", "Name");
            ViewBag.PaymentMethod = new SelectList(db.PaymentMethods.Where(a => a.Status == true).Select(s => new { s.Id, s.MethodName}), "Id", "MethodName");
            if (id > 0)
            {
                PaymentBodyModel model = new PaymentBodyModel();
                var aPaymentBody = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == id);
                model.PaymentBodyId = aPaymentBody.PaymentBodyId;
                model.PaymentCategoryId = aPaymentBody.PaymentTypeId;
                model.Inout = aPaymentBody.InOut;
                model.Name = aPaymentBody.Name;
                model.Balance = (decimal)aPaymentBody.Balance;
                model.Description = aPaymentBody.Description;
                model.SerialNumber = aPaymentBody.SerialNumber;
                ViewBag.SelectedMethods = db.ViewAssignedMethodNames.Where(a => a.AccountId == id).Select(s => new { Text = s.MethodName, Value = s.MethodId }).ToList();
                return PartialView(model);
            }
            return PartialView();
        }
        // payment body save
        public JsonResult PaymentBodySave(PaymentBodyModel model)
        {
            PaymentBody aPaymentBody;
            AccountAssignedWithMethod accMethod;
            try
            {
                if (model.PaymentBodyId > 0) // payment body update
                {
                    aPaymentBody = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == model.PaymentBodyId);
                    aPaymentBody.PaymentTypeId = model.PaymentCategoryId;
                    aPaymentBody.InOut = model.Inout;
                    aPaymentBody.Name = model.Name;
                    aPaymentBody.Description = model.Description;
                    aPaymentBody.Balance = model.Balance;
                    aPaymentBody.SerialNumber = model.SerialNumber;
                    aPaymentBody.UpdatedBy = model.CreatedBy;
                    aPaymentBody.UpdatedDate = DateTime.Now;
                    db.Entry(aPaymentBody).State = EntityState.Modified;
                    db.SaveChanges();

                    var list = new List<int>();
                    var assignedMethodId = db.AccountAssignedWithMethods.Where(a => a.AccountId == model.PaymentBodyId).Select(a => new { a.MethodId }).ToList();
                    if(model.MethodIds != null)
                    {
                        foreach (var id in model.MethodIds.Split(','))
                        {
                            var aAssignedId = Convert.ToInt32(id);
                            list.Add(aAssignedId);
                            if(assignedMethodId.Any(a => a.MethodId == aAssignedId) == false)
                            {
                                accMethod = new AccountAssignedWithMethod();
                                accMethod.AccountId = aPaymentBody.PaymentBodyId;
                                accMethod.MethodId = aAssignedId;
                                accMethod.AssignedBy = (int)model.CreatedBy;
                                accMethod.AssignedDate = DateTime.Now;
                                db.AccountAssignedWithMethods.Add(accMethod);
                                db.SaveChanges();
                            }
                        }
                    }
                    assignedMethodId = db.AccountAssignedWithMethods.Where(a => a.AccountId == model.PaymentBodyId).Select(a => new { a.MethodId }).ToList();
                    if(assignedMethodId.Any())
                    {
                        foreach(var id in assignedMethodId)
                        {
                            if(list.Any(a => a.Equals(id.MethodId)) == false)
                            {
                                accMethod = db.AccountAssignedWithMethods.FirstOrDefault(a => a.MethodId == id.MethodId && a.AccountId == aPaymentBody.PaymentBodyId);
                                db.Entry(accMethod).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else // payment body create
                {
                    aPaymentBody = new PaymentBody();
                    aPaymentBody.Name = model.Name;
                    aPaymentBody.Balance = model.Balance;
                    aPaymentBody.Description = model.Description;
                    aPaymentBody.PaymentTypeId = model.PaymentCategoryId;
                    aPaymentBody.InOut = model.Inout;
                    aPaymentBody.SerialNumber = model.SerialNumber;
                    aPaymentBody.CreatedBy = (int)model.CreatedBy;
                    aPaymentBody.CreatedDate = DateTime.Now;
                    aPaymentBody.Status = true;
                    db.PaymentBodies.Add(aPaymentBody);

                    db.SaveChanges();

                    if (model.MethodIds != null)
                    {
                        foreach (var id in model.MethodIds.Split(','))
                        {
                            accMethod = new AccountAssignedWithMethod();
                            accMethod.AccountId = aPaymentBody.PaymentBodyId;
                            accMethod.MethodId = Convert.ToInt32(id);
                            accMethod.AssignedBy =(int)model.CreatedBy;
                            accMethod.AssignedDate = DateTime.Now;
                            db.AccountAssignedWithMethods.Add(accMethod);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success",JsonRequestBehavior.AllowGet);
        }
        // payment body list 
        public PartialViewResult PaymentBodyList()
        {
            return PartialView(db.ViewAccounts.ToList());
        }
        //delete payment body 
        public JsonResult DeletePaymentBody(int? paymentBodyId)
        {
            PaymentBody aPaymentBody = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == paymentBodyId);
            db.Entry(aPaymentBody).State = EntityState.Deleted;
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
        //payment body active/inactive
        public JsonResult ChangePaymentBodyStatus(int id, int status, int userId)
        {
            var isDefault = db.DefaultAccounts.Any(a => a.AccountId == id);
            if(isDefault == true)
            {
                return Json(isDefault, JsonRequestBehavior.AllowGet);
            }
            PaymentBody aBody = db.PaymentBodies.FirstOrDefault(a => a.PaymentBodyId == id);
           
            if (status == 1)
            {
                aBody.Status = true;
            }
            else
            {
                aBody.Status = false;
            }
            aBody.UpdatedBy = userId;
            aBody.UpdatedDate = DateTime.Now;
            db.Entry(aBody).State = EntityState.Modified;
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
        public JsonResult MethodSelectedValue(int? accId)
        {
            if(accId > 0)
            {
              var list =  db.ViewAssignedMethodNames.Where(a => a.AccountId == accId).Select(s => new { Text = s.MethodName, Value = s.MethodId }).ToList();
              return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json("NotFound", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Supplier**********************************
        #region Supplier
        //Supplier Create
        #region Supplier Create
        public ActionResult Supplier()
        {
            return View();
        }
        public PartialViewResult SupplierList(int? status)
        {
            var list = new List<ViewSupplier>();
            if (status == 1) // active
            {
                list = db.ViewSuppliers.Where(a => a.Status == true && a.Type == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewSuppliers.Where(a => a.Status == false && a.Type == true).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewSuppliers.Where(a => a.Status == null && a.Type == true).ToList();
            }
            else
            {
                list = db.ViewSuppliers.Where(a => a.Status != null && a.Type == true).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        //****************supplier Create ***********************
        public PartialViewResult CreateSupplier(long? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult SupplierCreatePartial(long? id)
        {
            if (id > 0)
            {
                Supplier aSupplier = db.Suppliers.FirstOrDefault(a => a.SupplierId == id);
                SupplierModel model = new SupplierModel();
                model.SupplierId = aSupplier.SupplierId;
                model.Name = aSupplier.Name;
                model.Email = aSupplier.Email;
                model.Phone = aSupplier.Phone;
                model.SupplierSince = aSupplier.SupplierSince;
                model.Address = aSupplier.Address;
                return PartialView(model);
            }
            return PartialView();
        }
        //supplier save
        public JsonResult SupplierSave(SupplierModel model)
        {
            Supplier aSupplier;
            if (model.SupplierId > 0) //update
            {
                aSupplier = db.Suppliers.FirstOrDefault(a => a.SupplierId == model.SupplierId);
                aSupplier.Name = model.Name;
                aSupplier.Email = model.Email;
                aSupplier.Phone = model.Phone;
                aSupplier.Address = model.Address;
                aSupplier.SupplierSince = model.SupplierSince;
                aSupplier.UpdatedBy = model.CreatedBy;
                aSupplier.UpdatedDate = DateTime.Now;
                db.Entry(aSupplier).State = EntityState.Modified;
            }
            else //Create
            {
                aSupplier = new Supplier();
                aSupplier.Name = model.Name;
                aSupplier.Email = model.Email;
                aSupplier.Phone = model.Phone;
                aSupplier.SupplierSince = model.SupplierSince;
                aSupplier.Address = model.Address;
                aSupplier.Status = true;
                aSupplier.Type = true; // type true for supplier // type false for associate
                aSupplier.CreatedBy = model.CreatedBy;
                aSupplier.CreatedDate = DateTime.Now;
                db.Suppliers.Add(aSupplier);
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
        //associate active/inactive/delete
        public JsonResult ChangeSupplierStatus(long id, int status, int createdBy)
        {
            try
            {
                var supplier = db.Suppliers.Find(id);
                if (status == 1)
                {
                    supplier.Status = true; //active
                }
                else if (status == 0)
                {
                    supplier.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    supplier.Status = null; // delete
                }
                supplier.UpdatedBy = createdBy;
                supplier.UpdatedDate = now.Date;
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupplierInfo()
        {
            return View();
        }
        public JsonResult GetSupplierList(string text)
        {
            var list = new List<Supplier>();
            var alist = new SelectList(db.Suppliers.Where(a => a.Status == true && a.Type == true).Select(a => new { Name = a.Name, SupplierId = a.SupplierId }), "SupplierId", "Name");
            //if (!string.IsNullOrEmpty(text))
            //{
            //    list = db.Suppliers.Where(a => a.Name.ToLower().Contains(text.ToLower()) && a.Status == true).OrderBy(a => a.Name).ToList();
            //    alist = new SelectList(list.Select(a => new { Name = a.Name, SupplierId = a.SupplierId }), "SupplierId", "Name");
            //}
            return Json(alist,JsonRequestBehavior.AllowGet);
        }
        #endregion
        //Supplier Document 
        #region Supplier Document
        [EncryptedActionParameter]
        public ActionResult Document(long? supplierId)
        {
            ViewBag.SupplierId = supplierId;
            return View();
        }
        public PartialViewResult DocumentList(long? supplierId)
        {
            return PartialView();
        }
        //Document Create
        public PartialViewResult DocumentCreate()
        {
            return PartialView();
        }
        public PartialViewResult DocumentCreatePartial()
        {
            return PartialView();
        }
        #endregion
        //Supplier Invoice Info
        #region Supplier Invoice 
        [EncryptedActionParameter]
        public ActionResult SupplierInvoiceInfo(long supplierId)
        {
            return View(db.ViewSuppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult SupplierInvoiceList(long supplierId)
        {
            return PartialView(db.ViewStockImports.Where(a => a.SupplierId == supplierId));
        }
        //Ledger Invoice Info
        [EncryptedActionParameter]
        public ActionResult LedgerInvoice(long? supplierId)
        {
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult LedgerInvoiceList(long? supplierId, int? days, DateTime? from, DateTime? to, int? count, bool isPrint)
        {
            DateTime? start = from;
            DateTime? end = to;
            ViewBag.IsPrint = isPrint;
            var list = new List<ViewStockImport>();
            if(count > 0)
            {
                list = db.ViewStockImports.Where(a => a.SupplierId == supplierId && a.IsReturn != true).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewStockImports.Where(m => m.SupplierId == supplierId && m.IsReturn != true && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewStockImports.Where(m =>  m.SupplierId == supplierId && m.IsReturn != true && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        // ledger invoice print
        public ActionResult LedgerInvPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
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
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        //Ledger Invoice Details
        [EncryptedActionParameter]
        public ActionResult LedgerInvoiceDetails(long? supplierId)
        {
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult LedgerInoiceDetailsList(long? supplierId, int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = db.ViewStockImports.Where(a => a.SupplierId == supplierId && a.IsReturn != true).ToList();
            if(count > 0)
            {
                list = list.Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = list.Where(m => m.Date.Date == countDate.Date).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = list.Where(m => m.Date.Date >= start.Value.Date && m.Date.Date <= end.Value.Date).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        // Ledger Invoie Details print
        public ActionResult LedgerInvDetailsPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
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
            return View(db.Suppliers.FirstOrDefault(a=>a.SupplierId == supplierId));
        }
        //Import payment history list 
        public PartialViewResult ImportPaymentList(long? supplierId, int? days, DateTime? from, DateTime? to, int? count, long? importId)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewPayment>();
            if(importId > 0)
            {
                list = db.ViewPayments.Where(a => a.PaymentId == importId && (a.Type == 3 || a.Type == 2)).ToList();
            }
            else if(supplierId > 0)
            {
                if (count > 0)
                {
                    list = db.ViewPayments.Where(a => (a.SupplierId == supplierId || a.AssociateId == supplierId)).Take((int)count).ToList();
                }
                if (days == 1)
                {
                    DateTime countDate = DateTime.Now;
                    list = db.ViewPayments.Where(m => (m.SupplierId == supplierId || m.AssociateId == supplierId) && DbFunctions.TruncateTime(m.Date) == DbFunctions.TruncateTime(countDate)).ToList();
                }
                if (days > 1)
                {
                    int day = Convert.ToInt32(days - 1);
                    start = DateTime.Now.AddDays(-(day));
                    end = DateTime.Now;
                }
                if (start != null && end != null)
                {
                    list = db.ViewPayments.Where(m => (m.SupplierId == supplierId || m.AssociateId == supplierId) && DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(m.Date) <= DbFunctions.TruncateTime(end)).ToList();
                }
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        public PartialViewResult ImportPaymentListPartial(long? importId)
        {
            ViewBag.ImportId = importId;
            return PartialView();
        }
        //Import Payment Print
        public ActionResult ImportPaymentPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if(splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            return View(db.Suppliers.FirstOrDefault(a=>a.SupplierId == supplierId));
        }
        //Credit report 
        [EncryptedActionParameter]
        public ActionResult CreditReport(long supplierId)
        {
            return View(db.ViewSuppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult CreditReportList(long supplierId)
        {
            return PartialView(db.ViewStockImports.Where(a => a.SupplierId == supplierId && a.DueAmount > 0).OrderBy(a => a.Date).ToList());
        }
        [EncryptedActionParameter]
        public ActionResult CreditPaymentHistory(long supplierId)
        {
            return View(db.ViewSuppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult CreditPaymentList(long supplierId, int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = db.ViewPayments.Where(a => a.SupplierId == supplierId && a.IsCreditPayment == true).ToList();
            if(count > 0)
            {
                list = list.Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = list.Where(m => m.Date.Date == countDate.Date).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = list.Where(m => m.Date.Date >= start.Value.Date && m.Date.Date <= end.Value.Date).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        //Credit Payment History Print
        public ActionResult CreditPaymentPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
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
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        [EncryptedActionParameter]
        public ActionResult CreditPaymentLedger(long supplierId)
        {
            return View(db.ViewSuppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        public PartialViewResult CreditPaymentLedgerList(long supplierId, int? days, DateTime? from, DateTime? to, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = db.ViewPayments.Where(a => a.SupplierId == supplierId && a.IsCreditPayment == true).ToList();
            if(count > 0)
            {
                list = list.Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = list.Where(m => m.Date.Date == countDate.Date).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = list.Where(m => m.Date.Date >= start.Value.Date && m.Date.Date <= end.Value.Date).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        // Credit payment ledger print
        public ActionResult CreditPaymentLedgerPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
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
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        //Import Product unique list From supplier
        public PartialViewResult ImportProductList(long? supplierId, int? days, DateTime? from, DateTime? to, bool? isPrint, int? count)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = new List<ViewImportTransaction>();
            if(count > 0)
            {
                list = db.ViewImportTransactions.Where(a => a.SupplierId == supplierId && a.Quantity > 0).Take((int)count).ToList();
            }
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = db.ViewImportTransactions.Where(a => a.SupplierId == supplierId && a.Quantity > 0 && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(countDate.Date)).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = db.ViewImportTransactions.Where(a => a.SupplierId == supplierId && a.Quantity > 0 && DbFunctions.TruncateTime(a.Date) >= DbFunctions.TruncateTime(start) && DbFunctions.TruncateTime(a.Date) <= DbFunctions.TruncateTime(end)).ToList();
            }

            if (isPrint == true)
            {
                ViewBag.IsPrint = true;
            }
            else
            {
                ViewBag.IsPrint = false;
            }

            return PartialView(list.OrderByDescending(a => a.Date));
        }
        //Import product list with quantity transaction
        public PartialViewResult ImportProductListWithTrans(long? supplierId, int? days, DateTime? from, DateTime? to)
        {
            DateTime? start = from;
            DateTime? end = to;
            var list = db.ViewImportTransactions.Where(a => a.SupplierId == supplierId).ToList();
            if (days == 1)
            {
                DateTime countDate = DateTime.Now;
                list = list.Where(m => m.Date.Date == countDate.Date).ToList();
            }
            if (days > 1)
            {
                int day = Convert.ToInt32(days - 1);
                start = DateTime.Now.AddDays(-(day));
                end = DateTime.Now;
            }
            if (start != null && end != null)
            {
                list = list.Where(m => m.Date.Date >= start.Value.Date && m.Date.Date <= end.Value.Date).ToList();
            }
            return PartialView(list.OrderByDescending(a => a.Date));
        }
        // Import Product Quantity Transaction
        public PartialViewResult QuantityTransaction(int productId, long supplierId, long distributeid)
        {
            ViewBag.ProductId = productId;
            ViewBag.SupplierId = supplierId;
            ViewBag.DistributeId = distributeid;
            return PartialView();
        }
        public PartialViewResult QuantityTransactionList(int productId, long supplierId, long distributeid)
        {
            return PartialView(db.ViewImportTransactions.Where(a => a.ProductId == productId && a.DistributeId == distributeid && a.SupplierId == supplierId).OrderByDescending(a => a.Date).ToList());
        }
        // import product quantity transaction print
        [EncryptedActionParameter]
        public ActionResult ImportQauntityTransactionPrint(int productId, long supplierId)
        {
            ViewBag.ProductId = productId;
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        // Import Product Print 
        public ActionResult ImportProductsPrint(string q)
        {
            long supplierId = 0;
            int days = 0;
            int listype = 0;
            DateTime? from = null;
            DateTime? to = null;
            if (!string.IsNullOrEmpty(q))
            {
                var base64EncodedBytes = Convert.FromBase64String(q);
                var base64String = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var splitbyand = base64String.Split('&');
                supplierId = Convert.ToInt64(Convert.ToUInt64(splitbyand[0].Split('=')[1]));
                days = Convert.ToInt32(Convert.ToUInt32(splitbyand[1].Split('=')[1]));
                if (splitbyand[2].Split('=')[1] != "null")
                {
                    from = Convert.ToDateTime(splitbyand[2].Split('=')[1]);
                    to = Convert.ToDateTime(splitbyand[3].Split('=')[1]);
                }
                listype = Convert.ToInt32(Convert.ToUInt32(splitbyand[4].Split('=')[1]));
            }
            ViewBag.Days = days;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.Listype = listype;
            return View(db.Suppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        #region Supplier debit 
        [EncryptedActionParameter]
        public ActionResult SupplierDebitInfo(long supplierId)
        {
            return View(db.ViewSuppliers.FirstOrDefault(a => a.SupplierId == supplierId));
        }
        #endregion

        #endregion



        #endregion
        //***********************Customer**********************************
        #region Customer
        #region Customer Create/ Update/ Delete
        public ActionResult Customer()
        {
            return View();
        }
        public PartialViewResult CustomerCreate(int? id, bool? createFromPurchaseOrder)
        {
            ViewBag.CustomerId = id;
            ViewBag.IsCreateFromPurchaseOrder = false;
            if(createFromPurchaseOrder == true)
            {
                ViewBag.IsCreateFromPurchaseOrder = true;
            }
            return PartialView();
        }
        public PartialViewResult CustomerCreatePartial(int? id)
        {
            ViewBag.CreditLimit = new SelectList(db.CreditLimits.Where(a => a.Status == true).Select(s => new { s.Id, Name = s.Name + "(" + s.Limit + ")"  }), "Id", "Name");
            if (id > 0)
            {
                Customer aCustomer = db.Customers.FirstOrDefault(a => a.CustomerId == id);
                CustomerModelView model = new CustomerModelView();
                model.CustomerId = aCustomer.CustomerId;
                model.Name = aCustomer.Name;
                model.Email = aCustomer.Email;
                model.Phone = aCustomer.Phone;
                model.IsCreditAllowed = aCustomer.IsCreditAllowed;
                model.CreditLimitId = aCustomer.CreditLimitId;
                model.Address = aCustomer.Address;
                model.MembershipNumber = aCustomer.MembershipNumber;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult CustomerCreateSave(CustomerModelView model)
        {
            Customer aCustomer = new Customer();
            if (model.CustomerId > 0) // customer update
            {
                aCustomer = db.Customers.FirstOrDefault(a => a.CustomerId == model.CustomerId);
                aCustomer.Name = model.Name;
                aCustomer.Email = model.Email;
                aCustomer.Phone = model.Phone;
                aCustomer.IsCreditAllowed = model.IsCreditAllowed;
                aCustomer.CreditLimitId = model.CreditLimitId;
                aCustomer.Address = model.Address;
                aCustomer.MembershipNumber = model.MembershipNumber;
                aCustomer.UpdatedBy = model.CreatedBy;
                aCustomer.UpdatedDate = DateTime.Now;
                db.Entry(aCustomer).State = EntityState.Modified;
            }
            else // customer Create
            {
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
        //************Customer Create end***********************
        public PartialViewResult CustomerList(int? status)
        {
            var list = new List<ViewCustomer>();
            if (status == 1) // active
            {
                list = db.ViewCustomers.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewCustomers.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewCustomers.Where(a => a.Status == null).ToList();
            }
            else if(status == 3) // all debit accounts
            {
                list = db.ViewCustomers.Where(a =>  a.DebitLimitId > 0).ToList();
            }
            else
            {
                list = db.ViewCustomers.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        //Customer active/inactive/delete
        public JsonResult ChangeCustomerStatus(long id, int status, int createdBy)
        {
            try
            {
                var customer = db.Customers.Find(id);
                if (status == 1)
                {
                    customer.Status = true; //active
                }
                else if (status == 0)
                {
                    customer.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    customer.Status = null; // delete
                }
                customer.UpdatedBy = createdBy;
                customer.UpdatedDate = now.Date;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
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
        #endregion
        #region Customer Invoice 
        [EncryptedActionParameter]
        public ActionResult CustomerInvoiceInfo(long? customerId)
        {
            return View(db.ViewCustomers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        public PartialViewResult CustomerInvoiceList(long? customerId)
        {
            var customerInvoiceList = db.PosOrders.Where(a => a.CustomerId == customerId && a.RefundStatus != true).ToList();
            return PartialView(customerInvoiceList);
        }
        public ActionResult CustomerInfo()
        {
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult CustomerCreditInfo(long? customerId)
        {
            ViewBag.CustomerId = customerId;
            return View();
        }
        #endregion
        #endregion
        //***********************Associate**********************************
        #region Associate
        public ActionResult Associate()
        {
            return View();
        }
        public PartialViewResult AssociateCreate(long? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult AssociateCreatePartial(long? id)
        {
            if(id > 0)
            {
                var associate = db.Suppliers.Find(id);
                SupplierModel modelView = new SupplierModel();
                modelView.SupplierId = associate.SupplierId;
                modelView.Name = associate.Name;
                modelView.Phone = associate.Phone;
                modelView.Address = associate.Address;
                return PartialView(modelView);
            }
            return PartialView();
        }
        public JsonResult AssociateSave(SupplierModel model)
        {
            try
            {
                Supplier associate;
                if (model.SupplierId > 0) //update
                {
                    associate = db.Suppliers.Find(model.SupplierId);
                    associate.Name = model.Name;
                    associate.Phone = model.Phone;
                    associate.Address = model.Address;
                    associate.UpdatedBy = model.CreatedBy;
                    associate.UpdatedDate = now.Date;
                    db.Entry(associate).State = EntityState.Modified;
                }
                else //create
                {
                    associate = new Supplier();
                    associate.Name = model.Name;
                    associate.Phone = model.Phone;
                    associate.Address = model.Address;
                    associate.CreatedBy = model.CreatedBy;
                    associate.CreatedDate = now.Date;
                    associate.Status = true;
                    associate.Type = false;
                    db.Suppliers.Add(associate);
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
               return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AssociateList(int? status)
        {
            var list = new List<ViewSupplier>();
            if (status == 1) // active
            {
                list = db.ViewSuppliers.Where(a => a.Status == true && a.Type == false).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewSuppliers.Where(a => a.Status == false && a.Type == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewSuppliers.Where(a => a.Status == null && a.Type == false).ToList();
            }
            else
            {
                list = db.ViewSuppliers.Where(a => a.Status != null && a.Type == false).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        //associate active/inactive/delete
        public JsonResult ChangeAssociateStatus(long id, int status, int createdBy)
        {
            try
            {
                var associate = db.Suppliers.Find(id);
                if (status == 1)
                {
                    associate.Status = true; //active
                }
                else if (status == 0)
                {
                    associate.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    associate.Status = null; // delete
                }
                associate.UpdatedBy = createdBy;
                associate.UpdatedDate = now.Date;
                db.Entry(associate).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAssociateList(int? status)
        {
            var alist = new SelectList("", "");
            if(status == 1) //status 1 for all borrow product list 
            {
                alist = new SelectList(db.ViewBorrowProducts.GroupBy(g => g.AssociateId).Select(a => new { a.FirstOrDefault().AssociateId, a.FirstOrDefault().AssociateName }), "AssociateId", "AssociateName");
            }
            else if(status == 2) //status 2 for paid borrow product list
            {
                alist = new SelectList(db.ViewBorrowProducts.Where(a => a.IsBorrowPaid == true).GroupBy(g => g.AssociateId).Select(a => new { a.FirstOrDefault().AssociateId, a.FirstOrDefault().AssociateName }), "AssociateId", "AssociateName");
            }
            else if(status == 3)
            {
                alist = new SelectList(db.ViewBorrowProducts.Where(a => a.IsBorrowPaid != true).GroupBy(g => g.AssociateId).Select(a => new { a.FirstOrDefault().AssociateId, a.FirstOrDefault().AssociateName }), "AssociateId", "AssociateName");
            }
            return Json(alist,JsonRequestBehavior.AllowGet);
        }

        #endregion
        //***********************Payment Method**********************************
        #region Payment method
        public ActionResult PaymentMethod()
        {
            return View();
        }
        public PartialViewResult PaymentMethodCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult PaymentMethodCreatePartial(int? id)
        {
            var model = new PaymentMethodModelView();
            if(id > 0)
            {
                var method = db.PaymentMethods.FirstOrDefault(a => a.Id == id);
                model.Id = method.Id;
                model.MethodName = method.MethodName;
                model.Type = method.Type;
            }
            return PartialView(model);
        }
        public JsonResult PaymentMethodSave(PaymentMethodModelView model)
        {
            var method = new PaymentMethod();
            if (model.Id > 0)
            {
                method = db.PaymentMethods.FirstOrDefault(a => a.Id == model.Id);
                method.MethodName = model.MethodName;
                method.Type = model.Type;
                method.UpdatedBy = model.CreatedBy;
                method.UpdatedDate = DateTime.Now;
                db.Entry(method).State = EntityState.Modified;
            }
            else
            {
                method.MethodName = model.MethodName;
                method.Type = model.Type;
                method.Status = true;
                method.CreatedBy =(int)model.CreatedBy;
                method.CreatedDate = DateTime.Now;
                db.PaymentMethods.Add(method);
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
        public PartialViewResult PaymentMethodList()
        {
            return PartialView(db.ViewPaymentMethods.ToList());
        }
        public JsonResult DeletePaymentMethod(int id)
        {
            var method = db.PaymentMethods.FirstOrDefault(a => a.Id == id);
            db.Entry(method).State = EntityState.Deleted;
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
        public JsonResult ChangePaymentMethodStatus(int id, int status)
        {
            var method = db.PaymentMethods.FirstOrDefault(a => a.Id == id);
            if (status == 1)
            {
                method.Status = true;
            }
            else
            {
                method.Status = false;
            }
            db.Entry(method).State = EntityState.Modified;
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

        public JsonResult GetMethodList()
        {
            var alist = new SelectList(db.PaymentMethods.Where(a => a.Status == true).Select(a => new { a.MethodName, a.Id }), "Id", "MethodName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        #endregion
        //***********************Set default Account**********************************
        #region Default Account 
        public PartialViewResult SetInDefault(int id, int type)
        {
            ViewBag.Id = id;
            ViewBag.Type = type;
            return PartialView();
        }
        public PartialViewResult SetInDefaultPartial(int id, int type)
        {
            var list = new List<ViewAssignedMethodName>();
            var defaultList = new List<DefaultAccount>();
            if (type == 1)
            {
                defaultList = db.DefaultAccounts.Where(a => a.MethodId == id && a.Type == true).ToList();
                list = db.ViewAssignedMethodNames.Where(a => a.MethodId == id && a.Status == true && (a.InOut == 0 || a.InOut == type)).ToList();
            }
            else if(type == 2)
            {
                defaultList = db.DefaultAccounts.Where(a => a.MethodId == id && a.Type == false).ToList();
                list = db.ViewAssignedMethodNames.Where(a => a.MethodId == id && a.Status == true && (a.InOut == 0 || a.InOut == type)).ToList();
            }
            ViewBag.DefaultList = defaultList;
            return PartialView(list);
        }
        public JsonResult DefaultAccountSave(IList<DefaultAccountModel> model, int UserId, int type, int methodId)
        {
            try
            {
                bool defaultType = true;
                if (type == 2)
                {
                    defaultType = false;
                }
                if (model != null)
                {
                    DefaultAccount account;
                   
                    foreach (var list in model)
                    {
                        var IsExist = db.DefaultAccounts.FirstOrDefault(a => a.MethodId == list.MethodId && a.PaymentTypeId == list.PaymentTypeId && a.Type == defaultType);
                        if(IsExist != null)
                        {
                            if(IsExist.AccountId != list.AccountId)
                            {
                                IsExist.AccountId = list.AccountId;
                                IsExist.UpdatedBy = UserId;
                                IsExist.UpdatedDate = DateTime.Now;
                                db.Entry(IsExist).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            account = new DefaultAccount();
                            account.AccountId = list.AccountId;
                            account.MethodId = list.MethodId;
                            account.PaymentTypeId = list.PaymentTypeId;
                            account.Type = defaultType;
                            account.CreatedBy = UserId;
                            account.CreatedDate = DateTime.Now;
                            db.DefaultAccounts.Add(account);
                        }
                        db.SaveChanges();
                    }
                    var defaultList = db.DefaultAccounts.Where(a => a.MethodId == methodId && a.Type == defaultType).ToList();
                    foreach(var list in defaultList)
                    {
                        var newList = model.FirstOrDefault(a => a.AccountId == list.AccountId);
                        if(newList == null)
                        {
                            db.Entry(list).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var defaultList = db.DefaultAccounts.Where(a => a.MethodId == methodId && a.Type == defaultType).ToList();
                    foreach (var list in defaultList)
                    {
                        if(list.PaymentTypeId != 1)
                        {
                            db.Entry(list).State = EntityState.Deleted;
                            db.SaveChanges();
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
        //***********************Leave Type**********************************
        #region Leave Type
        public ActionResult LeaveType()
        {
            return View();
        }
        public PartialViewResult LeaveTypeLists(int? status)
        {
            var list = new List<ViewLeaveType>();
            if(status != null)
            {
                list = db.ViewLeaveTypes.Where(a => a.Status == status).ToList();
            }
            else
            {
                list = db.ViewLeaveTypes.Where(a => a.Status != 2).ToList();
            }
            return PartialView(list);
        }
        public PartialViewResult LeaveTypeCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult LeaveTypeCreatePartial(int? id)
        {
            if(id > 0)
            {
                var type = db.LeaveTypes.Find(id);
                LeaveTypeModelView model = new LeaveTypeModelView();
                model.Id = type.Id;
                model.Name = type.Name;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult LeaveTypeSave(LeaveTypeModelView model)
        {
            var type = new LeaveType();
            if(model.Id > 0)
            {
                type = db.LeaveTypes.Find(model.Id);
                type.Name = model.Name;
                type.UpdatedBy = model.CreatedBy;
                type.UpdatedDate = now;
                db.Entry(type).State = EntityState.Modified;
            }
            else
            {
                type.Name = model.Name;
                type.Status = 0;
                type.CreatedBy = (int)model.CreatedBy;
                type.CreatedDate = now;
                type.CreatedDate = now;
                db.LeaveTypes.Add(type);
            }
            try
            {
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error",JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //leav type active/inactive/delete
        public JsonResult ChangeLeaveTypeStatus(int id, int status)
        {
            var  type = db.LeaveTypes.Find(id);
            type.Status = status;
            db.Entry(type).State = EntityState.Modified;
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
        //***********************Tag**********************************
        #region Tag
        public ActionResult Tag()
        {
            return View();
        }
        public PartialViewResult TagCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult TagCreatePartial(int? id)
        {
            if(id > 0)
            {
                var tag = db.Tags.Find(id);
                TagModelView model = new TagModelView();
                model.Id = tag.Id;
                model.TagName = tag.TagName;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult TagSave(TagModelView model)
        {
            try
            {
                var tag = new Tag();
                if (model.Id > 0)
                {
                    tag = db.Tags.Find(model.Id);
                    tag.TagName = model.TagName;
                    tag.UpdatedBy = model.CreatedBy;
                    tag.UpdatedDate = now.Date;
                    db.Entry(tag).State = EntityState.Modified;
                }
                else
                {
                    tag = new Tag();
                    tag.TagName = model.TagName;
                    tag.Status = true;
                    tag.CreatedBy = (int)model.CreatedBy;
                    tag.CreatedDate = now.Date;
                    db.Tags.Add(tag);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult TagList(int? status)
        {
            var list = new List<ViewTag>();
            if (status == 1) // active
            {
                list = db.ViewTags.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewTags.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewTags.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewTags.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.TagName).ToList());
        }
        public JsonResult ChangeTagStatus(int id, int status)
        {
            try
            {
                var tag = db.Tags.Find(id);
                if (status == 1)
                {
                    tag.Status = true; //active
                }
                else if (status == 0)
                {
                    tag.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    tag.Status = null; // delete
                }
                db.Entry(tag).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
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
        public JsonResult GetTagById(int tagId)
        {
            var tag = db.Tags.Find(tagId);
            return Json(tag, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetTagByProductId(int id)
        {
            if (id > 0)
            {
                var tagList = db.ViewTagItems.Where(a => a.OwnerId == id && a.Type == 1 && a.Status == true).Select(s => new { Text = s.TagName, Value = s.TagId }).ToList();
                return Json(tagList, JsonRequestBehavior.AllowGet);
            }
            return Json("NotFound", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Tag Item**********************************
        #region Tag Item
        [EncryptedActionParameter]
        public ActionResult TagItem(int id)
        {
            return View(db.ViewTags.FirstOrDefault(a => a.Id == id));
        }
        public PartialViewResult TagItemList(int id)
        {
            return PartialView(db.ViewTagItems.Where(a => a.TagId == id && a.Type == 1 && a.Status == true).ToList());
        }
        public JsonResult TagItemSave(IList<TagItem> items, int CreatedBy,int tagId, long[] deleteItemIds)
        {
            try
            {
                TagItem tagItem;
                var tag = db.Tags.Find(tagId);
                tag.UpdatedBy = CreatedBy;
                tag.UpdatedDate = now.Date;
                db.Entry(tag).State = EntityState.Modified;
                db.SaveChanges();
                //Tag item save
                foreach (TagItem item in items)
                {
                    if (item.Id > 0)
                    {
                        
                    }
                    else
                    {
                        item.Type = 1; // type 1 for product tag
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
        #endregion
        //***********************Schedule**********************************
        #region Schedule
        public ActionResult Schedule()
        {
            return View();
        }
        public PartialViewResult ScheduleCreate(int? id, bool? isCreateFromEvent)
        {
            ViewBag.Id = id;
            ViewBag.IsMultiSchedule = false;
            if(id > 0)
            {
                ViewBag.IsMultiSchedule = db.Schedules.Find(id).IsMultiSchedule;
            }
            ViewBag.IsCreateFromEvent = false;
            if(isCreateFromEvent == true)
            {
                ViewBag.IsCreateFromEvent = true;
            }
            return PartialView();
        }
        public PartialViewResult ScheduleCreatePartial(int? id)
        {
            if(id > 0)
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
                if(model.Id > 0)
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
                if(model.ScheduleItems != null)
                {
                    foreach(var item in model.ScheduleItems)
                    {
                        if(item.Id > 0)
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
                if(model.deleteIds != null)
                {
                    foreach(var id in model.deleteIds)
                    {
                        schItem = db.ScheduleItems.Find(id);
                        schItem.Status = false;
                        db.Entry(schItem).State = EntityState.Modified;
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
            return Json(db.Schedules.Where(a => a.Status == true).OrderBy(o => o.Name).Select(s => new { s.Id, s.Name }).ToList(),JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Restriction**********************************
        #region Restriction
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
            if(id > 0)
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
                if(model.Id > 0)
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
            catch(Exception)
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
        //***********************Credit Limit**********************************
        #region Credit limit
        public ActionResult CreditLimit()
        {
            return View();
        }
        public PartialViewResult CreditLimitCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult CreditLimitCreatePartial(int? id)
        {
            if(id > 0)
            {
                var limit = db.CreditLimits.Find(id);
                CreditLimitModelView model = new CreditLimitModelView();
                model.Id = limit.Id;
                model.Name = limit.Name;
                model.Limit = limit.Limit;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult CreditLimitSave(CreditLimitModelView model)
        {
            try
            {
                CreditLimit limit;
                if(model.Id > 0)
                {
                    limit = db.CreditLimits.Find(model.Id);
                    limit.Name = model.Name;
                    limit.Limit = model.Limit;
                    limit.UpdatedBy = model.CreatedBy;
                    limit.UpdatedDate = now.Date;
                    db.Entry(limit).State = EntityState.Modified;
                }
                else
                {
                    limit = new CreditLimit();
                    limit.Name = model.Name;
                    limit.Limit = model.Limit;
                    limit.Status = true;
                    limit.CreatedBy = model.CreatedBy;
                    limit.CreatedDate = now.Date;
                    db.CreditLimits.Add(limit);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult CreditLimitList(int? status)
        {
            var list = new List<ViewCreditLimit>();
            if (status == 1) // active
            {
                list = db.ViewCreditLimits.Where(a => a.Status == true).ToList();
            }
            else if (status == 0) // inactive
            {
                list = db.ViewCreditLimits.Where(a => a.Status == false).ToList();
            }
            else if (status == 2) // delete
            {
                list = db.ViewCreditLimits.Where(a => a.Status == null).ToList();
            }
            else
            {
                list = db.ViewCreditLimits.Where(a => a.Status != null).ToList();
            }
            return PartialView(list.OrderBy(a => a.Name).ToList());
        }
        public JsonResult ChangeCreditLimitStatus(int id, int status)
        {
            try
            {
                var limit = db.CreditLimits.Find(id);
                if (status == 1)
                {
                    limit.Status = true; //active
                }
                else if (status == 0)
                {
                    limit.Status = false; //Inactive
                }
                else if (status == 2)
                {
                    limit.Status = null; // delete
                }
                db.Entry(limit).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        //***********************Debit Limit**********************************
        #region Debit Limit
        public PartialViewResult DebitLimit(long? customerId, long? debitLimitId, bool? isAmountEdit)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.DebitLimitId = debitLimitId;
            ViewBag.IsAmountEdit = false;
            if(isAmountEdit == true)
            {
                ViewBag.IsAmountEdit = true;
            }
            return PartialView();
        }
        public PartialViewResult DebitLimitPartial(long? debitLimitId, bool? isAmountEdit)
        {
            ViewBag.IsAmountEdit = false;
            if(isAmountEdit == true)
            {
               ViewBag.IsAmountEdit = true;
            }
            if (debitLimitId > 0)
            {
                var debitLimit = db.DebitLimits.Find(debitLimitId);
                DebitLimitModelView model = new DebitLimitModelView();
                model.Id = debitLimit.Id;
                model.Limit = debitLimit.Limit;
                model.Amount = debitLimit.Amount;
                return PartialView(model);
            }
            return PartialView();
        }
        public JsonResult DebitLimitSave(DebitLimitModelView model)
        {
            try
            {
                DebitLimit adebitLimit;
                if(model.Id > 0)
                {
                    adebitLimit = db.DebitLimits.Find(model.Id);
                    if(model.IsAmountEdit == true)
                    {
                        adebitLimit.Amount = model.Amount;
                    }
                    else
                    {
                        adebitLimit.Limit = model.Limit;
                    }
                    adebitLimit.UpdatedBy = model.CreatedBy;
                    adebitLimit.UpdatedDate = now.Date;
                    db.Entry(adebitLimit).State = EntityState.Modified;
                }
                else
                {
                    adebitLimit = new DebitLimit();
                    adebitLimit.CustomerId = model.CustomerId;
                    adebitLimit.Limit = model.Limit;
                    adebitLimit.Amount = 0;
                    adebitLimit.CreatedBy = model.CreatedBy;
                    adebitLimit.CreatedDate = now.Date;
                    adebitLimit.Status = true;
                    db.DebitLimits.Add(adebitLimit);

                    db.SaveChanges();

                    var customer = db.Customers.Find(model.CustomerId);
                    customer.DebitLimitId = adebitLimit.Id;
                    db.Entry(customer).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customer Debit Info
        [EncryptedActionParameter]
        public ActionResult CustomerDebitInfo(long customerId)
        {
            return View(db.ViewCustomers.FirstOrDefault(a => a.CustomerId == customerId));
        }
        #endregion

       
    }
}