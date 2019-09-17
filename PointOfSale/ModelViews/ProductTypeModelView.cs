using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class ProductTypeModelView
    {
        public int? ProductTypeId { get; set; }
        [Required(ErrorMessage = "Please Enter Product Type Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("ProductTypeExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This Type Already Exist !!!")]
        public string ProductTypeName { get; set; }
    }
}