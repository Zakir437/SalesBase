using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Attendance
{
    public class ManuallyAttModelView
    {
        [Required(ErrorMessage ="Please select an user")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        public DateTime DateTime { get; set; }
        public int? CreatedBy { get; set; }
    }
}