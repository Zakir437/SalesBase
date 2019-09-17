using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PointOfSale.Models;

namespace PointOfSale.ModelViews.Sales
{
    public class ScheduleModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter schedule name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        public bool IsMultiSchedule { get; set; }
        public IList<ScheduleItemModelView> ScheduleItems { get; set; }
        public int[] deleteIds { get; set; }
        public int CreatedBy { get; set; }
    }
}