using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class MiscFuncModel
    {
        public int? MiscId { get; set; }

        //tax rate
        [Required(ErrorMessage ="Please enter tax rate")]
        [Range(0,100,ErrorMessage ="Please enter 0 to 100")]
        public int? Rate { get; set; }

        //minimum quantity
        [Required(ErrorMessage = "Please enter minimum quantity")]
        [Range(0, 100, ErrorMessage = "Please enter 0 to 100")]
        public int? MinimumQuantity { get; set; }

        //salary pay day
        [Required(ErrorMessage = "Please enter pay day")]
        [Range(1, 10, ErrorMessage = "Please enter 1 to 10")]
        public int? PayDay { get; set; }

        //tax
        public bool? TaxFunction { get; set; }

        //Shop time
        public bool Is24Hours { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        //Age Restricted
        [Required(ErrorMessage ="Please enter restricted age")]
        public int Age { get; set; }

        //Points Value
        [Required(ErrorMessage = "Please enter points value")]
        public decimal PointsValue { get; set; }
        [Required(ErrorMessage = "Please enter points")]
        public int Points { get; set; }
        public bool? Status { get; set; }

        public int? Value { get; set; }
    }
}