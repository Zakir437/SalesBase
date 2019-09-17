using PointOfSale.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Offer
{
    public class OfferModelView
    {
        public long? Id { get; set; }
        public int? SubOfferId { get; set; }
        public int? ScheduleId { get; set; }
        public int Type { get; set; }
        [Required(ErrorMessage = "Please enter offer name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string OfferName { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> ValidityTime { get; set; }
        public int? ValidityDays { get; set; }
        public bool IsDateValidity { get; set; }
        public bool IsEditable { get; set; }

        public decimal ActualPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public int DiscPercentage { get; set; }
        public decimal DiscAmount { get; set; }

        //coupon
        [Required(ErrorMessage = "Please enter coupon code")]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Coupon should be minimum 4 and maximum 8 characters long.")]
        [Remote("CouponCodeExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Coupon code Already Exist !!!")]
        public string Coupon { get; set; }
        public bool IsCouponApplicable { get; set; }

        public IList<OfferItem> Items { get; set; }
        public IList<DiscountItem> DiscountItems { get; set; }
        public DiscountItem DiscItem { get; set; }
        public bool? IsSingleOffer { get; set; }

        public int CreatedBy { get; set; }
    }
}