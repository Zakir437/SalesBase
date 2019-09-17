using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.HR
{
    public class SalaryModelView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage ="Please enter amount")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage ="Please select a date")]
        public Nullable<DateTime> EffectiveDate { get; set; }
        public int CreatedBy { get; set; }
    }
}