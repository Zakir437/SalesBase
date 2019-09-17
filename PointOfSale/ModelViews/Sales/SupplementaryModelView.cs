using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Sales
{
    public class SupplementaryModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter item name")]
        [Remote("SupplementaryNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This name already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        public bool IsRequestConditionInfo { get; set; }
        public int CreatedBy { get; set; }
    }
}