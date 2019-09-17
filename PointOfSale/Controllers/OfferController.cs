using PointOfSale.Helpers;
using PointOfSale.Models;
using PointOfSale.ModelViews.Offer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.Controllers
{
    public class OfferController : Controller
    {
        #region Private Properties
        private PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        static private int offset = Convert.ToInt32(ConfigurationManager.AppSettings["localTime"]);
        DateTime now = DateTime.UtcNow.AddMinutes(offset);
        #endregion
        #region Offer
        public ActionResult Offer()
        {
            ViewBag.CouponCount = db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == true).Count();
            return View();
        }
        public ActionResult OfferCreate()
        {
            return View();
        }
        public PartialViewResult DiscountTab()
        {
            ViewBag.DiscItemCount = db.DiscountItems.Where(a => a.Status == true).Count();
            ViewBag.AmountDiscCount = db.Offers.Where(a => a.IsCouponApplicable == false && a.Type == 6 && a.Status == true).Count();
            ViewBag.DelChargeDiscCount = db.Offers.Where(a => a.IsCouponApplicable == false && a.Type == 5 && a.Status == true).Count();

            return PartialView();
        }
        public PartialViewResult CouponTab()
        {
            return PartialView();
        }
        public PartialViewResult PointsTab()
        {
            return PartialView();
        }

        public PartialViewResult DiscItemTab()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return PartialView();
        }
        public PartialViewResult AmountTab()
        {
            return PartialView();
        }
        public PartialViewResult DeliveryChargeTab()
        {
            return PartialView();
        }
        #endregion

        #region Coupon
        public ActionResult CouponOffer()
        {
            ViewBag.CouponCount = db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == true).Count();
            return View();
        }
        public ActionResult CouponOfferType()
        {
            return View();
        }
        [EncryptedActionParameter]
        public ActionResult CouponOfferCreate(int subOfferId)
        {
            ViewBag.SubOfferId = subOfferId;
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            ViewBag.ScheduleList = new SelectList(db.Schedules.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            return View();
        }
        public JsonResult CouponOfferSave(OfferModelView model)
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
                    offer.SubOfferId = model.SubOfferId;
                    offer.ScheduleId = model.ScheduleId;
                    offer.Type = model.Type;
                    offer.OfferName = model.OfferName;
                    offer.IsEditable = model.IsEditable;
                    offer.IsCouponApplicable = true;
                    offer.Coupon = model.Coupon;
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

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult CouponList(int? sortTypeId, int? subOfferId, string selectedIds, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewOffer>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                long offerId = 0;
                var offer = new ViewOffer();
                foreach (var id in selectedIds.Split(','))
                {
                    offerId = Convert.ToInt64(id);
                    offer = db.ViewOffers.FirstOrDefault(a => a.Id == offerId);
                    if (offer != null)
                    {
                        list.Add(offer);
                    }
                }
            }
            else
            {
                if(subOfferId > 0)
                {
                    if (sortTypeId == 1) // active
                    {
                        list = db.ViewOffers.Where(a => a.SubOfferId == subOfferId && a.Status == true).ToList();
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        list = db.ViewOffers.Where(a => a.SubOfferId == subOfferId && a.Status == false).ToList();
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        list = db.ViewOffers.Where(a => a.SubOfferId == subOfferId && a.Status == null).ToList();
                    }
                }
                else
                {
                    if (sortTypeId == 1) // active
                    {
                        list = db.ViewOffers.Where(a => a.IsCouponApplicable == true && a.Status == true).Take((int)count).ToList();
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        list = db.ViewOffers.Where(a => a.IsCouponApplicable == true && a.Status == false).ToList();
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        list = db.ViewOffers.Where(a => a.IsCouponApplicable == true && a.Status == null).ToList();
                    }
                }
            }

            if (sortBy > 0)
            {
                if (sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if (sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if (sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.OfferName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if (isPrint == true)
            {
                ViewBag.Isprint = true;
            }

            return PartialView(list);
        }

        public PartialViewResult CouponValidityEdit(string selectedIds, bool? isDiscountOffer)
        {
            ViewBag.SelectedIds = selectedIds;
            ViewBag.IsDiscountOffer = false;
            if(isDiscountOffer == true)
            {
                ViewBag.IsDiscountOffer = true;
            }
            return PartialView();
        }
        public PartialViewResult CouponValidityEditPartial()
        {
            return PartialView();
        }
        public JsonResult CouponValidityUpdate(OfferModelView model, string SelectedIds)
        {
            try
            {
                Offer offer;
                if (!string.IsNullOrEmpty(SelectedIds)) // validity update
                {
                    if (model.IsDateValidity == false)
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);

                        model.StartDate = DateTime.Now;
                        model.EndDate = date;
                    }
                    long offerId = 0;
                    foreach (var id in SelectedIds.Split(','))
                    {
                        offerId = Convert.ToInt64(id);
                        offer = db.Offers.Find(offerId);
                        offer.StartDate = (DateTime)model.StartDate;
                        offer.EndDate = (DateTime)model.EndDate;
                        offer.UpdatedBy = model.CreatedBy;
                        offer.UpdatedDate = DateTime.Now;
                        db.Entry(offer).State = EntityState.Modified;
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

        public JsonResult CouponOfferUpdate(OfferModelView model, string SelectedIds)
        {
            try
            {
                Offer offer;
                if (!string.IsNullOrEmpty(SelectedIds)) // validity update
                {
                    if (model.IsDateValidity == false)
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);

                        model.StartDate = DateTime.Now;
                        model.EndDate = date;
                    }
                    long offerId = 0;
                    foreach (var id in SelectedIds.Split(','))
                    {
                        offerId = Convert.ToInt64(id);
                        offer = db.Offers.Find(offerId);
                        offer.StartDate = (DateTime)model.StartDate;
                        offer.EndDate = (DateTime)model.EndDate;
                        offer.UpdatedBy = model.CreatedBy;
                        offer.UpdatedDate = DateTime.Now;
                        db.Entry(offer).State = EntityState.Modified;
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

        #region Delivery Charge Coupon
        public ActionResult DeliveryChargeCoupon()
        {
            ViewBag.Count = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status != null).Count();
            return View();
        }
        public PartialViewResult DChargeCouponCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult DChargeCouponCreatePartial(int? id)
        {
            if (id > 0)
            {
                var dChargeCoupon = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.Id == id);
                CouponModelView model = new CouponModelView();
                model.Id = dChargeCoupon.Id;
                model.OfferId = dChargeCoupon.OfferId;
                model.OfferName = dChargeCoupon.OfferName;
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

        public JsonResult CheckDelChargeCouponPriceRange(int? fromPrice, int? toPrice, int? price, int? id)
        {
            try
            {
                if (fromPrice > 0 && toPrice > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true && a.Id != id).Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true && a.Id != id).Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true && a.Id != id).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true).Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true).Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.IsPriceRange == true).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if (price > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewDeliveryChargeCoupons.Any(a => a.IsCouponApplicable == true && a.Id != id && a.FromPrice <= price && price <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (db.ViewDeliveryChargeCoupons.Any(a => a.IsCouponApplicable == true && a.FromPrice <= price && price <= a.ToPrice))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
                        offer.StartDate = (DateTime) model.StartDate;
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
                    offer.OfferName = model.OfferName;
                    offer.Coupon = model.Code;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {

                    offer = new Offer();
                    offer.Type = 3;
                    offer.SubOfferId = 7;
                    offer.IsCouponApplicable = true;
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
                    offer.OfferName = model.OfferName;
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
                    if (model.IsPriceRange == true)
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
        public PartialViewResult DChargeCouponList(int sortTypeId, string selectedIds, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewDeliveryChargeCoupon>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                int itemId = 0;
                var discItem = new ViewDeliveryChargeCoupon();
                foreach (var id in selectedIds.Split(','))
                {
                    itemId = Convert.ToInt32(id);
                    discItem = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.Id == itemId);
                    if (discItem != null)
                    {
                        list.Add(discItem);
                    }
                }
            }
            else if (sortTypeId > 0)
            {
                if (sortTypeId == 1) // active
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == true).Take((int)count).ToList();
                }
                else if (sortTypeId == 2) //inactive
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == false).ToList();
                }
                else if (sortTypeId == 3) // delete
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == null).ToList();
                }
            }

            if (sortBy > 0)
            {
                if (sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if (sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if (sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.OfferName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if (isPrint == true)
            {
                ViewBag.Isprint = true;
            }
            return PartialView(list);
        }
        #endregion

        #region Amount Coupon
        public ActionResult AmountCoupon()
        {
            ViewBag.Count = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status != null).Count();
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
                model.OfferName = amountCoupon.OfferName;
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

        public JsonResult CheckAmountCouponPriceRange(int? fromPrice, int? toPrice, int? price, int? id)
        {
            try
            {
                if (fromPrice > 0 && toPrice > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.FromPrice <= fromPrice && fromPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.FromPrice <= toPrice && toPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.Id != id).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice ) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice == 0 && a.Id != id))
                        {
                            var discamountCoupon = db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == true && a.ToPrice == 0);
                            if (discamountCoupon.FromPrice <= fromPrice || discamountCoupon.FromPrice <= toPrice)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {

                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.ToPrice > 0).Any(a =>  (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice == 0))
                        {
                            var discamountCoupon = db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == true && a.ToPrice == 0);
                            if (discamountCoupon.FromPrice <= fromPrice || discamountCoupon.FromPrice <= toPrice)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else if (price > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.Id != id && a.FromPrice <= price && price <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice == 0 && a.Id != id))
                        {
                            if (db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == true && a.ToPrice == 0).FromPrice <= price)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice > 0 && a.FromPrice <= price && price <= a.ToPrice))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == true && a.ToPrice == 0))
                    {
                        if (db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == true && a.ToPrice == 0).FromPrice <= price)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
                    if (model.IsInfinite == true)
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
                        offer.StartDate = (DateTime) model.StartDate;
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
                    offer.OfferName = model.OfferName;
                    offer.Coupon = model.Code;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {
                    offer = new Offer();
                    offer.Type = 4;
                    offer.SubOfferId = 8;
                    offer.IsCouponApplicable = true;
                    if (model.IsDateValidity)
                    {
                        offer.StartDate = (DateTime) model.StartDate;
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
                    offer.OfferName = model.OfferName;
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
        public PartialViewResult AmountCouponList(int sortTypeId, string selectedIds, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewAmountCoupon>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                int itemId = 0;
                var discItem = new ViewAmountCoupon();
                foreach (var id in selectedIds.Split(','))
                {
                    itemId = Convert.ToInt32(id);
                    discItem = db.ViewAmountCoupons.FirstOrDefault(a => a.Id == itemId);
                    if (discItem != null)
                    {
                        list.Add(discItem);
                    }
                }
            }
            else if (sortTypeId > 0)
            {
                if (sortTypeId == 1) // active
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == true).Take((int)count).ToList();
                }
                else if (sortTypeId == 2) //inactive
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == false).ToList();
                }
                else if (sortTypeId == 3) // delete
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == null).ToList();
                }
            }

            if (sortBy > 0)
            {
                if (sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if (sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if (sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.OfferName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if (isPrint == true)
            {
                ViewBag.Isprint = true;
            }
            return PartialView(list);
        }
        #endregion

        #region Discount Offer
        public ActionResult DiscountOfferType()
        {
            return View();
        }
        public ActionResult DiscountOffer()
        {
            ViewBag.CountNumber = db.DiscountItems.Where(a => a.Status != null).Count();
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            return View();
        }
        public ActionResult DiscountOfferCreate()
        {
            ViewBag.CategoryList = new SelectList(db.Categories.Where(a => a.Status == true).Select(s => new { s.CategoryId, s.CategoryName }), "CategoryId", "CategoryName");
            ViewBag.ScheduleList = new SelectList(db.Schedules.Where(a => a.Status == true).Select(s => new { s.Id, s.Name }), "Id", "Name");
            return View();
        }
        public JsonResult DiscountOfferSave(OfferModelView model)
        {
            try
            {
                if(model.DiscountItems != null)
                {
                    //discount item save
                    foreach (DiscountItem item in model.DiscountItems)
                    {
                        if(db.DiscountItems.Any(a => a.ProductId == item.ProductId && a.DistributeId == item.DistributeId) == false)
                        {
                            if (model.IsDateValidity)
                            {
                                item.StartDate = (DateTime)model.StartDate;
                                item.EndDate = (DateTime)model.EndDate;
                            }
                            else
                            {
                                DateTime date;
                                date = now.AddDays((int)model.ValidityDays).Date;
                                date = date.Add(model.ValidityTime.Value.TimeOfDay);

                                item.StartDate = DateTime.Now;
                                item.EndDate = date;
                            }
                            item.ScheduleId = model.ScheduleId;
                            item.IsEditable = model.IsEditable;
                            item.Status = true;
                            item.CreatedBy = model.CreatedBy;
                            item.CreatedDate = now.Date;
                            db.DiscountItems.Add(item);
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

        public JsonResult DiscountItemUpdate(OfferModelView model, string SelectedIds)
        {
            try
            {
                DiscountItem discItem;
                if (model.DiscItem != null) // discount item update
                {
                    discItem = db.DiscountItems.Find(model.DiscItem.Id);
                    if (model.IsDateValidity)
                    {
                        discItem.StartDate = (DateTime)model.StartDate;
                        discItem.EndDate = (DateTime)model.EndDate;
                    }
                    else
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);

                        discItem.StartDate = DateTime.Now;
                        discItem.EndDate = date;
                    }
                    discItem.ScheduleId = model.ScheduleId;
                    discItem.IsEditable = model.IsEditable;
                    discItem.IsFree = model.DiscItem.IsFree;
                    discItem.PercentageOff = model.DiscItem.PercentageOff;
                    discItem.AmountOff = model.DiscItem.AmountOff;
                    discItem.OfferPrice = model.DiscItem.OfferPrice;
                    discItem.UpdatedBy = model.CreatedBy;
                    discItem.UpdatedDate = DateTime.Now;
                    db.Entry(discItem).State = EntityState.Modified;

                    db.SaveChanges();
                }
                else if(!string.IsNullOrEmpty(SelectedIds)) // validity update
                {
                    if (model.IsDateValidity == false)
                    {
                        DateTime date;
                        date = now.AddDays((int)model.ValidityDays).Date;
                        date = date.Add(model.ValidityTime.Value.TimeOfDay);

                        model.StartDate = DateTime.Now;
                        model.EndDate = date;
                    }

                    int discId = 0;
                    foreach (var id in SelectedIds.Split(','))
                    {
                        discId = Convert.ToInt32(id);
                        discItem = db.DiscountItems.Find(discId);
                        discItem.StartDate = (DateTime)model.StartDate;
                        discItem.EndDate =(DateTime) model.EndDate;
                        discItem.UpdatedBy = model.CreatedBy;
                        discItem.UpdatedDate = DateTime.Now;
                        db.Entry(discItem).State = EntityState.Modified;
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
        public PartialViewResult DiscountList(int? sortTypeId, string selectedIds, int? categoryId, int? subCategoryId, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewDiscountItem>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                int itemId = 0;
                var discProduct = new ViewDiscountItem();
                foreach (var id in selectedIds.Split(','))
                {
                    itemId = Convert.ToInt32(id);
                    discProduct = db.ViewDiscountItems.FirstOrDefault(a => a.Id == itemId);
                    if (discProduct != null)
                    {
                        list.Add(discProduct);
                    }
                }
            }
            else if(categoryId > 0)
            {
                var productList = new List<ViewDiscountItem>();
                if (subCategoryId > 0)
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            productList = db.ViewDiscountItems.Where(a => a.ProductId == id.ProductId).ToList();
                            if (productList.Any())
                            {
                                list.AddRange(productList);
                            }
                        }
                    }
                }
                else
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            productList = db.ViewDiscountItems.Where(a => a.ProductId == id.ProductId).ToList();
                            if (productList.Any())
                            {
                                list.AddRange(productList);
                            }
                        }
                    }
                }
                if(list.Any())
                {
                    if (sortTypeId == 1) // active
                    {
                        list = list.Where(a => a.Status == true).ToList();
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        list = list.Where(a => a.Status == false).ToList();
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        list = list.Where(a => a.Status == null).ToList();
                    }
                }
            }
            else
            {
                if (sortTypeId == 1) // active
                {
                    list = db.ViewDiscountItems.Where(a => a.Status == true).Take((int)count).ToList();
                }
                else if (sortTypeId == 2) // inactive
                {
                    list = db.ViewDiscountItems.Where(a => a.Status == false).ToList();
                }
                else if (sortTypeId == 3) // delete
                {
                    list = db.ViewDiscountItems.Where(a => a.Status == null).ToList();
                }
            }


            if(sortBy > 0)
            {
                if(sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if(sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if(sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if(sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if(sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.ProductName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if(isPrint == true)
            {
                ViewBag.Isprint = true;
            }

            return PartialView(list);
        }
        public JsonResult DiscItemStatusChange(int? id, string selectedIds, int status, int createdBy)
        {
            try
            {
                var discItem = new DiscountItem();
                if(!string.IsNullOrEmpty(selectedIds))
                {
                    foreach(var itemId in selectedIds.Split(','))
                    {
                        id = Convert.ToInt32(itemId);
                        discItem = db.DiscountItems.Find(id);
                        if(discItem != null)
                        {
                            if (status == 1)
                            {
                                discItem.Status = true; //active
                            }
                            else if (status == 0)
                            {
                                discItem.Status = false; //Inactive
                            }
                            else if (status == 2)
                            {
                                discItem.Status = null; // delete
                            }
                            discItem.UpdatedBy = createdBy;
                            discItem.UpdatedDate = DateTime.Now;
                            db.Entry(discItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else if(id > 0)
                {
                    discItem = db.DiscountItems.Find(id);
                    if (status == 1)
                    {
                        discItem.Status = true; //active
                    }
                    else if (status == 0)
                    {
                        discItem.Status = false; //Inactive
                    }
                    else if (status == 2)
                    {
                        discItem.Status = null; // delete
                    }
                    discItem.UpdatedBy = createdBy;
                    discItem.UpdatedDate = DateTime.Now;
                    db.Entry(discItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult DiscItemEdit(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult DiscItemEditPartial(int id)
        {
            var discItem = db.DiscountItems.Find(id);
            OfferModelView model = new OfferModelView();
            model.StartDate = discItem.StartDate;
            model.EndDate = discItem.EndDate;
            model.IsEditable = discItem.IsEditable;
            model.ScheduleId = discItem.ScheduleId;
            model.DiscItem = discItem;
            return PartialView(model);
        }

        public PartialViewResult DiscValidityEdit(string selectedIds)
        {
            ViewBag.SelectedIds = selectedIds;
            return PartialView();
        }
        public PartialViewResult DiscValidityEditPartial()
        {
            return PartialView();
        }

        [EncryptedActionParameter]
        public ActionResult DiscountItemPrint(int? sortTypeId, string selectedIds, int? categoryId, int? subCategoryId, int? sortBy, int? count)
        {
            ViewBag.SortTypeId = sortTypeId;
            ViewBag.CategoryId = categoryId;
            ViewBag.SubCategoryId = subCategoryId;
            ViewBag.SortById = sortBy;
            ViewBag.SelectedIds = selectedIds;
            ViewBag.Count = count;
            return View();
        }

        #endregion

        #region DeliveryCharge Discount
        public ActionResult DeliveryChargeDiscount()
        {
            ViewBag.Count = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status != null).Count();
            return View();
        }
        public PartialViewResult DChargeDiscountCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult DChargeDiscountCreatePartial(int? id)
        {
            if (id > 0)
            {
                var dChargeCoupon = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.Id == id);
                CouponModelView model = new CouponModelView();
                model.Id = dChargeCoupon.Id;
                model.OfferId = dChargeCoupon.OfferId;
                model.OfferName = dChargeCoupon.OfferName;
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

        public JsonResult CheckDelChargeDisPriceRange(int? fromPrice, int? toPrice, int? price, int? id)
        {
            try
            {
                if (fromPrice > 0 && toPrice > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Id != id).Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Id != id).Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Id != id).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false).Any(a => a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false).Any(a => a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if (price > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewDeliveryChargeCoupons.Any(a => a.IsCouponApplicable == false && a.Id != id && a.FromPrice <= price && price <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (db.ViewDeliveryChargeCoupons.Any(a => a.IsCouponApplicable == false && a.FromPrice <= price && price <= a.ToPrice))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryChargeDiscountSave(CouponModelView model)
        {
            try
            {
                Offer offer;
                DeliveryChargeCoupon coupon;
                if (model.Id > 0)
                {
                    coupon = db.DeliveryChargeCoupons.Find(model.Id);
                    coupon.Percentage = model.Percentage;
                    coupon.IsPriceRange = true;
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
                    offer.OfferName = model.OfferName;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {

                    offer = new Offer();
                    offer.Type = 5;
                    offer.IsCouponApplicable = false;
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
                    offer.OfferName = model.OfferName;
                    offer.CreatedBy = model.CreatedBy;
                    offer.CreatedDate = now.Date;
                    offer.Status = true;
                    db.Offers.Add(offer);

                    db.SaveChanges();

                    coupon = new DeliveryChargeCoupon();
                    coupon.OfferId = offer.Id;
                    coupon.Percentage = model.Percentage;
                    coupon.IsPriceRange = true;
                    coupon.FromPrice = model.FromPrice;
                    coupon.ToPrice = model.ToPrice;
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
        public PartialViewResult DChargeDiscountList(int sortTypeId, string selectedIds, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewDeliveryChargeCoupon>();
            if(!string.IsNullOrEmpty(selectedIds))
            {
                int itemId = 0;
                var discItem = new ViewDeliveryChargeCoupon();
                foreach (var id in selectedIds.Split(','))
                {
                    itemId = Convert.ToInt32(id);
                    discItem = db.ViewDeliveryChargeCoupons.FirstOrDefault(a => a.Id == itemId);
                    if (discItem != null)
                    {
                        list.Add(discItem);
                    }
                }
            }
            else if(sortTypeId > 0)
            {
                if(sortTypeId == 1) //active
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == true).Take((int) count).ToList();
                }
                else if(sortTypeId == 2) // inactive
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == false).ToList();
                }
                else if(sortTypeId == 3) //delete
                {
                    list = db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == null).ToList();
                }
            }
            if (sortBy > 0)
            {
                if (sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if (sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if (sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.OfferName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if (isPrint == true)
            {
                ViewBag.Isprint = true;
            }
            return PartialView(list);
        }
        #endregion

        #region Amount Discount
        public ActionResult AmountDiscount()
        {
            ViewBag.Count = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status != null).Count();
            return View();
        }
        public PartialViewResult AmountDiscountCreate(int? id)
        {
            ViewBag.Id = id;
            return PartialView();
        }
        public PartialViewResult AmountDiscountCreatePartial(int? id)
        {
            if (id > 0)
            {
                var amountCoupon = db.ViewAmountCoupons.FirstOrDefault(a => a.Id == id);
                CouponModelView model = new CouponModelView();
                model.Id = amountCoupon.Id;
                model.OfferId = amountCoupon.OfferId;
                model.OfferName = amountCoupon.OfferName;
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

        public JsonResult CheckAmountDiscPriceRange(int? fromPrice, int? toPrice, int? price, int? id)
        {
            try
            {
                if (fromPrice > 0 && toPrice > 0)
                {
                    if (id > 0)
                    {
                       
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice > 0 && a.FromPrice <= fromPrice && fromPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice > 0 && a.FromPrice <= toPrice && toPrice <= a.ToPrice && a.Id != id))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.ToPrice > 0 && a.Id != id).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice == 0 && a.Id != id))
                        {
                            var discAmoun = db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.ToPrice == 0);
                            if (discAmoun.FromPrice <= fromPrice || discAmoun.FromPrice <= toPrice)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice > 0 && a.FromPrice <= fromPrice && fromPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice > 0 && a.FromPrice <= toPrice && toPrice <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.ToPrice > 0).Any(a => (fromPrice <= a.FromPrice && a.FromPrice <= toPrice) || (fromPrice <= a.ToPrice && a.ToPrice <= toPrice)))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice == 0))
                        {
                            var discAmoun = db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.ToPrice == 0);
                            if (discAmoun.FromPrice <= fromPrice || discAmoun.FromPrice <= toPrice)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else if (price > 0)
                {
                    if (id > 0)
                    {
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.Id != id && a.FromPrice <= price && price <= a.ToPrice))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice == 0 && a.Id != id))
                        {
                            if (db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.ToPrice == 0).FromPrice <= price)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.FromPrice <= price && price <= a.ToPrice))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    else if (db.ViewAmountCoupons.Any(a => a.IsCouponApplicable == false && a.ToPrice == 0))
                    {
                        if (db.ViewAmountCoupons.FirstOrDefault(a => a.IsCouponApplicable == false && a.ToPrice == 0).FromPrice <= price)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AmountDiscountSave(CouponModelView model)
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
                    if (model.IsInfinite == true)
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
                    offer.OfferName = model.OfferName;
                    offer.UpdatedBy = model.CreatedBy;
                    offer.UpdatedDate = now.Date;
                    db.Entry(offer).State = EntityState.Modified;
                }
                else
                {
                    offer = new Offer();
                    offer.Type = 6;
                    offer.IsCouponApplicable = false;
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
                    offer.OfferName = model.OfferName;
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
        public PartialViewResult AmountDiscountList(int sortTypeId, string selectedIds, int? sortBy, bool? isPrint, int? count)
        {
            var list = new List<ViewAmountCoupon>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                int itemId = 0;
                var discItem = new ViewAmountCoupon();
                foreach (var id in selectedIds.Split(','))
                {
                    itemId = Convert.ToInt32(id);
                    discItem = db.ViewAmountCoupons.FirstOrDefault(a => a.Id == itemId);
                    if (discItem != null)
                    {
                        list.Add(discItem);
                    }
                }
            }
            else if (sortTypeId > 0)
            {
                if (sortTypeId == 1) // active
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == true).Take((int)count).ToList();
                }
                else if (sortTypeId == 2) //inactive
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == false).ToList();
                }
                else if (sortTypeId == 3) // delete
                {
                    list = db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == null).ToList();
                }
            }

            if (sortBy > 0)
            {
                if (sortBy == 1) //date create recent
                {
                    list = list.OrderByDescending(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 2) //date created oldest
                {
                    list = list.OrderBy(a => a.CreatedDate).ToList();
                }
                else if (sortBy == 3) //date expire recent
                {
                    list = list.OrderBy(a => a.EndDate).ToList();
                }
                else if (sortBy == 4) // date expire oldest
                {
                    list = list.OrderByDescending(a => a.EndDate).ToList();
                }
                else if (sortBy == 5) //alphabetical
                {
                    list = list.OrderBy(a => a.OfferName).ToList();
                }
            }

            ViewBag.Isprint = false;
            if (isPrint == true)
            {
                ViewBag.Isprint = true;
            }
            return PartialView(list);
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
            if (!string.IsNullOrEmpty(ids))
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
        public JsonResult PointsSave(IList<PointsModelView> products, int CreatedBy)
        {
            try
            {
                ViewProduct viewProduct;
                Product product;
                if (products != null)
                {
                    foreach (var item in products)
                    {
                        viewProduct = db.ViewProducts.FirstOrDefault(a => a.RowID == item.Id);
                        product = db.Products.FirstOrDefault(a => a.ProductId == viewProduct.ProductId);
                        if(item.Points > 0)
                        {
                            if (product.Points != item.Points)
                            {
                                product.IsPointBased = true;
                                product.Points = item.Points;
                                product.UpdatedBy = CreatedBy;
                                product.DateUpdated = now.Date;
                                db.Entry(product).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            product.IsPointBased = false;
                            product.Points = 0;
                            product.UpdatedBy = CreatedBy;
                            product.DateUpdated = now.Date;
                            db.Entry(product).State = EntityState.Modified;
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

        [EncryptedActionParameter]
        public ActionResult DiscountPrint(int Type, int sortTypeId, string selectedIds, int? sortBy, int? count)
        {
            ViewBag.Type = Type; // discount type
            ViewBag.SortTypeId = sortTypeId;
            ViewBag.SortById = sortBy;
            ViewBag.SelectedIds = selectedIds;
            ViewBag.Count = count;
            return View();
        }

        public JsonResult GetOfferName(int? type, int? subOfferId, int sortTypeId)
        {
            var list = new SelectList("", "");
            if(subOfferId == 0 || subOfferId > 0)
            {
                if(subOfferId > 0)
                {
                    if (sortTypeId == 1) // active
                    {
                        list = new SelectList(db.Offers.Where(a => a.SubOfferId == subOfferId && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        list = new SelectList(db.Offers.Where(a => a.SubOfferId == subOfferId && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        list = new SelectList(db.Offers.Where(a => a.SubOfferId == subOfferId && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                }
                else
                {
                    if (sortTypeId == 1) // active
                    {
                        list = new SelectList(db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        list = new SelectList(db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        list = new SelectList(db.Offers.Where(a => a.IsCouponApplicable == true && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                    }
                }
            }
            else if (type == 3) //delivery charge discount with coupon
            {
                if(sortTypeId == 1) // active
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if(sortTypeId == 2) // inactive
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if(sortTypeId == 3) // delete
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == true && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
            }
            else if (type == 4) //amount discount with coupon
            {
                if(sortTypeId == 1) // active
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if(sortTypeId == 2) // inactive
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if(sortTypeId == 3) // delete
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == true && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
            }
            else if (type == 5) //delivery charge discount 
            {
                if (sortTypeId == 1) // active
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if (sortTypeId == 2) // inactive
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if (sortTypeId == 3) // delete
                {
                    list = new SelectList(db.ViewDeliveryChargeCoupons.Where(a => a.IsCouponApplicable == false && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
            }
            else if (type == 6) //amount discount
            {
                if (sortTypeId == 1) // active
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == true).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if (sortTypeId == 2) // inactive
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == false).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
                else if (sortTypeId == 3) // delete
                {
                    list = new SelectList(db.ViewAmountCoupons.Where(a => a.IsCouponApplicable == false && a.Status == null).Select(s => new { s.Id, s.OfferName }), "Id", "OfferName");
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OfferStatusChange(long? id, int status, int createdBy, string selectedIds)
        {
            try
            {
                Offer offer;
                if(!string.IsNullOrEmpty(selectedIds))
                {
                    foreach(var offerId in selectedIds.Split(','))
                    {
                        id = Convert.ToInt64(offerId);
                        offer = db.Offers.Find(id);
                        if (status == 1)
                        {
                            offer.Status = true; //active
                        }
                        else if (status == 0)
                        {
                            offer.Status = false; //Inactive
                        }
                        else if (status == 2)
                        {
                            offer.Status = null; // delete
                        }
                        offer.UpdatedBy = createdBy;
                        offer.UpdatedDate = DateTime.Now;
                        db.Entry(offer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if(id > 0)
                {
                    offer = db.Offers.Find(id);
                    if (status == 1)
                    {
                        offer.Status = true; //active
                    }
                    else if (status == 0)
                    {
                        offer.Status = false; //Inactive
                    }
                    else if (status == 2)
                    {
                        offer.Status = null; // delete
                    }
                    offer.UpdatedBy = createdBy;
                    offer.UpdatedDate = DateTime.Now;
                    db.Entry(offer).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProduct(int? productId, long? distributeId, string barcode, long? rowId, int? categoryId, int? subCategoryId, int? fromPrice, int? toPrice, int? tagId)
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                if (db.ViewProducts.Any(a => a.Barcode == barcode))
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.Barcode == barcode);
                    return Json(new { product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (productId > 0)
            {
                if (distributeId > 0)
                {
                    if (db.ViewProducts.Any(a => a.DistributeId == distributeId))
                    {
                        return Json(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId && a.DistributeId == distributeId), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (db.Products.Any(a => a.ProductId == productId))
                {
                    return Json(db.ViewProducts.FirstOrDefault(a => a.ProductId == productId), JsonRequestBehavior.AllowGet);
                }
            }
            else if (rowId > 0)
            {
                if (db.ViewProducts.Any(a => a.RowID == rowId))
                {
                    var product = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    return Json(new { product.RowID, product.Price, product.Categorys, product.Points, product.ProductId, product.DistributeId, product.IsUnitWise, product.IsUniqueItem, product.ProductName }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (categoryId > 0)
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
                    foreach (var id in productIds)
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
                var productIds = db.TagItems.Where(a => a.TagId == tagId && a.Status == true && a.Type == 1).GroupBy(g => g.OwnerId).Select(s => new { s.FirstOrDefault().OwnerId }).ToList();
                if (productIds.Any())
                {
                    foreach (var id in productIds)
                    {
                        var products = db.ViewProducts.Where(a => a.ProductId == id.OwnerId && a.Status == true).ToList();
                        if (products != null)
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

        public JsonResult GetProductListForText(string ids, string text, int? categoryId, int? subCategoryId, bool? isDiscountOffer)
        {
            List<ViewProduct> productList = new List<ViewProduct>();
            if (categoryId > 0)
            {
                if (subCategoryId > 0)
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            productList.AddRange(db.ViewProducts.Where(a => a.ProductId == id.ProductId && a.Status == true).ToList());
                        }
                    }
                }
                else
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            productList.AddRange(db.ViewProducts.Where(a => a.ProductId == id.ProductId && a.Status == true).ToList());
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (categoryId > 0)
                {
                    productList = productList.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).Take(5).ToList();
                }
                else
                {
                    productList = db.ViewProducts.Where(a => a.Status == true && a.ProductName.ToLower().Contains(text.ToLower())).Take(5).ToList();
                }
            }
            else
            {
                if (categoryId > 0)
                {
                    productList = productList.Take(5).ToList();
                }
                else
                {
                    productList = db.ViewProducts.Where(a => a.Status == true).Take(5).ToList();
                }
            }

            if (!string.IsNullOrEmpty(ids))
            {
                long rowId = 0;
                foreach (var id in ids.Split(','))
                {
                    rowId = Convert.ToInt64(id);
                    productList.Remove(productList.FirstOrDefault(a => a.RowID == rowId));

                    //var list = db.ViewProducts.FirstOrDefault(a => a.RowID == rowId);
                    //if (list != null)
                    //{
                    //    productList.Remove();
                    //}
                }
            }

            //check is discount item exist
            if (isDiscountOffer == true)
            {
                if (productList.Any())
                {
                    foreach (var product in productList.ToList())
                    {
                        if (db.DiscountItems.Any(a => a.ProductId == product.ProductId && a.DistributeId == product.DistributeId))
                        {
                            productList.Remove(product);
                        }
                    }
                }
            }

            var alist = new SelectList(productList.Select(a => new { a.RowID, a.ProductName }), "RowID", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiscountProduct(string text, int? categoryId, int? subCategoryId, int? sortTypeId)
        {
            List<DiscountItem> productList = new List<DiscountItem>();
            var list = new List<DiscountItem>();

            if (categoryId > 0)
            {
                if (subCategoryId > 0)
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true && a.SubCategoryId == subCategoryId && a.ProductSubCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            list = db.DiscountItems.Where(a => a.ProductId == id.ProductId).ToList();
                            if (list.Any())
                            {
                                productList.AddRange(list);
                            }
                        }
                    }
                }
                else
                {
                    var productIds = db.ViewProductCategories.Where(a => a.CategoryId == categoryId && a.ProductCategoryStatus == true).GroupBy(g => new { g.ProductId }).Select(a => new { a.FirstOrDefault().ProductId }).ToList();
                    if (productIds.Any())
                    {
                        foreach (var id in productIds)
                        {
                            list = db.DiscountItems.Where(a => a.ProductId == id.ProductId).ToList();
                            if (list.Any())
                            {
                                productList.AddRange(list);
                            }
                        }
                    }
                }

                if (productList.Any())
                {
                    if (sortTypeId == 1) // active
                    {
                        productList = productList.Where(a => a.Status == true).ToList();
                    }
                    else if (sortTypeId == 2) // inactive
                    {
                        productList = productList.Where(a => a.Status == false).ToList();
                    }
                    else if (sortTypeId == 3) // delete
                    {
                        productList = productList.Where(a => a.Status == null).ToList();
                    }
                }
            }
            else
            {
                if (sortTypeId == 1) // active
                {
                    productList = db.DiscountItems.Where(a => a.Status == true).ToList();
                }
                else if (sortTypeId == 2) // inactive
                {
                    productList = db.DiscountItems.Where(a => a.Status == false).ToList();
                }
                else if (sortTypeId == 3) // delete
                {
                    productList = db.DiscountItems.Where(a => a.Status == null).ToList();
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                productList = productList.Where(a => a.ProductName.ToLower().Contains(text.ToLower())).ToList();
            }

            var alist = new SelectList(productList.Take(5).Select(a => new { a.Id, a.ProductName }), "Id", "ProductName");
            return Json(alist, JsonRequestBehavior.AllowGet);
        }

    }
}