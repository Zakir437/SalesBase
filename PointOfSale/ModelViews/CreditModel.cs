using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class CreditModel
    {
        [Required(ErrorMessage = "Please enter credit amount")]
        public decimal? CreditAmount { get; set; }
    }
}