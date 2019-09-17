using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class CategoryModelView
    {
        public int? CategoryId { get; set; }
        [Required(ErrorMessage="Please Enter Category Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("CategoryNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Category Name Already Exist !!!")]
        public string CategoryName { get; set; }
        public int? RestrictionId { get; set; }
        public int CreatedBy { get; set; }
    }
}