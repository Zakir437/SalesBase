using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.POS
{
    public class CouponModelView
    {
        public long? Id { get; set; }
        public long? OfferId { get; set; }
        [Required(ErrorMessage ="Please enter coupon code")]
        [StringLength(50,MinimumLength =1,ErrorMessage ="Invalid")]
        [Remote("CouponCodeExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This code already exists..!!!")]
        public string Code { get; set; }
        //[Required(ErrorMessage = "Please enter amount")]
        public int Amount { get; set; }
        public int Percentage { get; set; }
        public bool IsPercentile { get; set; }
        public bool IsPriceRange { get; set; }
        public bool IsInfinite { get; set; }
        public int Percentile { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public decimal? MaximumAmount { get; set; }
        public int? FromPrice { get; set; }
        public int? ToPrice { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> ValidityDate { get; set; }
        public Nullable<DateTime> ValidityTime { get; set; }
        public int? ValidityDays { get; set; }
        public bool IsDateValidity { get; set; }
        public int Type { get; set; }
        public int CreatedBy { get; set; }
    }
}