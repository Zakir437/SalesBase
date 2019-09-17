using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.POS
{
    public class EventModelView
    {
        public long? Id { get; set; }
        public int SubOfferId { get; set; }
        public int? ScheduleId { get; set; }
        [Required(ErrorMessage = "Please enter event name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string EventName { get; set; }
        public Nullable<DateTime> ValidityDate { get; set; }
        public Nullable<DateTime> ValidityTime { get; set; }
        public int? ValidityDays { get; set; }
        public bool IsDateValidity { get; set; }

        public int CreatedBy { get; set; }
    }
}