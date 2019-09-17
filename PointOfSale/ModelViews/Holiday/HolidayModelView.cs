using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Holiday
{
    public class HolidayModelView
    {
        public int? Id { get; set; }
        public int? HolidayPackId { get; set; }
        [Required(ErrorMessage = "Please type holiday name.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string HolidayName { get; set; }
        public bool IsMultipleDay { get; set; }
        public string MonthName { get; set; }
        public Nullable<int> MonthCount { get; set; }
        public int TotalDay { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Year { get; set; }
        public int? CreatedBy { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
    }
}