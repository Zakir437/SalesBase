using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class PaymentTypeModel
    {
        public int? PaymentTypeId { get; set; }
        [Required (ErrorMessage ="Please enter payment type name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Please enter payment type name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please select a type")]
        public int Type { get; set; }
        public int? CreatedBy { get; set; }
    }
}