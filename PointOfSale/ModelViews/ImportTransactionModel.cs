using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class ImportTransactionModel
    {
        public long? ImportTransactionId { get; set; }
        [Required(ErrorMessage ="Please enter quantity")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid quantity")]
        public decimal Quantity { get; set; }
        [Required(ErrorMessage = "Please enter cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid cost")]
        public decimal Cost { get; set; }
    }
}