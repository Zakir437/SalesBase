using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class CountryOfRegionModelView
    {
        public int? COOId { get; set; }
        [Required(ErrorMessage = "Please Enter Country of Region Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("COONameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Country of Region Name Already Exist !!!")]
        public string COOName { get; set; }
        [Required(ErrorMessage = "Please Enter Culture Name")]
        public string CultureName { get; set; }
    }
}