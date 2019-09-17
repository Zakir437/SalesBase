using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.POS
{
    public class DebitModelView
    {
        [Required(ErrorMessage = "Please enter debit amount")]
        public decimal? DebitAmount { get; set; }
    }
}