using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Holiday
{
    public class HolidayPachakgeModelView
    {
        public int? HolidayPackId { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum lenth 3")]
        [Remote("HolidayPackNameExists", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Package name already exists")]
        public string HolidayPackName { get; set; }
        [Required(ErrorMessage = "Please enter number of paid leave")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Paid leave must be a an integer number")]
        [Range(0, int.MaxValue, ErrorMessage = "Paid leave must be a an integer number")]
        [Display(Name = "No.of paid Leave")]
        public int NoOfPaidLeave { get; set; }
        public int? CreatedBy { get; set; }
    }
}