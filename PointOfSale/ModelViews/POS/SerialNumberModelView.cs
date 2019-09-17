using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.POS
{
    public class SerialNumberModelView
    {
        [Required(ErrorMessage = "Please enter serial number")]
        [Remote("SerialNumberExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This number already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string SerialNumber { get; set; }
    }
}