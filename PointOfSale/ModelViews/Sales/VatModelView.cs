using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Sales
{
    public class VatModelView
    {
        public int? VatId { get; set; }
        [Required(ErrorMessage = "Please enter vat rate")]
        [Range(0, 100, ErrorMessage = "Please enter 0 to 100")]
        public int? Rate { get; set; }
        public int? CreatedBy { get; set; }
    }
}