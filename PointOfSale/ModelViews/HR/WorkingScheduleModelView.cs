using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PointOfSale.Models;

namespace PointOfSale.ModelViews.HR
{
    public class WorkingScheduleModelView
    {
        public int? ScheduleId { get; set; }
        [Required(ErrorMessage ="Please enter schedule name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string ScheduleName { get; set; }
        public IList<WorkingDay> WorkingDays { get; set; }
        public int? CreatedBy { get; set; }
    }
}