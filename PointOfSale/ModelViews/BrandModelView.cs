using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class BrandModelView
    {
        public int? BrandId { get; set; }
        [Required(ErrorMessage="Please Enter Brand Name")]
        [StringLength(50,MinimumLength=1, ErrorMessage="Invalid")]
        [Remote("BrandNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Brand Name Already Exist !!!")]
        public string BrandName { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> Status { get; set; }
    }
}