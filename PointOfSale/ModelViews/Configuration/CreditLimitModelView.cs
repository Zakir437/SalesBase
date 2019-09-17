using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Configuration
{
    public class CreditLimitModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter limit name")]
        [Remote("CreditLimitNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This name already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter limit amount")]
        public decimal Limit { get; set; }
        public int CreatedBy { get; set; }
    }
}