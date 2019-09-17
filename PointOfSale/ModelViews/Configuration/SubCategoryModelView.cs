using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Configuration
{
    public class SubCategoryModelView
    {
        public int? SubCategoryId { get; set; }
        [Required(ErrorMessage ="Please select a category")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter sub category name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("SubCategoryNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Sub category name already exist !!!")]
        public string Name { get; set; }
        public int? RestrictionId { get; set; }
        public int CreatedBy { get; set; }
    }
}