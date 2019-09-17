using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Configuration
{
    public class TagModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter tag name")]
        [Remote("TagNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This name already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string TagName { get; set; }
        public int? CreatedBy { get; set; }
    }
}