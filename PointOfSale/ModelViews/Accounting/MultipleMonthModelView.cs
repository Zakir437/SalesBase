using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Accounting
{
    public class MultipleMonthModelView
    {
        public int UserId { get; set; }
        public string MonthName { get; set; }
        [Required(ErrorMessage ="Please select a month")]
        public DateTime MonthDate { get; set; }
        public int CreatedBy { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        public DateTime SelectedDate { get; set; }
    }
}