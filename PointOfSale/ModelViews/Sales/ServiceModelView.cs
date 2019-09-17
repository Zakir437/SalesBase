using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Sales
{
    public class ServiceModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter service name")]
        [Remote("ServiceNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This name already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        [Required]
        public decimal ServiceCharge { get; set; }
        public int CreatedBy { get; set; }
    }
}