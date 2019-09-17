using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class PaymentMethodModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter method name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("PaymentMethodNameExists", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Method name already exsits")]
        public string MethodName { get; set; }
        [Required(ErrorMessage ="Please select one type")]
        public int Type { get; set; }
        public int? CreatedBy { get; set; }
    }
}