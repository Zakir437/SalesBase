using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class CashModel
    {
        [Required(ErrorMessage = "Please select a category")]
        public int? CashBodyId { get; set; }
        [Required(ErrorMessage ="Please enter pay amount")]
        public decimal? AmountPaid { get; set; }
    }
}